using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Engine
{
	
	public class Radar : ISensor    //TODO does this class work? Either delete or make own class out of it
	{
	public Radar(double sA, double eA,Robot o)
        {
			owner = o;
           
            startAngle = sA;
            endAngle = eA;
            max_range = 100.0;
            distance = (-1);
            noise = 0.0;
        }
		
		public double get_value() {
			return distance/max_range;
		}
		public double get_value_raw() {
			return distance;
		}
      
		public void update(Environment env, List<Robot> robots,CollisionManager cm) 
		{
			offsetx = Math.Cos(owner.heading+delta_theta)*delta_r;
			offsety = Math.Sin(owner.heading+delta_theta)*delta_r;
		}

		public void draw(Graphics g, CoordinateFrame frame)
        {
			Point a = frame.to_display((float)(owner.location.x+offsetx),(float)(owner.location.y+offsety));
	        Point b = frame.to_display((float)(a.X+Math.Cos(startAngle+owner.heading)*distance),
			                           (float)(a.Y+Math.Sin(startAngle+owner.heading)*distance));
			Point c = frame.to_display((float)(a.X+Math.Cos(endAngle+owner.heading)*distance),
			                           (float)(a.Y+Math.Sin(endAngle+owner.heading)*distance));
			g.DrawLine(EngineUtilities.GreendPen, a, b);
			g.DrawLine(EngineUtilities.GreendPen, a, c);
			g.DrawLine(EngineUtilities.GreendPen, b, c);
		}

		public Robot owner;
		
        public double startAngle;
        public double endAngle;

		public double delta_r=0.0;
		public double delta_theta=0.0;
		public double offsetx=0.0;
		public double offsety=0.0;
		
        public bool hitTerrorist;
        public bool hitRobot;
        public double distance;
        public double max_range;
        public double noise; //for broken sensors?
	}
	
    public class RangeFinder : ISensor
    {
		public bool seeRobot;
		public RangeFinder(double a,Robot o,double _max_range,double _noise)
        {
			seeRobot=false;
			owner =o;
            
            angle = a;
            max_range = _max_range;///EPUCK 15.0; 30 works   //epuck radius = 3.8
            distance = (-1);

            noise = _noise / 100.0;
        }
		
		public double get_value() {
			return distance/max_range;
		}
		public double get_value_raw() {
			return distance;
		}
      
      
		public void update(Environment env, List<Robot> robots,CollisionManager cm)
		{
			bool hitRobot;
            offsetx = Math.Cos(owner.heading+delta_theta)*delta_r;
			offsety = Math.Sin(owner.heading+delta_theta)*delta_r;
			Point2D location = new Point2D(offsetx + owner.location.x, offsety + owner.location.y);
			SimulatorObject hit;			
			distance = cm.Raycast(angle,max_range,location,owner,out hit);

			if (hit is Robot) {
				seeRobot=true;			
			}
			else {
				seeRobot=false;
			}
			//apply sensor noise
			/*
			if(noise>0.0) {
				distance*= 1.0 + (noise * (owner.rng.NextDouble()-0.5)*2.0);				
				if(distance>max_range)
					distance=max_range;
				if(distance<0.0)
					distance=0.0;
			}
			*/
            Debug.Assert(!Double.IsNaN(distance), "NaN in inputs");
		}

		public void set_delta(double dr,double dt) { delta_r=dr; delta_theta=dt; }

		public void draw(Graphics g, CoordinateFrame frame)
        {
			Point a = frame.to_display((float)(owner.location.x+offsetx),(float)(owner.location.y+offsety));
	        Point b = frame.to_display((float)(owner.location.x+offsetx+Math.Cos(angle+owner.heading)*distance),
			                           (float)(owner.location.y+offsety+Math.Sin(angle+owner.heading)*distance));
			g.DrawLine(EngineUtilities.GreendPen, a, b);
		}
		
		public Robot owner;
		
      
		public double delta_r=0.0;
		public double delta_theta=0.0;
		public double offsetx=0.0;
		public double offsety=0.0;
		
        public bool hitTerrorist;
        public bool hitRobot;
        public double angle;
        public double distance;
        public double max_range;
        public double noise; //for broken sensors?
    }
}
