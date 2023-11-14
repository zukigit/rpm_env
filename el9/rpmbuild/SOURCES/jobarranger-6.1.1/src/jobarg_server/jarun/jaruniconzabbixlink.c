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
#include "log.h"
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastr.h"
#include "jastatus.h"
#include "javalue.h"
#include "jaflow.h"
#include "jaruniconzabbixlink.h"
#include "jazbxmsg.h"
#include "jajobid.h"

#define SUCCEED                             0
#define FAIL                                -1
#define FAIL_STATUS                         -2

#define ZBXLINK_NON_TARGET_ITEM_TYPE        9

#define ZBXLINK_FLAG_DISCOVERY_NORMAL       0
#define ZBXLINK_FLAG_DISCOVERY_CREATED      4

#define ZBXLINK_TRIGGER_VALUE_TRUE          1
#define ZBXLINK_TRIGGER_VALUE_UNKNOWN       2

#define ZBXLINK_ITEM_STATUS_ENABLED         0
#define ZBXLINK_ITEM_STATUS_DISABLED        1

#define ZBXLINK_TRIGGER_STATUS_ENABLED      0
#define ZBXLINK_TRIGGER_STATUS_DISABLED     1

#define ZBXLINK_TRIGGER_VALUE_FLAG_NORMAL   0
#define ZBXLINK_TRIGGER_VALUE_FLAG_UNKNOWN  1

#define ZBXLINK_HOST_STATUS_MONITORED       0
#define ZBXLINK_HOST_STATUS_NOT_MONITORED   1

#define ZBXLINK_SERVICE_ALGORITHM_NONE      0
#define ZBXLINK_SERVICE_ALGORITHM_MAX       1
#define ZBXLINK_SERVICE_ALGORITHM_MIN       2

#define ZBXLINK_ZBX_LAST_STATUS             "ZBX_LAST_STATUS"
#define ZBXLINK_ZBX_LATEST_DATA             "ZBX_LATEST_DATA"
#define ZBXLINK_ZBX_DATA_TYPE               "ZBX_DATA_TYPE"

#define ZBXLINK_HOST_GROUP_FLAG_OFF         0
#define ZBXLINK_HOST_GROUP_FLAG_ON          1

extern int CONFIG_ZABBIX_VERSION;


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
char *jarun_icon_zabbixlink_get_trigger_message(const zbx_uint64_t inner_job_id, char *message_id)
{
    DB_RESULT result;
    DB_ROW row;
    char *p_lang;
    char *p_ret = NULL;
    char *p_message = NULL;
    const char *__function_name = "jarun_icon_zabbixlink_get_trigger_message";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    result = DBselect("select u.lang from ja_run_jobnet_table j, ja_run_job_table jb, users u"
                      " where u.username = j.execution_user_name"
                      " and j.inner_jobnet_id = jb.inner_jobnet_main_id"
                      " and jb.inner_job_id = " ZBX_FS_UI64,
                      inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        p_lang = row[0];

        /* Get the message body */
        p_message = ja_zabbix_message(message_id, p_lang, inner_job_id);
        if (p_message == NULL) {
            ja_log("JARUNICONZABBIXLINK200024", 0, NULL, inner_job_id, __function_name, inner_job_id, message_id);
        }
        else {
            p_ret = p_message;
        }
    }
    else {
        ja_log("JARUNICONZABBIXLINK200032", 0, NULL, inner_job_id, __function_name, inner_job_id);
    }

    DBfree_result(result);

    return p_ret;
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
int jarun_icon_zabbixlink_get_item_status(const zbx_uint64_t inner_job_id, const char *get_item_id)
{
    int ret = SUCCEED, status = 0, state=0 ;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_get_item_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() item_id: %s", __function_name, get_item_id);

    /* Zabbix version decision */
    switch (CONFIG_ZABBIX_VERSION) {
        case 1:  /* for zabbix 1.8 */
            /* Search the items table */
            result = DBselect("select status from items where itemid = %s and type <> %d",
                              get_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE);
            break;

        case 2:  /* for zabbix 2.0 */
        case 3:  /* for zabbix 2.2Å`4.0 */
            /* Search the items table */
            //result = DBselect("select status,state from items where itemid = %s and type <> %d and flags in(%d, %d)",
        	/* for zabbix 5.0 */
        	result = DBselect("select items.status,item_rtdata.state from items,item_rtdata where items.itemid = %s "
        			             " and item_rtdata.itemid =items.itemid and items.type <> %d and items.flags in(%d, %d)",
                              get_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE, ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
            break;

        default:
            ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
            return FAIL;
    }

    if (NULL != (row = DBfetch(result))) {
        /* status setting */
    	status = atoi(row[0]);
        state = atoi(row[1]);
        if (state != 0 ){
        	ret = 3;
        }else{
        	ret = status;
        }

    }
    else {
        ja_log("JARUNICONZABBIXLINK200002", 0, NULL, inner_job_id, __function_name, inner_job_id, get_item_id);
        ret = FAIL;
    }

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
int jarun_icon_zabbixlink_set_item_status(const zbx_uint64_t inner_job_id, const char *set_item_id, const int set_status)
{
    int ret = SUCCEED;
    int db_ret;
    int status;
    int no_support=3;
    const char *__function_name = "jarun_icon_zabbixlink_set_item_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() item_id: %s, status: %d", __function_name, set_item_id, set_status);

    /* item state acquisition */
    status = jarun_icon_zabbixlink_get_item_status(inner_job_id, set_item_id);

    /* item state get success */
    if (status >= SUCCEED) {
        /* There item state change */
        if (status != set_status) {
            /* Enabling item */
            if (set_status == ZBXLINK_ITEM_STATUS_ENABLED) {
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d, error = ''"
                                           " where itemid = %s and type <> %d",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE);
                        break;

                    case 2:  /* for zabbix 2.0 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d, error = ''"
                                           " where itemid = %s and type <> %d and flags in(%d, %d)",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
                        break;

                    case 3:  /* for zabbix 2.2 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d"
                                           " where itemid = %s and type <> %d and flags in(%d, %d)",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }
            /* item disabled */
            else {
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d"
                                           " where itemid = %s and type <> %d",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE);
                        break;

                    case 2:  /* for zabbix 2.0 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d, error = ''"
                                           " where itemid = %s and type <> %d and flags in(%d, %d)",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
                        break;

                    case 3:  /* for zabbix 2.2 */
                        /* the items table update */
                        db_ret = DBexecute("update items set status = %d"
                                           " where itemid = %s and type <> %d and flags in(%d, %d)",
                                           set_status, set_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }

            if (db_ret < ZBX_DB_OK) {
                /* item status update failure */
                ja_log("JARUNICONZABBIXLINK200003", 0, NULL, inner_job_id, __function_name, inner_job_id, set_item_id);
                ret = FAIL;
            }
        }
    }
    /* item state acquisition failure */
    else {
        ja_log("JARUNICONZABBIXLINK200002", 0, NULL, inner_job_id, __function_name, inner_job_id, set_item_id);
        ret = FAIL;
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
int jarun_icon_zabbixlink_get_item_data(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, const char *get_item_id)
{
    int ret = SUCCEED;
    int value_type;
    char str_value_type[4];
    DB_RESULT result;
    DB_RESULT result_sub;
    DB_ROW row;
    char *item_id;
    const char *__function_name = "jarun_icon_zabbixlink_get_item_data";
    const char __history_tbl_name[][128] =
    {
        { "history"      },
        { "history_str"  },
        { "history_log"  },
        { "history_uint" },
        { "history_text" }
    };

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() item_id: %s", __function_name, get_item_id);

    /* Zabbix version decision */
    switch (CONFIG_ZABBIX_VERSION) {
        case 1:  /* for zabbix 1.8 */
            /* Search the items table */
            result = DBselect("select lastvalue, value_type from items"
                              " where itemid = %s and type <> %d",
                              get_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE);

            if (NULL != (row = DBfetch(result))) {
                /* Job Controller variable settings */
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LATEST_DATA, row[0]);
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_DATA_TYPE, row[1]);
            }
            else {
                ja_log("JARUNICONZABBIXLINK200004", 0, NULL, inner_job_id, __function_name, inner_job_id, get_item_id);
                ret = FAIL;
            }
            break;

        case 2:  /* for zabbix 2.0 */
            /* Search the items table */
            result = DBselect("select lastvalue, value_type from items"
                              " where itemid = %s and type <> %d and flags in(%d, %d)",
                              get_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                              ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);

            if (NULL != (row = DBfetch(result))) {
                /* Job Controller variable settings */
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LATEST_DATA, row[0]);
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_DATA_TYPE, row[1]);
            }
            else {
                ja_log("JARUNICONZABBIXLINK200004", 0, NULL, inner_job_id, __function_name, inner_job_id, get_item_id);
                ret = FAIL;
            }
            break;

        case 3:  /* for zabbix 2.2 */
            /* Search the items table */
            result = DBselect("select itemid, value_type from items"
                              " where itemid = %s and type <> %d and flags in(%d, %d)",
                              get_item_id, ZBXLINK_NON_TARGET_ITEM_TYPE,
                              ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);

            if (NULL != (row = DBfetch(result))) {
                item_id = row[0];
                value_type = atoi(row[1]);
            }
            else {
                ja_log("JARUNICONZABBIXLINK200004", 0, NULL, inner_job_id, __function_name, inner_job_id, get_item_id);
                ret = FAIL;
                break;
            }

            /* history-based table lookup */
            result_sub = DBselect("select value from %s where itemid = %s"
                                  " order by clock desc limit 1 offset 0",
                                  __history_tbl_name[value_type], item_id);

            if (NULL != (row = DBfetch(result_sub))) {
                /* Job Controller variable settings */
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LATEST_DATA, row[0]);
                zbx_snprintf(str_value_type, sizeof(str_value_type), "%d", value_type);
                ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_DATA_TYPE, str_value_type);
            }
            else {
                ja_log("JARUNICONZABBIXLINK200005", 0, NULL, inner_job_id, __function_name, inner_job_id, item_id);
                ret = FAIL;
            }
            DBfree_result(result_sub);
            break;

        default:
            ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
            return FAIL;
    }

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
int jarun_icon_zabbixlink_get_trigger_status(const zbx_uint64_t inner_job_id, const char *get_trigger_id)
{
    int ret = SUCCEED,status = 0, state=0 ;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_get_trigger_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() trigger_id: %s", __function_name, get_trigger_id);

    /* Zabbix version decision */
    switch (CONFIG_ZABBIX_VERSION) {
        case 1:  /* for zabbix 1.8 */
            /* triggers table search */
            result = DBselect("select status from triggers where triggerid = %s", get_trigger_id);
            break;

        case 2:  /* for zabbix 2.0 */
        case 3:  /* for zabbix 2.2 */
            /* triggers table search */
            result = DBselect("select status,state from triggers where triggerid = %s and flags in(%d, %d)",
                              get_trigger_id, ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
            break;

        default:
            ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
            return FAIL;
    }

    if (NULL != (row = DBfetch(result))) {
        /* status setting */
    	status = atoi(row[0]);
        state = atoi(row[1]);
    	if (state != 0 ){
    		ret = 3;
    	}else{
    	  	ret = status;
    	}
    }
    else {
        ja_log("JARUNICONZABBIXLINK200006", 0, NULL, inner_job_id, __function_name, inner_job_id, get_trigger_id);
        ret = FAIL;
    }

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
int jarun_icon_zabbixlink_update_ids(const zbx_uint64_t inner_job_id, const char *table_name, const char *field_name, zbx_uint64_t* nextid)
{
    int ret = SUCCEED;
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_update_ids";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() table_name: %s, field_name: %s", __function_name, table_name, field_name);

    /* ids table update */
    db_ret = DBexecute("update ids set nextid = nextid + 1"
                       " where nodeid = 0 and table_name = '%s' and field_name = '%s'",
                       table_name, field_name);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JARUNICONZABBIXLINK200007", 0, NULL, inner_job_id, __function_name, inner_job_id, table_name, field_name);
        return FAIL;
    }

    result = DBselect("select nextid from ids where nodeid = 0 and table_name = '%s' and field_name = '%s'",
                      table_name, field_name);

    if (NULL != (row = DBfetch(result))) {
        /* nextid setting */
        ZBX_STR2UINT64(*nextid, row[0]);
    }
    else {
        ja_log("JARUNICONZABBIXLINK200008", 0, NULL, inner_job_id, __function_name, inner_job_id, table_name, field_name);
        ret = FAIL;
    }

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
int jarun_icon_zabbixlink_insert_service_alarms(const zbx_uint64_t inner_job_id, const char *service_id, const int status)
{
    int ret = SUCCEED;
    int db_ret;
    time_t now_time;
    zbx_uint64_t nextid;
    const char *__function_name = "jarun_icon_zabbixlink_insert_service_alarms";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() service_id: %s", __function_name, service_id);

    /* ids table update */
    db_ret = jarun_icon_zabbixlink_update_ids(inner_job_id, "service_alarms", "servicealarmid", &nextid);
    if (db_ret < ZBX_DB_OK) {
        ja_log("JARUNICONZABBIXLINK200009", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
        return FAIL;
    }

    /* The record added to the table service_alarms */
    db_ret = DBexecute("insert into service_alarms (servicealarmid, serviceid, clock, value)"
                       " values(" ZBX_FS_UI64 ", %s, %lu, %d)",
                       nextid, service_id, time(&now_time), status);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JARUNICONZABBIXLINK200010", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
        return FAIL;
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
int jarun_icon_zabbixlink_search_service_alarms(const zbx_uint64_t inner_job_id, const char *service_id, const int status)
{
    int ret = SUCCEED;
    int db_ret;
    int value;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_search_service_alarms";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() service_id: %s", __function_name, service_id);

    result = DBselect("select servicealarmid, value from service_alarms"
                      " where serviceid = %s"
                      " order by servicealarmid desc limit 1 offset 0",
                      service_id);

    if (NULL != (row = DBfetch(result))) {
        value = atoi(row[1]);

        if (value != status) {
            /* The record added to the table service_alarms */
            db_ret = jarun_icon_zabbixlink_insert_service_alarms(inner_job_id, service_id, status);
            if (db_ret < ZBX_DB_OK) {
                /* Add failure */
                ja_log("JARUNICONZABBIXLINK200010", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
                ret = FAIL;
            }
        }
    }
    else {
        /* Added at any None */
        /* The record added to the table service_alarms */
        db_ret = jarun_icon_zabbixlink_insert_service_alarms(inner_job_id, service_id, status);
        if (db_ret < ZBX_DB_OK) {
            /* Add failure */
            ja_log("JARUNICONZABBIXLINK200010", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
            ret = FAIL;
        }
    }

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
int jarun_icon_zabbixlink_search_service_links(const zbx_uint64_t inner_job_id, const char *service_id)
{
    int ret = SUCCEED;
    int db_ret;
    int status;
    int algorithm;
    DB_RESULT result;
    DB_RESULT result_sub;
    DB_ROW row;
    char *serviceup_id;
    const char *__function_name = "jarun_icon_zabbixlink_search_service_links";
    const char __sort_order[][5] =
    {
        { ""     },
        { "DESC" },
        { "ASC"  }
    };

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() service_id: %s", __function_name, service_id);

    result = DBselect("select l.serviceupid, s.algorithm from services_links l, services s"
                      " where s.serviceid = l.serviceupid and l.servicedownid = %s",
                      service_id);

    while (NULL != (row = DBfetch(result))) {
        status = 0;
        serviceup_id = row[0];
        algorithm = atoi(row[1]);

        if (algorithm == ZBXLINK_SERVICE_ALGORITHM_MAX || algorithm == ZBXLINK_SERVICE_ALGORITHM_MIN) {
            result_sub = DBselect("select s.status from services s, services_links l"
                                  " where l.serviceupid = %s and s.serviceid = l.servicedownid"
                                  " order by s.status %s",
                                  serviceup_id, __sort_order[algorithm]);

            /* Processing continues default value (0) when it is not possible to take */
            if (NULL != (row = DBfetch(result_sub))) {
                status = atoi(row[0]);
            }

            DBfree_result(result_sub);

            db_ret = jarun_icon_zabbixlink_search_service_alarms(inner_job_id, serviceup_id, status);
            if (db_ret < ZBX_DB_OK) {
                /* Add failure */
                ja_log("JARUNICONZABBIXLINK200011", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
                DBfree_result(result);
                return FAIL;
            }

            /* services table update */
            db_ret = DBexecute("update services set status = %d where serviceid = %s",
                               status, serviceup_id);

            if (db_ret < ZBX_DB_OK) {
                /* service status update failure */
                ja_log("JARUNICONZABBIXLINK200012", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
                DBfree_result(result);
                return FAIL;
            }
        }
        else if (algorithm != ZBXLINK_SERVICE_ALGORITHM_NONE) {
            ja_log("JARUNICONZABBIXLINK200013", 0, NULL, inner_job_id, __function_name, inner_job_id, algorithm, service_id);
            DBfree_result(result);
            return FAIL;
        }
    }

    DBfree_result(result);

    result = DBselect("select serviceupid from services_links where servicedownid = %s", service_id);

    while (NULL != (row = DBfetch(result))) {
        serviceup_id = row[0];
        jarun_icon_zabbixlink_search_service_links(inner_job_id, serviceup_id);
    }

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
int jarun_icon_zabbixlink_insert_events(const zbx_uint64_t inner_job_id, const char *trigger_id)
{
    int ret = SUCCEED;
    int db_ret;
    time_t now_time;
    zbx_uint64_t nextid;
    const char *__function_name = "jarun_icon_zabbixlink_insert_events";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() trigger_id: %s", __function_name, trigger_id);

    /* ids table update */
    db_ret = jarun_icon_zabbixlink_update_ids(inner_job_id, "events", "eventid", &nextid);
    if (db_ret < ZBX_DB_OK) {
        ja_log("JARUNICONZABBIXLINK200014", 0, NULL, inner_job_id, __function_name, inner_job_id, trigger_id);
        return FAIL;
    }

    /* Add events table */
    db_ret = DBexecute("insert into events (eventid, source, object, objectid, clock, value, acknowledged)"
                       " values(" ZBX_FS_UI64 ", 0, 0, %s, %lu, 2, 0)",
                       nextid, trigger_id, time(&now_time));

    if (db_ret < ZBX_DB_OK) {
        ja_log("JARUNICONZABBIXLINK200015", 0, NULL, inner_job_id, __function_name, inner_job_id, trigger_id);
        return FAIL;
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
int jarun_icon_zabbixlink_set_service(const zbx_uint64_t inner_job_id, const char *set_trigger_id, const int set_status)
{
    int ret = SUCCEED;
    int db_ret;
    int status;
    char *service_id;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_set_service";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() trigger_id: %s", __function_name, set_trigger_id);

    if (set_status == ZBXLINK_TRIGGER_STATUS_ENABLED) {
        /* trigger activation */
        result = DBselect("select priority from triggers"
                          " where triggerid = %s and status = %d and value = %d limit 1 offset 0",
                          set_trigger_id, ZBXLINK_TRIGGER_STATUS_ENABLED, ZBXLINK_TRIGGER_VALUE_TRUE);

        if (NULL != (row = DBfetch(result))) {
            status = atoi(row[0]);
        }
        else {
            status = 0;
        }

        DBfree_result(result);
    }
    else {
        /* trigger disabled */
        status = 0;
    }

    /* services table update */
    db_ret = DBexecute("update services set status = %d where triggerid = %s",
                       status, set_trigger_id);

    if (db_ret < ZBX_DB_OK) {
        /* service status update failure */
        ja_log("JARUNICONZABBIXLINK200016", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
        return FAIL;
    }

    result = DBselect("select serviceid from services where triggerid = %s", set_trigger_id);

    while (NULL != (row = DBfetch(result))) {
        service_id = row[0];
        db_ret = jarun_icon_zabbixlink_search_service_alarms(inner_job_id, service_id, status);
        if (db_ret < ZBX_DB_OK) {
            /* Add failure */
            ja_log("JARUNICONZABBIXLINK200011", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
            DBfree_result(result);
            return FAIL;
        }

        db_ret = jarun_icon_zabbixlink_search_service_links(inner_job_id, service_id);
        if (db_ret < ZBX_DB_OK) {
            /* Add failure */
            ja_log("JARUNICONZABBIXLINK200017", 0, NULL, inner_job_id, __function_name, inner_job_id, service_id);
            DBfree_result(result);
            return FAIL;
        }
    }

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
int jarun_icon_zabbixlink_set_trigger_status(const zbx_uint64_t inner_job_id, const char *set_trigger_id, const int set_status)
{
    int ret = SUCCEED;
    int db_ret;
    int status;
    int value;
    time_t now_time;
    DB_RESULT result;
    DB_ROW row;
    char *p_message = NULL;
    const char *__function_name = "jarun_icon_zabbixlink_set_trigger_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() trigger_id: %s, status: %d", __function_name, set_trigger_id, set_status);

    /* trigger state acquisition */
    status = jarun_icon_zabbixlink_get_trigger_status(inner_job_id, set_trigger_id);
    if (status >= SUCCEED) {
        /* trigger state acquisition success */
        /* There trigger state change */
        if (status != set_status) {
            if (set_status == 0) {
                /* trigger activation */
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* triggers table update */
                        db_ret = DBexecute("update triggers set status = %d where triggerid = %s",
                                           set_status, set_trigger_id);

                        if (db_ret < ZBX_DB_OK) {
                            /* triggers table update failure */
                            ja_log("JARUNICONZABBIXLINK200018", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }

                        db_ret = jarun_icon_zabbixlink_set_service(inner_job_id, set_trigger_id, set_status);
                        if (db_ret < ZBX_DB_OK) {
                            ja_log("JARUNICONZABBIXLINK200016", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }
                        break;

                    case 2:  /* for zabbix 2.0 */
                    case 3:  /* for zabbix 2.2 */
                        /* triggers table update */
                        db_ret = DBexecute("update triggers set status = %d"
                                           " where triggerid = %s and flags in(%d, %d)",
                                           set_status, set_trigger_id, ZBXLINK_FLAG_DISCOVERY_NORMAL,
                                           ZBXLINK_FLAG_DISCOVERY_CREATED);

                        if (db_ret < ZBX_DB_OK) {
                            /* triggers table update failure */
                            ja_log("JARUNICONZABBIXLINK200018", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }
            else {
                /* trigger disabled */
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* triggers table update */
                        db_ret = DBexecute("update triggers set status = %d where triggerid = %s",
                                           set_status, set_trigger_id);

                        if (db_ret < ZBX_DB_OK) {
                            /* triggers table update failure */
                            ja_log("JARUNICONZABBIXLINK200018", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }

                        /* triggers table search */
                        result = DBselect("select value from triggers where triggerid = %s", set_trigger_id);

                        if (NULL != (row = DBfetch(result))) {
                            value = atoi(row[0]);
                        }
                        else {
                            ja_log("JARUNICONZABBIXLINK200019", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }

                        if (value != ZBXLINK_TRIGGER_VALUE_UNKNOWN) {
                            /* The record added to the events table */
                            db_ret = jarun_icon_zabbixlink_insert_events(inner_job_id, set_trigger_id);
                            if (db_ret < ZBX_DB_OK) {
                                /* The failure to record additional events table */
                                ja_log("JARUNICONZABBIXLINK200015", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                                return FAIL;
                            }

                            /* triggers table update */
                            db_ret = DBexecute("update triggers set lastchange = %lu, value = %d"
                                               " where triggerid = %s",
                                               time(&now_time), ZBXLINK_TRIGGER_VALUE_UNKNOWN, set_trigger_id);

                            if (db_ret < ZBX_DB_OK) {
                                /* triggers table update failure */
                                ja_log("JARUNICONZABBIXLINK200020", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                                return FAIL;
                            }
                        }

                        /* services related table update */
                        db_ret = jarun_icon_zabbixlink_set_service(inner_job_id, set_trigger_id, set_status);
                        if (db_ret < ZBX_DB_OK) {
                            ja_log("JARUNICONZABBIXLINK200016", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }
                        break;

                    case 2:  /* for zabbix 2.0 */
                        /* triggers table update */
                        db_ret = DBexecute("update triggers set status = %d"
                                           " where triggerid = %s and flags in(%d, %d)",
                                           set_status, set_trigger_id, ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);

                        if (db_ret < ZBX_DB_OK) {
                            /* trigger table update failure */
                            ja_log("JARUNICONZABBIXLINK200018", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }

                        /* triggers table search */
                        result = DBselect("select t.triggerid from triggers t, functions f, items i, hosts h"
                                          " where t.triggerid = f.triggerid and f.itemid = i.itemid and i.hostid = h.hostid"
                                          " and t.triggerid = %s and t.value_flags = %d and h.status in(%d, %d)",
                                          set_trigger_id, ZBXLINK_TRIGGER_VALUE_FLAG_NORMAL, ZBXLINK_HOST_STATUS_MONITORED,
                                          ZBXLINK_HOST_STATUS_NOT_MONITORED);

                        if (NULL != (row = DBfetch(result))) {
                            DBfree_result(result);

                            /* Get the message body */
                            p_message = jarun_icon_zabbixlink_get_trigger_message(inner_job_id, "TRG0001");
                            if (p_message == NULL) {
                                ja_log("JARUNICONZABBIXLINK200024", 0, NULL, inner_job_id, __function_name, inner_job_id, "TRG0001");
                                return FAIL;
                            }

                            /* triggers table update */
                            db_ret = DBexecute("update triggers set value_flags = %d, error = '%s'"
                                               " where triggerid = %s",
                                               ZBXLINK_TRIGGER_VALUE_FLAG_UNKNOWN, p_message, set_trigger_id);

                            zbx_free(p_message);

                            if (db_ret < ZBX_DB_OK) {
                                /* trigger table update failure */
                                ja_log("JARUNICONZABBIXLINK200021", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                                return FAIL;
                            }

                            /* events table update */
                            db_ret = jarun_icon_zabbixlink_insert_events(inner_job_id, set_trigger_id);
                            if (db_ret < ZBX_DB_OK) {
                                /* events table update failure */
                                ja_log("JARUNICONZABBIXLINK200015", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                                return FAIL;
                            }
                        }
                        break;

                    case 3:  /* for zabbix 2.2 */
                        /* triggers table update */
                        db_ret = DBexecute("update triggers set status = %d"
                                           " where triggerid = %s and flags in(%d, %d)",
                                           set_status, set_trigger_id, ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
                        if (db_ret < ZBX_DB_OK) {
                            /* trigger table update failure */
                            ja_log("JARUNICONZABBIXLINK200018", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
                            return FAIL;
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }
        }
    }
    else {
        /* trigger state acquisition failure */
        ja_log("JARUNICONZABBIXLINK200006", 0, NULL, inner_job_id, __function_name, inner_job_id, set_trigger_id);
        ret = FAIL;
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
int jarun_icon_zabbixlink_get_host_status(const zbx_uint64_t inner_job_id, const char *get_host_id)
{
    int ret = SUCCEED;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "jarun_icon_zabbixlink_get_host_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host_id: %s", __function_name, get_host_id);

    /* Zabbix version decision */
    switch (CONFIG_ZABBIX_VERSION) {
        case 1:  /* for zabbix 1.8 */
        case 2:  /* for zabbix 2.0 */
            /* hosts table search */
            result = DBselect("select status from hosts"
                              " where hostid = %s and status in(%d, %d)",
                              get_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED);
            break;

        case 3:  /* for zabbix 2.2 */
            /* hosts table search */
            result = DBselect("select status from hosts"
                              " where hostid = %s and status in(%d, %d) and flags in(%d, %d)",
                              get_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED,
                              ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);
            break;

        default:
            ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
            return FAIL;
    }

    if (NULL != (row = DBfetch(result))) {
        ret = atoi(row[0]);
    }
    else {
        ja_log("JARUNICONZABBIXLINK200022", 0, NULL, inner_job_id, __function_name, inner_job_id, get_host_id);
        ret = FAIL;
    }

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
int jarun_icon_zabbixlink_set_host_status_ver18(const zbx_uint64_t inner_job_id, const char *set_host_id, const int set_status)
{
    int ret = SUCCEED;
    int db_ret;
    int value;
    DB_RESULT result;
    DB_ROW row;
    char *trigger_id;
    const char *__function_name = "jarun_icon_zabbixlink_set_host_status_ver18";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host_id: %s", __function_name, set_host_id);

    /* hosts table search */
    result = DBselect("select distinct t.triggerid, t.value from items i, triggers t, functions f"
                      " where f.triggerid = t.triggerid and f.itemid = i.itemid and i.hostid = %s",
                      set_host_id);

    while (NULL != (row = DBfetch(result))) {
        trigger_id = row[0];
        value = atoi(row[1]);

        if (value != ZBXLINK_TRIGGER_VALUE_UNKNOWN) {
            db_ret = jarun_icon_zabbixlink_insert_events(inner_job_id, trigger_id);
            if (db_ret < ZBX_DB_OK) {
                /* The failure to record additional events table */
                ja_log("JARUNICONZABBIXLINK200015", 0, NULL, inner_job_id, __function_name, inner_job_id, trigger_id);
                DBfree_result(result);
                return FAIL;
            }
        }
    }

    DBfree_result(result);

    /* hosts table update */
    db_ret = DBexecute("update hosts set status = %d where hostid = %s and status in(%d, %d)",
                       set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED);

    if (db_ret < ZBX_DB_OK) {
        /* host state update failure */
        ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
        return FAIL;
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
int jarun_icon_zabbixlink_set_host_status(const zbx_uint64_t inner_job_id, const char *set_host_id, const int set_status, const int group_flag)
{
    int ret = SUCCEED;
    int db_ret;
    int status;
    DB_RESULT result;
    char *p_triggerid_sql = NULL;
    char *p_triggerid_strtok = NULL;
    char *p_message = NULL;
    const char *__function_name = "jarun_icon_zabbixlink_set_host_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host_id: %s", __function_name, set_host_id);

    /* host state acquisition */
    status = jarun_icon_zabbixlink_get_host_status(inner_job_id, set_host_id);
    if (status >= SUCCEED) {
        /* host state acquisition success */
        /* host state update necessary */
        if (status != set_status) {
            /* Enabling host */
            if (set_status == ZBXLINK_HOST_STATUS_MONITORED) {
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* Host state update of the host group unit */
                        if (group_flag == ZBXLINK_HOST_GROUP_FLAG_ON) {
                            /* hosts table update */
                            db_ret = DBexecute("update hosts set status = %d"
                                               " where hostid = %s and status in(%d, %d)",
                                               set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED,
                                               ZBXLINK_HOST_STATUS_NOT_MONITORED);

                            if (db_ret < ZBX_DB_OK) {
                                /* hosts table update failure */
                                ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                return FAIL;
                            }
                        }
                        else {
                            db_ret = jarun_icon_zabbixlink_set_host_status_ver18(inner_job_id, set_host_id, set_status);
                            if (db_ret < ZBX_DB_OK) {
                                ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                return FAIL;
                            }
                        }
                        break;

                    case 2:  /* for zabbix 2.0 */
                        /* hosts table update */
                        db_ret = DBexecute("update hosts set status = %d"
                                           " where hostid = %s and status in(%d, %d)",
                                           set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED,
                                           ZBXLINK_HOST_STATUS_NOT_MONITORED);

                        if (db_ret < ZBX_DB_OK) {
                            /* hosts table update failure */
                            ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                            return FAIL;
                        }
                        break;

                    case 3:  /* for zabbix 2.2 */
                        /* hosts table update */
                        db_ret = DBexecute("update hosts set status = %d"
                                           " where hostid = %s and status in(%d, %d) and flags in(%d, %d)",
                                           set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);

                        if (db_ret < ZBX_DB_OK) {
                            /* hosts table update failure */
                            ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                            return FAIL;
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }
            else {
                /* host disabled */
                /* Zabbix version decision */
                switch (CONFIG_ZABBIX_VERSION) {
                    case 1:  /* for zabbix 1.8 */
                        /* Host state update of the host group unit */
                        if (group_flag == ZBXLINK_HOST_GROUP_FLAG_ON) {
                            /* hosts table update */
                            db_ret = DBexecute("update hosts set status = %d"
                                               " where hostid = %s and status in(%d, %d)",
                                               set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED,
                                               ZBXLINK_HOST_STATUS_NOT_MONITORED);

                            if (db_ret < ZBX_DB_OK) {
                                /* hosts table update failure */
                                ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                return FAIL;
                            }
                        }
                        else {
                            db_ret = jarun_icon_zabbixlink_set_host_status_ver18(inner_job_id, set_host_id, set_status);
                            if (db_ret < ZBX_DB_OK) {
                                ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                return FAIL;
                            }
                        }
                        break;

                    case 2:  /* for zabbix 2.0 */
                        /* hosts table search */
                        result = DBselect("select distinct t.triggerid from hosts h, items i, functions f, triggers t"
                                          " where h.hostid = i.hostid and i.itemid = f.itemid and f.triggerid = t.triggerid"
                                          " and h.hostid = %s and h.status = %d and t.value_flags = %d",
                                          set_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_TRIGGER_VALUE_FLAG_NORMAL);

                        /* Triggerid get from the search results */
                        p_triggerid_sql = jarun_icon_zabbixlink_create_csv_string(inner_job_id, result, set_host_id);
                        if (p_triggerid_sql != NULL) {
                            /* Get the message body */
                            p_message = jarun_icon_zabbixlink_get_trigger_message(inner_job_id, "TRG0002");
                            if (p_message == NULL) {
                                DBfree_result(result);
                                zbx_free(p_triggerid_sql);
                                ja_log("JARUNICONZABBIXLINK200024", 0, NULL, inner_job_id, __function_name, inner_job_id, "TRG0002");
                                return FAIL;
                            }

                            /* triggers table update */
                            db_ret = DBexecute("update triggers set value_flags = %d, error = '%s'"
                                               " where triggerid in(%s)",
                                               ZBXLINK_TRIGGER_VALUE_FLAG_UNKNOWN, p_message, p_triggerid_sql);

                            zbx_free(p_message);

                            if (db_ret < ZBX_DB_OK) {
                                /* trigger state update failure */
                                DBfree_result(result);
                                zbx_free(p_triggerid_sql);
                                ja_log("JARUNICONZABBIXLINK200025", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                return FAIL;
                            }

                            /* The record added to the events table */
                            p_triggerid_strtok = strtok(p_triggerid_sql, ",");
                            while (p_triggerid_strtok != NULL) {
                                db_ret = jarun_icon_zabbixlink_insert_events(inner_job_id, p_triggerid_strtok);
                                if (db_ret < ZBX_DB_OK) {
                                    /* The failure to record additional events table */
                                    DBfree_result(result);
                                    zbx_free(p_triggerid_sql);
                                    ja_log("JARUNICONZABBIXLINK200026", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                                    return FAIL;
                                }

                                p_triggerid_strtok = strtok(NULL, ",");
                            }

                            zbx_free(p_triggerid_sql);
                        }
                        DBfree_result(result);

                        /* hosts table update */
                        db_ret = DBexecute("update hosts set status = %d"
                                           " where hostid = %s and status in(%d, %d)",
                                           set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED,
                                           ZBXLINK_HOST_STATUS_NOT_MONITORED);

                        if (db_ret < ZBX_DB_OK) {
                            /* host state update failure */
                            ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                            return FAIL;
                        }
                        break;

                    case 3:  /* for zabbix 2.2 */
                        /* hosts table update */
                        db_ret = DBexecute("update hosts set status = %d"
                                           " where hostid = %s and status in(%d, %d) and flags in(%d, %d)",
                                           set_status, set_host_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED,
                                           ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED);

                        if (db_ret < ZBX_DB_OK) {
                            /* hosts table update failure */
                            ja_log("JARUNICONZABBIXLINK200023", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
                            return FAIL;
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
                        return FAIL;
                }
            }

        }
    }
    else {
        /* host state acquisition failure */
        ja_log("JARUNICONZABBIXLINK200022", 0, NULL, inner_job_id, __function_name, inner_job_id, set_host_id);
        ret = FAIL_STATUS;
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
char *jarun_icon_zabbixlink_create_csv_string(const zbx_uint64_t inner_job_id, DB_RESULT result, const char *log_id)
{
    DB_ROW row;
    zbx_uint64_t p_csv_string_len;
    zbx_uint64_t p_csv_string_buff_size;
    char *p_csv_string = NULL;
    const char *__function_name = "jarun_icon_zabbixlink_create_csv_string";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() id: %s", __function_name, log_id);

    if (NULL != (row = DBfetch(result))) {
        /* Dynamic ensure a buffer to store the id */
        p_csv_string = zbx_calloc(p_csv_string, 1024, sizeof(char));
        if (p_csv_string == NULL) {
            ja_log("JARUNICONZABBIXLINK200027", 0, NULL, inner_job_id, __function_name, inner_job_id, log_id);
            return NULL;
        }
        p_csv_string_buff_size = 1024;

        p_csv_string_len = strlen(row[0]);
        zbx_strlcat(p_csv_string, row[0], p_csv_string_buff_size);

        while (NULL != (row = DBfetch(result))) {
            /* In addition id, ',' the size of the component */
            p_csv_string_len += (strlen(row[0]) + 1);
            /* Re-allocation buffer if there are not enough */
            if (p_csv_string_buff_size <= p_csv_string_len) {
                p_csv_string = zbx_realloc(p_csv_string, 1024);
                if (p_csv_string == NULL) {
                    ja_log("JARUNICONZABBIXLINK200027", 0, NULL, inner_job_id, __function_name, inner_job_id, log_id);
                    return NULL;
                }
                p_csv_string_buff_size += 1024;
            }

            zbx_strlcat(p_csv_string, ",", p_csv_string_buff_size);
            zbx_strlcat(p_csv_string, row[0], p_csv_string_buff_size);
        }
    }

    return p_csv_string;
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
int jarun_icon_zabbixlink_set_host_group_status(const zbx_uint64_t inner_job_id, const char *set_group_id, const int set_status)
{
    int ret = SUCCEED;
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    char *host_id;
    const char *__function_name = "jarun_icon_zabbixlink_set_host_group_status";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() set_group_id: %s", __function_name, set_group_id);

    /* Zabbix version decision */
    switch (CONFIG_ZABBIX_VERSION) {
        case 1:  /* for zabbix 1.8 */
        case 2:  /* for zabbix 2.0 */
            /* DB Search */
            result = DBselect("select distinct h.hostid from hosts h, hosts_groups hg"
                              " where hg.groupid = %s and hg.hostid = h.hostid and h.status in(%d, %d)",
                              set_group_id, ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED);

            while (NULL != (row = DBfetch(result))) {
                host_id = row[0];
                db_ret = jarun_icon_zabbixlink_set_host_status(inner_job_id, host_id, set_status, ZBXLINK_HOST_GROUP_FLAG_ON);
                if (db_ret == FAIL_STATUS) {
                    /* To continue the process to target the next host in the case of abnormal state acquisition */
                    continue;
                }
                else if (db_ret < ZBX_DB_OK) {
                    ja_log("JARUNICONZABBIXLINK200028", 0, NULL, inner_job_id, __function_name, inner_job_id, set_group_id);
                    DBfree_result(result);
                    return FAIL;
                }
            }
            break;

        case 3:  /* for zabbix 2.2 */
            /* DB Search */
            result = DBselect("select distinct h.hostid from hosts h, hosts_groups hg"
                              " where h.flags in(%d, %d) and hg.groupid = %s"
                              " and hg.hostid = h.hostid and h.status in(%d, %d)",
                              ZBXLINK_FLAG_DISCOVERY_NORMAL, ZBXLINK_FLAG_DISCOVERY_CREATED, set_group_id,
                              ZBXLINK_HOST_STATUS_MONITORED, ZBXLINK_HOST_STATUS_NOT_MONITORED);

            while (NULL != (row = DBfetch(result))) {
                host_id = row[0];
                db_ret = jarun_icon_zabbixlink_set_host_status(inner_job_id, host_id, set_status, ZBXLINK_HOST_GROUP_FLAG_ON);
                if (db_ret == FAIL_STATUS) {
                    /* To continue the process to target the next host in the case of abnormal state acquisition */
                    continue;
                }
                else if (db_ret < ZBX_DB_OK) {
                    ja_log("JARUNICONZABBIXLINK200028", 0, NULL, inner_job_id, __function_name, inner_job_id, set_group_id);
                    DBfree_result(result);
                    return FAIL;
                }
            }
            break;

        default:
            ja_log("JARUNICONZABBIXLINK200001", 0, NULL, inner_job_id, CONFIG_ZABBIX_VERSION);
            return FAIL;
    }

    DBfree_result(result);

    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_zabbixlink_check_access_permission                    *
 *                                                                            *
 * Purpose: check the write permission to the host group                      *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id    (in) - inner job id                            *
 *             group_id        (in) - host group id                           *
 *                                                                            *
 * Return value:  SUCCEED - accessible                                        *
 *                FAIL    - inaccessible or an error occurred                 *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_zabbixlink_check_access_permission(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, const char *group_id)
{
    DB_RESULT result;
    DB_ROW row;
    ja_icon_info_t icon;
    int res;
    int permission;
    char *host_group_name;
    char *execution_user_name_esc = NULL;
    const char *__function_name = "jarun_icon_zabbixlink_check_access_permission";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host_group_id: %s", __function_name, group_id);

    /* get the execution user name */
    result = DBselect("select execution_user_name from ja_run_jobnet_table"
                      " where inner_jobnet_id = " ZBX_FS_UI64,
                      inner_jobnet_id);

    while (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONZABBIXLINK200033", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_jobnet_id);
        DBfree_result(result);
        return FAIL;
    }

    execution_user_name_esc = DBdyn_escape_string(row[0]);
    DBfree_result(result);

    result = DBselect("select r.type from users u,role r where u.username = '%s' and r.roleid = u.roleid",execution_user_name_esc);
    if (NULL != (row = DBfetch(result))) {
    	/* Zabbix Super Admin check */
    	if(atoi(row[0]) == 3 ){
    		DBfree_result(result);
    		return SUCCEED;
    	}
    }else{
    	zabbix_log(LOG_LEVEL_ERR, "In %s() not found user in zabbix : %s", __function_name, execution_user_name_esc);
    	DBfree_result(result);
    	return FAIL;
    }

    res = 0;
    /* get access privileges on the host group */
    result = DBselect("select c.permission from users a, users_groups b, rights c"
                      " where c.id = %s and b.usrgrpid = c.groupid and a.userid = b.userid and a.username = '%s'",
                      group_id, execution_user_name_esc);

    while (NULL != (row = DBfetch(result))) {
        permission = atoi(row[0]);
        /* deny ? */
        if (permission == 0) {
            res = 0;
            break;
        }
        /* read-write ? */
        if (permission == 3) {
            res = 1;
        }
    }

    DBfree_result(result);
    zbx_free(execution_user_name_esc);

    /* inaccessible ? */
    if (res == 0) {
        host_group_name = "";
        result = DBselect("select name from hstgrp where groupid = %s", group_id);
        if (NULL != (row = DBfetch(result))) {
            host_group_name = row[0];
        }
        ja_get_icon_info(inner_job_id, &icon);
        ja_log("JARUNICONZABBIXLINK200034", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id,
               host_group_name, icon.main_jobnet_id, icon.job_id, icon.execution_user_name);
        DBfree_result(result);
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
int jarun_icon_zabbixlink(const zbx_uint64_t inner_job_id, const int test_flag)
{
    int func_ret = FAIL;
    int link_target;
    int link_operation;
    zbx_uint64_t inner_jobnet_id;
    DB_RESULT result;
    DB_ROW row;
    char str_func_ret[4];
    char *group_id;
    char *host_id;
    char *item_id;
    char *trigger_id;
    const char *__function_name = "jarun_icon_zabbixlink";

    /* Log output */
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* DB Search */
    result = DBselect("select inner_jobnet_id, link_target, link_operation, groupid, hostid, itemid, triggerid"
                      " from ja_run_icon_zabbix_link_table"
                      " where inner_job_id = " ZBX_FS_UI64,
                      inner_job_id);

    if (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        link_target    = atoi(row[1]);
        link_operation = atoi(row[2]);
        group_id       = row[3];
        host_id        = row[4];
        item_id        = row[5];
        trigger_id     = row[6];

        /* Cooperation target determining */
        switch (link_target) {
            case 0:  /* Host group */
                /* Work together decision */
                switch (link_operation) {
                    case 0:  /* Activation */
                    case 1:  /* Invalidation */
                        if (test_flag == JA_JOB_TEST_FLAG_ON) {
                            func_ret = SUCCEED;
                        }
                        else {
                            func_ret = jarun_icon_zabbixlink_check_access_permission(inner_jobnet_id, inner_job_id, group_id);
                            if (func_ret == SUCCEED) {
                                func_ret = jarun_icon_zabbixlink_set_host_group_status(inner_job_id, group_id, link_operation);
                            }
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200029", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id, link_operation);
                }
                break;

            case 1:  /* Host */
                /* Work together decision */
                switch (link_operation) {
                    case 0:  /* Activation */
                    case 1:  /* Invalidation */
                        if (test_flag == JA_JOB_TEST_FLAG_ON) {
                            func_ret = SUCCEED;
                        }
                        else {
                            func_ret = jarun_icon_zabbixlink_check_access_permission(inner_jobnet_id, inner_job_id, group_id);
                            if (func_ret == SUCCEED) {
                                func_ret = jarun_icon_zabbixlink_set_host_status(inner_job_id, host_id, link_operation, ZBXLINK_HOST_GROUP_FLAG_OFF);
                            }
                        }
                        break;

                    case 2:  /* State acquisition */
                        func_ret = jarun_icon_zabbixlink_get_host_status(inner_job_id, host_id);
                        if (func_ret >= SUCCEED) {
                            /* Job Controller variable settings */
                            zbx_snprintf(str_func_ret, sizeof(str_func_ret), "%d", func_ret);
                            ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LAST_STATUS, str_func_ret);
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200029", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id, link_operation);
                }
                break;

            case 2:  /* Item */
                /* Work together decision */
                switch (link_operation) {
                    case 0:  /* Activation */
                    case 1:  /* Invalidation */
                        if (test_flag == JA_JOB_TEST_FLAG_ON) {
                            func_ret = SUCCEED;
                        }
                        else {
                            func_ret = jarun_icon_zabbixlink_check_access_permission(inner_jobnet_id, inner_job_id, group_id);
                            if (func_ret == SUCCEED) {
                                func_ret = jarun_icon_zabbixlink_set_item_status(inner_job_id, item_id, link_operation);
                            }
                        }
                        break;

                    case 2:  /* State acquisition */
                        func_ret = jarun_icon_zabbixlink_get_item_status(inner_job_id, item_id);
                        if (func_ret >= SUCCEED) {
                            /* Job Controller variable settings */
                            zbx_snprintf(str_func_ret, sizeof(str_func_ret), "%d", func_ret);
                            ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LAST_STATUS, str_func_ret);
                        }
                        break;

                    case 3:  /* Data acquisition */
                        func_ret = jarun_icon_zabbixlink_get_item_data(inner_jobnet_id, inner_job_id, item_id);
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200029", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id, link_operation);
                }
                break;

            case 3:  /* Trigger */
                /* Work together decision */
                switch (link_operation) {
                    case 0:  /* Activation */
                    case 1:  /* Invalidation */
                        if (test_flag == JA_JOB_TEST_FLAG_ON) {
                            func_ret = SUCCEED;
                        }
                        else {
                            func_ret = jarun_icon_zabbixlink_check_access_permission(inner_jobnet_id, inner_job_id, group_id);
                            if (func_ret == SUCCEED) {
                                func_ret = jarun_icon_zabbixlink_set_trigger_status(inner_job_id, trigger_id, link_operation);
                            }
                        }
                        break;

                    case 2:  /* State acquisition */
                        func_ret = jarun_icon_zabbixlink_get_trigger_status(inner_job_id, trigger_id);
                        zabbix_log(LOG_LEVEL_INFORMATION,"Park.iggy 11111 [%d]",func_ret);
                        if (func_ret >= SUCCEED) {
                            /* Job Controller variable settings */
                            zbx_snprintf(str_func_ret, sizeof(str_func_ret), "%d", func_ret);
                            ja_set_value_after(inner_job_id, inner_jobnet_id, ZBXLINK_ZBX_LAST_STATUS, str_func_ret);
                        }
                        break;

                    default:
                        ja_log("JARUNICONZABBIXLINK200029", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id, link_operation);
                }
                break;

            default:
                ja_log("JARUNICONZABBIXLINK200030", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id, link_target);
        }
    }
    else {
        ja_log("JARUNICONZABBIXLINK200031", inner_jobnet_id, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }

    DBfree_result(result);

    if (func_ret < SUCCEED) {
        return ja_set_runerr(inner_job_id, 2);
    }

    /* Set the status to start the next job */
    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}

