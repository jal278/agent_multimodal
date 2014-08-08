using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using System.IO;

namespace Engine
{
    public class ExperimentWrapper
    {
        [
            System.Xml.Serialization.XmlElement(typeof(SimulatorExperiment)),
            System.Xml.Serialization.XmlElement(typeof(MultiAgentExperiment)),
        System.Xml.Serialization.XmlElement(typeof(RoomClearingExperiment))
        ] 

        public SimulatorExperiment experiment;

        public ExperimentWrapper()
        {
        }

        public virtual void save(string name)
        {//typeof(SimulatorExperiment
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());
            TextWriter outfile = new StreamWriter(name);
            x.Serialize(outfile, this);
            outfile.Close();
        }

        //Loads a environment from an XML file and initializes it
        public static ExperimentWrapper load(string name)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ExperimentWrapper));
            TextReader infile = new StreamReader(name);
            ExperimentWrapper e = (ExperimentWrapper)x.Deserialize(infile);
            infile.Close();

            //TODO include LEO

            //Determine the number of CPPN inputs and outputs automatically
            //if (e.experiment.homogeneousTeam)
            //    e.experiment.inputs = 4;
            //else
            //    e.experiment.inputs = 5;

            //if (e.experiment.adaptableANN)
            //{
            //    if (e.experiment.modulatoryANN) e.experiment.outputs = 8; else e.experiment.outputs = 7;
            //}
            //else
            //    e.experiment.outputs = 2;

            //TODO maybe include			e.experiment.initialize();
            return e;
        }
    }
}
