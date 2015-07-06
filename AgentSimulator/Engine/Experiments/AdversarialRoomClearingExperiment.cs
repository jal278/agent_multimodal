using System;
using System.Collections.Generic;
using System.Text;

/**
 * Schrum2: Make an experiment that allows for an NPC adversary.
 * Simply modifies RoomClearingExperiment by adding an NPC enemy.
 * Currently, the enemy is added at a fixed location (magic numbers),
 * which should be improved in the future.
 */
namespace Engine
{
    public class AdversarialRoomClearingExperiment : RoomClearingExperiment
    {

        List<EnemyRobot> enemies = new List<EnemyRobot>();

        public AdversarialRoomClearingExperiment()
            : base()
        {
            // Debug:schrum2
            //Console.WriteLine("Init AdversarialRoomClearingExperiment");
        }

        public AdversarialRoomClearingExperiment(MultiAgentExperiment exp)
            : base(exp)
        {
            // Debug:schrum2
            //Console.WriteLine("Init AdversarialRoomClearingExperiment With MultiAgentExperiment");
        }

        override protected bool runEnvironment(Environment e, instance_pack ip, System.Threading.Semaphore sem)
        {
            // Schrum: ip is null initially, but should not be after first step
            if (ip != null)
            {
                //Console.WriteLine("Update ip");
                // Make sure enemies have accurate reference for evolved bot
                foreach(EnemyRobot r in enemies){
                    r.evolved = ip.robots[0];
                }
            }
            //else Console.WriteLine("IP NULL!");
            // Then run the environment as normal
            return base.runEnvironment(e, ip, sem);
        }

        // Schrum2: Add extra enemy robot after deault robots
        public override void initializeRobots(AgentBrain agentBrain, Environment e, float headingNoise, float sensorNoise, float effectorNoise, instance_pack ip)
        {
            // Debug:schrum2
            //Console.WriteLine("Init Robots");
            base.initializeRobots(agentBrain, e, headingNoise, sensorNoise, effectorNoise, ip);

            for (int i = 0; i < numEnemies; i++)
            {

                // schrum2: here is where the enemy robot is added
                // Location of evolved robot is needed to track it
                // Assumes that robot is in position 0 (problem?)
                EnemyRobot r = new EnemyRobot(robots[0], flee);
                enemies.Add(r);

                double _timestep = 0.0;
                if (ip == null)
                {
                    _timestep = this.timestep;
                }
                else
                {
                    _timestep = ip.timestep;
                }

                double nx = enemyX - i * enemyDeltaX;
                double ny = enemyY - i * enemyDeltaY;
                double h = 0;

                // Alternative starting positions for enemies
                if (enemiesSurround)
                {
                    double evolvedX = robots[0].location.x;
                    double evolvedY = robots[0].location.y;

                    double radius = 200.0;

                    nx = evolvedX + radius*Math.Cos((i * 2.0 * Math.PI) / numEnemies);
                    ny = evolvedY + radius*Math.Sin((i * 2.0 * Math.PI) / numEnemies);
                    h = Math.PI + i * (2 * Math.PI / numEnemies);
                }

                r.init(robots.Count, // id is last position in list of robots
                    nx, ny, h, // starting position and heading
                    agentBrain, // Has evolved brain (to avoid NullPointers, etc), but should never actually use it (problem?) 
                    e, sensorNoise, effectorNoise, headingNoise, (float)_timestep);

                r.collisionPenalty = collisionPenalty;
                // Only add enemy if it is not already present
                if (robots.Count <= numEnemies) // Makes limiting assumption that this experiment will always be used with only one evolved bot and one enemy
                {
                    robots.Add(r);
                    //Console.WriteLine("Added enemy to list: " + robots.Count);
                }

                // Required so that experiment works properly in both visual and non-visual mode
                // (The non-visual mode takes the robots from the instance_pack instead of the experiment)
                if (ip != null && ip.robots.Count <= numEnemies)
                {
                    ip.robots.Add(r);
                    //Console.WriteLine("Added enemy to ip");
                }
            }
        }

    }
}