using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;

namespace Engine
{
    class SimulatorPopulationEvaluator : MultiThreadedPopulationEvaluator
    {

        public SimulatorPopulationEvaluator(INetworkEvaluator eval)
            : base(eval, null)
        {

        }
    }
}
