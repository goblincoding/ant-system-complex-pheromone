using System.Collections.Generic;
using System.Linq;
using TspLibNet.Graph.Nodes;

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
    public string TourInfo { get; set; }

    /// <summary>
    /// A comma-separated list of the node ID's corresponding to the tour (in the
    /// sequence traversed).
    /// </summary>
    public string NodeSequence { get; set; }

    /// <summary>
    /// The length of the tour.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// The actual nodes the tour consists of.
    /// </summary>
    public IEnumerable<Node2D> Nodes { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="nodes">The nodes the tour consists of.</param>
    /// <param name="tourLength">The length of the tour.</param>
    /// <param name="tourInfo">Information about the tour (what tour does this item represent)</param>
    public ListViewTourItem(IEnumerable<Node2D> nodes, double tourLength, string tourInfo)
    {
      var node2Ds = nodes as Node2D[] ?? nodes.ToArray();
      var ids = from n in node2Ds
                select n.Id.ToString();
      NodeSequence = ids.Aggregate((a, b) => a + "," + b);
      Nodes = node2Ds;
      Length = tourLength;
      TourInfo = tourInfo;
    }
  }
}