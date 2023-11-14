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
use App\Services\ObjectLockService;
use App\Models\IndexModel;
use Rakit\Validation\Rules\Json;
use App\Models\ObjectLockModel;
use App\Utils\Util;
use Rakit\Validation\Validator;
use App\Models\ScheduleModel;
use DateTime;
use PDOException, Exception;

/**
 * This controller is used to manage the schedule.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Schedule extends Controller
{
    public function __construct()
    {
        parent::__construct();
        $this->indexModel = new IndexModel();
        $this->scheduleModel = new ScheduleModel();
        $this->utils = new Util();
        $this->objectLockService = new ObjectLockService();
        $this->logger = Core::logger();
        $this->validator = new Validator();
        $this->objectLockModel = new ObjectLockModel();
    }

    /**
     * It deletes the schedule list from object_list screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteList()
    {
        $this->logger->info('Multiple schedule delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $request = json_decode($json, true);
        $result = $this->deleteListService($request["params"]["datas"]);
        if (!is_array($result)) {
            $result = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, '');

            return $result;
        } else {
            $this->logger->info('Multiple schedule delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $result;
        }
    }

    /**
     * It deletes the schedule from object_list
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function deleteListService($para)
    {
        $this->logger->info('Multiple schedule delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->beginTransaction();
            $selectRows = $para['selectedRows'];

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $objectDetail = $this->scheduleModel->each($objectId);
                if ($objectDetail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND, $objectId);
                    return $data;
                }
                $detail = $this->getScheduleDetail($objectId, $update_date);
                if ($detail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);

                    return $data;
                }
                if (!$this->checkValid($detail, Constants::OBJ_DELETE_PROC)) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    $this->commit();
                    return $data;
                } //check if valid. Do
                if ($detail['validFlag'] == 1) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    $this->commit();
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_SCHEDULE, "attemptIp" =>  $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    $this->commit();
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    $this->commit();
                    return $data;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                }
            }
            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                if ($this->scheduleModel->deleteAllVer($objectId) == true) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                    $this->logger->info('[' . $objectId . '] schedule has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                    break;
                }
            }

            $this->commit();
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
        }
        $this->logger->info('Multiple schedule delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It deletes the schedule version from object_version screen
     *
     * @since   Method available since version 6.1.0
     */
    public function delete()
    {
        $this->logger->info('Schedule delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $request = json_decode($json);
        $validationReq = (array)$request;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'data' => 'required',
            'category' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $tmpValidatorArray = $validationReq["params"]->data;
        $validation = $this->validator->validate((array)$tmpValidatorArray['0'], [
            'updateDate' => 'required',
            'validState' => 'required',
            'objectId' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $this->logger->info('Schedule delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        //$request = json_decode($_POST['datas'], true);
        $result = $this->deleteService($request->params);
        if (!is_array($result)) {
            $data = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        } else if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result); // type = ok
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
        }
        $this->logger->info('Schedule delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * Check and delete process.
     *
     * @param   object $para  send data from the browser.
     * @return  array|string  result as json messages.
     * @since   Method available since version 6.1.0
     */
    public function deleteService($para)
    {
        $this->logger->info('schedule change valid process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $deleteRows = $para->data;
            //check for validation.
            foreach ($deleteRows as $selectRows) {
                $objectId = $selectRows->objectId;
                $detail = $this->getScheduleDetail($objectId, $selectRows->updateDate);
                if ($detail == false) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                    return $data;
                }
                //check if valid. Do
                if ($detail['validFlag'] == 1) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    return $data;
                }

                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_SCHEDULE, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                }
            }
            //start delete process.
            $this->beginTransaction();
            $breakflag = false;
            foreach ($deleteRows as $selectRows) {
                if ($this->scheduleModel->deleteArr($objectId, $deleteRows) == true) {
                    $this->logger->info('[' . $objectId . '] calendar has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $breakflag = true;
                }
            }
            //$totalRows = $this->scheduleModel->totalRows($objectId);

            if ($breakflag == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                $this->rollback();
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                $this->commit();
            }
            $this->logger->info('schedule version Disable process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Schedule change valid process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It remove deleted boottime from session
     *
     * @since   Method available since version 6.1.0
     */
    public function removeBoottimeCreate()
    {
        $arrkey = $_POST['remove_detail'];
        //$schedule_id=$_POST['schedule_id'];
        foreach ($arrkey as $key => $value) {
            foreach ($_SESSION['optiondatas'] as $sessionKey => $sessionVal) {
                if ($value == $sessionKey) {
                    //Remove from session
                    if ($sessionVal['dbFlag'] == "0") {
                        array_push(
                            $_SESSION['delBoottimeDatas'],
                            [
                                'id' => $sessionVal['id'],
                                'name' => $sessionVal['name'],
                                'time' => $sessionVal['time']
                            ]
                        );
                    }
                    unset($_SESSION['optiondatas'][$sessionKey]);
                    break;
                }
            }
        }
    }

    /**
     * It remove deleted jobnet from session
     *
     * @since   Method available since version 6.1.0
     */
    public function removeJobnetCreate()
    {
        $arrkey = $_POST['remove_detail'];
        foreach ($arrkey as $key => $value) {
            foreach ($_SESSION['job_net_optiondatas'] as $sessionKey => $sessionVal) {
                if ($value == $sessionKey) {
                    //Remove from session
                    if ($sessionVal['dbFlag'] == "0") {
                        array_push(
                            $_SESSION['delJob_net_Datas'],
                            [
                                'jobnetid' => $sessionVal['jobnetid'],
                                'jobnetname' => $sessionVal['jobnetname']
                            ]
                        );
                    }
                    unset($_SESSION['job_net_optiondatas'][$sessionKey]);
                    break;
                }
            }
        }
    }


    /**
     * The create controller action
     * 
     * It redirects to schedule_create screen with init data.
     *
     * @since   Method available since version 6.1.0
     */
    public function initCreate()
    {
        $this->logger->info('Schedule create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        // Converts it into a PHP object
        $data = Util::jsonDecode($json)["params"];

        $validation = $this->validator->validate($data, [
            'type' => 'required',
            'isPublic' => 'required',
        ]);
        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            try {
                $name = "";
                $description = "";
                $updateDate = "";
                $scheduleInfo = "";
                $calendarInfo = "";
                $jobnetInfo = "";
                $result = "";
                $isPublic = $data["isPublic"];
                $lastid = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_SCHEDULE);
                $calDetailLists = [];
                $jobnetDetailLists = [];
                $authority = true;
                $editable = 1;

                $result = [
                    'id'  => 'SCHEDULE_' . $lastid->nextid,
                    'name' => $name,
                    'description' => $description,
                    'updateDate' => $updateDate,
                    'isPublic' => $isPublic,
                    'scheduleInfo'  => $scheduleInfo,
                    'calendarInfo'  => $calendarInfo,
                    'jobnetInfo'  => $jobnetInfo,
                    'formType' =>  Constants::OBJECT_FORM_CREATE,
                    'editable' => $editable,
                    'authority' => $authority,
                    'userName' => $_SESSION["userInfo"]["userName"],
                    'calDetailLists' => $calDetailLists,
                    'jobnetDetailLists' => $jobnetDetailLists,
                ];
                $this->logger->info('Schedule create initialization process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
            } catch (PDOException $e) {
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            }
        }
        $this->logger->info('Schedule create initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves the calendar,jobnet and schedule data.
     *
     * @param   string $id           id of the schedule.
     * @param   string $updateDate   updated date of the schedule.
     * @return  array|string         could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function retrieveData($objData)
    {
        $this->logger->info('retrieving calendar,jobnet,schedule data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $id = $objData["objectId"];
            $updateDate = $objData["date"];
            $scheduleInfo = $this->scheduleModel->each($id, $updateDate); // retrieve schedule info for new obj create
            $calendarInfo = $this->scheduleModel->getBoottime($id, $updateDate); // retrieve Calendar info related 

            $calDetailLists = [];
            $jobnetDetailLists = [];
            $time = "";
            foreach ($calendarInfo as $calVal) {

                if ($calVal->object_flag == "0") {
                    $calDetail = $this->scheduleModel->getCalendarDetail($calVal->calendar_id); // edit will std class calendarId
                    foreach ($calDetail as $val) {

                        if ($calVal->boot_time) {
                            $chgtime = str_split($calVal->boot_time, 2);
                            $time = $chgtime[0] . ":" . $chgtime[1];
                        }
                        array_push(
                            $calDetailLists,
                            [
                                'id' => $val['calendarId'],
                                'name' => $val['calendarName'],
                                'updateDate' => $calVal->update_date,
                                'boottime' => $time,
                                'oldBoottime' => $time,
                                'type' => "calendar",

                            ]
                        );
                    }
                } else {
                    $filDetail = $this->scheduleModel->getFilterDetail($calVal->calendar_id);
                    foreach ($filDetail as $val) {
                        if ($calVal->boot_time) {
                            $chgtime = str_split($calVal->boot_time, 2);
                            $time = $chgtime[0] . ":" . $chgtime[1];
                        }

                        array_push(
                            $calDetailLists,
                            [
                                'id' => $val['filterId'],
                                'name' => $val['filterName'],
                                'updateDate' => $calVal->update_date,
                                'type' => "filter",
                                'boottime' => $time,
                                'oldBoottime' => $time,

                            ]
                        );
                    }
                }
            }



            $jobnetInfo = $this->scheduleModel->getJobnet($id, $updateDate);

            foreach ($jobnetInfo as $key => $jobnetVal) {
                $jobnetDetail = $this->scheduleModel->getJobnetDetail($jobnetVal->jobnet_id);
                foreach ($jobnetDetail as $val) {
                    array_push(
                        $jobnetDetailLists,
                        [
                            'jobnetId' => $val['jobnetId'],
                            'jobnetName' => $val['jobnetName'],
                            'updateDate' => $jobnetVal->update_date,
                            'dbFlag' => "0"
                        ]
                    );
                }
            }
            $retrieveDataArr = [
                'scheduleInfo'  => $scheduleInfo,
                'calendarInfo'  => $calendarInfo,
                'jobnetInfo'  => $jobnetInfo,
                'calDetailLists' => $calDetailLists,
                'jobnetDetailLists' => $jobnetDetailLists
            ];
            return $retrieveDataArr;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
        $this->logger->info('retrieving calendar,jobnet,schedule data process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }


    /**
     * It saves the schedule data.
     *
     * @since   Method available since version 6.1.0
     */
    public function save()
    {
        $this->logger->info('Schedule save process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        // Converts it into a PHP object
        $data = Util::jsonDecode($json);

        //validation check
        $validation = $this->validator->validate($data, [
            'params' => 'required',
        ]);
        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            $objData = $data["params"];
            if ($objData["formType"] == Constants::OBJECT_FORM_CREATE || $objData["formType"] == Constants::OBJECT_FORM_NEW_OBJECT) {
                $resultMessage = $this->checkID($objData["scheduleId"]);
                $this->logger->debug('Schedule new object of calendar create process for id: ' . $objData["scheduleId"] . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                switch ($resultMessage) {
                    case Constants::SERVICE_MESSAGE_RECORD_EXIST:
                        $this->logger->debug('Schedule with same Id exists.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        echo Util::response(Constants::SERVICE_MESSAGE_RECORD_EXIST, [Constants::API_RESPONSE_MESSAGE => $resultMessage]);
                        break;
                    case Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST:
                        $this->logger->debug('Schedule create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        $result = $this->manage($objData);
                        if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objData['scheduleId'], [
                                'scheduleId' => $objData['scheduleId'],
                                'formType' => $objData['formType'],
                                'publicFlag' => $objData['publicFlag']
                            ]);
                            $this->logger->debug('Schedule information is saved.id:' . $objData["scheduleId"], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                        } else {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                            $this->logger->debug('Schedule save process failed.' . $result, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            // echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
                        }
                        break;
                }
            } else {
                $ipAddr = $this->objectLockService->getClientIpAddress();
                $checkWhere = "object_id = '" . $objData['scheduleId'] . "' AND object_type = " . Constants::OBJECT_TYPE_SCHEDULE . " AND attempt_ip = '$ipAddr'";
                $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
                if (!$checkResult) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_NO_LOCK_SESSION]);
                    return;
                }
                $result = $this->manage($objData);
                if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objData['scheduleId'], [
                        'scheduleId' => $objData['scheduleId'],
                        'formType' => $objData['formType'],
                        'publicFlag' => $objData['publicFlag']
                    ]);
                    $this->logger->debug('Schedule information is saved.id:' . $objData["scheduleId"], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                    // $this->logger->debug('Schedule information is saved.id:' . $objData["scheduleId"], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    // echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => $result]);
                } else {
                    $this->logger->debug('Schedule save process failed.' . $result, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
                }
            }
        }
    }

    /**
     * It checks the schedule id is available or not.
     *
     * @param   string $id     id of the schedule.
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {
        try {
            if ($this->scheduleModel->checkID($id)) {
                $this->logger->info('[' . $id . '] is already inputted.', ['controller' => __METHOD__]);
                return Constants::SERVICE_MESSAGE_RECORD_EXIST;
            } else {
                return Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST;
            }
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }


    /**
     * It proceeds to create/edit/new version/new object the schedule based on type.
     *
     * @param   object $scheduleData     schedule object including schedule info and operation dates.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function manage($data)
    {
        try {
            $updateDate = $this->utils->getDate();
            if ($data["formType"] == Constants::OBJECT_FORM_NEW_OBJECT || $data["formType"] == Constants::OBJECT_FORM_NEW_VERSION || $data["formType"] == Constants::OBJECT_FORM_CREATE) {
                $saveData = [
                    'id' => $data["scheduleId"],
                    'name' => $data["scheduleName"],
                    'userName' => $data["userName"],
                    'public_flag' =>  $data["publicFlag"],
                    'update_date' => $updateDate,
                    'desc' => $data["desc"],
                    'calendarInfoArr' => $data["calendarInfoArr"],
                    'jobnetInfoArr' => $data["jobnetInfoArr"]
                ];
                $this->scheduleModel->saveScheduleInfo($saveData);
                $this->logger->debug('New Schedule created. id:' . $data['scheduleId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            if ($data["formType"] == Constants::OBJECT_FORM_EDIT) {
                $urldate = date("YmdHis", strtotime($data["urlDate"]));
                $updateData = [
                    'id' => $data["scheduleId"],
                    'name' => $data["scheduleName"],
                    'userName' => $data["userName"],
                    'public_flag' =>  $data["publicFlag"],
                    'update_date' => $updateDate,
                    'urldate' => $urldate,
                    'desc' => $data["desc"],
                    'calendarInfoArr' => $data["calendarInfoArr"],
                    'jobnetInfoArr' => $data["jobnetInfoArr"]
                ];

                $this->scheduleModel->editScheduleInfo($updateData);
                $this->logger->debug('Schedule date saved.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            if ($data["formType"] == Constants::OBJECT_FORM_NEW_VERSION || $data["formType"] == Constants::OBJECT_FORM_EDIT) {

                $objectUnlockResult = $this->objectLockService->deleteLockObject((object) ["objectId" => $data["scheduleId"], "objectType" => Constants::OBJECT_TYPE_SCHEDULE]);
            }
            // $this->logger->info('Filter id:' . $data["scheduleId"] . 'updated date :' . $updateDate . ' edit init complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            // return $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $data["scheduleId"], $data);

            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }

        $this->logger->info('Schedule manage process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * The create controller action
     * 
     * It redirects to schedule_edit screen with schedule data.
     *
     * @since   Method available since version 6.1.0
     */
    public function initEdit()
    {
        $this->logger->info('Schedule edit initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        // Converts it into a PHP object
        $objData = Util::jsonDecode($json)['params'];

        $validation = $this->validator->validate($objData, [
            'objectId' => 'required',
            'date' => 'required',
            'formType' => 'required',
            'isPublic' => 'required',
        ]);
        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {

            $logStart = strpos($objData["formType"], 'version') ? "Copy & New Version schedule initialization process is started." : "Schedule Edit initialization process is started.";
            $logEnd = strpos($objData["formType"], 'version') ? "Copy & New Version schedule initialization process is finished." : "Schedule Edit initialization process is finished.";
            $this->logger->info($logStart, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

            $result = $this->initEditData($objData);

            if (!is_array($result)) {
                $this->logger->error('Schedule edit initialization process failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            } else if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result); // type = ok
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
            $this->logger->info($logEnd, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Schedule edit initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    public function itemNotFound()
    {
        require_once '../app/controllers/Pages.php';
        $pages = new Pages();
        $pages->error();
    }

    /**
     * It prepares the data for schedule_edit.
     *
     * @return  array|string could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function initEditData($objData)
    {
        try {
            $isLocked = 0;
            $id = $objData["objectId"];
            $updateDate = $objData["date"];
            $detailChk = $this->scheduleModel->each($id, $updateDate);

            if ($detailChk == false) {
                $this->logger->info('Schedule id:' . $id . ' is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND);
            }

            $retrieveDataArr = $this->retrieveData($objData); //get all info
            $detail = $retrieveDataArr['scheduleInfo'][0];
            $validFlag = $detail['validFlag'];
            $editable = 0; //disable

            //check form type
            // if ($this->objectLockService->isEditable($detailChk, Constants::OBJECT_TYPE_SCHEDULE) == Constants::SERVICE_MESSAGE_EDITABLE || strpos($objData["formType"], 'version') || strpos($objData['formType'], 'object')) {
            if ($this->objectLockService->isEditable($detailChk, Constants::OBJECT_TYPE_SCHEDULE, ($objData['formType'] == Constants::OBJECT_FORM_NEW_OBJECT || $objData['formType'] == Constants::OBJECT_FORM_NEW_VERSION)) == Constants::SERVICE_MESSAGE_EDITABLE) {
                $editable = 1;
            }

            $authority = true;
            $scheduleId = $retrieveDataArr['scheduleInfo'][0]["scheduleId"];
            $result = strcmp($objData["formType"], Constants::OBJECT_FORM_NEW_OBJECT);
            if ($result == 0) {
                $scheduleId = 'SCHEDULE_' . $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_SCHEDULE)->nextid;
            }
            $detail = $this->getScheduleDetail($id, $updateDate);
            if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                $authority = false;
            }
            $isLock = $this->objectLockService->process((object) ["objectId" => $retrieveDataArr['scheduleInfo'][0]["scheduleId"], "objectType" => Constants::OBJECT_TYPE_SCHEDULE, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
            if ($isLock != Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK &&  $objData['formType'] != Constants::OBJECT_FORM_NEW_OBJECT) { //$isLock != Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME &&
                $isLocked = 1;
            }
            $type = $objData["formType"];
            switch ($type) {
                case Constants::OBJECT_FORM_NEW_OBJECT:
                    $userName = $_SESSION['userInfo']['userName'];
                    $updateDate = null;
                    $idEditable = 1;
                    break;
                case Constants::OBJECT_FORM_EDIT:
                    $userName = $retrieveDataArr['scheduleInfo'][0]["userName"];
                    $updateDate = strpos($objData["formType"], 'version') ? "" : $retrieveDataArr['scheduleInfo'][0]["updateDate"];
                    $idEditable = 0;
                    break;
                case Constants::OBJECT_FORM_NEW_VERSION:
                    $userName = $retrieveDataArr['scheduleInfo'][0]["userName"];
                    $updateDate = null;
                    $idEditable = 0;
                    break;
            }
            $data = [
                'scheduleInfo'  => $retrieveDataArr['scheduleInfo'],
                'calendarInfo'  => $retrieveDataArr['calendarInfo'],
                'jobnetInfo'  => $retrieveDataArr['jobnetInfo'],
                'formType' => $objData["formType"],
                'calDetailLists' => $retrieveDataArr["calDetailLists"],
                'jobnetDetailLists' => $retrieveDataArr["jobnetDetailLists"],
                'validFlag'  => $validFlag,
                'editable' => $editable,
                'authority' => $authority,
                'id' => $scheduleId,
                'name' => $retrieveDataArr['scheduleInfo'][0]["scheduleName"],
                'description' => $retrieveDataArr['scheduleInfo'][0]["memo"],
                'updateDate' => $updateDate,
                'isPublic' => $objData["isPublic"],
                'userName' =>  $userName,
                'idEditable' => $idEditable,
                'isLocked' => $isLocked,
            ];
            $this->logger->info('Schedule id:' . $scheduleId . 'updated date :' . $updateDate . ' edit init complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $scheduleId, $data);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        } catch (Exception $e) {
            $this->logger->error($e->getMessage, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks the object is valid or not.
     *
     * @param   object $scheduleData     schedule object.
     * @param   string $action     type of the action. "VALID" or "DEL"
     * @return  bool|string
     * @since   Method available since version 6.1.0
     */
    public function checkValid($detail, $action)
    {
        try {
            $valid = $this->objectLockService->isValid($detail, $action, Constants::OBJECT_TYPE_SCHEDULE);
            return $valid;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
    }


    /**
     * It retrieves the detail information of schedule.
     *
     * @param   string $id     id of the schedule.
     * @param   string $date   updated date of the schedule.
     * @return  array|string could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function getScheduleDetail($id, $updateDate)
    {
        try {
            // $id = $objData["objectId"];
            // $updateDate = $objData["date"];
            $detail = $this->scheduleModel->each($id, $updateDate);
            if (!empty($detail)) {
                return $detail[0];
            } else {
                return false;
            }
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
    }

    /**
     * It redirects to schedule_edit screen with the schedule data.
     *
     * @since   Method available since version 6.1.0
     */
    // public function edit()
    // {
    //     $data = [
    //         'optiondatas'  => isset($_SESSION['optiondatas']) ? $_SESSION['optiondatas'] : "",
    //         'job_net_optiondatas'  => isset($_SESSION['job_net_optiondatas']) ? $_SESSION['job_net_optiondatas'] : "",
    //         'formType' => strpos($_GET['type'], 'version') ? Constants::OBJECT_FORM_NEW_VERSION : Constants::OBJECT_FORM_EDIT,
    //         'editable' => 1
    //     ];
    //     $this->view('screens/schedule_edit', $data);
    // }

    /**
     * It saves the edited schedule data.
     *
     * @since   Method available since version 6.1.0
     */
    public function editSchedule()
    {
        $data = json_decode($_POST['data']);
        $result = $this->scheduleService->manage($data);
        if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
            $this->responseSuccess([
                'scheduleId' => $data->scheduleId,
                'formType' => $data->formType,
                'publicFlag' => $data->publicFlag
            ]);
        } else {
            $this->responseFail($result);
        }
    }

    /**
     * It redirects to object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function version()
    {
        $this->logger->info('Schedule version search process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!isset($_GET['id'])) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "");
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        try {
            $detail = $this->scheduleModel->detail($_GET['id'], null);
            $editable = 0;
            if (sizeof($detail) <= 0) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            }
            $editable = 0;
            if ($this->objectLockService->isEditable($detail[0], Constants::OBJECT_TYPE_JOBNET, true) == Constants::SERVICE_MESSAGE_EDITABLE) {
                $editable = 1;
            }
            $data = [
                'datas'  => $detail,
                'page_type' => "schedule",
                'edit' => $editable
            ];

            if (!is_array($data)) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $data);
            }
            return $data;
        } catch (PDOException $e) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR,  $_GET['id']);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, Constants::API_RESPONSE_NOT_FOUND);
        }
        $this->logger->info('Schedule version search process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks schedule enabled missing jobnet or boottime.
     *
     * @since   Method available since version 6.1.0
     */
    public function checkschempty()
    {
        $para = json_decode($_POST['datas'], true);
        $id = $para['id'];
        $updateDate = $para['updatedate'];

        $result = $this->scheduleService->checkschempty($id, $updateDate);
        echo $result;
    }

    /**
     * It changes the valid flag of schedule version.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidVersion()
    {
        $this->logger->info('Schedule version change valid process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $request = json_decode($json);
        $validationReq = (array)$request;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'updatedDate' => 'required',
            'category' => 'required',
            'validFlag' => 'required',
            'objectId' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $result = $this->changeValidVersionService($request);
        if (!is_array($result)) {
            $data = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        } else if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
        } else {
            echo Util::response(Constants::AJAX_MESSAGE_INCOMPLETE, $result);
        }
        $this->logger->info('Schedule version change valid process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It changes the valid flag of the object_version
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function changeValidVersionService($para)
    {
        try {
            $this->beginTransaction();

            $objectId = $para->params->objectId;
            $updateDate = $para->params->updatedDate;
            $curValidFlag = $para->params->validFlag;
            $detail = $this->getScheduleDetail($objectId, $updateDate);

            if ($detail == null) {
                $this->rollback();
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                return $data;
            } else {
                if ($this->checkLatest($objectId, $updateDate, $detail) == false) {
                    $this->rollback();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    return $data;
                }
            }

            $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_SCHEDULE, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
            if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {

                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                $this->commit();
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                return $data;
            } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                return $data;
            } else {
                if ($curValidFlag == "1") {
                    $data = $this->disable($objectId);
                } else {
                    $data = $this->enable($objectId, $updateDate);
                }

                if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                    $this->rollback();
                    return $data;
                }
                //$data = $this->scheduleModel->detail($objectId, null);
                // $data = [
                //     'datas'  => $detail,
                // ];

            }
            $this->commit();
            $this->logger->info('Schedule version change valid process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
    }


    /**
     * It changes the valid flag of schedule list.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidList()
    {
        $this->logger->info('Schedule version change valid process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $request = json_decode($json, true);
        $result = $this->changeValidListService($request['params']['datas']);
        if (!is_array($result)) {
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, "");
            return $data;
        } else {
            return $result;
        }
        $this->logger->info('Schedule version change valid process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * It changes the valid flag of the schedule_list
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function changeValidListService($para)
    {
        try {
            $this->beginTransaction();
            $selectRows = $para['selectedRows'];
            $actionType = $para['actionType'];
            $modelValid = 'enable';
            if ($actionType == "disable") {
                $modelValid = 'disable';
            }
            $this->logger->info('Multiple schedule ' . $actionType . ' process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $detail = $this->getScheduleDetail($objectId, $update_date);
                if ($detail == null) {
                    $this->rollback();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                    return $data;
                } else {
                    if ($this->checkLatest($objectId, $update_date, $detail) == false) {
                        $this->rollback();
                        $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                        return $data;
                    }
                }
                if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    $this->commit();
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_SCHEDULE, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    $this->commit();
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    $this->commit();
                    return $data;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                }
            }
            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $data = $this->$modelValid($objectId, $update_date);
                if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                    $this->rollback();
                    return $data;
                }
            }
            $this->commit();
            return $data;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Multiple schedule ' . $actionType . ' process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    public function sessionclear()
    {
        unset($_SESSION['optiondatas']);
        unset($_SESSION['job_net_optiondatas']);
    }
    /**
     * It checks the object data is latest or not.
     *
     * @param   string $id     id of the schedule.
     * @param   string $date     updated date of the schedule.
     * @param   string $validstate     valid flag of the schedule.
     * @param   object $dbLatest     object of the latest schedule.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function checkLatest($id, $date, $dbLatest)
    {
        $updated = false;
        if ($id == $dbLatest["scheduleId"] && $date == $dbLatest["updateDate"]) {
            $updated = true;
        }
        return $updated;
    }

    /**
     * It enables the schedule object. It means that change the valid flag of the schedule to 1.
     *
     * @param   object $objectId     id of the schedule.
     * @param   object $updateDate     updated date of the schedule.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function enable($objectId, $updateDate)
    {

        try {
            if ($this->scheduleModel->checkSchEmpty($objectId, $updateDate) == false) {
                $this->logger->info('Schedule Enabled missing jobnet or boottime.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NO_BOOTTIME_JOBNET, $objectId);
                return $data;
            } else {
                $calendarData = $this->scheduleModel->getRelatedDataForCalendar($objectId, $updateDate);
                $filterData = $this->scheduleModel->getRelatedDataForFilter($objectId, $updateDate);
                $jobnetData = $this->scheduleModel->getRelatedDataForJobnet($objectId, $updateDate);

                $calendarDisable = [];
                $filterDisable = [];
                $jobnetDisable = [];

                foreach ($calendarData as $value) {
                    if ($this->isAllDisabled($value->calendar_id, "calendar")) {
                        array_push($calendarDisable, $value);
                    }
                }

                foreach ($filterData as $value) {
                    if ($this->isAllDisabled($value->filter_id, "filter")) {
                        array_push($filterDisable, $value);
                    }
                }

                foreach ($jobnetData as $value) {
                    if ($this->isAllDisabled($value->jobnet_id, "jobnet")) {
                        array_push($jobnetDisable, $value);
                    }
                }
                if (!empty($calendarDisable) || !empty($jobnetDisable) || !empty($filterDisable)) {
                    $returnItemData = [
                        'objectId' => $objectId,
                        'calendarData'  => $calendarDisable,
                        'filterData' => $filterDisable,
                        'jobnetData'   => $jobnetDisable
                    ];
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, (array)$returnItemData);
                    return $data;
                } else {

                    $this->scheduleModel->changeAllStatusToDisabled($objectId);
                    $this->scheduleModel->changeStatusToEnabled($objectId, $updateDate);
                    $this->logger->info('[' . $objectId . '] schedule has been enabled.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                }
            }
            return $data;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
    }

    /**
     * It disables the schedule object. It means that change the valid flag of the schedule to 0.
     *
     * @param   object $objectId     id of the schedule.
     * @param   object $updateDate     updated date of the schedule.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function disable($objectId)
    {
        try {
            if ($this->scheduleModel->changeAllStatusToDisabled($objectId) == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
            }
            return $data;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
    }

    /**
     * It checks the object is disabled or not.
     *
     * @param   object $objectId    schedule id.
     * @param   string $objetType   type of the object.
     * @return  bool|string
     * @since   Method available since version 6.1.0
     */
    private function isAllDisabled($objectId, $objetType)
    {
        try {
            $count = 0;
            switch ($objetType) {
                case 'calendar':
                    $count = $this->scheduleModel->getCalendarEnableCount($objectId);
                    break;
                case 'filter':
                    $count = $this->scheduleModel->getFilterEnableCount($objectId);
                    break;
                case 'jobnet':
                    $count = $this->scheduleModel->getJobnetEnableCount($objectId);
                    break;
            };
            return ($count == 0);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw new PDOException($e->getMessage(), $e->getCode());
        }
    }
}
