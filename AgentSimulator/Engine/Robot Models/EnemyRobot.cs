using System;
using System.Collections.Generic;
using System.Text;
using simulator.Sensor_Models;

/**
 * Schrum2: Made this robot to serve as an NPC enemy.
 * It approaches the evolving robot, assuming it is in
 * index 0 of the robots array. Bot also turns to avoid walls,
 * but when doing this it does not take the evolved bot
 * location into account, so there is room for improvement here.
 */
namespace Engine
{
    class EnemyRobot : Khepera3RobotModelContinuous
    {
        public Robot evolved; // Enemy always knows where evolved bot is
        public double wallResponse = 0;
        public double chaseResponse = 0;
        public double angle = 0;

        public Robot getEvolved()
        {
            return evolved;
        }

        public EnemyRobot(Robot bot)
            : base()
        {
            name = "EnemyRobot";
            default_speed = 25.0f;
            default_turn_speed = 9.0f;
            actualRange = 40.0f;
            collisionAvoidance = false;
            autopilot = false;
            addtimer = false;
            evolved = bot;

            // Schrum: Start facing down: Doesn't seem to work
            //heading = 3 * Math.PI / 2;

            // Schrum: debug
            //Console.WriteLine("Create enemy robot");
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
            //Schrum: debug
            //Console.WriteLine("-------------------------------------------------------");
            //Console.WriteLine("Start EnemyRobot.doAction(): heading = " + heading);
            //Console.WriteLine(evolved.location.x + "\t" + evolved.location.y + "\t" + location.x + "\t" + location.y);
            //Console.WriteLine("Enemy doAction");
            
            // Schrum2: Simple behavior
            // Speed calculation taken from Khepera3RobotModelContinuous, but made faster
            // Perhaps too fast? Originally 9 ... 10 still seems reasonable. Not sure about 11.
            // Make parameter?
            float speed = 11.0f * (1.0f + (effectorNoise / 100.0f * (float)(2.0 * (rng.NextDouble() - 0.5))));
            velocity = speed;

            const double TURN_AMOUNT = Math.PI / 50.0;
            // Schrum: Debug lines from EnemyRobot to evolved bot.
            //Console.WriteLine(location.x + "\t" + location.y + "\n" + evolved.location.x + "\t" + evolved.location.y + "\n");

            Line2D toEvolved = new Line2D(location, getEvolved().location);
            double angleDifference = toEvolved.signedAngleFromSourceHeadingToTarget(heading);
            //Console.WriteLine("Start EnemyRobot.doAction(): angleDifference = " + angleDifference);
            double distance = toEvolved.length();
            // Schrum2: Debug
            //Console.WriteLine(location + ":"+ evolved.location + ":"+ toEvolved + ":" + angleDifference);
            //Console.WriteLine(angleDifference + ":" + TURN_AMOUNT);

            Boolean evolvedClose = distance < 40; // Magic number ... needs tweaking

            // schrum2: check sensors for walls.
            // sensor values of 1 mean there is no wall contact.
            // Lesser values means wall is closer along that sensor
            //Console.WriteLine("Check");
            double left = 0;
            int half = sensors.Count / 2;
            for (int j = 0; j < half; j++) {
                if (!(sensors[j] is SignalSensor)) {
                    left += transformSensor(sensors[j].get_value_raw());
                }
            }
            //Console.WriteLine("\t:left:" + left);
            double right = 0;
            for (int j = half+1; j < sensors.Count; j++)
            {
                if (!(sensors[j] is SignalSensor))
                {
                    right += transformSensor(sensors[j].get_value_raw());
                }
            }
            //Console.WriteLine("\t:right:" + right);

            wallResponse = 0;
            chaseResponse = 0;
            angle = angleDifference;
            if (!evolvedClose && right < left) 
            { // right sensors are closer to wall
                // turn left
                heading -= TURN_AMOUNT;
                wallResponse = -TURN_AMOUNT;
                //Console.WriteLine("Turn left to dodge wall: heading = " + heading);
            }
            else if (!evolvedClose && left < right)
            { // left sensors are closer to wall
                // turn right
                heading += TURN_AMOUNT;
                wallResponse = TURN_AMOUNT;
                //Console.WriteLine("Turn right to dodge wall: heading = " + heading);
            }
            else
            {
                // Base turn purely on evolved bot location if there are no wall problems
                if (angleDifference < 0)
                { // turn towards evolved bot
                    heading -= TURN_AMOUNT;
                    chaseResponse = -TURN_AMOUNT;
                    //Console.WriteLine("Turn left to chase enemy: heading = " + heading);
                }
                else
                {
                    heading += TURN_AMOUNT;
                    chaseResponse = TURN_AMOUNT;
                    //Console.WriteLine("Turn right to chase enemy: heading = " + heading);
                }
            }

            // Schrum2: Need to do this manually.
            updatePosition(); // Moves robot based on velocity and heading
            //Console.WriteLine("After updatePosition(): heading = " + heading);

            /*
            if (heading > Math.PI * 2)
            {
                Console.WriteLine("Heading out of range (doAction): " + heading);
                System.Windows.Forms.Application.Exit();
                System.Environment.Exit(1);
            }
            */
        }
    }
}
