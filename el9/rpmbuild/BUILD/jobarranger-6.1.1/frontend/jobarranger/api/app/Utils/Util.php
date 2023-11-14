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

namespace App\Utils;

use App\Models\GeneralSettingModel;
use Exception;
use DateTime;

class Util
{
    /**
     * It checks first character of password.
     *
     * @param string $string run user password
     * @param string $startString
     * @return bool could be true if first char of password is 1, could be false if not
     * @since      function available since version 6.1.0
     */
    public function startsWith($string, $startString)
    {
        $len = strlen($startString);
        return (substr($string, 0, $len) === $startString);
    }

    /**
     * It encrypts password.
     *
     * @param  string $password 
     * @return string encrypts run user password
     * @since      function available since version 6.1.0
     */
    public function getPasswordFromString($str)
    {
        $key = str_split("199907");
        $enc = "1";
        $toX16 = "";

        $j = null;
        $b = null;
        $j = 0;

        $strLen = strlen($str);
        $strArray = str_split($str);
        for ($i = 0; $i < $strLen; $i++) {
            $m = (string) $strArray[$i];
            $n = (string) $key[$j];
            $b = ($m ^ $n);
            //char >> binary >> hex
            $toX16 = bin2hex(iconv('UTF-8', 'ISO-8859-1', $b));
            // if ($b < 16) {
            //     $toX16 = "0" . base_convert($b, 10, 16);
            // } else {
            //     $toX16 = base_convert($b, 10, 16);
            // }
            $enc = (string) $enc . (string) $toX16;

            $j++;
            if ($j == count($key)) $j = 0;
        }

        return $enc;
    }

    /**
     * It decrypts password.
     *
     * @param  string $password 
     * @return string decrypted run user password
     * @since      function available since version 6.1.0
     */
    public function getStringFromPassword($password)
    {
        $key = str_split("199907");
        $dec = "";

        $j = 0;

        $passArray = str_split($password);
        $passLen = count($passArray);
        for ($i = 0; $i < $passLen; $i++) {
            $dd = $password[$i];
            $dec = $dec . ($password[$i] ^ $key[$j]);
            $j++;
            if ($j == count($key)) $j = 0;
        }
        return $dec;
    }

    /**
     * It decrypts password.
     *
     * @param  string $password run user password
     * @return string decoded password 
     *@since      function available since version 6.1.0
     */
    public function getStringFromX16Password($password)
    {
        $enc_code = "";
        $de_code = "";
        $x16 = "";

        $passArray = str_split($password);
        $passLen = count($passArray);
        for ($kk = 0; $kk < $passLen; $kk++) {
            if ($kk == 0) {
                continue;
            } else if (($kk % 2) == 1) {
                $x16 = $passArray[$kk];
            } else {
                $x16 = $x16 . $passArray[$kk];
                $x16 = base_convert($x16, 16, 10);
                $character = json_encode(chr($x16));
                $character = (string)chr($x16);
                $enc_code = $enc_code . $character;
                $x16 = "";
            }
        }
        $de_code = $this->getStringFromPassword($enc_code);

        return $de_code;
    }

    /**
     * It checks basic field validation (present and neither empty nor only white space).
     *
     * @param string $str run user password
     * @return bool could be true if null or white space, could be false for valid password
     * @since      function available since version 6.1.0
     */
    function IsNullOrEmptyString($str)
    {
        return ($str === null || trim($str) === '');
    }

    /**
     * It decrypts password.
     *
     * @param string $passwordText encrypted password
     * @return string decrypted password
     * @since      function available since version 6.1.0
     */
    function getStringFromPass($passwordText)
    {
        if ($this->startsWith($passwordText, '1')) {
            return $this->getStringFromX16Password($passwordText);
        } else {
            return $this->getStringFromPassword($passwordText);
        }
    }

    static function jsonDecode(string $json)
    {
        try {
            $test = json_decode($json, true);
            return $test;
        } catch (Exception $e) {
            return false;
        }
    }
    /**
     * Get datetime by checking ja_parameter_table.MANAGER_TIME_SYNC;
     * Use server time if MANAGER_TIME_SYNC value is 1(server time).
     * Use local time(from UI request) if MANAGER_TIME_SYNC value is 2(Local time).
     * If error occurred (on both local and server), use PHP server DateTime.
     * 
     * @return string "YYYYMMDDHHmmss".
     * @since      function available since version 6.1.0
     */
    public function getDate()
    {
        $generalSettingModel = new GeneralSettingModel();
        $req_raw = file_get_contents('php://input');
        //get local date from react server.
        if (isset($req_raw)) {
            $client_date = json_decode($req_raw, true)['date'];
            $temp_date = date_create_from_format("YmdHis", $client_date); //check if string is date with desired format.
        }
        //if request does not contain date or wrong date format, get PHP server datetime.
        if (!isset($req_raw) || !$temp_date) {
            $temp_date = new DateTime();
        }
        $local_date = $temp_date->format("YmdHis");
        $managerTimeSync = $generalSettingModel->getParameterValue("MANAGER_TIME_SYNC");
        //check general_setting->Job Arranger standard time
        if ($managerTimeSync == 1) {
            //server time
            try {
                $db = Core::db();
                if (DATA_SOURCE_NAME == Constants::DB_MYSQL) {
                    $db->query("SELECT CURRENT_TIMESTAMP AS systemtime");
                } else {
                    $db->query("SELECT NOW() :: timestamp(0) AS systemtime");
                }
                $db->execute();
                $tmp_server_date = $db->resultSet()['0'];
                $server_date_obj = date_create_from_format("Y-m-d H:i:s", $tmp_server_date->systemtime);
                $serverDate = $server_date_obj->format("YmdHis");
                return $serverDate;
            } catch (Exception $e) {
                //if server error, use local time.
                return $local_date;
            }
        } else {
            //local time
            return $local_date;
        }
    }


    /**
     * create a response json by given parameters.
     * @param string $type  Response Message Type.
     * @param string $detail    Detail of the response.
     * @param string $ObjectId  ID of resulting response object.
     * @param array $returnItemData related object list.
     * @return array    return a json message response.
     */
    public function createResponseJson($detail = "", $ObjectId = "", $returnItemData = [])
    {
        if ($detail == null || $detail == "") {
            $detail == "lab-server-error";
        }
        $detailTxt = Constants::MESSAGE[$detail];
        if ($detailTxt == null) {
            $detailTxt = "";
        }
        $data[Constants::AJAX_MESSAGE_DETAIL] =  $detail;
        $data[Constants::AJAX_MESSAGE_OBJECTID] = $ObjectId;
        $data[Constants::AJAX_MESSAGE_DETAIL_TXT] = $detailTxt;
        if ($returnItemData != null) {
            $data[Constants::AJAX_RETURN_ITEM_DATA] = (array)$returnItemData;
        }
        return (array)$data;
    }

    /**
     * create a common api response json by given parameters.
     * @param string $type  Response Type.
     * @param string $detail    Detail of the response.
     * @return array    return a json message response.
     */
    static function response($type, $detail = [])
    {
        return json_encode([
            Constants::API_RESPONSE_TYPE => $type,
            Constants::API_RESPONSE_DETAIL => $detail
        ]);
    }

    static function getLockObjectType(int $objectType): String
    {
        switch ($objectType) {
            case Constants::OBJECT_TYPE_CALENDAR:
                $objectTypeString = Constants::OBJECT_TYPE_CALENDAR_STRING;
                break;
            case Constants::OBJECT_TYPE_SCHEDULE:
                $objectTypeString = Constants::OBJECT_TYPE_SCHEDULE_STRING;
                break;
            case Constants::OBJECT_TYPE_FILTER:
                $objectTypeString = Constants::OBJECT_TYPE_FILTER_STRING;
                break;
            case Constants::OBJECT_TYPE_JOBNET:
                $objectTypeString = Constants::OBJECT_TYPE_JOBNET_STRING;
                break;
        }
        return $objectTypeString;
    }
}
