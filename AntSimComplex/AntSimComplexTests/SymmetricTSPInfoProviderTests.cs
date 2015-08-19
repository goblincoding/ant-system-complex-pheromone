using AntSimComplex.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests
{
    public class SymmetricTSPInfoProviderTests
    {
        private const string _packageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
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
        // Catching the exception of current TSPLib.Net 1.1.2
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void TestInvalidTspLibPathConstructor()
        {
            // Select an arbitrary directory.
            var info = new SymmetricTSPInfoProvider(System.IO.Directory.GetCurrentDirectory());
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetProblemParametersInvalidProblemName()
        {
            var info = new SymmetricTSPInfoProvider(LibPath);
            var param = info.GetProblemParameters("invalid");
        }

        [Test]
        public void TestGetProblemParametersValidProblemName()
        {
            var info = new SymmetricTSPInfoProvider(LibPath);
            foreach (var name in info.ProblemNames)
            {
                var param = info.GetProblemParameters(name);
                Assert.NotNull(param);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetGraphNodesInvalidProblemName()
        {
            var info = new SymmetricTSPInfoProvider(LibPath);
            var nodes = info.GetGraphNodes("invalid");
        }

        [Test]
        public void TestGetGraphNodesValidProblemName()
        {
            var info = new SymmetricTSPInfoProvider(LibPath);
            foreach (var name in info.ProblemNames)
            {
                var nodes = info.GetGraphNodes(name);
                Assert.IsFalse(nodes.Contains(null), $"Found a null node in {name}");
                Assert.IsTrue(nodes.All(n => n is Node2D), $"{name} contains non-Node2D objects.");
            }
        }

        [Test]
        public void TestGetOptimalTourInvalidProblemName()
        {
            var info = new SymmetricTSPInfoProvider(LibPath);
            var nodes = info.GetOptimalTourNodes("invalid");
            Assert.IsEmpty(nodes);
        }
    }
}