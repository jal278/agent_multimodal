
set terminal pdf color enhanced font "Arial, 14"
#set size 0.8, 0.8
set style data lines
#set key under
set key bottom right 
#set key top left maxrows 2
#set key top center 

#t=2.262 # df=9, p=0.05, two-tailed
#t=2.093 # df=19, p=0.05, two-tailed
t=2.045 # df=29, p=0.05, two-tailed

set ylabel "Fitness"
set xlabel "Generation"

set xrange [0:3001]
set yrange [0:1]


set style fill transparent solid 0.2 noborder

num = 0

do for [var in "Combo Team Lone DualHall DualForage TwoRooms"] {

	set output sprintf("SumFourTask-%d-%s-CURVES.pdf",num,var)
	plot \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MT5',var) u 1:2 notitle lw 2 lt 9, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MT5',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 9, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MT5',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 9, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MT5',var) every 50::0 u 1:2:2:2 t "MT5" with errorbars lw 2 lt 9, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP SPG5',var) u 1:2 notitle lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP SPG5',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP SPG5',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP SPG5',var) every 50::0 u 1:2:2:2 t "SPG5" with errorbars lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMR',var) u 1:2 notitle lw 2 lt 8, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMR',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 8, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMR',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 8, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMR',var) every 50::0 u 1:2:2:2 t "MMR" with errorbars lw 2 lt 8, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMP',var) u 1:2 notitle lw 2 lt 7, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMP',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 7, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMP',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 7, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMP',var) every 50::0 u 1:2:2:2 t "MMP" with errorbars lw 2 lt 7, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMD',var) u 1:2 notitle lw 2 lt 6, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMD',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 6, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMD',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 6, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP MMD',var) every 50::0 u 1:2:2:2 t "MMD" with errorbars lw 2 lt 6, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 5M',var) u 1:2 notitle lw 2 lt 5, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 5M',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 5, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 5M',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 5, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 5M',var) every 50::0 u 1:2:2:2 t "5M" with errorbars lw 2 lt 5, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 4M',var) u 1:2 notitle lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 4M',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 4M',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 4M',var) every 50::0 u 1:2:2:2 t "4M" with errorbars lw 2 lt 4, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 3M',var) u 1:2 notitle lw 2 lt 3, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 3M',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 3, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 3M',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 3, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 3M',var) every 50::0 u 1:2:2:2 t "3M" with errorbars lw 2 lt 3, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 2M',var) u 1:2 notitle lw 2 lt 2, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 2M',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 2, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 2M',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 2, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 2M',var) every 50::0 u 1:2:2:2 t "2M" with errorbars lw 2 lt 2, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 1M',var) u 1:2 notitle lw 2 lt 1, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 1M',var) every 5::0::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 1, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 1M',var) every 20::40 u 1:($2 - t*$4):($2 + t*$4) notitle with filledcurves lw 2 lt 1, \
	sprintf('< bash group.bash special%s.bash FourTasks-EXP 1M',var) every 50::0 u 1:2:2:2 t "1M" with errorbars lw 2 lt 1

	num = num + 1

}
