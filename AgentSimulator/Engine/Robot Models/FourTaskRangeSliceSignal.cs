using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

/**
 *  DELETE PENDING
 * 
 *  Schrum: Decided not to use this after all.
 *  Instead, switch the robot model with each environment
 *  of FourTasks domain. 
 * 
 *  This code duplicates the MazeRobotPieSlice,
 *  but adds a signal sensor to the robot so it will
 *  also work in the team patrol task. Not great
 *  code ... too much code is copied.
 **/
namespace Engine
{
    class FourTaskRangeSliceSignal : Khepera3RobotModelContinuous
    {
        private PieSliceSensorArray pieSliceSensor;
        public static float robotSize = 6.5f;

        public FourTaskRangeSliceSignal()
            : base()
        {
            name = "FourTaskRangeSliceSignal";
            default_speed = 25.0f;
            default_turn_speed = 9.0f;
            actualRange = 40.0f;
            collisionAvoidance = false;
            autopilot = false;
            addtimer = false;
        }

        public override float defaultRobotSize()
        {
            // return 10.5f; // From MazeRobotPieSlice (Dual Task and Two Rooms)
            return robotSize;
        }

        public override int defaultSensorDensity()
        {
            return 5;
        }
        
        public override void populateSensors(int numRangeFinders)
        {
            base.populateSensors(numRangeFinders);
        }
        

        public override void populateSensors()
        {
            base.populateSensors(); // Only adds range finders

            //Adds the pieSlice sensor to the robot
            pieSliceSensor = new PieSliceSensorArray(this);

            // Schrum: This one line distinguishes this code from MazeRobotPieSlice
            sensors.Add(new SignalSensor(this));
        }

        public override void updateSensors(Environment env, List<Robot> robots, CollisionManager cm)
        {
            base.updateSensors(env, robots, cm);
            if (env.name.EndsWith("FourTasks-ENV3.xml") || env.name.EndsWith("FourTasks-ENV4.xml")) 
            { // Only update pie slice sensors in dual task forage and two rooms
                pieSliceSensor.update(env, robots, cm);
            }
            else // For domains that usually have rangefinders only, but are added to the FourTasks group
            {
                // This will clear all pie slice sensor inputs
                pieSliceSensor.update(null, null, null);
            }
        }

        protected override void decideAction(float[] outputs, float timeStep)
        {
            float speed = 20.0f;
            float turnSpeed = 4.28f; ;

            velocity = speed * outputs[1];
            heading += (outputs[2] - outputs[0]) * turnSpeed * timeStep;
        }

    }
}
