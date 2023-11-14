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
#include "jastatus.h"
#include "javalue.h"
#include "jajobnet.h"
#include "jaflow.h"
#include "jaruniconrelease.h"

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
int jarun_icon_release(const zbx_uint64_t inner_job_id,
                       const int method_flag)
{
    int db_ret, hit_flag;
    char *job_id, *jobnet_id, *get_job_id, *release_job_id;
    DB_RESULT result;
    DB_RESULT result2;
    DB_ROW row;
    DB_ROW row2;
    zbx_uint64_t inner_jobnet_id, release_inner_job_id;
    const char *__function_name = "jarun_icon_release";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    /* search release job of internal jobnet */
    release_inner_job_id = 0;
    result =
        DBselect
        ("select inner_jobnet_id, release_job_id from ja_run_icon_release_table where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);
    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        jobnet_id = zbx_strdup(NULL, row[1]);
        release_job_id = zbx_strdup(NULL, row[1]);
        get_job_id = zbx_strdup(NULL, row[1]);
        release_inner_job_id = ja_jobnet_get_job_id(inner_jobnet_id, get_job_id);
        zbx_free(get_job_id);
    } else {
        ja_log("JARUNICONRELEASE200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }
    DBfree_result(result);

    if (release_inner_job_id != 0) {
        /* pending release of the icon */
        DBfree_result(DBselect
                      ("select status from ja_run_job_table where inner_job_id = "
                       ZBX_FS_UI64 " for update", release_inner_job_id));
        db_ret =
            DBexecute
            ("update ja_run_job_table set method_flag = 0 where inner_job_id = "
             ZBX_FS_UI64 " and method_flag = 1", release_inner_job_id);
        if (db_ret <= ZBX_DB_OK) {
            ja_log("JARUNICONRELEASE200002", 0, NULL, inner_job_id, __function_name, release_job_id, release_inner_job_id);
            zbx_free(jobnet_id);
            zbx_free(release_job_id);
            return ja_set_runerr(inner_job_id, 2);
        }
    } else {
        /* search release job of external jobnet */
        job_id = strstr(jobnet_id, "/");
        if (job_id == NULL) {
            ja_log("JARUNICONRELEASE200003", 0, NULL, inner_job_id, __function_name, release_job_id, inner_job_id);
            zbx_free(jobnet_id);
            zbx_free(release_job_id);
            return ja_set_runerr(inner_job_id, 2);
        }

        /* pre-check pending icon */
        *job_id = '\0';
        job_id++;
        hit_flag = 0;
        result =
            DBselect
            ("select inner_jobnet_id from ja_run_jobnet_summary_table"
             " where jobnet_id = '%s' and status = %d" ,
             jobnet_id, JA_JOBNET_STATUS_RUN);
        //while ((row = DBfetch(result)) != NULL) {
        if ((row = DBfetch(result)) != NULL) {
            ZBX_STR2UINT64(inner_jobnet_id, row[0]);
            get_job_id = zbx_strdup(NULL, job_id);
            release_inner_job_id = ja_jobnet_get_job_id(inner_jobnet_id, get_job_id);
            zbx_free(get_job_id);

            if (release_inner_job_id == 0) {
                ja_log("JARUNICONRELEASE200003", 0, NULL, inner_job_id, __function_name, release_job_id, inner_job_id);
                zbx_free(jobnet_id);
                zbx_free(release_job_id);
                DBfree_result(result);
                return ja_set_runerr(inner_job_id, 2);
            }

            result2 =
                DBselect
                ("select inner_job_id from ja_run_job_table"
                 " where inner_job_id = " ZBX_FS_UI64 " and method_flag = 1",
                 release_inner_job_id);
            if ((row2 = DBfetch(result2)) == NULL) {
                ja_log("JARUNICONRELEASE200002", 0, NULL, inner_job_id, __function_name, release_job_id, release_inner_job_id);
                zbx_free(jobnet_id);
                zbx_free(release_job_id);
                DBfree_result(result);
                DBfree_result(result2);
                return ja_set_runerr(inner_job_id, 2);
            }
            hit_flag = 1;
            DBfree_result(result2);
        }
        DBfree_result(result);

        if (hit_flag == 0) {
            ja_log("JARUNICONRELEASE200003", 0, NULL, inner_job_id, __function_name, release_job_id, inner_job_id);
            zbx_free(jobnet_id);
            zbx_free(release_job_id);
            return ja_set_runerr(inner_job_id, 2);
        }

        /* pending release of the icon */
        result =
            DBselect
            ("select inner_jobnet_id from ja_run_jobnet_summary_table"
             " where jobnet_id = '%s' and status = %d",
             jobnet_id, JA_JOBNET_STATUS_RUN);
        //while ((row = DBfetch(result)) != NULL) {
        if ((row = DBfetch(result)) != NULL) {
            ZBX_STR2UINT64(inner_jobnet_id, row[0]);
            get_job_id = zbx_strdup(NULL, job_id);
            release_inner_job_id = ja_jobnet_get_job_id(inner_jobnet_id, get_job_id);
            zbx_free(get_job_id);
            if (release_inner_job_id == 0) {
            	ja_log("JARUNICONRELEASE200002", 0, NULL, inner_job_id, __function_name, release_job_id, release_inner_job_id);
            	zbx_free(jobnet_id);
            	zbx_free(release_job_id);
            	DBfree_result(result);
            	return ja_set_runerr(inner_job_id, 2);
            }

            DBfree_result(DBselect
                          ("select status from ja_run_job_table where inner_job_id = "
                           ZBX_FS_UI64 " for update", release_inner_job_id));
            db_ret =
                DBexecute
                ("update ja_run_job_table set method_flag = 0 where inner_job_id = "
                 ZBX_FS_UI64 " and method_flag = 1", release_inner_job_id);
            if (db_ret <= ZBX_DB_OK) {
            	ja_log("JARUNICONRELEASE200002", 0, NULL, inner_job_id, __function_name, release_job_id, release_inner_job_id);
              	zbx_free(jobnet_id);
            	zbx_free(release_job_id);
            	DBfree_result(result);
            	return ja_set_runerr(inner_job_id, 2);
            }
        }
        DBfree_result(result);
    }

    zbx_free(jobnet_id);
    zbx_free(release_job_id);
    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);

}
