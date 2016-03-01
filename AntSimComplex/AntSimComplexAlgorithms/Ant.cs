﻿using AntSimComplexAlgorithms.Utilities.DataStructures;
using AntSimComplexAlgorithms.Utilities.NodeSelector;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms
{
  internal class Ant : IAnt
  {
    /// <summary>
    /// The ant's unique integer Id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The index of the node the ant is currently on.
    /// </summary>
    public int CurrentNode;

    /// <summary>
    /// Length of the ant's completed tour.
    /// </summary>
    public double TourLength { get; private set; }

    /// <summary>
    /// The node indices corresponding to the ant's tour.
    /// </summary>
    public List<int> Tour { get; } = new List<int>();

    private int _startNode;
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
      }
      _visited[CurrentNode] = true;

      TourLength = 0.0;
      Tour.Clear();
      Tour.Add(CurrentNode);
    }

    /// <summary>
    /// Move to the next node selected by the current node selection strategy.
    /// </summary>
    public void Step()
    {
      var selectedNext = SelectedNextNode();

      // Update tour information before we move to the next node.
      TourLength += _problemData.Distance(CurrentNode, selectedNext);

      MoveNext(selectedNext);
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

    private int SelectedNextNode()
    {
      // Find the neighbours we haven't visited yet.
      var neighbours = _problemData.NearestNeighbours(CurrentNode);
      var notVisited = neighbours.Where(n => !_visited[n]).ToArray();

      // Select the next node to visit ("start" if all nodes have been visited).
      var nextNode = notVisited.Any() ? _nodeSelector.SelectNextNode(notVisited, CurrentNode) : _startNode;
      return nextNode;
    }

    private void MoveNext(int selectedNext)
    {
      CurrentNode = selectedNext;
      Tour.Add(CurrentNode);
      _visited[CurrentNode] = true;
    }
  }
}