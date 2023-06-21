namespace Godot.Net.Core.Config;

public record VariantContainer
{
    public bool    Basic             { get; set; }
    public bool    HideFromEditor    { get; set; }
    public bool    IgnoreValueInDocs { get; set; }
    public object? Initial           { get; set; }
    public bool    Internal          { get; set; }
    public int     Order             { get; set; }
    public bool    Overridden        { get; set; }
    public bool    Persist           { get; set; }
    public bool    RestartIfChanged  { get; set; }
    public object? Variant           { get; set; }
}
