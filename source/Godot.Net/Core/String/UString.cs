namespace Godot.Net.Core.String;

using System.Runtime.CompilerServices;

#pragma warning disable IDE0060

public static class UString
{
    /// <summary>
    /// Use this to mark property names for editor translation.
    /// Often for dynamic properties defined in _get_property_list().
    /// Property names defined directly inside EDITOR_DEF, GLOBAL_DEF, and ADD_PROPERTY macros don't need this.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string PNAME(string value) => value;

    /// <summary>
    /// Similar to PNAME, but to mark groups, i.e. properties with PROPERTY_USAGE_GROUP.
    /// Groups defined directly inside ADD_GROUP macros don't need this.
    /// The arguments are the same as ADD_GROUP. m_prefix is only used for extraction.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string GNAME(string value, string prefix) => value;

    /// <summary>
    /// "Run-time TRanslate". Performs string replacement for internationalization
    /// within a running project. The translation string must be supplied by the
    /// project, as Godot does not provide built-in translations for `RTR()` strings
    /// to keep binary size low. A translation context can optionally be specified to
    /// disambiguate between identical source strings in translations. When
    /// placeholders are desired, use `vformat(RTR("Example: %s"), some_string)`.
    /// If a string mentions a quantity (and may therefore need a dynamic plural form),
    /// use `RTRN()` instead of `RTR()`.
    ///
    /// NOTE: Do not use `RTR()` in editor-only code (typically within the `editor/`
    /// folder). For editor translations, use `TTR()` instead.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string RTR(string text, string? context = default) => text; // TODO

    /// <summary>
    /// "Tools TRanslate". Performs string replacement for internationalization
    /// within the editor. A translation context can optionally be specified to
    /// disambiguate between identical source strings in translations. When
    /// placeholders are desired, use `vformat(TTR("Example: %s"), some_string)`.
    /// If a string mentions a quantity (and may therefore need a dynamic plural form),
    /// use `TTRN()` instead of `TTR()`.
    ///
    /// NOTE: Only use `TTR()` in editor-only code (typically within the `editor/` folder).
    /// For translations that can be supplied by exported projects, use `RTR()` instead.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string TTR(string text, string? context = default) => text;
}
