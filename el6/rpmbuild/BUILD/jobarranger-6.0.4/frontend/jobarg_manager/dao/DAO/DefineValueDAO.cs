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
using System.Data;
using System.Text;
using jp.co.ftf.jobcontroller.Common;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/19 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.DAO
{
    /// <summary>
    /// ジョブコントローラ変数定義テーブルのDAOクラス
    /// </summary>
    public class DefineValueDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_define_value_jobcon_table";

        private string[] _primaryKey = { "jobnet_id", "job_id", "update_date", "value_name" };

        private string _selectSql = "select * from ja_define_value_jobcon_table where 0!=0";

        private string _selectAllSql = "select value_name from ja_define_value_jobcon_table " +
                                       "order by value_name ASC";

        private string _selectSqlByPk = "select * from ja_define_value_jobcon_table " +
                                        "where value_name = ? ";

        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public DefineValueDAO(DBConnect db)
        {
            _db = db;
        }

        #endregion

        #region プロパティ

        /// <summary>　テーブル名前 </summary>
        public override string TableName
        {
            get
            {
                return _tableName;
            }
        }

        /// <summary>　キー </summary>
        public override string[] PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
        }

        /// <summary> 検索用のSQL </summary>
        public override string SelectSql
        {
            get
            {
                return _selectSql;
            }
        }

        /// <summary> 検索条件を指定したSQL文 </summary>
        public override string SelectSqlByPk
        {
            get
            {
                return _selectSqlByPk;
            }
        }

        #endregion

        #region publicメソッド

        //************************************************************************
        /// <summary> テーブルの構築</summary>
        /// <return>テーブルの結構</return>
        //************************************************************************
        public DataTable GetEmptyTable()
        {
            _db.CreateSqlConnect();

            DataTable dt = _db.ExecuteQuery(this._selectSql);

            return dt;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByNone()
        {
            _db.CreateSqlConnect();

            DataTable dt = _db.ExecuteQuery(this._selectAllSql, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="value_name">ジョブコントローラ変数名</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object value_name)
        {
            _db.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@value_name", value_name));

            DataTable dt = _db.ExecuteQuery(this._selectSqlByPk, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
