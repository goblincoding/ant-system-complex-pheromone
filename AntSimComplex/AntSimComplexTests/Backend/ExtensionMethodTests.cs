using AntSimComplexAlgorithms.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  public class ExtensionMethodTests
  {
    [TestCase(4, 5, -1)]
    [TestCase(4, 2, 1)]
    [TestCase(1, 1, 0)]
    public void NodeWeightPairCompareToShouldCompareOnWeight(int weight1, int weight2, int expected)
    {
      // arrange
      var nodeProvider = new MockNodeProvider();
      var node1 = nodeProvider.GetNode(4);
      var node2 = nodeProvider.GetNode(8);

      var nodeWeightPair1 = new TspLibNetExtensionMethods.NodeWeightPair { Node = node1, Weight = weight1 };
      var nodeWeightPair2 = new TspLibNetExtensionMethods.NodeWeightPair { Node = node2, Weight = weight2 };

      // act
      var result = nodeWeightPair1.CompareTo(nodeWeightPair2);

      // assert
      Assert.AreEqual(result, expected);
    }

    [TestCase(4, new[] { 6, 7, 8 }, new[] { 1, 2, 3 })]
    [TestCase(3, new[] { 2, 3, 4, 5 }, new[] { 5, 0, 7, 8 })]
    [TestCase(9, new[] { 0, 1, 2, 5, 6, 7 }, new[] { 9, 1, 2, 5, 6, 7 })]
    public void EdgeWeightsProviderGetWeightsToNodesShouldReturnCorrectWeightsFromMockObjects(int fromNodeIndex,
                                                                                              int[] nodeIndices,
                                                                                              int[] expectedWeights)
    {
      // arrange
      var weightsProvider = new MockEdgeWeightsProvider();
      var nodeProvider = new MockNodeProvider();
      var fromNode = nodeProvider.GetNode(fromNodeIndex);
      var nodes = nodeIndices.Select(index => nodeProvider.GetNode(index)).ToList();

      // act
      var result = weightsProvider.GetWeightsToNodes(fromNode, nodes);

      // assert
      var nodeWeightPairs = result as TspLibNetExtensionMethods.NodeWeightPair[] ?? result.ToArray();
      Assert.AreEqual(nodeIndices.Length, nodeWeightPairs.Length);

      for (var i = 0; i < expectedWeights.Length; i++)
      {
        Assert.AreEqual(expectedWeights[i], nodeWeightPairs[i].Weight);
      }
    }

    [TestCase(4, new[] { 6, 7, 8 }, 1, 6)]
    [TestCase(3, new[] { 2, 3, 4, 5 }, 0, 3)]
    [TestCase(9, new[] { 0, 1, 2, 5, 6, 7 }, 1, 1)]
    public void EdgeWeightsProviderGetNearestNodeWeightShouldReturnCorrectNodeWeightFromMockObjects(int fromNodeIndex,
                                                                                                    int[] nodeIndices,
                                                                                                    int expectedWeight,
                                                                                                    int expectedNodeId)
    {
      // arrange
      var weightsProvider = new MockEdgeWeightsProvider();
      var nodeProvider = new MockNodeProvider();
      var fromNode = nodeProvider.GetNode(fromNodeIndex);
      var nodes = nodeIndices.Select(index => nodeProvider.GetNode(index)).ToList();

      // act
      var result = weightsProvider.GetNearestNodeWeight(fromNode, nodes);

      // assert
      Assert.AreEqual(expectedNodeId, result.Node.Id);
      Assert.AreEqual(expectedWeight, result.Weight);
    }

    // Node(weight)NextNearest
    // 7(1)3(2)8(1)2(2)9(1)1(3)0(4)4(1)6(2)5(3)7
    [Test]
    public void ProblemGetNearestNeighbourTourLengthGivenStartNode7ShouldReturn20()
    {
      // arrange
      var problem = new MockProblem();
      var random = Substitute.For<Random>();
      random.Next(0, 9).Returns(7); // force start node to be 7

      // act
      var result = problem.GetNearestNeighbourTourLength(random);

      // assert
      Assert.AreEqual(20, result);
    }
  }
}