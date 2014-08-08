using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Engine
{
    public static class RobotModelFactory
    {
       // public static System.Collections.Generic.Dictionary<string, IFitnessFunction> fitnessFunctionTable = new Dictionary<string, IFitnessFunction>();

        public static Robot getRobotModel(string robotModelname)
        {
            Robot robotModel;

            robotModel = createRobotModel(robotModelname);

            return robotModel;
        }

        private static Robot createRobotModel(string robotModelname)
        {
            string className = typeof(RobotModelFactory).Namespace + '.' + robotModelname;
            return (Robot)Assembly.GetExecutingAssembly().CreateInstance(className);
        }
    }
}
