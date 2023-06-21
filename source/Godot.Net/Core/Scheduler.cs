namespace Godot.Net.Core;

public partial class Scheduler
{
    private readonly List<Exception> exceptions = new();
    private readonly Queue<Entry>    queue      = new();

    private async Task<object?> InternalEnqueueAsync(Func<object?> task, CancellationToken cancellationToken)
    {
        var completionSource = new TaskCompletionSource<object?>();
        var entry            = new Entry(task, completionSource, cancellationToken);

        this.queue.Enqueue(entry); // Cant convert Entry<T> to Entry<object>

        return await completionSource.Task;
    }

    public void Clear() => this.queue.Clear();

    public async Task EnqueueAsync(Action task, CancellationToken cancellationToken = default) =>
        await this.InternalEnqueueAsync(() => { task(); return null; }, cancellationToken);

    public async Task<T?> EnqueueAsync<T>(Func<T?> task, CancellationToken cancellationToken = default) =>
        (T?)await this.InternalEnqueueAsync(() => task(), cancellationToken);

    public void Execute()
    {
        this.exceptions.Clear();

        while (this.queue.TryDequeue(out var entry))
        {
            if (!entry.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    entry.CompletionSource.SetResult(entry.Task());
                }
                catch (Exception exception)
                {
                    entry.CompletionSource.SetException(exception);
                    this.exceptions.Add(exception);
                }
            }
            else
            {
                entry.CompletionSource.SetCanceled();
            }
        }

        if (this.exceptions.Count != 0)
        {
            throw new AggregateException(this.exceptions.ToArray());
        }
    }
}
