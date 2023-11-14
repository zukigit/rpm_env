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

declare(strict_types=1);

require_once('../vendor/autoload.php');

use Rakit\Validation\Validator;
use App\Utils\Core;
use App\Controllers\Api\ZabbixApi;
use App\Controllers\Api\LoginApi;
use App\Exceptions\DbConnectionException;
use App\Utils\Constants;
use App\Utils\Util;
use App\Utils\Response;

/**
 * This router determines which controllers and actions are executed based on the URL requested.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Router
{

    public const UNPROTECT_ROUTE = 0;

    public const PROTECT_ROUTE = 1;

    private const PARAMS_UNREQUIRED = 0;

    private const PARAMS_REQUIRED = 1;

    private $logger;

    public function __construct()
    {
        $this->response = new Response();
        $this->validator = new Validator();
    }

    public function resolve()
    {

        $dispatcher = FastRoute\SimpleDispatcher(function (FastRoute\RouteCollector $r) {

            $r->addGroup('/jobarranger/api', function (FastRoute\RouteCollector $r) {

                $r->addRoute('POST', '/', ['App\Controllers\ResponseSample', 'welcome']);

                //sample response
                $r->addRoute('GET', '/dataArray', ['App\Controllers\ResponseSample', 'sampledataArray']);
                $r->addRoute('GET', '/dataItem', ['App\Controllers\ResponseSample', 'sampleData']);
                $r->addRoute('POST', '/data', ['App\Controllers\ResponseSample', 'sampleOk']);
                $r->addRoute('POST', '/dataReturn', ['App\Controllers\ResponseSample', 'sampleOkWithReturn']);
                $r->addRoute('POST', '/errorBadRequest', ['App\Controllers\ResponseSample', 'errorBadRequest']);
                $r->addRoute('POST', '/errorForbidden', ['App\Controllers\ResponseSample', 'errorForbidden']);
                $r->addRoute('POST', '/errorUnauthorized', ['App\Controllers\ResponseSample', 'errorUnauthorized']);

                //login
                $r->addRoute('POST', '/login', ['App\Controllers\Users', 'login']);

                //logout
                $r->addRoute('POST', '/logout', ['App\Controllers\Users', 'logout', self::PROTECT_ROUTE]);

                //api check
                $r->addRoute('POST', '/apiCheck', ['App\Controllers\Users', 'apiCheck', self::PROTECT_ROUTE]);

                //object lock
                $r->addRoute('POST', '/objectLock/delete', ['App\Controllers\ObjectLock', 'unlock', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectLock/check', ['App\Controllers\ObjectLock', 'checkIsObjectLocked', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectLock/heartbeat', ['App\Controllers\ObjectLock', 'heartbeat', self::PROTECT_ROUTE]);

                //Lock Management
                $r->addRoute('POST', '/getAllLockedObj', ['App\Controllers\ObjectLock', 'getAllLockedObj', self::PROTECT_ROUTE]);

                //Object List
                $r->addRoute('POST', '/objectList', ['App\Controllers\ObjectList', 'getAll', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectList/changeValidList', ['App\Controllers\ObjectList', 'changeValidList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectList/deleteList', ['App\Controllers\ObjectList', 'deleteList', self::PROTECT_ROUTE]);

                //Schedule
                $r->addRoute('POST', '/schedule/initCreate', ['App\Controllers\Schedule', 'initCreate', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/boottime/getCalFltIDList', ['App\Controllers\Boottime', 'getCalFltIDList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/boottime/getJobnetIDList', ['App\Controllers\Boottime', 'getJobnetIDList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/boottime/registration', ['App\Controllers\Boottime', 'registration', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/boottime/jobnet', ['App\Controllers\Boottime', 'jobnet', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/schedule/save', ['App\Controllers\Schedule', 'save', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/schedule/initEdit', ['App\Controllers\Schedule', 'initEdit', self::PROTECT_ROUTE]);

                //Calendar
                $r->addRoute('POST', '/calendar/save', ['App\Controllers\Calendar', 'save', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/calendar/initCreate', ['App\Controllers\Calendar', 'initCreate', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/calendar/version', ['App\Controllers\ObjectList', 'version', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/calendar/initEdit', ['App\Controllers\Calendar', 'initEdit', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/calendar/calendarList', ['App\Controllers\Calendar', 'getCalendarList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/calendar/getValidOrLatest', ['App\Controllers\Calendar', 'getValidOrLatest', self::PROTECT_ROUTE]);

                //Filter
                $r->addRoute('POST', '/filter/initCreate', ['App\Controllers\Filter', 'initCreate', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/filter/getCalendarDate', ['App\Controllers\Filter', 'getCalendarDate', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/filter/save', ['App\Controllers\Filter', 'save', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/filter/initEdit', ['App\Controllers\Filter', 'initEdit', self::PROTECT_ROUTE]);

                //Import
                $r->addRoute('POST', '/importXML', ['App\Controllers\Import', 'importXML', self::PROTECT_ROUTE]);

                //Export
                $r->addRoute('POST', '/exportXML', ['App\Controllers\Export', 'exportXML', self::PROTECT_ROUTE]);

                //Jobnet
                $r->addRoute('POST', '/jobnet/host', ['App\Controllers\Jobnet', 'getAvailableHosts', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobnet/defineValueJobCon', ['App\Controllers\Jobnet', 'getDefineValueJobControl', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobnet/defineExtendedJob', ['App\Controllers\Jobnet', 'getDefineExtendedJob', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobnet/all', ['App\Controllers\Jobnet', 'getJobnetList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobnet/hostGroup', ['App\Controllers\Jobnet', 'getHostGroup', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobnet/zabbixApi/host', ['App\Controllers\Jobnet', 'selectHostName', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/zabbixApi/item', ['App\Controllers\Jobnet', 'selectItem', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/zabbixApi/trigger', ['App\Controllers\Jobnet', 'selectTrigger', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/getJobnetOption', ['App\Controllers\Jobnet', 'getJobnetOption', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/initCreate', ['App\Controllers\Jobnet', 'initCreate', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/initEdit', ['App\Controllers\Jobnet', 'initEdit', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/initSubJobnet', ['App\Controllers\Jobnet', 'initSubJobnet', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/save', ['App\Controllers\Jobnet', 'save', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/check', ['App\Controllers\Jobnet', 'checkJobnetExist', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/checkJob', ['App\Controllers\Jobnet', 'checkJobExist', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobnet/get', ['App\Controllers\Jobnet', 'getJobnetInfo', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);

                //Job Execution
                $r->addRoute('POST', '/exec/checkValid', ['App\Controllers\JobExecution', 'checkValid', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/run', ['App\Controllers\JobExecution', 'run', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/getData', ['App\Controllers\JobExecution', 'getRunJobnetData', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/singleRun', ['App\Controllers\JobExecution', 'singleJobRun', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/skip', ['App\Controllers\JobExecution', 'setSkip', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/hold', ['App\Controllers\JobExecution', 'setHold', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/normal', ['App\Controllers\JobExecution', 'setNormal', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/rerun', ['App\Controllers\JobExecution', 'setRerun', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/forceStop', ['App\Controllers\JobExecution', 'setForceStop', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/exec/valueChange', ['App\Controllers\JobExecution', 'variableValueChange', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);

                //Job Execution Management
                $r->addRoute('POST', '/jobExecManagement/all', ['App\Controllers\JobnetExecManagement', 'getAllOperationList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobExecManagement/error', ['App\Controllers\JobnetExecManagement', 'getErrorOperationList', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobExecManagement/during', ['App\Controllers\JobnetExecManagement', 'getDuringOperationList', self::PROTECT_ROUTE]);

                $r->addRoute('POST', '/jobExecManagement/stopAllJobnetSummary', ['App\Controllers\JobnetExecManagement', 'setAllJobnetSummaryStop', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/stopErrorJobnetSummary', ['App\Controllers\JobnetExecManagement', 'setErrorJobnetSummaryStop', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/stopDuringJobnetSummary', ['App\Controllers\JobnetExecManagement', 'setDuringJobnetSummaryStop', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/delayJobnetSummary', ['App\Controllers\JobnetExecManagement', 'delayJobnetSummary', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/updateSchedule', ['App\Controllers\JobnetExecManagement', 'updateScheduleJobnetSummary', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/holdJobnetSummary', ['App\Controllers\JobnetExecManagement', 'setHoldJobnetSummary', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/releaseJobnetSummary', ['App\Controllers\JobnetExecManagement', 'setReleaseJobnetSummary', self::PROTECT_ROUTE, self::PARAMS_REQUIRED]);
                $r->addRoute('POST', '/jobExecManagement/checkScheduleValid', ['App\Controllers\JobnetExecManagement', 'checkIsScheduleValid', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobExecManagement/deleteSchedule', ['App\Controllers\JobnetExecManagement', 'deleteScheduleJobnetSummary', self::PROTECT_ROUTE]);


                //General Setting
                $r->addRoute('POST', '/generalSetting/getAll', ['App\Controllers\GeneralSetting', 'getAll', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/generalSetting/update', ['App\Controllers\GeneralSetting', 'update', self::PROTECT_ROUTE]);

                //Job Execution Result
                $r->addRoute('POST', '/jobExecutionResult/getUsers', ['App\Controllers\JobExecutionResult', 'getAllUser', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobExecutionResult/search', ['App\Controllers\JobExecutionResult', 'searchResult', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/jobExecutionResult/exportCsv', ['App\Controllers\JobExecutionResult', 'exportCSV', self::PROTECT_ROUTE]);

                //Object Version
                $r->addRoute('POST', '/objectVersion/calendar', ['App\Controllers\Calendar', 'version', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/calendar/deleteVersion', ['App\Controllers\Calendar', 'delete', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/calendar/changeValidVersion', ['App\Controllers\Calendar', 'changeValidVersion', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/filter', ['App\Controllers\Filter', 'version', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/filter/deleteVersion', ['App\Controllers\Filter', 'delete', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/filter/changeValidVersion', ['App\Controllers\Filter', 'changeValidVersion', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/schedule', ['App\Controllers\Schedule', 'version', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/schedule/deleteVersion', ['App\Controllers\Schedule', 'delete', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/schedule/changeValidVersion', ['App\Controllers\Schedule', 'changeValidVersion', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/jobnet', ['App\Controllers\Jobnet', 'version', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/jobnet/deleteVersion', ['App\Controllers\Jobnet', 'delete', self::PROTECT_ROUTE]);
                $r->addRoute('POST', '/objectVersion/jobnet/changeValidVersion', ['App\Controllers\Jobnet', 'changeValidVersion', self::PROTECT_ROUTE]);
            });
        });

        // Fetch method and URI from somewhere
        $httpMethod = $_SERVER['REQUEST_METHOD'];
        $uri = $_SERVER['REQUEST_URI'];

        // Strip query string (?foo=bar) and decode URI
        if (false !== $pos = strpos($uri, '?')) {
            $uri = substr($uri, 0, $pos);
        }
        $uri = rawurldecode($uri);

        if (strpos($uri, '/jobarranger/public/') == false) {
            $routeInfo = $dispatcher->dispatch($httpMethod, $uri);
            if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
                $config = "/etc/jobarranger/web/jam.config.php";
            } else {
                $config = "../app/config/jam.config.php";
            }
            if (file_exists($config)) {
                try {
                    require_once $config;
                    $core = new Core();
                    $this->logger = $core::logger();
                    if (strpos($uri, 'setup') == true && CONFIG_CREATION_PERMISSION == 1) {
                        $this->SetupRouter($httpMethod, $uri);
                    } else {
                        define('APPROOT', dirname(dirname(__FILE__)));
                        switch ($routeInfo[0]) {
                            case FastRoute\Dispatcher::NOT_FOUND:
                                echo Util::response(Constants::API_RESPONSE_TYPE_404, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_NOT_FOUND]);
                                break;
                            case FastRoute\Dispatcher::METHOD_NOT_ALLOWED:
                                echo Util::response(Constants::API_RESPONSE_TYPE_404, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_NOT_FOUND]);
                                break;
                            case FastRoute\Dispatcher::FOUND:

                                $controller = new $routeInfo[1][0]; // "UserController"
                                $action = $routeInfo[1][1]; // "list" action
                                $parameters = $routeInfo[2]; // Action parameters list (e.g. route parameters list)

                                if (isset($routeInfo[1][2])) {
                                    if ($routeInfo[1][2] == self::PROTECT_ROUTE) {

                                        $json = file_get_contents('php://input');

                                        // Converts it into a PHP object
                                        $data = Util::jsonDecode($json);

                                        if ($data == false) {
                                            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_JSON_INVALID]);
                                            return;
                                        }

                                        if (isset($routeInfo[1][3]) && $routeInfo[1][3] == self::PARAMS_REQUIRED) {
                                            $validateParams = $this->validator->validate($data, [
                                                'params' => 'required',
                                            ]);

                                            if ($validateParams->fails()) {
                                                $errors = $validateParams->errors();
                                                echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                                                $this->logger->info(Constants::SERVICE_MESSAGE_JSON_INVALID, ['controller' => __METHOD__]);
                                                return;
                                            }
                                        }

                                        $validateSessionId = $this->validator->make($data, [
                                            'sessionId' => 'required',
                                        ]);

                                        $validateSessionId->setMessages([
                                            'sessionId:required' => Constants::API_RESPONSE_NOT_AUTHORISED . ' The sessionId is requied.',
                                        ]);
                                        $validateSessionId->validate();
                                        if ($validateSessionId->fails()) {
                                            $errors = $validateSessionId->errors();
                                            echo Util::response(Constants::API_RESPONSE_TYPE_BAD_REQUEST, [Constants::API_RESPONSE_MESSAGE => $errors->firstOfAll()]);
                                            $this->logger->info(Constants::SERVICE_MESSAGE_JSON_INVALID, ['controller' => __METHOD__]);
                                            return;
                                        } else {
                                            //check user session is valid
                                            $result = LoginApi::checkAuthentication($data["sessionId"]);

                                            if ($result == false) {
                                                echo Util::response(Constants::API_RESPONSE_TYPE_500, [Constants::API_RESPONSE_MESSAGE => "Couldn't connect with zabbix api"]);
                                                $this->logger->error("Couldn't connect with zabbix api", ['controller' => __METHOD__]);
                                                return;
                                            }

                                            $final = json_decode($result);
                                            if (isset($final->error)) {
                                                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => $final->error->data]);
                                                $this->logger->info($final->error->data, ['controller' => __METHOD__]);
                                                break;
                                            }
                                        }

                                        $groupResult = json_decode(ZabbixApi::GetUserGroup($final->result->sessionid, $final->result->userid));
                                        if(isset($groupResult->error)){
                                            $groupResultArray = [];
                                        }else{
                                            $groupResultArray = $groupResult->result;
                                        }
                                        $_SESSION['userInfo'] = [
                                            "userName" => $final->result->username,
                                            "sessionId" => $final->result->sessionid,
                                            "userId" => $final->result->userid,
                                            "userType" => $final->result->type,
                                            "userLangFull" => strtolower($final->result->lang),
                                            "hasUserGroup" => (count($groupResultArray) > 0),
                                            "groupList" => $groupResultArray
                                        ];
                                    }
                                }
                                if (is_callable([$controller, $action])) {
                                    return call_user_func_array([$controller, $action], $parameters);
                                }

                                break;
                        }
                    }
                } catch (DbConnectionException $e) {
                    //$response = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
                    $response = [
                        Constants::AJAX_MESSAGE_DETAIL =>  Constants::DETAIL_DB_ERROR,
                        Constants::AJAX_MESSAGE_OBJECTID => "",
                        Constants::AJAX_MESSAGE_DETAIL_TXT => Constants::MESSAGE[Constants::DETAIL_DB_ERROR],
                    ];
                    echo Util::response(Constants::API_RESPONSE_TYPE_500, $response);
                    // $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                } catch (Exception $e){
                    //$response = $this->utils->createResponseJson(Constants::DETAIL_SERVER_ERROR);
                    $response = [
                        Constants::AJAX_MESSAGE_DETAIL =>  Constants::DETAIL_SERVER_ERROR,
                        Constants::AJAX_MESSAGE_OBJECTID => "",
                        Constants::AJAX_MESSAGE_DETAIL_TXT => Constants::MESSAGE[Constants::DETAIL_SERVER_ERROR],
                    ];
                    echo Util::response(Constants::API_RESPONSE_TYPE_500, $response);
                }
            } else {
                $this->SetupRouter($httpMethod, $uri);
            }
        }
    }

    public function SetupRouter($httpMethod, $uri)
    {
        $dispatcherSetup = FastRoute\SimpleDispatcher(function (FastRoute\RouteCollector $r) {

            $r->addGroup('/jobarranger/api', function (FastRoute\RouteCollector $r) {
                $r->addRoute('POST', '/setup/Initial', ['App\Controllers\Setup', 'Initial']);
                $r->addRoute('POST', '/setup/PreRequirement', ['App\Controllers\Setup', 'PreRequirement']);
                $r->addRoute('POST', '/setup/dbConnection', ['App\Controllers\Setup', 'DBConnection']);
                $r->addRoute('POST', '/setup/zbxApiCheck', ['App\Controllers\Setup', 'zabbixApiCheck']);
                $r->addRoute('POST', '/setup/logPathCheck', ['App\Controllers\Setup', 'logPathCheck']);
                $r->addRoute('POST', '/setup/configCreate', ['App\Controllers\Setup', 'configCreated']);
            });
        });
        $routeInfo = $dispatcherSetup->dispatch($httpMethod, $uri);
        if (strpos($uri, 'setup') == false) {
            echo Util::response(Constants::API_RESPONSE_TYPE_CONFIG, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_CONFIG_NOT_FOUND]);
        } else {
            switch ($routeInfo[0]) {
                case FastRoute\Dispatcher::NOT_FOUND:
                    echo Util::response(Constants::API_RESPONSE_TYPE_404, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_NOT_FOUND]);
                    break;
                case FastRoute\Dispatcher::METHOD_NOT_ALLOWED:
                    echo Util::response(Constants::API_RESPONSE_TYPE_404, [Constants::API_RESPONSE_MESSAGE => Constants::API_RESPONSE_NOT_FOUND]);
                    break;
                case FastRoute\Dispatcher::FOUND:

                    $controller = new $routeInfo[1][0]; // "UserController"
                    $action = $routeInfo[1][1]; // "list" action
                    $parameters = $routeInfo[2];
                    if (is_callable([$controller, $action])) {
                        return call_user_func_array([$controller, $action], $parameters);
                    }
                    break;
            }
        }
    }
}
