<?php
/*
** Job Arranger Manager
** Copyright (C) 2023 Daiwa Institute of Research Ltd. All Rights Reserved.
**
** Licensed to the Apache Software Foundation (ASF) under one or more
** contributor license agreements. See the NOTICE file distributed with
** this work for additional information regarding copyright ownership.
** The ASF licenses this file to you under the Apache License, Version 2.0
** (the "License"); you may not use this file except in compliance with
** the License. You may obtain a copy of the License at
**
** http://www.apache.org/licenses/LICENSE-2.0
**
** Unless required by applicable law or agreed to in writing, software
** distributed under the License is distributed on an "AS IS" BASIS,
** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
** See the License for the specific language governing permissions and
** limitations under the License.
**
**/

namespace App\Models;

use App\Utils\Model;

/**
 * This model is used to manage the Host.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class HostModel extends Model
{

    /**
     * It retrieves the host for super admin.
     *
     * @return  array    list of hosts for super admin
     * @since   Method available since version 6.1.0
     */
    public function getHostDataSuper()
    {
        $this->db->query("select hostid, host from hosts where status in (0,1) and flags = 0 order by host ASC");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the host for not super admin.
     *
     * @param   string  $username  current logged in user name
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getHostData($username)
    {
        $this->db->query("select distinct(hosts.hostid), hosts.host from users inner join users_groups on users.userid = users_groups.userid inner join usrgrp on users_groups.usrgrpid = usrgrp.usrgrpid inner join rights on usrgrp.usrgrpid = rights.groupid inner join hosts_groups on rights.id = hosts_groups.groupid inner join hosts on hosts_groups.hostid = hosts.hostid where users.username = '$username' and rights.permission <> '0' and hosts.status in (0,1) and hosts.flags = 0 
        order by hosts.host ASC");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the host group.
     *
     * @return  array    list of host groups.
     * @since   Method available since version 6.1.0
     */
    public function getHostGroup()
    {
        $this->db->query("select groups1.groupid, groups1.name as group_name, '3' as permission from hstgrp as groups1 order by groups1.groupid");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the host gorup for not super admin by username.
     *
     * @param   string  $username  current logged in user name
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getHostGroupByUserName($username)
    {
        $this->db->query("select distinct groups1.groupid, groups1.name as group_name, rights.permission " .
        "from users inner join users_groups on users.userid = users_groups.userid " .
        "inner join rights on users_groups.usrgrpid = rights.groupid " .
        "inner join hstgrp as groups1 on rights.id = groups1.groupid " .
        "inner join hosts_groups on groups1.groupid = hosts_groups.groupid " .
        "where users.username = '$username' and rights.permission >= 0 " .
        "order by groups1.groupid  ");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the host name for super.
     *
     * @param   string  $username  current logged in user name
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getHostNameByGroupIdForSuper($groupId)
    {
        $this->db->query("select groups1.groupid, groups1.name as group_name, hosts.hostid, hosts.host, '3' as permission " .
        "from hstgrp as groups1 inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts " .
        "on hosts_groups.hostid = hosts.hostid where (hosts.status=0 or hosts.status=1) and " .
        "(hosts.flags=0 or hosts.flags=4) and groups1.groupid = $groupId order by host ASC");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the host name for not super admin by username.
     *
     * @param   string  $username  current logged in user name
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getHostNameByGroupId($groupId, $username)
    {
        $this->db->query("select users.userid, users.username, rights.id, groups1.groupid, groups1.name  as group_name, " .
        "hosts_groups.hostid, hosts.host, rights.permission from users inner join users_groups on users.userid = users_groups.userid " .
        "inner join rights on users_groups.usrgrpid = rights.groupid inner join hstgrp as groups1 on rights.id = groups1.groupid " .
        "inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts on hosts_groups.hostid = hosts.hostid " .
        "where users.username = '$username' and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and (hosts.flags=0 or hosts.flags=4) and groups1.groupid = $groupId " .
        "order by hosts_groups.groupid, hosts.host ");

        return $this->db->resultSet();
    }
}
