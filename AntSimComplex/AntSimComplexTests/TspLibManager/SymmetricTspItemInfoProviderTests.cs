using AntSimComplexTests.GUI;
using AntSimComplexTspLibItemManager.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;
using TspLibNet.Tours;

namespace AntSimComplexTests.TspLibManager
{
  [TestFixture]
  public class SymmetricTspItemInfoProviderTests
  {
    [Test]
    public void CtorNullTspLibItemShouldThrowArgumentNullException()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => new SymmetricTspItemInfoProvider(null));
    }

    [TestCase("bayg29")]
    [TestCase("gr21")]
    public void CtorGivenItemThatDoesNotHave2DNodesShouldThrowArgumentOutOfRangeException(string tspProblemName)
    {
      // arrange
      var tspLib = new TspLib95(Helpers.LibPath);
      tspLib.LoadTSP(tspProblemName);
      var items = tspLib.TSPItems();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SymmetricTspItemInfoProvider(items.First()));
    }

    [Test]
    public void ProblemNameGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly()
    {
      // arrange
      var infoProvider = CreateInfoProviderFromMockProblem();

      // assert
      Assert.AreEqual("MockProblem", infoProvider.ProblemName);
    }

    [Test]
    public void NodeCountGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly()
    {
      // arrange
      var infoProvider = CreateInfoProviderFromMockProblem();

      // assert
      Assert.AreEqual(10, infoProvider.NodeCount);
    }

    [Test]
    public void MinMaxCoordinatesGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly()
    {
      // arrange
      const int minX = 1;
      const int maxX = 10;
      const int minY = 0;
      const int maxY = 8;

      var infoProvider = CreateInfoProviderFromMockProblem();

      // assert
      Assert.AreEqual(minX, infoProvider.MinXCoordinate);
      Assert.AreEqual(minY, infoProvider.MinYCoordinate);
      Assert.AreEqual(maxX, infoProvider.MaxXCoordinate);
      Assert.AreEqual(maxY, infoProvider.MaxYCoordinate);
    }

    [Test]
    public void TspNodesGivenTspLib95ItemFromMockProblemShouldReturnListofTspNodesOrderedById()
    {
      // arrange
      var problem = new MockProblem();
      var infoProvider = CreateInfoProviderFromMockProblem();
      var expected = problem.NodeProvider.GetNodes()
                                         .OfType<Node2D>()
                                         .Select(n => new TspNode(n.Id, n.X, n.Y))
                                         .OrderBy(n => n.Id)
                                         .ToList();

      // assert
      CollectionAssert.AreEqual(expected, infoProvider.TspNodes);
    }

    [Test]
    public void OptimalTourPropertiesGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly()
    {
      // arrange
      const int optimalTourLength = 34534;
      var optimalTourNodes = new List<TspNode>
      {
        new TspNode(1,4,5),
        new TspNode(2,6,6),
        new TspNode(3,7,2)
      };

      var infoProvider = CreateInfoProviderFromMockProblem();

      // assert
      Assert.AreEqual(true, infoProvider.HasOptimalTour);
      Assert.AreEqual(optimalTourLength, infoProvider.OptimalTourLength);
      Assert.That(optimalTourNodes, Is.EqualTo(infoProvider.OptimalTour));
    }

    [Test]
    public void NearestNeighbourTourLengthGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly()
    {
      // arrange
      var random = Substitute.For<Random>();
      random.Next(0, 9).Returns(7); // force start node to be 7

      var problem = new MockProblem();
      var expected = problem.GetNearestNeighbourTourLength(random);

      var infoProvider = CreateInfoProviderFromMockProblem();

      // act
      var result = infoProvider.NearestNeighbourTourLength;

      // assert
      Assert.AreEqual(expected, result);
    }

    [TestCase(1, 2, 3)]
    [TestCase(2, 1, 3)]
    [TestCase(4, 7, 2)]
    public void DistancesGivenTspLib95ItemFromMockProblemShouldBeSetCorrectly(int index1, int index2, int expectedWeight)
    {
      // arrange
      var infoProvider = CreateInfoProviderFromMockProblem();

      // act
      var result = infoProvider.Distances[index1][index2];

      // assert
      Assert.AreEqual(expectedWeight, result);
    }

    private static SymmetricTspItemInfoProvider CreateInfoProviderFromMockProblem()
    {
      var problem = new MockProblem();

      const int optimalTourLength = 34534;
      var optimalTourNodes = new List<int> { 1, 2, 3 };
      var optimalTour = Substitute.For<ITour>();
      optimalTour.Nodes.Returns(optimalTourNodes);

      var tspLibItem = new TspLib95Item(problem, optimalTour, optimalTourLength);
      return new SymmetricTspItemInfoProvider(tspLibItem);
    }
  }
}