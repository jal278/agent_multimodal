using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    //base clase for simulated objects
    public abstract class SimulatorObject
    {
		public string name;
        public Point2D location; //location
        public bool dynamic; //will this object ever move?
        public bool visible; //is this object active
		public bool selected; //is this object selected (used for visualization)
        public abstract void update();
        public abstract void undo();
    }
}
