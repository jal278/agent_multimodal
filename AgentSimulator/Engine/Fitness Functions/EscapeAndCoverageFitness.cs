using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class EscapeAndCoverageFitness : IFitnessFunction
    {
        public double accum;
		public collision_grid grid;
        public bool allLeft = false;
		
		public IFitnessFunction copy()
		{
			return new EscapeAndCoverageFitness();
		}
		
        public EscapeAndCoverageFitness()
        {
			grid=null;
            accum = 0.0;
        }
        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for Coverage of Map and Leaving the Room"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment,out double[] objectives)
        {
			objectives=null;
          /*  double a = 0;

            grid = ((GridCollision)((MultiAgentExperiment)engine).collisionManager).grid;

            int dim = grid.coarseness;
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    int gx = (int)((double)x * grid.gridx) + (int)(grid.gridx / 2.0);
                    int gy = (int)((double)y * grid.gridy) + (int)(grid.gridy / 2.0);
                    if ((environment.AOIRectangle.Contains(gx, gy)))
                        a += grid.grid[x, y].viewed;
                }
            }*/

            double fit = 0;
            if (allLeft)
            {
                for (int j = 0; j < engine.robots.Count; j++)
            {
                if (!environment.AOIRectangle.Contains((int)engine.robots[j].location.x, (int)engine.robots[j].location.y))
                {
                    fit += 50;
                }
                else if(!engine.robots[j].collide_last)
                {
                    fit += 10.0 * (1.0-(EngineUtilities.euclideanDistance(engine.robots[j].location, environment.goal_point)/145.0));
                }
            }
            }
        
            return accum + fit;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment)
        {
			
            if (!(Experiment.timeSteps % (int)(1 / Experiment.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }

            if(!allLeft)
            {
                bool allOut = true;
            for (int j = 0; j < Experiment.robots.Count; j++)
            {
                if(!environment.AOIRectangle.Contains((int)Experiment.robots[j].location.x,(int)Experiment.robots[j].location.y))
                {
                    allOut = false;
                    break;
                }
            }
                if(allOut)
                    allLeft = true;
            }
			 
			grid = ((GridCollision)((MultiAgentExperiment)Experiment).collisionManager).grid;
			
            
            int dim = grid.coarseness;
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    int gx = (int)((double)x * grid.gridx) + (int)(grid.gridx / 2.0);
                    int gy = (int)((double)y * grid.gridy) + (int)(grid.gridy / 2.0);
                    if ((environment.AOIRectangle.Contains(gx, gy)))
                        accum += grid.grid[x, y].viewed;
                }
            }

            //grid.decay_viewed(.9);
        }

        void IFitnessFunction.reset()
        {
            accum = 0.0;
            allLeft = false;
			if(grid!=null)
			grid.reset_viewed();
        }

        #endregion
    }
}
