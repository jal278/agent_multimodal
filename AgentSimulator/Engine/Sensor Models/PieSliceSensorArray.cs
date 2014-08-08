using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace simulator.Sensor_Models
{
    class PieSliceSensorArray
    {
        public List<float> radarAngles1; //beginning angles for radar sensors
        public List<float> radarAngles2; //ending angles for radar sensors
        //public List<float> radar;

        public List<SignalSensor> signalsSensors;
        public Robot owner;

        public PieSliceSensorArray(Robot o)
        {
            signalsSensors = new List<SignalSensor>(4);

            radarAngles1 = new List<float>();
            radarAngles2 = new List<float>();
            //radar = new List<float>();

            //TODO make the number of slices adjustable

            //define the radar sensors
            radarAngles1.Add(315.0f);
            radarAngles2.Add(405.0f);

            radarAngles1.Add(45.0f);
            radarAngles2.Add(135.0f);

            radarAngles1.Add(135.0f);
            radarAngles2.Add(225.0f);

            radarAngles1.Add(225.0f);
            radarAngles2.Add(315.0f);

            for (int i = 0; i < 4; i++)
            {
                SignalSensor s = new SignalSensor(o);
                signalsSensors.Add(s);
                o.sensors.Add(signalsSensors[i]);
            }
            owner = o;
		}


        //per default we use the goal point of the environment as our target point. 
        public void update(Engine.Environment env, List<Robot> robots, CollisionManager cm)
        {
            Point2D temp = new Point2D((int)env.goal_point.x, (int)env.goal_point.y);

            temp.rotate((float)-(owner.heading * 180.0 / 3.14), owner.circle.p);

            //translate with respect to location of navigator
            temp.x -= (float)owner.circle.p.x;
            temp.y -= (float)owner.circle.p.y;

            //what angle is the vector between target & navigator
            float angle = (float)temp.angle();

            angle *= 57.297f;//convert to degrees

            //fire the appropriate radar sensor
            for (int i = 0; i < radarAngles1.Count; i++)
            {
                signalsSensors[i].setSignal(0.0);
                //radar[i] = 0.0f;

                if (angle >= radarAngles1[i] && angle < radarAngles2[i])
                {
                    signalsSensors[i].setSignal(1.0);
                 //   Console.WriteLine(i);
                }
                //radar[i] = 1.0f;

                if (angle + 360.0 >= radarAngles1[i] && angle + 360.0 < radarAngles2[i])
                {
                    signalsSensors[i].setSignal(1.0);
                   // Console.WriteLine(i);
                }
//                    radar[i] = 1.0f;

                
               // inputs[sim_engine.robots[0].rangefinders.Count + i] = sim_engine.radar[i];

            }

        }
    }
}
