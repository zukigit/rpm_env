
-- Job Arranger create table SQL for PostgreSQL (Ver 1.0.0 -> 1.2.0)  - 2013/03/29 -

-- Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.


CREATE TABLE ja_icon_fcopy_table (
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)              DEFAULT ''      NOT NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
        from_host_flag                  integer                  DEFAULT '0'     NOT NULL,
        to_host_flag                    integer                  DEFAULT '0'     NOT NULL,
        overwrite_flag                  integer                  DEFAULT '0'     NOT NULL,
        from_host_name                  varchar(128)             DEFAULT ''      NOT NULL,
        from_directory                  text                     DEFAULT ''      NOT NULL,
        from_file_name                  text                     DEFAULT ''      NOT NULL,
        to_host_name                    varchar(128)             DEFAULT ''      NOT NULL,
        to_directory                    text                                     NOT NULL,
CONSTRAINT ja_icon_fcopy_pk PRIMARY KEY (jobnet_id, job_id, update_date)
);


CREATE TABLE ja_icon_fwait_table (
        jobnet_id                       varchar(32)              DEFAULT ''      NOT NULL,
        job_id                          varchar(32)              DEFAULT ''      NOT NULL,
        update_date                     bigint                   DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
        file_name                       text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_icon_fwait_pk PRIMARY KEY (jobnet_id, job_id, update_date)
);


CREATE TABLE ja_run_icon_fcopy_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        from_host_flag                  integer                  DEFAULT '0'     NOT NULL,
        to_host_flag                    integer                  DEFAULT '0'     NOT NULL,
        overwrite_flag                  integer                  DEFAULT '0'     NOT NULL,
        from_host_name                  varchar(128)             DEFAULT ''      NOT NULL,
        from_directory                  text                     DEFAULT ''      NOT NULL,
        from_file_name                  text                     DEFAULT ''      NOT NULL,
        to_host_name                    varchar(128)             DEFAULT ''      NOT NULL,
        to_directory                    text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_run_icon_fcopy_pk PRIMARY KEY (inner_job_id)
);


CREATE TABLE ja_run_icon_fwait_table (
        inner_job_id                    bigint                   DEFAULT '0'     NOT NULL,
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        host_flag                       integer                  DEFAULT '0'     NOT NULL,
        host_name                       varchar(128)             DEFAULT ''      NOT NULL,
        file_name                       text                     DEFAULT ''      NOT NULL,
CONSTRAINT ja_run_icon_fwait_pk PRIMARY KEY (inner_job_id)
);


CREATE TABLE ja_value_before_jobnet_table (
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        before_value                    text                                     NOT NULL,
CONSTRAINT ja_value_before_jobnet_pk PRIMARY KEY (inner_jobnet_id, value_name)
);


CREATE TABLE ja_value_after_jobnet_table (
        inner_jobnet_id                 bigint                   DEFAULT '0'     NOT NULL,
        value_name                      varchar(128)             DEFAULT ''      NOT NULL,
        after_value                     text                                     NOT NULL,
CONSTRAINT ja_value_after_jobnet_pk PRIMARY KEY (inner_jobnet_id, value_name)
);


ALTER TABLE ONLY ja_icon_fcopy_table ADD CONSTRAINT ja_icon_fcopy_fk1 FOREIGN KEY (jobnet_id, job_id, update_date) REFERENCES ja_job_control_table (jobnet_id, job_id, update_date) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_icon_fwait_table ADD CONSTRAINT ja_icon_fwait_fk1 FOREIGN KEY (jobnet_id, job_id, update_date) REFERENCES ja_job_control_table (jobnet_id, job_id, update_date) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_run_icon_fcopy_table ADD CONSTRAINT ja_run_icon_fcopy_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_run_icon_fwait_table ADD CONSTRAINT ja_run_icon_fwait_fk1 FOREIGN KEY (inner_job_id) REFERENCES ja_run_job_table (inner_job_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_value_before_jobnet_table ADD CONSTRAINT ja_value_before_jobnet_fk1 FOREIGN KEY (inner_jobnet_id) REFERENCES ja_run_jobnet_table (inner_jobnet_id) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE ONLY ja_value_after_jobnet_table ADD CONSTRAINT ja_value_after_jobnet_fk1 FOREIGN KEY (inner_jobnet_id) REFERENCES ja_run_jobnet_table (inner_jobnet_id) ON DELETE CASCADE ON UPDATE CASCADE;
