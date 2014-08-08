
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace Engine
{

    //grid square for detecting collisions and calculating field of view
    public class collision_grid_square : IComparable
    {
		public double avg_idle;
		public double idleness;
        public double last_viewed; //last viewed
        public double viewed; //has this square been viewed in the last timestep?
		public double viewed2;
		public List<SimulatorObject> dynamic_objs; //list of dynamic objects in this square (e.g. robots) 
        public List<SimulatorObject> static_objs; //list of static objects in this sqaure (e.g. walls)
        public double ix; //x and y coordinates used when doing ray-casting
        public double iy;
        public double cx;
        public double cy;

        public void draw(Graphics g, CoordinateFrame frame)
        {
            if (viewed > 0.0)
            {
                float ox, oy;
                frame.to_display((float)cx, (float)cy, out ox, out oy);

                g.FillEllipse(new System.Drawing.SolidBrush(Color.FromArgb(255,255-(int)(viewed*255),255-(int)(viewed*255))), ox, oy, 25, 25);
                //g.FillEllipse(System.Drawing.Brushes.IndianRed, ox, oy, 25, 25);
                //Console.WriteLine(cx+" " + cy + " " + ox + " " +oy);

            }
        }

        //sort grid squares by x coordinate
        int IComparable.CompareTo(object obj)
        {
            collision_grid_square sq = (collision_grid_square)obj;
            if (this.ix > sq.ix)
                return 1;
            if (this.ix < sq.ix)
                return -1;
            return 0;
        }

        public collision_grid_square(double _ix, double _iy, double _cx, double _cy)
        {
            ix = _ix;
            iy = _iy;
            cx = _cx;
            cy = _cy;
            dynamic_objs = new List<SimulatorObject>();
            static_objs = new List<SimulatorObject>();
            viewed = 0.0;
			viewed2=0.0;
            last_viewed = 0.0;
        }

        public void reset_dynamic()
        {
            dynamic_objs.Clear();
        }

        public void reset_viewed()
        {
            last_viewed = viewed;
            viewed = 0.0;
			viewed2=0.0;
        }

        public void decay_viewed(double factor)
        {
            last_viewed = viewed;
            viewed *= factor;
			viewed2 *= factor;
        }
    }

    /// <summary>
    /// class that implements a grid overlaid the map used
    /// for speeding up collision detection (by only doing
    /// collision testing between objects in the same grid
    /// squares) and checking agent's field of view. 
    /// </summary>
    public class collision_grid
    {
        public int coarseness; //coarseness of the grid, if this is K, then the grid is KxK
        public int mapx; //size of map in x dimension
        public int mapy; //size of map in y dimension
        public double gridx; //size of grid square in x dimension
        public double gridy; //size of grid square in y dimension

        public collision_grid_square[,] grid;	  //actual grid data structure

        public void draw(Graphics g, CoordinateFrame frame)
        {

            foreach (collision_grid_square square in grid)
                square.draw(g, frame);
        }

        //what grid squares does a given circle intersect?
        //use to see what grid squares a robot needs to be checked against
        public List<collision_grid_square> circle(double x, double y, double radius)
        {
            List<collision_grid_square> intersect = new List<collision_grid_square>();
            double ulx = x - radius;  //calculate upper-left extent of circle
            double uly = y - radius;
            double lrx = x + radius; //calculate lower-right extent of circle
            double lry = y + radius;

            int ulx_i = (int)ulx / (int)gridx; //translate these coordinates into grid coordinates
            int uly_i = (int)uly / (int)gridy;
            int lrx_i = (int)lrx / (int)gridx;
            int lry_i = (int)lry / (int)gridy;

            //Check boundaries
            if (ulx_i < 0)
                ulx_i = 0;
            else if (ulx_i >= coarseness)
                ulx_i = coarseness - 1;

            if (uly_i < 0)
                uly_i = 0;
            else if (uly_i >= coarseness)
                uly_i = coarseness - 1;

            if (lry_i < 0)
                lry_i = 0;
            else if (lry_i >= coarseness)
                lry_i = coarseness - 1;

            if (lrx_i < 0)
                lrx_i = 0;
            else if (lrx_i >= coarseness)
                lrx_i = coarseness - 1;

            //loop through and add all grid squares that
            //the circle might touch
            for (int xi = ulx_i; xi <= lrx_i; xi++)
                for (int yi = uly_i; yi <= lry_i; yi++)
                    intersect.Add(grid[xi, yi]);

            //return those intersections
            return intersect;

        }

        //ray-casting for detecting collisions with walls using the grid stucture
        public List<collision_grid_square> cast_ray(double sx, double sy, double ex, double ey)
        {
            List<collision_grid_square> intersect = new List<collision_grid_square>();

            double bsx = sx;
            double bsy = sy;
            double bex = ex;
            double bey = ey;

            double maxx = gridx * (coarseness) - 0.001;
            double maxy = gridy * (coarseness) - 0.001;

            double tslope = (ey - sy) / (ex - sx);

            //clip ray within grid...
            if (ex < 0.0)
            {
                ey = sy + tslope * (0 - sx);
                ex = 0.0;
            }
            if (ex > maxx)
            {
                ey = sy + tslope * (maxx - sx);
                ex = maxx;
            }
            if (ey < 0.0)
            {
                ex = sx + (1.0 / tslope) * (0 - sy);
                ey = 0.0;
            }
            if (ey > maxy)
            {
                ex = sx + (1.0 / tslope) * (maxy - sy);
                ey = maxy;
            }

            //calculate special vertical and horizontal cases...
            double x0 = sx / gridx;
            double x1 = ex / gridx;
            double y0 = sy / gridy;
            double y1 = ey / gridy;

            //horizontal line, easy to calculate
            if (y0 == y1)
            {
                bool reverse = false;

                int sxi = (int)x0;
                int exi = (int)x1;
                int yi = (int)y0;

                if (exi < sxi)
                {
                    int temp = exi;
                    exi = sxi;
                    sxi = temp;
                    reverse = true;
                }

                for (int xi = sxi; xi <= exi; xi++)
                    intersect.Add(grid[xi, yi]);

                if (reverse)
                    intersect.Reverse();

                return intersect;
            }

            //vertical line, easy to calculate
            if (x0 == x1)
            {
                bool reverse = false;
                int syi = (int)y0;
                int eyi = (int)y1;
                int xi = (int)x0;

                if (eyi < syi)
                {
                    int temp = eyi;
                    eyi = syi;
                    syi = temp;
                    reverse = true;
                }

                for (int yi = syi; yi <= eyi; yi++)
                    intersect.Add(grid[xi, yi]);

                if (reverse)
                    intersect.Reverse();

                return intersect;
            }
            //we've handled the special vertical and horizontal cases...

            bool rev = false;
            //make sure we are going left-to-right
            //if not, swap the points
            if (sx > ex)
            {
                double temp = sx;
                sx = ex;
                ex = temp;
                rev = true;

                temp = ey;
                ey = sy;
                sy = temp;
            }

            //does our line slope up or down?
            bool up = true;
            if (sy > ey)
                up = false;

            collision_grid_square to_add = null;
          //  try
          //  {
                //add the current grid square
                to_add = grid[(int)(sx / gridx), (int)(sy / gridy)];
           // }
            //catch (Exception e)
            //{
            //    Console.WriteLine(bsx.ToString() + " " + bsy.ToString() + " " + bex.ToString() + " " + bey.ToString());
            //    Console.WriteLine(sx.ToString() + " " + sy.ToString() + " " + ex.ToString() + " " + ey.ToString());
            //    Console.WriteLine(e.Source);
            //    Console.WriteLine(e.Message);
            //    Console.WriteLine(e.StackTrace);

            //    Console.WriteLine("own sqaure...");
            //    throw (e);
            //}
            to_add.ix = sx;
            intersect.Add(to_add);

            double slope = Math.Abs((ey - sy) / (ex - sx));
            double invslope = 1.0 / slope;

            //vertical intersections
            //we must find the first intersection now

            double py = Math.Floor(sy / gridy) * gridy;
            double incy = gridy;
            if (up)
                py += gridy + 0.001;
            else
            {
                py -= 0.001;
                incy = -incy;
            }
            double px = sx + Math.Abs((py - sy)) / slope;
            double incx = invslope * gridy;
            while (true)
            {
                if (px > ex)
                    break;

                if (up)
                {
                    if (py > ey)
                        break;
                }
                else
                {
                    if (py < ey)
                        break;
                }

                try
                {
                    to_add = grid[(int)(px / gridx), (int)(py / gridy)];
                }
                catch (Exception e)
                {
                    Console.WriteLine(bsx.ToString() + " " + bsy.ToString() + " " + bex.ToString() + " " + bey.ToString());
                    Console.WriteLine(sx.ToString() + " " + sy.ToString() + " " + ex.ToString() + " " + ey.ToString());

                    Console.WriteLine("Vertical..." + ((int)(px / gridx)).ToString() + " " + ((int)(py / gridy)).ToString());
                    Console.WriteLine(px.ToString() + " " + py.ToString() + " " + ex.ToString() + " " + ey.ToString());
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    //throw (e);
                }
                to_add.ix = px;
                intersect.Add(to_add);

                px += incx;
                py += incy;
            }

            //horizontal intersections...
            slope = (ey - sy) / (ex - sx);
            invslope = 1.0 / slope;

            px = Math.Floor(sx / gridx) * gridx + gridx + 0.01;
            py = sy + (px - sx) * slope;

            incx = gridx;
            incy = gridx * slope;

            while (true)
            {
                if (px > ex)
                    break;

                if (up)
                {
                    if (py > ey)
                        break;
                }
                else
                {
                    if (py < ey)
                        break;
                }
                try
                {
                    to_add = grid[(int)(px / gridx), (int)(py / gridy)];
                }
                catch (Exception e)
                {
                    Console.WriteLine(bsx.ToString() + " " + bsy.ToString() + " " + bex.ToString() + " " + bey.ToString());
                    Console.WriteLine(sx.ToString() + " " + sy.ToString() + " " + ex.ToString() + " " + ey.ToString());

                    Console.WriteLine("Horizontal..." + ((int)(px / gridx)).ToString() + " " + ((int)(py / gridy)).ToString());
                    Console.WriteLine(px.ToString() + " " + py.ToString() + " " + ex.ToString() + " " + ey.ToString());
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    throw (e);
                }

                to_add.ix = px;
                if (!intersect.Contains(to_add))
                    intersect.Add(to_add);

                px += incx;
                py += incy;
            }
            //now we sort the grid squares so they are in
            //increasing x order
            intersect.Sort();

            //but if our line actually goes right to left, 
            //we need to reverse the order
            if (rev)
                intersect.Reverse();

            return intersect;
        }

        public collision_grid(Environment e, int _coarseness)
        {
            double mapx = 0.0, mapy = 0.0;

            foreach (Wall w in e.walls)
            {
                if (w.line.p1.x > mapx)
                    mapx = w.line.p1.x;
                if (w.line.p2.x > mapx)
                    mapx = w.line.p2.x;
                if (w.line.p1.y > mapy)
                    mapy = w.line.p1.y;
                if (w.line.p2.y > mapy)
                    mapy = w.line.p2.y;
            }
            //add some slack..
            mapx += 50;
            mapy += 50;

            coarseness = _coarseness;

            //mapx = 618 * 4;
            //mapy = 885 * 4;
            //coarseness = 20*4;

            //Console.WriteLine(mapx.ToString()+ " " + mapy.ToString() + " " + coarseness.ToString());

            gridx = (double)mapx / (double)coarseness;
            gridy = (double)mapy / (double)coarseness;

            grid = new collision_grid_square[coarseness, coarseness];
            for (int x = 0; x < coarseness; x++)
                for (int y = 0; y < coarseness; y++)
                    grid[x, y] = new collision_grid_square(x, y, x * gridx, y * gridy);

            //Console.WriteLine("Grid square x_size = " + gridx.ToString());
            //Console.WriteLine("Grid square y_size = " + gridy.ToString());
        }

        public collision_grid_square get_grid_square(double x, double y)
        {
            return grid[(int)((double)x / gridx), (int)((double)y / gridy)];
        }

        public void reset_dynamic()
        {
            foreach (collision_grid_square square in grid)
            {
                square.reset_dynamic();
            }
        }
        public void insert_into_grid(SimulatorObject obj)
        {

            bool dynamic = obj.dynamic;
            if (obj is Wall)
            {
                Wall w = (Wall)obj;
                foreach (collision_grid_square square in cast_ray(w.line.p1.x, w.line.p1.y, w.line.p2.x, w.line.p2.y))
                {
                    if (dynamic)
                        square.dynamic_objs.Add(obj);
                    else
                        square.static_objs.Add(obj);
                }
            }
            else if (obj is Robot)
            {
                Robot r = (Robot)obj;
                foreach (collision_grid_square square in circle(r.location.x, r.location.y, r.radius))
                {
                    if (dynamic)
                        square.dynamic_objs.Add(obj);
                    else
                        square.static_objs.Add(obj);
                }
            }
            else
            {
                Console.WriteLine("Object not supported by collision grid.");
            }
        }

        public void reset_viewed()
        {
            foreach (collision_grid_square square in grid)
            {
                square.reset_viewed();
            }
        }

        public void decay_viewed(double factor)
        {
            foreach (collision_grid_square square in grid)
            {
                square.decay_viewed(factor);
            }
        }
    }
}