using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class Khepera3RobotModel_Remote : Robot
    {
		public KheperaSerialPortCommunicator communicator;
		private float sensor_adjustment=0.0f;
		private float scale=0.35f;
		private float turn_scale=0.35f;
		public Khepera3RobotModel_Remote()
        {
            name = "Khepera3RobotModel_Remote";
            autopilot = false;
			Console.WriteLine("before khepera initialized");
			communicator = new KheperaSerialPortCommunicator("/dev/rfcomm0");
			Console.WriteLine("after khepera initialized");
		}


        public float[] getRemoteSensorValue()
        {
            return communicator.GetSensors();
        }

        private void remoteTurn(double p)
        {
            if(p>0.0) {
			 communicator.SendMotor(1.0f*turn_scale,-1.0f*turn_scale);
			}
			if(p<0.0) {
			 communicator.SendMotor(-1.0f*turn_scale,1.0f*turn_scale);
			}
			
        }

        private void setRemoteVelocity(int p)
        {
			System.Diagnostics.Stopwatch s= new System.Diagnostics.Stopwatch();
			s.Start();
			if(p>0) {
				communicator.SendMotor(1.0f*scale,1.0f*scale);
			}
			else {
				communicator.SendMotor(0.0f,0.0f);
			}
			s.Stop();
			Console.WriteLine("elapsed: " + s.ElapsedMilliseconds);
		}

        public override void doAction()
        {
            //TODO: probably need another function for heading noise?
            double distortion;

            if (autopilot)
            {
                //doAutopilot(effectorNoise);
                return;
            }

            float[] inputs = getRemoteSensorValue();
			Console.WriteLine("sensor count:" + sensors.Count);
            for (int j = 0; j < sensors.Count; j++)
            {
				inputs[j]-=sensor_adjustment;
				if (inputs[j] < 0.0) inputs[j] = 0.0f;
                if (inputs[j] > 1.0) inputs[j] = 1.0f;
                Console.Write(" " + inputs[j]);

            }
            Console.WriteLine("indone");

            agentBrain.setInputSignals(id, inputs);

        }

        public override float defaultRobotSize()
        {
            return 6.5f;
        }

        public override int defaultSensorDensity()
        {
            return 6;
            //There are 9 sensors but 3 are on the back, not sure if we'll use those or not
            //return 9;
        }

        public override void populateSensors()
        {
            populateSensors(defaultSensorDensity());
        }

        public override void populateSensors(int size)
        {
            sensors = new List<ISensor>();

            List<double> sensorAngles = new List<double>();
            double sensorStart = -1.3398;
            double sensorEnd = 1.3398;
            double delta = (sensorEnd - sensorStart) / (size - 1);
            double x = sensorStart;

            for (int y = 0; y < size; y++)
            {
                sensorAngles.Add(x);//sensor_angles[y]);
                x += delta;
            }

            foreach (double val in sensorAngles)
            {
                RangeFinder r = new RangeFinder(val, this, 40.0, this.sensorNoise);
                sensors.Add(r);
            }
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

        private void decideAction(float[] outputs, float timeStep)
        {
            float delta = 0.00f;
            if ((outputs[1] - outputs[0] >= delta) && (outputs[1] - outputs[2] >= delta)) //forward
            {
				Console.WriteLine("forward");
                velocity = 30;
                setRemoteVelocity(30);
            }
            else
            {
                velocity = 0;
                if ((outputs[0] - outputs[1] > delta) && (outputs[0] - outputs[2] > delta)) //turn left
                {
									Console.WriteLine("left");

					heading -= (timeStep * 6.28);
                    remoteTurn(-1.0);
                }
                else if ((outputs[2] - outputs[0] > delta) && (outputs[2] - outputs[1] > delta)) //right
                {
									Console.WriteLine("right");

                    heading += (timeStep * 6.28);
                    remoteTurn(1.0);
                }


            }
        }

    }
}
