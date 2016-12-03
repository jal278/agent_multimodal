 using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.CPPNs;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Experiments;
using System.Drawing;
using System.Xml;
using SharpNeatLib.NeatGenome.Xml;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using simulator.Experiments;

namespace Engine
{
		public class instance_pack {
	        public AgentBrain agentBrain;
    	 	public CollisionManager collisionManager;
			public List<Robot> robots;
		    public int num_rbts;
			public IFitnessFunction ff;
			public IBehaviorCharacterization bc;
			public Environment env;
			public int eval;
			public double elapsed;
			public int timeSteps;
			public double timestep;
			public instance_pack() {
				timeSteps=0;
				timestep=1.0;
			}
		}
		
   public class SimulatorExperiment : SimulatorExperimentInterface
    {

       // Schrum: Copied in from MultiAgentExperiment
        [XmlIgnore]
        public double elapsed;              //Elapsed simulation time

        [XmlIgnore]
        public AgentBrain agentBrain;
        [XmlIgnore]
        public SubstrateDescription substrateDescription;
        [XmlIgnore]
        public CollisionManager collisionManager;
        public bool gridCollision = false;


        public bool homogeneousTeam = true;

        public bool multibrain = false;

       // Schrum: Applies to domains with multiple environments. Default is the average.
        public bool multiplicativeFitness = false;

        // Schrum: punish CPPN modules when using Module Mutation; must use with multiobjective = true
        public bool cppnModuleCost = true;

       // Schrum: Added by me: Default number of brains is 1
        public int numBrains = 1;
       // Schrum: Added: Whether the CPPN can have a separate weight output for each of multiple brains
        public bool ungeometricBrains = false; // Schrum: Different brains do not have geometric relation (policy geometry)
       // Schrum: Added: Whether the CPPN has an output that is dedicated to defining preference neuron links, that distinguishes it more
        public bool dedicatedPreferenceOutputInCPPN = false;
       // Schrum: Added: Forcing situational policy geometry means the situation inputs simply go up from 0
        public bool forcedSituationalPolicyGeometry = false;

       // Schrum: Starting location of EnemyRobot, if there is one.
        public double enemyX = 0;
        public double enemyY = 0;

       // Schrum: offset position for multiple agents
        public double enemyDeltaX = 0;
        public double enemyDeltaY = 100;

       // Schrum: different enemy setups
        public bool enemiesSurround = false; // If multiple enemies in circle
        public bool flee = false; // EnemyRobots run from evolved bot
       
       // Schrum: Preference neurons in the substrate networks can be checked to see which of multiple brains to use
        public bool preferenceNeurons = false;

       // Schrum: Decided to put Module Mutation rates here for now ... may change
        public double mmpRate = 0.0;
        public double mmrRate = 0.0;
        public double mmdRate = 0.0;

       // Schrum: Number of enemy robots for experiments that have them
        public int numEnemies = 0;

        public string substrateDescriptionFilename;

        public bool multiobjective = false;

        public bool evolveSubstrate;

       //Is the ANN adatable
        public bool adaptableANN        = false;
       //Use neuromodulation
        public bool modulatoryANN       = false;
       //If true normalizes the weights of the ANN
        public bool normalizeWeights    = true;

       // public int inputs, outputs;
        public int populationSize = 150;

        //The different types of noise
        public float headingNoise   = 0.0f;
        public float effectorNoise  = 0.0f;
        public float sensorNoise    = 0.0f;

       //How many times should the environment be run
        public int timesToRunEnvironments = 1;
        public int evaluationTime;

        [XmlIgnore]
        public bool initialized;

        public double timestep;
        public string explanatoryText;
        public string scriptFile;
        public String robotModelName;
        public string fitnessFunctionName;
		public string behaviorCharacterizationName;

        [XmlIgnore]
        public IFitnessFunction fitnessFunction;

        [XmlIgnore]
        public IBehaviorCharacterization behaviorCharacterization;

        //The current environment
        [XmlIgnore]
        public Environment environment;
        [XmlIgnore]
        public List<Environment> environmentList = new List<Environment>();
        [XmlIgnore]
        public int timeSteps = 0;

        public string environmentName;

        [XmlIgnore]
        public NeatGenome bestGenomeSoFar;

        //Robots
        [XmlIgnore]
        public List<Robot> robots = new List<Robot>();

      //  public bool overrideDefaultSensorDensity = false;
        public int rangefinderSensorDensity;

        public bool overrideTeamFormation = false;
        public float group_orientation;
        public float group_spacing;
        public int robot_heading;

        public bool useScript = false;  
        [XmlIgnore] 
        public bool running;        //is the experiment running?

        ////genome
        [XmlIgnore]
        public NeatGenome genome;
        [XmlIgnore]
        public string genomeFilename;

        public void loadGenome(String filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            genome = XmlNeatGenomeReaderStatic.Read(doc);
            bestGenomeSoFar = genome;
            genomeFilename = filename;
		    //TODO maybe include	initialize(); 
        }

		public void setBehavioralCharacterization(string behaviorName)
		{
            if (behaviorName == null || behaviorName.Equals("null")) return;
			behaviorCharacterization = BehaviorCharacterizationFactory.getBehaviorCharacterization(behaviorName);
			behaviorCharacterization.reset();
		}

        public void setFitnessFunction(string fitnessFunctionName)
        {
            this.fitnessFunctionName = fitnessFunctionName;
            fitnessFunction = FitnessFunctionFactory.getFitnessFunction(fitnessFunctionName);
            fitnessFunction.reset();
        }

        public void loadEnvironment(String environment_name)
        {
            environmentName = environment_name;
			environment=Environment.loadEnvironment(environment_name);
        }

        public void clearEnvironment()
        {
            environmentName = "Unnammed";
            environment.reset();
        }

       // Schrum: Changed "internal" to "public". Probably a bad idea, but it works!
        public virtual double evaluateNetwork(INetwork network, out SharpNeatLib.BehaviorType behavior, System.Threading.Semaphore sem) { behavior = null;  return 0.0; }

        public void resetEnvironment()
        {
            environment.reset();
        }
        public virtual String defaultSeedGenome()
        {
            return null;
        }

        // Schrum: had to make this method "virtual" to allow it to be overridden in MultiAgentExperiment
        public virtual int getNumCPPNInputs()
        {
            int inputs;

            inputs = 4; // Base inputs: x1, y1, x2, y2

            // Schrum: Z-coordinate input now handled by overridden version of this
            // method in MultiAgentExperiment

            // Schrum: Check for signal from situational policy geometry.
            //         Make sure there are actually multiple brains, and
            //         make sure that the brains depend on policy geometry.
            if (this.multibrain && this.numBrains > 1 && !this.ungeometricBrains)
            {
                //Console.WriteLine("Include situation input S");
                inputs++; // Schrum: S-input that determines which brain is being made
            }

            return inputs;

            // Schrum: original definition below. Did not account for situational policy geometry input
            //if (homogeneousTeam)
            //    inputs = 4; // Schrum: Typical (x1,y1) and (x2,y2) coordinates
            //else
            //    inputs = 5; // Schrum: (x1,y1) and (x2,y2) plus a z-coordinate (improperly used constant z = 0 in some experiments?)
            //return inputs;
        }

        // Schrum: This used to be the original getNumCPPNOutputs() method, but now it
        // is used to identify the number of outputs per output module.
        public int getNumCPPNOutputsPerModule()
        {
            int outputs;

            if (adaptableANN)
            {
                if (modulatoryANN) outputs = 8; else outputs = 7;
            }
            else if (dedicatedPreferenceOutputInCPPN)
                outputs = 3; // Schrum: CPPN output used to define preference neuron link weights
            else
                outputs = 2; // Schrum: Default ... why 2 again?
            return outputs;
        }

        // Schrum: This function simply calls getNumCPPNOutputsPerModule(), but alters the
        // result if multiple brains need to be created by different output modules of the CPPN.
        public int getNumCPPNOutputs()
        {
            int outputs = getNumCPPNOutputsPerModule();
            // Schrum: Create multiple brains, each geometrically independent of the others
            if (ungeometricBrains && numBrains > 1)
                outputs *= numBrains; // Schrum: One set of outputs for each brain

            // Schrum: debugging
            //Console.WriteLine("ungeometricBrains:" + ungeometricBrains + ", Num CPPN outputs: " + outputs);
            return outputs;
        }

        #region Virtual Methods
        public virtual void run() { }
        
        public virtual void draw(Graphics g, CoordinateFrame scale) { }

        public virtual void initialize() {}
        #endregion

        #region XML Serialization
        //Save the environment to XML
        public virtual void save(string name)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());
            TextWriter outfile = new StreamWriter(name);
            x.Serialize(outfile, this);
            outfile.Close();
        }
        /*
        //Loads a environment from an XML file and initializes it
        public SimulatorExperiment load(string name)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SimulatorExperiment));
            TextReader infile = new StreamReader(name);
            SimulatorExperiment e = (SimulatorExperiment)x.Deserialize(infile);
            infile.Close();

            e.substrateDescription = new SubstrateDescription(e.substrateDescriptionFilename);

            e.fitnessFunction = FitnessFunctionFactory.getFitnessFunction(e.fitnessFunctionName);

            loadEnvironments(e);
            
            return e;
        }*/

        public static void loadEnvironments(SimulatorExperiment e)
        {
            //Only reload environment if it isn't loaded yet, otherwise changes to the environment are lost
            //TODO make sure
            if (e.environmentList.Count == 0)
            {
                //e.environmentList.Clear();
                Engine.Environment scene = Engine.Environment.loadEnvironment(e.environmentName);
                e.environmentList.Add(scene);

                e.environment = scene;

                Console.Write("Looking for additional environments [" + scene.name + "] ... ");
                String filenamePrefix = scene.name.Substring(0, scene.name.Length - 4);
                int num = 1;
                String filename2 = filenamePrefix + num + ".xml";
                while (File.Exists(filename2))
                {
                    Console.WriteLine("Found environment: " + filename2 + "\n");
                    e.environmentList.Add(Engine.Environment.loadEnvironment(filename2));
                    num++;
                    filename2 = filenamePrefix + num + ".xml";
                }
                Console.WriteLine("Done");
                Console.WriteLine(e.environmentList.Count.ToString() + " environment(s) found.");
            }
        }

        public virtual void initializeRobots(AgentBrain agentBrain, Environment e, float headingNoise, float sensorNoise, float effectorNoise, instance_pack x) { }

        #endregion
    }


}
