
script=$1
type=$2

list=""
for i in $type?; do
end="logfile.txt"
list="$list $i/$i$end"
done;

#for i in $type??; do
#if [ "${i}" != "${type}??" ]
#then
#	end="logfile.txt"
#	list="$list $i/$i$end"
#fi;
#done;

#echo $list
bash $script $list
