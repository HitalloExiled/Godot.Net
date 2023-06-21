namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Error;
using Godot.Net.Servers.Rendering;

public class SkyShaderData : ShaderData
{
    public string?                                    Code           { get; set; }
    public List<ShaderCompiler.GeneratedCode.Texture> TextureUniforms { get; set; } = new();
    public List<uint>                                 UboOffsets      { get; set; } = new();
    public uint                                       UboSize         { get; set; }
    public bool                                       UsesHalfRes     { get; set; }
    public bool                                       UsesLight       { get; set; }
    public bool                                       UsesPosition    { get; set; }
    public bool                                       UsesQuarterRes  { get; set; }
    public bool                                       UsesTime        { get; set; }
    public bool                                       Valid           { get; set; }
    public Guid                                       Version         { get; set; }

    public override bool CastsShadows() => throw new NotImplementedException();
    public override bool IsAnimated() => throw new NotImplementedException();

    public override void SetCode(string code)
    {
        //compile

        this.Code    =  code;
        this.Valid   = false;
        this.UboSize = 0;
        this.Uniforms.Clear();

        if (string.IsNullOrEmpty(this.Code))
        {
            return; //just invalid, but no error
        }

        var actions = new ShaderCompiler.IdentifierActions();

        actions.EntryPointStages["sky"] = ShaderCompiler.Stage.STAGE_FRAGMENT;

        this.UsesTime       = false;
        this.UsesHalfRes    = false;
        this.UsesQuarterRes = false;
        this.UsesPosition   = false;
        this.UsesLight      = false;

        actions.RenderModeFlags["use_half_res_pass"]    = this.UsesHalfRes;
        actions.RenderModeFlags["use_quarter_res_pass"] = this.UsesQuarterRes;

        actions.UsageFlagPointers["TIME"]             = this.UsesTime;
        actions.UsageFlagPointers["POSITION"]         = this.UsesPosition;
        actions.UsageFlagPointers["LIGHT0_ENABLED"]   = this.UsesLight;
        actions.UsageFlagPointers["LIGHT0_ENERGY"]    = this.UsesLight;
        actions.UsageFlagPointers["LIGHT0_DIRECTION"] = this.UsesLight;
        actions.UsageFlagPointers["LIGHT0_COLOR"]     = this.UsesLight;
        actions.UsageFlagPointers["LIGHT0_SIZE"]      = this.UsesLight;
        actions.UsageFlagPointers["LIGHT1_ENABLED"]   = this.UsesLight;
        actions.UsageFlagPointers["LIGHT1_ENERGY"]    = this.UsesLight;
        actions.UsageFlagPointers["LIGHT1_DIRECTION"] = this.UsesLight;
        actions.UsageFlagPointers["LIGHT1_COLOR"]     = this.UsesLight;
        actions.UsageFlagPointers["LIGHT1_SIZE"]      = this.UsesLight;
        actions.UsageFlagPointers["LIGHT2_ENABLED"]   = this.UsesLight;
        actions.UsageFlagPointers["LIGHT2_ENERGY"]    = this.UsesLight;
        actions.UsageFlagPointers["LIGHT2_DIRECTION"] = this.UsesLight;
        actions.UsageFlagPointers["LIGHT2_COLOR"]     = this.UsesLight;
        actions.UsageFlagPointers["LIGHT2_SIZE"]      = this.UsesLight;
        actions.UsageFlagPointers["LIGHT3_ENABLED"]   = this.UsesLight;
        actions.UsageFlagPointers["LIGHT3_ENERGY"]    = this.UsesLight;
        actions.UsageFlagPointers["LIGHT3_DIRECTION"] = this.UsesLight;
        actions.UsageFlagPointers["LIGHT3_COLOR"]     = this.UsesLight;
        actions.UsageFlagPointers["LIGHT3_SIZE"]      = this.UsesLight;

        actions.Uniforms = this.Uniforms;

        var err = MaterialStorage.Singleton.Shaders.CompilerSky.Compile(RS.ShaderMode.SHADER_SKY, code, actions, this.Path!, out var genCode);

        if (ERR_FAIL_COND_MSG(err != Error.OK, "Shader compilation failed."))
        {
            return;
        }

        if (this.Version == default)
        {
            this.Version = MaterialStorage.Singleton.Shaders.SkyShader.VersionCreate();
        }

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

        MaterialStorage.Singleton.Shaders.SkyShader.VersionSetCode(
            this.Version,
            genCode.Code,
            genCode.Uniforms,
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_VERTEX],
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_FRAGMENT],
            genCode.Defines,
            textureUniformNames
        );

        if (ERR_FAIL_COND(!MaterialStorage.Singleton.Shaders.SkyShader.VersionIsValid(this.Version)))
        {
            return;
        }

        this.UboSize         = genCode.UniformTotalSize;
        this.UboOffsets      = genCode.UniformOffsets;
        this.TextureUniforms = genCode.TextureUniforms;

        this.Valid = true;
    }
}
