/*
** Job Arranger Manager
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
** Copyright (C) 2021 Daiwa Institute of Research Ltd. All Rights Reserved.
**
** Licensed to the Apache Software Foundation (ASF) under one or more
** contributor license agreements. See the NOTICE file distributed with
** this work for additional information regarding copyright ownership.
** The ASF licenses this file to you under the Apache License, Version 2.0
** (the "License"); you may not use this file except in compliance with
** the License. You may obtain a copy of the License at
**
** http://www.apache.org/licenses/LICENSE-2.0
**
** Unless required by applicable law or agreed to in writing, software
** distributed under the License is distributed on an "AS IS" BASIS,
** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
** See the License for the specific language governing permissions and
** limitations under the License.
**
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace jp.co.ftf.jobcontroller.JobController
{
    public class JobnetExecInfo
    {
        public decimal inner_jobnet_id { get; set; }
        public String jobnet_id { get; set; }
        public String jobnet_name { get; set; }
        public int status { get; set; }
        public String display_status { get; set; }
        public int run_type { get; set; }
        public String job_id { get; set; }
        public String job_name { get; set; }
        public SolidColorBrush status_color { get; set; }
        public String scheduled_time { get; set; }
        public String start_time { get; set; }
        public String end_time { get; set; }

        //added by YAMA 2014/04/25
        public int load_status { get; set; }

        //added by YAMA 2014/07/01
        public String Foreground_color { get; set; }

        //added by YAMA 2014/09/22  実行中ジョブID表示
        public String running_job_id { get; set; }
        public String running_job_name { get; set; }

        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        public int start_pending_flag { get; set; }

        public int jobnet_timeout { get; set; }
        public String jobnet_timeout_run_type { get; set; }

        public String schedule_id { get; set; }
    }
}
