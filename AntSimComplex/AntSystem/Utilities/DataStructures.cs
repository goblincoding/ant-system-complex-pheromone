using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// The distance matrix is optimised so as to not duplicate values across
        /// the diagonal (i.e. since [3][5] and [5][3] is the same distance, only
        /// [3][5] exists within the jagged array).
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
            _nodeIDOffset = problem.NodeProvider.GetNodes().Min(n => n.Id);
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

            // For inter-node distances, [i][j] == [j][i],
            // but since we are working with a lower triangular matrix
            // (in the form of our jagged array), we need to ensure we only
            // query the values left of the diagonal.
            return (i > j) ? _distances[i][j] : _distances[j][i];
        }

        /// <summary>
        /// Builds a "lower triangular" matrix of inter-node distances, preventing
        /// duplicate entries by doing so (i.e. distance from node i to j is the same
        /// as the distance from node j to node i, so no need to spend the time
        /// calculating both...this only works since we work with symmetric TSP problems).
        /// </summary>
        /// <param name="problem"></param>
        /// <returns>A jagged array of inter-node distances.</returns>
        private double[][] CalculateInterNodeDistances(IProblem problem)
        {
            var nodes = problem.NodeProvider.GetNodes();
            var weightsProvider = problem.EdgeWeightsProvider;

            var rowDimension = nodes.Count;
            double[][] distances = new double[rowDimension][];

            for (int i = rowDimension - 1; i >= 0; i--)
            {
                distances[i] = new double[i + 1];
                var node1 = nodes[i];

                for (int j = 0; j < i; j++)
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