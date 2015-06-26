using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Engine
{
    class AdversarySurvivalFitness : IFitnessFunction
    {
        // Schrum: tracks whether evaluation ends in collision
        Boolean collided = false;
        double portionAlive = 0;

        public IFitnessFunction copy()
        {
            return new AdversarySurvivalFitness();
        }

        public AdversarySurvivalFitness()
        {

        }

        #region IFitnessFunction Members

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Adversary Survival Fitness"; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] objectives)
        {
            objectives = null;
            //Console.WriteLine("Fitness " + portionAlive);
            // Schrum: Debug
            /*
            if (portionAlive == 2001)
            {
                for (int i = 0; i < ip.robots[0].history.Count; i++)
                {
                    Console.WriteLine(ip.robots[0].history[i].x + "\t" + ip.robots[0].history[i].y);
                }
                System.Windows.Forms.Application.Exit();
                System.Environment.Exit(1);
            }
            */
            return portionAlive;
        }
        //int i = 0;
        void IFitnessFunction.update(SimulatorExperiment Experiment, Environment environment, instance_pack ip)
        {
            // Schrum: Debug: For comparing non-visual eval with visual
            // Prints out locations visited by all robots
            /*
            for (int i = 0; i < ip.robots.Count; i++)
            {
                Console.Write(ip.robots[i].location.x + "\t" + ip.robots[i].location.y + "\t");
                if (ip.robots[i] is EnemyRobot)
                {
                    Console.Write(((EnemyRobot)ip.robots[i]).wallResponse + "\t" + ((EnemyRobot)ip.robots[i]).chaseResponse + "\t" + ip.robots[i].heading + "\t" + ((EnemyRobot)ip.robots[i]).angle + "\t" + ip.robots[i].collisions + "\t");
                }
            }
            Console.WriteLine();
            */
            /*
            if (ip.robots[0].location.x != ((EnemyRobot)ip.robots[1]).getEvolved().location.x || ip.robots[0].location.y != ((EnemyRobot)ip.robots[1]).getEvolved().location.y)
            {
                Console.WriteLine("Different locations:");
                Console.WriteLine("Robot 0: " + ip.robots[0].location);
                Console.WriteLine("Enemy's reference refr   to evolved: " + ((EnemyRobot)ip.robots[1]).getEvolved().location);
                if (i++ > 5)
                {
                    System.Windows.Forms.Application.Exit();
                    System.Environment.Exit(1);
                }
            }
            */
            if (ip.timeSteps == 1)
            {
                collided = false;
                portionAlive = 0;
            }

            // Schrum2: Added to detect robot collisions and end the evaluation when they happen
            if (ip.robots[0].collisions > 0)
            { // Disabling prevents further action
                //Console.WriteLine("Collision");
                collided = true;
                ip.elapsed = Experiment.evaluationTime; // force end time: only affects non-visual evaluation
                Experiment.running = false; // Ends visual evaluation
            }
            else
            {
                portionAlive++;
            }
        }

        void IFitnessFunction.reset()
        {
        }

        #endregion
    }
}
