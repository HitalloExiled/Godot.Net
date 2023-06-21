namespace Godot.Net.Core;

public partial class Scheduler
{
    private record Entry(
        Func<object?>                 Task,
        TaskCompletionSource<object?> CompletionSource,
        CancellationToken             CancellationToken
    );
}
