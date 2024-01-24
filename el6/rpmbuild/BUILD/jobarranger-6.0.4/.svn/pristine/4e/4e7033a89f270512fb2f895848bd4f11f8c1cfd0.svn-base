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
#include "jajobobject.h"
#include "jatelegram.h"
#include "jatcp.h"

#ifndef ZBX_SOCKLEN_T
#define ZBX_SOCKLEN_T socklen_t
#endif

#ifdef HAVE_IPV6
#define ZBX_SOCKADDR struct sockaddr_storage
#else
#define ZBX_SOCKADDR struct sockaddr_in
#endif

#ifdef _WINDOWS
#define EINTR       WSAEINTR
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
int ja_tcp_accept_listener(zbx_sock_t * s)
{
    ZBX_SOCKLEN_T nlen;
    const char *__function_name = "ja_tcp_accept";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    nlen = sizeof(s->serverAddr);
    if (ZBX_SOCK_ERROR ==
        (s->socket =
         (ZBX_SOCKET) accept(s->sockets[0], (struct sockaddr *) &s->serverAddr,
                             &nlen))) {
        if (zbx_sock_last_error() == EINTR)
            return SUCCEED;
        zabbix_log(LOG_LEVEL_WARNING, "accept() failed: %s %d",
                   strerror_from_system(zbx_sock_last_error()),
                   zbx_sock_last_error());

        return FAIL;
    }

    s->socket_orig = ZBX_SOCK_ERROR;
    s->accepted = 1;

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
int ja_tcp_accept(zbx_sock_t * s)
{
    ZBX_SOCKADDR serv_addr;
    ZBX_SOCKLEN_T nlen;
    const char *__function_name = "ja_tcp_accept";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    zbx_tcp_unaccept(s);
    nlen = sizeof(serv_addr);
    if (ZBX_SOCK_ERROR ==
        (s->socket =
         (ZBX_SOCKET) accept(s->sockets[0], (struct sockaddr *) &serv_addr,
                             &nlen))) {
        if (zbx_sock_last_error() == EINTR)
            return SUCCEED;
        zabbix_log(LOG_LEVEL_WARNING, "accept() failed: %s %d",
                   strerror_from_system(zbx_sock_last_error()),
                   zbx_sock_last_error());

        return FAIL;
    }

    s->socket_orig = ZBX_SOCK_ERROR;
    s->accepted = 1;

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
int ja_tcp_send_to(zbx_sock_t * s, ja_job_object * job, int timeout)
{
    int ret;
    char *data;
    const char *__function_name = "ja_tcp_send_to";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    data = ja_telegram_to(job);
    if (data == NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() message: %s", __function_name,
                   job->message);
        return FAIL;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() %s", __function_name, data);
    ret = zbx_tcp_send_to(s, data, timeout);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   zbx_tcp_strerror());
    }
    zbx_free(data);
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
int ja_tcp_recv_to(zbx_sock_t * s, ja_job_object * job, int timeout)
{
    int ret, cnt=0;
    char *data;
    const char *__function_name = "ja_tcp_recv_to";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);
    // 2016/01/07 Park.iggy ADD
    /* ORG
    ret = zbx_tcp_recv_to(s, &data, timeout);
    if (ret == FAIL) {
        zbx_snprintf(job->message, sizeof(job->message), "%s",
                     zbx_tcp_strerror());
        goto error;
    }
    */

    while((ret = zbx_tcp_recv_to(s, &data, timeout+15)) == FAIL){
    	if(cnt >= 1)
    		break;
    	cnt++;
    }

    if (ret == FAIL) {
    	zbx_snprintf(job->message, sizeof(job->message), "%s",
                     zbx_tcp_strerror());
        goto error;
    }
    //Park.iggy END

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() %s", __function_name, data);
    if (strlen(data) == 0) {
        zbx_snprintf(job->message, sizeof(job->message),
                     "received data is null");
		ret = RECEIVED_NULL;
        goto error;
    }

    zbx_rtrim(data, "\r\n");
    ret = ja_telegram_from(data, job);

  error:
    if (ret == FAIL || ret == RECEIVED_NULL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() message: %s", __function_name,
                   job->message); 
    }
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
void ja_tcp_timeout_set(int fd, int timeout)
{
#ifdef _WINDOWS
    timeout *= 1000;

    if (ZBX_TCP_ERROR ==
        setsockopt(fd, SOL_SOCKET, SO_RCVTIMEO, (const char *) &timeout,
                   sizeof(timeout)))
        zabbix_log(LOG_LEVEL_ERR, "setsockopt() failed.");

    if (ZBX_TCP_ERROR ==
        setsockopt(fd, SOL_SOCKET, SO_SNDTIMEO, (const char *) &timeout,
                   sizeof(timeout)))
        zabbix_log(LOG_LEVEL_ERR, "setsockopt() failed.");
#else
    alarm(timeout);
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
int ja_tcp_send(zbx_sock_t * s, int timeout, json_object * json)
{
    int ret;
    char *data;
    const char *__function_name = "ja_tcp_send";

    if (json == NULL)
        return FAIL;
    data = (char *) json_object_to_json_string(json);
    zabbix_log(LOG_LEVEL_DEBUG, "In %s() data: %s", __function_name, data);

    ret = zbx_tcp_send_to(s, data, timeout);
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   zbx_tcp_strerror());
    }

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
int ja_tcp_recv(zbx_sock_t * s, int timeout, json_object * json)
{
    int ret;
    char *data, *err;
    json_object *jp;
    const char *__function_name = "ja_tcp_recv";

    zabbix_log(LOG_LEVEL_DEBUG, "In %s()", __function_name);

    ret = FAIL;
    jp = NULL;
    err = NULL;
    if (zbx_tcp_recv_to(s, &data, timeout) == FAIL) {
        err = zbx_dsprintf(NULL, "%s", zbx_tcp_strerror());
        goto error;
    }

    zabbix_log(LOG_LEVEL_DEBUG, "In %s() data: %s", __function_name, data);
    if (strlen(data) == 0) {
        err = zbx_dsprintf(NULL, "received data is null");
        goto error;
    }

    jp = json_tokener_parse(data);
    if (is_error(jp)) {
        err = zbx_dsprintf(NULL, "can not parse json data: %s", data);
        goto error;
    }

    json_object_object_add(json, "data", jp);
    ret = SUCCEED;
  error:
    if (ret == FAIL) {
        zabbix_log(LOG_LEVEL_ERR, "In %s() error: %s", __function_name,
                   err);
        json_object_object_add(json, "data", json_object_new_string(err));
    }
    if (err != NULL)
        zbx_free(err);
    return ret;
}

/******************************************************************************
 *                                                                            *
 * Function: check_security                                                   *
 *                                                                            *
 * Purpose: check if connection initiator is in list of IP addresses          *
 *                                                                            *
 * Parameters: sockfd - socket descriptor                                     *
 *             ip_list - comma-delimited list of IP addresses                 *
 *             allow_if_empty - allow connection if no IP given               *
 *             socket_ip -recived server IP address                           *
 *                                                                            *
 * Return value: SUCCEED - connection allowed                                 *
 *               FAIL - connection is not allowed                             *
 *                                                                            *
 * Author: Alexei Vladishev, Dmitry Borovikov                                 *
 *                                                                            *
 * Comments: standard, compatible and IPv4-mapped addresses are treated       *
 *           the same: 127.0.0.1 == ::127.0.0.1 == ::ffff:127.0.0.1           *
 *                                                                            *
 ******************************************************************************/
int	ja_tcp_check_security(zbx_sock_t *s, const char *ip_list, int allow_if_empty, char *socket_ip)
{
#if defined(HAVE_IPV6)
	struct addrinfo	hints, *ai = NULL;
	/* Network Byte Order is ensured */
	unsigned char	ipv4_cmp_mask[12] = {0};				/* IPv4-Compatible, the first 96 bits are zeros */
	unsigned char	ipv4_mpd_mask[12] = {0,0,0,0,0,0,0,0,0,0,255,255};	/* IPv4-Mapped, the first 80 bits are zeros, 16 next - ones */
#else
	struct hostent	*hp;
	char		*sip;
	int		i[4], j[4],k=0,kk=0;
#endif
	ZBX_SOCKADDR	name;
	ZBX_SOCKLEN_T	nlen;
	const char *__function_name = "ja_tcp_check_security";
	char		tmp[MAX_STRING_LEN], sname[MAX_STRING_LEN], *start = NULL, *end = NULL, p_tmp[MAX_STRING_LEN];

	if (1 == allow_if_empty && (NULL == ip_list || '\0' == *ip_list)){
        struct sockaddr_in *sockaddr = (struct sockaddr_in *)&s->serverAddr;
        if(sockaddr->sin_family == AF_INET){
            inet_ntop(AF_INET, &(sockaddr->sin_addr), socket_ip, INET_ADDRSTRLEN);
            goto success;
        } else if(sockaddr->sin_family == AF_INET6) {
            inet_ntop(AF_INET6, &(sockaddr->sin_addr), socket_ip, INET_ADDRSTRLEN);
            goto success;
        }
        zabbix_log(LOG_LEVEL_ERR, "In %s(), Unknown address family and returned FAIL.", __function_name);
        return FAIL;
    success:
        return SUCCEED;
    }

	nlen = sizeof(name);

	if (ZBX_TCP_ERROR == getpeername(s->socket, (struct sockaddr *)&name, &nlen))
	{
		zabbix_log(LOG_LEVEL_ERR,"connection rejected, getpeername() failed: %s", strerror_from_system(zbx_sock_last_error()));
		return FAIL;
	}
	else
	{
#if !defined(HAVE_IPV6)
		zbx_strlcpy(sname, inet_ntoa(name.sin_addr), sizeof(sname));

		if (4 != sscanf(sname, "%d.%d.%d.%d", &i[0], &i[1], &i[2], &i[3]))
			return FAIL;
#endif

		strscpy(p_tmp,ip_list);

		while(p_tmp[k] != '\0'){
			if (!isspace(p_tmp[k])) {
				tmp[kk++] = p_tmp[k];
			}
			k++;
		}
		tmp[kk] = '\0';

		//strscpy(tmp,ip_list);



		for (start = tmp; '\0' != *start;)
		{

			if (NULL != (end = strchr(start, ',')))
				*end = '\0';

			/* allow IP addresses or DNS names for authorization */
#if defined(HAVE_IPV6)
			memset(&hints, 0, sizeof(hints));
			hints.ai_family = PF_UNSPEC;
			if (0 == getaddrinfo(start, NULL, &hints, &ai))
			{
#ifdef HAVE_SOCKADDR_STORAGE_SS_FAMILY
				if (ai->ai_family == name.ss_family)
#else
				if (ai->ai_family == name.__ss_family)
#endif
				{
					switch (ai->ai_family)
					{
						case AF_INET  :
							if (((struct sockaddr_in*)&name)->sin_addr.s_addr == ((struct sockaddr_in*)ai->ai_addr)->sin_addr.s_addr)
							{
								freeaddrinfo(ai);
								return SUCCEED;
							}
							break;
						case AF_INET6 :
							if (0 == memcmp(((struct sockaddr_in6*)&name)->sin6_addr.s6_addr,
									((struct sockaddr_in6*)ai->ai_addr)->sin6_addr.s6_addr,
									sizeof(struct in6_addr)))
							{
								freeaddrinfo(ai);
								return SUCCEED;
							}
							break;
					}
				}
				else
				{
					switch (ai->ai_family)
					{
						case AF_INET  :
							/* incoming AF_INET6, must see whether it is comp or mapped */
							if((0 == memcmp(((struct sockaddr_in6*)&name)->sin6_addr.s6_addr, ipv4_cmp_mask, 12) ||
								0 == memcmp(((struct sockaddr_in6*)&name)->sin6_addr.s6_addr, ipv4_mpd_mask, 12)) &&
								0 == memcmp(&((struct sockaddr_in6*)&name)->sin6_addr.s6_addr[12],
									(unsigned char*)&((struct sockaddr_in*)ai->ai_addr)->sin_addr.s_addr, 4))
							{
								freeaddrinfo(ai);
								return SUCCEED;
							}
							break;
						case AF_INET6 :
							/* incoming AF_INET, must see whether the given is comp or mapped */
							if((0 == memcmp(((struct sockaddr_in6*)ai->ai_addr)->sin6_addr.s6_addr, ipv4_cmp_mask, 12) ||
								0 == memcmp(((struct sockaddr_in6*)ai->ai_addr)->sin6_addr.s6_addr, ipv4_mpd_mask, 12)) &&
								0 == memcmp(&((struct sockaddr_in6*)ai->ai_addr)->sin6_addr.s6_addr[12],
									(unsigned char*)&((struct sockaddr_in*)&name)->sin_addr.s_addr, 4))
							{
								freeaddrinfo(ai);
								return SUCCEED;
							}
							break;
					}
				}
				freeaddrinfo(ai);
			}
#else
			if (NULL != (hp = gethostbyname(start)))
			{
				sip = inet_ntoa(*((struct in_addr *)hp->h_addr));

				if (4 == sscanf(sip, "%d.%d.%d.%d", &j[0], &j[1], &j[2], &j[3]) &&
						i[0] == j[0] && i[1] == j[1] && i[2] == j[2] && i[3] == j[3])
				{
					zabbix_log(LOG_LEVEL_DEBUG, "In %s() sip ServerIP [%s]", __function_name, sip);
					for(k = 0; k < strlen(sip); k++){
						socket_ip[k] = sip[k];
					}
					socket_ip[k]='\0';
					zabbix_log(LOG_LEVEL_DEBUG, "In %s() socket_ip ServerIP [%s]", __function_name, socket_ip);
					return SUCCEED;
				}
			}
#endif	/* HAVE_IPV6 */
			if (NULL != end)
			{
				*end = ',';
				start = end + 1;
			}
			else
				break;
		}

		if (NULL != end)
			*end = ',';
	}
#if defined(HAVE_IPV6)
	if (0 == getnameinfo((struct sockaddr *)&name, sizeof(name), sname, sizeof(sname), NULL, 0, NI_NUMERICHOST))
		zabbix_log(LOG_LEVEL_ERR,"Connection from [%s] rejected. Allowed server is [%s].", sname, ip_list);
	else
		zabbix_log(LOG_LEVEL_ERR,"Connection rejected. Allowed server is [%s].", ip_list);
#else
	zabbix_log(LOG_LEVEL_ERR,"Connection from [%s] rejected. Allowed server is [%s].", sname, ip_list);
#endif
	return	FAIL;
}
