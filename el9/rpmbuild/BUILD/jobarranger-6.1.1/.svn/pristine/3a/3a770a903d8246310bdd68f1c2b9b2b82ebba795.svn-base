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
#include "jastr.h"
#include "jastatus.h"
#include "jaflow.h"
#include "javalue.h"
#include "jalog.h"
#include "jarunnormal.h"

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
int jarun_icon_if_comp(const int hand_flag, const char *value,
                       const char *comparison_value)
{
    int ret;
    const char *__function_name = "jarun_icon_if_comp";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() hand_flag: %d, value: %s, comparison_value: %s",
               __function_name, hand_flag, value, comparison_value);

    ret = -1;
    switch (hand_flag) {
    case 0:
        ret = ja_number_match(value, comparison_value);
        break;
    case 1:
        if (NULL == zbx_regexp_match(value, comparison_value, NULL))
            ret = 0;
        else
            ret = 1;
        break;
    default:
        ja_log("JARUNICONIF200001", 0, NULL, 0, __function_name,
               hand_flag);
        break;
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
int jarun_icon_if(const zbx_uint64_t inner_job_id, const int test_flag)
{
    DB_RESULT result;
    DB_ROW row;
    int comp, flow_type;
    const char *__function_name = "jarun_icon_if";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    result =
        DBselect
        ("select ji.hand_flag, ji.comparison_value, jb.before_value"
         " from ja_run_icon_if_table ji, ja_run_value_before_table jb"
         " where ji.inner_job_id = " ZBX_FS_UI64
         " and ji.inner_job_id = jb.inner_job_id and ji.value_name = jb.value_name",
         inner_job_id);

    flow_type = -1;
    if (NULL != (row = DBfetch(result))) {
        if (row[0] == NULL || row[1] == NULL || row[2] == NULL) {
            ja_log("JARUNICONIF200002", 0, NULL, inner_job_id,
                   __function_name, inner_job_id);
        } else {
            comp = jarun_icon_if_comp(atoi(row[0]), row[2], row[1]);
            if (comp == 0) {
                flow_type = JA_FLOW_TYPE_FALSE;
            } else if (comp == 1) {
                flow_type = JA_FLOW_TYPE_TRUE;
            } else {
                ja_log("JARUNICONIF200004", 0, NULL, inner_job_id,
                       __function_name, row[0], row[1], row[2], inner_job_id);
            }
        }
    } else {
        ja_log("JARUNICONIF200003", 0, NULL, inner_job_id, __function_name,
               inner_job_id);
    }

    DBfree_result(result);

    if (flow_type == -1) {
        return ja_set_runerr(inner_job_id, 2);
    }

    if (test_flag == JA_JOB_TEST_FLAG_ON) {
        return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
    }

    return ja_flow(inner_job_id, flow_type, 1);
}
