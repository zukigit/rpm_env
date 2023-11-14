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

use App\Helpers\LocaleHelper;
use App\Utils\Controller;
use App\Utils\Core;
use App\Services\ObjectLockService;
use App\Services\ObjectDetailService;
use App\Utils\Constants;
use DateTime;
use App\Utils\Response;
use App\Models\ObjectLockModel;
use PDOException;
use App\Utils\Util;
use Rakit\Validation\Validator;

/**
 * This controller is used to manage the object lock.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ObjectLock extends Controller
{

    public function __construct()
    {
        $this->objectDetailService = new ObjectDetailService();
        $this->objectLockService = new ObjectLockService();
        $this->objectLockModel = new ObjectLockModel();
        $this->response = new Response();
        $this->utils = new Util();
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    /**
     * It checks object is editable and locked for editing.
     *
     * @since   Method available since version 6.1.0
     */
    public function lock()
    {
        $this->logger->info('Object lock process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $data = json_decode($_POST['data']);
        $result = $this->objectLockService->process($data, Constants::SERVICE_TYPE_LOCK);
        echo $result;

        $this->logger->info('Object lock process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It unlocks object after editing and cancel process.
     *
     * @since   Method available since version 6.1.0
     */
    public function unlock()
    {
        $this->logger->info('Object unlock process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');

        // Converts it into a PHP object
        $data = Util::jsonDecode($json);

        $validation = $this->validator->validate($data, [
            'params' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            $result = $this->deleteLockObject($data["params"]);

            if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                $this->logger->info('Object unlock process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_SUCCESS]);
            } else if ($result == Constants::SERVICE_MESSAGE_FAIL) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_FAIL]);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            }
        }
        $this->logger->info('Object unlock process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * check for calendar object lock before data request.
     * 
     * @since Method available since version 6.1.0
     * 
     */
    public function checkIsObjectLocked()
    {
        //lock the object if type is edit or new version
        $iserror = 0;
        if (isset($_GET['id']) && isset($_GET['date']) && isset($_GET['category'])) {
            $id = $_GET['id'];
            $category = $_GET['category'];
            $updateDate = $_GET['date'] == "null" ? null : $_GET['date'];
            //get category type.
            $categoryId = $this->getObjectType($category);
            if ($categoryId == 0) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, $id);
                echo $this->response->withArray($data);
                exit;
            }
            //get job detail;
            $detail = $this->objectDetailService->getSingleObject($id, $categoryId, $updateDate);
            if ($detail == false) {
                $data = Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND;
                $this->logger->debug('Object could not be found. id:' . $id, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                $iserror = 1;
            }
        } else {
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_FAIL, "");
            $iserror = 1;
        }
        if ($iserror == 0) {
            $isLock = $this->objectLockService->process((object) ["objectId" => $id, "objectType" => $categoryId, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_LOCK, true);
            if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_PERMIT, $id);
                $iserror = 1;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_LOCK, $id);
                $iserror = 1;
            } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR, $id);
                $iserror = 1;
            }
            if ($iserror == 0) {
                $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SUCCESS, $id);
            }
        }
        if (!is_array($data)) {
            echo $this->response->withError($this->response->getPhrase(204), 204, "");
            $this->logger->error('Object lock checking process failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } else {
            echo $this->response->withArray($data);
        }
    }

    /**
     * It deletes locked object.
     *
     * @param   object $data   data of locked object.
     * @throws  PDOException
     * @return  string fail or success message for unlock process
     * @since   Method available since version 6.1.0
     */
    private function getObjectType($objTypeString): String
    {
        $objectType = 0;
        switch ($objTypeString) {
            case Constants::OBJECT_TYPE_CALENDAR_STRING:
                $objectType = Constants::OBJECT_TYPE_CALENDAR;
                break;
            case Constants::OBJECT_TYPE_SCHEDULE_STRING:
                $objectType = Constants::OBJECT_TYPE_SCHEDULE;
                break;
            case Constants::OBJECT_TYPE_FILTER_STRING:
                $objectType = Constants::OBJECT_TYPE_FILTER;
                break;
            case Constants::OBJECT_TYPE_JOBNET_STRING:
                $objectType = Constants::OBJECT_TYPE_JOBNET;
                break;
        }
        return $objectType;
    }

    /**
     * It deletes locked object.
     *
     * @param   object $data   data of locked object.
     * @throws  PDOException
     * @return  string fail or success message for unlock process
     * @since   Method available since version 6.1.0
     */
    public function deleteLockObject($data): String
    {
        try {
            $success = false;
            if (!is_array($data)) {
                $success = $this->objectLockModel->deleteLocked([$data]);
            } else {
                $success = $this->objectLockModel->deleteLocked($data);
            }

            if ($success) {
                $this->logger->debug('Object lock delete process is successful.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_SUCCESS;
            } else {
                $this->logger->debug('Object lock delete process fail.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_FAIL;
            }
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks object is locked or not.
     *
     * @since   Method available since version 6.1.0
     */
    public function check()
    {
        $this->logger->info('Object lock check process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $data = json_decode($_POST['data']);
        $result = $this->objectLockService->process($data, Constants::SERVICE_TYPE_CHECK);
        echo $result;

        $this->logger->info('Object lock check process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves locked object lists.
     *
     * @since   Method available since version 6.1.0
     */
    function getAllLockedObj()
    {
        $this->logger->info('Get all locked object process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $arr = $this->objectLockModel->getLockedData();
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $arr]);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
        }
        $this->logger->info('Get all locked object process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that heartbeat of the object active.
     *
     * @since   Method available since version 6.1.0
     */
    public function heartbeat(): void
    {
        $this->logger->info('Heartbeat process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];
            $validator = $this->validator;
            $validation = $this->validator->validate($params, [
                'objectId' => 'required',
                'objectType' => [
                    'required',
                    $validator('in', [Constants::OBJECT_TYPE_CALENDAR_STRING, Constants::OBJECT_TYPE_FILTER_STRING, Constants::OBJECT_TYPE_SCHEDULE_STRING, Constants::OBJECT_TYPE_JOBNET_STRING])
                ],
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $objectId = $params["objectId"];
                $objectType = $this->getObjectType($params["objectType"]);
                $checkWhere = "object_id = '$objectId' AND object_type = $objectType";
                $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
                if (!$checkResult) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_NOT_FOUND]);
                    return;
                }

                $userIp = $this->objectLockService->getClientIpAddress();
                if ($checkResult->attempt_ip != $userIp) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "You are trying to change the status of object that the other user has locked."]);
                    return;
                }

                $utcTime = gmdate("Y-m-d  H:i:s");

                $data = [
                    'object_id' => $objectId,
                    'object_type' => $objectType,
                    'attempt_ip' => $userIp,
                    'last_active_time' => $utcTime
                ];

                $this->objectLockModel->updateLastActiveTime($data);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Heartbeat process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
