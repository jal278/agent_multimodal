using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpNeatLib.NeuralNetwork;
using System.Drawing;
using SharpNeatLib.NeatGenome;

// Schrum: Class/form made by me to display the timeline of how a robot is using its modules/brains

namespace Engine.Forms
{
    class ModuleUsageVisualizerForm : Form
    {
        private Robot selectedRobot;
        ModularNetwork net;
        Graphics g;
        int evaluationTime;
        
        public ModuleUsageVisualizerForm(Robot _selectedRobot, ModularNetwork _net, int _evaluationTime) 
        {
            evaluationTime = _evaluationTime;
            _net.UpdateNetworkEvent += networkUpdated;
            InitializeComponent();
            net = _net;
            selectedRobot = _selectedRobot;
            this.Text = "Module Usage Visualizer [z=" + selectedRobot.zstack + "]";
            SetBounds(1, 1, 320, 90); // Schrum: Need to tweak?

            //set up double buffering
            this.SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);
        }

        //This function gets called when the current simulated network sends an update event
        public void networkUpdated(ModularNetwork _net)
        {
            Refresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ModuleUsageVisualizerForm
            // 
            this.ClientSize = new System.Drawing.Size(524, 53);
            this.Name = "ModuleUsageVisualizerForm";
            this.Text = "?";
            this.Load += new System.EventHandler(this.ModuleUsageVisualizerForm_Load);
            this.SizeChanged += new System.EventHandler(this.ModuleUsageVisualizerForm_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ModuleUsageVisualizerForm_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ModuleUsageVisualizerForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ModuleUsageVisualizerForm_MouseMove);
            this.ResumeLayout(false);

        }

        private void ModuleUsageVisualizerForm_Paint(object sender, PaintEventArgs e)
        {
            if (selectedRobot != null && selectedRobot.brainHistory != null && selectedRobot.brainHistory.Count > 1)
            {
                double widthPerStep = this.Width / (1.0 * selectedRobot.brainHistory.Count);
                g = e.Graphics;
                int currentMode = selectedRobot.brainHistory[0];
                int modeStartTime = 0;
                Rectangle r;
                for (int i = 0; i < selectedRobot.brainHistory.Count; i++)
                {
                    // Schrum: Draw each contiguous period of the same mode as a single
                    // rectangle. This is needed because individual time slices eventually
                    // drop in width below one pixel, and can't be drawn individuallly.
                    if (currentMode != selectedRobot.brainHistory[i])
                    {
                        r = new Rectangle((int)(modeStartTime * widthPerStep), 0, (int)((i - modeStartTime)*widthPerStep), this.Height);
                        g.FillRectangle(EngineUtilities.modePen(currentMode), r);
                        // Schrum: Adjust to track new mode
                        modeStartTime = i;
                        currentMode = selectedRobot.brainHistory[i];
                    }

                }

                // Schrum: Draw final rectangle for module used up until the current time step
                r = new Rectangle((int)(modeStartTime * widthPerStep), 0, (int)((selectedRobot.brainHistory.Count - modeStartTime)*widthPerStep), this.Height);
                g.FillRectangle(EngineUtilities.modePen(currentMode), r);
            }
        }

        private void ModuleUsageVisualizerForm_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void ModuleUsageVisualizerForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Schrum: Don't think anything is needed here
        }

        private void ModuleUsageVisualizerForm_Load(object sender, EventArgs e)
        {
            // Schrum: Nothing needed
        }

        private void ModuleUsageVisualizerForm_MouseClick(object sender, MouseEventArgs e)
        {
            // Schrum: Nothing needed
        }
    }
}
