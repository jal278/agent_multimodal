using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class HardMazeFitness : IFitnessFunction
    {
        bool[] reachedPOI = new bool[7];
        bool reachedGoal;

        public IFitnessFunction copy()
        {
            return new HardMazeFitness();
        }

        public HardMazeFitness()
        {
        
        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Hard Maze Single Goal Point Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
           double fitness = 0.0f;

            //fitness += bonus;
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                if (reachedPOI[i])
                {
                    fitness += 1.0f;
                }
                else
                {
                    fitness += (1.0f - ip.robots[0].location.distance(new Point2D(environment.POIPosition[i].X, environment.POIPosition[i].Y)) / 650.0f);
                    break;
                }
            }
            if (reachedGoal)
            {
                fitness = 10.0f;
            }

            objectives = null;
            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            if (reachedGoal) return;

           // if (ip.robots[0].collisions > 0) return;

            if (ip.robots[0].location.distance(environment.goal_point) < 35.0f)
            {
                reachedGoal = true;
            }

            for (int i = 0; i < environment.POIPosition.Count; i++)
            {
                if (ip.robots[0].location.distance(new Point2D((int)environment.POIPosition[i].X, (int)environment.POIPosition[i].Y)) < 20.0f)
                {
                    reachedPOI[i] = true;
                }
            }

        }

        void IFitnessFunction.reset()
        {
            for (int i = 0; i < reachedPOI.Length; i++)
            {
                reachedPOI[i] = false;
            }
            reachedGoal = false;
        }

        #endregion
    }
}
