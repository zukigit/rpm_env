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
#include "db.h"
#include "log.h"

#include "jacommon.h"

#define JOBNET_LOAD_SPAN "JOBNET_LOAD_SPAN"
#define DEFAULT_LOAD_SPAN 60
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
int ja_schedule_load_span()
{
    DB_RESULT result;
    DB_ROW row;
    int span = DEFAULT_LOAD_SPAN;
    const char *__function_name = "ja_schedule_load_span";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    result =
        DBselect
        ("select value from ja_parameter_table where parameter_name = '%s'",
         JOBNET_LOAD_SPAN);

    if (NULL != (row = DBfetch(result))) {
        span = atoi(row[0]);
        if (span < 1)
            span = 1;
        if (span > 1439)
            span = 1439;
    }
    DBfree_result(result);

    return span;
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
int ja_schedule_get_calendar_id(const char *schedule_id, char *calendar_id)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *update_date;
    const char *__function_name = "ja_schedule_get_calendar_id";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() schedule_id: %s", __function_name,
               schedule_id);

    ret = FAIL;
    result = NULL;
    update_date = NULL;

    result =
        DBselect
        ("select update_date from ja_schedule_control_table where schedule_id = '%s' and valid_flag = 1",
         schedule_id);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    update_date = zbx_strdup(update_date, row[0]);
    DBfree_result(result);

    result =
        DBselect
        ("select calendar_id from ja_schedule_detail_table where schedule_id = '%s' and update_date = '%s'",
         schedule_id, update_date);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    zbx_snprintf(calendar_id, strlen(row[0]) + 1, "%s", row[0]);

    ret = SUCCEED;
  error:
    DBfree_result(result);

    if (update_date != NULL)
        zbx_free(update_date);
    return ret;
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
int ja_schedule_check_time(const char *schedule_id,
                           const char *update_date,
                           const char *operating_date,
                           const char *boot_time)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *cal_update_date, *calendar_id;
    const char *__function_name = "ja_schedule_check_time";

    if (schedule_id == NULL || update_date == NULL
        || operating_date == NULL || boot_time == NULL)
        return FAIL;

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() schedule_id: %s, update_date: %s, operating_date: %s, boot_time: %s",
               __function_name, schedule_id, update_date, operating_date,
               boot_time);

    ret = FAIL;
    cal_update_date = NULL;
    calendar_id = NULL;

    result =
        DBselect
        ("select valid_flag from ja_schedule_control_table where schedule_id = '%s' and update_date = '%s' and valid_flag = 1",
         schedule_id, update_date);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    DBfree_result(result);

    result =
        DBselect
        ("select calendar_id from ja_schedule_detail_table where schedule_id = '%s' and update_date = '%s' and boot_time = '%s'",
         schedule_id, update_date, boot_time);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    calendar_id = zbx_strdup(calendar_id, row[0]);
    DBfree_result(result);

    result =
        DBselect
        ("select update_date from ja_calendar_control_table where calendar_id = '%s' and valid_flag = 1",
         calendar_id);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    cal_update_date = zbx_strdup(cal_update_date, row[0]);
    DBfree_result(result);

    result =
        DBselect
        ("select calendar_id from ja_calendar_detail_table where calendar_id = '%s' and update_date = '%s' and operating_date = %s",
         calendar_id, cal_update_date, operating_date);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    ret = SUCCEED;

  error:
    DBfree_result(result);
    if (cal_update_date != NULL)
        zbx_free(cal_update_date);
    if (calendar_id != NULL)
        zbx_free(calendar_id);
    return ret;
}
