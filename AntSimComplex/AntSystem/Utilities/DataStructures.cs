using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Represents the nearest neighbour lists for all nodes where _nearest[i]
        /// returns an array of the (adjusted) node ids sorted by increasing distance
        /// from node i.
        /// </summary>
        private int[][] _nearest;

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

            // Order is important!
            BuildDistancesMatrix(problem);
            BuildNearestNeighboursMatrix();
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
            return _distances[node1.Id - _nodeIDOffset][node2.Id - _nodeIDOffset];
        }

        /// <summary>
        /// This method does not create the nearest neighbours list, but references
        /// the lists obtained from the original problem with which the DataStructures object
        /// was constructed. Care must therefore be taken to only use node instances that exist
        /// for the problem at hand <seealso cref="DataStructures"/>
        /// </summary>
        /// <param name="node">The node whose neighbours to return.</param>
        /// <returns>Returns an array of neighbouring INode ID's in ascending order.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the node ID fall outside the expected range.</exception>
        public int[] GetNearestNeighbourIDs(INode node)
        {
            return _nearest[node.Id - _nodeIDOffset];
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
        private void BuildDistancesMatrix(IProblem problem)
        {
            var nodes = problem.NodeProvider.GetNodes();
            _nodeIDOffset = nodes.Min(n => n.Id);

            var nodeCount = nodes.Count;
            var weightsProvider = problem.EdgeWeightsProvider;
            _distances = new double[nodeCount][];

            for (int i = 0; i < nodeCount; i++)
            {
                _distances[i] = new double[nodeCount];
                var node1 = nodes[i];

                for (int j = 0; j < nodeCount; j++)
                {
                    var node2 = nodes[j];
                    _distances[i][j] = weightsProvider.GetWeight(node1, node2);
                    //Debug.WriteLine($"Distance from node {node1.Id} to {node2.Id} is {distances[i][j]}");
                }
            }
        }

        private void BuildNearestNeighboursMatrix()
        {
            var nrNodes = _distances.Length;
            _nearest = new int[nrNodes][];

            for (int n = 0; n < nrNodes; n++)
            {
                _nearest[n] = new int[nrNodes];
                var pairs = _distances[n]
                                  .Select((d, i) => new KeyValuePair<double, int>(d, i))
                                  .OrderBy(d => d.Key).ToList();

                // Add the node offset here so that it does not have to happen on every
                // "nearest neighbours list" query.
                var nearestIndices = (from p in pairs
                                      select p.Value + _nodeIDOffset).ToArray();
                nearestIndices.CopyTo(_nearest[n], 0);

                // Debug.
                //for (int i = 0; i < _nearest[n].Length; i++)
                //{
                //    var index = _nearest[n][i] - _nodeIDOffset;
                //    Debug.WriteLine($"Distance from {n} to {index} is {_distances[n][index]}");
                //}
            }
        }
    }
}