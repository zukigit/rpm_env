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

use Rakit\Validation\Validator;
use App\Utils\Controller;
use App\Utils\Core;
use App\Utils\Constants;
use App\Controllers\Api\LoginApi;
use App\Controllers\Api\ZabbixApi;
use App\Models\GeneralSettingModel;
use App\Services\UserService;
use App\Utils\Util;

/**
 * This controller is used to manage the user authentication.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Users extends Controller
{

    public function __construct()
    {
        parent::__construct();
        
        $this->generalSettingModel = new GeneralSettingModel();
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    public function apiCheck()
    {
        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "Authentication is success.", Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
    }

    /**
     * It validates user information and login.
     *
     * @since   Method available since version 6.1.0
     */
    public function login()
    {
        $this->logger->info('User login process is started.', ['controller' => __METHOD__]);

        $json = file_get_contents('php://input');
        $data = Util::jsonDecode($json);
        if ($data == false) {
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_JSON_INVALID]);
            $this->logger->info(Constants::SERVICE_MESSAGE_JSON_INVALID, ['controller' => __METHOD__]);
            return;
        }
        $validation = $this->validator->validate($data["params"], [
            'username' => 'required',
            'password' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            $result = LoginApi::AuthPathAPI($data["params"]);
            if($result == false){
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => "Couldn't connect with zabbix api"]);
                $this->logger->error("Couldn't connect with zabbix api", ['controller' => __METHOD__]);
                return;
            }
            $final = json_decode($result);
            if (isset($final->result->sessionid)) {
                $this->logger->info('Authentication is success.', ['controller' => __METHOD__]);

                $lang_cut = explode("_", $final->result->lang);
                $lang = ($lang_cut[1] == "JP") ?  "JP" : "EN";

                $groupResult = json_decode(ZabbixApi::GetUserGroup($final->result->sessionid, $final->result->userid));
                
                if(isset($groupResult->error)){
                    $groupResultArray = [];
                }else{
                    $groupResultArray = $groupResult->result;
                }

                $heartbeatIntervalTime = $this->generalSettingModel->getParameterValue("HEARTBEAT_INTERVAL_TIME");

                $data = [
                    "userName" => $final->result->username,
                    "language" => strtoupper($lang),
                    "sessionId" => $final->result->sessionid,
                    "appVersion" => Constants::APP_VERSION,
                    "userId" => $final->result->userid,
                    "userType" => $final->result->type,
                    "userLangFull" => strtolower($final->result->lang),
                    "hasUserGroup" => (count($groupResultArray) > 0),
                    "groupList" => $groupResultArray,
                    "heartbeatIntervalTime" => (int) $heartbeatIntervalTime
                ];
                $this->logger->info('User login process is finished.', ['controller' => __METHOD__, 'user' => $final->result->username]);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "Authentication is success.", Constants::API_RESPONSE_DATA => $data]);
            } else {
                $this->logger->info($final->error->data, ['controller' => __METHOD__]);
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $final->error->data]);
            }
        }
    }

    /**
     * It removes the user session id to logout the system.
     *
     * @since   Method available since version 6.1.0
     */
    public function logout()
    {
        $this->logger->info('User logout process is started.', ['controller' => __METHOD__]);

        $json = file_get_contents('php://input');
        $data = Util::jsonDecode($json);
        if ($data == false) {
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_JSON_INVALID]);
            $this->logger->info(Constants::SERVICE_MESSAGE_JSON_INVALID, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return;
        }
        $validation = $this->validator->validate($data, [
            'sessionId' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } else {
            $result = LoginApi::logoutAPI($data["sessionId"]);
            $final = json_decode($result);
            if (isset($final->error)) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $final->error->data]);
                $this->logger->info($final->error->data, ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            } else {
                if ($final->result) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "User is successfully logout"]);
                    $this->logger->info("User logout process is successfully finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
        }
    }
}
