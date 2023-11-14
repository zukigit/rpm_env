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
#include "threads.h"
#include "jatelegram.h"

#if defined(ZABBIX_SERVICE)
#include "service.h"
#elif defined(ZABBIX_DAEMON)
#include "daemon.h"
#endif
#include <stdio.h>
#include <sys/stat.h>
#include "jacommon.h"
#include "jastr.h"
#include "jafile.h"
#include "dirent.h"
#include "jajobobject.h"
#include "../jobarg_agent/jaagent.h"


#ifdef _WINDOWS
char* FILE_EXT[] = { "json", "bat", "start", "end", "ret", "stdout", "stderr", NULL };
#else
char* FILE_EXT[] = { "json", "sh", "start", "end", "ret", "stdout", "stderr", NULL };
#endif

//common global variable for folder path.
char JA_EXEC_FOLDER[JA_FILE_PATH_LEN];
char JA_END_FOLDER[JA_FILE_PATH_LEN];
char JA_DATA_FOLDER[JA_FILE_PATH_LEN];
char JA_ERROR_FOLDER[JA_FILE_PATH_LEN];
char JA_BEGIN_FOLDER[JA_FILE_PATH_LEN];
char JA_TEMP_FOLDER[JA_FILE_PATH_LEN];
char JA_CLOSE_FOLDER[JA_FILE_PATH_LEN];
char JA_JOBS_FOLDER[JA_FILE_PATH_LEN];

//timeout
extern int CONFIG_JA_EXEC_LOOP_TIMEOUT;
extern int CONFIG_JA_LISTEN_LOOP_TIMEOUT;
extern int CONFIG_JA_BKUP_LOOP_TIMEOUT;

int ja_jobfile_move(char *src_folder,char *dest_folder,char *filename){
	int createFolder;
	char dest_file[JA_FILE_PATH_LEN];
	char src_file[JA_FILE_PATH_LEN];
    struct stat      src_status;
    struct stat      dest_status;
    int src_file_exist, dest_file_exist, count = 0, src_file_size = 0, i = 0;

    const char* __function_name = "ja_jobfile_move";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
#ifdef _WINDOWS
	createFolder = _mkdir(dest_folder);
#else
	createFolder = mkdir(dest_folder, JA_PERMISSION);
#endif
	//if folder create fail without an existing folder
	if(createFolder != 0 && errno != EEXIST){
		zabbix_log(LOG_LEVEL_ERR , "Folder cannot be created.Path: %s",dest_folder);
		return FAIL;
	}
	zbx_snprintf(src_file, sizeof(src_file), "%s%c%s",src_folder,JA_DLM,filename);
	zbx_snprintf(dest_file, sizeof(dest_file), "%s%c%s",dest_folder,JA_DLM,filename);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),%s file -> to %s.",__function_name, src_file, dest_file);
    stat(src_file, &src_status);
    src_file_size = src_status.st_size;
    //remove destination file for windows.
rename:
    //try moving file for three time at most. After third time, return succeed.
    if (count > 3) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(),count exceeds 3. Exiting.", __function_name);
        return FAIL;
    }
#ifdef _WINDOWS
    //destination file with same name should not exist for windows rename. Linux rename would automatically replace destination.
    if (remove(dest_file) == 0) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(),%s file exists in destination.Removed the file.", __function_name,dest_file);
    }
#endif

    if (0 != rename(src_file, dest_file)) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(),%s file cannot be moved.(%s)", __function_name, src_file,strerror(errno));
    }
    //check for file move process
    // check if destination file exists.
    src_file_exist = stat(src_file, &src_status);
    dest_file_exist = stat(dest_file, &dest_status);
    if( dest_file_exist == 0 && (dest_status.st_size - src_file_size) == 0){
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(),%s file exists and has same file size as original source file.", __function_name, dest_file);
        //if dest file is success and src file still exist, delete the file.
        if (src_file_exist == 0) {
            //try removing file for 3 times, with 1 second gap between each try.
            while (i < 3) {
                if (remove(src_file) != 0) {
                    zabbix_log(LOG_LEVEL_WARNING, "In %s(),%s file still exist and cannot be deleted.(%s)", __function_name, src_file, strerror(errno));
                    zbx_sleep(1);
                    i++;
                }
                else {
                    zabbix_log(LOG_LEVEL_WARNING, "In %s(),%s file is deleted.", __function_name, src_file);
                    return SUCCEED;
                }
            }
            return FAIL;
        }
    }
    else {
        count++;
        zbx_sleep(1);
        goto rename;
    }
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
int ja_jobfile_create(const char *filepath,const char *script)
{
    char filename[JA_MAX_STRING_LEN];
    FILE *fp;
    int ret, err, i;
    const char *__function_name = "ja_jobfile_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    /* create result files */
    i = 0;
    while (FILE_EXT[i] != NULL) {
        if (strcmp(FILE_EXT[i], JA_EXE) == 0) {
            /* create script file */
            zbx_snprintf(filename, sizeof(filename), "%s.%s", filepath, FILE_EXT[i]);
            fp = fopen(filename, "w");
            if (fp == NULL) {
                zabbix_log(LOG_LEVEL_ERR, "In %s(),Can not open script file: %s. (%s)", __function_name, filename, strerror(errno));
                return FAIL;
            }

            err = fprintf(fp, "%s", script);
            fclose(fp);

            if (err < 0) {
                zabbix_log(LOG_LEVEL_ERR, "In %s(), can not write to script file: %s. (%s)", filename, strerror(errno));
                return FAIL;
            }

            if (chmod(filename, JA_PERMISSION) < 0) {
                zabbix_log(LOG_LEVEL_ERR, "Can not chmod script file: %s.(%s)", filename, strerror(errno));
                return FAIL;
            }

        }
        else if (strcmp(FILE_EXT[i], "json") != 0) {
            zbx_snprintf(filename, sizeof(filename), "%s.%s", filepath, FILE_EXT[i]);
            if (strcmp(FILE_EXT[i], "stdout") == 0 || strcmp(FILE_EXT[i], "stderr") == 0) {
                ret = ja_file_create(filename, JA_STD_OUT_LEN);
            }
            else {
                ret = ja_file_create(filename, 1);
            }

            if (ret == FAIL) {
                return FAIL;
            }
        }

        i++;
    }
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
int ja_jobfile_remove(const char *filename)
{
    char* pid_char;
    int move_check, i, j;
    char full_filename[JA_MAX_STRING_LEN];
    char prefix_data_file_name[JA_FILE_PATH_LEN];
    char close_new_folder_path[JA_FILE_PATH_LEN];
    const char *__function_name = "ja_jobfile_remove";
    
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

    move_check = SUCCEED;
    zbx_snprintf(close_new_folder_path, sizeof(close_new_folder_path), "%s%c%s", JA_CLOSE_FOLDER,JA_DLM,filename);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filepath: %s and archive folder in close : %s", __function_name, JA_DATA_FOLDER,close_new_folder_path);

    ///move data file to close/~~~ folder.
    pid_char = strrchr(filename, '-');
    zbx_snprintf(prefix_data_file_name, strlen(filename) - strlen(pid_char)+1, "%s", filename);
    //move stdout/stderror/.ret/.start/.end files to close/~~~ folder.
    i = 0;
    while (FILE_EXT[i] != NULL) {
        if (i == 0) {
            zbx_snprintf(full_filename, sizeof(full_filename), "%s.%s", prefix_data_file_name,FILE_EXT[i]);
        }
        else {
            zbx_snprintf(full_filename, sizeof(full_filename), "%s.%s", filename, FILE_EXT[i]);
        }
        move_check = ja_jobfile_move(JA_DATA_FOLDER,close_new_folder_path,full_filename);
        if (move_check == FAIL) {
            j = 0;
            while (j < i+1) {
                if (j == 0) {
                    zbx_snprintf(full_filename, sizeof(full_filename), "%s.%s", prefix_data_file_name, FILE_EXT[j]);
                }
                else {
                    zbx_snprintf(full_filename, sizeof(full_filename), "%s.%s", filename, FILE_EXT[j]);
                }
                ja_jobfile_move(close_new_folder_path, JA_DATA_FOLDER, full_filename);
                j++;
            }
            i = 0;
            break;
        }
        i++;
        
    }
    if (FILE_EXT[i] != NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() file move failed for [%s]", __function_name, full_filename);
        return FAIL;
    }
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
int ja_jobfile_load(const char *filepath, ja_job_object * job)
{
    int res_ret;
    time_t t;
    char filename[JA_MAX_STRING_LEN];
    char *buf;
    const char *__function_name = "ja_jobfile_load";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    if (job == NULL || filepath == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job object is null or filepath for job files does not exist.", __function_name);
        return FAIL;
    }


    buf = NULL;
    zbx_snprintf(filename, sizeof(filename), "%s.ret", filepath);
    if (ja_file_getsize(filename) == 0) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] file is empty.", __function_name,filename);
        job->return_code = 0;
        job->signal = 0;
    } else {
        //if job type is reboot process, set exit code as zero.
        if (strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0) {
            res_ret = 0;
        }
        else {
            if (ja_file_load(filename, sizeof(res_ret), &res_ret) == FAIL) {
                return FAIL;
            }
        }
#ifdef _WINDOWS
        job->return_code = res_ret;
        if (res_ret == SIGNALNO) {
            job->signal = 1;
        } else {
            job->signal = 0;
        }
#else
       if (WIFEXITED(res_ret)) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job id : "ZBX_FS_UI64", process id : %d terminated normally.", __function_name, job->jobid, job->pid);
            job->return_code = WEXITSTATUS(res_ret);
            job->signal = 0;
        } else {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job id : "ZBX_FS_UI64", process id : %d terminated with error.", __function_name, job->jobid, job->pid);
            job->return_code = WTERMSIG(res_ret);
            job->signal = 1;
        }
#endif
    }

    zbx_snprintf(filename, sizeof(filename), "%s.start", filepath);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() start file to read is: %s", __function_name, filename);
    if (ja_file_load(filename,0, &(t)) == FAIL) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() start size is: %d", __function_name, sizeof(t));
        return FAIL;
    }
    job->start_time = t;

    if (strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0) {
        job->end_time = time(NULL);
    }
    else {
        zbx_snprintf(filename, sizeof(filename), "%s.end", filepath);
        if (ja_file_getsize(filename) == 0) {
            job->end_time = job->start_time;
        }
        else {
            if (ja_file_load(filename, 0, &(t)) == FAIL) {
                return FAIL;
            }
            job->end_time = t;
        }
    }

    zbx_snprintf(filename, sizeof(filename), "%s.stdout", filepath);
    if (ja_file_load(filename, 0, &(job->std_out)) == FAIL) {
        return FAIL;
    }

#ifdef _WINDOWS
    buf = (char *) ja_acp_to_utf8(job->std_out);
    zbx_snprintf(job->std_out, sizeof(job->std_out), "%s", buf);
    zbx_free(buf);
#endif

    zbx_snprintf(filename, sizeof(filename), "%s.stderr", filepath);
    if (ja_file_load(filename, 0 , &(job->std_err)) == FAIL) {
        return FAIL;
    }
#ifdef _WINDOWS
    buf = (char *) ja_acp_to_utf8(job->std_err);
    zbx_snprintf(job->std_err, sizeof(job->std_err), "%s", buf);
    zbx_free(buf);
#endif

    return SUCCEED;
}
void alarm_handler(int signum) {
        zabbix_log(LOG_LEVEL_WARNING, "Timeout : Process execution is taking too long to response. Timeout process : %d",getpid());
        exit(10);
}
#ifdef _WINDOWS
// Function to be executed in the child process
//DWORD WINAPI sleepFunction(LPVOID lpParam) {
//    int timeout = &(int *)lpParam;
//    printf("Child process is executing MyFunction()\n");
//    Sleep(timeout);
//    //kill parent process.
//    DWORD currentProcessId = GetCurrentProcessId();
//    ja_kill_ppid(currentProcessId);
//    //kill itself.
//    ExitProcess(0);
//}
//int jaalarm(int timeout) {
//    static int pre_timeout = 0;
//    static HANDLE processHandle = NULL;
//
//    if (processHandle != NULL) {
//        TerminateProcess(processHandle, 0);
//        CloseHandle(processHandle);
//    }
//
//    processHandle = NULL;
//
//    if (timeout == 0) {
//        return 0;
//    }
//    pre_timeout = timeout;
//
//    if(processHandle = CreateThread(NULL, 0, sleepFunction, &timeout, 0, NULL) == NULL)
//    {
//        zabbix_log(LOG_LEVEL_ERR,"In jaalarm(), Cannot create child process. error :%s",strerror(errno));
//        exit(0);
//    }
//    
//    // PROCESS_INFORMATION pi;
//    // STARTUPINFO si;
//    // ZeroMemory(&si, sizeof(STARTUPINFO));
//    // si.cb = sizeof(STARTUPINFO);
//    // if (CreateProcess(NULL, "timeout.exe /t 5", NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi)) {
//    //     WaitForSingleObject(processHandle, timeout * 1000);
//    //     CloseHandle(processHandle);
//    //     CloseHandle(pi.hThread);
//    // } else {
//    //     fprintf(stderr, "Error creating child process: %d\n", GetLastError());
//    // }
//
//    return pre_timeout;
//}
#else
int jaalarm(int timeout)
{
    static pre_timeout = 0;
    static pid = 0;
    int status;
    int timeout_cnt = 0;
    if (pid > 0)
    {
        kill(pid, SIGKILL);
        waitpid(pid,&status,0);
    }
    pid = 0;
    if(timeout == 0) return 0;
    timeout_cnt = timeout;
    signal(SIGCHLD,SIG_DFL);
    pid = fork();
    if (pid < 0)
    {
        zabbix_log(LOG_LEVEL_ERR,"In jaalarm(), Cannot create child process. error :%s",strerror(errno));
        exit(0);
    }
    if (pid > 0)
    {
        pre_timeout = timeout;
        return pre_timeout;
    }
    sleep(timeout);
    kill(getppid(), SIGUSR1);
    exit(0);
}
#endif
int ja_jobfile_check_processexist(char *filename,char *full_filename) {
    char jobid_datetime[JA_FILE_NAME_LEN];
    int proc_run_flag = 0,proc_data_file_len = 0;
    DIR* exec_dir;
    struct dirent* entry;
    char* proc_data_file;
    const char* __function_name = "ja_jobfile_check_processexist";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), full_filename :%s", __function_name,full_filename);
    zbx_snprintf(jobid_datetime, strlen(full_filename) - 3, "%s", full_filename);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), jobid_datetime:%s strlen :%d", __function_name, jobid_datetime, strlen(jobid_datetime));
    proc_data_file = malloc(strlen(jobid_datetime)+2);
    proc_data_file_len = zbx_snprintf(proc_data_file, strlen(jobid_datetime)+1, "%s-", jobid_datetime);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), proc_data_file:%s strlen :%d", __function_name, proc_data_file, proc_data_file_len);
    //while((exec_dir = opendir_test(JA_EXEC_FOLDER)) ==NULL){ 
	exec_dir = opendir(JA_EXEC_FOLDER);

    //open directory above.
    if(exec_dir == NULL){
        proc_run_flag = 0;
        zabbix_log(LOG_LEVEL_ERR, "In %s(), filename : %s, cannot be opened.%s", __function_name,filename,strerror(errno));
        goto contd;  
    }
    while (NULL != (entry = readdir(exec_dir)))
    {
        if (strcmp(entry->d_name, ".") == 0 || strcmp(entry->d_name, "..") == 0) {
            continue;
        }
        if (strncmp( entry->d_name,proc_data_file, proc_data_file_len) == 0) {
            zbx_snprintf(filename, strlen(entry->d_name), "%s", entry->d_name);
            proc_run_flag = 1;
            break;
        }
    }
    closedir(exec_dir);
contd:
    zbx_free(proc_data_file);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), finished proc_run_flg:%d", __function_name,proc_run_flag);
    return proc_run_flag;

}
void get_jobid_datetime(char* filename, char* proc_data_file) {
    char* beg_char;
    size_t len;
    const char* __function_name = "get_jobid_datetime";
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), filename[%s]", __function_name, filename);

    beg_char = strrchr(filename, '-');
    len = strlen(beg_char);
    //get job id from file.
    zbx_snprintf(proc_data_file, strlen(filename) - len + 1, "%s", filename);
}

int read_datafile(ja_job_object* job, char* file_name) {
    int ret = FAIL;
    int size,i = 1, max_cnt = 5;
    const char* __function_name = "read_datafile";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (NULL == job) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() job is NULL", __function_name);
        return ret;
    }
    ja_job_object_init(job);
    size = ja_file_getsize(file_name);
    if (0 >= size) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(),data file [%s], [size : %d] is empty.", __function_name, file_name, size);
        return FAIL;
    }
    for (i = 1;i <= max_cnt;i++) {
        ret = read_json_data(job, file_name,size);
        if (ret == FAIL) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(),%d try : [%s] file cannot be read.", __function_name,i, file_name);
        }
        else {
            break;
        }
    }
    return ret;
}
void ja_jobfile_getpid_by_filename(char* filename,char *pid_str) {
    int i = 0, j = 0, jj = 0;
    const char* __function_name = "ja_jobfile_getpid_by_filename";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
    if (filename == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), filename is null.",__function_name);
        return;
    }
    while (i < strlen(filename)) {
        if (filename[i] == '-') {
            j++;
            i++;
            continue;
        }
        if (filename[i] == '\0' || filename[i] == '.') {
            pid_str[jj] = '\0';
            break;
        }
        if (j == 2) {
            pid_str[jj] = filename[i];
            jj++;
        }
        i++;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), pid str : %s.", __function_name,pid_str);
}
int read_json_data(ja_job_object* job,char* data_file,size_t size) {
    int ret = FAIL;
    FILE* fp;
    char* data = NULL;
    json_object* json_data = NULL;
    json_object* retry_count_data = NULL;
    json_object* tmp_data = NULL;
    json_object* serverip_data = NULL;
    json_object* status_data = NULL;
    json_object* message_data = NULL;
    const char* __function_name = "read_json_data";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), and data file to read is :[%s]", __function_name,data_file);
    data = zbx_calloc(data,size+1,sizeof(char));
    fp=fopen(data_file, "r");
    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot open data file [%s].error :[%s]", __function_name, data_file, strerror(errno));
        zbx_free(data);
        return FAIL;
    }
    else {
        fseek(fp, 0L, SEEK_SET);
        if (fread(data, size, 1, fp) == 0) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), data file read error: read result is 0.file : [%s] (%s).", __function_name, data_file, strerror(errno));
            ret = FAIL;
            goto error;
        }
        json_data = json_tokener_parse(data);
        if (json_data == NULL || is_error(json_data)) {
            json_data = NULL;
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse json from string. data[%s]. file [%s].", __function_name,data, data_file);
            ret = FAIL;
            goto error;
        }
        ret = ja_telegram_from_head(json_data, job);
        if (ret == FAIL) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse json head. data[%s]. file [%s].", __function_name,data, data_file);
            goto error;
        }
        retry_count_data = json_object_object_get(json_data, JA_PROTO_TAG_SEND_RETRY);
        if (retry_count_data != NULL) {
            job->send_retry = json_object_get_int(retry_count_data);
        }
        else {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse send retry json data[%s]. file [%s].", __function_name, data, data_file);
            goto error;
        }
        serverip_data = json_object_object_get(json_data, JA_PROTO_TAG_SERVERIP);
        if (serverip_data != NULL) {
            zbx_snprintf(job->serverip, sizeof(job->serverip),
                "%s", json_object_get_string(serverip_data));
        }
        else {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse server ip json data[%s]. file [%s].", __function_name,data, data_file);
            goto error;
        }
        
        ret = ja_telegram_from_request(json_data, job);
        if (ret == FAIL) {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse json data. data:[%s]. file [%s].", __function_name,data, data_file);
            goto error;
        }
        tmp_data = json_object_object_get(json_data, JA_PROTO_TAG_DATA);
        status_data = json_object_object_get(tmp_data, JA_PROTO_TAG_JOBSTATUS);
        if (status_data != NULL) {
            job->status = json_object_get_int(status_data);
        }
        else {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse status json data[%s]. file [%s].", __function_name, data, data_file);
            goto error;
        }
        message_data = json_object_object_get(tmp_data, JA_PROTO_TAG_MESSAGE);
        if (message_data != NULL) {
            zbx_snprintf(job->message, sizeof(job->message),
                "%s", json_object_get_string(message_data));
        }
        else {
            zabbix_log(LOG_LEVEL_WARNING, "In %s(), cannot parse message json data[%s]. file [%s].", __function_name, data, data_file);
            goto error;
        }
    }
    ret = SUCCEED;
error:
    fclose(fp);
    json_object_put(json_data);
    zbx_free(data);
    return ret;
}

int job_to_error(char* src_folder, char* full_filename) {
    int i = 0;
    size_t len=0;
    int folder_create_flg,ii,jj;
    char error_sub_folder[JA_FILE_PATH_LEN];
    char close_sub_folder[JA_FILE_PATH_LEN];
    char src_file[JA_FILE_PATH_LEN];
    char dest_file[JA_FILE_PATH_LEN];
    char data_filename[JA_FILE_PATH_LEN];
    char data_file[JA_FILE_PATH_LEN];
    char filename[JA_FILE_PATH_LEN];
    struct stat      stat_buf;
    const char* __function_name = "job_to_error";


    zabbix_log(LOG_LEVEL_INFORMATION, "In %s() [%s] file and it's data files will be moved to error folder.", __function_name, full_filename);


    if (src_folder == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() destination folder is NULL.", __function_name);
        return FAIL;
    }

    len = strlen(full_filename);
    zbx_strlcpy(filename, full_filename, len - 3);
#ifdef _WINDOWS
    folder_create_flg = _mkdir(JA_ERROR_FOLDER);
#else
    folder_create_flg = mkdir(JA_ERROR_FOLDER, JA_PERMISSION);
#endif
    //if folder create fail without an existing folder
    if (folder_create_flg != 0 && errno != EEXIST) {
        zabbix_log(LOG_LEVEL_ERR, "cannot create error folder.Path: %s", JA_ERROR_FOLDER);
        return FAIL;
    }
    zbx_snprintf(error_sub_folder, sizeof(error_sub_folder), "%s%c%s",JA_ERROR_FOLDER,JA_DLM,filename);
    zbx_snprintf(close_sub_folder, sizeof(close_sub_folder), "%s%c%s", JA_CLOSE_FOLDER, JA_DLM, filename);
#ifdef _WINDOWS
    folder_create_flg = _mkdir(error_sub_folder);
#else
    folder_create_flg = mkdir(error_sub_folder, JA_PERMISSION);
#endif
    //if folder create fail without an existing folder
    if (folder_create_flg != 0 && errno != EEXIST) {
        zabbix_log(LOG_LEVEL_ERR, "cannot create error/sub-folder.Path: %s", error_sub_folder);
        return FAIL;
    }
    zabbix_log(LOG_LEVEL_INFORMATION, "In %s() [%s*] files will be moved from [data] and [close] folders to %s folder.", __function_name,filename, error_sub_folder);
    while (FILE_EXT[i] != NULL) {
        //data[.json] 
        jj = 0;
        if (i == 0) {
            for (ii = 0; ii < strlen(filename); ii++) {
                data_file[ii] = filename[ii];
                if (filename[ii] == '-') {
                    jj++;
                    if (jj > 1) {
                        break;
                    }
                }
            }
            data_file[ii] = '\0';
            zbx_snprintf(data_filename, sizeof(data_filename), "%s.%s", data_file, FILE_EXT[i]);
        }
        else {
            zbx_snprintf(data_filename, sizeof(data_filename), "%s.%s", filename, FILE_EXT[i]);
        }
        zbx_snprintf(dest_file, sizeof(dest_file), "%s%c%s", error_sub_folder,JA_DLM,data_filename);

        zbx_snprintf(src_file, sizeof(src_file), "%s%c%s", JA_DATA_FOLDER, JA_DLM, data_filename);
        //log for file move warning and information fix
        //
        if (0 != rename(src_file, dest_file)) {
            if (errno != ENOENT) {
                zabbix_log(LOG_LEVEL_WARNING, "In %s() file under data folder[%s] cannot be moved to error folder.(%s)", __function_name, src_file, strerror(errno));
            }
            //take no return action here because there is a chance that .json datafile does not exist and cannot move the file.
        }
        else {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() moved file from %s to %s folder.", __function_name, src_file,dest_file);
        }
        zbx_snprintf(src_file, sizeof(src_file), "%s%c%s", close_sub_folder, JA_DLM, data_filename);
        if (0 != rename(src_file, dest_file)) {
            if (errno != ENOENT) {
                zabbix_log(LOG_LEVEL_WARNING, "In %s() file under close folder[%s] cannot be moved to error folder.(%s)", __function_name, src_file, strerror(errno));
            }
            //take no return action here because there is a chance that .json datafile does not exist and cannot move the file.
        }
        else {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() moved file from %s to %s folder.", __function_name, src_file, dest_file);
        }
        i++;
    }

    if (0 == rmdir(close_sub_folder)) {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() [%s] deleted folder.", __function_name, close_sub_folder);
    }
    else {
        if (stat(close_sub_folder, &stat_buf) == 0) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() [%s] folder, delete failed. (%s)", __function_name, close_sub_folder, strerror(errno));
            return FAIL;
        }
        else {
            zabbix_log(LOG_LEVEL_WARNING, "In %s() [%s] folder does not exist. (%s)", __function_name, close_sub_folder, strerror(errno));
        }
    }
    //move job file to error folder.
    zbx_snprintf(src_file, sizeof(src_file), "%s.%s",filename , "job");
    if (FAIL == ja_jobfile_move(src_folder, JA_ERROR_FOLDER, src_file)) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() cannot move file from %s to error folder.", __function_name, src_folder);
        return FAIL;
    }
    zabbix_log(LOG_LEVEL_INFORMATION, "In %s() move[%s] file from %s to %s folder.", __function_name, src_file, src_folder, JA_ERROR_FOLDER);
    return SUCCEED;
}
//count nubmer of files in a folder.Return success/failure value.
int get_file_count(char* folder) {
    int sub_files_count = 0;
    DIR* sub_dir;
    struct dirent* sub_entry;
    const char* __function_name = "get_file_count";

    sub_dir = opendir(folder);
    if (sub_dir == NULL) {
        zabbix_log(LOG_LEVEL_WARNING, "In %s [%s] cannot  be read.(%s)", __function_name, folder, strerror(errno));
        exit(0);
        //return sub_files_count;
    }
    else {
        sub_files_count = 0;
        //make a common file to count 7 files here.
        while ((sub_entry = readdir(sub_dir)) != NULL) {
            if (strcmp(sub_entry->d_name, ".") != 0 && strcmp(sub_entry->d_name, "..") != 0) {
                sub_files_count++;
                zabbix_log(LOG_LEVEL_DEBUG, "In %s file coutnt : %d.", __function_name, sub_files_count);
            }
        }
        closedir(sub_dir);
    }
    return sub_files_count;
}
void ja_jobfile_fullpath(char* full_filename, int folder_type, char* result_filepath) {
    char tmp_filename[JA_FILE_NAME_LEN];
    const char* __function_name = "ja_jobfile_fullpath";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
    tmp_filename[0] = '\0';
    if (full_filename == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), file name is null.", __function_name);
        return;
    }
    switch (folder_type) {
    case JA_TYPE_STATUS_DATA:
        get_jobid_datetime(full_filename, tmp_filename);
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s%s", JA_DATA_FOLDER, JA_DLM, tmp_filename, ".json");
        break;
    case JA_TYPE_STATUS_TEMP:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_TEMP_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_BEGIN:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_BEGIN_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_EXEC:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_EXEC_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_END:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_END_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_CLOSE:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_CLOSE_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_ERROR:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_ERROR_FOLDER, JA_DLM, full_filename);
        break;
    case JA_TYPE_STATUS_JOBS:
        zbx_snprintf(result_filepath, JA_FILE_PATH_LEN, "%s%c%s", JA_JOBS_FOLDER, JA_DLM, full_filename);
        break;
    default:
        zabbix_log(LOG_LEVEL_ERR, "In %s(), File type error.", __function_name);
        break;
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), result file name is : %s", __function_name,result_filepath);
}
// create temporary data files for the job.
int create_check_res_files(ja_job_object *job, char *src, char *filename)
{
    struct tm *tm;
    char *pid_char, *er_subfolder;
    char current_time[20];
    char file_err = SUCCEED;
    int ret = FAIL, i = 0;
    char message[JA_MAX_STRING_LEN];
    char data_file[JA_FILE_PATH_LEN];
    char output_file[JA_FILE_NAME_LEN];
    char load_filename[JA_FILE_NAME_LEN];
    char stat_flg_file[JA_FILE_PATH_LEN];
    char jobid_datetime[JA_FILE_NAME_LEN];
    char stat_flg_folder[JA_FILE_PATH_LEN];
    char file_checker[JA_FILE_PATH_LEN];    
    struct stat buffer;

    const char *__function_name = "create_check_res_files";
    if (src != NULL)
    {
        zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);
        zbx_snprintf(load_filename, strlen(filename) - 3, "%s", filename);
        pid_char = strrchr(filename, '-');
        zbx_snprintf(jobid_datetime, strlen(filename) - strlen(pid_char) + 1, "%s", filename);
        // zbx_snprintf(data_folder,sizeof(data_folder), "%s%c%s", src, JA_DLM, load_filename);
        //  copy data files from err/close folder to data folder.
        //  1. check stdout/stderr/start/end/ret files.
        while (FILE_EXT[i] != NULL)
        {
            if (i == 0)
            {
                zbx_snprintf(data_file, sizeof(data_file), "%s.%s", jobid_datetime, FILE_EXT[i]);
            }
            else
            {
                zbx_snprintf(data_file, sizeof(data_file), "%s.%s", filename, FILE_EXT[i]);
            }
            // check for status;
            zabbix_log(LOG_LEVEL_DEBUG, "In %s(), src : %s,JA_DATA_FOLDER:%s, data_file:%s", __function_name, src, JA_DATA_FOLDER, data_file);

            ret = ja_jobfile_move(src, JA_DATA_FOLDER, data_file);
            if (ret == FAIL)
            {
                zbx_snprintf(message, sizeof(message), "%s File from %s not found or corrupted.", data_file, src);
                zabbix_log(LOG_LEVEL_ERR, "In %s(), %s File move failed from %s to %s", __function_name, data_file, src, JA_DATA_FOLDER);
                break;
            }
            i++;
        }
        if (ret != FAIL)
        {
            zbx_snprintf(stat_flg_file, sizeof(stat_flg_file), "%s.job", filename);
            er_subfolder = strrchr(src, JA_DLM);
            zbx_snprintf(stat_flg_folder, strlen(src) - strlen(er_subfolder) + 1, "%s", src);
            ret = ja_jobfile_move(stat_flg_folder, JA_END_FOLDER, stat_flg_file);
            if (ret == FAIL)
            {
                zbx_snprintf(message, sizeof(message), "%s File from %s not found or corrupted", stat_flg_file, src);
                zabbix_log(LOG_LEVEL_ERR, "In %s(), %s File move failed from %s to %s", __function_name, stat_flg_file, src, JA_DATA_FOLDER);
            }
            else
            {
                return ret;
            }
        }
        //revert moved files.
        if(ret == FAIL){
            i = 0;
            while (FILE_EXT[i] != NULL)
            {
                if (i == 0)
                {
                    zbx_snprintf(data_file, sizeof(data_file), "%s.%s", jobid_datetime, FILE_EXT[i]);
                }
                else
                {
                    zbx_snprintf(data_file, sizeof(data_file), "%s.%s", filename, FILE_EXT[i]);
                }
                // check for status;
                zabbix_log(LOG_LEVEL_DEBUG, "In %s(), src : %s,JA_DATA_FOLDER:%s, data_file:%s", __function_name, src, JA_DATA_FOLDER, data_file);
                zbx_snprintf(file_checker, JA_FILE_PATH_LEN, "%s%c%s", JA_DATA_FOLDER, JA_DLM, data_file);
                if (stat(file_checker, &buffer) == 0) {
                    ja_jobfile_move(JA_DATA_FOLDER, src, data_file);
                }
                i++;
            }
            zbx_snprintf(stat_flg_file, sizeof(stat_flg_file), "%s.job", filename);
            er_subfolder = strrchr(src, JA_DLM);
            zbx_snprintf(stat_flg_folder, strlen(src) - strlen(er_subfolder) + 1, "%s", src);
            zbx_snprintf(file_checker, JA_FILE_PATH_LEN, "%s%c%s", JA_END_FOLDER, JA_DLM, stat_flg_file);
            if (stat(file_checker, &buffer) == 0) {
                ja_jobfile_move(JA_END_FOLDER, stat_flg_folder, stat_flg_file);
            }
        }
        return ret;
    }
    // create a file.
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), setting job message.", __function_name);
    if (src != NULL)
    {
        zbx_snprintf(job->message, sizeof(job->message), "%s", message);
    }
    else
    {
        zbx_snprintf(job->message, sizeof(job->message), "Files not found or corrupted.Error execution. jobid:" ZBX_FS_UI64, job->jobid);
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), setting job message done. Message : %s", __function_name,job->message);
    job->result = JA_JOBRESULT_FAIL;
    job->return_code = -1;
    job->signal = 1;
    zbx_snprintf(job->kind, sizeof(job->kind), "%s", JA_PROTO_VALUE_JOBRESULT);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s(), setting job kind done. kind : %s", __function_name, job->kind);
// create job files.
#ifdef _WINDOWS
    struct _timeb currentTime;
    _ftime(&currentTime);
    tm = localtime(&currentTime.time);
#else
    struct timeval currentTime;
    gettimeofday(&currentTime, NULL);
    tm = localtime(&currentTime.tv_sec);
#endif
    zbx_snprintf(current_time, 20, "%.4d%.2d%.2d%.2d%.2d%.2d", tm->tm_year + 1900, tm->tm_mon + 1, tm->tm_mday, tm->tm_hour, tm->tm_min, tm->tm_sec);

    if (ja_write_file_data(job, &current_time,0) == FAIL)
    {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), Cannot create data file. jobid:" ZBX_FS_UI64, __function_name, job->jobid);
        return FAIL;
    }
    zbx_snprintf(output_file, sizeof(output_file), ZBX_FS_UI64 "-%s-0000", job->jobid, current_time);
    if (ja_create_outputfile(output_file, -1, FAIL) == FAIL)
    {
        zabbix_log(LOG_LEVEL_ERR, "In %s(), Cannot create Output files. jobid:" ZBX_FS_UI64, __function_name, job->jobid);
        return FAIL;
    }
    zbx_snprintf(stat_flg_file, sizeof(stat_flg_file), "%s%c%s.job", JA_END_FOLDER, JA_DLM, output_file);
    if (ja_file_create(stat_flg_file, 1) == FAIL)
    {
        zabbix_log(LOG_LEVEL_ERR, "In %s() file cannot be created.%s", __function_name, stat_flg_file);
        return FAIL;
    }
}