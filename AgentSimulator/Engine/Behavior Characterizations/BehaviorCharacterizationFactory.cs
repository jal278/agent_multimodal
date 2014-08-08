using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Engine
{
    public static class BehaviorCharacterizationFactory
    {
        public static System.Collections.Generic.Dictionary<string, IBehaviorCharacterization> behaviorCharacterizationTable=new Dictionary<string,IBehaviorCharacterization>();

        public static IBehaviorCharacterization getBehaviorCharacterization(string functionName)
        {
            IBehaviorCharacterization behaviorCharacterization;
            /*
			if(behaviorCharacterizationTable.ContainsKey(functionName))
                behaviorCharacterization = behaviorCharacterizationTable[functionName];
            else
            {
            */
                behaviorCharacterization = createBehaviorCharacterization(functionName);

                if (behaviorCharacterization == null)
                    return null;
/*
                try
                {
                    behaviorCharacterizationTable.Add(functionName, behaviorCharacterization);
                }
                catch (Exception e)
                {
                    ;
                }
            }
*/
                        return behaviorCharacterization;
        }

        private static IBehaviorCharacterization createBehaviorCharacterization(string functionName)
        {
            string className = typeof(BehaviorCharacterizationFactory).Namespace + '.' + functionName;
            return (IBehaviorCharacterization)Assembly.GetExecutingAssembly().CreateInstance(className);
        }
    }
}
