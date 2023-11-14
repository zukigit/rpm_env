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
// use App\Services\GeneralSettingService;
use App\Models\GeneralSettingModel;
use Rakit\Validation\Validator;
use App\Utils\Util;
use PDOException;

/**
 * This controller is used to manage the general setting.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class GeneralSetting extends Controller
{

    public function __construct()
    {
        parent::__construct();
        // $this->generalSettingService = new GeneralSettingService();
        $this->generalSettingModel = new GeneralSettingModel();
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    /**
     * It return init general setting data.
     *
     * @since   Method available since version 6.1.0
     */
    function getAll()
    {
        $this->logger->info('General Setting screen initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $retrieveData = (object) $this->getGeneralSettingParamValue();
        $retrieveData->disabledEdit = $this->disabledEdit();
        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::AJAX_MESSAGE_DATA => $retrieveData]);
        $this->logger->info('General Setting screen initialization process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It saves the updated general setting data.
     *
     * @since   Method available since version 6.1.0
     */
    public function update()
    {
        $this->logger->info('Update function is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $updateData = Util::jsonDecode($json);
        if ($updateData == false) {
            $result = (array)$this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $result);
            exit;
        }
        $validation = $this->validator->validate($updateData['params']['value'], [
            'jobnetViewSpan' => 'required',
            'jobnetLoadSpan' => 'required',
            'jobnetKeepSpan' => 'required',
            'joblogKeepSpan' => 'required',
            'standardTime' => 'required',
            'notification' => 'required',
            'zabbixServerIPaddress' => 'required',
            'zabbixServerPortNumber' => 'required',
            'zabbixSenderCommand' => 'required',
            'messageDestinationServer' => 'required',
            'messageDestinationItemKey' => 'required',
            'retry' => 'required',
            'retryCount' => 'required',
            'retryInterval' => 'required',
            'disabledEdit' => 'required'
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        } else {
            $finalData = $this->updateData($updateData);
            if ($finalData) {
                if ($finalData == Constants::AJAX_MESSAGE_SUCCESS || $finalData == Constants::SERVICE_MESSAGE_SUCCESS) {
                    $finalData = (object) $this->getGeneralSettingParamValue();
                    $finalData->disabledEdit = $this->disabledEdit();
                    $data = [
                        'jobnetViewSpan' => $finalData->jobnetViewSpan,
                        'jobnetLoadSpan' => $finalData->jobnetLoadSpan,
                        'jobnetKeepSpan' => $finalData->jobnetKeepSpan,
                        'joblogKeepSpan' => $finalData->joblogKeepSpan,
                        'standardTime' => $finalData->standardTime,
                        'notification' => $finalData->notification,
                        'zabbixServerIPaddress' => $finalData->zabbixServerIPaddress,
                        'zabbixServerPortNumber' => $finalData->zabbixServerPortNumber,
                        'zabbixSenderCommand' => $finalData->zabbixSenderCommand,
                        'messageDestinationServer' => $finalData->messageDestinationServer,
                        'messageDestinationItemKey' => $finalData->messageDestinationItemKey,
                        'retry' => $finalData->retry,
                        'retryCount' => $finalData->retryCount,
                        'retryInterval' => $finalData->retryInterval,
                        'heartbeatIntervalTime' =>  $finalData->heartbeatIntervalTime,
                        'objectLockExpiredTime' => $finalData->objectLockExpiredTime,
                        'disabledEdit' => $finalData->disabledEdit
                    ];
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::AJAX_MESSAGE_DATA => $data]);
                } else {
                    $this->logger->debug('General Setting update process failed.' . $finalData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $finalData]);
                }
            } else {
                $this->logger->debug('General Setting update process failed.' . $finalData, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $finalData]);
            }
        }
        $this->logger->info('General Setting update process is complete.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the user permission to edit general setting.
     * 
     * @return  string 
     * @since   Method available since version 6.1.0
     */
    private function disabledEdit()
    {

        $disabled = "true";

        if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER || $_SESSION['userInfo']['userType'] == Constants::USER_TYPE_ADMIN) {
            $disabled = "false";
        }

        return $disabled;
    }

    /**
     * It retrieves all general setting data.
     *
     * @throws  PDOException
     * @return  array|string could be array if success,could be string if fail
     * @since      Class available since version 6.1.0
     */
    public function getGeneralSettingParamValue()
    {
        try {
            $data =  $this->generalSettingModel->getParameterTableValue();
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It updates general setting data.
     *
     * @param   array  $updateData updated data of general setting.
     * @throws  PDOException
     * @return  array   
     * @since      Class available since version 6.1.0
     */
    public function updateData($updateData)
    {
        try {
            $this->beginTransaction();
            $data = array();
            if (DATA_SOURCE_NAME == Constants::DB_MYSQL) {
                //if ($this->generalSettingModel->getLock(Constants::SCREEN_GENERAL_SETTING)) {
                if ($this->generalSettingModel->updateParameterTable($updateData)) {
                    $data = Constants::AJAX_MESSAGE_SUCCESS;
                    $this->logger->info('General Setting has been updated.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $data = Constants::AJAX_MESSAGE_FAIL;
                }
                // } else {
                //     $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_DB_LOCK;
                // }
            } else {
                // if ($this->generalSettingModel->getPgLock(Constants::SCREEN_GENERAL_SETTING)) {
                if ($this->generalSettingModel->updateParameterTable($updateData)) {
                    $data = Constants::AJAX_MESSAGE_SUCCESS;
                    $this->logger->info('General Setting has been updated.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $data = Constants::AJAX_MESSAGE_FAIL;
                }
                // } else {
                //     $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_DB_LOCK;
                // }
            }
            $this->commit();
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $data = (array)$this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
            return $data;
        }
    }
}
