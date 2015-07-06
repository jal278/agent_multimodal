using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class PreyCaptureFitness : IFitnessFunction
    {
        double livingCost = 0;
        const double PREY_REWARD = 1000;
        double rewards = 0;
        double fitness = 0;

        public IFitnessFunction copy()
        {
            return new PreyCaptureFitness();
        }

        public PreyCaptureFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Prey Capture Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = null;
            //Console.WriteLine("rewards:" + rewards + ":livingCost:" + livingCost + ":fitness:" + (rewards - livingCost));
            return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            // Initialize variables on first time step
            if (ip.timeSteps == 1)
            {
                livingCost = 0;
                rewards = 0;
                // Fitness values must be positive. Therefore, a high positive fitness is assigned in advance,
                // and the cost of living subtracts from it.
                // time / steps is the number of actual time steps, and the cost is multiplied by number of enemies
                fitness = (Experiment.evaluationTime / Experiment.timestep) * Experiment.numEnemies;
            }

            // Find closest active prey
            bool allCaptured = true; // becomes false if an active prey is found
            Robot evolved = ip.robots[0];
            for (int i = 1; i < ip.robots.Count; i++)
            {
                // Assumes all but first robot are EnemyRobot instances
                EnemyRobot er = (EnemyRobot)ip.robots[i];
                if (!er.disabled) // Not captured yet
                {
                    //Console.WriteLine("Robot "+i+" not disabled");

                    allCaptured = false;
                    // The collisionWithEvolved bool should always be the primary means of detecting these
                    // collisions, but the other call is here as a precaution. This check is needed because
                    // when the evolved bot normally collides with the enemy, the "undo" method is called,
                    // which prevents the collision from being detected using normal means in this method.
                    // Fortunately, the collisionWithEvolved member is used to remember when this collision
                    // occurs.
                    if (er.collisionWithEvolved || EngineUtilities.collide(evolved, er))
                    { // Reward evolved bot for colliding with prey, and disable prey
                        er.disabled = true;
                        er.stopped = true;
                        rewards += PREY_REWARD;
                        fitness += PREY_REWARD; // This is the value that matters
                        //Console.WriteLine("\treward:" + rewards + " from " + PREY_REWARD);
                    }
                    else
                    { // Each active prey punishes bot for not being caltured yet
                        double distance = evolved.location.distance(er.location);
                        double cost = distance / environment.maxDistance;
                        livingCost += cost;
                        fitness -= cost; // This is the value that matters
                        //Console.WriteLine("\tCost: " + (distance / 1000.0) + " to be " + livingCost + " raw distance: " + distance);
                    }
                }
            }

            // End evaluation and stop accruing negative fitness if all prey are captured
            if (allCaptured)
            { // Disabling prevents further action
                ip.elapsed = Experiment.evaluationTime; // force end time: only affects non-visual evaluation
                Experiment.running = false; // Ends visual evaluation
            }
        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
