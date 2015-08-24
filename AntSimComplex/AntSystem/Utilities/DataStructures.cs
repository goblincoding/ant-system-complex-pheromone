using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;

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

        private double[][] CalculateInterNodeDistances(IProblem problem)
        {
            var nodes = problem.NodeProvider.GetNodes();
            var weightsProvider = problem.EdgeWeightsProvider;
            double[][] distances = new double[nodes.Count][];

            for (int i = 0; i < nodes.Count; i++)
            {
                distances[i] = new double[nodes.Count];
                var node1 = nodes[i];

                for (int j = i + 1; j < nodes.Count; j++)
                {
                    var node2 = nodes[j];
                    distances[i][j] = weightsProvider.GetWeight(node1, node2);
                    Debug.WriteLine($"Distance from node {node1.Id} to {node2.Id} is {distances[i][j]}");
                }
            }

            return distances;
        }
    }
}