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
int ja_setenv(const zbx_uint64_t inner_job_id)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *key, *value;
    const char *__function_name = "ja_setenv";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    result =
        DBselect
        ("select value_name, before_value from ja_run_value_before_table"
         " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    ret = SUCCEED;
    while (NULL != (row = DBfetch(result))) {
        key = row[0];
        value = row[1];

        zabbix_log(LOG_LEVEL_DEBUG,
                   "In %s() inner_job_id: " ZBX_FS_UI64
                   ",  %s = %s", __function_name,
                   inner_job_id, key, value);

        if (setenv(key, value, 1) != 0) {
            ja_log("JAENV300001", 0, NULL, inner_job_id, __function_name,
                   inner_job_id, key, value);
            ret = FAIL;
            break;
        }
    }
    DBfree_result(result);
    return ret;
}
