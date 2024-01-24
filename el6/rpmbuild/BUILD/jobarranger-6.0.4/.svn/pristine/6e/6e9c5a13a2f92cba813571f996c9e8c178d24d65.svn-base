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
#include "daemon.h"
#include "zbxserver.h"
#include "zbxself.h"
#include "../events.h"

#include "jacommon.h"
#include "jalog.h"
#include "jaself.h"
#include "jaindex.h"
#include "jaloader.h"

#define JOBNET_LOAD_SPAN	"JOBNET_LOAD_SPAN"

#define DEFAULT_LOAD_SPAN	60

#define VALID_FLAG_ON		1
#define VALID_FLAG_OFF		0

#define UPDATE_DATE_LEN		14 + 1
#define OPERATING_DATE_LEN	8  + 1
#define SCHEDULED_TIME_LEN	12 + 1

extern unsigned char	process_type;
extern int		process_num;

static int		no_valid_flag;
static char		msgwork[2048];


/******************************************************************************
 *                                                                            *
 * Function: get_end_of_month                                                 *
 *                                                                            *
 * Purpose: get the last day of the specified month                           *
 *                                                                            *
 * Parameters: p_date (in) - specified month (YYYYMM)                         *
 *                                                                            *
 * Return value: last day of the month                                        *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_end_of_month(char *p_date)
{
	int		leap_year, year, month, day;
	char		w_year[5], w_month[3];
	const char	*__function_name = "get_end_of_month";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s)", __function_name, p_date);

	zbx_strlcpy(w_year,   p_date,      5);
	zbx_strlcpy(w_month, (p_date + 4), 3);

	year  = atoi(w_year);
	month = atoi(w_month);

        /* check of the month */
	switch (month)
	{
		case 2:  /* February */
			leap_year = 0;
			if ((year % 4) == 0)
			{
				leap_year = 1;
				if ((year % 100) == 0)
				{
					leap_year = 0;
					if ((year % 400) == 0)
					{
						leap_year = 1;
					}
				}
			}

			/* leap year ? */
			if (leap_year == 1)
			{
				day = 29;
			}
			else
			{
				day = 28;
			}
			break;

		case 4:  /* April */
			day = 30;
			break;

		case 6:  /* June */
			day = 30;
			break;

		case 9:  /* September */
			day = 30;
			break;

		case 11: /* November */
			day = 30;
			break;

		default: /* other month */
			day = 31;
			break;
	}

	return day;
}

/******************************************************************************
 *                                                                            *
 * Function: get_search_date                                                  *
 *                                                                            *
 * Purpose: calculate the search time of the schedule                         *
 *                                                                            *
 * Parameters: span (in) - deployment time of a jobnet (minute)               *
 *             start_date (out) - search start time (YYYYMMDDHHMM)            *
 *             end_date (out) - search end time (current time) (YYYYMMDDHHMM) *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void	get_search_date(int span, char **start_date, char **end_date)
{
	struct tm	*tm;
	time_t		now;
	const char	*__function_name = "get_search_date";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%d)", __function_name, span);

	time(&now);
	tm = localtime(&now);

	*end_date = zbx_dsprintf(NULL, "%04d%02d%02d%02d%02d",
				(tm->tm_year + 1900),
				(tm->tm_mon  + 1),
				 tm->tm_mday,
				 tm->tm_hour,
				 tm->tm_min);

	tm->tm_min = tm->tm_min + span;
	mktime(tm);

	*start_date = zbx_dsprintf(NULL, "%04d%02d%02d%02d%02d",
				(tm->tm_year + 1900),
				(tm->tm_mon  + 1),
				 tm->tm_mday,
				 tm->tm_hour,
				 tm->tm_min);

	return;
}

/******************************************************************************
 *                                                                            *
 * Function: shift_month                                                      *
 *                                                                            *
 * Purpose: shift the month by a specified number of months                   *
 *                                                                            *
 * Parameters: edit_date (in/out) - date to be changed (YYYYMM)               *
 *             months (in) - number of months to shift                        *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void	shift_month(char *edit_date, int months)
{
	struct tm	tm;
	int		i_year, i_mon;
	char		year[5], mon[3];
	const char	*__function_name = "shift_month";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %d)", __function_name, edit_date, months);

	zbx_strlcpy(year,  edit_date,      5);
	zbx_strlcpy(mon,  (edit_date + 4), 3);

	i_year = atoi(year);
	i_mon  = atoi(mon);

	tm.tm_year  = i_year - 1900;
	tm.tm_mon   = (i_mon - 1) + months;
	tm.tm_mday  = 1;
	tm.tm_hour  = 0;
	tm.tm_min   = 0;
	tm.tm_sec   = 0;
	tm.tm_isdst = -1;

	mktime(&tm);

	zbx_snprintf(edit_date, OPERATING_DATE_LEN, "%04d%02d",
		(tm.tm_year + 1900),
		(tm.tm_mon  + 1));

	return;
}

/******************************************************************************
 *                                                                            *
 * Function: shift_days                                                       *
 *                                                                            *
 * Purpose: shift the day by a specified number of days                       *
 *                                                                            *
 * Parameters: edit_date (in/out) - date to be changed (YYYYMMDD)             *
 *             days (in) - number of days to shift                            *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void	shift_days(char *edit_date, int days)
{
	struct tm	tm;
	int		i_year, i_mon, i_day;
	char		year[5], mon[3], day[3];
	const char	*__function_name = "shift_days";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %d)", __function_name, edit_date, days);

	zbx_strlcpy(year,  edit_date,      5);
	zbx_strlcpy(mon,  (edit_date + 4), 3);
	zbx_strlcpy(day,  (edit_date + 6), 3);

	i_year = atoi(year);
	i_mon  = atoi(mon);
	i_day  = atoi(day);

	tm.tm_year  = i_year - 1900;
	tm.tm_mon   = i_mon - 1;
	tm.tm_mday  = i_day + days;
	tm.tm_hour  = 0;
	tm.tm_min   = 0;
	tm.tm_sec   = 0;
	tm.tm_isdst = -1;

	mktime(&tm);

	zbx_snprintf(edit_date, OPERATING_DATE_LEN, "%04d%02d%02d",
		(tm.tm_year + 1900),
		(tm.tm_mon  + 1),
		 tm.tm_mday);

	return;
}

/******************************************************************************
 *                                                                            *
 * Function: get_load_span                                                    *
 *                                                                            *
 * Purpose: gets the deployment time of a jobnet                              *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value: deployment time jobnet                                       *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_load_span()
{
	DB_RESULT	result;
	DB_ROW		row;
	int		span = DEFAULT_LOAD_SPAN;
	const char	*__function_name = "get_load_span";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	result = DBselect("select value from ja_parameter_table where parameter_name = '%s'", JOBNET_LOAD_SPAN);

	if (NULL != (row = DBfetch(result)))
	{
		if (strlen(row[0]) <= 10) {
			span = atoi(row[0]);
		}
		if (span < 1)
		{
			span = DEFAULT_LOAD_SPAN;
		}
	}
	DBfree_result(result);

	return span;
}

/******************************************************************************
 *                                                                            *
 * Function: make_end_operating_date                                          *
 *                                                                            *
 * Purpose: edit the end date search                                          *
 *                                                                            *
 * Parameters: end_ope_date (out) - search end time (YYYYMMDD)                *
 *             end_date (in) - search end time (current time) (YYYYMMDDHHMM)  *
 *             boot_time (in) - boot time (HHMM 0000-9959)                    *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void	make_end_operating_date(char *end_ope_date, char *end_date, char *boot_time)
{
	struct tm	tm;
	int		i_year, i_mon, i_day, i_bt, i_sday;
	char		year[5], mon[3], day[3];
	char		*edit_date;
	const char	*__function_name = "make_end_operating_date";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s)", __function_name, end_date, boot_time);

	i_bt = atoi(boot_time);
	if (i_bt < 2400)
	{
		zbx_strlcpy(end_ope_date, end_date, OPERATING_DATE_LEN);
		return;
	}

	zbx_strlcpy(year,  end_date,      5);
	zbx_strlcpy(mon,  (end_date + 4), 3);
	zbx_strlcpy(day,  (end_date + 6), 3);

	i_year = atoi(year);
	i_mon  = atoi(mon);
	i_day  = atoi(day);

	i_sday = i_bt / 2400;

	tm.tm_year  = i_year - 1900;
	tm.tm_mon   = i_mon - 1;
	tm.tm_mday  = i_day - i_sday;
	tm.tm_hour  = 0;
	tm.tm_min   = 0;
	tm.tm_sec   = 0;
	tm.tm_isdst = -1;

	mktime(&tm);

	edit_date = zbx_dsprintf(NULL, "%04d%02d%02d",
				(tm.tm_year + 1900),
				(tm.tm_mon  + 1),
				 tm.tm_mday);

	zbx_strlcpy(end_ope_date, edit_date, OPERATING_DATE_LEN);

	zbx_free(edit_date);
	return;
}

/******************************************************************************
 *                                                                            *
 * Function: make_scheduled_time                                              *
 *                                                                            *
 * Purpose: edit a scheduled time                                             *
 *                                                                            *
 * Parameters: scheduled_time (out) - time schedule that has been edited      *
 *                                    (YYYYMMDDHHMM)                          *
 *             operating_date (in) - operating date (YYYYMMDD)                *
 *             boot_time (in) - boot time (HHMM 0000-9959)                    *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void	make_scheduled_time(char *scheduled_time, char *operating_date, char *boot_time)
{
	struct tm	tm;
	int		i_year, i_mon, i_day, i_bt, i_sday, w_bt;
	char		year[5], mon[3], day[3];
	char		*edit_date;
	const char	*__function_name = "make_scheduled_time";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s)", __function_name, operating_date, boot_time);

	i_bt = atoi(boot_time);
	if (i_bt < 2400)
	{
		zbx_strlcpy(scheduled_time, operating_date, SCHEDULED_TIME_LEN);
		zbx_strlcpy((scheduled_time + 8), boot_time, 5);
		return;
	}

	zbx_strlcpy(year,  operating_date,      5);
	zbx_strlcpy(mon,  (operating_date + 4), 3);
	zbx_strlcpy(day,  (operating_date + 6), 3);

	i_year = atoi(year);
	i_mon  = atoi(mon);
	i_day  = atoi(day);

	i_sday = i_bt / 2400;
	w_bt   = i_bt - (2400 * i_sday);

	tm.tm_year  = i_year - 1900;
	tm.tm_mon   = i_mon - 1;
	tm.tm_mday  = i_day + i_sday;
	tm.tm_hour  = 0;
	tm.tm_min   = 0;
	tm.tm_sec   = 0;
	tm.tm_isdst = -1;

	mktime(&tm);

	edit_date = zbx_dsprintf(NULL, "%04d%02d%02d%04d",
				(tm.tm_year + 1900),
				(tm.tm_mon  + 1),
				 tm.tm_mday,
				 w_bt);

	zbx_strlcpy(scheduled_time, edit_date, 13);

	zbx_free(edit_date);
	return;
}

/******************************************************************************
 *                                                                            *
 * Function: already_start_check_immediate                                    *
 *                                                                            *
 * Purpose: check if the immediate start jobnet is already deployed           *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *                                                                            *
 * Return value: SUCCEED - not started                                        *
 *               FAIL - already started                                       *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	already_start_check_immediate(zbx_uint64_t inner_jobnet_id)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		count;
	const char	*__function_name = "already_start_check_immediate";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 ")", __function_name, inner_jobnet_id);

	result = DBselect("select count(*) from ja_run_jobnet_summary_table"
			" where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

	if (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64, inner_jobnet_id);
		ja_log("JALOADER200001", inner_jobnet_id, NULL, 0, "ja_run_jobnet_summary_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	count = atoi(row[0]);
	DBfree_result(result);

	if (count > 0)
	{
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_flow_inner_job_id                                            *
 *                                                                            *
 * Purpose: gets the internal job id for the job flow                         *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             job_id (in) - job id                                           *
 *                                                                            *
 * Return value: internal job id                                              *
 *               FAIL - an error occurred                                     *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static zbx_uint64_t	get_flow_inner_job_id(zbx_uint64_t inner_jobnet_id, char *job_id)
{
	DB_RESULT	result;
	DB_ROW		row;
	zbx_uint64_t	inner_job_id;
	const char	*__function_name = "get_flow_inner_job_id";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " %s)",
		__function_name, inner_jobnet_id, job_id);

	result = DBselect("select inner_job_id from ja_run_job_table"
			" where inner_jobnet_id = " ZBX_FS_UI64 " and job_id = '%s'",
			inner_jobnet_id, job_id);

	if (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), ZBX_FS_UI64 " %s", inner_jobnet_id, job_id);
		ja_log("JALOADER200001", inner_jobnet_id, NULL, 0, "ja_run_job_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	ZBX_STR2UINT64(inner_job_id, row[0]);

	DBfree_result(result);

	return inner_job_id;
}

/******************************************************************************
 *                                                                            *
 * Function: get_boot_count                                                   *
 *                                                                            *
 * Purpose: gets the total number of flow                                     *
 *                                                                            *
 * Parameters: jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value: number of flows                                              *
 *               FAIL - an error occurred                                     *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_boot_count(char *jobnet_id, char *job_id, char *update_date)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		count;
	const char	*__function_name = "get_boot_count";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s %s)", __function_name, jobnet_id, job_id, update_date);

	result = DBselect("select count(*) from ja_flow_control_table"
			" where jobnet_id = '%s' and end_job_id = '%s' and update_date = %s",
			jobnet_id, job_id, update_date);

	if (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200001", 0, jobnet_id, 0, "ja_flow_control_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	count = atoi(row[0]);

	DBfree_result(result);

	return count;
}

/******************************************************************************
 *                                                                            *
 * Function: load_value                                                       *
 *                                                                            *
 * Purpose: expand job variable                                               *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_value(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_value";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);


	/* ja_run_value_job_table additional data */
	rc = DBexecute("insert into ja_run_value_job_table ("
			" inner_job_id, inner_jobnet_id,"
			" value_name, value)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', value_name, value"
			" from ja_value_job_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc < ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_value_job_table", msgwork);
		return FAIL;
	}


	/* ja_run_value_jobcon_table additional data */
	rc = DBexecute("insert into ja_run_value_jobcon_table ("
			" inner_job_id, inner_jobnet_id, value_name)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', value_name"
			" from ja_value_jobcon_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc < ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_value_jobcon_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_end                                                    *
 *                                                                            *
 * Purpose: end icon to expand the information                                *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_end(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		rc, jobnet_stop_flag;
	const char	*__function_name = "load_icon_end";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_end_table ("
			" inner_job_id, inner_jobnet_id, jobnet_stop_flag, jobnet_stop_code)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', jobnet_stop_flag, jobnet_stop_code"
			" from ja_icon_end_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_end_table", msgwork);
		return FAIL;
	}

	/* get jobnet stop flag */
	result = DBselect("select jobnet_stop_flag from ja_icon_end_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			jobnet_id, job_id, update_date);

	if (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200001", 0, jobnet_id, 0, "ja_icon_end_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	jobnet_stop_flag = atoi(row[0]);
	if (jobnet_stop_flag == 1)
	{
		result = DBselect("select inner_job_id,method_flag from ja_run_job_table where  inner_job_id = " ZBX_FS_UI64,inner_job_id);
		if (NULL == (row = DBfetch(result)))
		{
			zabbix_log(LOG_LEVEL_ERR,"In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s  not found ja_run_job_table )",
					__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);
		}else{
			if (jobnet_stop_flag != atoi(row[1]))
			{
				rc = DBexecute("update ja_run_job_table set method_flag = %d where inner_job_id = " ZBX_FS_UI64,
								JA_JOB_METHOD_WAIT, inner_job_id);

			}

			if (rc <= ZBX_DB_OK)
			{
				zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
				ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_job_table", msgwork);
				DBfree_result(result);
				return FAIL;
			}
		}
	}

	DBfree_result(result);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_if                                                     *
 *                                                                            *
 * Purpose: if icon to expand the information                                 *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_if(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_if";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_if_table ("
			" inner_job_id, inner_jobnet_id, hand_flag, value_name, comparison_value)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', hand_flag, value_name, comparison_value"
			" from ja_icon_if_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_icon_if_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_value                                                  *
 *                                                                            *
 * Purpose: job controller variable icon to expand the information            *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_value(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_value";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_value_table ("
			" inner_job_id, inner_jobnet_id, value_name, value)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', value_name, value"
			" from ja_icon_value_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_value_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_job                                                    *
 *                                                                            *
 * Purpose: job icon to expand the information                                *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_job(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_job";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);


	/* ja_run_icon_job_table additional data */
	rc = DBexecute("insert into ja_run_icon_job_table ("
			" inner_job_id, inner_jobnet_id, host_flag, stop_flag,"
			" command_type, timeout, host_name, stop_code, timeout_run_type)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', host_flag, stop_flag,"
			" command_type, timeout, host_name, stop_code, timeout_run_type"
			" from ja_icon_job_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_job_table", msgwork);
		return FAIL;
	}


	/* ja_run_job_command_table additional data */
	rc = DBexecute("insert into ja_run_job_command_table ("
			" inner_job_id, inner_jobnet_id,"
			" command_cls, command)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', command_cls, command"
			" from ja_job_command_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_job_command_table", msgwork);
		return FAIL;
	}

	/* expand the job variable */
	rc = load_value(inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	return rc;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_jobnet                                                 *
 *                                                                            *
 * Purpose: jobnet icon to expand the information                             *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_jobnet(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	zbx_uint64_t	link_inner_jobnet_id;
	const char	*__function_name = "load_icon_jobnet";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	link_inner_jobnet_id = get_next_id(JA_RUN_ID_JOBNET_LD, 0, jobnet_id, 0);
	if (link_inner_jobnet_id == 0)
	{
		return FAIL;
	}

	rc = DBexecute("insert into ja_run_icon_jobnet_table ("
			" inner_job_id, inner_jobnet_id, link_inner_jobnet_id, link_jobnet_id)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', link_jobnet_id"
			" from ja_icon_jobnet_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, link_inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_jobnet_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_extjob                                                 *
 *                                                                            *
 * Purpose: extended job icon to expand the information                       *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_extjob(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_extjob";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_extjob_table ("
			" inner_job_id, inner_jobnet_id, command_id, value, pid)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', command_id, value, 0"
			" from ja_icon_extjob_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_extjob_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_calc                                                   *
 *                                                                            *
 * Purpose: calculation icon to expand the information                        *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_calc(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_calc";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_calc_table ("
			" inner_job_id, inner_jobnet_id, hand_flag, formula, value_name)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', hand_flag, formula, value_name"
			" from ja_icon_calc_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_calc_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_task                                                   *
 *                                                                            *
 * Purpose: task icon to expand the information                               *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_task(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_task";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_task_table ("
			" inner_job_id, inner_jobnet_id, submit_inner_jobnet_id, submit_jobnet_id)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', 0, submit_jobnet_id"
			" from ja_icon_task_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_task_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_info                                                   *
 *                                                                            *
 * Purpose: information acquisition icon to expand the information            *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_info(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_info";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_info_table ("
			" inner_job_id, inner_jobnet_id, info_flag, item_id,"
			" trigger_id, host_group, host_name, get_job_id, get_calendar_id)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', info_flag, item_id,"
			" trigger_id, host_group, host_name, get_job_id, get_calendar_id"
			" from ja_icon_info_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_info_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_fcopy                                                  *
 *                                                                            *
 * Purpose: file copy icon to expand the information                          *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_fcopy(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_fcopy";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_fcopy_table ("
			" inner_job_id, inner_jobnet_id, from_host_flag,"
			" to_host_flag, overwrite_flag, from_host_name, from_directory,"
			" from_file_name, to_host_name, to_directory)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', from_host_flag,"
			" to_host_flag, overwrite_flag, from_host_name, from_directory,"
			" from_file_name, to_host_name, to_directory"
			" from ja_icon_fcopy_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_fcopy_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_fwait                                                  *
 *                                                                            *
 * Purpose: file wait icon to expand the information                          *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_fwait(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_fwait";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_fwait_table ("
			" inner_job_id, inner_jobnet_id,"
			" host_flag, fwait_mode_flag, file_delete_flag, file_wait_time,"
			" host_name, file_name)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "',"
			" host_flag, fwait_mode_flag, file_delete_flag, file_wait_time,"
			" host_name, file_name"
			" from ja_icon_fwait_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_fwait_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_reboot                                                 *
 *                                                                            *
 * Purpose: reboot icon to expand the information                             *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_reboot(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_reboot";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_reboot_table ("
			" inner_job_id, inner_jobnet_id,"
			" host_flag, reboot_mode_flag, reboot_wait_time, host_name, timeout)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "',"
			" host_flag, reboot_mode_flag, reboot_wait_time, host_name, timeout"
			" from ja_icon_reboot_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_reboot_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_rel                                                    *
 *                                                                            *
 * Purpose: suspension release icon to expand the information                 *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_rel(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_rel";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_release_table ("
			" inner_job_id, inner_jobnet_id, release_job_id)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', release_job_id"
			" from ja_icon_release_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_release_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_less                                                   *
 *                                                                            *
 * Purpose: suspension agentless icon to expand the information               *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_less(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		session_flag, rc;
	zbx_uint64_t	session_id_num;
	char		session_id[65];
	const char	*__function_name = "load_icon_less";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	/* expand the information icon */
	result = DBselect("select session_flag, session_id"
			" from ja_icon_agentless_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			jobnet_id, job_id, update_date);

	while (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200001", inner_jobnet_id, NULL, 0, "ja_icon_agentless_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	session_flag = atoi(row[0]);

	/* one-time session ? */
	if (session_flag == 0)
	{
		session_id_num = get_next_id(JA_RUN_ID_SESSION, 0, jobnet_id, 0);
		if (session_id_num == 0)
		{
			DBfree_result(result);
			return FAIL;
		}
		zbx_snprintf(session_id, sizeof(session_id), "@SESSION-" ZBX_FS_UI64, session_id_num);
	}
	else
	{
		zbx_strlcpy(session_id, row[1], sizeof(session_id));
	}

	DBfree_result(result);

	rc = DBexecute("insert into ja_run_icon_agentless_table ("
			" inner_job_id, inner_jobnet_id, host_flag, connection_method, session_flag,"
			" auth_method, run_mode, line_feed_code, timeout, session_id, login_user, login_password,"
			" public_key, private_key, passphrase, host_name, stop_code, terminal_type, character_code, prompt_string, command)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', host_flag, connection_method, session_flag,"
			" auth_method, run_mode, line_feed_code, timeout, '%s', login_user, login_password,"
			" public_key, private_key, passphrase, host_name, stop_code, terminal_type, character_code, prompt_string, command"
			" from ja_icon_agentless_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, session_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_agentless_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_icon_link                                                   *
 *                                                                            *
 * Purpose: suspension zabbix cooperation icon to expand the information      *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             jobnet_id (in) - jobnet id                                     *
 *             job_id (in) - job id                                           *
 *             update_date (in) - update date                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_icon_link(zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id,
				char *jobnet_id, char *job_id, char *update_date)
{
	int		rc;
	const char	*__function_name = "load_icon_link";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %s)",
		__function_name, inner_jobnet_id, inner_job_id, jobnet_id, job_id, update_date);

	rc = DBexecute("insert into ja_run_icon_zabbix_link_table ("
			" inner_job_id, inner_jobnet_id, link_target, link_operation,"
			" groupid, hostid, itemid, triggerid)"
			" select '" ZBX_FS_UI64 "','" ZBX_FS_UI64 "', link_target, link_operation,"
			" groupid, hostid, itemid, triggerid"
			" from ja_icon_zabbix_link_table"
			" where jobnet_id = '%s' and job_id = '%s' and update_date = %s",
			inner_job_id, inner_jobnet_id, jobnet_id, job_id, update_date);

	if (rc <= ZBX_DB_OK)
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", jobnet_id, job_id, update_date);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_icon_zabbix_link_table", msgwork);
		return FAIL;
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_job_definition                                              *
 *                                                                            *
 * Purpose: expand the job definition                                         *
 *                                                                            *
 * Parameters: inner_jobnet_main_id (in) - inner jobnet main id               *
 *             inner_jobnet_id (in) - inner jobnet id                         *
 *             jobnet_id (in) - jobnet id                                     *
 *             update_date (in) - update date                                 *
 *             test_flag (in) - test flag                                     *
 *             run_type (in) - run type                                       *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_job_definition(zbx_uint64_t inner_jobnet_main_id, zbx_uint64_t inner_jobnet_id,
					char *jobnet_id, char *update_date, int test_flag, int run_type)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		rc, job_type, boot_count;
	char		*job_name, *job_name_esc = NULL;
	char		*run_user, *run_user_esc = NULL;
	char		*run_user_password, *run_user_password_esc = NULL;
	zbx_uint64_t	inner_job_id, inner_job_id_fs_link, inner_flow_id, start_inner_job_id, end_inner_job_id;
	const char	*__function_name = "load_job_definition";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " " ZBX_FS_UI64 " %s %s %d %d)",
		__function_name, inner_jobnet_main_id, inner_jobnet_id, jobnet_id, update_date,
		test_flag, run_type);

	/* expand the information icon */
	result = DBselect("select jobnet_id, job_id, update_date, job_type, point_x, point_y,"
			" job_name, method_flag, force_flag, continue_flag, run_user, run_user_password"
			" from ja_job_control_table"
			" where jobnet_id = '%s' and update_date = %s",
			jobnet_id, update_date);

	while (NULL != (row = DBfetch(result)))
	{
		job_type = atoi(row[3]);

		inner_job_id = get_next_id(JA_RUN_ID_JOB, 0, jobnet_id, 0);
		if (inner_job_id == 0)
		{
			DBfree_result(result);
			return FAIL;
		}

		inner_job_id_fs_link = 0;
		if (job_type == JA_JOB_TYPE_JOB)
		{
			inner_job_id_fs_link = get_next_id(JA_RUN_ID_JOB, 0, jobnet_id, 0);
			if (inner_job_id_fs_link == 0)
			{
				DBfree_result(result);
				return FAIL;
			}
		}

		boot_count = get_boot_count(row[0], row[1], row[2]);
		if (boot_count == FAIL)
		{
			DBfree_result(result);
			return FAIL;
		}

		if (SUCCEED != DBis_null(row[6]))
		{
			job_name_esc = DBdyn_escape_string(row[6]);
			job_name     = job_name_esc;
		}
		else
		{
			job_name = "";
		}

		if (SUCCEED != DBis_null(row[10]))
		{
			run_user_esc = DBdyn_escape_string(row[10]);
			run_user     = run_user_esc;
		}
		else
		{
			run_user = "";
		}

		if (SUCCEED != DBis_null(row[11]))
		{
			run_user_password_esc = DBdyn_escape_string(row[11]);
			run_user_password     = run_user_password_esc;
		}
		else
		{
			run_user_password = "";
		}

		rc = DBexecute("insert into ja_run_job_table ("
				" inner_job_id, inner_jobnet_id, inner_jobnet_main_id, inner_job_id_fs_link,"
				" invo_flag, job_type, test_flag, method_flag, force_flag, timeout_flag, status,"
				" boot_count, end_count, start_time, end_time, point_x, point_y, job_id, job_name,"
				" continue_flag, run_user, run_user_password)"
				" values (" ZBX_FS_UI64 "," ZBX_FS_UI64 "," ZBX_FS_UI64 "," ZBX_FS_UI64 ","
				" %d, %d, %d, %s, %s, 0, %d,"
				" %d, 0, 0, 0, %s, %s, '%s', '%s', %s, '%s', '%s')",
				inner_job_id, inner_jobnet_id, inner_jobnet_main_id, inner_job_id_fs_link,
				JA_JOB_INVO_FLAG_OFF, job_type, test_flag, row[7], row[8], JA_JOB_STATUS_BEGIN,
				boot_count, row[4], row[5], row[1], job_name, row[9], run_user, run_user_password);

		zbx_free(job_name_esc);
		zbx_free(run_user_esc);
		zbx_free(run_user_password_esc);

		if (rc <= ZBX_DB_OK)
		{
			zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s", row[0], row[1], row[2]);
			ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_job_table", msgwork);
			DBfree_result(result);
			return FAIL;
		}

		rc = SUCCEED;
		switch (job_type)
		{
			case JA_JOB_TYPE_START:
				if (run_type == JA_JOBNET_RUN_TYPE_WAIT)
				{
					rc = DBexecute("update ja_run_job_table set method_flag = %d where inner_job_id = " ZBX_FS_UI64,
							JA_JOB_METHOD_WAIT, inner_job_id);

					if (rc <= ZBX_DB_OK)
					{
						zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s " ZBX_FS_UI64, row[0], row[1], row[2],
							inner_job_id);
						ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_job_table", msgwork);
						DBfree_result(result);
						return FAIL;
					}
					rc = SUCCEED;
				}
				break;

			case JA_JOB_TYPE_END:
				rc = load_icon_end(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_IF:
				rc = load_icon_if(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_VALUE:
				rc = load_icon_value(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_JOB:
				rc = load_icon_job(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_JOBNET:
				rc = load_icon_jobnet(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_EXTJOB:
				rc = load_icon_extjob(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_CALC:
				rc = load_icon_calc(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_TASK:
				rc = load_icon_task(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_INFO:
				rc = load_icon_info(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_FCOPY:
				rc = load_icon_fcopy(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_FWAIT:
				rc = load_icon_fwait(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_REBOOT:
				rc = load_icon_reboot(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_REL:
				rc = load_icon_rel(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_LESS:
				rc = load_icon_less(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;

			case JA_JOB_TYPE_LINK:
				rc = load_icon_link(inner_jobnet_id, inner_job_id, row[0], row[1], row[2]);
				break;
		}

		if (rc != SUCCEED)
		{
			DBfree_result(result);
			return FAIL;
		}
	}

	DBfree_result(result);


	/* expand the information flow */
	result = DBselect("select start_job_id, end_job_id, flow_type, flow_width"
			" from ja_flow_control_table"
			" where jobnet_id = '%s' and update_date = %s",
			jobnet_id, update_date);

	while (NULL != (row = DBfetch(result)))
	{
		inner_flow_id = get_next_id(JA_RUN_ID_FLOW, 0, jobnet_id, 0);
		if (inner_flow_id == 0)
		{
			DBfree_result(result);
			return FAIL;
		}

		start_inner_job_id = get_flow_inner_job_id(inner_jobnet_id, row[0]);
		if (start_inner_job_id == FAIL)
		{
			DBfree_result(result);
			return FAIL;
		}

		end_inner_job_id = get_flow_inner_job_id(inner_jobnet_id, row[1]);
		if (end_inner_job_id == FAIL)
		{
			DBfree_result(result);
			return FAIL;
		}

		rc = DBexecute("insert into ja_run_flow_table ("
				" inner_flow_id, inner_jobnet_id, start_inner_job_id, end_inner_job_id,"
				" flow_type, flow_width)"
				" values (" ZBX_FS_UI64 "," ZBX_FS_UI64 "," ZBX_FS_UI64 "," ZBX_FS_UI64 ","
				" %s, %s)",
				inner_flow_id, inner_jobnet_id, start_inner_job_id, end_inner_job_id,
				row[2], row[3]);

		if (rc <= ZBX_DB_OK)
		{
			zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %s %s", jobnet_id, update_date, row[0], row[1]);
			ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_flow_table", msgwork);
			DBfree_result(result);
			return FAIL;
		}
	}

	DBfree_result(result);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_sub_jobnet_definition                                       *
 *                                                                            *
 * Purpose: expand the sub jobnet definition                                  *
 *                                                                            *
 * Parameters: inner_jobnet_main_id (in) - inner jobnet main id               *
 *             run_type (in) - run type                                       *
 *             jobnet_id (in) - jobnet id                                     *
 *             schedule_id (in) - schedule id                                 *
 *             execution_user_name (in) - execution user name                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_sub_jobnet_definition(zbx_uint64_t inner_jobnet_main_id,
				int run_type, char *jobnet_id, char *schedule_id,
				char *execution_user_name)
{
	DB_RESULT	result;
	DB_RESULT	result2;
	DB_RESULT	result3;
	DB_ROW		row;
	DB_ROW		row2;
	DB_ROW		row3;
	int		hit, test_flag, rc;
	zbx_uint64_t	link_inner_jobnet_id;
	char		*memo, *memo_esc = NULL, *jobnet_name_esc = NULL, *user_name_esc = NULL;
	char		*execution_user_name_esc = NULL;
	const char	*__function_name = "load_sub_jobnet_definition";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(" ZBX_FS_UI64 " %d)",
		__function_name, inner_jobnet_main_id, run_type);

	hit = 1;
	while (hit == 1)
	{
		hit = 0;
		result = DBselect("select inner_job_id from ja_run_job_table"
				" where inner_jobnet_main_id = " ZBX_FS_UI64 " and job_type = %d and invo_flag = %d",
				inner_jobnet_main_id, JA_JOB_TYPE_JOBNET, JA_JOB_INVO_FLAG_OFF);

		while (NULL != (row = DBfetch(result)))
		{
			hit = 1;
			result2 = DBselect("select link_inner_jobnet_id, link_jobnet_id"
					" from ja_run_icon_jobnet_table"
					" where inner_job_id = %s", row[0]);

			if (NULL == (row2 = DBfetch(result2)))
			{
				ja_log("JALOADER200001", inner_jobnet_main_id, NULL, 0, "ja_run_icon_jobnet_table", row[0]);
				DBfree_result(result);
				DBfree_result(result2);
				return FAIL;
			}

			ZBX_STR2UINT64(link_inner_jobnet_id, row2[0]);

			/* expand the sub jobnet */
			result3 = DBselect("select jobnet_id, update_date, public_flag, user_name, jobnet_name, memo,"
					" multiple_start_up"
					" from ja_jobnet_control_table"
					" where jobnet_id = '%s' and valid_flag = %d",
					row2[1], VALID_FLAG_ON);

			if (NULL == (row3 = DBfetch(result3)))
			{
				no_valid_flag = 1;
				ja_log("JALOADER200005", inner_jobnet_main_id, NULL, 0, row2[1], jobnet_id, schedule_id);
				DBfree_result(result);
				DBfree_result(result2);
				DBfree_result(result3);
				return FAIL;
			}

			if (SUCCEED != DBis_null(row3[5]))
			{
				memo_esc = DBdyn_escape_string(row3[5]);
				memo     = memo_esc;
			}
			else
			{
				memo = "";
			}

			user_name_esc           = DBdyn_escape_string(row3[3]);
			jobnet_name_esc         = DBdyn_escape_string(row3[4]);
			execution_user_name_esc = DBdyn_escape_string(execution_user_name);

			rc = DBexecute("insert into ja_run_jobnet_table ("
					" inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date, run_type, main_flag,"
					" timeout_flag, status, scheduled_time, start_time, end_time, public_flag, multiple_start_up,"
					" jobnet_id, user_name, jobnet_name, memo, execution_user_name,"
					" virtual_time, virtual_start_time, virtual_end_time, initial_scheduled_time)"
					" values (" ZBX_FS_UI64 ", " ZBX_FS_UI64 ", %s, %s, %d, %d,"
					" 0, %d, 0, 0, 0, %s, %s,"
					" '%s', '%s', '%s', '%s', '%s',"
					" 0, 0, 0, 0)",
					link_inner_jobnet_id, inner_jobnet_main_id, row[0], row3[1], run_type, JA_JOBNET_MAIN_FLAG_SUB,
					JA_JOBNET_STATUS_BEGIN, row3[2], row3[6],
					row3[0], user_name_esc, jobnet_name_esc, memo, execution_user_name_esc);

			zbx_free(memo_esc);
			zbx_free(user_name_esc);
			zbx_free(jobnet_name_esc);
			zbx_free(execution_user_name_esc);

			if (rc <= ZBX_DB_OK)
			{
				zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row3[0], row3[1]);
				ja_log("JALOADER200002", inner_jobnet_main_id, NULL, 0, "ja_run_jobnet_table", msgwork);
				DBfree_result(result);
				DBfree_result(result2);
				DBfree_result(result3);
				return FAIL;
			}

			if (run_type == JA_JOBNET_RUN_TYPE_TEST)
			{
				test_flag = JA_JOB_TEST_FLAG_ON;
			}
			else
			{
				test_flag = JA_JOB_TEST_FLAG_OFF;
			}

			/* expand the sub job */
			rc = load_job_definition(inner_jobnet_main_id, link_inner_jobnet_id, row3[0], row3[1],
					test_flag, JA_JOBNET_RUN_TYPE_NORMAL);
			if (rc != SUCCEED)
			{
				DBfree_result(result);
				DBfree_result(result2);
				DBfree_result(result3);
				return FAIL;
			}

			rc = DBexecute("update ja_run_job_table set invo_flag = %d where inner_job_id = %s", JA_JOB_INVO_FLAG_ON, row[0]);
			if (rc <= ZBX_DB_OK)
			{
				ja_log("JALOADER200003", inner_jobnet_main_id, NULL, 0, "ja_run_job_table", row[0]);
				DBfree_result(result);
				DBfree_result(result2);
				DBfree_result(result3);
				return FAIL;
			}
			DBfree_result(result2);
			DBfree_result(result3);
		}
		DBfree_result(result);
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: load_jobnet_definition                                           *
 *                                                                            *
 * Purpose: expand the jobnet definition                                      *
 *                                                                            *
 * Parameters: jobnet_id (in) - jobnet id                                     *
 *             scheduled_time (in) - scheduled time                           *
 *             schedule_id (in) - schedule id                                 *
 *             calendar_id (in) - calendar id                                 *
 *             boot_time (in) - boot time (HHMM 0000-9959)                    *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	load_jobnet_definition(char *jobnet_id, char *scheduled_time,
				char *schedule_id, char *calendar_id, char *boot_time)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		count = 0, rc;
	zbx_uint64_t	inner_jobnet_main_id;
	int jobnet_timeout = 0,timeout_run_type = 0;
	char		*memo, *memo_esc = NULL, *jobnet_name_esc = NULL, *user_name_esc = NULL;
	const char	*__function_name = "load_jobnet_definition";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s %s %s %s)",
		__function_name, jobnet_id, scheduled_time, schedule_id, calendar_id, boot_time);

	/* double check the expansion of jobnet */
	result = DBselect("select count(*)"
			" from ja_run_jobnet_table"
			" where jobnet_id = '%s' and initial_scheduled_time = %s and run_type = %d and main_flag = %d"
			" and schedule_id = '%s' and calendar_id = '%s'",
			jobnet_id, scheduled_time, JA_JOBNET_RUN_TYPE_NORMAL, JA_JOBNET_MAIN_FLAG_MAIN,
			schedule_id, calendar_id);

	if (NULL == (row = DBfetch(result)))
	{
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s %d %d",
			jobnet_id, scheduled_time, JA_JOBNET_RUN_TYPE_NORMAL, JA_JOBNET_MAIN_FLAG_MAIN);
		ja_log("JALOADER200001", 0, jobnet_id, 0, "ja_run_jobnet_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	count = atoi(row[0]);
	DBfree_result(result);

	/* already registered ? */
	if (count > 0)
	{
		/* skip registration */
		/* zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= jobnet already registered: jobnet[%s] scheduled time[%s]", jobnet_id, scheduled_time); */
		return SUCCEED;
	}

	DBbegin();

	inner_jobnet_main_id = get_next_id(JA_RUN_ID_JOBNET_LD, 0, jobnet_id, 0);
	if (inner_jobnet_main_id == 0)
	{
		DBrollback();
		return FAIL;
	}

	/* expand the main jobnet */
	result = DBselect("select jobnet_id, update_date, public_flag, user_name, jobnet_name, memo,"
			" multiple_start_up, jobnet_timeout, timeout_run_type"
			" from ja_jobnet_control_table"
			" where jobnet_id = '%s' and valid_flag = %d",
			jobnet_id, VALID_FLAG_ON);

	if (NULL == (row = DBfetch(result)))
	{
		no_valid_flag = 1;
		DBrollback();
		ja_log("JALOADER200004", 0, NULL, 0, jobnet_id, schedule_id);
		DBfree_result(result);
		return FAIL;
	}

	if (SUCCEED != DBis_null(row[5]))
	{
		memo_esc = DBdyn_escape_string(row[5]);
		memo     = memo_esc;
	}
	else
	{
		memo = "";
	}

	user_name_esc   = DBdyn_escape_string(row[3]);
	jobnet_name_esc = DBdyn_escape_string(row[4]);

	if (SUCCEED != DBis_null(row[7]))
	{
		jobnet_timeout = atoi(row[7]);
	}
	if (SUCCEED != DBis_null(row[8]))
	{
		timeout_run_type = atoi(row[8]);
	}


	rc = DBexecute("insert into ja_run_jobnet_table ("
			" inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date,"
			" run_type, main_flag, timeout_flag, status,"
			" scheduled_time, start_time, end_time, public_flag, multiple_start_up,"
			" jobnet_id, user_name, jobnet_name, memo, schedule_id, calendar_id, boot_time, execution_user_name,"
			" virtual_time, virtual_start_time, virtual_end_time, initial_scheduled_time)"
			" values (" ZBX_FS_UI64 ", " ZBX_FS_UI64 ", 0, %s,"
			" %d, %d, 0, %d,"
			" %s, 0, 0, %s, %s,"
			" '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%s',"
			" 0, 0, 0, %s)",
			inner_jobnet_main_id, inner_jobnet_main_id, row[1],
			JA_JOBNET_RUN_TYPE_NORMAL, JA_JOBNET_MAIN_FLAG_MAIN, JA_JOBNET_STATUS_BEGIN,
			scheduled_time, row[2], row[6],
			row[0], user_name_esc, jobnet_name_esc, memo, schedule_id, calendar_id, boot_time, user_name_esc,
			scheduled_time);

	if (rc <= ZBX_DB_OK)
	{
		DBrollback();
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row[0], row[1]);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_jobnet_table", msgwork);
		DBfree_result(result);
		zbx_free(memo_esc);
		zbx_free(user_name_esc);
		zbx_free(jobnet_name_esc);
		return FAIL;
	}

	rc = DBexecute("insert into ja_run_jobnet_summary_table ("
			" inner_jobnet_id, update_date, invo_flag, run_type, status, job_status, jobnet_abort_flag, load_status,"
			" scheduled_time, start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo,"
			" schedule_id, calendar_id, boot_time, execution_user_name,"
			" virtual_time, virtual_start_time, virtual_end_time, start_pending_flag, initial_scheduled_time, jobnet_timeout, timeout_run_type)"
			" values (" ZBX_FS_UI64 ", %s, 1, %d, %d, 0, 0, %d,"
			" %s, 0, 0, %s, %s, '%s', '%s', '%s', '%s',"
			" '%s', '%s', '%s', '%s',"
			" 0, 0, 0, %d, %s, %d, %d)",
			inner_jobnet_main_id, row[1], JA_JOBNET_RUN_TYPE_NORMAL, JA_JOBNET_STATUS_BEGIN, JA_SUMMARY_LOAD_STATUS_NORMAL,
			scheduled_time, row[2], row[6], row[0], user_name_esc, jobnet_name_esc, memo,
			schedule_id, calendar_id, boot_time, user_name_esc,
			JA_SUMMARY_START_PENDING_NONE, scheduled_time, jobnet_timeout, timeout_run_type);

	zbx_free(memo_esc);
	zbx_free(user_name_esc);
	zbx_free(jobnet_name_esc);

	if (rc <= ZBX_DB_OK)
	{
		DBrollback();
		zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row[0], row[1]);
		ja_log("JALOADER200002", 0, jobnet_id, 0, "ja_run_jobnet_summary_table", msgwork);
		DBfree_result(result);
		return FAIL;
	}

	/* expand the main job */
	rc = load_job_definition(inner_jobnet_main_id, inner_jobnet_main_id, row[0], row[1],
			JA_JOB_TEST_FLAG_OFF, JA_JOBNET_RUN_TYPE_NORMAL);
	if (rc != SUCCEED)
	{
		DBfree_result(result);
		DBrollback();
		return FAIL;
	}

	/* expand the sub jobnet */
	rc = load_sub_jobnet_definition(inner_jobnet_main_id, JA_JOBNET_RUN_TYPE_NORMAL, row[0], schedule_id, row[3]);
	if (rc != SUCCEED)
	{
		DBfree_result(result);
		DBrollback();
		return FAIL;
	}

	DBfree_result(result);
	DBcommit();
	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_schedule_jobnet                                              *
 *                                                                            *
 * Purpose: to get the jobnet for execution                                   *
 *                                                                            *
 * Parameters: schedule_id (in) - schedule id                                 *
 *             update_date (in) - update date                                 *
 *             scheduled_time (in) - scheduled time                           *
 *             calendar_id (in) - calendar id                                 *
 *             boot_time (in) - boot time (HHMM 0000-9959)                    *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_schedule_jobnet(char *schedule_id, char *update_date,
				char *scheduled_time, char *calendar_id, char *boot_time)
{
	DB_RESULT	result;
	DB_ROW		row;
	const char	*__function_name = "get_schedule_jobnet";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s %s %s %s)",
		__function_name, schedule_id, update_date, scheduled_time, calendar_id, boot_time);

	result = DBselect("select jobnet_id"
			" from ja_schedule_jobnet_table"
			" where schedule_id = '%s' and update_date = %s",
			schedule_id, update_date);

	while (NULL != (row = DBfetch(result)))
	{
		load_jobnet_definition(row[0], scheduled_time, schedule_id, calendar_id, boot_time);
	}
	DBfree_result(result);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_calendar_detail                                              *
 *                                                                            *
 * Purpose: check the working days of jobnet                                  *
 *                                                                            *
 * Parameters: schedule_id (in) - schedule id                                 *
 *             update_date (in) - update date                                 *
 *             start_date (in) - search start time                            *
 *             end_date (in) - search end time (current time)                 *
 *             calendar_id (in) - calendar id                                 *
 *             boot_time (in) - boot time (HHMM 0000-9959)                    *
 *             object_flag (in) - object flag                                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_calendar_detail(char *schedule_id, char *update_date,
				char *start_date, char *end_date, char *calendar_id, char *boot_time, int object_flag)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		hit, cnt, months;
	int		base_date_flag, designated_day, w_designated_day, shift_day, w_shift_day;
	char		jc_update_date[UPDATE_DATE_LEN];
	char		start_ope_date[OPERATING_DATE_LEN], end_ope_date[OPERATING_DATE_LEN];
	char		base_date[OPERATING_DATE_LEN];
	char		edit_date[OPERATING_DATE_LEN] , edit_month[OPERATING_DATE_LEN];
	char		operating_date[OPERATING_DATE_LEN];
	char		scheduled_time[SCHEDULED_TIME_LEN];
	char		base_calendar_id[JA_CALENDAR_ID_LEN];
	const char	*__function_name = "get_calendar_detail";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s %s %s %s %s)",
		__function_name, schedule_id, update_date, start_date, end_date, calendar_id, boot_time);

	/* check the type of object */
	if (object_flag == 0) {
		/* calendar object */
		result = DBselect("select update_date"
				" from ja_calendar_control_table"
				" where calendar_id = '%s' and valid_flag = %d",
				calendar_id, VALID_FLAG_ON);

		if (NULL == (row = DBfetch(result)))
		{
			no_valid_flag = 1;
			ja_log("JALOADER200006", 0, NULL, 0, calendar_id, schedule_id);
			DBfree_result(result);
			return FAIL;
		}

		zbx_strlcpy(jc_update_date, row[0], sizeof(jc_update_date));
		DBfree_result(result);

		/* edit the start and end dates of the search range (YYYYMMDD <- YYYYMMDDHHMM) */
		zbx_strlcpy(start_ope_date, start_date, sizeof(start_ope_date));
		make_end_operating_date(end_ope_date, end_date, boot_time);

		result = DBselect("select operating_date"
				" from ja_calendar_detail_table"
				" where calendar_id = '%s' and update_date = %s"
				" and operating_date <= %s and operating_date >= %s",
				calendar_id, jc_update_date, start_ope_date, end_ope_date);

		while (NULL != (row = DBfetch(result)))
		{
			/* schedule time edit and check */
			make_scheduled_time(scheduled_time, row[0], boot_time);
			zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= scheduled_time[%s] <= start_date[%s] && scheduled_time[%s] >= end_date[%s]",
				scheduled_time, start_date, scheduled_time, end_date);
			if (strcmp(scheduled_time, start_date) <= 0 && strcmp(scheduled_time, end_date) >= 0)
			{
				get_schedule_jobnet(schedule_id, update_date, scheduled_time, calendar_id, boot_time);
			}
		}
		DBfree_result(result);
	}
	else {
		/* filter object */
		result = DBselect("select base_date_flag, designated_day, shift_day, base_calendar_id"
				" from ja_filter_control_table"
				" where filter_id = '%s' and valid_flag = %d",
				calendar_id, VALID_FLAG_ON);

		if (NULL == (row = DBfetch(result)))
		{
			no_valid_flag = 1;
			ja_log("JALOADER200008", 0, NULL, 0, calendar_id, schedule_id);
			DBfree_result(result);
			return FAIL;
		}

		base_date_flag   = atoi(row[0]);
		designated_day   = atoi(row[1]);
		shift_day        = atoi(row[2]);
		w_designated_day = designated_day;
		w_shift_day      = shift_day;
		zbx_strlcpy(base_calendar_id, row[3], sizeof(base_calendar_id));
		DBfree_result(result);

		if (w_shift_day < 0)
		{
			w_shift_day = w_shift_day * -1;
		}

		/* beginning of the month ? */
		if (base_date_flag == 0)
		{
			w_designated_day = 1;
		}

		result = DBselect("select update_date"
				" from ja_calendar_control_table"
				" where calendar_id = '%s' and valid_flag = %d",
				base_calendar_id, VALID_FLAG_ON);

		if (NULL == (row = DBfetch(result)))
		{
			no_valid_flag = 1;
			ja_log("JALOADER200006", 0, NULL, 0, calendar_id, schedule_id);
			DBfree_result(result);
			return FAIL;
		}

		zbx_strlcpy(jc_update_date, row[0], sizeof(jc_update_date));
		DBfree_result(result);

		/* operating day shift check */
		memcpy(base_date, end_date, 6);  /* YYYYMMDDHHMM -> YYYYMM */
		base_date[6] = '\0';

		months = -2;
		while (months <= 1)
		{
			/* check out one month before and after */
			hit = 0;
			cnt = 0;
			operating_date[0] = '\0';

			zbx_strlcpy(edit_month, base_date, sizeof(edit_month));
			shift_month(edit_month, months);

			switch (base_date_flag)
			{
				case 1: /* end of month */
					w_designated_day = get_end_of_month(edit_month);
					break;

				case 2: /* specified day */
					w_designated_day = get_end_of_month(edit_month);
					if (designated_day > w_designated_day)
					{
						if (shift_day >= 0)
						{
							/* search from the first day of the next month */
							shift_month(edit_month, 1);
							w_designated_day = 1;
							cnt = 1;
						}
						else
						{
							cnt = designated_day - w_designated_day;
						}
					}
					else
					{
						w_designated_day = designated_day;
					}
					break;
			}
			zbx_snprintf(edit_date, sizeof(edit_date), "%s%02d", edit_month, w_designated_day);

			while (cnt < 30)
			{
				/* check before and after 30 days from the month */
				zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= cnt[%d] designated_day[%d] shift_day[%d] edit_date[%s]",
					cnt, w_designated_day, shift_day, edit_date);

				result = DBselect("select operating_date from ja_calendar_detail_table"
						" where calendar_id = '%s' and update_date = %s and operating_date = %s",
						base_calendar_id, jc_update_date, edit_date);

				if (NULL != (row = DBfetch(result)))
				{
					hit = hit + 1;

					/* designated dates are operating days ? */
					if (cnt == 0 || hit >= w_shift_day)
					{
						zbx_strlcpy(operating_date, row[0], sizeof(operating_date));
						DBfree_result(result);
						break;
					}
				}

				/* no moving date ? */
				if (shift_day == 0)
				{
					break;
				}

				/* next day */
				if (shift_day >= 0)
				{
					shift_days(edit_date, 1);
				}
				else
				{
					shift_days(edit_date, -1);
				}
				cnt = cnt + 1;
				DBfree_result(result);
			}

			/* operating day get ? */
			if (operating_date[0] != '\0')
			{
				/* schedule time edit and check */
				make_scheduled_time(scheduled_time, operating_date, boot_time);
				zabbix_log(LOG_LEVEL_DEBUG, "scheduled_time[%s] <= start_date[%s] && scheduled_time[%s] >= end_date[%s]",
					scheduled_time, start_date, scheduled_time, end_date);
				if (strcmp(scheduled_time, start_date) <= 0 && strcmp(scheduled_time, end_date) >= 0)
				{
					zabbix_log(LOG_LEVEL_DEBUG, "jobnet schedule registration start. schedule_id[%s] update_date[%s] scheduled_time[%s] calendar_id[%s] boot_time[%s]",
						schedule_id, update_date, scheduled_time, calendar_id, boot_time);
					get_schedule_jobnet(schedule_id, update_date, scheduled_time, calendar_id, boot_time);
				}
			}
			months = months + 1;
		}
	}

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_schedule_detail                                              *
 *                                                                            *
 * Purpose: to get more information jobnet                                    *
 *                                                                            *
 * Parameters: schedule_id (in) - schedule id                                 *
 *             update_date (in) - update date                                 *
 *             start_date (in) - search start time                            *
 *             end_date (in) - search end time (current time)                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	get_schedule_detail(char *schedule_id, char *update_date, char *start_date, char *end_date)
{
	DB_RESULT	result;
	DB_ROW		row;
	const char	*__function_name = "get_schedule_detail";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s %s %s)",
		__function_name, schedule_id, update_date, start_date, end_date);

	result = DBselect("select calendar_id, boot_time, object_flag"
			" from ja_schedule_detail_table"
			" where schedule_id = '%s' and update_date = %s",
			schedule_id, update_date);

	while (NULL != (row = DBfetch(result)))
	{
		get_calendar_detail(schedule_id, update_date, start_date, end_date, row[0], row[1], atoi(row[2]));
	}
	DBfree_result(result);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jobnet_load_schedule                                             *
 *                                                                            *
 * Purpose: expand the jobnet (scheduled start)                               *
 *                                                                            *
 * Parameters: load_span (in) - deployment time of a jobnet                   *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	jobnet_load_schedule(int load_span)
{
	DB_RESULT	result;
	DB_ROW		row;
	int		rc;
	char		*start_date, *end_date;
	const char	*__function_name = "jobnet_load_schedule";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(%d)", __function_name, load_span);

	get_search_date(load_span, &start_date, &end_date);

	result = DBselect("select schedule_id, update_date from ja_schedule_control_table where valid_flag = %d", VALID_FLAG_ON);

	while (NULL != (row = DBfetch(result)))
	{
		no_valid_flag = 0;

		get_schedule_detail(row[0], row[1], start_date, end_date);

		if (no_valid_flag != 0)
		{
			DBbegin();
			rc = DBexecute("update ja_schedule_control_table set valid_flag = %d"
					" where schedule_id = '%s' and update_date = %s",
					 VALID_FLAG_OFF, row[0], row[1]);

			if (rc <= ZBX_DB_OK)
			{
				DBrollback();
				zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row[0], row[1]);
				ja_log("JALOADER200003", 0, NULL, 0, "ja_schedule_control_table", msgwork);
				continue;
			}
			DBcommit();
		}
	}
	DBfree_result(result);
	zbx_free(start_date);
	zbx_free(end_date);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jobnet_load_immediate                                            *
 *                                                                            *
 * Purpose: expand the immediate start jobnet                                 *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int	jobnet_load_immediate()
{
	DB_RESULT	result;
	DB_ROW		row;
	struct tm	*tm;
	time_t		now;
	int		run_type, test_flag, rc;
	zbx_uint64_t	inner_jobnet_id;
	char		*memo, *memo_esc = NULL, *jobnet_name_esc = NULL, *user_name_esc = NULL;
	char		*execution_user_name_esc = NULL;
	char		now_time[20];
	int jobnet_timeout = 0, timeout_run_type = 0;
	const char	*__function_name = "jobnet_load_immediate";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	result = DBselect("select inner_jobnet_id, inner_job_id, update_date, run_type, scheduled_time,"
			" public_flag, jobnet_id, user_name, jobnet_name, memo, execution_user_name,"
			" multiple_start_up, initial_scheduled_time "
			" from ja_run_jobnet_table"
			" where run_type <> %d and main_flag = %d and status = %d",
			JA_JOBNET_RUN_TYPE_NORMAL, JA_JOBNET_MAIN_FLAG_MAIN, JA_JOBNET_STATUS_BEGIN);

	while (NULL != (row = DBfetch(result)))
	{
		ZBX_STR2UINT64(inner_jobnet_id, row[0]);

		rc = already_start_check_immediate(inner_jobnet_id);
		if (rc != SUCCEED)
		{
			/* skip registration */
			zabbix_log(LOG_LEVEL_DEBUG," immediate jobnet already registered: jobnet[%s] inner_jobnet_id[%s]", row[6], row[0]);
			continue;
		}

		DBbegin();

		no_valid_flag = 0;

		if (SUCCEED != DBis_null(row[9]))
		{
			memo_esc = DBdyn_escape_string(row[9]);
			memo     = memo_esc;
		}
		else
		{
			memo = "";
		}

		user_name_esc           = DBdyn_escape_string(row[7]);
		jobnet_name_esc         = DBdyn_escape_string(row[8]);
		execution_user_name_esc = DBdyn_escape_string(row[10]);
		run_type = atoi(row[3]);
		if (run_type == JA_JOBNET_RUN_TYPE_TEST)
		{
			test_flag = JA_JOB_TEST_FLAG_ON;
		}
		else
		{
			test_flag = JA_JOB_TEST_FLAG_OFF;
		}

		/* expand the main job */
		rc = load_job_definition(inner_jobnet_id, inner_jobnet_id, row[6], row[2], test_flag, run_type);
		if (rc != SUCCEED)
		{
			DBrollback();
			zbx_free(memo_esc);
			zbx_free(user_name_esc);
			zbx_free(jobnet_name_esc);
			zbx_free(execution_user_name_esc);
			continue;
		}

		/* expand the sub jobnet */
		rc = load_sub_jobnet_definition(inner_jobnet_id, run_type, row[6], "none", row[10]);
		if (rc != SUCCEED)
		{
			DBrollback();

			if (no_valid_flag != 0)
			{
				DBbegin();

				time(&now);
				tm = localtime(&now);
				strftime(now_time, sizeof(now_time), "%Y%m%d%H%M%S", tm);

				rc = DBexecute("update ja_run_jobnet_table set status = %d, start_time = %s, end_time = %s"
						" where inner_jobnet_id = %s",
						 JA_JOBNET_STATUS_ENDERR, now_time, now_time, row[0]);

				if (rc <= ZBX_DB_OK)
				{
					DBrollback();
					zbx_snprintf(msgwork, sizeof(msgwork), "%s", row[0]);
					ja_log("JALOADER200003", inner_jobnet_id, NULL, 0, "ja_run_jobnet_table", msgwork);
					zbx_free(memo_esc);
					zbx_free(user_name_esc);
					zbx_free(jobnet_name_esc);
					zbx_free(execution_user_name_esc);
					continue;
				}

				jobnet_timeout = get_jobnet_timeout(row[6],row[2], &timeout_run_type);

				rc = DBexecute("insert into ja_run_jobnet_summary_table ("
						" inner_jobnet_id, update_date, invo_flag, run_type, status,"
						" job_status, jobnet_abort_flag, load_status,"
						" scheduled_time, start_time, end_time, public_flag, multiple_start_up,"
						" jobnet_id, user_name, jobnet_name, memo, execution_user_name,"
						" virtual_time, virtual_start_time, virtual_end_time, start_pending_flag, initial_scheduled_time,jobnet_timeout, timeout_run_type)"
						" values (%s, %s, 1, %s, %d,"
						" 2, 0, %d,"
						" %s, %s, %s, %s, %s,"
						" '%s', '%s', '%s', '%s', '%s',"
						" 0, 0, 0, %d, %s, %d, %d )",
						row[0], row[2], row[3], JA_JOBNET_STATUS_ENDERR,
						JA_SUMMARY_LOAD_STATUS_ERROR,
						row[4], now_time, now_time, row[5], row[11],
						row[6], user_name_esc, jobnet_name_esc, memo, execution_user_name_esc,
						JA_SUMMARY_START_PENDING_NONE, row[12], jobnet_timeout, timeout_run_type);

				if (rc <= ZBX_DB_OK)
				{
					DBrollback();
					zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row[0], row[6]);
					ja_log("JALOADER200002", inner_jobnet_id, NULL, 0, "ja_run_jobnet_summary_table", msgwork);
					zbx_free(memo_esc);
					zbx_free(user_name_esc);
					zbx_free(jobnet_name_esc);
					zbx_free(execution_user_name_esc);
					continue;
				}
				DBcommit();
			}
			zbx_free(memo_esc);
			zbx_free(user_name_esc);
			zbx_free(jobnet_name_esc);
			zbx_free(execution_user_name_esc);
			continue;
		}

		jobnet_timeout = get_jobnet_timeout(row[6],row[2], &timeout_run_type);

		rc = DBexecute("insert into ja_run_jobnet_summary_table ("
				" inner_jobnet_id, update_date, invo_flag, run_type, status, job_status, jobnet_abort_flag, load_status,"
				" scheduled_time, start_time, end_time, public_flag, multiple_start_up,"
				" jobnet_id, user_name, jobnet_name, memo, execution_user_name,"
				" virtual_time, virtual_start_time, virtual_end_time, start_pending_flag, initial_scheduled_time,jobnet_timeout, timeout_run_type)"
				" values (%s, %s, 1, %s, %d, 0, 0, %d,"
				" %s, 0, 0, %s, %s,"
				" '%s', '%s', '%s', '%s', '%s',"
				" 0, 0, 0, %d, %s, %d, %d)",
				row[0], row[2], row[3], JA_JOBNET_STATUS_BEGIN, JA_SUMMARY_LOAD_STATUS_NORMAL,
				row[4], row[5], row[11],
				row[6], user_name_esc, jobnet_name_esc, memo, execution_user_name_esc,
				JA_SUMMARY_START_PENDING_NONE, row[12], jobnet_timeout, timeout_run_type);

		zbx_free(memo_esc);
		zbx_free(user_name_esc);
		zbx_free(jobnet_name_esc);
		zbx_free(execution_user_name_esc);

		if (rc <= ZBX_DB_OK)
		{
			DBrollback();
			zbx_snprintf(msgwork, sizeof(msgwork), "%s %s", row[0], row[6]);
			ja_log("JALOADER200002", inner_jobnet_id, NULL, 0, "ja_run_jobnet_summary_table", msgwork);
			continue;
		}

		DBcommit();
	}
	DBfree_result(result);

	return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_jobnet_timeout                                               *
 *                                                                            *
 * Purpose: get the jobnet timeout                                            *
 *                                                                            *
 * Parameters:  jobnet_id, update_date                                        *
 *                                                                            *
 * Return value:  jobnet timeout                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/

int get_jobnet_timeout(char *jobnet_id,char *update_date, int *timeout_run_type){

	DB_RESULT	result;
	DB_ROW		row;
	int jobnet_timeout = 0;
	const char *__function_name = "get_jobnet_timeout";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	result = DBselect("select jobnet_timeout,timeout_run_type from ja_jobnet_control_table "
							" where jobnet_id='%s' and update_date='%s'",jobnet_id,update_date);
	row = DBfetch(result);
    if (row == NULL) {
		zabbix_log(LOG_LEVEL_ERR, "In %s(), cannot select jobnet_timeout from ja_jobnet_control_table. jobnet_id :%s update_date :%s", __function_name,jobnet_id,update_date);
		return 0;
	}
	if (SUCCEED != DBis_null(row[0]))
	{
		jobnet_timeout = atoi(row[0]);
	}
	if (SUCCEED != DBis_null(row[0]))
	{
		*timeout_run_type = atoi(row[1]);
	}
	DBfree_result(result);
	return jobnet_timeout;
}

/******************************************************************************
 *                                                                            *
 * Function: main_jaloader_loop (main process)                                *
 *                                                                            *
 * Purpose: expand the jobnet that is registered in the schedule              *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
void	main_jaloader_loop()
{
	int	load_span;
	int	sleep_cnt_1;
	int	sleep_cnt_2;
	double	sec;

	zabbix_log(LOG_LEVEL_DEBUG, "In main_jaloader_loop() process_type:'%s' process_num:%d",
			ja_get_process_type_string(process_type), process_num);

	zbx_setproctitle("%s [connecting to the database]", ja_get_process_type_string(process_type)); 
	ja_alarm_watcher("ja_loader");

	DBconnect(ZBX_DB_CONNECT_NORMAL);
	load_span = get_load_span();

	sleep_cnt_1 = 9999999;
	sleep_cnt_2 = 9999999;

	for (;;)
	{
		zbx_setproctitle("jobnet loader [performing loader]");
		ja_alarm_timeout(CONFIG_LOADER_TIMEOUT);
		/* scheduled start */
		if (sleep_cnt_1 >= CONFIG_JALOADER_INTERVAL)
		{
			sec = zbx_time();
			jobnet_load_schedule(load_span);
			sec = zbx_time() - sec;

			zabbix_log(LOG_LEVEL_DEBUG, "%s #%d (scheduled start) spent " ZBX_FS_DBL " seconds while processing rules",
					ja_get_process_type_string(process_type), process_num, sec);
			sleep_cnt_1 = 0;
		}

		/* immediate start */
		if (sleep_cnt_2 >= CONFIG_JALAUNCH_INTERVAL)
		{
			sec = zbx_time();
			jobnet_load_immediate();
			sec = zbx_time() - sec;

			zabbix_log(LOG_LEVEL_DEBUG, "%s #%d (immediate start) spent " ZBX_FS_DBL " seconds while processing rules",
					ja_get_process_type_string(process_type), process_num, sec);
			sleep_cnt_2 = 0;
		}

		sleep_cnt_1 = sleep_cnt_1 + 1;
		sleep_cnt_2 = sleep_cnt_2 + 1;
		ja_alarm_timeout(0);

		ja_sleep_loop(1);
	}
}
