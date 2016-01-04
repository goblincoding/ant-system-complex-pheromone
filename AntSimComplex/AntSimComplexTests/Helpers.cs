using AntSimComplexUI.Utilities;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests
{
  public static class Helpers
  {
    private const int MaxNodesPerProblem = 100;
    private const string PackageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
    public static readonly string LibPath = System.IO.Path.GetFullPath(PackageRelPath);

    /// <summary>
    /// For this application we are only ever interested in TSP problems with fewer than
    /// 100 nodes and whose nodes are 2D coordinate sets.
    /// <returns>A SymmetricTSPItemSelector</returns>
    /// </summary>
    public static SymmetricTspItemSelector GetItemSelector(string path)
    {
      return new SymmetricTspItemSelector(path, MaxNodesPerProblem, typeof(Node2D));
    }
  }
}