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
#include "jarunvalue.h"
#include "jastatus.h"
#include "jaruniconcommon.h"
#include "jaruniconend.h"
#include "jaruniconif.h"
#include "jaruniconvalue.h"
#include "jaruniconjob.h"
#include "jaruniconjobnet.h"
#include "jaruniconextjob.h"
#include "jaruniconcalc.h"
#include "jaruniconinfo.h"
#include "jarunicontask.h"
#include "jaruniconfcopy.h"
#include "jaruniconfwait.h"
#include "jaruniconreboot.h"
#include "jaruniconrelease.h"
#include "jaruniconless.h"
#include "jaruniconzabbixlink.h"
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
int jarun_normal(zbx_uint64_t inner_job_id, int job_type, int test_flag)
{
    int ret;
    const char *__function_name = "jarun_normal";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64
               ", job_type: %d, test_flag: %d", __function_name,
               inner_job_id, job_type, test_flag);

    if (jarun_value_before(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (ja_set_run(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (ja_set_run_job_id(inner_job_id) == FAIL) {
        return FAIL;
    }

    if (ja_value_before_after(inner_job_id) == FAIL) {
        return FAIL;
    }

    ret = SUCCEED;
    switch (job_type) {
    case JA_JOB_TYPE_START:
        ret = jarun_icon_start(inner_job_id);
        break;
    case JA_JOB_TYPE_END:
        ret = jarun_icon_end(inner_job_id);
        break;
    case JA_JOB_TYPE_IF:
        ret = jarun_icon_if(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_VALUE:
        ret = jarun_icon_value(inner_job_id);
        break;
    case JA_JOB_TYPE_JOB:
        if (test_flag == JA_JOB_TEST_FLAG_ON) {
            ret = jarun_icon_job(inner_job_id, JA_AGENT_METHOD_TEST);
        }
        else {
            ret = jarun_icon_job(inner_job_id, JA_AGENT_METHOD_NORMAL);
        }
        break;
    case JA_JOB_TYPE_JOBNET:
        ret = jarun_icon_jobnet(inner_job_id);
        break;
    case JA_JOB_TYPE_M:
        ret = jarun_icon_m(inner_job_id);
        break;
    case JA_JOB_TYPE_W:
        ret = jarun_icon_w(inner_job_id);
        break;
    case JA_JOB_TYPE_L:
        ret = jarun_icon_l(inner_job_id);
        break;
    case JA_JOB_TYPE_EXTJOB:
        ret = jarun_icon_extjob(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_CALC:
        ret = jarun_icon_calc(inner_job_id);
        break;
    case JA_JOB_TYPE_TASK:
        ret = jarun_icon_task(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_INFO:
        ret = jarun_icon_info(inner_job_id);
        break;
    case JA_JOB_TYPE_IFEND:
        ret = jarun_icon_ifend(inner_job_id);
        break;
    case JA_JOB_TYPE_FCOPY:
        ret = jarun_icon_fcopy(inner_job_id, JA_AGENT_METHOD_NORMAL);
        break;
    case JA_JOB_TYPE_FWAIT:
        ret = jarun_icon_fwait(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_REBOOT:
        ret = jarun_icon_reboot(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_REL:
        ret = jarun_icon_release(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_LESS:
        ret = jarun_icon_less(inner_job_id, test_flag);
        break;
    case JA_JOB_TYPE_LINK:
        ret = jarun_icon_zabbixlink(inner_job_id, test_flag);
        break;
    default:
        ja_log("JARUNNORMAL200001", 0, NULL, inner_job_id, __function_name, job_type, inner_job_id);
        ret = ja_set_runerr(inner_job_id, 2);
        break;
    }

    return ret;
}
