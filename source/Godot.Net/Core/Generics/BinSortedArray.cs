namespace Godot.Net.Core.Generics;

public class BinSortedArray<T>
{
    private readonly PagedArray<T> array     = new();
    private readonly List<uint>    binLimits = new();

    public BinSortedArray() => this.binLimits.Add(0);

    public void SetPagePool(PagedArrayPool<T> pagePool) => this.array.SetPagePool(pagePool);
}
