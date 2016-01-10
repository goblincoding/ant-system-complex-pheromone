using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.RouletteWheelSelector;
using System;

namespace AntSimComplexAlgorithms.ProblemContext
{
  internal interface IProblemContext : IDataStructures, IRouletteWheelSelector
  {
    /// <summary>
    /// The global AntSystem Random object instance.
    /// </summary>
    Random Random { get; }

    /// <summary>
    /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
    /// nearest neighbour and pheromone density matrices.
    /// </summary>
    int NodeCount { get; }
  }
}