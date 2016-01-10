using AntSimComplexAlgorithms;
using AntSimComplexAlgorithms.ProblemContext;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  internal class AntTests
  {
    [Test]
    public void MoveNextShouldBuildAccurateTourLength()
    {
      // arrange
      var context = Substitute.For<IProblemContext>();
      context.NodeCount.Returns(10);

      context.NearestNeighbours(7).Returns(new[] { 3, 8, 2 });
      context.SelectNextNode(Arg.Any<int[]>(), 7).Returns(3);
      context.Distance(7, 3).Returns(1);

      context.NearestNeighbours(3).Returns(new[] { 7, 8, 2 });
      context.SelectNextNode(Arg.Any<int[]>(), 3).Returns(8);
      context.Distance(3, 8).Returns(2);

      context.NearestNeighbours(8).Returns(new[] { 7, 3, 2 });
      context.SelectNextNode(Arg.Any<int[]>(), 8).Returns(2);
      context.Distance(8, 2).Returns(5);

      var ant = new Ant(context);
      var expectedTour = new List<int> { 7, 3, 8, 2, 7 };

      // act
      ant.Initialise(7);
      ant.MoveNext(new object(), new EventArgs());
      ant.MoveNext(new object(), new EventArgs());
      ant.MoveNext(new object(), new EventArgs());
      ant.MoveNext(new object(), new EventArgs());

      // assert
      Assert.AreEqual(8, ant.TourLength);
      Assert.AreEqual(expectedTour, ant.Tour);
    }
  }
}