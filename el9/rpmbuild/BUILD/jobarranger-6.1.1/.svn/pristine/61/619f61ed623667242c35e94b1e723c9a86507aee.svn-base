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

extern unsigned char process_type;
extern int process_num;
static int timeout_cnt = 0;

/******************************************************************************
 *                                                                            *
 * Function: process_jajobnet_kill                                            *
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

static void  process_jajobnet_kill()
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    double sec;
    zbx_uint64_t inner_jobnet_id;
    const char *__function_name = "process_jajobnet_kill";
    sec = zbx_time();

    result =
        DBselect
        ("select inner_jobnet_id, status, jobnet_abort_flag"
         " from ja_run_jobnet_summary_table where status = %d and jobnet_abort_flag = 1", JA_JOBNET_STATUS_RUN);
         
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_summary_table(status): " ZBX_FS_DBL " sec.", __function_name, sec);

    while (NULL != (row = DBfetch(result))) {  
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        sec = zbx_time() - sec;

        zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_summary_table read."
                   " inner_jobnet_id[%s] status[%s] jobnet_abort_flag[%s]",
                   __function_name, row[0], row[1], row[2]);
 
        ret = SUCCEED;
        DBbegin();
        ret = jajobnet_kill(inner_jobnet_id);
        ja_joblog(JC_JOBNET_ERR_END, inner_jobnet_id, 0);

        if (ret == SUCCEED) {
            DBcommit();
        } else {
            zabbix_log(LOG_LEVEL_ERR, "In %s() rollback", __function_name);
            DBrollback();
        }
    }
    DBfree_result(result);
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
static void  process_jajob_kill()
{
    int ret, job_type;
    DB_RESULT result;
    DB_ROW row;
    double sec;
    zbx_uint64_t inner_job_id, inner_jobnet_id;
    const char *__function_name = "process_jajob_kill";
    
    sec = zbx_time();

    result = DBselect("select inner_job_id, inner_jobnet_id, job_type, method_flag"
                      " from ja_run_job_table where status = %d and method_flag = %d", JA_JOB_STATUS_RUN, JA_JOB_METHOD_ABORT);

    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_job_table(status: RUN): " ZBX_FS_DBL " sec.", __function_name, sec);
    while (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_job_id, row[0]);
        ZBX_STR2UINT64(inner_jobnet_id, row[1]);
        job_type = atoi(row[2]);

        zabbix_log(LOG_LEVEL_DEBUG, "inner_job_id: " ZBX_FS_UI64 ", job_type: %d, method: abort",
                   inner_job_id, job_type);

        ret = SUCCEED;
        DBbegin();
        DBfree_result(DBselect("select inner_job_id from ja_run_job_table where inner_job_id = "
                               ZBX_FS_UI64 " for update", inner_job_id));
        switch (job_type) {
            case JA_JOB_TYPE_JOB:
            zabbix_log(LOG_LEVEL_DEBUG, "jajob_icon_job_abort(inner_job_id)");
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
            ja_log("JAJOB300001", inner_jobnet_id, NULL, 0, __function_name, inner_job_id, job_type);   //need to check
            DBrollback();
        }
    }
    DBfree_result(result);
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
	int abort_timeout_cnt = 30;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In main_jajobnet_loop() process_type:'%s' process_num:%d",
               ja_get_process_type_string(process_type), process_num);
    
    zbx_setproctitle("%s [connecting to the database]",
     ja_get_process_type_string(process_type));

    DBconnect(ZBX_DB_CONNECT_NORMAL);
    for (;;) {

        zbx_setproctitle("%s [processing data]",
                         ja_get_process_type_string(process_type));
        process_jajobnet_kill();
        process_jajob_kill();

        ja_sleep_loop(CONFIG_JAABORT_INTERVAL);
        if (timeout_cnt >= abort_timeout_cnt)
        	timeout_cnt = 0;
        else
            timeout_cnt += CONFIG_JAABORT_INTERVAL;
    }
}
