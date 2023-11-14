/*
** Job Arranger for ZABBIX
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
** Copyright (C) 2021 Daiwa Institute of Research Ltd. All Rights Reserved.
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


#ifndef JOBARG_JAINDEX_H
#define JOBARG_JAINDEX_H

#define	JA_RUN_ID_JOBNET_LD		1
#define	JA_RUN_ID_JOBNET_EX		3
#define	JA_RUN_ID_JOB			20
#define	JA_RUN_ID_FLOW			30
#define	JA_RUN_ID_SESSION		40

zbx_uint64_t	get_next_id(int count_id, zbx_uint64_t inner_jobnet_id, char *jobnet_id, zbx_uint64_t inner_job_id);

#endif
