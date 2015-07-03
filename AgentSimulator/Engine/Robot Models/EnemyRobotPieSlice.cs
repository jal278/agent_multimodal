using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

namespace Engine
{
    // Schrum: This is basically identical to MazeRobotPieSlice, but uses
    // EnemyPieSliceSensorArray to sense enemies

    class EnemyRobotPieSlice : Khepera3RobotModelContinuous
    {
        private EnemyPieSliceSensorArray pieSliceSensor;

        public EnemyRobotPieSlice()
            : base()
        {
            name = "EnemyRobotPieSlice";
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
        
        public override void populateSensors(int numRangeFinders)
        {
            base.populateSensors(numRangeFinders);
            if (numRangeFinders == 0)
                pieSliceSensor = new EnemyPieSliceSensorArray(this);
        }
        

        public override void populateSensors()
        {
            base.populateSensors();
            pieSliceSensor = new EnemyPieSliceSensorArray(this);
        }

        public override void updateSensors(Environment env, List<Robot> robots, CollisionManager cm)
        {
            base.updateSensors(env, robots, cm);
            pieSliceSensor.update(env, robots, cm);
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
