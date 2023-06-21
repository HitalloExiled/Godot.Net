namespace Godot.Net.Drivers.GLES3.Api;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Godot.Net.Core;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Api.Interfaces;

public class OpenGL : IDisposable
{
    private delegate void GLActiveTexture(TextureUnit texture);
    private delegate void GLAttachShader(uint program, uint shader);
    private delegate void GLBindBuffer(BufferTargetARB target, uint buffer);
    private delegate void GLBindBufferBase(BufferTargetARB target, uint index, uint buffer);
    private delegate void GLBindFramebuffer(FramebufferTarget target, uint framebuffer);
    private delegate void GLBindTexture(TextureTarget target, uint texture);
    private unsafe delegate void GLBindTextures(uint first, int count, /*const*/ uint* textures);
    private delegate void GLBindVertexArray(uint array);
    private delegate void GLBlendColor(float red, float green, float blue, float alpha);
    private delegate void GLBlendEquation(BlendEquationModeEXT mode);
    private delegate void GLBlendFuncSeparate(BlendingFactor sfactorRGB, BlendingFactor dfactorRGB, BlendingFactor sfactorAlpha, BlendingFactor dfactorAlpha);
    private delegate void GLBufferData(BufferTargetARB target, nint size, /*const*/ nint data, BufferUsageARB usage);
    private delegate void GLBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter);
    private delegate void GLBufferSubData(BufferTargetARB target, nint offset, nint size, nint data);
    private delegate FramebufferStatus GLCheckFramebufferStatus(FramebufferTarget target);
    private delegate void GLClear(ClearBufferMask mask);
    private unsafe delegate void GLClearBufferfv(Buffer buffer, int drawbuffer, /*const*/ float* value);
    private delegate void GLClearColor(float red, float green, float blue, float alpha);
    private delegate SyncStatus GLClientWaitSync(nint sync, SyncObjectMask flags, ulong timeout);
    private delegate void GLColorMask(bool red, bool green, bool blue, bool alpha);
    private delegate void GLCompileShader(uint shader);
    private delegate void GLCompressedTexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, int imageSize, nint data);
    private delegate uint GLCreateProgram();
    private delegate uint GLCreateShader(ShaderType type);
    private unsafe delegate void GLDeleteBuffers(int n, /*const*/ uint* buffers);
    private unsafe delegate void GLDeleteFramebuffers(int n, /*const*/ uint* framebuffers);
    private delegate void GLDeleteProgram(uint program);
    private delegate void GLDeleteShader(uint shader);
    private delegate void GLDeleteSync(nint sync);
    private unsafe delegate void GLDeleteTextures(int n, /*const*/ uint* textures);
    private delegate void GLDepthMask(bool flag);
    private delegate void GLDisable(EnableCap cap);
    private delegate void GLDisableVertexAttribArray(uint index);
    private delegate void GLDrawArraysInstanced(PrimitiveType mode, int first, int count, int instancecount);
    private delegate void GLDrawElementsInstanced(PrimitiveType mode, int count, DrawElementsType type, nint indices, int instancecount);
    private delegate void GLEnable(EnableCap cap);
    private delegate void GLEnableVertexAttribArray(uint index);
    private delegate nint GLFenceSync(SyncCondition condition, SyncBehaviorFlags flags);
    private delegate void GLFinish();
    private delegate void GLFramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level);
    private delegate void GLFramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer);
    private unsafe delegate void GLGenBuffers(int n, uint* buffers);
    private unsafe delegate void GLGenFramebuffers(int n, uint* framebuffers);
    private unsafe delegate void GLGenQueries(int n, uint* ids);
    private unsafe delegate void GLGenTextures(int n, uint* textures);
    private unsafe delegate void GLGenVertexArrays(int n, uint* arrays);
    private unsafe delegate void GLGetFloatv(GetPName pname, float* data);
    private unsafe delegate void GLGetIntegerv(GetPName pname, int* data);
    private delegate nint GLGetString(StringName name);
    private delegate nint GLGetStringi(StringName name, uint index);
    private unsafe delegate void GLGetQueryObjectui64v(uint id, QueryObjectParameterName pname, ulong* @params);
    private unsafe delegate void GLGetShaderiv(uint shader, ShaderParameterName pname, int* @params);
    private unsafe delegate void GLGetShaderInfoLog(uint shader, int bufSize, int* length, byte* infoLog);
    private unsafe delegate void GLGetProgramiv(uint program, ProgramPropertyARB pname, int* @params);
    private unsafe delegate void GLGetProgramInfoLog(uint program, int bufSize, int* length, byte* infoLog);
    private unsafe delegate uint GLGetUniformBlockIndex(uint program, /*const*/ char* uniformBlockName);
    private unsafe delegate int GLGetUniformLocation(uint program, /*const*/ char* name);
    private unsafe delegate void GLGetSynciv(nint sync, SyncParameterName pname, int count, int* length, int* values);
    private delegate nint GLMapBufferRange(BufferTargetARB target, nint offset, nint length, MapBufferAccessMask access);
    private delegate void GLLinkProgram(uint program);
    private delegate void GLPixelStorei(PixelStoreParameter pname, int param);
    private delegate void GLQueryCounter(uint id, QueryCounterTarget target);
    private delegate void GLScissor(int x, int y, int width, int height);
    private unsafe delegate void GLShaderSource(uint shader, int count, /*const GLchar * const **/ char** @string, /*const*/ int* length);
    private delegate void GLTexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, nint pixels);
    private delegate void GLTexImage3D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, nint pixels);
    private delegate void GLTexParameteri(TextureTarget target, TextureParameterName pname, int param);
    private delegate void GLTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, nint pixels);
    private unsafe delegate void GLTransformFeedbackVaryings(uint program, int count, /*const GLchar * const **/ char* varyings, TransformFeedbackBufferMode bufferMode);
    private delegate bool GLUnmapBuffer(BufferTargetARB target);
    private delegate void GLUniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding);
    private delegate void GLUseProgram(uint program);
    private delegate void GLVertexAttrib4f(uint index, float x, float y, float z, float w);
    private delegate void GLVertexAttribDivisor(uint index, uint divisor);
    private delegate void GLVertexAttribIPointer(uint index, int size, VertexAttribIType type, int stride, nint pointer);
    private delegate void GLVertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, nint pointer);
    private delegate void GLViewport(int x, int y, int width, int height);

    private readonly ILoader loader;

    private readonly GLActiveTexture             glActiveTexture;
    private readonly GLAttachShader              glAttachShader;
    private readonly GLBindBuffer                glBindBuffer;
    private readonly GLBindBufferBase            glBindBufferBase;
    private readonly GLBindFramebuffer           glBindFramebuffer;
    private readonly GLBindTexture               glBindTexture;
    private readonly GLBindTextures              glBindTextures;
    private readonly GLBindVertexArray           glBindVertexArray;
    private readonly GLBlendColor                glBlendColor;
    private readonly GLBlendEquation             glBlendEquation;
    private readonly GLBlendFuncSeparate         glBlendFuncSeparate;
    private readonly GLBufferData                glBufferData;
    private readonly GLBlitFramebuffer           glBlitFramebuffer;
    private readonly GLBufferSubData             glBufferSubData;
    private readonly GLCheckFramebufferStatus    glCheckFramebufferStatus;
    private readonly GLClear                     glClear;
    private readonly GLClearBufferfv             glClearBufferfv;
    private readonly GLClearColor                glClearColor;
    private readonly GLClientWaitSync            glClientWaitSync;
    private readonly GLColorMask                 glColorMask;
    private readonly GLCompileShader             glCompileShader;
    private readonly GLCompressedTexImage2D      glCompressedTexImage2D;
    private readonly GLCreateProgram             glCreateProgram;
    private readonly GLCreateShader              glCreateShader;
    private readonly GLDeleteBuffers             glDeleteBuffers;
    private readonly GLDeleteFramebuffers        glDeleteFramebuffers;
    private readonly GLDeleteProgram             glDeleteProgram;
    private readonly GLDeleteShader              glDeleteShader;
    private readonly GLDeleteSync                glDeleteSync;
    private readonly GLDeleteTextures            glDeleteTextures;
    private readonly GLDepthMask                 glDepthMask;
    private readonly GLDisable                   glDisable;
    private readonly GLDisableVertexAttribArray  glDisableVertexAttribArray;
    private readonly GLDrawArraysInstanced       glDrawArraysInstanced;
    private readonly GLDrawElementsInstanced     glDrawElementsInstanced;
    private readonly GLEnable                    glEnable;
    private readonly GLEnableVertexAttribArray   glEnableVertexAttribArray;
    private readonly GLFenceSync                 glFenceSync;
    private readonly GLFinish                    glFinish;
    private readonly GLFramebufferTexture2D      glFramebufferTexture2D;
    private readonly GLFramebufferTextureLayer   glFramebufferTextureLayer;
    private readonly GLGenBuffers                glGenBuffers;
    private readonly GLGenFramebuffers           glGenFramebuffers;
    private readonly GLGenQueries                glGenQueries;
    private readonly GLGenTextures               glGenTextures;
    private readonly GLGenVertexArrays           glGenVertexArrays;
    private readonly GLGetFloatv                 glGetFloatv;
    private readonly GLGetIntegerv               glGetIntegerv;
    private readonly GLGetString                 glGetString;
    private readonly GLGetStringi                glGetStringi;
    private readonly GLGetQueryObjectui64v       glGetQueryObjectui64v;
    private readonly GLGetShaderiv               glGetShaderiv;
    private readonly GLGetShaderInfoLog          glGetShaderInfoLog;
    private readonly GLGetProgramiv              glGetProgramiv;
    private readonly GLGetProgramInfoLog         glGetProgramInfoLog;
    private readonly GLGetUniformBlockIndex      glGetUniformBlockIndex;
    private readonly GLGetUniformLocation        glGetUniformLocation;
    private readonly GLGetSynciv                 glGetSynciv;
    private readonly GLMapBufferRange            glMapBufferRange;
    private readonly GLLinkProgram               glLinkProgram;
    private readonly GLPixelStorei               glPixelStorei;
    private readonly GLQueryCounter              glQueryCounter;
    private readonly GLScissor                   glScissor;
    private readonly GLShaderSource              glShaderSource;
    private readonly GLTexImage2D                glTexImage2D;
    private readonly GLTexImage3D                glTexImage3D;
    private readonly GLTexParameteri             glTexParameteri;
    private readonly GLTexSubImage3D             glTexSubImage3D;
    private readonly GLTransformFeedbackVaryings glTransformFeedbackVaryings;
    private readonly GLUnmapBuffer               glUnmapBuffer;
    private readonly GLUniformBlockBinding       glUniformBlockBinding;
    private readonly GLUseProgram                glUseProgram;
    private readonly GLVertexAttrib4f            glVertexAttrib4f;
    private readonly GLVertexAttribDivisor       glVertexAttribDivisor;
    private readonly GLVertexAttribIPointer      glVertexAttribIPointer;
    private readonly GLVertexAttribPointer       glVertexAttribPointer;
    private readonly GLViewport                  glViewport;

    private bool disposed;

    public OpenGL(ILoader loader)
    {
        this.loader = loader;

        this.glActiveTexture             = this.loader.Load<GLActiveTexture>("glActiveTexture");
        this.glAttachShader              = this.loader.Load<GLAttachShader>("glAttachShader");
        this.glBindBuffer                = this.loader.Load<GLBindBuffer>("glBindBuffer");
        this.glBindBufferBase            = this.loader.Load<GLBindBufferBase>("glBindBufferBase");
        this.glBindFramebuffer           = this.loader.Load<GLBindFramebuffer>("glBindFramebuffer");
        this.glBindTexture               = this.loader.Load<GLBindTexture>("glBindTexture");
        this.glBindTextures              = this.loader.Load<GLBindTextures>("glBindTextures");
        this.glBindVertexArray           = this.loader.Load<GLBindVertexArray>("glBindVertexArray");
        this.glBlendColor                = this.loader.Load<GLBlendColor>("glBlendColor");
        this.glBlendEquation             = this.loader.Load<GLBlendEquation>("glBlendEquation");
        this.glBlendFuncSeparate         = this.loader.Load<GLBlendFuncSeparate>("glBlendFuncSeparate");
        this.glBlitFramebuffer           = this.loader.Load<GLBlitFramebuffer>("glBlitFramebuffer");
        this.glBufferData                = this.loader.Load<GLBufferData>("glBufferData");
        this.glBufferSubData             = this.loader.Load<GLBufferSubData>("glBufferSubData");
        this.glCheckFramebufferStatus    = this.loader.Load<GLCheckFramebufferStatus>("glCheckFramebufferStatus");
        this.glClear                     = this.loader.Load<GLClear>("glClear");
        this.glClearBufferfv             = this.loader.Load<GLClearBufferfv>("glClearBufferfv");
        this.glClearColor                = this.loader.Load<GLClearColor>("glClearColor");
        this.glClientWaitSync            = this.loader.Load<GLClientWaitSync>("glClientWaitSync");
        this.glColorMask                 = this.loader.Load<GLColorMask>("glColorMask");
        this.glCompileShader             = this.loader.Load<GLCompileShader>("glCompileShader");
        this.glCompressedTexImage2D      = this.loader.Load<GLCompressedTexImage2D>("glCompressedTexImage2D");
        this.glCompressedTexImage2D      = this.loader.Load<GLCompressedTexImage2D>("glCompressedTexImage2D");
        this.glCreateProgram             = this.loader.Load<GLCreateProgram>("glCreateProgram");
        this.glCreateShader              = this.loader.Load<GLCreateShader>("glCreateShader");
        this.glDeleteBuffers             = this.loader.Load<GLDeleteBuffers>("glDeleteBuffers");
        this.glDeleteFramebuffers        = this.loader.Load<GLDeleteFramebuffers>("glDeleteFramebuffers");
        this.glDeleteProgram             = this.loader.Load<GLDeleteProgram>("glDeleteProgram");
        this.glDeleteShader              = this.loader.Load<GLDeleteShader>("glDeleteShader");
        this.glDeleteSync                = this.loader.Load<GLDeleteSync>("glDeleteSync");
        this.glDeleteTextures            = this.loader.Load<GLDeleteTextures>("glDeleteTextures");
        this.glDepthMask                 = this.loader.Load<GLDepthMask>("glDepthMask");
        this.glDisable                   = this.loader.Load<GLDisable>("glDisable");
        this.glDisableVertexAttribArray  = this.loader.Load<GLDisableVertexAttribArray>("glDisableVertexAttribArray");
        this.glDrawArraysInstanced       = this.loader.Load<GLDrawArraysInstanced>("glDrawArraysInstanced");
        this.glDrawElementsInstanced     = this.loader.Load<GLDrawElementsInstanced>("glDrawElementsInstanced");
        this.glEnable                    = this.loader.Load<GLEnable>("glEnable");
        this.glEnableVertexAttribArray   = this.loader.Load<GLEnableVertexAttribArray>("glEnableVertexAttribArray");
        this.glFenceSync                 = this.loader.Load<GLFenceSync>("glFenceSync");
        this.glFinish                    = this.loader.Load<GLFinish>("glFinish");
        this.glFramebufferTexture2D      = this.loader.Load<GLFramebufferTexture2D>("glFramebufferTexture2D");
        this.glFramebufferTextureLayer   = this.loader.Load<GLFramebufferTextureLayer>("glFramebufferTextureLayer");
        this.glGenBuffers                = this.loader.Load<GLGenBuffers>("glGenBuffers");
        this.glGenFramebuffers           = this.loader.Load<GLGenFramebuffers>("glGenFramebuffers");
        this.glGenQueries                = this.loader.Load<GLGenQueries>("glGenQueries");
        this.glGenTextures               = this.loader.Load<GLGenTextures>("glGenTextures");
        this.glGenVertexArrays           = this.loader.Load<GLGenVertexArrays>("glGenVertexArrays");
        this.glGetFloatv                 = this.loader.Load<GLGetFloatv>("glGetFloatv");
        this.glGetIntegerv               = this.loader.Load<GLGetIntegerv>("glGetIntegerv");
        this.glGetProgramInfoLog         = this.loader.Load<GLGetProgramInfoLog>("glGetProgramInfoLog");
        this.glGetProgramiv              = this.loader.Load<GLGetProgramiv>("glGetProgramiv");
        this.glGetQueryObjectui64v       = this.loader.Load<GLGetQueryObjectui64v>("glGetQueryObjectui64v");
        this.glGetShaderInfoLog          = this.loader.Load<GLGetShaderInfoLog>("glGetShaderInfoLog");
        this.glGetShaderiv               = this.loader.Load<GLGetShaderiv>("glGetShaderiv");
        this.glGetString                 = this.loader.Load<GLGetString>("glGetString");
        this.glGetStringi                = this.loader.Load<GLGetStringi>("glGetStringi");
        this.glGetSynciv                 = this.loader.Load<GLGetSynciv>("glGetSynciv");
        this.glGetUniformBlockIndex      = this.loader.Load<GLGetUniformBlockIndex>("glGetUniformBlockIndex");
        this.glGetUniformLocation        = this.loader.Load<GLGetUniformLocation>("glGetUniformLocation");
        this.glLinkProgram               = this.loader.Load<GLLinkProgram>("glLinkProgram");
        this.glMapBufferRange            = this.loader.Load<GLMapBufferRange>("glMapBufferRange");
        this.glPixelStorei               = this.loader.Load<GLPixelStorei>("glPixelStorei");
        this.glQueryCounter              = this.loader.Load<GLQueryCounter>("glQueryCounter");
        this.glScissor                   = this.loader.Load<GLScissor>("glScissor");
        this.glShaderSource              = this.loader.Load<GLShaderSource>("glShaderSource");
        this.glTexImage2D                = this.loader.Load<GLTexImage2D>("glTexImage2D");
        this.glTexImage3D                = this.loader.Load<GLTexImage3D>("glTexImage3D");
        this.glTexParameteri             = this.loader.Load<GLTexParameteri>("glTexParameteri");
        this.glTexSubImage3D             = this.loader.Load<GLTexSubImage3D>("glTexSubImage3D");
        this.glTransformFeedbackVaryings = this.loader.Load<GLTransformFeedbackVaryings>("glTransformFeedbackVaryings");
        this.glUniformBlockBinding       = this.loader.Load<GLUniformBlockBinding>("glUniformBlockBinding");
        this.glUnmapBuffer               = this.loader.Load<GLUnmapBuffer>("glUnmapBuffer");
        this.glUseProgram                = this.loader.Load<GLUseProgram>("glUseProgram");
        this.glVertexAttrib4f            = this.loader.Load<GLVertexAttrib4f>("glVertexAttrib4f");
        this.glVertexAttribDivisor       = this.loader.Load<GLVertexAttribDivisor>("glVertexAttribDivisor");
        this.glVertexAttribIPointer      = this.loader.Load<GLVertexAttribIPointer>("glVertexAttribIPointer");
        this.glVertexAttribPointer       = this.loader.Load<GLVertexAttribPointer>("glVertexAttribPointer");
        this.glViewport                  = this.loader.Load<GLViewport>("glViewport");
    }

    ~OpenGL() => this.Dispose(disposing: false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.loader.Dispose();
            }

            this.disposed = true;
        }
    }

    public void ActiveTexture(TextureUnit texture) =>
        this.glActiveTexture.Invoke(texture);

    public void AttachShader(uint program, uint shader) =>
        this.glAttachShader.Invoke(program, shader);

    public void BindBuffer(BufferTargetARB target, uint buffer) =>
        this.glBindBuffer.Invoke(target, buffer);

    public void BindBufferBase(BufferTargetARB target, uint index, uint buffer) =>
        this.glBindBufferBase.Invoke(target, index, buffer);

    public void BindFramebuffer(FramebufferTarget target, uint framebuffer) =>
        this.glBindFramebuffer.Invoke(target, framebuffer);

    public void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter) =>
        this.glBlitFramebuffer.Invoke(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);

    public void BufferSubData(BufferTargetARB target, nint offset, nint size, nint data) =>
        this.glBufferSubData.Invoke(target, offset, size, data);

    public unsafe void BufferSubData<T>(BufferTargetARB target, uint offset, uint size, T data) where T : unmanaged =>
        this.glBufferSubData.Invoke(target, new(offset), new(size), new(&data));

    public void BufferSubData<T>(BufferTargetARB target, int offset, int size, T data) where T : unmanaged =>
        this.BufferSubData(target, (uint)offset, (uint)size, data);

    public unsafe void BufferSubData<T>(BufferTargetARB target, int offset, T[] data) where T : unmanaged
    {
        fixed(T* pData = data)
        {
            this.glBufferSubData.Invoke(target, offset, data.Length * Marshal.SizeOf<T>(), new(pData));
        }
    }

    public void BindTexture(TextureTarget target, uint texture) =>
        this.glBindTexture.Invoke(target, texture);

    public unsafe void BindTextures(uint first, int count, /*const*/ uint* textures) =>
        this.glBindTextures.Invoke(first, count, textures);

    public unsafe void BindTextures(uint first, uint[] textures)
    {
        fixed (uint* pointer = textures)
        {
            this.glBindTextures.Invoke(first, textures.Length, pointer);
        }
    }

    public void BindVertexArray(uint array) =>
        this.glBindVertexArray.Invoke(array);

    public void BlendColor(float red, float green, float blue, float alpha) =>
        this.glBlendColor.Invoke(red, green, blue, alpha);

    public void BlendEquation(BlendEquationModeEXT mode) =>
        this.glBlendEquation.Invoke(mode);

    public void BlendFuncSeparate(BlendingFactor sfactorRGB, BlendingFactor dfactorRGB, BlendingFactor sfactorAlpha, BlendingFactor dfactorAlpha) =>
        this.glBlendFuncSeparate.Invoke(sfactorRGB, dfactorRGB, sfactorAlpha, dfactorAlpha);

    public void BufferData(BufferTargetARB target, uint size, nint data, BufferUsageARB usage) =>
        this.glBufferData.Invoke(target, new(size), data, usage);

    public void BufferData(BufferTargetARB target, nint size, nint data, BufferUsageARB usage) =>
        this.glBufferData.Invoke(target, size, data, usage);

    public unsafe void BufferData<T>(BufferTargetARB target, T[] data, BufferUsageARB usage) where T : unmanaged
    {
        fixed (T* dataPtr = data)
        {
            this.glBufferData.Invoke(target, Marshal.SizeOf<T>() * data.Length, new(dataPtr), usage);
        }
    }

    public unsafe void BufferData<T>(BufferTargetARB target, in T data, BufferUsageARB usage) where T : unmanaged
    {
        fixed (T* pointer = &data)
        {
            this.glBufferData.Invoke(target, Marshal.SizeOf<T>(), new(pointer), usage);
        }
    }

    public FramebufferStatus CheckFramebufferStatus(FramebufferTarget target) =>
        this.glCheckFramebufferStatus.Invoke(target);

    public void Clear(ClearBufferMask mask) =>
        this.glClear(mask);

    public unsafe void ClearBufferfv(Buffer buffer, int drawbuffer, /*const*/ float* value) =>
        this.glClearBufferfv.Invoke(buffer, drawbuffer, value);

    public unsafe void ClearBufferfv(Buffer buffer, int drawbuffer, Span<float> value)
    {
        fixed (float* pointer = value)
        {
            this.glClearBufferfv.Invoke(buffer, drawbuffer, pointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe void ClearBufferfv(Buffer buffer, int drawbuffer, float[] value) =>
        this.ClearBufferfv(buffer, drawbuffer, value.AsSpan());

    public void ClearColor(float red, float green, float blue, float alpha) =>
        this.glClearColor(red, green, blue, alpha);

    public SyncStatus ClientWaitSync(nint sync, SyncObjectMask flags, ulong timeout) =>
        this.glClientWaitSync.Invoke(sync, flags, timeout);

    public void ColorMask(bool red, bool green, bool blue, bool alpha) =>
        this.glColorMask.Invoke(red, green, blue, alpha);

    public void CompileShader(uint shader) =>
        this.glCompileShader.Invoke(shader);

    public void CompressedTexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, int imageSize, nint data) =>
        this.glCompressedTexImage2D.Invoke(target, level, internalformat, width, height, border, imageSize, data);

    public unsafe void CompressedTexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, int imageSize, Span<byte> data)
    {
        fixed (byte* pointer = data)
        {
            this.glCompressedTexImage2D.Invoke(target, level, internalformat, width, height, border, imageSize, (nint)pointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe void CompressedTexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, int imageSize, byte[] data) =>
        this.CompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data.AsSpan());

    public uint CreateProgram() =>
        this.glCreateProgram.Invoke();

    public uint CreateShader(ShaderType type) =>
        this.glCreateShader.Invoke(type);

    public unsafe void DeleteBuffers(int n, /*const*/ uint* buffers) =>
        this.glDeleteBuffers.Invoke(n, buffers);

    public unsafe void DeleteBuffers(uint buffer) =>
        this.glDeleteBuffers.Invoke(1, &buffer);

    public unsafe void DeleteBuffers(uint[] buffers)
    {
        fixed (uint* pointer = buffers)
        {
            this.glDeleteBuffers.Invoke(buffers.Length, pointer);
        }
    }

    public unsafe void DeleteFramebuffers(int n, /*const*/ uint* framebuffers) =>
        this.glDeleteFramebuffers.Invoke(n, framebuffers);

    public unsafe void DeleteFramebuffers(uint framebuffers) =>
        this.glDeleteFramebuffers.Invoke(1, &framebuffers);

    public unsafe void DeleteFramebuffers(uint[] framebuffers)
    {
        fixed (uint* pointer = framebuffers)
        {
            this.glDeleteFramebuffers.Invoke(framebuffers.Length, pointer);
        }
    }

    public void DeleteProgram(uint program) =>
        this.glDeleteProgram.Invoke(program);

    public void DeleteShader(uint shader) =>
        this.glDeleteShader.Invoke(shader);

    public void DeleteSync(nint sync) =>
        this.glDeleteSync.Invoke(sync);

    public unsafe void DeleteTextures(int n, /*const*/ uint* textures) =>
        this.glDeleteTextures.Invoke(n, textures);

    public unsafe void DeleteTextures(uint texture) =>
        this.glDeleteTextures.Invoke(1, &texture);

    public unsafe void DeleteTextures(uint[] textures)
    {
        fixed (uint* pointer = textures)
        {
            this.glDeleteTextures.Invoke(textures.Length, pointer);
        }
    }

    public void DepthMask(bool flag) =>
        this.glDepthMask.Invoke(flag);

    public void Disable(EnableCap cap) =>
        this.glDisable.Invoke(cap);

    public void DisableVertexAttribArray(uint index) =>
        this.glDisableVertexAttribArray.Invoke(index);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void DrawArraysInstanced(PrimitiveType mode, int first, int count, int instancecount) =>
        this.glDrawArraysInstanced.Invoke(mode, first, count, instancecount);

    public void DrawElementsInstanced(PrimitiveType mode, int count, DrawElementsType type, nint indices, int instancecount) =>
        this.glDrawElementsInstanced.Invoke(mode, count, type, indices, instancecount);

    public void Enable(EnableCap cap) =>
        this.glEnable(cap);

    public void EnableVertexAttribArray(uint index) =>
        this.glEnableVertexAttribArray.Invoke(index);

    public nint FenceSync(SyncCondition condition, SyncBehaviorFlags flags) =>
        this.glFenceSync.Invoke(condition, flags);

    public void Finish() =>
        this.glFinish.Invoke();

    public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level) =>
        this.glFramebufferTexture2D.Invoke(target, attachment, textarget, texture, level);

    public void FramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer) =>
        this.glFramebufferTextureLayer.Invoke(target, attachment, texture, level, layer);

    public unsafe void GenBuffers(int n, uint* buffers) =>
        this.glGenBuffers.Invoke(n, buffers);

    public unsafe void GenBuffers(uint[] buffers)
    {
        fixed (uint* pointer = buffers)
        {
            this.glGenBuffers.Invoke(buffers.Length, pointer);
        }
    }

    public unsafe void GenBuffers(out uint buffer)
    {
        fixed (uint* pointer = &buffer)
        {
            this.glGenBuffers.Invoke(1, pointer);
        }
    }

    public unsafe void GenFramebuffers(int n, uint* framebuffers) =>
        this.glGenFramebuffers.Invoke(n, framebuffers);

    public unsafe void GenFramebuffers(uint[] framebuffers)
    {
        fixed (uint* pointer = framebuffers)
        {
            this.glGenFramebuffers.Invoke(framebuffers.Length, pointer);
        }
    }

    public unsafe void GenFramebuffers(out uint framebuffer)
    {
        fixed (uint* pointer = &framebuffer)
        {
            this.glGenFramebuffers.Invoke(1, pointer);
        }
    }

    public unsafe void GenQueries(int n, uint* ids) =>
        this.glGenQueries.Invoke(n, ids);

    public unsafe void GenQueries(uint[] ids)
    {
        fixed (uint* pointer = ids)
        {
            this.glGenQueries.Invoke(ids.Length, pointer);
        }
    }

    public unsafe void GenQueries(out uint id)
    {
        fixed (uint* pointer = &id)
        {
            this.glGenQueries.Invoke(1, pointer);
        }
    }

    public unsafe void GenTextures(int n, uint* textures) =>
        this.glGenTextures.Invoke(n, textures);

    public unsafe void GenTextures(uint[] textures)
    {
        fixed (uint* pointer = textures)
        {
            this.glGenTextures.Invoke(textures.Length, pointer);
        }
    }

    public unsafe void GenTextures(out uint texture)
    {
        fixed (uint* pointer = &texture)
        {
            this.glGenTextures.Invoke(1, pointer);
        }
    }

    public unsafe void GenVertexArrays(int n, uint* arrays) =>
        this.glGenVertexArrays.Invoke(n, arrays);

    public unsafe void GenVertexArrays(uint[] arrays)
    {
        fixed (uint* pointer = arrays)
        {
            this.glGenVertexArrays.Invoke(arrays.Length, pointer);
        }
    }

    public unsafe void GenVertexArrays(out uint array)
    {
        fixed (uint* pointer = &array)
        {
            this.glGenVertexArrays.Invoke(1, pointer);
        }
    }

    public HashSet<string> GetExtensions()
    {
        this.GetIntegerv(GetPName.NumExtensions, out int maxExtensions);

        var extensions = new HashSet<string>(maxExtensions);

        for (var i = 0; i < maxExtensions; i++)
        {
            var value = this.GetStringi(StringName.Extensions, (uint)i);
            if (value == null)
            {
                break;
            }

            extensions.Add(value);
        }

        return extensions.Concat(this.loader.GetExtensions()).ToHashSet();
    }

    public unsafe void GetFloatv(GetPName pname, float* data) =>
        this.glGetFloatv.Invoke(pname, data);

    public unsafe void GetFloatv(GetPName pname, out float data)
    {
        fixed (float* pointer = &data)
        {
            this.glGetFloatv.Invoke(pname, pointer);
        }
    }

    public unsafe void GetFloatv(GetPName pname, out float[] data)
    {
        fixed (float* pointer = data)
        {
            this.glGetFloatv.Invoke(pname, pointer);
        }
    }

    public unsafe void GetIntegerv(GetPName pname, int* data) =>
        this.glGetIntegerv.Invoke(pname, data);

    public unsafe void GetIntegerv(GetPName pname, out int data)
    {
        fixed (int* pointer = &data)
        {
            this.glGetIntegerv.Invoke(pname, pointer);
        }
    }

    public unsafe void GetIntegerv(GetPName pname, out int[] data)
    {
        fixed (int* pointer = data)
        {
            this.glGetIntegerv.Invoke(pname, pointer);
        }
    }

    public unsafe void GetQueryObjectui64v(uint id, QueryObjectParameterName pname, ulong* @params) =>
        this.glGetQueryObjectui64v.Invoke(id, pname, @params);

    public unsafe void GetQueryObjectui64v(uint id, QueryObjectParameterName pname, out ulong @params)
    {
        fixed (ulong* pointer = &@params)
        {
            this.glGetQueryObjectui64v.Invoke(id, pname, pointer);
        }
    }

    public unsafe void GetShaderiv(uint shader, ShaderParameterName pname, out int @params)
    {
        fixed (int* pointer = &@params)
        {
            this.glGetShaderiv.Invoke(shader, pname, pointer);
        }
    }

    public unsafe void GetShaderiv(uint shader, ShaderParameterName pname, int* @params) =>
        this.glGetShaderiv.Invoke(shader, pname, @params);

    public unsafe void GetShaderInfoLog(uint shader, int bufSize, int* length, byte* infoLog) =>
        this.glGetShaderInfoLog.Invoke(shader, bufSize, length, infoLog);

    public unsafe void GetShaderInfoLog(uint shader, int bufSize, out string info)
    {
        var buffer = stackalloc byte[bufSize + 1];

        this.glGetShaderInfoLog.Invoke(shader, bufSize, &bufSize, buffer);

        info = Encoding.UTF8.GetString(UnmanagedUtils.PointerToArray(buffer, bufSize));
    }

    public string? GetString(StringName name)
    {
        var result = Marshal.PtrToStringAnsi(this.glGetString(name));

        return result;
    }

    public string? GetStringi(StringName name, uint index)
    {
        var result = Marshal.PtrToStringAnsi(this.glGetStringi(name, index));

        return result;
    }

    public unsafe void GetProgramiv(uint program, ProgramPropertyARB pname, int* @params) =>
        this.glGetProgramiv.Invoke(program, pname, @params);

    public unsafe void GetProgramiv(uint program, ProgramPropertyARB pname, out int @params)
    {
        fixed (int* pointer = &@params)
        {
            this.glGetProgramiv.Invoke(program, pname, pointer);
        }
    }

    public unsafe void GetProgramInfoLog(uint program, int bufSize, int* length, byte* infoLog) =>
        this.glGetProgramInfoLog.Invoke(program, bufSize, length, infoLog);

    public unsafe void GetProgramInfoLog(uint program, int bufSize, out string infoLog)
    {
        var buffer = stackalloc byte[bufSize + 1];

        this.glGetProgramInfoLog.Invoke(program, bufSize, &bufSize, buffer);

        infoLog = Encoding.UTF8.GetString(UnmanagedUtils.PointerToArray(buffer, bufSize));
    }

    public unsafe uint GetUniformBlockIndex(uint program, /*const*/ char* uniformBlockName) =>
        this.glGetUniformBlockIndex.Invoke(program, uniformBlockName);

    public unsafe uint GetUniformBlockIndex(uint program, string uniformBlockName)
    {
        fixed (char* pointer = uniformBlockName)
        {
            return this.glGetUniformBlockIndex.Invoke(program, pointer);
        }
    }

    public unsafe int GetUniformLocation(uint program, /*const*/ char* name) =>
        this.glGetUniformLocation.Invoke(program, name);

    public unsafe int GetUniformLocation(uint program, string name)
    {
        using var pointer = new StringA(name);

        return this.glGetUniformLocation.Invoke(program, pointer);
    }

    public bool HasExtension(string extension) =>
        this.GetExtensions().Contains(extension);

    public unsafe void GetSynciv(nint sync, SyncParameterName pname, int count, int* length, int* values) =>
        this.glGetSynciv.Invoke(sync, pname, count, length, values);

    public unsafe void GetSynciv(nint sync, SyncParameterName pname, int count, out int length, out int values)
    {
        fixed (int* lengthPtr = &length)
        fixed (int* valuesPtr = &values)
        {
            this.glGetSynciv.Invoke(sync, pname, count, lengthPtr, valuesPtr);
        }
    }

    public nint MapBufferRange(BufferTargetARB target, nint offset, nint length, MapBufferAccessMask access) =>
        this.glMapBufferRange.Invoke(target, offset, length, access);

    public nint MapBufferRange(BufferTargetARB target, int offset, int length, MapBufferAccessMask access) =>
        this.glMapBufferRange.Invoke(target, offset, length, access);

    public void LinkProgram(uint program) =>
        this.glLinkProgram.Invoke(program);

    public void PixelStorei(PixelStoreParameter pname, int param) =>
        this.glPixelStorei.Invoke(pname, param);

    public void QueryCounter(uint id, QueryCounterTarget target) =>
        this.glQueryCounter.Invoke(id, target);

    public void Scissor(int x, int y, int width, int height) =>
        this.glScissor.Invoke(x, y, width, height);

    public unsafe void ShaderSource(uint shader, int count, /*const GLchar * const **/ char** @string, /*const*/ int* length) =>
        this.glShaderSource.Invoke(shader, count, @string, length);

    public unsafe void ShaderSource(uint shader, string @string)
    {
        using var pointer = new StringA(@string);

        var stringArray = stackalloc char*[1];
        var sizeArray   = stackalloc int[1];

        stringArray[0] = pointer;
        sizeArray[0]   = @string.Length;

        this.glShaderSource.Invoke(shader, 1, stringArray, sizeArray);
    }

    public unsafe void ShaderSource(uint shader, string[] @string)
    {
        var stringArray = stackalloc char*[@string.Length];
        var sizeArray   = stackalloc int[@string.Length];

        for (var i = 0; i < @string.Length; i++)
        {
            stringArray[i] = (char*)Marshal.StringToHGlobalAnsi(@string[i]);
            sizeArray[i]   = @string[i].Length;
        }

        this.glShaderSource.Invoke(shader, @string.Length, stringArray, sizeArray);

        for (var i = 0; i < @string.Length; i++)
        {
            Marshal.FreeHGlobal((nint)stringArray[i]);
        }
    }

    public void TexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, nint pixels) =>
        this.glTexImage2D.Invoke(target, level, internalformat, width, height, border, format, type, pixels);

    public unsafe void TexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, Span<byte> pixels)
    {
        fixed (byte* pointer = pixels)
        {
            this.glTexImage2D.Invoke(target, level, internalformat, width, height, border, format, type, (nint)pointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe void TexImage2D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int border, PixelFormat format, PixelType type, byte[] pixels) =>
        this.TexImage2D(target, level, internalformat, width, height, border, format, type, pixels.AsSpan());

    public void TexImage3D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, nint pixels) =>
        this.glTexImage3D.Invoke(target, level, internalformat, width, height, depth, border, format, type, pixels);

    public unsafe void TexImage3D(TextureTarget target, int level, InternalFormat internalformat, int width, int height, int depth, int border, PixelFormat format, PixelType type, byte[] pixels)
    {
        fixed (byte* pointer = pixels)
        {
            this.glTexImage3D.Invoke(target, level, internalformat, width, height, depth, border, format, type, (nint)pointer);
        }
    }

    public void TexParameterI(TextureTarget target, TextureParameterName pname, int param) =>
        this.glTexParameteri.Invoke(target, pname, param);

    public void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, nint pixels) =>
        this.glTexSubImage3D.Invoke(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);

    public unsafe void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, Span<byte> pixels)
    {
        fixed (byte* pointer = pixels)
        {
            this.glTexSubImage3D.Invoke(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, (nint)pointer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public unsafe void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, byte[] pixels) =>
        this.TexSubImage3D(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels.AsSpan());

    public unsafe void TransformFeedbackVaryings(uint program, string[] varyings, TransformFeedbackBufferMode bufferMode)
    {
        fixed (char* varyingsPtr = varyings.SelectMany(x => x).ToArray())
        {
            this.glTransformFeedbackVaryings.Invoke(program, varyings.Length, varyingsPtr, bufferMode);
        }
    }

    public unsafe void TransformFeedbackVaryings(uint program, int count, /*const GLchar * const **/ char* varyings, TransformFeedbackBufferMode bufferMode) =>
        this.glTransformFeedbackVaryings.Invoke(program, count, varyings, bufferMode);

    public bool TryGetExtension<T>(out T? extension) where T : class, IExtension
    {
        if (this.HasExtension(T.Name))
        {
            extension = T.Create(this.loader) as T;
            return true;
        }

        extension = null;
        return false;
    }

    public void Uniform1i(int location, int v0) => throw new NotImplementedException();
    public void Uniform1ui(int location, uint v0) => throw new NotImplementedException();
    public void Uniform1f(int location, float v0) => throw new NotImplementedException();
    public void Uniform2f(int location, float v0, float v1) => throw new NotImplementedException();
    public void Uniform3f(int location, float v0, float v1, float v2) => throw new NotImplementedException();
    public void Uniform4f(int location, float v0, float v1, float v2, float v3) => throw new NotImplementedException();
    public unsafe void Uniform2fv(int location, int count, /*const*/ float* value) => throw new NotImplementedException();
    public void Uniform2fv(int location, int count, float[] value) => throw new NotImplementedException();
    public unsafe void Uniform2iv(int location, int count, /*const*/ int* value) => throw new NotImplementedException();
    public void Uniform2iv(int location, int count, int[] value) => throw new NotImplementedException();
    public unsafe void Uniform3fv(int location, int count, /*const*/ float* value) => throw new NotImplementedException();
    public void Uniform3fv(int location, int count, float[] value) => throw new NotImplementedException();
    public unsafe void Uniform4fv(int location, int count, /*const*/ float* value) => throw new NotImplementedException();
    public void Uniform4fv(int location, int count, float[] value) => throw new NotImplementedException();

    public void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding) =>
        this.glUniformBlockBinding.Invoke(program, uniformBlockIndex, uniformBlockBinding);

    public unsafe void UniformMatrix4fv(int location, int count, bool transpose, /*const*/ float* value) => throw new NotImplementedException();
    public void UniformMatrix4fv(int location, int count, bool transpose, float[] value) => throw new NotImplementedException();

    public void UseProgram(uint program) =>
        this.glUseProgram.Invoke(program);

    public void VertexAttribDivisor(uint index, uint divisor) =>
        this.glVertexAttribDivisor.Invoke(index, divisor);

    public void VertexAttribIPointer(uint index, int size, VertexAttribIType type, int stride, nint pointer) =>
        this.glVertexAttribIPointer.Invoke(index, size, type, stride, pointer);

    public void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, nint pointer) =>
        this.glVertexAttribPointer.Invoke(index, size, type, normalized, stride, pointer);

    public void VertexAttrib4f(uint index, float x, float y, float z, float w) =>
        this.glVertexAttrib4f.Invoke(index, x, y, z, w);

    public void Viewport(int x, int y, int width, int height) =>
        this.glViewport.Invoke(x, y, width, height);

    public bool UnmapBuffer(BufferTargetARB target) =>
        this.glUnmapBuffer.Invoke(target);
}
