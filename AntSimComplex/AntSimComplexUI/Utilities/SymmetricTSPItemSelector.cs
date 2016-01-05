using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;

namespace AntSimComplexUI.Utilities
{
  /// <summary>
  /// This class selects a subset of symmetric TSP item instances based on the criteria passed in to its constructor.
  /// </summary>
  public class SymmetricTspItemSelector
  {
    private readonly List<TspLib95Item> _tspLibItems;

    /// <summary>
    /// The list of names of all symmetric TSP problems loaded.
    /// </summary>
    public List<string> ProblemNames { get; }

    /// <summary>
    /// Constructor.  Exceptions thrown are from underlying TspLib95 instance.
    /// </summary>
    /// <param name="tspLibPath">The directory path to the TSPLIB95 library.</param>
    /// <param name="maxNodes">The maximum number of nodes the selected TSP problems may contain.</param>
    /// <param name="nodeType">The derived node class (e.g. Node2D)</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if no TspLib95Items were loaded.</exception>
    public SymmetricTspItemSelector(string tspLibPath, uint maxNodes, Type nodeType)
    {
      try
      {
        var tspLib = new TspLib95(tspLibPath);
        var items = tspLib.LoadAllTSP();
        var tspLib95Items = items as TspLib95Item[] ?? items.ToArray();

        // We only need to check one node for node type since it is not possible to
        // have different types in the same list.
        _tspLibItems = (from i in tspLib95Items
                        where i.Problem.NodeProvider.CountNodes() <= maxNodes
                        where i.Problem.NodeProvider.GetNodes().First().GetType() == nodeType
                        select i).ToList();

        ProblemNames = (from i in _tspLibItems
                        select i.Problem.Name).ToList();
      }
      catch (Exception e)
      {
        throw new ArgumentOutOfRangeException($"No TspLib95Items were loaded for {tspLibPath} " +
                                              $"with max nodes {maxNodes} and node type {nodeType}", e);
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