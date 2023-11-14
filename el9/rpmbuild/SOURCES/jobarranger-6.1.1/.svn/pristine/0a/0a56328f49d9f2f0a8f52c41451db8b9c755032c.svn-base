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

namespace App\Models;

use App\Utils\Model;

/**
 * This model is used to manage the general setting.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class GeneralSettingModel extends Model
{
    private $selectSql = "SELECT value FROM ja_parameter_table WHERE parameter_name = :param";
    private $updateSql = "UPDATE ja_parameter_table SET value = :value WHERE parameter_name = :param";
    private $lockSql = "SELECT GET_LOCK('ja_parameter_table.:lockName', 0) as count";
    private $releaseLockSql = "SELECT RELEASE_LOCK('ja_parameter_table.:lockName') as count";
    //Obtain exclusive session level advisory lock if available
    private $lockPgSql = "SELECT pg_try_advisory_lock(1)";
    //Release an exclusive session level advisory lock
    private $releasePgLockSql = "SELECT pg_advisory_unlock(1)";

    /**
     * It retrieves all general setting data.
     *
     * @return  array $data  general setting data
     * @since      Class available since version 6.1.0
     */
    public function getParameterTableValue()
    {

        $data = [
            'jobnetViewSpan' => $this->getParameterValue("JOBNET_VIEW_SPAN"),
            'jobnetLoadSpan' => $this->getParameterValue("JOBNET_LOAD_SPAN"),
            'jobnetKeepSpan' => $this->getParameterValue("JOBNET_KEEP_SPAN"),
            'joblogKeepSpan' => $this->getParameterValue("JOBLOG_KEEP_SPAN"),
            'standardTime' => $this->getParameterValue("MANAGER_TIME_SYNC"),
            'notification' => $this->getParameterValue("ZBXSND_ON"),
            'zabbixServerIPaddress' => $this->getParameterValue("ZBXSND_ZABBIX_IP"),
            'zabbixServerPortNumber' => $this->getParameterValue("ZBXSND_ZABBIX_PORT"),
            'zabbixSenderCommand' => $this->getParameterValue("ZBXSND_SENDER"),
            'messageDestinationServer' => $this->getParameterValue("ZBXSND_ZABBIX_HOST"),
            'messageDestinationItemKey' => $this->getParameterValue("ZBXSND_ITEM_KEY"),
            'retry' => $this->getParameterValue("ZBXSND_RETRY"),
            'retryCount' => $this->getParameterValue("ZBXSND_RETRY_COUNT"),
            'retryInterval' => $this->getParameterValue("ZBXSND_RETRY_INTERVAL"),
            'heartbeatIntervalTime' => $this->getParameterValue("HEARTBEAT_INTERVAL_TIME"),
            'objectLockExpiredTime' => $this->getParameterValue("OBJECT_LOCK_EXPIRED_TIME"),
        ];

        return $data;
    }

    /**
     * It updates general setting data.
     *
     * @param   object  $data     data of general setting to update.
     * @return  bool   $success  could be true if update success   
     * @since      Class available since version 6.1.0
     */
    public function updateParameterTable($data)
    {

        $success = true;
        $success &= $this->setParameterValue("JOBNET_VIEW_SPAN", $data['params']['value']['jobnetViewSpan']);
        $success &= $this->setParameterValue("JOBNET_LOAD_SPAN",  $data['params']['value']['jobnetLoadSpan']);
        $success &= $this->setParameterValue("JOBNET_KEEP_SPAN",  $data['params']['value']['jobnetKeepSpan']);
        $success &= $this->setParameterValue("JOBLOG_KEEP_SPAN",  $data['params']['value']['joblogKeepSpan']);
        $success &= $this->setParameterValue("MANAGER_TIME_SYNC",  $data['params']['value']['standardTime']);
        $success &= $this->setParameterValue("ZBXSND_ON",  $data['params']['value']['notification']);
        $success &= $this->setParameterValue("ZBXSND_ZABBIX_IP",  $data['params']['value']['zabbixServerIPaddress']);
        $success &= $this->setParameterValue("ZBXSND_ZABBIX_PORT",  $data['params']['value']['zabbixServerPortNumber']);
        $success &= $this->setParameterValue("ZBXSND_SENDER",  $data['params']['value']['zabbixSenderCommand']);
        $success &= $this->setParameterValue("ZBXSND_ZABBIX_HOST",  $data['params']['value']['messageDestinationServer']);
        $success &= $this->setParameterValue("ZBXSND_ITEM_KEY",  $data['params']['value']['messageDestinationItemKey']);
        $success &= $this->setParameterValue("ZBXSND_RETRY",  $data['params']['value']['retry']);
        $success &= $this->setParameterValue("ZBXSND_RETRY_COUNT",  $data['params']['value']['retryCount']);
        $success &= $this->setParameterValue("ZBXSND_RETRY_INTERVAL",  $data['params']['value']['retryInterval']);
        $success &= $this->setParameterValue("HEARTBEAT_INTERVAL_TIME",  $data['params']['value']['heartbeatIntervalTime']);
        $success &= $this->setParameterValue("OBJECT_LOCK_EXPIRED_TIME",  $data['params']['value']['objectLockExpiredTime']);

        return $success;
    }

    /**
     * It retrieves general setting data on specific column name.
     *
     * @param   string $columnName 
     * @return  string $paramValue  general setting data on specific column name
     * @since      Class available since version 6.1.0
     */
    public function getParameterValue($columName)
    {
        $paramValue = $this->db->singleValueByParam($this->selectSql, $columName);
        if (empty($paramValue)) {
            $paramValue = constant('App\Utils\Constants::DEFAULT_' . $columName . '_VALUE');
        }

        return $paramValue;
    }

    /**
     * It updates general setting data on specific column name.
     *
     * @param   string  $columnName    column name 
     * @param   string  $value         update value
     * @return  bool    could be true if update success   
     * @since      Class available since version 6.1.0
     */
    public function setParameterValue($columName, $value)
    {
        $this->db->query($this->updateSql);

        $this->db->bind(':param', $columName);
        $this->db->bind(':value', $value);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
        // return $this->db->execute();
    }

    /**
     * It gets lock for current update process for mysql.
     *
     * @param   string  $lockName    
     * @return  bool    could be true if lock process success   
     * @since      Class available since version 6.1.0
     */
    public function getLock($lockName)
    {
        $this->db->query($this->lockSql);
        $this->db->bind(':lockName', $lockName);

        $count = $this->db->resultSet()[0]->count;
        if ($count < 1) {
            $this->releaseLock($lockName);
            return false;
        }
        return true;
    }

    /**
     * It releases current lock for mysql.
     *
     * @param   string  $lockName
     * @return  bool    could be true if release process success   
     * @since      Class available since version 6.1.0
     */
    public function releaseLock($lockName)
    {
        $this->db->query($this->releaseLockSql);
        $this->db->bind(':lockName', $lockName);
        $count = $this->db->resultSet()[0]->count;
        return $this->db->execute();
    }

    /**
     * It gets lock for current update process for postgresql.
     *
     * @param   string  $lockName    
     * @return  bool    could be true if lock process success   
     * @since      Class available since version 6.1.0
     */
    public function getPgLock()
    {
        $this->db->query($this->lockPgSql);
        $lock = $this->db->execute();
        if (!$lock) {
            $this->releasePgLock();
            return false;
        }
        return true;
    }

    /**
     * It releases current lock for postgresql.
     *
     * @param   string  $lockName
     * @return  bool    could be true if release process success   
     * @since      Class available since version 6.1.0
     */
    public function releasePgLock()
    {
        $this->db->query($this->releasePgLockSql);
        return $this->db->execute();
    }
}
