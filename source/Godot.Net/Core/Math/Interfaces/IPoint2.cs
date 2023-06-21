namespace Godot.Net.Core.Math.Interfaces;

using System.Numerics;

public interface IPoint2<T> where T : INumber<T>
{
    T X { get; set; }
    T Y { get; set; }
}
