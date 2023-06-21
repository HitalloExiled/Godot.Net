namespace Godot.Net.Platforms.Windows.Native;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Value}")]
public readonly record struct ATOM(int Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator int(ATOM value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ATOM(int value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct BOOL(int Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator bool(BOOL value) => value.Value == 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator BOOL(bool value) => new(value ? 1 : 0);
}

[DebuggerDisplay("{Value}")]
public readonly record struct BYTE(byte Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte(BYTE value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator BYTE(byte value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct DWORD(uint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(DWORD value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator DWORD(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator DWORD(int value) => new((uint)value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct FARPROC(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(FARPROC value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator FARPROC(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HANDLE(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HANDLE value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HANDLE(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HBRUSH(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HBRUSH value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HBRUSH(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HCURSOR(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HCURSOR value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HCURSOR(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HDC(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HDC value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HDC(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HICON(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HICON value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HICON(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HINSTANCE(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HINSTANCE value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HINSTANCE(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HGDIOBJ(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HGDIOBJ value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HGDIOBJ(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HGLRC(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HGLRC value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HGLRC(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HHOOK(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HHOOK value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HHOOK(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HMENU(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HMENU value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HMENU(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HMODULE(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HMODULE value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HMODULE(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HMONITOR(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HMONITOR value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HMONITOR(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HRESULT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HRESULT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HRESULT(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HRGN(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HRGN value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HRGN(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct HWND(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(HWND value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator HWND(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct INT(int Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator int(INT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator INT(int value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct INT_PTR(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator INT*(INT_PTR value) => (INT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator INT_PTR(INT* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator int(INT_PTR value) => (int)value.Value;
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPBYTE(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator BYTE*(LPBYTE value) => (BYTE*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPBYTE(BYTE* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte(LPBYTE value) => (byte)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPBYTE(byte value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LONG(long Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator long(LONG value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LONG(long value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LONG_PTR(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LONG*(LONG_PTR value) => (LONG*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LONG_PTR(LONG* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator long(LONG_PTR value) => value.Value;
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPARAM(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator void*(LPARAM value) => (void*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPARAM(void* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(LPARAM value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPARAM(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPCRECT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator RECT*(LPCRECT value) => (RECT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCRECT(RECT* value) => new(new(value));
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPCSTR(nint Value = default) : IDisposable
{
    public LPCSTR(string? value) : this(Marshal.StringToHGlobalAnsi(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(LPCSTR value) => (char*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPCSTR value) => Marshal.PtrToStringAnsi(value.Value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPCVOID(nint Value = default)
{

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator void*(LPCVOID value) => (void*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCVOID(void* value) => new(new(value));
}

public record struct LPCWCH(nint Value = default) : IDisposable
{
    public LPCWCH(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public readonly void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator WCHAR*(LPCWCH value) => (WCHAR*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCWCH(WCHAR* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPCWCH value) => Marshal.PtrToStringUni(value.Value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPDWORD(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator DWORD*(LPDWORD value) => (DWORD*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPDWORD(DWORD* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPDWORD(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPCWSTR(nint Value = default) : IDisposable
{
    public LPCWSTR(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(LPCWSTR value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPCWSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPCWSTR value) => Marshal.PtrToStringUni(value.Value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPPOINT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator POINT*(LPPOINT value) => (POINT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPPOINT(POINT* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPPOINT(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPRECT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator RECT*(LPRECT value) => (RECT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPRECT(RECT* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPRECT(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPVOID(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(LPVOID value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPVOID(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPVOID(nuint value) => new(new(value.ToPointer()));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator void*(LPVOID value) => (void*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPVOID(void* value) => new(new(value));
}

[DebuggerDisplay("{Value}")]
public readonly record struct LPWSTR(nint Value = default) : IDisposable
{
    public LPWSTR(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(LPWSTR value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator LPWSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(LPWSTR value) => Marshal.PtrToStringUni(value.Value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct LRESULT(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(LRESULT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LRESULT(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct PROC(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PROC value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator PROC(nint value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct PUINT(nuint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator UINT*(PUINT value) => (UINT*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator PUINT(UINT* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator uint(PUINT value) => (uint)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator PUINT(uint value) => new(new(value));
}

[DebuggerDisplay("{Value}")]
public readonly record struct PWSTR(nint Value = default) : IDisposable
{
    public PWSTR(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public PWSTR(int size) : this(Marshal.AllocHGlobal(size))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(PWSTR value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator PWSTR(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(PWSTR value) => Marshal.PtrToStringUni(value.Value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct SHORT(short Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator short(SHORT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator SHORT(short value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct UINT(uint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(UINT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator UINT(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator UINT(int value) => new((uint)value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct UINT_PTR(nuint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator uint*(UINT_PTR value) => (uint*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator UINT_PTR(uint* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(UINT_PTR value) => (uint)value.Value;
}

[DebuggerDisplay("{Value}")]
public readonly record struct ULONG(ulong Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ulong(ULONG value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ULONG(ulong value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ULONG(long value) => new((ulong)value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct ULONG_PTR(nuint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator ULONG*(ULONG_PTR value) => (ULONG*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator ULONG_PTR(ULONG* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ulong(ULONG_PTR value) => value.Value;
}

[DebuggerDisplay("{Value}")]
public readonly record struct USHORT(ushort Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ushort(USHORT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator USHORT(ushort value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator USHORT(short value) => new((ushort)value);
}

public record struct WCHAR(char Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator char(WCHAR value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator WCHAR(char value) => new(value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct WORD(short Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator short(WORD value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator WORD(short value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator WORD(int value) => new((short)value);
}

[DebuggerDisplay("{Value}")]
public readonly record struct WPARAM(nuint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator void*(WPARAM value) => (void*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator WPARAM(void* value) => new(new(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nuint(WPARAM value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator WPARAM(nuint value) => new(value);
}
