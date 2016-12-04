requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEAT
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/two_room_PunishMMP.err.$(Process)
Output = logs/two_room_PunishMMP.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment EXP_two_room-PunishMMP.xml -generations 2000 -rng_seed $(Process) -folder results/two-room-PunishMMP-$(Process)-
queue 10

