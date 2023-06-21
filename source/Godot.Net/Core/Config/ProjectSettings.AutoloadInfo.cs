namespace Godot.Net.Core.Config;
#pragma warning disable IDE0052, CS0414 // TODO Remove

public partial class ProjectSettings
{
    public record AutoloadInfo
    {
		public bool   IsSingleton { get; set; }
		public string Name        { get; set; } = "";
		public string Path        { get; set; } = "";
	};
}
