using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

// Schrum: A fitness function for the plus maze with a single agent that needs to visit all points
// Schrum: 11/15/15: Made minor improvement in terms of consistency.
//         MAX_DISTANCE added, and Manhattan distance now used, as in Team Patrol. Fitness still very different.

namespace Engine
{
    class VisitThreeFitness : IFitnessFunction
    {
        bool[] reachedPOI = new bool[4];
        double fitness = 0.0f;

        public IFitnessFunction copy()
        {
            return new VisitThreeFitness();
        }

        public VisitThreeFitness()
        {
            fitness = 0.0f;
        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Visit Three Points of Plus Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = null;
            return fitness;
        }

        void incrementFitness(Environment environment, instance_pack ip) 
        {
            // Schrum: Added to avoid magic numbers and make purpose clear
            float MAX_DISTANCE = 300.0f;
            // Schrum: Last POI is actually goal: Return to start
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                if (reachedPOI[i])
                {
                    fitness += 1.0f;
                }
                else if (i == 3) { // Schrum: Hack: Last POI is actually the goal
                    double gain = (1.0f - ip.robots[0].location.manhattenDistance(environment.goal_point) / MAX_DISTANCE);
                    //Console.WriteLine("Goal Gain = " + gain);
                    fitness += gain;
                    break;
                }
                else
                {
                    // Schrum: From HardMaze
                    //fitness += (1.0f - ip.robots[0].location.distance(new Point2D(environment.POIPosition[i].X, environment.POIPosition[i].Y)) / 650.0f);
                    // Schrum: Guessing at distance to divide by
                    // Schrum: Switched to Manhattan distance since Team Patrol uses it
                    double gain = (1.0f - ip.robots[0].location.manhattenDistance(new Point2D(environment.POIPosition[i].X, environment.POIPosition[i].Y)) / MAX_DISTANCE);
                    //Console.WriteLine("Gain = " + gain);
                    fitness += gain;
                    break;
                }
            }
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            incrementFitness(environment, ip);

            bool all = true;
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                all = all && reachedPOI[i];
            }
            if (all) return; // Schrum: Done if all goals were reached

            for (int i = 0; i < environment.POIPosition.Count; i++)
            {
                if (reachedPOI[i])
                {
                    continue; // Schrum: Once one POI has been reached, move to the next
                }
                else if (ip.robots[0].location.distance(new Point2D((int)environment.POIPosition[i].X, (int)environment.POIPosition[i].Y)) < 10.0f)
                {
                    reachedPOI[i] = true;
                    // Schrum: Only manually change brains if preference neurons are not used
                    // Schrum: Don't switch brains here if there are 5 brains, since this corresponds to the FourTasks experiments.
                    if (Experiment.multibrain && !Experiment.preferenceNeurons && Experiment.numBrains != 5)
                    {
                        if (ip.agentBrain.numBrains == 3) // Schrum: Playing with special cases. Still need something more general.
                        {
                            int[] mapping = new int[] { 1, 2, 1, 0 }; // Mapping to the next module to use. Module 1 is repeated since it is for straight corridors.
                            ip.agentBrain.switchBrains(mapping[i]);
                        }
                        else
                        {   // Schrum: I'm not sure this option is actually used anywhere
                            ip.agentBrain.switchBrains(i + 1); // Schrum: Switch to next brain (one for each step of task)
                        }
                    }
                }
                break; // Schrum: Can't reach two at once, and must reach in order. Only "continue" can get past this
            }

            // Schrum: Once all POIs have been checked, the goal (returning) can be checked. Goal treated like extra POI
            if (reachedPOI[2] && ip.robots[0].location.distance(environment.goal_point) < 10.0f)
            {
                reachedPOI[3] = true;
            }
        }

        void IFitnessFunction.reset()
        {
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                reachedPOI[i] = false;
            }
            fitness = 0;
        }

        #endregion
    }
}
