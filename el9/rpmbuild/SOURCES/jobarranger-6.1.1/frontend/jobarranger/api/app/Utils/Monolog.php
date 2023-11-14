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

require_once('../vendor/autoload.php');

use DateTimeZone;
use Monolog\Handler\StreamHandler;
use Monolog\Formatter\LineFormatter;
use Monolog\Handler\RotatingFileHandler;
use Monolog\Processor\ProcessIdProcessor;
use Monolog\Logger;

/**
 * This class is used to manage log.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Monolog
{
    
    private static $maxFiles = 10;

    private static $loggerTimeFormat = "Ymd:His.v";

    /**
     *It creates application logger.
     *
     * @return Logger $appLogger
     * @since      Class available since version 6.1.0
     */
    public static function getApplicationLogger()
    {
        $serverTimezone =  shell_exec('timedatectl status | grep "zone" | sed -e "s/^[ ]*Time zone: \(.*\) (.*)$/\1/g"');
        $serverTimezone = str_replace("\n", "", $serverTimezone);
        date_default_timezone_set("Asia/Yangon");
        if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
            date_default_timezone_set($serverTimezone);
        }
        // Application Logger
        $output = "%extra.process_id%:%datetime% [%level_name%] [%context.user%] [%context.controller%()] %message% \n";
        $formatter = new LineFormatter($output, self::$loggerTimeFormat);

        $path = APPLICATION_LOG_PATH;

        $logLevel = constant('Monolog\Logger::' . strtoupper(APPLICATION_LOG_LEVEL));
        // bind it to a logger object
        $appLogger = new Logger('main');
        $logRotate = new RotatingFileHandler($path, self::$maxFiles, $logLevel);
        $logRotate->setFormatter($formatter);
        $appLogger->pushHandler($logRotate);
        $appLogger->pushProcessor(new ProcessIdProcessor());
        return $appLogger;
    }

    /**
     *It creates clean up logger.
     *
     * @return Logger $appLogger
     * @since      Class available since version 6.1.0
     */
    public static function getCleanUpLogger()
    {
        $serverTimezone =  shell_exec('timedatectl status | grep "zone" | sed -e "s/^[ ]*Time zone: \(.*\) (.*)$/\1/g"');
        $serverTimezone = str_replace("\n", "", $serverTimezone);
        date_default_timezone_set($serverTimezone);
        // Application Logger
        $output = "%extra.process_id%:%datetime% [%level_name%] %message% \n";
        $formatter = new LineFormatter($output, self::$loggerTimeFormat);
        $originalPath = APPLICATION_LOG_PATH; //production
        $lastSlashIndex = strripos($originalPath, "/");
        $path = substr($originalPath, 0, $lastSlashIndex);
        // Create a handler
        $logLevel = constant('Monolog\Logger::' . strtoupper(APPLICATION_LOG_LEVEL));
        // bind it to a logger object
        $appLogger = new Logger('main');
        // if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
            
        //     //$appLogger->setTimezone(new DateTimeZone($serverTimezone));
        // }
        $serverTimezone =  shell_exec('timedatectl status | grep "zone" | sed -e "s/^[ ]*Time zone: \(.*\) (.*)$/\1/g"');
        $serverTimezone = str_replace("\n", "", $serverTimezone);
        $logRotate = new RotatingFileHandler($path . '/cleanup.log', self::$maxFiles, $logLevel);
        $logRotate->setFormatter($formatter);
        $appLogger->pushHandler($logRotate);
        $appLogger->pushProcessor(new ProcessIdProcessor());
        return $appLogger;
    }
}
