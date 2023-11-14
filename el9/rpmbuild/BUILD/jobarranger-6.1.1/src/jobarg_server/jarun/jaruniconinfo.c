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
#include "jastr.h"
#include "jastatus.h"
#include "javalue.h"
#include "jaflow.h"
#include "jaruniconinfo.h"

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
int jarun_icon_info_get_status(const zbx_uint64_t inner_jobnet_id,
                               char *get_job_id, const zbx_uint64_t inner_job_id)
{
    char *tp;
    DB_RESULT result;
    DB_ROW row;
    int status;
    zbx_uint64_t sub_inner_job_id, sub_inner_jobnet_id;
    const char *__function_name = "jarun_icon_info_get_status";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64 " get_job_id: %s",
               __function_name, inner_jobnet_id, get_job_id);

    status = -1;
    sub_inner_job_id = 0;
    sub_inner_jobnet_id = inner_jobnet_id;
    tp = strtok(get_job_id, "/");
    while (tp != NULL) {
        if (sub_inner_job_id > 0) {
            result =
                DBselect
                ("select link_inner_jobnet_id from ja_run_icon_jobnet_table"
                 " where inner_job_id = " ZBX_FS_UI64, sub_inner_job_id);
            if (NULL != (row = DBfetch(result))) {
                ZBX_STR2UINT64(sub_inner_jobnet_id, row[0]);
            } else {
                ja_log("JARUNICONINFO200001", 0, NULL, inner_job_id,
                       __function_name, sub_inner_job_id);
                status = -1;
                DBfree_result(result);
                break;
            }
            DBfree_result(result);
        }

        result =
            DBselect
            ("select inner_job_id, status from ja_run_job_table"
             " where inner_jobnet_id = " ZBX_FS_UI64
             " and job_id = '%s'", sub_inner_jobnet_id, tp);
        if (NULL != (row = DBfetch(result))) {
            ZBX_STR2UINT64(sub_inner_job_id, row[0]);
            status = atoi(row[1]);
        } else {
            ja_log("JARUNICONINFO200002", 0, NULL, inner_job_id,
                   __function_name, tp, inner_job_id);
            status = -1;
            DBfree_result(result);
            break;
        }
        DBfree_result(result);
        tp = strtok(NULL, "/");
    }
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
int jarun_icon_info_get_calendar(const char *get_calendar_id, const zbx_uint64_t inner_job_id)
{
    int status;
    char today[9];
    DB_RESULT result;
    DB_ROW row;
    char *update_date;
    const char *__function_name = "jarun_icon_info_get_calendar";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() get_calendar_id: %s",
               __function_name, get_calendar_id);
    status = -1;
    update_date = NULL;
    zbx_snprintf(today, sizeof(today), "%s", ja_timestamp2str(time(NULL)));

    result =
        DBselect
        ("select update_date from ja_calendar_control_table where calendar_id = '%s' and valid_flag = 1",
         get_calendar_id);
    if (NULL != (row = DBfetch(result))) {
        update_date = zbx_strdup(NULL, row[0]);
    }
    DBfree_result(result);
    if (update_date == NULL) {
        ja_log("JARUNICONINFO200004", 0, NULL, inner_job_id,
               __function_name, get_calendar_id);
        return -1;
    }
    result =
        DBselect
        ("select operating_date from ja_calendar_detail_table where calendar_id = '%s' and update_date = %s and operating_date = %s",
         get_calendar_id, update_date, today);
    if (NULL != (row = DBfetch(result))) {
        status = 1;
    } else {
        status = 0;
    }
    DBfree_result(result);

    zbx_free(update_date);
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
int jarun_icon_info(const zbx_uint64_t inner_job_id)
{
    DB_RESULT result;
    DB_ROW row;
    int status;
    char str_status[4];
    zbx_uint64_t inner_jobnet_id;
    int info_flag;
    char *get_job_id, *get_calendar_id;
    const char *__function_name = "jarun_icon_info";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    result =
        DBselect
        ("select inner_jobnet_id, info_flag, get_job_id, get_calendar_id"
         " from ja_run_icon_info_table" " where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);

    status = -1;
    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        info_flag = atoi(row[1]);
        get_job_id = row[2];
        get_calendar_id = row[3];
        switch (info_flag) {
        case 0:
            status = jarun_icon_info_get_status(inner_jobnet_id, get_job_id, inner_job_id);
            break;
        case 3:
            status = jarun_icon_info_get_calendar(get_calendar_id, inner_job_id);
            break;
        default:
            break;
        }
    } else {
        ja_log("JARUNICONINFO200003", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }
    DBfree_result(result);
    if (status < 0)
        return ja_set_runerr(inner_job_id, 2);

    zbx_snprintf(str_status, sizeof(str_status), "%d", status);
    ja_set_value_after(inner_job_id, inner_jobnet_id, "LAST_STATUS",
                       str_status);

    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}
