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
using System.Data;
using jp.co.ftf.jobcontroller.Common;

namespace jp.co.ftf.jobcontroller.JobController
{
    public class JobconDBSource
    {
        private string _jobconName;
        private string _dbUser;
        private string _dbPass;
        private string _dbs;
        private string _connnectStr;
        private Consts.DBTYPE _dbType;
        private bool _healthCheckFlag;
        private int _healthCheckInterval;

        //added by YAMA 2014/03/03
        private int _jazabbixVersion;
        private string _zabbixURL;

        public string JobconName
        {
            get { return _jobconName; }
            set { _jobconName = value; }
        }
        public string DBUser
        {
            get { return _dbUser; }
            set { _dbUser = value; }
        }
        public string DBPass
        {
            get { return _dbPass; }
            set { _dbPass = value; }
        }
        public string DBS
        {
            get { return _dbs; }
            set { _dbs = value; }
        }
        public String ConnnectStr
        {
            get { return _connnectStr; }
            set { _connnectStr = value; }
        }

        public Consts.DBTYPE DBType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        public bool HealthCheckFlag
        {
            get { return _healthCheckFlag; }
            set { _healthCheckFlag = value; }
        }

        public int HealthCheckInterval
        {
            get { return _healthCheckInterval; }
            set { _healthCheckInterval = value; }
        }

        //added by YAMA 2014/03/03
        public int JaZabbixVersion
        {
            get { return _jazabbixVersion; }
            set { _jazabbixVersion = value; }
        }

        public String ZabbixUrl
        {
            get { return _zabbixURL; }
            set { _zabbixURL = value; }
        }


        public JobconDBSource(String jobconName, String dbUser, String dbPass, String dbs, Consts.DBTYPE dbType,
                                bool healthCheckFlag, int healthCheckInterval, int jazabbixVersion, String zabbixURL)
//        public JobconDBSource(String jobconName, String dbUser, String dbPass, String dbs, Consts.DBTYPE dbType,
//                                bool healthCheckFlag, int healthCheckInterval)
        {
            _jobconName = jobconName;
            _dbUser = dbUser;
            _dbPass = dbPass;
            _dbs = dbs;
            _dbType = dbType;
            _healthCheckFlag = healthCheckFlag;
            _healthCheckInterval = healthCheckInterval;

            //added by YAMA 2014/03/03
            _jazabbixVersion = jazabbixVersion;

            _zabbixURL = zabbixURL;

            _connnectStr = "DSN=" + dbs + ";UID=" + dbUser + ";PWD=" + dbPass + "; Connect Timeout=300"; ;
        }
        public override string ToString()
        {
            return _jobconName;
        }

        public static JobconDBSource Create(DataRow dr)
        {
            String jobconName = (String)dr["JobconName"];
            String dbUser = (String)dr["DBUser"];
            String dbPass = (String)dr["DBPassword"];
            String dbs = (String)dr["DBSource"];
            String strDbtype = (String)dr["DBType"];
            Consts.DBTYPE dbType = (Consts.DBTYPE)Convert.ToInt16(strDbtype);
            bool healthCheckFlag=true;
            if (dr["HealthCheckFlag"]!=null)
            {
                if (((String)dr["HealthCheckFlag"]).Equals("0"))
                    healthCheckFlag = false;
            }
            int healthCheckInterval = 5;
            if (dr["HealthCheckInterval"] != null)
            {
                healthCheckInterval = Convert.ToInt32(dr["HealthCheckInterval"]);
            }
            if (healthCheckFlag && healthCheckInterval == 0)
                healthCheckInterval = 5;

            int jazabbixVersion = 3;

            String strZabbixURL = (String)dr["ZabbixUrl"];


            //added by YAMA 2014/03/03
            return new JobconDBSource(jobconName, dbUser, dbPass, dbs, dbType, healthCheckFlag, healthCheckInterval, jazabbixVersion, strZabbixURL);
//            return new JobconDBSource(jobconName, dbUser, dbPass, dbs, dbType, healthCheckFlag, healthCheckInterval);
        }

    }
}
