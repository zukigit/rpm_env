#!/bin/bash

# Job Arranger Monitor Zabbix error transmission Shell script  - 2013/06/13 -

# Parameters
# $1 : Message identification number (1:Jobnet undeployed  2:Jobnet is not started)
# $2 : Calendar ID
# $3 : Schedule ID
# $4 : Jobnet ID
# $5 : User name
# $6 : Scheduled start time

# Please specify the parameter information of Zabbix Sender.

ZABBIX_SERVER="127.0.0.1"
ZABBIX_PORT="10051"
ZABBIX_SENDER="zabbix_sender"
KEY=""
HOST=""


case "$1" in
    "1") 
        MESSAGE="ジョブネットが事前展開されていません： ジョブネット[$4]  起動予定時刻[$6]  カレンダー[$2]  スケジュール[$3]  ユーザー名[$5]"
        ;;
    "2")
        MESSAGE="ジョブネットが起動していません： ジョブネット[$4]  起動予定時刻[$6]  カレンダー[$2]  スケジュール[$3]  ユーザー名[$5]"
        ;;
    *)
        exit 1
        ;;
esac

$ZABBIX_SENDER -z $ZABBIX_SERVER -p $ZABBIX_PORT -s "$HOST" -k $KEY -o "[ERROR] [JAMONITOR20000$1] $MESSAGE"
