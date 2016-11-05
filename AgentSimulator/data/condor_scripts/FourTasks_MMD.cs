requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_MMD.err.$(Process)
Output = logs/FourTasks_MMD.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-MMD.xml -generations 5000 -rng_seed $(Process) -folder results/FourTasks-EXP-MMD-$(Process)-
queue 30

