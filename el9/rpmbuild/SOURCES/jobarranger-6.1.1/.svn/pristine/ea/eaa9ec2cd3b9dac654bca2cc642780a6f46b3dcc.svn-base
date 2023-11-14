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

namespace App\Controllers;

use App\Utils\Controller;
use App\Utils\Core;
use App\Utils\Constants;
use App\Models\ImportModel;
use Exception;
use App\Utils\Util;
use PDOException;
use DateTime;
use Rakit\Validation\Validator;

/**
 * This controller is used to manage the import.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Import extends Controller
{

    // Key list format is ("check table" =>array(check Column1,check Column2)
    private $controlTblKeyList = array(
        'ja_calendar_control_table' => array('colId' => 'calendar_id', 'colDate' => 'update_date'),
        'ja_filter_control_table' => array('colId' => 'filter_id', 'colDate' => 'update_date'),
        'ja_schedule_control_table' => array('colId' => 'schedule_id', 'colDate' => 'update_date'),
        'ja_jobnet_control_table' => array('colId' => 'jobnet_id', 'colDate' => 'update_date')
    );

    public function __construct()
    {
        parent::__construct();
        $this->importModel = new ImportModel();
        $this->logger = Core::logger();
        $this->validator = new Validator();
        $this->util = new Util();
    }

    // Key list format is ("check table" =>array('check column','related table','related column'))
    private $relatedTblKeyList = array(
        'ja_filter_control_table' => array('colKey' => 'base_calendar_id', 'relTbl' => 'ja_calendar_control_table', 'relColKey' => 'calendar_id'),
        'ja_schedule_detail_table' => array('colKey' => 'calendar_id', 'relTbl' => 'ja_calendar_control_table', 'relColKey' => 'calendar_id', 'relTbl2' => 'ja_filter_control_table', 'relColKey2' => 'filter_id'),
        'ja_schedule_jobnet_table' => array('colKey' => 'jobnet_id', 'relTbl' => 'ja_jobnet_control_table', 'relColKey' => 'jobnet_id'),
        'ja_icon_jobnet_table' => array('colKey' => 'link_jobnet_id', 'relTbl' => 'ja_jobnet_control_table', 'relColKey' => 'jobnet_id'),
        'ja_icon_task_table' => array('colKey' => 'submit_jobnet_id', 'relTbl' => 'ja_jobnet_control_table', 'relColKey' => 'jobnet_id'),
        'ja_icon_extjob_table' => array('colKey' => 'command_id', 'relTbl' => 'ja_define_extjob_table', 'relColKey' => 'command_id'),
        'ja_icon_value_table' => array('colKey' => 'value_name', 'relTbl' => 'ja_define_value_jobcon_table', 'relColKey' => 'value_name')
    );

    /**
     * api endpoint that import xml data into database.
     * It checks file content,format and user permission then import object.
     *
     * @return  array message type and message info
     * @since   Method available since version 6.1.0
     */
    public function importXML()
    {

        $this->logger->info('Import xml process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $overwriteFlag = false;
        $json = file_get_contents('php://input');

        $params = Util::jsonDecode($json)["params"];
        $validation = $this->validator->validate($params, [
            'chkOverwrite' => 'required',
            'fileContent' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return;
        } else {
            try {
                $fileData = json_decode($params['fileContent'], true);

                // libxml_use_internal_errors(true);
                //Read XML File
                $xml = @simplexml_load_string($fileData);
                if (false === $xml) {
                    // $error = libxml_get_errors();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "ERR_XML_FORMAT"]);
                    return;
                }

                if (isset($params['chkOverwrite']) && $params['chkOverwrite'] == "true") {
                    $overwriteFlag = true;
                }

                $userInfo = $this->checkUserInfo($xml);
                if (!$userInfo) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "ERR_USER_INFO"]);
                    return;
                }

                $loginUser = [
                    "userName" => $_SESSION['userInfo']['userName'],
                    "userType" => $_SESSION['userInfo']['userType']
                ];

                $userPermission = $this->checkUserPermission($userInfo, $loginUser);
                if ($userPermission != '1') {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $userPermission]);
                    return;
                }

                [$result, $data] = $this->import($xml, $overwriteFlag);
                if ($result == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
                    return;
                } else {
                    $this->logger->info('Import xml process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response($data, [Constants::API_RESPONSE_MESSAGE => $result]);
                    return;
                }
            } catch (Exception $e) {
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            }
        }
    }

    /**
     * It checks data exist in control table.
     *
     * @throws PDOException
     * @return array
     * @since  Class available since version 6.1.0
     */
    public function import(object $xmlDataArr, bool $overwriteFlag)
    {
        try {
            $this->beginTransaction();
            // Data Existence Check and insert tbl
            foreach ($xmlDataArr as $eleName => $eleVal) {
                $tableName = $eleName;
                if ($tableName != "UserInfo") {
                    //check for control tables
                    $checkCtrl = $this->checkCtrlTbl($tableName, $eleVal, $overwriteFlag);
                    if ($checkCtrl !== true) {
                        if ($checkCtrl == 'ERR_DB') {
                            return [Constants::SERVICE_INTERNAL_SERVER_ERROR, Constants::API_RESPONSE_TYPE_500];
                        } else {
                            return [$checkCtrl, Constants::API_RESPONSE_TYPE_INCOMPLETE];
                        }
                    }

                    //check for other related tables
                    // $checkOtherRel= $this->checkRelatedTbl($xmlDataArr,$tableName,$eleVal);
                    // if($checkOtherRel !== true){                          
                    //    return $this->showErrMsg($checkOtherRel);
                    // }

                    //Date validation
                    if (!empty($eleVal->created_date)) {
                        $date = $eleVal->created_date;
                        if (date_create_from_format(DateTime::ATOM, $date) || date_create_from_format("Y-m-d H:i:s", $date) || date_create_from_format("Y-m-d\TH:i:s.uP", $date) || date_create_from_format("Y-m-d H:i:s.u", $date)) {
                            //$createdDate = date("Y-m-d H:i:s", strtotime($date));
                            $createdDate = date_format(date_create($date), "Y-m-d H:i:s");
                            $eleVal->created_date = $createdDate;
                        } else {
                            return ["ERR_IMPORT", Constants::API_RESPONSE_TYPE_INCOMPLETE];
                        }
                    }
                    if (isset($eleVal->update_date) && !$this->validateUpdateDate($eleVal->update_date)) {
                        return ["ERR_IMPORT", Constants::API_RESPONSE_TYPE_INCOMPLETE];
                    }

                    //insert data to tables
                    in_array($tableName, array_keys($this->controlTblKeyList)) ? $logFlag = true : $logFlag = false;
                    $insertTbl = $this->importModel->insertTblInfo($tableName, $eleVal, $logFlag);
                    if ($insertTbl !== true) {
                        $this->logger->info('An error occured in inserting tables.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        if (strpos($insertTbl, "Duplicate") !== false) {
                            return ["ERR_REGISTERED", Constants::API_RESPONSE_TYPE_INCOMPLETE];
                        } else {
                            return [Constants::SERVICE_INTERNAL_SERVER_ERROR, Constants::API_RESPONSE_TYPE_500];
                        }
                    }
                }
            }
            $this->commit();
            $this->logger->info('Import data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return ["SUCCESS_MSG", Constants::API_RESPONSE_TYPE_OK];
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return [Constants::SERVICE_INTERNAL_SERVER_ERROR, Constants::API_RESPONSE_TYPE_500];
        }
    }

    /**
     * It checks data exist in control table.
     *
     * @return bool
     * @since  Class available since version 6.1.0
     */
    public function checkCtrlTbl(string $tblName, object $colValue, bool $overwriteFlag)
    {
        $result = true;
        if (in_array($tblName, array_keys($this->controlTblKeyList))) {
            try {
                $idExist = $this->importModel->checkControlTbl($this->controlTblKeyList[$tblName], $tblName, $colValue, $overwriteFlag);
                if ($idExist !== true) {
                    switch ($idExist) {
                        case Constants::AJAX_MESSAGE_IMPORT_ERROR:
                            $result = "ERR_IMPORT";
                            break;
                        case Constants::AJAX_MESSAGE_DB_EXEC_ERROR:
                            $result = "ERR_DB";
                            break;
                        case false:
                            $result = "ERR_REGISTERED";
                            break;
                    }
                    $this->logger->info('An error occured in checking control tables', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $this->logger->info('Result : ' . $result, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            } catch (PDOException $e) {
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->debug('Control tables checking process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It checks update date format.
     *
     * @return bool
     * @since  Class available since version 6.1.0
     */
    public function validateUpdateDate(string $date)
    {
        $result = false;
        $upDate = date_create_from_format("YmdHis", $date);
        if ($upDate && $upDate->format("YmdHis") == $date) $result = true;
        else $this->logger->info('An error occurred in update date.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $this->logger->debug('Update date validation process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }

    /**
     * It checks related data exist in DB.
     *
     * @return bool
     * @since  Class available since version 6.1.0
     */
    public function checkRelatedTable(string $relKeys, string $curTblName, string $curColValue)
    {
        try {
            $checkOthRel = $this->importModel->checkRelatedTbl($relKeys, $curTblName, $curColValue);
            return $checkOthRel;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks user info in xml file is exists or not.
     *
     * @return  object | bool  return user name and type if user is exist. if not, return false.
     * @since   Method available since version 6.1.0
     */
    public function checkUserInfo(object $xmlData)
    {
        foreach ($xmlData as $key => $val) {
            if ($key == "UserInfo") {
                //$val = json_decode(json_encode($val), TRUE);
                if (isset($val->user_name) && isset($val->type)) {
                    if (!empty($val->user_name) && !empty($val->type)) {
                        return $val;
                    }
                }
            }
        }
        $this->logger->info('Check User info process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return false;
    }

    /**
     * It checks user has permission or not to import.
     *
     * @return  string could be '1' if user has permission, could be error message if not.
     * @since   Method available since version 6.1.0
     */
    public function checkUserPermission(object $userInfo, array $loginUserInfo)
    {
        try {
            if ($loginUserInfo['userType'] == strval(Constants::USER_TYPE_SUPER)) {
                $result =  '1';
            } else {
                if ($loginUserInfo['userType'] == Constants::USER_TYPE_GENERAL || (strcasecmp($userInfo->type, "SUPER") == 0 && $loginUserInfo['userType'] < Constants::USER_TYPE_SUPER)) {
                    $result = "ERR_PERMISSION";
                } elseif ($loginUserInfo['userName'] == $userInfo->user_name && $loginUserInfo['userType'] != Constants::USER_TYPE_GENERAL) {
                    $result =  '1';
                } else {
                    $userSameGP = $this->importModel->checkUserSameGP($loginUserInfo['userName'], $userInfo->user_name);
                    if ($userSameGP === Constants::AJAX_MESSAGE_DB_EXEC_ERROR) $result = "ERR_DB";
                    else if ($userSameGP) $result =  '1';
                    else $result = "ERR_PERMISSION";
                }
            }
            $this->logger->info('Check User permission process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $result;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }
}
