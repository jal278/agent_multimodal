using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class CoverageFitness : IFitnessFunction
    {
		public int coll_count;
		public double dist_trav;
        public double accum;
		public double stop_accum;
		public collision_grid grid;
		
		public IFitnessFunction copy()
		{
			return new CoverageFitness();
		}
		
        public CoverageFitness()
        {
			dist_trav=0.0;
			coll_count=0;			
			grid=null;
            accum = 0.0;
			stop_accum=0.0;
        }
        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for Coverage of Map"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment,instance_pack ip,out double[] objectives)
        {
			objectives=new double[6];
        	objectives[0]=accum;
			double travel=0.0;
			foreach(Robot r in ip.robots) {
				travel+=r.dist_trav;
				coll_count+=r.collisions;
			}
			objectives[1]= stop_accum; //-collisions;
            return accum+stop_accum*2.0;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment,instance_pack ip)
        {
			
            grid = ((GridCollision)(ip.collisionManager)).grid;
			
            if (!(ip.timeSteps % (int)(1 / Experiment.timestep) == 0))
            {
                return;
            }

            int dim = grid.coarseness;
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    int gx = (int)((double)x * grid.gridx) + (int)(grid.gridx / 2.0);
                    int gy = (int)((double)y * grid.gridy) + (int)(grid.gridy / 2.0);
                    if ((environment.AOIRectangle.Contains(gx, gy)))
                    {
                        accum += grid.grid[x, y].viewed;
						stop_accum+= grid.grid[x,y].viewed2;
                    }

                }
            }

			foreach(Robot r in ip.robots) {
				if (!r.autopilot) {
					foreach(ISensor s in r.sensors) 
						if(s is SignalSensor) {
						 SignalSensor ss = (SignalSensor)s;
						 double val = ss.get_value();
						 val+=0.05;
						 if(val>1.0) val=1.0;
						 ss.setSignal(val);
						}
				}
			}
			

            grid.decay_viewed(0);
            //grid.decay_viewed(.95);
        }

        void IFitnessFunction.reset()
        {
			dist_trav=0.0;
			coll_count=0;
            accum = 0.0;
			if(grid!=null)
			    grid.reset_viewed();
        }

        #endregion
    }
}
