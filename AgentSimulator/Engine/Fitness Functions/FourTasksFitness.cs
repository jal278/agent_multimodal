﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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

                // Robot sizes are different in the different experiments
                FourTaskRangeSliceSignal.robotSize = 6.5f;
                
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
                FourTaskRangeSliceSignal.robotSize = 6.5f;
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
                FourTaskRangeSliceSignal.robotSize = 10.5f;
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
                FourTaskRangeSliceSignal.robotSize = 10.5f;
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

        public static double MAX_TEAM_PATROL = 70; // 65; // Observed in GECCO 2016 work
        public static double MAX_LONE_PATROL = 6000; // Observed in GECCO 2016 work

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            if (environment.name.EndsWith("FourTasks-ENV.xml") && currentEnvironment == TASK_TEAM_PATROL)
            { // Team patrol
                // Must normalize score
                //Console.WriteLine("Team Patrol fitness");
                return teamPatrol.calculate(engine, environment, ip, out objectives) / MAX_TEAM_PATROL;
            }
            else if (environment.name.EndsWith("FourTasks-ENV1.xml") && currentEnvironment == TASK_LONE_PATROL)
            { // Lone patrol
                // Must normalize score
                //Console.WriteLine("Lone Patrol fitness");
                return lonePatrol.calculate(engine, environment, ip, out objectives) / MAX_LONE_PATROL;
            }
            else if ((environment.name.EndsWith("FourTasks-ENV2.xml") && currentEnvironment == TASK_DUAL_TASK_HALLWAY)
                  || (environment.name.EndsWith("FourTasks-ENV3.xml") && currentEnvironment == TASK_DUAL_TASK_FORAGE))
            { // Dual task
                // Both individual dual task fitnss scores are already normalized
                //Console.WriteLine("Dual Task fitness: " + environment.name);
                return dualTask.calculate(engine, environment, ip, out objectives);
            }
            else if (environment.name.EndsWith("FourTasks-ENV4.xml") && currentEnvironment == TASK_TWO_ROOMS)
            { // Two rooms
                // Score is already normalized to [0,1]
                //Console.WriteLine("Two Rooms fitness");
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
            // Need to store the experiment reference in order to cycle through environments.
            // This shouldn't be needed because setExperiment is used above.
            //experiment = (MultiAgentExperiment) simExp;

            if (environment.name.EndsWith("FourTasks-ENV.xml") && currentEnvironment == TASK_TEAM_PATROL)
            { // Team patrol
                //Console.WriteLine("update team patrol");
                teamPatrol.update(simExp, environment, ip);
            } else {
                // Make sure the signal sensor is only used in the Team Patrol task
                foreach(Robot r in ip.robots) {
                    // Signal sensor is last input of the FourTasks substrate
                    SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count - 1];
                    s.setSignal(0.0);
                }

                if (environment.name.EndsWith("FourTasks-ENV1.xml") && currentEnvironment == TASK_LONE_PATROL)
                { // Lone patrol
                    //Console.WriteLine("update lone patrol");
                    lonePatrol.update(simExp, environment, ip);
                }
                else if ((environment.name.EndsWith("FourTasks-ENV2.xml") && currentEnvironment == TASK_DUAL_TASK_HALLWAY)
                      || (environment.name.EndsWith("FourTasks-ENV3.xml") && currentEnvironment == TASK_DUAL_TASK_FORAGE))
                { // Dual task
                    //Console.WriteLine("update dual task: " + environment.name);
                    dualTask.update(simExp, environment, ip);
                }
                else if (environment.name.EndsWith("FourTasks-ENV4.xml") && currentEnvironment == TASK_TWO_ROOMS)
                { // Two rooms
                    //Console.WriteLine("update two rooms");
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
    }
}
