using AntSimComplexAlgorithms;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
  [TestFixture]
  public class AntSystemTests
  {
    [Test]
    public void TestNullProblemParametersConstructor()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Assert.Throws<ArgumentNullException>(() => new AntSystem(null));
    }
  }
}