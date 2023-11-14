
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 1.4.1 -> 1.4.2)  - 2014/02/19 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


ALTER TABLE ja_jobnet_control_table     ADD COLUMN multiple_start_up integer DEFAULT '0' NOT NULL;
ALTER TABLE ja_run_jobnet_summary_table ADD COLUMN multiple_start_up integer DEFAULT '0' NOT NULL;
ALTER TABLE ja_run_jobnet_table         ADD COLUMN multiple_start_up integer DEFAULT '0' NOT NULL;

ALTER TABLE ja_job_control_table        ADD COLUMN force_flag integer DEFAULT '0' NOT NULL;
ALTER TABLE ja_run_job_table            ADD COLUMN force_flag integer DEFAULT '0' NOT NULL;

ALTER TABLE ja_run_log_table DROP COLUMN log_type;
ALTER TABLE ja_run_log_table DROP COLUMN message;


CREATE TABLE ja_define_run_log_message_table (
        message_id                      varchar(32)              DEFAULT ''      NOT NULL,
        lang                            varchar(5)               DEFAULT ''      NOT NULL,
        message                         text                     DEFAULT ''      NOT NULL,
        log_type                        integer                  DEFAULT '0'     NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT ja_define_run_log_message_pk PRIMARY KEY (message_id, lang)
);


START TRANSACTION;
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'en_us', 'Sleep until the specified time', 'I will wait for the process only during the specified number of seconds. Please specify the time in seconds waiting for the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'en_us', 'Waiting until the specified time', 'I will wait for the process until the specified time. Please specify in the format HHMM the time waiting for the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'en_us', 'Check the day of the week', 'We will determine whether to match the day the day of the week during the run icon is specified. Please specify (Sun Mon Tue Wed Thu Fri Sat) days of the week that you want to compare the parameter. The day of the week can be specified more than once.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'en_us', 'Issue the zabbix_sender', 'I do call Zabbix sender. Please specify the parameters to be passed to the command the parameter zabbix_sender. Example: -z zabbix_hostname -p zabbix_port_number -s host_name -k item_key -o "value"');

INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000001', 'en_gb', 'Jobnet has started.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000002', 'en_gb', 'Jobnet has ended.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000003', 'en_gb', 'Job has started.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000004', 'en_gb', 'Job has ended.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000005', 'en_gb', 'Job has timed out.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000006', 'en_gb', 'Skip the job.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000007', 'en_gb', 'Job is rerun.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000001', 'en_gb', 'Failed to schedule start-up of jobnet.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000002', 'en_gb', 'Job failed.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000003', 'en_gb', 'Jobnet failed.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000001', 'en_us', 'Jobnet has started.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000002', 'en_us', 'Jobnet has ended.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000003', 'en_us', 'Job has started.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000004', 'en_us', 'Job has ended.', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000005', 'en_us', 'Job has timed out.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000006', 'en_us', 'Skip the job.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000007', 'en_us', 'Job is rerun.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000001', 'en_us', 'Failed to schedule start-up of jobnet.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000002', 'en_us', 'Job failed.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000003', 'en_us', 'Jobnet failed.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000001', 'ja_jp', 'ジョブネットが開始しました。', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000002', 'ja_jp', 'ジョブネットが終了しました。', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000003', 'ja_jp', 'ジョブが開始しました。', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000004', 'ja_jp', 'ジョブが終了しました。', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000005', 'ja_jp', 'ジョブがタイムアウトしました。', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000006', 'ja_jp', 'ジョブをスキップします。', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000007', 'ja_jp', 'ジョブを再実行します。', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000001', 'ja_jp', 'ジョブネットのスケジュール起動が行えませんでした。', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000002', 'ja_jp', 'ジョブが異常終了しました。', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000003', 'ja_jp', 'ジョブネットが異常終了しました。', 2);

INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_DATA_TYPE');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_LAST_STATUS');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_LATEST_DATA');
COMMIT;
