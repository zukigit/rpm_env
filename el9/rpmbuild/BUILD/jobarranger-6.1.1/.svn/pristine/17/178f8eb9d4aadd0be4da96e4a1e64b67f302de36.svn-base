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

use Exception;

/**
 * This model is used to manage the import process.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ImportModel extends Model
{

    /**
     * It checks login user & user from XML are in same group.
     *
     * @param  string  $loginUser  logged in user name
     * @param  string  $user       user name from xml file   
     * @throws Exception
     * @return array|string $result could be array if same group,could be string if not
     * @since      Class available since version 6.1.0
     */
    public function checkUserSameGP($loginUser, $user)
    {
        $this->logger->debug("Retrieving & checking same group user process is started.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $selectQuery = "SELECT UG1.usrgrpid FROM users U1,users U2,users_groups AS UG1,users_groups AS UG2 
            WHERE U1.username = '%s'AND U2.username = '%s' AND UG1.userid = U1.userid AND UG2.userid = U2.userid 
            AND UG1.usrgrpid = UG2.usrgrpid";
            $selectQuery =  sprintf($selectQuery, $user, $loginUser);
            $this->db->query($selectQuery);
            $result = $this->db->singleAsArrayInCamelCase();
            if (count($result) <= 0) $result = false;
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $result = Constants::AJAX_MESSAGE_DB_EXEC_ERROR;
        }
        $this->logger->debug("Retrieving & checking same group user process is finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It checks data exist in control table and delete.
     *
     * @param  string  $ctrlTblKeys
     * @param  string  $tblName
     * @param  string  $tblColVal
     * @param  string  $overwrite
     * @throws Exception
     * @return bool|string could be array if control table exists and delete process success,could be string if not
     * @since      Class available since version 6.1.0
     */
    public function checkControlTbl($ctrlTblKeys, $tblName, $tblColVal, $overwrite)
    {
        $this->logger->debug('Retrieving & checking control tables process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = true;
        $tblColVal = json_decode(json_encode($tblColVal), TRUE);
        try {
            $selectQuery = "SELECT * FROM %s WHERE %s = '%s' AND %s = '%s'";
            $selectQuery = sprintf($selectQuery, $tblName, $ctrlTblKeys['colId'], $tblColVal[$ctrlTblKeys['colId']], $ctrlTblKeys['colDate'], $tblColVal[$ctrlTblKeys['colDate']]);
            $this->db->query($selectQuery);
            $ctrlResult = $this->db->resultSetAsArrayInCamelCase();

            if (count($ctrlResult) == 1) {
                if (!$overwrite) $result = false;
                else {
                    $deleteQuery = "DELETE FROM %s WHERE %s = '%s' AND %s = '%s'";
                    $deleteQuery = sprintf($deleteQuery, $tblName, $ctrlTblKeys['colId'], $tblColVal[$ctrlTblKeys['colId']], $ctrlTblKeys['colDate'], $tblColVal[$ctrlTblKeys['colDate']]);
                    $this->db->query($deleteQuery);

                    if (!$this->db->execute()) {
                        $result = Constants::AJAX_MESSAGE_IMPORT_ERROR;
                    }
                }
            }
        } catch (Exception $e) {
            $result = Constants::AJAX_MESSAGE_DB_EXEC_ERROR;
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Retrieving & checking control tables process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It checks related column exist in related table.
     *
     * @param  string  $relKeys
     * @param  string  $tbl
     * @param  string  $colVal
     * @throws Exception
     * @return bool|string could be array if related data exists,could be string if not
     * @since      Class available since version 6.1.0
     */
    public function checkRelatedTbl($relKeys, $tbl, $colVal)
    {
        $this->logger->debug('Retrieving & checking other related tables process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = true;
        try {
            $query = "SELECT * FROM %s WHERE %s = '%s'";
            $selectQuery = sprintf($query, $relKeys['relTbl'], $relKeys['relColKey'], $colVal[$relKeys['colKey']]);
            $this->db->query($selectQuery);
            $relResult = $this->db->resultSetAsArrayInCamelCase();
            if (count($relResult) <= 0 && $tbl == "ja_schedule_detail_table") {
                $selectQuery = sprintf($query, $relKeys['relTbl2'], $relKeys['relColKey2'], $colVal[$relKeys['colKey']]);
                $this->db->query($selectQuery);
                $relResult = $this->db->resultSetAsArrayInCamelCase();
            }
            if (count($relResult) <= 0) $result =  false;
        } catch (Exception $e) {
            $result = Constants::AJAX_MESSAGE_DB_EXEC_ERROR;
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('Retrieving & checking other related tables process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It inserts import information to tables.
     *
     * @param  string  $tblName
     * @param  object  $tblColVal
     * @param  bool    $logFlag
     * @throws Exception
     * @return bool could be true if insert process success,could be false if not
     * @since      Class available since version 6.1.0
     */
    public function insertTblInfo($tblName, $tblColVal, $logFlag)
    {
        if ($logFlag) {
            $this->logger->debug('Insert tables process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        // $tblColVal = json_decode(json_encode($tblColVal), TRUE);
        $insertQuery = "INSERT INTO " . $tblName . " (";
        $insertVal = "";
        $colCount = 0;
        $result = true;
        try {
            foreach ($tblColVal as $colName => $colValue) {
                if (!is_array($colValue)) {
                    if ($colCount > 0) {
                        $insertQuery .= ",";
                        $insertVal .= ",";
                    }
                    $insertQuery .= $colName;
                    $insertVal .= ":" . $colName;
                    $colCount++;
                }
            }
            $insertQuery .= ") VALUES (" . $insertVal . ")";

            $this->db->query($insertQuery);
            foreach ($tblColVal as $colName => $colValue) {
                if (!is_array($colValue)) {
                    if ($colName == "valid_flag") {
                        $this->db->bind(':' . $colName, '0', null, $logFlag);
                    } else {
                        $this->db->bind(':' . $colName, $colValue, null, $logFlag);
                    }
                }
            }

            if (!$this->db->execute($logFlag)) {
                $result = false;
            }
        } catch (Exception $e) {
            $result = $e->getMessage();
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        if ($logFlag) {
            $this->logger->debug('Insert tables process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        return $result;
    }
}
