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
#include "javalue.h"
#include "jastatus.h"
#include "jaflow.h"

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
int ja_flow_set_status(const zbx_uint64_t start_inner_job_id,
                       const zbx_uint64_t end_inner_job_id)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t link_inner_jobnet_id;
    int status, upd_status, test_flag, job_type;
    int boot_count, end_count;
    const char *__function_name = "ja_flow_set_status";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() start_inner_job_id: " ZBX_FS_UI64
               ", end_inner_job_id: " ZBX_FS_UI64, __function_name,
               start_inner_job_id, end_inner_job_id);

    result =
        DBselect
        ("select status, test_flag, job_type, boot_count, end_count"
         " from ja_run_job_table " " where inner_job_id = " ZBX_FS_UI64
         " for update ",
         end_inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAFLOW200001", 0, NULL, start_inner_job_id,
               __function_name, end_inner_job_id);
        DBfree_result(result);
        return ja_set_enderr(start_inner_job_id, 1);
    }

    status = atoi(row[0]);
    test_flag = atoi(row[1]);
    job_type = atoi(row[2]);
    boot_count = atoi(row[3]);
    end_count = atoi(row[4]);
    DBfree_result(result);

    upd_status = JA_JOB_STATUS_BEGIN;
    // reset job status
    if (status == JA_JOB_STATUS_END || status == JA_JOB_STATUS_RUNERR) {
        if (test_flag == JA_JOB_TEST_FLAG_ON) {
            return SUCCEED;
        }
        // can not reset end icon status
        if (job_type == JA_JOB_TYPE_END) {
            ja_log("JAFLOW300001", 0, NULL, start_inner_job_id,
                   __function_name, end_inner_job_id);
            return ja_set_enderr(start_inner_job_id, 1);
        } else {
            status = JA_JOB_STATUS_BEGIN;
            end_count = 0;
        }
    }
    // count up
    if (status == JA_JOB_STATUS_BEGIN) {
        end_count++;
        if (job_type == JA_JOB_TYPE_IFEND || job_type == JA_JOB_TYPE_L) {
            upd_status = JA_JOB_STATUS_READY;
        } else {
            if (boot_count <= end_count) {
                upd_status = JA_JOB_STATUS_READY;
            }
            else {
                upd_status = JA_JOB_STATUS_BEGIN;
            }
        }
    } else {
        if (test_flag == JA_JOB_TEST_FLAG_ON) {
            return SUCCEED;
        }
        ja_log("JAFLOW200002", 0, NULL, start_inner_job_id,
               __function_name, end_inner_job_id, status);
        return ja_set_enderr(start_inner_job_id, 1);
    }

    // reset status for all icons of the jobnet
    if (job_type == JA_JOB_TYPE_JOBNET
        && upd_status == JA_JOB_STATUS_READY) {
        link_inner_jobnet_id = 0;
        result =
            DBselect
            ("select link_inner_jobnet_id from ja_run_icon_jobnet_table"
             " where inner_job_id = " ZBX_FS_UI64, end_inner_job_id);

        if (NULL != (row = DBfetch(result))) {
            ZBX_STR2UINT64(link_inner_jobnet_id, row[0]);
        } else {
            ja_log("JAFLOW200004", 0, NULL, start_inner_job_id,
                   __function_name, end_inner_job_id);
            DBfree_result(result);
            return ja_set_enderr(start_inner_job_id, 1);
        }
        DBfree_result(result);

        if (ZBX_DB_OK >
            DBexecute
            ("update ja_run_job_table set status = %d, end_count = 0"
             " where inner_jobnet_id = " ZBX_FS_UI64, JA_JOB_STATUS_BEGIN,
             link_inner_jobnet_id)) {
            return FAIL;
        }
    }
    // set status and end_count
    if (ZBX_DB_OK >
        DBexecute("update ja_run_job_table set status = %d, end_count = %d"
                  " where inner_job_id = " ZBX_FS_UI64, upd_status,
                  end_count, end_inner_job_id)) {
        return FAIL;
    }
    // set env value
    return ja_value_after_before(start_inner_job_id, end_inner_job_id);
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
int ja_flow(const zbx_uint64_t inner_job_id, const int type, int msg_flag)
{
    zbx_uint64_t inner_jobnet_id, end_inner_job_id;
    int flows, flow_type, jobnet_status;
    DB_RESULT result;
    DB_RESULT count_result;
    DB_ROW row;
    const char *__function_name = "ja_flow";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", type: %d",
               __function_name, inner_job_id, type);

    if (ja_clean_value_after(inner_job_id) == FAIL)
        return FAIL;

    result =
        DBselect
        ("select inner_jobnet_id, end_inner_job_id, flow_type from ja_run_flow_table"
         " where start_inner_job_id = " ZBX_FS_UI64 " for update ", inner_job_id);

    if(result == NULL) {
        ja_log("JAFLOW200006", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }

    flows = 0;
    while (NULL != (row = DBfetch(result))) {
        ZBX_STR2UINT64(inner_jobnet_id, row[0]);
        ZBX_STR2UINT64(end_inner_job_id, row[1]);
        flow_type = atoi(row[2]);
        zabbix_log(LOG_LEVEL_DEBUG,
                   "In %s() start_inner_job_id: " ZBX_FS_UI64
                   ", end_inner_job_id: " ZBX_FS_UI64
                   ", flow_type: %d,  type: %d", __function_name,
                   inner_job_id, end_inner_job_id, flow_type, type);

        flows++;
        if (flows == 1) {
            jobnet_status = ja_get_status_jobnet(inner_jobnet_id);
            if (jobnet_status != JA_JOBNET_STATUS_RUN
                && jobnet_status != JA_JOBNET_STATUS_RUNERR) {
                ja_log("JAFLOW200005", inner_jobnet_id, NULL, inner_job_id,
                       __function_name, jobnet_status, inner_jobnet_id);
                return ja_set_runerr(inner_job_id, -99);
            }
        }
        if (flow_type == type
            || (type == JA_FLOW_TYPE_NORMAL
                && (flow_type == JA_FLOW_TYPE_TRUE
                    || flow_type == JA_FLOW_TYPE_FALSE))) {
            if (ja_flow_set_status(inner_job_id, end_inner_job_id) == FAIL) {
                DBfree_result(result);
                return FAIL;
            }
        }
    }
    DBfree_result(result);

    if (flows == 0) {
        ja_log("JAFLOW200003", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id);
        return ja_set_enderr(inner_job_id, 1);
    } else {
        return ja_set_end(inner_job_id, msg_flag);
    }

    return SUCCEED;
}
