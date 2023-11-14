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
#include "jathreads.h"
#include "jajobfile.h"
#include "jaflow.h"
#include "jastr.h"
#include "jaenv.h"
#include "jalog.h"
#include "jajobid.h"
#include "javalue.h"
#include "jastatus.h"
#include "jaruniconextjob.h"

#define ZBXSND_SENDER           "ZBXSND_SENDER"

#define DATE_LEN                16
#define FULL_COMMAND_LEN        (JA_MAX_DATA_LEN * 2) + JA_MAX_STRING_LEN

extern char     *CONFIG_TMPDIR;

/******************************************************************************
 *                                                                            *
 * Function: ja_get_zbxsnd_sender                                             *
 *                                                                            *
 * Purpose: gets the  zabbix_sender command path                              *
 *                                                                            *
 * Parameters: cmd (out) - zabbix_sender command                              *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_get_zbxsnd_sender(char *cmd)
{
    DB_RESULT    result;
    DB_ROW       row;
    const char   *__function_name = "ja_get_zbxsnd_sender";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    result = DBselect("select value from ja_parameter_table where parameter_name = '%s'", ZBXSND_SENDER);

    if (NULL == (row = DBfetch(result))) {
        zbx_strlcpy(cmd, "zabbix_sender", JA_MAX_STRING_LEN);
        DBfree_result(result);
        return SUCCEED;
    }

    zbx_strlcpy(cmd, row[0], JA_MAX_STRING_LEN);
    DBfree_result(result);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_get_sleep_time                                                *
 *                                                                            *
 * Purpose: edit the waiting time                                             *
 *                                                                            *
 * Parameters: des_sec (in) - indicated wait second (0-999999)                *
 *             sleep_time (out) - sleep time (YYYYMMDDHHMMSS)                 *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_get_sleep_time(char *des_sec, char *sleep_time)
{
    struct tm    *tm;
    time_t       now;
    const char   *__function_name = "ja_get_sleep_time";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s)", __function_name, des_sec);

    time(&now);
    now = now + atoi(des_sec);
    tm = localtime(&now);

    zbx_snprintf(sleep_time, DATE_LEN, "%04d%02d%02d%02d%02d%02d",
                (tm->tm_year + 1900),
                (tm->tm_mon  + 1),
                 tm->tm_mday,
                 tm->tm_hour,
                 tm->tm_min,
                 tm->tm_sec);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_check_wait_time                                               *
 *                                                                            *
 * Purpose: check that it is a wait time format                               *
 *                                                                            *
 * Parameters: data (in) - string to be checked ([H]HMM[SS] : 000-995959)     *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - detect incorrect data                                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_check_wait_time(char *data)
{
    int          size, hour, min, sec;
    char         hh[3], mm[3], ss[3] , hhmmss[7];
    const char   *__function_name = "ja_check_wait_time";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s)", __function_name, data);

    size = strlen(data);

    if (size < 3 || size > 6) {
        return FAIL;
    }

    if (size == 3 || size == 5) {
        zbx_snprintf(hhmmss, sizeof(hhmmss), "0%s", data);
    }
    else {
        zbx_strlcpy(hhmmss, data, sizeof(hhmmss));
    }

    zbx_strlcpy(hh,  hhmmss,      3);
    zbx_strlcpy(mm, (hhmmss + 2), 3);
    hour = atoi(hh);
    min  = atoi(mm);
    sec  = 0;

    if (size > 4) {
        zbx_strlcpy(ss, (hhmmss + 4), 3);
        sec = atoi(ss);
    }

    if (hour < 0 || hour > 99) {
        return FAIL;
    }

    if (min < 0 || min > 59) {
        return FAIL;
    }

    if (sec < 0 || sec > 59) {
        return FAIL;
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_get_wait_time                                                 *
 *                                                                            *
 * Purpose: edit the waiting time                                             *
 *                                                                            *
 * Parameters: des_time (in) - indicated wait time ([H]HMM[SS] : 000-995959)  *
 *             start_time (in) - jobnet start time (YYYYMMDDHHMMSS)           *
 *             wait_time (out) - waiting time (YYYYMMDDHHMMSS)                *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - detect incorrect data                                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_get_wait_time(char *des_time, char *start_time, char *wait_time)
{
    struct tm    stm;
    int          wtime, year, mon, day, pday, size;
    char         yy[5], mm[3], dd[3], hhmmss[7];
    char         w_time[DATE_LEN], w_ss[3];
    const char   *__function_name = "ja_get_wait_time";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s %s)", __function_name, des_time, start_time);

    size = strlen(des_time);

    /* HMM -> HHMM or HMMSS -> HHMMSS */
    if (size == 3 || size == 5) {
        zbx_snprintf(hhmmss, sizeof(hhmmss), "0%s", des_time);
    }
    else {
        zbx_strlcpy(hhmmss, des_time, sizeof(hhmmss));
    }

    /* HHMMSS -> SS */
    if (strlen(hhmmss) == 4) {
        zbx_strlcpy(w_ss, "00", sizeof(w_ss));
    }
    else {
        zbx_strlcpy(w_ss, (hhmmss + 4), 3);
    }

    /* HHMMSS -> HHMM */
    zbx_strlcpy(w_time, hhmmss, 5);

    pday  = 0;
    wtime = atoi(w_time);
    if (wtime >= 2400) {
        pday  = wtime / 2400;
        wtime = wtime - (2400 * pday);
    }

    zbx_strlcpy(yy,  start_time,      5);
    zbx_strlcpy(mm, (start_time + 4), 3);
    zbx_strlcpy(dd, (start_time + 6), 3);

    year = atoi(yy);
    mon  = atoi(mm);
    day  = atoi(dd);

    stm.tm_year  = year - 1900;
    stm.tm_mon   = mon - 1;
    stm.tm_mday  = day + pday;
    stm.tm_hour  = 0;
    stm.tm_min   = 0;
    stm.tm_sec   = 0;
    stm.tm_isdst = -1;

    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= des_time[%s] start_time[%s] wtime[%d] pday[%d] yy[%s] mm[%s] dd[%s] year[%d] mon[%d] day[%d] stm.tm_year[%d] stm.tm_mon[%d] stm.tm_mday[%d]",
                                 des_time, start_time, wtime, pday, yy, mm, dd, year, mon, day, stm.tm_year, stm.tm_mon, stm.tm_mday);

    /* time conversion */
    if (-1 == mktime(&stm)) {
        return FAIL;
    }

    zbx_snprintf(wait_time, DATE_LEN, "%04d%02d%02d%04d%s",
                (stm.tm_year + 1900),
                (stm.tm_mon  + 1),
                 stm.tm_mday,
                 wtime,
                 w_ss);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_check_week                                                    *
 *                                                                            *
 * Purpose: check the day of the week format                                  *
 *                                                                            *
 * Parameters: value (in) - string to be checked                              *
 *                         (Sun, Mon, Tue, Wed, Thu, Fri, Sat)                *
 *             jw (out) - day of the week for a check                         *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                1 - detect incorrect data                                   *
 *                2 - detect duplicates of the day of the week                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_check_week(char *value, ja_week_t *jw)
{
    int          hit;
    char         *tp, *p, wday[4];
    const char   *__function_name = "ja_check_week";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s)", __function_name, value);

    jw->sun = 0;
    jw->mon = 0;
    jw->tue = 0;
    jw->wed = 0;
    jw->thu = 0;
    jw->fri = 0;
    jw->sat = 0;

    if (strlen(value) == 0) {
        /* get the day of the week now */
        jw->sun = 1;
        jw->mon = 2;
        jw->tue = 3;
        jw->wed = 4;
        jw->thu = 5;
        jw->fri = 6;
        jw->sat = 7;
        return SUCCEED;
    }

    tp = strtok(value, " ");
    while (tp != NULL) {
        if (strlen(tp) != 3) {
            return 1;
        }

        zbx_strlcpy(wday, tp, sizeof(wday));
        for (p = wday; *p != '\0'; p++) {
            *p = toupper(*p);
        }

        hit = 0;
        if (strcmp(wday, "SUN") == 0) {
            if (jw->sun != 0) {
                return 2;
            }
            jw->sun = 1;
            hit     = 1;
        }
        if (strcmp(wday, "MON") == 0) {
            if (jw->mon != 0) {
                return 2;
            }
            jw->mon = 2;
            hit     = 1;
        }
        if (strcmp(wday, "TUE") == 0) {
            if (jw->tue != 0) {
                return 2;
            }
            jw->tue = 3;
            hit     = 1;
        }
        if (strcmp(wday, "WED") == 0) {
            if (jw->wed != 0) {
                return 2;
            }
            jw->wed = 4;
            hit     = 1;
        }
        if (strcmp(wday, "THU") == 0) {
            if (jw->thu != 0) {
                return 2;
            }
            jw->thu = 5;
            hit     = 1;
        }
        if (strcmp(wday, "FRI") == 0) {
            if (jw->fri != 0) {
                return 2;
            }
            jw->fri = 6;
            hit     = 1;
        }
        if (strcmp(wday, "SAT") == 0) {
            if (jw->sat != 0) {
                return 2;
            }
            jw->sat = 7;
            hit     = 1;
        }

        if (hit == 0) {
            /* incorrect day of the week */
            return 1;
        }
        tp = strtok(NULL, " ");
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_extjob_sleep                                          *
 *                                                                            *
 * Purpose: run the sleep of the extended job icon                            *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             value (in) - sleep time                                        *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_extjob_sleep(const zbx_uint64_t inner_job_id, char *value)
{
    int          rc;
    char         sleep_time[DATE_LEN];
    const char   *__function_name = "jarun_icon_extjob_sleep";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " value: %s", __function_name, inner_job_id, value);

    /* number of digits check */
    if (strlen(value) < 1 || strlen(value) > 6) {
        ja_log("JARUNICONEXTJOB200009", 0, NULL, inner_job_id, __function_name, value, inner_job_id, ja_get_jobid(inner_job_id));
        return ja_set_runerr(inner_job_id, 2);
    }

    /* numeric check */
    if (FAIL == ja_check_number(value)) {
        ja_log("JARUNICONEXTJOB200009", 0, NULL, inner_job_id, __function_name, value, inner_job_id, ja_get_jobid(inner_job_id));
        return ja_set_runerr(inner_job_id, 2);
    }

    /* get sleep time */
    ja_get_sleep_time(value, sleep_time);

    /* record locking */
    DBfree_result(DBselect("select command_id from ja_run_icon_extjob_table"
                           " where inner_job_id = " ZBX_FS_UI64 " for update", inner_job_id));

    /* registration waiting time */
    rc = DBexecute("update ja_run_icon_extjob_table set wait_time = '%s'"
                   " where inner_job_id = " ZBX_FS_UI64,
                   sleep_time, inner_job_id);

    if (rc < ZBX_DB_OK) {
        ja_log("JARUNICONEXTJOB200010", 0, NULL, inner_job_id, __function_name, sleep_time, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_extjob_time                                           *
 *                                                                            *
 * Purpose: run the time waits of the extended job icon                       *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             value (in) - sleep time                                        *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_extjob_time(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, char *value)
{
    DB_RESULT    result;
    DB_ROW       row;
    zbx_uint64_t inner_jobnet_main_id;
    int          rc;
    char         start_time[DATE_LEN], wait_time[DATE_LEN];
    const char   *__function_name = "jarun_icon_extjob_time";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64 " inner_job_id: " ZBX_FS_UI64 " value: %s",
               __function_name, inner_jobnet_id, inner_job_id, value);

    /* numeric check */
    if (FAIL == ja_check_number(value)) {
        ja_log("JARUNICONEXTJOB200011", 0, NULL, inner_job_id, __function_name, value, inner_job_id, ja_get_jobid(inner_job_id));
        return ja_set_runerr(inner_job_id, 2);
    }

    /* wait time format check */
    if (FAIL == ja_check_wait_time(value)) {
        ja_log("JARUNICONEXTJOB200011", 0, NULL, inner_job_id, __function_name, value, inner_job_id, ja_get_jobid(inner_job_id));
        return ja_set_runerr(inner_job_id, 2);
    }

    /* get inner jobnet main id */
    result = DBselect("select inner_jobnet_main_id from ja_run_jobnet_table"
                      " where inner_jobnet_id = " ZBX_FS_UI64, inner_jobnet_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONEXTJOB200007", 0, NULL, inner_job_id, __function_name, inner_jobnet_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_main_id, row[0]);
    DBfree_result(result);

    /* get jobnet start time */
    if (FAIL == ja_get_jobnet_summary_start(inner_jobnet_main_id, start_time)) {
        ja_log("JARUNICONEXTJOB200005", inner_jobnet_main_id, NULL, inner_job_id, __function_name, inner_jobnet_main_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    /* get wait time */
    if (FAIL == ja_get_wait_time(value, start_time, wait_time)) {
        ja_log("JARUNICONEXTJOB200016", inner_jobnet_main_id, NULL, inner_job_id, __function_name, value, start_time, inner_job_id, ja_get_jobid(inner_job_id));
        return ja_set_runerr(inner_job_id, 2);
    }

    /* record locking */
    DBfree_result(DBselect("select command_id from ja_run_icon_extjob_table"
                           " where inner_job_id = " ZBX_FS_UI64 " for update", inner_job_id));

    /* registration waiting time */
    rc = DBexecute("update ja_run_icon_extjob_table set wait_time = '%s'"
                   " where inner_job_id = " ZBX_FS_UI64,
                   wait_time, inner_job_id);

    if (rc < ZBX_DB_OK) {
        ja_log("JARUNICONEXTJOB200012", 0, NULL, inner_job_id, __function_name, wait_time, inner_job_id);
        return ja_set_runerr(inner_job_id, 2);
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_extjob_week                                           *
 *                                                                            *
 * Purpose: run the week check of the extended job icon                       *
 *                                                                            *
 * Parameters: inner_jobnet_id (in) - inner jobnet id                         *
 *             inner_job_id (in) - inner job id                               *
 *             value (in) - day of the week                                   *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_extjob_week(const zbx_uint64_t inner_jobnet_id, const zbx_uint64_t inner_job_id, char *value)
{
    struct tm    *tm;
    time_t       now;
    ja_week_t    jw;
    int          rc, wd, wday;
    char         s_wday[3], w_value[JA_MAX_DATA_LEN * 2];
    const char   *__function_name = "jarun_icon_extjob_week";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_jobnet_id: " ZBX_FS_UI64 " inner_job_id: " ZBX_FS_UI64 " value: %s",
               __function_name, inner_jobnet_id, inner_job_id, value);

    zbx_strlcpy(w_value, value, sizeof(w_value));

    /* check the day of the week format */
    rc = ja_check_week(value, &jw);
    if (rc != SUCCEED) {
        if (rc == 2) {
            ja_log("JARUNICONEXTJOB200017", 0, NULL, inner_job_id, __function_name, w_value, inner_job_id, ja_get_jobid(inner_job_id));
        }
        else {
            ja_log("JARUNICONEXTJOB200013", 0, NULL, inner_job_id, __function_name, w_value, inner_job_id, ja_get_jobid(inner_job_id));
        }
        return ja_set_runerr(inner_job_id, 2);
    }

    time(&now);
    tm = localtime(&now);

    wday = tm->tm_wday + 1;

    wd = 0;   /* mismatch */

    if (wday == jw.sun || wday == jw.mon || wday == jw.tue ||
        wday == jw.wed || wday == jw.thu || wday == jw.fri ||
        wday == jw.sat) {
        wd = wday;
    }

    zbx_snprintf(s_wday, sizeof(s_wday), "%d", wd);

    /* check result write */
    if (FAIL == ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", s_wday)) {
        return FAIL;
    }

    return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_extjob_zbxsender                                      *
 *                                                                            *
 * Purpose: run the zabbix_sender execution of the extended job icon          *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             value (in) - zabbix_sender parameters                          *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_extjob_zbxsender(const zbx_uint64_t inner_job_id, char *value)
{
    int          rc, state;
    char         cmd[JA_MAX_STRING_LEN], full_command[FULL_COMMAND_LEN];
    const char   *__function_name = "jarun_icon_extjob_zbxsender";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " value: %s", __function_name, inner_job_id, value);

    /* get zabbix_sender command path */
    ja_get_zbxsnd_sender(cmd);

    /* zabbix_sender execution */
    zbx_snprintf(full_command, sizeof(full_command), "%s -vv %s > %s/%s." ZBX_FS_UI64 " 2>&1 &", cmd, value, CONFIG_TMPDIR, JA_EXTJOB_RESULT_FILE, inner_job_id);
    rc = ja_system_call(full_command);

    zabbix_log(LOG_LEVEL_DEBUG, "zabbix_sender execution [%s] (%d)", full_command, rc);

    if (rc != EXIT_SUCCESS) {
        if (WIFEXITED(rc)) {
            state = WEXITSTATUS(rc);
        }
        else {
            state = rc;
        }
        ja_log("JARUNICONEXTJOB200015", 0, NULL, inner_job_id, __function_name, inner_job_id, ja_get_jobid(inner_job_id), state, full_command);
        return ja_set_runerr(inner_job_id, 2);
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: jarun_icon_extjob                                                *
 *                                                                            *
 * Purpose: run the expanded job icon                                         *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *             test_flag (in) - test mode flag                                *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int jarun_icon_extjob(const zbx_uint64_t inner_job_id, const int test_flag)
{
    DB_RESULT    result;
    DB_ROW       row;
    zbx_uint64_t inner_jobnet_id;
    char         command_id[JA_COMMAND_ID_LEN];
    char         val[JA_MAX_DATA_LEN], value[JA_MAX_DATA_LEN * 2];
    const char   *__function_name = "jarun_icon_extjob";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64 " test_flag: %d",
               __function_name, inner_job_id, test_flag);

    /* get expanded job icon information */
    result = DBselect("select inner_jobnet_id, command_id, value from ja_run_icon_extjob_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JARUNICONEXTJOB200004", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return ja_set_runerr(inner_job_id, 2);
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    zbx_strlcpy(command_id, row[1], sizeof(command_id));
    ja_format_extjob(row[2], val);
    DBfree_result(result);

    /* test mode ? */
    if (test_flag == JA_JOB_TEST_FLAG_ON) {
        if (strcmp(command_id, JA_CMD_SLEEP) == 0 || strcmp(command_id, JA_CMD_TIME) == 0) {
            return ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1);
        }
    }

    /* replace the job controller variable */
    if (FAIL == ja_replace_variable(inner_job_id, val, value, sizeof(value))) {
        return ja_set_runerr(inner_job_id, 2);
    }

    /* delete all left and right spaces for value */
    lrtrim_spaces(value);

    /* sleep */
    if (strcmp(command_id, JA_CMD_SLEEP) == 0) {
        return jarun_icon_extjob_sleep(inner_job_id, value);
    }

    /* time waits */
    if (strcmp(command_id, JA_CMD_TIME) == 0) {
        return jarun_icon_extjob_time(inner_jobnet_id, inner_job_id, value);
    }

    /* day of the week check */
    if (strcmp(command_id, JA_CMD_WEEK) == 0) {
        return jarun_icon_extjob_week(inner_jobnet_id, inner_job_id, value);
    }

    /* zabbix sender run */
    if (strcmp(command_id, JA_CMD_ZBXSENDER) == 0) {
        return jarun_icon_extjob_zbxsender(inner_job_id, value);
    }

    /* detect invalid command id */
    ja_log("JARUNICONEXTJOB200008", 0, NULL, inner_job_id, __function_name, command_id, inner_job_id, ja_get_jobid(inner_job_id));
    return ja_set_runerr(inner_job_id, 2);

}
