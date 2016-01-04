using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms
{
  /// <summary>
  /// Ants are implemented predominantly as per "Ant Colony Optimisation" Dorigo and Stutzle (2004), Ch3.8, p103.
  /// </summary>
  public class Ant : IComparable<Ant>
  {
    public double TourLength { get; private set; } = 0.0;
    public List<int> Tour { get; } = new List<int>();   // the indices of the nodes belonging to the current tour.

    private int _startNode;
    private int _currentNode;

    private readonly int[] _visited;     // the indices of the nodes the Ant has already visited.
    private readonly ProblemContext _problemContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemContext">Provides access to the problem-specific parameters and information matrices
    /// used in applying the random proportional rule.</param>
    /// <param name="nrNodes">The nr of nodes in the TSP graph.</param>
    public Ant(ProblemContext problemContext)
    {
      _problemContext = problemContext;
      _visited = new int[_problemContext.NodeCount];
    }

    /// <summary>
    /// Initialises the internal state of the Ant (discards constructed tour information if it exists).
    /// </summary>
    public void Initialise(int startNode)
    {
      _startNode = startNode;
      _currentNode = _startNode;

      for (var i = 0; i < _visited.Length; i++)
      {
        _visited[i] = 0;
      }
      _visited[_currentNode] = 1;

      TourLength = 0.0;
      Tour.Clear();
      Tour.Add(_currentNode);
    }

    /// <summary>
    /// Applies the random proportional rule on non-visited neighbours and moves the ant
    /// to the node selected by the <seealso cref="RouletteWheelSelector"/>.
    /// </summary>
    public void MoveNext(object sender, EventArgs args)
    {
      // Find the neighbours we haven't visited yet.
      var neighbours = _problemContext.DataStructures.NearestNeighbours(_currentNode);
      var notVisited = neighbours.Where(n => _visited[n] != 1).ToArray();

      // If we've visited all nodes, return to the starting node.
      var selectedNext = notVisited.Any() ?
                              _problemContext.RouletteWheelSelector.MakeSelection(notVisited, _currentNode) : _startNode;

      // Update tour information and move to the next node.
      TourLength += _problemContext.DataStructures.Distance(_currentNode, selectedNext);
      _currentNode = selectedNext;
      Tour.Add(_currentNode);
      _visited[_currentNode] = 1;
    }

    public int CompareTo(Ant other)
    {
      return (other != null) ? TourLength.CompareTo(other.TourLength) : 1;
    }
  }
}