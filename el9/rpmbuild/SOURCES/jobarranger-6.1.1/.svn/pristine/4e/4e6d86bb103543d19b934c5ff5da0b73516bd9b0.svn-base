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
#include "jastatus.h"
#include "jalog.h"

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
int jajobnet_summary_ready(const zbx_uint64_t inner_jobnet_id)
{
    const char *__function_name = "jajobnet_summary_ready";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64, __function_name,
               inner_jobnet_id);

    if (ja_set_status_jobnet(inner_jobnet_id, JA_JOBNET_STATUS_READY, 0, 0)
        == FAIL)
        return FAIL;

    if (ja_set_status_jobnet_summary
        (inner_jobnet_id, JA_JOBNET_STATUS_RUN, 1, 0) == FAIL)
        return FAIL;

    return SUCCEED;
}
