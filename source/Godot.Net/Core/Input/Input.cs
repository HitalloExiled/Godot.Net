namespace Godot.Net.Core.Input;

using Godot.Net.Core.Config;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Extensions;

#pragma warning disable IDE0021, CA1822, IDE0060, CS0649 // TODO - REMOVE

public partial class Input
{
    private static readonly Dictionary<string, JoyButton> joyButtons = new()
    {
        ["a"]             = JoyButton.A,
        ["b"]             = JoyButton.B,
        ["x"]             = JoyButton.X,
        ["y"]             = JoyButton.Y,
        ["back"]          = JoyButton.BACK,
        ["guide"]         = JoyButton.GUIDE,
        ["start"]         = JoyButton.START,
        ["leftstick"]     = JoyButton.LEFT_STICK,
        ["rightstick"]    = JoyButton.RIGHT_STICK,
        ["leftshoulder"]  = JoyButton.LEFT_SHOULDER,
        ["rightshoulder"] = JoyButton.RIGHT_SHOULDER,
        ["dpup"]          = JoyButton.DPAD_UP,
        ["dpdown"]        = JoyButton.DPAD_DOWN,
        ["dpleft"]        = JoyButton.DPAD_LEFT,
        ["dpright"]       = JoyButton.DPAD_RIGHT,
        ["misc1"]         = JoyButton.MISC1,
        ["paddle1"]       = JoyButton.PADDLE1,
        ["paddle2"]       = JoyButton.PADDLE2,
        ["paddle3"]       = JoyButton.PADDLE3,
        ["paddle4"]       = JoyButton.PADDLE4,
        ["touchpad"]      = JoyButton.TOUCHPAD,
    };

    private static readonly Dictionary<string, JoyAxis> joyAxes = new()
    {
        ["leftx"]        = JoyAxis.LEFT_X,
        ["lefty"]        = JoyAxis.LEFT_Y,
        ["rightx"]       = JoyAxis.RIGHT_X,
        ["righty"]       = JoyAxis.RIGHT_X,
        ["lefttrigger"]  = JoyAxis.TRIGGER_LEFT,
        ["righttrigger"] = JoyAxis.TRIGGER_RIGHT,
    };

    private static readonly object padlock    = new();

    private static Input? singleton;
    private static bool   firstPrint = true;
    private long          lastParsedFrame;


    public static bool  IsInitialized => singleton != null;
    public static Input Singleton     => singleton ?? throw new NullReferenceException();

    private readonly HashSet<InputEvent>    bufferedEvents    = new();
    private readonly HashSet<InputEvent>    frameParsedEvents = new();
    private readonly List<JoyDeviceMapping> mapDb             = new();

    private readonly bool useAccumulatedInput = true;
    private readonly bool useInputBuffering;

    #region public properties
    public bool           EmulatingMouseFromTouch { get; set; }
    public bool           EmulatingTouchFromMouse { get; set; }
    public Vector2<RealT> LastMouseVelocity       { get; set; }
    public Vector2<RealT> MousePosition           { get; set; }
    #endregion public properties

    public Input()
    {
        singleton = this;

        // Parse default mappings.
        {
            foreach (var mapping in DefaultControllerMappings.Mappings)
            {
                this.ParseMapping(mapping);
            }
        }

        // If defined, parse SDL_GAMECONTROLLERCONFIG for possible new mappings/overrides.
        var envMapping = Environment.GetEnvironmentVariable("SDL_GAMECONTROLLERCONFIG");
        if (envMapping != null)
        {
            var entries = envMapping.Split('\n');

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry))
                {
                    continue;
                }

                this.ParseMapping(entry);
            }
        }
    }

    private void ParseInputEventImpl(InputEvent @event, bool isEmulated) => throw new NotImplementedException();

    public JoyAxis GetOutputAxis(string output) =>
        joyAxes.TryGetValue(output, out var joyAxe) ? joyAxe : JoyAxis.INVALID;

    public JoyButton GetOutputButton(string output) =>
        joyButtons.TryGetValue(output, out var joyButton) ? joyButton : JoyButton.INVALID;

    public void ParseMapping(string mapping)
    {
        lock (padlock)
        {
            var entry = mapping.Split(',');
            if (entry.Length < 2)
            {
                return;
            }


            var joyDeviceMapping = new JoyDeviceMapping
            {
                Uid  = entry[0],
                Name = entry[1]
            };

            var idx = 1;
            while (++idx < entry.Length)
            {
                if (string.IsNullOrEmpty(entry[idx]))
                {
                    continue;
                }

                var slices = entry[idx].Split(":");

                var output = slices[0].Replace(" ", "");
                var input  = slices[1].Replace(" ", "");
                if (ERR_CONTINUE_MSG(output.Length < 1 || input.Length < 2, $"Invalid device mapping entry \"{entry[idx]}\" in mapping:\n{mapping}s"))
                {
                    return;
                }

                if (output == "platform" || output == "hint")
                {
                    continue;
                }

                var outputRange = JoyAxisRange.FULL_AXIS;
                if (output[0] == '+' || output[0] == '-')
                {
                    if (ERR_CONTINUE_MSG(output.Length < 2, $"Invalid output entry \"{entry[idx]}\" in mapping:\n{mapping}"))
                    {
                        continue;
                    }

                    if (output[0] == '+')
                    {
                        outputRange = JoyAxisRange.POSITIVE_HALF_AXIS;
                    }
                    else if (output[0] == '-')
                    {
                        outputRange = JoyAxisRange.NEGATIVE_HALF_AXIS;
                    }
                    output = output[1..];
                }

                var inputRange = JoyAxisRange.FULL_AXIS;

                if (input[0] == '+')
                {
                    inputRange = JoyAxisRange.POSITIVE_HALF_AXIS;
                    input = input[1..];
                }
                else if (input[0] == '-')
                {
                    inputRange = JoyAxisRange.NEGATIVE_HALF_AXIS;
                    input = input[1..];
                }

                var invertAxis = false;

                if (input[^1] == '~')
                {
                    invertAxis = true;
                    input = input.Left(input.Length - 1);
                }

                var outputButton = this.GetOutputButton(output);
                var outputAxis   = this.GetOutputAxis(output);

                if (outputButton == JoyButton.INVALID && outputAxis == JoyAxis.INVALID)
                {
                    PrintVerbose($"Unrecognized output string \"{output}\" in mapping:\n{mapping}");
                }
                if (ERR_CONTINUE_MSG(outputButton != JoyButton.INVALID && outputAxis != JoyAxis.INVALID, $"Output string \"{output}\" matched both button and axis in mapping:\n{mapping}"))
                {
                    continue;
                }

                var binding = new JoyBinding();

                if (outputButton != JoyButton.INVALID)
                {
                    binding.Output.Type   = JoyType.TYPE_BUTTON;
                    binding.Output.Button = outputButton;
                }
                else if (outputAxis != JoyAxis.INVALID)
                {
                    binding.Output.Type  = JoyType.TYPE_AXIS;
                    binding.Output.Axis  = outputAxis;
                    binding.Output.Range = outputRange;
                }

                switch (input[0])
                {
                    case 'b':
                        binding.Input.Type = JoyType.TYPE_BUTTON;
                        binding.Input.Button = (JoyButton)int.Parse(input[1..]);
                        break;
                    case 'a':
                        binding.Input.Type = JoyType.TYPE_AXIS;
                        binding.Input.Axis = (JoyAxis)int.Parse(input[1..]);
                        binding.Input.Range = inputRange;
                        binding.Input.Invert = invertAxis;
                        break;
                    case 'h':
                        if (ERR_CONTINUE_MSG(input.Length != 4 || input[2] != '.', $"Invalid had input \"{input}\" in mapping:\n{mapping}"))
                        {
                            return;
                        }

                        binding.Input.Type    = JoyType.TYPE_HAT;
                        binding.Input.Hat     = (HatDir)int.Parse(input.Substring(1, 1));
                        binding.Input.HatMask = (HatMask)int.Parse(input[3..]);
                        break;
                    default:
                        ERR_CONTINUE_MSG(true, $"Unrecognized input string \"{input}\" in mapping:\n{mapping}");
                        continue;
                }

                joyDeviceMapping.Bindings.Add(binding);
            }

            this.mapDb.Add(joyDeviceMapping);
        }
    }

    public void FlushBufferedEvents() => throw new NotImplementedException();
    public bool IsKeyPressed(Key keycode) => throw new NotImplementedException();
    public bool IsMouseButtonPressed(MouseButton button) => throw new NotImplementedException();

    public void ParseInputEvent(InputEvent @event)
    {
        lock (padlock)
        {
            // ERR_FAIL_COND(p_event.is_null());

            #if DEBUG
            var currFrame = Engine.Singleton.ProcessFrames;
            if (currFrame != this.lastParsedFrame)
            {
                this.frameParsedEvents.Clear();
                this.lastParsedFrame = currFrame;
                this.frameParsedEvents.Add(@event);
            }
            else if (this.frameParsedEvents.Contains(@event))
            {
                // It would be technically safe to send the same event in cases such as:
                // - After an explicit flush.
                // - In platforms using buffering when agile flushing is enabled, after one of the mid-frame flushes.
                // - If platform doesn't use buffering and event accumulation is disabled.
                // - If platform doesn't use buffering and the event type is not accumulable.
                // However, it wouldn't be reasonable to ask users to remember the full ruleset and be aware at all times
                // of the possibilities of the target platform, project settings and engine internals, which may change
                // without prior notice.
                // Therefore, the guideline is, "don't send the same event object more than once per frame".
                WARN_PRINT_ONCE(
                    """
                    An input event object is being parsed more than once in the same frame, which is unsafe.
                    If you are generating events in a script, you have to instantiate a new event instead of sending the same one more than once, unless the original one was sent on an earlier frame.
                    You can call duplicate() on the event to get a new instance with identical values.
                    """,
                    ref firstPrint
                );
            }
            else
            {
                this.frameParsedEvents.Add(@event);
            }
            #endif

            if (this.useAccumulatedInput)
            {
                if (this.bufferedEvents.Count == 0 || !this.bufferedEvents.Last().Accumulate(@event))
                {
                    this.bufferedEvents.Add(@event);
                }
            }
            else if (this.useInputBuffering)
            {
                this.bufferedEvents.Add(@event);
            }
            else
            {
                this.ParseInputEventImpl(@event, false);
            }
        }
    }

    public void ReleasePressedEvents() => throw new NotImplementedException();
}
