namespace Godot.Net.Core.Generics;

public partial class CommandQueueMT
{
    public record SyncSemaphore
    {
        public Semaphore Semaphore { get; } = new(0, 1);

        public bool InUse { get; set; }
    }
}
