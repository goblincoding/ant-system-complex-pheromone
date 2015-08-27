using AntSimComplexAS.Utilities;
using System;

namespace AntSystemAS
{
    public class Ant
    {
        private double _tourLength;
        private int _currentNode;
        private int[] _visited;
        private int[] _tour;

        public Ant(int startNode, int nrNodes)
        {
            _currentNode = startNode;

            // +1 since the ant has to return to the initial node.
            _tour = new int[nrNodes + 1];

            // Flag the current node as having been visited.
            _visited = new int[nrNodes];
            _visited[_currentNode] = 1;
        }

        public void MoveNext(object sender, EventArgs args)
        {
        }

        public void Reset(object sender, EventArgs args)
        {
            // Do nothing with current node, we can start with whichever node
            // we are on (that should be random enough).
            _tourLength = 0.0;
            _tour.Initialize();
            _visited.Initialize();
        }
    }
}