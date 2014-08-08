using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Engine
{
    class SimpleScript : IScript
    {
        #region IFitnessFunction Members
		int tsteps;
		bool all_in;
		public SimpleScript()
		{
			all_in=false;
			tsteps=0;
		}
        public string name
        {
            get { return this.GetType().Name; }
        }

       public string description
        {
            get { return "Calculates fitness based on the distance to the closest agent for each agent.  Agents outside the ROI are ignored."; }
        }

        public void onTimeStep(SimulatorExperiment experiment)
        {
            //Disabled for now
            //TODO include again

            //tsteps++;
            //if(!all_in && eval.autopilot_count==0)
            //{
            //    eval.sim_engine.find_wall_by_name("door").visible=true;
            //    all_in=true;
            //}
        }
		
		public void reset()
		{
			all_in=false;
			tsteps=0;
		}
        #endregion
    }
}
