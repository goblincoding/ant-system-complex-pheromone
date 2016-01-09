using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.EdgeWeights;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexAlgorithms.Utilities
{
  public static class TspLibNetExtensionMethods
  {
    /// <summary>
    /// Helper class representing a node and an edge weight to that node from
    /// an unspecified, other node.
    ///
    /// Compares on weight.
    ///
    /// Not intended to be used outside of the extension methods provided by
    /// TspLibNetExtensionMethods
    /// </summary>
    public class NodeWeightPair : IComparable<NodeWeightPair>, IComparable
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

    /// <summary>
    /// Returns a list of node/weight pairs representing the weights of the edges between
    /// "fromNode" and the nodes in the list of KeyValuePairs
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="fromNode">The source node</param>
    /// <param name="nodes">A list of all the nodes we want weights for from the source node</param>
    /// <returns></returns>
    public static IEnumerable<NodeWeightPair> GetWeightsToNodes(this IEdgeWeightsProvider provider,
                                                                INode fromNode,
                                                                IEnumerable<INode> nodes)
    {
      return from n in nodes
             let w = provider.GetWeight(fromNode, n)
             select new NodeWeightPair { Node = n, Weight = w };
    }

    public static NodeWeightPair GetNearestNodeWeight(this IEdgeWeightsProvider provider,
                                                      INode fromNode,
                                                      IEnumerable<INode> nodes)
    {
      var weightList = provider.GetWeightsToNodes(fromNode, nodes);
      return weightList.Min();
    }

    /// <summary>
    /// Calculates the pheromone initialisation value based on the nearest neighbour heuristic (ACO Dorigo Ch3, p70).
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
        var nearest = weightsProvider.GetNearestNodeWeight(current, notVisited);
        current = nearest.Node;
        tourLength += nearest.Weight;
        notVisited.Remove(current);
      }

      // Return to the first node.
      tourLength += weightsProvider.GetWeight(current, first);
      return tourLength;
    }
  }
}