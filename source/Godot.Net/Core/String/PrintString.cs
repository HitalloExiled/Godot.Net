namespace Godot.Net.Core.String;

using OS = OS.OS;

public record PrintHandlerList(object UserData, PrintHandlerFunc ErrorHandler);

public static class PrintString
{
    private static readonly object                       padlock          = new();
    private static readonly LinkedList<PrintHandlerList> printHandlerList = new();

    public delegate void PrintHandlerFunc(object data, string content, bool error, bool rich);

    public static void PrintLine(string content)
    {
        if (!CoreGlobals.PrintLineEnabled)
        {
            return;
        }

        OS.Singleton.Print($"{content}\n");

        lock (padlock)
        {
            foreach (var item in printHandlerList)
            {
                item.ErrorHandler.Invoke(item.UserData, content, false, false);
            }
        }
    }

    public static void PrintVerbose(string message)
    {
        if (OS.Singleton.IsStdoutVerbose)
        {
            PrintLine(message);
        }
    }
}
