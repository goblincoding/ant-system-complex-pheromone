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
        private double _tourLength;
        private int _currentNode;

        private readonly int[] _visited;     // the indices of the nodes the Ant has already visited.
        private readonly List<int> _tour;    // the indices of the nodes belonging to the current tour.

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
            _dataStructures = dataStructures;
            _currentNode = startNode;
            _tour = new List<int>();

            // Flag the current node as having been visited.
            _visited = new int[nrNodes];
            _visited[_currentNode] = 1;
        }

        public void MoveNext(object sender, EventArgs args)
        {
            var neighbours = _dataStructures.NearestNeighbours(_currentNode);
            var notVisited = from n in neighbours
                             where _visited[n] != 1
                             select n;

            var selectedNext = RouletteWheelSelector.MakeSelection(_dataStructures, notVisited.ToArray(), _currentNode);
            _visited[selectedNext] = 1;
            _currentNode = selectedNext;
            _tour.Add(_currentNode);
        }

        public void Reset(object sender, EventArgs args)
        {
            // Do nothing with current node, we can start with whichever node
            // we are on (that should be random enough).
            _tourLength = 0.0;
            _tour.Clear();
            _visited.Initialize();
        }
    }
}