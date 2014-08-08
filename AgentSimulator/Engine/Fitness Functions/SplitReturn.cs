

using System;
using System.Collections.Generic;
using System.Text;
using Engine;
namespace Engine
{


	class SplitReturn:IFitnessFunction
    {
        #region IFitnessFunction Members

        double fitness = 0;
		List<int>[] travelList;
		int[] currentLocation;
		double[] finishList;
        //int steps = 0;
		public IFitnessFunction copy()
		{
			return new SplitReturn();
		}
		
		public SplitReturn()
        {
			const int maxsamples = 100;
			const int maxrobots = 10; 
			travelList=new List<int>[maxrobots];
			for(int i=0;i<maxrobots;i++)
				travelList[i]=new List<int>();
			currentLocation=new int[maxrobots];
			finishList=new double[maxrobots];
		this.reset0();
		}
		void reset0() {
			for(int i=0;i<currentLocation.Length;i++)
				currentLocation[i]= -1;
			for(int i=0;i<travelList.Length;i++) 
				travelList[i].Clear();
			for(int i=0;i<finishList.Length;i++)
				finishList[i]=0;
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
			double[] pmax=new double[3];
		    double[] prog=new double[3];
			double fitness=1.0;
			pmax[0]=0;
			pmax[1]=0;
			pmax[2]=0;
            for(int i=0;i<engine.robots.Count;i++) {
				
				
				int up_progress= -1;
				int up_type=0;
				int down_progress= 1000;
				double progress= 0;
				
				bool debug=false;
				if(travelList[i].Count==0) continue;
				int up_ind=0;
				for(int z=0;z<travelList[i].Count;z++) {
					int d=travelList[i][z];
					if (debug) Console.Write(d+" ");
					if ((d%3)>up_progress) {
					up_progress=d%3;
					up_ind=z;
					up_type=d/3;
					}
				}
				if(debug) Console.WriteLine();
				for(int z=up_ind;z<travelList[i].Count;z++) {
					int d=travelList[i][z];
					
					if ((d%3)<down_progress && (up_type==d/3)) {
					down_progress=d%3;
					}
				}
				progress= (up_progress+1) + (up_progress - down_progress);
				//if(finishList[i]==1) 
				progress+=finishList[i]*10.0;
				prog[i]=progress;
				if(progress>pmax[up_type])
					pmax[up_type]=progress;
			}
			double limit=Math.Min(Math.Min(prog[0],prog[1]),prog[2]);
			double differential=Math.Max(Math.Max(prog[0]-limit,prog[1]-limit),prog[2]-limit);
			for(int i=0;i<3;i++) {
				fitness+=pmax[i]*5;
				fitness+=prog[i];
				fitness-=differential*2;				
			}
            return Math.Max(0.00001,fitness);
        }

		void IFitnessFunction.update(SimulatorExperiment engine, Environment environment)
		{
				
            if (!(engine.timeSteps % (int)(0.5 / engine.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }
			 
            double k;
			int r_ind=0;
			int p_ind=0;
			foreach (Robot r in engine.robots) {
				p_ind=0;
				//Console.WriteLine(r.sensors.Count);
				//Console.WriteLine(engine.agentBrain.brain.InputNeuronCount);
			 foreach (System.Drawing.Point p in environment.POIPosition) { 
					
			 	Point2D point= new Point2D((double)p.X,(double)p.Y);
				if(r.location.distance(point) < 15.0f) {
				 if(currentLocation[r_ind]!=p_ind) {
				//			Console.WriteLine(p_ind);
				   currentLocation[r_ind]=p_ind;
				   travelList[r_ind].Add(p_ind);
				 }
				}
				p_ind+=1;
			}
			
				finishList[r_ind]=Math.Max(0.0,(100.0-environment.goal_point.distance(r.location))/100.0);
				r_ind+=1;
			}
							
            return;		
		}
		void IFitnessFunction.reset() 
		{
        	reset0();				
		}
        #endregion
    }
}
