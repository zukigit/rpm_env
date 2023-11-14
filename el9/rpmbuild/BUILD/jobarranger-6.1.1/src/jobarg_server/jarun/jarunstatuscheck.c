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
#include "jarunstatuscheck.h"

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
int jarun_status_check(const char *host_name, int *joblist, int count_job,int* reboot_job_list)
{
    int ret,jacon_ret;
    ja_job_object job, job_res;
    int con_err;
    zbx_sock_t sock;
    zbx_uint64_t hostid;
    int port;
    zbx_uint64_t inner_job_id,tmp_inner_job_id;
    char host_ip[128];
    int set_runerr = FAIL;
    int err_count = 0;

    DB_ROW row;
    DB_RESULT result;
    int job_status;

    const char *__function_name = "jarun_status_check";
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() hostname: %s, count job : %d ",
               __function_name, host_name, count_job);

    ja_job_object_init(&job);

    job.size_of_host_running_job = count_job;
    job.host_running_job  = (zbx_uint64_t*)malloc((job.size_of_host_running_job) * sizeof(zbx_uint64_t));

    zbx_snprintf(job.kind, sizeof(job.kind), "%s", JA_PROTO_VALUE_CHKJOB);
    job.version = JA_PROTO_TELE_VERSION;
    zbx_snprintf(job.serverid, sizeof(job.serverid), "%s", serverid);
    zbx_snprintf(job.hostname, sizeof(job.hostname), "%s", host_name);
	int i=0;
    inner_job_id = joblist[i];
    for (i = 0; i < count_job; i++) {
        job.host_running_job[i] = joblist[i];
    }

    hostid = ja_host_getip(host_name, host_ip, inner_job_id, NULL, JA_TXN_ON);
    if (hostid == 0) {
        ret = FAIL;
        goto error;
    }

    port = ja_host_getport(hostid, 0);

    ret = FAIL;
    con_err = SUCCEED;
    ja_job_object_init(&job_res);
    jacon_ret = ja_connect_status_check(&sock, host_ip,port, inner_job_id);
    if ( jacon_ret== FAIL) {
        zabbix_log(LOG_LEVEL_ERR,"In %s() Cannot connect to Agent.",__function_name);
        con_err = FAIL;
        goto error;
    }else if(jacon_ret!=SUCCEED){
        zabbix_log(LOG_LEVEL_WARNING,"In %s() Connected to agent[%s] with timeout count [%d]. Check again next time.",__function_name,host_ip,jacon_ret);
        ret = SUCCEED;
        free(job.host_running_job);
        job.host_running_job  = (zbx_uint64_t*)malloc(sizeof(zbx_uint64_t));
        job.size_of_host_running_job=0;
    }

    if (ja_tcp_send_to(&sock, &job, CONFIG_TIMEOUT) == FAIL) {
        zabbix_log(LOG_LEVEL_ERR,"In %s() Data send error.",__function_name);
        goto error;
    }
    job_res.result = JA_RESPONSE_FAIL;
    ret=ja_tcp_recv_to(&sock, &job_res, CONFIG_TIMEOUT);
    if ( ret== FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "Can not get the response from host.");
        goto error;
    }

    ret = SUCCEED;
  error:
    if(con_err == SUCCEED){
        zbx_tcp_close(&sock);
    }
    if(ret == FAIL){
		int i=0;
        for(i=0;i<count_job;i++){
            int ii = 0;
            int isReboot = FAIL;
            while(reboot_job_list[ii]!=-1){
                if(reboot_job_list[ii] == joblist[i]){
                    isReboot = SUCCEED;
                    break;
                }
                ii++;
            }
            if(isReboot == SUCCEED){
                continue;
            }
            tmp_inner_job_id = joblist[i];
            set_runerr = FAIL;
            err_count = 0;
            
            DBconnect(ZBX_DB_CONNECT_ONCE);

            result = DBselect("select status from ja_run_job_table where inner_job_id = " ZBX_FS_UI64, tmp_inner_job_id);
            row = DBfetch(result);
            if(row!=NULL){
                job_status = atoi(row[0]);

                if(job_status >= 3 && job_status < 6) {
                    zabbix_log(LOG_LEVEL_INFORMATION, "Can not get the response from host but the status is already changed. inner jobnet id :"ZBX_FS_UI64", status : %d",tmp_inner_job_id,job_status);
                    set_runerr=SUCCEED;
                }

                if(job_status < 3 || job_status == 6) {
                    ja_log("JACONNECT300001", 0, NULL, tmp_inner_job_id, __function_name, zbx_tcp_strerror(),host_ip, port, CONFIG_SOURCE_IP, CONFIG_TIMEOUT);
                    while(set_runerr == FAIL){
                        set_runerr = ja_set_runerr(tmp_inner_job_id, 2);
                        if(set_runerr == SUCCEED || err_count>2){
                            break;
                        }
                        err_count++;
                    }
                    if(set_runerr == FAIL){
                        zabbix_log(LOG_LEVEL_ERR, "In %s(), cannot set runerr status. inner_job_id :"ZBX_FS_UI64,__function_name,tmp_inner_job_id);
                    }
                }
            }else{
                set_runerr = FAIL;
                zabbix_log(LOG_LEVEL_WARNING,"In%s(), Cannot get status from database. jobid :"ZBX_FS_UI64,__function_name,inner_job_id);
            }
            DBfree_result(result);

            DBclose();
        }
    }
    free(job.host_running_job);
    return ret;
}
