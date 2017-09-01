#!/bin/sh  
  
  
filelist=`ls /Users/yfzhineng/Documents/mine/SampleSocket/ProtoGen/*.proto`  
for file in $filelist  
do  
echo '========='+$file  
#mono /Users/yfzhineng/Documents/mine/SampleSocket/ProtoGen/protogen.exe -i:$file -o:${file%.*}.cs  
protogen -i:$file -o:${file%.*}.cs
done 