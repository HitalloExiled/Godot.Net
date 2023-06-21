namespace Godot.Net.Core.Math.Interfaces;

using System.Numerics;

public interface ISize2<T> where T : INumber<T>
{
    T Height { get; set; }
    T Width  { get; set; }
}
