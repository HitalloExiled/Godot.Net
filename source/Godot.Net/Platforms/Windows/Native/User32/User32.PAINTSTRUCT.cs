namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    public unsafe struct PAINTSTRUCT
    {
        public HDC        hdc;
        public BOOL       fErase;
        public RECT       rcPaint;
        public BOOL       fRestore;
        public BOOL       fIncUpdate;
        public fixed byte rgbReserved[32];
    }
}
