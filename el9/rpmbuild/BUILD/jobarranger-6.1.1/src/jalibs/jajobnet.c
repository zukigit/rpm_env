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
int ja_jobnet_get_user_name(const char *jobnet_id, char *user_name)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "ja_jobnet_get_user_name";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobnet_id: %s", __function_name,
               jobnet_id);
    ret = FAIL;
    result = NULL;

    result =
        DBselect
        ("select user_name from ja_jobnet_control_table where jobnet_id = '%s' and valid_flag = 1",
         jobnet_id);
    if ((row = DBfetch(result)) == NULL)
        goto error;
    zbx_snprintf(user_name, strlen(row[0]) + 1, "%s", row[0]);

    ret = SUCCEED;
  error:
    DBfree_result(result);
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
zbx_uint64_t ja_jobnet_get_job_id(const zbx_uint64_t inner_jobnet_id,
                                  char *get_job_id)
{
    char *tp;
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_job_id, link_inner_jobnet_id;
    const char *__function_name = "ja_jobnet_get_job_id";

    if (inner_jobnet_id == 0 || get_job_id == NULL)
        return 0;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64 ", get_job_id: %s",
               __function_name, inner_jobnet_id, get_job_id);

    inner_job_id = 0;
    link_inner_jobnet_id = inner_jobnet_id;
    tp = strtok(get_job_id, "/");
    while (tp != NULL) {
        if (inner_job_id > 0) {
            result =
                DBselect
                ("select link_inner_jobnet_id from ja_run_icon_jobnet_table"
                 " where inner_job_id = " ZBX_FS_UI64, inner_job_id);
            if (NULL != (row = DBfetch(result))) {
                ZBX_STR2UINT64(link_inner_jobnet_id, row[0]);
            } else {
                inner_job_id = 0;
                DBfree_result(result);
                break;
            }
            DBfree_result(result);
        }

        result =
            DBselect
            ("select inner_job_id from ja_run_job_table"
             " where inner_jobnet_id = " ZBX_FS_UI64
             " and job_id = '%s'", link_inner_jobnet_id, tp);
        if (NULL != (row = DBfetch(result))) {
            ZBX_STR2UINT64(inner_job_id, row[0]);
        } else {
            inner_job_id = 0;
            DBfree_result(result);
            break;
        }
        DBfree_result(result);
        tp = strtok(NULL, "/");
    }
    return inner_job_id;
}
