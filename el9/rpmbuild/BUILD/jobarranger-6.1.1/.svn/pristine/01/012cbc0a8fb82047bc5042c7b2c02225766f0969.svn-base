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


#ifndef JOBARG_JASELF_H
#define JOBARG_JASELF_H

#if defined(_WINDOWS)
#	error "This module allowed only for Unix OS"
#endif

#define JA_PROCESS_TYPE_JARUN		0
#define JA_PROCESS_TYPE_JATRAPPER	1
#define JA_PROCESS_TYPE_JAJOB		2
#define JA_PROCESS_TYPE_JAJOBNET	3
#define JA_PROCESS_TYPE_JALOADER	4
#define JA_PROCESS_TYPE_JABOOT		5
#define JA_PROCESS_TYPE_JASNDMSG	6
#define JA_PROCESS_TYPE_SELFMON		7
#define JA_PROCESS_TYPE_JAPURGE		8
#define JA_PROCESS_TYPE_JAABORT		9
#define JA_PROCESS_TYPE_COUNT		10	/* number of process types */
#define JA_PROCESS_TYPE_UNKNOWN		255


int	ja_get_process_type_forks(unsigned char process_type);
const char	*ja_get_process_type_string(unsigned char process_type);
void	ja_init_selfmon_collector();
void	ja_free_selfmon_collector();
void	ja_update_selfmon_counter(unsigned char state);
void	ja_collect_selfmon_stats();
void	ja_sleep_loop(int sleeptime);
void	ja_wakeup();

void	ja_porcess_description(pid_t *threads, int threads_num);

#endif	/* JA_SELF_H */
