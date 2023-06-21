namespace Godot.Net.Drivers.GLES3.Api.Enums;

[Flags]
public enum ClearBufferMask : uint
{
    DepthBufferBit   = 0x00000100,
    AccumBufferBit   = 0x00000200,
    StencilBufferBit = 0x00000400,
    ColorBufferBit   = 0x00004000,
}
