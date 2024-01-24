
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 1.2.0 -> 1.2.1)  - 2013/04/22 -

-- Copyright (C) 2012-2013 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


DROP TABLE ja_run_value_before_table CASCADE;
DROP TABLE ja_run_value_after_table CASCADE;
DROP TABLE ja_value_before_jobnet_table CASCADE;
DROP TABLE ja_value_after_jobnet_table CASCADE;

CREATE TABLE ja_run_value_before_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        before_value                    text                     DEFAULT ''      NOT NULL,
        seq_no                          bigserial                                NOT NULL,
CONSTRAINT ja_run_value_before_idx1 UNIQUE (seq_no)
);

CREATE TABLE ja_run_value_after_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        after_value                     text                     DEFAULT ''      NOT NULL,
        seq_no                          bigserial                                NOT NULL,
CONSTRAINT ja_run_value_after_idx1 UNIQUE (seq_no)
);

CREATE TABLE ja_value_before_jobnet_table (
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        before_value                    text                                     NOT NULL,
        seq_no                          bigserial                                NOT NULL,
CONSTRAINT ja_value_before_jobnet_idx1 UNIQUE (seq_no)
);

CREATE TABLE ja_value_after_jobnet_table (
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        after_value                     text                                     NOT NULL,
        seq_no                          bigserial                                NOT NULL,
CONSTRAINT ja_value_after_jobnet_idx1 UNIQUE (seq_no)
);

ALTER TABLE ONLY ja_run_value_before_table ADD CONSTRAINT ja_run_value_before_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_run_value_after_table ADD CONSTRAINT ja_run_value_after_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_value_before_jobnet_table ADD CONSTRAINT ja_value_before_jobnet_fk1 FOREIGN KEY (inner_jobnet_id) REFERENCES ja_run_jobnet_table (inner_jobnet_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_value_after_jobnet_table ADD CONSTRAINT ja_value_after_jobnet_fk1 FOREIGN KEY (inner_jobnet_id) REFERENCES ja_run_jobnet_table (inner_jobnet_id) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE ja_icon_reboot_table (
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)              DEFAULT ''      NOT NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        reboot_mode_flag                integer                  DEFAULT '0'     NOT NULL,
        reboot_wait_time                integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
CONSTRAINT ja_icon_reboot_pk PRIMARY KEY (jobnet_id, job_id, update_date)
);


CREATE TABLE ja_run_icon_reboot_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        reboot_mode_flag                integer                  DEFAULT '0'     NOT NULL,
        reboot_wait_time                integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
CONSTRAINT ja_run_icon_reboot_pk PRIMARY KEY (inner_job_id)
);


CREATE TABLE ja_host_lock_table (
        lock_host_name                  varchar(128)             DEFAULT ''      NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT ja_host_lock_pk PRIMARY KEY (lock_host_name)
);


ALTER TABLE ONLY ja_icon_reboot_table ADD CONSTRAINT ja_icon_reboot_fk1 FOREIGN KEY (jobnet_id, job_id, update_date) REFERENCES ja_job_control_table (jobnet_id, job_id, update_date) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_run_icon_reboot_table ADD CONSTRAINT ja_run_icon_reboot_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;


START TRANSACTION;
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_START_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_START_Y', '39');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_JOB_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_JOB_Y', '93');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_END_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_END_Y', '146');
INSERT INTO ja_host_lock_table (lock_host_name) VALUES ('HOST_LOCK_RECORD');
COMMIT;
