using System;
using System.Collections.Generic;
using SharpNeatLib.Evolution;

namespace SharpNeatLib.Evolution
{
	/// <summary>
	/// Sort by Fitness(Descending). Genomes with like fitness are then sorted by genome size(Ascending).
	/// This means the selection routines are more liley to select the fit AND the smallest first.
	/// </summary>
	public class GenomeComparer : IComparer<IGenome>
	{

        #region IComparer<IGenome> Members

        public int Compare(IGenome x, IGenome y)
        {
            double fitnessDelta = y.Fitness - x.Fitness;
            if (fitnessDelta < 0.0D)
                return -1;
            else if (fitnessDelta > 0.0D)
                return 1;

            // Schrum: for multiobjective comparison with one "real" objective and a secondary objective.
            //         First objective is still most important, but second objective breaks ties.
            //         Also, only apply when the Fitness is not the "rank" for multiobjective sorting
            //         (Fitness is not always the RealFitness when this comparer is used)
            if(x.objectives != null && y.objectives != null && // multiple objectives exist
               x.objectives[0] == x.Fitness && y.objectives[0] == y.Fitness) // Currently sorting by real fitness and not rank
            {
                double secondFitnessDelta = y.objectives[1] - x.objectives[1];
                if (secondFitnessDelta < 0.0D)
                    return -1;
                else if (secondFitnessDelta > 0.0D)
                    return 1;
            }

            // FIX: This section below seems to cause problems. It seems to prevent new genomes from ever having a chance.

            // Schrum: Trust genomes that have experienced an evaluation over others.
            //         Might contradict the original intention of the age comparison below.
            if (x.objectives == null && y.objectives != null)
                return 1; // y is "better" since it has been evaluated, but x has not
            else if (x.objectives != null && y.objectives == null)
                return -1; // x is "better" since it has been evaluated, but y has not

            long ageDelta = x.GenomeAge - y.GenomeAge;

            // Convert result to an int.
            if (ageDelta < 0)
                return -1;
            else if (ageDelta > 0)
                return 1;

            return 0;
        }

        #endregion
    }
}
