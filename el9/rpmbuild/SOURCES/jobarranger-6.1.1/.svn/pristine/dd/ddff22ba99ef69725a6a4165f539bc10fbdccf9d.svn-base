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
 * This model is used to manage the flow.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class FlowModel extends Model
{
    /**
     * It inserts the flow data.
     *
     * @param   object
     * @return  bool
     * @since   Method available since version 6.1.0
     */
    public function insertFlow($data)
    {
        $this->db->query('INSERT INTO ja_flow_control_table (jobnet_id, update_date, start_job_id, end_job_id, flow_type, flow_width, flow_style) VALUES (:jobnet_id, :update_date, :start_job_id, :end_job_id, :flow_type, :flow_width, :flow_style)');

        $this->db->bind(':jobnet_id', $data['jobnet_id']);
        $this->db->bind(':update_date', $data['update_date']);
        $this->db->bind(':start_job_id', $data['start_job_id']);
        $this->db->bind(':end_job_id', $data['end_job_id']);
        $this->db->bind(':flow_type', $data['flow_type']);
        $this->db->bind(':flow_width', $data['flow_width']);
        $this->db->bind(':flow_style', $data['flow_style']);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }
}
