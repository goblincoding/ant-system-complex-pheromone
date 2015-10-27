using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities
{
    /// <summary>
    /// The probabilistic selection of the next node to be visited is done in accordance to the
    /// roulette wheel selection procedure: https://en.wikipedia.org/wiki/Fitness_proportionate_selection
    /// (ACO p107)
    /// </summary>
    public class RouletteWheelSelector
    {
        private Random _random = null;
        private DataStructures _dataStructures = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataStructures">The problem specific <seealso cref="DataStructures"/> object containing distance,
        /// pheromone, choice info, etc information.</param>
        /// <param name="random">The global random number generator object.</param>
        public RouletteWheelSelector(DataStructures dataStructures, Random random)
        {
            _dataStructures = dataStructures;
            _random = random;
        }

        /// <summary>
        /// Randomly selects the index of the next node based on the "roulette wheel
        /// selection" principle.
        /// </summary>
        /// <param name="notVisited">The indices of the neighbouring nodes that have not been visited.</param>
        /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
        /// <returns>The index of the next node to visit.</returns>
        public int MakeSelection(int[] notVisited, int currentNode)
        {
            var selectedProbability = _random.NextDouble();
            var probabilities = Probabilities(notVisited, currentNode);
            var probability = probabilities.First();
            var sum = probability.Key;
            for (int i = 1; i < probabilities.Count; i++)
            {
                if (sum >= selectedProbability)
                {
                    break;
                }

                probability = probabilities[i];
                sum += probability.Key;
            }

            return probability.Value;
        }

        /// <summary>
        /// Determines the probabilities of selection of the "neighbour" nodes based on the
        /// "random proportional rule" (ACO, Dorigo, 2004 p70).
        /// </summary>
        /// <param name="notVisited">An array of neighbouring node indices that have not been visited.</param>
        /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
        /// <returns>A list of KeyValuePairs sorted by key (probability) with value being the index
        /// of the node corresponding to the probability of selection represented by the key.</returns>
        private List<KeyValuePair<double, int>> Probabilities(int[] notVisited, int currentNode)
        {
            // Select all the probability/index pairs (we need this so that we do not
            // lose the position of the neighbour index once we sort by probability).
            // In this case, "key" is probability and "value" is the corresponding node index.
            var pairs = new List<KeyValuePair<double, int>>();
            var denominator = notVisited.Sum(n => _dataStructures.ChoiceInfo(currentNode, n));

            for (int i = 0; i < notVisited.Length; i++)
            {
                var neighbour = notVisited[i];
                var numerator = _dataStructures.ChoiceInfo(currentNode, neighbour);
                var probability = numerator / denominator;
                pairs.Add(new KeyValuePair<double, int>(probability, neighbour));
            }

            return pairs.OrderBy(pair => pair.Key).ToList();
        }
    }
}