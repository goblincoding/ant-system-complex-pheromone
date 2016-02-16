using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTspLibItemManager.Utilities
{
  /// <summary>
  /// This class is a wrapper for TspLib95Item objects representing symmetric TSP problems
  /// that fit the research criteria of a hundred nodes or less and with 2D node coordinates.
  /// </summary>
  internal class SymmetricTspItemInfoProvider
  {
    /// <returns>The number of nodes in the TSP graph.</returns>
    public int NodeCount { get; }

    /// <returns>The name of the current TSP problem.</returns>
    public string ProblemName { get; }

    /// <returns>The maximum "x" coordinate of all the nodes in the graph</returns>
    public double MaxXCoordinate { get; private set; }

    /// <returns>The minimum "x" coordinate of all the nodes in the graph</returns>
    public double MinXCoordinate { get; private set; }

    /// <returns>The maximum "y" coordinate of all the nodes in the graph</returns>
    public double MaxYCoordinate { get; private set; }

    /// <returns>The minimum "y" coordinate of all the nodes in the graph</returns>
    public double MinYCoordinate { get; private set; }

    /// <returns>Returns a distance matrix of edge weights between nodes.  I.e. Distances[i][j]
    /// will return the distance between node i and j.</returns>
    public IReadOnlyList<IReadOnlyList<double>> Distances => _distances;

    private double[][] _distances;

    /// <returns>A list of the current TspNodes ordered by ID.</returns>
    public IReadOnlyList<TspNode> TspNodes => _tspNodes;

    private readonly List<TspNode> _tspNodes;

    /// <returns>Returns the tour length constructed by the nearest neighbour heuristic (ACO Dorigo Ch3, p70).</returns>
    public double NearestNeighbourTourLength { get; private set; }

    /// <returns>Returns true if the TSP instance has a known optimal tour.</returns>
    public bool HasOptimalTour { get; private set; }

    /// <returns>The optimal tour length if known, double.MaxValue if not.</returns>
    public double OptimalTourLength { get; private set; } = double.MaxValue;

    /// <returns>A list of TspNode objects corresponding to the optimal tour for the problem (if it is known).</returns>
    public List<TspNode> OptimalTour { get; private set; } = new List<TspNode>();

    /// <summary>
    /// INode ID's aren't necessarily zero-based.  This integer keeps track of the difference between
    /// the INode ID's and the zero-based indices used everywhere else.
    /// </summary>
    private readonly int _zeroBasedIdOffset;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="item">The item to provide information for (must be symmetric TSP item with 2D nodes).</param>
    /// <exception cref="ArgumentNullException">Thrown when "item" is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if problem nodes are not Node2D types.</exception>
    public SymmetricTspItemInfoProvider(TspLib95Item item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      var nodes2D = item.Problem.NodeProvider.GetNodes();
      _tspNodes = nodes2D.OfType<Node2D>().Select(n => new TspNode(n.Id, n.X, n.Y)).OrderBy(n => n.Id).ToList();
      if (_tspNodes.Any() == false)
      {
        string errMsg = $"Selected problem: {item.Problem.Name} does not contain Node2D objects.";
        throw new ArgumentOutOfRangeException(nameof(item), errMsg);
      }

      _zeroBasedIdOffset = _tspNodes.Min(n => n.Id) - 0;

      var problem = item.Problem;
      NodeCount = problem.NodeProvider.CountNodes();
      ProblemName = problem.Name;

      var random = new Random(Guid.NewGuid().GetHashCode());
      NearestNeighbourTourLength = problem.GetNearestNeighbourTourLength(random);

      SetCoordinateProperties();
      SetOptimalTourProperties(item);
      CalculateDistances(problem);
    }

    /// <summary>
    /// Constructs a tour consisting of TspNode elements from zero based tour indices.
    /// </summary>
    /// <param name="tourIndices">A list of zero-based node indices.</param>
    /// <returns>A list of TspNode objects representing an Ant's constructed tour.</returns>
    public IEnumerable<TspNode> BuildTspNodeTourFromZeroBasedIndices(IEnumerable<int> tourIndices)
    {
      return tourIndices.Select(index => _tspNodes.First(n => n.Id == index + _zeroBasedIdOffset));
    }

    private void SetCoordinateProperties()
    {
      MaxXCoordinate = _tspNodes.Max(i => i.X);
      MinXCoordinate = _tspNodes.Min(i => i.X);
      MaxYCoordinate = _tspNodes.Max(i => i.Y);
      MinYCoordinate = _tspNodes.Min(i => i.Y);
    }

    private void SetOptimalTourProperties(TspLib95Item item)
    {
      if (item.OptimalTour != null)
      {
        var nodes = item.OptimalTour.Nodes.Select(n => item.Problem.NodeProvider.GetNode(n));
        OptimalTour = nodes.OfType<Node2D>().Select(n => new TspNode(n.Id, n.X, n.Y)).ToList();
        OptimalTourLength = item.OptimalTourDistance;
        HasOptimalTour = true;
      }
    }

    private void CalculateDistances(IProblem problem)
    {
      _distances = new double[NodeCount][];

      // Ensure that the nodes are sorted by ID ascending
      // or else all matrix indices will be off.
      var nodes = problem.NodeProvider.GetNodes().OrderBy(n => n.Id).ToArray();
      var weightsProvider = problem.EdgeWeightsProvider;

      for (var i = 0; i < NodeCount; i++)
      {
        // Initialise columns.
        _distances[i] = new double[NodeCount];

        for (var j = 0; j < NodeCount; j++)
        {
          _distances[i][j] = weightsProvider.GetWeight(nodes[i], nodes[j]);
        }
      }
    }
  }
}