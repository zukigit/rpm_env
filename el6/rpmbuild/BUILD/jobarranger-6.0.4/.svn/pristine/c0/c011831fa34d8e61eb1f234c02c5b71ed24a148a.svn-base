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
#include "comms.h"
#include "log.h"

#include "jacommon.h"
#include "jastr.h"
#include "jajobobject.h"
#include "jatelegram.h"

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
int ja_telegram_new(ja_telegram_object * obj)
{
    json_object *jp;
    const char *__function_name = "ja_telegram_new";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (obj == NULL)
        return FAIL;

    obj->request = json_object_new_object();
    obj->response = json_object_new_object();

    jp = json_object_new_object();
    json_object_object_add(obj->request, JA_PROTO_TAG_DATA, jp);
    jp = json_object_new_object();
    json_object_object_add(obj->response, JA_PROTO_TAG_DATA, jp);

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
void ja_telegram_clear(ja_telegram_object * obj)
{
    const char *__function_name = "ja_telegram_clear";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (obj == NULL)
        return;

    if (obj->request != NULL)
        json_object_put(obj->request);
    if (obj->response != NULL)
        json_object_put(obj->response);
    obj->request = NULL;
    obj->response = NULL;
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
int ja_telegram_check(ja_telegram_object * obj)
{
    json_object *jp;
    const char *__function_name = "ja_telegram_check";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (obj == NULL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s() ja_telegram_object is null",
                   __function_name);
        return FAIL;
    }

    if (obj->request == NULL) {
        zabbix_log(LOG_LEVEL_WARNING,
                   "In %s() ja_telegram_object request is null",
                   __function_name);
        return FAIL;
    }

    if (obj->response == NULL) {
        zabbix_log(LOG_LEVEL_WARNING,
                   "In %s() ja_telegram_object response is null",
                   __function_name);
        return FAIL;
    }

    jp = json_object_object_get(obj->request, JA_PROTO_TAG_DATA);
    if (jp == NULL) {
        zabbix_log(LOG_LEVEL_WARNING,
                   "In %s() ja_telegram_object request have not the tag '%s'",
                   __function_name, JA_PROTO_TAG_DATA);
        return FAIL;
    }

    jp = json_object_object_get(obj->response, JA_PROTO_TAG_DATA);
    if (jp == NULL) {
        zabbix_log(LOG_LEVEL_WARNING,
                   "In %s() ja_telegram_object response have not the tag '%s'",
                   __function_name, JA_PROTO_TAG_DATA);
        return FAIL;
    }

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
int ja_telegram_seterr(ja_telegram_object * obj, const char *message)
{
    json_object *jp;
    const char *__function_name = "ja_telegram_seterr";

    if (obj == NULL || message == NULL)
        return FAIL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() message: %s", __function_name,
               message);

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    jp = json_object_object_get(obj->response, JA_PROTO_TAG_DATA);

    json_object_object_add(jp, JA_PROTO_TAG_RESULT,
                           json_object_new_int(FAIL));
    json_object_object_add(jp, JA_PROTO_TAG_MESSAGE,
                           json_object_new_string(message));

    return SUCCEED;
}

// error for data recovery (added new parameter and new json tag)
int ja_telegram_seterr_backup(ja_telegram_object * obj, const char *message, const* pre_unique_id)
{
    json_object *jp;
    const char *__function_name = "ja_telegram_seterr";

    if (obj == NULL || message == NULL || pre_unique_id == NULL)
        return FAIL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() message: %s", __function_name,
               message);

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;
    jp = json_object_object_get(obj->response, JA_PROTO_TAG_DATA);

    json_object_object_add(jp, JA_PROTO_TAG_RESULT,
                           json_object_new_int(FAIL));
    json_object_object_add(jp, JA_PROTO_TAG_MESSAGE,
                           json_object_new_string(message));
    json_object_object_add(jp, JA_PROTO_TAG_PRE_UNIQUE_ID,
                           json_object_new_string(pre_unique_id));

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
int ja_telegram_recv(ja_telegram_object * obj, zbx_sock_t * s, int timeout)
{
    int ret;
    char *data, *err;
    const char *__function_name = "ja_telegram_recv";

    if (obj == NULL || s == NULL)
        return FAIL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    ret = FAIL;
    err = NULL;
    if (zbx_tcp_recv_to(s, &data, timeout) == FAIL) {
        err = zbx_dsprintf(NULL, "recv error: %s", zbx_tcp_strerror());
        goto error;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() data: %s", __function_name, data);
    if (strlen(data) == 0) {
        err = zbx_dsprintf(NULL, "the received data is null");
        goto error;
    }

    json_object_put(obj->request);
    obj->request = json_tokener_parse(data);
    if (is_error(obj->request)) {
        err = zbx_dsprintf(NULL, "can not parse the json data: %s", data);
        obj->request = NULL;
        goto error;
    }

    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   err);
        ja_telegram_seterr(obj, err);
    }
    if (err != NULL)
        zbx_free(err);
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
int ja_telegram_from(const char *telegram, ja_job_object * job)
{
    int ret;
    json_object *jp_telegram;
    const char *__function_name = "ja_telegram_from";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() telegram: %s", __function_name,
               telegram);
    if (job == NULL)
        return FAIL;

    ret = FAIL;
    jp_telegram = json_tokener_parse(telegram);
    if (is_error(jp_telegram)) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't parse json data. telegram: %s", telegram);
        jp_telegram = NULL;
        goto error;
    }

    ret = ja_telegram_from_head(jp_telegram, job);
    if (ret == FAIL)
        goto error;

    if (strcmp(job->kind, JA_PROTO_VALUE_JOBRUN) == 0
        || strcmp(job->kind, JA_PROTO_VALUE_FCOPY) == 0
        || strcmp(job->kind, JA_PROTO_VALUE_JOBRESULT) == 0) {
        ret = ja_telegram_from_request(jp_telegram, job);
    } else if (strcmp(job->kind, JA_PROTO_VALUE_JOBRUN_RES) == 0
               || strcmp(job->kind, JA_PROTO_VALUE_JOBRESULT_RES) == 0
               || strcmp(job->kind, JA_PROTO_VALUE_FCOPY_RES) == 0) {
        ret = ja_telegram_from_response(jp_telegram, job);
    } else if(strcmp(job->kind, JA_PROTO_VALUE_CHKJOB)==0){
        //get_chkjob_list.
        ret = ja_telegram_from_chkjob(jp_telegram, job);
    } else if(strcmp(job->kind, JA_PROTO_VALUE_IPCHANGE)==0){
        ret = ja_telegram_from_ip_response(jp_telegram, job);
    }else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Unsupport the kind [%s]", job->kind);
        ret = FAIL;
        goto error;
    }

  error:
    if (jp_telegram != NULL)
        json_object_put(jp_telegram);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
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
int ja_telegram_from_head(json_object * json, ja_job_object * job)
{
    json_object *jp;
    char *telegram;
    double tele_version;
    const char *__function_name = "ja_telegram_from_head";

    if (json == NULL || job == NULL)
        return FAIL;

    telegram = (char *) json_object_get_string(json);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() telegram: %s", __function_name,
               telegram);

    if (json_object_get_type(json) != json_type_object) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "[%s] is not an object", telegram);
        return FAIL;
    }
    // KIND
    jp = json_object_object_get(json, JA_PROTO_TAG_KIND);
    if (jp != NULL) {
        zbx_snprintf(job->kind, sizeof(job->kind),
                     "%s", json_object_get_string(jp));
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_KIND, telegram);
        return FAIL;
    }

    // SERVERID
    jp = json_object_object_get(json, JA_PROTO_TAG_SERVERID);
    if (jp != NULL) {
        zbx_snprintf(job->serverid, sizeof(job->serverid),
                     "%s", json_object_get_string(jp));
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_SERVERID, telegram);
        return FAIL;
    }

    // VERSION
    jp = json_object_object_get(json, JA_PROTO_TAG_VERSION);
    if (jp != NULL) {
        job->version = json_object_get_double(jp);
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_VERSION, telegram);
        return FAIL;
    }
    if(strcmp(job->kind,JA_PROTO_VALUE_IPCHANGE) == 0) {
        tele_version = 1.3;
    } else {
        tele_version = JA_PROTO_TELE_VERSION;
    }
    if (job->version != tele_version) {
        job->result = JA_RESPONSE_VERSION_FAIL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "%s:[%d] does not match [%d]", JA_PROTO_TAG_VERSION,
                     job->version, tele_version);
        return FAIL;
    }
    // HOSTNAME
    jp = json_object_object_get(json, JA_PROTO_TAG_HOSTNAME);
    if (jp != NULL) {
        zbx_snprintf(job->hostname, sizeof(job->hostname), "%s",
                     json_object_get_string(jp));
    } else {
        if (CONFIG_HOSTNAME != NULL)
            zbx_snprintf(job->hostname, sizeof(job->hostname), "%s",
                         CONFIG_HOSTNAME);
    }

    if (CONFIG_HOSTNAME != NULL) {
        if (strcmp(job->hostname, CONFIG_HOSTNAME) != 0) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "%s '%s' does not match Hostname",
                         JA_PROTO_TAG_HOSTNAME, job->hostname);
            return FAIL;
        }
    }
    // DATA
    if(strcmp(job->kind,JA_PROTO_VALUE_IPCHANGE) == 0)
        return SUCCEED;
    jp = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_DATA, telegram);
        return FAIL;
    }
    
    if (strcmp(job->kind, JA_PROTO_VALUE_CHKJOB) != 0 && json_object_get_type(jp) != json_type_object) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "[%s] is not an object from json data [%s]",
                     JA_PROTO_TAG_DATA, telegram);
        return FAIL;
    }

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
int ja_telegram_from_request(json_object * json, ja_job_object * job)
{
    json_object *jp_data, *jp;
    char *data;
    const char *__function_name = "ja_telegram_from_request";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (json == NULL || job == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;
    data = (char *) json_object_get_string(jp_data);

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_JOBID);
    if (jp != NULL) {
        str2uint64((char *) json_object_get_string(jp), "KMGT",
                   &(job->jobid));
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_JOBID, data);
        return FAIL;
    }
    if (job->jobid <= 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "%s:[" ZBX_FS_UI64 "] must be more than 0",
                     JA_PROTO_TAG_JOBID, job->jobid);
        return FAIL;
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_METHOD);
    if (jp != NULL) {
        job->method = json_object_get_int(jp);
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_METHOD, data);
        return FAIL;
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_CUR_UNIQUE_ID);
    if (jp != NULL) {
        zbx_snprintf(job->cur_unique_id, sizeof(job->cur_unique_id), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_PRE_UNIQUE_ID);
    if (jp != NULL) {
        zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_TYPE);
    if (jp != NULL) {
        zbx_snprintf(job->type, sizeof(job->type), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_ARGUMENT);
    if (jp != NULL) {
        zbx_snprintf(job->argument, sizeof(job->argument), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_SCRIPT);
    if (jp != NULL) {
        zbx_snprintf(job->script, sizeof(job->script), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_ENV);
    if (jp != NULL) {
        if (json_object_get_type(jp) != json_type_object) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "[%s] is not an object from json data [%s]",
                         JA_PROTO_TAG_ENV, data);
            return FAIL;
        }
        zbx_snprintf(job->env, sizeof(job->env), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RESULT);
    if (jp != NULL) {
        job->result = json_object_get_int(jp);
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_JOBSTDOUT);
    if (jp != NULL) {
        zbx_snprintf(job->std_out, sizeof(job->std_out), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_JOBSTDERR);
    if (jp != NULL) {
        zbx_snprintf(job->std_err, sizeof(job->std_err), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RET);
    if (jp != NULL) {
        job->return_code = json_object_get_int(jp);
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_SIGNAL);
    if (jp != NULL) {
        job->signal = json_object_get_int(jp);
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_MESSAGE);
    if (jp != NULL) {
        zbx_snprintf(job->message, sizeof(job->message), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_VALUE_RUNUSR);
    if (jp != NULL) {
        zbx_snprintf(job->run_user, sizeof(job->run_user), "%s",
                     json_object_get_string(jp));
    }

    jp = json_object_object_get(jp_data, JA_PROTO_VALUE_RUNUSRPWD);
    if (jp != NULL) {
        zbx_snprintf(job->run_user_password, sizeof(job->run_user_password), "%s",
                     json_object_get_string(jp));
    }

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
int ja_telegram_from_response(json_object * json, ja_job_object * job)
{
    json_object *jp_data, *jp;
    char *data;
    const char *__function_name = "ja_telegram_from_response";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (json == NULL || job == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;
    data = (char *) json_object_get_string(jp_data);

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RESULT);
    if (jp != NULL) {
        job->result = json_object_get_int(jp);
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_RESULT, data);
        return FAIL;
    }

    if (job->result != JA_RESPONSE_SUCCEED) {
        jp = json_object_object_get(jp_data, JA_PROTO_TAG_MESSAGE);
        if (jp != NULL) {
            zbx_snprintf(job->message, sizeof(job->message), "%s",
                         json_object_get_string(jp));
        }
    }

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
int ja_telegram_from_chkjob(json_object *json, ja_job_object *job)
{
    json_object *jp_data, *data, *jobid_obj;
    zbx_uint64_t tmp_value;
    size_t arraylen;
    // char jobid[MAX_STRING_LEN];
    int i;
    const char *__function_name = "ja_telegram_from_chkjob";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (json == NULL || job == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data != NULL)
    {
        data = (char *)json_object_object_get(jp_data, JA_PROTO_TAG_HOST_JOB_LIST);
        if (data != NULL)
        {

            arraylen = json_object_array_length(data);
            if(json_object_get_type(data) != json_type_array){
                zbx_snprintf(job->message, sizeof(job->message),
                                 "Check data object type does not match. jobid :"ZBX_FS_UI64,job->jobid);
                return FAIL;
            }
            //job->host_running_job = realloc(job->host_running_job, sizeof(zbx_uint64_t)*(arraylen+1)); removed code
            job->host_running_job  = (zbx_uint64_t*)malloc((arraylen+1) * sizeof(zbx_uint64_t));
            job->size_of_host_running_job = arraylen;
            for (i = 0; i < arraylen; i++)
            {
                jobid_obj = json_object_array_get_idx(data, i);
                if (jobid_obj != NULL)
                {
                    //zabbix_log(LOG_LEVEL_ERR,"[Debug] In %s(), value : %s",__function_name,json_object_get_string(jobid_obj));
                    str2uint64((char *)json_object_get_string(jobid_obj), "KMGT",
                               &tmp_value);
                    job->host_running_job[i] = tmp_value;
                }
                else
                {
                    zbx_snprintf(job->message, sizeof(job->message),
                                 "Can't get check job list from telegram. jobid :"ZBX_FS_UI64,job->jobid);
                    return FAIL;
                }
            }
        }
        else
        {
            zbx_snprintf(job->message, sizeof(job->message),
                         "Can't get the tag [%s] from json data. jobid :"ZBX_FS_UI64,
                         JA_PROTO_TAG_HOST_JOB_LIST, job->jobid);
            return FAIL;
        }
    }
    else
    {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data. jobid : "ZBX_FS_UI64,
                     JA_PROTO_TAG_DATA, job->jobid);
        return FAIL;
    }
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
int ja_telegram_from_jobresult_res(const char *telegram,
                                   ja_job_object * job)
{
    int ret;
    char *buf, *data;
    json_object *jp_telegram, *jp_data, *jp;
    const char *__function_name = "ja_telegram_from_jobresult_res";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return FAIL;

    buf = NULL;
#ifdef _WINDOWS
    buf = (char *) ja_utf8_to_acp(telegram);
#else
    buf = zbx_strdup(buf, telegram);
#endif
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() telegram: %s", __function_name,
               buf);

    ret = FAIL;
    jp_telegram = json_tokener_parse(buf);
    if (is_error(jp_telegram)) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't parse json data. telegram [%s]", buf);
        jp_telegram = NULL;
        job->result = JA_JOBRESULT_RES_FAIL;
        goto exit;
    }

    if (ja_telegram_from_head(jp_telegram, job) != 0) {
        job->result = JA_JOBRESULT_RES_FAIL;
        goto exit;
    }

    if (strcmp(job->kind, JA_PROTO_VALUE_JOBRESULT_RES) != 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "the kind: %s does not match %s",
                     job->kind, JA_PROTO_VALUE_JOBRESULT_RES);
        job->result = JA_JOBRESULT_RES_FAIL;
        goto exit;
    }

    jp_data = json_object_object_get(jp_telegram, JA_PROTO_TAG_DATA);
    data = (char *) json_object_get_string(jp_data);
    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RESULT);
    if (jp != NULL) {
        job->result = json_object_get_int(jp);
    } else {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can't get the tag [%s] from json data [%s]",
                     JA_PROTO_TAG_RESULT, data);
        job->result = JA_JOBRESULT_RES_FAIL;
        goto exit;
    }

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_MESSAGE);
    if (jp != NULL) {
        zbx_snprintf(job->message, sizeof(job->message), "%s",
                     json_object_get_string(jp));
    }
    ret = SUCCEED;

  exit:
    zbx_free(buf);
    if (jp_telegram != NULL)
        json_object_put(jp_telegram);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
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
int ja_telegram_from_ip_response(json_object * json, ja_job_object * job) {
    json_object *jp_data, *jp;
    char *data;
    const char *__function_name = "ja_telegram_from_ip_response";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (json == NULL || job == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;
    data = (char *) json_object_get_string(jp_data);

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RET);
    if (jp != NULL) {
        job->return_code = json_object_get_int(jp);
    } else {
        return FAIL;
    }

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
char *ja_telegram_to(ja_job_object * job)
{
    int ret;
    json_object *jp_telegram, *jp_data;
    char *telegram;
    const char *__function_name = "ja_telegram_to";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL)
        return NULL;

    telegram = NULL;
    jp_telegram = json_object_new_object();
    ret = ja_telegram_to_head(job, jp_telegram);
    jp_data = json_object_new_object();
    json_object_object_add(jp_telegram, JA_PROTO_TAG_DATA, jp_data);

    if (strcmp(job->kind, JA_PROTO_VALUE_IPCHANGE) != 0){
        if (strcmp(job->kind, JA_PROTO_VALUE_JOBRUN) == 0
        || strcmp(job->kind, JA_PROTO_VALUE_FCOPY) == 0) {
            ret = ja_telegram_to_request(job, jp_telegram);
        } else if (strcmp(job->kind, JA_PROTO_VALUE_JOBRESULT) == 0) {
            ret = ja_telegram_to_jobresult(job, jp_telegram);
        } else
            if (strcmp(job->kind, JA_PROTO_VALUE_JOBRUN_RES) == 0
                || strcmp(job->kind, JA_PROTO_VALUE_FCOPY_RES) == 0
                || strcmp(job->kind, JA_PROTO_VALUE_CHKJOB) == 0) {
            ret = ja_telegram_to_response(job, jp_telegram);
        } else{
            ret = FAIL;
        }
    }else{
        ret = ja_telegram_to_ip_response(job, jp_telegram);
    }

    if (ret == SUCCEED) {
        telegram =
            zbx_strdup(telegram,
                       (char *) json_object_to_json_string(jp_telegram));
    }
    json_object_put(jp_telegram);
    return telegram;
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
int ja_telegram_to_head(ja_job_object * job, json_object * json)
{
    const char *__function_name = "ja_telegram_to_head";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || json == NULL)
        return FAIL;
    json_object_object_add(json, JA_PROTO_TAG_KIND,
                           json_object_new_string(job->kind));
    json_object_object_add(json, JA_PROTO_TAG_VERSION,
                           json_object_new_double(job->version));
    json_object_object_add(json, JA_PROTO_TAG_SERVERID,
                           json_object_new_string(job->serverid));
    json_object_object_add(json, JA_PROTO_TAG_HOSTNAME,
                           json_object_new_string(job->hostname));
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
int ja_telegram_to_request(ja_job_object * job, json_object * json)
{
    json_object *jp_data;
    json_object *jp_argument, *jp_env;
    char str[JA_MAX_STRING_LEN];
    const char *__function_name = "ja_telegram_to_request";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || json == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;

    zbx_snprintf(str, sizeof(str), ZBX_FS_UI64, job->jobid);
    json_object_object_add(jp_data, JA_PROTO_TAG_JOBID,
                           json_object_new_string(str));

    json_object_object_add(jp_data, JA_PROTO_TAG_TYPE,
                           json_object_new_string(job->type));
    json_object_object_add(jp_data, JA_PROTO_TAG_METHOD,
                           json_object_new_int(job->method));
    json_object_object_add(jp_data, JA_PROTO_TAG_SCRIPT,
                           json_object_new_string(job->script));

    json_object_object_add(jp_data, JA_PROTO_TAG_CUR_UNIQUE_ID,
                           json_object_new_string(job->cur_unique_id));

    json_object_object_add(jp_data, JA_PROTO_TAG_PRE_UNIQUE_ID,
                           json_object_new_string(job->pre_unique_id));
    if (strlen(job->argument) > 0)
        jp_argument = json_tokener_parse(job->argument);
    else
        jp_argument = json_tokener_parse("{}");
    if (is_error(jp_argument)) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() can not parse [argument]. Do you Check if env is too large. \n %s",
                   __function_name, job->argument);
        return FAIL;
    }
    json_object_object_add(jp_data, JA_PROTO_TAG_ARGUMENT, jp_argument);

    if (strlen(job->env) > 0)
        jp_env = json_tokener_parse(job->env);
    else
        jp_env = json_tokener_parse("{}");
    if (is_error(jp_env)) {
        json_object_put(jp_argument);
        zabbix_log(LOG_LEVEL_ERR, "In %s() can not parse [env].  Do you Check if env is too large. \n %s",
                   __function_name, job->env);
        return FAIL;
    }
    json_object_object_add(jp_data, JA_PROTO_TAG_ENV, jp_env);

    json_object_object_add(jp_data, JA_PROTO_VALUE_RUNUSR,
                           json_object_new_string(job->run_user));
    json_object_object_add(jp_data, JA_PROTO_VALUE_RUNUSRPWD,
                           json_object_new_string(job->run_user_password));

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
int ja_telegram_to_response(ja_job_object * job, json_object * json)
{
    json_object *jp_data;
    json_object *jp_joblist;
    const char *__function_name = "ja_telegram_to_response";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || json == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;
    json_object_object_add(jp_data, JA_PROTO_TAG_RESULT,
                           json_object_new_int(job->result));
    if (job->result != JA_RESPONSE_SUCCEED) {
        json_object_object_add(jp_data, JA_PROTO_TAG_MESSAGE,
                               json_object_new_string(job->message));
    }
    if (job->result == JA_RESPONSE_VERSION_FAIL) {
        json_object_object_add(jp_data, JA_PROTO_TAG_MYVERSION,
                               json_object_new_int(JA_PROTO_TELE_VERSION));
    }

    if (strcmp(job->kind, JA_PROTO_VALUE_CHKJOB) == 0) {
        jp_joblist = json_object_new_array();
		int i;
        for (i = 0; i < job->size_of_host_running_job; i++) {
            if(i > job->size_of_host_running_job){
                break;
            }
            if(job->host_running_job[i] <= 0){
                break;
            }
            json_object_array_add(jp_joblist, json_object_new_int(job->host_running_job[i]));
        }
        json_object_object_add(jp_data, JA_PROTO_TAG_HOST_JOB_LIST, jp_joblist);
    }

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
int ja_telegram_to_jobresult(ja_job_object * job, json_object * json)
{
    json_object *jp_data;
    char str[JA_MAX_STRING_LEN];
    const char *__function_name = "ja_telegram_to_jobresult";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || json == NULL)
        return FAIL;

    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL)
        return FAIL;

    zbx_snprintf(str, sizeof(str), ZBX_FS_UI64, job->jobid);
    json_object_object_add(jp_data, JA_PROTO_TAG_JOBID,
                           json_object_new_string(str));

    json_object_object_add(jp_data, JA_PROTO_TAG_HOSTNAME,
                           json_object_new_string(CONFIG_HOSTNAME));
    json_object_object_add(jp_data, JA_PROTO_TAG_METHOD,
                           json_object_new_int(job->method));
    json_object_object_add(jp_data, JA_PROTO_TAG_RESULT,
                           json_object_new_int(job->result));
                    
    json_object_object_add(jp_data, JA_PROTO_TAG_CUR_UNIQUE_ID,
                            json_object_new_string(job->cur_unique_id));
    json_object_object_add(jp_data, JA_PROTO_TAG_PRE_UNIQUE_ID,
                            json_object_new_string(job->pre_unique_id));
    
    if (job->result == JA_JOBRESULT_SUCCEED) {
        json_object_object_add(jp_data, JA_PROTO_TAG_JOBSTDOUT,
                               json_object_new_string(job->std_out));
        json_object_object_add(jp_data, JA_PROTO_TAG_JOBSTDERR,
                               json_object_new_string(job->std_err));
        json_object_object_add(jp_data, JA_PROTO_TAG_RET,
                               json_object_new_int(job->return_code));
        json_object_object_add(jp_data, JA_PROTO_TAG_SIGNAL,
                               json_object_new_int(job->signal));

        json_object_object_add(jp_data, JA_PROTO_VALUE_RUNUSR,
                               json_object_new_string(job->run_user));
        json_object_object_add(jp_data, JA_PROTO_VALUE_RUNUSRPWD,
                               json_object_new_string(job->run_user_password));

	} else {
        json_object_object_add(jp_data, JA_PROTO_TAG_MESSAGE,
                               json_object_new_string(job->message));
    }
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
int ja_telegram_to_ip_response(ja_job_object * job, json_object * json) {
    json_object *jp_data;
    const char *__function_name = "ja_telegram_to_ip_response";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || json == NULL) {
        zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), Job or Json is NULL", __function_name);
        return FAIL;   
    }
        
    jp_data = json_object_object_get(json, JA_PROTO_TAG_DATA);
    if (jp_data == NULL) {
        zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), Job or Json is NULL", __function_name);
        return FAIL;
    }

    json_object_object_add(jp_data, JA_PROTO_TAG_RET,  json_object_new_int(job->return_code));
    return SUCCEED;
}