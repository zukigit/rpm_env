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
#include "md5.h"
#include "log.h"

#include "jacommon.h"
#include "jastr.h"

#ifdef _WINDOWS
#include "gnuregex.h"
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
char *ja_timestamp2str(const time_t time)
{
    static char buffer[15];
    struct tm *tmbuf;

    tmbuf = localtime(&time);
    zbx_snprintf(buffer, sizeof(buffer), "%.4d%.2d%.2d%.2d%.2d%.2d",
                 tmbuf->tm_year + 1900, tmbuf->tm_mon + 1, tmbuf->tm_mday,
                 tmbuf->tm_hour, tmbuf->tm_min, tmbuf->tm_sec);
    return buffer;
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
time_t ja_str2timestamp(const char *str)
{
    struct tm tmbuf;
    int year, mon, mday, hour, min, sec, maxday;
    static int day[] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    char t_str[15], buf[5];
    const char *__function_name = "ja_str2timestamp";

    switch (strlen(str)) {
    case 1:
        return 0;
        break;
    case 8:
        zbx_snprintf(t_str, sizeof(t_str), "%s000000", str);
        break;
    case 10:
        zbx_snprintf(t_str, sizeof(t_str), "%s0000", str);
        break;
    case 12:
        zbx_snprintf(t_str, sizeof(t_str), "%s00", str);
        break;
    case 14:
        zbx_snprintf(t_str, sizeof(t_str), "%s", str);
        break;
    default:
        goto error;
        break;
    }

    zbx_snprintf(buf, 5, "%s", t_str);
    year = atoi(buf);
    zbx_snprintf(buf, 3, "%s", t_str + 4);
    mon = atoi(buf);
    zbx_snprintf(buf, 3, "%s", t_str + 6);
    mday = atoi(buf);
    zbx_snprintf(buf, 3, "%s", t_str + 8);
    hour = atoi(buf);
    zbx_snprintf(buf, 3, "%s", t_str + 10);
    min = atoi(buf);
    zbx_snprintf(buf, 3, "%s", t_str + 12);
    sec = atoi(buf);

    maxday = day[mon - 1] + (mon == 2 && year % 4 == 0
                             && (year % 100 != 0 || year % 400 == 0));
    if (year < 1900 || mon < 1 || mon > 12)
        goto error;
    if (mday < 1 || mday > maxday)
        goto error;
    if (hour < 0 || hour > 23)
        goto error;
    if (min < 0 || min > 59)
        goto error;
    if (sec < 0 || sec > 59)
        goto error;
    tmbuf.tm_year = year - 1900;
    tmbuf.tm_mon = mon - 1;
    tmbuf.tm_mday = mday;
    tmbuf.tm_hour = hour;
    tmbuf.tm_min = min;
    tmbuf.tm_sec = sec;
    return mktime(&tmbuf);

  error:
    zabbix_log(LOG_LEVEL_WARNING, "In %s() unknown the time format: %s",
               __function_name, str);
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
int ja_format_timestamp(const char *input, char *output)
{
    int i, len;
    char *p, *q;

    len = strlen(input);
    if (len != 8 && len != 10 && len != 12 && len != 14 && len != 17)
        return FAIL;

    p = (char *) input;
    q = output;
    for (i = 0; i < len; i++) {
        if (i == 4 || i == 6)
            *q++ = '/';
        if (i == 8)
            *q++ = ' ';
        if (i == 10 || i == 12)
            *q++ = ':';
        if (i == 14)
            *q++ = '.';
        *q++ = *p++;
    }
    *q = '\0';
    return SUCCEED;
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
int ja_match(const char *string, const char *pattern)
{
    switch (*pattern) {
    case '\0':
        return '\0' == *string;
    case '*':
        return ja_match(string, pattern + 1)
            || (('\0' != *string) && ja_match(string + 1, pattern));
    case '?':
        return ('\0' != *string)
            && ja_match(string + 1, pattern + 1);
    default:
        return ((unsigned char) *pattern == (unsigned char) *string)
            && ja_match(string + 1, pattern + 1);
    }
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
int ja_regexp(const char *string, const char *pattern)
{
    int ret;
    regex_t re;
    regmatch_t match;
    char *s1, *s2, *s3;
    char new_pattern[JA_MAX_STRING_LEN];
    const char *__function_name = "ja_regexp";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() string: %s, pattern: %s",
               __function_name, string, pattern);
    if (string == NULL || pattern == NULL)
        return 1;

    s1 = string_replace(pattern, ".", "\\.");
    s2 = string_replace(s1, "*", ".*");
    s3 = string_replace(s2, "?", ".");
    zbx_snprintf(new_pattern, sizeof(new_pattern), "^%s$", s3);
    zbx_free(s1);
    zbx_free(s2);
    zbx_free(s3);

    if (regcomp(&re, new_pattern, REG_EXTENDED | REG_NEWLINE) != 0)
        return 0;

    if (0 == regexec(&re, string, (size_t) 1, &match, 0))
        ret = 1;
    else
        ret = 0;

    regfree(&re);
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
int ja_is_number(const char *str)
{
    if (NULL ==
        zbx_regexp_match(str, "^[-]{0,1}[0-9]+[.]{0,1}[0-9]*$", NULL)) {
        return 0;
    } else {
        return 1;
    }
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
int ja_number_comp(const double num, const char *range)
{
    int ret, flag;
    char *new_range, *p;
    double s, e;
    const char *__function_name = "ja_number_comp";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() num: %f, range: %s",
               __function_name, num, range);

    ret = -1;
    new_range = zbx_strdup(NULL, range);
    p = new_range + 1;
    flag = 0;

    while (*p != '\0') {
        if (*p == '-') {
            *p++ = '\0';
            flag = 1;
            break;
        }
        p++;
    }

    if (ja_is_number(new_range) != 1) {
        zabbix_log(LOG_LEVEL_WARNING, "%s is not a number", new_range);
        goto error;
    } else {
        s = atof(new_range);
    }
    if (flag == 1) {
        if (ja_is_number(p) != 1) {
            zabbix_log(LOG_LEVEL_WARNING, "%s is not a number", p);
            goto error;
        } else {
            e = atof(p);
            if (s > e) {
                zabbix_log(LOG_LEVEL_WARNING, "%f > %f is wrong", s, e);
                goto error;
            }
        }
        if (num >= s && num <= e) {
            ret = 1;
        } else {
            ret = 0;
        }
    } else {
        if (num == s)
            ret = 1;
        else
            ret = 0;
    }

  error:
    zbx_free(new_range);
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
int ja_number_match(const char *string, const char *pattern)
{
    int ret;
    char *str, *new_pattern;
    char *tp;
    double value;
    const char *__function_name = "ja_number_match";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    if (string == NULL || pattern == NULL) {
        return 0;
    }

    if (strlen(string) == 0 || strlen(pattern) == 0) {
        return 0;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() string: %s, pattern: %s",
               __function_name, string, pattern);

    str = string_replace(string, " ", "");
    new_pattern = string_replace(pattern, " ", "");
    if (strlen(str) == 0 || strlen(new_pattern) == 0) {
        ret = 0;
        goto exit;
    }

    if (ja_is_number(str) != 1) {
        zabbix_log(LOG_LEVEL_WARNING, "string: %s is not a number", str);
        ret = -1;
        goto exit;
    }

    ret = -1;
    value = atof(str);
    tp = strtok(new_pattern, ",");
    while (tp != NULL) {
        ret = ja_number_comp(value, tp);
        if (ret != 0)
            break;
        tp = strtok(NULL, ",");
    }

  exit:
    zbx_free(str);
    zbx_free(new_pattern);
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
int ja_format_expr(char *input, char *output)
{
    char *p, *q;
    const char *__function_name = "ja_format_expr";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() input: [%s]",
               __function_name, input);

    zbx_snprintf(output, JA_MAX_STRING_LEN, "expr ");
    p = input;
    q = output + strlen(output);
    while (*p != '\0') {
        switch (*p) {
        case '+':
        case '-':
        case '*':
        case '/':
        case '%':
        case '!':
        case '=':
        case '<':
        case '>':
        case '(':
        case ')':
        case '|':
        case '&':
        case ':':
        case ';':
        case '[':
        case ']':
        case '`':
            *q++ = '\\';
            break;
        default:
            break;
        }
        *q++ = *p++;
    }
    *q = '\0';

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() output: [%s]",
               __function_name, output);

    return SUCCEED;
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
int ja_format_date(char *input, char *output)
{
    int flag;
    char *p, *q;
    const char *__function_name = "ja_format_date";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() input: [%s]",
               __function_name, input);

    zbx_snprintf(output, JA_MAX_STRING_LEN, "date --date \"");
    p = input;
    q = output + strlen(output);

    flag = 0;
    while (*p != '\0') {
        if (flag == 0 && *p == ';') {
            flag = 1;
            *q++ = '"';
            *q++ = ' ';
            *q++ = '+';
            *q++ = '"';
            p++;
            continue;
        }
        switch (*p) {
        case '"':
        case '`':
        case '\'':
        case '>':
        case '<':
        case '&':
        case '\\':
            p++;
            break;
        default:
            *q++ = *p++;
            break;
        }
    }
    *q++ = '"';
    *q = '\0';

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() output: [%s]",
               __function_name, output);

    return SUCCEED;
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
int ja_format_extjob(char *input, char *output)
{
    char *p, *q;
    const char *__function_name = "ja_format_extjob";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() input: [%s]",
               __function_name, input);

    p = input;
    q = output;
    while (*p != '\0') {
        switch (*p) {
        case ';':
        case '`':
        case '>':
        case '<':
        case '&':
        case '|':
            p++;
            break;
        default:
            *q++ = *p++;
            break;
        }
    }
    *q = '\0';

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() output: [%s]",
               __function_name, output);

    return SUCCEED;

}

#ifdef _WINDOWS
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
LPSTR ja_unicode_to_acp(LPCTSTR wide_string)
{
    LPSTR acp_string = NULL;
    int acp_size;

    acp_size =
        WideCharToMultiByte(CP_ACP, 0, wide_string, -1, NULL, 0, NULL,
                            NULL);
    acp_string = (LPSTR) zbx_malloc(acp_string, (size_t) acp_size);

    /* convert from wide_string to acp_string */
    WideCharToMultiByte(CP_ACP, 0, wide_string, -1, acp_string, acp_size,
                        NULL, NULL);

    return acp_string;
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
LPSTR ja_utf8_to_acp(LPCSTR utf8_string)
{
    LPSTR acp_string = NULL;
    wchar_t *wide_string;

    wide_string = zbx_utf8_to_unicode(utf8_string);
    acp_string = ja_unicode_to_acp(wide_string);
    if (wide_string != NULL)
        zbx_free(wide_string);

    return acp_string;
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
LPSTR ja_acp_to_utf8(LPCSTR acp_string)
{
    LPSTR utf8_string = NULL;
    wchar_t *wide_string;

    wide_string = zbx_acp_to_unicode(acp_string);
    utf8_string = zbx_unicode_to_utf8(wide_string);
    if (wide_string != NULL)
        zbx_free(wide_string);

    return utf8_string;
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
char *ja_checksum(const char *filename)
{
    char *hash_text = NULL;
    md5_state_t state;
    md5_byte_t hash[MD5_DIGEST_SIZE];
    u_char buf[16 * ZBX_KIBIBYTE];
    int i, nbytes, f = -1;
    size_t sz;

    if (-1 == (f = zbx_open(filename, O_RDONLY | O_BINARY)))
        goto error;

    md5_init(&state);
    while (0 < (nbytes = (int) read(f, buf, sizeof(buf)))) {
        md5_append(&state, (const md5_byte_t *) buf, nbytes);
    }
    md5_finish(&state, hash);

    if (0 > nbytes)
        goto error;

    sz = MD5_DIGEST_SIZE * 2 + 1;
    hash_text = (char *) zbx_malloc(hash_text, sz);
    for (i = 0; i < MD5_DIGEST_SIZE; i++) {
        zbx_snprintf(&hash_text[i << 1], sz - (i << 1), "%02x", hash[i]);
    }

  error:
    if (-1 != f)
        close(f);

    return hash_text;
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
char *ja_md5(const char *str)
{
    char *hash_text = NULL;
    md5_state_t state;
    md5_byte_t hash[MD5_DIGEST_SIZE];
    int i;
    size_t sz;

    md5_init(&state);
    md5_append(&state, (const md5_byte_t *) str, strlen(str));
    md5_finish(&state, hash);

    sz = MD5_DIGEST_SIZE * 2 + 1;
    hash_text = (char *) zbx_malloc(hash_text, sz);
    for (i = 0; i < MD5_DIGEST_SIZE; i++) {
        zbx_snprintf(&hash_text[i << 1], sz - (i << 1), "%02x", hash[i]);
    }

    return hash_text;
}

/******************************************************************************
 *                                                                            *
 * Function: ja_check_number                                                  *
 *                                                                            *
 * Purpose: check that it is a number                                         *
 *                                                                            *
 * Parameters: data (in) - string to be checked                               *
 *                                                                            *
 * Return value:  SUCCEED - processed successfully                            *
 *                FAIL - detect incorrect data                                *
 *                                                                            *
 * Comments:                                                                  *
 *                                                                            *
 ******************************************************************************/
int ja_check_number(char *data)
{
    const char *__function_name = "ja_check_number";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s(%s)", __function_name, data);

    while ('\0' != *data) {
        if (0 == isdigit(*data)) {
            return FAIL;
        }
        data++;
    }
    return SUCCEED;
}
