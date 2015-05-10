using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class TwoRoomFitness : IFitnessFunction
    {
        int collectedFood=0;
        int POINr = 0;

        public IFitnessFunction copy()
        {
            return new TwoRoomFitness();
        }

        public TwoRoomFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Two Room Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {


            double fitness = 0.0f;
            objectives = null;

            //food gathering 
            // Schrum: sets goal to a new food item
            float distFood = (float)(1.0f - (engine.robots[0].location.distance(environment.goal_point) / environment.maxDistance));
            fitness = (collectedFood + distFood) / (environment.POIPosition.Count * 1.0f);

            // Schrum: This error checking shouldn't be necessary
            if (fitness < 0) fitness = 0.00001f;
            if (collectedFood >= environment.POIPosition.Count) fitness = 1.0f;

            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            // Schrum: Is a brain-switching policy needed?

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
                if (POINr >= environment.POIPosition.Count) POINr = 0;
                environment.goal_point.x = environment.POIPosition[POINr].X;
                environment.goal_point.y = environment.POIPosition[POINr].Y;

            }

            // Schrum2: Added to detect robot collisions and end the evaluation when they happen
            if (ip.robots[0].collisions > 0)
            { // Disabling prevents further action
                //Console.WriteLine("Fitness before:" + ip.elapsed + "/" + Experiment.evaluationTime + ":" + ip.timeSteps);
                ip.elapsed = Experiment.evaluationTime; // force end time: only affects non-visual evaluation
                Experiment.running = false; // Ends visual evaluation
                //Console.WriteLine("Collision");
                //Console.WriteLine("Fitness after:" + ip.elapsed + "/" + Experiment.evaluationTime + ":" + ip.timeSteps);
            }
        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
