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


#ifndef JOBARG_JAVALUE_H
#define JOBARG_JAVALUE_H

typedef struct {
    int  len1;
    int  len2;
    char name[JA_VALUE_NAME_LEN];
    char *value;
} ja_variable;

int ja_clean_value_before(const zbx_uint64_t inner_job_id);
int ja_clean_value_after(const zbx_uint64_t inner_job_id);

int ja_set_value_before(const zbx_uint64_t inner_job_id, const zbx_uint64_t inner_jobnet_id, const char *value_name, const char *before_value);
int ja_set_value_after(const zbx_uint64_t inner_job_id, const zbx_uint64_t inner_jobnet_id, const char *value_name, const char *after_value);

int ja_cpy_value(const zbx_uint64_t inner_job_id, const char *value_src, char *value_dest);

int ja_get_value_before(const zbx_uint64_t inner_job_id, const char *value_name, char *before_value);
int ja_get_value_after(const zbx_uint64_t inner_job_id, const char *value_name, char *after_value);

int ja_remove_value_before(const zbx_uint64_t inner_job_id, const char *value_name);
int ja_remove_value_after(const zbx_uint64_t inner_job_id, const char *value_name);

int ja_value_before_after(const zbx_uint64_t inner_job_id);
int ja_value_after_before(const zbx_uint64_t inner_job_id, const zbx_uint64_t next_inner_job_id);

int ja_clean_value_jobnet_before(const zbx_uint64_t inner_jobnet_id);
int ja_clean_value_jobnet_after(const zbx_uint64_t inner_jobnet_id);

int ja_set_value_jobnet_before(const zbx_uint64_t inner_jobnet_id, const char *value_name, const char *before_value);
int ja_get_value_jobnet_before(const zbx_uint64_t inner_jobnet_id, const char *value_name, char *before_value);

int ja_value_before_jobnet_in(const zbx_uint64_t inner_job_id, const zbx_uint64_t inner_jobnet_id);
int ja_value_before_jobnet_out(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id);

int ja_get_jobnet_summary_start(const zbx_uint64_t inner_jobnet_id, char *start_time);

int ja_replace_variable(const zbx_uint64_t inner_job_id, char *value_src, char *value_dest, int dest_len);
#endif
