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
#include "jajoblog.h"
#include "jajobnetkill.h"
#include "jaabort.h"
#include "jaself.h"
#include "../jajob/jajobiconjob.h"
#include "../jajob/jajobiconextjob.h"
#include "../jajob/jajobiconless.h"
#include "../jajob/jajobiconreboot.h"
#include "../jarun/jaruniconreboot.h"
#include "../jarun/jaruniconfwait.h"
#define _POSIX_C_SOURCE 199309L //this is for nanosleep
#include <time.h>
#include <stdio.h>

extern unsigned char process_type;
extern int process_num;
static int timeout_cnt = 0;
static int u_time = 0;
time_t start_time, current_time, req_start_time;

/******************************************************************************
 *                                                                            *
 * Function: process_jajobnet_abort                                            *
 *                                                                            *
 * Purpose: to kill the jobnet with abort flag on                             *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/

static void  process_jajobnet_abort()
{
    int ret;
    DB_RESULT result;
    DB_RESULT abort_jobnet_result;
    DB_RESULT abort_job_result;
    DB_ROW row;
    DB_ROW user_row;
    int job_type;
    double sec, current_time;
    zbx_uint64_t inner_job_id, inner_jobnet_id;
    const char *__function_name = "process_jajobnet_abort";
    sec = zbx_time();

    abort_jobnet_result =
        DBselect
        ("select inner_jobnet_id, status, jobnet_abort_flag"
         " from ja_run_jobnet_summary_table where status = %d and jobnet_abort_flag = %d",
         JA_JOBNET_STATUS_RUN, JA_JOBENT_ABORT_FLAG_ON);
         
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_summary_table(status): " ZBX_FS_DBL " sec.", __function_name, sec);

    while (NULL != (row = DBfetch(abort_jobnet_result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_summary_table read."
                " inner_jobnet_id[%s] status[%s] jobnet_abort_flag[%s]",
                __function_name, row[0], row[1], row[2]);
        ret = SUCCEED;
        DBbegin();

        // jobnet abort status change starts from here

        result = DBselect("select jobnet_id, execution_user_name from ja_run_jobnet_table"
                        " where inner_jobnet_id = " ZBX_FS_UI64,
                        inner_jobnet_id);
        if(NULL != (row = DBfetch(result))){
            ja_log("JAABORT000001", inner_jobnet_id, NULL, 0, __function_name, inner_jobnet_id, row[0], row[1]);
            DBfree_result(result);
        }
    
        if (ZBX_DB_OK > DBexecute
                ("delete from ja_session_table where inner_jobnet_main_id = " ZBX_FS_UI64,
                inner_jobnet_id) )
            ret = FAIL;

        DBfree_result(DBselect
            ("select inner_job_id from ja_run_job_table where inner_jobnet_main_id = "
            ZBX_FS_UI64 " for update", inner_jobnet_id));
        
        sec = zbx_time();
        abort_job_result =
            DBselect
            ("select inner_job_id, job_type, status from ja_run_job_table"
            " where inner_jobnet_main_id = " ZBX_FS_UI64 " and status in (%d, %d)",
            inner_jobnet_id, JA_JOB_STATUS_READY, JA_JOB_STATUS_RUN);
        sec = zbx_time() - sec;
        zabbix_log(LOG_LEVEL_DEBUG,
                "In %s() ja_run_job_table:(inner_jobnet_main_id: " ZBX_FS_UI64 ") " ZBX_FS_DBL " sec.",
                __function_name, inner_jobnet_id, sec);
        
        while (NULL != (row = DBfetch(abort_job_result))) {
            ZBX_STR2UINT64(inner_job_id, row[0]);
            job_type = atoi(row[1]);

            if(atoi(row[1]) == JA_JOB_STATUS_READY) {
                if (ZBX_DB_OK >
                    DBexecute ("update ja_run_job_table set method_flag = %d where inner_job_id = " ZBX_FS_UI64,
                                JA_JOB_METHOD_ABORT, inner_job_id))
                    ret = FAIL;
                continue;
            }

            switch (job_type) {
                case JA_JOB_TYPE_JOB:
                case JA_JOB_TYPE_EXTJOB:
                case JA_JOB_TYPE_FWAIT:
                case JA_JOB_TYPE_LESS:
                    if (ZBX_DB_OK >
                        DBexecute ("update ja_run_job_table set method_flag = %d where inner_job_id = " ZBX_FS_UI64,
                                    JA_JOB_METHOD_ABORT, inner_job_id))
                        ret = FAIL;
                    break;
                if (ret == FAIL) break;
                // zabbix_log(LOG_LEVEL_INFORMATION, "DEBUG : in process_jajobnet_abort() method is changed to JA_JOB_METHOD_ABORT : "ZBX_FS_UI64, inner_job_id);
                // zabbix_log(LOG_LEVEL_INFORMATION, "DEBUG : in process_jajobnet_abort() jobtype is : %d ", job_type);
            }
        }

        DBfree_result(abort_job_result);

        if(jajobnet_kill(inner_jobnet_id) == FAIL)  ret = FAIL;

        // jobnet abort status change ends from here

        if (ret == SUCCEED) {
            DBcommit();
        } else {
            zabbix_log(LOG_LEVEL_ERR, "In %s() rollback", __function_name);
            DBrollback();
        }
    }
    DBfree_result(abort_jobnet_result);
    zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
}


/******************************************************************************
 *                                                                            *
 * Function: process_jajob_abort()                                            *
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
void process_jajob_abort()
{
    DB_RESULT new_job_result;
    DB_RESULT old_job_result;
    DB_ROW row;
    zbx_uint64_t inner_job_id, inner_jobnet_id;
    int job_type;
    double sec;
    const char *__function_name = "process_jajob_abort";

    sec = zbx_time();
    new_job_result = DBselect("select inner_job_id, inner_jobnet_id, job_type, method_flag"
                      " from ja_run_job_table where method_flag = %d and status = %d", JA_JOB_METHOD_ABORT, JA_JOB_STATUS_RUN);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_job_table(status: RUN): " ZBX_FS_DBL " sec.", __function_name, sec);
    
    sec = zbx_time();
    old_job_result = DBselect("select inner_job_id, inner_jobnet_id, job_type, method_flag"
                      " from ja_run_job_table where method_flag = %d and status = %d", JA_JOB_METHOD_ABORT,JA_JOB_STATUS_ABORT);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_job_table(status: ABORT): " ZBX_FS_DBL " sec.", __function_name, sec);

    //this loop is for new job kill request
    while (NULL != (row = DBfetch(new_job_result))) {
        ZBX_STR2UINT64(inner_job_id, row[0]);
        ZBX_STR2UINT64(inner_jobnet_id, row[1]);
        job_type = atoi(row[2]);
        process_jajob_kill(inner_job_id, inner_jobnet_id, job_type);
    }
    current_time = time(NULL);
    if(current_time - req_start_time > CONFIG_JAOLDJOBKILL_INTERVAL){
        //this loop is for old job kill request
        while (NULL != (row = DBfetch(old_job_result))) {
            ZBX_STR2UINT64(inner_job_id, row[0]);
            ZBX_STR2UINT64(inner_jobnet_id, row[1]);
            job_type = atoi(row[2]);
            process_jajob_kill(inner_job_id, inner_jobnet_id, job_type);
        }
        req_start_time = current_time;
    }
    DBfree_result(new_job_result);
    DBfree_result(old_job_result);
    
    zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
}


/******************************************************************************
 *                                                                            *
 * Function: process_jajob_kill                                               *
 *                                                                            *
 * Purpose: to kill the job with abort flag on                                *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
void  process_jajob_kill(zbx_uint64_t inner_job_id, zbx_uint64_t inner_jobnet_id, int job_type)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    double sec;
    unsigned long long nano_sleeper ;
    const char *__function_name = "process_jajob_kill";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 ", job_type: %d, method: abort",
                __function_name,inner_job_id, job_type);

    ret = SUCCEED;
    DBbegin();
    DBfree_result(DBselect("select inner_job_id from ja_run_job_table where inner_job_id = "
                    ZBX_FS_UI64 " for update", inner_job_id));
    switch (job_type) {
        case JA_JOB_TYPE_JOB:
            jajob_icon_job_abort(inner_job_id);
            break;
        case JA_JOB_TYPE_EXTJOB:
            ret = jajob_icon_extjob_kill(inner_job_id);
            break;
        case JA_JOB_TYPE_FWAIT:
            jarun_icon_fwait(inner_job_id, JA_AGENT_METHOD_KILL);
            break;
        case JA_JOB_TYPE_REBOOT:
            zabbix_log(LOG_LEVEL_INFORMATION,"In %s(), reboot job icon kill ",__function_name);
            jarun_icon_reboot(inner_job_id, JA_AGENT_METHOD_KILL);
            break;
        case JA_JOB_TYPE_LESS:
            jajob_icon_less_abort(inner_job_id, JA_SES_FORCE_STOP_ON);
            break;
        default:
            break;
    }

    if (ret == SUCCEED) {
        DBcommit();
    } else {
        ja_log("JAJOB300001", inner_jobnet_id, NULL, 0, __function_name, inner_job_id, job_type);
        DBrollback();
    }
    // usleep(CONFIG_JAJOBKILL_INTERVAL*1000);
    nano_sleeper = (unsigned long long)CONFIG_JAJOBKILL_INTERVAL*1000000;
    ja_jobkill_sleep(nano_sleeper);
    zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
}


/******************************************************************************
 *                                                                            *
 * Function: main_jaabort_loop()                                              *
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
void main_jaabort_loop()
{
    const char *__function_name = "main_jaabort_loop";
	int abort_timeout_cnt = 30;
    int loop_cnt = 0;

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() process_type:'%s' process_num:%d",__function_name,
               ja_get_process_type_string(process_type), process_num);
    
    zbx_setproctitle("%s [connecting to the database]",
     ja_get_process_type_string(process_type));
    u_time = zbx_time();
    ja_alarm_watcher("ja_abort");
    DBconnect(ZBX_DB_CONNECT_NORMAL);
    start_time = time(NULL);
    req_start_time = time(NULL);
    for (;;) {
        
        ja_alarm_timeout(CONFIG_ABORT_TIMEOUT);
        zbx_setproctitle("%s [processing data]", ja_get_process_type_string(process_type));
        process_jajobnet_abort();
        process_jajob_abort();

        ja_alarm_timeout(0);

        current_time = time(NULL);
        if (current_time - start_time < CONFIG_JAABORT_INTERVAL){
            // zabbix_log(LOG_LEVEL_INFORMATION,"DEBUG : abort process is sleeping for %d seconds", CONFIG_JAABORT_INTERVAL - (current_time - start_time));
            ja_sleep_loop(CONFIG_JAABORT_INTERVAL - (current_time - start_time));
        }
        start_time = time(NULL);
        if (timeout_cnt >= abort_timeout_cnt)
            timeout_cnt = 0;
        else
            timeout_cnt += CONFIG_JAABORT_INTERVAL;
        
    }
}

void ja_jobkill_sleep(unsigned long long nanoseconds){
    struct timespec req, rem;
    req.tv_sec = nanoseconds / 1000000000;
    req.tv_nsec = nanoseconds % 1000000000;
    while (nanosleep(&req, &rem) == -1) {
        req = rem;
    }
}
