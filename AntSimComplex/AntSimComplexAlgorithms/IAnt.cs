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
    /// Length of the ant's completed tour.
    /// </summary>
    double TourLength { get; }

    /// <summary>
    /// The node indices corresponding to the ant's tour.
    /// </summary>
    List<int> Tour { get; }

    /// <summary>
    /// Initialises (or resets) the internal state of the Ant.
    /// </summary>
    /// <param name="startNode">The node the ant starts its tour on.</param>
    void Initialise(int startNode);

    /// <summary>
    /// Move to the next node selected by the current node selection strategy.
    /// </summary>
    void Step();
  }
}