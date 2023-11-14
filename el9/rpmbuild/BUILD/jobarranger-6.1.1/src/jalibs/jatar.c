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
#include <libtar.h>

#include "common.h"
#include "comms.h"
#include "log.h"

#include "jacommon.h"
#include "jastr.h"
#include "jafile.h"
#include "jatcp.h"
#include "jatar.h"

extern int CONFIG_FCOPY_TIMEOUT;
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
ssize_t ja_tar_read(int fd, const void *buf, size_t len)
{
    ssize_t total, left, n;
    char *p, *q;

    q = (char *) buf;
    total = 0;
    left = len;
    ja_tcp_timeout_set(fd, CONFIG_FCOPY_TIMEOUT);
    while (1) {
        p = q + total;
        n = ZBX_TCP_READ(fd, p, left);
        if (n == ZBX_TCP_ERROR) {
            total = -1;
            zabbix_log(LOG_LEVEL_ERR, "ja_tar_read() failed: %s",
                       strerror_from_system(zbx_sock_last_error()));
            break;
        }
        total = total + n;
        left = left - n;
        if (left == 0 || n == 0)
            break;
    }
    ja_tcp_timeout_set(fd, 0);

    return total;
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
ssize_t ja_tar_write(int fd, void *buf, size_t len)
{
    ssize_t total, left, n;
    char *p, *q;

    q = (char *) buf;
    total = 0;
    left = len;
    ja_tcp_timeout_set(fd, CONFIG_FCOPY_TIMEOUT);
    while (1) {
        p = q + total;
        n = ZBX_TCP_WRITE(fd, p, left);
        if (n == ZBX_TCP_ERROR) {
            total = -1;
            zabbix_log(LOG_LEVEL_ERR, "ja_tar_write() failed: %s",
                       strerror_from_system(zbx_sock_last_error()));
            break;
        }
        total = total + n;
        left = left - n;
        if (left == 0 || n == 0)
            break;
    }
    ja_tcp_timeout_set(fd, 0);

    return total;
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
int ja_tar_close(int fd)
{
    return 0;
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
int ja_tar_chksum(const char *dir, const char *filename,
                  json_object * json, ja_job_object * job)
{
#ifdef _WINDOWS
    int ret;
    HANDLE hFind;
    WIN32_FIND_DATA win32fd;
    char fullname[JA_MAX_STRING_LEN];
    wchar_t *wfullname;
    char *name, *chksum;
    const char *__function_name = "ja_tar_chksum";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    ret = FAIL;
    if (json == NULL)
        return FAIL;
    if (job == NULL)
        return FAIL;

    zbx_snprintf(fullname, sizeof(fullname), "%s%c*", dir, JA_DLM);
    wfullname = zbx_utf8_to_unicode(fullname);
    hFind = FindFirstFile(wfullname, &win32fd);
    zbx_free(wfullname);
    if (hFind == INVALID_HANDLE_VALUE) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open dir: %s (%s)", dir, strerror(errno));
        goto error;
    }
    do {
        if (win32fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
            continue;
        }
        name = zbx_unicode_to_utf8(win32fd.cFileName);
        if (ja_match(name, filename) == 1) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() check sum file: %s",
                       __function_name, name);
            zbx_snprintf(fullname, sizeof(fullname), "%s%c%s", dir, JA_DLM,
                         name);
            chksum = ja_checksum(fullname);
            if (chksum == NULL) {
                zbx_snprintf(job->message, sizeof(job->message),
                             "can not make %s check sum", fullname);
                zbx_free(name);
                goto error;
            }
            json_object_object_add(json, name,
                                   json_object_new_string(chksum));
            zbx_free(chksum);
        }
        zbx_free(name);
    } while (FindNextFile(hFind, &win32fd));

    ret = SUCCEED;
  error:
    if (hFind != INVALID_HANDLE_VALUE)
        FindClose(hFind);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    return ret;
#else
    int ret;
    DIR *dp;
    struct dirent *de;
    char fullname[JA_MAX_STRING_LEN];
    char *chksum;
    const char *__function_name = "ja_tar_chksum";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    ret = FAIL;
    dp = NULL;
    if (json == NULL)
        return FAIL;
    if (job == NULL)
        return FAIL;

    dp = opendir(dir);
    if (dp == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open dir: %s (%s)", dir, strerror(errno));
        goto error;
    }

    while ((de = readdir(dp)) != NULL) {
        zbx_snprintf(fullname, sizeof(fullname), "%s%c%s", dir, JA_DLM,
                     de->d_name);
        if (ja_file_is_regular(fullname) == FAIL)
            continue;
        if (ja_match(de->d_name, filename) == 1) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() check sum file: %s",
                       __function_name, de->d_name);
            chksum = ja_checksum(fullname);
            if (chksum == NULL) {
                zbx_snprintf(job->message, sizeof(job->message),
                             "can not make %s check sum", fullname);
                goto error;
            }
            json_object_object_add(json, de->d_name,
                                   json_object_new_string(chksum));
            zbx_free(chksum);
        }
    }

    ret = SUCCEED;
  error:
    if (dp != NULL)
        closedir(dp);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    return ret;
#endif
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
int ja_tar_create(const char *dir, const char *filename, const int fd,
                  ja_job_object * job)
{
#ifdef _WINDOWS
    int ret;
    HANDLE hFind;
    WIN32_FIND_DATA win32fd;
    char fullname[JA_MAX_STRING_LEN];
    wchar_t *wfullname;
    char *name;
    TAR *tar;
    tartype_t tartype;
    char *fullname_acp, *name_acp;
    const char *__function_name = "ja_tar_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    if (job == NULL)
        return FAIL;
    ret = FAIL;
    tar = NULL;
    tartype.openfunc = open;
    tartype.closefunc = (closefunc_t) ja_tar_close;
    tartype.readfunc = (readfunc_t) ja_tar_read;
    tartype.writefunc = (writefunc_t) ja_tar_write;

    zbx_snprintf(fullname, sizeof(fullname), "%s%c*", dir, JA_DLM);
    wfullname = zbx_utf8_to_unicode(fullname);
    hFind = FindFirstFile(wfullname, &win32fd);
    zbx_free(wfullname);
    if (hFind == INVALID_HANDLE_VALUE) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open dir: %s (%s)", dir, strerror(errno));
        goto error;
    }

    if (tar_fdopen(&tar, fd, NULL, &tartype, O_WRONLY, 0644, 0) != 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open tar (%s)", strerror(errno));
        tar = NULL;
        goto error;
    }

    do {
        if (win32fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
            continue;
        }
        name = zbx_unicode_to_utf8(win32fd.cFileName);
        if (ja_match(name, filename) == 1) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() check sum file: %s",
                       __function_name, name);
            zbx_snprintf(fullname, sizeof(fullname), "%s%c%s", dir, JA_DLM,
                         name);
            fullname_acp = ja_utf8_to_acp(fullname);
            name_acp = ja_utf8_to_acp(name);
            if (tar_append_file(tar, fullname_acp, name_acp) != 0) {
                zbx_snprintf(job->message, sizeof(job->message),
                             "can not append file %s (%s)", fullname,
                             strerror(errno));
                zbx_free(name);
                zbx_free(fullname_acp);
                zbx_free(name_acp);
                goto error;
            }
            zbx_free(fullname_acp);
            zbx_free(name_acp);
        }
        zbx_free(name);
    } while (FindNextFile(hFind, &win32fd));
    tar_append_eof(tar);

    ret = SUCCEED;
  error:
    if (hFind != INVALID_HANDLE_VALUE)
        FindClose(hFind);
    if (tar != NULL)
        tar_close(tar);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    return ret;
#else
    int ret;
    TAR *tar;
    DIR *dp;
    struct dirent *de;
    tartype_t tartype;
    char fullname[JA_MAX_STRING_LEN];
    const char *__function_name = "ja_tar_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    ret = FAIL;
    tar = NULL;
    dp = NULL;
    tartype.openfunc = open;
    tartype.closefunc = (closefunc_t) ja_tar_close;
    tartype.readfunc = (readfunc_t) ja_tar_read;
    tartype.writefunc = (writefunc_t) ja_tar_write;

    if (tar_fdopen
        (&tar, fd, NULL, &tartype, O_WRONLY, 0644, TAR_IGNORE_CRC) != 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open tar (%s)", strerror(errno));
        tar = NULL;
        goto error;
    }

    dp = opendir(dir);
    if (dp == NULL) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open dir: %s (%s)", dir, strerror(errno));
        goto error;
    }

    while ((de = readdir(dp)) != NULL) {
        zbx_snprintf(fullname, sizeof(fullname), "%s%c%s", dir, JA_DLM,
                     de->d_name);
        if (ja_file_is_regular(fullname) == FAIL)
            continue;

        if (ja_match(de->d_name, filename) == 1) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() tar append file: %s",
                       __function_name, de->d_name);
            if (tar_append_file(tar, fullname, de->d_name) != 0) {
                zbx_snprintf(job->message, sizeof(job->message),
                             "can not append file %s (%s)", fullname,
                             strerror(errno));
                goto error;
            }
        }
    }
    tar_append_eof(tar);

    ret = SUCCEED;
  error:
    if (dp != NULL)
        closedir(dp);
    if (tar != NULL)
        tar_close(tar);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    return ret;
#endif
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
int ja_tar_extract(const int fd, char *dir, const int overwrite,
                   ja_job_object * job)
{
    int ret;
    int option;
    TAR *tar;
    char *destdir;
    tartype_t tartype;
    const char *__function_name = "ja_tar_extract";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, overwrite: %d",
               __function_name, dir, overwrite);

    if (job == NULL)
        return FAIL;
    ret = FAIL;
    tar = NULL;
    destdir = NULL;
    tartype.openfunc = open;
    tartype.closefunc = (closefunc_t) ja_tar_close;
    tartype.readfunc = (readfunc_t) ja_tar_read;
    tartype.writefunc = (writefunc_t) ja_tar_write;

    option = TAR_IGNORE_CRC;
    if (overwrite == 0)
        option |= TAR_NOOVERWRITE;
    if (tar_fdopen(&tar, fd, NULL, &tartype, O_RDONLY, 0, option) != 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not open tar (%s)", strerror(errno));
        tar = NULL;
        goto error;
    }
#ifdef _WINDOWS
    destdir = ja_utf8_to_acp(dir);
#else
    destdir = zbx_strdup(NULL, dir);
#endif
    if (tar_extract_all(tar, destdir) == -1) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "can not extract tar to dir: %s (%s)", dir,
                     strerror(errno));
        while (th_read(tar) == 0) {
            if (TH_ISREG(tar) && tar_skip_regfile(tar))
                break;
            continue;
        }
        goto error;
    }

    ret = SUCCEED;
  error:
    if (tar != NULL)
        tar_close(tar);
    if (destdir != NULL)
        zbx_free(destdir);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() end", __function_name);
    return ret;
}
