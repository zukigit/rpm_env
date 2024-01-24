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
#include "db.h"
#include "log.h"

#include "jacommon.h"

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
void ja_db_data_head(DB_RESULT result)
{
#if defined(HAVE_MYSQL)
    mysql_data_seek(result, 0);
#elif defined(HAVE_POSTGRESQL)
    result->cursor = 0;
#endif
    return;
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
int ja_db_check_table(char *table)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *table_esc;

    ret = FAIL;
    table_esc = DBdyn_escape_string(table);
#if defined(HAVE_MYSQL)
    result = DBselect("show tables like '%s'", table_esc);
#elif defined(HAVE_POSTGRESQL)
    result =
        DBselect
        ("select relname from pg_stat_user_tables where relname = '%s'",
         table_esc);
#endif
    row = DBfetch(result);
    if (row == NULL)
        ret = FAIL;
    else
        ret = SUCCEED;

    DBfree_result(result);
    zbx_free(table_esc);
    return ret;
}
