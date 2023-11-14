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
#include "log.h"
#include "cfg.h"
#include "zbxgetopt.h"
#include "sysinfo.h"

#if defined(ZABBIX_SERVICE)
#include "service.h"
#elif defined(ZABBIX_DAEMON)
#include "daemon.h"
#endif

#include "jacommon.h"
#include "jahostname.h"
#include "listener.h"
#include "executive.h"
#include "jobarg_agentd.h"
#include "jadbbackup.h"
#include "jajobfile.h"

#include <dirent.h>

/******************************************************************************
 *                                                                            *
 *                                                                            *
 *                                                                            *
 *                                                                            *
 ******************************************************************************/
const char *progname = NULL;

/* Default config file location */
#ifdef _WINDOWS
static char DEFAULT_CONFIG_FILE[] = "C:\\jobarg_agentd.conf";
#else
static char DEFAULT_CONFIG_FILE[] = SYSCONFDIR "/jobarg_agentd.conf";
#endif

/* application TITLE */
const char title_message[] = "Job Arranger Agent"
#if defined(_WIN64)
    " Win64"
#elif defined(WIN32)
    " Win32"
#endif
#if defined(ZABBIX_SERVICE)
    " (service)"
#elif defined(ZABBIX_DAEMON)
    " (daemon)"
#endif
    ;
/* end of application TITLE */

/* application USAGE message */
const char usage_message[] = "[-Vh]"
#ifdef _WINDOWS
    " [-idsx]"
#endif
    " [-c <config-file>]";
/* end of application USAGE message */

/* application HELP message */
const char *help_message[] = {
    "Options:",
    "  -c --config <config-file>  Absolute path to the configuration file",
    "",
    "Other options:",
    "  -h --help                  Give this help",
    "  -V --version               Display version number",
#ifdef _WINDOWS
    "",
    "Functions:",
    "  -i --install          Install Job arranger agent as service",
    "  -d --uninstall        Uninstall Job arranger agent from service",
    "  -s --start            Start Job arranger agent service",
    "  -x --stop             Stop Job arranger agent service",
#endif
    NULL
};

/* end of application HELP message */

/* COMMAND LINE OPTIONS */
static struct zbx_option longopts[] = {
    {"config", 1, NULL, 'c'},
    {"help", 0, NULL, 'h'},
    {"version", 0, NULL, 'V'},
#ifdef _WINDOWS
    {"install", 0, NULL, 'i'},
    {"uninstall", 0, NULL, 'd'},
    {"start", 0, NULL, 's'},
    {"stop", 0, NULL, 'x'},
#endif
    {NULL}
};

static char shortopts[] = "c:hV"
#ifdef _WINDOWS
    "idsx"
#endif
    ;
/* end of COMMAND LINE OPTIONS */

int threads_num = 0;
ZBX_THREAD_HANDLE *threads = NULL;
unsigned char daemon_type = ZBX_DAEMON_TYPE_AGENT;

/******************************************************************************
 *                                                                            *
 * Function: help_jobarg                                                      *
 *                                                                            *
 * Purpose: show the help message                                             *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
void help_jobarg()
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
static void parse_commandline(int argc, char **argv, ZBX_TASK_EX * t)
{
    char ch;
    t->task = ZBX_TASK_START;

    ch = '\0';
    while ((char) EOF != (ch = (char) zbx_getopt_long(argc, argv, shortopts, longopts, NULL))) {
        switch (ch) {
        case 'c':
            CONFIG_FILE = strdup(zbx_optarg);
            break;
        case 'h':
            help_jobarg();
            exit(FAIL);
            break;
        case 'V':
            version_jobarg();
            exit(FAIL);
            break;
#ifdef _WINDOWS
        case 'i':
            t->task = ZBX_TASK_INSTALL_SERVICE;
            break;
        case 'd':
            t->task = ZBX_TASK_UNINSTALL_SERVICE;
            break;
        case 's':
            t->task = ZBX_TASK_START_SERVICE;
            break;
        case 'x':
            t->task = ZBX_TASK_STOP_SERVICE;
            break;
#endif
        default:
            t->task = ZBX_TASK_SHOW_USAGE;
            break;
        }
    }

    if (NULL == CONFIG_FILE)
        CONFIG_FILE = DEFAULT_CONFIG_FILE;
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
static void set_defaults()
{
    char hostname[JA_MAX_STRING_LEN];
    if (NULL == CONFIG_HOSTNAME) {
        if (ja_hostname(hostname) == SUCCEED)
            CONFIG_HOSTNAME = zbx_strdup(CONFIG_HOSTNAME, hostname);
        else
            zabbix_log(LOG_LEVEL_WARNING, "failed to get system hostname");
    }

#ifdef USE_PID_FILE
    if (NULL == CONFIG_PID_FILE)
        CONFIG_PID_FILE = "/var/lib/jobarranger/tmp/jobarg_agentd.pid";
#endif

    if (NULL == CONFIG_TMPDIR)
        CONFIG_TMPDIR = zbx_strdup(CONFIG_TMPDIR, "/var/lib/jobarranger/tmp");

	
    if (NULL == CONFIG_LOG_TYPE_STR)
        CONFIG_LOG_TYPE_STR = zbx_strdup(CONFIG_LOG_TYPE_STR, "file");

    if (CONFIG_JA_BKUP_LOOP_TIMEOUT == 0) {
        CONFIG_JA_BKUP_LOOP_TIMEOUT = 300;
    }
    if (CONFIG_JA_EXEC_LOOP_TIMEOUT == 0) {
        CONFIG_JA_EXEC_LOOP_TIMEOUT = 1800;
    }
    if (CONFIG_JA_LISTEN_LOOP_TIMEOUT == 0) {
        CONFIG_JA_LISTEN_LOOP_TIMEOUT = 300;
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
static void jobarg_load_config(int requirement)
{
    struct cfg_line cfg[] = {
        /* PARAMETER,         VAR,                         TYPE,        MANDATORY, MIN, MAX */
#ifdef USE_PID_FILE
        {"JaPidFile",         &CONFIG_PID_FILE,            TYPE_STRING, PARM_OPT,  0,   0},
#endif
        {"TmpDir",            &CONFIG_TMPDIR,              TYPE_STRING, PARM_MAND, 0,   0},
        {"Server",            &CONFIG_HOSTS_ALLOWED,       TYPE_STRING, PARM_OPT,  0,   0},
        {"Hostname",          &CONFIG_HOSTNAME,            TYPE_STRING, PARM_OPT,  0,   0},
        {"AllowRoot",         &CONFIG_ALLOW_ROOT,          TYPE_INT,    PARM_OPT,  0,   1},
        {"ListenIP",          &CONFIG_LISTEN_IP,           TYPE_STRING, PARM_OPT,  0,   0},
        {"ListenIP",          &CONFIG_SOURCE_IP,           TYPE_STRING, PARM_OPT,  0,   0},
        {"Timeout",           &CONFIG_TIMEOUT,             TYPE_INT,    PARM_OPT,  1,   300},
        {"DebugLevel",        &CONFIG_LOG_LEVEL,           TYPE_INT,    PARM_OPT,  0,   4},
        {"LogFileSize",       &CONFIG_LOG_FILE_SIZE,       TYPE_INT,    PARM_OPT,  0,   1024},
        {"LogType",           &CONFIG_LOG_TYPE_STR,        TYPE_STRING, PARM_OPT,  0,   0},
        {"JaLogFile",         &CONFIG_LOG_FILE,            TYPE_STRING, PARM_OPT,  0,   0},
        {"JaServerPort",      &CONFIG_SERVER_PORT,         TYPE_INT,    PARM_OPT,  1,   65535},
        {"JaListenPort",      &CONFIG_LISTEN_PORT,         TYPE_INT,    PARM_OPT,  1,   65535},
        {"JaSendRetry",       &CONFIG_SEND_RETRY,          TYPE_INT,    PARM_OPT,  0,   3600},
        {"JaJobHistory",      &CONFIG_JOB_HISTORY,         TYPE_INT,    PARM_OPT,  1,   365},
        {"JaBackupTime",      &CONFIG_BACKUP_TIME,         TYPE_INT,    PARM_OPT,  1,   24},
        {"JaBackupRunTime",      &CONFIG_BACKUP_RUN_TIME,  TYPE_INT,    PARM_OPT,  0,   24},
        {"JaExtjobPath",      &CONFIG_EXTJOB_PATH,         TYPE_STRING, PARM_MAND, 0,   0},
        {"JaFcopyTimeout",    &CONFIG_FCOPY_TIMEOUT,       TYPE_INT,    PARM_OPT,  1,   3600},
        {"JaListenRetry",     &CONFIG_LISTEN_RETRY,        TYPE_INT,    PARM_OPT,  0,   3600},
        {"JaExecLoopTimeout",&CONFIG_JA_EXEC_LOOP_TIMEOUT,TYPE_INT,  PARM_OPT,  0,   3600},
        {"JaListenLoopTimeout",&CONFIG_JA_LISTEN_LOOP_TIMEOUT,TYPE_INT,  PARM_OPT,  0,   3600},
        {"JaBkupLoopTimeout",&CONFIG_JA_BKUP_LOOP_TIMEOUT,TYPE_INT,  PARM_OPT,  0,   3600},
        {"JaExecutionUser",   &CONFIG_JA_EXECUTION_USER,   TYPE_STRING, PARM_OPT,  0,   0},
        {"JaCommandUser",     &CONFIG_JA_COMMAND_USER,     TYPE_STRING, PARM_OPT,  0,   0},
        {"JaCommandPassword", &CONFIG_JA_COMMAND_PASSWORD, TYPE_STRING, PARM_OPT,  0,   0},
        {"JaPsCommand",       &CONFIG_JA_PS_COMMAND,       TYPE_STRING, PARM_OPT,  0,   0},
        {NULL}
    };

    parse_cfg_file(CONFIG_FILE, cfg, requirement, JA_CFG_STRICT);
    set_defaults();

    if (CONFIG_HOSTNAME == NULL) {
        zbx_error("Hostname must be defined");
        exit(EXIT_FAILURE);
    }
    if (ZBX_CFG_FILE_REQUIRED == requirement
        && NULL == CONFIG_HOSTS_ALLOWED) {
        zbx_error("Server must be defined");
        exit(EXIT_FAILURE);
    }
    if (ZBX_CFG_FILE_REQUIRED == requirement && NULL == CONFIG_EXTJOB_PATH) {
        zbx_error("extjob path must be defined");
        exit(EXIT_FAILURE);
    }

    CONFIG_REQUEST_FLAG =
        zbx_dsprintf(NULL, "%s%cjobarg_agentd_request_flag.%d", CONFIG_TMPDIR, JA_DLM, (int)getpid());
    CONFIG_CMD_FILE =
        zbx_dsprintf(NULL, "%s%c%s", CONFIG_EXTJOB_PATH, JA_DLM,
                     JA_JOBARG_COMMAND);
    CONFIG_REBOOT_FLAG =
        zbx_dsprintf(NULL, "%s%cjobarg_agentd_reboot_flag", CONFIG_TMPDIR,
                     JA_DLM);
    CONFIG_REBOOT_FILE =
        zbx_dsprintf(NULL, "%s%c%s", CONFIG_EXTJOB_PATH, JA_DLM,
                     JA_JOBARG_REBOOT);
    //set config tmp dir with /tmp/[temp/begin/close/data/end/error/execute] folder path to global variables.
    zbx_snprintf(JA_TEMP_FOLDER, sizeof(JA_TEMP_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "temp");
    zbx_snprintf(JA_DATA_FOLDER, sizeof(JA_DATA_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "data");
    zbx_snprintf(JA_BEGIN_FOLDER, sizeof(JA_BEGIN_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "begin");
    zbx_snprintf(JA_EXEC_FOLDER, sizeof(JA_EXEC_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "exec");
    zbx_snprintf(JA_ERROR_FOLDER, sizeof(JA_ERROR_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "error");
    zbx_snprintf(JA_CLOSE_FOLDER, sizeof(JA_CLOSE_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "close");
    zbx_snprintf(JA_END_FOLDER, sizeof(JA_END_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "end");
    zbx_snprintf(JA_JOBS_FOLDER, sizeof(JA_JOBS_FOLDER), "%s%c%s", CONFIG_TMPDIR, JA_DLM, "jobs");
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
#ifdef _WINDOWS
static int jobarg_exec_service_task(const char *name,
                                    const ZBX_TASK_EX * t)
{
    int ret;

    ret = FAIL;
    switch (t->task) {
    case ZBX_TASK_INSTALL_SERVICE:
        ret = ZabbixCreateService(name, 0);
        break;
    case ZBX_TASK_UNINSTALL_SERVICE:
        ret = ZabbixRemoveService();
        break;
    case ZBX_TASK_START_SERVICE:
        ret = ZabbixStartService();
        break;
    case ZBX_TASK_STOP_SERVICE:
        ret = ZabbixStopService();
        break;
    default:
        assert(0);
    }

    return ret;
}
#endif
#ifndef _WINDOWS
static void	zbx_alrm_signal_handler(int sig, siginfo_t *siginfo, void *context)
{
	if (NULL == siginfo)
	{
		zabbix_log(LOG_LEVEL_DEBUG, "received [signal:%d(%s)] with NULL siginfo",
				sig, get_signal_name(sig));
	}

	if (NULL == context)
	{
		zabbix_log(LOG_LEVEL_DEBUG, "received [signal:%d(%s)] with NULL context",
				sig, get_signal_name(sig));
	}

	switch (sig)
	{
		case SIGALRM:
			break;
		default:
			zabbix_log(LOG_LEVEL_WARNING, "Got signal [signal:%d(%s)]. Ignoring ...",sig, get_signal_name(sig));
	}
}
#endif

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
#ifdef _WINDOWS
DWORD WINAPI tmpFolderPrepare(LPVOID lpParam) {
    //char** folderList = (char**)lpParam;
    DIR* dir;
    FILE* fp;
    int folder_create_status;
    char tmp_file[JA_FILE_PATH_LEN];
    char folderpath[JA_FILE_PATH_LEN];
    int tmp_cnt = 0,iia = 0;
    char* tmp_folder[] = {"temp","begin","close","data","end","error","exec","jobs",NULL};

    while (tmp_folder[tmp_cnt] != NULL) {
        zbx_snprintf(folderpath, sizeof(folderpath), "%s%c%s", CONFIG_TMPDIR, JA_DLM, tmp_folder[tmp_cnt]);
        zabbix_log(LOG_LEVEL_INFORMATION, "Use folder [%s]", folderpath);
        folder_create_status = _mkdir(folderpath);
        if (folder_create_status != 0) {
            if (errno == EEXIST) {
                zabbix_log(LOG_LEVEL_DEBUG, "Directory already exist in %s", folderpath);
            }
            else {
                zabbix_log(LOG_LEVEL_ERR, "Directory cannot be created[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
                return 2;
            }
        }
        //check for read/write permission
        //check for read access.
        dir = opendir(folderpath);
        if (dir == NULL) {
            zabbix_log(LOG_LEVEL_ERR, "Directory cannot be opened.[%s].\n Exiting jobarg_agentd (%s)", folderpath, strerror(errno));
            return 2;
        }
        closedir(dir);
        //check for write access
        zbx_snprintf(tmp_file, sizeof(tmp_file), "%s%ctmp.txt", folderpath, JA_DLM);

        fp = fopen(tmp_file, "wb");
        if (fp == NULL) {
            zabbix_log(LOG_LEVEL_ERR, "Cannot create file under directory[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
            return 2;
        }
        fclose(fp);
        if (remove(tmp_file) != 0) {
            zabbix_log(LOG_LEVEL_ERR, "Cannot delete file under directory[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
            return 2;
        }
        tmp_cnt++;
    }
    return 0;
}
#endif
int MAIN_ZABBIX_ENTRY()
{
    zbx_thread_args_t *thread_args;
#ifndef _WINDOWS
    __sighandler_t sig_alarm_handler;
	struct sigaction	phan;
#endif
    zbx_sock_t listen_sock;
    int thread_num = 0;
    int cnt = 0;
    DIR* dir;
    FILE* fp;
    int tmp_cnt = 0,loop_cnt = 0;
    int folder_create_status;
    char folderpath[JA_FILE_PATH_LEN];
    char tmp_file[JA_FILE_PATH_LEN];
    char* tmp_folder[] = { "temp","begin","close","data","end","error","exec","jobs",NULL };

#ifdef _WINDOWS
    DWORD res;
#else
    int status;
#endif
	
    if (strcmp(CONFIG_LOG_TYPE_STR, ZBX_OPTION_LOGTYPE_SYSTEM) == 0 || NULL == CONFIG_LOG_FILE || '\0' == *CONFIG_LOG_FILE) {
        zabbix_open_log(LOG_TYPE_SYSLOG, CONFIG_LOG_LEVEL, NULL);
    } else {
        zabbix_open_log(LOG_TYPE_FILE, CONFIG_LOG_LEVEL, CONFIG_LOG_FILE);
    }
    zabbix_log(LOG_LEVEL_INFORMATION,
               "Starting Job Arranger Agent. Job Arranger %s (revision %s).",
               JOBARG_VERSION, JOBARG_REVISION);


    zabbix_log(LOG_LEVEL_DEBUG, "My host name is [%s]", CONFIG_HOSTNAME);

    //
     //check for tmp folders before agent start.
#ifdef _WINDOWS
    HANDLE hThread = CreateThread(NULL, 0, tmpFolderPrepare, NULL, 0, NULL);

    if (hThread == NULL) {
        printf("Thread creation failed.\n");
        exit(FAIL);
    }

    // Wait for the thread to finish
    int timeout_sec = 10 * 1000;
    DWORD waitResult = WaitForSingleObject(hThread, timeout_sec);
    if (waitResult == WAIT_OBJECT_0) {
        DWORD threadExitCode;
        GetExitCodeThread(hThread, &threadExitCode); // Get the exit code of the thread

        // Check the exit code against your error codes
        switch (threadExitCode) {
        case 0:
            zabbix_log(LOG_LEVEL_DEBUG, "jobarranger_agent file create finished successfully.\n");
            break;
        default:
            zabbix_log(LOG_LEVEL_ERR, "File creation error occurred. exit code : %d\n", threadExitCode);
            break;
        }
    }
    else if (waitResult == WAIT_FAILED) {
        zabbix_log(LOG_LEVEL_ERR, "File creation thread failed with error code :[%s]", GetLastError());
    }
    else if (waitResult == WAIT_TIMEOUT) {
        zabbix_log(LOG_LEVEL_ERR, "temp folder creation timed out."); 
        TerminateThread(hThread, 0);
    }
    // Close the thread handle
    CloseHandle(hThread);
    
#else
    tmp_cnt = 0;
    sig_alarm_handler = signal(SIGALRM,alarm_handler);
    alarm(10);
    while (tmp_folder[tmp_cnt] != NULL) {
        zbx_snprintf(folderpath, sizeof(folderpath), "%s%c%s", CONFIG_TMPDIR, JA_DLM, tmp_folder[tmp_cnt]);
        zabbix_log(LOG_LEVEL_INFORMATION, "Use folder [%s]", folderpath);

        folder_create_status = mkdir(folderpath,JA_PERMISSION);
        chmod(folderpath, JA_PERMISSION);
        if (folder_create_status != 0) {
            if (errno == EEXIST) {
                zabbix_log(LOG_LEVEL_DEBUG, "Directory already exist in %s", folderpath);
            }
            else {
                zabbix_log(LOG_LEVEL_ERR, "Directory cannot be created[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
                exit(FAIL);
            }
        }
        //check for read/write permission
        //check for read access.
        dir = opendir(folderpath);
        if (dir == NULL) {
            zabbix_log(LOG_LEVEL_ERR, "Directory cannot be opened.[%s].\n Exiting jobarg_agentd (%s)", folderpath, strerror(errno));
            exit(FAIL);
        }
        closedir(dir);
        //check for write access
        zbx_snprintf(tmp_file, sizeof(tmp_file), "%s%ctmp.txt", folderpath, JA_DLM);

        fp = fopen(tmp_file, "wb");
        if (fp == NULL) {
            zabbix_log(LOG_LEVEL_ERR, "Cannot create file under directory[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
            exit(FAIL);
        }
        fclose(fp);
        if (remove(tmp_file) != 0) {
            zabbix_log(LOG_LEVEL_ERR, "Cannot delete file under directory[%s].\nExiting jobarg_agentd (%s)", folderpath, strerror(errno));
            exit(FAIL);
        }
        tmp_cnt++;
    }
    alarm(0);
    signal(SIGALRM, sig_alarm_handler);
    //Set the zabbix default behavior for sigalrm here.
    phan.sa_sigaction = zbx_alrm_signal_handler;
    sigemptyset(&phan.sa_mask);
    phan.sa_flags = SA_SIGINFO;
    sigaction(SIGALRM, &phan, NULL);

#endif
    
    cnt = 0;
    while (1) {
        if (SUCCEED == zbx_tcp_listen(&listen_sock, CONFIG_LISTEN_IP, (unsigned short) CONFIG_LISTEN_PORT)) {
            break;
        }

        if (cnt >= CONFIG_LISTEN_RETRY) {
            zabbix_log(LOG_LEVEL_CRIT, "listener failed: %s", zbx_tcp_strerror());
            exit(EXIT_FAILURE);
        }
        cnt = cnt + 1;
        zbx_sleep(1);
    }

    if (listen_sock.num_socks != 1
        || listen_sock.sockets[0] == ZBX_SOCK_ERROR) {
        zabbix_log(LOG_LEVEL_CRIT,
                   "listener failed(job arranger agent): %s",
                   zbx_tcp_strerror());
        exit(EXIT_FAILURE);
    }

    /* --- START THREADS --- */
#ifndef _WINDOWS
    setpgid(0,0); //set group id.
#endif
    threads_num = 3;
    threads =
        (ZBX_THREAD_HANDLE *) zbx_calloc(threads, threads_num,
                                         sizeof(ZBX_THREAD_HANDLE));

  
    /* start the executive thread */
    thread_args =
        (zbx_thread_args_t *) zbx_malloc(NULL, sizeof(zbx_thread_args_t));
    thread_args->thread_num = thread_num;
    thread_args->args = NULL;
    threads[thread_num++] =
        zbx_thread_start(executive_thread, thread_args);
    

    /* start the listener thread */
    thread_args =
        (zbx_thread_args_t *) zbx_malloc(NULL, sizeof(zbx_thread_args_t));
    thread_args->thread_num = thread_num;
    thread_args->args = &listen_sock;
    threads[thread_num++] = zbx_thread_start(listener_thread, thread_args);

    /* start the jadbbackup thread */
    thread_args =
        (zbx_thread_args_t *) zbx_malloc(NULL, sizeof(zbx_thread_args_t));
    thread_args->thread_num = thread_num;
    thread_args->args = NULL;
    threads[thread_num++] = zbx_thread_start(jadbbackup_thread, thread_args);

#ifdef _WINDOWS
    set_parent_signal_handler();
    res =
        WaitForMultipleObjectsEx(threads_num, threads, FALSE, INFINITE,
                                 FALSE);
    zabbix_log(LOG_LEVEL_CRIT,"One thread has terminated unexpectedly (code:%lu). Exiting ...",res);
    if (ZBX_IS_RUNNING()) {
        zabbix_log(LOG_LEVEL_CRIT,
                   "One thread has terminated unexpectedly (code:%lu). Exiting ...",
                   res);
        THIS_SHOULD_NEVER_HAPPEN;
        ZBX_DO_EXIT();
        zbx_sleep(1);
    } else {
        zbx_sleep(2);
        THIS_SHOULD_NEVER_HAPPEN;
    }
#else
    while (-1 == wait(&status)) {

        if (WIFEXITED(status)) {
            zabbix_log(LOG_LEVEL_ERR,"Child process exited with status: %d\n", WEXITSTATUS(status));
            if ( WEXITSTATUS(status) != 10 ) { exit(WEXITSTATUS(status)); }
        }
        if (WIFSIGNALED(status) && WIFSIGNALED(status) != SIGCLD) {
            zabbix_log(LOG_LEVEL_ERR,"Child process terminated by signal: %d\n", WTERMSIG(status));
            kill(getppid(),WTERMSIG(status));
        }


        if (EINTR != errno) {
            zabbix_log(LOG_LEVEL_ERR,
                       "failed to wait on child processes: %s",
                       zbx_strerror(errno));
            break;
        }
    }
    THIS_SHOULD_NEVER_HAPPEN;
#endif
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
void zbx_on_exit()
{
    int i;
    zabbix_log(LOG_LEVEL_DEBUG, "zbx_on_exit() called");

    if (NULL != threads) {
#ifndef _WINDOWS
        sigset_t set;
        sigemptyset(&set);
        sigaddset(&set, SIGCHLD);
        sigprocmask(SIG_BLOCK, &set, NULL);
#endif
        for (i = 0; i < threads_num; i++) {
            if (threads[i]) {
                zbx_thread_kill(threads[i]);
                threads[i] = ZBX_THREAD_HANDLE_NULL;
            }
        }
        zbx_free(threads);
    }

#ifndef _WINDOWS
    pid_t pgid = getpgid(0);
    if (kill(-pgid, SIGTERM) == -1) {
        zabbix_log(LOG_LEVEL_ERR,"Cannot stop process group pgid : %d",pgid);
        exit(FAIL);
    }
    zbx_sleep(2);
#endif

    zabbix_log(LOG_LEVEL_INFORMATION,
               "Job Arranger Agent stopped. Job Arranger %s (revision %s).",
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
#if defined(HAVE_SIGQUEUE) && defined(ZABBIX_DAEMON)
void zbx_sigusr_handler(zbx_task_t task)
{
    /* nothing to do */
}
#endif

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
    ZBX_TASK_EX t;
#ifdef _WINDOWS
    int ret;
    SetErrorMode(SEM_FAILCRITICALERRORS);
#endif
    memset(&t, 0, sizeof(t));
    t.task = ZBX_TASK_START;

    progname = get_program_name(argv[0]);
    parse_commandline(argc, argv, &t);

    switch (t.task) {
    case ZBX_TASK_SHOW_USAGE:
        usage();
        exit(FAIL);
        break;
#ifdef _WINDOWS
    case ZBX_TASK_INSTALL_SERVICE:
    case ZBX_TASK_UNINSTALL_SERVICE:
    case ZBX_TASK_START_SERVICE:
    case ZBX_TASK_STOP_SERVICE:
        jobarg_load_config(ZBX_CFG_FILE_REQUIRED);
        ret = jobarg_exec_service_task(argv[0], &t);
        exit(ret);
        break;
#endif
    default:
        jobarg_load_config(ZBX_CFG_FILE_REQUIRED);
        break;
    }

	START_MAIN_ZABBIX_ENTRY(CONFIG_ALLOW_ROOT);
    exit(SUCCEED);
}
