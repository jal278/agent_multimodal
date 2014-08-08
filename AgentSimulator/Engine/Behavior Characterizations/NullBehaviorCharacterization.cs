using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    class NullBehaviorCharacterization : IBehaviorCharacterization
    {

        #region IBehaviorCharacterization Members

        public IBehaviorCharacterization copy()
        {
            return new NullBehaviorCharacterization();
        }

        public string name
        {
            get { return this.GetType().Name; }
        }

        public string description
        {
            get { return "Does nothing"; }
        }

        public List<double> calculate(SimulatorExperiment experiment, instance_pack i)
        {
            return null;
        }

        public void update(SimulatorExperiment engine, instance_pack i)
        {
            return;
        }

        public void reset()
        {
            return;
        }

        #endregion
    }
}
