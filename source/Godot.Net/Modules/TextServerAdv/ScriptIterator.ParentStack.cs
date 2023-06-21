namespace Godot.Net.Modules.TextServerAdv;

using Godot.Net.ThirdParty.Icu;

public partial class ScriptIterator
{
    private record ParentStack
    {
        public int         PairIndex  { get; set; }
        public UScriptCode ScriptCode { get; set; }
    }
}
