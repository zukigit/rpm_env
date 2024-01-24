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


#ifndef JOBARG_JAFCOPY_H
#define JOBARG_JAFCOPY_H

extern int CONFIG_TIMEOUT;
extern int CONFIG_FCOPY_TIMEOUT;

typedef struct {
    ja_job_object *job;
    zbx_sock_t *s;
} ja_fcopy_args_t;

#ifdef _WINDOWS
unsigned __stdcall ja_fcopy_getfile_thread(ja_fcopy_args_t * args);
unsigned __stdcall ja_fcopy_putfile_thread(ja_fcopy_args_t * args);
#endif

int ja_fcopy_begin(ja_job_object * job, zbx_sock_t * sock);
int ja_fcopy_thread(ja_job_object * job, zbx_sock_t * sock);
int ja_fcopy_getfile(ja_job_object * job, zbx_sock_t * sock);
int ja_fcopy_putfile(ja_job_object * job, zbx_sock_t * sock);
int ja_fcopy_putfile_chksum(char *chksum, char *dir, ja_job_object * job);

#endif
