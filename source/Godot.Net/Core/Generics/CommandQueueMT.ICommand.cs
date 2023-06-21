namespace Godot.Net.Core.Generics;

public partial class CommandQueueMT
{
    public interface ICommand
    {
        public void Call();
		public void Post();
	}
}
