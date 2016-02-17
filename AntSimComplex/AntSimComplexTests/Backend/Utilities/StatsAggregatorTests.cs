using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;
using System;
using System.Linq;

namespace AntSimComplexTests.Backend.Utilities
{
  [TestFixture]
  internal class StatsAggregatorTests
  {
    [Test]
    public void CtorShouldNotThrow()
    {
      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.DoesNotThrow(() => new StatsAggregator());
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(5)]
    public void StartIterationShouldNotThrow(int iteration)
    {
      // arrange
      var statsAggregator = new StatsAggregator();

      // assert
      Assert.DoesNotThrow(() => statsAggregator.StartIteration(iteration));
    }

    [Test]
    public void StopIterationGivenNullTourLengthArrayShouldThrowArgumentNullException()
    {
      // arrange
      var statsAggregator = new StatsAggregator();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => statsAggregator.StopIteration(null));
    }

    [Test]
    public void StopIterationGivenEmptyTourLengthArrayShouldThrowArgumentOutofRangeException()
    {
      // arrange
      var tourLengths = new double[0];
      var statsAggregator = new StatsAggregator();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => statsAggregator.StopIteration(tourLengths));
    }

    [Test]
    public void CallingStopIterationWithoutHavingCalledStartIterationShouldThrowInvalidOperationException()
    {
      // arrange
      var tourLengths = new[] { 0.0 };
      var statsAggregator = new StatsAggregator();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<InvalidOperationException>(() => statsAggregator.StopIteration(tourLengths));
    }

    [Test]
    public void IterationStatsAfterSingleStartSleepStopShouldHaveOnlyOneItem()
    {
      // arrange
      const int iteration = 1;
      var tourLengths = new[] { 0.0 };
      var statsAggregator = new StatsAggregator();

      // act
      statsAggregator.StartIteration(iteration);
      statsAggregator.StopIteration(tourLengths);

      var result = statsAggregator.IterationStats.Count;

      // assert
      Assert.AreEqual(iteration, result);
    }

    [Test]
    public void ClearStatsShouldRemoveAllIterationStatsItems()
    {
      // arrange
      const int iteration = 1;
      var tourLengths = new[] { 1.0, 2.0, 3.0, 4.0 };
      var statsAggregator = new StatsAggregator();

      // act
      statsAggregator.StartIteration(iteration);
      statsAggregator.StopIteration(tourLengths);
      statsAggregator.ClearStats();

      var result = statsAggregator.IterationStats.Count;

      // assert
      Assert.AreEqual(0, result);
    }
  }
}