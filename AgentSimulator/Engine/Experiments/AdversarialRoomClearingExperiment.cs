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
            base.initializeRobots(agentBrain, e, headingNoise, sensorNoise, effectorNoise, ip);

            // schrum2: here is where the enemy robot is added
            // Location of evolved robot is needed to track it
            // Assumes that robot is in position 0 (problem?)
            Robot r = new EnemyRobot(robots[0]);

            double _timestep = 0.0;
            if (ip == null) {
                _timestep = this.timestep;
            } else {
                _timestep = ip.timestep;
            }

            // Schrum: will definitely need to change these into parameters
            double nx = 300;
            double ny = 300;
            r.init(robots.Count, // id is last position in list of robots
                nx, ny, 0, // starting position and heading of 0 (change?)
                agentBrain, // Has evolved brain (to avoid NullPointers, etc), but should never actually use it (problem?) 
                e, sensorNoise, effectorNoise, headingNoise, (float)_timestep);

            r.collisionPenalty = collisionPenalty;
            robots.Add(r);
        }
        
    }
}