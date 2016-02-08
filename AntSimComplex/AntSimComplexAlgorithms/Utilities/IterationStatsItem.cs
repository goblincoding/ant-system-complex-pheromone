using System;

namespace AntSimComplexAlgorithms.Utilities
{
  public struct IterationStatsItem : IComparable<IterationStatsItem>
  {
    public static string CsvHeader => $"{nameof(Iteration)},{nameof(TimeElapsed)}(ms),{nameof(AverageTourLength)}";
    public string CsvString => $"{Iteration},{TimeElapsed},{AverageTourLength}";

    public int Iteration { get; set; }
    public long TimeElapsed { get; set; }
    public int AverageTourLength { get; set; }

    public IterationStatsItem(int iteration, long timeElapsed, int averageTourLength)
    {
      Iteration = iteration;
      TimeElapsed = timeElapsed;
      AverageTourLength = averageTourLength;
    }

    public int CompareTo(IterationStatsItem other)
    {
      return Iteration.CompareTo(other.Iteration);
    }
  }
}