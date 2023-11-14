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
use PDOException;;
/**
 * This model is used to manage the schedule.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ScheduleModel extends Model
{

    /**
     * It retrieves schedule lists on search data.
     *
     * @param   int    $publicFlag   public=1; private=0
     * @param   string $sort
     * @param   string $limit
     * @param   string $search
     * @return  array  $resultArray  schedule lists
     * @since   Method available since version 6.1.0
     */
    public function getData($publicFlag, $search)
    {
        $sortBy = "";
        $where = "";
        $objBaseQuery = "";
        $objSelectQuery = "";
        $validFlag = 1;

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $publicFlag == 1) {
            $sortBy = " ORDER BY schedule_id ";
        } else {
            $sortBy = " ORDER BY SCHEDULE.schedule_id ";
        }

        $where = "";
        if ($search !== "") {
            $where = " AND " . substr($search, 0, -4);
        }

        $objBaesQueryFormat = "SELECT * FROM ja_schedule_control_table 
                                WHERE valid_flag = %s AND public_flag = %s 
                                %s
                                UNION ALL  
                                SELECT * FROM ja_schedule_control_table A 
                                WHERE A.update_date= ( SELECT MAX(update_date) FROM ja_schedule_control_table B 
                                WHERE B.schedule_id NOT IN (SELECT schedule_id FROM ja_schedule_control_table  
                                WHERE valid_flag = %s )  
                                AND B.public_flag = %s AND A.schedule_id = B.schedule_id 
                                GROUP BY schedule_id  )
                                %s";
        $objBaseQuery = sprintf($objBaesQueryFormat, $validFlag, $publicFlag, $where, $validFlag, $publicFlag, $where);

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $publicFlag == 1) {
            $objSelectQuery = $objBaseQuery . $sortBy;
        } else {
            $userid = $_SESSION['userInfo']['userId'];
            $objBaseQuery = "SELECT SCHEDULE.* FROM ( 
                            $objBaseQuery ) AS SCHEDULE, 
                            users AS U, users_groups AS UG1, users_groups AS UG2  
                            WHERE SCHEDULE.user_name = U.username  
                            AND U.userid = UG1.userid 
                            AND UG2.userid=$userid 
                            AND UG1.usrgrpid = UG2.usrgrpid ";

            $objSelectQuery = $objBaseQuery . $sortBy;
        }

        $this->db->query($objSelectQuery);
        $resultArray = $this->db->resultSet();

        return $resultArray;
    }

    /**
     * It prepares the data for schedule version.
     *
     * @param   string  $id  schedule id
     * @return  array   schedule version lists
     * @since   Method available since version 6.1.0
     */
    public function detail($id)
    {
        $this->db->query("SELECT * FROM ja_schedule_control_table WHERE schedule_id = '$id' ORDER BY update_date DESC");

        return $this->db->resultSet();
    }

    /**
     * It checks the schedule id is available or not.
     *
     * @param   string $id     id of the schedule.
     * @return  bool   could be true if schedule id already exists,could be false if not 
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {

        $this->db->query("SELECT schedule_id FROM ja_schedule_control_table WHERE schedule_id = '$id'");
        $result = $this->db->execute();
        if ($this->db->rowcount() > 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the schedule data.
     *
     * @param   string $id           id of the schedule.
     * @param   object $updateDate   updated date of the schedule.
     * @throws  PDOException
     * @return  array|string         schedule info
     * @since   Method available since version 6.1.0
     */
    public function each($id, $updateDate = null)
    {
        $this->logger->debug('Schedule information retrieving process is started.', ['controller' => __METHOD__, 'user =>' . $_SESSION['userInfo']['userName']]);
        try {
            // $id = $objData["objectId"];
            // $updateDate = $objData["date"];

            $query = " schedule_id = '$id'";
            if ($updateDate != null) {
                $query .= " and update_date='$updateDate' ";
            }
            $this->db->query("SELECT * FROM ja_schedule_control_table WHERE $query");
            $result = "";
            // if ($formType=='edit') {
            //     $result = $this->db->single();
            // } else {
            $result = $this->db->singleAsArrayInCamelCase();
            // }

            //$this->logger->debug('Schedule information is '.$result, ['controller' => __METHOD__ ,'user =>' . $_SESSION['userInfo']['userName']]);      
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Schedule information retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It delete selected schedule rows of schedule version lists.
     *
     * @param   string $objectId    schedule id 
     * @param   array  $deleteRows  array for delete
     * @return  bool  could be true if delete process success, could be false if fail
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
     * It delete schedule row.
     *
     * @param   string $id          schedule id 
     * @param   string $updateDate
     * @return  bool could be true if delete process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function delete($id, $updateDate)
    {
        $this->db->query("DELETE FROM ja_schedule_control_table WHERE schedule_id = '$id' AND update_date = '$updateDate' ");
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It delete all verion in schedule version list.
     *
     * @param   string $id    schedule id 
     * @return  bool          could be true if delete process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function deleteAllVer($id)
    {
        $query = " schedule_id = '$id'";
        $this->db->query("DELETE FROM ja_schedule_control_table WHERE $query");
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It change valid status schedule object to enabled on schedule id.
     *
     * @param   string  $id          schedule id
     * @param   string  $updateDate 
     * @return  bool                 could be true if status change process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToEnabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_schedule_control_table SET valid_flag = 1 WHERE schedule_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes schedule valid status to disable.
     *
     * @param   string $id          schedule id
     * @param   string $updateDate
     * @return  bool                could be true if status change process is success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToDisabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_schedule_control_table SET valid_flag = 0 WHERE schedule_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes all schedule version status to disable.
     *
     * @param   string $id     schedule id
     * @return  bool           could be true if status change process is success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeAllStatusToDisabled($id)
    {
        $this->db->query('UPDATE ja_schedule_control_table SET valid_flag = 0 WHERE schedule_id = :id');
        $this->db->bind(':id', $id);
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It update the public_flag of entire schedule version.
     *
     * @param   array $data    schedule object including schedule info and operation dates.
     * @return  bool           could be true if update process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function updateFlag($data)
    {
        $this->db->query('UPDATE ja_schedule_control_table SET public_flag=:public_flag WHERE schedule_id=:id');
        $this->db->bind(':id', $data['id']);
        $this->db->bind(':public_flag', $data['public_flag']);
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It create the new schedule object.
     *
     * @param   array $data     schedule object including schedule info and operation dates.
     * @throws  PDOException
     * @return  bool  $boolRet  could be true if create process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function saveScheduleInfo($data)
    {
        $boolRet = true;
        try {
            $this->db->query('INSERT INTO ja_schedule_control_table (schedule_id, schedule_name, user_name, public_flag, update_date, memo) VALUES (:schedule_id, :schedule_name, :user_name, :public_flag, :update_date, :desc)');
            $this->db->bind(':schedule_id', $data['id']);
            $this->db->bind(':schedule_name', $data['name']);
            $this->db->bind(':user_name', $data['userName']);
            $this->db->bind(':public_flag', $data['public_flag']);
            $this->db->bind(':update_date', $data['update_date']);
            $this->db->bind(':desc', $data['desc']);

            $boolRet = $this->db->execute();
            $this->updateFlag($data);
            if ($boolRet) {
                $boolRet = $this->saveCalendarInfo($data['calendarInfoArr'], $data['id'], $data['update_date'], "new");
            }

            if ($boolRet) {
                $boolRet = $this->saveJobnetInfo($data['jobnetInfoArr'], $data['id'], $data['update_date'], "new");
            }
        } catch (PDOException $e) {
            return false;
        }

        return $boolRet;
    }

    /**
     * It insert the calendar info related with schedule.
     *
     * @param   array  $data                calendar object including calendar info.
     * @param   string $scheduleId
     * @param   string $scheduleUpdateDate
     * @param   string $type                action type new|edit
     * @return  bool          could be true if process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function saveCalendarInfo($data, $scheduleId, $scheduleUpdateDate, $type)

    {

        try {
            if ($type != 'create') {
                $this->db->query("DELETE FROM ja_schedule_detail_table WHERE schedule_id = :schedule_id AND update_date=:update_date");
                $this->db->bind(':schedule_id', $scheduleId);
                $this->db->bind(':update_date', $scheduleUpdateDate);
                $this->db->execute();
            }

            foreach ($data as $key => $value) {
                
                $this->db->query('INSERT INTO ja_schedule_detail_table(schedule_id, calendar_id, update_date, boot_time,object_flag) VALUES (:schedule_id, :calendar_id, :update_date, :boot_time,:object_flag)');
                $this->db->bind(':schedule_id', $scheduleId);
                $this->db->bind(':calendar_id', $value['id']);
                $this->db->bind(':update_date', $scheduleUpdateDate);
                $this->db->bind(':boot_time', str_replace(":", "", $value['boottime']));
                $this->db->bind(':object_flag', $value['type']=="filter"?1:0); 

                if (!$this->db->execute()) {
                    return false;
                }
            }
        } catch (PDOException $e) {
            return false;
        }
        return true;
    }

    /**
     * It insert the jobnet info related with schedule.
     *
     * @param   array  $data                jobnet object including jobnet info.
     * @param   string $scheduleId
     * @param   string $scheduleUpdateDate
     * @param   string $type                action type new|edit
     * @return  bool          could be true if process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function saveJobnetInfo($data, $scheduleId, $scheduleUpdateDate, $type)
    {
        try {
            if ($type != 'create') {
                $this->db->query("DELETE FROM ja_schedule_jobnet_table WHERE schedule_id = :schedule_id AND update_date=:update_date");
                $this->db->bind(':schedule_id', $scheduleId);
                $this->db->bind(':update_date', $scheduleUpdateDate);
                $this->db->execute();
            }
            foreach ($data as $key => $value) {

                $this->db->query('INSERT INTO ja_schedule_jobnet_table (schedule_id, jobnet_id, update_date) VALUES (:schedule_id, :jobnet_id, :update_date)');

                $this->db->bind(':schedule_id', $scheduleId);
                $this->db->bind(':jobnet_id', $value['jobnetId']); //jobnetid will error
                $this->db->bind(':update_date', $scheduleUpdateDate);

                if (!$this->db->execute()) {
                    return false;
                }
            }
        } catch (PDOException $e) {
            return false;
        }

        return true;
    }

    /**
     * It retrieves boottime information.
     *
     * @param   string $id
     * @param   string $updateDate
     * @throws  PDOException
     * @return  array  $result   boottime information
     * @since   Method available since version 6.1.0
     */
    public function getBoottime($id, $updateDate)
    {
        $this->logger->debug('Calendar information retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            // $id = $objData["objectId"];
            // $updateDate = $objData["date"];

            $this->db->query("SELECT * FROM ja_schedule_detail_table WHERE schedule_id = '$id' AND update_date = '$updateDate' ");

            $result = $this->db->resultSet();

            //$this->logger->debug('Calendar information is '.$result, ['controller' => __METHOD__ ,'user=>' . $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Calendar information retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves the calendar data.
     *
     * @param   string $id              id of the calendar.
     * @return  array  $calendarInfo    calendar detail info
     * @since   Method available since version 6.1.0
     */
    public function getCalendarDetail($id)
    {
        $this->db->query("SELECT calendar_id, calendar_name,update_date From ja_calendar_control_table WHERE calendar_id = '$id' and valid_flag = '1'");
        $calendarInfo = $this->db->resultSetAsArrayInCamelCase();
        if (count($calendarInfo) <= 0) {
            $this->db->query("SELECT A.calendar_id, A.calendar_name FROM ja_calendar_control_table AS A WHERE A.calendar_id = '$id'and A.update_date= ( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B WHERE B.calendar_id = A.calendar_id group by B.calendar_id)");
            $calendarInfo = $this->db->resultSetAsArrayInCamelCase();
        }
        return $calendarInfo;
    }

    /**
     * It retrieves the filter data.
     *
     * @param   string $id            id of the filter.
     * @return  array  $filterInfo    filter detail info
     * @since   Method available since version 6.1.0
     */
    public function getFilterDetail($id)
    {
        $this->db->query("SELECT filter_id, filter_name,update_date From ja_filter_control_table WHERE filter_id = '$id' and valid_flag = '1'");
        $filterInfo = $this->db->resultSetAsArrayInCamelCase();
        if (count($filterInfo) <= 0) {
            $this->db->query("SELECT A.filter_id, A.filter_name FROM ja_filter_control_table AS A WHERE A.filter_id = '$id'and A.update_date= ( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B WHERE B.filter_id = A.filter_id group by B.filter_id)");
            $filterInfo = $this->db->resultSetAsArrayInCamelCase();
        }
        return $filterInfo;
    }

    /**
     * It retrieves the jobnet data.
     *
     * @param   string $id            id of the jobnet.
     * @return  array  $jobnetInfo    jobnet detail info
     * @since   Method available since version 6.1.0
     */
    public function getJobnetDetail($id)
    {
        $this->db->query("SELECT jobnet_id, jobnet_name,update_date From ja_jobnet_control_table WHERE jobnet_id = '$id' and valid_flag = '1'");
        $jobnetInfo = $this->db->resultSetAsArrayInCamelCase();
        if (count($jobnetInfo) <= 0) {
            $this->db->query("SELECT A.jobnet_id, A.jobnet_name FROM ja_jobnet_control_table AS A WHERE A.jobnet_id = '$id'and A.update_date= ( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id)");
            $jobnetInfo = $this->db->resultSetAsArrayInCamelCase();
        }
        return $jobnetInfo;
    }

    /**
     * It retrieves the jobnet data for schedule.
     *
     * @param   string $id            id of the schedule
     * @param   string $updateDate    update date of schedule
     * @return  array  $result        jobnet object
     * @since   Method available since version 6.1.0
     */
    public function getJobnet($id, $updateDate)
    {
        // $id = $objData["objectId"];
        // $updateDate = $objData["date"];
        $this->db->query("SELECT * FROM ja_schedule_jobnet_table WHERE schedule_id = '$id' AND update_date = '$updateDate'");
        $result = $this->db->resultSet();
        return $result;
    }

    /**
     * It proceeds update the schedule object.
     *
     * @param   array $data     schedule object including schedule info.
     * @return  bool  $boolRet  could be true if update process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function editScheduleInfo($data)
    {
        $boolRet = true;
        $this->db->query("UPDATE ja_schedule_control_table SET update_date= :update_date, memo= :memo, public_flag= :public_flag, user_name= :user_name, schedule_name= :schedule_name WHERE schedule_id = :schedule_id and update_date = :urldate");

        $this->db->bind(':schedule_id', $data['id']);
        $this->db->bind(':schedule_name', $data['name']);
        $this->db->bind(':user_name', $data['userName']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':urldate', $data['urldate']);
        $this->db->bind(':memo', $data['desc']);

        $boolRet = $this->db->execute();


        if ($boolRet) {
            $this->updateFlag($data);
            //$boolRet = $this->updateOtherVersion($data['id'], $data['public_flag']);
        }


        if ($boolRet) {
            $boolRet = $this->saveCalendarInfo($data['calendarInfoArr'], $data['id'], $data['update_date'], "edit");
        }
        if ($boolRet) {
            $boolRet = $this->saveJobnetInfo($data['jobnetInfoArr'], $data['id'], $data['update_date'], "edit");
        }
        return $boolRet;
    }

    // //update the other version public_flag
    // public function updateOtherVersion($id, $public_flag)
    // {
    //     $this->db->query('UPDATE ja_schedule_control_table SET public_flag= :public_flag WHERE schedule_id = :id');

    //     $this->db->bind(':id', $id);
    //     $this->db->bind(':public_flag', $public_flag);

    //     if ($this->db->execute(__METHOD__)) {
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }

    /**
     * It delete boottime information.
     *
     * @param   array  $scheduleId 
     * @param   string $updateDate
     * @param   string $calendarId
     * @param   string $time
     * @return  bool   could be true if delete process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function deleteBoottime($scheduleId, $updateDate, $calendarId, $time)
    {
        $boottime = str_replace(":", "", $time);
        $this->db->query("DELETE FROM ja_schedule_detail_table WHERE schedule_id = '$scheduleId' AND calendar_id = '$calendarId' AND boot_time = '$boottime' AND update_date = '$updateDate'");
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It delete jobnet information.
     *
     * @param   array  $scheduleId 
     * @param   string $updateDate
     * @param   string $jobnetId
     * @return  bool   could be true if delete process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function deleteJobnet($scheduleId, $updateDate, $jobnetId)
    {
        $this->db->query("DELETE FROM ja_schedule_jobnet_table WHERE schedule_id = '$scheduleId' AND jobnet_id = '$jobnetId' AND update_date = '$updateDate'");
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It checks the schedule data is exists or not for enable.
     *
     * @param   object $id           id of the schedule.
     * @param   object $updateDate   updated date of the schedule.
     * @return  bool   could be true if schedule data exists,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function checkSchEmpty($id, $updateDate)
    {
        $this->db->query("SELECT schedule_id FROM ja_schedule_detail_table  WHERE schedule_id = '$id' AND update_date = '$updateDate'");
        $this->db->execute();
        $boottime = $this->db->rowcount();

        $this->db->query("SELECT schedule_id FROM ja_schedule_jobnet_table  WHERE schedule_id = '$id' AND update_date = '$updateDate'");
        $this->db->execute();
        $jobnet = $this->db->rowcount();
        if ($boottime > 0 && $jobnet > 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It checks the related calendar info is exists or not for schedule enable.
     *
     * @param   object $id           id of the schedule.
     * @param   object $updateDate   updated date of the schedule.
     * @return  bool   could be true if related data exists,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function getRelatedDataForCalendar($id, $updateDate)
    {
        //$this->db->query("SELECT calendar_id, update_date FROM ja_schedule_detail_table WHERE schedule_id = '$id' AND update_date = '$updateDate' GROUP BY calendar_id");
        $this->db->query("SELECT S.calendar_id FROM ja_schedule_detail_table S
                            INNER JOIN
                            ja_calendar_control_table C ON S.calendar_id=C.calendar_id WHERE S.schedule_id= '$id' AND S.update_date='$updateDate'  GROUP BY S.calendar_id;");

        return $this->db->resultSet();
    }

    /**
     * It checks the related filter info is exists or not for schedule enable.
     *
     * @param   object $id           id of the schedule.
     * @param   object $updateDate   updated date of the schedule.
     * @return  bool   could be true if related data exists,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function getRelatedDataForFilter($id, $updateDate)
    {
        //$this->db->query("SELECT filter_id, update_date FROM ja_filter_control_table WHERE base_calendar_id = '$id' GROUP BY filter_id");
        $this->db->query("SELECT F.filter_id FROM ja_schedule_detail_table S
                        INNER JOIN
                        ja_filter_control_table F ON S.calendar_id=F.filter_id WHERE S.schedule_id= '$id' AND S.update_date='$updateDate'  GROUP BY S.calendar_id, F.filter_id;");
        return $this->db->resultSet();
    }

    // public function getRelatedDataForSchedule($id)
    // {
    //     $this->db->query("SELECT schedule_id, update_date  FROM ja_schedule_detail_table WHERE calendar_id = '$id' GROUP BY update_date ORDER BY schedule_id asc");
    //     return $this->db->resultSet();
    // }

    /**
     * It checks the related jobnet info is exists or not for schedule enable.
     *
     * @param   object $id           id of the schedule.
     * @param   object $updateDate   updated date of the schedule.
     * @return  bool   could be true if related data exists,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function getRelatedDataForJobnet($id, $updateDate)
    {
        $this->db->query("SELECT jobnet_id FROM ja_schedule_jobnet_table WHERE schedule_id = '$id' AND update_date = '$updateDate'");
        return $this->db->resultSet();
    }

    /**
     * It retrieves count of enabled calendar.
     *
     * @param   string  $id    calendar id.
     * @return  int            enabled count
     * @since   Method available since version 6.1.0
     */
    public function getCalendarEnableCount($id)
    {
        $this->db->query("SELECT COUNT(*) as count FROM ja_calendar_control_table WHERE calendar_id = '$id' AND valid_flag=1;");
        return (int) $this->db->resultSet()[0]->count;
    }

    /**
     * It retrieves count of enabled filter.
     *
     * @param   string  $id    filter id.
     * @return  int            enabled count
     * @since   Method available since version 6.1.0
     */
    public function getFilterEnableCount($id)
    {
        $this->db->query("SELECT COUNT(*) as count FROM ja_filter_control_table WHERE filter_id = '$id' AND valid_flag=1;");
        return (int)  $this->db->resultSet()[0]->count;
    }

    /**
     * It retrieves count of enabled jobnet.
     *
     * @param   string  $id    jobnet id
     * @return  int            enabled count
     * @since   Method available since version 6.1.0
     */
    public function getJobnetEnableCount($id)
    {
        $this->db->query("SELECT COUNT(*) as count FROM ja_jobnet_control_table WHERE jobnet_id = '$id' AND valid_flag=1;");
        return (int)  $this->db->resultSet()[0]->count;
    }

    /**
     * It retrieves total version count of a schedule.
     *
     * @param   string  $id    schedule id
     * @return  int     number of schedule version
     * @since   Method available since version 6.1.0
     */
    public function totalRows($id)
    {
        $this->db->query('SELECT COUNT(*) as count FROM ja_schedule_control_table WHERE SCHEDULE_id = :id');

        $this->db->bind(':id', $id);
        return (int) $this->db->resultSet()[0]->count;
    }
}
