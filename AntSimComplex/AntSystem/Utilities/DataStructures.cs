using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAS.Utilities
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
        /// Represents the simple pheromone density trail between two nodes for the
        /// "standard" Ant System implementation.
        /// </summary>
        private double[][] _pheromone;

        /// <summary>
        /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
        /// nearest neighbour and pheromone density matrices.
        /// </summary>
        private int _nodeCount = 0;

        /// <summary>
        /// Since INode ID's are not necessarily zero-indexed, using this property to obtain
        /// the list of indices corresponding to the ordered (ascending by INode ID) list of
        /// problem nodes is essential for use with the <seealso cref="DataStructures"/> object.
        /// </summary>
        public int[] OrderedNodeIndices { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
        public DataStructures(IProblem problem, double initialPheromoneDensity)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The AntSystem constructor needs a valid problem instance argument");
            }

            _nodeCount = problem.NodeProvider.CountNodes();
            OrderedNodeIndices = Enumerable.Range(0, _nodeCount).ToArray();

            // Order is important!
            BuildDistancesMatrix(problem);
            BuildNearestNeighboursMatrix();
            BuildPheromoneDensityMatrix(initialPheromoneDensity);
        }

        /// <summary>
        /// This method does not calculate the edge weight between two nodes, but references
        /// the weights obtained from the original problem with which the <seealso cref="DataStructures"/>
        /// object was constructed. Care must therefore be taken to only use node indices obtained
        /// through <seealso cref="OrderedNodeIndices"/> for the problem at hand since INode ID's
        /// will result in incorrect index offsets.
        /// </summary>
        /// <param name="node1">The index of the first node</param>
        /// <param name="node2">The index of the second node</param>
        /// <returns>Returns the distance (weight of the edge) between two nodes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
        public double Distance(int node1, int node2)
        {
            return _distances[node1][node2];
        }

        /// <summary>
        /// This method does not create the nearest neighbours list, but references
        /// the lists obtained from the original problem with which the <seealso cref="DataStructures"/>
        /// object was constructed. Care must therefore be taken to only use node indices obtained
        /// through <seealso cref="OrderedNodeIndices"/> for the problem at hand since INode ID's
        /// will result in incorrect index offsets.
        /// </summary>
        /// <param name="node">The node index whose neighbours should be returned.</param>
        /// <returns>Returns an array of neighbouring node indices in ascending order.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the node index falls outside the expected range.</exception>
        public int[] NearestNeighbours(int node)
        {
            return _nearest[node];
        }

        /// <summary>
        /// This method depends on the graph dimensions of the original problem with which the
        /// <seealso cref="DataStructures"/> object was constructed. Care must therefore be taken
        /// to only use node indices obtained through <seealso cref="OrderedNodeIndices"/> for the
        /// problem at hand since INode ID's will result in incorrect index offsets.
        /// </summary>
        /// <param name="node1">The index of the first node</param>
        /// <param name="node2">The index of the second node</param>
        /// <returns>Returns the distance (weight of the edge) between two nodes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
        public double PheromoneTrailDensity(int node1, int node2)
        {
            return _pheromone[node1][node2];
        }

        /// <summary>
        /// Sets the pheromone trail between "node1" and "node2" to "value".
        ///
        /// This method depends on the graph dimensions of the original problem with which the
        /// <seealso cref="DataStructures"/> object was constructed. Care must therefore be taken
        /// to only use node indices obtained through <seealso cref="OrderedNodeIndices"/> for the
        /// problem at hand since INode ID's will result in incorrect index offsets.
        /// </summary>
        /// <param name="node1">The index of the first node</param>
        /// <param name="node2">The index of the second node</param>
        /// <param name="value">The pheromone density</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
        public void SetPheromoneTrailDensity(int node1, int node2, double value)
        {
            _pheromone[node1][node2] = value;
        }

        /// <summary>
        /// From Ant Colony Optimization, Dorigo 2004 , p100
        /// "In fact, although for symmetric TSPs we only need to store n(n-1)/2 distinct
        /// distances, it is more efficient to use an n^2 matrix to avoid performing
        /// additional operations to check whether, when accessing a generic distance
        /// d(i,j), entry (i,j) or entry (j,i) of the matrix should be used."
        /// </summary>
        /// <param name="problem"></param>
        private void BuildDistancesMatrix(IProblem problem)
        {
            // Ensure that the nodes are sorted by ID ascending
            // or else all matrix indices will be off.
            var nodes = problem.NodeProvider.GetNodes()
                                                .OrderBy(n => n.Id)
                                                .ToArray();

            var weightsProvider = problem.EdgeWeightsProvider;
            _distances = new double[_nodeCount][];

            for (int i = 0; i < _nodeCount; i++)
            {
                _distances[i] = new double[_nodeCount];
                var node1 = nodes[i];

                for (int j = 0; j < _nodeCount; j++)
                {
                    var node2 = nodes[j];
                    _distances[i][j] = weightsProvider.GetWeight(node1, node2);
                    //Debug.WriteLine($"Distance from node {node1.Id} to {node2.Id} is {distances[i][j]}");
                }
            }
        }

        private void BuildNearestNeighboursMatrix()
        {
            _nearest = new int[_nodeCount][];

            for (int n = 0; n < _nodeCount; n++)
            {
                _nearest[n] = new int[_nodeCount];
                var pairs = _distances[n]
                                  .Select((d, i) => new KeyValuePair<double, int>(d, i))
                                  .OrderBy(d => d.Key).ToList();

                // Add the node offset here so that it does not have to happen on every
                // "nearest neighbours list" query.
                var nearestIndices = pairs.Select(p => p.Value).ToArray();
                nearestIndices.CopyTo(_nearest[n], 0);

                // Debug.
                //for (int i = 0; i < _nearest[n].Length; i++)
                //{
                //    var index = _nearest[n][i] - _nodeIDOffset;
                //    Debug.WriteLine($"Distance from {n} to {index} is {_distances[n][index]}");
                //}
            }
        }

        /// <summary>
        /// From Ant Colony Optimization, Dorigo 2004 , p102
        /// "Again, as was the case for the distance matrix, it is more convenient to use some
        /// redundancy and to store the pheromones in a symmetric n^2 matrix."
        /// </summary>
        private void BuildPheromoneDensityMatrix(double initialPheromoneDensity)
        {
            _pheromone = new double[_nodeCount][];
            for (int n = 0; n < _nodeCount; n++)
            {
                _pheromone[n] = new double[_nodeCount];
                for (int i = 0; i < _nodeCount; i++)
                {
                    _pheromone[n][i] = initialPheromoneDensity;
                }
            }
        }
    }
}