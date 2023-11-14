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

use App\Utils\Constants;
use App\Utils\Model;

/**
 * This model is used to manage the jobnet.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobnetModel extends Model
{

    /**
     * It checks the jobnet id is available or not.
     *
     * @param   bool $publicFlag  1: public, 0: private.
     * @param   string $sort
     * @param   string $limit
     * @param   string $search
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getData($publicFlag, $search, $ignoreJobnetId = null): array
    {
        $sortBy = "";
        $where = "";
        $objBaseQuery = "";
        $objSelectQuery = "";

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $publicFlag == 1) {
            $sortBy = " ORDER BY jobnet_id ";
        } else {
            $sortBy = " ORDER BY JOBNET.jobnet_id ";
        }
        $where = "";
        if ($search !== "") {
            $where = " AND " . substr($search, 0, -4);
        }
        if ($ignoreJobnetId != null) {
            $where .= "AND jobnet_id != '$ignoreJobnetId' ";
        }
        $objBaesQueryFormat = "SELECT * FROM ja_jobnet_control_table WHERE valid_flag = 1 AND public_flag = %s  %s
                            UNION ALL  
                            SELECT * FROM ja_jobnet_control_table A  WHERE A.update_date= ( SELECT MAX(update_date) FROM ja_jobnet_control_table B 
                            WHERE B.jobnet_id NOT IN (SELECT jobnet_id FROM ja_jobnet_control_table WHERE valid_flag = 1 )  
                            AND B.public_flag = %s AND A.jobnet_id = B.jobnet_id %s
                            GROUP BY jobnet_id  )";

        $objBaseQuery = sprintf($objBaesQueryFormat, $publicFlag, $where, $publicFlag, $where);

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $publicFlag == 1) {
            $objSelectQuery = $objBaseQuery . $sortBy;
        } else {
            $userid = $_SESSION['userInfo']['userId'];
            $objBaseQuery = "SELECT JOBNET.* FROM ( $objBaseQuery)
                        AS JOBNET, users AS U, users_groups AS UG1, users_groups AS UG2  
                        WHERE JOBNET.user_name = U.username  AND U.userid = UG1.userid  
                        AND UG2.userid=$userid AND UG1.usrgrpid = UG2.usrgrpid  ";
            $objSelectQuery = $objBaseQuery . $sortBy;
        }

        $this->db->query($objSelectQuery);
        $resultArray = $this->db->resultSet();

        return $resultArray;
    }

    /**
     * It retrieves the first row jobnet data 
     *
     * @return  object
     * @since   Method available since version 6.1.0
     */
    public function first()
    {
        $this->db->query("SELECT * FROM ja_jobnet_control_table LIMIT 1");

        return $this->db->single();
    }

    /**
     * It retrieves the jobnet data 
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function detail($id, $updateDate)
    {
        // $query = "jobnet_id = '$id'";
        // if ($updateDate != null) {
        //     $query .= " and update_date=' $updateDate' ";
        // }
        $this->db->query("SELECT * FROM ja_jobnet_control_table WHERE jobnet_id = '$id'  ORDER BY update_date DESC");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the single jobnet data 
     *
     * @param   string $id  id of the jobnet.
     * @param   object $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function each($id, $updateDate = null)
    {
        $query = " jobnet_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date=' $updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_jobnet_control_table WHERE $query");

        return $this->db->single();
    }

    /**
     * It retrieves the all jobnet data 
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function all()
    {
        $this->db->query("select t1.jobnet_id, t1.update_date, t1.valid_flag, t2.jobnet_name, t2.memo
        from ja_jobnet_control_table t2
        join (SELECT jobnet_id, max(update_date) as update_date, max(valid_flag) as valid_flag FROM ja_jobnet_control_table GROUP BY jobnet_id) t1
        on t1.jobnet_id = t2.jobnet_id and
        t1.update_date = t2.update_date");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the jobnet data for Super Administrator.
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getInfoByUserIdSuper()
    {
        $this->db->query("SELECT A.jobnet_id,A.jobnet_name,A.update_date " .
            "FROM ja_jobnet_control_table AS A " .
            "WHERE A.update_date=" .
            "( SELECT MAX(B.update_date) " .
            "FROM ja_jobnet_control_table AS B " .
            "WHERE A.jobnet_id = B.jobnet_id) " .
            "order by A.jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the jobnet data by user id.
     *
     * @param   string $userId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getInfoByUserId($userId)
    {
        $this->db->query("SELECT jobnet.jobnet_id, jobnet.jobnet_name, jobnet.update_date " .
            "FROM ( " .
            "(" .
            "SELECT C.jobnet_id, C.jobnet_name, C.update_date " .
            "FROM ja_jobnet_control_table as C " .
            "WHERE C.public_flag =1 " .
            "and C.update_date=" .
            "( SELECT MAX(D.update_date) " .
            "FROM ja_jobnet_control_table AS D " .
            "WHERE D.jobnet_id = C.jobnet_id) " .
            ") " .
            "UNION (" .
            "SELECT A.jobnet_id, A.jobnet_name, A.update_date " .
            "FROM ja_jobnet_control_table AS A, users AS U, users_groups AS UG1, users_groups AS UG2 " .
            "WHERE A.user_name = U.username " .
            "AND U.userid = UG1.userid " .
            "AND UG2.userid='$userId' " .
            "AND UG1.usrgrpid = UG2.usrgrpid " .
            "AND A.update_date = ( " .
            "SELECT MAX( B.update_date ) " .
            "FROM ja_jobnet_control_table AS B " .
            "WHERE B.jobnet_id = A.jobnet_id " .
            "GROUP BY B.jobnet_id ) " .
            "AND A.public_flag =0" .
            ")" .
            ") AS jobnet " .
            "ORDER BY jobnet.jobnet_id ");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the valid jobnet data by jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  object
     * @since   Method available since version 6.1.0
     */
    public function getValidEntityById($id)
    {
        $this->db->query("SELECT * FROM ja_jobnet_control_table WHERE jobnet_id = '$id' and valid_flag = '1'");

        return $this->db->single();
    }

    /**
     * It retrieves the valid jobnet data by jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  object
     * @since   Method available since version 6.1.0
     */
    public function GetMaxUpdateDateEntityById($id)
    {
        $this->db->query("select * from ja_jobnet_control_table where jobnet_id = '$id' and update_date = (select max(update_date) from ja_jobnet_control_table where jobnet_id='$id')");

        return $this->db->single();
    }

    /**
     * It retrieves the calendar data
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getCalendar()
    {
        //check if the string contains public
        $this->db->query("SELECT calendar_id, max(calendar_name) as calendar_name, max(update_date) as update_date FROM ja_calendar_control_table group by calendar_id;");
        return $this->db->resultSet();
    }

    /**
     * It deletes the all jobnet version.
     *
     * @param   string $id  id of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function deleteAllVer($id)
    {
        $query = " jobnet_id = '$id'";
        $this->db->query("DELETE FROM ja_jobnet_control_table WHERE $query");
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It deletes the jobnet data.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function delete($id, $updateDate)
    {
        $query = " jobnet_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("DELETE FROM ja_jobnet_control_table WHERE $query");
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves valid or latest jobnet data depends on id.
     *
     * @param   string $id  jobnet id
     * @return  array  jobnet info 
     * @since   Method available since version 6.1.0
     */
    public function GetValidORMaxUpdateDateJobnetById($id)
    {
        $this->db->query("SELECT * FROM ja_jobnet_control_table WHERE jobnet_id = '$id' AND valid_flag = '1'");
        $data = $this->db->single();
        if (empty($data)) {
            $this->db->query("select * from ja_jobnet_control_table where jobnet_id = '$id' and update_date = (select max(update_date) from ja_jobnet_control_table where jobnet_id='$id')");
            $data = $this->db->single();
        }

        return $data;
    }

    /**
     * It deletes the jobnet data as array.
     *
     * @param   string $objectId  id of the jobnet.
     * @param   array $deleteRows array of the update data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function deleteArr($objectId, $deleteRows)
    {
        foreach ($deleteRows as $updateDate) {
            if ($this->delete($objectId, $updateDate->updateDate) == false) {
                return false;
            }
        }
        return true;
    }

    /**
     * It deletes the job data.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function deleteJobControl($id, $updateDate)
    {
        $this->db->query("DELETE FROM ja_job_control_table WHERE jobnet_id = '$id' and update_date = '$updateDate'");
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the jobnet data by public flag.
     *
     * @param   int $type  Optional. value of the public flag.
     * @param   string $jobnetId  id of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetOption($type = null, $jobnetId = null)
    {
        $where = '';
        if ($type != null) {
            $where .= "A.public_flag = $type AND ";
        }
        if ($jobnetId != null) {
            $where .= "A.jobnet_id != '$jobnetId' AND ";
        }
        $this->db->query("SELECT distinct A.jobnet_id,A.jobnet_name,A.update_date,A.valid_flag,A.memo, A.public_flag FROM ja_jobnet_control_table AS A,users AS U WHERE $where A.user_name = U.username  and A.update_date= ( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) order by A.jobnet_id");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the job data.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobControlData($jobnetId, $updateDate, $jobid = null)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($updateDate != null) {
            $query .= " and update_date= '$updateDate' ";
        }
        if ($jobid != null) {
            $query .= " and job_id= '$jobid' ";
        }
        $this->db->query("SELECT * FROM ja_job_control_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the flow data.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getFlowData($id, $updateDate)
    {
        $query = " jobnet_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date=' $updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_flow_control_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the end icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getEndIconData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_end_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the if icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getConditionalBranchIconData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_if_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the env icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobControlValueIconData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_value_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the job icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobIconData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_job_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the value job data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getValueJobData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_value_job_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the value job control data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getValueJobControlData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_value_jobcon_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the job command data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobCommandData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_job_command_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the jobnet icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetIconData($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_jobnet_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the extended job icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getExtendedIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_extjob_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the cal icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getCalculationIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_calc_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the task icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getTaskIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jt.jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and jt.job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and jt.update_date='$updateDate' ";
        }
        $query .= " and jjc.update_date = (select max(update_date) from ja_jobnet_control_table where jobnet_id=jjc.jobnet_id)";
        $this->db->query("SELECT jt.*, jjc.jobnet_name FROM ja_icon_task_table jt inner join ja_jobnet_control_table jjc on jt.submit_jobnet_id = jjc.jobnet_id WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the icon icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getInfoIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_info_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the calendar name.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getInfoIconCalendarName($calendarId)
    {
        $query = " calendar_id = '$calendarId' and update_date = (select max(update_date) from ja_calendar_control_table where calendar_id= '$calendarId')";
        $this->db->query("SELECT calendar_name FROM ja_calendar_control_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the file copy icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getFileTransferIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_fcopy_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the file wait data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getFileWaitIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_fwait_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the reboot icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRebootIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_reboot_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the release hold icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getReleaseHoldIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_release_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the agentless icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getAgentLessIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_agentless_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the zabbix icon data.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @param   string $jobId  id of the job.
     * @param   string $updateDate updated date of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getZabbixIcon($jobnetId, $jobId, $updateDate)
    {
        $query = " jobnet_id = '$jobnetId'";
        if ($jobId != null) {
            $query .= " and job_id='$jobId' ";
        }
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_icon_zabbix_link_table WHERE $query");
        return $this->db->resultSet();
    }

    /**
     * It updates the public flag of the jobent by jobnet id.
     *
     * @param   array $data  array of the jobnet data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateFlag($data)
    {
        $this->db->query('UPDATE ja_jobnet_control_table SET public_flag=:public_flag WHERE jobnet_id=:id');
        $this->db->bind(':id', $data['jobnet_id']);
        $this->db->bind(':public_flag', $data['public_flag']);
        if ($this->db->execute(__METHOD__)) {
            return true;
        }
    }

    /**
     * It checks the jobnet id is exist or not.
     *
     * @param   string $id  id of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {
        $this->db->query("SELECT jobnet_id FROM ja_jobnet_control_table WHERE jobnet_id = '$id'");
        $result = $this->db->execute();
        if ($this->db->rowcount() > 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the jobnet control data.
     *
     * @param   array $data  array of the jobnet data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertJobNetControl($data)
    {
        $this->db->query('INSERT INTO ja_jobnet_control_table (jobnet_id, update_date, valid_flag, public_flag, multiple_start_up, user_name, jobnet_name, memo, jobnet_timeout, timeout_run_type) VALUES (:jobnet_id, :update_date, :valid_flag, :public_flag, :multiple_start_up, :user_name, :jobnet_name, :memo, :jobnet_timeout, :timeout_run_type)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':valid_flag', $data['valid_flag']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':multiple_start_up', $data['multiple_start_up']);
        $this->db->bind(':user_name', $data['user_name']);
        $this->db->bind(':jobnet_name', $data['jobnet_name']);
        $this->db->bind(':memo', $data['memo']);
        $this->db->bind(':jobnet_timeout', $data['jobnet_timeout']);
        $this->db->bind(':timeout_run_type', $data['timeout_run_type']);

        if ($this->db->execute()) {
            $updateResult = $this->updateFlag($data);
            return true;
        } else {
            return false;
        }
    }

    /**
     * It updates the jobnet control data.
     *
     * @param   array $data  array of the jobnet data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateJobnetControl($data)
    {
        $this->db->query("UPDATE ja_jobnet_control_table SET update_date= :update_date, valid_flag= :valid_flag, public_flag= :public_flag, multiple_start_up= :multiple_start_up, user_name= :user_name, jobnet_name= :jobnet_name, memo= :memo, jobnet_timeout= :jobnet_timeout, timeout_run_type= :timeout_run_type WHERE jobnet_id = :urlid AND update_date = :urldate");

        $this->db->bind(':urlid', $data['urlid']);
        $this->db->bind(':urldate', $data['urldate']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':valid_flag', $data['valid_flag']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':multiple_start_up', $data['multiple_start_up']);
        $this->db->bind(':user_name', $data['user_name']);
        $this->db->bind(':jobnet_name', $data['jobnet_name']);
        $this->db->bind(':memo', $data['memo']);
        $this->db->bind(':jobnet_timeout', $data['jobnet_timeout']);
        $this->db->bind(':timeout_run_type', $data['timeout_run_type']);
        if ($this->db->execute()) {
            $updateResult = $this->updateFlag($data);
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the job control data.
     *
     * @param   array $data  array of the job data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertJobControl($data)
    {
        $this->db->query('INSERT INTO ja_job_control_table (jobnet_id, job_id, update_date, job_type, point_x, point_y, job_name, method_flag, force_flag, continue_flag, run_user, run_user_password) VALUES (:jobnet_id, :job_id, :update_date, :job_type, :point_x, :point_y, :job_name, :method_flag, :force_flag, :continue_flag, :run_user, :run_user_password)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':job_type', $data['job_type']);
        $this->db->bind(':point_x', $data['point_x']);
        $this->db->bind(':point_y', $data['point_y']);
        $this->db->bind(':job_name', $data['job_name']);
        $this->db->bind(':method_flag', $data['method_flag']);
        $this->db->bind(':force_flag', $data['force_flag']);
        $this->db->bind(':continue_flag', $data['continue_flag']);
        $this->db->bind(':run_user', $data['run_user']);
        $this->db->bind(':run_user_password', $data['run_user_password']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the job command data.
     *
     * @param   array $data  array of the job command data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertJobCommand($data)
    {
        $this->db->query('INSERT INTO ja_job_command_table (jobnet_id, job_id, update_date, command_cls, command) VALUES (:jobnet_id, :job_id, :update_date, :command_cls, :command)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':command_cls', $data['command_cls']);
        $this->db->bind(':command', $data['command']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the value job data.
     *
     * @param   array $data  array of the value job data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertValueJob($data)
    {
        $this->db->query('INSERT INTO ja_value_job_table (jobnet_id, job_id, update_date, value_name, value) VALUES (:jobnet_id, :job_id, :update_date, :value_name, :value)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':value_name', $data['value_name']);
        $this->db->bind(':value', $data['value']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the value job control data.
     *
     * @param   array $data  array of the value job control data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertValueJobControl($data)
    {
        $this->db->query('INSERT INTO ja_value_jobcon_table (jobnet_id, job_id, update_date, value_name) VALUES (:jobnet_id, :job_id, :update_date, :value_name)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':value_name', $data['value_name']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the job icon data.
     *
     * @param   array $data  array of the job icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertJobIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_job_table (jobnet_id, job_id, update_date, host_flag, stop_flag, command_type, timeout, host_name, stop_code, timeout_run_type) VALUES (:jobnet_id, :job_id, :update_date, :host_flag, :stop_flag, :command_type, :timeout, :host_name, :stop_code, :timeout_run_type)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':host_flag', $data['host_flag']);
        $this->db->bind(':stop_flag', $data['stop_flag']);
        $this->db->bind(':command_type', $data['command_type']);
        $this->db->bind(':timeout', $data['timeout']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':stop_code', $data['stop_code']);
        $this->db->bind(':timeout_run_type', $data['timeout_run_type']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the end icon data.
     *
     * @param   array $data  array of the end icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertEndIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_end_table (jobnet_id, job_id, update_date, jobnet_stop_flag, jobnet_stop_code) VALUES (:jobnet_id, :job_id, :update_date, :jobnet_stop_flag, :jobnet_stop_code)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':jobnet_stop_flag', $data['jobnet_stop_flag']);
        $this->db->bind(':jobnet_stop_code', $data['jobnet_stop_code']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the if icon data.
     *
     * @param   array $data  array of the if icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertIfIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_if_table (jobnet_id, job_id, update_date, hand_flag, value_name, comparison_value ) VALUES (:jobnet_id, :job_id, :update_date, :hand_flag, :value_name, :comparison_value)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':hand_flag', $data['hand_flag']);
        $this->db->bind(':value_name', $data['value_name']);
        $this->db->bind(':comparison_value', $data['comparison_value']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the env icon data.
     *
     * @param   array $data  array of the env icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertValueIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_value_table (jobnet_id, job_id, update_date, value_name, value) VALUES (:jobnet_id, :job_id, :update_date, :value_name, :value)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':value_name', $data['value_name']);
        $this->db->bind(':value', $data['value']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the extended job icon data.
     *
     * @param   array $data  array of the extended job icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertExtendedIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_extjob_table (jobnet_id, job_id, update_date, command_id, value) VALUES (:jobnet_id, :job_id, :update_date, :command_id, :value)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':command_id', $data['command_id']);
        $this->db->bind(':value', $data['value']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the calculation icon data.
     *
     * @param   array $data  array of the calculation icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertCalculationIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_calc_table (jobnet_id, job_id, update_date, hand_flag, formula, value_name) VALUES (:jobnet_id, :job_id, :update_date, :hand_flag, :formula, :value_name)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':hand_flag', $data['hand_flag']);
        $this->db->bind(':formula', $data['formula']);
        $this->db->bind(':value_name', $data['value_name']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the task icon data.
     *
     * @param   array $data  array of the task icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertTaskIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_task_table (jobnet_id, job_id, update_date, submit_jobnet_id) VALUES (:jobnet_id, :job_id, :update_date, :submit_jobnet_id)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':submit_jobnet_id', $data['submit_jobnet_id']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the icon icon data.
     *
     * @param   array $data  array of the icon icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertInfoIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_info_table (jobnet_id, job_id, update_date, info_flag, item_id, trigger_id, host_group, host_name, get_job_id, get_calendar_id) VALUES (:jobnet_id, :job_id, :update_date, :info_flag, :item_id, :trigger_id, :host_group, :host_name, :get_job_id, :get_calendar_id)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':info_flag', $data['info_flag']);
        $this->db->bind(':item_id', $data['item_id']);
        $this->db->bind(':trigger_id', $data['trigger_id']);
        $this->db->bind(':host_group', $data['host_group']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':get_job_id', $data['get_job_id']);
        $this->db->bind(':get_calendar_id', $data['get_calendar_id']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the file transfer icon data.
     *
     * @param   array $data  array of the file copy data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertFileTransferIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_fcopy_table (jobnet_id, job_id, update_date, from_host_flag, to_host_flag, overwrite_flag, from_host_name, from_directory, from_file_name, to_host_name, to_directory) VALUES (:jobnet_id, :job_id, :update_date, :from_host_flag, :to_host_flag, :overwrite_flag, :from_host_name, :from_directory, :from_file_name, :to_host_name, :to_directory)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':from_host_flag', $data['from_host_flag']);
        $this->db->bind(':to_host_flag', $data['to_host_flag']);
        $this->db->bind(':overwrite_flag', $data['overwrite_flag']);
        $this->db->bind(':from_host_name', $data['from_host_name']);
        $this->db->bind(':from_directory', $data['from_directory']);
        $this->db->bind(':from_file_name', $data['from_file_name']);
        $this->db->bind(':to_host_name', $data['to_host_name']);
        $this->db->bind(':to_directory', $data['to_directory']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the file wait icon data.
     *
     * @param   array $data  array of the file wait data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertFileWaitIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_fwait_table (jobnet_id, job_id, update_date, host_flag, fwait_mode_flag, file_delete_flag, file_wait_time, host_name, file_name) VALUES (:jobnet_id, :job_id, :update_date, :host_flag, :fwait_mode_flag, :file_delete_flag, :file_wait_time, :host_name, :file_name)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':host_flag', $data['host_flag']);
        $this->db->bind(':fwait_mode_flag', $data['fwait_mode_flag']);
        $this->db->bind(':file_delete_flag', $data['file_delete_flag']);
        $this->db->bind(':file_wait_time', $data['file_wait_time']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':file_name', $data['file_name']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }


    /**
     * It inserts the reboot icon data.
     *
     * @param   array $data  array of the reboot icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRebootIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_reboot_table (jobnet_id, job_id, update_date, host_flag, reboot_mode_flag, reboot_wait_time, host_name, timeout) VALUES (:jobnet_id, :job_id, :update_date, :host_flag, :reboot_mode_flag, :reboot_wait_time, :host_name, :timeout)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':host_flag', $data['host_flag']);
        $this->db->bind(':reboot_mode_flag', $data['reboot_mode_flag']);
        $this->db->bind(':reboot_wait_time', $data['reboot_wait_time']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':timeout', $data['timeout']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the release icon data.
     *
     * @param   array $data  array of the release icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertReleaseIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_release_table (jobnet_id, job_id, update_date, release_job_id) VALUES (:jobnet_id, :job_id, :update_date, :release_job_id)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':release_job_id', $data['release_job_id']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the zabbix icon data.
     *
     * @param   array $data  array of the zabbix icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertZabbixIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_zabbix_link_table (jobnet_id, job_id, update_date, link_target, link_operation, groupid, hostid, itemid, triggerid) VALUES (:jobnet_id, :job_id, :update_date, :link_target, :link_operation, :groupid, :hostid, :itemid, :triggerid)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':link_target', $data['link_target']);
        $this->db->bind(':link_operation', $data['link_operation']);
        $this->db->bind(':groupid', $data['groupid']);
        $this->db->bind(':hostid', $data['hostid']);
        $this->db->bind(':itemid', $data['itemid']);
        $this->db->bind(':triggerid', $data['triggerid']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the agentless icon data.
     *
     * @param   array $data  array of the agentless icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertAgentLessIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_agentless_table (jobnet_id, job_id, update_date, host_flag, connection_method, session_flag, auth_method, run_mode, line_feed_code, timeout, session_id, login_user, login_password, public_key, private_key, passphrase, host_name, stop_code, terminal_type, character_code, prompt_string, command) VALUES (:jobnet_id, :job_id, :update_date, :host_flag, :connection_method, :session_flag, :auth_method, :run_mode, :line_feed_code, :timeout, :session_id, :login_user, :login_password, :public_key, :private_key, :passphrase, :host_name, :stop_code, :terminal_type, :character_code, :prompt_string, :command)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':host_flag', $data['host_flag']);
        $this->db->bind(':connection_method', $data['connection_method']);
        $this->db->bind(':session_flag', $data['session_flag']);
        $this->db->bind(':auth_method', $data['auth_method']);
        $this->db->bind(':run_mode', $data['run_mode']);
        $this->db->bind(':line_feed_code', $data['line_feed_code']);
        $this->db->bind(':timeout', $data['timeout']);
        $this->db->bind(':session_id', $data['session_id']);
        $this->db->bind(':login_user', $data['login_user']);
        $this->db->bind(':login_password', $data['login_password']);
        $this->db->bind(':public_key', $data['public_key']);
        $this->db->bind(':private_key', $data['private_key']);
        $this->db->bind(':passphrase', $data['passphrase']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':stop_code', $data['stop_code']);
        $this->db->bind(':terminal_type', $data['terminal_type']);
        $this->db->bind(':character_code', $data['character_code']);
        $this->db->bind(':prompt_string', $data['prompt_string']);
        $this->db->bind(':command', $data['command']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It inserts the jobnet icon data.
     *
     * @param   array $data  array of the jobnet icon data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertJobnetIcon($data)
    {
        $this->db->query('INSERT INTO ja_icon_jobnet_table (jobnet_id, job_id, update_date, link_jobnet_id) VALUES (:jobnet_id, :job_id, :update_date, :link_jobnet_id)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':link_jobnet_id', $data['link_jobnet_id']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes the valid_flag of the jobnet to 1 by jobnet id and update date.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToEnabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_jobnet_control_table SET valid_flag = 1 WHERE jobnet_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes the valid_flag of the jobnet to 0 by jobnet id and update date.
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate updated date of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToDisabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_jobnet_control_table SET valid_flag = 0 WHERE jobnet_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes the valid_flag of the all jobnet version to 0 by jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function changeAllStatusToDisabled($id)
    {
        $this->db->query('UPDATE ja_jobnet_control_table SET valid_flag = 0 WHERE jobnet_id = :id');
        $this->db->bind(':id', $id);
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the total row count of the jobnet by jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  int
     * @since   Method available since version 6.1.0
     */
    public function totalRows($id)
    {
        $this->db->query('SELECT COUNT(*) as count FROM ja_jobnet_control_table WHERE jobnet_id = :id');
        $this->db->bind(':id', $id);
        return (int) $this->db->resultSet()[0]->count;
    }

    /**
     * It checks the jobnet for delete.
     *
     * @param   string $id  id of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function checkJobnetForDelete($id)
    {
        $result = [];
        $this->db->query("SELECT jobnet_id, update_date FROM ja_job_control_table where job_id= '$id' 
        UNION
        SELECT jobnet_id, update_date FROM ja_icon_task_table where submit_jobnet_id= '$id' GROUP BY jobnet_id , update_date;");
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * It checks that the jobnet is used by schedule bu jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function checkScheduleForDelete($id)
    {
        $this->db->query("SELECT schedule_id,update_date FROM ja_schedule_jobnet_table WHERE jobnet_id = :id");
        $this->db->bind(':id', $id);
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * It checks the enable schedule is exist by jobnet id.
     *
     * @param   string $id  id of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function checkScheduleEnable($id)
    {
        $this->db->query("SELECT c.schedule_id,c.update_date FROM ja_schedule_jobnet_table j  INNER JOIN ja_schedule_control_table c ON j.schedule_id = c.schedule_id WHERE jobnet_id = :id AND valid_flag = 1 AND c.update_date = j.update_date");
        $this->db->bind(':id', $id);
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * Check for child jobnet validity (before changing jobnet to enable state).
     *
     * @param   string $id  id of the jobnet.
     * @return  Object
     * @since   Method available since version 6.1.0
     */
    public function  checkJonbetChildForEnable($id, $updateDate)
    {
        $relatedObjects = [];
        $this->db->query("select netCtl.jobnet_id, jobCtl.job_id, jobCtl.job_name, jobCtl.job_type from ja_jobnet_control_table as netCtl, ja_job_control_table as jobCtl where netCtl.jobnet_id = :id and netCtl.update_date = :updateDate and netCtl.jobnet_id = jobCtl.jobnet_id and netCtl.update_date = jobCtl.update_date and jobCtl.job_type in (5, 11);");
        $this->db->bind(':updateDate', $updateDate);
        $this->db->bind(':id', $id);
        $this->db->execute();
        $childJobnet = $this->db->resultSet();
        if (count($childJobnet) <= 0) {
            return (object)$relatedObjects;
        } else {
            foreach ($childJobnet as $child) {
                //get related data.
                $childJobnetType = $child->job_type;
                if ($childJobnetType == "5") {
                    //for [jobnet] icon

                    //check if valid jobnet exists
                    $this->db->query("select distinct icon.link_jobnet_id from ja_icon_jobnet_table as icon, ja_jobnet_control_table as netCtl where icon.jobnet_id = :jobnetId and icon.job_id = :jobId and icon.link_jobnet_id = netCtl.jobnet_id and netCtl.valid_flag = 1 and icon.update_date = :updateDate");
                    $this->db->bind(':jobId', $child->job_id);
                    $this->db->bind(':jobnetId', $child->jobnet_id);
                    $this->db->bind(':updateDate', $updateDate);
                    $this->db->execute();
                    $validLinkedJobnet = $this->db->resultSet();
                    //no valid jobnet linked.
                    if (count($validLinkedJobnet) == 0) {
                        $this->db->query("select distinct icon.link_jobnet_id  as jobnet_id from ja_icon_jobnet_table as icon, ja_jobnet_control_table as netCtl where icon.jobnet_id = :jobnetId and icon.job_id = :jobId and icon.link_jobnet_id = netCtl.jobnet_id and netCtl.valid_flag = 0 and icon.update_date = :updateDate");
                        $this->db->bind(':jobId', $child->job_id);
                        $this->db->bind(':jobnetId', $child->jobnet_id);
                        $this->db->bind(':updateDate', $updateDate);
                        $this->db->execute();
                        if (!is_numeric(array_search((array)$this->db->single(), $relatedObjects))) {
                            array_push($relatedObjects, (array)$this->db->single());
                        }
                    }
                } else if ($childJobnetType == '11') {
                    //check if valid jobnet exists

                    $this->db->query("select  jobnet_id from ja_jobnet_control_table where jobnet_id = (select submit_jobnet_id from ja_icon_task_table  where jobnet_id = :jobnetId and job_id = :jobId and update_date =:updateDate) and valid_flag = 1");
                    $this->db->bind(':jobId', $child->job_id);
                    $this->db->bind(':jobnetId', $child->jobnet_id);
                    $this->db->bind(':updateDate', $updateDate);
                    $this->db->execute();
                    $validLinkedJobnet = $this->db->resultSet();
                    //no valid jobnet linked.
                    if (count($validLinkedJobnet) == 0) {
                        $this->db->query("select distinct submit_jobnet_id as jobnet_id from ja_icon_task_table where jobnet_id = :jobnetId and job_id = :jobId and update_date = :updateDate");
                        $this->db->bind(':jobId', $child->job_id);
                        $this->db->bind(':jobnetId', $child->jobnet_id);
                        $this->db->bind(':updateDate', $updateDate);
                        $this->db->execute();
                        array_push($relatedObjects, (array)$this->db->single());
                    }
                }
            }
            return (object)$relatedObjects;
        }
    }

    /**
     * Check for child jobnet validity (before changing jobnet to disable state and delete.).
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate  updateDate of the jobnet.
     * @param   string $type    Type of parent process("delete" or "disable")    
     * @return  Object
     * @since   Method available since version 6.1.0
     */
    public function  checkJonbetRelatedParentSchedule($id, $type)
    {
        $relatedObjects = [];
        if ($type == "delete") {
            $this->db->query("select distinct sch.schedule_id as schedule_id , sch.update_date as update_date from ja_schedule_jobnet_table as sch, ja_schedule_control_table as ctl where sch.jobnet_id = :jobnetId and sch.schedule_id = ctl.schedule_id");
            $this->db->bind(':jobnetId', $id);
            $this->db->execute();
            $parentSchedule = $this->db->resultSet();
        } else {
            $this->db->query("select distinct sch.schedule_id as schedule_id from ja_schedule_jobnet_table as sch, ja_schedule_control_table as ctl where sch.jobnet_id = :jobnetId and sch.schedule_id = ctl.schedule_id and ctl.valid_flag = 1 and sch.update_date = ctl.update_date");
            $this->db->bind(':jobnetId', $id);
            $this->db->execute();
            $parentSchedule = $this->db->resultSet();
        }

        if (count((array)$parentSchedule) <= 0) {
            return (object)$relatedObjects;
        } else {
            foreach ($parentSchedule as $parent) {
                array_push($relatedObjects, (array)$parent);
            }
            return (object)$relatedObjects;
        }
    }

    /**
     * Check for child jobnet validity (before changing jobnet to disable state and delete.).
     *
     * @param   string $id  id of the jobnet.
     * @param   string $updateDate  updateDate of the jobnet.
     * @param   string $type    Type of parent process("delete" or "disable")    
     * @return  Object
     * @since   Method available since version 6.1.0
     */
    public function  checkJonbetRelatedParentJobnet($id, $type)
    {
        $relatedObjects = [];
        $this->db->query("select icon.jobnet_id ,icon.update_date from ja_icon_jobnet_table as icon where icon.link_jobnet_id = :jobnetId union select tsk.jobnet_id , tsk.update_date from ja_icon_task_table as tsk where tsk.submit_jobnet_id = :jobnetId");
        $this->db->bind(':jobnetId', $id);
        $this->db->execute();
        $parentJobnet = $this->db->resultSet();
        if (count((array)$parentJobnet) <= 0) {
            return (object)$relatedObjects;
        } else {
            foreach ($parentJobnet as $parent) {
                if ($type == "delete") {
                    //delete : checks if parent exists.
                    $this->db->query("select jobnet_id, update_date, valid_flag, user_name, jobnet_name from ja_jobnet_control_table where jobnet_id = :jobnetId and update_date = :updateDate");
                } else {
                    //disable : check if parent with [enabled] state exists.
                    $this->db->query("select jobnet_id, update_date, valid_flag, user_name, jobnet_name from ja_jobnet_control_table where jobnet_id = :jobnetId and update_date = :updateDate and valid_flag = 1");
                }
                $this->db->bind(':jobnetId', $parent->jobnet_id);
                //$this->db->bind(':jobnetId', "noid");
                $this->db->bind(':updateDate', $parent->update_date);
                $this->db->execute();
                $parentJobnetItem = $this->db->single();
                $count = count((array)$parentJobnetItem);
                if (count((array)$parentJobnetItem) > 0 && $parentJobnetItem != false) {
                    array_push($relatedObjects, (array)$this->db->single());
                }
            }
            return (object)$relatedObjects;
        }
    }

    /**
     * It retrieves the item name.
     *
     * @param   string  $hostId  user selected host id
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getItem($hostId)
    {
        $this->db->query("select hosts.hostid, hosts.host, items.itemid, items.name as item_name, items.key_ as item_key, items.type " .
            "from hosts inner join items on hosts.hostid = items.hostid where hosts.hostid = '$hostId' and (hosts.status=0 or hosts.status=1) " .
            "and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4) order by hosts.hostid, items.itemid ");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the trigger name.
     *
     * @param   string  $hostId  user selected host id
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getTrigger($hostId)
    {
        $this->db->query("select hosts.hostid, hosts.host, triggers.triggerid, triggers.expression, triggers.description " .
            "from hosts inner join items on hosts.hostid = items.hostid inner join functions on items.itemid= functions.itemid " .
            "inner join triggers on functions.triggerid = triggers.triggerid where hosts.hostid = $hostId and (hosts.status=0 or hosts.status=1) " .
            "and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4) and (triggers.flags=0 or triggers.flags=4) " .
            "order by hosts.hostid, items.itemid, triggers.triggerid ");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the trigger expression.
     *
     * @param   string  $functionId
     * @return  array    list of hosts for not super admin
     * @since   Method available since version 6.1.0
     */
    public function getTriggerExpression($functionId)
    {
        $this->db->query("select hosts.host,items.key_,functions.name,functions.parameter " .
            "from functions " .
            "inner join items on functions.itemid = items.itemid inner join hosts on items.hostid= hosts.hostid " .
            "where functions.functionid = $functionId " .
            "and (hosts.status=0 or hosts.status=1) and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4)");

        return $this->db->resultSet();
    }

    /**
     * It retrieves valid or latest calendar info
     *
     * @param   string  $calendarId
     * @return  array    number of calendar version
     * @since   Method available since version 6.1.0
     */
    public function getValidORMaxUpdateDateEntityById($jobnetId)
    {
        $result = $this->getValidEntityById($jobnetId);
        if($result){
            return $result;
        }
        return $this->GetMaxUpdateDateEntityById($jobnetId);
    }
}
