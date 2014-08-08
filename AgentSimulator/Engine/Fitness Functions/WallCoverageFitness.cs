using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class WallCoverageFitness : IFitnessFunction
    {
		public int coll_count;
		public double dist_trav;
        public double accum;
		
		public IFitnessFunction copy()
		{
			return new WallCoverageFitness();
		}
		
        public WallCoverageFitness()
        {
			dist_trav=0.0;
			coll_count=0;			
            accum = 0.0;
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
        	objectives[0]=100000.0/(accum+1.0);
			objectives[0]*=1000.0;
			double travel=0.0;
			foreach(Robot r in ip.robots) {
		        coll_count += r.collisions; //sensorList[i];//engine.robots[i].collisions;
				travel+=r.dist_trav;
			}
			objectives[1]= -coll_count;
			//objectives[1]=travel;
            return objectives[0];
        }
		
		double nearest(instance_pack ip, double x, double y) {
			double nd=10000000.0;
			Point2D p = new Point2D(x,y);
			foreach(Robot r in ip.robots) {
				double d=r.location.distance(p);
				if(d<nd) nd=d;
			}
			return nd;
		}
		double test_interpolation(instance_pack ip, double x1, double y1, double x2, double y2, int steps) {
			double acc=0.0;
			double ix = x1;
			double iy = y1;
			double dx = (x2-x1)/(steps-1);
			double dy = (y2-y1)/(steps-1);
			for(int k=0;k<steps;k++) {
				acc+=nearest(ip,ix,iy);
				ix+=dx;
				iy+=dy;
			}
			return acc;
		}
		
        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment,instance_pack ip)
        {
			
            if (!(Experiment.timeSteps % (int)(1 / Experiment.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
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
			
			
			
			double x1=(double)environment.AOIRectangle.Left;		
			double y1=(double)environment.AOIRectangle.Top;		
			double x2=(double)environment.AOIRectangle.Right;		
			double y2=(double)environment.AOIRectangle.Bottom;		
			int steps=10;
			accum+=test_interpolation(ip,x1,y1,x2,y1,steps);
			accum+=test_interpolation(ip,x2,y1,x2,y2,steps);
			accum+=test_interpolation(ip,x2,y2,x2,y1,steps);
			accum+=test_interpolation(ip,x2,y1,x1,y1,steps);

        }

        void IFitnessFunction.reset()
        {
			dist_trav=0.0;
			coll_count=0;
            accum = 0.0;
       }

        #endregion
    }
}
