namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;
using Godot.Net.ThirdParty.Icu;

using DirProp    = Byte;
using UBiDiLevel = Byte;
using Flags      = UInt32;

public delegate void UBiDiClassCallback();

[StructLayout(LayoutKind.Sequential)]
public unsafe struct UBiDi
{
    /// <summary>Number of isolate entries allocated initially</summary>
    private const int SIMPLE_ISOLATES_COUNT = 5;

    /// <summary>Number of paras entries allocated initially</summary>
    private const int SIMPLE_PARAS_COUNT    = 10;

    /// <summary>Number of isolate run entries for paired brackets allocated initially</summary>
    private const int SIMPLE_OPENINGS_COUNT = 20;

    /// <summary>
    /// UBIDI_REORDER_xxx values must be ordered so that all the regular logical to visual modes come first, and all inverse BiDi modes come last.
    /// </summary>
    public const int UBIDI_REORDER_LAST_LOGICAL_TO_VISUAL = (int)UBiDiReorderingMode.UBIDI_REORDER_NUMBERS_SPECIAL;

    /// <summary>
    /// Pointer to parent paragraph object (pointer to self if this object is a paragraph object); set to NULL in a newly opened object; set to a real value after a successful execution of ubidi_setPara or ubidi_setLine.
    /// </summary>
    public UBiDi* pParaBiDi = null;

    /// <summary>
    /// Alias pointer to the current text.
    /// </summary>
    public ushort* text;

    /// <summary>
    /// Length of the current text.
    /// </summary>
    public int originalLength;

    /// <summary>
    /// If the UBIDI_OPTION_STREAMING option is set, this is the length of text actually processed by ubidi_setPara, which may be shorter than the original length. Otherwise, it is identical to the original length.
    /// </summary>
    public int length;

    /// <summary>
    /// If the UBIDI_OPTION_REMOVE_CONTROLS option is set, and/or marks are allowed to be inserted in one of the reordering mode, the length of the result string may be different from the processed length.
    /// </summary>
    public int resultLength;

    /// <summary>
    /// Memory sizes in bytes.
    /// </summary>
    public int dirPropsSize, levelsSize, openingsSize, parasSize, runsSize, isolatesSize;

    /// <summary>
    /// Allocated memory.
    /// </summary>
    public DirProp*    dirPropsMemory;
    public UBiDiLevel* levelsMemory;
    public Opening*    openingsMemory;
    public Para*       parasMemory;
    public Run*        runsMemory;
    public Isolate*    isolatesMemory;

    /// <summary>
    /// Indicators for whether memory may be allocated after ubidi_open().
    /// </summary>
    public bool mayAllocateText, mayAllocateRuns;

    /// <summary>
    /// Arrays with one value per text-character.
    /// </summary>
    public DirProp*    dirProps;
    public UBiDiLevel* levels;

    /// <summary>
    /// Determines whether we are performing an approximation of the "inverse BiDi" algorithm.
    /// </summary>
    public bool isInverse;

    /// <summary>
    /// Determines whether we are using the basic algorithm or its variation.
    /// </summary>
    public UBiDiReorderingMode reorderingMode;

    /// <summary>
    /// Bitmask for reordering options.
    /// </summary>
    public uint reorderingOptions;

    /// <summary>
    /// Determines whether block separators receive level 0.
    /// </summary>
    public bool orderParagraphsLTR;

    /// <summary>
    /// The paragraph level.
    /// </summary>
    public UBiDiLevel paraLevel;

    /// <summary>
    /// Original paraLevel when contextual. Must be one of UBIDI_DEFAULT_xxx or 0 if not contextual.
    /// </summary>
    public UBiDiLevel defaultParaLevel;

    /// <summary>
    /// Context data.
    /// </summary>
    public ushort* prologue;
    public int     proLength;
    public ushort* epilogue;
    public int     epiLength;

    /// <summary>
    /// The following is set in ubidi_setPara, used in processPropertySeq.
    /// </summary>
    public ImpTabPair* pImpTabPair;  // Pointer to levels state table pair

    /// <summary>
    /// The overall paragraph or line directionality. See UBiDiDirection.
    /// </summary>
    public UBiDiDirection direction;

    /// <summary>
    /// Flags is a bit set for which directional properties are in the text.
    /// </summary>
    public Flags flags;

    /// <summary>
    /// LastArabicPos is index to the last AL in the text, -1 if none.
    /// </summary>
    public int lastArabicPos;

    /// <summary>
    /// Characters after trailingWSStart are WS and are implicitly at the paraLevel (rule (L1)) - levels may not reflect that.
    /// </summary>
    public int trailingWSStart;

    /// <summary>
    /// Fields for paragraph handling.
    /// </summary>
    public int paraCount;                  // Set in getDirProps()
    /// <summary>
    /// Filled in getDirProps().
    /// </summary>
    public Para* paras;

    /// <summary>
    /// For relatively short text, we only need a tiny array of paras).
    /// </summary>
    /// <remarks>
    /// Same as
    /// <code>
    ///     Para simpleParas[SIMPLE_PARAS_COUNT]
    /// </code>
    /// </remarks>
    // public fixed byte simpleParas[SIMPLE_PARAS_COUNT];
    public fixed byte simpleParas[SIMPLE_PARAS_COUNT * 8];

    /// <summary>
    /// Fields for line reordering.
    /// </summary>
    public int runCount;     // ==-1: runs not set up yet
    public Run* runs;

    /// <summary>
    /// For non-mixed text, we only need a tiny array of runs).
    /// </summary>
    /// <remarks>
    /// Same as
    /// <code>
    ///     Run simpleParas[1]
    /// </code>
    /// </remarks>
    public fixed byte simpleRuns[12];

    /// <summary>
    /// Maximum or current nesting depth of isolate sequences. Within resolveExplicitLevels() and checkExplicitLevels(), this is the maximal nesting encountered. Within resolveImplicitLevels(), this is the index of the current isolates stack entry.
    /// </summary>
    public int isolateCount;
    public Isolate* isolates;

    /// <summary>
    /// For simple text, have a small stack).
    /// </summary>
    /// <remarks>
    /// Same as
    /// <code>
    ///     Isolate simpleIsolates[SIMPLE_ISOLATES_COUNT]
    /// </code>
    /// </remarks>
    public fixed byte simpleIsolates[SIMPLE_ISOLATES_COUNT * 18];

    /// <summary>
    /// For inverse Bidi with insertion of directional marks.
    /// </summary>
    public InsertPoints insertPoints;

    /// <summary>
    /// For option UBIDI_OPTION_REMOVE_CONTROLS.
    /// </summary>
    public int controlCount;

    /// <summary>
    /// For Bidi class callback.
    /// </summary>
    public nint fnClassCallback; // Action pointer
    public nint coClassCallback; // Context pointer

    public UBiDi()
    { }
}
