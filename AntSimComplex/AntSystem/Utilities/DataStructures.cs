using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;

namespace AntSimComplexAS
{
    /// <summary>
    /// This class represents the prepopulated (prior to algorithm run-time), consolidated,
    /// calculated values of different aspects of a particular TSP problem instance in data
    /// structures such as distance and nearest neighbour matrices.
    /// </summary>
    public class DataStructures
    {
        /// <param name="problem">The TSP problem instance to which Ant System is to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when "problem" is null.</exception>
        public DataStructures(IProblem problem)
        {
            if (problem == null)
            {
                throw new ArgumentNullException(nameof(problem), "The AntSystem constructor needs a valid problem instance argument");
            }
        }
    }
}