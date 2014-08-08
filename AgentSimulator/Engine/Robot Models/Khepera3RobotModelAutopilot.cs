using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class Khepera3RobotModelAutopilot : Khepera3RobotModel
    {
        public Khepera3RobotModelAutopilot() : base()
        {
            autopilot = true;
            name = "Khepera3RobotModelAutopilot";
        }
		
		public override void populateSensors()
        {
            base.populateSensors();
            //if (!agentBrain.multipleBrains)
            //    sensors.Add(new SignalSensor(this));
        }
		
        protected override void doAutopilot(float effectorNoise)
        {
            velocity = 9.0f;
            if (transformSensor(sensors[0].get_value_raw()) > .9 && transformSensor(sensors[5].get_value_raw()) > .9)
                autopilot = false;
        }
    }
}
