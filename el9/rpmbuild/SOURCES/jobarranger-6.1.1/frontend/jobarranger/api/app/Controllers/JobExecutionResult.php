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
use App\Models\JobExceResultModel;
use App\Models\UserModel;
use PDOException;
use Exception, DateTime;
use App\Utils\Util;



/**
 * This controller is used to manage the job execution result.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobExecutionResult extends Controller
{

    private $userAlias;

    private $csvExportFilePath;

    private $fields = array('log date', 'inner jobnet main id', 'inner jobnet id', 'run type', 'public flag', 'jobnet id', 'job id', 'message id', 'message', 'jobnet name', 'job name', 'user name', 'update date', 'return code');

    function __destruct()
    {
        if (file_exists($this->csvExportFilePath)) {
            unlink($this->csvExportFilePath);
        }
    }

    public function __construct()
    {
        parent::__construct();
        $this->logger = Core::logger();
        $this->userModel = new UserModel();
        $this->jobExceResModel = new JobExceResultModel();
        register_shutdown_function(function () {
            $error = error_get_last();
            // if (null !== $error) {
            //     echo 'Caught at shutdown';
            // }
            if($error != null){
                if ($error['message'] != null) {
                    $response = [
                        Constants::AJAX_MESSAGE_DETAIL =>  $error['message'],
                    ];
                    echo Util::response(Constants::API_RESPONSE_TYPE_500, $response);
                    $this->logger->error($error['message'], ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
        });
    }

    /**
     * It redirects to error_page screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function itemNotFound()
    {
        require_once '../app/controllers/Pages.php';
        $pages = new Pages();
        $pages->error();
    }

    /**
     * It redirects to job_execution_result screen with all user name.
     *
     * @since   Method available since version 6.1.0
     */
    public function getAllUser()
    {
        $this->logger->info('Job Execution Result Screen is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->userAlias = $this->userModel->getAllUserAlias();
            echo $this->response->withArray($this->userAlias);
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $this->itemNotFound();
            exit;
        }
        $this->logger->info('Job Execution Result Screen is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It searchs job execution result data.
     *
     * @since   Method available since version 6.1.0
     */
    public function searchResult()
    {
        $this->logger->info('Job Execution Result Screen - Search function is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $parameter = (array) json_decode($json);
        $searchData = (array) $parameter['params'];
        $search = "";
        $this->logger->debug('Search Data - ' . json_encode($searchData), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!empty($searchData['jobnetId'])) {
            $search = $search . "jobnet_id" . "  LIKE '%" . $searchData['jobnetId'] . "%' and ";
        }
        if (!empty($searchData['jobId'])) {
            $search = $search  . "job_id" . "  LIKE '%" . $searchData['jobId'] . "%' and ";
        }
        if (!empty($searchData['manageId'])) {
            if (DATA_SOURCE_NAME == Constants::DB_MYSQL) {
                $search = $search . " inner_jobnet_main_id " . "  LIKE '%" . $searchData['manageId'] . "%' and ";
            }
            if (DATA_SOURCE_NAME == Constants::DB_PGSQL) {
                $search = $search . " inner_jobnet_main_id::text " . "  LIKE '%" . $searchData['manageId'] . "%' and ";
            }
        }
        if (!empty($searchData['userName'])) {
            $search = $search . " user_name " . "  LIKE '%" . $searchData['userName'] . "%' and ";
        }
        $searchData['search'] = $search;
        $resultArray = $this->jobExceResModel->getEntity($searchData);
        $this->logger->info('Job Execution Result Screen - Search function is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        echo $this->response->withArray($resultArray);
    }

    /**
     * It exports job execution result data to CSV file.
     *
     * @since   Method available since version 6.1.0
     */
    public function exportCSV()
    {
        $this->logger->info('Job Execution Result Screen - Export function is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $parameter = (array) json_decode($json);
        $searchData = (array) $parameter['params'];
        $csvRootText = ucwords('JobExecutionResult');

        $search = "";
        $this->logger->debug('Search Data - ' . json_encode($searchData), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!empty($searchData['jobnetId'])) {
            $search = $search . "jobnet_id" . "  LIKE '%" . $searchData['jobnetId'] . "%' and ";
        }
        if (!empty($searchData['jobId'])) {
            $search = $search  . "job_id" . "  LIKE '%" . $searchData['jobId'] . "%' and ";
        }
        if (!empty($searchData['manageId'])) {
            $search = $search . " inner_jobnet_main_id " . "  LIKE '%" . $searchData['manageId'] . "%' and ";
        }
        if (!empty($searchData['userName'])) {
            $search = $search . " user_name " . "  LIKE '%" . $searchData['userName'] . "%' and ";
        }
        $searchData['search'] = $search;

        $filename = $this->generateExportCSVFileName($csvRootText);

        if ($filename != false) {
            $retVal = $this->createCsvFileInTemp($searchData);

            if ($retVal != false) {
                $this->logger->debug('Temp csv file creation completed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            } else {
                $this->logger->error('Temp csv file cannot create.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }

            if (false != $retVal) {
                $fp = fopen($this->csvExportFilePath, 'rb');

                // send the right headers
                header("Content-Type: text/csv");
                header("Content-Length: " . filesize($this->csvExportFilePath));

                // dump the picture and stop the script
                fpassthru($fp);
            }
        }

        $this->logger->info('Job Execution Result Screen - Export function is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It create csv file in temp folder.
     *
     * @param  array $searchData
     * @return string temp csv file path.
     * @since   Method available since version 6.1.0
     */
    private function createCsvFileInTemp($searchData)
    {

        try {
            $output = fopen($this->csvExportFilePath, 'w');
            if (!$output) {
                $this->logger->error('File cannot create.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return false;
            }
            function push(&$arrange, $item)
            {
                $value = str_replace('"', '""', $item);
                $format = '"%s"';
                $arrange[] = sprintf($format, $value);
            }
            $header = [];
            foreach ($this->fields as $key => $data) {
                push($header, $data);
            }
            fputs($output, implode(',', $header) . "\r\n");
            $totalRows = $this->jobExceResModel->getResTotal($searchData);

            $start = 0;
            $limit = 10000;
            do {
                $searchResult = $this->jobExceResModel->getExportResult($searchData, $start, $limit);
                foreach ($searchResult as $key => $data) {
                    fputs($output, implode(',', $this->createCsvLine($data)) . "\r\n");
                }
                $start = $start + $limit;
            } while ($start <= $totalRows);
            fclose($output);
            return $this->csvExportFilePath;
        } catch (PDOException $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return false;
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return false;
        }
    }

    /**
     * It create csv line.
     *
     * @param  array $data jobexecutionresult data. 
     * @return array $arr
     * @since   Method available since version 6.1.0
     */
    private function createCsvLine($data)
    {
        $arr = [];
        $log_date = $data["log_date"];
        $miliSec = substr($log_date, -3);
        $log_date = substr($log_date, 0, -3);
        $dateFormat = DateTime::createFromFormat('YmdHis', $log_date);
        $formated_log_date = $dateFormat->format("Y/m/d H:i:s") . ".{$miliSec}";
        push($arr, $formated_log_date);
        push($arr, $data["inner_jobnet_main_id"]);
        push($arr, $data["inner_jobnet_id"]);
        push($arr, $data["run_type"]);
        push($arr, $data["public_flag"]);
        push($arr, $data["jobnet_id"]);
        push($arr, $data["job_id"]);
        push($arr, $data["message_id"]);
        push($arr, $data["message"]);
        push($arr, $data["jobnet_name"]);
        push($arr, $data["job_name"]);
        push($arr, $data["user_name"]);
        push($arr, (new DateTime($data["update_date"]))->format('Y/m/d H:i:s'));
        push($arr, $data["return_code"]);
        return $arr;
    }

    /**
     * It generates file name in temp directory.
     *
     * @param  string $surfix JobExecutionResult. 
     * @return string $fileName csv file name
     * @since   Method available since version 6.1.0
     */
    private function generateExportCSVFileName($surfix = "")
    {

        $this->logger->info('Generating file name in temp directory process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $fileName = $this->getAppTempDir();
        if (!$fileName) {
            $this->logger->error('Cannot generate file name.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return false;
        }

        $fileName .= "Export_";
        if (!$surfix == "") {
            $fileName .= ucwords($surfix) . "_";
        }
        $fileName .= gmdate("YmdHisv", time()) . ".csv";

        $this->logger->debug('File name generation is successful.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $this->csvExportFilePath = $fileName;

        return $fileName;
    }

    /**
     * It create temp directory.
     *
     * @return string $appTempPath temp file path.
     * @since   Method available since version 6.1.0
     */
    private function getAppTempDir()
    {

        $this->logger->debug('Temp directory create process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $appTempPath = rtrim(sys_get_temp_dir(), DIRECTORY_SEPARATOR);

        if (!is_dir($appTempPath) || !is_writable($appTempPath)) {
            $this->logger->error('"' . $appTempPath . '" - does not have right permission.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return false;
        }
        $appTempPath .= DIRECTORY_SEPARATOR . Constants::APP_TEMP_FOLDER_NAME . DIRECTORY_SEPARATOR;

        if (!file_exists($appTempPath)) {
            if (!mkdir($appTempPath, 0700)) {
                $this->logger->error('Cannot create temp folder.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return false;
            }
        }
        return $appTempPath;
    }
}
