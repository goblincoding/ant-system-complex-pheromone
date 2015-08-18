using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSystem
{
    /// <summary>
    /// Ant Colony Optimisation - Dorigo and Stutzle, p71, Box 3.1
    /// Good settings for ACO without local search applied.
    /// These parameters are for the random proportional rule (probability
    /// of selection formula given on p70 of the same book).
    /// </summary>
    public class Parameters
    {
        private readonly IProblem _tspProblem;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problem">A TSPLib.Net problem instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Parameters(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The Parameters constructor needs a valid problem instance argument");
            }

            _tspProblem = problem;
            TauZero = CalculateTauZero();
        }

        /// <summary>
        /// Determines the relative influence of the pheromone trail in the random proportional rule.
        /// </summary>
        public const int Alpha = 1;

        /// <summary>
        /// Good values are 2 - 5, determines the relative influence of the heuristic information
        /// in the random proportional rule.
        /// </summary>
        public const double Beta = 2.0;

        /// <summary>
        /// The pheromone evaporation rate for the pheromone update cycle.
        /// </summary>
        public const double Rho = 0.5;

        public double TauZero { get; } = 0.1;

        /// <summary>
        /// Calculates the pheromone initialisation value based on the nearest neighbour heuristic.
        /// Pseudo code:
        /// 1. Select a random city.
        /// 2. Find the nearest unvisited city and go there.
        /// 3. Are there any unvisitied cities left? If yes, repeat step 2.
        /// 4. Return to the first city.
        /// </summary>
        private double CalculateTauZero()
        {
            // Select a random node.
            var random = new Random();
            var nodes = _tspProblem.NodeProvider.GetNodes().ToList();
            var weightsProvider = _tspProblem.EdgeWeightsProvider;

            var selected = nodes.ElementAt(random.Next(1, nodes.Count()));
            var visited = new List<INode>() { selected };

            var tourLength = 0.0;

            // Any unvisited nodes left?
            var notVisited = nodes.Except(visited);
            while (notVisited.Any())
            {
                var weightList = from n in notVisited
                                 let w = weightsProvider.GetWeight(selected, n)
                                 where n != selected
                                 select new { Nearest = n, Weight = w };

                var tuple = weightList.First(i => i.Weight.Equals(weightList.Min(t => t.Weight)));
                visited.Add(tuple.Nearest);
                tourLength += tuple.Weight;

                // Leave this here for performance test.
                //var nearest = notVisited.First();
                //var currentMin = weightsProvider.GetWeight(selected, nearest);
                //foreach (var node in notVisited)
                //{
                //    var weight = weightsProvider.GetWeight(selected, node);
                //    if (weight < currentMin)
                //    {
                //        currentMin = weight;
                //        nearest = node;
                //    }
                //}

                //visited.Add(nearest);

                notVisited = nodes.Except(visited);
            }

            return tourLength;
        }
    }
}