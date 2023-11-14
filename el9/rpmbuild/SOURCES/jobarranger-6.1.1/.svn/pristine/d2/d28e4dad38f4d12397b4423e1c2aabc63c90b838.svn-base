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

use App\Utils\Constants;
use App\Utils\Model;

/**
 * This model is used to manage the run jobnet.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class RunJobnetModel extends Model{
    public function __construct() {
        parent::__construct();
        $this->dbUtilModel = new DbUtilModel();
    }

    /**
     * It retrieves the valid jobnet.
     *
     * @param   string $jobnetId  id of the jobnet.
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getValidJobnetVersion($jobnetId) {
        $this->db->query("select * from ja_jobnet_control_table where jobnet_id= '$jobnetId' and valid_flag=1");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run value after.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunJobValueAfter($innerJobId) {
        $this->db->query("select * from ja_run_value_after_table where inner_job_id = '$innerJobId' order by value_name");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run value before.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunJobValueBefore($innerJobId, $jobType) {
        $strSqlJobBefore = "(select 'ja_run_value_before_table' as table_name, 'before_value' as value_column, BV.inner_jobnet_id, BV.value_name, BV.before_value from ja_run_value_before_table BV where BV.inner_job_id = $innerJobId and BV.value_name not in (select JV1.value_name from ja_run_value_job_table JV1 where JV1.inner_job_id = $innerJobId)) union (select 'ja_run_value_job_table' as table_name, 'value' as value_column, JV2.inner_jobnet_id, JV2.value_name, JV2.value from ja_run_value_job_table JV2 where JV2.inner_job_id = $innerJobId ) order by value_name";
        $strSqlNotJobBefore = "select 'ja_run_value_before_table' as table_name, 'before_value' as value_column, inner_jobnet_id,value_name,before_value from ja_run_value_before_table where inner_job_id = $innerJobId order by value_name";
        $strSqlBefore = $strSqlNotJobBefore;
        if((int)$jobType == Constants::ICON_TYPE_JOB){
            $strSqlBefore = $strSqlJobBefore;
        }
        $this->db->query($strSqlBefore);
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run jobnet table data.
     *
     * @param   string $innerJobnetId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunJobnetTable($innerJobnetId) {
        if ($innerJobnetId != null)  
        {  
            $query = "WHERE inner_jobnet_id = '$innerJobnetId'";
        }
        
        $this->db->query("select * from ja_run_jobnet_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run jobnet data.
     *
     * @param   object $data
     * @param   int $runType
     * @param   string $username
     * @param   string $innerJobId
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunJobnet($data, $runType, $username, $innerJobId) {

        $this->db->query('INSERT INTO ja_run_jobnet_table (inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date, run_type, main_flag, status, start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo, execution_user_name) VALUES (:inner_jobnet_id, :inner_jobnet_main_id, :inner_job_id, :update_date, :run_type, :main_flag, :status, :start_time, :end_time, :public_flag, :multiple_start_up, :jobnet_id, :user_name, :jobnet_name, :memo, :execution_user_name)');
        
        $this->db->bind(':inner_jobnet_id', $innerJobId);
        $this->db->bind(':inner_jobnet_main_id', $innerJobId);
        $this->db->bind(':inner_job_id', 0);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':run_type', $runType);
        $this->db->bind(':main_flag', 0);
        $this->db->bind(':status', 0);
        $this->db->bind(':start_time', 0);
        $this->db->bind(':end_time', 0);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':multiple_start_up', $data['multiple_start_up']);
        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':user_name', $data['user_name']);
        $this->db->bind(':jobnet_name', $data['jobnet_name']);
        $this->db->bind(':memo', $data['memo']);
        $this->db->bind(':execution_user_name', $username);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run jobnet summary data.
     *
     * @param   string $innerJobnetId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunJobnetSummary($innerJobnetId) {
        if ($innerJobnetId != null)  
        {  
            $query = "WHERE inner_jobnet_id = '$innerJobnetId'";
        }
        
        $this->db->query("select * from ja_run_jobnet_summary_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run jobnet summary data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunJobnetSummary($data) {

        $this->db->query('INSERT INTO ja_run_jobnet_summary_table (inner_jobnet_id, update_date, run_type, invo_flag, start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo,jobnet_timeout) VALUES (:inner_jobnet_id, :update_date, :run_type, :invo_flag, :start_time, :end_time, :public_flag, :multiple_start_up, :jobnet_id, :user_name, :jobnet_name, :memo, :jobnet_timeout)');
        
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':invo_flag', $data['invo_flag']);
        $this->db->bind(':run_type', $data['run_type']);
        $this->db->bind(':start_time', $data['start_time']);
        $this->db->bind(':end_time', $data['end_time']);
        $this->db->bind(':public_flag', $data['public_flag']);
        $this->db->bind(':multiple_start_up', $data['multiple_start_up']);
        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':user_name', $data['user_name']);
        $this->db->bind(':jobnet_name', $data['jobnet_name']);
        $this->db->bind(':memo', $data['memo']);
        $this->db->bind(':jobnet_timeout', $data['jobnet_timeout']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run job data.
     *
     * @param   string $innerJobnetId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunJob($innerJobnetId) {
        if ($innerJobnetId != null)  
        {  
            $query = "WHERE inner_jobnet_id = '$innerJobnetId'";
        }
        
        $this->db->query("select * from ja_run_job_table $query order by created_date desc, job_type");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run job data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunJob($data) {

        $this->db->query('INSERT INTO ja_run_job_table (inner_job_id, inner_jobnet_id, inner_jobnet_main_id, job_type, method_flag, invo_flag, force_flag, boot_count, start_time, end_time, point_x, point_y, job_id, job_name, run_user, run_user_password) VALUES (:inner_job_id, :inner_jobnet_id, :inner_jobnet_main_id, :job_type,:method_flag, :invo_flag, :force_flag, :boot_count, :start_time, :end_time, :point_x, :point_y, :job_id, :job_name, :run_user, :run_user_password)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':inner_jobnet_main_id', $data['inner_jobnet_main_id']);
        $this->db->bind(':job_type', $data['job_type']);
        $this->db->bind(':method_flag', $data['method_flag']);
        $this->db->bind(':invo_flag', $data['invo_flag']);
        $this->db->bind(':force_flag', $data['force_flag']);
        $this->db->bind(':boot_count', $data['boot_count']);
        $this->db->bind(':start_time', $data['start_time']);
        $this->db->bind(':end_time', $data['end_time']);
        $this->db->bind(':point_x', $data['point_x']);
        $this->db->bind(':point_y', $data['point_y']);
        $this->db->bind(':job_id', $data['job_id']);
        $this->db->bind(':job_name', $data['job_name']);
        $this->db->bind(':run_user', $data['run_user']);
        $this->db->bind(':run_user_password', $data['run_user_password']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It updates the run job method.
     *
     * @param   object $data
     * @param   string $where
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateRunJobMethod($data, $where) {
        $this->db->query($this->dbUtilModel->build_sql_update("ja_run_job_table",$data, $where));

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It updates the run jobnet method.
     *
     * @param   object $data
     * @param   string $where
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateRunJobnetMethod($data, $where) {
        $this->db->query($this->dbUtilModel->build_sql_update("ja_run_jobnet_table",$data, $where));

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It updates the public flag of the jobent by jobnet id.
     *
     * @param   array $data  array of the jobnet data.
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function updateBeforeVariableMethod($data)
    {
        $this->db->query("UPDATE " . $data['table_name'] . " SET " . $data['value_column'] . "=:value WHERE inner_job_id=:inner_job_id and value_name=:value_name");
        $this->db->bind(':value', $data['value']);
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':value_name', $data['value_name']);
        if ($this->db->execute(__METHOD__)) {
            return true;
        }
    }

    /**
     * It retrieves the parameter data.
     *
     * @param   string $parameterName
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getParameterData($parameterName) {
        $strParameterVelue = "";
        $this->db->query("SELECT value FROM ja_parameter_table WHERE parameter_name = '$parameterName'");
        $data = $this->db->resultSet();
        if(count($data) == 1){
            $strParameterVelue = $data[0]->value;
            $retVal = (int) $strParameterVelue;
                if ($retVal < 0)
                {
                    $strParameterVelue = $this->getParamDefaultData($parameterName);
                }
        }else{
            $strParameterVelue = $this->getParamDefaultData($parameterName);
        }
        return $strParameterVelue;
    }

    /**
     * It retrieves the parameter default value.
     *
     * @param   string $parameterName
     * @return  string
     * @since   Method available since version 6.1.0
     */
    public function getParamDefaultData($parameterName) {
        $defaultValue = "";

            switch ($parameterName)
            {
                case "JOBNET_VIEW_SPAN":
                    $defaultValue = "60";
                    break;
                case "JOBNET_LOAD_SPAN":
                    $defaultValue = "60";
                    break;
                case "JOBNET_KEEP_SPAN":
                    $defaultValue = "60";
                    break;
                case "JOBLOG_KEEP_SPAN":
                    $defaultValue = "129600";
                    break;
                case "JOBNET_DUMMY_START_X":
                    $defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_START_Y":
                    $defaultValue = "39";
                    break;
                case "JOBNET_DUMMY_JOB_X":
                    $defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_JOB_Y":
                    $defaultValue = "93";
                    break;
                case "JOBNET_DUMMY_END_X":
                    $defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_END_Y":
                    $defaultValue = "146";
                    break;
                //added by YAMA 2014/08/18
                case "MANAGER_TIME_SYNC":
                    $defaultValue = "0";
                    break;
                case "ZBXSND_ZABBIX_IP":
                    $defaultValue = "127.0.0.1";
                    break;
                case "ZBXSND_ZABBIX_PORT":
                    $defaultValue = "10051";
                    break;
                case "ZBXSND_ZABBIX_HOST":
                    $defaultValue = "Zabbix server";
                    break;
                case "ZBXSND_ITEM_KEY":
                    $defaultValue = "jasender";
                    break;
                case "ZBXSND_SENDER":
                    $efaultValue = "zabbix_sender";
                    break;
                case "ZBXSND_RETRY":
                    $defaultValue = "0";
                    break;
                case "ZBXSND_RETRY_COUNT":
                    $defaultValue = "0";
                    break;
                case "ZBXSND_RETRY_INTERVAL":
                    $defaultValue = "5";
                    break;
            }
            return $defaultValue;
    }

    /**
     * It retrieves the run flow data.
     *
     * @param   string $innerJobnetId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunFlow($innerJobnetId) {

        if ($innerJobnetId != null)  
        {  
            $query = "WHERE inner_jobnet_id = '$innerJobnetId'";
        }
        
        $this->db->query("select * from ja_run_flow_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run flow data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunFlow($data) {

        $this->db->query('INSERT INTO ja_run_flow_table (inner_flow_id, inner_jobnet_id, start_inner_job_id, end_inner_job_id) VALUES (:inner_flow_id, :inner_jobnet_id, :start_inner_job_id, :end_inner_job_id)');
        
        $this->db->bind(':inner_flow_id', $data['inner_flow_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':start_inner_job_id', $data['start_inner_job_id']);
        $this->db->bind(':end_inner_job_id', $data['end_inner_job_id']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run end icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconEnd($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_end_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run end icon data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunIconEnd($data) {

        $this->db->query('INSERT INTO ja_run_icon_end_table (inner_job_id, inner_jobnet_id) VALUES (:inner_job_id, :inner_jobnet_id)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run extended job icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconExtended($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_extjob_table $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run if icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconIf($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_if_table $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run calculation icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconCalculation($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_calc_table $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run info icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconInfo($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select *, ' ' as calendar_name from ja_run_icon_info_table $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run jobnet icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconJobnet($innerJobId, $innerJobnetId = null) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        if ($innerJobnetId != null)  
        {  
            $query = "WHERE inner_jobnet_id = '$innerJobnetId'";
        }
        
        $this->db->query("select * from ja_run_icon_jobnet_table $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run job icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconJob($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_job_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run job icon data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunIconJob($data) {

        $this->db->query('INSERT INTO ja_run_icon_job_table (inner_job_id, inner_jobnet_id, host_flag, stop_flag, timeout, host_name, stop_code) VALUES (:inner_job_id, :inner_jobnet_id, :host_flag, :stop_flag, :timeout, :host_name, :stop_code)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':host_flag', $data['host_flag']);
        $this->db->bind(':stop_flag', $data['stop_flag']);
        $this->db->bind(':timeout', $data['timeout']);
        $this->db->bind(':host_name', $data['host_name']);
        $this->db->bind(':stop_code', $data['stop_code']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }
    
    /**
     * It retrieves the run job command data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconJobCommand($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_job_command_table $query order by command_cls");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run job command data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunJobCommand($data) {

        $this->db->query('INSERT INTO ja_run_job_command_table (inner_job_id, inner_jobnet_id, command_cls, command) VALUES (:inner_job_id, :inner_jobnet_id, :command_cls, :command)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':command_cls', $data['command_cls']);
        $this->db->bind(':command', $data['command']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run value job data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconValueJob($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_value_job_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run value job data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunValueJob($data) {

        $this->db->query('INSERT INTO ja_run_value_job_table (inner_job_id, inner_jobnet_id, value_name, value) VALUES (:inner_job_id, :inner_jobnet_id, :value_name, :value)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':value_name', $data['value_name']);
        $this->db->bind(':value', $data['value']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run value job control data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconValueJobControl($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_value_jobcon_table $query");
        return $this->db->resultSet();
    }

    /**
     * It inserts the run value job control data.
     *
     * @param   object $data
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertRunValueJobControl($data) {

        $this->db->query('INSERT INTO ja_run_value_jobcon_table (inner_job_id, inner_jobnet_id, value_name) VALUES (:inner_job_id, :inner_jobnet_id, :value_name)');
        
        $this->db->bind(':inner_job_id', $data['inner_job_id']);
        $this->db->bind(':inner_jobnet_id', $data['inner_jobnet_id']);
        $this->db->bind(':value_name', $data['value_name']);
        
        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * It retrieves the run task icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconTask($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "and jt.inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select jt.*, jjc.jobnet_name from ja_run_icon_task_table jt inner join ja_jobnet_control_table jjc on jt.submit_jobnet_id = jjc.jobnet_id $query");
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run env icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconValue($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_value_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run file transfer icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconFileTransfer($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_fcopy_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run file wait icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconFileWait($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_fwait_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run reboot icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconReboot($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_reboot_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run release icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconReleaseHold($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_release_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run zabbix icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconZabbix($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_zabbix_link_table $query");
        
        return $this->db->resultSet();
    }

    /**
     * It retrieves the run agentless icon data.
     *
     * @param   string $innerJobId
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getRunIconAgentLess($innerJobId) {
        if ($innerJobId != null)  
        {  
            $query = "WHERE inner_job_id = '$innerJobId'";
        }
        
        $this->db->query("select * from ja_run_icon_agentless_table $query");
        
        return $this->db->resultSet();
    }
}