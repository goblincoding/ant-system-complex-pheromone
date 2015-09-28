using AntSimComplexAlgorithms;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
    public class AntSystemTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullProblemParametersConstructor()
        {
            var antSystem = new AntSystem(null);
        }
    }
}