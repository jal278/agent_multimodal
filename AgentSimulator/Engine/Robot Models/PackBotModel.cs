using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib;

namespace Engine
{
    public class PackBotModel : Robot
    {

        public PackBotModel()
        {
            name = "PackBotModel";
        }

        //public PackBotModel(int id, double nx, double ny, double dir, 
        //    AgentBrain agentBrain, Environment environment,
        //    float sensorNoise, float effectorNoise, float headingNoise, float timeStep)
        //{
        //    steps = 0;

        //    this.id = id;
        //    updatecounter = 0;
      
        //    path = new List<Point2D>();
        //    location = new Point2D(nx, ny);
        //    circle = new Circle2D(location, radius);
        //    old_location = new Point2D(location);
        //    radius = radius;
        //    heading = dir;
        //    velocity = 0.0;
        //    collide_last = false;
        //    this.timeStep = timeStep;
        //    this.environment = environment;
        //    this.agentBrain = agentBrain;
        //    this.sensorNoise = sensorNoise;
        //    this.effectorNoise = effectorNoise;
        //    this.headingNoise = headingNoise;
        //    populateSensors();
        //}

        //public override void populateSensors()
        //{
        //    populateSensors(defaultSensorDensity());
        //}

        public override void populateSensors(int size)
        {
            sensors = new List<ISensor>();

            List<double> sensorAngles = new List<double>();
            double sensorStart = -1.57;
            double sensorEnd = 1.57;
            double delta = (sensorEnd - sensorStart) / (size - 1);
            double x = sensorStart;

            for (int y = 0; y < size; y++)
            {
                sensorAngles.Add(x);//sensor_angles[y]);
                x += delta;
            }

            foreach (double val in sensorAngles)
            {
                RangeFinder r = new RangeFinder(val, this, 400.0,this.sensorNoise);
                r.set_delta(30, 0.5);
                sensors.Add(r);
            }
        }

        public override void robotSettingsChanged()
        {
        }
		
		public override void doAction()
        {
            //TODO: probably need another function for heading noise?
            double distortion;

            if (autopilot)
            {
                doAutopilot(effectorNoise);
                return;
            }

            float[] inputs = new float[sensors.Count];
            for (int j = 0; j < sensors.Count; j++)
            {
			    inputs[j] = (float)sensors[j].get_value_raw()/350.0f;
				if (inputs[j]>1.0) inputs[j]=1.0f;
				//Console.Write(" " + inputs[j]);
            
            }
			//Console.WriteLine("indone");

			agentBrain.setInputSignals(id, inputs);

        }

        protected void decideAction(float[] outputs, double timeStep)
        {
            float delta = 0.00f;
            if ((outputs[1] - outputs[0] >= delta) && (outputs[1] - outputs[2] >= delta)) //forward
            {
                velocity = 50;
				Console.WriteLine("forward!");
            }
            else
            {
                velocity = 0;
                if ((outputs[0] - outputs[1] > delta) && (outputs[0] - outputs[2] > delta)) //turn left
                {
                    heading -= (timeStep * 1.28);
                }
                else if ((outputs[2] - outputs[0] > delta) && (outputs[2] - outputs[1] > delta)) //right
                {
                    heading += (timeStep * 1.28);
                }
            }
        }

        protected void doAutopilot(float noise)
        {
            velocity = 40.0;
        }

        public override int defaultSensorDensity()
        {
            return 5;
        }

        public override float defaultRobotSize()
        {
            return 41.5f;
        }

        public override void draw(Graphics g,CoordinateFrame frame)
        {
            Rectangle r = new Rectangle();
			float cx,cy;
			frame.to_display((float)circle.p.x,(float)circle.p.y,out cx,out cy);
			r.X = (int)(cx-radius/frame.scale);
			r.Y = (int)(cy-radius/frame.scale);
			r.Height = (int)((radius * 2) / frame.scale);
            r.Width = (int)((radius * 2) / frame.scale);
            if (collide_last)
                g.DrawEllipse(Pens.Red, r);
            else
                g.DrawEllipse(Pens.Blue, r);



            float x1 = (float)(Math.Cos(heading + 0.51f) * radius);
            float y1 = (float)(Math.Sin(heading + 0.51f) * radius);
            float x2 = (float)(Math.Cos(heading - 0.51f) * radius);
            float y2 = (float)(Math.Sin(heading - 0.51f) * radius);

            float x3 = (float)(Math.Cos(heading + 0.51f + 3.14f) * radius);
            float y3 = (float)(Math.Sin(heading + 0.51f + 3.14f) * radius);
            float x4 = (float)(Math.Cos(heading - 0.51f + 3.14f) * radius);
            float y4 = (float)(Math.Sin(heading - 0.51f + 3.14f) * radius);
			
			float ox1,ox2,ox3,ox4;
			float oy1,oy2,oy3,oy4;
			
			frame.offset_to_display(x1,y1,out ox1,out oy1);
			ox1+=cx;
			oy1+=cy;

			frame.offset_to_display(x2,y2,out ox2,out oy2);
			ox2+=cx;
			oy2+=cy;

			frame.offset_to_display(x3,y3,out ox3,out oy3);
			ox3+=cx;
			oy3+=cy;

			frame.offset_to_display(x4,y4,out ox4,out oy4);
			ox4+=cx;
			oy4+=cy;

			
            g.DrawLine(Pens.Blue, ox1, oy1, ox2, oy2);
            g.DrawLine(Pens.Blue, ox2, oy2, ox3, oy3);
            g.DrawLine(Pens.Blue, ox1, oy1, ox4, oy4);
            g.DrawLine(Pens.Blue, ox3, oy3, ox4, oy4);
            //3.14=1.57

            //if (scene.POIPosition.Count == sim_engine.robots.Count)
            //{
            //    g.DrawLine(redpen, new PointF((float)location.x, (float)location.y),
            //        new PointF(this.scene.POIPosition[agentIndex].X, this.scene.POIPosition[agentIndex].Y));
            //}

            float dx, dy;

            dx = (float)(radius * Math.Cos(heading));
            dy = (float)(radius * Math.Sin(heading));
			float odx,ody;
			frame.offset_to_display(dx,dy,out odx,out ody);
			dx+=cx;
			dy+=cy;
            g.DrawLine(Pens.Blue, cx,cy, dx,dy);

            //TODO: Lable drawing code, how do we know to do this?
            //label drawing
            //if (bDrawLabel)
            //    g.DrawString(robot_count.ToString(), new Font("Tahoma", 12, FontStyle.Bold), Brushes.Black, r.X, r.Y + (float)radius);
 			foreach(ISensor sensor in sensors)
			{
				sensor.draw(g,frame);
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
    }
	
}
