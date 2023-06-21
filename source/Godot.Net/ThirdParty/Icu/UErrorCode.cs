namespace Godot.Net.ThirdParty.Icu;
#pragma warning disable CA1069

/// <summary>
/// Standard ICU4C error code type, a substitute for exceptions.
///</summary>
/// <remarks>
/// Initialize the <see cref="UErrorCode"/> with <see cref="U_ZERO_ERROR"/>, and check for success or
/// failure using <see cref="U_SUCCESS"/> or <see cref="U_FAILURE"/>:
/// <code>
/// UErrorCode errorCode = U_ZERO_ERROR;
/// // call ICU API that needs an error code parameter.
/// if (U_FAILURE(errorCode))
/// {
///     // An error occurred. Handle it here.
/// }
/// </code>
/// C++ code should use <see cref="icu.ErrorCode"/>, available in unicode/errorcode.h, or a suitable subclass.
/// For more information, see:
/// https://unicode-org.github.io/icu/userguide/dev/codingguidelines#details-about-icu-error-codes
/// Note: By convention, ICU functions that take a reference (C++) or a pointer (C) to a <see cref="UErrorCode"/> first test:
/// <code>
/// if (U_FAILURE(errorCode)) { return immediately; }
/// </code>
/// so that in a chain of such functions the first one that sets an error code
/// causes the following ones to not perform any operations.
/// </remarks>
public enum UErrorCode
{
    /* The ordering of U_ERROR_INFO_START Vs U_USING_FALLBACK_WARNING looks weird
     * and is that way because VC++ debugger displays first encountered constant,
     * which is not the what the code is used for
     */

    /// <summary>A resource bundle lookup returned a fallback result (not an error)</summary>
    U_USING_FALLBACK_WARNING = -128,

    /// <summary>Start of information results (semantically successful)</summary>
    U_ERROR_WARNING_START = -128,

    /// <summary>A resource bundle lookup returned a result from the root locale (not an error)</summary>
    U_USING_DEFAULT_WARNING = -127,

    /// <summary>A SafeClone operation required allocating memory (informational only)</summary>
    U_SAFECLONE_ALLOCATED_WARNING = -126,

    /// <summary>ICU has to use compatibility layer to construct the service. Expect performance/memory usage degradation. Consider upgrading</summary>
    U_STATE_OLD_WARNING = -125,

    /// <summary>An output string could not be NUL-terminated because output length==destCapacity.</summary>
    U_STRING_NOT_TERMINATED_WARNING = -124,

    /// <summary>Number of levels requested in getBound is higher than the number of levels in the sort key</summary>
    U_SORT_KEY_TOO_SHORT_WARNING = -123,

    /// <summary>This converter alias can go to different converter implementations</summary>
    U_AMBIGUOUS_ALIAS_WARNING = -122,

    /// <summary>ucol_open encountered a mismatch between UCA version and collator image version, so the collator was constructed from rules. No impact to further function</summary>
    U_DIFFERENT_UCA_VERSION = -121,

    /// <summary>A plugin caused a level change. May not be an error, but later plugins may not load.</summary>
    U_PLUGIN_CHANGED_LEVEL_WARNING = -120,

#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal UErrorCode warning value</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_ERROR_WARNING_LIMIT,
#endif

    /// <summary>No error, no warning.</summary>
    U_ZERO_ERROR = 0,

    /// <summary>Start of codes indicating failure</summary>
    U_ILLEGAL_ARGUMENT_ERROR = 1,

    /// <summary>The requested resource cannot be found</summary>
    U_MISSING_RESOURCE_ERROR = 2,

    /// <summary>Data format is not what is expected</summary>
    U_INVALID_FORMAT_ERROR = 3,

    /// <summary>The requested file cannot be found</summary>
    U_FILE_ACCESS_ERROR = 4,

    /// <summary>Indicates a bug in the library code</summary>
    U_INTERNAL_PROGRAM_ERROR = 5,

    /// <summary>Unable to parse a message (message format)</summary>
    U_MESSAGE_PARSE_ERROR = 6,

    /// <summary>Memory allocation error</summary>
    U_MEMORY_ALLOCATION_ERROR = 7,

    /// <summary>Trying to access the index that is out of bounds</summary>
    U_INDEX_OUTOFBOUNDS_ERROR = 8,

    /// <summary>Equivalent to Java ParseException</summary>
    U_PARSE_ERROR = 9,

    /// <summary>Character conversion: Unmappable input sequence. In other APIs: Invalid character.</summary>
    U_INVALID_CHAR_FOUND = 10,

    /// <summary>Character conversion: Incomplete input sequence.</summary>
    U_TRUNCATED_CHAR_FOUND = 11,

    /// <summary>Character conversion: Illegal input sequence/combination of input units.</summary>
    U_ILLEGAL_CHAR_FOUND = 12,

    /// <summary>Conversion table file found, but corrupted</summary>
    U_INVALID_TABLE_FORMAT = 13,

    /// <summary>Conversion table file not found</summary>
    U_INVALID_TABLE_FILE = 14,

    /// <summary>A result would not fit in the supplied buffer</summary>
    U_BUFFER_OVERFLOW_ERROR = 15,

    /// <summary>Requested operation not supported in current context</summary>
    U_UNSUPPORTED_ERROR = 16,

    /// <summary>an operation is requested over a resource that does not support it</summary>
    U_RESOURCE_TYPE_MISMATCH = 17,

    /// <summary>ISO-2022 illegal escape sequence</summary>
    U_ILLEGAL_ESCAPE_SEQUENCE = 18,

    /// <summary>ISO-2022 unsupported escape sequence</summary>
    U_UNSUPPORTED_ESCAPE_SEQUENCE = 19,

    /// <summary>No space available for in-buffer expansion for Arabic shaping</summary>
    U_NO_SPACE_AVAILABLE = 20,

    /// <summary>Currently used only while setting variable top, but can be used generally</summary>
    U_CE_NOT_FOUND_ERROR = 21,

    /// <summary>User tried to set variable top to a primary that is longer than two bytes</summary>
    U_PRIMARY_TOO_LONG_ERROR = 22,

    /// <summary>ICU cannot construct a service from this state, as it is no longer supported</summary>
    U_STATE_TOO_OLD_ERROR = 23,

    /// <summary>
    /// There are too many aliases in the path to the requested resource.
    /// It is very possible that a circular alias definition has occurred
    ///</summary>
    U_TOO_MANY_ALIASES_ERROR = 24,

    /// <summary>UEnumeration out of sync with underlying collection</summary>
    U_ENUM_OUT_OF_SYNC_ERROR = 25,

    /// <summary>Unable to convert a UChar* string to char* with the invariant converter.</summary>
    U_INVARIANT_CONVERSION_ERROR = 26,

    /// <summary>Requested operation can not be completed with ICU in its current state</summary>
    U_INVALID_STATE_ERROR = 27,

    /// <summary>Collator version is not compatible with the base version</summary>
    U_COLLATOR_VERSION_MISMATCH = 28,

    /// <summary>Collator is options only and no base is specified</summary>
    U_USELESS_COLLATOR_ERROR = 29,

    /// <summary>Attempt to modify read-only or constant data.</summary>
    U_NO_WRITE_PERMISSION = 30,
    /**
     * The input is impractically long for an operation.
     * It is rejected because it may lead to problems such as excessive
     * processing time, stack depth, or heap memory requirements.
     *
     * @stable ICU 68
     */
    U_INPUT_TOO_LONG_ERROR = 31,

#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest standard error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_STANDARD_ERROR_LIMIT = 32,
#endif

    /*
     * Error codes in the range 0x10000 0x10100 are reserved for Transliterator.
     */

    /// <summary>Missing '$' or duplicate variable name</summary>
    U_BAD_VARIABLE_DEFINITION = 0x10000,

    /// <summary>Start of Transliterator errors</summary>
    U_PARSE_ERROR_START = 0x10000,

    /// <summary>Elements of a rule are misplaced</summary>
    U_MALFORMED_RULE,

    /// <summary>A UnicodeSet pattern is invalid</summary>
    U_MALFORMED_SET,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_MALFORMED_SYMBOL_REFERENCE,

    /// <summary>A Unicode escape pattern is invalid</summary>
    U_MALFORMED_UNICODE_ESCAPE,

    /// <summary>A variable definition is invalid</summary>
    U_MALFORMED_VARIABLE_DEFINITION,

    /// <summary>A variable reference is invalid</summary>
    U_MALFORMED_VARIABLE_REFERENCE,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_MISMATCHED_SEGMENT_DELIMITERS,

    /// <summary>A start anchor appears at an illegal position</summary>
    U_MISPLACED_ANCHOR_START,

    /// <summary>A cursor offset occurs at an illegal position</summary>
    U_MISPLACED_CURSOR_OFFSET,

    /// <summary>A quantifier appears after a segment close delimiter</summary>
    U_MISPLACED_QUANTIFIER,

    /// <summary>A rule contains no operator</summary>
    U_MISSING_OPERATOR,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_MISSING_SEGMENT_CLOSE,

    /// <summary>More than one ante context</summary>
    U_MULTIPLE_ANTE_CONTEXTS,

    /// <summary>More than one cursor</summary>
    U_MULTIPLE_CURSORS,

    /// <summary>More than one post context</summary>
    U_MULTIPLE_POST_CONTEXTS,

    /// <summary>A dangling backslash</summary>
    U_TRAILING_BACKSLASH,

    /// <summary>A segment reference does not correspond to a defined segment</summary>
    U_UNDEFINED_SEGMENT_REFERENCE,

    /// <summary>A variable reference does not correspond to a defined variable</summary>
    U_UNDEFINED_VARIABLE,

    /// <summary>A special character was not quoted or escaped</summary>
    U_UNQUOTED_SPECIAL,

    /// <summary>A closing single quote is missing</summary>
    U_UNTERMINATED_QUOTE,

    /// <summary>A rule is hidden by an earlier more general rule</summary>
    U_RULE_MASK_ERROR,

    /// <summary>A compound filter is in an invalid location</summary>
    U_MISPLACED_COMPOUND_FILTER,

    /// <summary>More than one compound filter</summary>
    U_MULTIPLE_COMPOUND_FILTERS,

    /// <summary>A "::id" rule was passed to the RuleBasedTransliterator parser</summary>
    U_INVALID_RBT_SYNTAX,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_INVALID_PROPERTY_PATTERN,

    /// <summary>A 'use' pragma is invalid</summary>
    U_MALFORMED_PRAGMA,

    /// <summary>A closing ')' is missing</summary>
    U_UNCLOSED_SEGMENT,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_ILLEGAL_CHAR_IN_SEGMENT,

    /// <summary>Too many stand-ins generated for the given variable range</summary>
    U_VARIABLE_RANGE_EXHAUSTED,

    /// <summary>The variable range overlaps characters used in rules</summary>
    U_VARIABLE_RANGE_OVERLAP,

    /// <summary>A special character is outside its allowed context</summary>
    U_ILLEGAL_CHARACTER,

    /// <summary>Internal transliterator system error</summary>
    U_INTERNAL_TRANSLITERATOR_ERROR,

    /// <summary>A "::id" rule specifies an unknown transliterator</summary>
    U_INVALID_ID,

    /// <summary>A "&fn()" rule specifies an unknown transliterator</summary>
    U_INVALID_FUNCTION,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal Transliterator error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420.")]
    U_PARSE_ERROR_LIMIT,
#endif

    /*
     * Error codes in the range 0x10100 0x10200 are reserved for the formatting API.
     */

    /// <summary>Syntax error in format pattern</summary>
    U_UNEXPECTED_TOKEN = 0x10100,

    /// <summary>Start of format library errors</summary>
    U_FMT_PARSE_ERROR_START = 0x10100,

    /// <summary>More than one decimal separator in number pattern</summary>
    U_MULTIPLE_DECIMAL_SEPARATORS,

    /// <summary>Typo: kept for backward compatibility. Use U_MULTIPLE_DECIMAL_SEPARATORS</summary>
    U_MULTIPLE_DECIMAL_SEPERATORS = U_MULTIPLE_DECIMAL_SEPARATORS,

    /// <summary>More than one exponent symbol in number pattern</summary>
    U_MULTIPLE_EXPONENTIAL_SYMBOLS,

    /// <summary>Grouping symbol in exponent pattern</summary>
    U_MALFORMED_EXPONENTIAL_PATTERN,

    /// <summary>More than one percent symbol in number pattern</summary>
    U_MULTIPLE_PERCENT_SYMBOLS,

    /// <summary>More than one permill symbol in number pattern</summary>
    U_MULTIPLE_PERMILL_SYMBOLS,

    /// <summary>More than one pad symbol in number pattern</summary>
    U_MULTIPLE_PAD_SPECIFIERS,

    /// <summary>Syntax error in format pattern</summary>
    U_PATTERN_SYNTAX_ERROR,

    /// <summary>Pad symbol misplaced in number pattern</summary>
    U_ILLEGAL_PAD_POSITION,

    /// <summary>Braces do not match in message pattern</summary>
    U_UNMATCHED_BRACES,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_UNSUPPORTED_PROPERTY,

    /// <summary>UNUSED as of ICU 2.4</summary>
    U_UNSUPPORTED_ATTRIBUTE,

    /// <summary>Argument name and argument index mismatch in MessageFormat functions</summary>
    U_ARGUMENT_TYPE_MISMATCH,

    /// <summary>Duplicate keyword in PluralFormat</summary>
    U_DUPLICATE_KEYWORD,

    /// <summary>Undefined Plural keyword</summary>
    U_UNDEFINED_KEYWORD,

    /// <summary>Missing DEFAULT rule in plural rules</summary>
    U_DEFAULT_KEYWORD_MISSING,

    /// <summary>Decimal number syntax error</summary>
    U_DECIMAL_NUMBER_SYNTAX_ERROR,

    /// <summary>Cannot format a number exactly and rounding mode is ROUND_UNNECESSARY @stable ICU 4.8</summary>
    U_FORMAT_INEXACT_ERROR,

    /// <summary>The argument to a NumberFormatter helper method was out of bounds; the bounds are usually 0 to 999. @stable ICU 61</summary>
    U_NUMBER_ARG_OUTOFBOUNDS_ERROR,

    /// <summary>The number skeleton passed to C++ NumberFormatter or C UNumberFormatter was invalid or contained a syntax error. @stable ICU 62</summary>
    U_NUMBER_SKELETON_SYNTAX_ERROR,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal formatting API error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_FMT_PARSE_ERROR_LIMIT = 0x10114,
#endif

    /*
     * Error codes in the range 0x10200 0x102ff are reserved for BreakIterator.
     */
    /// <summary>An internal error (bug) was detected.</summary>
    U_BRK_INTERNAL_ERROR = 0x10200,

    /// <summary>Start of codes indicating Break Iterator failures</summary>
    U_BRK_ERROR_START = 0x10200,

    /// <summary>Hex digits expected as part of a escaped char in a rule.</summary>
    U_BRK_HEX_DIGITS_EXPECTED,

    /// <summary>Missing ';' at the end of a RBBI rule.</summary>
    U_BRK_SEMICOLON_EXPECTED,

    /// <summary>Syntax error in RBBI rule.</summary>
    U_BRK_RULE_SYNTAX,

    /// <summary>UnicodeSet writing an RBBI rule missing a closing ']'.</summary>
    U_BRK_UNCLOSED_SET,

    /// <summary>Syntax error in RBBI rule assignment statement.</summary>
    U_BRK_ASSIGN_ERROR,

    /// <summary>RBBI rule $Variable redefined.</summary>
    U_BRK_VARIABLE_REDFINITION,

    /// <summary>Mis-matched parentheses in an RBBI rule.</summary>
    U_BRK_MISMATCHED_PAREN,

    /// <summary>Missing closing quote in an RBBI rule.</summary>
    U_BRK_NEW_LINE_IN_QUOTED_STRING,

    /// <summary>Use of an undefined $Variable in an RBBI rule.</summary>
    U_BRK_UNDEFINED_VARIABLE,

    /// <summary>Initialization failure.  Probable missing ICU Data.</summary>
    U_BRK_INIT_ERROR,

    /// <summary>Rule contains an empty Unicode Set.</summary>
    U_BRK_RULE_EMPTY_SET,

    /// <summary>!!option in RBBI rules not recognized.</summary>
    U_BRK_UNRECOGNIZED_OPTION,

    /// <summary>The {nnn} tag on a rule is malformed</summary>
    U_BRK_MALFORMED_RULE_TAG,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal BreakIterator error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_BRK_ERROR_LIMIT,
#endif

    /*
     * Error codes in the range 0x10300-0x103ff are reserved for regular expression related errors.
     */
    /// <summary>An internal error (bug) was detected.</summary>
    U_REGEX_INTERNAL_ERROR = 0x10300,
    /// <summary>Start of codes indicating Regexp failures</summary>
    U_REGEX_ERROR_START = 0x10300,
    /// <summary>Syntax error in regexp pattern.</summary>
    U_REGEX_RULE_SYNTAX,

    /// <summary>RegexMatcher in invalid state for requested operation</summary>
    U_REGEX_INVALID_STATE,

    /// <summary>Unrecognized backslash escape sequence in pattern</summary>
    U_REGEX_BAD_ESCAPE_SEQUENCE,

    /// <summary>Incorrect Unicode property</summary>
    U_REGEX_PROPERTY_SYNTAX,

    /// <summary>Use of regexp feature that is not yet implemented.</summary>
    U_REGEX_UNIMPLEMENTED,

    /// <summary>Incorrectly nested parentheses in regexp pattern.</summary>
    U_REGEX_MISMATCHED_PAREN,

    /// <summary>Decimal number is too large.</summary>
    U_REGEX_NUMBER_TOO_BIG,

    /// <summary>Error in {min,max} interval</summary>
    U_REGEX_BAD_INTERVAL,

    /// <summary>In {min,max}, max is less than min.</summary>
    U_REGEX_MAX_LT_MIN,

    /// <summary>Back-reference to a non-existent capture group.</summary>
    U_REGEX_INVALID_BACK_REF,

    /// <summary>Invalid value for match mode flags.</summary>
    U_REGEX_INVALID_FLAG,

    /// <summary>Look-Behind pattern matches must have a bounded maximum length.</summary>
    U_REGEX_LOOK_BEHIND_LIMIT,

    /// <summary>Regexps cannot have UnicodeSets containing strings.</summary>
    U_REGEX_SET_CONTAINS_STRING,
#if !U_HIDE_DEPRECATED_API

    /// <summary>Octal character constants must be <= 0377.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_REGEX_OCTAL_TOO_BIG,
#endif
    U_REGEX_MISSING_CLOSE_BRACKET = U_REGEX_SET_CONTAINS_STRING + 2, /**< Missing closing bracket on a bracket expression. */
    /// <summary>In a character range [x-y], x is greater than y.</summary>
    U_REGEX_INVALID_RANGE,

    /// <summary>Regular expression backtrack stack overflow.</summary>
    U_REGEX_STACK_OVERFLOW,

    /// <summary>Maximum allowed match time exceeded</summary>
    U_REGEX_TIME_OUT,

    /// <summary>Matching operation aborted by user callback fn.</summary>
    U_REGEX_STOPPED_BY_CALLER,

    /// <summary>Pattern exceeds limits on size or complexity. @stable ICU 55</summary>
    U_REGEX_PATTERN_TOO_BIG,

    /// <summary>Invalid capture group name. @stable ICU 55</summary>
    U_REGEX_INVALID_CAPTURE_GROUP_NAME,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal regular expression error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_REGEX_ERROR_LIMIT = U_REGEX_STOPPED_BY_CALLER + 3,
#endif

    /*
     * Error codes in the range 0x10400-0x104ff are reserved for IDNA related error codes.
     */
    U_IDNA_PROHIBITED_ERROR = 0x10400,
    U_IDNA_ERROR_START = 0x10400,
    U_IDNA_UNASSIGNED_ERROR,
    U_IDNA_CHECK_BIDI_ERROR,
    U_IDNA_STD3_ASCII_RULES_ERROR,
    U_IDNA_ACE_PREFIX_ERROR,
    U_IDNA_VERIFICATION_ERROR,
    U_IDNA_LABEL_TOO_LONG_ERROR,
    U_IDNA_ZERO_LENGTH_LABEL_ERROR,
    U_IDNA_DOMAIN_NAME_TOO_LONG_ERROR,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal IDNA error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_IDNA_ERROR_LIMIT,
#endif
    /*
     * Aliases for StringPrep
     */
    U_STRINGPREP_PROHIBITED_ERROR = U_IDNA_PROHIBITED_ERROR,
    U_STRINGPREP_UNASSIGNED_ERROR = U_IDNA_UNASSIGNED_ERROR,
    U_STRINGPREP_CHECK_BIDI_ERROR = U_IDNA_CHECK_BIDI_ERROR,

    /*
     * Error codes in the range 0x10500-0x105ff are reserved for Plugin related error codes.
     */
    /// <summary>Start of codes indicating plugin failures</summary>
    U_PLUGIN_ERROR_START = 0x10500,
    /// <summary>The plugin's level is too high to be loaded right now.</summary>
    U_PLUGIN_TOO_HIGH = 0x10500,
    /// <summary>The plugin didn't call uplug_setPlugLevel in response to a QUERY</summary>
    U_PLUGIN_DIDNT_SET_LEVEL,
#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal plug-in error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_PLUGIN_ERROR_LIMIT,
#endif

#if !U_HIDE_DEPRECATED_API
    /// <summary>One more than the highest normal error code.</summary>
    [Obsolete("ICU 58 The numeric value may change over time, see ICU ticket #12420")]
    U_ERROR_LIMIT = U_PLUGIN_ERROR_LIMIT
#endif
}
