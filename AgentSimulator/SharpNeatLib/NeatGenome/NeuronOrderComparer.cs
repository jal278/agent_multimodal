using System;
using System.Collections.Generic;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.NeatGenome
{
    /// <summary>
    /// By Schrum:
    /// 
    /// Compares type of gene first, ordering the nodes
    /// as bias, inputs, outputs, then hidden. Ties are broken
    /// based on innovation number.
    /// </summary>
    public class NeuronOrderComparer : IComparer<NeuronGene>
    {
        #region IComparer<NeuronGene> Members

        public int Compare(NeuronGene x, NeuronGene y)
        {
            if (typeToValue(x) < typeToValue(y))
                return -1;
            else if (typeToValue(x) > typeToValue(y))
                return 1;
            else // If types are the same, focus on the innovation number
            { 
                if (x.InnovationId < y.InnovationId)
                    return -1;
                else if (x.InnovationId > y.InnovationId)
                    return 1;
                else
                    return 0;
            }
        }

        public static int typeToValue(NeuronGene x)
        {
            // Genomes but bias first, then inputs, then outputs, then hidden neurons
            if (x.NeuronType == NeuronType.Bias)
                return 0;
            else if (x.NeuronType == NeuronType.Input)
                return 1;
            else if (x.NeuronType == NeuronType.Output)
                return 2;
            else if (x.NeuronType == NeuronType.Hidden)
                return 3;
            else // Should not happen
                return 4;
        }

        #endregion
    }
}
