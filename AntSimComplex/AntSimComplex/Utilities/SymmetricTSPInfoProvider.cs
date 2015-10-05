using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexUI.Utilities
{
    /// <summary>
    /// This class is responsible for selecting the symmetrical TSP problems that we're interested in.
    /// For the sake of this research application only problems with fewer than or equal to 100 nodes and
    /// 2D coordinate sets are considered.
    /// </summary>
    public class SymmetricTSPInfoProvider
    {
        /// <returns>A list of problem graph Node2D objects.</returns>
        public List<Node2D> Nodes2D { get; } = new List<Node2D>();

        /// <returns>A list of Node2D objects corresponding to the optimal tour for the problem (if it is known).</returns>
        public List<Node2D> OptimalTourNodes2D { get; } = new List<Node2D>();

        /// <returns>The optimal tour length if known, double.MaxValue if not.</returns>
        public double OptimalTourLength { get; } = double.MaxValue;

        public bool HasOptimalTour { get; } = false;

        /// <summary>
        /// INode ID's aren't necessarily zero-based.  This integer keeps track of the difference between
        /// the INode ID's and the zero-based indices used by <seealso cref="AntSystem"/>, the underlying
        /// <seealso cref="DataStructures"/> and <seealso cref="Ant"/>.
        /// </summary>
        private int _zeroBasedOffset = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">The item to provide information for.</param>
        /// <exception cref="ArgumentNullException">Thrown when "item" is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if problem nodes are not Node2D types.</exception>
        public SymmetricTSPInfoProvider(TspLib95Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Nodes2D = (from n in item.Problem.NodeProvider.GetNodes()
                       where n is Node2D
                       select n as Node2D)?.ToList();

            Nodes2D.RemoveAll(n => n == null);
            if (Nodes2D?.Any() == false)
            {
                throw new ArgumentOutOfRangeException(nameof(item), "Selected problem node list does not contain Node2D objects.");
            }

            _zeroBasedOffset = Nodes2D.Min(n => n.Id) - 0;

            if (item?.OptimalTour != null)
            {
                var nodes = (from n in item.OptimalTour.Nodes
                             select item.Problem.NodeProvider.GetNode(n) as Node2D).ToList();
                nodes.RemoveAll(n => n == null);
                OptimalTourNodes2D = nodes;
                OptimalTourLength = item.OptimalTourDistance;
                HasOptimalTour = true;
            }
        }

        /// <summary>
        /// Constructs a tour consisting of Node2D elements from the zero based tour indices
        /// representing an Ant's tour of the TSP graph.
        /// </summary>
        /// <param name="antTourIndices">A list of zero-based node indices.</param>
        /// <returns>A list of Node2D objects representing an Ant's constructed tour.</returns>
        public List<Node2D> BuildNode2DTourFromZeroBasedIndices(List<int> antTourIndices)
        {
            var nodes = new List<Node2D>();
            foreach (var index in antTourIndices)
            {
                var node = Nodes2D.First(n => n.Id == index + _zeroBasedOffset);
                nodes.Add(node);
            }
            return nodes;
        }

        /// <returns>The maximum "x" coordinate of all the nodes in the graph</returns>
        public double GetMaxX()
        {
            var max = Nodes2D.Max(i => i.X);
            return max;
        }

        /// <returns>The minimum "x" coordinate of all the nodes in the graph</returns>
        public double GetMinX()
        {
            var min = Nodes2D.Min(i => i.X);
            return min;
        }

        /// <returns>The maximum "y" coordinate of all the nodes in the graph</returns>
        public double GetMaxY()
        {
            var max = Nodes2D.Max(i => i.Y);
            return max;
        }

        /// <returns>The minimum "y" coordinate of all the nodes in the graph</returns>
        public double GetMinY()
        {
            var min = Nodes2D.Min(i => i.Y);
            return min;
        }
    }
}