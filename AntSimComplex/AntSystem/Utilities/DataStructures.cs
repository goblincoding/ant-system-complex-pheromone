using System;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexAS
{
    /// <summary>
    /// This class represents the prepopulated (prior to algorithm run-time), consolidated,
    /// calculated values of different aspects of a particular TSP problem instance in data
    /// structures such as distance and nearest neighbour matrices.
    /// </summary>
    public class DataStructures
    {
        /// <summary>
        /// Represents ALL inter-city distances in a grid format, i.e. querying
        /// _distances[3][5] will return the distance from node 3 to node 5.
        /// </summary>
        private double[][] _distances;

        /// <summary>
        /// The node ID numbering in TSPLIB95 problem sets are not necessarily zero based.
        /// This creates difficulties for data structure reference by node ID since data
        /// structures are obviously zero-based indexed.  This node offset takes care of
        /// the difference.
        /// </summary>
        private int _nodeIDOffset = 0;

        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
        public DataStructures(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The AntSystem constructor needs a valid problem instance argument");
            }

            _distances = CalculateInterNodeDistances(problem);
        }

        /// <summary>
        /// This method does not calculate the edge weight between two nodes, but references
        /// the weights obtained from the original problem with which the DataStructures object
        /// was constructed. Care must therefore be taken to only use node instances that exist
        /// for the problem at hand <seealso cref="DataStructures"/>
        /// </summary>
        /// <param name="node1">First node</param>
        /// <param name="node2">Second node</param>
        /// <returns>Returns the distance (weight of the edge) between two nodes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node IDs fall outside the expected range.</exception>
        public double GetInterNodeDistance(INode node1, INode node2)
        {
            var i = node1.Id - _nodeIDOffset;
            var j = node2.Id - _nodeIDOffset;

            if (i > _distances.Length || j > _distances.Length)
            {
                throw new IndexOutOfRangeException();
            }

            return _distances[i][j];
        }

        /// <summary>
        /// From Ant Colony Optimization, Dorigo 2004 , p100
        /// "In fact, although for symmetric TSPs we only need to store n(n-1)/2 distinct
        /// distances, it is more efficient to use an n^2 matrix to avoid performing
        /// additional operations to check whether, when accessing a generic distance
        /// d(i,j), entry (i,j) or entry (j,i) of the matrix should be used."
        /// </summary>
        /// <param name="problem"></param>
        /// <returns>A jagged array (n^2 matrix) of inter-node distances.</returns>
        private double[][] CalculateInterNodeDistances(IProblem problem)
        {
            var nodes = problem.NodeProvider.GetNodes();
            _nodeIDOffset = nodes.Min(n => n.Id);

            var nodeCount = nodes.Count;
            var weightsProvider = problem.EdgeWeightsProvider;
            double[][] distances = new double[nodeCount][];

            for (int i = 0; i < nodeCount; i++)
            {
                distances[i] = new double[nodeCount];
                var node1 = nodes[i];

                for (int j = 0; j < nodeCount; j++)
                {
                    var node2 = nodes[j];
                    distances[i][j] = weightsProvider.GetWeight(node1, node2);
                    //Debug.WriteLine($"Distance from node {node1.Id} to {node2.Id} is {distances[i][j]}");
                }
            }

            return distances;
        }
    }
}