using AntSimComplexTspLibItemManager.Utilities;
using System.Collections.Generic;
using System.Windows;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTspLibItemManager
{
  public class TspLibItemManager
  {
    private readonly SymmetricTspItemLoader _itemLoader;
    private SymmetricTspItemInfoProvider _infoProvider;
    private TspLib95Item _currentItem;

    /// <returns>The name of the current TSP problem.</returns>
    public List<string> AllProblemNames => _itemLoader.ProblemNames;

    /// <returns>The name of the current TSP problem.</returns>
    public string ProblemName => _infoProvider.ProblemName;

    /// <returns>The maximum "x" coordinate of all the nodes in the graph</returns>
    public double MaxXCoordinate => _infoProvider.MaxXCoordinate;

    /// <returns>The minimum "x" coordinate of all the nodes in the graph</returns>
    public double MinXCoordinate => _infoProvider.MinXCoordinate;

    /// <returns>The maximum "y" coordinate of all the nodes in the graph</returns>
    public double MaxYCoordinate => _infoProvider.MaxYCoordinate;

    /// <returns>The minimum "y" coordinate of all the nodes in the graph</returns>
    public double MinYCoordinate => _infoProvider.MinYCoordinate;

    public bool HasOptimalTour => _infoProvider.HasOptimalTour;

    /// <returns>The optimal tour length if known, double.MaxValue if not.</returns>
    public double OptimalTourLength => _infoProvider.OptimalTourLength;

    /// <returns>A list of Node2D objects corresponding to the optimal tour for the problem (if it is known).</returns>
    public List<Node2D> OptimalTour => _infoProvider.OptimalTour;

    /// <returns>A list of Points corresponding to the current nodes' coordinates.</returns>
    public IEnumerable<Point> NodeCoordinatesAsPoints => _infoProvider.NodeCoordinatesAsPoints;

    public TspLibItemManager(string tspLibPath)
    {
      _itemLoader = new SymmetricTspItemLoader(tspLibPath);
    }

    public void LoadItem(string problemName)
    {
      _currentItem = _itemLoader.GetItem(problemName);
      _infoProvider = new SymmetricTspItemInfoProvider(_currentItem);
    }

    public IProblem CurrentProblem()
    {
      return _currentItem.Problem;
    }

    public IEnumerable<Node2D> ConvertTourIndicesToNodes(IEnumerable<int> tour)
    {
      return _infoProvider.BuildNode2DTourFromZeroBasedIndices(tour);
    }
  }
}