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


#ifndef JOBARG_JASTR_H
#define JOBARG_JASTR_H

char *ja_timestamp2str(const time_t time);
time_t ja_str2timestamp(const char *str);
int ja_format_timestamp(const char *input, char *output);

int ja_match(const char *string, const char *pattern);
int ja_regexp(const char *string, const char *pattern);
int ja_is_number(const char *str);
int ja_number_comp(const double num, const char *range);
int ja_number_match(const char *string, const char *pattern);

int ja_format_expr(char *input, char *output);
int ja_format_date(char *input, char *output);
int ja_format_extjob(char *input, char *output);

#ifdef _WINDOWS
LPSTR ja_unicode_to_acp(LPCTSTR wide_string);
LPSTR ja_utf8_to_acp(LPCSTR utf8_string);
LPSTR ja_acp_to_utf8(LPCSTR acp_string);
#endif

char *ja_checksum(const char *filename);
char *ja_md5(const char *str);
int ja_check_number(char *data);

#endif
