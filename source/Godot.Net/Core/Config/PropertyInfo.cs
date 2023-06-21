namespace Godot.Net.Core.Config;

public record PropertyInfo(
    VariantType            Type,
    string             Name,
    PropertyHint       Hint       = PropertyHint.PROPERTY_HINT_NONE,
    string?            HintString = null,
    PropertyUsageFlags Usage      = PropertyUsageFlags.PROPERTY_USAGE_DEFAULT,
    string?            ClassName  = null
);
