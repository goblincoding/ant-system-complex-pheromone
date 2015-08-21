using NUnit.Framework;
using System;

namespace AntSimComplexTests
{
    public class SymmetricTSPItemSelectorTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPathConstructor(string path)
        {
            var itemSelector = Helpers.GetItemSelector(path);
        }

        [Test]
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void TestInvalidTspLibPathConstructor()
        {
            // Select an arbitrary directory.
            var itemSelector = Helpers.GetItemSelector(System.IO.Directory.GetCurrentDirectory());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("invalid")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetItemInvalidProblemName(string problemName)
        {
            var itemSelector = Helpers.GetItemSelector(Helpers.LibPath);
            var item = itemSelector.GetItem(problemName);
        }

        [Test]
        public void TestGetItemValidForAll()
        {
            var itemSelector = Helpers.GetItemSelector(Helpers.LibPath);
            foreach (var name in itemSelector.ProblemNames)
            {
                var item = itemSelector.GetItem(name);
                Assert.IsNotNull(item);
            }
        }
    }
}