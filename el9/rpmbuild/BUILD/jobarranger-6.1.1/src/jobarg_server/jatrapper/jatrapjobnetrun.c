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

#include "jacommon.h"
#include "jalog.h"
#include "jajoblog.h"
#include "jauser.h"
#include "jatrapjobnetrun.h"
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
int jatrap_jobnetrun_load(ja_jobnetrun_object * job,
                          ja_telegram_object * obj)
{
    int ret;
    json_object *jp_data, *jp;
    char *request, *err;
    const char *__function_name = "jatrap_jobnetrun_load";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    if (job == NULL)
        return FAIL;

    ret = FAIL;
    job->username = NULL;
    job->password = NULL;
    job->jobnetid = NULL;
    job->start_time = NULL;
    job->env = NULL;
    err = NULL;

    request = (char *) json_object_to_json_string(obj->request);
    jp_data = json_object_object_get(obj->request, JA_PROTO_TAG_DATA);

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_USERNAME);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_USERNAME, request);
        goto error;
    }
    job->username = zbx_strdup(NULL, (char *) json_object_get_string(jp));

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_PASSWORD);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_PASSWORD, request);
        goto error;
    }
    job->password = zbx_strdup(NULL, (char *) json_object_get_string(jp));

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_JOBNETID);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_JOBNETID, request);
        goto error;
    }
    job->jobnetid = zbx_strdup(NULL, (char *) json_object_get_string(jp));

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_STARTTIME);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_STARTTIME, request);
        goto error;
    }
    job->start_time =
        zbx_strdup(NULL, (char *) json_object_get_string(jp));

    job->env = json_object_object_get(jp_data, JA_PROTO_TAG_ENV);

    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   err);
        ja_telegram_seterr(obj, err);
        jatrap_jobnetrun_free(job);
    }

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
void jatrap_jobnetrun_free(ja_jobnetrun_object * job)
{
    const char *__function_name = "jatrap_jobnetrun_free";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return;
    if (job->username != NULL)
        zbx_free(job->username);
    if (job->password != NULL)
        zbx_free(job->password);
    if (job->jobnetid != NULL)
        zbx_free(job->jobnetid);
    if (job->start_time != NULL)
        zbx_free(job->start_time);

    job->username = NULL;
    job->password = NULL;
    job->jobnetid = NULL;
    job->start_time = NULL;
    job->env = NULL;
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
int jatrap_jobnetrun_auth(ja_jobnetrun_object * job)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t userid, usrgrpid;
    char *jobnet_id;
    int user_type, public_flag, user_cmp;
    const char *__function_name = "job_exec_auth";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return FAIL;

    public_flag = 0;
    user_cmp = 0;
    userid = ja_user_auth(job->username, job->password);
    if (userid == 0) {
        ja_log("JATRAPPER200008", 0, job->jobnetid, 0);
        return FAIL;
    }
    if (ja_user_status(userid) != 0) {
        ja_log("JATRAPPER200050", 0, job->jobnetid, 0, job->username);
        return FAIL;
    }
    user_type = ja_user_type(userid);
    usrgrpid = ja_user_usrgrpid(userid);

    ret = SUCCEED;
    jobnet_id = DBdyn_escape_string(job->jobnetid);
    result =
        DBselect
        ("select user_name, public_flag from ja_jobnet_control_table where jobnet_id = '%s' and valid_flag = 1",
         jobnet_id);
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JATRAPPER200046", 0, jobnet_id, 0, jobnet_id);
        ret = FAIL;
    } else {
        public_flag = atoi(row[1]);
        if (usrgrpid == ja_user_usrgrpid(ja_user_id(row[0]))
            && usrgrpid > 0)
            user_cmp = 1;
    }
    zbx_free(jobnet_id);
    DBfree_result(result);

    if (ret == FAIL)
        return FAIL;
    if (user_type == USER_TYPE_SUPER_ADMIN || public_flag == 1
        || user_cmp == 1)
        return SUCCEED;
    ja_log("JATRAPPER200047", 0, jobnet_id, 0, job->jobnetid);
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
int jatrap_jobnetrun(ja_telegram_object * obj)
{
    int ret;
    ja_jobnetrun_object job;
    char *err;
    const char *__function_name = "jatrap_jobnetrun";

    zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
               (char *) json_object_to_json_string(obj->request));
    ret = FAIL;
    err = NULL;

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    if (jatrap_jobnetrun_load(&job, obj) == FAIL)
        return FAIL;
    if (jatrap_jobnetrun_auth(&job) == FAIL) {
    }

    return SUCCEED;
}
