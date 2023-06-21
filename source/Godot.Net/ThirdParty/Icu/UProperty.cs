namespace Godot.Net.ThirdParty.Icu;

/// <summary>
/// Selection constants for Unicode properties.
/// These constants are used in functions like u_hasBinaryProperty to select
/// one of the Unicode properties.
///
/// The properties APIs are intended to reflect Unicode properties as defined
/// in the Unicode Character Database (UCD) and Unicode Technical Reports (UTR).
///
/// For details about the properties see UAX #44: Unicode Character Database (http://www.unicode.org/reports/tr44/).
///
/// Important: If ICU is built with UCD files from Unicode versions below, e.g., 3.2,
/// then properties marked with "new in Unicode 3.2" are not or not fully available.
/// Check u_getUnicodeVersion to be sure.
///
/// See also: u_hasBinaryProperty, u_getIntPropertyValue, u_getUnicodeVersion.
///
/// Stable: ICU 2.1
/// </summary>
public enum UProperty
{
    /*
     * Note: UProperty constants are parsed by preparseucd.py.
     * It matches lines like
     *     UCHAR_<Unicode property name> = <integer>,
     */

    /*  Note: Place UCHAR_ALPHABETIC before UCHAR_BINARY_START so that
    debuggers display UCHAR_ALPHABETIC as the symbolic name for 0,
    rather than UCHAR_BINARY_START.  Likewise for other *_START
    identifiers. */

    /// <summary>
    /// Binary property Alphabetic.
    /// Same as u_isUAlphabetic, different from u_isalpha.
    /// Lu+Ll+Lt+Lm+Lo+Nl+Other_Alphabetic
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_ALPHABETIC = 0,
    /// <summary>
    /// First constant for binary Unicode properties.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_BINARY_START = UCHAR_ALPHABETIC,

    /// <summary>
    /// Binary property ASCII_Hex_Digit. 0-9 A-F a-f
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_ASCII_HEX_DIGIT = 1,

    /// <summary>
    /// Binary property Bidi_Control.
    /// Format controls which have specific functions in the Bidi Algorithm.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_BIDI_CONTROL = 2,

    /// <summary>
    /// Binary property Bidi_Mirrored.
    /// Characters that may change display in RTL text.
    /// Same as u_isMirrored.
    /// See Bidi Algorithm, UTR 9.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_BIDI_MIRRORED = 3,

    /// <summary>
    /// Binary property Dash. Variations of dashes.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_DASH = 4,

    /// <summary>
    /// Binary property Default_Ignorable_Code_Point (new in Unicode 3.2).
    /// Ignorable in most processing.
    /// <2060..206F, FFF0..FFFB, E0000..E0FFF>+Other_Default_Ignorable_Code_Point+(Cf+Cc+Cs-White_Space)
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_DEFAULT_IGNORABLE_CODE_POINT = 5,

    /// <summary>
    /// Binary property Deprecated (new in Unicode 3.2).
    /// The usage of deprecated characters is strongly discouraged.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_DEPRECATED = 6,

    /// <summary>
    /// Binary property Diacritic.
    /// Characters that linguistically modify the meaning of another character to which they apply.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_DIACRITIC = 7,

    /// <summary>
    /// Binary property Extender.
    /// Extend the value or shape of a preceding alphabetic character, e.g., length and iteration marks.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_EXTENDER = 8,

    /// <summary>
    /// Binary property Full_Composition_Exclusion.
    /// CompositionExclusions.txt+Singleton Decompositions+Non-Starter Decompositions.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_FULL_COMPOSITION_EXCLUSION = 9,

    /// <summary>
    /// Binary property Grapheme_Base (new in Unicode 3.2).
    /// For programmatic determination of grapheme cluster boundaries.
    /// [0..10FFFF]-Cc-Cf-Cs-Co-Cn-Zl-Zp-Grapheme_Link-Grapheme_Extend-CGJ
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_GRAPHEME_BASE = 10,
    /// <summary>
    /// Binary property Grapheme_Extend (new in Unicode 3.2).
    /// For programmatic determination of grapheme cluster boundaries.
    /// Me+Mn+Mc+Other_Grapheme_Extend-Grapheme_Link-CGJ
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_GRAPHEME_EXTEND = 11,

    /// <summary>
    /// Binary property Grapheme_Link (new in Unicode 3.2).
    /// For programmatic determination of grapheme cluster boundaries.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_GRAPHEME_LINK = 12,

    /// <summary>
    /// Binary property Hex_Digit.
    /// Characters commonly used for hexadecimal numbers.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_HEX_DIGIT = 13,

    /// <summary>
    /// Binary property Hyphen. Dashes used to mark connections between pieces of words, plus the Katakana middle dot.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_HYPHEN = 14,

    /// <summary>
    /// Binary property ID_Continue.
    /// Characters that can continue an identifier.
    /// DerivedCoreProperties.txt also says "NOTE: Cf characters should be filtered out."
    /// ID_Start+Mn+Mc+Nd+Pc
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_ID_CONTINUE = 15,

    /// <summary>
    /// Binary property ID_Start.
    /// Characters that can start an identifier.
    /// Lu+Ll+Lt+Lm+Lo+Nl
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_ID_START = 16,

    /// <summary>
    /// Binary property Ideographic.
    /// CJKV ideographs.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_IDEOGRAPHIC = 17,

    /// <summary>
    /// Binary property IDS_Binary_Operator (new in Unicode 3.2).
    /// For programmatic determination of Ideographic Description Sequences.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_IDS_BINARY_OPERATOR = 18,

    /// <summary>
    /// Binary property IDS_Trinary_Operator (new in Unicode 3.2).
    /// For programmatic determination of Ideographic Description Sequences.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_IDS_TRINARY_OPERATOR = 19,

    /// <summary>
    /// Binary property Join_Control.
    /// Format controls for cursive joining and ligation.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_JOIN_CONTROL = 20,
    /// <summary>
    /// Binary property Logical_Order_Exception (new in Unicode 3.2).
    /// Characters that do not use logical order and require special handling in most processing.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_LOGICAL_ORDER_EXCEPTION = 21,

    /// <summary>
    /// Binary property Lowercase. Same as u_isULowercase, different from u_islower.
    /// Ll+Other_Lowercase
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_LOWERCASE = 22,

    /// <summary>
    /// Binary property Math. Sm+Other_Math
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_MATH = 23,

    /// <summary>
    /// Binary property Noncharacter_Code_Point.
    /// Code points that are explicitly defined as illegal for the encoding of characters.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_NONCHARACTER_CODE_POINT = 24,

    /// <summary>
    /// Binary property Quotation_Mark.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_QUOTATION_MARK = 25,

    /// <summary>
    /// Binary property Radical (new in Unicode 3.2).
    /// For programmatic determination of Ideographic Description Sequences.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_RADICAL = 26,

    /// <summary>
    /// Binary property Soft_Dotted (new in Unicode 3.2).
    /// Characters with a "soft dot", like i or j.
    /// An accent placed on these characters causes the dot to disappear.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_SOFT_DOTTED = 27,

    /// <summary>
    /// Binary property Terminal_Punctuation.
    /// Punctuation characters that generally mark the end of textual units.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_TERMINAL_PUNCTUATION = 28,

    /// <summary>
    /// Binary property Unified_Ideograph (new in Unicode 3.2).
    /// For programmatic determination of Ideographic Description Sequences.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_UNIFIED_IDEOGRAPH = 29,

    /// <summary>
    /// Binary property Uppercase. Same as u_isUUppercase, different from u_isupper.
    /// Lu+Other_Uppercase
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_UPPERCASE = 30,

    /// <summary>
    /// Binary property White_Space.
    /// Same as u_isUWhiteSpace, different from u_isspace and u_isWhitespace.
    /// Space characters+TAB+CR+LF-ZWSP-ZWNBSP
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_WHITE_SPACE = 31,

    /// <summary>
    /// Binary property XID_Continue.
    /// ID_Continue modified to allow closure under normalization forms NFKC and NFKD.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_XID_CONTINUE = 32,

    /// <summary>
    /// Binary property XID_Start. ID_Start modified to allow closure under normalization forms NFKC and NFKD.
    ///
    /// Stable: ICU 2.1
    /// </summary>
    UCHAR_XID_START = 33,

    /// <summary>
    /// Binary property Case_Sensitive. Either the source of a case mapping or _in_ the target of a case mapping.
    /// Not the same as the general category Cased_Letter.
    ///
    /// Stable: ICU 2.6
    /// </summary>
    UCHAR_CASE_SENSITIVE = 34,

    /// <summary>
    /// Binary property STerm (new in Unicode 4.0.1).
    /// Sentence Terminal. Used in UAX #29: Text Boundaries (http://www.unicode.org/reports/tr29/)
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_S_TERM = 35,

    /// <summary>
    /// Binary property Variation_Selector (new in Unicode 4.0.1).
    /// Indicates all those characters that qualify as Variation Selectors.
    /// For details on the behavior of these characters, see StandardizedVariants.html and 15.6 Variation Selectors.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_VARIATION_SELECTOR = 36,

    /// <summary>
    /// Binary property NFD_Inert.
    /// ICU-specific property for characters that are inert under NFD,
    /// i.e., they do not interact with adjacent characters.
    /// See the documentation for the Normalizer2 class and the Normalizer2::isInert() method.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFD_INERT = 37,

    /// <summary>
    /// Binary property NFKD_Inert.
    /// ICU-specific property for characters that are inert under NFKD,
    /// i.e., they do not interact with adjacent characters.
    /// See the documentation for the Normalizer2 class and the Normalizer2::isInert() method.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFKD_INERT = 38,

    /// <summary>
    /// Binary property NFC_Inert.
    /// ICU-specific property for characters that are inert under NFC,
    /// i.e., they do not interact with adjacent characters.
    /// See the documentation for the Normalizer2 class and the Normalizer2::isInert() method.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFC_INERT = 39,

    /// <summary>
    /// Binary property NFKC_Inert.
    /// ICU-specific property for characters that are inert under NFKC,
    /// i.e., they do not interact with adjacent characters.
    /// See the documentation for the Normalizer2 class and the Normalizer2::isInert() method.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFKC_INERT = 40,
    /// <summary>
    /// Binary Property Segment_Starter.
    /// ICU-specific property for characters that are starters in terms of Unicode normalization and combining character sequences.
    /// They have ccc = 0 and do not occur in non-initial position of the canonical decomposition of any character (like a-umlaut in NFD and a Jamo T in an NFD(Hangul LVT)).
    /// ICU uses this property for segmenting a string for generating a set of canonically equivalent strings, e.g. for canonical closure while processing collation tailoring rules.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_SEGMENT_STARTER = 41,

    /// <summary>
    /// Binary property Pattern_Syntax (new in Unicode 4.1).
    /// See UAX #31 Identifier and Pattern Syntax (http://www.unicode.org/reports/tr31/)
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_PATTERN_SYNTAX = 42,

    /// <summary>
    /// Binary property Pattern_White_Space (new in Unicode 4.1).
    /// See UAX #31 Identifier and Pattern Syntax (http://www.unicode.org/reports/tr31/)
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_PATTERN_WHITE_SPACE = 43,

    /// <summary>
    /// Binary property alnum (a C/POSIX character class).
    /// Implemented according to the UTS #18 Annex C Standard Recommendation.
    /// See the uchar.h file documentation.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_POSIX_ALNUM = 44,

    /// <summary>
    /// Binary property blank (a C/POSIX character class).
    /// Implemented according to the UTS #18 Annex C Standard Recommendation.
    /// See the uchar.h file documentation.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_POSIX_BLANK = 45,

    /// <summary>
    /// Binary property graph (a C/POSIX character class).
    /// Implemented according to the UTS #18 Annex C Standard Recommendation.
    /// See the uchar.h file documentation.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_POSIX_GRAPH = 46,

    /// <summary>
    /// Binary property print (a C/POSIX character class).
    /// Implemented according to the UTS #18 Annex C Standard Recommendation.
    /// See the uchar.h file documentation.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_POSIX_PRINT = 47,

    /// <summary>
    /// Binary property xdigit (a C/POSIX character class).
    /// Implemented according to the UTS #18 Annex C Standard Recommendation.
    /// See the uchar.h file documentation.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_POSIX_XDIGIT = 48,

    /// <summary>
    /// Binary property Cased. For Lowercase, Uppercase and Titlecase characters.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CASED = 49,

    /// <summary>
    /// Binary property Case_Ignorable. Used in context-sensitive case mappings.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CASE_IGNORABLE = 50,

    /// <summary>
    /// Binary property Changes_When_Lowercased.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_LOWERCASED = 51,

    /// <summary>
    /// Binary property Changes_When_Uppercased.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_UPPERCASED = 52,

    /// <summary>
    /// Binary property Changes_When_Titlecased.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_TITLECASED = 53,

    /// <summary>
    /// Binary property Changes_When_Casefolded.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_CASEFOLDED = 54,

    /// <summary>
    /// Binary property Changes_When_Casemapped.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_CASEMAPPED = 55,

    /// <summary>
    /// Binary property Changes_When_NFKC_Casefolded.
    ///
    /// Stable: ICU 4.4
    /// </summary>
    UCHAR_CHANGES_WHEN_NFKC_CASEFOLDED = 56,

    /// <summary>
    /// Binary property Emoji.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 57
    /// </summary>
    UCHAR_EMOJI = 57,

    /// <summary>
    /// Binary property Emoji_Presentation.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 57
    /// </summary>
    UCHAR_EMOJI_PRESENTATION = 58,

    /// <summary>
    /// Binary property Emoji_Modifier.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 57
    /// </summary>
    UCHAR_EMOJI_MODIFIER = 59,

    /// <summary>
    /// Binary property Emoji_Modifier_Base.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 57
    /// </summary>
    UCHAR_EMOJI_MODIFIER_BASE = 60,
    /// <summary>
    /// Binary property Emoji_Component.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 60
    /// </summary>
    UCHAR_EMOJI_COMPONENT = 61,

    /// <summary>
    /// Binary property Regional_Indicator.
    ///
    /// Stable: ICU 60
    /// </summary>
    UCHAR_REGIONAL_INDICATOR = 62,

    /// <summary>
    /// Binary property Prepended_Concatenation_Mark.
    ///
    /// Stable: ICU 60
    /// </summary>
    UCHAR_PREPENDED_CONCATENATION_MARK = 63,

    /// <summary>
    /// Binary property Extended_Pictographic.
    /// See http://www.unicode.org/reports/tr51/#Emoji_Properties
    ///
    /// Stable: ICU 62
    /// </summary>
    UCHAR_EXTENDED_PICTOGRAPHIC = 64,

    /// <summary>
    /// Binary property of strings Basic_Emoji.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_BASIC_EMOJI = 65,

    /// <summary>
    /// Binary property of strings Emoji_Keycap_Sequence.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_EMOJI_KEYCAP_SEQUENCE = 66,

    /// <summary>
    /// Binary property of strings RGI_Emoji_Modifier_Sequence.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_RGI_EMOJI_MODIFIER_SEQUENCE = 67,

    /// <summary>
    /// Binary property of strings RGI_Emoji_Flag_Sequence.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_RGI_EMOJI_FLAG_SEQUENCE = 68,

    /// <summary>
    /// Binary property of strings RGI_Emoji_Tag_Sequence.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_RGI_EMOJI_TAG_SEQUENCE = 69,

    /// <summary>
    /// Binary property of strings RGI_Emoji_ZWJ_Sequence.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_RGI_EMOJI_ZWJ_SEQUENCE = 70,

    /// <summary>
    /// Binary property of strings RGI_Emoji.
    /// See https://www.unicode.org/reports/tr51/#Emoji_Sets
    ///
    /// Stable: ICU 70
    /// </summary>
    UCHAR_RGI_EMOJI = 71,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for binary Unicode properties.
    /// Deprecated: ICU 58 The numeric value may change over time, see ICU ticket #12420.
    /// </summary>
    UCHAR_BINARY_LIMIT = 72,
    #endif

    /// <summary>
    /// Enumerated property Bidi_Class.
    /// Same as u_charDirection, returns UCharDirection values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_BIDI_CLASS = 0x1000,

    /// <summary>
    /// First constant for enumerated/integer Unicode properties.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_INT_START = UCHAR_BIDI_CLASS,

    /// <summary>
    /// Enumerated property Block.
    /// Same as ublock_getCode, returns UBlockCode values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_BLOCK = 0x1001,

    /// <summary>
    /// Enumerated property Canonical_Combining_Class.
    /// Same as u_getCombiningClass, returns 8-bit numeric values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_CANONICAL_COMBINING_CLASS = 0x1002,

    /// <summary>
    /// Enumerated property Decomposition_Type.
    /// Returns UDecompositionType values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_DECOMPOSITION_TYPE = 0x1003,

    /// <summary>
    /// Enumerated property East_Asian_Width.
    /// See http://www.unicode.org/reports/tr11/
    /// Returns UEastAsianWidth values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_EAST_ASIAN_WIDTH = 0x1004,

    /// <summary>
    /// Enumerated property General_Category.
    /// Same as u_charType, returns UCharCategory values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_GENERAL_CATEGORY = 0x1005,

    /// <summary>
    /// Enumerated property Joining_Group.
    /// Returns UJoiningGroup values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_JOINING_GROUP = 0x1006,

    /// <summary>
    /// Enumerated property Joining_Type.
    /// Returns UJoiningType values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_JOINING_TYPE = 0x1007,
    /// <summary>
    /// Enumerated property Line_Break.
    /// Returns ULineBreak values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_LINE_BREAK = 0x1008,

    /// <summary>
    /// Enumerated property Numeric_Type.
    /// Returns UNumericType values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_NUMERIC_TYPE = 0x1009,

    /// <summary>
    /// Enumerated property Script.
    /// Same as uscript_getScript, returns UScriptCode values.
    ///
    /// Stable: ICU 2.2
    /// </summary>
    UCHAR_SCRIPT = 0x100A,

    /// <summary>
    /// Enumerated property Hangul_Syllable_Type, new in Unicode 4.
    /// Returns UHangulSyllableType values.
    ///
    /// Stable: ICU 2.6
    /// </summary>
    UCHAR_HANGUL_SYLLABLE_TYPE = 0x100B,

    /// <summary>
    /// Enumerated property NFD_Quick_Check.
    /// Returns UNormalizationCheckResult values.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFD_QUICK_CHECK = 0x100C,

    /// <summary>
    /// Enumerated property NFKD_Quick_Check.
    /// Returns UNormalizationCheckResult values.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFKD_QUICK_CHECK = 0x100D,

    /// <summary>
    /// Enumerated property NFC_Quick_Check.
    /// Returns UNormalizationCheckResult values.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFC_QUICK_CHECK = 0x100E,

    /// <summary>
    /// Enumerated property NFKC_Quick_Check.
    /// Returns UNormalizationCheckResult values.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_NFKC_QUICK_CHECK = 0x100F,

    /// <summary>
    /// Enumerated property Lead_Canonical_Combining_Class.
    /// ICU-specific property for the ccc of the first code point
    /// of the decomposition, or lccc(c) = ccc(NFD(c)[0]).
    /// Useful for checking for canonically ordered text;
    /// see UNORM_FCD and http://www.unicode.org/notes/tn5/#FCD .
    /// Returns 8-bit numeric values like UCHAR_CANONICAL_COMBINING_CLASS.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_LEAD_CANONICAL_COMBINING_CLASS = 0x1010,

    /// <summary>
    /// Enumerated property Trail_Canonical_Combining_Class.
    /// ICU-specific property for the ccc of the last code point
    /// of the decomposition, or tccc(c) = ccc(NFD(c)[last]).
    /// Useful for checking for canonically ordered text;
    /// see UNORM_FCD and http://www.unicode.org/notes/tn5/#FCD .
    /// Returns 8-bit numeric values like UCHAR_CANONICAL_COMBINING_CLASS.
    ///
    /// Stable: ICU 3.0
    /// </summary>
    UCHAR_TRAIL_CANONICAL_COMBINING_CLASS = 0x1011,

    /// <summary>
    /// Enumerated property Grapheme_Cluster_Break (new in Unicode 4.1).
    /// Used in UAX #29: Text Boundaries
    /// (http://www.unicode.org/reports/tr29/)
    /// Returns UGraphemeClusterBreak values.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_GRAPHEME_CLUSTER_BREAK = 0x1012,

    /// <summary>
    /// Enumerated property Sentence_Break (new in Unicode 4.1).
    /// Used in UAX #29: Text Boundaries
    /// (http://www.unicode.org/reports/tr29/)
    /// Returns USentenceBreak values.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_SENTENCE_BREAK = 0x1013,

    /// <summary>
    /// Enumerated property Word_Break (new in Unicode 4.1).
    /// Used in UAX #29: Text Boundaries
    /// (http://www.unicode.org/reports/tr29/)
    /// Returns UWordBreakValues values.
    ///
    /// Stable: ICU 3.4
    /// </summary>
    UCHAR_WORD_BREAK = 0x1014,

    /// <summary>
    /// Enumerated property Bidi_Paired_Bracket_Type (new in Unicode 6.3).
    /// Used in UAX #9: Unicode Bidirectional Algorithm
    /// (http://www.unicode.org/reports/tr9/)
    /// Returns UBidiPairedBracketType values.
    ///
    /// Stable: ICU 52
    /// </summary>
    UCHAR_BIDI_PAIRED_BRACKET_TYPE = 0x1015,

    /// <summary>
    /// Enumerated property Indic_Positional_Category.
    /// New in Unicode 6.0 as provisional property Indic_Matra_Category;
    /// renamed and changed to informative in Unicode 8.0.
    /// See http://www.unicode.org/reports/tr44/#IndicPositionalCategory.txt
    ///
    /// Stable: ICU 63
    /// </summary>
    UCHAR_INDIC_POSITIONAL_CATEGORY = 0x1016,
    /// <summary>
    /// Enumerated property Indic_Syllabic_Category.
    /// New in Unicode 6.0 as provisional; informative since Unicode 8.0.
    /// See http://www.unicode.org/reports/tr44/#IndicSyllabicCategory.txt
    /// </summary>
    UCHAR_INDIC_SYLLABIC_CATEGORY = 0x1017,

    /// <summary>
    /// Enumerated property Vertical_Orientation.
    /// Used for UAX #50 Unicode Vertical Text Layout (https://www.unicode.org/reports/tr50/).
    /// New as a UCD property in Unicode 10.0.
    /// </summary>
    UCHAR_VERTICAL_ORIENTATION = 0x1018,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for enumerated/integer Unicode properties.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UCHAR_INT_LIMIT = 0x1019,
    #endif

    /// <summary>
    /// Bitmask property General_Category_Mask.
    /// This is the General_Category property returned as a bit mask.
    /// When used in u_getIntPropertyValue(c), same as U_MASK(u_charType(c)),
    /// returns bit masks for UCharCategory values where exactly one bit is set.
    /// When used with u_getPropertyValueName() and u_getPropertyValueEnum(),
    /// a multi-bit mask is used for sets of categories like "Letters".
    /// Mask values should be cast to uint32_t.
    /// </summary>
    UCHAR_GENERAL_CATEGORY_MASK = 0x2000,

    /// <summary>
    /// First constant for bit-mask Unicode properties.
    /// </summary>
    UCHAR_MASK_START = UCHAR_GENERAL_CATEGORY_MASK,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for bit-mask Unicode properties.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UCHAR_MASK_LIMIT = 0x2001,
    #endif

    /// <summary>
    /// Double property Numeric_Value.
    /// Corresponds to u_getNumericValue.
    /// </summary>
    UCHAR_NUMERIC_VALUE = 0x3000,

    /// <summary>
    /// First constant for double Unicode properties.
    /// </summary>
    UCHAR_DOUBLE_START = UCHAR_NUMERIC_VALUE,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for double Unicode properties.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UCHAR_DOUBLE_LIMIT = 0x3001,
    #endif

    /// <summary>
    /// String property Age.
    /// Corresponds to u_charAge.
    /// </summary>
    UCHAR_AGE = 0x4000,

    /// <summary>
    /// First constant for string Unicode properties.
    /// </summary>
    UCHAR_STRING_START = UCHAR_AGE,

    /// <summary>
    /// String property Bidi_Mirroring_Glyph.
    /// Corresponds to u_charMirror.
    /// </summary>
    UCHAR_BIDI_MIRRORING_GLYPH = 0x4001,

    /// <summary>
    /// String property Case_Folding.
    /// Corresponds to u_strFoldCase in ustring.h.
    /// </summary>
    UCHAR_CASE_FOLDING = 0x4002,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// Deprecated string property ISO_Comment.
    /// Corresponds to u_getISOComment.
    /// </summary>
    /// <deprecated>ICU 49</deprecated>
    UCHAR_ISO_COMMENT = 0x4003,
    #endif

    /// <summary>
    /// String property Lowercase_Mapping.
    /// Corresponds to u_strToLower in ustring.h.
    /// </summary>
    UCHAR_LOWERCASE_MAPPING = 0x4004,

    /// <summary>
    /// String property Name.
    /// Corresponds to u_charName.
    /// </summary>
    UCHAR_NAME = 0x4005,

    /// <summary>
    /// String property Simple_Case_Folding.
    /// Corresponds to u_foldCase.
    /// </summary>
    UCHAR_SIMPLE_CASE_FOLDING = 0x4006,

    /// <summary>
    /// String property Simple_Lowercase_Mapping.
    /// Corresponds to u_tolower.
    /// </summary>
    UCHAR_SIMPLE_LOWERCASE_MAPPING = 0x4007,

    /// <summary>
    /// String property Simple_Titlecase_Mapping.
    /// Corresponds to u_totitle.
    /// </summary>
    UCHAR_SIMPLE_TITLECASE_MAPPING = 0x4008,

    /// <summary>
    /// String property Simple_Uppercase_Mapping.
    /// Corresponds to u_toupper.
    /// </summary>
    UCHAR_SIMPLE_UPPERCASE_MAPPING = 0x4009,

    /// <summary>
    /// String property Titlecase_Mapping.
    /// Corresponds to u_strToTitle in ustring.h.
    /// </summary>
    UCHAR_TITLECASE_MAPPING = 0x400A,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// String property Unicode_1_Name.
    /// This property is of little practical value.
    /// Beginning with ICU 49, ICU APIs return an empty string for this property.
    /// Corresponds to u_charName(U_UNICODE_10_CHAR_NAME).
    /// </summary>
    /// <deprecated>ICU 49</deprecated>
    UCHAR_UNICODE_1_NAME = 0x400B,
    #endif

    /// <summary>
    /// String property Uppercase_Mapping.
    /// Corresponds to u_strToUpper in ustring.h.
    /// </summary>
    UCHAR_UPPERCASE_MAPPING = 0x400C,

    /// <summary>
    /// String property Bidi_Paired_Bracket (new in Unicode 6.3).
    /// Corresponds to u_getBidiPairedBracket.
    /// </summary>
    UCHAR_BIDI_PAIRED_BRACKET = 0x400D,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for string Unicode properties.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UCHAR_STRING_LIMIT = 0x400E,
    #endif

    /// <summary>
    /// Miscellaneous property Script_Extensions (new in Unicode 6.0).
    /// Some characters are commonly used in multiple scripts.
    /// For more information, see UAX #24: http://www.unicode.org/reports/tr24/.
    /// Corresponds to uscript_hasScript and uscript_getScriptExtensions in uscript.h.
    /// </summary>
    UCHAR_SCRIPT_EXTENSIONS = 0x7000,

    /// <summary>
    /// First constant for Unicode properties with unusual value types.
    /// </summary>
    UCHAR_OTHER_PROPERTY_START = UCHAR_SCRIPT_EXTENSIONS,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// One more than the last constant for Unicode properties with unusual value types.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UCHAR_OTHER_PROPERTY_LIMIT = 0x7001,
    #endif

    /// <summary>
    /// Represents a nonexistent or invalid property or property value.
    /// </summary>
    UCHAR_INVALID_CODE = -1
}
