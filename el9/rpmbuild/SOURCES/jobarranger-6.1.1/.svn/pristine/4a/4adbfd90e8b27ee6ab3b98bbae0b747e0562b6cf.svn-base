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
 * This model is used to manage the schedule boottime.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class BoottimeModel extends Model
{

    /**
     * It retrieves all public calendar lists.
     * 
     * @throws  PDOException
     * @return  array public calendar lists
     * @since   Method available since version 6.1.0
     */
    public function getPublicCalendar()
    {
        $this->logger->debug('Public Calendar list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = [];
        try {
            $this->db->query("SELECT calendar_id,MAX(update_date) AS update_date FROM ja_calendar_control_table
            WHERE public_flag = '1' group by calendar_id order by calendar_id");
            $result = $this->db->resultSetAsArrayInCamelCase();
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Public Calendar list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves all private calendar list depend on user priviledge.
     *
     * @throws  PDOException
     * @return  array private calendar lists
     * @since   Method available since version 6.1.0
     */
    public function getPrivateCalendar()
    {
        $result = [];
        $this->logger->debug('Private Calendar list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $this->db->query("SELECT calendar_id,MAX(update_date) AS update_date FROM ja_calendar_control_table
                    WHERE public_flag = '0' group by calendar_id order by calendar_id");
            } else {

                $userid = $_SESSION['userInfo']['userId'];
                $this->db->query("SELECT CALENDAR.calendar_id,CALENDAR.update_date FROM ( SELECT * FROM ja_calendar_control_table 
                WHERE valid_flag = 1 AND public_flag = 0 
                UNION ALL  
                SELECT * FROM ja_calendar_control_table A 
                WHERE A.update_date= ( SELECT MAX(update_date) FROM ja_calendar_control_table B 
                WHERE B.calendar_id NOT IN (SELECT calendar_id FROM ja_calendar_control_table  
                WHERE valid_flag = 1 )  
                AND B.public_flag = 0 AND A.calendar_id = B.calendar_id 
                GROUP BY calendar_id  )
                  ) AS CALENDAR, 
                users AS U, users_groups AS UG1, users_groups AS UG2  
                WHERE CALENDAR.user_name = U.username  
                AND U.userid = UG1.userid 
                AND UG2.userid=$userid 
                AND UG1.usrgrpid = UG2.usrgrpid   ORDER BY CALENDAR.calendar_id ");
            }
            $result = $this->db->resultSetAsArrayInCamelCase();
            $this->logger->debug('Total count of Private Calendar is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Private Calendar list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves all public filter lists.
     *
     * @throws  PDOException
     * @return  array public filter lists
     * @since   Method available since version 6.1.0
     */
    public function getPublicFilter()
    {
        $result = [];
        $this->logger->debug('Public Filter list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("SELECT filter_id,MAX(update_date) AS update_date FROM ja_filter_control_table
                WHERE public_flag = '1' group by filter_id order by filter_id");
            $result = $this->db->resultSetAsArrayInCamelCase();
            $this->logger->debug('Total count of Public Filter is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Public Filter list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves all private filter list depend on user priviledge.
     *
     * @throws  PDOException
     * @return  array private filter lists
     * @since   Method available since version 6.1.0
     */
    public function getPrivateFilter()
    {
        $this->logger->debug('Private Filter list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = [];
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $this->db->query("SELECT filter_id,MAX(update_date) AS update_date FROM ja_filter_control_table
                    WHERE public_flag = '0' group by filter_id order by filter_id");
            } else {
                $userid = $_SESSION['userInfo']['userId'];
                $this->db->query("SELECT distinct A.filter_id,A.update_date 
                    FROM ja_filter_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 
                    WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=$userid
                    and UG1.usrgrpid = UG2.usrgrpid and 
                    A.update_date= ( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B 
                    WHERE B.filter_id = A.filter_id group by B.filter_id) and A.public_flag='0' order by A.filter_id");
            }
            $result = $this->db->resultSetAsArrayInCamelCase();
            $this->logger->debug('Total count of Private Filter is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Private Filter list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves base calendar info depend on calendar id.
     *
     * @param   string $optID  base calendar id
     * @throws  PDOException
     * @return  array  base calendar info 
     * @since   Method available since version 6.1.0
     */
    public function getCalendarInfo($optID)
    {
        $result = [];
        $this->logger->debug('Calendar data retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("select * from ja_calendar_control_table where calendar_id = '$optID' and valid_flag = '1'");
            $result = $this->db->singleAsArrayInCamelCase();
            if (empty($result)) {
                $this->db->query("select * from ja_calendar_control_table where calendar_id = '$optID' and 
                    update_date = (select max(update_date) from ja_calendar_control_table where calendar_id='$optID')");
                $result = $this->db->singleAsArrayInCamelCase();
            }
            $this->logger->debug('Calendar info  . [' . json_encode($result) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Calendar data retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves filter information.
     *
     * @param   string $optID  filter id
     * @param   string $updateDate
     * @throws  PDOException
     * @return  array  filter info 
     * @since   Method available since version 6.1.0
     */
    public function getFilterInfo($optID, $updateDate)
    {
        $result = [];
        $this->logger->debug('Filter data retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("select * from ja_filter_control_table where filter_id = '$optID' and valid_flag = '1'");
            $result = $this->db->singleAsArrayInCamelCase();
            if (empty($result)) {
                $this->db->query("SELECT * FROM ja_filter_control_table WHERE filter_id = '$optID' AND update_date='$updateDate'");
                $result = $this->db->singleAsArrayInCamelCase();
            }
            $this->logger->debug('Filter info  . [' . json_encode($result) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Filter data retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves all public jobnet lists.
     *
     * @throws  PDOException
     * @return  array public jobnet lists
     * @since   Method available since version 6.1.0
     */
    public function getPublicJobnet()
    {
        $result = [];
        $this->logger->debug('Public Jobnet list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("SELECT jobnet_id,Max(update_date) AS update_date FROM ja_jobnet_control_table
            WHERE public_flag = '1' group by jobnet_id order by jobnet_id");
            $result = $this->db->resultSetAsArrayInCamelCase();
            $this->logger->debug('Total count of Public Jobnet is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Public Jobnet list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves all private jobnet lists depend on user priviledge.
     *
     * @throws  PDOException
     * @return  array private jobnet lists
     * @since   Method available since version 6.1.0
     */
    public function getPrivateJobnet()
    {
        $result = [];
        $this->logger->debug('Private Jobnet list retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $this->db->query("SELECT jobnet_id,Max(update_date) AS update_date FROM ja_jobnet_control_table
                WHERE public_flag = '0' group by jobnet_id order by jobnet_id");
            } else {

                $userid = $_SESSION['userInfo']['userId'];
                $this->db->query("SELECT distinct A.jobnet_id,A.update_date 
                    FROM ja_jobnet_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 
                    WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=$userid
                    and UG1.usrgrpid = UG2.usrgrpid and 
                    A.update_date= ( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B 
                    WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) and A.public_flag='0' order by A.jobnet_id");
            }
            $result = $this->db->resultSetAsArrayInCamelCase();
            $this->logger->debug('Total count of Private Jobnet is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Private Jobnet list retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves jobnet information.
     *
     * @param   string $optID  jobnet id
     * @param   string $updateDate
     * @throws  PDOException
     * @return  array  jobnet info 
     * @since   Method available since version 6.1.0
     */
    public function getJobnetInfo($optID)
    {
        $this->logger->debug('Jobnet data retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->db->query("select * from ja_jobnet_control_table where jobnet_id = '$optID' and valid_flag = '1'");
            $result = $this->db->singleAsArrayInCamelCase();
            if (empty($result)) {
                $this->db->query("select * from ja_jobnet_control_table 
              where jobnet_id = '$optID' and update_date = (select max(update_date) from ja_jobnet_control_table where jobnet_id='$optID')");
                $result = $this->db->singleAsArrayInCamelCase();
            }
            $this->logger->debug('Jobnet info  . [' . json_encode($result) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Jobnet data retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It retrieves the last day of the calendar.
     *
     * @param   string $optID  calendar id
     * @param   string $type
     * @param   string $updateDate
     * @throws  PDOException
     * @return  array  last day of calendar
     * @since   Method available since version 6.1.0
     */
    public function getCalLastDay($optID, $type, $updateDate)
    {
        $this->logger->debug('Last day retrieving process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            if (strpos($type, 'calendar') !== false) {
                $this->db->query(" SELECT MAX(operating_date) AS max FROM ja_calendar_detail_table WHERE calendar_id = '$optID' and update_date='$updateDate'");
            } else {
                $this->db->query("SELECT MAX(operating_date) AS max FROM ja_calendar_detail_table WHERE calendar_id = '$optID'");
            }
            $result = $this->db->singleAsArrayInCamelCase();
            $this->logger->debug('Last Day  ' . json_encode($result) . $optID . $updateDate . $type, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Last day retrieving process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }
}
