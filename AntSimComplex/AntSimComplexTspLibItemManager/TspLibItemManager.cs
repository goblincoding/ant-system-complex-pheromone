using AntSimComplexTspLibItemManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexTspLibItemManager
{
  public sealed class TspLibItemManager
  {
    private readonly SymmetricTspItemLoader _itemLoader;
    private SymmetricTspItemInfoProvider _infoProvider;

    /// <returns>The number of nodes in the TSP graph.</returns>
    public int NodeCount => _infoProvider.NodeCount;

    /// <returns>Returns a distance matrix of edge weights between nodes.  I.e. Distances[i][j]
    /// will return the distance between node i and j.</returns>
    public IReadOnlyList<IReadOnlyList<double>> Distances => _infoProvider.Distances;

    /// <returns>Returns the tour length constructed by the nearest neighbour heuristic (ACO Dorigo Ch3, p70).</returns>
    public double NearestNeighbourTourLength => _infoProvider.NearestNeighbourTourLength;

    /// <returns>The maximum "x" coordinate of all the nodes in the graph</returns>
    public double MaxXCoordinate => _infoProvider.MaxXCoordinate;

    /// <returns>The minimum "x" coordinate of all the nodes in the graph</returns>
    public double MinXCoordinate => _infoProvider.MinXCoordinate;

    /// <returns>The maximum "y" coordinate of all the nodes in the graph</returns>
    public double MaxYCoordinate => _infoProvider.MaxYCoordinate;

    /// <returns>The minimum "y" coordinate of all the nodes in the graph</returns>
    public double MinYCoordinate => _infoProvider.MinYCoordinate;

    /// <returns>True if the current item has a known optimal tour.</returns>
    public bool HasOptimalTour => _infoProvider.HasOptimalTour;

    /// <returns>The optimal tour length if known, double.MaxValue if not.</returns>
    public double OptimalTourLength => _infoProvider.OptimalTourLength;

    /// <returns>A list of TspNode objects corresponding to the optimal tour for the problem (if it is known).</returns>
    public List<TspNode> OptimalTour => _infoProvider.OptimalTour;

    /// <returns>The name of the current TSP problem.</returns>
    public string ProblemName => _infoProvider.ProblemName;

    /// <returns>A list of the current TspNodes ordered by ID.</returns>
    public IReadOnlyList<TspNode> TspNodes => _infoProvider.TspNodes;

    /// <returns>The name of the current TSP problem.</returns>
    public List<string> AllProblemNames => _itemLoader.ProblemNames;

    /// <summary>
    /// Constructor.  Automatically loads the first STSP instance available.
    /// </summary>
    /// <param name="tspLibPath">The directory path to the TSPLIB95 library.</param>
    /// <exception cref="ArgumentException">Thrown if no TSP items were loaded.</exception>
    public TspLibItemManager(string tspLibPath)
    {
      try
      {
        _itemLoader = new SymmetricTspItemLoader(tspLibPath);
        var problemName = _itemLoader.ProblemNames.First();
        LoadItem(problemName);
      }
      catch (ArgumentException e)
      {
        throw new ArgumentException("Could not load TSP items", e);
      }
    }

    /// <summary>Loads the TSP library item corresponding to problemName.</summary>
    /// <param name="problemName">The name of the symmetric TSP problem to load</param>
    /// <exception cref="ArgumentException">Thrown if the problem could not be loaded.</exception>
    public void LoadItem(string problemName)
    {
      try
      {
        var currentItem = _itemLoader.GetItem(problemName);
        _infoProvider = new SymmetricTspItemInfoProvider(currentItem);
      }
      catch (ArgumentException e)
      {
        throw new ArgumentException("Could not load TSP problem", e);
      }
    }

    /// <summary>
    /// Constructs a tour consisting of TspNode elements from zero based tour indices.
    /// </summary>
    /// <param name="tour">A list of zero-based node indices.</param>
    /// <returns>A list of TspNode objects representing an Ant's constructed tour.</returns>
    public IEnumerable<TspNode> ConvertTourIndicesToNodes(IEnumerable<int> tour)
    {
      return _infoProvider.BuildTspNodeTourFromZeroBasedIndices(tour);
    }
  }
}