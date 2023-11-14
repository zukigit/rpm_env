
-- Job Arranger drop table SQL for MySQL  - 2014/09/17 -

-- Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.


-- DROP FOREIGN KEY

ALTER TABLE `ja_calendar_detail_table` DROP FOREIGN KEY `ja_calendar_detail_fk1`;
ALTER TABLE `ja_schedule_detail_table` DROP FOREIGN KEY `ja_schedule_detail_fk1`;
ALTER TABLE `ja_schedule_jobnet_table` DROP FOREIGN KEY `ja_schedule_jobnet_fk1`;
ALTER TABLE `ja_job_control_table` DROP FOREIGN KEY `ja_job_control_fk1`;
ALTER TABLE `ja_flow_control_table` DROP FOREIGN KEY `ja_flow_control_fk1`;
ALTER TABLE `ja_flow_control_table` DROP FOREIGN KEY `ja_flow_control_fk2`;
ALTER TABLE `ja_icon_agentless_table` DROP FOREIGN KEY `ja_icon_agentless_fk1`;
ALTER TABLE `ja_icon_calc_table` DROP FOREIGN KEY `ja_icon_calc_fk1`;
ALTER TABLE `ja_icon_end_table` DROP FOREIGN KEY `ja_icon_end_fk1`;
ALTER TABLE `ja_icon_extjob_table` DROP FOREIGN KEY `ja_icon_extjob_fk1`;
ALTER TABLE `ja_icon_fcopy_table` DROP FOREIGN KEY `ja_icon_fcopy_fk1`;
ALTER TABLE `ja_icon_fwait_table` DROP FOREIGN KEY `ja_icon_fwait_fk1`;
ALTER TABLE `ja_icon_if_table` DROP FOREIGN KEY `ja_icon_if_fk1`;
ALTER TABLE `ja_icon_info_table` DROP FOREIGN KEY `ja_icon_info_fk1`;
ALTER TABLE `ja_icon_jobnet_table` DROP FOREIGN KEY `ja_icon_jobnet_fk1`;
ALTER TABLE `ja_icon_job_table` DROP FOREIGN KEY `ja_icon_job_fk1`;
ALTER TABLE `ja_job_command_table` DROP FOREIGN KEY `ja_icon_job_command_fk1`;
ALTER TABLE `ja_value_job_table` DROP FOREIGN KEY `ja_value_job_fk1`;
ALTER TABLE `ja_value_jobcon_table` DROP FOREIGN KEY `ja_value_jobcon_fk1`;
ALTER TABLE `ja_icon_reboot_table` DROP FOREIGN KEY `ja_icon_reboot_fk1`;
ALTER TABLE `ja_icon_release_table` DROP FOREIGN KEY `ja_icon_release_fk1`;
ALTER TABLE `ja_icon_task_table` DROP FOREIGN KEY `ja_icon_task_fk1`;
ALTER TABLE `ja_icon_value_table` DROP FOREIGN KEY `ja_icon_value_fk1`;
ALTER TABLE `ja_icon_zabbix_link_table` DROP FOREIGN KEY `ja_icon_zabbix_link_fk1`;
ALTER TABLE `ja_run_jobnet_summary_table` DROP FOREIGN KEY `ja_run_jobnet_summary_fk1`;
ALTER TABLE `ja_run_job_table` DROP FOREIGN KEY `ja_run_job_fk1`;
ALTER TABLE `ja_run_flow_table` DROP FOREIGN KEY `ja_run_flow_fk1`;
ALTER TABLE `ja_run_icon_agentless_table` DROP FOREIGN KEY `ja_run_icon_agentless_fk1`;
ALTER TABLE `ja_run_icon_calc_table` DROP FOREIGN KEY `ja_run_icon_calc_fk1`;
ALTER TABLE `ja_run_icon_end_table` DROP FOREIGN KEY `ja_run_icon_end_fk1`;
ALTER TABLE `ja_run_icon_extjob_table` DROP FOREIGN KEY `ja_run_icon_extjob_fk1`;
ALTER TABLE `ja_run_icon_fcopy_table` DROP FOREIGN KEY `ja_run_icon_fcopy_fk1`;
ALTER TABLE `ja_run_icon_fwait_table` DROP FOREIGN KEY `ja_run_icon_fwait_fk1`;
ALTER TABLE `ja_run_icon_if_table` DROP FOREIGN KEY `ja_run_icon_if_fk1`;
ALTER TABLE `ja_run_icon_info_table` DROP FOREIGN KEY `ja_run_icon_info_fk1`;
ALTER TABLE `ja_run_icon_jobnet_table` DROP FOREIGN KEY `ja_run_icon_jobnet_fk1`;
ALTER TABLE `ja_run_icon_job_table` DROP FOREIGN KEY `ja_run_icon_job_fk1`;
ALTER TABLE `ja_run_job_command_table` DROP FOREIGN KEY `ja_run_job_command_fk1`;
ALTER TABLE `ja_run_value_job_table` DROP FOREIGN KEY `ja_run_value_job_fk1`;
ALTER TABLE `ja_run_value_jobcon_table` DROP FOREIGN KEY `ja_run_value_jobcon_fk1`;
ALTER TABLE `ja_run_icon_reboot_table` DROP FOREIGN KEY `ja_run_icon_reboot_fk1`;
ALTER TABLE `ja_run_icon_release_table` DROP FOREIGN KEY `ja_run_icon_release_fk1`;
ALTER TABLE `ja_run_icon_task_table` DROP FOREIGN KEY `ja_run_icon_task_fk1`;
ALTER TABLE `ja_run_icon_value_table` DROP FOREIGN KEY `ja_run_icon_value_fk1`;
ALTER TABLE `ja_run_icon_zabbix_link_table` DROP FOREIGN KEY `ja_run_icon_zabbix_link_fk1`;
ALTER TABLE `ja_run_value_before_table` DROP FOREIGN KEY `ja_run_value_before_fk1`;
ALTER TABLE `ja_run_value_after_table` DROP FOREIGN KEY `ja_run_value_after_fk1`;
ALTER TABLE `ja_value_before_jobnet_table` DROP FOREIGN KEY `ja_value_before_jobnet_fk1`;
ALTER TABLE `ja_value_after_jobnet_table` DROP FOREIGN KEY `ja_value_after_jobnet_fk1`;


-- DROP INDEX

ALTER TABLE `ja_schedule_control_table` DROP INDEX `ja_schedule_control_idx1`;
ALTER TABLE `ja_run_jobnet_summary_table` DROP INDEX `ja_run_jobnet_summary_idx1`;
ALTER TABLE `ja_run_jobnet_summary_table` DROP INDEX `ja_run_jobnet_summary_idx2`;
ALTER TABLE `ja_run_jobnet_summary_table` DROP INDEX `ja_run_jobnet_summary_idx3`;
ALTER TABLE `ja_run_jobnet_table` DROP INDEX `ja_run_jobnet_idx1`;
ALTER TABLE `ja_run_jobnet_table` DROP INDEX `ja_run_jobnet_idx2`;
ALTER TABLE `ja_run_jobnet_table` DROP INDEX `ja_run_jobnet_idx3`;
ALTER TABLE `ja_run_jobnet_table` DROP INDEX `ja_run_jobnet_idx4`;
ALTER TABLE `ja_run_job_table` DROP INDEX `ja_run_job_idx1`;
ALTER TABLE `ja_run_job_table` DROP INDEX `ja_run_job_idx2`;
ALTER TABLE `ja_run_log_table` DROP INDEX `ja_run_log_idx1`;
ALTER TABLE `ja_run_log_table` DROP INDEX `ja_run_log_idx2`;
ALTER TABLE `ja_run_log_table` DROP INDEX `ja_run_log_idx3`;
ALTER TABLE `ja_send_message_table` DROP INDEX `ja_send_message_idx1`;
ALTER TABLE `ja_run_job_table` DROP INDEX `ja_run_job_idx3`;
ALTER TABLE `ja_run_icon_job_table` DROP INDEX `ja_run_icon_job_idx1`;
ALTER TABLE `ja_run_icon_job_table` DROP INDEX `ja_run_icon_job_idx2`;
ALTER TABLE `ja_run_value_before_table` DROP INDEX `ja_run_value_before_idx2`;
ALTER TABLE `ja_run_icon_reboot_table` DROP INDEX `ja_run_icon_reboot_idx2`;


-- DROP TABLE

DROP TABLE `ja_calendar_control_table`;
DROP TABLE `ja_calendar_detail_table`;
DROP TABLE `ja_filter_control_table`;
DROP TABLE `ja_schedule_control_table`;
DROP TABLE `ja_schedule_detail_table`;
DROP TABLE `ja_schedule_jobnet_table`;
DROP TABLE `ja_jobnet_control_table`;
DROP TABLE `ja_job_control_table`;
DROP TABLE `ja_flow_control_table`;
DROP TABLE `ja_icon_agentless_table`;
DROP TABLE `ja_icon_calc_table`;
DROP TABLE `ja_icon_end_table`;
DROP TABLE `ja_icon_extjob_table`;
DROP TABLE `ja_icon_fcopy_table`;
DROP TABLE `ja_icon_fwait_table`;
DROP TABLE `ja_icon_if_table`;
DROP TABLE `ja_icon_info_table`;
DROP TABLE `ja_icon_jobnet_table`;
DROP TABLE `ja_icon_job_table`;
DROP TABLE `ja_job_command_table`;
DROP TABLE `ja_value_job_table`;
DROP TABLE `ja_value_jobcon_table`;
DROP TABLE `ja_icon_reboot_table`;
DROP TABLE `ja_icon_release_table`;
DROP TABLE `ja_icon_task_table`;
DROP TABLE `ja_icon_value_table`;
DROP TABLE `ja_icon_zabbix_link_table`;
DROP TABLE `ja_define_value_jobcon_table`;
DROP TABLE `ja_define_extjob_table`;
DROP TABLE `ja_run_jobnet_summary_table`;
DROP TABLE `ja_run_jobnet_table`;
DROP TABLE `ja_run_job_table`;
DROP TABLE `ja_run_flow_table`;
DROP TABLE `ja_run_icon_agentless_table`;
DROP TABLE `ja_run_icon_calc_table`;
DROP TABLE `ja_run_icon_end_table`;
DROP TABLE `ja_run_icon_extjob_table`;
DROP TABLE `ja_run_icon_fcopy_table`;
DROP TABLE `ja_run_icon_fwait_table`;
DROP TABLE `ja_run_icon_if_table`;
DROP TABLE `ja_run_icon_info_table`;
DROP TABLE `ja_run_icon_jobnet_table`;
DROP TABLE `ja_run_icon_job_table`;
DROP TABLE `ja_run_job_command_table`;
DROP TABLE `ja_run_value_job_table`;
DROP TABLE `ja_run_value_jobcon_table`;
DROP TABLE `ja_run_icon_reboot_table`;
DROP TABLE `ja_run_icon_release_table`;
DROP TABLE `ja_run_icon_task_table`;
DROP TABLE `ja_run_icon_value_table`;
DROP TABLE `ja_run_icon_zabbix_link_table`;
DROP TABLE `ja_run_value_before_table`;
DROP TABLE `ja_run_value_after_table`;
DROP TABLE `ja_value_before_jobnet_table`;
DROP TABLE `ja_value_after_jobnet_table`;
DROP TABLE `ja_session_table`;
DROP TABLE `ja_run_log_table`;
DROP TABLE `ja_define_run_log_message_table`;
DROP TABLE `ja_send_message_table`;
DROP TABLE `ja_index_table`;
DROP TABLE `ja_parameter_table`;
DROP TABLE `ja_host_lock_table`;
DROP TABLE `ja_object_lock_table`;
