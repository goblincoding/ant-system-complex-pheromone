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

        public event EventHandler Reset = delegate { };

        public Ant[] Ants { get; }

        private readonly DataStructures _dataStructures;
        private readonly Parameters _parameters;
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

            _parameters = new Parameters(problem);
            _dataStructures = new DataStructures(problem, _parameters.InitialPheromone);
            _nodeCount = _dataStructures.OrderedNodeIndices.Count();

            Ants = new Ant[_nodeCount];
            ConstructAnts();
        }

        /// <summary>
        /// Completes one full iteration of the TSP graph, raising the "MoveNext"
        /// event the same number of times as there are nodes in the graph.
        /// </summary>
        public void Execute()
        {
            Reset(this, EventArgs.Empty);
            for (int i = 0; i < _nodeCount; i++)
            {
                MoveNext(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initialise Ants and place one on each node of the TSP graph.
        /// </summary>
        private void ConstructAnts()
        {
            var random = new Random();
            foreach (var index in _dataStructures.OrderedNodeIndices)
            {
                var startNode = random.Next(0, _nodeCount);
                var ant = new Ant(_dataStructures, startNode, _nodeCount);
                MoveNext += ant.MoveNext;
                Reset += ant.Reset;
                Ants[index] = ant;
            }
        }
    }
}