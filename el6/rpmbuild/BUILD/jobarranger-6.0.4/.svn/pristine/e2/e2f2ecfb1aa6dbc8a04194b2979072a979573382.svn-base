#!/bin/bash

cd `dirname $0`
docker build -t jobarg-server-postgres:centos-5.0.1 .

docker run --name jobarg-server -t \
      -e DB_SERVER_HOST="zabbix-docker_postgres-server_1.zabbix-docker_zbx_net_frontend" \
      -e POSTGRES_DATABASE="zabbix" \
      -e POSTGRES_USER="zabbix" \
      -e POSTGRES_PASSWORD="zabbix" \
      -e JAZABBIXURL="http://192.168.137.145" \
      -e LOGTYPE="file" \
      -e DEBUGLEVEL=3 \
      -v /tmp:/tmp:rw \
      -v /etc/localtime:/etc/localtime:ro \
      -p 0.0.0.0:10061:10061 \
      --net zabbix-docker_zbx_net_frontend \
      --link zabbix-docker_postgres-server_1:postgres \
      -d jobarg-server-postgres:centos-5.0.1
