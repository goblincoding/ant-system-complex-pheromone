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
    private readonly Random _random = new Random();
    private const double InitialPheromoneDensity = 0.1;
    private const double ArcWeight = 12.3;
    private const int Node1 = 0;
    private const int Node2 = 1;

    [Test]
    public void CtorShouldInitialiseDensitiesToInitialPheromone()
    {
      // arrange
      const int stepCount = 0;
      var smart = SmartPheromone();

      // act
      var density = smart.PresentedDensity(stepCount);

      // assert
      Assert.AreEqual(InitialPheromoneDensity, density);
    }

    [Test]
    public void DensityGivenInvalidAntIdShouldThrow()
    {
      // arrange
      const int stepCount = 0;
      var smart = SmartPheromone();

      // assert
      Assert.Throws<KeyNotFoundException>(() => smart.PresentedDensity(stepCount));
    }

    [Test]
    public void ResetShouldReturnAllDensitiesToInitialValue()
    {
      // arrange
      const int stepCount = 0;
      const double deposit = 0.56;
      var nodeId = _random.Next(0, 1);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);
      smart.Reset();

      // assert
      Assert.AreEqual(InitialPheromoneDensity, smart.PresentedDensity(stepCount));
    }

    [Test]
    public void EvaporateShouldReduceDensitiesWithCorrectAmount()
    {
      // arrange
      const int stepCount = 0;
      const double deposit = 0.00012;
      const double evaporationRate = 0.25;
      const double expected = (InitialPheromoneDensity + deposit) * (1.0 - evaporationRate);
      var nodeId = _random.Next(0, 1);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);
      smart.Evaporate(evaporationRate);

      // assert
      Assert.AreEqual(expected, smart.PresentedDensity(stepCount));
    }

    [Test]
    public void DepositShouldIncreaseDensitiesWithCorrectAmount()
    {
      // arrange
      const int stepCount = 0;
      const double deposit = 0.99999999;
      const double expected = InitialPheromoneDensity + deposit;
      var nodeId = _random.Next(0, 1);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);

      // assert
      Assert.AreEqual(expected, smart.PresentedDensity(stepCount));
    }

    [Test]
    public void TouchGivenAntWithCurrentNodeNotAtVertexShouldThrow()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(5);
      var smart = SmartPheromone();

      // assert
      Assert.Throws<ArgumentOutOfRangeException>(() => smart.Touch(ant));
    }

    [Test]
    public void TouchGivenAntWithCurrentNodeOnVertexShouldUpdateDensityCorrectly()
    {
      // arrange
      const int stepCount = 0;
      var nodeId = _random.Next(0, 1);

      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(Node1);

      var smart = SmartPheromone();
      var adjustment = 1.0 / (ant.TourLength + ArcWeight);
      var expected = 0.1 + adjustment;

      // act
      smart.Touch(ant);

      // assert
      Assert.AreEqual(expected, smart.PresentedDensity(stepCount));
    }

    private static SmartPheromone SmartPheromone()
    {
      var pheromone = new SmartPheromone(2, 4, 10, InitialPheromoneDensity, new ISmartPheromone[][] { });
      return pheromone;
    }
  }
}