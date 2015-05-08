using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

/**
 * Schrum2: Made this robot to serve as an NPC enemy 
 */
namespace Engine
{
    class EnemyRobot : Khepera3RobotModelContinuous
    {
        public EnemyRobot()
            : base()
        {
            name = "EnemyRobot";
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
                
        protected override void decideAction(float[] outputs, float timeStep)
        {
            // schrum2: Ok to do nothing here?
            //Console.WriteLine("Enemy decideAction");
        }

        public override void doAction()
        {
            // Schrum: Ok to do nothing here?
            //Console.WriteLine("Enemy doAction");
            
            // Schrum2: Simple behavior
            // Speed calculation taken from Khepera3RobotModelContinuous
            float speed = 9.0f * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));
            velocity = speed;
            heading = 0; // TODO: set intelligently
            // Schrum2: Need to do this manually.
            updatePosition(); // Moves robot based on velocity and heading
        }
    }
}
