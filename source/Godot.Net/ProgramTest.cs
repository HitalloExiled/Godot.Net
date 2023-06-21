namespace Godot.Net;

using Godot.Net.Drivers.GLES3.Api;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Api.Extensions.ARB;
using Godot.Net.Platforms.Windows.Native;

public class ProgramTest
{
    private static bool closed;
    private static HWND window;
    private static OpenGL gl = null!;

    public unsafe static void Run()
    {
        window = CreateWindow();

        if (window == default)
        {
            var errorCode = Kernel32.GetLastError();

            var result = Kernel32.FormatMessageW(
                Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_FROM_SYSTEM
                | Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_ALLOCATE_BUFFER
                | Kernel32.FORMAT_MESSAGE_FLAGS.FORMAT_MESSAGE_IGNORE_INSERTS,
                default,
                errorCode,
                0,
                out var message,
                0,
                default
            );

            if (result != 0)
            {
                throw new Exception($"Error {errorCode.Value}: {message}");
            }
            else
            {
                Console.WriteLine("FormatMessageW failed with error code: " + result);
            }
        }

        SetOpenglPixelFormat(window);

        var hglrc = StartOpenglRenderingContext(window);

        gl = new OpenGL(new WindowsLoader());

        if (gl.TryGetExtension<ArbDebugOutput>(out var extension))
        {
            Console.WriteLine(extension);
        }

        var program    = gl.CreateProgram();
        var vertex     = gl.CreateShader(ShaderType.VertexShader);
        var vertexCode =
            """
            #version 330

            void main()
            {
                gl_Position = vec4(0.0, 0.0, 0.0, 1.0); // example position
            }
            """;


        gl.ShaderSource(vertex, vertexCode);
        gl.CompileShader(vertex);

        gl.GetShaderiv(vertex, ShaderParameterName.CompileStatus, out var vertexCompileStatus);
        if (vertexCompileStatus == 0)
        {
            gl.GetShaderiv(vertex, ShaderParameterName.InfoLogLength, out var iloglen);

            gl.GetShaderInfoLog(vertex, iloglen, out var error);

            Console.WriteLine(error);
        }

        var fragment     = gl.CreateShader(ShaderType.FragmentShader);
        var fragmentCode =
            """
            #version 330

            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
            """;

        gl.ShaderSource(fragment, fragmentCode);
        gl.CompileShader(fragment);

        gl.GetShaderiv(fragment, ShaderParameterName.CompileStatus, out var fragmentCompileStatus);
        if (fragmentCompileStatus == 0)
        {
            gl.GetShaderiv(fragment, ShaderParameterName.InfoLogLength, out var iloglen);

            gl.GetShaderInfoLog(fragment, iloglen, out var error);

            Console.WriteLine(error);
        }

        gl.AttachShader(program, vertex);
        gl.AttachShader(program, fragment);

        gl.LinkProgram(program);

        gl.GetProgramiv(program, ProgramPropertyARB.LinkStatus, out var linkStatus);

        if (linkStatus == 0)
        {
            gl.GetProgramiv(program, ProgramPropertyARB.InfoLogLength, out var iloglen);

            gl.GetProgramInfoLog(program, iloglen, out var error);

            Console.WriteLine(error);
        }

        RunMainLoop(window);

        Cleanup(hglrc);
    }

    private static unsafe void RunMainLoop(HWND window)
    {
        var startTime = DateTime.UtcNow;

        var msg = default(User32.MSG);
        var dc = User32.GetDC(window);
        while (!closed)
        {
            while (User32.PeekMessageW(ref msg, window, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !closed)
            {
                User32.DispatchMessageW(msg);
            }

            var dTime = (float)(DateTime.UtcNow - startTime).TotalSeconds;
            var r = normalizeSinCos(MathF.Sin(dTime));
            var g = normalizeSinCos(MathF.Cos(dTime));
            var b = normalizeSinCos(MathF.Sin(-dTime));

            gl.ClearColor(r, g, b, 0);
            gl.Clear(ClearBufferMask.ColorBufferBit);

            GDI32.SwapBuffers(dc);

            static float normalizeSinCos(float sinCos) => 0.5f + sinCos / 2.0f;
        }

        _ = User32.ReleaseDC(window, dc);
    }

    private static unsafe LRESULT WinProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == User32.WINDOW_MESSAGE.WM_PAINT)
        {
            _ = User32.BeginPaint(hwnd, out var ps);
            User32.EndPaint(hwnd, ps);
            return 0;
        }

        if (msg == User32.WINDOW_MESSAGE.WM_CLOSE)
        {
            closed = true;
            return 0;
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    private static unsafe HWND CreateWindow()
    {
        using var className = new LPCWSTR("Engine");

        var windowClass = new User32.WNDCLASSEXW
        {
            cbSize        = (uint)sizeof(User32.WNDCLASSEXW),
            hbrBackground = default,
            hCursor       = default,
            hIcon         = default,
            hIconSm       = default,
            hInstance     = default,
            lpszClassName = className,
            lpszMenuName  = null,
            style         = 0,
            lpfnWndProc   = new(WinProc)
        };

        var classId = User32.RegisterClassExW(windowClass);

        var windowName = "windowName";
        var width = 500;
        var height = 500;
        var x = 0;
        var y = 0;

        var styles = User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW | User32.WINDOW_STYLES.WS_VISIBLE;
        var exStyles = default(User32.WINDOW_STYLES_EX);

        return User32.CreateWindowExW(
            exStyles,
            className,
            windowName,
            styles,
            x,
            y,
            width,
            height,
            default,
            default,
            default,
            0
        );
    }

    private static unsafe void SetOpenglPixelFormat(HWND hwnd)
    {
        // Contains desired pixel format characteristics
        GDI32.PIXELFORMATDESCRIPTOR pfd = new()
        {
            // the size of the struct
            nSize = (ushort)sizeof(GDI32.PIXELFORMATDESCRIPTOR),

            // hardcoded version of the struct
            nVersion = 1,

            // we will draw to the window, we will draw via opengl, and we will use two buffers to swap between them each frame
            dwFlags = GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_DRAW_TO_WINDOW | GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_SUPPORT_OPENGL | GDI32.PIXEL_FORMAT_DESCRIPTOR_FLAGS.PFD_DOUBLEBUFFER,

            // We expect to use RGBA pixels
            iPixelType = GDI32.PIXEL_TYPE.PFD_TYPE_RGBA,

            // pixels with 3 * 8 = 24 bits for color
            cColorBits = 24,

            // Depth of z-buffer (we don't actually care about that for now)
            cDepthBits = 32
        };

        var hdc = User32.GetDC(hwnd);
        int iPixelFormat;

        // get the device context's best, available pixel format match
        iPixelFormat = GDI32.ChoosePixelFormat(hdc, ref pfd);

        // make that match the device context's current pixel format
        GDI32.SetPixelFormat(hdc, iPixelFormat, ref pfd);

        _ = User32.ReleaseDC(hwnd, hdc);
    }

    private static HGLRC StartOpenglRenderingContext(HWND hwnd)
    {
        var dc = User32.GetDC(hwnd);
        var gctx = OpenGL32.WglCreateContext(dc);
        if (!OpenGL32.WglMakeCurrent(dc, gctx))
        {
            throw new Exception("Whyyyyyyyyyyy!!!");
        }

        _ = User32.ReleaseDC(hwnd, dc);

        return gctx;
    }

    private static void Cleanup(HGLRC gctx) => OpenGL32.WglDeleteContext(gctx);
}
