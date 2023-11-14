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

namespace App\Services;

use App\Utils\Service;
use App\Utils\Core;
use App\Utils\Constants;
use App\Services\ObjectDetailService;
use App\Models\ObjectLockModel;
use App\Models\CalendarModel;
use App\Models\FilterModel;
use App\Models\JobnetModel;
use App\Models\ScheduleModel;
use App\Models\UserModel;
use Datetime, DateInterval;
use PDOException;

/**
 * This service is used to manage the object lock services.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ObjectLockService extends Service
{

    public function __construct()
    {
        parent::__construct();
        $this->logger = Core::logger();
        $this->objectDetailService = new ObjectDetailService();
        $this->objectLockModel = new ObjectLockModel();
        $this->calendarModel = new CalendarModel();
        $this->filterModel = new FilterModel();
        $this->scheduleModel = new ScheduleModel();
        $this->jobnetModel = new JobnetModel();
        $this->userModel = new UserModel();
    }

    public function getClientIpAddress()
    {
        if (!empty($_SERVER["HTTP_CLIENT_IP"])) {
            $ip = $_SERVER["HTTP_CLIENT_IP"];
        } elseif (!empty($_SERVER["HTTP_X_FORWARDED_FOR"])) {
            $ip = $_SERVER["HTTP_X_FORWARDED_FOR"];
        } else {
            $ip = $_SERVER["REMOTE_ADDR"];
        }
        return $ip;
    }
    /**
     * It checks whether or not current login user got permission for the object
     *
     * @param   object $detail   detail information of object.
     * @param   string $action   type of the action. "VALID" or "DEL"
     * @param   int    $type     type of object
     * @throws  PDOException
     * @return  bool 
     * @since   Method available since version 6.1.0
     */
    public function isValid($detail, $action, $type = null)
    {
        $this->logger->debug('Check editable service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            if (Constants::OBJECT_TYPE_SCHEDULE == $type) {
                $username = $detail['userName'];
            } else {
                $username = $detail->user_name;
            }

            $objectUserGroupList = $this->userModel->getGroupIDListByAlias($username);
            $userGroupList = $_SESSION['userInfo']['groupList'];
            $objectOwnType = Constants::OBJECT_USER_TYPE_OTHER;
            if ($this->userModel->isExistGroupId($userGroupList, $objectUserGroupList)) {
                $objectOwnType = Constants::OBJECT_USER_TYPE_OWN;
            }
            $valid = true;
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_GENERAL) {
                $valid = false;
            } else if ($_SESSION['userInfo']['userType'] != Constants::USER_TYPE_SUPER && $objectOwnType == Constants::OBJECT_USER_TYPE_OTHER && $action == Constants::OBJ_VALID_PROC) {
                $valid = false;
            } else if ($_SESSION['userInfo']['userType'] != Constants::USER_TYPE_ADMIN && $_SESSION['userInfo']['userType'] != Constants::USER_TYPE_SUPER && $objectOwnType == Constants::OBJECT_USER_TYPE_OTHER && $action == Constants::OBJ_DELETE_PROC) {
                $valid = false;
            }
            return $valid;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks object is editable or not.
     *
     * @param   object $detail  detail information of object.
     * @param   int    $type    type of object
     * @param   bool   $version
     * @throws  PDOException
     * @return  string $editable
     * @since   Method available since version 6.1.0
     */
    public function isEditable($detail, $type, $version = false): String
    {
        $this->logger->debug('Check editable service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            if (Constants::OBJECT_TYPE_SCHEDULE == $type) {
                $detail = $detail[0];
                $validFlag = $detail['validFlag'];
                $objectUserGroupList = $this->userModel->getGroupIDListByAlias($detail['userName']);
            } else {
                $validFlag = $detail->valid_flag;
                $objectUserGroupList = $this->userModel->getGroupIDListByAlias($detail->user_name);
            }

            $userGroupList = $_SESSION['userInfo']['groupList'];
            $editable =  Constants::SERVICE_MESSAGE_UNEDITABLE;

            $objectOwnType = Constants::OBJECT_USER_TYPE_OTHER;
            if ($this->userModel->isExistGroupId($userGroupList, $objectUserGroupList)) {
                $objectOwnType = Constants::OBJECT_USER_TYPE_OWN;
            }
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || ($objectOwnType == Constants::OBJECT_USER_TYPE_OWN && $_SESSION['userInfo']['userType'] == Constants::USER_TYPE_ADMIN)) {
                if ($version) {
                    $editable = Constants::SERVICE_MESSAGE_EDITABLE;
                } else {
                    if ($validFlag == 0) {
                        $editable = Constants::SERVICE_MESSAGE_EDITABLE;
                    }
                }
            }
            return $editable;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks object is editable or not for editing.
     *
     * @param   object $data  object info.
     * @param   string $type  
     * @throws  PDOException
     * @return  string|array  could be array if editable, could be string if uneditable
     * @since   Method available since version 6.1.0
     */
    public function process($data, $type, $version = false): String
    {
        $this->logger->debug($type . ' service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $detail = $this->objectDetailService->getSingleObject($data->objectId, $data->objectType, null);
        if ($detail == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
        $isEditable = $this->isEditable($detail, $data->objectType, $version);
        if ($isEditable != Constants::SERVICE_MESSAGE_EDITABLE) {
            $this->logger->debug('Object is not editable.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $isEditable;
        }

        $this->logger->debug('Object is editable.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $checkWhere = "object_id = '$data->objectId' AND object_type = $data->objectType";
            $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
            if ($checkResult) {
                if ($checkResult->username == $_SESSION['userInfo']['userName'] && $checkResult->attempt_ip == $data->attemptIp) {
                    //Lock user is same
                    $this->logger->info(Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME;
                } else {
                    $this->logger->info(Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS;
                }
                // $this->logger->info(Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                // return Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS;
            } else {
                //Object is not locked.
                if ($type == Constants::SERVICE_TYPE_LOCK) {
                    return $this->lockObject($data);
                } else {
                    $this->logger->info(Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    return Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK;
                }
            }
        } catch (PDOException $e) {
            $this->logger->info(Constants::SERVICE_INTERNAL_SERVER_ERROR, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }



    /**
     * It locks for editing object.
     *
     * @param   object $data   data of object to lock.
     * @throws  PDOException
     * @return  string fail or success message for lock process
     * @since   Method available since version 6.1.0
     */
    public function lockObject($data): String
    {
        $this->logger->debug('Lock object service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $utcTime = gmdate("Y-m-d  H:i:s");
        $objectData = [
            'object_id' => $data->objectId,
            'object_type' =>  $data->objectType,
            'username' => $_SESSION['userInfo']['userName'],
            'attempt_ip' => $data->attemptIp,
            'created_date' => $utcTime,
            'last_active_time' => $utcTime
        ];

        try {
            if ($this->objectLockModel->insertObjectLock($objectData)) {
                return Constants::SERVICE_MESSAGE_SUCCESS;
            } else {
                return Constants::SERVICE_MESSAGE_FAIL;
            }
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It retrieves locked object lists.
     *
     * @throws  PDOException
     * @return  array|string could be array if success, could be string if fail
     * @since   Method available since version 6.1.0
     */
    public function init()
    {
        $this->logger->debug('Init service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $data = $this->objectLockModel->getData();
            if ($data) {
                return $data;
            } else {
                $this->logger->error('Object init data is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_FAIL;
            }
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It retrieves all locked object lists on search data.
     *
     * @param   array $para
     * @throws  PDOException
     * @return  array|string could be array if success, could be string if fail
     * @since   Method available since version 6.1.0
     */
    public function lazySearch($para)
    {
        $this->logger->debug('Object lock lazy search service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $sort = $para['sort'];
        $limit = $para['limit'];
        $searchArr = $para['whereArr'];

        $search = "";
        if ($searchArr[0] !== "") {
            $search = $search . "username LIKE '%" . $searchArr[0] . "%'" . " and";
        }
        if ($searchArr[1] !== "") {
            $search = $search  . " object_id LIKE '%" . $searchArr[1] . "%'" . " and";
        }
        if ($searchArr[2] !== "") {
            $search = $search . " object_type LIKE " . $searchArr[2] . " and";
        }

        try {
            return $this->objectLockModel->searchLockData($sort, $limit, $search);
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
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
    public function deleteLockObject($data): String
    {
        $this->logger->debug('Delete lock service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $deleteWhere = "object_id = '$data->objectId' AND object_type = $data->objectType";
            if ($this->objectLockModel->deleteObjectLock($deleteWhere)) {
                return Constants::SERVICE_MESSAGE_SUCCESS;
            } else {
                return Constants::SERVICE_MESSAGE_FAIL;
                $this->logger->debug('Object lock delete failed. id:' . $data->objectId, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }
}
