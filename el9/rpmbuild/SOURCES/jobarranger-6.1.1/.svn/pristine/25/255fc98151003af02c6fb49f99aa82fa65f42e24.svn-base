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
 * This model is used to manage the jobnet execution management.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobExecutionManagementModel extends Model
{

    public function __construct()
    {
        parent::__construct();
        $this->dbUtilModel = new DbUtilModel();
    }

    /**
     * It retrieve the current system time.
     *
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getSysTime()
    {
        $this->db->query("SELECT CURRENT_TIMESTAMP AS systemtime");

        return $this->db->single()->systemtime;
    }

    /**
     * It retrieve the valid schedule data
     *
     * @param   string $scheduleId  id of the schedule.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getScheduleValid($scheduleId)
    {
        $this->db->query("select * from ja_schedule_control_table where schedule_id = '$scheduleId' and valid_flag=1");

        if (count($this->db->resultSet()) == 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieve the valid schedule data
     *
     * @param   string $innerJobnetId  inner jobnet id of the run jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function deleteScheduleJobnet($innerJobnetId)
    {
        $this->db->query("select count(*) as count from ja_run_jobnet_table where inner_jobnet_main_id='$innerJobnetId' and main_flag=0 and status=0");

        $count = $this->db->single();

        if ($count->count > 0) {
            if (DATA_SOURCE_NAME == Constants::DB_MYSQL) {
                $this->db->query("delete from ja_run_jobnet_table where inner_jobnet_main_id='$innerJobnetId' and scheduled_time > cast(date_format((current_timestamp + interval + 5 minute),'%Y%m%d%H%i') as decimal(12))");
            }else{
                $this->db->query("delete from ja_run_jobnet_table where inner_jobnet_main_id='$innerJobnetId' and scheduled_time > to_number(to_char((current_timestamp+ '5 minutes'),'YYYYMMDDHH24MI'),'999999999999')");
            }
            if ($this->db->execute()) {
                if($this->db->rowCount() > 0){
                    return true;
                }else{
                    return false;
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    /**
     * It retrieve the jobnet summary data for Super Administrator.
     *
     * @param   string $fromTime (YmdHi)
     * @param   string $toTime (YmdHi)
     * @param   string $startFromTime (YmdHis)
     * @param   string $startToTime (YmdHis)
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetSummaryForSuper($fromTime, $toTime, $startFromTime, $startToTime)
    {
        $this->db->query("select JR.* from ja_run_jobnet_summary_table AS JR where ((JR.scheduled_time between $fromTime and $toTime) OR (JR.start_time between $startFromTime and $startToTime) OR ((JR.scheduled_time = 0) and (JR.start_time between $startFromTime and $startToTime)) OR ((JR.scheduled_time = 0) and (JR.start_time = 0) and (multiple_start_up = 2))) or start_pending_flag = 1 order by JR.scheduled_time,JR.start_time,JR.inner_jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It retrieve the jobnet summary data for all other users
     *
     * @param   string $fromTime (YmdHi)
     * @param   string $toTime (YmdHi)
     * @param   string $startFromTime (YmdHis)
     * @param   string $startToTime (YmdHis)
     * @param   string $userId id of the user.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetSummaryForAll($fromTime, $toTime, $startFromTime, $startToTime, $userId)
    {
        $this->db->query("select JRAll.* from ((select JR.* from ja_run_jobnet_summary_table AS JR where (((JR.scheduled_time between $fromTime and $toTime) OR (JR.start_time between $startFromTime and $startToTime) OR ((JR.scheduled_time = 0) and (JR.start_time between $startFromTime and $startToTime)) OR ((JR.scheduled_time = 0) and (JR.start_time = 0) and (multiple_start_up = 2))) and JR.public_flag=1 )) union (select JR.* from ja_run_jobnet_summary_table AS JR, users AS U, users_groups AS UG1, users_groups AS UG2 where (((JR.scheduled_time between $fromTime and $toTime) OR (JR.start_time between $startFromTime and $startToTime) OR ((JR.scheduled_time = 0) and (JR.start_time between $startFromTime and $startToTime)) OR ((JR.scheduled_time = 0) and (JR.start_time = 0) and (multiple_start_up = 2)))  and JR.public_flag=0 and JR.user_name=U.username and U.userid=UG1.userid and UG2.userid=$userId and UG1.usrgrpid=UG2.usrgrpid) or (JR.user_name=U.username and U.userid=UG1.userid and UG2.userid=$userId and UG1.usrgrpid=UG2.usrgrpid and JR.start_pending_flag = 1)) union (select JR.* from ja_run_jobnet_summary_table AS JR where start_pending_flag = 1 and public_flag=1)  ) as JRAll order by JRAll.scheduled_time,JRAll.start_time,JRAll.inner_jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the error jobnet summary for Super Administrator.
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetErrSummaryForSuper()
    {
        $this->db->query("select JR.* from ja_run_jobnet_summary_table AS JR where JR.job_status=2 and (JR.status=2 or JR.status=4 or JR.status=5) order by start_time desc,inner_jobnet_id desc");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the error jobnet summary for all other users.
     *
     * @param   string $userId id of the user.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetErrSummaryForAll($userId)
    {
        $this->db->query("select JRAll.* from ((select JR.* "
            . "from ja_run_jobnet_summary_table AS JR where JR.job_status=2 and (JR.status=2 or JR.status=4 or JR.status=5) and JR.public_flag=1) "
            . "union "
            . "(select JR.* "
            . "from ja_run_jobnet_summary_table AS JR, users AS U, users_groups AS UG1, users_groups AS UG2 "
            . "where JR.job_status=2 and (JR.status=2 or JR.status=4 or JR.status=5) and "
            . "JR.public_flag=0 and JR.user_name=U.username and U.userid=UG1.userid and UG2.userid=$userId and UG1.usrgrpid=UG2.usrgrpid)) "
            . "as JRAll "
            . "order by JRAll.start_time desc,JRAll.inner_jobnet_id desc");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the running jobnet summary for Super Administrator.
     *
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getJobnetRunningSummaryForSuper()
    {
        $this->db->query("select JR.* from "
            . "ja_run_jobnet_summary_table AS JR "
            . "where JR.status=2 order by start_time,inner_jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the running jobnet summary for all other users.
     *
     * @param   string $userId id of the user.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getJobnetRunningSummaryForAll($userId)
    {
        $this->db->query("select JRAll.* from ((select JR.* from "
            . "ja_run_jobnet_summary_table AS JR "
            . "where JR.status=2 and "
            . "JR.public_flag=1) "
            . "union "
            . "(select JR.* from "
            . "ja_run_jobnet_summary_table AS JR, users AS U, users_groups AS UG1, users_groups AS UG2 "
            . "where JR.status=2 and "
            . "JR.public_flag=0 and JR.user_name=U.username and U.userid=UG1.userid and UG2.userid=$userId and UG1.usrgrpid=UG2.usrgrpid)) "
            . "as JRAll "
            . "order by JRAll.start_time,JRAll.inner_jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It retrieves the jobnet summary data for the user who is not belong to user group.
     *
     * @param   string $fromTime (YmdHi)
     * @param   string $toTime (YmdHi)
     * @param   string $startFromTime (YmdHis)
     * @param   string $startToTime (YmdHis)
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getJobnetSummaryForNotBelongGroup($fromTime, $toTime, $startFromTime, $startToTime)
    {
        $this->db->query("select JRAll.* from ((select JR.* from "
            . "ja_run_jobnet_summary_table AS JR "
            . "where (((JR.scheduled_time between $fromTime and $toTime) OR (JR.start_time between $startFromTime and $startToTime) OR ((JR.scheduled_time = 0) and (JR.start_time between $startFromTime and $startToTime)) OR ((JR.scheduled_time = 0) and (JR.start_time = 0) and (multiple_start_up = 2))) and JR.public_flag=1)) "
            . "union "
            . "(select JR.* from "
            . "ja_run_jobnet_summary_table AS JR, users AS U "
            . "where (((JR.scheduled_time between $fromTime and $toTime) OR (JR.start_time between $startFromTime and $startToTime) OR ((JR.scheduled_time = 0) and (JR.start_time between $startFromTime and $startToTime)) OR ((JR.scheduled_time = 0) and (JR.start_time = 0) and (multiple_start_up = 2)))  and "
            . "JR.public_flag=0 and JR.user_name=U.username)) "
            . " ) "
            . "as JRAll "
            . "order by JRAll.scheduled_time,JRAll.start_time,JRAll.inner_jobnet_id");

        return $this->db->resultSet();
    }

    /**
     * It updates the run jobnet summary data.
     *
     * @param   array $data
     * @param   string $where
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateRunJobnetSummary($data, $where)
    {
        $this->db->query($this->dbUtilModel->build_sql_update("ja_run_jobnet_summary_table", $data, $where));

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }
}
