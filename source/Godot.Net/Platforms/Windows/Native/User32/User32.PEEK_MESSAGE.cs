namespace Godot.Net.Platforms.Windows.Native;
internal static partial class User32
{
    public enum PEEK_MESSAGE : uint
    {
        /// <summary>
        /// Messages are not removed from the queue after processing by PeekMessage.
        /// </summary>
        PM_NOREMOVE = 0x0000,
        /// <summary>
        /// Messages are removed from the queue after processing by PeekMessage.
        /// </summary>
        PM_REMOVE = 0x0001,
        /// <summary>
        /// Prevents the system from releasing any thread that is waiting for the caller to go idle (see WaitForInputIdle).
        /// </summary>
        PM_NOYIELD = 0x0002,

        // <summary>
        /// Process mouse and keyboard messages.
        /// </summary>
        PM_QS_INPUT = QUEUE_STATUS.QS_INPUT << 16,
        /// <summary>
        /// Process paint messages.
        /// </summary>
        PM_QS_PAINT = QUEUE_STATUS.QS_PAINT << 16,
        /// <summary>
        /// Process all posted messages, including timers and hotkeys.
        /// </summary>
        PM_QS_POSTMESSAGE = (QUEUE_STATUS.QS_POSTMESSAGE | QUEUE_STATUS.QS_HOTKEY | QUEUE_STATUS.QS_TIMER) << 16,
        /// <summary>
        /// Process all sent messages.
        /// </summary>
        PM_QS_SENDMESSAGE = QUEUE_STATUS.QS_SENDMESSAGE << 16
    }
}
