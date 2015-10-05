using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TspLibNet.Graph.Nodes;

namespace AntSimComplex.Utilities
{
    public class ListViewTourItem
    {
        public string Type { get; set; }
        public string NodeSequence { get; set; }
        public double Length { get; set; }
        public List<Node2D> Nodes { get; set; }

        public ListViewTourItem(List<Node2D> nodes, double tourLength, string type)
        {
            var ids = from n in nodes
                      select n.Id.ToString();
            NodeSequence = ids.Aggregate((a, b) => a + "," + b);
            Nodes = nodes;
            Length = tourLength;
            Type = type;
        }
    }
}