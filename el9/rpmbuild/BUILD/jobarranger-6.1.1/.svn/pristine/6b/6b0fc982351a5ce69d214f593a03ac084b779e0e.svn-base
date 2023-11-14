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
use App\Models\JobExecutionManagementModel;
use App\Models\RunJobnetModel;
use App\Utils\Controller;
use App\Utils\Core;
use App\Utils\Constants;
use App\Utils\Util;
use DateInterval;
use DateTime;
use PDOException;

/**
 * This controller is used to manage the Jobnet Execution Management.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class JobnetExecManagement extends Controller
{

    private $jobnetViewSpan = 0;

    public function __construct()
    {
        parent::__construct();
        $this->runJobnetModel = new RunJobnetModel;
        $this->jobExecutionManagementModel = new JobExecutionManagementModel;
        $this->util = new Util;
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    /**
     * api endpoint that retrieves the all operation list
     *
     * @since   Method available since version 6.1.0
     */
    public function getAllOperationList()
    {
        $this->logger->info('Get all operation list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->jobnetViewSpan = $this->runJobnetModel->getParameterData(Constants::JOBNET_VIEW_SPAN);

            $nowString = $this->util->getDate();

            $after = new DateTime($nowString);
            $after->add(new DateInterval('PT' . $this->jobnetViewSpan . 'M'));
            $before = new DateTime($nowString);
            $before->sub(new DateInterval('PT' . $this->jobnetViewSpan . 'M'));

            $fromTime = $before->format('YmdHi');
            $toTime = $after->format('YmdHi');
            $startFromTime = $before->format('YmdHis');
            $startToTime = $after->format('YmdHis');

            $jobnetList = null;

            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $jobnetList = $this->jobExecutionManagementModel->getJobnetSummaryForSuper($fromTime, $toTime, $startFromTime, $startToTime);
            } else {
                if ($_SESSION['userInfo']['hasUserGroup']) {
                    $jobnetList = $this->jobExecutionManagementModel->getJobnetSummaryForAll($fromTime, $toTime, $startFromTime, $startToTime, $_SESSION['userInfo']['userId']);
                } else {
                    $jobnetList = $this->jobExecutionManagementModel->getJobnetSummaryForNotBelongGroup($fromTime, $toTime, $startFromTime, $startToTime);
                }
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $jobnetList]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get all operation list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves the error operation list
     *
     * @since   Method available since version 6.1.0
     */
    public function getErrorOperationList()
    {
        $this->logger->info('Get error operation list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $jobnetErrList = null;
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $jobnetErrList = $this->jobExecutionManagementModel->getJobnetErrSummaryForSuper();
            } else {
                $jobnetErrList = $this->jobExecutionManagementModel->getJobnetErrSummaryForAll($_SESSION['userInfo']['userId']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $jobnetErrList]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get error operation list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves the during operation list
     *
     * @since   Method available since version 6.1.0
     */
    public function getDuringOperationList()
    {
        $this->logger->info('Get during operation list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $jobnetRunningList = null;
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $jobnetRunningList = $this->jobExecutionManagementModel->getJobnetRunningSummaryForSuper();
            } else {
                $jobnetRunningList = $this->jobExecutionManagementModel->getJobnetRunningSummaryForAll($_SESSION['userInfo']['userId']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $jobnetRunningList]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get during operation list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that stops execution of the all jobnet summary.
     *
     * @since   Method available since version 6.1.0
     */
    public function setAllJobnetSummaryStop()
    {
        $this->logger->info('Stop all jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'status' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                if ($params["status"] == Constants::RUN_JOB_STATUS_DURING) {

                    $updateData = [
                        "jobnet_abort_flag" => Constants::ABORT_FLAG_FORCE_STOP
                    ];
                    $where = "inner_jobnet_id = " . $params["innerJobnetId"] . " and status=" . Constants::RUN_JOB_STATUS_TYPE_DURING;

                    if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($updateData, $where)) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }
                } else {

                    $now = date("Ymdhis");
                    $updateData = [
                        "status" => Constants::RUN_JOB_STATUS_TYPE_ABNORMAL,
                        "start_time" => $now,
                        "end_time" => $now
                    ];
                    $where = "inner_jobnet_id = " . $params["innerJobnetId"] . " and status=" . Constants::RUN_JOB_STATUS_TYPE_NONE;

                    if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($updateData, $where)) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }
                }
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Stop all jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that stops execution of the error jobnet summary.
     *
     * @since   Method available since version 6.1.0
     */
    public function setErrorJobnetSummaryStop()
    {
        $this->logger->info('Stop error jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetIdList' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                foreach ($params["innerJobnetIdList"] as $innerJobnetId) {
                    $updateData = [
                        "jobnet_abort_flag" => Constants::ABORT_FLAG_FORCE_STOP
                    ];
                    $where = "inner_jobnet_id = " . $innerJobnetId . " and status=" . Constants::RUN_JOB_STATUS_TYPE_DURING;
                    $updateRunJobMethod = $this->jobExecutionManagementModel->updateRunJobnetSummary($updateData, $where);
                    if (!$updateRunJobMethod) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }
                }
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Stop error jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that stops execution of the during jobnet summary.
     *
     * @since   Method available since version 6.1.0
     */
    public function setDuringJobnetSummaryStop()
    {
        $this->logger->info('Stop during jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                $updateData = [
                    "jobnet_abort_flag" => Constants::ABORT_FLAG_FORCE_STOP
                ];
                $where = "inner_jobnet_id = " . $params["innerJobnetId"] . " and status=" . Constants::RUN_JOB_STATUS_TYPE_DURING;
                $updateRunJobMethod = $this->jobExecutionManagementModel->updateRunJobnetSummary($updateData, $where);
                if (!$updateRunJobMethod) {
                    throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                }
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Stop during jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that holds execution of the jobnet summary.
     *
     * @since   Method available since version 6.1.0
     */
    public function setHoldJobnetSummary()
    {
        $this->logger->info('Hold jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                $runJobnetSummaryData = [
                    "start_pending_flag" => Constants::START_PENDING_FLAG_PENDING,
                ];
                $runJobnetSummaryWhere = "inner_jobnet_id = " . $params["innerJobnetId"] . " and status = " . Constants::RUN_JOB_STATUS_TYPE_NONE;

                if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($runJobnetSummaryData, $runJobnetSummaryWhere)) {
                    throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                }
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Hold jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that releases the hold jobnet summary.
     *
     * @since   Method available since version 6.1.0
     */
    public function setReleaseJobnetSummary()
    {
        $this->logger->info('Release jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $this->beginTransaction();
                $runJobnetSummaryData = [
                    "start_pending_flag" => Constants::START_PENDING_RELEASE,
                ];
                $runJobnetSummaryWhere = "inner_jobnet_id = " . $params["innerJobnetId"] . "  and start_pending_flag=" . Constants::START_PENDING_FLAG_PENDING;

                if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($runJobnetSummaryData, $runJobnetSummaryWhere)) {
                    throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                }
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Release jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that check the schedule of the jobnet summary is valid or not
     *
     * @since   Method available since version 6.1.0
     */
    public function checkIsScheduleValid()
    {
        $this->logger->info('Check schedule jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'scheduleId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                if (!$this->jobExecutionManagementModel->getScheduleValid($params["scheduleId"])) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_VALID]);
                } else {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INVALID]);
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Check schedule jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that deletes the schedule jobnet summary
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteScheduleJobnetSummary()
    {
        $this->logger->info('Delete schedule jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'scheduleId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {

                if (!$this->jobExecutionManagementModel->getScheduleValid($params["scheduleId"])) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $params["scheduleId"] . " schedule is enabled. Please delete the schedule after disabled it."]);
                } else {
                    $this->beginTransaction();
                    if (!$this->jobExecutionManagementModel->deleteScheduleJobnet($params["innerJobnetId"])) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "Jobnet will run within 5 minutes. Verify that it has been executed.", Constants::API_RESPONSE_MESSAGE_CODE => "err-msg-common-030"]);
                    }else{
                        $this->commit();
                        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
                    }
                }
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {   
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Delete schedule jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that updates the schedule jobnet summary
     *
     * @since   Method available since version 6.1.0
     */
    public function updateScheduleJobnetSummary()
    {
        $this->logger->info('Update schedule jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'scheduleTime' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $now = date("YmdHm", strtotime($this->util->getDate()));
                if (!preg_match("/^[0-9]{12}$/", $params["scheduleTime"])) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => "Schedule time is invalid format."]);
                    $this->logger->info("Schedule time is invalid format.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } elseif ($params["scheduleTime"] <= $now) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => "Schedule time must not be past time."]);
                    $this->logger->info("Schedule time must not be past time.", ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $this->beginTransaction();

                    $time = $params["scheduleTime"];
                    $runJobnetSummaryData = [
                        "scheduled_time" => $time,
                    ];
                    $runJobnetSummaryWhere = "inner_jobnet_id = " . $params["innerJobnetId"] . " and status = " . Constants::RUN_JOB_STATUS_TYPE_NONE;
                    if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($runJobnetSummaryData, $runJobnetSummaryWhere)) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }

                    $runJobnetData = [
                        "scheduled_time" => $time,
                    ];
                    $runJobnetWhere = "inner_jobnet_id = " . $params["innerJobnetId"] . "  and status = " . Constants::RUN_JOB_STATUS_TYPE_NONE;
                    if (!$this->runJobnetModel->updateRunJobnetMethod($runJobnetData, $runJobnetWhere)) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
                    }
                    $this->commit();
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
                }
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Update schedule jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that delays the jobnet summary
     *
     * @since   Method available since version 6.1.0
     */
    public function delayJobnetSummary()
    {
        $this->logger->info('Delay jobnet summary process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'innerJobnetId' => 'required',
                'loadStatus' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                if ($params["loadStatus"] == Constants::LOAD_STATUS_DELAY) {
                    $this->beginTransaction();

                    $this->setJaRunJobnetTableStatus($params["innerJobnetId"]);
                    $this->setJaRunJobnetSummaryTableStatus($params["innerJobnetId"]);
                    $this->setJaRunJobTableStatus($params["innerJobnetId"]);

                    $this->commit();
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS]);
                }
            }
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Delay jobnet summary process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It updates run jobnet status to delay.
     *
     * @param   int $innerJobnetId
     * @throws  PDOException
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function setJaRunJobnetTableStatus($innerJobnetId)
    {
        $this->logger->debug('Set run jobnet table status delay process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            // $jobnetLoadSpan = $this->runJobnetModel->getParameterData(Constants::JOBNET_LOAD_SPAN);
            $runJobnetData = [
                "status" => Constants::RUN_JOB_STATUS_TYPE_NONE
            ];
            $runJobnetWhere = "inner_jobnet_id = $innerJobnetId";

            if (!$this->runJobnetModel->updateRunJobnetMethod($runJobnetData, $runJobnetWhere)) {
                throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
            }

            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->debug('Set run jobnet table status process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It updates run jobnet summary status to dalay.
     *
     * @param   int $innerJobnetId
     * @throws  PDOException
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function setJaRunJobnetSummaryTableStatus($innerJobnetId)
    {
        $this->logger->debug('Set run jobnet summary table status to delay process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $runJobnetSummaryData = [
                "status" => Constants::RUN_JOB_STATUS_TYPE_PREPARE,
                "job_status" => Constants::RUN_JOB_OPERATION_STATUS_NORMAL,
                "load_status" => Constants::LOAD_STATUS_TYPE_NONE
            ];
            $runJobnetSummaryWhere = "inner_jobnet_id = $innerJobnetId";

            if (!$this->jobExecutionManagementModel->updateRunJobnetSummary($runJobnetSummaryData, $runJobnetSummaryWhere)) {
                throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
            }

            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->debug('Set run jobnet summary table status to delay process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It updates run job status to dalay.
     *
     * @param   string $innerJobnetId
     * @throws  PDOException
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function setJaRunJobTableStatus($innerJobnetId)
    {
        $this->logger->debug('Set run job table status to delay process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $runJobData = [
                "status" => Constants::RUN_JOB_STATUS_TYPE_NONE
            ];
            $runJobWhere = "inner_jobnet_id = $innerJobnetId and job_type = " . Constants::ICON_TYPE_START;

            if (!$this->runJobnetModel->updateRunJobMethod($runJobData, $runJobWhere)) {
                throw new PDOException(Constants::SERVICE_MESSAGE_FAIL);
            }

            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->debug('Set run job table status to delay process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
