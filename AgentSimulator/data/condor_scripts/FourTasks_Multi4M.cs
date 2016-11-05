requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_Multi4M.err.$(Process)
Output = logs/FourTasks_Multi4M.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-multi-4M.xml -generations 5000 -rng_seed $(Process) -folder results/FourTasks-EXP-multi-4M-$(Process)-
queue 30

