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
 * This model is used to manage the index.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class IndexModel extends Model
{
    /**
     * It retrieves the next object id and increase 1.
     *
     * @param   int    $id      object id 
     * @return  string $nextId  last object id
     * @since   Method available since version 6.1.0
     */
    public function getNextIdAndIncrease($id)
    {
        $nextId = $this->getNextID($id);
        if (!$this->increateNextID($id, 1)) {
            $nextId = false;
        }
        return $nextId;
    }

    /**
     * It retrieves the next object id.
     *
     * @param   int $id  object id count
     * @return  string last object id
     * @since   Method available since version 6.1.0
     */
    public function getNextID($id)
    {
        $this->db->query("SELECT nextid FROM ja_index_table WHERE count_id = '$id' for update");

        return $this->db->single();
    }

    /**
     * It increase the object id after retrieve next id
     *
     * @param   int $id    object id count
     * @param   int $plus  
     * @return  bool could be true if increase process success, could be false if fail
     * @since   Method available since version 6.1.0
     */
    public function increateNextID($id, $plus = null)
    {
        if ($plus == null) {
            $plus = 1;
        }
        $this->db->query('UPDATE ja_index_table SET nextid = nextid + :plus WHERE count_id = :id');

        $this->db->bind(':id', $id);
        $this->db->bind(':plus', $plus);

        if ($this->db->execute()) {
            return true;
        } else {
            return false;
        }
    }
}
