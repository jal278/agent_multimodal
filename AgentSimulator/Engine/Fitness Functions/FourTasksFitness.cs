using System;
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
        IFitnessFunction teamPatrol = new POIFIT_MO();
        IFitnessFunction lonePatrol = new VisitThreeFitness();
        IFitnessFunction dualTask = new DualTaskFitness();
        IFitnessFunction twoRooms = new TwoRoomFitness();

        int currentEnvironment = 0;
        MultiAgentExperiment experiment = null; // This will need to be stored

        public IFitnessFunction copy()
        {
            // There are 5 environments across the four tasks.
            // Whenever the fitness function is copied (happens only in MultiAgentExperiment)
            // it means the agent is moving on to being evaluated in the next task.
            // When experiment is null, the environment should still be 0 because of the
            // first copy() call that happens in MultiAgentExperiment.
            return new FourTasksFitness(experiment, experiment == null ? 0 : currentEnvironment + 1 % 5);
        }

        // Start in environment 0 by default
        public FourTasksFitness() : this(null, 0) { }

        public FourTasksFitness(MultiAgentExperiment exp, int environmentNum)
        {
            currentEnvironment = environmentNum;
            experiment = exp;

            // Special reconfiguring based on particular environment
            if (currentEnvironment == 0)
            { // Team patrol
                if (experiment != null) // null should only be possible with Team patrol
                { // will be null on first use, but not afterwards
                    
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
            }
            else if (currentEnvironment == 1)
            { // Lone patrol
                experiment.evaluationTime = 80;
                experiment.timestep = 0.033;
                
                experiment.overrideTeamFormation = false;
                experiment.group_orientation = 0;
                experiment.robot_heading = 0;

                experiment.numberRobots = 1;
                experiment.agentsVisible = false;
                experiment.agentsCollide = false;
                FourTaskRangeSliceSignal.robotSize = 6.5f;
            }
            else if (currentEnvironment == 2 || currentEnvironment == 3)
            { // Dual task
                experiment.evaluationTime = 45;
                experiment.timestep = 0.2;
                
                experiment.overrideTeamFormation = false;
                experiment.group_orientation = 0;
                experiment.robot_heading = 90;

                experiment.numberRobots = 1;
                experiment.agentsVisible = true; // does this even matter?
                experiment.agentsCollide = true; // does this even matter?
                FourTaskRangeSliceSignal.robotSize = 10.5f;
            }
            else if (currentEnvironment == 4)
            { // Two rooms
                experiment.evaluationTime = 200;
                experiment.timestep = 0.1; // Is this correct? See paper

                experiment.overrideTeamFormation = false;
                experiment.group_orientation = 0;
                experiment.robot_heading = 270;

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

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            if (environment.name.EndsWith("FourTasks-ENV.xml"))
            { // Team patrol
                return teamPatrol.calculate(engine, environment, ip, out objectives);
            }
            else if (environment.name.EndsWith("FourTasks-ENV1.xml"))
            { // Lone patrol
                return lonePatrol.calculate(engine, environment, ip, out objectives);
            }
            else if (environment.name.EndsWith("FourTasks-ENV2.xml") || environment.name.EndsWith("FourTasks-ENV3.xml"))
            { // Dual task
                return dualTask.calculate(engine, environment, ip, out objectives);
            }
            else if (environment.name.EndsWith("FourTasks-ENV4.xml"))
            { // Two rooms
                return twoRooms.calculate(engine, environment, ip, out objectives);
            }
            else
            {
                Console.WriteLine("Error! Unknown environment! " + environment.name);
                objectives = new double[0];
                System.Environment.Exit(1);                
                return -1000;
            }
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            if (environment.name.EndsWith("FourTasks-ENV.xml"))
            { // Team patrol
                teamPatrol.update(Experiment, environment, ip);
            } else {
                // Make sure the signal sensor is only used in the Team Patrol task
                foreach(Robot r in ip.robots) {
                    // Signal sensor is last input of the FourTasks substrate
                    SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count - 1];
                    s.setSignal(0.0);
                }

                if (environment.name.EndsWith("FourTasks-ENV1.xml"))
                { // Lone patrol
                    lonePatrol.update(Experiment, environment, ip);
                }
                else if (environment.name.EndsWith("FourTasks-ENV2.xml") || environment.name.EndsWith("FourTasks-ENV3.xml"))
                { // Dual task
                    dualTask.update(Experiment, environment, ip);
                }
                else if (environment.name.EndsWith("FourTasks-ENV4.xml"))
                { // Two rooms
                    twoRooms.update(Experiment, environment, ip);
                }
                else
                {
                    Console.WriteLine("Error! Unknown environment! " + environment.name);
                    System.Environment.Exit(1);
                }
            }
        }

        void IFitnessFunction.reset()
        {
            //Console.WriteLine("Reset four tasks");
            //Console.WriteLine("Reset team patrol");
            teamPatrol.reset();
            //Console.WriteLine("Reset lone patrol");
            lonePatrol.reset();
            //Console.WriteLine("Reset dual task");
            dualTask.reset();
            //Console.WriteLine("Reset two rooms");
            twoRooms.reset();
            //Console.WriteLine("Done resetting four tasks");
        }

        #endregion
    }
}
