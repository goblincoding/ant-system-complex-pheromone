using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Ants
{
  internal class Ant : IAnt
  {
    public int Id { get; }
    public int CurrentNode { get; private set; }
    public double TourLength { get; private set; }
    public IReadOnlyList<int> Tour => _tour.Where(n => n != -1).ToArray();
    public IReadOnlyList<bool> Visited => _visited;

    private int _startNode;
    private readonly int[] _tour;
    private readonly bool[] _visited; // the indices of the nodes the Ant has already visited.
    private readonly IProblemData _problemData;
    private readonly INodeSelector _nodeSelector;

    /// <summary>
    /// Ants are implemented predominantly as per "Ant Colony Optimisation" Dorigo and Stutzle (2004), Ch3.8, p103.
    /// </summary>
    /// <param name="id">The ant's unique integer Id.</param>
    /// <param name="problemData">Provides access to the problem-specific parameters and data matrices.</param>
    /// <param name="nodeSelector">Used to select the next node to move to.</param>
    public Ant(int id, IProblemData problemData, INodeSelector nodeSelector)
    {
      Id = id;
      _problemData = problemData;
      _nodeSelector = nodeSelector;

      // Ant ultimately returns to starting node.
      _tour = new int[_problemData.NodeCount + 1];
      _visited = new bool[_problemData.NodeCount];
    }

    /// <summary>
    /// Initialises (or resets) the internal state of the Ant.
    /// </summary>
    /// <param name="startNode">The node the ant starts its tour on.</param>
    public void Initialise(int startNode)
    {
      _startNode = startNode;
      CurrentNode = _startNode;

      // Set all nodes to "not visited" except for current (start) node.
      for (var i = 0; i < _visited.Length; i++)
      {
        _visited[i] = false;
        _tour[i] = -1;
      }
      _visited[CurrentNode] = true;

      // This bit of acrobatics is to ensure we reset the "nodecount + 1"
      // node as well as all the others.
      _tour[_tour.Length - 1] = -1;

      TourLength = 0.0;
      _tour[0] = CurrentNode;
    }

    /// <summary>
    /// Move to the next node selected by the current node selection strategy.
    /// </summary>
    /// <param name="i"></param>
    public void Step(int i)
    {
      // Select the next node to visit ("start" if all nodes have been visited).
      var selectedNext = _visited.Any(n => !n) ? _nodeSelector.SelectNextNode(this) : _startNode;

      // Update tour information before we move to the next node.
      TourLength += _problemData.Distance(CurrentNode, selectedNext);

      CurrentNode = selectedNext;
      _tour[i] = CurrentNode;
      _visited[CurrentNode] = true;
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