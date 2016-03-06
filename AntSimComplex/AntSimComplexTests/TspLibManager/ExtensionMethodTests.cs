using AntSimComplexTspLibItemManager.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.TspLibManager
{
  [TestFixture]
  internal class ExtensionMethodTests
  {
    [Test]
    public void GetNearestNeighbourTourLengthShouldReturnCorrectValue()
    {
      // Node(weight)NextNearest
      // 7(1)3(2)8(1)2(2)9(1)1(3)0(4)4(1)6(2)5(3)7

      // arrange
      var problem = new MockProblem();
      var random = Substitute.For<Random>();
      random.Next(0, 9).Returns(7); // force start node to be 7

      // act
      var result = problem.GetNearestNeighbourTourLength(random);

      // assert
      Assert.AreEqual(20, result);
    }
  }
}