namespace Godot.Net.Core.IO;

using System;
using Godot.Net.Core.Error;

public class ConfigFile
{
    private readonly Dictionary<string, Dictionary<string, object>> values = new();

    public object? GetValue(string section, string key, object? @default = default) => throw new NotImplementedException();
    public T? GetValue<T>(string section, string key, T? @default = default) => throw new NotImplementedException();
    public void GetSectionKeys(string section, out List<string> keys) => throw new NotImplementedException();

    public bool HasSection(string section) =>
        this.values.ContainsKey(section);

    public bool HasSectionKey(string section, string key) => throw new NotImplementedException();
    public Error Load(string path) => throw new NotImplementedException();
    public void SetValue<T>(string section, string key, T? value) => throw new NotImplementedException();
}
