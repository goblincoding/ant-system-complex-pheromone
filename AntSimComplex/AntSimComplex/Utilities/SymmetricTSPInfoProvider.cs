using AntSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace AntSimComplex.Utilities
{
    /// <summary>
    /// This class is responsible for selecting the symmetrical TSP problems that we're interested in.
    /// For the sake of this research application only problems with fewer than or equal to 100 nodes and
    /// 2D coordinate sets are considered.
    /// </summary>
    public class SymmetricTSPInfoProvider
    {
        private List<TspLib95Item> _tspLibItems;

        /// <summary>
        /// The list of names of all symmetric TSP problems loaded.
        /// </summary>
        public List<string> ProblemNames { get; private set; }

        /// <summary>
        /// Constructor.  Exceptions thrown are from underlying TspLib95 instance.
        /// </summary>
        /// <param name="tspLibPath">The directory path to the TSPLIB95 library.</param>
        /// <exception cref="ArgumentNullException">Thrown if path argument is null or empty</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if TSP lib path does not point to TSPLIB95 or doesn't exist.</exception>
        public SymmetricTSPInfoProvider(string tspLibPath)
        {
            var tspLib = new TspLib95(tspLibPath);
            var items = tspLib.LoadAllTSP();
            Debug.Assert(items.Any());

            // We only need to check one node for node type since it is not possible to
            // have different types in the same list.
            _tspLibItems = (from i in items
                            where i.Problem.NodeProvider.CountNodes() <= 100
                            where i.Problem.NodeProvider.GetNodes().First() is Node2D
                            select i)?.ToList();
            Debug.Assert(_tspLibItems != null && _tspLibItems.Any());

            ProblemNames = (from i in _tspLibItems
                            select i.Problem.Name)?.ToList();
            Debug.Assert(ProblemNames != null && ProblemNames.Any());
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>A new "Parameters" object instance for the relevant TSP problem</returns>
        public Parameters GetProblemParameters(string problemName)
        {
            var problem = _tspLibItems.First(i => i.Problem.Name == problemName).Problem;
            return new Parameters(problem);
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>A list of Node2D objects.</returns>
        public List<Node2D> GetGraphNodes(string problemName)
        {
            var item = _tspLibItems.First(i => i.Problem.Name == problemName);
            var nodes = from n in item.Problem.NodeProvider.GetNodes()
                        select n as Node2D;
            return nodes.ToList();
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>A list of Node2D objects corresponding to the optimal tour for the problem (if it is known).</returns>
        public List<Node2D> GetOptimalTourNodes(string problemName)
        {
            var item = _tspLibItems.First(i => i.Problem.Name == problemName);
            if (item.OptimalTour != null)
            {
                var nodes = (from n in item.OptimalTour?.Nodes
                             select item.Problem.NodeProvider.GetNode(n) as Node2D).ToList();
                nodes.RemoveAll(n => n == null);
                return nodes;
            }

            return new List<Node2D>();
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>The maximum "x" coordinate of all the nodes in the graph</returns>
        public double GetMaxX(string problemName)
        {
            var nodes = GetGraphNodes(problemName);
            var max = nodes.Max(i => i.X);
            return max;
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>The minimum "x" coordinate of all the nodes in the graph</returns>
        public double GetMinX(string problemName)
        {
            var nodes = GetGraphNodes(problemName);
            var max = nodes.Min(i => i.X);
            return max;
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>The maximum "y" coordinate of all the nodes in the graph</returns>
        public double GetMaxY(string problemName)
        {
            var nodes = GetGraphNodes(problemName);
            var max = nodes.Max(i => i.Y);
            return max;
        }

        /// <param name="problemName">The name of the symmetric TSP problem</param>
        /// <returns>The minimum "y" coordinate of all the nodes in the graph</returns>
        public double GetMinY(string problemName)
        {
            var nodes = GetGraphNodes(problemName);
            var max = nodes.Min(i => i.Y);
            return max;
        }
    }
}