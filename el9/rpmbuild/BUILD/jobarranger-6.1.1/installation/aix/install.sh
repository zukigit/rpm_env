#!/bin/sh

# install - install a Job Arranger Agent for AIX  - 2014/12/12 -
#
# Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
#
# This installation shell does the standard installation of Job Arranger
# Agent for AIX. Advanced options I can be found in the "install.sh -h".
#
# This shell will create the following directory.
#
#  /etc/jobarranger
#  /var/lib/jobarranger
#  /var/log/jobarranger
#  /var/run/jobarranger
#
# This shell is to copy the files to the following directory.
#
#  ./bin/jobarg_agentd    --> /usr/sbin/.
#  ./bin/jobarg_exec      --> /usr/bin/.
#  ./bin/jobarg_get       --> /usr/bin/.
#  ./bin/jobarg_joblogput --> /usr/bin/.
#  ./bin/jobarg_release   --> /usr/bin/.
#
#  ./etc/jobarranger/extendedjob/*      --> /etc/jobarranger/extendedjob/.
#  ./etc/jobarranger/jobarg_agentd.conf --> /etc/jobarranger/.
#  ./etc/rc.d/init.d/jobarg-agentd      --> /etc/rc.d/init.d/.
#
# Create the following symbolic files.
#
#  /etc/rc.d/init.d/jobarg-agentd --> /etc/rc.d/rc2.d/S999jobarg-agentd


# Define the installation environment.

BIN_DIR="/usr/bin"
SBIN_DIR="/usr/sbin"

ETC_DIR="/etc/jobarranger"
EXTJOB_DIR="${ETC_DIR=}/extendedjob"
INIT_DIR="/etc/rc.d/init.d"
START_UP_DIR="/etc/rc.d/rc2.d"

LIB_DIR="/var/lib/jobarranger"
LOG_DIR="/var/log/jobarranger"
PID_DIR="/var/run/jobarranger"


# Define the usage message.

usage="\
Usage: $0
Options:
     -h   display this help and exit.
"


# Check the parameters.

if [ $# -ne 0 ] 
then
    if [ "$1" = "-h" ] 
    then
        echo "$usage"
        exit 0
    else
        echo "invalid parameter was specified. Please Type in the '$0 -h'."
        exit 0
    fi
fi


# Create a directory.

/usr/bin/mkdir -p ${EXTJOB_DIR}
if [ $? -ne 0 ] 
then
    echo "failed to create the directory [${EXTJOB_DIR}]"
    exit -1
fi

/usr/bin/mkdir -p ${LIB_DIR}/tmp
if [ $? -ne 0 ] 
then
    echo "failed to create the directory [${LIB_DIR}/tmp]"
    exit -1
fi

/usr/bin/mkdir -p ${LOG_DIR}
if [ $? -ne 0 ] 
then
    echo "failed to create the directory [${LOG_DIR}]"
    exit -1
fi

/usr/bin/mkdir -p ${PID_DIR}
if [ $? -ne 0 ] 
then
    echo "failed to create the directory [${PID_DIR}]"
    exit -1
fi


# Copy the executable file.

/usr/bin/cp ./bin/jobarg_agentd ${SBIN_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./bin/jobarg_agentd file"
    exit -1
fi

/usr/bin/cp ./bin/jobarg_exec ${BIN_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./bin/jobarg_exec file"
    exit -1
fi

/usr/bin/cp ./bin/jobarg_get ${BIN_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./bin/jobarg_get file"
    exit -1
fi

/usr/bin/cp ./bin/jobarg_joblogput ${BIN_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./bin/jobarg_joblogput file"
    exit -1
fi

/usr/bin/cp ./bin/jobarg_release ${BIN_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./bin/jobarg_release file"
    exit -1
fi

/usr/bin/cp ./etc/jobarranger/jobarg_agentd.conf ${ETC_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./etc/jobarranger/jobarg_agentd.conf file"
    exit -1
fi

/usr/bin/cp ./etc/jobarranger/extendedjob/* ${EXTJOB_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./etc/jobarranger/extendedjob/* file"
    exit -1
fi

/usr/bin/cp ./etc/rc.d/init.d/jobarg-agentd ${INIT_DIR}/.
if [ $? -ne 0 ] 
then
    echo "failed to copy the ./etc/rc.d/init.d/jobarg-agentd file"
    exit -1
fi

ln -f -s ${INIT_DIR}/jobarg-agentd ${START_UP_DIR}/S999jobarg-agentd
if [ $? -ne 0 ] 
then
    echo "failed to create a symbolic link of ${START_UP_DIR}/S999jobarg-agentd file"
    exit -1
fi


# The grant execute permission.

/usr/bin/chmod 755 ${SBIN_DIR}/jobarg_agentd
/usr/bin/chmod 755 ${BIN_DIR}/jobarg_exec
/usr/bin/chmod 755 ${BIN_DIR}/jobarg_get
/usr/bin/chmod 755 ${BIN_DIR}/jobarg_joblogput
/usr/bin/chmod 755 ${BIN_DIR}/jobarg_release
/usr/bin/chmod 755 ${EXTJOB_DIR}/*
/usr/bin/chmod 755 ${INIT_DIR}/jobarg-agentd


echo "Job Arranger Agent installation is complete."
echo "Please set up ${ETC_DIR}/jobarg_agentd.conf file."
exit 0
