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
use App\Models\RunJobnetModel;
use App\Models\JobnetModel;
use App\Models\CalendarModel;
use App\Models\IndexModel;
use App\Models\JobExecutionManagementModel;
use App\Utils\Controller;
use App\Utils\Core;
use App\Utils\Constants;
use App\Utils\Util;
use PDOException;

/**
 * This controller is used to manage the job execution.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobExecution extends Controller
{

    private $timeout = 300;

    public function __construct()
    {
        parent::__construct();
        $this->runJobnetModel = new RunJobnetModel;
        $this->jobnetModel = new JobnetModel;
        $this->calendarModel = new CalendarModel;
        $this->indexModel = new IndexModel;
        $this->jobExecutionManagementModel = new JobExecutionManagementModel;
        $this->util = new Util();
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    /**
     * api endpoint that skip the run job.
     *
     * @since   Method available since version 6.1.0
     */
    public function setSkip()
    {
        $this->logger->info('Set skips process is started,', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required',
                'runStatus' => 'required|in:0,1,2,3,4,5,6'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $setData = [];

                if ($params["runStatus"] == Constants::RUN_JOB_STATUS_TYPE_NONE || $params["runStatus"] == Constants::RUN_JOB_STATUS_TYPE_PREPARE || $params["runStatus"] == Constants::RUN_JOB_STATUS_TYPE_RUN_ERR) {

                    if ($params["runStatus"] == Constants::RUN_JOB_STATUS_TYPE_RUN_ERR) {
                        $setData = [
                            "method_flag" => Constants::RUN_JOB_METHOD_TYPE_SKIP,
                            "status" => Constants::RUN_JOB_STATUS_TYPE_PREPARE
                        ];
                    } else {
                        $setData = [
                            "method_flag" => Constants::RUN_JOB_METHOD_TYPE_SKIP
                        ];
                    }
                }

                $where = "inner_jobnet_id = " . $params["innerJobnetId"] . " and inner_job_id = " . $params["innerJobId"];
                if (!$this->runJobnetModel->updateRunJobMethod($setData, $where)) {
                    throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                }
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Set skip process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that holds the run job.
     *
     * @since   Method available since version 6.1.0
     */
    public function setHold()
    {
        $this->logger->info('Set hold process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->setMethod($params["innerJobnetId"], $params["innerJobId"], Constants::RUN_JOB_METHOD_TYPE_HOLD);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Set hold process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that rerun the run job.
     *
     * @since   Method available since version 6.1.0
     */
    public function setRerun()
    {
        $this->logger->info('Rerun process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $json = file_get_contents('php://input');

            $data = Util::jsonDecode($json);

            $validation = $this->validator->validate($data["params"], [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $updateData = [
                    "method_flag" => Constants::RUN_JOB_METHOD_TYPE_RERUN,
                    "status" => Constants::RUN_JOB_STATUS_TYPE_PREPARE,
                    "timeout_flag" => Constants::RUN_JOB_TIMEOUT_TYPE_NORMAL
                ];
                $where = "inner_jobnet_id = " . $data["params"]["innerJobnetId"] . " and inner_job_id = " . $data["params"]["innerJobId"];

                if (!$this->runJobnetModel->updateRunJobMethod($updateData, $where)) {
                    throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                }
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Rerun process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that stop the run job.
     *
     * @since   Method available since version 6.1.0
     */
    public function setForceStop()
    {
        $this->logger->info('Force stop process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->setMethod($params["innerJobnetId"], $params["innerJobId"], Constants::RUN_JOB_METHOD_TYPE_STOP);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Force stop process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that set method to normal status to the run job.
     *
     * @since   Method available since version 6.1.0
     */
    public function setNormal()
    {
        $this->logger->info('Set normal process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->setMethod($params["innerJobnetId"], $params["innerJobId"], Constants::RUN_JOB_METHOD_TYPE_NORMAL);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Set normal process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * it updates the run job method flag.
     *
     * @param   int $innerJobnetId
     * @param   int $innerJobId
     * @param   int $method
     * @throws  PDOException
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function setMethod($innerJobnetId, $innerJobId, $method)
    {
        $this->logger->debug('Set method function is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $data = [
                "method_flag" => $method
            ];
            $where = "inner_jobnet_id = $innerJobnetId and inner_job_id = $innerJobId";
            if (!$this->runJobnetModel->updateRunJobMethod($data, $where)) {
                throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
            }

            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->debug('Set method function is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that changes the run job variable value.
     *
     * @since   Method available since version 6.1.0
     */
    public function variableValueChange()
    {
        $this->logger->info('Variable value change process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'innerJobId' => 'required',
                'variables' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                foreach ($params['variables'] as $variable) {

                    $updateData = [
                        'inner_job_id' => $params['innerJobId'],
                        'table_name' => $variable['tableName'],
                        'value_name' => $variable['valueName'],
                        'value_column' => $variable['valueColumn'],
                        'value' => $variable['value'],
                    ];
                    if (!$this->runJobnetModel->updateBeforeVariableMethod($updateData)) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }
                }

                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Variable value change process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that check the jobnet is executable or not
     *
     * @since   Method available since version 6.1.0
     */
    public function checkValid(): void
    {
        $this->logger->info('Check jobnet to execute process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'id' => 'required',
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                if (count($this->runJobnetModel->getValidJobnetVersion($params["id"])) > 0) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_VALID]);
                } else {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INVALID]);
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Check jobnet to execute process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that execute the jobnet.
     *
     * @since   Method available since version 6.1.0
     */
    public function run(): void
    {
        $this->logger->info('Jobnet execution process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'id' => 'required',
                'runType' => 'required|in:1,2,3,5'// run, hold run, test run, single run
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $recheckJobnetValidVersion = $this->runJobnetModel->getValidJobnetVersion($params["id"]);
                if (count($recheckJobnetValidVersion) > 0) {

                    $innerJobnetId = $this->indexModel->getNextID(Constants::COUNT_INNER_JOBNET_ID)->nextid;

                    $runJobnetData = [
                        "update_date" => $recheckJobnetValidVersion[0]->update_date,
                        "public_flag" => $recheckJobnetValidVersion[0]->public_flag,
                        "multiple_start_up" => $recheckJobnetValidVersion[0]->multiple_start_up,
                        "jobnet_id" => $recheckJobnetValidVersion[0]->jobnet_id,
                        "user_name" => $recheckJobnetValidVersion[0]->user_name,
                        "jobnet_name" => $recheckJobnetValidVersion[0]->jobnet_name,
                        "memo" => $recheckJobnetValidVersion[0]->memo,
                    ];
                    $runJobnet = $this->runJobnetModel->insertRunJobnet($runJobnetData, $params["runType"], $_SESSION['userInfo']['userName'], $innerJobnetId);
                    if ($runJobnet) {
                        $this->indexModel->increateNextID(Constants::JOB_EXEC_MANAGEMENT_COUNT_ID);
                        $loopFlag = true;
                        $errMessage = "";
                        $startTime = time();
                        while ($loopFlag) {
                            usleep(1000000); // 1 second
                            $runJobnetSummary = $this->runJobnetModel->getRunJobnetSummary($innerJobnetId);

                            if (count($runJobnetSummary) > 0) {
                                $loadStatus = $runJobnetSummary[0]->load_status;
                                if ($loadStatus == Constants::LOAD_STATUS_TYPE_NONE) {
                                    echo $this->response->ok("Jobnet is executed.", [
                                        'innerJobnetId' => $innerJobnetId
                                    ]);
                                    $loopFlag = false;
                                    break;
                                }
                                // 遅延起動
                                else if ($loadStatus == Constants::LOAD_STATUS_TYPE_DELAY) {
                                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "Because a starting job net existed, the job was reserved.", Constants::API_RESPONSE_MESSAGE_CODE => "err_jobnet_002"]);
                                    $this->logger->info("Because a starting job net existed, the job was reserved.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                                    $loopFlag = false;
                                    break;
                                }
                                // 実行スキップ
                                else if ($loadStatus == Constants::LOAD_STATUS_TYPE_SKIP) {
                                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "The practice of the job net was canceled(skipped).", Constants::API_RESPONSE_MESSAGE_CODE => "err_jobnet_003"]);
                                    $this->logger->info("The practice of the job net was canceled(skipped).", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                                    $loopFlag = false;
                                    break;
                                } else {
                                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "The job net of a job net icon and a task icon is invalid.", Constants::API_RESPONSE_MESSAGE_CODE => "err_jobnet_001"]);
                                    $this->logger->info("The job net of a job net icon and a task icon is invalid.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                                    $loopFlag = false;
                                    break;
                                }
                            }
                            $endTime = time();
                            if (($endTime - $startTime) > $this->timeout) {
                                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "Job execution is timeout. May be jobarg-server is down.", Constants::API_RESPONSE_MESSAGE_CODE => "err_jobnet_004"]);
                                $this->logger->error("Job execution is timeout. May be jobarg-server is down.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                                $loopFlag = false;
                                break;
                            }
                        }
                    }
                } else {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INVALID]);
                    $this->logger->info("Run jobnet is not valid.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Jobnet execution process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that execute the single job.
     *
     * @since   Method available since version 6.1.0
     */
    public function singleJobRun()
    {
        $this->logger->info('Single job run process is started,', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $json = file_get_contents('php://input');

        $params = Util::jsonDecode($json)["params"];

        $validation = $this->validator->validate($params, [
            'jobnetId' => 'required',
            'iconSetting' => 'required',
            'methodFlag' => 'required|in:0,1,2,3,4'
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            try {
                $this->beginTransaction();
                $startXPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_START_X);
                $startYPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_START_Y);
                $jobXPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_JOB_X);
                $jobYPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_JOB_Y);
                $endXPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_END_X);
                $endYPoint = $this->runJobnetModel->getParameterData(Constants::JOBNET_DUMMY_END_Y);

                $dt_inner_jobnet = $this->indexModel->getNextID(Constants::COUNT_INNER_JOBNET_ID);
                $dt_inner_job_start = $this->indexModel->getNextID(Constants::COUNT_INNER_JOB_ID);
                $dt_inner_flow1 = $this->indexModel->getNextID(Constants::COUNT_INNER_FLOW_ID);

                $this->indexModel->increateNextID(Constants::COUNT_INNER_JOBNET_ID);
                $this->indexModel->increateNextID(Constants::COUNT_INNER_JOB_ID, 3);
                $this->indexModel->increateNextID(Constants::COUNT_INNER_FLOW_ID, 2);

                $strInnerJobnetNextId = "";
                $strInnerJobNextIdStart = "";
                $strInnerJobNextIdJob = "";
                $strInnerJobNextIdEnd = "";
                $strInnerFlowNextId1 = "";
                $strInnerFlowNextId2 = "";
                $strJobnetNextId = "";

                $strNow = date("Ymdhis");

                $runJobnetName = $params["jobnetId"] . "/" . $params["iconSetting"]["jobId"];

                if ($dt_inner_jobnet && $dt_inner_job_start && $dt_inner_flow1) {
                    $strInnerJobnetNextId = $dt_inner_jobnet->nextid;
                    $strInnerJobNextIdStart = $dt_inner_job_start->nextid;
                    $strInnerJobNextIdJob = (string)((int)$dt_inner_job_start->nextid + 1);
                    $strInnerJobNextIdEnd = (string)((int)$dt_inner_job_start->nextid + 2);
                    $strInnerFlowNextId1 = $dt_inner_flow1->nextid;
                    $strInnerFlowNextId2 = (string)((int)$dt_inner_flow1->nextid + 1);
                    $strJobnetNextId = "RUN_JOB_" . (string)$dt_inner_jobnet->nextid;
                } else {
                    echo "Error retreiving next ID";
                }

                $insertRunJobnetData = [
                    "update_date" => $strNow,
                    "public_flag" => Constants::PRIVATE_FLAG,
                    "multiple_start_up" => Constants::MULTIPLE_START_UP_YES,
                    "jobnet_id" => $strJobnetNextId,
                    "user_name" => $_SESSION['userInfo']['userName'],
                    "jobnet_name" => $runJobnetName,
                    "memo" => null,
                ];
                $runJobnetResult = $this->runJobnetModel->insertRunJobnet($insertRunJobnetData, Constants::RUN_TYPE_SINGLE_RUN, $_SESSION['userInfo']['userName'], $strInnerJobnetNextId);

                $insertRunJobnetSummaryData = [
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "update_date" => $strNow,
                    "invo_flag" => Constants::INVO_FLAG_DEPLOY_COMPLETE,
                    "run_type" => Constants::RUN_TYPE_SINGLE_RUN,
                    "start_time" => 0,
                    "end_time" => 0,
                    "public_flag" => 0,
                    "multiple_start_up" => Constants::MULTIPLE_START_UP_YES,
                    "jobnet_id" => $strJobnetNextId,
                    "user_name" => $_SESSION['userInfo']['userName'],
                    "jobnet_name" => $runJobnetName,
                    "memo" => null,
                    "jobnet_timeout" => 0,
                ];
                $this->runJobnetModel->insertRunJobnetSummary($insertRunJobnetSummaryData);

                $insertRunJobStartData = [
                    "inner_job_id" => $strInnerJobNextIdStart,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "inner_jobnet_main_id" => $strInnerJobnetNextId,
                    "job_type" => Constants::ICON_TYPE_START,
                    "invo_flag" => Constants::INVO_FLAG_DEPLOY_COMPLETE,
                    "boot_count" => 0,
                    "start_time" => 0,
                    "end_time" => 0,
                    "point_x" => $startXPoint,
                    "point_y" => $startYPoint,
                    "job_id" => "START",
                    "method_flag" => 0,
                    "force_flag" => 0,
                    "job_name" => null,
                    "run_user" => null,
                    "run_user_password" => null,
                ];
                $this->runJobnetModel->insertRunJob($insertRunJobStartData);

                $insertRunJobData = [
                    "inner_job_id" => $strInnerJobNextIdJob,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "inner_jobnet_main_id" => $strInnerJobnetNextId,
                    "job_type" => Constants::ICON_TYPE_JOB,
                    "invo_flag" => Constants::INVO_FLAG_DEPLOY_COMPLETE,
                    "boot_count" => 1,
                    "start_time" => 0,
                    "end_time" => 0,
                    "point_x" => $jobXPoint,
                    "point_y" => $jobYPoint,
                    "job_id" => $params["iconSetting"]["jobId"],
                    "method_flag" => $params["methodFlag"],
                    "force_flag" => $params["iconSetting"]["forceFlag"],
                    "job_name" => $params["iconSetting"]["jobName"],
                    "run_user" => $params["iconSetting"]["runUser"],
                    "run_user_password" => isset($params["iconSetting"]["runUserPassword"]) ? (!$this->util->IsNullOrEmptyString($params["iconSetting"]["runUserPassword"]) ? $this->util->getPasswordFromString($params["iconSetting"]["runUserPassword"]) : null) : null,
                ];
                $this->runJobnetModel->insertRunJob($insertRunJobData);

                $insertRunJobEndData = [
                    "inner_job_id" => $strInnerJobNextIdEnd,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "inner_jobnet_main_id" => $strInnerJobnetNextId,
                    "job_type" => Constants::ICON_TYPE_END,
                    "invo_flag" => Constants::INVO_FLAG_DEPLOY_COMPLETE,
                    "boot_count" => 1,
                    "start_time" => 0,
                    "end_time" => 0,
                    "point_x" => $endXPoint,
                    "point_y" => $endYPoint,
                    "job_id" => "END-1",
                    "method_flag" => Constants::RUN_JOB_METHOD_TYPE_NORMAL,
                    "force_flag" => 0,
                    "job_name" => null,
                    "run_user" => null,
                    "run_user_password" => null,
                ];
                $this->runJobnetModel->insertRunJob($insertRunJobEndData);

                $insertRunIconJobData = [
                    "inner_job_id" => $strInnerJobNextIdJob,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "host_flag" => $params["iconSetting"]["hostFlag"],
                    "stop_flag" => $params["iconSetting"]["stopFlag"],
                    "timeout" => $params["iconSetting"]["timeout"],
                    "host_name" => $params["iconSetting"]["hostName"],
                    "stop_code" => $params["iconSetting"]["stopCode"],
                ];
                $this->runJobnetModel->insertRunIconJob($insertRunIconJobData);

                $insertRunIconEndData = [
                    "inner_job_id" => $strInnerJobNextIdEnd,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                ];
                $this->runJobnetModel->insertRunIconEnd($insertRunIconEndData);

                $insertRunFlowData1 = [
                    "inner_flow_id" => $strInnerFlowNextId1,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "start_inner_job_id" => $strInnerJobNextIdStart,
                    "end_inner_job_id" => $strInnerJobNextIdJob,
                ];
                $this->runJobnetModel->insertRunFlow($insertRunFlowData1);

                $insertRunFlowData2 = [
                    "inner_flow_id" => $strInnerFlowNextId2,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "start_inner_job_id" => $strInnerJobNextIdJob,
                    "end_inner_job_id" => $strInnerJobNextIdEnd,
                ];
                $this->runJobnetModel->insertRunFlow($insertRunFlowData2);

                $insertRunJobExecCommand = [
                    "inner_job_id" => $strInnerJobNextIdJob,
                    "inner_jobnet_id" => $strInnerJobnetNextId,
                    "command_cls" => 0,
                    "command" => $params["iconSetting"]["exec"],
                ];
                $this->runJobnetModel->insertRunJobCommand($insertRunJobExecCommand);

                if ($params["iconSetting"]["stopFlag"] == 1) {
                    $insertRunJobStopCommand = [
                        "inner_job_id" => $strInnerJobNextIdJob,
                        "inner_jobnet_id" => $strInnerJobnetNextId,
                        "command_cls" => Constants::COMMAND_CLS_STOP_COMMAND,
                        "command" => $params["iconSetting"]["stopCommand"],
                    ];
                    $this->runJobnetModel->insertRunJobCommand($insertRunJobStopCommand);
                }

                if (sizeof($params["iconSetting"]["valueJob"]) > 0) {
                    foreach ($params["iconSetting"]["valueJob"] as $valueJob) {
                        $valueJobData = [
                            'inner_job_id' => $strInnerJobNextIdJob,
                            'inner_jobnet_id' => $strInnerJobnetNextId,
                            'value_name' =>  $valueJob["valueName"],
                            'value' => $valueJob["value"],
                        ];

                        $this->runJobnetModel->insertRunValueJob($valueJobData);
                    }
                }

                if (sizeof($params["iconSetting"]["valueJobCon"]) > 0) {
                    foreach ($params["iconSetting"]["valueJobCon"] as $valueJobCon) {
                        $valueJobConData = [
                            'inner_job_id' => $strInnerJobNextIdJob,
                            'inner_jobnet_id' => $strInnerJobnetNextId,
                            'value_name' => $valueJobCon,
                        ];

                        $this->runJobnetModel->insertRunValueJobControl($valueJobConData);
                    }
                }

                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS, Constants::API_RESPONSE_DATA => [
                    'innerJobnetId' => $strInnerJobnetNextId
                ]]);
            } catch (PDOException $e) {
                if ($this->db->inTransaction()) {
                    $this->rollback();
                }
                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
        }

        $this->logger->info('Single job run process is finished,', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves the run jobnet data.
     *
     * @since   Method available since version 6.1.0
     */
    public function getRunJobnetData(): void
    {
        $this->logger->info('Get run jobnet data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'id' => 'required',
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $innerJobnetId = $params["id"];
                $runJobnetSummary = $this->runJobnetModel->getRunJobnetSummary($innerJobnetId);
                $runJobnet = $this->runJobnetModel->getRunJobnetTable($innerJobnetId);

                if (count($runJobnet) > 0) {
                    [$result, $runJobData, $runFlow] = $this->getRunJobData($innerJobnetId);
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => ["runJobSummary" => $runJobnetSummary[0] ?? null, "runJobnet" => $runJobnet[0], "runJob" => $runJobData, "runFlow" => $runFlow]]);
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Get run jobnet data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves the run detail job data.
     *
     * @param   int $innerJobnetId
     * @throws  PDOException
     * @return  array
     * @since   Method available since version 6.1.0
     */
    private function getRunJobData($innerJobnetId)
    {
        $this->logger->debug('Get Job data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $runJob = $this->runJobnetModel->getRunJob($innerJobnetId);
            $runJobData = [];
            $runJobCount = count($runJob);
            if ($runJobCount > 0) {
                for ($i = 0; $i < $runJobCount; $i++) {
                    $data = $this->getDetailJobData($runJob[$i]);
                    $beforeVariable = $this->runJobnetModel->getRunJobValueBefore($runJob[$i]->inner_job_id, $runJob[$i]->job_type);
                    $afterVariable = $this->runJobnetModel->getRunJobValueAfter($runJob[$i]->inner_job_id);
                    array_push($runJobData, [
                        "jobData" => $runJob[$i],
                        "toolTip" => $data["toolTipMsg"],
                        "isErrorTooltip" => $data["isErrorTooltip"],
                        "iconSetting" => isset($data["iconSetting"]) ? $data["iconSetting"] : null,
                        "jobCommand" => isset($data["jobCommand"]) ? $data["jobCommand"] : null,
                        "valueJob" => isset($data["valueJob"]) ? $data["valueJob"] : null,
                        "valueJobControl" => isset($data["valueJobControl"]) ? $data["valueJobControl"] : null,
                        "beforeVariable" => $beforeVariable,
                        "afterVariable" => $afterVariable
                    ]);
                }
            }

            $runFlow = $this->runJobnetModel->getRunFlow($innerJobnetId);

            return [Constants::SERVICE_MESSAGE_SUCCESS, $runJobData, $runFlow];
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->debug('Get Job data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves the detail job data such as icon setting, tooltip message.
     *
     * @param   object $job
     * @throws  PDOException
     * @return  array
     * @since   Method available since version 6.1.0
     */
    private function getDetailJobData($job)
    {
        try {
            $toolTipMsg = "";
            $isErrorTooltip = false;
            $data = array();

            switch ($job->job_type) {
                case Constants::ICON_TYPE_END:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconEnd($job->inner_job_id)[0];

                    break;

                case Constants::ICON_TYPE_CONDITIONAL_START:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconIf($job->inner_job_id)[0];

                    break;

                case Constants::ICON_TYPE_JOB_CONTROL_VARIABLE:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconValue($job->inner_job_id);
                    
                    break;

                case Constants::ICON_TYPE_JOB:

                    $data["iconSetting"] = $this->runJobnetModel->getRunIconJob($job->inner_job_id)[0];
                    $data["jobCommand"] = $this->runJobnetModel->getRunIconJobCommand($job->inner_job_id);
                    $data["valueJob"] = $this->runJobnetModel->getRunIconValueJob($job->inner_job_id);
                    $data["valueJobControl"] = $this->runJobnetModel->getRunIconValueJobControl($job->inner_job_id);

                    if (!$this->util->IsNullOrEmptyString($job->run_user_password)) {
                        $job->run_user_password = $this->util->getStringFromPass($job->run_user_password);
                    }

                    break;

                case Constants::ICON_TYPE_JOBNET:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconJobnet($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_EXTENDED_JOB:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconExtended($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_CALCULATION:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconCalculation($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_TASK:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconTask($job->inner_job_id)[0];

                    break;

                case Constants::ICON_TYPE_INFO:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconInfo($job->inner_job_id)[0];
                    $calendarData = $this->calendarModel->getValidORMaxUpdateDateEntityById($data["iconSetting"]->get_calendar_id);
                    if($calendarData != false){
                        $data["iconSetting"]->calendar_name = $calendarData->calendar_name;
                    }
                    
                    break;

                case Constants::ICON_TYPE_FILE_COPY:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconFileTransfer($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_FILE_WAIT:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconFileWait($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_REBOOT:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconReboot($job->inner_job_id)[0];

                    break;

                case Constants::ICON_TYPE_RELEASE:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconReleaseHold($job->inner_job_id)[0];
                    
                    break;

                case Constants::ICON_TYPE_AGENT_LESS:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconAgentLess($job->inner_job_id)[0];
                    
                    if (!$this->util->IsNullOrEmptyString($data['iconSetting']->login_password)) {
                        $loginPW = $this->util->getStringFromPass($data['iconSetting']->login_password);
                        if(strpos($loginPW, '|') != false){
                            $strPass = explode("|", $loginPW);
                            $loginPW = $strPass[1];
                        }
                        $data['iconSetting']->login_password = $loginPW;
                    }
                    break;

                case Constants::ICON_TYPE_ZABBIX:
                    $data["iconSetting"] = $this->runJobnetModel->getRunIconZabbix($job->inner_job_id)[0];
                    
                    break;

            }
            if ($job->status == Constants::RUN_JOB_STATUS_TYPE_RUN_ERR || $job->status == Constants::RUN_JOB_STATUS_TYPE_ABNORMAL) {
                $initialAfterValue = $this->runJobnetModel->getRunJobValueAfter($job->inner_job_id);

                if (count($initialAfterValue) > 0) {
                    $toolTipMsg = "";
                    $stdOut = "";
                    $stdErr = "";
                    $jobExitCode = "";
                    $jobargMsg = "";

                    for ($k = 0; $k < count($initialAfterValue); $k++) {

                        if ($initialAfterValue[$k]->value_name == 'STD_OUT') {
                            $stdOut = $initialAfterValue[$k]->after_value;
                        }

                        if ($initialAfterValue[$k]->value_name == 'STD_ERR') {
                            $stdErr = $initialAfterValue[$k]->after_value;
                        }

                        if ($initialAfterValue[$k]->value_name == 'JOB_EXIT_CD') {
                            $jobExitCode = $initialAfterValue[$k]->after_value;
                        }

                        if ($initialAfterValue[$k]->value_name == 'JOBARG_MESSAGE') {
                            $jobargMsg = $initialAfterValue[$k]->after_value;
                        }
                    }
                    $toolTipMsg = $toolTipMsg . "<div>STD_OUT : " . $stdOut . "</div><div>STD_ERR : " . $stdErr . "</div><div>JOB_EXIT_CD : " . $jobExitCode . "</div><div>JOBARG_MESSAGE : " . $jobargMsg . "</div>";
                    $isErrorTooltip = true;
                }
            }
            $data["toolTipMsg"] = $toolTipMsg;
            $data["isErrorTooltip"] = $isErrorTooltip;
            return $data;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->info('Get detail job data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
