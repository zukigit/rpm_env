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


#ifndef JOBARG_JATELEGRAM_H
#define JOBARG_JATELEGRAM_H

#include <json.h>
#include "comms.h"
#include "jacommon.h"

extern char *CONFIG_HOSTNAME;

typedef struct {
    json_object *request;
    json_object *response;
} ja_telegram_object;

int ja_telegram_new(ja_telegram_object * obj);
void ja_telegram_clear(ja_telegram_object * obj);
int ja_telegram_check(ja_telegram_object * obj);
int ja_telegram_seterr(ja_telegram_object * obj, const char *message);
int ja_telegram_recv(ja_telegram_object * obj, zbx_sock_t * s,
                     int timeout);

int ja_telegram_from(const char *telegram, ja_job_object * job);
int ja_telegram_from_head(json_object * json, ja_job_object * job);
int ja_telegram_from_request(json_object * json, ja_job_object * job);
int ja_telegram_from_response(json_object * json, ja_job_object * job);
int ja_telegram_from_jobresult_res(const char *telegram,
                                   ja_job_object * job);

char *ja_telegram_to(ja_job_object * job);
int ja_telegram_to_head(ja_job_object * job, json_object * json);
int ja_telegram_to_request(ja_job_object * job, json_object * json);
int ja_telegram_to_response(ja_job_object * job, json_object * json);
int ja_telegram_to_jobresult(ja_job_object * job, json_object * json);

#endif
