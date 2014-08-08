using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Experiments;
using SharpNeatLib.Evolution;

namespace Engine
{
    public class Experiment : IExperiment
    {
        IPopulationEvaluator populationEval = null;
        SimulatorExperiment simExp = null;
        NeatParameters neatParams = null;
        int inputs, outputs; 
        // Schrum: Added
        int outputsPerPolicy;

        public Experiment(SimulatorExperiment exp)
        {
            simExp = exp;
            inputs = exp.getNumCPPNInputs();
            outputs = exp.getNumCPPNOutputs();
            outputsPerPolicy = exp.getNumCPPNOutputsPerModule();
        }

        #region IExperiment Members

        public void LoadExperimentParameters(System.Collections.Hashtable parameterTable)
        {
            ;
        }

        public SharpNeatLib.Evolution.IPopulationEvaluator PopulationEvaluator
        {
            get
            {
                if (populationEval == null)
                    ResetEvaluator(HyperNEATParameters.substrateActivationFunction);
                return populationEval;
            }
        }

        public void ResetEvaluator(SharpNeatLib.NeuralNetwork.IActivationFunction activationFn)
        {
              //populationEval = new SingleFilePopulationEvaluator(new NetworkEvaluator(simExp),null);
   		      populationEval = new MultiThreadedPopulationEvaluator(new NetworkEvaluator(simExp),null);
		}

        public int InputNeuronCount
        {
            get { return inputs; }
        }

        public int OutputNeuronCount
        {
            get { return outputs; }
        }

        // Schrum: Added
        public int OutputsPerPolicy
        {
            get { return outputsPerPolicy; }
        }

        public SharpNeatLib.Evolution.NeatParameters DefaultNeatParameters
        {
            get
            {
                if (neatParams == null)
                {
                    NeatParameters np = new NeatParameters();
                    np.connectionWeightRange = 3;
                    np.pMutateAddConnection = .03;
                    np.pMutateAddNode = .01;

                    np.pMutateAddModule = 0;

                    np.pMutateConnectionWeights = .96;
                    np.pMutateDeleteConnection = 0;
                    np.pMutateDeleteSimpleNeuron = 0;
                    np.activationProbabilities = new double[4];
                    np.activationProbabilities[0] = .25;
                    np.activationProbabilities[1] = .25;
                    np.activationProbabilities[2] = .25;
                    np.activationProbabilities[3] = .25;
                    np.populationSize = 500;
                    np.pruningPhaseBeginComplexityThreshold = float.MaxValue;
                    np.pruningPhaseBeginFitnessStagnationThreshold = int.MaxValue;
                    np.pruningPhaseEndComplexityStagnationThreshold = int.MinValue;
                    np.pInitialPopulationInterconnections = 1;
                    np.elitismProportion = .2;
                    np.targetSpeciesCountMax = np.populationSize / 10;
                    np.targetSpeciesCountMin = np.populationSize / 10 - 2;
                    np.selectionProportion = .8;
                    np.multiobjective = simExp.multiobjective;
                    neatParams = np;
                }
                return neatParams;
            }
        }

        public SharpNeatLib.NeuralNetwork.IActivationFunction SuggestedActivationFunction
        {
            get { return HyperNEATParameters.substrateActivationFunction; }
        }

        public AbstractExperimentView CreateExperimentView()
        {
            return null;
        }

        public string ExplanatoryText
        {
            get { return "Experiment to interface with the simulator code"; }
        }

        #endregion
    }
}
