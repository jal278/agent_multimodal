using System;
using System.Collections.Generic;
using System.Text;
using SharpNeatLib.NeuralNetwork;
using SharpNeatLib;
using System.Drawing;

namespace Engine
{

    //Supports basic rangefinders and pie-slice sensors
    //TODO make this class abstract
    public abstract class Robot : SimulatorObject
    {
		
        protected AgentBrain agentBrain;
        public int rangefinderDensity;
		public List<Point2D> history;
		public bool draw_sensors;
		public bool collisionPenalty = false;
        public int id;        //Robot ID
        public List<ISensor> sensors = new List<ISensor>();
        public double velocity, heading, radius = 5;
        public int steps;
        public int marker;
        public Circle2D circle;
        public Point2D old_location = new Point2D();
        public bool collide_last;
        public bool autopilot;
		public bool display_debug = false;
		public SharpNeatLib.Maths.FastRandom rng;
        protected float headingNoise;
        protected float effectorNoise;
        protected float sensorNoise;
		public float dist_trav=0.0f;
        public float temp_dist=0.0f;
		public bool corrected = false;
	    protected float timeStep;
        // Schrum: Added to track which brain each robot is using
        public int currentBrainMode = 0;
        // Schrum: For remembering every brain/mode used through evaluation
        public List<int> brainHistory = new List<int>();
		
        //z-stack coordinate
        public float zstack;
 
        //Reference to current environment
        public Environment environment;

        public bool stopped = false;
        public bool disabled = false;

        public int collisions = 0;
		public bool recordHistory=false;
        public Robot()
        {
            dynamic = true;
            draw_sensors=true;
			history=new List<Point2D>();

            rangefinderDensity = defaultSensorDensity();
		}
        

          public void init(int id, double nx, double ny, double dir, 
            AgentBrain agentBrain, Environment environment,
            float sensorNoise, float effectorNoise, float headingNoise, float timeStep)
        {
            steps = 0;

            this.id = id;
    	    radius = defaultRobotSize();
            location = new Point2D(nx, ny);
            circle = new Circle2D(location, radius);
            old_location = new Point2D(location);
           
            heading = dir;
            velocity = 0.0;
            collide_last = false;
            this.timeStep = timeStep;
            this.environment = environment;
            this.agentBrain = agentBrain;
            this.sensorNoise = sensorNoise;
            this.effectorNoise = effectorNoise;
            this.headingNoise = headingNoise;
            populateSensors();
			if(environment.seed != -1) 
            	rng=environment.rng; //new SharpNeatLib.Maths.FastRandom(environment.seed); //Utilities.random;
        	else
				rng=environment.rng; //new SharpNeatLib.Maths.FastRandom();
		}

        public override void update()
        {
            throw new NotImplementedException();
        }

        // Schrum: Track which of several brains is being chosen
        public void updateBrainMode(int newMode)
        {
            currentBrainMode = newMode;
            brainHistory.Add(currentBrainMode);                        
        }

        //Called when robot collides
        public virtual void onCollision()
        {
			dist_trav-=temp_dist;
            collisions++;
			undo();
			
			if (collisionPenalty && collisions > 0)
            {
                disabled = true;
                stopped = true;
            }
            //if (robot.collisions > 1)
            //{
            //    robot.disabled = true;
            //    robot.stopped = true;
            //}
            //else
            //{
            //    robot.health = (1.0 / ((double)robot.collisions + 1.0));
            //    robot.undo();
            //}
        }

        public double noisyHeading()
        {
            return heading + 0.1 * (rng.NextBool() ? 1 : -1) * rng.Next(0, (int)headingNoise) / 100.0;
        }

        public virtual void updateSensors(Environment env, List<Robot> robots,CollisionManager cm)
        {
			foreach (ISensor sensor in sensors) {
				sensor.update(env, robots,cm);
			}
        }


        public void populateSensors(float[] rangeFinderSensorAngles)
        {
            sensors = new List<ISensor>();
            foreach (double val in rangeFinderSensorAngles)
            {
                sensors.Add(new RangeFinder(val, this, 400.0,this.sensorNoise));
            }
        }
            
        public void updatePosition()
        {
            //record old coordinates
			temp_dist=(float)old_location.distance_sq(location);
			dist_trav+=temp_dist;

			old_location.x = location.x;
            old_location.y = location.y;

            //update current coordinates (may be revoked if new position forces collision)
            if (!stopped)
            {
                //Console.WriteLine("In updatePosition() before noisyHeading(): heading = " + heading);
                double tempHeading = noisyHeading();
                //Console.WriteLine("After noisyHeading(): tempHeading = " + tempHeading + ",heading = " + heading);
				heading=tempHeading;
				//Console.WriteLine(velocity + " " + timeStep);
                double dx = Math.Cos(tempHeading) * velocity * timeStep;
                double dy = Math.Sin(tempHeading) * velocity * timeStep;
                location.x += dx;
                location.y += dy;
				//Console.WriteLine(dx);
            }
			history.Add(new Point2D(location));

            // Schrum: I think the heading should always be from 0 to 2PI. This check assures that.
            while (heading > Math.PI * 2)
            {
                heading -= Math.PI * 2;
                /*
                Console.WriteLine("Heading out of range (updatePosition): " + heading);
                Console.WriteLine(this.GetType().ToString());
                System.Windows.Forms.Application.Exit();
                System.Environment.Exit(1);
                */
            }

        }

        public override void undo()
        {
            location.x = old_location.x;
            location.y = old_location.y;
        }

        //default robot draw method
        public virtual void draw(Graphics g, CoordinateFrame frame)
        {
            Point upperleft = frame.to_display((float)(circle.p.x - radius),(float)(circle.p.y- radius));
			int size = (int)((radius*2)/frame.scale);
			Rectangle r = new Rectangle(upperleft.X,upperleft.Y,size,size);
            if (disabled)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else if (collide_last)
                g.DrawEllipse(EngineUtilities.RedPen, r);
            else if (corrected)
                g.DrawEllipse(EngineUtilities.YellowPen, r);
            else
                g.DrawEllipse(EngineUtilities.BluePen, r);
			int sensCount=0;
			foreach(ISensor sensor in sensors)
			{
				sensor.draw(g,frame);
			}
			
			if(display_debug)
			foreach(ISensor sensor in sensors)
			{
				if(draw_sensors) {
				double val = sensor.get_value();
				if(val<0.0) val=0.0;
				if(val>1.0) val=1.0;
				Color col = Color.FromArgb((int)(val*255),0,0); //(int)(val*255),(int)(val*255));
				SolidBrush newpen = new SolidBrush(col);
				g.FillRectangle(newpen,sensCount*40,500+30*id,40,30);
				sensCount+=1;
				}
			}
        }
        
        public abstract float defaultRobotSize();

        public abstract int defaultSensorDensity();

        public virtual void populateSensors()
        {
            populateSensors(rangefinderDensity);
        }
	
        public abstract void populateSensors(int size);

        public abstract void networkResults(float[] outputs);
        
        public virtual void robotSettingsChanged()
        {
        }

        public virtual void doAction() {}
    }
}
