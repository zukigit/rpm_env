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
#include "log.h"

#include "jacommon.h"
#include "jastr.h"
#include "jafile.h"
#include "jatcp.h"

#include <archive.h>
#include <archive_entry.h>
#include "jaarchive.h"

#define _FILE_OFFSET_BITS 64
#define TARBLOCKSIZE 512
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
int ja_archive_open(struct archive *a, void *client_data)
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
ssize_t ja_archive_read(struct archive * a, void *client_data,
                        const void **buf)
{
    ja_archive_data *data;
    ssize_t total, left, n;
    char *p;

    data = (ja_archive_data *) client_data;
    *buf = data->buf;
    total = 0;
    left = TARBLOCKSIZE;

    ja_tcp_timeout_set(data->fd, CONFIG_FCOPY_TIMEOUT);
    while (1) {
        p = data->buf + total;
        n = ZBX_TCP_READ(data->fd, p, left);
        if (n == ZBX_TCP_ERROR) {
            total = -1;
            zabbix_log(LOG_LEVEL_ERR, "In ja_tar_read() failed: %s",
                       strerror_from_system(zbx_sock_last_error()));
            break;
        }
        total = total + n;
        left = left - n;
        if (left == 0 || n == 0)
            break;
    }
    ja_tcp_timeout_set(data->fd, 0);

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
ssize_t ja_archive_write(struct archive * a, void *client_data,
                         const void *buf, size_t len)
{
    ja_archive_data *data;
    ssize_t total, left, n;
    char *p;

    data = (ja_archive_data *) client_data;
    total = 0;
    left = len;
    ja_tcp_timeout_set(data->fd, CONFIG_FCOPY_TIMEOUT);
    while (1) {
        p = (char *) buf + total;
        n = ZBX_TCP_WRITE(data->fd, p, left);
        if (n == ZBX_TCP_ERROR) {
            total = -1;
            zabbix_log(LOG_LEVEL_ERR, "In ja_tar_write() failed: %s",
                       strerror_from_system(zbx_sock_last_error()));
            break;
        }
        total = total + n;
        left = left - n;
        if (left == 0 || n == 0)
            break;
    }
    ja_tcp_timeout_set(data->fd, 0);

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
int ja_archive_close(struct archive *a, void *client_data)
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
int ja_archive_chksum(const char *dir, const char *filename,
                      json_object * json, ja_job_object * job)
{
#ifdef _WINDOWS
    int ret;
    HANDLE hFind;
    WIN32_FIND_DATA win32fd;
    char fullname[JA_MAX_STRING_LEN];
    wchar_t *wfullname;
    char *name, *chksum;
    const char *__function_name = "ja_archive_chksum";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    ret = FAIL;
    if (json == NULL || job == NULL)
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
        if (ja_match(name, filename) != 1) {
            zbx_free(name);
            continue;
        }
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
        json_object_object_add(json, name, json_object_new_string(chksum));
        zbx_free(chksum);
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
    const char *__function_name = "ja_archive_chksum";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    if (json == NULL || job == NULL)
        return FAIL;

    ret = FAIL;
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
        if (ja_match(de->d_name, filename) != 1)
            continue;
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
int ja_archive_add(struct archive *a, const char *filename,
                   const char *pathname)
{
    int ret, fd, len;
    struct archive_entry *entry;
    struct stat s;
    char buf[JA_MAX_DATA_LEN];
    char *fn;
    char pname[JA_MAX_STRING_LEN];
    const char *__function_name = "ja_archive_add";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() filename: %s, pathname: %s",
               __function_name, filename, pathname);

    if (zbx_stat(filename, &s) != 0)
        return FAIL;
#ifdef _WINDOWS
    fn = ja_utf8_to_acp(pathname);
    zbx_snprintf(pname, sizeof(pname), "%s", fn);
    zbx_free(fn);
#else
    fn = NULL;
    zbx_snprintf(pname, sizeof(pname), "%s", pathname);
#endif
    fd = zbx_open(filename, O_RDONLY | O_BINARY);
    if (fd < 0)
        return FAIL;

    ret = SUCCEED;
    entry = archive_entry_new();
    archive_entry_copy_stat(entry, &s);
    archive_entry_set_pathname(entry, pname);
    archive_write_header(a, entry);
    archive_entry_free(entry);

    while (1) {
        len = read(fd, buf, sizeof(buf));
        if (len < 0)
            ret = FAIL;
        if (len <= 0)
            break;
        if (len == archive_write_data(a, buf, len))
            continue;
        ret = FAIL;
        break;
    }
    close(fd);

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
int ja_archive_create(const char *dir, const char *filename, const int fd,
                      ja_job_object * job)
{
#ifdef _WINDOWS
    int ret;
    HANDLE hFind;
    WIN32_FIND_DATA win32fd;
    char fullname[JA_MAX_STRING_LEN];
    wchar_t *wfullname;
    char *name;
    ja_archive_data data;
    struct archive *a;
    const char *__function_name = "ja_archive_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    if (job == NULL)
        return FAIL;
    ret = FAIL;
    data.fd = fd;
    a = archive_write_new();
    archive_write_set_compression_none(a);
    archive_write_set_format_ustar(a);
    archive_write_set_bytes_per_block(a, TARBLOCKSIZE);
    archive_write_open(a, &data, ja_archive_open, ja_archive_write,
                       ja_archive_close);

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
        if (win32fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
            continue;
        name = zbx_unicode_to_utf8(win32fd.cFileName);
        if (ja_match(name, filename) != 1) {
            zbx_free(name);
            continue;
        }

        zbx_snprintf(fullname, sizeof(fullname), "%s%c%s", dir, JA_DLM,
                     name);
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() archive file: %s",
                   __function_name, name);
        if (ja_archive_add(a, fullname, name) == FAIL) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "can not add file %s", fullname);
            goto error;
        }
        zbx_free(name);
    } while (FindNextFile(hFind, &win32fd));

    ret = SUCCEED;
  error:
    archive_write_close(a);
    archive_write_finish(a);
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
    ja_archive_data data;
    struct archive *a;
    const char *__function_name = "ja_archive_create";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, filename: %s",
               __function_name, dir, filename);

    ret = FAIL;
    data.fd = fd;
    a = archive_write_new();
    archive_write_set_compression_none(a);
    archive_write_set_format_ustar(a);
    archive_write_set_bytes_per_block(a, TARBLOCKSIZE);
    archive_write_open(a, &data, ja_archive_open, ja_archive_write,
                       ja_archive_close);

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
        if (ja_match(de->d_name, filename) != 1)
            continue;
        zabbix_log(LOG_LEVEL_DEBUG, "In %s() archive file: %s",
                   __function_name, de->d_name);
        if (ja_archive_add(a, fullname, de->d_name) == FAIL) {
            zbx_snprintf(job->message, sizeof(job->message),
                         "can not add file %s", fullname);
            goto error;
        }
    }

    ret = SUCCEED;
  error:
    archive_write_close(a);
    archive_write_finish(a);
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
int ja_archive_extract(const int fd, char *dir, const int overwrite,
                       ja_job_object * job)
{
    int ret;
    ja_archive_data data;
    struct archive *a;
    struct archive_entry *entry;
    char *pathname;
    int r, opt;
    const char *__function_name = "ja_archive_extract";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() dir: %s, overwrite: %d",
               __function_name, dir, overwrite);

    if (job == NULL)
        return FAIL;

    ret = SUCCEED;
    if (overwrite == 1)
        opt = 0;
    else
        opt = ARCHIVE_EXTRACT_NO_OVERWRITE;

    data.fd = fd;
    if (chdir(dir) == -1)
        return FAIL;
    a = archive_read_new();
    archive_read_support_format_tar(a);
    archive_read_open(a, &data, ja_archive_open, ja_archive_read,
                      ja_archive_close);
    while (1) {
        r = archive_read_next_header(a, &entry);
        if (r == ARCHIVE_EOF)
            break;
        pathname = (char *) archive_entry_pathname(entry);
        if (ret == SUCCEED) {
            zabbix_log(LOG_LEVEL_DEBUG, "In %s() extract the file: %s",
                       __function_name, pathname);
            r = archive_read_extract(a, entry, opt);
        } else {
            r = archive_read_data_skip(a);
        }

        if (r != ARCHIVE_OK) {
            zbx_snprintf(job->message, sizeof(job->message), "%s: %s",
                         archive_error_string(a), pathname);
            ret = FAIL;
        }
    }

    archive_read_close(a);
    archive_read_finish(a);
    if (ret == FAIL)
        zabbix_log(LOG_LEVEL_ERR, "In %s() %s", __function_name,
                   job->message);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() end", __function_name);
    return ret;
}
