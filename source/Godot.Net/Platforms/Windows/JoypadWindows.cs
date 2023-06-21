namespace Godot.Net.Platforms.Windows;

#pragma warning disable IDE0052 // TODO - Remover

public class JoypadWindows
{
    private readonly nint hWnd;

    public JoypadWindows(nint hWnd) => this.hWnd = hWnd;

    #pragma warning disable CA1822 // TODO - REMOVE
    public void ProcessJoypads()
    {
        // TODO
    }
    #pragma warning restore CA1822 // TODO - REMOVE
}
