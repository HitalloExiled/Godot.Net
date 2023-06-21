namespace Godot.Net.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ClonableAttribute : Attribute
{
    public bool Deep { get; set; }

    public ClonableAttribute(bool deep = false) =>
        this.Deep = deep;
}
