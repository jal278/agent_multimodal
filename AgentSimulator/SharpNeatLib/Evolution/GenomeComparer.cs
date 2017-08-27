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

            // ALSO: The Fitness has different values during multiobjective population sorting
            // (for example, "rank") so I also make sure that the Fitness equals the value of the
            // first objective before using the second objective as a tie-breaker.
            if (x.Behavior.multiobjective && // multiobjective evolution (only makes sense for 2 objectives where the second is a meta-objective)
                x.Fitness != 0 && // 0 fitness means the genome was just initialized, and hasn't been evaluated yet
                x.Behavior.objectives != null && y.Behavior.objectives != null && // Make sure objectives have been provided
                x.Fitness == x.Behavior.objectives[0]) // Make sure Fitness hasn't been swapped with rank during Pareto layer sorting
            {
                // Schrum:
                // Apparently, genomes with fitness 0 get "sorted" after creation, but before evaluation.
                // In this case, 0 is the worst possible fitness, but not Behavior or objectives will be defined.
                // However, at this point in the code, if one fitness is 0, then both must be, meaning we can skip
                // this code and rely on age, as below.

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
