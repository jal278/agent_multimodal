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
        Robot evolved; // Enemy always knows where evolved bot is

        public EnemyRobot(Robot evolvedBot)
            : base()
        {
            name = "EnemyRobot";
            default_speed = 25.0f;
            default_turn_speed = 9.0f;
            actualRange = 40.0f;
            collisionAvoidance = false;
            autopilot = false;
            addtimer = false;
            evolved = evolvedBot;
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

            const double TURN_AMOUNT = Math.PI / 50.0;
            Line2D toEvolved = new Line2D(location, evolved.location);
            double angleDifference = toEvolved.signedAngleFromSourceHeadingToTarget(heading);
            // Schrum2: Debug
            //Console.WriteLine(location + ":"+ evolved.location + ":"+ toEvolved + ":" + angleDifference);
            //Console.WriteLine(angleDifference + ":" + TURN_AMOUNT);
            if (angleDifference < 0)
            { // turn towards evolved bot
                heading -= TURN_AMOUNT;
            } else {
                heading += TURN_AMOUNT;
            }

            // Schrum2: Need to do this manually.
            updatePosition(); // Moves robot based on velocity and heading
        }
    }
}
