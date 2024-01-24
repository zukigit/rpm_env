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

#if defined(ZABBIX_SERVICE)
#include "service.h"
#elif defined(ZABBIX_DAEMON)
#include "daemon.h"
#endif
#ifdef _WINDOWS
#include "Windows.h"
#else
#include "unistd.h"
#include <sys/wait.h>
#endif

#include "jacommon.h"
#include <sys/types.h>
#include <stdlib.h>
#include "executive.h"
#include "stdio.h"
#include "dirent.h"
#include "errno.h"
#include "signal.h"
#include "jaagent.h"
#include "jafile.h"
#include "jajobfile.h"
#include "jastr.h"
#include "jareboot.h"
#include "listener.h"
#define EXIST 0
#define NOT_EXIST 2

extern int CONFIG_JA_EXEC_LOOP_TIMEOUT;
#ifndef _WINDOWS
int exec_mthread_pid;
#endif

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
#ifndef _WINDOWS
void catch_exec_sig_term(){
	int status;
	if(exec_mthread_pid>0){
		kill(exec_mthread_pid,SIGTERM);
        waitpid(exec_mthread_pid,&status,WUNTRACED);
	}
  	exit(0);
}
#endif

void main_executive_thread() {
	zabbix_log(LOG_LEVEL_DEBUG, "main_executive_thread start");
	ja_job_object* job;
	job = (ja_job_object*)zbx_malloc(NULL, sizeof(ja_job_object));

	//check begin files to run.
	if (ja_job_check_beginfile(job) == FAIL) {
		zabbix_log(LOG_LEVEL_ERR, "jobarg_agentd begin files check failed. ");
	}

	//process e-3 check and confirm exec files if the job process is executed or failed.
	//if(ja_job_checkprocess(job, files)==SUCCEED){
	if (ja_job_checkprocess(job) == SUCCEED) {
		//process e-4 check if the job from exec and move to end, load job process info from ret/start/end files
		if (ja_job_exec_end(job) == SUCCEED) {
			//if(ja_job_exec_end(job, files)==SUCCEED){
				//process e-5 check job end status and move to close.
			ja_job_exec_close(job);
		}

	}
	zbx_free(job);
	zabbix_log(LOG_LEVEL_DEBUG, "main_executive_thread end");
	return 0;
}
ZBX_THREAD_ENTRY(executive_thread, args)
{
    struct tm	*tm;
    time_t		now;
	int local_request_failed = 0, backup_flag = 0, write_log = 1, thread_number = 0;
	//for #180 thread status log
	ja_job_object* job;
	//thread check process
	int status;
	int alarm_pid;
#ifdef _WINDOWS
	HANDLE hThread;
	DWORD threadId;
#else
    while(exec_mthread_pid = fork()){
		signal(SIGTERM,catch_exec_sig_term);
        if ( exec_mthread_pid < 0 ) { exit(-1); }
        if(exec_mthread_pid >0 ){
            zabbix_log(LOG_LEVEL_DEBUG,"executive process execution thread : %d",exec_mthread_pid);
        }
        wait(&status);
        if (WIFEXITED(status)) {
            if ( WEXITSTATUS(status) != 10 ) { 
				exit(WEXITSTATUS(status)); 
			}else{
				zabbix_log(LOG_LEVEL_INFORMATION,"[executive] Timeout error occurred. Restart.");
			}
        }
        if (WIFSIGNALED(status)) {
            kill(getpid(),WTERMSIG(status));
        }
        sleep(1);
    }
#endif
    assert(args);

    zabbix_log(LOG_LEVEL_INFORMATION,
               "jobarg_agentd #%d started [executive]",
               ((zbx_thread_args_t *) args)->thread_num);
	thread_number = ((zbx_thread_args_t*)args)->thread_num;
    zbx_free(args);
	ja_job_clean_exec();
#ifndef _WINDOWS
	signal(SIGUSR1,alarm_handler);
	// zabbix_log(LOG_LEVEL_INFORMATION,"Config exec loop timeout  : %d",CONFIG_JA_EXEC_LOOP_TIMEOUT);
#endif


    while (ZBX_IS_RUNNING()) {
		zabbix_log(LOG_LEVEL_DEBUG, "Executive Thread loop start.\n");		
        zbx_setproctitle("process executive");
		time(&now);
		tm = localtime(&now);
		if (0 == tm->tm_hour || 4 == tm->tm_hour || 8 == tm->tm_hour || 12 == tm->tm_hour || 16 == tm->tm_hour || 20 == tm->tm_hour) {
			if (write_log == 0) {
				zabbix_log(LOG_LEVEL_INFORMATION,"jobarg_agentd[%s (revision %s)] #%d running [executive]",JOBARG_VERSION, JOBARG_REVISION,thread_number);
				write_log = 1;
			}
		}
		else {
			write_log = 0;
		}
#ifdef _WINDOWS;
			// Create a thread
		zabbix_log(LOG_LEVEL_DEBUG,"Executive thread creation.");
			hThread = CreateThread(NULL, 0, main_executive_thread, NULL, 0, &threadId);
			zabbix_log(LOG_LEVEL_DEBUG, "Executive thread created.");
			if (hThread == NULL) {
				zabbix_log(LOG_LEVEL_ERR,"Failed to create thread. Error code: %lu\n", GetLastError());
				return 1;
			}

			// Wait for the thread to finish (in this case, it never finishes)

			DWORD waitResult = WaitForSingleObject(hThread, CONFIG_JA_EXEC_LOOP_TIMEOUT * 1000);
			if (waitResult == WAIT_OBJECT_0) {
				DWORD threadExitCode;
				GetExitCodeThread(hThread, &threadExitCode); // Get the exit code of the thread

				// Check the exit code against your error codes
				switch (threadExitCode) {
				case 0:
					zabbix_log(LOG_LEVEL_DEBUG, "Executive Thread finished successfully.\n");
					break;
				default:
					zabbix_log(LOG_LEVEL_ERR, "Executive process error occurred. exit code : %d\n", threadExitCode);
					break;
				}
			}
			else if (waitResult == WAIT_FAILED) {
				zabbix_log(LOG_LEVEL_ERR, "Executive execution  thread failed with error code :[%s]", GetLastError());
			}
			else if (waitResult == WAIT_TIMEOUT) {
				zabbix_log(LOG_LEVEL_ERR, "Executive execution timed out. Restart Executive Thread.");
				TerminateThread(hThread, 0);
			}
			// Close the thread handle
			zbx_sleep(1);
			CloseHandle(hThread);
#else
		while (1) {
			// Check if any child process has terminated
			pid_t child_pid = waitpid(-1, NULL, WNOHANG);

			if (child_pid <= 0) {
				// No child process has terminated, break the loop
				break;
			}
			else {
				// A child process has terminated
				zabbix_log(LOG_LEVEL_DEBUG, "child process terminated.PID :%d ", child_pid);
			}
	}
		jaalarm(CONFIG_JA_EXEC_LOOP_TIMEOUT);
		zabbix_log(LOG_LEVEL_DEBUG, "Main executive thread start.PID :%d ", getpid());
		main_executive_thread();
		zabbix_log(LOG_LEVEL_DEBUG, "Main executive thread stopped.PID :%d ", getpid());
		jaalarm(0);
		zbx_sleep(1);
#endif
    }
error:
    zabbix_log(LOG_LEVEL_INFORMATION, "jobarg_agentd executive stopped");
    ZBX_DO_EXIT();
    zbx_thread_exit(0);
}
//E-1 check job files.
int ja_job_check_beginfile(ja_job_object* job){
	int ret = SUCCEED;
	int i, job_per_sec = 10, data_file_count = 0, begin_file_count = 0, proc_run_flag = 0;
	char data_file[JA_FILE_PATH_LEN];
	char exec_file[FILENAME_MAX];
	char begin_file[FILENAME_MAX];
	char tmp_exec_file[JA_FILE_PATH_LEN];
	char filename[FILENAME_MAX];
	char tmp_filename[FILENAME_MAX];
	ja_file_object* files = NULL;
	FILE* fp;
	const char *__function_name = "ja_job_check_beginfile";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

	//[start]check all files from begin folder.//
	///get pid from file name
	data_file[0] = '\0';
	if((files = read_all_files(JA_BEGIN_FOLDER,&begin_file_count))==NULL){
		if(begin_file_count==-1){
			ret=FAIL;
			zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), reading status files failed. file count :%d", __function_name,begin_file_count);
			goto error;
		}
	}
	data_file_count=0;
	if(begin_file_count>0){
		zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Total read file : %d", __function_name, begin_file_count);
		while(data_file_count<begin_file_count){
			ja_job_object_init(job);
			proc_run_flag = 0;
			zbx_snprintf(filename, sizeof(filename), "%s",(files+data_file_count)->filename);
			ja_jobfile_fullpath(filename, JA_TYPE_STATUS_BEGIN, begin_file);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Processing [%s]", __function_name,filename);
			size_t len = strlen(filename);
			for (i = 0; i < len - 4;i++) {
				tmp_filename[i] = filename[i];
			}
			tmp_filename[i] = '\0';
			//add get accept_datetime for comparison.
			zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json",JA_DATA_FOLDER,JA_DLM, tmp_filename);
			zbx_snprintf(exec_file, sizeof(exec_file), "%s%c%s%s", JA_EXEC_FOLDER, JA_DLM, tmp_filename,"-.job");

			if(read_datafile(job,data_file)==FAIL){
				zabbix_log(LOG_LEVEL_ERR, "In %s()  job object data cannot be read from [%s] file.", __function_name, data_file);
				fp = fopen(begin_file, "r");
				if (fp == NULL) {
					zabbix_log(LOG_LEVEL_ERR, "In %s()  job status file[%s] does not exist in begin status folder.", __function_name, begin_file);
					goto contd;
				}
				fclose(fp);
				if (FAIL != job_to_error(JA_BEGIN_FOLDER, filename))
					zabbix_log(LOG_LEVEL_WARNING, "[%s] ,along with the data files, are moved to error folder.", filename);
				goto contd;
			}

			job->status = JA_AGENT_STATUS_BEGIN;
			proc_run_flag = ja_jobfile_check_processexist(tmp_exec_file,filename,JA_BEGIN_FOLDER);
			// check for the exec file before the job run.
			if (proc_run_flag == 0) {
				//use data filecount (current loop number) as a variable and set 1 second sleep after every 4 jobs.
				if (data_file_count % job_per_sec == 0) {
					zbx_sleep(1);
				}
				zabbix_log(LOG_LEVEL_INFORMATION, "In %s(),job id :"ZBX_FS_UI64"[%s] is found, and will be starting.", __function_name,job->jobid,filename);
				//move files from begin to exec folder.
				if (rename(begin_file, exec_file) != 0) {
					//error moving files from begin folder to exec folder.
					zabbix_log(LOG_LEVEL_ERR, "In %s(), %s file cannot be moved to %s folder. : %s", __function_name, begin_file, JA_EXEC_FOLDER, strerror(errno));
					goto contd;
				}
				ret = ja_agent_run(job, filename);
				zabbix_log(LOG_LEVEL_INFORMATION, "In %s(),job id :"ZBX_FS_UI64"[%s] started and deleted begin file.", __function_name,job->jobid,filename);
			}
			else {
				// Delete begin file 
				zabbix_log(LOG_LEVEL_ERR, "In %s()  jobid: " ZBX_FS_UI64 " Job process is already running. process id : %d. delete [%s] file.", __function_name,job->jobid,job->pid,begin_file);
				if (FAIL != job_to_error(JA_BEGIN_FOLDER, filename))
					zabbix_log(LOG_LEVEL_WARNING, "[%s] ,along with the data files, are moved to error folder.", filename);
			}
			
		contd:
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Processing [%s] finished.", __function_name,filename);
			data_file_count++;
		}
	}
	zbx_free(files);
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Processing [%d] files finished.", __function_name,data_file_count);
	
error:
	return ret;
	
}

//for E-3
int ja_job_checkprocess(ja_job_object* job){
	int ret=SUCCEED;
	size_t len;
	FILE* fp;
	JA_PID result_pid;
	JA_PID child_pid;
	int extcd, file_size, file_count = 0, loop_count = 0,fail_extcd = 137;
	char check_file[JA_FILE_PATH_LEN];
	char ret_file[JA_FILE_PATH_LEN];
	char temp_pid[JA_FILE_NAME_LEN];
	char exec_file[JA_FILE_PATH_LEN];
	char current_time[20];
	char data_file[JA_FILE_PATH_LEN];
	char proc_ret_file[JA_FILE_PATH_LEN];
	char filename[JA_FILE_PATH_LEN];
	ja_file_object* files = NULL;
#ifdef _WINDOWS
	DWORD exitCode = 0;
#endif

	const char* __function_name = "ja_job_checkprocess";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

	temp_pid[0] = '\0';
		//open directory file and get filename;
	if((files = read_all_files(JA_EXEC_FOLDER,&file_count)) == NULL){
		//above function read "all" job files in the exec filder, no job id or process id to be specific.
		if(file_count==-1){
        	zabbix_log(LOG_LEVEL_ERR,"In %s(), read_all_files() error occurred. file_count : %d",__function_name, file_count);
			ret=FAIL;
			goto error;
		}
	}
	if(file_count>0){
		//process previously read exec files according to the file count.
		zabbix_log(LOG_LEVEL_DEBUG, "In %s(), total file read : %d", __function_name, file_count);
		while(loop_count<file_count){
			ja_job_object_init(job);
			///get pid from file name
			zbx_snprintf(filename, sizeof(filename), "%s",(files+loop_count)->filename);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() processing [%s]", __function_name,filename);
			ja_jobfile_fullpath(filename, JA_TYPE_STATUS_DATA, data_file);
			// [end] data file name creation
			zbx_snprintf(exec_file, sizeof(exec_file), "%s%c%s", JA_EXEC_FOLDER, JA_DLM, filename);
			ja_jobfile_getpid_by_filename(filename, temp_pid);
			if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
				zabbix_log(LOG_LEVEL_DEBUG, "In %s()  No pid yet in [%s] file.", __function_name,filename);
                zabbix_log(LOG_LEVEL_INFORMATION, "In %s()  No pid yet in [%s] file.", __function_name,filename);
				goto contd;
			}
			if (read_datafile(job, data_file) == FAIL) {
				if(FAIL != job_to_error(JA_EXEC_FOLDER, filename))
					zabbix_log(LOG_LEVEL_WARNING, "[%s] ,along with the data files, are moved to error folder.", filename);
				goto contd;
			}
			job->pid = atoi(temp_pid);
#ifdef _WINDOWS

			result_pid = 1;
			child_pid = job->pid;
			HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, child_pid);
			if (GetExitCodeProcess(hProcess, &exitCode) != 0) {
				if (exitCode == STILL_ACTIVE) {
					//process is still running.
					zabbix_log(LOG_LEVEL_DEBUG, "In %s(),job id : " ZBX_FS_UI64 ", pid : %d - Process is still active", __function_name, job->jobid, child_pid);
					result_pid = 0;
				}
				else {
					exitCode = fail_extcd;
				}
			}
			extcd = exitCode;
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(),job id : " ZBX_FS_UI64 ", pid : %d exit code : %d DWORD exit code : %d", __function_name, job->jobid, child_pid,extcd,exitCode);
			CloseHandle(hProcess);
#else
			int exitCode;
			extcd = 1;
			child_pid = job->pid;
			result_pid = kill(child_pid, 0);
			if (result_pid == -1) {
				result_pid = waitpid(child_pid, &exitCode ,WUNTRACED);
				if (exitCode == NULL || errno == ECHILD) {
					extcd = fail_extcd;
					zabbix_log(LOG_LEVEL_DEBUG, "In %s(), child exitCode is Null.(%s)", __function_name, strerror(errno));
				}
				else if (WIFSIGNALED(exitCode)) {
					extcd = WTERMSIG(exitCode);
				}
				else if (WIFEXITED(exitCode)) {
					extcd = WEXITSTATUS(exitCode);
				}
			}

#endif
			if(result_pid != 0)
			{
				//child process exited.Check file size
				zabbix_log(LOG_LEVEL_DEBUG, "In %s() child process executed. process id : %d", __function_name,child_pid);
				file_size = 0;
				zbx_snprintf(check_file, sizeof(check_file), "%s%c%s",JA_EXEC_FOLDER,JA_DLM,filename);
				file_size = ja_file_getsize(check_file);
                if(file_size == -1){
                    zabbix_log(LOG_LEVEL_ERR,"In %s(), cannot get file size. filename :%s",__function_name,check_file);
                }
				// if process exited and job file size ids less than 30, Write failed time and errorno to job file.
				if(file_size<30)
				{
					zabbix_log(LOG_LEVEL_ERR, "In %s(), jobid: " ZBX_FS_UI64 " job execution is incomplete.", __function_name, job->jobid);
					fp = fopen(check_file,"a+");
					if(fp==NULL){
						zabbix_log(LOG_LEVEL_ERR, "In %s() jobid: " ZBX_FS_UI64 ", cannot open the file : %s (%s)", __function_name,job->jobid,check_file,strerror(errno));
						goto contd;
					}
#ifdef _WINDOWS
					_lock_file(fp);
#else
					flockfile(fp);
#endif
					get_current_time(&current_time);
					fprintf(fp,"\n%s\n%d",current_time, extcd);
#ifdef _WINDOWS
					_unlock_file(fp);
#else
					funlockfile(fp);
#endif
					fclose(fp);
					//open ret file
					get_jobid_datetime(filename,proc_ret_file);
					zbx_snprintf(ret_file, sizeof(ret_file), "%s%cdata%c%s.ret",CONFIG_TMPDIR, JA_DLM,JA_DLM, proc_ret_file);
					fp = fopen(ret_file,"wb");
					if(fp!=NULL){
						fwrite(&extcd, sizeof(extcd),1, fp);
					    fclose(fp);
					}
				}else{
					zabbix_log(LOG_LEVEL_DEBUG, "In %s(),job id " ZBX_FS_UI64 " , process id : %d finished.", __function_name, job->jobid, child_pid);
				}
			}
		contd:
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() processing [%s] finished.", __function_name, filename);
			loop_count++;
		}
	}
	zbx_free(files);
error :
	zabbix_log(LOG_LEVEL_DEBUG, "In %s() processing [%d] files finished.", __function_name, loop_count);
	return ret;
}
int ja_job_exec_end(ja_job_object* job){
	int ret = SUCCEED;
	int file_size, chk, file_count = 0, loop_count = 0;
	char exec_file[JA_FILE_PATH_LEN];
	char load_filepath[JA_FILE_PATH_LEN];
	char load_filename[JA_FILE_PATH_LEN];
	char temp_pid[JA_FILE_NAME_LEN];
	char prefix[JA_FILE_PATH_LEN];
	char data_file[JA_FILE_PATH_LEN];
	char filename[JA_FILE_PATH_LEN];
	ja_file_object* files = NULL;

	const char* __function_name = "ja_job_exec_end";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

	temp_pid[0] = '\0';
	if((files = read_all_files(JA_EXEC_FOLDER,&file_count))==NULL){
		if(file_count==-1){
        	zabbix_log(LOG_LEVEL_ERR,"In %s(), read_all_files() error occurred. file_count : %d",__function_name, file_count);
			return FAIL;
		}
	}
	if(file_count>0){
		zabbix_log(LOG_LEVEL_DEBUG, "In %s(),total file read : %d", __function_name, file_count);
		while(loop_count<file_count){
			ja_job_object_init(job);
			chk = 0;
			zbx_snprintf(filename, sizeof(filename), "%s",(files+loop_count)->filename);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), processing [%s]", __function_name,filename);

			ja_jobfile_getpid_by_filename(filename, temp_pid);
			if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
				zabbix_log(LOG_LEVEL_DEBUG, "In %s()  No pid yet in [%s] file.", __function_name,filename);
                zabbix_log(LOG_LEVEL_INFORMATION, "In %s()  No pid yet in [%s] file.", __function_name,filename);
				goto end;
			}
			ja_jobfile_fullpath(filename,JA_TYPE_STATUS_EXEC, exec_file);
			file_size = ja_file_getsize(exec_file);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), [%s] file size : %d", __function_name,filename,file_size);
			ja_jobfile_fullpath(filename, JA_TYPE_STATUS_DATA, data_file);
			if (read_datafile(job, data_file) == FAIL) {
				if(FAIL != job_to_error(JA_EXEC_FOLDER, filename))
					zabbix_log(LOG_LEVEL_WARNING, "[%s] ,along with the data files, are moved to error folder.", filename);
				goto end;
			}
			job->pid = atoi(temp_pid);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job id : "ZBX_FS_UI64", process id : %d", __function_name,job->jobid,job->pid);
			zbx_snprintf(load_filename,strlen(filename)-3,"%s",filename);
			zbx_snprintf(load_filepath, sizeof(load_filepath), "%s%c%s%c%s", CONFIG_TMPDIR, JA_DLM, "data",JA_DLM,load_filename);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(), job type is : %s.", __function_name,job->type);
			if (strcmp(job->type, JA_PROTO_VALUE_REBOOT) == 0) {
				if (ja_reboot_chkend(job, load_filename,data_file) == FAIL) {
					job->result = JA_JOBRESULT_FAIL;
					zbx_snprintf(job->message, sizeof(job->message), "Reboot job failed. jobid: " ZBX_FS_UI64, job->jobid);
					zabbix_log(LOG_LEVEL_ERR, "In %s(), job reboot check failed.", __function_name);
					goto contd;
				}
			}
			if (file_size > 30) {
				chk = ja_jobfile_chkend(load_filepath,job);
			    if (chk == -1) {
		            job->result = JA_JOBRESULT_FAIL;
		            zbx_snprintf(job->message, sizeof(job->message), "Check job status(end) failed. jobid: " ZBX_FS_UI64, job->jobid);
					zabbix_log(LOG_LEVEL_ERR, "In %s(), job file check end failed.", __function_name);
			    }
				if (job->result != JA_JOBRESULT_FAIL)
					job->result = JA_JOBRESULT_SUCCEED;
				if(ja_jobfile_move(JA_EXEC_FOLDER,JA_END_FOLDER,filename)==FAIL){
					zabbix_log(LOG_LEVEL_ERR , "In %s() jobid: " ZBX_FS_UI64 ",[%s] file cannot be moved.",__function_name,job->jobid,filename);
				}
				// else{
				// 	// generate unique id for jobs
				// 	char nano_time[40];
				// 	char unique_id[JA_FILE_NAME_LEN];
				// 	char index_name[JA_FILE_NAME_LEN];
				// 	char jobfile_path[JA_FILE_PATH_LEN];
				// 	char index_file_dir[JA_FILE_PATH_LEN];

				// 	char previous[JA_FILE_NAME_LEN];
				// 	char ip_to_change[JA_JOB_ID_LEN];

				// 	get_nano_time(nano_time);

				// 	zbx_snprintf(ip_to_change,50,"%s",job->serverip);
				// 	change_ip_format(ip_to_change);
				// 	zbx_snprintf(unique_id, 50, ZBX_FS_UI64 "_%s_%s", job->jobid, nano_time, ip_to_change);

				// 	zbx_snprintf(index_name, 50, ZBX_FS_UI64 "_%s", job->jobid,ip_to_change);
				// 	filePath_for_tmpjob_agent(index_name, index_file_dir);

				// 	if(read_lastLine_from_file(index_file_dir,previous)==FAIL){
				// 		zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id), "new");
				// 	}else{
				// 		zbx_snprintf(job->pre_unique_id, sizeof(job->pre_unique_id), "%s", previous);
				// 	}

				// 	zbx_snprintf(job->cur_unique_id, sizeof(job->cur_unique_id), "%s", unique_id);
				// 	zabbix_log(LOG_LEVEL_DEBUG,"cur-id[%s] pre-id[%s]",job->cur_unique_id,job->pre_unique_id);
				// 	zabbix_log(LOG_LEVEL_ERR,"cur-id[%s] pre-id[%s]",job->cur_unique_id,job->pre_unique_id);
				// }
				zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), job id : "ZBX_FS_UI64",total files read :%d, process id : %d finished.", __function_name,job->jobid,file_count,job->pid);
			}
			else {
				zabbix_log(LOG_LEVEL_DEBUG, "In %s() file [%s] execution process is still running.", __function_name,filename);
				goto end;
			}

		contd:
			if (NULL != job) {
				job->status = JA_AGENT_STATUS_END;
				if (write_data_file(job, data_file) == FAIL) {
					zabbix_log(LOG_LEVEL_ERR, "In %s() jobid: " ZBX_FS_UI64 "failed to write [%s] file.(%s)", __function_name, job->jobid, data_file, strerror(errno));
					goto end;
				}
			}
		end:
			loop_count++;
		}
	}
	zbx_free(files);
				zabbix_log(LOG_LEVEL_DEBUG, "In %s() file [%d] execution process.", __function_name,loop_count);
	return ret;
}

int ja_job_exec_close(ja_job_object* job){
	int ret = SUCCEED;
	int file_count = 0, loop_count = 0, close_status;
	char end_file[JA_FILE_PATH_LEN];
	char data_file[JA_FILE_PATH_LEN];
	char load_filepath[JA_FILE_PATH_LEN];
	char load_filename[JA_FILE_PATH_LEN];
	char temp_pid[JA_FILE_NAME_LEN];
	char prefix[JA_FILE_PATH_LEN];
	char tmp_src[JA_FILE_PATH_LEN];
	char tmp_dest[JA_FILE_PATH_LEN];
	char filename[JA_FILE_PATH_LEN];
	char tmp_message[JA_MAX_STRING_LEN];
	char* buf = NULL;
	ja_file_object* files = NULL;
	char json_server_ip[JA_SERVERID_LEN];
	const char* __function_name = "ja_job_exec_close";
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);


	temp_pid[0] ='\0';
	if ((files = read_all_files(JA_END_FOLDER, &file_count)) == NULL) {
		if(file_count==-1){
			zabbix_log(LOG_LEVEL_ERR,"In %s(), Reading status files failed. file count :%d",__function_name,file_count);
			return FAIL;
		}
	}
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(), total read file : %d ", __function_name, file_count);
	if(file_count>0){
		zabbix_log(LOG_LEVEL_DEBUG, "In %s(), total read file : %d ", __function_name, file_count);
		while(loop_count<file_count){
			ja_job_object_init(job);
			zbx_snprintf(filename, sizeof(filename), "%s",(files+loop_count)->filename);
			ja_jobfile_fullpath(filename, JA_TYPE_STATUS_END, end_file);
			get_jobid_datetime(filename,prefix);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() processing [%s]", __function_name, end_file);
			ja_jobfile_fullpath(filename, JA_TYPE_STATUS_DATA, data_file);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() load data file [%s]", __function_name,data_file);

			if (read_datafile(job,data_file) == FAIL || NULL == job) {

				if(FAIL != job_to_error(JA_END_FOLDER, filename))
					zabbix_log(LOG_LEVEL_WARNING, "[%s] ,along with the data files, are moved to error folder.", filename);
				goto contd;
			}

			 if(getServerIPs(job->serverid, json_server_ip) == FAIL){
				if (job->send_retry >= CONFIG_SEND_RETRY) {
					job_to_error(JA_END_FOLDER, filename);
					zbx_snprintf(data_file, sizeof(data_file), "%s%cerror%c%s%c%s%s", CONFIG_TMPDIR, JA_DLM, JA_DLM, load_filename, JA_DLM, prefix, ".json");
					zabbix_log(LOG_LEVEL_ERR, "In %s() agent close failed.%s", __function_name, job->message);
					goto contd;
				}
				else {
					job->send_retry++;
					zabbix_log(LOG_LEVEL_ERR,"In %s(), Can not find server ip.", __function_name);
					goto contd;
				}
			} else if(json_server_ip != NULL){
				zbx_snprintf(job->serverip, sizeof(job->serverip), "%s", json_server_ip);
			}

			ja_jobfile_getpid_by_filename(filename, temp_pid);
			if (temp_pid == NULL || temp_pid[0] == '\0' || atoi(temp_pid) == 0) {
				zabbix_log(LOG_LEVEL_DEBUG, "In %s() job pid cannot be read from status file:%s.", __function_name, filename);
			}
			else {
				job->pid = atoi(temp_pid);
			}

			if(strlen(filename) < 6){
				zabbix_log(LOG_LEVEL_ERR,"In %s(), File name format should be wrong : %s",filename);
				continue;
			}

			zbx_snprintf(load_filename, strlen(filename) - 3, "%s", filename);
			zbx_snprintf(load_filepath, sizeof(load_filepath), "%s%c%s%c%s", CONFIG_TMPDIR, JA_DLM, "data", JA_DLM, load_filename);
			if (ja_jobfile_load(load_filepath, job) == FAIL) {
				job->result = JA_JOBRESULT_FAIL;
				if (strlen(job->message) < 1) {
					zbx_snprintf(tmp_message, sizeof(tmp_message), "%s\nCan not load the result files[%s]. jobid: " ZBX_FS_UI64, job->message, load_filepath, job->jobid);
					zbx_snprintf(job->message, sizeof(job->message), "%s", tmp_message);
				}
				zabbix_log(LOG_LEVEL_ERR, "In %s(), job file load process failed.", __function_name);
			}
			if (job->result != JA_JOBRESULT_FAIL)
				job->result = JA_JOBRESULT_SUCCEED;
			if(ja_agent_close(job, filename) ==FAIL){
				if (job->send_retry >= CONFIG_SEND_RETRY) {
					job_to_error(JA_END_FOLDER, filename);
					zbx_snprintf(data_file, sizeof(data_file), "%s%cerror%c%s%c%s%s", CONFIG_TMPDIR, JA_DLM, JA_DLM, load_filename, JA_DLM, prefix, ".json");
					zabbix_log(LOG_LEVEL_ERR, "In %s() agent close failed.%s", __function_name, job->message);
				}
				else {
					zabbix_log(LOG_LEVEL_WARNING, "In %s() agent close failed. retry count :[%d].%s", __function_name, job->send_retry, job->message);
					job->send_retry++;
					goto contd;
				}
			}
			else {
				if (ja_jobfile_move(JA_END_FOLDER, JA_CLOSE_FOLDER, filename) == FAIL) {
					zabbix_log(LOG_LEVEL_ERR, "In %s() [%s] file cannot be moved.", __function_name, filename);
					goto contd;
				}
				if (ja_jobfile_remove(load_filename) == FAIL) {
					zabbix_log(LOG_LEVEL_ERR, "In %s()  jobid: " ZBX_FS_UI64 ",job file[%s.*] under data folder move failed.", __function_name, job->jobid, load_filename);
					zbx_snprintf(tmp_src, sizeof(tmp_src), "\n Can not move data files to close status folder. jobid: " ZBX_FS_UI64, job->jobid);
					zbx_snprintf(tmp_dest, sizeof(tmp_dest), "%s", job->message);
					zbx_snprintf(job->message, sizeof(job->message), "%s%s", tmp_dest,tmp_src);

				}
				else {
					//set datafile for close folder.
					zabbix_log(LOG_LEVEL_INFORMATION, "In ja_jobfile_remove(),job id :"ZBX_FS_UI64"'s all data have been archived in close-folder.", job->jobid);
					zbx_snprintf(data_file, sizeof(data_file), "%s%cclose%c%s%c%s%s", CONFIG_TMPDIR, JA_DLM, JA_DLM, load_filename, JA_DLM, prefix, ".json");
				}
			}
			if(job->method == JA_AGENT_METHOD_KILL) ja_remove_abort_job_file(job->jobid);
		contd:
#ifdef _WINDOWS
			if (job->std_out != NULL) {
				buf = (char*)ja_utf8_to_acp(job->std_out);
				zbx_snprintf(job->std_out, sizeof(job->std_out), "%s", buf);
				zbx_free(buf);
			}
			if (job->std_err != NULL) {
				buf = (char*)ja_utf8_to_acp(job->std_err);
				zbx_snprintf(job->std_err, sizeof(job->std_err), "%s", buf);
				zbx_free(buf);
			}
#endif
			if (write_data_file(job, data_file) == FAIL) {
				zabbix_log(LOG_LEVEL_ERR, "In %s() jobid: " ZBX_FS_UI64 "failed to write [%s] file.(%s)", __function_name, job->jobid, data_file, strerror(errno));
			}
			loop_count++;
		}
	}
	zbx_free(files);
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(), %d execution finished", __function_name, loop_count);	
	return ret;
}
void get_current_time(char *current_time){
	struct tm* tm;
	time_t now = time(NULL);
	tm = localtime(&now);
	zbx_snprintf(current_time, 20, "%.4d%.2d%.2d%.2d%.2d%.2d", tm->tm_year + 1900, tm->tm_mon + 1, tm->tm_mday, tm->tm_hour, tm->tm_min, tm->tm_sec);
}
//reading all files in a folder.
ja_file_object * read_all_files(char *folder,int *file_count){
	int createFolder = 0;
	DIR * directory;
	int loop_cnt=0;
	int file_obj_len = 0;
	int limit = 0;
	struct dirent *entry;
	ja_file_object * tmp_files = NULL;
	int alarm_counter = 0;
	int cur_ja_max_read_len = 0;
	const char* __function_name = "read_all_file";
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

	#if defined _WINDOWS
	createFolder = _mkdir(folder);
	#else
	createFolder = mkdir(folder, JA_PERMISSION);
	#endif
	if(createFolder != 0 && errno != EEXIST){
		zabbix_log(LOG_LEVEL_ERR , "In %s(), Directory cannot be created.Path: %s",__function_name,folder);
		*file_count = -1;
		return NULL;
	}
	directory = opendir(folder);
	if(directory == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] cannot be opened. (%s)", __function_name, folder, strerror(errno));
		return NULL;
	}
	// while(NULL!=(entry=readdir(directory)) )
	// {
	// 	if(strcmp(entry->d_name,".")!=0 && strcmp(entry->d_name,"..")!=0){
	// 		file_obj_len++;
	// 	}
	// }
	// if(file_obj_len == 0){
	// 	*file_count=0;
	// 	closedir(directory);
	// 	return NULL;
	// }
	cur_ja_max_read_len = JA_MAX_READ_LEN ;
	tmp_files = (ja_file_object*)zbx_malloc(NULL, (size_t)cur_ja_max_read_len * sizeof(ja_file_object));
	//tmp_files = (ja_file_object*)zbx_malloc(NULL, (size_t)(file_obj_len+1) * sizeof(ja_file_object));

	if (tmp_files == NULL) {
    	zabbix_log(LOG_LEVEL_ERR, "In %s(), failed to allocate memory for tmp_files.", __function_name);
    	*file_count = -1;
		closedir(directory);
    	return NULL;
	}
	//rewinddir(directory);
	*file_count=0;
	while(NULL!=(entry=readdir(directory)) )
	{		
		// if(*file_count>file_obj_len){
		// 	//new files enter and exceed allocated file count size. Stop reading.
		// 	break;
		// }
		if(*file_count+10>= cur_ja_max_read_len ){
			cur_ja_max_read_len += 100;
			tmp_files = zbx_realloc(tmp_files,(size_t)cur_ja_max_read_len * sizeof(ja_file_object));
			if ( tmp_files == NULL ) { 
    			zabbix_log(LOG_LEVEL_ERR, "In %s(), failed to allocate memory for tmp_files.", __function_name);
    			*file_count = -1;
				closedir(directory);
    			return NULL;
			 }
        }
		if(strcmp(entry->d_name,".")!=0 && strcmp(entry->d_name,"..")!=0){
			(tmp_files + *file_count)->filename[0] = '\0';
			int dname_len = strlen(entry->d_name);
			int fname_len = sizeof((tmp_files + *file_count)->filename);
			zbx_snprintf((tmp_files + *file_count)->filename, (size_t)(dname_len+1), "%s", entry->d_name);
			zabbix_log(LOG_LEVEL_DEBUG, "In %s(),tmp_files[%d]:%s", __function_name,*file_count,(tmp_files+*file_count)->filename);
			(*file_count)++;
		}
	}
	zabbix_log(LOG_LEVEL_DEBUG, "In %s(), Done.", __function_name);
	closedir(directory);
	return tmp_files;
}
int ja_jobfile_chkend(const char *filepath, ja_job_object * job)
{
    char filename[JA_MAX_STRING_LEN];
    int ret_fsize, start_fsize, end_fsize;
	JA_PID pid;
	char *type;

#ifdef _WINDOWS
    ZBX_THREAD_HANDLE hd;
    DWORD st;
#endif
    const char *__function_name = "ja_jobfile_chkend";

    pid = job->pid;
    type = job->type;

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filepath: %s, pid: %d", __function_name, filepath, pid);

    zbx_snprintf(filename, sizeof(filename), "%s.ret", filepath);
    ret_fsize = ja_file_getsize(filename);

    zbx_snprintf(filename, sizeof(filename), "%s.start", filepath);
    start_fsize = ja_file_getsize(filename);

    zbx_snprintf(filename, sizeof(filename), "%s.end", filepath);
    end_fsize = ja_file_getsize(filename);

    //check file and make log files for the datafiles.
    if (ret_fsize < 0 || start_fsize < 0 || end_fsize < 0) {
		zabbix_log(LOG_LEVEL_ERR, "In %s() jobfile check failed.One of [%s.ret] or [%s.start] or [%s.end] files are empty. ", __function_name, filename, filename, filename);
        return -1;
    }

    if (ret_fsize > 3 && start_fsize > 3 && end_fsize > 3) {
        return 1;
    }

    //check file and make log files for the datafiles.
    if (pid == 0) {
        return 0;
    }

#ifdef _WINDOWS
    hd = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, pid);
    if (hd != NULL) {
        if (GetExitCodeProcess(hd, &st) != 0) {
            if (st == STILL_ACTIVE) {
                return 0;
            }
        }
    }
#else
    if (kill(pid, 0) == 0) {
        return 0;
    }
#endif

	zbx_sleep(3);

	zbx_snprintf(filename, sizeof(filename), "%s.ret", filepath);
	ret_fsize = ja_file_getsize(filename);

	zbx_snprintf(filename, sizeof(filename), "%s.start", filepath);
	start_fsize = ja_file_getsize(filename);

	zbx_snprintf(filename, sizeof(filename), "%s.end", filepath);
	end_fsize = ja_file_getsize(filename);

	if (strcmp(type, JA_PROTO_VALUE_REBOOT) == 0) {
		if (start_fsize > 3) {
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() reboot is complete (other than Linux)", __function_name);
			return 1;
		}
	}
	else {
		if (ret_fsize > 3 && start_fsize > 3 && end_fsize > 3) {
			zabbix_log(LOG_LEVEL_DEBUG, "In %s() job execution completion (with file write delay)", __function_name);
			return 1;
		}
	}
    zabbix_log(LOG_LEVEL_ERR, "In %s() process %d does not exist,  jobid: " ZBX_FS_UI64, __function_name, pid,job->jobid);
    return -1;
}

void ja_job_clean_exec() {
	int i = 0, fp_ret = -1,ext_cd = -1;
	DIR* dir;
	struct dirent* entry;
	char filename[JA_FILE_PATH_LEN];
	char exec_file[JA_FILE_PATH_LEN];
	char data_file[JA_FILE_PATH_LEN];
	char output_file[JA_FILE_PATH_LEN];
	char tmp_output[JA_FILE_PATH_LEN];
	char tmp_exec[JA_FILE_PATH_LEN];
	char exec_pid_file[JA_FILE_PATH_LEN];
	int loop_cnt = 0;
	ja_job_object* job;
	const char* __function_name = "ja_job_clean_exec";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s(),", __function_name);

	dir = opendir(JA_EXEC_FOLDER);
	if (dir == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "In %s, [%s] cannot be opened.%s", __function_name,JA_EXEC_FOLDER,strerror(errno));
		return;

	}
	job = (ja_job_object*)zbx_malloc(NULL, sizeof(ja_job_object));
	while (NULL != (entry = readdir(dir)))
	{
		if (strstr(entry->d_name, "-.job") != NULL) {
			ja_job_object_init(job);
			zbx_snprintf(filename, sizeof(filename), "%s", entry->d_name);
			zbx_snprintf(exec_file, sizeof(exec_file), "%s%c%s", JA_EXEC_FOLDER,JA_DLM,entry->d_name);
			zbx_snprintf(tmp_exec, strlen(filename)-4, "%s",filename);
			zbx_snprintf(output_file, strlen(filename) - 3, "%s", filename);
			zbx_snprintf(data_file, sizeof(data_file), "%s%c%s.json", JA_DATA_FOLDER,JA_DLM,tmp_exec);
			if (read_datafile(job, data_file) == FAIL || NULL == job) {
				zabbix_log(LOG_LEVEL_ERR, "In %s(), [%s] data file cannot be read.", __function_name,data_file);
				continue;
			}
			zbx_snprintf(job->message, sizeof(job->message), "jobarg_agentd stopped before executing job script. jobid: " ZBX_FS_UI64, job->jobid);
			job->result = JA_JOBRESULT_FAIL;

			if (write_data_file(job, data_file) == FAIL) {
				zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.%s", __function_name, data_file);
				continue;
			}
			if (ja_create_outputfile(output_file, ext_cd,SUCCEED) == FAIL) {
				zabbix_log(LOG_LEVEL_ERR, "In %s(), restul output file creation failed.[%s]", __function_name, output_file);
				continue;
			}
			
			zbx_snprintf(exec_pid_file, sizeof(exec_pid_file), "%s-0000",tmp_exec);
			if (job_to_end(tmp_exec, exec_pid_file,"exec") == SUCCEED) {
				zabbix_log(LOG_LEVEL_INFORMATION, "In %s() %s* file is moved from exec folder to end folder as %s*", __function_name, tmp_exec,exec_pid_file);
			}
			else {
				zbx_snprintf(job->message, sizeof(job->message), "jobarg_agentd stopped before executing job script, and files cannot be moved to end status folder. jobid: " ZBX_FS_UI64, job->jobid);
				if (write_data_file(job, data_file) == FAIL) {
					zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.%s", __function_name, data_file);
				}
				zbx_snprintf(exec_file, sizeof(exec_file), "%s.job",exec_pid_file);
				job_to_error(JA_EXEC_FOLDER, exec_file);
			}

		//end strstr.
		}
	}
	closedir(dir);
	zbx_free(job);
}
