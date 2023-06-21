namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum QueryCounterTarget
{
    Timestamp = 0x8E28,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    TimestampEXT = 0x8E28,
}
