namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Error;
using Godot.Net.Servers.Rendering;

public partial class CanvasShaderData : ShaderData
{
    public BlendModeType                              BlendMode                { get; set; } = BlendModeType.BLEND_MODE_MIX;
    public string?                                    Code                     { get; set; }
    public List<ShaderCompiler.GeneratedCode.Texture> TextureUniforms          { get; set; } = new();
    public List<uint>                                 UboOffsets               { get; set; } = new();
    public uint                                       UboSize                  { get; set; }
    public bool                                       UsesScreenTexture        { get; set; }
    public bool                                       UsesScreenTextureMipmaps { get; set; }
    public bool                                       UsesSdf                  { get; set; }
    public bool                                       UsesTime                 { get; set; }
    public bool                                       Valid                    { get; set; }
    public Guid                                       Version                  { get; set; }

    public override bool CastsShadows() => throw new NotImplementedException();
    public override bool IsAnimated() => throw new NotImplementedException();

    // void CanvasShaderData::set_code(const String &p_code)
    public override void SetCode(string code)
    {
        // compile the shader

        this.Code                     = code;
        this.Valid                    = false;
        this.UboSize                  = 0;
        this.Uniforms.Clear();
        this.UsesScreenTexture        = false;
        this.UsesScreenTextureMipmaps = false;
        this.UsesSdf                  = false;
        this.UsesTime                 = false;

        if (string.IsNullOrEmpty(code))
        {
            return; //just invalid, but no error
        }

        var blendModei = (int)BlendModeType.BLEND_MODE_MIX;

        var actions = new ShaderCompiler.IdentifierActions();

        actions.EntryPointStages.Add("vertex",   ShaderCompiler.Stage.STAGE_VERTEX);
        actions.EntryPointStages.Add("fragment", ShaderCompiler.Stage.STAGE_FRAGMENT);
        actions.EntryPointStages.Add("light",    ShaderCompiler.Stage.STAGE_FRAGMENT);

        actions.RenderModeValues.Add("blend_add",          new(blendModei, (int)BlendModeType.BLEND_MODE_ADD));
        actions.RenderModeValues.Add("blend_mix",          new(blendModei, (int)BlendModeType.BLEND_MODE_MIX));
        actions.RenderModeValues.Add("blend_sub",          new(blendModei, (int)BlendModeType.BLEND_MODE_SUB));
        actions.RenderModeValues.Add("blend_mul",          new(blendModei, (int)BlendModeType.BLEND_MODE_MUL));
        actions.RenderModeValues.Add("blend_premul_alpha", new(blendModei, (int)BlendModeType.BLEND_MODE_PMALPHA));
        actions.RenderModeValues.Add("blend_disabled",     new(blendModei, (int)BlendModeType.BLEND_MODE_DISABLED));

        actions.UsageFlagPointers.Add("texture_sdf", this.UsesSdf);
        actions.UsageFlagPointers.Add("TIME",        this.UsesTime);

        actions.Uniforms = this.Uniforms;

        var err = MaterialStorage.Singleton.Shaders.CompilerCanvas.Compile(RS.ShaderMode.SHADER_CANVAS_ITEM, code, actions, this.Path!, out var genCode);

        if (ERR_FAIL_COND_MSG(err != Error.OK, "Shader compilation failed."))
        {
            return;
        }

        if (this.Version == default)
        {
            this.Version = MaterialStorage.Singleton.Shaders.CanvasShader.VersionCreate();
        }

        this.BlendMode                = (BlendModeType)blendModei;
        this.UsesScreenTextureMipmaps = genCode.UsesScreenTextureMipmaps;
        this.UsesScreenTexture        = genCode.UsesScreenTexture;

        #if _0
        PrintLine("**compiling shader:");
        PrintLine("**defines:\n");
        for (var i = 0; i < genCode.Defines.Count; i++)
        {
            PrintLine(genCode.Defines[i]);
        }

        foreach (var el in genCode.Code)
        {
            PrintLine("\n**code " + el.Key + ":\n" + el.Value);
        }

        PrintLine("\n**uniforms:\n" + genCode.Uniforms);
        PrintLine("\n**vertex_globals:\n" + genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_VERTEX]);
        PrintLine("\n**fragment_globals:\n" + genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_FRAGMENT]);
        #endif

        var textureUniformNames = new List<string>();
        for (var i = 0; i < genCode.TextureUniforms.Count; i++)
        {
            textureUniformNames.Add(genCode.TextureUniforms[i].Name!);
        }

        MaterialStorage.Singleton.Shaders.CanvasShader.VersionSetCode(
            this.Version,
            genCode.Code,
            genCode.Uniforms,
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_VERTEX],
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_FRAGMENT],
            genCode.Defines,
            textureUniformNames
        );

        if (ERR_FAIL_COND(!MaterialStorage.Singleton.Shaders.CanvasShader.VersionIsValid(this.Version)))
        {
            return;
        }

        this.UboSize         = genCode.UniformTotalSize;
        this.UboOffsets      = genCode.UniformOffsets;
        this.TextureUniforms = genCode.TextureUniforms;

        this.Valid = true;
    }
}
