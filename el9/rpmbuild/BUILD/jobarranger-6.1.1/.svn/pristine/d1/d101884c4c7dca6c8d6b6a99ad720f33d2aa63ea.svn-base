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


#ifndef JOBARG_JASTATUS_H
#define JOBARG_JASTATUS_H

int ja_set_jobstatus(const zbx_uint64_t inner_jobnet_id, const int status,
                     const int jobstatus);
int ja_set_status_jobnet_summary(const zbx_uint64_t inner_jobnet_id,
                                 const int status, const int start,
                                 const int end);
int ja_set_status_jobnet(const zbx_uint64_t inner_jobnet_id,
                         const int status, const int start, const int end);
int ja_set_status_job(const zbx_uint64_t inner_job_id, const int status,
                      const int start, const int end);

int ja_get_status_jobnet_summary(const zbx_uint64_t inner_jobnet_id);
int ja_get_status_jobnet(const zbx_uint64_t inner_jobnet_id);
int ja_get_status_job(const zbx_uint64_t inner_job_id);

int ja_set_run_jobnet(const zbx_uint64_t inner_jobnet_id);
int ja_set_end_jobnet(const zbx_uint64_t inner_jobnet_id);
int ja_set_runerr_jobnet(const zbx_uint64_t inner_jobnet_id);
int ja_set_enderr_jobnet(const zbx_uint64_t inner_jobnet_id);

int ja_set_run(const zbx_uint64_t inner_job_id);
int ja_set_end(const zbx_uint64_t inner_job_id, int msg_flag);
int ja_set_runerr(const zbx_uint64_t inner_job_id, int icon_status);
int ja_set_enderr(const zbx_uint64_t inner_job_id, int msg_flag);
int ja_set_run_job_id(const zbx_uint64_t inner_job_id);

#endif
