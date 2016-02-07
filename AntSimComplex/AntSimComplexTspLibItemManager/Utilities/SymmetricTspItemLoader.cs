using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTspLibItemManager.Utilities
{
  /// <summary>
  /// This class is responsible for selecting the symmetrical TSP problems that we're interested in.
  /// For the sake of this research application only problems with fewer than or equal to 100 nodes and
  /// 2D coordinate sets are considered.
  /// </summary>
  internal class SymmetricTspItemLoader
  {
    private readonly List<TspLib95Item> _tspLibItems;

    /// <summary>
    /// The list of names of all symmetric TSP problems loaded.
    /// </summary>
    public List<string> ProblemNames { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="tspLibPath">The directory path to the TSPLIB95 library.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if no TspLib95Items were loaded.</exception>
    public SymmetricTspItemLoader(string tspLibPath)
    {
      try
      {
        var tspLib = new TspLib95(tspLibPath);
        var items = tspLib.LoadAllTSP();

        const int maxNodes = 100;
        var nodeType = typeof(Node2D);

        _tspLibItems = (from i in items
                        let nodes = i.Problem.NodeProvider.GetNodes()
                        where nodes.Count <= maxNodes
                        where nodes.All(n => n.GetType() == nodeType)
                        select i).ToList();

        ProblemNames = _tspLibItems.Select(i => i.Problem.Name).ToList();
      }
      catch (Exception e)
      {
        throw new ArgumentOutOfRangeException($"No TspLib95Items were loaded for path: '{tspLibPath}'", e);
      }
    }

    /// <returns>The TSP library item corresponding to problemName.</returns>
    /// <param name="problemName">The name of the symmetric TSP problem</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if no problem of this name was found.</exception>
    public TspLib95Item GetItem(string problemName)
    {
      var item = _tspLibItems.FirstOrDefault(i => i.Problem.Name == problemName);
      if (item == null)
      {
        throw new ArgumentOutOfRangeException(nameof(problemName), "No problem of this name was found.");
      }
      return item;
    }
  }
}