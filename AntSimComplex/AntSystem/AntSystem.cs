using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;

namespace AntSimComplexAS
{
    /// <summary>
    /// This class is the entry point for the Ant System implementation.
    /// </summary>
    public class AntSystem
    {
        private DataStructures _dataStructures;
        private Parameters _parameters;

        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
        public AntSystem(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The AntSystem constructor needs a valid problem instance argument");
            }

            _dataStructures = new DataStructures(problem);
            _parameters = new Parameters(problem);
        }
    }
}