namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Config;
using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Rendering.Storage;

public abstract class ShaderData
{
    public Dictionary<string, Dictionary<int, Guid>>             DefaultTextureParams { get; } = new();
    public string?                                               Path                 { get; set; }
    public Dictionary<string, ShaderLanguage.ShaderNode.Uniform> Uniforms             { get; } = new();

    public virtual RS.ShaderNativeSourceCode NativeSourceCode => new();

    public virtual object? GetDefaultParameter(string parameter) => throw new NotImplementedException();
    public virtual void GetInstanceParamList(List<RendererMaterialStorage.InstanceShaderParam> paramList) => throw new NotImplementedException();
    public virtual void GetShaderUniformList(List<PropertyInfo> paramList) => throw new NotImplementedException();
    public virtual bool IsParameterTexture(string param) => throw new NotImplementedException();
    public virtual void SetDefaultTextureParameter(string name, Guid texture, int index) => throw new NotImplementedException();
    public virtual void SetPathHint(string hint) => throw new NotImplementedException();

    public abstract bool CastsShadows();
    public abstract bool IsAnimated();
    public abstract void SetCode(string code);
}
