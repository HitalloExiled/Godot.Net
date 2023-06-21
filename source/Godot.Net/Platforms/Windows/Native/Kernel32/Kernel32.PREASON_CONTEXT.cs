namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class Kernel32
{
    public struct REASON_CONTEXT
    {
        public POWER_REQUEST_CONTEXT_FLAGS Version;
        public POWER_REQUEST_CONTEXT_FLAGS Flags;
        public REASON_UNION             Reason;

        [StructLayout(LayoutKind.Explicit)]
        public struct REASON_UNION
        {
            [FieldOffset(0)]
            public DETAILED_STRUCT Detailed;

            [FieldOffset(0)]
            public LPWSTR SimpleReasonString;

            public struct DETAILED_STRUCT
            {
                public HMODULE LocalizedReasonModule;
                public ULONG   LocalizedReasonId;
                public ULONG   ReasonStringCount;
                public LPWSTR  ReasonStrings;
            }
        }
    }

    public readonly record struct PREASON_CONTEXT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator REASON_CONTEXT*(PREASON_CONTEXT value) => (REASON_CONTEXT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator PREASON_CONTEXT(REASON_CONTEXT* value) => new(new(value));
    }
}
