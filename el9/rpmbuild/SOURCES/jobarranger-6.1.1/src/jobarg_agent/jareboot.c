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


#include <json.h>
#include "common.h"
#include "log.h"
#include "threads.h"
#include "jacommon.h"
#include "jajobobject.h"
#include "jafile.h"
#include "jakill.h"
#include "jareboot.h"
#include "dirent.h"
#include "executive.h"
#include "jajobfile.h"

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
void ja_reboot_killall(ja_file_object* files,int num,JA_PID rbt_pid)
{
	/* Need to add kill function using file system*/
    JA_PID pid;
    char pid_char[JA_FILE_NAME_LEN];
    int loop_count;
	char temp_dir[JA_FILE_PATH_LEN];
    char filename[JA_FILE_PATH_LEN];
    char current_pid[JA_FILE_NAME_LEN];
    const char *__function_name = "ja_reboot_killall";
    temp_dir[0] = '\0';
    filename[0] = '\0';
    pid_char[0] = '\0';
    current_pid[0] = '\0';
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    loop_count = 0;
    while(loop_count<num){
    	//read pid from file.
    	zbx_snprintf(filename, sizeof(filename), "%s", (files + loop_count)->filename);
        if (filename == NULL || strlen(filename) == 0) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() filename is empty or null.", __function_name);
            goto contd;
        }
        zbx_snprintf(current_pid, sizeof(current_pid), "%d", rbt_pid);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), processing [%s] file.", __function_name, filename);
        if (strcmp(filename,current_pid ) == 0)
            goto contd;
        ja_jobfile_getpid_by_filename(filename, pid_char);
        if (pid_char == NULL || strlen(pid_char) == 0) {
            zabbix_log(LOG_LEVEL_ERR, "In %s()  jobid cannot be read from status file.", __function_name);
            goto contd;
        }
        pid = atoi(pid_char);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), processing to kill %d process before reboot.", __function_name, pid);
		ja_kill_ppid(pid);
        contd:
    	loop_count++;
    }
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
int ja_reboot_load_arg(ja_job_object * job, ja_reboot_arg * arg)
{
    int ret;
    json_object *jp_arg, *jp_reboot_mode_flag, *jp_reboot_wait_time;
    const char *__function_name = "ja_reboot_load_arg";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

    if (job == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(),job object is NULL.", __function_name);
        return FAIL;
    }
    if (arg == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(),job reboot arguement is NULL.", __function_name);
        return FAIL;
    }
    jp_reboot_mode_flag = NULL;
    jp_reboot_wait_time = NULL;
    ret = FAIL;
    jp_arg = json_tokener_parse(job->argument);
    if (is_error(jp_arg)) {
        jp_arg = NULL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not parse job argument [%s]", job->argument);
        goto error;
    }

    jp_reboot_mode_flag =
        json_object_object_get(jp_arg, JA_PROTO_TAG_REBOOT_MODE);
    if (jp_reboot_mode_flag == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get reboot_mode_flag from job argument [%s]",
                     job->argument);
        goto error;
    }
    arg->reboot_mode_flag = json_object_get_int(jp_reboot_mode_flag);

    jp_reboot_wait_time =
        json_object_object_get(jp_arg, JA_PROTO_TAG_REBOOT_WAIT_TIME);
    if (jp_reboot_wait_time == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get jp_reboot_wait_time from job argument [%s]",
                     job->argument);
        goto error;
    }
    arg->reboot_wait_time = json_object_get_int(jp_reboot_wait_time);

    ret = SUCCEED;
error:
      json_object_put(jp_reboot_mode_flag);
      json_object_put(jp_reboot_wait_time);
      json_object_put(jp_arg);
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
int ja_reboot_chkend(ja_job_object * job,char *file_prefix)
{
    int ret = SUCCEED;
    ja_reboot_arg arg;
    int num, i, loop;
    zbx_uint64_t sec;
    char reboot_flag_file[JA_MAX_STRING_LEN];
    char folder[JA_FILE_PATH_LEN];
    char start_time_file[JA_FILE_PATH_LEN];
    ja_file_object* files;
    time_t t;
    const char *__function_name = "ja_reboot_chkend";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
    folder[0] = '\0';
    start_time_file[0] = '\0';
    reboot_flag_file[0] = '\0';
    if (job == NULL) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() job object is NULL.", __function_name);
        return FAIL;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobid: and file prefix is :" ZBX_FS_UI64, __function_name, job->jobid,file_prefix);

    zbx_snprintf(reboot_flag_file, sizeof(reboot_flag_file),
                 "%s-" ZBX_FS_UI64, CONFIG_REBOOT_FLAG, job->jobid);

    if (ja_file_getsize(reboot_flag_file) < 0) {
        return SUCCEED;
    }
    //get start time from file.
    zbx_snprintf(start_time_file, sizeof(start_time_file), "%s%cdata%c%s.start", CONFIG_TMPDIR,JA_DLM,JA_DLM,file_prefix);
    if (ja_file_load(start_time_file, 0, &(t)) == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() start file load failed.", __function_name);
        return FAIL;
    }
    job->start_time = t;

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() processing reboot flg file [%s]. ", __function_name, reboot_flag_file);
    files = (ja_file_object*)zbx_malloc(NULL,JA_MAX_READ_LEN * sizeof(ja_file_object));
    if (NULL == files) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() files is NULL.", __function_name);
        return FAIL;
    }
    sec = time(NULL) - job->start_time;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() job start time is : [%d] and difference now is :[%d]. ", __function_name, job->start_time,sec);
    if (ja_reboot_load_arg(job, &arg) == FAIL) {
        arg.reboot_mode_flag = 0;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() job has been processing %d seconds, reboot will wait %d second with mode [%d]. ", __function_name, sec,arg.reboot_wait_time,arg.reboot_mode_flag);

    zbx_snprintf(folder, sizeof(folder), "%s%cexec", CONFIG_TMPDIR, JA_DLM);
    if ((files = read_all_files(folder, &num)) == NULL) {
        if(num == -1){
            zabbix_log(LOG_LEVEL_ERR, "In %s(), cannot read exec folder", __function_name);
            ret = FAIL;
            goto contd;
        }
        
    }
    num--;//-1 job for current process.
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() reboot_flag_file: %s, job num: %d, reboot_mode_flag: %d, reboot_wait_time: %d, sec: %d, jobid: "
               ZBX_FS_UI64, __function_name, reboot_flag_file, num,
               arg.reboot_mode_flag, arg.reboot_wait_time, sec, job->jobid);

    if (num == 0 || arg.reboot_mode_flag == 0 ||
       (sec >= arg.reboot_wait_time && arg.reboot_wait_time > 0)) {
        if (num != 0) {
            ja_reboot_killall(files,num,job->pid);
        }

        if (ja_file_remove(reboot_flag_file) == SUCCEED) {
            while (1) {
                zabbix_log(LOG_LEVEL_INFORMATION, "In %s() reboot now. jobid: " ZBX_FS_UI64",pid :%d", __function_name, job->jobid,job->pid);
                zbx_sleep(1);
#ifndef _WINDOWS
                waitpid(job->pid,NULL,WNOHANG);
#endif
                if (ja_process_check(job->pid) == SUCCEED) {
                    continue;
                }

                if (job->method == JA_AGENT_METHOD_TEST) {
                    loop = 3;
                }
                else {
                    loop = 120;
                }
                for (i = 0; i < loop; i++) {
                    zbx_sleep(1);
                }
                ret = FAIL;
                goto contd;
            }
        }
    }
contd:
    zbx_free(files);
    return ret;
}
