using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using simulator.Experiments;
using System.Reflection;

namespace simulator.Forms
{
    public partial class NewExperimentForm : Form
    {
        public SimulatorExperimentInterface simExp = null;

        public NewExperimentForm()
        {
            InitializeComponent();

            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (t.GetInterface("SimulatorExperimentInterface", true) != null)
                    this.experimentComboBox.Items.Add(t.Name);
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            String experimentName = experimentComboBox.SelectedItem.ToString();

            string className = "Engine." + experimentName;
            simExp =  (SimulatorExperimentInterface)Assembly.GetExecutingAssembly().CreateInstance(className);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
