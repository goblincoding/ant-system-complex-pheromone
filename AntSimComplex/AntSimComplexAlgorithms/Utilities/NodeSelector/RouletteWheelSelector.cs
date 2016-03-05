using AntSimComplexAlgorithms.Utilities.DataStructures;
using System;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  /// <summary>
  /// The probabilistic selection of the next node to be visited is done in accordance to the
  /// roulette wheel selection procedure: https://en.wikipedia.org/wiki/Fitness_proportionate_selection
  /// </summary>
  internal class RouletteWheelSelector : INodeSelector
  {
    private readonly Random _random;
    private readonly IProblemData _problemData;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemData">The problem specific <seealso cref="StandardProblemData"/> object containing distance,
    /// pheromone, heuristic and choice info information.</param>
    /// <param name="random">The global random number generator object.</param>
    public RouletteWheelSelector(IProblemData problemData, Random random)
    {
      _problemData = problemData;
      _random = random;
    }

    /// <summary>
    /// Selects the index of the next node based on the "roulette wheel selection" principle.
    /// </summary>
    /// <param name="ant"></param>
    /// <returns>The index of the next node to visit.</returns>
    public int SelectNextNode(IAnt ant)
    {
      var notVisited = ant.NotVisited;
      var probabilities = new double[notVisited.Count];
      var choice = _problemData.ChoiceInfo(ant);
      var sumProbabilities = 0.0;

      // LINQ is not viable in this case due to the closure.  Performance
      // is increased significantly by using a basic for loop here instead.
      // ReSharper disable once LoopCanBeConvertedToQuery
      for (var i = 0; i < notVisited.Count; i++)
      {
        var probability = choice[ant.CurrentNode][notVisited[i]];
        probabilities[i] = probability;
        sumProbabilities += probability;
      }

      var selectedProbability = _random.NextDouble() * (sumProbabilities);

      var j = 0;
      var probabilitySum = probabilities[j];

      while (probabilitySum < selectedProbability &&
             j < probabilities.Length - 1)
      {
        j++;
        probabilitySum += probabilities[j];
      }

      return notVisited[j];
    }
  }
}