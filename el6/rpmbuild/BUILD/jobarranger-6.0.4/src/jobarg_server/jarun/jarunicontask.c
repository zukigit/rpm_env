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
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastatus.h"
#include "jaflow.h"
#include "jaindex.h"

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
int jarun_icon_task(const zbx_uint64_t inner_job_id, const int test_flag)
{
    int db_ret, run_type;
    char *inner_jobnet_id;
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t submit_inner_jobnet_id;
    char submit_jobnet_id[40];
    const char *__function_name = "jarun_icon_task";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (test_flag == JA_JOB_TEST_FLAG_OFF)
        run_type = JA_JOBNET_RUN_TYPE_IMMEDIATE;
    else
        run_type = JA_JOBNET_RUN_TYPE_TEST;

    // get submit_inner_jobnet_id, submit_jobnet_id
    result =
        DBselect
        ("select submit_jobnet_id, inner_jobnet_id"
         " from ja_run_icon_task_table where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        zbx_snprintf(submit_jobnet_id, sizeof(submit_jobnet_id), "%s", row[0]);
        inner_jobnet_id = zbx_strdup(NULL, row[1]);
    } else {
        ja_log("JARUNICONTASK200001", 0, NULL, inner_job_id,
               __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    DBfree_result(result);

    // get submit_inner_jobnet_id
    submit_inner_jobnet_id = get_next_id(JA_RUN_ID_JOBNET_EX, 0, NULL, inner_job_id);
    if (submit_inner_jobnet_id == 0) {
        zbx_free(inner_jobnet_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    // get execution_user_name
    result =
        DBselect
        ("select execution_user_name from ja_run_jobnet_table where inner_jobnet_id = %s",
        inner_jobnet_id);
    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONTASK200005", 0, NULL, inner_job_id,
               __function_name, inner_jobnet_id);
        zbx_free(inner_jobnet_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    zbx_free(inner_jobnet_id);

    // insert the submit_inner_jobnet_id
    db_ret = DBexecute
        ("insert into ja_run_jobnet_table "
         "(inner_jobnet_id, inner_jobnet_main_id, inner_job_id,"
         " update_date, run_type, main_flag, status,"
         " public_flag, jobnet_id, user_name, jobnet_name, memo, execution_user_name)"
         " (select " ZBX_FS_UI64 ", " ZBX_FS_UI64
         ", 0, update_date, %d, 0, %d, public_flag, jobnet_id, user_name, jobnet_name, memo, '%s'"
         " from ja_jobnet_control_table where valid_flag = 1 and jobnet_id = '%s')",
         submit_inner_jobnet_id, submit_inner_jobnet_id, run_type, 0, row[0],
         submit_jobnet_id);

    if (db_ret < ZBX_DB_OK) {
        DBfree_result(result);
        return FAIL;
    }

    if (db_ret == ZBX_DB_OK) {
        ja_log("JARUNICONTASK200004", 0, NULL, inner_job_id,
               __function_name, submit_jobnet_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    DBfree_result(result);
    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}
