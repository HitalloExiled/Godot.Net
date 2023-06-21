namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;

public partial class TabBar : Control
{
    public Action<int>? TabChanged;
    public Action<int>? TabSelected;
    public Action<int>? TabButtonPressed;

    private readonly List<Tab> tabs = new();

    #region public readonly properties
    public bool OffsetButtonsVisible => throw new NotImplementedException();
    public int  TabCount             => this.tabs.Count;
    #endregion public readonly properties

    #region public properties
    public bool ClipTabs
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public AlignmentMode TabAlignment
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    #endregion public properties

    public TabBar()
    {
        this.SetSize(new(this.Size.X, this.MinimumSize.Y));
        MouseExited += this.OnMouseExited;
    }

    private void OnMouseExited() => throw new NotImplementedException();

    public Rect2<RealT> GetTabRect(int tab) => throw new NotImplementedException();
}
