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
#include "comms.h"
#include "db.h"
#include "log.h"

#include "jacommon.h"
#include "jalog.h"
#include "javalue.h"
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
int ja_host_getname(const zbx_uint64_t inner_job_id, const int host_flag,
                    const char *host_name, char *host)
{
    const char *__function_name = "ja_host_getname";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host_flag: %d, host_name: %s, ",
               __function_name, host_flag, host_name);

    if (host_flag == 0) {
        zbx_snprintf(host, JA_MAX_STRING_LEN, "%s", host_name);
        return SUCCEED;
    }

    return ja_get_value_before(inner_job_id, host_name, host);
}
/******************************************************************************
 *                                                                            *
 * Function: ja_host_getip_ipchange                                           *
 *                                                                            *
 * Purpose: get the ip address and port of the host                           *
 *          Modified version of ja_host_getip for ja_send_ipchange_request()  *
 *                                                                            *
 * Parameters: host         (in)  - host name                                 *
 *             host_ip      (out) - ip address                                *
 *             port         (out) - port                                      *
 *             txn          (in)  - transaction instruction                   *
 *                                                                            *
 * Return value: return the host id. failure is zero                          *
 *                                                                            *
 * Comments: value is not set when host_ip or port is NULL                    *
 *                                                                            *
 ******************************************************************************/
zbx_uint64_t ja_host_getip_ipchange(const char *host, char *host_ip, int *port, int txn)
{
    DB_RESULT result;
    DB_RESULT result2;
    DB_ROW row;
    DB_ROW row2;
    char *host_esc;
    zbx_uint64_t hostid;
    const char *__function_name = "ja_host_getip_ipchange";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);
    hostid = 0;
    result = NULL;
    host_esc = DBdyn_escape_string(host);
    switch (CONFIG_ZABBIX_VERSION) {
    case 1:
        // for zabbix 1.8
        result = DBselect("select hostid, useip, dns, ip, status, port from hosts where host = '%s'",
                          host_esc);
        break;
    case 2:
    case 3:
        // for zabbix 2.0 or 2.2
        result = DBselect(" select i.hostid, i.useip, i.dns, i.ip, h.status, i.port from hosts h, interface i"
                          " where h.hostid = i.hostid and i.main = 1 and i.type = 1 and h.host = '%s'",
                          host_esc);
        break;
    default:
        ja_log("JAHOST200001", 0, NULL, 0, __function_name, CONFIG_ZABBIX_VERSION);
        goto error;
    }

    if (result == NULL) {
        if (txn == JA_TXN_ON) {
            DBrollback();
        }
        ja_log("JAHOST200007", 0, NULL, 0, __function_name, CONFIG_ZABBIX_VERSION);
        goto error;
    }

    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAHOST200002", 0, NULL, 0, __function_name, host, 0);
        goto error;
    }

    ZBX_STR2UINT64(hostid, row[0]);
    if (host_ip != NULL) {
        if (atoi(row[1]) == 0) {
            // use dns
            zbx_snprintf(host_ip, strlen(row[2]) + 1, "%s", row[2]);
        } else {
            // use ip
            zbx_snprintf(host_ip, strlen(row[3]) + 1, "%s", row[3]);
        }
    }

    /* get port */
    if (port != NULL) {
        *port = atoi(row[5]);
    }

    DBfree_result(result);
    result = NULL;

  error:
    zbx_free(host_esc);
    if (result != NULL)
        DBfree_result(result);
    return hostid;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_host_getip                                                    *
 *                                                                            *
 * Purpose: get the ip address and port of the host                           *
 *                                                                            *
 * Parameters: host         (in)  - host name                                 *
 *             host_ip      (out) - ip address                                *
 *             inner_job_id (in)  - inner job id                              *
 *             port         (out) - port                                      *
 *             txn          (in)  - transaction instruction                   *
 *                                                                            *
 * Return value: return the host id. failure is zero                          *
 *                                                                            *
 * Comments: value is not set when host_ip or port is NULL                    *
 *                                                                            *
 ******************************************************************************/
zbx_uint64_t ja_host_getip(const char *host, char *host_ip, const zbx_uint64_t inner_job_id, int *port, int txn)
{
    DB_RESULT result;
    DB_RESULT result2;
    DB_ROW row;
    DB_ROW row2;
    char *host_esc;
    zbx_uint64_t hostid;
    const char *__function_name = "ja_host_getip";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);
    
    hostid = 0;
    result = NULL;
    host_esc = DBdyn_escape_string(host);
    switch (CONFIG_ZABBIX_VERSION) {
    case 1:
        // for zabbix 1.8
        result = DBselect("select hostid, useip, dns, ip, status, port from hosts where host = '%s'",
                          host_esc);
        break;
    case 2:
    case 3:
        // for zabbix 2.0 or 2.2
        result = DBselect(" select i.hostid, i.useip, i.dns, i.ip, h.status, i.port from hosts h, interface i"
                          " where h.hostid = i.hostid and i.main = 1 and i.type = 1 and h.host = '%s'",
                          host_esc);
        break;
    default:
        ja_log("JAHOST200001", 0, NULL, inner_job_id, __function_name, CONFIG_ZABBIX_VERSION);
        goto error;
    }

    if (result == NULL) {
        if (txn == JA_TXN_ON) {
            DBrollback();
        }
        ja_log("JAHOST200007", 0, NULL, inner_job_id, __function_name, CONFIG_ZABBIX_VERSION);
        goto error;
    }

    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAHOST200002", 0, NULL, inner_job_id, __function_name, host, inner_job_id);
        goto error;
    }

    /* get the force flag and job id */
    result2 = DBselect("select force_flag, job_id from ja_run_job_table where inner_job_id = " ZBX_FS_UI64,
                       inner_job_id);
    if (NULL == (row2 = DBfetch(result2))) {
        ja_log("JAHOST200004", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result2);
        goto error;
    }

    if (atoi(row[4]) != HOST_STATUS_MONITORED && atoi(row2[0]) == JA_JOB_FORCE_FLAG_OFF) {
        ja_log("JAHOST200003", 0, NULL, inner_job_id, __function_name, host, inner_job_id, row2[1]);
        DBfree_result(result2);
        goto error;
    }

    DBfree_result(result2);

    ZBX_STR2UINT64(hostid, row[0]);
    if (host_ip != NULL) {
        if (atoi(row[1]) == 0) {
            // use dns
            zbx_snprintf(host_ip, strlen(row[2]) + 1, "%s", row[2]);
        } else {
            // use ip
            zbx_snprintf(host_ip, strlen(row[3]) + 1, "%s", row[3]);
        }
    }

    /* get port */
    if (port != NULL) {
        *port = atoi(row[5]);
    }

    DBfree_result(result);
    result = NULL;

  error:
    zbx_free(host_esc);
    if (result != NULL)
        DBfree_result(result);
    return hostid;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_host_getport                                                  *
 *                                                                            *
 * Purpose: get the port that is specified in the host macro                  *
 *                                                                            *
 * Parameters: hostid     (in)  - host id                                     *
 *             macro_flag (in)  - macro flag                                  *
 *                                0: job agent listen port                    *
 *                                1: ssh connect port                         *
 *                                                                            *
 *                                                                            *
 * Return value: return the port number                                       *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_host_getport(zbx_uint64_t hostid, int macro_flag)
{
    DB_RESULT result;
    DB_ROW row;
    int port;
    char *macro_name;
    const char *__function_name = "ja_host_getport";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() hostid: " ZBX_FS_UI64, __function_name, hostid);

    if (macro_flag == 0) {
        port = CONFIG_AGENT_LISTEN_PORT;
        macro_name = JA_AGENT_PORT;
    }
    else {
        port = JA_SSH_CONNECT_PORT;
        macro_name = JA_SSH_PORT;
    }

    result =  DBselect("select value from hostmacro where hostid = " ZBX_FS_UI64
                       " and macro = '%s'", hostid, macro_name);

    row = DBfetch(result);
    if (row != NULL) {
        port = atoi(row[0]);
    }
    DBfree_result(result);

    return port;
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
int ja_host_auth(zbx_sock_t * sock, const char *host, const zbx_uint64_t inner_job_id)
{
    char host_ip[128];
    const char *__function_name = "ja_host_auth";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);

    if (ja_host_getip(host, host_ip, inner_job_id, NULL, JA_TXN_ON) == 0) {
        return FAIL;
    }

    if (zbx_tcp_check_security(sock, host_ip, 0) == FAIL) {
        //ja_log("JAHOST200006", 0, NULL, inner_job_id, __function_name, host, host_ip, inner_job_id);
        //return FAIL;
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
int ja_host_lockinfo(const char *host)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *host_esc;
    const char *__function_name = "ja_host_lockinfo";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);

    ret = FAIL;
    host_esc = DBdyn_escape_string(host);
    result =
        DBselect
        ("select lock_host_name from ja_host_lock_table where lock_host_name = '%s'",
         host_esc);
    row = DBfetch(result);
    if (row != NULL)
        ret = SUCCEED;

    zbx_free(host_esc);
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
int ja_host_lock(const char *host, const zbx_uint64_t inner_job_id)
{
    int db_ret;
    char *host_esc;
    const char *__function_name = "ja_host_lock";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);

    if (ja_host_getip(host, NULL, inner_job_id, NULL, JA_TXN_ON) == 0)
        return FAIL;

    DBfree_result(DBselect
                  ("select lock_host_name from ja_host_lock_table where lock_host_name = 'HOST_LOCK_RECORD' for update"));

    host_esc = DBdyn_escape_string(host);
    db_ret =
        DBexecute
        ("insert into ja_host_lock_table (lock_host_name) values ('%s')",
         host_esc);
    zbx_free(host_esc);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAHOST200005", 0, NULL, inner_job_id, __function_name, host, inner_job_id);
        return FAIL;
    } else {
        return SUCCEED;
    }
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
int ja_host_unlock(const char *host)
{
    int db_ret;
    char *host_esc;
    const char *__function_name = "ja_host_unlock";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);

    DBfree_result(DBselect
                  ("select lock_host_name from ja_host_lock_table where lock_host_name = 'HOST_LOCK_RECORD' for update"));

    host_esc = DBdyn_escape_string(host);
    db_ret =
        DBexecute
        ("delete from ja_host_lock_table where lock_host_name = '%s'",
         host_esc);
    zbx_free(host_esc);

    if (db_ret < ZBX_DB_OK)
        return FAIL;
    else
        return SUCCEED;
}
