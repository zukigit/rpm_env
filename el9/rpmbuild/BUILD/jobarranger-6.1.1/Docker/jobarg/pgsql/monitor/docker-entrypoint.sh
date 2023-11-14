#!/bin/bash

set -e

escape_spec_char() {
    local var_value=$1

    var_value="${var_value//\\/\\\\}"
    var_value="${var_value//[$'\n']/}"
    var_value="${var_value//\//\\/}"
    var_value="${var_value//./\\.}"
    var_value="${var_value//\*/\\*}"
    var_value="${var_value//^/\\^}"
    var_value="${var_value//\$/\\\$}"
    var_value="${var_value//\&/\\\&}"
    var_value="${var_value//\[/\\[}"
    var_value="${var_value//\]/\\]}"

    echo $var_value
}

update_config_var() {
    local config_path=$1
    local var_name=$2
    local var_value=$3
    local is_multiple=$4

    if [ ! -f "$config_path" ]; then
        echo "**** Configuration file '$config_path' does not exist"
        return
    fi

    echo -n "** Updating '$config_path' parameter \"$var_name\": '$var_value'... "

    # Remove configuration parameter definition in case of unset parameter value
    if [ -z "$var_value" ]; then
        sed -i -e "/^$var_name=/d" "$config_path"
        echo "removed"
        return
    fi

    # Remove value from configuration parameter in case of double quoted parameter value
    if [ "$var_value" == '""' ]; then
        sed -i -e "/^$var_name=/s/=.*/=/" "$config_path"
        echo "NULL"
        return
    fi

    # Escaping characters in parameter value
    var_value=$(escape_spec_char "$var_value")

    if [ "$(grep -E "^$var_name=" $config_path)" ] && [ "$is_multiple" != "true" ]; then
        sed -i -e "/^$var_name=/s/=.*/=$var_value/" "$config_path"
        echo "updated"
    elif [ "$(grep -Ec "^# $var_name=" $config_path)" -gt 1 ]; then
        sed -i -e  "/^[#;] $var_name=$/i\\$var_name=$var_value" "$config_path"
        echo "added first occurrence"
    else
        sed -i -e "/^[#;] $var_name=/s/.*/&\n$var_name=$var_value/" "$config_path"
        echo "added"
    fi

}
check_db_connect_postgresql() {
    WAIT_TIMEOUT=5
    LOOP_MAX_CNT=20
    LOOP_CNT=0
    DB_SERVER_ROOT_USER="postgres"
    export PGPASSWORD=${POSTGRES_PASSWORD}
    echo "***************It connct to DB START ******************"

    while true 
    do
        echo "loop +++ "${LOOP_CNT}
        psql --host ${DB_SERVER_HOST} --port ${DB_SERVER_PORT} -U ${POSTGRES_USER} ${POSTGRES_DATABASE} --list --quiet 1>/dev/null 2>&1 && break

        echo "**** PostgreSQL monitor is not available. Waiting $WAIT_TIMEOUT seconds..."
        sleep $WAIT_TIMEOUT
        if [ $LOOP_MAX_CNT -lt $LOOP_CNT ];then
            echo "***************It connot connct to DB******************"
            exit 1
        else
            LOOP_CNT=$(($LOOP_CNT + 1))
        fi
    done
    echo "***************It connct to DB END ******************"

}
psql_query(){
    query=$1
    export PGPASSWORD=${POSTGRES_PASSWORD}
    result=`psql --no-align --quiet --tuples-only --host=${DB_SERVER_HOST} --port=${DB_SERVER_PORT} --username=${DB_SERVER_ZBX_USER} --dbname=${DB_SERVER_DBNAME} --command "$query"`
    echo $result
}


echo "***********LOG START*********"

JOBARG=/usr/sbin/jobarg_monitor
JOBARRANGER_ETC_DIR=/etc/jobarranger
JAZ_CONFIG=${JOBARRANGER_ETC_DIR}/jobarg_monitor.conf

LOGTYPE=${LOGTYPE:-"console"}
DB_SERVER_PORT=${DB_SERVER_PORT:-5432}
DEBUGLEVEL=${DEBUGLEVEL:-"3"}
JALOGfILE=${JALOGFILE:-"/tmp/jobarg_monitor.log"}
JAPIDFILE=${JAPIDFILE:-"/tmp/jobarg_monitor.pid"}


update_config_var $JAZ_CONFIG "LogType" "${LOGTYPE}"
update_config_var $JAZ_CONFIG "DBHost" "${DB_SERVER_HOST}"
update_config_var $JAZ_CONFIG "DBName" "${POSTGRES_DATABASE}"
update_config_var $JAZ_CONFIG "DBSchema" "${DB_SERVER_SCHEMA}"
update_config_var $JAZ_CONFIG "DBUser" "${POSTGRES_USER}"
update_config_var $JAZ_CONFIG "DBPort" "${DB_SERVER_PORT}"
update_config_var $JAZ_CONFIG "DBPassword" "${POSTGRES_PASSWORD}"
update_config_var $JAZ_CONFIG "DebugLevel" "${DEBUGLEVEL}"
update_config_var $JAZ_CONFIG "JaLogFile" "${JALOGfILE}"
update_config_var $JAZ_CONFIG "JaSendRetry" "${JASENDRETRY}"
update_config_var $JAZ_CONFIG "JaPidFile" "${JAPIDFILE}"

echo -e "\n\nTmpDir=/tmp/" >> $JAZ_CONFIG


check_db_connect_postgresql

result=$(psql_query "SELECT count(*) FROM pg_tables where tablename like 'ja%'")
if [ "$result" = "0" ] || [ "$result" = "" ];then
    export LANG=C.UTF-8
    export PGPASSWORD=${POSTGRES_PASSWORD}
    echo "***********CREATE DATABASE START***********[${result}] "
    cat /usr/share/doc/jobarranger-server-postgresql/database/postgresql/PostgreSQL_JA_CREATE_TABLE.sql | psql --quiet \
                --host ${DB_SERVER_HOST} --port ${DB_SERVER_PORT} \
                --username=${DB_SERVER_ZBX_USER} --dbname=${DB_SERVER_DBNAME} 1>/dev/null || exit 1
    cat /usr/share/doc/jobarranger-server-postgresql/database/data/JA_INSERT_TABLE.sql | psql --quiet \
                --host ${DB_SERVER_HOST} --port ${DB_SERVER_PORT} \
                --username=${DB_SERVER_ZBX_USER} --dbname=${DB_SERVER_DBNAME} 1>/dev/null || exit 1
    echo "***********CREATE DATABASE END ***********"
else
    if expr "$result" : "[0-9]*$" >&/dev/null;then
        echo "*********There are [${result}] tables *******"
    else
        echo "********* create table error *******[${result}]*******"
        exit 1
    fi
fi

${JOBARG} -fc ${JAZ_CONFIG}
