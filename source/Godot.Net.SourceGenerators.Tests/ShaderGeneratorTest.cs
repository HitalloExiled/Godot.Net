namespace Godot.Net.SourceGenerators.Tests;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public partial class ShaderGeneratorTest
{
    private static readonly string godotProject = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../Godot.Net/bin/Debug/net8.0/Godot.Net.dll"));

    private static async Task<CSharpCompilation> CreateCompilationAsync()
    {
        var referenceAssemblies = new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0-preview.6.23329.7"), Path.Combine("ref", "net8.0"));

        var references = await referenceAssemblies.ResolveAsync(null, default);

        return CSharpCompilation.Create(
            "compilation",
            null,
            references.Append(MetadataReference.CreateFromFile(godotProject)),
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
        );
    }

    [Fact]
    public async Task GenerateAsync()
    {
        var compilation = await CreateCompilationAsync();

        var generator = new ShaderGenerator();

        var additionalFiles = Fixtures.AditionalFiles
            .Select(x => new AdditionalFile(x.Path, x.Content))
            .Cast<AdditionalText>();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { generator }, additionalFiles);

        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var outputDiagnostics = outputCompilation.GetDiagnostics();

        Assert.Empty(diagnostics);
        // Assert.Empty(outputDiagnostics);
        Assert.Equal(Fixtures.Scenarios.Count, outputCompilation.SyntaxTrees.Count());

        var verifier = new XUnitVerifier();

        var matches = outputCompilation.SyntaxTrees
            .Join(
                Fixtures.Scenarios,
                x => Path.GetFileName(x.FilePath),
                x => x.GeneratedPath,
                (actual, expected) => (actual.GetText().ToString(), expected.GeneratedContent)
            ).ToArray();

        Assert.Equal(Fixtures.Scenarios.Count, matches.Length);

        foreach (var (actual, expected) in matches)
        {
            verifier.EqualOrDiff(actual, expected);
        }
    }
}
