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
using System.Data;
using System.Text;
using System.Data.Common;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 郭 暁宇 2012/10/15 新規作成<BR>                    *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.DAO
{
    /// <summary>
    /// SQLへのパラメータについてのクラス.
    /// </summary>
    public class ComSqlParam
    {
        #region フィールド

        /// <summary>パラメータの型</summary>
        private DbType _paramType ;

        /// <summary>パラメータの入力、出力</summary>
        private ParameterDirection _paramDirection;

        /// <summary>パラメータの名前</summary>
        private string _paramName;

        /// <summary>パラメータの値</summary>
        private object _paramValue;

        #endregion

        #region プロパティ

        /// <summary>パラメータの型</summary>
        public DbType ParamType
        {
            get
            {
                return _paramType;
            }
            set
            {
                _paramType = value;
            }
        }

        // <summary>パラメータの入力、出力</summary>
        public ParameterDirection ParamDirection
        {
            get
            {
                return _paramDirection;
            }
            set
            {
                _paramDirection = value;
            }
        }

        /// <summary>パラメータの名前</summary>
        public string ParamName
        {
            get
            {
                return _paramName;
            }
            set
            {
                _paramName = value;
            }
        }

        /// <summary>パラメータの値</summary>
        public object ParamValue
        {
            get
            {
                return _paramValue;
            }
            set
            {
                _paramValue = value;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="paramType">パラメータの型</param>
        /// <param name="paramName">パラメータの名前</param>
        /// <param name="paramValue">パラメータの値</param>
        public ComSqlParam(DbType paramType, string paramName, object paramValue)
        {
            _paramType = paramType;
            _paramName = paramName;
            _paramValue = paramValue;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="paramType">パラメータの型</param>
        /// <param name="paramDirection">パラメータの入力、出力</param>
        /// <param name="paramName">パラメータの名前</param>
        /// <param name="paramValue">パラメータの値</param>
        public ComSqlParam(DbType paramType, ParameterDirection paramDirection,
                           string paramName, object paramValue)
        {
            _paramType = paramType;
            _paramDirection = paramDirection;
            _paramName = paramName;
            _paramValue = paramValue;
        }

        #endregion

    }
}
