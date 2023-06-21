namespace Godot.Net.ThirdParty.Icu;

using System.Runtime.InteropServices;
using Godot.Net.Core;
using Godot.Net.ThirdParty.Icu.Native;

public unsafe class BiDi : IDisposable
{
    private readonly UBiDi* pBiDi;

    private nint pPara;
    private bool disposed;

    private BiDi(UBiDi* biDi) =>
        this.pBiDi = biDi;

    public BiDi(int maxLength, int maxRunCount)
    {
        UErrorCode errorCode;

        this.pBiDi = IcuNative.Singleton.UbidiOpenSized(maxLength, maxRunCount, &errorCode);

        IcuException.ThrowIfError(errorCode);
    }

    public static UBiDiDirection GetBaseDirection(string text)
    {
        fixed (char* pText = text)
        {
            return IcuNative.Singleton.UGetBaseDirection(pText, text.Length);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // Dispose managed
            }

            if (this.pPara != default)
            {
                Marshal.FreeHGlobal(this.pPara);
            }

            Marshal.FreeHGlobal((nint)this.pBiDi);

            this.disposed = true;
        }
    }

    public int CountRuns()
    {
        UErrorCode errorCode;

        var result = IcuNative.Singleton.UbidiCountRuns(this.pBiDi, &errorCode);

        IcuException.ThrowIfError(errorCode);

        return result;
    }

    public UBiDiDirection GetVisualRun(int index, out int logicalStart, out int length)
    {
        logicalStart = 0;
        length       = 0;

        fixed (int* pLogicalStart = &logicalStart)
        fixed (int* pLength       = &length)
        {
            return IcuNative.Singleton.UbidiGetVisualRun(this.pBiDi, index, pLogicalStart, pLength);
        }
    }

    public BiDi SetLine(int start, int limit)
    {
        var bidiSize = Marshal.SizeOf<UBiDi>();

        var pLineBidi = (UBiDi*)Marshal.AllocHGlobal(bidiSize);

        UnmanagedUtils.ZeroFill(pLineBidi, bidiSize);

        UErrorCode errorCode;

        IcuNative.Singleton.UbidiSetLine(this.pBiDi, start, limit, pLineBidi, &errorCode);

        IcuException.ThrowIfError(errorCode);

        return new BiDi(pLineBidi);
    }

    public void SetPara(string text, byte paraLevel, byte[]? embeddingLevels)
    {
        if (this.pPara != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(this.pPara);

            this.pPara = IntPtr.Zero;
        }

        ArgumentNullException.ThrowIfNull(text);

        this.pPara = Marshal.StringToHGlobalUni(text);

        fixed(byte* pEmbeddingLevels = embeddingLevels)
        {
            UErrorCode errorCode;

            IcuNative.Singleton.UbidiSetPara(this.pBiDi, (char*)this.pPara, text.Length, paraLevel, pEmbeddingLevels, &errorCode);

            IcuException.ThrowIfError(errorCode);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
