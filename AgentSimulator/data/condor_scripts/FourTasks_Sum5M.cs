requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEATFourTasks
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/FourTasks_5M.err.$(Process)
Output = logs/FourTasks_5M.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment FourTasks-EXP-5M.xml -generations 4000 -rng_seed $(Process) -folder results/FourTasks-EXP-5M-$(Process)-
queue 30

