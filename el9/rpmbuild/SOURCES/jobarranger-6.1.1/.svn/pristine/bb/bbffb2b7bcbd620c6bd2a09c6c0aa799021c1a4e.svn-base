#!/bin/bash

cd `dirname $0`
docker build -t jobarg-server-mysql:centos-5.0.1 .

docker run --name jobarg-server -t \
      -e DB_SERVER_HOST="zabbix-docker_mysql-server_1.zabbix-docker_zbx_net_frontend" \
      -e MYSQL_DATABASE="zabbix" \
      -e MYSQL_USER="zabbix" \
      -e MYSQL_PASSWORD="zabbix" \
      -e JAZABBIXURL="http://192.168.137.211" \
      -e LOGTYPE="file" \
      -e DEBUGLEVEL=3 \
      -v /tmp:/tmp:rw \
      -v /etc/localtime:/etc/localtime:ro \
      -p 0.0.0.0:10061:10061 \
      --net zabbix-docker_zbx_net_frontend \
      --link zabbix-docker_mysql-server_1:mysql \
      -d jobarg-server-mysql:centos-5.0.1
