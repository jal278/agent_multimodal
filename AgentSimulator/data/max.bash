#!/bin/bash

sum=$#
cols=$(awk '{print NF}' $1 | sort -nu | tail -n 1)
paste -d" " $* | /cygdrive/c/Program\ Files\ \(x86\)/GnuWin32/bin/nawk.exe -v s="$sum" -v c="$cols" '{
	if(end != 100) {
	    for(i=0;i<=s-1;i++)
	    {
		for(j=1;j<=c;j++)
		{
			t[j] = j+(i*c)
			if($t[j] > temp[j]) {
				temp[j] = $t[j]
			}
		}
	    }
	    if(temp[1] == 0 || temp[1] > gen) 
	    {
		gen=temp[1]
		for(j=1;j<=c;j++)
		{
			printf("%f ",temp[j])
			temp[j] = 0 
		}
		printf("\n")
	    } else {
		end=100
	    }
	}
}'
