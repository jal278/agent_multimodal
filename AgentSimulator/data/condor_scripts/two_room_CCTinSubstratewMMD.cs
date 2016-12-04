requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEAT
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/two_room_CCTinSubstratewMMD.err.$(Process)
Output = logs/two_room_CCTinSubstratewMMD.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment EXP_two_room-CCTinSubstratewMMD.xml -generations 2000 -rng_seed $(Process) -folder results/two-room-CCTinSubstratewMMD-$(Process)-
queue 10

