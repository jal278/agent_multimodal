using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class DistanceBC : IBehaviorCharacterization
    {
		public	IBehaviorCharacterization copy() {
			return new DistanceBC();
		}

		List<double> bc;
		double[,] xc;
		double[,] yc;
		bool initialized;
		int sample_rate=30;
		int count=0;
		int samples=0;
		public DistanceBC()
		{
			const int maxsamples = 100;
			const int maxrobots = 10; 
				     initialized=true;
				     xc=new double[maxrobots,maxsamples];
				     yc=new double[maxrobots,maxsamples];
				  
		
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
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment exp,instance_pack ip)
        {   
	
			for(int i=0;i<ip.robots.Count;i++) {
					
				double d=0.0;
			 for(int j=1;j<samples;j++) {
				double dx=xc[i,j]-xc[i,j-1];
				double dy=yc[i,j]-yc[i,j-1];
				d+=dx*dx+dy*dy;
				}			 
			 if(ip.robots[i].disabled) d *= -0.1; 
			 bc.Add(d);
			 bc.Sort();
				
		    }

			//Console.WriteLine(bc.Count.ToString());
            return new List<double>(bc);
        }

        void IBehaviorCharacterization.update(SimulatorExperiment exp,instance_pack ip)
		{
			
			if(count%sample_rate==0) {
			 int rc=0;
			 foreach(Robot r in ip.robots) {
			  xc[rc,samples]=r.location.x;
			  yc[rc,samples]=r.location.y;
			  rc++;
			 }
			 //Console.WriteLine(samples);
		     samples++;
			}
			count++;
			
		}
		
		void IBehaviorCharacterization.reset() 
		{
			count=0;
			samples=0;
			bc.Clear();
		}
        #endregion
    }
}
