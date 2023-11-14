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
#include "db.h"
#include "dbcache.h"
#include "threads.h"

#include "jacommon.h"
#include "jalog.h"
#include "jastr.h"
#include "jastatus.h"
#include "jaflow.h"
#include "jahost.h"
#include "jaconnect.h"
#include "javalue.h"

#define LOOP_CONTINUE           0
#define LOOP_EXIT               1
#define TRANSACTION_FAIL       -1

const char *progname       = NULL;
const char title_message[] = "Job Arranger Session management";
const char usage_message[] = "[-hV] -S <session-id> -r <registry-number> [-T <test-flag>] [-c <file>]";
const char *help_message[] = {
    "Options:",
    "  -S --session-id <session-id>              Specify session id",
    "  -r --registry-number <registry-number>    Specify the main jobnet registration number",
    " [-T --test-flag <test-flag>]               Specify the presence or absence of the test mode (0:non test mode, 1:test mode). Default is non test mode",
    " [-c --config <file>]                       Absolute path to the configuration file",
    "",
    "Other options:",
    "  -h --help                                 Give this help",
    "  -V --version                              Display version number",
    NULL    /* end of text */
};

/* COMMAND LINE OPTIONS */
/* long options */
static struct zbx_option longopts[] = {
    {"session-id",      1, NULL, 'S'},
    {"registry-number", 1, NULL, 'r'},
    {"test-flag",       1, NULL, 'T'},
    {"config",          1, NULL, 'c'},
    {"help",            0, NULL, 'h'},
    {"version",         0, NULL, 'V'},
    {NULL}
};

/* short options */
static char shortopts[] = "S:r:T:c:hV";
/* end of COMMAND LINE OPTIONS */

int  CONFIG_LOG_LEVEL        = LOG_LEVEL_WARNING;
char *CONFIG_DBHOST          = NULL;
char *CONFIG_DBNAME          = NULL;
char *CONFIG_DBSCHEMA        = NULL;
char *CONFIG_DBUSER          = NULL;
char *CONFIG_DBPASSWORD      = NULL;
char *CONFIG_DBSOCKET        = NULL;
int  CONFIG_DBPORT           = 0;

int  CONFIG_AGENT_LISTEN_PORT    = JA_DEFAULT_AGENT_PORT;
int  CONFIG_ZABBIX_VERSION       = 3;
char *CONFIG_JA_LOG_MESSAGE_FILE = NULL;
char *CONFIG_ERROR_CMD_PATH      = NULL;

char *JOBARG_SESSION_ID      = NULL;
char *JOBARG_REGISTRY_NUMBER = NULL;
int  JOBARG_TEST_FLAG        = 0;

unsigned char daemon_type    = ZBX_DAEMON_TYPE_SERVER;

static const char *password;
static zbx_sock_t sock;
LIBSSH2_SESSION   *session   = NULL;
LIBSSH2_CHANNEL   *channel   = NULL;
time_t            now_time   = 0;
time_t            last_time  = 0;

/* information agentless icon */
static zbx_uint64_t inner_jobnet_id;
static int  operation_flag;
static int  host_flag;
static int  connection_method;
static int  session_flag;
static int  auth_method;
static int  run_mode;
static int  line_feed_code;
static int  timeout;

static char session_id[64+1];
static char login_user[256+1];
static char login_password[256+1];
static char public_key[2048+1];
static char private_key[2048+1];
static char passphrase[256+1];
static char host_name[JA_HOST_NAME_LEN];
static char stop_code[JA_STOP_CODE_LEN];
static char terminal_type[80+1];
static char character_code[80+1];
static char prompt_string[1024+1];
static char command[8000+1];
static char host[JA_HOST_NAME_LEN];

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

/* other items */
static int  icon_status;

/******************************************************************************
 *                                                                            *
 * Function: session_signal_handler                                           *
 *                                                                            *
 * Purpose: do the processing of the signal received                          *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void session_signal_handler(int sig)
{
    if (SIGALRM != sig) {
        exit(FAIL);
    }
}

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
static void help_jobarg()
{
    const char **p = help_message;
    usage();
    printf("\n");
    while (NULL != *p) {
        printf("%s\n", *p++);
    }
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
 * Function: parse_commandline                                                *
 *                                                                            *
 * Purpose: parse the command line options                                    *
 *                                                                            *
 * Parameters: argc (in) - number of parameters                               *
 *             argv (in) - Parameter values                                   *
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
            case 'S':
                JOBARG_SESSION_ID = zbx_strdup(JOBARG_SESSION_ID, zbx_optarg);
                break;
            case 'r':
                JOBARG_REGISTRY_NUMBER = zbx_strdup(JOBARG_REGISTRY_NUMBER, zbx_optarg);
                break;
            case 'T':
                JOBARG_TEST_FLAG = atoi(zbx_optarg);
                break;
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
            default:
                usage();
                exit(FAIL);
                break;
        }
    }

    /* check the required parameters */
    if (JOBARG_SESSION_ID == NULL || JOBARG_REGISTRY_NUMBER == NULL) {
        usage();
        exit(FAIL);
    }

    /* set the default value */
    if (CONFIG_FILE == NULL) {
        CONFIG_FILE = zbx_strdup(CONFIG_FILE, SYSCONFDIR "/jobarg_server.conf");
    }
    if (CONFIG_LOG_TYPE_STR == NULL){
		CONFIG_LOG_TYPE_STR = zbx_strdup(CONFIG_LOG_TYPE_STR,"file");
    }
}

/******************************************************************************
 *                                                                            *
 * Function: ja_load_config                                                   *
 *                                                                            *
 * Purpose: parse config file and update configuration parameters             *
 *                                                                            *
 * Parameters: config_file (in) - config file name                            *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void ja_load_config(const char *config_file)
{
    struct cfg_line cfg[] = {
        /* PARAMETER,            VAR,                          TYPE,         MANDATORY,  MIN,   MAX */
        {"DBHost",               &CONFIG_DBHOST,               TYPE_STRING,  PARM_OPT,   0,     0},
        {"DBName",               &CONFIG_DBNAME,               TYPE_STRING,  PARM_MAND,  0,     0},
        {"DBSchema",             &CONFIG_DBSCHEMA,             TYPE_STRING,  PARM_OPT,   0,     0},
        {"DBUser",               &CONFIG_DBUSER,               TYPE_STRING,  PARM_MAND,  0,     0},
        {"DBPassword",           &CONFIG_DBPASSWORD,           TYPE_STRING,  PARM_OPT,   0,     0},
        {"DBSocket",             &CONFIG_DBSOCKET,             TYPE_STRING,  PARM_OPT,   0,     0},
        {"DBPort",               &CONFIG_DBPORT,               TYPE_INT,     PARM_OPT,   1024,  65535},
        {"LogSlowQueries",       &CONFIG_LOG_SLOW_QUERIES,     TYPE_INT,     PARM_OPT,   0,     3600000},
        {"SourceIP",             &CONFIG_SOURCE_IP,            TYPE_STRING,  PARM_OPT,   0,     0},
        {"Timeout",              &CONFIG_TIMEOUT,              TYPE_INT,     PARM_OPT,   1,     300},
        {"DebugLevel",           &CONFIG_LOG_LEVEL,            TYPE_INT,     PARM_OPT,   0,     4},
        {"LogFileSize",          &CONFIG_LOG_FILE_SIZE,        TYPE_INT,     PARM_OPT,   0,     1024},
		{"LogType",			     &CONFIG_LOG_TYPE_STR,	       TYPE_STRING,	 PARM_OPT,	 0,	    0},
        {"JaLogFile",            &CONFIG_LOG_FILE,             TYPE_STRING,  PARM_OPT,   0,     0},
        {"JaAgentListenPort",    &CONFIG_AGENT_LISTEN_PORT,    TYPE_INT,     PARM_OPT,   1,     65535},
        {"JaErrorCmdPath",       &CONFIG_ERROR_CMD_PATH,       TYPE_STRING,  PARM_MAND,  0,     0},
        {"JaLogMessageFile",     &CONFIG_JA_LOG_MESSAGE_FILE,  TYPE_STRING,  PARM_MAND,  0,     0},
        {"JaZabbixVersion",      &CONFIG_ZABBIX_VERSION,       TYPE_INT,     PARM_OPT,   1,     3},
        {"JaSesDebugLevel",      &CONFIG_LOG_LEVEL,            TYPE_INT,     PARM_OPT,   0,     4},
        {NULL}
    };

    parse_cfg_file(config_file, cfg, ZBX_CFG_FILE_REQUIRED, ZBX_CFG_NOT_STRICT);

    /* set the default value */
    if (NULL == CONFIG_DBHOST) {
        CONFIG_DBHOST = zbx_strdup(CONFIG_DBHOST, "localhost");
    }
}

/******************************************************************************
 *                                                                            *
 * Function: process_event                                                    *
 *                                                                            *
 * Purpose: dummy function                                                    *
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

/******************************************************************************
 *                                                                            *
 * Function: init_agentless_info                                              *
 *                                                                            *
 * Purpose: initialization of agentless information icon area                 *
 *                                                                            *
 * Return value:                                                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static void init_agentless_info(void)
{
    inner_jobnet_id   = 0;
    operation_flag    = 0;
    host_flag         = 0;
    connection_method = 0;
    session_flag      = 0;
    auth_method       = 0;
    run_mode          = 0;
    line_feed_code    = 0;
    timeout           = 0;

    session_id[0]     = '\0';
    login_user[0]     = '\0';
    login_password[0] = '\0';
    public_key[0]     = '\0';
    private_key[0]    = '\0';
    passphrase[0]     = '\0';
    host_name[0]      = '\0';
    stop_code[0]      = '\0';
    terminal_type[0]  = '\0';
    character_code[0] = '\0';
    prompt_string[0]  = '\0';
    command[0]        = '\0';
    host[0]           = '\0';

    /* signal handler definition */
    signal(SIGINT,  session_signal_handler);
    signal(SIGTERM, session_signal_handler);
    signal(SIGQUIT, session_signal_handler);
    signal(SIGALRM, session_signal_handler);
}

/******************************************************************************
 *                                                                            *
 * Function: check_force_stop                                                 *
 *                                                                            *
 * Purpose: check the instructions on the forced stop                         *
 *                                                                            *
 * Return value: value of the forced stop flag                                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int check_force_stop(void)
{
    DB_RESULT    result;
    DB_ROW       row;
    int          force_stop;
    const char   *__function_name = "check_force_stop";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() session_id: %s inner_jobnet_main_id: %s",
               __function_name, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    /* get force stop of session management information */
    result = DBselect("select force_stop from ja_session_table"
                      " where session_id = '%s' and inner_jobnet_main_id = %s",
                      JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    if (NULL == (row = DBfetch(result))) {
        DBfree_result(result);
        return JA_SES_FORCE_STOP_KILL;
    }

    force_stop = atoi(row[0]);
    DBfree_result(result);

    return force_stop;
}

/******************************************************************************
 *                                                                            *
 * Function: kbd_callback                                                     *
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
static void kbd_callback(const char *name, int name_len, const char *instruction,
                         int instruction_len, int num_prompts,
                         const LIBSSH2_USERAUTH_KBDINT_PROMPT *prompts,
                         LIBSSH2_USERAUTH_KBDINT_RESPONSE *responses, void **abstract)
{
    (void)name;
    (void)name_len;
    (void)instruction;
    (void)instruction_len;

    if (num_prompts == 1) {
        responses[0].text   = zbx_strdup(NULL, password);
        responses[0].length = strlen(password);
    }

    (void)prompts;
    (void)abstract;
}

/******************************************************************************
 *                                                                            *
 * Function: waitsocket                                                       *
 *                                                                            *
 * Purpose: when the return value of the function of libssh2 is               *
 *          LIBSSH2_ERROR_EAGAIN, transmission and reception are waited       *
 *                                                                            *
 * Parameters: socket_fd (in) - socket file descriptor                        *
 *             session   (in) - ssh session instance                          *
 *                                                                            *
 * Return value: return value of the select function                          *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int waitsocket(int socket_fd, LIBSSH2_SESSION *session)
{
    struct timeval  tv;
    int             rc, dir;
    fd_set          fd, *writefd = NULL, *readfd = NULL;

    /* set the response wait time in seconds from the SSH host */
    tv.tv_sec  = 1;
    tv.tv_usec = 0;

    FD_ZERO(&fd);
    FD_SET(socket_fd, &fd);

    /* now make sure we wait in the correct direction */
    dir = libssh2_session_block_directions(session);

    if (0 != (dir & LIBSSH2_SESSION_BLOCK_INBOUND)) {
        readfd = &fd;
    }

    if (0 != (dir & LIBSSH2_SESSION_BLOCK_OUTBOUND)) {
        writefd = &fd;
    }

    rc = select(socket_fd + 1, readfd, writefd, NULL, &tv);

    return rc;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_channel_close                                                *
 *                                                                            *
 * Purpose: opening of resources and close the channel                        *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: exit code which is generated in the process running on the   *
 *               remote host                                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_channel_close(const zbx_uint64_t inner_job_id)
{
    int             rc, exitcode, cnt, loop;
    char            *ssherr;
    const char      *__function_name = "ssh_channel_close";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    exitcode = 0;

    if (channel == NULL) {
        return exitcode;
    }

    /* channel close */
    cnt       = 0;
    loop      = 0;
    last_time = 0;
    while (0 == loop) {
        rc = libssh2_channel_close(channel);
        switch (rc) {
            case 0:
                 loop = 1;
                 break;

            case LIBSSH2_ERROR_EAGAIN:
                 now_time = time(NULL);
                 if (last_time != now_time) {
                     cnt = cnt + 1;
                     if (cnt > CONFIG_TIMEOUT) {
                         loop = 1;
                         break;
                     }
                     last_time = now_time;
                 }
                 waitsocket(sock.socket, session);
                 continue;

            default:
                 libssh2_session_last_error(session, &ssherr, NULL, 0);
                 ja_log("JASESSION300001", 0, NULL, 0, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
                 loop = 1;
                 break;
        }
    }

    if (rc == 0) {
        exitcode = libssh2_channel_get_exit_status(channel);
    }

    libssh2_channel_free(channel);
    channel = NULL;

    zabbix_log(LOG_LEVEL_DEBUG, "%s() SSH return value: %d", __function_name, exitcode);

    return exitcode;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_close                                                        *
 *                                                                            *
 * Purpose: do post-processing of the session error                           *
 *                                                                            *
 * Parameters: level        (in) - level of post-processing                   *
 *                                  1: tcp close                              *
 *                                  2: session free                           *
 *                                  3: session close                          *
 *             inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_close(const int level, const zbx_uint64_t inner_job_id)
{
    const char *disconnect_msg  = "Normal Shutdown";
    const char *__function_name = "ssh_close";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() level: %d inner_job_id: " ZBX_FS_UI64, __function_name, level, inner_job_id);

    switch (level) {
        case 3:  /* session close */
             if (session != NULL) {
                 libssh2_session_disconnect(session, disconnect_msg);
             }

        case 2:  /* session free */
             if (session != NULL) {
                 libssh2_session_free(session);
                 session = NULL;
             }

        case 1:  /* tcp close */
             if (sock.socket != 0) {
                 zbx_tcp_close(&sock);
             }
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_read                                                         *
 *                                                                            *
 * Purpose: read the standard output data                                     *
 *                                                                            *
 * Parameters: inner_job_id (in)     - inner job id                           *
 *             std_out      (in/out) - standard output data storage pointer   *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_read(const zbx_uint64_t inner_job_id, char **std_out)
{
    size_t          sz;
    int             rc, count;
    char            *ssherr;
    char            buffer[JA_STD_OUT_LEN], buf[2048];
    const char      *__function_name = "ssh_read";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    memset(buffer, '\0', sizeof(buffer));
    count = 0;
    while (1) {
        /* loop until we block */
        do {
            if (0 < (rc = libssh2_channel_read(channel, buf, sizeof(buf)))) {
                sz = (size_t)rc;
                if (sz > JA_STD_OUT_LEN - (count + 1)) {
                    sz = JA_STD_OUT_LEN - (count + 1);
                }
                if (0 == sz) {
                    continue;
                }
                memcpy(buffer + count, buf, sz);
                count = count + sz;
                zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDOUT) Response data: (%d) [%s]", __function_name, rc, buffer);
            }
            if (rc == 0) {
                zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDOUT) Response data ZERO (%d)", __function_name, rc);
            }

            /* receive complete character check */
            if (rc > 0 && run_mode == JA_RUN_MODE_INTERACTIVE) {
                if (NULL != strstr(buffer, prompt_string)) {
                    rc = 0;
                }
            }
        } while (rc > 0);

        /* this is due to blocking that would occur otherwise so we loop on this condition */
        if (rc == LIBSSH2_ERROR_EAGAIN) {
            /* icon forced stop check */
            now_time = time(NULL);
            if (last_time != now_time) {
                if (JA_SES_FORCE_STOP_OFF != check_force_stop()) {
                    ja_log("JASESSION300003", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
                    return FAIL;
                }
                last_time = now_time;
            }
            waitsocket(sock.socket, session);
            zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDOUT) Response data being received. length: %d", __function_name, count);
            continue;
        }

        /* reading failure ? */
        if (rc < 0) {
            libssh2_session_last_error(session, &ssherr, NULL, 0);
            ja_log("JASESSION200019", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
            return FAIL;
        }
        break;
    }

    buffer[count] = '\0';
    zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDOUT) Last response data (%d) [%s]", __function_name, count, buffer);

    /* convert character code */
    *std_out = convert_to_utf8(buffer, count, character_code);

    zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDOUT) Conversion data (%d) [%s]", __function_name, strlen(*std_out), *std_out);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_read_stderr                                                  *
 *                                                                            *
 * Purpose: read the standard error data                                      *
 *                                                                            *
 * Parameters: inner_job_id (in)     - inner job id                           *
 *             std_err      (in/out) - standard error data storage pointer    *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_read_stderr(const zbx_uint64_t inner_job_id, char **std_err)
{
    size_t          sz;
    int             rc, count;
    char            *ssherr;
    char            buffer[JA_STD_OUT_LEN], buf[2048];
    const char      *__function_name = "ssh_read_stderr";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    memset(buffer, '\0', sizeof(buffer));
    count = 0;
    while (1) {
        /* loop until we block */
        do {
            if (0 < (rc = libssh2_channel_read_stderr(channel, buf, sizeof(buf)))) {
                sz = (size_t)rc;
                if (sz > JA_STD_OUT_LEN - (count + 1)) {
                    sz = JA_STD_OUT_LEN - (count + 1);
                }
                memcpy(buffer + count, buf, sz);
                count = count + sz;
                zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDERR) Response data: (%d) [%s]", __function_name, rc, buffer);
            }
            if (rc == 0) {
                zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDERR) Response data ZERO (%d)", __function_name, rc);
            }
        } while (rc > 0);

        /* this is due to blocking that would occur otherwise so we loop on this condition */
        if (rc == LIBSSH2_ERROR_EAGAIN) {
            /* icon forced stop check */
            now_time = time(NULL);
            if (last_time != now_time) {
                if (JA_SES_FORCE_STOP_OFF != check_force_stop()) {
                    ja_log("JASESSION300003", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
                    return FAIL;
                }
                last_time = now_time;
            }
            waitsocket(sock.socket, session);
            zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDERR) Response data being received. length: %d", __function_name, count);
            continue;
        }

        /* reading failure ? */
        if (rc < 0) {
            libssh2_session_last_error(session, &ssherr, NULL, 0);
            ja_log("JASESSION200019", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
            return FAIL;
        }
        break;
    }

    buffer[count] = '\0';
    zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDERR) Last response data (%d) [%s]", __function_name, count, buffer);

    /* convert character code */
    *std_err = convert_to_utf8(buffer, count, character_code);

    zabbix_log(LOG_LEVEL_DEBUG, "%s() (STDERR) Conversion data (%d) [%s]", __function_name, strlen(*std_err), *std_err);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_connect                                                      *
 *                                                                            *
 * Purpose: SSH connection is made and login attestation after connection is  *
 *          performed.                                                        *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_connect(const zbx_uint64_t inner_job_id)
{
    int             auth_pw1 = 0, auth_pw2 = 0, auth_pw3 = 0, rc;
    char            *userauthlist, *ssherr, *std_out;
    const char      *__function_name = "ssh_connect";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    std_out = NULL;

    /* get host name */
    if (FAIL == ja_host_getname(inner_job_id, host_flag, host_name, host)) {
        ja_log("JASESSION200005", inner_jobnet_id, NULL, inner_job_id, __function_name, host_name, host_flag, inner_job_id);
        return FAIL;
    }

    /* connect to SSH server */
    if (FAIL == ja_connect_to_port(&sock, host, inner_job_id, JA_TXN_OFF)) {
        return FAIL;
    }

    /* stop the alarm timer */
    alarm(0);

    /* initializes an SSH session object */
    if (NULL == (session = libssh2_session_init())) {
        ja_log("JASESSION200006", inner_jobnet_id, NULL, inner_job_id, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
        ssh_close(1, inner_job_id);
        return FAIL;
    }

    /* set non-blocking mode on session */
    libssh2_session_set_blocking(session, 0);

    /* Create a session instance and start it up. This will trade welcome */
    /* banners, exchange keys, and setup crypto, compression, and MAC layers */
    while (0 != (rc = libssh2_session_startup(session, sock.socket))) {
        switch (rc) {
            case LIBSSH2_ERROR_EAGAIN:
                 waitsocket(sock.socket, session);
                 continue;

            default:
                 libssh2_session_last_error(session, &ssherr, NULL, 0);
                 ja_log("JASESSION200007", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                 ssh_close(2, inner_job_id);
                 return FAIL;
        }
    }

    /* check what authentication methods are available */
    while (NULL == (userauthlist = libssh2_userauth_list(session, login_user, strlen(login_user)))) {
        rc = libssh2_session_last_error(session, &ssherr, NULL, 0);
        switch (rc) {
            case LIBSSH2_ERROR_EAGAIN:
                 waitsocket(sock.socket, session);
                 continue;

            default:
                 ja_log("JASESSION200008", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                 ssh_close(3, inner_job_id);
                 return FAIL;
        }
    }

    /* check the User authentication list */
    if (NULL != strstr(userauthlist, "password")) {
        auth_pw1 = 1;
    }
    if (NULL != strstr(userauthlist, "keyboard-interactive")) {
        auth_pw2 = 1;
    }
    if (NULL != strstr(userauthlist, "publickey")) {
        auth_pw3 = 1;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "%s() supported authentication methods: [%s] password: [%d] keyboard-interactive: [%d] publickey: [%d]", __function_name, userauthlist, auth_pw1, auth_pw2, auth_pw3);

    /* do user authentication */
    switch (auth_method) {
        case JA_SES_AUTH_PASSWORD:
             /* check the authentication methods supported */
             if (auth_pw1 == 0 && auth_pw2 == 0) {
                 ja_log("JASESSION200009", inner_jobnet_id, NULL, inner_job_id, __function_name, userauthlist, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                 ssh_close(3, inner_job_id);
                 return FAIL;
             }

             if (auth_pw1 == 1) {
                 /* we could authenticate via password */
                 while (0 != (rc = libssh2_userauth_password(session, login_user, login_password))) {
                     switch (rc) {
                         case LIBSSH2_ERROR_EAGAIN:
                              waitsocket(sock.socket, session);
                              continue;

                         default:
                              libssh2_session_last_error(session, &ssherr, NULL, 0);
                              ja_log("JASESSION200010", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                              ssh_close(3, inner_job_id);
                              return FAIL;
                     }
                 }
                 zabbix_log(LOG_LEVEL_DEBUG, "%s() password authentication succeeded", __function_name);
             }
             else {
                 /* or via keyboard-interactive */
                 password = login_password;
                 while (0 != (rc = libssh2_userauth_keyboard_interactive(session, login_user, &kbd_callback))) {
                     switch (rc) {
                         case LIBSSH2_ERROR_EAGAIN:
                              waitsocket(sock.socket, session);
                              continue;

                         default:
                              libssh2_session_last_error(session, &ssherr, NULL, 0);
                              ja_log("JASESSION200011", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                              ssh_close(3, inner_job_id);
                              return FAIL;
                     }
                 }
                 zabbix_log(LOG_LEVEL_DEBUG, "%s() keyboard-interactive authentication succeeded", __function_name);
             }
             break;

        case JA_SES_AUTH_PUBLICKEY:
             /* public key */
             /* check the authentication methods supported */
             if (auth_pw3 == 0) {
                 ja_log("JASESSION200009", inner_jobnet_id, NULL, inner_job_id, __function_name, userauthlist, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                 ssh_close(3, inner_job_id);
                 return FAIL;
             }

             /* public key file check */
             if (SUCCEED != zbx_is_regular_file(public_key)) {
                 ja_log("JASESSION200012", inner_jobnet_id, NULL, inner_job_id, __function_name, public_key, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                 ssh_close(3, inner_job_id);
                 return FAIL;
             }

             /* private key file check */
             if (SUCCEED != zbx_is_regular_file(private_key)) {
                 ja_log("JASESSION200013", inner_jobnet_id, NULL, inner_job_id, __function_name, private_key, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                 ssh_close(3, inner_job_id);
                 return FAIL;
             }

             /* do public key authentication */
             while (0 != (rc = libssh2_userauth_publickey_fromfile(session, login_user, public_key, private_key, passphrase))) {
                 switch (rc) {
                     case LIBSSH2_ERROR_EAGAIN:
                          waitsocket(sock.socket, session);
                          continue;

                     default:
                          libssh2_session_last_error(session, &ssherr, NULL, 0);
                          ja_log("JASESSION200014", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, public_key, private_key, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                          ssh_close(3, inner_job_id);
                          return FAIL;
                 }
             }
             zabbix_log(LOG_LEVEL_DEBUG, "%s() authentication by public key succeeded", __function_name);
             break;

        default:
             ja_log("JASESSION200015", 0, NULL, inner_job_id, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id, auth_method);
             ssh_close(3, inner_job_id);
             return FAIL;
    }

    if (run_mode == JA_RUN_MODE_INTERACTIVE) {
        /* opening of session */
        while (NULL == (channel = libssh2_channel_open_session(session))) {
            rc = libssh2_session_last_error(session, &ssherr, NULL, 0);
            switch (rc) {
                /* marked for non-blocking I/O but the call would block. */
                case LIBSSH2_ERROR_EAGAIN:
                     waitsocket(sock.socket, session);
                     continue;

                default:
                     ja_log("JASESSION200016", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                     ssh_close(3, inner_job_id);
                     return FAIL;
            }
        }

        /* request a terminal with 'vanilla (default)' terminal emulation */
        zabbix_log(LOG_LEVEL_DEBUG, "%s() run mode is INTERACTIVE (shell request)", __function_name);
        while (0 != (rc = libssh2_channel_request_pty(channel, terminal_type))) {
            switch (rc) {
                case LIBSSH2_ERROR_EAGAIN:
                     waitsocket(sock.socket, session);
                     continue;

                default:
                     ja_log("JASESSION200020", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                     ssh_close(3, inner_job_id);
                     return FAIL;
            }
        }

        /* open a shell on that pty */
        while (0 != (rc = libssh2_channel_shell(channel))) {
            switch (rc) {
                case LIBSSH2_ERROR_EAGAIN:
                     waitsocket(sock.socket, session);
                     continue;

                default:
                     ja_log("JASESSION200021", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                     ssh_close(3, inner_job_id);
                     return FAIL;
            }
        }

        /* skip prompt from the terminal */
        if (FAIL == ssh_read(inner_job_id, &std_out)) {
            ssh_close(3, inner_job_id);
            return FAIL;
        }
        zbx_free(std_out);
    }

    /* icon forced stop check */
    if (JA_SES_FORCE_STOP_OFF != check_force_stop()) {
        ja_log("JASESSION300003", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        zabbix_log(LOG_LEVEL_DEBUG, "%s() exit", __function_name);
        return FAIL;
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: ssh_exec                                                         *
 *                                                                            *
 * Purpose: perform command execution and the establishment of channel        *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int ssh_exec(const zbx_uint64_t inner_job_id)
{
    int             rc, len, wlen, exitcode;
    char            *ssherr;
    char            *send_data, *std_out, *std_err, *return_code;
    const char      *__function_name = "ssh_exec";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    std_out = NULL;
    std_err = NULL;

    /* no transmit data or test mode on ? */
    if (strlen(command) == 0 || JOBARG_TEST_FLAG == JA_JOB_TEST_FLAG_ON) {
        ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", "");
        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", "");
        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", "");
        return SUCCEED;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "%s() before command data (%d) [%s]", __function_name, strlen(command), command);

    /* conversion of line feed */
    switch (line_feed_code) {
        case JA_LINE_FEED_CODE_LF:  /* CRLF => LF */
             dos2unix(command);
             break;

        case JA_LINE_FEED_CODE_CR:  /* CRLF => CR */
             dos2cr(command);
             break;
    }

    /* convert character code */
    send_data = convert_from_utf8(command, strlen(command), character_code);

    if (run_mode == JA_RUN_MODE_INTERACTIVE) {
        zabbix_log(LOG_LEVEL_DEBUG, "%s() run mode is INTERACTIVE", __function_name);
        /* command transmission */
        len  = strlen(send_data);
        wlen = 0;
        do {
            rc = libssh2_channel_write(channel, send_data, len);
            if (rc == LIBSSH2_ERROR_EAGAIN) {
                waitsocket(sock.socket, session);
                continue;
            }

            if (rc < 0) {
                ja_log("JASESSION200022", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                zbx_free(send_data);
                return FAIL;
            }
            wlen = wlen + rc;
        } while (wlen < len);

        zabbix_log(LOG_LEVEL_DEBUG, "%s() command transmission completion (%d) [%s]", __function_name, strlen(send_data), send_data);
        zbx_free(send_data);

        /* read the standard output data */
        if (FAIL == ssh_read(inner_job_id, &std_out)) {
            return FAIL;
        }

        /* set the return value to the job controller variable */
        ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", "");
        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", std_out);
        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", "");
        zbx_free(std_out);
    }
    else {
        /* request a shell on a channel and execute command */
        zabbix_log(LOG_LEVEL_DEBUG, "%s() run mode is NON INTERACTIVE (EXEC)", __function_name);
        /* opening of session */
        while (NULL == (channel = libssh2_channel_open_session(session))) {
            rc = libssh2_session_last_error(session, &ssherr, NULL, 0);
            switch (rc) {
                /* marked for non-blocking I/O but the call would block. */
                case LIBSSH2_ERROR_EAGAIN:
                     waitsocket(sock.socket, session);
                     continue;

                default:
                     ja_log("JASESSION200016", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, ssherr, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                     return FAIL;
            }
        }

        while (0 != (rc = libssh2_channel_exec(channel, send_data))) {
            switch (rc) {
                case LIBSSH2_ERROR_EAGAIN:
                     waitsocket(sock.socket, session);
                     continue;

                default:
                     ja_log("JASESSION200018", inner_jobnet_id, NULL, inner_job_id, __function_name, rc, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, host_name, inner_job_id);
                     zbx_free(send_data);
                     return FAIL;
            }
        }

        zabbix_log(LOG_LEVEL_DEBUG, "%s() command transmission completion (%d) [%s]", __function_name, strlen(send_data), send_data);
        zbx_free(send_data);

        /* read the standard output data */
        if (FAIL == ssh_read(inner_job_id, &std_out)) {
            return FAIL;
        }

        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_OUT", std_out);
        zbx_free(std_out);

        /* read the standard error data */
        if (FAIL == ssh_read_stderr(inner_job_id, &std_err)) {
            return FAIL;
        }

        ja_set_value_after(inner_job_id, inner_jobnet_id, "STD_ERR", std_err);
        zbx_free(std_err);

        /* channel close */
        exitcode = ssh_channel_close(inner_job_id);

        /* set the return value to the job controller variable */
        return_code = zbx_dsprintf(NULL, "%d", exitcode);
        ja_set_value_after(inner_job_id, inner_jobnet_id, "JOB_EXIT_CD", return_code);

        /* check of match the job stop code */
        if (0 != ja_number_match(return_code, stop_code)) {
            icon_status = 1;
            ja_log("JASESSION300004", 0, NULL, inner_job_id, return_code, stop_code, inner_job_id);
            zbx_free(return_code);
            return FAIL;
        }
        zbx_free(return_code);
    }

    /* icon forced stop check */
    if (JA_SES_FORCE_STOP_OFF != check_force_stop()) {
        ja_log("JASESSION300003", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        return FAIL;
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: get_agentless_info                                               *
 *                                                                            *
 * Purpose: get the set value of the agentless icon                           *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int get_agentless_info(const zbx_uint64_t inner_job_id)
{
    DB_RESULT       result;
    DB_ROW          row;
    char            *prompt;
    const char      *__function_name = "get_agentless_info";
    //Park.iggy Add
    char	d_passwd[JA_MAX_STRING_LEN];
    char	d_dec[256];
    char	passwd[256];
    char   d_num[256];
    char	*d_flag="1";
    char	d_catX16[5];
    char   d_x16[3];
    char   dec[256];
    char   *key = "199907";
    char	*searchChar;
    int     i,j,k,kk,x16toX10,splitLen;
    //Park.iggy END

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* get information of agentless icon */
    result = DBselect("select inner_jobnet_id, host_flag, connection_method, session_flag, auth_method, run_mode,"
                      " line_feed_code, timeout, session_id, login_user, login_password, public_key, private_key,"
                      " passphrase, host_name, stop_code, terminal_type, character_code, prompt_string, command"
                      " from ja_run_icon_agentless_table"
                      " where inner_job_id = " ZBX_FS_UI64, inner_job_id);

    if (NULL == (row = DBfetch(result))) {
        ja_log("JASESSION200004", 0, NULL, inner_job_id, __function_name, inner_job_id);
        DBfree_result(result);
        return FAIL;
    }

    ZBX_STR2UINT64(inner_jobnet_id, row[0]);
    session_flag = atoi(row[3]);
    zbx_strlcpy(session_id, row[8], sizeof(session_id));

    switch (operation_flag) {
        case JA_SES_OPERATION_FLAG_ONETIME:
        case JA_SES_OPERATION_FLAG_CONNECT:
             host_flag         = atoi(row[1]);
             connection_method = atoi(row[2]);
             auth_method       = atoi(row[4]);
             run_mode          = atoi(row[5]);
             line_feed_code    = atoi(row[6]);

             if (SUCCEED != DBis_null(row[9])) {
                 if (FAIL == ja_cpy_value(inner_job_id, row[9], login_user)) {
                     ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, row[9], JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                     DBfree_result(result);
                     return FAIL;
                 }
             }

             if (SUCCEED != DBis_null(row[10])) {
            	 //Park.iggy Add
            	 zbx_strlcpy(d_passwd, row[10], sizeof(d_passwd));
            	 if(d_flag[0] == d_passwd[0] && strlen(d_passwd) > 0 ) {
            		 j=0;
            		 k=0;
            		 for(kk = 1; kk < strlen(d_passwd) ; kk++){
            			 if((kk%2) != 0){
            				 d_x16[0] = d_passwd[kk];
            			 }else{
            				 d_x16[1] = d_passwd[kk];
            				 d_x16[2] = '\0';
            				 zbx_snprintf(d_catX16,   sizeof(d_catX16),   "0x%s", d_x16);
            				 x16toX10 = (unsigned long)strtol(d_catX16,NULL,0);
            				 *d_x16 = '\0';
            				 *d_catX16 = '\0';
            				 d_dec[k] = (char)(x16toX10) ;
            				 dec[k] = (char)(d_dec[k]^key[j]);
            				 j++;
            				 k++;
            				 if (j == strlen(key)) j =0;
   		 				}
            		 }
            		 dec[k]='\0';
            		 searchChar = strchr(dec,'|');
            		 if ( searchChar == NULL )
            			 zbx_strlcpy(passwd, d_passwd, sizeof(passwd));
            	 	 else{
            	 		 splitLen = strlen(dec) - strlen(searchChar) ;
            	 		 for(i=0; i < splitLen;i++){
            	 			d_num[i] = dec[i];
            	 		 }
            	 		 d_num[i] = '\0';

            	 		 for(i=0; i < strlen(d_num); i++){
            	 			 if ( !isdigit(d_num[i])){
            	 				 i = 0;
            	 			     break;
            	 			 }
            	 		 }
            	 		zabbix_log(LOG_LEVEL_DEBUG,"Change dec[%s]: , searchChar:[%s] strlen(dec)=[%d] strlen(searchChar)=[%d] lenth[%d]",
            	 				dec, searchChar,  strlen(dec) ,strlen(searchChar) ,splitLen);

            	 		 if( i != 0){
            	 			 if ( atoi(d_num) == (strlen(searchChar)-1)) {
            	 				 for(i=0; i < atoi(d_num) ;i++ ){
            	 					passwd[i] = searchChar[i+1];
            	 				 }
            	 				 passwd[i] = '\0';
            	 			 }else{
            	 				 i = 0;
            	 			 }
            	 		 }
            	 		 if ( i == 0 )
            	 			 zbx_strlcpy(passwd, d_passwd, sizeof(passwd));

            	 	 }

            	 }else{
            		 zbx_strlcpy(passwd, d_passwd, sizeof(passwd));
            	 }
            	 zabbix_log(LOG_LEVEL_DEBUG,
            	                "SSH passwd[%s]: , d_passwd:[%s]", passwd, d_passwd);
            	 //Park.iggy END
                 //if (FAIL == ja_cpy_value(inner_job_id, row[10], login_password)) {
            	 if (FAIL == ja_cpy_value(inner_job_id, passwd, login_password)) {
                     //ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, row[10], JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
            		 ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, passwd, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                     DBfree_result(result);
                     return FAIL;
                 }
             }

             if (SUCCEED != DBis_null(row[11])) {
                 if (FAIL == ja_cpy_value(inner_job_id, row[11], public_key)) {
                     ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, row[11], JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                     DBfree_result(result);
                     return FAIL;
                 }
             }

             if (SUCCEED != DBis_null(row[12])) {
                 if (FAIL == ja_cpy_value(inner_job_id, row[12], private_key)) {
                     ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, row[12], JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                     DBfree_result(result);
                     return FAIL;
                 }
             }

             if (SUCCEED != DBis_null(row[13])) {
                 if (FAIL == ja_cpy_value(inner_job_id, row[13], passphrase)) {
                     ja_log("JASESSION200017", 0, NULL, inner_job_id, __function_name, row[13], JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id);
                     DBfree_result(result);
                     return FAIL;
                 }
             }

             if (SUCCEED != DBis_null(row[14])) {
                 zbx_strlcpy(host_name, row[14], sizeof(host_name));
             }

             if (SUCCEED != DBis_null(row[16])) {
                 zbx_strlcpy(terminal_type, row[16], sizeof(terminal_type));
             }
             else {
                 zbx_strlcpy(terminal_type, "vanilla", sizeof(terminal_type));
             }

             if (SUCCEED != DBis_null(row[17])) {
                 zbx_strlcpy(character_code, row[17], sizeof(character_code));
             }


        case JA_SES_OPERATION_FLAG_CONTINUE:
             timeout = atoi(row[7]);

             if (SUCCEED != DBis_null(row[15])) {
                 zbx_strlcpy(stop_code, row[15], sizeof(stop_code));
             }

             if (SUCCEED != DBis_null(row[18])) {
                 zbx_strlcpy(prompt_string, row[18], sizeof(prompt_string));
                 prompt = convert_from_utf8(prompt_string, strlen(prompt_string), character_code);
                 zbx_strlcpy(prompt_string, prompt, sizeof(prompt_string));
                 zbx_free(prompt);
             }

             if (SUCCEED != DBis_null(row[19])) {
                 if (FAIL == ja_replace_variable(inner_job_id, row[19], command, sizeof(command))) {
                     DBfree_result(result);
                     return FAIL;
                 }
             }
    }

    DBfree_result(result);
    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: session_onetime                                                  *
 *                                                                            *
 * Purpose: transmits the connection and command by the one-time session      *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int session_onetime(const zbx_uint64_t inner_job_id)
{
    const char      *__function_name = "session_onetime";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* read agentless icon information */
    if (FAIL == get_agentless_info(inner_job_id)) {
        return FAIL;
    }

    /* login and connect to the SSH host */
    if (FAIL == ssh_connect(inner_job_id)) {
        return FAIL;
    }

    /* send command to SSH host */
    if (FAIL == ssh_exec(inner_job_id)) {
        ssh_close(3, inner_job_id);
        return FAIL;
    }

    /* channel close */
    if (run_mode == JA_RUN_MODE_INTERACTIVE) {
        ssh_channel_close(inner_job_id);
    }

    /* ssh session termination */
    ssh_close(3, inner_job_id);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: session_connect                                                  *
 *                                                                            *
 * Purpose: rexecution of the command and connection to the host              *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int session_connect(const zbx_uint64_t inner_job_id)
{
    const char      *__function_name = "session_connect";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* read agentless icon information */
    if (FAIL == get_agentless_info(inner_job_id)) {
        return FAIL;
    }

    /* login and connect to the SSH host */
    if (FAIL == ssh_connect(inner_job_id)) {
        return FAIL;
    }

    /* send command to SSH host */
    if (FAIL == ssh_exec(inner_job_id)) {
        ssh_close(3, inner_job_id);
        return FAIL;
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: session_continue                                                 *
 *                                                                            *
 * Purpose: command execution to the host                                     *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int session_continue(const zbx_uint64_t inner_job_id)
{
    int          rc, port;
    char         host_ip[128];
    const char   *__function_name = "session_continue";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* read agentless icon information */
    if (FAIL == get_agentless_info(inner_job_id)) {
        return FAIL;
    }

    /* check enable host */
    rc = ja_host_getip(host, host_ip, inner_job_id, &port, JA_TXN_OFF);
    if (rc == 0) {
        return FAIL;
    }

    /* send command to SSH host */
    if (FAIL == ssh_exec(inner_job_id)) {
        return FAIL;
    }

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: session_close                                                    *
 *                                                                            *
 * Purpose: close the session with the host                                   *
 *                                                                            *
 * Parameters: inner_job_id (in) - inner job id                               *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int session_close(const zbx_uint64_t inner_job_id)
{
    int          rc, port;
    char         host_ip[128];
    const char   *__function_name = "session_close";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() inner_job_id: " ZBX_FS_UI64, __function_name, inner_job_id);

    /* check enable host */
    rc = ja_host_getip(host, host_ip, inner_job_id, &port, JA_TXN_OFF);
    if (rc == 0) {
        return FAIL;
    }

    /* channel close */
    if (run_mode == JA_RUN_MODE_INTERACTIVE) {
        ssh_channel_close(inner_job_id);
    }

    /* close the session to SSH host */
    ssh_close(3, inner_job_id);

    return SUCCEED;
}

/******************************************************************************
 *                                                                            *
 * Function: update_session                                                   *
 *                                                                            *
 * Purpose: Change the status of finished session                             *
 *                                                                            *
 * Return value: LOOP_CONTINUE    - continue processing                       *
 *               LOOP_EXIT        - end the process                           *
 *               TRANSACTION_FAIL - restart the transaction                   *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int update_session(int rtn, zbx_uint64_t inner_job_id) {
    DB_RESULT    result;
    DB_ROW       row;
    int          rc;
    int          next = LOOP_EXIT;
    const char   *__function_name = "update_session";

    if ((operation_flag == JA_SES_OPERATION_FLAG_ONETIME) ||
        (operation_flag == JA_SES_OPERATION_FLAG_CONNECT && rtn == FAIL) ||
        (operation_flag == JA_SES_OPERATION_FLAG_CLOSE   && rtn == SUCCEED)) {
        /* do stop process */
        rc = DBexecute("delete from ja_session_table"
                       " where session_id = '%s' and inner_jobnet_main_id = %s",
                       JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

        if (rc < ZBX_DB_OK) {
            return TRANSACTION_FAIL;
        }
        next = LOOP_EXIT;
    }
    else {
        /* status is updated to end */
        result = DBselect("select inner_job_id from ja_session_table"
                          " where session_id = '%s' and inner_jobnet_main_id = %s for update",
                          JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

        if (NULL != (row = DBfetch(result))) {
            rc = DBexecute("update ja_session_table set status = %d"
                           " where session_id = '%s' and inner_jobnet_main_id = %s",
                           JA_SES_STATUS_END, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

            if (rc <= ZBX_DB_OK) {
                
                if(ja_set_runerr(inner_job_id, 2) == FAIL) {
                    return TRANSACTION_FAIL;
                }
                
                ja_log("JASESSION200001", 0, NULL, inner_job_id, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
                ssh_close(3, inner_job_id);
                DBfree_result(result);
                return LOOP_EXIT;
            }
            next = LOOP_CONTINUE;
        }
        else {
            rtn  = FAIL;
            next = LOOP_EXIT; 
        }
        DBfree_result(result);
    }

    if (rtn == SUCCEED) {
        /* icon normal end */
        if(ja_flow(inner_job_id, JA_FLOW_TYPE_NORMAL, 1) == FAIL) {
            next = TRANSACTION_FAIL;
        }
    }
    else {
        /* icon error stop */
        if(ja_set_runerr(inner_job_id, 2) == FAIL) {
            next = TRANSACTION_FAIL;
        } else {
            ja_log("JASESSION300002", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
            ssh_close(3, inner_job_id);
        }
    }
    
    return next;
}

/******************************************************************************
 *                                                                            *
 * Function: process_session                                                  *
 *                                                                            *
 * Purpose: reads the session management table, and then disconnects or       *
 *          command transmission and connection to the host.                  *
 *                                                                            *
 * Return value: LOOP_CONTINUE - continue processing                          *
 *               LOOP_EXIT     - end the process                              *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
static int process_session(void)
{
    DB_RESULT    result;
    DB_ROW       row;
    int          status, force_stop, rc;
    int          rtn, next = LOOP_EXIT;
    zbx_uint64_t inner_job_id;
    const char   *__function_name = "process_session";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() session_id: %s inner_jobnet_main_id: %s test_flag: %d",
               __function_name, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER, JOBARG_TEST_FLAG);

    /* get information of session management information */
    result = DBselect("select inner_job_id, operation_flag, status, force_stop"
                      " from ja_session_table"
                      " where session_id = '%s' and inner_jobnet_main_id = %s",
                      JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    if(result == NULL) {
        ja_log("JASESSION200024", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        return LOOP_CONTINUE;
    }

    if (NULL == (row = DBfetch(result))) {
        ja_log("JASESSION300002", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        ssh_close(3, inner_job_id);
        DBfree_result(result);
        return LOOP_EXIT;
    }

    ZBX_STR2UINT64(inner_job_id, row[0]);
    operation_flag = atoi(row[1]);
    status         = atoi(row[2]);
    force_stop     = atoi(row[3]);
    DBfree_result(result);

    if (status != JA_SES_STATUS_BEGIN) {
        return LOOP_CONTINUE;
    }

    switch (operation_flag) {
        case JA_SES_OPERATION_FLAG_ONETIME:
             rtn = session_onetime(inner_job_id);
             break;

        case JA_SES_OPERATION_FLAG_CONNECT:
             rtn = session_connect(inner_job_id);
             break;

        case JA_SES_OPERATION_FLAG_CONTINUE:
             rtn = session_continue(inner_job_id);
             break;

        case JA_SES_OPERATION_FLAG_CLOSE:
             rtn = session_close(inner_job_id);
             break;

        default:
             ja_log("JASESSION200002", 0, NULL, inner_job_id, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID, inner_job_id, operation_flag);
             ssh_close(3, inner_job_id);
             ja_set_runerr(inner_job_id, 2);
             return LOOP_EXIT;
    }

    while(1) {
        DBbegin();
        next = update_session(rtn, inner_job_id);

        if(next == TRANSACTION_FAIL) {
            ja_log("JASESSION200023", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
            DBrollback();
            zbx_sleep(3);
        } else
            break;
    }

    DBcommit();
    return next;
}


/******************************************************************************
 *                                                                            *
 * Function: main                                                             *
 *                                                                            *
 * Purpose: transmits the connection and command to the host by telnet or ssh *
 *                                                                            *
 * Parameters: argc (in) - number of parameters                               *
 *             argv (in) - Parameter values                                   *
 *                                                                            *
 * Return value: SUCCEED - normal end                                         *
 *               FAIL    - an error occurred                                  *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int main(int argc, char **argv)
{
    DB_RESULT    result;
    DB_ROW       row;
    int          rc;
    const char   *__function_name = "main";

    progname = get_program_name(argv[0]);
    parse_commandline(argc, argv);
    ja_load_config(CONFIG_FILE);
    if (strcmp(CONFIG_LOG_TYPE_STR, ZBX_OPTION_LOGTYPE_SYSTEM) == 0 ||NULL == CONFIG_LOG_FILE || '\0' == *CONFIG_LOG_FILE){
		zabbix_open_log(LOG_TYPE_SYSLOG, CONFIG_LOG_LEVEL, NULL);
    }
	else{
		zabbix_open_log(LOG_TYPE_FILELOG, CONFIG_LOG_LEVEL, CONFIG_LOG_FILE);
    }
    ja_log("JASESSION000001", 0, NULL, 0, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER, JOBARG_TEST_FLAG);
    init_agentless_info();

    DBconnect(ZBX_DB_CONNECT_NORMAL);

    /* set the PID of the process itself */
    DBbegin();
    result = DBselect("select inner_job_id from ja_session_table"
                      " where session_id = '%s' and inner_jobnet_main_id = %s for update",
                      JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    if (NULL == (row = DBfetch(result))) {
        DBfree_result(result);
        ja_log("JASESSION300002", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        ja_log("JASESSION000002", 0, NULL, 0, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);
        DBrollback();
        DBclose();
        exit(SUCCEED);
    }

    rc = DBexecute("update ja_session_table set pid = %d"
                   " where session_id = '%s' and inner_jobnet_main_id = %s",
                   (int)getpid(), JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    if (rc <= ZBX_DB_OK) {
        DBfree_result(result);
        ja_log("JASESSION200001", 0, NULL, 0, __function_name, JOBARG_REGISTRY_NUMBER, JOBARG_SESSION_ID);
        ja_log("JASESSION000002", 0, NULL, 0, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);
        DBrollback();
        DBclose();
        exit(SUCCEED);
    }
    DBfree_result(result);
    DBcommit();

    /* main loop */
    while (1) {
        zbx_setproctitle("session management");
        rc = process_session();
        if (rc == LOOP_EXIT) {
             break;
        }
        zbx_sleep(1);
    }

    DBclose();
    ja_log("JASESSION000002", 0, NULL, 0, JOBARG_SESSION_ID, JOBARG_REGISTRY_NUMBER);

    exit(SUCCEED);
}
