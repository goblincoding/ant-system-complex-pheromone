﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;
using TspLibNet.Tours;

namespace AntSystem
{
    /// <summary>
    /// Ant Colony Optimisation - Dorigo and Stutzle, p71, Box 3.1
    /// "Good settings for ACO without local search applied."
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
        /// <exception cref="ArgumentNullException">Thrown if an null problem instance was provided.</exception>
        public Parameters(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The Parameters constructor needs a valid problem instance argument");
            }

            _tspProblem = problem;
            NumberOfAnts = _tspProblem.NodeProvider.GetNodes().Count();
            InitialPheromone = NumberOfAnts / GetNearestNeighbourTourLength();
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
        /// The pheromone evaporation rate for the pheromone update cycle (rho).
        /// </summary>
        public const double EvaporationRate = 0.5;

        /// <summary>
        /// The pheromone density initialisation value (tau zero).
        /// </summary>
        public double InitialPheromone { get; } = 0.1;

        /// <summary>
        /// The number of artificial ants initialised for the problem.  For Ant System
        /// this number will always be equal to the number of nodes in the TSP.
        /// </summary>
        public int NumberOfAnts { get; set; } = 0;

        public List<Node2D> NearestNeighbourTour { get; private set; } = new List<Node2D>();

        /// <summary>
        /// Calculates the pheromone initialisation value based on the nearest neighbour heuristic.
        /// Pseudo code:
        /// 1. Select a random city.
        /// 2. Find the nearest unvisited city and go there.
        /// 3. Are there any unvisitied cities left? If yes, repeat step 2.
        /// 4. Return to the first city.
        /// </summary>
        private double GetNearestNeighbourTourLength()
        {
            var notVisited = _tspProblem.NodeProvider.GetNodes().ToList();
            var weightsProvider = _tspProblem.EdgeWeightsProvider;
            var tourLength = 0.0;

            // Select a random node.
            var random = new Random();
            var current = notVisited.First(n => n.Id == 5); //notVisited.ElementAt(random.Next(0, notVisited.Count()));
            notVisited.Remove(current);
            NearestNeighbourTour.Add(current as Node2D);

            // Any unvisited nodes left?
            while (notVisited.Any())
            {
                Debug.WriteLine($"Current node: {current.Id}");

                // Calculate the weights (distances) from the current selected
                // node to the remaining, unvisited nodes.
                var weightList = from n in notVisited
                                 let w = weightsProvider.GetWeight(current, n)
                                 select new { NearestNode = n, Weight = w };

                var print = weightList.ToList();
                var minWeight = weightList.Min(t => t.Weight);
                var tuple = weightList.First(t => t.Weight.Equals(minWeight));
                current = tuple.NearestNode;
                Debug.WriteLine($"Distance to nearest: {tuple.Weight}");
                tourLength += tuple.Weight;
                NearestNeighbourTour.Add(current as Node2D);
                notVisited.Remove(current);
            }

            var tourIds = new List<int>() { 5, 15, 14, 13, 12, 7, 6, 10, 9, 16, 1, 8, 4, 2, 3 };
            var tour = new Tour(_tspProblem.Name, _tspProblem.Comment, tourIds.Count(), tourIds);
            var tourDistance = _tspProblem.TourDistance(tour);
            Debug.WriteLine($"Tour length from problem: {tourDistance}");

            Debug.WriteLine($"Tour length: {tourLength}");
            return tourLength;
        }
    }
}