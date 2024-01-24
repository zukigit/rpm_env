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
#include "threads.h"
#include "log.h"
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastr.h"
#include "jaenv.h"
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
int jarun_icon_calc(const zbx_uint64_t inner_job_id)
{
    DB_RESULT result;
    DB_ROW row;
    zbx_uint64_t inner_jobnet_id;
    int chk;
    pid_t pid;
    int exit_code;
    FILE *fp;
    char value_name[JA_MAX_STRING_LEN], cmd[JA_MAX_STRING_LEN],
        value[JA_MAX_DATA_LEN];
    const char *__function_name = "jarun_icon_calc";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    inner_jobnet_id = 0;
    result =
        DBselect
        ("select inner_jobnet_id, hand_flag, formula, value_name from ja_run_icon_calc_table"
         " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONCALC200001", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    chk = FAIL;
    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    if (atoi(row[1]) == 0) {
        chk = ja_format_expr(row[2], cmd);
    } else {
        chk = ja_format_date(row[2], cmd);
    }
    zbx_snprintf(value_name, sizeof(value_name), "%s", row[3]);
    DBfree_result(result);

    if (chk != SUCCEED)
        return ja_set_runerr(inner_job_id, 2);

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() command: [%s]",
               __function_name, cmd);

    exit_code = -1;
    signal(SIGCHLD, SIG_DFL);
    pid = zbx_child_fork();
    if (pid == -1) {
        ja_log("JARUNICONCALC200002", inner_jobnet_id, NULL, inner_job_id,
               __function_name);
        return ja_set_runerr(inner_job_id, 2);
    } else if (pid == 0) {
        if (ja_setenv(inner_job_id) == FAIL)
            exit(-2);
        if ((fp = popen(cmd, "r")) == NULL) {
            ja_log("JARUNICONCALC200003", inner_jobnet_id, NULL,
                   inner_job_id, __function_name);
            exit(-3);
        }
        memset(value, 0, JA_MAX_DATA_LEN);
        if (fread(value, sizeof(char), JA_MAX_DATA_LEN, fp) == 0) {
            ja_log("JARUNICONCALC200004", inner_jobnet_id, NULL,
                   inner_job_id, __function_name, cmd);
            pclose(fp);
            exit(-4);
        }
        zbx_rtrim(value, "\n");
        ja_set_value_after(inner_job_id, inner_jobnet_id, value_name,
                           value);
        pclose(fp);
        exit(0);
    }

    waitpid(pid, &exit_code, WUNTRACED);
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() go back parent process. exit_code: %d",
               __function_name, exit_code);

    if (exit_code != 0) {
        ja_log("JARUNICONCALC200005", inner_jobnet_id, NULL, inner_job_id,
               __function_name, exit_code);
        return ja_set_runerr(inner_job_id, 2);
    }

    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}
