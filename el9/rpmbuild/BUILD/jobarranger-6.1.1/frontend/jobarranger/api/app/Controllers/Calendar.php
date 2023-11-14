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
use App\Utils\Util;
use App\Utils\Core;
use App\Utils\Constants;
use App\Models\CalendarModel;
use App\Services\ObjectLockService;
use App\Services\ObjectDetailService;
use App\Models\IndexModel;
use App\Models\ObjectLockModel;
//use App\Utils\ResponseJson;
use Rakit\Validation\Validator;
use PDOException, Exception;

/**
 * This controller is used to manage the calendar.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Calendar extends Controller
{
    public function __construct()
    {
        parent::__construct();
        $this->logger = Core::logger();
        $this->objectDetailService = new ObjectDetailService();
        $this->objectLockService = new ObjectLockService();
        $this->calendarModel = new CalendarModel();
        $this->indexModel = new IndexModel();
        $this->utils = new Util();
        $this->validator = new Validator();
        $this->objectLockModel = new ObjectLockModel();
        //$this->responseJson = new ResponseJson;
    }

    /**
     * It redirects to calendar_create screen with init data.
     *
     * @since   Method available since version 6.1.0
     */
    function initCreate(): void
    {
        $this->logger->info('Calendar create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $authority = false;
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $_SESSION['userInfo']['userType'] == Constants::USER_TYPE_ADMIN) {
                $authority = true;
                $json = file_get_contents('php://input');
                $params = Util::jsonDecode($json)["params"];
                $validation = $this->validator->validate($params, [
                    'type' => 'required'
                ]);
                if ($validation->fails()) {
                    $errors = $validation->errors();
                    echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                    $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
                } else {
                    $lastid = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_CALENDAR)->nextid;
                    $response = [
                        'name'  => $_SESSION['userInfo']['userName'],
                        'lastid'  => $lastid,
                        'formType' => Constants::OBJECT_FORM_CREATE,
                        'editable' => 1,
                    ];
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", $response);
                    $this->logger->debug('Calendar creation with : [' . json_encode($response) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                }
            } else {
                $response = ['authority' => $authority];
                $responseData = $this->utils->createResponseJson(Constants::DETAIL_PERMIT, "", $response);
                $this->logger->debug('Calendar creation with : [' . json_encode($response) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
            }
        } catch (PDOException $e) {
            $result = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
        } catch (Exception $e) {
            $result = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
        }
        $this->logger->info('Calendar create initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It saves the calendar data.
     *
     * @since   Method available since version 6.1.0
     */
    ////
    public function save(): void
    {
        $this->logger->info('Calendar save process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = json_decode($req_raw, true)['params'];
            $formType = $data['formType'];
            if ($formType == Constants::OBJECT_FORM_CREATE || $formType == Constants::OBJECT_FORM_NEW_OBJECT) {
                $resultMessage = $this->checkID($data['calendarId']);
                $this->logger->debug('Calendar new object of calendar create process for id: ' . $data['calendarId'] . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                switch ($resultMessage) {
                    case Constants::SERVICE_MESSAGE_RECORD_EXIST:
                        $this->logger->debug('Calendar with same Id exists.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        $responseData = $this->utils->createResponseJson(Constants::DETAIL_REC_EXISTS, $data['calendarId']);
                        echo Util::response(Constants::SERVICE_MESSAGE_RECORD_EXIST, $responseData);
                        break;
                    case Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST:
                        $this->logger->debug('Calendar create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        $result = $this->manage($data);
                        if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", [
                                'calendarId' => $data['calendarId'],
                                'formType' => $data['formType'],
                                'publicFlag' => $data['publicFlag']
                            ]);
                            $this->logger->debug('Calendar information is saved.id:' . $data['calendarId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                        } else {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                            $this->logger->debug('Calendar save process failed.' . $responseData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                        }
                        break;
                }
            } else {
                //check for latest update/enable-disable/lock
                $detail = $this->objectDetailService->getSingleObject($data['calendarId'], Constants::OBJECT_TYPE_CALENDAR, $data['urlDate']);
                if ($detail == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                    $result = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $data['calendarId']);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
                    $this->logger->debug('Calendar Item could not be found. id:' . $data['calendarId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    exit;
                }
                if ($formType == Constants::OBJECT_FORM_EDIT) {
                    if ($this->checkLatest($data['calendarId'], $data['urlDate'], $detail) == false) {
                        $result = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $data['calendarId']);
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
                        $this->logger->debug('Current calender updated date is not latest. id:' . $data['calendarId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        exit;
                    }
                    // $isLock = $this->objectLockService->process((object) ["objectId" => $data['calendarId'], "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                    // if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    //     $result = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $data['calendarId']);
                    //     echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
                    //     exit;
                    // } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    //     $result = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $data['calendarId']);
                    //     echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
                    //     exit;
                    // }
                    $ipAddr = $this->objectLockService->getClientIpAddress();
                    $checkWhere = "object_id = '" . $data['calendarId'] . "' AND object_type = " . Constants::OBJECT_TYPE_CALENDAR . " AND attempt_ip = '$ipAddr'";
                    $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
                    if (!$checkResult) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_NO_LOCK_SESSION]);
                        return;
                    }
                }
                if (!$this->checkValid($detail, Constants::OBJ_DELETE_PROC)) {
                    $result = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $data['calendarId']);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
                    exit;
                }
                $result = $this->manage($data);
                if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", [
                        'calendarId' => $data['calendarId'],
                        'formType' => $data['formType'],
                        'publicFlag' => $data['publicFlag']
                    ]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                } else {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                }
            }
            $this->logger->info('Calendar save process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } else {
            $result = (array)$this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $result);
        }
    }

    /**
     * It redirects to object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function version()
    {
        $this->logger->info('Calendar version search process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!isset($_GET['id'])) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "");
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        try {
            $detail = $this->calendarModel->detail($_GET['id'], null);
            $editable = 0;
            if (sizeof($detail) <= 0) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            }
            if ($this->objectLockService->isEditable($detail[0], Constants::OBJECT_TYPE_JOBNET, true) == Constants::SERVICE_MESSAGE_EDITABLE) {
                $editable = 1;
            }
            $data = [
                'datas'  => $detail,
                'page_type' => "calendar",
                'edit' => $editable
            ];
            if (!is_array($data)) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $data);
            }
        } catch (PDOException $e) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR,  $_GET['id']);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, Constants::API_RESPONSE_NOT_FOUND);
        }
        $this->logger->info('Calendar version search process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirects to calendar_edit screen with the calendar data.
     *
     * @since   Method available since version 6.1.0
     */
    public function initEdit(): void
    {
        $this->logger->info('Calendar edit initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = $this->edit();
        if (!is_array($result)) {
            //echo $this->response->withError($this->response->getPhrase(204), 204, "");
            $this->logger->error('Calendar edit initialization process failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
        } else {
            //echo $this->response->withArray($result);
            if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
        $this->logger->info('Calendar edit initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It changes the valid flag of calendar version.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidVersion()
    {
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
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        //$request = json_decode($_POST['datas'], true);
        $result = $this->changeValidVersionService($request);
        if (!is_array($result)) {
            $data = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        } else {
            if ($result["message-detail"] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
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
        $this->logger->info('Calendar version change valid process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->beginTransaction();
            $objectId = $para->params->objectId;
            $updateDate = $para->params->updatedDate;
            $curValidFlag = $para->params->validFlag;
            $detail = $this->calendarModel->each($objectId, $updateDate);

            if ($detail == false) {
                $this->rollback();
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                return $data;
            } else {
                if ($this->checkLatest($objectId, $updateDate, $detail) == false) {
                    $this->rollback();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    return $data;
                }
            }
            $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK);
            if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
            } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                return $data;
            } else {
                if ($curValidFlag == "1") {
                    $data = $this->disable($objectId, $updateDate);
                } else {
                    $data = $this->enable($objectId, $updateDate);
                }
                if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                    $this->rollback();
                    return $data;
                }
            }
            $this->commit();
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Calendar version change valid process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * It changes the valid flag of the calendar_list
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
            $this->logger->info('Multiple calendar ' . $actionType . ' process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];

                $detail = $this->calendarModel->each($objectId, $update_date);
                if ($detail == false) {
                    $this->rollback();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
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
                    // $this->commit();
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    // $this->commit();
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    // $this->commit();
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
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Multiple calendar ' . $actionType . ' process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * It changes the valid flag of calendar list.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidList()
    {
        $req_raw = file_get_contents('php://input');
        $para = json_decode($req_raw, true)['params'];
        $result = $this->changeValidListService($para['datas']);
        if (!is_array($result)) {
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            return $data;
        } else {
            return $result;
        }
    }

    /**
     * It deletes the calendar list from object_list screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteList()
    {
        $this->logger->info('Multiple calendar delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $para = json_decode($json, true);
        $request = $para["params"]["datas"];
        $result = $this->deleteListService($request);
        if (!is_array($result)) {
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            return $data;
        } else {
            $this->logger->info('Multiple calendar version delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $result;
        }
    }

    /**
     * It deletes the calendar version from object_version screen
     *
     * @since   Method available since version 6.1.0
     */
    public function delete()
    {
        $this->logger->info('Calendar delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        if (!isset($json)) {
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $para = json_decode($json);
        $validationReq = (array)$para;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'data' => 'required',
            'category' => 'required',
        ]);
        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }

        $para = json_decode($json, true);
        $validationReq = (array)$para;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'data' => 'required',
            'category' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }

        $validation = $this->validator->validate((array)$validationReq["params"]["data"]['0'], [
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


        $result = $this->deleteService($para["params"]);
        if (!is_array($result)) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
        } else {
            //echo $this->response->withArray($result);
            if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SERVER_ERROR) {
                echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
        $this->logger->info('Calendar delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    public function deleteService($param)
    {
        try {
            $deleteRows = $param['data'];
            $breakflag = false;
            foreach ($deleteRows as $selectRows) {
                $objectId = $selectRows['objectId'];
                $detail = $this->calendarModel->each($objectId, $selectRows['updateDate']);
                if ($detail == false) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
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

                //check if valid. Do
                if ($detail->valid_flag == 1) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    return $data;
                }
                $totalRows = $this->calendarModel->totalRows($objectId);
                if ($totalRows == count($deleteRows)) {
                    if ($this->calendarModel->checkCalendarAndFilterForDelete($objectId) == true) {
                        $usingSchedule = $this->calendarModel->checkScheduleForDelete($objectId);
                        $usingFilter =  $this->calendarModel->checkFilterForDelete($objectId);
                        $returnItemData = [
                            'objectId' => $objectId,
                            'scheduleData'  => $usingSchedule,
                            'filterData'   => $usingFilter
                        ];
                        $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, $returnItemData);
                        return $data;
                    }
                }
            }
            $this->beginTransaction();
            foreach ($deleteRows as $selectRows) {
                $objectId = $selectRows['objectId'];
                if ($this->calendarModel->deleteArr($objectId, $deleteRows) == true) {
                    // $detail = $this->calendarModel->detail($objectId, null);
                    // $data = [
                    //     'datas'  => $detail,
                    // ];
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                    $this->logger->info('[' . $objectId . '] calendar has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $breakflag = true;
                    break;
                }
            }

            if ($breakflag == true) {
                $this->rollback();
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
            } else {
                $this->commit();
            }
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
            return $data;
        }
    }
    /**
     * It proceeds to create/edit/new version/new object the calendar based on type.
     *
     * @param   object $calendarData     calendar object including calendar info and operation dates.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function manage($data)
    {
        try {
            $this->beginTransaction();
            $updateDate = $this->utils->getDate();
            if ($data['formType'] == Constants::OBJECT_FORM_NEW_OBJECT || $data['formType'] == Constants::OBJECT_FORM_NEW_VERSION || $data['formType'] == Constants::OBJECT_FORM_CREATE) {
                $saveData = [
                    'id' => $data['calendarId'],
                    'name' => $data['calendarName'],
                    'username' => $data['userName'],
                    'public_flag' =>  $data['publicFlag'],
                    'update_date' => $updateDate,
                    'desc' => $data['desc'],
                ];
                $this->calendarModel->save($saveData);
                $this->logger->debug('New Calendar created. id:' . $data['calendarId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
            if ($data['formType'] == Constants::OBJECT_FORM_EDIT) {
                $urlid = $data['urlId'];
                $urldate = date("YmdHis", strtotime($data['urlDate']));
                $this->calendarModel->deleteDates($urlid, $urldate);
                $updateData = [
                    'urlid' => $urlid,
                    'urldate' => $urldate,
                    'id' => $data['calendarId'],
                    'name' => $data['calendarName'],
                    'public_flag' => $data['publicFlag'],
                    'update_date' => $updateDate,
                    'desc' => $data['desc'],
                ];
                $this->calendarModel->update($updateData);
                $this->logger->debug('Calendar information updated. id:' . $urlid, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
            if (!empty($data['dates'])) {
                $dateStringArray = $data['dates'];
                foreach ($dateStringArray as $dateString) {
                    $date = [
                        'id' => $data['calendarId'],
                        'update_date' => $updateDate,
                        'operating_date' => $dateString,
                    ];
                    $this->calendarModel->saveDate($date);
                    $this->logger->debug('Calendar date saved.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
            // if ($data['formType'] == Constants::OBJECT_FORM_NEW_VERSION || $data['formType'] == Constants::OBJECT_FORM_EDIT) {
            //     $objectUnlockResult = $this->objectLockService->deleteLockObject((object) ["objectId" => $data['calendarId'], "objectType" => Constants::OBJECT_TYPE_CALENDAR]);
            // }
            $this->commit();
            $this->logger->info('Calendar manage process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
    }

    /**
     * It deletes the calendar from object_list
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function deleteListService($para)
    {
        $this->logger->info('Multiple calendar delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->beginTransaction();
            $selectRows = $para['selectedRows'];

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $updatedate = $value["update"];
                $objectDetail = $this->calendarModel->each($objectId);
                if ($objectDetail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND, $objectId);
                    return $data;
                }
                $detail = $this->calendarModel->each($objectId, $updatedate);
                if ($detail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    return $data;
                }
                if (!$this->checkValid($detail, Constants::OBJ_DELETE_PROC)) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    $this->commit();
                    return $data;
                }
                //disabled to wait for user
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
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
                //check if valid. Do
                if ($detail->valid_flag == 1) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    $this->commit();
                    return $data;
                }
            }
            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                if ($this->calendarModel->checkCalendarAndFilterForDelete($objectId) == true) {
                    $usingSchedule = $this->calendarModel->checkScheduleForDelete($objectId);
                    $usingFilter =  $this->calendarModel->checkFilterForDelete($objectId);
                    $returnItemData = [
                        'objectId' => $objectId,
                        'scheduleData'  => $usingSchedule,
                        'filterData'   => $usingFilter
                    ];
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, $returnItemData);
                    $this->rollback();
                    return $data;
                }

                if ($this->calendarModel->deleteAll($objectId) == true) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                    $this->logger->info('[' . $objectId . '] calendar has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                }
            }
            $this->commit();
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Multiple calendar delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the calendar id is available or not.
     *
     * @param   string $id     id of the calendar.
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {

        try {
            if ($this->calendarModel->checkID($id)) {
                return Constants::SERVICE_MESSAGE_RECORD_EXIST;
            } else {
                return Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST;
            }
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It prepares the data for calendar_edit.
     *
     * @return  array|string could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function edit()
    {
        try {
            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];
            $validator = $this->validator;
            $validation = $this->validator->validate($params, [
                'type' => [
                    'required',
                    $validator('in', [Constants::OBJECT_FORM_CREATE, Constants::OBJECT_FORM_EDIT, Constants::OBJECT_FORM_NEW_OBJECT, Constants::OBJECT_FORM_NEW_VERSION, Constants::OBJECT_TYPE_SCHEDULE_STRING])
                ],
                'id' => 'required',
                'date' => 'required',
            ]);
            if ($validation->fails()) {
                $errors = $validation->errors();
                return $errors;
                $this->logger->info('Calendar id or type is missing from the request.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $this->utils->createResponseJson(Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND);
            } else {
                $id = $params['id'];
                $updateDate = $params['date'];

                $isLocked = 0;
                $detail = null;
                if ($params['type'] == Constants::OBJECT_TYPE_SCHEDULE_STRING) {
                    $detail = $this->calendarModel->GetValidORMaxUpdateDateCalendarById($params['id']);
                    $updateDate = $detail->update_date;
                } else {
                    $detail = $this->objectDetailService->getSingleObject($id, Constants::OBJECT_TYPE_CALENDAR, $updateDate);
                }

                if ($detail == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                    $this->logger->info('Calendar id:' . $id . ' is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return $this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND);
                }
                $editable = 0;
                if ($this->objectLockService->isEditable($detail, Constants::OBJECT_TYPE_CALENDAR, ($params['type'] == Constants::OBJECT_FORM_NEW_OBJECT || $params['type'] == Constants::OBJECT_FORM_NEW_VERSION)) == Constants::SERVICE_MESSAGE_EDITABLE) {
                    $editable = 1;
                }
                //check object lock editable.
                $isLock = $this->objectLockService->process((object) ["objectId" => $id, "objectType" => Constants::OBJECT_TYPE_CALENDAR, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock != Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK &&  $params['type'] != Constants::OBJECT_FORM_NEW_OBJECT) { //$isLock != Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME &&
                    $isLocked = 1;
                }
                $authrority = true;
                //$detail = $this->calendarModel->each($id, $updateDate);
                if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                    $authrority = false;
                }
                $dates = $this->calendarModel->getDates($id, $updateDate);
                $lastday = $this->calendarModel->getLastDay($id, $updateDate);
                $dateArray = array_map(function ($val) {
                    return $val->operating_date;
                }, $dates);
                $result = strcmp($params['type'], Constants::OBJECT_FORM_NEW_OBJECT);
                if ($result == 0) {
                    $detail->calendar_id =  $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_CALENDAR)->nextid;
                }
                $type = $params['type'];
                $tmp_updated_date = $detail->update_date;
                switch ($type) {
                    case Constants::OBJECT_FORM_NEW_OBJECT:
                        $formType = Constants::OBJECT_FORM_NEW_OBJECT;
                        $userName = $_SESSION['userInfo']['userName'];
                        $tmp_updated_date = null;
                        break;
                    case Constants::OBJECT_FORM_EDIT:
                        $formType = Constants::OBJECT_FORM_EDIT;
                        $userName = $detail->user_name;
                        break;
                    case Constants::OBJECT_FORM_NEW_VERSION:
                        $formType = Constants::OBJECT_FORM_NEW_VERSION;
                        $userName = $detail->user_name;
                        $tmp_updated_date = null;
                        break;
                    case Constants::OBJECT_FORM_SCHEDULE:
                        $formType = Constants::OBJECT_FORM_SCHEDULE;
                        $userName = $detail->user_name;
                        break;
                    default:
                        $this->logger->info('Calendar id:' . $id . 'type :' . $type . ' is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        return $this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND);
                }
                $data = [
                    'calendarId' => $detail->calendar_id,
                    'calendarName' => $detail->calendar_name,
                    'userName' => $userName,
                    'publicFlag' => $detail->public_flag,
                    'updateDate' => $tmp_updated_date,
                    'desc' => $detail->memo,
                    'formType' => $formType,
                    'dates' => $dateArray,
                    'createdDate' => $detail->created_date,
                    'validFlag' => $detail->valid_flag,
                    'lastday' => $lastday->max,
                    'editable' => $editable,
                    'authority' => $authrority,
                    'notInitialize' => false,
                    'isLocked' => $isLocked
                ];
                $this->logger->info('Calendar id : ' . $detail->calendar_id . ', updated date : ' . $detail->update_date . ' edit init complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $detail->calendar_id, $data);
            }
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        } catch (Exception $e) {
            $this->logger->error($e->getMessage, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks the object is valid or not.
     *
     * @param   object $calendarData     calendar object.
     * @param   string $action     type of the action. "VALID" or "DEL"
     * @return  bool|string
     * @since   Method available since version 6.1.0
     */
    public function checkValid($detail, $action)
    {
        try {
            $valid = $this->objectLockService->isValid($detail, $action);
            return $valid;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }
    /**
     * It retrieves the operation date of calendar.
     *
     * @param   string $id     id of the calendar.
     * @param   string $updateDate     updated date of the calendar.
     * @return  array|string could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function getOperationDate($id, $updateDate)
    {
        try {
            $date = $this->calendarModel->getDates($id, $updateDate);

            return $date;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks the object data is latest or not.
     *
     * @param   string $id     id of the calendar.
     * @param   string $date     updated date of the calendar.
     * @param   string $validstate     valid flag of the calendar.
     * @param   object $dbLatest     object of the latest calendar.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function checkLatest($id, $date, $dbLatest)
    {
        $updated = false;
        if ($id == $dbLatest->calendar_id && $date == $dbLatest->update_date) {
            $updated = true;
        }
        return $updated;
    }

    /**
     * It enables the calendar object. It means that change the valid flag of the calendar to 1.
     *
     * @param   object $objectId     id of the calendar.
     * @param   object $updateDate     updated date of the calendar.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function enable($objectId, $updateDate)
    {
        $this->logger->info('Calendar version enable process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->calendarModel->changeAllStatusToDisabled($objectId);
            if ($this->calendarModel->changeStatusToEnabled($objectId, $updateDate) == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                $this->logger->debug('Calendar version enable process is successful.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                $this->logger->info('Calendar version enable failed for id:' . $objectId . ' updated date:' . $updateDate . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            return $data;
        } catch (PDOException $e) {
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
            $this->logger->error('id:' . $objectId . ' updated date:' . $updateDate . ' Message: ' . $e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
    }

    /**
     * It disables the calendar object. It means that change the valid flag of the calendar to 0.
     *
     * @param   object $objectId     id of the calendar.
     * @param   object $updateDate     updated date of the calendar.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function disable($objectId, $updateDate)
    {
        try {
            $enableFilter   = $this->calendarModel->checkFilterDisable($objectId);
            $enableSchedule = $this->calendarModel->checkScheduleDisable($objectId);
            if (!empty($enableFilter) || !empty($enableSchedule)) {
                $returnItemData = [
                    'objectId' => $objectId,
                    'filterData'  => $enableFilter,
                    'scheduleData'  => $enableSchedule
                ];
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, (array)$returnItemData);
                return $data;
            }

            if ($this->calendarModel->changeStatusToDisabled($objectId, $updateDate) == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
            }
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * api endpoint that retrieves calender list
     *
     * @since   Method available since version 6.1.0
     */
    public function getCalendarList()
    {
        $this->logger->info('Get calendar list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $calendarList = $this->calendarModel->getInfoByUserIdSuper();
            } else {
                $calendarList = $this->calendarModel->getInfoByUserId($_SESSION['userInfo']['userId']);
            }

            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $calendarList]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Get calendar list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves calender list
     *
     * @since   Method available since version 6.1.0
     */
    public function getValidOrLatest()
    {
        $this->logger->info('Get valid or latest object process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'id' => 'required',
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {

                $calendarList = $this->calendarModel->getValidORMaxUpdateDateEntityById($params["id"]);

                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $calendarList]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Get valid or latest object process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
