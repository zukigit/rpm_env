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
#include "javalue.h"
#include "jastatus.h"
#include "jastr.h"
#include "jalog.h"

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
int jajobnet_ready(const zbx_uint64_t inner_jobnet_id)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t icon_start_id;
    int count;
    char time_str[13];
    const char *__function_name = "jajobnet_ready";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64, __function_name,
               inner_jobnet_id);

    // set jobnet before value
    result =
        DBselect
        ("select jobnet_id, jobnet_name, user_name, inner_jobnet_id, main_flag, scheduled_time"
         " from ja_run_jobnet_table"
         " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL != (row = DBfetch(result))) {
        ja_set_value_jobnet_before(inner_jobnet_id, "JOBNET_ID", row[0]);
        ja_set_value_jobnet_before(inner_jobnet_id, "JOBNET_NAME", row[1]);
        ja_set_value_jobnet_before(inner_jobnet_id, "USER_NAME", row[2]);
        ja_set_value_jobnet_before(inner_jobnet_id, "MANAGEMENT_ID", row[3]);
    } else {
        ja_log("JAJOBNETREADY200001", inner_jobnet_id, NULL, 0,
               __function_name, inner_jobnet_id);
        DBfree_result(result);
        return ja_set_enderr_jobnet(inner_jobnet_id);
    }

    if (atoi(row[4]) == JA_JOBNET_MAIN_FLAG_MAIN) {
        zbx_snprintf(time_str, sizeof(time_str), "%s", ja_timestamp2str(time(NULL)));
        ja_set_value_jobnet_before(inner_jobnet_id, "JOBNET_BOOT_TIME", time_str);

        if (atoi(row[5]) == 0) {
            ja_set_value_jobnet_before(inner_jobnet_id, "JOBNET_SCHEDULED_TIME", "");
        }
        else {
            ja_set_value_jobnet_before(inner_jobnet_id, "JOBNET_SCHEDULED_TIME", row[5]);
        }
    }

    DBfree_result(result);

    // set jobnet status
    if (ja_set_run_jobnet(inner_jobnet_id) == FAIL)
        return FAIL;

    // search start icon
    count = 0;
    icon_start_id = 0;
    result =
        DBselect
        ("select inner_job_id from ja_run_job_table"
         " where inner_jobnet_id = " ZBX_FS_UI64 " and job_type = %d ",
         inner_jobnet_id, JA_JOB_TYPE_START);
    while (NULL != (row = DBfetch(result))) {
        count++;
        if (count == 1)
            ZBX_STR2UINT64(icon_start_id, row[0]);
        else
            icon_start_id = 0;
    }
    DBfree_result(result);

    if (icon_start_id == 0) {
        ja_log("JAJOBNETREADY200002", inner_jobnet_id, NULL, 0,
               __function_name, count, inner_jobnet_id);
        return ja_set_enderr_jobnet(inner_jobnet_id);
    } else {
        if (ja_value_before_jobnet_out(inner_jobnet_id, icon_start_id) ==
            FAIL)
            return FAIL;
        if (ja_set_status_job
            (icon_start_id, JA_JOB_STATUS_READY, 0, 0) == FAIL)
            return FAIL;
    }
    return SUCCEED;
}
