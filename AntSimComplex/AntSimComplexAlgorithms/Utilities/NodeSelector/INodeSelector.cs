namespace AntSimComplexAlgorithms.Utilities.NodeSelector
{
  internal interface INodeSelector
  {
    /// <summary>
    /// Selects the next (unvisited) node the ant should visit.
    /// </summary>
    /// <param name="ant">The ant that is contemplating the next node to step to.</param>
    /// <returns>The index of the next node to visit.</returns>
    int SelectNextNode(IAnt ant);
  }
}