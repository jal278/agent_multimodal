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

            // Schrum: a hack. Currently, we know there will only be two objectives.
            // So, just check the second. Make sure that second is tie-breaker when first
            // objective is the same, since we assume the first objective is the only
            // "real" objective. Second objective could be negative CPPN link count or
            // negative substrate link count.
            if (x.Behavior.multiobjective && x.Behavior.objectives != null && y.Behavior.objectives != null)
            {
                // Second objective in index 1
                double secondObjectiveDelta = y.Behavior.objectives[1] - x.Behavior.objectives[1];
                if (secondObjectiveDelta < 0.0D)
                    return -1;
                else if (secondObjectiveDelta > 0.0D)
                    return 1;
            }

            // Schrum: Not sure if comparing based on age is really a good idea.
            //         Wonder why this is here.
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
