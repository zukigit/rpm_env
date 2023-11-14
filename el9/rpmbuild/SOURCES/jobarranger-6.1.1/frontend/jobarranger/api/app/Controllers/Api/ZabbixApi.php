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

namespace App\Controllers\Api;

use App\Utils\Constants;

/**
 * This controller is used to manage zabbix api request.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ZabbixApi
{
    /**
     * It requests host,item and trigger info for zabbix linkage icon.
     *
     * @param  string      $method could be method name
     * @param  array       $params 
     * @return string|bool $result could be string if success,could be bool if fail  
     * @since   Method available since version 6.1.0
     */
    public static function RequestApi($method, $params)
    {

        $ch = curl_init(ZBX_API_ROOT . Constants::ZBX_MAIN_END_POINT);

        $payload = json_encode(array(
            'jsonrpc' =>  '2.0',
            'method' =>  $method,
            'params' => $params,
            'auth' => $_SESSION['userInfo']['sessionId'],
            'id' =>  1
        ));

        // Attach encoded JSON string to the POST fields
        curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);

        // Set the content type to application/json
        curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));

        // Return response instead of outputting
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

        // Execute the POST request//bypass ssl check
        // curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        // curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);
        $result = curl_exec($ch);

        // Close cURL resource
        curl_close($ch);

        return $result;
    }

    public static function GetUserGroup($sessionId, $userId)
    {

        $ch = curl_init(ZBX_API_ROOT . Constants::ZBX_MAIN_END_POINT);

        $payload = json_encode(array(
            'jsonrpc' =>  '2.0',
            'method' =>  'usergroup.get',
            'params' => [
                'output' => 'extend',
                'status' => 0,
                "userids" => $userId
            ],
            'id' =>  1,
            'auth' => $sessionId
        ));

        // Attach encoded JSON string to the POST fields
        curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);

        // Set the content type to application/json
        curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));

        // Return response instead of outputting
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

        // Execute the POST request//bypass ssl check
        // curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        // curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);
        $result = curl_exec($ch);

        // Close cURL resource
        curl_close($ch);

        return $result;
    }
}
