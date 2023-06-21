namespace Godot.Net.SourceGenerators.Tests;
public partial class ShaderGeneratorTest
{
    private class Fixtures
    {
        public static (string, string)[] AditionalFiles { get; } = new[]
        {
            // (
            //     Path.GetFullPath("Godot.Net/Modules/Shaders/_included.glsl"),
            //     """
            //     #define M_PI 3.14159265359
            //     """
            // ),
            // (
            //     Path.GetFullPath("Godot.Net/Modules/Shaders/compute.glsl"),
            //     """
            //     #[compute]

            //     #version 450

            //     #VERSION_DEFINES


            //     #include "_included.glsl"

            //     void main() {
            //         vec3 static_light = vec3(0, 1, 0);
            //     }
            //     """
            // ),
            // (
            //     Path.GetFullPath("Godot.Net/Modules/Shaders/vertex_fragment.glsl"),
            //     """
            //     #[versions]

            //     lines = "#define MODE_LINES";

            //     #[vertex]

            //     #version 450

            //     #VERSION_DEFINES

            //     layout(location = 0) out vec3 uv_interp;

            //     void main() {

            //     #ifdef MODE_LINES
            //         uv_interp = vec3(0,0,1);
            //     #endif
            //     }

            //     #[fragment]

            //     #version 450

            //     #VERSION_DEFINES

            //     #include "_included.glsl"

            //     layout(location = 0) out vec4 dst_color;

            //     void main() {
            //         dst_color = vec4(1,1,0,0);
            //     }
            //     """
            // ),
            (
                Path.GetFullPath("Godot.Net/Drivers/GLES3/Shaders/.generated"),
                """
                canvas_occlusion.glsl
                vertex_fragment.glsl
                """
            ),
            (
                Path.GetFullPath("Godot.Net/Drivers/GLES3/Shaders/_included.glsl"),
                """
                #define M_PI 3.14159265359
                """
            ),
            (
                Path.GetFullPath("Godot.Net/Drivers/GLES3/Shaders/canvas_occlusion.glsl"),
                """
                /* clang-format off */
                #[modes]

                mode_sdf =
                mode_shadow = #define MODE_SHADOW
                mode_shadow_RGBA = #define MODE_SHADOW \n#define USE_RGBA_SHADOWS

                #[specializations]

                #[vertex]

                layout(location = 0) in vec3 vertex;

                uniform highp mat4 projection;
                uniform highp vec4 modelview1;
                uniform highp vec4 modelview2;
                uniform highp vec2 direction;
                uniform highp float z_far;

                #ifdef MODE_SHADOW
                out float depth;
                #endif

                void main() {
                    highp vec4 vtx = vec4(vertex, 1.0) * mat4(modelview1, modelview2, vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));

                #ifdef MODE_SHADOW
                    depth = dot(direction, vtx.xy);
                #endif
                    gl_Position = projection * vtx;
                }

                #[fragment]


                uniform highp mat4 projection;
                uniform highp vec4 modelview1;
                uniform highp vec4 modelview2;
                uniform highp vec2 direction;
                uniform highp float z_far;

                #ifdef MODE_SHADOW
                in highp float depth;
                #endif

                #ifdef USE_RGBA_SHADOWS
                layout(location = 0) out lowp vec4 out_buf;
                #else
                layout(location = 0) out highp float out_buf;
                #endif

                void main() {
                    float out_depth = 1.0;

                #ifdef MODE_SHADOW
                    out_depth = depth / z_far;
                #endif

                #ifdef USE_RGBA_SHADOWS
                    out_depth = clamp(out_depth, -1.0, 1.0);
                    out_depth = out_depth * 0.5 + 0.5;
                    highp vec4 comp = fract(out_depth * vec4(255.0 * 255.0 * 255.0, 255.0 * 255.0, 255.0, 1.0));
                    comp -= comp.xxyz * vec4(0.0, 1.0 / 255.0, 1.0 / 255.0, 1.0 / 255.0);
                    out_buf = comp;
                #else
                    out_buf = out_depth;
                #endif
                }

                """
            ),
            (
                Path.GetFullPath("Godot.Net/Drivers/GLES3/Shaders/vertex_fragment.glsl"),
                """
                #include "_included.glsl"

                #[modes]

                mode_ninepatch = #define USE_NINEPATCH

                #[specializations]

                DISABLE_LIGHTING = false

                #[vertex]

                precision highp float;
                precision highp int;

                layout(location = 0) in highp vec3 vertex;

                out highp vec4 position_interp;

                void main() {
                    position_interp = vec4(vertex.x,1,0,1);
                }

                #[fragment]

                precision highp float;
                precision highp int;

                in highp vec4 position_interp;

                void main() {
                    highp float depth = ((position_interp.z / position_interp.w) + 1.0);
                    frag_color = vec4(depth);
                }

                """
            ),
            // (
            //     Path.GetFullPath("Godot.Net/Servers/Rendering/RendererRD/Shaders/_included.glsl"),
            //     """
            //     #define M_PI 3.14159265359
            //     """
            // ),
            // (
            //     Path.GetFullPath("Godot.Net/Servers/Rendering/RendererRD/Shaders/compute.glsl"),
            //     """
            //     #[compute]

            //     #version 450

            //     #VERSION_DEFINES

            //     #define BLOCK_SIZE 8

            //     #include "_included.glsl"

            //     void main() {
            //         uint t = BLOCK_SIZE + 1;
            //     }

            //     """
            // ),
            // (
            //     Path.GetFullPath("Godot.Net/Servers/Rendering/RendererRD/Shaders/vertex_fragment.glsl"),
            //     """
            //     #[vertex]

            //     #version 450

            //     #VERSION_DEFINES

            //     #include "_included.glsl"

            //     layout(location = 0) out vec2 uv_interp;

            //     void main() {
            //         uv_interp = vec2(0, 1);
            //     }

            //     #[fragment]

            //     #version 450

            //     #VERSION_DEFINES

            //     layout(location = 0) in vec2 uv_interp;

            //     void main() {
            //         uv_interp = vec2(1, 0);
            //     }

            //     """
            // ),
        };

        public static (string, string)[] GeneratedSources { get; } = new[]
        {
            // (
            //     Path.GetFullPath("GLES/ComputeShader.g.cs"),
            //     """
            //     /* WARNING, THIS FILE WAS GENERATED, DO NOT EDIT */
            //     #ifndef COMPUTE_SHADER_GLSL_RAW_H
            //     #define COMPUTE_SHADER_GLSL_RAW_H

            //     static const char compute_shader_glsl[] = {
            //         35,91,99,111,109,112,117,116,101,93,10,10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,10,35,100,101,102,105,110,101,32,77,95,80,73,32,51,46,49,52,49,53,57,50,54,53,51,53,57,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,9,118,101,99,51,32,115,116,97,116,105,99,95,108,105,103,104,116,32,61,32,118,101,99,51,40,48,44,32,49,44,32,48,41,59,10,125,10,0
            //     };
            //     #endif

            //     """
            // ),
            // (
            //     Path.GetFullPath("GLES/VertexFragmentShader.g.cs"),
            //     """
            //     /* WARNING, THIS FILE WAS GENERATED, DO NOT EDIT */
            //     #ifndef VERTEX_FRAGMENT_SHADER_GLSL_RAW_H
            //     #define VERTEX_FRAGMENT_SHADER_GLSL_RAW_H

            //     static const char vertex_fragment_shader_glsl[] = {
            //         35,91,118,101,114,115,105,111,110,115,93,10,10,108,105,110,101,115,32,61,32,34,35,100,101,102,105,110,101,32,77,79,68,69,95,76,73,78,69,83,34,59,10,10,35,91,118,101,114,116,101,120,93,10,10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,108,97,121,111,117,116,40,108,111,99,97,116,105,111,110,32,61,32,48,41,32,111,117,116,32,118,101,99,51,32,117,118,95,105,110,116,101,114,112,59,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,10,35,105,102,100,101,102,32,77,79,68,69,95,76,73,78,69,83,10,9,117,118,95,105,110,116,101,114,112,32,61,32,118,101,99,51,40,48,44,48,44,49,41,59,10,35,101,110,100,105,102,10,125,10,10,35,91,102,114,97,103,109,101,110,116,93,10,10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,35,100,101,102,105,110,101,32,77,95,80,73,32,51,46,49,52,49,53,57,50,54,53,51,53,57,10,10,108,97,121,111,117,116,40,108,111,99,97,116,105,111,110,32,61,32,48,41,32,111,117,116,32,118,101,99,52,32,100,115,116,95,99,111,108,111,114,59,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,9,100,115,116,95,99,111,108,111,114,32,61,32,118,101,99,52,40,49,44,49,44,48,44,48,41,59,10,125,10,0
            //     };
            //     #endif

            //     """
            // ),
            (
                "CanvasOcclusionShaderGLES3.g.cs",
                """"
                // <auto-generated/>
                #if    !CANVAS_OCCLUSION_SHADER_GLES3_GLES
                #define CANVAS_OCCLUSION_SHADER_GLES3_GLES

                #nullable enable

                using Godot.Net.Core.Math;
                using Godot.Net.Drivers.GLES3;
                using System.Runtime.CompilerServices;
                using System;

                namespace Godot.Net.Drivers.GLES3.Shaders
                {
                    using RealT = System.Single;

                    public class CanvasOcclusionShaderGLES3 : ShaderGLES3
                    {
                        public enum Uniforms
                        {
                            PROJECTION,
                            MODELVIEW1,
                            MODELVIEW2,
                            DIRECTION,
                            Z_FAR,
                        }

                        public enum ShaderVariant
                        {
                            MODE_SDF,
                            MODE_SHADOW,
                            MODE_SHADOW_RGBA,
                        }

                        protected override void Init()
                        {
                            var uniformStrings = new string[]
                            {
                                "projection",
                                "modelview1",
                                "modelview2",
                                "direction",
                                "z_far",
                            };

                            var variantDefines = new string[]
                            {
                                "",
                                "#define MODE_SHADOW",
                                "#define MODE_SHADOW \n#define USE_RGBA_SHADOWS",
                            };

                            var texunitPairs = Array.Empty<TexUnitPair>();

                            var uboPairs = Array.Empty<UBOPair>();

                            var specPairs = Array.Empty<Specialization>();

                            var feedbacks = Array.Empty<Feedback>();

                            var vertexCode =
                                """
                                layout(location = 0) in vec3 vertex;
                                uniform highp mat4 projection;
                                uniform highp vec4 modelview1;
                                uniform highp vec4 modelview2;
                                uniform highp vec2 direction;
                                uniform highp float z_far;
                                #ifdef MODE_SHADOW
                                out float depth;
                                #endif
                                void main() {
                                    highp vec4 vtx = vec4(vertex, 1.0) * mat4(modelview1, modelview2, vec4(0.0, 0.0, 1.0, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
                                #ifdef MODE_SHADOW
                                    depth = dot(direction, vtx.xy);
                                #endif
                                    gl_Position = projection * vtx;
                                }
                                """;

                            var fragmentCode =
                                """
                                uniform highp mat4 projection;
                                uniform highp vec4 modelview1;
                                uniform highp vec4 modelview2;
                                uniform highp vec2 direction;
                                uniform highp float z_far;
                                #ifdef MODE_SHADOW
                                in highp float depth;
                                #endif
                                #ifdef USE_RGBA_SHADOWS
                                layout(location = 0) out lowp vec4 out_buf;
                                #else
                                layout(location = 0) out highp float out_buf;
                                #endif
                                void main() {
                                    float out_depth = 1.0;
                                #ifdef MODE_SHADOW
                                    out_depth = depth / z_far;
                                #endif
                                #ifdef USE_RGBA_SHADOWS
                                    out_depth = clamp(out_depth, -1.0, 1.0);
                                    out_depth = out_depth * 0.5 + 0.5;
                                    highp vec4 comp = fract(out_depth * vec4(255.0 * 255.0 * 255.0, 255.0 * 255.0, 255.0, 1.0));
                                    comp -= comp.xxyz * vec4(0.0, 1.0 / 255.0, 1.0 / 255.0, 1.0 / 255.0);
                                    out_buf = comp;
                                #else
                                    out_buf = out_depth;
                                #endif
                                }
                                """;

                            this.Setup(
                                vertexCode,
                                fragmentCode,
                                "CanvasOcclusionShaderGLES3",
                                uniformStrings,
                                uboPairs,
                                feedbacks,
                                texunitPairs,
                                specPairs,
                                variantDefines
                            );
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public bool VersionBindShader(Guid version, ShaderVariant variant, ulong specialization = 0) =>
                            base.VersionBindShader(version, (int)variant, specialization);

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public int VersionGetUniform(Uniforms uniform, Guid version, ShaderVariant variant, ulong specialization = 0) =>
                            base.VersionGetUniform((int)uniform, version, (int)variant, specialization);

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, byte value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1f(VersionGetUniform(uniform, version, variant, specialization), (float)value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, double value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1f(VersionGetUniform(uniform, version, variant, specialization), (float)value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, float value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1f(VersionGetUniform(uniform, version, variant, specialization), value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, int value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1i(VersionGetUniform(uniform, version, variant, specialization), value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, sbyte value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1i(VersionGetUniform(uniform, version, variant, specialization), (int)value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, short value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1i(VersionGetUniform(uniform, version, variant, specialization), (int)value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, uint value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1i(VersionGetUniform(uniform, version, variant, specialization), (int)value);
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public void VersionSetUniform(Uniforms uniform, ushort value, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform1ui(VersionGetUniform(uniform, version, variant, specialization), (uint)value);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Color color, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var col = new float[4] { color.R, color.G, color.B, color.A };
                            GL.Singleton.Uniform4fv(VersionGetUniform(uniform, version, variant, specialization), 1, col);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Vector2<RealT> vector, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var vec2 = new float[2] { vector.X, vector.Y };
                            GL.Singleton.Uniform2fv(VersionGetUniform(uniform, version, variant, specialization), 1, vec2);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Vector2<int> vector, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var vec2 = new int[2] { vector.X, vector.Y };
                            GL.Singleton.Uniform2iv(VersionGetUniform(uniform, version, variant, specialization), 1, vec2);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Vector3<RealT> vector, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var vec3 = new float[3] { vector.X, vector.Y, vector.Z };
                            GL.Singleton.Uniform3fv(VersionGetUniform(uniform, version, variant, specialization), 1, vec3);
                        }

                        public void VersionSetUniform(Uniforms uniform, float a, float b, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform2f(VersionGetUniform(uniform, version, variant, specialization), a, b);
                        }

                        public void VersionSetUniform(Uniforms uniform, float a, float b, float c, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform3f(VersionGetUniform(uniform, version, variant, specialization), a, b, c);
                        }

                        public void VersionSetUniform(Uniforms uniform, float a, float b, float c, float d, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            GL.Singleton.Uniform4f(VersionGetUniform(uniform, version, variant, specialization), a, b, c, d);
                        }

                        public void VersionSetUniform(Uniforms uniform, Transform3D<RealT> transform, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var matrix = new float[16]
                            {
                                /* build a 16x16 matrix */
                                transform.Basis[0, 0],
                                transform.Basis[1, 0],
                                transform.Basis[2, 0],
                                0,
                                transform.Basis[0, 1],
                                transform.Basis[1, 1],
                                transform.Basis[2, 1],
                                0,
                                transform.Basis[0, 2],
                                transform.Basis[1, 2],
                                transform.Basis[2, 2],
                                0f,
                                transform.Origin.X,
                                transform.Origin.Y,
                                transform.Origin.Z,
                                1f
                            };

                            GL.Singleton.UniformMatrix4fv(VersionGetUniform(uniform, version, variant, specialization), 1, false, matrix);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Transform2D<RealT> transform, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var matrix = new float[16]
                            {
                                /* build a 16x16 matrix */
                                transform[0, 0],
                                transform[0, 1],
                                0f,
                                0f,
                                transform[1, 0],
                                transform[1, 1],
                                0f,
                                0f,
                                0f,
                                0f,
                                1f,
                                0f,
                                transform[2, 0],
                                transform[2, 1],
                                0f,
                                1f
                            };

                            GL.Singleton.UniformMatrix4fv(this.VersionGetUniform(uniform, version, variant, specialization), 1, false, matrix);
                        }

                        public void VersionSetUniform(Uniforms uniform, in Projection<RealT> projection, Guid version, ShaderVariant variant, ulong specialization = 0)
                        {
                            if (this.VersionGetUniform(uniform, version, variant, specialization) < 0) return;
                            var matrix = new float[16];
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    matrix[i * 4 + j] = projection[i, j];
                                }

                            }

                            GL.Singleton.UniformMatrix4fv(VersionGetUniform(uniform, version, variant, specialization), 1, false, matrix);
                        }
                    }
                }

                #endif

                """"
            ),
            (
                "VertexFragmentShaderGLES3.g.cs",
                """"
                // <auto-generated/>
                #if    !VERTEX_FRAGMENT_SHADER_GLES3_GLES
                #define VERTEX_FRAGMENT_SHADER_GLES3_GLES

                #nullable enable

                using Godot.Net.Core.Math;
                using Godot.Net.Drivers.GLES3;
                using System.Runtime.CompilerServices;
                using System;

                namespace Godot.Net.Drivers.GLES3.Shaders
                {
                    using RealT = System.Single;

                    public class VertexFragmentShaderGLES3 : ShaderGLES3
                    {
                        public enum ShaderVariant
                        {
                            MODE_NINEPATCH,
                        }

                        public enum Specializations
                        {
                            DISABLE_LIGHTING = 1,
                        }

                        protected override void Init()
                        {
                            var uniformStrings = Array.Empty<string>();

                            var variantDefines = new string[]
                            {
                                "#define USE_NINEPATCH",
                            };

                            var texunitPairs = Array.Empty<TexUnitPair>();

                            var uboPairs = Array.Empty<UBOPair>();

                            var specPairs = new Specialization[]
                            {
                                new("DISABLE_LIGHTING", false),
                            };

                            var feedbacks = Array.Empty<Feedback>();

                            var vertexCode =
                                """
                                precision highp float;
                                precision highp int;
                                layout(location = 0) in highp vec3 vertex;
                                out highp vec4 position_interp;
                                void main() {
                                    position_interp = vec4(vertex.x,1,0,1);
                                }
                                """;

                            var fragmentCode =
                                """
                                precision highp float;
                                precision highp int;
                                in highp vec4 position_interp;
                                void main() {
                                    highp float depth = ((position_interp.z / position_interp.w) + 1.0);
                                    frag_color = vec4(depth);
                                }
                                """;

                            this.Setup(
                                vertexCode,
                                fragmentCode,
                                "VertexFragmentShaderGLES3",
                                uniformStrings,
                                uboPairs,
                                feedbacks,
                                texunitPairs,
                                specPairs,
                                variantDefines
                            );
                        }

                        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        public bool VersionBindShader(Guid version, ShaderVariant variant, ulong specialization = 0) =>
                            base.VersionBindShader(version, (int)variant, specialization);

                    }
                }

                #endif

                """"
            ),
            // (
            //     Path.GetFullPath("RDGLES/ComputeShaderRD.g.cs"),
            //     """
            //     /* WARNING, THIS FILE WAS GENERATED, DO NOT EDIT */
            //     #ifndef COMPUTE_GLSL_GEN_H_RD
            //     #define COMPUTE_GLSL_GEN_H_RD

            //     #include "servers/rendering/renderer_rd/shader_rd.h"

            //     class ComputeShaderRD : public ShaderRD {

            //     public:

            //         ComputeShaderRD() {

            //             static const char _compute_code[] = {
            //     10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,35,100,101,102,105,110,101,32,66,76,79,67,75,95,83,73,90,69,32,56,10,10,35,100,101,102,105,110,101,32,77,95,80,73,32,51,46,49,52,49,53,57,50,54,53,51,53,57,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,9,117,105,110,116,32,116,32,61,32,66,76,79,67,75,95,83,73,90,69,32,43,32,49,59,10,125,10,0
            //             };
            //             setup(nullptr, nullptr, _compute_code, "ComputeShaderRD");
            //         }
            //     };

            //     #endif


            //     """
            // ),
            // (
            //     Path.GetFullPath("RDGLES/VertexFragmentShaderRD.g.cs"),
            //     """
            //     /* WARNING, THIS FILE WAS GENERATED, DO NOT EDIT */
            //     #ifndef VERTEX_FRAGMENT_GLSL_GEN_H_RD
            //     #define VERTEX_FRAGMENT_GLSL_GEN_H_RD

            //     #include "servers/rendering/renderer_rd/shader_rd.h"

            //     class VertexFragmentShaderRD : public ShaderRD {

            //     public:

            //         VertexFragmentShaderRD() {

            //             static const char _vertex_code[] = {
            //     10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,35,100,101,102,105,110,101,32,77,95,80,73,32,51,46,49,52,49,53,57,50,54,53,51,53,57,10,10,108,97,121,111,117,116,40,108,111,99,97,116,105,111,110,32,61,32,48,41,32,111,117,116,32,118,101,99,50,32,117,118,95,105,110,116,101,114,112,59,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,9,117,118,95,105,110,116,101,114,112,32,61,32,118,101,99,50,40,48,44,32,49,41,59,10,125,10,10,0
            //             };
            //             static const char _fragment_code[] = {
            //     10,35,118,101,114,115,105,111,110,32,52,53,48,10,10,35,86,69,82,83,73,79,78,95,68,69,70,73,78,69,83,10,10,108,97,121,111,117,116,40,108,111,99,97,116,105,111,110,32,61,32,48,41,32,105,110,32,118,101,99,50,32,117,118,95,105,110,116,101,114,112,59,10,10,118,111,105,100,32,109,97,105,110,40,41,32,123,10,9,117,118,95,105,110,116,101,114,112,32,61,32,118,101,99,50,40,49,44,32,48,41,59,10,125,10,0
            //             };
            //             setup(_vertex_code, _fragment_code, nullptr, "VertexFragmentShaderRD");
            //         }
            //     };

            //     #endif


            //     """
            // ),
        };
    }
}
