
-- Job Arranger upgrade table SQL for PostgreSQL (Ver 6.0.2.3)  - 2023/08/03 -

-- Copyright (C) 2012-2014 FitechForce, Inc. All Rights Reserved.
-- Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.

DROP INDEX IF EXISTS ja_run_job_idx3;

CREATE INDEX ja_run_job_idx3 ON ja_run_job_table (job_type,status);
CREATE UNIQUE INDEX ja_run_icon_job_idx2 ON ja_run_icon_job_table (inner_job_id,host_flag);