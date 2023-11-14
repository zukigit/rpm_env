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

use App\Utils\Response;
use App\Controllers\Pages;
use App\Utils\Constants;
use PDOStatement;
use PDOException;
use App\Exceptions\DbTransactionException;
use App\Utils\Database;

/**
 * This class is used to manage controller.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Controller
{

    protected $db;
    protected $serviceLevel = Constants::MAIN_SERVICE;

    public function __construct()
    {
        $this->response = new Response();
        $this->db = Core::db();
    }

    public function __destruct()
    {
        if(isset($_SESSION['userInfo'])){
            unset($_SESSION["userInfo"]);
        }
    }

    public function model($model)
    {
        return new $model();
    }

    /**
     * It checks for the file to load view.
     *
     * @param  string $view screen name
     * @param  array  $data page type and page id to load view
     * @since      Class available since version 6.1.0
     */
    public function view($view, $data = [])
    {
        if (file_exists('../app/views/' . $view . '.php')) {
            require_once('../app/views/' . $view . '.php');
        } else {
            die("View does not exists.");
        }
    }

    /**
     * It redirects to error_page.
     *
     * @since      Class available since version 6.1.0
     */
    public function itemNotFound()
    {
        $pages = new Pages();
        $pages->error();
    }

    /**
     * It sets error message to array.
     *
     * @param  string $errMsg 
     * @since      Class available since version 6.1.0
     */
    public function showErrMsg($errMsg)
    {
        $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_INVALID;
        $data[Constants::AJAX_MESSAGE_DETAIL] = $errMsg;
        echo json_encode($data);
    }

    /**
     * It sets object id duplicate error message to array.
     *
     * @since      Class available since version 6.1.0
     */
    public function recordExists()
    {
        $data[Constants::AJAX_MESSAGE_TYPE] = Constants::SERVICE_MESSAGE_RECORD_EXIST;
        echo json_encode($data);
    }

    /**
     * It responses fail error message to array.
     *
     * @param  string $errMsg 
     * @since      Class available since version 6.1.0
     */
    public function responseFail($errMsg)
    {
        $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_FAIL;
        $data[Constants::AJAX_MESSAGE_DETAIL] = $errMsg;
        echo json_encode($data);
    }

    /**
     * It responses object and success info message to array.
     *
     * @param  array $obj
     * @since      Class available since version 6.1.0
     */
    public function responseSuccess($obj)
    {
        $data[Constants::AJAX_MESSAGE_TYPE] = Constants::AJAX_MESSAGE_SUCCESS;
        $data[Constants::AJAX_MESSAGE_DATA] = $obj;
        echo json_encode($data);
    }

    public function changeServiceLevel(int $serviceLevel)
    {
        $this->serviceLevel = $serviceLevel;
    }

    public function getServiceLevel()
    {
        return $this->serviceLevel;
    }

    public function beginTransaction()
    {
        if ($this->serviceLevel == Constants::MAIN_SERVICE) {
            $this->logger->debug('Begin transaction process is started.', ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
            try {
                return $this->db->beginTransaction();
            } catch (PDOException $e) {
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
                throw new DbTransactionException($e->getMessage(), $e->getCode());
            }
        }
    }

    public function commit()
    {
        if ($this->serviceLevel == Constants::MAIN_SERVICE) {
            $this->logger->debug('Commit transaction  process is started.', ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
            try {
                return $this->db->commit();
            } catch (PDOException $e) {
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
                throw new DbTransactionException($e->getMessage(), $e->getCode());
            }
        }
    }

    public function rollback()
    {
        if ($this->serviceLevel == Constants::MAIN_SERVICE) {
            $this->logger->debug('Rollback transaction  process is started.', ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
            try {
                return $this->db->rollback();
            } catch (PDOException $e) {
                $this->logger->error($e->getMessage(), ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
                throw new DbTransactionException($e->getMessage(), $e->getCode());
            }
        }
    }
}
