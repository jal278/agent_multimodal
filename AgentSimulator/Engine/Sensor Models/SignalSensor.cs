using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Engine
{
	
	public class SignalSensor : ISensor
	{
		double val=0.0;
	public SignalSensor(Robot o)
        {
			owner=o;
			val=0.0;
		}
	public void setSignal(double v) {
			val=v;
		}
		
		
		public double get_value() {
			return val;
		}
		public double get_value_raw() {
			return  val;
		}
      
		public void update(Environment env, List<Robot> robots,CollisionManager cm) 
		{
		}

		public void draw(Graphics g, CoordinateFrame frame)
        {
			Point a = frame.to_display((float)(owner.location.x),(float)(owner.location.y));
	        //Point b = frame.to_display((float)(a.X+Math.Cos(startAngle+owner.heading)*distance),
			 //                          (float)(a.Y+Math.Sin(startAngle+owner.heading)*distance));
			//Point c = frame.to_display((float)(a.X+Math.Cos(endAngle+owner.heading)*distance),
			 //                          (float)(a.Y+Math.Sin(endAngle+owner.heading)*distance));
			Pen pen = EngineUtilities.RedPen;
			if(val>0.5) pen=EngineUtilities.GreendPen;
			g.DrawRectangle(pen, a.X, a.Y,8,8);
		}

		public Robot owner;
		
        
	}
	
}
