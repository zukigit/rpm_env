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


#ifndef JOBARG_JAREBOOT_H
#define JOBARG_JAREBOOT_H

extern char *CONFIG_REBOOT_FLAG;
extern char *CONFIG_TMPDIR;
extern int CONFIG_TIMEOUT;

typedef struct {
    int reboot_mode_flag;
    int reboot_wait_time;
} ja_reboot_arg;

void ja_reboot_killall(ja_file_object* files, int num, JA_PID rbt_pid);
int ja_reboot_load_arg(ja_job_object * job, ja_reboot_arg * arg);
int ja_reboot_chkend(ja_job_object * job, char* file_prefix);

#endif
