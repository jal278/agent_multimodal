requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_MMR.err.$(Process)
Output = logs/FourTasks_MMR.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-MMR.xml -generations 4000 -rng_seed $(Process) -folder results/FourTasks-EXP-MMR-$(Process)-
queue 30

