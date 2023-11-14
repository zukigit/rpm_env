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
#include "jatrapjoblogput.h"
#include "jastr.h"
#include "jauser.h"

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
int jatrap_joblogput_load(ja_joblogput_object * job,
                          ja_telegram_object * obj)
{
    int ret;
    json_object *jp_data, *jp;
    char *request, *err;
    char *str = NULL;
    char *str2 = NULL;
    int len;
    zbx_uint64_t inner_jobnet_id;
    const char *__function_name = "jatrap_jobnetrun_load";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    if (job == NULL)
        return FAIL;

    ret = FAIL;
    err = NULL;
    job->username = NULL;
    job->password = NULL;
    job->from_time = NULL;
    job->to_time = NULL;
    job->jobnetid = NULL;
    job->jobid = NULL;
    job->target_user = NULL;
    job->registry_number = NULL;

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

    jp = json_object_object_get(jp_data, "from_time");
    if (jp != NULL) {
        str = (char *) json_object_get_string(jp);
        len = strlen(str);
        if (len == 8) {
            job->from_time = zbx_dsprintf(NULL, "%s000000", str);
        } else if (len == 12) {
            job->from_time = zbx_dsprintf(NULL, "%s00", str);
        } else {
            job->from_time = zbx_dsprintf(NULL, "00");
        }
        if (ja_str2timestamp(job->from_time) == 0) {
            err = zbx_dsprintf(NULL, "Unknowed from_time: %s", str);
            goto error;
        }
    }

    jp = json_object_object_get(jp_data, "to_time");
    if (jp != NULL) {
        str2 = (char *) json_object_get_string(jp);
        len = strlen(str2);
        if (len == 8) {
            job->to_time = zbx_dsprintf(NULL, "%s235959", str2);
        } else if (len == 12) {
            job->to_time = zbx_dsprintf(NULL, "%s59", str2);
        } else {
            job->to_time = zbx_dsprintf(NULL, "00");
        }
        if (ja_str2timestamp(job->to_time) == 0) {
            err = zbx_dsprintf(NULL, "Unknowed to_time: %s", str2);
            goto error;
        }
    }

    if (job->from_time != NULL && job->to_time != NULL) {
        if (strcmp(job->from_time, job->to_time) > 0) {
            err = zbx_dsprintf(NULL, "to_time is greater than the from_time: from_time %s to_time %s", str, str2);
            goto error;
        }
    }

    jp = json_object_object_get(jp_data, "jobnetid");
    if (jp != NULL) {
        job->jobnetid =
            zbx_strdup(NULL, (char *) json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, "jobid");
    if (jp != NULL) {
        job->jobid = zbx_strdup(NULL, (char *) json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, "target_user");
    if (jp != NULL) {
        job->target_user =
            zbx_strdup(NULL, (char *) json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, "registry_number");
    if (jp != NULL) {
        job->registry_number =
            zbx_strdup(NULL, (char *) json_object_get_string(jp));
        inner_jobnet_id = 0;
        ZBX_STR2UINT64(inner_jobnet_id, job->registry_number);
        if (inner_jobnet_id <= 0) {
            err =
                zbx_dsprintf(NULL, "registry_number: '%s' is not correct",
                             job->registry_number);
            goto error;
        }
    }

    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   err);
        ja_telegram_seterr(obj, err);
        jatrap_joblogput_free(job);
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
void jatrap_joblogput_free(ja_joblogput_object * job)
{
    const char *__function_name = "jatrap_joblogput_free";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return;
    if (job->username != NULL)
        zbx_free(job->username);
    if (job->password != NULL)
        zbx_free(job->password);
    if (job->from_time != NULL)
        zbx_free(job->from_time);
    if (job->to_time != NULL)
        zbx_free(job->to_time);
    if (job->jobnetid != NULL)
        zbx_free(job->jobnetid);
    if (job->jobid != NULL)
        zbx_free(job->jobid);
    if (job->target_user != NULL)
        zbx_free(job->target_user);
    if (job->registry_number != NULL)
        zbx_free(job->registry_number);

    job->username = NULL;
    job->password = NULL;
    job->from_time = NULL;
    job->to_time = NULL;
    job->jobnetid = NULL;
    job->jobid = NULL;
    job->target_user = NULL;
    job->registry_number = NULL;
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
int jatrap_joblogput(ja_telegram_object * obj)
{
    int ret;
    ja_joblogput_object job;
    zbx_uint64_t userid, userid2;
    char *err;
    char *user_lang;
    char csv[JA_MAX_DATA_LEN];
    char sql[JA_MAX_DATA_LEN];
    char *msg, jobid[JA_MAX_STRING_LEN], job_name[JA_MAX_STRING_LEN],
        exit_cd[255];
    char *jobnet_id, *job_id, *user_name;
    DB_RESULT result;
    DB_ROW row;
    json_object *jp_data, *jp_csv;
    char log_date[24], update_date[20];
    int user_type, public_flag, user_cmp;
    const char *__function_name = "jatrap_joblogput";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() %s", __function_name,
               (char *) json_object_to_json_string(obj->request));
    ret = FAIL;
    err = NULL;
    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    if (jatrap_joblogput_load(&job, obj) == FAIL)
        goto error;

    jp_data = json_object_object_get(obj->response, JA_PROTO_TAG_DATA);
    jp_csv = json_object_new_array();
    json_object_object_add(jp_data, "joblog", jp_csv);
    userid = ja_user_auth(job.username, job.password);
    if (userid == 0) {
        err =
            zbx_dsprintf(NULL,
                         "authentication failed. username: %s",
                         job.username);
        goto error;
    }
    if (ja_user_status(userid) != 0) {
        err =
            zbx_dsprintf(NULL, "invalid user, username: %s", job.username);
        goto error;
    }

    if (job.registry_number == NULL) {
        if (job.from_time == NULL || job.to_time == NULL) {
            err = zbx_dsprintf(NULL, "Can not get 'from_time' and 'to_time' from the telegram");
            goto error;
        }
    }

    user_type = ja_user_type(userid);
    user_lang = ja_user_lang(userid);

    if (job.registry_number != NULL) {
        zbx_snprintf(sql, sizeof(sql), "a.inner_jobnet_main_id = %s", job.registry_number);
    } else {
        zbx_snprintf(sql, sizeof(sql), "a.log_date >= %s000 and a.log_date <= %s999", job.from_time, job.to_time);
    }

    zbx_snprintf(csv, sizeof(csv), "\"log date\",\"inner jobnet main id\",\"inner jobnet id\",\"run type\",\"public flag\",\"jobnet id\",\"job id\",\"message id\",\"message\",\"jobnet name\",\"job name\",\"user name\",\"update date\",\"return code\"");
    json_object_array_add(jp_csv, json_object_new_string(csv));

    result = DBselect("select a.log_date, a.inner_jobnet_id, a.inner_jobnet_main_id, a.inner_job_id, a.update_date, b.log_type, a.method_flag,"
                      " a.jobnet_status, a.job_status, a.run_type, a.public_flag, a.jobnet_id, a.jobnet_name, a.job_id, a.job_name, a.user_name,"
                      " a.return_code, a.std_out, a.std_err, a.message_id, b.message"
                      " from ja_run_log_table a, ja_define_run_log_message_table b"
                      " where %s"
                      " and b.lang = '%s' and a.message_id = b.message_id order by a.inner_jobnet_main_id, a.log_date",
                      sql, user_lang);

    zbx_free(user_lang);

    while ((row = DBfetch(result)) != NULL) {
        public_flag = atoi(row[10]);
        jobnet_id = row[11];
        job_id = row[13];
        user_name = row[15];
        userid2 = ja_user_id(user_name);
        if (ja_user_groups(userid, userid2) > 0)
            user_cmp = 1;
        else
            user_cmp = 0;

        if (!
            (user_type == USER_TYPE_SUPER_ADMIN || public_flag == 1
             || user_cmp == 1))
            continue;

        if (job.jobnetid != NULL) {
            if (jobnet_id == NULL)
                continue;
            if (NULL == zbx_regexp_match(jobnet_id, job.jobnetid, NULL))
                continue;
        }
        if (job.jobid != NULL) {
            if (job_id == NULL)
                continue;
            if (NULL == zbx_regexp_match(job_id, job.jobid, NULL))
                continue;
        }
        if (job.target_user != NULL) {
            if (user_name == NULL)
                continue;
            if (NULL == zbx_regexp_match(user_name, job.target_user, NULL))
                continue;
        }
        ja_format_timestamp(row[0], log_date);
        ja_format_timestamp(row[4], update_date);
        msg = DBdyn_escape_string(row[20]);
        if (row[13] == NULL) {
            zbx_snprintf(jobid, sizeof(jobid), "");
        } else {
            zbx_snprintf(jobid, sizeof(jobid), "%s", row[13]);
        }
        if (row[14] == NULL) {
            zbx_snprintf(job_name, sizeof(job_name), "");
        } else {
            zbx_snprintf(job_name, sizeof(job_name), "%s", row[14]);
        }
        if (row[16] == NULL) {
            zbx_snprintf(exit_cd, sizeof(exit_cd), "");
        } else {
            zbx_snprintf(exit_cd, sizeof(exit_cd), "%s", row[16]);
        }
        zbx_snprintf(csv, sizeof(csv),
                     "\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\",\"%s\"",
                     log_date, row[2], row[1], row[9], row[10], row[11],
                     jobid, row[19], msg, row[12], job_name, row[15],
                     update_date, exit_cd);
        zbx_free(msg);
        json_object_array_add(jp_csv, json_object_new_string(csv));
    }
    DBfree_result(result);

    ret = SUCCEED;

  error:
    if (ret == FAIL) {
        if (err != NULL) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                       err);
            ja_telegram_seterr(obj, err);
        }
    }

    if (err != NULL)
        zbx_free(err);

    jatrap_joblogput_free(&job);
    return ret;
}
