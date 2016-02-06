using AntSimComplexTspLibItemManager.Utilities;
using NUnit.Framework;
using System;
using System.IO;

namespace AntSimComplexTests.GUI
{
  public class SymmetricTspItemLoaderTests
  {
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CtorGivenNullOrEmptyPathShouldThrowArgumentOutOfRangeException(string path)
    {
      // assert
      Assert.Throws<ArgumentOutOfRangeException>(() => ItemLoader(path));
    }

    [Test]
    public void CtorGivenInvalidTspLibPathShouldThrowArgumentOutOfRangeException()
    {
      // assert
      // Select an arbitrary directory.
      Assert.Throws<ArgumentOutOfRangeException>(() => ItemLoader(Directory.GetCurrentDirectory()));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    [TestCase(null)]
    public void GetItemGivenInvalidProblemNameShouldThrowArgumentOutOfRangeException(string problemName)
    {
      // arrange
      var itemLoader = ItemLoader(Helpers.LibPath);

      // assert
      Assert.Throws<ArgumentOutOfRangeException>(() => itemLoader.GetItem(problemName));
    }

    [TestCase("eil76")]
    [TestCase("att48")]
    [TestCase("berlin52")]
    public void GetItemGivenValidNamesForProblemsShouldReturnCorrectProblemItem(string problemName)
    {
      // arrange
      var itemLoader = ItemLoader(Helpers.LibPath);

      // act
      var result = itemLoader.GetItem(problemName);

      // assert
      Assert.IsNotNull(result);
      Assert.AreEqual(problemName, result.Problem.Name);
    }

    private static SymmetricTspItemLoader ItemLoader(string path)
    {
      return new SymmetricTspItemLoader(path);
    }
  }
}