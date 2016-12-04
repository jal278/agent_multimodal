requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEAT
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/team_patrol_PunishMMR.err.$(Process)
Output = logs/team_patrol_PunishMMR.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment EXP_patrol_signal-PunishMMR.xml -generations 2000 -rng_seed $(Process) -folder results/team-patrol-PunishMMR-$(Process)-
queue 10

