using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib.CPPNs;

/**
 *  This fitness function uses four other fitness functions.
 *  The idea is that the agent is evaluated in each of the four
 *  tasks in sequence, so we can simply reuse the code from
 *  the separate fitness functions.
 * */
namespace Engine
{
    public class FourTasksFitness : IFitnessFunction
    {
        public const int TASK_TEAM_PATROL = 0;
        public const int TASK_LONE_PATROL = 1;
        public const int TASK_DUAL_TASK_HALLWAY = 2;
        public const int TASK_DUAL_TASK_FORAGE = 3;
        public const int TASK_TWO_ROOMS = 4;

        // Load these from file once and change as the environment changes
        public static SubstrateDescription SUBSTRATE_TEAM_PATROL = new SubstrateDescription("substrate_PatrolSignalPreference.xml");
        public static SubstrateDescription SUBSTRATE_LONE_PATROL = new SubstrateDescription("substrate_PatrolPreference.xml");
        public static SubstrateDescription SUBSTRATE_DUAL_TASK = new SubstrateDescription("hardmaze_substrate-preference.xml");
        public static SubstrateDescription SUBSTRATE_TWO_ROOMS = new SubstrateDescription("hardmaze_substrate-preference.xml"); // redundant

        IFitnessFunction teamPatrol = new POIFIT_MO();
        IFitnessFunction lonePatrol = new VisitThreeFitness();
        IFitnessFunction dualTask = new DualTaskFitness();
        IFitnessFunction twoRooms = new TwoRoomFitness();

        int currentEnvironment = TASK_TEAM_PATROL;
        MultiAgentExperiment experiment = null; // This will need to be stored

        public IFitnessFunction copy()
        {
            // Maintain a reference to the experiment when copied, and set the experiment up properly
            return new FourTasksFitness(experiment);
        }

        // This is used by the factory that constructs the fitness function,
        // but a reference to the experiment must immediately be provided.
        public FourTasksFitness() { }

        public void setExperiment(MultiAgentExperiment exp)
        {
            experiment = exp; // This mem reference is required for the fitness function to work
        }

        public FourTasksFitness(MultiAgentExperiment exp)
        {
            setExperiment(exp);
        }
        
        public void setupFitness(int environmentNum)
        {
            //Console.WriteLine("setupFitness(" + exp + "," + environmentNum + ")");
            currentEnvironment = environmentNum;

            // Special reconfiguring based on particular environment
            if (currentEnvironment == TASK_TEAM_PATROL)
            { // Team patrol
                //Console.WriteLine("Set to Team Patrol");
                    
                // settings from the Team Patrol experiment files
                // that differ from the other experiments
                experiment.evaluationTime = 45;
                experiment.timestep = 0.033;
                    
                experiment.overrideTeamFormation = true;
                experiment.group_orientation = 90;
                experiment.robot_heading = 270;

                experiment.numberRobots = 3;
                experiment.agentsVisible = false;
                experiment.agentsCollide = false;

                experiment.normalizeWeights = true;

                // Robot sizes are different in the different experiments
                FourTaskRangeSliceSignal.robotSize = 6.5f;

                experiment.substrateDescription = SUBSTRATE_TEAM_PATROL;
                experiment.robotModelName = "Khepera3RobotModelComeHome";
                experiment.rangefinderSensorDensity = 6;
            }
            else if (currentEnvironment == TASK_LONE_PATROL)
            { // Lone patrol
                //Console.WriteLine("Set to Lone Patrol");

                experiment.evaluationTime = 80;
                experiment.timestep = 0.033;

                experiment.overrideTeamFormation = true;
                experiment.group_orientation = 90;
                experiment.robot_heading = 270;

                experiment.numberRobots = 1;
                experiment.agentsVisible = false;
                experiment.agentsCollide = false;
                experiment.normalizeWeights = true;
                FourTaskRangeSliceSignal.robotSize = 6.5f;
                experiment.substrateDescription = SUBSTRATE_LONE_PATROL;
                experiment.robotModelName = "Khepera3RobotModel";
                experiment.rangefinderSensorDensity = 6;

            }
            else if (currentEnvironment == TASK_DUAL_TASK_HALLWAY || currentEnvironment == TASK_DUAL_TASK_FORAGE)
            { // Dual task
                //Console.WriteLine("Set to Dual Task: " + currentEnvironment);

                experiment.evaluationTime = 45;
                experiment.timestep = 0.2;

                experiment.overrideTeamFormation = true;
                experiment.group_orientation = 0;
                experiment.robot_heading = 270;

                experiment.numberRobots = 1;
                experiment.agentsVisible = true; // does this even matter?
                experiment.agentsCollide = true; // does this even matter?
                experiment.normalizeWeights = false;
                FourTaskRangeSliceSignal.robotSize = 10.5f;
                experiment.substrateDescription = SUBSTRATE_DUAL_TASK;
                experiment.robotModelName = "MazeRobotPieSlice";
                experiment.rangefinderSensorDensity = 5;

            }
            else if (currentEnvironment == TASK_TWO_ROOMS)
            { // Two rooms
                //Console.WriteLine("Set to Two Rooms");

                experiment.evaluationTime = 200;
                experiment.timestep = 0.1; // Is this correct? See paper

                experiment.overrideTeamFormation = true;
                experiment.group_orientation = 0;
                experiment.robot_heading = 90;

                experiment.numberRobots = 1;
                experiment.agentsVisible = true; // does this even matter?
                experiment.agentsCollide = true; // does this even matter?
                experiment.normalizeWeights = false;
                FourTaskRangeSliceSignal.robotSize = 10.5f;
                experiment.substrateDescription = SUBSTRATE_TWO_ROOMS;
                experiment.robotModelName = "MazeRobotPieSlice";
                experiment.rangefinderSensorDensity = 5;
            }
            else
            {
                Console.WriteLine("Error! Unknown environment! " + currentEnvironment);
                System.Environment.Exit(1);
            }

            // Schrum:
            // This extra function seemed necessary due to some kind of method access
            // rules in C#. Not sure I fully understand them, but I saw a similar
            // workaround in POIFIT_MO
            reset0();
        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Four Tasks Fitness"; }
        }

        public static double MAX_TEAM_PATROL = 65; // Observed in GECCO 2016 work
        public static double MAX_LONE_PATROL = 6000; // Observed in GECCO 2016 work

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            if (currentEnvironment == TASK_TEAM_PATROL)
            { // Team patrol
                // Must normalize score
                return teamPatrol.calculate(engine, environment, ip, out objectives) / MAX_TEAM_PATROL;
            }
            else if (currentEnvironment == TASK_LONE_PATROL)
            { // Lone patrol
                // Must normalize score
                return lonePatrol.calculate(engine, environment, ip, out objectives) / MAX_LONE_PATROL;
            }
            else if (currentEnvironment == TASK_DUAL_TASK_HALLWAY || currentEnvironment == TASK_DUAL_TASK_FORAGE)
            { // Dual task
                // Both individual dual task fitnss scores are already normalized
                return dualTask.calculate(engine, environment, ip, out objectives);
            }
            else if (currentEnvironment == TASK_TWO_ROOMS)
            { // Two rooms
                // Score is already normalized to [0,1]
                return twoRooms.calculate(engine, environment, ip, out objectives);
            }
            else
            {
                Console.WriteLine("Error! Unknown environment! " + environment.name + ":" + currentEnvironment);
                objectives = new double[0];
                System.Environment.Exit(1);                
                return -1000;
            }
        }

        void IFitnessFunction.update(SimulatorExperiment simExp, Environment environment, instance_pack ip)
        {
            if (currentEnvironment == TASK_TEAM_PATROL)
            { // Team patrol
                teamPatrol.update(simExp, environment, ip);
            } else {
                // Make sure the signal sensor is only used in the Team Patrol task
                // Schrum: Code no longer needed since I switch the robot model for each environment
                // instead of forcing them each to use the same one with (sometimes) unused sensors
                /**
                foreach(Robot r in ip.robots) {
                    // Signal sensor is last input of the FourTasks substrate
                    SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count - 1];
                    s.setSignal(0.0);
                }
                 */

                if (currentEnvironment == TASK_LONE_PATROL)
                { // Lone patrol
                    lonePatrol.update(simExp, environment, ip);
                }
                else if (currentEnvironment == TASK_DUAL_TASK_HALLWAY || currentEnvironment == TASK_DUAL_TASK_FORAGE)
                { // Dual task
                    dualTask.update(simExp, environment, ip);
                }
                else if (currentEnvironment == TASK_TWO_ROOMS)
                { // Two rooms
                    twoRooms.update(simExp, environment, ip);
                }
                else
                {
                    Console.WriteLine("Error! Unknown environment! " + environment.name + ":" + currentEnvironment);
                    System.Environment.Exit(1);
                }
            }
        }

        void IFitnessFunction.reset()
        {
            reset0();
        }

        private void reset0() {
            teamPatrol.reset();
            lonePatrol.reset();
            dualTask.reset();
            twoRooms.reset();
        }

        #endregion

        public static int environmentID(String environmentName)
        {
                int environmentNumber = 0; // team patrol, default
                if (environmentName.EndsWith("FourTasks-ENV.xml"))
                { // Team patrol
                    environmentNumber = TASK_TEAM_PATROL;
                }
                else if (environmentName.EndsWith("FourTasks-ENV1.xml"))
                { // Lone patrol
                    environmentNumber = TASK_LONE_PATROL;
                }
                else if (environmentName.EndsWith("FourTasks-ENV2.xml"))
                { // dual task hallway
                    environmentNumber = TASK_DUAL_TASK_HALLWAY;
                }
                else if (environmentName.EndsWith("FourTasks-ENV3.xml"))
                { // Dual task foraging
                    environmentNumber = TASK_DUAL_TASK_FORAGE;
                }
                else if (environmentName.EndsWith("FourTasks-ENV4.xml"))
                { // Two rooms
                    environmentNumber = TASK_TWO_ROOMS;
                }
                else
                {
                    Console.WriteLine("Error! Unknown environment! " + environmentName);
                    System.Environment.Exit(1);
                }
                return environmentNumber;
        }
    }
}
