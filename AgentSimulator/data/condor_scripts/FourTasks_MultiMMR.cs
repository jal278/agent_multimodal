requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_MultiMMR.err.$(Process)
Output = logs/FourTasks_MultiMMR.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-multi-MMR.xml -generations 5000 -rng_seed $(Process) -folder results/FourTasks-EXP-multi-MMR-$(Process)-
queue 30

