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
#include "threads.h"

#include "jacommon.h"
#include "jajobobject.h"
#include "jalog.h"
#include "javalue.h"
#include "jatcp.h"
#include "jaconnect.h"
#include "jastatus.h"
#include "jathreads.h"
#include "jahost.h"
#include "jaruniconjob.h"

static int host_lock_flag = 0;

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
int jarun_icon_job_getenv(zbx_uint64_t inner_job_id, ja_job_object * job)
{
    json_object  *jp_env;
    DB_RESULT    result;
    DB_ROW       row;
    char         *p;
    char         value[JA_STD_OUT_LEN];
    const char   *__function_name = "jarun_icon_job_getenv";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    jp_env = json_object_new_object();
    result = DBselect("select value_name from ja_run_value_jobcon_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    while (NULL != (row = DBfetch(result))) {
        zbx_snprintf(value, sizeof(value), "");
        ja_get_value_before(inner_job_id, row[0], value);
        json_object_object_add(jp_env, row[0], json_object_new_string(value));
    }
    DBfree_result(result);

    result = DBselect("select value_name, value from ja_run_value_job_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    while (NULL != (row = DBfetch(result))) {
        zbx_snprintf(value, sizeof(value), "");
        p = row[1];
        if (strlen(p) > 1) {
            if (*p == '$' && *(p + 1) != '$') {
                ja_get_value_before(inner_job_id, p + 1, value);
            } else {
                if (*p == '$' && *(p + 1) == '$') {
                    zbx_strlcpy(value, (row[1] + 1), sizeof(value));
                } else {
                    zbx_strlcpy(value, row[1], sizeof(value));
                }
            }
        } else {
            zbx_strlcpy(value, row[1], sizeof(value));
        }
        json_object_object_add(jp_env, row[0], json_object_new_string(value));
    }
    DBfree_result(result);

    zbx_snprintf(job->env, sizeof(job->env), "%s", json_object_to_json_string(jp_env));
    json_object_put(jp_env);
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
int jarun_icon_job(zbx_uint64_t inner_job_id, int flag)
{
    const int TIMEOUT_LIMIT = 600;
    int ret,tcp_ret;
    DB_RESULT result;
    DB_ROW row;
    ja_job_object job, job_res;
    zbx_uint64_t inner_jobnet_id;
    int host_flag;
    int con_err;
    int command_type;
    char host_name[128];
    char host[128];
    int timeout_cnt = 0;
    int set_runerr_cnt = 0;
    pid_t pid;
    zbx_sock_t sock;
    zbx_uint64_t hostid;
    int port;
    char host_ip[128];
    const char *__function_name = "jarun_icon_job";
    int job_status;
    int isRecvFail = 0;
    int retry_count = 0;

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    ja_job_object_init(&job);
    result = DBselect
        ("select inner_jobnet_id, host_flag, command_type, host_name from ja_run_icon_job_table where inner_job_id = "
         ZBX_FS_UI64, inner_job_id);
    row = DBfetch(result);
    if (row != NULL) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        host_flag = atoi(row[1]);
        command_type = atoi(row[2]);
        zbx_snprintf(host_name, sizeof(host_name), "%s", row[3]);
    } else {
        ja_log("JARUNICONJOB200017", inner_jobnet_id, NULL, inner_job_id, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    DBfree_result(result);

    if (ja_host_getname(inner_job_id, host_flag, host_name, host) == FAIL) {
        ja_log("JARUNICONJOB200028", inner_jobnet_id, NULL, inner_job_id, host_name, host_flag, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    if (flag == JA_AGENT_METHOD_NORMAL || flag == JA_AGENT_METHOD_TEST) {
    	if (ja_host_lockinfo(host) == SUCCEED) {
    		zabbix_log(LOG_LEVEL_DEBUG,
    			"In %s() host '%s' is locked. inner_job_id: "
    			ZBX_FS_UI64, __function_name, host, inner_job_id);
    		if(host_lock_flag == 0 ){
    			zabbix_log(LOG_LEVEL_WARNING,
    					   "In %s() host '%s' is locked. inner_job_id: "
    					   ZBX_FS_UI64, __function_name, host, inner_job_id);
    			host_lock_flag = 1;
    		}
    	    return FAIL;
    	}
    	host_lock_flag = 0;
    }

    zbx_snprintf(job.kind, sizeof(job.kind), "%s", JA_PROTO_VALUE_JOBRUN);
    job.version = JA_PROTO_TELE_VERSION;
    job.jobid = inner_job_id;
    zbx_snprintf(job.serverid, sizeof(job.serverid), "%s", serverid);
    zbx_snprintf(job.hostname, sizeof(job.hostname), "%s", host);
    job.method = flag;

    if (flag == JA_AGENT_METHOD_ABORT || flag == JA_AGENT_METHOD_KILL) {
        if (ja_set_status_job(inner_job_id, JA_JOB_STATUS_ABORT, -1, -1) ==
            FAIL)
            return FAIL;
    }

    if (flag == JA_AGENT_METHOD_ABORT) {
        command_type = 2;
        result =
            DBselect("select inner_job_id_fs_link from ja_run_job_table"
                     " where inner_job_id = " ZBX_FS_UI64, inner_job_id);
        row = DBfetch(result);
        if (row == NULL) {
            ja_log("JARUNICONJOB200024", inner_jobnet_id, NULL, inner_job_id, inner_job_id);
            DBfree_result(result);
            return ja_set_runerr(inner_job_id, 2);
        }
        ZBX_STR2UINT64(job.jobid, row[0]);
        DBfree_result(result);
    }

    if (job.method != JA_AGENT_METHOD_KILL) {
        zbx_snprintf(job.type, sizeof(job.type), "%s",
                     JA_PROTO_VALUE_COMMAND);
        result =
            DBselect("select command from ja_run_job_command_table"
                     " where inner_job_id = " ZBX_FS_UI64
                     " and command_cls = %d", inner_job_id, command_type);
        row = DBfetch(result);
        if (row == NULL) {
            ja_log("JARUNICONJOB200024", inner_jobnet_id, NULL, inner_job_id, inner_job_id);
            DBfree_result(result);
            return ja_set_runerr(inner_job_id, 2);
        }
        zbx_snprintf(job.script, sizeof(job.script), "%s", row[0]);
        DBfree_result(result);

        result =
            DBselect("select run_user,run_user_password from ja_run_job_table"
                     " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

        row = DBfetch(result);
        if (row == NULL) {
            ja_log("JARUNICONJOB200024", inner_jobnet_id, NULL, inner_job_id, inner_job_id);
            DBfree_result(result);
            return ja_set_runerr(inner_job_id, 2);
        }

        zbx_snprintf(job.run_user, sizeof(job.run_user), "%s", row[0]);
        zbx_snprintf(job.run_user_password, sizeof(job.run_user_password), "%s", row[1]);

        DBfree_result(result);

    	jarun_icon_job_getenv(inner_job_id, &job);
    }
    
    hostid = ja_host_getip(host, host_ip, inner_job_id, NULL, JA_TXN_ON);
    if (hostid == 0) {
        return ja_set_runerr(inner_job_id, 2);
    }

    port = ja_host_getport(hostid, 0);

    pid = ja_fork();
    if (pid == -1) {
        ja_log("JARUNICONJOB200027", inner_jobnet_id, NULL, inner_job_id, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    } else if (pid != 0) {
        waitpid(pid, NULL, WNOHANG);
        return SUCCEED;
    }
    
    //DBconnect(ZBX_DB_CONNECT_NORMAL);
    ret = FAIL;
    con_err = SUCCEED;
    ja_job_object_init(&job_res);

  connector:
    retry_count++;
    if (ja_connect(&sock, host_ip,port, inner_job_id) == FAIL) {
        con_err = FAIL;
        goto error;
    }
    if (ja_tcp_send_to(&sock, &job, CONFIG_TIMEOUT) == FAIL) {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        ja_log("JARUNICONJOB200012", inner_jobnet_id, NULL, inner_job_id, inner_job_id, "tcp_send_to() is failed");
        DBclose();
        goto error;
    }
    job_res.result = JA_RESPONSE_FAIL;
    ret=ja_tcp_recv_to(&sock, &job_res, CONFIG_TIMEOUT);
    if (ret== FAIL) {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        ja_log("JARUNICONJOB200029", inner_jobnet_id, NULL, inner_job_id, inner_job_id, job_res.message);
        DBclose();
        isRecvFail = 1;
        goto error;
    }

    if (ret== RECEIVED_NULL) {
        if(retry_count <= CONFIG_SEND_RETRY){
            zabbix_log(LOG_LEVEL_ERR,"In %s() tcp receive failed.retry[%d] , ",__function_name,retry_count);
            zbx_tcp_close(&sock);
            sleep(1);           
            goto connector;
        }
        DBconnect(ZBX_DB_CONNECT_ONCE);
        ja_log("JARUNICONJOB200029", inner_jobnet_id, NULL, inner_job_id, inner_job_id, job_res.message);
        DBclose();
        isRecvFail = 1;
        goto error;
    }
    if (job_res.result != JA_RESPONSE_SUCCEED)
        goto error;
    ret = SUCCEED;

  error:
   if(con_err == SUCCEED){
        zbx_tcp_close(&sock);
    } 
    if (ret == SUCCEED) {
        exit(0);
    }
    else
    {
        DBconnect(ZBX_DB_CONNECT_ONCE);

        set_runerr_cnt = 0;
        int set_runerr = FAIL;
        while (set_runerr == FAIL)
        {
            result = DBselect("select status from ja_run_job_table where inner_job_id = " ZBX_FS_UI64, inner_job_id);
            row = DBfetch(result);
            if (row != NULL)
            {
                job_status = atoi(row[0]);

                if (job_status >= 3)
                {
                    zabbix_log(LOG_LEVEL_INFORMATION, "Can not get the response from host but the status is already changed");
                    set_runerr = SUCCEED;
                }

                if (job_status < 3)
                {
                    set_runerr = ja_set_runerr(inner_job_id, 2);
                }
            }
            else
            {
                set_runerr = FAIL;
                zabbix_log(LOG_LEVEL_WARNING, "In%s(), Cannot get status from database. jobid :" ZBX_FS_UI64, __function_name, inner_job_id);
            }
            DBfree_result(result);

            if (set_runerr_cnt <= TIMEOUT_LIMIT && set_runerr == FAIL)
            {
                zabbix_log(LOG_LEVEL_WARNING, "In %s(), Error setting job to runerror state. Retry transaction. inner jobnet id :" ZBX_FS_UI64 "inner job id :" ZBX_FS_UI64, __function_name, inner_jobnet_id, inner_job_id);
                set_runerr_cnt += 5;
                sleep(5);
                continue;
            }
            else
            {
                break;
            }
        }
        if (set_runerr_cnt > TIMEOUT_LIMIT)
        {
            zabbix_log(LOG_LEVEL_ERR, "In %s(), Cannot set job to  runerror state. Timed out. inner jobnet id :" ZBX_FS_UI64 "inner job id :" ZBX_FS_UI64, __function_name, inner_jobnet_id, inner_job_id);
        }
        DBclose();
        exit(1);
    }
}
