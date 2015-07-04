using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace simulator.Sensor_Models
{
    class EnemyPieSliceSensorArray : PieSliceSensorArray
    {
        public EnemyPieSliceSensorArray(Robot o) : base(o)
        {
            // Just call parent constructor
        }

        //Different from PieSliceSensorArray in that EnemyRobots are used as targets instead
        // of goal points. 
        public void update(Engine.Environment env, List<Robot> robots, CollisionManager cm)
        {
            List<float> translatedEnemyAngles = new List<float>();
            // Start at 1: Assume all robots after first are enemies
            for (int i = 1; i < robots.Count; i++)
            {
                EnemyRobot er = (EnemyRobot) robots[i];
                if (!er.stopped) // Only sense prey robots that are active
                {
                    Point2D temp = new Point2D((int)er.location.x, (int)er.location.y);

                    temp.rotate((float)-(owner.heading), owner.circle.p);  //((float)-(owner.heading * 180.0 / 3.14), owner.circle.p);

                    //translate with respect to location of navigator
                    temp.x -= (float)owner.circle.p.x;
                    temp.y -= (float)owner.circle.p.y;

                    //what angle is the vector between target & navigator
                    float angle = angleValue((float)temp.x, (float)temp.y);// (float)temp.angle();

                    translatedEnemyAngles.Add(angle);
                }
            }

            //fire the appropriate radar sensor
            for (int i = 0; i < radarAngles1.Count; i++)
            {
                signalsSensors[i].setSignal(0.0);

                for (int a = 0; a < translatedEnemyAngles.Count; a++)
                {
                    float angle = translatedEnemyAngles[a];

                    if (angle >= radarAngles1[i] && angle < radarAngles2[i])
                    {
                        signalsSensors[i].setSignal(1.0);
                    }

                    if (angle + 360.0 >= radarAngles1[i] && angle + 360.0 < radarAngles2[i])
                    {
                        signalsSensors[i].setSignal(1.0);
                    }
                }
            }

        }
    }
}
