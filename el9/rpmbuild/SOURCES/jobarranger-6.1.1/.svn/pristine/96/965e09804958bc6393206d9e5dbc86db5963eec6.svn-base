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

#include <json.h>

#include "common.h"
#include "log.h"
#include "db.h"

#include "jatelegram.h"
#include "jatrapauth.h"
#include "jajobnet.h"

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
int jatrap_jobrelease_jobid(zbx_uint64_t inner_job_id)
{
    int db_ret;
    const char *__function_name = "jatrap_jobrelease_jobid";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    DBfree_result(DBselect
                  ("select status from ja_run_job_table where inner_job_id = "
                   ZBX_FS_UI64 " for update", inner_job_id));
    db_ret =
        DBexecute
        ("update ja_run_job_table set method_flag = 0 where inner_job_id = "
         ZBX_FS_UI64 " and method_flag = 1", inner_job_id);
    if (db_ret > ZBX_DB_OK)
        return SUCCEED;
    return FAIL;

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
int jatrap_jobrelease(ja_telegram_object * obj)
{
    int ret;
    json_object *jp_data_req, *jp_data_res, *jp, *jp_jobnet;
    char *err;
    char *start_time, *job_id, *jobnet_id, *get_job_id, *get_job_id_2;
    char *str;
    zbx_uint64_t userid, inner_job_id, inner_jobnet_id;
    DB_RESULT result;
    DB_ROW row;
    int len = 0;
    const char *__function_name = "jatrap_jobrelease";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    ret = FAIL;
    job_id = NULL;
    start_time = NULL;
    inner_jobnet_id = 0;
    get_job_id = NULL;
    err = NULL;
    result = NULL;
    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    userid = jatrap_auth_user(obj);
    if (userid == 0)
        return FAIL;

    jp_data_req = json_object_object_get(obj->request, JA_PROTO_TAG_DATA);
    jp_data_res = json_object_object_get(obj->response, JA_PROTO_TAG_DATA);

    DBbegin();
    jp = json_object_object_get(jp_data_req, "jobid");
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         "jobid",
                         json_object_to_json_string(obj->request));
        goto error;
    }
    job_id = zbx_strdup(NULL, (char *) json_object_get_string(jp));
    get_job_id = strstr(job_id, "/");
    if (get_job_id == NULL) {
        err = zbx_dsprintf(NULL, "jobid '%s' is not correct", job_id);
        goto error;
    }
    *get_job_id = '\0';
    jobnet_id = job_id;
    get_job_id++;

    jp = json_object_object_get(jp_data_req, "start_time");
    if (jp != NULL)
        start_time = (char *) json_object_get_string(jp);
    jp = json_object_object_get(jp_data_req, "inner_jobnet_id");
    if (jp != NULL) {
        ZBX_STR2UINT64(inner_jobnet_id, (char *) json_object_get_string(jp));
        if (inner_jobnet_id <= 0) {
            err =
                zbx_dsprintf(NULL, "registry-number '%s' is not correct",
                             (char *) json_object_get_string(jp));
            goto error;
        }
    }

    jp_jobnet = json_object_new_array();
    json_object_object_add(jp_data_res, "inner_jobnet_id", jp_jobnet);
    if (inner_jobnet_id > 0) {
        if (jatrap_auth_jobnet(userid, inner_jobnet_id) == FAIL) {
            err =
                zbx_dsprintf(NULL,
                             "can not access the job. inner_jobnet_id: "
                             ZBX_FS_UI64, inner_jobnet_id);
            goto error;
        }
        result = DBselect("select inner_jobnet_id from ja_run_jobnet_summary_table"
                          " where inner_jobnet_id = " ZBX_FS_UI64
                          " and jobnet_id = '%s' and status = %d",
                          inner_jobnet_id, jobnet_id, JA_JOBNET_STATUS_RUN);
        if ((row = DBfetch(result)) != NULL) {
            get_job_id_2 = zbx_strdup(NULL, get_job_id);
            inner_job_id = ja_jobnet_get_job_id(inner_jobnet_id, get_job_id_2);
            zbx_free(get_job_id_2);
            if (inner_job_id == 0) {
                err =
                    zbx_dsprintf(NULL,
                                 "can not find the job id. inner_jobnet_id: "
                                 ZBX_FS_UI64 ", jobnet_id: '%s'",
                                 inner_jobnet_id, jobnet_id);
                goto error;
            }
            if (jatrap_jobrelease_jobid(inner_job_id) == FAIL) {
                err =
                    zbx_dsprintf(NULL,
                                 "icon is not pending. inner_jobnet_id: "
                                 ZBX_FS_UI64 ", jobnet_id: '%s'",
                                 inner_jobnet_id, jobnet_id);
                goto error;
            }
            str = zbx_dsprintf(NULL, ZBX_FS_UI64, inner_jobnet_id);
            json_object_array_add(jp_jobnet, json_object_new_string(str));
            zbx_free(str);
        }
    } else {
        if (start_time != NULL) {
            len = strlen(start_time);
            if (!(len == 8 || len == 12)) {
                err =
                    zbx_dsprintf(NULL, "unknowed start_time %s",
                                 start_time);
                goto error;
            }
        }
        result =
            DBselect
            ("select inner_jobnet_id, start_time from ja_run_jobnet_summary_table"
             " where jobnet_id = '%s' and status = %d",
             jobnet_id, JA_JOBNET_STATUS_RUN);
        while ((row = DBfetch(result)) != NULL) {
            ZBX_STR2UINT64(inner_jobnet_id, row[0]);
            if (start_time != NULL) {
                if (strncmp(start_time, row[1], len) != 0)
                    continue;
            }
            get_job_id_2 = zbx_strdup(NULL, get_job_id);
            inner_job_id = ja_jobnet_get_job_id(inner_jobnet_id, get_job_id_2);
            zbx_free(get_job_id_2);
            if (inner_job_id == 0) {
                err =
                    zbx_dsprintf(NULL,
                                 "can not find the job id. inner_jobnet_id: "
                                 ZBX_FS_UI64 ", jobnet_id: '%s'",
                                 inner_jobnet_id, jobnet_id);
                goto error;
            }
            if (jatrap_auth_jobnet(userid, inner_jobnet_id) == FAIL)
                continue;
            if (jatrap_jobrelease_jobid(inner_job_id) == FAIL) {
                err =
                    zbx_dsprintf(NULL,
                                 "icon is not pending. inner_jobnet_id: "
                                 ZBX_FS_UI64 ", jobnet_id: '%s'",
                                 inner_jobnet_id, jobnet_id);
                goto error;
            }
            str = zbx_dsprintf(NULL, ZBX_FS_UI64, inner_jobnet_id);
            json_object_array_add(jp_jobnet, json_object_new_string(str));
            zbx_free(str);
        }
    }

    if (json_object_array_length(jp_jobnet) == 0) {
        err = zbx_dsprintf(NULL, "can not match the jobnet");
        goto error;
    }
    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name, err);
        ja_telegram_seterr(obj, err);
        DBrollback();
    } else {
        json_object_object_add(jp_data_res, JA_PROTO_TAG_RESULT,
                               json_object_new_int(SUCCEED));
        DBcommit();
    }
    if (job_id != NULL)
        zbx_free(job_id);
    if (err != NULL)
        zbx_free(err);
    if (result != NULL)
        DBfree_result(result);
    return ret;
}
