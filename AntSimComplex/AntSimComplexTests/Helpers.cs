using AntSimComplexUI.Utilities;
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
    }
}