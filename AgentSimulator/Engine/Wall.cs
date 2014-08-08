using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    //simple wall class
    public class Wall : SimulatorObject
    {
        //a wall is basically just a line
        
        public Line2D line;
        public bool colored;

        public override void update() { }
        public override void undo() { }
           

        public Wall()
        {
        }

        public Wall(Wall k)
        {
            name = k.name;
            //Console.WriteLine(name);
            colored = false;

            line = new Line2D(k.line);

            //set center point
            location = line.midpoint();

            dynamic = false;
            visible = k.visible;
        }

        public Wall(double nx1, double ny1, double nx2, double ny2, bool vis, string n)
        {
            name = n;
            colored = false;
            Point2D p1 = new Point2D(nx1, ny1);
            Point2D p2 = new Point2D(nx2, ny2);
            line = new Line2D(p1, p2);

            //set center point
            location = line.midpoint();

            dynamic = false;
            visible = vis;
        }

        public void draw(Graphics g, CoordinateFrame frame)
        {
			float ax,ay,bx,by;
			frame.to_display((float)line.p1.x,(float)line.p1.y,out ax, out ay);
			frame.to_display((float)line.p2.x,(float)line.p2.y,out bx, out by);
			

                if (visible)
                    g.DrawLine(EngineUtilities.BluePen, ax,ay, bx,by);
                else
                    g.DrawLine(EngineUtilities.GreendPen, ax,ay,bx, by);
        }
    }
}
