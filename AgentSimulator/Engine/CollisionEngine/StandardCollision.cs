
using System;
using System.Collections.Generic;

namespace Engine
{

	public class StandardCollision:CollisionManager {
		public Environment env;
		public SimulatorExperiment exp;
		public List<Robot> rbts;
		public StandardCollision() { }
		public override CollisionManager copy() { return new StandardCollision(); }

		public override void Initialize (Environment e,SimulatorExperiment _exp,List<Robot> _rbts)
		{
			rbts = _rbts;
			exp=_exp;
            if (_exp is MultiAgentExperiment)
            {
                agentCollide = ((MultiAgentExperiment)exp).agentsCollide;
                agentVisible = ((MultiAgentExperiment)exp).agentsVisible;
            }
            env = e;
			
		}
		
		public override bool RobotCollide (Robot robot)
		{
			foreach (Wall wall in env.walls)
            {
                if (EngineUtilities.collide(robot, wall))
                {
                    return true;
                }
            }
			if(!agentCollide) return false;
			
			foreach (Robot robot2 in rbts)
            {
                 if (robot == robot2)
                        continue;
                 if (EngineUtilities.collide(robot, robot2))
                 {
                       return true;
                 }
            }
			
			return false;
		}
		
		public override double Raycast (double angle, double max_range, Point2D point, Robot owner, out SimulatorObject hit)
		{
			hit=null;
			bool hitRobot=false;
	        Point2D casted = new Point2D(point);
            double distance = max_range;

            //cast point casted out from the robot's center point along the sensor direction
            casted.x += Math.Cos(angle + owner.heading) * distance;
            casted.y += Math.Sin(angle + owner.heading) * distance;

            //create line segment from robot's center to casted point
            Line2D cast = new Line2D(point, casted);

            //TODO remove
            //now do naive detection of collision of casted rays with objects
            //first for all walls
            foreach (Wall wall in env.walls)
            {
                if (!wall.visible)
                    continue;
                bool found = false;
                Point2D intersection = wall.line.intersection(cast, out found);
                if (found)
                {
                    double new_distance = intersection.distance(point);
                    if (new_distance < distance) {
                        distance = new_distance;
						hit=wall;
					}
                }
            }

            //then for all robots
            hitRobot = false;
			if(!agentVisible)
            	return distance;

            //if (agentsVisible)
            foreach (Robot robot2 in rbts)
            {
                bool found = false;

                if (robot2 == owner)
                    continue;

                double new_distance = cast.nearest_intersection(robot2.circle, out found);

                if (found)
                {
                    if (new_distance < distance)
                    {
                        distance = new_distance;
                        hitRobot = true;
						hit=robot2;
                    }
                }
            }
            return distance;
		}
	}
}
