using System.Collections.Generic;
using System.Linq;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexUI.Utilities
{
  public class ListViewTourItem
  {
    public string Type { get; set; }
    public string NodeSequence { get; set; }
    public double Length { get; set; }
    public IEnumerable<Node2D> Nodes { get; set; }

    public ListViewTourItem(IEnumerable<Node2D> nodes, double tourLength, string type)
    {
      var node2Ds = nodes as Node2D[] ?? nodes.ToArray();
      var ids = from n in node2Ds
                select n.Id.ToString();
      NodeSequence = ids.Aggregate((a, b) => a + "," + b);
      Nodes = node2Ds;
      Length = tourLength;
      Type = type;
    }
  }
}