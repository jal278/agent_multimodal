REM "USAGE: run-experiment-multiple.bat <experiment file> <first id number> <final id number> <generations>"
FOR /L %%A IN (%2,1,%3) DO run-experiment-single.bat %1 %%A %4