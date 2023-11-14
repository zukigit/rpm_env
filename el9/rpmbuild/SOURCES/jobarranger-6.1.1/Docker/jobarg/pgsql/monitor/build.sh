#!/bin/bash

cd `dirname $0`
if [ -f ./jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm ];then
    rm -rf jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm
fi
cp -p ../server/jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm .

docker build -t jobarg-monitor-postgres:centos-5.0.1 .
if [ "0" != "$?" ];then
    echo "Docker monitor image ERROR "
    exit 1;
fi

docker run --name jobarg-monitor -t \
      -e DB_SERVER_HOST="zabbix-docker_postgres-server_1.zabbix-docker_zbx_net_frontend" \
      -e POSTGRES_DATABASE="zabbix" \
      -e POSTGRES_USER="zabbix" \
      -e POSTGRES_PASSWORD="zabbix" \
      -e LOGTYPE="file" \
      -v /tmp:/tmp:rw \
      -v /etc/localtime:/etc/localtime:ro \
      --net zabbix-docker_zbx_net_frontend \
      --link zabbix-docker_postgres-server_1:postgres \
      -d jobarg-monitor-postgres:centos-5.0.1

rm -rf jobarranger-server-postgresql-5.0.1-1.el8.x86_64.rpm
