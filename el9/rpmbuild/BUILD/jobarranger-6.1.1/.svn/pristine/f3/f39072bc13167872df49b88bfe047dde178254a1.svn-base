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


#ifndef JOBARG_JAJOBLOG_H
#define JOBARG_JAJOBLOG_H

#define	LOG_TYPE_INFO		0
#define	LOG_TYPE_WARN		1
#define	LOG_TYPE_ERR		2

#define	JC_JOBNET_START		"JC00000001"
#define	JC_JOBNET_END		"JC00000002"
#define	JC_JOB_START		"JC00000003"
#define	JC_JOB_END		"JC00000004"
#define	JC_JOB_TIMEOUT		"JC00000005"
#define	JC_JOB_SKIP		"JC00000006"
#define	JC_JOB_RERUN		"JC00000007"
#define	JC_JOBNET_TIMEOUT	"JC00000008"
#define	JC_JOBNET_START_ERR	"JC90000001"
#define	JC_JOB_ERR_END		"JC90000002"
#define	JC_JOBNET_ERR_END	"JC90000003"

int ja_joblog(char *message_id, zbx_uint64_t inner_jobnet_id, zbx_uint64_t inner_job_id);

#endif
