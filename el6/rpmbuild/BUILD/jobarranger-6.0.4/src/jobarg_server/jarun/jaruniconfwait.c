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


#include <json.h>
#include "common.h"
#include "comms.h"
#include "log.h"
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastatus.h"
#include "javalue.h"
#include "jaflow.h"
#include "javalue.h"
#include "jajobobject.h"
#include "jarunagent.h"
#include "jaruniconfwait.h"

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
int jarun_icon_fwait(const zbx_uint64_t inner_job_id, const int method)
{
    DB_RESULT result;
    DB_ROW row;
    ja_job_object job;
    json_object *jp_arg;
    zbx_uint64_t inner_jobnet_id;
    int host_flag, fwait_mode_flag;
    char host_name[128];
    char file_name[JA_MAX_STRING_LEN];
    const char *__function_name = "jarun_icon_fwait";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    ja_job_object_init(&job);
    result = DBselect
        ("select inner_jobnet_id, host_flag, fwait_mode_flag, file_delete_flag, file_wait_time, host_name, file_name"
         " from ja_run_icon_fwait_table where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);
    row = DBfetch(result);
    if (row != NULL) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        host_flag = atoi(row[1]);
        fwait_mode_flag = atoi(row[2]);
        zbx_snprintf(host_name, sizeof(host_name), "%s", row[5]);
        if (ja_cpy_value(inner_job_id, row[6], file_name) == FAIL) {
            ja_log("JARUNICONFWAIT200002", 0, NULL, inner_job_id,
                   __function_name, row[6], inner_job_id);
            DBfree_result(result);
            return ja_set_runerr(inner_job_id, 2);
        }

        jp_arg = json_object_new_array();
        json_object_array_add(jp_arg, json_object_new_string(file_name));
        json_object_array_add(jp_arg, json_object_new_string(row[3]));
        json_object_array_add(jp_arg, json_object_new_string(row[4]));
        zbx_snprintf(job.argument, sizeof(job.argument), "%s",
                     json_object_to_json_string(jp_arg));
        json_object_put(jp_arg);
    } else {
        ja_log("JARUNICONFWAIT200001", 0, NULL, inner_job_id,
               __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    DBfree_result(result);

    job.jobid = inner_job_id;
    job.method = method;
    if (job.method == JA_AGENT_METHOD_NORMAL || job.method == JA_AGENT_METHOD_TEST) {
        zbx_snprintf(job.type, sizeof(job.type), "%s", JA_PROTO_VALUE_EXTJOB);
        if (fwait_mode_flag == 0) {
            zbx_snprintf(job.script, sizeof(job.script), "jafwait");
        } else {
            zbx_snprintf(job.script, sizeof(job.script), "jafcheck");
        }
    }

    return jarun_agent(&job, host_name, host_flag, inner_job_id);
}
