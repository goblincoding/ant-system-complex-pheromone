using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromoneConstant : ISmartPheromone
  {
    public int Node1 { get; }
    public int Node2 { get; }

    public SmartPheromoneConstant(int node1, int node2)
    {
      Node1 = node1;
      Node2 = node2;
    }

    public double PresentedDensity(int antId)
    {
      return double.MaxValue;
    }

    public double GraphDensity()
    {
      return double.MaxValue;
    }

    public void Deposit(double amount)
    {
    }

    public void Evaporate(double evaporationRate)
    {
    }

    public void Reset()
    {
    }

    public void UpdatePresentedDensity(IAnt ant)
    {
    }

    public void UpdatePheromoneGraphSnapshot()
    {
    }
  }
}