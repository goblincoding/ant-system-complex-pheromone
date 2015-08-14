using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplex.Utilities
{
    /// <summary>
    /// This class is responsible for selecting the symmetrical TSP problems that we're interested in.
    /// For the sake of this research application, only problem with fewer than or equal to 100 nodes
    /// are considered.  Furthermore, only problems with 2D coordinate sets are considered.
    /// </summary>
    internal class TspLibProcessor
    {
        private List<TspLib95Item> _tspLibItems;
        public List<string> ProblemNames { get; private set; }

        public TspLibProcessor(TspLib95 lib)
        {
            // We only need to check one node for node type since it is not possible to
            // have different types in the same list.
            _tspLibItems = (from i in lib.TSPItems()
                            where i.Problem.NodeProvider.CountNodes() <= 100
                            where i.Problem.NodeProvider.GetNodes().First() is Node2D
                            select i).ToList();

            ProblemNames = (from i in _tspLibItems
                            select i.Problem.Name).ToList();
        }

        public List<Node2D> GetNodes(string problemName)
        {
            var item = _tspLibItems.First(i => i.Problem.Name == problemName);
            var nodes = from n in item.Problem.NodeProvider.GetNodes()
                        select n as Node2D;
            return nodes.ToList();
        }

        public double GetMaxX(string problemName)
        {
            var nodes = GetNodes(problemName);
            var max = nodes.Max(i => i.X);
            return max;
        }

        public double GetMinX(string problemName)
        {
            var nodes = GetNodes(problemName);
            var max = nodes.Min(i => i.X);
            return max;
        }

        public double GetMaxY(string problemName)
        {
            var nodes = GetNodes(problemName);
            var max = nodes.Max(i => i.Y);
            return max;
        }

        public double GetMinY(string problemName)
        {
            var nodes = GetNodes(problemName);
            var max = nodes.Min(i => i.Y);
            return max;
        }
    }
}