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

#include "jacommon.h"
#include "threads.h"

static char journal_filename[JA_MAX_STRING_LEN];
/******************************************************************************
 *                                                                            *
 * Function:                                                                  *
 *                                                                            *
 * Purpose:                                                                   *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_journal_create(const char *filename)
{
    FILE *fp;
    const char *__function_name = "ja_journal_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s", __function_name,
               filename);
    fp = fopen(filename, "a");
    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() Can not open the journal file %s. [%s]",__function_name,
                   filename, zbx_strerror(errno));
        return FAIL;
    }

    zbx_snprintf(journal_filename, sizeof(journal_filename), "%s", filename);
    fclose(fp);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function:                                                                  *
 *                                                                            *
 * Purpose:                                                                   *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_journal_save(const char *str)
{
    int ret;
    FILE *fp;
	int cnt;
    const char *__function_name = "ja_journal_save";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s", __function_name,
    		journal_filename);

    ret = SUCCEED;
	cnt = 0;
	while(cnt < 3){

		fp = fopen(journal_filename, "a");
		if (fp != NULL){
			break;
		}
		cnt ++;
		zbx_sleep(1);
	}

    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() Can not open the journal file %s. [%s]",__function_name,
                   journal_filename, zbx_strerror(errno));
        return FAIL;
    }

    if (fprintf
        (fp, ZBX_FS_UI64 " %d %s\n", (zbx_uint64_t) time(NULL),
         (int) strlen(str), str)
        < 0) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() Can not write journal file %s. [%s]",__function_name,
                   journal_filename, zbx_strerror(errno));
        ret = FAIL;
    }

    if (fp != NULL)
        fclose(fp);
    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function:                                                                  *
 *                                                                            *
 * Purpose:                                                                   *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_journal_vacate()
{
    FILE *fp;
    const char *__function_name = "ja_journal_vacate";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s", __function_name,
               journal_filename);

    fp = fopen(journal_filename, "w");
    if (fp == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() Can not open the journal file %s. [%s]",__function_name,
                   journal_filename, zbx_strerror(errno));
        return FAIL;
    }
    fclose(fp);
    return SUCCEED;
}


