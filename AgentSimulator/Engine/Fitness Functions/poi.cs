

using System;
using System.Collections.Generic;
using System.Text;
using Engine;
namespace Engine
{


	class POIFIT:IFitnessFunction
    {
		public IFitnessFunction copy()
		{
			return new POIFIT();
		}
		
        #region IFitnessFunction Members
		SimulatorExperiment theengine=null;
        double fitness = 0;
		List<int>[] travelList;
		int[] currentLocation;
		int[] takenList;
		bool[] reachList;
		double[] gotoList;
		double[] endList;
		double[] origDist;
		bool first;
        //int steps = 0;
		public POIFIT()
        {
			first=false;
			const int maxsamples = 100;
			const int maxrobots = 10; 
			travelList=new List<int>[maxrobots];
			for(int i=0;i<maxrobots;i++)
				travelList[i]=new List<int>();
			currentLocation=new int[maxrobots];
			endList=new double[maxrobots];
			gotoList=new double[maxrobots];
			origDist=new double[maxrobots];
			
			reachList=new bool[maxrobots];
			takenList=new int[maxrobots];
			theengine=null;
			this.reset0();
		}
		
		void reset0() {
			
			
			for(int i=0;i<currentLocation.Length;i++)
				currentLocation[i]= -1;
			for(int i=0;i<travelList.Length;i++) 
				travelList[i].Clear();
			for(int i=0;i<gotoList.Length;i++)
				gotoList[i]=0;
			for(int i=0;i<endList.Length;i++)
				endList[i]=0;
			for(int i=0;i<origDist.Length;i++)
				origDist[i]=0;
			first=false;
		}
		
        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness is the sum of the distance of each agent from goal point, each substracted from 1000.  The optimal score is 1000*# of agents."; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment,out double[] obj)
        {
			obj=null;
			fitness=0.000001;
			for(int i=0;i<engine.robots.Count;i++)
			{
				fitness+=gotoList[i];
				fitness+=endList[i];
			}
			
			return fitness;
            //return Math.Max(0.00001,fitness);
        }

		void IFitnessFunction.update(SimulatorExperiment engine, Environment environment)
		{
            if (!(engine.timeSteps % 25 ==0)) //(int)(0.5 / engine.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }
			

			double[] gl=new double[3];
			double[] el=new double[3];
			
				if(!first) {
					for(int i=0;i<engine.robots.Count;i++)
					 	gl[i]=0.0;
				}
				else {
					for(int i=0;i<engine.robots.Count;i++)
						el[i]=0.0;
				}
				int r_ind=0;
				if(!first && (engine.timeSteps * engine.timestep )> 6.0) {
				foreach(Robot r in engine.robots) {
				 SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count-1];
					s.setSignal(1.0);
					 origDist[r_ind]=r.location.distance(environment.goal_point);
					r_ind++;
				}
				r_ind=0;
					first=true;	
				}
			
			
			foreach (Robot r in engine.robots) {
				int p_ind=0;
				double d2 = r.location.distance(environment.goal_point);
				if(first) {
				   	//if(point.distance(environment.goal_point) <25.0) {
					//	endList[i]=1.5;
					//}
					//else {
						el[r_ind]=Math.Max(0.0,(origDist[r_ind]-d2)/200.0);
					//}
				}
				/*
				else {
					System.Drawing.Point p=environment.POIPosition[r_ind];
					Point2D point= new Point2D((double)p.X,(double)p.Y);
					double d1=point.distance(r.location);
					gl[r_ind]=Math.Max(0.0,(200.0-d1)/200.0);
				}
				*/
				
				foreach (System.Drawing.Point p in environment.POIPosition) {
				Point2D point= new Point2D((double)p.X,(double)p.Y);
				int i =p_ind;
				double d1 = point.distance(r.location);
				
				if(!first) {
					gl[i]= Math.Max(gl[i],(200.0-d1)/200.0*5.0);
				}
				
				p_ind+=1;
				}
			    
				r_ind+=1;
				
				
			}
			
			for(int i=0;i<3;i++) {
					gotoList[i]+=gl[i];
					endList[i]+=el[i];
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
