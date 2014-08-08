set style data lines
set key bottom right

plot \
"patrol_signal-0/patrol_signal-0logfile.txt" u 1:2 t "patrol_signal" lt 1, \
"patrol_signal-1/patrol_signal-1logfile.txt" u 1:2 notitle lt 1, \
"patrol_signal-2/patrol_signal-2logfile.txt" u 1:2 notitle lt 1, \
"patrol_signal-3/patrol_signal-3logfile.txt" u 1:2 notitle lt 1, \
"patrol_signal-4/patrol_signal-4logfile.txt" u 1:2 notitle lt 1, \
"patrol_switch-0/patrol_switch-0logfile.txt" u 1:2 t "patrol_switch" lt 2, \
"patrol_switch-1/patrol_switch-1logfile.txt" u 1:2 notitle lt 2, \
"patrol_switch-2/patrol_switch-2logfile.txt" u 1:2 notitle lt 2, \
"patrol_switch-3/patrol_switch-3logfile.txt" u 1:2 notitle lt 2, \
"patrol_switch-4/patrol_switch-4logfile.txt" u 1:2 notitle lt 2 

pause -1

plot \
"< bash group.bash average.bash patrol_signal-" u 1:2 t "patrol_signal", \
"< bash group.bash average.bash patrol_switch-" u 1:2 t "patrol_switch"

pause -1
