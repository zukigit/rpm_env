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
use App\Utils\Util;

use Exception;

/**
 * This model is used to manage the export.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ExportModel extends Model
{

    private $exportVersionSql = "SELECT * FROM %s WHERE %s = '%s' AND update_date = %d";
    private $exportMultiObjectSql = "SELECT * FROM %s WHERE %s = '%s'";
    private $exportObjectSqlOrderBy = " ORDER BY 1,2,3";
    private $exportAllSql = "SELECT * FROM %s";
    private $exportAllSqlFirst = "SELECT DISTINCT A.* FROM %s AS A, users AS U, users_groups AS UG1, users_groups AS UG2 "
        . "WHERE A.user_name=U.username AND UG1.userid=U.userid AND UG2.userid='%s' AND "
        . "UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2,3";
    private $exportAllSqlOther = "SELECT DISTINCT B.* FROM %s AS B, %s AS A,users AS U,users_groups AS UG1, users_groups AS UG2 "
        . "WHERE A.%s=B.%s AND A.update_date=B.update_date AND A.user_name=U.username AND UG1.userid=U.userid AND "
        . "UG2.userid='%s' AND UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2,3";

    private $EXPORT_CALENDAR_TABLES = ["ja_calendar_control_table", "ja_calendar_detail_table"];
    private $EXPORT_FILTER_TABLES = ["ja_filter_control_table"];
    private $EXPORT_SCHEDULE_TABLES = ["ja_schedule_control_table", "ja_schedule_detail_table", "ja_schedule_jobnet_table"];
    private $EXPORT_JOBNET_TABLES = [
        "ja_jobnet_control_table", "ja_job_control_table", "ja_flow_control_table",
        "ja_icon_calc_table", "ja_icon_end_table", "ja_icon_extjob_table",
        "ja_icon_if_table", "ja_icon_info_table", "ja_icon_jobnet_table",
        "ja_icon_job_table", "ja_job_command_table", "ja_value_job_table",
        "ja_value_jobcon_table", "ja_icon_task_table", "ja_icon_value_table",
        "ja_icon_fcopy_table", "ja_icon_fwait_table", "ja_icon_reboot_table",
        "ja_icon_release_table", "ja_icon_zabbix_link_table", "ja_icon_agentless_table"
    ];

    private $idColumns = array("calendar" => "calendar_id", "filter" => "filter_id", "schedule" => "schedule_id", "jobnet" => "jobnet_id");

    /**
     * It retrieves special version data on object type to export.
     *
     * @param   string $objectType  calendar|filter|jobnet|schedule
     * @param   array  $selectedRows
     * @return  array  $resultDataArr  special version data of object type
     * @since   Method available since version 6.1.0
     */
    public function getExportSpecialVerData($objectType, $selectedRows)
    {

        $this->logger->debug('Data retrieving process for Export special version of ' . $objectType . ' is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $idColumnName = $this->idColumns[$objectType];
            $tableNameArr = array();
            $resultDataArr = array();

            switch ($objectType) {
                case 'calendar':
                    $tableNameArr = $this->EXPORT_CALENDAR_TABLES;
                    break;
                case 'filter':
                    $tableNameArr = $this->EXPORT_FILTER_TABLES;
                    break;
                case 'schedule':
                    $tableNameArr = $this->EXPORT_SCHEDULE_TABLES;
                    break;
                case 'jobnet':
                    $tableNameArr = $this->EXPORT_JOBNET_TABLES;
                    break;
            }

            foreach ($tableNameArr as $tableName) {

                $selectSqlArr = array();
                $selectSql = '';

                foreach ($selectedRows as $row) {
                    $tempSql = sprintf($this->exportVersionSql, $tableName, $idColumnName, $row['objectId'], $row['updateDate']);
                    array_push($selectSqlArr, $tempSql);
                }
                $selectSql = implode(" UNION ALL ", $selectSqlArr);

                $this->logger->debug('Select query for export version is "' . $selectSql . '"', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                $this->db->query($selectSql);
                $result = $this->db->resultSetAsArray($tableName);

                $this->logger->debug('Total count of result data is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                if ($tableName == "ja_job_control_table" || $tableName == "ja_icon_agentless_table") {
                    $result = $this->checkPasswordToExport($result, $tableName);
                }

                foreach ($result as $key => $value) {
                    array_push($resultDataArr, array($tableName => $value));
                }
            }
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $resultDataArr = array();
        }
        $this->logger->debug('Data retrieving process for Export special version of ' . $objectType . ' is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $resultDataArr;
    }

    /**
     * It retrieves object lists to export.
     *
     * @param   string $objectType    calendar|filter|jobnet|schedule
     * @param   array  $selectedRowData
     * @return  array  $resultDataArr  object lists of object type
     * @since   Method available since version 6.1.0
     */
    public function getExportObjectData($objectType, $selectedRowData)
    {
        $this->logger->debug('Data retrieving process for Export object (' . $objectType . ') is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $idColumnName = $this->idColumns[$objectType];
            $tableNameArr = array();
            $resultDataArr = array();

            switch ($objectType) {
                case 'calendar':
                    $tableNameArr = $this->EXPORT_CALENDAR_TABLES;
                    break;
                case 'filter':
                    $tableNameArr = $this->EXPORT_FILTER_TABLES;
                    break;
                case 'schedule':
                    $tableNameArr = $this->EXPORT_SCHEDULE_TABLES;
                    break;
                case 'jobnet':
                    $tableNameArr = $this->EXPORT_JOBNET_TABLES;
                    break;
            }

            foreach ($tableNameArr as $tableName) {

                $selectSqlArr = array();
                $selectSql = '';

                foreach ($selectedRowData as $row) {
                    $tempSql = sprintf($this->exportMultiObjectSql, $tableName, $idColumnName, $row['id']);
                    array_push($selectSqlArr, $tempSql);
                }
                $selectSql = implode(" UNION ALL ", $selectSqlArr) . $this->exportObjectSqlOrderBy;

                $this->logger->debug('Select query for export object is "' . $selectSql . '"', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                $this->db->query($selectSql);
                $result = $this->db->resultSetAsArray($tableName);

                $this->logger->debug('Total count of result data is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                if ($tableName == "ja_job_control_table" || $tableName == "ja_icon_agentless_table") {
                    $result = $this->checkPasswordToExport($result, $tableName);
                }

                foreach ($result as $key => $value) {
                    array_push($resultDataArr, array($tableName => $value));
                }
            }
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $resultDataArr = array();
        }
        $this->logger->debug('Data retrieving process for Export object (' . $objectType . ') is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $resultDataArr;
    }

    /**
     * It retrieves all object lists to export.
     *
     * @param   string  $loginUserId    id of current logged in user
     * @return  array  $resultDataArr  object lists of object type
     * @since   Method available since version 6.1.0
     */
    public function getExportAllData($loginUserId)
    {

        $this->logger->debug('Data retrieving process for Export All is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $tableNameArr = array();
            $resultDataArr = array();

            foreach ($this->idColumns as $objectType => $idColName) {

                switch ($objectType) {
                    case 'calendar':
                        $tableNameArr = $this->EXPORT_CALENDAR_TABLES;
                        break;
                    case 'filter':
                        $tableNameArr = $this->EXPORT_FILTER_TABLES;
                        break;
                    case 'schedule':
                        $tableNameArr = $this->EXPORT_SCHEDULE_TABLES;
                        break;
                    case 'jobnet':
                        $tableNameArr = $this->EXPORT_JOBNET_TABLES;
                        break;
                }

                $count = 0;
                foreach ($tableNameArr as $tableName) {
                    $selectSqlArr = array();
                    $selectSql = sprintf($this->exportAllSql, $tableName);

                    if ($_SESSION['userInfo']['userType'] != Constants::USER_TYPE_SUPER) {
                        if ($count == 0) {
                            $selectSql = sprintf($this->exportAllSqlFirst, $tableName, $loginUserId);
                            $count++;
                        } else {
                            $selectSql = sprintf($this->exportAllSqlOther, $tableName, $tableNameArr[0], $idColName, $idColName, $loginUserId);
                        }
                    }
                    $this->logger->debug('Select query for export all is "' . $selectSql . '"', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $this->db->query($selectSql);
                    $result = $this->db->resultSetAsArray($tableName);

                    if ($tableName == "ja_job_control_table" || $tableName == "ja_icon_agentless_table") {
                        $result = $this->checkPasswordToExport($result, $tableName);
                    }

                    foreach ($result as $key => $value) {
                        array_push($resultDataArr, array($tableName => $value));
                    }
                    $this->logger->debug('Total count of result data is ' . count($result), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $resultDataArr = array();
        }
        $this->logger->debug('Data retrieving process for Export All is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $resultDataArr;
    }

    /**
     * It checks password to export.
     *
     * @param   array  $dataArray    object lists to export with encrypted password
     * @param   string $tableName    table name to check password
     * @return  array  $dataArray    object lists to export with decrypted password
     * @since   Method available since version 6.1.0
     */
    public function checkPasswordToExport($dataArray, $tableName)
    {

        $util = new Util();
        $count = count($dataArray);
        if ($tableName == "ja_job_control_table") {
            $key = "run_user_password";
        } else {
            $key = "login_password";
        }
        for ($i = 0; $i < $count; $i++) {
            if (array_key_exists($key, $dataArray[$i])) {
                if (!$util->IsNullOrEmptyString($dataArray[$i][$key])) {
                    if (!$util->startsWith($dataArray[$i][$key], '1')) {
                        $dataArray[$i][$key] = $util->getStringFromPassword($dataArray[$i][$key]);
                        $dataArray[$i][$key] = $util->getPasswordFromString($dataArray[$i][$key]);
                    }
                }
            }
        }
        return $dataArray;
    }
}
