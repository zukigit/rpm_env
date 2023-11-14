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

use App\Utils\Database;
use App\Utils\Monolog;
use App\Controllers\Pages;
use App\Exceptions\DbConnectionException;
use Exception;

/**
 * This class is used to manage the log and db.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Core
{

  private static $db;
  private static $logger;

  public function __construct()
  {
    try {
      static::$logger = Monolog::getApplicationLogger();
      static::$db = new Database(static::$logger);
    } catch (DbConnectionException $e) {
      throw new DbConnectionException($e->getMessage());
    }catch (Exception $e){
      throw new Exception($e->getMessage());
    }
  }

  public static function db()
  {
    return static::$db;
  }

  public static function logger()
  {
    return static::$logger;
  }
}
