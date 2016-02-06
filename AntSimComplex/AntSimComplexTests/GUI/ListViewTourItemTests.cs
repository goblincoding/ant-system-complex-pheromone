using AntSimComplexTspLibItemManager.Utilities;
using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AntSimComplexTests.GUI
{
  [TestFixture]
  internal class ListViewTourItemTests
  {
    [Test]
    public void CtorShouldSetPropertiesCorrectly()
    {
      // arrange
      var tspNodes = new List<TspNode> { new TspNode(1, 4.5, 6.7), new TspNode(1, 4.5, 6.7), new TspNode(1, 4.5, 6.7) };
      const int tourLength = 234;
      const string tourInfo = "test";

      var ids = from n in tspNodes
                select n.Id.ToString();
      var nodeSequence = ids.Aggregate((a, b) => a + "," + b);

      // act
      var listViewTourItem = new ListViewTourItem(tspNodes, tourLength, tourInfo);

      // assert
      Assert.AreEqual(tourLength, listViewTourItem.Length);
      Assert.AreEqual(tourInfo, listViewTourItem.TourInfo);
      Assert.AreEqual(nodeSequence, listViewTourItem.NodeSequence);
      Assert.AreEqual(tspNodes, listViewTourItem.Nodes);
    }
  }
}