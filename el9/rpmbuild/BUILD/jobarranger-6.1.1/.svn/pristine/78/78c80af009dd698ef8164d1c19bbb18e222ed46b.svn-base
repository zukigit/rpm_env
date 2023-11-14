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

#ifndef JOBARG_JATRAPPER_H
#define JOBARG_JATRAPPER_H

#include "comms.h"

extern int CONFIG_TIMEOUT;
extern int CONFIG_TRAPPER_TIMEOUT;

#define JOBARG_EXEC_REQUEST     struct zbx_jobarg_exec_request
#define JOBARG_JOBNET_INFO      struct zbx_jobarg_jobnet_info

JOBARG_EXEC_REQUEST {
    char *username;
    char *password;
    char *jobnetid;
    char *starttime;
    char *env[1024];
    char *value[1024];
    int deterrence;
};

JOBARG_JOBNET_INFO {
    char *jobnetid;
    char *jobnetname;
    int jobnetruntype;
    int jobnetstatus;
    int jobstatus;
    zbx_uint64_t scheduled_time;
    zbx_uint64_t start_time;
    zbx_uint64_t end_time;
    char lastexitcd[JA_STD_OUT_LEN];
    char laststdout[JA_STD_OUT_LEN];
    char laststderr[JA_STD_OUT_LEN];
};

void main_jatrapper_loop(zbx_sock_t * s);

#endif
