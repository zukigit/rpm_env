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

#include "threads.h"
#include "comms.h"
#include "log.h"
#include "zbxgetopt.h"
#include "zbxjson.h"

#include "jacommon.h"

#if defined(_WINDOWS)
#include "jastr.h"
#endif                          /* _WINDOWS */

#define JOBARG_DEFAULT_SERVER_PORT_STR  "10061"

/* long options */
static struct zbx_option longopts[] = {
    {"jobarranger-server", 1, NULL, 'z'},
    {"port", 1, NULL, 'p'},
    {"user-name", 1, NULL, 'U'},
    {"password", 1, NULL, 'P'},
    {"registry-number", 1, NULL, 'r'},
    {"variable-format", 0, NULL, 'e'},
    {"help", 0, NULL, 'h'},
    {"version", 0, NULL, 'V'},
    {NULL}
};

/* short options */
static char shortopts[] = "z:p:U:P:r:ehV";

/* end of COMMAND LINE OPTIONS */

static int CONFIG_LOG_LEVEL = LOG_LEVEL_WARNING;

static char *JOBARG_SOURCE_IP = NULL;
static char *JOBARG_SERVER = NULL;
unsigned short JOBARG_SERVER_PORT = 0;
unsigned short JOBARG_DEFAULT_SERVER_PORT = 10061;
static char *JOBARG_REGISTRY_NUMBER = NULL;
static char *JOBARG_USERNAME = NULL;
static char *JOBARG_PASSWORD = NULL;
static int JOBARG_VARIABLE_FORMAT = 0;

const char *progname = NULL;
const char title_message[] = "Job Arranger Jobnet status get";
const char usage_message[] =
    "[-hV] -z <server> [-p <port>] -U <user-name> -P <password> -r <registry-number> [-e]";

const char *help_message[] = {
    "Options:",
    "  -z --jobarranger-server <server>          Hostname or IP address of Job Arranger server",
    " [-p --port <port>]                         Specify port number of server trapper running on the server. Default is " JOBARG_DEFAULT_SERVER_PORT_STR,
    "  -U --user-name <user-name>                Specify user who has permission to reference the jobnet",
    "  -P --password <password>                  Specify user password",
    "  -r --registry-number <registry-number>    Specify the jobnet registration number to be referenced",
    " [-e --variable-format]                     Specify output in the environment variable format (with bash format)",
    "",
    "Other options:",
    "  -h --help                                 Give this help",
    "  -V --version                              Display version number",
    NULL                        /* end of text */
};

/* COMMAND LINE OPTIONS */

#if !defined(_WINDOWS)

static void send_signal_handler(int sig)
{
    if (SIGALRM == sig)
        zabbix_log(LOG_LEVEL_WARNING, "Timeout while executing operation");

    exit(FAIL);
}

#endif                          /* NOT _WINDOWS */

void help_jobarg()
{
    const char **p = help_message;

    usage();
    printf("\n");

    while (NULL != *p)
        printf("%s\n", *p++);
}

void version_jobarg()
{
    printf("%s v%s (revision %s) (%s)\n", title_message, JOBARG_VERSION, JOBARG_REVISION, JOBARG_REVDATE);
    printf("Compilation time: %s %s\n", __DATE__, __TIME__);
}

int reginum_check(const char *str)
{
    int i;

    for (i = 0; str[i]; i++) {
        if ((str[i] < '0') || (str[i] > '9')) {
            zabbix_log(LOG_LEVEL_ERR, "argument check error: %s", str);
            exit(FAIL);
        }
    }
    return SUCCEED;
}

static int check_response(char *response)
{
    struct zbx_json_parse jp, jp_row;
    char value[JA_STD_OUT_LEN];
    char *message;
    const char *p;
    int ret = FAIL;
    int result;
    int version;
    char *jobnetid = NULL;
    char *jobnetname = NULL;
    char *jobnetruntype = NULL;
    char *jobnetstatus = NULL;
    char *jobstatus = NULL;
    char *lastexitcd = NULL;
    char *laststdout = NULL;
    char *laststderr = NULL;
    zbx_uint64_t scheduled_time;
    zbx_uint64_t start_time;
    zbx_uint64_t end_time;
    int ch, jobnet_status = 0;

    if (SUCCEED == zbx_json_open(response, &jp)) {
        if (SUCCEED ==
            zbx_json_value_by_name(&jp, JA_PROTO_TAG_KIND, value,
                                   sizeof(value))) {
            if (0 != strcmp(value, JA_PROTO_VALUE_JOBNETSTATUSRQ_RES)) {
                zabbix_log(LOG_LEVEL_ERR,
                           "Received message error: [kind] is not [jobnetstatusrq-res]");
                return ret;
            }
        } else {
            zabbix_log(LOG_LEVEL_ERR,
                       "Received message error: [kind] not found");
            return ret;
        }

        if (SUCCEED ==
            zbx_json_value_by_name(&jp, JA_PROTO_TAG_VERSION, value,
                                   sizeof(value))) {
            version = atoi(value);
            if (version != JA_PROTO_VALUE_VERSION_1) {
                zabbix_log(LOG_LEVEL_ERR,
                           "Received message error: [version] is not [%d]",
                           JA_PROTO_VALUE_VERSION_1);
                return ret;
            }
        } else {
            zabbix_log(LOG_LEVEL_ERR,
                       "Received message error: [version] not found");
            return ret;
        }

        if (NULL == (p = zbx_json_pair_by_name(&jp, JA_PROTO_TAG_DATA))) {
            zabbix_log(LOG_LEVEL_ERR,
                       "Received message error: [data] not found");
            return ret;

        } else {
            if (FAIL == zbx_json_brackets_open(p, &jp_row)) {
                zabbix_log(LOG_LEVEL_ERR,
                           "Received message error: Cannot open [data] object");
                return ret;

            } else {
                if (SUCCEED ==
                    zbx_json_value_by_name(&jp_row, JA_PROTO_TAG_RESULT,
                                           value, sizeof(value))) {
                    result = atoi(value);
                    if (result == 0) {

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_JOBNETID,
                                                   value, sizeof(value))) {
                            jobnetid = strdup(value);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_JOBNETNAME,
                                                   value, sizeof(value))) {
#if defined(_WINDOWS)
                            jobnetname = ja_utf8_to_acp((LPSTR)value);
#else
                            jobnetname = strdup(value);
#endif                          /* _WINDOWS */
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_SCHEDULEDTIME,
                                                   value, sizeof(value))) {
                            ZBX_STR2UINT64(scheduled_time, value);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_STARTTIME,
                                                   value, sizeof(value))) {
                            ZBX_STR2UINT64(start_time, value);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_ENDTIME,
                                                   value, sizeof(value))) {
                            ZBX_STR2UINT64(end_time, value);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_JOBNETRUNTYPE,
                                                   value, sizeof(value))) {
                            ch = atoi(value);
                            switch (ch) {
                            case 0:
                                jobnetruntype = "NORMAL";
                                break;
                            case 1:
                                jobnetruntype = "IMMEDIATE";
                                break;
                            case 2:
                                jobnetruntype = "WAIT";
                                break;
                            case 3:
                                jobnetruntype = "TEST";
                                break;
                            case 4:
                                jobnetruntype = "TIME";
                                break;
                            default:
                                exit(FAIL);
                                break;
                            }
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_JOBNETSTATUS,
                                                   value, sizeof(value))) {
                            jobnet_status = atoi(value);
                            switch (jobnet_status) {
                            case 0:
                                jobnetstatus = "BEGIN";
                                break;
                            case 1:
                                jobnetstatus = "READY";
                                break;
                            case 2:
                                jobnetstatus = "RUN";
                                break;
                            case 3:
                                jobnetstatus = "END";
                                break;
                            case 4:
                                jobnetstatus = "RUNERR";
                                break;
                            case 5:
                                jobnetstatus = "ENDERR";
                                break;
                            case 6:
                                jobnetstatus = "ABORT";
                                break;
                            default:
                                exit(FAIL);
                                break;
                            }
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_JOBSTATUS,
                                                   value, sizeof(value))) {
                            ch = atoi(value);
                            switch (ch) {
                            case 0:
                                jobstatus = "NORMAL";
                                break;
                            case 1:
                                jobstatus = "TIMEOUT";
                                if (jobnet_status == JA_JOBNET_STATUS_RUN) {
                                    jobnet_status = 21;
                                }
                                break;
                            case 2:
                                jobstatus = "ERROR";
                                if (jobnet_status == JA_JOBNET_STATUS_RUN) {
                                    jobnet_status = 22;
                                }
                                break;
                            default:
                                exit(FAIL);
                                break;
                            }
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_LASTEXITCD,
                                                   value, sizeof(value))) {
                            lastexitcd = strdup(value);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_LASTSTDOUT,
                                                   value, sizeof(value))) {
#if defined(_WINDOWS)
                            laststdout = ja_utf8_to_acp((LPSTR)value);
#else
                            laststdout = strdup(value);
#endif                          /* _WINDOWS */

                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_LASTSTDERR,
                                                   value, sizeof(value))) {
#if defined(_WINDOWS)
                            laststderr = ja_utf8_to_acp((LPSTR)value);
#else
                            laststderr = strdup(value);
#endif                          /* _WINDOWS */

                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Status");
                        }

                        if (JOBARG_VARIABLE_FORMAT == 0) {
                            fprintf(stderr,
                                    "\n\n"
                                    "jobnetid                 : %s\n"
                                    "jobnetname               : %s\n"
                                    "Time of a schedule       : " ZBX_FS_UI64 "\n"
                                    "Time of a start          : " ZBX_FS_UI64 "\n"
                                    "Time of a end            : " ZBX_FS_UI64 "\n"
                                    "The run type of a jobnet : %s\n"
                                    "Status of a jobnet       : %s\n"
                                    "Status of a job          : %s\n"
                                    "Last job return value    : %s\n"
                                    "Last job standard output : %s\n"
                                    "Last job standard error  : %s\n",
                                    jobnetid, jobnetname, scheduled_time,
                                    start_time, end_time, jobnetruntype,
                                    jobnetstatus, jobstatus, lastexitcd, laststdout, laststderr);
                        } else {
                            fprintf(stdout,
                                    "export JA_JOBNETID=\"%s\"\n"
                                    "export JA_JOBNETNAME=\"%s\"\n"
                                    "export JA_SCHEDULEDTIME=\"" ZBX_FS_UI64 "\"\n"
                                    "export JA_STARTTIME=\"" ZBX_FS_UI64 "\"\n"
                                    "export JA_ENDTIME=\"" ZBX_FS_UI64 "\"\n"
                                    "export JA_JOBNETRUNTYPE=\"%s\"\n"
                                    "export JA_JOBNETSTATUS=\"%s\"\n"
                                    "export JA_JOBSTATUS=\"%s\"\n"
                                    "export JA_LASTEXITCD=\"%s\"\n"
                                    "export JA_LASTSTDOUT=\"%s\"\n"
                                    "export JA_LASTSTDERR=\"%s\"\n",
                                     jobnetid, jobnetname, scheduled_time,
                                     start_time, end_time, jobnetruntype,
                                     jobnetstatus, jobstatus, lastexitcd, laststdout, laststderr);
                        }

                        ret = jobnet_status;

                    } else if (result == 1) {

                        if (SUCCEED ==
                            zbx_json_value_by_name(&jp_row,
                                                   JA_PROTO_TAG_MESSAGE,
                                                   value, sizeof(value))) {
                            message = strdup(value);
                            zabbix_log(LOG_LEVEL_ERR,
                                       "Cannnot send data: message [%s]",
                                       message);
                        } else {
                            zabbix_log(LOG_LEVEL_ERR,
                                       "Cannnot send data: cannnot get message from agent response");
                            return ret;
                        }

                    } else {
                        zabbix_log(LOG_LEVEL_ERR,
                                   "result range error : result [%d]",
                                   result);
                        return ret;
                    }

                } else {
                    zabbix_log(LOG_LEVEL_ERR,
                               "Received message error: [result] not found");
                    return ret;
                }
            }
        }
    } else {
        zabbix_log(LOG_LEVEL_ERR, "Cannot open received data.");
    }

    return ret;
}

static void parse_commandline(int argc, char **argv)
{
    char ch = '\0';

    /* parse the command-line */
    while ((char) EOF !=
           (ch =
            (char) zbx_getopt_long(argc, argv, shortopts, longopts,
                                   NULL))) {
        switch (ch) {
        case 'h':
            help_jobarg();
            exit(-1);
            break;
        case 'V':
            version_jobarg();
            exit(-1);
            break;
        case 'z':
            JOBARG_SERVER = zbx_strdup(JOBARG_SERVER, zbx_optarg);
            break;
        case 'p':
            JOBARG_SERVER_PORT = (unsigned short) atoi(zbx_optarg);
            break;
        case 'U':
            JOBARG_USERNAME = zbx_strdup(JOBARG_USERNAME, zbx_optarg);
            break;
        case 'P':
            JOBARG_PASSWORD = zbx_strdup(JOBARG_PASSWORD, zbx_optarg);
            break;
        case 'r':
            if (SUCCEED == reginum_check(zbx_optarg)) {
                JOBARG_REGISTRY_NUMBER =
                    zbx_strdup(JOBARG_REGISTRY_NUMBER, zbx_optarg);
                break;
            }
        case 'e':
            JOBARG_VARIABLE_FORMAT = 1;
            break;
        default:
            usage();
            exit(FAIL);
            break;
        }
    }

    if (NULL == JOBARG_SERVER || NULL == JOBARG_USERNAME
        || NULL == JOBARG_PASSWORD || NULL == JOBARG_REGISTRY_NUMBER) {
        usage();
        exit(FAIL);
    }
}

int main(int argc, char **argv)
{
    int ret = SUCCEED;
    struct zbx_json json;
    zbx_sock_t sock;
    char *answer = NULL;

#if defined(_WINDOWS)
	LPSTR acp_string = NULL;
#endif

    progname = get_program_name(argv[0]);

    parse_commandline(argc, argv);

    /*output to stderr */
    zabbix_open_log(LOG_TYPE_UNDEFINED, CONFIG_LOG_LEVEL, NULL);

    if (NULL == JOBARG_SERVER) {
        zabbix_log(LOG_LEVEL_ERR, "'Server' parameter required");
        ret = FAIL;
        goto exit;
    }
    if (0 == JOBARG_SERVER_PORT) {
        JOBARG_SERVER_PORT = JOBARG_DEFAULT_SERVER_PORT;
    }
    if (MIN_ZABBIX_PORT > JOBARG_SERVER_PORT) {
        zabbix_log(LOG_LEVEL_ERR,
                   "Incorrect port number [%d]. Allowed [%d:%d]",
                   (int) JOBARG_SERVER_PORT, (int) MIN_ZABBIX_PORT,
                   (int) MAX_ZABBIX_PORT);
        ret = FAIL;
        goto exit;
    }

    /*JSON data */
    zbx_json_init(&json, ZBX_JSON_STAT_BUF_LEN);
    zbx_json_addstring(&json, JA_PROTO_TAG_KIND,
                       JA_PROTO_VALUE_JOBNETSTATUSRQ,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_adduint64(&json, JA_PROTO_TAG_VERSION,
                       JA_PROTO_VALUE_VERSION_1);
    zbx_json_addobject(&json, JA_PROTO_TAG_DATA);
    zbx_json_addstring(&json, JA_PROTO_TAG_USERNAME, JOBARG_USERNAME,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_addstring(&json, JA_PROTO_TAG_PASSWORD, JOBARG_PASSWORD,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_addstring(&json, JA_PROTO_TAG_REGISTRYNUMBER,
                       JOBARG_REGISTRY_NUMBER, ZBX_JSON_TYPE_STRING);
    zbx_json_close(&json);
    zbx_json_close(&json);

    zabbix_log(LOG_LEVEL_DEBUG, "JSON before sending [%s]", json.buffer);

#if !defined(_WINDOWS)
    signal(SIGINT, send_signal_handler);
    signal(SIGTERM, send_signal_handler);
    signal(SIGQUIT, send_signal_handler);
    signal(SIGALRM, send_signal_handler);
#endif                          /* NOT _WINDOWS */

    if (SUCCEED != zbx_tcp_connect(&sock, JOBARG_SOURCE_IP, JOBARG_SERVER, JOBARG_SERVER_PORT, GET_SENDER_TIMEOUT)) {

#if defined(_WINDOWS)
		acp_string = ja_utf8_to_acp((LPSTR)zbx_tcp_strerror());
        zabbix_log(LOG_LEVEL_ERR, "Job arranger server connect error: [%s : %u] %s", JOBARG_SERVER, JOBARG_SERVER_PORT, acp_string);
		zbx_free(acp_string);
#else
        zabbix_log(LOG_LEVEL_ERR, "Job arranger server connect error: [%s : %u] %s", JOBARG_SERVER, JOBARG_SERVER_PORT, zbx_tcp_strerror());
#endif                          /* _WINDOWS */

        zbx_json_free(&json);
        ret = FAIL;
        goto exit;
    }

    if (SUCCEED != zbx_tcp_send(&sock, json.buffer)) {

#if defined(_WINDOWS)
		acp_string = ja_utf8_to_acp((LPSTR)zbx_tcp_strerror());
        zabbix_log(LOG_LEVEL_ERR, "Job arranger message send error: %s", acp_string);
		zbx_free(acp_string);
#else
        zabbix_log(LOG_LEVEL_ERR, "Job arranger message send error: %s", zbx_tcp_strerror());
#endif                          /* _WINDOWS */

        zbx_json_free(&json);
        zbx_tcp_close(&sock);
        ret = FAIL;
        goto exit;
    }

    if (SUCCEED != zbx_tcp_recv(&sock, &answer)) {

#if defined(_WINDOWS)
		acp_string = ja_utf8_to_acp((LPSTR)zbx_tcp_strerror());
        zabbix_log(LOG_LEVEL_ERR, "Job arranger message receive error: %s", acp_string);
		zbx_free(acp_string);
#else
        zabbix_log(LOG_LEVEL_ERR, "Job arranger message receive error: %s", zbx_tcp_strerror());
#endif                          /* _WINDOWS */

        zbx_json_free(&json);
        zbx_tcp_close(&sock);
        ret = FAIL;
        goto exit;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "Answer from server [%s]", answer);
    if (NULL == answer || FAIL == (ret = check_response(answer))) {
         ret = FAIL;
    }

    zbx_tcp_close(&sock);
    zbx_json_free(&json);

  exit:
    zabbix_close_log();

    return ret;
}
