set style data lines
set key bottom right

plot \
"lone_patrol_single-0/lone_patrol_single-0logfile.txt" u 1:2 t "lone_patrol_single" lt 1, \
"lone_patrol_single-1/lone_patrol_single-1logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-2/lone_patrol_single-2logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-3/lone_patrol_single-3logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-4/lone_patrol_single-4logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-5/lone_patrol_single-5logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-6/lone_patrol_single-6logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-7/lone_patrol_single-7logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-8/lone_patrol_single-8logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_single-9/lone_patrol_single-9logfile.txt" u 1:2 notitle lt 1, \
"lone_patrol_switch3-0/lone_patrol_switch3-0logfile.txt" u 1:2 t "lone_patrol_switch3" lt 2, \
"lone_patrol_switch3-1/lone_patrol_switch3-1logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-2/lone_patrol_switch3-2logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-3/lone_patrol_switch3-3logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-4/lone_patrol_switch3-4logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-5/lone_patrol_switch3-5logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-6/lone_patrol_switch3-6logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-7/lone_patrol_switch3-7logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-8/lone_patrol_switch3-8logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switch3-9/lone_patrol_switch3-9logfile.txt" u 1:2 notitle lt 2, \
"lone_patrol_switchUngeom3-0/lone_patrol_switchUngeom3-0logfile.txt" u 1:2 t "lone_patrol_switchUngeom3" lt 3, \
"lone_patrol_switchUngeom3-1/lone_patrol_switchUngeom3-1logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-2/lone_patrol_switchUngeom3-2logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-3/lone_patrol_switchUngeom3-3logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-4/lone_patrol_switchUngeom3-4logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-5/lone_patrol_switchUngeom3-5logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-6/lone_patrol_switchUngeom3-6logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-7/lone_patrol_switchUngeom3-7logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-8/lone_patrol_switchUngeom3-8logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeom3-9/lone_patrol_switchUngeom3-9logfile.txt" u 1:2 notitle lt 3


pause -1

plot \
"< bash group.bash average.bash lone_patrol_single-" u 1:2 t "lone_patrol_single" lt 1, \
"< bash group.bash average.bash lone_patrol_switch3-" u 1:2 t "lone_patrol_switch3" lt 2, \
"< bash group.bash average.bash lone_patrol_switchUngeom3-" u 1:2 t "lone_patrol_switchUngeom3" lt 3

pause -1
