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
#include "daemon.h"
#include "zbxserver.h"
#include "zbxself.h"
#include "../events.h"

#include "jacommon.h"
#include "jastr.h"
#include "jalog.h"
#include "jajoblog.h"
#include "jaself.h"
#include "jamsgsnd.h"

#define SNDMSG_KEEP_SPAN            "SNDMSG_KEEP_SPAN"

#define ZBXSND_ZABBIX_IP            "ZBXSND_ZABBIX_IP"
#define ZBXSND_ZABBIX_PORT          "ZBXSND_ZABBIX_PORT"
#define ZBXSND_ZABBIX_HOST          "ZBXSND_ZABBIX_HOST"
#define ZBXSND_ITEM_KEY             "ZBXSND_ITEM_KEY"
#define ZBXSND_SENDER               "ZBXSND_SENDER"
#define ZBXSND_RETRY                "ZBXSND_RETRY"
#define ZBXSND_RETRY_COUNT          "ZBXSND_RETRY_COUNT"
#define ZBXSND_RETRY_INTERVAL       "ZBXSND_RETRY_INTERVAL"

#define DEFAULT_KEEP_SPAN           1440

#define DEFAULT_RETRY               0
#define DEFAULT_RETRY_COUNT         3
#define DEFAULT_RETRY_INTERVAL      5

#define DATE_LEN                    14 + 1

/* zbxsnd parameter information */
typedef struct {
    char        zabbix_ip[JA_MAX_DATA_LEN + 1];
    char        zabbix_port[JA_MAX_DATA_LEN + 1];
    char        zabbix_host[JA_MAX_DATA_LEN + 1];
    char        item_key[JA_MAX_DATA_LEN + 1];
    char        sender[JA_MAX_DATA_LEN + 1];
    int         retry;
    int         retry_count;
    int         retry_interval;
    int         keep_span;
} ja_zbxsnd_info_t;

extern unsigned char  process_type;
extern int            process_num;

/******************************************************************************
 *                                                                            *
 * Function: get_purge_date                                                   *
 *                                                                            *
 * Purpose: get the deletion time of sending messages                         *
 *                                                                            *
 * Parameters: span (in) - retention period of message information (Minute)   *
 *             purge_date (out) - purge start time (YYYYMMDDHHMMSS)           *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void get_purge_date(int span, char *purge_date)
{
    struct tm    *tm;
    time_t       now;
    const char   *__function_name = "get_purge_date";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%d)", __function_name, span);

    time(&now);
    tm = localtime(&now);

    tm->tm_min = tm->tm_min - span;
    mktime(tm);

    strftime(purge_date, DATE_LEN, "%Y%m%d%H%M59", tm);

    return;
}

/******************************************************************************
 *                                                                            *
 * Function: get_zbxsnd_parameters                                            *
 *                                                                            *
 * Purpose: gets the zabbix sending parameters                                *
 *                                                                            *
 * Parameters: zbxsnd (out) - zbxsnd parameter information                    *
 *                                                                            *
 * Return value: retention period                                             *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void get_zbxsnd_parameters(ja_zbxsnd_info_t *zbxsnd)
{
    DB_RESULT    result;
    DB_ROW       row;
    int          cnt;
    char         parameter_name[128];
    const char   *__function_name = "get_zbxsnd_parameters";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    /* set the default value */
    zbx_strlcpy(zbxsnd->zabbix_ip,   "127.0.0.1",     sizeof(zbxsnd->zabbix_ip));
    zbx_strlcpy(zbxsnd->zabbix_port, "10051",         sizeof(zbxsnd->zabbix_port));
    zbx_strlcpy(zbxsnd->zabbix_host, "Zabbix server", sizeof(zbxsnd->zabbix_host));
    zbx_strlcpy(zbxsnd->item_key,    "jasender",      sizeof(zbxsnd->item_key));
    zbx_strlcpy(zbxsnd->sender,      "zabbix_sender", sizeof(zbxsnd->sender));

    zbxsnd->retry          = DEFAULT_RETRY;
    zbxsnd->retry_count    = DEFAULT_RETRY_COUNT;
    zbxsnd->retry_interval = DEFAULT_RETRY_INTERVAL;
    zbxsnd->keep_span      = DEFAULT_KEEP_SPAN;

    cnt = 1;
    while (cnt <= 9) {
        /* parameter name set */
        switch (cnt) {
            case 1:
                 zbx_strlcpy(parameter_name, ZBXSND_ZABBIX_IP, sizeof(parameter_name));
                 break;

            case 2:
                 zbx_strlcpy(parameter_name, ZBXSND_ZABBIX_PORT, sizeof(parameter_name));
                 break;

            case 3:
                 zbx_strlcpy(parameter_name, ZBXSND_ZABBIX_HOST, sizeof(parameter_name));
                 break;

            case 4:
                 zbx_strlcpy(parameter_name, ZBXSND_ITEM_KEY, sizeof(parameter_name));
                 break;

            case 5:
                 zbx_strlcpy(parameter_name, ZBXSND_SENDER, sizeof(parameter_name));
                 break;

            case 6:
                 zbx_strlcpy(parameter_name, ZBXSND_RETRY, sizeof(parameter_name));
                 break;

            case 7:
                 zbx_strlcpy(parameter_name, ZBXSND_RETRY_COUNT, sizeof(parameter_name));
                 break;

            case 8:
                 zbx_strlcpy(parameter_name, ZBXSND_RETRY_INTERVAL, sizeof(parameter_name));
                 break;

            case 9:
                 zbx_strlcpy(parameter_name, SNDMSG_KEEP_SPAN, sizeof(parameter_name));
                 break;
        }

        /* get the zbxsnd parameters */
        result = DBselect("select value from ja_parameter_table where parameter_name = '%s'", parameter_name);

        if (NULL != (row = DBfetch(result))) {
            /* parameter value set */
            switch (cnt) {
                case 1:
                     zbx_strlcpy(zbxsnd->zabbix_ip, row[0], sizeof(zbxsnd->zabbix_ip));
                     break;

                case 2:
                     if (SUCCEED == ja_check_number(row[0])) {
                         zbx_strlcpy(zbxsnd->zabbix_port, row[0], sizeof(zbxsnd->zabbix_port));
                     }
                     break;

                case 3:
                     zbx_strlcpy(zbxsnd->zabbix_host, row[0], sizeof(zbxsnd->zabbix_host));
                     break;

                case 4:
                     zbx_strlcpy(zbxsnd->item_key, row[0], sizeof(zbxsnd->item_key));
                     break;

                case 5:
                     zbx_strlcpy(zbxsnd->sender, row[0], sizeof(zbxsnd->sender));
                     break;

                case 6:
                     if (strlen(row[0]) == 1) {
                         zbxsnd->retry = atoi(row[0]);
                     }
                     if (zbxsnd->retry < 0 || zbxsnd->retry > 1) {
                         zbxsnd->retry = DEFAULT_RETRY;
                     }
                     break;

                case 7:
                     if (strlen(row[0]) <= 10) {
                         zbxsnd->retry_count = atoi(row[0]);
                     }
                     if (zbxsnd->retry_count < 0) {
                         zbxsnd->retry_count = DEFAULT_RETRY_COUNT;
                     }
                     break;

                case 8:
                     if (strlen(row[0]) <= 10) {
                         zbxsnd->retry_interval = atoi(row[0]);
                     }
                     if (zbxsnd->retry_interval < 0) {
                         zbxsnd->retry_interval = DEFAULT_RETRY_INTERVAL;
                     }
                     break;

                case 9:
                     if (strlen(row[0]) <= 10) {
                         zbxsnd->keep_span = atoi(row[0]);
                     }
                     if (zbxsnd->keep_span < 0) {
                         zbxsnd->keep_span = DEFAULT_KEEP_SPAN;
                     }
                     break;
            }
        }
        DBfree_result(result);
        cnt = cnt + 1;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_ZABBIX_IP:      [%s]", zbxsnd->zabbix_ip);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_ZABBIX_PORT:    [%s]", zbxsnd->zabbix_port);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_ZABBIX_HOST:    [%s]", zbxsnd->zabbix_host);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_ITEM_KEY:       [%s]", zbxsnd->item_key);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_SENDER:         [%s]", zbxsnd->sender);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_RETRY:          [%d]", zbxsnd->retry);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_RETRY_COUNT:    [%d]", zbxsnd->retry_count);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= ZBXSND_RETRY_INTERVAL: [%d]", zbxsnd->retry_interval);
    zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= SNDMSG_KEEP_SPAN:      [%d]", zbxsnd->keep_span);

    return;
}

/******************************************************************************
 *                                                                            *
 * Function: message_purge                                                    *
 *                                                                            *
 * Purpose: remove send message information in the sent                       *
 *                                                                            *
 * Parameters: zbxsnd (in) - zbxsnd parameter information                     *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int message_purge(ja_zbxsnd_info_t *zbxsnd)
{
    int          rc;
    char         purge_date[DATE_LEN];
    const char   *__function_name = "message_purge";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    get_purge_date(zbxsnd->keep_span, purge_date);

    /* delete sent message information */
    DBbegin();

    rc = DBexecute("delete from ja_send_message_table"
                   " where send_status = %d and send_date <= %s",
                   JA_SNT_SEND_STATUS_END, purge_date);

    if (rc < ZBX_DB_OK) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() failed to delete the sent message information. purge_date: %s",
                   __function_name, purge_date);
        DBrollback();
        return FAIL;
    }

    DBcommit();

    /* deleting a transmission failure message information */
    DBbegin();

    rc = DBexecute("delete from ja_send_message_table"
                   " where send_status = %d and send_error_date <= %s",
                   JA_SNT_SEND_STATUS_ERROR, purge_date);

    if (rc < ZBX_DB_OK) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() failed to delete the transmission failure message information. purge_date: %s",
                   __function_name, purge_date);
        DBrollback();
        return FAIL;
    }

    DBcommit();

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: message_send                                                     *
 *                                                                            *
 * Purpose: send a message to Zabbix                                          *
 *                                                                            *
 * Parameters: zbxsnd (in) - zbxsnd parameter information                     *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - an error occurred                                    *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int    message_send(ja_zbxsnd_info_t *zbxsnd)
{
    DB_RESULT    result;
    DB_ROW       row;
    struct tm    *tm, stm;
    time_t       now;
    int          send_status, retry_count, message_type, rc, state;
    char         *user_name, *jobnet_id, *jobnet_name, *job_id, *job_id_full, *job_name, *log_message_id, *log_message, *inner_jobnet_main_id;
    char         yy[5], mm[3], dd[3], hh[3], mi[3], ss[3];
    char         now_date[DATE_LEN], now_time[80], retry_date[DATE_LEN], msg_type[10], cmd[JA_MAX_DATA_LEN * 2], host_name[JA_MAX_DATA_LEN];
    const char   *__function_name = "message_send";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    time(&now);
    tm = localtime(&now);
    strftime(now_date, sizeof(now_date), "%Y%m%d%H%M%S", tm);

    result = DBselect("select send_no, message_date, send_status, retry_count, retry_date,"
                      " message_type, user_name, host_name, jobnet_id, jobnet_name, job_id,"
                      " job_id_full, job_name, log_message_id, log_message, inner_jobnet_main_id"
                      " from ja_send_message_table"
                      " where (send_status = %d or send_status = %d) and retry_date <= %s"
                      " order by retry_date, message_date",
                      JA_SNT_SEND_STATUS_BEGIN, JA_SNT_SEND_STATUS_RETRY, now_date);

    while (NULL != (row = DBfetch(result))) {

        zabbix_log(LOG_LEVEL_DEBUG, "=DEBUG= get ja_send_message_table data:"
                   " send_no[%s] message_date[%s] send_status[%s] retry_count[%s] retry_date[%s]",
                   row[0], row[1], row[2], row[3], row[4]);

        user_name       = "";
        jobnet_id       = "";
        jobnet_name     = "";
        job_id          = "";
        job_id_full     = "";
        job_name        = "";
        log_message_id  = "";
        log_message     = "";
        inner_jobnet_main_id = "";

        zbx_strlcpy(yy,  row[1],       sizeof(yy));
        zbx_strlcpy(mm, (row[1] + 4),  sizeof(mm));
        zbx_strlcpy(dd, (row[1] + 6),  sizeof(dd));
        zbx_strlcpy(hh, (row[1] + 8),  sizeof(hh));
        zbx_strlcpy(mi, (row[1] + 10), sizeof(mi));
        zbx_strlcpy(ss, (row[1] + 12), sizeof(ss));

        stm.tm_year  = atoi(yy) - 1900;
        stm.tm_mon   = atoi(mm) - 1;
        stm.tm_mday  = atoi(dd);
        stm.tm_hour  = atoi(hh);
        stm.tm_min   = atoi(mi);
        stm.tm_sec   = atoi(ss);
        stm.tm_isdst = -1;

        if (-1 == mktime(&stm)) {
            zbx_strlcpy(now_time, row[1], sizeof(now_time));
        }
        else {
            strftime(now_time, sizeof(now_time), "%Y/%m/%d %H:%M:%S", &stm);
        }

        send_status     = atoi(row[2]);
        retry_count     = atoi(row[3]);
        message_type    = atoi(row[5]);

        switch (message_type) {
            case 0:
                 zbx_strlcpy(msg_type, "INFO", sizeof(msg_type));
                 break;

            case 1:
                 zbx_strlcpy(msg_type, "CRIT", sizeof(msg_type));
                 break;

            case 2:
                 zbx_strlcpy(msg_type, "ERROR", sizeof(msg_type));
                 break;

            case 3:
                 zbx_strlcpy(msg_type, "WARN", sizeof(msg_type));
                 break;

            default:
                 zbx_strlcpy(msg_type, "DEBUG", sizeof(msg_type));
                 break;
        }

        if (SUCCEED != DBis_null(row[6])) {
            user_name = row[6];
        }

        if (SUCCEED != DBis_null(row[7])) {
            //host_name = row[7];
            if(strlen(row[7]) > 0){
            	zbx_snprintf(host_name, sizeof(host_name), "HOST=%s",row[7]);
            }else{
            	zbx_snprintf(host_name, sizeof(host_name), "HOST=JAZServer ");
            }

        }else{
        	zbx_snprintf(host_name, sizeof(host_name), "HOST=JAZServer ");
        }

        if (SUCCEED != DBis_null(row[8])) {
            jobnet_id = row[8];
        }

        if (SUCCEED != DBis_null(row[9])) {
            jobnet_name = row[9];
        }

        if (SUCCEED != DBis_null(row[10])) {
            job_id = row[10];
        }

        if (SUCCEED != DBis_null(row[11])) {
            job_id_full = row[11];
        }

        if (SUCCEED != DBis_null(row[12])) {
            job_name = row[12];
        }

        if (SUCCEED != DBis_null(row[13])) {
           log_message_id = row[13];
        }

        if (SUCCEED != DBis_null(row[14])) {
            log_message = row[14];
        }
        if (SUCCEED != DBis_null(row[15])) {
        	inner_jobnet_main_id = row[15];
                }

        zbx_snprintf(cmd, sizeof(cmd), "%s -z '%s' -p '%s' -s '%s' -k '%s' -o '[%s] [%s] [%s] %s (USER NAME=%s %s JOBNET=%s JOB=%s INNER_JOBNET_MAIN_ID=%s)'",
                     zbxsnd->sender, zbxsnd->zabbix_ip, zbxsnd->zabbix_port, zbxsnd->zabbix_host, zbxsnd->item_key,
                     now_time, msg_type, log_message_id, log_message, user_name, host_name, jobnet_id, job_id_full, inner_jobnet_main_id);

        /* command execution */
        state = ja_system_call(cmd);
        zabbix_log(LOG_LEVEL_DEBUG, "application execution [%s] (%d)", cmd, state);

        time(&now);
        tm = localtime(&now);
        strftime(now_date, sizeof(now_date), "%Y%m%d%H%M%S", tm);

        if (state == EXIT_SUCCESS) {
            /* sending completion */
            rc = DBexecute("update ja_send_message_table set send_status = %d, send_date = %s"
                           " where send_no = %s",
                           JA_SNT_SEND_STATUS_END, now_date, row[0]);

            if (rc < ZBX_DB_OK) {
                zabbix_log(LOG_LEVEL_ERR, "In %s() failed to update the sent message information. send_no: %s send_status: %d send_date: %s",
                           __function_name, row[0], JA_SNT_SEND_STATUS_END, now_date);
            }
            continue;
        }

        /* sending failure */
        if (WIFEXITED(state)) {
            state = WEXITSTATUS(state);
        }

        zabbix_log(LOG_LEVEL_ERR, "In %s() failed to execute the command. command: (%d) [%s]",
                   __function_name, state, cmd);

        /* retry time get */
        time(&now);
        now = now + zbxsnd->retry_interval;
        tm = localtime(&now);
        strftime(retry_date, sizeof(retry_date), "%Y%m%d%H%M%S", tm);

        /* endless retry ? */
        if (zbxsnd->retry == 1 && zbxsnd->retry_count == 0) {
            rc = DBexecute("update ja_send_message_table set send_status = %d, retry_date = %s"
                           " where send_no = %s",
                           JA_SNT_SEND_STATUS_RETRY, retry_date, row[0]);

            if (rc < ZBX_DB_OK) {
                zabbix_log(LOG_LEVEL_ERR, "In %s() failed to update the sent message information. send_no: %s send_status: %d retry_date: %s",
                           __function_name, row[0], JA_SNT_SEND_STATUS_RETRY, retry_date);
            }
            continue;
        }

        /* resend without or retry the end */
        if (zbxsnd->retry == 0 ||
           (zbxsnd->retry == 1 && retry_count >= zbxsnd->retry_count)) {
            rc = DBexecute("update ja_send_message_table set send_status = %d, send_error_date = %s"
                           " where send_no = %s",
                           JA_SNT_SEND_STATUS_ERROR, now_date, row[0]);

            if (rc < ZBX_DB_OK) {
                zabbix_log(LOG_LEVEL_ERR, "In %s() failed to update the sent message information. send_no: %s send_status: %d send_error_date: %s",
                           __function_name, row[0], JA_SNT_SEND_STATUS_ERROR, now_date);
            }
            continue;
        }

        /* trying again */
        retry_count = retry_count + 1;
        rc = DBexecute("update ja_send_message_table set send_status = %d, retry_count = %d, retry_date = %s"
                       " where send_no = %s",
                       JA_SNT_SEND_STATUS_RETRY, retry_count, retry_date, row[0]);

        if (rc < ZBX_DB_OK) {
            zabbix_log(LOG_LEVEL_ERR, "In %s() failed to update the sent message information. send_no: %s send_status: %d retry_count: %d retry_date: %s",
                       __function_name, row[0], JA_SNT_SEND_STATUS_RETRY, retry_count, retry_date);
        }
    }

    DBfree_result(result);
    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: main_jamsgsnd_loop (main process)                                *
 *                                                                            *
 * Purpose: message notification to Zabbix. Also, delete the sent message     *
 *          information                                                       *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
void    main_jamsgsnd_loop()
{
    ja_zbxsnd_info_t  zbxsnd;
    double            sec;

    zabbix_log(LOG_LEVEL_DEBUG, "In main_jamsgsnd_loop() process_type:'%s' process_num:%d",
               ja_get_process_type_string(process_type), process_num);

    zbx_setproctitle("%s [connecting to the database]", ja_get_process_type_string(process_type));

    ja_alarm_watcher("ja_msgsnd");
    DBconnect(ZBX_DB_CONNECT_NORMAL);

    get_zbxsnd_parameters(&zbxsnd);

    for (;;) {
        zbx_setproctitle("message send [zabbix message send and data purge]");

        /* message purge */
        ja_alarm_timeout(CONFIG_MSGSND_TIMEOUT);
        sec = zbx_time();
        message_purge(&zbxsnd);
        sec = zbx_time() - sec;

        zabbix_log(LOG_LEVEL_DEBUG, "%s #%d (message purge) spent " ZBX_FS_DBL " seconds while processing rules",
                   ja_get_process_type_string(process_type), process_num, sec);

        /* message send */
        sec = zbx_time();
        message_send(&zbxsnd);
        sec = zbx_time() - sec;

        zabbix_log(LOG_LEVEL_DEBUG, "%s #%d (message send) spent " ZBX_FS_DBL " seconds while processing rules",
                   ja_get_process_type_string(process_type), process_num, sec);
        ja_alarm_timeout(0);
        ja_sleep_loop(CONFIG_JAMSGSND_INTERVAL);
    }
}
