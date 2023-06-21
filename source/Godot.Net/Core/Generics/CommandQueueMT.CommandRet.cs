namespace Godot.Net.Core.Generics;

using Godot.Net.Core.Object;

public partial class CommandQueueMT
{
    public class CommandRet<T> : ICommand
    {
        private readonly Func<T>        func;
        private readonly SyncSemaphore syncSemaphore;

        public Ref<T> Value { get; } = new();

        public CommandRet(Func<T> func, SyncSemaphore syncSemaphore)
        {
            this.syncSemaphore = syncSemaphore;
            this.func          = func;
        }

        public void Call() =>
            this.Value.Value = this.func();

        public void Post() =>
            this.syncSemaphore.Semaphore.Release();
    }
}
