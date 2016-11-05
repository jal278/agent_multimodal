requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_Multi1M.err.$(Process)
Output = logs/FourTasks_Multi1M.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-multi-1M.xml -generations 3000 -rng_seed $(Process) -folder results/FourTasks-EXP-multi-1M-$(Process)-
queue 30

