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

use PDO, PDOException;
use App\Controllers\Pages;
use App\Utils\Constants;
use App\Exceptions\DbConnectionException;
use App\Utils\Monolog;
use Exception;

/**
 * This class is used to manage the database connection.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Database
{
    private $dsn = DATA_SOURCE_NAME;
    private $dbHost = DB_HOST;
    private $dbUser = DB_USER;
    private $dbPass = DB_PASS;
    private $dbName = DB_NAME;
    private $encoding = DATA_SOURCE_NAME == Constants::DB_MYSQL ? Constants::MYSQL_UT8_ENCODING : Constants::PGSQL_UT8_ENCODING;

    static $_instance;
    private $statement;
    private $dbHandler;
    private $error;

    protected $maxReconnectTries = 3;
    protected $reconnectErrors = [
        1317 // interrupted
        , 2002 // refused
        , 2006 // gone away
    ];
    protected $reconnectTries = 0;
    protected $reconnectDelay = 300; // in ms

    /**
     * It starts database connection.
     *
     * @param  object  $logger  
     * @throws DbConnectionException
     * @since      Class available since version 6.1.0
     */
    public function __construct($logger)
    {
        $this->logger = $logger;

        try {
            $this->getConnection();

            $this->logger->debug('DB is successfully connected.', ['controller' => __METHOD__]);
        } catch (PDOException $e) {

            $this->error = $e->getMessage();
            $this->logger->error($e->getMessage(), ['controller' => __METHOD__]);
            throw new DbConnectionException($e->getMessage(), (int) $e->getCode());
        }
    }

    /**
     * It gets connection to database.
     *     
     * @since      Class available since version 6.1.0
     */
    public function getConnection()
    {
        if (!$this->dbHandler) {
            $conn = $this->dsn . ':host=' . $this->dbHost . ';dbname=' . $this->dbName . $this->encoding;
            $options = array(
                PDO::ATTR_PERSISTENT => true,
                PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
                PDO::ATTR_CASE => PDO::CASE_NATURAL,
                PDO::ATTR_STRINGIFY_FETCHES => true
            );
            $this->dbHandler = new PDO($conn, $this->dbUser, $this->dbPass, $options);
        }
    }

    /**
     * It reconnect to database.
     *
     * @throws Exception
     * @throws DbConnectionException
     * @since      Class available since version 6.1.0
     */
    public function reconnect()
    {
        $connected = false;
        while (!$connected && $this->reconnectTries < $this->maxReconnectTries) {
            usleep($this->reconnectDelay * 1000); // micro second
            ++$this->reconnectTries;
            $this->dbHandler = null;
            try {
                $this->getConnection();
                if ($this->dbHandler) {
                    $connected = true;
                }
            } catch (Exception $e) {
            }
        }
        if (!$connected) {
            throw new DbConnectionException(Constants::DATABASE_CONNECTION_FAIL);
        }
    }

    /**
     * It allows us to write queries.
     *
     * @since      Class available since version 6.1.0
     */
    public function query($sql)
    {
        $this->statement = $this->dbHandler->prepare($sql);
    }

    /**
     * It binds value.
     *
     * @since      Class available since version 6.1.0
     */
    public function bind($parameter, $value, $type = null, bool $logFlag = true)
    {
        switch (is_null($type)) {
            case is_int($value):
                $type = PDO::PARAM_INT;
                break;
            case is_bool($value):
                $type = PDO::PARAM_BOOL;
                break;
            case is_null($value):
                $type = PDO::PARAM_NULL;
                break;
            default:
                $type = PDO::PARAM_STR;
        }
        $this->statement->bindValue($parameter, $value, $type);
        if ($logFlag) {
            $this->logger->debug('Bind Parameter ' . $parameter . ', Bind Value :' . $value, ['controller' => __METHOD__]);
        }
    }

    /**
     * It executes the prepared statement.
     *
     * @since      Class available since version 6.1.0
     */
    public function execute(bool $logFlag = true)
    {
        if ($logFlag) {
            $this->logger->debug('Preparing : ' . $this->statement->queryString, ['controller' => __METHOD__,  'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
        }

        try {
            $result = $this->statement->execute();
            return $result;
        } catch (PDOException $e) {
            throw new PDOException($e->getMessage(), (int) $e->getCode());
            $this->logger->debug($e->getMessage(), ['controller' => __METHOD__, 'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
        }
    }

    //Return an array
    public function resultSet(string $method = "", bool $bolCamelCase = false, bool $logFlag = true)
    {

        if (is_bool($method)) {
            $bolCamelCase = boolval($method);
            $method = "";
        }

        $this->execute($method);

        if ($bolCamelCase) {
            $result = $this->changeCaseInResult($this->statement->fetchAll(PDO::FETCH_OBJ));
        } else {
            $result = $this->statement->fetchAll(PDO::FETCH_OBJ);
        }
        if ($logFlag) {
            $this->logger->debug('Result : ' . count($result), ['controller' => __METHOD__,  'user' => isset($_SESSION['userInfo']['userName']) ? $_SESSION['userInfo']['userName'] : '']);
        }
        return $result;
    }

    public function resultSetAsArrayInCamelCase(string $method = "", bool $logFlag = true)
    {
        return $this->resultSet($method, true, $logFlag);
    }

    public function resultSetAsArray($objName = "stdClass")
    {
        $this->execute();
        $result = $this->statement->fetchAll(PDO::FETCH_ASSOC);
        return $result;
    }

    public function resultSetByParams($sqlQuery = "",  $sqlParams = "")
    {
        $this->query($sqlQuery);
        if ($sqlParams == "") {
            $this->statement->execute();
        } else {
            $this->statement->execute($sqlParams);
        }

        $result = $this->changeCaseInResult($this->statement->fetchAll(PDO::FETCH_OBJ));

        return $result;
    }

    public function singleValueByParam($sqlQuery = "",  $sqlParam = "")
    {
        $this->query($sqlQuery);
        if ($sqlParam == "") {
            $this->statement->execute();
        } else {
            $this->bind(":param", $sqlParam);
            $this->statement->execute();
        }

        $result = $this->statement->fetchAll(PDO::FETCH_COLUMN);
        return (isset($result[0]) ? $result[0] : "");
    }

    //Return a specific row as an object
    public function single(string $method = "", bool $bolCamelCase = false)
    {

        if (is_bool($method)) {
            $bolCamelCase = boolval($method);
            $method = "";
        }

        $this->execute($method);

        if ($bolCamelCase) {
            $result = $this->changeCaseInSingle($this->statement->fetch(PDO::FETCH_OBJ));
        } else {
            $result = $this->statement->fetch(PDO::FETCH_OBJ);
        }
        return $result;
    }

    public function singleAsArrayInCamelCase(string $method = "")
    {
        return $this->single($method, true);
    }

    //Get's the row count
    public function rowCount($method = "", bool $logFlag = true)
    {
        $result = $this->statement->rowCount();
        if ($logFlag) {
            $this->logger->debug('Result : ' . $result, ['controller' => $method]);
        }

        return $result;
    }

    private function changeCaseInResult($result)
    {

        $resultArray = array();

        foreach ($result as $k => $v) {
            $arr = json_decode(json_encode($v), true);
            foreach ($arr as $kk => $vv) {
                $newKK = $this->underscoreToCamelCase($kk);
                $arr = $this->replaceArrayKey($arr, $kk, $newKK);
            }
            array_push($resultArray, $arr);
        }

        return $resultArray;
    }
    private function changeCaseInSingle($result)
    {

        $resultArray = array();
        if (false != $result) {
            $arr = json_decode(json_encode($result), true);
            foreach ($arr as $kk => $vv) {
                $newKK = $this->underscoreToCamelCase($kk);
                $arr = $this->replaceArrayKey($arr, $kk, $newKK);
            }
            array_push($resultArray, $arr);
        }
        return $resultArray;
    }

    private function underscoreToCamelCase($string, $capitalizeFirstCharacter = false)
    {
        $str = str_replace('_', '', ucwords($string, '_'));
        if (!$capitalizeFirstCharacter) {
            $str = lcfirst($str);
        }
        return $str;
    }

    private function replaceArrayKey($array, $oldKey, $newKey)
    {
        //Get a list of all keys in the array.
        $arrayKeys = array_keys($array);
        //Replace the key in our $arrayKeys array.
        $oldKeyIndex = array_search($oldKey, $arrayKeys);
        $arrayKeys[$oldKeyIndex] = $newKey;
        //Combine them back into one array.
        $newArray =  array_combine($arrayKeys, $array);
        return $newArray;
    }

    public function getDbHandler()
    {
        return $this->dbHandler;
    }

    public function getError()
    {
        return $this->error;
    }

    public function beginTransaction()
    {
        return $this->dbHandler->beginTransaction();
    }

    public function commit()
    {
        return $this->dbHandler->commit();
    }

    public function rollback()
    {
        return $this->dbHandler->rollBack();
    }

    public function inTransaction()
    {
        return $this->dbHandler->inTransaction();
    }
}
