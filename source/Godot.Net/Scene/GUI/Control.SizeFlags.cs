namespace Godot.Net.Scene.GUI;
#pragma warning disable IDE0052 // TODO Remove

public partial class Control
{
    [Flags]
    public enum SizeFlags
    {
		SIZE_SHRINK_BEGIN  = 0,
		SIZE_FILL          = 1,
		SIZE_EXPAND        = 2,
		SIZE_SHRINK_CENTER = 4,
		SIZE_SHRINK_END    = 8,
		SIZE_EXPAND_FILL   = SIZE_EXPAND | SIZE_FILL,
	}

}
