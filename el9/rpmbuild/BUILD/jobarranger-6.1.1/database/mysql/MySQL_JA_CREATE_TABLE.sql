
-- Job Arranger create table SQL for MySQL  - 2014/10/30 -

-- Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


CREATE TABLE `ja_calendar_control_table` (
        `calendar_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `valid_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `calendar_name`                   varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
CONSTRAINT `ja_calendar_control_pk` PRIMARY KEY (`calendar_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_calendar_detail_table` (
        `calendar_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `operating_date`                  integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_calendar_detail_pk` PRIMARY KEY (`calendar_id`,`update_date`,`operating_date`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_schedule_control_table` (
        `schedule_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `valid_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `schedule_name`                   varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
CONSTRAINT `ja_schedule_control_pk` PRIMARY KEY (`schedule_id`,`update_date`)
) ENGINE=InnoDB;

CREATE INDEX `ja_schedule_control_idx1` ON `ja_schedule_control_table` (`valid_flag`);


CREATE TABLE `ja_schedule_detail_table` (
        `schedule_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `calendar_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `boot_time`                       char(4)                  DEFAULT ''      NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `object_flag`                     integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_schedule_detail_pk` PRIMARY KEY (`schedule_id`,`calendar_id`,`update_date`,`boot_time`,`object_flag`)
) ENGINE=InnoDB;


CREATE TABLE `ja_schedule_jobnet_table` (
        `schedule_id`                     varchar(32)              DEFAULT ''      NOT NULL,
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_schedule_jobnet_pk` PRIMARY KEY (`schedule_id`,`jobnet_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_jobnet_control_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `valid_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `multiple_start_up`               integer                  DEFAULT '0'     NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `jobnet_name`                     varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
        `jobnet_timeout`                  integer                  DEFAULT '0'     NOT NULL,
        `timeout_run_type`                integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_jobnet_control_pk` PRIMARY KEY (`jobnet_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_job_control_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `job_type`                        integer                  DEFAULT '0'     NOT NULL,
        `point_x`                         integer                  DEFAULT '0'     NOT NULL,
        `point_y`                         integer                  DEFAULT '0'     NOT NULL,
        `job_name`                        varchar(64)                              NULL,
        `method_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `force_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `continue_flag`                   integer                  DEFAULT '0'     NOT NULL,
        `run_user`                        varchar(256)                             NULL,
        `run_user_password`               varchar(256)                             NULL,
CONSTRAINT `ja_job_control_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_flow_control_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `start_job_id`                    varchar(32)              DEFAULT ''      NOT NULL,
        `end_job_id`                      varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `flow_type`                       integer                  DEFAULT '0'     NOT NULL,
        `flow_width`                      integer                  DEFAULT '0'     NOT NULL,
		`flow_style`					  varchar(400)		   	   DEFAULT NULL,
CONSTRAINT `ja_flow_control_pk` PRIMARY KEY (`jobnet_id`,`start_job_id`,`end_job_id`,`update_date`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_icon_calc_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `hand_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `formula`                         varchar(100)             DEFAULT ''      NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
CONSTRAINT `ja_icon_calc_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_end_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `jobnet_stop_flag`                integer                  DEFAULT '0'     NOT NULL,
        `jobnet_stop_code`                varchar(256)             DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_icon_end_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_extjob_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `command_id`                      varchar(32)              DEFAULT ''      NOT NULL,
        `value`                           text                                     NULL,
CONSTRAINT `ja_icon_extjob_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_fcopy_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `from_host_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `to_host_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `overwrite_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `from_host_name`                  varchar(128)             DEFAULT ''      NOT NULL,
        `from_directory`                  text                                     NOT NULL,
        `from_file_name`                  text                                     NOT NULL,
        `to_host_name`                    varchar(128)             DEFAULT ''      NOT NULL,
        `to_directory`                    text                                     NOT NULL,
CONSTRAINT `ja_icon_fcopy_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_fwait_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `fwait_mode_flag`                 integer                  DEFAULT '0'     NOT NULL,
        `file_delete_flag`                integer                  DEFAULT '0'     NOT NULL,
        `file_wait_time`                  integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `file_name`                       text                                     NOT NULL,
CONSTRAINT `ja_icon_fwait_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_if_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `hand_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `comparison_value`                text                                     NOT NULL,
CONSTRAINT `ja_icon_if_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_info_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `info_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `item_id`                         bigint unsigned                          NULL,
        `trigger_id`                      bigint unsigned                          NULL,
        `host_group`                      varchar(64)                              NULL,
        `host_name`                       varchar(128)                             NULL,
        `get_job_id`                      text                                     NULL,
        `get_calendar_id`                 varchar(32)                              NULL,
CONSTRAINT `ja_icon_info_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_jobnet_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `link_jobnet_id`                  varchar(32)              DEFAULT ''      NOT NULL,
CONSTRAINT `ja_icon_jobnet_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_job_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `stop_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `command_type`                    integer                  DEFAULT '0'     NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `stop_code`                       varchar(32)                              NULL,
        `timeout_run_type`                integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_icon_job_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_job_command_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `command_cls`                     integer                  DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `command`                         text                                     NOT NULL,
CONSTRAINT `ja_job_command_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`,`command_cls`)
) ENGINE=InnoDB;


CREATE TABLE `ja_value_job_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `value`                           text                                     NOT NULL,
CONSTRAINT `ja_value_job_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`,`value_name`)
) ENGINE=InnoDB;


CREATE TABLE `ja_value_jobcon_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_value_jobcon_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`value_name`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_reboot_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `reboot_mode_flag`                integer                  DEFAULT '0'     NOT NULL,
        `reboot_wait_time`                integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_icon_reboot_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_release_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `release_job_id`                  text                                     NOT NULL,
CONSTRAINT `ja_icon_release_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_task_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `submit_jobnet_id`                varchar(32)              DEFAULT ''      NOT NULL,
CONSTRAINT `ja_icon_task_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`)
) ENGINE=InnoDB;


CREATE TABLE `ja_icon_value_table` (
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `value`                           text                                     NOT NULL,
CONSTRAINT `ja_icon_value_pk` PRIMARY KEY (`jobnet_id`,`job_id`,`update_date`,`value_name`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_define_value_jobcon_table` (
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_define_value_jobcon_pk` PRIMARY KEY (`value_name`)
) ENGINE=InnoDB;


CREATE TABLE `ja_define_extjob_table` (
        `command_id`                      varchar(32)              DEFAULT ''      NOT NULL,
        `lang`                            varchar(5)               DEFAULT ''      NOT NULL,
        `command_name`                    varchar(128)             DEFAULT ''      NOT NULL,
        `memo`                            text                                     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_define_extjob_pk` PRIMARY KEY (`command_id`,`lang`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_jobnet_summary_table` (
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `invo_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `run_type`                        integer                  DEFAULT '0'     NOT NULL,
        `status`                          integer                  DEFAULT '0'     NOT NULL,
        `job_status`                      integer                  DEFAULT '0'     NOT NULL,
        `jobnet_abort_flag`               integer                  DEFAULT '0'     NOT NULL,
        `load_status`                     integer                  DEFAULT '0'     NOT NULL,
        `scheduled_time`                  bigint unsigned          DEFAULT '0'     NOT NULL,
        `start_time`                      bigint unsigned          DEFAULT '0'     NOT NULL,
        `end_time`                        bigint unsigned          DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `multiple_start_up`               integer                  DEFAULT '0'     NOT NULL,
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `jobnet_name`                     varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
        `schedule_id`                     varchar(32)                              NULL,
        `calendar_id`                     varchar(32)                              NULL,
        `boot_time`                       char(4)                                  NULL,
        `execution_user_name`             varchar(100)             DEFAULT ''      NOT NULL,
        `running_job_id`                  varchar(1024)                            NULL,
        `running_job_name`                varchar(64)                              NULL,
        `virtual_time`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `virtual_start_time`              bigint unsigned          DEFAULT '0'     NOT NULL,
        `virtual_end_time`                bigint unsigned          DEFAULT '0'     NOT NULL,
        `start_pending_flag`              integer                  DEFAULT '0'     NOT NULL,
        `initial_scheduled_time`          bigint unsigned          DEFAULT '0'     NOT NULL,
        `jobnet_timeout`                  integer                  DEFAULT '0'     NOT NULL,
        `timeout_run_type`                integer                  DEFAULT '0'     NOT NULL,
        `jobnet_timeout_flag`             integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_jobnet_summary_pk` PRIMARY KEY (`inner_jobnet_id`)
) ENGINE=InnoDB;

CREATE INDEX `ja_run_jobnet_summary_idx1` ON `ja_run_jobnet_summary_table` (`status`);
CREATE INDEX `ja_run_jobnet_summary_idx2` ON `ja_run_jobnet_summary_table` (`end_time`);
CREATE INDEX `ja_run_jobnet_summary_idx3` ON `ja_run_jobnet_summary_table` (`public_flag`);


CREATE TABLE `ja_run_jobnet_table` (
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_main_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `run_type`                        integer                  DEFAULT '0'     NOT NULL,
        `main_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `timeout_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `status`                          integer                  DEFAULT '0'     NOT NULL,
        `scheduled_time`                  bigint unsigned          DEFAULT '0'     NOT NULL,
        `start_time`                      bigint unsigned          DEFAULT '0'     NOT NULL,
        `end_time`                        bigint unsigned          DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `multiple_start_up`               integer                  DEFAULT '0'     NOT NULL,
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `jobnet_name`                     varchar(64)              DEFAULT ''      NOT NULL,
        `memo`                            varchar(100)                             NULL,
        `schedule_id`                     varchar(32)                              NULL,
        `calendar_id`                     varchar(32)                              NULL,
        `boot_time`                       char(4)                                  NULL,
        `execution_user_name`             varchar(100)             DEFAULT ''      NOT NULL,
        `running_job_id`                  varchar(1024)                            NULL,
        `running_job_name`                varchar(64)                              NULL,
        `virtual_time`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `virtual_start_time`              bigint unsigned          DEFAULT '0'     NOT NULL,
        `virtual_end_time`                bigint unsigned          DEFAULT '0'     NOT NULL,
        `initial_scheduled_time`          bigint unsigned          DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_jobnet_pk` PRIMARY KEY (`inner_jobnet_id`)
) ENGINE=InnoDB;

CREATE INDEX `ja_run_jobnet_idx1` ON `ja_run_jobnet_table` (`inner_jobnet_main_id`);
CREATE INDEX `ja_run_jobnet_idx2` ON `ja_run_jobnet_table` (`status`);
CREATE INDEX `ja_run_jobnet_idx3` ON `ja_run_jobnet_table` (`scheduled_time`);
CREATE INDEX `ja_run_jobnet_idx4` ON `ja_run_jobnet_table` (`jobnet_id`);


CREATE TABLE `ja_run_job_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_main_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_job_id_fs_link`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
        `invo_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `job_type`                        integer                  DEFAULT '0'     NOT NULL,
        `test_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `method_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `force_flag`                      integer                  DEFAULT '0'     NOT NULL,
        `timeout_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `status`                          integer                  DEFAULT '0'     NOT NULL,
        `boot_count`                      integer                  DEFAULT '0'     NOT NULL,
        `end_count`                       integer                  DEFAULT '0'     NOT NULL,
        `start_time`                      bigint unsigned          DEFAULT '0'     NOT NULL,
        `end_time`                        bigint unsigned          DEFAULT '0'     NOT NULL,
        `point_x`                         integer                  DEFAULT '0'     NOT NULL,
        `point_y`                         integer                  DEFAULT '0'     NOT NULL,
        `job_id`                          varchar(32)              DEFAULT ''      NOT NULL,
        `job_name`                        varchar(64)                              NULL,
        `continue_flag`                   integer                  DEFAULT '0'     NOT NULL,
        `run_user`                        varchar(256)                             NULL,
        `run_user_password`               varchar(256)                             NULL,
CONSTRAINT `ja_run_job_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;

CREATE INDEX `ja_run_job_idx1` ON `ja_run_job_table` (`status`);
CREATE INDEX `ja_run_job_idx2` ON `ja_run_job_table` (`inner_jobnet_main_id`);
CREATE INDEX `ja_run_job_idx3` ON `ja_run_job_table` (`job_type`,`status`);


CREATE TABLE `ja_run_flow_table` (
        `inner_flow_id`                   bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `start_inner_job_id`              bigint unsigned          DEFAULT '0'     NOT NULL,
        `end_inner_job_id`                bigint unsigned          DEFAULT '0'     NOT NULL,
        `flow_type`                       integer                  DEFAULT '0'     NOT NULL,
        `flow_width`                      integer                  DEFAULT '0'     NOT NULL,
		`flow_style`					  varchar(400)		   	   DEFAULT NULL,
CONSTRAINT `ja_run_flow_pk` PRIMARY KEY (`inner_flow_id`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_run_icon_calc_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `hand_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `formula`                         varchar(100)             DEFAULT ''      NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
CONSTRAINT `ja_run_icon_calc_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_end_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `jobnet_stop_flag`                integer                  DEFAULT '0'     NOT NULL,
        `jobnet_stop_code`                varchar(256)             DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_icon_end_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_extjob_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `command_id`                      varchar(32)              DEFAULT ''      NOT NULL,
        `value`                           text                                     NULL,
        `pid`                             integer                  DEFAULT '0'     NOT NULL,
        `wait_count`                      integer                  DEFAULT '0'     NOT NULL,
        `wait_time`                       varchar(14)                              NULL,
CONSTRAINT `ja_run_icon_extjob_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_fcopy_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `from_host_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `to_host_flag`                    integer                  DEFAULT '0'     NOT NULL,
        `overwrite_flag`                  integer                  DEFAULT '0'     NOT NULL,
        `from_host_name`                  varchar(128)             DEFAULT ''      NOT NULL,
        `from_directory`                  text                                     NOT NULL,
        `from_file_name`                  text                                     NOT NULL,
        `to_host_name`                    varchar(128)             DEFAULT ''      NOT NULL,
        `to_directory`                    text                                     NOT NULL,
CONSTRAINT `ja_run_icon_fcopy_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_fwait_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `fwait_mode_flag`                 integer                  DEFAULT '0'     NOT NULL,
        `file_delete_flag`                integer                  DEFAULT '0'     NOT NULL,
        `file_wait_time`                  integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `file_name`                       text                                     NOT NULL,
CONSTRAINT `ja_run_icon_fwait_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_if_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `hand_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `comparison_value`                text                                     NOT NULL,
CONSTRAINT `ja_run_icon_if_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_info_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `info_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `item_id`                         bigint unsigned                          NULL,
        `trigger_id`                      bigint unsigned                          NULL,
        `host_group`                      varchar(64)                              NULL,
        `host_name`                       varchar(128)                             NULL,
        `get_job_id`                      text                                     NULL,
        `get_calendar_id`                 varchar(32)                              NULL,
CONSTRAINT `ja_run_icon_info_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_jobnet_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `link_inner_jobnet_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `link_jobnet_id`                  varchar(32)              DEFAULT ''      NOT NULL,
CONSTRAINT `ja_run_icon_jobnet_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_job_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `stop_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `command_type`                    integer                  DEFAULT '0'     NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `stop_code`                       varchar(32)                              NULL,
        `timeout_run_type`                integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_icon_job_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;
CREATE INDEX `ja_run_icon_job_idx1` ON `ja_run_icon_job_table` (`host_flag`,`host_name`);
CREATE UNIQUE INDEX `ja_run_icon_job_idx2` ON `ja_run_icon_job_table` (`inner_job_id`,`host_flag`);


CREATE TABLE `ja_run_job_command_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `command_cls`                     integer                  DEFAULT '0'     NOT NULL,
        `command`                         text                                     NOT NULL,
CONSTRAINT `ja_run_job_command_pk` PRIMARY KEY (`inner_job_id`,`command_cls`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_value_job_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `value`                           text                                     NOT NULL,
CONSTRAINT `ja_run_value_job_pk` PRIMARY KEY (`inner_job_id`,`value_name`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_value_jobcon_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
CONSTRAINT `ja_run_value_jobcon_pk` PRIMARY KEY (`inner_job_id`,`value_name`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_reboot_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `host_flag`                       integer                  DEFAULT '0'     NOT NULL,
        `reboot_mode_flag`                integer                  DEFAULT '0'     NOT NULL,
        `reboot_wait_time`                integer                  DEFAULT '0'     NOT NULL,
        `host_name`                       varchar(128)             DEFAULT ''      NOT NULL,
        `timeout`                         integer                  DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_run_icon_reboot_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;

CREATE UNIQUE INDEX `ja_run_icon_reboot_idx2` ON `ja_run_icon_reboot_table` (`inner_job_id`,`host_flag`);


CREATE TABLE `ja_run_icon_release_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `release_job_id`                  text                                     NOT NULL,
CONSTRAINT `ja_run_icon_release_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_task_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `submit_inner_jobnet_id`          bigint unsigned          DEFAULT '0'     NOT NULL,
        `submit_jobnet_id`                varchar(32)              DEFAULT ''      NOT NULL,
CONSTRAINT `ja_run_icon_task_pk` PRIMARY KEY (`inner_job_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_run_icon_value_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `value`                           text                                     NOT NULL,
CONSTRAINT `ja_run_icon_value_pk` PRIMARY KEY (`inner_job_id`,`value_name`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_run_value_before_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `before_value`                    text                                     NOT NULL,
        `seq_no`                          bigint unsigned          AUTO_INCREMENT  NOT NULL,
CONSTRAINT UNIQUE INDEX `ja_run_value_before_idx1` (`seq_no`)
) ENGINE=InnoDB;
CREATE UNIQUE INDEX `ja_run_value_before_idx2` ON `ja_run_value_before_table` (`inner_job_id`,`value_name`,`seq_no`);


CREATE TABLE `ja_run_value_after_table` (
        `inner_job_id`                    bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `after_value`                     text                                     NOT NULL,
        `seq_no`                          bigint unsigned          AUTO_INCREMENT  NOT NULL,
CONSTRAINT UNIQUE INDEX `ja_run_value_after_idx1` (`seq_no`)
) ENGINE=InnoDB;


CREATE TABLE `ja_value_before_jobnet_table` (
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `before_value`                    text                                     NOT NULL,
        `seq_no`                          bigint unsigned          AUTO_INCREMENT  NOT NULL,
CONSTRAINT UNIQUE INDEX `ja_value_before_jobnet_idx1` (`seq_no`)
) ENGINE=InnoDB;


CREATE TABLE `ja_value_after_jobnet_table` (
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `value_name`                      varchar(128)             DEFAULT ''      NOT NULL,
        `after_value`                     text                                     NOT NULL,
        `seq_no`                          bigint unsigned          AUTO_INCREMENT  NOT NULL,
CONSTRAINT UNIQUE INDEX `ja_value_after_jobnet_idx1` (`seq_no`)
) ENGINE=InnoDB;


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


CREATE TABLE `ja_run_log_table` (
        `log_date`                        bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_id`                 bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_jobnet_main_id`            bigint unsigned          DEFAULT '0'     NOT NULL,
        `inner_job_id`                    bigint unsigned                          NULL,
        `update_date`                     bigint unsigned          DEFAULT '0'     NOT NULL,
        `method_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `jobnet_status`                   integer                  DEFAULT '0'     NOT NULL,
        `job_status`                      integer                                  NULL,
        `run_type`                        integer                  DEFAULT '0'     NOT NULL,
        `public_flag`                     integer                  DEFAULT '0'     NOT NULL,
        `jobnet_id`                       varchar(32)              DEFAULT ''      NOT NULL,
        `jobnet_name`                     varchar(64)              DEFAULT ''      NOT NULL,
        `job_id`                          varchar(32)                              NULL,
        `job_name`                        varchar(64)                              NULL,
        `user_name`                       varchar(100)             DEFAULT ''      NOT NULL,
        `return_code`                     text                                     NULL,
        `std_out`                         text                                     NULL,
        `std_err`                         text                                     NULL,
        `message_id`                      varchar(32)              DEFAULT ''      NOT NULL
) ENGINE=InnoDB;

CREATE INDEX `ja_run_log_idx1` ON `ja_run_log_table` (`log_date`);
CREATE INDEX `ja_run_log_idx2` ON `ja_run_log_table` (`inner_jobnet_main_id`);
CREATE INDEX `ja_run_log_idx3` ON `ja_run_log_table` (`inner_jobnet_id`,`message_id`);


CREATE TABLE `ja_define_run_log_message_table` (
        `message_id`                      varchar(32)              DEFAULT ''      NOT NULL,
        `lang`                            varchar(5)               DEFAULT ''      NOT NULL,
        `message`                         text                                     NOT NULL,
        `log_type`                        integer                  DEFAULT '0'     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_define_run_log_message_pk` PRIMARY KEY (`message_id`,`lang`)
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


CREATE TABLE `ja_index_table` (
        `count_id`                        integer                  DEFAULT '1'     NOT NULL,
        `nextid`                          bigint unsigned          DEFAULT '0'     NOT NULL,
CONSTRAINT `ja_index_pk` PRIMARY KEY (`count_id`)
) ENGINE=InnoDB;


CREATE TABLE `ja_parameter_table` (
        `parameter_name`                  varchar(128)             DEFAULT ''      NOT NULL,
        `value`                           text                                     NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_parameter_pk` PRIMARY KEY (`parameter_name`)
) ENGINE=InnoDB;


CREATE TABLE `ja_host_lock_table` (
        `lock_host_name`                  varchar(128)             DEFAULT ''      NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT `ja_host_lock_pk` PRIMARY KEY (`lock_host_name`)
) ENGINE=InnoDB;

CREATE TABLE `ja_object_lock_table` (
        `object_id`                       varchar(32)              DEFAULT ''      NOT NULL,
		`object_type`                     tinyint(11) unsigned     NOT NULL,
		`username`                     	  varchar(100)             DEFAULT ''      NOT NULL,
		`attempt_ip`                 varchar(39)             DEFAULT ''      NOT NULL,
		`last_active_time`                 timestamp             DEFAULT CURRENT_TIMESTAMP      NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP 	  NOT NULL,
CONSTRAINT `ja_object_lock_pk` PRIMARY KEY (`object_id`,`object_type`)
) ENGINE=InnoDB;


ALTER TABLE `ja_calendar_detail_table` ADD CONSTRAINT `ja_calendar_detail_fk1` FOREIGN KEY (`calendar_id`,`update_date`) REFERENCES `ja_calendar_control_table` (`calendar_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_schedule_detail_table` ADD CONSTRAINT `ja_schedule_detail_fk1` FOREIGN KEY (`schedule_id`,`update_date`) REFERENCES `ja_schedule_control_table` (`schedule_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_schedule_jobnet_table` ADD CONSTRAINT `ja_schedule_jobnet_fk1` FOREIGN KEY (`schedule_id`,`update_date`) REFERENCES `ja_schedule_control_table` (`schedule_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_job_control_table` ADD CONSTRAINT `ja_job_control_fk1` FOREIGN KEY (`jobnet_id`,`update_date`) REFERENCES `ja_jobnet_control_table` (`jobnet_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_flow_control_table` ADD CONSTRAINT `ja_flow_control_fk1` FOREIGN KEY (`jobnet_id`,`start_job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_flow_control_table` ADD CONSTRAINT `ja_flow_control_fk2` FOREIGN KEY (`jobnet_id`,`end_job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_agentless_table` ADD CONSTRAINT `ja_icon_agentless_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_calc_table` ADD CONSTRAINT `ja_icon_calc_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_end_table` ADD CONSTRAINT `ja_icon_end_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_extjob_table` ADD CONSTRAINT `ja_icon_extjob_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_fcopy_table` ADD CONSTRAINT `ja_icon_fcopy_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_fwait_table` ADD CONSTRAINT `ja_icon_fwait_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_if_table` ADD CONSTRAINT `ja_icon_if_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_info_table` ADD CONSTRAINT `ja_icon_info_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_jobnet_table` ADD CONSTRAINT `ja_icon_jobnet_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_job_table` ADD CONSTRAINT `ja_icon_job_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_job_command_table` ADD CONSTRAINT `ja_icon_job_command_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_icon_job_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_value_job_table` ADD CONSTRAINT `ja_value_job_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_icon_job_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_value_jobcon_table` ADD CONSTRAINT `ja_value_jobcon_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_icon_job_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_reboot_table` ADD CONSTRAINT `ja_icon_reboot_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_release_table` ADD CONSTRAINT `ja_icon_release_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_task_table` ADD CONSTRAINT `ja_icon_task_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_value_table` ADD CONSTRAINT `ja_icon_value_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_icon_zabbix_link_table` ADD CONSTRAINT `ja_icon_zabbix_link_fk1` FOREIGN KEY (`jobnet_id`,`job_id`,`update_date`) REFERENCES `ja_job_control_table` (`jobnet_id`,`job_id`,`update_date`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_jobnet_summary_table` ADD CONSTRAINT `ja_run_jobnet_summary_fk1` FOREIGN KEY (`inner_jobnet_id`) REFERENCES `ja_run_jobnet_table` (`inner_jobnet_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_job_table` ADD CONSTRAINT `ja_run_job_fk1` FOREIGN KEY (`inner_jobnet_id`) REFERENCES `ja_run_jobnet_table` (`inner_jobnet_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_flow_table` ADD CONSTRAINT `ja_run_flow_fk1` FOREIGN KEY (`inner_jobnet_id`) REFERENCES `ja_run_jobnet_table` (`inner_jobnet_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_agentless_table` ADD CONSTRAINT `ja_run_icon_agentless_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_calc_table` ADD CONSTRAINT `ja_run_icon_calc_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_end_table` ADD CONSTRAINT `ja_run_icon_end_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_extjob_table` ADD CONSTRAINT `ja_run_icon_extjob_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_fcopy_table` ADD CONSTRAINT `ja_run_icon_fcopy_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_fwait_table` ADD CONSTRAINT `ja_run_icon_fwait_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_if_table` ADD CONSTRAINT `ja_run_icon_if_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_info_table` ADD CONSTRAINT `ja_run_icon_info_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_jobnet_table` ADD CONSTRAINT `ja_run_icon_jobnet_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_job_table` ADD CONSTRAINT `ja_run_icon_job_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_job_command_table` ADD CONSTRAINT `ja_run_job_command_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_icon_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_value_job_table` ADD CONSTRAINT `ja_run_value_job_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_icon_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_value_jobcon_table` ADD CONSTRAINT `ja_run_value_jobcon_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_icon_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_reboot_table` ADD CONSTRAINT `ja_run_icon_reboot_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_release_table` ADD CONSTRAINT `ja_run_icon_release_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_task_table` ADD CONSTRAINT `ja_run_icon_task_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_value_table` ADD CONSTRAINT `ja_run_icon_value_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_icon_zabbix_link_table` ADD CONSTRAINT `ja_run_icon_zabbix_link_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_value_before_table` ADD CONSTRAINT `ja_run_value_before_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_run_value_after_table` ADD CONSTRAINT `ja_run_value_after_fk1` FOREIGN KEY (`inner_job_id`) REFERENCES `ja_run_job_table` (`inner_job_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_value_before_jobnet_table` ADD CONSTRAINT `ja_value_before_jobnet_fk1` FOREIGN KEY (`inner_jobnet_id`) REFERENCES `ja_run_jobnet_table` (`inner_jobnet_id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE `ja_value_after_jobnet_table` ADD CONSTRAINT `ja_value_after_jobnet_fk1` FOREIGN KEY (`inner_jobnet_id`) REFERENCES `ja_run_jobnet_table` (`inner_jobnet_id`) ON DELETE CASCADE ON UPDATE CASCADE;
