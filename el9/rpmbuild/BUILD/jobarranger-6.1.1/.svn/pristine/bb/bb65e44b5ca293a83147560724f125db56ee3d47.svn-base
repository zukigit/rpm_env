
-- Job Arranger upgrade table SQL for MySQL (Ver 2.1.0 -> 3.2.0)  - 2017/01/30 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.

ALTER TABLE `ja_jobnet_control_table`         ADD COLUMN `jobnet_timeout`            integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `jobnet_timeout`            integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_jobnet_control_table`         ADD COLUMN `timeout_run_type`          integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `timeout_run_type`          integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_jobnet_summary_table`     ADD COLUMN `jobnet_timeout_flag`       integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_icon_job_table`               ADD COLUMN `timeout_run_type`          integer          DEFAULT '0' NOT NULL;
ALTER TABLE `ja_run_icon_job_table`           ADD COLUMN `timeout_run_type`          integer          DEFAULT '0' NOT NULL;


INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'ja_jp', 'ジョブネットがタイムアウトしました。', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'en_us', 'Jobnet has timed out.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'ko_kr', 'JOBNET이 TIMEOUT되었습니다.', 1);