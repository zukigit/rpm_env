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
 * This model is used to manage the define extended job table.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class DefineExtendedJobModel extends Model
{

    /**
     * It retrieve the define extended job data.
     *
     * @return  array
     * @since   Method available since version 6.1.0
     */
    public function getDefineExtendedJob($lang)
    {
        $this->db->query("SELECT command_id, command_name, memo FROM ja_define_extjob_table WHERE lang = '$lang' order by command_id ASC");

        return $this->db->resultSet();
    }
}
