namespace Godot.Net.Platforms.Windows;

using Godot.Net.Core.Error;
using Godot.Net.Main;

#pragma warning disable IDE0060 // Todo - Remove

public class GodotWindows
{
    // platform\windows\godot_windows.cpp[191:231] - Todo Analyze
    private static OSWindows os = null!;

    public static int Initialize(string[] args)
    {
        os = new OSWindows();
        // platform\windows\godot_windows.cpp[154:162] - Todo

        var error = Main.Setup(Environment.ProcessPath!, args, true);

        if (error != Error.OK)
        {
            return error == Error.ERR_HELP ? 0 : 255;
        }

        if (Main.Start())
        {
            os.Run();
        }

        Main.Cleanup();

        return OSWindows.Singleton.ExitCode;
    }
}
