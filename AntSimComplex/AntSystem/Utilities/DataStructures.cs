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
        /// Once initialised, the values in this matrix do not change.
        /// </summary>
        private double[][] _distances;

        /// <summary>
        /// Represents the [n_ij]^B heuristic values for each edge [i][j] where
        /// n_ij = 1/d_ij and 'B' is the Beta parameter value <seealso cref="Parameters"/>
        /// Once initialised, the values in this matrix do not change.
        /// </summary>
        private double[][] _heuristic;

        /// <summary>
        /// Represents the simple pheromone density trail between two nodes for the
        /// "standard" Ant System implementation.
        /// </summary>
        private double[][] _pheromone;

        /// <summary>
        /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where
        /// t_ij is the pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha
        /// and Beta parameter values <seealso cref="Parameters"/>
        /// </summary>
        private double[][] _choiceInfo;

        /// <summary>
        /// Represents the nearest neighbour lists for all nodes where _nearest[i]
        /// returns an array of the (adjusted) node ids sorted by increasing distance
        /// from node i.
        /// Once initialised, the values in this matrix do not change.
        /// </summary>
        private int[][] _nearest;

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
                throw new ArgumentNullException(nameof(problem), $"The {nameof(DataStructures)} constructor needs a valid problem instance argument");
            }

            _nodeCount = problem.NodeProvider.CountNodes();
            OrderedNodeIndices = Enumerable.Range(0, _nodeCount).ToArray();

            // Order is important!
            BuildInfoMatrices(problem, initialPheromoneDensity);
            BuildNearestNeighboursLists();
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
        /// <returns>Returns an array of neighbouring node indices, ordered by ascending distance.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the node index falls outside the expected range.</exception>
        public int[] NearestNeighbours(int node)
        {
            return _nearest[node];
        }

        /// <summary>
        /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where t_ij is the
        /// pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha and Beta parameter values.
        /// This method does not calculate the choice info heuristics, but references values in a matrix
        /// of dimensions dependent on the original problem with which the <seealso cref="DataStructures"/>
        /// object was constructed. Care must therefore be taken to only use node indices obtained
        /// through <seealso cref="OrderedNodeIndices"/> for the problem at hand since INode ID's
        /// will result in incorrect index offsets.
        /// </summary>
        /// <param name="node1">The index of the first node</param>
        /// <param name="node2">The index of the second node</param>
        /// <returns>Returns the "choice info" heuristic for two nodes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
        public double ChoiceInfo(int node1, int node2)
        {
            return _choiceInfo[node1][node2];
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
            UpdateChoiceInfo(node1, node2);
        }

        /// <summary>
        /// From Ant Colony Optimization, Dorigo 2004 , p100
        /// "In fact, although for symmetric TSPs we only need to store n(n-1)/2 distinct
        /// distances, it is more efficient to use an n^2 matrix to avoid performing
        /// additional operations to check whether, when accessing a generic distance
        /// d(i,j), entry (i,j) or entry (j,i) of the matrix should be used."
        ///
        /// Pheromone density matrix - p102
        /// Heuristics matrix - p117
        /// Choice info matrix - p117
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="initialPheromoneDensity"></param>
        private void BuildInfoMatrices(IProblem problem, double initialPheromoneDensity)
        {
            _distances = new double[_nodeCount][];
            _pheromone = new double[_nodeCount][];
            _heuristic = new double[_nodeCount][];
            _choiceInfo = new double[_nodeCount][];

            // Ensure that the nodes are sorted by ID ascending
            // or else all matrix indices will be off.
            var nodes = problem.NodeProvider.GetNodes()
                                                .OrderBy(n => n.Id)
                                                .ToArray();

            var weightsProvider = problem.EdgeWeightsProvider;

            for (int i = 0; i < _nodeCount; i++)
            {
                _distances[i] = new double[_nodeCount];
                _pheromone[i] = new double[_nodeCount];
                _heuristic[i] = new double[_nodeCount];
                _choiceInfo[i] = new double[_nodeCount];

                for (int j = 0; j < _nodeCount; j++)
                {
                    _distances[i][j] = weightsProvider.GetWeight(nodes[i], nodes[j]);
                    _heuristic[i][j] = Math.Pow((1 / _distances[i][j]), Parameters.Beta);
                    _pheromone[i][j] = initialPheromoneDensity;
                    UpdateChoiceInfo(i, j);
                }
            }
        }

        private void UpdateChoiceInfo(int i, int j)
        {
            _choiceInfo[i][j] = Math.Pow(_pheromone[i][j], Parameters.Alpha) * _heuristic[i][j];
        }

        /// <summary>
        /// From Ant Colony Optimization, Dorigo 2004 , p116.
        /// </summary>
        private void BuildNearestNeighboursLists()
        {
            _nearest = new int[_nodeCount][];

            for (int i = 0; i < _nodeCount; i++)
            {
                // Remove nodes from their own nearest neighbour lists.
                _nearest[i] = new int[_nodeCount - 1];
                var pairs = _distances[i]
                                  .Select((d, j) => new KeyValuePair<double, int>(d, j))
                                  .OrderBy(d => d.Key).ToList();

                // Add the node offset here so that it does not have to happen on every
                // "nearest neighbours list" query.
                var nearestIndices = pairs.Where(p => p.Value != i).Select(p => p.Value).ToArray();
                nearestIndices.CopyTo(_nearest[i], 0);

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