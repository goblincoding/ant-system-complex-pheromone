using AntSimComplexTests.GUI;
using AntSimComplexTspLibItemManager.Utilities;
using NUnit.Framework;
using System;
using System.IO;

namespace AntSimComplexTests.TspLibManager
{
  internal class SymmetricTspItemLoaderTests
  {
    private readonly SymmetricTspItemLoader _loader = new SymmetricTspItemLoader(Helpers.LibPath);

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CtorGivenNullOrEmptyPathShouldThrowArgumentOutOfRangeException(string path)
    {
      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SymmetricTspItemLoader(path));
    }

    [Test]
    public void CtorGivenInvalidTspLibPathShouldThrowArgumentOutOfRangeException()
    {
      // assert
      // Select an arbitrary directory.
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentOutOfRangeException>(() => new SymmetricTspItemLoader(Directory.GetCurrentDirectory()));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    [TestCase(null)]
    public void GetItemGivenInvalidProblemNameShouldThrowArgumentOutOfRangeException(string problemName)
    {
      // assert
      Assert.Throws<ArgumentOutOfRangeException>(() => _loader.GetItem(problemName));
    }

    [TestCase("eil76")]
    [TestCase("att48")]
    [TestCase("berlin52")]
    public void GetItemGivenValidNamesForProblemsShouldReturnCorrectProblemItem(string problemName)
    {
      // act
      var result = _loader.GetItem(problemName);

      // assert
      Assert.IsNotNull(result);
      Assert.AreEqual(problemName, result.Problem.Name);
    }
  }
}