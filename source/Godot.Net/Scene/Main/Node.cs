#define DEBUG_ENABLED
#define TOOLS_ENABLED

namespace Godot.Net.Scene.Main;

using System;
using System.Runtime.CompilerServices;
using Godot.Net.Core.Config;
using Godot.Net.Core.Object;

#pragma warning disable CS0649, IDE0044, IDE0044, IDE0060, CA1822 // TODO - REMOVE

public partial class Node : GodotObject
{
    private static int orphanNodeCount;

    private readonly List<Node>                    children = new();
    private readonly Dictionary<string, GroupData> grouped = new();
    private readonly List<Node>                    owned = new();


    #region private fields
    private int             blocked;
    private int             depth = -1;
    private bool            inConstructor = true;
    private int             index = -1;
    private int             internalChildrenBack;
    private int             internalChildrenFront;
    private string          name;
    private Node?           owner;
    private bool            parentOwned;
    private bool            physicsProcess;
    private bool            physicsProcessInternal;
    private bool            process;
    private bool            processInternal;
    private ProcessModeKind processMode;
    private Node?           processOwner;
    private bool            readyFirst = true;
    private bool            readyNotified;
    private string?         sceneFilePath;
    private SceneTree?      tree;
    private bool            unhandledKeyInput;
    private bool            uniqueNameInOwner;
    private Viewport?       viewport;
    #endregion private fields

    public required bool PostInitialize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        init
        {
            if (value)
            {
                this.Notification(NotificationKind.NOTIFICATION_POSTINITIALIZE);
            }
        }
    }

    #region public readonly properties
    public IEnumerable<Node> Children        => this.children.AsEnumerable();
    public bool              HasViewport     => this.viewport != null;
    public bool              IsInsideTree    { get; private set; }
    public bool              IsInternalBack  => this.Parent != null && this.index >= this.Parent.children.Count - this.Parent.internalChildrenBack;
    public bool              IsInternalFront => this.Parent != null && this.index < this.Parent.internalChildrenFront;
    public Node?             Parent          { get; private set; }
    public Viewport          Viewport        => this.viewport ?? throw new NullReferenceException();
    #endregion public readonly properties

    #region public properties
    public bool CanProcess { get; set; }

    public string Name
    {
        get => this.name;
        set
        {
            if (ERR_FAIL_COND(string.IsNullOrEmpty(value)))
            {
                return;
            }

            if (this.uniqueNameInOwner && this.owner != null)
            {
                this.ReleaseUniqueNameInOwner();
            }

            this.name = value;

            this.Parent?.ValidateChildName(this, true);

            if (this.uniqueNameInOwner && this.owner != null)
            {
                this.AcquireUniqueNameInOwner();
            }

            this.PropagateNotification(NotificationKind.NOTIFICATION_PATH_RENAMED);

            if (this.IsInsideTree)
            {
                Renamed?.Invoke();
                this.Tree.NotifyNodeRenamed(this);
                this.Tree.NotifyTreeChanged();
            }
        }
    }

    public Node? Owner
    {
        get => this.owner;
        set
        {
            if (this.owner != null)
            {
                if (this.uniqueNameInOwner)
                {
                    this.ReleaseUniqueNameInOwner();
                }

                this.owner.owned.Remove(this);

                this.owner = null;
            }

            if (ERR_FAIL_COND(value == this))
            {
                return;
            }

            if (value == null)
            {
                return;
            }

            var check = this.Parent;
            var ownerValid = false;

            while (check != null)
            {
                if (check == value)
                {
                    ownerValid = true;
                    break;
                }

                check = check.Parent;
            }

            if (ERR_FAIL_COND(!ownerValid))
            {
                return;
            }

            this.SetOwnerNocheck(value);

            if (this.uniqueNameInOwner)
            {
                this.AcquireUniqueNameInOwner();
            }
        }
    }

    public double ProcessDeltaTime => this.tree != null ? this.tree.ProcessTime : 0;

    public bool ProcessInternal
    {
        get => this.processInternal;
        set
        {
            if (this.processInternal == value)
            {
                return;
            }

            this.processInternal = value;

            if (this.processInternal)
            {
                this.AddToGroup("_process_internal", false);
            }
            else
            {
                this.RemoveFromGroup("_process_internal");
            }
        }
    }

    public ProcessModeKind ProcessMode
    {
        get => this.processMode;
        set
        {
            if (this.processMode == value)
            {
                return;
            }

            if (!this.IsInsideTree)
            {
                this.processMode = value;
                return;
            }

            var prevCanProcess = this.CanProcess;
            var prevEnabled = this.IsEnabled();

            if (value == ProcessModeKind.PROCESS_MODE_INHERIT)
            {
                if (this.Parent != null)
                {
                    this.processOwner = this.Parent.processOwner;
                }
                else
                {
                    ERR_FAIL_MSG("The root node can't be set to Inherit process mode.");
                    return;
                }
            }
            else
            {
                this.processOwner = this;
            }

            this.processMode = value;

            var nextCanProcess = this.CanProcess;
            var nextEnabled    = this.IsEnabled();

            NotificationKind pauseNotification = default;

            if (prevCanProcess && !nextCanProcess)
            {
                pauseNotification = NotificationKind.NOTIFICATION_PAUSED;
            }
            else if (!prevCanProcess && nextCanProcess)
            {
                pauseNotification = NotificationKind.NOTIFICATION_UNPAUSED;
            }

            NotificationKind enabledNotification = default;

            if (prevEnabled && !nextEnabled)
            {
                enabledNotification = NotificationKind.NOTIFICATION_DISABLED;
            }
            else if (!prevEnabled && nextEnabled)
            {
                enabledNotification = NotificationKind.NOTIFICATION_ENABLED;
            }

            this.PropagateProcessOwner(this.processOwner, pauseNotification, enabledNotification);

            #if TOOLS_ENABLED
            // This is required for the editor to update the visibility of disabled nodes
            // It's very expensive during runtime to change, so editor-only
            if (Engine.Singleton.IsEditorHint)
            {
                this.Tree.NotifyTreeProcessModeChanged();
            }
            #endif
        }
    }

    public bool ProcessUnhandledKeyInput
    {
        get => this.unhandledKeyInput;
        set
        {
            if (value == this.unhandledKeyInput)
            {
                return;
            }

            this.unhandledKeyInput = value;

            if (!this.IsInsideTree)
            {
                return;
            }

            if (value)
            {
                this.AddToGroup("_vp_unhandled_key_input" + this.Viewport.InstanceId);
            }
            else
            {
                this.RemoveFromGroup("_vp_unhandled_key_input" + this.Viewport.InstanceId);
            }
        }
    }

    public SceneTree Tree
    {
        get => this.tree ?? throw new NullReferenceException();
        set
        {
            var treeChangedA = default(SceneTree);
            var treeChangedB = default(SceneTree);

            //ERR_FAIL_COND(p_scene && parent && !parent.scene); //nobug if both are null

            if (this.tree != null)
            {
                this.PropagateExitTree();

                treeChangedA = this.tree;
            }

            this.tree = value;

            if (this.tree != null)
            {
                this.PropagateEnterTree();
                if (this.Parent == null || this.Parent.readyNotified)
                {
                    // No parent (root) or parent ready
                    this.PropagateReady(); //reverse_notification(NOTIFICATION_READY);
                }

                treeChangedB = this.tree;
            }

            treeChangedA?.NotifyTreeChanged();
            treeChangedB?.NotifyTreeChanged();
        }
    }

    public bool UniqueNameInOwner
    {
        get => this.uniqueNameInOwner;
        set
        {
            if (this.uniqueNameInOwner == value)
            {
                return;
            }

            if (this.uniqueNameInOwner && this.owner != null)
            {
                this.ReleaseUniqueNameInOwner();
            }
            this.uniqueNameInOwner = value;

            if (this.uniqueNameInOwner && this.owner != null)
            {
                this.AcquireUniqueNameInOwner();
            }

            this.UpdateConfigurationWarnings();
        }
    }
    #endregion public properties

    #region events
    public event Action<Node>? ChildEnteredTree;
    public event Action<Node>? ChildExitingTree;
    public event Action?       Ready;
    public event Action?       Renamed;
    public event Action?       TreeEntered;
    public event Action?       TreeExiting;
    #endregion events

    public Node()
    {
        this.name       = this.GetType().Name;
        orphanNodeCount++;
    }

    #region private static methods
    private static string GetNameNumSeparator() => GLOBAL_GET<int>("editor/naming/node_name_num_separator") switch
    {
        0 => "",
        1 => " ",
        2 => "_",
        3 => "-",
        _ => " ",
    };

    private static string IncreaseNumericString(string s)
    {
        var res = s.ToCharArray();
        var carry = res.Length > 0;

        for (var i = res.Length - 1; i >= 0; i--)
        {
            if (!carry)
            {
                break;
            }
            var n = s[i];
            if (n == '9')
            { // keep carry as true: 9 + 1
                res[i] = '0';
            }
            else
            {
                res[i] = char.Parse((int.Parse(s[i].ToString()) + 1).ToString());
                carry  = false;
            }
        }

        if (carry)
        {
            res = res.Prepend('1').ToArray();
        }

        return new string(res);
    }
    #endregion private static methods

    #region private methods
    private void AcquireUniqueNameInOwner() => throw new NotImplementedException();

    private void AddChildNocheck(Node child, string name)
    {
        //add a child node quickly, without name validation

        child.name = name;
        child.index = this.children.Count;
        this.children.Add(child);
        child.Parent = this;

        if (this.internalChildrenBack > 0)
        {
            this.MoveChild(child, this.children.Count - this.internalChildrenBack - 1);
        }
        child.Notification(NotificationKind.NOTIFICATION_PARENTED);

        if (this.tree != null)
        {
            child.tree = this.tree;
        }

        /* Notify */
        //recognize children created in this node constructor
        child.parentOwned = this.inConstructor;
        this.AddChildNotify(child);
    }

    private bool IsEnabled()
    {
        var processMode = this.processMode == ProcessModeKind.PROCESS_MODE_INHERIT
            ? this.processOwner == null
                ? ProcessModeKind.PROCESS_MODE_PAUSABLE
                : this.processOwner.ProcessMode
            : this.processMode;

        return processMode != ProcessModeKind.PROCESS_MODE_DISABLED;
    }

    private void GenerateSerialChildName(Node child, ref string name)
    {
        if (name == default)
        {
            // No name and a new name is needed, create one.

            name = child.GetType().Name;
        }

        //quickly test if proposed name exists

        var exists = this.children.Any(x => x != child && x.Name == child.Name);

        if (!exists)
        {
            return; //if it does not exist, it does not need validation
        }

        // Extract trailing number
        var nameString = name;

        var nums = "";
        for (var i = nameString.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(nameString[i]))
            {
                nums = nameString[i] + nums;
            }
            else
            {
                break;
            }
        }

        var nnsep = GetNameNumSeparator();
        var nameLastIndex = nameString.Length - nnsep.Length - nums.Length;

        // Assign the base name + separator to name if we have numbers preceded by a separator
        if (nums.Length > 0 && nameString.Substring(nameLastIndex, nnsep.Length) == nnsep)
        {
            nameString = nameString[..(nameLastIndex + nnsep.Length)];
        }
        else
        {
            nums = "";
        }

        while (true)
        {
            var attempt = nameString + nums;
            exists = this.children.Any(x => x != child && x.Name == attempt);

            if (!exists)
            {
                name = attempt;
                return;
            }
            else
            {
                if (nums.Length == 0)
                {
                    // Name was undecorated so skip to 2 for a more natural result
                    nums = "2";
                    nameString += nnsep; // Add separator because nums.Length > 0 was false
                }
                else
                {
                    nums = IncreaseNumericString(nums);
                }
            }
        }
    }

    private void PropagateNotification(NotificationKind notification)
    {
        this.blocked++;
        this.Notification(notification);

        for (var i = 0; i < this.children.Count; i++)
        {
            this.children[i].PropagateNotification(notification);
        }
        this.blocked--;
    }

    private void MoveChild(Node child, int index, bool ignoreEnd = false)
    {
        if (ERR_FAIL_COND_MSG(this.blocked > 0, "Parent node is busy setting up children, `move_child()` failed. Consider using `move_child.call_deferred(child, index)` instead (or `popup.call_deferred()` if this is from a popup)."))
        {
            return;
        }

        // Specifying one place beyond the end
        // means the same as moving to the last index
        if (!ignoreEnd)
        { // ignoreEnd is a little hack to make back internal children work properly.
            if (child.IsInternalFront)
            {
                if (index == this.internalChildrenFront)
                {
                    index--;
                }
            }
            else if (child.IsInternalBack)
            {
                if (index == this.children.Count)
                {
                    index--;
                }
            }
            else
            {
                if (index == this.children.Count - this.internalChildrenBack)
                {
                    index--;
                }
            }
        }

        if (child.index == index)
        {
            return; //do nothing
        }

        var motionFrom = Math.Min(index, child.index);
        var motionTo   = Math.Max(index, child.index);

        this.children.RemoveAt(child.index);
        this.children.Insert(index, child);

        this.tree?.NotifyTreeChanged();

        this.blocked++;
        //new pos first
        for (var i = motionFrom; i <= motionTo; i++)
        {
            this.children[i].index = i;
        }
        // notification second
        this.MoveChildNotify(child);
        for (var i = motionFrom; i <= motionTo; i++)
        {
            this.children[i].Notification(NotificationKind.NOTIFICATION_MOVED_IN_PARENT);
        }
        child.PropagateGroupsDirty();

        this.blocked--;
    }

    private void PropagateEnterTree()
    {
        // this needs to happen to all children before any enter_tree

        if (this.Parent != null)
        {
            this.tree  = this.Parent.Tree;
            this.depth = this.Parent.depth + 1;
        }
        else
        {
            this.depth = 1;
        }

        this.viewport = this as Viewport;

        if (this.viewport == null && this.Parent != null)
        {
            this.viewport = this.Parent.viewport;
        }

        this.IsInsideTree = true;

        foreach (var item in this.grouped)
        {
            item.Value.Group = this.Tree.AddToGroup(item.Key, this);
        }

        this.Notification(NotificationKind.NOTIFICATION_ENTER_TREE);

        // GDVIRTUAL_CALL(_enter_tree);

        TreeEntered?.Invoke();

        this.Tree.NotifyNodeAdded(this);

        this.Parent?.ChildEnteredTree?.Invoke(this);

        this.blocked++;
        //block while adding children

        for (var i = 0; i < this.children.Count; i++)
        {
            if (!this.children[i].IsInsideTree)
            { // could have been added in enter_tree
                this.children[i].PropagateEnterTree();
            }
        }

        this.blocked--;

#if DEBUG_ENABLED
        // TODO SceneDebugger.AddToCache(sceneFilePath, this);
#endif
        // enter groups
    }

    private void PropagateExitTree()
    {
        //block while removing children

        #if DEBUG_ENABLED
        if (!string.IsNullOrEmpty(this.sceneFilePath))
        {
            // Only remove if file path is set (optimization).
            // TODO SceneDebugger.RemoveFromCache(this.sceneFilePath, this);
        }
        #endif
        this.blocked++;

        for (var i = this.children.Count - 1; i >= 0; i--)
        {
            this.children[i].PropagateExitTree();
        }

        this.blocked--;

        // TODO GDVIRTUAL_CALL(_exit_tree);

        TreeExiting?.Invoke();

        this.Notification(NotificationKind.NOTIFICATION_EXIT_TREE, true);

        this.tree?.NotifyNodeRemoved(this);

        this.Parent?.ChildExitingTree?.Invoke(this);

        // exit groups
        foreach (var item in this.grouped)
        {
            this.tree?.RemoveFromGroup(item.Key, this);
            item.Value.Group = null;
        }

        this.viewport = null;

        this.tree?.NotifyTreeChanged();

        this.IsInsideTree = false;
        this.readyNotified = false;
        this.depth = -1;
    }
    private void PropagateGroupsDirty() => throw new NotImplementedException();

    private void PropagateProcessOwner(Node? owner, NotificationKind pauseNotification, NotificationKind enabledNotification)
    {
        this.processOwner = owner;

        if (pauseNotification != default)
        {
            this.Notification(pauseNotification);
        }

        if (enabledNotification != default)
        {
            this.Notification(enabledNotification);
        }

        foreach (var c in this.children)
        {
            if (c.processMode == ProcessModeKind.PROCESS_MODE_INHERIT)
            {
                c.PropagateProcessOwner(owner, pauseNotification, enabledNotification);
            }
        }
    }

    private void PropagateReady()
    {
        this.readyNotified = true;
        this.blocked++;
        for (var i = 0; i < this.children.Count; i++)
        {
            this.children[i].PropagateReady();
        }
        this.blocked--;

        this.Notification(NotificationKind.NOTIFICATION_POST_ENTER_TREE);

        if (this.readyFirst)
        {
            this.readyFirst = false;
            this.Notification(NotificationKind.NOTIFICATION_READY);
            Ready?.Invoke();
        }
    }

    private void ReleaseUniqueNameInOwner() => throw new NotImplementedException();

    private void SetOwnerNocheck(Node value)
    {
        if (this.owner == value)
        {
            return;
        }

        if (ERR_FAIL_COND(this.owner != null))
        {
            return;
        }

        this.owner = value;
        this.owner.owned.Add(this);

        this.OwnerChangedNotify();
    }

    private void ValidateChildName(Node child, bool forceHumanReadable)
    {
        /* Make sure the name is unique */

        if (forceHumanReadable)
        {
            //this approach to autoset node names is human readable but very slow

            var name = child.Name;
            this.GenerateSerialChildName(child, ref name);
            child.Name = name;
        }
        else
        {
            //this approach to autoset node names is fast but not as readable
            //it's the default and reserves the '@' character for unique names.

            var unique = true;

            if (child.Name == default)
            {
                //new unique name must be assigned
                unique = false;
            }
            else
            {
                unique = !this.children.Any(x => x.Name == this.name);
            }

            if (!unique)
            {
                if (ERR_FAIL_COND(this.children.Count != 0))
                {
                    return;
                }

                var name = "@" + child.name + "@" + this.children.Count;

                child.Name = name;
            }
        }
    }


    #endregion private methods

    #region protected virtual methods
    protected void UpdateConfigurationWarnings()
    {
        #if TOOLS_ENABLED
        if (!this.IsInsideTree)
        {
            return;
        }
        if (this.Tree.EditedSceneRoot != null && (this.Tree.EditedSceneRoot == this || this.Tree.EditedSceneRoot.IsAncestorOf(this)))
        {
            this.Tree.NotifyNodeConfigurationWarningChanged(this);
        }
        #endif
    }
    #endregion protected virtual methods

    #region protected virtual methods
    protected virtual void AddChildNotify(Node child)
    {
        // to be used when not wanted
    }

    protected virtual void MoveChildNotify(Node child)
    {
        // to be used when not wanted
    }

    protected virtual void OwnerChangedNotify()
    {
        // to be used when not wanted
    }

    protected virtual void RemoveChildNotify(Node child)
    {
        // to be used when not wanted
    }
    #endregion protected virtual methods

    #region public methods
    public void AddChild(Node? child, bool forceReadableName = default, InternalMode internalMode = default)
    {
        if (ERR_FAIL_NULL(child))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(child == this, $"Can't add child '{child!.Name}' to itself." )) // adding to itself!
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(child.Parent != null, $"Can't add child '{child.Name}' to '{this.Name}', already has a parent '{child.Parent?.Name}'.")) //Fail if node has a parent
        {
            return;
        }

        #if DEBUG_ENABLED
        if (ERR_FAIL_COND_MSG(child.IsAncestorOf(this), $"Can't add child '{child.Name}' to '{this.Name}' as it would result in a cyclic dependency since '{child.Name}' is already a parent of '{this.Name}'."))
        {
            return;
        }
        #endif
        if (ERR_FAIL_COND_MSG(this.blocked > 0, $"Parent node is busy setting up children, `{nameof(AddChild)}` failed. Consider using ``{nameof(AddChildDeferred)}`` instead."))
        {
            return;
        }

        this.ValidateChildName(child, forceReadableName);
        this.AddChildNocheck(child, child.Name);

        if (internalMode == InternalMode.INTERNAL_MODE_FRONT)
        {
            this.MoveChild(child, this.internalChildrenFront);
            this.internalChildrenFront++;
        }
        else if (internalMode == InternalMode.INTERNAL_MODE_BACK)
        {
            if (this.internalChildrenBack > 0)
            {
                this.MoveChild(child, this.children.Count - 1, true);
            }

            this.internalChildrenBack++;
        }
    }

    public void AddChildDeferred(Node? chield) => throw new NotImplementedException();

    // void Node::add_to_group(const StringName &p_identifier, bool p_persistent)
    public void AddToGroup(string identifier, bool persistent = default)
    {
        if (ERR_FAIL_COND(string.IsNullOrEmpty(identifier)))
        {
            return;
        }

        if (this.grouped.ContainsKey(identifier))
        {
            return;
        }

        var gd = new GroupData();

        if (this.tree != null)
        {
            gd.Group = this.Tree.AddToGroup(identifier, this);
        }

        gd.Persistent = persistent;

        this.grouped.Add(identifier, gd);
    }

    public bool CanProcessNotification(NotificationKind notification) => notification switch
    {
        NotificationKind.NOTIFICATION_PHYSICS_PROCESS          => this.physicsProcess,
        NotificationKind.NOTIFICATION_PROCESS                  => this.process,
        NotificationKind.NOTIFICATION_INTERNAL_PROCESS         => this.processInternal,
        NotificationKind.NOTIFICATION_INTERNAL_PHYSICS_PROCESS => this.physicsProcessInternal,
        _ => true
    };

    public Node? GetChild(int index, bool includeInternal = default)
    {
        if (includeInternal)
        {
            if (index < 0)
            {
                index += this.children.Count;
            }
            return ERR_FAIL_INDEX_V(index, this.children.Count) ? null : this.children[index];
        }
        else
        {
            if (index < 0)
            {
                index += this.children.Count - this.internalChildrenFront - this.internalChildrenBack;
            }

            if (ERR_FAIL_INDEX_V(index, this.children.Count - this.internalChildrenFront - this.internalChildrenBack))
            {
                return null;
            }

            index += this.internalChildrenFront;

            return this.children[index];
        }
    }

    public int GetChildCount(bool includeInternal = false) =>
        includeInternal
            ? this.children.Count
            : this.children.Count - this.internalChildrenFront - this.internalChildrenBack;

    public int GetIndex(bool includeInternal = true) =>
        // p_include_internal = false doesn't make sense if the node is internal.
        ERR_FAIL_COND_V_MSG(!includeInternal && (this.IsInternalFront || this.IsInternalBack), "Node is internal. Can't get index with 'includeInternal' being false.")
            ? -1
            : this.Parent != null && !includeInternal ? this.index - this.Parent.internalChildrenFront : this.index;

    public bool IsAncestorOf(Node node)
    {
        if (ERR_FAIL_NULL_V(node))
        {
            return false;
        }

        var p = node.Parent;

        while (p != null)
        {
            if (p == this)
            {
                return true;
            }
            p = p.Parent;
        }

        return false;
    }

    // bool Node::is_in_group(const StringName &p_identifier)
    public bool IsInGroup(string identifier) => this.grouped.ContainsKey(identifier);

    // void Node::remove_from_group(const StringName &p_identifier)
    public void RemoveFromGroup(string identifier)
    {
        if (!this.grouped.ContainsKey(identifier))
        {
            return;
        }

        this.tree?.RemoveFromGroup(identifier, this);

        this.grouped.Remove(identifier);
    }
    #endregion public methods
}
