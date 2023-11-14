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
#include "jajoblog.h"
#include "jaself.h"
#include "jarunnormal.h"
#include "jarunskip.h"
#include "jarun.h"
#include "jarunstatuscheck.h"

extern unsigned char process_type;
extern int process_num;
char var_file_dir[256] = "/tmp/jobstatus.chk";
const char* var_file_name = "/jobstatus.chk";
int* getRebootRunning(int sql_flag, char* hostname, int* joblist, int count_job,int* reboot_job_list);

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
static int process_jarun()
{
    int ret;
    DB_RESULT result;
    DB_RESULT result2;
    DB_RESULT result3;
    DB_RESULT result4;
    DB_ROW row;
    DB_ROW row2;
    DB_ROW row3;
    DB_ROW row4;
    double sec;
    zbx_uint64_t inner_job_id, inner_jobnet_id;
    int method_flag, job_type, test_flag;

    static pid_t pid_check = -1;
    JA_PID result_pid;
    
    const char *__function_name = "process_jarun";

    ret = FAIL;
    sec = zbx_time();
    result =
        DBselect("select inner_job_id, inner_jobnet_id, method_flag, job_type, test_flag from ja_run_job_table"
                 " where status = %d ",
                 JA_JOB_STATUS_READY);
    sec = zbx_time() - sec;
    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() ja_run_job_table(status = READY): " ZBX_FS_DBL
               " sec.",
               __function_name, sec);

    while (NULL != (row = DBfetch(result)))
    {
        ZBX_STR2UINT64(inner_job_id, row[0]);
        ZBX_STR2UINT64(inner_jobnet_id, row[1]);
        method_flag = atoi(row[2]);
        job_type = atoi(row[3]);
        test_flag = atoi(row[4]);
        zabbix_log(LOG_LEVEL_DEBUG,
                   "In %s() inner_job_id: " ZBX_FS_UI64
                   ", inner_jobnet_id: " ZBX_FS_UI64
                   ", method_flag: %d, job_type: %d, test_flag: %d",
                   __function_name, inner_job_id, inner_jobnet_id,
                   method_flag, job_type, test_flag);

        ret = SUCCEED;
        DBbegin();
        result2 = DBselect("select inner_job_id from ja_run_job_table where inner_job_id = " ZBX_FS_UI64 " and status = %d for update", inner_job_id, JA_JOB_STATUS_READY);
        if (NULL == (row2 = DBfetch(result2)))
        {
            DBcommit();
            DBfree_result(result2);
            continue;
        }
        DBfree_result(result2);

        switch (method_flag)
        {
        case JA_JOB_METHOD_NORMAL:
            ja_joblog(JC_JOB_START, inner_jobnet_id, inner_job_id);
            ret = jarun_normal(inner_job_id, job_type, test_flag);
            break;
        case JA_JOB_METHOD_WAIT:
            ret = FAIL;
            break;
        case JA_JOB_METHOD_SKIP:
            ret = jarun_skip(inner_job_id, inner_jobnet_id, job_type);
            break;
        case JA_JOB_METHOD_ABORT:
            ja_log("JARUN300001", inner_jobnet_id, NULL, inner_job_id,
                   __function_name, inner_job_id);
            break;
        case JA_JOB_METHOD_RERUN:
            ja_joblog(JC_JOB_RERUN, inner_jobnet_id, inner_job_id);
            ret = jarun_normal(inner_job_id, job_type, test_flag);
            break;
        default:
            ja_log("JARUN200001", inner_jobnet_id, NULL, inner_job_id,
                   method_flag, inner_job_id, job_type);
            break;
        }

        if (ret == SUCCEED)
        {
            DBcommit();
        }
        else
        {
            DBrollback();
        }
    }
    DBfree_result(result);

    if (pid_check > 0) {
        result_pid = kill(pid_check, 0);
        if (result_pid == 0) {
            zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
            return ret;
        }
    }

    pid_check = ja_fork();
    if (pid_check == 0) {
        DBconnect(ZBX_DB_CONNECT_ONCE);
        double next_execution_time_for_request  = -1;
        char last_hostname[256] = "";
        int sql_flag = 0;
        char hostname[256];
        int size_of_joblist=512;
        int size_of_reboot_joblist=512;
        int* joblist = (int*)malloc(size_of_joblist * sizeof(int));
        int* reboot_job_list = (int*)malloc(size_of_reboot_joblist * sizeof(int));
        int count_job = 0;
        hostname[0] = 0x00;
        reboot_job_list[0] = -1;
        readVariablesFromFile(var_file_dir, &next_execution_time_for_request, last_hostname, &sql_flag);

        zabbix_log(LOG_LEVEL_DEBUG,
                    "In %s() status check hostname: %s, sql_flag: %d",
                    __function_name, last_hostname, sql_flag);
        
        if (next_execution_time_for_request < zbx_time()) {
            if(sql_flag == 0) {
                result3 = DBselect("select c.host_name, b.inner_job_id, b.inner_jobnet_id"
                                   " from ( select a.inner_job_id,a.inner_jobnet_id from ja_run_job_table as a"
                                   " where a.job_type = %d and a.status in(%d,%d)) as b,"
                                   " ja_run_icon_job_table as c"
                                   " where c.host_flag = 0 and b.inner_job_id = c.inner_job_id and c.host_name > '%s'"
                                   " order by c.host_name asc", JA_JOB_TYPE_JOB, JA_JOB_STATUS_ABORT,JA_JOB_STATUS_RUN, 
                                   last_hostname);
            }else{
                result3 = DBselect("select f.inner_jobnet_id, e.inner_job_id, f.before_value as host_name, f.seq_no"
                                   " from"
                                   " ( select g.inner_job_id, max(g.seq_no) as max_seq_no"
                                   " from ja_run_value_before_table g,"
                                   " ( select c.host_name, b.inner_job_id"
                                   " from ( select a.inner_job_id from ja_run_job_table as a"
                                   " where a.job_type = %d and a.status in(%d,%d) ) as b,"
                                   " ja_run_icon_job_table as c"
                                   " where c.host_flag = 1 and b.inner_job_id = c.inner_job_id) as d"
                                   " where g.inner_job_id = d.inner_job_id and g.value_name = d.host_name"
                                   " group by g.inner_job_id ) as e,"
                                   " ja_run_value_before_table f"
                                   " where e.max_seq_no = f.seq_no and f.before_value > '%s'"
                                   " order by f.before_value asc",
                                   JA_JOB_TYPE_JOB, JA_JOB_STATUS_ABORT,JA_JOB_STATUS_RUN,  last_hostname);
            }
            while (NULL != (row3 = DBfetch(result3)))
            {
                if (hostname[0] == 0x00){
                    zbx_strlcpy(hostname, row3[0], sizeof(hostname));
                }
                if (strcmp(hostname, row3[0]) != 0 ) {
                    break;
                }
                if (count_job > 510) { 
                    size_of_joblist += 512;
                    int* resized_joblist = (int*)realloc(joblist, size_of_joblist * sizeof(int));
                    joblist = resized_joblist;
                }
                joblist[count_job] = atoi(row3[1]);
                count_job++;
            }
            if(count_job == 0){
                result4 = DBselect("SELECT lock_host_name FROM ja_host_lock_table WHERE lock_host_name > '%s' AND lock_host_name != 'HOST_LOCK_RECORD'"
                                   " order by lock_host_name asc", last_hostname);
                if (NULL != (row4 = DBfetch(result4))){
                    zbx_strlcpy(hostname, row4[0], sizeof(hostname));
                    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host '%s' is locked", __function_name, hostname);
                    //count_job = getRebootRunning(sql_flag, hostname, joblist, count_job);
                    count_job = getRebootRunning(sql_flag, hostname, joblist, count_job,reboot_job_list);
                    
                }
                DBfree_result(result4);
            }else{
                if (ja_host_lockinfo(hostname) == SUCCEED) {
                    zabbix_log(LOG_LEVEL_DEBUG, "In %s() host '%s' is locked", __function_name, hostname);
                    //count_job = getRebootRunning(sql_flag,hostname, joblist, count_job);
                    count_job = getRebootRunning(sql_flag, hostname, joblist, count_job,reboot_job_list);
                }            
            }
            DBfree_result(result3);
            
            if (count_job > 0) {
                zbx_strlcpy(last_hostname, hostname, sizeof(last_hostname));
                //ret = jarun_status_check(hostname, joblist, count_job);
                ret = jarun_status_check(hostname, joblist, count_job,reboot_job_list);
            }
            
            if( sql_flag > 0 && count_job == 0 ) {
            sql_flag = 0; 
            last_hostname[0] = 0x00;
            next_execution_time_for_request = zbx_time() + 60;
            }else{
                if( sql_flag == 0 && count_job == 0) {
                sql_flag++; 
                last_hostname[0] = 0x00;
                }
            }
            saveVariablesToFile(var_file_dir, next_execution_time_for_request, last_hostname, sql_flag);
        }
        free(joblist);
        DBclose();
        exit(0);
    }else if (pid_check == -1) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() failed to create child process.", __function_name);
        zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
        return ret;
    }else{
        waitpid(pid_check, NULL, WNOHANG);
        zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
        return ret;
    }
}

//int* getRebootRunning(int sql_flag, char* hostname, int* joblist, int count_job)
int* getRebootRunning(int sql_flag, char* hostname, int* joblist, int count_job,int* reboot_job_list)
{
    DB_RESULT result;
    DB_ROW row;
    int size_of_joblist=512;
    int size_of_reboot_joblist=512;
    int count_reboot_job = 0;
    if (sql_flag == 0) {
        result = DBselect("select b.inner_job_id"
            " from ( select a.inner_job_id,a.inner_jobnet_id from ja_run_job_table as a"
            " where a.job_type = %d and a.status = %d ) as b,"
            " ja_run_icon_reboot_table as c"
            " where c.host_flag = 0 and b.inner_job_id = c.inner_job_id and c.host_name = '%s'"
            " order by c.host_name asc", JA_JOB_TYPE_REBOOT, JA_JOB_STATUS_RUN,
            hostname);
    } else {
        result = DBselect("select e.inner_job_id"
            " from"
            " ( select g.inner_job_id, max(g.seq_no) as max_seq_no"
            " from ja_run_value_before_table g,"
            " ( select c.host_name, b.inner_job_id"
            " from ( select a.inner_job_id from ja_run_job_table as a"
            " where a.job_type = %d and a.status = %d ) as b,"
            " ja_run_icon_reboot_table as c"
            " where c.host_flag = 1 and b.inner_job_id = c.inner_job_id) as d"
            " where g.inner_job_id = d.inner_job_id and g.value_name = d.host_name"
            " group by g.inner_job_id ) as e,"
            " ja_run_value_before_table f"
            " where e.max_seq_no = f.seq_no and f.before_value = '%s'"
            " order by f.before_value asc",
            JA_JOB_TYPE_REBOOT, JA_JOB_STATUS_RUN, hostname);
    }
    
    while (NULL != (row = DBfetch(result))) {
        if (count_job > 510) { 
            size_of_joblist += 512;
            int* resized_joblist = (int*)realloc(joblist, size_of_joblist * sizeof(int));
            joblist = resized_joblist;
        }
        joblist[count_job] = atoi(row[0]);

        if(count_reboot_job > 510){
            
            size_of_reboot_joblist += 512;
            int* resized_reboot_joblist = (int*)realloc(reboot_job_list, size_of_reboot_joblist * sizeof(int));
            reboot_job_list = resized_reboot_joblist;
        }
        reboot_job_list[count_reboot_job] = atoi(row[0]);
        count_reboot_job++;
        count_job++;
    }
    reboot_job_list[count_reboot_job] = -1;

        /*
            if (count_job > 510) { 
                size_of_joblist += 512;
                int* resized_joblist = (int*)realloc(joblist, size_of_joblist * sizeof(int));
                reboot_job_list = resized_joblist;
            }
            reboot_job_list[index] = atoi(row[0]);
            count_job++;
            index++;
        }
        */

    DBfree_result(result);
    return count_job;
}              

void saveVariablesToFile(const char* filename, double next_execution_time_for_request, const char* last_hostname, int sql_flag) {
    const char *__function_name = "saveVariablesToFile";
    FILE* fp = fopen(filename, "w");
    if (fp != NULL) {

        char next_execution_str[20];
        if (next_execution_time_for_request == -1) {
            zbx_snprintf(next_execution_str, sizeof(next_execution_str), "-1");
        }else{
            zbx_snprintf(next_execution_str, sizeof(next_execution_str), "%lf", next_execution_time_for_request);
        }

        char sql_flag_str[20];
        zbx_snprintf(sql_flag_str, sizeof(sql_flag_str), "%d", sql_flag);

        fprintf(fp, "%s\n", next_execution_str);
        fprintf(fp, "%s\n", last_hostname);
        fprintf(fp, "%s\n", sql_flag_str);
        fclose(fp);
    } else {
        zabbix_log(LOG_LEVEL_ERR,"In %s() Error opening file for writing. %s", __function_name,filename);
    }
}

void readVariablesFromFile(const char* filename, double* next_execution_time_for_request, char* last_hostname, int* sql_flag) {
    const char *__function_name = "readVariablesFromFile";

    FILE* fp = fopen(filename, "r");
    if (fp != NULL) {
        char next_execution_str[20];
        char sql_flag_str[20];

        fgets(next_execution_str, sizeof(next_execution_str), fp);
        *next_execution_time_for_request = strtod(next_execution_str, NULL);

        fgets(last_hostname, 256, fp);
        size_t newlinePos = strcspn(last_hostname, "\n");
        if (last_hostname[newlinePos] == '\n') {
            last_hostname[newlinePos] = '\0';
        }

        fgets(sql_flag_str, sizeof(sql_flag_str), fp);
        *sql_flag = strtol(sql_flag_str, NULL, 10);

        fclose(fp);
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
void main_jarun_loop()
{
    zabbix_log(LOG_LEVEL_DEBUG,
               "In main_jarun_loop() process_type:'%s' process_num:%d",
               ja_get_process_type_string(process_type), process_num);

    zbx_setproctitle("%s [connecting to the database]",
                     ja_get_process_type_string(process_type));

    const char* lastSlash = strrchr(CONFIG_LOG_FILE, '/');
    
    if (lastSlash != NULL) {
        size_t directoryLength = lastSlash - CONFIG_LOG_FILE;
        zbx_strlcpy(var_file_dir, CONFIG_LOG_FILE, directoryLength+1);
        strcat(var_file_dir, var_file_name);
        var_file_dir[directoryLength+ strlen(var_file_name)] = '\0';

    }

    DBconnect(ZBX_DB_CONNECT_NORMAL);
    for (;;) {
        zbx_setproctitle("%s [processing data]",
                         ja_get_process_type_string(process_type));
        if (process_jarun() == SUCCEED)
            continue;
        ja_sleep_loop(CONFIG_JARUN_INTERVAL);
    }
}
