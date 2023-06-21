namespace Godot.Net.SourceGenerators;

using System.Collections.Generic;

public class GLES3Ast
{
    public List<string>           Fbos                  { get; }      = new List<string>();
    public List<(string, string)> Feedbacks             { get; }      = new List<(string, string)>();
    public List<string>           FragmentIncludedFiles { get; }      = new List<string>();
    public List<string>           FragmentLines         { get; }      = new List<string>();
    public int                    FragmentOffset        { get; set; }
    public int                    LineOffset            { get; set; }
    public string                 Reading               { get; set; } = "";
    public List<string>           SpecializationNames   { get; }      = new List<string>();
    public List<string>           SpecializationValues  { get; }      = new List<string>();
    public List<string>           TexunitNames          { get; }      = new List<string>();
    public List<string>           Texunits              { get; }      = new List<string>();
    public List<string>           UboNames              { get; }      = new List<string>();
    public List<string>           Ubos                  { get; }      = new List<string>();
    public List<string>           Uniforms              { get; }      = new List<string>();
    public List<string>           VariantDefines        { get; }      = new List<string>();
    public List<string>           VariantNames          { get; }      = new List<string>();
    public List<string>           VertexIncludedFiles   { get; }      = new List<string>();
    public List<string>           VertexLines           { get; }      = new List<string>();
    public int                    VertexOffset          { get; set; }
}
