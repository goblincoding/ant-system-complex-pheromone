using System;
using System.Collections.Generic;

namespace AntSimComplexAlgorithms.Utilities
{
  public class BestTour : IComparable<BestTour>, IComparable
  {
    /// <summary>
    /// Tour indices.
    /// </summary>
    public IEnumerable<int> Tour { get; set; }

    public int TourLength { get; set; }

    public int CompareTo(BestTour other)
    {
      if (other == null)
      {
        throw new ArgumentNullException(nameof(other));
      }

      return TourLength.CompareTo(other.TourLength);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
      {
        return 1;
      }

      var other = obj as BestTour;
      if (other == null)
      {
        throw new ArgumentException("Object is not of type 'BestTour'");
      }

      return TourLength.CompareTo(other.TourLength);
    }
  }
}