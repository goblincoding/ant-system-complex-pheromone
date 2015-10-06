using AntSimComplexAlgorithms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms
{
    /// <summary>
    /// Ants are implemented predominantly as per "Ant Colony Optimisation" Dorigo and Stutzle (2004), Ch3.8, p103.
    /// </summary>
    public class Ant
    {
        public double TourLength { get; private set; } = 0.0;
        public List<int> Tour { get; } = new List<int>();   // the indices of the nodes belonging to the current tour.

        private int _startNode;
        private int _currentNode;

        private readonly int[] _visited;     // the indices of the nodes the Ant has already visited.
        private readonly DataStructures _dataStructures;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStructures">Provides access to the underlying neighbour, pheromone density,
        /// heuristic and choice info matrices used in applying the random proportional rule.</param>
        /// <param name="nrNodes">The nr of nodes in the TSP graph.</param>
        public Ant(DataStructures dataStructures, int nrNodes)
        {
            _dataStructures = dataStructures;
            _visited = new int[nrNodes];
        }

        /// <summary>
        /// Initialises the internal state of the Ant.  Discards constructed tour information (if it exists).
        /// </summary>
        public void Initialise(int startNode)
        {
            _startNode = startNode;
            _currentNode = _startNode;

            for (int i = 0; i < _visited.Length; i++)
            {
                _visited[i] = 0;
            }
            _visited[_currentNode] = 1;

            TourLength = 0.0;
            Tour.Clear();
        }

        /// <summary>
        /// Moves the ant to the next node selected according to the random proportional rule,
        /// updating tour information in the process.
        /// </summary>
        public void MoveNext(object sender, EventArgs args)
        {
            // Find the neighbours we haven't visited yet.
            var neighbours = _dataStructures.NearestNeighbours(_currentNode);
            var notVisited = (from n in neighbours
                              where _visited[n] != 1
                              select n).ToArray();

            // If we've visited all nodes, return to the starting node.
            var selectedNext = notVisited.Any() ?
                                    RouletteWheelSelector.MakeSelection(_dataStructures, notVisited, _currentNode) : _startNode;

            // Update tour information and move to the next node.
            TourLength += _dataStructures.Distance(_currentNode, selectedNext);
            Tour.Add(selectedNext);
            _visited[selectedNext] = 1;
            _currentNode = selectedNext;
        }
    }
}