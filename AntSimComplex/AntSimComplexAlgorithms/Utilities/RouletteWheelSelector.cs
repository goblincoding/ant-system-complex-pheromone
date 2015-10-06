using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities
{
    public static class RouletteWheelSelector
    {
        /// <summary>
        /// Use a single, static random variable so that we do not end up with roughly
        /// the same number generation sequences with fast clock cycles.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Randomly selects the index of the next node based on the "roulette wheel
        /// selection" principle.
        /// </summary>
        /// <param name="neighbours">The indices of the neighbouring nodes.</param>
        /// <param name="referenceNode"> The index of the node whose neighbours are being assessed.</param>
        /// <returns>The index of the next node to visit.</returns>
        public static int MakeSelection(DataStructures dataStructures, int[] neighbours, int referenceNode)
        {
            var selectedProbability = random.NextDouble();
            var probabilities = Probabilities(dataStructures, neighbours, referenceNode);
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
        /// <param name="referenceNode"> The index of the node whose neighbours are being assessed.</param>
        /// <returns>A list of KeyValuePairs sorted by key (probability) with value being the index
        /// of the node corresponding to the probability of selection represented by the key.</returns>
        private static List<KeyValuePair<double, int>> Probabilities(DataStructures dataStructures, int[] neighbours, int referenceNode)
        {
            // Select all the probability/index pairs (we need this so that we do not
            // lose the position of the neighbour index once we sort by probability).
            // In this case, "key" is probability and "value" is the corresponding node index.
            var pairs = new List<KeyValuePair<double, int>>();
            var denominator = neighbours.Sum(n => dataStructures.ChoiceInfo(referenceNode, n));

            for (int i = 0; i < neighbours.Length; i++)
            {
                var neighbour = neighbours[i];
                var numerator = dataStructures.ChoiceInfo(referenceNode, neighbour);
                var probability = numerator / denominator;
                pairs.Add(new KeyValuePair<double, int>(probability, neighbour));
            }

            return pairs.OrderBy(pair => pair.Key).ToList();
        }
    }
}