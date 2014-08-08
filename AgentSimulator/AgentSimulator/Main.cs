using System;
using System.Windows.Forms;
using Engine;
using SharpNeatLib.CPPNs;
using SharpNeatLib;
using SharpNeatLib.NeatGenome.Xml;
using System.Collections.Generic;
using SharpNeatLib.NeatGenome;


namespace PackbotExperiment
{
    class MainClass
    {
        /**
         * Example command line usage:
         * 
         *  evolve -experiment hardmaze_exp.xml -es true
         *  evolve -experiment roomclear_exp2.xml -seed seedGenome2x.xml -multiobjective true
         *  
         **/
       
        public static string info = "MultiAgent-HyperSharpNEAT Simulator v1.0";

        [STAThreadAttribute]
        public static void Main(string[] args)
        {
            string folder = "";
            string environment_name = null;// "hallway.xml";
            string substrate_name = null;// "substrate.xml";
            string filename = null; // "seedGenome2x.xml";
            string to_eval = null;
            bool homogenous=true;// = true;
            bool overrideHomogenousSetting = false;
            int generations = 1000, populationSize = 0; //was 500
            bool novelty = false;
			bool eval = false;
            bool evolveSubstrate = false;
			bool leo_setting=false;
			bool overrideLeo=false;
            bool overrideInputDensity = false;
            bool overrideHiddenDensity = false;
            bool overrideFitnessFunction = false;
			bool overrideBehaviorCharacterization=false;
			bool seed_population=false;
            bool overrideAgentCount = false;
			bool multiobjective=false;
            int input_density_override = -1;
            int hidden_density_override = -1;
            int agent_count_override = -1;
            string fitness_function_override = "";
			string behavior_override="";
            string experimentName = null;
			bool benchmark=false;
            bool overrideES = false;

            if (args.Length!=0 && args[0] == "-help")
            {
                showHelp();
                return;
            }
            //evolve -experiment multiagent_exp.xml -seed seedGenome2x.xml
            if (!(args.Length == 0) && args[0] == "evolve")
            {
                for (int j = 1; j < args.Length; j++)
                {
                    if (j <= args.Length - 2)
                        switch (args[j])
                        {
						    case "-benchmark":
								benchmark=true;
								break;
						    case "-multiobjective":
						        multiobjective=true;
						        break;
                            case "-es":
                                evolveSubstrate = Convert.ToBoolean(args[++j]);
                                overrideES = true;
                                Console.WriteLine("Evolvable-substrate: " + evolveSubstrate);
                                break;
						    case "-leo":
						        leo_setting=Convert.ToBoolean(args[++j]);
						        overrideLeo=true;
								break;
                            case "-experiment":
                                experimentName = args[++j];
                                break;
                            case "-homogenous":    
                                homogenous = Convert.ToBoolean(args[++j]);
                                overrideHomogenousSetting = true;
                                break;
                            case "-populationSize": if (!int.TryParse(args[++j], out populationSize))
                                    Console.WriteLine("Invalid number of runs specified.");
                                break;

                            case "-generations": if (!int.TryParse(args[++j], out generations))
                                    Console.WriteLine("Invalid number of generations specified.");
                                break;
                            case "-agent_count":
                                overrideAgentCount = true;
                                agent_count_override = Convert.ToInt32(args[++j]);
                                break;
                            case "-novelty":
                                novelty = true;
                                Console.WriteLine("Novelty search enabled...");
                                //TODO   j++;
                                break;
                            case "-fitness_function":
                                overrideFitnessFunction = true;
                                fitness_function_override = args[++j];
                                Console.WriteLine("Setting fitness function to " + fitness_function_override);
                                break;
						    case "-behavior_characterization":
						        overrideBehaviorCharacterization = true;
								behavior_override = args[++j];
						        Console.WriteLine("Setting behavior characterization to " + behavior_override);
                                break;
                            case "-hidden_density":
                                overrideHiddenDensity = true;
                                hidden_density_override = Convert.ToInt32(args[++j]);
                                Console.WriteLine("Setting hidden density to " + hidden_density_override);
                                break;

                            case "-input_density":
                                overrideInputDensity = true;
                                input_density_override = Convert.ToInt32(args[++j]);
                                Console.WriteLine("Setting rangefinder input density to " + input_density_override);
                                break;

                            case "-rng_seed": int seed = Convert.ToInt32(args[++j]);
                                Utilities.random.Reinitialise(seed);
                                Console.WriteLine("Using RNG seed " + seed);
                                break;

                            case "-environment": environment_name = args[++j];
                                Console.WriteLine("Using environment " + environment_name);
                                break;

                            case "-substrate": substrate_name = args[++j];
                                Console.WriteLine("Using substrate " + substrate_name);
                                break;

                            case "-eval": to_eval = args[++j];
								eval=true;
                                Console.WriteLine("Attempting to evaluate file " + to_eval);
                                break;

                            case "-seed": filename = args[++j];
                                Console.WriteLine("Attempting to use seed from file " + filename);
                                break;

							case "-seed_pop": filename = args[++j];
						        seed_population=true;
							    Console.WriteLine("Attempting to use seed population from file " + filename);
                            	break;
						
                            case "-folder": folder = args[++j];
                                Console.WriteLine("Attempting to output to folder " + folder);
                                break;
                        }
                }

                if (!homogenous && evolveSubstrate)
                {
                    Console.WriteLine("Evolvalbe-Substrate not supported for heterogenous teams");
                    return;

                }

                if (experimentName == null)
                {
                    Console.WriteLine("Missing [experimentName].");
                    Console.WriteLine("See help \"-help\"");
                    return;
                }
				
                ExperimentWrapper wr = ExperimentWrapper.load(experimentName);
                SimulatorExperiment experiment = wr.experiment;

                if(populationSize!=0)
                    experiment.populationSize = populationSize;
				else
					populationSize=experiment.populationSize;
				
                if (overrideHomogenousSetting)
                {
                    if (experiment is MultiAgentExperiment)
                        ((MultiAgentExperiment)experiment).homogeneousTeam = homogenous;
                }

                if (environment_name != null)
                {
                    experiment.environmentName = environment_name;
                    experiment.environmentList.Add(Engine.Environment.loadEnvironment(environment_name));
                }
				
                //change sensor density if requested
                if (overrideInputDensity)
                    experiment.rangefinderSensorDensity = input_density_override;

                if (overrideAgentCount)
                {
                    if (experiment is MultiAgentExperiment)
                        ((MultiAgentExperiment)experiment).numberRobots = agent_count_override;
                }

				
                if (substrate_name != null)
                    experiment.substrateDescription = new SubstrateDescription(substrate_name);

				

                //  se.substrateDescription = substrate;
                if (filename != null)
				if (!seed_population)
                {
                    experiment.loadGenome(filename);
                }
				
				
				
				if (eval)
				{
					experiment.loadGenome(to_eval);
					experiment.initialize();
					if(benchmark) {
						experiment.timesToRunEnvironments=6; //was 25
						MultiAgentExperiment exp= (MultiAgentExperiment)experiment;
						exp.benchmark=true;
					}
					Engine.NetworkEvaluator x = new Engine.NetworkEvaluator(experiment);
					SharpNeatLib.BehaviorType behavior;
					Console.WriteLine("Fitness score:" + x.EvaluateNetwork(experiment.genome.Decode(null),out behavior));
					Console.Write("Behavior: ");
					foreach (double d in behavior.behaviorList) {
						Console.Write(d+ " ");
					}
					Console.WriteLine();
					Console.Write("MO: ");
					foreach (double d in behavior.objectives) {
						Console.Write(d+" ");	
					}
					return;
					}

				
                experiment.initialize();

                if (experiment.adaptableANN && evolveSubstrate)
                {
                    Console.WriteLine("Right now ES-HyperNEAT does not support plastic ANNs. Disable adaptation or use the fixed-substrate HyperNEAT");
                    return;
                }

				if(overrideLeo) 
					experiment.substrateDescription.useLeo=leo_setting;

				
				//change fitness function if requested
                if (overrideFitnessFunction)
                    experiment.setFitnessFunction(fitness_function_override);

				if (overrideBehaviorCharacterization)
					experiment.setBehavioralCharacterization(behavior_override);

                if (overrideES)
                {
                    experiment.evolveSubstrate = evolveSubstrate;
                }

                //make sure substrate's input density matches # of inputs in environment
                uint number_of_sensors = Convert.ToUInt32(experiment.rangefinderSensorDensity);

                uint dx = 0, dy = 0;
				/*
                experiment.substrateDescription.getNeuronDensity(0, out dx, out dy);

                //TODO maybe updateInputDensity of AgentBrain needs to be called

                if (dx != number_of_sensors)
                {
                    Console.WriteLine("Input density forced to match number of sensors.");
                    Console.WriteLine("Input Density beforehand: " + dx.ToString() + " " + dy.ToString());
                    experiment.substrateDescription.setNeuronDensity(0, number_of_sensors, 1);
                    experiment.substrateDescription.getNeuronDensity(0, out dx, out dy);
                    Console.WriteLine("Input Density afterwards: " + dx.ToString() + " " + dy.ToString());
                }
				 */
                if (overrideHiddenDensity)
                {
                    experiment.substrateDescription.getNeuronDensity(1, out dx, out dy);
                    Console.WriteLine("Hidden Density beforehand: " + dx.ToString() + " " + dy.ToString());
                    experiment.substrateDescription.setNeuronDensity(0, Convert.ToUInt32(hidden_density_override), 1);
                    experiment.substrateDescription.getNeuronDensity(0, out dx, out dy);
                    Console.WriteLine("Hidden Density afterwards: " + dx.ToString() + " " + dy.ToString());
                }

                HyperNEATEvolver evolve = new HyperNEATEvolver(experiment);

				if(novelty) {
					evolve.enableNoveltySearch(true);
				}

				if(multiobjective)
					evolve.experiment.DefaultNeatParameters.multiobjective=true;
				
				evolve.setOutputFolder(folder);
                
				if(filename!=null)
				{
					if(seed_population) {
					evolve.initializeEvolutionFromPopFile(filename);
					}
					else {
                	evolve.initializeEvolution(populationSize,experiment.genome);
					}
				}
				else
					evolve.initializeEvolution(populationSize);


                evolve.evolve(generations);
				/*for (int i = 0; i < generations; i++)
                {                    
                    evolve.oneGeneration(i);
                }*/
            }
            else
            {
                experimentName = "hardmaze_exp.xml";      //Default experiment 
                //"hardmaze_exp.xml";//

                for (int j = 0; j < args.Length; j++)
                {
                    if (j <= args.Length - 2)
                        switch (args[j])
                        {
                            case "-experiment":
                                experimentName = args[++j];
                                break;
							case "-genome":
								filename=args[++j];
						        break;
	                      }
                }
                SimulatorVisualizer vis = new SimulatorVisualizer(experimentName,filename);
                Application.Run(vis);
            }
        }


        public static void showHelp()
        {
            Console.WriteLine(info);
            Console.WriteLine("If called with \"evolve\" the command line tool is used. Otherwise the visual simulator is started.");
            Console.WriteLine();
            Console.WriteLine("-experiment [filename]       Load this experiment.");
            Console.WriteLine("-homogenous [true/false]     If true use homogenous teams otherwise heterogenous.");
            Console.WriteLine("-populationSize [number]");
            Console.WriteLine("-generations [number]");
            Console.WriteLine("-agent_count [number]");
            Console.WriteLine("-novelty                     Use novelty search");
            Console.WriteLine("-es                          Enable evolvable-substrate");
            Console.WriteLine("-fitness_function [filename]");
            Console.WriteLine("-hidden_density [number]     Specifies the number of hidden neurons");
            Console.WriteLine("-input_density [number]      Specifies the number of input neurons");
            Console.WriteLine("-rng_seed [number]           What random seed should be used.");
            Console.WriteLine("-environment [filename]");
            Console.WriteLine("-substrate [filename]");
            Console.WriteLine("-eval [filename]             Evaluate the given genome and return fitness");
            Console.WriteLine("-seed [filename]             Seed evolution with the given genome");
			Console.WriteLine("-seed_pop [filename]         Seed evolution with the given population");
            Console.WriteLine("-folder [name]               Output files to the specified folder");
            Console.WriteLine("-multiobjective              Turns on multiobjective search");
            Console.WriteLine("\nExample usage: AgentSimulator.exe evolve -experiment hardmaze_exp.xml");
            
        }
    }
}