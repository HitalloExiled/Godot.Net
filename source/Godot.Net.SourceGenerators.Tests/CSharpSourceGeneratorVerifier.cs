namespace Godot.Net.SourceGenerators.Tests;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
    {
        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

        public Test()
        {
            this.TestBehaviors       = TestBehaviors.SkipGeneratedCodeCheck;
            this.ReferenceAssemblies = new ReferenceAssemblies("net7.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), Path.Combine("ref", "net7.0"));
        }

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            var args                 = new[] { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings     = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions
                .WithConcurrentBuild(false)
                .WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler())
            );
        }

        protected override ParseOptions CreateParseOptions() => ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(this.LanguageVersion);
    }
}
