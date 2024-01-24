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

namespace jp.co.ftf.jobcontroller.DAO
{
    public class ImportDoubleRelationCheck
    {
        private string _tableName;
        private string[] _keys;
        private string _refTableName;
        private string[] _refKeys;

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }
        public string[] Keys
        {
            get { return _keys; }
            set { _keys = value; }
        }
        public string RefTableName
        {
            get { return _refTableName; }
            set { _refTableName = value; }
        }
        public string[] RefKeys
        {
            get { return _refKeys; }
            set { _refKeys = value; }
        }

        public ImportDoubleRelationCheck(String tableName, String[] keys, String refTableName, String[] refKeys)
        {
            _tableName = tableName;
            _keys = keys;
            _refTableName = refTableName;
            _refKeys = refKeys;
        }
    }
}
