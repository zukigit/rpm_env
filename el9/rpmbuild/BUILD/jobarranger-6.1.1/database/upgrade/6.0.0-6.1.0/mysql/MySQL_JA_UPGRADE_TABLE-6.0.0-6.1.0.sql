
-- Job Arranger upgrade table SQL for MySQL (Ver 6.0.0 -> 6.1.0)  - 2022/09/23 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
-- Copyright (C) 2022 Daiwa Institute of Research Ltd. All Rights Reserved.

ALTER TABLE `ja_flow_control_table`         ADD COLUMN `flow_style`            varchar(400)          NULL			AFTER `flow_width`;
ALTER TABLE `ja_run_flow_table`         ADD COLUMN `flow_style`            varchar(400)          NULL			AFTER `flow_width`;


CREATE TABLE `ja_object_lock_table` (
        `object_id`                       varchar(32)              DEFAULT ''      NOT NULL,
		`object_type`                     tinyint(11) unsigned     NOT NULL,
		`username`                     	  varchar(100)             DEFAULT ''      NOT NULL,
		`attempt_ip`                 varchar(39)             DEFAULT ''      NOT NULL,
		`last_active_time`                timestamp                DEFAULT CURRENT_TIMESTAMP 	  NOT NULL,
        `created_date`                    timestamp                DEFAULT CURRENT_TIMESTAMP 	  NOT NULL,
CONSTRAINT `ja_object_lock_pk` PRIMARY KEY (`object_id`,`object_type`)
) ENGINE=InnoDB;

INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('HEARTBEAT_INTERVAL_TIME', '60');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('OBJECT_LOCK_EXPIRED_TIME', '15');