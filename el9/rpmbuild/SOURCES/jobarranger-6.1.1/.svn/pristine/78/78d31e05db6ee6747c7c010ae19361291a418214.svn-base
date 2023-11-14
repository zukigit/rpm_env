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
#include "jafile.h"
#include "jakill.h"
#include "jajobfile.h"
#include "jajobobject.h"
#include "javalue.h"
#include "jastatus.h"
#include "jalog.h"
#include "jaflow.h"
#include "jajobid.h"
#include "jajobiconextjob.h"

extern char     *CONFIG_TMPDIR;
extern int      CONFIG_EXTJOB_WAITTIME;

/******************************************************************************
 *                                                                            *
 * Function: ja_zbxsender_result_check                                        *
 *                                                                            *
 * Purpose: check the results of the zabbix_sender command execution result   *
 *          file                                                              *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             file_name (in) - zabbix_sender command result file name        *
 *             save_line (out) - save the command output data of previous     *
 *                                                                            *
 * Return value:  0 - normal end                                              *
 *                1 - error occurrence                                        *
 *                2 - running                                                 *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ja_zbxsender_result_check(const zbx_uint64_t inner_job_id, char *file_name, char *save_line)
{
    FILE         *fp;
    int          sw;
    char         line[JA_MAX_DATA_LEN];
    int          i=0,openerrno;
    const char   *__function_name = "ja_zbxsender_result_check";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " file: [%s]", __function_name, inner_job_id, file_name);

    *save_line = '\0';

    while((fp = fopen(file_name, "r")) == NULL)
    {
        openerrno = errno;
        if( i > 2)
        {
            ja_log("JAJOBICONEXTJOB200004", 0, NULL, inner_job_id, __function_name, file_name, strerror(openerrno), inner_job_id, ja_get_jobid(inner_job_id));
            return 1;
        }
        sleep(1);
        i++;
    }

    sw = 0;
    while (fgets(line, sizeof(line), fp) != NULL) {
        /* remove LF character */
        if (strlen(line) > 0) {
            if (line[strlen(line)-1] == '\n') {
                line[strlen(line)-1] = '\0';
            }
        }
        /* remove CR character */
        if (strlen(line) > 0) {
            if (line[strlen(line)-1] == '\r') {
                line[strlen(line)-1] = '\0';
            }
        }

        if (sw == 0) {
            zbx_strlcpy(save_line, line, JA_MAX_DATA_LEN);
            sw = 1;
        }



        /* zabbix_sender command invalid argument ? */
        if (strstr(line, "usage: ") != NULL) {
            ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            fclose(fp);
            return 1;
        }

        /* zabbix_sender command sending failed ? */
        if (strstr(line, "Sending failed.") != NULL) {
            ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            fclose(fp);
            return 1;
        }

        /* zabbix_sender command notification failure ? */
        if (strstr(line, "sent: 0;") != NULL) {
            ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            fclose(fp);
            return 1;
        }

        /* zabbix_sender command notification failure ? */
        if (strstr(line, "processed: 0;") != NULL) {
            ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            fclose(fp);
            return 1;
        }


        /* zabbix_sender command normal end ? */
        if (strstr(line, "sent: ") != NULL) {
            zbx_strlcpy(save_line, line, JA_MAX_DATA_LEN);
            fclose(fp);
            return 0;
        }

        /* zabbix_sender command startup error ? */
        if (strstr(line, "sh: ") != NULL) {
            ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            fclose(fp);
            return 1;
        }

        /* zabbix_sender command invalid argument ? */
        if (strstr(line, ": option") != NULL) {
        	ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
        	fclose(fp);
        	return 1;
        }
        /* zabbix_sender command invalid argument ? */
        if (strstr(line, ": invalid") != NULL) {
        	ja_log("JAJOBICONEXTJOB200005", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
        	fclose(fp);
        	return 1;
        }

        zbx_strlcpy(save_line, line, JA_MAX_DATA_LEN);
    }

    fclose(fp);
    return 2;
}

/******************************************************************************
 *                                                                            *
 * Function: jajob_icon_extjob                                                *
 *                                                                            *
 * Purpose: monitor the end of expanded job icon                              *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             inner_jobnet_id (in) - inner jobnet id                         *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jajob_icon_extjob(const zbx_uint64_t inner_job_id, const zbx_uint64_t inner_jobnet_id)
{
    DB_RESULT    result;
    DB_ROW       row;
    struct tm    *tm;
    time_t       now;
    int          wait_count, rc;
    char         wait_time[16], now_time[16];
    char         command_id[JA_COMMAND_ID_LEN], file_name[JA_MAX_STRING_LEN], save_line[JA_MAX_DATA_LEN];
    char         result_data[JA_STD_OUT_LEN];
    const char   *__function_name = "jajob_icon_extjob";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get expanded job icon information */
    result = DBselect("select command_id, wait_count, wait_time from ja_run_icon_extjob_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONEXTJOB200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    zbx_strlcpy(command_id, row[0], sizeof(command_id));
    wait_count = atoi(row[1]);

    wait_time[0] = '\0';
    if (SUCCEED != DBis_null(row[2])) {
        zbx_strlcpy(wait_time,  row[2], sizeof(wait_time));
    }

    DBfree_result(result);

    if (strcmp(command_id, JA_CMD_WEEK) == 0) {
        /* unchecked */
        return SUCCEED;
    }

    if (strcmp(command_id, JA_CMD_ZBXSENDER) == 0) {
        memset(result_data, '\0', sizeof(result_data));

        /* command result file name edit */
        zbx_snprintf(file_name, sizeof(file_name), "%s/%s." ZBX_FS_UI64, CONFIG_TMPDIR, JA_EXTJOB_RESULT_FILE, inner_job_id);

        rc = ja_zbxsender_result_check(inner_job_id, file_name, save_line);
        switch (rc) {
            case 0:  /* normal end */
                 ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", "0");
                 ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", save_line);
                 ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", "");
                 ja_file_remove(file_name);
                 return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);

            case 1:  /* error occurrence */
                 ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", "255");
                 ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", "");
                 if (SUCCEED == ja_file_load(file_name, 0, result_data)) {
                     ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", result_data);
                 }
                 else {
                     ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", "");
                 }
                 ja_file_remove(file_name);
                 return ja_set_runerr(inner_job_id, 2);
        }

        if (wait_count >= CONFIG_EXTJOB_WAITTIME) {
            ja_log("JAJOBICONEXTJOB200002", 0, NULL, inner_job_id, __function_name, save_line, inner_job_id, ja_get_jobid(inner_job_id));
            ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", "255");
            ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", "");
            if (SUCCEED == ja_file_load(file_name, 0, result_data)) {
                ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", result_data);
            }
            else {
                ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", "");
            }
            ja_file_remove(file_name);
            return ja_set_runerr(inner_job_id, 2);
        }

        /* record locking */
        DBfree_result(DBselect("select wait_count from ja_run_icon_extjob_table"
                               " where inner_job_id = " ZBX_FS_UI64 " for update", inner_job_id));

        /* zabbix_sender completion waiting times count up */
        wait_count = wait_count + 1;
        rc = DBexecute("update ja_run_icon_extjob_table set wait_count = %d"
                       " where inner_job_id = " ZBX_FS_UI64,
                       wait_count, inner_job_id);

        if (rc < ZBX_DB_OK) {
            ja_log("JAJOBICONEXTJOB200003", 0, NULL, inner_job_id, __function_name, wait_count, inner_job_id);
            return ja_set_runerr(inner_job_id, 2);
        }

        return SUCCEED;
    }

    if (strcmp(command_id, JA_CMD_SLEEP) == 0 || strcmp(command_id, JA_CMD_TIME) == 0) {
        time(&now);
        tm = localtime(&now);
        strftime(now_time, sizeof(now_time), "%Y%m%d%H%M%S", tm);

        zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= DATE CHECK wait_time[%s] now_time[%s]", wait_time, now_time);

        if (strcmp(wait_time, now_time) > 0) {
            return SUCCEED;
        }
        return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jajob_icon_extjob_kill                                           *
 *                                                                            *
 * Purpose: abort the expanded job icon                                       *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jajob_icon_extjob_kill(const zbx_uint64_t inner_job_id)
{
    DB_RESULT    result;
    DB_ROW       row;
    char         command_id[JA_COMMAND_ID_LEN], file_name[JA_MAX_STRING_LEN];
    const char   *__function_name = "jajob_icon_extjob_kill";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get expanded job icon information */
    result = DBselect("select command_id from ja_run_icon_extjob_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAJOBICONEXTJOB200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    zbx_strlcpy(command_id, row[0], sizeof(command_id));
    DBfree_result(result);

    if (strcmp(command_id, JA_CMD_WEEK) == 0) {
        return SUCCEED;
    }

    if (strcmp(command_id, JA_CMD_ZBXSENDER) == 0) {
        zbx_snprintf(file_name, sizeof(file_name), "%s/%s." ZBX_FS_UI64, CONFIG_TMPDIR, JA_EXTJOB_RESULT_FILE, inner_job_id);
        ja_file_remove(file_name);
    }

    return ja_set_runerr(inner_job_id, 2);

}
