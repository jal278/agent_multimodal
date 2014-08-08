REM "USAGE: run-experiment-single.bat <experiment file> <id number> <generations>"
mkdir %1-%2
AgentSimulator.exe evolve -experiment %1.xml -folder %1-%2/%1-%2 -rng_seed %2 -generations %3