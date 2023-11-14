/*
** Job Arranger for ZABBIX
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
** Copyright (C) 2021 Daiwa Institute of Research Ltd. All Rights Reserved.
**
** This program is free software; you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation; either version 2 of the License, or
** (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software
** Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
**/


#ifndef JOBARG_AGENTD_H
#define JOBARG_AGENTD_H

int   CONFIG_LOG_LEVEL              = LOG_LEVEL_WARNING;

char *CONFIG_HOSTS_ALLOWED          = NULL;
int   CONFIG_SERVER_PORT            = JA_DEFAULT_SERVER_PORT;

char *CONFIG_LISTEN_IP              = NULL;
int   CONFIG_LISTEN_PORT            = JA_DEFAULT_AGENT_PORT;

char *CONFIG_SOURCE_IP              = NULL;
char *CONFIG_HOSTNAME               = NULL;
int   CONFIG_SEND_RETRY             = 30;

int   CONFIG_JOB_HISTORY            = 1;
int   CONFIG_BACKUP_TIME            = 24;
int   CONFIG_BACKUP_RUN_TIME        = 24;

char *CONFIG_TMPDIR                 = NULL;
char *CONFIG_EXTJOB_PATH            = NULL;
char *CONFIG_REQUEST_FLAG           = NULL;
char *CONFIG_CMD_FILE               = NULL;
char *CONFIG_REBOOT_FLAG            = NULL;
char *CONFIG_REBOOT_FILE            = NULL;

int   CONFIG_UNSAFE_USER_PARAMETERS = 1;
int   CONFIG_ENABLE_REMOTE_COMMANDS = 1;
int   CONFIG_LOG_REMOTE_COMMANDS    = 1;
int   CONFIG_FCOPY_TIMEOUT          = 180;
int   CONFIG_LISTEN_RETRY           = 30;
char *CONFIG_JA_EXECUTION_USER      = NULL;

char *CONFIG_JA_COMMAND_USER        = NULL;
char *CONFIG_JA_COMMAND_PASSWORD    = NULL;
int   CONFIG_JA_EXEC_LOOP_TIMEOUT   =1800;
int   CONFIG_JA_LISTEN_LOOP_TIMEOUT =300;
int   CONFIG_JA_BKUP_LOOP_TIMEOUT   =300;

char *CONFIG_JA_PS_COMMAND          = "ps -ef | awk '{ print $2,$3 }'";

#endif
