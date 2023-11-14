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
#include "javalue.h"
#include "jarunvalue.h"
#include "jastatus.h"
#include "jaflow.h"
#include "jalog.h"
#include "jajoblog.h"

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
int jarun_skip(const zbx_uint64_t inner_job_id,
               const zbx_uint64_t inner_jobnet_id, const int job_type)
{
    const char *__function_name = "jarun_skip";
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", job_type: %d",
               __function_name, inner_job_id, job_type);

    if (jarun_value_before(inner_job_id) == FAIL)
        return FAIL;

    if (ja_joblog(JC_JOB_SKIP, 0, inner_job_id) == FAIL)
        return FAIL;

    if (ja_value_before_after(inner_job_id) == FAIL)
        return FAIL;

    if (job_type == JA_JOB_TYPE_START || job_type == JA_JOB_TYPE_END
        || job_type == JA_JOB_TYPE_M || job_type == JA_JOB_TYPE_W
        || job_type == JA_JOB_TYPE_IF || job_type == JA_JOB_TYPE_IFEND
        || job_type == JA_JOB_TYPE_L) {
        ja_log("JARUNSKIP200001", inner_jobnet_id, NULL, inner_job_id,
               __function_name, job_type, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}
