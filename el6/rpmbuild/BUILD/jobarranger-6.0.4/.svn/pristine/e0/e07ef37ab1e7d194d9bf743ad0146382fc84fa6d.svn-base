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
#include "javalue.h"
#include "jastatus.h"

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
int jarun_icon_jobnet(const zbx_uint64_t inner_job_id)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_jobnet_id;
    const char *__function_name = "jarun_icon_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    inner_jobnet_id = 0;
    result =
        DBselect
        ("select link_inner_jobnet_id from ja_run_icon_jobnet_table"
         " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    } else {
        ja_log("JARUNICONJOBNET200001", inner_jobnet_id, NULL,
               inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    DBfree_result(result);

    if (ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_READY, -1, -1)
        == FAIL)
        return ja_set_runerr(inner_job_id, 2);

    if (ja_value_before_jobnet_in(inner_job_id, inner_jobnet_id) == FAIL)
        return ja_set_runerr(inner_job_id, 2);

    if (ja_get_status_jobnet(inner_jobnet_id) != JA_JOB_STATUS_READY)
        return ja_set_runerr(inner_job_id, 2);

    return SUCCEED;
}
