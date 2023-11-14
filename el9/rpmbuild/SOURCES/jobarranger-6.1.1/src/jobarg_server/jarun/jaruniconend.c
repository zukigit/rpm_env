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
#include "jaruniconend.h"
extern char *CONFIG_LOG_FILE;

/******************************************************************************
 *                                                                            *
 * Function: ja_stop_code_conv                                                *
 *                                                                            *
 * Purpose: convert to a number stop code                                     *
 *                                                                            *
 * Parameters: str (in) - stop code of string                                 *
 *                                                                            *
 * Return value: stop code of number (0 - 255)                                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_stop_code_conv(char *str)
{
    int        sw, cnt, num;
    char       *p_str;
    const char *__function_name = "ja_stop_code_conv";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s))", __function_name, str);

    if ('\0' == *str) {
        return 0;
    }

    sw    = 0;
    cnt   = 0;
    p_str = str;

    while ('\0' != *p_str) {
        if (0 == isdigit(*p_str)) {
            return 0;  /* not a digit */
        }
        if (0 == sw && '0' != *p_str) {
            sw = 1;
        }
        if (1 == sw) {
            cnt = cnt + 1;
        }
        if (3 < cnt) {
            return 255;  /* number of digits over */
        }
        p_str++;
    }

    num = atoi(str);

    if (num > 255) {
        return 255;
    }

    if (num < 0) {
        return 0;
    }

    return num;
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
int jarun_icon_end(const zbx_uint64_t inner_job_id)
{
    DB_RESULT    result;
    DB_ROW       row;
    zbx_uint64_t inner_jobnet_id;
    char         stop_code_value[JA_STD_OUT_LEN];
    const char   *__function_name = "jarun_icon_end";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get information of end icon */
    result = DBselect("select inner_jobnet_id, jobnet_stop_code from ja_run_icon_end_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONEND200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);

    /* job controller variable get */
    if (FAIL == ja_cpy_value(inner_job_id, row[1], stop_code_value)) {
        ja_log("JARUNICONEND200002", 0, NULL, inner_job_id, __function_name, row[1], inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }
    DBfree_result(result);

    zbx_snprintf(stop_code_value, sizeof(stop_code_value), "%d", ja_stop_code_conv(stop_code_value));

    /* job(net) exit code write */
    if (FAIL == ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", stop_code_value)) {
        return FAIL;
    }

    result = DBselect("select inner_job_id from ja_run_job_table where inner_jobnet_id="ZBX_FS_UI64" and job_type=%d;", inner_jobnet_id, JA_JOB_TYPE_JOB);

    while (NULL != (row = DBfetch(result))) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id found: %s", __function_name, row[0]);
        delete_finished_files(row[0]);
    }
    DBfree_result(result);

    return ja_set_end(inner_job_id, 1);
}

void delete_finished_files(char *id_to_delete)
{
    const char *__function_name = "delete_finished_files";
    DIR *dirp;
    struct dirent *entry;
    char id[10];
    char unique_id[JA_FILE_NAME_LEN];
    char jobfile_path[JA_FILE_PATH_LEN];
    char folder_path[JA_FILE_PATH_LEN];
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),deleting finished files", __function_name);
    const char* lastSlash = strrchr(CONFIG_LOG_FILE, '/');

    char var_file_name[JA_FILE_NAME_LEN];
    zbx_snprintf(var_file_name,sizeof(var_file_name), "/job/", unique_id);
    
    if (lastSlash != NULL) {
        size_t directoryLength = lastSlash - CONFIG_LOG_FILE;
        zbx_strlcpy(folder_path, CONFIG_LOG_FILE, directoryLength+1);
        strcat(folder_path, var_file_name);
        folder_path[directoryLength+ strlen(var_file_name)] = '\0';
    }else{
        zabbix_log(LOG_LEVEL_WARNING,"In %s(), cannot get folder name : %s",__function_name,CONFIG_LOG_FILE);
        return;
    }
    // const char* lastSlash = strrchr(CONFIG_LOG_FILE, '/');
    // //char folder_path[JA_FILE_NAME_LEN];
    // zbx_snprintf(folder_path,sizeof(folder_path), "/job/");
    //zbx_snprintf(folder_path, sizeof(folder_path), "%s/job/",CONFIG_LOG_FILE);
    //zabbix_log(LOG_LEVEL_WARNING,"----------------TEST In %s(), folder name : %s",__function_name,folder_path);

    dirp = opendir(folder_path);
    if(dirp == NULL){
        zabbix_log(LOG_LEVEL_WARNING,"In %s(), cannot open %s folder.",__function_name,folder_path);
        return;
    }
    while ((entry = readdir(dirp)) != NULL)
    {
        if (entry->d_type == DT_REG)
        {
            zbx_snprintf(unique_id, sizeof(unique_id), "%s", entry->d_name);
            unique_id[strlen(unique_id) - 4] = '\0';

            if(!strcmp(unique_id,id_to_delete)){
                filePath_for_tmpjob_server(unique_id, jobfile_path);
                zabbix_log(LOG_LEVEL_DEBUG, "Deleted %s", jobfile_path);
                remove(jobfile_path);
            }
        }
    }
    closedir(dirp);
}