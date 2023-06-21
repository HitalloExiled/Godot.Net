namespace Godot.Net.Modules.TextServerAdv;

using Godot.Net.ThirdParty.Icu;
using HarfBuzzSharp;

public partial class ScriptIterator
{
    private const int PAREN_STACK_DEPTH = 128;
    public readonly List<ScriptRange> ScriptRanges = new();

    public ScriptIterator(string @string, int start, int length)
    {
        if (start >= length)
        {
            start = length - 1;
        }

        if (start < 0)
        {
            start = 0;
        }

        var parenSize  = PAREN_STACK_DEPTH;
        var parenStack = new ParentStack[parenSize];
        var scriptEnd  = start;
        var parenSp    = -1;
        var startSp    = parenSp;
        try
        {
            var str = @string;

            do
            {
                var scriptCode = UScriptCode.COMMON;
                int scriptStart;
                for (scriptStart = scriptEnd; scriptEnd < length; scriptEnd++)
                {
                    var ch = str[scriptEnd];
                    var sc = (UScriptCode)Character.GetIntPropertyValue(ch, UProperty.UCHAR_SCRIPT);

                    if ((UBidiPairedBracketType)Character.GetIntPropertyValue(ch, UProperty.UCHAR_BIDI_PAIRED_BRACKET_TYPE) != UBidiPairedBracketType.NONE)
                    {
                        if ((UBidiPairedBracketType)Character.GetIntPropertyValue(ch, UProperty.UCHAR_BIDI_PAIRED_BRACKET_TYPE) == UBidiPairedBracketType.OPEN)
                        {
                            // If it's an open character, push it onto the stack.
                            parenSp++;
                            if (parenSp >= parenSize)
                            {
                                // If the stack is full, allocate more space to handle deeply nested parentheses. This is unlikely to happen with any real text.
                                parenSize += PAREN_STACK_DEPTH;
                                parenStack = new ParentStack[parenSize];
                            }
                            parenStack[parenSp].PairIndex = ch;
                            parenStack[parenSp].ScriptCode = scriptCode;
                        }
                        else if (parenSp >= 0)
                        {
                            // If it's a close character, find the matching open on the stack, and use that script code. Any non-matching open characters above it on the stack will be popped.
                            var pairedCh = (UBidiPairedBracketType)Character.GetIntPropertyValue(ch, UProperty.UCHAR_BIDI_PAIRED_BRACKET_TYPE);

                            while (parenSp >= 0 && parenStack[parenSp].PairIndex != (int)pairedCh)
                            {
                                parenSp -= 1;
                            }

                            if (parenSp < startSp)
                            {
                                startSp = parenSp;
                            }

                            if (parenSp >= 0)
                            {
                                sc = parenStack[parenSp].ScriptCode;
                            }
                        }
                    }

                    if (SameScript(scriptCode, sc))
                    {
                        if (scriptCode <= UScriptCode.INHERITED && sc > UScriptCode.INHERITED)
                        {
                            scriptCode = sc;
                            // Now that we have a final script code, fix any open characters we pushed before we knew the script code.
                            while (startSp < parenSp)
                            {
                                parenStack[++startSp].ScriptCode = scriptCode;
                            }
                        }
                        if ((UBidiPairedBracketType)Character.GetIntPropertyValue(ch, UProperty.UCHAR_BIDI_PAIRED_BRACKET_TYPE) == UBidiPairedBracketType.CLOSE && parenSp >= 0)
                        {
                            // If this character is a close paired character pop the matching open character from the stack.
                            parenSp -= 1;

                            if (startSp >= 0)
                            {
                                startSp -= 1;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                var rng = new ScriptRange
                {
                    Script = Script.Parse(scriptCode.ToString()),
                    Start  = scriptStart,
                    End    = scriptEnd
                };

                this.ScriptRanges.Add(rng);
            } while (scriptEnd < length);

        }
        catch (Exception exception)
        {
            ERR_FAIL_MSG(exception.Message);

            return;
        }
    }

    private static bool SameScript(UScriptCode scriptOne, UScriptCode scriptTwo) =>
        scriptOne <= UScriptCode.INHERITED || scriptTwo <= UScriptCode.INHERITED || scriptOne == scriptTwo;
}
