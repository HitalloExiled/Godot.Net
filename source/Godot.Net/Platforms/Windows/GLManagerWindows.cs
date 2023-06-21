namespace Godot.Net.Platforms.Windows;

using System.Runtime.InteropServices;
using Godot.Net.Core.Error;
using Godot.Net.Core.OS;
using Godot.Net.Platforms.Windows.Native;
using static Godot.Net.Platforms.Windows.Native.Macros;

public partial class GLManagerWindows
{
    #region public_delegates
    public unsafe delegate HGLRC PFNWGLCREATECONTEXTATTRIBSARBPROC(HDC hdc, HGLRC hglrc, int* value);
    public delegate bool PFNWGLSWAPINTERVALEXTPROC(int interval);
    #endregion public_delegates

    #region macros
    private const int WGL_CONTEXT_CORE_PROFILE_BIT_ARB       = 0x00000001;
    private const int WGL_CONTEXT_FLAGS_ARB                  = 0x2094;
    private const int WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x00000002;
    private const int WGL_CONTEXT_MAJOR_VERSION_ARB          = 0x2091;
    private const int WGL_CONTEXT_MINOR_VERSION_ARB          = 0x2092;
    private const int WGL_CONTEXT_PROFILE_MASK_ARB           = 0x9126;
    #endregion macros

    #region private_static_fields
    private static GDI32.PIXELFORMATDESCRIPTOR pfd;
    #endregion private_static_fields

    #region private_readonly_fields
    private readonly List<GLDisplay>           displays = new();
    private readonly ContextType               openglApiType;
    private readonly Dictionary<int, GLWindow> windows = new();
    #endregion private_readonly_fields

    #region private_fields
    private GLWindow?                  currentWindow;
    private PFNWGLSWAPINTERVALEXTPROC? wglSwapIntervalEXT;
    #endregion private_fields

    #region private_properties
    private GLDisplay CurrentDisplay => this.displays[this.currentWindow!.GldisplayId];
    #endregion private_properties

    public GLManagerWindows(ContextType contextType) => this.openglApiType = contextType;

    #region private_methods
    private static Error ConfigurePixelFormat(HDC hDC)
    {
        pfd = new()
        {
            nSize = (ushort)Marshal.SizeOf<GDI32.PIXELFORMATDESCRIPTOR>(), // Size Of This Pixel Format Descriptor
            nVersion = 1,
            dwFlags = GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_DRAW_TO_WINDOW | // Format Must Support Window
                    GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_SUPPORT_OPENGL | // Format Must Support OpenGL
                    GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_DOUBLEBUFFER,
            iPixelType      = GDI32.PIXEL_TYPE.PFD_TYPE_RGBA,
            cColorBits      = (byte)(OS.Singleton.IsLayeredAllowed ? 32 : 24),
            cRedBits        = 0,
            cRedShift       = 0,
            cGreenBits      = 0,
            cGreenShift     = 0,
            cBlueBits       = 0,
            cBlueShift      = 0, // Color Bits Ignored
            cAlphaBits      = (byte)(OS.Singleton.IsLayeredAllowed ? 8 : 0), // Alpha Buffer
            cAlphaShift     = 0, // Shift Bit Ignored
            cAccumBits      = 0, // No Accumulation Buffer
            cAccumRedBits   = 0,
            cAccumGreenBits = 0,
            cAccumBlueBits  = 0,
            cAccumAlphaBits = 0, // Accumulation Bits Ignored
            cDepthBits      = 24, // 24Bit Z-Buffer (Depth Buffer)
            cStencilBits    = 0, // No Stencil Buffer
            cAuxBuffers     = 0, // No Auxiliary Buffer
            iLayerType      = GDI32.PFD_MAIN_PLANE, // Main Drawing Layer
            bReserved       = 0, // Reserved
            dwLayerMask     = 0,
            dwVisibleMask   = 0,
            dwDamageMask    = 0 // Layer Masks Ignored
        };

        var pixelFormat = GDI32.ChoosePixelFormat(hDC, ref pfd);
        if (pixelFormat == 0) // Did Windows Find A Matching Pixel Format?
        {
            return Error.ERR_CANT_CREATE; // Return FALSE
        }

        var ret = GDI32.SetPixelFormat(hDC, pixelFormat, ref pfd);
        if (!ret) // Are We Able To Set The Pixel Format?
        {
            return Error.ERR_CANT_CREATE; // Return FALSE
        }

        return Error.OK;
    }

    private Error CreateContext(GLWindow win, ref GLDisplay glDisplay)
    {
        var err = ConfigurePixelFormat(win.HDC);

        if (err != Error.OK)
        {
            return err;
        }

        glDisplay.HRc = OpenGL32.WglCreateContext(win.HDC);

        if (glDisplay.HRc == default) // Are We Able To Get A Rendering Context?
        {
            return Error.ERR_CANT_CREATE; // Return FALSE
        }

        if (!OpenGL32.WglMakeCurrent(win.HDC, glDisplay.HRc))
        {
            ERR_PRINT("Could not attach OpenGL context to newly created window: " + FormatErrorMessage(Kernel32.GetLastError()));
        }

        var attribs = new int[]
        {
            WGL_CONTEXT_MAJOR_VERSION_ARB, 3, //we want a 3.3 context
            WGL_CONTEXT_MINOR_VERSION_ARB, 3,
            //and it shall be forward compatible so that we can only use up to date functionality
            WGL_CONTEXT_PROFILE_MASK_ARB, WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
            WGL_CONTEXT_FLAGS_ARB, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB /*| _WGL_CONTEXT_DEBUG_BIT_ARB*/,
            0
        }; //zero indicates the end of the array

        var wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<PFNWGLCREATECONTEXTATTRIBSARBPROC>(OpenGL32.WglGetProcAddress("wglCreateContextAttribsARB"));

        if (wglCreateContextAttribsARB == default) //OpenGL 3.0 is not supported
        {
            glDisplay.HRc = 0;
            return Error.ERR_CANT_CREATE;
        }

        unsafe
        {
            fixed (int* pAttribs = attribs)
            {

                var newHRc = wglCreateContextAttribsARB(win.HDC, 0, pAttribs);

                if (newHRc == default)
                {
                    OpenGL32.WglDeleteContext(glDisplay.HRc);

                    glDisplay.HRc = 0;

                    return Error.ERR_CANT_CREATE;
                }

                if (!OpenGL32.WglMakeCurrent(win.HDC, default))
                {
                    ERR_PRINT("Could not detach OpenGL context from newly created window: " + FormatErrorMessage(Kernel32.GetLastError()));
                }

                OpenGL32.WglDeleteContext(glDisplay.HRc);

                glDisplay.HRc = newHRc;

                if (!OpenGL32.WglMakeCurrent(win.HDC, glDisplay.HRc)) // Try To Activate The Rendering Context
                {
                    ERR_PRINT("Could not attach OpenGL context to newly created window with replaced OpenGL context: " + FormatErrorMessage(Kernel32.GetLastError()));
                    OpenGL32.WglDeleteContext(glDisplay.HRc);
                    glDisplay.HRc = 0;
                    return Error.ERR_CANT_CREATE;
                }

                this.wglSwapIntervalEXT ??= Marshal.GetDelegateForFunctionPointer<PFNWGLSWAPINTERVALEXTPROC>(OpenGL32.WglGetProcAddress("wglSwapIntervalEXT"));

                return Error.OK;
            }
        }
    }

    private int FindOrCreateDisplay(GLWindow win)
    {
        // find display NYI, only 1 supported so far
        if (this.displays.Count != 0)
        {
            return 0;
        }

        // create
        var display = new GLDisplay();

        this.displays.Add(display);

        var err = this.CreateContext(win, ref display);

        if (err != Error.OK)
        {
            // not good
            // delete the _display?
            this.displays.Remove(display);

            return -1;
        }

        return this.displays.Count - 1;
    }

    private static string FormatErrorMessage(DWORD id)
    {
        Kernel32.FormatMessageW(
            Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_ALLOCATE_BUFFER
            | Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_FROM_SYSTEM
            | Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_IGNORE_INSERTS,
            null,
            id,
            (int)MAKELANGID(Kernel32.LANG_NEUTRAL, Kernel32.SUBLANG_DEFAULT),
            out var messageBuffer,
            0,
            default
        );

        var msg = "Error " + id + ": " + messageBuffer;

        return msg;
    }

    private void MakeCurrent()
    {
        if (this.currentWindow != null)
        {
            return;
        }
        var disp = this.CurrentDisplay;

        if (!OpenGL32.WglMakeCurrent(this.currentWindow!.HDC, disp.HRc))
        {
            ERR_PRINT("Could not switch OpenGL context to window marked current: " + FormatErrorMessage(Kernel32.GetLastError()));
        }
    }
    #endregion private_methods

    #region public_methods
    #pragma warning disable CA1822
    public Error Initialize() => Error.OK;
    #pragma warning restore CA1822

    public void SetUseVsync(int windowId, bool use)
    {
        var win = this.windows[windowId];

        var current = this.currentWindow;

        if (win != this.currentWindow)
        {
            this.WindowMakeCurrent(windowId);
        }

        if (this.wglSwapIntervalEXT != null)
        {
            win.UseVsync = use;
            this.wglSwapIntervalEXT(use ? 1 : 0);
        }

        if (current != this.currentWindow)
        {
            this.currentWindow = current;
            this.MakeCurrent();
        }
    }

    public void SwapBuffers() => GDI32.SwapBuffers(this.currentWindow!.HDC);

    public Error WindowCreate(int windowId, HWND hwnd, HINSTANCE _, int width, int height)
    {
        var hDC = User32.GetDC(hwnd);
        if (hDC == default)
        {
            return Error.ERR_CANT_CREATE;
        }

        // configure the HDC to use a compatible pixel format
        var result = ConfigurePixelFormat(hDC);
        if (result != Error.OK)
        {
            return result;
        }

        var win = new GLWindow
        {
            Width  = width,
            Height = height,
            Hwnd   = hwnd,
            HDC    = hDC
        };

        win.GldisplayId = this.FindOrCreateDisplay(win);

        if (win.GldisplayId == -1)
        {
            return Error.FAILED;
        }

        // WARNING: p_window_id is an eternally growing integer since popup windows keep coming and going
        // and each of them has a higher id than the previous, so it must be used in a map not a vector
        this.windows.Add(windowId, win);

        // make current
        this.WindowMakeCurrent(windowId);

        return Error.OK;
    }

    public void WindowMakeCurrent(int window)
    {
        if (window == -1)
        {
            return;
        }

        // crash if our data structures are out of sync, i.e. not found
        var win = this.windows[window];

        // noop
        if (win == this.currentWindow)
        {
            return;
        }

        var disp = this.displays[win.GldisplayId];

        if (!OpenGL32.WglMakeCurrent(win.HDC, disp.HRc))
        {
            ERR_PRINT("Could not switch OpenGL context to other window: " + FormatErrorMessage(Kernel32.GetLastError()));
        }

        this.currentWindow = win;
    }
    public void WindowResize(int window, int width, int height)
    {
        this.windows[window].Width  = width;
        this.windows[window].Height = height;
    }
    #endregion public_methods
}
