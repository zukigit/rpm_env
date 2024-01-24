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
        MESSAGE="Jobnet has not been pre-deployment: jobnet[$4]  scheduled time[$6]  calendar[$2]  schedule[$3]  user name[$5]"
        ;;
    "2")
        MESSAGE="Jobnet has not started: jobnet[$4]  scheduled time[$6]  calendar[$2]  schedule[$3]  user name[$5]"
        ;;
    *)
        exit 1
        ;;
esac

$ZABBIX_SENDER -z $ZABBIX_SERVER -p $ZABBIX_PORT -s "$HOST" -k $KEY -o "[ERROR] [JAMONITOR20000$1] $MESSAGE"
