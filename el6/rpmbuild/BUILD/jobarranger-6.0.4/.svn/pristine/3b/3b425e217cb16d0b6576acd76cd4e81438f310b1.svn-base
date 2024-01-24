
-- Job Arranger upgrade table SQL for MySQL (Ver 6.0.3)  - 2024/01/16 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.

DROP PROCEDURE IF EXISTS DropIndexIfExists;
DROP PROCEDURE IF EXISTS CreateIndexIfNotExists;
DROP PROCEDURE IF EXISTS CreateUniqueIndexIfNotExists;

DELIMITER //

-- Check if the index exists and drop it if it is
CREATE PROCEDURE DropIndexIfExists(IN dbName VARCHAR(255), IN tableName VARCHAR(255), IN indexName VARCHAR(255))
BEGIN
    DECLARE stmt VARCHAR(1000);
    
    -- Check if the index exists
    IF EXISTS (SELECT * FROM information_schema.statistics WHERE table_schema = dbName AND table_name = tableName AND index_name = indexName) THEN
        -- If the index exists, drop it using dynamic SQL
        SET @stmt = CONCAT('DROP INDEX ', indexName, ' ON ', dbName, '.', tableName);
        PREPARE drop_index_stmt FROM @stmt;
        EXECUTE drop_index_stmt;
        DEALLOCATE PREPARE drop_index_stmt;
    END IF;
END;

-- Check if the index exists and create it if not
CREATE PROCEDURE CreateIndexIfNotExists(tableName VARCHAR(255), indexName VARCHAR(255), columnList VARCHAR(255))
BEGIN
  SET @indexQuery = CONCAT('
    CREATE INDEX ', indexName, ' ON ', tableName, ' (', columnList, ');
  ');

  IF NOT EXISTS (
    SELECT 1
    FROM information_schema.statistics
    WHERE table_name = tableName
    AND index_name = indexName
  ) THEN
    PREPARE stmt FROM @indexQuery;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
  END IF;
END;

CREATE PROCEDURE CreateUniqueIndexIfNotExists(tableName VARCHAR(255), indexName VARCHAR(255), columnList VARCHAR(255))
BEGIN
  SET @indexQuery = CONCAT('
    CREATE UNIQUE INDEX ', indexName, ' ON ', tableName, ' (', columnList, ');
  ');

  IF NOT EXISTS (
    SELECT 1
    FROM information_schema.statistics
    WHERE table_name = tableName
    AND index_name = indexName
  ) THEN
    PREPARE stmt FROM @indexQuery;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
  END IF;
END;
//
DELIMITER ;

CALL DropIndexIfExists('zabbix', 'ja_run_job_table', 'ja_run_job_idx3');
CALL CreateIndexIfNotExists('ja_run_job_table', 'ja_run_job_idx3', 'job_type, status');
CALL CreateIndexIfNotExists('ja_run_icon_job_table', 'ja_run_icon_job_idx1', 'host_flag, host_name');
CALL CreateUniqueIndexIfNotExists('ja_run_icon_job_table', 'ja_run_icon_job_idx2', 'inner_job_id, host_flag');
CALL CreateUniqueIndexIfNotExists('ja_run_value_before_table', 'ja_run_value_before_idx2', 'inner_job_id, value_name, seq_no');
CALL CreateUniqueIndexIfNotExists('ja_run_icon_reboot_table', 'ja_run_icon_reboot_idx2', 'inner_job_id, host_flag');
