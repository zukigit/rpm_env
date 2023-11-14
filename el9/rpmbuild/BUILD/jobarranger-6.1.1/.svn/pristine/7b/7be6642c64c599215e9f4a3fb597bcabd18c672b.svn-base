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
use App\Models\ExportModel;
use App\Utils\Util;
use Exception, DOMDocument, SimpleXMLElement, PDOException;
use Rakit\Validation\Validator;

/**
 * This controller is used to manage file export.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Export extends Controller
{

    private $xmlExportFilePath;

    public function __construct()
    {
        $this->exportModel = new ExportModel();
        $this->logger = Core::logger();
        $this->validator = new Validator();
        $this->utils = new Util();
    }

    function __destruct()
    {
        if (file_exists($this->xmlExportFilePath)) {
            unlink($this->xmlExportFilePath);
        }
    }

    /**
     * api endpoint that export object data as a xml file.
     * It export objects as XML file
     *
     * @since   Method available since version 6.1.0
     */
    public function exportXml()
    {
        $this->logger->info("Export xml process is started.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');

        $params = Util::jsonDecode($json)["params"];
        $validation = $this->validator->validate($params, [
            'objectType' => 'required',
            'exportType' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return;
        } else {
            $xmlRootText = ucwords($params['objectType']);

            [$xmlDataArr, $xmlRootText] = $this->ExportData($params, $xmlRootText);

            $this->generateExportXMLFileName($xmlRootText);

            $newArr = [];
            for ($i = 0; $i < sizeof($xmlDataArr); $i++) {
                //check created_date format
                $newKey = "";
                $newValue = [];
                foreach ($xmlDataArr[$i] as  $key => $value) {
                    $newKey = $key;
                    $newValue = $value;
                    if (!empty($newValue['created_date'])) {
                        $date = $newValue['created_date'];
                        $timeZoneFormat = "Y-m-d\TH:i:s" . $params['timeZone'];
                        $createdDate = date($timeZoneFormat, strtotime($date));
                        $newValue['created_date'] = $createdDate;
                    }
                }
                array_push($newArr, [$newKey => $newValue]);
            }
            //if (count($xmlDataArr) > 0 && isset($this->xmlExportFilePath)) {
            $userInfoArr = array(
                "UserInfo" => array(
                    "user_name" => $_SESSION['userInfo']['userName'],
                    "type" => Constants::USER_TYPE_NAMES_ARRAY[$_SESSION['userInfo']['userType']]
                )
            );
            array_unshift($newArr, $userInfoArr);

            if (false != $this->createXml($newArr, $xmlRootText)) {
                $fp = fopen($this->xmlExportFilePath, 'rb');

                // send the right headers
                header("Content-Type: text/txt");
                header("Content-Length: " . filesize($this->xmlExportFilePath));

                // dump the picture and stop the script
                fpassthru($fp);
            }
            //}
        }
        $this->logger->info("Export xml process is finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It create the xml element.
     *
     * @return  string|bool temp file path of export file
     * @since   Method available since version 6.1.0
     */
    private function createXml(array $data, string $rootText)
    {
        try {
            $xmlData = new SimpleXMLElement('<' . $rootText . '></' . $rootText . '>');
            if (!isset($xmlData)) {
                return false;
            }

            $this->arrayToXmlElement($data, $xmlData);

            $dom = new DOMDocument('1.0', 'UTF-8');
            if (!isset($dom)) {
                return false;
            }
            $dom->preserveWhiteSpace = TRUE;
            $dom->formatOutput = TRUE;
            $dom->encoding = 'utf-8';
            $dom->loadXML($xmlData->asXML());
            $xmlString = $dom->saveXML($dom->documentElement);
            $retVal = $this->createXMLFileInTemp($xmlString);
            return $retVal;
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return false;
        }
        $this->logger->info("Create xml process is finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It change array data to xml element.
     *
     * @throws  Exception
     * @since   Method available since version 6.1.0
     */
    private function arrayToXmlElement(array $data, object &$xml_data)
    {
        try {
            foreach ($data as $key => $value) {
                if (is_array($value)) {
                    if (!is_numeric($key)) {
                        $subnode = $xml_data->addChild($key);
                        $this->arrayToXmlElement($value, $subnode);
                    } else {
                        $this->arrayToXmlElement($value, $xml_data);
                    }
                } else {
                    if (!is_null($value)) {
                        $xml_data->addChild("$key", htmlspecialchars(str_ireplace(["\r\n", "\r"], "\n", "$value")));
                    }
                }
            }
            $this->logger->debug("Convert array to xml element.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            throw $e;
        }
    }

    /**
     * It create xml file in temp directory.
     *
     * @return  string|bool could be temp file directory if success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    private function createXMLFileInTemp(string $xmlString)
    {
        try {
            $handle = fopen($this->xmlExportFilePath, "w");
            if (!$handle) {
                return false;
            }
            fwrite($handle, $xmlString);
            fclose($handle);
            $this->logger->debug("Create xml file in temp is finised.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $this->xmlExportFilePath;
        } catch (Exception $e) {
            return false;
        }
    }

    /**
     * It generates temp file name.
     *
     * @since   Method available since version 6.1.0
     */
    private function generateExportXMLFileName(string $surfix = "")
    {
        $fileName = $this->getAppTempDir();
        if (!$fileName) {
            return false;
        }

        $fileName .= "Export_";
        if (!$surfix == "") {
            $fileName .= ucwords($surfix) . "_";
        }
        $fileName .= gmdate("YmdHisv", time()) . ".xml";
        $this->xmlExportFilePath = $fileName;
        $this->logger->debug("Generate xml file path is " . $this->xmlExportFilePath . ".", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $this->logger->debug("Generate xml file name process is finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It gets temp directory.
     *
     * @return  string temp folder path
     * @since   Method available since version 6.1.0
     */
    private function getAppTempDir()
    {
        $appTempPath = rtrim(sys_get_temp_dir(), DIRECTORY_SEPARATOR);

        if (!is_dir($appTempPath) || !is_writable($appTempPath)) {
            return false;
        }
        $appTempPath .= DIRECTORY_SEPARATOR . Constants::APP_TEMP_FOLDER_NAME . DIRECTORY_SEPARATOR;

        if (!file_exists($appTempPath)) {
            if (!mkdir($appTempPath, 0700)) {
                return false;
            }
        }
        $this->logger->debug("Temp directory is " . $appTempPath . ".", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $appTempPath;
    }

    /**
     * It retrieves objects data on export type.
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function exportData(array $params, string $xmlRootText)
    {
        try {
            switch ($params["exportType"]) {
                case Constants::EXPORT_TYPE_VERSION:
                    $xmlDataArr = $this->exportModel->getExportSpecialVerData($params['objectType'], $params['data']);
                    break;
                case Constants::EXPORT_TYPE_OBJECT:
                    $xmlDataArr = $this->exportModel->getExportObjectData($params['objectType'], $params['data']);
                    break;
                case Constants::EXPORT_TYPE_ALL:
                    $xmlRootText = "All";
                    $xmlDataArr = $this->exportModel->getExportAllData($_SESSION['userInfo']['userId']);
                    break;
            }
            $this->logger->info("Export " . $params["exportType"] . " process is finished.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return [$xmlDataArr, $xmlRootText];
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500);
        }
    }
}
