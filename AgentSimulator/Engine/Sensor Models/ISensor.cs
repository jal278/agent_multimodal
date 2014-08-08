using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Engine
{
    public interface ISensor
    {
		double get_value();
		double get_value_raw();
		void update(Environment env,List<Robot> robots,CollisionManager cm);
		void draw(Graphics g, CoordinateFrame frame);
    }
}
