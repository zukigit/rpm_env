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
#include "cfg.h"
#include "log.h"
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastr.h"
#include "jaenv.h"
#include "javalue.h"
#include "jastatus.h"
#include "jajobid.h"
#include "jaruniconless.h"

extern char *CONFIG_EXTJOB_PATH;

/******************************************************************************
 *                                                                            *
 * Function: load_icon_less                                                   *
 *                                                                            *
 * Purpose: do the execution of the agentless icon                            *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             test_flag (in) - job test flag                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_less(const zbx_uint64_t inner_job_id, const int test_flag)
{
    DB_RESULT    result;
    DB_ROW       row;
    int          session_flag, operation_flag, status, rc, count, state;
    zbx_uint64_t inner_jobnet_id, inner_jobnet_main_id;
    char         session_id[JA_MAX_STRING_LEN];
    char         cmd[JA_MAX_STRING_LEN];
    const char   *__function_name = "jarun_icon_less";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get information of agentless icon */
    result = DBselect("select inner_jobnet_id, session_flag, session_id"
                      " from ja_run_icon_agentless_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONLESS200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    session_flag = atoi(row[1]);
    zbx_strlcpy(session_id, row[2], sizeof(session_id));
    DBfree_result(result);

    /* get the internal management main jobnet id */
    result = DBselect("select inner_jobnet_main_id from ja_run_jobnet_table"
                      " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONLESS200002", 0, NULL, inner_job_id, __function_name, inner_jobnet_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_main_id, row[0]);
    DBfree_result(result);

    switch (session_flag) {
        case JA_SESSION_FLAG_ONETIME:
        case JA_SESSION_FLAG_CONNECT:
             if (session_flag == JA_SESSION_FLAG_ONETIME) {
                 /* one-time connection */
                 operation_flag = JA_SES_OPERATION_FLAG_ONETIME;
             }
             else {
                 /* terminal connection */
                 operation_flag = JA_SES_OPERATION_FLAG_CONNECT;
             }

             /* session management table record lock */
             result = DBselect("select count(*) from ja_session_table"
                               " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64,
                               session_id, inner_jobnet_main_id);

             if (NULL == (row = DBfetch(result))) {
                 ja_log("JARUNICONLESS200008", 0, NULL, inner_job_id, __function_name, session_id, inner_jobnet_main_id);
                 DBfree_result(result);
                 return ja_set_runerr(inner_job_id, 2);
             }
             count = atoi(row[0]);
             DBfree_result(result);

             /* session id double registration check */
             if (count != 0) {
                 ja_log("JARUNICONLESS200004", 0, NULL, inner_job_id, __function_name, inner_jobnet_main_id, session_id, ja_get_jobid(inner_job_id));
                 return ja_set_runerr(inner_job_id, 2);
             }

             /* session management data registration */
             rc = DBexecute("insert into ja_session_table ("
                            " session_id, inner_jobnet_main_id, inner_job_id, operation_flag, status, force_stop)"
                            " values ('%s', " ZBX_FS_UI64 ", " ZBX_FS_UI64 ", %d, %d, %d)",
                            session_id, inner_jobnet_main_id, inner_job_id, operation_flag, JA_SES_STATUS_BEGIN, JA_SES_FORCE_STOP_OFF);

             if (rc <= ZBX_DB_OK) {
                 ja_log("JARUNICONLESS200009", 0, NULL, inner_job_id, __function_name, session_id, inner_jobnet_main_id);
                 return ja_set_runerr(inner_job_id, 2);
             }

             /* session management data commit */
             DBcommit();
             DBbegin();

             /* start the session management process */
             zbx_snprintf(cmd, sizeof(cmd), "%s/jobarg_session -S '%s' -r '" ZBX_FS_UI64 "' -T '%d' -c '%s' &",
                          CONFIG_EXTJOB_PATH, session_id, inner_jobnet_main_id, test_flag, CONFIG_FILE);
            rc = ja_system_call(cmd);
             zabbix_log(LOG_LEVEL_DEBUG, "jobarg_session execution [%s] (%d)", cmd, rc);
             if (rc != EXIT_SUCCESS) {
                 if (WIFEXITED(rc)) {
                     state = WEXITSTATUS(rc);
                 }
                 else {
                     state = rc;
                 }
                 ja_log("JARUNICONLESS200003", 0, NULL, inner_job_id, __function_name, state, cmd);
                 return ja_set_runerr(inner_job_id, 2);
             }
             break;

        case JA_SESSION_FLAG_CONTINUE:
        case JA_SESSION_FLAG_CLOSE:
             /* session management table record lock */
             result = DBselect("select status from ja_session_table"
                               " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64
                               " for update",
                               session_id, inner_jobnet_main_id);

             if (NULL == (row = DBfetch(result))) {
                 ja_log("JARUNICONLESS200005", 0, NULL, inner_job_id, __function_name, inner_jobnet_main_id, session_id, ja_get_jobid(inner_job_id));
                 DBfree_result(result);
                 return ja_set_runerr(inner_job_id, 2);
             }
             status = atoi(row[0]);
             DBfree_result(result);

             /* session state check */
             if (status != JA_SES_STATUS_END) {
                 /* other agentless icons use a session */
                 ja_log("JARUNICONLESS200010", 0, NULL, inner_job_id, __function_name, inner_jobnet_main_id, session_id, ja_get_jobid(inner_job_id));
                 return ja_set_runerr(inner_job_id, 2);
             }

             if (session_flag == JA_SESSION_FLAG_CONTINUE) {
                 /* command execution */
                 operation_flag = JA_SES_OPERATION_FLAG_CONTINUE;
             }
             else {
                 /* terminal disconnection */
                 operation_flag = JA_SES_OPERATION_FLAG_CLOSE;
             }

             /* session management data update */
             rc = DBexecute("update ja_session_table set"
                            " inner_job_id = " ZBX_FS_UI64 ", operation_flag = %d, status = %d, force_stop = %d"
                            " where session_id = '%s' and inner_jobnet_main_id = " ZBX_FS_UI64,
                            inner_job_id, operation_flag, JA_SES_STATUS_BEGIN, JA_SES_FORCE_STOP_OFF,
                            session_id, inner_jobnet_main_id);

             if (rc <= ZBX_DB_OK) {
                 ja_log("JARUNICONLESS200006", 0, NULL, inner_job_id, __function_name, inner_job_id, session_id, inner_job_id);
                 return ja_set_runerr(inner_job_id, 2);
             }
             break;

        default:
             ja_log("JARUNICONLESS200007", 0, NULL, inner_job_id, __function_name, inner_job_id, session_id, inner_job_id, session_flag);
             return ja_set_runerr(inner_job_id, 2);
    }

    return SUCCEED;
}
