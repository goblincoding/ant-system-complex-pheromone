using AntSimComplexAlgorithms.Utilities;
using System;
using System.Linq;
using TspLibNet;

namespace AntSimComplexAlgorithms
{
    /// <summary>
    /// This class is the entry point for the Ant System implementation.
    /// </summary>
    public class AntSystem
    {
        public event EventHandler MoveNext = delegate { };

        public event EventHandler Reset = delegate { };

        private readonly DataStructures _dataStructures;
        private readonly Parameters _parameters;
        private Ant[] _ants;

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

            _parameters = new Parameters(problem);
            _dataStructures = new DataStructures(problem, _parameters.InitialPheromone);

            ConstructAnts();
        }

        /// <summary>
        /// Initialise Ants and place one on each node of the TSP graph.
        /// </summary>
        private void ConstructAnts()
        {
            var random = new Random();
            var nodeCount = _dataStructures.OrderedNodeIndices.Count();
            _ants = new Ant[nodeCount];

            foreach (var index in _dataStructures.OrderedNodeIndices)
            {
                var startNode = random.Next(0, nodeCount);
                var ant = new Ant(_dataStructures, startNode, nodeCount);
                MoveNext += ant.MoveNext;
                Reset += ant.Reset;
                _ants[index] = ant;
            }
        }
    }
}