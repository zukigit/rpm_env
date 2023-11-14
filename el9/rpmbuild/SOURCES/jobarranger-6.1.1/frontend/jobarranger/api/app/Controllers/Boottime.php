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
use App\Utils\Util;
use App\Models\BoottimeModel;
use PDOException;
use Rakit\Validation\Validator;

/**
 * This controller is used to manage the boottime.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Boottime extends Controller
{
    public function __construct()
    {
        $this->boottimeModel = new BoottimeModel();
        $this->logger = Core::logger();
        $this->validator = new Validator();
        $this->util = new Util();
    }

    /**
     * It checks boottime is already exists or not in session.
     *
     * @return  string could be true if exists, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function isBoottimeExists($id,$boottime,$objLists)
    {
    
        if (isset($objLists)) {
            foreach ($objLists as $key => $value) {
                if ($value['boottime'] == $boottime && $value['id'] == $id) {
                    return true;
                   
                }
            }
            
        }
        return false ;
    }

  

    /**
     * It gets ID list for select that related with Calendar/Filter type.
     *
     * @since   Method available since version 6.1.0
     */
    public function getCalFltIDList()
    
    {
        
        $this->logger->info('In boottime registration screen, selecting Calendar/Filter id process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $result = $this->getCalFltIDLists();
        if (!is_array($result)) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
        }
        $this->logger->info('In boottime registration screen, selecting Calendar/Filter id process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        
    }

    /**
     * It retrieves id list of calendar or filter objects.
     *
     * @param   string $type object type
     * @throws  PDOException
     * @return  array  
     * @since   Method available since version 6.1.0
     */
    public function getCalFltIDLists()
    {
        
        try {
            $pub_cal_id_list = $this->boottimeModel->getPublicCalendar();

            $pri_cal_id_list = $this->boottimeModel->getPrivateCalendar();

            $pub_fil_id_list = $this->boottimeModel->getPublicFilter();

            $pri_fil_id_list = $this->boottimeModel->getPrivateFilter();

            $id_list = array($pub_cal_id_list, $pri_cal_id_list, $pub_fil_id_list, $pri_fil_id_list);
            return $id_list;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
        
    }


      /**
     * It retrieves id list of jobnet objects.
     *
     * @param   string $type jobnet type public|private
     * @throws  PDOException
     * @return  array  
     * @since   Method available since version 6.1.0
     */
    public function getJobnetIDList()
    {
        try {
            $this->logger->info('In boottime registration screen, selecting jobnet id process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            
            $pub_jobnet_id_list = $this->boottimeModel->getPublicJobnet();
            $pri_jobnet_id_list = $this->boottimeModel->getPrivateJobnet();

            $id_list = array($pub_jobnet_id_list, $pri_jobnet_id_list);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $id_list]);
            $this->logger->info('In boottime registration screen, selecting jobnet id process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
        }
    }


    /**
     * It redirects to schedule_jobnet screen with init data.
     *
     * @since   Method available since version 6.1.0
     */
    public function jobnet()
    {
        $json = file_get_contents('php://input');
        // Converts it into a PHP object
        $data = Util::jsonDecode($json)['params'];

        $validation = $this->validator->validate($data, [
            'optid' => 'required',
            'optDate' => 'required',
            'type' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        }else{
            $this->logger->info('Jobnet registration process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $optID = $data["optid"];
            $objLists = $data["objLists"];
          
            $result = $this->getJobnetInfo($optID,$objLists);

            if(is_array($result)){
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
            }else if($result==0){
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
            }else{
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            }

            
            $this->logger->debug('The Jobnet selected is: [' . json_encode($result) . ']', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
       
    }

    /**
     * It retrieves jobnet info.
     *
     * @param   string $optID 
     * @throws  PDOException
     * @return  array|string could be array if success, could be string if fail  
     * @since   Method available since version 6.1.0
     */
    public function getJobnetInfo($optID,$objLists)
    {
        $result = [];
        try {
            $data = $this->boottimeModel->getJobnetInfo($optID);
            
            if($this->isJobnetExists($data[0]["jobnetId"],$objLists)){
                $result = 0;
            }else{
                $result=$data[0];
                
            }
            return $result;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It checks boottime is already exists or not in session.
     *
     * @return  string could be true if exists, could be false if not
     * @since   Method available since version 6.1.0
     */
    public function isJobnetExists($id,$objLists)
    {
         
        if (isset($objLists)) {
            foreach ($objLists as $key => $value) {
                if ($value['jobnetId'] == $id) {
                    return true;
                 
                }
            }
            
        }
        return false ;
    }

    /**
     * It redirects to schedule_boottime screen with init data.
     *
     * @since   Method available since version 6.1.0
     */
    public function registration()
    {

        $this->logger->info('Boottime registration process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
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
        }else{
            if($data["params"]["time"] == "time"){
                $result = $this->getBoottimeInfo($data["params"]);  
            }else{
                $result = $this->create($data["params"]);  
            }
            if(is_array($result)){
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
            }else if($result==0){
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result]);
            }else{
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            }
        }
        $this->logger->info('Boottime registration process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
         
    }

    /**
     * It retrieves boottime information of calendar or filter.
     *
     * @param   string $optID
     * @param   string $type
     * @param   string $updateDate
     * @return  array|string could be array if success, could be string if fail
     * @since   Method available since version 6.1.0
     */
    public function getBoottimeInfo($obj)
    {
        $optID = $obj['optid'];
        $type = $obj["type"];
        $updateDate = $obj['optDate'];
        $boottime = $obj['boottime'];
        $objLists = $obj['objLists'];
        $data=[];

        try {
            if (strpos($type, "calendar") !== false) {
                $getdata = $this->boottimeModel->getCalendarInfo($optID, $updateDate);
                if($this->isBoottimeExists($getdata[0]["calendarId"],$boottime,$objLists)){
                    $data= 0;
                } else{
                   array_push($data,
                        ['id'=>$getdata[0]["calendarId"],
                        'type' => 'calendar',
                        'updateDate'=>$getdata[0]["updateDate"],
                        'createdDate'=>$getdata[0]["createdDate"],
                        'validFlag'=>$getdata[0]['validFlag'],
                        'publicFlag'=>$getdata[0]['publicFlag'],
                        'userName'=>$getdata[0]["userName"],
                        'name'=>$getdata[0]["calendarName"],
                        'memo'=>$getdata[0]["memo"],
                        'boottime'=>$boottime,
                        'oldBoottime'=>$boottime]
                    );
                        
                    $lastday = $this->boottimeModel->getCalLastDay($optID, $type, $updateDate);
                    
                }
                
            } else {

                $getdata = $this->boottimeModel->getFilterInfo($optID, $updateDate);
                if($this->isBoottimeExists($getdata[0]["filterId"],$boottime,$objLists)){
                    $data= 0;
                } else{
                    array_push($data, [
                        'id'=>$getdata[0]["filterId"],
                        'type' => 'filter',
                        'updateDate'=>$getdata[0]["updateDate"],
                        'createdDate'=>$getdata[0]["createdDate"],
                        'validFlag'=>$getdata[0]['validFlag'],
                        'publicFlag'=>$getdata[0]['publicFlag'],
                        'userName'=>$getdata[0]["userName"],
                        'name'=>$getdata[0]["filterName"],
                        'memo'=>$getdata[0]["memo"],
                        'boottime'=>$boottime,
                        'oldBoottime'=>$boottime
                    ]);
                    $lastday = "";
                    
                }
            };
            // $data = [
            //     'getdata'  => $getdata,
            //     'lastday'  => $lastday
            // ];
            return $data;
        } catch (PDOException $e) {
            $this->showErrMsg(Constants::SERVICE_INTERNAL_SERVER_ERROR);
        }
    }

    /**
     * It creates new schedule.
     *
     * @since   Method available since version 6.1.0
     */
    public function create($data)
    {
           
            $optID = $data['optid'];
            $type = $data["type"];
            $updateDate = $data['optDate'];
            $objLists = $data['objLists'];
            $result=[];
              
                $start = strtotime($data['startTime']);
                $end = strtotime($data['endTime']);
                if (!$end) {
                    $end_over = explode(':', $data['endTime']);
                    $end = strtotime("24:00");
                    $end_over = strtotime((int)$end_over[0] - 24 . ":" . (int)$end_over[1]);
                    $get_over_time = ($end_over - strtotime("00:00")) / 60;
                }
                $diff = ($end - $start) / 60;
                $min = $data['interval'];
                if ((int)explode(':', $data['endTime'])[0] > 24) {
                    $rows = (($diff + $get_over_time)  / $min);
                } else {
                    $rows = ($diff  / $min);
                }

                $result_boots = [];

                for ($x = 0; $x <= $rows; $x++) {
                    $boots = null;
                    $chgMin = $min * $x;
                    array_push($result_boots, strtotime("+" . $chgMin . " minutes", $start));
                }

                if ($type=='calendar') {
                    $getdata = $this->boottimeModel->getCalendarInfo($optID, $updateDate);
                    foreach ($result_boots as $key => $val) {
                        $found = false;

                        foreach ($objLists as $key => $value) {
                            
                            if ($data['optid'] == $value['id'] && date('H:i', $val) == $value['boottime']) {
                                $found = true;
                                break;
                            }
                        }
                        if(!$found){
                            array_push($result,[
                                'id'=>$getdata[0]["calendarId"],
                                'type' => 'calendar',
                                'updateDate'=>$getdata[0]["updateDate"],
                                'createdDate'=>$getdata[0]["createdDate"],
                                'validFlag'=>$getdata[0]['validFlag'],
                                'publicFlag'=>$getdata[0]['publicFlag'],
                                'userName'=>$getdata[0]["userName"],
                                'name'=>$getdata[0]["calendarName"],
                                'memo'=>$getdata[0]["memo"],
                                'boottime'=>date("H:i", $val),
                                'oldBoottime'=>date("H:i", $val)
                            ]);
                        }
                    }
                }else{
                    $getdata = $this->boottimeModel->getFilterInfo($optID, $updateDate);
                    foreach ($result_boots as $key => $val) {
                        $found = false;
                        foreach ($data['objLists'] as $key => $value) {
                            if ($data['optid'] == $value['id'] && date('H:i', $val) == $value['boottime']) {
                                $found = true;
                                break;
                            }
                        }
                        if(!$found){     
                            array_push($result,[
                                'id'=>$getdata[0]["filterId"],
                                'type' => 'filter',
                                'updateDate'=>$getdata[0]["updateDate"],
                                'createdDate'=>$getdata[0]["createdDate"],
                                'validFlag'=>$getdata[0]['validFlag'],
                                'publicFlag'=>$getdata[0]['publicFlag'],
                                'userName'=>$getdata[0]["userName"],
                                'name'=>$getdata[0]["filterName"],
                                'memo'=>$getdata[0]["memo"],
                                'boottime'=>date("H:i", $val),
                                'oldBoottime'=>date("H:i", $val)
                            ]);
                        }
                    }
                }; 
                 
        return $result;
    }

    // /**
    //  * It gets boottime information.
    //  *
    //  * @since   Method available since version 6.1.0
    //  */
    // public function getDate()
    // {
    //     $optID = $_GET['id'];
    //     $date = $_GET['update_date'];
    //     $type = $_GET['type'];
    //     $optDate = $_GET['date'];
    //     $result = $this->boottimeService->getBoottimeInfo($optID, $type, $date, $optDate);
    //     if (!is_array($result)) {
    //         if ($result == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
    //             $this->responseFail($result);
    //         }
    //     } else {
    //         echo json_encode($result);
    //     }
    // }

    /**
     * It gets operation date of the calendar.
     *
     * @since   Method available since version 6.1.0
     */
    public function getDateCalendar()
    {
        $this->logger->info('Get Calendar Date process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $id = $_GET['id'];
        $updateDate = empty($_GET['update_date']) ? null : $_GET['update_date'];
        $result = $this->calendarService->getOperationDate($id, $updateDate);
        echo json_encode($result);
        $this->logger->info('Get Calendar Date process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It gets base calendar id for filter.
     *
     * @since   Method available since version 6.1.0
     */
    public function getBaseCalendar()
    {
        $this->logger->info('Get BaseCalendar Date process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $optionID = $_GET['id'];
        if (!isset($optionID)) {
            $this->itemNotFound();
            exit;
        }

        $result = $this->filterService->getCalendarID($optionID);
        if (!is_array($result)) {
            if ($result == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                $this->itemNotFound();
                exit;
            }
        } else {
            echo json_encode($result);
        }
        $this->logger->info('Get BaseCalendar Date process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirect to  error_page.
     *
     * @since   Method available since version 6.1.0
     */
    public function itemNotFound()
    {
        require_once '../app/controllers/Pages.php';
        $pages = new Pages();
        $pages->error();
    }
}
