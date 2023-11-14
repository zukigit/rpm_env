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
#include "jajobid.h"


/******************************************************************************
 *                                                                            *
 * Function: ja_get_icon_info                                                 *
 *                                                                            *
 * Purpose: get the execution user name and job id and the main jobnet id     *
 *                                                                            *
 * Parameters: inner_job_id (in)  - inner job id                              *
 *             icon         (out) - icon information                          *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_get_icon_info(zbx_uint64_t inner_job_id, ja_icon_info_t *icon)
{
    DB_RESULT    result;
    DB_ROW       row;
    const char   *__function_name = "ja_get_icon_info";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    icon->main_jobnet_id[0]      = '\0';
    icon->execution_user_name[0] = '\0';
    icon->job_id[0]              = '\0';

    /* main jobnet id and user name get */
    result = DBselect("select a.jobnet_id, a.execution_user_name from ja_run_jobnet_table a, ja_run_job_table b"
                      " where a.inner_jobnet_id = b.inner_jobnet_main_id and b.inner_job_id = " ZBX_FS_UI64,
                      inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        zbx_strlcpy(icon->main_jobnet_id,      row[0], sizeof(icon->main_jobnet_id));
        zbx_strlcpy(icon->execution_user_name, row[1], sizeof(icon->execution_user_name));
    }
    DBfree_result(result);

    /* job id get */

    zbx_strlcpy(icon->job_id, ja_get_jobid(inner_job_id), sizeof(icon->job_id));

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_get_jobid                                                     *
 *                                                                            *
 * Purpose: get the job id of the full path from the internal management      *
 *          job id                                                            *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value:  pointer of the job id                                       *
 *                                                                            *
 * Comments: return the area of the sky if the job id can not be obtained     *
 *                                                                            *
 ******************************************************************************/
char *ja_get_jobid(const zbx_uint64_t inner_job_id)
{
    DB_RESULT    result;
    DB_RESULT    result2;
    DB_ROW       row;
    DB_ROW       row2;
    zbx_uint64_t inner_jobnet_main_id, w_inner_job_id;
    int          idx, len;
    char         main_jobnet_id[JA_JOBNET_ID_LEN];
    static char  full_job_id[JA_MAX_DATA_LEN];
    const char   *__function_name = "ja_get_jobid";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    full_job_id[0] = '\0';

    /* internal management job id value check */
    if (inner_job_id <= 0) {
        return full_job_id;
    }

    /* get the full path job id */
    inner_jobnet_main_id = 0;
    main_jobnet_id[0]    = '\0';

    idx                  = sizeof(full_job_id) - 1;
    full_job_id[idx]     = '\0';
    w_inner_job_id       = inner_job_id;

    /* job id search */
    while (1) {
        /* get the job id */
        result = DBselect("select inner_jobnet_id, inner_jobnet_main_id, job_id from ja_run_job_table where inner_job_id = " ZBX_FS_UI64, w_inner_job_id);

        if (NULL == (row = DBfetch(result))) {
            DBfree_result(result);
            break;
        }

        ZBX_STR2UINT64(inner_jobnet_main_id, row[1]);

        /* job id set */
        len = strlen(row[2]);
        if ((idx - (len + 1)) < 0) {
            DBfree_result(result);
            break;
        } else {
            idx = idx - len;
            memcpy(&full_job_id[idx], row[2], len);
            idx = idx - 1;
            memcpy(&full_job_id[idx], "/", 1);
        }

        /* main jobnet ? */
        if (strcmp(row[0], row[1]) == 0) {
            DBfree_result(result);
            break;
        }

        /* get the inner job id of the jobnet icon */
        result2 = DBselect("select inner_job_id from ja_run_icon_jobnet_table where link_inner_jobnet_id = %s", row[0]);

        if (NULL == (row2 = DBfetch(result2))) {
            DBfree_result(result);
            DBfree_result(result2);
            break;
        }

        ZBX_STR2UINT64(w_inner_job_id, row2[0]);
        DBfree_result(result);
        DBfree_result(result2);
    }

    /* get the main jobnet id */
    if (inner_jobnet_main_id != 0) {
        result = DBselect("select jobnet_id from ja_run_jobnet_table"
                          " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_main_id);

        if (NULL != (row = DBfetch(result))) {
            zbx_strlcpy(main_jobnet_id, row[0], sizeof(main_jobnet_id));
        }
        DBfree_result(result);

        /* main jobnet id set */
        len = strlen(main_jobnet_id);
        if ((idx - len) >= 0) {
            idx = idx - len;
            memcpy(&full_job_id[idx], main_jobnet_id, len);
        }
    }

    return &full_job_id[idx];
}
