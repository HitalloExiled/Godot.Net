namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    private record BuiltinFuncDef(string? Name, DataType Rettype, DataType[] Args, string[] ArgsNames, SubClassTag Tag, bool HighEnd);

    private static readonly BuiltinFuncDef[] builtinFuncDefs = new BuiltinFuncDef[]
    {
        // Constructors.

        new("bool",  DataType.TYPE_BOOL,  new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BVEC2, DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BVEC2, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_BOOL, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BVEC2, DataType.TYPE_BOOL,  DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC2, DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BOOL,  DataType.TYPE_BVEC2, DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BOOL,  DataType.TYPE_BVEC3, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC3, DataType.TYPE_BOOL,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC2, DataType.TYPE_BVEC2, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("float", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                                                new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec2",  DataType.TYPE_VEC2,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                                                new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec2",  DataType.TYPE_VEC2,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3",  DataType.TYPE_VEC3,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                                                new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3",  DataType.TYPE_VEC3,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3",  DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3",  DataType.TYPE_VEC3,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC2,  DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                                                new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VEC2,  DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC3,  DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_FLOAT, DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4",  DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID },                                           new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("int",   DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID },                                                              new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID },                                                              new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID },                                                              new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT, DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_INT,   DataType.TYPE_IVEC2, DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID },                                                              new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_INT,   DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_VOID },                    new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID },                    new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_IVEC2, DataType.TYPE_VOID },                    new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_INT,   DataType.TYPE_IVEC3, DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC3, DataType.TYPE_INT,   DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("uint",  DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT, DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT, DataType.TYPE_VOID },                      new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UINT,  DataType.TYPE_UVEC2, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID },                                                               new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UINT,  DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UVEC2, DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UINT,  DataType.TYPE_UVEC3, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UINT,  DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID },                                          new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("mat2", DataType.TYPE_MAT2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID },                                         new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat3", DataType.TYPE_MAT3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID },                     new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat4", DataType.TYPE_MAT4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("mat2", DataType.TYPE_MAT2, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat3", DataType.TYPE_MAT3, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat4", DataType.TYPE_MAT4, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        // Conversion scalars.

        new("int", DataType.TYPE_INT, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("int", DataType.TYPE_INT, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("int", DataType.TYPE_INT, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("int", DataType.TYPE_INT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("float", DataType.TYPE_FLOAT, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("float", DataType.TYPE_FLOAT, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("float", DataType.TYPE_FLOAT, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("float", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("uint", DataType.TYPE_UINT, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uint", DataType.TYPE_UINT, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uint", DataType.TYPE_UINT, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uint", DataType.TYPE_UINT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("bool", DataType.TYPE_BOOL, new[] { DataType.TYPE_BOOL,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bool", DataType.TYPE_BOOL, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bool", DataType.TYPE_BOOL, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bool", DataType.TYPE_BOOL, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        // Conversion vectors.

        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec2", DataType.TYPE_IVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("vec2", DataType.TYPE_VEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec2", DataType.TYPE_VEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec2", DataType.TYPE_VEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec2", DataType.TYPE_VEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec2", DataType.TYPE_UVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec2", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec3", DataType.TYPE_IVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("vec3", DataType.TYPE_VEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3", DataType.TYPE_VEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3", DataType.TYPE_VEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec3", DataType.TYPE_VEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec3", DataType.TYPE_UVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec3", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("ivec4", DataType.TYPE_IVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("vec4", DataType.TYPE_VEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4", DataType.TYPE_VEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4", DataType.TYPE_VEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("vec4", DataType.TYPE_VEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("uvec4", DataType.TYPE_UVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("bvec4", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        // Conversion between matrixes.

        new("mat2", DataType.TYPE_MAT2, new[] { DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat2", DataType.TYPE_MAT2, new[] { DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat3", DataType.TYPE_MAT3, new[] { DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat3", DataType.TYPE_MAT3, new[] { DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat4", DataType.TYPE_MAT4, new[] { DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
        new("mat4", DataType.TYPE_MAT4, new[] { DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),

        // Built-ins - trigonometric functions.
        // radians

        new("radians", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "degrees" }, SubClassTag.TAG_GLOBAL, false),
        new("radians", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "degrees" }, SubClassTag.TAG_GLOBAL, false),
        new("radians", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "degrees" }, SubClassTag.TAG_GLOBAL, false),
        new("radians", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "degrees" }, SubClassTag.TAG_GLOBAL, false),

        // degrees

        new("degrees", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "radians" }, SubClassTag.TAG_GLOBAL, false),
        new("degrees", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "radians" }, SubClassTag.TAG_GLOBAL, false),
        new("degrees", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "radians" }, SubClassTag.TAG_GLOBAL, false),
        new("degrees", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "radians" }, SubClassTag.TAG_GLOBAL, false),

        // sin

        new("sin", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("sin", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("sin", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("sin", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),

        // cos

        new("cos", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("cos", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("cos", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("cos", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),

        // tan

        new("tan", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("tan", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("tan", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),
        new("tan", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "angle" }, SubClassTag.TAG_GLOBAL, false),

        // asin

        new("asin", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asin", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asin", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asin", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // acos

        new("acos", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acos", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acos", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acos", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // atan

        new("atan", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID },                       new[] { "y_over_x" }, SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID },                       new[] { "y_over_x" }, SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID },                       new[] { "y_over_x" }, SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID },                       new[] { "y_over_x" }, SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID },  new[] { "y", "x" },   SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID },  new[] { "y", "x" },   SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID },  new[] { "y", "x" },   SubClassTag.TAG_GLOBAL, false),
        new("atan", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID },  new[] { "y", "x" },   SubClassTag.TAG_GLOBAL, false),

        // sinh

        new("sinh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sinh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sinh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sinh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // cosh

        new("cosh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("cosh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("cosh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("cosh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // tanh

        new("tanh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("tanh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("tanh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("tanh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // asinh

        new("asinh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asinh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asinh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("asinh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // acosh

        new("acosh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acosh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acosh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("acosh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // atanh

        new("atanh", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("atanh", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("atanh", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("atanh", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // Builtins - exponential functions.
        // pow

        new("pow", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("pow", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("pow", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("pow", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),

        // exp

        new("exp", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // log

        new("log", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // exp2

        new("exp2", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp2", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp2", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("exp2", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // log2

        new("log2", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log2", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log2", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("log2", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // sqrt

        new("sqrt", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sqrt", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sqrt", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sqrt", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // inversesqrt

        new("inversesqrt", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("inversesqrt", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("inversesqrt", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("inversesqrt", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // Built-ins - common functions.
        // abs

        new("abs", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        new("abs", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("abs", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // sign

        new("sign", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        new("sign", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("sign", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // floor

        new("floor", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floor", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floor", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floor", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // trunc

        new("trunc", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("trunc", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("trunc", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("trunc", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // round

        new("round", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("round", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("round", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("round", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // roundEven

        new("roundEven", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("roundEven", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("roundEven", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("roundEven", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // ceil

        new("ceil", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("ceil", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("ceil", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("ceil", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // fract

        new("fract", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("fract", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("fract", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("fract", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // mod

        new("mod", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),
        new("mod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "y" }, SubClassTag.TAG_GLOBAL, false),

        // modf

        new("modf", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "i" }, SubClassTag.TAG_GLOBAL, false),
        new("modf", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x", "i" }, SubClassTag.TAG_GLOBAL, false),
        new("modf", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x", "i" }, SubClassTag.TAG_GLOBAL, false),
        new("modf", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x", "i" }, SubClassTag.TAG_GLOBAL, false),

        // min

        new("min", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_FLOAT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_FLOAT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("min", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("min", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("min", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // max

        new("max", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("max", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("max", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("max", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // clamp

        new("clamp", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),

        new("clamp", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),

        new("clamp", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),
        new("clamp", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "minVal", "maxVal" }, SubClassTag.TAG_GLOBAL, false),

        // mix

        new("mix", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("mix", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "a", "b", "value" }, SubClassTag.TAG_GLOBAL, false),

        // step

        new("step", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC2,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC3,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),
        new("step", DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "edge", "x" }, SubClassTag.TAG_GLOBAL, false),

        // smoothstep

        new("smoothstep", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC2,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC3,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),
        new("smoothstep", DataType.TYPE_VEC4,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "edge0", "edge1", "value" }, SubClassTag.TAG_GLOBAL, false),

        // isnan

        new("isnan", DataType.TYPE_BOOL,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isnan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isnan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isnan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // isinf

        new("isinf", DataType.TYPE_BOOL,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isinf", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isinf", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("isinf", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // floatBitsToInt

        new("floatBitsToInt", DataType.TYPE_INT,   new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToInt", DataType.TYPE_IVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToInt", DataType.TYPE_IVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToInt", DataType.TYPE_IVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // floatBitsToUint

        new("floatBitsToUint", DataType.TYPE_UINT,  new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToUint", DataType.TYPE_UVEC2, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToUint", DataType.TYPE_UVEC3, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("floatBitsToUint", DataType.TYPE_UVEC4, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // intBitsToFloat

        new("intBitsToFloat", DataType.TYPE_FLOAT, new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("intBitsToFloat", DataType.TYPE_VEC2,  new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("intBitsToFloat", DataType.TYPE_VEC3,  new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("intBitsToFloat", DataType.TYPE_VEC4,  new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // uintBitsToFloat

        new("uintBitsToFloat", DataType.TYPE_FLOAT, new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("uintBitsToFloat", DataType.TYPE_VEC2,  new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("uintBitsToFloat", DataType.TYPE_VEC3,  new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("uintBitsToFloat", DataType.TYPE_VEC4,  new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // Built-ins - geometric functions.
        // length

        new("length", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("length", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("length", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("length", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // distance

        new("distance", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("distance", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("distance", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("distance", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4,   DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // dot

        new("dot", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("dot", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("dot", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("dot", DataType.TYPE_FLOAT, new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // cross

        new("cross", DataType.TYPE_VEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // normalize

        new("normalize", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("normalize", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("normalize", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("normalize", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),

        // reflect

        new("reflect", DataType.TYPE_VEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "I", "N" }, SubClassTag.TAG_GLOBAL, false),
        new("reflect", DataType.TYPE_VEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "I", "N" }, SubClassTag.TAG_GLOBAL, false),
        new("reflect", DataType.TYPE_VEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "I", "N" }, SubClassTag.TAG_GLOBAL, false),

        // refract

        new("refract", DataType.TYPE_VEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "I", "N", "eta" }, SubClassTag.TAG_GLOBAL, false),
        new("refract", DataType.TYPE_VEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "I", "N", "eta" }, SubClassTag.TAG_GLOBAL, false),
        new("refract", DataType.TYPE_VEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "I", "N", "eta" }, SubClassTag.TAG_GLOBAL, false),

        // faceforward

        new("faceforward", DataType.TYPE_VEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "N", "I", "Nref" }, SubClassTag.TAG_GLOBAL, false),
        new("faceforward", DataType.TYPE_VEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "N", "I", "Nref" }, SubClassTag.TAG_GLOBAL, false),
        new("faceforward", DataType.TYPE_VEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "N", "I", "Nref" }, SubClassTag.TAG_GLOBAL, false),

        // matrixCompMult

        new("matrixCompMult", DataType.TYPE_MAT2, new[] { DataType.TYPE_MAT2, DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("matrixCompMult", DataType.TYPE_MAT3, new[] { DataType.TYPE_MAT3, DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("matrixCompMult", DataType.TYPE_MAT4, new[] { DataType.TYPE_MAT4, DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // outerProduct

        new("outerProduct", DataType.TYPE_MAT2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "c", "r" }, SubClassTag.TAG_GLOBAL, false),
        new("outerProduct", DataType.TYPE_MAT3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "c", "r" }, SubClassTag.TAG_GLOBAL, false),
        new("outerProduct", DataType.TYPE_MAT4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "c", "r" }, SubClassTag.TAG_GLOBAL, false),

        // transpose

        new("transpose", DataType.TYPE_MAT2, new[] { DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("transpose", DataType.TYPE_MAT3, new[] { DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("transpose", DataType.TYPE_MAT4, new[] { DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),

        // determinant

        new("determinant", DataType.TYPE_FLOAT, new[] { DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("determinant", DataType.TYPE_FLOAT, new[] { DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("determinant", DataType.TYPE_FLOAT, new[] { DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),

        // inverse

        new("inverse", DataType.TYPE_MAT2, new[] { DataType.TYPE_MAT2, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("inverse", DataType.TYPE_MAT3, new[] { DataType.TYPE_MAT3, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),
        new("inverse", DataType.TYPE_MAT4, new[] { DataType.TYPE_MAT4, DataType.TYPE_VOID }, new[] { "m" }, SubClassTag.TAG_GLOBAL, false),

        // lessThan

        new("lessThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("lessThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("lessThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // greaterThan

        new("greaterThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("greaterThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("greaterThan", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThan", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // lessThanEqual

        new("lessThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("lessThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("lessThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("lessThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // greaterThanEqual

        new("greaterThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("greaterThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("greaterThanEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("greaterThanEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // equal

        new("equal", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("equal", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("equal", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("equal", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("equal", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // notEqual

        new("notEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_VEC4, DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("notEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("notEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        new("notEqual", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),
        new("notEqual", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "a", "b" }, SubClassTag.TAG_GLOBAL, false),

        // any

        new("any", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("any", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("any", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // all

        new("all", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("all", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("all", DataType.TYPE_BOOL, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // not

        new("not", DataType.TYPE_BVEC2, new[] { DataType.TYPE_BVEC2, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("not", DataType.TYPE_BVEC3, new[] { DataType.TYPE_BVEC3, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),
        new("not", DataType.TYPE_BVEC4, new[] { DataType.TYPE_BVEC4, DataType.TYPE_VOID }, new[] { "x" }, SubClassTag.TAG_GLOBAL, false),

        // Built-ins: texture functions.
        // textureSize

        new("textureSize", DataType.TYPE_IVEC2, new[] { DataType.TYPE_SAMPLER2D,        DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC2, new[] { DataType.TYPE_ISAMPLER2D,       DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC2, new[] { DataType.TYPE_USAMPLER2D,       DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_SAMPLER2DARRAY,   DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_ISAMPLER2DARRAY,  DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_USAMPLER2DARRAY,  DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_SAMPLER3D,        DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_ISAMPLER3D,       DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC3, new[] { DataType.TYPE_USAMPLER3D,       DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC2, new[] { DataType.TYPE_SAMPLERCUBE,      DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureSize", DataType.TYPE_IVEC2, new[] { DataType.TYPE_SAMPLERCUBEARRAY, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "lod" }, SubClassTag.TAG_GLOBAL, false),

        // texture

        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,        DataType.TYPE_VEC2, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,        DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,   DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,   DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,        DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,        DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,      DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,      DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBEARRAY, DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("texture", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBEARRAY, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),

        // textureProj

        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,  DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,  DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_VOID },                      new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureProj", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "bias" }, SubClassTag.TAG_GLOBAL, false),

        // textureLod

        new("textureLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,        DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,   DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,        DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,      DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBEARRAY, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),

        // texelFetch

        new("texelFetch", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,       DataType.TYPE_IVEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,      DataType.TYPE_IVEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,      DataType.TYPE_IVEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,  DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY, DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY, DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,       DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D,      DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("texelFetch", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D,      DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),

        // textureProjLod

        new("textureProjLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,  DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjLod", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "lod" }, SubClassTag.TAG_GLOBAL, false),

        // textureGrad

        new("textureGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,        DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,   DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,        DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D,       DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,      DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBEARRAY, DataType.TYPE_VEC4, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),

        // textureProjGrad

        new("textureProjGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,  DataType.TYPE_VEC4, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC3, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D, DataType.TYPE_VEC4, DataType.TYPE_VEC2, DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER3D,  DataType.TYPE_VEC4, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),
        new("textureProjGrad", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER3D, DataType.TYPE_VEC4, DataType.TYPE_VEC3, DataType.TYPE_VEC3, DataType.TYPE_VOID }, new[] { "sampler", "coords", "dPdx", "dPdy" }, SubClassTag.TAG_GLOBAL, false),

        // textureGather

        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,      DataType.TYPE_VEC2, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,      DataType.TYPE_VEC2, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2D,       DataType.TYPE_VEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2D,      DataType.TYPE_VEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2D,      DataType.TYPE_VEC2, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY, DataType.TYPE_VEC3, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY, DataType.TYPE_VEC3, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLER2DARRAY,  DataType.TYPE_VEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_IVEC4, new[] { DataType.TYPE_ISAMPLER2DARRAY, DataType.TYPE_VEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_UVEC4, new[] { DataType.TYPE_USAMPLER2DARRAY, DataType.TYPE_VEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,     DataType.TYPE_VEC3, DataType.TYPE_VOID },                    new[] { "sampler", "coords" },         SubClassTag.TAG_GLOBAL, false),
        new("textureGather", DataType.TYPE_VEC4,  new[] { DataType.TYPE_SAMPLERCUBE,     DataType.TYPE_VEC3, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "sampler", "coords", "comp" }, SubClassTag.TAG_GLOBAL, false),

        // textureQueryLod

        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_SAMPLER2D,       DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_ISAMPLER2D,      DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_USAMPLER2D,      DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_SAMPLER2DARRAY,  DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_ISAMPLER2DARRAY, DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_USAMPLER2DARRAY, DataType.TYPE_VEC2 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_SAMPLER3D,       DataType.TYPE_VEC3 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_ISAMPLER3D,      DataType.TYPE_VEC3 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_USAMPLER3D,      DataType.TYPE_VEC3 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLod", DataType.TYPE_VEC2, new[] { DataType.TYPE_SAMPLERCUBE,     DataType.TYPE_VEC3 }, new[] { "sampler", "coords" }, SubClassTag.TAG_GLOBAL, true),

        // textureQueryLevels

        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_SAMPLER2D },       new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_ISAMPLER2D },      new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_USAMPLER2D },      new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_SAMPLER2DARRAY },  new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_ISAMPLER2DARRAY }, new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_USAMPLER2DARRAY }, new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_SAMPLER3D },       new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_ISAMPLER3D },      new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_USAMPLER3D },      new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),
        new("textureQueryLevels", DataType.TYPE_INT, new[] { DataType.TYPE_SAMPLERCUBE },     new[] { "sampler" }, SubClassTag.TAG_GLOBAL, true),

        // dFdx

        new("dFdx", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdx", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdx", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdx", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),

        // dFdxCoarse

        new("dFdxCoarse", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxCoarse", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxCoarse", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxCoarse", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // dFdxFine

        new("dFdxFine", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxFine", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxFine", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdxFine", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // dFdy

        new("dFdy", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdy", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdy", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("dFdy", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),

        // dFdyCoarse

        new("dFdyCoarse", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyCoarse", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyCoarse", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyCoarse", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // dFdyFine

        new("dFdyFine", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyFine", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyFine", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("dFdyFine", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // fwidth

        new("fwidth", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("fwidth", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("fwidth", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),
        new("fwidth", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, false),

        // fwidthCoarse

        new("fwidthCoarse", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthCoarse", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthCoarse", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthCoarse", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // fwidthFine

        new("fwidthFine", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthFine", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthFine", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),
        new("fwidthFine", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "p" }, SubClassTag.TAG_GLOBAL, true),

        // Sub-functions.
        // array

        new("length", DataType.TYPE_INT, new[] { DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_ARRAY, false),

        // Modern functions.
        // fma

        new("fma", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_FLOAT, DataType.TYPE_VOID }, new[] { "a", "b", "c" }, SubClassTag.TAG_GLOBAL, true),
        new("fma", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VEC2,  DataType.TYPE_VOID }, new[] { "a", "b", "c" }, SubClassTag.TAG_GLOBAL, true),
        new("fma", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VEC3,  DataType.TYPE_VOID }, new[] { "a", "b", "c" }, SubClassTag.TAG_GLOBAL, true),
        new("fma", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VEC4,  DataType.TYPE_VOID }, new[] { "a", "b", "c" }, SubClassTag.TAG_GLOBAL, true),

        // Packing/Unpacking functions.

        new("packHalf2x16",  DataType.TYPE_UINT, new[] { DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("packUnorm2x16", DataType.TYPE_UINT, new[] { DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("packSnorm2x16", DataType.TYPE_UINT, new[] { DataType.TYPE_VEC2, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("packUnorm4x8",  DataType.TYPE_UINT, new[] { DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("packSnorm4x8",  DataType.TYPE_UINT, new[] { DataType.TYPE_VEC4, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),

        new("unpackHalf2x16",  DataType.TYPE_VEC2, new[] { DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("unpackUnorm2x16", DataType.TYPE_VEC2, new[] { DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("unpackSnorm2x16", DataType.TYPE_VEC2, new[] { DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("unpackUnorm4x8",  DataType.TYPE_VEC4, new[] { DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),
        new("unpackSnorm4x8",  DataType.TYPE_VEC4, new[] { DataType.TYPE_UINT, DataType.TYPE_VOID }, new[] { "v" }, SubClassTag.TAG_GLOBAL, false),

        // bitfieldExtract

        new("bitfieldExtract", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),

        new("bitfieldExtract", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldExtract", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "value", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),

        // bitfieldInsert

        new("bitfieldInsert", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),

        new("bitfieldInsert", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldInsert", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_INT, DataType.TYPE_INT, DataType.TYPE_VOID }, new[] { "base", "insert", "offset", "bits" }, SubClassTag.TAG_GLOBAL, true),

        // bitfieldReverse

        new("bitfieldReverse", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        new("bitfieldReverse", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitfieldReverse", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        // bitCount

        new("bitCount", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        new("bitCount", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("bitCount", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        // findLSB

        new("findLSB", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        new("findLSB", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findLSB", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        // findMSB

        new("findMSB", DataType.TYPE_INT,   new[] { DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_IVEC2, new[] { DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_IVEC3, new[] { DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_IVEC4, new[] { DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        new("findMSB", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),
        new("findMSB", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "value" }, SubClassTag.TAG_GLOBAL, true),

        // umulExtended

        new("umulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("umulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("umulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("umulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),

        // imulExtended

        new("imulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("imulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("imulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),
        new("imulExtended", DataType.TYPE_VOID, new[] { DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x", "y", "msb", "lsb" }, SubClassTag.TAG_GLOBAL, true),

        // uaddCarry

        new("uaddCarry", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "y", "carry" }, SubClassTag.TAG_GLOBAL, true),
        new("uaddCarry", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "x", "y", "carry" }, SubClassTag.TAG_GLOBAL, true),
        new("uaddCarry", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "x", "y", "carry" }, SubClassTag.TAG_GLOBAL, true),
        new("uaddCarry", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "x", "y", "carry" }, SubClassTag.TAG_GLOBAL, true),

        // usubBorrow

        new("usubBorrow", DataType.TYPE_UINT,  new[] { DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_UINT,  DataType.TYPE_VOID }, new[] { "x", "y", "borrow" }, SubClassTag.TAG_GLOBAL, true),
        new("usubBorrow", DataType.TYPE_UVEC2, new[] { DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_UVEC2, DataType.TYPE_VOID }, new[] { "x", "y", "borrow" }, SubClassTag.TAG_GLOBAL, true),
        new("usubBorrow", DataType.TYPE_UVEC3, new[] { DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_UVEC3, DataType.TYPE_VOID }, new[] { "x", "y", "borrow" }, SubClassTag.TAG_GLOBAL, true),
        new("usubBorrow", DataType.TYPE_UVEC4, new[] { DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_UVEC4, DataType.TYPE_VOID }, new[] { "x", "y", "borrow" }, SubClassTag.TAG_GLOBAL, true),

        // ldexp

        new("ldexp", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("ldexp", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("ldexp", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("ldexp", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),

        // frexp

        new("frexp", DataType.TYPE_FLOAT, new[] { DataType.TYPE_FLOAT, DataType.TYPE_INT,   DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("frexp", DataType.TYPE_VEC2,  new[] { DataType.TYPE_VEC2,  DataType.TYPE_IVEC2, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("frexp", DataType.TYPE_VEC3,  new[] { DataType.TYPE_VEC3,  DataType.TYPE_IVEC3, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),
        new("frexp", DataType.TYPE_VEC4,  new[] { DataType.TYPE_VEC4,  DataType.TYPE_IVEC4, DataType.TYPE_VOID }, new[] { "x", "exp" }, SubClassTag.TAG_GLOBAL, true),

        new(null, DataType.TYPE_VOID,  new[] { DataType.TYPE_VOID }, new[] { "" }, SubClassTag.TAG_GLOBAL, false),
    };
}
