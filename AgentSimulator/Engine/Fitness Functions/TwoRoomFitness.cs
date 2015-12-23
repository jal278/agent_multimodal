using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class TwoRoomFitness : IFitnessFunction
    {
        int collectedFood=0;
        int POINr = 0;
        // Schrum: tracks whether evaluation ends in collision
        Boolean collided = false;
		Boolean finished = false;
        double portionAlive = 0;

        public IFitnessFunction copy()
        {
            return new TwoRoomFitness();
        }

        public TwoRoomFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Two Room Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {


            double fitness = 0.0f;
            objectives = null;

            //food gathering 
            // Schrum: sets goal to a new food item
            float distFood = (float)(1.0f - (engine.robots[0].location.distance(environment.goal_point) / environment.maxDistance));
			if (finished) {
				distFood = 1.0f;
			}
			//distFood = 0;
            // Schrum: collision has fitness score like another food collected. Hence the 1.0 added in the divisor.
            // The longer a collision is avoided, the more points are earned.
			//fitness = (collectedFood + distFood + (collided ? portionAlive : 1)) / (environment.POIPosition.Count * 1.0f + 1.0);
			fitness = (collectedFood + distFood) / (environment.POIPosition.Count * 1.0f + 1.0);

            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            // Schrum: brain-switching policy
            if (Experiment.multibrain && !Experiment.preferenceNeurons && Experiment.numBrains == 2)
            {
                // Schrum: These magic numbers directly correspond to the Two Rooms environment.
                //         This range of y values is the portion of the environment that is taken
                //         up by the hallway.
                if (ip.agentBrain.getBrainCounter() == 1 && // Don't "switch" if already using right brain
                    ip.robots[0].location.y < 716 && ip.robots[0].location.y > 465) //hallway
                {
                    //Console.WriteLine("Switch to 0");
                    ip.agentBrain.switchBrains(0); // use brain 0 for hallway
                }
                else if (ip.agentBrain.getBrainCounter() == 0)   //room
                {
                    //Console.WriteLine("Switch to 1");
                    ip.agentBrain.switchBrains(1); // use brain 1 when in the open
                }
            }

            // Schrum: Debug: For comparing non-visual eval with visual
            // Prints out locations visited by all robots
            /*
            for (int i = 0; i < ip.robots.Count; i++)
            {
                Console.Write(ip.robots[i].location.x + "\t" + ip.robots[i].location.y + "\t");
                if (ip.robots[i] is EnemyRobot)
                {
                    Console.Write(((EnemyRobot)ip.robots[i]).wallResponse + "\t" + ((EnemyRobot)ip.robots[i]).chaseResponse + "\t" + ip.robots[i].heading + "\t" + ((EnemyRobot)ip.robots[i]).angle + "\t");
                }
            }
            Console.WriteLine();
            */
            // End debug

            //For food gathering
            if (ip.timeSteps == 1)
            {
                environment.goal_point.x = environment.POIPosition[0].X;
                environment.goal_point.y = environment.POIPosition[0].Y;

                collectedFood = 0;
                POINr = 0;
                collided = false;
				finished = false;
                portionAlive = 0;
            }

			Point2D goalPoint = new Point2D (environment.POIPosition [POINr].X, environment.POIPosition [POINr].Y);
	
			float d = (float)ip.robots[0].location.distance(goalPoint);
			bool guidance = true;
			if (d < 20.0f && !finished)
            {
                collectedFood++;
                POINr++;

				if (POINr >= environment.POIPosition.Count) {
					POINr = 0;
					finished = true;
				}

				if (POINr > 4 && POINr < 11) {
					guidance = false;
				}

				if (guidance) {
					environment.goal_point.x = environment.POIPosition [POINr].X;
					environment.goal_point.y = environment.POIPosition [POINr].Y;
				}

			
            }

            // Schrum2: Added to detect robot collisions and end the evaluation when they happen
			if (ip.robots[0].collisions > 0)
            { // Disabling prevents further action
                collided = true;
                portionAlive = (ip.elapsed * 1.0) / Experiment.evaluationTime;
                //Console.WriteLine("Fitness before:" + ip.elapsed + "/" + Experiment.evaluationTime + ":" + ip.timeSteps);
                ip.elapsed = Experiment.evaluationTime; // force end time: only affects non-visual evaluation
                Experiment.running = false; // Ends visual evaluation
                //Console.WriteLine("Collision");
                //Console.WriteLine("Fitness after:" + ip.elapsed + "/" + Experiment.evaluationTime + ":" + ip.timeSteps);
            }
        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
