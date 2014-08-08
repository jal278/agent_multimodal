using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Engine
{
    class FitnessBC : IBehaviorCharacterization
    {
		List<double> bc;
		public	IBehaviorCharacterization copy() {
			return new FitnessBC();
		}

		public FitnessBC()
		{
			bc=new List<double>();
			bc.Add(0.001);
		}
		
        #region IBehaviorCharacterization Members

        string IBehaviorCharacterization.name
        {
            get { return this.GetType().Name; }
        }

        string IBehaviorCharacterization.description
        {
            get { return "Vector of robot's ending locations appended together"; }
        }


        //characterizing behavior...
        List<double> IBehaviorCharacterization.calculate(SimulatorExperiment exp,instance_pack i)
        {   
		//	Console.WriteLine(bc.Count.ToString());
			double[] obj;
			//TODO: fix eventually;
			//bc[0]=exp.fitnessFunction.calculate(exp,exp.environment,out obj);
            bc[0]=0.0;
			return new List<double>(bc);
        }

        void IBehaviorCharacterization.update(SimulatorExperiment exp,instance_pack i)
		{
            //TODO reimplement
        //    double accum=0.0;
        //            //if the timestep is changed, this needs to be changed as well.  First # is 20/timestep, Second # is 1/timestep


        //    if (!(engine.timeSteps >= 50 && engine.timeSteps % 25 == 0))
        //        return;

        //    //double minDistance;
        //    //int closestAgentIndex, agentIndex, robotIndex;

        //    //check to see which robots are in, we don't want to calculate distance between agents outside the room
        //   /* bool allIn = true;
        //    bool[] isIn = new bool[engine.robots.Count];
        //    for (robotIndex = 0; robotIndex < engine.robots.Count; robotIndex++)
        //    {
        //        if (engine.robots[robotIndex].autopilot)
        //            return;
        //        //isIn[robotIndex] = false;
        //        else
        //            isIn[robotIndex] = environment.AOIRectangle.Contains((int)engine.robots[robotIndex].location.x, (int)engine.robots[robotIndex].location.y);
        //        if (!isIn[robotIndex])
        //            allIn = false;
        //    }

        //    if (!allIn)
        //        return;*/

        //    int dim = engine.grid.coarseness;
        //    for (int x = 0; x < dim; x++)
        //    {
        //        for (int y = 0; y < dim; y++)
        //        {
        //            int gx = (int)((double)x * engine.grid.gridx) + (int)(engine.grid.gridx / 2.0);
        //            int gy = (int)((double)y * engine.grid.gridy) + (int)(engine.grid.gridy / 2.0);
        //            if ((environment.AOIRectangle.Contains(gx, gy)))
        //                //if ((engine.grid.grid[x, y].viewed) != 0.0)
        //                 //   accum += 1.0;
        //                accum += engine.grid.grid[x,y].viewed;
        //        }
        //    }
        //    bc.Add(accum);
			
		
        ////now update the distance values

        //    /*for (robotIndex = 0; robotIndex < engine.robots.Count; robotIndex++)
        //    {
        //        if (!isIn[robotIndex])
        //            continue;
        //        closestAgentIndex = -1;
        //        minDistance = double.MaxValue;
        //        for (agentIndex = 0; agentIndex < engine.robots.Count; agentIndex++)
        //        {
        //            if (!isIn[agentIndex] || agentIndex == robotIndex)
        //                continue;
        //            double dist = engine.euclideanDistance(engine.robots[robotIndex], engine.robots[agentIndex]);
        //            if (dist < minDistance)
        //            {
        //                minDistance = dist;
        //                closestAgentIndex = agentIndex;
        //            }
        //        }

        //        if (closestAgentIndex == -1)
        //            continue;

        //        engine.robots[robotIndex].distanceClosestPOI.Add((minDistance)/564.0);

        //        //if (engine.timeSteps >= 20 * 20)
        //        //    engine.robots[robotIndex].distanceClosestPOI.Add(5 * (minDistance));

        //    }*/
			
		}
		
		void IBehaviorCharacterization.reset() 
		{
			
		}
        #endregion
    }
}
