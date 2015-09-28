using AntSimComplexAlgorithms.Utilities;
using System;
using System.Linq;

namespace AntSimComplexAlgorithms
{
    public class Ant
    {
        private double _tourLength;
        private int _currentNode;
        private int[] _visited;
        private int[] _tour;

        private readonly DataStructures _dataStructures;

        public Ant(DataStructures dataStructures, int startNode, int nrNodes)
        {
            _dataStructures = dataStructures;
            _currentNode = startNode;

            // +1 since the ant has to return to the initial node.
            _tour = new int[nrNodes + 1];

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

            //var probabilities = Probabilities(notVisited);
        }

        public void Reset(object sender, EventArgs args)
        {
            // Do nothing with current node, we can start with whichever node
            // we are on (that should be random enough).
            _tourLength = 0.0;
            _tour.Initialize();
            _visited.Initialize();
        }

        /// <summary>
        /// Determines the probabilities of selection of the "neighbour" nodes based on the
        /// "random proportional rule" (ACO, Dorigo, 2004 p70).
        /// </summary>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        private double[] Probabilities(int[] neighbours)
        {
            var probabilities = new double[neighbours.Length];
            var denominator = neighbours.Sum(n => _dataStructures.ChoiceInfo(_currentNode, n));
            foreach (var neighbour in neighbours)
            {
                var numerator = _dataStructures.ChoiceInfo(_currentNode, neighbour);
                probabilities[neighbour] = numerator / denominator;
            }

            return probabilities;
        }
    }
}