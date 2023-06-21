namespace Godot.Net.ThirdParty.Icu;

using System.Runtime.CompilerServices;
using Godot.Net.Platforms.Windows;
using Godot.Net.ThirdParty.Icu.Native;

using UBiDiLevel = Byte;

/// <summary>
/// Api reference https://unicode-org.github.io/icu-docs/apidoc/released/icu4c
/// </summary>
public unsafe class IcuNative : IDisposable
{
    private const string RUNTIME = "win-x64";
    private const int    VERSION = 68;

    private delegate int UbidiCountRunsDelegate(UBiDi* pBiDi, UErrorCode* pErrorCode);
    private delegate UBiDiDirection UbidiGetBaseDirectionDelegate(char* text, int length);
    private delegate UBiDiDirection UbidiGetVisualRunDelegate(UBiDi* pBiDi, int runIndex, int* pLogicalStart, int* pLength);
    private delegate UBiDi* UbidiOpenSizedDelegate(int maxLength, int maxRunCount, UErrorCode* pErrorCode);
    private delegate UBiDi* UbidiSetLineDelegate(UBiDi* pParaBiDi, int start, int limit, UBiDi* pLineBiDi, UErrorCode* pErrorCode);
    private delegate void UbidiSetParaDelegate(UBiDi* pBiDi, char* text, int length, UBiDiLevel paraLevel, UBiDiLevel* embeddingLevels, UErrorCode* pErrorCode);
    private delegate void UbrkCloseDelegate(UBreakIterator* bi);
    private delegate int UbrkCurrentDelegate(/*const*/ UBreakIterator* bi);
    private delegate int UbrkGetRuleStatusDelegate(UBreakIterator* bi);
    private delegate int UbrkNextDelegate(UBreakIterator* bi);
    private delegate UBreakIterator* UbrkOpenDelegate(UBreakIteratorType type, /*const*/ char* locale, /*const*/ char* text, int textLength, UErrorCode* status);
    private delegate int UGetIntPropertyValueDelegate(int c, UProperty which);
    private delegate bool UIsPunctDelegate(int c);

    private static IcuNative? singleton;

    public static IcuNative Singleton => singleton ?? throw new NullReferenceException();

    private readonly WindowsUnmanagedLibrary library;

    private bool disposed;

    private readonly UbidiCountRunsDelegate        ubidiCountRuns;
    private readonly UbidiGetBaseDirectionDelegate ubidiGetBaseDirection;
    private readonly UbidiGetVisualRunDelegate     ubidiGetVisualRun;
    private readonly UbidiOpenSizedDelegate        ubidiOpenSized;
    private readonly UbidiSetLineDelegate          ubidiSetLine;
    private readonly UbidiSetParaDelegate          ubidiSetPara;
    private readonly UbrkCloseDelegate             ubrkClose;
    private readonly UbrkCurrentDelegate           ubrkCurrent;
    private readonly UbrkGetRuleStatusDelegate     ubrkGetRuleStatus;
    private readonly UbrkNextDelegate              ubrkNext;
    private readonly UbrkOpenDelegate              ubrkOpen;
    private readonly UGetIntPropertyValueDelegate  uGetIntPropertyValue;
    private readonly UIsPunctDelegate              uIsPunct;

    private IcuNative()
    {
        var path = Path.Join(AppContext.BaseDirectory, "runtimes", RUNTIME, "native", $"icuuc{VERSION}.dll");

        if (!Path.Exists(path))
        {
            throw new InvalidOperationException("Failed to locate icu");
        }

        this.library = new WindowsUnmanagedLibrary(path);

        if (!this.library.IsLoaded)
        {
            throw new InvalidOperationException("Failed to load icu");
        }

        this.ubidiCountRuns        = this.library.GetProcAddress<UbidiCountRunsDelegate>($"ubidi_countRuns_{VERSION}")!;
        this.ubidiGetBaseDirection = this.library.GetProcAddress<UbidiGetBaseDirectionDelegate>($"ubidi_getBaseDirection_{VERSION}")!;
        this.ubidiGetVisualRun     = this.library.GetProcAddress<UbidiGetVisualRunDelegate>($"ubidi_getVisualRun_{VERSION}")!;
        this.ubidiOpenSized        = this.library.GetProcAddress<UbidiOpenSizedDelegate>($"ubidi_openSized_{VERSION}")!;
        this.ubidiSetLine          = this.library.GetProcAddress<UbidiSetLineDelegate>($"ubidi_setLine_{VERSION}")!;
        this.ubidiSetPara          = this.library.GetProcAddress<UbidiSetParaDelegate>($"ubidi_setPara_{VERSION}")!;
        this.ubrkClose             = this.library.GetProcAddress<UbrkCloseDelegate>($"ubrk_close_{VERSION}")!;
        this.ubrkCurrent           = this.library.GetProcAddress<UbrkCurrentDelegate>($"ubrk_current_{VERSION}")!;
        this.ubrkGetRuleStatus     = this.library.GetProcAddress<UbrkGetRuleStatusDelegate>($"ubrk_getRuleStatus_{VERSION}")!;
        this.ubrkNext              = this.library.GetProcAddress<UbrkNextDelegate>($"ubrk_next_{VERSION}")!;
        this.ubrkOpen              = this.library.GetProcAddress<UbrkOpenDelegate>($"ubrk_open_{VERSION}")!;
        this.uGetIntPropertyValue  = this.library.GetProcAddress<UGetIntPropertyValueDelegate>($"u_getIntPropertyValue_{VERSION}")!;
        this.uIsPunct              = this.library.GetProcAddress<UIsPunctDelegate>($"u_ispunct_{VERSION}")!;
    }

    public static void Initialize() =>
        singleton = new();

    public static void Close()
    {
        singleton?.Dispose();
        singleton = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.library.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <para>Get the number of runs.</para>
    /// <para>This function may invoke the actual reordering on the UBiDi object, after <see cref="UbidiSetPara"/> may have resolved only the levels of the text. Therefore, ubidi_countRuns() may have to allocate memory, and may fail doing so.</para>
    /// </summary>
    /// <param name="pBiDi">Is the paragraph or line UBiDi object.</param>
    /// <param name="pErrorCode">Must be a valid pointer to an error code value.</param>
    /// <returns>The number of runs.</returns>
    /// <remarks>Stable ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int UbidiCountRuns(UBiDi* pBiDi, UErrorCode* pErrorCode) =>
        this.ubidiCountRuns.Invoke(pBiDi, pErrorCode);

    /// <summary>
    /// <para>Get one run's logical start, length, and directionality, which can be 0 for LTR or 1 for RTL.</para>
    /// <para>In an RTL run, the character at the logical start is visually on the right of the displayed run. The length is the number of characters in the run.</para>
    /// <para><see cref="UbidiCountRuns"/> should be called before the runs are retrieved.</para>
    /// </summary>
    /// <param name="pBiDi">Is the paragraph or line UBiDi object.</param>
    /// <param name="runIndex">Is the number of the run in visual order, in the range [0..CountRuns(pBiDi)-1].</param>
    /// <param name="pLogicalStart">Is the first logical character index in the text. The pointer may be NULL if this index is not needed.</param>
    /// <param name="pLength">Is the number of characters (at least one) in the run. The pointer may be NULL if this is not needed.</param>
    /// <returns>The directionality of the run, UBIDI_LTR == 0 or UBIDI_RTL == 1, never UBIDI_MIXED, never UBIDI_NEUTRAL.</returns>
    /// <seealso cref="UbidiCountRuns"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public UBiDiDirection UbidiGetVisualRun(UBiDi* pBiDi, int runIndex, int* pLogicalStart, int* pLength) =>
        this.ubidiGetVisualRun.Invoke(pBiDi, runIndex, pLogicalStart, pLength);

    /// <summary>
    /// <para>Allocate a UBiDi structure with preallocated memory for internal structures.</para>
    /// <para>This function provides a UBiDi object like ubidi_open() with no arguments, but it also preallocates memory for internal structures according to the sizings supplied by the caller.</para>
    /// <para>Subsequent functions will not allocate any more memory, and are thus guaranteed not to fail because of lack of memory.</para>
    /// <para>The preallocation can be limited to some of the internal memory by setting some values to 0 here. That means that if, e.g., maxRunCount cannot be reasonably predetermined and should not be set to maxLength (the only failproof value) to avoid wasting memory, then maxRunCount could be set to 0 here and the internal structures that are associated with it will be allocated on demand, just like with ubidi_open().</para>
    /// </summary>
    /// <param name="maxLength">Is the maximum text or line length that internal memory will be preallocated for. An attempt to associate this object with a longer text will fail, unless this value is 0, which leaves the allocation up to the implementation.</param>
    /// <param name="maxRunCount">
    /// <para>Is the maximum anticipated number of same-level runs that internal memory will be preallocated for. An attempt to access visual runs on an object that was not preallocated for as many runs as the text was actually resolved to will fail, unless this value is 0, which leaves the allocation up to the implementation.</para>
    /// <para>The number of runs depends on the actual text and maybe anywhere between 1 and maxLength. It is typically small.</para>
    /// </param>
    /// <param name="pErrorCode">Must be a valid pointer to an error code value.</param>
    /// <returns>An empty UBiDi object with preallocated memory.</returns>
    /// <remarks>Stable ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public UBiDi* UbidiOpenSized(int maxLength, int maxRunCount, UErrorCode* pErrorCode) =>
        this.ubidiOpenSized.Invoke(maxLength, maxRunCount, pErrorCode);

    /// <summary>
    /// <para>ubidi_setLine() sets a UBiDi to contain the reordering information, especially the resolved levels, for all the characters in a line of text.</para>
    /// <para>This line of text is specified by referring to a UBiDi object representing this information for a piece of text containing one or more paragraphs, and by specifying a range of indexes in this text.</para>
    /// <para>In the new line object, the indexes will range from 0 to limit-start-1.</para>
    /// <para>This is used after calling ubidi_setPara() for a piece of text, and after line-breaking on that text. It is not necessary if each paragraph is treated as a single line.</para>
    /// <para>After line-breaking, rules (L1) and (L2) for the treatment of trailing WS and for reordering are performed on a UBiDi object that represents a line.</para>
    /// <para>Important: pLineBiDi shares data with pParaBiDi. You must destroy or reuse pLineBiDi before pParaBiDi. In other words, you must destroy or reuse the UBiDi object for a line before the object for its parent paragraph.</para>
    /// <para>The text pointer that was stored in pParaBiDi is also copied, and start is added to it so that it points to the beginning of the line for this object.</para>
    /// </summary>
    /// <param name="bidi">Is the parent paragraph object. It must have been set by a successful call to ubidi_setPara.</param>
    /// <param name="start">Is the line's first index into the text.</param>
    /// <param name="limit">
    /// <para>Is just behind the line's last index into the text (its last index +1).</para>
    /// <para>It must be 0 &lt;=start &lt; limit &lt;= containing paragraph limit. If the specified line crosses a paragraph boundary, the function will terminate with error code U_ILLEGAL_ARGUMENT_ERROR.</para>
    /// </param>
    /// <param name="pLineBiDi">Is the object that will now represent a line of the text.</param>
    /// <param name="pErrorCode">Must be a valid pointer to an error code value.</param>
    /// <remarks>Stable ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void UbidiSetLine(UBiDi* bidi, int start, int limit, UBiDi* pLineBiDi, UErrorCode* pErrorCode) =>
        this.ubidiSetLine.Invoke(bidi, start, limit, pLineBiDi, pErrorCode);

    /// <summary>
    /// <para>Perform the Unicode Bidi algorithm.</para>
    /// <para>It is defined in the Unicode Standard Annex #9, version 13, also described in The Unicode Standard, Version 4.0 .</para>
    /// <para>This function takes a piece of plain text containing one or more paragraphs, with or without externally specified embedding levels from styled text and computes the left-right-directionality of each character.</para>
    /// <para>If the entire text is all of the same directionality, then the function may not perform all the steps described by the algorithm, i.e., some levels may not be the same as if all steps were performed. This is not relevant for unidirectional text.</para>
    /// <para>For example, in pure LTR text with numbers the numbers would get a resolved level of 2 higher than the surrounding text according to the algorithm. This implementation may set all resolved levels to the same value in such a case.</para>
    /// <para>The text can be composed of multiple paragraphs. Occurrence of a block separator in the text terminates a paragraph, and whatever comes next starts a new paragraph. The exception to this rule is when a Carriage Return (CR) is followed by a Line Feed (LF). Both CR and LF are block separators, but in that case, the pair of characters is considered as terminating the preceding paragraph, and a new paragraph will be started by a character coming after the LF.</para>
    /// </summary>
    /// <param name="pBiDi">A UBiDi object allocated with ubidi_open() which will be set to contain the reordering information, especially the resolved levels for all the characters in text.</param>
    /// <param name="text">
    /// <para>Is a pointer to the text that the Bidi algorithm will be performed on. This pointer is stored in the UBiDi object and can be retrieved with ubidi_getText().</para>
    /// <para>Note: the text must be (at least) length long.</para>
    /// </param>
    /// <param name="length">Is the length of the text; if length==-1 then the text must be zero-terminated.</param>
    /// <param name="paraLevel">Specifies the default level for the text; it is typically 0 (LTR) or 1 (RTL). If the function shall determine the paragraph level from the text, then paraLevel can be set to either UBIDI_DEFAULT_LTR or UBIDI_DEFAULT_RTL; if the text contains multiple paragraphs, the paragraph level shall be determined separately for each paragraph; if a paragraph does not include any strongly typed character, then the desired default is used (0 for LTR or 1 for RTL). Any other value between 0 and UBIDI_MAX_EXPLICIT_LEVEL is also valid, with odd levels indicating RTL.</param>
    /// <param name="embeddingLevels">
    /// <para>(in) May be used to preset the embedding and override levels, ignoring characters like LRE and PDF in the text. A level overrides the directional property of its corresponding (same index) character if the level has the UBIDI_LEVEL_OVERRIDE bit set.</para>
    /// <para>Aside from that bit, it must be paraLevel &lt;= embeddingLevels[] &lt;= UBIDI_MAX_EXPLICIT_LEVEL, except that level 0 is always allowed. Level 0 for a paragraph separator prevents reordering of paragraphs; this only works reliably if UBIDI_LEVEL_OVERRIDE is also set for paragraph separators. Level 0 for other characters is treated as a wildcard and is lifted up to the resolved level of the surrounding paragraph.</para>
    /// <para>Caution: A copy of this pointer, not of the levels, will be stored in the UBiDi object; the embeddingLevels array must not be deallocated before the UBiDi structure is destroyed or reused, and the embeddingLevels should not be modified to avoid unexpected results on subsequent Bidi operations. However, the ubidi_setPara() and ubidi_setLine() functions may modify some or all of the levels.</para>
    /// <para>After the UBiDi object is reused or destroyed, the caller must take care of the deallocation of the embeddingLevels array.</para>
    /// <para>Note: the embeddingLevels array must be at least length long. This pointer can be NULL if this value is not necessary.</para>
    /// </param>
    /// <param name="pErrorCode">Must be a valid pointer to an error code value.</param>
    /// <remarks>Stable ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void UbidiSetPara(UBiDi* pBiDi, char* text, int length, UBiDiLevel paraLevel, UBiDiLevel* embeddingLevels, UErrorCode* pErrorCode) =>
        this.ubidiSetPara.Invoke(pBiDi, text, length, paraLevel, embeddingLevels, pErrorCode);

    /// <summary>
    /// <para>Close a UBreakIterator.</para>
    /// <para>Once closed, a UBreakIterator may no longer be used.</para>
    /// </summary>
    /// <param name="bi">The break iterator to close.</param>
    /// <remarks>Stable ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void UbrkClose(UBreakIterator* bi) =>
        this.ubrkClose.Invoke(bi);

    /// <summary>
    /// Determine the most recently-returned text boundary.
    /// </summary>
    /// <param name="bi">The break iterator to use.</param>
    /// <returns>The character index most recently returned by ubrk_next, ubrk_previous, ubrk_first, or ubrk_last.</returns>
    /// <remarks>Stable: ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int UbrkCurrent(/*const*/ UBreakIterator* bi) =>
        this.ubrkCurrent.Invoke(bi);

    /// <summary>
    /// <para>Return the status from the break rule that determined the most recently returned break position.</para>
    /// <para>The values appear in the rule source within brackets, {123}, for example. For rules that do not specify a status, a default value of 0 is returned.</para>
    /// <para>For word break iterators, the possible values are defined in enum UWordBreak.</para>
    /// </summary>
    /// <param name="bi"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int UbrkGetRuleStatus(UBreakIterator* bi) =>
        this.ubrkGetRuleStatus.Invoke(bi);

    /// <summary>
    /// Advance the iterator to the boundary following the current boundary.
    /// </summary>
    /// <param name="bi">The break iterator to use.</param>
    /// <returns>The character index of the next text boundary, or UBRK_DONE if all text boundaries have been returned.</returns>
    /// <seealso cref="UbrkPrevious"/>
    /// <remarks>ICU 2.0</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int UbrkNext(UBreakIterator* bi) =>
        this.ubrkNext(bi);

    /// <summary>
    /// <para>Open a new UBreakIterator for locating text boundaries for a specified locale.</para>
    /// <para>A UBreakIterator may be used for detecting character, line, word, and sentence breaks in text.</para>
    /// </summary>
    /// <param name="type">The type of UBreakIterator to open: one of UBRK_CHARACTER, UBRK_WORD, UBRK_LINE, UBRK_SENTENCE</param>
    /// <param name="locale">The locale specifying the text-breaking conventions. Note that locale keys such as "lb" and "ss" may be used to modify text break behavior, see general discussion of BreakIterator C API.</param>
    /// <param name="text">The text to be iterated over. May be null, in which case ubrk_setText() is used to specify the text to be iterated.</param>
    /// <param name="textLength">The number of characters in text, or -1 if null-terminated.</param>
    /// <param name="status">A UErrorCode to receive any errors.</param>
    /// <returns>A UBreakIterator for the specified locale.</returns>
    /// <seealso cref="UbrkOpenRules"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public UBreakIterator* UbrkOpen(UBreakIteratorType type, /*const*/ char* locale, /*const*/ char* text, int textLength, UErrorCode* status) =>
        this.ubrkOpen.Invoke(type, locale, text, textLength, status);

    /// <summary>
    /// <para>Gets the base direction of the text provided according to the Unicode Bidirectional Algorithm.</para>
    /// <para>The base direction is derived from the first character in the string with bidirectional character type L, R, or AL. If the first such character has type L, UBIDI_LTR is returned. If the first such character has type R or AL, UBIDI_RTL is returned. If the string does not contain any character of these types, then UBIDI_NEUTRAL is returned.</para>
    /// <para>This is a lightweight function for use when only the base direction is needed and no further bidi processing of the text is needed.</para>
    /// </summary>
    /// <param name="text">Is a pointer to the text whose base direction is needed. Note: the text must be (at least) length long.</param>
    /// <param name="length">Is the length of the text; if length==-1 then the text must be zero-terminated.</param>
    /// <returns>UBIDI_LTR, UBIDI_RTL, UBIDI_NEUTRAL</returns>
    /// <seealso cref="UBiDiDirection"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public UBiDiDirection UGetBaseDirection(char* text, int length) =>
        this.ubidiGetBaseDirection.Invoke(text, length);

    /// <summary>
    /// <para>Get the property value for an enumerated or integer Unicode property for a code point.</para>
    /// <para>Also returns binary and mask property values.</para>
    /// <para>Unicode, especially in version 3.2, defines many more properties than the original set in UnicodeData.txt.</para>
    /// <para>The properties APIs are intended to reflect Unicode properties as defined in the Unicode Character Database (UCD) and Unicode Technical Reports (UTR). For details about the properties see http://www.unicode.org/ . For names of Unicode properties see the UCD file PropertyAliases.txt.</para>
    /// <example>
    /// Sample usage:
    /// <code>
    ///     var ea = (UEastAsianWidth)GetIntPropertyValue(c, UProperty.UCHAR_EAST_ASIAN_WIDTH);
    ///     var b  = (bool)GetIntPropertyValue(c, UProperty.UCHAR_IDEOGRAPHIC);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="c">Code point to test.</param>
    /// <param name="which">UProperty selector constant, identifies which property to check. Must be UCHAR_BINARY_START &lt;= which &lt; UCHAR_BINARY_LIMIT or UCHAR_INT_START &lt;= which &lt; UCHAR_INT_LIMIT or UCHAR_MASK_START &lt;= which &lt; UCHAR_MASK_LIMIT..</param>
    /// <returns>Numeric value that is directly the property value or, for enumerated properties, corresponds to the numeric value of the enumerated constant of the respective property value enumeration type (cast to enum type if necessary). Returns 0 or 1 (for false/true) for binary Unicode properties. Returns a bit-mask for mask properties. Returns 0 if 'which' is out of bounds or if the Unicode version does not have data for the property at all, or not for this code point.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int UGetIntPropertyValue(int c, UProperty which) =>
        this.uGetIntPropertyValue.Invoke(c, which);

    /// <summary>
    /// <para>Determines whether the specified code point is a punctuation character.</para>
    /// <para>True for characters with general categories "P" (punctuation).</para>
    /// <para>This is a C/POSIX migration function. See the comments about C/POSIX character classification functions in the documentation at the top of this header file.</para>
    /// </summary>
    /// <param name="c">The code point to be tested</param>
    /// <returns>true if the code point is a punctuation character</returns>
    /// <remarks>Stable ICU 2.6</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool UIsPunct(int c) =>
        this.uIsPunct.Invoke(c);
}
