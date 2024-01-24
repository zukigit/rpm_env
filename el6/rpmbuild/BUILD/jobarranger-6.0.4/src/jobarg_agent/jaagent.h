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


#ifndef JOBARG_JAAGENT_H
#define JOBARG_JAAGENT_H

#include "comms.h"

extern char *CONFIG_REQUEST_FLAG;
extern char *CONFIG_CMD_FILE;
extern char *CONFIG_TMPDIR;
extern int CONFIG_SEND_RETRY;
extern char *CONFIG_SOURCE_IP;
extern char *CONFIG_HOSTS_ALLOWED;
extern int CONFIG_SERVER_PORT;
extern int CONFIG_TIMEOUT;
extern char *CONFIG_REBOOT_FLAG;
extern char *CONFIG_REBOOT_FILE;
extern char *CONFIG_JA_COMMAND_USER;
extern char *CONFIG_JA_COMMAND_PASSWORD;

int ja_agent_setenv(ja_job_object * job, char *env_string);
int ja_agent_kill(ja_job_object * job,char *current_time);
int ja_agent_begin(ja_job_object* job, char* current_time, char* datafile);
int ja_agent_run(ja_job_object * job,char *data_filename);
int ja_agent_close(ja_job_object* job, char* filename);
int ja_agent_send(ja_job_object * job);
//Added by ThihaOo@DAT : 12/01/2021
int ja_create_status_filepath(ja_job_object * job,char *filename,char *current_time,int file_type);
int ja_delete_data(ja_job_object* job,char *current_time);
int ja_jobstatus_file(ja_job_object* job,char *current_time);
int ja_write_file_data(ja_job_object* job, char* current_time,int file_type);
int write_data_file(ja_job_object* job, char* data_file);
int write_json_data(ja_job_object* job, char* data_file);
int get_char_length(char* char_pointer);
int job_to_end(char* filename, char* filename_pid, char* folder);
void ja_job_clean_exec();
int ja_jobfile_check_proc_kill(char* filename, char* jobid_datetime, ja_job_object* job, char* status_folder);
int ja_create_outputfile(char* output_file, int ext_cd, int fill_res);
void ja_job_fetch_json_data_filename(char *filename);
#endif
