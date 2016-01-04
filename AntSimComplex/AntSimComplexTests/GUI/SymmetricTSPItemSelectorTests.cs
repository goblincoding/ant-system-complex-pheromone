using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;
using System.IO;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests.GUI
{
  public class SymmetricTspItemSelectorTests
  {
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void TestNullPathConstructor(string path)
    {
      Assert.Throws<ArgumentNullException>(() => Helpers.GetItemSelector(path));
    }

    [Test]
    public void TestInvalidTspLibPathConstructor()
    {
      // Select an arbitrary directory.
      Assert.Throws<DirectoryNotFoundException>(() => Helpers.GetItemSelector(Directory.GetCurrentDirectory()));
    }

    [Test]
    public void TestNoTspLibItemsLoadedConstructor()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SymmetricTSPItemSelector(Helpers.LibPath, 10, typeof(Node3D)));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    public void TestGetItemInvalidProblemName(string problemName)
    {
      var itemSelector = Helpers.GetItemSelector(Helpers.LibPath);
      Assert.Throws<ArgumentOutOfRangeException>(() => itemSelector.GetItem(problemName));
    }

    [Test]
    public void TestGetItemValidForAll()
    {
      var itemSelector = Helpers.GetItemSelector(Helpers.LibPath);
      foreach (var name in itemSelector.ProblemNames)
      {
        var item = itemSelector.GetItem(name);
        Assert.IsNotNull(item);
      }
    }
  }
}