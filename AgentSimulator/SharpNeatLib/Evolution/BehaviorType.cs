// BehaviorType.cs created with MonoDevelop
// User: joel at 1:44 PM 8/5/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
namespace SharpNeatLib
{
    public class BehaviorType
    {        
        public List<double> behaviorList;
		public double[] objectives;

        // Schrum: These three fields are not related to "behavior" but
        // this class is the most convenient place to store this information
        public int modules;
        public int cppnLinks;
        public int substrateLinks;
        
        public BehaviorType()
        {
         
        }
        public BehaviorType(BehaviorType copyFrom)
        {
            if(copyFrom.behaviorList!=null)
            behaviorList = new List<double>(copyFrom.behaviorList);
        }
    }
    
    public static class BehaviorDistance
    {
        public static double Distance(BehaviorType x, BehaviorType y)
        {
           double dist = 0.0;
           for(int k=0;k<x.behaviorList.Count;k++)
           {
            double delta = x.behaviorList[k]-y.behaviorList[k]; 
            dist += delta*delta;
           }
           return dist;
        }
    }
}
