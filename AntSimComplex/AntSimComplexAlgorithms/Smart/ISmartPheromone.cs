using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal interface ISmartPheromone
  {
    double PresentedDensity(int antId);

    double GraphDensity();

    void Deposit(double amount);

    void Evaporate(double evaporationRate);

    void Reset();

    void Touch(IAnt ant);
  }
}