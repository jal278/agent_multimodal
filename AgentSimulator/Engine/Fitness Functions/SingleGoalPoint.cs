using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class SingleGoalPoint : IFitnessFunction
    {
        #region IFitnessFunction Members

        double maxSensorValue = 0;
        bool reachedGoal = false;// reachedGoal=false;
        private int t;
        double closest = double.MaxValue;
        double fitness = 0;
        //int steps = 0;

		public IFitnessFunction copy()
		{
			return new SingleGoalPoint();
		}
		
		
        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness is the sum of the distance of each agent from goal point, each substracted from 1000.  The optimal score is 1000*# of agents."; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment,out double[] objectives)
        {
			objectives=null;
            if (reachedGoal)
            {
                //126
                // System.Diagnostics.Debug.Assert(t > 176.0);

                //fitness = (176.0 / t) * Math.Exp(-0.1) +
                //    Math.Exp(-engine.robots[0].location.distance(environment.start_point) / environment.maxDistance);

                fitness = 1.0f + (1.0f - maxSensorValue);// (176.0 / t) * Math.Exp(-0.1);


            }
            else
            {
                fitness = Math.Exp(-closest / environment.maxDistance);
            }

            return fitness;
           // return fitness+(1000 - (engine.robots[0].location.distance(environment.goal_point)));
           // //!
           //// double fitness=0.0;
           // for(int x=0;x<environment.num_robots;x++)
           // {
           //     if (engine.robots[0].fitness < 0.0) engine.robots[0].fitness = 1.0f;
           //     //engine.robots[0].fitness += 1000 - (engine.robots[x].location.distance(environment.goal_point));
           //     //if (engine.robots[x].reachedGoal)
           //     //{
           //     //    fitness += 1000;
           //     //}
           //     //if (engine.robots[x].reachedFirst)
           //     //{
           //     //    fitness += 500;
           //     //}
           // }


           // return engine.robots[0].fitness;
        }

		void IFitnessFunction.update(SimulatorExperiment engine, Environment environment)
		{
            double k;
            
            foreach (RangeFinder r in engine.robots[0].sensors)
            {
                k = 1.0f - (r.distance / r.max_range);
                if (k > maxSensorValue)
                    maxSensorValue = k;
            }

			if (engine.robots[0].location.distance(environment.goal_point) <closest) 
				closest=engine.robots[0].location.distance(environment.goal_point);
				
            if (reachedGoal) return;
            if (engine.robots[0].location.distance(environment.goal_point) < 15.0f)
            {
              //  actValue = 1.0f;
             //   engine.robots[0].reachedGoal = true;
                reachedGoal = true;
                t = engine.timeSteps;
            }

            return;

            //if ((engine.robots[0].location.distance(environment.goal_point) < 15.0f))
            //{
            //    fitness = 1000;
            //};
            //return;
           // if (reachedGoal) return;

            //RIGHT NOW ONLY FOR FIRST ROBOT
            //if (engine.robots[0].location.distance(environment.goal_point) < 20.0f)
            //{
            //    //engine.robots[0].reachedGoal = true;
            //    engine.robots[0].fitness += 1000;
            //    engine.robots[0].heading = 270 / 180.0 * 3.14;
            //    engine.robots[0].location.x = environment.start_point.x;
            //    engine.robots[0].location.y = environment.start_point.y;
            //}
            //if (engine.robots[0].collide_last)
            //{
            //    engine.robots[0].fitness -= 50;
            //    engine.robots[0].heading = 270 / 180.0 * 3.14;
            //    engine.robots[0].location.x = environment.start_point.x;
            //    engine.robots[0].location.y = environment.start_point.y;
            //}
            //engine.robots[0].steps++;
            //if (engine.robots[0].steps % 80 == 0)
            //{
            ////!    engine.robots[0].fitness += 1000 - (engine.robots[0].location.distance(environment.goal_point));
            //    if (environment.goal_point.x == 247)
            //    {
            //        environment.goal_point.x = 355;
            //        environment.start_point.x = 355;
            //    }
            //    else
            //    {
            //        environment.goal_point.x = 247;
            //        environment.start_point.x = 249;
            //    }
            //    engine.robots[0].marker++;
            //    engine.robots[0].heading = 270 / 180.0 * 3.14;
            //    engine.robots[0].location.x = environment.start_point.x;
            //    engine.robots[0].location.y = environment.start_point.y;
            //}
            //for (int i = 0; i < environment.POIPosition.Count; i++)
            //{
            //    if (engine.robots[0].location.distance(new Point2D(environment.POIPosition[i].X, environment.POIPosition[i].Y)) < 20.0f)
            //    {
            //        engine.robots[0].marker = (i+1);
            //    }
            //}
		}
		
		void IFitnessFunction.reset() 
		{
            reachedGoal = false;
            closest = double.MaxValue;
            t = 0;
            maxSensorValue = 0;
		}
        #endregion
    }
}
