using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Edges;
using TspLibNet.Graph.EdgeWeights;
using TspLibNet.Graph.FixedEdges;
using TspLibNet.Graph.Nodes;
using TspLibNet.Tours;

namespace AntSimComplexTests
{
    internal static class MockConstants
    {
        public const int NrNodes = 10;
    }

    internal class MockEdgeWeightsProvider : IEdgeWeightsProvider
    {
        // Random edge weight values (10 * 10).
        private static readonly double[][] _weights =
        {
            new double[]{ 0, 3, 7, 3, 4, 5, 6, 7, 8, 9 },
            new double[]{ 3, 0, 3, 4, 5, 6, 7, 8, 9, 1 },
            new double[]{ 7, 3, 0, 5, 6, 7, 8, 9, 1, 2 },
            new double[]{ 3, 4, 5, 0, 7, 8, 9, 1, 2, 3 },
            new double[]{ 4, 5, 6, 7, 0, 9, 1, 2, 3, 4 },
            new double[]{ 5, 6, 7, 8, 9, 0, 2, 3, 4, 5 },
            new double[]{ 6, 7, 8, 9, 1, 2, 0, 4, 5, 6 },
            new double[]{ 7, 8, 9, 1, 2, 3, 4, 0, 6, 7 },
            new double[]{ 8, 9, 1, 2, 3, 4, 5, 6, 0, 8 },
            new double[]{ 9, 1, 2, 3, 4, 5, 6, 7, 8, 0 }
        };

        // Convenience method so we can work with indices directly.
        public double GetWeight(int first, int second)
        {
            return _weights[first][second];
        }

        // Don't want to use node objects for testing.  Use the
        // MockProblem GetWeight method!
        public double GetWeight(INode first, INode second)
        {
            return _weights[first.Id][second.Id];
        }
    }

    internal class MockNodeProvider : INodeProvider
    {
        // 10 nodes.
        private static readonly int[] _xCoords = { 2, 4, 6, 7, 4, 3, 8, 9, 1, 0 };

        private static readonly int[] _yCoords = { 3, 5, 6, 2, 5, 8, 4, 0, 1, 7 };

        private static readonly INode[] _nodes =
        {
            new Node2D(0, _xCoords[0], _yCoords[0]),
            new Node2D(1, _xCoords[1], _yCoords[1]),
            new Node2D(2, _xCoords[2], _yCoords[2]),
            new Node2D(3, _xCoords[3], _yCoords[3]),
            new Node2D(4, _xCoords[4], _yCoords[4]),
            new Node2D(5, _xCoords[5], _yCoords[5]),
            new Node2D(6, _xCoords[6], _yCoords[6]),
            new Node2D(7, _xCoords[7], _yCoords[7]),
            new Node2D(8, _xCoords[8], _yCoords[8]),
            new Node2D(9, _xCoords[9], _yCoords[9])
        };

        public int CountNodes()
        {
            return _nodes.Length;
        }

        public INode GetNode(int id)
        {
            return _nodes[id];
        }

        public List<INode> GetNodes()
        {
            return _nodes.ToList();
        }
    }

    internal class MockProblem : IProblem
    {
        public IEdgeWeightsProvider EdgeWeightsProvider { get; } = new MockEdgeWeightsProvider();
        public INodeProvider NodeProvider { get; } = new MockNodeProvider();

        /// <summary>
        /// Convenience method to bypass IEdgeWeightsProvider GetWeight(INode,INode) method
        /// </summary>
        public double GetWeight(int first, int second)
        {
            return ((MockEdgeWeightsProvider)EdgeWeightsProvider).GetWeight(first, second);
        }

#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar

        public string Comment
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEdgeProvider EdgeProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IFixedEdgesProvider FixedEdgesProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ProblemType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double TourDistance(ITour tour)
        {
            throw new NotImplementedException();
        }

#pragma warning restore RECS0083 // Shows NotImplementedException throws in the quick task bar
    }
}