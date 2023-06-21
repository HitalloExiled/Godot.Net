namespace Godot.Net.Core.Input;

public enum JoyAxis
{
    INVALID       = -1,
    LEFT_X        = 0,
    LEFT_Y        = 1,
    RIGHT_X       = 2,
    RIGHT_Y       = 3,
    TRIGGER_LEFT  = 4,
    TRIGGER_RIGHT = 5,
    SDL_MAX       = 6,
    MAX           = 10, // OpenVR supports up to 5 Joysticks making a total of 10 axes.
}
