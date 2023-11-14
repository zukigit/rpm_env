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


#ifndef JOBARG_JAARCHIVE_H
#define JOBARG_JAARCHIVE_H

#include <archive.h>

typedef struct {
    char buf[512];
    int fd;
} ja_archive_data;

int ja_archive_open(struct archive *a, void *client_data);
ssize_t ja_archive_read(struct archive *a, void *client_data,
                        const void **buf);
ssize_t ja_archive_write(struct archive *a, void *client_data,
                         const void *buf, size_t len);
int ja_archive_close(struct archive *a, void *client_data);

int ja_archive_chksum(const char *dir, const char *filename,
                      json_object * json, ja_job_object * job);
int ja_archive_add(struct archive *a, const char *filename,
                   const char *pathname);
int ja_archive_create(const char *dir, const char *filename, const int fd,
                      ja_job_object * job);
int ja_archive_extract(const int fd, char *dir, const int overwrite,
                       ja_job_object * job);

#endif
