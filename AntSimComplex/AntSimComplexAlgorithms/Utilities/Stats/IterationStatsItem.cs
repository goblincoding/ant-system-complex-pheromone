using System;

namespace AntSimComplexAlgorithms.Utilities.Stats
{
  public struct IterationStatsItem : IComparable<IterationStatsItem>
  {
    public static string CsvHeader => "Iteration, Time Elapsed (ms), Avg Tour, Best Tour";
    public string CsvResult => $"{_iteration},{_timeElapsed},{_averageTourLength}, {_bestTourLength}";

    private readonly int _iteration;
    private readonly long _timeElapsed;
    private readonly double _averageTourLength;
    private readonly double _bestTourLength;

    public IterationStatsItem(int iteration, long timeElapsed, double averageTourLength, double bestTourLength)
    {
      _iteration = iteration;
      _timeElapsed = timeElapsed;
      _averageTourLength = averageTourLength;
      _bestTourLength = bestTourLength;
    }

    public int CompareTo(IterationStatsItem other)
    {
      return _iteration.CompareTo(other._iteration);
    }
  }
}