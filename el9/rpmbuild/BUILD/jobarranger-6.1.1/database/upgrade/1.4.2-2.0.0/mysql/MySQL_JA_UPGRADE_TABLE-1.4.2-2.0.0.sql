
-- Job Arranger upgrade table SQL for MySQL (Ver 1.4.2 -> 2.0.0)  - 2014/06/23 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


CREATE INDEX `ja_run_log_idx3` ON `ja_run_log_table` (`inner_jobnet_id`,`message_id`);

CREATE TABLE `ja_icon_agentless_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `connection_method`               integer                  DEFAULT '0'     NOT NULL,
        `session_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `auth_method`                     integer                  DEFAULT '0'     NOT NULL,
        `run_mode`                        integer                  DEFAULT '0'     NOT NULL,
        `line_feed_code`                  integer                  DEFAULT '0'     NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
        `session_id`                      varchar(64)                              NULL,
        `login_user`                      varchar(256)                             NULL,
        `login_password`                  varchar(256)                             NULL,
        `public_key`                      text                                     NULL,
        `private_key`                     text                                     NULL,
        `passphrase`                      varchar(256)                             NULL,
        `host_name`                       varchar(128)                             NULL,
        `stop_code`                       varchar(32)                              NULL,
        `terminal_type`                   varchar(80)                              NULL,
        `character_code`                  varchar(80)                              NULL,
        `prompt_string`                   varchar(256)                             NULL,
        `command`                         text                                     NULL,
CONSTRAINT `ja_icon_agentless_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;
ALTER TABLE `ja_icon_agentless_table` ADD CONSTRAINT `ja_icon_agentless_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE `ja_icon_zabbix_link_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `link_target`                     integer                  DEFAULT '0'     NOT NULL,
        `link_operation`                  integer                  DEFAULT '0'     NOT NULL,
        `groupid`                         bigint unsigned          DEFAULT '0'     NOT NULL,
        `hostid`                          bigint unsigned          DEFAULT '0'     NOT NULL,
        `itemid`                          bigint unsigned          DEFAULT '0'     NOT NULL,
        `triggerid`                       bigint unsigned          DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_icon_zabbix_link_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;
ALTER TABLE `ja_icon_zabbix_link_table` ADD CONSTRAINT `ja_icon_zabbix_link_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE `ja_run_icon_agentless_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `connection_method`               integer                  DEFAULT '0'     NOT NULL,
        `session_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `auth_method`                     integer                  DEFAULT '0'     NOT NULL,
        `run_mode`                        integer                  DEFAULT '0'     NOT NULL,
        `line_feed_code`                  integer                  DEFAULT '0'     NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
        `session_id`                      varchar(64)              DEFAULT ''      NOT NULL,
        `login_user`                      varchar(256)                             NULL,
        `login_password`                  varchar(256)                             NULL,
        `public_key`                      text                                     NULL,
        `private_key`                     text                                     NULL,
        `passphrase`                      varchar(256)                             NULL,
        `host_name`                       varchar(128)                             NULL,
        `stop_code`                       varchar(32)                              NULL,
        `terminal_type`                   varchar(80)                              NULL,
        `character_code`                  varchar(80)                              NULL,
        `prompt_string`                   varchar(256)                             NULL,
        `command`                         text                                     NULL,
CONSTRAINT `ja_run_icon_agentless_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;
ALTER TABLE `ja_run_icon_agentless_table` ADD CONSTRAINT `ja_run_icon_agentless_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE `ja_run_icon_zabbix_link_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `link_target`                     integer                  DEFAULT '0'     NOT NULL,
        `link_operation`                  integer                  DEFAULT '0'     NOT NULL,
        `groupid`                         bigint unsigned          DEFAULT '0'     NOT NULL,
        `hostid`                          bigint unsigned          DEFAULT '0'     NOT NULL,
        `itemid`                          bigint unsigned          DEFAULT '0'     NOT NULL,
        `triggerid`                       bigint unsigned          DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_icon_zabbix_link_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;
ALTER TABLE `ja_run_icon_zabbix_link_table` ADD CONSTRAINT `ja_run_icon_zabbix_link_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE `ja_session_table` (
        `session_id`                      varchar(64)              DEFAULT ''      NOT NULL,
        `inner_jobnet_main_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `operation_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `status`                          integer                  DEFAULT '0'     NOT NULL,
        `force_stop`                      integer                  DEFAULT '0'     NOT NULL,
        `pid`                             integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_session_pk` PRIMARY KEY (`session_id`, `inner_jobnet_main_id`)
) ENGINE=InnoDB;


START TRANSACTION;
INSERT INTO ja_index_table (count_id, nextid) VALUES (40, 1);
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_VIEW_SPAN', '60');
COMMIT;
