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

require_once('../app/Utils/Constants.php');
require_once('../app/Utils/Monolog.php');

use App\Exceptions\DbConnectionException;
use App\Utils\Constants;
use App\Utils\Database;
use PDO as pdo;
use PDOException as pdoException;
use App\Utils\Monolog;
use App\Utils\Util;

pcntl_async_signals(true);
pcntl_signal(SIGTERM, "signal_handler");
pcntl_signal(SIGINT, "signal_handler");

function signal_handler($signal)
{
  $logger = Monolog::getCleanUpLogger();
  switch ($signal) {
    case SIGTERM:
      $logger->info("jam-cleanup service is stopped. Job Arranger " . Constants::APP_VERSION . " (revision " . Constants::REVISION . ").");
      exit;
    case SIGINT:
      exit;
  }
}


expiredLockObjectClean();

/**
 * This function is used to delete the expired locked object data from ja_object_lock_table.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
function expiredLockObjectClean()
{
  if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
    $configPath = "/etc/jobarranger/web/jam.config.php";
  } else {
    $configPath = "../app/config/jam.config.php";
  }

  $selectParameterSql = "SELECT value FROM ja_parameter_table WHERE parameter_name = :param";

  if (file_exists($configPath)) {
    try {
      require_once $configPath;

      if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
        $serverTimezone =  shell_exec('timedatectl status | grep "zone" | sed -e "s/^[ ]*Time zone: \(.*\) (.*)$/\1/g"');
        $serverTimezone = str_replace("\n", "", $serverTimezone);
        $serverTimezone = new DateTimeZone($serverTimezone);
      } else {
        $serverTimezone = new DateTimeZone('Asia/Yangon');
      }
      $logger = Monolog::getCleanUpLogger();
      $logger->info("jam-cleanup service is started. Job Arranger " . Constants::APP_VERSION . " (revision " . Constants::REVISION . ").");

      $db = new Database($logger);
      $dbHandler = $db->getDbHandler();

      $utils = new Util();

      $selectStatement = $dbHandler->prepare($selectParameterSql);
      $selectStatement->bindValue(":param", "OBJECT_LOCK_EXPIRED_TIME", pdo::PARAM_STR);
      $writeLog = 0;
      while (true) {
        $today = gmdate("Y-m-d  H:i:s");
        // $curHour = intval(gmdate("H"));
        $date = new DateTime("now", $serverTimezone);
        $curHour = intval($date->format('H'));
        if ($curHour == 0 || ($curHour % 4) == 0) {
          if ($writeLog == 0) {
            $logger->info('Expired Object Lock cleaning service is running OK, deleting in every minute if Expired exists.');
            $writeLog = 1;
          }
        } else {
          $writeLog = 0;
        }

        $selectStatement->execute();
        $selectResult = $selectStatement->fetchAll(pdo::FETCH_COLUMN);
        if (DATA_SOURCE_NAME == Constants::DB_MYSQL) {
          // $selectExpiredQuery = "SELECT * FROM ja_object_lock_table";
          $selectExpiredQuery = "SELECT * FROM ja_object_lock_table WHERE TIMESTAMPDIFF(second, last_active_time,'" . $today . "' ) >" . (intval($selectResult[0]) * 60);
        } else {
          // $selectExpiredQuery = "SELECT * FROM ja_object_lock_table";
          $selectExpiredQuery = "SELECT * FROM ja_object_lock_table WHERE EXTRACT(EPOCH FROM (now() at time zone ('utc') - last_active_time)) > " . (intval($selectResult[0]) * 60);
        }

        $selectExpiredStatemenet = $dbHandler->prepare($selectExpiredQuery);
        $selectExpiredStatemenet->execute();
        $selectExpiredResult = $selectExpiredStatemenet->fetchAll(pdo::FETCH_OBJ);

        if (count($selectExpiredResult) > 0) {
          foreach ($selectExpiredResult as $expiredObject) {
            $deleteStatement = $dbHandler->prepare("DELETE FROM ja_object_lock_table WHERE object_id = '" . $expiredObject->object_id . "' and object_type = " . $expiredObject->object_type);
            $deleteStatement->execute();
            $result = $deleteStatement->rowCount();
            if ($result) {
              $lastActiveTime = new DateTime($expiredObject->last_active_time);
              $lastActiveTime->setTimeZone($serverTimezone);
              $lastActiveTime = $lastActiveTime->format('Y-m-d H:i:s');
              $objectType = $utils::getLockObjectType((int)$expiredObject->object_type);
              $logger->info("Deleted Expired Object Lock:(Object type : $objectType, Object id : $expiredObject->object_id, Username : $expiredObject->username, IP Address : $expiredObject->attempt_ip, last active time : $lastActiveTime) ");
            }
          }
        }

        flush();
        if (ob_get_level() > 0) {
          ob_flush();
        }
        sleep(60);
      }
    } catch (pdoException $e) {
      echo $e->getMessage();
      $logger->error($e->getMessage());
      if ($e->getCode() == "HY000") {
        $logger->info("jam-cleanup service is stopped. Job Arranger " . Constants::APP_VERSION . " (revision " . Constants::REVISION . ").");
      }
    } catch (DbConnectionException $e) {
      echo $e->getMessage();
      $logger->error($e->getMessage());
    }
  } else {
    echo "jam.config file doesnt exist";
  }
}
