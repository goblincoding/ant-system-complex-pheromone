using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal interface ISmartPheromone
  {
    int Node1 { get; }
    int Node2 { get; }

    void UpdatePresentedDensity(IAnt ant);

    void UpdatePheromoneGraphSnapshot();

    double PresentedDensity(int antId);

    double GraphDensity();

    void Deposit(double amount);

    void Evaporate(double evaporationRate);

    void Reset();
  }
}