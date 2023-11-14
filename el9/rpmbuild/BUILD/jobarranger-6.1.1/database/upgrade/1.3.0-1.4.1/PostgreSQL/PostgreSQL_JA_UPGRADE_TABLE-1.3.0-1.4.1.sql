
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 1.3.0 -> 1.4.0)  - 2013/10/17 -

-- Copyright (C) 2012-2013 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


ALTER TABLE ja_run_jobnet_summary_table ADD COLUMN execution_user_name varchar(100) DEFAULT '' NOT NULL;
ALTER TABLE ja_run_jobnet_table         ADD COLUMN execution_user_name varchar(100) DEFAULT '' NOT NULL;


DROP TABLE ja_define_extjob_table CASCADE;
CREATE TABLE ja_define_extjob_table (
        command_id                      varchar(32)              DEFAULT ''      NOT NULL,
        lang                            varchar(5)               DEFAULT ''      NOT NULL,
        command_name                    varchar(128)             DEFAULT ''      NOT NULL,
        memo                            text                     DEFAULT ''      NOT NULL,
        created_date                    timestamp                DEFAULT CURRENT_TIMESTAMP  NOT NULL,
CONSTRAINT ja_define_extjob_pk PRIMARY KEY (command_id, lang)
);


START TRANSACTION;
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'en_gb', 'Sleep until the specified time', 'I will wait for the process only during the specified number of seconds. Please specify the time in seconds waiting for the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'en_gb', 'Waiting until the specified time', 'I will wait for the process until the specified time. Please specify in the format HHMM the time waiting for the parameter.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'en_gb', 'Check the day of the week', 'We will determine whether to match the day the day of the week during the run icon is specified. Please specify (Sun Mon Tue Wed Thu Fri Sat) days of the week that you want to compare the parameter. The day of the week can be specified more than once.');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'en_gb', 'Issue the zabbix_sender', 'I do call Zabbix sender. Please specify the parameters to be passed to the command the parameter zabbix_sender. Example: -z zabbix_hostname -p zabbix_port_number -s host_name -k item_key -o "value"');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdsleep', 'ja_jp', '時間待合せ（SLEEP）', '指定された秒数の間だけ処理を待合せします。パラメータに待合せ時間を秒単位(0～999999)で指定してください。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdtime', 'ja_jp', '時刻待合せ（TIME）', '指定された時刻まで処理を待合せします。パラメータに待合せ時刻を HHMM(0000～9959) の形式で指定してください。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('jacmdweek', 'ja_jp', '曜日判断', 'アイコン実行時の曜日が指定された曜日と一致するかを判断します。パラメータに比較したい曜日(Sun, Mon, Tue, Wed, Thu, Fri, Sat)を指定してください。曜日は複数指定が可能です。');
INSERT INTO ja_define_extjob_table (command_id, lang, command_name, memo) VALUES ('zabbix_sender', 'ja_jp', 'Zabbix通知（zabbix_sender）', 'Zabbix senderの呼出しを行います。パラメータに zabbix_sender コマンドに渡すパラメータを指定してください。 例：-z Zabbixホスト名 -p Zabbixポート番号 -s ホスト名 -k アイテムキー -o "通知内容（値）"');
INSERT INTO ja_define_value_jobcon_table (value_name) VALUES ('MANAGEMENT_ID');
UPDATE ja_parameter_table SET value = '43200' WHERE parameter_name = 'JOBLOG_KEEP_SPAN';
COMMIT;
