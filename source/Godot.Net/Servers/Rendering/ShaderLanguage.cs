namespace Godot.Net.Servers.Rendering;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Godot.Net.Core.Config;
using Godot.Net.Core.Error;
using Godot.Net.Core.OS;
using Godot.Net.Extensions;

#pragma warning disable IDE0051, IDE0052, IDE0044, IDE0044, IDE0060, CS0169, CS0414, CS0649, CA1822 // TODO Remove

public partial class ShaderLanguage
{
    private const ContextFlag KCF_DATATYPE         = ContextFlag.CF_BLOCK | ContextFlag.CF_GLOBAL_SPACE | ContextFlag.CF_DATATYPE | ContextFlag.CF_FUNC_DECL_PARAM_TYPE | ContextFlag.CF_UNIFORM_TYPE;
    private const ContextFlag KCF_SAMPLER_DATATYPE = ContextFlag.CF_FUNC_DECL_PARAM_TYPE | ContextFlag.CF_UNIFORM_TYPE;

    public delegate DataType GlobalShaderUniformGetTypeFunc(string name);

    private const int MAX_INSTANCE_UNIFORM_INDICES = 16;

    private static readonly BuiltinFuncConstArgs[] builtinFuncConstArgs =
    {
        new("textureGather", 2, 0, 3),
        new(null, 0, 0, 0)
    };

    private static readonly BuiltinFuncOutArgs[] builtinFuncOutArgs =
    {
        new("modf",         new[] { 1, -1 }),
        new("umulExtended", new[] { 2,  3 }),
        new("imulExtended", new[] { 2,  3 }),
        new("uaddCarry",    new[] { 2, -1 }),
        new("usubBorrow",   new[] { 2, -1 }),
        new("ldexp",        new[] { 1, -1 }),
        new("frexp",        new[] { 1, -1 }),
        new(null,           new[] { 0, -1 }),
    };

    private static readonly int[] cardinalityTable =
    {
        0,
        1,
        2,
        3,
        4,
        1,
        2,
        3,
        4,
        1,
        2,
        3,
        4,
        1,
        2,
        3,
        4,
        4,
        9,
        16,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
    };

    private static readonly Dictionary<string, KeyWord> keywordList = new()
    {
        { "true",  new(TokenType.TK_TRUE,  "true",  ContextFlag.CF_BLOCK | ContextFlag.CF_IF_DECL | ContextFlag.CF_BOOLEAN) },
        { "false", new(TokenType.TK_FALSE, "false", ContextFlag.CF_BLOCK | ContextFlag.CF_IF_DECL | ContextFlag.CF_BOOLEAN) },

        // data types

        { "void",             new(TokenType.TK_TYPE_VOID,             "void",             ContextFlag.CF_GLOBAL_SPACE) },
        { "bool",             new(TokenType.TK_TYPE_BOOL,             "bool",             KCF_DATATYPE) },
        { "bvec2",            new(TokenType.TK_TYPE_BVEC2,            "bvec2",            KCF_DATATYPE) },
        { "bvec3",            new(TokenType.TK_TYPE_BVEC3,            "bvec3",            KCF_DATATYPE) },
        { "bvec4",            new(TokenType.TK_TYPE_BVEC4,            "bvec4",            KCF_DATATYPE) },
        { "int",              new(TokenType.TK_TYPE_INT,              "int",              KCF_DATATYPE) },
        { "ivec2",            new(TokenType.TK_TYPE_IVEC2,            "ivec2",            KCF_DATATYPE) },
        { "ivec3",            new(TokenType.TK_TYPE_IVEC3,            "ivec3",            KCF_DATATYPE) },
        { "ivec4",            new(TokenType.TK_TYPE_IVEC4,            "ivec4",            KCF_DATATYPE) },
        { "uint",             new(TokenType.TK_TYPE_UINT,             "uint",             KCF_DATATYPE) },
        { "uvec2",            new(TokenType.TK_TYPE_UVEC2,            "uvec2",            KCF_DATATYPE) },
        { "uvec3",            new(TokenType.TK_TYPE_UVEC3,            "uvec3",            KCF_DATATYPE) },
        { "uvec4",            new(TokenType.TK_TYPE_UVEC4,            "uvec4",            KCF_DATATYPE) },
        { "float",            new(TokenType.TK_TYPE_FLOAT,            "float",            KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "vec2",             new(TokenType.TK_TYPE_VEC2,             "vec2",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "vec3",             new(TokenType.TK_TYPE_VEC3,             "vec3",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "vec4",             new(TokenType.TK_TYPE_VEC4,             "vec4",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "mat2",             new(TokenType.TK_TYPE_MAT2,             "mat2",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "mat3",             new(TokenType.TK_TYPE_MAT3,             "mat3",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "mat4",             new(TokenType.TK_TYPE_MAT4,             "mat4",             KCF_DATATYPE | ContextFlag.CF_VARYING_TYPE) },
        { "sampler2D",        new(TokenType.TK_TYPE_SAMPLER2D,        "sampler2D",        KCF_SAMPLER_DATATYPE) },
        { "isampler2D",       new(TokenType.TK_TYPE_ISAMPLER2D,       "isampler2D",       KCF_SAMPLER_DATATYPE) },
        { "usampler2D",       new(TokenType.TK_TYPE_USAMPLER2D,       "usampler2D",       KCF_SAMPLER_DATATYPE) },
        { "sampler2DArray",   new(TokenType.TK_TYPE_SAMPLER2DARRAY,   "sampler2DArray",   KCF_SAMPLER_DATATYPE) },
        { "isampler2DArray",  new(TokenType.TK_TYPE_ISAMPLER2DARRAY,  "isampler2DArray",  KCF_SAMPLER_DATATYPE) },
        { "usampler2DArray",  new(TokenType.TK_TYPE_USAMPLER2DARRAY,  "usampler2DArray",  KCF_SAMPLER_DATATYPE) },
        { "sampler3D",        new(TokenType.TK_TYPE_SAMPLER3D,        "sampler3D",        KCF_SAMPLER_DATATYPE) },
        { "isampler3D",       new(TokenType.TK_TYPE_ISAMPLER3D,       "isampler3D",       KCF_SAMPLER_DATATYPE) },
        { "usampler3D",       new(TokenType.TK_TYPE_USAMPLER3D,       "usampler3D",       KCF_SAMPLER_DATATYPE) },
        { "samplerCube",      new(TokenType.TK_TYPE_SAMPLERCUBE,      "samplerCube",      KCF_SAMPLER_DATATYPE) },
        { "samplerCubeArray", new(TokenType.TK_TYPE_SAMPLERCUBEARRAY, "samplerCubeArray", KCF_SAMPLER_DATATYPE) },

        // interpolation qualifiers

        { "flat",   new(TokenType.TK_INTERPOLATION_FLAT,   "flat",   ContextFlag.CF_INTERPOLATION_QUALIFIER) },
        { "smooth", new(TokenType.TK_INTERPOLATION_SMOOTH, "smooth", ContextFlag.CF_INTERPOLATION_QUALIFIER) },

        // precision modifiers

        { "lowp",    new(TokenType.TK_PRECISION_LOW, "lowp",    ContextFlag.CF_BLOCK | ContextFlag.CF_PRECISION_MODIFIER) },
        { "mediump", new(TokenType.TK_PRECISION_MID, "mediump", ContextFlag.CF_BLOCK | ContextFlag.CF_PRECISION_MODIFIER) },
        { "highp",   new(TokenType.TK_PRECISION_HIGH, "highp",  ContextFlag.CF_BLOCK | ContextFlag.CF_PRECISION_MODIFIER) },

        // global space keywords

        { "uniform",        new(TokenType.TK_UNIFORM,       "uniform",        ContextFlag.CF_GLOBAL_SPACE | ContextFlag.CF_UNIFORM_KEYWORD) },
        { "group_uniforms", new(TokenType.TK_UNIFORM_GROUP, "group_uniforms", ContextFlag.CF_GLOBAL_SPACE) },
        { "varying",        new(TokenType.TK_VARYING,       "varying",        ContextFlag.CF_GLOBAL_SPACE, new[] { "particles", "sky", "fog" }) },
        { "const",          new(TokenType.TK_CONST,         "const",          ContextFlag.CF_BLOCK | ContextFlag.CF_GLOBAL_SPACE | ContextFlag.CF_CONST_KEYWORD) },
        { "struct",         new(TokenType.TK_STRUCT,        "struct",         ContextFlag.CF_GLOBAL_SPACE) },
        { "shader_type",    new(TokenType.TK_SHADER_TYPE,   "shader_type",    ContextFlag.CF_SHADER_TYPE) },
        { "render_mode",    new(TokenType.TK_RENDER_MODE,   "render_mode",    ContextFlag.CF_GLOBAL_SPACE) },

        // uniform qualifiers

        { "instance", new(TokenType.TK_INSTANCE, "instance", ContextFlag.CF_GLOBAL_SPACE | ContextFlag.CF_UNIFORM_QUALIFIER) },
        { "global",   new(TokenType.TK_GLOBAL,   "global",   ContextFlag.CF_GLOBAL_SPACE | ContextFlag.CF_UNIFORM_QUALIFIER) },

        // block keywords

        { "if",       new(TokenType.TK_CF_IF,       "if",       ContextFlag.CF_BLOCK) },
        { "else",     new(TokenType.TK_CF_ELSE,     "else",     ContextFlag.CF_BLOCK) },
        { "for",      new(TokenType.TK_CF_FOR,      "for",      ContextFlag.CF_BLOCK) },
        { "while",    new(TokenType.TK_CF_WHILE,    "while",    ContextFlag.CF_BLOCK) },
        { "do",       new(TokenType.TK_CF_DO,       "do",       ContextFlag.CF_BLOCK) },
        { "switch",   new(TokenType.TK_CF_SWITCH,   "switch",   ContextFlag.CF_BLOCK) },
        { "case",     new(TokenType.TK_CF_CASE,     "case",     ContextFlag.CF_BLOCK) },
        { "default",  new(TokenType.TK_CF_DEFAULT,  "default",  ContextFlag.CF_BLOCK) },
        { "break",    new(TokenType.TK_CF_BREAK,    "break",    ContextFlag.CF_BLOCK) },
        { "continue", new(TokenType.TK_CF_CONTINUE, "continue", ContextFlag.CF_BLOCK) },
        { "return",   new(TokenType.TK_CF_RETURN,   "return",   ContextFlag.CF_BLOCK) },
        { "discard",  new(TokenType.TK_CF_DISCARD,  "discard",  ContextFlag.CF_BLOCK, new[] { "particles", "sky", "fog" }, new[] { "fragment" } ) },

        // function specifier keywords

        { "in",    new(TokenType.TK_ARG_IN,    "in",    ContextFlag.CF_FUNC_DECL_PARAM_SPEC) },
        { "out",   new(TokenType.TK_ARG_OUT,   "out",   ContextFlag.CF_FUNC_DECL_PARAM_SPEC) },
        { "inout", new(TokenType.TK_ARG_INOUT, "inout", ContextFlag.CF_FUNC_DECL_PARAM_SPEC) },

        // hints

        { "source_color",   new(TokenType.TK_HINT_SOURCE_COLOR,   "source_color",   ContextFlag.CF_UNSPECIFIED) },
        { "hint_range",     new(TokenType.TK_HINT_RANGE,          "hint_range",     ContextFlag.CF_UNSPECIFIED) },
        { "instance_index", new(TokenType.TK_HINT_INSTANCE_INDEX, "instance_index", ContextFlag.CF_UNSPECIFIED) },

        // sampler hints

        { "hint_normal",                   new(TokenType.TK_HINT_NORMAL_TEXTURE,              "hint_normal",                   ContextFlag.CF_UNSPECIFIED) },
        { "hint_default_white",            new(TokenType.TK_HINT_DEFAULT_WHITE_TEXTURE,       "hint_default_white",            ContextFlag.CF_UNSPECIFIED) },
        { "hint_default_black",            new(TokenType.TK_HINT_DEFAULT_BLACK_TEXTURE,       "hint_default_black",            ContextFlag.CF_UNSPECIFIED) },
        { "hint_default_transparent",      new(TokenType.TK_HINT_DEFAULT_TRANSPARENT_TEXTURE, "hint_default_transparent",      ContextFlag.CF_UNSPECIFIED) },
        { "hint_anisotropy",               new(TokenType.TK_HINT_ANISOTROPY_TEXTURE,          "hint_anisotropy",               ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_r",              new(TokenType.TK_HINT_ROUGHNESS_R,                 "hint_roughness_r",              ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_g",              new(TokenType.TK_HINT_ROUGHNESS_G,                 "hint_roughness_g",              ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_b",              new(TokenType.TK_HINT_ROUGHNESS_B,                 "hint_roughness_b",              ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_a",              new(TokenType.TK_HINT_ROUGHNESS_A,                 "hint_roughness_a",              ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_normal",         new(TokenType.TK_HINT_ROUGHNESS_NORMAL_TEXTURE,    "hint_roughness_normal",         ContextFlag.CF_UNSPECIFIED) },
        { "hint_roughness_gray",           new(TokenType.TK_HINT_ROUGHNESS_GRAY,              "hint_roughness_gray",           ContextFlag.CF_UNSPECIFIED) },
        { "hint_screen_texture",           new(TokenType.TK_HINT_SCREEN_TEXTURE,              "hint_screen_texture",           ContextFlag.CF_UNSPECIFIED) },
        { "hint_normal_roughness_texture", new(TokenType.TK_HINT_NORMAL_ROUGHNESS_TEXTURE,    "hint_normal_roughness_texture", ContextFlag.CF_UNSPECIFIED) },
        { "hint_depth_texture",            new(TokenType.TK_HINT_DEPTH_TEXTURE,               "hint_depth_texture",            ContextFlag.CF_UNSPECIFIED) },

        { "filter_nearest",                    new(TokenType.TK_FILTER_NEAREST,                    "filter_nearest",                    ContextFlag.CF_UNSPECIFIED) },
        { "filter_linear",                     new(TokenType.TK_FILTER_LINEAR,                     "filter_linear",                     ContextFlag.CF_UNSPECIFIED) },
        { "filter_nearest_mipmap",             new(TokenType.TK_FILTER_NEAREST_MIPMAP,             "filter_nearest_mipmap",             ContextFlag.CF_UNSPECIFIED) },
        { "filter_linear_mipmap",              new(TokenType.TK_FILTER_LINEAR_MIPMAP,              "filter_linear_mipmap",              ContextFlag.CF_UNSPECIFIED) },
        { "filter_nearest_mipmap_anisotropic", new(TokenType.TK_FILTER_NEAREST_MIPMAP_ANISOTROPIC, "filter_nearest_mipmap_anisotropic", ContextFlag.CF_UNSPECIFIED) },
        { "filter_linear_mipmap_anisotropic",  new(TokenType.TK_FILTER_LINEAR_MIPMAP_ANISOTROPIC,  "filter_linear_mipmap_anisotropic",  ContextFlag.CF_UNSPECIFIED) },
        { "repeat_enable",                     new(TokenType.TK_REPEAT_ENABLE,                     "repeat_enable",                     ContextFlag.CF_UNSPECIFIED) },
        { "repeat_disable",                    new(TokenType.TK_REPEAT_DISABLE,                    "repeat_disable",                    ContextFlag.CF_UNSPECIFIED) },
    };

    private static readonly string[] opNames = new string[(int)Operator.OP_MAX]
        {
            "==",
            "!=",
            "<",
            "<=",
            ">",
            ">=",
            "&&",
            "||",
            "!",
            "-",
            "+",
            "-",
            "*",
            "/",
            "%",
            "<<",
            ">>",
            "=",
            "+=",
            "-=",
            "*=",
            "/=",
            "%=",
            "<<=",
            ">>=",
            "&=",
            "|=",
            "^=",
            "&",
            "|",
            "^",
            "~",
            "++",
            "--",
            "?",
            ":",
            "++",
            "--",
            "()",
            "construct",
            "index",
            "empty",
            ""
        };

    private static readonly DataType[] scalarTypes =
    {
        DataType.TYPE_VOID,
        DataType.TYPE_BOOL,
        DataType.TYPE_BOOL,
        DataType.TYPE_BOOL,
        DataType.TYPE_BOOL,
        DataType.TYPE_INT,
        DataType.TYPE_INT,
        DataType.TYPE_INT,
        DataType.TYPE_INT,
        DataType.TYPE_UINT,
        DataType.TYPE_UINT,
        DataType.TYPE_UINT,
        DataType.TYPE_UINT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_INT,
        DataType.TYPE_UINT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_INT,
        DataType.TYPE_UINT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_INT,
        DataType.TYPE_UINT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_FLOAT,
        DataType.TYPE_VOID,
    };

    private static readonly bool[,]  suffixLut  = new bool[(int)Case.CASE_MAX, 127];
    private static readonly string[] tokenNames = new string[(int)TokenType.TK_MAX];

    private static bool isuffixLutInitialized;

    #if DEBUG
    public record Usage
    {
        public int  DeclLine { get; set; } = -1;
        public bool Used     { get; set; }

        public Usage(int declLine, bool used = default)
        {
            this.DeclLine = declLine;
            this.Used     = used;
        }

        public Usage(uint declLine, bool used = default) : this((int)declLine, used)
        { }
    }

    private readonly Dictionary<string, Usage>                                            usedConstants     = new();
    private readonly Dictionary<string, Usage>                                            usedFunctions     = new();
    private readonly Dictionary<string, Dictionary<string, Usage>>                        usedLocalVars     = new();
    private readonly Dictionary<string, Usage>                                            usedStructs       = new();
    private readonly Dictionary<string, Usage>                                            usedUniforms      = new();
    private readonly Dictionary<string, Usage>                                            usedVaryings      = new();
    private List<ShaderWarning>                                                           warnings          = new();
    private Dictionary<ShaderWarning.Code, Dictionary<string, Usage>>                     warningsCheckMap  = new();
    private Dictionary<ShaderWarning.Code, Dictionary<string, Dictionary<string, Usage>>> warningsCheckMap2 = new();

    private bool                    checkWarnings;
    private ContextFlag             keywordCompletionContext;
    private ShaderWarning.CodeFlags warningFlags;
    #endif // DEBUG_ENABLED

    private int                               charIdx;
    private string?                           code;
    private int                               completionArgument;
    private DataType                          completionBase;
    private bool                              completionBaseArray;
    private BlockNode?                        completionBlock;
    private SubClassTag                       completionClass;
    private string?                           completionFunction;
    private int                               completionLine;
    private string?                           completionStruct;
    private CompletionType                    completionType;
    private string?                           currentFunction;
    private TextureFilter                     currentUniformFilter = TextureFilter.FILTER_DEFAULT;
    private string?                           currentUniformGroupName;
    private ShaderNode.Uniform.HintKind       currentUniformHint = ShaderNode.Uniform.HintKind.HINT_NONE;
    private bool                              currentUniformInstanceIndexDefined;
    private TextureRepeat                     currentUniformRepeat = TextureRepeat.REPEAT_DEFAULT;
    private string?                           currentUniformSubgroupName;
    private int                               errorLine;
    private bool                              errorSet;
    private string?                           errorStr;
    private GlobalShaderUniformGetTypeFunc?   globalShaderUniformGetTypeFunc;
    private List<FilePosition>                includePositions = new();
    private bool                              isConstDecl;
    private bool                              isConstSuffixLutInitialized;
    private bool                              isShaderInc;
    private string?                           lastName;
    private IdentifierType                    lastType = IdentifierType.IDENTIFIER_MAX;
    private Node?                             nodes;
    private ShaderNode?                       shader;
    private string?                           shaderTypeIdentifier;
    private Dictionary<string, FunctionInfo>? stages;
    private int                               tkLine;
    private VaryingFunctionNames              varyingFunctionNames = new();

    #if DEBUG
    static ShaderLanguage() =>
        Debug.Assert(sizeof(int) * cardinalityTable.Length / sizeof(int) == (int)DataType.TYPE_MAX);
    #endif

    public ShaderLanguage()
    {
        this.completionClass = SubClassTag.TAG_GLOBAL;

        #if DEBUG
        this.warningsCheckMap.Add(ShaderWarning.Code.UNUSED_CONSTANT, this.usedConstants);
        this.warningsCheckMap.Add(ShaderWarning.Code.UNUSED_FUNCTION, this.usedFunctions);
        this.warningsCheckMap.Add(ShaderWarning.Code.UNUSED_STRUCT,   this.usedStructs);
        this.warningsCheckMap.Add(ShaderWarning.Code.UNUSED_UNIFORM,  this.usedUniforms);
        this.warningsCheckMap.Add(ShaderWarning.Code.UNUSED_VARYING,  this.usedVaryings);

        this.warningsCheckMap2.Add(ShaderWarning.Code.UNUSED_LOCAL_VARIABLE, this.usedLocalVars);
        #endif // DEBUG_ENABLED
    }

    // static int _get_first_ident_pos(string &p_code)
    private static int GetFirstIdentPos(string code)
    {
        var idx = 0;

        char getChar(int index) => (idx + index < code.Length) ? code[idx + index] : default;

        while (true)
        {
            if (getChar(0) == '/' && getChar(1) == '/')
            {
                idx += 2;
                while (true)
                {
                    if (getChar(0) == 0)
                    {
                        return 0;
                    }
                    if (getChar(0) == '\n')
                    {
                        idx++;
                        break; // loop
                    }
                    idx++;
                }
            }
            else if (getChar(0) == '/' && getChar(1) == '*')
            {
                idx += 2;
                while (true)
                {
                    if (getChar(0) == 0)
                    {
                        return 0;
                    }
                    if (getChar(0) == '*' && getChar(1) == '/')
                    {
                        idx += 2;
                        break; // loop
                    }
                    idx++;
                }
            }
            else
            {
                switch (getChar(0))
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        {
                            idx++;
                        }
                        break; // switch
                    default:
                        return idx;
                }
            }
        }
    }

    public static void GetBuiltinFuncs(out List<string> keywords)
    {
        var kws = new HashSet<string>();

        var idx = 0;

        while (builtinFuncDefs[idx].Name != null)
        {
            kws.Add(builtinFuncDefs[idx].Name!);

            idx++;
        }

        keywords = kws.ToList();
    }

    public static object ConstantValueToVariant(List<ConstantNode.ValueUnion> value, DataType type, int arraySize, ShaderNode.Uniform.HintKind hint = ShaderNode.Uniform.HintKind.HINT_NONE) => throw new NotImplementedException();

    public static bool ConvertConstant(ConstantNode constant, DataType toType) =>
        ConvertConstant(constant, toType, default(List<ConstantNode.ValueUnion>));

    public static bool ConvertConstant(ConstantNode constant, DataType toType, ConstantNode.ValueUnion value) =>
        ConvertConstant(constant, toType, new List<ConstantNode.ValueUnion>() { value });

    public static bool ConvertConstant(ConstantNode constant, DataType toType, List<ConstantNode.ValueUnion>? values)
    {
        if (constant.Datatype == toType)
        {
            if (values != null)
            {
                for (var i = 0; i < constant.Values.Count; i++)
                {
                    if (i < values.Count)
                    {
                        values[i] = constant.Values[i];
                    }
                    else
                    {
                        values.Add(constant.Values[i]);
                    }
                }
            }

            return true;
        }

        var value = values?.FirstOrDefault();

        if (constant.Datatype == DataType.TYPE_INT && toType == DataType.TYPE_FLOAT)
        {
            if (value != null)
            {
                value.Real = constant.Values[0].Sint;
            }

            return true;
        }
        else if (constant.Datatype == DataType.TYPE_UINT && toType == DataType.TYPE_FLOAT)
        {
            if (value != null)
            {
                value.Real = constant.Values[0].Uint;
            }

            return true;
        }
        else if (constant.Datatype == DataType.TYPE_INT && toType == DataType.TYPE_UINT)
        {
            if (constant.Values[0].Sint < 0)
            {
                return false;
            }

            if (value != null)
            {
                value.Uint = (uint)constant.Values[0].Sint;
            }

            return true;
        }
        else if (constant.Datatype == DataType.TYPE_UINT && toType == DataType.TYPE_INT)
        {
            if (constant.Values[0].Uint > 0x7FFFFFFF)
            {
                return false;
            }

            if (value != null)
            {
                value.Sint = (int)constant.Values[0].Uint;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsBuiltinFuncOutParameter(string name, int param)
    {
        var i = 0;
        while (!string.IsNullOrEmpty(builtinFuncOutArgs[i].Name))
        {
            if (name == builtinFuncOutArgs[i].Name)
            {
                for (var j = 0; j < BuiltinFuncOutArgs.MAX_ARGS; j++)
                {
                    var arg = builtinFuncOutArgs[i].Arguments[j];

                    if (arg == param)
                    {
                        return true;
                    }

                    if (arg < 0)
                    {
                        return false;
                    }
                }
            }
            i++;
        }
        return false;
    }

    public static int GetCardinality(DataType type) => cardinalityTable[(int)type];

    public static string GetDatatypeName(DataType type) =>
        type switch
        {
            DataType.TYPE_VOID             => "void",
            DataType.TYPE_BOOL             => "bool",
            DataType.TYPE_BVEC2            => "bvec2",
            DataType.TYPE_BVEC3            => "bvec3",
            DataType.TYPE_BVEC4            => "bvec4",
            DataType.TYPE_INT              => "int",
            DataType.TYPE_IVEC2            => "ivec2",
            DataType.TYPE_IVEC3            => "ivec3",
            DataType.TYPE_IVEC4            => "ivec4",
            DataType.TYPE_UINT             => "uint",
            DataType.TYPE_UVEC2            => "uvec2",
            DataType.TYPE_UVEC3            => "uvec3",
            DataType.TYPE_UVEC4            => "uvec4",
            DataType.TYPE_FLOAT            => "float",
            DataType.TYPE_VEC2             => "vec2",
            DataType.TYPE_VEC3             => "vec3",
            DataType.TYPE_VEC4             => "vec4",
            DataType.TYPE_MAT2             => "mat2",
            DataType.TYPE_MAT3             => "mat3",
            DataType.TYPE_MAT4             => "mat4",
            DataType.TYPE_SAMPLER2D        => "sampler2D",
            DataType.TYPE_ISAMPLER2D       => "isampler2D",
            DataType.TYPE_USAMPLER2D       => "usampler2D",
            DataType.TYPE_SAMPLER2DARRAY   => "sampler2DArray",
            DataType.TYPE_ISAMPLER2DARRAY  => "isampler2DArray",
            DataType.TYPE_USAMPLER2DARRAY  => "usampler2DArray",
            DataType.TYPE_SAMPLER3D        => "sampler3D",
            DataType.TYPE_ISAMPLER3D       => "isampler3D",
            DataType.TYPE_USAMPLER3D       => "usampler3D",
            DataType.TYPE_SAMPLERCUBE      => "samplerCube",
            DataType.TYPE_SAMPLERCUBEARRAY => "samplerCubeArray",
            DataType.TYPE_STRUCT           => "struct",
            DataType.TYPE_MAX              => "invalid",
            _                              => "",
        };

    public static uint GetDatatypeSize(DataType type) =>
        type switch
        {
            DataType.TYPE_VOID             => 0U,
            DataType.TYPE_BOOL             => 4U,
            DataType.TYPE_BVEC2            => 8U,
            DataType.TYPE_BVEC3            => 12U,
            DataType.TYPE_BVEC4            => 16U,
            DataType.TYPE_INT              => 4U,
            DataType.TYPE_IVEC2            => 8U,
            DataType.TYPE_IVEC3            => 12U,
            DataType.TYPE_IVEC4            => 16U,
            DataType.TYPE_UINT             => 4U,
            DataType.TYPE_UVEC2            => 8U,
            DataType.TYPE_UVEC3            => 12U,
            DataType.TYPE_UVEC4            => 16U,
            DataType.TYPE_FLOAT            => 4U,
            DataType.TYPE_VEC2             => 8U,
            DataType.TYPE_VEC3             => 12U,
            DataType.TYPE_VEC4             => 16U,
            DataType.TYPE_MAT2             => 32U,// 4 * 4 + 4 * 4
            DataType.TYPE_MAT3             => 48U,// 4 * 4 + 4 * 4 + 4 * 4
            DataType.TYPE_MAT4             => 64U,
            DataType.TYPE_SAMPLER2D        => 16U,
            DataType.TYPE_ISAMPLER2D       => 16U,
            DataType.TYPE_USAMPLER2D       => 16U,
            DataType.TYPE_SAMPLER2DARRAY   => 16U,
            DataType.TYPE_ISAMPLER2DARRAY  => 16U,
            DataType.TYPE_USAMPLER2DARRAY  => 16U,
            DataType.TYPE_SAMPLER3D        => 16U,
            DataType.TYPE_ISAMPLER3D       => 16U,
            DataType.TYPE_USAMPLER3D       => 16U,
            DataType.TYPE_SAMPLERCUBE      => 16U,
            DataType.TYPE_SAMPLERCUBEARRAY => 16U,
            DataType.TYPE_STRUCT           => 0U,
            _                              => ERR_FAIL_V(0U),
        };

    public static string GetInterpolationName(DataInterpolation interpolation) => throw new NotImplementedException();
    public static void GetKeywordList(List<string> keywords) => throw new NotImplementedException();

    public static string GetOperatorText(Operator op) => opNames[(int)op];

    public static string GetPrecisionName(DataPrecision type) => throw new NotImplementedException();

    public static DataType GetScalarType(DataType type) => scalarTypes[(int)type];

    // string ShaderLanguage.get_shader_type(string &p_code)
    public static string? GetShaderType(string code)
    {
        var readingType = false;

        var curIdentifier = default(string);

        for (var i = GetFirstIdentPos(code); i < code.Length; i++)
        {
            if (code[i] == ';')
            {
                break;
            }
            else if (code[i] <= 32)
            {
                if (!string.IsNullOrEmpty(curIdentifier))
                {
                    if (!readingType)
                    {
                        if (curIdentifier != "shader_type")
                        {
                            return null;
                        }

                        readingType   = true;
                        curIdentifier = null;
                    }
                    else
                    {
                        return curIdentifier;
                    }
                }
            }
            else
            {
                curIdentifier += code[i];
            }
        }

        return readingType ? curIdentifier : null;
    }

    public static string GetTextureFilterName(TextureFilter filter) => throw new NotImplementedException();
    public static string GetTextureRepeatName(TextureRepeat repeat) => throw new NotImplementedException();
    public static DataType GetTokenDatatype(TokenType type) => (DataType)((int)type - (int)TokenType.TK_TYPE_VOID);
    public static DataInterpolation GetTokenInterpolation(TokenType type) => throw new NotImplementedException();
    public static DataPrecision GetTokenPrecision(TokenType type) => throw new NotImplementedException();
    public static string GetTokenText(Token token) => throw new NotImplementedException();
    public static string GetUniformHintName(ShaderNode.Uniform.HintKind hint) => throw new NotImplementedException();

    public static bool HasBuiltin(Dictionary<string, FunctionInfo> functions, string name) =>
        functions.Any(x => x.Value.BuiltIns.ContainsKey(name));

    public static bool IsControlFlowKeyword(string keyword) => throw new NotImplementedException();

    public static bool IsFloatType(DataType type) =>
        type                         is
        DataType.TYPE_FLOAT          or
        DataType.TYPE_VEC2           or
        DataType.TYPE_VEC3           or
        DataType.TYPE_VEC4           or
        DataType.TYPE_MAT2           or
        DataType.TYPE_MAT3           or
        DataType.TYPE_MAT4           or
        DataType.TYPE_SAMPLER2D      or
        DataType.TYPE_SAMPLER2DARRAY or
        DataType.TYPE_SAMPLER3D      or
        DataType.TYPE_SAMPLERCUBE    or
        DataType.TYPE_SAMPLERCUBEARRAY;

    public static bool IsSamplerType(DataType type) =>
        type > DataType.TYPE_MAT4 && type < DataType.TYPE_STRUCT;

    public static bool IsScalarType(DataType type) => throw new NotImplementedException();
    public static bool IsTokenArgQual(TokenType type) => throw new NotImplementedException();

    public static bool IsTokenDatatype(TokenType type) =>
        type == TokenType.TK_TYPE_VOID ||
        type == TokenType.TK_TYPE_BOOL ||
        type == TokenType.TK_TYPE_BVEC2 ||
        type == TokenType.TK_TYPE_BVEC3 ||
        type == TokenType.TK_TYPE_BVEC4 ||
        type == TokenType.TK_TYPE_INT ||
        type == TokenType.TK_TYPE_IVEC2 ||
        type == TokenType.TK_TYPE_IVEC3 ||
        type == TokenType.TK_TYPE_IVEC4 ||
        type == TokenType.TK_TYPE_UINT ||
        type == TokenType.TK_TYPE_UVEC2 ||
        type == TokenType.TK_TYPE_UVEC3 ||
        type == TokenType.TK_TYPE_UVEC4 ||
        type == TokenType.TK_TYPE_FLOAT ||
        type == TokenType.TK_TYPE_VEC2 ||
        type == TokenType.TK_TYPE_VEC3 ||
        type == TokenType.TK_TYPE_VEC4 ||
        type == TokenType.TK_TYPE_MAT2 ||
        type == TokenType.TK_TYPE_MAT3 ||
        type == TokenType.TK_TYPE_MAT4 ||
        type == TokenType.TK_TYPE_SAMPLER2D ||
        type == TokenType.TK_TYPE_ISAMPLER2D ||
        type == TokenType.TK_TYPE_USAMPLER2D ||
        type == TokenType.TK_TYPE_SAMPLER2DARRAY ||
        type == TokenType.TK_TYPE_ISAMPLER2DARRAY ||
        type == TokenType.TK_TYPE_USAMPLER2DARRAY ||
        type == TokenType.TK_TYPE_SAMPLER3D ||
        type == TokenType.TK_TYPE_ISAMPLER3D ||
        type == TokenType.TK_TYPE_USAMPLER3D ||
        type == TokenType.TK_TYPE_SAMPLERCUBE ||
        type == TokenType.TK_TYPE_SAMPLERCUBEARRAY;

    public static bool IsTokenHint(TokenType type) =>
        type > TokenType.TK_RENDER_MODE && type < TokenType.TK_SHADER_TYPE;

    public static bool IsTokenInterpolation(TokenType type) =>
        type == TokenType.TK_INTERPOLATION_FLAT ||
        type == TokenType.TK_INTERPOLATION_SMOOTH;

    public static bool IsTokenNonvoidDatatype(TokenType type) =>
        IsTokenDatatype(type) && type != TokenType.TK_TYPE_VOID;

    public static bool IsTokenOperator(TokenType type) =>
        type == TokenType.TK_OP_EQUAL ||
        type == TokenType.TK_OP_NOT_EQUAL ||
        type == TokenType.TK_OP_LESS ||
        type == TokenType.TK_OP_LESS_EQUAL ||
        type == TokenType.TK_OP_GREATER ||
        type == TokenType.TK_OP_GREATER_EQUAL ||
        type == TokenType.TK_OP_AND ||
        type == TokenType.TK_OP_OR ||
        type == TokenType.TK_OP_NOT ||
        type == TokenType.TK_OP_ADD ||
        type == TokenType.TK_OP_SUB ||
        type == TokenType.TK_OP_MUL ||
        type == TokenType.TK_OP_DIV ||
        type == TokenType.TK_OP_MOD ||
        type == TokenType.TK_OP_SHIFT_LEFT ||
        type == TokenType.TK_OP_SHIFT_RIGHT ||
        type == TokenType.TK_OP_ASSIGN ||
        type == TokenType.TK_OP_ASSIGN_ADD ||
        type == TokenType.TK_OP_ASSIGN_SUB ||
        type == TokenType.TK_OP_ASSIGN_MUL ||
        type == TokenType.TK_OP_ASSIGN_DIV ||
        type == TokenType.TK_OP_ASSIGN_MOD ||
        type == TokenType.TK_OP_ASSIGN_SHIFT_LEFT ||
        type == TokenType.TK_OP_ASSIGN_SHIFT_RIGHT ||
        type == TokenType.TK_OP_ASSIGN_BIT_AND ||
        type == TokenType.TK_OP_ASSIGN_BIT_OR ||
        type == TokenType.TK_OP_ASSIGN_BIT_XOR ||
        type == TokenType.TK_OP_BIT_AND ||
        type == TokenType.TK_OP_BIT_OR ||
        type == TokenType.TK_OP_BIT_XOR ||
        type == TokenType.TK_OP_BIT_INVERT ||
        type == TokenType.TK_OP_INCREMENT ||
        type == TokenType.TK_OP_DECREMENT ||
        type == TokenType.TK_QUESTION ||
        type == TokenType.TK_COLON;

    public static bool IsTokenOperatorAssign(TokenType type) => throw new NotImplementedException();

    public static bool IsTokenPrecision(TokenType type) =>
        type == TokenType.TK_PRECISION_LOW ||
        type == TokenType.TK_PRECISION_MID ||
        type == TokenType.TK_PRECISION_HIGH;

    public static bool IsTokenVariableDatatype(TokenType type) =>
        type == TokenType.TK_TYPE_VOID ||
        type == TokenType.TK_TYPE_BOOL ||
        type == TokenType.TK_TYPE_BVEC2 ||
        type == TokenType.TK_TYPE_BVEC3 ||
        type == TokenType.TK_TYPE_BVEC4 ||
        type == TokenType.TK_TYPE_INT ||
        type == TokenType.TK_TYPE_IVEC2 ||
        type == TokenType.TK_TYPE_IVEC3 ||
        type == TokenType.TK_TYPE_IVEC4 ||
        type == TokenType.TK_TYPE_UINT ||
        type == TokenType.TK_TYPE_UVEC2 ||
        type == TokenType.TK_TYPE_UVEC3 ||
        type == TokenType.TK_TYPE_UVEC4 ||
        type == TokenType.TK_TYPE_FLOAT ||
        type == TokenType.TK_TYPE_VEC2 ||
        type == TokenType.TK_TYPE_VEC3 ||
        type == TokenType.TK_TYPE_VEC4 ||
        type == TokenType.TK_TYPE_MAT2 ||
        type == TokenType.TK_TYPE_MAT3 ||
        type == TokenType.TK_TYPE_MAT4;

    public static PropertyInfo UniformToPropertyInfo(ShaderNode.Uniform uniform) => throw new NotImplementedException();

    #region private methods
    #if DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool HAS_WARNING(ShaderWarning.CodeFlags flag) => flag.HasFlag(this.warningFlags);
    private void AddGlobalWarning(ShaderWarning.Code code, string subject = "", object[]? extraArgs = default) => throw new NotImplementedException();
    private void AddLineWarning(ShaderWarning.Code code, string subject = "", object[]? extraArgs = default) => throw new NotImplementedException();
    private void AddWarning(ShaderWarning.Code code, int line, string subject = "", object[]? extraArgs = default) => throw new NotImplementedException();
    private void ParseUsedIdentifier(string identifier, IdentifierType p_type, string function) => throw new NotImplementedException();
    private void CheckWarningAccums() => throw new NotImplementedException();
    #endif

    private T AllocNode<T>() where T : Node, new()
    {
        var node = new T
        {
            Next = this.nodes
        };

        this.nodes = node;
        return node;
    }

    private bool CheckNodeConstness(Node node) => throw new NotImplementedException();

    private void Clear()
    {
        this.currentFunction                    = null;
        this.lastName                           = null;
        this.lastType                           = IdentifierType.IDENTIFIER_MAX;
        this.currentUniformGroupName            = "";
        this.currentUniformSubgroupName         = "";
        this.currentUniformHint                 = ShaderNode.Uniform.HintKind.HINT_NONE;
        this.currentUniformFilter               = TextureFilter.FILTER_DEFAULT;
        this.currentUniformRepeat               = TextureRepeat.REPEAT_DEFAULT;
        this.currentUniformInstanceIndexDefined = false;

        this.completionType      = CompletionType.COMPLETION_NONE;
        this.completionBlock     = null;
        this.completionFunction  = null;
        this.completionClass     = SubClassTag.TAG_GLOBAL;
        this.completionStruct    = null;
        this.completionBase      = DataType.TYPE_VOID;
        this.completionBaseArray = false;

        this.includePositions.Clear();
        this.includePositions.Add(new());

        #if DEBUG
        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
        this.usedConstants.Clear();
        this.usedVaryings.Clear();
        this.usedUniforms.Clear();
        this.usedFunctions.Clear();
        this.usedStructs.Clear();
        this.usedLocalVars.Clear();
        this.warnings.Clear();
        #endif // DEBUG_ENABLED

        this.errorLine   = 0;
        this.tkLine      = 1;
        this.charIdx     = 0;
        this.errorSet    = false;
        this.errorStr    = "";
        this.isConstDecl = false;
        this.nodes       = null;
    }

    private bool CompareDatatypes(DataType datatypeA, string datatypeNameA, uint arraySizeA, DataType datatypeB, string datatypeNameB, uint arraySizeB)
    {
        var result = true;

        if (datatypeA == DataType.TYPE_STRUCT || datatypeB == DataType.TYPE_STRUCT)
        {
            if (datatypeNameA != datatypeNameB)
            {
                result = false;
            }
        }
        else
        {
            if (datatypeA != datatypeB)
            {
                result = false;
            }
        }

        if (arraySizeA != arraySizeB)
        {
            result = false;
        }

        if (!result)
        {
            var typeName = datatypeA == DataType.TYPE_STRUCT ? datatypeNameA : GetDatatypeName(datatypeA);

            if (arraySizeA > 0)
            {
                typeName += "[";
                typeName += arraySizeA;
                typeName += "]";
            }

            var typeName2 = datatypeB == DataType.TYPE_STRUCT ? datatypeNameB : GetDatatypeName(datatypeB);

            if (arraySizeB > 0)
            {
                typeName2 += "[";
                typeName2 += arraySizeB;
                typeName2 += "]";
            }

            this.SetError(string.Format(RTR("Invalid assignment of '{0}' to '{1}'."), typeName2, typeName));
        }

        return result;
    }

    private bool CompareDatatypesInNodes(Node a, Node b) => throw new NotImplementedException();

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out var _, out var _, out var _, out var _, out var _, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out dataType, out var _, out var _, out var _, out var _, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType, out IdentifierType type) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out dataType, out type, out var _, out var _, out var _, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType, out IdentifierType type, out bool isConst) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out dataType, out type, out isConst, out var _, out var _, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType, out IdentifierType type, out bool isConst, out uint arraySize) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out dataType, out type, out isConst, out arraySize, out var _, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType, out IdentifierType type, out bool isConst, out uint arraySize, out string? structName) =>
        this.FindIdentifier(block, allowReassign, functionInfo, identifier, out dataType, out type, out isConst, out arraySize, out structName, out var _);

    private bool FindIdentifier(BlockNode? block, bool allowReassign, FunctionInfo functionInfo, string identifier, out DataType dataType, out IdentifierType type, out bool isConst, out uint arraySize, out string? structName, out ConstantNode.ValueUnion? constantValue)
    {
        dataType      = default;
        isConst       = default;
        type          = default;
        arraySize     = default;
        structName    = default;
        constantValue = default;

        if (this.isShaderInc)
        {
            for (var i = 0; i < (int)RS.ShaderMode.SHADER_MAX; i++)
            {
                foreach (var item in ShaderTypes.Singleton.GetFunctions((RS.ShaderMode)i))
                {
                    if ((this.currentFunction == item.Key || item.Key == "global" || item.Key == "constants") && item.Value.BuiltIns.TryGetValue(identifier, out var builtin))
                    {
                        dataType = builtin.Type;
                        isConst  = builtin.Constant;
                        type     = IdentifierType.IDENTIFIER_BUILTIN_VAR;

                        return true;
                    }
                }
            }
        }
        else
        {
            if (functionInfo.BuiltIns.TryGetValue(identifier, out var builtin))
            {
                dataType = builtin.Type;
                isConst  = builtin.Constant;
                type     = IdentifierType.IDENTIFIER_BUILTIN_VAR;

                return true;
            }
        }

        if (functionInfo.StageFunctions.TryGetValue(identifier, out var stageFunction))
        {
            dataType = stageFunction.ReturnType;
            isConst  = true;
            type     = IdentifierType.IDENTIFIER_FUNCTION;

            return true;
        }

        var function = default(FunctionNode);

        while (block != null)
        {
            if (block.Variables.TryGetValue(identifier, out var variable))
            {
                dataType      = variable.Type;
                isConst       = variable.IsConst;
                arraySize     = variable.ArraySize;
                structName    = variable.StructName;
                constantValue = variable.Value;
                type          = IdentifierType.IDENTIFIER_LOCAL_VAR;

                return true;
            }

            if (block.ParentFunction != null)
            {
                function = block.ParentFunction;
                break;
            }
            else
            {
                if (allowReassign)
                {
                    break;
                }

                if (ERR_FAIL_COND_V(block.ParentBlock == null))
                {
                    return false;
                }
                block = block.ParentBlock;
            }
        }

        if (function != null)
        {
            if (function.Arguments.TryGetValue(identifier, out var argument))
            {
                dataType   = argument.Type;
                structName = argument.TypeStr;
                arraySize  = argument.ArraySize;
                isConst    = argument.IsConst;
                type       = IdentifierType.IDENTIFIER_FUNCTION_ARGUMENT;

                return true;
            }
        }

        if (this.shader!.Varyings.TryGetValue(identifier, out var varying))
        {
            dataType  = varying.Type;
            arraySize = varying.ArraySize;
            type      = IdentifierType.IDENTIFIER_VARYING;

            return true;
        }

        if (this.shader!.Uniforms.TryGetValue(identifier, out var uniform))
        {
            dataType  = uniform.Type;
            arraySize = uniform.ArraySize;
            type      = IdentifierType.IDENTIFIER_UNIFORM;

            return true;
        }

        if (this.shader.Constants.TryGetValue(identifier, out var constant))
        {
            isConst    = true;
            dataType   = constant.Type;
            arraySize  = constant.ArraySize;
            structName = constant.TypeStr;

            if ((constant!.Initializer?.Values.Count ?? 0) == 1)
            {
                constantValue = constant!.Initializer!.Values[0];
            }
            type = IdentifierType.IDENTIFIER_CONSTANT;

            return true;
        }

        if (this.shader.Functions.TryGetValue(identifier, out var fn) && !fn.Callable)
        {

            dataType  = fn.FunctionNode.ReturnType;
            arraySize = fn.FunctionNode.ReturnArraySize;
            type      = IdentifierType.IDENTIFIER_FUNCTION;

            return true;
        }

        return false;
    }

    private Error FindLastFlowOpInBlock(BlockNode block, FlowOperation op) => throw new NotImplementedException();
    private Error FindLastFlowOpInOp(ControlFlowNode flow, FlowOperation op) => throw new NotImplementedException();

    private bool GetCompletableIdentifier(BlockNode? block, CompletionType type, out string? identifier)
    {
        identifier = null;

        var pos = new TkPos(0, 0);

        var tk = this.GetToken();

        if (tk.Type == TokenType.TK_IDENTIFIER)
        {
            identifier = tk.Text;
            pos = this.GetTkpos();
            tk = this.GetToken();
        }

        if (tk.Type == TokenType.TK_CURSOR)
        {
            this.completionType  = type;
            this.completionLine  = this.tkLine;
            this.completionBlock = block;

            pos = this.GetTkpos();
            tk  = this.GetToken();

            if (tk.Type == TokenType.TK_IDENTIFIER)
            {
                identifier += tk.Text;
            }
            else
            {
                this.SetTkpos(pos);
            }
            return true;
        }
        else if (identifier != null)
        {
            this.SetTkpos(pos);
        }

        return false;
    }

    private string GetQualifierStr(ArgumentQualifier qualifier) => throw new NotImplementedException();
    private string GetShaderTypeList(HashSet<string> shaderTypes) => throw new NotImplementedException();

    private TkPos GetTkpos() => new(this.charIdx, this.tkLine);

    private Token GetToken()
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        char getChar(int idx) =>
            (this.charIdx + idx < this.code!.Length)
                ? this.code[this.charIdx + idx]
                : default;

        while (true)
        {
            this.charIdx++;
            switch (getChar(-1))
            {
                case '\0':
                    return this.MakeToken(TokenType.TK_EOF);
                case '\uffff':
                    return this.MakeToken(TokenType.TK_CURSOR); //for completion
                case '\t':
                case '\r':
                case ' ':
                    continue;
                case '\n':
                    this.tkLine++;
                    continue;
                case '/':
                    switch (getChar(0))
                    {
                        case '*':
                            // block comment
                            this.charIdx++;
                            while (true)
                            {
                                if (getChar(0) == 0)
                                {
                                    return this.MakeToken(TokenType.TK_EOF);
                                }
                                if (getChar(0) == '*' && getChar(1) == '/')
                                {
                                    this.charIdx += 2;
                                    break;
                                }
                                else if (getChar(0) == '\n')
                                {
                                    this.tkLine++;
                                }

                                this.charIdx++;
                            }
                            break;
                        case '/':
                            // line comment skip
                            while (true)
                            {
                                if (getChar(0) == '\n')
                                {
                                    this.tkLine++;
                                    this.charIdx++;
                                    break;
                                }
                                if (getChar(0) == 0)
                                {
                                    return this.MakeToken(TokenType.TK_EOF);
                                }
                                this.charIdx++;
                            }
                            break;
                        case '=':
                            // diveq
                            this.charIdx++;
                            return this.MakeToken(TokenType.TK_OP_ASSIGN_DIV);
                        default:
                            return this.MakeToken(TokenType.TK_OP_DIV);
                    }

                    continue; //a comment, continue to next token
                case '=':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_EQUAL);
                    }

                    return this.MakeToken(TokenType.TK_OP_ASSIGN);
                case '<':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_LESS_EQUAL);
                    }
                    else if (getChar(0) == '<')
                    {
                        this.charIdx++;
                        if (getChar(0) == '=')
                        {
                            this.charIdx++;
                            return this.MakeToken(TokenType.TK_OP_ASSIGN_SHIFT_LEFT);
                        }

                        return this.MakeToken(TokenType.TK_OP_SHIFT_LEFT);
                    }

                    return this.MakeToken(TokenType.TK_OP_LESS);
                case '>':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_GREATER_EQUAL);
                    }
                    else if (getChar(0) == '>')
                    {
                        this.charIdx++;
                        if (getChar(0) == '=')
                        {
                            this.charIdx++;
                            return this.MakeToken(TokenType.TK_OP_ASSIGN_SHIFT_RIGHT);
                        }

                        return this.MakeToken(TokenType.TK_OP_SHIFT_RIGHT);
                    }

                    return this.MakeToken(TokenType.TK_OP_GREATER);
                case '!':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_NOT_EQUAL);
                    }

                    return this.MakeToken(TokenType.TK_OP_NOT);

                //case '"' //string - no strings in shader
                //case '\'' //string - no strings in shader
                case '{':
                    return this.MakeToken(TokenType.TK_CURLY_BRACKET_OPEN);
                case '}':
                    return this.MakeToken(TokenType.TK_CURLY_BRACKET_CLOSE);
                case '[':
                    return this.MakeToken(TokenType.TK_BRACKET_OPEN);
                case ']':
                    return this.MakeToken(TokenType.TK_BRACKET_CLOSE);
                case '(':
                    return this.MakeToken(TokenType.TK_PARENTHESIS_OPEN);
                case ')':
                    return this.MakeToken(TokenType.TK_PARENTHESIS_CLOSE);
                case ',':
                    return this.MakeToken(TokenType.TK_COMMA);
                case ';':
                    return this.MakeToken(TokenType.TK_SEMICOLON);
                case '?':
                    return this.MakeToken(TokenType.TK_QUESTION);
                case ':':
                    return this.MakeToken(TokenType.TK_COLON);
                case '^':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_BIT_XOR);
                    }

                    return this.MakeToken(TokenType.TK_OP_BIT_XOR);
                case '~':
                    return this.MakeToken(TokenType.TK_OP_BIT_INVERT);
                case '&':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_BIT_AND);
                    }
                    else if (getChar(0) == '&')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_AND);
                    }

                    return this.MakeToken(TokenType.TK_OP_BIT_AND);
                case '|':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_BIT_OR);
                    }
                    else if (getChar(0) == '|')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_OR);
                    }

                    return this.MakeToken(TokenType.TK_OP_BIT_OR);
                case '*':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_MUL);
                    }

                    return this.MakeToken(TokenType.TK_OP_MUL);
                case '+':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_ADD);
                    }
                    else if (getChar(0) == '+')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_INCREMENT);
                    }

                    return this.MakeToken(TokenType.TK_OP_ADD);
                case '-':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_SUB);
                    }
                    else if (getChar(0) == '-')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_DECREMENT);
                    }

                    return this.MakeToken(TokenType.TK_OP_SUB);
                case '%':
                    if (getChar(0) == '=')
                    {
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_OP_ASSIGN_MOD);
                    }

                    return this.MakeToken(TokenType.TK_OP_MOD);
                case '@':
                    if (getChar(0) == '@' && getChar(1) == '>')
                    {
                        this.charIdx += 2;

                        var incp = new List<char>();
                        while (getChar(0) != '\n')
                        {
                            incp.Add(getChar(0));
                            this.charIdx++;
                        }

                        incp.Add(default); // Zero end it.
                        var includePath = string.Join("", incp);
                        this.includePositions[^1].Line = this.tkLine;
                        var fp = new FilePosition { File = includePath, Line = 0 };
                        this.tkLine = 0;
                        this.includePositions.Add(fp);

                    }
                    else if (getChar(0) == '@' && getChar(1) == '<')
                    {
                        if (this.includePositions.Count == 1)
                        {
                            return this.MakeToken(TokenType.TK_ERROR, "Invalid include exit hint @@< without matching enter hint.");
                        }
                        this.charIdx += 2;

                        this.includePositions.Capacity = this.includePositions.Count - 1; // Pop back.
                        this.tkLine = this.includePositions[^1].Line - 1; // Restore line.

                    }
                    else
                    {
                        return this.MakeToken(TokenType.TK_ERROR, "Invalid include enter/exit hint token (@@> and @@<)");
                    }
                    break;
                default:
                    this.charIdx--; //go back one, since we have no idea what this is

                    if (char.IsDigit(getChar(0)) || getChar(0) == '.' && char.IsDigit(getChar(1)))
                    {
                        // parse number
                        var hexaFound        = false;
                        var periodFound      = false;
                        var exponentFound    = false;
                        var floatSuffixFound = false;
                        var uintSuffixFound  = false;
                        var endSuffixFound   = false;

                        var lutCase = Case.CASE_ALL;

                        if (!this.isConstSuffixLutInitialized)
                        {
                            this.isConstSuffixLutInitialized = true;

                            for (var j = 0; j < 127; j++)
                            {
                                var t = (char)j;

                                suffixLut[(int)Case.CASE_ALL, j]                 = t == '.' || t == 'x' || t == 'e' || t == 'f' || t == 'u' || t == '-' || t == '+';
                                suffixLut[(int)Case.CASE_HEXA_PERIOD, j]         = t == 'e' || t == 'f';
                                suffixLut[(int)Case.CASE_EXPONENT, j]            = t == 'f' || t == '-' || t == '+';
                                suffixLut[(int)Case.CASE_SIGN_AFTER_EXPONENT, j] = t == 'f';
                                suffixLut[(int)Case.CASE_NONE, j]                = false;
                            }
                        }

                        var str           = "";
                        var i             = 0;
                        var digitAfterExp = false;

                        while (true)
                        {
                            var symbol = getChar(i).ToString().ToLower()[0];
                            var error  = false;

                            if (char.IsDigit(symbol))
                            {
                                if (exponentFound)
                                {
                                    digitAfterExp = true;
                                }
                                if (endSuffixFound)
                                {
                                    error = true;
                                }
                            }
                            else
                            {
                                if (symbol < 0x7F && suffixLut[(int)lutCase, symbol])
                                {
                                    if (symbol == 'x')
                                    {
                                        hexaFound = true;
                                        lutCase = Case.CASE_HEXA_PERIOD;
                                    }
                                    else if (symbol == '.')
                                    {
                                        periodFound = true;
                                        lutCase = Case.CASE_HEXA_PERIOD;
                                    }
                                    else if (symbol == 'e' && !hexaFound)
                                    {
                                        exponentFound = true;
                                        lutCase = Case.CASE_EXPONENT;
                                    }
                                    else if (symbol == 'f' && !hexaFound)
                                    {
                                        if (!periodFound && !exponentFound)
                                        {
                                            error = true;
                                        }
                                        floatSuffixFound = true;
                                        endSuffixFound = true;
                                        lutCase = Case.CASE_NONE;
                                    }
                                    else if (symbol == 'u')
                                    {
                                        uintSuffixFound = true;
                                        endSuffixFound = true;
                                        lutCase = Case.CASE_NONE;
                                    }
                                    else if (symbol == '-' || symbol == '+')
                                    {
                                        if (exponentFound)
                                        {
                                            lutCase = Case.CASE_SIGN_AFTER_EXPONENT;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (!hexaFound || !char.IsAsciiHexDigit(symbol))
                                {
                                    if (symbol.IsAsciiIdentifierChar())
                                    {
                                        error = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            if (error)
                            {
                                return hexaFound
                                    ? this.MakeToken(TokenType.TK_ERROR, "Invalid (hexadecimal) numeric constant")
                                    : periodFound || exponentFound || floatSuffixFound
                                        ? this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant")
                                        : uintSuffixFound
                                            ? this.MakeToken(TokenType.TK_ERROR, "Invalid (unsigned integer) numeric constant")
                                            : this.MakeToken(TokenType.TK_ERROR, "Invalid (integer) numeric constant");
                            }
                            str += symbol;
                            i++;
                        }

                        var lastChar = str[^1];

                        if (hexaFound)
                        {
                            // Integer (hex).
                            if (str.Length > 11 || !str.IsValidHexNumber(true))
                            {
                                // > 0xFFFFFFFF
                                return this.MakeToken(TokenType.TK_ERROR, "Invalid (hexadecimal) numeric constant");
                            }
                        }
                        else if (periodFound || exponentFound || floatSuffixFound)
                        {
                            // Float
                            if (exponentFound && (!digitAfterExp || !char.IsDigit(lastChar) && lastChar != 'f'))
                            {
                                // Checks for eg: "2E", "2E-", "2E+" and 0ef, 0e+f, 0.0ef, 0.0e-f (exponent without digit after it).
                                return this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant");
                            }

                            if (periodFound)
                            {
                                if (floatSuffixFound)
                                {
                                    //checks for eg "1.f" or "1.99f" notations
                                    if (lastChar != 'f')
                                    {
                                        return this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant");
                                    }
                                }
                                else
                                {
                                    //checks for eg. "1." or "1.99" notations
                                    if (lastChar != '.' && !char.IsDigit(lastChar))
                                    {
                                        return this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant");
                                    }
                                }
                            }
                            else if (floatSuffixFound)
                            {
                                // if no period found the float suffix must be the last character, like in "2f" for "2.0"
                                if (lastChar != 'f')
                                {
                                    return this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant");
                                }
                            }

                            if (floatSuffixFound)
                            {
                                // Strip the suffix.
                                str = str[..^1];
                                // Compensate reading cursor position.
                                this.charIdx += 1;
                            }

                            if (!float.TryParse(str, out var _))
                            {
                                return this.MakeToken(TokenType.TK_ERROR, "Invalid (float) numeric constant");
                            }
                        }
                        else
                        {
                            // Integer
                            if (uintSuffixFound)
                            {
                                // Strip the suffix.
                                str = str[..^1];
                                // Compensate reading cursor position.
                                this.charIdx += 1;
                            }
                            if (!int.TryParse(str, out var _))
                            {
                                return uintSuffixFound
                                    ? this.MakeToken(TokenType.TK_ERROR, "Invalid (unsigned integer) numeric constant")
                                    : this.MakeToken(TokenType.TK_ERROR, "Invalid (integer) numeric constant");
                            }
                        }

                        this.charIdx += str.Length;

                        var tk = new Token
                        {
                            Type = periodFound || exponentFound || floatSuffixFound
                                ? TokenType.TK_FLOAT_CONSTANT
                                : uintSuffixFound
                                    ? TokenType.TK_UINT_CONSTANT
                                    : TokenType.TK_INT_CONSTANT,

                            Constant = hexaFound ? str.HexToInt() : float.Parse(str, CultureInfo.InvariantCulture),
                            Line     = this.tkLine
                        };

                        return tk;
                    }

                    if (getChar(0) == '.')
                    {
                        //parse period
                        this.charIdx++;
                        return this.MakeToken(TokenType.TK_PERIOD);
                    }

                    if (getChar(0).IsAsciiIdentifierChar())
                    {
                        // parse identifier
                        var str = "";

                        while (getChar(0).IsAsciiIdentifierChar())
                        {
                            str += getChar(0);
                            this.charIdx++;
                        }

                        //see if keyword
                        if (keywordList.TryGetValue(str, out var keyword))
                        {
                            return this.MakeToken(keyword.Token);
                        }

                        str = str.Replace("dus_", "_");

                        return this.MakeToken(TokenType.TK_IDENTIFIER, str);
                    }

                    return (getChar(0) > 32)
                        ? this.MakeToken(TokenType.TK_ERROR, $"Tokenizer: Unknown character #{(int)getChar(0)}: '{getChar(0)}'")
                        : this.MakeToken(TokenType.TK_ERROR, $"Tokenizer: Unknown character #{(int)getChar(0)}");
            }
        }
    }

    private bool IsOperatorAssign(Operator op) =>
        op switch
        {
            Operator.OP_ASSIGN             or
            Operator.OP_ASSIGN_ADD         or
            Operator.OP_ASSIGN_SUB         or
            Operator.OP_ASSIGN_MUL         or
            Operator.OP_ASSIGN_DIV         or
            Operator.OP_ASSIGN_MOD         or
            Operator.OP_ASSIGN_SHIFT_LEFT  or
            Operator.OP_ASSIGN_SHIFT_RIGHT or
            Operator.OP_ASSIGN_BIT_AND     or
            Operator.OP_ASSIGN_BIT_OR      or
            Operator.OP_ASSIGN_BIT_XOR => true,
            _ => false
        };

    private bool LookupNext(out Token tk)
    {
        var prePos = this.GetTkpos();

        var line = prePos.TkLine;

        this.GetToken();

        tk = this.GetToken();

        this.SetTkpos(prePos);

        return tk.Line == line;
    }

    private Token MakeToken(TokenType type, string? text = default)
    {
        if (type == TokenType.TK_ERROR)
        {
            this.SetError(text);
        }

        return new(type, text ?? "", this.tkLine);
    }

    private Node? ParseAndReduceExpression(BlockNode? block, FunctionInfo functionInfo)
    {
        var expr = this.ParseExpression(block, functionInfo);
        if (expr == null)
        {
            //errored
            return null;
        }

        expr = this.ReduceExpression(block, expr);

        return expr;
    }

    private Error ParseArraySize(BlockNode? block, FunctionInfo functionInfo, bool forbidUnknownSize, out Node sizeExpression, out uint arraySize, out bool unknownSize) => throw new NotImplementedException();
    private Node ParseArrayConstructor(BlockNode? block, FunctionInfo functionInfo) => throw new NotImplementedException();
    private Node ParseArrayConstructor(BlockNode? block, FunctionInfo functionInfo, DataType type, string structName, uint arraySize) => throw new NotImplementedException();

    private Error ParseBlock(BlockNode block, FunctionInfo functionInfo, bool justOne = false, bool canBreak = false, bool canContinue = false)
    {
        while (true)
        {
            var pos = this.GetTkpos();
            var tk  = this.GetToken();

            if (block!.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_SWITCH)
            {
                if (tk.Type != TokenType.TK_CF_CASE && tk.Type != TokenType.TK_CF_DEFAULT && tk.Type != TokenType.TK_CURLY_BRACKET_CLOSE)
                {
                    this.SetError(string.Format(RTR("A switch may only contain '{0}' and '{1}' blocks."), "case", "default"));
                    return Error.ERR_PARSE_ERROR;
                }
            }

            var isStruct    = this.shader!.Structs.ContainsKey(tk.Text);
            var isVarInit   = false;
            var isCondition = false;

            if (tk.Type == TokenType.TK_CURLY_BRACKET_CLOSE)
            {
                //end of block
                if (justOne)
                {
                    this.SetExpectedError("}");
                    return Error.ERR_PARSE_ERROR;
                }

                return Error.OK;

            }
            else if (tk.Type == TokenType.TK_CONST || IsTokenPrecision(tk.Type) || IsTokenNonvoidDatatype(tk.Type) || isStruct)
            {
                isVarInit = true;

                var structName = "";
                if (isStruct)
                {
                    structName = tk.Text;

                    #if DEBUG
                    if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_STRUCT_FLAG) && this.usedStructs.TryGetValue(structName, out var usedStruct))
                    {
                        usedStruct.Used = true;
                    }
                    #endif // DEBUG_ENABLED
                }

                #if DEBUG
                var precisionFlag = ContextFlag.CF_PRECISION_MODIFIER;

                this.keywordCompletionContext = ContextFlag.CF_DATATYPE;
                if (!IsTokenPrecision(tk.Type))
                {
                    if (!isStruct)
                    {
                        this.keywordCompletionContext |= precisionFlag;
                    }
                }
                #endif // DEBUG_ENABLED

                var isConst = false;

                if (tk.Type == TokenType.TK_CONST)
                {
                    isConst = true;
                    tk = this.GetToken();

                    if (!isStruct)
                    {
                        isStruct = this.shader.Structs.ContainsKey(tk.Text); // check again.
                        structName = tk.Text;
                    }
                }

                var precision = DataPrecision.PRECISION_DEFAULT;
                if (IsTokenPrecision(tk.Type))
                {
                    precision = GetTokenPrecision(tk.Type);
                    tk = this.GetToken();

                    if (!isStruct)
                    {
                        isStruct = this.shader.Structs.ContainsKey(tk.Text); // check again.
                    }

                    #if DEBUG
                    if (this.keywordCompletionContext.HasFlag(precisionFlag))
                    {
                        this.keywordCompletionContext ^= precisionFlag;
                    }
                    #endif // DEBUG_ENABLED
                }

                #if DEBUG
                if (isConst && this.LookupNext(out var next))
                {
                    if (IsTokenPrecision(next.Type))
                    {
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                    }

                    if (IsTokenDatatype(next.Type))
                    {
                        this.keywordCompletionContext ^= ContextFlag.CF_DATATYPE;
                    }
                }
                #endif // DEBUG_ENABLED

                if (precision != DataPrecision.PRECISION_DEFAULT)
                {
                    if (!IsTokenNonvoidDatatype(tk.Type))
                    {
                        this.SetError(RTR("Expected variable type after precision modifier."));
                        return Error.ERR_PARSE_ERROR;
                    }
                }

                if (!isStruct)
                {
                    if (!IsTokenVariableDatatype(tk.Type))
                    {
                        this.SetError(RTR("Invalid variable type (samplers are not allowed)."));
                        return Error.ERR_PARSE_ERROR;
                    }
                }

                var type = isStruct ? DataType.TYPE_STRUCT : GetTokenDatatype(tk.Type);

                if (precision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(type, precision) != Error.OK)
                {
                    return Error.ERR_PARSE_ERROR;
                }

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                #endif // DEBUG_ENABLED

                var arraySize      = 0U;
                var fixedArraySize = false;
                var first          = true;

                var vdnode = this.AllocNode<VariableDeclarationNode>();
                vdnode.Precision = precision;

                if (isStruct)
                {
                    vdnode.StructName = structName;
                    vdnode.Datatype   = DataType.TYPE_STRUCT;
                }
                else
                {
                    vdnode.Datatype = type;
                }

                vdnode.IsConst = isConst;

                do
                {
                    var unknownSize = false;
                    var decl = new VariableDeclarationNode.Declaration();

                    tk = this.GetToken();

                    if (first)
                    {
                        first = false;

                        if (tk.Type != TokenType.TK_IDENTIFIER && tk.Type != TokenType.TK_BRACKET_OPEN)
                        {
                            this.SetError(RTR("Expected an identifier or '[' after type."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (tk.Type == TokenType.TK_BRACKET_OPEN)
                        {
                            var error = this.ParseArraySize(block, functionInfo, false, out var sizeExpression, out arraySize, out unknownSize);

                            decl.SizeExpression = sizeExpression;

                            if (error != Error.OK)
                            {
                                return error;
                            }

                            decl.Size = arraySize;

                            fixedArraySize = true;
                            tk = this.GetToken();
                        }
                    }

                    if (tk.Type != TokenType.TK_IDENTIFIER)
                    {
                        this.SetError(RTR("Expected an identifier."));
                        return Error.ERR_PARSE_ERROR;
                    }

                    var name = tk.Text;

                    if (this.FindIdentifier(block, true, functionInfo, name, out var _, out var itype))
                    {
                        if (itype != IdentifierType.IDENTIFIER_FUNCTION)
                        {
                            this.SetRedefinitionError(name);
                            return Error.ERR_PARSE_ERROR;
                        }
                    }

                    decl.Name = name;

                    #if DEBUG
                    if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_LOCAL_VARIABLE_FLAG) && block != null)
                    {
                        var parentFunction = default(FunctionNode);

                        while (block != null && block.ParentFunction == null)
                        {
                            block = block.ParentBlock!;
                        }

                        parentFunction = block!.ParentFunction;

                        if (parentFunction != null)
                        {
                            var funcName = parentFunction.Name!;

                            if (this.usedLocalVars.TryGetValue(funcName, out var fn3))
                            {
                                fn3.Add(name, new(this.tkLine));
                            }
                            else
                            {
                                this.usedLocalVars.Add(funcName, new());
                            }
                        }
                    }
                    #endif // DEBUG_ENABLED

                    this.isConstDecl = isConst;

                    var variable = new BlockNode.Variable
                    {
                        Type       = type,
                        Precision  = precision,
                        Line       = this.tkLine,
                        ArraySize  = arraySize,
                        IsConst    = isConst,
                        StructName = structName,
                    };

                    tk = this.GetToken();

                    if (tk.Type == TokenType.TK_BRACKET_OPEN)
                    {
                        var error = this.ParseArraySize(block, functionInfo, false, out var sizeExpression, out arraySize, out unknownSize);

                        decl.SizeExpression = sizeExpression;
                        variable.ArraySize  = arraySize;

                        if (error != Error.OK)
                        {
                            return error;
                        }

                        decl.Size = variable.ArraySize;
                        arraySize = variable.ArraySize;

                        tk = this.GetToken();
                    }

                    #if DEBUG
                    if (variable.Type == DataType.TYPE_BOOL)
                    {
                        this.keywordCompletionContext = ContextFlag.CF_BOOLEAN;
                    }
                    #endif // DEBUG_ENABLED

                    if (variable.ArraySize > 0 || unknownSize)
                    {
                        var fullDef = false;

                        if (tk.Type == TokenType.TK_OP_ASSIGN)
                        {
                            var prevPos = this.GetTkpos();
                            tk = this.GetToken();

                            if (tk.Type == TokenType.TK_IDENTIFIER)
                            {
                                // a function call array initialization
                                this.SetTkpos(prevPos);

                                var n = this.ParseAndReduceExpression(block, functionInfo);

                                if (n == null)
                                {
                                    this.SetError(RTR("Expected array initializer."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                                else
                                {
                                    if (unknownSize)
                                    {
                                        decl.Size          = n.GetArraySize();
                                        variable.ArraySize = n.GetArraySize();
                                    }

                                    if (!this.CompareDatatypes(variable.Type, variable.StructName, variable.ArraySize, n.GetDatatype(), n.GetDatatypeName()!, n.GetArraySize()))
                                    {
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    decl.SingleExpression = true;
                                    decl.Initializer.Add(n);
                                }

                                tk = this.GetToken();
                            }
                            else
                            {
                                if (tk.Type != TokenType.TK_CURLY_BRACKET_OPEN)
                                {
                                    if (unknownSize)
                                    {
                                        this.SetExpectedError("{");
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    fullDef = true;

                                    var precision2 = DataPrecision.PRECISION_DEFAULT;

                                    if (IsTokenPrecision(tk.Type))
                                    {
                                        precision2 = GetTokenPrecision(tk.Type);
                                        tk         = this.GetToken();

                                        if (!IsTokenNonvoidDatatype(tk.Type))
                                        {
                                            this.SetError(RTR("Expected data type after precision modifier."));
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                    }

                                    var type2       = default(DataType);
                                    var structName2 = "";

                                    if (this.shader.Structs.ContainsKey(tk.Text))
                                    {
                                        type2       = DataType.TYPE_STRUCT;
                                        structName2 = tk.Text;
                                    }
                                    else
                                    {
                                        if (!IsTokenVariableDatatype(tk.Type))
                                        {
                                            this.SetError(RTR("Invalid data type for the array."));
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        type2 = GetTokenDatatype(tk.Type);
                                    }

                                    if (precision2 != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(type2, precision2) != Error.OK)
                                    {
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    var arraySize2 = 0u;

                                    tk = this.GetToken();
                                    if (tk.Type == TokenType.TK_BRACKET_OPEN)
                                    {
                                        var error = this.ParseArraySize(block, functionInfo, false, out var _, out arraySize2, out var isUnknownSize);

                                        if (error != Error.OK)
                                        {
                                            return error;
                                        }

                                        if (isUnknownSize)
                                        {
                                            arraySize2 = variable.ArraySize;
                                        }
                                        tk = this.GetToken();
                                    }
                                    else
                                    {
                                        this.SetExpectedError("[");
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    if (precision != precision2 || type != type2 || structName != structName2 || variable.ArraySize != arraySize2)
                                    {
                                        var from = "";
                                        if (precision2 != DataPrecision.PRECISION_DEFAULT)
                                        {
                                            from += GetPrecisionName(precision2);
                                            from += " ";
                                        }
                                        if (type2 == DataType.TYPE_STRUCT)
                                        {
                                            from += structName2;
                                        }
                                        else
                                        {
                                            from += GetDatatypeName(type2);
                                        }
                                        from += "[";
                                        from += arraySize2;
                                        from += "]'";

                                        var to = "";
                                        if (precision != DataPrecision.PRECISION_DEFAULT)
                                        {
                                            to += GetPrecisionName(precision);
                                            to += " ";
                                        }
                                        if (type == DataType.TYPE_STRUCT)
                                        {
                                            to += structName;
                                        }
                                        else
                                        {
                                            to += GetDatatypeName(type);
                                        }
                                        to += "[";
                                        to += variable.ArraySize;
                                        to += "]'";

                                        this.SetError(string.Format(RTR("Cannot convert from '{0}' to '{1}'."), from, to));
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                }

                                var curly = tk.Type == TokenType.TK_CURLY_BRACKET_OPEN;

                                if (unknownSize)
                                {
                                    if (!curly)
                                    {
                                        this.SetExpectedError("{");
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                }
                                else
                                {
                                    if (fullDef)
                                    {
                                        if (curly)
                                        {
                                            this.SetExpectedError("(");
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                    }
                                }

                                if (tk.Type == TokenType.TK_PARENTHESIS_OPEN || curly)
                                {
                                    // initialization
                                    while (true)
                                    {
                                        var n = this.ParseAndReduceExpression(block, functionInfo);

                                        if (n == null)
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        if (isConst && n.Type == Node.NodeType.TYPE_OPERATOR && ((OperatorNode)n).Op == Operator.OP_CALL)
                                        {
                                            this.SetError(RTR("Expected a constant expression."));
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        if (!this.CompareDatatypes(variable.Type, structName, 0, n.GetDatatype(), n.GetDatatypeName(), 0))
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        tk = this.GetToken();
                                        if (tk.Type == TokenType.TK_COMMA)
                                        {
                                            decl.Initializer.Add(n);
                                            continue;
                                        }
                                        else if (!curly && tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
                                        {
                                            decl.Initializer.Add(n);
                                            break;
                                        }
                                        else if (curly && tk.Type == TokenType.TK_CURLY_BRACKET_CLOSE)
                                        {
                                            decl.Initializer.Add(n);
                                            break;
                                        }
                                        else
                                        {
                                            if (curly)
                                            {
                                                this.SetExpectedError("}", ",");
                                            }
                                            else
                                            {
                                                this.SetExpectedError(")", ",");
                                            }
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                    }
                                    if (unknownSize)
                                    {
                                        decl.Size = (uint)decl.Initializer.Count;
                                        variable.ArraySize = (uint)decl.Initializer.Count;
                                    }
                                    else if (decl.Initializer.Count != variable.ArraySize)
                                    {
                                        this.SetError(RTR("Array size mismatch."));
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                    tk = this.GetToken();
                                }
                            }
                        }
                        else
                        {
                            if (unknownSize)
                            {
                                this.SetError(RTR("Expected array initialization."));
                                return Error.ERR_PARSE_ERROR;
                            }
                            if (isConst)
                            {
                                this.SetError(RTR("Expected initialization of constant."));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        arraySize = variable.ArraySize;
                    }
                    else if (tk.Type == TokenType.TK_OP_ASSIGN)
                    {
                        //variable created with assignment! must parse an expression
                        var n = this.ParseAndReduceExpression(block, functionInfo);

                        if (n == null)
                        {
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (isConst && n.Type == Node.NodeType.TYPE_OPERATOR && ((OperatorNode)n).Op == Operator.OP_CALL)
                        {
                            var op = (OperatorNode)n;
                            for (var i = 1; i < op.Arguments.Count; i++)
                            {
                                if (!this.CheckNodeConstness(op.Arguments[i]))
                                {
                                    this.SetError(string.Format(RTR("Expected constant expression for argument {0} of function call after '='."), i - 1));
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }
                        }

                        if (n.Type == Node.NodeType.TYPE_CONSTANT)
                        {
                            var constNode = (ConstantNode)n;
                            if (constNode != null && constNode.Values.Count == 1)
                            {
                                variable.Value = constNode.Values[0];
                            }
                        }

                        if (!this.CompareDatatypes(variable.Type, variable.StructName, variable.ArraySize, n.GetDatatype(), n.GetDatatypeName(), n.GetArraySize()))
                        {
                            return Error.ERR_PARSE_ERROR;
                        }

                        decl.Initializer.Add(n);
                        tk = this.GetToken();
                    }
                    else
                    {
                        if (isConst)
                        {
                            this.SetError(RTR("Expected initialization of constant."));
                            return Error.ERR_PARSE_ERROR;
                        }
                    }

                    vdnode.Declarations.Add(decl);
                    block!.Variables.Add(name, variable);
                    this.isConstDecl = false;

                    if (!fixedArraySize)
                    {
                        arraySize = 0;
                    }

                    if (tk.Type == TokenType.TK_SEMICOLON)
                    {
                        break;
                    }
                    else if (tk.Type != TokenType.TK_COMMA)
                    {
                        this.SetExpectedError(",", ";");
                        return Error.ERR_PARSE_ERROR;
                    }
                }
                while (tk.Type == TokenType.TK_COMMA); //another variable

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_BLOCK;
                #endif // DEBUG_ENABLED
                block.Statements.Add(vdnode);
            }
            else if (tk.Type == TokenType.TK_CURLY_BRACKET_OPEN)
            {
                //a sub block, just because..
                var subBlock = this.AllocNode<BlockNode>();

                subBlock.ParentBlock = block;

                if (this.ParseBlock(subBlock, functionInfo, false, canBreak, canContinue) != Error.OK)
                {
                    return Error.ERR_PARSE_ERROR;
                }

                block.Statements.Add(subBlock);
            }
            else if (tk.Type == TokenType.TK_CF_IF)
            {
                //if () {}
                tk = this.GetToken();
                if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                {
                    this.SetExpectedAfterError("(", "if");
                    return Error.ERR_PARSE_ERROR;
                }

                var cf = this.AllocNode<ControlFlowNode>();
                cf.FlowOp = FlowOperation.FLOW_OP_IF;

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_IF_DECL;
                #endif // DEBUG_ENABLED

                var n = this.ParseAndReduceExpression(block, functionInfo);
                if (n == null)
                {
                    return Error.ERR_PARSE_ERROR;
                }

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_BLOCK;
                #endif // DEBUG_ENABLED

                if (n.GetDatatype() != DataType.TYPE_BOOL)
                {
                    this.SetError(RTR("Expected a boolean expression."));
                    return Error.ERR_PARSE_ERROR;
                }

                tk = this.GetToken();
                if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                {
                    this.SetExpectedError(")");
                    return Error.ERR_PARSE_ERROR;
                }

                var ifBlock = this.AllocNode<BlockNode>();
                ifBlock.ParentBlock = block;

                cf.Expressions.Add(n);
                cf.Blocks.Add(ifBlock);

                block.Statements.Add(cf);

                var err = this.ParseBlock(ifBlock, functionInfo, true, canBreak, canContinue);
                if (err != default)
                {
                    return err;
                }

                pos = this.GetTkpos();
                tk = this.GetToken();
                if (tk.Type == TokenType.TK_CF_ELSE)
                {
                    var elseBlock = this.AllocNode<BlockNode>();
                    elseBlock.ParentBlock = block;

                    cf.Blocks.Add(elseBlock);

                    err = this.ParseBlock(elseBlock, functionInfo, true, canBreak, canContinue);
                    if (err != default)
                    {
                        return err;
                    }
                }
                else
                {
                    this.SetTkpos(pos); //rollback
                }
            }
            else if (tk.Type == TokenType.TK_CF_SWITCH)
            {
                // switch() {}
                tk = this.GetToken();
                if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                {
                    this.SetExpectedAfterError("(", "switch");
                    return Error.ERR_PARSE_ERROR;
                }
                var cf = this.AllocNode<ControlFlowNode>();
                cf.FlowOp = FlowOperation.FLOW_OP_SWITCH;

                var n = this.ParseAndReduceExpression(block, functionInfo);
                if (n == null)
                {
                    return Error.ERR_PARSE_ERROR;
                }

                var switchType = n.GetDatatype();

                if (switchType != DataType.TYPE_INT && switchType != DataType.TYPE_UINT)
                {
                    this.SetError(RTR("Expected an integer expression."));
                    return Error.ERR_PARSE_ERROR;
                }
                tk = this.GetToken();
                if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                {
                    this.SetExpectedError(")");
                    return Error.ERR_PARSE_ERROR;
                }
                tk = this.GetToken();
                if (tk.Type != TokenType.TK_CURLY_BRACKET_OPEN)
                {
                    this.SetExpectedAfterError("{", "switch");
                    return Error.ERR_PARSE_ERROR;
                }
                var switchBlock = this.AllocNode<BlockNode>();

                switchBlock.BlockType   = BlockNode.BlockTypeKind.BLOCK_TYPE_SWITCH;
                switchBlock.ParentBlock = block;

                cf.Expressions.Add(n);
                cf.Blocks.Add(switchBlock);

                block.Statements.Add(cf);

                var prevType = TokenType.TK_CF_CASE;

                while (true)
                {
                    // Go-through multiple cases.

                    if (this.ParseBlock(switchBlock, functionInfo, true, true, false) != Error.OK)
                    {
                        return Error.ERR_PARSE_ERROR;
                    }

                    pos = this.GetTkpos();
                    tk = this.GetToken();

                    if (tk.Type == TokenType.TK_CF_CASE || tk.Type == TokenType.TK_CF_DEFAULT)
                    {
                        if (prevType == TokenType.TK_CF_DEFAULT)
                        {
                            if (tk.Type == TokenType.TK_CF_CASE)
                            {
                                this.SetError(RTR("Cases must be defined before default case."));
                                return Error.ERR_PARSE_ERROR;
                            }
                            else if (prevType == TokenType.TK_CF_DEFAULT)
                            {
                                this.SetError(RTR("Default case must be defined only once."));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }
                        prevType = tk.Type;
                        this.SetTkpos(pos);
                        continue;
                    }
                    else
                    {
                        var constants = new HashSet<int>();
                        // for (var i = 0; i < switchBlock.Statements.Count; i++)
                        foreach (var statement in switchBlock.Statements)
                        {
                            // Checks for duplicates.
                            var flow = statement as ControlFlowNode;
                            if (flow != null)
                            {
                                if (flow.FlowOp == FlowOperation.FLOW_OP_CASE)
                                {
                                    if (flow.Expressions[0].Type == Node.NodeType.TYPE_CONSTANT)
                                    {
                                        var cn = (ConstantNode)flow.Expressions[0];
                                        if (cn == null || cn.Values.Count == 0)
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        if (constants.Contains(cn.Values[0].Sint))
                                        {
                                            this.SetError(string.Format(RTR("Duplicated case label: {0}."), cn.Values[0].Sint));
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        constants.Add(cn.Values[0].Sint);
                                    }
                                    else if (flow.Expressions[0].Type == Node.NodeType.TYPE_VARIABLE)
                                    {
                                        var vn = flow.Expressions[0] as VariableNode;
                                        if (vn == null)
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        this.FindIdentifier(block, false, functionInfo, vn.Name!, out var _, out var _, out var _, out var _, out var _, out var v);
                                        if (constants.Contains(v!.Sint))
                                        {
                                            this.SetError(string.Format(RTR("Duplicated case label: {0}."), v.Sint));
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        constants.Add(v.Sint);
                                    }
                                }
                                else if (flow.FlowOp == FlowOperation.FLOW_OP_DEFAULT)
                                {
                                    continue;
                                }
                                else
                                {
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }
                            else
                            {
                                return Error.ERR_PARSE_ERROR;
                            }
                        }
                        break;
                    }
                }
            }
            else if (tk.Type == TokenType.TK_CF_CASE)
            {
                // case x : break; | return;

                if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_CASE)
                {
                    this.SetTkpos(pos);
                    return Error.OK;
                }

                if (block.BlockType != BlockNode.BlockTypeKind.BLOCK_TYPE_SWITCH)
                {
                    this.SetError(string.Format(RTR("'{0}' must be placed within a '{1}' block."), "case", "switch"));
                    return Error.ERR_PARSE_ERROR;
                }

                tk = this.GetToken();

                var sign = 1;

                if (tk.Type == TokenType.TK_OP_SUB)
                {
                    sign = -1;
                    tk = this.GetToken();
                }

                var n = default(Node);

                if (!tk.IsIntegerConstant)
                {
                    var correctConstantExpression = false;

                    if (tk.Type == TokenType.TK_IDENTIFIER)
                    {
                        this.FindIdentifier(block, false, functionInfo, tk.Text, out var dataType, out var _, out var isConst);
                        if (isConst)
                        {
                            if (dataType == DataType.TYPE_INT)
                            {
                                correctConstantExpression = true;
                            }
                        }
                    }
                    if (!correctConstantExpression)
                    {
                        this.SetError(RTR("Expected an integer constant."));
                        return Error.ERR_PARSE_ERROR;
                    }

                    var vn = this.AllocNode<VariableNode>();
                    vn.Name = tk.Text;
                    n = vn;
                }
                else
                {
                    var v = new ConstantNode.ValueUnion();
                    if (tk.Type == TokenType.TK_UINT_CONSTANT)
                    {
                        v.Uint = (uint)tk.Constant;
                    }
                    else
                    {
                        v.Sint = (int)tk.Constant * sign;
                    }

                    var cn = this.AllocNode<ConstantNode>();
                    cn.Values.Add(v);
                    cn.Datatype = tk.Type == TokenType.TK_UINT_CONSTANT ? DataType.TYPE_UINT : DataType.TYPE_INT;
                    n = cn;
                }

                tk = this.GetToken();

                if (tk.Type != TokenType.TK_COLON)
                {
                    this.SetExpectedError(":");
                    return Error.ERR_PARSE_ERROR;
                }

                var cf = this.AllocNode<ControlFlowNode>();
                cf.FlowOp = FlowOperation.FLOW_OP_CASE;

                var caseBlock = this.AllocNode<BlockNode>();

                caseBlock.BlockType   = BlockNode.BlockTypeKind.BLOCK_TYPE_CASE;
                caseBlock.ParentBlock = block;

                cf.Expressions.Add(n);
                cf.Blocks.Add(caseBlock);

                block.Statements.Add(cf);

                var err = this.ParseBlock(caseBlock, functionInfo, false, true, false);
                return err != default ? err : Error.OK;
            }
            else if (tk.Type == TokenType.TK_CF_DEFAULT)
            {
                if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_CASE)
                {
                    this.SetTkpos(pos);
                    return Error.OK;
                }

                if (block.BlockType != BlockNode.BlockTypeKind.BLOCK_TYPE_SWITCH)
                {
                    this.SetError(string.Format(RTR("'{0}' must be placed within a '{1}' block."), "default", "switch"));
                    return Error.ERR_PARSE_ERROR;
                }

                tk = this.GetToken();

                if (tk.Type != TokenType.TK_COLON)
                {
                    this.SetExpectedError(":");
                    return Error.ERR_PARSE_ERROR;
                }

                var cf = this.AllocNode<ControlFlowNode>();
                cf.FlowOp = FlowOperation.FLOW_OP_DEFAULT;

                var defaultBlock = this.AllocNode<BlockNode>();

                defaultBlock.BlockType   = BlockNode.BlockTypeKind.BLOCK_TYPE_DEFAULT;
                defaultBlock.ParentBlock = block;

                cf.Blocks.Add(defaultBlock);

                block.Statements.Add(cf);

                var err = this.ParseBlock(defaultBlock, functionInfo, false, true, false);
                return err != default ? err : Error.OK;
            }
            else if (tk.Type == TokenType.TK_CF_DO || tk.Type == TokenType.TK_CF_WHILE)
            {
                // do {} while()
                // while() {}
                var isDo = tk.Type == TokenType.TK_CF_DO;

                var doBlock = default(BlockNode);
                if (isDo)
                {
                    doBlock = this.AllocNode<BlockNode>();
                    doBlock.ParentBlock = block;

                    var err = this.ParseBlock(doBlock, functionInfo, true, true, true);
                    if (err != default)
                    {
                        return err;
                    }

                    tk = this.GetToken();
                    if (tk.Type != TokenType.TK_CF_WHILE)
                    {
                        this.SetExpectedAfterError("while", "do");
                        return Error.ERR_PARSE_ERROR;
                    }
                }
                tk = this.GetToken();

                if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                {
                    this.SetExpectedAfterError("(", "while");
                    return Error.ERR_PARSE_ERROR;
                }

                var cf = this.AllocNode<ControlFlowNode>();

                cf.FlowOp = isDo ? FlowOperation.FLOW_OP_DO : FlowOperation.FLOW_OP_WHILE;

                var n = this.ParseAndReduceExpression(block, functionInfo);

                if (n == null)
                {
                    return Error.ERR_PARSE_ERROR;
                }

                tk = this.GetToken();

                if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                {
                    this.SetExpectedError(")");
                    return Error.ERR_PARSE_ERROR;
                }

                if (!isDo)
                {
                    var whileBlock = this.AllocNode<BlockNode>();
                    whileBlock.ParentBlock = block;

                    cf.Expressions.Add(n);
                    cf.Blocks.Add(whileBlock);

                    block.Statements.Add(cf);

                    var err = this.ParseBlock(whileBlock, functionInfo, true, true, true);
                    if (err != default)
                    {
                        return err;
                    }
                }
                else
                {
                    cf.Expressions.Add(n);
                    cf.Blocks.Add(doBlock!);

                    block.Statements.Add(cf);

                    tk = this.GetToken();

                    if (tk.Type != TokenType.TK_SEMICOLON)
                    {
                        this.SetExpectedError(";");
                        return Error.ERR_PARSE_ERROR;
                    }
                }
            }
            else if (tk.Type == TokenType.TK_CF_FOR)
            {
                // for() {}
                tk = this.GetToken();
                if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                {
                    this.SetExpectedAfterError("(", "for");
                    return Error.ERR_PARSE_ERROR;
                }

                var cf = this.AllocNode<ControlFlowNode>();
                cf.FlowOp = FlowOperation.FLOW_OP_FOR;

                var initBlock = this.AllocNode<BlockNode>();

                initBlock.BlockType       = BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_INIT;
                initBlock.ParentBlock     = block;
                initBlock.SingleStatement = true;

                cf.Blocks.Add(initBlock);

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_DATATYPE;
                #endif // DEBUG_ENABLED

                var err = this.ParseBlock(initBlock, functionInfo, true, false, false);

                if (err != Error.OK)
                {
                    return err;
                }

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                #endif // DEBUG_ENABLED

                var conditionBlock = this.AllocNode<BlockNode>();

                conditionBlock.BlockType                 = BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_CONDITION;
                conditionBlock.ParentBlock               = initBlock;
                conditionBlock.SingleStatement           = true;
                conditionBlock.UseCommaBetweenStatements = true;

                cf.Blocks.Add(conditionBlock);

                err = this.ParseBlock(conditionBlock, functionInfo, true, false, false);
                if (err != Error.OK)
                {
                    return err;
                }

                var expressionBlock = this.AllocNode<BlockNode>();

                expressionBlock.BlockType                 = BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_EXPRESSION;
                expressionBlock.ParentBlock               = initBlock;
                expressionBlock.SingleStatement           = true;
                expressionBlock.UseCommaBetweenStatements = true;

                cf.Blocks.Add(expressionBlock);

                err = this.ParseBlock(expressionBlock, functionInfo, true, false, false);
                if (err != Error.OK)
                {
                    return err;
                }

                var forBlock = this.AllocNode<BlockNode>();

                forBlock.ParentBlock = initBlock;

                cf.Blocks.Add(forBlock);

                block.Statements.Add(cf);

                #if DEBUG
                this.keywordCompletionContext = ContextFlag.CF_BLOCK;
                #endif // DEBUG_ENABLED

                err = this.ParseBlock(forBlock, functionInfo, true, true, true);
                if (err != Error.OK)
                {
                    return err;
                }

            }
            else if (tk.Type == TokenType.TK_CF_RETURN)
            {
                //check return type
                var b = block;

                while (b != null && b.ParentFunction == null)
                {
                    b = b.ParentBlock;
                }

                if (b == null)
                {
                    this.SetParsingError();
                    return Error.ERR_BUG;
                }

                if (b.ParentFunction != null && functionInfo.MainFunction)
                {
                    this.SetError(string.Format(RTR("Using '{0}' in the '{1}' processor function is incorrect."), "return", b.ParentFunction.Name));
                    return Error.ERR_PARSE_ERROR;
                }

                var returnStructName = b!.ParentFunction!.ReturnStructName;
                var arraySizeString  = "";

                if (b.ParentFunction.ReturnArraySize > 0)
                {
                    arraySizeString = $"[{b.ParentFunction.ReturnArraySize}]";
                }

                var flow = this.AllocNode<ControlFlowNode>();
                flow.FlowOp = FlowOperation.FLOW_OP_RETURN;

                pos = this.GetTkpos();
                tk = this.GetToken();
                if (tk.Type == TokenType.TK_SEMICOLON)
                {
                    //all is good
                    if (b.ParentFunction.ReturnType != DataType.TYPE_VOID)
                    {
                        this.SetError(string.Format(RTR("Expected '{0}' with an expression of type '{1}'."), "return", (!string.IsNullOrEmpty(returnStructName) ? returnStructName : GetDatatypeName(b.ParentFunction.ReturnType)) + arraySizeString));
                        return Error.ERR_PARSE_ERROR;
                    }
                }
                else
                {
                    this.SetTkpos(pos); //rollback, wants expression

                    #if DEBUG
                    if (b.ParentFunction.ReturnType == DataType.TYPE_BOOL)
                    {
                        this.keywordCompletionContext = ContextFlag.CF_BOOLEAN;
                    }
                    #endif // DEBUG_ENABLED

                    var expr = this.ParseAndReduceExpression(block, functionInfo);
                    if (expr == null)
                    {
                        return Error.ERR_PARSE_ERROR;
                    }

                    if (b.ParentFunction.ReturnType != expr.GetDatatype() || b.ParentFunction.ReturnArraySize != expr.GetArraySize() || returnStructName != expr.GetDatatypeName())
                    {
                        this.SetError(string.Format(RTR("Expected return with an expression of type '%s'."), (!string.IsNullOrEmpty(returnStructName) ? returnStructName : GetDatatypeName(b.ParentFunction.ReturnType)) + arraySizeString));
                        return Error.ERR_PARSE_ERROR;
                    }

                    tk = this.GetToken();

                    if (tk.Type != TokenType.TK_SEMICOLON)
                    {
                        this.SetExpectedAfterError(";", "return");
                        return Error.ERR_PARSE_ERROR;
                    }

                    #if DEBUG
                    if (b.ParentFunction.ReturnType == DataType.TYPE_BOOL)
                    {
                        this.keywordCompletionContext = ContextFlag.CF_BLOCK;
                    }
                    #endif // DEBUG_ENABLED

                    flow.Expressions.Add(expr);
                }

                block.Statements.Add(flow);

                b = block;
                while (b != null)
                {
                    if (b.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_CASE || b.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_DEFAULT)
                    {
                        return Error.OK;
                    }
                    b = b.ParentBlock;
                }
            }
            else if (tk.Type == TokenType.TK_CF_DISCARD)
            {
                //check return type
                var b = block;
                while (b != null && b.ParentFunction == null)
                {
                    b = b.ParentBlock;
                }
                if (b == null)
                {
                    this.SetParsingError();
                    return Error.ERR_BUG;
                }

                if (!b!.ParentFunction!.CanDiscard)
                {
                    this.SetError(string.Format(RTR("Use of '{0}' is not allowed here."), "discard"));
                    return Error.ERR_PARSE_ERROR;
                }

                var flow = this.AllocNode<ControlFlowNode>();

                flow.FlowOp = FlowOperation.FLOW_OP_DISCARD;

                pos = this.GetTkpos();
                tk = this.GetToken();

                if (tk.Type != TokenType.TK_SEMICOLON)
                {
                    this.SetExpectedAfterError(";", "discard");
                    return Error.ERR_PARSE_ERROR;
                }

                block.Statements.Add(flow);
            }
            else if (tk.Type == TokenType.TK_CF_BREAK)
            {
                if (!canBreak)
                {
                    this.SetError(string.Format(RTR("'{0}' is not allowed outside of a loop or '{1}' statement."), "break", "switch"));
                    return Error.ERR_PARSE_ERROR;
                }

                var flow = this.AllocNode<ControlFlowNode>();
                flow.FlowOp = FlowOperation.FLOW_OP_BREAK;

                pos = this.GetTkpos();
                tk = this.GetToken();

                if (tk.Type != TokenType.TK_SEMICOLON)
                {
                    this.SetExpectedAfterError(";", "break");
                    return Error.ERR_PARSE_ERROR;
                }

                block.Statements.Add(flow);

                var b = block;

                while (b != null)
                {
                    if (b.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_CASE || b.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_DEFAULT)
                    {
                        return Error.OK;
                    }
                    b = b.ParentBlock;
                }

            }
            else if (tk.Type == TokenType.TK_CF_CONTINUE)
            {
                if (!canContinue)
                {
                    this.SetError(string.Format(RTR("'{0}' is not allowed outside of a loop."), "continue"));
                    return Error.ERR_PARSE_ERROR;
                }

                var flow = this.AllocNode<ControlFlowNode>();

                flow.FlowOp = FlowOperation.FLOW_OP_CONTINUE;

                pos = this.GetTkpos();
                tk  = this.GetToken();

                if (tk.Type != TokenType.TK_SEMICOLON)
                {
                    //all is good
                    this.SetExpectedAfterError(";", "continue");
                    return Error.ERR_PARSE_ERROR;
                }

                block.Statements.Add(flow);

            }
            else
            {
                //nothing else, so expression
                this.SetTkpos(pos); //rollback

                var expr = this.ParseAndReduceExpression(block, functionInfo);

                if (expr == null)
                {
                    return Error.ERR_PARSE_ERROR;
                }
                isCondition = expr.Type == Node.NodeType.TYPE_OPERATOR && expr.GetDatatype() == DataType.TYPE_BOOL;

                if (expr.Type == Node.NodeType.TYPE_OPERATOR)
                {
                    var op = (OperatorNode)expr;

                    if (op.Op == Operator.OP_EMPTY)
                    {
                        isVarInit = true;
                        isCondition = true;
                    }
                }

                block.Statements.Add(expr);
                tk = this.GetToken();

                if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_CONDITION)
                {
                    if (tk.Type == TokenType.TK_COMMA)
                    {
                        if (!isCondition)
                        {
                            this.SetError(RTR("The middle expression is expected to be a boolean operator."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        continue;
                    }
                    if (tk.Type != TokenType.TK_SEMICOLON)
                    {
                        this.SetExpectedError(",", ";");
                        return Error.ERR_PARSE_ERROR;
                    }
                }
                else if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_EXPRESSION)
                {
                    if (tk.Type == TokenType.TK_COMMA)
                    {
                        continue;
                    }
                    if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                    {
                        this.SetExpectedError(",", ")");
                        return Error.ERR_PARSE_ERROR;
                    }
                }
                else if (tk.Type != TokenType.TK_SEMICOLON)
                {
                    this.SetExpectedError(";");
                    return Error.ERR_PARSE_ERROR;
                }
            }

            if (block != null)
            {
                if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_INIT && !isVarInit)
                {
                    this.SetError(RTR("The left expression is expected to be a variable declaration."));
                    return Error.ERR_PARSE_ERROR;
                }
                if (block.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_CONDITION && !isCondition)
                {
                    this.SetError(RTR("The middle expression is expected to be a boolean operator."));
                    return Error.ERR_PARSE_ERROR;
                }
            }

            if (justOne)
            {
                break;
            }
        }

        return Error.OK;
    }

    private Node? ParseExpression(BlockNode? block, FunctionInfo functionInfo)
    {
        var expression = new List<Expression>();

        //Vector<TokenType> operators;

        while (true)
        {
            var expr   = default(Node);
            var prepos = this.GetTkpos();
            var tk     = this.GetToken();
            var pos    = this.GetTkpos();

            var isConst = false;

            if (tk.Type == TokenType.TK_PARENTHESIS_OPEN)
            {
                //handle subexpression

                expr = this.ParseAndReduceExpression(block, functionInfo);
                if (expr == null)
                {
                    return null;
                }

                tk = this.GetToken();

                if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                {
                    this.SetError(RTR("Expected ')' in expression."));
                    return null;
                }

            }
            else if (tk.Type == TokenType.TK_FLOAT_CONSTANT)
            {
                var constant = this.AllocNode<ConstantNode>();
                var v = new ConstantNode.ValueUnion
                {
                    Real = tk.Constant,
                };

                constant.Values.Add(v);
                constant.Datatype = DataType.TYPE_FLOAT;
                expr = constant;

            }
            else if (tk.Type == TokenType.TK_INT_CONSTANT)
            {
                var constant = this.AllocNode<ConstantNode>();

                var v = new ConstantNode.ValueUnion
                {
                    Sint = (int)tk.Constant,
                };

                constant.Values.Add(v);
                constant.Datatype = DataType.TYPE_INT;
                expr = constant;

            }
            else if (tk.Type == TokenType.TK_UINT_CONSTANT)
            {
                var constant = this.AllocNode<ConstantNode>();
                var v = new ConstantNode.ValueUnion()
                {
                    Uint = (uint)tk.Constant,
                };

                constant.Values.Add(v);
                constant.Datatype = DataType.TYPE_UINT;
                expr = constant;
            }
            else if (tk.Type == TokenType.TK_TRUE)
            {
                //handle true constant
                var constant = this.AllocNode<ConstantNode>();
                var v = new ConstantNode.ValueUnion
                {
                    Boolean = true,
                };

                constant.Values.Add(v);
                constant.Datatype = DataType.TYPE_BOOL;
                expr = constant;

            }
            else if (tk.Type == TokenType.TK_FALSE)
            {
                //handle false constant
                var constant = this.AllocNode<ConstantNode>();
                var v = new ConstantNode.ValueUnion
                {
                    Boolean = false,
                };

                constant.Values.Add(v);
                constant.Datatype = DataType.TYPE_BOOL;
                expr = constant;
            }
            else if (tk.Type == TokenType.TK_TYPE_VOID)
            {
                //make sure void is not used in expression
                this.SetError(RTR("Void value not allowed in expression."));
                return null;
            }
            else if (IsTokenNonvoidDatatype(tk.Type) || tk.Type == TokenType.TK_CURLY_BRACKET_OPEN)
            {
                if (tk.Type == TokenType.TK_CURLY_BRACKET_OPEN)
                {
                    //array constructor

                    this.SetTkpos(prepos);
                    expr = this.ParseArrayConstructor(block, functionInfo);
                }
                else
                {
                    var precision = DataPrecision.PRECISION_DEFAULT;

                    if (IsTokenPrecision(tk.Type))
                    {
                        precision = GetTokenPrecision(tk.Type);
                        tk        = this.GetToken();
                    }

                    var datatype = GetTokenDatatype(tk.Type);
                    if (precision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(datatype, precision) != Error.OK)
                    {
                        return null;
                    }

                    tk = this.GetToken();

                    if (tk.Type == TokenType.TK_BRACKET_OPEN)
                    {
                        //array constructor

                        this.SetTkpos(prepos);
                        expr = this.ParseArrayConstructor(block, functionInfo);
                    }
                    else
                    {
                        if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                        {
                            this.SetError(RTR("Expected '(' after the type name."));
                            return null;
                        }
                        //basic type constructor

                        var func = this.AllocNode<OperatorNode>();
                        func.Op = Operator.OP_CONSTRUCT;

                        if (precision != DataPrecision.PRECISION_DEFAULT)
                        {
                            func.ReturnPrecisionCache = precision;
                        }

                        var funcname  = this.AllocNode<VariableNode>();
                        funcname.Name = GetDatatypeName(datatype);
                        func.Arguments.Add(funcname);

                        var ok = this.ParseFunctionArguments(block, functionInfo, func, out var carg);

                        if (carg >= 0)
                        {
                            this.completionType     = CompletionType.COMPLETION_CALL_ARGUMENTS;
                            this.completionLine     = this.tkLine;
                            this.completionBlock    = block;
                            this.completionFunction = funcname.Name;
                            this.completionArgument = carg;
                        }

                        if (!ok)
                        {
                            return null;
                        }

                        if (!this.ValidateFunctionCall(block, functionInfo, func, out var returnCache, out var structName, out var _))
                        {
                            this.SetError(string.Format(RTR("No matching constructor found for: '{0}'."), funcname.Name));
                            return null;
                        }

                        func.ReturnCache = returnCache;
                        func.StructName  = structName!;

                        expr = this.ReduceExpression(block, func);
                    }
                }
            }
            else if (tk.Type == TokenType.TK_IDENTIFIER)
            {
                this.SetTkpos(prepos);

                var pstruct    = default(StructNode);
                var structInit = false;

                this.GetCompletableIdentifier(block, CompletionType.COMPLETION_IDENTIFIER, out var identifier);

                if (this.shader!.Structs.TryGetValue(identifier!, out var @struct))
                {
                    pstruct    = @struct.ShaderStruct;
                    structInit = true;
                }

                tk = this.GetToken();

                if (tk.Type == TokenType.TK_PARENTHESIS_OPEN)
                {
                    if (structInit)
                    {
                        //a struct constructor

                        var name = identifier;

                        var func         = this.AllocNode<OperatorNode>();
                        func.Op          = Operator.OP_STRUCT;
                        func.StructName  = name!;
                        func.ReturnCache = DataType.TYPE_STRUCT;

                        var funcname = this.AllocNode<VariableNode>();
                        funcname.Name = name!;

                        func.Arguments.Add(funcname);

                        for (var i = 0; i < pstruct!.Members.Count; i++)
                        {
                            var nexpr = default(Node);

                            if (pstruct.Members[i].ArraySize != 0)
                            {
                                nexpr = this.ParseArrayConstructor(block, functionInfo, pstruct.Members[i].GetDatatype(), pstruct.Members[i].StructName, pstruct.Members[i].ArraySize);

                                if (nexpr == null)
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                nexpr = this.ParseAndReduceExpression(block, functionInfo);

                                if (nexpr == null)
                                {
                                    return null;
                                }

                                if (!this.CompareDatatypesInNodes(pstruct.Members[i], nexpr))
                                {
                                    return null;
                                }
                            }

                            if (i + 1 < pstruct.Members.Count)
                            {
                                tk = this.GetToken();
                                if (tk.Type != TokenType.TK_COMMA)
                                {
                                    this.SetExpectedError(",");
                                    return null;
                                }
                            }

                            func.Arguments.Add(nexpr);
                        }
                        tk = this.GetToken();
                        if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                        {
                            this.SetExpectedError(")");
                            return null;
                        }

                        expr = func;

                    }
                    else
                    {
                        //a function call
                        if (block == null)
                        {
                            // Non-constructor function call in global space is forbidden.
                            if (this.isConstDecl)
                            {
                                this.SetError(RTR("Expected constant expression."));
                            }
                            return null;
                        }

                        var name = identifier!;

                        var func = this.AllocNode<OperatorNode>();
                        func.Op = Operator.OP_CALL;

                        var funcname = this.AllocNode<VariableNode>();
                        funcname.Name = name;
                        func.Arguments.Add(funcname);

                        var ok = this.ParseFunctionArguments(block, functionInfo, func, out var carg);

                        // Check if block has a variable with the same name as function to prevent shader crash.
                        var bnode = block;
                        while (bnode != null)
                        {
                            if (bnode.Variables.ContainsKey(name))
                            {
                                this.SetError(RTR("Expected a function name."));
                                return null;
                            }
                            bnode = bnode.ParentBlock;
                        }

                        // test if function was parsed first
                        if (this.shader.Functions.TryGetValue(name, out var fn))
                        {
                            if (this.shader.Functions.TryGetValue(this.currentFunction!, out var fn1))
                            {
                                fn1.UsesFunction.Add(name);
                            }
                        }

                        if (carg >= 0)
                        {
                            this.completionType     = CompletionType.COMPLETION_CALL_ARGUMENTS;
                            this.completionLine     = this.tkLine;
                            this.completionBlock    = block;
                            this.completionFunction = funcname.Name;
                            this.completionArgument = carg;
                        }

                        if (!ok)
                        {
                            return null;
                        }
                        if (!this.ValidateFunctionCall(block, functionInfo, func, out var returnCache, out var structName, out var isCustomFunc))
                        {
                            this.SetError(string.Format(RTR("No matching function found for: '{0}'."), funcname.Name));
                            return null;
                        }

                        func.ReturnCache = returnCache;
                        func.StructName  = structName!;

                        this.completionClass = SubClassTag.TAG_GLOBAL; // reset sub-class

                        if (fn != null)
                        {
                            //connect texture arguments, so we can cache in the
                            //argument what type of filter and repeat to use

                            var callFunction = fn.FunctionNode;
                            if (callFunction != null)
                            {
                                func.ReturnCache     = callFunction.GetDatatype();
                                func.StructName      = callFunction.GetDatatypeName();
                                func.ReturnArraySize = callFunction.GetArraySize();

                                //get current base function
                                var baseFunction = default(FunctionNode);
                                {
                                    var b = block;

                                    while (b != null)
                                    {
                                        if (b.ParentFunction != null)
                                        {
                                            baseFunction = b.ParentFunction;
                                            break;
                                        }
                                        else
                                        {
                                            b = b.ParentBlock;
                                        }
                                    }
                                }

                                if (ERR_FAIL_COND_V(baseFunction == null)) //bug, wtf
                                {
                                    return null;
                                }

                                foreach (var callArgument in callFunction.Arguments.Values.OrderBy(x => x.Index))
                                {
                                    var argidx = callArgument.Index + 1;
                                    if (argidx < func.Arguments.Count)
                                    {
                                        var error    = false;
                                        var n        = func.Arguments[argidx];
                                        var argQual  = callArgument.Qualifier;
                                        var isOutArg = argQual != ArgumentQualifier.ARGUMENT_QUALIFIER_IN;

                                        if (n.Type == Node.NodeType.TYPE_VARIABLE || n.Type == Node.NodeType.TYPE_ARRAY)
                                        {
                                            var varname = "";

                                            if (n.Type == Node.NodeType.TYPE_VARIABLE)
                                            {
                                                var vn = (VariableNode)n;
                                                varname = vn.Name;
                                            }
                                            else
                                            {
                                                // TYPE_ARRAY
                                                var an = (ArrayNode)n;
                                                varname = an.Name;
                                            }

                                            if (this.shader.Varyings.TryGetValue(varname!, out var varying))
                                            {
                                                switch (varying.Stage)
                                                {
                                                    case ShaderNode.Varying.StageKind.STAGE_UNKNOWN:
                                                        {
                                                            this.SetError(string.Format(RTR("Varying '{0}' must be assigned in the 'vertex' or 'fragment' function first."), varname));
                                                            return null;
                                                        }
                                                    case ShaderNode.Varying.StageKind.STAGE_VERTEX_TO_FRAGMENT_LIGHT:
                                                    case ShaderNode.Varying.StageKind.STAGE_VERTEX:
                                                        if (isOutArg && this.currentFunction != this.varyingFunctionNames.Vertex)
                                                        {
                                                            // inout/out
                                                            error = true;
                                                        }
                                                        break;
                                                    case ShaderNode.Varying.StageKind.STAGE_FRAGMENT_TO_LIGHT:
                                                    case ShaderNode.Varying.StageKind.STAGE_FRAGMENT:
                                                        if (!isOutArg)
                                                        {
                                                            if (this.currentFunction != this.varyingFunctionNames.Fragment && this.currentFunction != this.varyingFunctionNames.Light)
                                                            {
                                                                error = true;
                                                            }
                                                        }
                                                        else if (this.currentFunction != this.varyingFunctionNames.Fragment)
                                                        {
                                                            // inout/out
                                                            error = true;
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }

                                                if (error)
                                                {
                                                    this.SetError(string.Format(RTR("Varying '{0}' cannot be passed for the '{1}' parameter in that context."), varname, this.GetQualifierStr(argQual)));
                                                    return null;
                                                }
                                            }
                                        }

                                        var isConstArg = callArgument.IsConst;

                                        if (isConstArg || isOutArg)
                                        {
                                            var varname = "";

                                            if (n.Type == Node.NodeType.TYPE_CONSTANT || n.Type == Node.NodeType.TYPE_OPERATOR || n.Type == Node.NodeType.TYPE_ARRAY_CONSTRUCT)
                                            {
                                                if (!isConstArg)
                                                {
                                                    error = true;
                                                }
                                            }
                                            else if (n.Type == Node.NodeType.TYPE_ARRAY)
                                            {
                                                var an = (ArrayNode)n;
                                                if (!isConstArg && (an.CallExpression != null || an.IsConst))
                                                {
                                                    error = true;
                                                }
                                                varname = an.Name;
                                            }
                                            else if (n.Type == Node.NodeType.TYPE_VARIABLE)
                                            {
                                                var vn = (VariableNode)n;
                                                if (vn.IsConst && !isConstArg)
                                                {
                                                    error = true;
                                                }
                                                varname = vn.Name;
                                            }
                                            else if (n.Type == Node.NodeType.TYPE_MEMBER)
                                            {
                                                var mn = (MemberNode)n;
                                                if (mn.BasetypeConst && isOutArg)
                                                {
                                                    error = true;
                                                }
                                            }
                                            if (!error && varname != null)
                                            {
                                                if (this.shader.Constants.ContainsKey(varname))
                                                {
                                                    error = true;
                                                }
                                                else if (this.shader.Uniforms.ContainsKey(varname))
                                                {
                                                    error = true;
                                                }
                                                else if (functionInfo.BuiltIns.TryGetValue(varname, out var info))
                                                {
                                                    if (info.Constant)
                                                    {
                                                        error = true;
                                                    }
                                                }
                                            }

                                            if (error)
                                            {
                                                this.SetError(string.Format(RTR("A constant value cannot be passed for '{0}' parameter."), this.GetQualifierStr(argQual)));
                                                return null;
                                            }
                                        }

                                        if (IsSamplerType(callArgument.Type))
                                        {
                                            //let's see where our argument comes from
                                            if (ERR_CONTINUE(n.Type != Node.NodeType.TYPE_VARIABLE)) //bug? this should always be a variable
                                            {
                                                continue;
                                            }

                                            var vn = (VariableNode)n;

                                            var varname = vn.Name;
                                            if (this.shader.Uniforms.TryGetValue(varname, out var u))
                                            {
                                                //being sampler, this either comes from a uniform
                                                if (ERR_CONTINUE(u.Type != callArgument.Type)) //this should have been validated previously
                                                {
                                                    continue;
                                                }

                                                if (RendererCompositor.Singleton.IsXrEnabled && isCustomFunc)
                                                {
                                                    var hint = u.Hint;

                                                    if (hint == ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE || hint == ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE || hint == ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE)
                                                    {
                                                        this.SetError(RTR("Unable to pass a multiview texture sampler as a parameter to custom function. Consider to sample it in the main function and then pass the vector result to it."));
                                                        return null;
                                                    }
                                                }

                                                //propagate
                                                if (!this.PropagateFunctionCallSamplerUniformSettings(name, callArgument.Index, u.Filter, u.Repeat))
                                                {
                                                    return null;
                                                }
                                            }
                                            else if (functionInfo.BuiltIns.ContainsKey(varname))
                                            {
                                                //a built-in
                                                if (!this.PropagateFunctionCallSamplerBuiltinReference(name, callArgument.Index, varname))
                                                {
                                                    return null;
                                                }
                                            }
                                            else
                                            {
                                                //or this comes from an argument, but nothing else can be a sampler
                                                var found = false;

                                                if (baseFunction!.Arguments.TryGetValue(varname, out var baseFunctionArgument))
                                                {
                                                    if (!baseFunctionArgument.TexArgumentConnect.TryGetValue(callFunction.Name, out var texArgumentConnect))
                                                    {
                                                        baseFunctionArgument.TexArgumentConnect.Add(callFunction.Name, new());
                                                    }

                                                    texArgumentConnect!.Add(baseFunctionArgument.Index);
                                                    found = true;
                                                }

                                                ERR_CONTINUE(!found);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        expr = func;
                        #if DEBUG
                        if (this.checkWarnings)
                        {
                            var funcName = "";

                            if (block?.ParentFunction != null)
                            {
                                funcName = block.ParentFunction.Name;
                            }

                            this.ParseUsedIdentifier(name, IdentifierType.IDENTIFIER_FUNCTION, funcName);
                        }
                        #endif // DEBUG_ENABLED
                    }
                }
                else
                {
                    //an identifier

                    this.lastName = identifier;
                    this.lastType = IdentifierType.IDENTIFIER_MAX;
                    this.SetTkpos(pos);

                    var dataType   = DataType.TYPE_MAX;
                    var identType  = IdentifierType.IDENTIFIER_MAX;
                    var arraySize  = 0u;
                    var structName = "";
                    var isLocal    = false;

                    if (block?.BlockTag != SubClassTag.TAG_GLOBAL)
                    {
                        var found = false;
                        var idx   = 0;

                        while (!string.IsNullOrEmpty(builtinFuncDefs[idx].Name))
                        {
                            if (builtinFuncDefs[idx].Tag == block!.BlockTag && builtinFuncDefs[idx].Name == identifier)
                            {
                                found = true;
                                break;
                            }
                            idx++;
                        }
                        if (!found)
                        {
                            this.SetError(string.Format(RTR("Unknown identifier in expression: '{0}'."), identifier));
                            return null;
                        }
                    }
                    else
                    {
                        if (!this.FindIdentifier(block, false, functionInfo, identifier!, out dataType, out identType, out isConst, out arraySize, out structName))
                        {
                            if (identifier == "SCREEN_TEXTURE" || identifier == "DEPTH_TEXTURE" || identifier == "NORMAL_ROUGHNESS_TEXTURE")
                            {
                                var name = identifier;
                                var nameLower = name.ToLower();
                                this.SetError(string.Format(RTR("{0} has been removed in favor of using hint_{1} with a uniform.\nTo continue with minimal code changes add 'uniform sampler2D {2} : hint_{3}, filter_linear_mipmap;' near the top of your shader."), name, nameLower, name, nameLower));
                                return null;
                            }
                            this.SetError(string.Format(RTR("Unknown identifier in expression: '{0}'."), identifier));
                            return null;
                        }

                        if (this.isConstDecl && !isConst)
                        {
                            this.SetError(RTR("Expected constant expression."));
                            return null;
                        }

                        if (identType == IdentifierType.IDENTIFIER_VARYING)
                        {
                            var prevPos   = this.GetTkpos();
                            var nextToken = this.GetToken();

                            // An array of varyings.
                            if (nextToken.Type == TokenType.TK_BRACKET_OPEN)
                            {
                                this.GetToken(); // Pass constant.
                                this.GetToken(); // Pass TK_BRACKET_CLOSE.
                                nextToken = this.GetToken();
                            }

                            this.SetTkpos(prevPos);

                            var varying = this.shader.Varyings[identifier!];
                            if (IsTokenOperatorAssign(nextToken.Type))
                            {
                                if (!this.ValidateVaryingAssign(this.shader.Varyings[identifier!], out var error))
                                {
                                    this.SetError(error);
                                    return null;
                                }
                            }
                            else
                            {
                                switch (varying.Stage)
                                {
                                    case ShaderNode.Varying.StageKind.STAGE_UNKNOWN:
                                        if (varying.Type < DataType.TYPE_INT)
                                        {
                                            if (this.currentFunction == this.varyingFunctionNames.Vertex)
                                            {
                                                this.SetError(string.Format(RTR("Varying with '{0}' data type may only be used in the 'fragment' function."), GetDatatypeName(varying.Type)));
                                            }
                                            else
                                            {
                                                this.SetError(string.Format(RTR("Varying '{0}' must be assigned in the 'fragment' function first."), identifier));
                                            }
                                            return null;
                                        }
                                        break;
                                    case ShaderNode.Varying.StageKind.STAGE_VERTEX:
                                        if (this.currentFunction == this.varyingFunctionNames.Fragment || this.currentFunction == this.varyingFunctionNames.Light)
                                        {
                                            varying.Stage = ShaderNode.Varying.StageKind.STAGE_VERTEX_TO_FRAGMENT_LIGHT;
                                        }
                                        break;
                                    case ShaderNode.Varying.StageKind.STAGE_FRAGMENT:
                                        if (this.currentFunction == this.varyingFunctionNames.Light)
                                        {
                                            varying.Stage = ShaderNode.Varying.StageKind.STAGE_FRAGMENT_TO_LIGHT;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (varying.Stage != ShaderNode.Varying.StageKind.STAGE_FRAGMENT && varying.Stage != ShaderNode.Varying.StageKind.STAGE_FRAGMENT_TO_LIGHT && varying.Type < DataType.TYPE_FLOAT && varying.Interpolation != DataInterpolation.INTERPOLATION_FLAT)
                            {
                                this.SetTkpos(varying.Tkpos);
                                this.SetError(RTR("Varying with integer data type must be declared with `flat` interpolation qualifier."));
                                return null;
                            }
                        }

                        if (identType == IdentifierType.IDENTIFIER_FUNCTION)
                        {
                            this.SetError(string.Format(RTR("Can't use function as identifier: '{0}'."), identifier));
                            return null;
                        }

                        this.lastType = isConst
                            ? IdentifierType.IDENTIFIER_CONSTANT
                            : this.lastType = identType;

                        isLocal = identType == IdentifierType.IDENTIFIER_LOCAL_VAR || identType == IdentifierType.IDENTIFIER_FUNCTION_ARGUMENT;
                    }

                    var indexExpression  = default(Node);
                    var callExpression   = default(Node);
                    var assignExpression = default(Node);

                    if (arraySize > 0)
                    {
                        prepos = this.GetTkpos();
                        tk     = this.GetToken();

                        if (tk.Type == TokenType.TK_OP_ASSIGN)
                        {
                            if (isConst)
                            {
                                this.SetError(RTR("Constants cannot be modified."));
                                return null;
                            }
                            assignExpression = this.ParseArrayConstructor(block, functionInfo, dataType, structName!, arraySize);
                            if (assignExpression == null)
                            {
                                return null;
                            }
                        }
                        else if (tk.Type == TokenType.TK_PERIOD)
                        {
                            this.completionClass = SubClassTag.TAG_ARRAY;

                            if (block != null)
                            {
                                block.BlockTag = SubClassTag.TAG_ARRAY;
                            }

                            callExpression = this.ParseAndReduceExpression(block, functionInfo);

                            if (block != null)
                            {
                                block.BlockTag = SubClassTag.TAG_GLOBAL;
                            }

                            if (callExpression == null)
                            {
                                return null;
                            }
                        }
                        else if (tk.Type == TokenType.TK_BRACKET_OPEN)
                        {
                            // indexing
                            indexExpression = this.ParseAndReduceExpression(block, functionInfo);
                            if (indexExpression == null)
                            {
                                return null;
                            }

                            if (indexExpression!.GetArraySize() != 0 || indexExpression.GetDatatype() != DataType.TYPE_INT && indexExpression.GetDatatype() != DataType.TYPE_UINT)
                            {
                                this.SetError(RTR("Only integer expressions are allowed for indexing."));
                                return null;
                            }

                            if (indexExpression.Type == Node.NodeType.TYPE_CONSTANT)
                            {
                                var cnode = indexExpression as ConstantNode;
                                if (cnode != null)
                                {
                                    if (cnode.Values.Count != 0)
                                    {
                                        var value = cnode.Values[0].Sint;
                                        if (value < 0 || value >= arraySize)
                                        {
                                            this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), value, 0, arraySize - 1));
                                            return null;
                                        }
                                    }
                                }
                            }

                            tk = this.GetToken();
                            if (tk.Type != TokenType.TK_BRACKET_CLOSE)
                            {
                                this.SetExpectedError("]");
                                return null;
                            }
                        }
                        else
                        {
                            this.SetTkpos(prepos);
                        }

                        var arrname = this.AllocNode<ArrayNode>();

                        arrname.Name             = identifier!;
                        arrname.DatatypeCache    = dataType;
                        arrname.StructName       = structName!;
                        arrname.IndexExpression  = indexExpression;
                        arrname.CallExpression   = callExpression;
                        arrname.AssignExpression = assignExpression;
                        arrname.IsConst          = isConst;
                        arrname.ArraySize        = arraySize;
                        arrname.IsLocal          = isLocal;

                        expr = arrname;
                    }
                    else
                    {
                        var varname = this.AllocNode<VariableNode>();

                        varname.Name          = identifier!;
                        varname.DatatypeCache = dataType;
                        varname.IsConst       = isConst;
                        varname.StructName    = structName!;
                        varname.IsLocal       = isLocal;

                        expr = varname;
                    }

                    #if DEBUG
                    if (this.checkWarnings)
                    {
                        var funcName = "";
                        var b        = block;

                        while (b != null)
                        {
                            if (b.ParentFunction != null)
                            {
                                funcName = b.ParentFunction.Name;
                                break;
                            }
                            else
                            {
                                b = b.ParentBlock;
                            }
                        }

                        this.ParseUsedIdentifier(identifier!, identType, funcName);
                    }
                    #endif // DEBUG_ENABLED
                }
            }
            else if (tk.Type == TokenType.TK_OP_ADD)
            {
                continue; //this one does nothing
            }
            else if (tk.Type == TokenType.TK_OP_SUB || tk.Type == TokenType.TK_OP_NOT || tk.Type == TokenType.TK_OP_BIT_INVERT || tk.Type == TokenType.TK_OP_INCREMENT || tk.Type == TokenType.TK_OP_DECREMENT)
            {
                var e = new Expression
                {
                    IsOp = true,
                };

                switch (tk.Type)
                {
                    case TokenType.TK_OP_SUB:
                        e.Op = Operator.OP_NEGATE;
                        break;
                    case TokenType.TK_OP_NOT:
                        e.Op = Operator.OP_NOT;
                        break;
                    case TokenType.TK_OP_BIT_INVERT:
                        e.Op = Operator.OP_BIT_INVERT;
                        break;
                    case TokenType.TK_OP_INCREMENT:
                        e.Op = Operator.OP_INCREMENT;
                        break;
                    case TokenType.TK_OP_DECREMENT:
                        e.Op = Operator.OP_DECREMENT;
                        break;
                    default:
                        return ERR_FAIL_V(default(Node));
                }

                expression.Add(e);
                continue;
            }
            else
            {
                var valid = false;
                if (block?.BlockType == BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_EXPRESSION && tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
                {
                    valid = true;
                    this.SetTkpos(prepos);

                    var func = this.AllocNode<OperatorNode>();
                    func.Op = Operator.OP_EMPTY;
                    expr = func;
                }

                if (!valid)
                {
                    if (tk.Type != TokenType.TK_SEMICOLON)
                    {
                        this.SetError(string.Format(RTR("Expected expression, found: '{0}'."), GetTokenText(tk)));
                        return null;
                    }
                    else
                    {
                        #if DEBUG
                        if (block == null || block.BlockType != BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_INIT && block.BlockType != BlockNode.BlockTypeKind.BLOCK_TYPE_FOR_CONDITION)
                        {
                            if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.FORMATTING_ERROR_FLAG))
                            {
                                this.AddLineWarning(ShaderWarning.Code.FORMATTING_ERROR, RTR("Empty statement. Remove ';' to fix this warning."));
                            }
                        }
                        #endif // DEBUG_ENABLED

                        this.SetTkpos(prepos);

                        var func = this.AllocNode<OperatorNode>();
                        func.Op = Operator.OP_EMPTY;
                        expr = func;
                    }
                }
            }

            if (ERR_FAIL_COND_V(expr == null))
            {
                return null;
            }

            /* OK now see what's NEXT to the operator.. */

            while (true)
            {
                var pos2 = this.GetTkpos();
                tk       = this.GetToken();

                if (tk.Type == TokenType.TK_CURSOR)
                {
                    //do nothing
                }
                else if (tk.Type == TokenType.TK_PERIOD)
                {
                    #if DEBUG
                    var prevKeywordCompletionContext = this.keywordCompletionContext;
                    this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                    #endif

                    var dt = expr!.GetDatatype();
                    var st = expr.GetDatatypeName();

                    if (!expr.IsIndexed && expr.GetArraySize() > 0)
                    {
                        this.completionClass = SubClassTag.TAG_ARRAY;

                        if (block != null)
                        {
                            block.BlockTag = SubClassTag.TAG_ARRAY;
                        }

                        var callExpression = this.ParseAndReduceExpression(block, functionInfo);

                        if (block != null)
                        {
                            block.BlockTag = SubClassTag.TAG_GLOBAL;
                        }

                        if (callExpression == null)
                        {
                            return null;
                        }

                        expr = callExpression;
                        break;
                    }

                    if (this.GetCompletableIdentifier(block, dt == DataType.TYPE_STRUCT ? CompletionType.COMPLETION_STRUCT : CompletionType.COMPLETION_INDEX, out var identifier))
                    {
                        if (dt == DataType.TYPE_STRUCT)
                        {
                            this.completionStruct = st;
                        }
                        else
                        {
                            this.completionBase = dt;
                        }
                    }

                    if (identifier == null)
                    {
                        this.SetError(RTR("Expected an identifier as a member."));
                        return null;
                    }

                    var ident = identifier;

                    var ok               = true;
                    var repeated         = false;
                    var memberType       = DataType.TYPE_VOID;
                    var memberStructName = "";
                    var arraySize        = 0u;

                    var positionSymbols = new SortedSet<char>();
                    var colorSymbols    = new SortedSet<char>();
                    var textureSymbols  = new SortedSet<char>();

                    var mixError = false;

                    switch (dt)
                    {
                        case DataType.TYPE_STRUCT:
                            {
                                ok = false;
                                var memberName = ident;
                                if (this.shader!.Structs.TryGetValue(st, out var @struct))
                                {
                                    var n = @struct.ShaderStruct;
                                    foreach (var member in n!.Members)
                                    {
                                        if (member.Name == memberName)
                                        {
                                            memberType = member.Datatype;
                                            arraySize = member.ArraySize;
                                            if (memberType == DataType.TYPE_STRUCT)
                                            {
                                                memberStructName = member.StructName;
                                            }
                                            ok = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        case DataType.TYPE_BVEC2:
                        case DataType.TYPE_IVEC2:
                        case DataType.TYPE_UVEC2:
                        case DataType.TYPE_VEC2:
                            {
                                var l = ident.Length;
                                if (l == 1)
                                {
                                    memberType = dt - 1;
                                }
                                else if (l == 2)
                                {
                                    memberType = dt;
                                }
                                else if (l == 3)
                                {
                                    memberType = dt + 1;
                                }
                                else if (l == 4)
                                {
                                    memberType = dt + 2;
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }

                                var c = ident;
                                for (var i = 0; i < l; i++)
                                {
                                    switch (c[i])
                                    {
                                        case 'r':
                                        case 'g':
                                            if (positionSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!colorSymbols.Contains(c[i]))
                                            {
                                                colorSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 'x':
                                        case 'y':
                                            if (colorSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!positionSymbols.Contains(c[i]))
                                            {
                                                positionSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 's':
                                        case 't':
                                            if (colorSymbols.Count > 0 || positionSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!textureSymbols.Contains(c[i]))
                                            {
                                                textureSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        default:
                                            ok = false;
                                            break;
                                    }
                                }
                        } break;
                        case DataType.TYPE_BVEC3:
                        case DataType.TYPE_IVEC3:
                        case DataType.TYPE_UVEC3:
                        case DataType.TYPE_VEC3:
                            {
                                var l = ident.Length;
                                if (l == 1)
                                {
                                    memberType = dt - 2;
                                }
                                else if (l == 2)
                                {
                                    memberType = dt - 1;
                                }
                                else if (l == 3)
                                {
                                    memberType = dt;
                                }
                                else if (l == 4)
                                {
                                    memberType = dt + 1;
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }

                                var c = ident;
                                for (var i = 0; i < l; i++)
                                {
                                    switch (c[i])
                                    {
                                        case 'r':
                                        case 'g':
                                        case 'b':
                                            if (positionSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!colorSymbols.Contains(c[i]))
                                            {
                                                colorSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 'x':
                                        case 'y':
                                        case 'z':
                                            if (colorSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!positionSymbols.Contains(c[i]))
                                            {
                                                positionSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 's':
                                        case 't':
                                        case 'p':
                                            if (colorSymbols.Count > 0 || positionSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!textureSymbols.Contains(c[i]))
                                            {
                                                textureSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        default:
                                            ok = false;
                                            break;
                                    }
                                }

                            }
                            break;
                        case DataType.TYPE_BVEC4:
                        case DataType.TYPE_IVEC4:
                        case DataType.TYPE_UVEC4:
                        case DataType.TYPE_VEC4:
                            {
                                var l = ident.Length;
                                if (l == 1)
                                {
                                    memberType = dt - 3;
                                }
                                else if (l == 2)
                                {
                                    memberType = dt - 2;
                                }
                                else if (l == 3)
                                {
                                    memberType = dt - 1;
                                }
                                else if (l == 4)
                                {
                                    memberType = dt;
                                }
                                else
                                {
                                    ok = false;
                                    break;
                                }

                                var c = ident;
                                for (var i = 0; i < l; i++)
                                {
                                    switch (c[i])
                                    {
                                        case 'r':
                                        case 'g':
                                        case 'b':
                                        case 'a':
                                            if (positionSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!colorSymbols.Contains(c[i]))
                                            {
                                                colorSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 'x':
                                        case 'y':
                                        case 'z':
                                        case 'w':
                                            if (colorSymbols.Count > 0 || textureSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!positionSymbols.Contains(c[i]))
                                            {
                                                positionSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        case 's':
                                        case 't':
                                        case 'p':
                                        case 'q':
                                            if (colorSymbols.Count > 0 || positionSymbols.Count > 0)
                                            {
                                                mixError = true;
                                                break;
                                            }
                                            if (!textureSymbols.Contains(c[i]))
                                            {
                                                textureSymbols.Add(c[i]);
                                            }
                                            else
                                            {
                                                repeated = true;
                                            }
                                            break;
                                        default:
                                            ok = false;
                                            break;
                                    }
                                }
                            }
                            break;

                        default:
                            ok = false;
                            break;
                    }

                    if (mixError)
                    {
                        this.SetError(string.Format(RTR("Cannot combine symbols from different sets in expression '.{0}'."), ident));
                        return null;
                    }

                    if (!ok)
                    {
                        this.SetError(string.Format(RTR("Invalid member for '{0}' expression: '.{1}'."), dt == DataType.TYPE_STRUCT ? st : GetDatatypeName(dt), ident));
                        return null;
                    }

                    var mn = this.AllocNode<MemberNode>();

                    mn.Basetype               = dt;
                    mn.BasetypeConst          = isConst;
                    mn.Datatype               = memberType;
                    mn.BaseStructName         = st;
                    mn.StructName             = memberStructName;
                    mn.ArraySize              = arraySize;
                    mn.Name                   = ident;
                    mn.Owner                  = expr;
                    mn.HasSwizzlingDuplicates = repeated;

                    if (arraySize > 0)
                    {
                        var prevPos = this.GetTkpos();
                        tk = this.GetToken();
                        if (tk.Type == TokenType.TK_OP_ASSIGN)
                        {
                            if (this.lastType == IdentifierType.IDENTIFIER_CONSTANT)
                            {
                                this.SetError(RTR("Constants cannot be modified."));
                                return null;
                            }
                            var assignExpression = this.ParseArrayConstructor(block, functionInfo, memberType, memberStructName, arraySize);
                            if (assignExpression == null)
                            {
                                return null;
                            }
                            mn.AssignExpression = assignExpression;
                        }
                        else if (tk.Type == TokenType.TK_PERIOD)
                        {
                            this.completionClass = SubClassTag.TAG_ARRAY;
                            if (block != null)
                            {
                                block.BlockTag = SubClassTag.TAG_ARRAY;
                            }
                            mn.CallExpression = this.ParseAndReduceExpression(block, functionInfo);
                            if (block != null)
                            {
                                block.BlockTag = SubClassTag.TAG_GLOBAL;
                            }
                            if (mn.CallExpression == null)
                            {
                                return null;
                            }
                        }
                        else if (tk.Type == TokenType.TK_BRACKET_OPEN)
                        {
                            var indexExpression = this.ParseAndReduceExpression(block, functionInfo);
                            if (indexExpression == null)
                            {
                                return null;
                            }

                            if (indexExpression!.GetArraySize() != 0 || indexExpression.GetDatatype() != DataType.TYPE_INT && indexExpression.GetDatatype() != DataType.TYPE_UINT)
                            {
                                this.SetError(RTR("Only integer expressions are allowed for indexing."));
                                return null;
                            }

                            if (indexExpression.Type == Node.NodeType.TYPE_CONSTANT)
                            {
                                var cnode = indexExpression as ConstantNode;
                                if (cnode != null)
                                {
                                    if (cnode.Values.Count != 0)
                                    {
                                        var value = cnode.Values[0].Sint;
                                        if (value < 0 || value >= arraySize)
                                        {
                                            this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), value, 0, arraySize - 1));
                                            return null;
                                        }
                                    }
                                }
                            }

                            tk = this.GetToken();
                            if (tk.Type != TokenType.TK_BRACKET_CLOSE)
                            {
                                this.SetExpectedError("]");
                                return null;
                            }
                            mn.IndexExpression = indexExpression;
                        }
                        else
                        {
                            this.SetTkpos(prevPos);
                        }
                    }
                    expr = mn;

                    #if DEBUG
                    this.keywordCompletionContext = prevKeywordCompletionContext;
                    #endif

                    //todo
                    //member (period) has priority over any operator
                    //creates a subindexing expression in place

                    /*} else if (tk.Type==TK_BRACKET_OPEN) {
                    //todo
                    //subindexing has priority over any operator
                    //creates a subindexing expression in place

                    */
                }
                else if (tk.Type == TokenType.TK_BRACKET_OPEN)
                {
                    var index = this.ParseAndReduceExpression(block, functionInfo);

                    if (index == null)
                    {
                        return null;
                    }

                    if (index.GetArraySize() != 0 || index.GetDatatype() != DataType.TYPE_INT && index.GetDatatype() != DataType.TYPE_UINT)
                    {
                        this.SetError(RTR("Only integer expressions are allowed for indexing."));
                        return null;
                    }

                    var memberType       = DataType.TYPE_VOID;
                    var memberStructName = "";

                    if (expr!.GetArraySize() > 0)
                    {
                        if (index.Type == Node.NodeType.TYPE_CONSTANT)
                        {
                            var indexConstant = ((ConstantNode)index).Values[0].Uint;
                            if (indexConstant >= expr.GetArraySize())
                            {
                                this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), indexConstant, 0, expr.GetArraySize() - 1));
                                return null;
                            }
                        }
                        memberType = expr.GetDatatype();
                        if (memberType == DataType.TYPE_STRUCT)
                        {
                            memberStructName = expr.GetDatatypeName();
                        }
                    }
                    else
                    {
                        switch (expr.GetDatatype())
                        {
                            case DataType.TYPE_BVEC2:
                            case DataType.TYPE_VEC2:
                            case DataType.TYPE_IVEC2:
                            case DataType.TYPE_UVEC2:
                            case DataType.TYPE_MAT2:
                                if (index.Type == Node.NodeType.TYPE_CONSTANT)
                                {
                                    var indexConstant = ((ConstantNode)index).Values[0].Uint;
                                    if (indexConstant >= 2)
                                    {
                                        this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), indexConstant, 0, 1));
                                        return null;
                                    }
                                }

                                switch (expr.GetDatatype())
                                {
                                    case DataType.TYPE_BVEC2:
                                        memberType = DataType.TYPE_BOOL;
                                        break;
                                    case DataType.TYPE_VEC2:
                                        memberType = DataType.TYPE_FLOAT;
                                        break;
                                    case DataType.TYPE_IVEC2:
                                        memberType = DataType.TYPE_INT;
                                        break;
                                    case DataType.TYPE_UVEC2:
                                        memberType = DataType.TYPE_UINT;
                                        break;
                                    case DataType.TYPE_MAT2:
                                        memberType = DataType.TYPE_VEC2;
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case DataType.TYPE_BVEC3:
                            case DataType.TYPE_VEC3:
                            case DataType.TYPE_IVEC3:
                            case DataType.TYPE_UVEC3:
                            case DataType.TYPE_MAT3:
                                if (index.Type == Node.NodeType.TYPE_CONSTANT)
                                {
                                    var indexConstant = ((ConstantNode)index).Values[0].Uint;
                                    if (indexConstant >= 3)
                                    {
                                        this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), indexConstant, 0, 2));
                                        return null;
                                    }
                                }

                                switch (expr.GetDatatype())
                                {
                                    case DataType.TYPE_BVEC3:
                                        memberType = DataType.TYPE_BOOL;
                                        break;
                                    case DataType.TYPE_VEC3:
                                        memberType = DataType.TYPE_FLOAT;
                                        break;
                                    case DataType.TYPE_IVEC3:
                                        memberType = DataType.TYPE_INT;
                                        break;
                                    case DataType.TYPE_UVEC3:
                                        memberType = DataType.TYPE_UINT;
                                        break;
                                    case DataType.TYPE_MAT3:
                                        memberType = DataType.TYPE_VEC3;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case DataType.TYPE_BVEC4:
                            case DataType.TYPE_VEC4:
                            case DataType.TYPE_IVEC4:
                            case DataType.TYPE_UVEC4:
                            case DataType.TYPE_MAT4:
                                if (index.Type == Node.NodeType.TYPE_CONSTANT)
                                {
                                    var indexConstant = ((ConstantNode)index).Values[0].Uint;
                                    if (indexConstant >= 4)
                                    {
                                        this.SetError(string.Format(RTR("Index [{0}] out of range [{1}..{2}]."), indexConstant, 0, 3));
                                        return null;
                                    }
                                }

                                switch (expr.GetDatatype())
                                {
                                    case DataType.TYPE_BVEC4:
                                        memberType = DataType.TYPE_BOOL;
                                        break;
                                    case DataType.TYPE_VEC4:
                                        memberType = DataType.TYPE_FLOAT;
                                        break;
                                    case DataType.TYPE_IVEC4:
                                        memberType = DataType.TYPE_INT;
                                        break;
                                    case DataType.TYPE_UVEC4:
                                        memberType = DataType.TYPE_UINT;
                                        break;
                                    case DataType.TYPE_MAT4:
                                        memberType = DataType.TYPE_VEC4;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                {
                                    this.SetError(string.Format(RTR("An object of type '{0}' can't be indexed."), expr.GetDatatype() == DataType.TYPE_STRUCT ? expr.GetDatatypeName() : GetDatatypeName(expr.GetDatatype())));
                                    return null;
                                }
                        }
                    }

                    var op = this.AllocNode<OperatorNode>();

                    op.Op          = Operator.OP_INDEX;
                    op.ReturnCache = memberType;
                    op.StructName  = memberStructName;

                    op.Arguments.Add(expr);
                    op.Arguments.Add(index);

                    expr = op;

                    tk = this.GetToken();
                    if (tk.Type != TokenType.TK_BRACKET_CLOSE)
                    {
                        this.SetExpectedError("]");
                        return null;
                    }

                }
                else if (tk.Type == TokenType.TK_OP_INCREMENT || tk.Type == TokenType.TK_OP_DECREMENT)
                {
                    var op = this.AllocNode<OperatorNode>();
                    op.Op = tk.Type == TokenType.TK_OP_DECREMENT ? Operator.OP_POST_DECREMENT : Operator.OP_POST_INCREMENT;
                    op.Arguments.Add(expr!);

                    if (!this.ValidateOperator(op, out var returnCache, out var returnArraySize))
                    {
                        this.SetError(RTR("Invalid base type for increment/decrement operator."));
                        return null;
                    }

                    op.ReturnCache     = returnCache;
                    op.ReturnArraySize = returnArraySize;

                    if (!this.ValidateAssign(expr!, functionInfo))
                    {
                        this.SetError(RTR("Invalid use of increment/decrement operator in a constant expression."));
                        return null;
                    }
                    expr = op;
                }
                else
                {
                    this.SetTkpos(pos2);
                    break;
                }
            }

            var e1 = new Expression
            {
                IsOp = false,
                Node = expr,
            };

            expression.Add(e1);

            pos = this.GetTkpos();
            tk  = this.GetToken();

            if (IsTokenOperator(tk.Type))
            {
                var o = new Expression
                {
                    IsOp = true,
                };

                switch (tk.Type)
                {
                    case TokenType.TK_OP_EQUAL:
                        o.Op = Operator.OP_EQUAL;
                        break;
                    case TokenType.TK_OP_NOT_EQUAL:
                        o.Op = Operator.OP_NOT_EQUAL;
                        break;
                    case TokenType.TK_OP_LESS:
                        o.Op = Operator.OP_LESS;
                        break;
                    case TokenType.TK_OP_LESS_EQUAL:
                        o.Op = Operator.OP_LESS_EQUAL;
                        break;
                    case TokenType.TK_OP_GREATER:
                        o.Op = Operator.OP_GREATER;
                        break;
                    case TokenType.TK_OP_GREATER_EQUAL:
                        o.Op = Operator.OP_GREATER_EQUAL;
                        break;
                    case TokenType.TK_OP_AND:
                        o.Op = Operator.OP_AND;
                        break;
                    case TokenType.TK_OP_OR:
                        o.Op = Operator.OP_OR;
                        break;
                    case TokenType.TK_OP_ADD:
                        o.Op = Operator.OP_ADD;
                        break;
                    case TokenType.TK_OP_SUB:
                        o.Op = Operator.OP_SUB;
                        break;
                    case TokenType.TK_OP_MUL:
                        o.Op = Operator.OP_MUL;
                        break;
                    case TokenType.TK_OP_DIV:
                        o.Op = Operator.OP_DIV;
                        break;
                    case TokenType.TK_OP_MOD:
                        o.Op = Operator.OP_MOD;
                        break;
                    case TokenType.TK_OP_SHIFT_LEFT:
                        o.Op = Operator.OP_SHIFT_LEFT;
                        break;
                    case TokenType.TK_OP_SHIFT_RIGHT:
                        o.Op = Operator.OP_SHIFT_RIGHT;
                        break;
                    case TokenType.TK_OP_ASSIGN:
                        o.Op = Operator.OP_ASSIGN;
                        break;
                    case TokenType.TK_OP_ASSIGN_ADD:
                        o.Op = Operator.OP_ASSIGN_ADD;
                        break;
                    case TokenType.TK_OP_ASSIGN_SUB:
                        o.Op = Operator.OP_ASSIGN_SUB;
                        break;
                    case TokenType.TK_OP_ASSIGN_MUL:
                        o.Op = Operator.OP_ASSIGN_MUL;
                        break;
                    case TokenType.TK_OP_ASSIGN_DIV:
                        o.Op = Operator.OP_ASSIGN_DIV;
                        break;
                    case TokenType.TK_OP_ASSIGN_MOD:
                        o.Op = Operator.OP_ASSIGN_MOD;
                        break;
                    case TokenType.TK_OP_ASSIGN_SHIFT_LEFT:
                        o.Op = Operator.OP_ASSIGN_SHIFT_LEFT;
                        break;
                    case TokenType.TK_OP_ASSIGN_SHIFT_RIGHT:
                        o.Op = Operator.OP_ASSIGN_SHIFT_RIGHT;
                        break;
                    case TokenType.TK_OP_ASSIGN_BIT_AND:
                        o.Op = Operator.OP_ASSIGN_BIT_AND;
                        break;
                    case TokenType.TK_OP_ASSIGN_BIT_OR:
                        o.Op = Operator.OP_ASSIGN_BIT_OR;
                        break;
                    case TokenType.TK_OP_ASSIGN_BIT_XOR:
                        o.Op = Operator.OP_ASSIGN_BIT_XOR;
                        break;
                    case TokenType.TK_OP_BIT_AND:
                        o.Op = Operator.OP_BIT_AND;
                        break;
                    case TokenType.TK_OP_BIT_OR:
                        o.Op = Operator.OP_BIT_OR;
                        break;
                    case TokenType.TK_OP_BIT_XOR:
                        o.Op = Operator.OP_BIT_XOR;
                        break;
                    case TokenType.TK_QUESTION:
                        o.Op = Operator.OP_SELECT_IF;
                        break;
                    case TokenType.TK_COLON:
                        o.Op = Operator.OP_SELECT_ELSE;
                        break;
                    default:
                        this.SetError(string.Format(RTR("Invalid token for the operator: '{0}'."), GetTokenText(tk)));
                        return null;
                }

                expression.Add(o);

            }
            else
            {
                this.SetTkpos(pos); //something else, so rollback and end
                break;
            }
        }

        /* Reduce the set set of expressions and place them in an operator tree, respecting precedence */

        while (expression.Count > 1)
        {
            var nextOp      = -1;
            var minPriority = 0xFFFFF;
            var isUnary     = false;
            var isTernary   = false;

            for (var i = 0; i < expression.Count; i++)
            {
                if (!expression[i].IsOp)
                {
                    continue;
                }

                var unary = false;
                var ternary = false;
                var op = expression[i].Op;

                int priority;
                switch (op)
                {
                    case Operator.OP_EQUAL:
                        priority = 8;
                        break;
                    case Operator.OP_NOT_EQUAL:
                        priority = 8;
                        break;
                    case Operator.OP_LESS:
                        priority = 7;
                        break;
                    case Operator.OP_LESS_EQUAL:
                        priority = 7;
                        break;
                    case Operator.OP_GREATER:
                        priority = 7;
                        break;
                    case Operator.OP_GREATER_EQUAL:
                        priority = 7;
                        break;
                    case Operator.OP_AND:
                        priority = 12;
                        break;
                    case Operator.OP_OR:
                        priority = 14;
                        break;
                    case Operator.OP_NOT:
                        priority = 3;
                        unary = true;
                        break;
                    case Operator.OP_NEGATE:
                        priority = 3;
                        unary = true;
                        break;
                    case Operator.OP_ADD:
                        priority = 5;
                        break;
                    case Operator.OP_SUB:
                        priority = 5;
                        break;
                    case Operator.OP_MUL:
                        priority = 4;
                        break;
                    case Operator.OP_DIV:
                        priority = 4;
                        break;
                    case Operator.OP_MOD:
                        priority = 4;
                        break;
                    case Operator.OP_SHIFT_LEFT:
                        priority = 6;
                        break;
                    case Operator.OP_SHIFT_RIGHT:
                        priority = 6;
                        break;
                    case Operator.OP_ASSIGN:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_ADD:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_SUB:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_MUL:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_DIV:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_MOD:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_SHIFT_LEFT:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_SHIFT_RIGHT:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_BIT_AND:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_BIT_OR:
                        priority = 16;
                        break;
                    case Operator.OP_ASSIGN_BIT_XOR:
                        priority = 16;
                        break;
                    case Operator.OP_BIT_AND:
                        priority = 9;
                        break;
                    case Operator.OP_BIT_OR:
                        priority = 11;
                        break;
                    case Operator.OP_BIT_XOR:
                        priority = 10;
                        break;
                    case Operator.OP_BIT_INVERT:
                        priority = 3;
                        unary = true;
                        break;
                    case Operator.OP_INCREMENT:
                        priority = 3;
                        unary = true;
                        break;
                    case Operator.OP_DECREMENT:
                        priority = 3;
                        unary = true;
                        break;
                    case Operator.OP_SELECT_IF:
                        priority = 15;
                        ternary = true;
                        break;
                    case Operator.OP_SELECT_ELSE:
                        priority = 15;
                        ternary = true;
                        break;

                    default:
                        return ERR_FAIL_V(default(Node)); //unexpected operator
                }

                #if DEBUG
                if (
                    this.checkWarnings
                    && this.HAS_WARNING(ShaderWarning.CodeFlags.FLOAT_COMPARISON_FLAG)
                    && op == Operator.OP_EQUAL || op == Operator.OP_NOT_EQUAL
                    && !expression[i - 1].IsOp
                    && !expression[i + 1].IsOp
                    && expression[i - 1].Node!.GetDatatype() == DataType.TYPE_FLOAT
                    && expression[i + 1].Node!.GetDatatype() == DataType.TYPE_FLOAT
                )
                {
                    this.AddLineWarning(ShaderWarning.Code.FLOAT_COMPARISON);
                }
                #endif // DEBUG_ENABLED

                if (priority < minPriority)
                {
                    // < is used for left to right (default)
                    // <= is used for right to left
                    nextOp      = i;
                    minPriority = priority;
                    isUnary     = unary;
                    isTernary   = ternary;
                }
            }

            if (ERR_FAIL_COND_V(nextOp == -1))
            {
                return null;
            }

            // OK! create operator..
            if (isUnary)
            {
                var exprPos = nextOp;
                while (expression[exprPos].IsOp)
                {
                    exprPos++;
                    if (exprPos == expression.Count)
                    {
                        //can happen..
                        this.SetError(RTR("Unexpected end of expression."));
                        return null;
                    }
                }

                //consecutively do unary operators
                for (var i = exprPos - 1; i >= nextOp; i--)
                {
                    var op = this.AllocNode<OperatorNode>();
                    op.Op = expression[i].Op;
                    if ((op.Op == Operator.OP_INCREMENT || op.Op == Operator.OP_DECREMENT) && !this.ValidateAssign(expression[i + 1].Node!, functionInfo))
                    {
                        this.SetError(RTR("Invalid use of increment/decrement operator in a constant expression."));
                        return null;
                    }
                    op.Arguments.Add(expression[i + 1].Node!);

                    expression[i].IsOp = false;
                    expression[i].Node = op;

                    if (!this.ValidateOperator(op, out var returnCache, out var returnArraySize))
                    {
                        var at = "";
                        for (var j = 0; j < op.Arguments.Count; j++)
                        {
                            if (j > 0)
                            {
                                at += ", ";
                            }
                            at += GetDatatypeName(op.Arguments[j].GetDatatype());
                            if (!op.Arguments[j].IsIndexed && op.Arguments[j].GetArraySize() > 0)
                            {
                                at += "[";
                                at += op.Arguments[j].GetArraySize();
                                at += "]";
                            }
                        }
                        this.SetError(string.Format(RTR("Invalid arguments to unary operator '{0}': {1}."), GetOperatorText(op.Op), at));
                        return null;
                    }

                    op.ReturnCache     = returnCache;
                    op.ReturnArraySize = returnArraySize;

                    expression.RemoveAt(i + 1);
                }
            }
            else if (isTernary)
            {
                if (nextOp < 1 || nextOp >= expression.Count - 1)
                {
                    this.SetParsingError();
                    return ERR_FAIL_V(default(Node));
                }

                if (nextOp + 2 >= expression.Count || !expression[nextOp + 2].IsOp || expression[nextOp + 2].Op != Operator.OP_SELECT_ELSE)
                {
                    this.SetError(RTR("Missing matching ':' for select operator."));
                    return null;
                }

                var op = this.AllocNode<OperatorNode>();
                op.Op = expression[nextOp].Op;
                op.Arguments.Add(expression[nextOp - 1].Node!);
                op.Arguments.Add(expression[nextOp + 1].Node!);
                op.Arguments.Add(expression[nextOp + 3].Node!);

                expression[nextOp - 1].IsOp = false;
                expression[nextOp - 1].Node = op;
                if (!this.ValidateOperator(op, out var returnCache, out var returnArraySize))
                {
                    var at = "";
                    for (var i = 0; i < op.Arguments.Count; i++)
                    {
                        if (i > 0)
                        {
                            at += ", ";
                        }
                        at += GetDatatypeName(op.Arguments[i].GetDatatype());
                        if (!op.Arguments[i].IsIndexed && op.Arguments[i].GetArraySize() > 0)
                        {
                            at += "[";
                            at += op.Arguments[i].GetArraySize();
                            at += "]";
                        }
                    }
                    this.SetError(string.Format(RTR("Invalid argument to ternary operator: '{0}'."), at));
                    return null;
                }

                op.ReturnCache     = returnCache;
                op.ReturnArraySize = returnArraySize;

                for (var i = 0; i < 4; i++)
                {
                    expression.RemoveAt(nextOp);
                }

            }
            else
            {
                if (nextOp < 1 || nextOp >= expression.Count - 1)
                {
                    this.SetParsingError();
                    return ERR_FAIL_V(default(Node));
                }

                var op = this.AllocNode<OperatorNode>();
                op.Op = expression[nextOp].Op;

                if (expression[nextOp - 1].IsOp)
                {
                    this.SetParsingError();
                    return ERR_FAIL_V(default(Node));
                }

                if (this.IsOperatorAssign(op.Op))
                {
                    if (!this.ValidateAssign(expression[nextOp - 1].Node!, functionInfo, out var assignMessage))
                    {
                        this.SetError(assignMessage);
                        return null;
                    }
                }

                if (expression[nextOp + 1].IsOp)
                {
                    // this is not invalid and can really appear
                    // but it becomes invalid anyway because no binary op
                    // can be followed by a unary op in a valid combination,
                    // due to how precedence works, unaries will always disappear first

                    this.SetParsingError();
                }

                op.Arguments.Add(expression[nextOp - 1].Node!); //expression goes as left
                op.Arguments.Add(expression[nextOp + 1].Node!); //next expression goes as right
                expression[nextOp - 1].Node = op;

                //replace all 3 nodes by this operator and make it an expression

                if (!this.ValidateOperator(op, out var returnCache, out var returnArraySize))
                {
                    var at = "";
                    for (var i = 0; i < op.Arguments.Count; i++)
                    {
                        if (i > 0)
                        {
                            at += ", ";
                        }
                        if (op.Arguments[i].GetDatatype() == DataType.TYPE_STRUCT)
                        {
                            at += op.Arguments[i].GetDatatypeName();
                        }
                        else
                        {
                            at += GetDatatypeName(op.Arguments[i].GetDatatype());
                        }
                        if (!op.Arguments[i].IsIndexed && op.Arguments[i].GetArraySize() > 0)
                        {
                            at += "[";
                            at += op.Arguments[i].GetArraySize();
                            at += "]";
                        }
                    }
                    this.SetError(string.Format(RTR("Invalid arguments to operator '{0}': '{1}'."), GetOperatorText(op.Op), at));
                    return null;
                }

                op.ReturnCache     = returnCache;
                op.ReturnArraySize = returnArraySize;

                expression.RemoveAt(nextOp);
                expression.RemoveAt(nextOp);
            }
        }

        return expression[0].Node;
    }

    private bool ParseFunctionArguments(BlockNode? block, FunctionInfo functionInfo, OperatorNode func, out int completeArg)
    {
        completeArg = -1;

        var pos = this.GetTkpos();
        var tk =  this.GetToken();

        if (tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
        {
            return true;
        }

        this.SetTkpos(pos);

        while (true)
        {
            pos = this.GetTkpos();
            tk  = this.GetToken();

            if (tk.Type == TokenType.TK_CURSOR)
            {
                completeArg = func.Arguments.Count - 1;
            }
            else
            {
                this.SetTkpos(pos);
            }

            var arg = this.ParseAndReduceExpression(block, functionInfo);

            if (arg == null)
            {
                return false;
            }

            if (this.isConstDecl && arg.Type == Node.NodeType.TYPE_VARIABLE)
            {
                var variable = (VariableNode)arg;
                if (!variable.IsConst)
                {
                    this.SetError(RTR("Expected constant expression."));
                    return false;
                }
            }

            func.Arguments.Add(arg);

            tk = this.GetToken();

            if (tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
            {
                return true;
            }
            else if (tk.Type != TokenType.TK_COMMA)
            {
                // something is broken
                this.SetError(RTR("Expected ',' or ')' after argument."));
                return false;
            }
        }
    }

    private Error ParseShader(Dictionary<string, FunctionInfo> functions, ModeInfo[] renderModes, HashSet<string> shaderTypes)
    {
        Token tk;
        TkPos prevPos;
        Token next;

        if (!this.isShaderInc)
        {
            #if DEBUG
            this.keywordCompletionContext = ContextFlag.CF_SHADER_TYPE;
            #endif // DEBUG_ENABLED

            tk = this.GetToken();

            if (tk.Type != TokenType.TK_SHADER_TYPE)
            {
                this.SetError(string.Format(RTR("Expected '{0}' at the beginning of shader. Valid types are: {1}.", "shader_type"), this.GetShaderTypeList(shaderTypes)));
                return Error.ERR_PARSE_ERROR;
            }

            #if DEBUG
            this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
            #endif // DEBUG_ENABLED

            this.GetCompletableIdentifier(null, CompletionType.COMPLETION_SHADER_TYPE, out this.shaderTypeIdentifier);

            if (this.shaderTypeIdentifier == null)
            {
                this.SetError(string.Format(RTR("Expected an identifier after '{0}', indicating the type of shader. Valid types are: {1}."), "shader_type", this.GetShaderTypeList(shaderTypes)));
                return Error.ERR_PARSE_ERROR;
            }

            if (!shaderTypes.Contains(this.shaderTypeIdentifier))
            {
                this.SetError(string.Format(RTR("Invalid shader type. Valid types are: {0}"), this.GetShaderTypeList(shaderTypes)));
                return Error.ERR_PARSE_ERROR;
            }

            prevPos = this.GetTkpos();
            tk      = this.GetToken();

            if (tk.Type != TokenType.TK_SEMICOLON)
            {
                this.SetTkpos(prevPos);
                this.SetExpectedAfterError(";", "shader_type " + this.shaderTypeIdentifier);

                return Error.ERR_PARSE_ERROR;
            }
        }

        #if DEBUG
        this.keywordCompletionContext = ContextFlag.CF_GLOBAL_SPACE;
        #endif // DEBUG_ENABLED

        tk = this.GetToken();

        var textureUniforms = 0;
        var textureBinding  = 0u;
        var uniforms        = 0;
        var instanceIndex   = 0;

        #if DEBUG
        var uniformBufferSize         = 0UL;
        var maxUniformBufferSize      = 0UL;
        var uniformBufferExceededLine = -1;

        var checkDeviceLimitWarnings = false;
        var device                   = RenderingDevice.Singleton;

        if (device != null)
        {
            checkDeviceLimitWarnings = this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.DEVICE_LIMIT_EXCEEDED_FLAG);

            maxUniformBufferSize = device.LimitGet(RenderingDevice.Limit.LIMIT_MAX_UNIFORM_BUFFER_SIZE);
        }
        #endif // DEBUG_ENABLED

        var uniformScope = ShaderNode.Uniform.ScopeKind.SCOPE_LOCAL;

        this.stages = functions;

        if (!functions.TryGetValue("constants", out var constants))
        {
            constants = new();
        }

        var definedModes = new Dictionary<string, string>();

        while (tk.Type != TokenType.TK_EOF)
        {
            switch (tk.Type)
            {
                case TokenType.TK_RENDER_MODE:
                    while (true)
                    {
                        this.GetCompletableIdentifier(null, CompletionType.COMPLETION_RENDER_MODE, out var mode);

                        if (mode == null)
                        {
                            this.SetError(RTR("Expected an identifier for render mode."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        var smode = mode;

                        if (this.shader!.RenderModes.Contains(mode))
                        {
                            this.SetError(string.Format(RTR("Duplicated render mode: '{0}'."), smode));
                            return Error.ERR_PARSE_ERROR;
                        }

                        var found = false;

                        if (this.isShaderInc)
                        {
                            for (var i = 0; i < (int)RS.ShaderMode.SHADER_MAX; i++)
                            {
                                var modes = ShaderTypes.Singleton.GetModes((RS.ShaderMode)i);

                                for (var j = 0; j < modes.Length; j++)
                                {
                                    var info = modes[j];
                                    var name = info.Name!;

                                    if (smode.StartsWith(name))
                                    {
                                        if (info.Options.Count != 0)
                                        {
                                            if (info.Options.Contains(smode[(name.Length + 1)..]))
                                            {
                                                found = true;

                                                if (definedModes.TryGetValue(name, out var value))
                                                {
                                                    this.SetError(string.Format(RTR("Redefinition of render mode: '{0}'. The '{1}' mode has already been set to '{2}'."), smode, name, value));
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                                definedModes.Add(name, smode);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < renderModes.Length; i++)
                            {
                                var info = renderModes[i];
                                var name = info.Name!;

                                if (smode.StartsWith(name))
                                {
                                    if (info.Options.Count != 0)
                                    {
                                        if (info.Options.Contains(smode[(name.Length + 1)..]))
                                        {
                                            found = true;

                                            if (definedModes.TryGetValue(name, out var value))
                                            {
                                                this.SetError(string.Format(RTR("Redefinition of render mode: '{0}'. The '{1}' mode has already been set to '{2}'."), smode, name, value));
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                            definedModes.Add(name, smode);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!found)
                        {
                            this.SetError(string.Format(RTR("Invalid render mode: '{0}'."), smode));
                            return Error.ERR_PARSE_ERROR;
                        }

                        this.shader.RenderModes.Add(mode);

                        tk = this.GetToken();
                        if (tk.Type == TokenType.TK_COMMA)
                        {
                            //all good, do nothing
                        }
                        else if (tk.Type == TokenType.TK_SEMICOLON)
                        {
                            break; //done
                        }
                        else
                        {
                            this.SetError(string.Format(RTR("Unexpected token: '{0}'."), GetTokenText(tk)));
                            return Error.ERR_PARSE_ERROR;
                        }
                    }
                    break;
                case TokenType.TK_STRUCT:
                    {
                        ShaderNode.Struct st;

                        DataType type;

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                        #endif // DEBUG_ENABLED

                        tk = this.GetToken();

                        if (tk.Type == TokenType.TK_IDENTIFIER)
                        {
                            st = new ShaderNode.Struct() { Name = tk.Text };

                            if (this.shader!.Constants.ContainsKey(st.Name) || this.shader.Structs.ContainsKey(st.Name))
                            {
                                this.SetRedefinitionError(st.Name);
                                return Error.ERR_PARSE_ERROR;
                            }

                            tk = this.GetToken();

                            if (tk.Type != TokenType.TK_CURLY_BRACKET_OPEN)
                            {
                                this.SetExpectedError("{");
                                return Error.ERR_PARSE_ERROR;
                            }
                        }
                        else
                        {
                            this.SetError(RTR("Expected a struct identifier."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        var stNode = this.AllocNode<StructNode>();
                        st.ShaderStruct = stNode;

                        var memberCount = 0;
                        var memberNames = new HashSet<string>();

                        while (true)
                        {
                            // variables list
                            #if DEBUG
                            this.keywordCompletionContext = ContextFlag.CF_DATATYPE | ContextFlag.CF_PRECISION_MODIFIER;
                            #endif // DEBUG_ENABLED

                            tk = this.GetToken();
                            if (tk.Type == TokenType.TK_CURLY_BRACKET_CLOSE)
                            {
                                break;
                            }

                            var structName = "";
                            var structDt   = false;
                            var precision  = DataPrecision.PRECISION_DEFAULT;

                            if (tk.Type == TokenType.TK_STRUCT)
                            {
                                this.SetError(RTR("Nested structs are not allowed."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (IsTokenPrecision(tk.Type))
                            {
                                precision = GetTokenPrecision(tk.Type);
                                tk = this.GetToken();

                                #if DEBUG
                                this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER;
                                #endif // DEBUG_ENABLED
                            }

                            if (this.shader.Structs.ContainsKey(tk.Text))
                            {
                                structName = tk.Text;
                                #if DEBUG
                                if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_STRUCT_FLAG) && this.usedStructs.TryGetValue(structName, out var value))
                                {
                                    value.Used = true;
                                }
                                #endif // DEBUG_ENABLED
                                structDt = true;
                            }

                            if (!IsTokenDatatype(tk.Type) && !structDt)
                            {
                                this.SetError(RTR("Expected data type."));
                                return Error.ERR_PARSE_ERROR;
                            }
                            else
                            {
                                type = structDt ? DataType.TYPE_STRUCT : GetTokenDatatype(tk.Type);

                                if (precision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(type, precision) != Error.OK)
                                {
                                    return Error.ERR_PARSE_ERROR;
                                }

                                if (type == DataType.TYPE_VOID || IsSamplerType(type))
                                {
                                    this.SetError(string.Format(RTR("A '{0}' data type is not allowed here."), GetDatatypeName(type)));
                                    return Error.ERR_PARSE_ERROR;
                                }

                                #if DEBUG
                                this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                                #endif // DEBUG_ENABLED

                                var first          = true;
                                var fixedArraySize = false;
                                var arraySize      = 0u;

                                do
                                {
                                    tk = this.GetToken();

                                    if (first)
                                    {
                                        first = false;

                                        if (tk.Type != TokenType.TK_IDENTIFIER && tk.Type != TokenType.TK_BRACKET_OPEN)
                                        {
                                            this.SetError(RTR("Expected an identifier or '['."));
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        if (tk.Type == TokenType.TK_BRACKET_OPEN)
                                        {
                                            var error = this.ParseArraySize(null, constants, true, out var _, out arraySize, out var _);
                                            if (error != Error.OK)
                                            {
                                                return error;
                                            }
                                            fixedArraySize = true;
                                            tk = this.GetToken();
                                        }
                                    }

                                    if (tk.Type != TokenType.TK_IDENTIFIER)
                                    {
                                        this.SetError(RTR("Expected an identifier."));
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    var member = this.AllocNode<MemberNode>();

                                    member.Precision  = precision;
                                    member.Datatype   = type;
                                    member.StructName = structName;
                                    member.Name       = tk.Text;
                                    member.ArraySize  = arraySize;

                                    if (memberNames.Contains(member.Name))
                                    {
                                        this.SetRedefinitionError(member.Name);
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                    memberNames.Add(member.Name);
                                    tk = this.GetToken();

                                    if (tk.Type == TokenType.TK_BRACKET_OPEN)
                                    {
                                        var error = this.ParseArraySize(null, constants, true, out var _, out arraySize, out var _);
                                        member.ArraySize = arraySize;
                                        if (error != Error.OK)
                                        {
                                            return error;
                                        }
                                        tk = this.GetToken();
                                    }

                                    if (!fixedArraySize)
                                    {
                                        arraySize = 0;
                                    }

                                    if (tk.Type != TokenType.TK_SEMICOLON && tk.Type != TokenType.TK_COMMA)
                                    {
                                        this.SetExpectedError(",", ";");
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    stNode.Members.Add(member);
                                    memberCount++;
                                } while (tk.Type == TokenType.TK_COMMA); // another member
                            }
                        }

                        if (memberCount == 0)
                        {
                            this.SetError(RTR("Empty structs are not allowed."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                        #endif // DEBUG_ENABLED

                        tk = this.GetToken();

                        if (tk.Type != TokenType.TK_SEMICOLON)
                        {
                            this.SetExpectedError(";");
                            return Error.ERR_PARSE_ERROR;
                        }

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_GLOBAL_SPACE;
                        #endif // DEBUG_ENABLED

                        this.shader.Structs.Add(st.Name, st);
                        this.shader.Vstructs.Add(st); // struct's order is important!

                        #if DEBUG
                        if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_STRUCT_FLAG))
                        {
                            this.usedStructs.Add(st.Name, new(this.tkLine));
                        }
                        #endif // DEBUG_ENABLED
                    }
                    break;
                case TokenType.TK_GLOBAL:
                    {
                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNIFORM_KEYWORD;
                        if (this.LookupNext(out next))
                        {
                            if (next.Type == TokenType.TK_UNIFORM)
                            {
                                this.keywordCompletionContext ^= ContextFlag.CF_UNIFORM_KEYWORD;
                            }
                        }
                        #endif // DEBUG_ENABLED
                        tk = this.GetToken();
                        if (tk.Type != TokenType.TK_UNIFORM)
                        {
                            this.SetExpectedAfterError("uniform", "global");
                            return Error.ERR_PARSE_ERROR;
                        }
                        uniformScope = ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL;
                        goto case TokenType.TK_INSTANCE;
                    }
                case TokenType.TK_INSTANCE:
                    {
                        if (tk.Type == TokenType.TK_INSTANCE)
                        {
                            #if DEBUG
                            this.keywordCompletionContext = ContextFlag.CF_UNIFORM_KEYWORD;
                            if (this.LookupNext(out next))
                            {
                                if (next.Type == TokenType.TK_UNIFORM)
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_UNIFORM_KEYWORD;
                                }
                            }
                            #endif // DEBUG_ENABLED
                            if (this.shaderTypeIdentifier != "spatial")
                            {
                                this.SetError(RTR("Uniform instances are not yet implemented for '{shader_type_identifier}' shaders."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (OS.Singleton.CurrentRenderingMethod == "gl_compatibility")
                            {
                                this.SetError(RTR("Uniform instances are not supported in gl_compatibility shaders."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_LOCAL)
                            {
                                tk = this.GetToken();

                                if (tk.Type != TokenType.TK_UNIFORM)
                                {
                                    this.SetExpectedAfterError("uniform", "instance");
                                    return Error.ERR_PARSE_ERROR;
                                }

                                uniformScope = ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE;
                            }
                        }
                    }
                    goto case TokenType.TK_VARYING;
                case TokenType.TK_UNIFORM:
                case TokenType.TK_VARYING:
                    {
                        var isUniform = tk.Type == TokenType.TK_UNIFORM;

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                        #endif // DEBUG_ENABLED

                        if (!isUniform)
                        {
                            if (this.shaderTypeIdentifier == "particles" || this.shaderTypeIdentifier == "sky" || this.shaderTypeIdentifier == "fog")
                            {
                                this.SetError(string.Format(RTR("Varyings cannot be used in '{0}' shaders."), this.shaderTypeIdentifier));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        var precision     = DataPrecision.PRECISION_DEFAULT;
                        var interpolation = DataInterpolation.INTERPOLATION_DEFAULT;
                        var type          = default(DataType);
                        var name          = "";
                        var arraySize     = 0u;

                        tk = this.GetToken();
                        #if DEBUG

                        var tempError = false;
                        ContextFlag datatypeFlag;

                        if (!isUniform)
                        {
                            datatypeFlag            = ContextFlag.CF_VARYING_TYPE;
                            this.keywordCompletionContext = ContextFlag.CF_INTERPOLATION_QUALIFIER | ContextFlag.CF_PRECISION_MODIFIER | datatypeFlag;

                            if (this.LookupNext(out next))
                            {
                                if (IsTokenInterpolation(next.Type))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_INTERPOLATION_QUALIFIER | datatypeFlag;
                                }
                                else if (IsTokenPrecision(next.Type))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER | datatypeFlag;
                                }
                                else if (IsTokenDatatype(next.Type))
                                {
                                    this.keywordCompletionContext ^= datatypeFlag;
                                }
                            }
                        }
                        else
                        {
                            datatypeFlag = ContextFlag.CF_UNIFORM_TYPE;

                            this.keywordCompletionContext = ContextFlag.CF_PRECISION_MODIFIER | datatypeFlag;

                            if (this.LookupNext(out next))
                            {
                                if (IsTokenPrecision(next.Type))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER | datatypeFlag;
                                }
                                else if (IsTokenDatatype(next.Type))
                                {
                                    this.keywordCompletionContext ^= datatypeFlag;
                                }
                            }
                        }
                        #endif // DEBUG_ENABLED

                        if (IsTokenInterpolation(tk.Type))
                        {
                            if (isUniform)
                            {
                                this.SetError(RTR("Interpolation qualifiers are not supported for uniforms."));
                                #if DEBUG
                                tempError = true;
                                #else
                                return Error.ERR_PARSE_ERROR;
                                #endif // DEBUG_ENABLED
                            }

                            interpolation = GetTokenInterpolation(tk.Type);

                            tk = this.GetToken();

                            #if DEBUG
                            if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_INTERPOLATION_QUALIFIER))
                            {
                                this.keywordCompletionContext ^= ContextFlag.CF_INTERPOLATION_QUALIFIER;
                            }
                            if (this.LookupNext(out next))
                            {
                                if (IsTokenPrecision(next.Type))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER;
                                }
                                else if (IsTokenDatatype(next.Type))
                                {
                                    this.keywordCompletionContext ^= datatypeFlag;
                                }
                            }
                            if (tempError)
                            {
                                return Error.ERR_PARSE_ERROR;
                            }
                            #endif // DEBUG_ENABLED
                        }

                        if (IsTokenPrecision(tk.Type))
                        {
                            precision = GetTokenPrecision(tk.Type);

                            tk = this.GetToken();

                            #if DEBUG
                            if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_INTERPOLATION_QUALIFIER))
                            {
                                this.keywordCompletionContext ^= ContextFlag.CF_INTERPOLATION_QUALIFIER;
                            }

                            if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_PRECISION_MODIFIER))
                            {
                                this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER;
                            }

                            if (this.LookupNext(out next))
                            {
                                if (IsTokenDatatype(next.Type))
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                                }
                            }
                            #endif // DEBUG_ENABLED
                        }

                        if (this.shader!.Structs.ContainsKey(tk.Text))
                        {
                            if (isUniform)
                            {
                                this.SetError(RTR("The 'struct' data type is not supported for uniforms."));
                                return Error.ERR_PARSE_ERROR;
                            }
                            else
                            {
                                this.SetError(RTR("The 'struct' data type is not allowed here."));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        if (!IsTokenDatatype(tk.Type))
                        {
                            this.SetError(RTR("Expected data type."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        type = GetTokenDatatype(tk.Type);

                        if (precision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(type, precision) != Error.OK)
                        {
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (type == DataType.TYPE_VOID)
                        {
                            this.SetError(RTR("The 'void' data type is not allowed here."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (!isUniform && interpolation != DataInterpolation.INTERPOLATION_DEFAULT && type < DataType.TYPE_INT)
                        {
                            this.SetError(string.Format(RTR("Interpolation modifier '{0}' cannot be used with boolean types."), GetInterpolationName(interpolation)));
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (!isUniform && type > DataType.TYPE_MAT4)
                        {
                            this.SetError(RTR("Invalid data type for varying."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                        #endif // DEBUG_ENABLED
                        tk = this.GetToken();

                        if (tk.Type != TokenType.TK_IDENTIFIER && tk.Type != TokenType.TK_BRACKET_OPEN)
                        {
                            this.SetError(RTR("Expected an identifier or '['."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (tk.Type == TokenType.TK_BRACKET_OPEN)
                        {
                            var error = this.ParseArraySize(null, constants, true, out var _, out arraySize, out var _);
                            if (error != Error.OK)
                            {
                                return error;
                            }
                            tk = this.GetToken();
                        }

                        if (tk.Type != TokenType.TK_IDENTIFIER)
                        {
                            this.SetError(RTR("Expected an identifier."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        prevPos = this.GetTkpos();
                        name = tk.Text;

                        if (this.FindIdentifier(null, false, constants, name))
                        {
                            this.SetRedefinitionError(name);
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (HasBuiltin(functions, name))
                        {
                            this.SetRedefinitionError(name);
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (isUniform)
                        {
                            if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL && Engine.Singleton.IsEditorHint)
                            {
                                // Type checking for global uniforms is not allowed outside the editor.
                                //validate global uniform
                                var gvtype = this.globalShaderUniformGetTypeFunc!(name);
                                if (gvtype == DataType.TYPE_MAX)
                                {
                                    this.SetError(string.Format(RTR("Global uniform '{0}' does not exist. Create it in Project Settings."), name));
                                    return Error.ERR_PARSE_ERROR;
                                }

                                if (type != gvtype)
                                {
                                    this.SetError(string.Format(RTR("Global uniform '{0}' must be of type '{1}'."), name, GetDatatypeName(gvtype)));
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            var uniform = new ShaderNode.Uniform
                            {
                                Type      = type,
                                Scope     = uniformScope,
                                Precision = precision,
                                ArraySize = arraySize,
                                Group     = this.currentUniformGroupName,
                                Subgroup  = this.currentUniformSubgroupName
                            };

                            tk = this.GetToken();
                            if (tk.Type == TokenType.TK_BRACKET_OPEN)
                            {
                                var error = this.ParseArraySize(null, constants, true, out var _, out arraySize, out var _);
                                uniform.ArraySize = arraySize;
                                if (error != Error.OK)
                                {
                                    return error;
                                }
                                tk = this.GetToken();
                            }

                            if (IsSamplerType(type))
                            {
                                if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                                {
                                    this.SetError(RTR("The 'SCOPE_INSTANCE' qualifier is not supported for sampler types."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                                uniform.TextureOrder = textureUniforms++;
                                uniform.TextureBinding = textureBinding;
                                if (uniform.ArraySize > 0)
                                {
                                    textureBinding += uniform.ArraySize;
                                }
                                else
                                {
                                    ++textureBinding;
                                }
                                uniform.Order = -1;
                            }
                            else
                            {
                                if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE && (type == DataType.TYPE_MAT2 || type == DataType.TYPE_MAT3 || type == DataType.TYPE_MAT4))
                                {
                                    this.SetError(RTR("The 'SCOPE_INSTANCE' qualifier is not supported for matrix types."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                                uniform.TextureOrder = -1;
                                if (uniformScope != ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                                {
                                    uniform.Order = uniforms++;
                                    #if DEBUG
                                    if (checkDeviceLimitWarnings)
                                    {
                                        if (uniform.ArraySize > 0)
                                        {
                                            var size = (int)(GetDatatypeSize(uniform.Type) * uniform.ArraySize);
                                            var m = 16 * uniform.ArraySize;
                                            if (size % m != 0U)
                                            {
                                                size += (int)(m - size % m);
                                            }
                                            uniformBufferSize += (ulong)size;
                                        }
                                        else
                                        {
                                            uniformBufferSize += GetDatatypeSize(uniform.Type);
                                        }

                                        if (uniformBufferExceededLine == -1 && uniformBufferSize > maxUniformBufferSize)
                                        {
                                            uniformBufferExceededLine = this.tkLine;
                                        }
                                    }
                                    #endif // DEBUG_ENABLED
                                }
                            }

                            if (uniform.ArraySize > 0)
                            {
                                if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL)
                                {
                                    this.SetError(RTR("The 'SCOPE_GLOBAL' qualifier is not supported for uniform arrays."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                                if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                                {
                                    this.SetError(RTR("The 'SCOPE_INSTANCE' qualifier is not supported for uniform arrays."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            var customInstanceIndex = -1;

                            if (tk.Type == TokenType.TK_COLON)
                            {
                                this.completionType      = CompletionType.COMPLETION_HINT;
                                this.completionBase      = type;
                                this.completionBaseArray = uniform.ArraySize > 0;

                                //hint
                                do
                                {
                                    tk = this.GetToken();
                                    this.completionLine = tk.Line;

                                    if (!IsTokenHint(tk.Type))
                                    {
                                        this.SetError(RTR("Expected valid type hint after ':'."));
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    if (uniform.ArraySize > 0)
                                    {
                                        if (tk.Type != TokenType.TK_HINT_SOURCE_COLOR)
                                        {
                                            this.SetError(RTR("This hint is not supported for uniform arrays."));
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                    }

                                    var newHint   = ShaderNode.Uniform.HintKind.HINT_NONE;
                                    var newFilter = TextureFilter.FILTER_DEFAULT;
                                    var newRepeat = TextureRepeat.REPEAT_DEFAULT;

                                    switch (tk.Type)
                                    {
                                        case TokenType.TK_HINT_SOURCE_COLOR:
                                            if (type != DataType.TYPE_VEC3 && type != DataType.TYPE_VEC4 && !IsSamplerType(type))
                                            {
                                                this.SetError(RTR("Source color hint is for 'vec3', 'vec4' or sampler types only."));
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            if (IsSamplerType(type))
                                            {
                                                if (uniform.UseColor)
                                                {
                                                    this.SetError(RTR("Duplicated hint: 'source_color'."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                                uniform.UseColor = true;
                                            }
                                            else
                                            {
                                                newHint = ShaderNode.Uniform.HintKind.HINT_SOURCE_COLOR;
                                            }
                                            break;
                                        case TokenType.TK_HINT_DEFAULT_BLACK_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_DEFAULT_BLACK;
                                            break;
                                        case TokenType.TK_HINT_DEFAULT_WHITE_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_DEFAULT_WHITE;
                                            break;
                                        case TokenType.TK_HINT_DEFAULT_TRANSPARENT_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_DEFAULT_TRANSPARENT;
                                            break;
                                        case TokenType.TK_HINT_NORMAL_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_NORMAL;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_NORMAL_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_NORMAL;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_R:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_R;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_G:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_G;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_B:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_B;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_A:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_A;
                                            break;
                                        case TokenType.TK_HINT_ROUGHNESS_GRAY:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ROUGHNESS_GRAY;
                                            break;
                                        case TokenType.TK_HINT_ANISOTROPY_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_ANISOTROPY;
                                            break;
                                        case TokenType.TK_HINT_RANGE:
                                            {
                                                if (type != DataType.TYPE_FLOAT && type != DataType.TYPE_INT)
                                                {
                                                    this.SetError(RTR("Range hint is for 'float' and 'int' only."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                tk = this.GetToken();
                                                if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                                                {
                                                    this.SetExpectedAfterError("(", "hint_range");
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                tk = this.GetToken();

                                                var sign = 1.0f;

                                                if (tk.Type == TokenType.TK_OP_SUB)
                                                {
                                                    sign = -1.0f;
                                                    tk = this.GetToken();
                                                }

                                                if (tk.Type != TokenType.TK_FLOAT_CONSTANT && !tk.IsIntegerConstant)
                                                {
                                                    this.SetError(RTR("Expected an integer constant."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                uniform.HintRange[0] = tk.Constant;
                                                uniform.HintRange[0] *= sign;

                                                tk = this.GetToken();

                                                if (tk.Type != TokenType.TK_COMMA)
                                                {
                                                    this.SetError(RTR("Expected ',' after integer constant."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                tk = this.GetToken();

                                                sign = 1.0f;

                                                if (tk.Type == TokenType.TK_OP_SUB)
                                                {
                                                    sign = -1.0f;
                                                    tk = this.GetToken();
                                                }

                                                if (tk.Type != TokenType.TK_FLOAT_CONSTANT && !tk.IsIntegerConstant)
                                                {
                                                    this.SetError(RTR("Expected an integer constant after ','."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                uniform.HintRange[1] = tk.Constant;
                                                uniform.HintRange[1] *= sign;

                                                tk = this.GetToken();

                                                if (tk.Type == TokenType.TK_COMMA)
                                                {
                                                    tk = this.GetToken();

                                                    if (tk.Type != TokenType.TK_FLOAT_CONSTANT && !tk.IsIntegerConstant)
                                                    {
                                                        this.SetError(RTR("Expected an integer constant after ','."));
                                                        return Error.ERR_PARSE_ERROR;
                                                    }

                                                    uniform.HintRange[2] = tk.Constant;
                                                    tk = this.GetToken();
                                                }
                                                else
                                                {
                                                    uniform.HintRange[2] = type == DataType.TYPE_INT
                                                        ? 1
                                                        : 0.001f;
                                                }

                                                if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                                                {
                                                    this.SetExpectedError(")");
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                newHint = ShaderNode.Uniform.HintKind.HINT_RANGE;
                                            }
                                            break;
                                        case TokenType.TK_HINT_INSTANCE_INDEX:
                                            if (customInstanceIndex != -1)
                                            {
                                                this.SetError(RTR("Can only specify 'instance_index' once."));
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            tk = this.GetToken();
                                            if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                                            {
                                                this.SetExpectedAfterError("(", "instance_index");
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            tk = this.GetToken();

                                            if (tk.Type == TokenType.TK_OP_SUB)
                                            {
                                                this.SetError(RTR("The instance index can't be negative."));
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            if (!tk.IsIntegerConstant)
                                            {
                                                this.SetError(RTR("Expected an integer constant."));
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            customInstanceIndex = (int)tk.Constant;
                                            this.currentUniformInstanceIndexDefined = true;

                                            if (customInstanceIndex >= MAX_INSTANCE_UNIFORM_INDICES)
                                            {
                                                this.SetError(string.Format(RTR("Allowed instance uniform indices must be within [0..{0}] range."), MAX_INSTANCE_UNIFORM_INDICES - 1));
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            tk = this.GetToken();

                                            if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                                            {
                                                this.SetExpectedError(")");
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                            break;
                                        case TokenType.TK_HINT_SCREEN_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE;
                                            --textureUniforms;
                                            --textureBinding;
                                            break;
                                        case TokenType.TK_HINT_NORMAL_ROUGHNESS_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE;
                                            --textureUniforms;
                                            --textureBinding;
                                            if (OS.Singleton.CurrentRenderingMethod == "gl_compatibility")
                                            {
                                                this.SetError(RTR("'hint_normal_roughness_texture' is not supported in gl_compatibility shaders."));
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                            break;
                                        case TokenType.TK_HINT_DEPTH_TEXTURE:
                                            newHint = ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE;
                                            --textureUniforms;
                                            --textureBinding;
                                            break;
                                        case TokenType.TK_FILTER_NEAREST:
                                            newFilter = TextureFilter.FILTER_NEAREST;
                                            break;
                                        case TokenType.TK_FILTER_LINEAR:
                                            newFilter = TextureFilter.FILTER_LINEAR;
                                            break;
                                        case TokenType.TK_FILTER_NEAREST_MIPMAP:
                                            newFilter = TextureFilter.FILTER_NEAREST_MIPMAP;
                                            break;
                                        case TokenType.TK_FILTER_LINEAR_MIPMAP:
                                            newFilter = TextureFilter.FILTER_LINEAR_MIPMAP;
                                            break;
                                        case TokenType.TK_FILTER_NEAREST_MIPMAP_ANISOTROPIC:
                                            newFilter = TextureFilter.FILTER_NEAREST_MIPMAP_ANISOTROPIC;
                                            break;
                                        case TokenType.TK_FILTER_LINEAR_MIPMAP_ANISOTROPIC:
                                            newFilter = TextureFilter.FILTER_LINEAR_MIPMAP_ANISOTROPIC;
                                            break;
                                        case TokenType.TK_REPEAT_DISABLE:
                                            newRepeat = TextureRepeat.REPEAT_DISABLE;
                                            break;
                                        case TokenType.TK_REPEAT_ENABLE:
                                            newRepeat = TextureRepeat.REPEAT_ENABLE;
                                            break;

                                        default:
                                            break;
                                    }
                                    if ((newFilter != TextureFilter.FILTER_DEFAULT || newRepeat != TextureRepeat.REPEAT_DEFAULT || newHint != ShaderNode.Uniform.HintKind.HINT_NONE && newHint != ShaderNode.Uniform.HintKind.HINT_SOURCE_COLOR && newHint != ShaderNode.Uniform.HintKind.HINT_RANGE) && !IsSamplerType(type))
                                    {
                                        this.SetError(RTR("This hint is only for sampler types."));
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    if (newHint != ShaderNode.Uniform.HintKind.HINT_NONE)
                                    {
                                        if (uniform.Hint != ShaderNode.Uniform.HintKind.HINT_NONE)
                                        {
                                            if (uniform.Hint == newHint)
                                            {
                                                this.SetError(string.Format(RTR("Duplicated hint: '{0}'."), GetUniformHintName(newHint)));
                                            }
                                            else
                                            {
                                                this.SetError(string.Format(RTR("Redefinition of hint: '{0}'. The hint has already been set to '{1}'."), GetUniformHintName(newHint), GetUniformHintName(uniform.Hint)));
                                            }
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        else
                                        {
                                            uniform.Hint       = newHint;
                                            this.currentUniformHint = newHint;
                                        }
                                    }

                                    if (newFilter != TextureFilter.FILTER_DEFAULT)
                                    {
                                        if (uniform.Filter != TextureFilter.FILTER_DEFAULT)
                                        {
                                            if (uniform.Filter == newFilter)
                                            {
                                                this.SetError(string.Format(RTR("Duplicated filter mode: '{0}'."), GetTextureFilterName(newFilter)));
                                            }
                                            else
                                            {
                                                this.SetError(string.Format(RTR("Redefinition of filter mode: '{0}'. The filter mode has already been set to '{1}'."), GetTextureFilterName(newFilter), GetTextureFilterName(uniform.Filter)));
                                            }
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        else
                                        {
                                            uniform.Filter = newFilter;
                                            this.currentUniformFilter = newFilter;
                                        }
                                    }

                                    if (newRepeat != TextureRepeat.REPEAT_DEFAULT)
                                    {
                                        if (uniform.Repeat != TextureRepeat.REPEAT_DEFAULT)
                                        {
                                            if (uniform.Repeat == newRepeat)
                                            {
                                                this.SetError(string.Format(RTR("Duplicated repeat mode: '{0}'."), GetTextureRepeatName(newRepeat)));
                                            }
                                            else
                                            {
                                                this.SetError(string.Format(RTR("Redefinition of repeat mode: '{0}'. The repeat mode has already been set to '{1}'."), GetTextureRepeatName(newRepeat), GetTextureRepeatName(uniform.Repeat)));
                                            }
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                        else
                                        {
                                            uniform.Repeat = newRepeat;
                                            this.currentUniformRepeat = newRepeat;
                                        }
                                    }

                                    tk = this.GetToken();

                                } while (tk.Type == TokenType.TK_COMMA);
                            }

                            if (uniformScope == ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                            {
                                if (customInstanceIndex >= 0)
                                {
                                    uniform.InstanceIndex = customInstanceIndex;
                                }
                                else
                                {
                                    uniform.InstanceIndex = instanceIndex++;
                                    if (instanceIndex > MAX_INSTANCE_UNIFORM_INDICES)
                                    {
                                        this.SetError(string.Format(RTR("Too many '{0}' uniforms in shader, maximum supported is {2}."), "instance", MAX_INSTANCE_UNIFORM_INDICES));
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                }
                            }

                            // reset scope for next uniform

                            if (tk.Type == TokenType.TK_OP_ASSIGN)
                            {
                                if (uniform.ArraySize > 0)
                                {
                                    this.SetError(RTR("Setting default values to uniform arrays is not supported."));
                                    return Error.ERR_PARSE_ERROR;
                                }

                                var expr = this.ParseAndReduceExpression(null, constants);

                                if (expr == null)
                                {
                                    return Error.ERR_PARSE_ERROR;
                                }

                                if (expr.Type != Node.NodeType.TYPE_CONSTANT)
                                {
                                    this.SetError(RTR("Expected constant expression after '='."));
                                    return Error.ERR_PARSE_ERROR;
                                }

                                var cn = (ConstantNode)expr;

                                uniform.DefaultValue = new List<ConstantNode.ValueUnion>(cn.Values.Count);

                                if (!ConvertConstant(cn, uniform.Type, uniform.DefaultValue))
                                {
                                    this.SetError(string.Format(RTR("Can't convert constant to '{0}'."), GetDatatypeName(uniform.Type)));
                                    return Error.ERR_PARSE_ERROR;
                                }
                                tk = this.GetToken();
                            }

                            this.shader.Uniforms[name] = uniform;

                            #if DEBUG
                            if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_UNIFORM_FLAG))
                            {
                                this.usedUniforms.Add(name, new(this.tkLine));
                            }
                            #endif // DEBUG_ENABLED

                            //reset scope for next uniform
                            uniformScope = ShaderNode.Uniform.ScopeKind.SCOPE_LOCAL;

                            if (tk.Type != TokenType.TK_SEMICOLON)
                            {
                                this.SetExpectedError(";");
                                return Error.ERR_PARSE_ERROR;
                            }

                            #if DEBUG
                            this.keywordCompletionContext = ContextFlag.CF_GLOBAL_SPACE;
                            #endif // DEBUG_ENABLED
                            this.completionType = CompletionType.COMPLETION_NONE;

                            this.currentUniformHint                 = ShaderNode.Uniform.HintKind.HINT_NONE;
                            this.currentUniformFilter               = TextureFilter.FILTER_DEFAULT;
                            this.currentUniformRepeat               = TextureRepeat.REPEAT_DEFAULT;
                            this.currentUniformInstanceIndexDefined = false;
                        }
                        else
                        {
                            // varying
                            var varying = new ShaderNode.Varying
                            {
                                Type          = type,
                                Precision     = precision,
                                Interpolation = interpolation,
                                Tkpos         = prevPos,
                                ArraySize     = arraySize
                            };

                            tk = this.GetToken();
                            if (tk.Type != TokenType.TK_SEMICOLON && tk.Type != TokenType.TK_BRACKET_OPEN)
                            {
                                if (arraySize == 0)
                                {
                                    this.SetExpectedError(";", "[");
                                }
                                else
                                {
                                    this.SetExpectedError(";");
                                }
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (tk.Type == TokenType.TK_BRACKET_OPEN)
                            {
                                var error = this.ParseArraySize(null, constants, true, out var _, out arraySize, out var _);
                                varying.ArraySize = arraySize;
                                if (error != Error.OK)
                                {
                                    return error;
                                }
                                tk = this.GetToken();
                            }

                            this.shader.Varyings[name] = varying;
                            #if DEBUG
                            if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_VARYING_FLAG))
                            {
                                this.usedVaryings.Add(name, new(this.tkLine));
                            }
                            #endif // DEBUG_ENABLED
                        }
                    }
                    break;
                case TokenType.TK_UNIFORM_GROUP:
                    tk = this.GetToken();
                    if (tk.Type == TokenType.TK_IDENTIFIER)
                    {
                        this.currentUniformGroupName = tk.Text;
                        tk = this.GetToken();
                        if (tk.Type == TokenType.TK_PERIOD)
                        {
                            tk = this.GetToken();
                            if (tk.Type == TokenType.TK_IDENTIFIER)
                            {
                                this.currentUniformSubgroupName = tk.Text;
                                tk = this.GetToken();
                                if (tk.Type != TokenType.TK_SEMICOLON)
                                {
                                    this.SetExpectedError(";");
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }
                            else
                            {
                                this.SetError(RTR("Expected an uniform subgroup identifier."));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }
                        else if (tk.Type != TokenType.TK_SEMICOLON)
                        {
                            this.SetExpectedError(";", ".");
                            return Error.ERR_PARSE_ERROR;
                        }
                    }
                    else
                    {
                        if (tk.Type != TokenType.TK_SEMICOLON)
                        {
                            if (string.IsNullOrEmpty(this.currentUniformGroupName))
                            {
                                this.SetError(RTR("Expected an uniform group identifier."));
                            }
                            else
                            {
                                this.SetError(RTR("Expected an uniform group identifier or `;`."));
                            }
                            return Error.ERR_PARSE_ERROR;
                        }
                        else if (string.IsNullOrEmpty(this.currentUniformGroupName))
                        {
                            this.SetError(RTR("Group needs to be opened before."));
                            return Error.ERR_PARSE_ERROR;
                        }
                        else
                        {
                            this.currentUniformGroupName    = "";
                            this.currentUniformSubgroupName = "";
                        }
                    }
                    break;
                case TokenType.TK_SHADER_TYPE:
                    this.SetError(RTR("Shader type is already defined."));
                    return Error.ERR_PARSE_ERROR;
                default:
                    {
                        //function or constant variable

                        var isConstant = false;
                        var isStruct   = false;
                        var structName = default(string);
                        var precision  = DataPrecision.PRECISION_DEFAULT;
                        var type       = default(DataType);
                        var arraySize = 0u;

                        if (tk.Type == TokenType.TK_CONST)
                        {
                            isConstant = true;
                            tk = this.GetToken();
                        }

                        if (IsTokenPrecision(tk.Type))
                        {
                            precision = GetTokenPrecision(tk.Type);
                            tk = this.GetToken();
                        }

                        if (this.shader!.Structs.ContainsKey(tk.Text))
                        {
                            isStruct = true;
                            structName = tk.Text;
                        }
                        else
                        {
                            #if DEBUG
                            if (this.LookupNext(out next))
                            {
                                if (next.Type == TokenType.TK_UNIFORM)
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_UNIFORM_QUALIFIER;
                                }
                            }
                            #endif // DEBUG_ENABLED
                            if (!IsTokenDatatype(tk.Type))
                            {
                                this.SetError(RTR("Expected constant, function, uniform or varying."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (!IsTokenVariableDatatype(tk.Type))
                            {
                                if (isConstant)
                                {
                                    this.SetError(RTR("Invalid constant type (samplers are not allowed)."));
                                }
                                else
                                {
                                    this.SetError(RTR("Invalid function type (samplers are not allowed)."));
                                }
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        type = isStruct
                            ? DataType.TYPE_STRUCT
                            : GetTokenDatatype(tk.Type);

                        if (precision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(type, precision) != Error.OK)
                        {
                            return Error.ERR_PARSE_ERROR;
                        }

                        prevPos = this.GetTkpos();
                        tk = this.GetToken();

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                        #endif // DEBUG_ENABLED

                        var unknownSize    = false;
                        var fixedArraySize = false;

                        if (tk.Type == TokenType.TK_BRACKET_OPEN)
                        {
                            var error = this.ParseArraySize(null, constants, !isConstant, out var _, out arraySize, out unknownSize);
                            if (error != Error.OK)
                            {
                                return error;
                            }
                            fixedArraySize = true;
                            prevPos = this.GetTkpos();
                        }

                        this.SetTkpos(prevPos);

                        this.GetCompletableIdentifier(null, CompletionType.COMPLETION_MAIN_FUNCTION, out var name);

                        if (name == null)
                        {
                            if (isConstant)
                            {
                                this.SetError(RTR("Expected an identifier or '[' after type."));
                            }
                            else
                            {
                                this.SetError(RTR("Expected a function name after type."));
                            }
                            return Error.ERR_PARSE_ERROR;
                        }

                        if (this.shader.Structs.ContainsKey(name) || this.FindIdentifier(null, false, constants, name) || HasBuiltin(functions, name))
                        {
                            this.SetRedefinitionError(name);
                            return Error.ERR_PARSE_ERROR;
                        }

                        tk = this.GetToken();
                        if (tk.Type != TokenType.TK_PARENTHESIS_OPEN)
                        {
                            if (type == DataType.TYPE_VOID)
                            {
                                this.SetError(RTR("Expected '(' after function identifier."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            //variable
                            while (true)
                            {
                                var constant = new ShaderNode.Constant
                                {
                                    ArraySize   = arraySize,
                                    Index       = this.shader.Constants.Count,
                                    Initializer = null,
                                    Name        = name,
                                    Precision   = precision,
                                    Type        = isStruct ? DataType.TYPE_STRUCT : type,
                                    TypeStr     = structName!,
                                };

                                this.isConstDecl = true;

                                if (tk.Type == TokenType.TK_BRACKET_OPEN)
                                {
                                    var error = this.ParseArraySize(null, constants, false, out var _, out arraySize, out unknownSize);

                                    constant.ArraySize = arraySize;
                                    if (error != Error.OK)
                                    {
                                        return error;
                                    }
                                    tk = this.GetToken();
                                }

                                if (tk.Type == TokenType.TK_OP_ASSIGN)
                                {
                                    if (!isConstant)
                                    {
                                        this.SetError(string.Format(RTR("Global non-constant variables are not supported. Expected '{0}' keyword before constant definition."), "const"));
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    if (constant.ArraySize > 0 || unknownSize)
                                    {
                                        var fullDef = false;

                                        var decl = new VariableDeclarationNode.Declaration
                                        {
                                            Name = name,
                                            Size = constant.ArraySize
                                        };

                                        tk = this.GetToken();

                                        if (tk.Type != TokenType.TK_CURLY_BRACKET_OPEN)
                                        {
                                            if (unknownSize)
                                            {
                                                this.SetExpectedError("{");
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            fullDef = true;

                                            var precision2 = DataPrecision.PRECISION_DEFAULT;
                                            if (IsTokenPrecision(tk.Type))
                                            {
                                                precision2 = GetTokenPrecision(tk.Type);
                                                tk = this.GetToken();
                                                if (!IsTokenNonvoidDatatype(tk.Type))
                                                {
                                                    this.SetError(RTR("Expected data type after precision modifier."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                            }

                                            var structName2 = default(string);
                                            var type2       = default(DataType);

                                            if (this.shader.Structs.ContainsKey(tk.Text))
                                            {
                                                type2 = DataType.TYPE_STRUCT;
                                                structName2 = tk.Text;
                                            }
                                            else
                                            {
                                                if (!IsTokenVariableDatatype(tk.Type))
                                                {
                                                    this.SetError(RTR("Invalid data type for the array."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                                type2 = GetTokenDatatype(tk.Type);
                                            }

                                            var arraySize2 = 0u;
                                            tk = this.GetToken();

                                            if (tk.Type == TokenType.TK_BRACKET_OPEN)
                                            {
                                                var error = this.ParseArraySize(null, constants, false, out var _, out arraySize2, out var isUnknownSize);
                                                if (error != Error.OK)
                                                {
                                                    return error;
                                                }
                                                if (isUnknownSize)
                                                {
                                                    arraySize2 = constant.ArraySize;
                                                }
                                                tk = this.GetToken();
                                            }
                                            else
                                            {
                                                this.SetExpectedError("[");
                                                return Error.ERR_PARSE_ERROR;
                                            }

                                            if (constant.Precision != precision2 || constant.Type != type2 || structName != structName2 || constant.ArraySize != arraySize2)
                                            {
                                                var from = "";
                                                if (type2 == DataType.TYPE_STRUCT)
                                                {
                                                    from += structName2;
                                                }
                                                else
                                                {
                                                    if (precision2 != DataPrecision.PRECISION_DEFAULT)
                                                    {
                                                        from += GetPrecisionName(precision2);
                                                        from += " ";
                                                    }
                                                    from += GetDatatypeName(type2);
                                                }
                                                from += "[";
                                                from += arraySize2;
                                                from += "]'";

                                                var to = "";
                                                if (type == DataType.TYPE_STRUCT)
                                                {
                                                    to += structName;
                                                }
                                                else
                                                {
                                                    if (precision != DataPrecision.PRECISION_DEFAULT)
                                                    {
                                                        to += GetPrecisionName(precision);
                                                        to += " ";
                                                    }
                                                    to += GetDatatypeName(type);
                                                }
                                                to += "[";
                                                to += constant.ArraySize;
                                                to += "]'";

                                                this.SetError(string.Format(RTR("Cannot convert from '{0}' to '{1}'."), from, to));
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                        }

                                        var curly = tk.Type == TokenType.TK_CURLY_BRACKET_OPEN;

                                        if (unknownSize)
                                        {
                                            if (!curly)
                                            {
                                                this.SetExpectedError("{");
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                        }
                                        else
                                        {
                                            if (fullDef)
                                            {
                                                if (curly)
                                                {
                                                    this.SetExpectedError("(");
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                            }
                                        }

                                        if (tk.Type == TokenType.TK_PARENTHESIS_OPEN || curly)
                                        {
                                            // initialization
                                            while (true)
                                            {
                                                var n = this.ParseAndReduceExpression(null, constants);
                                                if (n == null)
                                                {
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                if (n.Type == Node.NodeType.TYPE_OPERATOR && ((OperatorNode)n).Op == Operator.OP_CALL)
                                                {
                                                    this.SetError(RTR("Expected constant expression."));
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                if (!this.CompareDatatypes(constant.Type, structName!, 0u, n.GetDatatype(), n.GetDatatypeName()!, 0u))
                                                {
                                                    return Error.ERR_PARSE_ERROR;
                                                }

                                                tk = this.GetToken();
                                                if (tk.Type == TokenType.TK_COMMA)
                                                {
                                                    decl.Initializer.Add(n);
                                                    continue;
                                                }
                                                else if (!curly && tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
                                                {
                                                    decl.Initializer.Add(n);
                                                    break;
                                                }
                                                else if (curly && tk.Type == TokenType.TK_CURLY_BRACKET_CLOSE)
                                                {
                                                    decl.Initializer.Add(n);
                                                    break;
                                                }
                                                else
                                                {
                                                    if (curly)
                                                    {
                                                        this.SetExpectedError("}", ",");
                                                    }
                                                    else
                                                    {
                                                        this.SetExpectedError(")", ",");
                                                    }
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                            }

                                            if (unknownSize)
                                            {
                                                decl.Size = (uint)decl.Initializer.Count;
                                                constant.ArraySize = (uint)decl.Initializer.Count;
                                            }
                                            else if (decl.Initializer.Count != constant.ArraySize)
                                            {
                                                this.SetError(RTR("Array size mismatch."));
                                                return Error.ERR_PARSE_ERROR;
                                            }
                                        }

                                        arraySize = constant.ArraySize;

                                        var expr = new ConstantNode
                                        {
                                            Datatype          = constant.Type,
                                            StructName        = constant.TypeStr,
                                            ArraySize         = constant.ArraySize,
                                            ArrayDeclarations = new[] { decl }
                                        };

                                        constant.Initializer = expr;
                                    }
                                    else
                                    {
                                        #if DEBUG
                                        if (constant.Type == DataType.TYPE_BOOL)
                                        {
                                            this.keywordCompletionContext = ContextFlag.CF_BOOLEAN;
                                        }
                                        #endif // DEBUG_ENABLED

                                        //variable created with assignment! must parse an expression
                                        var expr = this.ParseAndReduceExpression(null, constants);
                                        if (expr == null)
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }

                                        #if DEBUG
                                        if (constant.Type == DataType.TYPE_BOOL)
                                        {
                                            this.keywordCompletionContext = ContextFlag.CF_GLOBAL_SPACE;
                                        }
                                        #endif // DEBUG_ENABLED

                                        if (expr.Type == Node.NodeType.TYPE_OPERATOR && ((OperatorNode)expr).Op == Operator.OP_CALL)
                                        {
                                            var op = (OperatorNode)expr;

                                            for (var i = 1; i < op.Arguments.Count; i++)
                                            {
                                                if (!this.CheckNodeConstness(op.Arguments[i]))
                                                {
                                                    this.SetError(string.Format(RTR("Expected constant expression for argument {0} of function call after '='."), i - 1));
                                                    return Error.ERR_PARSE_ERROR;
                                                }
                                            }
                                        }

                                        constant.Initializer = (ConstantNode)expr;

                                        if (!this.CompareDatatypes(type, structName!, 0, expr.GetDatatype(), expr.GetDatatypeName()!, expr.GetArraySize()))
                                        {
                                            return Error.ERR_PARSE_ERROR;
                                        }
                                    }
                                    tk = this.GetToken();
                                }
                                else
                                {
                                    if (constant.ArraySize > 0 || unknownSize)
                                    {
                                        this.SetError(RTR("Expected array initialization."));
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                    else
                                    {
                                        this.SetError(RTR("Expected initialization of constant."));
                                        return Error.ERR_PARSE_ERROR;
                                    }
                                }

                                this.shader.Constants[name] = constant;
                                this.shader.Vconstants.Add(constant.Name, constant);

                                #if DEBUG
                                if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_CONSTANT_FLAG))
                                {
                                    this.usedConstants.Add(name, new(this.tkLine));
                                }
                                #endif // DEBUG_ENABLED

                                if (tk.Type == TokenType.TK_COMMA)
                                {
                                    tk = this.GetToken();
                                    if (tk.Type != TokenType.TK_IDENTIFIER)
                                    {
                                        this.SetError(RTR("Expected an identifier after type."));
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    name = tk.Text;
                                    if (this.FindIdentifier(null, false, constants, name))
                                    {
                                        this.SetRedefinitionError(name);
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    if (HasBuiltin(functions, name))
                                    {
                                        this.SetRedefinitionError(name);
                                        return Error.ERR_PARSE_ERROR;
                                    }

                                    tk = this.GetToken();

                                    if (!fixedArraySize)
                                    {
                                        arraySize = 0;
                                    }
                                    unknownSize = false;

                                }
                                else if (tk.Type == TokenType.TK_SEMICOLON)
                                {
                                    this.isConstDecl = false;
                                    break;
                                }
                                else
                                {
                                    this.SetExpectedError(",", ";");
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            break;
                        }

                        var builtins = new FunctionInfo();
                        if (functions.TryGetValue(name, out var namedFunctions))
                        {
                            builtins = namedFunctions;
                        }

                        if (functions.TryGetValue("global", out var globalFunctions))
                        {
                            // Adds global variables: 'TIME'
                            foreach (var item in globalFunctions.BuiltIns)
                            {
                                builtins.BuiltIns[item.Key] = item.Value;
                            }
                        }

                        if (functions.TryGetValue("constants", out var constantsFunctions))
                        {
                            // Adds global constants: 'PI', 'TAU', 'E'
                            foreach (var item in constantsFunctions.BuiltIns)
                            {
                                builtins.BuiltIns[item.Key] = item.Value;
                            }
                        }

                        if (this.shader.Functions.TryGetValue(name, out var fn1) && !fn1.Callable)
                        {
                            this.SetRedefinitionError(name);
                            return Error.ERR_PARSE_ERROR;
                        }

                        var funcNode = this.AllocNode<FunctionNode>();

                        var function = new ShaderNode.Function
                        {
                            Callable     = !functions.ContainsKey(name),
                            Name         = name,
                            FunctionNode = funcNode,
                            Index        = this.shader.Functions.Count,
                        };

                        this.shader.Functions.Add(name, function);

                        funcNode.Name             = name;
                        funcNode.ReturnType       = type;
                        funcNode.ReturnStructName = structName!;
                        funcNode.ReturnPrecision  = precision;
                        funcNode.ReturnArraySize  = arraySize;

                        if (functions.TryGetValue(name, out var fn2))
                        {
                            funcNode.CanDiscard = fn2.CanDiscard;
                        }
                        else
                        {
                            #if DEBUG
                            if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_FUNCTION_FLAG))
                            {
                                this.usedFunctions.Add(name, new(this.tkLine));
                            }
                            #endif // DEBUG_ENABLED
                        }

                        funcNode.Body = this.AllocNode<BlockNode>();
                        funcNode.Body.ParentFunction = funcNode;

                        tk = this.GetToken();

                        while (true)
                        {
                            if (tk.Type == TokenType.TK_PARENTHESIS_CLOSE)
                            {
                                break;
                            }

                            #if DEBUG
                            this.keywordCompletionContext = ContextFlag.CF_CONST_KEYWORD | ContextFlag.CF_FUNC_DECL_PARAM_SPEC | ContextFlag.CF_PRECISION_MODIFIER | ContextFlag.CF_FUNC_DECL_PARAM_TYPE; // eg. const in mediump float

                            if (this.LookupNext(out next))
                            {
                                if (next.Type == TokenType.TK_CONST)
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                                }
                                else if (IsTokenArgQual(next.Type))
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_CONST_KEYWORD;
                                }
                                else if (IsTokenPrecision(next.Type))
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_CONST_KEYWORD | ContextFlag.CF_FUNC_DECL_PARAM_SPEC | ContextFlag.CF_FUNC_DECL_PARAM_TYPE;
                                }
                                else if (IsTokenDatatype(next.Type))
                                {
                                    this.keywordCompletionContext = ContextFlag.CF_CONST_KEYWORD | ContextFlag.CF_FUNC_DECL_PARAM_SPEC | ContextFlag.CF_PRECISION_MODIFIER;
                                }
                            }
                            #endif // DEBUG_ENABLED

                            var paramIsConst = false;
                            if (tk.Type == TokenType.TK_CONST)
                            {
                                paramIsConst = true;

                                tk = this.GetToken();

                                #if DEBUG
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_CONST_KEYWORD))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_CONST_KEYWORD;
                                }

                                if (this.LookupNext(out next))
                                {
                                    if (IsTokenArgQual(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                                    }
                                    else if (IsTokenPrecision(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_FUNC_DECL_PARAM_SPEC | ContextFlag.CF_FUNC_DECL_PARAM_TYPE;
                                    }
                                    else if (IsTokenDatatype(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_FUNC_DECL_PARAM_SPEC | ContextFlag.CF_PRECISION_MODIFIER;
                                    }
                                }
                                #endif // DEBUG_ENABLED
                            }

                            var paramQualifier = ArgumentQualifier.ARGUMENT_QUALIFIER_IN;

                            if (IsTokenArgQual(tk.Type))
                            {
                                var error = false;
                                switch (tk.Type)
                                {
                                    case TokenType.TK_ARG_IN:
                                        paramQualifier = ArgumentQualifier.ARGUMENT_QUALIFIER_IN;
                                        break;
                                    case TokenType.TK_ARG_OUT:
                                        if (paramIsConst)
                                        {
                                            this.SetError(string.Format(RTR("The '{0}' qualifier cannot be used within a function parameter declared with '{1}'."), "out", "const"));
                                            error = true;
                                        }
                                        paramQualifier = ArgumentQualifier.ARGUMENT_QUALIFIER_OUT;
                                        break;
                                    case TokenType.TK_ARG_INOUT:
                                        if (paramIsConst)
                                        {
                                            this.SetError(string.Format(RTR("The '{0}' qualifier cannot be used within a function parameter declared with '{1}'."), "inout", "const"));
                                            error = true;
                                        }
                                        paramQualifier = ArgumentQualifier.ARGUMENT_QUALIFIER_INOUT;
                                        break;
                                    default:
                                        error = true;
                                        break;
                                }

                                tk = this.GetToken();

                                #if DEBUG
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_CONST_KEYWORD))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_CONST_KEYWORD;
                                }
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_FUNC_DECL_PARAM_SPEC))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_FUNC_DECL_PARAM_SPEC;
                                }

                                if (this.LookupNext(out next))
                                {
                                    if (IsTokenPrecision(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_FUNC_DECL_PARAM_TYPE;
                                    }
                                    else if (IsTokenDatatype(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_PRECISION_MODIFIER;
                                    }
                                }
                                #endif // DEBUG_ENABLED

                                if (error)
                                {
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            var paramType       = default(DataType);
                            var paramName       = default(string);
                            var paramStructName = default(string);
                            var paramPrecision  = DataPrecision.PRECISION_DEFAULT;
                            var argArraySize    = 0u;

                            if (IsTokenPrecision(tk.Type))
                            {
                                paramPrecision = GetTokenPrecision(tk.Type);
                                tk = this.GetToken();
                                #if DEBUG
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_CONST_KEYWORD))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_CONST_KEYWORD;
                                }
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_FUNC_DECL_PARAM_SPEC))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_FUNC_DECL_PARAM_SPEC;
                                }
                                if (this.keywordCompletionContext.HasFlag(ContextFlag.CF_PRECISION_MODIFIER))
                                {
                                    this.keywordCompletionContext ^= ContextFlag.CF_PRECISION_MODIFIER;
                                }

                                if (this.LookupNext(out next))
                                {
                                    if (IsTokenDatatype(next.Type))
                                    {
                                        this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                                    }
                                }
                                #endif // DEBUG_ENABLED
                            }

                            isStruct = false;

                            if (this.shader.Structs.ContainsKey(tk.Text))
                            {
                                isStruct = true;
                                paramStructName = tk.Text;

                                #if DEBUG
                                if (this.checkWarnings && this.HAS_WARNING(ShaderWarning.CodeFlags.UNUSED_STRUCT_FLAG) && this.usedStructs.TryGetValue(paramStructName, out var value))
                                {
                                    value.Used = true;
                                }
                                #endif // DEBUG_ENABLED
                            }

                            if (!isStruct && !IsTokenDatatype(tk.Type))
                            {
                                this.SetError(RTR("Expected a valid data type for argument."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            if (paramQualifier == ArgumentQualifier.ARGUMENT_QUALIFIER_OUT || paramQualifier == ArgumentQualifier.ARGUMENT_QUALIFIER_INOUT)
                            {
                                if (IsSamplerType(GetTokenDatatype(tk.Type)))
                                {
                                    this.SetError(RTR("Opaque types cannot be output parameters."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            if (isStruct)
                            {
                                paramType = DataType.TYPE_STRUCT;
                            }
                            else
                            {
                                paramType = GetTokenDatatype(tk.Type);
                                if (paramType == DataType.TYPE_VOID)
                                {
                                    this.SetError(RTR("Void type not allowed as argument."));
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            if (paramPrecision != DataPrecision.PRECISION_DEFAULT && this.ValidatePrecision(paramType, paramPrecision) != Error.OK)
                            {
                                return Error.ERR_PARSE_ERROR;
                            }

                            #if DEBUG
                            this.keywordCompletionContext = ContextFlag.CF_UNSPECIFIED;
                            #endif // DEBUG_ENABLED

                            tk = this.GetToken();

                            if (tk.Type == TokenType.TK_BRACKET_OPEN)
                            {
                                var error = this.ParseArraySize(null, constants, true, out var _, out argArraySize, out var _);

                                if (error != Error.OK)
                                {
                                    return error;
                                }

                                tk = this.GetToken();
                            }
                            if (tk.Type != TokenType.TK_IDENTIFIER)
                            {
                                this.SetError(RTR("Expected an identifier for argument name."));
                                return Error.ERR_PARSE_ERROR;
                            }

                            paramName = tk.Text;

                            if (this.FindIdentifier(funcNode.Body, false, builtins, paramName, out var _, out var itype))
                            {
                                if (itype != IdentifierType.IDENTIFIER_FUNCTION)
                                {
                                    this.SetRedefinitionError(paramName);
                                    return Error.ERR_PARSE_ERROR;
                                }
                            }

                            if (HasBuiltin(functions, paramName))
                            {
                                this.SetRedefinitionError(paramName);
                                return Error.ERR_PARSE_ERROR;
                            }

                            var arg = new FunctionNode.Argument
                            {
                                Index             = funcNode.Arguments.Count,
                                IsConst           = paramIsConst,
                                Name              = paramName,
                                Precision         = paramPrecision,
                                Qualifier         = paramQualifier,
                                TexArgumentCheck  = false,
                                TexArgumentFilter = TextureFilter.FILTER_DEFAULT,
                                TexArgumentRepeat = TextureRepeat.REPEAT_DEFAULT,
                                TexBuiltinCheck   = false,
                                Type              = paramType,
                                TypeStr           = paramStructName!,
                            };

                            tk = this.GetToken();
                            if (tk.Type == TokenType.TK_BRACKET_OPEN)
                            {
                                var error = this.ParseArraySize(null, constants, true, out var _, out argArraySize, out var _);
                                if (error != Error.OK)
                                {
                                    return error;
                                }
                                tk = this.GetToken();
                            }

                            arg.ArraySize = argArraySize;
                            funcNode.Arguments.Add(arg.Name, arg);

                            if (tk.Type == TokenType.TK_COMMA)
                            {
                                tk = this.GetToken();
                                //do none and go on
                            }
                            else if (tk.Type != TokenType.TK_PARENTHESIS_CLOSE)
                            {
                                this.SetExpectedError(",", ")");
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        if (functions.ContainsKey(name))
                        {
                            //if one of the core functions, make sure they are of the correct form
                            if (funcNode.Arguments.Count != 0)
                            {
                                this.SetError(string.Format(RTR("Function '{0}' expects no arguments."), name));
                                return Error.ERR_PARSE_ERROR;
                            }
                            if (funcNode.ReturnType != DataType.TYPE_VOID)
                            {
                                this.SetError(string.Format(RTR("Function '{0}' must be of '{1}' return type."), name, "void"));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }

                        //all good let's parse inside the function!
                        tk = this.GetToken();
                        if (tk.Type != TokenType.TK_CURLY_BRACKET_OPEN)
                        {
                            this.SetError(RTR("Expected a '{' to begin function."));
                            return Error.ERR_PARSE_ERROR;
                        }

                        this.currentFunction = name;

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_BLOCK;
                        #endif // DEBUG_ENABLED

                        var err = this.ParseBlock(funcNode.Body, builtins);
                        if (err != default)
                        {
                            return err;
                        }

                        #if DEBUG
                        this.keywordCompletionContext = ContextFlag.CF_GLOBAL_SPACE;
                        #endif // DEBUG_ENABLED

                        if (funcNode.ReturnType != DataType.TYPE_VOID)
                        {
                            var block = funcNode.Body;
                            if (this.FindLastFlowOpInBlock(block, FlowOperation.FLOW_OP_RETURN) != Error.OK)
                            {
                                this.SetError(string.Format(RTR("Expected at least one '{0}' statement in a non-void function."), "return"));
                                return Error.ERR_PARSE_ERROR;
                            }
                        }
                        this.currentFunction = null;
                    }
                    break;
            }

            tk = this.GetToken();
        }

        #if DEBUG
        if (checkDeviceLimitWarnings && uniformBufferExceededLine != -1)
        {
            this.AddWarning(ShaderWarning.Code.DEVICE_LIMIT_EXCEEDED, uniformBufferExceededLine, RTR("uniform buffer"), new object[] { uniformBufferSize, maxUniformBufferSize });
        }
        #endif // DEBUG_ENABLED
        return Error.OK;
    }

    private bool PropagateFunctionCallSamplerBuiltinReference(string name, int argument, string builtin) => throw new NotImplementedException();
    private bool PropagateFunctionCallSamplerUniformSettings(string name, int argument, TextureFilter filter, TextureRepeat repeat) => throw new NotImplementedException();
    private Node ReduceExpression(BlockNode? block, Node node)
    {
        if (node.Type != Node.NodeType.TYPE_OPERATOR)
        {
            return node;
        }

        //for now only reduce simple constructors
        var op = (OperatorNode)node;

        if (op.Op == Operator.OP_CONSTRUCT)
        {
            if (ERR_FAIL_COND_V(op.Arguments[0].Type != Node.NodeType.TYPE_VARIABLE))
            {
                return node;
            }

            var type        = op.GetDatatype();
            var @base       = GetScalarType(type);
            var cardinality = GetCardinality(type);

            var values = new List<ConstantNode.ValueUnion>();

            for (var i = 1; i < op.Arguments.Count; i++)
            {
                op.Arguments[i] = this.ReduceExpression(block, op.Arguments[i]);
                if (op.Arguments[i].Type == Node.NodeType.TYPE_CONSTANT)
                {
                    var cn = (ConstantNode)op.Arguments[i];

                    if (GetScalarType(cn.Datatype) == @base)
                    {
                        for (var j = 0; j < cn.Values.Count; j++)
                        {
                            values.Add(cn.Values[j]);
                        }
                    }
                    else if (GetScalarType(cn.Datatype) == cn.Datatype)
                    {
                        var v = new ConstantNode.ValueUnion();

                        if (!ConvertConstant(cn, @base, v))
                        {
                            return node;
                        }

                        values.Add(v);
                    }
                    else
                    {
                        return node;
                    }
                }
                else
                {
                    return node;
                }
            }

            if (values.Count == 1)
            {
                if (type >= DataType.TYPE_MAT2 && type <= DataType.TYPE_MAT4)
                {
                    var value = values[0];
                    var zero  = new ConstantNode.ValueUnion
                    {
                        Real = 0.0f,
                    };

                    var size = 2 + (type - DataType.TYPE_MAT2);

                    values.Clear();

                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            values.Add(i == j ? value : zero);
                        }
                    }
                }
                else
                {
                    var value = values[0];
                    for (var i = 1; i < cardinality; i++)
                    {
                        values.Add(value);
                    }
                }
            }
            else if (values.Count != cardinality)
            {
                ERR_PRINT("Failed to reduce expression, values and cardinality mismatch.");
                return node;
            }

            var constant = this.AllocNode<ConstantNode>();

            constant.Datatype = op.GetDatatype();
            constant.Values = values;

            return constant;
        }
        else if (op.Op == Operator.OP_NEGATE)
        {
            op.Arguments[0] = this.ReduceExpression(block, op.Arguments[0]);

            if (op.Arguments[0].Type == Node.NodeType.TYPE_CONSTANT)
            {
                var cn = (ConstantNode)op.Arguments[0];

                var @base = GetScalarType(cn.Datatype);

                var values = new List<ConstantNode.ValueUnion>();

                for (var i = 0; i < cn.Values.Count; i++)
                {
                    var nv = new ConstantNode.ValueUnion();
                    switch (@base)
                    {
                        case DataType.TYPE_BOOL:
                            nv.Boolean = !cn.Values[i].Boolean;
                            break;
                        case DataType.TYPE_INT:
                            nv.Sint = -cn.Values[i].Sint;
                            break;
                        case DataType.TYPE_UINT:
                            // Intentionally wrap the unsigned int value, because GLSL does.
                            nv.Uint = 0 - cn.Values[i].Uint;
                            break;
                        case DataType.TYPE_FLOAT:
                                nv.Real = -cn.Values[i].Real;
                            break;
                        default:
                            break;
                    }

                    values.Add(nv);
                }

                cn.Values = values;
                return cn;
            }
        }

        return node;
    }

    private void SetError(string? str)
    {
        if (this.errorSet)
        {
            return;
        }

        this.errorLine                 = this.tkLine;
        this.errorSet                  = true;
        this.errorStr                  = str;
        this.includePositions[^1].Line = this.tkLine;
    }

    private void SetExpectedAfterError(string what, string after) => throw new NotImplementedException();
    private void SetExpectedError(string first, string second) => throw new NotImplementedException();
    private void SetExpectedError(string what) => throw new NotImplementedException();
    private void SetParsingError() => throw new NotImplementedException();
    private void SetRedefinitionError(string what) => throw new NotImplementedException();

    private void SetTkpos(TkPos pos)
    {
        this.charIdx = pos.CharIdx;
        this.tkLine  = pos.TkLine;
    }

    private bool ValidateAssign(Node node, FunctionInfo functionInfo) => throw new NotImplementedException();

    private bool ValidateAssign(Node node, FunctionInfo functionInfo, out string? message)
    {
        message = default;

        if (node.Type == Node.NodeType.TYPE_OPERATOR)
        {
            var op = (OperatorNode)node;

            if (op.Op == Operator.OP_INDEX)
            {
                return this.ValidateAssign(op.Arguments[0], functionInfo, out message);

            }
            else if (this.IsOperatorAssign(op.Op))
            {
                //chained assignment
                return this.ValidateAssign(op.Arguments[1], functionInfo, out message);

            }
            else if (op.Op == Operator.OP_CALL)
            {
                message = RTR("Assignment to function.");
                return false;
            }

        }
        else if (node.Type == Node.NodeType.TYPE_MEMBER)
        {
            var member = (MemberNode)node;

            if (member.HasSwizzlingDuplicates)
            {
                message = RTR("Swizzling assignment contains duplicates.");
                return false;
            }

            return this.ValidateAssign(member.Owner!, functionInfo, out message);

        }
        else if (node.Type == Node.NodeType.TYPE_VARIABLE)
        {
            var var = (VariableNode)node;

            if (this.shader!.Uniforms.ContainsKey(var.Name))
            {
                message = RTR("Assignment to uniform.");
                return false;
            }

            if (this.shader.Constants.ContainsKey(var.Name) || var.IsConst)
            {
                message = RTR("Constants cannot be modified.");
                return false;
            }

            if (!(functionInfo.BuiltIns.ContainsKey(var.Name) && functionInfo.BuiltIns[var.Name].Constant))
            {
                return true;
            }
        }
        else if (node.Type == Node.NodeType.TYPE_ARRAY)
        {
            var arr = (ArrayNode)node;

            if (this.shader!.Constants.ContainsKey(arr.Name) || arr.IsConst)
            {
                message = RTR("Constants cannot be modified.");
                return false;
            }

            return true;
        }

        message = RTR("Assignment to constant expression.");
        return false;
    }

    private bool ValidateFunctionCall(BlockNode? block, FunctionInfo functionInfo, OperatorNode func, out DataType retType, out string? retTypeStr, out bool isCustomFunction)
    {
        retType          = default;
        retTypeStr       = default;
        isCustomFunction = default;

        if (ERR_FAIL_COND_V(func.Op != Operator.OP_CALL && func.Op != Operator.OP_CONSTRUCT))
        {
            return false;
        }

        var args  = new List<DataType>();
        var args2 = new List<string>();
        var args3 = new List<uint>();

        if (ERR_FAIL_COND_V(func.Arguments[0].Type != Node.NodeType.TYPE_VARIABLE))
        {
            return false;
        }

        var name = ((VariableNode)func.Arguments[0]).Name;

        for (var i = 1; i < func.Arguments.Count; i++)
        {
            args.Add(func.Arguments[i].GetDatatype());
            args2.Add(func.Arguments[i].GetDatatypeName());
            args3.Add(func.Arguments[i].GetArraySize());
        }

        var argcount = args.Count;

        if (functionInfo.StageFunctions.TryGetValue(name, out var sf))
        {
            //stage based function
            if (argcount != sf.Arguments.Length)
            {
                this.SetError(string.Format(RTR("Invalid number of arguments when calling stage function '{0}', which expects {1} arguments."), name, sf.Arguments.Length));
                return false;
            }
            //validate arguments
            for (var i = 0; i < argcount; i++)
            {
                if (args[i] != sf.Arguments[i].Type)
                {
                    this.SetError(string.Format(RTR("Invalid argument type when calling stage function '{0}', type expected is '{1}'."), name, GetDatatypeName(sf.Arguments[i].Type)));
                    return false;
                }
            }

            retType    = sf.ReturnType;
            retTypeStr = "";

            return true;
        }

        var failedBuiltin      = false;
        var unsupportedBuiltin = false;
        var builtinIdx         = 0;

        if (argcount <= 4)
        {
            // test builtins
            var idx = 0;

            while (!string.IsNullOrEmpty(builtinFuncDefs[idx].Name))
            {
                if (this.completionClass != builtinFuncDefs[idx].Tag)
                {
                    idx++;
                    continue;
                }

                if (name == builtinFuncDefs[idx].Name)
                {
                    failedBuiltin = true;
                    var fail = false;
                    for (var i = 0; i < argcount; i++)
                    {
                        if (func.Arguments[i + 1].Type == Node.NodeType.TYPE_ARRAY)
                        {
                            var anode = (ArrayNode)func.Arguments[i + 1];
                            if (anode.CallExpression == null && !anode.IsIndexed)
                            {
                                fail = true;
                                break;
                            }
                        }
                        if (GetScalarType(args[i]) == args[i] && func.Arguments[i + 1].Type == Node.NodeType.TYPE_CONSTANT && ConvertConstant((ConstantNode)func.Arguments[i + 1], builtinFuncDefs[idx].Args[i]))
                        {
                            //all good, but needs implicit conversion later
                        }
                        else if (args[i] != builtinFuncDefs[idx].Args[i])
                        {
                            fail = true;
                            break;
                        }
                    }

                    if (!fail)
                    {
                        if (RS.Singleton.IsLowEnd)
                        {
                            if (builtinFuncDefs[idx].HighEnd)
                            {
                                fail               = true;
                                unsupportedBuiltin = true;
                                builtinIdx         = idx;
                            }
                        }
                    }

                    if (!fail && argcount < 4 && builtinFuncDefs[idx].Args[argcount] != DataType.TYPE_VOID)
                    {
                        fail = true; //make sure the number of arguments matches
                    }

                    if (!fail)
                    {
                        var constargIdx = 0;
                        while (!string.IsNullOrEmpty(builtinFuncConstArgs[constargIdx].Name))
                        {
                            if (name == builtinFuncConstArgs[constargIdx].Name)
                            {
                                var arg = builtinFuncConstArgs[constargIdx].Arg + 1;
                                if (func.Arguments.Count <= arg)
                                {
                                    break;
                                }

                                var min = builtinFuncConstArgs[constargIdx].Min;
                                var max = builtinFuncConstArgs[constargIdx].Max;

                                var error = false;
                                if (func.Arguments[arg].Type == Node.NodeType.TYPE_VARIABLE)
                                {
                                    var vn = (VariableNode)func.Arguments[arg];

                                    this.FindIdentifier(block, false, functionInfo, vn.Name, out var _, out var _, out var isConst, out var _, out var _, out var value);

                                    if (!isConst || value!.Sint < min || value.Sint > max)
                                    {
                                        error = true;
                                    }
                                }
                                else
                                {
                                    if (func.Arguments[arg].Type == Node.NodeType.TYPE_CONSTANT)
                                    {
                                        var cn = (ConstantNode)func.Arguments[arg];

                                        if (cn.GetDatatype() == DataType.TYPE_INT && cn.Values.Count == 1)
                                        {
                                            var value = cn.Values[0].Sint;

                                            if (value < min || value > max)
                                            {
                                                error = true;
                                            }
                                        }
                                        else
                                        {
                                            error = true;
                                        }
                                    }
                                    else
                                    {
                                        error = true;
                                    }
                                }
                                if (error)
                                {
                                    this.SetError(string.Format(RTR("Expected integer constant within [{0}..{1}] range."), min, max));
                                    return false;
                                }
                            }
                            constargIdx++;
                        }

                        //make sure its not an out argument used in the wrong way
                        var outargIdx = 0;
                        while (!string.IsNullOrEmpty(builtinFuncOutArgs[outargIdx].Name))
                        {
                            if (name == builtinFuncOutArgs[outargIdx].Name)
                            {
                                for (var arg = 0; arg < BuiltinFuncOutArgs.MAX_ARGS; arg++)
                                {
                                    var argIdx = builtinFuncOutArgs[outargIdx].Arguments[arg];

                                    if (argIdx == -1)
                                    {
                                        break;
                                    }

                                    if (argIdx < argcount)
                                    {
                                        if (func.Arguments[argIdx + 1].Type != Node.NodeType.TYPE_VARIABLE && func.Arguments[argIdx + 1].Type != Node.NodeType.TYPE_MEMBER && func.Arguments[argIdx + 1].Type != Node.NodeType.TYPE_ARRAY)
                                        {
                                            this.SetError(string.Format(RTR("Argument {0} of function '{1}' is not a variable, array, or member."), argIdx + 1, name));
                                            return false;
                                        }

                                        if (func.Arguments[argIdx + 1].Type == Node.NodeType.TYPE_ARRAY)
                                        {
                                            var mn = (ArrayNode)func.Arguments[argIdx + 1];
                                            if (mn.IsConst)
                                            {
                                                fail = true;
                                            }
                                        }
                                        else if (func.Arguments[argIdx + 1].Type == Node.NodeType.TYPE_MEMBER)
                                        {
                                            var mn = (MemberNode)func.Arguments[argIdx + 1];
                                            if (mn.BasetypeConst)
                                            {
                                                fail = true;
                                            }
                                        }
                                        else
                                        {
                                            // TYPE_VARIABLE
                                            var vn = (VariableNode)func.Arguments[argIdx + 1];
                                            if (vn.IsConst)
                                            {
                                                fail = true;
                                            }
                                            else
                                            {
                                                var varname = vn.Name;

                                                if (this.shader!.Uniforms.ContainsKey(varname))
                                                {
                                                    fail = true;
                                                }
                                                else
                                                {
                                                    if (this.shader.Varyings.ContainsKey(varname))
                                                    {
                                                        this.SetError(string.Format(RTR("Varyings cannot be passed for the '{0}' parameter."), "out"));
                                                        return false;
                                                    }
                                                    if (functionInfo.BuiltIns.TryGetValue(varname, out var info))
                                                    {
                                                        if (info.Constant)
                                                        {
                                                            fail = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (fail)
                                        {
                                            this.SetError(string.Format(RTR("A constant value cannot be passed for the '{0}' parameter."), "out"));
                                            return false;
                                        }

                                        var varName = "";
                                        if (func.Arguments[argIdx + 1].Type == Node.NodeType.TYPE_ARRAY)
                                        {
                                            varName = ((ArrayNode)func.Arguments[argIdx + 1]).Name;
                                        }
                                        else if (func.Arguments[argIdx + 1].Type == Node.NodeType.TYPE_MEMBER)
                                        {
                                            var n = ((MemberNode)func.Arguments[argIdx + 1]).Owner!;

                                            while (n.Type == Node.NodeType.TYPE_MEMBER)
                                            {
                                                n = ((MemberNode)n).Owner!;
                                            }

                                            if (n.Type != Node.NodeType.TYPE_VARIABLE && n.Type != Node.NodeType.TYPE_ARRAY)
                                            {
                                                this.SetError(string.Format(RTR("Argument %d of function '{0}' is not a variable, array, or member."), argIdx + 1, name));
                                                return false;
                                            }
                                            if (n.Type == Node.NodeType.TYPE_VARIABLE)
                                            {
                                                varName = ((VariableNode)n).Name;
                                            }
                                            else
                                            {
                                                // TYPE_ARRAY
                                                varName = ((ArrayNode)n).Name;
                                            }
                                        }
                                        else
                                        {
                                            // TYPE_VARIABLE
                                            varName = ((VariableNode)func.Arguments[argIdx + 1]).Name;
                                        }

                                        var b     = block;
                                        var valid = false;

                                        while (b != null)
                                        {
                                            if (b.Variables.ContainsKey(varName) || functionInfo.BuiltIns.ContainsKey(varName))
                                            {
                                                valid = true;
                                                break;
                                            }

                                            if (b.ParentFunction != null)
                                            {
                                                if (b.ParentFunction.Arguments.ContainsKey(varName))
                                                {
                                                    valid = true;
                                                    break;
                                                }
                                            }

                                            b = b.ParentBlock;
                                        }

                                        if (!valid)
                                        {
                                            this.SetError(string.Format(RTR("Argument {0} of function '{1}' can only take a local variable, array, or member."), argIdx + 1, name));
                                            return false;
                                        }
                                    }
                                }
                            }

                            outargIdx++;
                        }

                        //implicitly convert values if possible
                        for (var i = 0; i < argcount; i++)
                        {
                            if (GetScalarType(args[i]) != args[i] || args[i] == builtinFuncDefs[idx].Args[i] || func.Arguments[i + 1].Type != Node.NodeType.TYPE_CONSTANT)
                            {
                                //can't do implicit conversion here
                                continue;
                            }

                            //this is an implicit conversion
                            var constant   = (ConstantNode)func.Arguments[i + 1];
                            var conversion = this.AllocNode<ConstantNode>();

                            conversion.Datatype        = builtinFuncDefs[idx].Args[i];
                            conversion.Values.Capacity = 1;

                            ConvertConstant(constant, builtinFuncDefs[idx].Args[i], conversion.Values);
                            func.Arguments[i + 1] = conversion;
                        }

                        retType = builtinFuncDefs[idx].Rettype;

                        return true;
                    }
                }

                idx++;
            }
        }

        if (unsupportedBuiltin)
        {
            var builtinFnArglist = "";
            for (var i = 0; i < argcount; i++)
            {
                if (i > 0)
                {
                    builtinFnArglist += ", ";
                }

                builtinFnArglist += GetDatatypeName(builtinFuncDefs[builtinIdx].Args[i]);
            }

            this.SetError(string.Format(RTR("Built-in function \"{0}({1})\" is only supported on high-end platforms."), name, builtinFnArglist));
            return false;
        }

        if (failedBuiltin)
        {
            var failedBuiltinArgList = "";
            for (var i = 0; i < argcount; i++)
            {
                if (i > 0)
                {
                    failedBuiltinArgList += ",";
                }

                var argName = args[i] == DataType.TYPE_STRUCT
                    ? args2[i]
                    : GetDatatypeName(args[i]);

                if (args3[i] > 0)
                {
                    argName += "[";
                    argName += args3[i];
                    argName += "]";
                }

                failedBuiltinArgList += argName;
            }
            this.SetError(string.Format(RTR("Invalid arguments for the built-in function: \"{0}({0})\"."), name, failedBuiltinArgList));
            return false;
        }

        // try existing functions..

        var excludeFunction = "";
        var b1              = block;

        while (b1 != null)
        {
            if (b1.ParentFunction != null)
            {
                excludeFunction = b1.ParentFunction.Name;
            }
            b1 = b1.ParentBlock;
        }

        if (name == excludeFunction)
        {
            this.SetError(RTR("Recursion is not allowed."));
            return false;
        }

        var lastArgCount = 0;
        var argList      = "";

        if (this.shader!.Functions.TryGetValue(name, out var fn))
        {
            if (fn.Callable)
            {
                this.SetError(string.Format(RTR("Function '{0}' can't be called from source code."), name));
                return false;
            }

            var pfunc          = fn.FunctionNode;
            var pfuncArguments = pfunc.Arguments.Values.OrderBy(x => x.Index).ToArray();
            if (argList.Length == 0)
            {
                for (var j = 0; j < pfuncArguments.Length; j++)
                {
                    if (j > 0)
                    {
                        argList += ", ";
                    }

                    var funcArgName = pfuncArguments[j].Type == DataType.TYPE_STRUCT
                        ? pfuncArguments[j].TypeStr
                        : GetDatatypeName(pfuncArguments[j].Type);

                    if (pfuncArguments[j].ArraySize > 0)
                    {
                        funcArgName += "[";
                        funcArgName += pfuncArguments[j].ArraySize;
                        funcArgName += "]";
                    }
                    argList += funcArgName;
                }
            }

            if (pfunc.Arguments.Count != args.Count)
            {
                lastArgCount = pfunc.Arguments.Count;
            }
            else
            {
                var fail = false;

                for (var j = 0; j < args.Count; j++)
                {
                    if (GetScalarType(args[j]) == args[j] && func.Arguments[j + 1].Type == Node.NodeType.TYPE_CONSTANT && args3[j] == 0 && ConvertConstant((ConstantNode)func.Arguments[j + 1], pfuncArguments[j].Type))
                    {
                        //all good, but it needs implicit conversion later
                    }
                    else if (args[j] != pfuncArguments[j].Type || args[j] == DataType.TYPE_STRUCT && args2[j] != pfuncArguments[j].TypeStr || args3[j] != pfuncArguments[j].ArraySize)
                    {
                        var funcArgName = pfuncArguments[j].Type == DataType.TYPE_STRUCT
                            ? pfuncArguments[j].TypeStr
                            : GetDatatypeName(pfuncArguments[j].Type);

                        if (pfuncArguments[j].ArraySize > 0)
                        {
                            funcArgName += "[";
                            funcArgName += pfuncArguments[j].ArraySize;
                            funcArgName += "]";
                        }

                        var argName = args[j] == DataType.TYPE_STRUCT
                            ? args2[j]
                            : GetDatatypeName(args[j]);

                        if (args3[j] > 0)
                        {
                            argName += "[";
                            argName += args3[j];
                            argName += "]";
                        }

                        this.SetError(string.Format(RTR("Invalid argument for \"{0}({1})\" function: argument %d should be {2} but is {3}."), name, argList, j + 1, funcArgName, argName));
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {

                    //implicitly convert values if possible
                    for (var k = 0; k < args.Count; k++)
                    {
                        if (GetScalarType(args[k]) != args[k] || args[k] == pfuncArguments[k].Type || func.Arguments[k + 1].Type != Node.NodeType.TYPE_CONSTANT)
                        {
                            //can't do implicit conversion here
                            continue;
                        }

                        //this is an implicit conversion
                        var constant   = (ConstantNode)func.Arguments[k + 1];
                        var conversion = this.AllocNode<ConstantNode>();

                        conversion.Datatype = pfuncArguments[k].Type;
                        conversion.Values.Capacity = 1;

                        ConvertConstant(constant, pfuncArguments[k].Type, conversion.Values);
                        func.Arguments[k + 1] = conversion;
                    }

                    retType = pfunc.ReturnType;
                    if (pfunc.ReturnType == DataType.TYPE_STRUCT)
                    {
                        retTypeStr = pfunc.ReturnStructName;
                    }

                    isCustomFunction = true;
                    return true;
                }
            }
        }

        if (lastArgCount > args.Count)
        {
            this.SetError(string.Format(RTR("Too few arguments for \"{0}({1})\" call. Expected at least {2} but received {3}."), name, argList, lastArgCount, args.Count));
        }
        else if (lastArgCount < args.Count)
        {
            this.SetError(string.Format(RTR("Too many arguments for \"{0}({1})\" call. Expected at most {2} but received {3}."), name, argList, lastArgCount, args.Count));
        }

        return false;
    }

    private bool ValidateOperator(OperatorNode op, out DataType retType, out uint retSize)
    {
        retType = default;
        retSize = default;

        var valid = false;

        switch (op.Op)
        {
            case Operator.OP_EQUAL:
            case Operator.OP_NOT_EQUAL:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || !op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();
                    valid = na == nb;
                    retType = DataType.TYPE_BOOL;
                }
                break;
            case Operator.OP_LESS:
            case Operator.OP_LESS_EQUAL:
            case Operator.OP_GREATER:
            case Operator.OP_GREATER_EQUAL: {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    valid = na == nb && (na == DataType.TYPE_UINT || na == DataType.TYPE_INT || na == DataType.TYPE_FLOAT);
                    retType = DataType.TYPE_BOOL;
                }
                break;
            case Operator.OP_AND:
            case Operator.OP_OR:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || !op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    valid = na == nb && na == DataType.TYPE_BOOL;
                    retType = DataType.TYPE_BOOL;

                }
                break;
            case Operator.OP_NOT:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    valid = na == DataType.TYPE_BOOL;
                    retType = DataType.TYPE_BOOL;

                }
                break;
            case Operator.OP_INCREMENT:
            case Operator.OP_DECREMENT:
            case Operator.OP_POST_INCREMENT:
            case Operator.OP_POST_DECREMENT:
            case Operator.OP_NEGATE:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    valid = na > DataType.TYPE_BOOL && na < DataType.TYPE_MAT2;
                    retType = na;
                }
                break;
            case Operator.OP_ADD:
            case Operator.OP_SUB:
            case Operator.OP_MUL:
            case Operator.OP_DIV:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || !op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    if (na > nb)
                    {
                        //make things easier;
                        (na, nb) = (nb, na);
                    }

                    if (na == nb)
                    {
                        valid = na > DataType.TYPE_BOOL && na <= DataType.TYPE_MAT4;
                        retType = na;
                    }
                    else if (na == DataType.TYPE_INT && nb == DataType.TYPE_IVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_INT && nb == DataType.TYPE_IVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_INT && nb == DataType.TYPE_IVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_VEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC2;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_VEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC3;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_VEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC4;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_MAT2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT2;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_MAT3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT3;
                    }
                    else if (na == DataType.TYPE_FLOAT && nb == DataType.TYPE_MAT4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT4;
                    }
                    else if (op.Op == Operator.OP_MUL && na == DataType.TYPE_VEC2 && nb == DataType.TYPE_MAT2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC2;
                    }
                    else if (op.Op == Operator.OP_MUL && na == DataType.TYPE_VEC3 && nb == DataType.TYPE_MAT3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC3;
                    }
                    else if (op.Op == Operator.OP_MUL && na == DataType.TYPE_VEC4 && nb == DataType.TYPE_MAT4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC4;
                    }
                }
                break;
            case Operator.OP_ASSIGN_MOD:
            case Operator.OP_MOD:
                {
                    /*
                    * The operator modulus (%) operates on signed or unsigned integers or integer vectors. The operand
                    * types must both be signed or both be unsigned. The operands cannot be vectors of differing size. If
                    * one operand is a scalar and the other vector, then the scalar is applied component-wise to the vector,
                    * resulting in the same type as the vector. If both are vectors of the same size, the result is computed
                    * component-wise.
                    */

                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || !op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    if (na == DataType.TYPE_INT && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_INT;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_IVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_IVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_IVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                        /////
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UINT;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                }
                break;
            case Operator.OP_ASSIGN_SHIFT_LEFT:
            case Operator.OP_ASSIGN_SHIFT_RIGHT:
            case Operator.OP_SHIFT_LEFT:
            case Operator.OP_SHIFT_RIGHT:
                {
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0 || !op.Arguments[1].IsIndexed && op.Arguments[1].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    if (na == DataType.TYPE_INT && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_INT;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_IVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_IVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_IVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UINT;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                }
                break;
            case Operator.OP_ASSIGN:
                {
                    var sa = 0u;
                    var sb = 0u;
                    if (!op.Arguments[0].IsIndexed)
                    {
                        sa = op.Arguments[0].GetArraySize();
                    }
                    if (!op.Arguments[1].IsIndexed)
                    {
                        sb = op.Arguments[1].GetArraySize();
                    }
                    if (sa != sb)
                    {
                        break; // don't accept arrays if their sizes are not equal
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    valid = na == DataType.TYPE_STRUCT || nb == DataType.TYPE_STRUCT
                        ? op.Arguments[0].GetDatatypeName() == op.Arguments[1].GetDatatypeName()
                        : na == nb;

                    retType = na;
                    retSize = sa;
                }
                break;
            case Operator.OP_ASSIGN_ADD:
            case Operator.OP_ASSIGN_SUB:
            case Operator.OP_ASSIGN_MUL:
            case Operator.OP_ASSIGN_DIV:
                {
                    var sa = 0u;
                    var sb = 0u;
                    if (!op.Arguments[0].IsIndexed)
                    {
                        sa = op.Arguments[0].GetArraySize();
                    }
                    if (!op.Arguments[1].IsIndexed)
                    {
                        sb = op.Arguments[1].GetArraySize();
                    }
                    if (sa > 0 || sb > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    if (na == nb)
                    {
                        valid   = na > DataType.TYPE_BOOL && na <= DataType.TYPE_MAT4;
                        retType = na;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                    else if (na == DataType.TYPE_VEC2 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC2;
                    }
                    else if (na == DataType.TYPE_VEC3 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC3;
                    }
                    else if (na == DataType.TYPE_VEC4 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC4;
                    }
                    else if (na == DataType.TYPE_MAT2 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT2;
                    }
                    else if (na == DataType.TYPE_MAT3 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT3;
                    }
                    else if (na == DataType.TYPE_MAT4 && nb == DataType.TYPE_FLOAT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_MAT4;
                    }
                    else if (op.Op == Operator.OP_ASSIGN_MUL && na == DataType.TYPE_VEC2 && nb == DataType.TYPE_MAT2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC2;
                    }
                    else if (op.Op == Operator.OP_ASSIGN_MUL && na == DataType.TYPE_VEC3 && nb == DataType.TYPE_MAT3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC3;
                    }
                    else if (op.Op == Operator.OP_ASSIGN_MUL && na == DataType.TYPE_VEC4 && nb == DataType.TYPE_MAT4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_VEC4;
                    }
                }
                break;
            case Operator.OP_ASSIGN_BIT_AND:
            case Operator.OP_ASSIGN_BIT_OR:
            case Operator.OP_ASSIGN_BIT_XOR:
            case Operator.OP_BIT_AND:
            case Operator.OP_BIT_OR:
            case Operator.OP_BIT_XOR:
                {
                    /*
                    * The bitwise operators and (&), exclusive-or (^), and inclusive-or (|). The operands must be of type
                    * signed or unsigned integers or integer vectors. The operands cannot be vectors of differing size. If
                    * one operand is a scalar and the other a vector, the scalar is applied component-wise to the vector,
                    * resulting in the same type as the vector. The fundamental types of the operands (signed or unsigned)
                    * must match.
                    */

                    var sa = 0u;
                    var sb = 0u;
                    if (!op.Arguments[0].IsIndexed)
                    {
                        sa = op.Arguments[0].GetArraySize();
                    }
                    if (!op.Arguments[1].IsIndexed)
                    {
                        sb = op.Arguments[1].GetArraySize();
                    }
                    if (sa > 0 || sb > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();

                    if (na > nb && op.Op >= Operator.OP_BIT_AND)
                    {
                        //can swap for non assign
                        (na, nb) = (nb, na);
                    }

                    if (na == DataType.TYPE_INT && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_INT;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_INT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                    }
                    else if (na == DataType.TYPE_IVEC2 && nb == DataType.TYPE_IVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC2;
                    }
                    else if (na == DataType.TYPE_IVEC3 && nb == DataType.TYPE_IVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC3;
                    }
                    else if (na == DataType.TYPE_IVEC4 && nb == DataType.TYPE_IVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_IVEC4;
                        /////
                    }
                    else if (na == DataType.TYPE_UINT && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UINT;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UINT)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                    else if (na == DataType.TYPE_UVEC2 && nb == DataType.TYPE_UVEC2)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC2;
                    }
                    else if (na == DataType.TYPE_UVEC3 && nb == DataType.TYPE_UVEC3)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC3;
                    }
                    else if (na == DataType.TYPE_UVEC4 && nb == DataType.TYPE_UVEC4)
                    {
                        valid   = true;
                        retType = DataType.TYPE_UVEC4;
                    }
                }
                break;
            case Operator.OP_BIT_INVERT:
                {
                    //unaries
                    if (!op.Arguments[0].IsIndexed && op.Arguments[0].GetArraySize() > 0)
                    {
                        break; // don't accept arrays
                    }

                    var na = op.Arguments[0].GetDatatype();
                    valid = na >= DataType.TYPE_INT && na < DataType.TYPE_FLOAT;
                    retType = na;
                }
                break;
            case Operator.OP_SELECT_IF:
                {
                    var sa = 0u;
                    var sb = 0u;
                    if (!op.Arguments[1].IsIndexed)
                    {
                        sa = op.Arguments[1].GetArraySize();
                    }
                    if (!op.Arguments[2].IsIndexed)
                    {
                        sb = op.Arguments[2].GetArraySize();
                    }
                    if (sa != sb)
                    {
                        break; // don't accept arrays if their sizes are not equal
                    }

                    var na = op.Arguments[0].GetDatatype();
                    var nb = op.Arguments[1].GetDatatype();
                    var nc = op.Arguments[2].GetDatatype();

                    valid = na == DataType.TYPE_BOOL && nb == nc;
                    retType = nb;
                    retSize = sa;
                }
                break;
            default:
                return ERR_FAIL_V(false);
        }

        return valid;
    }
    private Error ValidatePrecision(DataType type, DataPrecision precision) => throw new NotImplementedException();
    private bool ValidateVaryingAssign(ShaderNode.Varying varying, out string message) => throw new NotImplementedException();
    #endregion private methods

    #region public methods
    public Error Compile(string code, ShaderCompileInfo info)
    {
        this.Clear();

        this.isShaderInc                    = info.IsInclude;
        this.code                           = code;
        this.globalShaderUniformGetTypeFunc = info.GlobalShaderUniformTypeFunc;
        this.varyingFunctionNames           = info.VaryingFunctionNames;
        this.nodes                          = null;
        this.shader                         = this.AllocNode<ShaderNode>();

        var err = this.ParseShader(info.Functions, info.RenderModes, info.ShaderTypes);

        #if DEBUG
        if (this.checkWarnings)
        {
            this.CheckWarningAccums();
        }
        #endif // DEBUG_ENABLED

        return err;
    }

    // TODO public Error Complete(string code, ShaderCompileInfo info, out List<ScriptLanguage.CodeCompletionOption> options, out string callHint) => throw new NotImplementedException();
    public int GetErrorLine() => this.errorLine;
    public string GetErrorText() => throw new NotImplementedException();

    public List<FilePosition> GetIncludePositions() => this.includePositions;

    public ShaderNode GetShader() => this.shader!;

    public string TokenDebug(string code) => throw new NotImplementedException();
    #endregion public methods
}
