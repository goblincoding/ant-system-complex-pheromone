using System;

namespace AntSimComplexAlgorithms.Utilities.DataStructures
{
  internal interface IDataStructures
  {
    /// <summary>
    /// Represents the simple pheromone density trails between two nodes (graph arcs)
    /// for the "standard" Ant System implementation. Pheromone is frequently updated
    /// during the evaporation and deposit steps.
    /// </summary>
    double[][] Pheromone { get; }

    /// <summary>
    /// This method does not create the nearest neighbours list, but references
    /// the lists obtained from the original problem with which the <seealso cref="Data"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node">The node index whose neighbours should be returned.</param>
    /// <returns>Returns an array of neighbouring node indices, ordered by ascending distance.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the node index falls outside the expected range.</exception>
    int[] NearestNeighbours(int node);

    /// <summary>
    /// This method does not calculate the edge weight between two nodes, but references
    /// the weights obtained from the original problem with which the <seealso cref="Data"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node1">The index of the first node</param>
    /// <param name="node2">The index of the second node</param>
    /// <returns>Returns the distance (weight of the edge) between two nodes.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
    double Distance(int node1, int node2);

    /// <summary>
    /// Represents the [t_ij]^A [n_ij]^B heuristic values for each edge [i][j] where t_ij is the
    /// pheromone density, n_ij = 1/d_ij, and 'A' and 'B' are the Alpha and Beta parameter values.
    /// This method does not calculate the choice info heuristics, but references values in a matrix
    /// of dimensions dependent on the original problem with which the <seealso cref="Data"/>
    /// object was constructed.
    /// </summary>
    /// <param name="node1">The index of the first node</param>
    /// <param name="node2">The index of the second node</param>
    /// <returns>Returns the "choice info" heuristic for two nodes.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when either of the two node indices fall outside the expected range.</exception>
    double ChoiceInfo(int node1, int node2);

    /// <summary>
    /// Updates the ChoiceInfo matrix with the latest pheromone values.  Should be called after the pheromone update
    /// process is completed.
    /// </summary>
    void UpdateChoiceInfoMatrix();

    /// <summary>
    /// Resets all pheromone densities to the initial pheromone density the pheromone
    /// matrix values were initialised with and updates the choice info matrix.
    /// </summary>
    void ResetPheromone();
  }
}