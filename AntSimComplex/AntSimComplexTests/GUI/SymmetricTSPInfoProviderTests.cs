using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.GUI
{
  public class SymmetricTspInfoProviderTests
  {
    [Test]
    public void TestTspLibItemNullConstructor()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => new SymmetricTSPInfoProvider(null));
    }
  }
}