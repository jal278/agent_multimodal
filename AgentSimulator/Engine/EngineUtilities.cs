using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib;

namespace Engine
{
	public class CoordinateFrame {
			public float cx,cy;
			public float scale;
			public float rotation;
			public CoordinateFrame(float _dx,float _dy,float _scale,float _rotation) {
				cx=_dx;
				cy=_dy;
				scale=_scale;
				rotation=_rotation;
			}
			public void sync_from_environment(Environment x) {
				cx=x.view_x;
				cy=x.view_y;
				scale=x.view_scale;
			}
			public void sync_to_environment(Environment x) {
				x.view_x=cx;
				x.view_y=cy;
				x.view_scale=scale;
			}
		    public Point to_display(float x,float y) {
			 float ox,oy;
			  to_display(x,y,out ox,out oy);
			 return new Point((int)ox,(int)oy);
			}
		
			public Point to_display(Point x) {
				//float ox,oy;
				return to_display(x.X,x.Y);
			}
		
			public Point2D to_display(Point2D p) {
				float ox,oy;
				to_display((float)p.x,(float)p.y,out ox, out oy);
				return new Point2D((double)ox,(double)oy);
			}
		
			//changes an input coordinate in simulator space to display space
			public void to_display(float ix,float iy,out float ox, out float oy) 
			{
				ox = (ix-cx)/scale;
			    oy = (iy-cy)/scale;
			}
		
			//changes an input coordinate in display space to simulator space
			public void from_display(float ix,float iy,out float ox, out float oy) 
			{
				ox = ix*scale+cx;
			    oy = iy*scale+cy;
			}
			
			public void offset_to_display(float ix, float iy, out float ox, out float oy)
			{
				ox = ix/scale;
				oy = iy/scale;
			}
		
			//input is a delta, not an absolute point in terms of screen coordinates
			//output is a delta in terms of simulator coordinates
			public void offset_from_display(float ix, float iy, out float ox, out float oy)
			{
				ox = ix*scale;
				oy = iy*scale;
			}
	}
	
	public abstract class CollisionManager {
		public abstract CollisionManager copy();
		public abstract void Initialize(Environment e,SimulatorExperiment _exp,List<Robot> rbts);
		public virtual void SimulationStepCallback() { }
		public abstract bool RobotCollide(Robot r);
		public abstract double Raycast(double angle, double max_range, Point2D point, Robot owner,out SimulatorObject hit);
		public bool agentVisible;
		public bool agentCollide;
	}

	
	
    public class EngineUtilities
    {
		
        //TODO: Maybe put this elsewhere so we can set the seed
		//TODO: One static rng will conflict if we ever want to reimplement multithreading
        public static Random random = new Random();

        #region Pen Colors
        public static Pen RedPen = new Pen(System.Drawing.Brushes.Red, 2.0f);
        public static Pen BluePen = new Pen(System.Drawing.Brushes.Blue, 2.0f);
        public static Pen GreendPen = new Pen(System.Drawing.Brushes.Green, 2.0f);
        public static Pen YellowPen = new Pen(System.Drawing.Brushes.Yellow, 2.0f);
        public static Pen DashedPen = new Pen(Brushes.Black, 1.0f);

        // Schrum: Different coloerd pen for each mode/module/brain
        public static Brush modePen(int mode)
        {
            Brush b;
            // Schrum: These colors were originally in alphabetical order,
            // but I manipulated the order manually to make the more common
            // (lower) values more distinctive.
            switch(mode) {
                case 0:
                    b = System.Drawing.Brushes.Brown;
                    break;
                case 1:
                    b = System.Drawing.Brushes.Chartreuse;
                    break;
                case 2:
                    b = System.Drawing.Brushes.Aqua;
                    break;
                case 3:
                    b = System.Drawing.Brushes.Cyan;
                    break;
                case 4:
                    b = System.Drawing.Brushes.Azure;
                    break;
                case 5:
                    b = System.Drawing.Brushes.Beige;
                    break;
                case 6:
                    b = System.Drawing.Brushes.Bisque;
                    break;
                case 7:
                    b = System.Drawing.Brushes.Black;
                    break;
                case 8:
                    b = System.Drawing.Brushes.BlanchedAlmond;
                    break;
                case 9:
                    b = System.Drawing.Brushes.Blue;
                    break;
                case 10:
                    b = System.Drawing.Brushes.BlueViolet;
                    break;
                case 11:
                    b = System.Drawing.Brushes.AliceBlue;
                    break;
                case 12:
                    b = System.Drawing.Brushes.BurlyWood;
                    break;
                case 13:
                    b = System.Drawing.Brushes.CadetBlue;
                    break;
                case 14:
                    b = System.Drawing.Brushes.AntiqueWhite;
                    break;
                case 15:
                    b = System.Drawing.Brushes.Chocolate;
                    break;
                case 16:
                    b = System.Drawing.Brushes.Coral;
                    break;
                case 17:
                    b = System.Drawing.Brushes.CornflowerBlue;
                    break;
                case 18:
                    b = System.Drawing.Brushes.Cornsilk;
                    break;
                case 19:
                    b = System.Drawing.Brushes.Crimson;
                    break;
                case 20:
                    b = System.Drawing.Brushes.Aquamarine;
                    break;
                case 21:
                    b = System.Drawing.Brushes.DarkBlue;
                    break;
                case 22:
                    b = System.Drawing.Brushes.DarkCyan;
                    break;
                case 23:
                    b = System.Drawing.Brushes.DarkGoldenrod;
                    break;
                case 24:
                    b = System.Drawing.Brushes.DarkGray;
                    break;
                case 25:
                    b = System.Drawing.Brushes.DarkGreen;
                    break;
                case 26:
                    b = System.Drawing.Brushes.DarkKhaki;
                    break;
                case 27:
                    b = System.Drawing.Brushes.DarkMagenta;
                    break;
                case 28:
                    b = System.Drawing.Brushes.DarkOliveGreen;
                    break;
                case 29:
                    b = System.Drawing.Brushes.DarkOrange;
                    break;
                case 30:
                    b = System.Drawing.Brushes.DarkOrchid;
                    break;
                case 31:
                    b = System.Drawing.Brushes.DarkRed;
                    break;
                case 32:
                    b = System.Drawing.Brushes.DarkSalmon;
                    break;
                case 33:
                    b = System.Drawing.Brushes.DarkSeaGreen;
                    break;
                case 34:
                    b = System.Drawing.Brushes.DarkSlateBlue;
                    break;
                case 35:
                    b = System.Drawing.Brushes.DarkSlateGray;
                    break;
                case 36:
                    b = System.Drawing.Brushes.DarkTurquoise;
                    break;
                case 37:
                    b = System.Drawing.Brushes.DarkViolet;
                    break;
                case 38:
                    b = System.Drawing.Brushes.DeepPink;
                    break;
                case 39:
                    b = System.Drawing.Brushes.DeepSkyBlue;
                    break;
                case 40:
                    b = System.Drawing.Brushes.DimGray;
                    break;
                case 41:
                    b = System.Drawing.Brushes.DodgerBlue;
                    break;
                case 42:
                    b = System.Drawing.Brushes.Firebrick;
                    break;
                case 43:
                    b = System.Drawing.Brushes.FloralWhite;
                    break;
                case 44:
                    b = System.Drawing.Brushes.ForestGreen;
                    break;
                case 45:
                    b = System.Drawing.Brushes.Fuchsia;
                    break;
                case 46:
                    b = System.Drawing.Brushes.Gainsboro;
                    break;
                case 47:
                    b = System.Drawing.Brushes.GhostWhite;
                    break;
                case 48:
                    b = System.Drawing.Brushes.Gold;
                    break;
                case 49:
                    b = System.Drawing.Brushes.Goldenrod;
                    break;
                case 50:
                    b = System.Drawing.Brushes.Gray;
                    break;
                case 51:
                    b = System.Drawing.Brushes.Green;
                    break;
                case 52:
                    b = System.Drawing.Brushes.GreenYellow;
                    break;
                case 53:
                    b = System.Drawing.Brushes.Honeydew;
                    break;
                case 54:
                    b = System.Drawing.Brushes.HotPink;
                    break;
                case 55:
                    b = System.Drawing.Brushes.IndianRed;
                    break;
                case 56:
                    b = System.Drawing.Brushes.Indigo;
                    break;
                case 57:
                    b = System.Drawing.Brushes.Ivory;
                    break;
                case 58:
                    b = System.Drawing.Brushes.Khaki;
                    break;
                case 59:
                    b = System.Drawing.Brushes.Lavender;
                    break;
                case 60:
                    b = System.Drawing.Brushes.LavenderBlush;
                    break;
                case 61:
                    b = System.Drawing.Brushes.LawnGreen;
                    break;
                case 62:
                    b = System.Drawing.Brushes.LemonChiffon;
                    break;
                case 63:
                    b = System.Drawing.Brushes.LightBlue;
                    break;
                case 64:
                    b = System.Drawing.Brushes.LightCoral;
                    break;
                case 65:
                    b = System.Drawing.Brushes.LightCyan;
                    break;
                case 66:
                    b = System.Drawing.Brushes.LightGoldenrodYellow;
                    break;
                case 67:
                    b = System.Drawing.Brushes.LightGray;
                    break;
                case 68:
                    b = System.Drawing.Brushes.LightGreen;
                    break;
                case 69:
                    b = System.Drawing.Brushes.LightPink;
                    break;
                case 70:
                    b = System.Drawing.Brushes.LightSalmon;
                    break;
                case 71:
                    b = System.Drawing.Brushes.LightSeaGreen;
                    break;
                case 72:
                    b = System.Drawing.Brushes.LightSkyBlue;
                    break;
                case 73:
                    b = System.Drawing.Brushes.LightSlateGray;
                    break;
                case 74:
                    b = System.Drawing.Brushes.LightSteelBlue;
                    break;
                case 75:
                    b = System.Drawing.Brushes.LightYellow;
                    break;
                case 76:
                    b = System.Drawing.Brushes.Lime;
                    break;
                case 77:
                    b = System.Drawing.Brushes.LimeGreen;
                    break;
                case 78:
                    b = System.Drawing.Brushes.Linen;
                    break;
                case 79:
                    b = System.Drawing.Brushes.Magenta;
                    break;
                case 80:
                    b = System.Drawing.Brushes.Maroon;
                    break;
                case 81:
                    b = System.Drawing.Brushes.MediumAquamarine;
                    break;
                case 82:
                    b = System.Drawing.Brushes.MediumBlue;
                    break;
                case 83:
                    b = System.Drawing.Brushes.MediumOrchid;
                    break;
                case 84:
                    b = System.Drawing.Brushes.MediumPurple;
                    break;
                case 85:
                    b = System.Drawing.Brushes.MediumSeaGreen;
                    break;
                case 86:
                    b = System.Drawing.Brushes.MediumSlateBlue;
                    break;
                case 87:
                    b = System.Drawing.Brushes.MediumSpringGreen;
                    break;
                case 88:
                    b = System.Drawing.Brushes.MediumTurquoise;
                    break;
                case 89:
                    b = System.Drawing.Brushes.MediumVioletRed;
                    break;
                case 90:
                    b = System.Drawing.Brushes.MidnightBlue;
                    break;
                case 91:
                    b = System.Drawing.Brushes.MintCream;
                    break;
                case 92:
                    b = System.Drawing.Brushes.MistyRose;
                    break;
                case 93:
                    b = System.Drawing.Brushes.Moccasin;
                    break;
                case 94:
                    b = System.Drawing.Brushes.NavajoWhite;
                    break;
                case 95:
                    b = System.Drawing.Brushes.Navy;
                    break;
                case 96:
                    b = System.Drawing.Brushes.OldLace;
                    break;
                case 97:
                    b = System.Drawing.Brushes.Olive;
                    break;
                case 98:
                    b = System.Drawing.Brushes.OliveDrab;
                    break;
                case 99:
                    b = System.Drawing.Brushes.Orange;
                    break;
                case 100:
                    b = System.Drawing.Brushes.OrangeRed;
                    break;
                case 101:
                    b = System.Drawing.Brushes.Orchid;
                    break;
                case 102:
                    b = System.Drawing.Brushes.PaleGoldenrod;
                    break;
                case 103:
                    b = System.Drawing.Brushes.PaleGreen;
                    break;
                case 104:
                    b = System.Drawing.Brushes.PaleTurquoise;
                    break;
                case 105:
                    b = System.Drawing.Brushes.PaleVioletRed;
                    break;
                case 106:
                    b = System.Drawing.Brushes.PapayaWhip;
                    break;
                case 107:
                    b = System.Drawing.Brushes.PeachPuff;
                    break;
                case 108:
                    b = System.Drawing.Brushes.Peru;
                    break;
                case 109:
                    b = System.Drawing.Brushes.Pink;
                    break;
                case 110:
                    b = System.Drawing.Brushes.Plum;
                    break;
                case 111:
                    b = System.Drawing.Brushes.PowderBlue;
                    break;
                case 112:
                    b = System.Drawing.Brushes.Purple;
                    break;
                case 113:
                    b = System.Drawing.Brushes.Red;
                    break;
                case 114:
                    b = System.Drawing.Brushes.RosyBrown;
                    break;
                case 115:
                    b = System.Drawing.Brushes.RoyalBlue;
                    break;
                case 116:
                    b = System.Drawing.Brushes.SaddleBrown;
                    break;
                case 117:
                    b = System.Drawing.Brushes.Salmon;
                    break;
                case 118:
                    b = System.Drawing.Brushes.SandyBrown;
                    break;
                case 119:
                    b = System.Drawing.Brushes.SeaGreen;
                    break;
                case 120:
                    b = System.Drawing.Brushes.SeaShell;
                    break;
                case 121:
                    b = System.Drawing.Brushes.Sienna;
                    break;
                case 122:
                    b = System.Drawing.Brushes.Silver;
                    break;
                case 123:
                    b = System.Drawing.Brushes.SkyBlue;
                    break;
                case 124:
                    b = System.Drawing.Brushes.SlateBlue;
                    break;
                case 125:
                    b = System.Drawing.Brushes.SlateGray;
                    break;
                case 126:
                    b = System.Drawing.Brushes.Snow;
                    break;
                case 127:
                    b = System.Drawing.Brushes.SpringGreen;
                    break;
                case 128:
                    b = System.Drawing.Brushes.SteelBlue;
                    break;
                case 129:
                    b = System.Drawing.Brushes.Tan;
                    break;
                case 130:
                    b = System.Drawing.Brushes.Teal;
                    break;
                case 131:
                    b = System.Drawing.Brushes.Thistle;
                    break;
                case 132:
                    b = System.Drawing.Brushes.Tomato;
                    break;
                case 133:
                    b = System.Drawing.Brushes.Transparent;
                    break;
                case 134:
                    b = System.Drawing.Brushes.Turquoise;
                    break;
                case 135:
                    b = System.Drawing.Brushes.Violet;
                    break;
                case 136:
                    b = System.Drawing.Brushes.Wheat;
                    break;
                case 137:
                    b = System.Drawing.Brushes.White;
                    break;
                case 138:
                    b = System.Drawing.Brushes.WhiteSmoke;
                    break;
                case 139:
                    b = System.Drawing.Brushes.Yellow;
                    break;
                default: // Schrum: Should not need anywhere this many modes
                    b = System.Drawing.Brushes.YellowGreen;
                    break;
            }

            //return new Pen(b, 2.0f); // Schrum: Used for drawing outlines of rectangles
            return b; // Schrum: Used for filling rectangles
        }

        public static SolidBrush backGroundColorBrush = new SolidBrush( Color.White ); 
        #endregion

        //TODO dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
      
        //TODO how can a new object handle collsions differently?
        #region Collision Handling
	
		

        //wall-wall (should never collide)
        public static bool collide(Wall a, Wall b)
        {
            return false;
        }

        public static bool collide(Wall wall, Robot robot)
        {
            Point2D a1 = new Point2D(wall.line.p1);
            Point2D a2 = new Point2D(wall.line.p2);
            Point2D b = new Point2D(robot.location.x, robot.location.y);
            if (!wall.visible)
                return false;
            double rad = robot.radius;
            double r = ((b.x - a1.x) * (a2.x - a1.x) + (b.y - a1.y) * (a2.y - a1.y)) / wall.line.length_sq();
            double px = a1.x + r * (a2.x - a1.x);
            double py = a1.y + r * (a2.y - a1.y);
            Point2D np = new Point2D(px, py);
            double rad_sq = rad * rad;

            if (r >= 0.0f && r <= 1.0f)
            {
                if (np.distance_sq(b) < rad_sq)
                    return true;
                else
                    return false;
            }

            double d1 = b.distance_sq(a1);
            double d2 = b.distance_sq(a2);
            if (d1 < rad_sq || d2 < rad_sq)
                return true;
            else
                return false;
        }

        public static bool collide(Robot a, Wall b)
        {
            return EngineUtilities.collide(b, a);
        }

        public static bool collide(Robot a, Robot b)
        {
            // Schrum: The "undo" that happens on collisions makes it hard for
            // all robots to be aware of collisions, and to be aware of with whom
            // they collided. Therefore, I changed this code so that the evolved
            // bot will tell an enemy when it collides with it.
            bool result = a.circle.collide(b.circle);
            if (result && b is EnemyRobot && !(a is EnemyRobot))
            {
                ((EnemyRobot)b).collisionWithEvolved = true;
            }
            return result;
        }

        #endregion

        //TODO: port to radar sensor class
        public static double scanCone(Radar rf, List<SimulatorObject> objList)
        {
            double distance = rf.max_range;
            //hitRobot = false;
            double new_distance;
			double heading=rf.owner.heading;
			Point2D point=new Point2D(rf.owner.location.x+rf.offsetx,rf.owner.location.y+rf.offsety);
			
            double startAngle = rf.startAngle + heading;
            double endAngle = rf.endAngle + heading;
            double twoPi = 2 * Math.PI;

            if (startAngle < 0)
            {
                startAngle += twoPi;
            }
            else if (startAngle > twoPi)
            {
                startAngle -= twoPi;
            }
            if (endAngle < 0)
            {
                endAngle += twoPi;
            }
            else if (endAngle > twoPi)
            {
                endAngle -= twoPi;
            }

           // if (agentsVisible)
                foreach (SimulatorObject obj in objList)
                {
                    bool found = false;

                    if (obj == rf.owner)
                        continue;

                    new_distance = point.distance(obj.location);
                     
                  //  if (new_distance - obj.radius <= rf.max_range) //TODO do we need this
                    //{
                      //TODO  before: double angle = Math.Atan2(robot2.circle.p.y - point.y, robot2.circle.p.x - point.x);
                         double angle = Math.Atan2(obj.location.y - point.y, obj.location.x - point.x);
                       
                        if (angle < 0)
                        {
                            angle += Utilities.twoPi;
                        }

                        if (endAngle < startAngle)//sensor spans the 0 line
                        {
                            if ((angle >= startAngle && angle <= Math.PI * 2) || (angle >= 0 && angle <= endAngle))
                            {
                                found = true;
                            }
                        }
                        else if ((angle >= startAngle && angle <= endAngle))
                            found = true;
                   // }


                    if (found)
                    {
                        if (new_distance < distance)
                        {
                            distance = new_distance;
                       //     hitRobot = true;
                        }
                    }
                }

            return distance;

        }


        public static double euclideanDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double euclideanDistance(Point2D p1, Point2D p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        public static double euclideanDistance(Robot r1, Robot r2)
        {
            return Math.Sqrt(Math.Pow(r1.location.x - r2.location.x, 2) + Math.Pow(r1.location.y - r2.location.y, 2));
        }

        public static double squaredDistance(Robot r1, Robot r2)
        {
            return Math.Pow(r1.location.x - r2.location.x, 2) + Math.Pow(r1.location.y - r2.location.y, 2);
        }

        public static double clamp(double val, double min, double max)
        {
            if (val > max)
                val = max;
            else if (val < min)
                val = min;
            
            return val;
        }
    }
}
