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
#include "daemon.h"
#include "zbxself.h"
#include "log.h"

#include "jacommon.h"
#include "jaself.h"
#include "jaselfmon.h"
extern unsigned char process_type;

void main_jaselfmon_loop(pid_t *threads, int threads_num)
{
    const char *__function_name = "main_jaselfmon_loop";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    for (;;) {
        zbx_setproctitle("%s [processing data]",
                         ja_get_process_type_string(process_type));

        ja_collect_selfmon_stats();
        ja_sleep_loop(CONFIG_JASELFMON_INTERVAL);
        ja_porcess_description(threads,threads_num);
        
    }
}
