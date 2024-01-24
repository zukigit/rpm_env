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
#include "zbxdb.h"
#include "log.h"
#include "jacommon.h"
#include "jagetagent.h"
#include "jalog.h"
extern int CONFIG_AGENT_LISTEN_PORT;
extern int CONFIG_ZABBIX_VERSION;
int get_agent_info(zbx_uint64_t inner_job_id,
                   zbx_uint64_t inner_jobnet_id,
                   JA_AGENT_INFO * agent_info)
{
    const char *__function_name = "get_agent_info";
    DB_RESULT result;
    DB_ROW row;
    int host_flag;
    int useip;
    unsigned short port;
    char *before_value = NULL;
    int ret = SUCCEED;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    result =
        DBselect("select host_flag from ja_run_icon_job_table"
                 " where inner_job_id=" ZBX_FS_UI64, inner_job_id);
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAGETAGENT200001", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id);
        DBfree_result(result);
        ret = FAIL;
        return ret;
    }
    host_flag = atoi(row[0]);
    DBfree_result(result);
    if (host_flag == USE_HOSTVARIABLE) {
        result =
            DBselect
            ("select v.before_value from ja_run_value_before_table v, ja_run_icon_job_table i, hosts h"
             " where i.inner_job_id = " ZBX_FS_UI64
             " and v.inner_job_id = " ZBX_FS_UI64
             " and i.host_name = v.value_name"
             " and v.before_value = h.host" " and h.status = %d",
             inner_job_id, inner_job_id, HOST_STATUS_MONITORED);
        row = DBfetch(result);
        if (row == NULL) {
            ja_log("JAGETAGENT200002", inner_jobnet_id, NULL,
                   inner_job_id, inner_job_id);
            DBfree_result(result);
            ret = FAIL;
            return ret;
        } else {
            before_value = DBdyn_escape_string(row[0]);
            DBfree_result(result);
            result = DBselect("select m.value from hosts h, hostmacro m"
                              " where h.host = '%s'"
                              " and h.hostid=m.hostid"
                              " and m.macro='%s'", before_value,
                              JA_AGENT_PORT);
        }
    } else if (host_flag == USE_HOSTNAME) {
        result =
            DBselect
            ("select m.value from ja_run_icon_job_table i, hosts h, hostmacro m"
             " where i.inner_job_id = " ZBX_FS_UI64
             " and i.host_name=h.host" " and h.hostid=m.hostid"
             " and m.macro='%s'" " and h.status = %d", inner_job_id,
             JA_AGENT_PORT, HOST_STATUS_MONITORED);
    } else {
        ja_log("JAGETAGENT200003", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id, host_flag);
        ret = FAIL;
        return ret;
    }
    row = DBfetch(result);

    /*Port number */
    if (row == NULL) {
        /*Use default port */
        zabbix_log(LOG_LEVEL_DEBUG,
                   "Cannot get agent port. Use default port.");
        agent_info->port = CONFIG_AGENT_LISTEN_PORT;
    } else {
        port = atoi(row[0]);
        if (port < 1) {
            ja_log("JAGETAGENT200004", inner_jobnet_id, NULL,
                   inner_job_id);
            ret = FAIL;
            return ret;
        } else {
            agent_info->port = port;
        }
    }
    DBfree_result(result);

    /*IP address */
    if (host_flag == USE_HOSTNAME) {
        switch (CONFIG_ZABBIX_VERSION) {
        case 1:
            // for zabbix 1.8
            result =
                DBselect
                ("select h.useip from ja_run_icon_job_table j, hosts h"
                 " where j.inner_job_id=" ZBX_FS_UI64
                 " and j.host_name=h.host", inner_job_id);
            break;
        case 2:
        case 3:
            // for zabbix 2.0 or 2.2
            result =
                DBselect
                ("select i.useip from ja_run_icon_job_table j, hosts h, interface i"
                 " where j.inner_job_id=" ZBX_FS_UI64
                 " and j.host_name=h.host"
                 " and h.hostid=i.hostid and i.main=1", inner_job_id);
            break;
        default:
            ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                   inner_job_id, CONFIG_ZABBIX_VERSION);
            ret = FAIL;
            return ret;
        }
    } else {
        switch (CONFIG_ZABBIX_VERSION) {
        case 1:
            // for zabbix 1.8
            result =
                DBselect
                ("select h.useip from ja_run_icon_job_table j, hosts h"
                 " where j.inner_job_id=" ZBX_FS_UI64 " and h.host='%s'",
                 inner_job_id, before_value);
            break;
        case 2:
        case 3:
            // for zabbix 2.0 or 2.2
            result =
                DBselect
                ("select i.useip from ja_run_icon_job_table j, hosts h, interface i"
                 " where j.inner_job_id=" ZBX_FS_UI64
                 " and h.host='%s'" " and h.hostid=i.hostid and i.main=1",
                 inner_job_id, before_value);
            break;
        default:
            ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                   inner_job_id, CONFIG_ZABBIX_VERSION);
            ret = FAIL;
            return ret;
        }
    }
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAGETAGENT200005", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id);
        DBfree_result(result);
        ret = FAIL;
        return ret;
    }
    useip = atoi(row[0]);
    DBfree_result(result);
    if (useip == USE_IPADDRESS) {
        if (host_flag == USE_HOSTNAME) {

            /*IP address */
            switch (CONFIG_ZABBIX_VERSION) {
            case 1:
                // for zabbix 1.8
                result =
                    DBselect
                    ("select h.ip from ja_run_icon_job_table j, hosts h"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and j.host_name=h.host", inner_job_id);
                break;
            case 2:
            case 3:
                // for zabbix 2.0 or 2.2
                result =
                    DBselect
                    ("select i.ip from ja_run_icon_job_table j, hosts h, interface i"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and j.host_name=h.host"
                     " and h.hostid=i.hostid and i.main=1", inner_job_id);
                break;
            default:
                ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                       inner_job_id, CONFIG_ZABBIX_VERSION);
                ret = FAIL;
                return ret;
            }
        } else {
            switch (CONFIG_ZABBIX_VERSION) {
            case 1:
                // for zabbix 1.8
                result =
                    DBselect
                    ("select h.ip from ja_run_icon_job_table j, hosts h"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and h.host='%s'", inner_job_id, before_value);
                break;
            case 2:
            case 3:
                // for zabbix 2.0 or 2.2
                result =
                    DBselect
                    ("select i.ip from ja_run_icon_job_table j, hosts h, interface i"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and h.host='%s'"
                     " and h.hostid=i.hostid and i.main=1", inner_job_id,
                     before_value);
                break;
            default:
                ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                       inner_job_id, CONFIG_ZABBIX_VERSION);
                ret = FAIL;
                return ret;
            }
        }
    } else if (useip == USE_DNS) {
        if (host_flag == USE_HOSTNAME) {
            /*DNS */
            switch (CONFIG_ZABBIX_VERSION) {
            case 1:
                // for zabbix 1.8
                result =
                    DBselect
                    ("select h.dns from ja_run_icon_job_table j, hosts h"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and j.host_name=h.host", inner_job_id);
                break;
            case 2:
            case 3:
                // for zabbix 2.0 or 2.2
                result =
                    DBselect
                    ("select i.dns from ja_run_icon_job_table j, hosts h, interface i"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and j.host_name=h.host"
                     " and h.hostid=i.hostid and i.main=1", inner_job_id);
                break;
            default:
                ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                       inner_job_id, CONFIG_ZABBIX_VERSION);
                ret = FAIL;
                return ret;
            }
        } else {
            switch (CONFIG_ZABBIX_VERSION) {
            case 1:
                // for zabbix 1.8
                result =
                    DBselect
                    ("select h.dns from ja_run_icon_job_table j, hosts h"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and h.host='%s'", inner_job_id, before_value);
                break;
            case 2:
            case 3:
                // for zabbix 2.0 or 2.2
                result =
                    DBselect
                    ("select i.dns from ja_run_icon_job_table j, hosts h, interface i"
                     " where j.inner_job_id=" ZBX_FS_UI64
                     " and h.host='%s'"
                     " and h.hostid=i.hostid and i.main=1", inner_job_id,
                     before_value);
                break;
            default:
                ja_log("JAGETAGENT200008", inner_jobnet_id, NULL,
                       inner_job_id, CONFIG_ZABBIX_VERSION);
                ret = FAIL;
                return ret;
            }
        }
    } else {
        ja_log("JAGETAGENT200006", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id, useip);
        ret = FAIL;
        return ret;
    }
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAGETAGENT200007", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id);
        DBfree_result(result);
        ret = FAIL;
        return ret;
    }
    agent_info->host = strdup(row[0]);
    DBfree_result(result);
    zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
    return ret;
}

char *get_agent_hostname(zbx_uint64_t inner_job_id,
                         zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "get_agent_hostname";
    char *hostname = NULL;
    DB_RESULT result;
    DB_ROW row;
    int host_flag;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    result = DBselect("select host_flag from ja_run_icon_job_table"
                      " where inner_job_id=" ZBX_FS_UI64 ";\n",
                      inner_job_id);
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAGETAGENT200001", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id);
        DBfree_result(result);
        return hostname;
    }
    host_flag = atoi(row[0]);
    DBfree_result(result);
    if (host_flag == USE_HOSTVARIABLE) {
        result =
            DBselect
            ("select v.before_value from ja_run_value_before_table v, ja_run_icon_job_table i, hosts h"
             " where i.inner_job_id = " ZBX_FS_UI64
             " and v.inner_job_id = " ZBX_FS_UI64
             " and i.host_name = v.value_name"
             " and v.before_value = h.host" " and h.status = %d",
             inner_job_id, inner_job_id, HOST_STATUS_MONITORED);
    } else if (host_flag == USE_HOSTNAME) {
        result =
            DBselect
            ("select i.host_name from ja_run_icon_job_table i, hosts h"
             " where i.inner_job_id = " ZBX_FS_UI64
             " and i.host_name = h.host" " and h.status = %d",
             inner_job_id, HOST_STATUS_MONITORED);
    } else {
        ja_log("JAGETAGENT200003", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id, host_flag);
        return hostname;
    }
    row = DBfetch(result);
    if (row == NULL) {
        ja_log("JAGETAGENT200002", inner_jobnet_id, NULL, inner_job_id,
               inner_job_id);
        DBfree_result(result);
        return hostname;
    }
    hostname = strdup(row[0]);
    DBfree_result(result);
    return hostname;
}
