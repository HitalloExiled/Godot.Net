namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

#pragma warning disable IDE0044 // TODO Remove

public record BlitToScreen
{
    public record LensDistortionRecord
    {
        //lens distorted parameters for VR
        public bool           Apply       { get; set; }
        public float          AspectRatio { get; set; } = 1.0f;
        public Vector2<RealT> EyeCenter   { get; set; }
        public float          K1          { get; set; }
        public float          K2          { get; set; }
        public float          Upscale     { get; set; } = 1.0f;
    }

    public record MultiViewRecord
    {
        public uint Layer    { get; set; }
        public bool UseLayer { get; set; }
    };

    public Rect2<int>           DstRect        { get; set; }
    public LensDistortionRecord LensDistortion { get; set; } = new();
    public MultiViewRecord      MultiView      { get; set; } = new();
    public Guid                 RenderTarget   { get; set; }
    public Rect2<RealT>         SrcRect        { get; set; } = new(0.0f, 0.0f, 1.0f, 1.0f);
};
