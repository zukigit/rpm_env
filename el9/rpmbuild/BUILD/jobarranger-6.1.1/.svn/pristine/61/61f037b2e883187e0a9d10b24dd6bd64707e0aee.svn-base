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

#ifndef JOBARG_JARUNICONZABBIXLINK_H
#define JOBARG_JARUNICONZABBIXLINK_H

char *jarun_icon_zabbixlink_get_trigger_message(const zbx_uint64_t inner_job_id, char* message_id);
int   jarun_icon_zabbixlink_get_item_status(const zbx_uint64_t inner_job_id, const char* get_item_id);
int   jarun_icon_zabbixlink_set_item_status(const zbx_uint64_t inner_job_id, const char* set_item_id, const int set_status);
int   jarun_icon_zabbixlink_get_item_data(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, const char* get_item_id);
int   jarun_icon_zabbixlink_get_trigger_status(const zbx_uint64_t inner_job_id, const char* get_trigger_id);
int   jarun_icon_zabbixlink_update_ids(const zbx_uint64_t inner_job_id, const char* table_name, const char* field_name, zbx_uint64_t* nextid);
int   jarun_icon_zabbixlink_insert_service_alarms(const zbx_uint64_t inner_job_id, const char* serviceid, const int status);
int   jarun_icon_zabbixlink_search_service_alarms(const zbx_uint64_t inner_job_id, const char* serviceid, const int status);
int   jarun_icon_zabbixlink_search_service_links(const zbx_uint64_t inner_job_id, const char* serviceid);
int   jarun_icon_zabbixlink_insert_events(const zbx_uint64_t inner_job_id, const char* trigger_id);
int   jarun_icon_zabbixlink_set_service(const zbx_uint64_t inner_job_id, const char* set_trigger_id, const int set_status);
int   jarun_icon_zabbixlink_set_trigger_status(const zbx_uint64_t inner_job_id, const char* set_trigger_id, const int set_status);
int   jarun_icon_zabbixlink_get_host_status(const zbx_uint64_t inner_job_id, const char* get_host_id);
int   jarun_icon_zabbixlink_set_host_status_ver18(const zbx_uint64_t inner_job_id, const char* set_host_id, const int set_status);
int   jarun_icon_zabbixlink_set_host_status(const zbx_uint64_t inner_job_id, const char* set_host_id, const int set_status, const int group_flag);
char *jarun_icon_zabbixlink_create_csv_string(const zbx_uint64_t inner_job_id, DB_RESULT result, const char* log_id);
int   jarun_icon_zabbixlink_set_host_group_status(const zbx_uint64_t inner_job_id, const char* set_group_id, const int set_status);
int   jarun_icon_zabbixlink(const zbx_uint64_t inner_job_id, const int test_flag);
int   jarun_icon_zabbixlink_check_access_permission(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, const char *group_id);

#endif

