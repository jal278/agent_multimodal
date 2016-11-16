#!/bin/bash

#sum=$#
allScores=""
for ARG in $*
do
	#echo $ARG
	#output=$(awk 'END {print $1 "\t" $12}' $ARG) 
	#echo $ARG ":" $output
	score=$(awk '{if($2 != 1) x = $1} END {print (x+1)}' $ARG)
	echo $score
	allScores="$allScores$score, "
done
echo "ALL"
echo $allScores
