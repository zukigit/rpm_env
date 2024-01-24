/*
** Job Arranger for ZABBIX
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
** Copyright (C) 2022 Daiwa Institute of Research Ltd. All Rights Reserved.
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


#ifndef JOBARG_JACOMMON_H
#define JOBARG_JACOMMON_H

/* version or revsion */
#define JOBARG_VERSION          "6.0.4"
#define JOBARG_REVISION         "3105"
#define JOBARG_REVDATE			"2024-01-19"

/* treat unknown parameters as error */
#define JA_CFG_STRICT			0

/* config default value */
#define JA_DEFAULT_AGENT_PORT		10055
#define JA_DEFAULT_SERVER_PORT		10061

/* data size */
#define JA_MAX_STRING_LEN		1024
#define JA_MAX_DATA_LEN			4096
#define JA_STD_OUT_LEN			64000 + 1
#define JA_JOBNET_ID_LEN		32 + 1
#define JA_JOB_ID_LEN			32 + 1
#define JA_CALENDAR_ID_LEN		32 + 1
#define JA_SCHEDULE_ID_LEN		32 + 1
#define JA_FILTER_ID_LEN		32 + 1

#define JA_JOBNET_NAME_LEN		96 + 1
#define JA_JOB_NAME_LEN			96 + 1

#define JA_USER_NAME_LEN		100 + 1
#define JA_VALUE_NAME_LEN		128 + 1
#define JA_HOST_NAME_LEN		128 + 1
#define JA_STOP_CODE_LEN		32 + 1
#define JA_KIND_LEN			16
#define JA_SERVERID_LEN			32+1
#define JA_COMMAND_ID_LEN		32 + 1
#define JA_ENV_LEN				64000 + 64000 + 4096 + 1
//added by ThihaOo@dat
#define JA_MAX_READ_LEN			101
#define JA_FILE_PATH_LEN		260
/* extjob command */
#define JA_CMD_SLEEP			"jacmdsleep"
#define JA_CMD_TIME			"jacmdtime"
#define JA_CMD_WEEK			"jacmdweek"
#define JA_CMD_ZBXSENDER		"zabbix_sender"

/* extjob result file name */
#define JA_EXTJOB_RESULT_FILE		"extjob_result"

/* transaction instruction */
#define	JA_TXN_OFF			0
#define	JA_TXN_ON			1

/* ja_run_jobnet_table */
/* run type */
#define	JA_JOBNET_RUN_TYPE_NORMAL	0
#define	JA_JOBNET_RUN_TYPE_IMMEDIATE	1
#define	JA_JOBNET_RUN_TYPE_WAIT		2
#define	JA_JOBNET_RUN_TYPE_TEST		3
#define	JA_JOBNET_RUN_TYPE_SCHEDULED	4
#define	JA_JOBNET_RUN_TYPE_JOBALONE	5

/* main flag */
#define	JA_JOBNET_MAIN_FLAG_MAIN	0
#define	JA_JOBNET_MAIN_FLAG_SUB		1

/* abort flag */
#define	JA_JOBENT_ABORT_FLAG_OFF	0
#define	JA_JOBENT_ABORT_FLAG_ON		1

/* jobnet status */
#define	JA_JOBNET_STATUS_BEGIN		0
#define	JA_JOBNET_STATUS_READY		1
#define	JA_JOBNET_STATUS_RUN		2
#define	JA_JOBNET_STATUS_END		3
#define	JA_JOBNET_STATUS_RUNERR		4
#define	JA_JOBNET_STATUS_ENDERR		5

/* jobnet abort */
#define JA_JOBNET_IS_NORMAL         0
#define JA_JOBNET_IS_ABORTING       1

/* multiple start up */
#define	JA_JOBNET_MULTIPLE_ON		0
#define	JA_JOBNET_MULTIPLE_SKIP		1
#define	JA_JOBNET_MULTIPLE_DELAY	2

/* ja_run_jobnet_summary_table */
/* jobnet job status */
#define	JA_SUMMARY_JOB_STATUS_NORMAL	0
#define	JA_SUMMARY_JOB_STATUS_TIMEOUT	1
#define	JA_SUMMARY_JOB_STATUS_ERROR	2

/* load status */
#define	JA_SUMMARY_LOAD_STATUS_NORMAL	0
#define	JA_SUMMARY_LOAD_STATUS_ERROR	1
#define	JA_SUMMARY_LOAD_STATUS_DELAY	2
#define	JA_SUMMARY_LOAD_STATUS_SKIP	3

/* start pending flag */
#define	JA_SUMMARY_START_PENDING_NONE	0
#define	JA_SUMMARY_START_PENDING_ON	1
#define	JA_SUMMARY_START_PENDING_OFF	2


/* ja_run_job_table */
/* job type */
#define	JA_JOB_TYPE_START		0
#define	JA_JOB_TYPE_END			1
#define	JA_JOB_TYPE_IF			2
#define	JA_JOB_TYPE_VALUE		3
#define	JA_JOB_TYPE_JOB			4
#define	JA_JOB_TYPE_JOBNET		5
#define	JA_JOB_TYPE_M			6
#define	JA_JOB_TYPE_W			7
#define	JA_JOB_TYPE_L			8
#define	JA_JOB_TYPE_EXTJOB		9
#define	JA_JOB_TYPE_CALC		10
#define	JA_JOB_TYPE_TASK		11
#define	JA_JOB_TYPE_INFO		12
#define	JA_JOB_TYPE_IFEND		13
#define	JA_JOB_TYPE_FCOPY		14
#define	JA_JOB_TYPE_FWAIT		15
#define	JA_JOB_TYPE_REBOOT		16
#define	JA_JOB_TYPE_REL			17
#define	JA_JOB_TYPE_LESS		18
#define	JA_JOB_TYPE_LINK		19
#define	JA_JOB_TYPE_ABORT		90

/* job status */
#define	JA_JOB_STATUS_BEGIN		0
#define	JA_JOB_STATUS_READY		1
#define	JA_JOB_STATUS_RUN		2
#define	JA_JOB_STATUS_END		3
#define	JA_JOB_STATUS_RUNERR		4
#define	JA_JOB_STATUS_ENDERR		5
#define	JA_JOB_STATUS_ABORT		6

/* job method */
#define	JA_JOB_METHOD_NORMAL		0
#define	JA_JOB_METHOD_WAIT		1
#define	JA_JOB_METHOD_SKIP		2
#define	JA_JOB_METHOD_ABORT		3
#define	JA_JOB_METHOD_RERUN		4

/* test flag */
#define	JA_JOB_TEST_FLAG_OFF		0
#define	JA_JOB_TEST_FLAG_ON		1

/* invo flag */
#define	JA_JOB_INVO_FLAG_OFF		0
#define	JA_JOB_INVO_FLAG_ON		1

/* force flag */
#define	JA_JOB_FORCE_FLAG_OFF		0
#define	JA_JOB_FORCE_FLAG_ON		1

/* continue flag */
#define	JA_JOB_CONTINUE_FLAG_OFF	0
#define	JA_JOB_CONTINUE_FLAG_ON		1


/* ja_run_flow_table */
/* flow type */
#define	JA_FLOW_TYPE_NORMAL		0
#define	JA_FLOW_TYPE_TRUE		1
#define	JA_FLOW_TYPE_FALSE		2


/* ja_run_icon_agentless_table */
/* session flag */
#define JA_SESSION_FLAG_ONETIME		0
#define JA_SESSION_FLAG_CONNECT		1
#define JA_SESSION_FLAG_CONTINUE	2
#define JA_SESSION_FLAG_CLOSE		3

/* auth method */
#define JA_SES_AUTH_PASSWORD		0
#define JA_SES_AUTH_PUBLICKEY		1

/* run mode */
#define JA_RUN_MODE_INTERACTIVE		0
#define JA_RUN_MODE_NON_INTERACTIVE	1

/* line feed code */
#define JA_LINE_FEED_CODE_LF		0
#define JA_LINE_FEED_CODE_CR		1
#define JA_LINE_FEED_CODE_CRLF		2


/* ja_session_table */
/* operation flag */
#define JA_SES_OPERATION_FLAG_ONETIME	0
#define JA_SES_OPERATION_FLAG_CONNECT	1
#define JA_SES_OPERATION_FLAG_CONTINUE	2
#define JA_SES_OPERATION_FLAG_CLOSE	3

/* session status */
#define JA_SES_STATUS_BEGIN		0
#define JA_SES_STATUS_END		1

/* force stop */
#define JA_SES_FORCE_STOP_OFF		0
#define JA_SES_FORCE_STOP_ON		1
#define JA_SES_FORCE_STOP_KILL		2


/* ja_send_message_table */
/* send status */
#define JA_SNT_SEND_STATUS_BEGIN	0
#define JA_SNT_SEND_STATUS_END		1
#define JA_SNT_SEND_STATUS_RETRY	2
#define JA_SNT_SEND_STATUS_ERROR	3


/* ssh info */
#define JA_SSH_PORT			"{$JOBARRANGER_SSH_PORT}"
#define JA_SSH_CONNECT_PORT		22


/* job agent info */
#define JA_AGENT_PORT			"{$JOBARRANGER_AGENT_PORT}"


/* telegram message */
#define JA_PROTO_TAG_JOBNETID		"jobnetid"
#define JA_PROTO_TAG_JOBNETNAME		"jobnetname"
#define JA_PROTO_TAG_JOBNETRUNTYPE	"jobnetruntype"
#define JA_PROTO_TAG_JOBNETSTATUS	"jobentstatus"
#define JA_PROTO_TAG_JOBSTATUS		"jobstatus"
#define JA_PROTO_TAG_USERNAME		"username"
#define JA_PROTO_TAG_PASSWORD		"password"
#define JA_PROTO_TAG_SCHEDULEDTIME	"scheduled_time"
#define JA_PROTO_TAG_STARTTIME		"start_time"
#define JA_PROTO_TAG_ENDTIME		"end_time"
#define JA_PROTO_TAG_REGISTRYNUMBER	"registrynumber"
#define JA_PROTO_TAG_LASTEXITCD         "lastexitcd"
#define JA_PROTO_TAG_LASTSTDOUT         "laststdout"
#define JA_PROTO_TAG_LASTSTDERR         "laststderr"
#define JA_PROTO_TAG_DETERRENCE		"deterrence"

/* message item value */
#define JA_PROTO_VALUE_JOBNETRUN	"jobnetrun"
#define JA_PROTO_VALUE_JOBNETRUN_RES	"jobnetrun-res"
#define JA_PROTO_VALUE_JOBNETSTATUSRQ		"jobnetstatusrq"
#define JA_PROTO_VALUE_JOBNETSTATUSRQ_RES	"jobnetstatusrq-res"

/* telegram head */
#define JA_PROTO_TAG_KIND		"kind"
#define JA_PROTO_TAG_VERSION		"version"
#define JA_PROTO_TAG_SERVERID		"serverid"
#define JA_PROTO_TAG_SERVERIP		"serverip"
#define JA_PROTO_TAG_SEND_RETRY		"sendretry"
#define JA_PROTO_TAG_DATA		"data"
#define JA_PROTO_TELE_VERSION		JA_PROTO_VALUE_VERSION_1
#define JA_PROTO_VALUE_VERSION_1	1

/* jobrun telegram */
#define JA_PROTO_VALUE_JOBRUN		"jobrun"
#define JA_PROTO_TAG_JOBID		"jobid"
#define JA_PROTO_TAG_METHOD		"method"
#define JA_PROTO_TAG_TYPE		"type"
#define JA_PROTO_TAG_ARGUMENT		"argument"
#define JA_PROTO_TAG_ENV		"env"
#define JA_PROTO_TAG_SCRIPT		"script"
#define JA_PROTO_VALUE_SCRIPT		"script"
#define JA_PROTO_VALUE_COMMAND		"command"
#define JA_PROTO_VALUE_RUNUSR		"run_user"
#define JA_PROTO_VALUE_RUNUSRPWD	"run_user_password"

/* jobrun response */
#define JA_PROTO_VALUE_JOBRUN_RES	"jobrun-res"
#define JA_RESPONSE_SUCCEED		0
#define JA_RESPONSE_VERSION_FAIL	1
#define JA_RESPONSE_FAIL		2
#define JA_RESPONSE_ALREADY_RUN		3

/* job result */
#define JA_PROTO_VALUE_JOBRESULT	"jobresult"
#define JA_PROTO_TAG_HOSTNAME		"hostname"
#define JA_PROTO_TAG_RESULT		"result"
#define JA_PROTO_TAG_MESSAGE		"message"
#define JA_PROTO_TAG_MYVERSION		"myversion"
#define JA_PROTO_TAG_JOBSTDOUT		"std_out"
#define JA_PROTO_TAG_JOBSTDERR		"std_err"
#define JA_PROTO_TAG_RET		"return_code"
#define JA_PROTO_TAG_SIGNAL		"signal"
#define JA_JOBRESULT_SUCCEED		0
#define JA_JOBRESULT_FAIL		1

#define JA_PROTO_TAG_CUR_UNIQUE_ID "cur_unique_id"
#define JA_PROTO_TAG_PRE_UNIQUE_ID "pre_unique_id"

/* job result response */
#define JA_PROTO_VALUE_JOBRESULT_RES	"jobresult-res"
#define JA_JOBRESULT_RES_SUCCEED	0
#define JA_JOBRESULT_RES_FAIL		1

/* job file wait */
#define JA_PROTO_VALUE_FWAIT		"fwait"

/* job file copy */
#define JA_PROTO_VALUE_FCOPY		"fcopy"
#define JA_PROTO_VALUE_FCOPY_RES	"fcopy-res"
#define JA_PROTO_VALUE_GETFILE		"getfile"
#define JA_PROTO_VALUE_PUTFILE		"putfile"
#define JA_PROTO_TAG_FROMDIR		"fromdir"
#define JA_PROTO_TAG_FILENAME		"filename"
#define JA_PROTO_TAG_TODIR		"todir"
#define JA_PROTO_TAG_OVERWRITE		"overwrite"
#define JA_FCOPY_FLAG_SUCCEED		'0'
#define JA_FCOPY_FLAG_FAIL		'1'
#define JA_FCOPY_FLAG_NOFILE		'2'
#define JA_FCOPY_FLAG_KEEP		'3'

/* job agent reboot */
#define JA_PROTO_VALUE_REBOOT		"reboot"
#define JA_PROTO_TAG_REBOOT_MODE	"reboot_mode_flag"
#define JA_PROTO_TAG_REBOOT_WAIT_TIME	"reboot_wait_time"

/* job agent extjob */
#define JA_PROTO_VALUE_EXTJOB           "extjob"

/* job agent checkjob */
#define JA_PROTO_VALUE_CHKJOB           "chkjob"
#define JA_PROTO_TAG_HOST_JOB_LIST      "host_job_list"

/* job ipchange request */
#define JA_PROTO_VALUE_IPCHANGE           "ipchange"

/* job agent check abort job */
#define JA_ABORT_FILE_ALREADY_EXIST           1

/* job arranger agent */
#ifdef _WINDOWS
    #include <TlHelp32.h>
    #define JA_DLM '\\'
    #define JA_EXE "bat"
    #define JA_PID DWORD
    #define JA_JOBARG_COMMAND		"jobarg_command.exe"
    #define JA_JOBARG_REBOOT		"jareboot.bat"
    #define SIGNALNO 137
#else
    #define JA_DLM '/'
    #define JA_EXE "sh"
    #define JA_PID pid_t
    #define JA_JOBARG_COMMAND		"jobarg_command"
    #define JA_JOBARG_REBOOT		"jareboot.sh"
    #define O_BINARY 0
#endif

/* job arranger agent method */
#define JA_AGENT_METHOD_NORMAL		0
#define JA_AGENT_METHOD_TEST		1
#define JA_AGENT_METHOD_ABORT		2
#define JA_AGENT_METHOD_KILL		3

/* job arranger agent status */
#define JA_AGENT_STATUS_INIT		-1
#define JA_AGENT_STATUS_BEGIN		0
#define JA_AGENT_STATUS_RUN		1
#define JA_AGENT_STATUS_END		2
#define JA_AGENT_STATUS_CLOSE		3

//Added by ThihaOo@dat
/* job file read_all_file*/
#define JA_FILE_NAME_LEN		53+1
#define JA_PERMISSION      0755

/* job arranger agent interface data */
typedef struct {
    char		kind[JA_KIND_LEN];
    double		version;
    zbx_uint64_t	jobid;
    char		serverid[JA_SERVERID_LEN];
    char		hostname[128];
    int			method;
    char		type[JA_MAX_STRING_LEN];
    char		argument[JA_MAX_DATA_LEN];
    char		script[JA_MAX_DATA_LEN];
    char		env[JA_ENV_LEN];
    int			result;
    int			status;
    JA_PID		pid;
    zbx_uint64_t	start_time;
    zbx_uint64_t	end_time;
    char		message[JA_MAX_STRING_LEN];
    char		std_out[JA_STD_OUT_LEN];
    char		std_err[JA_STD_OUT_LEN];
    int			return_code;
    int			signal;
    int			send_retry;
    char		run_user[JA_MAX_STRING_LEN];
    char		run_user_password[JA_MAX_STRING_LEN];
    int			loop_cnt;

    char cur_unique_id[JA_MAX_STRING_LEN];
    char pre_unique_id[JA_MAX_STRING_LEN];

    char       serverip[JA_SERVERID_LEN];
    // zbx_uint64_t    host_running_job[512];
    zbx_uint64_t*    host_running_job;
    int         size_of_host_running_job;
} ja_job_object;

/* Job file read object.*/
typedef struct {
    char        filename[JA_FILE_NAME_LEN];
} ja_file_object;

//Status(FOLDER) type
#define JA_TYPE_STATUS_DATA		0
#define JA_TYPE_STATUS_BEGIN	1
#define JA_TYPE_STATUS_EXEC		2
#define JA_TYPE_STATUS_END		3
#define JA_TYPE_STATUS_CLOSE	4
#define JA_TYPE_STATUS_ERROR	5
#define JA_TYPE_STATUS_TEMP 	6
#define JA_TYPE_STATUS_JOBS 	7
#define JA_TYPE_STATUS_IPS      8
#endif  /* JOBARG_JACOMMON_H */
