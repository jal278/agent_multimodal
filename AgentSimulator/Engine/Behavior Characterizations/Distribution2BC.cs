using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class Distribution2BC : IBehaviorCharacterization
    {
		public	IBehaviorCharacterization copy() {
			return new Distribution2BC();
		}

		List<double> bc;
		double[,] xc;
		double[,] yc;
		bool initialized;
		int sample_rate=20;
		int count=0;
		int samples=0;
		public Distribution2BC()
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
			int count = samples;
			for(int i=0;i<exp.robots.Count;i++) {
			 double sumx=0.0,sumsqx=0.0,sumy=0.0,sumsqy=0.0;
			 for(int j=0;j<samples;j++) {
				double x=xc[i,j];
				double y=yc[i,j];
				sumx+=x;
				sumsqx+=(x*x);
				sumy+=y;
				sumsqy+=(y*y);	
			 }
				double meanx=sumx/count;
				double meansqx=sumsqx/count;
				double meany=sumy/count;
				double meansqy=sumsqy/count;
			 double varx = meansqx-(meanx*meanx);
			 double vary = meansqy-(meany*meany);
				if(varx<0) varx=0;
				if(vary<0) vary=0;
				double stdx=Math.Sqrt(varx);
				double stdy=Math.Sqrt(vary);
				
			 
			double disable_mult=1.0;
			 if(disabled) {
			 	disable_mult= -0.1;
			 }
			
			 double[] list={meanx*disable_mult,meany*disable_mult,stdx*disable_mult,stdy*disable_mult};
						 
			 int index=0;
				for(index=0;index<bc.Count;index+=4) {
					if(!disabled &&list[0] < bc[index])
						break;
					else if(disabled&&list[0] >bc[index])
						break;
				}
				bc.InsertRange(index,list);
			}
			
			/*
			for(int i=0;i<bc.Count;i++)
			Console.Write(bc[i].ToString()+ " ");
			Console.WriteLine();
			*/
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
