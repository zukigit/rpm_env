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
#include "jaself.h"
#include "jajobnetsummaryready.h"
#include "jajobnetready.h"
#include "jajobnetrun.h"
#include "jajobnet.h"
#include "jatimeout.h"

extern unsigned char process_type;
extern int process_num;
static int timeout_cnt = 0;

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

int jajobnet_timeout(zbx_uint64_t inner_jobnet_id,char *start_time, char *timeout){
	int rc, jastatus = 0;
	DB_RESULT result;
	DB_ROW row;
	const char *__function_name = "jajobnet_timeout";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	if(ja_timeout_check(timeout, start_time) == FAIL) {
		/* no time-out */
		return SUCCEED;
	}

	result = DBselect("select inner_jobnet_id, running_job_id, start_time, jobnet_timeout, timeout_run_type "
			          " from ja_run_jobnet_summary_table "
					  " where inner_jobnet_id =" ZBX_FS_UI64 " and status=%d and job_status in (%d,%d) "
						, inner_jobnet_id, JA_JOBNET_STATUS_RUN, JA_SUMMARY_JOB_STATUS_NORMAL,JA_SUMMARY_JOB_STATUS_TIMEOUT);



	if(NULL != (row = DBfetch(result))){

		rc = DBexecute("update ja_run_jobnet_summary_table set job_status=%d,jobnet_abort_flag=%d, jobnet_timeout_flag=1"
						   " where inner_jobnet_id = " ZBX_FS_UI64, JA_SUMMARY_JOB_STATUS_TIMEOUT, atoi(row[4]), inner_jobnet_id);
		if (rc < ZBX_DB_OK) {
			DBfree_result(result);
			return FAIL;
		}
		zabbix_log(LOG_LEVEL_INFORMATION, "In %s() jobnet timeout inner_jobnet_id["ZBX_FS_UI64"] running_job_id[%s] timeout[%s] jobnet_abort_flag[%s]",
				__function_name,inner_jobnet_id, row[1],row[3],row[4]);
		jastatus = 1;
	}else{
		rc = DBexecute("update ja_run_jobnet_summary_table set jobnet_timeout_flag=1"
								   " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);
		if (rc < ZBX_DB_OK) {
			DBfree_result(result);
			return FAIL;
		}
		zabbix_log(LOG_LEVEL_INFORMATION, "In %s() jobnet timeout etc inner_jobnet_id["ZBX_FS_UI64"] timeout[%s]",
				__function_name, inner_jobnet_id,  timeout);
	}
	if(jastatus == 1){
		ja_log("JAJOBNETRUN200006", inner_jobnet_id, NULL, 0, __function_name, inner_jobnet_id, timeout, start_time, row[1]);

		ja_joblog(JC_JOBNET_TIMEOUT, inner_jobnet_id, 1);
	}
	DBfree_result(result);
	return SUCCEED;
}


static void process_jajobnet_summary()
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    double sec;
    zbx_uint64_t inner_jobnet_id;
    int status, jobnet_abort_flag,jobnet_timeout,jobnet_timeout_flag;
    const char *__function_name = "process_jajobnet_summary";

    sec = zbx_time();
    result =
        DBselect
        ("select inner_jobnet_id, status, start_time, jobnet_timeout, jobnet_timeout_flag, job_status"
         " from ja_run_jobnet_summary_table where status in (%d, %d) and jobnet_abort_flag = %d",
         JA_JOBNET_STATUS_READY, JA_JOBNET_STATUS_RUN, JA_JOBENT_ABORT_FLAG_OFF);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() ja_run_jobnet_summary_table(status): " ZBX_FS_DBL
               " sec.", __function_name, sec);

    while (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        status = atoi(row[1]);

        zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_summary_table read."
                   " inner_jobnet_id[%s] status[%s]",
                   __function_name, row[0], row[1]);

        ret = SUCCEED;
        DBbegin();
        switch (status) {
        case JA_JOBNET_STATUS_READY:
            ret = jajobnet_summary_ready(inner_jobnet_id);
            break;
        case JA_JOBNET_STATUS_RUN:
            jobnet_timeout = atoi(row[3]);
            jobnet_timeout_flag = atoi(row[4]);
            if((jobnet_timeout > 0 && jobnet_timeout_flag == 0) && timeout_cnt == 0) {
            	ret = jajobnet_timeout(inner_jobnet_id,row[2],row[3]);
            }
            break;
        default:
            break;
        }

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
static void process_jajobnet()
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    double sec;
    zbx_uint64_t inner_jobnet_id, inner_job_id;
    int timeout_flag, status;
    const char *__function_name = "process_jajobnet";

    sec = zbx_time();
    result =
        DBselect
        ("select inner_jobnet_id, inner_job_id, timeout_flag, status"
         " from ja_run_jobnet_table where status in (%d, %d ,%d)",
         JA_JOBNET_STATUS_READY, JA_JOBNET_STATUS_RUN,
         JA_JOBNET_STATUS_RUNERR);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() ja_run_jobnet_table(status): " ZBX_FS_DBL " sec.",
               __function_name, sec);

    while (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        ZBX_STR2UINT64(inner_job_id, row[1]);
        timeout_flag = atoi(row[2]);
        status = atoi(row[3]);

        zabbix_log(LOG_LEVEL_DEBUG, "In %s() ja_run_jobnet_table read."
                   " inner_jobnet_id[%s] inner_job_id[%s] timeout_flag[%s] status[%s]",
                   __function_name, row[0], row[1], row[2], row[3]);

        ret = SUCCEED;
        DBbegin();
        switch (status) {
        case JA_JOBNET_STATUS_READY:
            ret = jajobnet_ready(inner_jobnet_id);
            break;
        case JA_JOBNET_STATUS_RUN:
        case JA_JOBNET_STATUS_RUNERR:
            ret = jajobnet_run(inner_jobnet_id, inner_job_id, status, timeout_flag);
            break;
        default:
            break;
        }
        if (ret == SUCCEED) {
            DBcommit();
        } else {
            DBrollback();
        }
    }
    DBfree_result(result);

    zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
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
void main_jajobnet_loop()
{
	int jobnet_timeout_cnt = 30;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In main_jajobnet_loop() process_type:'%s' process_num:%d",
               ja_get_process_type_string(process_type), process_num);

    zbx_setproctitle("%s [connecting to the database]",
                     ja_get_process_type_string(process_type));

    ja_alarm_watcher("ja_jobnet");
    DBconnect(ZBX_DB_CONNECT_NORMAL);
    for (;;) {
        ja_alarm_timeout(CONFIG_JOBNET_TIMEOUT);
        zbx_setproctitle("%s [processing data]",
                         ja_get_process_type_string(process_type));
        process_jajobnet_summary();
        process_jajobnet();
        ja_sleep_loop(CONFIG_JAJOBNET_INTERVAL);
        if (timeout_cnt >= jobnet_timeout_cnt)
        	timeout_cnt = 0;
        else
            timeout_cnt += CONFIG_JAJOBNET_INTERVAL;
        ja_alarm_timeout(0);
    }
}
