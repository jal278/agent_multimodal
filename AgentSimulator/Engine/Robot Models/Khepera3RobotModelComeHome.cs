using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class Khepera3RobotModelComeHome : Khepera3RobotModel
    {
        public Khepera3RobotModelComeHome()
            : base()
        {
            name = "Khepera3RobotModelComeHome";
        }
    
        public override void populateSensors()
        {
            base.populateSensors();
            // Schrum: Commented this condition out, since choosing this particular robot model means
            // that the user wants to have the come-home signal. Hopefully this doesn't break anything else.
            //if (!agentBrain.multipleBrains)
            sensors.Add(new SignalSensor(this));
        }
    }
}
