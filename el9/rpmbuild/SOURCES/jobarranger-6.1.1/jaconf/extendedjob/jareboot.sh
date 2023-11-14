#!/bin/bash

echo "[`date '+%Y/%m/%d %H:%M:%S'`] Wait for the completion of the job."
# echo "check reboot flag file: '$1' ..."
while [ -e "$1" ]; do
    sleep 1
done

echo "[`date '+%Y/%m/%d %H:%M:%S'`] Start the reboot."
sync
sync
sync
/sbin/shutdown -r now
