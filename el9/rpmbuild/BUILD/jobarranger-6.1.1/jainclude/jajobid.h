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

#ifndef JOBARG_JAJOBID_H
#define JOBARG_JAJOBID_H

/* run icon information */
typedef struct {
    char        main_jobnet_id[JA_JOBNET_ID_LEN];
    char        execution_user_name[JA_USER_NAME_LEN];
    char        job_id[JA_MAX_DATA_LEN + 1];
} ja_icon_info_t;

int ja_get_icon_info(zbx_uint64_t inner_job_id, ja_icon_info_t *icon);
char *ja_get_jobid(const zbx_uint64_t inner_job_id);

#endif
