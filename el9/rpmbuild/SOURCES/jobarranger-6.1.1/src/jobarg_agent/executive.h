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


#ifndef JOBARG_EXECUTIVE_H
#define JOBARG_EXECUTIVE_H

#include "threads.h"

ZBX_THREAD_ENTRY(executive_thread, args);
int ja_job_check_beginfile(ja_job_object* job);
//int ja_job_checkprocess(ja_job_object* job, ja_file_object* files);
//int ja_job_exec_end(ja_job_object* job);
int ja_job_checkprocess(ja_job_object* job);
int ja_job_exec_end(ja_job_object* job);
int ja_job_exec_close(ja_job_object* job);
void get_current_time(char *current_time);
ja_file_object * read_all_files(char *folder,int *file_count);
int ja_jobfile_chkend(const char *filepath, ja_job_object * job);
void ja_job_clean_exec();
#endif
