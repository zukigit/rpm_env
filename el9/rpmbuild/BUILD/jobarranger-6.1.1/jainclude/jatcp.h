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


#ifndef JOBARG_JATCP_H
#define JOBARG_JATCP_H

#include <json.h>
#include "comms.h"

int ja_tcp_accept(zbx_sock_t * s);
int ja_tcp_send_to(zbx_sock_t * s, ja_job_object * job, int timeout);
int ja_tcp_recv_to(zbx_sock_t * s, ja_job_object * job, int timeout);
void ja_tcp_timeout_set(int fd, int timeout);

int ja_tcp_send(zbx_sock_t * s, int timeout, json_object * json);
int ja_tcp_recv(zbx_sock_t * s, int timeout, json_object * json);

int	ja_tcp_check_security(zbx_sock_t *s, const char *ip_list, int allow_if_empty, char *socket_ip);

#endif
