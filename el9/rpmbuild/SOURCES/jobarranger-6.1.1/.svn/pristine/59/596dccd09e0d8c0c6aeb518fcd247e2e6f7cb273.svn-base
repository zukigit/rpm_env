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
use App\Models\ObjectLockModel;
use App\Services\FilterService;
use App\Models\FilterModel;
use App\Models\CalendarModel;
use App\Services\ObjectDetailService;
use App\Models\IndexModel;
use App\Utils\Util;
use Rakit\Validation\Validator;
use PDOException, Exception;

use DateTime;

/**
 * This controller is used to manage the filter.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Filter extends Controller
{
    public function __construct()
    {
        parent::__construct();
        $this->filterModel = new FilterModel();
        $this->calendarModel = new CalendarModel();
        $this->objectLockService = new ObjectLockService();
        $this->objectDetailService = new ObjectDetailService();
        $this->utils = new Util();
        $this->logger = Core::logger();
        $this->indexModel = new IndexModel();
        $this->validator = new Validator();
        $this->objectLockModel = new ObjectLockModel();
    }

    /**
     * It redirects to object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function version()
    {
        $this->logger->info('Filter version search process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        if (!isset($_GET['id'])) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "");
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }

        try {
            $detail = $this->filterModel->detail($_GET['id'], null);
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
                'page_type' => "filter",
                'edit' => $editable
            ];
            $result =  $data;
        } catch (PDOException $e) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR,  $_GET['id']);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, Constants::API_RESPONSE_NOT_FOUND);
        }

        if (!is_array($result)) {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
            exit;
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
        }
        $this->logger->info('Filter version search process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * The create controller action
     * 
     * It redirects to filter_create screen with init data.
     *
     * @since   Method available since version 6.1.0
     */
    public function initCreate(): void
    {
        $this->logger->info('Filter Create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
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
                $calendarList = array("pubCal" => $this->filterModel->getPublicCalendarOption(), "pivCal" => $this->filterModel->getPrivateCalendarOption());
                $lastid = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_FILTER)->nextid;
                $response = [
                    'userName'  => $_SESSION['userInfo']['userName'],
                    'lastid'  => $lastid,
                    'formType' => Constants::OBJECT_FORM_CREATE,
                    'editable' => 1,
                    'calendarList'  => $calendarList,
                ];
                $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", $response);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                $this->logger->debug('Filter creation with : [' . json_encode($response) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
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
        $this->logger->info('Filter create initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirects to filter_edit screen with the filter data.
     *
     * @since   Method available since version 6.1.0
     */
    public function initEdit(): void
    {
        $this->logger->info('Filter edit initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $result = $this->edit();
        if (!is_array($result)) {
            $this->logger->error('Filter edit initialization process failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
        } else {
            if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
        $this->logger->info('Filter edit initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirects to error_page.
     *
     * @since   Method available since version 6.1.0
     */
    public function itemNotFound()
    {
        require_once '../app/controllers/Pages.php';
        $pages = new Pages();
        $pages->error();
    }

    /**
     * It prepares the data for filter_edit.
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
                    $detail = $this->filterModel->GetValidORMaxUpdateDateFilterById($params['id']);
                    $updateDate = $detail->update_date;
                } else {
                    $detail = $this->objectDetailService->getSingleObject($id, Constants::OBJECT_TYPE_FILTER, $updateDate);
                }
                if ($detail == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                    $this->logger->info('Filter id:' . $id . ' is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return $this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND);
                }
                $editable = ($this->objectLockService->isEditable($detail, Constants::OBJECT_TYPE_FILTER, ($params['type'] == Constants::OBJECT_FORM_NEW_OBJECT || $params['type'] == Constants::OBJECT_FORM_NEW_VERSION)) == Constants::SERVICE_MESSAGE_EDITABLE) ? 1 : 0;
                //check object lock editable.
                $isLock = $this->objectLockService->process((object) ["objectId" => $id, "objectType" => Constants::OBJECT_TYPE_FILTER, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock != Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK &&  $params['type'] != Constants::OBJECT_FORM_NEW_OBJECT) { //$isLock != Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME &&
                    $isLocked = 1;
                }
                $authrority = !$this->checkValid($detail, Constants::OBJ_VALID_PROC) ? false : true;
                $result = strcmp($params['type'], Constants::OBJECT_FORM_NEW_OBJECT);
                if ($result == 0) {
                    $detail->filter_id = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_FILTER)->nextid;
                }
                $calendarList = array("pubCal" => $this->filterModel->getPublicCalendarOption(), "pivCal" => $this->filterModel->getPrivateCalendarOption());
                $type = $params['type'];
                switch ($type) {
                    case Constants::OBJECT_FORM_NEW_OBJECT:
                        $formType = Constants::OBJECT_FORM_NEW_OBJECT;
                        $userName = $_SESSION['userInfo']['userName'];
                        $updateDate = null;
                        break;
                    case Constants::OBJECT_FORM_EDIT:
                        $formType = Constants::OBJECT_FORM_EDIT;
                        $userName = $detail->user_name;
                        $updateDate = $detail->update_date;
                        break;
                    case Constants::OBJECT_FORM_NEW_VERSION:
                        $formType = Constants::OBJECT_FORM_NEW_VERSION;
                        $userName = $detail->user_name;
                        $updateDate = null;
                        break;
                    case Constants::OBJECT_FORM_SCHEDULE:
                        $formType = Constants::OBJECT_FORM_SCHEDULE;
                        $userName = $detail->user_name;
                        $updateDate = $detail->update_date;
                        break;
                    default:
                        $this->logger->info('Filter id:' . $id . 'type :' . $type . ' is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        return $this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND);
                }

                $data = [
                    'filterId' => $detail->filter_id,
                    'filterName' => $detail->filter_name,
                    'userName' => $userName,
                    'publicFlag' => $detail->public_flag,
                    'updateDate' => $updateDate,
                    'desc' => $detail->memo,
                    'formType' => $formType,
                    'createdDate' => $detail->created_date,
                    'validFlag' => $detail->valid_flag,
                    'editable' => $editable,
                    'authority' => $authrority,
                    'baseDateFlag' => $detail->base_date_flag,
                    "designatedDay" => $detail->designated_day,
                    "shiftDay" => $detail->shift_day,
                    "baseCalendarId" => $detail->base_calendar_id,
                    "calendarList" => $calendarList,
                    "isLocked" => $isLocked,
                ];
                $this->logger->info('Filter id : ' . $detail->filter_id . ', updated date : ' . $detail->update_date . ' edit init complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $detail->filter_id, $data);
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
     * @param   object $filterData     filter object.
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
     * It gets the calendar data.
     *
     * @since   Method available since version 6.1.0
     */
    public function getCalendarDate()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = (object) json_decode($req_raw, true)['params'];
            if (!empty($data)) {
                $result = $this->getCalendarID($data->calendarId);
                if (!is_array($result)) {
                    if ($result == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                        $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                        $this->logger->debug('Get base calendar data fail' . $responseData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                    }
                } else {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", [
                        'calendarDate' => $result,
                    ]);
                    $this->logger->debug('base Calendar Dates retreve:' . $data->calendarId, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                }
            }
        } else {
            $data = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        }
    }

    /**
     * It saves the filter data.
     *
     * @since   Method available since version 6.1.0
     */
    public function save(): void
    {
        $this->logger->info('Calendar save process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = json_decode($req_raw, true)['params'];
            $formType = $data['formType'];
            if ($formType == Constants::OBJECT_FORM_CREATE || $formType == Constants::OBJECT_FORM_NEW_OBJECT) {
                $resultMessage = $this->checkID($data['filterId']);
                $this->logger->debug('Filter new object of Filter create process for id: ' . $data['filterId'] . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                switch ($resultMessage) {
                    case Constants::SERVICE_MESSAGE_RECORD_EXIST:
                        $this->logger->debug('Filter with same Id exists.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        $responseData = $this->utils->createResponseJson(Constants::DETAIL_REC_EXISTS, $data['filterId']);
                        echo Util::response(Constants::SERVICE_MESSAGE_RECORD_EXIST, $responseData);
                        // $this->recordExists($resultMessage);
                        break;
                    case Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST:
                        $result = $this->manage($data);
                        if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", [
                                'filterId' => $data['filterId'],
                                'formType' => $data['formType'],
                                'publicFlag' => $data['publicFlag']
                            ]);
                            $this->logger->debug('Filter information is saved.id:' . $data['filterId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                        } else {
                            $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                            $this->logger->debug('Filter save process failed.' . $responseData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                            $this->logger->debug('Filter save process failed.' . $data, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        }
                        break;
                }
            } else {
                $ipAddr = $this->objectLockService->getClientIpAddress();
                $checkWhere = "object_id = '" . $data['filterId'] . "' AND object_type = " . Constants::OBJECT_TYPE_FILTER . " AND attempt_ip = '$ipAddr'";
                $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
                if (!$checkResult) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_NO_LOCK_SESSION]);
                    return;
                }
                $result = $this->manage($data);
                if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_SUCCESS, "", [
                        'filterId' => $data['filterId'],
                        'formType' => $data['formType'],
                        'publicFlag' => $data['publicFlag']
                    ]);
                    $this->logger->debug('Filter information is saved.id:' . $data['filterId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, $responseData);
                } else {
                    $responseData = $this->utils->createResponseJson(Constants::DETAIL_FAIL);
                    $this->logger->debug('Filter save process failed.' . $responseData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $responseData);
                    // $this->responseFail($result);
                }
            }
            $this->logger->info('Filter save process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } else {
            $result = (array)$this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $result);
        }
    }

    /**
     * It changes the valid flag of filter version.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidVersion()
    {
        $this->logger->info('Filter valid change process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $para = json_decode($json);
        $validationReq = (array)$para;
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
        $result = $this->changeValidVersionService($para);
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
        $this->logger->info('Filter valid change process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    public function changeValidVersionService($para)
    {
        try {
            $this->beginTransaction();
            $objectId = $para->params->objectId;
            $updateDate = $para->params->updatedDate;
            $curValidFlag = $para->params->validFlag;

            $detail = $this->filterModel->each($objectId, $updateDate);
            if ($detail == false) {
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
            $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_FILTER, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
            if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                $this->commit();
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                $this->commit();
                return $data;
            } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR,  $objectId);
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                return $data;
            }
            if ($curValidFlag == "1") {
                $data = $this->disable($objectId, $updateDate);
            } else {
                $data = $this->enable($objectId, $updateDate);
            }
            // if ($data["AJAX_MESSAGE_TYPE"] != "AJAX_MESSAGE_SUCCESS") {
            //     $this->rollback();
            //     return $data;
            // }
            if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                $this->rollback();
                return $data;
            }
            $this->commit();
            $this->logger->info('Change valid version service is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $result = Constants::SERVICE_INTERNAL_SERVER_ERROR;
            return $result;
        }
    }
    /**
     * It disables the filter object. It means that change the valid flag of the filter to 0.
     *
     * @param   object $objectId     id of the filter.
     * @param   object $updateDate     updated date of the filter.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function disable($objectId, $updateDate)
    {
        $this->logger->info('Filter disable process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $enableSchedule = $this->filterModel->checkScheduleDisable($objectId);
            if (!empty($enableSchedule)) {
                $returnItemData = [
                    'objectId' => $objectId,
                    'scheduleData'  => $enableSchedule
                ];
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, $returnItemData);
                // $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_RELATED_DATA;
                // $data['returnItemData'] = $returnItemData;
                return $data;
            }

            if ($this->filterModel->changeStatusToDisabled($objectId, $updateDate) == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                $this->logger->debug('Filter version disable process is successful.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                $this->logger->info('Filter version disable failed for id:' . $objectId . ' updated date:' . $updateDate . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            $this->logger->error('id:' . $objectId . ' updated date:' . $updateDate . ' Message: ' . $e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
    }

    /**
     * It enables the filter object. It means that change the valid flag of the filter to 1.
     *
     * @param   object $objectId     id of the filter.
     * @param   object $updateDate     updated date of the filter.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function enable($objectId, $updateDate)
    {
        $this->logger->info('Filter enable process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            if ($this->filterModel->checkBaseCalendar($objectId, $updateDate) == 0) {
                $baseCalendar = $this->filterModel->getBaseCalendar($objectId, $updateDate);
                $returnItemData = [
                    'objectId' => $objectId,
                    'calendarData'  => $baseCalendar
                ];
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, $returnItemData);
                return $data;
            }
            $this->filterModel->changeAllStatusToDisabled($objectId);

            if ($this->filterModel->changeStatusToEnabled($objectId, $updateDate) == true) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
            }
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            $this->logger->error('id:' . $objectId . ' updated date:' . $updateDate . ' Message: ' . $e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Filter enable process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the object data is latest or not.
     *
     * @param   string $id     id of the filter.
     * @param   string $date     updated date of the filter.
     * @param   string $validstate     valid flag of the filter.
     * @param   object $dbLatest     object of the latest filter.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function checkLatest($id, $date, $dbLatest)
    {
        $updated = false;
        if ($id == $dbLatest->filter_id && $date == $dbLatest->update_date) {
            $updated = true;
        }
        return $updated;
    }


    /**
     * It changes the valid flag of filter list.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidList()
    {
        try {
            $this->beginTransaction();
            $req_raw = file_get_contents('php://input');
            $para = json_decode($req_raw, true)['params'];
            $selectRows = $para['datas']['selectedRows'];
            $actionType = $para['datas']['actionType'];
            $modelValid = 'enable';
            if ($actionType == "disable") {
                $modelValid = 'disable';
            }
            $this->logger->info('Multiple filter ' . $actionType . ' process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $detail = $this->filterModel->each($objectId, $update_date);
                if ($detail == false) {
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
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_FILTER, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
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
            if ($this->db->inTransaction()) {
                $this->rollback();
                return Constants::SERVICE_INTERNAL_SERVER_ERROR;
            }
        }
        $this->logger->info('Multiple filter ' . $actionType . ' process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It deletes the filter list from object_list screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteList()
    {
        $this->logger->info('Filter delete list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $json = file_get_contents('php://input');
            $para = json_decode($json, true);
            $this->beginTransaction();
            $selectRows = $para["params"]["datas"]['selectedRows'];

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $updateDate = $value["update"];
                $this->logger->debug('Delete filter Id:' . $objectId . ' updated date: ' . $updateDate . ' in progress.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                $objectDetail = $this->filterModel->each($objectId);
                if ($objectDetail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND, $objectId);
                    return $data;
                }
                $detail = $this->filterModel->each($objectId, $updateDate);
                if ($detail == false) {
                    $this->commit();
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    $this->logger->info('filter Id:' . $objectId . ' updated date: ' . $updateDate . 'is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return $data;
                }
                if (!$this->checkValid($detail, Constants::OBJ_DELETE_PROC)) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $objectId);

                    $this->logger->info('filter Id:' . $objectId . ' updated date: ' . $updateDate . 'does not have authority.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $this->commit();
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_FILTER, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
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
                if ($this->filterModel->checkScheduleForDelete($objectId) == true) {
                    $usingSchedule = $this->filterModel->getScheduleForDelete($objectId);
                    $returnItemData = [
                        'objectId' => $objectId,
                        'scheduleData'  => $usingSchedule
                    ];
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, (array)$returnItemData);
                    return $data;
                }

                if ($this->filterModel->deleteAllVer($objectId) == true) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                    $this->logger->info('[' . $objectId . '] filter has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                    $this->logger->info('[' . $objectId . '] filter delete process failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
            $this->commit();
            $this->logger->info('Filter delete list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $this->logger->info($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
            return $data;
        }
    }

    /**
     * It deletes the filter version from object_version screen
     *
     * @since   Method available since version 6.1.0
     */
    public function delete()
    {
        $this->logger->info('Filter delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $para = json_decode($json);
        $validationReq = (array)$para;
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
        $tmpParamData = (array)$validationReq["params"]->data;
        $validation = $this->validator->validate((array)$tmpParamData["0"], [
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
        $result = $this->deleteService($para->params);
        if (!is_array($result)) {
            $data = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        } else {
            if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
        $this->logger->info('Filter delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    public function deleteService($param)
    {
        try {
            $deleteRows = $param->data;
            $breakflag = false;
            foreach ($deleteRows as $selectRows) {
                $objectId = $selectRows->objectId;
                $detail = $this->filterModel->each($objectId, $selectRows->updateDate);
                if ($detail == false) {
                    $data = (array)$this->utils->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                    return $data;
                }
                //check if valid. Do
                // if ($detail->valid_flag == 1) {
                //     $data = (array)$this->utils->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "err-msg-del", $objectId);
                //     return $data;
                // }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_FILTER, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
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
                $totalRows = $this->filterModel->totalRows($objectId);
                if ($totalRows == count($deleteRows)) {
                    if ($this->filterModel->checkScheduleForDelete($objectId) == true) {
                        $usingSchedule = $this->filterModel->getScheduleForDelete($objectId);
                        $returnItemData = [
                            'objectId' => $objectId,
                            'scheduleData'  => $usingSchedule
                        ];
                        $data = (array)$this->utils->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, (array)$returnItemData);
                        return $data;
                    }
                }
            }
            $this->beginTransaction();
            foreach ($deleteRows as $selectRows) {
                if ($this->filterModel->deleteArr($objectId, $deleteRows) == true) {
                    $this->logger->info('[' . $objectId . '] filter has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $breakflag = true;
                    break;
                }
            }

            if ($breakflag == true) {
                $this->rollback();
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL,  $objectId);
            } else {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS,  $objectId);
                $this->commit();
            }
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                $result = Constants::SERVICE_INTERNAL_SERVER_ERROR;
                return $result;
            }
        }
    }
    /**
     * It checks the filter id is available or not.
     *
     * @param   string $id     id of the filter.
     * @since   Method available since version 6.1.0
     */
    public function checkID($id)
    {
        try {
            if ($this->filterModel->checkID($id)) {
                $this->logger->debug('[' . $id . '] is already inputted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_RECORD_EXIST;
            } else {
                return Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST;
            }
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It proceeds to create/edit/new version/new object the filter based on type.
     *
     * @param   object $filterData     filter object including filter info and operation dates.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function manage($data)
    {
        try {
            $this->beginTransaction();
            // $current_date = new DateTime();
            // $updateDate = $current_date->format("YmdHis");            
            $updateDate = $this->utils->getDate();

            if ($data['formType'] == Constants::OBJECT_FORM_NEW_OBJECT || $data['formType'] == Constants::OBJECT_FORM_NEW_VERSION || $data['formType'] == Constants::OBJECT_FORM_CREATE) {
                $saveData = [
                    'id' => $data['filterId'],
                    'name' => $data['filterName'],
                    'username' => $data['userName'],
                    'public_flag' =>  $data['publicFlag'],
                    'update_date' => $updateDate,
                    'desc' => $data['desc'],
                    'shift_day' => $data['shiftDay'],
                    'base_date_flag' => $data['baseDateFlag'],
                    'designated_day' => $data['baseDateFlag'] == 2 ? $data['designatedDay'] : '0',
                    'base_calendar_id' => $data['baseCalendarId']
                ];

                $this->filterModel->save($saveData);
                $this->logger->debug('New filter created. id:' . $data['filterId'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            if ($data['formType'] == Constants::OBJECT_FORM_EDIT) {

                $urlid = $data['filterId'];
                $urldate = date("YmdHis", strtotime($data['urlDate']));

                $updateData = [
                    'urlid' => $urlid,
                    'urldate' => $urldate,
                    'id' => $data['filterId'],
                    'name' => $data['filterName'],
                    'user_name' => $data['userName'],
                    'public_flag' => $data['publicFlag'],
                    'desc' => $data['desc'],
                    'updateDate' => $updateDate,
                    'shift_day' => $data['shiftDay'],
                    'base_date_flag' => $data['baseDateFlag'],
                    'designated_day' => $data['baseDateFlag'] == 2 ? $data['designatedDay'] : '0',
                    'base_calendar_id' => $data['baseCalendarId']
                ];

                $this->filterModel->update($updateData);
                $this->logger->debug('Filter information updated. id:' . $urlid, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            if ($data['formType'] == Constants::OBJECT_FORM_NEW_VERSION || $data['formType'] == Constants::OBJECT_FORM_EDIT) {

                $objectUnlockResult = $this->objectLockService->deleteLockObject((object) ["objectId" => $data['filterId'], "objectType" => Constants::OBJECT_TYPE_FILTER]);
            }

            $this->commit();
            $this->logger->info('Filter manage process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
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
     * It gets the operation dates of calendar id.
     *
     * @param   string $optionID     id of the calendar.
     * @return  array|string could be an array if success, could be a string if fail
     * @since   Method available since version 6.1.0
     */
    public function getCalendarID($optionID)
    {
        try {
            $baseCalendar = $this->calendarModel->GetValidORMaxUpdateDateCalendarById($optionID);
            $id = $baseCalendar->calendar_id;
            $updateDate = $baseCalendar->update_date;
            $dates = $this->filterModel->getDates($id, $updateDate);
            return $dates;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }
}
