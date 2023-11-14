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
#include "jajobid.h"
#include "jalog.h"

#define JALOG_TYPE_INFO		0
#define JALOG_TYPE_CRIT		1
#define JALOG_TYPE_ERR		2
#define JALOG_TYPE_WARN		3
#define JALOG_TYPE_DEBUG	4

#define JASENDER_OFF		0
#define JASENDER_ON		1

#define ZBXSND_NOTICE_OFF	0
#define ZBXSND_NOTICE_ON	1

#define VALID_FLAG_ON		1
#define USE_HOSTNAME		0

#define AP_MESSAGE_BUF_SIZE	4096
#define JA_DATA_BUFFER_LEN	256

#define JOBARG_MESSAGE		"JOBARG_MESSAGE"
#define ZBXSND_ON		"ZBXSND_ON"

/******************************************************************************
 *                                                                            *
 * Function: ja_get_zbxsnd_on                                                 *
 *                                                                            *
 * Purpose: gets the zabbix notification existence                            *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:  value of Zabbix notification existence that get             *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_get_zbxsnd_on()
{
	DB_RESULT	result;
	DB_ROW		row;
	int		vl;
	const char	*__function_name = "ja_get_zbxsnd_on";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	result = DBselect("select value from ja_parameter_table where parameter_name = '%s'", ZBXSND_ON);

	if (NULL == (row = DBfetch(result)))
	{
		DBfree_result(result);
		return ZBXSND_NOTICE_ON;
	}

	if (strlen(row[0]) != 1)
	{
		DBfree_result(result);
		return ZBXSND_NOTICE_ON;
	}

	vl = atoi(row[0]);
	DBfree_result(result);

	if (vl == ZBXSND_NOTICE_OFF || vl == ZBXSND_NOTICE_ON)
	{
		return vl;
	}

	return ZBXSND_NOTICE_ON;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_log                                                           *
 *                                                                            *
 * Purpose: output a message to the log file                                  *
 *          it also performs error notification app                           *
 *                                                                            *
 * Parameters: message_id (in) - message id                                   *
 *             inner_jobnet_id (in) - inner jobnet id                         *
 *             jobnet_id (in) - jobnet id                                     *
 *             inner_job_id (in) - inner job id                               *
 *             ... - additional information (variable parameters)             *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - Processing error occurs                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int	ja_log(char *message_id, zbx_uint64_t inner_jobnet_id, char *jobnet_id, zbx_uint64_t inner_job_id, ...)
{
	FILE		*fp;
	struct tm	*tm;
	time_t		now;
	DIR		*dir;
	struct dirent	*dp;
	va_list		ap;
	DB_RESULT	result;
	DB_RESULT	result2;
	DB_ROW		row;
	DB_ROW		row2;
	int		log_type, log_level, send_flag, rc, hit, m, n, cnt, job_type, host_flag, get_host_flag;
	int		state, zbxsnd;
	zbx_uint64_t	w_inner_jobnet_id, inner_jobnet_main_id;
	char		*now_date, *message = NULL, *name = NULL, *type = NULL, *send = NULL, *msg = NULL;
	char		*after_value_esc, *user_name_esc, *host_name_esc, *jobnet_name_esc, *job_name_esc;
	char		s_jobnet_id[JA_DATA_BUFFER_LEN] = "none";
	char		s_user_name[JA_DATA_BUFFER_LEN] = "none";
	char		s_job_id[JA_DATA_BUFFER_LEN] = "none";
	char		s_job_id_full[JA_DATA_BUFFER_LEN] = "none";
	char		line[AP_MESSAGE_BUF_SIZE];
	char		cmd[AP_MESSAGE_BUF_SIZE];
	char		host_name[JA_DATA_BUFFER_LEN] = "";
	char		jobnet_name[JA_JOBNET_NAME_LEN] = "";
	char		job_name[JA_JOB_NAME_LEN] = "";
	char		now_time[20];
	char		*str_inner_jobnet_main_id;
	const char	*__function_name = "ja_log";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s() message_id: %s inner_jobnet_id: " ZBX_FS_UI64 " inner_job_id: " ZBX_FS_UI64,
	           __function_name, message_id, inner_jobnet_id, inner_job_id);

	w_inner_jobnet_id    = inner_jobnet_id;
	inner_jobnet_main_id = 0;

	/* message file open */
	fp = fopen(CONFIG_JA_LOG_MESSAGE_FILE, "r");
	if (fp == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "failed to open the log message file: [%s] (%s)", CONFIG_JA_LOG_MESSAGE_FILE, strerror(errno));
		return FAIL;
	}

	/* message get */
	cnt = 0;
	hit = 0;
	while (fgets(line, sizeof(line), fp) != NULL)
	{
		cnt = cnt + 1;

		if (line[0] == '#' || line[0] == '\n' || line[0] == '\r' )
		{
			continue;
		}

		if (strlen(line) > 0)
		{
			if (line[strlen(line)-1] == '\n')
			{
				line[strlen(line)-1] = '\0';
			}
		}

		if (strlen(line) > 0)
		{
			if (line[strlen(line)-1] == '\r')
			{
				line[strlen(line)-1] = '\0';
			}
		}

		n = 0;
		m = 0;
		name = line;
		type = NULL;
		send = NULL;
		msg  = NULL;
		while (line[++n])
		{
			if (line[n] == ',')
			{
				if (m == 0)
				{
					line[n] = '\0';
					type = &line[n + 1];
					m++;
				}
				else if (m == 1)
				{
					line[n] = '\0';
					send = &line[n + 1];
					m++;
				}
				else if (m == 2)
				{
					line[n] = '\0';
					msg = &line[n + 1];
					break;
				}
			}
		}

		lrtrim_spaces(name);

		if (strcmp(name, message_id) == 0)
		{
			hit = 1;
			break;
		}
	}
	fclose(fp);

	/* message hit check */
	if (hit == 0)
	{
		zabbix_log(LOG_LEVEL_ERR, "could not find an appropriate message: [%s]", message_id);
		return FAIL;
	}

	/* message get check */
	if (name == NULL || type == NULL || send == NULL || msg == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "line data in the message file is invalid: line(%d) id[%s] file[%s]",
			cnt, message_id, CONFIG_JA_LOG_MESSAGE_FILE);
		return FAIL;
	}

	lrtrim_spaces(type);
	lrtrim_spaces(send);

	/* check the empty data */
	if (strlen(type) <= 0 || strlen(send) <= 0 || strlen(msg) <= 0)
	{
		zabbix_log(LOG_LEVEL_ERR, "line data in the message file is invalid: line(%d) id[%s] file[%s]",
			cnt, message_id, CONFIG_JA_LOG_MESSAGE_FILE);
		return FAIL;
	}

	/* log type check */
	log_level = LOG_LEVEL_INFORMATION;
	log_type  = atoi(type);
	switch (log_type)
	{
		case JALOG_TYPE_INFO:
			log_level = LOG_LEVEL_INFORMATION;
			break;

		case JALOG_TYPE_CRIT:
			log_level = LOG_LEVEL_CRIT;
			break;

		case JALOG_TYPE_ERR:
			log_level = LOG_LEVEL_ERR;
			break;

		case JALOG_TYPE_WARN:
			log_level = LOG_LEVEL_WARNING;
			break;

		case JALOG_TYPE_DEBUG:
			log_level = LOG_LEVEL_DEBUG;
			break;

		default:
			zabbix_log(LOG_LEVEL_ERR, "detected an invalid log type: line(%d) type[%s] id[%s] file[%s]",
				cnt, type, message_id, CONFIG_JA_LOG_MESSAGE_FILE);
			return FAIL;
	}

	/* send flad check */
	send_flag = atoi(send);
	if (send_flag == JASENDER_OFF || send_flag == JASENDER_ON)
	{
		/* OK */
	}
	else
	{
		zabbix_log(LOG_LEVEL_ERR, "detected an invalid jasender flag: line(%d) flag[%s] id[%s] file[%s]",
			cnt, send, message_id, CONFIG_JA_LOG_MESSAGE_FILE);
		return FAIL;
	}

	/* message body edit */
	va_start(ap, inner_job_id);
	message = zbx_dvsprintf(message, msg, ap);
	va_end(ap);

	if (strlen(message) > AP_MESSAGE_BUF_SIZE)
	{
		*(message + (AP_MESSAGE_BUF_SIZE - 1)) = '\0';
	}

	/* get the current time */
	time(&now);
	tm = localtime(&now);
	strftime(now_time, sizeof(now_time), "%Y%m%d%H%M%S", tm);
	now_date = zbx_dsprintf(NULL, "%04d/%02d/%02d %02d:%02d:%02d",
				(tm->tm_year + 1900),
				(tm->tm_mon  + 1),
				 tm->tm_mday,
				 tm->tm_hour,
				 tm->tm_min,
				 tm->tm_sec);

	/* get the inner jobnet id */
	if (w_inner_jobnet_id == 0 && inner_job_id != 0)
	{
		result = DBselect("select inner_jobnet_id from ja_run_job_table"
				" where inner_job_id = " ZBX_FS_UI64,
				inner_job_id);

		if (NULL != (row = DBfetch(result)))
		{
			ZBX_STR2UINT64(w_inner_jobnet_id, row[0]);
		}
		DBfree_result(result);
	}

	/* get the host name and job id */
	if (inner_job_id != 0)
	{
		zbx_strlcpy(s_job_id_full, ja_get_jobid(inner_job_id), sizeof(s_job_id_full));

		job_type = -1;
		result = DBselect("select job_type, job_id, job_name"
				" from ja_run_job_table"
				" where inner_job_id = " ZBX_FS_UI64,
				inner_job_id);

		if (NULL != (row = DBfetch(result)))
		{
			job_type = atoi(row[0]);
			zbx_strlcpy(s_job_id, row[1], sizeof(s_job_id));
			if (SUCCEED != DBis_null(row[2]))
			{
				zbx_strlcpy(job_name, row[2], sizeof(job_name));
			}
		}

		DBfree_result(result);

		get_host_flag = 0;
		switch (job_type)
		{
			case JA_JOB_TYPE_JOB:
				get_host_flag = 1;
				result = DBselect("select host_flag, host_name"
						" from ja_run_icon_job_table"
						" where inner_job_id = " ZBX_FS_UI64, inner_job_id);
				break;

			case JA_JOB_TYPE_FCOPY:
				get_host_flag = 1;
				result = DBselect("select from_host_flag, from_host_name"
						" from ja_run_icon_fcopy_table"
						" where inner_job_id = " ZBX_FS_UI64, inner_job_id);
				break;

			case JA_JOB_TYPE_FWAIT:
				get_host_flag = 1;
				result = DBselect("select host_flag, host_name"
						" from ja_run_icon_fwait_table"
						" where inner_job_id = " ZBX_FS_UI64, inner_job_id);
				break;

			case JA_JOB_TYPE_REBOOT:
				get_host_flag = 1;
				result = DBselect("select host_flag, host_name"
						" from ja_run_icon_reboot_table"
						" where inner_job_id = " ZBX_FS_UI64, inner_job_id);
				break;

			case JA_JOB_TYPE_LESS:
				get_host_flag = 1;
				result = DBselect("select host_flag, host_name"
						" from ja_run_icon_agentless_table"
						" where inner_job_id = " ZBX_FS_UI64, inner_job_id);
				break;
		}

		/* host name acquisition target icon */
		if (get_host_flag != 0)
		{
			if (NULL != (row = DBfetch(result)))
			{
				if (SUCCEED != DBis_null(row[1]))
				{
					host_flag = atoi(row[0]);
					if (host_flag == USE_HOSTNAME)
					{
						zbx_strlcpy(host_name, row[1], sizeof(host_name));
					}
					else
					{
						/* get the host name from the job controller variable */
						result2 = DBselect("select before_value"
								" from ja_run_value_before_table"
								" where inner_job_id = " ZBX_FS_UI64 " and value_name = '%s'",
								inner_job_id, row[1]);

						if (NULL != (row2 = DBfetch(result2)))
						{
							zbx_strlcpy(host_name, row2[0], sizeof(host_name));
						}
						DBfree_result(result2);
					}
				}
			}
			DBfree_result(result);
		}
	}

	/* get the user id */
	if (w_inner_jobnet_id != 0 || jobnet_id != NULL)
	{
		if (w_inner_jobnet_id != 0)
		{
			result = DBselect("select inner_jobnet_id, inner_jobnet_main_id, jobnet_id, user_name, jobnet_name"
					" from ja_run_jobnet_table"
					" where inner_jobnet_id = " ZBX_FS_UI64,
					w_inner_jobnet_id);

			if (NULL != (row = DBfetch(result)))
			{
				ZBX_STR2UINT64(inner_jobnet_main_id, row[1]);
				if (strcmp(row[0], row[1]) != 0)
				{
					result2 = DBselect("select jobnet_id, user_name, jobnet_name"
							" from ja_run_jobnet_table"
							" where inner_jobnet_id = %s",
							row[1]);

					if (NULL != (row2 = DBfetch(result2)))
					{
						zbx_strlcpy(s_jobnet_id, row2[0], sizeof(s_jobnet_id));
						zbx_strlcpy(s_user_name, row2[1], sizeof(s_user_name));
						zbx_strlcpy(jobnet_name, row2[2], sizeof(jobnet_name));
					}
					DBfree_result(result2);
				}
				else
				{
					zbx_strlcpy(s_jobnet_id, row[2], sizeof(s_jobnet_id));
					zbx_strlcpy(s_user_name, row[3], sizeof(s_user_name));
					zbx_strlcpy(jobnet_name, row[4], sizeof(jobnet_name));
				}
			}
		}
		else
		{
			result = DBselect("select jobnet_id, user_name, jobnet_name"
					" from ja_jobnet_control_table"
					" where jobnet_id = '%s' and valid_flag = %d",
					jobnet_id, VALID_FLAG_ON);

			if (NULL != (row = DBfetch(result)))
			{
				zbx_strlcpy(s_jobnet_id, row[0], sizeof(s_jobnet_id));
				zbx_strlcpy(s_user_name, row[1], sizeof(s_user_name));
				zbx_strlcpy(jobnet_name, row[2], sizeof(jobnet_name));
			}
		}
		DBfree_result(result);
	}

	/* log write */
	zabbix_log(log_level, "[%s] %s", message_id, message);

	/* register the error message variable */
	if (w_inner_jobnet_id == 0 || inner_job_id == 0 ||
	    log_level == LOG_LEVEL_INFORMATION || log_level == LOG_LEVEL_DEBUG)
	{
		/* skip the registration */
	}
	else
	{
		after_value_esc = DBdyn_escape_string(message);
		/* to add a variable */
		rc = DBexecute("insert into ja_run_value_after_table ("
				"inner_job_id, inner_jobnet_id, value_name, after_value) "
				"values (" ZBX_FS_UI64 ", " ZBX_FS_UI64 ", '%s', '[%s] %s')",
				inner_job_id, w_inner_jobnet_id, JOBARG_MESSAGE, message_id, after_value_esc);

		if (rc <= ZBX_DB_OK)
		{
			zabbix_log(LOG_LEVEL_ERR, "failed to insert the ja_run_value_after_table (ja_log): "
				" message id[%s] inner job id[" ZBX_FS_UI64 "] value name[%s]",
				message_id, inner_job_id, JOBARG_MESSAGE);
			zbx_free(after_value_esc);
			zbx_free(message);
			zbx_free(now_date);
			return FAIL;
		}
		zbx_free(after_value_esc);
	}

	/* no notification of message ? */
	if (send_flag == JASENDER_OFF)
	{
		zbx_free(message);
		zbx_free(now_date);
		return SUCCEED;
	}

	/* get zabbix notification existence */
	zbxsnd = ja_get_zbxsnd_on();

	/* with no notice of Zabbix ? */
	if (zbxsnd == ZBXSND_NOTICE_ON)
	{
		/* registration of the send message information */
		after_value_esc = DBdyn_escape_string(message);
		user_name_esc   = DBdyn_escape_string(s_user_name);
		host_name_esc   = DBdyn_escape_string(host_name);
		jobnet_name_esc = DBdyn_escape_string(jobnet_name);
		job_name_esc    = DBdyn_escape_string(job_name);

		rc = DBexecute("insert into ja_send_message_table ("
				"message_date, inner_jobnet_id, inner_jobnet_main_id, send_status, retry_count, retry_date, send_date, send_error_date, "
				"message_type, user_name, host_name, jobnet_id, jobnet_name, job_id, job_id_full, job_name, "
				"log_message_id, log_message) "
				"values (%s, " ZBX_FS_UI64 ", " ZBX_FS_UI64 ", %d, 0, 0, 0, 0, "
				"%d, '%s', '%s', '%s', '%s', '%s', '%s', '%s', "
				"'%s', '%s')",
				now_time, w_inner_jobnet_id, inner_jobnet_main_id, JA_SNT_SEND_STATUS_BEGIN,
				log_type, user_name_esc, host_name_esc, s_jobnet_id, jobnet_name_esc, s_job_id, s_job_id_full, job_name_esc,
				message_id, after_value_esc);

		if (rc <= ZBX_DB_OK)
		{
			zabbix_log(LOG_LEVEL_ERR, "failed to insert the ja_send_message_table (ja_log): "
				" message date[%s] message id[%s]",
				now_time, message_id);
			zbx_free(after_value_esc);
			zbx_free(user_name_esc);
			zbx_free(host_name_esc);
			zbx_free(jobnet_name_esc);
			zbx_free(job_name_esc);
			zbx_free(message);
			zbx_free(now_date);
			return FAIL;
		}
		zbx_free(after_value_esc);
		zbx_free(user_name_esc);
		zbx_free(host_name_esc);
		zbx_free(jobnet_name_esc);
		zbx_free(job_name_esc);

		/* application execution error notification */
		dir = opendir(CONFIG_ERROR_CMD_PATH);
		if (dir == NULL)
		{
			zabbix_log(LOG_LEVEL_ERR, "failed to open the error notification directory: [%s]", CONFIG_ERROR_CMD_PATH);
			zbx_free(message);
			zbx_free(now_date);
			return FAIL;
		}

		while ((dp = readdir(dir)) != NULL)
		{
			if (dp->d_name[0] == '.')
			{
				continue;
			}
			/* start command in the background */

			str_inner_jobnet_main_id = NULL;
			str_inner_jobnet_main_id = zbx_dsprintf(NULL,"" ZBX_FS_UI64 "" ,inner_jobnet_main_id);
			zbx_snprintf(cmd, sizeof(cmd), "%s/%s '%s' '%s' '%s' '%s' '%s' '%s' '%s' '%s' '%s' &",
				CONFIG_ERROR_CMD_PATH, dp->d_name, s_user_name, s_jobnet_id, now_date, message_id, type, message, host_name, s_job_id_full,str_inner_jobnet_main_id);
			rc = ja_system_call(cmd);
			zabbix_log(LOG_LEVEL_DEBUG, "application execution [%s] (%d)", cmd, rc);
			zbx_free(str_inner_jobnet_main_id);
			if (rc != EXIT_SUCCESS)
			{
				if (WIFEXITED(rc))
				{
					state = WEXITSTATUS(rc);
				}
				else
				{
					state = rc;
				}
				zabbix_log(LOG_LEVEL_ERR, "failed to run the error notification application: (%d) [%s]", state, cmd);
				zbx_free(message);
				zbx_free(now_date);
				closedir(dir);
				return FAIL;
			}
		}
		closedir(dir);

		zbx_free(message);
		zbx_free(now_date);
	}

	return SUCCEED;
}
