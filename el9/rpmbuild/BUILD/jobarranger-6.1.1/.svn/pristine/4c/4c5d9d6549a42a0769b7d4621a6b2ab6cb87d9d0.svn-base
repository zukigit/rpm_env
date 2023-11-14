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

#include "common.h"
#include "log.h"
#include "db.h"
#include "dbcache.h"

#include "jacommon.h"
#include "jalog.h"
#include "jazbxmsg.h"

/******************************************************************************
 *                                                                            *
 * Function: ja_zabbix_message                                                *
 *                                                                            *
 * Purpose: get Zabbix status change message body                             *
 *                                                                            *
 * Parameters: message_id (in) - message id                                   *
 *             lang (in) - message language                                   *
 *             inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value:  pointer of the message body                                 *
 *                NULL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
char *ja_zabbix_message(char *message_id, char *lang, zbx_uint64_t inner_job_id)
{
	FILE		*fp;
	int		hit, n, cnt;
	char		*name = NULL, *msg = NULL, *p = NULL;
	char		fname[1024], line[4096];
	const char	*__function_name = "ja_zabbix_message";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s([%s] [%s])", __function_name, message_id, lang);

	/* convert all characters to lower case */
	for (p = lang; *p != '\0'; p++) {
		*p = tolower(*p);
	}

	/* zabbix message file open */
	zbx_snprintf(fname, sizeof(fname), "%s/zabbix_message.%s", CONFIG_JA_ZBX_MESSAGE_FILE, lang);

	fp = fopen(fname, "r");
	if (fp == NULL)
	{
		ja_log("JATRGMSG200001", 0, NULL, inner_job_id, CONFIG_JA_ZBX_MESSAGE_FILE);
		return NULL;
	}

	/* message get */
	cnt = 0;
	hit = 0;
	while (fgets(line, sizeof(line), fp) != NULL)
	{
		cnt = cnt + 1;

		if (line[0] == '#' || line[0] == '\n' || line[0] == '\r' )
		{
			continue;
		}

		if (strlen(line) > 0)
		{
			if (line[strlen(line)-1] == '\n')
			{
				line[strlen(line)-1] = '\0';
			}
		}

		if (strlen(line) > 0)
		{
			if (line[strlen(line)-1] == '\r')
			{
				line[strlen(line)-1] = '\0';
			}
		}

		n = 0;
		name = line;
		msg  = NULL;
		while (line[++n])
		{
			if (line[n] == ',')
			{
				line[n] = '\0';
				msg     = &line[n + 1];
				break;
			}
		}

		lrtrim_spaces(name);

		if (strcmp(name, message_id) == 0)
		{
			hit = 1;
			break;
		}
	}
	fclose(fp);

	/* message hit check */
	if (hit == 0)
	{
		ja_log("JATRGMSG200002", 0, NULL, inner_job_id, message_id, lang);
		return NULL;
	}

	/* message get check */
	if (name == NULL || msg == NULL)
	{
		ja_log("JATRGMSG200003", 0, NULL, inner_job_id, cnt, message_id, CONFIG_JA_ZBX_MESSAGE_FILE);
		return NULL;
	}

	zabbix_log(LOG_LEVEL_DEBUG, "In %s() get message ID:[%s] MESSAGE[%s]", __function_name, message_id, msg);

	return DBdyn_escape_string(msg);

}
