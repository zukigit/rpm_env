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


#define _GNU_SOURCE
#include <time.h>

#include "common.h"
#include "log.h"
int mthread_pid;

/******************************************************************************
 *                                                                            *
 * Function: ja_timeout_check                                                 *
 *                                                                            *
 * Purpose: check whether the timeout has occurred                            *
 *                                                                            *
 * Parameters: timeout (in) - timeout period (minute)                         *
 *             start_time (in) - job icon start time (YYYYMMDDHHMMSS)         *
 *                                                                            *
 * Return value:  SUCCEED - timeout occurs                                    *
 *                FAIL - no timeout                                           *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_timeout_check(char *timeout, char *start_time)
{
    struct tm start_time_tm;
    time_t now, timeout_sec, start_time_sec;
    const char *__function_name = "ja_timeout_check";

    if (timeout == NULL || start_time == NULL)
        return FAIL;

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() timeout: %s, start_time: %s",
               __function_name, timeout, start_time);

    timeout_sec = atoi(timeout) * 60;
    if (timeout_sec == 0 || atoi(start_time) == 0)
        return FAIL;


    now = time(NULL);
    strptime(start_time, "%Y%m%d%H%M%S", &start_time_tm);
    start_time_sec = mktime(&start_time_tm);

    if (start_time_sec + timeout_sec > now)
        return FAIL;
    else
        return SUCCEED;
}
void catch_sig_term(){
	int status;
	if(mthread_pid>0){
		kill(mthread_pid,SIGTERM);
        waitpid(mthread_pid,&status,WUNTRACED);
	}
  	exit(0);
}

void ja_alarm_handler(int signum) {
        zabbix_log(LOG_LEVEL_WARNING, "Timeout : Process is taking too long to response. Timeout process : %d",getpid());
        exit(10);
}
void ja_alarm_watcher(char* process_name){
    int status;
	signal(SIGUSR1,ja_alarm_handler);
    while(mthread_pid = fork()){
		signal(SIGTERM,catch_sig_term);
        if (mthread_pid < 0 ) { exit(-1); }
        if(mthread_pid >0 ){
            zabbix_log(LOG_LEVEL_DEBUG,"%s process execution thread : %d",process_name,mthread_pid);
            zabbix_log(LOG_LEVEL_INFORMATION,"%s process execution thread : %d",process_name,mthread_pid);
        }
        wait(&status);
        if (WIFEXITED(status)) {
            if ( WEXITSTATUS(status) != 10 ) { 
				exit(WEXITSTATUS(status)); 
			}else{
				zabbix_log(LOG_LEVEL_INFORMATION,"[%s] Timeout error occurred. Restart.",process_name);
			}
        }
        if (WIFSIGNALED(status)) {
            kill(getpid(),WTERMSIG(status));
        }
        sleep(1);
    }
}

int ja_alarm_timeout(int timeout)
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
    //signal(SIGCHLD,SIG_DFL);
    pid = ja_fork();
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
    int remain = 0;    
    while (remain<timeout){
        sleep(1);
        remain++;
    }
    kill(getppid(), SIGUSR1);
    exit(0);
}
