using AntSimComplexTests.GUI;
using AntSimComplexTspLibItemManager;
using AntSimComplexTspLibItemManager.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;
using TspLibNet.Tours;

namespace AntSimComplexTests.TspLibManager
{
  [TestFixture]
  internal class TspLibManagerTests
  {
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
      var manager = new TspLibItemManager(Helpers.LibPath);

      // assert
      Assert.Throws<ArgumentException>(() => manager.LoadItem(problemName));
    }

    [Test]
    public void DistancesShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.Distances, manager.Distances);
    }

    [Test]
    public void NodeCountShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.NodeCount, manager.NodeCount);
    }

    [Test]
    public void MinMaxCoordinatesShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.MaxXCoordinate, manager.MaxXCoordinate);
      Assert.AreEqual(infoProvider.MaxYCoordinate, manager.MaxYCoordinate);
      Assert.AreEqual(infoProvider.MinXCoordinate, manager.MinXCoordinate);
      Assert.AreEqual(infoProvider.MinYCoordinate, manager.MinYCoordinate);
    }

    [Test]
    public void HasOptimalTourShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.HasOptimalTour, manager.HasOptimalTour);
    }

    [Test]
    public void OptimalTourLengthShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.OptimalTourLength, manager.OptimalTourLength);
    }

    [Test]
    public void OptimalTourShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.OptimalTour, manager.OptimalTour);
    }

    [Test]
    public void ProblemNameShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      Assert.AreEqual(infoProvider.ProblemName, manager.ProblemName);
    }

    [Test]
    public void NodeCoordinatesAsPointsShouldMatchThatOfInfoProvider()
    {
      // arrange
      const string problemName = "eil76";
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var item = loader.GetItem(problemName);
      var infoProvider = new SymmetricTspItemInfoProvider(item);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // act
      manager.LoadItem(problemName);

      // assert
      CollectionAssert.AreEqual(infoProvider.NodeCoordinatesAsPoints, manager.NodeCoordinatesAsPoints);
    }

    [Test]
    public void AllProblemNamesShouldMatchThatOfItemLoader()
    {
      // arrange
      var loader = new SymmetricTspItemLoader(Helpers.LibPath);
      var manager = new TspLibItemManager(Helpers.LibPath);

      // assert
      Assert.AreEqual(loader.ProblemNames, manager.AllProblemNames);
    }
  }
}