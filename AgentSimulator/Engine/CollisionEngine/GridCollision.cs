using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib;

namespace Engine
{
		
	public class GridCollision:CollisionManager {
		public Environment env;
		public List<Robot> rbts;
		public SimulatorExperiment exp;
		public collision_grid grid;
		public bool robots_collide=true;
		public int coarseness=20;
		public GridCollision() { }
		public override CollisionManager copy() { return new GridCollision(); }

		public override void Initialize (Environment e,SimulatorExperiment _exp,List<Robot> _rbts)
		{
			rbts=_rbts;
			env=e;
			exp=_exp;
            if (_exp is MultiAgentExperiment)
            {
                agentCollide = ((MultiAgentExperiment)_exp).agentsCollide;
                agentVisible = ((MultiAgentExperiment)_exp).agentsVisible;
            }
                grid = new collision_grid(e,coarseness);
			foreach (Wall w in env.walls)
			{
				grid.insert_into_grid(w);
			}
		}
		
		public override void SimulationStepCallback ()
		{
			//grid.reset_viewed();
			grid.reset_dynamic();
			foreach (Robot r in rbts)
			{
				grid.insert_into_grid(r);
			}
		}
		
		public override bool RobotCollide(Robot robot) {
			List<collision_grid_square> squares = 
				grid.circle(robot.location.x,robot.location.y,robot.radius);
			List<Wall> checked_walls = new List<Wall>();
			List<Robot> checked_robots = new List<Robot>();
			foreach(collision_grid_square square in squares)
			{
				//TODO
				if(this.agentCollide)
				foreach(SimulatorObject o in square.dynamic_objs)
				{
					Robot r = (Robot)o;
					
					if (r == robot)
						continue;
					
					if (!checked_robots.Contains(r))
					{
						if (EngineUtilities.collide(r,robot))
							return true;
						checked_robots.Add(r);
					}
				}
				
				foreach(SimulatorObject o in square.static_objs)
				{
					Wall w = (Wall)o;
					if(!checked_walls.Contains(w))
					{
						if(EngineUtilities.collide(robot,w))
						   return true;
						checked_walls.Add(w);
					}
				}
			}
			return false;
		}
		
		public void draw(Graphics g, CoordinateFrame frame) {
			grid.draw(g,frame);
		}
		
		public double RaycastGrid(double angle, double max_range, Point2D point, Robot owner,bool view, out SimulatorObject hitobj)
		{
			hitobj=null;
			Point2D casted = new Point2D(point);
            double distance = max_range;
			bool hit = false;
			
			bool hitRobot = false;
			
			//cast point casted out from the robot's center point along the sensor direction
			double sum_angle = angle+owner.heading;
			double cosval = Math.Cos(sum_angle);
			double sinval = Math.Sin(sum_angle);
			double add_valx = cosval * distance;
			double add_valy = sinval * distance;
			casted.x += add_valx;
			casted.y += add_valy;
			
			//      if(Double.IsNaN(casted.x))
			//      	Console.WriteLine("casted x nan " + cosval + " " + add_valx + " " + Math.Cos(sum_angle));
			//      if(Double.IsNaN(casted.y))
			//      	Console.WriteLine("casted y nan " + sinval + " " + add_valy + " " + Math.Sin(sum_angle));
			      
			   
			//create line segment from robot's center to casted point
			Line2D cast=new Line2D(point,casted);
					
			List<collision_grid_square> squares =
				grid.cast_ray(point.x,point.y,casted.x,casted.y);
			
			foreach(collision_grid_square square in squares)
			{
				//if we had at least one collision, then quit!
				if(hit)
					break;
				if(view && !owner.disabled) {
					if(owner.stopped)
					square.viewed2=1.0;

					square.viewed=1.0;
				}
				//if(view && square.viewed < health)
				//	square.viewed=health;
				
			//now do naive detection of collision of casted rays with objects in square
			//first for all walls
			foreach (SimulatorObject obj in square.static_objs)
			{
				Wall wall = (Wall)obj;

				bool found=false;
				Point2D intersection = wall.line.intersection(cast,out found);
				if (found)
				{
					double new_distance = intersection.distance(point);
					if (new_distance<distance) {
						distance=new_distance;
						hitobj=wall;
					}
					hit=true;
					
				}
				
			}	
			
				//then for all robots
             	if(agentVisible)
				foreach  (SimulatorObject obj in square.dynamic_objs)
				{
					Robot r = (Robot) obj;
					bool found=false;
						
					if(r==owner)
						continue;
						
					double new_distance = cast.nearest_intersection(r.circle,out found);
						
					if(found)
					{
                         if (new_distance < distance)
                         {
                            distance = new_distance;
                            hitRobot = true;
							hitobj=r;
                         }
						 hit=true;
				    }						
			    }
			}
            return distance;
		}
			
		public override double Raycast(double angle, double max_range, Point2D point, Robot owner, out SimulatorObject hit) {
			RaycastGrid(angle+3.14,max_range/2.0,point,owner,true,out hit);
			RaycastGrid(angle,max_range/2.0,point,owner,true,out hit);
			double dist= RaycastGrid(angle,max_range,point,owner,false,out hit);
			return dist;
		}
	}
}
