using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.Experiments;
using Engine;
using System.IO;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.Evolution;
using System.Xml;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;

namespace Engine
{
    public class HyperNEATEvolver
    {
        public IExperiment experiment;
        SimulatorExperiment simExperiment;
        double maxFitness = 0;
        StreamWriter logOutput;
        string outputFolder = "";
        EvolutionAlgorithm ea = null;
        XmlDocument doc;
        FileInfo oFileInfo;

        public bool logging = true;
        //public bool timeEachGeneration = true;

        public HyperNEATEvolver(SimulatorExperiment simExp)
        {
            simExperiment = simExp;
            experiment = new Experiment(simExperiment);	
        }
		
		public void enableNoveltySearch(bool enable) {
			if(enable) {
			experiment.DefaultNeatParameters.noveltySearch=true;
			experiment.DefaultNeatParameters.noveltyFloat=true;
			}
			else {
			experiment.DefaultNeatParameters.noveltySearch=false;
			experiment.DefaultNeatParameters.noveltyFloat=false;
			}
		}
        public void initializeEvolution(int populationSize)
        {
            logOutput = new StreamWriter(outputFolder + "logfile.txt");
            IdGenerator idgen = new IdGenerator();
            ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(experiment.DefaultNeatParameters, idgen, experiment.InputNeuronCount, experiment.OutputNeuronCount, experiment.OutputsPerPolicy, experiment.DefaultNeatParameters.pInitialPopulationInterconnections, populationSize)), experiment.PopulationEvaluator, experiment.DefaultNeatParameters);
        }

        public void initializeEvolution(int populationSize, NeatGenome seedGenome)
        {
            if (seedGenome == null)
            {
                initializeEvolution(populationSize);
                return;
            }
            logOutput = new StreamWriter(outputFolder + "logfile.txt");
            IdGenerator idgen = new IdGeneratorFactory().CreateIdGenerator(seedGenome);
            ea = new EvolutionAlgorithm(new Population(idgen, GenomeFactory.CreateGenomeList(seedGenome, populationSize, experiment.DefaultNeatParameters, idgen)), experiment.PopulationEvaluator, experiment.DefaultNeatParameters);
        }
		public void initializeEvolutionFromPopFile(string fname) {
			XmlDocument doc = new XmlDocument();
            doc.Load(fname);
			Population pop = XmlPopulationReader.Read(doc,new XmlNeatGenomeReader(),new SharpNeatLib.NeatGenome.IdGeneratorFactory());
			initalizeEvolution(pop);
		}
		
        public void initalizeEvolution(Population pop)
        {
            logOutput = new StreamWriter(outputFolder + "logfile.txt");
            //IdGenerator idgen = new IdGeneratorFactory().CreateIdGenerator(pop.GenomeList);
            ea = new EvolutionAlgorithm(pop, experiment.PopulationEvaluator, experiment.DefaultNeatParameters);
        }

        public void setOutputFolder(string folder)
        {
            outputFolder = folder;
        }

        public void oneGeneration(int currentGeneration)
        {
            DateTime dt = DateTime.Now;
            ea.PerformOneGeneration();
            if (ea.BestGenome.RealFitness > maxFitness)
            {
                simExperiment.bestGenomeSoFar = (NeatGenome)ea.BestGenome;
                maxFitness = ea.BestGenome.RealFitness;
                doc = new XmlDocument();
                XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome);
                oFileInfo = new FileInfo(outputFolder + "bestGenome" + currentGeneration.ToString() + ".xml");
                doc.Save(oFileInfo.FullName);
            }
            //Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.RealFitness + " "  + ea.Population.GenomeList.Count + " " + (DateTime.Now.Subtract(dt)));
            // Schrum: Changed this to include fitness values from each environment: Mainly for FourTasks
            Console.WriteLine(ea.Generation.ToString() + " " + ea.BestGenome.RealFitness + " " + ea.Population.GenomeList.Count + " (" + string.Join(",", ea.BestGenome.Behavior.objectives) + ") " + (DateTime.Now.Subtract(dt)) + " ID:" + ea.BestGenome.GenomeId + " " + ea.BestGenome.Behavior.modules + " " + ea.BestGenome.Behavior.cppnLinks + " " + ea.BestGenome.Behavior.substrateLinks);
            int gen_mult = 200;
            if (logging)
            {
				
                if (experiment.DefaultNeatParameters.noveltySearch && currentGeneration % gen_mult == 0)
                {
                    XmlDocument archiveout = new XmlDocument();

                    XmlPopulationWriter.WriteGenomeList(archiveout, ea.noveltyFixed.archive);
                    oFileInfo = new FileInfo(outputFolder + "archive.xml");
                    archiveout.Save(oFileInfo.FullName);
                }

                if ((experiment.DefaultNeatParameters.noveltySearch||experiment.DefaultNeatParameters.multiobjective) && currentGeneration % gen_mult == 0)
                {
                    XmlDocument popout = new XmlDocument();
                    if(!experiment.DefaultNeatParameters.multiobjective)
					XmlPopulationWriter.Write(popout, ea.Population, ActivationFunctionFactory.GetActivationFunction("NullFn"));
                    else 
					XmlPopulationWriter.WriteGenomeList(popout, ea.multiobjective.population);
                    
					oFileInfo = new FileInfo(outputFolder + "population" + currentGeneration.ToString() + ".xml");
                    popout.Save(oFileInfo.FullName);
                }
                // Schrum: Added contents of objective array to log so individual environment scores can be seen in FourTasks domain
                // Also always print modules, cppn links, and substrate links
                logOutput.WriteLine(ea.Generation.ToString() + " " + (maxFitness).ToString() + " " + string.Join(" ", ea.BestGenome.Behavior.objectives) + " " + ea.BestGenome.Behavior.modules + " " + ea.BestGenome.Behavior.cppnLinks + " " + ea.BestGenome.Behavior.substrateLinks);
            }
        }

        public void evolve(int generations)
        {
            for (int j = 0; j < generations; j++)
            {
                oneGeneration(j);
            }
            logOutput.Close();

            doc = new XmlDocument();
            XmlGenomeWriterStatic.Write(doc, (NeatGenome)ea.BestGenome, ActivationFunctionFactory.GetActivationFunction("NullFn"));
            oFileInfo = new FileInfo(outputFolder + "bestGenome.xml");
            doc.Save(oFileInfo.FullName);

            //if doing novelty search, write out archive
            if (experiment.DefaultNeatParameters.noveltySearch)
            {
                XmlDocument archiveout = new XmlDocument();

                XmlPopulationWriter.WriteGenomeList(archiveout, ea.noveltyFixed.archive);
                oFileInfo = new FileInfo(outputFolder + "archive.xml");
                archiveout.Save(oFileInfo.FullName);
            }
        }

        //Do any cleanup here
        public void end()
        {
            if (logging)
                logOutput.Close();
        }
    }
}
