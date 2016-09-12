using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeatGenome.Xml;
using System.Xml;
using SharpNeatLib.CPPNs;
using SharpNeatLib.Experiments;
using System.Drawing;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace Engine
{
    //Basic Multi-Agent simulator experiment
    public class MultiAgentExperiment : SimulatorExperiment
    {
        public bool collisionPenalty;
        public bool multipleEnvironments;   //Evaluation over multiple environments?
        public bool modulatory;
        public bool benchmark;

        // Schrum: Removed this to move it into SimulatorExperiment
        //[XmlIgnore]
        //public double elapsed;              //Elapsed simulation time

        public int numberRobots;
        public bool agentsVisible;          //Can the agents see each other
        public bool agentsCollide;          //Do the agents collide with each other



        public MultiAgentExperiment()
        {
            benchmark = false;
            multibrain = false;
            numBrains = 1; // Schrum: default
            evolveSubstrate = false;

            collisionPenalty = false;
            //heterogenous trams by default
            homogeneousTeam = false;

            scriptFile = "";
            numberRobots = 1;
            //how many range finders
            rangefinderSensorDensity = 5;
            timestep = 0.16f;

            evaluationTime = 100;

            explanatoryText = "Multi-Agent Experiment";
            //Environment = new List<Environment>();

            //Create an empty environment
            environment = new Environment();
            robots = new List<Robot>();

            fitnessFunctionName = "SingleGoalPoint";
            behaviorCharacterizationName = "EndPointBC";
            robotModelName = "PackBotModel";

            initialized = false;
            running = false;
        }

        // Schrum: Added to add the Z coordinate input when using a heterogeneous team
        public override int getNumCPPNInputs()
        {
            // Schrum: debug
            //Console.WriteLine("Multiagent experiment");

            int inputs = base.getNumCPPNInputs();
            // Schrum: Homogeneous team doesn't need Z input because each robot is the same.
            //         homogeneousTeam may equal false even when there is no team. Don't add
            //         a Z input if there is only one robot, because the Z coordinate is not needed.
            if (!homogeneousTeam && this.numberRobots > 1)
            {
                //Console.WriteLine("Include Z-coord");
                inputs++; // Schrum: Z-coordinate for specific team member (aka Stack coord)
            }

            return inputs;
        }

        public override void initialize()
        {
            setupVariables();

            initialized = true;
        }

        protected void setupVariables()
        {

            robots = new List<Robot>();
            // Schrum: more special handling: Don't want to overwrite substrate in visual mode
            if(!(fitnessFunction is FourTasksFitness))
                substrateDescription = new SubstrateDescription(substrateDescriptionFilename);

            agentBrain = new AgentBrain(homogeneousTeam, numberRobots, substrateDescription, genome != null ? genome.Decode(null) : null, normalizeWeights, adaptableANN, modulatoryANN, multibrain, numBrains, evolveSubstrate, preferenceNeurons, forcedSituationalPolicyGeometry);

            //if(homogeneousTeam)
            //Console.WriteLine("inc:"+agentBrain.getBrain(0).InputNeuronCount);

            loadEnvironments(this);

            //agentsCollide=environment.agentsCollide;
            //agentsVisible=environment.agentsVisible;

            //Substrate sensor density
            initializeRobots(agentBrain, environment, headingNoise, sensorNoise, effectorNoise, null);

            // Schrum: more special handling: Don't want to overwrite fitness function in visual mode
            if (!(fitnessFunction is FourTasksFitness))
                setFitnessFunction(fitnessFunctionName);
            else // sufficient if FourTasks is already loaded
            {
                fitnessFunction.reset();
                ((FourTasksFitness)fitnessFunction).setupFitness(FourTasksFitness.environmentID(environmentName));
            }
            setBehavioralCharacterization(behaviorCharacterizationName);


            if (gridCollision)
                collisionManager = new GridCollision();
            else
                collisionManager = new StandardCollision();
            collisionManager.Initialize(environment, this, this.robots);
            timeSteps = 0;
            elapsed = 0;

        }

        #region Environment

        public void loadEnvironment(String filename)
        {
            Console.WriteLine("loading environment...");
            environment = Environment.loadEnvironment(filename);
        }

        #endregion

        public override void draw(Graphics g, CoordinateFrame scale)
        {
            if (collisionManager is GridCollision)
            {
                GridCollision gc = (GridCollision)collisionManager;
                gc.draw(g, scale);
            }


            if (environment != null)
            {
                environment.draw(g, scale);
            }

            foreach (Robot robot in robots)
            {
                robot.draw(g, scale);
            }
            double[] obj = null;

            instance_pack ip = new instance_pack();

            ip.robots = robots;
            ip.env = environment;
            ip.timeSteps = this.timeSteps;
            ip.agentBrain = agentBrain;
            ip.collisionManager = collisionManager;
            ip.elapsed = this.elapsed;
            ip.ff = this.fitnessFunction;
            ip.bc = this.behaviorCharacterization;
            ip.timestep = timestep;

            g.DrawString("Fitness: " + this.fitnessFunction.calculate(this, this.environment, ip, out obj), new Font("Tahoma", 12), Brushes.Black, 10, 90);
            g.DrawString("Elapsed time: " + this.elapsed, new Font("Tahoma", 12), Brushes.Black, 10, 60);
        }


        public override void run()
        {

            runEnvironment(environment, null, null);
        }

        //TODO this could be put into a Formation class or something that takes an environment as input
        public override void initializeRobots(AgentBrain agentBrain, Environment e, float headingNoise, float sensorNoise, float effectorNoise, instance_pack ip)
        {
            int num_bots;
            //Set up robots.
            //set up deltas for spacing the robots
            double dx, dy;
            if (overrideTeamFormation)
            {
                dx = Math.Cos(group_orientation / 180.0 * 3.14) * group_spacing;
                dy = Math.Sin(group_orientation / 180.0 * 3.14) * group_spacing;
            }
            else
            {
                dx = Math.Cos(e.group_orientation / 180.0 * 3.14) * e.robot_spacing;
                dy = Math.Sin(e.group_orientation / 180.0 * 3.14) * e.robot_spacing;
            }


            //record z-stack coordinates
            //float zstack = -1.0f;
            //float zdelta = 2.0f / (numberRobots - 1);
            AgentBrain ab;
            List<Robot> rbts;
            double _timestep = 0.0;
            if (ip == null)
            {
                ab = agentBrain;
                rbts = robots;
                num_bots = numberRobots;
                _timestep = this.timestep;
            }
            else
            {
                ab = ip.agentBrain;
                rbts = new List<Robot>();
                ip.robots = rbts;
                num_bots = ip.num_rbts;
                _timestep = ip.timestep;
            }

            rbts.Clear();

            int heading = overrideTeamFormation ? robot_heading : e.robot_heading;

            //add robots in their formation according to environment
            double nx, ny;
            for (int num = 0; num < num_bots; num++)
            {
                Robot r = RobotModelFactory.getRobotModel(robotModelName);
                r.rangefinderDensity = this.rangefinderSensorDensity;

                //TODO: Make a variable that controls this
                /*if (e.POIPosition.Count >= numberRobots)
                {
                    nx = e.POIPosition[num].X;
                    ny = e.POIPosition[num].Y;
                }
                else*/
                {
                    nx = e.start_point.x + num * dx;
                    ny = e.start_point.y + num * dy;
                }
                r.init(num, nx, ny,
                    heading / 180.0 * 3.14, ab, e, sensorNoise, effectorNoise, headingNoise, (float)_timestep);

                r.collisionPenalty = collisionPenalty;
                ab.registerRobot(r);

                //if (overrideDefaultSensorDensity)
                //    r.populateSensors(rangefinderSensorDensity);
                //else
                r.populateSensors();  //TODO populate sensors gets called multiple times. also in robot contructor

                if (agentBrain.zcoordinates == null) r.zstack = 0;
                else
                    r.zstack = ab.zcoordinates[num];
                //zstack;
                rbts.Add(r);

                //zstack += zdelta;
            }

            uint count = 0;
            foreach (ISensor sensor in robots[0].sensors)
            {
                if (sensor is RangeFinder)
                {
                    count++;
                }
            }

            //Console.WriteLine("Range finder count : " + count);
            //By convention the rangefinders are in layer zero so scale that layer
            // Schrum: HOWEVER, if there are no rangefinders, then we do not want to do this
            if (count != 0)
            { // don't rescale rangefinders if there are none.
                substrateDescription.setNeuronDensity(0, count, 1);
            }
            ab.updateInputDensity();
        }

        // Schrum: Removed "internal" from this so I could override it ... probably not the best design decision, but it works.
        //protected internal virtual bool runEnvironment(Environment e, instance_pack ip, System.Threading.Semaphore sem)
        protected virtual bool runEnvironment(Environment e, instance_pack ip, System.Threading.Semaphore sem)
        {
            bool collide;

            if (ip == null)
            {
                elapsed += timestep;
                timeSteps++;
            }
            else
            {
                ip.timeSteps++;
                ip.elapsed += ip.timestep;
                ip.env = e;
            }

            AgentBrain ab;
            CollisionManager cm;
            List<Robot> rbts;
            IFitnessFunction fit_fun;
            IBehaviorCharacterization beh_char;

            if (ip == null)
            {
                // Schrum2: Debug
                //Console.WriteLine("rbts from experiment");

                ip = new instance_pack();
                ab = agentBrain;
                cm = collisionManager;
                rbts = robots;
                fit_fun = fitnessFunction;
                beh_char = behaviorCharacterization;
                ip.agentBrain = agentBrain;
                ip.collisionManager = cm;
                ip.robots = rbts;
                ip.ff = fit_fun;
                ip.env = environment;
                ip.bc = beh_char;
                ip.timeSteps = timeSteps;
                ip.timestep = timestep;
            }
            else
            {
                // Schrum2: Debug
                //Console.WriteLine("rbts from ip");

                ab = ip.agentBrain;
                cm = ip.collisionManager;
                rbts = ip.robots;
                fit_fun = ip.ff;
                beh_char = ip.bc;
            }

            cm.SimulationStepCallback();

            // Schrum2: Debug
            //Console.WriteLine("rbts.Count = " + rbts.Count);

            for (int x = 0; x < rbts.Count; x++)
            {
                if (e.AOIRectangle.Contains((int)rbts[x].location.x, (int)rbts[x].location.y))
                    rbts[x].autopilot = false;
                rbts[x].updateSensors(e, rbts, cm);
                //TODO: Apply input noise?  Maybe should apply it at the sensors (setup with addRobots)
                //TODO unclean. experiment specific
                //if(!rbts[x].autopilot)
                rbts[x].doAction();
                //else
                //	ab.clearANNSignals(rbts[x].zstack);
            }

            ab.execute(sem);

            for (int x = 0; x < rbts.Count; x++)
            {
                collide = cm.RobotCollide(rbts[x]);
                rbts[x].collide_last = collide;
                if (collide)
                    rbts[x].onCollision();
            }

            if (ip != null)
            {
                if (beh_char != null)
                    beh_char.update(this, ip);
                fit_fun.update(this, e, ip);
            }
            return false;
        }

        // Schrum: Had to remove the internal from this too so that I could override it in AdversarialRoomClearingExperiment
        public override double evaluateNetwork(INetwork network, out SharpNeatLib.BehaviorType behavior, System.Threading.Semaphore sem)
        {
            double fitness = 0;
            behavior = new SharpNeatLib.BehaviorType();

            // Schrum: Why is there a magic number 6 here?
            double[] accumObjectives = new double[6];
            for (int i = 0; i < 6; i++) accumObjectives[i] = 0.0;

            // Schrum: Special handling for FourTasks domain again
            if (fitnessFunction is FourTasksFitness)
            {
                // Pass the experiment into the fitness function once so its parameters can be changed for each environment
                ((FourTasksFitness)fitnessFunction).setExperiment(this);
            }

            IFitnessFunction fit_copy;
            IBehaviorCharacterization bc_copy;
            //CollisionManager cm;
            instance_pack inst = new instance_pack();
            int envNum = 0;
            // Schrum: Separate fitness for each environment.
            // Should this be put in accumObjectives instead? What is that for?
            double[] environmentScores = new double[environmentList.Count];
            foreach (Environment env2 in environmentList)
            {
                fit_copy = fitnessFunction.copy();
                if (behaviorCharacterization != null)
                {
                    bc_copy = behaviorCharacterization.copy();
                    inst.bc = bc_copy;
                }
                inst.ff = fit_copy;

                // Schrum: Just add special handling for FourTasks to get settings right
                if (inst.ff is FourTasksFitness)
                {
                    // FourTasks needs to know the current environment. Experiment reference provided earlier.
                    ((FourTasksFitness)inst.ff).setupFitness(envNum);
                }

                // Schrum: moved here from outside/before the loop. Most domains use the same timestep in each environment,
                // but the FourTasks domain is an exception to this, so the setting needed to be moved after setupFitness
                inst.timestep = timestep;
                double tempFit = 0;
                double[] fitnesses = new double[timesToRunEnvironments];
                SharpNeatLib.Maths.FastRandom evalRand = new SharpNeatLib.Maths.FastRandom(100);
                for (int evals = 0; evals < timesToRunEnvironments; evals++)
                {
                    int agent_trials = timesToRunEnvironments;

                    inst.num_rbts = this.numberRobots;

                    Environment env = env2.copy();

                    double evalTime = evaluationTime;

                    inst.eval = evals;
                    env.seed = evals;
                    env.rng = new SharpNeatLib.Maths.FastRandom(env.seed + 1);


                    int noise_lev = (int)this.sensorNoise + 1;

                    float new_sn = evalRand.NextUInt() % noise_lev;
                    float new_ef = evalRand.NextUInt() % noise_lev;


                    inst.agentBrain = new AgentBrain(homogeneousTeam, inst.num_rbts, substrateDescription, network, normalizeWeights, adaptableANN, modulatoryANN, multibrain, numBrains, evolveSubstrate, preferenceNeurons, forcedSituationalPolicyGeometry);
                    initializeRobots(agentBrain, env, headingNoise, new_sn, new_ef, inst);

                    inst.elapsed = 0;
                    inst.timestep = this.timestep;
                    //Console.WriteLine(this.timestep);

                    inst.timeSteps = 0;
                    inst.collisionManager = collisionManager.copy();
                    inst.collisionManager.Initialize(env, this, inst.robots);

                    try
                    {


                        while (inst.elapsed < evalTime)
                        {
                            // Schrum2: Only called for non-visual evaluations
                            //Console.WriteLine("MAE:" + inst.elapsed + "/" + evalTime);
                            runEnvironment(env, inst, sem);
                        }
                    }
                    catch (Exception e)
                    {

                        for (int x = 0; x < inst.robots.Count; x++)
                        {
                            for (int y = 0; y < inst.robots[x].history.Count; y++)
                            {
                                Console.WriteLine(x + " " + y + " " + inst.robots[x].history[y].x + " " + inst.robots[x].history[y].y);
                            }
                        }


                        behavior = new SharpNeatLib.BehaviorType();
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                        throw (e);
                    }


                    double thisFit = inst.ff.calculate(this, env, inst, out behavior.objectives);
                    
                    //Console.WriteLine(env.name + ": Fitness for one eval: " + thisFit);
                    fitnesses[evals] = thisFit;
                    tempFit += thisFit;


                    if (behavior != null && behavior.objectives != null && inst.bc != null)
                    {
                        for (int i = 0; i < behavior.objectives.Length; i++)
                            accumObjectives[i] += behavior.objectives[i];

                        if (behavior.behaviorList == null)
                            behavior.behaviorList = new List<double>();
                        behavior.behaviorList.AddRange(inst.bc.calculate(this, inst));

                        inst.bc.reset();
                    }

                    inst.ff.reset();

                }
                // Schrum: Save each fitness separately
                environmentScores[envNum] = tempFit;
                envNum++; // go to the next environment

                fitness += tempFit / timesToRunEnvironments;
            }

            if (fitnessFunction is FourTasksFitness)
            { // Schrum: Special handling for FourTasksFitness ... could I just use accumObjectives?
                behavior.objectives = environmentScores;
            }
            else // Original default
            {
                behavior.objectives = accumObjectives;
            }
            //Console.WriteLine("Total: " + (fitness / environmentList.Count));
            return fitness / environmentList.Count;

        }

        private double variance(double[] x) //TODO this should be moved somewhere else
        {
            double sum = 0;
            double mean = 0;
            double dev = 0;
            double v = 0;
            foreach (double y in x)
            {
                sum += y;
            }
            mean = sum / x.Length;
            foreach (double y in x)
            {
                dev = y - mean;
                v += dev * dev;
            }
            return v / x.Length;
        }

        public override String defaultSeedGenome()
        {
            if (homogeneousTeam) return
                "seedGenome2x.xml";
            else return
                "seedGenomeHomo.xml";
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
