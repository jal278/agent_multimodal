using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class SplitReturnBC : IBehaviorCharacterization
    {
		List<double> bc;
    
      
        double fitness = 0;
		List<int>[] travelList;
		int[] currentLocation;
		int[] takenList;
		bool[] reachList;
		double[] origDist;
		double[] gotoList;
		double[] endList;
		bool first;
		bool last;
        //int steps = 0;
		public	IBehaviorCharacterization copy() {
			return new SplitReturnBC();
		}

		public SplitReturnBC()
        {
			first=false;
			last=false;
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
			bc = new List<double>();
			this.reset0();
		}
		
		void reset0() {
			bc.Clear();
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
			last=false;
		}
		
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment engine,instance_pack ip)
        {
			Environment environment = engine.environment;
			bc.Clear();
			double gotosum=1.0;
			double endsum=1.0;
			for(int i=0;i<ip.robots.Count;i++)
			{
				bc.Add(gotoList[i]);
				bc.Add(endList[i]);
			}
			bc.Sort();
			return new List<double>(bc);
        }

		void IBehaviorCharacterization.update(SimulatorExperiment engine,instance_pack ip)
	
		{
			Environment environment = engine.environment;
	   if (!(engine.timeSteps % 25 ==0)) //(int)(0.5 / engine.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }
	
			double[] gl=new double[3];
			double[] el=new double[3];
			
					for(int i=0;i<engine.robots.Count;i++)
					 	gl[i]=0.0;
			
					for(int i=0;i<engine.robots.Count;i++)
						el[i]=0.0;
				int r_ind=0;
			    if(!last && (engine.timeSteps * engine.timestep)>11.0) {
					foreach(Robot r in ip.robots) {
				
					last=true;
					}
				}
			/*
				if(!first && (engine.timeSteps * engine.timestep )> 6.0) {
				foreach(Robot r in engine.robots) {
				 SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count-1];
					s.setSignal(1.0);
				
					 origDist[r_ind]=r.location.distance(environment.goal_point);
					r_ind++;
				}
					first=true;	
				  r_ind=0;
				}
			*/
			
			foreach (Robot r in ip.robots) {
				
				double d2 = r.location.distance(environment.goal_point);
				if(first) {
				   	//if(point.distance(environment.goal_point) <25.0) {
					//	endList[i]=1.5;
					//}
					//else {
						el[r_ind]=Math.Max(el[r_ind],(origDist[r_ind]-d2)/200.0);
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
				
				 int p_ind=0;
				foreach (System.Drawing.Point p in environment.POIPosition) {
				Point2D point= new Point2D((double)p.X,(double)p.Y);
				int i =p_ind;
				double d1 = point.distance(r.location);
				
				if(!first) {
					gl[i]= Math.Max(gl[i],(200.0-d1)/200.0);
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
		void IBehaviorCharacterization.reset() 
		{
        reset0();
							
		}
	
	

        string IBehaviorCharacterization.name
        {
            get { return this.GetType().Name; }
        }

        string IBehaviorCharacterization.description
        {
            get { return "Vector of robot's ending locations appended together"; }
        }
       
    }
}
