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
#include "jastatus.h"
#include "jalog.h"
#include "../jarun/jaruniconjob.h"
#include "../jarun/jaruniconfwait.h"
#include "../jajob/jajobiconextjob.h"
#include "../jajob/jajobiconless.h"

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
int jajobnet_kill(const zbx_uint64_t inner_jobnet_id)
{
    DB_RESULT result;
    DB_ROW row;
    double sec;
    zbx_uint64_t inner_job_id;
    int job_status, job_type;
    int ready_cnt, run_cnt;
    int db_ret;
    const char *__function_name = "jajobnet_kill";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    result = DBselect("select jobnet_id, execution_user_name from ja_run_jobnet_table"
                      " where inner_jobnet_id = " ZBX_FS_UI64,
                      inner_jobnet_id);
    if (NULL != (row = DBfetch(result))) {
        ja_log("JAJOBNET000001", inner_jobnet_id, NULL, 0, __function_name, inner_jobnet_id, row[0], row[1]);
    }
    DBfree_result(result);

    DBfree_result(DBselect
                  ("select inner_job_id from ja_run_job_table where inner_jobnet_main_id = "
                   ZBX_FS_UI64 " for update", inner_jobnet_id));
    db_ret =
        DBexecute
        ("update ja_run_job_table set status = %d where inner_jobnet_main_id = "
         ZBX_FS_UI64 " and status = %d", JA_JOB_STATUS_BEGIN,
         inner_jobnet_id, JA_JOB_STATUS_READY);
    if (db_ret < ZBX_DB_OK)
        return FAIL;

    db_ret =
        DBexecute
        ("update ja_run_job_table set status = %d where inner_jobnet_main_id = "
         ZBX_FS_UI64 " and job_type = %d and status in (%d, %d)",
         JA_JOB_STATUS_ENDERR, inner_jobnet_id, JA_JOB_TYPE_JOBNET,
         JA_JOB_STATUS_RUN, JA_JOB_STATUS_RUNERR);
    if (db_ret < ZBX_DB_OK)
        return FAIL;

    db_ret =
        DBexecute
        ("update ja_run_jobnet_table set status = %d where inner_jobnet_main_id = "
         ZBX_FS_UI64 " and status in (%d, %d)",
         JA_JOBNET_STATUS_ENDERR, inner_jobnet_id, JA_JOBNET_STATUS_RUN,
         JA_JOBNET_STATUS_RUNERR);
    if (db_ret < ZBX_DB_OK)
        return FAIL;

    db_ret =
        DBexecute
        ("delete from ja_session_table where inner_jobnet_main_id = " ZBX_FS_UI64,
         inner_jobnet_id);
    if (db_ret < ZBX_DB_OK)
        return FAIL;

    sec = zbx_time();
    result =
        DBselect
        ("select inner_job_id, status, job_type from ja_run_job_table"
         " where inner_jobnet_main_id = " ZBX_FS_UI64
         " and status in (%d, %d)", inner_jobnet_id, JA_JOB_STATUS_READY,
         JA_JOB_STATUS_RUN);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() ja_run_job_table:(inner_jobnet_main_id: "
               ZBX_FS_UI64 ") " ZBX_FS_DBL " sec.", __function_name,
               inner_jobnet_id, sec);

    ready_cnt = 0;
    run_cnt = 0;
    while (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_job_id, row[0]);
        job_status = atoi(row[1]);
        job_type = atoi(row[2]);

        switch (job_status) {
        case JA_JOB_STATUS_READY:
            ready_cnt++;
            break;
        case JA_JOB_STATUS_RUN:
            run_cnt++;
            if (job_type == JA_JOB_TYPE_JOB) {
                jarun_icon_job(inner_job_id, JA_AGENT_METHOD_KILL);
            } else if (job_type == JA_JOB_TYPE_EXTJOB) {
                jajob_icon_extjob_kill(inner_job_id);
            } else if (job_type == JA_JOB_TYPE_FWAIT) {
                jarun_icon_fwait(inner_job_id, JA_AGENT_METHOD_KILL);
            } else if (job_type == JA_JOB_TYPE_LESS) {
                jajob_icon_less_abort(inner_job_id, JA_SES_FORCE_STOP_KILL);
            }
            break;
        default:
            break;
        }
    }
    DBfree_result(result);

    if (ready_cnt + run_cnt == 0) {
        return ja_set_jobstatus(inner_jobnet_id, JA_JOBNET_STATUS_ENDERR,
                                JA_SUMMARY_JOB_STATUS_ERROR);
    }
    ja_log("JAJOBNET000002", inner_jobnet_id, NULL, 0, __function_name,
           inner_jobnet_id, ready_cnt, run_cnt);
    return SUCCEED;
}
