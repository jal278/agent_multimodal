using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class IdleBC : IBehaviorCharacterization
    {
		public	IBehaviorCharacterization copy() {
			return new IdleBC();
		}

		
        public double accum, averageIdleness;
   
		collision_grid grid;
		int dim ;

		List<double> bc;
		bool initialized;
		public IdleBC()
		{
            accum = 0.0;
            averageIdleness = 0.0f;
			initialized=false;
			bc=new List<double>();
		}
		
        #region IBehaviorCharacterization Members

        string IBehaviorCharacterization.name
        {
            get { return this.GetType().Name; }
        }

        string IBehaviorCharacterization.description
        {
            get { return "Vector of robot's ending locations appended together"; }
        }


        //characterizing behavior...
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment exp)
        {   
			double mult=1.0;
			bool disabled=false;
			for(int i=0;i<exp.robots.Count;i++) {
			if(exp.robots[i].disabled) disabled=true;
			}
			if(disabled) mult= -0.1;
			
			for(int x=0;x<dim;x++)
				for(int y=0;y<dim;y++)
					bc.Add((grid.grid[x,y].avg_idle+1)/10000.0*mult);
		//	Console.WriteLine(bc.Count.ToString());
            return new List<double>(bc);
        }

        void IBehaviorCharacterization.update(SimulatorExperiment exp)
		{
			grid = ((GridCollision)((MultiAgentExperiment)exp).collisionManager).grid;
            dim = grid.coarseness;
            if (exp.timeSteps <=1)
            {
                for (int x = 0; x < dim; x++)
                {
                    for (int y = 0; y < dim; y++)
                    {
                        int gx = (int)((double)x * grid.gridx) + (int)(grid.gridx / 2.0);
                        int gy = (int)((double)y * grid.gridy) + (int)(grid.gridy / 2.0);
                        grid.grid[x, y].viewed = 0.0f;
                        grid.grid[x, y].idleness = 0.0f;  
						grid.grid[x, y].avg_idle=0.0f;
					}
                }
            }

            //if(!(Experiment.timeSteps % 5 ==0))
            //{
            //    grid.decay_viewed(0.3);
            //    return;
            //}

            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    int gx = (int)((double)x * grid.gridx) + (int)(grid.gridx / 2.0);
                    int gy = (int)((double)y * grid.gridy) + (int)(grid.gridy / 2.0);
                    if ((exp.environment.AOIRectangle.Contains(gx, gy)))
                    {
                        if (grid.grid[x, y].viewed>=0.95f)
                        {
                            grid.grid[x, y].idleness = 0.0f;
                        }
                        else
                        {
                            if (grid.grid[x, y].idleness<255)
                                grid.grid[x, y].idleness += 1.0f;
                            
                            grid.grid[x,y].avg_idle += grid.grid[x, y].idleness;
                        }
                        //accum +=
                    }
                }
            }
			
            
        	
		}
		
		void IBehaviorCharacterization.reset() 
		{
			accum=0.0;
			bc.Clear();
		}
        #endregion
    }
}
