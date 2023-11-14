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

namespace App\Controllers;

use App\Utils\Controller;
use App\Utils\Monolog;
use App\Utils\Util;
use App\Utils\Constants;

use Exception, DateTime;
use PDO, PDOException;

include '../app/Controllers/SetupRequire.php';
/**
 * This controller is used to manage setup.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Setup extends Controller
{

    private $frontend_setup;

    public function __construct()
    {
        parent::__construct();
        // $this->logger = Core::logger();
        // $this->logger = Monolog::getApplicationLogger();
        $this->frontend_setup = new SetupRequire();
    }

    /**
     * Check OK, setup can continue.
     */
    const CHECK_OK = 1;

    /**
     * Check failed, but setup can still continue. A warning will be displayed.
     */
    const CHECK_WARNING = 2;

    /**
     * Check failed, setup cannot continue. An error will be displayed.
     */
    const CHECK_FATAL = 3;

    /**
     * It redirects to setup screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function iniSetup()
    {
        $this->view('screens/setup');
    }

    /**
     * Check setup initial.
     *
     * @since   Method available since version 6.1.0
     */
    public function Initial()
    {
        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => Constants::SERVICE_MESSAGE_SUCCESS]);
    }

    /**
     * It checks setup requirement.
     *
     * @since   Method available since version 6.1.0
     */
    public function PreRequirement()
    {
        $finalResult = SetupRequire::CHECK_OK;
        // $rows = "";
        $returnMsg = "";
        $results = $this->frontend_setup->checkRequirements();
        foreach ($results as $req) {
            if ($req['result'] == SetupRequire::CHECK_OK) {
                $class = 'pass';
                $result = 'OK';
            } elseif ($req['result'] == SetupRequire::CHECK_WARNING) {
                $class = 'warning';
                $result = 'Warning';
            } else {
                $class = 'fail';
                $result = ('Fail');
                $returnMsg .= "-" . $req['error'] . "<br>";
            }

            if ($req['result'] > $finalResult) {
                $finalResult = $req['result'];
            }
        }

        $data = [
            'check' => $results,
            'finalresult' => $finalResult,
            'returnMsg' => $returnMsg
        ];
        echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $data]);
    }

    /**
     * It gets new database connection.
     *
     * @since   Method available since version 6.1.0
     */
    public function DBConnection()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = (object) json_decode($req_raw, true)['params'];
            $dsn = $data->DBType;
            $DBHost = $data->DBHost;
            $DBname = $data->DBName;
            $DBUser = $data->DBUser;
            $DBPass = $data->DBPass;
            $encoding = $dsn == Constants::DB_MYSQL ? Constants::MYSQL_UT8_ENCODING : Constants::PGSQL_UT8_ENCODING;

            $conn = $dsn . ':host=' . $DBHost . ';dbname=' . $DBname . $encoding;
            $options = array(
                PDO::ATTR_PERSISTENT => true,
                PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
                PDO::ATTR_CASE => PDO::CASE_NATURAL
            );
            try {
                $dbHandler = new PDO($conn, $DBUser, $DBPass, $options);
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "success"]);
            } catch (PDOException $e) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "connectionFail"]);
            }
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "request data is corrupted or missing."]);
        }
    }

    /**
     * It checks the zabbix API.
     *
     * @since   Method available since version 6.1.0
     */
    public function zabbixApiCheck()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = (object) json_decode($req_raw, true)['params'];
            $url  = $data->zabbixURL . '/api_jsonrpc.php';
            $ch = curl_init($url);

            $payload = json_encode(array(
                "jsonrpc" => "2.0",
                "method" => "apiinfo.version",
                "params" => [],
                "id" => 1
            ));

            // Attach encoded JSON string to the POST fields
            curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);

            // Set the content type to application/json
            curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type:application/json'));

            // Return response instead of outputting
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

            // Execute the POST request
            $result = curl_exec($ch);

            // Close cURL resource
            curl_close($ch);

            $final = json_decode($result);
            if ($final == null) {
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "zbxnotfound"]);
            } else {
                $version = $final->result;
                if (substr($version, 0, 1) === '6') {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "success"]);
                } else {
                    echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_MESSAGE => "version"]);
                }
            }
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "error"]);
        }
    }

    /**
     * It checks log file path and permission.
     *
     * @since   Method available since version 6.1.0
     */
    public function logPathCheck()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = (object) json_decode($req_raw, true)['params'];
            // $file = substr($data->appLog, $dirEnd + 1);
            $applog = $data->appLog;
            // $wslog = $para['wslog'] . '/socket.log';
            $message = "";
            $flag = 'success';
            try {
                $dirname = dirname($applog);
                // (!file_exists($file) && is_writable(dirname($file)))
                if (is_dir($dirname)) {
                    if (!is_writable($dirname)) {
                        $flag = 'fail';
                        $message .= "Given directory don't have permission to create a file<br>";
                    } else {
                        if (file_exists($applog) && (!is_writable($dirname))) {
                            $flag = 'fail';
                            $message .= "app.log already exist and cannot be update<br>";
                        }
                    }
                } else {
                    $flag = 'fail';
                    $message .= "Given directory not found<br>";
                }

                // $dirname = dirname($wslog);
                // if (is_dir($dirname)) {
                //     if (!is_writable($dirname)) {
                //         $flag = 'fail';
                //         $message .= "Given directory don't have permission to create socket.log<br>";
                //     } else {
                //         if (file_exists($wslog) && (!is_writable($dirname))) {
                //             $flag = 'fail';
                //             $message .= "wslog.log already exist and cannot be update<br>";
                //         }
                //     }
                // } else {
                //     $flag = 'fail';
                //     $message .= "No directory found for socket.log<br>";
                // }

                $data = [
                    'result' => $flag,
                    'resultMessage' => $message,
                ];
                echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $data]);
            } catch (Exception $e) {
                echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "error"]);
            };
        } else {
            echo Util::response(Constants::API_RESPONSE_TYPE_INCOMPLETE, [Constants::API_RESPONSE_MESSAGE => "error"]);
        }
    }

    /**
     * It creates config file.
     *
     * @since   Method available since version 6.1.0
     */
    public function configCreated()
    {
        $req_raw = file_get_contents('php://input');
        if (isset($req_raw)) {
            $data = (object) json_decode($req_raw, true)['params'];
            $DBType = $data->DBType;
            $DBHost = $data->DBHost;
            $DBname = $data->DBName;
            $DBUser = $data->DBUser;
            $DBPass = $data->DBPass;
            $zabbixURL = $data->zabbixURL;
            $applog = $data->appLog;

            $firstText = "<?php
//Database parmams
define('DATA_SOURCE_NAME' , '$DBType'); //mysql=MySQL,pgsql=PostgreSQL
define('DB_HOST' , '" . $DBHost . "');
define('DB_USER' , '" . $DBUser . "'); 
define('DB_PASS' , '" . $DBPass . "'); 
define('DB_NAME' , '" . $DBname . "');

//ZABBIX API URL
define('ZBX_API_ROOT', '$zabbixURL');

//Application Log Path (Dynamic links)
define('APPLICATION_LOG_PATH', '$applog');

//Application Log Level (Dynamic)
define('APPLICATION_LOG_LEVEL', 'INFO');

//Permission To access SETUP screen 0=No 1=Yes
define('CONFIG_CREATION_PERMISSION',1);
";
            $result = "success";
            $msg = "";
            $req = $this->saveConfig($firstText);
            if (!is_null($req)) {
                $result = "fail";
                $msg = $req;
            }
            // else {
            //     if (!file_exists($applog)) {
            //         //file_put_contents($applog, "");
            //     }
            // }
            $data = [
                'result' => $result,
                'resultMessage' => $msg
            ];

            echo Util::response(Constants::API_RESPONSE_TYPE_OK, [Constants::API_RESPONSE_DATA => $data]);
        }
    }

    /**
     * It creates config file.
     *
     * @param  string $firstText content of config file.
     * @throws Exception
     * @return null|string could be null if success, could be string if fail
     * @since  Method available since version 6.1.0
     */
    public function saveConfig($firstText)
    {
        try {
            $result = null;
            if (Constants::PROJECT_ENV == Constants::PRODUCTION_ENV) {
                $file =   '/etc/jobarranger/web/jam.config.php';
            } else {
                $file =   '../app/config/jam.config.php';
            }

            if (is_null($file)) {
                $result = 'Cannot save, config file is not set.';
            }

            if (is_link($file)) {
                $file = readlink($file);
            }

            $dirname = dirname($file);
            if (is_dir($dirname)) {
                if (!is_writable($dirname)) {
                    $result .= "'/etc/jobarranger/web' directory don't have permission to create jam.config.php";
                } else {
                    $file_is_writable = ((!file_exists($file) && is_writable(dirname($file))) || is_writable($file));

                    if ($file_is_writable && file_put_contents($file, $firstText)) {
                        if (!chmod($file, 0600)) {
                            $result = 'Unable to change configuration file permissions to 0600.';
                        }
                    } elseif (is_readable($file)) {
                        if (file_get_contents($file) !== $firstText) {
                            $result = 'Unable to overwrite the existing configuration file.';
                        }
                    } else {
                        $result = 'Unable to create the configuration file.';
                    }
                }
            } else {
                $result = "/etc/jobarranger/web/ directory do not exist";
            }

            return $result;
        } catch (Exception $e) {
            return $e->getMessage();
        }
    }
}

define('ZBX_KIBIBYTE',    '1024');
define('ZBX_MEBIBYTE',    '1048576');
define('ZBX_GIBIBYTE',    '1073741824');
define('ZBX_TEBIBYTE',    '1099511627776');

define('ZBX_UNITS_ROUNDOFF_SUFFIXED',        2);
define('ZBX_UNITS_ROUNDOFF_UNSUFFIXED',        4);

define('ZBX_DB_MYSQL',        'MYSQL');
define('ZBX_DB_ORACLE',        'ORACLE');
define('ZBX_DB_POSTGRESQL',    'POSTGRESQL');

function _s($string)
{
    $arguments = array_slice(func_get_args(), 1);

    return _params(_($string), $arguments);
}

/**
 * Returns a formatted string.
 *
 * @param string $format		receives already stranlated string with format
 * @param array  $arguments		arguments to replace according to given format
 *
 * @return string
 */
function _params($format, array $arguments)
{
    return vsprintf($format, $arguments);
}

/**
 * Convert suffixed string to decimal bytes ('10K' => 10240).
 * Note: this function must not depend on optional PHP libraries, since it is used in Zabbix setup.
 *
 * @param string $value
 *
 * @return int
 */
function str2mem($value)
{
    $value = trim($value);
    $suffix = strtoupper(substr($value, -1));

    if (ctype_digit($suffix)) {
        return (int) $value;
    }

    $value = (int) substr($value, 0, -1);

    if ($suffix === 'G') {
        $value *= ZBX_GIBIBYTE;
    } elseif ($suffix === 'M') {
        $value *= ZBX_MEBIBYTE;
    } elseif ($suffix === 'K') {
        $value *= ZBX_KIBIBYTE;
    }

    return $value;
}

/**
 * Convert decimal bytes to suffixed string (10240 => '10K').
 * Note: this function must not depend on optional PHP libraries, since it is used in Zabbix setup.
 *
 * @param int $bytes
 *
 * @return string
 */
function mem2str($bytes)
{
    if ($bytes > ZBX_GIBIBYTE) {
        return round($bytes / ZBX_GIBIBYTE, ZBX_UNITS_ROUNDOFF_SUFFIXED) . 'G';
    } elseif ($bytes > ZBX_MEBIBYTE) {
        return round($bytes / ZBX_MEBIBYTE, ZBX_UNITS_ROUNDOFF_SUFFIXED) . 'M';
    } elseif ($bytes > ZBX_KIBIBYTE) {
        return round($bytes / ZBX_KIBIBYTE, ZBX_UNITS_ROUNDOFF_SUFFIXED) . 'K';
    } else {
        return round($bytes) . 'B';
    }
}

/**
 * Translates the string with respect to the given context.
 * If no translation is found, the original string will be used.
 *
 * Example: _x('Message', 'context');
 * returns: 'Message'
 *
 * @param string $message		string to translate
 * @param string $context		context of the string
 *
 * @return string
 */
function _x($message, $context)
{
    return ($context == '')
        ? _($message)
        : pgettext($context, $message);
}

/**
 * Translates the string with respect to the given context.
 *
 * @see _x
 *
 * @param string $context
 * @param string $msgId
 *
 * @return string
 */
function pgettext($context, $msgId)
{
    $contextString = $context . "\004" . $msgId;
    $translation = _($contextString);

    return ($translation == $contextString) ? $msgId : $translation;
}

/**
 * Verify that function exists and can be called as a function.
 *
 * @param array		$names
 *
 * @return bool
 */
function zbx_is_callable(array $names)
{
    foreach ($names as $name) {
        if (!is_callable($name)) {
            return false;
        }
    }

    return true;
}
