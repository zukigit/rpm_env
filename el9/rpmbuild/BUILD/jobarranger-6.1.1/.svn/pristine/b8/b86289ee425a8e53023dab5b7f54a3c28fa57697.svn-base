
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 1.2.1 -> 1.3.0)  - 2013/05/24 -

-- Copyright (C) 2012-2013 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.

-- Caution
-- There is the possibility that this upgrade, net job scheduled for execution is registered double.
-- You can do a time when net job plan does not exist if you want to upgrade to 1.3.0 from version 1.2.1,
-- Please go to after you delete a running job net information of all.

-- By the following SQL, I can delete the job net information of all running.
-- DELETE FROM ja_run_jobnet_table;


ALTER TABLE ja_job_control_table        ADD COLUMN   method_flag     integer     DEFAULT '0' NOT NULL;
ALTER TABLE ja_icon_info_table          ADD COLUMN   get_calendar_id varchar(32) NULL;
ALTER TABLE ja_run_jobnet_summary_table ADD COLUMN   schedule_id     varchar(32) NULL;
ALTER TABLE ja_run_jobnet_summary_table ADD COLUMN   calendar_id     varchar(32) NULL;
ALTER TABLE ja_run_jobnet_summary_table ADD COLUMN   boot_time       char(4)     NULL;
ALTER TABLE ja_run_jobnet_table         ADD COLUMN   schedule_id     varchar(32) NULL;
ALTER TABLE ja_run_jobnet_table         ADD COLUMN   calendar_id     varchar(32) NULL;
ALTER TABLE ja_run_jobnet_table         ADD COLUMN   boot_time       char(4)     NULL;
ALTER TABLE ja_run_icon_info_table      ALTER COLUMN get_job_id                  DROP NOT NULL;
ALTER TABLE ja_run_icon_info_table      ADD COLUMN   get_calendar_id varchar(32) NULL;


CREATE TABLE ja_icon_release_table (
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)              DEFAULT ''      NOT NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
        release_job_id                  text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_icon_release_pk PRIMARY KEY (jobnet_id, job_id, update_date)
);
ALTER TABLE ONLY ja_icon_release_table ADD CONSTRAINT ja_icon_release_fk1 FOREIGN KEY (jobnet_id, job_id, update_date) REFERENCES ja_job_control_table (jobnet_id, job_id, update_date) ON DELETE CASCADE ON UPDATE CASCADE;


CREATE TABLE ja_run_icon_release_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        release_job_id                  text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_run_icon_release_pk PRIMARY KEY (inner_job_id)
);
ALTER TABLE ONLY ja_run_icon_release_table ADD CONSTRAINT ja_run_icon_release_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;


DROP TABLE ja_icon_fwait_table CASCADE;
CREATE TABLE ja_icon_fwait_table (
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)              DEFAULT ''      NOT NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        fwait_mode_flag                 integer                  DEFAULT '0'     NOT NULL,
        file_delete_flag                integer                  DEFAULT '0'     NOT NULL,
        file_wait_time                  integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
        file_name                       text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_icon_fwait_pk PRIMARY KEY (jobnet_id, job_id, update_date)
);
ALTER TABLE ONLY ja_icon_fwait_table ADD CONSTRAINT ja_icon_fwait_fk1 FOREIGN KEY (jobnet_id, job_id, update_date) REFERENCES ja_job_control_table (jobnet_id, job_id, update_date) ON DELETE CASCADE ON UPDATE CASCADE;


DROP TABLE ja_run_icon_fwait_table CASCADE;
CREATE TABLE ja_run_icon_fwait_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        fwait_mode_flag                 integer                  DEFAULT '0'     NOT NULL,
        file_delete_flag                integer                  DEFAULT '0'     NOT NULL,
        file_wait_time                  integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
        file_name                       text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_run_icon_fwait_pk PRIMARY KEY (inner_job_id)
);
ALTER TABLE ONLY ja_run_icon_fwait_table ADD CONSTRAINT ja_run_icon_fwait_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;


DROP TABLE ja_run_log_table CASCADE;
CREATE TABLE ja_run_log_table (
        log_date                        bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_main_id            bigint                   DEFAULT '0'     NOT NULL,
        inner_job_id                    bigint                                   NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        log_type                        integer                  DEFAULT '0'     NOT NULL,
        method_flag                     integer                  DEFAULT '0'     NOT NULL,
        jobnet_status                   integer                  DEFAULT '0'     NOT NULL,
        job_status                      integer                                  NULL,
        run_type                        integer                  DEFAULT '0'     NOT NULL,
        public_flag                     integer                  DEFAULT '0'     NOT NULL,
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        jobnet_name                     varchar(64)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)                              NULL,
        job_name                        varchar(64)                              NULL,
        user_name                       varchar(100)             DEFAULT ''      NOT NULL,
        return_code                     text                                     NULL,
        std_out                         text                                     NULL,
        std_err                         text                                     NULL,
        message_id                      varchar(32)              DEFAULT ''      NOT NULL,
        message                         text                     DEFAULT ''      NOT NULL
);
CREATE INDEX ja_run_log_idx1 ON ja_run_log_table (log_date);
CREATE INDEX ja_run_log_idx2 ON ja_run_log_table (inner_jobnet_main_id);


START TRANSACTION;
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('CURRENT_TIME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_BOOT_TIME');
COMMIT;
