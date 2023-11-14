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
#include "comms.h"
#include "pid.h"
#include "cfg.h"
#include "log.h"
#include "zbxgetopt.h"
#include "daemon.h"
#include "db.h"

#include "jacommon.h"
#include "jastr.h"
#include "jaschedule.h"
#include "jajobnet.h"

const char *progname = NULL;
static char DEFAULT_CONFIG_FILE[] = SYSCONFDIR "/jobarg_monitor.conf";

const char title_message[] = "Job Arranger Monitor";
const char usage_message[] = "[-hV] [-c <file>]";
const char *help_message[] = {
    "Options:",
    "  -c --config <file>              Absolute path to the configuration file",
	"  -f --foreground     Run Job Arranger monitor in foreground",
    "",
    "Other options:",
    "  -h --help                       Give this help",
    "  -V --version                    Display version number",
    "",
    "Example: jobarg_monitor -c /etc/jobarrager/jobarg_monitor.conf",
    NULL                        /* end of text */
};

/* long options */
static struct zbx_option longopts[] = {
    {"config", 1, NULL, 'c'},
	{"foreground",		0,	NULL,	'f'},
    {"help", 0, NULL, 'h'},
    {"version", 0, NULL, 'V'},
    {NULL}
};

/* short options */
static char shortopts[] = "c:hV:f";

/* end of COMMAND LINE OPTIONS */

int threads_num = 0;
ZBX_THREAD_HANDLE *threads = NULL;
unsigned char daemon_type = ZBX_DAEMON_TYPE_AGENT;

int  CONFIG_LOG_LEVEL              = LOG_LEVEL_WARNING;
char *CONFIG_DBHOST                = NULL;
char *CONFIG_DBNAME                = NULL;
char *CONFIG_DBSCHEMA              = NULL;
char *CONFIG_DBUSER                = NULL;
char *CONFIG_DBPASSWORD            = NULL;
char *CONFIG_DBSOCKET              = NULL;
int  CONFIG_DBPORT                 = 0;
int  CONFIG_LOAD_SHIFT_TIME        = 0;
int  CONFIG_RUN_SHIFT_TIME         = 0;
int  CONFIG_JAMONITOR_INTERVAL     = 60;
char *CONFIG_SENDER_SCRIPT         = NULL;
int  CONFIG_SPAN_TIME              = 0;
char *CONFIG_JA_EXECUTION_USER     = NULL;

/* for Zabbix */
char *CONFIG_SOURCE_IP             = NULL;
int  CONFIG_NODEID                 = 0;
int  CONFIG_MASTER_NODEID          = 0;
int  CONFIG_NODE_NOHISTORY         = 0;
int  CONFIG_TEXT_CACHE_SIZE        = 0;
int  CONFIG_HISTORY_CACHE_SIZE     = 0;
int  CONFIG_TRENDS_CACHE_SIZE      = 0;
int  CONFIG_CONF_CACHE_SIZE        = 0;
int  CONFIG_POLLER_FORKS           = 0;
int  CONFIG_JAVAPOLLER_FORKS       = 0;
int  CONFIG_HISTSYNCER_FORKS       = 0;
int  CONFIG_IPMIPOLLER_FORKS       = 0;
int  CONFIG_PINGER_FORKS           = 0;
int  CONFIG_HISTSYNCER_FREQUENCY   = 0;
int  CONFIG_UNREACHABLE_DELAY      = 0;
int  CONFIG_UNREACHABLE_PERIOD     = 0;
int  CONFIG_UNAVAILABLE_DELAY      = 0;
int  CONFIG_UNSAFE_USER_PARAMETERS = 0;
int  CONFIG_ENABLE_REMOTE_COMMANDS = 0;
int  CONFIG_LOG_REMOTE_COMMANDS    = 0;
int  CONFIG_SERVER_STARTUP_TIME    = 0;
int  CONFIG_LOG_SLOW_QUERIES       = 0;
int  CONFIG_PROXYCONFIG_FREQUENCY  = 0;
int  CONFIG_PROXYDATA_FREQUENCY    = 0;


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
static void help_jobarg()
{
    const char **p = help_message;
    usage();
    printf("\n");
    while (NULL != *p)
        printf("%s\n", *p++);
}

/******************************************************************************
 *                                                                            *
 * Function: version_jobarg                                                   *
 *                                                                            *
 * Purpose: show the version of the application                               *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
void version_jobarg()
{
    printf("%s v%s (revision %s) (%s)\n", title_message, JOBARG_VERSION, JOBARG_REVISION, JOBARG_REVDATE);
    printf("Compilation time: %s %s\n", __DATE__, __TIME__);
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
static void parse_commandline(int argc, char **argv)
{
    char ch;

    /* parse the command-line */
    CONFIG_FILE = NULL;
    while ((char) EOF != (ch = (char) zbx_getopt_long(argc, argv, shortopts, longopts, NULL))) {
        switch (ch) {
        case 'c':
            CONFIG_FILE = zbx_strdup(CONFIG_FILE, zbx_optarg);
            break;
        case 'h':
            help_jobarg();
            exit(FAIL);
            break;
        case 'V':
            version_jobarg();
            exit(FAIL);
            break;
        case 'f':
            JAZ_TASK_FLAG_FOREGROUND = 1;
            break;
        default:
            usage();
            exit(FAIL);
            break;
        }
    }

    if (NULL == CONFIG_FILE) {
        CONFIG_FILE = DEFAULT_CONFIG_FILE;
    }
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
static void ja_load_config(const char *config_file)
{
    struct cfg_line cfg[] = {
        /* PARAMETER,         VAR,                        TYPE,        MANDATORY, MIN,  MAX */
        {"DBHost",            &CONFIG_DBHOST,             TYPE_STRING, PARM_OPT,  0,    0},
        {"DBName",            &CONFIG_DBNAME,             TYPE_STRING, PARM_MAND, 0,    0},
        {"DBSchema",          &CONFIG_DBSCHEMA,           TYPE_STRING, PARM_OPT,  0,    0},
        {"DBUser",            &CONFIG_DBUSER,             TYPE_STRING, PARM_MAND, 0,    0},
        {"DBPassword",        &CONFIG_DBPASSWORD,         TYPE_STRING, PARM_OPT,  0,    0},
        {"DBSocket",          &CONFIG_DBSOCKET,           TYPE_STRING, PARM_OPT,  0,    0},
        {"DBPort",            &CONFIG_DBPORT,             TYPE_INT,    PARM_OPT,  1024, 65535},
        {"DebugLevel",        &CONFIG_LOG_LEVEL,          TYPE_INT,    PARM_OPT,  0,    4},
        {"LogFileSize",       &CONFIG_LOG_FILE_SIZE,      TYPE_INT,    PARM_OPT,  0,    1024},
        {"JaLogFile",         &CONFIG_LOG_FILE,           TYPE_STRING, PARM_OPT,  0,    0},
        {"JaPidFile",         &CONFIG_PID_FILE,           TYPE_STRING, PARM_OPT,  0,    0},
        {"JaLoadShiftTime",   &CONFIG_LOAD_SHIFT_TIME,    TYPE_INT,    PARM_OPT,  0,    1440},
        {"JaRunShiftTime",    &CONFIG_RUN_SHIFT_TIME,     TYPE_INT,    PARM_OPT,  0,    1440},
        {"JaMonitorInterval", &CONFIG_JAMONITOR_INTERVAL, TYPE_INT,    PARM_OPT,  1,    60},
        {"JaSenderScript",    &CONFIG_SENDER_SCRIPT,      TYPE_STRING, PARM_OPT,  0,    0},
        {"JaExecutionUser",   &CONFIG_JA_EXECUTION_USER,  TYPE_STRING, PARM_OPT,  0,    0},
		{"LogType",			  &CONFIG_LOG_TYPE_STR,       TYPE_STRING, PARM_OPT,	0,	0},
        {NULL}
    };

    parse_cfg_file(config_file, cfg, ZBX_CFG_FILE_REQUIRED, ZBX_CFG_NOT_STRICT);

    if (NULL == CONFIG_DBHOST) {
        CONFIG_DBHOST = zbx_strdup(CONFIG_DBHOST, "localhost");
    }

    if (NULL == CONFIG_PID_FILE) {
        CONFIG_PID_FILE = zbx_strdup(CONFIG_PID_FILE, "/var/run/jobarranger/jobarg_monitor.pid");
    }

    if (NULL == CONFIG_LOG_TYPE_STR) {
        CONFIG_LOG_TYPE_STR = zbx_strdup(CONFIG_LOG_TYPE_STR, "file");
    }
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
void zbx_sigusr_handler(zbx_task_t task)
{
    /* nothing to do */
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
void zbx_on_exit()
{
    int i;

    zabbix_log(LOG_LEVEL_DEBUG, "zbx_on_exit() called");

    if (NULL != threads) {
        for (i = 0; i < threads_num; i++) {
            if (threads[i]) {
                zbx_thread_kill(threads[i]);
                threads[i] = ZBX_THREAD_HANDLE_NULL;
            }
        }
        zbx_free(threads);
    }
    zabbix_log(LOG_LEVEL_INFORMATION, "Job Arranger monitor stopped. Job Arranger %s (revision %s).",
               JOBARG_VERSION, JOBARG_REVISION);
    zabbix_close_log();
    exit(SUCCEED);
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
int ja_sender(const int id, const char *schedule_id, const char *jobnet_id,
              const char *schedule_time, const char *start_time)
{
    int          ret, state;
    char         cmd[JA_MAX_DATA_LEN];
    char         calendar_id[33];
    char         user_name[101];
    char         sc_time[17], st_time[20];
    const char   *__function_name = "ja_sender";

    if (id < 1 || id > 3 || schedule_id == NULL || jobnet_id == NULL || schedule_time == NULL || start_time == NULL) {
        return FAIL;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() id: %d, schedule_id: %s, jobnet_id: %s",
               __function_name, id, schedule_id, jobnet_id);

    zbx_snprintf(calendar_id, sizeof(calendar_id), "");
    zbx_snprintf(user_name,   sizeof(user_name),   "");
    zbx_snprintf(sc_time,     sizeof(sc_time), "%s", schedule_time);
    zbx_snprintf(st_time,     sizeof(st_time), "%s", start_time);

    if (strlen(schedule_id) > 0) {
        ja_schedule_get_calendar_id(schedule_id, calendar_id);
    }

    ja_jobnet_get_user_name(jobnet_id, user_name);
    ja_format_timestamp(schedule_time, sc_time);
    ja_format_timestamp(start_time, st_time);

    switch (id) {
    case 1:
        zabbix_log(LOG_LEVEL_ERR,
                   "[JAMONITOR200001] In %s() jobnet_id '%s' can not be not loaded on schedule_time '%s'. calendar_id: %s, schedule_id: %s, user_name: %s",
                   __function_name, jobnet_id, schedule_time, calendar_id, schedule_id, user_name);
        break;

    case 2:
        zabbix_log(LOG_LEVEL_ERR,
                   "[JAMONITOR200002] In %s() jobnet_id '%s' can not be not run on schedule time '%s'. calendar_id: %s, schedule_id: %s, user_name: %s",
                   __function_name, jobnet_id, schedule_time, calendar_id, schedule_id, user_name);
        break;

    case 3:
        zabbix_log(LOG_LEVEL_ERR,
                   "[JAMONITOR200003] In %s() jobnet_id '%s' start time '%s' is late than schedule time '%s'",
                   __function_name, jobnet_id, start_time, schedule_time, calendar_id, schedule_id, user_name);
        break;

    default:
        break;
    }

    if (CONFIG_SENDER_SCRIPT == NULL) {
        return SUCCEED;
    }

    if (strlen(CONFIG_SENDER_SCRIPT) == 0) {
        return SUCCEED;
    }

    zbx_snprintf(cmd, sizeof(cmd), "%s '%d' '%s' '%s' '%s' '%s' '%s' '%s'",
                 CONFIG_SENDER_SCRIPT, id, calendar_id, schedule_id, jobnet_id, user_name, sc_time, st_time);

    zabbix_log(LOG_LEVEL_INFORMATION, "In %s() cmd: %s", __function_name, cmd);

    ret = ja_system_call(cmd);
    if (ret != 0) {
        if (WIFEXITED(ret)) {
            state = WEXITSTATUS(ret);
        }
        else {
            state = ret;
        }
        zabbix_log(LOG_LEVEL_ERR, "In %s() can not execute the command: %s, ret: %d",
                   __function_name, cmd, state);
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
static void process_monitor()
{
    DB_RESULT    result;
    DB_ROW       row;
    DB_RESULT    result_load;
    DB_ROW       row_load;
    char         *schedule_id, *jobnet_id, *update_date;
    char         sch_time[13], s_date[9], s_time[5];
    struct tm    *tm;
    time_t       now, t, start_time, schedule_time;
    const char   *__function_name = "process_monitor";

    now = time(NULL);

    /* check loader */
    t = now + (CONFIG_SPAN_TIME - CONFIG_LOAD_SHIFT_TIME - 1) * 60;
    tm = localtime(&t);
    zbx_snprintf(s_date, sizeof(s_date), "%.4d%.2d%.2d", tm->tm_year + 1900, tm->tm_mon + 1, tm->tm_mday);
    zbx_snprintf(s_time, sizeof(s_time), "%.2d%.2d", tm->tm_hour, tm->tm_min);

    result = DBselect("select schedule_id, jobnet_id, update_date from ja_schedule_jobnet_table");

    while (NULL != (row = DBfetch(result))) {
        schedule_id = row[0];
        jobnet_id   = row[1];
        update_date = row[2];

        if (ja_schedule_check_time(schedule_id, update_date, s_date, s_time) == FAIL) {
            continue;
        }

        zabbix_log(LOG_LEVEL_DEBUG, "In %s() schedule_id: %s, jobnet_id: %s, date: %s, time: %s",
                   __function_name, schedule_id, jobnet_id, s_date, s_time);

        result_load = DBselect("select start_time from ja_run_jobnet_summary_table"
                               " where jobnet_id = '%s' and scheduled_time = '%s%s' and run_type = %d",
                               jobnet_id, s_date, s_time, JA_JOBNET_RUN_TYPE_NORMAL);

        if ((row_load = DBfetch(result_load)) == NULL) {
            zbx_snprintf(sch_time, sizeof(sch_time), "%s%s", s_date, s_time);
            ja_sender(1, schedule_id, jobnet_id, sch_time, "");
        }
        DBfree_result(result_load);
    }
    DBfree_result(result);

    /* check jarun */
    t = now - (1 + CONFIG_RUN_SHIFT_TIME) * 60;

    result = DBselect("select start_time, scheduled_time, jobnet_id, schedule_id from ja_run_jobnet_summary_table"
                      " where run_type = %d and start_pending_flag = %d",
                      JA_JOBNET_RUN_TYPE_NORMAL, JA_SUMMARY_START_PENDING_NONE);

    while (NULL != (row = DBfetch(result))) {
        start_time    = ja_str2timestamp(row[0]);
        schedule_time = ja_str2timestamp(row[1]);
        jobnet_id     = row[2];
        schedule_id   = row[3];
        if (t >= schedule_time && t < schedule_time + 60) {
            if (start_time == 0) {
                ja_sender(2, schedule_id, jobnet_id, row[1], row[0]);
            }
        }
    }
    DBfree_result(result);
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
ZBX_THREAD_ENTRY(monitor_thread, args)
{
    assert(args);
    zabbix_log(LOG_LEVEL_INFORMATION, "jobarg_monitor #%d started [monitor]", ((zbx_thread_args_t *) args)->thread_num);
    zbx_free(args);

    DBconnect(ZBX_DB_CONNECT_NORMAL);
    CONFIG_SPAN_TIME = ja_schedule_load_span();
    while (ZBX_IS_RUNNING()) {
        zbx_setproctitle("process monitor");
        process_monitor();
        zbx_sleep(CONFIG_JAMONITOR_INTERVAL);
    }

    zabbix_log(LOG_LEVEL_INFORMATION, "jobarg_monitor stopped");
    zbx_thread_exit(0);
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
int MAIN_ZABBIX_ENTRY()
{
    zbx_thread_args_t *thread_args;
    int               thread_num = 0;
    int               status, console_flag=0;

    if (JAZ_TASK_FLAG_FOREGROUND)
    {
    	printf("Starting Job Arranger Server. Job Arranger %s (revision %s).\nPress Ctrl+C to exit.\n\n",
    			 JOBARG_VERSION, JOBARG_REVISION);
    	if (0 == strcmp(CONFIG_LOG_TYPE_STR, ZBX_OPTION_LOGTYPE_CONSOLE))
    	{
    		zabbix_open_log(LOG_TYPE_CONSOLE, CONFIG_LOG_LEVEL, CONFIG_LOG_FILE);
    		console_flag = 1;
    	}
    }
    if (0 == console_flag)
    {
    	if (strcmp(CONFIG_LOG_TYPE_STR,ZBX_OPTION_LOGTYPE_SYSTEM) == 0 || NULL == CONFIG_LOG_FILE || '\0' == *CONFIG_LOG_FILE) {
    		zabbix_open_log(LOG_TYPE_SYSLOG, CONFIG_LOG_LEVEL, NULL);
    	}
    	else {
    		zabbix_open_log(LOG_TYPE_FILE, CONFIG_LOG_LEVEL, CONFIG_LOG_FILE);
    	}
    }

    zabbix_log(LOG_LEVEL_INFORMATION, "Starting Job Arranger monitor. Job Arranger %s (revision %s).",
               JOBARG_VERSION, JOBARG_REVISION);

    /* --- START THREADS --- */
    threads_num = 1;
    threads     = (ZBX_THREAD_HANDLE *) zbx_calloc(threads, threads_num, sizeof(ZBX_THREAD_HANDLE));

    /* start the executive thread */
    thread_args             = (zbx_thread_args_t *) zbx_malloc(NULL, sizeof(zbx_thread_args_t));
    thread_args->thread_num = thread_num;
    thread_args->args       = NULL;
    threads[thread_num++]   = zbx_thread_start(monitor_thread, thread_args);

    while (-1 == wait(&status)) {
        if (EINTR != errno) {
            zabbix_log(LOG_LEVEL_ERR, "failed to wait on child processes: %s", zbx_strerror(errno));
            break;
        }
    }
    THIS_SHOULD_NEVER_HAPPEN;
    zbx_on_exit();

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
int main(int argc, char **argv)
{
    progname = get_program_name(argv[0]);
    parse_commandline(argc, argv);
    ja_load_config(CONFIG_FILE);

    START_MAIN_ZABBIX_ENTRY(CONFIG_ALLOW_ROOT);
    exit(SUCCEED);
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
int process_event(zbx_uint64_t eventid, int source, int object,
                  zbx_uint64_t objectid, const zbx_timespec_t * timespec,
                  int value, unsigned char value_changed, int acknowledged,
                  int force_actions)
{
    return SUCCEED;
}
