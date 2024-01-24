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
#include "jaflow.h"
#include "jastr.h"
#include "jajoblog.h"
#include "javalue.h"
#include "jastatus.h"
#include "jajobid.h"

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
int ja_set_jobstatus(const zbx_uint64_t inner_jobnet_id, const int status,
                     const int jobstatus)
{
    int rc;
    char *ts;
    char str_end[50];
    char str_runjob[50];
    const char *__function_name = "ja_set_jobstatus";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64
               ", status: %d, jobstatus: %d",
               __function_name, inner_jobnet_id, status, jobstatus);

    ts = ja_timestamp2str(time(NULL));
    if (status == JA_JOBNET_STATUS_END || status == JA_JOBNET_STATUS_ENDERR) {
        rc = DBexecute("delete from ja_session_table where inner_jobnet_main_id = " ZBX_FS_UI64,
                       inner_jobnet_id);
        if (rc < ZBX_DB_OK) {
            return FAIL;
        }
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = %s", ts);
        zbx_snprintf(str_runjob, sizeof(str_runjob), ", running_job_id = '', running_job_name = ''");
    }
    else {
        zbx_snprintf(str_end, sizeof(str_end), "");
        zbx_snprintf(str_runjob, sizeof(str_runjob), "");
    }

    DBfree_result(DBselect
                  ("select status from ja_run_jobnet_summary_table where inner_jobnet_id = "
                   ZBX_FS_UI64 " for update", inner_jobnet_id));

    if (ZBX_DB_OK >
        DBexecute
        ("update ja_run_jobnet_summary_table set status = %d, job_status = %d %s %s"
         " where inner_jobnet_id = " ZBX_FS_UI64,
         status, jobstatus, str_end, str_runjob, inner_jobnet_id)) {
        return FAIL;
    }

    return SUCCEED;
}

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
int ja_set_status_jobnet_summary(const zbx_uint64_t inner_jobnet_id,
                                 const int status, const int start,
                                 const int end)
{
    int rc;
    char *ts;
    char str_start[50];
    char str_end[50];
    char str_runjob[50];
    const char *__function_name = "ja_set_status_jobnet_summary";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64 ", status: %d",
               __function_name, inner_jobnet_id, status);

    if (status == JA_JOBNET_STATUS_END || status == JA_JOBNET_STATUS_ENDERR) {
        rc = DBexecute("delete from ja_session_table where inner_jobnet_main_id = " ZBX_FS_UI64,
                       inner_jobnet_id);
        if (rc < ZBX_DB_OK) {
            return FAIL;
        }
        zbx_snprintf(str_runjob, sizeof(str_runjob), ", running_job_id = '', running_job_name = ''");
    }
    else {
        zbx_snprintf(str_runjob, sizeof(str_runjob), "");
    }

    ts = ja_timestamp2str(time(NULL));
    if (start == 0)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = 0");
    else if (start == 1)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = %s",
                     ts);
    else
        zbx_snprintf(str_start, sizeof(str_start), "");

    if (end == 0)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = 0");
    else if (end == 1)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = %s", ts);
    else
        zbx_snprintf(str_end, sizeof(str_end), "");

    DBfree_result(DBselect
                  ("select status from ja_run_jobnet_summary_table where inner_jobnet_id = "
                   ZBX_FS_UI64 " for update", inner_jobnet_id));

    if (ZBX_DB_OK >
        DBexecute
        ("update ja_run_jobnet_summary_table set status = %d %s %s %s"
         " where inner_jobnet_id = " ZBX_FS_UI64,
         status, str_start, str_end, str_runjob, inner_jobnet_id)) {
        return FAIL;
    }

    return SUCCEED;
}

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
int ja_set_status_jobnet(const zbx_uint64_t inner_jobnet_id,
                         const int status, const int start, const int end)
{
    char *ts;
    char str_start[50];
    char str_end[50];
    const char *__function_name = "ja_set_status_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64 ", status: %d",
               __function_name, inner_jobnet_id, status);

    ts = ja_timestamp2str(time(NULL));
    if (start == 0)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = 0");
    else if (start == 1)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = %s",
                     ts);
    else
        zbx_snprintf(str_start, sizeof(str_start), "");

    if (end == 0)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = 0");
    else if (end == 1)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = %s", ts);
    else
        zbx_snprintf(str_end, sizeof(str_end), "");

    DBfree_result(DBselect
                  ("select status from ja_run_jobnet_table where inner_jobnet_id = "
                   ZBX_FS_UI64 " for update", inner_jobnet_id));

    if (ZBX_DB_OK >
        DBexecute
        ("update ja_run_jobnet_table set status = %d %s %s"
         " where inner_jobnet_id = " ZBX_FS_UI64, status, str_start,
         str_end, inner_jobnet_id)) {
        return FAIL;
    }

    return SUCCEED;
}

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
int ja_set_status_job(const zbx_uint64_t inner_job_id,
                      const int status, const int start, const int end)
{
    char *ts;
    char str_start[50];
    char str_end[50];
    const char *__function_name = "ja_set_status_job";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", status: %d",
               __function_name, inner_job_id, status);

    ts = ja_timestamp2str(time(NULL));
    if (start == 0)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = 0");
    else if (start == 1)
        zbx_snprintf(str_start, sizeof(str_start), ", start_time = %s",
                     ts);
    else
        zbx_snprintf(str_start, sizeof(str_start), "");

    if (end == 0)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = 0");
    else if (end == 1)
        zbx_snprintf(str_end, sizeof(str_end), ", end_time = %s", ts);
    else
        zbx_snprintf(str_end, sizeof(str_end), "");

    DBfree_result(DBselect
                  ("select status from ja_run_job_table where inner_job_id = "
                   ZBX_FS_UI64 " for update", inner_job_id));

    if (ZBX_DB_OK >
        DBexecute
        ("update ja_run_job_table set status = %d %s %s"
         " where inner_job_id = " ZBX_FS_UI64, status, str_start, str_end,
         inner_job_id)) {
        return FAIL;
    }

    return SUCCEED;
}

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
int ja_get_status_jobnet_summary(const zbx_uint64_t inner_jobnet_id)
{
    DB_RESULT result;
    DB_ROW row;
    int status;
    const char *__function_name = "ja_get_status_jobnet_summary";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    status = -1;
    result =
        DBselect
        ("select status from ja_run_jobnet_summary_table"
         " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL != (row = DBfetch(result))) {
        status = atoi(row[0]);
    }
    DBfree_result(result);
    return status;
}

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
int ja_get_status_jobnet(const zbx_uint64_t inner_jobnet_id)
{
    DB_RESULT result;
    DB_ROW row;
    int status;
    const char *__function_name = "ja_get_status_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    status = -1;
    result =
        DBselect
        ("select status from ja_run_jobnet_table"
         " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL != (row = DBfetch(result))) {
        status = atoi(row[0]);
    }
    DBfree_result(result);
    return status;
}

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
int ja_get_status_job(const zbx_uint64_t inner_job_id)
{
    DB_RESULT result;
    DB_ROW row;
    int status;
    const char *__function_name = "ja_get_status_job";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    status = -1;
    result =
        DBselect
        ("select status from ja_run_job_table"
         " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        status = atoi(row[0]);
    }
    DBfree_result(result);
    return status;
}

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
int ja_set_run_jobnet(const zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "ja_set_run_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    if (ja_clean_value_jobnet_before(inner_jobnet_id) == FAIL)
        return FAIL;

    if (ja_joblog(JC_JOBNET_START, inner_jobnet_id, 0) == FAIL)
        return FAIL;

    return ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_RUN, 1, 0);
}

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
int ja_set_end_jobnet(const zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "ja_set_run_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    if (ja_clean_value_jobnet_after(inner_jobnet_id) == FAIL)
        return FAIL;

    if (ja_joblog(JC_JOBNET_END, inner_jobnet_id, 0) == FAIL)
        return FAIL;

    return ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_END, -1, 1);
}

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
int ja_set_runerr_jobnet(const zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "ja_set_runerr_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    if (ja_clean_value_jobnet_after(inner_jobnet_id) == FAIL)
        return FAIL;

    //if (ja_joblog(JC_JOBNET_ERR_END, inner_jobnet_id, 0) == FAIL)
    //    return FAIL;

    return ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_RUNERR, -1, 1);
}

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
int ja_set_enderr_jobnet(const zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "ja_set_enderr_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    if (ja_clean_value_jobnet_after(inner_jobnet_id) == FAIL)
        return FAIL;

    return ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_ENDERR, -1, 1);
}

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
int ja_set_run(const zbx_uint64_t inner_job_id)
{
    const char *__function_name = "ja_set_run";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    if (ja_clean_value_before(inner_job_id) == FAIL)
        return FAIL;

    //if (ja_joblog(JC_JOB_START, 0, inner_job_id) == FAIL)
    //    return FAIL;

    return ja_set_status_job(inner_job_id, JA_JOB_STATUS_RUN, 1, 0);
}

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
int ja_set_end(const zbx_uint64_t inner_job_id, int msg_flag)
{
    const char *__function_name = "ja_set_end";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " msg_flag: %d",
               __function_name, inner_job_id, msg_flag);

    if (msg_flag == 0) {
        return SUCCEED;
    }

    if (ja_clean_value_after(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (ja_joblog(JC_JOB_END, 0, inner_job_id) == FAIL) {
        return FAIL;
    }

    return ja_set_status_job(inner_job_id, JA_JOB_STATUS_END, -1, 1);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_set_runerr                                                    *
 *                                                                            *
 * Purpose: updated to run error the status of icon                           *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             icon_status (in) - instruct the next processing of the         *
 *                                processing continuation                     *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_set_runerr(const zbx_uint64_t inner_job_id, int icon_status)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_jobnet_id;
    zbx_uint64_t inner_jobnet_main_id;
    int job_type, continue_flag;
    char w_main_jobnet_id[JA_JOBNET_ID_LEN];
    char w_execution_user_name[JA_USER_NAME_LEN];
    char w_job_exit_cd[10];
    char w_icon_status[10];
    const char *__function_name = "ja_set_runerr";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    if (ja_clean_value_after(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (ja_joblog(JC_JOB_ERR_END, 0, inner_job_id) == FAIL) {
        return FAIL;
    }

    /* print the job stop error message */
    inner_jobnet_id          = 0;
    inner_jobnet_main_id     = 0;
    w_main_jobnet_id[0]      = '\0';
    w_execution_user_name[0] = '\0';
    w_job_exit_cd[0]         = '\0';
    w_icon_status[0]         = '\0';
    job_type                 = JA_JOB_TYPE_START;
    continue_flag            = 0;

    result = DBselect("select inner_jobnet_id, inner_jobnet_main_id, job_type, continue_flag"
                      " from ja_run_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        ZBX_STR2UINT64(inner_jobnet_main_id, row[1]);
        job_type      = atoi(row[2]);
        continue_flag = atoi(row[3]);
    }
    DBfree_result(result);

    /* write the job controller variables icon status */
    if (job_type == JA_JOB_TYPE_JOB && icon_status != -99) {
        zbx_snprintf(w_icon_status, sizeof(w_icon_status), "%d", icon_status);
        ja_set_value_after(inner_job_id, inner_jobnet_id, "ICON_STATUS", w_icon_status);
    }

    if (job_type != JA_JOB_TYPE_JOBNET) {
        /* jobnet id and user name get */
        if (inner_jobnet_main_id != 0) {
            result = DBselect("select jobnet_id, execution_user_name from ja_run_jobnet_table"
                              " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_main_id);

            if (NULL != (row = DBfetch(result))) {
                zbx_strlcpy(w_main_jobnet_id, row[0], sizeof(w_main_jobnet_id));
                zbx_strlcpy(w_execution_user_name, row[1], sizeof(w_execution_user_name));
            }
            DBfree_result(result);
        }

        /* get the job exit code */
        if (job_type == JA_JOB_TYPE_JOB || job_type == JA_JOB_TYPE_LESS) {
            result = DBselect("select after_value from ja_run_value_after_table"
                              " where inner_job_id = " ZBX_FS_UI64 " and value_name = 'JOB_EXIT_CD'",
                              inner_job_id);

            if (NULL != (row = DBfetch(result))) {
                zbx_strlcpy(w_job_exit_cd, row[0], sizeof(w_job_exit_cd));
            }
            DBfree_result(result);
        }

        ja_log("JAJOBNETRUN000001", 0, NULL, inner_job_id,
               __function_name, inner_job_id, w_main_jobnet_id, ja_get_jobid(inner_job_id), w_execution_user_name,
               w_job_exit_cd, w_icon_status);
    }

    if (continue_flag == JA_JOB_CONTINUE_FLAG_OFF || icon_status == -99 || job_type == JA_JOB_TYPE_IF) {
        /* icon error stop */
        return ja_set_status_job(inner_job_id, JA_JOB_STATUS_RUNERR, -1, 1);
    }
    else{/* icon processing continues */
        //job close
        if((ja_set_status_job(inner_job_id, JA_JOB_STATUS_RUNERR, -1, 1)) == FAIL ){
            return FAIL;
        }
        //next job enable
        if (continue_flag == JA_JOB_CONTINUE_FLAG_ON){
            if(ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 0) == FAIL) {
                return FAIL;
            }
        }
    }
    return SUCCEED;
}

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
int ja_set_enderr(const zbx_uint64_t inner_job_id, int msg_flag)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_jobnet_main_id;
    char w_main_jobnet_id[JA_JOBNET_ID_LEN];
    char w_execution_user_name[JA_USER_NAME_LEN];
    const char *__function_name = "ja_set_enderr";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " msg_flag: %d",
               __function_name, inner_job_id, msg_flag);

    if (ja_clean_value_after(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (msg_flag == 1) {
        if (ja_joblog(JC_JOB_ERR_END, 0, inner_job_id) == FAIL) {
            return FAIL;
        }

        /* print the job stop error message */
        inner_jobnet_main_id     = 0;
        w_main_jobnet_id[0]      = '\0';
        w_execution_user_name[0] = '\0';

        result = DBselect("select inner_jobnet_main_id, job_type"
                          " from ja_run_job_table"
                          " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

        if (NULL != (row = DBfetch(result))) {
            ZBX_STR2UINT64(inner_jobnet_main_id, row[0]);
        }
        DBfree_result(result);

        /* jobnet id and user name get */
        if (inner_jobnet_main_id != 0) {
            result = DBselect("select jobnet_id, execution_user_name from ja_run_jobnet_table"
                              " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_main_id);

            if (NULL != (row = DBfetch(result))) {
                zbx_strlcpy(w_main_jobnet_id, row[0], sizeof(w_main_jobnet_id));
                zbx_strlcpy(w_execution_user_name, row[1], sizeof(w_execution_user_name));
            }
            DBfree_result(result);
        }

        ja_log("JAJOBNETRUN200001", 0, NULL, inner_job_id,
               __function_name, inner_job_id, w_main_jobnet_id, ja_get_jobid(inner_job_id), w_execution_user_name);
    }

    return ja_set_status_job(inner_job_id, JA_JOB_STATUS_ENDERR, -1, 1);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_set_run_job_id                                                *
 *                                                                            *
 * Purpose: update the job name and job id of the running of the jobnet       *
 *          information                                                       *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_set_run_job_id(const zbx_uint64_t inner_job_id)
{
    DB_RESULT  result;
    DB_ROW     row;
    int        rc;
    char       *job_id, *job_name, *job_name_esc = NULL;
    const char *__function_name = "ja_set_run_job_id";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    /* get job information */
    result = DBselect("select inner_jobnet_main_id, job_name"
                      " from ja_run_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JASTATUS200002", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }

    if (SUCCEED != DBis_null(row[1])) {
        job_name_esc = DBdyn_escape_string(row[1]);
        job_name     = job_name_esc;
    }
    else {
        job_name = "";
    }

    /* get run job id */
    job_id = ja_get_jobid(inner_job_id);

    /* jobnet summary information update */
    DBfree_result(DBselect("select status from ja_run_jobnet_summary_table"
                           " where inner_jobnet_id = %s for update", row[0]));

    rc = DBexecute("update ja_run_jobnet_summary_table"
                   " set running_job_id = '%s', running_job_name = '%s'"
                   " where inner_jobnet_id  = %s",
                   job_id, job_name, row[0]);

    if (rc < ZBX_DB_OK) {
        ja_log("JASTATUS200003", 0, NULL, inner_job_id, __function_name, "ja_run_jobnet_summary_table", row[0]);
        zbx_free(job_name_esc);
        DBfree_result(result);
        return FAIL;
    }

    /* jobnet management information update */
    DBfree_result(DBselect("select status from ja_run_jobnet_table"
                           " where inner_jobnet_id = %s for update", row[0]));

    rc = DBexecute("update ja_run_jobnet_table"
                   " set running_job_id = '%s', running_job_name = '%s'"
                   " where inner_jobnet_id  = %s",
                   job_id, job_name, row[0]);

    if (rc < ZBX_DB_OK) {
        ja_log("JASTATUS200003", 0, NULL, inner_job_id, __function_name, "ja_run_jobnet_table", row[0]);
        zbx_free(job_name_esc);
        DBfree_result(result);
        return FAIL;
    }

    zbx_free(job_name_esc);
    DBfree_result(result);

    return SUCCEED;
}
