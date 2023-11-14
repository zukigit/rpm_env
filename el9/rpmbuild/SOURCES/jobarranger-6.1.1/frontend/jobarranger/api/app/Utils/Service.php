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

namespace App\Utils;

use App\Utils\Constants;
use PDOStatement;
use PDOException;
use App\Exceptions\DbTransactionException;
use App\Utils\Database;

/**
 * This class is used to manage the service.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
abstract class Service
{
    protected $db;
    protected $serviceLevel = Constants::MAIN_SERVICE;

    public function __construct()
    {
        $this->db = Core::db();
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
