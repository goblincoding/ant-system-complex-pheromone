using AntSimComplexTests.GUI;
using AntSimComplexTspLibItemManager;
using AntSimComplexTspLibItemManager.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.TspLibManager
{
  [TestFixture]
  internal class TspLibManagerTests
  {
    private readonly SymmetricTspItemLoader _loader = new SymmetricTspItemLoader(Helpers.LibPath);
    private readonly TspLibItemManager _manager = new TspLibItemManager(Helpers.LibPath);

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void CtorGivenNullOrEmptyPathShouldThrowArgumentException(string path)
    {
      // assert
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentException>(() => new TspLibItemManager(path));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    [TestCase(null)]
    public void LoadItemGivenInvalidProblemNameShouldThrowArgumentException(string problemName)
    {
      // arrange

      // assert
      Assert.Throws<ArgumentException>(() => _manager.LoadItem(problemName));
    }

    [Test]
    public void DistancesShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.Distances, _manager.Distances);
    }

    [Test]
    public void NodeCountShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.NodeCount, _manager.NodeCount);
    }

    [Test]
    public void MinMaxCoordinatesShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.MaxXCoordinate, _manager.MaxXCoordinate);
      Assert.AreEqual(infoProvider.MaxYCoordinate, _manager.MaxYCoordinate);
      Assert.AreEqual(infoProvider.MinXCoordinate, _manager.MinXCoordinate);
      Assert.AreEqual(infoProvider.MinYCoordinate, _manager.MinYCoordinate);
    }

    [Test]
    public void HasOptimalTourShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.HasOptimalTour, _manager.HasOptimalTour);
    }

    [Test]
    public void OptimalTourLengthShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.OptimalTourLength, _manager.OptimalTourLength);
    }

    [Test]
    public void OptimalTourShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.OptimalTour, _manager.OptimalTour);
    }

    [Test]
    public void ProblemNameShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.ProblemName, _manager.ProblemName);
    }

    [Test]
    public void TspNodesShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var item = _loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);

      // act
      _manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.TspNodes, _manager.TspNodes);
    }

    [Test]
    public void AllProblemNamesShouldMatchThatOfItemLoader()
    {
      // assert
      Assert.AreEqual(_loader.ProblemNames, _manager.AllProblemNames);
    }
  }
}