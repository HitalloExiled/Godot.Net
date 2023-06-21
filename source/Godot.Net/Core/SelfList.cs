namespace Godot.Net.Core;

using System.Runtime.CompilerServices;

public partial class SelfList<T> where T : notnull
{
    public bool              InList => this.Root != null;
    public SelfList<T>?      Next   { get; private set; }
    public SelfList<T>?      Prev   { get; private set; }
    public SelfList<T>.List? Root   { get; private set; }
    public T?                Self   { get; }

    public SelfList() { }
    public SelfList(T self) => this.Self = self;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void RemoveFromList() => this.Root?.Remove(this);
}
