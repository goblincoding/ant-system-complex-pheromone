using AntSimComplexTspLibItemManager.Utilities;
using NUnit.Framework;

namespace AntSimComplexTests.TspLibManager
{
  [TestFixture]
  internal class TspNodeTests
  {
    [Test]
    // ReSharper disable once InconsistentNaming
    public void CtorGivenIdXYShouldSetPropertiesCorrectly()
    {
      // arrange
      const int id = 1;
      const double x = 3.4;
      const double y = 4.5;

      // act
      var tspNode = new TspNode(id, x, y);

      // assert
      Assert.AreEqual(id, tspNode.Id);
      Assert.AreEqual(x, tspNode.X);
      Assert.AreEqual(y, tspNode.Y);
    }
  }
}