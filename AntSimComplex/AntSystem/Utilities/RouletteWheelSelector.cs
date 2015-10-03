using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities
{
    public static class RouletteWheelSelector
    {
        /// <summary>
        /// Randomly selects the index of the next node based on the "roulette wheel
        /// selection" principle.
        /// </summary>
        /// <param name="neighbours">The indices of the neighbouring nodes.</param>
        /// <returns>The index of the next node to visit.</returns>
        public static int MakeSelection(DataStructures dataStructures, int[] neighbours, int currentNode)
        {
            var random = new Random();
            var selectedProbability = random.NextDouble();

            var probabilities = Probabilities(dataStructures, neighbours, currentNode);
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
        /// <param name="neighbours">An array of neighbouring node indices.</param>
        /// <returns>A list of KeyValuePairs sorted by key (probability) with value being the index
        /// of the node corresponding to the probability of selection represented by the key.</returns>
        private static List<KeyValuePair<double, int>> Probabilities(DataStructures dataStructures, int[] neighbours, int currentNode)
        {
            var probabilities = new double[neighbours.Length];
            var denominator = neighbours.Sum(n => dataStructures.ChoiceInfo(currentNode, n));

            for (int i = 0; i < neighbours.Length; i++)
            {
                var neighbour = neighbours[i];
                var numerator = dataStructures.ChoiceInfo(currentNode, neighbour);
                probabilities[i] = numerator / denominator;
            }

            // Select all the probability/index pairs (we need this so that we do not
            // lose the position of the index once we sort by probability).  In this case, "key" is
            // probability and "value" is the corresponding node index.
            var pairs = probabilities
                              .Select((prob, index) => new KeyValuePair<double, int>(prob, index))
                              .OrderBy(pair => pair.Key).ToList();

            return pairs;
        }
    }
}