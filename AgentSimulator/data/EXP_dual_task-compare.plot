set style data lines
set key bottom right

plot \
"EXP_dual_task-0/EXP_dual_task-0logfile.txt" u 1:2 t "EXP_dual_task" lt 1, \
"EXP_dual_task-1/EXP_dual_task-1logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-2/EXP_dual_task-2logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-3/EXP_dual_task-3logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-4/EXP_dual_task-4logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-5/EXP_dual_task-5logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-6/EXP_dual_task-6logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-7/EXP_dual_task-7logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-8/EXP_dual_task-8logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-9/EXP_dual_task-9logfile.txt" u 1:2 notitle lt 1, \
"EXP_dual_task-twoBrainSwitch-0/EXP_dual_task-twoBrainSwitch-0logfile.txt" u 1:2 t "EXP_dual_task-twoBrainSwitch" lt 2, \
"EXP_dual_task-twoBrainSwitch-1/EXP_dual_task-twoBrainSwitch-1logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-2/EXP_dual_task-twoBrainSwitch-2logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-3/EXP_dual_task-twoBrainSwitch-3logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-4/EXP_dual_task-twoBrainSwitch-4logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-5/EXP_dual_task-twoBrainSwitch-5logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-6/EXP_dual_task-twoBrainSwitch-6logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-7/EXP_dual_task-twoBrainSwitch-7logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-8/EXP_dual_task-twoBrainSwitch-8logfile.txt" u 1:2 notitle lt 2, \
"EXP_dual_task-twoBrainSwitch-9/EXP_dual_task-twoBrainSwitch-9logfile.txt" u 1:2 notitle lt 2

pause -1

plot \
"< bash group.bash average.bash EXP_dual_task-" u 1:2 t "EXP_dual_task" lt 1, \
"< bash group.bash average.bash EXP_dual_task-twoBrainSwitch-" u 1:2 t "EXP_dual_task-twoBrainSwitch" lt 2

pause -1
