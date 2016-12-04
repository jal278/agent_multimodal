requirements=InMastodon
+Group="GRAD"
+Project="AI_ROBOTICS"
+ProjectDescription="Multiagent HyperNEAT"
executable = /usr/bin/mono
Initialdir = /scratch/cluster/schrum2/HyperNEAT
Notification = always
Notify_user = schrum2@cs.utexas.edu
Error  = logs/team_patrol_CCTinCPPNwMMD.err.$(Process)
Output = logs/team_patrol_CCTinCPPNwMMD.out.$(Process)

arguments = ./AgentSimulator.exe evolve -experiment EXP_patrol_signal-CCTinCPPNwMMD.xml -generations 2000 -rng_seed $(Process) -folder results/team-patrol-CCTinCPPNwMMD-$(Process)-
queue 10

