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
"lone_patrol_switchUngeom3-9/lone_patrol_switchUngeom3-9logfile.txt" u 1:2 notitle lt 3, \
"lone_patrol_switchUngeomMMD-0/lone_patrol_switchUngeomMMD-0logfile.txt" u 1:2 t "lone_patrol_switchUngeomMMD" lt 4, \
"lone_patrol_switchUngeomMMD-1/lone_patrol_switchUngeomMMD-1logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-2/lone_patrol_switchUngeomMMD-2logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-3/lone_patrol_switchUngeomMMD-3logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-4/lone_patrol_switchUngeomMMD-4logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-5/lone_patrol_switchUngeomMMD-5logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-6/lone_patrol_switchUngeomMMD-6logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-7/lone_patrol_switchUngeomMMD-7logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-8/lone_patrol_switchUngeomMMD-8logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMD-9/lone_patrol_switchUngeomMMD-9logfile.txt" u 1:2 notitle lt 4, \
"lone_patrol_switchUngeomMMP-0/lone_patrol_switchUngeomMMP-0logfile.txt" u 1:2 t "lone_patrol_switchUngeomMMP" lt 5, \
"lone_patrol_switchUngeomMMP-1/lone_patrol_switchUngeomMMP-1logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-2/lone_patrol_switchUngeomMMP-2logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-3/lone_patrol_switchUngeomMMP-3logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-4/lone_patrol_switchUngeomMMP-4logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-5/lone_patrol_switchUngeomMMP-5logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-6/lone_patrol_switchUngeomMMP-6logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-7/lone_patrol_switchUngeomMMP-7logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-8/lone_patrol_switchUngeomMMP-8logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMP-9/lone_patrol_switchUngeomMMP-9logfile.txt" u 1:2 notitle lt 5, \
"lone_patrol_switchUngeomMMR-0/lone_patrol_switchUngeomMMR-0logfile.txt" u 1:2 t "lone_patrol_switchUngeomMMR" lt 6, \
"lone_patrol_switchUngeomMMR-1/lone_patrol_switchUngeomMMR-1logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-2/lone_patrol_switchUngeomMMR-2logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-3/lone_patrol_switchUngeomMMR-3logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-4/lone_patrol_switchUngeomMMR-4logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-5/lone_patrol_switchUngeomMMR-5logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-6/lone_patrol_switchUngeomMMR-6logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-7/lone_patrol_switchUngeomMMR-7logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-8/lone_patrol_switchUngeomMMR-8logfile.txt" u 1:2 notitle lt 6, \
"lone_patrol_switchUngeomMMR-9/lone_patrol_switchUngeomMMR-9logfile.txt" u 1:2 notitle lt 6

pause -1

plot \
"< bash group.bash average.bash lone_patrol_single-" u 1:2 t "lone_patrol_single" lt 1, \
"< bash group.bash average.bash lone_patrol_switch3-" u 1:2 t "lone_patrol_switch3" lt 2, \
"< bash group.bash average.bash lone_patrol_switchUngeom3-" u 1:2 t "lone_patrol_switchUngeom3" lt 3, \
"< bash group.bash average.bash lone_patrol_switchUngeomMMD-" u 1:2 t "lone_patrol_switchUngeomMMD" lt 4, \
"< bash group.bash average.bash lone_patrol_switchUngeomMMP-" u 1:2 t "lone_patrol_switchUngeomMMP" lt 5, \
"< bash group.bash average.bash lone_patrol_switchUngeomMMR-" u 1:2 t "lone_patrol_switchUngeomMMR" lt 6

pause -1
