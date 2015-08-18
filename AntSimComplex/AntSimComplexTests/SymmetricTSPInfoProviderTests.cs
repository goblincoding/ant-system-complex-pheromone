using AntSimComplex.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests
{
    public class SymmetricTSPInfoProviderTests
    {
        private const string _packageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.2\TSPLIB95";
        private readonly string LibPath = System.IO.Path.GetFullPath(_packageRelPath);

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPathConstructor(string path)
        {
            var info = new SymmetricTSPInfoProvider(path);
        }

        [Test]
        //[ExpectedException(typeof(ApplicationException), ExpectedMessage = "No symmetric TSP library items were loaded.")]
        // Catching the exception of current TSPLib.Net 1.1.2
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void TestNoItemsLoadedConstructor()
        {
            // Select an arbitrary directory.
            var info = new SymmetricTSPInfoProvider(System.IO.Directory.GetCurrentDirectory());
        }
    }
}