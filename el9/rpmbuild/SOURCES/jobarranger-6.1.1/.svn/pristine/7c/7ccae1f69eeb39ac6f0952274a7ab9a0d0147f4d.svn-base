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
 * This model is used to manage the user.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class UserModel extends Model
{

    private $selectGroupIdListByAlais = "select UG.usrgrpid from users_groups AS UG,users AS U where U.username=? and U.userid=UG.userid";

    private $selectAllUsernames = "select username from users order by username";

    /**
     * It retrieves logged in user name.
     *
     * @param   string $getUserId    logged in user id
     * @return  string               logged in user name
     * @since   Method available since version 6.1.0
     */
    public function getUserName($getUserId)
    {
        $this->db->query("SELECT usrgrpid FROM users_groups WHERE userid = '$getUserId'");
        $getUserGrpId = $this->db->single();

        $this->db->query("SELECT name FROM usrgrp WHERE usrgrpid = '$getUserGrpId->usrgrpid'");
        return $this->db->single();
    }

    /**
     * It retrieves group id.
     *
     * @param   string $usernames    user name
     * @return  array                lists of group id 
     * @since   Method available since version 6.1.0
     */
    public function getGroupIDListByAlias($usernames)
    {

        $sqlParams = array();

        array_push($sqlParams, $usernames);

        return $this->db->resultSetByParams($this->selectGroupIdListByAlais, $sqlParams);
    }

    /**
     * It retrieves all user name.
     *
     * @return  array   lists of username
     * @since   Method available since version 6.1.0
     */
    public function getAllUserAlias()
    {
        return $this->db->resultSetByParams($this->selectAllUsernames);
    }

    // //retrieve alias name
    // public function checkUsernameWithUsrgrp($username, $curUser)
    // {
    //     //from check user
    //     $this->db->query("SELECT userid FROM users WHERE username = '$username'");
    //     $getUsrID = $this->db->single();
    //     $userID = $getUsrID->userid;
    //     $this->db->query("SELECT usrgrpid FROM users_groups WHERE userid = '$userID'");
    //     $getGrpID = $this->db->single();
    //     $userGrpID = $getGrpID->usrgrpid;

    //     //for current login user 
    //     $this->db->query("SELECT userid FROM users WHERE username = '$curUser'");
    //     $getCurUsrID = $this->db->single();
    //     $curUserID = $getCurUsrID->userid;
    //     $this->db->query("SELECT usrgrpid FROM users_groups WHERE userid = '$curUserID'");
    //     $getCurGrpID = $this->db->single();
    //     $curUserGrpID = $getCurGrpID->usrgrpid;

    //     if ($userGrpID == $curUserGrpID) {
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }

    /**
     * It checks logged in user group is same or not with locked user.
     *
     * @param   array  $loginUserGroupList
     * @param   array  $objectUserGroupList
     * @return  bool   could be true if the group is same,could be false is not
     * @since   Method available since version 6.1.0
     */
    public function isExistGroupId($loginUserGroupList, $objectUserGroupList)
    {
        foreach ($loginUserGroupList as $key => $userGroupId) {
            foreach ($objectUserGroupList as $key => $objectGroupId) {
                if ($objectGroupId["usrgrpid"] == $userGroupId->usrgrpid) {
                    return true;
                }
            }
            // if (in_array($userGroupId, $objectUserGroupList)) {
            //     return true;
            // }
        }
        return false;
    }
}
