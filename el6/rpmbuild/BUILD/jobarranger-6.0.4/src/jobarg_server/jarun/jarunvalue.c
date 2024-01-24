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
#include "jastr.h"
#include "javalue.h"
#include "jalog.h"
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
int jarun_value_before(const zbx_uint64_t inner_job_id)
{
    zbx_uint64_t inner_jobnet_id;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_value_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    result =
        DBselect
        ("select inner_jobnet_id, job_id, job_name from ja_run_job_table where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);
    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        if (ja_set_value_before
            (inner_job_id, inner_jobnet_id, "JOB_ID", row[1]) == FAIL) {
            DBfree_result(result);
            return FAIL;
        }
        if (ja_set_value_before
            (inner_job_id, inner_jobnet_id, "JOB_NAME", row[2]) == FAIL) {
            DBfree_result(result);
            return FAIL;
        }
        if (ja_set_value_before
            (inner_job_id, inner_jobnet_id, "CURRENT_TIME",
             ja_timestamp2str(time(NULL))) == FAIL) {
            DBfree_result(result);
            return FAIL;
        }
    } else {
        ja_log("JARUNVALUE300001", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id);
    }
    DBfree_result(result);

    if (ja_set_value_before(inner_job_id, inner_jobnet_id, "JOB_ID_FULL", ja_get_jobid(inner_job_id)) == FAIL) {
        return FAIL;
    }

    if (ja_remove_value_before(inner_job_id, "JOBARG_MESSAGE") == FAIL)
        return FAIL;

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
int jarun_value_after(const zbx_uint64_t inner_job_id)
{
    const char *__function_name = "jarun_value_after";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (ja_remove_value_after(inner_job_id, "JOB_EXIT_CD") == FAIL)
        return FAIL;
    if (ja_remove_value_after(inner_job_id, "STD_ERR") == FAIL)
        return FAIL;
    if (ja_remove_value_after(inner_job_id, "STD_OUT") == FAIL)
        return FAIL;

    return SUCCEED;
}
