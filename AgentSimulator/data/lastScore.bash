#!/bin/bash

#sum=$#
allScores=""
for ARG in $*
do
	#echo $ARG
	#output=$(awk 'END {print $1 "\t" $12}' $ARG) 
	#echo $ARG ":" $output
	score=$(awk 'END {print $12}' $ARG)
	echo $score
	allScores="$allScores$score, "
done
echo "ALL"
echo $allScores
echo "AVG"
bash special.bash $* | awk 'END {print $2}'
echo "Sample Variance"
sSquared=$(bash sampleVariance.bash $* | awk 'END {print $12}')
echo $sSquared
echo "Sample Standard Deviation"
echo "sqrt ( $sSquared )" | bc -l
echo "Standard Error"
bash special.bash $* | awk 'END {print $4}'
