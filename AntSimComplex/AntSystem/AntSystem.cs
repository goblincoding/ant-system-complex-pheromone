using AntSimComplexAlgorithms.Utilities;
using System;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAlgorithms
{
    /// <summary>
    /// This class is the entry point for the basic Ant System implementation.
    /// </summary>
    public class AntSystem
    {
        public event EventHandler MoveNext = delegate { };

        public event EventHandler Initialise = delegate { };

        public Ant[] Ants { get; }

        private readonly DataStructures _dataStructures;
        private readonly Parameters _parameters;
        private readonly Random _random;

        private readonly int _nodeCount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
        public AntSystem(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The AntSystem constructor needs a valid problem instance argument");
            }

            _random = new Random();
            _parameters = new Parameters(problem);
            _dataStructures = new DataStructures(problem, _parameters.InitialPheromone);
            _nodeCount = _dataStructures.NodeCount;

            Ants = new Ant[_nodeCount];
            ConstructAnts();
        }

        /// <summary>
        /// Completes one full iteration of the TSP graph, raising the "MoveNext"
        /// event the same number of times as there are nodes in the graph.
        /// </summary>
        public void Execute()
        {
            // Initialise the ants at random start nodes.
            foreach (var ant in Ants)
            {
                var startNode = _random.Next(0, _nodeCount);
                ant.Initialise(startNode);
            }

            // Construct solutions (iterate through nr of nodes since
            // each ant has to visit each node).
            for (int i = 0; i < _nodeCount; i++)
            {
                MoveNext(this, EventArgs.Empty);
            }

            // Update pheromone trails.
            Evaporate();
            foreach (var ant in Ants)
            {
                DepositPheromone(ant);
            }

            _dataStructures.UpdateChoiceInfoMatrix();
        }

        /// <summary>
        /// Evaporates all the pheromone on all the arcs by the evaporation factor <seealso cref="Parameters"/>.
        /// </summary>
        private void Evaporate()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                for (int j = i; j < _nodeCount; j++)
                {
                    // Matrix is symmetric.
                    var pher = _dataStructures.Pheromone[i][j] * Parameters.EvaporationRate;
                    _dataStructures.Pheromone[i][j] = pher;
                    _dataStructures.Pheromone[i][j] = pher;
                }
            }
        }

        private void DepositPheromone(Ant ant)
        {
            var deposit = 1 / ant.TourLength;
            for (int i = 0; i < _nodeCount; i++)
            {
                var j = ant.Tour[i];
                var l = ant.Tour[i + 1]; // stays within array bounds since Tour = n + 1 (returns to starting node)
                var pher = _dataStructures.Pheromone[i][j] + deposit;
                _dataStructures.Pheromone[j][l] = pher;  // matrix is symmetric
                _dataStructures.Pheromone[l][j] = pher;
            }
        }

        /// <summary>
        /// Initialise Ants and place one on each node of the TSP graph.
        /// </summary>
        private void ConstructAnts()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                var ant = new Ant(_dataStructures, _nodeCount);
                MoveNext += ant.MoveNext;
                Ants[i] = ant;
            }
        }
    }
}