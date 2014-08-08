using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class LandmarkFitness : IFitnessFunction
    {
        #region IFitnessFunction Members
		public IFitnessFunction copy()
		{
			return new LandmarkFitness();
		}
		
        double radius = 35;

        double[,] distances;
        int one, two;
        bool first = true;

        public string name
        {
            get { return this.GetType().Name; }
        }

        public string description
        {
            get { return "Fitness"; }
        }
        
        public double calculate(SimulatorExperiment engine, Environment environment, out double[] obj)
        {
			obj=null;
            if (first)
                return 0;

            double fit = 0;
            double one = radius, two = radius;
            for (int j = 0; j < environment.POIPosition.Count; j++)
            {
                one = radius;
                two = radius;
                for (int k = 0; k < engine.robots.Count; k++)
                {
                    if (distances[j, k] < radius)
                    {
                        if (distances[j, k] < one)
                        {
                            two = one;
                            one = distances[j, k];
                        }
                        else if (distances[j, k] < two)
                            two = distances[j, k];
                    }
                }
                if (one != radius && two != radius)
                    fit += radius - ((one + two) / 2.0);
                else if (one != radius)
                    fit += (radius - one) / 10.0;
            }
            return Math.Max(fit,0.001);
        }

        public void update(SimulatorExperiment engine, Environment environment)
        {
            if (first)
            {
                distances = new double[environment.POIPosition.Count, engine.robots.Count];
                for (int v = 0; v < environment.POIPosition.Count; v++)
                    for (int w = 0; w < engine.robots.Count; w++)
                        distances[v, w] = radius;
                first = false;
                return;
            }

            double dist = 0;
            for(int k=0;k<engine.robots.Count;k++)
            {
                Robot r = engine.robots[k];
                for (int j = 0; j < environment.POIPosition.Count; j++)
                {
                    dist = EngineUtilities.euclideanDistance(r.location, new Point2D(environment.POIPosition[j].X, environment.POIPosition[j].Y));
                    if (dist < distances[j, k])
                        distances[j, k] = dist;
                }

            }
        }

        public void reset()
        {
            first = true;
            distances = new double[3, 2];
        }

        #endregion
    }
}
