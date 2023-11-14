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
#include "jatimeout.h"
#include "jastatus.h"
#include "jalog.h"
#include "jajoblog.h"
#include "jajobid.h"
#include "jajobiconjob.h"
#include "../jarun/jaruniconjob.h"

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
int jajob_icon_job_abort(zbx_uint64_t inner_job_id)
{
    DB_RESULT  result;
    DB_ROW     row;
    int        ret;
    const char *__function_name = "jajob_icon_job_abort";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    ret = FAIL;
    result = DBselect("select stop_flag from ja_run_icon_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        if (atoi(row[0]) == 0) {
            ret = jarun_icon_job(inner_job_id, JA_AGENT_METHOD_KILL);
        }
        else {
            ret = jarun_icon_job(inner_job_id, JA_AGENT_METHOD_ABORT);
        }
    }
    else {
        ja_log("JAJOBICONJOB200002", 0, NULL, inner_job_id, __function_name, inner_job_id);
        ret = ja_set_runerr(inner_job_id, 2);
    }

    DBfree_result(result);
    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: jajob_icon_job_timeout                                           *
 *                                                                            *
 * Purpose: monitor the time-out of job icon                                  *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             start_time   (in) - start time of the icon (YYYYMMDDHHMMSS)    *
 *                                                                            *
 * Return value: return value of the ja_set_runerr function or SUCCEED        *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jajob_icon_job_timeout(zbx_uint64_t inner_job_id, char *start_time)
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
    int			 timeout_run_type;
    char 		 w_timeout_run_type[50];
    const char   *__function_name = "jajob_icon_job_timeout";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " start_time: %s",
               __function_name, inner_job_id, start_time);

    result = DBselect("select inner_jobnet_id, timeout, timeout_run_type from ja_run_icon_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONJOB200002", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    zbx_strlcpy(timeout, row[1], sizeof(timeout));
    timeout_run_type = atoi(row[2]);
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

    ja_log("JAJOBICONJOB300001", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id,
           timeout, start_time, w_main_jobnet_id, ja_get_jobid(inner_job_id), w_execution_user_name);

    w_timeout_run_type[0] = "\0";
    //timeout_run_type 1:abort flag  method_flag 3:Forced stop flag
    if(timeout_run_type == 1){
    	zbx_strlcpy(w_timeout_run_type, " ,method_flag = 3 ", sizeof(w_timeout_run_type));
    }else{
    	zbx_strlcpy(w_timeout_run_type, " ", sizeof(w_timeout_run_type));
    }
    rc = DBexecute("update ja_run_job_table set timeout_flag = 1 %s"
                   " where inner_job_id = " ZBX_FS_UI64, w_timeout_run_type, inner_job_id);

    if (rc < ZBX_DB_OK) {
        ja_log("JAJOBICONJOB200003", 0, NULL, inner_job_id, __function_name, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    ja_joblog(JC_JOB_TIMEOUT, inner_jobnet_id, inner_job_id);

    return SUCCEED;
}
