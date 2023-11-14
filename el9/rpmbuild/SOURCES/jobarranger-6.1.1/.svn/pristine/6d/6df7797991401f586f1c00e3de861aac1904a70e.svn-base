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


#include "zbxself.h"
#include "common.h"
#include "mutexs.h"
#include "ipc.h"
#include "log.h"

#include "jacommon.h"
#include "jaself.h"
#include "jalog.h"

#define MAX_HISTORY	60

typedef struct
{
	unsigned short	h_counter[ZBX_PROCESS_STATE_COUNT][MAX_HISTORY];
	unsigned short	counter[ZBX_PROCESS_STATE_COUNT];
	clock_t		last_ticks;
	unsigned char	last_state;
}
zbx_stat_process_t;

typedef struct
{
	zbx_stat_process_t	**process;
	int			first;
	int			count;
}
zbx_selfmon_collector_t;

static zbx_selfmon_collector_t	*collector = NULL;
static int		shm_id;
static int		sleep_remains;

#define	LOCK_SM		zbx_mutex_lock(&sm_lock)
#define	UNLOCK_SM	zbx_mutex_unlock(&sm_lock)

static ZBX_MUTEX	sm_lock;

extern char	*CONFIG_FILE;
extern int	CONFIG_JASELFMON_FORKS;
extern int	CONFIG_JARUN_FORKS;
extern int	CONFIG_JATRAPPER_FORKS;
extern int	CONFIG_JAJOB_FORKS;
extern int	CONFIG_JAJOBNET_FORKS;
extern int	CONFIG_JALOADER_FORKS;
extern int	CONFIG_JABOOT_FORKS;
extern int	CONFIG_JAMSGSND_FORKS;
extern int  CONFIG_JAPURGE_FORKS;
extern int  CONFIG_JAABORT_FORKS;

static int write_flag = 1;

/******************************************************************************
 *                                                                            *
 * Function: ja_get_process_type_forks                                        *
 *                                                                            *
 * Purpose: Returns number of processes depending on process type             *
 *                                                                            *
 * Parameters: process_type - [IN] process type; ZBX_PROCESS_TYPE_*           *
 *                                                                            *
 * Return value: number of processes                                          *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
int	ja_get_process_type_forks(unsigned char process_type)
{
	switch (process_type)
	{
		case JA_PROCESS_TYPE_JARUN:
			return CONFIG_JARUN_FORKS;
		case JA_PROCESS_TYPE_JATRAPPER:
			return CONFIG_JATRAPPER_FORKS;
		case JA_PROCESS_TYPE_JAJOB:
			return CONFIG_JAJOB_FORKS;
		case JA_PROCESS_TYPE_JAJOBNET:
			return CONFIG_JAJOBNET_FORKS;
		case JA_PROCESS_TYPE_JALOADER:
			return CONFIG_JALOADER_FORKS;
		case JA_PROCESS_TYPE_JABOOT:
			return CONFIG_JABOOT_FORKS;
		case JA_PROCESS_TYPE_JASNDMSG:
			return CONFIG_JAMSGSND_FORKS;
		case JA_PROCESS_TYPE_SELFMON:
			return CONFIG_JASELFMON_FORKS;
		case JA_PROCESS_TYPE_JAPURGE:
			return CONFIG_JAPURGE_FORKS;
		case JA_PROCESS_TYPE_JAABORT:
			return CONFIG_JAABORT_FORKS;
	}

	assert(0);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_get_process_type_string                                       *
 *                                                                            *
 * Purpose: Returns process name                                              *
 *                                                                            *
 * Parameters: process_type - [IN] process type; ZBX_PROCESS_TYPE_*           *
 *                                                                            *
 * Comments: used in internals checks zabbix["process",...], process titles   *
 *           and log files                                                    *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
const char	*ja_get_process_type_string(unsigned char process_type)
{
	switch (process_type)
	{
		case JA_PROCESS_TYPE_JARUN:
			return "start the job";
		case JA_PROCESS_TYPE_JATRAPPER:
			return "job trapper";
		case JA_PROCESS_TYPE_JAJOB:
			return "check the running job";
		case JA_PROCESS_TYPE_JAJOBNET:
			return "start and check the jobnet";
		case JA_PROCESS_TYPE_JALOADER:
			return "job loader";
		case JA_PROCESS_TYPE_JABOOT:
			return "jobnet boot";
		case JA_PROCESS_TYPE_JASNDMSG:
			return "message send";
		case JA_PROCESS_TYPE_SELFMON:
			return "self-monitoring";
		case JA_PROCESS_TYPE_JAPURGE:
			return "purge old jobnet";
		case JA_PROCESS_TYPE_JAABORT:
			return "abort process";
	}

	assert(0);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_init_selfmon_collector                                        *
 *                                                                            *
 * Purpose: Initialize structures and prepare state                           *
 *          for self-monitoring collector                                     *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
void	ja_init_selfmon_collector()
{
	const char	*__function_name = "ja_init_selfmon_collector";
	size_t		sz, sz_array, sz_process[JA_PROCESS_TYPE_COUNT], sz_total;
	key_t		shm_key;
	char		*p;
	clock_t		ticks;
	struct tms	buf;
	unsigned char	process_type;
	int		process_num, process_forks;

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	sz_total = sz = sizeof(zbx_selfmon_collector_t);
	sz_total += sz_array = sizeof(zbx_stat_process_t *) * JA_PROCESS_TYPE_COUNT;

	for (process_type = 0; JA_PROCESS_TYPE_COUNT > process_type; process_type++)
		sz_total += sz_process[process_type] = sizeof(zbx_stat_process_t) * ja_get_process_type_forks(process_type);

	zabbix_log(LOG_LEVEL_DEBUG, "%s() size:" ZBX_FS_SIZE_T, __function_name, (zbx_fs_size_t)sz_total);

	if (-1 == (shm_key = zbx_ftok(CONFIG_FILE, ZBX_IPC_SELFMON_ID)))
	{
		ja_log("JASELF100001", 0, NULL, 0);
		exit(FAIL);
	}

	if (ZBX_MUTEX_ERROR == zbx_mutex_create_force(&sm_lock, ZBX_MUTEX_SELFMON))
	{
		zbx_error("unable to create mutex for a self-monitoring collector");
		exit(FAIL);
	}

	if (-1 == (shm_id = zbx_shmget(shm_key, sz_total)))
	{
		ja_log("JASELF100002", 0, NULL, 0);
		exit(FAIL);
	}

	if ((void *)(-1) == (p = shmat(shm_id, NULL, 0)))
	{
		ja_log("JASELF100003", 0, NULL, 0, zbx_strerror(errno));
		exit(FAIL);
	}

	collector = (zbx_selfmon_collector_t *)p; p += sz;
	collector->process = (zbx_stat_process_t **)p; p += sz_array;

	ticks = times(&buf);

	for (process_type = 0; JA_PROCESS_TYPE_COUNT > process_type; process_type++)
	{
		collector->process[process_type] = (zbx_stat_process_t *)p; p += sz_process[process_type];
		memset(collector->process[process_type], 0, sz_process[process_type]);

		process_forks = ja_get_process_type_forks(process_type);
		for (process_num = 0; process_num < process_forks; process_num++)
		{
			collector->process[process_type][process_num].last_ticks = ticks;
			collector->process[process_type][process_num].last_state = ZBX_PROCESS_STATE_BUSY;
		}
	}

	zabbix_log(LOG_LEVEL_DEBUG, "End of %s() collector:%p", __function_name, collector);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_free_selfmon_collector                                        *
 *                                                                            *
 * Purpose: Free memory allocated for self-monitoring collector               *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
void	ja_free_selfmon_collector()
{
	const char	*__function_name = "ja_free_selfmon_collector";

	zabbix_log(LOG_LEVEL_DEBUG, "In %s() collector:%p", __function_name, collector);

	if (NULL == collector)
		return;

	LOCK_SM;

	collector = NULL;

	if (-1 == shmctl(shm_id, IPC_RMID, 0))
	{
		ja_log("JASELF300001", 0, NULL, 0, zbx_strerror(errno));
	}

	UNLOCK_SM;

	zbx_mutex_destroy(&sm_lock);

	zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_update_selfmon_counter                                        *
 *                                                                            *
 * Parameters: state - [IN] new process state; ZBX_PROCESS_STATE_*            *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
void	ja_update_selfmon_counter(unsigned char state)
{
	extern int		process_num;
	extern unsigned char	process_type;

	zbx_stat_process_t	*process;
	clock_t			ticks;
	struct tms		buf;

	if (JA_PROCESS_TYPE_UNKNOWN == process_type)
		return;

	process = &collector->process[process_type][process_num - 1];
	ticks = times(&buf);

	LOCK_SM;

	if (ticks > process->last_ticks)
		process->counter[process->last_state] += ticks - process->last_ticks;
	process->last_ticks = ticks;
	process->last_state = state;

	UNLOCK_SM;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_collect_selfmon_stats                                         *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
void	ja_collect_selfmon_stats()
{
	const char		*__function_name = "ja_collect_selfmon_stats";
	zbx_stat_process_t	*process;
	clock_t			ticks;
	struct tms		buf;
	unsigned char		process_type, state;
	int			process_num, process_forks, index;

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	ticks = times(&buf);

	LOCK_SM;

	if (MAX_HISTORY <= (index = collector->first + collector->count))
		index -= MAX_HISTORY;

	if (collector->count < MAX_HISTORY)
		collector->count++;
	else if (++collector->first == MAX_HISTORY)
		collector->first = 0;

	for (process_type = 0; process_type < JA_PROCESS_TYPE_COUNT; process_type++)
	{
		process_forks = ja_get_process_type_forks(process_type);
		for (process_num = 0; process_num < process_forks; process_num++)
		{
			process = &collector->process[process_type][process_num];
			for (state = 0; state < ZBX_PROCESS_STATE_COUNT; state++){
				process->h_counter[state][index] = process->counter[state];

			}
			if (ticks > process->last_ticks)
				process->h_counter[process->last_state][index] += ticks - process->last_ticks;

		}
	}

	UNLOCK_SM;

	zabbix_log(LOG_LEVEL_DEBUG, "End of %s()", __function_name);
}

/******************************************************************************
 *                                                                            *
 * Function: ja_sleep_loop                                                    *
 *                                                                            *
 * Purpose: sleeping process                                                  *
 *                                                                            *
 * Parameters: sleeptime - [IN] required sleeptime, in seconds                *
 *                                                                            *
 * Author: Alexander Vladishev                                                *
 *                                                                            *
 ******************************************************************************/
void	ja_sleep_loop(int sleeptime)
{
#ifdef HAVE_FUNCTION_SETPROCTITLE
	extern unsigned char	process_type;
	const char		*process_type_string;
#endif

	if (0 >= sleeptime)
		return;

	sleep_remains = sleeptime;

	zabbix_log(LOG_LEVEL_DEBUG, "sleeping for %d seconds", sleep_remains);

	ja_update_selfmon_counter(ZBX_PROCESS_STATE_IDLE);

#ifdef HAVE_FUNCTION_SETPROCTITLE
	process_type_string = ja_get_process_type_string(process_type);
#endif

	do
	{
#ifdef HAVE_FUNCTION_SETPROCTITLE
		zbx_setproctitle("%s [sleeping for %d seconds]", process_type_string, sleep_remains);
#endif
		sleep(1);
	}
	while (0 < --sleep_remains);

	ja_update_selfmon_counter(ZBX_PROCESS_STATE_BUSY);
}

void	ja_wakeup()
{
	sleep_remains = 0;
}


void	ja_porcess_description(pid_t *threads, int threads_num ){

	const char		*__function_name = "ja_porcess_description";
	struct tm	*tm;
	time_t		now;
	int process_pid, p_type;
	int i;

	zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

	time(&now);
	tm = localtime(&now);

	if(0 == write_flag && (0 == tm->tm_hour || 4 == tm->tm_hour ||  8 == tm->tm_hour || 12 == tm->tm_hour
				|| 16 == tm->tm_hour || 20 == tm->tm_hour)){
		zabbix_log(LOG_LEVEL_INFORMATION, "jobarg_server[Version %s (revision %s)]", JOBARG_VERSION, JOBARG_REVISION);
		for(i=0;i< threads_num-1 ; i++){
			    	if(0 == i){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JAJOBNET;
			    	}else if(1 == i){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JARUN;
			    	}else if(2 == i){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JAJOB;
			    	}else if(3 <= i && (3 + CONFIG_JATRAPPER_FORKS -1) >= i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JATRAPPER;
			    	}else if((4 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JALOADER;
			    	}else if((5 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JABOOT;
			    	}else if((6 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JASNDMSG;
			    	}else if((7 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_SELFMON;
			    	}else if((8 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JAPURGE;
			    	}else if((9 + CONFIG_JATRAPPER_FORKS -1) == i ){
			    		process_pid = threads[i];
			    		p_type = JA_PROCESS_TYPE_JAABORT;
			    	}

	        zabbix_log(LOG_LEVEL_INFORMATION, "server #%d running [%s] pid:[%d] ",
	        	(i+1), ja_get_process_type_string(p_type) , process_pid);
	    }
	    write_flag = 1;

	 }else if( 1 == write_flag && (0 != tm->tm_hour && 4 != tm->tm_hour && 8 != tm->tm_hour && 12 != tm->tm_hour
				&& 16 != tm->tm_hour && 20 != tm->tm_hour))
		 write_flag = 0;

}
