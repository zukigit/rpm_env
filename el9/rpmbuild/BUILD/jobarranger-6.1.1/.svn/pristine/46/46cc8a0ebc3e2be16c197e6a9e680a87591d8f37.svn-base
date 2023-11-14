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
use App\Services\ObjectLockService;
use App\Services\ObjectDetailService;
use App\Models\JobnetModel;
use App\Models\UserModel;
use App\Models\IndexModel;
use App\Models\HostModel;
use App\Models\DefineValueJobControlModel;
use App\Models\DefineExtendedJobModel;
use App\Models\FlowModel;
use App\Models\RunJobnetModel;
use App\Exceptions\JobnetNotExecutableException;
use App\Models\CalendarModel;
use App\Models\ObjectLockModel;
use App\Utils\Controller;
use App\Utils\Core;
use App\Utils\Constants;
use App\Utils\Util;
use Exception;
use PDOException;

/**
 * This controller is used to manage the jobnet.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Jobnet extends Controller
{

    public function __construct()
    {
        parent::__construct();
        $this->objectLockService = new ObjectLockService();
        $this->objectDetailService = new ObjectDetailService();
        $this->jobnetModel = new JobnetModel();
        $this->calendarModel = new CalendarModel();
        $this->userModel = new UserModel();
        $this->indexModel = new IndexModel();
        $this->hostModel = new HostModel();
        $this->defineValueJobControlModel = new DefineValueJobControlModel();
        $this->defineExtendedJobModel = new DefineExtendedJobModel();
        $this->objectLockModel = new ObjectLockModel();
        $this->flowModel = new FlowModel();
        $this->runJobnetModel = new RunJobnetModel();
        $this->objectLockService->changeServiceLevel(Constants::SUB_SERVICE);
        $this->objectDetailService->changeServiceLevel(Constants::SUB_SERVICE);
        $this->util = new Util();
        $this->logger = Core::logger();
        $this->validator = new Validator();
    }

    /**
     * api endpoint that retrieves init data to create jobnet.
     *
     * @since   Method available since version 6.1.0
     */
    public function initCreate(): void
    {
        $this->logger->info('Jobnet create initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];
            $validation = $this->validator->validate($params, [
                'type' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $lastid = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_JOBNET);
                $response = [
                    'lastid'  => $lastid->nextid,
                    'type' => 'create',
                    'editable' => 1
                ];
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $response]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Jobnet create initialization process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves init data for edit/new object/new version.
     *
     * @since   Method available since version 6.1.0
     */
    public function initEdit(): void
    {
        $this->logger->info('Jobnet edit initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];
            $validator = $this->validator;
            $validation = $this->validator->validate($params, [
                'type' => [
                    'required',
                    $validator('in', [Constants::OBJECT_FORM_CREATE, Constants::OBJECT_FORM_EDIT, Constants::OBJECT_FORM_NEW_OBJECT, Constants::OBJECT_FORM_NEW_VERSION, Constants::OBJECT_TYPE_SCHEDULE_STRING])
                ],
                'id' => 'required',
                'date' => 'required',
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $id = $params['id'];
                $updateDate = $params['date'];
                $detail = null;
                if ($params['type'] == Constants::OBJECT_TYPE_SCHEDULE_STRING) {
                    $detail = $this->jobnetModel->GetValidORMaxUpdateDateJobnetById($params['id']);
                    $updateDate = $detail->update_date;
                } else {
                    $detail = $this->objectDetailService->getSingleObject($id, Constants::OBJECT_TYPE_JOBNET, $updateDate);
                }

                if ($detail == Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND) {
                    $this->logger->info("Editing data is not found", ['controller' => __METHOD__]);
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $this->util->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND));
                    return;
                }
                $graphData = $this->graphData($id, $updateDate);
                if ($params["type"] == Constants::OBJECT_FORM_NEW_OBJECT) {
                    $editable = 1;
                    $authrority = true;
                    $lastid = $this->indexModel->getNextIdAndIncrease(Constants::COUNT_ID_JOBNET);
                    if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                        $authrority = false;
                        $editable = 0;
                    }

                    $response = [
                        'lastid'  => $lastid->nextid,
                        'urlid' => $id,
                        'type' => $params["type"],
                        'detail'  => $detail,
                        'editable' => $editable,
                        'authority' => $authrority,
                        'isLocked' => 0,
                    ];
                } elseif ($params["type"] == Constants::OBJECT_FORM_EDIT || $params["type"] == Constants::OBJECT_FORM_NEW_VERSION || $params["type"] == Constants::OBJECT_FORM_NEW_VERSION || $params["type"] == Constants::OBJECT_FORM_SCHEDULE) {

                    $editable = $this->objectLockService->isEditable($detail, Constants::OBJECT_TYPE_JOBNET, ($params["type"] == Constants::OBJECT_FORM_NEW_VERSION)) == Constants::SERVICE_MESSAGE_EDITABLE ? 1 : 0;

                    $authrority = $this->checkValid($detail, Constants::OBJ_VALID_PROC) ? true : false;
                    $isLocked = 0;
                    $isLock = $this->objectLockService->process((object) ["objectId" => $id, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                    if ($isLock != Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK  && $params["type"] != Constants::OBJECT_FORM_NEW_OBJECT) { //&& $isLock != Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME
                        $isLocked = 1;
                    }

                    $response = [
                        'detail'  => $detail,
                        'type' => $params["type"],
                        'editable' => $editable,
                        'isLocked' => $isLocked,
                        'authority' => $authrority,
                    ];
                }

                $response = array_merge($response, $graphData);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $response]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Jobnet edit initialization process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves init data for sub jobnet
     *
     * @since   Method available since version 6.1.0
     */
    public function initSubJobnet(): void
    {
        $this->logger->info('Sub jobnet initialization process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
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

                $id = $params['id'];
                $detail = $this->getValidORMaxUpdateDateEntityById($id);
                if ($detail == false) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND]);
                    $this->logger->info(Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND, ['controller' => __METHOD__]);
                    return;
                }

                $graphData = $this->graphData($id, $detail->update_date);

                $editable = $this->objectLockService->isEditable($detail, Constants::OBJECT_TYPE_JOBNET) == Constants::SERVICE_MESSAGE_EDITABLE ? 1 : 0;

                $locked = $this->objectLockService->process((object) ["objectId" => $id, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);

                $isLocked = 0;
                if ($locked != Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME && $locked != Constants::SERVICE_MESSAGE_OBJ_NOT_LOCK) {
                    $isLocked = 1;
                }

                $authrority = $this->checkValid($detail, Constants::OBJ_VALID_PROC) ? true : false;

                $response = [
                    'detail'  => $detail,
                    'type' => Constants::OBJECT_FORM_EDIT,
                    'editable' => $editable,
                    'authority' => $authrority,
                    'isLocked' => $isLocked
                ];

                $response = array_merge($response, $graphData);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $response]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Sub jobnet initialization process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the object is valid or not.
     *
     * @return  bool|string could be an bool if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    private function checkValid(object $detail, string $action)
    {
        try {
            $valid = $this->objectLockService->isValid($detail, $action);
            return $valid;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->info('Check jobnet valid process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It redirects to object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function version()
    {
        $this->logger->info('Select jobnet version process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        if (!isset($_GET['id'])) {
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "");
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        try {
            $detail = $this->jobnetModel->detail($_GET['id'], null);
            $editable = 0;
            if (sizeof($detail) <= 0) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            }
            if ($this->objectLockService->isEditable($detail[0], Constants::OBJECT_TYPE_JOBNET, true) == Constants::SERVICE_MESSAGE_EDITABLE) {
                $editable = 1;
            }

            $data = [
                'datas'  => $detail,
                'page_type' => "jobnet",
                'edit' => $editable
            ];
            if (!is_array($data)) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, Constants::API_RESPONSE_NOT_FOUND);
                exit;
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $data);
            }
        } catch (PDOException $e) {
            $responseData = $this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR,  $_GET['id']);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, Constants::API_RESPONSE_NOT_FOUND);
        }
        $this->logger->info('Select jobnet version process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves available host
     *
     * @since   Method available since version 6.1.0
     */
    public function getAvailableHosts()
    {
        $this->logger->info('Get host data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $hosts = $this->hostModel->getHostDataSuper();
            } else {
                $hosts = $this->hostModel->getHostData($_SESSION['userInfo']['userName']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $hosts]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get host data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves define value job control
     *
     * @since   Method available since version 6.1.0
     */
    public function getDefineValueJobControl()
    {
        $this->logger->info('Get define value job control process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $defineValueJobControl = $this->defineValueJobControlModel->getDefineValueJobControlVariable();
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $defineValueJobControl]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get define value job control process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves define extended job
     *
     * @since   Method available since version 6.1.0
     */
    public function getDefineExtendedJob()
    {
        $this->logger->info('Get define extended job data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $defineExtendJob = $this->defineExtendedJobModel->getDefineExtendedJob($_SESSION['userInfo']['userLangFull']);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $defineExtendJob]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Get define extended job data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves jobnet list for task icon
     *
     * @since   Method available since version 6.1.0
     */
    public function getJobnetList()
    {
        $this->logger->info('Get jobnet list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $jobnetList = $this->jobnetModel->getInfoByUserIdSuper();
            } else {
                $jobnetList = $this->jobnetModel->getInfoByUserId($_SESSION['userInfo']['userId']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $jobnetList]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Get jobnet list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves host group
     *
     * @since   Method available since version 6.1.0
     */
    public function getHostGroup()
    {
        $this->logger->info('Get host group process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $hostGroup = $this->hostModel->getHostGroup();
            } else {
                $hostGroup = $this->hostModel->getHostGroupByUserName($_SESSION['userInfo']['userName']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $hostGroup]);
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Get host group process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves the valid object or maximum updated date object
     *
     * @throws  PDOException
     * @return  object | string
     * @since   Method available since version 6.1.0
     */
    private function getValidORMaxUpdateDateEntityById(string $jobnetId)
    {
        try {
            $data = $this->jobnetModel->getValidEntityById($jobnetId);
            if ($data) {
                return $data;
            }
            $data = $this->jobnetModel->GetMaxUpdateDateEntityById($jobnetId);
            return $data;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
        $this->logger->info('Get valid jobnet or max updated date jobnet process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It retrieves the data for jobnet graph of opened jobnet.
     *
     * @throws  PDOException
     * @return  array | string
     * @since   Method available since version 6.1.0
     */
    private function graphData(string $id, string $updateDate)
    {
        try {
            $jobControlData = $this->jobnetModel->getJobControlData($id, $updateDate);
            $flowData = $this->jobnetModel->getFlowData($id, $updateDate);
            $iconData = array();
            foreach ($jobControlData as $key => $job) {
                $icon = null;
                switch ($job->job_type) {
                    case Constants::ICON_TYPE_END:
                        $endIconSetting = $this->jobnetModel->getEndIconData($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $endIconSetting ? $endIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_CONDITIONAL_START:
                        $startIconSetting = $this->jobnetModel->getConditionalBranchIconData($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $startIconSetting ? $startIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_JOB_CONTROL_VARIABLE:
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $this->jobnetModel->getJobControlValueIconData($id, $job->job_id, $updateDate)
                        ];
                        break;

                    case Constants::ICON_TYPE_JOB:
                        $jobIconSetting = $this->jobnetModel->getJobIconData($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $jobIconSetting ? $jobIconSetting[0] : null,
                            'valueJob' => $this->jobnetModel->getValueJobData($id, $job->job_id, $updateDate),
                            'valueJobControl' => $this->jobnetModel->getValueJobControlData($id, $job->job_id, $updateDate),
                            'jobCommand' => $this->jobnetModel->getJobCommandData($id, $job->job_id, $updateDate)
                        ];

                        if (!$this->util->IsNullOrEmptyString($icon['jobData']->run_user_password)) {
                            $icon['jobData']->run_user_password = $this->util->getStringFromPass($icon['jobData']->run_user_password);
                        }

                        break;

                    case Constants::ICON_TYPE_JOBNET:
                        $jobnetIconSetting = $this->jobnetModel->getJobnetIconData($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $jobnetIconSetting ? $jobnetIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_EXTENDED_JOB:
                        $extendedIconSetting = $this->jobnetModel->getExtendedIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $extendedIconSetting ? $extendedIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_CALCULATION:
                        $calIconSetting = $this->jobnetModel->getCalculationIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $calIconSetting ? $calIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_TASK:
                        $taskIconSetting = $this->jobnetModel->getTaskIcon($id, $job->job_id, $updateDate);
                        $jobnetInfo = $this->getValidORMaxUpdateDateEntityById($taskIconSetting[0]->submit_jobnet_id);
                        $taskIconSetting[0]->jobnet_name = $jobnetInfo->jobnet_name;
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $taskIconSetting ? $taskIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_INFO:
                        $infoIconSetting = $this->jobnetModel->getInfoIcon($id, $job->job_id, $updateDate);
                        if (count($infoIconSetting) > 0) {
                            $infoIconSetting = json_decode(json_encode($infoIconSetting[0]), true);
                            if ($infoIconSetting["info_flag"] == 0) {
                                $infoIconSetting["calendar_name"] = null;
                            } else {
                                $calendarData = $this->calendarModel->getValidORMaxUpdateDateEntityById($infoIconSetting["get_calendar_id"]);
                                if($calendarData != false){
                                    $infoIconSetting["calendar_name"] = $calendarData->calendar_name;
                                }
                            }
                            $icon = [
                                'jobData' => $job,
                                'iconSetting' => $infoIconSetting ? $infoIconSetting : null
                            ];
                        } else {
                            $icon = [
                                'jobData' => $job,
                                'iconSetting' => null
                            ];
                        }
                        break;

                    case Constants::ICON_TYPE_FILE_COPY:
                        $fileCopyIconSetting = $this->jobnetModel->getFileTransferIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $fileCopyIconSetting ? $fileCopyIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_FILE_WAIT:
                        $fileWaitIconSetting = $this->jobnetModel->getFileWaitIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $fileWaitIconSetting ? $fileWaitIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_REBOOT:
                        $rebootIconSetting = $this->jobnetModel->getRebootIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $rebootIconSetting ? $rebootIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_RELEASE:
                        $releaseIconSetting = $this->jobnetModel->getReleaseHoldIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $releaseIconSetting ? $releaseIconSetting[0] : null
                        ];
                        break;

                    case Constants::ICON_TYPE_AGENT_LESS:
                        $agentLessIconSetting = $this->jobnetModel->getAgentLessIcon($id, $job->job_id, $updateDate);
                        if (count($agentLessIconSetting) > 0) {
                            $icon = [
                                'jobData' => $job,
                                'iconSetting' => $agentLessIconSetting ? $agentLessIconSetting[0] : null
                            ];

                            if (!$this->util->IsNullOrEmptyString($icon['iconSetting']->login_password)) {
                                $loginPW = $this->util->getStringFromPass($icon['iconSetting']->login_password);
                                if(strpos($loginPW, '|') != false){
                                    $strPass = explode("|", $loginPW);
                                    $loginPW = $strPass[1];
                                }
                                $icon['iconSetting']->login_password = $loginPW;
                            }
                        } else {
                            $icon = [
                                'jobData' => $job,
                                'iconSetting' => null
                            ];
                        }

                        break;

                    case Constants::ICON_TYPE_ZABBIX:
                        $zabbixIconSetting = $this->jobnetModel->getzabbixIcon($id, $job->job_id, $updateDate);
                        $icon = [
                            'jobData' => $job,
                            'iconSetting' => $zabbixIconSetting ? $zabbixIconSetting[0] : null
                        ];
                        break;

                    default:
                        $icon = [
                            'jobData' => $job,
                        ];
                }
                array_push($iconData, $icon);
            }
            $data = [
                'jobs'  => $iconData,
                'flows' => $flowData
            ];
            $this->logger->info('Get graph data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage());
        }
    }


    /**
     * api endpoint that retrieves host names
     *
     * @since   Method available since version 6.1.0
     */
    public function selectHostName()
    {
        $this->logger->info('Select host names process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $json = file_get_contents('php://input');
        $params = Util::jsonDecode($json)["params"];
        $validation = $this->validator->validate($params, [
            'groupid' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            // get host data by zabbix api
            // $method = "host.get";
            // if ($params['groupid'] == 0) {
            //     $param = [
            //         'output' => ["host"]
            //     ];
            // } else {
            //     $param = [
            //         'output' => ["host"],
            //         'groupids' => $params['groupid']
            //     ];
            // }
            // $result = json_decode(ZabbixApi::RequestApi($method, $param));
            // if (isset($result->error)) {
            //     if ($result->error->data == Constants::ZABBIX_SESSION_EXPIRED_ERROR) {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "[Zabbix] " . Constants::ZABBIX_SESSION_EXPIRED_ERROR]);
            //     } else {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => $result->error->data]);
            //     }
            // } else {
            //     echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result->result]);
            // }

            if ($params['groupid'] == 0) {
                $hostGroup = $this->hostModel->getHostGroup();
                $hostGroupCount = count($hostGroup);
                $hostNameResult = [];
                $tmpHostArray = [];
                for ($i = 0; $i < $hostGroupCount; $i++) {
                    $hostNameData = $this->getHostName($hostGroup[$i]->groupid);
                    $hostNameCount = count($hostNameData);
                    if ($hostNameCount > 0) {
                        for ($j = 0; $j < $hostNameCount; $j++) {
                            if(!in_array($hostNameData[$j]->host, $tmpHostArray)){
                                array_push($hostNameResult, $hostNameData[$j]);
                                array_push($tmpHostArray, $hostNameData[$j]->host);
                            }

                        }
                    }
                }
            } else {
                $hostNameResult = $this->getHostName($params['groupid']);
            }
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $hostNameResult]);
        }
        $this->logger->info('Select host name process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It changes the valid flag of the object_version
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    private function getHostName($groupdId)
    {

        try {
            if ($_SESSION['userInfo']['userType'] == Constants::USER_TYPE_SUPER) {
                $hostGroup = $this->hostModel->getHostNameByGroupIdForSuper($groupdId);
            } else {
                $hostGroup = $this->hostModel->getHostNameByGroupId($groupdId, $_SESSION['userInfo']['userName']);
            }
            return $hostGroup;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
        $this->logger->info('jobnet version Disable process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves items
     *
     * @since   Method available since version 6.1.0
     */
    public function selectItem()
    {
        $this->logger->info('Select Items process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $json = file_get_contents('php://input');
        $params = Util::jsonDecode($json)["params"];
        $validation = $this->validator->validate($params, [
            'hostid' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            // $method = "item.get";
            // $param = [
            //     'output' => ["itemid", "name"],
            //     "sortfield" => "itemid",
            //     'hostids' => $params['hostid']
            // ];

            // $result = json_decode(ZabbixApi::RequestApi($method, $param));

            // if (isset($result->error)) {
            //     if ($result->error->data == Constants::ZABBIX_SESSION_EXPIRED_ERROR) {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "[Zabbix] " . Constants::ZABBIX_SESSION_EXPIRED_ERROR]);
            //     } else {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => $result->error->data]);
            //     }
            // } else {
            //     echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result->result]);
            // }

            $items = $this->jobnetModel->getItem($params['hostid']);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $items]);
        }
        $this->logger->info('Select items process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that retrieves triggers
     *
     * @since   Method available since version 6.1.0
     */
    public function selectTrigger()
    {
        $this->logger->info('Select triggers process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $json = file_get_contents('php://input');
        $params = Util::jsonDecode($json)["params"];
        $validation = $this->validator->validate($params, [
            'hostid' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
            $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
        } else {
            // $method = "trigger.get";
            // $param = [
            //     "output" => ["triggerid", "expression", "description"],
            //     "hostids" => $params['hostid'],
            //     "selectFunctions" => "extend",
            //     "selectItems" => ["key_"],
            //     "sortfield" => "triggerid",
            // ];

            // $result = json_decode(ZabbixApi::RequestApi($method, $param));

            // if (isset($result->error)) {
            //     if ($result->error->data == Constants::ZABBIX_SESSION_EXPIRED_ERROR) {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "[Zabbix] " . Constants::ZABBIX_SESSION_EXPIRED_ERROR]);
            //     } else {
            //         echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => $result->error->data]);
            //     }
            // } else {
            //     echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $result->result]);
            // }

            $triggers = $this->jobnetModel->getTrigger($params['hostid']);
            $triggerCount = count($triggers);
            $newTriggers = $this->setConditionalExpression($triggers);

            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $newTriggers]);
        }
        $this->logger->info('Select triggers process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It set the conditional exprssion for trigger
     *
     * @param   array $triggers     
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    private function setConditionalExpression($triggers)
    {

        $triggerCount = count($triggers);
        for ($i = 0; $i < $triggerCount; $i++) {

            $strExpression = $triggers[$i]->expression;
            $strWkExpression = $strExpression;

            $foundIndex = strpos($strWkExpression, '{');
            while (!is_bool($foundIndex)) {
                $strFunctionid = substr($strWkExpression, $foundIndex, strpos($strWkExpression, "}") - $foundIndex + 1);
                $strWkFunctionid = $strFunctionid;

                $strFuncArr = str_split($strFunctionid, 1);
                if (is_numeric($strFuncArr[1])) {
                    $strWkFunctionid = substr($strWkFunctionid, 1, strlen($strWkFunctionid) - 2);
                    $triggerExpressionResult = $this->jobnetModel->getTriggerExpression($strWkFunctionid);
                    $strHost = $triggerExpressionResult[0]->host;
                    $strKey = $triggerExpressionResult[0]->key_;
                    $strFunction = $triggerExpressionResult[0]->name;
                    $strParameter = $triggerExpressionResult[0]->parameter;

                    $strConditionalExpression = "{" . $strHost . ":" . $strKey . "." . $strFunction . "(" . $strParameter . ")" . "}";

                    $strExpression = str_replace($strFunctionid, $strConditionalExpression, $strExpression);
                }

                $strWkExpression = str_replace($strFunctionid, "AA22", $strWkExpression);
                $foundIndex = strpos($strWkExpression, '{');
            }
            $triggerArr = json_decode(json_encode($triggers[$i]), true);
            $triggerArr["conditonalExpression"] = $triggers[$i]->description . "(" . $strExpression . ")";
            $triggers[$i] = json_decode(json_encode($triggerArr), false);
        }
        $this->logger->info('Set conditional expression process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $triggers;
    }

    /**
     * api endpoint that retrieves available jobnet options to create sub jobnet
     *
     * @since   Method available since version 6.1.0
     */
    public function getJobnetOption()
    {
        $this->logger->info('Get jobnet option process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $jobnetId = isset($params["ignoreJobnetId"]) ? $params["ignoreJobnetId"] : null;
            $publicJobnet = $this->jobnetModel->getData(Constants::PUBLIC_FLAG, "", $jobnetId);
            $privateJobnet = $this->jobnetModel->getData(Constants::PRIVATE_FLAG, "", $jobnetId);
            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => ['public' => $publicJobnet, 'private' => $privateJobnet]]);
        } catch (PDOException $e) {
            echo $this->response->errorInternalError(Constants::SERVICE_INTERNAL_SERVER_ERROR);
        }

        $this->logger->info('Get jobnet option process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that change valid status of the jobnet object version
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidVersion()
    {
        $this->logger->info('Change valid version process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $request = json_decode($json);
        $validationReq = (array)$request;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'updatedDate' => 'required',
            'category' => 'required',
            'validFlag' => 'required',
            'objectId' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $result = $this->changeValidVersionService($request);
        if (!is_array($result)) {
            //resultstring->json
            //get data from json -> error
            //
            $data = $this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, "");
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        } else {
            if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SUCCESS) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $result);
            } else if ($result[Constants::AJAX_MESSAGE_DETAIL] == Constants::DETAIL_SERVER_ERROR) {
                echo Util::response(Constants::API_RESPONSE_TYPE_500, $result);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $result);
            }
        }
        $this->logger->info('Change valid version process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }


    /**
     * It changes the valid flag of the object_version
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    private function changeValidVersionService($para)
    {
        try {
            $this->beginTransaction();
            $objectId = $para->params->objectId;
            $updateDate = $para->params->updatedDate;
            $curValidFlag = $para->params->validFlag;
            $detail = $this->jobnetModel->each($objectId, $updateDate);

            if ($detail == false) {
                $this->rollback();
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_DB_LOCK, $objectId);
                return $data;
            } else {
                if ($this->checkLatest($objectId, $updateDate, $detail) == false) {
                    $this->rollback();
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    return $data;
                }
            }
            $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);

            if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                $this->rollback();
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                $this->rollback();
                return $data;
            } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                $this->rollback();
                return $data;
            } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                return $data;
            }
            //check for related data
            if ($curValidFlag == "1") {
                //disable
                $usingSchedule = $this->jobnetModel->checkJonbetRelatedParentSchedule($objectId, "disable");
                $usingJobnet = $this->jobnetModel->checkJonbetRelatedParentJobnet($objectId, "disable");
                if (count((array)$usingJobnet) > 0 || count((array)$usingSchedule) > 0) {
                    $returnItemData = [
                        'objectId' => $objectId,
                        'scheduleData'   => (array)$usingSchedule,
                        'jobnetData'   => (array)$usingJobnet,
                    ];
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, (array)$returnItemData);
                    return $data;
                }
            } else {
                //enable
                $usingJobnet = $this->jobnetModel->checkJonbetChildForEnable($objectId, $updateDate);
                if (count((array)$usingJobnet) > 0) {
                    $returnItemData = [
                        'objectId' => $objectId,
                        'jobnetData'   => (array)$usingJobnet,
                    ];
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, (array)$returnItemData);
                    return $data;
                }
            }
            if ($curValidFlag == "1") {
                $data = $this->disable($objectId, $updateDate);
            } else {
                $data = $this->enable($objectId, $updateDate);
            }
            if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                $this->rollback();
                return $data;
            }
            $this->commit();
            $this->logger->info('Change valid version service is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        } catch (JobnetNotExecutableException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                return $e->getMessage();
            }
        }
    }


    /**
     * It changes the valid flag of jobnet list.
     *
     * @since   Method available since version 6.1.0
     */
    public function changeValidList()
    {
        $this->logger->info('Change valid list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $req_raw = file_get_contents('php://input');
        $para = json_decode($req_raw, true)['params'];
        $result = $this->changeValidListService($para);
        if (!is_array($result)) {
            $result = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, "");
        }
        $this->logger->info('Change valid list process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return $result;
    }
    /**
     * It changes the valid flag of the object_list
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function changeValidListService($para)
    {
        try {
            $selectRows = $para['datas']['selectedRows'];
            $actionType = $para['datas']['actionType'];
            $modelValid = 'enable';
            if ($actionType == "disable") {
                $modelValid = 'disable';
            }
            // $this->logger->info('Multiple jobnet ' . $actionType . ' process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $detail = $this->jobnetModel->each($objectId, $update_date);
                if ($detail == false) {
                    // $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "db-lock", $objectId);
                    // return $data;
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND, $objectId);
                    return $data;
                } else {
                    // if ($this->checkLatest($objectId, $update_date, $detail) == false) {
                    //     $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_RELOAD, "err-msg-not-latest", $objectId);
                    //     return $data;
                    // }
                    if ($this->checkLatest($objectId, $update_date, $detail) == false) {
                        $data = (array)$this->util->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                        return $data;
                    }
                }
                if (!$this->checkValid($detail, Constants::OBJ_VALID_PROC)) {
                    // $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "alt-msg-permit", $objectId);
                    // return $data;
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    // $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "alt-msg-permit", $objectId);
                    // return $data;
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    // $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "alt-msg-lock", $objectId);
                    // return $data;
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    // $data = (array)$this->util->createResponseJson(Constants::AJAX_MESSAGE_INCOMPLETE, "lab-server-error", $objectId);
                    // return $data;
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                }
                //check for related data
                if ($actionType == "disable") {
                    $usingSchedule = $this->jobnetModel->checkJonbetRelatedParentSchedule($objectId, $actionType);
                    $usingJobnet = $this->jobnetModel->checkJonbetRelatedParentJobnet($objectId, $actionType);
                    if (count((array)$usingJobnet) > 0 || count((array)$usingSchedule) > 0) {
                        $returnItemData = [
                            'objectId' => $objectId,
                            'scheduleData'   => (array)$usingSchedule,
                            'jobnetData'   => (array)$usingJobnet,
                        ];
                        $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, (array)$returnItemData);
                        return $data;
                    }
                } else {
                    $usingJobnet = $this->jobnetModel->checkJonbetChildForEnable($objectId, $update_date);
                    if (count((array)$usingJobnet) > 0) {
                        $returnItemData = [
                            'objectId' => $objectId,
                            'jobnetData'   => (array)$usingJobnet,
                        ];
                        $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, (array)$returnItemData);
                        return $data;
                    }
                }
            }
            $this->beginTransaction();
            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $data = $this->$modelValid($objectId, $update_date);
                if ($data[Constants::AJAX_MESSAGE_DETAIL] != Constants::DETAIL_SUCCESS) {
                    $this->rollback();
                    return $data;
                }
            }
            $this->commit();
            $this->logger->info('Multiple jobnet ' . $actionType . ' process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        } catch (JobnetNotExecutableException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
                $this->logger->info('Multiple jobnet ' . $actionType . ' process error:' . $e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            }
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }
    /**
     * It manages the jobnet data such as create, update, new version, new object.
     *
     * @since   Method available since version 6.1.0
     */
    public function save()
    {
        $this->logger->info('Jobnet save process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'jobnetId' => 'required',
                'multiple' => 'required|in:0,1,2',
                'public' => 'required|in:0,1',
                'jobnetName' => 'required',
                'timeoutSec' => 'required',
                'icon' => 'required',
                'flow' => 'required',
                'userName' => 'required',
                'type' => 'required',
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                [$result, $err] = $this->validateIcons($params["icon"]);
                if (!$result) {
                    echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $err]);
                    return;
                }
                $data = json_decode(json_encode($params));
                $updateDate = $this->util->getDate();
                if ($data->type == "create" || $data->type == Constants::OBJECT_FORM_NEW_OBJECT) {
                    $resultMessage = $this->checkID($data->jobnetId);
                    switch ($resultMessage) {
                        case Constants::SERVICE_MESSAGE_RECORD_EXIST:
                            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_RECORD_EXIST]);
                            break;
                        case Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST:
                            $result = $this->manage($data, $updateDate);
                            if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [
                                    Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS,
                                    Constants::API_RESPONSE_DATA => [
                                        'jobnetId' => $data->jobnetId,
                                        'formType' => $data->type,
                                        'publicFlag' => $data->public,
                                        'updateDate' => $updateDate
                                    ]
                                ]);
                            } else {
                                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_FAIL]);
                            }
                            break;
                    }
                } else {

                    //check data exist for update
                    $detail = $this->jobnetModel->each($data->jobnetId, $data->updateDate);
                    if ($detail == false) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST]);
                        return;
                    } else {
                        if ($this->checkLatest($data->jobnetId, $data->updateDate, $detail) == false) {
                            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::MESSAGE[Constants::DETAIL_NOT_LAST_UPDATED], Constants::API_RESPONSE_MESSAGE_CODE => Constants::DETAIL_NOT_LAST_UPDATED]);
                            return;
                        }
                    }

                    $ipAddr = $this->objectLockService->getClientIpAddress();
                    $checkWhere = "object_id = '$data->jobnetId' AND object_type = " . Constants::OBJECT_TYPE_JOBNET . " AND attempt_ip = '$ipAddr'";
                    $checkResult = $this->objectLockModel->checkObjectLock($checkWhere);
                    if (!$checkResult) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_NO_LOCK_SESSION]);
                        return;
                    }

                    $result = $this->manage($data, $updateDate);
                    if ($result == Constants::SERVICE_MESSAGE_SUCCESS) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [
                            Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_SUCCESS,
                            Constants::API_RESPONSE_DATA => [
                                'jobnetId' => $data->jobnetId,
                                'formType' => $data->type,
                                'publicFlag' => $data->public,
                                'updateDate' => $updateDate
                            ]
                        ]);
                    } else {
                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_MESSAGE_OPERATION_FAIL]);
                    }
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        } catch (JobnetNotExecutableException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $e->getMessage()]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->info('Jobnet save process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It proceeds to create/edit/new version/new object the jobnet based on type.
     *
     * @param   object $jobData     jobnet object including icon and flow data.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function validateIcons($icons)
    {
        foreach ($icons as $icon) {
            if (!isset($icon["iconType"])) {
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . "The icon type is required.", ['controller' => __METHOD__]);
                return [false, ["iconType" => "The icon type is required."]];
            }

            $rules = [];
            switch ($icon["iconType"]) {
                case Constants::ICON_TYPE_INFO:
                    $rules = [
                        'methodFlag' => 'required|in:0,1,2,3,4',
                        'x' => 'required',
                        'y' => 'required',
                        'iconSetting' => 'array',
                        'iconSetting.infoFlag' => 'required|in:0,3',
                        'iconSetting.getJobId' => 'required_if:iconSetting.infoFlag,0',
                        'iconSetting.getCalendarId' => 'required_if:iconSetting.infoFlag,3'
                    ];
                    break;

                default:
                    $rules = [
                        'methodFlag' => 'required|in:0,1,2,3,4',
                        'x' => 'required',
                        'y' => 'required',
                    ];
                    break;
            }

            $iconValidation = $this->validator->validate($icon, $rules);

            if ($iconValidation->fails()) {
                $errors = $iconValidation->errors();
                // $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
                return [false, $errors->firstOfAll()];
            }
        }
        $this->logger->info('Validate Icons process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        return [true, null];
    }

    /**
     * It checks the jobnet id is available or not.
     *
     * @param   string $id     id of the jobnet.
     * @since   Method available since version 6.1.0
     */
    private function checkID($id)
    {
        try {
            if ($this->jobnetModel->checkID($id)) {
                $this->logger->debug('[' . $id . '] is already inputted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_RECORD_EXIST;
            } else {
                return Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST;
            }
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage(), $e->getCode());
        }
        $this->logger->debug('Check Jobnet exist process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It proceeds to create/edit/new version/new object the jobnet based on type.
     *
     * @param   object $jobData     jobnet object including icon and flow data.
     * @return  string
     * @since   Method available since version 6.1.0
     */
    private function manage($jobData, $updateDate)
    {
        try {

            $this->beginTransaction();
            $isExecutable = $this->isExecutableJobnet($jobData->icon, $jobData->flow, true, $jobData->jobnetId);
            if (is_array($isExecutable)) {
                throw new JobnetNotExecutableException($isExecutable["message-detail-txt"]);
            }

            if ($jobData->type == Constants::OBJECT_FORM_CREATE || $jobData->type == Constants::OBJECT_FORM_NEW_OBJECT || $jobData->type == Constants::OBJECT_FORM_NEW_VERSION) {
                $jobnetControlData = [
                    'jobnet_id' => $jobData->jobnetId,
                    'update_date' => $updateDate,
                    'valid_flag' =>  0,
                    'public_flag' => $jobData->public,
                    'multiple_start_up' => $jobData->multiple,
                    'user_name' => $jobData->userName,
                    'jobnet_name' => $jobData->jobnetName,
                    'memo' =>  $jobData->description,
                    'jobnet_timeout' => $jobData->timeoutSec == null ? 0 : $jobData->timeoutSec,
                    'timeout_run_type' => $jobData->timeoutType,
                ];

                $this->jobnetModel->insertJobNetControl($jobnetControlData);
            }

            if ($jobData->type == Constants::OBJECT_FORM_EDIT || $jobData->type == Constants::OBJECT_FORM_JOBNET_ICON_EDIT) {

                $this->jobnetModel->deleteJobControl($jobData->jobnetId, $jobData->updateDate);

                $jobnetControlUpdateData = [
                    'urlid' => $jobData->jobnetId,
                    'urldate' => $jobData->updateDate,
                    'jobnet_id' => $jobData->jobnetId,
                    'update_date' => $updateDate,
                    'valid_flag' =>  0,
                    'public_flag' => $jobData->public,
                    'multiple_start_up' => $jobData->multiple,
                    'user_name' => $jobData->userName,
                    'jobnet_name' => $jobData->jobnetName,
                    'memo' =>  $jobData->description,
                    'jobnet_timeout' => $jobData->timeoutSec == null ? 0 : $jobData->timeoutSec,
                    'timeout_run_type' => $jobData->timeoutType,
                ];

                $this->jobnetModel->updateJobnetControl($jobnetControlUpdateData);
            }

            if (count($jobData->icon) > 0) {
                $this->insertIconData($jobData, $updateDate);
            }

            if (count($jobData->flow) > 0) {
                $this->insertFlowData($jobData, $updateDate);
            }
            $this->logger->info('Jobnet ' . $jobData->type . ' process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            $this->commit();
            return Constants::SERVICE_MESSAGE_SUCCESS;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            throw new PDOException($e->getMessage(), $e->getCode());
        } catch (JobnetNotExecutableException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            throw new JobnetNotExecutableException($e->getMessage());
        }
    }

    /**
     * It inserts the icon data.
     *
     * @param   object $jobData     jobnet object including icon and flow data.
     * @param   string $updateDate     updated date of the jobnet.
     * @since   Method available since version 6.1.0
     */
    private function insertIconData($jobData, $updateDate)
    {
        $this->logger->debug('Save icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        foreach ($jobData->icon as $icon) {

            $jobControlData = [
                'jobnet_id' => $jobData->jobnetId,
                'job_id' => $icon->iconSetting->jobId,
                'update_date' => $updateDate,
                'job_type' =>  $icon->iconType,
                'point_x' => intval($icon->x),
                'point_y' => intval($icon->y),
                'job_name' => $icon->iconSetting->jobName ?? null,
                'method_flag' => $icon->methodFlag,
                'force_flag' =>  isset($icon->iconSetting->forceFlag) ? $icon->iconSetting->forceFlag : 0,
                'continue_flag' => isset($icon->iconSetting->continueFlag) ? $icon->iconSetting->continueFlag : 0,
                'run_user' => isset($icon->iconSetting->runUser) ? (!$this->util->IsNullOrEmptyString($icon->iconSetting->runUser) ? $icon->iconSetting->runUser : null) : null,
                'run_user_password' =>  isset($icon->iconSetting->runUserPassword) ? (!$this->util->IsNullOrEmptyString($icon->iconSetting->runUserPassword) ? $this->util->getPasswordFromString($icon->iconSetting->runUserPassword) : null) : null,
            ];

            $this->jobnetModel->insertJobControl($jobControlData);

            switch ($icon->iconType) {
                case Constants::ICON_TYPE_JOB:
                    $this->logger->debug('Save job icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $jobIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'host_flag' =>  $icon->iconSetting->hostFlag,
                        'stop_flag' => $icon->iconSetting->stopFlag,
                        'command_type' => 0,
                        'timeout' => $icon->iconSetting->timeout,
                        'host_name' => $icon->iconSetting->hostName,
                        'stop_code' =>  $icon->iconSetting->stopCode,
                        'timeout_run_type' => $icon->iconSetting->timeoutRunType,
                    ];

                    $this->jobnetModel->insertJobIcon($jobIconData);

                    if (sizeof($icon->iconSetting->valueJob) > 0) {
                        $this->logger->debug('Save value job data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        foreach ($icon->iconSetting->valueJob as $valueJob) {
                            $valueJobData = [
                                'jobnet_id' => $jobData->jobnetId,
                                'job_id' => $icon->iconSetting->jobId,
                                'update_date' => $updateDate,
                                'value_name' =>  $valueJob->valueName,
                                'value' => $valueJob->value,
                            ];

                            $this->jobnetModel->insertValueJob($valueJobData);
                        }
                        $this->logger->debug('Save value job data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    }

                    if (sizeof($icon->iconSetting->valueJobCon) > 0) {
                        $this->logger->debug('Save value job control data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                        foreach ($icon->iconSetting->valueJobCon as $valueJobCon) {
                            $valueJobConData = [
                                'jobnet_id' => $jobData->jobnetId,
                                'job_id' => $icon->iconSetting->jobId,
                                'update_date' => $updateDate,
                                'value_name' =>  $valueJobCon,
                            ];

                            $this->jobnetModel->insertValueJobControl($valueJobConData);
                        }
                        $this->logger->debug('Save value job control data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    }

                    $this->logger->debug('Save job command data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $jobExecCommandData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'command_cls' =>  0,
                        'command' => $icon->iconSetting->exec,
                    ];

                    $this->jobnetModel->insertJobCommand($jobExecCommandData);

                    if ($icon->iconSetting->stopFlag == 1) {
                        $jobStopCommandData = [
                            'jobnet_id' => $jobData->jobnetId,
                            'job_id' => $icon->iconSetting->jobId,
                            'update_date' => $updateDate,
                            'command_cls' =>  2,
                            'command' => $icon->iconSetting->stopCommand,
                        ];

                        $this->jobnetModel->insertJobCommand($jobStopCommandData);
                    }
                    $this->logger->debug('Save job command data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_END:
                    $this->logger->debug('Save end icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $endIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'jobnet_stop_flag' =>  $icon->iconSetting->stopFlag ?? 0,
                        'jobnet_stop_code' => $icon->iconSetting->stopCode ?? 0,
                    ];

                    $this->jobnetModel->insertEndIcon($endIconData);
                    $this->logger->debug('Save exit icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_CONDITIONAL_START:
                    $this->logger->debug('Save conditional branch icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $ifIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'hand_flag' =>  $icon->iconSetting->handFlag,
                        'value_name' => $icon->iconSetting->variable,
                        'comparison_value' => $icon->iconSetting->comparisonValue,
                    ];

                    $this->jobnetModel->insertIfIcon($ifIconData);
                    $this->logger->debug('Save conditional branch icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_JOB_CONTROL_VARIABLE:

                    $this->logger->debug('Save job control variable icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    if (sizeof($icon->iconSetting->valueJob) > 0) {
                        foreach ($icon->iconSetting->valueJob as $valueJob) {
                            $jobConVarIconData = [
                                'jobnet_id' => $jobData->jobnetId,
                                'job_id' => $icon->iconSetting->jobId,
                                'update_date' => $updateDate,
                                'value_name' =>  $valueJob->valueName,
                                'value' => $valueJob->value,
                            ];

                            $this->jobnetModel->insertValueIcon($jobConVarIconData);
                        }
                        $this->logger->debug('Save job control variable icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    }
                    break;

                case Constants::ICON_TYPE_EXTENDED_JOB:
                    $this->logger->debug('Save extended job icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $extendedIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'command_id' =>  $icon->iconSetting->commandId,
                        'value' => $icon->iconSetting->parameter,
                    ];

                    $this->jobnetModel->insertExtendedIcon($extendedIconData);
                    $this->logger->debug('Save extended job icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;


                case Constants::ICON_TYPE_CALCULATION:
                    $this->logger->debug('Save calculation icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $calculationIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'hand_flag' =>  $icon->iconSetting->handFlag,
                        'formula' => $icon->iconSetting->formula,
                        'value_name' => $icon->iconSetting->valueName,
                    ];

                    $this->jobnetModel->insertCalculationIcon($calculationIconData);
                    $this->logger->debug('Save calculation icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_TASK:

                    $this->logger->debug('Save task icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $checkJobnet = $this->checkID($icon->iconSetting->taskJobnetId);
                    if ($checkJobnet === Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST) {
                        throw new PDOException(Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST . ":" . $icon->iconSetting->taskJobnetId);
                    }
                    $taskIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'submit_jobnet_id' =>  $icon->iconSetting->taskJobnetId,
                    ];

                    $this->jobnetModel->insertTaskIcon($taskIconData);
                    $this->logger->debug('Save task icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_INFO:

                    $this->logger->debug('Save info icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $infoIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'info_flag' =>  $icon->iconSetting->infoFlag,
                        'item_id' =>  null,
                        'trigger_id' =>  null,
                        'host_group' => null,
                        'host_name' =>  null,
                        'get_job_id' =>  $icon->iconSetting->getJobId,
                        'get_calendar_id' =>  $icon->iconSetting->getCalendarId,
                    ];

                    $this->jobnetModel->insertInfoIcon($infoIconData);

                    $this->logger->debug('Save info icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_FILE_COPY:

                    $this->logger->debug('Save file transfer icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $fileTransferIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'from_host_flag' =>  $icon->iconSetting->fromHostFlag,
                        'to_host_flag' =>  $icon->iconSetting->toHostFlag,
                        'overwrite_flag' =>  $icon->iconSetting->overwriteFlag,
                        'from_host_name' => $icon->iconSetting->fromHostName,
                        'from_directory' =>  $icon->iconSetting->fromDirectory,
                        'from_file_name' =>  $icon->iconSetting->fromFileName,
                        'to_host_name' =>  $icon->iconSetting->toHostName,
                        'to_directory' =>  $icon->iconSetting->toDirectory,
                    ];

                    $this->jobnetModel->insertFileTransferIcon($fileTransferIconData);

                    $this->logger->debug('Save file transfer icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    break;

                case Constants::ICON_TYPE_FILE_WAIT:

                    $this->logger->debug('Save file wait icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $fileWaitIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'host_flag' =>  $icon->iconSetting->hostFlag,
                        'fwait_mode_flag' =>  $icon->iconSetting->fwaitModeFlag,
                        'file_delete_flag' =>  $icon->iconSetting->fileDeleteFlag,
                        'file_wait_time' => $icon->iconSetting->fileWaitTime,
                        'host_name' =>  $icon->iconSetting->hostName,
                        'file_name' =>  $icon->iconSetting->fileName,
                    ];

                    $this->jobnetModel->insertFileWaitIcon($fileWaitIconData);
                    $this->logger->debug('Save file wait icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_REBOOT:

                    $this->logger->debug('Save reboot icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $rebootIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'host_flag' =>  $icon->iconSetting->hostFlag,
                        'reboot_mode_flag' =>  $icon->iconSetting->rebootModeFlag,
                        'reboot_wait_time' => $icon->iconSetting->rebootWaitTime,
                        'host_name' =>  $icon->iconSetting->hostName,
                        'timeout' =>  $icon->iconSetting->timeout,
                    ];

                    $this->jobnetModel->insertRebootIcon($rebootIconData);
                    $this->logger->debug('Save reboot icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_RELEASE:
                    $this->logger->debug('Save release hold icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

                    $releaseIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'release_job_id' =>  $icon->iconSetting->releaseUnholdJobId,
                    ];

                    $this->jobnetModel->insertReleaseIcon($releaseIconData);
                    $this->logger->debug('Save release hold icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_AGENT_LESS:
                    $this->logger->debug('Save agentless icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $loginPW = null;
                    if(isset($icon->iconSetting->loginPassword)){
                        if(!$this->util->IsNullOrEmptyString($icon->iconSetting->loginPassword)){
                            $passLen = strlen($icon->iconSetting->loginPassword) . "|";
                            $loginPW = $this->util->getPasswordFromString($passLen . $icon->iconSetting->loginPassword);
                        }
                    }
                     
                    $agentLessIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'host_flag' =>  $icon->iconSetting->hostFlag,
                        'connection_method' =>  $icon->iconSetting->connectionMethod,
                        'session_flag' =>  $icon->iconSetting->sessionFlag,
                        'auth_method' =>  $icon->iconSetting->authMethod,
                        'run_mode' =>  $icon->iconSetting->runMode,
                        'line_feed_code' =>  $icon->iconSetting->lineFeedCode,
                        'timeout' =>  $icon->iconSetting->timeout,
                        'session_id' =>  $icon->iconSetting->sessionId,
                        'login_user' =>  $icon->iconSetting->loginUser,
                        'login_password' =>  $loginPW,
                        'public_key' =>  $icon->iconSetting->publicKey,
                        'private_key' =>  $icon->iconSetting->privateKey,
                        'passphrase' =>  $icon->iconSetting->passPhrase,
                        'host_name' =>  $icon->iconSetting->hostName,
                        'stop_code' =>  $icon->iconSetting->stopCode,
                        'terminal_type' =>  'vanilla',
                        'character_code' =>  $icon->iconSetting->characterCode,
                        'prompt_string' =>  $icon->iconSetting->promptString,
                        'command' =>  $icon->iconSetting->command,
                    ];

                    $this->jobnetModel->insertAgentLessIcon($agentLessIconData);
                    $this->logger->debug('Save agentless icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_ZABBIX:
                    $this->logger->debug('Save zabbix icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $zabbixIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'link_target' =>  $icon->iconSetting->linkTarget,
                        'link_operation' =>  $icon->iconSetting->linkOperation,
                        'groupid' =>  isset($icon->iconSetting->groupId) ? $icon->iconSetting->groupId : 0,
                        'hostid' =>  isset($icon->iconSetting->hostId) ? $icon->iconSetting->hostId : 0,
                        'itemid' =>  isset($icon->iconSetting->itemId) ? $icon->iconSetting->itemId : 0,
                        'triggerid' =>  isset($icon->iconSetting->triggerId) ? $icon->iconSetting->triggerId : 0,
                    ];

                    $this->jobnetModel->insertZabbixIcon($zabbixIconData);
                    $this->logger->debug('Save zabbix icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;

                case Constants::ICON_TYPE_JOBNET:
                    $this->logger->debug('Save jobnet icon data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    $jobnetIconData = [
                        'jobnet_id' => $jobData->jobnetId,
                        'job_id' => $icon->iconSetting->jobId,
                        'update_date' => $updateDate,
                        'link_jobnet_id' =>  $icon->iconSetting->linkJobnetId,
                    ];

                    $this->jobnetModel->insertJobnetIcon($jobnetIconData);
                    $this->logger->debug('Save jobnet icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                    break;
            }
            $this->logger->debug('Save icon data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
    }

    /**
     * It inserts the flow data.
     *
     * @param   object $jobData     jobnet object including icon and flow data.
     * @param   string $updateDate     updated date of the jobnet.
     * @since   Method available since version 6.1.0
     */
    private function insertFlowData($jobData, $updateDate)
    {
        $this->logger->debug('Save flow data process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        foreach ($jobData->flow as $flow) {

            $flowData = [
                'jobnet_id' => $jobData->jobnetId,
                'update_date' => $updateDate,
                'start_job_id' =>  $flow->startJobId,
                'end_job_id' => $flow->endJobId,
                'flow_type' => $flow->flowType,
                'flow_width' => 0,
                'flow_style' => $flow->flowStyle
            ];

            $this->flowModel->insertFlow($flowData);
        }

        $this->logger->debug('Save flow data process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the jobnet is execuatable or not
     *
     * @param   array $jobInfo     array of the icon.
     * @param   array $flowInfo     array of the flow.
     * @param   bool $isCamel     optional. Check object property is camel case. default value is false.
     * @param   string $objectId     id of the jobnet.
     * @throws  PDOException
     * @throws  JobnetNotExecutableException if the jobnet is not executable.
     * @return  void|string could be an void if it is executable, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function isExecutableJobnet($jobInfo, $flowInfo, $isCamel = false, $objectId)
    {
        $this->logger->debug('check executable jobnet process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {

            $parallelEndValid = $this->enableCheckParallelEnd($jobInfo, $isCamel);
            if (is_array($parallelEndValid)) {
                $this->logger->info('Parallel job icon check failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $parallelEndValid;
            }
            $confirmJobnetCheckValid = $this->confirmJobnetCheck($jobInfo, $flowInfo, $isCamel, $objectId);
            if (is_array($confirmJobnetCheckValid)) {
                $this->logger->info('Job icon valid check failed.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return $confirmJobnetCheckValid;
            }
            return true;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage(), $e->getCode());
        } catch (Exception $e) {
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }
        $this->logger->debug('check executable jobnet process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the parallel flow of jobnet is correct or not.
     *
     * @param   array $jobs     array of the icon.
     * @param   bool $isCamel     optional. Check object property is camel case. default value is false.
     * @throws  JobnetNotExecutableException if the jobnet is not executable.
     * @return  bool|string could be an bool if it is correct, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function enableCheckParallelEnd($jobs, $isCamel = false)
    {

        $this->logger->debug('check parallel end process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $parallelStartCount = 0;
        $parallelEndCount = 0;

        foreach ($jobs as $job) {
            if ($isCamel) {
                $jobType = $job->iconType;
            } else {
                $jobType = $job->job_type;
            }

            if ($jobType == Constants::ICON_TYPE_PARALLEL_START) {
                $parallelStartCount++;
            }
            if ($jobType == Constants::ICON_TYPE_PARALLEL_END) {
                $parallelEndCount++;
            }

            if ($parallelStartCount != $parallelEndCount) {
                if ($parallelStartCount > $parallelEndCount) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_ERR_JOBEDIT_16, "");
                    return $data;
                } else {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_ERR_JOBEDIT_16, "");
                    return $data;
                }
            }
            return true;
        }
        $this->logger->debug('check parallel end process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It checks the icon and flow are correctly connected or not.
     * It also checks require icons are existed or not.
     *
     * @param   array $jobs     array of the icon.
     * @param   array $flows     array of the flow.
     * @param   bool $isCamel     optional. Check object property is camel case. default value is false.
     * @throws  JobnetNotExecutableException if the jobnet is not executable.
     * @return  bool|string could be an bool if it is correct, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function confirmJobnetCheck($jobs, $flows, $isCamel = false, $objectId)
    {

        $this->logger->debug('Check jobnet flow process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        $result = true;
        $startIconCount = 0;
        $endIconCount = 0;

        foreach ($jobs as $job) {
            $inFlowNum = 0;
            $outFlowNum = 0;
            $trueFlowNum = 0;
            $falseFlowNum = 0;

            if ($isCamel) {
                $jobId = $job->iconSetting->jobId;
                $jobType = $job->iconType;
            } else {
                $jobId = $job->job_id;
                $jobType = $job->job_type;
            }

            $flow = array_filter($flows, function ($flow) use ($jobId, $isCamel) {
                if ($isCamel) {
                    if ($flow->endJobId == $jobId) {
                        return true;
                    }
                } else {
                    if ($flow->end_job_id == $jobId) {
                        return true;
                    }
                }
                return false;
            });

            if ($flow != null) {
                $inFlowNum = count($flow);
            }

            $flow = array_filter($flows, function ($flow) use ($jobId, $isCamel) {
                if ($isCamel) {
                    if ($flow->startJobId == $jobId) {
                        return true;
                    }
                } else {
                    if ($flow->start_job_id == $jobId) {
                        return true;
                    }
                }
                return false;
            });

            if ($flow != null) {
                $outFlowNum = count($flow);
            }

            switch ($jobType) {
                case Constants::ICON_TYPE_JOB:
                case Constants::ICON_TYPE_JOB_CONTROL_VARIABLE:
                case Constants::ICON_TYPE_EXTENDED_JOB:
                case Constants::ICON_TYPE_JOBNET:
                case Constants::ICON_TYPE_CALCULATION:
                case Constants::ICON_TYPE_TASK:
                case Constants::ICON_TYPE_INFO:
                case Constants::ICON_TYPE_FILE_COPY:
                case Constants::ICON_TYPE_FILE_WAIT:
                case Constants::ICON_TYPE_REBOOT:
                case Constants::ICON_TYPE_RELEASE:
                case Constants::ICON_TYPE_ZABBIX:
                case Constants::ICON_TYPE_AGENT_LESS:
                    if ($inFlowNum != 1 || $outFlowNum != 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_START:
                    $startIconCount++;
                    // INフローの本数≠0、またはOUTフローの本数≠1の場合
                    if ($inFlowNum != 0 || $outFlowNum != 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_END:
                    $endIconCount++;
                    // INフローの本数≠1、またはOUTフローの本数≠0の場合
                    if ($inFlowNum != 1 || $outFlowNum != 0)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_CONDITIONAL_START:

                    $flow = array_filter($flows, function ($flow) use ($jobId, $isCamel) {
                        if ($isCamel) {
                            if ($flow->startJobId == $jobId && $flow->flowType == 1) {
                                return true;
                            }
                        } else {
                            if ($flow->start_job_id == $jobId && $flow->flow_type == 1) {
                                return true;
                            }
                        }
                        return false;
                    });

                    if ($flow != null) {
                        $trueFlowNum = count($flow);
                    }

                    $flow = array_filter($flows, function ($flow) use ($jobId, $isCamel) {
                        if ($isCamel) {
                            if ($flow->startJobId == $jobId && $flow->flowType == 2) {
                                return true;
                            }
                        } else {
                            if ($flow->start_job_id == $jobId && $flow->flow_type == 2) {
                                return true;
                            }
                        }
                        return false;
                    });
                    if ($flow != null) {
                        $falseFlowNum = count($flow);
                    }

                    // INフローの本数≠1、TRUEフローの本数≠1
                    // またはFALSEフローの本数≠1の場合
                    if ($inFlowNum != 1 || $trueFlowNum != 1 || $falseFlowNum != 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_CONDITIONAL_END:
                    if ($inFlowNum < 1 || $outFlowNum != 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_PARALLEL_START:
                    if ($inFlowNum != 1 || $outFlowNum < 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_PARALLEL_END:
                    if ($inFlowNum < 1 || $outFlowNum != 1)
                        $result = false;
                    break;

                case Constants::ICON_TYPE_LOOP:
                    if ($inFlowNum != 2 || $outFlowNum != 1)
                        $result = false;
                    break;
            }

            if ($result == false) {
                //throw new JobnetNotExecutableException(Constants::ERROR_FLOW_001 . $jobId . Constants::ERROR_FLOW_002);
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_ERR_FLOW, $objectId, $jobId);
                return $data;
            }
        }

        if ($result == true && ($startIconCount != 1 || $endIconCount < 1)) {
            $data = (array)$this->util->createResponseJson(Constants::DETAIL_ERR_JOBEDIT_12, "");
            return $data;
        }

        return $result;

        $this->logger->debug('Check jobnet flow process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * It deletes the jobnet list from object_list screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function deleteList()
    {
        $this->logger->info('Multiple jobnet delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        $json = file_get_contents('php://input');
        $para = json_decode($json, true);
        $request = $para["params"]["datas"];
        $result = $this->deleteListService($request);
        if (!is_array($result)) {
            $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, "");
            return $data;
        } else {
            $this->logger->info('Multiple calendar version delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $result;
        }
    }
    /**
     * It deletes the jobnet from object_list
     *
     * @param   object $para     send data from the browser.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function deleteListService($para)
    {
        $this->logger->info('jobnet change valid process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $selectRows = $para['selectedRows'];

            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                $update_date = $value["update"];
                $objectDetail = $this->jobnetModel->each($objectId);
                if ($objectDetail == false) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_OBJECT_NOT_FOUND, $objectId);
                    return $data;
                }

                $detail = $this->jobnetModel->each($objectId, $update_date);
                if ($detail == false) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_NOT_LAST_UPDATED, $objectId);
                    return $data;
                }
                if (!$this->checkValid($detail, Constants::OBJ_DELETE_PROC)) {

                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    return $data;
                }
                //check if valid. Do
                if ($detail->valid_flag == 1) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    return $data;
                }
                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_UNEDITABLE) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_PERMIT, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    return $data;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    return $data;
                }
                $jobnetData = $this->jobnetModel->checkJobnetForDelete($objectId);
                $scheduleData = $this->jobnetModel->checkScheduleForDelete($objectId);
                if (!empty($jobnetData) || !empty($scheduleData)) {
                    $returnItemData = [
                        'objectId' => $objectId,
                        'jobnetData'  => $jobnetData,
                        'scheduleData' => $scheduleData
                    ];
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR,  $objectId, $returnItemData);
                    return $data;
                }
                //check if valid. Do
                if ($detail->valid_flag == 1) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_DEL,  $objectId);
                    return $data;
                }
            }

            $this->beginTransaction();
            $breakFlag = 0;
            foreach ($selectRows as $value) {
                $objectId = $value["id"];
                if ($this->jobnetModel->deleteAllVer($objectId) == true) {
                    $this->logger->info('[' . $objectId . '] jobnet has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } else {
                    $breakFlag = 1;
                }
            }
            if ($breakFlag == 1) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_FAIL,  $objectId);
                $this->rollback();
            } else {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_SUCCESS,  $objectId);
                $this->commit();
            }
            $this->logger->info('jobnet version Disable process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * It deletes the jobnet version from object_version screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function delete()
    {
        $json = file_get_contents('php://input');
        $para = json_decode($json);
        $validationReq = (array)$para;
        $validation = $this->validator->validate((array)$validationReq["params"], [
            'data' => 'required',
            'category' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST, "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $tmpValidationArray = (array)$validationReq["params"]->data;
        $validation = $this->validator->validate((array)$tmpValidationArray['0'], [
            'updateDate' => 'required',
            'validState' => 'required',
            'objectId' => 'required',
        ]);

        if ($validation->fails()) {
            $errors = $validation->errors();
            $responseData = $this->util->createResponseJson(Constants::DETAIL_BAD_REQUEST,  "", $errors->firstOfAll());
            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, $responseData);
            exit;
        }
        $this->logger->info('jobnet delete process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {
            $this->beginTransaction();
            $deleteRows = $para->params->data;
            foreach ($deleteRows as $selectRows) {
                $objectId = $selectRows->objectId;

                $detail = $this->jobnetModel->each($objectId, $selectRows->updateDate);
                if ($detail == false) {
                    $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_RELOAD;
                    $data[Constants::AJAX_MESSAGE_DETAIL] = "db-lock";
                    $this->rollback();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                    exit;
                }
                //check if valid. Do
                if ($detail->valid_flag == 1) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_DEL, $objectId);
                    $this->rollback();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                    exit;
                }

                $isLock = $this->objectLockService->process((object) ["objectId" => $objectId, "objectType" => Constants::OBJECT_TYPE_JOBNET, "attemptIp" => $this->objectLockService->getClientIpAddress()], Constants::SERVICE_TYPE_CHECK, true);
                if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_BY_OTHERS) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK,  $objectId);
                    $this->rollback();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                    exit;
                } else if ($isLock == Constants::SERVICE_INTERNAL_SERVER_ERROR) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
                    $this->rollback();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                    exit;
                } else if ($isLock == Constants::SERVICE_MESSAGE_OBJ_LOCK_USER_SAME) {
                    $data = (array)$this->util->createResponseJson(Constants::DETAIL_LOCK, $objectId);
                    $this->rollback();
                    echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                    exit;
                }
                $totalRows = $this->jobnetModel->totalRows($objectId);
                $breakflag = false;
                if ($totalRows == count($deleteRows)) {
                    $jobnetData = $this->jobnetModel->checkJobnetForDelete($objectId);
                    $scheduleData = $this->jobnetModel->checkScheduleForDelete($objectId);
                    if (!empty($jobnetData) || !empty($scheduleData)) {
                        $returnItemData = [
                            'objectId' => $objectId,
                            'jobnetData'  => $jobnetData,
                            'scheduleData' => $scheduleData
                        ];
                        $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, (array)$returnItemData);
                        $this->rollback();

                        echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
                        exit;
                    }
                }
            }
            if ($this->jobnetModel->deleteArr($objectId, $deleteRows) == true) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                $this->commit();
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, $data);
                $this->logger->info('[' . $objectId . '] jobnet has been deleted.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            } else {
                $breakflag = true;
            }

            if ($breakflag == true) {
                $this->rollback();
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_FAIL,  $objectId);
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, $data);
            }
            // return $data;
        } catch (PDOException $e) {
            if ($this->db->inTransaction()) {
                $this->rollback();
            }
            $data = (array)$this->util->createResponseJson(Constants::DETAIL_SERVER_ERROR, $objectId);
            echo Util::response(Constants::API_RESPONSE_TYPE_500, $data);
        }

        $this->logger->info('Jobnet delete process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
    /**
     * It checks the object data is latest or not.
     *
     * @param   string $id     id of the jobnet.
     * @param   string $date     updated date of the jobnet.
     * @param   string $validstate     valid flag of the jobnet.
     * @param   object $dbLatest     object of the latest jobnet.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function checkLatest($id, $date, $dbLatest)
    {
        $updated = false;
        if ($id == $dbLatest->jobnet_id && $date == $dbLatest->update_date) {
            $updated = true;
        }
        return $updated;
    }
    /**
     * It enables the jobnet object. It means that change the valid flag of the jobnet to 1.
     *
     * @param   object $objectId     id of the jobnet.
     * @param   object $updateDate     updated date of the jobnet.
     * @throws  JobnetNotExecutableException if the jobnet is not executable.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function enable($objectId, $updateDate)
    {
        $this->logger->info('jobnet version enable process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $jobIcon = $this->jobnetModel->getJobControlData($objectId, $updateDate);
            $flow = $this->jobnetModel->getFlowData($objectId, $updateDate);

            $isExecutable = $this->isExecutableJobnet($jobIcon, $flow, false, $objectId);
            if (is_array($isExecutable)) {
                return $isExecutable;
            }

            $this->jobnetModel->changeAllStatusToDisabled($objectId);
            if ($this->jobnetModel->changeStatusToEnabled($objectId, $updateDate) == true) {
                // $data = (array)$this->util->createResponseJson(Constants::DETAIL_SUCCESS, $objectId);
                // $this->logger->debug('Jobnet version enable process is successful.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_SUCCESS,  $objectId);
            } else {
                // $data = (array)$this->util->createResponseJson(Constants::DETAIL_FAIL, $objectId);
                // $this->logger->info('Jobnet version enable failed for id:' . $objectId . ' updated date:' . $updateDate . '.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_FAIL,  $objectId);
            }
            $this->logger->debug('jobnet enable process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        } catch (JobnetNotExecutableException $e) {
            throw new JobnetNotExecutableException($e->getMessage(), $e->getCode());
        }
    }
    /**
     * It disables the jobnet object. It means that change the valid flag of the jobnet to 0.
     *
     * @param   object $objectId     id of the jobnet.
     * @param   object $updateDate     updated date of the jobnet.
     * @return  array|string could be an array if it is success, could be a string if not.
     * @since   Method available since version 6.1.0
     */
    public function disable($objectId, $updateDate)
    {

        try {
            $enableSchedule = $this->jobnetModel->checkScheduleEnable($objectId);
            if (!empty($enableSchedule)) {
                $returnItemData = [
                    'objectId' => $objectId,
                    'scheduleData'  => $enableSchedule
                ];
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_REL_ERROR, $objectId, (array)$returnItemData);
                return $data;
            }
            if ($this->jobnetModel->changeStatusToDisabled($objectId, $updateDate) == true) {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_SUCCESS,  $objectId);
            } else {
                $data = (array)$this->util->createResponseJson(Constants::DETAIL_FAIL, $objectId);
            }
            $this->logger->debug('jobnet disable process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            return $data;
        } catch (PDOException $e) {
            return Constants::SERVICE_INTERNAL_SERVER_ERROR;
        }
    }

    /**
     * api endpoint that checks jobnet exist
     *
     * @since   Method available since version 6.1.0
     */
    public function checkJobnetExist(): void
    {
        $this->logger->info('Check jobnet exist process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'jobnetId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => $this->checkID($params["jobnetId"])]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Check jobnet exist process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that checks job exist
     *
     * @since   Method available since version 6.1.0
     */
    public function checkJobExist(): void
    {
        $this->logger->info('Create job exist process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');
            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'jobnetId' => 'required',
                'jobs' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $jobnetInfo = $this->jobnetModel->GetMaxUpdateDateEntityById($params["jobnetId"]);
                if ($jobnetInfo) {
                    $isExist = true;
                    foreach ($params["jobs"] as $job) {
                        $result = $this->jobnetModel->getJobControlData($params["jobnetId"], $jobnetInfo->update_date, $job);
                        if (count($result) == 0) {
                            $isExist = false;
                            break;
                        }
                    }
                    if ($isExist) {
                        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_RECORD_EXIST]);
                    } else {
                        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST]);
                    }
                } else {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_RECORD_NOT_EXIST]);
                }
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Create job exist process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }

    /**
     * api endpoint that checks jobnet exist
     *
     * @since   Method available since version 6.1.0
     */
    public function getJobnetInfo(): void
    {
        $this->logger->info('Check jobnet exist process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);

        try {

            $json = file_get_contents('php://input');

            $params = Util::jsonDecode($json)["params"];

            $validation = $this->validator->validate($params, [
                'jobnetId' => 'required'
            ]);

            if ($validation->fails()) {
                $errors = $validation->errors();
                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                $this->logger->info(Constants::SERVICE_MESSAGE_VALIDATION_FAIL . implode(", ", $errors->firstOfAll()), ['controller' => __METHOD__]);
            } else {
                $jobnetInfo = $this->getValidORMaxUpdateDateEntityById($params["jobnetId"]);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $jobnetInfo]);
            }
        } catch (PDOException $e) {
            echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_INTERNAL_SERVER_ERROR]);
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        }

        $this->logger->info('Check jobnet exist process is finished.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
    }
}
