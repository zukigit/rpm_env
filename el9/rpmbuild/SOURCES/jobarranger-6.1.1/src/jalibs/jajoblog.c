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
#include "dbcache.h"

#include "jacommon.h"
#include "jalog.h"
#include "jajoblog.h"

static char	msgwork[2048];

/******************************************************************************
 *                                                                            *
 * Function: ja_joblog                                                        *
 *                                                                            *
 * Purpose: add data to a job run log table                                   *
 *                                                                            *
 * Parameters: message_id (in) - message id                                   *
 *             inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - Processing error occurs                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int	ja_joblog(char *message_id, zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id)
{
	struct tm	*tm;
	struct timeval	tv;
	DB_RESULT	result;
	DB_ROW		row;
	int		job_type, sql_flag, rc, ms, cnt, session_flag;
	zbx_uint64_t	i_inner_jobnet_id;
	char		*now_date;
	const char	*__function_name = "ja_joblog";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s() message_id: %s", __function_name, message_id);

	/* parameters check */
	if (inner_jobnet_id == 0 && inner_job_id == 0)
	{
		ja_log("JAJOBLOG200006", 0, NULL, 0);
		return FAIL;
	}

	i_inner_jobnet_id = inner_jobnet_id;

	/* inner jobnet id get */
	if (inner_jobnet_id == 0)
	{
		result = DBselect("select inner_jobnet_id from ja_run_job_table"
				" where inner_job_id = " ZBX_FS_UI64,
				inner_job_id);

		if (NULL == (row = DBfetch(result)))
		{
			zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64, inner_job_id);
			ja_log("JAJOBLOG200007", inner_jobnet_id, NULL, inner_job_id, "ja_run_job_table", msgwork);
			DBfree_result(result);
			return FAIL;
		}

		ZBX_STR2UINT64(i_inner_jobnet_id, row[0]);
		DBfree_result(result);
	}

	/* get the current time */
	gettimeofday(&tv, NULL);
	tm = localtime(&tv.tv_sec);
	ms = (int)(tv.tv_usec / 1000);
	now_date = zbx_dsprintf(NULL, "%04d%02d%02d%02d%02d%02d%03d",
				(tm->tm_year + 1900),
				(tm->tm_mon  + 1),
				 tm->tm_mday,
				 tm->tm_hour,
				 tm->tm_min,
				 tm->tm_sec,
				 ms);

	/* determine the type of sql */
	sql_flag = 0;
	if (inner_job_id != 0 && strcmp(message_id, JC_JOBNET_TIMEOUT) != 0)
	{
		sql_flag = 1;
		if (strcmp(message_id, JC_JOB_END) == 0 || strcmp(message_id, JC_JOB_ERR_END) == 0)
		{
			result = DBselect("select job_type from ja_run_job_table"
					" where inner_job_id = " ZBX_FS_UI64,
					inner_job_id);

			if (NULL == (row = DBfetch(result)))
			{
				zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64, inner_job_id);
				ja_log("JAJOBLOG200005", i_inner_jobnet_id, NULL, inner_job_id, msgwork);
				zbx_free(now_date);
				DBfree_result(result);
				return FAIL;
			}
			job_type = atoi(row[0]);
			DBfree_result(result);

			if (job_type == JA_JOB_TYPE_JOB || job_type == JA_JOB_TYPE_LESS)
			{
				sql_flag = 2;
			}

			if (job_type == JA_JOB_TYPE_LESS)
			{
				result = DBselect("select session_flag from ja_run_icon_agentless_table"
						" where inner_job_id = " ZBX_FS_UI64,
						inner_job_id);

				if (NULL == (row = DBfetch(result)))
				{
					zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64, inner_job_id);
					ja_log("JAJOBLOG200007", inner_jobnet_id, NULL, inner_job_id, "ja_run_icon_agentless_table", msgwork);
					zbx_free(now_date);
					DBfree_result(result);
					return FAIL;
				}
				session_flag = atoi(row[0]);
				DBfree_result(result);

				if (session_flag == JA_SESSION_FLAG_CLOSE)
				{
					sql_flag = 1;
				}
			}

			/* check standard output of job icon or agent less icon */
			if (sql_flag == 2)
			{
				result = DBselect("select count(*) from ja_run_value_after_table"
						" where inner_job_id = " ZBX_FS_UI64
						" and (value_name = 'JOB_EXIT_CD' or value_name = 'STD_OUT' or value_name = 'STD_ERR')",
						inner_job_id);

				if (NULL == (row = DBfetch(result)))
				{
					zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64, inner_job_id);
					ja_log("JAJOBLOG200007", inner_jobnet_id, NULL, inner_job_id, "ja_run_value_after_table", msgwork);
					zbx_free(now_date);
					DBfree_result(result);
					return FAIL;
				}
				cnt = atoi(row[0]);
				DBfree_result(result);

				if (cnt != 3)
				{
					sql_flag = 1;
				}
			}
		}
	}else if(strcmp(message_id, JC_JOBNET_TIMEOUT) == 0 ){
		// jobnet timeout
		sql_flag = 3;
	}

	/* job run log write */
	switch (sql_flag)
	{
		case 1:
			/* job message */
			rc = DBexecute("insert into ja_run_log_table ("
					" log_date, inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date,"
					" method_flag, jobnet_status, job_status, run_type, public_flag, jobnet_id, jobnet_name,"
					" job_id, job_name, user_name, message_id)"
					" select %s, a.inner_jobnet_id, a.inner_jobnet_main_id, b.inner_job_id, a.update_date,"
					" b.method_flag, a.status, b.status, a.run_type, a.public_flag, a.jobnet_id, a.jobnet_name,"
					" b.job_id, b.job_name, a.user_name, '%s'"
					" from ja_run_jobnet_table a, ja_run_job_table b"
					" where a.inner_jobnet_id = b.inner_jobnet_id and b.inner_job_id = " ZBX_FS_UI64 " and b.inner_jobnet_id = " ZBX_FS_UI64,
					now_date, message_id,
					inner_job_id, i_inner_jobnet_id);
			break;

		case 2:
			/* end of Job icon and agentless icon */
			rc = DBexecute("insert into ja_run_log_table ("
					" log_date, inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date,"
					" method_flag, jobnet_status, job_status, run_type, public_flag, jobnet_id, jobnet_name,"
					" job_id, job_name, user_name, return_code, std_out, std_err, message_id)"
					" select %s, a.inner_jobnet_id, a.inner_jobnet_main_id, b.inner_job_id, a.update_date,"
					" b.method_flag, a.status, b.status, a.run_type, a.public_flag, a.jobnet_id, a.jobnet_name,"
					" b.job_id, b.job_name, a.user_name, c.after_value, d.after_value, e.after_value, '%s'"
					" from ja_run_jobnet_table a, ja_run_job_table b,"
					" (select after_value from ja_run_value_after_table where inner_job_id = " ZBX_FS_UI64 " and value_name = 'JOB_EXIT_CD') c,"
					" (select after_value from ja_run_value_after_table where inner_job_id = " ZBX_FS_UI64 " and value_name = 'STD_OUT') d,"
					" (select after_value from ja_run_value_after_table where inner_job_id = " ZBX_FS_UI64 " and value_name = 'STD_ERR') e"
					" where a.inner_jobnet_id = b.inner_jobnet_id and b.inner_job_id = " ZBX_FS_UI64 " and b.inner_jobnet_id = " ZBX_FS_UI64,
					now_date, message_id,
					inner_job_id, inner_job_id, inner_job_id,
					inner_job_id, i_inner_jobnet_id);
			break;

		case 3:
			// jobnet timeout
			rc = DBexecute("insert into ja_run_log_table ("
								" log_date, inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date,"
								" method_flag, jobnet_status, job_status, run_type, public_flag, jobnet_id, jobnet_name,"
								" job_id, job_name, user_name, message_id)"
								" select %s, inner_jobnet_id, inner_jobnet_main_id, null , update_date,"
								" 0, status, 0, run_type, public_flag, jobnet_id, jobnet_name,"
								" '', '', user_name, '%s'"
								" from ja_run_jobnet_table "
								" where inner_jobnet_id = " ZBX_FS_UI64 ,
								now_date, message_id, i_inner_jobnet_id);
			break;
		default:
			/* duplication error jobnet message deletion */
			if (strcmp(message_id, JC_JOBNET_ERR_END) == 0)
			{
				rc = DBexecute("delete from ja_run_log_table"
						" where inner_jobnet_id = " ZBX_FS_UI64 " and message_id = '%s'",
						i_inner_jobnet_id, message_id);

				if (rc < ZBX_DB_OK)
				{
					zbx_snprintf(msgwork, sizeof(msgwork),
						"message id[%s] inner jobnet id[" ZBX_FS_UI64 "] inner job id[" ZBX_FS_UI64 "]",
						message_id, i_inner_jobnet_id, inner_job_id);
					ja_log("JAJOBLOG200009", i_inner_jobnet_id, NULL, inner_job_id, "ja_run_log_table", msgwork);
					zbx_free(now_date);
					return FAIL;
				}
			}

			/* jobnet message */
			rc = DBexecute("insert into ja_run_log_table ("
					" log_date, inner_jobnet_id, inner_jobnet_main_id, update_date,"
					" method_flag, jobnet_status, run_type, public_flag, jobnet_id, jobnet_name,"
					" user_name, message_id)"
					" select %s, inner_jobnet_id, inner_jobnet_main_id, update_date,"
					" 0, status, run_type, public_flag, jobnet_id, jobnet_name,"
					" user_name, '%s'"
					" from ja_run_jobnet_table"
					" where inner_jobnet_id = " ZBX_FS_UI64,
					now_date, message_id, i_inner_jobnet_id);
			break;
	}

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork),
			"message id[%s] inner jobnet id[" ZBX_FS_UI64 "] inner job id[" ZBX_FS_UI64 "]",
			message_id, i_inner_jobnet_id, inner_job_id);
		ja_log("JAJOBLOG200008", i_inner_jobnet_id, NULL, inner_job_id, "ja_run_log_table", msgwork);
		zbx_free(now_date);
		return FAIL;
	}

	zbx_free(now_date);

	return SUCCEED;
}
