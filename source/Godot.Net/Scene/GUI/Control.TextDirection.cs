#define TOOLS_ENABLED

namespace Godot.Net.Scene.GUI;
using Godot.Net.Servers;

#pragma warning disable IDE0052, CS0067 // TODO Remove

public partial class Control
{
    public enum TextDirection
    {
		TEXT_DIRECTION_AUTO      = TextServer.Direction.DIRECTION_AUTO,
		TEXT_DIRECTION_LTR       = TextServer.Direction.DIRECTION_LTR,
		TEXT_DIRECTION_RTL       = TextServer.Direction.DIRECTION_RTL,
		TEXT_DIRECTION_INHERITED = TextServer.Direction.DIRECTION_INHERITED,
	}
}
