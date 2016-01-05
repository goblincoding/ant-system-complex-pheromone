using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests.GUI
{
  public class SymmetricTspItemSelectorTests
  {
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void ConstructorGivenNullOrEmptyPathShouldThrowArgumentOutOfRangeException(string path)
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => GetItemSelector(path));
    }

    [Test]
    public void ConstructorGivenInvalidTspLibPathShouldThrowArgumentOutOfRangeException()
    {
      // Select an arbitrary directory.
      Assert.Throws<ArgumentOutOfRangeException>(() => GetItemSelector(Directory.GetCurrentDirectory()));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    [TestCase(null)]
    public void GetItemGivenInvalidProblemNameShouldThrowArgumentOutOfRangeException(string problemName)
    {
      var itemSelector = GetItemSelector(LibPath);
      Assert.Throws<ArgumentOutOfRangeException>(() => itemSelector.GetItem(problemName));
    }

    [Test]
    public void GetItemGivenValidNamesForAllProblemsMayNotBeNull()
    {
      var itemSelector = GetItemSelector(LibPath);
      foreach (var item in itemSelector.ProblemNames.Select(name => itemSelector.GetItem(name)))
      {
        Assert.IsNotNull(item);
      }
    }

    private const string PackageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
    private static string LibPath { get; } = Path.GetFullPath(PackageRelPath);

    /// <summary>
    /// For this application we are only ever interested in TSP problems with fewer than
    /// 100 nodes and whose nodes are 2D coordinate sets.
    /// <returns>A SymmetricTSPItemSelector</returns>
    /// </summary>
    private static SymmetricTspItemSelector GetItemSelector(string path)
    {
      return new SymmetricTspItemSelector(path, 100, typeof(Node2D));
    }
  }
}