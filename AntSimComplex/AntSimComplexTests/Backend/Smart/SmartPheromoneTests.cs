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
    private const int NumberOfAnts = 10;
    private const int Node1 = 0;
    private const int Node2 = 1;

    [Test]
    public void CtorShouldInitialiseDensitiesToInitialPheromone()
    {
      // arrange
      var antId = _random.Next(0, NumberOfAnts);
      var smart = SmartPheromone();

      // act
      var density = smart.Density(antId);

      // assert
      Assert.AreEqual(InitialPheromoneDensity, density);
    }

    [Test]
    public void DensityGivenInvalidAntIdShouldThrow()
    {
      // arrange
      var smart = SmartPheromone();

      // assert
      Assert.Throws<KeyNotFoundException>(() => smart.Density(NumberOfAnts));
    }

    [Test]
    public void ResetShouldReturnAllDensitiesToInitialValue()
    {
      // arrange
      const double deposit = 0.56;
      var antId = _random.Next(0, NumberOfAnts);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);
      smart.Reset();

      // assert
      Assert.AreEqual(InitialPheromoneDensity, smart.Density(antId));
    }

    [Test]
    public void EvaporateShouldReduceDensitiesWithCorrectAmount()
    {
      // arrange
      const double deposit = 0.00012;
      const double evaporationRate = 0.25;
      const double expected = (InitialPheromoneDensity + deposit) * (1.0 - evaporationRate);
      var antId = _random.Next(0, NumberOfAnts);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);
      smart.Evaporate(evaporationRate);

      // assert
      Assert.AreEqual(expected, smart.Density(antId));
    }

    [Test]
    public void DepositShouldIncreaseDensitiesWithCorrectAmount()
    {
      // arrange
      const double deposit = 0.99999999;
      const double expected = InitialPheromoneDensity + deposit;
      var antId = _random.Next(0, NumberOfAnts);
      var smart = SmartPheromone();

      // act
      smart.Deposit(deposit);

      // assert
      Assert.AreEqual(expected, smart.Density(antId));
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
    public void TouchGivenUnknownAntShouldThrow()
    {
      // arrange
      var ant = Substitute.For<IAnt>();
      ant.Id.Returns(NumberOfAnts);
      var smart = SmartPheromone();

      // assert
      Assert.Throws<KeyNotFoundException>(() => smart.Touch(ant));
    }

    [Test]
    public void TouchGivenAntWithCurrentNodeOnVertexShouldUpdateDensityCorrectly()
    {
      // arrange
      var antId = _random.Next(0, NumberOfAnts);

      var ant = Substitute.For<IAnt>();
      ant.CurrentNode.Returns(Node1);
      ant.Id.Returns(antId);

      var smart = SmartPheromone();
      var adjustment = 1.0 / (ant.TourLength / ArcWeight);
      var expected = 0.1 * adjustment;

      // act
      smart.Touch(ant);

      // assert
      Assert.AreEqual(expected, smart.Density(ant.Id));
    }

    private static SmartPheromone SmartPheromone()
    {
      var pheromone = new SmartPheromone(Node1, Node2, ArcWeight, NumberOfAnts, InitialPheromoneDensity);
      return pheromone;
    }
  }
}