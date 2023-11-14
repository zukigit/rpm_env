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


#ifndef JOBARG_JATAR_H
#define JOBARG_JATAR_H

ssize_t ja_tar_read(int fd, const void *buf, size_t len);
ssize_t ja_tar_write(int fd, void *buf, size_t len);
int ja_tar_close(int fd);

int ja_tar_chksum(const char *dir, const char *filename,
                  json_object * json, ja_job_object * job);
int ja_tar_create(const char *dir, const char *filenam, const int fd,
                  ja_job_object * job);
int ja_tar_extract(const int fd, char *dir, const int overwrite,
                   ja_job_object * job);

#endif
