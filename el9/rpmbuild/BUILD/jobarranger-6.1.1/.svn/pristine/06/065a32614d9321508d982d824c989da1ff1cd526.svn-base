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

#ifndef JOBARG_JATRAPJOBLOGPUT_H
#define JOBARG_JATRAPJOBLOGPUT_H

#include "jatelegram.h"

typedef struct {
    char *username;
    char *password;
    char *from_time;
    char *to_time;
    char *jobnetid;
    char *jobid;
    char *target_user;
    char *registry_number;
} ja_joblogput_object;

int jatrap_joblogput_load(ja_joblogput_object * job,
                          ja_telegram_object * obj);
void jatrap_joblogput_free(ja_joblogput_object * job);
int jatrap_joblogput(ja_telegram_object * obj);

#endif
