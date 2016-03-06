using AntSimComplexAlgorithms.Ants;
using AntSimComplexAlgorithms.Smart;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AntSimComplexTests.Backend.Smart
{
  [TestFixture]
  internal class SmartPheromoneTests
  {
    [Test]
    public void CtorShouldInitialiseDensitiesToInitialPheromone()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(0);
      var pheromone = SmartPheromone();

      // act
      var density = pheromone.Density(ant.Id);

      // assert
      Assert.AreEqual(0.1, density);
    }

    [Test]
    public void TouchGivenAntWithCurrentNodeNotAVertexShouldThrow()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(5);
      var pheromone = SmartPheromone();

      // assert
      Assert.Throws<ArgumentOutOfRangeException>(() => pheromone.Touch(ant));
    }

    [Test]
    public void TouchGivenUnknownAntShouldThrow()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(5);
      var pheromone = SmartPheromone();

      // assert
      Assert.Throws<KeyNotFoundException>(() => pheromone.Touch(ant));
    }

    [Test]
    public void TouchGivenAntWithCurrentNodeOnVertexShouldUpdateDensityCorrectly()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(0);
      var pheromone = SmartPheromone();
      var adjustment = 1 / (ant.TourLength + 15);
      var expected = 0.1 * adjustment;

      // act
      pheromone.Touch(ant);

      // assert
      Assert.AreEqual(expected, pheromone.Density(ant.Id));
    }

    [Test]
    public void DensityGivenInvalidAntIdShouldThrow()
    {
      // arrange
      var pheromone = SmartPheromone();

      // assert
      Assert.Throws<KeyNotFoundException>(() => pheromone.Density(7));
    }

    private static SmartPheromone SmartPheromone()
    {
      var ant1 = Substitute.For<IAnt>();
      ant1.Id.Returns(0);
      ant1.CurrentNode.Returns(1);
      ant1.TourLength.Returns(10);

      var ant2 = Substitute.For<IAnt>();
      ant2.Id.Returns(1);
      ant2.CurrentNode.Returns(0);
      ant2.TourLength.Returns(5);

      var ant3 = Substitute.For<IAnt>();
      ant3.Id.Returns(2);
      ant3.CurrentNode.Returns(2);
      ant3.TourLength.Returns(15);

      var ants = new List<IAnt> { ant1, ant2, ant3 };
      var pheromone = new SmartPheromone(0, 1, 15, 0.1, ants);
      return pheromone;
    }
  }
}