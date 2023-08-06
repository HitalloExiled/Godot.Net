namespace Godot.Net.SourceGenerators.Tests;

using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

internal class AdditionalFile : AdditionalText
{
    private readonly string content;

    public override string Path { get; }

    public AdditionalFile(string path, string content)
    {
        this.Path    = path;
        this.content = content;
    }

    public override SourceText? GetText(CancellationToken cancellationToken = default) =>
        SourceText.From(this.content, Encoding.UTF8);
}
