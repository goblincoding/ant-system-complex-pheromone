using System.Collections.Generic;

namespace AntSimComplexAlgorithms
{
  internal interface IAnt
  {
    /// <summary>
    /// The ant's unique integer Id.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The index of the node the ant is currently on.
    /// </summary>
    int CurrentNode { get; }

    /// <summary>
    /// Length of the ant's completed tour.
    /// </summary>
    double TourLength { get; }

    /// <summary>
    /// The node indices corresponding to the ant's tour.
    /// </summary>
    IReadOnlyList<int> Tour { get; }

    /// <summary>
    /// Indices of visited nodes are set to "true", e.g. node x
    /// has not been visited if Visited[x] is "false".
    /// </summary>
    IReadOnlyList<bool> Visited { get; }

    /// <summary>
    /// Initialises (or resets) the internal state of the Ant.
    /// </summary>
    /// <param name="startNode">The node the ant starts its tour on.</param>
    void Initialise(int startNode);

    /// <summary>
    /// Move to the next node selected by the current node selection strategy.
    /// </summary>
    /// <param name="i">The current step</param>
    void Step(int i);
  }
}