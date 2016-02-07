using System;

namespace AntSimComplexTspLibItemManager.Utilities
{
  public sealed class TspNode : IEquatable<TspNode>
  {
    public int Id { get; }
    public double X { get; }
    public double Y { get; }

    public TspNode(int id, double x, double y)
    {
      Id = id;
      X = x;
      Y = y;
    }

    public bool Equals(TspNode other)
    {
      return Id == other.Id &&
             X.Equals(other.X) &&
             Y.Equals(other.Y);
    }
  }
}