namespace Godot.Net.Modules.TextServerAdv;

using HarfBuzzSharp;

public partial class ScriptIterator
{
    public record struct ScriptRange(int Start, int End, Script Script);
}
