using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class Khepera3RobotModelContinuous : Khepera3RobotModel
    {

        protected bool addtimer;

        public Khepera3RobotModelContinuous()
            : base()
        {
			autopilot=true;
            addtimer = true;
            name = "Khepera3RobotModelContinuous";
        }


		public override void populateSensors()
        {
            base.populateSensors();
            if (!agentBrain.multipleBrains && addtimer)
                sensors.Add(new SignalSensor(this));
        }
		
		protected override void doAutopilot(float effectorNoise)
        {
            velocity = 9.0f;
            if (transformSensor(sensors[0].get_value_raw()) > .9 && transformSensor(sensors[5].get_value_raw()) > .9)
                autopilot = false;
        }
 
        protected override void decideAction(float[] outputs, float timeStep)
        {
            if (autopilot)
            {
                doAutopilot(effectorNoise);
                return;
            }

            bool debug = false;
            output_copy = new float[outputs.Length];
            List<int> maxx = new List<int>();
            float maxy = 0.0f;
            for (int x = 0; x < outputs.Length; x++)
            {
                if (outputs[x] > maxy)
                {
                    maxx.Clear();
                    maxx.Add(x);
                    maxy = outputs[x];
                }
                else if (outputs[x] == maxy)
                {
                    maxx.Add(x);
                }
            }

            //if(maxx.Count>1)
            //	this.stopped=true;

            foreach (int k in maxx)
            {
                output_copy[k] = 1.0f;
            }
            /*
                outputs[1]=1.0f;
                outputs[0]=0.0f;
                outputs[2]=0.0f;
            */
            /*
                if (debug) {
                Console.Write(id+" INPUTS: ");
                foreach(ISensor sensor in sensors) {
                    Console.Write((sensor.get_value_raw() - defaultRobotSize()) / (float)actualRange + " ");				}			
                Console.WriteLine();

                Console.Write(id+" OUTPUTS: ");
                foreach(float f in outputs) {
                    Console.Write(f+" ");		
                }
                Console.WriteLine();
                }
                */


            corrected = false;

            float delta = 0.00f;
            float speed = 9.0f * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));
            float turnSpeed = 6.28f / 3.0f * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));

            //speed = 6.45f + speedBias;
            //turnSpeed = 6.28f/4.8f;

            //speed*=0.66f;
            speed += speedBias;
            //turnSpeed*=0.66f;

            bool collavoid = true;
            //if((outputs[1] >= outputs[0] && outputs[1] >= outputs[2]))
            //ignore delta for forward, forward is default

			if(outputs.Length>3 && outputs[3] > 0.5)
				stopped=true;
            velocity = speed * outputs[1];
            heading += (outputs[2] - outputs[0]) * turnSpeed * timeStep;
        }
    }
}
