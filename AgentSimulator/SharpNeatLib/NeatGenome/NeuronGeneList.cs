using System;
using System.Collections.Generic;
using SharpNeatLib.NeuralNetwork;


namespace SharpNeatLib.NeatGenome
{
	public class NeuronGeneList : List<NeuronGene>
	{
		static NeuronGeneComparer neuronGeneComparer = new NeuronGeneComparer();
		public bool OrderInvalidated=false;
        // Schrum: Added to put new output neurons back in order when needed
        static NeuronOrderComparer neuronOrderComparer = new NeuronOrderComparer();

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public NeuronGeneList()
		{}

        public NeuronGeneList(int count)
        {
            Capacity = (int)(count*1.5);
        }

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="copyFrom"></param>
		public NeuronGeneList(NeuronGeneList copyFrom)
		{
			int count = copyFrom.Count;
			Capacity = count;
			
			for(int i=0; i<count; i++)
				Add(new NeuronGene(copyFrom[i]));

            // Schrum: Added to assure that this is the starting point
            SortByInnovationId();

//			foreach(NeuronGene neuronGene in copyFrom)
//				InnerList.Add(new NeuronGene(neuronGene));
		}

		#endregion

		#region Public Methods


		new public void Remove(NeuronGene neuronGene)
		{
			Remove(neuronGene.InnovationId);

			// This invokes a linear search. Invoke our binary search instead.
			//InnerList.Remove(neuronGene);
		}

		public void Remove(uint neuronId)
		{
			int idx = BinarySearch(neuronId);
			if(idx<0)
				throw new ApplicationException("Attempt to remove neuron with an unknown neuronId");
			else
				RemoveAt(idx);

//			// Inefficient scan through the neuron list.
//			// TODO: Implement a binary search method for NeuronList (Will generics resolve this problem anyway?).
//			int bound = List.Count;
//			for(int i=0; i<bound; i++)
//			{
//				if(((NeuronGene)List[i]).InnovationId == neuronId)
//				{
//					InnerList.RemoveAt(i);
//					return;
//				}
//			}
//			throw new ApplicationException("Attempt to remove neuron with an unknown neuronId");
		}

		public NeuronGene GetNeuronById(uint neuronId)
		{
			int idx = BinarySearch(neuronId);
			if(idx<0)
				return null;
			else
				return this[idx];

//			// Inefficient scan through the neuron list.
//			// TODO: Implement a binary search method for NeuronList (Will generics resolve this problem anyway?).
//			int bound = List.Count;
//			for(int i=0; i<bound; i++)
//			{
//				if(((NeuronGene)List[i]).InnovationId == neuronId)
//					return (NeuronGene)List[i];
//			}
//
//			// Not found;
//			return null;
		}

		public void SortByInnovationId()
		{
			Sort(neuronGeneComparer);
			OrderInvalidated=false;
		}

        // Schrum: Added to (ultimately) allow Module Mutation
        public void SortByNeuronOrder()
        {
            Sort(neuronOrderComparer);
            OrderInvalidated = true; // Innovation numbers (potentially) no longer ordered
        }

        // Schrum: (not sure it is safe to only do this sometimes)
        // Sorts genes by innovation number, but only if necessary
        public void InnovationSortCheck()
        {
            if (OrderInvalidated)
            {
                SortByInnovationId();
            }

            // Schrum: Debugging
            /*
            if (!IsInovationSorted())
            {
                Console.WriteLine("Not innovation sorted");
                Console.WriteLine("OrderInvalidated:" + OrderInvalidated);
                // Schrum
                foreach (NeuronGene gene in this)
                {
                    Console.Write(gene.NeuronType + ":" + gene.InnovationId + ", ");
                }
                Console.WriteLine();
                // Schrum: Break the code now
                int[] a = new int[1];
                a[5]++; // Will crash with exception
            }
            */
        }

        // Schrum: Sorts by neuron order, if necessary
        public void NeuronSortCheck()
        {
            // Schrum: Debugging
            //bool needSort = !OrderInvalidated;

            // Schrum: Assumes the only "invalid" order is from a neuron order sort
            //if (!OrderInvalidated)
            
            // Schrum: It is perhaps inefficient to perform this sort every time,
            // but at the moment it is the easiest approach because I don't know
            // why it sometimes becomes unsorted. This check will assure that it
            // never breaks before being used.
            SortByNeuronOrder();
            

            // Schrum: Make sure the sort happened or wasn't needed
            // Debugging
            /*
            if(!IsNeuronSorted()) {
                Console.WriteLine("Not neuron sorted");
                Console.WriteLine("OrderInvalidated:" + OrderInvalidated);
                // Schrum
                foreach (NeuronGene gene in this)
                {
                    Console.Write(gene.NeuronType+":"+gene.InnovationId+", ");
                }
                Console.WriteLine();
                // Schrum: Break the code now
                int[] a = new int[1];
                a[5]++; // Will crash with exception
            }
            */
        }

        // Schrum: For debugging
        // Tells if neurons are in order of bias, input, output, hidden
        /*
        public bool IsNeuronSorted()
        {
            NeuronType expectedType = NeuronType.Bias;
            foreach (NeuronGene gene in this)
            {
                if (expectedType == NeuronType.Bias && gene.NeuronType == NeuronType.Input)
                {
                    expectedType = NeuronType.Input;
                }
                else if (expectedType == NeuronType.Input && gene.NeuronType == NeuronType.Output)
                {
                    expectedType = NeuronType.Output;
                }
                else if (expectedType == NeuronType.Output && gene.NeuronType == NeuronType.Hidden)
                {
                    expectedType = NeuronType.Hidden;
                }
                else if (expectedType != gene.NeuronType)
                {
                    return false;
                }
            }
            return true;
        }
        */

		public int BinarySearch(uint innovationId) 
		{
            // Schrum: Added this check in anticipation of changes I will make
            // that mess up node gene order. I think that it will be safe to leave
            // the list out of order as long as proper checks and corresponding sorts
            // are performed just in time as needed.
            InnovationSortCheck();

			int lo = 0;
			int hi = Count-1;

			while (lo <= hi) 
			{
				int i = (lo + hi) >> 1;

				if(this[i].InnovationId<innovationId)
					lo = i + 1;
				else if(this[i].InnovationId>innovationId)
					hi = i - 1;
				else
					return i;


				// TODO: This is wrong. It will fail for large innovation numbers because they are of type uint.
				// Fortunately it's very unlikely anyone has reached such large numbers!
//				int c = (int)((NeuronGene)InnerList[i]).InnovationId - (int)innovationId;
//				if (c == 0) return i;
//
//				if (c < 0) 
//					lo = i + 1;
//				else 
//					hi = i - 1;
			}
			
			return ~lo;
		}

		// For debug purposes only.
		/*
        public bool IsInovationSorted()
		{
			uint prevId=0;
			foreach(NeuronGene gene in this)
			{
				if(gene.InnovationId<prevId)
					return false;
				prevId = gene.InnovationId;
			}
			return true;
		}
        */

		#endregion
	}
}
