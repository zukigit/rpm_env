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
#include "db.h"

#include "jatelegram.h"
#include "jauser.h"
#include "jahost.h"

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
int jatrap_auth_host(zbx_sock_t * sock, ja_telegram_object * obj)
{
    int ret;
    zbx_uint64_t inner_job_id;
    json_object *jp_data, *jp;
    int host_delete_cnt;
    char *request;
    char *hostname, *err;
    DB_RESULT result;
    DB_ROW row;



    const char *__function_name = "jatrap_auth_host";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (ja_telegram_check(obj) == FAIL)
        return FAIL;

    ret = FAIL;
    err = NULL;
    request = (char *) json_object_to_json_string(obj->request);
    jp_data = json_object_object_get(obj->request, JA_PROTO_TAG_DATA);
    jp = json_object_object_get(jp_data, JA_PROTO_TAG_HOSTNAME);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_HOSTNAME, request);
        goto error;
    }
    hostname = (char *) json_object_get_string(jp);

    jp = json_object_object_get(jp_data, JA_PROTO_TAG_JOBID);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_JOBID, request);
        goto error;
    }
    ZBX_STR2UINT64(inner_job_id, (char *) json_object_get_string(jp));

    if (ja_host_auth(sock, hostname, inner_job_id) == FAIL) {
    	err = zbx_dsprintf(NULL, "host '%s' is not authenticated", hostname);
        result =
               DBselect ("select count(lock_host_name) from ja_host_lock_table where lock_host_name = '%s'",
            		   hostname);
        row = DBfetch(result);
        if(row != NULL){
        	host_delete_cnt = atoi(row[0]);
        	if(host_delete_cnt == 1 && ja_host_unlock(hostname) == SUCCEED){
					zabbix_log(LOG_LEVEL_WARNING, "host '%s' is  released", hostname);
        	}else{
        		zabbix_log(LOG_LEVEL_WARNING, "host '%s' does not exist. cnt[%d]", hostname, host_delete_cnt);
        		goto error;
        	}
        }else{
        	goto error;
        }
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
zbx_uint64_t jatrap_auth_user(ja_telegram_object * obj)
{
    zbx_uint64_t userid;
    json_object *jp_data, *jp;
    char *username, *password, *err;
    const char *__function_name = "jatrap_auth_user";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    userid = 0;
    err = NULL;
    if (ja_telegram_check(obj) == FAIL)
        return userid;

    jp_data = json_object_object_get(obj->request, JA_PROTO_TAG_DATA);
    jp = json_object_object_get(jp_data, JA_PROTO_TAG_USERNAME);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_USERNAME,
                         json_object_to_json_string(obj->request));
        goto error;
    }
    username = (char *) json_object_get_string(jp);
    jp = json_object_object_get(jp_data, JA_PROTO_TAG_PASSWORD);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_PASSWORD,
                         json_object_to_json_string(obj->request));
        goto error;
    }
    password = (char *) json_object_get_string(jp);

    userid = ja_user_auth(username, password);
    if (userid == 0) {
        err =
            zbx_dsprintf(NULL, "user authentication error, username: %s",
                         username);
        goto error;
    }
    if (ja_user_status(userid) != 0) {
        err = zbx_dsprintf(NULL, "invalid user, username: %s", username);
        userid = 0;
    }
  error:
    if (userid == 0) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   err);
        ja_telegram_seterr(obj, err);
    }
    if (err != NULL)
        zbx_free(err);
    return userid;
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
int jatrap_auth_jobnet(zbx_uint64_t userid, zbx_uint64_t inner_jobnet_id)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    int user_type, public_flag, user_cmp;
    const char *__function_name = "jatrap_auth_jobnet";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (userid == 0)
        return FAIL;

    user_type = ja_user_type(userid);

    ret = SUCCEED;
    public_flag = 0;
    user_cmp = 0;

    result =
        DBselect
        ("select user_name, public_flag from ja_run_jobnet_table where inner_jobnet_id = "
         ZBX_FS_UI64, inner_jobnet_id);
    row = DBfetch(result);
    if (row == NULL) {
        ret = FAIL;
    } else {
        public_flag = atoi(row[1]);
        if (ja_user_groups(ja_user_id(row[0]), userid) > 0)
            user_cmp = 1;
    }
    DBfree_result(result);
    if (ret == FAIL)
        return FAIL;
    if (user_type == USER_TYPE_SUPER_ADMIN || public_flag == 1
        || user_cmp == 1)
        return SUCCEED;
    return FAIL;
}
