namespace Godot.Net.Servers.Rendering.Storage;

using Godot.Net.Core.Config;

// Filename and type are different
public abstract partial class RendererMaterialStorage
{
    public abstract void GlobalShaderParameterAdd(string name, RS.GlobalShaderParameterType type, object value);
    public abstract void GlobalShaderParameterRemove(string name);
    public abstract List<string> GlobalShaderParameterGetList();
    public abstract void GlobalShaderParameterSet(string name, object value);
    public abstract void GlobalShaderParameterSetOverGuide(string name, object value);
    public abstract object GlobalShaderParameterGet(string name);
    public abstract RS.GlobalShaderParameterType GlobalShaderParameterGetType(string name);
    public abstract void GlobalShaderParametersLoadSettings(bool loadTextures = true);
    public abstract void GlobalShaderParametersClear();
    public abstract int GlobalShaderParametersInstanceAllocate(Guid instance);
    public abstract void GlobalShaderParametersInstanceFree(Guid instance);
    public abstract void GlobalShaderParametersInstanceUpdate(Guid instance, int index, object value, int flagsCount = 0);
    public abstract void ShaderInitialize(Guid id);
    public abstract void ShaderFree(Guid id);
    public abstract void ShaderSetCode(Guid shaderId, string code);
    public abstract void ShaderSetPathHint(Guid shaderId, string path);
    public abstract string ShaderGetCode(Guid shaderId);
    public abstract void GetShaderParameterList(Guid shaderId, List<PropertyInfo> paramList);
    public abstract void ShaderSetDefaultTextureParameter(Guid shaderId, string name, Guid texture, int index);
    public abstract Guid ShaderGetDefaultTextureParameter(Guid shaderId, string name, int index);
    public abstract object ShaderGetParameterDefault(Guid material, string param);
    public abstract RS.ShaderNativeSourceCode ShaderGetNativeSourceCode(Guid shaderId);
    public abstract void MaterialInitialize(Guid id);
    public abstract void MaterialFree(Guid id);
    public abstract void MaterialSetRenderPriority(Guid material, int priority);
    public abstract void MaterialSetShader(Guid materialId, Guid shaderId);
    public abstract void MaterialSetParam(Guid material, string param, object value);
    public abstract object? MaterialGetParam(Guid material, string param);
    public abstract void MaterialSetNextPass(Guid material, Guid nextMaterial);
    public abstract bool MaterialIsAnimated(Guid material);
    public abstract bool MaterialCastsShadows(Guid material);
    public abstract void MaterialGetInstanceShaderParameters(Guid material, List<InstanceShaderParam> parameters);
    public abstract void MaterialUpdateDependency(Guid material, DependencyTracker instance);
}
