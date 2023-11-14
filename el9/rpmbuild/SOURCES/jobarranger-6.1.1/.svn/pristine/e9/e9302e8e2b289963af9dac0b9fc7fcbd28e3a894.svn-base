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

#include "jacommon.h"
#include "jadb.h"
#include "jalog.h"
#include "javalue.h"

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
int ja_clean_value_before(const zbx_uint64_t inner_job_id)
{
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    char value_name[130];
    const char *__function_name = "ja_clean_value_before";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    zbx_snprintf(value_name, sizeof(value_name), "");
    result =
        DBselect
        ("select seq_no, value_name from ja_run_value_before_table "
         " where inner_job_id = " ZBX_FS_UI64
         " order by value_name, seq_no desc", inner_job_id);

    while (NULL != (row = DBfetch(result))) {
        if (strcmp(row[1], value_name) != 0) {
            zbx_snprintf(value_name, sizeof(value_name), "%s", row[1]);
            continue;
        }
        db_ret =
            DBexecute
            ("delete from ja_run_value_before_table where seq_no = %s",
             row[0]);
        if (db_ret < ZBX_DB_OK) {
            DBfree_result(result);
            return FAIL;
        }
    }
    DBfree_result(result);

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
int ja_clean_value_after(const zbx_uint64_t inner_job_id)
{
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    char value_name[130];
    const char *__function_name = "ja_clean_value_after";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    zbx_snprintf(value_name, sizeof(value_name), "");
    result =
        DBselect("select seq_no, value_name from ja_run_value_after_table "
                 " where inner_job_id = " ZBX_FS_UI64
                 " order by value_name, seq_no desc", inner_job_id);

    while (NULL != (row = DBfetch(result))) {
        if (strcmp(row[1], value_name) != 0) {
            zbx_snprintf(value_name, sizeof(value_name), "%s", row[1]);
            continue;
        }
        db_ret =
            DBexecute
            ("delete from ja_run_value_after_table where seq_no = %s",
             row[0]);
        if (db_ret < ZBX_DB_OK) {
            DBfree_result(result);
            return FAIL;
        }
    }
    DBfree_result(result);

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
int ja_set_value_before(const zbx_uint64_t inner_job_id,
                        const zbx_uint64_t inner_jobnet_id,
                        const char *value_name, const char *before_value)
{
    int ret, db_ret;
    char *value_name_esc, *before_value_esc;
    const char *__function_name = "ja_set_value_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", inner_jobnet_id: "
               ZBX_FS_UI64, __function_name, inner_job_id,
               inner_jobnet_id);

    if (inner_job_id <= 0 || inner_jobnet_id <= 0 || value_name == NULL)
        return FAIL;

    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);
    if (before_value == NULL) {
        before_value_esc = DBdyn_escape_string("");
    } else {
        before_value_esc = DBdyn_escape_string(before_value);
    }

    db_ret =
        DBexecute("insert into ja_run_value_before_table"
                  " (inner_job_id, inner_jobnet_id, value_name, before_value) values ("
                  ZBX_FS_UI64 ", " ZBX_FS_UI64 ", '%s', '%s')",
                  inner_job_id, inner_jobnet_id, value_name_esc,
                  before_value_esc);
    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300001", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id, inner_jobnet_id);
        ret = FAIL;
    }

    zbx_free(value_name_esc);
    zbx_free(before_value_esc);
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
int ja_set_value_after(const zbx_uint64_t inner_job_id,
                       const zbx_uint64_t inner_jobnet_id,
                       const char *value_name, const char *after_value)
{
    int ret, db_ret;
    char *value_name_esc, *after_value_esc;
    const char *__function_name = "ja_set_value_after";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", inner_jobnet_id: "
               ZBX_FS_UI64, __function_name, inner_job_id,
               inner_jobnet_id);

    if (inner_job_id <= 0 || inner_jobnet_id <= 0 || value_name == NULL)
        return FAIL;

    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);

    if (after_value == NULL) {
        after_value_esc = DBdyn_escape_string("");
    } else {

        char dst[JA_STD_OUT_LEN+1];
        int i = 0, j = 0;

        i = strlen(after_value);

        zabbix_log(LOG_LEVEL_DEBUG,"In %s()  data length [%d] after_value [%s] ",__function_name,i,after_value);

        if(i >=  JA_STD_OUT_LEN -1 ){
        	for(i = JA_STD_OUT_LEN - 1 ; i> 0; i--){
        		if(after_value[i] == NULL){
        			zabbix_log(LOG_LEVEL_DEBUG,"1byte NULL 0x[%x] [%d] ",after_value[i],i);
        			continue;
        		}
        		if((after_value[i] & 0xf0) == 0xf0){ // 4バイト文字
					zabbix_log(LOG_LEVEL_DEBUG,"4byte 0xf0 [%d] ",i);
        	    	break;
				}else if((after_value[i] & 0xe0) == 0xe0){ // 3バイト文字
					zabbix_log(LOG_LEVEL_DEBUG,"3byte 0xe0 [%d] ",i);
					break;
				}else if((after_value[i] & 0xc0) == 0xc0){ // 2バイト文字
					zabbix_log(LOG_LEVEL_DEBUG,"2-1byte 0xc0 [%d] ",i);
					break;
				}else if((after_value[i] & 0x80) == 0){ // 1バイト文字の場合
        			zabbix_log(LOG_LEVEL_DEBUG,"1byte 0x[%x] [%d] ",after_value[i],i);
					break;
				}
        	}
        	for(j = 0; j < i; j++){
        		dst[j] = after_value[j];
        	}

  	        dst[j] = '\0';
   	        zabbix_log(LOG_LEVEL_DEBUG,"In %s() strlen [%d] i=[%d] j=[%d]\n",__function_name, strlen(dst),i,j+1);
   	        after_value_esc = DBdyn_escape_string(dst);

        }else{
        	after_value_esc = DBdyn_escape_string(after_value);
        }

    }

    db_ret =
        DBexecute("insert into ja_run_value_after_table"
                  " (inner_job_id, inner_jobnet_id, value_name, after_value) values ("
                  ZBX_FS_UI64 ", " ZBX_FS_UI64 ", '%s', '%s')",
                  inner_job_id, inner_jobnet_id, value_name_esc,
                  after_value_esc);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300001", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_job_id, inner_jobnet_id);
        ret = FAIL;
    }

    zbx_free(value_name_esc);
    zbx_free(after_value_esc);

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
int ja_cpy_value(const zbx_uint64_t inner_job_id, const char *value_src,
                 char *value_dest)
{
    const char *__function_name = "ja_cpy_value";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (value_src == NULL || value_dest == NULL) {
        return FAIL;
    }

    if (strlen(value_src) > 1) {
        if (*value_src == '$' && *(value_src + 1) != '$') {
            return ja_get_value_before(inner_job_id, value_src + 1, value_dest);
        }

        /* escape the $ character ($$ -> $) */
        if (*value_src == '$' && *(value_src + 1) == '$') {
             zbx_snprintf(value_dest, strlen(value_src), "%s", (value_src + 1));
             return SUCCEED;
        }
    }

    zbx_snprintf(value_dest, strlen(value_src) + 1, "%s", value_src);

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
int ja_get_value_before(const zbx_uint64_t inner_job_id,
                        const char *value_name, char *before_value)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *value_name_esc;
    const char *__function_name = "ja_get_value_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (value_name == NULL)
        return FAIL;

    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);
    result =
        DBselect
        ("select before_value from ja_run_value_before_table "
         " where inner_job_id = " ZBX_FS_UI64 " and value_name = '%s'",
         inner_job_id, value_name_esc);
    row = DBfetch(result);
    if (row == NULL) {
        ret = FAIL;
    } else {
        if (row[0] == NULL) {
            ret = FAIL;
        } else {
            zbx_snprintf(before_value, strlen(row[0]) + 1, "%s", row[0]);
        }
    }
    DBfree_result(result);
    zbx_free(value_name_esc);

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
int ja_get_value_after(const zbx_uint64_t inner_job_id,
                       const char *value_name, char *after_value)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *value_name_esc;
    const char *__function_name = "ja_get_value_after";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (value_name == NULL)
        return FAIL;

    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);
    result =
        DBselect
        ("select after_value from ja_run_value_after_table "
         " where inner_job_id = " ZBX_FS_UI64 " and value_name = '%s'",
         inner_job_id, value_name_esc);
    row = DBfetch(result);
    if (row == NULL) {
        ret = FAIL;
    } else {
        if (row[0] == NULL) {
            ret = FAIL;
        } else {
            zbx_snprintf(after_value, strlen(row[0]) + 1, "%s", row[0]);
        }
    }
    DBfree_result(result);
    zbx_free(value_name_esc);

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
int ja_remove_value_before(const zbx_uint64_t inner_job_id,
                           const char *value_name)
{
    int db_ret;
    char *value_name_esc;
    const char *__function_name = "ja_remove_value_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (value_name == NULL)
        return FAIL;

    value_name_esc = DBdyn_escape_string(value_name);
    db_ret =
        DBexecute
        ("delete from ja_run_value_before_table "
         " where inner_job_id = " ZBX_FS_UI64 " and value_name = '%s'",
         inner_job_id, value_name_esc);
    zbx_free(value_name_esc);

    if (ZBX_DB_OK > db_ret)
        return FAIL;
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
int ja_remove_value_after(const zbx_uint64_t inner_job_id,
                          const char *value_name)
{
    int db_ret;
    char *value_name_esc;
    const char *__function_name = "ja_remove_value_after";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id);

    if (value_name == NULL)
        return FAIL;

    value_name_esc = DBdyn_escape_string(value_name);
    db_ret =
        DBexecute
        ("delete from ja_run_value_after_table "
         " where inner_job_id = " ZBX_FS_UI64 " and value_name = '%s'",
         inner_job_id, value_name_esc);
    zbx_free(value_name_esc);

    if (ZBX_DB_OK > db_ret)
        return FAIL;
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
int ja_value_before_after(const zbx_uint64_t inner_job_id)
{
    int db_ret;
    const char *__function_name = "ja_value_before_after";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64,
               __function_name, inner_job_id);

    db_ret =
        DBexecute("insert into ja_run_value_after_table"
                  " (inner_job_id, inner_jobnet_id, value_name, after_value)"
                  " select inner_job_id, inner_jobnet_id, value_name, before_value"
                  " from ja_run_value_before_table where inner_job_id = "
                  ZBX_FS_UI64, inner_job_id);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300003", 0, NULL, inner_job_id, __function_name,
               inner_job_id);
        return FAIL;
    }
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
int ja_value_after_before(const zbx_uint64_t inner_job_id,
                          const zbx_uint64_t next_inner_job_id)
{
    int db_ret;
    const char *__function_name = "ja_value_after_before";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64
               ", next_inner_job_id: " ZBX_FS_UI64, __function_name,
               inner_job_id, next_inner_job_id);

    db_ret =
        DBexecute("insert into ja_run_value_before_table"
                  " (inner_job_id, inner_jobnet_id, value_name, before_value) select "
                  ZBX_FS_UI64 ", inner_jobnet_id, value_name, after_value"
                  " from ja_run_value_after_table where inner_job_id = "
                  ZBX_FS_UI64, next_inner_job_id, inner_job_id);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300003", 0, NULL, inner_job_id, __function_name,
               inner_job_id);
        return FAIL;
    }
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
int ja_clean_value_jobnet_before(const zbx_uint64_t inner_jobnet_id)
{
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    char value_name[130];
    const char *__function_name = "ja_clean_value_jobnet_before";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    zbx_snprintf(value_name, sizeof(value_name), "");
    result =
        DBselect
        ("select seq_no, value_name from ja_value_before_jobnet_table "
         " where inner_jobnet_id = " ZBX_FS_UI64
         " order by value_name, seq_no desc", inner_jobnet_id);

    while (NULL != (row = DBfetch(result))) {
        if (strcmp(row[1], value_name) != 0) {
            zbx_snprintf(value_name, sizeof(value_name), "%s", row[1]);
            continue;
        }
        db_ret =
            DBexecute
            ("delete from ja_value_before_jobnet_table where seq_no = %s",
             row[0]);
        if (db_ret < ZBX_DB_OK) {
            DBfree_result(result);
            return FAIL;
        }
    }
    DBfree_result(result);

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
int ja_clean_value_jobnet_after(const zbx_uint64_t inner_jobnet_id)
{
    int db_ret;
    DB_RESULT result;
    DB_ROW row;
    char value_name[130];
    const char *__function_name = "ja_clean_value_jobnet_after";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64,
               __function_name, inner_jobnet_id);

    zbx_snprintf(value_name, sizeof(value_name), "");
    result =
        DBselect
        ("select seq_no, value_name from ja_value_after_jobnet_table "
         " where inner_jobnet_id = " ZBX_FS_UI64
         " order by value_name, seq_no desc", inner_jobnet_id);

    while (NULL != (row = DBfetch(result))) {
        if (strcmp(row[1], value_name) != 0) {
            zbx_snprintf(value_name, sizeof(value_name), "%s", row[1]);
            continue;
        }
        db_ret =
            DBexecute
            ("delete from ja_value_after_jobnet_table where seq_no = %s",
             row[0]);
        if (db_ret < ZBX_DB_OK) {
            DBfree_result(result);
            return FAIL;
        }
    }
    DBfree_result(result);

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
int ja_set_value_jobnet_before(const zbx_uint64_t inner_jobnet_id,
                               const char *value_name,
                               const char *before_value)
{
    int ret, db_ret;
    char *value_name_esc, *before_value_esc;
    const char *__function_name = "ja_set_value_jobnet_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64, __function_name,
               inner_jobnet_id);

    if (inner_jobnet_id <= 0 || value_name == NULL)
        return FAIL;
    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);
    if (before_value == NULL) {
        before_value_esc = DBdyn_escape_string("");
    } else {
        before_value_esc = DBdyn_escape_string(before_value);
    }

    db_ret =
        DBexecute("insert into ja_value_before_jobnet_table"
                  " (inner_jobnet_id, value_name, before_value) values ("
                  ZBX_FS_UI64 ", '%s', '%s')", inner_jobnet_id,
                  value_name_esc, before_value_esc);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300004", inner_jobnet_id, NULL, 0, __function_name,
               inner_jobnet_id, value_name, before_value);
        ret = FAIL;
    }

    zbx_free(value_name_esc);
    zbx_free(before_value_esc);
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
int ja_get_value_jobnet_before(const zbx_uint64_t inner_jobnet_id,
                               const char *value_name, char *before_value)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    char *value_name_esc;
    const char *__function_name = "ja_get_value_jobnet_before";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64, __function_name,
               inner_jobnet_id);

    if (value_name == NULL)
        return FAIL;

    ret = SUCCEED;
    value_name_esc = DBdyn_escape_string(value_name);
    result =
        DBselect
        ("select before_value from ja_value_before_jobnet_table "
         " where inner_jobnet_id = " ZBX_FS_UI64 " and value_name = '%s'",
         inner_jobnet_id, value_name_esc);
    row = DBfetch(result);
    if (row == NULL) {
        ret = FAIL;
    } else {
        if (row[0] == NULL) {
            ret = FAIL;
        } else {
            zbx_snprintf(before_value, strlen(row[0]) + 1, "%s", row[0]);
        }
    }
    DBfree_result(result);
    zbx_free(value_name_esc);

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
int ja_value_before_jobnet_in(const zbx_uint64_t inner_job_id,
                              const zbx_uint64_t inner_jobnet_id)
{
    int db_ret;
    const char *__function_name = "ja_value_before_jobnet_in";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_job_id: " ZBX_FS_UI64 ", inner_jobnet_id: "
               ZBX_FS_UI64, __function_name, inner_job_id,
               inner_jobnet_id);

    db_ret =
        DBexecute("insert into ja_value_before_jobnet_table"
                  " (inner_jobnet_id, value_name, before_value) select "
                  ZBX_FS_UI64 ", value_name, before_value"
                  " from ja_run_value_before_table where inner_job_id = "
                  ZBX_FS_UI64, inner_jobnet_id, inner_job_id);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300005", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_jobnet_id, inner_job_id);
        return FAIL;
    }
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
int ja_value_before_jobnet_out(const zbx_uint64_t inner_jobnet_id,
                               const zbx_uint64_t inner_job_id)
{
    int db_ret;
    const char *__function_name = "ja_value_before_jobnet_out";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobnet_id: " ZBX_FS_UI64 ", inner_job_id: "
               ZBX_FS_UI64, __function_name, inner_jobnet_id,
               inner_job_id);

    db_ret =
        DBexecute("insert into ja_run_value_before_table"
                  " (inner_job_id, inner_jobnet_id, value_name, before_value) select "
                  ZBX_FS_UI64 ", " ZBX_FS_UI64 ", value_name, before_value"
                  " from ja_value_before_jobnet_table where inner_jobnet_id = "
                  ZBX_FS_UI64, inner_job_id, inner_jobnet_id,
                  inner_jobnet_id);

    if (db_ret < ZBX_DB_OK) {
        ja_log("JAVALUE300005", inner_jobnet_id, NULL, inner_job_id,
               __function_name, inner_jobnet_id, inner_job_id);
        return FAIL;
    }

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
int ja_get_jobnet_summary_start(const zbx_uint64_t inner_jobnet_id,
                                char *start_time)
{
    int ret;
    DB_RESULT result;
    DB_ROW row;
    const char *__function_name = "ja_get_jobnet_summary_start";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() inner_jobinet_id: " ZBX_FS_UI64, __function_name,
               inner_jobnet_id);

    ret = SUCCEED;
    result =
        DBselect
        ("select start_time from ja_run_jobnet_summary_table "
         " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);
    row = DBfetch(result);
    if (row == NULL) {
        ret = FAIL;
    } else {
        if (row[0] == NULL) {
            ret = FAIL;
        } else {
            zbx_snprintf(start_time, 16, "%s", row[0]);
        }
    }
    DBfree_result(result);

    if (ret == FAIL) {
        ja_log("JAVALUE300002", inner_jobnet_id, NULL, 0, __function_name,
               inner_jobnet_id);
    }
    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_variable_free                                                 *
 *                                                                            *
 * Purpose: free the memory of the job controller variable acquisition area   *
 *                                                                            *
 * Parameters: count       (in) - number of variables                         *
 *             ja_variable (in) - job controller variable storage area        *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void ja_variable_free(const int count, ja_variable *val)
{
    int         idx;
    const char  *__function_name = "ja_variable_free";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() count: %d", __function_name, count);

    idx = 0;
    while (idx < count) {
        zbx_free(val[idx].value);
        idx = idx + 1;
    }

    zbx_free(val);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_replace_variable                                              *
 *                                                                            *
 * Purpose: replaced with the value the job controller variables included in  *
 *          the string                                                        *
 *                                                                            *
 * Parameters: inner_job_id (in)  - inner job id                              *
 *             value_src    (in)  - string before replacement                 *
 *             value_dest   (out) - string area after replacement             *
 *             dest_len     (in)  - length of the string area after           *
 *                                  replacement                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments: when using "$" as a character it is specified as "$$"            *
 *           examples of variable specification.                              *
 *           $AAA, ${AAA}, $AAA$BBB, AAA${AAA}BBB                             *
 *                                                                            *
 ******************************************************************************/
int ja_replace_variable(const zbx_uint64_t inner_job_id, char *value_src, char *value_dest, int dest_len)
{
    DB_RESULT   result;
    DB_ROW      row;
    ja_variable *val = NULL;
    char        *ss, *dd, *vv;
    int         count, idx, len, dlen, hit, brace;
    char        variable_name[JA_MAX_DATA_LEN];
    const char  *__function_name = "ja_replace_variable";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    if (value_src == NULL || value_dest == NULL) {
        ja_log("JAVALUE200001", 0, NULL, inner_job_id, __function_name, inner_job_id);
        return FAIL;
    }

    if (dest_len < 1) {
        ja_log("JAVALUE200002", 0, NULL, inner_job_id, __function_name, dest_len, inner_job_id);
        return FAIL;
    }

    dlen = dest_len - 1;

    /* get the number of jobs controller variable */
    result = DBselect("select count(*) from ja_run_value_before_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JAVALUE200003", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }

    count = atoi(row[0]);
    DBfree_result(result);

    if (count <= 0) {
        ja_log("JAVALUE200003", 0, NULL, inner_job_id, __function_name, inner_job_id);
        return FAIL;
    }

    /* get the job controller variable before */
    val = (ja_variable *)zbx_malloc(NULL, (sizeof(ja_variable) * count));

    result = DBselect("select value_name, before_value from ja_run_value_before_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    idx = 0;
    while (NULL != (row = DBfetch(result))) {
        if (count <= idx) {
            ja_log("JAVALUE200004", 0, NULL, inner_job_id, __function_name, inner_job_id);
            DBfree_result(result);
            ja_variable_free(count, val);
            return FAIL;
        }
        val[idx].len1  = strlen(row[0]);
        val[idx].len2  = strlen(row[1]);
        val[idx].value = (char *)zbx_malloc(NULL, (val[idx].len2 + 1));
        zbx_strlcpy(val[idx].name,  row[0], sizeof(val[idx].name));
        zbx_strlcpy(val[idx].value, row[1], val[idx].len2 + 1);
        idx = idx + 1;
    }
    DBfree_result(result);

    /* variable is replaced by a value */
    ss = value_src;
    dd = value_dest;
    while (*ss != '\0') {
        /* other than the variable name ? */
        if (*ss != '$') {
            if (dlen < 1) {
                ja_log("JAVALUE200007", 0, NULL, inner_job_id, __function_name, inner_job_id);
                ja_variable_free(count, val);
                return FAIL;
            }
            *dd  = *ss;
            ss   = ss   + 1;
            dd   = dd   + 1;
            dlen = dlen - 1;
            continue;
        }

        /* escape the '$' character */
        if (*(ss + 1) == '$') {
            if (dlen < 1) {
                ja_log("JAVALUE200007", 0, NULL, inner_job_id, __function_name, inner_job_id);
                ja_variable_free(count, val);
                return FAIL;
            }
            *dd  = *ss;
            ss   = ss   + 2;
            dd   = dd   + 1;
            dlen = dlen - 1;
            continue;
        }

        /* start the replacement of variable */
        brace = 0;
        if (*(ss + 1) == '{') {
            brace = 1;
            ss = ss + 2;
        }
        else {
            ss = ss + 1;
        }

        /* get the variable name */
        for (vv = ss; *ss != '\0'; ss++) {
            if (*ss == '$' || *ss == '\r' || *ss == '\n') {
                break;
            }
            if (*ss == '}') {
                brace = brace - 1;
                break;
            }
        }

        len = ss - vv;

        /* incorrect variable name ? ($ or ${}) */
        if (len == 0) {
            if (*(vv - 1) == '{') {
                zbx_strlcpy(variable_name, "${}", sizeof(variable_name));
            }
            else {
                zbx_strlcpy(variable_name, "$", sizeof(variable_name));
            }
            ja_log("JAVALUE200008", 0, NULL, inner_job_id, __function_name, variable_name, inner_job_id);
            ja_variable_free(count, val);
            return FAIL;
        }

        /* get variable name */
        memcpy(variable_name, vv, len);
        variable_name[len] = '\0';

        /* incorrect variable name ? (${xxx or $xxx}) */
        if (brace != 0) {
            ja_log("JAVALUE200008", 0, NULL, inner_job_id, __function_name, variable_name, inner_job_id);
            ja_variable_free(count, val);
            return FAIL;
        }

        /* the variable name is long ? */
        if (len > (JA_VALUE_NAME_LEN - 1)) {
            ja_log("JAVALUE200009", 0, NULL, inner_job_id, __function_name, (JA_VALUE_NAME_LEN - 1), len, variable_name, inner_job_id);
            ja_variable_free(count, val);
            return FAIL;
        }

        /* find the value of a variable */
        hit = 0;
        for (idx = 0; idx < count; idx++) {
            if (val[idx].len1 != len) {
                continue;
            }

            if (strcmp(val[idx].name, variable_name) == 0) {
                if (val[idx].len2 > dlen) {
                    ja_log("JAVALUE200005", 0, NULL, inner_job_id, __function_name, variable_name, inner_job_id);
                    ja_variable_free(count, val);
                    return FAIL;
                }
                memcpy(dd, val[idx].value, val[idx].len2);
                dd   = dd   + val[idx].len2;
                dlen = dlen - val[idx].len2;
                hit  = 1;
                break;
            }
        }

        /* variable no hit ? */
        if (hit == 0) {
            ja_log("JAVALUE200006", 0, NULL, inner_job_id, __function_name, variable_name, inner_job_id);
            ja_variable_free(count, val);
            return FAIL;
        }

        if (*ss == '}') {
            ss = ss + 1;
        }
    }

    *dd = '\0';

    ja_variable_free(count, val);

    return SUCCEED;
}
