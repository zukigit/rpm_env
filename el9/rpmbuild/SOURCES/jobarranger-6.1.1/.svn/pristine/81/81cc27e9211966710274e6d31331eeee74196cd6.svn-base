#!/bin/bash

cd `dirname $0`
if [ -f ./jobarranger-server-mysql-5.0.1-1.el8.x86_64.rpm ];then
    rm -rf jobarranger-server-mysql-5.0.1-1.el8.x86_64.rpm
fi
cp -p ../server/jobarranger-server-mysql-5.0.1-1.el8.x86_64.rpm .

docker build -t jobarg-monitor-mysql:centos-5.0.1 .
if [ "0" != "$?" ];then
    echo "Docker monitor image ERROR "
    exit 1;
fi

docker run --name jobarg-monitor -t \
      -e DB_SERVER_HOST="192.168.106.211" \
      -e MYSQL_DATABASE="zabbix" \
      -e MYSQL_USER="zabbix" \
      -e MYSQL_PASSWORD="zabbix" \
      -e LOGTYPE="file" \
      -e DEBUGLEVEL=3 \
      -v /tmp:/tmp:rw \
      -v /etc/localtime:/etc/localtime:ro \
      --net zabbix-docker_zbx_net_frontend \
      --link zabbix-docker_mysql-server_1:mysql \
      -d jobarg-monitor-mysql:centos-5.0.1

rm -rf jobarranger-server-mysql-5.0.1-1.el8.x86_64.rpm
