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
#include "log.h"
#include "db.h"
#include "jacommon.h"
#include "jalog.h"
#include "jarunsessioncheck.h"
#include "jakill.h"
#include "jastatus.h"

void jarun_session_check(int *pid_list, int *count_pid) {
    const char *__function_name = "jarun_session_check";
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_job_id;
    char *JOBARG_REGISTRY_NUMBER;
    char *JOBARG_SESSION_ID;

    for(int i =0; i < count_pid; i++) {
        if(ja_process_check(pid_list[i]) == FAIL) {

            result = DBselect("select inner_job_id, inner_jobnet_main_id, session_id from ja_session_table where pid = %d", pid_list[i]);
            while(NULL != (row = DBfetch(result))) {
                ZBX_STR2UINT64(inner_job_id, row[0]);
                JOBARG_REGISTRY_NUMBER = row[1];
                JOBARG_SESSION_ID = row[2];
            }

            if(ja_set_runerr(inner_job_id, 2) == SUCCEED) {
                DBexecute(" delete from ja_session_table"
                          " where session_id = '%s' and inner_jobnet_main_id = %s",
                            JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);
            }

            ja_log("JASESSIONCHCK000001", JOBARG_REGISTRY_NUMBER, NULL, inner_job_id, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER, pid_list[i], inner_job_id);
            DBfree_result(result);
        }
    }
}