namespace Godot.Net.Core.Input;

public enum JoyButton
{
	INVALID        = -1,
	A              = 0,
	B              = 1,
	X              = 2,
	Y              = 3,
	BACK           = 4,
	GUIDE          = 5,
	START          = 6,
	LEFT_STICK     = 7,
	RIGHT_STICK    = 8,
	LEFT_SHOULDER  = 9,
	RIGHT_SHOULDER = 10,
	DPAD_UP        = 11,
	DPAD_DOWN      = 12,
	DPAD_LEFT      = 13,
	DPAD_RIGHT     = 14,
	MISC1          = 15,
	PADDLE1        = 16,
	PADDLE2        = 17,
	PADDLE3        = 18,
	PADDLE4        = 19,
	TOUCHPAD       = 20,
	SDL_MAX        = 21,
	MAX            = 128, // Android supports up to 36 buttons. DirectInput supports up to 128 buttons.
}
