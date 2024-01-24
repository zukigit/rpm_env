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
#include "jadb.h"
#include "jalog.h"
#include "jahost.h"
#include "jaconnect.h"

/******************************************************************************
 *                                                                            *
 * Function: ja_connect                                                       *
 *                                                                            *
 * Purpose: connect to the job agent host                                     *
 *                                                                            *
 * Parameters: s            (in) - socket identification information          *
 *             host         (in) - host name                                  *
 *             inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_connect_ipchange(zbx_sock_t * s, char * host_ip, int port){
    int ret = SUCCEED;
    const int TIMEOUT_LIMIT = 120;
    const int SLEEP_TIMEOUT = 10;
    // const int TIMEOUT_LIMIT = 600;
    int timeout_cnt = 0,time = 0;
    int job_status;
    const char *__function_name = "ja_connect_ipchange";

    //zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);


    zabbix_log(LOG_LEVEL_DEBUG, "In %s() connect the host. source_ip: %s, host_ip: %s, port: %d, timeout: %d",
               __function_name, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    //repeat connection until timeout min.
    // Effected : jaruniconreboot: jarun_icon_reboot()
    //          : jaruiconjob    : jarun_icon_job()
    //          : jaruniconfcopy : jarun_icon_fcopy()
    //          : jarunagent     : jarun_agent()
    //ret = zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    while( zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT) == FAIL){
        if(time <= TIMEOUT_LIMIT){
            zabbix_log(LOG_LEVEL_WARNING,"In %s(), Error connecting to agent. Retry connection for host :%s",__function_name,host_ip);
            time += SLEEP_TIMEOUT;
            timeout_cnt++;
            sleep(SLEEP_TIMEOUT);
            continue; 
        }else{
            ret = FAIL;
            time = 0;
            break;
        }
        
    }

    return ret;

}
int ja_connect(zbx_sock_t * s, char * host_ip, int port, const zbx_uint64_t inner_job_id)
{
    int ret = SUCCEED;
    const int TIMEOUT_LIMIT = 120;
    const int SLEEP_TIMEOUT = 10;
    // const int TIMEOUT_LIMIT = 600;
    int timeout_cnt = 0,time = 0;
    DB_RESULT result;
    DB_ROW row;
    int job_status;
    const char *__function_name = "ja_connect";

    //zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);


    zabbix_log(LOG_LEVEL_DEBUG, "In %s() connect the host. source_ip: %s, host_ip: %s, port: %d, timeout: %d",
               __function_name, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    //repeat connection until timeout min.
    // Effected : jaruniconreboot: jarun_icon_reboot()
    //          : jaruiconjob    : jarun_icon_job()
    //          : jaruniconfcopy : jarun_icon_fcopy()
    //          : jarunagent     : jarun_agent()
    //ret = zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    while( zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT) == FAIL){
        if(time <= TIMEOUT_LIMIT){
            zabbix_log(LOG_LEVEL_WARNING,"In %s(), Error connecting to agent. Retry connection for inner job id :"ZBX_FS_UI64,__function_name,inner_job_id);
            time += SLEEP_TIMEOUT;
            timeout_cnt++;
            sleep(SLEEP_TIMEOUT);
            continue; 
        }else{
            ret = FAIL;
            time = 0;
            break;
        }
        
    }
    if(timeout_cnt>0 && ret==SUCCEED){
        return timeout_cnt;
    }
    if (ret == FAIL) {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        result = DBselect("select status from ja_run_job_table where inner_job_id = " ZBX_FS_UI64, inner_job_id);
        row = DBfetch(result);
        if(row!=NULL){
            job_status = atoi(row[0]);
            if(job_status < 3) {
                ja_log("JACONNECT300001", 0, NULL, inner_job_id, __function_name, zbx_tcp_strerror(),host_ip, port, CONFIG_SOURCE_IP, CONFIG_TIMEOUT);
            }
        }else{
            zabbix_log(LOG_LEVEL_WARNING,"In%s(), Cannot get status from database. jobid :"ZBX_FS_UI64,__function_name,inner_job_id);
        }
        DBfree_result(result);
        DBclose();
    }

    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_connect_status_check                                          *
 *                                                                            *
 * Purpose: connect to the job agent host                                     *
 *                                                                            *
 * Parameters: s            (in) - socket identification information          *
 *             host         (in) - host name                                  *
 *             inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments: ja_connect function for jarun_status_check                       *
 *           not to close connection if fail                                  *
 *                                                                            *
 ******************************************************************************/
int ja_connect_status_check(zbx_sock_t * s, char * host_ip, int port, const zbx_uint64_t inner_job_id)
{
    int ret = SUCCEED;
    const int TIMEOUT_LIMIT = 120;
    const int SLEEP_TIMEOUT = 10;
    int timeout_cnt = 0,time = 0;
    DB_RESULT result;
    DB_ROW row;
    int job_status;
    const char *__function_name = "ja_connect_status_check";


    zabbix_log(LOG_LEVEL_DEBUG, "In %s() connect the host. source_ip: %s, host_ip: %s, port: %d, timeout: %d",
               __function_name, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    while( zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT) == FAIL){
        if(time <= TIMEOUT_LIMIT){
            zabbix_log(LOG_LEVEL_WARNING,"In %s(), Error connecting to agent. Retry connection for inner job id :"ZBX_FS_UI64,__function_name,inner_job_id);
            time += SLEEP_TIMEOUT;
            timeout_cnt++;
            sleep(SLEEP_TIMEOUT);
            continue; 
        }else{
            ret = FAIL;
            time = 0;
            break;
        }
        
    }
    if(timeout_cnt>0 && ret==SUCCEED){
        return timeout_cnt;
    }
    if (ret == FAIL) {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        result = DBselect("select status from ja_run_job_table where inner_job_id = " ZBX_FS_UI64, inner_job_id);
        row = DBfetch(result);
        if(row!=NULL){
            job_status = atoi(row[0]);
            if(job_status < 3) {
                ja_log("JACONNECT300001", 0, NULL, inner_job_id, __function_name, zbx_tcp_strerror(),host_ip, port, CONFIG_SOURCE_IP, CONFIG_TIMEOUT);
            }
        }else{
            zabbix_log(LOG_LEVEL_WARNING,"In%s(), Cannot get status from database. jobid :"ZBX_FS_UI64,__function_name,inner_job_id);
        }
        DBfree_result(result);
        DBclose();
    }

    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_connect_to_port                                               *
 *                                                                            *
 * Purpose: connection to the specified port on the host                      *
 *                                                                            *
 * Parameters: s            (in)  - socket identification information         *
 *             host         (in)  - host name                                 *
 *             inner_job_id (in)  - inner job id                              *
 *             txn          (in)  - transaction instruction                   *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_connect_to_port(zbx_sock_t * s, const char *host, const zbx_uint64_t inner_job_id, int txn)
{
    int ret;
    char host_ip[128];
    zbx_uint64_t hostid;
    int port;
    const char *__function_name = "ja_connect_to_port";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host: %s", __function_name, host);

    hostid = ja_host_getip(host, host_ip, inner_job_id, &port, txn);
    if (hostid == 0) {
        return FAIL;
    }

    port = ja_host_getport(hostid, 1);

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() connect the host. source_ip: %s, host_ip: %s, port: %d, timeout: %d",
               __function_name, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);

    ret = zbx_tcp_connect(s, CONFIG_SOURCE_IP, host_ip, port, CONFIG_TIMEOUT);
    if (ret == FAIL) {
        ja_log("JACONNECT300001", 0, NULL, inner_job_id, __function_name, zbx_tcp_strerror(),
               host_ip, port, CONFIG_SOURCE_IP, CONFIG_TIMEOUT);
    }

    return ret;
}
