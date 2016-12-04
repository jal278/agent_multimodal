using System;

namespace SharpNeatLib.NeuralNetwork
{
	/// <summary>
	/// A base class for neural networks. This class provides the underlying data structures
	/// for neurons and connections but not a technique for 'executing' the network.
	/// </summary>
	public abstract class AbstractNetwork : INetwork
	{
		// The master list of ALL neurons within the network.
		protected NeuronList masterNeuronList;

		// There follows a number of Lists that hold various neuron subsets. Perhaps not 
		// a particularly efficient way of doing things, but at least clear!

		// All input neurons. *Not* including single bias neuron. Used by SetInputSignal().
		NeuronList inputNeuronList;

		// All output neurons. Used by GetOutputSignal().
		NeuronList outputNeuronList;

		#region Constructor
		
		public AbstractNetwork(NeuronList neuronList)
		{
			inputNeuronList = new NeuronList();
			outputNeuronList = new NeuronList();
			LoadNeuronList(neuronList);
		}

		#endregion

		#region Properties

		public int InputNeuronCount
		{
			get	
			{
				return inputNeuronList.Count;	
			}
		}

		public int OutputNeuronCount
		{
			get	
			{
				return outputNeuronList.Count;	
			}
		}

        // Schrum: Had to add for compatibility with my changes to INetwork, but have decided
        // that this type of network can never have more than the default of one output module,
        // so the current module cannot actually change.
        public int CurrentOutputModule
        {
            get 
            {
                return 0;
            }
            set
            {
                // Nothing
            }
        }

        // Schrum: Since one module, all outputs are part of it
        public int OutputsPerPolicy
        {
            get
            {
                return OutputNeuronCount;
            }
        }

        // Schrum: Assume only one module for this network
        public int NumOutputModules
        {
            get
            {
                return 1;
            }
        }


        abstract public int TotalNeuronCount
        {
            get;

        }

		public NeuronList MasterNeuronList
		{
			get
			{
				return masterNeuronList;
			}
		}

        // Schrum: Added
        public int NumLinks {
            get
            {
                // Check each neuron and count the connections leaving it
                int total = 0;
                foreach (Neuron n in masterNeuronList)
                {
                    total += n.ConnectionList.Count;
                }
                return total;
            }
        }

        #endregion

        #region INetwork [Implemented]

        public void SetInputSignal(int index, float signalValue)
		{
			inputNeuronList[index].OutputValue = signalValue;
		}

		public void SetInputSignals(float[] signalArray)
		{
			// For speed we don't bother with bounds checks.
			for(int i=0; i<signalArray.Length; i++)
				inputNeuronList[i].OutputValue = signalArray[i];
		}
		
		public float GetOutputSignal(int index)
		{
			return (float)outputNeuronList[index].OutputValue;
		}

		public void ClearSignals()
		{
			int loopBound = masterNeuronList.Count;
			for(int j=0; j<loopBound; j++)
			{
				Neuron neuron = masterNeuronList[j];
				if(neuron.NeuronType != NeuronType.Bias)
					neuron.OutputValue=0;
			}
		}

		#endregion

		#region INetwork [Abstract]

		abstract public void SingleStep();
		abstract public void MultipleSteps(int numberOfSteps);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSteps">The number of timesteps to run the network before we give up.</param>
		/// <param name="maxAllowedSignalDelta"></param>
		/// <returns>False if the network did not relax. E.g. due to oscillating signals.</returns>
		abstract public bool RelaxNetwork(int maxSteps, double maxAllowedSignalDelta);

		#endregion

		#region Private Methods

		/// <summary>
		/// Accepts a list of interconnected neurons that describe the network and loads them into this class instance
		/// so that the network can be run. This primarily means placing input and output nodes into their own Lists
		/// for use during the run.
		/// </summary>
		/// <param name="neuronList"></param>
		private void LoadNeuronList(NeuronList neuronList)
		{
			masterNeuronList = neuronList;

			int loopBound = masterNeuronList.Count;
			for(int j=0; j<loopBound; j++)
			{
				Neuron neuron = masterNeuronList[j];

				switch(neuron.NeuronType)
				{
					case NeuronType.Input:
						inputNeuronList.Add(neuron);
						break;

					case NeuronType.Output:
						outputNeuronList.Add(neuron);
						break;
				}
			}
		}

		#endregion
	}
}
