using AntSimComplexAS;
using NUnit.Framework;
using System;

namespace AntSimComplexTests
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