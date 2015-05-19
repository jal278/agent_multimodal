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

        // Schrum2: Add extra enemy robot after deault robots
        public override void initializeRobots(AgentBrain agentBrain, Environment e, float headingNoise, float sensorNoise, float effectorNoise, instance_pack ip)
        {
            // Debug:schrum2
            //Console.WriteLine("Init Robots");
            base.initializeRobots(agentBrain, e, headingNoise, sensorNoise, effectorNoise, ip);



            // schrum2: here is where the enemy robot is added
            // Location of evolved robot is needed to track it
            // Assumes that robot is in position 0 (problem?)
            Robot r = new EnemyRobot(robots[0]);

            double _timestep = 0.0;
            if (ip == null)
            {
                _timestep = this.timestep;
            }
            else
            {
                _timestep = ip.timestep;
            }

            // Schrum: will definitely need to change these into parameters
            double nx = enemyX; // 300;
            double ny = enemyY; // 300;
            r.init(robots.Count, // id is last position in list of robots
                nx, ny, 0, // starting position and heading of 0 (change?)
                agentBrain, // Has evolved brain (to avoid NullPointers, etc), but should never actually use it (problem?) 
                e, sensorNoise, effectorNoise, headingNoise, (float)_timestep);

            r.collisionPenalty = collisionPenalty;
            // Only add enemy if it is not already present
            if (robots.Count < 2) // Makes limiting assumption that this experiment will always be used with only one evolved bot and one enemy
            {
                robots.Add(r);
                //Console.WriteLine("Added enemy to list: " + robots.Count);
            }

            // Required so that experiment works properly in both visual and non-visual mode
            // (The non-visual mode takes the robots from the instance_pack instead of the experiment)
            if (ip != null && ip.robots.Count < 2)
            {
                ip.robots.Add(r);
                //Console.WriteLine("Added enemy to ip");
            }
        }

    }
}