using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Engine
{
    public static class FitnessFunctionFactory
    {
        public static System.Collections.Generic.Dictionary<string, IFitnessFunction> fitnessFunctionTable=new Dictionary<string,IFitnessFunction>();

        public static IFitnessFunction getFitnessFunction(string functionName)
        {
            IFitnessFunction fitnessFunction;
            /*if(fitnessFunctionTable.ContainsKey(functionName))
                fitnessFunction = fitnessFunctionTable[functionName];
            else
            {*/
                fitnessFunction = createFitnessFuction(functionName);

                if (fitnessFunction == null)
                    return null;

              /*  try
                {
                    fitnessFunctionTable.Add(functionName, fitnessFunction);
                }
                catch (Exception e)
                {
                    ;
                }
            }*/
            return fitnessFunction;
        }

        private static IFitnessFunction createFitnessFuction(string functionName)
        {
            string className = typeof(FitnessFunctionFactory).Namespace + '.' + functionName;
            return (IFitnessFunction)Assembly.GetExecutingAssembly().CreateInstance(className);
        }
    }
}
