using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

namespace Engine
{
    class MazeRobotPieSlice : Khepera3RobotModelContinuous
    {
        private PieSliceSensorArray pieSliceSensor;

        public MazeRobotPieSlice()
            : base()
        {
            name = "MazeRobotPieSlice";
            default_speed = 25.0f;
            default_turn_speed = 9.0f;
            actualRange = 40.0f;
            collisionAvoidance = false;
            autopilot = false;
            addtimer = false;
        }

        public override float defaultRobotSize()
        {
            return 10.5f;
        }

        public override int defaultSensorDensity()
        {
            return 5;
        }
        
        /*
         * Schrum: Adding the if statement seems to fix this method so that it works
         * with both Dangerous Foraging and Two Rooms experiments. Need to keep an eye on
         * it though.
         */ 
        // Schrum: Added so pie-slice sensors would be initialized even with specialized number of rangefinders
        public override void populateSensors(int numRangeFinders)
        {
            //Console.WriteLine("numRangeFinders:" + numRangeFinders);
            base.populateSensors(numRangeFinders);
            //Console.WriteLine("before: " + pieSliceSensor);
            
            //This code only seems to be needed for Dangerous Foraging, which
            // has no range finders.
            if (numRangeFinders == 0) 
                pieSliceSensor = new PieSliceSensorArray(this);
            //Console.WriteLine("after: " + pieSliceSensor);
        }
        

        public override void populateSensors()
        {
            base.populateSensors();

            //Adds the pieSlice sensor to the robot
            pieSliceSensor = new PieSliceSensorArray(this);
        }

        public override void updateSensors(Environment env, List<Robot> robots, CollisionManager cm)
        {

           // Console.WriteLine(this.sensors.Count);
            base.updateSensors(env, robots, cm);
            pieSliceSensor.update(env, robots, cm);
        }

        protected override void decideAction(float[] outputs, float timeStep)
        {
            float speed = 20.0f;
            float turnSpeed = 4.28f; ;

            velocity = speed * outputs[1];
            heading += (outputs[2] - outputs[0]) * turnSpeed * timeStep;
         //   if (heading < 0) heading = 0;;
         //   if (heading >= 3.2) heading = 0;
        }

        //public override void doAction()
        //{
        //    base.doAction();
        //  //  pieSliceSensor.update();
        //}
    }
}
