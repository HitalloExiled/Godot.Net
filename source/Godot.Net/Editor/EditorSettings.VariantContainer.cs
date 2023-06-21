namespace Godot.Net.Editor;
public partial class EditorSettings
{
    private record VariantContainer
    {
		public bool    HasDefaultValue  { get; set; }
		public bool    HideFromEditor   { get; set; }
		public object? Initial          { get; set; }
		public int     Order            { get; set; }
		public bool    RestartIfChanged { get; set; }
		public bool    Save             { get; set; }
		public object  Variant          { get; set; }

		public VariantContainer(object variant, int order)
        {
            this.Variant = variant;
            this.Order   = order;
        }
	}
}
