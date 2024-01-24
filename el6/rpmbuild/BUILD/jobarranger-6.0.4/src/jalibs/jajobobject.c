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

#include "common.h"
#include "log.h"

#include "jacommon.h"

/******************************************************************************
 *                                                                            *
 * Function:                                                                  *
 *                                                                            *
 * Purpose:                                                                   *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_job_object_init(ja_job_object * job)
{
    const char *__function_name = "ja_job_object_init";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return FAIL;

    memset(job->kind, 0, sizeof(job->kind));
    job->version = 0;
    job->jobid = 0;
    memset(job->serverid, 0, sizeof(job->serverid));
    memset(job->hostname, 0, sizeof(job->hostname));
    job->method = -1;
    memset(job->type, 0, sizeof(job->type));
    memset(job->argument, 0, sizeof(job->argument));
    memset(job->script, 0, sizeof(job->script));
    memset(job->env, 0, sizeof(job->env));
    job->result = 0;
    job->status = -1;
    job->pid = 0;
    job->start_time = 0;
    job->end_time = 0;
    memset(job->message, 0, sizeof(job->message));
    memset(job->std_out, 0, sizeof(job->std_out));
    memset(job->std_err, 0, sizeof(job->std_err));
    job->return_code = -1;
    job->signal = 0;
    job->send_retry = 0;

    memset(job->run_user, 0, sizeof(job->run_user));
    memset(job->run_user_password, 0, sizeof(job->run_user_password));

    memset(job->cur_unique_id, 0,sizeof(job->cur_unique_id));
    memset(job->pre_unique_id, 0,sizeof(job->pre_unique_id));

    memset(job->serverip, 0, sizeof(job->serverip));
    //initialize host_running_job.
    // memset(job->host_running_job,0,sizeof(job->host_running_job));
    job->host_running_job=NULL;
    job->size_of_host_running_job=0;

    return SUCCEED;
}
