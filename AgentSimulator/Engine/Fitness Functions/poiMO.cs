using System;
using System.Collections.Generic;
using System.Text;
using Engine;
namespace Engine
{
    class POIFIT_MO : IFitnessFunction
    {

        bool penalizeGettingToPoints = true;
        bool penalizeSignalResponse = true;
        bool penalizeCorrection = false;

        #region IFitnessFunction Members
        SimulatorExperiment theengine = null;
        double fitness = 0;
        List<int>[] travelList;
        int[] currentLocation;
        int[] takenList;
        bool[] reachList;
        double[] gotoList;
        double[] endList;
        double[] origDist;
        double[] origHeadings;
        double[] sensorList;
        bool first;
        bool allCorrected;
        bool allClose;
		bool[] reachGoal;
        bool[] turned;
        //int steps = 0;
        public IFitnessFunction copy()
        {
            return new POIFIT_MO();
        }

        public POIFIT_MO()
        {
            first = false;
            const int maxsamples = 100;
            const int maxrobots = 10;
			reachGoal = new bool[maxrobots];
            travelList = new List<int>[maxrobots];
            for (int i = 0; i < maxrobots; i++)
                travelList[i] = new List<int>();
            currentLocation = new int[maxrobots];
            endList = new double[maxrobots];
            gotoList = new double[maxrobots];
            origDist = new double[maxrobots];
            origHeadings = new double[maxrobots];

            turned = new bool[maxrobots];
            reachList = new bool[maxrobots];
            takenList = new int[maxrobots];
            sensorList = new double[maxrobots];
            theengine = null;
            allClose = false;
            allCorrected = false;
            this.reset0();
        }

        void reset0()
        {
			for(int i=0;i<reachGoal.Length;i++)
				reachGoal[i]=false;
            for (int i = 0; i < currentLocation.Length; i++)
                currentLocation[i] = -1;
            for (int i = 0; i < travelList.Length; i++)
                travelList[i].Clear();
            for (int i = 0; i < gotoList.Length; i++)
                gotoList[i] = 0;
            for (int i = 0; i < endList.Length; i++)
                endList[i] = 0;
            for (int i = 0; i < origDist.Length; i++)
            {
                origDist[i] = 0;
                origHeadings[i] = 0;
                turned[i] = false;
            }
            for (int i = 0; i < sensorList.Length; i++)
                sensorList[i] = 0;
            first = false;
        }

        string IFitnessFunction.name
        {
            get { return this.GetType().Name; }
        }

        string IFitnessFunction.description
        {
            get { return "Fitness is the sum of the distance of each agent from goal point, each substracted from 1000.  The optimal score is 1000*# of agents."; }
        }

        double IFitnessFunction.calculate(SimulatorExperiment engine, Environment environment, instance_pack ip, out double[] obj)
        {
            obj = new double[6];
            fitness = 0.000001;
            double go_sum = 1.0;
            double ret_sum = 1.0;
            double collide_count = 0;
            bool moved = true;

            //Checks to see if the robots have moved or turned since the signal has fired, meaning that they reacted to the signal 
            for (int j = 0; j < ip.robots.Count; j++)
                if (turned[j] || origDist[j] - ip.robots[j].location.manhattenDistance(environment.goal_point) >= 5)
                {
                    continue;
                }
                else
                {
                    moved = false;
                    break;
                }

            if (!penalizeGettingToPoints)
                allClose = true;

			bool solve=true;
            double temp;
			if(!allClose || !moved) solve=false;
            for (int i = 0; i < ip.robots.Count; i++)
            {
				if(!reachGoal[i]) solve=false;
                if ((allClose && moved) || !penalizeSignalResponse)
                    fitness += gotoList[i];
                else 
                    fitness += gotoList[i] / 10.0;
                temp = endList[i];
                    //if ((penalizeCorrection && !allCorrected) || (penalizeGettingToPoints && !allClose) || (penalizeSignalResponse && !moved))
                if (penalizeCorrection && (!allCorrected || !allClose))
                    temp /= 100;
                else if (penalizeGettingToPoints && !allClose)
                    temp /= 100;
                //if(penalizeGettingToPoints && !allClose)
                //    temp/=100;
                //if(penalizeSignalResponse && !moved)
                //    temp/=10;
                fitness+= temp;
               
                //Console.WriteLine(gotoList[i] + " " + endList[i]);
                go_sum *= (gotoList[i] + 0.01);
                ret_sum *= (endList[i] + 0.01);
                obj[i * 2] = 0; //gotoList[i];
                obj[i * 2 + 1] = 0; //endList[i];
                collide_count += ip.robots[i].collisions; //sensorList[i];//engine.robots[i].collisions;
            }

            obj[0] = go_sum;
            obj[1] = ret_sum;
            obj[2] = -collide_count;
			// Schrum: This 100 point fitness bonus shouldn't be here
            //if(solve) fitness+=100.0;
			return fitness;
        }

        void IFitnessFunction.update(SimulatorExperiment engine, Environment environment, instance_pack ip)
        {

            if (!(ip.timeSteps % (int)(1 / engine.timestep) == 0))
            {
                //grid.decay_viewed(0);
                return;
            }


            double[] gl = new double[3];
            double[] el = new double[3];
            double[] sense = new double[3];

            int r_ind = 0;
            if (!first && (ip.timeSteps * engine.timestep) > engine.evaluationTime / 2.0)
            {
                allCorrected = true;
                bool[] close = new bool[3];
                // Schrum: Brains don't get switched with preference neurons ... all need to be evaluated
                // Schrum: Checking that numBrains == 2 assures that this mode of switching does not occur in FourTask experiments with 5 brains
                if (!ip.agentBrain.preferenceNeurons && engine.numBrains == 2) ip.agentBrain.switchBrains();
                foreach (Robot r in ip.robots)
                {
                    //Schrum: Debugging
                    //Console.WriteLine("Robot id: " + r.id + ", " + r.name);
                    //Console.WriteLine("r.sensors.Count=" + r.sensors.Count);
                    //Console.WriteLine("Last sensor type: " + r.sensors[r.sensors.Count - 1].GetType());

                    if (!ip.agentBrain.multipleBrains || // Schrum: Original condition
                        (r.sensors[r.sensors.Count - 1] is SignalSensor)) // Schrum: Broader condition that also works with pref neurons
                    {
                        //Schrum: Debugging
                        //Console.WriteLine("Switched signal at " + (ip.timeSteps * engine.timestep));

                        SignalSensor s = (SignalSensor)r.sensors[r.sensors.Count - 1];
                        s.setSignal(1.0);
                    }
                    origDist[r_ind] = r.location.distance(environment.goal_point);
                    origHeadings[r_ind] = r.heading;

                    //checks to see if all points have an agent close to them when the signal fires
                    for (int p = 0; p < environment.POIPosition.Count; p++)
                        if (r.location.manhattenDistance(new Point2D(environment.POIPosition[p].X, environment.POIPosition[p].Y)) < 15)
                            close[p] = true;

                    //checks to see if agents are being corrected (stopped) when the signal fires
                    if (!r.corrected)
                        allCorrected = false;

                    r_ind++;
                }
                r_ind = 0;
                first = true;
                allClose = close[0] && close[1] && close[2];
                    
            }



            foreach (Robot r in ip.robots)
            {
                int p_ind = 0;
                double d2 = r.location.manhattenDistance(environment.goal_point);
                if (first)
                {

                    if (!turned[r_ind])
                        if (origHeadings[r_ind] != r.heading)
                            turned[r_ind] = true;
                    //if(point.distance(environment.goal_point) <25.0) {
                    //      endList[i]=1.5;
                    //}
                    //else {
                    if (d2 <= 20) {
                        el[r_ind] = 1;
						reachGoal[r_ind]=true;
					}
					
                    el[r_ind] = Math.Max(0.0, (origDist[r_ind] - d2) / 167.0);


                    //}
                }
                /*
                else {
                        System.Drawing.Point
p=environment.POIPosition[r_ind];
                        Point2D point= new
Point2D((double)p.X,(double)p.Y);
                        double d1=point.distance(r.location);
                        gl[r_ind]=Math.Max(0.0,(200.0-d1)/200.0);
                }
                */

                foreach (System.Drawing.Point p in environment.POIPosition)
                {
                    Point2D point = new Point2D((double)p.X, (double)p.Y);
                    int i = p_ind;
                    double d1 = point.manhattenDistance(r.location);

                    if (!first)
                    {
                        // Schrum: Magic numbers everywhere! I think robot has reached the POI if within 10 units
                        if (d1 <= 10)
                            gl[i] = 1;
                        else
                            // Otherwise, add (D - d)/D where D = 110 and d = d1 = distance from POI
                            gl[i] = Math.Max(gl[i], (110.0 - d1) / 110.0);
                    }

                    p_ind += 1;
                }

                sense[r_ind] = 1;
                foreach (ISensor s in r.sensors)
                {
                    if (s is RangeFinder)
                        if (s.get_value() < sense[r_ind])
                            sense[r_ind] = s.get_value();
                }


                r_ind += 1;


            }



            for (int i = 0; i < 3; i++)
            {
                gotoList[i] += gl[i];
                endList[i] += el[i];
                sensorList[i] += sense[i];
            }

            return;
        }
        void IFitnessFunction.reset()
        {

            reset0();
        }
        #endregion
    }
}
