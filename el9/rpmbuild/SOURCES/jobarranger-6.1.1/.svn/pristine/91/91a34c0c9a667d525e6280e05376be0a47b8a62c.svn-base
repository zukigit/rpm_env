
-- Job Arranger initial data register SQL - 2014/09/17 -

-- Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


START TRANSACTION;

DELETE FROM ja_define_value_jobcon_table;
DELETE FROM ja_define_extjob_table;
DELETE FROM ja_index_table;
DELETE FROM ja_parameter_table;
DELETE FROM ja_host_lock_table;
DELETE FROM ja_define_run_log_message_table;

INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('CURRENT_TIME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ICON_STATUS');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_BOOT_TIME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_ID');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_NAME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOBNET_SCHEDULED_TIME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOB_EXIT_CD');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOB_ID');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOB_ID_FULL');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('JOB_NAME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('LAST_STATUS');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('MANAGEMENT_ID');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('STD_ERR');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('STD_OUT');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('USER_NAME');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_DATA_TYPE');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_LAST_STATUS');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('ZBX_LATEST_DATA');

INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'en_gb', 'Pause during the specified number of seconds.', 'This function pause the specified number of seconds.Please specify the time in seconds waiting to the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'en_gb', 'Pause until the specified clock time.', 'This function pause the process until the specified clock time. Please specify in the format HHMM the time waiting to the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'en_gb', 'Check the day of the week', 'This function compare the specified days of week and the current day of week at executing this icon.Please specify (Sun Mon Tue Wed Thu Fri Sat) days of the week to the parameter. The days of the week can be specified more than once.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'en_gb', 'Execute the zabbix_sender.', 'This function execute the zabbix_sender. Please specify the parameters of zabbix_sender. Example: -z zabbix_hostname -p zabbix_port_number -s host_name -k item_key -o "value"');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'en_us', 'Pause during the specified number of seconds.', 'This function pause the specified number of seconds.Please specify the time in seconds waiting to the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'en_us', 'Pause until the specified clock time.', 'This function pause the process until the specified clock time. Please specify in the format HHMM the time waiting to the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'en_us', 'Check the day of the week', 'This function compare the specified days of week and the current day of week at executing this icon.Please specify (Sun Mon Tue Wed Thu Fri Sat) days of the week to the parameter. The days of the week can be specified more than once.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'en_us', 'Execute the zabbix_sender.', 'This function execute the zabbix_sender. Please specify the parameters of zabbix_sender. Example: -z zabbix_hostname -p zabbix_port_number -s host_name -k item_key -o "value"');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'ja_jp', '時間待合せ（SLEEP）', '指定された秒数の間だけ処理を待機します。パラメータに待合せ時間を秒単位(0～999999)で指定してください。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'ja_jp', '時刻待合せ（TIME）', '指定された時刻まで処理を待機します。パラメータに待合せ時刻を HHMM(0000～9959) の形式で指定してください。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'ja_jp', '曜日判断', 'アイコン実行時の曜日が指定された曜日と一致するかを判断します。パラメータに比較したい曜日(Sun, Mon, Tue, Wed, Thu, Fri, Sat)を指定してください。曜日は複数指定が可能です。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'ja_jp', 'Zabbix通知（zabbix_sender）', 'zabbix_senderを実行します。パラメータに zabbix_sender コマンドに渡すパラメータを指定してください。 例：-z Zabbixホスト名 -p Zabbixポート番号 -s ホスト名 -k アイテムキー -o "通知内容（値）"');

INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'ko_kr', '대기 (SLEEP)', '지정하신 수초간 처리를 대기합니다.Parameter에 대기시간을 초단위(0～999999)로 지정해 주세요.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'ko_kr', '지정시간 대기 (TIME)', '지정한 시각까지 처리를 대기 합니다. Parameter에 시각을 HHMM(0000～9959)의 형식으로 지정해 주세요.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'ko_kr', '요일 판정', '아이콘을 실행할때, 지정한 요일과 일치하는지를 판단합니다. Parameter에 비교하실 요일(Sun, Mon, Tue, Wed, Thu, Fri, Sat)로 지정해 주세요.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'ko_kr', 'Zabbix통지 (zabbix_sender)', 'zabbix_sender를 실행합니다. Parameter에 zabbix_sender command에 넘길 Parameter를 지정해 주세요. 예) -z Zabbix호스트명 -p Zabbix포트  -s 대상호스트명 -k 아이템 키 -o "전달하고 싶은 내용(값)"');
INSERT INTO ja_index_table (count_id, nextid) VALUES (1, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (2, 1500000000000000000);
INSERT INTO ja_index_table (count_id, nextid) VALUES (3, 1600000000000000000);
INSERT INTO ja_index_table (count_id, nextid) VALUES (20, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (30, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (40, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (100, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (101, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (102, 1);
INSERT INTO ja_index_table (count_id, nextid) VALUES (103, 1);

INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBLOG_KEEP_SPAN', '43200');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_KEEP_SPAN', '60');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_LOAD_SPAN', '60');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_VIEW_SPAN', '60');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_START_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_START_Y', '39');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_JOB_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_JOB_Y', '93');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_END_X', '117');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('JOBNET_DUMMY_END_Y', '146');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('MANAGER_TIME_SYNC', '0');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('SNDMSG_KEEP_SPAN', '1440');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ON', '1');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_IP', '127.0.0.1');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_PORT', '10051');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ZABBIX_HOST', 'Zabbix server');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_ITEM_KEY', 'jasender');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_SENDER', 'zabbix_sender');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY', '0');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY_COUNT', '3');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('ZBXSND_RETRY_INTERVAL', '5');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('HEARTBEAT_INTERVAL_TIME', '60');
INSERT INTO ja_parameter_table (parameter_name, value) VALUES ('OBJECT_LOCK_EXPIRED_TIME', '15');

INSERT INTO ja_host_lock_table (lock_host_name) VALUES ('HOST_LOCK_RECORD');

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
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'en_us', 'Jobnet has timed out.', 1);
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
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'ja_jp', 'ジョブネットがタイムアウトしました。', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000001', 'ja_jp', 'ジョブネットのスケジュール起動が行えませんでした。', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000002', 'ja_jp', 'ジョブが異常終了しました。', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000003', 'ja_jp', 'ジョブネットが異常終了しました。', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000001', 'ko_kr', 'JOBNET 시작', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000002', 'ko_kr', 'JOBNET 종료', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000003', 'ko_kr', 'JOB 시작', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000004', 'ko_kr', 'JOB 종료', 0);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000005', 'ko_kr', 'JOB이 TIMEOUT되었습니다.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000006', 'ko_kr', 'JOB이 SKIP되었습니다.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000007', 'ko_kr', 'JOB이 재실행 되었습니다.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC00000008', 'ko_kr', 'JOBNET이 TIMEOUT되었습니다.', 1);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000001', 'ko_kr', 'JOBNET이 스케줄에 의한 기동이 되지않았습니다.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000002', 'ko_kr', 'JOB이 비정상 종료되었습니다.', 2);
INSERT INTO ja_define_run_log_message_table (message_id, lang, message, log_type) VALUES ('JC90000003', 'ko_kr', 'JOBNET이 비정상 종료되었습니다.', 2);

COMMIT;
