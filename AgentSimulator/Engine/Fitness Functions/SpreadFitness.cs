using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class SpreadFitness : IFitnessFunction
    {
		public int coll_count;
		public double dist_trav;
        public double accum;
		public double stopaccum;

		public IFitnessFunction copy()
		{
			return new SpreadFitness();
		}
		
        public SpreadFitness()
        {
			dist_trav=0.0;
			coll_count=0;			
            accum = 0.0001;
			stopaccum=0.0001;
        }
        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness Awarded for Coverage of Walls"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment,instance_pack ip,out double[] objectives)
        {
			objectives=new double[6];
        	objectives[0]=(accum+2.5*stopaccum) / 1000.0;
			//objectives[0]=accum;
			//objectives[1]=stopaccum;

			double travel=0.0;
			double delta=0.0;
			double sight=0.0;
			foreach(Robot r in ip.robots) {
		        delta += ((Khepera3RobotModel)r).totDelta; //r.collisions; //sensorList[i];//engine.robots[i].collisions;
			  	sight += ((Khepera3RobotModel)r).totSight;
				travel+=r.dist_trav;
			}
			objectives[1]= -delta; // + -sight; //-Math.Log(delta+1.0)/10.0;
			//objectives[2]= -Math.Log(sight+1.0)/10.0;
			//objectives[1]= -coll_count;
			//objectives[1]=travel;
			//Console.WriteLine(ip.robots.Count);
			double fitness=(accum+3.5*stopaccum)*(3.0*(ip.robots.Count-3))-delta*20+0.0001;
        	return fitness;
		}
		
		double nearest(instance_pack ip, Robot robot,Environment env) {
			double nd=1000000.0;
			bool found =false;
			Point2D p = robot.location;
			foreach(Robot r in ip.robots) {
				if(r != robot && env.AOIRectangle.Contains((int)r.location.x,(int)r.location.y)) {
				double d=r.location.distance(p);
				if(d<nd) nd=d;
				found=true;
				//nd+=d;
				}
			}
			
			if(found)
				return nd;
			else
				return 0.0;
		}
		
        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment,instance_pack ip)
        {			
            if (!(ip.timeSteps % (int)(1 / ip.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }
			bool all_in=true;
			double a_accum=0.00000001;
			double a_saccum=0.00000001;
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
				
				if ((environment.AOIRectangle.Contains((int)r.location.x,(int)r.location.y)))
				{
				a_accum+=1.0/(nearest(ip,r,environment));
				if(r.corrected)
					a_saccum+=nearest(ip,r,environment);
				//else
				//	a_saccum+=1.0;
					
				}
				else {
					all_in=false;
				}
			}
			
				if(all_in) {
					accum+=((double)ip.robots.Count)/(a_accum);
					stopaccum+=a_saccum/((double)ip.robots.Count);
				}
        }

        void IFitnessFunction.reset()
        {
			dist_trav=0.0;
			coll_count=0;
            accum = 0.0001;
			stopaccum=0.0001;
        }

        #endregion
    }
}
