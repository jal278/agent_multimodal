
script=$1
experiment=$2
type=$3

list=""
for i in ../results/$experiment-$type-*-logfile.txt; do
list="$list $i"
done;
echo $list
