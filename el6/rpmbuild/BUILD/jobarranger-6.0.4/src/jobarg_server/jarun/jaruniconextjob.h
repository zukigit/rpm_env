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


#ifndef JOBARG_JARUNICONEXTJOB_H
#define JOBARG_JARUNICONEXTJOB_H

/* day of the week */
typedef struct {
    int    sun;
    int    mon;
    int    tue;
    int    wed;
    int    thu;
    int    fri;
    int    sat;
} ja_week_t;

int ja_get_zbxsnd_sender(char *cmd);
int ja_get_sleep_time(char *des_sec, char *sleep_time);
int ja_check_wait_time(char *data);
int ja_get_wait_time(char *des_time, char *start_time, char *wait_time);
int ja_check_week(char *value, ja_week_t *jw);
int jarun_icon_extjob_sleep(const zbx_uint64_t inner_job_id, char *value);
int jarun_icon_extjob_time(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, char *value);
int jarun_icon_extjob_week(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, char *value);
int jarun_icon_extjob_zbxsender(const zbx_uint64_t inner_job_id, char *value);
int jarun_icon_extjob(const zbx_uint64_t inner_job_id, const int test_flag);

#endif
