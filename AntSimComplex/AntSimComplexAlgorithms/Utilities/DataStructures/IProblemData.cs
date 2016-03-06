using AntSimComplexAlgorithms.Ants;
using System;
using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Utilities.DataStructures
{
  internal interface IProblemData
  {
    /// <summary>
    /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
    /// nearest neighbour and pheromone density matrices.
    /// </summary>
    int NodeCount { get; }

    /// <summary>
    /// This method does not create the nearest neighbours list, but references
    /// the lists obtained from the original problem with which the <seealso cref="StandardProblemData"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node">The node index whose neighbours should be returned.</param>
    /// <returns>Returns an array of neighbouring node indices, ordered by ascending distance.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the node index falls outside the expected range.</exception>
    IReadOnlyList<int> NearestNeighbours(int node);

    /// <summary>
    /// This method does not calculate the edge weight between two nodes, but references
    /// the weights obtained from the original problem with which the <seealso cref="StandardProblemData"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node1">The index of the first node</param>
    /// <param name="node2">The index of the second node</param>
    /// <returns>Returns the distance (weight of the edge) between two nodes (double.MaxValue if to node indices are the same).</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
    double Distance(int node1, int node2);

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where t_ij is the
    /// pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha and Beta parameter values.
    /// This method does not calculate the choice info heuristics, but references values in a matrix
    /// of dimensions dependent on the original problem with which the <seealso cref="StandardProblemData"/>
    /// object was constructed.
    /// </summary>
    /// <param name="ant">The ant for which the choice info structure is requested.</param>
    /// <returns>Returns "choice info" heuristic data.</returns>
    IReadOnlyList<IReadOnlyList<double>> ChoiceInfo(IAnt ant);

    /// <summary>
    /// Updates all pheromone trails traversed by the ants during their solution construction.
    /// </summary>
    /// <param name="ants">A list of active ants.</param>
    void UpdatePheromoneTrails(IEnumerable<IAnt> ants);

    /// <summary>
    /// Resets all pheromone densities to the initial pheromone density the pheromone
    /// matrix values were initialised with and updates the choice info matrix.
    /// </summary>
    void ResetPheromone();
  }
}