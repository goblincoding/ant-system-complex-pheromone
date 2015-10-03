using AntSimComplexAlgorithms.Utilities;
using NUnit.Framework;
using System;

namespace AntSimComplexTests.Backend
{
    public class RouletteWheelSelectorTests
    {
        [Test]
        public void TestMakeSelectionIndexValid()
        {
            const int current = 0;
            var problem = new MockProblem();
            var data = new DataStructures(problem, 0.3);
            var neighbours = data.NearestNeighbours(current);
            var nextIndex = RouletteWheelSelector.MakeSelection(data, neighbours, current);
            Assert.Contains(nextIndex, data.OrderedNodeIndices);
        }
    }
}