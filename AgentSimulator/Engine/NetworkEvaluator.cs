using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib;

namespace Engine
{
    //A Simple NetworkEvaluator that passes the CPPN to it's experiment to be evaluated
    public class NetworkEvaluator : INetworkEvaluator 
    {
        SimulatorExperiment exp;

        public NetworkEvaluator(SimulatorExperiment e)
        {
            exp = e;
        }

        #region INetworkEvaluator Members

        public double EvaluateNetwork(SharpNeatLib.NeuralNetwork.INetwork network, out SharpNeatLib.BehaviorType behavior)
        {
            return exp.evaluateNetwork(network, out behavior,null);
        }
        //TODO: deal with threading
        public double threadSafeEvaluateNetwork(SharpNeatLib.NeuralNetwork.INetwork network, System.Threading.Semaphore sem, out SharpNeatLib.BehaviorType behavior, int thread)
        {
		//	try {
            return exp.evaluateNetwork(network, out behavior,sem);
            //} catch (Exception e) {
				
            //    behavior=new BehaviorType();
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine(e.StackTrace);
            //    throw e;
            //    return 0.0001;
				
            //}
        }

        public string EvaluatorStateMessage
        {
            get { return ""; }
        }

        #endregion
    }
}
