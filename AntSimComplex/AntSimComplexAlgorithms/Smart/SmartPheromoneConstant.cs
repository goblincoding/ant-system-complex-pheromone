using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromoneConstant : ISmartPheromone
  {
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

    public void Touch(IAnt ant)
    {
    }
  }
}