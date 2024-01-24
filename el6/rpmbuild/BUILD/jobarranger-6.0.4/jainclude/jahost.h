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


#ifndef JOBARG_JAHOST_H
#define JOBARG_JAHOST_H

#include "comms.h"

extern int CONFIG_AGENT_LISTEN_PORT;
extern int CONFIG_ZABBIX_VERSION;

int ja_host_getname(const zbx_uint64_t inner_job_id, const int host_flag,
                    const char *host_name, char *host);
zbx_uint64_t ja_host_getip(const char *host, char *host_ip, const zbx_uint64_t inner_job_id, int *port, int txn);
zbx_uint64_t ja_host_getip_ipchange(const char *host, char *host_ip, int *port, int txn);
int ja_host_getport(zbx_uint64_t hostid, int macro_flag);
int ja_host_auth(zbx_sock_t * sock, const char *host, const zbx_uint64_t inner_job_id);

int ja_host_lockinfo(const char *host);
int ja_host_lock(const char *host, const zbx_uint64_t inner_job_id);
int ja_host_unlock(const char *host);

#endif
