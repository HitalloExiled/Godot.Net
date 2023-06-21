namespace Godot.Net.Core.Generics;

public partial class CommandQueueMT
{
    public readonly struct SyncCommand : ICommand
    {
        private readonly Action        action;
        private readonly SyncSemaphore syncSemaphore;

        public SyncCommand(Action action, SyncSemaphore syncSemaphore)
        {
            this.action        = action;
            this.syncSemaphore = syncSemaphore;
        }

        public void Call() =>
            this.action.Invoke();

        public readonly void Post() =>
            this.syncSemaphore.Semaphore.Release();
    }
}
