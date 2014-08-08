using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class DistributionYC : IBehaviorCharacterization
    {
		List<double> bc;
		double[,] xc;
		double[,] yc;
		bool initialized;
		int sample_rate=50;
		int count=0;
		int samples=0;
		public DistributionBC()
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
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment exp)
        {   
			bool disabled=false;
			for(int i=0;i<exp.robots.Count;i++) {
			if(exp.robots[i].disabled) disabled=true;
			}
			
			for(int i=0;i<exp.robots.Count;i++) {
			 double minx=1000,miny=1000,maxx=-1000,maxy=-1000;
			 for(int j=0;j<samples;j++) {
				if(xc[i,j]<minx) minx=xc[i,j];
				if(xc[i,j]>maxx) maxx=xc[i,j];
				if(yc[i,j]<miny) miny=yc[i,j];
				if(yc[i,j]>maxy) maxy=yc[i,j];
			 }
			 disabled=false;//disable for now...
			 if(disabled) {
			  minx *= -0.1;
			  maxx *= -0.1;
			  miny *= -0.1;
			  maxy *= -0.1;
			 }
			 //bc.Add(minx);
			 bc.Add(miny);
			 //bc.Add(maxx);
			 bc.Add(maxy);
			 
			}

		//	Console.WriteLine(bc.Count.ToString());
            return new List<double>(bc);
        }

        void IBehaviorCharacterization.update(SimulatorExperiment exp)
		{
			
			if(count%sample_rate==0) {
			 int rc=0;
			 foreach(Robot r in exp.robots) {
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
