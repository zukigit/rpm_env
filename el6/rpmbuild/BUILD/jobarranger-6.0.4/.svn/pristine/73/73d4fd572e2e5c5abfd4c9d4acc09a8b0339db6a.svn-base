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
#include "javalue.h"
#include "jastatus.h"
#include "jaflow.h"
#include "jathreads.h"
#include "jajobobject.h"
#include "jahost.h"
#include "jatcp.h"
#include "jaconnect.h"
#include "jarunagent.h"

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
int jarun_agent(ja_job_object * job, const char *host_name,
                const int host_flag, const zbx_uint64_t inner_job_id)
{
    int ret;
    char host[128];
    zbx_sock_t sock;
    pid_t pid;
    ja_job_object job_res;
    zbx_uint64_t hostid;
    int port;
    char host_ip[128];
    const char *__function_name = "jarun_agent";

    if (job == NULL)
        return FAIL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() job id: " ZBX_FS_UI64,
               __function_name, job->jobid);

    if (ja_host_getname(job->jobid, host_flag, host_name, host) == FAIL) {
        ja_log("JARUNAGENT200001", 0, NULL, inner_job_id,
               __function_name, host_name, host_flag, inner_job_id);
        return ja_set_runerr(job->jobid, 2);
    }

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

    zbx_snprintf(job->kind, sizeof(job->kind), "%s",
                 JA_PROTO_VALUE_JOBRUN);
    job->version = JA_PROTO_TELE_VERSION;
    zbx_snprintf(job->serverid, sizeof(job->serverid), "%s", serverid);
    zbx_snprintf(job->hostname, sizeof(job->hostname), "%s", host);

    if (job->method == JA_AGENT_METHOD_KILL) {
        if (ja_set_status_job(job->jobid, JA_JOB_STATUS_ABORT, -1, -1) ==
            FAIL)
            return FAIL;
    }

    
    hostid = ja_host_getip(host, host_ip, inner_job_id, NULL, JA_TXN_ON);
    if (hostid == 0) {
        return ja_set_runerr(job->jobid, 2);
    }

    port = ja_host_getport(hostid, 0);

    pid = ja_fork();
    if (pid == -1) {
        ja_log("JARUNAGENT200002", 0, NULL, inner_job_id,
               __function_name, inner_job_id);
        if (job->method == JA_AGENT_METHOD_KILL)
            return FAIL;
        else
            return ja_set_runerr(job->jobid, 2);
    } else if (pid != 0) {
        waitpid(pid, NULL, WNOHANG);
        return SUCCEED;
    }

    ret = FAIL;
    ja_job_object_init(&job_res);
    //DBconnect(ZBX_DB_CONNECT_ONCE);
    if (ja_connect(&sock, host_ip, port, inner_job_id) == FAIL) {
        ja_set_runerr(job->jobid, 2);
        //DBclose();
        exit(1);
    }
    //DBclose();

    zbx_snprintf(job_res.message, sizeof(job_res.message),
                 "Can not get the response from host: %s", job->hostname);
    if (ja_tcp_send_to(&sock, job, CONFIG_TIMEOUT) == FAIL)
        goto error;
    job_res.result = JA_RESPONSE_FAIL;
    if (ja_tcp_recv_to(&sock, &job_res, CONFIG_TIMEOUT) == FAIL)
        goto error;
    if (job_res.result != JA_RESPONSE_SUCCEED)
        goto error;

    ret = SUCCEED;

  error:
    zbx_tcp_close(&sock);
    if (ret == SUCCEED) {
        exit(0);
    } else {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        ja_log("JARUNAGENT200004", 0, NULL, inner_job_id,
               __function_name, inner_job_id, job_res.message);
        if (job->method != JA_AGENT_METHOD_KILL)
            ja_set_runerr(job->jobid, 2);
        DBclose();
        exit(1);
    }
}
