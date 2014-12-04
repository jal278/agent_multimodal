using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class DangerousForagingFitness : IFitnessFunction
    {
        int collectedFood=0;
        int POINr = 0;

        public IFitnessFunction copy()
        {
            return new DangerousForagingFitness();
        }

        public DangerousForagingFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Dangerous Foraging Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            double fitness = 0.0f;
            objectives = null;
                // Only acknowledge actual eating of food, not just proximity
                fitness = collectedFood / 4.0f;

                if (collectedFood >= 4) fitness = 1.0f;

                // Schrum
                // HACK: Both environments are the same, but this hack allows one to treat the food as poison
                bool poison = !environment.name.Equals("ENV_dual_task1.xml");
                // Extra aspect of the HACK: The first task loaded excludes its path from the name, but this is not
                // the case for the second task loaded. This is why the negation is used instead of looking up the second
                // task directly, which is named ENV_dual_task11.xml (convention of simulator)
                if (poison) fitness *= -0.9;

                // Schrum: For troubleshooting
                //Console.WriteLine(environment.name + " " + fitness + " " + poison);

            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            // HACK: Both environments are the same, but this hack allows one to treat the food as poison
            bool poison = !environment.name.Equals("ENV_dual_task1.xml");

            if (Experiment.multibrain && !Experiment.preferenceNeurons && Experiment.numBrains == 2)
            {
                if (!poison) //forage
                {
                    ip.agentBrain.switchBrains(0);
                }
                else   //poison 
                {
                    ip.agentBrain.switchBrains(1);
                }
            }

            //For food gathering
            if (ip.timeSteps == 1)
            {
                environment.goal_point.x = environment.POIPosition[0].X;
                environment.goal_point.y = environment.POIPosition[0].Y;

                collectedFood = 0;
                POINr = 0;
            }

            // Schrum: Last sensor is for detecting when food/poison is eaten
            Robot r = ip.robots[0]; // There should be only one robot in this task
            SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count - 1];

            float d = (float)ip.robots[0].location.distance(environment.goal_point);
            if (d < 20.0f)
            {
                // Need to detect when food or poison is eaten
                s.setSignal(poison ? -1.0 : 1.0);

                collectedFood++;
                POINr++;
                if (POINr > 3) POINr = 0;
                environment.goal_point.x = environment.POIPosition[POINr].X;
                environment.goal_point.y = environment.POIPosition[POINr].Y;

            }
            else
            {// Nothing eaten, so food/poison sensor is 0
                s.setSignal(0.0);
            }

        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
