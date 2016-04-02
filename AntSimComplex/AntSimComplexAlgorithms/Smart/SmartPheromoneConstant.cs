using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal class SmartPheromoneConstant : ISmartPheromone
  {
    public double Density(int fromNode, int stepCount)
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