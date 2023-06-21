namespace Godot.Net.Core.Generics;

using Godot.Net.Core.Object;

public partial class CommandQueueMT : IDisposable
{
    private readonly Mutex      mutex = new();
    private readonly Semaphore? sync;

    private bool disposed;

    private readonly Queue<ICommand> commands = new();

    public CommandQueueMT(bool sync = false)
    {
        if (sync)
        {
            this.sync = new Semaphore(0, 1);
        }
    }

    ~CommandQueueMT()
    {
        // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        this.Dispose(disposing: false);
    }

    private void Flush()
    {
        this.Lock();

        while (this.commands.Count > 0)
        {
            var command = this.commands.Dequeue();

            command.Call();
            command.Post();
        }

        this.Unlock();
	}

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.mutex.Dispose();
                this.sync?.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void FlushAll() =>
        this.Flush();

    public void FlushIfPending()
    {
        if (this.commands.Count > 0)
        {
            this.Flush();
        }
    }

    public void PushAndSync(Action action)
    {
        var ss = new SyncSemaphore();

        this.commands.Enqueue(new SyncCommand(action, ss));

        this.sync?.WaitOne();
        ss.Semaphore.WaitOne();
    }

    public void PushAndRet<T>(Func<T> fn, out Ref<T> ret)
    {
        ret = new();

        var ss = new SyncSemaphore();

        var command = new CommandRet<T>(fn, ss);

        ret = command.Value;

        this.commands.Enqueue(command);

        this.sync?.WaitOne();
        ss.Semaphore.WaitOne();
    }

    public void Lock() =>
        this.mutex.WaitOne();

    public void Unlock() =>
        this.mutex.ReleaseMutex();

    public void WaitAndFlush()
    {
        if (ERR_FAIL_COND(this.sync == null))
        {
            return;
        }

        this.sync!.WaitOne();
        this.Flush();
    }
}
