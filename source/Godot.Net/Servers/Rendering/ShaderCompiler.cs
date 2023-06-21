namespace Godot.Net.Servers.Rendering;

using System.Globalization;
using Godot.Net.Core.Error;
using Godot.Net.Extensions;
using SL = ShaderLanguage;

#pragma warning disable CS0414, IDE0052, IDE0044, IDE0051, CS0169 // TODO Remove

public partial class ShaderCompiler
{
    private readonly SL parser = new();

    private DefaultIdentifierActions  actions           = new();
    private string                    currentFuncName   = "";
    private HashSet<string>           fragmentVaryings  = new();
    private SL.FunctionNode?          function          = new();
    private HashSet<string>           internalFunctions = new();
    private SL.ShaderNode?            shader;
    private HashSet<string>           textureFunctions  = new();
    private string?                   timeName;
    private HashSet<string>           usedFlagPointers  = new();
    private HashSet<string>           usedNameDefines   = new();
    private HashSet<string>           usedRmodeDefines  = new();

    private static string Constr(bool isConst) =>
        isConst ? "const " : "";

    private static string F2sp0(float value)
    {
        var num = value.ToString(CultureInfo.InvariantCulture);

        if (!num.Contains('.') && !num.Contains('e'))
        {
            num += ".0";
        }

        return num;
    }

    private static string GetConstantText(SL.DataType type, List<SL.ConstantNode.ValueUnion> values)
    {
        switch (type)
        {
            case SL.DataType.TYPE_BOOL:
                return values[0].Boolean ? "true" : "false";
            case SL.DataType.TYPE_BVEC2:
            case SL.DataType.TYPE_BVEC3:
            case SL.DataType.TYPE_BVEC4:
                {
                    var text = $"bvec{type - SL.DataType.TYPE_BOOL}{1}(";
                    for (var i = 0; i < values.Count; i++)
                    {
                        if (i > 0)
                        {
                            text += ",";
                        }

                        text += values[i].Boolean ? "true" : "false";
                    }
                    text += ")";
                    return text;
                }

            case SL.DataType.TYPE_INT:
                return values[0].Sint.ToString();
            case SL.DataType.TYPE_IVEC2:
            case SL.DataType.TYPE_IVEC3:
            case SL.DataType.TYPE_IVEC4:
                {
                    var text = $"ivec{type - SL.DataType.TYPE_INT + 1}(";
                    for (var i = 0; i < values.Count; i++)
                    {
                        if (i > 0)
                        {
                            text += ",";
                        }

                        text += values[i].Sint;
                    }
                    text += ")";

                    return text;
                }
            case SL.DataType.TYPE_UINT:
                return values[0].Uint + "u";
            case SL.DataType.TYPE_UVEC2:
            case SL.DataType.TYPE_UVEC3:
            case SL.DataType.TYPE_UVEC4:
                {
                    var text = $"uvec{type - SL.DataType.TYPE_UINT + 1}(";
                    for (var i = 0; i < values.Count; i++)
                    {
                        if (i > 0)
                        {
                            text += ",";
                        }

                        text += values[i].Uint + "u";
                    }
                    text += ")";

                    return text;
                }
            case SL.DataType.TYPE_FLOAT:
                return F2sp0(values[0].Real);
            case SL.DataType.TYPE_VEC2:
            case SL.DataType.TYPE_VEC3:
            case SL.DataType.TYPE_VEC4:
                {
                    var text = $"vec{type - SL.DataType.TYPE_FLOAT}{1}(";
                    for (var i = 0; i < values.Count; i++)
                    {
                        if (i > 0)
                        {
                            text += ",";
                        }

                        text += F2sp0(values[i].Real);
                    }
                    text += ")";

                    return text;

                }
            case SL.DataType.TYPE_MAT2:
            case SL.DataType.TYPE_MAT3:
            case SL.DataType.TYPE_MAT4:
                {
                    var text = $"mat{type - SL.DataType.TYPE_MAT2 + 2}(";
                    for (var i = 0; i < values.Count; i++)
                    {
                        if (i > 0)
                        {
                            text += ",";
                        }

                        text += F2sp0(values[i].Real);
                    }
                    text += ")";
                    return text;

                }
            default:
                return ERR_FAIL_V("");
        }
    }

    private static int GetDatatypeAlignment(SL.DataType type)
    {
        switch (type)
        {
            case SL.DataType.TYPE_VOID:
                return 0;
            case SL.DataType.TYPE_BOOL:
                return 4;
            case SL.DataType.TYPE_BVEC2:
                return 8;
            case SL.DataType.TYPE_BVEC3:
                return 16;
            case SL.DataType.TYPE_BVEC4:
                return 16;
            case SL.DataType.TYPE_INT:
                return 4;
            case SL.DataType.TYPE_IVEC2:
                return 8;
            case SL.DataType.TYPE_IVEC3:
                return 16;
            case SL.DataType.TYPE_IVEC4:
                return 16;
            case SL.DataType.TYPE_UINT:
                return 4;
            case SL.DataType.TYPE_UVEC2:
                return 8;
            case SL.DataType.TYPE_UVEC3:
                return 16;
            case SL.DataType.TYPE_UVEC4:
                return 16;
            case SL.DataType.TYPE_FLOAT:
                return 4;
            case SL.DataType.TYPE_VEC2:
                return 8;
            case SL.DataType.TYPE_VEC3:
                return 16;
            case SL.DataType.TYPE_VEC4:
                return 16;
            case SL.DataType.TYPE_MAT2:
                return 16;
            case SL.DataType.TYPE_MAT3:
                return 16;
            case SL.DataType.TYPE_MAT4:
                return 16;
            case SL.DataType.TYPE_SAMPLER2D:
                return 16;
            case SL.DataType.TYPE_ISAMPLER2D:
                return 16;
            case SL.DataType.TYPE_USAMPLER2D:
                return 16;
            case SL.DataType.TYPE_SAMPLER2DARRAY:
                return 16;
            case SL.DataType.TYPE_ISAMPLER2DARRAY:
                return 16;
            case SL.DataType.TYPE_USAMPLER2DARRAY:
                return 16;
            case SL.DataType.TYPE_SAMPLER3D:
                return 16;
            case SL.DataType.TYPE_ISAMPLER3D:
                return 16;
            case SL.DataType.TYPE_USAMPLER3D:
                return 16;
            case SL.DataType.TYPE_SAMPLERCUBE:
                return 16;
            case SL.DataType.TYPE_SAMPLERCUBEARRAY:
                return 16;
            case SL.DataType.TYPE_STRUCT:
                return 0;
            case SL.DataType.TYPE_MAX:
            default:
            {
                return ERR_FAIL_V(0);
            }
        }
    }

    private static SL.DataType GetGlobalShaderUniformType(string name) => throw new NotImplementedException();

    private static string GetGlobalShaderUniformFromTypeAndIndex(string buffer, string index, SL.DataType type) =>
        type switch
        {
            SL.DataType.TYPE_BOOL  => "bool(floatBitsToUint(" + buffer + "[" + index + "].x))",
            SL.DataType.TYPE_BVEC2 => "bvec2(floatBitsToUint(" + buffer + "[" + index + "].xy))",
            SL.DataType.TYPE_BVEC3 => "bvec3(floatBitsToUint(" + buffer + "[" + index + "].xyz))",
            SL.DataType.TYPE_BVEC4 => "bvec4(floatBitsToUint(" + buffer + "[" + index + "].xyzw))",
            SL.DataType.TYPE_INT   => "floatBitsToInt(" + buffer + "[" + index + "].x)",
            SL.DataType.TYPE_IVEC2 => "floatBitsToInt(" + buffer + "[" + index + "].xy)",
            SL.DataType.TYPE_IVEC3 => "floatBitsToInt(" + buffer + "[" + index + "].xyz)",
            SL.DataType.TYPE_IVEC4 => "floatBitsToInt(" + buffer + "[" + index + "].xyzw)",
            SL.DataType.TYPE_UINT  => "floatBitsToUint(" + buffer + "[" + index + "].x)",
            SL.DataType.TYPE_UVEC2 => "floatBitsToUint(" + buffer + "[" + index + "].xy)",
            SL.DataType.TYPE_UVEC3 => "floatBitsToUint(" + buffer + "[" + index + "].xyz)",
            SL.DataType.TYPE_UVEC4 => "floatBitsToUint(" + buffer + "[" + index + "].xyzw)",
            SL.DataType.TYPE_FLOAT => "(" + buffer + "[" + index + "].x)",
            SL.DataType.TYPE_VEC2  => "(" + buffer + "[" + index + "].xy)",
            SL.DataType.TYPE_VEC3  => "(" + buffer + "[" + index + "].xyz)",
            SL.DataType.TYPE_VEC4  => "(" + buffer + "[" + index + "].xyzw)",
            SL.DataType.TYPE_MAT2  => "mat2(" + buffer + "[" + index + "].xy," + buffer + "[" + index + "+1].xy)",
            SL.DataType.TYPE_MAT3  => "mat3(" + buffer + "[" + index + "].xyz," + buffer + "[" + index + "+1].xyz," + buffer + "[" + index + "+2].xyz)",
            SL.DataType.TYPE_MAT4  => "mat4(" + buffer + "[" + index + "].xyzw," + buffer + "[" + index + "+1].xyzw," + buffer + "[" + index + "+2].xyzw," + buffer + "[" + index + "+3].xyzw)",
            _ => ERR_FAIL_V("void"),
        };

    private static string Interpstr(SL.DataInterpolation interp) => interp switch
    {
        SL.DataInterpolation.INTERPOLATION_FLAT    => "flat ",
        SL.DataInterpolation.INTERPOLATION_SMOOTH  => "",
        SL.DataInterpolation.INTERPOLATION_DEFAULT => "",
        _ => "",
    };

    private static string Mkid(string id) =>
        "m_" + id.Replace("__", "_dus_").Replace("__", "_dus_"); //doubleunderscore is reserved in glsl

    private static string Mktab(int level) => new('\t', level);

    private static string Opstr(SL.Operator op) => SL.GetOperatorText(op);

    private static string Prestr(SL.DataPrecision pres, bool forceHighp = false)
    {
        switch (pres)
        {
            case SL.DataPrecision.PRECISION_LOWP:
                return "lowp ";
            case SL.DataPrecision.PRECISION_MEDIUMP:
                return "mediump ";
            case SL.DataPrecision.PRECISION_HIGHP:
                return "highp ";
            case SL.DataPrecision.PRECISION_DEFAULT:
                return forceHighp ? "highp " : "";
            default:
                break;
        }
        return "";
    }

    private static string Qualstr(SL.ArgumentQualifier qual) =>
        qual switch
        {
            SL.ArgumentQualifier.ARGUMENT_QUALIFIER_IN => "",
            SL.ArgumentQualifier.ARGUMENT_QUALIFIER_OUT => "out ",
            SL.ArgumentQualifier.ARGUMENT_QUALIFIER_INOUT => "inout ",
            _ => "",
        };

    private static string Typestr(SL.DataType type)
    {
        var typeName = SL.GetDatatypeName(type);

        if (!RS.Singleton.IsLowEnd && SL.IsSamplerType(type))
        {
            typeName = typeName.Replace("sampler", "texture"); //we use textures instead of samplers in Vulkan GLSL
        }
        return typeName;
    }

    private string GetSamplerName(SL.TextureFilter filter, SL.TextureRepeat repeat)
    {
        if (filter == SL.TextureFilter.FILTER_DEFAULT)
        {
            if (ERR_FAIL_COND_V(this.actions.DefaultFilter == SL.TextureFilter.FILTER_DEFAULT))
            {
                return "";
            }
            filter = this.actions.DefaultFilter;
        }
        if (repeat == SL.TextureRepeat.REPEAT_DEFAULT)
        {
            if (ERR_FAIL_COND_V(this.actions.DefaultRepeat == SL.TextureRepeat.REPEAT_DEFAULT))
            {
                return "";
            }

            repeat = this.actions.DefaultRepeat;
        }
        return this.actions.SamplerArrayName + "[" + ((int)filter + (int)(repeat == SL.TextureRepeat.REPEAT_ENABLE ? SL.TextureFilter.FILTER_DEFAULT : 0)) + "]";
    }

    private void DumpFunctionDeps(SL.ShaderNode node, string forFunc, Dictionary<string, string> funcCode, out string toAdd, HashSet<string> added)
    {
        toAdd = "";

        node.Functions.TryGetValue(forFunc, out var fn);

        if (ERR_FAIL_COND(fn == null))
        {
            return;
        }

        var usesFunctions = new List<string>();

        foreach (var e in fn!.UsesFunction)
        {
            usesFunctions.Add(e);
        }

        usesFunctions.Sort(); //ensure order is deterministic so the same shader is always produced

        for (var k = 0; k < usesFunctions.Count; k++)
        {
            if (added.Contains(usesFunctions[k]))
            {
                continue; //was added already
            }

            this.DumpFunctionDeps(node, usesFunctions[k], funcCode, out toAdd, added);

            var fnode = fn.FunctionNode;

            if (ERR_FAIL_COND(fnode == null))
            {
                return;
            }

            toAdd += "\n";

            var header = fnode!.ReturnType == SL.DataType.TYPE_STRUCT ? Mkid(fnode.ReturnStructName) : Typestr(fnode.ReturnType);

            if (fnode.ReturnArraySize > 0)
            {
                header += "[";
                header += fnode.ReturnArraySize;
                header += "]";
            }

            header += " ";
            header += Mkid(fnode.Name);
            header += "(";

            foreach (var argument in fnode.Arguments.Values.OrderBy(x => x.Index))
            {
                if (argument.Index > 0)
                {
                    header += ", ";
                }
                header += Constr(argument.IsConst);

                if (argument.Type == SL.DataType.TYPE_STRUCT)
                {
                    header += Qualstr(argument.Qualifier) + Mkid(argument.TypeStr) + " " + Mkid(argument.Name);
                }
                else
                {
                    header += Qualstr(argument.Qualifier) + Prestr(argument.Precision) + Typestr(argument.Type) + " " + Mkid(argument.Name);
                }

                if (argument.ArraySize > 0)
                {
                    header += "[";
                    header += argument.ArraySize;
                    header += "]";
                }
            }

            header += ")\n";
            toAdd += header;
            toAdd += funcCode[usesFunctions[k]];

            added.Add(usesFunctions[k]);
        }
    }

    private string DumpNodeCode(SL.Node node, int level, GeneratedCode genCode, IdentifierActions actions, DefaultIdentifierActions defaultActions, bool assigning, bool useScope = true)
    {
        var code = "";

        switch (node.Type)
        {
            case SL.Node.NodeType.TYPE_SHADER:
                {
                    var pnode = (SL.ShaderNode)node;

                    for (var i = 0; i < pnode.RenderModes.Count; i++)
                    {
                        if (defaultActions.RenderModeDefines.TryGetValue(pnode.RenderModes[i], out var value) && !this.usedRmodeDefines.Contains(pnode.RenderModes[i]))
                        {
                            genCode.Defines.Add(value);
                            this.usedRmodeDefines.Add(pnode.RenderModes[i]);
                        }

                        if (actions.RenderModeFlags.ContainsKey(pnode.RenderModes[i]))
                        {
                            actions.RenderModeFlags[pnode.RenderModes[i]] = true;
                        }

                        if (actions.RenderModeValues.TryGetValue(pnode.RenderModes[i], out var value1))
                        {
                            actions.RenderModeValues[pnode.RenderModes[i]] = (value1.Item2, value1.Item2);
                        }
                    }

                    // structs

                    for (var i = 0; i < pnode.Vstructs.Count; i++)
                    {
                        var st = pnode.Vstructs[i].ShaderStruct!;
                        var structCode = "";

                        structCode += "struct ";
                        structCode += Mkid(pnode.Vstructs[i].Name);
                        structCode += " ";
                        structCode += "{\n";
                        for (var j = 0; j < st.Members.Count; j++)
                        {
                            var m = st.Members[j];
                            if (m.Datatype == SL.DataType.TYPE_STRUCT)
                            {
                                structCode += Mkid(m.StructName);
                            }
                            else
                            {
                                structCode += Prestr(m.Precision);
                                structCode += Typestr(m.Datatype);
                            }
                            structCode += " ";
                            structCode += m.Name;
                            if (m.ArraySize > 0)
                            {
                                structCode += "[";
                                structCode += m.ArraySize;
                                structCode += "]";
                            }
                            structCode += ";\n";
                        }
                        structCode += "}";
                        structCode += ";\n";

                        for (var j = 0; j < (int)Stage.STAGE_MAX; j++)
                        {
                            genCode.StageGlobals[j] += structCode;
                        }
                    }

                    var maxTextureUniforms = 0;
                    var maxUniforms        = 0;

                    foreach (var e in pnode.Uniforms)
                    {
                        if (SL.IsSamplerType(e.Value.Type))
                        {
                            if (
                                e.Value.Hint is SL.ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE or
                                SL.ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE or
                                SL.ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE
                            )
                            {
                                continue; // Don't create uniforms in the generated code for these.
                            }
                            maxTextureUniforms++;
                        }
                        else
                        {
                            if (e.Value.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                            {
                                continue; // Instances are indexed directly, don't need index uniforms.
                            }

                            maxUniforms++;
                        }
                    }

                    genCode.TextureUniforms.Capacity = maxTextureUniforms;

                    var uniformSizes      = new uint[maxUniforms];
                    var uniformAlignments = new int[maxUniforms];
                    var uniformDefines    = new string[maxUniforms];

                    var usesUniforms = false;

                    var uniformNames = new List<string>();

                    foreach (var e in pnode.Uniforms)
                    {
                        uniformNames.Add(e.Key);
                    }

                    uniformNames.Sort(); //ensure order is deterministic so the same shader is always produced

                    for (var k = 0; k < uniformNames.Count; k++)
                    {
                        var uniformName = uniformNames[k];
                        var uniform     = pnode.Uniforms[uniformName];

                        var ucode = "";

                        if (uniform.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                        {
                            //insert, but don't generate any code.
                            actions.Uniforms.Add(uniformName, uniform);
                            continue; // Instances are indexed directly, don't need index uniforms.
                        }

                        if (
                            uniform.Hint is SL.ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE or
                            SL.ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE or
                            SL.ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE
                        )
                        {
                            continue; // Don't create uniforms in the generated code for these.
                        }

                        if (SL.IsSamplerType(uniform.Type))
                        {
                            // Texture layouts are different for OpenGL GLSL and Vulkan GLSL
                            if (!RS.Singleton.IsLowEnd)
                            {
                                ucode = "layout(set = " + this.actions.TextureLayoutSet + ", binding = " + this.actions.BaseTextureBindingIndex + uniform.TextureBinding + ") ";
                            }
                            ucode += "uniform ";
                        }

                        var isBufferGlobal = !SL.IsSamplerType(uniform.Type) && uniform.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL;

                        if (isBufferGlobal)
                        {
                            //this is an integer to index the global table
                            ucode += Typestr(SL.DataType.TYPE_UINT);
                        }
                        else
                        {
                            ucode += Prestr(uniform.Precision, SL.IsFloatType(uniform.Type));
                            ucode += Typestr(uniform.Type);
                        }

                        ucode += " " + Mkid(uniformName);

                        if (uniform.ArraySize > 0)
                        {
                            ucode += "[";
                            ucode += uniform.ArraySize;
                            ucode += "]";
                        }
                        ucode += ";\n";

                        if (SL.IsSamplerType(uniform.Type))
                        {
                            for (var j = 0; j < (int)Stage.STAGE_MAX; j++)
                            {
                                genCode.StageGlobals[j] += ucode;
                            }

                            var texture = new GeneratedCode.Texture
                            {
                                Name      = uniformName,
                                Hint      = uniform.Hint,
                                Type      = uniform.Type,
                                UseColor  = uniform.UseColor,
                                Filter    = uniform.Filter,
                                Repeat    = uniform.Repeat,
                                Global    = uniform.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL,
                                ArraySize = uniform.ArraySize
                            };
                            if (texture.Global)
                            {
                                genCode.UsesGlobalTextures = true;
                            }

                            genCode.TextureUniforms[uniform.TextureOrder] = texture;
                        }
                        else
                        {
                            if (!usesUniforms)
                            {
                                usesUniforms = true;
                            }

                            uniformDefines[uniform.Order] = ucode;

                            if (isBufferGlobal)
                            {
                                //globals are indices into the global table
                                uniformSizes[uniform.Order]      = SL.GetDatatypeSize(SL.DataType.TYPE_UINT);
                                uniformAlignments[uniform.Order] = GetDatatypeAlignment(SL.DataType.TYPE_UINT);
                            }
                            else
                            {
                                // The following code enforces a 16-byte alignment of uniform arrays.
                                if (uniform.ArraySize > 0)
                                {
                                    var size = SL.GetDatatypeSize(uniform.Type) * uniform.ArraySize;
                                    var m = 16 * uniform.ArraySize;
                                    if (size % m != 0)
                                    {
                                        size += m - size % m;
                                    }
                                    uniformSizes[uniform.Order]      = size;
                                    uniformAlignments[uniform.Order] = 16;
                                }
                                else
                                {
                                    uniformSizes[uniform.Order]      = SL.GetDatatypeSize(uniform.Type);
                                    uniformAlignments[uniform.Order] = GetDatatypeAlignment(uniform.Type);
                                }
                            }
                        }

                        actions.Uniforms.Add(uniformName, uniform);
                    }

                    for (var i = 0; i < maxUniforms; i++)
                    {
                        genCode.Uniforms += uniformDefines[i];
                    }

                    // add up
                    var offset = 0u;
                    for (var i = 0; i < uniformSizes.Length; i++)
                    {
                        var align = offset % uniformAlignments[i];

                        if (align != 0)
                        {
                            offset += (uint)(uniformAlignments[i] - align);
                        }

                        genCode.UniformOffsets.Add(offset);

                        offset += uniformSizes[i];
                    }

                    genCode.UniformTotalSize = offset;

                    if (genCode.UniformTotalSize % 16 != 0)
                    {
                        //UBO sizes must be multiples of 16
                        genCode.UniformTotalSize += 16 - genCode.UniformTotalSize % 16;
                    }

                    var index = defaultActions.BaseVaryingIndex;

                    var varFragToLight = new List<(string, SL.ShaderNode.Varying)>();
                    var varyingNames   = new List<string>();

                    foreach (var e in pnode.Varyings)
                    {
                        varyingNames.Add(e.Key);
                    }

                    varyingNames.Sort(); //ensure order is deterministic so the same shader is always produced

                    for (var k = 0; k < varyingNames.Count; k++)
                    {
                        var varyingName = varyingNames[k];
                        var varying = pnode.Varyings[varyingName];

                        if (varying.Stage is SL.ShaderNode.Varying.StageKind.STAGE_FRAGMENT_TO_LIGHT or SL.ShaderNode.Varying.StageKind.STAGE_FRAGMENT)
                        {
                            varFragToLight.Add((varyingName, varying));
                            this.fragmentVaryings.Add(varyingName);

                            continue;
                        }
                        if (varying.Type < SL.DataType.TYPE_INT)
                        {
                            continue; // Ignore boolean types to prevent crashing (if varying is just declared).
                        }

                        var vcode       = "";
                        var interpMode = Interpstr(varying.Interpolation);
                        vcode += Prestr(varying.Precision, SL.IsFloatType(varying.Type));
                        vcode += Typestr(varying.Type);
                        vcode += " " + Mkid(varyingName);

                        var inc = 1U;

                        if (varying.ArraySize > 0)
                        {
                            inc = varying.ArraySize;

                            vcode += "[";
                            vcode += varying.ArraySize;
                            vcode += "]";
                        }

                        switch (varying.Type)
                        {
                            case SL.DataType.TYPE_MAT2:
                                inc *= 2U;
                                break;
                            case SL.DataType.TYPE_MAT3:
                                inc *= 3U;
                                break;
                            case SL.DataType.TYPE_MAT4:
                                inc *= 4U;
                                break;
                            default:
                                break;
                        }

                        vcode += ";\n";
                        // GLSL ES 3.0 does not allow layout qualifiers for varyings
                        if (!RS.Singleton.IsLowEnd)
                        {
                            genCode.StageGlobals[(int)Stage.STAGE_VERTEX] += "layout(location=" + index + ") ";
                            genCode.StageGlobals[(int)Stage.STAGE_FRAGMENT] += "layout(location=" + index + ") ";
                        }
                        genCode.StageGlobals[(int)Stage.STAGE_VERTEX]   += interpMode + "out " + vcode;
                        genCode.StageGlobals[(int)Stage.STAGE_FRAGMENT] += interpMode + "in " + vcode;

                        index += inc;
                    }

                    if (varFragToLight.Count > 0)
                    {
                        var gcode = "\n\nstruct {\n";
                        foreach (var e in varFragToLight)
                        {
                            gcode += "\t" + Prestr(e.Item2.Precision) + Typestr(e.Item2.Type) + " " + Mkid(e.Item1);
                            if (e.Item2.ArraySize > 0)
                            {
                                gcode += "[";
                                gcode += e.Item2.ArraySize;
                                gcode += "]";
                            }
                            gcode += ";\n";
                        }
                        gcode += "} frag_to_light;\n";
                        genCode.StageGlobals[(int)Stage.STAGE_FRAGMENT] += gcode;
                    }

                    foreach (var vconstant in pnode.Vconstants.Values.OrderBy(x => x.Index))
                    {
                        var cnode = vconstant;
                        var gcode = "";
                        gcode += Constr(true);
                        gcode += Prestr(cnode.Precision, SL.IsFloatType(cnode.Type));
                        if (cnode.Type == SL.DataType.TYPE_STRUCT)
                        {
                            gcode += Mkid(cnode.TypeStr);
                        }
                        else
                        {
                            gcode += Typestr(cnode.Type);
                        }
                        gcode += " " + Mkid(cnode.Name);
                        if (cnode.ArraySize > 0)
                        {
                            gcode += "[";
                            gcode += cnode.ArraySize;
                            gcode += "]";
                        }
                        gcode += "=";
                        gcode += this.DumpNodeCode(cnode.Initializer!, level, genCode, actions, defaultActions, assigning);
                        gcode += ";\n";
                        for (var j = 0; j < (int)Stage.STAGE_MAX; j++)
                        {
                            genCode.StageGlobals[j] += gcode;
                        }
                    }

                    var functionCode = new Dictionary<string, string>();

                    //code for functions
                    foreach (var fn in pnode.Functions.Values.OrderBy(x => x.Index))
                    {
                        var fnode            = fn.FunctionNode;
                        this.function        = fnode;
                        this.currentFuncName = fnode.Name;

                        functionCode[fnode.Name] = this.DumpNodeCode(fnode.Body!, level + 1, genCode, actions, defaultActions, assigning);

                        this.function = null;
                    }

                    //place functions in actual code

                    foreach (var fn in pnode.Functions.Values)
                    {
                        var fnode            = fn.FunctionNode;
                        this.function        = fnode;
                        this.currentFuncName = fnode.Name;

                        if (actions.EntryPointStages.TryGetValue(fnode.Name, out var stage))
                        {
                            var addedFuncsPerStage = new HashSet<string>((int)stage);

                            this.DumpFunctionDeps(pnode, fnode.Name, functionCode, out var toAdd, addedFuncsPerStage);

                            genCode.StageGlobals[(int)stage] = toAdd;

                            genCode.Code[fnode.Name] = functionCode[fnode.Name];
                        }

                        this.function = null;
                    }

                    //code+=dumnodeCode(pnode.body,p_level);
                }
                break;
            case SL.Node.NodeType.TYPE_STRUCT:
                break;
            case SL.Node.NodeType.TYPE_FUNCTION:
                break;
            case SL.Node.NodeType.TYPE_BLOCK:
                {
                    var bnode = (SL.BlockNode)node;

                    //variables
                    if (!bnode.SingleStatement)
                    {
                        code += Mktab(level - 1) + "{\n";
                    }

                    for (var i = 0; i < bnode.Statements.Count; i++)
                    {
                        var scode = this.DumpNodeCode(bnode.Statements[i], level, genCode, actions, defaultActions, assigning);

                        if (bnode.Statements[i].Type == SL.Node.NodeType.TYPE_CONTROL_FLOW || bnode.SingleStatement)
                        {
                            code += scode; //use directly
                            if (bnode.UseCommaBetweenStatements && i + 1 < bnode.Statements.Count)
                            {
                                code += ",";
                            }
                        }
                        else
                        {
                            code += Mktab(level) + scode + ";\n";
                        }
                    }
                    if (!bnode.SingleStatement)
                    {
                        code += Mktab(level - 1) + "}\n";
                    }

                }
                break;
            case SL.Node.NodeType.TYPE_VARIABLE_DECLARATION:
                {
                    var vdnode = (SL.VariableDeclarationNode)node;

                    var declaration = "";
                    declaration += Constr(vdnode.IsConst);
                    if (vdnode.Datatype == SL.DataType.TYPE_STRUCT)
                    {
                        declaration += Mkid(vdnode.StructName);
                    }
                    else
                    {
                        declaration += Prestr(vdnode.Precision) + Typestr(vdnode.Datatype);
                    }
                    declaration += " ";
                    for (var i = 0; i < vdnode.Declarations.Count; i++)
                    {
                        var isArray = vdnode.Declarations[i].Size > 0;
                        if (i > 0)
                        {
                            declaration += ",";
                        }
                        declaration += Mkid(vdnode.Declarations[i].Name);
                        if (isArray)
                        {
                            declaration += "[";
                            if (vdnode.Declarations[i].SizeExpression != null)
                            {
                                declaration += this.DumpNodeCode(vdnode.Declarations[i].SizeExpression!, level, genCode, actions, defaultActions, assigning);
                            }
                            else
                            {
                                declaration += vdnode.Declarations[i].Size;
                            }
                            declaration += "]";
                        }

                        if (!isArray || vdnode.Declarations[i].SingleExpression)
                        {
                            if (vdnode.Declarations[i].Initializer.Count != 0)
                            {
                                declaration += "=";
                                declaration += this.DumpNodeCode(vdnode.Declarations[i].Initializer[0], level, genCode, actions, defaultActions, assigning);
                            }
                        }
                        else
                        {
                            var size = vdnode.Declarations[i].Initializer.Count;
                            if (size > 0)
                            {
                                declaration += "=";
                                if (vdnode.Datatype == SL.DataType.TYPE_STRUCT)
                                {
                                    declaration += Mkid(vdnode.StructName);
                                }
                                else
                                {
                                    declaration += Typestr(vdnode.Datatype);
                                }
                                declaration += "[";
                                declaration += size;
                                declaration += "]";
                                declaration += "(";

                                for (var j = 0; j < size; j++)
                                {
                                    if (j > 0)
                                    {
                                        declaration += ",";
                                    }
                                    declaration += this.DumpNodeCode(vdnode.Declarations[i].Initializer[j], level, genCode, actions, defaultActions, assigning);
                                }
                                declaration += ")";
                            }
                        }
                    }

                    code += declaration;
                }
                break;
            case SL.Node.NodeType.TYPE_VARIABLE:
                {
                    var vnode = (SL.VariableNode)node;
                    var useFragmentVarying = false;

                    if (!vnode.IsLocal && !(actions.EntryPointStages.ContainsKey(this.currentFuncName!) && actions.EntryPointStages[this.currentFuncName] == Stage.STAGE_VERTEX))
                    {
                        if (assigning)
                        {
                            if (this.shader!.Varyings.ContainsKey(vnode.Name))
                            {
                                useFragmentVarying = true;
                            }
                        }
                        else
                        {
                            if (this.fragmentVaryings.Contains(vnode.Name))
                            {
                                useFragmentVarying = true;
                            }
                        }
                    }

                    if (assigning && actions.WriteFlagPointers.ContainsKey(vnode.Name))
                    {
                        actions.WriteFlagPointers[vnode.Name] = true;
                    }

                    if (defaultActions.UsageDefines.TryGetValue(vnode.Name, out var value) && !this.usedNameDefines.Contains(vnode.Name))
                    {
                        var define = value;
                        if (define.StartsWith("@"))
                        {
                            define = defaultActions.UsageDefines[define.Substring(1, define.Length)];
                        }
                        genCode.Defines.Add(define);
                        this.usedNameDefines.Add(vnode.Name);
                    }

                    if (actions.UsageFlagPointers.ContainsKey(vnode.Name) && !this.usedFlagPointers.Contains(vnode.Name))
                    {
                        actions.UsageFlagPointers[vnode.Name] = true;
                        this.usedFlagPointers.Add(vnode.Name);
                    }

                    if (defaultActions.Renames.TryGetValue(vnode.Name, out var rename))
                    {
                        code = rename;
                    }
                    else
                    {
                        if (this.shader!.Uniforms.TryGetValue(vnode.Name, out var u))
                        {
                            //its a uniform!
                            if (u.TextureOrder >= 0)
                            {
                                var name = vnode.Name;
                                if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE)
                                {
                                    name = "color_buffer";
                                    if (u.Filter >= SL.TextureFilter.FILTER_NEAREST_MIPMAP)
                                    {
                                        genCode.UsesScreenTextureMipmaps = true;
                                    }
                                    genCode.UsesScreenTexture = true;
                                }
                                else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE)
                                {
                                    name = "normal_roughness_buffer";
                                    genCode.UsesNormalRoughnessTexture = true;
                                }
                                else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE)
                                {
                                    name = "depth_buffer";
                                    genCode.UsesDepthTexture = true;
                                }
                                else
                                {
                                    name = Mkid(vnode.Name); //texture, use as is
                                }

                                code = name;
                            }
                            else
                            {
                                //a scalar or vector
                                if (u.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL)
                                {
                                    code = this.actions.BaseUniformString + Mkid(vnode.Name); //texture, use as is
                                                                                           //global variable, this means the code points to an index to the global table
                                    code = GetGlobalShaderUniformFromTypeAndIndex(defaultActions.GlobalBufferArrayVariable, code, u.Type);
                                }
                                else if (u.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                                {
                                    //instance variable, index it as such
                                    code = $"({defaultActions.InstanceUniformIndexVariable}+{u.InstanceIndex})";
                                    code = GetGlobalShaderUniformFromTypeAndIndex(defaultActions.GlobalBufferArrayVariable, code, u.Type);
                                }
                                else
                                {
                                    //regular uniform, index from UBO
                                    code = this.actions.BaseUniformString + Mkid(vnode.Name);
                                }
                            }

                        }
                        else
                        {
                            if (useFragmentVarying)
                            {
                                code = "frag_to_light.";
                            }
                            code += Mkid(vnode.Name); //its something else (local var most likely) use as is
                        }
                    }

                    if (vnode.Name == this.timeName)
                    {
                        if (actions.EntryPointStages.TryGetValue(this.currentFuncName, out var value1) && value1 == Stage.STAGE_VERTEX)
                        {
                            genCode.UsesVertexTime = true;
                        }
                        if (actions.EntryPointStages.TryGetValue(this.currentFuncName, out var value2) && value2 == Stage.STAGE_FRAGMENT)
                        {
                            genCode.UsesFragmentTime = true;
                        }
                    }

                }
                break;
            case SL.Node.NodeType.TYPE_ARRAY_CONSTRUCT:
                {
                    var acnode = (SL.ArrayConstructNode)node;
                    var sz = acnode.Initializer.Count;
                    if (acnode.Datatype == SL.DataType.TYPE_STRUCT)
                    {
                        code += Mkid(acnode.StructName);
                    }
                    else
                    {
                        code += Typestr(acnode.Datatype);
                    }
                    code += "[";
                    code += acnode.Initializer.Count;
                    code += "]";
                    code += "(";
                    for (var i = 0; i < sz; i++)
                    {
                        code += this.DumpNodeCode(acnode.Initializer[i], level, genCode, actions, defaultActions, assigning);
                        if (i != sz - 1)
                        {
                            code += ", ";
                        }
                    }
                    code += ")";
                }
                break;
            case SL.Node.NodeType.TYPE_ARRAY:
                {
                    var anode = (SL.ArrayNode)node;
                    var useFragmentVarying = false;

                    if (!anode.IsLocal && !(actions.EntryPointStages.ContainsKey(this.currentFuncName) && actions.EntryPointStages[this.currentFuncName] == Stage.STAGE_VERTEX))
                    {
                        if (anode.AssignExpression != null && this.shader!.Varyings.ContainsKey(anode.Name))
                        {
                            useFragmentVarying = true;
                        }
                        else
                        {
                            if (assigning)
                            {
                                if (this.shader!.Varyings.ContainsKey(anode.Name))
                                {
                                    useFragmentVarying = true;
                                }
                            }
                            else
                            {
                                if (this.fragmentVaryings.Contains(anode.Name))
                                {
                                    useFragmentVarying = true;
                                }
                            }
                        }
                    }

                    if (assigning && actions.WriteFlagPointers.ContainsKey(anode.Name))
                    {
                        actions.WriteFlagPointers[anode.Name] = true;
                    }

                    if (defaultActions.UsageDefines.TryGetValue(anode.Name, out var value) && !this.usedNameDefines.Contains(anode.Name))
                    {
                        var define = value;
                        if (define.StartsWith("@"))
                        {
                            define = defaultActions.UsageDefines[define.Substring(1, define.Length)];
                        }
                        genCode.Defines.Add(define);
                        this.usedNameDefines.Add(anode.Name);
                    }

                    if (actions.UsageFlagPointers.ContainsKey(anode.Name) && !this.usedFlagPointers.Contains(anode.Name))
                    {
                        actions.UsageFlagPointers[anode.Name] = true;
                        this.usedFlagPointers.Add(anode.Name);
                    }

                    if (defaultActions.Renames.TryGetValue(anode.Name, out var rename))
                    {
                        code = rename;
                    }
                    else
                    {
                        if (this.shader!.Uniforms.TryGetValue(anode.Name, out var u))
                        {
                            //its a uniform!
                            if (u.TextureOrder >= 0)
                            {
                                code = Mkid(anode.Name); //texture, use as is
                            }
                            else
                            {
                                //a scalar or vector
                                if (u.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_GLOBAL)
                                {
                                    code = this.actions.BaseUniformString + Mkid(anode.Name); //texture, use as is
                                                                                           //global variable, this means the code points to an index to the global table
                                    code = GetGlobalShaderUniformFromTypeAndIndex(defaultActions.GlobalBufferArrayVariable, code, u.Type);
                                }
                                else if (u.Scope == SL.ShaderNode.Uniform.ScopeKind.SCOPE_INSTANCE)
                                {
                                    //instance variable, index it as such
                                    code = "(" + defaultActions.InstanceUniformIndexVariable + "+" + u.InstanceIndex + ")";
                                    code = GetGlobalShaderUniformFromTypeAndIndex(defaultActions.GlobalBufferArrayVariable, code, u.Type);
                                }
                                else
                                {
                                    //regular uniform, index from UBO
                                    code = this.actions.BaseUniformString + Mkid(anode.Name);
                                }
                            }
                        }
                        else
                        {
                            if (useFragmentVarying)
                            {
                                code = "frag_to_light.";
                            }
                            code += Mkid(anode.Name);
                        }
                    }

                    if (anode.CallExpression != null)
                    {
                        code += ".";
                        code += this.DumpNodeCode(anode.CallExpression, level, genCode, actions, defaultActions, assigning, false);
                    }
                    else if (anode.IndexExpression != null)
                    {
                        code += "[";
                        code += this.DumpNodeCode(anode.IndexExpression, level, genCode, actions, defaultActions, assigning);
                        code += "]";
                    }
                    else if (anode.AssignExpression != null)
                    {
                        code += "=";
                        code += this.DumpNodeCode(anode.AssignExpression, level, genCode, actions, defaultActions, true, false);
                    }

                    if (anode.Name == this.timeName)
                    {
                        if (actions.EntryPointStages.TryGetValue(this.currentFuncName, out var value1) && value1 == Stage.STAGE_VERTEX)
                        {
                            genCode.UsesVertexTime = true;
                        }
                        if (actions.EntryPointStages.TryGetValue(this.currentFuncName, out var value2) && value2 == Stage.STAGE_FRAGMENT)
                        {
                            genCode.UsesFragmentTime = true;
                        }
                    }

                }
                break;
            case SL.Node.NodeType.TYPE_CONSTANT:
                {
                    var cnode = (SL.ConstantNode)node;

                    if (cnode.ArraySize == 0)
                    {
                        return GetConstantText(cnode.Datatype, cnode.Values);
                    }
                    else
                    {
                        if (cnode.GetDatatype() == SL.DataType.TYPE_STRUCT)
                        {
                            code += Mkid(cnode.StructName);
                        }
                        else
                        {
                            code += Typestr(cnode.Datatype);
                        }
                        code += "[";
                        code += cnode.ArraySize;
                        code += "]";
                        code += "(";
                        for (var i = 0; i < cnode.ArraySize; i++)
                        {
                            if (i > 0)
                            {
                                code += ",";
                            }
                            else
                            {
                                code += "";
                            }
                            code += this.DumpNodeCode(cnode.ArrayDeclarations[0].Initializer[i], level, genCode, actions, defaultActions, assigning);
                        }
                        code += ")";
                    }

                }
                break;
            case SL.Node.NodeType.TYPE_OPERATOR:
                {
                    var onode = (SL.OperatorNode)node;

                    switch (onode.Op)
                    {
                        case SL.Operator.OP_ASSIGN:
                        case SL.Operator.OP_ASSIGN_ADD:
                        case SL.Operator.OP_ASSIGN_SUB:
                        case SL.Operator.OP_ASSIGN_MUL:
                        case SL.Operator.OP_ASSIGN_DIV:
                        case SL.Operator.OP_ASSIGN_SHIFT_LEFT:
                        case SL.Operator.OP_ASSIGN_SHIFT_RIGHT:
                        case SL.Operator.OP_ASSIGN_MOD:
                        case SL.Operator.OP_ASSIGN_BIT_AND:
                        case SL.Operator.OP_ASSIGN_BIT_OR:
                        case SL.Operator.OP_ASSIGN_BIT_XOR:
                            code = this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, true) + Opstr(onode.Op) + this.DumpNodeCode(onode.Arguments[1], level, genCode, actions, defaultActions, assigning);
                            break;
                        case SL.Operator.OP_BIT_INVERT:
                        case SL.Operator.OP_NEGATE:
                        case SL.Operator.OP_NOT:
                        case SL.Operator.OP_DECREMENT:
                        case SL.Operator.OP_INCREMENT:
                            code = Opstr(onode.Op) + this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, assigning);
                            break;
                        case SL.Operator.OP_POST_DECREMENT:
                        case SL.Operator.OP_POST_INCREMENT:
                            code = this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, assigning) + Opstr(onode.Op);
                            break;
                        case SL.Operator.OP_CALL:
                        case SL.Operator.OP_STRUCT:
                        case SL.Operator.OP_CONSTRUCT:
                            {
                                if (ERR_FAIL_COND_V(onode.Arguments[0].Type != SL.Node.NodeType.TYPE_VARIABLE))
                                {
                                    return "";
                                }

                                var vnode          = (SL.VariableNode)onode.Arguments[0];
                                var func           = default(SL.FunctionNode);
                                var isInternalFunc = this.internalFunctions.Contains(vnode.Name);

                                if (!isInternalFunc)
                                {
                                    if (this.shader!.Functions.TryGetValue(vnode.Name, out var fn))
                                    {
                                        func = fn.FunctionNode;
                                    }
                                }

                                var isTextureFunc          = false;
                                var isScreenTexture        = false;
                                var textureFuncNoUv        = false;
                                var textureFuncReturnsData = false;

                                if (onode.Op == SL.Operator.OP_STRUCT)
                                {
                                    code += Mkid(vnode.Name);
                                }
                                else if (onode.Op == SL.Operator.OP_CONSTRUCT)
                                {
                                    code += vnode.Name;
                                }
                                else
                                {
                                    if (actions.UsageFlagPointers.ContainsKey(vnode.Name) && !this.usedFlagPointers.Contains(vnode.Name))
                                    {
                                        actions.UsageFlagPointers[vnode.Name] = true;
                                        this.usedFlagPointers.Add(vnode.Name);
                                    }

                                    if (isInternalFunc)
                                    {
                                        code += vnode.Name;
                                        isTextureFunc          = this.textureFunctions.Contains(vnode.Name);
                                        textureFuncNoUv        = vnode.Name is "textureSize" or "textureQueryLevels";
                                        textureFuncReturnsData = textureFuncNoUv || vnode.Name == "textureQueryLod";
                                    }
                                    else if (defaultActions.Renames.TryGetValue(vnode.Name, out var renames))
                                    {
                                        code += renames;
                                    }
                                    else
                                    {
                                        code += Mkid(vnode.Name);
                                    }
                                }

                                code += "(";

                                // if color backbuffer, depth backbuffer or normal roughness texture is used,
                                // we will add logic to automatically switch between
                                // sampler2D and sampler2D array and vec2 UV and vec3 UV.
                                var multiviewUvNeeded = false;

                                var funcArguments = func?.Arguments.Values.OrderBy(x => x.Index).ToArray();

                                for (var i = 1; i < onode.Arguments.Count; i++)
                                {
                                    if (i > 1)
                                    {
                                        code += ", ";
                                    }

                                    var isOutQualifier = false;
                                    if (isInternalFunc)
                                    {
                                        isOutQualifier = SL.IsBuiltinFuncOutParameter(vnode.Name, i - 1);
                                    }
                                    else if (funcArguments != null)
                                    {
                                        var qualifier = funcArguments[i - 1].Qualifier;
                                        isOutQualifier = qualifier is SL.ArgumentQualifier.ARGUMENT_QUALIFIER_OUT or SL.ArgumentQualifier.ARGUMENT_QUALIFIER_INOUT;
                                    }

                                    if (isOutQualifier)
                                    {
                                        var name = "";
                                        var found = false;
                                        {
                                            var pnode = onode.Arguments[i];

                                            var done = false;
                                            do
                                            {
                                                switch (pnode.Type)
                                                {
                                                    case SL.Node.NodeType.TYPE_VARIABLE:
                                                        name  = ((SL.VariableNode)pnode).Name;
                                                        done  = true;
                                                        found = true;
                                                        break;
                                                    case SL.Node.NodeType.TYPE_MEMBER:
                                                        pnode = ((SL.MemberNode)pnode).Owner!;
                                                        break;
                                                    default:
                                                        done = true;
                                                        break;
                                                }
                                            } while (!done);
                                        }

                                        if (found && actions.WriteFlagPointers.ContainsKey(name))
                                        {
                                            actions.WriteFlagPointers[name] = true;
                                        }
                                    }

                                    var nodeCode = this.DumpNodeCode(onode.Arguments[i], level, genCode, actions, defaultActions, assigning);
                                    if (isTextureFunc && i == 1)
                                    {
                                        // If we're doing a texture lookup we need to check our texture argument
                                        var textureUniform = "";
                                        var correctTextureUniform = false;

                                        switch (onode.Arguments[i].Type)
                                        {
                                            case SL.Node.NodeType.TYPE_VARIABLE:
                                                {
                                                    var varnode = (SL.VariableNode)onode.Arguments[i];
                                                    textureUniform = varnode.Name;
                                                    correctTextureUniform = true;
                                                }
                                                break;
                                            case SL.Node.NodeType.TYPE_ARRAY:
                                                {
                                                    var anode = (SL.ArrayNode)onode.Arguments[i];
                                                    textureUniform = anode.Name;
                                                    correctTextureUniform = true;
                                                }
                                                break;
                                            default:
                                                break;
                                        }

                                        if (correctTextureUniform && !RS.Singleton.IsLowEnd)
                                        {
                                            // Need to map from texture to sampler in order to sample when using Vulkan GLSL.
                                            var samplerName              = "";
                                            var isDepthTexture           = false;
                                            var isNormalRoughnessTexture = false;

                                            if (this.actions.CustomSamplers.TryGetValue(textureUniform, out var customSampler))
                                            {
                                                samplerName = customSampler;
                                            }
                                            else
                                            {
                                                if (this.shader!.Uniforms.TryGetValue(textureUniform, out var u))
                                                {
                                                    if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE)
                                                    {
                                                        isScreenTexture = true;
                                                    }
                                                    else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE)
                                                    {
                                                        isDepthTexture = true;
                                                    }
                                                    else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE)
                                                    {
                                                        isNormalRoughnessTexture = true;
                                                    }
                                                    samplerName = this.GetSamplerName(u.Filter, u.Repeat);
                                                }
                                                else
                                                {
                                                    var found = false;

                                                    foreach (var argument in this.function!.Arguments.Values.OrderBy(x => x.Index))
                                                    {
                                                        if (argument.Name == textureUniform)
                                                        {
                                                            if (argument.TexBuiltinCheck)
                                                            {
                                                                if (ERR_CONTINUE(!this.actions.CustomSamplers.ContainsKey(argument.TexBuiltin)))
                                                                {
                                                                    continue;
                                                                }

                                                                samplerName = this.actions.CustomSamplers[argument.TexBuiltin];
                                                                found = true;
                                                                break;
                                                            }
                                                            if (argument.TexArgumentCheck)
                                                            {
                                                                samplerName = this.GetSamplerName(argument.TexArgumentFilter, argument.TexArgumentRepeat);
                                                                found = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (!found)
                                                    {
                                                        //function was most likely unused, so use anything (compiler will remove it anyway)
                                                        samplerName = this.GetSamplerName(SL.TextureFilter.FILTER_DEFAULT, SL.TextureRepeat.REPEAT_DEFAULT);
                                                    }
                                                }
                                            }

                                            var dataTypeName = "";
                                            if (this.actions.CheckMultiviewSamplers && (isScreenTexture || isDepthTexture || isNormalRoughnessTexture))
                                            {
                                                dataTypeName = "multiviewSampler";
                                                multiviewUvNeeded = true;
                                            }
                                            else
                                            {
                                                dataTypeName = SL.GetDatatypeName(onode.Arguments[i].GetDatatype());
                                            }

                                            code += $"{dataTypeName}({nodeCode}, {samplerName})";
                                        }
                                        else if (this.actions.CheckMultiviewSamplers && correctTextureUniform && RS.Singleton.IsLowEnd)
                                        {
                                            // Texture function on low end hardware (i.e. OpenGL).
                                            // We just need to know if the texture supports multiview.

                                            if (this.shader!.Uniforms.TryGetValue(textureUniform, out var u))
                                            {
                                                if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_SCREEN_TEXTURE)
                                                {
                                                    multiviewUvNeeded = true;
                                                }
                                                else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_DEPTH_TEXTURE)
                                                {
                                                    multiviewUvNeeded = true;
                                                }
                                                else if (u.Hint == SL.ShaderNode.Uniform.HintKind.HINT_NORMAL_ROUGHNESS_TEXTURE)
                                                {
                                                    multiviewUvNeeded = true;
                                                }
                                            }

                                            code += nodeCode;
                                        }
                                        else
                                        {
                                            code += nodeCode;
                                        }
                                    }
                                    else if (multiviewUvNeeded && !textureFuncNoUv && i == 2)
                                    {
                                        // UV coordinate after using color, depth or normal roughness texture.
                                        nodeCode = $"multiview_uv({nodeCode}.xy)";

                                        code += nodeCode;
                                    }
                                    else
                                    {
                                        code += nodeCode;
                                    }
                                }
                                code += ")";
                                if (isScreenTexture && !textureFuncReturnsData && this.actions.ApplyLuminanceMultiplier)
                                {
                                    code = $"({code} * vec4(vec3(sc_luminance_multiplier), 1.0))";
                                }
                            }
                            break;
                        case SL.Operator.OP_INDEX:
                            code += this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, assigning);
                            code += "[";
                            code += this.DumpNodeCode(onode.Arguments[1], level, genCode, actions, defaultActions, assigning);
                            code += "]";
                            break;
                        case SL.Operator.OP_SELECT_IF:
                            code += "(";
                            code += this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, assigning);
                            code += "?";
                            code += this.DumpNodeCode(onode.Arguments[1], level, genCode, actions, defaultActions, assigning);
                            code += ":";
                            code += this.DumpNodeCode(onode.Arguments[2], level, genCode, actions, defaultActions, assigning);
                            code += ")";
                            break;
                        case SL.Operator.OP_EMPTY:
                                // Semicolon (or empty statement) - ignored.
                            break;

                        default:
                            {
                                if (useScope)
                                {
                                    code += "(";
                                }
                                code += this.DumpNodeCode(onode.Arguments[0], level, genCode, actions, defaultActions, assigning) + Opstr(onode.Op) + this.DumpNodeCode(onode.Arguments[1], level, genCode, actions, defaultActions, assigning);
                                if (useScope)
                                {
                                    code += ")";
                                }
                                break;
                            }
                    }
                }
                break;
            case SL.Node.NodeType.TYPE_CONTROL_FLOW:
                {
                    var cfnode = (SL.ControlFlowNode)node;
                    if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_IF)
                    {
                        code += Mktab(level) + "if (" + this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning) + ")\n";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                        if (cfnode.Blocks.Count == 2)
                        {
                            code += Mktab(level) + "else\n";
                            code += this.DumpNodeCode(cfnode.Blocks[1], level + 1, genCode, actions, defaultActions, assigning);
                        }
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_SWITCH)
                    {
                        code += $"{Mktab(level)}switch ({this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning)})\n";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_CASE)
                    {
                        code += $"{Mktab(level)}case {this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning)}:\n";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_DEFAULT)
                    {
                        code += Mktab(level) + "default:\n";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_DO)
                    {
                        code += $"{Mktab(level)}do";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                        code += $"{Mktab(level)}while ({this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning)});";
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_WHILE)
                    {
                        code += $"{Mktab(level)}while ({this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning)})\n";
                        code += this.DumpNodeCode(cfnode.Blocks[0], level + 1, genCode, actions, defaultActions, assigning);
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_FOR)
                    {
                        var left = this.DumpNodeCode(cfnode.Blocks[0], level, genCode, actions, defaultActions, assigning);
                        var middle = this.DumpNodeCode(cfnode.Blocks[1], level, genCode, actions, defaultActions, assigning);
                        var right = this.DumpNodeCode(cfnode.Blocks[2], level, genCode, actions, defaultActions, assigning);
                        code += Mktab(level) + "for (" + left + ";" + middle + ";" + right + ")\n";
                        code += this.DumpNodeCode(cfnode.Blocks[3], level + 1, genCode, actions, defaultActions, assigning);

                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_RETURN)
                    {
                        code = cfnode.Expressions.Count > 0
                            ? $"return {this.DumpNodeCode(cfnode.Expressions[0], level, genCode, actions, defaultActions, assigning)};"
                            : "return;";
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_DISCARD)
                    {
                        if (actions.UsageFlagPointers.ContainsKey("DISCARD") && !this.usedFlagPointers.Contains("DISCARD"))
                        {
                            actions.UsageFlagPointers["DISCARD"] = true;
                            this.usedFlagPointers.Add("DISCARD");
                        }

                        code = "discard;";
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_CONTINUE)
                    {
                        code = "continue;";
                    }
                    else if (cfnode.FlowOp == SL.FlowOperation.FLOW_OP_BREAK)
                    {
                        code = "break;";
                    }

                }
                break;
            case SL.Node.NodeType.TYPE_MEMBER:
                {
                    var mnode = (SL.MemberNode)node;
                    code = this.DumpNodeCode(mnode.Owner!, level, genCode, actions, defaultActions, assigning) + "." + mnode.Name;
                    if (mnode.IndexExpression != null)
                    {
                        code += "[";
                        code += this.DumpNodeCode(mnode.IndexExpression, level, genCode, actions, defaultActions, assigning);
                        code += "]";
                    }
                    else if (mnode.AssignExpression != null)
                    {
                        code += "=";
                        code += this.DumpNodeCode(mnode.AssignExpression, level, genCode, actions, defaultActions, true, false);
                    }
                    else if (mnode.CallExpression != null)
                    {
                        code += ".";
                        code += this.DumpNodeCode(mnode.CallExpression, level, genCode, actions, defaultActions, assigning, false);
                    }
                }
                break;
        }

        return code;
    }

    public Error Compile(RS.ShaderMode mode, string code, IdentifierActions actions, string path, out GeneratedCode genCode)
    {
        genCode = new();

        var info = new SL.ShaderCompileInfo
        {
            Functions                   = ShaderTypes.Singleton.GetFunctions(mode),
            RenderModes                 = ShaderTypes.Singleton.GetModes(mode),
            ShaderTypes                 = ShaderTypes.Singleton.Types,
            GlobalShaderUniformTypeFunc = GetGlobalShaderUniformType
        };

        var err = this.parser.Compile(code, info);

        if (err != Error.OK)
        {
            var includePositions = this.parser.GetIncludePositions();

            var current  = "";
            var includes = new Dictionary<string, List<string>>
            {
                { "", new() }
            };
            var includeStack = new List<string>();
            var shaderLines  = code.Split("\n");

            // Reconstruct the files.
            for (var i = 0; i < shaderLines.Length; i++)
            {
                var l = shaderLines[i];
                if (l.StartsWith("@@>"))
                {
                    var incPath = l.ReplaceFirst("@@>", "");

                    l = "#include \"" + incPath + "\"";

                    includes[current].Add("#include \"" + incPath + "\""); // Restore the include directive
                    includeStack.Add(current);
                    current = incPath;
                    includes.Add(incPath, new());

                }
                else if (l.StartsWith("@@<"))
                {
                    if (includeStack.Count != 0)
                    {
                        current = includeStack[^1];
                        includeStack.Capacity = includeStack.Count - 1;
                    }
                }
                else
                {
                    includes[current].Add(l);
                }
            }

            // Print the files.
            foreach (var e in includes)
            {
                if (string.IsNullOrEmpty(e.Key))
                {
                    if (path == "")
                    {
                        PrintLine("--Main Shader--");
                    }
                    else
                    {
                        PrintLine("--" + path + "--");
                    }
                }
                else
                {
                    PrintLine("--" + e.Key + "--");
                }
                var errLine = -1;

                for (var i = 0; i < includePositions.Count; i++)
                {
                    if (includePositions[i].File == e.Key)
                    {
                        errLine = includePositions[i].Line;
                    }
                }
                var v = e.Value;

                for (var i = 0; i < v.Count; i++)
                {
                    if (i == errLine - 1)
                    {
                        // Mark the error line to be visible without having to look at
                        // the trace at the end.
                        PrintLine($"{i + 1:0.0000} | {v[i]}");
                    }
                    else
                    {
                        PrintLine($"{i + 1:0.00000} | {v[i]}");
                    }
                }
            }

            string file;
            int    line;

            if (includePositions.Count > 1)
            {
                file = includePositions[^1].File!;
                line = includePositions[^1].Line;
            }
            else
            {
                file = path;
                line = this.parser.GetErrorLine();
            }

            ErrPrintError(null!, file, line, this.parser.GetErrorText(), false, ErrorHandlerType.ERR_HANDLER_SHADER);
            return err;
        }

        genCode.Defines.Clear();
        genCode.Code.Clear();

        for (var i = 0; i < (int)Stage.STAGE_MAX; i++)
        {
            genCode.StageGlobals[i] = default;
        }

        genCode.UsesFragmentTime           = false;
        genCode.UsesVertexTime             = false;
        genCode.UsesGlobalTextures         = false;
        genCode.UsesScreenTextureMipmaps   = false;
        genCode.UsesScreenTexture          = false;
        genCode.UsesDepthTexture           = false;
        genCode.UsesNormalRoughnessTexture = false;

        this.usedNameDefines.Clear();
        this.usedRmodeDefines.Clear();
        this.usedFlagPointers.Clear();
        this.fragmentVaryings.Clear();

        this.shader   = this.parser.GetShader();
        this.function = null;

        this.DumpNodeCode(this.shader, 1, genCode, actions, this.actions, false);

        return Error.OK;
    }

    public void Initialize(DefaultIdentifierActions actions)
    {
        this.actions = actions;

        this.timeName = "TIME";

        SL.GetBuiltinFuncs(out var funcList);

        foreach (var item in funcList)
        {
            this.internalFunctions.Add(item);
        }

        this.textureFunctions.Add("texture");
        this.textureFunctions.Add("textureProj");
        this.textureFunctions.Add("textureLod");
        this.textureFunctions.Add("textureProjLod");
        this.textureFunctions.Add("textureGrad");
        this.textureFunctions.Add("textureProjGrad");
        this.textureFunctions.Add("textureGather");
        this.textureFunctions.Add("textureSize");
        this.textureFunctions.Add("textureQueryLod");
        this.textureFunctions.Add("textureQueryLevels");
        this.textureFunctions.Add("texelFetch");
    }
}
