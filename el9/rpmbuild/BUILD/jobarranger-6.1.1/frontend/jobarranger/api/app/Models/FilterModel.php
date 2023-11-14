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
use PDOException;

/**
 * This model is used to manage the filter.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class FilterModel extends Model
{

    /**
     * It retrieves filter lists on search data.
     *
     * @param   int    $publicFlag   public=1; private=0
     * @param   string $sort
     * @param   string $limit
     * @param   string $search
     * @return  array  $resultArray filter lists
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
            $sortBy = " ORDER BY filter_id ";
        } else {
            $sortBy = " ORDER BY FILTER.filter_id ";
        }

        if ($search !== "") {
            $where = " AND " . substr($search, 0, -4);
        }

        $objBaesQueryFormat = "SELECT * FROM ja_filter_control_table 
                                WHERE valid_flag = %s AND public_flag = %s 
                                %s
                                UNION ALL  
                                SELECT * FROM ja_filter_control_table A 
                                WHERE A.update_date= ( SELECT MAX(update_date) FROM ja_filter_control_table B 
                                WHERE B.filter_id NOT IN (SELECT filter_id FROM ja_filter_control_table  
                                WHERE valid_flag = %s )  
                                AND B.public_flag = %s AND A.filter_id = B.filter_id 
                                GROUP BY filter_id  ) %s";

        $objBaseQuery = sprintf($objBaesQueryFormat, $validFlag, $publicFlag, $where, $validFlag, $publicFlag, $where);

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $publicFlag == 1) {
            $objSelectQuery = $objBaseQuery . $sortBy;
        } else {
            $userid = $_SESSION['userInfo']['userId'];
            $objBaseQuery = "SELECT FILTER.* FROM ( 
                                    $objBaseQuery ) AS FILTER, 
                                    users AS U, users_groups AS UG1, users_groups AS UG2  
                                    WHERE FILTER.user_name = U.username  
                                    AND U.userid = UG1.userid 
                                    AND UG2.userid=$userid 
                                    AND UG1.usrgrpid = UG2.usrgrpid ";

            $objSelectQuery = $objBaseQuery . $sortBy;
        }

        $this->db->query($objSelectQuery);
        $resultArray = $this->db->resultSet();
        return $resultArray;
    }

    // //retrieve the first row data 
    // public function first()
    // {
    //     $this->db->query("SELECT * FROM ja_filter_control_table LIMIT 1");

    //     return $this->db->single();
    // }

    /**
     * It checks the filter id is available or not.
     *
     * @param   string $id     id of the filter.
     * @return  bool   could be false if available,could be true if not
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {
        $this->db->query("SELECT filter_id FROM ja_filter_control_table WHERE filter_id = '$id'");
        $result = $this->db->execute();
        if ($this->db->rowcount() > 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves filter object on selected filter id.
     *
     * @param   string $id     id of the filter.
     * @return  array  filter information
     * @since   Method available since version 6.1.0
     */
    public function detail($id)
    {
        $this->db->query("SELECT * FROM ja_filter_control_table WHERE filter_id = '$id' ORDER BY update_date DESC");

        return $this->db->resultSet();
    }

    /**
     * It retrieves filter object on filter id and update date.
     *
     * @param   string $id     id of the filter
     * @param   object $updateDate  update date of filter
     * @return  array  filter information
     * @since   Method available since version 6.1.0
     */
    public function each($id, $updateDate = null)
    {
        $query = " filter_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date=' $updateDate' ";
        }
        $this->db->query("SELECT * FROM ja_filter_control_table WHERE $query");

        return $this->db->single();
    }

    /**
     * It delete filter row.
     *
     * @param   string $id  filter id 
     * @param   string $updateDate
     * @return  bool could be true if delete process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function delete($id, $updateDate)
    {
        $query = " filter_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date= '$updateDate' ";
        }
        $this->db->query("DELETE FROM ja_filter_control_table WHERE $query");
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves valid or latest filter data depends on id.
     *
     * @param   string $id  filter id
     * @return  array  filter info 
     * @since   Method available since version 6.1.0
     */
    public function GetValidORMaxUpdateDateFilterById($id)
    {
        $this->db->query("SELECT * FROM ja_filter_control_table WHERE filter_id = '$id' AND valid_flag = '1'");
        $data = $this->db->single();
        if (empty($data)) {
            $this->db->query("select * from ja_filter_control_table where filter_id = '$id' and update_date = (select max(update_date) from ja_filter_control_table where filter_id='$id')");
            $data = $this->db->single();
        }

        return $data;
    }
    /**
     * It delete selected filter rows of filter version lists.
     *
     * @param   string $objectId   filter id 
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
     * It delete all verion in filter version list.
     *
     * @param   string $id   filter id 
     * @return  bool could be true if delete process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function deleteAllVer($id)
    {
        $query = " filter_id = '$id'";
        $this->db->query("DELETE FROM ja_filter_control_table WHERE $query");
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves user name.
     *
     * @return  string user name
     * @since   Method available since version 6.1.0
     */
    public function getName()
    {
        $this->db->query("SELECT user_name FROM ja_filter_control_table");

        return $this->db->single();
    }

    // //retrieve the filter id
    // public function getlastID()
    // {
    //     $this->db->query("SELECT filter_id FROM ja_filter_control_table WHERE filter_id regexp '_[0-9]' ORDER BY update_date DESC");

    //     return $this->db->single();
    // }

    /**
     * It proceeds update the filter object.
     *
     * @param   array $data     filter object including filter info.
     * @return  bool could be true if update process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function update($data)
    {
        $this->db->query("UPDATE ja_filter_control_table SET user_name= :user_name, filter_name= :name, memo= :desc, public_flag= :public_flag, shift_day= :shift_day, base_calendar_id= :base_calendar_id, base_date_flag= :base_date_flag, designated_day= :designated_day, update_date= :updateDate WHERE filter_id = :urlid AND update_date = :urldate");

        $this->db->bind(':urlid', $data['urlid']);
        $this->db->bind(':urldate', $data['urldate']);
        $this->db->bind(':user_name', $data['user_name']);
        $this->db->bind(':name', $data['name']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':desc', $data['desc']);
        $this->db->bind(':updateDate', $data['updateDate']);
        $this->db->bind(':shift_day', $data['shift_day']);
        $this->db->bind(':base_calendar_id', $data['base_calendar_id']);
        $this->db->bind(':base_date_flag', $data['base_date_flag']);
        $this->db->bind(':designated_day', $data['designated_day']);
        if ($this->db->execute(__METHOD__)) {
            if ($this->updateFlag($data)) {
                return true;
            }
        }
        return false;
    }

    // /**
    //  * It proceeds update the calendar operation date.
    //  *
    //  * @param   array $data     calendar object including calendar info and operation dates.
    //  * @return  bool could be true if update process success, could be false if not
    //  * @since   Method available since version 6.1.0
    //  */
    // public function updateDates($data)
    // {
    //     $this->db->query('UPDATE ja_calendar_detail_table SET calendar_id = :id, update_date= :update_date, operating_date = :operating_date  WHERE calendar_id = :id');

    //     $this->db->bind(':id', $data['id']);
    //     $this->db->bind(':update_date', $data['update_date']);
    //     $this->db->bind(':operating_date', $data['operating_date']);

    //     if ($this->db->execute()) {
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }

    // //delete the calendar detail row
    // public function deleteDates($id)
    // {
    //     $this->db->query("DELETE FROM ja_calendar_detail_table WHERE calendar_id = '$id'");


    //     if ($this->db->execute()) {
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }


    /**
     * It retrieves distinct operation date lists of calendar.
     *
     * @param   string $id  calendar id 
     * @param   string $updateDate
     * @return  array distinct operation date lists
     * @since   Method available since version 6.1.0
     */
    public function getDates($id, $updateDate)
    {
        $query = " calendar_id = '$id'";
        if ($updateDate != null) {
            $query .= " and update_date='$updateDate'";
        }
        $this->db->query("SELECT distinct(operating_date) FROM ja_calendar_detail_table WHERE $query order by operating_date asc");

        return $this->db->resultSet();
    }

    /**
     * It update the public_flag of entire filter version.
     *
     * @param   array $data     filter object including filter info and operation dates.
     * @return  bool could be true if update process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function updateFlag($data)
    {
        $this->db->query('UPDATE ja_filter_control_table SET public_flag=:public_flag WHERE filter_id=:id');
        $this->db->bind(':id', $data['id']);
        $this->db->bind(':public_flag', $data['public_flag']);
        if ($this->db->execute(__METHOD__)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It create the new filter object.
     *
     * @param   array $data     filter object including filter info and operation dates.
     * @return  bool could be true if create process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function save($data)
    {
        $this->db->query('INSERT INTO ja_filter_control_table (filter_id, filter_name, user_name, public_flag, update_date, memo, base_calendar_id, base_date_flag, designated_day, shift_day) VALUES (:id, :name, :username, :public_flag, :update_date, :desc, :base_calendar_id, :base_date_flag, :designated_day, :shift_day )');

        $this->db->bind(':id', $data['id']);
        $this->db->bind(':name', $data['name']);
        $this->db->bind(':username', $data['username']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':desc', $data['desc']);
        $this->db->bind(':shift_day', $data['shift_day']);
        $this->db->bind(':base_date_flag', $data['base_date_flag']);
        $this->db->bind(':designated_day', $data['designated_day']);
        $this->db->bind(':base_calendar_id', $data['base_calendar_id']);

        if ($this->db->execute()) {
            if ($this->updateFlag($data)) {
                return true;
            }
        }
        return false;
    }

    /**
     * It retrieves the next filter id.
     *
     * @param   int $id  filter id count
     * @return  string last filter id
     * @since   Method available since version 6.1.0
     */
    public function getNextID($id)
    {
        $this->db->query("SELECT nextid FROM ja_index_table WHERE count_id = '$id' for update");

        return $this->db->single();
    }


    /**
     * It increase the filter id after retrieve next id
     *
     * @param   int $id  filter id count
     * @return  bool could be true if increase process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function increateNextID($id)
    {
        $this->db->query('UPDATE ja_index_table SET nextid = nextid + 1 WHERE count_id = :id');

        $this->db->bind(':id', $id);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrives the calendar list for filter create|update.
     *
     * @return  array calendar list 
     * @since   Method available since version 6.1.0
     */
    public function getCalendarOption()
    {
        $this->db->query("SELECT distinct A.calendar_id,A.update_date FROM ja_calendar_control_table AS A,users AS U WHERE A.user_name = U.username  and A.update_date= ( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B WHERE B.calendar_id = A.calendar_id group by B.calendar_id) order by A.calendar_id");
        return $this->db->resultSet();
    }

    /**
     * It retrives the public calendar list for filter create|update.
     * 
     * @return  array public calendar lists
     * @since   Method available since version 6.1.0
     */
    public function getPublicCalendarOption()
    {
        $this->logger->debug('Public Calendar list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("SELECT calendar_id,MAX(update_date) AS update_date FROM ja_calendar_control_table
            WHERE public_flag = '1' group by calendar_id order by calendar_id");
            $result = $this->db->resultSet();
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Public Calendar list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrives the private calendar list for filter create|update.
     *
     * @return  array private calendar lists
     * @since   Method available since version 6.1.0
     */
    public function getPrivateCalendarOption()
    {
        $this->logger->debug('Private Calendar list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $this->db->query("SELECT calendar_id,MAX(update_date) AS update_date FROM ja_calendar_control_table
                    WHERE public_flag = '0' group by calendar_id order by calendar_id");
            } else {

                $userid = $_SESSION['userInfo']['userId'];
                $this->db->query("SELECT distinct A.calendar_id,A.update_date 
                    FROM ja_calendar_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 
                    WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=$userid
                    and UG1.usrgrpid = UG2.usrgrpid and A.public_flag = 0 and A.update_date= 
                    ( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B 
                    WHERE B.calendar_id = A.calendar_id group by B.calendar_id) order by A.calendar_id");
            }
            $result = $this->db->resultSet();
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Private Calendar list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It change valid status filter object to enabled on filter id.
     *
     * @param   string $id     filter id
     * @param   string $updateDate 
     * @return  bool could be true if status change process success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToEnabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_filter_control_table SET valid_flag = 1 WHERE filter_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It checks filter is used in enabled schedule.
     *
     * @param   string $id     filter id
     * @return  array          related schedule data
     * @since   Method available since version 6.1.0
     */
    public function checkScheduleDisable($id)
    {
        $this->db->query('SELECT c.schedule_id , c.update_date FROM ja_schedule_detail_table d INNER JOIN ja_schedule_control_table c ON  c.schedule_id = d.schedule_id WHERE d.calendar_id = :id AND c.valid_flag = 1 AND d.update_date = c.update_date GROUP BY c.schedule_id , c.update_date');
        $this->db->bind(':id', $id);
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * It changes filter valid status to disable.
     *
     * @param   string $id     filter id
     * @param   string $updateDate
     * @return  bool could be true if status change process is success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeStatusToDisabled($id, $updateDate)
    {
        $this->db->query('UPDATE ja_filter_control_table SET valid_flag = 0 WHERE filter_id = :id AND update_date = :updateDate');

        $this->db->bind(':id', $id);
        $this->db->bind(':updateDate', $updateDate);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It changes all filter version status to disable.
     *
     * @param   string $id     filter id
     * @return  bool could be true if status change process is success, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function changeAllStatusToDisabled($id)
    {
        $this->db->query('UPDATE ja_filter_control_table SET valid_flag = 0 WHERE filter_id = :id');
        $this->db->bind(':id', $id);
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves enable base calendar count.
     *
     * @param   string $id           filter id
     * @param   string $updateDate   update date of filter
     * @return  int enabled base calendar count
     * @since   Method available since version 6.1.0
     */
    public function checkBaseCalendar($id, $updateDate)
    {
        $this->db->query("SELECT COUNT(*) as count FROM ja_filter_control_table f left JOIN ja_calendar_control_table c ON f.base_calendar_id = c.calendar_id WHERE f.filter_id = :id AND f.update_date = :date AND c.valid_flag = 1");
        $this->db->bind(':id', $id);
        $this->db->bind(":date", $updateDate);
        $this->db->execute();
        return $this->db->resultSet()[0]->count;
    }

    /**
     * It retrieves base calendar information.
     *
     * @param   string $id           filter id
     * @param   string $updateDate   update date of filter
     * @return  array  base calendar data
     * @since   Method available since version 6.1.0
     */
    public function getBaseCalendar($id, $updateDate)
    {
        $this->db->query("SELECT base_calendar_id as calendar_id FROM ja_filter_control_table f  WHERE f.filter_id = :id AND f.update_date = :date ");
        $this->db->bind(':id', $id);
        $this->db->bind(":date", $updateDate);
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * It checks schedule for detete.
     *
     * @param   string $id     calendar id
     * @return  bool could be true if  related schedule info exist,could be false if not
     * @since   Method available since version 6.1.0
     */
    public function checkScheduleForDelete($id)
    {
        $this->db->query("SELECT schedule_id FROM ja_schedule_detail_table
        WHERE calendar_id = '$id'");
        $this->db->execute();
        $scheduleChk = $this->db->rowcount() > 0;

        if ($scheduleChk) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves related schedule information.
     *
     * @param   string $id     calendar id
     * @return  array related schedule info
     * @since   Method available since version 6.1.0
     */
    public function getScheduleForDelete($id)
    {
        $this->db->query("SELECT schedule_id , update_date FROM ja_schedule_detail_table
        WHERE calendar_id = '$id' GROUP BY schedule_id, update_date");
        $this->db->execute();
        return $this->db->resultSet();
    }

    /**
     * It retrieves total version count of a filter.
     *
     * @param   string  $id    filter id
     * @return  int    number of filter version object
     * @since   Method available since version 6.1.0
     */
    public function totalRows($id)
    {
        $this->db->query('SELECT COUNT(*) as count FROM ja_filter_control_table WHERE filter_id = :id');

        $this->db->bind(':id', $id);
        //$a = (int) $this->db->resultSet()[0]->count;
        return (int) $this->db->resultSet()[0]->count;
    }
}
