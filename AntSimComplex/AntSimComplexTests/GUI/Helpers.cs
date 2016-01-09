using System.IO;

namespace AntSimComplexTests.GUI
{
  public static class Helpers
  {
    private const string PackageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
    public static string LibPath { get; } = Path.GetFullPath(PackageRelPath);
  }
}