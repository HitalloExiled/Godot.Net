namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Error;
using Godot.Net.Servers.Rendering;

public partial class SceneShaderData : ShaderData
{
    private bool warnPrintOnce;

    public AlphaAntiAliasing                          AlphaAntialiasingMode       { get; set; }
    public BlendModeKind                              BlendMode                   { get; set; }
    public string?                                    Code                        { get; set; }
    public Cull                                       CullMode                    { get; set; }
    public DepthDrawKind                              DepthDraw                   { get; set; }
    public DepthTestKind                              DepthTest                   { get; set; }
    public uint                                       Index                       { get; set; }
    public ulong                                      LastPass                    { get; set; }
    public List<ShaderCompiler.GeneratedCode.Texture> TextureUniforms             { get; set; } = new();
    public List<uint>                                 UboOffsets                  { get; set; } = new();
    public uint                                       UboSize                     { get; set; }
    public bool                                       Unshaded                    { get; set; }
    public bool                                       UsesAlpha                   { get; set; }
    public bool                                       UsesAlphaClip               { get; set; }
    public bool                                       UsesBlendAlpha              { get; set; }
    public bool                                       UsesBones                   { get; set; }
    public bool                                       UsesColor                   { get; set; }
    public bool                                       UsesCustom0                 { get; set; }
    public bool                                       UsesCustom1                 { get; set; }
    public bool                                       UsesCustom2                 { get; set; }
    public bool                                       UsesCustom3                 { get; set; }
    public bool                                       UsesDepthPrepassAlpha       { get; set; }
    public bool                                       UsesDepthTexture            { get; set; }
    public bool                                       UsesDiscard                 { get; set; }
    public bool                                       UsesFragmentTime            { get; set; }
    public bool                                       UsesNormal                  { get; set; }
    public bool                                       UsesNormalTexture           { get; set; }
    public bool                                       UsesParticleTrails          { get; set; }
    public bool                                       UsesPointSize               { get; set; }
    public bool                                       UsesPosition                { get; set; }
    public bool                                       UsesRoughness               { get; set; }
    public bool                                       UsesScreenTexture           { get; set; }
    public bool                                       UsesScreenTextureMipmaps    { get; set; }
    public bool                                       UsesSss                     { get; set; }
    public bool                                       UsesTangent                 { get; set; }
    public bool                                       UsesTime                    { get; set; }
    public bool                                       UsesTransmittance           { get; set; }
    public bool                                       UsesUv2                     { get; set; }
    public bool                                       UsesUv                      { get; set; }
    public bool                                       UsesVertex                  { get; set; }
    public bool                                       UsesVertexTime              { get; set; }
    public bool                                       UsesWeights                 { get; set; }
    public bool                                       UsesWorldCoordinates        { get; set; }
    public bool                                       Valid                       { get; set; }
    public Guid                                       Version                     { get; set; }
    public uint                                       VertexInputMask             { get; set; }
    public bool                                       Wireframe                   { get; set; }
    public bool                                       WritesModelviewOrProjection { get; set; }

    public override bool CastsShadows() => throw new NotImplementedException();
    public override bool IsAnimated() => throw new NotImplementedException();

    public override void SetCode(string code)
    {
        //compile
        this.Code    = code;
        this.Valid   = false;
        this.UboSize = 0;

        this.Uniforms.Clear();

        if (string.IsNullOrEmpty(this.Code))
        {
            return; //just invalid, but no error
        }

        var blendModei             = (int)BlendModeKind.BLEND_MODE_MIX;
        var depthTesti             = (int)DepthTestKind.DEPTH_TEST_ENABLED;
        var alphaAntialiasingModei = (int)AlphaAntiAliasing.ALPHA_ANTIALIASING_OFF;
        var cullModei              = (int)Cull.CULL_BACK;
        var depthDrawi             = (int)DepthDrawKind.DEPTH_DRAW_OPAQUE;

        this.UsesPointSize         = false;
        this.UsesAlpha             = false;
        this.UsesAlphaClip         = false;
        this.UsesBlendAlpha        = false;
        this.UsesDepthPrepassAlpha = false;
        this.UsesDiscard           = false;
        this.UsesRoughness         = false;
        this.UsesNormal            = false;
        this.Wireframe             = false;

        this.Unshaded                    = false;
        this.UsesVertex                  = false;
        this.UsesPosition                = false;
        this.UsesSss                     = false;
        this.UsesTransmittance           = false;
        this.UsesScreenTexture           = false;
        this.UsesDepthTexture            = false;
        this.UsesNormalTexture           = false;
        this.UsesTime                    = false;
        this.WritesModelviewOrProjection = false;
        this.UsesWorldCoordinates        = false;
        this.UsesParticleTrails          = false;

        var actions = new ShaderCompiler.IdentifierActions();

        actions.EntryPointStages["vertex"]   = ShaderCompiler.Stage.STAGE_VERTEX;
        actions.EntryPointStages["fragment"] = ShaderCompiler.Stage.STAGE_FRAGMENT;
        actions.EntryPointStages["light"]    = ShaderCompiler.Stage.STAGE_FRAGMENT;

        actions.RenderModeValues["blend_add"] = (blendModei, (int)BlendModeKind.BLEND_MODE_ADD);
        actions.RenderModeValues["blend_mix"] = (blendModei, (int)BlendModeKind.BLEND_MODE_MIX);
        actions.RenderModeValues["blend_sub"] = (blendModei, (int)BlendModeKind.BLEND_MODE_SUB);
        actions.RenderModeValues["blend_mul"] = (blendModei, (int)BlendModeKind.BLEND_MODE_MUL);

        actions.RenderModeValues["alpha_to_coverage"]         = (alphaAntialiasingModei, (int)AlphaAntiAliasing.ALPHA_ANTIALIASING_ALPHA_TO_COVERAGE);
        actions.RenderModeValues["alpha_to_coverage_and_one"] = (alphaAntialiasingModei, (int)AlphaAntiAliasing.ALPHA_ANTIALIASING_ALPHA_TO_COVERAGE_AND_TO_ONE);

        actions.RenderModeValues["depth_draw_never"]  = (depthDrawi, (int)DepthDrawKind.DEPTH_DRAW_DISABLED);
        actions.RenderModeValues["depth_draw_opaque"] = (depthDrawi, (int)DepthDrawKind.DEPTH_DRAW_OPAQUE);
        actions.RenderModeValues["depth_draw_always"] = (depthDrawi, (int)DepthDrawKind.DEPTH_DRAW_ALWAYS);

        actions.RenderModeValues["depth_test_disabled"] = (depthTesti, (int)DepthTestKind.DEPTH_TEST_DISABLED);

        actions.RenderModeValues["cull_disabled"] = (cullModei, (int)Cull.CULL_DISABLED);
        actions.RenderModeValues["cull_front"]    = (cullModei, (int)Cull.CULL_FRONT);
        actions.RenderModeValues["cull_back"]     = (cullModei, (int)Cull.CULL_BACK);

        actions.RenderModeFlags["unshaded"]        = this.Unshaded;
        actions.RenderModeFlags["wireframe"]       = this.Wireframe;
        actions.RenderModeFlags["particle_trails"] = this.UsesParticleTrails;

        actions.UsageFlagPointers["ALPHA"]                   = this.UsesAlpha;
        actions.UsageFlagPointers["ALPHA_SCISSOR_THRESHOLD"] = this.UsesAlphaClip;
        // Use alpha clip pipeline for alpha hash/dither.
        // This prevents sorting issues inherent to alpha blending and allows such materials to cast shadows.
        actions.UsageFlagPointers["ALPHA_HASH_SCALE"]  = this.UsesAlphaClip;
        actions.RenderModeFlags["depth_prepass_alpha"] = this.UsesDepthPrepassAlpha;

        actions.UsageFlagPointers["SSS_STRENGTH"]            = this.UsesSss;
        actions.UsageFlagPointers["SSS_TRANSMITTANCE_DEPTH"] = this.UsesTransmittance;

        actions.UsageFlagPointers["DISCARD"]    = this.UsesDiscard;
        actions.UsageFlagPointers["TIME"]       = this.UsesTime;
        actions.UsageFlagPointers["ROUGHNESS"]  = this.UsesRoughness;
        actions.UsageFlagPointers["NORMAL"]     = this.UsesNormal;
        actions.UsageFlagPointers["NORMAL_MAP"] = this.UsesNormal;

        actions.UsageFlagPointers["POINT_SIZE"]  = this.UsesPointSize;
        actions.UsageFlagPointers["POINT_COORD"] = this.UsesPointSize;

        actions.WriteFlagPointers["MODELVIEW_MATRIX"]  = this.WritesModelviewOrProjection;
        actions.WriteFlagPointers["PROJECTION_MATRIX"] = this.WritesModelviewOrProjection;
        actions.WriteFlagPointers["VERTEX"]            = this.UsesVertex;
        actions.WriteFlagPointers["POSITION"]          = this.UsesPosition;

        actions.UsageFlagPointers["TANGENT"]      = this.UsesTangent;
        actions.UsageFlagPointers["BINORMAL"]     = this.UsesTangent;
        actions.UsageFlagPointers["COLOR"]        = this.UsesColor;
        actions.UsageFlagPointers["UV"]           = this.UsesUv;
        actions.UsageFlagPointers["UV2"]          = this.UsesUv2;
        actions.UsageFlagPointers["CUSTOM0"]      = this.UsesCustom0;
        actions.UsageFlagPointers["CUSTOM1"]      = this.UsesCustom1;
        actions.UsageFlagPointers["CUSTOM2"]      = this.UsesCustom2;
        actions.UsageFlagPointers["CUSTOM3"]      = this.UsesCustom3;
        actions.UsageFlagPointers["BONE_INDICES"] = this.UsesBones;
        actions.UsageFlagPointers["BONE_WEIGHTS"] = this.UsesWeights;

        actions.Uniforms = this.Uniforms;

        var err = MaterialStorage.Singleton.Shaders.CompilerScene.Compile(RS.ShaderMode.SHADER_SPATIAL, code, actions, this.Path!, out var genCode);
        ERR_FAIL_COND_MSG(err != Error.OK, "Shader compilation failed.");

        if (this.Version == default)
        {
            this.Version = MaterialStorage.Singleton.Shaders.SceneShader.VersionCreate();
        }

        this.DepthDraw                = (DepthDrawKind)depthDrawi;
        this.DepthTest                = (DepthTestKind)depthTesti;
        this.CullMode                 = (Cull)cullModei;
        this.BlendMode                = (BlendModeKind)blendModei;
        this.AlphaAntialiasingMode    = (AlphaAntiAliasing)alphaAntialiasingModei;
        this.VertexInputMask          =  Convert.ToUInt32(this.UsesNormal);
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesTangent) << 1;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesColor) << 2;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesUv) << 3;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesUv2) << 4;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesCustom0) << 5;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesCustom1) << 6;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesCustom2) << 7;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesCustom3) << 8;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesBones) << 9;
        this.VertexInputMask          |= Convert.ToUInt32(this.UsesWeights) << 10;
        this.UsesScreenTextureMipmaps = genCode.UsesScreenTextureMipmaps;
        this.UsesScreenTexture        = genCode.UsesScreenTexture;
        this.UsesDepthTexture         = genCode.UsesDepthTexture;
        this.UsesNormalTexture        = genCode.UsesNormalRoughnessTexture;
        this.UsesVertexTime           = genCode.UsesVertexTime;
        this.UsesFragmentTime         = genCode.UsesFragmentTime;

        #if DEBUG
        if (this.UsesParticleTrails)
        {
            WARN_PRINT_ONCE_ED("Particle trails are only available when using the Forward+ or Mobile rendering backends.", ref this.warnPrintOnce);
        }

        if (this.UsesSss)
        {
            WARN_PRINT_ONCE_ED("Sub-surface scattering is only available when using the Forward+ rendering backend.", ref this.warnPrintOnce);
        }

        if (this.UsesTransmittance)
        {
            WARN_PRINT_ONCE_ED("Transmittance is only available when using the Forward+ rendering backend.", ref this.warnPrintOnce);
        }

        if (this.UsesScreenTexture)
        {
            WARN_PRINT_ONCE_ED("Reading from the screen texture is not supported when using the GL Compatibility backend yet. Support will be added in a future release.", ref this.warnPrintOnce);
        }

        if (this.UsesDepthTexture)
        {
            WARN_PRINT_ONCE_ED("Reading from the depth texture is not supported when using the GL Compatibility backend yet. Support will be added in a future release.", ref this.warnPrintOnce);
        }

        if (this.UsesNormalTexture)
        {
            WARN_PRINT_ONCE_ED("Reading from the normal-roughness texture is only available when using the Forward+ or Mobile rendering backends.", ref this.warnPrintOnce);
        }
        #endif

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

        var texture_uniform_names = new List<string>(genCode.TextureUniforms.Count);

        for (var i = 0; i < genCode.TextureUniforms.Count; i++)
        {
            texture_uniform_names.Add(genCode.TextureUniforms[i].Name!);
        }

        MaterialStorage.Singleton.Shaders.SceneShader.VersionSetCode(
            this.Version,
            genCode.Code,
            genCode.Uniforms,
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_VERTEX],
            genCode.StageGlobals[(int)ShaderCompiler.Stage.STAGE_FRAGMENT],
            genCode.Defines,
            texture_uniform_names
        );

        if (ERR_FAIL_COND(!MaterialStorage.Singleton.Shaders.SceneShader.VersionIsValid(this.Version)))
        {
            return;
        }

        this.UboSize         = genCode.UniformTotalSize;
        this.UboOffsets      = genCode.UniformOffsets;
        this.TextureUniforms = genCode.TextureUniforms;

        // if any form of Alpha Antialiasing is enabled, set the blend mode to alpha to coverage
        if (this.AlphaAntialiasingMode != AlphaAntiAliasing.ALPHA_ANTIALIASING_OFF)
        {
            this.BlendMode = BlendModeKind.BLEND_MODE_ALPHA_TO_COVERAGE;
        }

        this.Valid = true;
    }
}
