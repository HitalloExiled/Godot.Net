namespace Godot.Net.Editor;

using System.Runtime.CompilerServices;

public static class BuiltinFonts
{
    private static readonly string fontsPath = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts");

    public static byte[] DroidSansFallback           { get; }
    public static byte[] DroidSansJapanese           { get; }
    public static byte[] JetBrainsMonoRegular        { get; }
    public static byte[] NotoNaskhArabicUiBold       { get; }
    public static byte[] NotoNaskhArabicUiRegular    { get; }
    public static byte[] NotoSansBengaliUiBold       { get; }
    public static byte[] NotoSansBengaliUiRegular    { get; }
    public static byte[] NotoSansBold                { get; }
    public static byte[] NotoSansDevanagariUiBold    { get; }
    public static byte[] NotoSansDevanagariUiRegular { get; }
    public static byte[] NotoSansGeorgianBold        { get; }
    public static byte[] NotoSansGeorgianRegular     { get; }
    public static byte[] NotoSansHebrewBold          { get; }
    public static byte[] NotoSansHebrewRegular       { get; }
    public static byte[] NotoSansMalayalamUiBold     { get; }
    public static byte[] NotoSansMalayalamUiRegular  { get; }
    public static byte[] NotoSansOriyaUiBold         { get; }
    public static byte[] NotoSansOriyaUiRegular      { get; }
    public static byte[] NotoSansRegular             { get; }
    public static byte[] NotoSansSinhalaUiBold       { get; }
    public static byte[] NotoSansSinhalaUiRegular    { get; }
    public static byte[] NotoSansTamilUiBold         { get; }
    public static byte[] NotoSansTamilUiRegular      { get; }
    public static byte[] NotoSansTeluguUiBold        { get; }
    public static byte[] NotoSansTeluguUiRegular     { get; }
    public static byte[] NotoSansThaiUiBold          { get; }
    public static byte[] NotoSansThaiUiRegular       { get; }

    static BuiltinFonts()
    {
        DroidSansFallback           = LoadFont("DroidSansFallback.woff2");
        DroidSansJapanese           = LoadFont("DroidSansJapanese.woff2");
        JetBrainsMonoRegular        = LoadFont("JetBrainsMono_Regular.woff2");
        NotoNaskhArabicUiBold       = LoadFont("NotoNaskhArabicUi_Bold.woff2");
        NotoNaskhArabicUiRegular    = LoadFont("NotoNaskhArabicUi_Regular.woff2");
        NotoSansBengaliUiBold       = LoadFont("NotoSansBengaliUi_Bold.woff2");
        NotoSansBengaliUiRegular    = LoadFont("NotoSansBengaliUi_Regular.woff2");
        NotoSansBold                = LoadFont("NotoSans_Bold.woff2");
        NotoSansDevanagariUiBold    = LoadFont("NotoSansDevanagariUi_Bold.woff2");
        NotoSansDevanagariUiRegular = LoadFont("NotoSansDevanagariUi_Regular.woff2");
        NotoSansGeorgianBold        = LoadFont("NotoSansGeorgian_Bold.woff2");
        NotoSansGeorgianRegular     = LoadFont("NotoSansGeorgian_Regular.woff2");
        NotoSansHebrewBold          = LoadFont("NotoSansHebrew_Bold.woff2");
        NotoSansHebrewRegular       = LoadFont("NotoSansHebrew_Regular.woff2");
        NotoSansMalayalamUiBold     = LoadFont("NotoSansMalayalamUi_Bold.woff2");
        NotoSansMalayalamUiRegular  = LoadFont("NotoSansMalayalamUi_Regular.woff2");
        NotoSansOriyaUiBold         = LoadFont("NotoSansOriyaUi_Bold.woff2");
        NotoSansOriyaUiRegular      = LoadFont("NotoSansOriyaUi_Regular.woff2");
        NotoSansRegular             = LoadFont("NotoSans_Regular.woff2");
        NotoSansSinhalaUiBold       = LoadFont("NotoSansSinhalaUi_Bold.woff2");
        NotoSansSinhalaUiRegular    = LoadFont("NotoSansSinhalaUi_Regular.woff2");
        NotoSansTamilUiBold         = LoadFont("NotoSansTamilUi_Bold.woff2");
        NotoSansTamilUiRegular      = LoadFont("NotoSansTamilUi_Regular.woff2");
        NotoSansTeluguUiBold        = LoadFont("NotoSansTeluguUi_Bold.woff2");
        NotoSansTeluguUiRegular     = LoadFont("NotoSansTeluguUi_Regular.woff2");
        NotoSansThaiUiBold          = LoadFont("NotoSansThaiUi_Bold.woff2");
        NotoSansThaiUiRegular       = LoadFont("NotoSansThaiUi_Regular.woff2");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static byte[] LoadFont(string name) =>
        File.ReadAllBytes(Path.Join(fontsPath, name));
}
