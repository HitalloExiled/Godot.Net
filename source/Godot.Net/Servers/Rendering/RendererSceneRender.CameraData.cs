namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

#pragma warning disable CA1822 // TODO - Remove

public partial class RendererSceneRender
{
    public record struct CameraData
    {
        public bool                 IsOrthogonal   { get; set; }
        public Projection<RealT>    MainProjection { get; set; }
        public Transform3D<RealT>   MainTransform  { get; set; }
        public Vector2<RealT>       TaaJitter      { get; set; }
        public bool                 VAspect        { get; set; }
        public uint                 ViewCount      { get; set; }
        public Transform3D<RealT>[] ViewOffset     { get; } = new Transform3D<RealT>[MAX_RENDER_VIEWS];
        public Projection<RealT>[]  ViewProjection { get; } = new Projection<RealT>[MAX_RENDER_VIEWS];
        public uint                 VisibleLayers  { get; set; }

        public CameraData()
        { }

        private static void ERR_FAIL_COND_MSG(string v) => throw new NotImplementedException();

        public void SetCamera(Transform3D<RealT> transform, Projection<RealT> projection, bool isOrthogonal, bool vaspect, Vector2<RealT> taaJitter, uint visibleLayers)
        {
            this.ViewCount         = 1;
            this.IsOrthogonal      = isOrthogonal;
            this.VAspect           = vaspect;
            this.MainTransform     = transform;
            this.MainProjection    = projection;
            this.VisibleLayers     = visibleLayers;
            this.ViewOffset[0]     = new();
            this.ViewProjection[0] = projection;
            this.TaaJitter         = taaJitter;
        }

        public void SetMultiviewCamera(uint viewCount, Transform3D<RealT>[] transforms, Projection<RealT>[] projections, bool isOrthogonal, bool vaspect)
        {
            if (viewCount != 2)
            {
                Console.WriteLine("Incorrect view count for stereoscopic view");

                return;
            }

            this.ViewCount    = viewCount;
            this.IsOrthogonal = isOrthogonal;
            this.VAspect      = vaspect;

            var planes = new Plane<RealT>[viewCount][];

            for (var i = 0u; i < viewCount; i++)
            {
                planes[i] = projections[i].GetProjectionPlanes(transforms[i]);
            }

            var n0 = planes[0][(int)Projection.PlanesTypes.PLANE_LEFT].Normal;
            var n1 = planes[1][(int)Projection.PlanesTypes.PLANE_RIGHT].Normal;
            var y  = n0.Cross(n1).Normalized;
            var z  = (n0 + n1).Normalized;
            var x  = y.Cross(z).Normalized;

            y = z.Cross(x).Normalized;

            this.MainTransform = this.MainTransform with
            {
                Basis = new(x, y, z),
            };

            var horizon = new Plane<RealT>(y, transforms[0].Origin);
            var intersection = horizon.Intersect(
                planes[0][(int)Projection.PlanesTypes.PLANE_LEFT],
                planes[1][(int)Projection.PlanesTypes.PLANE_RIGHT]
            );

            if (!intersection.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine camera origin");

                return;
            }

            this.MainTransform = this.MainTransform with { Origin = intersection.Value };

            var mainTransformInv = this.MainTransform.Inverse();
            var farCenter = planes[0][(int)Projection.PlanesTypes.PLANE_FAR].Center * (RealT)0.5;
            var far = new Plane<RealT>(-z, farCenter);

            var topLeft = far.Intersect(planes[0][(int)Projection.PlanesTypes.PLANE_LEFT], planes[0][(int)Projection.PlanesTypes.PLANE_TOP]);

            if (!topLeft.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera far/left/top vector");

                return;
            }

            var other = far.Intersect(planes[1][(int)Projection.PlanesTypes.PLANE_LEFT], planes[1][(int)Projection.PlanesTypes.PLANE_TOP]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera far/left/top vector");

                return;
            }

            if (y.Dot(topLeft.Value) < y.Dot(other.Value))
            {
                topLeft = other;
            }

            var bottomLeft = far.Intersect(planes[0][(int)Projection.PlanesTypes.PLANE_LEFT], planes[0][(int)Projection.PlanesTypes.PLANE_BOTTOM]);

            if (!bottomLeft.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera far/left/bottom vector");

                return;
            }

            other = far.Intersect(planes[1][(int)Projection.PlanesTypes.PLANE_LEFT], planes[1][(int)Projection.PlanesTypes.PLANE_BOTTOM]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera far/left/bottom vector");

                return;
            }

            if (y.Dot(other.Value) < y.Dot(bottomLeft.Value))
            {
                bottomLeft = other;
            }

            var topRight = far.Intersect(planes[0][(int)Projection.PlanesTypes.PLANE_RIGHT], planes[0][(int)Projection.PlanesTypes.PLANE_TOP]);

            if (!topRight.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera far/right/top vector");

                return;
            }

            other = far.Intersect(planes[1][(int)Projection.PlanesTypes.PLANE_RIGHT], planes[1][(int)Projection.PlanesTypes.PLANE_TOP]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera far/right/top vector");

                return;
            }

            if (y.Dot(topRight.Value) < y.Dot(other.Value))
            {
                topRight = other;
            }


            var bottomRight = far.Intersect(planes[0][(int)Projection.PlanesTypes.PLANE_RIGHT], planes[0][(int)Projection.PlanesTypes.PLANE_BOTTOM]);

            if (!bottomRight.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera far/right/bottom vector");

                return;
            }

            other = far.Intersect(planes[1][(int)Projection.PlanesTypes.PLANE_RIGHT], planes[1][(int)Projection.PlanesTypes.PLANE_BOTTOM]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera far/right/bottom vector");

                return;
            }

            if (y.Dot(other.Value) < y.Dot(bottomRight.Value))
            {
                bottomRight = other;
            }

            var top = new Plane<RealT>(this.MainTransform.Origin, topLeft.Value, topRight.Value);
            var bottom = new Plane<RealT>(this.MainTransform.Origin, bottomLeft.Value, bottomRight.Value);
            var negZ = -z;

            var near = (negZ.Dot(transforms[1].Origin) < negZ.Dot(transforms[0].Origin))
                ? new Plane<RealT>(negZ, transforms[0].Origin)
                : new Plane<RealT>(negZ, transforms[1].Origin);

            var minVec = near.Intersect(bottom, planes[0][(int)Projection.PlanesTypes.PLANE_LEFT]);

            if (!minVec.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera near/left/bottom vector");

                return;
            }

            other = near.Intersect(bottom, planes[1][(int)Projection.PlanesTypes.PLANE_LEFT]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera near/left/bottom vector");

                return;
            }

            if (x.Dot(other.Value) < x.Dot(minVec.Value))
            {
                minVec = other;
            }

            var maxVec = near.Intersect(top, planes[0][(int)Projection.PlanesTypes.PLANE_RIGHT]);

            if (!maxVec.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine left camera near/right/top vector");

                return;
            }

            other = near.Intersect(top, planes[1][(int)Projection.PlanesTypes.PLANE_RIGHT]);

            if (!other.HasValue)
            {
                ERR_FAIL_COND_MSG("Can't determine right camera near/right/top vector");

                return;
            }

            if (x.Dot(maxVec.Value) < x.Dot(other.Value))
            {
                maxVec = other;
            }

            var localMinVec = mainTransformInv.XForm(minVec.Value);
            var localMaxVec = mainTransformInv.XForm(maxVec.Value);

            var zNear = -near.DistanceTo(this.MainTransform.Origin);
            var zFar = -far.DistanceTo(this.MainTransform.Origin);

            this.MainProjection.SetFrustum(
                localMinVec.X,
                localMaxVec.X,
                localMinVec.Y,
                localMaxVec.Y,
                zNear,
                zFar
            );

            for (var v = 0u; v < viewCount; v++)
            {
                this.ViewOffset[v] = mainTransformInv * transforms[v];
                this.ViewProjection[v] = projections[v] * new Projection<RealT>(this.ViewOffset[v].Inverse());
            }
        }
    }
}
