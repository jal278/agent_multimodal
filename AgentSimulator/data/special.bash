#!/bin/bash

sum=$#
cols=$(awk '{print NF}' $1 | sort -nu | tail -n 1)

i=0
list=""
NUMBERS=""
for ARG in $*
do
        awk '{print $1 "\t" $12}' $ARG > temp.$i
	list="$list temp.$i"
	NUMBERS="$NUMBERS $i"
	i=$(($i+1)) 
done
bash average.bash $list > temp.averages
bash standardError.bash $list > temp.standardErrors
for i in `echo $NUMBERS`
do
	rm temp.$i
done

paste -d" " temp.averages temp.standardErrors

rm temp.averages
rm temp.standardErrors
