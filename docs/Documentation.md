**USAGE**
FilesCount counts all files in a directory with the number of files for each extension found in the subdirectories.
It then displays or generates a csv report with the following report line format :

DIRNAME;FILE TYPE;FILE COUNT

**SYNTAX**
FilesCount.exe DirPath {"[ CountReportsPath [ STAMP ](-CountReportsPath-[-STAMP-) ]"}

**SAMPLES**
FilesCount.exe c:\repository\allfiles
FilesCount.exe c:\repository\allfiles c:\repositroy\reports
FilesCount.exe c:\repository\allfiles c:\repository\reports STAMP

**DETAILS**
FilesCount counts recursively each file's type in the **DirPath** folder. It ouputs the result to screen.
If **CountReportsPath** is present, it generates a .csv report in CountReportsPath directory.
If **STAMP** option is present, it sets a timestamp in the report file name.
The report contains also the total number of files in each sub-directory of DirPath. Files directly in the root of DirPath are not counted.
