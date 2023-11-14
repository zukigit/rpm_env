
-- Job Arranger upgrade table SQL for MySQL (Ver 2.0.0 -> 2.1.0)  - 2014/10/30 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


ALTER TABLE `ja_schedule_detail_table`        ADD COLUMN `object_flag`            integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_schedule_detail_table`  DROP PRIMARY KEY;
ALTER TABLE `ja_schedule_detail_table`    ADD CONSTRAINT `ja_schedule_detail_pk`  PRIMARY KEY (`schedule_id`,`calendar_id`,`update_date`,`boot_time`,`object_flag`);

ALTER TABLE `ja_job_control_table`            ADD COLUMN `continue_flag`          integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_job_control_table`            ADD COLUMN `run_user`               varchar(256)                      NULL;
ALTER TABLE `ja_job_control_table`            ADD COLUMN `run_user_password`      varchar(256)                      NULL;

ALTER TABLE `ja_icon_end_table`            MODIFY COLUMN `jobnet_stop_code`       varchar(256)     DEFAULT '0' NOT NULL;

ALTER TABLE `ja_icon_reboot_table`            ADD COLUMN `timeout`                integer          DEFAULT '0' NOT NULL;

ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `running_job_id`         varchar(1024)                    NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `running_job_name`       varchar(64)                      NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `virtual_time`           bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `virtual_start_time`     bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `virtual_end_time`       bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `start_pending_flag`     integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `initial_scheduled_time` bigint unsigned  DEFAULT '0' NOT NULL;

ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `running_job_id`         varchar(1024)                    NULL;
ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `running_job_name`       varchar(64)                      NULL;
ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `virtual_time`           bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `virtual_start_time`     bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `virtual_end_time`       bigint unsigned  DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_table`             ADD COLUMN `initial_scheduled_time` bigint unsigned  DEFAULT '0' NOT NULL;

ALTER TABLE `ja_run_job_table`                ADD COLUMN `continue_flag`          integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_job_table`                ADD COLUMN `run_user`               varchar(256)                     NULL;
ALTER TABLE `ja_run_job_table`                ADD COLUMN `run_user_password`      varchar(256)                     NULL;

ALTER TABLE `ja_run_icon_end_table`        MODIFY COLUMN `jobnet_stop_code`       varchar(256)     DEFAULT '0' NOT NULL;

ALTER TABLE `ja_run_icon_extjob_table`        ADD COLUMN `wait_count`             integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_icon_extjob_table`        ADD COLUMN `wait_time`              varchar(14)                      NULL;

ALTER TABLE `ja_run_icon_reboot_table`        ADD COLUMN `timeout`                integer          DEFAULT '0' NOT NULL;


CREATE TABLE `ja_filter_control_table` (
        `filter_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `valid_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `base_date_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `designated_day`                  integer                  DEFAULT '0'     NOT NULL,
        `shift_day`                       integer                  DEFAULT '0'     NOT NULL,
        `base_calendar_id`                varchar(32)              DEFAULT ''      NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `filter_name`                     varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
CONSTRAINT `ja_filter_control_pk` PRIMARY KEY (`filter_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_send_message_table` (
        `send_no`                         bigint unsigned          AUTO_INCREMENT  NOT NULL,
        `message_date`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_main_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `send_status`                     integer                  DEFAULT '0'     NOT NULL,
        `retry_count`                     integer                  DEFAULT '0'     NOT NULL,
        `retry_date`                      bigint unsigned          DEFAULT '0'     NOT NULL,
        `send_date`                       bigint unsigned          DEFAULT '0'     NOT NULL,
        `send_error_date`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `message_type`                    integer                  DEFAULT '0'     NOT NULL,
        `user_name`                       varchar(100)                             NULL,
        `host_name`                       varchar(128)                             NULL,
        `jobnet_id`                       varchar(32)                              NULL,
        `jobnet_name`                     varchar(64)                              NULL,
        `job_id`                          varchar(32)                              NULL,
        `job_id_full`                     text                                     NULL,
        `job_name`                        varchar(64)                              NULL,
        `log_message_id`                  varchar(128)                             NULL,
        `log_message`                     text                                     NULL,
CONSTRAINT `ja_send_message_pk` PRIMARY KEY (`send_no`)
) ENGINE=InnoDB;

CREATE INDEX `ja_send_message_idx1` ON `ja_send_message_table` (`send_status`);


START TRANSACTION;
INSERT INTO ja_index_table (count_id, nextid) VALUES (103, 1);

INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('MANAGER_TIME_SYNC',     '0');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('SNDMSG_KEEP_SPAN',      '1440');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ON',             '1');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_IP',      '127.0.0.1');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_PORT',    '10051');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_HOST',    'Zabbix server');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ITEM_KEY',       'jasender');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_SENDER',         'zabbix_sender');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY',          '0');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY_COUNT',    '3');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY_INTERVAL', '5');

INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ICON_STATUS');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_SCHEDULED_TIME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOB_ID_FULL');
COMMIT;
