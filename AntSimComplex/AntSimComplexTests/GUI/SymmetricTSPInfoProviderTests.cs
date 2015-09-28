using AntSimComplexUI.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.GUI
{
    public class SymmetricTSPInfoProviderTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTspLibItemNullConstructor()
        {
            var infoProvider = new SymmetricTSPInfoProvider(null);
        }
    }
}