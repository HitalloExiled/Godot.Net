namespace Godot.Net.ThirdParty.Icu;

using Godot.Net.ThirdParty.Icu.Native;

public unsafe class BreakIterator : IDisposable
{
    private readonly UBreakIterator* pBreakIterator;
    private bool disposed;

    public int Current => IcuNative.Singleton.UbrkCurrent(this.pBreakIterator);

    public BreakIterator(UBreakIteratorType type, string locale, string	text)
    {
        fixed (char* pLocale = locale)
        fixed (char* pText   = text)
        {
            UErrorCode errorCode;
            this.pBreakIterator = IcuNative.Singleton.UbrkOpen(type, pLocale, pText, text.Length, &errorCode);

            IcuException.ThrowIfError(errorCode);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            IcuNative.Singleton.UbrkClose(this.pBreakIterator);
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int GetRuleStatus() =>
        IcuNative.Singleton.UbrkGetRuleStatus(this.pBreakIterator);

    public int Next() =>
        IcuNative.Singleton.UbrkNext(this.pBreakIterator);
}
