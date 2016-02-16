using AntSimComplexTspLibItemManager.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexUI.Utilities
{
  /// <summary>
  /// Used to display data related to a tour in a ListView for easy reference.
  /// </summary>
  public class ListViewTourItem
  {
    /// <summary>
    /// What tour does this item represent?
    /// </summary>
    public string TourInfo { get; }

    /// <summary>
    /// A comma-separated list of the node ID's corresponding to the tour (in the
    /// sequence traversed).
    /// </summary>
    public string NodeSequence { get; }

    /// <summary>
    /// The length of the tour.
    /// </summary>
    public double Length { get; }

    /// <summary>
    /// The actual nodes the tour consists of.
    /// </summary>
    public IEnumerable<TspNode> Nodes { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodes">The nodes the tour consists of.</param>
    /// <param name="tourLength">The length of the tour.</param>
    /// <param name="tourInfo">Information about the tour (what tour does this item represent)</param>
    public ListViewTourItem(IEnumerable<TspNode> nodes, double tourLength, string tourInfo)
    {
      var tspNodes = nodes as TspNode[] ?? nodes.ToArray();
      var ids = tspNodes.Select(n => n.Id.ToString());
      NodeSequence = ids.Aggregate((a, b) => a + "," + b);
      Nodes = tspNodes;
      Length = tourLength;
      TourInfo = tourInfo;
    }
  }
}