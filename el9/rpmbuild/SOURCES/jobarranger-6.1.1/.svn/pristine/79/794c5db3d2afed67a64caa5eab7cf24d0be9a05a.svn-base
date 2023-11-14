#!/usr/bin/sh

echo "[`date '+%Y/%m/%d %H:%M:%S'`] Wait for the completion of the job."
# echo "check reboot flag file: '$1' ..."
while [ -e "$1" ]; do
    /usr/bin/sleep 1
done

echo "[`date '+%Y/%m/%d %H:%M:%S'`] Start the reboot."
/usr/sbin/sync
/usr/sbin/sync
/usr/sbin/sync
/usr/sbin/shutdown -Fr now
