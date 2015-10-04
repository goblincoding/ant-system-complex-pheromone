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
        /// <summary>
        /// Debugging helper.
        /// </summary>
        private static int _counter = 0;

        private int _id = 0;

        public double TourLength { get; private set; } = 0.0;
        public List<int> Tour { get; } = new List<int>();   // the indices of the nodes belonging to the current tour.

        private int _startingNode;
        private int _currentNode;
        private readonly int[] _visited;     // the indices of the nodes the Ant has already visited.
        private readonly DataStructures _dataStructures;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStructures">Provides access to the underlying neighbour, pheromone density,
        /// heuristic and choice info matrices used in applying the random proportional rule.</param>
        /// <param name="startNode">The node the Ant is spawned on.</param>
        /// <param name="nrNodes">The nr of nodes in the TSP graph.</param>
        public Ant(DataStructures dataStructures, int startNode, int nrNodes)
        {
            _counter++;
            _id = _counter;

            _dataStructures = dataStructures;
            _startingNode = startNode;
            _currentNode = startNode;

            // Flag the current node as having been visited.
            _visited = new int[nrNodes];
            _visited[_currentNode] = 1;
        }

        public void MoveNext(object sender, EventArgs args)
        {
            var neighbours = _dataStructures.NearestNeighbours(_currentNode);
            var notVisited = (from n in neighbours
                              where _visited[n] != 1
                              select n).ToArray();

            // If we've visited all nodes, return to the starting node.
            var selectedNext = notVisited.Any() ? RouletteWheelSelector.MakeSelection(_dataStructures, notVisited, _currentNode) : _startingNode;
            TourLength += _dataStructures.Distance(_currentNode, selectedNext);
            Tour.Add(selectedNext);
            _visited[selectedNext] = 1;
            _currentNode = selectedNext;
        }
    }
}