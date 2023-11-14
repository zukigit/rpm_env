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
use App\Helpers\LocaleHelper;
use App\Utils\Controller;
use App\Utils\Core;
use App\Services\ObjectDetailService;
use App\Controllers\Calendar;
use App\Controllers\Filter;
use App\Controllers\Jobnet;
use App\Controllers\Schedule;
use App\Services\ScheduleService;
use App\Services\FilterService;
use App\Utils\Constants;
use DateTime;
use App\Utils\Util;

/**
 * This controller is used to manage the object list.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ObjectList extends Controller
{
    public function __construct()
    {
        parent::__construct();
        $this->objectDetailService = new ObjectDetailService();
        $this->calendar = new Calendar();
        $this->schedule = new Schedule();
        $this->filter = new Filter();
        $this->jobnet = new Jobnet();
        $this->logger = Core::logger();
        $this->validator = new Validator();
        $this->utils = new Util();
    }

    // function list()
    // {
    //     $updateData = json_decode($_POST['updateData'], true);
    // }

    /**
     * It retrieves all object lists on search data.
     *
     * @since   Method available since version 6.1.0
     */
    public function getAll()
    {
        $this->logger->info('Select object list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');

        // Converts it into a PHP object
        $data = Util::jsonDecode($json);

        if ($data == false) {
            //echo $this->response->errorWrongArgs(["Invalid Json Request"]);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, Constants::API_RESPONSE_TYPE_BAD_REQUEST);
        }
        $validation = $this->validator->validate($data["params"], [
            'publicType' => 'required',
            'category' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::API_RESPONSE_TYPE_BAD_REQUEST, "lab-bad-request", "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            //echo $this->response->errorWrongArgs($errors->firstOfAll());

        } else {
            $objectPublicType =  $data["params"]["publicType"];
            $pageObject = $data["params"]["category"];
            $publicFlag = strcmp($objectPublicType, 'public') == 0 ? 1 : 0;

            $this->logger->info('Select object type is ' . $pageObject . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $search = "";
            // backend search features
            // if ($searchArr[0] !== "") {
            //     $search = $search . $pageObject . "_id" . "  LIKE '%" . $searchArr[0] . "%' and ";
            // }
            // if ($searchArr[1] !== "") {
            //     $search = $search . $pageObject . "_name" . "  LIKE '%" . $searchArr[1] . "%' and ";
            // }
            // if ($searchArr[2] !== "") {
            //     $search = $search . "memo" . "  LIKE '%" . $searchArr[2] . "%' and ";
            // }

            $retrieveData = $this->objectDetailService->getLazyObjectData($pageObject, $publicFlag, $search);
            //echo $this->response->withArray($retrieveData);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $retrieveData);
        }
        $this->logger->info('Select object list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    public function changeValidList()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $request = json_decode($req_raw, true)['params'];
            switch ($request['datas']['category']) {
                case "calendar":
                    $result = $this->calendar->changeValidList();
                    break;
                case "filter":
                    $result = $this->filter->changeValidList();
                    break;
                case "schedule":
                    $result = $this->schedule->changeValidList();
                    break;
                case "jobnet":
                    $result = $this->jobnet->changeValidList();
                    break;
                default:
                    $result = "error";
            }
            if (!is_array($result)) {
                if ($result == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                    //echo $this->response->withError(Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND, 204, $result);
                }
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
        } else {
            //echo $this->response->errorWrongArgs(["datas" => "request data is corrupted or missing."]);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, Constants::API_RESPONSE_TYPE_BAD_REQUEST);
        }
    }
    /**
     * It deletes the calendar list from object_list screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteList(): void
    {
        $this->logger->info('Multiple calendar delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $req_raw = file_get_contents('php://input');
        if (!isset($req_raw)) {
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST);
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $para = json_decode($req_raw);
        $validationReq = (array)$para;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'datas' => 'required',
        ]);
        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->utils->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }


        $request = json_decode($req_raw, true)['params'];
        switch ($request['datas']['category']) {
            case "calendar":
                $result = $this->calendar->deleteList($request['datas']);
                break;
            case "filter":
                $result = $this->filter->deleteList($request['datas']);
                break;
            case "schedule":
                $result = $this->schedule->deleteList($request['datas']);
                break;
            case "jobnet":
                $result = $this->jobnet->deleteList($request['datas']);
                break;
            default:
                $result = "error";
        }

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
        $this->logger->info('Multiple calendar version delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirects to object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function version(): void
    {
        $this->logger->info('Calendar version search process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!isset($_GET['id'])) {
            $responseData = $this->utils->createResponseJson(Constants::API_RESPONSE_TYPE_BAD_REQUEST, "lab-bad-request", "");
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }

        $result = $this->calendar->version();
        if (!is_array($result)) {
            if ($result == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
            }
        } else {
            //echo $this->response->withArray($result);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
        }
        $this->logger->info('Calendar version search process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
