using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpNeatLib.NeuralNetwork;

namespace Engine
{
    class EPuckModel : Robot
    {

        public EPuckModel()
        {
            name = "EPuckModel";
        }

        //public EPuckModel(int id, double nx, double ny, double dir, AgentBrain agentBrain, Environment environment,
        //    float sensorNoise, float effectorNoise, float headingNoise, float timeStep)
        //{
        //    this.id = id;
        //    steps = 0;
      
        //    updatecounter = 0;

        //    radius = 7.6f; 

        //    path = new List<Point2D>();
        //    location = new Point2D(nx, ny);
        //    circle = new Circle2D(location, radius);
        //    old_location = new Point2D(location);
        //    //radius = rad;
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
        //    //sensors = new List<ISensor>();
        //    //foreach (double val in rangeFinderSensorAngles)
        //    //{
        //    //    sensors.Add(new RangeFinder(val,this,11.0));
        //    //}
        //}

        public override void populateSensors()
        {
            populateSensors(defaultSensorDensity());
        }

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
                RangeFinder r = new RangeFinder(val, this, 11.0,this.sensorNoise);
                sensors.Add(r);
            }
            
        }


        //public float[] defaultRangeFinderAngles()
        //{
        //    sensors.Add(-1.5);
        //    sensors.Add(-1.0);
        //    sensors.Add(0.0);
        //    sensors.Add(1.0);
        //    sensors.Add(1.5);
        //}

        public override int defaultSensorDensity()
        {
            return 5;
        }

        public override float defaultRobotSize()
        {
            return 7.6f;
        }

        public override void robotSettingsChanged()
        {
        }
		
		public override void doAction()
        {
            //TODO: probably need another function for heading noise?

            //if (autopilot)
            //{
            //    doAutopilot(effectorNoise);
            //    return;
            //}

            float[] inputs = new float[sensors.Count];
            for (int j = 0; j < sensors.Count; j++)
            {
			    inputs[j] = (float)sensors[j].get_value_raw()/350.0f;
				if (inputs[j]>1.0) inputs[j]=1.0f;
            
            }

            agentBrain.setInputSignals(id, inputs);

        }

        protected void decideAction(float[] outputs, double timeStep)
        {
            float delta = 0.00f;
            if ((outputs[1] - outputs[0] > delta) && (outputs[1] - outputs[2] > delta)) //forward
            {
                velocity = 6.5;
            }
            else
            {
                velocity = 0;
                if ((outputs[0] - outputs[1] > delta) && (outputs[0] - outputs[2] > delta)) //turn left
                {
                    heading -= (timeStep * 5.36026)/2.0;
                }
                else if ((outputs[2] - outputs[0] > delta) && (outputs[2] - outputs[1] > delta)) //right
                {
                    heading += (timeStep * 5.36026)/2.0;
                }
            }
        }

        public void draw(Graphics g, float scale)
        {
            //robot drawing
            Rectangle r = new Rectangle();
            r.X = (int)(circle.p.x / scale - radius / scale);
            r.Y = (int)(circle.p.y / scale - radius / scale);
            r.Height = (int)((radius * 2) / scale);
            r.Width = (int)((radius * 2) / scale);
            if (collide_last)
                g.DrawEllipse(Pens.Red, r);
            else
                g.DrawEllipse(Pens.Blue, r);
        }

        public override void networkResults(float[] outputs)
        {

          //  controller.SetInputSignals(inputs);

            //TODO: Should be based on network depth or something
         //   controller.MultipleSteps(2);

           // float[] outputs = new float[controller.OutputNeuronCount];

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
