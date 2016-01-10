namespace AntSimComplexAlgorithms.Utilities.RouletteWheelSelector
{
  internal interface IRouletteWheelSelector
  {
    /// <summary>
    /// Randomly selects the index of the next node based on the "roulette wheel
    /// selection" principle.
    /// </summary>
    /// <param name="notVisited">The indices of the neighbouring nodes that have not been visited.</param>
    /// <param name="currentNode"> The index of the node whose neighbours are being assessed.</param>
    /// <returns>The index of the next node to visit.</returns>
    int SelectNextNode(int[] notVisited, int currentNode);
  }
}