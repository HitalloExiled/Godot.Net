namespace Godot.Net.SourceGenerators.Tests;

using System.Text.RegularExpressions;

public partial class ShaderGeneratorTest
{
    public record Scenario(string SourcePath, string SourceContent, string GeneratedPath, string GeneratedContent);

    private partial class Fixtures
    {
        public static IList<(string Path, string Content)> AditionalFiles { get; }

        public static IList<Scenario>                      Scenarios      { get; }

        static Fixtures()
        {
            var fixturesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../Fixtures/Shaders/GLES3"));

            var files = Directory.GetFiles(fixturesPath);

            const string VIRTUAL_PATH = "Godot.Net/Drivers/GLES3/Shaders";

            var additionalFiles = new List<(string, string)>
            {
                (
                    Path.GetFullPath($"{VIRTUAL_PATH}/.generate"),
                    File.ReadAllText(Path.Combine(fixturesPath, ".generate"))
                )
            };

            var scenarios = new List<Scenario>();

            foreach (var file in files.Where(x => x.EndsWith(".glsl")))
            {
                var name           = Path.GetFileNameWithoutExtension(file);
                var pascalCaseName = SnakeCaseToPascalCase(name) + "ShaderGLES3";
                var generatedPath  = Path.Combine(fixturesPath, $"{pascalCaseName}.g.cs");

                var sourcePath    = Path.GetFullPath($"{VIRTUAL_PATH}/{name}.glsl");
                var sourceContent = File.ReadAllText(file);

                if (Path.Exists(generatedPath))
                {
                    var scenario = new Scenario(
                        sourcePath,
                        sourceContent,
                        pascalCaseName + ".g.cs",
                        File.ReadAllText(generatedPath)
                    );

                    scenarios.Add(scenario);
                }

                additionalFiles.Add((sourcePath, sourceContent));
            }

            AditionalFiles = additionalFiles;
            Scenarios      = scenarios;
        }

        [GeneratedRegex("^([a-z])|_([a-z])")]
        private static partial Regex SnakePattern();

        private static string SnakeCaseToPascalCase(string value) =>
            SnakePattern()
                .Replace(
                    value,
                    match => (!string.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[1].Value : match.Groups[2].Value).ToUpper()
                );
    }
}
