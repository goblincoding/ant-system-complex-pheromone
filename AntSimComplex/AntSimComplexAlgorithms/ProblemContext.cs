using AntSimComplexAlgorithms.Utilities;
using System;
using TspLibNet;

namespace AntSimComplexAlgorithms
{
    /// <summary>
    /// Wraps all of the Utility classes containing information related to a specific TSP problem.
    /// </summary>
    public class ProblemContext
    {
        /// <summary>
        /// Use a single, static random variable so that we do not end up with roughly
        /// the same number generation sequences with fast clock cycles.
        /// </summary>
        public static Random Random { get; } = new Random();

        /// <summary>
        /// Nr of nodes is used everywhere as it determines the dimensions of the distance,
        /// nearest neighbour and pheromone density matrices.
        /// </summary>
        public int NodeCount { get; }

        /// <summary>
        /// Provides access to the problem constants.
        /// </summary>
        public Parameters Parameters { get; }

        /// <summary>
        /// Provides access to the problem information matrices (distances, pheromone density, etc).
        /// </summary>
        public DataStructures DataStructures { get; }

        /// <summary>
        /// Provides access to the roulette wheel selector.
        /// </summary>
        public RouletteWheelSelector RouletteWheelSelector { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        public ProblemContext(IProblem problem)
        {
            Parameters = new Parameters(problem);
            DataStructures = new DataStructures(problem, Parameters.InitialPheromone);
            RouletteWheelSelector = new RouletteWheelSelector(DataStructures, Random);
            NodeCount = problem.NodeProvider.CountNodes();
        }
    }
}