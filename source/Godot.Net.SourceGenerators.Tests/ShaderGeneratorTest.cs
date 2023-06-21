namespace Godot.Net.SourceGenerators.Tests;

using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Verifier = CSharpSourceGeneratorVerifier<ShaderGenerator>;

public partial class ShaderGeneratorTest
{
    // FIXME - Broken due randomly GeneratedSources order and unsupported c# 11
    [Fact]
    public async Task GenerateAsync()
    {
        var test = new Verifier.Test();

        var godotProject = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../Godot.Net/bin/Debug/net7.0/Godot.Net.dll"));

        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(godotProject));

        foreach (var (filename, content) in Fixtures.AditionalFiles)
        {
            test.TestState.AdditionalFiles.Add((typeof(ShaderGenerator), filename, SourceText.From(content, Encoding.UTF8)));
        }

        foreach (var (filename, content) in Fixtures.GeneratedSources)
        {
            test.TestState.GeneratedSources.Add((typeof(ShaderGenerator), filename, SourceText.From(content, Encoding.UTF8)));
        }

        await test.RunAsync();
    }
}
