namespace Godot.Net.Servers.Rendering;

using System.Runtime.CompilerServices;
using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public record Polygon
    {
        public ulong        PolygonId { get; set; }
        public Rect2<RealT> RectCache { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Create(
            IList<int>             indices,
            IList<Vector2<RealT>>  points,
            IList<Color>           colors,
            IList<Vector2<RealT>>? uvs     = default,
            IList<int>?            bones   = default,
            IList<float>?          weights = default
        )
        {
            if (ERR_FAIL_COND(this.PolygonId != 0))
            {
                return;
            }

            var rectCache = this.RectCache with { Position = points[0] };

            for (var i = 1; i < points.Count; i++)
            {
                rectCache.ExpandTo(points[i]);
            }

            this.RectCache = rectCache;

            this.PolygonId = Singleton.RequestPolygon(indices, points, colors, uvs, bones, weights);
        }

        ~Polygon()
        {
            if (this.PolygonId > 0)
            {
                Singleton.FreePolygon(this.PolygonId);
            }
        }
    }
}
