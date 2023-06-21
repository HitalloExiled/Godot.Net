#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public record InstanceSort<T>(T Instance, float Depth)
    {
        public static bool operator <(InstanceSort<T> left, InstanceSort<T> right) => left.Depth < right.Depth;
        public static bool operator >(InstanceSort<T> left, InstanceSort<T> right) => left.Depth > right.Depth;
    }

}
