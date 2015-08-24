using AntSimComplex.Utilities;
using System;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests
{
    public static class Helpers
    {
        private const string _packageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
        public static readonly string LibPath = System.IO.Path.GetFullPath(_packageRelPath);

        /// <summary>
        /// For this application we are only ever interested in TSP problems with fewer than
        /// 100 nodes and whose nodes are 2D coordinate sets.
        /// <returns>A SymmetricTSPItemSelector</returns>
        public static SymmetricTSPItemSelector GetItemSelector(string path)
        {
            return new SymmetricTSPItemSelector(path, 100, typeof(Node2D));
        }

        public static IProblem GetRandomTSPProblem()
        {
            var itemSelector = GetItemSelector(LibPath);
            var rand = new Random();
            var name = itemSelector.ProblemNames.ElementAt(rand.Next(0, itemSelector.ProblemNames.Count()));
            var item = itemSelector.GetItem("ulysses16.tsp"/*name*/);
            return item.Problem;
        }
    }
}