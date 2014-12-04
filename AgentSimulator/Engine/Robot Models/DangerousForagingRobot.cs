using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

namespace Engine
{
    class DangerousForagingRobot : MazeRobotPieSlice
    {
        public DangerousForagingRobot()
            : base()
        {
            name = "DangerousForagingRobot";
        }

        public override void populateSensors()
        {
            base.populateSensors(0); // sets rangefinder number to 0. There will still be pie-slice sensors though
            sensors.Add(new SignalSensor(this)); // Detects when food/poison

            // Debugging
            //Console.WriteLine("Num sensors = " + sensors.Count);
        }

    }
}
