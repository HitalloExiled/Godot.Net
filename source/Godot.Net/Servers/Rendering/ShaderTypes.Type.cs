namespace Godot.Net.Servers.Rendering;

using System.Collections.Generic;

#pragma warning disable IDE0044, IDE0052, CS0649 // TODO Remove

public partial class ShaderTypes
{
    private record Type
    {
        public Dictionary<string, ShaderLanguage.FunctionInfo> Functions { get; init; } = new();
        public ShaderLanguage.ModeInfo[]                       Modes     { get; init; } = Array.Empty<ShaderLanguage.ModeInfo>();
    }
}
