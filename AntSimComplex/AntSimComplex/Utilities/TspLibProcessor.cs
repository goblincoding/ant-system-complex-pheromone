using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;

namespace AntSimComplex.Utilities
{
    /// <summary>
    /// This class is responsible for selecting the symmetrical TSP problems that we're interested in.
    /// For the sake of this research application, only problem with fewer than or equal to 100 nodes
    /// are considered.  Furthermore, only problems with 2D coordinate sets are considered.
    /// </summary>
    internal class TspLibProcessor
    {
        private List<TspLib95Item> _tspLibItems;
        public List<string> ProblemNames { get; private set; }

        public TspLibProcessor(TspLib95 lib)
        {
            _tspLibItems = (from i in lib.TSPItems()
                            where i.Problem.NodeProvider.CountNodes() <= 100
                            select i).ToList();

            ProblemNames = (from i in _tspLibItems
                            select i.Problem.Name).ToList();
        }
    }
}