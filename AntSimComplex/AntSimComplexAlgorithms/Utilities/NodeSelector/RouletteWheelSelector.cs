using AntSimComplexAlgorithms.Utilities.DataStructures;
using System;
using System.Linq;

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
    private readonly double[] _probabilities;

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
      _probabilities = new double[problemData.NodeCount];
    }

    /// <summary>
    /// Selects the index of the next node based on the "roulette wheel selection" principle.
    /// </summary>
    /// <param name="ant"></param>
    /// <returns>The index of the next node to visit.</returns>
    public int SelectNextNode(IAnt ant)
    {
      var choice = _problemData.ChoiceInfo(ant);
      var sumProbabilities = 0.0;

      // LINQ is not viable in this case due to the closure.  Performance
      // is increased significantly by using a basic for loop here instead.
      // ReSharper disable once LoopCanBeConvertedToQuery
      for (var i = 0; i < _problemData.NodeCount; i++)
      {
        var probability = ant.Visited[i] ? 0.0 : choice[ant.CurrentNode][i];
        _probabilities[i] = probability;
        sumProbabilities += probability;
      }

      var selectedProbability = _random.NextDouble() * sumProbabilities;
      var j = 0;
      var probabilitySum = _probabilities[j];

      // NextDouble returns a value in [0.0, 1.0), deal with the edge
      // case where it is zero or all probabilities of selection are zero;
      if (selectedProbability.Equals(0.0))
      {
        return Enumerable.Range(0, _probabilities.Length).First(i => !ant.Visited[i]);
      }

      while (probabilitySum < selectedProbability &&
             j < _probabilities.Length - 1)
      {
        j++;
        probabilitySum += _probabilities[j];
      }

      return j;
    }
  }
}