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


#include <json.h>
#include "common.h"
#include "log.h"

#include "jacommon.h"
#include "jajobobject.h"
#include "jaextjob.h"
#include "jaagent.h"
#include "jastr.h"


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
int ja_extjob_script(ja_job_object * job, char* datafile)
{
    int ret;
    json_object *jp_arg, *jp;
    int i;
    char *cmd;
    const char *__function_name = "ja_extjob_script";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() extjob script start!", __function_name);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    ret = FAIL;
    int changeToAcp = FALSE;
//added by DAT
#ifdef _WINDOWS
    UINT codePage = GetACP();
#endif
    //data filepath
    jp_arg = json_tokener_parse(job->argument);
    if (is_error(jp_arg)) {
        jp_arg = NULL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not parse job argument [%s]", job->argument);
        goto error;
    }

    if (json_object_get_type(jp_arg) != json_type_array) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "[%s] is not an array", job->argument);
        goto error;
    }
    //added by DAT
#ifdef _WINDOWS
    if((strcmp(job->script, "jafwait") == 0 || strcmp(job->script, "jafcheck") == 0) && codePage != 65001) {      //check codepage and ext job types
        changeToAcp = TRUE;
    }
#endif
    cmd =
        zbx_dsprintf(NULL, "\"%s%c%s.%s\"", CONFIG_EXTJOB_PATH, JA_DLM,
                     job->script, JA_EXE);
    zbx_snprintf(job->script, sizeof(job->script), "%s", cmd);
    zbx_free(cmd);
    
    for (i = 0; i < json_object_array_length(jp_arg); i++) {
        jp = json_object_array_get_idx(jp_arg, i);

        //added by DAT
        if(changeToAcp && i == 0) {     //0 index is file patch, changed it to acp 
            zabbix_log(LOG_LEVEL_INFORMATION, "In %s(), host is not using utf8 and changed utf8 to acp string", __function_name);
            cmd =
                zbx_dsprintf(NULL, "%s \"%s\"", job->script,
                            ja_utf8_to_acp(json_object_get_string(jp)));
        } else {
            cmd =
                zbx_dsprintf(NULL, "%s \"%s\"", job->script,
                            json_object_get_string(jp));
        }

        zbx_snprintf(job->script, sizeof(job->script), "%s", cmd);
        zbx_free(cmd);
    }
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() extjob script ended. Script is : %s", __function_name,job->script);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() start writing on data file \n. filepath is: %s", __function_name, datafile);
    if (write_data_file(job,datafile) == FAIL) {
        ret = FAIL;
        zabbix_log(LOG_LEVEL_ERR, "In %s() failed to write data on file.", __function_name);
    }
    ret = SUCCEED;

  error:
    json_object_put(jp_arg);
    return ret;


}
