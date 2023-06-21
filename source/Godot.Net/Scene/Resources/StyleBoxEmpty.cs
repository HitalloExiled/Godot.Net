namespace Godot.Net.Scene.Resources;

using System;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;

public class StyleBoxEmpty : StyleBox
{
    protected override float GetStyleMargin(Side side) => 0;
    public override void Draw(Guid canvasItemId, in Rect2<float> rect)  { /* NoOp */ }
}
