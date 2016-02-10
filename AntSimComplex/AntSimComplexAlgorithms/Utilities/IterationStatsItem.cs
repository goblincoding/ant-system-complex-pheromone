using System;

namespace AntSimComplexAlgorithms.Utilities
{
  public struct IterationStatsItem : IComparable<IterationStatsItem>
  {
    public static string CsvHeader => $"{nameof(Iteration)},{nameof(TimeElapsed)}(ms),{nameof(AverageTourLength)},{nameof(BestTour)}";
    public string CsvString => $"{Iteration},{TimeElapsed},{AverageTourLength},{BestTour}";

    public int Iteration { get; set; }
    public long TimeElapsed { get; set; }
    public int AverageTourLength { get; set; }
    public int BestTour { get; set; }

    public IterationStatsItem(int iteration, long timeElapsed, int averageTourLength, int bestTour)
    {
      Iteration = iteration;
      TimeElapsed = timeElapsed;
      AverageTourLength = averageTourLength;
      BestTour = bestTour;
    }

    public int CompareTo(IterationStatsItem other)
    {
      return Iteration.CompareTo(other.Iteration);
    }
  }
}