namespace Godot.Net.Core.Generics;

using static Godot.Net.Core.Generics.Common;

#pragma warning disable IDE0044, IDE0052, CS0649, CS0414, IDE0051, CS0169 // TODO Remove

public class PagedArrayPool<T>
{
    private uint[]?  availablePagePool;
    private T[]?     pagePool;
    private uint     pagesAllocated;
    private uint     pagesAvailable;
    private uint     pageSize;
    private SpinLock spinLock;

    public PagedArrayPool(uint pageSize = 4096) =>
        // power of 2 recommended because of alignment with OS page sizes. Even if element is bigger, its still a multiple and get rounded amount of pages
        this.Configure(pageSize);

    public uint PageSizeMask  => this.pageSize - 1;
    public uint PageSizeShift => (uint)GetShiftFromPowerOf2(this.pageSize);

    public void Configure(uint pageSize)
    {
        //sanity check
        if (ERR_FAIL_COND(this.pagePool != null) || ERR_FAIL_COND(pageSize == 0))
        {
            return;
        }

        this.pageSize = NearestPowerOf2Templated(pageSize);
    }
}

public class PagedArray<T>
{
    private ulong              count;
    private uint               maxPagesUsed;
    private T[]?               pageData;
    private uint[]?            pageIds;
    private PagedArrayPool<T>? pagePool;
    private uint               pageSizeMask;
    private uint               pageSizeShift;

    public void SetPagePool(PagedArrayPool<T> pagePool)
    {
        if (ERR_FAIL_COND(this.maxPagesUsed > 0)) //sanity check
        {
            return;
        }

        this.pagePool      = pagePool;
        this.pageSizeMask  = pagePool.PageSizeMask;
        this.pageSizeShift = pagePool.PageSizeShift;
    }
}
