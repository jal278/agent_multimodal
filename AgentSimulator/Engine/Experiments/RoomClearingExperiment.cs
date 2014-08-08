using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class RoomClearingExperiment : MultiAgentExperiment
    {
		
        public RoomClearingExperiment()
            : base()
        {
			/*collisionPenalty=false;
            homogeneousTeam = false;

            scriptFile = "";
            numberRobots = 1;

            sensorDensity = 21;
            timestep = 0.2f;

            evaluationTime = 100;

            explanatoryText = "Multi-Agent Room Clearing Experiment";
            fitnessFunctionName = "CoverageFitness";
            robotModelName = "RoomClearingRobotModel";

            initialized = false;
            running = false;

            gridCollision = true;*/
        }

        public RoomClearingExperiment(MultiAgentExperiment exp)
        {
            gridCollision = exp.gridCollision;

            substrateDescriptionFilename = exp.substrateDescriptionFilename;
            adaptableANN = exp.adaptableANN;
            modulatoryANN = exp.modulatoryANN;
            normalizeWeights = exp.normalizeWeights;
            headingNoise = exp.headingNoise;
            effectorNoise = exp.effectorNoise;
            sensorNoise = exp.sensorNoise;

            timesToRunEnvironments = exp.timesToRunEnvironments;
            evaluationTime = exp.evaluationTime;
            initialized = false;

            timestep = exp.timestep;
            explanatoryText = exp.explanatoryText;

            scriptFile = exp.scriptFile;

            robotModelName = exp.robotModelName;

            fitnessFunctionName = exp.fitnessFunctionName;
            behaviorCharacterizationName = exp.behaviorCharacterizationName;
            environmentName = exp.environmentName;


            bestGenomeSoFar = exp.bestGenomeSoFar;

            numberRobots = exp.numberRobots;

            homogeneousTeam = exp.homogeneousTeam;

          //  overrideDefaultSensorDensity = exp.overrideDefaultSensorDensity;
            rangefinderSensorDensity = exp.rangefinderSensorDensity;

            useScript = exp.useScript;

            running = exp.running;

            agentsVisible = exp.agentsVisible;
            agentsCollide = exp.agentsCollide;

            genome = exp.genome;

            multipleEnvironments = exp.multipleEnvironments;

            evolveSubstrate = exp.evolveSubstrate;

            elapsed = exp.elapsed;

            populationSize = exp.populationSize;

            overrideTeamFormation = exp.overrideTeamFormation;
            group_orientation = exp.group_orientation;
            group_spacing = exp.group_spacing;
            robot_heading = exp.robot_heading;

            multibrain = exp.multibrain;
            numBrains = exp.numBrains; // Schrum: added by me
            ungeometricBrains = exp.ungeometricBrains; // Schrum: added by me
            multiobjective = exp.multiobjective;

        }

        /*protected internal override bool runEnvironment(Environment e,instance_pack x,System.Threading.Semaphore sem)
        {
	        bool value = base.runEnvironment(e,x,sem);
			List<Robot> rbts;
			if(x==null)
				rbts = robots;
			else
				rbts = x.robots;
	        foreach (Robot r in rbts)
            {
                if (e.AOIRectangle.Contains((int)r.location.x, (int)r.location.y))
                    r.autopilot = false;
                else
                    r.autopilot = true;
			}

            return value;
        }*/
    }
}