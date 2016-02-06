namespace AntSimComplexTspLibItemManager.Utilities
{
  public class TspNode
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
  }
}