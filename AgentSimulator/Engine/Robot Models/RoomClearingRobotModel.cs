using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class RoomClearingRobotModel : Robot
    {
  		
        public RoomClearingRobotModel()
        {
            name = "RoomClearingRobotModel";
            autopilot = true;
        }

        public override int defaultSensorDensity()
        {
            return 11;
        }

        public override float defaultRobotSize()
        {
            return 10f; //was 5
        }
            
        public override void onCollision() {
			base.onCollision();
		}

        public override void populateSensors(int size)
        {
            sensors = new List<ISensor>();
			double max_range=100;//0; //was 100
            //Special case for only one sensor
            if (size == 1)
            {
                sensors.Add(new RangeFinder(0, this, max_range,this.sensorNoise));
                return;
            }

            double startAngle = -Math.PI / 2.0;
            //starts at -PI/2 ends at PI/2 so spans PI
            double delta = Math.PI / (size - 1);

            for (int j = 0; j < size; j++)
            {
                sensors.Add(new RangeFinder(startAngle + (delta * j),this,max_range,this.sensorNoise));
            }
        }

        public void decideAction(float[] outputs, double timeStep)
        {
            if (autopilot)
            {
                doAutopilot(timeStep);
                return;
            }
            //stopped = true;
           /* if (outputs[1] < .01)
            {
                velocity = 0;
                stopped = true;
            }*/
            if (stopped) {
			velocity=0.0;
                return;
			}    

            velocity = outputs[1] * 40;  //was 40
            heading += (outputs[2] - outputs[0]) * 0.05; // was 0.05
			collide_last=false;
        }

        private void doAutopilot(double timestep)
        {
            velocity = 40;
        }

        public override void networkResults(float[] outputs)
        {

            double distortion;
            if (effectorNoise > 0)
            {
                for (int k = 0; k < outputs.Length; k++)
                {

                    distortion = 1.0 + (EngineUtilities.random.NextDouble() - 0.5) * 2.0 * (effectorNoise) / 100.0;
                    outputs[k] *= (float)distortion;
                    outputs[k] = (float)EngineUtilities.clamp(outputs[k], 0, 1);
                }
            }
            decideAction(outputs, timeStep);
            updatePosition();
        }

        public override void doAction()
        {
            double distortion;

                float[] inputs = new float[sensors.Count];
			
				for (int j = 0; j < sensors.Count; j++)
                {
                    inputs[j] = (float)sensors[j].get_value();
                }

                //Console.WriteLine("indone");

            agentBrain.setInputSignals(id, inputs);
          

        }
    }
}
