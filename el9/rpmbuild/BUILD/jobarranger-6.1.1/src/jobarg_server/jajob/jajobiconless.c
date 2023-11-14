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
#include "jastr.h"
#include "jalog.h"
#include "jajoblog.h"
#include "jatimeout.h"
#include "jastatus.h"
#include "jakill.h"
#include "jajobid.h"
#include "jajobiconless.h"

/******************************************************************************
 *                                                                            *
 * Function: jajob_icon_less_abort                                            *
 *                                                                            *
 * Purpose: do a forced stop of agentless icon                                *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             force_stop   (in) - force stop flag                            *
 *                                                                            *
 * Return value: return value of the ja_set_runerr function or SUCCEED        *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jajob_icon_less_abort(zbx_uint64_t inner_job_id, int force_stop)
{
    DB_RESULT    result;
    DB_ROW       row;
    zbx_uint64_t inner_jobnet_id, inner_jobnet_main_id;
    int          rc;
    JA_PID       smpid;
    char         session_id[JA_MAX_STRING_LEN];
    const char   *__function_name = "jajob_icon_less_abort";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get information of agentless icon */
    result = DBselect("select inner_jobnet_id, session_id from ja_run_icon_agentless_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);
    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONLESS200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    zbx_strlcpy(session_id, row[1], sizeof(session_id));
    DBfree_result(result);

    /* get the internal management main jobnet id */
    result = DBselect("select inner_jobnet_main_id from ja_run_jobnet_table"
                      " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONLESS200002", 0, NULL, inner_job_id, __function_name, inner_jobnet_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    ZBX_STR2UINT64(inner_jobnet_main_id, row[0]);
    DBfree_result(result);

    /* session management table record lock */
    result = DBselect("select pid from ja_session_table"
                      " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64
                      " for update",
                      session_id, inner_jobnet_main_id);
    if (NULL == (row = DBfetch(result))) {
        DBfree_result(result);
        ja_set_runerr(inner_job_id, 2);
        return SUCCEED;
    }
    smpid = atoi(row[0]);
    DBfree_result(result);
    /* session management process existing check */
    if (smpid <= 0) {
        /* process down */
        DBexecute("delete from ja_session_table"
                  " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64,
                  session_id, inner_jobnet_main_id);
        return ja_set_runerr(inner_job_id, 2);
    }
    if (ja_process_check(smpid) == FAIL) {
        /* process down */
        DBexecute("delete from ja_session_table"
                  " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64,
                  session_id, inner_jobnet_main_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    if (force_stop != JA_SES_FORCE_STOP_ON) {
        return SUCCEED;
    }
    /* the forced stop if the session management process is running */
    rc = DBexecute("update ja_session_table set force_stop = %d"
                   " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64,
                   JA_SES_FORCE_STOP_ON,
                   session_id, inner_jobnet_main_id);

    if (rc <= ZBX_DB_OK) {
        ja_log("JAJOBICONLESS200003", 0, NULL, inner_job_id, __function_name, inner_jobnet_main_id, session_id, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }
    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jajob_icon_less_timeout                                          *
 *                                                                            *
 * Purpose: monitor the time-out of agentless icon                            *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             start_time   (in) - start time of the icon (YYYYMMDDHHMMSS)    *
 *                                                                            *
 * Return value: return value of the ja_set_runerr function or SUCCEED        *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jajob_icon_less_timeout(zbx_uint64_t inner_job_id, char *start_time)
{
    DB_RESULT    result;
    DB_RESULT    result2;
    DB_ROW       row;
    DB_ROW       row2;
    zbx_uint64_t inner_jobnet_id;
    int          rc;
    char         timeout[64];
    char         w_main_jobnet_id[64];
    char         w_execution_user_name[110];
    const char   *__function_name = "jajob_icon_less_timeout";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " start_time: %s", __function_name, inner_job_id, start_time);

    /* get information of agentless icon */
    result = DBselect("select inner_jobnet_id, timeout from ja_run_icon_agentless_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONLESS200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    zbx_strlcpy(timeout, row[1], sizeof(timeout));
    DBfree_result(result);

    if (ja_timeout_check(timeout, start_time) == FAIL) {
        /* no time-out */
        return SUCCEED;
    }

    w_main_jobnet_id[0]      = '\0';
    w_execution_user_name[0] = '\0';

    /* main jobnet id and user name get */
    result = DBselect("select inner_jobnet_main_id, job_id from ja_run_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        result2 = DBselect("select jobnet_id, execution_user_name from ja_run_jobnet_table"
                           " where inner_jobnet_id = %s", row[0]);

        if (NULL != (row2 = DBfetch(result2))) {
            zbx_strlcpy(w_main_jobnet_id,      row2[0], sizeof(w_main_jobnet_id));
            zbx_strlcpy(w_execution_user_name, row2[1], sizeof(w_execution_user_name));
        }
        DBfree_result(result2);
    }
    DBfree_result(result);

    ja_log("JAJOBICONLESS300001", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id,
           timeout, start_time, w_main_jobnet_id, ja_get_jobid(inner_job_id), w_execution_user_name);

    rc = DBexecute("update ja_run_job_table set timeout_flag = 1"
                   " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (rc < ZBX_DB_OK) {
        ja_log("JAJOBICONLESS200004", 0, NULL, inner_job_id, __function_name, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    ja_joblog(JC_JOB_TIMEOUT, inner_jobnet_id, inner_job_id);

    return SUCCEED;
}
