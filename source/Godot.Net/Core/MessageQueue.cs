namespace Godot.Net.Core;

#pragma warning disable IDE0044, IDE0052 // TODO Remove

// <ReAged>
public class MessageQueue
{
    private static readonly object padlock = new();

    public record Message(
        Action Callable,
        bool   ShowError
    );

    public static MessageQueue Singleton { get; private set; } = null!;

    private readonly Queue<Message> messages = new();


    public MessageQueue()
    {
        if (ERR_FAIL_COND_MSG(Singleton != null, "A MessageQueue singleton already exists."))
        {
            return;
        }

        Singleton = this;
    }

    public void Flush()
    {
        lock (padlock)
        {
            while (this.messages.Count > 0)
            {
                var message = this.messages.Dequeue();

                try
                {
                    message.Callable.Invoke();
                }
                catch
                {
                    if (message.ShowError)
                    {
                        ERR_PRINT("Error calling deferred method: " + message.Callable.Method.Name + ".");
                    }
                }
            }
        }
    }

    public void PushCallable(Action callable, bool showError = default)
    {
        lock (padlock)
        {
            this.messages.Enqueue(new(callable, showError));
        }
    }
}
