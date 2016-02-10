using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AntSimComplexAlgorithms.Utilities
{
  /// <summary>
  /// Keeps track of performance statistics for each execution of the algorithm.
  /// </summary>
  internal class StatsAggregator
  {
    private readonly Stopwatch _stopWatch;
    private int _currentIteration;
    private bool _startTimerCalled;

    /// <summary>
    /// A list of iteration data.
    /// </summary>
    public List<IterationStatsItem> IterationStats { get; } = new List<IterationStatsItem>();

    /// <summary>
    /// Constructor.
    /// </summary>
    public StatsAggregator()
    {
      _stopWatch = new Stopwatch();
    }

    /// <summary>
    /// Start gathering information for an iteration.
    /// </summary>
    /// <param name="iteration">The current iteration number</param>
    public void StartIteration(int iteration)
    {
      _currentIteration = iteration;
      _startTimerCalled = true;

      // Start last so that setup does not interfere with performance data.
      _stopWatch.Start();
    }

    /// <summary>
    /// Consolidate information for the current iteration.
    /// </summary>
    /// <param name="tourLengths">The tour lengths of all the tours constructed during the iteration ending.</param>
    public void StopIteration(IEnumerable<int> tourLengths)
    {
      // Stop first so that the safety checks do not interfere with performance data.
      _stopWatch.Stop();

      if (tourLengths == null)
      {
        throw new ArgumentNullException(nameof(tourLengths), "Ant array can't be null");
      }

      var enumerable = tourLengths as int[] ?? tourLengths.ToArray();
      if (!enumerable.Any())
      {
        throw new ArgumentOutOfRangeException(nameof(tourLengths), "Ant array can't be empty");
      }

      if (!_startTimerCalled)
      {
        throw new InvalidOperationException("Cannot call StopIteration without calling StartIteration first");
      }

      _startTimerCalled = false;
      IterationStats.Add(new IterationStatsItem(_currentIteration, _stopWatch.ElapsedMilliseconds, (int)enumerable.Average(), enumerable.Min()));
      _stopWatch.Reset();
    }

    /// <summary>
    /// Remove all current iteration stats items.
    /// </summary>
    public void ClearStats()
    {
      IterationStats.Clear();
    }
  }
}