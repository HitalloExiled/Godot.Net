namespace Godot.Net.Platforms;

using Godot.Net.Core.IO;

// Cover WindowsTerminalLogger

public class TerminalLogger : Logger
{
    public override void Logv(string message, bool error)
    {
        if (ShouldLog(error))
        {
            Console.Write(message);
        }
    }
}
