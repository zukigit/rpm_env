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
 * This model is used to manage the lock.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ObjectLockModel extends Model
{
    public function __construct()
    {
        parent::__construct();
        $this->dbUtilModel = new DbUtilModel();
    }

    /**
     * It locks for editing object.
     *
     * @param   object $data   object to lock.
     * @return  bool   could be true if lock success,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function insertObjectLock($data)
    {
        $this->db->query('INSERT INTO ja_object_lock_table (object_id, object_type, username, attempt_ip, created_date, last_active_time) VALUES (:object_id, :object_type, :username, :attempt_ip, :created_date, :last_active_time)');
        $this->db->bind(':object_id', $data['object_id']);
        $this->db->bind(':object_type', $data['object_type']);
        $this->db->bind(':username', $data['username']);
        $this->db->bind(':attempt_ip', $data['attempt_ip']);
        $this->db->bind(':created_date', $data['created_date']);
        $this->db->bind(':last_active_time', $data['last_active_time']);

        return $this->db->execute();
    }

    /**
     * It deletes locked object.
     *
     * @param   string  $where   condition for delete
     * @return  bool    could be true if delete locked object process success
     * @since   Method available since version 6.1.0
     */
    public function deleteObjectLock($where)
    {
        $this->db->query("DELETE FROM ja_object_lock_table WHERE $where");
        return $this->db->execute(__METHOD__);
    }

    /**
     * It delete selected calendar rows of calendar version lists.
     *
     * @param   string $objectId   calendar id 
     * @param   array  $deleteRows  array for delete
     * @return  bool  could be true if delete process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function deleteLocked($data)
    {
        $objectType = "";
        $username = "";
        foreach ($data as $deleteRow) {

            switch ($deleteRow["objectType"]) {
                case 'CALENDAR':
                    $objectType = 1;
                    break;
                case 'FILTER':
                    $objectType = 2;
                    break;
                case 'SCHEDULE':
                    $objectType = 3;
                    break;
                case 'JOBNET':
                    $objectType = 4;
                    break;
            }
            if (isset($deleteRow["username"])) {
                $username = $deleteRow["username"];
            } else {
                $username = $_SESSION['userInfo']['userName'];
            }
            $deleteWhere = "object_id = '" . $deleteRow["objectId"] . "' AND object_type = '$objectType' AND username = '$username'";
            if ($this->deleteArr($deleteWhere) == false) {
                return false;
            }
        }
        return true;
    }

    /**
     * It deletes locked object.
     *
     * @param   string  $where   condition for delete
     * @return  bool    could be true if delete locked object process success
     * @since   Method available since version 6.1.0
     */
    public function deleteArr($where)
    {

        $this->db->query("DELETE FROM ja_object_lock_table WHERE $where");
        return $this->db->execute(__METHOD__);
    }

    /**
     * It checks object is locked or not for editing.
     *
     * @param   string  $where   condition to check lock
     * @return  array   could be array if locked object exists
     * @since   Method available since version 6.1.0
     */
    public function checkObjectLock($where)
    {
        $this->db->query("SELECT * FROM ja_object_lock_table WHERE $where");
        return $this->db->single();
    }

    /**
     * It retrieves locked object lists and user role id.
     *
     * @return  array could be array if success
     * @since   Method available since version 6.1.0
     */
    public function getData()
    {
        $this->db->query("SELECT *, users.roleid
        FROM ja_object_lock_table
        LEFT JOIN users ON ja_object_lock_table.username = users.username");
        return $this->db->resultSet();
    }

    /**
     * It retrieves all locked object lists on search data.
     *
     * @param   string  $sort    sort ASC | DESC
     * @param   string  $limit   
     * @param   string  $search
     * @return  array   $resultArray  could be array if success
     * @since   Method available since version 6.1.0
     */
    public function searchLockData($sort, $limit, $search)
    {
        $sortBy = "";
        $objQueryFormat = "";
        $objSelectQuery = "";

        if ($sort !== "") {
            $sortArr = explode(" ", $sort);
            if ($sortArr[0] == "username") {
                $sortArr[0] = "ja_object_lock_table." . $sortArr[0];
            }
            $sortBy = " ORDER BY LOWER(" . $sortArr[0] . ") " . $sortArr[1];
        } else {
            $sortBy = " ORDER BY object_id ";
        }

        if ($search !== "") {
            $searchArr = explode(" ", $search);
            if ($searchArr[0] == "username") {
                $search = "ja_object_lock_table." . $search;
            }
            $search = substr($search, 0, -3);
            $objQueryFormat = "SELECT ja_object_lock_table.username,ja_object_lock_table.object_id,ja_object_lock_table.object_type, ja_object_lock_table.created_date, ja_object_lock_table.last_active_time, users.roleid
            FROM ja_object_lock_table
            LEFT JOIN users ON ja_object_lock_table.username = users.username where " . $search;
        } else {
            $objQueryFormat = "SELECT ja_object_lock_table.username,ja_object_lock_table.object_id,ja_object_lock_table.object_type, ja_object_lock_table.created_date, ja_object_lock_table.last_active_time, users.roleid
            FROM ja_object_lock_table
            LEFT JOIN users ON ja_object_lock_table.username = users.username";
        }

        $objSelectQuery = $objQueryFormat . $sortBy . $limit;
        $this->db->query($objSelectQuery);
        $datas = $this->db->resultSet();

        $this->db->query("SELECT COUNT(*) as count FROM ($objQueryFormat) t1");
        $totalRowsQUL = $this->db->single()->count;

        $resultArray = array($totalRowsQUL, $datas);
        return $resultArray;
    }

    /**
     * It retrieves all locked object lists on search data.
     *
     * @return  array   $resultArray  could be array if success
     * @since   Method available since version 6.1.0
     */
    public function getLockedData()
    {

        $objQueryFormat = "";
        $objSelectQuery = "";
        $objQueryFormat = "SELECT ja_object_lock_table.object_id, ja_object_lock_table.username, ja_object_lock_table.object_type, ja_object_lock_table.attempt_ip, ja_object_lock_table.created_date, ja_object_lock_table.last_active_time, users.roleid 
        FROM ja_object_lock_table 
        LEFT JOIN users ON ja_object_lock_table.username = users.username";
        $objSelectQuery = $objQueryFormat;
        $this->db->query($objSelectQuery);
        $datas = $this->db->resultSet();
        return $datas;
    }

    /**
     * It retrieves all locked object lists on search data.
     *
     * @return  array   $resultArray  could be array if success
     * @since   Method available since version 6.1.0
     */
    public function updateLastActiveTime($data)
    {
        $this->db->query('UPDATE ja_object_lock_table SET last_active_time=:last_active_time WHERE object_id=:object_id and object_type=:object_type');
        $this->db->bind(':object_id', $data['object_id']);
        $this->db->bind(':object_type', $data['object_type']);
        $this->db->bind(':last_active_time', $data['last_active_time']);
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }
}
