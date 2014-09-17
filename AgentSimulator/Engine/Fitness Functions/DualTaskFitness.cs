using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class DualTaskFitness : IFitnessFunction
    {
        bool reachedGoal=false;
        int collectedFood=0;
        int POINr = 0;

        public IFitnessFunction copy()
        {
            return new DualTaskFitness();
        }

        public DualTaskFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Dual Task Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {


            double fitness = 0.0f;
            objectives = null;

            if (environment.name.Equals("ENV_dual_task.xml")) //HACK navigation
            {
                fitness = (1.0f - ip.robots[0].location.distance(new Point2D(environment.POIPosition[4].X, environment.POIPosition[4].Y)) / 650.0f);
 
                 if (fitness < 0) fitness = 0.00001f;
                 if (reachedGoal) fitness = 1;

               //  fitness = 1;
            }
            else   //food gathering 
            {
                float distFood = (float)(1.0f - (engine.robots[0].location.distance(environment.goal_point) / environment.maxDistance));
                fitness = (collectedFood + distFood) / 4.0f;

                if (fitness < 0) fitness = 0.00001f;
                if (collectedFood >= 4) fitness = 1.0f;

             //  fitness = 1;
            }
         
            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            //Console.WriteLine(Experiment.multibrain + " && " + !Experiment.preferenceNeurons + " && " + (Experiment.numBrains == 2));
            // Schrum: Set which brain to use if the number is an experiment parameter
            if (Experiment.multibrain && !Experiment.preferenceNeurons && Experiment.numBrains == 2)
            {
                if (environment.name.Equals("ENV_dual_task.xml")) //HACK navigation
                {
                    ip.agentBrain.switchBrains(0);
                }
                else   //food gathering 
                {
                    ip.agentBrain.switchBrains(1);
                }
            }

            //For navigation
            if (ip.robots[0].location.distance(new Point2D((int)environment.POIPosition[4].X, (int)environment.POIPosition[4].Y)) < 20.0f)
            {
                reachedGoal = true;
            }


            //For food gathering
            if (ip.timeSteps == 1)
            {
                environment.goal_point.x = environment.POIPosition[0].X;
                environment.goal_point.y = environment.POIPosition[0].Y;

                collectedFood = 0;
                POINr = 0;
            }

            float d = (float)ip.robots[0].location.distance(environment.goal_point);
            if (d < 20.0f)
            {
                collectedFood++;
                POINr++;
                if (POINr > 3) POINr = 0;
                environment.goal_point.x = environment.POIPosition[POINr].X;
                environment.goal_point.y = environment.POIPosition[POINr].Y;

            }

        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
