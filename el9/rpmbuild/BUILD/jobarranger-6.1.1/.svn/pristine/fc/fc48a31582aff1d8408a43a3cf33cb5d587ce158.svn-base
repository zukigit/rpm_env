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
#include "cfg.h"
#include "pid.h"
#include "log.h"
#include "zbxgetopt.h"
#include "daemon.h"
#include "zbxself.h"
#include "db.h"

#include "jacommon.h"
#include "jalog.h"
#include "jaself.h"
#include "jarun/jarun.h"
#include "jajob/jajob.h"
#include "jajobnet/jajobnet.h"
#include "jatrapper/jatrapper.h"
#include "jaboot/jaboot.h"
#include "jaloader/jaloader.h"
#include "jamsgsnd/jamsgsnd.h"
#include "jaselfmon/jaselfmon.h"
#include "japurge/japurge.h"
#include "jaabort/jaabort.h"


#define INIT_SERVER(type, count)								\
	process_type = type;									\
	process_num = server_num - server_count + count;					\
	ja_log("JASERVER000004", 0, NULL, 0,							\
			server_num, ja_get_process_type_string(process_type), process_num)

const char	*progname = NULL;
const char	title_message[] = "Job Arranger Server";
const char	usage_message[] = "[-hV] [-c <file>]";

const char	*help_message[] = {
	"Options:",
	"  -c --config <file>   Absolute path to the configuration file",
	"  -f --foreground     Run Job Arranger server in foreground",
	"",
	"Other options:",
	"  -h --help            Give this help",
	"  -V --version         Display version number",
	NULL	/* end of text */
};

/* COMMAND LINE OPTIONS */
/* long options */
static struct zbx_option longopts[] = {
	{"config",	1,	NULL,	'c'},
	{"foreground",		0,	NULL,	'f'},
	{"help",	0,	NULL,	'h'},
	{"version",	0,	NULL,	'V'},
	{NULL}
};

/* short options */
static char	shortopts[] = "c:hVR:f";
/* end of COMMAND LINE OPTIONS */

int	threads_num = 0;
pid_t	*threads = NULL;
unsigned char	daemon_type		= ZBX_DAEMON_TYPE_SERVER;
int		process_num		= 0;
unsigned char	process_type		= JA_PROCESS_TYPE_UNKNOWN;

const char	*jobext[]		= { "start", "end", "ret", "stdout", "stderr", NULL };
char		serverid[JA_SERVERID_LEN];

/* for Job Arranger */
char	*CONFIG_HOSTNAME		= NULL;
int	CONFIG_JASELFMON_FORKS		= 1;
int	CONFIG_JAPURGE_FORKS		= 1;
int	CONFIG_JARUN_FORKS			= 1;
int	CONFIG_JAJOB_FORKS			= 1;
int	CONFIG_JAJOBNET_FORKS		= 1;
int	CONFIG_JATRAPPER_FORKS		= 5;
int	CONFIG_JALOADER_FORKS		= 1;
int	CONFIG_JABOOT_FORKS			= 1;
int	CONFIG_JAABORT_FORKS		= 1;
int	CONFIG_JASELFMON_INTERVAL	= 1;
int	CONFIG_JARUN_INTERVAL		= 1;
int	CONFIG_JAJOB_INTERVAL		= 1;
int	CONFIG_JAJOBNET_INTERVAL	= 1;
int	CONFIG_JAABORT_INTERVAL		= 1;
int	CONFIG_JALOADER_INTERVAL	= 30;
int	CONFIG_JABOOT_INTERVAL		= 1;
int	CONFIG_AGENT_LISTEN_PORT	= JA_DEFAULT_AGENT_PORT;
int	CONFIG_LISTEN_PORT		= JA_DEFAULT_SERVER_PORT;
char	*CONFIG_LISTEN_IP		= NULL;
char	*CONFIG_SOURCE_IP		= NULL;
int	CONFIG_JA_JOB_TIMEOUT		= 30;
int	CONFIG_SEND_RETRY			= 3;
char	*CONFIG_EXTJOB_PATH		= NULL;
char	*CONFIG_ERROR_CMD_PATH		= NULL;
char	*CONFIG_JA_LOG_MESSAGE_FILE	= NULL;
int	CONFIG_FCOPY_TIMEOUT		= 180;
int	CONFIG_ZABBIX_VERSION		= 3;
int	CONFIG_JALAUNCH_INTERVAL	= 1;
char	*CONFIG_JA_ZBX_MESSAGE_FILE	= NULL;
char	*CONFIG_JA_EXECUTION_USER	= NULL;
int	CONFIG_JAMSGSND_INTERVAL	= 1;
int	CONFIG_JAMSGSND_FORKS		= 1;
int	CONFIG_EXTJOB_WAITTIME		= 300;
char	*CONFIG_JA_PS_COMMAND		= "ps -ef | awk '{ print $2,$3 }'";
char *CONFIG_JA_ZABBIX_URL = NULL;

/* for Zabbix */
int	CONFIG_ALERTER_FORKS		= 1;
int	CONFIG_DISCOVERER_FORKS		= 1;
int	CONFIG_HOUSEKEEPER_FORKS	= 1;
int	CONFIG_NODEWATCHER_FORKS	= 1;
int	CONFIG_PINGER_FORKS		= 1;
int	CONFIG_POLLER_FORKS		= 5;
int	CONFIG_UNREACHABLE_POLLER_FORKS	= 1;
int	CONFIG_HTTPPOLLER_FORKS		= 1;
int	CONFIG_IPMIPOLLER_FORKS		= 0;
int	CONFIG_TIMER_FORKS		= 1;
int	CONFIG_TRAPPER_FORKS		= 5;
int	CONFIG_SNMPTRAPPER_FORKS	= 0;
int	CONFIG_JAVAPOLLER_FORKS		= 0;
int	CONFIG_ESCALATOR_FORKS		= 1;
int	CONFIG_SELFMON_FORKS		= 1;
int	CONFIG_WATCHDOG_FORKS		= 1;
int	CONFIG_DATASENDER_FORKS		= 0;
int	CONFIG_HEARTBEAT_FORKS		= 0;
int	CONFIG_HISTSYNCER_FORKS		= 4;
int	CONFIG_CONFSYNCER_FORKS		= 1;
int	CONFIG_TRAPPER_TIMEOUT		= 300;
int	CONFIG_HISTSYNCER_FREQUENCY	= 5;
int	CONFIG_CONF_CACHE_SIZE		= 8 * ZBX_MEBIBYTE;
int	CONFIG_HISTORY_CACHE_SIZE	= 8 * ZBX_MEBIBYTE;
int	CONFIG_TRENDS_CACHE_SIZE	= 4 * ZBX_MEBIBYTE;
int	CONFIG_TEXT_CACHE_SIZE		= 16 * ZBX_MEBIBYTE;
int	CONFIG_DISABLE_HOUSEKEEPING	= 0;
int	CONFIG_UNREACHABLE_PERIOD	= 45;
int	CONFIG_UNREACHABLE_DELAY	= 15;
int	CONFIG_UNAVAILABLE_DELAY	= 60;
int	CONFIG_LOG_LEVEL		= LOG_LEVEL_WARNING;
char	*CONFIG_ALERT_SCRIPTS_PATH	= NULL;
char	*CONFIG_EXTERNALSCRIPTS		= NULL;
char	*CONFIG_TMPDIR			= NULL;
char	*CONFIG_FPING_LOCATION		= NULL;
#ifdef HAVE_IPV6
char	*CONFIG_FPING6_LOCATION		= NULL;
#endif
char	*CONFIG_DBHOST			= NULL;
char	*CONFIG_DBNAME			= NULL;
char	*CONFIG_DBSCHEMA		= NULL;
char	*CONFIG_DBUSER			= NULL;
char	*CONFIG_DBPASSWORD		= NULL;
char	*CONFIG_DBSOCKET		= NULL;
int	CONFIG_DBPORT			= 0;
int	CONFIG_ENABLE_REMOTE_COMMANDS	= 0;
int	CONFIG_LOG_REMOTE_COMMANDS	= 0;
int	CONFIG_UNSAFE_USER_PARAMETERS	= 0;

int	CONFIG_NODEID			= 0;
int	CONFIG_MASTER_NODEID		= 0;
int	CONFIG_NODE_NOEVENTS		= 0;
int	CONFIG_NODE_NOHISTORY		= 0;

char	*CONFIG_SNMPTRAP_FILE		= NULL;
char	*CONFIG_JAVA_GATEWAY		= NULL;
int	CONFIG_JAVA_GATEWAY_PORT	= ZBX_DEFAULT_GATEWAY_PORT;

char	*CONFIG_SSH_KEY_LOCATION	= NULL;
int	CONFIG_LOG_SLOW_QUERIES		= 0;	/* ms; 0 - disable */
int	CONFIG_SERVER_STARTUP_TIME	= 0;	/* zabbix server startup time */
int	CONFIG_PROXYPOLLER_FORKS	= 1;	/* parameters for passive proxies */

/* how often zabbix server sends configuration data to proxy, in seconds */
int	CONFIG_PROXYCONFIG_FREQUENCY	= 3600;	/* 1h */
int	CONFIG_PROXYDATA_FREQUENCY	= 1;	/* 1s */

/* mutex for node syncs */
//ZBX_MUTEX	node_sync_access;

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
 * Function: zbx_set_defaults                                                 *
 *                                                                            *
 * Purpose: set configuration defaults                                        *
 *                                                                            *
 * Author: Vladimir Levijev                                                   *
 *                                                                            *
 ******************************************************************************/
static void zbx_set_defaults()
{
	CONFIG_SERVER_STARTUP_TIME = time(NULL);

	if (NULL == CONFIG_DBHOST)
		CONFIG_DBHOST = zbx_strdup(CONFIG_DBHOST, "localhost");

	if (NULL == CONFIG_PID_FILE)
		CONFIG_PID_FILE = zbx_strdup(CONFIG_PID_FILE, "/var/run/jobarranger/jobarg_server.pid");

	if (NULL == CONFIG_TMPDIR)
		CONFIG_TMPDIR = zbx_strdup(CONFIG_TMPDIR, "tmp");

	if (NULL == CONFIG_LOG_TYPE_STR)
		CONFIG_LOG_TYPE_STR = zbx_strdup(CONFIG_LOG_TYPE_STR,"file");
}

/******************************************************************************
 *                                                                            *
 * Function: zbx_load_config                                                  *
 *                                                                            *
 * Purpose: parse config file and update configuration parameters             *
 *                                                                            *
 * Parameters:                                                                *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Author: Alexei Vladishev                                                   *
 *                                                                            *
 * Comments: will terminate process if parsing fails                          *
 *                                                                            *
 ******************************************************************************/
static void zbx_load_config()
{
	static struct cfg_line	cfg[] = {
		/* PARAMETER,		VAR,				TYPE,			MANDATORY,	MIN,	MAX */
		{"TmpDir",		&CONFIG_TMPDIR,			TYPE_STRING,		PARM_OPT,	0,	0},
		{"DBHost",		&CONFIG_DBHOST,			TYPE_STRING,		PARM_OPT,	0,	0},
		{"DBName",		&CONFIG_DBNAME,			TYPE_STRING,		PARM_MAND,	0,	0},
		{"DBSchema",		&CONFIG_DBSCHEMA,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"DBUser",		&CONFIG_DBUSER,			TYPE_STRING,		PARM_MAND,	0,	0},
		{"DBPassword",		&CONFIG_DBPASSWORD,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"DBSocket",		&CONFIG_DBSOCKET,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"DBPort",		&CONFIG_DBPORT,			TYPE_INT,		PARM_OPT,	1024,	65535},
		{"LogSlowQueries",	&CONFIG_LOG_SLOW_QUERIES,	TYPE_INT,		PARM_OPT,	0,	3600000},
		{"ListenIP",		&CONFIG_LISTEN_IP,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"SourceIP",		&CONFIG_SOURCE_IP,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"Timeout",		&CONFIG_TIMEOUT,		TYPE_INT,		PARM_OPT,	1,	300},
		{"DebugLevel",		&CONFIG_LOG_LEVEL,		TYPE_INT,		PARM_OPT,	0,	4},
		{"LogFileSize",		&CONFIG_LOG_FILE_SIZE,		TYPE_INT,		PARM_OPT,	0,	1024},
		{"LogType",			&CONFIG_LOG_TYPE_STR,	TYPE_STRING,		PARM_OPT,	0,	0},
		{"JaLogFile",		&CONFIG_LOG_FILE,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"JaPidFile",		&CONFIG_PID_FILE,		TYPE_STRING,		PARM_OPT,	0,	0},
		{"JaSelfmonInterval",	&CONFIG_JASELFMON_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaLoaderInterval",	&CONFIG_JALOADER_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaBootInterval",	&CONFIG_JABOOT_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaJobnetInterval",	&CONFIG_JAJOBNET_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaJobInterval",	&CONFIG_JAJOB_INTERVAL,		TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaJobTimeout",	&CONFIG_JA_JOB_TIMEOUT,		TYPE_INT,		PARM_OPT,	0,	600},
		{"JaSendRetry",		&CONFIG_SEND_RETRY,			TYPE_INT,		PARM_OPT,	0,	3600},
		{"JaRunInterval",	&CONFIG_JARUN_INTERVAL,		TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaStartTrappers",	&CONFIG_JATRAPPER_FORKS,	TYPE_INT,		PARM_OPT,	0,	1000},
		{"JaTrapperListenPort",	&CONFIG_LISTEN_PORT,		TYPE_INT,		PARM_OPT,	1,	65535},
		{"JaAgentListenPort",	&CONFIG_AGENT_LISTEN_PORT,	TYPE_INT,		PARM_OPT,	1,	65535},
		{"JaExtjobPath",	&CONFIG_EXTJOB_PATH,		TYPE_STRING,		PARM_MAND,	0,	0},
		{"JaErrorCmdPath",	&CONFIG_ERROR_CMD_PATH,		TYPE_STRING,		PARM_MAND,	0,	0},
		{"JaLogMessageFile",	&CONFIG_JA_LOG_MESSAGE_FILE,	TYPE_STRING,		PARM_MAND,	0,	0},
		{"JaFcopyTimeout",	&CONFIG_FCOPY_TIMEOUT,		TYPE_INT,		PARM_OPT,	1,	3600},
		//{"JaZabbixVersion",	&CONFIG_ZABBIX_VERSION,		TYPE_INT,		PARM_OPT,	1,	3},
		{"JaLaunchInterval",	&CONFIG_JALAUNCH_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaZabbixMessageFile",	&CONFIG_JA_ZBX_MESSAGE_FILE,	TYPE_STRING,		PARM_MAND,	0,	0},
		{"JaExecutionUser",	&CONFIG_JA_EXECUTION_USER,	TYPE_STRING,		PARM_OPT,	0,	0},
		{"JaMsgsndInterval",	&CONFIG_JAMSGSND_INTERVAL,	TYPE_INT,		PARM_OPT,	1,	9999999},
		{"JaExtjobWaitTime",	&CONFIG_EXTJOB_WAITTIME,	TYPE_INT,		PARM_OPT,	0,	9999999},
		{"JaZabbixURL",	&CONFIG_JA_ZABBIX_URL,	TYPE_STRING,		PARM_OPT,	0,	0},
		{NULL}
	};

	parse_cfg_file(CONFIG_FILE, cfg, ZBX_CFG_FILE_REQUIRED, ZBX_CFG_NOT_STRICT);
	zbx_set_defaults();
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
#ifdef HAVE_SIGQUEUE
void zbx_sigusr_handler(zbx_task_t task)
{
	switch (task) {
	case ZBX_TASK_CONFIG_CACHE_RELOAD:
		if (ZBX_PROCESS_TYPE_CONFSYNCER == process_type) {
			ja_log("JASERVER300001", 0, NULL, 0);
			ja_wakeup();
		}
		break;
	default:
		break;
	}
}
#endif

/******************************************************************************
 *                                                                            *
 * Function: main                                                             *
 *                                                                            *
 * Purpose: executes server processes                                         *
 *                                                                            *
 * Author: Eugene Grigorjev                                                   *
 *                                                                            *
 ******************************************************************************/
int main(int argc, char **argv)
{
	struct tm	*tm;
	struct timeval	tv;
	int		ms;
	char		ch = '\0';
	int		opt_c = 0;

	progname = get_program_name(argv[0]);

	/* parse the command-line */
	while ((char) EOF != (ch = (char) zbx_getopt_long(argc, argv, shortopts, longopts, NULL))) {

		switch (ch) {
		case 'c':
			opt_c++;
			if (NULL == CONFIG_FILE)
				CONFIG_FILE = zbx_strdup(CONFIG_FILE, zbx_optarg);
			break;
		case 'h':
			help_jobarg();
			exit(-1);
			break;
		case 'V':
			version_jobarg();
			exit(-1);
			break;
		case 'f':
			JAZ_TASK_FLAG_FOREGROUND = 1;
			break;
		default:
			usage();
			exit(-1);
			break;
		}
	}

	/* every option may be specified only once */
	if (1 > opt_c)
	{
		printf("option \"-c\" or \"--config\" specified multiple times \n");
		exit(1);
	}

	if (NULL == CONFIG_FILE)
	{
		CONFIG_FILE = zbx_strdup(CONFIG_FILE, SYSCONFDIR "/jobarg_server.conf");
	}

	zbx_load_config();


	gettimeofday(&tv, NULL);
	tm = localtime(&tv.tv_sec);
	ms = (int)(tv.tv_usec / 1000);
	zbx_snprintf(serverid, sizeof(serverid), "%04d%02d%02d%02d%02d%02d%03d",
			1900 + tm->tm_year, 1 + tm->tm_mon, tm->tm_mday,
			tm->tm_hour, tm->tm_min, tm->tm_sec, ms);

	return daemon_start(CONFIG_ALLOW_ROOT);
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
	pid_t pid;
	zbx_sock_t	listen_sock;
	int		i, server_num = 0, server_count = 0, console_flag = 0;

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
		if (strcmp(CONFIG_LOG_TYPE_STR, ZBX_OPTION_LOGTYPE_SYSTEM) == 0 ||NULL == CONFIG_LOG_FILE || '\0' == *CONFIG_LOG_FILE)
			zabbix_open_log(LOG_TYPE_SYSLOG, CONFIG_LOG_LEVEL, NULL);
		else
			zabbix_open_log(LOG_TYPE_FILE, CONFIG_LOG_LEVEL, CONFIG_LOG_FILE);

	}

	ja_log("JASERVER000001", 0, NULL, 0, JOBARG_VERSION, JOBARG_REVISION);

	ja_init_selfmon_collector();


	threads_num = CONFIG_JARUN_FORKS + CONFIG_JATRAPPER_FORKS + CONFIG_JAJOB_FORKS
			+ CONFIG_JAJOBNET_FORKS + CONFIG_JALOADER_FORKS + CONFIG_JABOOT_FORKS
			+ CONFIG_JAMSGSND_FORKS + CONFIG_JASELFMON_FORKS + CONFIG_JAPURGE_FORKS + CONFIG_JAABORT_INTERVAL;

	threads = zbx_calloc(threads, threads_num, sizeof(pid_t));

	if (FAIL == zbx_tcp_listen(&listen_sock, CONFIG_LISTEN_IP, (unsigned short) CONFIG_LISTEN_PORT)) {
		ja_log("JASERVER100001", 0, NULL, 0, zbx_tcp_strerror());
		exit(1);
	}

	char folderpath[JA_FILE_PATH_LEN];
	char tmp_file[JA_FILE_PATH_LEN];
	const char *lastSlash = strrchr(CONFIG_LOG_FILE, '/');
	char var_file_name[JA_FILE_NAME_LEN];
	int folder_create_status;
	DIR *dir;
    FILE* fp;

	zbx_snprintf(var_file_name, sizeof(var_file_name), "/job");

	if (lastSlash != NULL)
	{
		size_t directoryLength = lastSlash - CONFIG_LOG_FILE;
		zbx_strlcpy(folderpath, CONFIG_LOG_FILE, directoryLength + 1);
		strcat(folderpath, var_file_name);
		folderpath[directoryLength + strlen(var_file_name)] = '\0';
	}

	folder_create_status = mkdir(folderpath, JA_PERMISSION);
	chmod(folderpath, JA_PERMISSION);
	if (folder_create_status != 0)
	{
		if (errno == EEXIST)
		{
			zabbix_log(LOG_LEVEL_DEBUG, "Temporary job Directory already exist in %s", folderpath);
		}
		else
		{
			zabbix_log(LOG_LEVEL_ERR, "Temporary job  Directory cannot be created[%s].\nExiting creation (%s)", folderpath, strerror(errno));
			exit(FAIL);
		}
	}

	dir = opendir(folderpath);
	if (dir == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "Temporary job Directory cannot be opened.[%s].\n Exiting creation (%s)", folderpath, strerror(errno));
		exit(FAIL);
	}
	closedir(dir);
	// check for write access
	zbx_snprintf(tmp_file, sizeof(tmp_file), "%s%ctmp.txt", folderpath, JA_DLM);

	fp = fopen(tmp_file, "wb");
	if (fp == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "Cannot create file under Temporary job directory[%s].\nExiting jobarranger server (%s)", folderpath, strerror(errno));
		exit(FAIL);
	}
	fclose(fp);
	if (remove(tmp_file) != 0)
	{
		zabbix_log(LOG_LEVEL_ERR, "Cannot delete file under Temporary job directory[%s].\nExiting jobarranger server(%s)", folderpath, strerror(errno));
		exit(FAIL);
	}

	//check for tmp folder
	zbx_snprintf(var_file_name, sizeof(var_file_name), "/jobstatus.chk");

	if (lastSlash != NULL)
	{
		size_t directoryLength = lastSlash - CONFIG_LOG_FILE;
		zbx_strlcpy(folderpath, CONFIG_LOG_FILE, directoryLength + 1);
		strcat(folderpath, var_file_name);
		folderpath[directoryLength + strlen(var_file_name)] = '\0';
	}
	fp = fopen(folderpath,"w");
	if(fp == NULL)
	{
		zabbix_log(LOG_LEVEL_ERR, "Cannot create job check file[%s].\nExiting jobarranger server (%s)", folderpath, strerror(errno));
		exit(FAIL);
	}
	fclose(fp);

	for (i = 0; i < threads_num; i++) {
		if (0 == (pid = zbx_child_fork())) {
			server_num = i + 1;
			break;
		} else
			threads[i] = pid;
	}

	if (0 == server_num) {
			ja_log("JASERVER000003", 0, NULL, 0);

		while (-1 == wait(&i)) {
			if (EINTR != errno) {
				ja_log("JASERVER200001", 0, NULL, 0, zbx_strerror(errno));
				break;
			}
		}
		THIS_SHOULD_NEVER_HAPPEN;
		zbx_on_exit();
	} else if (server_num <= (server_count += CONFIG_JAJOBNET_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JAJOBNET, CONFIG_JAJOBNET_FORKS);
		main_jajobnet_loop();
	} else if (server_num <= (server_count += CONFIG_JARUN_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JARUN, CONFIG_JARUN_FORKS);
		main_jarun_loop();
	} else if (server_num <= (server_count += CONFIG_JAJOB_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JAJOB, CONFIG_JAJOB_FORKS);
		main_jajob_loop();
	} else if (server_num <= (server_count += CONFIG_JATRAPPER_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JATRAPPER, CONFIG_JATRAPPER_FORKS);
		main_jatrapper_loop(&listen_sock);
	} else if (server_num <= (server_count += CONFIG_JALOADER_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JALOADER, CONFIG_JALOADER_FORKS);
		main_jaloader_loop();
	} else if (server_num <= (server_count += CONFIG_JABOOT_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JABOOT, CONFIG_JABOOT_FORKS);
		main_jaboot_loop();
	} else if (server_num <= (server_count += CONFIG_JAMSGSND_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JASNDMSG, CONFIG_JAMSGSND_FORKS);
		main_jamsgsnd_loop();
	} else if (server_num <= (server_count += CONFIG_JASELFMON_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_SELFMON, CONFIG_JASELFMON_FORKS);
		main_jaselfmon_loop(threads , threads_num);
	} else if (server_num <= (server_count += CONFIG_JAPURGE_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JAPURGE, CONFIG_JAPURGE_FORKS);
		main_japurge_loop();
	} else if (server_num <= (server_count += CONFIG_JAABORT_FORKS)) {
		INIT_SERVER(JA_PROCESS_TYPE_JAABORT, CONFIG_JAABORT_FORKS);
		main_jaabort_loop();
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
void zbx_on_exit()
{
	int	rc;
	zabbix_log(LOG_LEVEL_DEBUG, "zbx_on_exit() called");

	if (SUCCEED == DBtxn_ongoing())
		DBrollback();

	if (NULL != threads) {
		int		i;
		sigset_t	set;

		/* ignore SIGCHLD signals in order for zbx_sleep() to work */
		sigemptyset(&set);
		sigaddset(&set, SIGCHLD);
		sigprocmask(SIG_BLOCK, &set, NULL);

		for (i = 0; i < threads_num; i++) {
			if (threads[i]) {
				kill(threads[i], SIGTERM);
				threads[i] = ZBX_THREAD_HANDLE_NULL;
			}
		}

		zbx_free(threads);
	}

	zbx_sleep(2);	/* wait for all child processes to exit */

	ja_free_selfmon_collector();

	/* kill the external command */
	rc = system("pkill -SIGTERM jobarg_session");
	rc = system("pkill -SIGTERM jobarg_command");

	ja_log("JASERVER000002", 0, NULL, 0, JOBARG_VERSION, JOBARG_REVISION);
	zabbix_close_log();
	exit(SUCCEED);
}
