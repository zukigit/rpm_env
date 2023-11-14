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

#include "jatelegram.h"
#include "jatrapjobresult.h"
#include "jatrapjobnetrun.h"
#include "jatrapjobrelease.h"
#include "jatrapjoblogput.h"
#include "jahost.h"
#include "jatrapauth.h"
#include "jatrapkind.h"

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
int jatrap_kind(zbx_sock_t * sock, ja_telegram_object * obj)
{
    int ret;
    json_object *jp;
    char *request;
    char *kind, *err;
    int version;
    const char *__function_name = "jatrap_kind";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    ret = FAIL;
    err = NULL;
    if (ja_telegram_check(obj) == FAIL)
        return FAIL;

    request = (char *) json_object_to_json_string(obj->request);
    jp = json_object_object_get(obj->request, JA_PROTO_TAG_KIND);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_KIND, request);
        goto error;
    }
    kind = (char *) json_object_get_string(jp);
    jp = json_object_object_get(obj->request, JA_PROTO_TAG_VERSION);
    if (jp == NULL) {
        err =
            zbx_dsprintf(NULL,
                         "can not find the tag '%s' from the request telegram: %s",
                         JA_PROTO_TAG_VERSION, request);
        goto error;
    }
    version = json_object_get_int(jp);
    if (strcmp(kind, JA_PROTO_VALUE_JOBRESULT) == 0) {
        if (jatrap_auth_host(sock, obj) == SUCCEED)
            jatrap_jobresult(obj);
    } else if (strcmp(kind, "job.release") == 0) {
        jatrap_jobrelease(obj);
    } else if (strcmp(kind, "job.logput") == 0) {
        jatrap_joblogput(obj);
    } else {
        return FAIL;
    }

    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name);
        ja_telegram_seterr(obj, err);
    }
    if (err != NULL)
        zbx_free(err);
    return ret;
}
