namespace Godot.Net.Core.OS;

public class MutexLock : IDisposable
{
    private readonly Mutex mutex;

    private bool disposed;

    public MutexLock(Mutex mutex)
    {
        mutex.WaitOne();

        this.mutex = mutex;
    }

    ~MutexLock() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.mutex.ReleaseMutex();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
