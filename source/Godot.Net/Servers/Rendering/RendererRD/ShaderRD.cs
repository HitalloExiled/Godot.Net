namespace Godot.Net.Servers.Rendering.RendererRD;

using System.Text;
using Godot.Net.Extensions;

#pragma warning disable CS0067,IDE0044,CS0649,IDE0052,CS0414 // TODO - REMOVE

public class ShaderRD
{
    public static string? ShaderCacheDir            { get; set; }
    public static bool    ShaderCacheCleanupOnStart { get; set; }

    private readonly List<string> variantDefines  = new();
    private readonly List<bool>   variantsEnabled = new();

    private string? generalDefines;
    private string? baseSha256;
    private string? name;
    private bool    shaderCacheDirValid;

    public void Initialize(List<string> variantDefines, in string? generalDefines = default)
    {
        if (ERR_FAIL_COND(this.variantDefines.Count == 0))
        {
            return;
        }

        if (ERR_FAIL_COND(variantDefines.Count == 0))
        {
            return;
        }

        this.generalDefines = generalDefines;

        for (var i = 0; i < variantDefines.Count; i++)
        {
            this.variantDefines.Add(variantDefines[i]);
            this.variantsEnabled.Add(true);
        }

        if (!string.IsNullOrEmpty(ShaderCacheDir))
        {
            var hashBuild = new StringBuilder();

            hashBuild.Append("[base_hash]");
            hashBuild.Append(this.baseSha256);
            hashBuild.Append("[general_defines]");
            hashBuild.Append(this.generalDefines);

            for (var i = 0; i < variantDefines.Count; i++)
            {
                hashBuild.Append("[variant_defines:" + i + "]");
                hashBuild.Append(variantDefines[i]);
            }

            this.baseSha256 = hashBuild.ToString().ToSHA256();

            if (ERR_FAIL_COND(!Directory.Exists(ShaderCacheDir)))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(Path.Join(ShaderCacheDir, this.name));
            }
            catch (Exception)
            {
                return;
            }

            //erase other versions?
            if (ShaderCacheCleanupOnStart)
            { }
            //
            if (!Directory.Exists(ShaderCacheDir))
            {
                try
                {
                    Directory.CreateDirectory(Path.Join(ShaderCacheDir, this.baseSha256));
                }
                catch (Exception)
                {
                    return;
                }
            }

            this.shaderCacheDirValid = true;

            PrintVerbose("Shader '" + this.name + "' SHA256: " + this.baseSha256);
        }
    }

    public Guid VersionCreate() => throw new NotImplementedException();
    public Guid VersionGetShader(Guid shaderVersion, int i) => throw new NotImplementedException();
}
