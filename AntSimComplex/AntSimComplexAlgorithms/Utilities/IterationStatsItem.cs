using System;

namespace AntSimComplexAlgorithms.Utilities
{
  public struct IterationStatsItem : IComparable<IterationStatsItem>
  {
    public static string CsvHeader => $"{nameof(Iteration)},{nameof(TimeElapsed)}(ms),{nameof(AverageTourLength)}, {nameof(BestTourLength)}";
    public string CsvString => $"{Iteration},{TimeElapsed},{AverageTourLength}, {BestTourLength}";

    public int Iteration { get; set; }
    public long TimeElapsed { get; set; }
    public double AverageTourLength { get; set; }
    public double BestTourLength { get; set; }

    public IterationStatsItem(int iteration, long timeElapsed, double averageTourLength, double bestTourLength)
    {
      Iteration = iteration;
      TimeElapsed = timeElapsed;
      AverageTourLength = averageTourLength;
      BestTourLength = bestTourLength;
    }

    public int CompareTo(IterationStatsItem other)
    {
      return Iteration.CompareTo(other.Iteration);
    }
  }
}