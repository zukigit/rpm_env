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

#include "jacommon.h"
#ifdef _WINDOWS
#include <dirent.h>
#include <stdio.h>
#include <sys/stat.h>
#include <string.h>
#include <errno.h>
#include <sys/types.h>
#endif
extern char *CONFIG_TMPDIR;
extern char *CONFIG_LOG_FILE;
#ifdef _WINDOWS
void get_nano_time(char* nano_time)
{
    int		gmtoff, ms;
    struct _timeb	tv;
    struct tm* tm;
    _ftime(&tv);
    tm = localtime(&tv.time);
    ms = tv.millitm;
   

    // Format the timestamp as YYYYMMDDHHMMSSNNNNNNNNN
    zbx_snprintf(nano_time, 20, "%.4d%.2d%.2d%.2d%.2d%.2d%.4d", tm->tm_year + 1900, tm->tm_mon + 1, tm->tm_mday, tm->tm_hour, tm->tm_min, tm->tm_sec, ms);
}
#else
void get_nano_time(char *nano_time)
{
	struct tm *tm;
	time_t now = time(NULL);
	tm = localtime(&now);

	struct timeval time_now;
	gettimeofday(&time_now, NULL);
	time_t time_str_tm = gmtime(&time_now.tv_sec);

	zbx_snprintf(nano_time, 20, "%.4d%.2d%.2d%.2d%.2d%.2d%.4d", tm->tm_year + 1900, tm->tm_mon + 1, tm->tm_mday, tm->tm_hour, tm->tm_min, tm->tm_sec, time_now.tv_usec);
}
#endif

void change_ip_format(char *ip_to_change){
	int i = 0;
	while(i < strlen(ip_to_change)){
		if(ip_to_change[i] == '.'){
			ip_to_change[i] = '-';
		}
		i++;
	}
}
void filePath_for_tmpjob_agent(const char*unique_id, char * jobfile_path){
    const char *__function_name = "filePath_for_tmpjob_agent";
    zbx_snprintf(jobfile_path, JA_FILE_PATH_LEN, "%s%cjobs%c%s.job", CONFIG_TMPDIR, JA_DLM, JA_DLM, unique_id);
    /*
    const char* lastSlash = strrchr(CONFIG_TMPDIR, JA_DLM);

    char var_file_name[JA_FILE_NAME_LEN];
    zbx_snprintf(var_file_name,sizeof(var_file_name), "%ctmp%cjobs%c%s.job",JA_DLM,JA_DLM,JA_DLM, unique_id);
    
    if (lastSlash != NULL) {
        size_t directoryLength = lastSlash - CONFIG_TMPDIR;
        zbx_strlcpy(jobfile_path, CONFIG_TMPDIR, directoryLength+1);
        strcat(jobfile_path, var_file_name);
        jobfile_path[directoryLength+ strlen(var_file_name)] = '\0';
    }
    */
}
void filePath_for_tmpjob_server(const char*unique_id, char * jobfile_path){
    const char *__function_name = "filePath_for_tmpjob_server";

    const char* lastSlash = strrchr(CONFIG_LOG_FILE, '/');

    char var_file_name[JA_FILE_NAME_LEN];
    zbx_snprintf(var_file_name,sizeof(var_file_name), "/job/%s.job", unique_id);
    
    if (lastSlash != NULL) {
        size_t directoryLength = lastSlash - CONFIG_LOG_FILE;
        zbx_strlcpy(jobfile_path, CONFIG_LOG_FILE, directoryLength+1);
        strcat(jobfile_path, var_file_name);
        jobfile_path[directoryLength+ strlen(var_file_name)] = '\0';
    }
}

int add_uid_to_job_file(char *jobfile_path,char *unique_id)
{
    const char *__function_name = "add_uid_to_job_file";
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() adding uid to file. path: %s. uid [%s]", __function_name,jobfile_path,unique_id);
    FILE *f = fopen(jobfile_path, "a+");
    if (f != NULL)
    {
#ifdef _WINDOWS
        _lock_file(f);
#else
        flockfile(f);
#endif
        fprintf(f,"%s\n",unique_id); 
#ifdef _WINDOWS
            _unlock_file(f);
#else
        funlockfile(f);
#endif
        fclose(f);
        
        return SUCCEED;
    }
    else
    {
        zabbix_log(LOG_LEVEL_ERR, "In %s() Error adding uid[%s] to file %s. Error : %s", __function_name,unique_id, jobfile_path,strerror(errno));
        return FAIL;
    }
}
int read_lastLine_from_file(char *jobfile_path,char* previous){
    const char *__function_name = "read_lastLine_from_file";
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() Reading file. flg: %s", __function_name, jobfile_path);

    FILE *file;
    file = fopen(jobfile_path, "r");

    if (file == NULL) {
        zbx_snprintf(previous, JA_MAX_STRING_LEN, "new");
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() No file found", __function_name);
        return FAIL;
    }

    char lastLine[50];
    char currentLine[50];
    lastLine[0] = '\0';
    currentLine[0] = '\0';
    while (fgets(currentLine, sizeof(currentLine), file) != NULL) {
        // Copy the current line to lastLine
        zbx_snprintf(lastLine, JA_MAX_STRING_LEN, "%s", currentLine);
    }

    if (lastLine[0] != '\0') {
        lastLine[strlen(lastLine)-1] = '\0';
        zabbix_log(LOG_LEVEL_DEBUG, "Last line from [%s] is [%s]",jobfile_path,lastLine);
        zbx_snprintf(previous, JA_MAX_STRING_LEN, "%s", lastLine);
    }

    fclose(file);
    return SUCCEED;
}

void extract_id(char *id, char *unique_id)
{
	int i = 0;
	while (i < strlen(unique_id))
	{
		if (unique_id[i] == '_')
		{
			break;
		}
		id[i] = unique_id[i];
		i++;
	}
	id[i] = '\0';
}

void get_cur_pre_filenames(char *unique_id, char *current, char *previous)
{
    const char *__function_name = "check_files_num";
    DIR *dirp;
    struct dirent *entry;
    char id[10];
    char idFromUid[10];
    char jobfile_path[JA_FILE_PATH_LEN];
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() reading[%s]", __function_name,unique_id);

    extract_id(id, unique_id);

    const char* lastSlash = strrchr(CONFIG_TMPDIR, '/');

    char var_file_name[JA_FILE_NAME_LEN];
    zbx_snprintf(var_file_name,sizeof(var_file_name), "/tmp/jobs/");
    
    if (lastSlash != NULL) {
        size_t directoryLength = lastSlash - CONFIG_TMPDIR;
        zbx_strlcpy(jobfile_path, CONFIG_TMPDIR, directoryLength+1);
        strcat(jobfile_path, var_file_name);
        jobfile_path[directoryLength+ strlen(var_file_name)] = '\0';
    }

    dirp = opendir(jobfile_path);

    memset(previous, 0, sizeof(previous));
    while ((entry = readdir(dirp)) != NULL)
    {
        if (strcmp(entry->d_name, ".") == 0 || strcmp(entry->d_name, "..") == 0) {
            continue;
        }else{
            extract_id(idFromUid, entry->d_name);

            if (strcmp(id, idFromUid) == 0)
            {
                zbx_strlcpy(current,entry->d_name,strlen(entry->d_name)+1);
                current[strlen(current) - 4] = '\0';

                if (strcmp(current, unique_id) == 0)
                {
                    break;
                }
                // set previous data
                zbx_strlcpy(previous,current,strlen(current)+1);
            }
        }
    }
    closedir(dirp);
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
int ja_file_create(const char *filename, const int size)
{
    FILE *fp;
    const char *__function_name = "ja_file_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s, size: %d",
               __function_name, filename, size);

    fp = fopen(filename, "wb");
    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "Can not open the file: %s (%s)",
                   filename, strerror(errno));
        return FAIL;
    }
    if (fseek(fp, size, SEEK_SET) != 0) {
        zabbix_log(LOG_LEVEL_ERR, "Can not seek the file: %s (%s)",
                   filename, strerror(errno));
        fclose(fp);
        return FAIL;
    }
    if (fputc(0, fp) < 0) {
        zabbix_log(LOG_LEVEL_ERR, "Can not fputc the file: %s (%s)",
                   filename, strerror(errno));
        fclose(fp);
        return FAIL;
    }
    fclose(fp);
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
int ja_file_remove(const char *filename)
{
    int ret;
    const char *__function_name = "ja_file_remove";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s", __function_name,
               filename);
    ret = SUCCEED;
    if (remove(filename) != 0) {
        zabbix_log(LOG_LEVEL_ERR, "Can not remove the file: %s (%s)",
                   filename, strerror(errno));
        ret = FAIL;
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
long ja_file_getsize(const char *filename)
{
    long size;
    FILE *fp;
    const char *__function_name = "ja_file_getsize";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s", __function_name,
               filename);

    fp = fopen(filename, "rb");
    if (fp == NULL) {
        return -1;
    }
    fseek(fp, 0, SEEK_END);
    size = ftell(fp);
    fclose(fp);

    return size;
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
int ja_file_load(const char *filename, const int size, void *data)
{
    int ret, err, fsize,read_return;
    FILE *fp;
    const char *__function_name = "ja_file_load";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s, size: %d",
               __function_name, filename, size);
    ret = SUCCEED;
    fp = fopen(filename, "rb");
    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_ERR,
                   "Can not open the file: %s (%s)", filename,
                   strerror(errno));
        return FAIL;
    }

    err = 1;
    if (size > 0) {
        fseek(fp, 0L, SEEK_SET);
        read_return = fread(data,size, sizeof(char), fp);
        if (read_return == 0) {
            err = 0;
            zabbix_log(LOG_LEVEL_ERR,"In %s() ,fread failed for file: %s, file size is : %d, read return is : %d and error is :%s", __function_name,filename, size, read_return,strerror(errno));
        }
    } else {
        fseek(fp, 0L, SEEK_END);
        fsize = ftell(fp);
        if (fsize >= JA_STD_OUT_LEN)
            fsize = JA_STD_OUT_LEN - 1;
        fseek(fp, 0L, SEEK_SET);
        if (fsize != 0) {
            if (fread(data, fsize, sizeof(char), fp) == 0) {
                zabbix_log(LOG_LEVEL_ERR, "In %s() ,fread failed for file: %s, file size is :%d,and error is :%s", __function_name, filename, fsize, strerror(errno));
                err = 0;
            }
        }

    }
    if (err != 1) {
        zabbix_log(LOG_LEVEL_ERR,
                   "Can not read result file: %s (%s)", filename,
                   strerror(errno));
        ret = FAIL;
    }
    fclose(fp);
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
#ifdef _WINDOWS
#else
int ja_file_is_regular(const char *path)
{
    struct stat st;

    if (0 == lstat(path, &st) && 0 != S_ISREG(st.st_mode))
        return SUCCEED;

    return FAIL;
}
#endif
