using AntSimComplex.Utilities;
using TspLibNet.Graph.Nodes;

namespace AntSimComplexTests
{
    public static class Helpers
    {
        private const string _packageRelPath = @"..\..\..\packages\TSPLib.Net.1.1.4\TSPLIB95";
        public static readonly string LibPath = System.IO.Path.GetFullPath(_packageRelPath);

        public static SymmetricTSPItemSelector GetItemSelector(string path)
        {
            return new SymmetricTSPItemSelector(path, 100, typeof(Node2D));
        }
    }
}