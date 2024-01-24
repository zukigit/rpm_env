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

#ifndef JOBARG_JAUSER_H
#define JOBARG_JAUSER_H

zbx_uint64_t ja_user_auth(const char *username, const char *password);
zbx_uint64_t ja_user_id(const char *username);
zbx_uint64_t ja_user_usrgrpid(zbx_uint64_t userid);
int ja_user_status(zbx_uint64_t userid);
int ja_user_type(zbx_uint64_t userid);
char *ja_user_lang(zbx_uint64_t userid);
int ja_user_groups(zbx_uint64_t userid1, zbx_uint64_t userid2);

#endif
