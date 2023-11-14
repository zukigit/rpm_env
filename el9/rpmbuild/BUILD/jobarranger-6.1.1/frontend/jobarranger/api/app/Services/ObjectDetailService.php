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

namespace App\Services;

use App\Utils\Service;
use App\Utils\Core;
use App\Utils\Constants;
use App\Models\ObjectLockModel;
use App\Models\CalendarModel;
use App\Models\FilterModel;
use App\Models\JobnetModel;
use App\Models\ScheduleModel;
use App\Models\UserModel;
use Datetime, DateInterval;
use PDOException;

/**
 * This service is used to manage the object detail services.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class ObjectDetailService extends Service
{

    public function __construct()
    {
        parent::__construct();
        $this->objectLockModel = new ObjectLockModel();
        $this->logger = Core::logger();
        $this->calendarModel = new CalendarModel();
        $this->filterModel = new FilterModel();
        $this->scheduleModel = new ScheduleModel();
        $this->jobnetModel = new JobnetModel();
        $this->userModel = new UserModel();
    }

    /**
     * It retrieves detail information of object.
     *
     * @param   string $id     id of the object.
     * @param   string $type   object type calendar|filter|schedule|jobnet
     * @param   string $updateDate
     * @throws  PDOException
     * @return  array|string could be array if success, could be string if fail
     * @since   Method available since version 6.1.0
     */
    public function getSingleObject($id, $type, $updateDate)
    {
        //$this->logger->debug('Get detail information of object process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            switch ($type) {
                case Constants::OBJECT_TYPE_CALENDAR:
                    $model = Constants::CALENDAR_MODEL;
                    break;
                case Constants::OBJECT_TYPE_FILTER:
                    $model = Constants::FILTER_MODEL;
                    break;
                case Constants::OBJECT_TYPE_SCHEDULE:
                    $model = Constants::SCHEDULE_MODEL;
                    break;
                case Constants::OBJECT_TYPE_JOBNET:
                    $model = Constants::JOBNET_MODEL;
                    break;
            }
            $detail = $this->$model->each($id, $updateDate);
            //$detail = $this->$model->each($id, $updateDate);
            if ($detail == false) {
                //$this->logger->error('Object is not found.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
                return Constants::SERVICE_MESSAGE_ITEM_NOT_FOUND;
            }
            return $detail;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage(), (int) $e->getCode());
        }
    }

    /**
     * It retrieves object lists on search data.
     *
     * @param   string $pageObject   object type calendar|filter|schedule|jobnet
     * @param   int    $publicFlag   public=1; private=0
     * @param   string $sort
     * @param   string $limit
     * @param   string $search
     * @throws  PDOException
     * @return  array|string could be array if success, could be string if fail
     * @since   Method available since version 6.1.0
     */
    public function getLazyObjectData($pageObject, $publicFlag, $search)
    {
        $this->logger->debug('Check editable service is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
        try {
            $this->logger->info('Select ' . $pageObject . ' object list process is started.', ['controller' => __METHOD__, 'user' => $_SESSION['userInfo']['userName']]);
            switch ($pageObject) {
                case "calendar":
                    $result = $this->calendarModel->getData($publicFlag, $search);
                    break;
                case "filter":
                    $result = $this->filterModel->getData($publicFlag, $search);
                    break;
                case "schedule":
                    $result = $this->scheduleModel->getData($publicFlag, $search);
                    break;
                case "jobnet":
                    $result = $this->jobnetModel->getData($publicFlag, $search);
                    break;
            }
            return $result;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage(), (int) $e->getCode());
        }
    }
}
