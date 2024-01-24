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

namespace jp.co.ftf.jobcontroller.Common
{
    public static class LoginSetting
    {
        public static string _jobconName;
        public static string JobconName
        {
            get
            {
                return _jobconName;
            }
            set
            {
                _jobconName = value;
            }
        }
        public static string _connectStr;
        public static string ConnectStr
        {
            get
            {
                return _connectStr;
            }
            set
            {
                _connectStr = value;
            }
        }

        public static string _userName;
        public static string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public static decimal _userID;
        public static decimal UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }

        public static Consts.AuthorityEnum _authority;
        public static Consts.AuthorityEnum Authority
        {
            get
            {
                return _authority;
            }
            set
            {
                _authority = value;
            }
        }

        public static Consts.ActionMode _mode;
        public static Consts.ActionMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        public static List<Decimal> _groupList;
        public static List<Decimal> GroupList
        {
            get
            {
                return _groupList;
            }
            set
            {
                _groupList = value;
            }
        }
        public static Consts.DBTYPE DBType {get; set;}

        public static String Lang { get; set; }

        public static bool HealthCheckFlag { get; set; }

        public static int HealthCheckInterval { get; set; }

        //added by YAMA 2014/03/03
        public static int JaZabbixVersion { get; set; }

        // added by YAMA 2014/10/20    マネージャ内部時刻同期
        public static int ManagerTimeSync { get; set; }

        // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
        // false：グループ所属無しユーザー
        public static bool BelongToUsrgrpFlag { get; set; }

    }

}
