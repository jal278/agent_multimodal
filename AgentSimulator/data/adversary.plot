
plot "loc_temp.txt" u 2,  "loc_temp.txt" u 4, "loc_temp.txt" u 9, "loc_temp.txt" u 11, "loc_temp.txt" u 16
pause -1

set xrange [0:650]
set yrange [-1100:0]

set arrow from 106,-716 to 231,-716
set arrow from 277,-717 to 277,-640
set arrow from 277,-640 to 371,-641
set arrow from 230,-594 to 369,-595
set arrow from 371,-641 to 419,-641
set arrow from 230,-594 to 231,-726
set arrow from 369,-595 to 370,-465
set arrow from 277,-716 to 585,-716
set arrow from 421,-466 to 419,-641
set arrow from 106,-1066 to 106,-716
set arrow from 585,-1066 to 585,-716
set arrow from 106,-1066 to 585,-1066
set arrow from 106,-465 to 370,-465
set arrow from 421,-466 to 585,-466
set arrow from 106,-465 to 106,-165
set arrow from 585,-466 to 585,-166
set arrow from 106,-166 to 585,-166

set style data lines
end=2000

plot \
"line_temp.txt" u 1:(-$2) every ::0::end t "Connection Silent", \
"line_vis.txt" u 1:(-$2) every ::0::end t "Connection Visual"
pause -1

plot \
"loc_temp.txt" u 1:(-$2) every ::0::end t "Evolved Silent", \
"loc_temp.txt" u 3:(-$4) every ::0::end t "Enemy Silent", \
"loc_temp.txt" u 10:(-$11) every ::0::end t "Enemy Silent 2", \
"vis_temp.txt" u 1:(-$2) every ::0::end t "Evolved Vis", \
"vis_temp.txt" u 3:(-$4) every ::0::end t "Enemy Vis"
pause -1

plot \
"loc_temp.txt" u 1:(-$2) every ::0::end t "Evolved Silent", \
"loc_temp.txt" u 3:($5==0 ? 1/0 : -$4) every ::0::end t "Enemy Silent", \
"vis_temp.txt" u 1:(-$2) every ::0::end t "Evolved Vis", \
"vis_temp.txt" u 3:($5==0 ? 1/0 : -$4) every ::0::end t "Enemy Vis"
pause -1

plot \
"loc_temp.txt" u 1:(-$2) every ::0::end t "Evolved Silent", \
"loc_temp.txt" u 3:($5>=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Left", \
"loc_temp.txt" u 3:($5<=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Right", \
"vis_temp.txt" u 1:(-$2) every ::0::end t "Evolved Vis", \
"vis_temp.txt" u 3:($5>=0 ? 1/0 : -$4) every ::0::end t "Enemy Vis Left", \
"vis_temp.txt" u 3:($5<=0 ? 1/0 : -$4) every ::0::end t "Enemy Vis Right"
pause -1

plot \
"loc_temp.txt" u 1:(-$2) every ::0::end t "Evolved Silent", \
"loc_temp.txt" u 3:($5<0 || $6<0 ? -$4 : 1/0) every ::0::end t "Enemy Silent Left", \
"loc_temp.txt" u 3:($5>0 || $6>0 ? -$4 : 1/0) every ::0::end t "Enemy Silent Right"
pause -1

plot \
"loc_temp.txt" u 1:(-$2) every ::0::end t "Evolved Silent", \
"loc_temp.txt" u 3:($6>=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Chase Left", \
"loc_temp.txt" u 3:($6<=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Chase Right", \
"loc_temp.txt" u 3:($5>=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Wall Left", \
"loc_temp.txt" u 3:($5<=0 ? 1/0 : -$4) every ::0::end t "Enemy Silent Wall Right"
pause -1

