using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;

using SharpNeatLib;
using SharpNeatLib.AppConfig;
using SharpNeatLib.Evolution;
using SharpNeatLib.Evolution.Xml;
using SharpNeatLib.Experiments;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib.NeuralNetwork.Xml;
using SharpNeatLib.CPPNs;
using System.Text;
using System.Net.Sockets;
using Engine.Forms;
using simulator.Forms;

namespace Engine
{
    //Main Simulator Form
    public partial class SimulatorVisualizer : Form
	{
		public bool robot_display_debug=false;
	    public int frameskip = 1;
        //The different draw modes
        public enum drawModes { wallMode, startMode, goalMode, selectMode, POIMode, AOIMode };
        public enum selectModes { dragMode, rotateMode, scaleMode };

        //Coordinate frame used for drawing
        private CoordinateFrame frame;
        private NeatGenome evolveGenome;

        //The simulator experiment
        public SimulatorExperiment experiment;
        //Experiment variable used to run evolution
        public SimulatorExperiment evolutionExperiment;

        //Form to visualize the CPPN
        private GenomeVisualizerForm genomeVisualizerForm;

        #region Evolution Threading Properties
        
        volatile bool bEvolve;
        Thread evolveThread;
        Mutex evolveMutex = new Mutex();
        
        #endregion

        #region Gui Editing Properties

        //do we label the robots?
        bool bDrawLabel;
        bool bDrawFOV;
       
        //x1,y1,x2,y2 are selected coordinates for a new wall
        public int x1, y1, x2, y2;

        //snapx and snapy are for recording snap-to points
        public int snapx, snapy;

        //should we display the temporary wall under construction?
        public bool display_tempwall;

        //Should the Area of Interest be displayed
        private bool displayAOIRectangle;

        //is there a snap point that needs to be displayed
        public bool display_snap;

        //what is our current drawing mode, e.g. drawing a wall, setting start point etc.
        public drawModes drawMode;
        public selectModes selectMode;

        //is a wall selected?
		public bool bDragDisplay;
        public bool bSelected_wall;
        //what is the selected wall?
        public Wall selected_wall;
        //what POI is selected
        public int selected_POI;

        //is a robot selected?
        public bool bSelected_robot;
        //what is the selected robot?
        public Robot selected_robot;

        #endregion

        public bool displayEvolution;
        public bool noveltySearch = false;

        public SimulatorVisualizer(string experimentName,string genome)
        {
            frame = new CoordinateFrame(0.0f, 0.0f, 5.0f, 0.0f);

			ExperimentWrapper wr = ExperimentWrapper.load(experimentName);

            experiment = wr.experiment;
			if(genome!=null) {
			 experiment.loadGenome(genome);
			}
			
            experiment.initialize();
			
			
			frame.sync_from_environment(experiment.environment);

            selected_POI = -1;
            bDrawFOV = false;
            bDrawFOV = false;
            bDrawLabel = true;
            displayAOIRectangle = false;
            displayEvolution = true;

            bEvolve = false;                    //by default we are not evolving, just displaying the environment
            drawMode = drawModes.selectMode;    //default mode is selecting and moving walls
            selectMode = selectModes.dragMode;

            display_tempwall = false;           //no temporary wall exists at creation
            display_snap = false;               //we have nothing to snap to at creation

            InitializeComponent();

            this.Text = "MultiAgent-HyperSharpNEAT Simulator - " + experimentName;
            //set up double buffering
            this.SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);
        }

        #region Environment Handling

        private void saveEnvironmentClick(object sender, EventArgs e)
        {
            //for(int j=0;j<experiment.environment.walls.Count;j++)
            //    experiment.environment.walls[j] = new Wall(experiment.environment.walls[j].line.p1.x / 2.0f, experiment.environment.walls[j].line.p1.y / 2.0f, experiment.environment.walls[j].line.p2.x / 2.0f, experiment.environment.walls[j].line.p2.y / 2.0f, experiment.environment.walls[j].visible, experiment.environment.walls[j].name);

            fileSaveDialog.Title = "Save Environment";
            DialogResult res = fileSaveDialog.ShowDialog(this);
            string filename = fileSaveDialog.FileName;
			frame.sync_to_environment(experiment.environment);
            if (res == DialogResult.OK || res == DialogResult.Yes)
                experiment.environment.save(filename);
        }

        private void clearEnvironmentClick(object sender, EventArgs e)
        {
            experiment.clearEnvironment();
            Invalidate();
        }

        private void loadEnvironmentClick(object sender, EventArgs e)
        {
            fileOpenDialog.Title = "Load Environment";
            DialogResult res = fileOpenDialog.ShowDialog(this);
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                string filename = fileOpenDialog.FileName;
                Console.WriteLine("Load environment " + filename);
                try
                {
                    experiment.loadEnvironment(filename);

                    // Schrum: FourTasks environments need to set up fitness function for each environment
                    specialHandlingForFourTasks();

                    experiment.initialize();

                    // Schrum: must repeat this after the initialize because it resets the fitness from a factory.
                    // Also had to do it before as well to assure correct variable values during the initialize.
                    specialHandlingForFourTasks();

					frame.sync_from_environment(experiment.environment);
                    Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            //!environmentList.Add(scene);
        }

        #endregion

        private void specialHandlingForFourTasks()
        {
            if (experiment.fitnessFunction is FourTasksFitness)
            {
                ((FourTasksFitness)experiment.fitnessFunction).setExperiment((MultiAgentExperiment)experiment);
                ((FourTasksFitness)experiment.fitnessFunction).setupFitness(FourTasksFitness.environmentID(experiment.environmentName));
            }
        }

        private void clocktick(object sender, EventArgs e)
        {
            if (experiment.running)
            {
                for (int i = 0; i < frameskip; i++)
                {
                    experiment.run();
                    // Schrum: Added automatic pause/stop when time runs out, to get
                    // a better idea of what happens in actual evaluations.
                    if(experiment.elapsed >= experiment.evaluationTime) 
                    {
                        Console.WriteLine("Paused due to time expiring");
                        // Schrum: Are these the right params to send?
                        this.pauseButton_Click(sender, e);
                    }
                }
                Invalidate();
            }
        }

        private void paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(EngineUtilities.backGroundColorBrush, 0, 0, Width, Height);

			
			if(experiment!=null) {
				foreach(Robot r in experiment.robots)
					r.display_debug=robot_display_debug;
	            experiment.draw(e.Graphics, frame);
			}
				
            if (display_snap)
            {
                Rectangle snaprect = new Rectangle((int)(snapx - 3), (int)(snapy - 3), 6, 6);
                e.Graphics.DrawEllipse(EngineUtilities.YellowPen, snaprect);
            }

            if (display_tempwall)
            {
                PointF a = new PointF((float)x1, (float)y1);
                PointF b = new PointF((float)x2, (float)y2);
                e.Graphics.DrawLine(EngineUtilities.RedPen, a, b);
            }

            if (drawMode == drawModes.selectMode && selected_robot != null)
            {
                e.Graphics.DrawString("Z Coordinate: " + selected_robot.zstack.ToString(), new Font("Tahoma", 15), Brushes.Black, 150, 65);
            }
        }

        #region Event Handling

        //handles key down event...e.g. control keys like delete
        private void KeyDownHandler(object sender, System.Windows.Forms.KeyEventArgs e)
        {
			/*if (this.experiment.robots!=null)
				if(this.experiment.robots.Count>0) {
					if(this.experiment.robots[0] is Khepera3RobotModelJoy) { 
					Khepera3RobotModelJoy k = (Khepera3RobotModelJoy)this.experiment.robots[0];
					if(e.KeyCode ==Keys.Z) k.SetCommand(1);
					if(e.KeyCode ==Keys.X) k.SetCommand(0);
					if(e.KeyCode ==Keys.C) k.SetCommand(2);
					if(e.KeyCode ==Keys.Space) k.SetCommand(3);
					
					}
					
				}*/
            if (drawMode == drawModes.selectMode)
            {
                if (e.KeyCode == Keys.W)
                {
                    if (selected_wall != null)
                    {
                        Wall thewall = selected_wall;
                        //InputBoxSample.InputBoxResult result;

                        ObjectRenamer or = new ObjectRenamer(selected_wall.name);
                        DialogResult result = or.ShowDialog(this);

                        //result = InputBoxSample.InputBox.Show("Name wall", "Name wall", selected_wall.name, null);

                        if (result == DialogResult.OK)
                        {
                            //System.Console.WriteLine(selected_wall.name);
                            thewall.name = or.textBox1.Text;
                        }
                    }
                }
				if (e.KeyCode == Keys.D) 
					robot_display_debug=!robot_display_debug;
                if (e.KeyCode == Keys.V)
                {
                    if (selected_wall != null)
                    {
                        selected_wall.visible = !selected_wall.visible;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    if (selected_wall != null)
                        experiment.environment.walls.Remove(selected_wall);
                    selected_wall = null;

                    if (selected_POI != -1)
                    {
                        experiment.environment.POIPosition.RemoveAt(selected_POI);
                        selected_POI = -1;
                    }

                    display_snap = false;
                    Invalidate();
                }

            }
        }

        //handles key press event...e.g. normal keys (alphabetical etc.)
        private void KeyPressHandler(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 'p' || e.KeyChar == 'P')
			{
				       if (selected_wall != null) {
             				Console.WriteLine(selected_wall.line.p1.x+" "+selected_wall.line.p1.y);
							Console.WriteLine("to");
		          			Console.WriteLine(selected_wall.line.p2.x+" "+selected_wall.line.p2.y);
					}
			}
            if (e.KeyChar == 'W' || e.KeyChar == 'w')
            {
                wallAdder adder = new wallAdder();
                adder.Show(this);
            }

            if (e.KeyChar == 'R' || e.KeyChar == 'r')
            {
                experiment.running = !experiment.running;
                setRunningState();
            }
            if (e.KeyChar == 'L' || e.KeyChar == 'l')
                this.bDrawLabel = !this.bDrawLabel;

            if (e.KeyChar == 'N' || e.KeyChar == 'n')
            {
                if (drawMode == drawModes.selectMode && selected_robot != null)
                {
                    NetworkVisualizerForm netForm=null;
                    if (experiment is MultiAgentExperiment) //if this is a multiagentexperiment use agentBrain class
                    {
                        // Schrum: With preference neurons, all brains need to be displayed
                        if (experiment.preferenceNeurons)
                        {
                            for (int i = 0; i < experiment.agentBrain.numBrains; i++)
                            {
                                experiment.agentBrain.switchBrains(i); // Schrum: Select the right brain
                                INetwork brain = experiment.agentBrain.getBrain(selected_robot.id);
                                int brainCounter = experiment.agentBrain.getBrainCounter(); // Schrum: Added to know which brain is being used
                                bool checkZ = experiment.robots.Count > 1;
                                if (brain == null) return; //TODO maybe show dialog
                                netForm = new NetworkVisualizerForm(selected_robot, (ModularNetwork)brain, brainCounter, checkZ); // Schrum: Know which brain is being used
                                netForm.genomeVisualizerForm = genomeVisualizerForm;                               
                                netForm.Show();

                                // Schrum: Make layout/organization of multiple brains better
                                // These are custom layouts for different numbers of brains.
                                int hspacing;
                                int vspacing;

                                if (experiment.mmdRate > 0 || experiment.mmpRate > 0 || experiment.mmrRate > 0)
                                {
                                    if (experiment.agentBrain.numBrains > 10)
                                    {
                                        netForm.Width = 130;
                                        netForm.Height = 130;
                                        hspacing = -20;
                                        vspacing = +170;
                                    }
                                    else
                                    {
                                        netForm.Width = 170;
                                        netForm.Height = 170;
                                        hspacing = -20;
                                        vspacing = +40;
                                    }
                                }
                                else
                                { // Schrum: these settings work well for up to about 5 brains
                                    netForm.Width = 200;
                                    netForm.Height = 200;
                                    hspacing = -25;
                                    vspacing = +40;
                                }
                                netForm.SetDesktopLocation((i%10) * (netForm.Width + hspacing), selected_robot.id * (netForm.Height + vspacing) + (i >= 10 ? 120 : 0));
                            }
                            return; // Schrum: need to break normal execution flow to avoid netForm being shown again
                        }
                        else // Schrum: Original code
                        {
                            INetwork brain = experiment.agentBrain.getBrain(selected_robot.id);
                            int brainCounter = experiment.agentBrain.getBrainCounter(); // Schrum: Added to know which brain is being used
                            bool checkZ = experiment.robots.Count > 1;
                            if (brain == null)
                            {
                                Console.WriteLine("NULL Brain!");
                                return; //TODO maybe show dialog
                            }
                            netForm = new NetworkVisualizerForm(selected_robot, (ModularNetwork)brain, brainCounter,checkZ); // Schrum: Know which brain is being used
                        }
                    }
                    else 
                    {
                      //  TODO test with non MultiAgentExperiment
                        if (experiment.agentBrain.brain == null) return;
                        int brainCounter = experiment.agentBrain.getBrainCounter(); // Schrum: Added to know which brain is being used
                        bool checkZ = experiment.robots.Count > 1;
                        netForm = new NetworkVisualizerForm(selected_robot, (ModularNetwork)experiment.agentBrain.brain, brainCounter, checkZ); // Schrum: Know which brain is being used
                    }

                    netForm.genomeVisualizerForm = genomeVisualizerForm;
                    netForm.Show();
                  
                   // this.selected_robot.displayPath = !this.selected_robot.displayPath;
                }
            }

            // Schrum: Track mode/brain usage
            if (e.KeyChar == 'U' || e.KeyChar == 'u')
            {
                if (drawMode == drawModes.selectMode && selected_robot != null && selected_robot.brainHistory != null)
                {
                    INetwork brain = experiment.agentBrain.getBrain(selected_robot.id);
                    ModuleUsageVisualizerForm usageForm = new ModuleUsageVisualizerForm(selected_robot, (ModularNetwork)brain, experiment.evaluationTime);
                    usageForm.Show();

                    // Schrum: Custom displays for certain number of brains
                    usageForm.Width = experiment.agentBrain.numBrains * (200 - 25);
                }
            }

            if (e.KeyChar == '+' || e.KeyChar == '=')
            {
                animation.Interval += 5;
            }
            if (e.KeyChar == '-')
            {
                if (animation.Interval - 5 <= 0) {
                    animation.Interval = 1;
					frameskip++;
				}
                else
                    animation.Interval -= 5;
                //if (experiment.timestep <= 0.0f)
                //if(animation.Interval<=0)
				// animation.Interval = 3;
            }
            Invalidate();
        }

        private void toggleFOVClick(object sender, EventArgs e)
        {
            this.bDrawFOV = !this.bDrawFOV;
            this.displayFOVItem.Checked = this.bDrawFOV;
        }

        private void sensorSettingsClick(object sender, EventArgs e)
        {
            sensor_info subform = new sensor_info();

            int density;

           // if (experiment.overrideDefaultSensorDensity)
                density = experiment.rangefinderSensorDensity;
            //else if (experiment.robots != null && experiment.robots.Count > 0)
            //    density = experiment.robots[0].defaultSensorDensity();
            //else
            //    density = 1;

            subform.density.Text = Convert.ToString(density);

            //populate form		
            //subform.lower_range.Text = Convert.ToString(experiment.sensorStart * (180.0 / 3.14));
            //subform.upper_range.Text = Convert.ToString(experiment.sensorEnd * (180.0 / 3.14));
            subform.density.Text = Convert.ToString(experiment.rangefinderSensorDensity);
            subform.sensorNoiseTrackBar.Value = (int)experiment.sensorNoise;
            subform.effectorNoiseTrackBar.Value = (int)experiment.effectorNoise;
            subform.headingNoiseTrackBar.Value = (int)experiment.headingNoise;
            subform.setTextValues();
            subform.ShowDialog();

            //if ok is clicked
            if (subform.good)
            {
                //update environment with new information from form
                //double low_range = Convert.ToDouble(subform.lower_range.Text) / 180.0 * 3.14;
                //double high_range = Convert.ToDouble(subform.upper_range.Text) / 180.0 * 3.14;
                int newDensity = Convert.ToInt32(subform.density.Text);

            //    if (newDensity != density)
            //        experiment.overrideDefaultSensorDensity = true;
                density = newDensity;

                //experiment.sensorStart = low_range;
                //experiment.sensorEnd = high_range;
                experiment.rangefinderSensorDensity = density;
                experiment.sensorNoise = subform.sensorNoiseTrackBar.Value;
                experiment.headingNoise = subform.headingNoiseTrackBar.Value;
                experiment.effectorNoise = subform.effectorNoiseTrackBar.Value;

                experiment.initialize();
                Invalidate();
            }
        }

        private void agentSettingsClick(object sender, EventArgs e)
        {
            agent_info subform = new agent_info();

            //populate form
            if(experiment.overrideTeamFormation)
            {
                subform.orientation.Text = Convert.ToString(experiment.group_orientation);
                subform.spacing.Text = Convert.ToString(experiment.group_spacing);
                subform.heading.Text = Convert.ToString(experiment.robot_heading);
            }
            else
            {
                subform.orientation.Text = Convert.ToString(experiment.environment.group_orientation);
                subform.spacing.Text = Convert.ToString(experiment.environment.robot_spacing);
                subform.heading.Text = Convert.ToString(experiment.environment.robot_heading);
            }
            subform.substrate.Text = experiment.substrateDescriptionFilename;
            subform.populationSizeTextBox.Text = Convert.ToString(experiment.populationSize);
            if (experiment is MultiAgentExperiment)
            {
                subform.number_of_agents.Text = Convert.ToString(((MultiAgentExperiment)experiment).numberRobots);
                subform.collideCheckbox.Checked = ((MultiAgentExperiment)experiment).agentsCollide;
                subform.visibleCheckbox.Checked = ((MultiAgentExperiment)experiment).agentsVisible;
                subform.homogeneousCheckbox.Checked = ((MultiAgentExperiment)experiment).homogeneousTeam;
            }
                subform.modulationCheckBox.Checked = experiment.modulatoryANN;
            subform.adaptiveNetworkCheckBox.Checked = experiment.adaptableANN;

            subform.ShowDialog();

            //if ok is clicked
            if (subform.good)
            {
                //update environment with new information from form
                experiment.adaptableANN = subform.adaptiveNetworkCheckBox.Checked;
                experiment.modulatoryANN = subform.modulationCheckBox.Checked;

                float tempSpace, tempOrientation;
                int tempHeading;
                tempOrientation = Convert.ToInt32(subform.orientation.Text);
                tempSpace = Convert.ToInt32(subform.spacing.Text);
                tempHeading = Convert.ToInt32(subform.heading.Text);

                if(tempHeading != experiment.environment.robot_heading || tempOrientation != experiment.environment.group_orientation || tempSpace != experiment.environment.robot_spacing)
                    experiment.overrideTeamFormation = true;
                if(experiment.overrideTeamFormation)
                {
                    experiment.robot_heading = tempHeading;
                    experiment.group_spacing = tempSpace;
                    experiment.group_orientation = tempOrientation;
                }
                experiment.substrateDescriptionFilename = subform.substrate.Text;
                experiment.populationSize = Convert.ToInt32(subform.populationSizeTextBox.Text);
                if (experiment is MultiAgentExperiment)
                {
                    ((MultiAgentExperiment)experiment).numberRobots = Convert.ToInt32(subform.number_of_agents.Text);
                    ((MultiAgentExperiment)experiment).agentsVisible = subform.visibleCheckbox.Checked;
                    ((MultiAgentExperiment)experiment).agentsCollide = subform.collideCheckbox.Checked;
                    ((MultiAgentExperiment)experiment).homogeneousTeam = subform.homogeneousCheckbox.Checked;
                }
                experiment.normalizeWeights = subform.normalizeWeightsCheckBox1.Checked;
               
                experiment.initialize();
                Invalidate();
            }
        }

        private void wallModeClick(object sender, EventArgs e)
        {
            drawMode = drawModes.wallMode;
            updateSelection();
        }

        private void POIModeClick(object sender, EventArgs e)
        {
            drawMode = drawModes.POIMode;
            updateSelection();
        }

        private void selectModeClick(object sender, EventArgs e)
        {
            drawMode = drawModes.selectMode;
            updateSelection();
        }

        private void startModeClick(object sender, EventArgs e)
        {
            drawMode = drawModes.startMode;
            updateSelection();
        }

        private void goalModeClick(object sender, EventArgs e)
        {
            drawMode = drawModes.goalMode;
            updateSelection();
        }

        private void MouseClickHandler(object sender, MouseEventArgs e)
        {
            if (drawMode == drawModes.goalMode)
            {
				float x,y;
				frame.from_display((float)e.X,(float)e.Y,out x,out y);
                experiment.environment.goal_point.x = (double)x;
                experiment.environment.goal_point.y = (double)y;
            }
            if (drawMode == drawModes.startMode)
            {
				float x,y;
				frame.from_display((float)e.X,(float)e.Y,out x,out y);
                experiment.environment.start_point.x = (double)x;
                experiment.environment.start_point.y = (double)y;
            }
            else if (drawMode == drawModes.POIMode)
            {
				float x,y;
				frame.from_display((float)e.X,(float)e.Y,out x,out y);
                experiment.environment.POIPosition.Add(new Point((int) x,(int)y));
            }

            Invalidate();
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                selectMode = selectModes.dragMode;
            if (e.Button == MouseButtons.Right)
                selectMode = selectModes.rotateMode;
            if (e.Button == MouseButtons.Middle)
                selectMode = selectModes.scaleMode;

            if (drawMode == drawModes.selectMode)
            {

                if (selected_robot != null)
                {
                    bSelected_robot = true;
                    display_snap = false;
                    x1 = e.X;
                    y1 = e.Y;
                }
                else if (selected_wall != null)
                {
                    bSelected_wall = true;
                    display_snap = false;
                    x1 = e.X;
					y1 = e.Y;
                }
				else {
					bDragDisplay=true;
					x1 = e.X;
					y1 = e.Y;
				}
				
		
            }

            if (drawMode == drawModes.wallMode)
            {

                //try to avert some of the menu clicking business
                if (e.Y > 20)
                {

                    x1 = e.X;
                    y1 = e.Y;

                    if (display_snap)
                    {
                        x1 = snapx;
                        y1 = snapy;
                    }

                    display_tempwall = true;
                }
            }

            if (drawMode == drawModes.AOIMode)
            {
				float ox,oy;
				frame.from_display(e.X,e.Y,out ox, out oy);
                experiment.environment.AOIRectangle = new Rectangle((int)ox,(int)oy, (int)ox, (int)oy);
                displayAOIRectangle = true;

            }
        }

        private void MouseUpHandler(object sender, MouseEventArgs e)
        {
            if (drawMode == drawModes.selectMode)
            {
                if (bSelected_wall && selected_wall != null)
                    selected_wall.location = selected_wall.line.midpoint();
                bSelected_wall = false;
				bDragDisplay = false;
                bSelected_robot = false;
                Invalidate();
            }

            if (drawMode == drawModes.wallMode)
            {
                x2 = e.X;
                y2 = e.Y;

                if (display_snap)
                {
                    x2 = snapx;
                    y2 = snapy;
                }

                display_tempwall = false;
				float ox1,oy1,ox2,oy2;
				frame.from_display((float)x1,(float)y1,out ox1, out oy1);
				frame.from_display((float)x2,(float)y2,out ox2, out oy2);
                Wall new_wall = new Wall((double)ox1, (double)oy1, (double)ox2, (double)oy2, true, "");
                experiment.environment.walls.Add(new_wall);
                Invalidate();
            }

            if (drawMode == drawModes.AOIMode)
            {
                displayAOIRectangle = false;
            }
        }
		private void MouseWheelHandler(object sender, MouseEventArgs e)
		{
			frame.scale += (-0.001f) * e.Delta;
			Invalidate();
		}
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (drawMode == drawModes.selectMode)
            {
				if (bDragDisplay)
				{
					       //how far has the wall been dragged?
                    int dx = e.X - x1;
                    int dy = e.Y - y1;

                    x1 = e.X;
                    y1 = e.Y;
					
					float odx,ody;
					frame.offset_from_display((float)dx,(float)dy,out odx, out ody);
					frame.cx-=odx;
					frame.cy-=ody;
				}
				
                if (bSelected_wall)
                {
                    //wall-dragging code

                    //how far has the wall been dragged?
                    int dx = e.X - x1;
                    int dy = e.Y - y1;

                    x1 = e.X;
                    y1 = e.Y;
					
					float odx,ody;
					frame.offset_from_display((float)dx,(float)dy,out odx, out ody);

                    if (selectMode == selectModes.dragMode)
                    {
                        //update wall's position
                        selected_wall.line.p1.x += odx;
                        selected_wall.line.p1.y += ody;
                        selected_wall.line.p2.x += odx;
                        selected_wall.line.p2.y += ody;
                    }

                    if (selectMode == selectModes.rotateMode)
                    {
                        Point2D midpoint = selected_wall.line.midpoint();
                        selected_wall.line.p1.rotate(dy / 180.0 * 3.14, midpoint);
                        selected_wall.line.p2.rotate(dy / 180.0 * 3.14, midpoint);
                    }

                    if (selectMode == selectModes.scaleMode)
                    {
                        selected_wall.line.scale(1.0 + 0.05 * dy);
                    }

                    Invalidate();
                }
                if (bSelected_robot)
                {
                    //robot-dragging code

                    //how far has the robot been dragged?
                    int dx = e.X - x1;
                    int dy = e.Y - y1;

                    x1 = e.X;
                    y1 = e.Y;
					
					float odx,ody;
					frame.offset_from_display((float)dx,(float)dy,out odx, out ody);

					
                    //update robot's position
                    if (selectMode == selectModes.dragMode)
                    {
                        selected_robot.location.x += odx;
                        selected_robot.location.y += ody;
                    }
                    if (selectMode == selectModes.rotateMode)
                    {
                        selected_robot.heading += (dy / 10.0);
                    }
                    if (selectMode == selectModes.scaleMode)
                  {
                        selected_robot.radius += (dy / 2.0);
                        if (selected_robot.radius < 1.0)
                            selected_robot.radius = 1.0;
                        selected_robot.circle.radius = selected_robot.radius;
                    }
                    Invalidate();
                }

            }

            if (drawMode == drawModes.selectMode && !bSelected_wall && !bSelected_robot)
            {
                selected_wall = null;
                selected_robot = null;
                display_snap = false;
                //find snap
                Point2D newpoint = new Point2D((double)e.X, (double)e.Y);

                if (experiment != null)
				{
					if(experiment.robots!=null)
                    foreach (Robot robot in experiment.robots)
                    {
						Point2D screen_location = frame.to_display(robot.location);
                        if (screen_location.distance(newpoint) < 20.0)
                        {
                            display_snap = true;
                            snapx = (int)screen_location.x;
                            snapy = (int)screen_location.y;
                            selected_robot = robot;
                        }
                    }

                    if(experiment.environment.walls!=null)
                    foreach (Wall wall in experiment.environment.walls)
                    {
                        Point2D screen_location = frame.to_display(wall.location);
                        if (screen_location.distance(newpoint) < 20.0)
                        {
                            display_snap = true;
                            snapx = (int)screen_location.x;
                            snapy = (int)screen_location.y;
                            selected_wall = wall;
                        }
                    }

                    int index = 0;
                    selected_POI = -1;
                    if (experiment.environment.walls != null)
                        foreach (Point p in experiment.environment.POIPosition)
                        {
                            Point2D screen_location = frame.to_display(new Point2D(p.X, p.Y));
                            if (screen_location.distance(newpoint) < 20.0)
                            {
                                display_snap = true;
                                snapx = (int)screen_location.x;
                                snapy = (int)screen_location.y;
                                selected_POI = index;
                            }
                            index++;
                        }
				}
				
                
                Invalidate();
            }

            if (drawMode == drawModes.wallMode)
            {
                display_snap = false;
                //find snap
                Point2D newpoint = new Point2D((double)e.X, (double)e.Y);
                foreach (Wall wall in experiment.environment.walls)
                {
					Point2D screen_location1 = frame.to_display(wall.line.p1);
					Point2D screen_location2 = frame.to_display(wall.line.p2);
                    
                    if (screen_location1.distance(newpoint) < 10.0)
                    {
                        display_snap = true;
                        snapx = (int)screen_location1.x;
                        snapy = (int)screen_location1.y;
                    }
                    if (screen_location2.distance(newpoint) < 10.0)
                    {
                        display_snap = true;
                        snapx = (int)screen_location2.x;
                        snapy = (int)screen_location2.y;
                    }
                }
                x2 = e.X;
                y2 = e.Y;
                Invalidate();
            }
            if (drawMode == drawModes.AOIMode && displayAOIRectangle)
            {
				float ox,oy;
				frame.from_display(e.X,e.Y,out ox,out oy);
                experiment.environment.AOIRectangle = new Rectangle(experiment.environment.AOIRectangle.X,
                    experiment.environment.AOIRectangle.Y,(int) ox - experiment.environment.AOIRectangle.X, (int) oy - experiment.environment.AOIRectangle.Y);
                Invalidate();
            }
        }

        private void runExperiment()
        {
            if (experiment.agentBrain.getBrain(0) == null)
            {
                MessageBox.Show("Load a genome first.");
                return;
            }
            experiment.running = !experiment.running;
            setRunningState();
        }
        
        private void toggleAnimationClick(object sender, EventArgs e)
        {
            runExperiment();
        }

        private void loadGenomeClick(object sender, EventArgs e)
        {
            fileOpenDialog.Title = "Load Genome";
            DialogResult res = fileOpenDialog.ShowDialog(this);
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                string filename = fileOpenDialog.FileName;
                try
                {
                    experiment.loadGenome(filename);
                    evolveGenome = experiment.bestGenomeSoFar;
                    experiment.initialize();
                    // Schrum: fitness function was reset from factory, which is a problem for the FourTasks fitness.
                    // The special handling fixes the problem.
                    specialHandlingForFourTasks();
                    showCPPNWindow();

                    // Schrum: Two lines added by me in an attempt to get loaded multibrains to behave correctly
                    // Neither fixed not broke anything, so commented it out again.
                    //experiment.running = true;
                    //setRunningState();

                    toggleDisplayToolStripMenuItem.Enabled = true;
                    displayLoadedGenomeToolStripMenuItem.Enabled = true;
                    saveDecodedANNToolStripMenuItem.Enabled = true;
                    saveCPPNAsDOTToolStripMenuItem.Enabled = true;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
	    
	
        private void pOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.POIMode;
            updateSelection();
        }

        private void areaOfInterestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.AOIMode;
            updateSelection();
        }

        //Display the best genome
        private void toggleDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            evolveMutex.WaitOne();

            if (evolveGenome != null)
            {
                NeatGenome genome;
                genome = new NeatGenome(evolveGenome, 0);
                experiment = new RoomClearingExperiment((MultiAgentExperiment)evolutionExperiment);
                experiment.bestGenomeSoFar = genome;
                experiment.genome = genome;
                experiment.initialize();
                //step_size = 0.01f;
                experiment.running = true;
                setRunningState();
            }

            evolveMutex.ReleaseMutex();
        }

        private void displatyNetworkDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO not implemented yet
            //NetworkDifferencesForm netDif = new NetworkDifferencesForm((ModularNetwork)experiment.brain, experiment.robots);
            //netDif.Show();
        }

        private void doEvolution()
        {
            int currentGen = 0;
            HyperNEATEvolver evolve = new HyperNEATEvolver(evolutionExperiment);
			evolve.enableNoveltySearch(noveltySearch);						
			evolve.initializeEvolution(evolutionExperiment.populationSize,evolveGenome);
            evolve.experiment.DefaultNeatParameters.multiobjective = experiment.multiobjective;
            while (bEvolve)
            {
                evolveMutex.WaitOne();
                evolve.oneGeneration(currentGen);
                evolveGenome = evolutionExperiment.bestGenomeSoFar;
                evolveMutex.ReleaseMutex();
                Thread.Sleep(10);
                currentGen++;
            }
            evolve.end();
            evolveGenome = null;
        }

        private void saveCPPNAsDOTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (genome==null) 
            DialogResult res = fileSaveDialog.ShowDialog(this);
            string filename = fileSaveDialog.FileName;
            if (res == DialogResult.OK || res == DialogResult.Yes) CPPNDotWriterStatic.saveCPPNasDOT(experiment.bestGenomeSoFar, filename);
        }

        private void saveDecodedANNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (experiment.agentBrain.ANN != null)
            {
                XmlDocument thedoc = new XmlDocument();
                XmlNetworkWriterStatic.Write(thedoc, experiment.agentBrain.ANN, null);
                thedoc.Save("complete_nn.xml");
            }
        }

        #endregion

        private void loadExperimentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileOpenDialog.Title = "Open Experiment";
             DialogResult res = fileOpenDialog.ShowDialog(this);
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                string filename = fileOpenDialog.FileName;
                try
                {
                    ExperimentWrapper wr = ExperimentWrapper.load(filename);
                    this.Text = "MultiAgent-HyperSharpNEAT Simulator - " + filename;
                    experiment = wr.experiment;
                    experiment.initialize();

                    Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void saveExperimentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileSaveDialog.Title = "Save Experiment";
            DialogResult res = fileSaveDialog.ShowDialog(this);
            string filename = fileSaveDialog.FileName;
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                ExperimentWrapper wr = new ExperimentWrapper();
                wr.experiment = experiment;
                wr.save(filename);
                //experiment.save(filename);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();   
        }

        private void showCPPNWindow()
        {
            if (evolveGenome != null)
            {
                if (genomeVisualizerForm != null)
                    genomeVisualizerForm.Close();
                genomeVisualizerForm = new GenomeVisualizerForm(evolveGenome);
                genomeVisualizerForm.Show();
            }
        }

       
        private void showGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showCPPNWindow();
            Invalidate();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!bEvolve)
            {
                evolutionExperiment = new RoomClearingExperiment((MultiAgentExperiment)experiment);
                evolutionExperiment.initialize();
                bEvolve = true;
                evolveThread = new Thread(doEvolution);
                evolveThread.Start();
            }
            else //if it was already on, turn it off
            {
                bEvolve = false;
                evolveThread.Join();
                
            }
            startToolStripMenuItem.Checked = bEvolve;
        }

        //private void noveltySearchToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    noveltySearchToolStripMenuItem.Checked = !noveltySearchToolStripMenuItem.Checked;
		
        //}

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void keyCommandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            runExperiment();
        }

        private void setRunningState()
        {
            animationMenuItem.Checked = experiment.running;
            toggleAnimation.Checked = experiment.running;
            this.playButton.Enabled = !experiment.running;
            this.pauseButton.Enabled = experiment.running;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            experiment.initialize();
            Invalidate();
        }


        private void selectRobotModelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RobotModelSetter subForm = new RobotModelSetter();

            subForm.setup(experiment.robotModelName);

            if (subForm.ShowDialog(this) == DialogResult.OK)
            {
                if (subForm.robot != null)
                {
                    experiment.running = false;
                    experiment.robotModelName = subForm.robot.name;
                    experiment.initialize();
                }
                else
                    MessageBox.Show("Invalid Robot Model Selected");
            }
        }

        private void animationMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            experiment.running = false;
			if(experiment.robots!=null)
			if (experiment.robots.Count >0 )
			{
				if(experiment.robots[0] is Khepera3RobotModel_Remote) {
				 Khepera3RobotModel_Remote r = (Khepera3RobotModel_Remote)experiment.robots[0];
				 r.communicator.SendMotor(0.0f,0.0f);
				}
			}
			
            setRunningState();
        }

        private void updateSelection()
        {
            toolStripAOIButton.Checked = false;
            toolStripSelectButton.Checked = false;
            toolStripGoalButton.Checked = false;
            toolStripStartButton.Checked = false;
            toolStripWallButton.Checked = false;
            toolStripPOIButton.Checked = false;

            switch (drawMode)
            {
                case drawModes.selectMode: toolStripSelectButton.Checked = true; break;
                case drawModes.AOIMode: toolStripAOIButton.Checked = true; break;
                case drawModes.goalMode: toolStripGoalButton.Checked = true; break;
                case drawModes.POIMode: toolStripPOIButton.Checked = true; break;
                case drawModes.startMode: toolStripStartButton.Checked = true; break;
                case drawModes.wallMode: toolStripWallButton.Checked = true; break;
            }

        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.selectMode;
            updateSelection();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            drawMode = drawModes.wallMode;
            updateSelection();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.goalMode;
            updateSelection();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.startMode;
            updateSelection();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.AOIMode;
            updateSelection();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            drawMode = drawModes.POIMode;
            updateSelection();
        }

        private void SimulatorVisualizer_Load(object sender, EventArgs e)
        {

        }

        private void readmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadmeForm f = new ReadmeForm();
            f.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsDialog subform = new SettingsDialog();

            subform.seedTextBox.Text = experiment.genomeFilename;
            
            subform.adaptiveNetworkCheckBox.Enabled = !((MultiAgentExperiment)experiment).evolveSubstrate;
            if (!subform.adaptiveNetworkCheckBox.Enabled) subform.adaptiveNetworkCheckBox.Checked = false;
            subform.modulationCheckBox.Enabled = !((MultiAgentExperiment)experiment).evolveSubstrate;
            if (!subform.modulationCheckBox.Enabled) subform.modulationCheckBox.Checked = false;

            subform.EScheckBox.Enabled = ((MultiAgentExperiment)experiment).homogeneousTeam;

            subform.setupRobots(experiment.robotModelName);
            subform.setupFitnessFunction(experiment.fitnessFunctionName);

            subform.setupBehaviorCharazterization(experiment.behaviorCharacterizationName);
            subform.NoveltySearchCheckBox.Checked = noveltySearch;

            //populate form
            if (experiment.overrideTeamFormation)
            {
                subform.orientation.Text = Convert.ToString(experiment.group_orientation);
                subform.spacing.Text = Convert.ToString(experiment.group_spacing);
                subform.heading.Text = Convert.ToString(experiment.robot_heading);
            }
            else
            {
                subform.orientation.Text = Convert.ToString(experiment.environment.group_orientation);
                subform.spacing.Text = Convert.ToString(experiment.environment.robot_spacing);
                subform.heading.Text = Convert.ToString(experiment.environment.robot_heading);
            }
            subform.substrate.Text = experiment.substrateDescriptionFilename;
            subform.populationSizeTextBox.Text = Convert.ToString(experiment.populationSize);
            if (experiment is MultiAgentExperiment)
            {
                subform.number_of_agents.Text = Convert.ToString(((MultiAgentExperiment)experiment).numberRobots);
                subform.collideCheckbox.Checked = ((MultiAgentExperiment)experiment).agentsCollide;
                subform.visibleCheckbox.Checked = ((MultiAgentExperiment)experiment).agentsVisible;
                subform.homogeneousCheckbox.Checked = ((MultiAgentExperiment)experiment).homogeneousTeam;
            }
            subform.modulationCheckBox.Checked = experiment.modulatoryANN;
            subform.adaptiveNetworkCheckBox.Checked = experiment.adaptableANN;

            subform.normalizeWeightsCheckBox1.Checked = experiment.normalizeWeights;

            subform.multiObjectiveCheckBox.Checked = experiment.multiobjective;
            subform.multiBrainCheckBox.Checked = experiment.agentBrain.multipleBrains;
            subform.EScheckBox.Checked = experiment.evolveSubstrate;

            //subform.NoveltySearchCheckBox.Checked = 
          //  noveltySearchToolStripMenuItem.Checked = !noveltySearchToolStripMenuItem.Checked;

            int density;

           // if (experiment.overrideDefaultSensorDensity)
                density = experiment.rangefinderSensorDensity;
            //else if (experiment.robots != null && experiment.robots.Count > 0)
            //    density = experiment.robots[0].defaultSensorDensity();
            //else
            //    density = 1;

            subform.density.Text = Convert.ToString(density);

            //populate form		
            //subform.lower_range.Text = Convert.ToString(experiment.sensorStart * (180.0 / 3.14));
            //subform.upper_range.Text = Convert.ToString(experiment.sensorEnd * (180.0 / 3.14));
            subform.density.Text = Convert.ToString(experiment.rangefinderSensorDensity);
            subform.sensorNoiseTrackBar.Value = (int)experiment.sensorNoise;
            subform.effectorNoiseTrackBar.Value = (int)experiment.effectorNoise;
            subform.headingNoiseTrackBar.Value = (int)experiment.headingNoise;
            subform.setTextValues();

            subform.ShowDialog();

            //if ok is clicked
            if (subform.good)
            {
                experiment.multibrain = subform.multiBrainCheckBox.Checked;
                experiment.multiobjective = subform.multiObjectiveCheckBox.Checked;
                noveltySearch = subform.NoveltySearchCheckBox.Checked;
                experiment.evolveSubstrate = subform.EScheckBox.Checked;

                if (subform.bc != null)
                {
                    experiment.behaviorCharacterizationName = subform.bc.name;
                    experiment.behaviorCharacterization = BehaviorCharacterizationFactory.getBehaviorCharacterization(experiment.behaviorCharacterizationName);
                }

                if (subform.fitFun != null)
                {
                    experiment.fitnessFunctionName = subform.fitFun.name;
                    experiment.fitnessFunction = FitnessFunctionFactory.getFitnessFunction(experiment.fitnessFunctionName);
                }

                if (subform.robot != null)
                {
                    experiment.running = false;
                    experiment.robotModelName = (string)subform.robotModelComboBox.SelectedItem;
                    experiment.initialize();
                }

                int newDensity = Convert.ToInt32(subform.density.Text);

             //   if (newDensity != density)
             //       experiment.overrideDefaultSensorDensity = true;
                density = newDensity;
               
                //experiment.sensorStart = low_range;
                //experiment.sensorEnd = high_range;
                experiment.rangefinderSensorDensity = density;
                experiment.sensorNoise = subform.sensorNoiseTrackBar.Value;
                experiment.headingNoise = subform.headingNoiseTrackBar.Value;
                experiment.effectorNoise = subform.effectorNoiseTrackBar.Value;

                //update environment with new information from form
                experiment.adaptableANN = subform.adaptiveNetworkCheckBox.Checked;
                experiment.modulatoryANN = subform.modulationCheckBox.Checked;

                float tempSpace, tempOrientation;
                int tempHeading;
                tempOrientation = Convert.ToInt32(subform.orientation.Text);
                tempSpace = Convert.ToInt32(subform.spacing.Text);
                tempHeading = Convert.ToInt32(subform.heading.Text);

                if (tempHeading != experiment.environment.robot_heading || tempOrientation != experiment.environment.group_orientation || tempSpace != experiment.environment.robot_spacing)
                    experiment.overrideTeamFormation = true;
                if (experiment.overrideTeamFormation)
                {
                    experiment.robot_heading = tempHeading;
                    experiment.group_spacing = tempSpace;
                    experiment.group_orientation = tempOrientation;
                }
                experiment.substrateDescriptionFilename = subform.substrate.Text;
                experiment.populationSize = Convert.ToInt32(subform.populationSizeTextBox.Text);
                if (experiment is MultiAgentExperiment)
                {
                    ((MultiAgentExperiment)experiment).numberRobots = Convert.ToInt32(subform.number_of_agents.Text);
                    ((MultiAgentExperiment)experiment).agentsVisible = subform.visibleCheckbox.Checked;
                    ((MultiAgentExperiment)experiment).agentsCollide = subform.collideCheckbox.Checked;
                    ((MultiAgentExperiment)experiment).homogeneousTeam = subform.homogeneousCheckbox.Checked;
                }
                experiment.normalizeWeights = subform.normalizeWeightsCheckBox1.Checked;

                experiment.initialize();

                Invalidate();
            }
        }

        private void displayLoadedGenomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showCPPNWindow();
            Invalidate();
        }

        private void newExperimentMenuItem_Click(object sender, EventArgs e)
        {
            NewExperimentForm f = new NewExperimentForm();
            f.ShowDialog();

            if (f.simExp != null)
            {
                this.Text = "MultiAgent-HyperSharpNEAT Simulator - Unnamed";
                experiment = (SimulatorExperiment)f.simExp;
                experiment.substrateDescriptionFilename = "substrate.xml";
                experiment.environmentName = "default_environment.xml";
                experiment.setFitnessFunction("CoverageFitness");
                experiment.initialize();

                Invalidate();
            }
        }

        private void newEnvironmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            experiment.clearEnvironment();
            Invalidate();

            //experiment.environment = new Environment();
            //frame.sync_from_environment(experiment.environment);
            //experiment.initialize();
        }
    }
}
