using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public interface IScript
    {
        /// <summary>
        /// Name of the function, should be the same as the class name.  Use this.GetType().Name;
        /// </summary>
        string name
        {
            get;
        }

        /// <summary>
        /// Plain text description of what the fitness function does.
        /// </summary>
        string description
        {
            get;
        }

        /// <summary>
        /// Calculates the fitness function.
        /// </summary>
        /// <param name="engine">The engine currently running the simulation.</param>
        /// <param name="environment">The environment the agents are current in.</param>
        /// <returns>The current fitness value.</returns>
        void onTimeStep(SimulatorExperiment experiment);
		void reset();
		
    }
}
