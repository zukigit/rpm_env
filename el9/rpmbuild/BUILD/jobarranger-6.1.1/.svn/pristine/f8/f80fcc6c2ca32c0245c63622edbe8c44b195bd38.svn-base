
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 6.1.0-6.1.1)  - 2017/01/30 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
DROP INDEX IF EXISTS ja_run_job_idx3;

CREATE INDEX IF NOT EXISTS ja_run_job_idx3 ON ja_run_job_table (job_type, status);
CREATE INDEX IF NOT EXISTS ja_run_icon_job_idx1 ON ja_run_icon_job_table (host_flag, host_name);
CREATE UNIQUE INDEX IF NOT EXISTS ja_run_icon_job_idx2 ON ja_run_icon_job_table (inner_job_id, host_flag);
CREATE UNIQUE INDEX IF NOT EXISTS ja_run_value_before_idx2 ON ja_run_value_before_table (inner_job_id, value_name, seq_no);
CREATE UNIQUE INDEX IF NOT EXISTS ja_run_icon_reboot_idx2 ON ja_run_icon_reboot_table (inner_job_id, host_flag);
