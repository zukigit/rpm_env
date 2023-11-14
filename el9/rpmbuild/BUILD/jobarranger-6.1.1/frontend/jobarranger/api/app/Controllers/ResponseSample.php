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
use App\Services\ObjectDetailService;
use DateTime;

/**
 * This controller is used to manage the object list.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ResponseSample extends Controller
{
    public function __construct()
    {
        parent::__construct();
        $this->objectDetailService = new ObjectDetailService();
        $this->logger = Core::logger();
    }

    function welcome()
    {
        echo "Welcome to Job Arranger Manager API";
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
    function sampleDataArray()
    {
        $objectPublicType =  $_GET['type'];
        $pageObject = $_GET['category'];
        $publicFlag = strcmp($objectPublicType, 'public') == 0 ? 1 : 0;

        $this->logger->info('Select ' . $pageObject . ' object list load process is started.', ['controller' => __METHOD__]);
        $search = "";

        $retrieveData = $this->objectDetailService->getLazyObjectData($pageObject, $publicFlag, $search);
        $this->logger->info('Select ' . $pageObject . ' object list load process is finished.', ['controller' => __METHOD__]);
        // echo json_encode($retrieveData, JSON_HEX_TAG);
        echo $this->response->withArray($retrieveData);
    }

    function sampleData()
    {
        $objectPublicType =  $_GET['type'];
        $pageObject = $_GET['category'];
        $publicFlag = strcmp($objectPublicType, 'public') == 0 ? 1 : 0;

        $this->logger->info('Select ' . $pageObject . ' object list load process is started.', ['controller' => __METHOD__]);
        $search = "";

        $retrieveData = $this->objectDetailService->getLazyObjectData($pageObject, $publicFlag, $search);
        $this->logger->info('Select ' . $pageObject . ' object list load process is finished.', ['controller' => __METHOD__]);
        // echo json_encode($retrieveData, JSON_HEX_TAG);
        echo $this->response->withItem($retrieveData[0]);
    }

    function sampleOk()
    {
        echo $this->response->ok("Data is created successfully");
    }

    function sampleOkWithReturn()
    {
        echo $this->response->ok("Data is save successfully", ["objectId"=>22]);
    }

    function errorBadRequest()
    {
        echo $this->response->errorWrongArgs(["objectId" => "input is required","objectName" =>"input is required"]);
    }

    function errorForbidden()
    {
        echo $this->response->errorForbidden("You can do this operation");
    }

    function errorUnauthorized()
    {
        echo $this->response->withError("You are unauthorised", 406);
    }
}
