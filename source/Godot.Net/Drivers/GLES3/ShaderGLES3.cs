#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;

using System.Text;
using Godot.Net.Core;
using Godot.Net.Core.Generics;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Extensions;

#pragma warning disable IDE0044, IDE0052, CS0649, CS0414, IDE0051, CS0169 // TODO Remove

public abstract partial class ShaderGLES3
{
    private static bool    printOnceVersionBindShader;
    private static bool    shaderCacheCleanupOnStart;
    private static string? shaderCacheDir;
    private static bool    shaderCacheSaveCompressed;
    private static bool    shaderCacheSaveCompressedZstd;
    private static bool    shaderCacheSaveDebug;

    private string?                 baseSha256;
    private int                     baseTextureIndex;
    private Version.Specialization? currentShader;
    private Feedback[]              feedbacks       = Array.Empty<Feedback>();
    private string?                 generalDefines;
    private int                     maxImageUnits;
    private string?                 name;
    private bool                    shaderCacheDirValid;
    private ulong                   specializationDefaultMask;
    private Specialization[]        specializations = Array.Empty<Specialization>();
    private StageTemplate[]         stageTemplates  = Common.FillArray<StageTemplate>(2);
    private TexUnitPair[]           texunitPairs    = Array.Empty<TexUnitPair>();
    private UBOPair[]               uboPairs        = Array.Empty<UBOPair>();
    private string[]                uniformNames    = Array.Empty<string>();
    private string[]                variantDefines  = Array.Empty<string>();
    private GuidOwner<Version>      versionOwner    = new(true);

    private static void ClearVersion(Version version)
    {
        // Variants not compiled yet, just return
        if (version.Variants.Count == 0)
        {
            return;
        }

        var gl = GL.Singleton;

        foreach (var value in version.Variants.SelectMany(x => x.Values))
        {
            if (value.Id != 0)
            {
                gl.DeleteShader(value.VertId);
                gl.DeleteShader(value.FragId);
                gl.DeleteProgram(value.Id);
            }
        }

        version.Variants.Clear();
    }

    private static void DisplayErrorWithCode(string error, string code)
    {
        var line = 1;
        var lines = code.Split("\n");

        for (var j = 0; j < lines.Length; j++)
        {
            PrintLine(line + ": " + lines[j]);
            line++;
        }

        ERR_PRINT(error);
    }

    private static string Mkid(string id) =>
        "m_" + id.Replace("__", "_dus_").Replace("__", "_dus_"); //doubleunderscore is reserved in glsl

    public static void SetShaderCacheDir(string dir) => throw new NotImplementedException();
    public static void SetShaderCacheSaveCompressed(bool enable) => throw new NotImplementedException();
    public static void SetShaderCacheSaveCompressedZstd(bool enable) => throw new NotImplementedException();
    public static void SetShaderCacheSaveDebug(bool enable) => throw new NotImplementedException();

    protected abstract void Init();

    private void AddStage(string code, StageType stageType)
    {
        var lines = code.Split("\n");

        var text = new StringBuilder();

        for (var i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            var pushChunk = false;

            var chunk = new StageTemplate.Chunk();

            if (l.StartsWith("#GLOBALS"))
            {
                switch (stageType)
                {
                    case StageType.STAGE_TYPE_VERTEX:
                        chunk.Type = StageTemplate.ChunkType.TYPE_VERTEX_GLOBALS;
                        break;
                    case StageType.STAGE_TYPE_FRAGMENT:
                        chunk.Type = StageTemplate.ChunkType.TYPE_FRAGMENT_GLOBALS;
                        break;
                    default:
                        break;
                }

                pushChunk = true;
            }
            else if (l.StartsWith("#MATERIAL_UNIFORMS"))
            {
                chunk.Type = StageTemplate.ChunkType.TYPE_MATERIAL_UNIFORMS;
                pushChunk = true;
            }
            else if (l.StartsWith("#CODE"))
            {
                chunk.Type = StageTemplate.ChunkType.TYPE_CODE;
                pushChunk  = true;
                chunk.Code = l.ReplaceFirst("#CODE", "").Replace(":", "").Trim().ToUpper();
            }
            else
            {
                text.AppendLine(l);
            }

            if (pushChunk)
            {
                if (text.Length > 0)
                {
                    var textChunk = new StageTemplate.Chunk
                    {
                        Type = StageTemplate.ChunkType.TYPE_TEXT,
                        Text = text.ToString(),
                    };

                    this.stageTemplates[(int)stageType].Chunks.Add(textChunk);
                    text.Clear();
                }
                this.stageTemplates[(int)stageType].Chunks.Add(chunk);
            }

            if (text.Length > 0)
            {
                var textChunk = new StageTemplate.Chunk
                {
                    Type = StageTemplate.ChunkType.TYPE_TEXT,
                    Text = text.ToString(),
                };

                this.stageTemplates[(int)stageType].Chunks.Add(textChunk);
                text.Clear();
            }
        }
    }

    private void BuildVariantCode(uint variant, Version version, StageType stageType, ulong specialization, out string code)
    {
        var builder = new StringBuilder();

        #if GLES_OVER_GL
        builder.Append("#version 330\n");
        builder.Append("#define USE_GLES_OVER_GL\n");
        #else
        builder.Append("#version 300 es\n");
        #endif

        for (var i = 0; i < this.specializations.Length; i++)
        {
            if ((specialization & (ulong)(1 << i)) != 0)
            {
                builder.Append($"#define {this.specializations[i].Name}\n");
            }
        }

        if (!string.IsNullOrEmpty(version.Uniforms))
        {
            builder.Append("#define MATERIAL_UNIFORMS_USED\n");
        }

        foreach (var e in version.CodeSections)
        {
            builder.Append($"#define {e.Key}_CODE_USED\n");
        }

        builder.Append('\n'); //make sure defines begin at newline
        builder.Append(this.generalDefines);
        builder.Append('\n');
        builder.Append(this.variantDefines[variant]);
        builder.Append('\n');

        for (var j = 0; j < version.CustomDefines.Count; j++)
        {
            builder.Append(version.CustomDefines[j]);
        }
        builder.Append('\n'); //make sure defines begin at newline

        // Insert multiview extension loading, because it needs to appear before
        // any non-preprocessor code (like the "precision highp..." lines below).
        builder.Append("#ifdef USE_MULTIVIEW\n");
        builder.Append("#if defined(GL_OVR_multiview2)\n");
        builder.Append("#extension GL_OVR_multiview2 : require\n");
        builder.Append("#elif defined(GL_OVR_multiview)\n");
        builder.Append("#extension GL_OVR_multiview : require\n");
        builder.Append("#endif\n");

        if (stageType == StageType.STAGE_TYPE_VERTEX)
        {
            builder.Append("layout(num_views=2) in;\n");
        }

        builder.Append("#define ViewIndex gl_ViewID_OVR\n");
        builder.Append("#define MAX_VIEWS 2\n");
        builder.Append("#else\n");
        builder.Append("#define ViewIndex uint(0)\n");
        builder.Append("#define MAX_VIEWS 1\n");
        builder.Append("#endif\n");

        // Default to highp precision unless specified otherwise.
        builder.Append("precision highp float;\n");
        builder.Append("precision highp int;\n");

        #if !GLES_OVER_GL
        builder.Append("precision highp sampler2D;\n");
        builder.Append("precision highp samplerCube;\n");
        builder.Append("precision highp sampler2DArray;\n");
        #endif

        var stageTemplate = this.stageTemplates[(int)stageType];

        for (var i = 0; i < stageTemplate.Chunks.Count; i++)
        {
            var chunk = stageTemplate.Chunks[i];

            switch (chunk.Type)
            {
                case StageTemplate.ChunkType.TYPE_MATERIAL_UNIFORMS:
                    builder.Append(version.Uniforms); //uniforms (same for vertex and fragment)
                    break;
                case StageTemplate.ChunkType.TYPE_VERTEX_GLOBALS:
                    builder.Append(version.VertexGlobals); // vertex globals
                    break;
                case StageTemplate.ChunkType.TYPE_FRAGMENT_GLOBALS:
                    builder.Append(version.FragmentGlobals); // fragment globals
                    break;
                case StageTemplate.ChunkType.TYPE_CODE:
                    if (version.CodeSections.TryGetValue(chunk.Code!, out var value))
                    {
                        builder.Append(value);
                    }
                    break;
                case StageTemplate.ChunkType.TYPE_TEXT:
                    builder.Append(chunk.Text);
                    break;
            }
        }

        code = builder.ToString();
    }

    private void CompileSpecialization(Version.Specialization spec, uint variant, Version version, ulong specialization)
    {
        var gl = GL.Singleton;

        spec.Id = gl.CreateProgram();
        spec.Ok = false;

        //vertex stage
        {
            this.BuildVariantCode(variant, version, StageType.STAGE_TYPE_VERTEX, specialization, out var code);

            spec.VertId = gl.CreateShader(ShaderType.VertexShader);

            gl.ShaderSource(spec.VertId, code);
            gl.CompileShader(spec.VertId);

            gl.GetShaderiv(spec.VertId, ShaderParameterName.CompileStatus, out var status);
            if (status == 0)
            {
                gl.GetShaderiv(spec.VertId, ShaderParameterName.InfoLogLength, out var iloglen);

                if (iloglen < 0)
                {
                    gl.DeleteShader(spec.VertId);
                    gl.DeleteProgram(spec.Id);

                    spec.Id = 0;

                    ERR_PRINT("No OpenGL vertex shader compiler log.");
                }
                else
                {
                    if (iloglen == 0)
                    {
                        iloglen = 4096; // buggy driver (Adreno 220+)
                    }

                    gl.GetShaderInfoLog(spec.VertId, iloglen, out var ilogmem);

                    var errString = $"{this.name}: Vertex shader compilation failed:\n{ilogmem}";

                    errString += ilogmem;

                    DisplayErrorWithCode(errString, code);

                    gl.DeleteShader(spec.VertId);
                    gl.DeleteProgram(spec.Id);

                    spec.Id = 0;
                }

                ERR_FAIL();

                return;
            }
        }

        //fragment stage
        {
            this.BuildVariantCode(variant, version, StageType.STAGE_TYPE_FRAGMENT, specialization, out var code);

            spec.FragId = gl.CreateShader(ShaderType.FragmentShader);

            gl.ShaderSource(spec.FragId, code);
            gl.CompileShader(spec.FragId);

            gl.GetShaderiv(spec.FragId, ShaderParameterName.CompileStatus, out var status);
            if (status == 0)
            {
                gl.GetShaderiv(spec.FragId, ShaderParameterName.InfoLogLength, out var iloglen);

                if (iloglen < 0)
                {
                    gl.DeleteShader(spec.FragId);
                    gl.DeleteProgram(spec.Id);

                    spec.Id = 0;

                    ERR_PRINT("No OpenGL fragment shader compiler log.");
                }
                else
                {
                    if (iloglen == 0)
                    {
                        iloglen = 4096; // buggy driver (Adreno 220+)
                    }

                    gl.GetShaderInfoLog(spec.FragId, iloglen, out var ilogmem);

                    var errString = $"{this.name}: Fragment shader compilation failed:\n{ilogmem}";

                    DisplayErrorWithCode(errString, code);

                    gl.DeleteShader(spec.FragId);
                    gl.DeleteProgram(spec.Id);

                    spec.Id = 0;
                }

                ERR_FAIL();

                return;
            }
        }

        gl.AttachShader(spec.Id, spec.FragId);
        gl.AttachShader(spec.Id, spec.VertId);

        // If feedback exists, set it up.

        if (this.feedbacks.Length > 0)
        {
            var feedback = new List<string>();

            for (var i = 0; i < this.feedbacks.Length; i++)
            {
                if (this.feedbacks[i].Specialization == 0 || (this.feedbacks[i].Specialization & specialization) != 0)
                {
                    // Specialization for this feedback is enabled
                    feedback.Add(this.feedbacks[i].Name);
                }
            }

            if (feedback.Count != 0)
            {
                gl.TransformFeedbackVaryings(spec.Id, feedback.ToArray(), TransformFeedbackBufferMode.InterleavedAttribs);
            }
        }

        gl.LinkProgram(spec.Id);

        {
            gl.GetProgramiv(spec.Id, ProgramPropertyARB.LinkStatus, out var status);

            if (status == 0)
            {
                gl.GetProgramiv(spec.Id, ProgramPropertyARB.InfoLogLength, out var iloglen);

                if (iloglen < 0)
                {
                    gl.DeleteShader(spec.FragId);
                    gl.DeleteShader(spec.VertId);
                    gl.DeleteProgram(spec.Id);

                    spec.Id = 0;

                    ERR_PRINT("No OpenGL program link log. Something is wrong.");
                    ERR_FAIL();

                    return;
                }

                if (iloglen == 0)
                {
                    iloglen = 4096; // buggy driver (Adreno 220+)
                }

                gl.GetProgramInfoLog(spec.Id, iloglen, out var ilogmem);

                var errString = $"{this.name}: Program linking failed:\n{ilogmem}";

                DisplayErrorWithCode(errString, "");

                gl.DeleteShader(spec.FragId);
                gl.DeleteShader(spec.VertId);
                gl.DeleteProgram(spec.Id);

                spec.Id = 0;

                ERR_FAIL();

                return;
            }
        }

        // get uniform locations

        gl.UseProgram(spec.Id);

        spec.UniformLocation.Capacity = this.uniformNames.Length;

        for (var i = 0; i < this.uniformNames.Length; i++)
        {
            spec.UniformLocation.Add(gl.GetUniformLocation(spec.Id, this.uniformNames[i]));
        }

        for (var i = 0; i < this.texunitPairs.Length; i++)
        {
            var loc = gl.GetUniformLocation(spec.Id, this.texunitPairs[i].Name);

            if (loc >= 0)
            {
                if (this.texunitPairs[i].Index < 0)
                {
                    gl.Uniform1i(loc, this.maxImageUnits + this.texunitPairs[i].Index);
                }
                else
                {
                    gl.Uniform1i(loc, this.texunitPairs[i].Index);
                }
            }
        }

        for (var i = 0; i < this.uboPairs.Length; i++)
        {
            var loc = gl.GetUniformBlockIndex(spec.Id, this.uboPairs[i].Name);

            if (loc >= 0)
            {
                gl.UniformBlockBinding(spec.Id, loc, (uint)this.uboPairs[i].Index);
            }
        }

        // textures
        for (var i = 0; i < version.TextureUniforms.Count; i++)
        {
            var nativeUniformName = Mkid(version.TextureUniforms[i]);
            var location          = gl.GetUniformLocation(spec.Id, nativeUniformName);

            gl.Uniform1i(location, i + this.baseTextureIndex);
        }

        gl.UseProgram(0);

        spec.Ok = true;
    }

    private void InitializeVersion(Version version)
    {
        if (ERR_FAIL_COND(version.Variants.Count > 0))
        {
            return;
        }

        version.Variants.Capacity = this.variantDefines.Length;

        for (var i = 0; i < this.variantDefines.Length; i++)
        {
            var variant = new Dictionary<ulong, Version.Specialization>();

            version.Variants.Add(variant);

            var spec = new Version.Specialization();

            this.CompileSpecialization(spec, (uint)i, version, this.specializationDefaultMask);

            version.Variants[i].Add(this.specializationDefaultMask, spec);
        }
    }

    private bool LoadFromCache(Version version) => throw new NotImplementedException();
    private void SaveToCache(Version version) => throw new NotImplementedException();
    private string VersionGetSha1(Version version) => throw new NotImplementedException();

    // void ShaderGLES3::_setup(const char *p_vertex_code, const char *p_fragment_code, const char *p_name, int p_uniform_count, const char **p_uniform_names, int p_ubo_count, const UBOPair *p_ubos, int p_feedback_count, const Feedback *p_feedback, int p_texture_count, const TexUnitPair *p_tex_units, int specialization_count, const Specialization *specializations, int variant_count, const char **variants)
    protected void Setup(string vertexCode, string fragmentCode, string name, string[] uniformNames, UBOPair[] ubos, Feedback[] feedback, TexUnitPair[] texUnits, Specialization[] specializations, string[] variants)
    {
        this.name = name;

        if (!string.IsNullOrEmpty(vertexCode))
        {
            this.AddStage(vertexCode, StageType.STAGE_TYPE_VERTEX);
        }
        if (!string.IsNullOrEmpty(fragmentCode))
        {
            this.AddStage(fragmentCode, StageType.STAGE_TYPE_FRAGMENT);
        }

        this.uniformNames              = uniformNames;
        this.uboPairs                  = ubos;
        this.texunitPairs              = texUnits;
        this.specializations           = specializations;
        this.specializationDefaultMask = 0;
        for (var i = 0; i < specializations.Length; i++)
        {
            if (specializations[i].DefaultValue)
            {
                this.specializationDefaultMask |= 1UL << i;
            }
        }
        this.variantDefines = variants;
        this.feedbacks      = feedback;

        var tohash = new StringBuilder();
        /*
        tohash.append("[SpirvCacheKey]");
        tohash.append(RenderingDevice::get_singleton().shader_get_spirv_cache_key());
        tohash.append("[BinaryCacheKey]");
        tohash.append(RenderingDevice::get_singleton().shader_get_binary_cache_key());
        */
        tohash.Append("[Vertex]");
        tohash.Append(vertexCode);
        tohash.Append("[Fragment]");
        tohash.Append(fragmentCode);

        this.baseSha256 = tohash.ToString().ToSHA256();
    }

    protected bool VersionBindShader(Guid versionId, int variant, ulong specialization)
    {
        var gl = GL.Singleton;

        if (ERR_FAIL_INDEX_V(variant, this.variantDefines.Length))
        {
            return false;
        }

        var version = this.versionOwner.GetOrNull(versionId);

        if (ERR_FAIL_COND_V(version == null))
        {
            return false;
        }

        if (version!.Variants.Count == 0)
        {
            this.InitializeVersion(version); //may lack initialization
        }

        if (!version.Variants[variant].TryGetValue(specialization, out var spec))
        {
            #pragma warning disable CS0162
            if (false)
            {
                // Queue load this specialization and use defaults in the meantime (TODO)

                spec = version.Variants[variant][this.specializationDefaultMask];
            }
            #pragma warning restore CS0162
            else
            {
                // Compile on the spot
                spec = new Version.Specialization();
                this.CompileSpecialization(spec, (uint)variant, version, specialization);
                version.Variants[variant].Add(specialization, spec);
            }
        }
        else if (spec.BuildQueued)
        {
            // Still queued, wait
            spec = version.Variants[variant][this.specializationDefaultMask];
        }

        if (spec == null || !spec.Ok)
        {
            WARN_PRINT_ONCE("shader failed to compile, unable to bind shader.", ref printOnceVersionBindShader);
            return false;
        }

        gl.UseProgram(spec.Id);

        this.currentShader = spec;

        return true;
    }

    // _FORCE_INLINE_ int _version_get_uniform(int p_which, RID version, int variant, uint64_t specialization)
    protected int VersionGetUniform(int which, Guid version, int variant, ulong specialization) => throw new NotImplementedException();

    // void ShaderGLES3::initialize(const String &p_general_defines, int p_base_texture_index)
    public void Initialize(string? generalDefines = default, int baseTextureIndex = default)
    {
        this.generalDefines   = generalDefines;
        this.baseTextureIndex = baseTextureIndex;

        this.Init();

        if (shaderCacheDir != null)
        {
            var hashBuild = new StringBuilder();

            hashBuild.Append("[base_hash]");
            hashBuild.Append(this.baseSha256);
            hashBuild.Append("[general_defines]");
            hashBuild.Append(generalDefines);
            for (var i = 0; i < this.variantDefines.Length; i++)
            {
                hashBuild.Append("[variant_defines:" + i + "]");
                hashBuild.Append(this.variantDefines[i]);
            }

            this.baseSha256 = hashBuild.ToString().ToSHA256();

            if (!Directory.Exists(shaderCacheDir))
            {
                var dir          = shaderCacheDir;
                var dirSlashName = Path.Join(dir, this.name);

                if (!Directory.Exists(dirSlashName))
                {
                    Directory.CreateDirectory(dirSlashName);
                    dir = dirSlashName;
                }
                //erase other versions?
                if (shaderCacheCleanupOnStart)
                {
                }

                var dirSlashBaseSha256 = Path.Join(dir, this.baseSha256);
                //
                if (!Directory.Exists(dirSlashBaseSha256))
                {
                    Directory.CreateDirectory(dirSlashName);
                }
            }

            this.shaderCacheDirValid = true;

            PrintVerbose($"Shader '{this.name}' SHA256: {this.baseSha256}");
        }

        GL.Singleton.GetIntegerv(GetPName.MaxTextureImageUnits, out this.maxImageUnits);
    }

    // RID ShaderGLES3::version_create()
    public Guid VersionCreate() =>
        //initialize() was never called
        ERR_FAIL_COND_V(this.variantDefines.Length == 0) ? default : this.versionOwner.Initialize();

    public bool VersionFree(Guid version) => throw new NotImplementedException();
    public RS.ShaderNativeSourceCode VersionGetNativeSourceCode(Guid version) => throw new NotImplementedException();

    public bool VersionIsValid(Guid version) => this.versionOwner.GetOrNull(version) != null;

    public void VersionSetCode(Guid versionId, Dictionary<string, string> code, string? uniforms, string? vertexGlobals, string? fragmentGlobals, List<string> customDefines, List<string> textureUniforms, bool initialize = false)
    {
        var version = this.versionOwner.GetOrNull(versionId);

        if (ERR_FAIL_COND(version == null))
        {
            return;
        }

        ClearVersion(version!); //clear if existing

        version!.VertexGlobals  = vertexGlobals;
        version.FragmentGlobals = fragmentGlobals;
        version.Uniforms        = uniforms;
        version.CodeSections.Clear();
        version.TextureUniforms.AddRange(textureUniforms);

        foreach (var e in code)
        {
            version.CodeSections[e.Key.ToUpper()] = e.Value;
        }

        version.CustomDefines.Clear();
        version.CustomDefines.AddRange(customDefines);

        if (initialize)
        {
            this.InitializeVersion(version);
        }
    }
}
