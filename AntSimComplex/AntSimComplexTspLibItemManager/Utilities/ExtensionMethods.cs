using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.EdgeWeights;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTspLibItemManager.Utilities
{
  internal static class TspLibNetExtensionMethods
  {
    /// <summary>
    /// Returns the tour length constructed by the nearest neighbour heuristic (ACO Dorigo Ch3, p70).
    /// Pseudo code:
    /// 1. Select a random city.
    /// 2. Find the nearest unvisited city and go there.
    /// 3. Are there any unvisitied cities left? If yes, repeat step 2.
    /// 4. Return to the first city.
    /// </summary>
    /// <param name="problem"></param>
    /// <param name="random">A random number generator</param>
    public static double GetNearestNeighbourTourLength(this IProblem problem, Random random)
    {
      var notVisited = problem.NodeProvider.GetNodes().ToList();
      var weightsProvider = problem.EdgeWeightsProvider;
      var tourLength = 0.0;

      // Select a random node.
      var current = notVisited.ElementAt(random.Next(0, notVisited.Count));
      var first = current;  // have to return here eventually
      notVisited.Remove(current);

      while (notVisited.Any())
      {
        // Calculate the weights (distances) from the current selected
        // node to the remaining, unvisited nodes and determine the nearest.
        var nearest = GetNearestNodeWeight(weightsProvider, current, notVisited);
        current = nearest.Node;
        tourLength += nearest.Weight;
        notVisited.Remove(current);
      }

      // Return to the first node.
      tourLength += weightsProvider.GetWeight(current, first);
      return tourLength;
    }

    /// <summary>
    /// Helper class representing a node and an edge weight to that node from
    /// an unspecified, other node.
    ///
    /// Compares on weight.
    ///
    ///</summary>
    private class NodeWeightPair : IComparable<NodeWeightPair>, IComparable
    {
      public INode Node { get; set; }
      public double Weight { get; set; }

      public int CompareTo(NodeWeightPair other)
      {
        if (other == null)
        {
          throw new ArgumentNullException(nameof(other));
        }

        return Weight.CompareTo(other.Weight);
      }

      public int CompareTo(object obj)
      {
        if (obj == null)
        {
          return 1;
        }

        var other = obj as NodeWeightPair;
        if (other == null)
        {
          throw new ArgumentException("Object is not of type 'NodeWeightPair'");
        }

        return Weight.CompareTo(other.Weight);
      }
    }

    private static NodeWeightPair GetNearestNodeWeight(IEdgeWeightsProvider provider,
                                                      INode fromNode,
                                                      IEnumerable<INode> nodes)
    {
      var weightList = from n in nodes
                       let w = provider.GetWeight(fromNode, n)
                       select new NodeWeightPair { Node = n, Weight = w };
      return weightList.Min();
    }
  }
}