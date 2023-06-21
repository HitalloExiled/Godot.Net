namespace Godot.Net.SourceGenerators;

using System.Text;

public class CodeWriter
{
    private readonly StringBuilder source = new();
    private int identation;

    public string Source => this.source.ToString();

    public void Deindent() => this.identation -= 4;
    public void Indent() => this.identation += 4;
    public void Write(string value) => this.source.Append(value);
    public void WriteIdent() => this.source.Append(new string(' ', this.identation));

    public void WriteIdentedLine(string value)
    {
        this.Indent();
        this.WriteLine(value);
        this.Deindent();
    }

    public void WriteLine(string value) => this.source.Append(new string(' ', this.identation) + value + '\n');
    public void NewLine(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            this.source.Append('\n');
        }
    }
}
