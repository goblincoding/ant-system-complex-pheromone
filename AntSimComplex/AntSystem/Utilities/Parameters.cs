﻿using System;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAS.Utilities
{
    /// <summary>
    /// Ant Colony Optimisation - Dorigo and Stutzle, p71, Box 3.1
    /// "Good settings for ACO without local search applied."
    /// These parameters are for the random proportional rule (probability
    /// of selection formula given on p70 of the same book).
    /// </summary>
    public class Parameters
    {
        /// <param name="problem">A TSPLib.Net problem instance</param>
        /// <exception cref="ArgumentNullException">Thrown if an null problem instance was provided.</exception>
        public Parameters(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The Parameters constructor needs a valid problem instance argument");
            }

            NumberOfAnts = problem.NodeProvider.CountNodes();
            InitialPheromone = NumberOfAnts / GetNearestNeighbourTourLength(problem);
        }

        /// <summary>
        /// Determines the relative influence of the pheromone trail in the random proportional rule.
        /// </summary>
        public int Alpha { get; } = 1;

        /// <summary>
        /// Good values are 2 - 5, determines the relative influence of the heuristic information
        /// in the random proportional rule.
        /// </summary>
        public double Beta { get; } = 2.0;

        /// <summary>
        /// The pheromone evaporation rate for the pheromone update cycle (rho).
        /// </summary>
        public double EvaporationRate { get; } = 0.5;

        /// <summary>
        /// The pheromone density initialisation value (tau zero).
        /// </summary>
        public double InitialPheromone { get; } = 0.1;

        /// <summary>
        /// The number of artificial ants initialised for the problem.  For Ant System
        /// this number will always be equal to the number of nodes in the TSP.
        /// </summary>
        public int NumberOfAnts { get; } = 0;

        /// <summary>
        /// Calculates the pheromone initialisation value based on the nearest neighbour heuristic (ACO Dorigo Ch3, p70).
        /// Pseudo code:
        /// 1. Select a random city.
        /// 2. Find the nearest unvisited city and go there.
        /// 3. Are there any unvisitied cities left? If yes, repeat step 2.
        /// 4. Return to the first city.
        /// </summary>
        private double GetNearestNeighbourTourLength(IProblem problem)
        {
            var notVisited = problem.NodeProvider.GetNodes().ToList();
            var weightsProvider = problem.EdgeWeightsProvider;
            var tourLength = 0.0;

            // Select a random node.
            var random = new Random();
            var current = notVisited.ElementAt(random.Next(0, notVisited.Count()));
            var first = current;  // have to return here eventually
            notVisited.Remove(current);

            while (notVisited.Any())
            {
                // Calculate the weights (distances) from the current selected
                // node to the remaining, unvisited nodes.
                var weightList = from n in notVisited
                                 let w = weightsProvider.GetWeight(current, n)
                                 select new { NearestNode = n, Weight = w };

                var minWeight = weightList.Min(t => t.Weight);
                var tuple = weightList.First(t => t.Weight.Equals(minWeight));
                current = tuple.NearestNode;
                tourLength += tuple.Weight;
                notVisited.Remove(current);
            }

            // Return to the first node.
            tourLength += weightsProvider.GetWeight(current, first);
            return tourLength;
        }
    }
}