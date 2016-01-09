using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using TspLibNet;

namespace AntSimComplexTests.GUI
{
  [TestFixture]
  public class SymmetricTspItemInfoProviderTests
  {
    [Test]
    public void CtorNullTspLibItemShouldThrowArgumentNullException()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => new SymmetricTspItemInfoProvider(null));
    }

    [TestCase("bayg29")]
    [TestCase("gr21")]
    public void CtorGivenItemThatDoesNotHave2DNodesShouldThrowArgumentOutOfRangeException(string tspProblemName)
    {
      // arrange
      var tspLib = new TspLib95(Helpers.LibPath);
      tspLib.LoadTSP(tspProblemName);
      var items = tspLib.TSPItems();

      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SymmetricTspItemInfoProvider(items.First()));
    }
  }
}