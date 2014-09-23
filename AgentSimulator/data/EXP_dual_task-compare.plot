set term postscript eps enhanced color
set style data lines
set key bottom right

set output "Dual-Task-All.eps"
plot \
"EXP_dual_task-0/EXP_dual_task-0logfile.txt" u 1:2 t "EXP_dual_task" lt 1, \
"EXP_dual_task-1/EXP_dual_task-1logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-2/EXP_dual_task-2logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-3/EXP_dual_task-3logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-4/EXP_dual_task-4logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-MMD-0/EXP_dual_task-MMD-0logfile.txt" u 1:2 t "EXP_dual_task-MMD" lt 3, \
"EXP_dual_task-MMD-1/EXP_dual_task-MMD-1logfile.txt" u 1:2 notitle lt 3, \
"EXP_dual_task-MMD-2/EXP_dual_task-MMD-2logfile.txt" u 1:2 notitle lt 3, \
"EXP_dual_task-MMD-3/EXP_dual_task-MMD-3logfile.txt" u 1:2 notitle lt 3, \
"EXP_dual_task-MMD-4/EXP_dual_task-MMD-4logfile.txt" u 1:2 notitle lt 3, \
"EXP_dual_task-MMR-0/EXP_dual_task-MMR-0logfile.txt" u 1:2 t "EXP_dual_task-MMR" lt 4, \
"EXP_dual_task-MMR-1/EXP_dual_task-MMR-1logfile.txt" u 1:2 notitle lt 4, \
"EXP_dual_task-MMR-2/EXP_dual_task-MMR-2logfile.txt" u 1:2 notitle lt 4, \
"EXP_dual_task-MMR-3/EXP_dual_task-MMR-3logfile.txt" u 1:2 notitle lt 4, \
"EXP_dual_task-MMR-4/EXP_dual_task-MMR-4logfile.txt" u 1:2 notitle lt 4, \
"EXP_dual_task-MMP-0/EXP_dual_task-MMP-0logfile.txt" u 1:2 t "EXP_dual_task-MMP" lt 5, \
"EXP_dual_task-MMP-1/EXP_dual_task-MMP-1logfile.txt" u 1:2 notitle lt 5, \
"EXP_dual_task-MMP-2/EXP_dual_task-MMP-2logfile.txt" u 1:2 notitle lt 5, \
"EXP_dual_task-MMP-3/EXP_dual_task-MMP-3logfile.txt" u 1:2 notitle lt 5, \
"EXP_dual_task-MMP-4/EXP_dual_task-MMP-4logfile.txt" u 1:2 notitle lt 5, \
"EXP_dual_task-twoBrainSwitch-0/EXP_dual_task-twoBrainSwitch-0logfile.txt" u 1:2 t "EXP_dual_task-twoBrainSwitch" lt 2, \
"EXP_dual_task-twoBrainSwitch-1/EXP_dual_task-twoBrainSwitch-1logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-2/EXP_dual_task-twoBrainSwitch-2logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-3/EXP_dual_task-twoBrainSwitch-3logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-4/EXP_dual_task-twoBrainSwitch-4logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainPref-0/EXP_dual_task-twoBrainPref-0logfile.txt" u 1:2 t "EXP_dual_task-twoBrainPref" lt 6, \
"EXP_dual_task-twoBrainPref-1/EXP_dual_task-twoBrainPref-1logfile.txt" u 1:2 notitle lt 6, \
"EXP_dual_task-twoBrainPref-2/EXP_dual_task-twoBrainPref-2logfile.txt" u 1:2 notitle lt 6, \
"EXP_dual_task-twoBrainPref-3/EXP_dual_task-twoBrainPref-3logfile.txt" u 1:2 notitle lt 6, \
"EXP_dual_task-twoBrainPref-4/EXP_dual_task-twoBrainPref-4logfile.txt" u 1:2 notitle lt 6

set output "Dual-Task-AVG.eps"
plot \
"< bash group.bash average.bash EXP_dual_task-" u 1:2 t "EXP_dual_task" lt 1, \
"< bash group.bash average.bash EXP_dual_task-MMD-" u 1:2 t "EXP_dual_task-MMD" lt 3, \
"< bash group.bash average.bash EXP_dual_task-MMR-" u 1:2 t "EXP_dual_task-MMR" lt 4, \
"< bash group.bash average.bash EXP_dual_task-MMP-" u 1:2 t "EXP_dual_task-MMP" lt 5, \
"< bash group.bash average.bash EXP_dual_task-twoBrainSwitch-" u 1:2 t "EXP_dual_task-twoBrainSwitch" lt 2, \
"< bash group.bash average.bash EXP_dual_task-twoBrainPref-" u 1:2 t "EXP_dual_task-twoBrainPref" lt 6

