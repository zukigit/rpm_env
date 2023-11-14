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
use DateTime;

/**
 * This model is used to manage the job execution result.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobExceResultModel extends Model
{

    private $superUserQuery = "select JR.*, DM.message as message "
        . "from ja_run_log_table AS JR, ja_define_run_log_message_table as DM "
        . "where JR.log_date >= :fromdate and JR.log_date <= :todate and JR.message_id=DM.message_id and DM.lang= :lang %s";

    private $userHasGroupQuery = "select JRAll.*, DM.message as message from ( (select JR.* from "
        . "ja_run_log_table AS JR "
        . "where JR.log_date >= :fromdate and JR.log_date <= :todate and JR.public_flag=1 %s) "
        . "union "
        . "(select JR.* from "
        . "ja_run_log_table AS JR, users AS U, users_groups AS UG1, users_groups AS UG2 "
        . "where JR.log_date >= :fromdate and JR.log_date <= :todate and "
        . "JR.public_flag=0 and JR.user_name=U.username and U.userid=UG1.userid and UG2.userid=:userid and UG1.usrgrpid=UG2.usrgrpid %s)) "
        . "as JRAll, ja_define_run_log_message_table as DM "
        . "where JRAll.message_id=DM.message_id and DM.lang=:lang ";

    private $userQuery = "select JRAll.*, DM.message as message from ( (select JR.* from "
        . "ja_run_log_table AS JR "
        . "where JR.log_date >= :fromdate and JR.log_date <= :todate and JR.public_flag=1 %s) "
        . "union "
        . "(select JR.* from "
        . "ja_run_log_table AS JR, users AS U "
        . "where JR.log_date >= :fromdate and JR.log_date <= :todate and "
        . "JR.public_flag=0 and JR.user_name=U.username %s)) "
        . "as JRAll, ja_define_run_log_message_table as DM "
        . "where JRAll.message_id=DM.message_id and DM.lang= :lang ";

    /**
     * It retrieves the job execution result data on search data.
     *
     * @param   array   $searchData
     * @return  array   $datas       could be an array if success
     * @since   Method available since version 6.1.0
     */
    public function getEntity($searchData)
    {
        $where = "";
        $sortTable = "";
        if ($searchData['search'] !== "") {
            $where = " AND " . substr($searchData['search'], 0, -4);
        }
        if ($searchData['userType'] == 3) {

            $sqlQuery = sprintf($this->superUserQuery, $where);
            $sortTable = "JR";
        } else {
            if ($searchData['hasUserGroup']) {
                $sqlQuery = sprintf($this->userHasGroupQuery, $where, $where);
            } else {
                $sqlQuery = sprintf($this->userQuery, $where, $where);
            }
            $sortTable = "JRAll";
        }
        $sortBy = " ORDER BY " . $sortTable . ".inner_jobnet_main_id," . $sortTable . ".log_date";

        $limit = "";
        $this->db->query($sqlQuery . $sortBy . $limit);
        $this->db->bind(':fromdate', $searchData['fromDateTime']);
        $this->db->bind(':todate', $searchData['toDateTime']);
        $this->db->bind(':lang', $searchData['lang']);
        if ($searchData['userType'] != 3) {
            $this->db->bind(':userid', $searchData['userId']);
        }
        $datas = $this->db->resultSet();

        return $datas;
    }

    /**
     * It retrieves the job execution result data to export.
     *
     * @param   array  $searchData
     * @param   int    $start
     * @param   int    $limit
     * @return  array  job execution result data to export
     * @since   Method available since version 6.1.0
     */
    public function getExportResult($searchData, $start, $limit)
    {
        $where = "";

        if ($searchData['search'] !== "") {
            $where = " AND " . substr($searchData['search'], 0, -4);
        }
        if ($searchData['userType'] == 3) {
            $sqlQuery = sprintf($this->superUserQuery, $where);
            $sortBy = " ORDER BY JR.inner_jobnet_main_id,JR.log_date";
        } else {
            if ($searchData['hasUserGroup']) {
                $sqlQuery = sprintf($this->userHasGroupQuery, $where, $where);
            } else {
                $sqlQuery = sprintf($this->userQuery, $where, $where);
            }
            $sortBy = " ORDER BY JRAll.inner_jobnet_main_id,JRAll.log_date";
        }

        $limit = ' Limit ' . $limit . ' OFFSET ' . $start;
        $this->db->query($sqlQuery . $sortBy . $limit);

        $this->db->bind(':fromdate', $searchData['fromDateTime']);
        $this->db->bind(':todate', $searchData['toDateTime']);
        $this->db->bind(':lang', $searchData['lang']);
        if ($searchData['userType'] != 3) {
            $this->db->bind(':userid', $searchData['userId']);
        }
        $result = $this->db->resultSetAsArray();
        if ($result)

            return $result;
    }

    /**
     * It retrieves the job execution result data count.
     *
     * @param   array $searchData     
     * @return  int   result count
     * @since   Method available since version 6.1.0
     */
    public function getResTotal($searchData)
    {
        $where = "";

        if ($searchData['search'] !== "") {
            $where = " AND " . substr($searchData['search'], 0, -4);
        }
        if ($searchData['userType'] == 3) {
            $sqlQuery = sprintf($this->superUserQuery, $where);
        } else {
            if ($searchData['hasUserGroup']) {
                $sqlQuery = sprintf($this->userHasGroupQuery, $where, $where);
            } else {
                $sqlQuery = sprintf($this->userQuery, $where, $where);
            }
        }
        $this->db->query("SELECT COUNT(*) as count FROM ($sqlQuery) t1");
        $this->db->bind(':fromdate', $searchData['fromDateTime']);
        $this->db->bind(':todate', $searchData['toDateTime']);
        $this->db->bind(':lang', $searchData['lang']);
        if ($searchData['userType'] != 3) {
            $this->db->bind(':userid', $searchData['userId']);
        }
        $totalRowsQUL = $this->db->single()->count;

        return $totalRowsQUL;
    }
}
