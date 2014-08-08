using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class Khepera3RobotModel : Robot
    {
        protected bool collisionAvoidance = true;       //TODO this works right now only if the number of sensors is at the default value (6)
		public float totDelta=0.0f;
		public float totSight=0.0f;
		protected float[] output_copy;
        private Queue<double>[] sensor_queues;
        protected double actualRange = 10.16; //10.16; //was 15 , 15.24 would be more accurate
        private int windowsize = 5;
        private float floor = 0.25f;
        protected float threshold = 0.25f;
		private bool running_avg=false;
		public bool confused=false;	
		public float leftBias=0.0f;
		public float rightBias=0.0f;
		public float forwardBias=0.0f;
		public float speedBias=0.0f;

        public float default_speed = 9.0f;
        public float default_turn_speed = 6.28f;

		public Khepera3RobotModel()
            :base()
        {
            
			output_copy=new float[3];
            name = "Khepera3RobotModel";
            autopilot = false;
            sensor_queues = new Queue<double>[7];
            for (int x = 0; x < sensor_queues.Length; x++)
                sensor_queues[x] = new Queue<double>();
        }

        public override void doAction()
        {
            //TODO: probably need another function for heading noise?
            double distortion;


            float[] inputs = new float[sensors.Count];
            for (int j = 0; j < sensors.Count; j++)
            {
				if(sensors[j] is SignalSensor) {
					inputs[j] = (float)sensors[j].get_value();
				} else {
                	inputs[j] = transformSensor(sensors[j].get_value_raw());
					
					if(sensorNoise>0.0) {			
						inputs[j]+= sensorNoise/100.0f * ((float)rng.NextDouble()-0.5f)*2.0f;				
						//if(inputs[j]<0.0f)
						// inputs[j]=0.0f;
					}
				
				if (inputs[j] > 1.0f) inputs[j] = 1.0f;
				if (inputs[j] <= floor) inputs[j] = 0.0f;
				}
				
				if(running_avg) {
				sensor_queues[j].Enqueue(inputs[j]);
                if (sensor_queues[j].Count > windowsize)
                    sensor_queues[j].Dequeue();
                double v = 0.0;
                foreach (double d in sensor_queues[j])
                {
                    v += d;
                }
                inputs[j] = (float)(v / sensor_queues[j].Count);
				}
            }
			
            agentBrain.setInputSignals(id, inputs);
        }

        protected virtual void doAutopilot(float effectorNoise)
        {
            //TODO add effectorNoise?
            velocity = 9.0f;
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
                RangeFinder r = new RangeFinder(val, this, actualRange + radius, this.sensorNoise);
                sensors.Add(r);
            }
        }

        public override void networkResults(float[] outputs)
        {
            decideAction(outputs, timeStep);
            updatePosition();
        }

        protected virtual void decideAction(float[] outputs, float timeStep)
        {
            if (autopilot)
            {
                doAutopilot(effectorNoise);
                return;
            }

            bool debug = false;
			output_copy = new float[outputs.Length];
			List<int> maxx= new List<int>();
			float maxy= -0.1f;
			
			for(int x=0;x<outputs.Length;x++) {
				output_copy[x]=outputs[x];
				if(outputs[x]>maxy) {
					maxx.Clear();
					maxx.Add(x);
					maxy=outputs[x];
				}
				else if (outputs[x]==maxy) {
					maxx.Add(x);
				}
			}
			
			float min_difference=1.0f;
			
			for(int x=0;x<outputs.Length;x++) 
			{
				if(x==maxx[0]) continue;
				float dif=Math.Abs(outputs[x]-maxy);
				if(dif <min_difference)
					min_difference=dif;
			}
			if(min_difference < 0.001) {
				confused=true;
				totDelta+=1.0f;
			}
			else 
				confused=false;
			
			foreach(ISensor k in sensors) {
				if(k is RangeFinder) {
					if (((RangeFinder)k).seeRobot)
						totSight+=1.0f;
				}
			}
			
			//totDelta+=Math.Min(0.1f,min_difference);
			

			//if(maxx.Count>1)
			//	this.stopped=true;
			/*
			foreach(int k in maxx) {
				output_copy[k]=1.0f;
			}
			*/
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
            float speed = default_speed * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));
            float turnSpeed = default_turn_speed / 3.0f * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));
            
			//speed = 6.45f + speedBias;
			//turnSpeed = 6.28f/4.8f;
			
			//speed*=0.66f;
			speed+=speedBias;
			//turnSpeed*=0.66f;
			
			//bool collavoid = true;
            //if((outputs[1] >= outputs[0] && outputs[1] >= outputs[2]))
            //ignore delta for forward, forward is default
			
			
            if(outputs.Length>3 && outputs[3] > 0.5)
				stopped=true;
			if (outputs.Length < 4 || outputs[3] < 0.5)
            {
                if ((outputs[1] - outputs[0] >= delta) && (outputs[1] - outputs[2] >= delta)) //forward
                {
                    //if too close to wall, try and avoid if

                    if (collisionAvoidance)
                    {
                        if (transformSensor(sensors[0].get_value_raw()) <= threshold || transformSensor(sensors[1].get_value_raw()) <= threshold)
                            heading += (timeStep * (turnSpeed+rightBias));
                        else if (transformSensor(sensors[2].get_value_raw()) <= threshold || transformSensor(sensors[3].get_value_raw()) <= threshold)
                        {
                            corrected = true;
                            velocity = 0;
                        }
                        else if (transformSensor(sensors[4].get_value_raw()) <= threshold || transformSensor(sensors[5].get_value_raw()) <= threshold)
                            heading -= (timeStep * (turnSpeed+leftBias));
                        else
						{
                            velocity = speed;
							heading+=forwardBias;
						}
                    }
                    else
                    {
                        velocity = speed;
						heading+=forwardBias;
                    }

                    if (debug)
                        Console.WriteLine(this.id + " FORWARD");

                }
                else
                {
                    velocity = 0;
                    if ((outputs[0] - outputs[1] >= delta) && (outputs[0]- outputs[2] >= delta)) //turn left
                    {
                        heading -= (timeStep * (turnSpeed+leftBias));
                        if (debug)
                            Console.WriteLine(this.id + " LEFT");

                    }
                    else if ((outputs[2] - outputs[0] >= delta) && (outputs[2] - outputs[1] >= delta)) //right
                    {
                        heading += (timeStep * (turnSpeed+rightBias));
                        if (debug)
                            Console.WriteLine(this.id + " RIGHT");
                    }
                    else
                    {
                        //How does it get here?
                        velocity = 0;
                    }
                }
            }

            //stop
            else
            {
                velocity = 0;
            }
            //stop

        }


		public override void draw(Graphics g, CoordinateFrame frame)
        {
            Point upperleft = frame.to_display((float)(circle.p.x - radius),(float)(circle.p.y- radius));
			int size = (int)((radius*2)/frame.scale);
			Rectangle r = new Rectangle(upperleft.X,upperleft.Y,size,size);
            // Schrum: Colored rectangle for mode tracking 
            Rectangle mode = new Rectangle(upperleft.X + (size / 4), upperleft.Y + (size / 4), size / 2, size / 2);
            
            if (disabled)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else if (collide_last || confused)
                g.DrawEllipse(EngineUtilities.RedPen, r);
            else if (corrected)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else if (autopilot)
                g.DrawEllipse(EngineUtilities.GreendPen, r);
            else
                g.DrawEllipse(EngineUtilities.BluePen, r);
			int sensCount=0;

			// Schrum: Draw the mode rectangle
            g.FillRectangle(EngineUtilities.modePen(currentBrainMode), mode);

			if(display_debug)
			foreach(float f in output_copy) {
				Color col = Color.FromArgb(0,0,(int)(f*255)); //(int)(val*255),(int)(val*255));
				SolidBrush newpen = new SolidBrush(col);
				g.FillRectangle(newpen,sensCount*40+400,500+30*id,40,30);
				sensCount+=1;
			}
			sensCount=0;
			
			foreach(ISensor sensor in sensors)
			{
				sensor.draw(g,frame);
				if(draw_sensors) {
				double val = sensor.get_value();
				if(val<0.0) val=0.0;
				if(val>1.0) val=1.0;
				if(display_debug) {
				Color col = Color.FromArgb((int)(val*255),0,0); //(int)(val*255),(int)(val*255));
				SolidBrush newpen = new SolidBrush(col);
				g.FillRectangle(newpen,sensCount*40,500+30*id,40,30);
				}
				sensCount+=1;
				}
			}
		}
        protected float transformSensor(double val)
        {
            return (float)(val - defaultRobotSize()) / (float)actualRange;//(float)sensors[j].get_value();
        }
    }
}
