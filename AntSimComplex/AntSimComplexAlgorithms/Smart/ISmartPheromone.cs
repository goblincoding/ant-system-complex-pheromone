using AntSimComplexAlgorithms.Ants;

namespace AntSimComplexAlgorithms.Smart
{
  internal interface ISmartPheromone
  {
    double Density(int fromNode);
    void Deposit(double amount);
    void Evaporate(double evaporationRate);
    void Reset();
    void Touch(IAnt ant);
  }
}