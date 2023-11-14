@echo off
date /T
echo "check reboot flag file: '%1' ..."

:LOOP
if not exist %1 (goto EXIT)
ping -n 2 localhost > NUL
goto LOOP
:EXIT

date /T
shutdown -r -f -t 10