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


#ifndef JOBARG_JAJOBFILE_H
#define JOBARG_JAJOBFILE_H

int ja_jobfile_move(char* src_folder, char* dest_folder, char* filename);
int ja_jobfile_create(const char *filepath, const char *script);
int ja_jobfile_remove(const char *filepath);
void ja_remove_abort_job_file(zbx_uint64_t jobid);
int ja_jobfile_load(const char *filepath, ja_job_object * job);
int ja_jobfile_check_processexist(char *filename, char* jobid_datetime,char* cur_dir);
void get_jobid_datetime(char* filename, char* proc_data_file);
void ja_jobfile_getpid_by_filename(char* filename, char* pid_str);
int read_datafile(ja_job_object* job, char* file_name);
int job_to_error(char* src_folder, char* filename);
int get_file_count(char* folder);
void ja_jobfile_fullpath(char* full_filename, int folder_type, char* result_filepath);
int create_check_res_files(ja_job_object *job, char *src, char *filename);
void alarm_handler(int signum);
int jaalarm(int timeout);
extern char* FILE_EXT[];

//common global variable for folder path.
extern char JA_EXEC_FOLDER[JA_FILE_PATH_LEN];
extern char JA_END_FOLDER[JA_FILE_PATH_LEN];
extern char JA_DATA_FOLDER[JA_FILE_PATH_LEN];
extern char JA_ERROR_FOLDER[JA_FILE_PATH_LEN];
extern char JA_BEGIN_FOLDER[JA_FILE_PATH_LEN];
extern char JA_TEMP_FOLDER[JA_FILE_PATH_LEN];
extern char JA_CLOSE_FOLDER[JA_FILE_PATH_LEN];
extern char JA_JOBS_FOLDER[JA_FILE_PATH_LEN];
extern char JA_IPS_FOLDER[JA_FILE_PATH_LEN];
#endif
