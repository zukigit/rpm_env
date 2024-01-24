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
    /// ジョブ変数設定テーブルのDAOクラス
    /// </summary>
    public class RunValueJobDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_run_value_job_table";

        private string[] _primaryKey = { "inner_job_id", "value_name" };

        private string _selectSql = "select * from ja_run_value_job_table where 0!=0";

        private string _selectSqlByPk = "select * from ja_run_value_job_table " +
                                        "where inner_job_id = ? " +
                                        "and value_name = ?";

        private string _selectSqlByJobnet = "select * from ja_run_value_job_table " +
                        "where inner_jobnet_id = ? ";

        private string _selectSqlByJob = "select * from ja_run_value_job_table " +
                        "where inner_job_id = ? ";

        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public RunValueJobDAO(DBConnect db)
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

            DataTable dt = _db.ExecuteQuery(SelectSql);

            return dt;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="inner_job_id">実行用ジョブ内部管理ID</param>
        /// <param name="value_name">変数名</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object inner_job_id, object value_name)
        {
            _db.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_job_id", inner_job_id));
            sqlParams.Add(new ComSqlParam(DbType.String, "@value_name", value_name));
            DataTable dt = _db.ExecuteQuery(this._selectSqlByPk, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary>
        /// データの取得.
        /// </summary>
        /// <param name="inner_jobnet_id">実行用ジョブネット内部管理ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByJobnet(object inner_jobnet_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", inner_jobnet_id));

            DataTable dt = _db.ExecuteQuery(_selectSqlByJobnet, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary>
        /// データの取得.
        /// </summary>
        /// <param name="inner_job_id">実行用ジョブ内部管理ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByJobId(object inner_job_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", inner_job_id));

            DataTable dt = _db.ExecuteQuery(_selectSqlByJob, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
