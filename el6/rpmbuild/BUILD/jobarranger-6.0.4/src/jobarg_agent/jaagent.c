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
#include "comms.h"
#include "log.h"

#if defined(ZABBIX_SERVICE)
#include "service.h"
#elif defined(ZABBIX_DAEMON)
#include "daemon.h"
#endif

#include "jacommon.h"
#include "jafile.h"
#include "jajobfile.h"
#include "jajobobject.h"
#include "jakill.h"
#include "jastr.h"
#include "jatelegram.h"
#include "jatcp.h"
#include "jathreads.h"
#include "jareboot.h"
#include "jaextjob.h"
#include "jaagent.h"
#include "errno.h"	//for error number fwrite/fread
#include "time.h" 	//get current local time
#include "jafile.h"
#include "jajobfile.h"
#include "stdio.h"
#define DATA_FILE 0
#define STAT_FILE 1
#define TEMP_FILE 2 

#ifdef _WINDOWS
#include "Windows.h"
#include <stdio.h>
#include <stdlib.h>
#include "locale.h"
#include "Userenv.h"
#include <winsafer.h>
#include <dirent.h>

#include <wincrypt.h>
#include <string.h>
#include <WinCrypt.h>
#define  KEYLENGTH_256   256 * 0x10000        /* 256-bit */
#endif
//char* jobext[] = { "start", "end", "ret", "stdout", "stderr", NULL };

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
int ja_agent_setenv(ja_job_object* job, char* env_string)
{
    int ret;
    struct json_object* json_env;
    char* key, *env;
    char tmp_string[JA_VALUE_NAME_LEN + JA_STD_OUT_LEN + 1];
    struct json_object* val;
    struct lh_entry* entry;
    const char* __function_name = "ja_job_object_setenv";

    val = NULL;
    if (job == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job is NULL.", __function_name);
        return FAIL;
    }

    if (env_string != NULL) {
        memset(env_string, 0, JA_MAX_DATA_LEN);
    }

    if (strlen(job->env) == 0) {
        return SUCCEED;
    }

    ret = FAIL;
    json_env = json_tokener_parse(job->env);
    if (is_error(json_env)) {
        zbx_snprintf(job->message, sizeof(job->message), "Can not parse json env data [%s]", job->env);
        json_env = NULL;
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobid: " ZBX_FS_UI64 ", json evn data cannot be parised.", __function_name, job->jobid);
        goto clear;
    }

    for (entry = json_object_get_object(json_env)->head;
        (entry ? (key = (char*)entry->k, val = (struct json_object*)entry->v, entry) : 0);
        entry = entry->next) {
        if (key == NULL || val == NULL) {
            zbx_snprintf(job->message, sizeof(job->message), "Can not parse json object data [%s]", job->env);
            goto clear;
        }
#ifdef _WINDOWS
        zbx_snprintf(tmp_string, sizeof(tmp_string), "%s", env_string);
        zbx_snprintf(env_string, JA_MAX_DATA_LEN, "%sset %s=%s\r\n", tmp_string, key, json_object_get_string(val));
#else
        zbx_snprintf(tmp_string, sizeof(tmp_string), "%s=%s", key, json_object_get_string(val));
        env = zbx_strdup(NULL, tmp_string);
        if (putenv(env) != 0) {
            zbx_snprintf(job->message, sizeof(job->message), "Can not putenv %s=%s", key, json_object_get_string(val));
            goto clear;
        }
#endif
    }
    ret = SUCCEED;

clear:
        json_object_put(val);
        json_object_put(json_env);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name, job->message);
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
int ja_agent_kill(ja_job_object* job, char* current_time)
{
    int ret;
    const char* __function_name = "ja_agent_kill";


    ret = FAIL;
    if (job == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job is NULL.", __function_name);
        return FAIL;
    }
    //[start] read job data from file.
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job->pid[%d]", __function_name,  job->pid);

    if (ja_kill(job->pid) == FAIL) {
        zbx_snprintf(job->message, sizeof(job->message), "Can not kill pid: %d, jobid: " ZBX_FS_UI64, job->pid, job->jobid);
        goto error;
    }
    ret = SUCCEED;
    //[end] Execute job according to status //
error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s ,Server IP %s ", __function_name, job->message, job->serverip);
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
int ja_agent_begin(ja_job_object* job, char* current_time, char* datafile)
{
    int ret = FAIL;
    int cnt = 0, proc_run_flag = 0, fp_ret = -1, ext_cd = 137;
    int kill_process = SUCCEED;
    ja_reboot_arg reboot_arg;
    char reboot_flag_file[JA_MAX_STRING_LEN];
    char tmp_exec_file[JA_FILE_PATH_LEN];
    char temp_pid[JA_FILE_NAME_LEN];
    char jobid[JA_FILE_NAME_LEN];
    char full_filepath[JA_FILE_PATH_LEN];
    char filename[JA_FILE_PATH_LEN];
    char init_stat_file[JA_FILE_PATH_LEN];
    char tmp_data_file[JA_FILE_PATH_LEN];
    char* zero_pid = "-0000";

    const char* __function_name = "ja_agent_begin";

    zabbix_log(LOG_LEVEL_DEBUG,"In %s(),",__function_name);
    temp_pid[0] = '\0';
    if (job == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job is NULL.", __function_name);
        return FAIL;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobid " ZBX_FS_UI64 ", method %d : start begin process.", __function_name, job->jobid, job->method);

    zbx_snprintf(reboot_flag_file, sizeof(reboot_flag_file), "%s-" ZBX_FS_UI64, CONFIG_REBOOT_FLAG, job->jobid);

    switch (job->method) {
    
    case JA_AGENT_METHOD_NORMAL:
    case JA_AGENT_METHOD_ABORT:
    case JA_AGENT_METHOD_TEST:
    
        if (strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0) {
            if (ja_reboot_load_arg(job, &reboot_arg) == FAIL) {
                goto error;
            }

            if (ja_file_create(reboot_flag_file, 1) == FAIL) {
                zbx_snprintf(job->message, sizeof(job->message), "Can not create the file [%s]", reboot_flag_file);
                goto error;
            }
            zbx_snprintf(job->script, sizeof(job->script), "\"%s\" \"%s\"", CONFIG_REBOOT_FILE, reboot_flag_file);
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), reboot script is : %s", __function_name, job->script);
            //write job script to data file.
            if (write_data_file(job, datafile) == FAIL) {
                zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.%s", __function_name,datafile);
                goto error;
            }
        }
        if (strcmp(job->type, JA_PROTO_VALUE_COMMAND) == 0 ||
            strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0 ||
            strcmp(job->type, JA_PROTO_VALUE_EXTJOB) == 0) {
            if (strcmp(job->type, JA_PROTO_VALUE_EXTJOB) == 0) {
                if (ja_extjob_script(job, datafile) == FAIL) {
                    goto error;
                }
            }
            job->status = JA_AGENT_STATUS_BEGIN;

        }
        else {
            zbx_snprintf(job->message, sizeof(job->message), "Invalid job type [%s]", job->type);
            goto error;
        }
        break;

    case JA_AGENT_METHOD_KILL:
        zbx_snprintf(jobid, sizeof(jobid), ZBX_FS_UI64"-", job->jobid);
        proc_run_flag = ja_jobfile_check_proc_kill(tmp_exec_file, jobid, job, "exec");
        if (proc_run_flag == 1) {

            ja_jobfile_getpid_by_filename(tmp_exec_file, temp_pid);
            if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
                zabbix_log(LOG_LEVEL_ERR, "In %s()  jobid cannot be read from status file.", __function_name);
                //move files
                zbx_snprintf(filename, strlen(tmp_exec_file) - 3, "%s", tmp_exec_file);
                if (ja_create_outputfile(filename, ext_cd,SUCCEED) == FAIL) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s(), restul output file creation failed.[%s]", __function_name, filename);
                    goto error;
                }
                zbx_snprintf(filename, strlen(tmp_exec_file) - 4, "%s", tmp_exec_file);
                zbx_snprintf(full_filepath, sizeof(full_filepath), "%s%s", filename, zero_pid);
                if (job_to_end(filename, full_filepath,"exec") == FAIL) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s() [%s*] files cannot be moved to end.", __function_name, filename);
                    goto error;
                }

            }
            else {
                job->pid = atoi(temp_pid);
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), file[%s] job->pid[%d] temp_pid[%s]", __function_name, tmp_exec_file, job->pid, temp_pid);
                
                if (ja_agent_kill(job, current_time) == FAIL) {
                    job->signal=0;
                    goto error;
                }else {
                    kill_process = SUCCEED;
                    zbx_snprintf(tmp_data_file, sizeof(tmp_data_file),ZBX_FS_UI64, job->jobid);
                    ja_job_fetch_json_data_filename(tmp_data_file);
                    if(strcmp(tmp_data_file,"") != 0){
                        job->signal=1;
                        if (write_data_file(job, tmp_data_file) == FAIL) {
                            zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.%s", __function_name,tmp_data_file);
                            goto error;
                        }
                    }
                }
            }
            break;
        }
        else {

            proc_run_flag = ja_jobfile_check_proc_kill(tmp_exec_file, jobid, job, "begin");
            if(proc_run_flag == 1){
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), Job ["ZBX_FS_UI64"] is not running.", __function_name, job->jobid);
                //set begin file name to -job
                zbx_snprintf(filename, strlen(tmp_exec_file) - 3, "%s", tmp_exec_file);   //jobid-datetime
                zbx_snprintf(init_stat_file, sizeof(init_stat_file), "%s%c%s-.job", JA_BEGIN_FOLDER, JA_DLM, filename);
                zbx_snprintf(full_filepath, sizeof(full_filepath), "%s%c%s", JA_BEGIN_FOLDER,JA_DLM,tmp_exec_file);
                if (rename(full_filepath, init_stat_file) != 0) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s(),[%s] job begin file cannot be renamed.%s", __function_name, tmp_exec_file, strerror(errno));
                    goto error;
                }
                zbx_snprintf(init_stat_file, sizeof(init_stat_file), "%s-",filename);
                if (ja_create_outputfile(init_stat_file, ext_cd,SUCCEED) == FAIL) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s(), restul output file creation failed.[%s]", __function_name, init_stat_file);
                    goto error;
                }
                zbx_snprintf(full_filepath, sizeof(full_filepath), "%s%s", filename, zero_pid);
                if (job_to_end(filename, full_filepath,"begin") == FAIL) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s() [%s*] files cannot be moved to end.", __function_name, filename);
                    goto error;
                }
                kill_process = SUCCEED;
            }else {
                proc_run_flag = ja_jobfile_check_proc_kill(tmp_exec_file, jobid, job, "temp");
                if(proc_run_flag == 1){
                    zabbix_log(LOG_LEVEL_WARNING, "In %s(), Job ["ZBX_FS_UI64"] is not running (still  in temp folder).", __function_name, job->jobid);
                    //set begin file name to -job
                    zbx_snprintf(filename, strlen(tmp_exec_file) - 3, "%s", tmp_exec_file);   //jobid-datetime
                    zbx_snprintf(init_stat_file, sizeof(init_stat_file), "%s%c%s-.job", JA_TEMP_FOLDER, JA_DLM, filename);
                    zbx_snprintf(full_filepath, sizeof(full_filepath), "%s%c%s", JA_TEMP_FOLDER,JA_DLM,tmp_exec_file);
                    if (rename(full_filepath, init_stat_file) != 0) {
                        zabbix_log(LOG_LEVEL_ERR, "In %s(),[%s] job temp file cannot be renamed.%s", __function_name, tmp_exec_file, strerror(errno));
                        goto error;
                    }
                    zbx_snprintf(init_stat_file, sizeof(init_stat_file), "%s-",filename);
                    if (ja_create_outputfile(init_stat_file, ext_cd,SUCCEED) == FAIL) {
                        zabbix_log(LOG_LEVEL_ERR, "In %s(), restul output file creation failed.[%s]", __function_name, init_stat_file);
                        goto error;
                    }
                    zbx_snprintf(full_filepath, sizeof(full_filepath), "%s%s", filename, zero_pid);
                     zabbix_log(LOG_LEVEL_INFORMATION, "In %s() file name is : %s",__function_name,filename);
                    if (job_to_end(filename, full_filepath,"temp") == FAIL) {
                        zabbix_log(LOG_LEVEL_ERR, "In %s() [%s*] files cannot be moved to end.", __function_name, filename);
                        goto error;
                    }
                    kill_process = SUCCEED;
                }
            }
            
        }

        if (strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0) {
            ja_file_remove(reboot_flag_file);
        }
        break;

    default:
        zbx_snprintf(job->message, sizeof(job->message), "Invalid method: %d, jobid " ZBX_FS_UI64 ")", job->method, job->jobid);
        goto error;
        break;
    }
    ret = SUCCEED; 

error:
    if (job->method != JA_AGENT_METHOD_KILL) {
        if (write_data_file(job, datafile) == FAIL) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.%s", __function_name, datafile);
            ret = FAIL;
        }
    }
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() jobid[" ZBX_FS_UI64 ",] %s ,Server IP %s  ",
                    __function_name, job->jobid, job->message, job->serverip);
    }
    else {
        if (job->status == JA_AGENT_STATUS_BEGIN) {
            zbx_snprintf(filename, sizeof(filename),  ZBX_FS_UI64 "-%s.job", job->jobid, current_time);
             //move from temp to begin
			if (ja_jobfile_move(JA_TEMP_FOLDER, JA_BEGIN_FOLDER, filename) == FAIL) {
			    zabbix_log(LOG_LEVEL_ERR, "In %s() [%s] file cannot be moved.", __function_name, filename);
                return FAIL;
			}
        }
        zabbix_log(LOG_LEVEL_INFORMATION, "In %s(),jobid: " ZBX_FS_UI64 ", method: %d is begin", 
                    __function_name, job->jobid, job->method);
    }
    if(job->method == JA_AGENT_METHOD_KILL && kill_process == FAIL){
        zabbix_log(LOG_LEVEL_INFORMATION, "In %s() jobid: " ZBX_FS_UI64 ", method: %d failed!",__function_name, job->jobid, job->method);
        ja_remove_abort_job_file(job->jobid);
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
int ja_agent_run(ja_job_object* job, char* full_filename)
{
    JA_PID pid;
    int        ret =SUCCEED;
    int        isSeteuid = 1;                               /* 0:run user,  1:not run user,  2:command user */
    int        status;
    char       filepath[JA_MAX_STRING_LEN];
    char       full_command[JA_MAX_STRING_LEN];
    char       w_user[JA_MAX_STRING_LEN];
    char       w_passwd[JA_MAX_STRING_LEN];
    char       cmd_user[JA_MAX_STRING_LEN];
    char       cmd_passwd[JA_MAX_STRING_LEN];
    const char* __function_name = "ja_agent_run";

    //Park.iggy ADD START
    char    d_passwd[JA_MAX_STRING_LEN];
    char    d_dec[256];
    char    d_flag[2] = "1";
    char    d_x16[3];
    char* d_cat = "0x";
    char    d_catX16[5];

    int     k, kk, x16toX10;

    //Park.iggy END
    //ThihaOo@DAT start
    size_t len;
    char data_filepath[JA_MAX_STRING_LEN];
    char data_file[JA_MAX_STRING_LEN];
    char tmp_exe_file[JA_MAX_STRING_LEN];
    char exec_file[JA_MAX_STRING_LEN];
    char tmp_filename[JA_MAX_STRING_LEN];
    char data_filename[JA_FILE_NAME_LEN];
    //end

    len = strlen(full_filename);
    zbx_strlcpy(data_filename, full_filename, len - 3);
    zbx_snprintf(data_filepath, sizeof(data_filepath), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "data");
    zbx_snprintf(data_file, sizeof(data_file), "%s%c%s%s", data_filepath, JA_DLM, data_filename, ".json");

#ifdef _WINDOWS
    STARTUPINFO si;
    PROCESS_INFORMATION pi;
    LPTSTR wcommand;
    char env[JA_MAX_DATA_LEN];
    char full_script[JA_MAX_DATA_LEN];

    int     i;
    int     j;
    wchar_t user[256];
    wchar_t pwd[256];
    size_t    wLen;
    HANDLE  hToken;

    char    dec[256];
    char* key = "199907";

    BYTE    pbData[256];
    DWORD   dwDataLen = (DWORD)(strlen((char*)pbData) + 1);
    DWORD   strLen = 1000;

#endif

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (job == NULL) {
        return FAIL;
    }

    zabbix_log(LOG_LEVEL_INFORMATION, "In %s(),jobid: " ZBX_FS_UI64 " process started.Status: %d",__function_name, job->jobid, job->status);
    //Set initial message for rerun cases.
    zbx_snprintf(job->message, sizeof(job->message), "");
    pid = 0;
    job->start_time = time(NULL);
    job->end_time = time(NULL);

    status = job->status;
    if (status == JA_JOB_STATUS_ABORT) {
        zabbix_log(LOG_LEVEL_DEBUG, "jobid: " ZBX_FS_UI64 ", status: [%d] Abort ", job->jobid, status);
        job->status = JA_AGENT_STATUS_END;
        write_data_file(job, data_file);
        return FAIL;
    }

#ifdef _WINDOWS
    ret = SUCCEED;
    memset(&si, 0, sizeof(si));
    si.cb = sizeof(si);
    GetStartupInfo(&si);
    wcommand = NULL;

    if (ja_agent_setenv(job, env) == FAIL)
        goto error;
    if (job->method == JA_AGENT_METHOD_TEST) {
        zbx_snprintf(full_script, sizeof(full_script), "%s", env);
    }
    else {
        zbx_snprintf(full_script, sizeof(full_script), "%s%s", env, job->script);
    }
    pid = getpid();
    zbx_snprintf(filepath, sizeof(filepath), "%s%c%s%c",JA_DATA_FOLDER, JA_DLM, data_filename, '-');
    zbx_snprintf(tmp_exe_file, sizeof(tmp_exe_file), "%s%c%s%c", JA_EXEC_FOLDER, JA_DLM, data_filename);
    zbx_snprintf(exec_file, sizeof(exec_file), "%s%c%s%s", JA_EXEC_FOLDER, JA_DLM, data_filename, "-.job");
    if (ja_jobfile_create(filepath,full_script) == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job data files creation failed. (%s)", __function_name, strerror(errno));
        zbx_snprintf(job->message, sizeof(job->message), "Can not create the result files [%s]", filepath);
        ret = FAIL;
        goto error;
    }

    zbx_snprintf(w_user, sizeof(w_user), "%s", job->run_user);
    zbx_snprintf(w_passwd, sizeof(w_passwd), "%s", job->run_user_password);
    zbx_snprintf(d_passwd, sizeof(d_passwd), "%s", job->run_user_password);

    zbx_snprintf(cmd_user, sizeof(cmd_user), "%s", CONFIG_JA_COMMAND_USER);
    zbx_snprintf(cmd_passwd, sizeof(cmd_passwd), "%s", CONFIG_JA_COMMAND_PASSWORD);

    zabbix_log(LOG_LEVEL_DEBUG, "[jaagent]   run user = %s ,  passwd (Encryption) = %s  ", w_user, w_passwd);
    zabbix_log(LOG_LEVEL_DEBUG, "[jaagent]   config user = %s ,  passwd = %s  ", cmd_user, cmd_passwd);
    if ((strcmp(w_user, "") != 0) && (strcmp(w_user, "(null)") != 0)) {              /* run_user specified ? */
        isSeteuid = 0;
    }
    else if ((strcmp(cmd_user, "") != 0) && (strcmp(cmd_user, "(null)") != 0)) {   /* is command_user specified as jobarg_agentd.conf */
        zbx_snprintf(w_user, sizeof(w_user), "%s", CONFIG_JA_COMMAND_USER);
        zbx_snprintf(w_passwd, sizeof(w_passwd), "%s", CONFIG_JA_COMMAND_PASSWORD);
        isSeteuid = 2;
    }
    else {
        zbx_snprintf(w_user, sizeof(w_user), "%s", "");
        zbx_snprintf(w_passwd, sizeof(w_passwd), "%s", "");
    }

    /* if not command, not CreateProcessAsUser() */
    if (strcmp(job->type, JA_PROTO_VALUE_COMMAND) != 0) {
        isSeteuid = 1;
        zbx_snprintf(w_user, sizeof(user), "%s", "");
    }

    if ((isSeteuid == 0) || (isSeteuid == 2)) {
        /* decodes password */
        if (isSeteuid == 0) {

            if (d_flag[0] == d_passwd[0]) {
                j = 0;
                k = 0;
                for (kk = 1; kk < strlen(d_passwd); kk++) {
                    if ((kk % 2) != 0) {
                        d_x16[0] = d_passwd[kk];
                    }
                    else {
                        d_x16[1] = d_passwd[kk];
                        d_x16[2] = '\0';
                        zbx_snprintf(d_catX16, sizeof(d_catX16), "0x%s", d_x16);
                        x16toX10 = (unsigned long)strtol(d_catX16, NULL, 0);
                        *d_x16 = NULL;
                        *d_catX16 = NULL;
                        d_dec[k] = (char)(x16toX10);
                        dec[k] = (char)(d_dec[k] ^ key[j]);

                        j++;
                        k++;
                        if (j == strlen(key)) j = 0;
                    }
                }
                *w_passwd = NULL;
            }
            dec[k] = '\0';

            j = 0;
            for (i = 0; i < strlen(w_passwd); i++)
            {
                dec[i] = (char)(w_passwd[i] ^ key[j]);
                j++;
                if (j == strlen(key)) j = 0;
            }
            memset(w_passwd, '\0', strlen(w_passwd));
            zbx_snprintf(w_passwd, sizeof(w_passwd), "%s", dec);
            zabbix_log(LOG_LEVEL_DEBUG, "[jaagent]   password (Decryption) = %s  ", w_passwd);
        }

        mbstowcs_s(&wLen, user, sizeof(user) / 2, w_user, _TRUNCATE);
        mbstowcs_s(&wLen, pwd, sizeof(pwd) / 2, w_passwd, _TRUNCATE);

        /* attempt to log a user on to the local computer */
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobarg_command execution command1 is : [%s]", __function_name, full_command);
        if (!LogonUser(
            user,                      /* A pointer to a null-terminated string that specifies the name of the user                           */
            NULL,                      /* A pointer to a null-terminated string that specifies the name of the domain or server               */
            pwd,                       /* A pointer to a null-terminated string that specifies the plaintext password for the user            */
            LOGON32_LOGON_INTERACTIVE, /* The type of logon operation to perform                                                              */
            LOGON32_PROVIDER_DEFAULT,  /* Specifies the logon provider                                                                        */
            &hToken                    /* A pointer to a handle variable that receives a handle to a token that represents the specified user */
        )) {
            job->result = JA_JOBRESULT_FAIL;
            job->status = JA_AGENT_STATUS_END;
            if (GetLastError() == ERROR_LOGON_FAILURE) {
                zbx_snprintf(job->message, sizeof(job->message),
                    "can not specify the user account for  '%ws' ", user);
            }
            else {
                zbx_snprintf(job->message, sizeof(job->message),
                    "failed to LogonUser function");
            }
            // Add database update function(job,file_name) here.
            ret = FAIL;
            CloseHandle(hToken);
            zbx_free(wcommand);
            goto error;
        }
        CloseHandle(hToken);

        zbx_snprintf(full_command, sizeof(full_command), "\"%s\" \"%s\" \"%s.%s\" \"%s\" \"%s\" \"%s\" \"%s\"", CONFIG_CMD_FILE, filepath, filepath, JA_EXE, w_user, w_passwd, tmp_exe_file, exec_file);
        wcommand = zbx_acp_to_unicode(full_command);
    }
    else {
        zbx_snprintf(full_command, sizeof(full_command), "\"%s\" \"%s\" \"%s.%s\" \"%s\" \"%s\" \"%s\" \"%s\"", CONFIG_CMD_FILE, filepath, filepath, JA_EXE, "", "", tmp_exe_file, exec_file);
        wcommand = zbx_acp_to_unicode(full_command);
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobarg_command execution command2 is : [%s]", __function_name, full_command);

    if (0 == CreateProcess(NULL,        /* no module name (use command line) */
        wcommand,    /* name of app to launch */
        NULL,        /* default process security attributes */
        NULL,        /* default thread security attributes */
        FALSE,       /* do not inherit handles from the parent */
        0,           /* normal priority */
        NULL,        /* use the same environment as the parent */
        NULL,        /* launch in the current directory */
        &si,         /* startup information */
        &pi          /* process information stored upon return */
    )) {
        zabbix_log(LOG_LEVEL_ERR, "In %s()  jobid: " ZBX_FS_UI64 ",Job create failed. (%s)", __function_name, job->jobid, strerror_from_system(GetLastError()));
        zbx_snprintf(job->message, sizeof(job->message), "failed to create process for jobid " ZBX_FS_UI64 " [%s]: %s",
            job->jobid, full_command, strerror_from_system(GetLastError()));
        ret = FAIL;
    }
    else {
        job->pid = pi.dwProcessId;
        CloseHandle(pi.hThread);
        CloseHandle(pi.hProcess);
    }
    zbx_free(wcommand);
#else

    int ext = 1;
    char filename_pid[JA_FILE_PATH_LEN];
    pid = ja_fork();
    if (pid == -1) {
        ret = FAIL;
        //zbx_snprintf(filename_pid, sizeof(filename_pid), "%s-%d", data_filename, "0000");
        zabbix_log(LOG_LEVEL_ERR, "In %s() ,job pid : "ZBX_FS_UI64"failed to create child process.(%s)", __function_name, job->pid,strerror(errno));
        zbx_snprintf(job->message, sizeof(job->message), "ja_fork() failed for jobid: " ZBX_FS_UI64 "[%s]",job->jobid, zbx_strerror(errno));
        // if (job_to_end(data_filename, filename_pid,"exec") == FAIL) {
        //     zabbix_log(LOG_LEVEL_DEBUG, "In %s() ,job_to_end() failed . filename : %s", __function_name,data_filename);
        // }
        goto error;
    }
    else if (pid == 0) {
        setpgid(0,0);
        job->pid = getpid();
        zbx_snprintf(filepath, sizeof(filepath), "%s%c%s%c"ZBX_FS_UI64, JA_DATA_FOLDER, JA_DLM, data_filename, '-', getpid());
        zbx_snprintf(tmp_exe_file, sizeof(tmp_exe_file), "%s%c%s%s%d%s", JA_EXEC_FOLDER, JA_DLM, data_filename, "-", getpid(), ".job");
        zbx_snprintf(exec_file, sizeof(exec_file), "%s%c%s%s", JA_EXEC_FOLDER, JA_DLM, data_filename, "-.job");
        if (job->method == JA_AGENT_METHOD_TEST) {
            ret = ja_jobfile_create(filepath,"");
        }
        else {
            ret = ja_jobfile_create(filepath, job->script);
        }
        if (ret == FAIL) {
            zbx_snprintf(job->message, sizeof(job->message), "Can not create the result files [%s]", filepath);
            zabbix_log(LOG_LEVEL_ERR,"In %s(), %s jobid:"ZBX_FS_UI64,__function_name,job->message,job->jobid);
            goto child_error;
        }
        zbx_snprintf(full_command, sizeof(full_command), "%s.%s", filepath, JA_EXE);

        zbx_snprintf(w_user, sizeof(w_user), "%s", job->run_user);
        zbx_snprintf(w_passwd, sizeof(w_passwd), "%s", job->run_user_password);
        zbx_snprintf(cmd_user, sizeof(cmd_user), "%s", CONFIG_JA_COMMAND_USER);
        zbx_snprintf(cmd_passwd, sizeof(cmd_passwd), "%s", CONFIG_JA_COMMAND_PASSWORD);

        zabbix_log(LOG_LEVEL_DEBUG, "[jaagent]   run_user = %s ,  run_passwd = %s  ", w_user, w_passwd);
        zabbix_log(LOG_LEVEL_DEBUG, "[jaagent]   config_user = %s ,  config_passwd = %s  ", cmd_user, cmd_passwd);

        isSeteuid = 1;

        /* run_user specified ? */
        if ((strcmp(w_user, "") != 0) && (strcmp(w_user, "(null)") != 0)) {
            isSeteuid = 0;
        }
        else if ((strcmp(cmd_user, "") != 0) && (strcmp(cmd_user, "(null)") != 0)) {   /* is command_user specified as jobarg_agentd.conf */
            zbx_snprintf(w_user, sizeof(w_user), "%s", CONFIG_JA_COMMAND_USER);
            isSeteuid = 2;
        }

        /* if not command, not seteuid() */
        if (strcmp(job->type, JA_PROTO_VALUE_COMMAND) != 0) {
            isSeteuid = 1;
            zbx_snprintf(w_user, sizeof(w_user), "%s", "");
        }

        if (isSeteuid == 0 || isSeteuid == 2) {
            /* the agent to run as 'root' ? */
            if (0 == getuid() || 0 == getgid()) {
                /* set the agent UID to the realUID( run_user/command_user) */

                struct passwd* pwd;
                /* get the local password file that matches user name */
                pwd = getpwnam(w_user);

                if (NULL == pwd) {
                    zbx_snprintf(job->message, sizeof(job->message), "User [ %s ] does not exist", w_user);
                    zabbix_log(LOG_LEVEL_ERR,"In %s(), %s jobid:"ZBX_FS_UI64,__function_name,job->message,job->jobid);
                    // Add database update function(job,file_name) here.
                    ext = -1;
                    goto child_error;
                }
            }
            else {
                /* the agent not to run as 'root' */
                zbx_snprintf(job->message, sizeof(job->message), "Agent does not to run as 'root'");
                zabbix_log(LOG_LEVEL_ERR,"In %s(), %s jobid:"ZBX_FS_UI64,__function_name,job->message,job->jobid);
                // Add database update function(job,file_name) here.
                ext = -1;
                goto child_error;
            }
        }

        if (ja_agent_setenv(job, NULL) == SUCCEED) {
            /* execl(CONFIG_CMD_FILE, CONFIG_CMD_FILE, filepath, full_command, NULL); */
            char exefile[JA_FILE_PATH_LEN];
            zbx_snprintf(exefile, sizeof(exefile), "%s", tmp_exe_file);
            execl(CONFIG_CMD_FILE, CONFIG_CMD_FILE, filepath, full_command, w_user, w_passwd, exefile, exec_file, NULL);
            zbx_snprintf(job->message, sizeof(job->message),
                "execl() failed for [%s]: %s", CONFIG_CMD_FILE,
                zbx_strerror(errno));
        }
    child_error:
        zbx_snprintf(filename_pid, sizeof(filename_pid),"%s-%d",data_filename,getpid());
        zbx_sleep(1);
        job->result = JA_JOBRESULT_FAIL;
        job->status = JA_AGENT_STATUS_END;
        write_data_file(job, data_file);
        if (job_to_end(data_filename,filename_pid,"exec") == FAIL) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() ,job_to_end() failed . filename : %s", __function_name, data_filename);
        }
        exit(ext);
    }
    else {
        //waitpid(pid, NULL, WNOHANG);
        ret == SUCCEED;
        job->pid = pid;
    }
#endif


error:
    if (ret == FAIL) {
        zbx_snprintf(tmp_filename, sizeof(tmp_filename), "%s%s", data_filename, "-0000");
        zabbix_log(LOG_LEVEL_ERR, "In %s() ,ret failed . filename : %s", __function_name, data_filename);
        job->result = JA_JOBRESULT_FAIL;
        job->status = JA_AGENT_STATUS_END;
        if (job_to_end(data_filename,tmp_filename,"exec") == FAIL) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() ,job_to_end() failed . filename : %s", __function_name, data_filename);
        }

        zabbix_log(LOG_LEVEL_ERR, "In %s(), job agent run error.%s.server ip: %s", __function_name, job->message, job->serverip);
    }
    else {
        job->status = JA_AGENT_STATUS_RUN;
        job->result = JA_JOBRESULT_SUCCEED;
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job agent run succeed.server ip: %s", __function_name, job->serverip);
    }
    write_data_file(job, data_file);
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
int ja_agent_close(ja_job_object* job, char* filename)
{
    char filepath[JA_FILE_PATH_LEN];
    char temp_prefix[JA_FILE_PATH_LEN];
    const char* __function_name = "ja_agent_close";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

    filepath[0] = '\0';
    temp_prefix[0] = '\0';

    if (job == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job is NULL.", __function_name);
        return FAIL;
    }

    zbx_snprintf(filepath, sizeof(filepath), "%s%cend", CONFIG_TMPDIR, JA_DLM);
    if(ja_agent_send(job) == FAIL) {
        if (job->send_retry >= CONFIG_SEND_RETRY) {
            zbx_snprintf(job->message, sizeof(job->message), "Can not send the result to server. jobid: " ZBX_FS_UI64, job->jobid);
            job->status = JA_AGENT_STATUS_CLOSE;
        }
        return FAIL;
    }
    zabbix_log(LOG_LEVEL_INFORMATION, "In %s() jobid: " ZBX_FS_UI64 " closed OK, status: %d, server ip : %s", __function_name, job->jobid, job->status, job->serverip);
    job->status = JA_AGENT_STATUS_CLOSE;
    return SUCCEED;
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
int ja_agent_send(ja_job_object* job)
{
    int ret;
    zbx_sock_t s;
    char* data;
    json_object* response;
    json_object* jp_data, * jp;
    int result;
    const char* __function_name = "ja_agent_send";

    char index_file_dir[JA_FILE_PATH_LEN];
	char index_name[JA_FILE_NAME_LEN];
    char previous[JA_FILE_NAME_LEN];
	char ip_to_change[JA_JOB_ID_LEN];

    zbx_snprintf(previous, sizeof(previous),"new");

    response = NULL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (job == NULL) {
        return FAIL;
    }

    if (job->method == JA_AGENT_METHOD_ABORT) {
        return SUCCEED;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "Send job result Server IP %s,", job->serverip);
    zbx_snprintf(job->hostname, sizeof(job->hostname), "%s", CONFIG_HOSTNAME);

    ret = zbx_tcp_connect(&s, CONFIG_SOURCE_IP, job->serverip, CONFIG_SERVER_PORT, CONFIG_TIMEOUT);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "Send job result error: Server IP %s, [connect] %s", job->serverip, zbx_tcp_strerror());
        return FAIL;
    }
    zbx_snprintf(job->kind, sizeof(job->kind), "%s", JA_PROTO_VALUE_JOBRESULT);

    zbx_snprintf(ip_to_change,sizeof(ip_to_change),"%s",job->serverip);
	change_ip_format(ip_to_change);
	zbx_snprintf(index_name,sizeof(index_name), ZBX_FS_UI64 "_%s", job->jobid,ip_to_change);
	filePath_for_tmpjob_agent(index_name, index_file_dir);
    
    //check for Data anomalies
    read_lastLine_from_file(index_file_dir,previous);
    zbx_snprintf(job->pre_unique_id, JA_MAX_STRING_LEN,"%s",previous);
    
    zabbix_log(LOG_LEVEL_DEBUG,"job->pre_unique_id is %s",job->pre_unique_id);
    ret = ja_tcp_send_to(&s, job, CONFIG_TIMEOUT);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "Send job result error: Server IP %s, [send] %s", job->serverip, zbx_tcp_strerror());
        goto error;
    }
    ret = zbx_tcp_recv_to(&s, &data, CONFIG_TIMEOUT);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "Send job result error: Server IP %s, [recv] %s", job->serverip, zbx_tcp_strerror());
        goto error;
    }

    if (strlen(data) == 0) {
        ret = FAIL;
        goto error;
    }

    response = json_tokener_parse(data);
    if (is_error(response)) {
        zabbix_log(LOG_LEVEL_WARNING, "the recv data is not json data. %s , Server IP %s", data, job->serverip);
        response = NULL;
        ret = FAIL;
        goto error;
    }

    jp_data = json_object_object_get(response, JA_PROTO_TAG_DATA);
    if (jp_data == NULL) {
        zabbix_log(LOG_LEVEL_WARNING, "can not get the tag [%s] from json data [%s] , Server IP %s", JA_PROTO_TAG_DATA, data, job->serverip);
        ret = FAIL;
        goto error;
    }
    jp = json_object_object_get(jp_data, JA_PROTO_TAG_RESULT);
    result = json_object_get_int(jp);

    if (result == FAIL) {
        jp = json_object_object_get(jp_data, JA_PROTO_TAG_MESSAGE);

        if(strcmp(json_object_get_string(jp),"Data recovery") == 0){
            jp = json_object_object_get(jp_data, JA_PROTO_TAG_PRE_UNIQUE_ID);
            zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), Data recovered for %s, Server IP %s ",__function_name, json_object_get_string(jp), job->serverip);
            zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id),"%s",json_object_get_string(jp));
            add_uid_to_job_file(index_file_dir,json_object_get_string(jp));
            ret = FAIL;

            goto error;
        }else if(strcmp(json_object_get_string(jp),"DUPLICATE DATA") == 0){
            jp = json_object_object_get(jp_data, JA_PROTO_TAG_PRE_UNIQUE_ID);
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Duplicate transaction was detected. Id - %s, Server IP %s ",__function_name, json_object_get_string(jp), job->serverip);
            zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id),"%s",json_object_get_string(jp));
            add_uid_to_job_file(index_file_dir,job->pre_unique_id);
            ret = FAIL;

            goto error;
        }else if(strcmp(json_object_get_string(jp),"AFTER_STATUS ERROR") == 0){
            ret = FAIL;
            goto error;
        }else{
            zabbix_log(LOG_LEVEL_WARNING, "job response message:  %s, Server IP %s ", json_object_get_string(jp), job->serverip);
        }
    }else{
        zabbix_log(LOG_LEVEL_DEBUG, "Data result is :  %s, adding uid: %s to %s", json_object_get_string(jp), job->cur_unique_id,index_file_dir);

        int createFolder;
        #ifdef _WINDOWS
                createFolder = _mkdir(JA_JOBS_FOLDER);
        #else
                createFolder = mkdir(JA_JOBS_FOLDER, JA_PERMISSION);
        #endif
        //if folder create fail without an existing folder
        if(createFolder != 0 && errno != EEXIST){
            zabbix_log(LOG_LEVEL_ERR , "Folder cannot be created.Path: %s",JA_JOBS_FOLDER);
            return FAIL;
        }
        
        add_uid_to_job_file(index_file_dir,job->cur_unique_id);
    }
    zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), job id :"ZBX_FS_UI64" sent OK, job result sent to server IP %s,",__function_name, job->jobid, job->serverip);
    ret = SUCCEED;

error:
    zbx_tcp_close(&s);
    if (response != NULL) {
        json_object_put(response);
    }
    return ret;
}
//Added by ThihaOo@dat
int ja_jobstatus_file(ja_job_object* job, char* current_time) {
    const char* __function_name = "ja_jobstatus_file";
    char filename[JA_FILE_PATH_LEN];
    int ret = SUCCEED;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    //Get file path/file name
    if (ja_create_status_filepath(job, filename, current_time, TEMP_FILE) == FAIL) {
        return FAIL;
    }
    //To see if the file name works
    if (ja_file_create(filename, 1) == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() file cannot be created.%s", __function_name, filename);
        return FAIL;
    }

    ja_unique_id_generete(job);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() temp file created", __function_name);
    return ret;
}

int ja_unique_id_generete(ja_job_object* job){
    
    const char* __function_name = "ja_unique_id_generete";
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    // generate unique id for jobs
    char nano_time[40];
    char unique_id[JA_FILE_NAME_LEN];
    char index_name[JA_FILE_NAME_LEN];
    char jobfile_path[JA_FILE_PATH_LEN];
    char index_file_dir[JA_FILE_PATH_LEN];

    char previous[JA_FILE_NAME_LEN];
    char ip_to_change[JA_JOB_ID_LEN];

    zbx_snprintf(previous, sizeof(previous),"");
    get_nano_time(nano_time);

    zbx_snprintf(ip_to_change,50,"%s",job->serverip);
    change_ip_format(ip_to_change);
    zbx_snprintf(unique_id, 50, ZBX_FS_UI64 "_%s_%s", job->jobid, nano_time, ip_to_change);

    zbx_snprintf(index_name, 50, ZBX_FS_UI64 "_%s", job->jobid,ip_to_change);
    filePath_for_tmpjob_agent(index_name, index_file_dir);

    if(read_lastLine_from_file(index_file_dir,previous)==FAIL){
        zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id), "new");
    }else{
        zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id), "%s", previous);
    }

    zbx_snprintf(job->cur_unique_id, sizeof(job->cur_unique_id), "%s", unique_id);
    zabbix_log(LOG_LEVEL_DEBUG,"cur-id[%s] pre-id[%s]",job->cur_unique_id,job->pre_unique_id);
}


//Added by ThihaOo@dat
int ja_delete_file(ja_job_object* job, char* current_time,int file_type) {
    int ret = SUCCEED;
    int remFlag = 0;
    const char* __function_name = "ja_delete_file";
    char filename[JA_FILE_PATH_LEN];
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() jobid: " ZBX_FS_UI64, __function_name,job->jobid);
    //Get file path/file name
    if (ja_create_status_filepath(job, filename, current_time, file_type) == FAIL) {
        return FAIL;
    }
    remFlag = remove(filename);
    if (remFlag != 0) {
        if (errno == EPERM) {
            zabbix_log(LOG_LEVEL_ERR, " In %s() Permission denied in %s", __function_name, filename);
        }
        zabbix_log(LOG_LEVEL_ERR, "In %s() cannot delete file in %s", __function_name, filename);
    }
    return ret;
}

//Added by ThihaOo@dat
int ja_create_status_filepath(ja_job_object* job, char* filename, char* current_time, int file_type) {
    int ret = FAIL;
    const char* __function_name = "ja_create_status_filepath";
    char filepath[JA_FILE_PATH_LEN];
    char folderpath[JA_FILE_PATH_LEN];
    char statusFolder[JA_FILE_PATH_LEN];
    int createFolder;
    zabbix_log(LOG_LEVEL_DEBUG, "%s() Started. ", __function_name);
    if (file_type == TEMP_FILE) {
        zbx_snprintf(statusFolder, sizeof(statusFolder), "%s", "temp");
    }
    else if (file_type == DATA_FILE) {
        zbx_snprintf(statusFolder, sizeof(statusFolder), "%s", "data");
    }
    zbx_snprintf(folderpath, sizeof(folderpath), "%s%c%s", CONFIG_TMPDIR, JA_DLM, statusFolder);
#if defined _WINDOWS
    createFolder = _mkdir(folderpath);
#else
    createFolder = mkdir(folderpath, JA_PERMISSION);
#endif
    if (createFolder != 0) {
        if (errno == EEXIST) {
            zabbix_log(LOG_LEVEL_DEBUG,
                "In %s() Folder already exist in %s", __function_name, folderpath);
        }
        else if (errno == EPERM) {
            zabbix_log(LOG_LEVEL_ERR,
                "In %s() Permission denied in %s", __function_name, folderpath);
            goto error;
        }
        else {
            zabbix_log(LOG_LEVEL_ERR,
                "In %s() Folder cannot be created.Path: %s", __function_name, folderpath);
            goto error;
        }
    }
    zbx_snprintf(filepath, sizeof(filepath), "%s%c" ZBX_FS_UI64 "-%s", folderpath, JA_DLM, job->jobid, current_time);
    //To see if the file path works
    if (file_type == TEMP_FILE) {
        zbx_snprintf(filename, JA_FILE_PATH_LEN, "%s.%s", filepath, "job");
    }
    else if (file_type == DATA_FILE) {
        zbx_snprintf(filename, JA_FILE_PATH_LEN, "%s.%s", filepath, "json");
    }
    ret = SUCCEED;
    error:
    return ret;
}

//Added by ThihaOo@dat
//write and read data file for listener process
int ja_write_file_data(ja_job_object* job, char* current_time,int file_type) {
    char filename[JA_FILE_PATH_LEN];
    int ret = SUCCEED;
    const char* __function_name = "ja_write_file_data";

    //start write file process
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()  ", __function_name);

    ja_create_status_filepath(job, filename, current_time, file_type);
#ifdef _WINDOWS
    char* buf;
    buf = (char*)ja_utf8_to_acp(job->script);
    zbx_snprintf(job->script, sizeof(job->script), "%s", buf);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), script output is converted to utf8 to acp.", __function_name);
    zbx_free(buf);
    if (job->env != NULL) {
        buf = (char*)ja_utf8_to_acp(job->env);
        zbx_snprintf(job->env, sizeof(job->env), "%s", buf);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), env output is converted to utf8 to acp.", __function_name);
        zbx_free(buf);
    }
#else
    char* buf;
    int len, i = 0, j = 0;
    len = strlen(job->script);
    buf = (char*)zbx_malloc(buf, len+1);
    for (i = 0; i < len; i++) {
        if ('\r' != job->script[i]) {
            buf[j] = job->script[i];
            j++;
        }
    }
    buf[j] = '\0';
    zbx_snprintf(job->script, sizeof(job->script), "%s", buf);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job->script [%s]", __function_name, job->script);
    zbx_free(buf);
#endif
    ret = write_json_data(job, filename);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), failed to write data file: [%s]", __function_name, filename);
        return FAIL;
    }
    return ret;
}

//write on data file on agent_run process
int write_data_file(ja_job_object* job, char* data_file) {
    size_t data_filesize;
    ja_job_object* temp_job;
    int i=0, max_cnt = 5;
    int ret = FAIL;
    const char* __function_name = "write_data_file";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), start writing data file:%s , serverip : %s", __function_name, data_file, job->serverip);
    for (i = 0; i < max_cnt;i++) {
        if (write_json_data(job, data_file) == FAIL) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), failed to write file: [%s].", __function_name, data_file);
            continue;
        }
        data_filesize = ja_file_getsize(data_file);

        if (0 >= data_filesize) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(),data file [%s], [size : %d] is empty.Rewrite.", __function_name, data_file, data_filesize);
            continue;
        }
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), jobid: " ZBX_FS_UI64 ", status: %d , serverip : %s ", __function_name, job->jobid, job->status, job->serverip);

        temp_job = (ja_job_object*)malloc(sizeof(ja_job_object));
        if (NULL == temp_job)
            continue;
        if (read_datafile(temp_job, data_file) == FAIL) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), file cannot be read after writing file.file : [%s]", __function_name,data_file);
        }

        if (strcmp(job->serverid, temp_job->serverid) == 0) {
            ret = SUCCEED;
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), SUCCEED ", __function_name);
            break;
        }

        zbx_free(temp_job);
        zabbix_log(LOG_LEVEL_WARNING, "In %s(), file write process failed. try : %d", __function_name, i);
    }


    if (ret == FAIL)
    {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), jobid: " ZBX_FS_UI64 ", Unable to write to file.[%s] ", __function_name, job->jobid, data_file);

    }
    else {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(), data file: [%s] write complete.", __function_name, data_file);
    }
    zbx_free(temp_job);

    return ret;
}

int write_json_data(ja_job_object* job, char* data_file) {
    char* data = NULL;
    json_object* json_data = NULL;
    json_object* tmp_data = NULL;
    char str[JA_MAX_STRING_LEN];
    int ret,file_size, write_count;
    FILE* fp;

    const char* __function_name = "write_json_data";
    ret = FAIL;
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() and datafile :%s,", __function_name,data_file);
    json_data = json_object_new_object();
    json_object_object_add(json_data, JA_PROTO_TAG_SEND_RETRY,json_object_new_int(job->send_retry));

    ret = ja_telegram_to_head(job, json_data);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),Parsing to json head failed.[%s] ", __function_name, data_file);
        goto error;
    }

    tmp_data = json_object_new_object();
    json_object_object_add(json_data, JA_PROTO_TAG_DATA, tmp_data);
    /*
    ret = ja_telegram_to_jobresult(job, json_data);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),Parsing to json data failed(result).[%s] ", __function_name, data_file);
        goto error;
    }
    */
    ret = ja_telegram_to_request(job, json_data);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),Parsing to json data failed.[%s] ", __function_name, data_file);
        goto error;
    }
    zbx_snprintf(str, sizeof(str), ZBX_FS_UI64, job->jobid);
    json_object_object_add(tmp_data, JA_PROTO_TAG_JOBID, json_object_new_string(str));
    json_object_object_add(tmp_data, JA_PROTO_TAG_JOBSTATUS, json_object_new_int(job->status));
    json_object_object_add(tmp_data, JA_PROTO_TAG_MESSAGE, json_object_new_string(job->message));

    json_object_object_add(tmp_data, JA_PROTO_TAG_CUR_UNIQUE_ID,json_object_new_string(job->cur_unique_id));
    json_object_object_add(tmp_data, JA_PROTO_TAG_PRE_UNIQUE_ID,json_object_new_string(job->pre_unique_id));

    json_object_object_add(tmp_data, JA_PROTO_TAG_HOSTNAME,json_object_new_string(CONFIG_HOSTNAME));
    json_object_object_add(tmp_data, JA_PROTO_TAG_RESULT,json_object_new_int(job->result)); 
    json_object_object_add(tmp_data, JA_PROTO_TAG_JOBSTDOUT,json_object_new_string(job->std_out));
    json_object_object_add(tmp_data, JA_PROTO_TAG_JOBSTDERR,json_object_new_string(job->std_err));
    json_object_object_add(tmp_data, JA_PROTO_TAG_RET,json_object_new_int(job->return_code));
    json_object_object_add(tmp_data, JA_PROTO_TAG_SIGNAL,json_object_new_int(job->signal));


    data = zbx_strdup(data, (char*)json_object_to_json_string(json_data));
    if (data == NULL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),Parsing to json string failed.[%s] ", __function_name, data_file);
        ret = FAIL;
        goto error;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),json data :[%s] to be written in [%s] ", __function_name, data, data_file);
    fp = fopen(data_file, "w");
    if (fp == NULL)
    {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),failed to open data file: [%s] (%s)", __function_name, data_file, strerror(errno));
        ret =  FAIL;
        goto error;
    }
    file_size = strlen(data);
    if (0 >= file_size) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(), [data] is empty.", __function_name);
        fclose(fp);
        ret = FAIL;
        goto error;
    }
    fseek(fp, 0L, SEEK_SET);
    write_count = fwrite(data, file_size, 1, fp);
    if (write_count != 1) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(), file write failed. data : [%s], to file : %s", __function_name, data,data_file);
        ret = FAIL;
    }
    fclose(fp);
error:
    //json_object_put(tmp_data);
    json_object_put(json_data);
    zbx_free(data);
    return ret;
}
int get_char_length(char* char_pointer) {
    int count = 0;
    if (char_pointer == NULL)
        return 0;
    while (*char_pointer != '\0') {
        count++;
        char_pointer++;
    }
    return count;
}
int job_to_end(char* filename,char* filename_pid,char *folder) {
    char end[JA_MAX_STRING_LEN];
    char src[JA_MAX_STRING_LEN];
    char start[JA_MAX_STRING_LEN];
    char temp_pid[JA_FILE_NAME_LEN];
    FILE* fp;
    time_t now;
    int i = 0;
    char data_created[JA_MAX_STRING_LEN];
    char new_data_created[JA_MAX_STRING_LEN];
    const char* __function_name = "job_to_end";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),filename : %s", __function_name,filename);
    temp_pid[0] = '\0';
    ja_jobfile_getpid_by_filename(filename, temp_pid);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),pid : %s", __function_name, temp_pid);
    if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
        data_created[0] = '\0';
        new_data_created[0] = '\0';
        while (FILE_EXT[i] != NULL) {
            if (i != 0) {
                zbx_snprintf(data_created, sizeof(data_created), "%s%c%s-.%s", JA_DATA_FOLDER, JA_DLM, filename, FILE_EXT[i]);
                zbx_snprintf(new_data_created, sizeof(new_data_created), "%s%c%s.%s", JA_DATA_FOLDER, JA_DLM, filename_pid, FILE_EXT[i]);
                if (0 != rename(data_created, new_data_created)) {
                    zabbix_log(LOG_LEVEL_ERR, "In %s(),%s file cannot be renamed.(%s)", __function_name, data_created, strerror(errno));
                }
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(),%s file renamed as %s", __function_name, data_created, new_data_created);
            }
            i++;
        }
    }
    zbx_snprintf(end, sizeof(end), "%s%c%s%s", JA_END_FOLDER, JA_DLM, filename_pid, ".job");
    zbx_snprintf(src, sizeof(src), "%s%c%s%c%s%s", CONFIG_TMPDIR, JA_DLM,folder, JA_DLM, filename, "-.job");
    zbx_snprintf(start, sizeof(start), "%s%c%s.%s", JA_DATA_FOLDER, JA_DLM, filename_pid, FILE_EXT[2]);
    if (0 != rename(src, end)) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(),%s file cannot be moved.(%s)", __function_name, src, strerror(errno));
        return FAIL;
    }
    now = time(NULL);
    fp = fopen(start, "wb");
    if (fp != NULL) {
        if (fwrite(&now, sizeof(now), 1, fp) != 1) {
            zabbix_log(LOG_LEVEL_ERR, "In %s(),start time cannot be written.(%s)", __function_name,  strerror(errno));
            fclose(fp);
            return FAIL;
        }
        fclose(fp);
    }
    return SUCCEED;
}
int ja_jobfile_check_proc_kill(char* filename, char* jobid,ja_job_object* job,char* status_folder) {
    int proc_run_flag = 0, tmp_flg = 0;
    char folder[JA_FILE_PATH_LEN];
    char pre_file[JA_FILE_PATH_LEN];
    char data_file[JA_FILE_PATH_LEN];
    char temp_pid[JA_FILE_NAME_LEN];
    int loop_cnt = 0,isNewData = 0;
    DIR* exec_dir;
    struct dirent* entry;
    ja_job_object* tmp_job;
    const char* __function_name = "ja_jobfile_check_proc_kill";
    FILE* fp_chk;

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    tmp_job = NULL;
    folder[0] = '\0';
    temp_pid[0] = '\0';
    zbx_snprintf(folder, sizeof(folder), "%s%c%s", CONFIG_TMPDIR, JA_DLM, status_folder);
    exec_dir = opendir(folder);
    if (exec_dir == NULL)
    {
        zabbix_log(LOG_LEVEL_ERR, " In%s()  exec folder [%s] cannot be opened.", __function_name, folder);
        proc_run_flag = 0;
        goto contd;
    }

    tmp_job = (ja_job_object*)zbx_malloc(NULL, sizeof(ja_job_object));
    while (NULL != (entry = readdir(exec_dir)))
    {
        tmp_flg++;
        if (strcmp(entry->d_name, ".") == 0 || strcmp(entry->d_name, "..") == 0) {
            continue;
        }
        if (strncmp(entry->d_name, jobid, strlen(jobid))== 0) {
            ja_job_object_init(tmp_job);
            zbx_snprintf(filename, sizeof(entry->d_name), "%s", entry->d_name);
            if (strcmp(status_folder, "begin") == 0) {
                zbx_snprintf(pre_file, strlen(filename) - 3, "%s", filename);
                zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER, JA_DLM, pre_file);
            }else if(strcmp(status_folder,"temp") == 0){
                zbx_snprintf(pre_file, strlen(filename) - 3, "%s", filename);
                zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER, JA_DLM, pre_file);

                fp_chk = fopen(data_file,"r");
                if(fp_chk == NULL){
                    write_json_data(job,data_file);
                }else{
                    fclose(fp_chk);
                }
            }
            else {
                ja_jobfile_getpid_by_filename(filename, temp_pid);
                if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
                    zbx_snprintf(pre_file, strlen(filename) - 4, "%s", filename);
                    zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER, JA_DLM, pre_file);
                    isNewData = 1;
                }
                else {
                    get_jobid_datetime(filename, pre_file);
                    zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER, JA_DLM, pre_file);
                }
            }
            //zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER, JA_DLM, pre_file);
            if (read_datafile(tmp_job, data_file) == FAIL || NULL == tmp_job) {
                zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] data file cannot be read.", __function_name, data_file);
                break;
            }
            if (strncmp(job->serverid, tmp_job->serverid, sizeof(job->serverid)) != 0) {
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), same job id with different server ip.kill : [%s],[%s]", __function_name, job->serverip, tmp_job->serverip);
                continue;
            }
            zbx_snprintf(tmp_job->message, sizeof(tmp_job->message), "Job" ZBX_FS_UI64 " is aborted by server : %s", tmp_job->jobid, tmp_job->serverip);
            tmp_job->method = JA_AGENT_METHOD_KILL;
            if (write_data_file(tmp_job, data_file) == FAIL || NULL == tmp_job) {
                zabbix_log(LOG_LEVEL_ERR, "In %s(),cannot write to [%s] data file .", __function_name, data_file);
                break;
            }
            proc_run_flag = 1;
            break;
        }
    }
    closedir(exec_dir);
    zbx_free(tmp_job);
contd:
    return proc_run_flag;
}


int ja_create_outputfile(char* output_file, int ext_cd, int fill_res) {
    int ret = SUCCEED;
    int i = 0, fp_ret = -1;
    time_t now;
    FILE* fp;
    char tmp_output[JA_FILE_PATH_LEN];
    const char* __function_name = "ja_create_outputfile";
    //create output data files if not exists.
    while (FILE_EXT[i] != NULL) {
        if (i == 0) {
            i++;
            continue;
        }
        zbx_snprintf(tmp_output, sizeof(tmp_output), "%s%c%s.%s",JA_DATA_FOLDER, JA_DLM, output_file, FILE_EXT[i]);
        //set return code to ret file.
        if (i == 4) {
            fp = fopen(tmp_output, "wb");
            if (fp != NULL) {
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), [%s] file can be opened.", __function_name, tmp_output);
                fwrite(&ext_cd, sizeof(ext_cd), 1, fp);
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), [%s] file is updated with ext_cd : %d.", __function_name, tmp_output,ext_cd);
                fclose(fp);
            }
            else {
                zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] file cannot be read.", __function_name, tmp_output);
                ret = FAIL;
            }
        }
        else {
            //create files if does not exists.
            fp = fopen(tmp_output, "a+");
            if (fp != NULL) {
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), [%s] file can be opened.", __function_name, tmp_output);
                if(fill_res == FAIL){
                    if(i == 2 || i == 3){
                        now = time(NULL);
                        fwrite(&now, sizeof(now), 1, fp);
                    }else{
                        fwrite(&fill_res, sizeof(fill_res), 1, fp);
                    }
                }
                fclose(fp);
            }
            else {
                zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] file cannot be read.", __function_name, tmp_output);
                ret = FAIL;
            }
        }
        i++;
    }
    return ret;
    //
}

void ja_job_fetch_json_data_filename(char *tmp_file_name){
	
	DIR* dir;
	struct dirent* entry;
	int ret = FAIL;
	const char* __function_name = "ja_job_fetch_json_data_filename";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
	
	dir = opendir(JA_DATA_FOLDER);
	if (dir == NULL) {
		zabbix_log(LOG_LEVEL_ERR, "In %s, [%s] cannot be opened.%s", __function_name,JA_DATA_FOLDER,strerror(errno));
        return;
	}
	while (NULL != (entry = readdir(dir))){
		if (strstr(entry->d_name, ".json") != NULL) {

			if(strncmp(entry->d_name,tmp_file_name,strlen(tmp_file_name)) == 0 ){
				// zbx_snprintf(tmp_file_name,sizeof(entry->d_name),"%s",entry->d_name);
                zbx_snprintf(tmp_file_name, JA_FILE_PATH_LEN+1+sizeof(entry->d_name), "%s%c%s",JA_DATA_FOLDER, JA_DLM, entry->d_name);
				ret = SUCCEED;
                break;
            }
		}
	}
    if(ret != SUCCEED){
        zabbix_log(LOG_LEVEL_WARNING, "In %s, cannot find json data file for job_id:%s", __function_name,tmp_file_name);
        zbx_snprintf(tmp_file_name,sizeof(tmp_file_name),"");
    }
}