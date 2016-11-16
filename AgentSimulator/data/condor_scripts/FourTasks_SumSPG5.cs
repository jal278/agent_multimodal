requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_SPG5.err.$(Process)
Output = logs/FourTasks_SPG5.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-SPG5.xml -generations 4000 -rng_seed $(Process) -folder results/FourTasks-EXP-SPG5-$(Process)-
queue 30

