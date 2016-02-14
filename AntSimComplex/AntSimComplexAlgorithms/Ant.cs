using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms
{
  /// <summary>
  /// Ants are implemented predominantly as per "Ant Colony Optimisation" Dorigo and Stutzle (2004), Ch3.8, p103.
  /// </summary>
  internal class Ant : IComparable<Ant>
  {
    /// <summary>
    /// Length of the ant's completed tour.
    /// </summary>
    public double TourLength { get; private set; }

    /// <summary>
    /// The node indices corresponding to the ant's tour.
    /// </summary>
    public List<int> Tour { get; } = new List<int>();

    private int _startNode;
    private int _currentNode;

    private readonly bool[] _visited; // the indices of the nodes the Ant has already visited.
    private readonly IProblemData _problemData;
    private readonly INodeSelector _rouletteWheel;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="problemData">Provides access to the problem-specific parameters and information matrices
    /// used in applying the random proportional rule.</param>
    /// <param name="rouletteWheel">Used to select the next node based on the probabilities of selection.</param>
    public Ant(IProblemData problemData, INodeSelector rouletteWheel)
    {
      _problemData = problemData;
      _rouletteWheel = rouletteWheel;
      _visited = new bool[_problemData.NodeCount];
    }

    /// <summary>
    /// Initialises the internal state of the Ant (discards constructed tour information if it exists).
    /// </summary>
    /// <param name="startNode">The node the ant starts its tour on.</param>
    public void Initialise(int startNode)
    {
      _startNode = startNode;
      _currentNode = _startNode;

      // Set all nodes to "not visited" except for current (start) node.
      for (var i = 0; i < _visited.Length; i++)
      {
        _visited[i] = false;
      }
      _visited[_currentNode] = true;

      TourLength = 0.0;
      Tour.Clear();
      Tour.Add(_currentNode);
    }

    /// <summary>
    /// Applies the random proportional rule on non-visited neighbours and moves the ant
    /// to the node selected by the <seealso cref="RouletteWheelSelector"/>.
    /// </summary>
    public void MoveNext()
    {
      // Find the neighbours we haven't visited yet.
      var neighbours = _problemData.NearestNeighbours(_currentNode);
      var notVisited = neighbours.Where(n => !_visited[n]).ToArray();

      // Select the next node to visit ("start" if all nodes have been visited).
      var selectedNext = notVisited.Any() ?
                              _rouletteWheel.SelectNextNode(notVisited, _currentNode) : _startNode;

      // Update tour information and move to the next selected node.
      TourLength += _problemData.Distance(_currentNode, selectedNext);
      _currentNode = selectedNext;
      Tour.Add(_currentNode);
      _visited[_currentNode] = true;
    }

    /// <summary>
    /// Ants are compared on TourLength.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Ant other)
    {
      return other != null ? TourLength.CompareTo(other.TourLength) : 1;
    }
  }
}