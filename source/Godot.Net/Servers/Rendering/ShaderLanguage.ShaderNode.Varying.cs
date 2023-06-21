namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public partial record Varying
        {

            public uint              ArraySize     { get; set; }
            public DataInterpolation Interpolation { get; set; } = DataInterpolation.INTERPOLATION_FLAT;
            public DataPrecision     Precision     { get; set; } = DataPrecision.PRECISION_DEFAULT;
            public StageKind         Stage         { get; set; } = StageKind.STAGE_UNKNOWN;
            public TkPos             Tkpos         { get; set; }
            public DataType          Type          { get; set; } = DataType.TYPE_VOID;
        }
    }
}
