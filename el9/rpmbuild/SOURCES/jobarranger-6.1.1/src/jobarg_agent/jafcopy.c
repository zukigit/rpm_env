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
#include "comms.h"
#include "threads.h"
#include "log.h"
#include "dirent.h"

#include "jacommon.h"
#include "jatelegram.h"
#include "jastr.h"
#include "jatar.h"
#include "jatcp.h"
#include "jathreads.h"
#include "jafcopy.h"

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
int ja_fcopy_begin(ja_job_object * job, zbx_sock_t * sock)
{
    const char *__function_name = "ja_fcopy_begin";

    zabbix_log(LOG_LEVEL_INFORMATION,
               "In %s() jobid: " ZBX_FS_UI64 ", method: %d",
               __function_name, job->jobid, job->method);

    switch (job->method) {
    case JA_AGENT_METHOD_NORMAL:
    case JA_AGENT_METHOD_TEST:
        if (strcmp(job->type, JA_PROTO_VALUE_PUTFILE) == 0) {
            job->status = JA_AGENT_STATUS_CLOSE;
            if (ja_fcopy_thread(job, sock) == FAIL)
                goto error;
        } else if (strcmp(job->type, JA_PROTO_VALUE_GETFILE) == 0) {
            if (ja_fcopy_thread(job, sock) == FAIL)
                goto error;
        } else {
            zbx_snprintf(job->message, sizeof(job->message),
                         "Invalid job type [%s]", job->type);
            goto error;
        }
        break;
    default:
        zbx_snprintf(job->message, sizeof(job->message),
                     "Invalid method: %d, jobid: " ZBX_FS_UI64,
                     job->method, job->jobid);
        goto error;
        break;
    }

    return SUCCEED;
  error:
    job->result = JA_RESPONSE_FAIL;
    zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name, job->message);
    if (ja_tcp_send_to(sock, job, CONFIG_TIMEOUT) == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s , Server IP %s ",
                   zbx_tcp_strerror(), job->serverip );
    zbx_tcp_unaccept(sock);
    return FAIL;
}

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
int ja_fcopy_thread(ja_job_object * job, zbx_sock_t * sock)
{
    ZBX_THREAD_HANDLE thread = ZBX_THREAD_HANDLE_NULL;
    ja_fcopy_args_t *fcopy_args;
    int ret;

    ret = SUCCEED;
    fcopy_args = NULL;
#ifdef _WINDOWS
    fcopy_args = (ja_fcopy_args_t *) malloc(sizeof(ja_fcopy_args_t));
    fcopy_args->job = (ja_job_object *) malloc(sizeof(ja_job_object));
    fcopy_args->s = (zbx_sock_t *) malloc(sizeof(zbx_sock_t));
    memcpy(fcopy_args->job, job, sizeof(ja_job_object));
    memcpy(fcopy_args->s, sock, sizeof(zbx_sock_t));
    if (strcmp(job->type, JA_PROTO_VALUE_PUTFILE) == 0) {
        thread =
            (ZBX_THREAD_HANDLE) _beginthreadex(NULL, 0,
                                               (unsigned
                                                int (__stdcall *) (void *))
                                               ja_fcopy_putfile_thread,
                                               fcopy_args, 0, NULL);
    } else if (strcmp(job->type, JA_PROTO_VALUE_GETFILE) == 0) {
        thread =
            (ZBX_THREAD_HANDLE) _beginthreadex(NULL, 0,
                                               (unsigned
                                                int (__stdcall *) (void *))
                                               ja_fcopy_getfile_thread,
                                               fcopy_args, 0, NULL);
    }
    sock->accepted = 0;
#else
    thread = ja_fork();
    if (thread == -1) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "fork() failed for jobid: " ZBX_FS_UI64
                     "[%s]", job->jobid, zbx_strerror(errno));
        return FAIL;
    } else if (thread != 0) {
        waitpid(thread, NULL, WNOHANG);
        zbx_sock_close(sock->socket);
        sock->socket = ZBX_SOCK_ERROR;
        sock->accepted = 0;
        return SUCCEED;
    }
    if (strcmp(job->type, JA_PROTO_VALUE_PUTFILE) == 0) {
        ret = ja_fcopy_putfile(job, sock);
    } else if (strcmp(job->type, JA_PROTO_VALUE_GETFILE) == 0) {
        ret = ja_fcopy_getfile(job, sock);
    }

    if (ret == SUCCEED)
        exit(0);
    else
        exit(1);
#endif
    return ret;
}



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
#ifdef _WINDOWS
unsigned __stdcall ja_fcopy_getfile_thread(ja_fcopy_args_t * args)
{
    int ret;
    const char *__function_name = "ja_fcopy_getfile_thread";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    ret = FAIL;
    if (args == NULL)
        goto error;
    if (args->job == NULL || args->s == NULL)
        goto error;
    if (ja_fcopy_getfile(args->job, args->s) == FAIL)
        goto error;

    ret = SUCCEED;
  error:
    if (args != NULL) {
        zbx_free(args->job);
        zbx_free(args->s);
        zbx_free(args);
    }
    if (ret == SUCCEED) {
        zbx_thread_exit(0);
    } else {
        zbx_thread_exit(1);
    }
}
#endif

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
int ja_fcopy_getfile(ja_job_object * job, zbx_sock_t * sock)
{
    int ret,count = 0,file_count = 0, chksum_size, written = 0,i = 0;
    char *fromdir, *filename, *chksum, *filtered_filename;
    char ret_str[JA_FILE_NAME_LEN];
    json_object *jp_fromdir, *jp_filename, *jp_arg, *jp_chksum;
    char fcopyflag;
    ja_file_object *file_list;
    const char *__function_name = "ja_fcopy_getfile";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() jobid: " ZBX_FS_UI64, __function_name, job->jobid);

    ret = FAIL;
    //initialize filtered file name;
    filtered_filename = (char *)zbx_malloc(NULL,(size_t)JA_FILE_PATH_LEN);
    fcopyflag = JA_FCOPY_FLAG_FAIL;
    file_list = (ja_file_object*)zbx_malloc(NULL, (size_t)JA_MAX_READ_LEN * sizeof(ja_file_object));
    jp_chksum = json_object_new_object();
    jp_arg = json_tokener_parse(job->argument);
    if (is_error(jp_arg)) {
        jp_arg = NULL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not parse job argument [%s]", job->argument);
        goto error;
    }
    if (json_object_get_type(jp_arg) != json_type_object) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "The job argument is not an object [%s]",
                     job->argument);
        goto error;
    }
    jp_fromdir = json_object_object_get(jp_arg, JA_PROTO_TAG_FROMDIR);
    if (jp_fromdir == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get from_directory from job argument [%s]",
                     job->argument);
        goto error;
    }
    fromdir = (char *) json_object_get_string(jp_fromdir);
    jp_filename = json_object_object_get(jp_arg, JA_PROTO_TAG_FILENAME);
    if (jp_filename == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get filename from job argument [%s]",
                     job->argument);
        goto error;
    }
    filename = (char *) json_object_get_string(jp_filename);
    
    // testing the function.
    file_count = ja_list_matchfiles(fromdir, filename,file_list);
    if(file_count == 0){
        zbx_snprintf(job->message, sizeof(job->message),"No file match the regex. [%s]",filename);
        goto error;
    }else if(file_count < 0){
        zbx_snprintf(job->message, sizeof(job->message),"file match the regex error. [%s]",filename);
        goto error;
    }
    //set timeout
    ja_tcp_timeout_set(sock->socket, CONFIG_FCOPY_TIMEOUT);
    //send file count to server(and hostn.)
    if (ZBX_TCP_ERROR ==
        ZBX_TCP_WRITE(sock->socket, &file_count, sizeof(int))) {
        zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",
                   zbx_tcp_strerror());
        goto error;
    }
    while(count<file_count){
        zbx_snprintf(filtered_filename,JA_FILE_PATH_LEN,"%s",(file_list+count)->filename);
        if (ja_tar_chksum(fromdir, filtered_filename, jp_chksum, job) == FAIL){
            zabbix_log(LOG_LEVEL_ERR, "In %s() check sum failed. file name : %s", __function_name,filtered_filename);
            goto error;
        }
        chksum = (char *) json_object_to_json_string(jp_chksum);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() check sum: %s", __function_name,
                   chksum);
        
        if (strlen(chksum) < 5) {
            fcopyflag = JA_FCOPY_FLAG_NOFILE;
            zabbix_log(LOG_LEVEL_ERR, "In %s() checksum size failed. file name : %s", __function_name,filtered_filename);
            zbx_snprintf(job->message, sizeof(job->message),
                         "Checksum creation failed for file : %s", filtered_filename);
            goto error;
        }

        fcopyflag = JA_FCOPY_FLAG_SUCCEED;
        if (ZBX_TCP_ERROR ==
            ZBX_TCP_WRITE(sock->socket, &fcopyflag, sizeof(char))) {
            zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",
                       zbx_tcp_strerror());
            goto error;
        }
        //send current file name
         if (ZBX_TCP_ERROR ==
            ZBX_TCP_WRITE(sock->socket, filtered_filename, JA_FILE_PATH_LEN)) {
            zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",
                       zbx_tcp_strerror());
            goto error;
        }
        if (ja_tar_create(fromdir, filtered_filename, sock->socket, job) == SUCCEED) {
            chksum_size = (int)strlen(chksum);
            if (chksum_size == 0 || ZBX_TCP_ERROR ==
            (i = ZBX_TCP_WRITE(sock->socket, &chksum_size,sizeof(int)))) {
                zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",zbx_tcp_strerror());
                goto error;
            }
            if (ZBX_TCP_ERROR ==ZBX_TCP_WRITE(sock->socket, chksum, chksum_size)) {
                zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",zbx_tcp_strerror());
                goto error;
            }
            
            //flush memory.
            if (jp_chksum != NULL)
                json_object_put(jp_chksum);
            jp_chksum = json_object_new_object();
        } else {
            zabbix_log(LOG_LEVEL_ERR, "Can not send the tar file");
            break;
        }
        count++;
    }
    ret = SUCCEED;

  error:
    zbx_free(filtered_filename);
    zbx_free(file_list);
    if (jp_chksum != NULL)
        json_object_put(jp_chksum);
    if (jp_arg != NULL)
        json_object_put(jp_arg);
    if (ret == FAIL) {
        //set file count as 0 to  make works done.
        if(file_count <= 0){   
            zabbix_log(LOG_LEVEL_ERR, "[Delete] In %s() file count failed. :", __function_name);
            file_count = 0;
            if (ZBX_TCP_ERROR ==
                ZBX_TCP_WRITE(sock->socket, &file_count, sizeof(int))) {
                zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_WRITE() failed: %s , Server IP %s",
                        zbx_tcp_strerror(), job->serverip );
            }
        }else{
            if(fcopyflag != JA_FCOPY_FLAG_SUCCEED){
                zabbix_log(LOG_LEVEL_ERR, "[Delete] In %s() fcopy flag failed. :", __function_name);
                if (ZBX_TCP_ERROR ==
                ZBX_TCP_WRITE(sock->socket, &fcopyflag, sizeof(char))) {
                zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_WRITE() failed: %s , Server IP %s",
                        zbx_tcp_strerror(), job->serverip );
                }
            }
        }
        if (zbx_tcp_send_to(sock, job->message, CONFIG_TIMEOUT) == FAIL) {
            zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s, Server IP %s ",
                       zbx_tcp_strerror(), job->serverip );
        }
    }else{
        zbx_snprintf(ret_str,sizeof(int),"%d",ret);
        if (zbx_tcp_send_to(sock, ret_str, CONFIG_TIMEOUT) == FAIL) {
            zabbix_log(LOG_LEVEL_ERR, "TCP send to error: %s",zbx_tcp_strerror());
            goto error;
        }    
    }
    
    zbx_tcp_close(sock);

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() END", __function_name);
    return ret;
}

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
#ifdef _WINDOWS
unsigned __stdcall ja_fcopy_putfile_thread(ja_fcopy_args_t * args)
{
    int ret;
    const char *__function_name = "ja_fcopy_putfile_thread";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    ret = FAIL;
    if (args == NULL)
        goto error;
    if (args->job == NULL || args->s == NULL)
        goto error;
    if (ja_fcopy_putfile(args->job, args->s) == FAIL)
        goto error;

    ret = SUCCEED;
  error:
    if (args != NULL) {
        zbx_free(args->job);
        zbx_free(args->s);
        zbx_free(args);
    }
    if (ret == SUCCEED) {
        zbx_thread_exit(0);
    } else {
        zbx_thread_exit(1);
    }
}
#endif

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
int ja_fcopy_putfile(ja_job_object * job, zbx_sock_t * sock)
{
    int ret,file_count,count,getfile_ret,chksum_size;
    json_object *jp_arg;
    json_object *jp_todir, *jp_overwrite;
    int overwrite;
    char *todir, *data, *msg;
    ssize_t nbytes;
    struct stat buf;
    char fcopyflag;
    char * char_filecount, *getfile_ret_str;
    const char *__function_name = "ja_fcopy_putfile";

    zabbix_log(LOG_LEVEL_DEBUG,
               "In %s() jobid: " ZBX_FS_UI64, __function_name, job->jobid);

    ret = FAIL;
    jp_arg = json_tokener_parse(job->argument);
    if (is_error(jp_arg)) {
        jp_arg = NULL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not parse job argument [%s]", job->argument);
        goto error;
    }
    if (json_object_get_type(jp_arg) != json_type_object) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "The job argument is not an object [%s]",
                     job->argument);
        goto error;
    }

    jp_todir = json_object_object_get(jp_arg, JA_PROTO_TAG_TODIR);
    if (jp_todir == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get from_directory from job argument [%s]",
                     job->argument);
        goto error;
    }
    todir = (char *) json_object_get_string(jp_todir);

    jp_overwrite = json_object_object_get(jp_arg, JA_PROTO_TAG_OVERWRITE);
    if (jp_overwrite == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not get overwrite from job argument [%s]",
                     job->argument);
        goto error;
    }
    overwrite = json_object_get_int(jp_overwrite);

    if (zbx_stat(todir, &buf) != 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not stat: %s (%s)", todir, strerror(errno));
        goto error;
    } else if (!S_ISDIR(buf.st_mode)) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "%s is not a directory.", todir);
        goto error;
    }

    job->result = JA_RESPONSE_SUCCEED;
    if (ja_tcp_send_to(sock, job, CONFIG_TIMEOUT) == FAIL)
        goto error;

    ja_tcp_timeout_set(sock->socket, CONFIG_FCOPY_TIMEOUT);
    //get file count first.
    nbytes = ZBX_TCP_READ(sock->socket, &file_count, sizeof(int));
    if (nbytes <= 0) {
        zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_READ error: %s",
                   zbx_tcp_strerror());
        goto error;
    }

    if(file_count < 1){
        //No matching file.
        zabbix_log(LOG_LEVEL_ERR, "[Delete] file empty : %s",char_filecount);
        zbx_snprintf(job->message, sizeof(job->message),"No file recieved.");
        goto error;
    }
    // loop all files.
    count = 0;
    while (count < file_count){
        fcopyflag = JA_FCOPY_FLAG_FAIL;
        nbytes = ZBX_TCP_READ(sock->socket, &fcopyflag, sizeof(char));
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() nbytes: %d, fcopyflag: %c",
                   __function_name, nbytes, fcopyflag);
        if (nbytes <= 0) {
            zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_READ error: %s",
                       zbx_tcp_strerror());
            goto error;
        }
        
        if (fcopyflag == JA_FCOPY_FLAG_SUCCEED) {

            json_object *ja_filename;
            char *filename,fullname[JA_MAX_STRING_LEN];
            /*
            ja_filename = json_object_object_get(jp_arg, JA_PROTO_TAG_FILENAME);
            filename = (char *) json_object_get_string(ja_filename);
            */
            nbytes = ZBX_TCP_READ(sock->socket, &filename,JA_FILE_PATH_LEN);
            if (nbytes <= 0) {
                zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_READ error: %s",
                        zbx_tcp_strerror());
                goto error;
            }
            
            zbx_snprintf(fullname,sizeof(fullname),"%s%s",todir,&filename);
            zabbix_log(LOG_LEVEL_DEBUG,"In %s()  overwrite[%d] todir[%s] filename[%s] full[%s] ", __function_name, overwrite,todir,&filename,fullname);

            if (zbx_stat(fullname, &buf) == 0 && overwrite == 0){
                zbx_snprintf(job->message, sizeof(job->message),
                             "%s cannot be overwritten. Please check the overwrite.", fullname);
                goto error;
            }

            if (ja_tar_extract(sock->socket, todir, overwrite, job) == FAIL) {
                zbx_tcp_recv_to(sock, &data, CONFIG_TIMEOUT);
                goto error;
            }
                
            nbytes = ZBX_TCP_READ(sock->socket, &chksum_size,sizeof(int));
            if (nbytes <= 0) {
                zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_READ error: %s",
                        zbx_tcp_strerror());
                goto error;
            }

            nbytes = ZBX_TCP_READ(sock->socket, &data, chksum_size);
            if (nbytes <= 0) {
                zabbix_log(LOG_LEVEL_ERR, "ZBX_TCP_READ error: %s",
                        zbx_tcp_strerror());
                goto error;
            }
            if (fcopyflag == JA_FCOPY_FLAG_SUCCEED) {
                if (ja_fcopy_putfile_chksum(&data, todir, job) == FAIL)
                    goto error;
            } else {
                goto error;
            }

        }else{
            zabbix_log(LOG_LEVEL_ERR, "In %s(), fcopy flag failed.",__function_name);
            goto error;
        }
        
        count++;
    }
    
    ret = SUCCEED;
  error:
    if(fcopyflag != JA_FCOPY_FLAG_SUCCEED || file_count <1){
        if (zbx_tcp_recv_to(sock, &msg, CONFIG_TIMEOUT) == ZBX_TCP_ERROR) {
            zabbix_log(LOG_LEVEL_ERR, "TCP recv to error: %s",zbx_tcp_strerror());
        }
        zbx_snprintf(job->message, sizeof(job->message), "%s", msg);
    }else{
        if (zbx_tcp_recv_to(sock, &getfile_ret_str, CONFIG_TIMEOUT) == ZBX_TCP_ERROR) {
            zabbix_log(LOG_LEVEL_ERR, "TCP recv to error: %s",
                        zbx_tcp_strerror());
        }
        getfile_ret = strtol(getfile_ret_str,NULL,10);
        if(getfile_ret == FAIL)
            ret = FAIL;
    }
    
    if (jp_arg != NULL)
        json_object_put(jp_arg);

    if (ret == FAIL) {
        job->result = JA_RESPONSE_FAIL;
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s. Servere IP %s ", __function_name,
                   job->message, job->serverip );
    } else {
        job->result = JA_RESPONSE_SUCCEED;
    }
    ja_tcp_send_to(sock, job, CONFIG_TIMEOUT);
    zbx_tcp_close(sock);

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() END", __function_name);
    return ret;
}

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
int ja_fcopy_putfile_chksum(char *chksum, char *dir, ja_job_object * job)
{
    int ret;
    json_object *jp_chksum;
    struct lh_entry *entry;
    char *key, *fullname;
    struct json_object *val;
    char *new_chksum, *org_chksum;
    const char *__function_name = "ja_fcopy_putfile_chksum";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() chksum: %s, dir: %s",
               __function_name, chksum, dir);

    ret = FAIL;
    fullname = NULL;
    jp_chksum = json_tokener_parse(chksum);
    if (is_error(jp_chksum)) {
        jp_chksum = NULL;
        zbx_snprintf(job->message, sizeof(job->message),
                     "Can not parse check sum data [%s]", chksum);
        goto error;
    }

    if (!json_object_is_type(jp_chksum, json_type_object)) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "chksum is not json object", chksum);
        goto error;
    }

    for (entry = json_object_get_object(jp_chksum)->head;
         (entry
          ? (key = (char *) entry->k, val =
             (struct json_object *) entry->v, entry) : 0);
         entry = entry->next) {
        org_chksum = (char *) json_object_get_string(val);
        zabbix_log(LOG_LEVEL_DEBUG, "check sum %s: %s", key, org_chksum);
        fullname = zbx_dsprintf(fullname, "%s%c%s", dir, JA_DLM, key);
        new_chksum = ja_checksum(fullname);
        if (new_chksum == NULL) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "can not get check sum. %s", fullname);
            goto error;
        }
        if (strcmp(org_chksum, new_chksum) != 0) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "File %s checksum %s can not match %s", key,
                         org_chksum, new_chksum);
            zbx_free(new_chksum);
            goto error;
        }
        zbx_free(new_chksum);
        zbx_free(fullname);
        fullname = NULL;
    }

    ret = SUCCEED;
  error:
    if (jp_chksum != NULL)
        json_object_put(jp_chksum);
    zbx_free(fullname);
    return ret;
}

/************************************************************************************************
*ja_list_matchfiles() 
*       - List  files with matching name as given regexp pattern.
*       - Retrun file count.
************************************************************************************************/

int ja_list_matchfiles(char *directory, char *regexp_str, ja_file_object *file_list){
    int file_count = 0;
    //dir
    DIR *dir;
    struct stat buff;
    struct dirent *file;
    char file_name[JA_FILE_PATH_LEN];
    char fullpath[JA_FILE_PATH_LEN];
    char * format_regexp_str;

    const char * __function_name = "ja_list_matchfiles";
    format_regexp_str = ja_fcopy_regex_pattern_format(regexp_str);
    dir = opendir(directory);
    if(dir == NULL){
        zabbix_log(LOG_LEVEL_ERR, "In %s(), Cannot open directory : %s",__function_name,strerror(errno));
        file_count = -1;
        goto end;
    }

    while((file = readdir(dir)) != NULL){
        
        if(file->d_name == NULL){
            zabbix_log(LOG_LEVEL_ERR, "In %s(), Cannot get file name : %s",__function_name,strerror(errno));
            closedir(dir);
            file_count = -1;
            goto end;
        }
        zbx_snprintf(file_name,sizeof(file_name),"%s",file->d_name);

        zbx_snprintf(fullpath,sizeof(fullpath),"%s%c%s",directory,JA_DLM,file_name);
        if(0 != stat(fullpath,&buff)){
            zabbix_log(LOG_LEVEL_DEBUG,"In %s(), %s file status check failed.",__function_name,fullpath);
            continue;
        }
        if( S_ISREG( buff.st_mode ) == 0 )
        {
            zabbix_log(LOG_LEVEL_DEBUG,"In %s(), %s is not a file.",__function_name,file_name);
            continue;
        }

        if(strcmp(file_name,".") != 0 && strcmp(file_name,"..") !=0 && ja_regexp(file_name,format_regexp_str) == 1){
            zabbix_log(LOG_LEVEL_DEBUG,"In %s(), file matched : %s",__function_name,file_name);
			zbx_snprintf((file_list+file_count)->filename, sizeof(file_list->filename), "%s", file_name);
            file_count++;
        }
    }
    
    closedir(dir);
end:
    zbx_free(format_regexp_str);
    return file_count;
}
