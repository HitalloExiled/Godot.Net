namespace Godot.Net.Drivers.GLES3.Api;

public class LoadSymbolException : Exception
{
    public LoadSymbolException(string symbol) : base($"Failed to load symbol {symbol}")
    { }
}
