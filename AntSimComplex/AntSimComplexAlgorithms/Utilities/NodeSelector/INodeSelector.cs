using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  internal interface INodeSelector
  {
    /// <param name="notVisited">The indices of the neighbouring nodes (sorted by distance ascending) that
    /// have not been visited.</param>
    /// <param name="currentNode">The index of the node whose neighbours are being assessed.</param>
    /// <returns>The index of the next node to visit.</returns>
    int SelectNextNode(IReadOnlyList<int> notVisited, int currentNode);
  }
}