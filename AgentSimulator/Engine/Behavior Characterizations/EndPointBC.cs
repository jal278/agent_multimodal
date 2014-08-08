using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class EndPointBC : IBehaviorCharacterization
    {
		public	IBehaviorCharacterization copy() {
			return new EndPointBC();
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
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment exp,instance_pack ip)
        {
          
            //TODO reimplement
            List<double> bc = new List<double>();
			
            for (int agentIndex = 0; agentIndex < ip.robots.Count; agentIndex++)
            {
				double x;
				double y;
                if(exp.environment.AOIRectangle.Contains((int)ip.robots[agentIndex].location.x, (int)ip.robots[agentIndex].location.y))
                {
                    x = ip.robots[agentIndex].location.x;
                    y = ip.robots[agentIndex].location.y;
                    x = (x - exp.environment.AOIRectangle.Left) / exp.environment.AOIRectangle.Width;
                    y = (y - exp.environment.AOIRectangle.Top) / exp.environment.AOIRectangle.Height;
					
					if(!ip.robots[agentIndex].corrected) {
						x*= -0.1;
						y*= -0.1;
					}
                    
                }
                else
				{ 
			        x=-0.1;
                    y=-0.1;
				}  
				int k;
				for(k=0;k<bc.Count;k+=2) {
					if(x<bc[k]) {
						break;
					}
				}
				bc.Insert(k,y);
                bc.Insert(k,x);
                
                    
            }
            return bc;
        }

        void IBehaviorCharacterization.update(SimulatorExperiment exp,instance_pack ip)
		{
		}
		
		void IBehaviorCharacterization.reset() 
		{
		}
        #endregion
    }
}
