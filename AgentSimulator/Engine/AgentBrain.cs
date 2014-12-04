using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;
using Engine;
using SharpNeatLib.NeatGenome;
using System.Xml;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeatGenome.Xml;
using System.IO;

namespace Engine
{
    //Abstracts away neural networks and heterogeneity
    public class AgentBrain
    {
        //How many times should the network get activated. TODO make configurable
        private static int NET_ACTIVATION_STEPS = 2;
        // private bool EVOLVED_SUBSTRATE = true;

        //The "big" brain for heterogenous teams
        public INetwork brain;

        //Brains for homogenous teams
        public List<INetwork> brains;

        //Homogenous team?
        public bool homogenous;
        public int numRobots;
        private List<Robot> robotListeners;
        private bool allActivated;

        private float[] teamInput;
        private bool[] activated;

        private SubstrateDescription substrateDescription;
        public INetwork genome;
        public NeatGenome ANN;

        private bool normalizeANNWeights;
        private bool adaptableANN;
        private bool modulatoryANN;

        // Schrum: Added
        public bool preferenceNeurons;

        public bool multipleBrains = false;
        public bool evolveSubstrate = false;

        public List<INetwork> multiBrains = new List<INetwork>();

        public List<float> zcoordinates;

        // Schrum: Identify current brain from multiBrains
        private int brainCounter = 0;
        public int numBrains; // Schrum: Total number

        public AgentBrain(bool homogenous, int numAgents, SubstrateDescription substrateDescription, INetwork genome,
            bool normalizeANNWeights, bool adaptableANN, bool modulatoryANN, bool multi, int brains, bool evolveSubstrate, bool preferenceNeurons)
        {
            this.evolveSubstrate = evolveSubstrate;
            this.normalizeANNWeights = normalizeANNWeights;
            this.adaptableANN = adaptableANN;
            this.modulatoryANN = modulatoryANN;
            this.genome = genome;
            this.substrateDescription = substrateDescription;
            this.numRobots = numAgents;
            this.homogenous = homogenous;
            this.multipleBrains = multi;
            // Schrum: When preference neurons are used, the number of modules in the network is the
            // more reliable source of information. Especially if Module Mutation will allow more modules
            // to be created, each creating a new brain.
            this.numBrains = genome != null && preferenceNeurons ? genome.NumOutputModules : brains;
            this.preferenceNeurons = preferenceNeurons;

            //inputCounter = 0;
            teamInput = new float[numAgents * substrateDescription.InputCount];
            activated = new bool[numAgents];

            createBrains();

            robotListeners = new List<Robot>();
        }

        // Schrum: Get brainCounter value
        public int getBrainCounter()
        {
            return brainCounter;
        }

        // Schrum: New method to specifically choose the brain to use (assuming only one is being used)
        public void switchBrains(int index)
        {
            //Console.WriteLine("switchBrains(" + index + ")");
            //Console.WriteLine(multipleBrains + " && " + (index >= 0) + " && " + multiBrains.Count);
            if (multipleBrains && index >= 0 && index < multiBrains.Count)
            {
                //Console.WriteLine("Switch to brain: " + index + " out of " + multiBrains.Count);
                brainCounter = index;
                brain = multiBrains[index];
            }
        }

        // Schrum: This is the original version of this method, which is only kept for backwards compatability
        public void switchBrains()
        {
            if (multipleBrains)
            {
                brainCounter = 1;
                brain = multiBrains[brainCounter];
            }
        }

        private void createBrains()
        {
            if (genome != null)
            {
                //Schrum: debugging
                //Console.WriteLine("genome != null");
                if (homogenous)
                {
                    //Schrum: debugging
                    //Console.WriteLine("homogenous");

                    ANN = substrateDescription.generateHomogeneousGenome(genome, normalizeANNWeights, this.adaptableANN, this.modulatoryANN, evolveSubstrate);
                    brains = new List<INetwork>();
                    for (int i = 0; i < numRobots; i++)
                    {
                        INetwork b = ANN.Decode(null);
                        brains.Add(b);
                    }
                }
                else
                {
                    //Schrum: debugging
                    //Console.WriteLine("!homogenous");
                    if (multipleBrains) //Multiple brains with situational policies
                    {
                        //Schrum: debugging
                        //Console.WriteLine("multipleBrains");

                        // Schrum: The original code hard codes "2" as the nubmer of brains for any multi brain scenario TODO
                        //List<NeatGenome> genes = substrateDescription.generateGenomeStackSituationalPolicy(genome, Convert.ToUInt32(numRobots), normalizeANNWeights, adaptableANN, modulatoryANN, 2, out zcoordinates);
                        List<NeatGenome> genes = substrateDescription.generateGenomeStackSituationalPolicy(genome, Convert.ToUInt32(numRobots), normalizeANNWeights, adaptableANN, modulatoryANN, numBrains, out zcoordinates);

                        for (int j = 0; j < genes.Count; j++)
                        {
                            multiBrains.Add(genes[j].Decode(null));
                        }

                        // Schrum: for debugging
                        /*
                        for (int j = 0; j < multiBrains.Count; j++)
                        {
                            if (multiBrains[j] == null)
                            {
                                Console.WriteLine(j +": Null brain!");
                            }
                            else
                            {
                                Console.WriteLine(j + ":" + multiBrains[j]);
                                Console.WriteLine(j + ":CurrentOutputModule:" + multiBrains[j].CurrentOutputModule);
                                Console.WriteLine(j + ":InputNeuronCount:" + multiBrains[j].InputNeuronCount);
                                Console.WriteLine(j + ":OutputNeuronCount:" + multiBrains[j].OutputNeuronCount);
                                Console.WriteLine(j + ":OutputsPerPolicy:" + multiBrains[j].OutputsPerPolicy);
                                Console.WriteLine(j + ":TotalNeuronCount:" + multiBrains[j].TotalNeuronCount);
                                Console.WriteLine(j + ":NumOutputModules:" + multiBrains[j].NumOutputModules);
                            }
                        }
                        */

                        brain = multiBrains[0];

                    }
                    else
                    {
                        //Schrum: debugging
                        //Console.WriteLine("!multipleBrains");

                        ANN = substrateDescription.generateMultiGenomeStack(genome, Convert.ToUInt32(numRobots), normalizeANNWeights, adaptableANN,
                                                                        modulatoryANN, out zcoordinates, evolveSubstrate);
                        brain = ANN.Decode(null);
                    }
                }
            }
        }

        //Needs to be called if the number of sensors changes
        //Right now all brains have the same number of inputs
        public void updateInputDensity()
        {
            // Schrum: Debug
            //Console.WriteLine("substrateDescription.InputCount = " + substrateDescription.InputCount);
            teamInput = new float[numRobots * substrateDescription.InputCount];
            //Check if the number of sensors changed so we have to regenerate the ANNs
            if (homogenous)
            {
                if (brains != null && brains[0] != null && brains[0].InputNeuronCount != substrateDescription.InputCount)
                {
                    Console.WriteLine("Recreating ANNs");
                    createBrains();
                }
            }
            else
            {
                if (brain != null && (brain.InputNeuronCount / numRobots) != substrateDescription.InputCount)
                {
                    Console.WriteLine("Recreating ANNs");
                    createBrains();
                }

            }
        }

        public void clearRobotListeners()
        {
            robotListeners.Clear();
        }

        //Robots need to be registered before they can receive ANN results
        public void registerRobot(Robot robot)
        {
            if (robotListeners.Contains(robot))
            {
                Console.WriteLine("Robot " + robot.id + " already registered");
                return;
            }
            robotListeners.Add(robot);
            if (robotListeners.Count > numRobots)
            {
                Console.WriteLine("Number of registered agents [" + robotListeners.Count + "] and number of agents [" + numRobots + "] does not match");
            }
        }

        public INetwork getBrain(int number)
        {
            if (homogenous)
            {
                if (brains == null) return null;
                return brains[number];
            }
            else
                return brain;   //only one brain for heterogenous teams
        }

        //Clear the listener list
        public void reset()
        {
            robotListeners.Clear();
        }

        //call once all agenht received their inputs
        public void execute(System.Threading.Semaphore sem)
        {
            //return;
            // Schrum: None of the multibrain features have been implemented in
            //         homogeneous teams.
            if (homogenous)
            {
                float[] inputs = new float[substrateDescription.InputCount];

                for (int agentNumber = 0; agentNumber < numRobots; agentNumber++)
                {
                    //prepare inputs 
                    for (int i = 0; i < substrateDescription.InputCount; i++)
                    {
                        inputs[i] = teamInput[(i + agentNumber * substrateDescription.InputCount)];
                    }

                    brains[agentNumber].SetInputSignals(inputs);
                    brains[agentNumber].MultipleSteps(NET_ACTIVATION_STEPS); //maybe base this on ANN depth

                    float[] outputs = new float[brains[agentNumber].OutputNeuronCount];
                    for (int j = 0; j < outputs.Length; j++)
                    {
                        outputs[j] = brains[agentNumber].GetOutputSignal(j);
                    }

                    robotListeners[agentNumber].networkResults(outputs);
                }
                return;
            }

            if (brain == null)
                return;

            //Heterogenous

            // Schrum: With preference neurons, each brain must execute.
            // Lot of tricky code here that could use some checking.
            if (preferenceNeurons)
            {
                // Schrum: Each robot can have multiple brains.
                // List preferences for the different team members first, then each separate brain
                float[][] preferences = new float[numRobots][];
                for (int j = 0; j < numRobots; j++)
                {
                    preferences[j] = new float[numBrains];
                }
                int numOutputTeam = brain.OutputNeuronCount;
                int numOutputAgent = brain.OutputNeuronCount / numRobots;
                int numPolicyOutputs = numOutputAgent - 1;
                // Schrum: run each brain
                for (int i = 0; i < numBrains; i++)
                {
                    // Schrum: Does teamInput need to be copied? Or is it ok to reuse the same array?
                    // Seems ok to re-use.
                    multiBrains[i].SetInputSignals(teamInput);
                    multiBrains[i].MultipleSteps(NET_ACTIVATION_STEPS);
                    for (int j = 0; j < numRobots; j++)
                    {
                        preferences[j][i] = multiBrains[i].GetOutputSignal((j * numOutputAgent) + numPolicyOutputs);
                    }
                }

                // Schrum: Each robot in the team picks a (possibly different) brain based on preferences
                int[] brainChoices = new int[numRobots];
                for (int i = 0; i < numRobots; i++)
                {
                    brainChoices[i] = argmax(preferences[i]);
                }

                int out_count = 0;
                float[] outp = new float[numPolicyOutputs]; // Schrum: ignore preference neuron
                // Schrum: Not using foreach because I need the index for reference
                for (int i = 0; i < robotListeners.Count; i++)
                {
                    Robot robot = robotListeners[i];
                    // Schrum: Only look at policy outputs (ignore preference neuron)
                    for (int y = 0; y < numPolicyOutputs; y++)
                    {
                        // Schrum: For each output, need to check the brain that the particular robot chose
                        outp[out_count % numPolicyOutputs] = multiBrains[brainChoices[i]].GetOutputSignal(out_count);
                        // Schrum: Make the robot track the brain it is using
                        robot.updateBrainMode(brainChoices[i]);
                        
                        if (Double.IsNaN(outp[out_count % numPolicyOutputs])) // Schrum: Had to change this too
                            Console.WriteLine("NaN in outputs");
                        out_count++;
                    }

                    robot.networkResults(outp);
                }
            }
            else // Schrum: This entire else was the original code without preference neurons
            {
                brain.SetInputSignals(teamInput);
                brain.MultipleSteps(NET_ACTIVATION_STEPS);

                int out_count = 0;
                int numOutputAgent = brain.OutputNeuronCount / numRobots;
                float[] outp = new float[numOutputAgent];
                foreach (Robot robot in robotListeners)
                {
                    for (int y = 0; y < numOutputAgent; y++)
                    {
                        outp[out_count % numOutputAgent] = brain.GetOutputSignal(out_count);

                        if (Double.IsNaN(outp[out_count % numOutputAgent]))
                            Console.WriteLine("NaN in outputs");
                        out_count++;
                    }


                    //              Console.WriteLine("Sending results to " + robot.id);
                    //sem.WaitOne();
                    robot.networkResults(outp);
                    //sem.Release();
                }
                //  Console.WriteLine(outp[0] + " " + outp[1] + " " + outp[2]);
            }


            // Schrum: This debug code was in the orignal, but I broke it and have commented it out
            /*
            bool debug = false;
            if (debug)
            {
                //for(int id=0;id<numRobots;id++) {
                for (int id = 1; id <= 1; id++)
                {
                    Console.Write(id + " INPUTS: ");
                    for (int i = 0; i < substrateDescription.InputCount; i++)
                        Console.Write(teamInput[i + id * substrateDescription.InputCount] + " ");
                    Console.WriteLine();
                    Console.Write(id + " OUTPUTS: ");
                    int biggest = 0;
                    float val = 0.0f;
                    for (int i = 0; i < 3; i++) //numOutputAgent;i++)
                    {
                        float temp = brain.GetOutputSignal(numOutputAgent * id + i);
                        Console.Write(temp + " ");
                        if (temp > val)
                        {
                            biggest = i;
                            val = temp;
                        }
                    }

                    Console.Write(biggest); //brain.GetOutputSignal(numOutputAgent*id+i)+" ");

                    Console.WriteLine();
                }
            }
            */

        }

        // Schrum: Added to find out which module has the highest preference
        public static int argmax(float[] preferences)
        {
            int maxpos = 0;
            for (int i = 1; i < preferences.Length; i++)
            {
                if(preferences[i] > preferences[maxpos]){
                    maxpos = i;
                }
            }
            return maxpos;
        }

        //TODO not really clean
        public void clearANNSignals(float zstack)
        {
            int index = 0;
            ModularNetwork net = ((ModularNetwork)brain);
            foreach (ConnectionGene gene in net.genome.ConnectionGeneList)
            {
                if (gene.coordinates.Length > 4 && !gene.coordinates[4].Equals(float.NaN))
                {
                    if (gene.coordinates[4] != zstack)        //Only valid if robot has z-values
                    {
                        index++;
                        continue;
                    }
                }

                ((ModularNetwork)brain).neuronSignals[net.connections[index].targetNeuronIdx] = 0.0f;
                ((ModularNetwork)brain).neuronSignals[net.connections[index].sourceNeuronIdx] = 0.0f;


            }
        }


        //Activate the agents network with the given inputs
        public void setInputSignals(int agentNumber, float[] inputs)
        {
            // Schrum: Debug
            //Console.WriteLine("inputs.Length = " + inputs.Length);
            //Console.WriteLine("teamInput.Length = " + teamInput.Length);
            for (int i = 0; i < inputs.Length; i++)
            {
                teamInput[(i + agentNumber * inputs.Length)] = inputs[i];
            }
        }

        public void setInputSignalsOLD(int agentNumber, float[] inputs)
        {
            if (homogenous)
            {
                brains[agentNumber].SetInputSignals(inputs);
                brains[agentNumber].MultipleSteps(NET_ACTIVATION_STEPS); //maybe base this on ANN depth

                float[] outputs = new float[brains[agentNumber].OutputNeuronCount];
                for (int j = 0; j < outputs.Length; j++)
                {
                    outputs[j] = brains[agentNumber].GetOutputSignal(j);
                }

                robotListeners[agentNumber].networkResults(outputs);

                return;
            }

            if (brain == null)
                return;

            //  Console.WriteLine("Received :"+agentNumber+" input length "+inputs.Length+" inputcounter "+inputCounter);

            for (int i = 0; i < inputs.Length; i++)
            {
                //if (inputCounter != i+agentNumber * inputs.Length)
                //{
                //    Console.WriteLine(inputCounter + " " + (i + agentNumber * inputs.Length));
                //}

                teamInput[(i + agentNumber * inputs.Length)] = inputs[i];
                //inputCounter++;
            }

            activated[agentNumber] = true;

            allActivated = true;
            for (int i = 0; i < activated.Length; i++)
            {
                if (!activated[i])
                {
                    allActivated = false;
                    break;
                }
            }

            if (allActivated)       //weight for all ANN to get activated before sending out the networkResult event
            {
                //             Console.WriteLine("All agents activated");
                allActivated = false;
                //   inputCounter = 0;
                for (int i = 0; i < activated.Length; i++)
                {
                    activated[i] = false;
                }

                brain.SetInputSignals(teamInput);
                brain.MultipleSteps(NET_ACTIVATION_STEPS);

                int out_count = 0;
                int numOutputAgent = brain.OutputNeuronCount / numRobots;
                float[] outputs = new float[numOutputAgent];
                foreach (Robot robot in robotListeners)
                {
                    for (int y = 0; y < numOutputAgent; y++)
                    {
                        outputs[out_count % numOutputAgent] = brain.GetOutputSignal(out_count);

                        if (Double.IsNaN(outputs[out_count % numOutputAgent]))
                            Console.WriteLine("NaN in outputs");
                        out_count++;
                    }

                    //              Console.WriteLine("Sending results to " + robot.id);
                    robot.networkResults(outputs);
                }
            }
        }
    }
}