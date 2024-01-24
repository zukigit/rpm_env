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
#include "cfg.h"
#include "log.h"
#include "zbxgetopt.h"
#include "zbxjson.h"

#include "jacommon.h"
#include "jastr.h"

#define JOBARG_DEFAULT_SERVER_PORT_STR  "10061"

/* long options */
static struct zbx_option longopts[] = {
    {"jobarranger-server", 1, NULL, 'z'},
    {"port", 1, NULL, 'p'},
    {"user-name", 1, NULL, 'U'},
    {"password", 1, NULL, 'P'},
    {"jobnet-id", 1, NULL, 'j'},
    {"start-time", 1, NULL, 't'},
    {"environment-variables", 1, NULL, 'E'},
    {"deterrence", 0, NULL, 'D'},
    {"help", 0, NULL, 'h'},
    {"version", 0, NULL, 'V'},
    {NULL}
};

/* short options */
static char shortopts[] = "z:p:U:P:j:t:E:DhV";

/* end of COMMAND LINE OPTIONS */

static int CONFIG_LOG_LEVEL = LOG_LEVEL_WARNING;

static char *JOBARG_SOURCE_IP = NULL;
static char *JOBARG_SERVER = NULL;
unsigned short JOBARG_SERVER_PORT = 0;
unsigned short JOBARG_DEFAULT_SERVER_PORT = 10061;
static char *JOBARG_JOBNETID = NULL;
static char *JOBARG_USERNAME = NULL;
static char *JOBARG_PASSWORD = NULL;
static char *JOBARG_START_TIME = NULL;
static char *JOBARG_ENV_VARIABLES = NULL;
static char *JOBARG_DETERRENCE = "0";

const char *progname = NULL;
const char title_message[] = "Job Arranger Jobnet execution";
const char usage_message[] =
    "[-hV] -z <server> [-p <port>] -U <user-name> -P <password> -j <jobnet-id> [-t <YYYYMMDDHHMM>] [-E <environment-variables>,...] [-D]";

const char *help_message[] = {
    "Options:",
    "  -z --jobarranger-server <server>                         Hostname or IP address of Job Arranger server",
    " [-p --port <port>]                                        Specify port number of server trapper running on the server. Default is " JOBARG_DEFAULT_SERVER_PORT_STR,
    "  -U --user-name <user-name>                               Specify user with authority to operate the jobnet",
    "  -P --password <password>                                 Specify user password",
    "  -j --jobnet-id <jobnet-id>                               Specify jobnet id",
    " [-t --start-time <YYYYMMDDHHSS>]                          Specify start time",
    " [-E --environment-variable <environment-variable>,...]    Specify environment variables",
    " [-D --deterrence]                                         Specify the double registration deterrence of time start-up with -t option",
    "",
    "Other options:",
    "  -h --help                                                Give this help",
    "  -V --version                                             Display version number",
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

int is_number(const char *str)
{
    if (NULL ==
        zbx_regexp_match(str, "^[-]{0,1}[0-9]+[.]{0,1}[0-9]*$", NULL)) {
        return 0;
    } else {
        return 1;
    }
}

int Ymdhm_check(const char *str)
{
    int val;
    int flg;
    int year;
    int month;
    int day;
    char YMdhm[13];
    char MM[3];
    char dd[3];
    char hh[3];
    char mm[3];
    char yyyy[5];
    int TIME_LENGTH = 12;

    /*length check */
    if (strlen(str) != TIME_LENGTH) {
        zabbix_log(LOG_LEVEL_ERR, "start_time length error.");
        return FAIL;
    }

    /*number check */
    if (!is_number(str)) {
        zabbix_log(LOG_LEVEL_ERR, "start_time number error.");
        return FAIL;
    }

    zbx_strlcpy(YMdhm, str, 12);
    YMdhm[12] = '\0';

    /*month check */
    MM[0] = YMdhm[4];
    MM[1] = YMdhm[5];
    MM[2] = '\0';

    if ((atoi(MM) < 1) || (atoi(MM) > 12)) {
        zabbix_log(LOG_LEVEL_ERR, "start_time month range error.");
        return FAIL;
    }

    /*date check */
    dd[0] = YMdhm[6];
    dd[1] = YMdhm[7];
    dd[2] = '\0';

    if ((atoi(dd) < 1) || (atoi(dd) > 31)) {
        zabbix_log(LOG_LEVEL_ERR, "start_time date range error.");
        return FAIL;
    }

    month = atoi(MM);
    day = atoi(dd);

    if ((month == 4) || (month == 6) || (month == 9) || (month == 11)) {
        if (day == 31) {
            zabbix_log(LOG_LEVEL_ERR, "start_time date range error.");
            return FAIL;
        }
    }

    /*leap year check */
    if (month == 2) {
        memset(yyyy, '\0', sizeof(yyyy));
        memcpy(yyyy, str, 4);
        year = atoi(yyyy);
        flg = 0;
        if ((val = year % 4) == 0) {
            flg = 1;
            if ((val = year % 100) == 0) {
                flg = 0;
                if ((val = year % 400) == 0) {
                    flg = 1;
                }
            }
        } else {
            flg = 0;
        }

        if (flg == 1) {
            if (day > 29) {
                zabbix_log(LOG_LEVEL_ERR, "start_time date range error.");
                return FAIL;
            }
        } else {
            if (day > 28) {
                zabbix_log(LOG_LEVEL_ERR, "start_time leap year error.");
                return FAIL;
            }
        }
    }

    /*hour check */
    hh[0] = YMdhm[8];
    hh[1] = YMdhm[9];
    hh[2] = '\0';

    if ((atoi(hh) < 0) || (atoi(hh) > 23)) {
        zabbix_log(LOG_LEVEL_ERR, "start_time hour range error.");
        return FAIL;
    }

    /*minute check */
    mm[0] = YMdhm[10];
    mm[1] = YMdhm[11];
    mm[2] = '\0';

    if ((atoi(mm) < 0) || (atoi(mm) > 59)) {
        zabbix_log(LOG_LEVEL_ERR, "start_time minute range error.");
        return FAIL;
    }

    return SUCCEED;

}

static int check_response(char *response)
{
    struct zbx_json_parse jp, jp_row;
    char value[MAX_STRING_LEN];
    char *message;
    const char *p;
    int ret = FAIL;
    int result;
    int version;

    if (SUCCEED == zbx_json_open(response, &jp)) {
        if (SUCCEED ==
            zbx_json_value_by_name(&jp, JA_PROTO_TAG_KIND, value,
                                   sizeof(value))) {
            if (0 != strcmp(value, JA_PROTO_VALUE_JOBNETRUN_RES)) {
                zabbix_log(LOG_LEVEL_ERR,
                           "Received message error: [kind] is not [jobnetrun-res]");
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
                                                   JA_PROTO_TAG_MESSAGE,
                                                   value, sizeof(value))) {
                            message = strdup(value);
                            zabbix_log(LOG_LEVEL_INFORMATION, "%s",
                                       message);
                        } else {
                            zabbix_log(LOG_LEVEL_INFORMATION,
                                       "Succeeded, but could not get Registry number");
                        }
                        ret = SUCCEED;

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
    while ((char) EOF != (ch = (char) zbx_getopt_long(argc, argv, shortopts, longopts, NULL))) {
        switch (ch) {
        case 'h':
            help_jobarg();
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
        case 'j':
            JOBARG_JOBNETID = zbx_strdup(JOBARG_JOBNETID, zbx_optarg);
            break;
        case 't':
            JOBARG_START_TIME = zbx_strdup(JOBARG_START_TIME, zbx_optarg);
            break;
        case 'E':
            JOBARG_ENV_VARIABLES = zbx_strdup(JOBARG_ENV_VARIABLES, zbx_optarg);
            break;
        case 'D':
            JOBARG_DETERRENCE = "1";
            break;
        case 'V':
            version_jobarg();
            exit(-1);
            break;
        default:
            usage();
            exit(FAIL);
            break;
        }
    }

    if (NULL == JOBARG_SERVER || NULL == JOBARG_USERNAME ||
        NULL == JOBARG_PASSWORD || NULL == JOBARG_JOBNETID) {
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
    char *tp = NULL;
    char *copy = NULL;
    char *env = NULL;

#if defined(_WINDOWS)
	LPSTR acp_string = NULL;
#endif

    progname = get_program_name(argv[0]);

    parse_commandline(argc, argv);

    /* output to stderr */
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

    /* JSON data */
    zbx_json_init(&json, ZBX_JSON_STAT_BUF_LEN);
    zbx_json_addstring(&json, JA_PROTO_TAG_KIND, JA_PROTO_VALUE_JOBNETRUN,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_adduint64(&json, JA_PROTO_TAG_VERSION,
                       JA_PROTO_VALUE_VERSION_1);
    zbx_json_addobject(&json, JA_PROTO_TAG_DATA);
    zbx_json_addstring(&json, JA_PROTO_TAG_USERNAME, JOBARG_USERNAME,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_addstring(&json, JA_PROTO_TAG_PASSWORD, JOBARG_PASSWORD,
                       ZBX_JSON_TYPE_STRING);
    zbx_json_addstring(&json, JA_PROTO_TAG_JOBNETID, JOBARG_JOBNETID,
                       ZBX_JSON_TYPE_STRING);

    /* Start time */
    if (JOBARG_START_TIME != NULL) {
        ret = Ymdhm_check(JOBARG_START_TIME);
        if (ret != SUCCEED) {
            zbx_json_free(&json);
            goto exit;
        }
        zbx_json_addstring(&json, JA_PROTO_TAG_STARTTIME,
                           JOBARG_START_TIME, ZBX_JSON_TYPE_STRING);
        zbx_json_addstring(&json, JA_PROTO_TAG_DETERRENCE,
                           JOBARG_DETERRENCE, ZBX_JSON_TYPE_STRING);
    }

    /* Env */
    if (JOBARG_ENV_VARIABLES != NULL) {
        zbx_json_addobject(&json, JA_PROTO_TAG_ENV);
        copy = (char *) malloc(strlen(JOBARG_ENV_VARIABLES) + 1);

        zbx_strlcpy(copy, JOBARG_ENV_VARIABLES,
                    (strlen(JOBARG_ENV_VARIABLES) + 1));
        tp = strtok(copy, ",");

        if (strcmp(JOBARG_ENV_VARIABLES, tp)) {
            /* delim */
            env = getenv(tp);
            if (env == NULL) {
                zabbix_log(LOG_LEVEL_ERR, "Env [%s] does not exist.", tp);
                zbx_json_free(&json);
                ret = FAIL;
                goto exit;
            } else {
                zbx_json_addstring(&json, tp, env, ZBX_JSON_TYPE_STRING);
            }
            while (NULL != (tp = strtok(0, ","))) {
                env = getenv(tp);
                if (env == NULL) {
                    zabbix_log(LOG_LEVEL_ERR, "Env [%s] does not exist.",
                               tp);
                    zbx_json_free(&json);
                    ret = FAIL;
                    goto exit;
                } else {
                    zbx_json_addstring(&json, tp, env,
                                       ZBX_JSON_TYPE_STRING);
                }
            }
        } else {
            env = getenv(tp);
            if (env == NULL) {
                zabbix_log(LOG_LEVEL_ERR, "Env [%s] does not exist.", tp);
                zbx_json_free(&json);
                ret = FAIL;
                goto exit;
            } else {
                zbx_json_addstring(&json, tp, env, ZBX_JSON_TYPE_STRING);
            }
        }
        zbx_json_close(&json);
    }

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
    if (NULL == answer || SUCCEED != check_response(answer)) {
        ret = FAIL;
    }

    zbx_tcp_close(&sock);
    zbx_json_free(&json);

  exit:
    zabbix_close_log();

    return ret;
}
