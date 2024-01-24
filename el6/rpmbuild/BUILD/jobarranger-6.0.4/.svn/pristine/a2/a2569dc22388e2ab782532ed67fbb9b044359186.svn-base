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
//  * @author Yam       2014/08/18 新規作成<BR>                     *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.DAO
{
    /// <summary>
    /// ジョブコマンド設定テーブルのDAOクラス
    /// </summary>
    public class ParameterTableDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_parameter_table";

        private string[] _primaryKey = { "parameter_name" };

        private string _selectSql = "select * from ja_parameter_table where 0!=0";

        private string _selectSqlByPk = "select * from ja_parameter_table where parameter_name = ? ";

        /// <summary>ロック </summary>
        private string _selectWithLock = "select * from ja_parameter_table where parameter_name = ? for update nowait ";


        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public ParameterTableDAO(DBConnect db)
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
        /// <summary>パラメータテーブルロック取得</summary>
        /// <param name="windowId">画面ID</param>
        /// <param name="dbType">DB種別</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock(object windowId, Consts.DBTYPE dbType)
        {
            if (dbType == Consts.DBTYPE.MYSQL)
            {
                return GetLock4Mysql(windowId);
            }
            return GetLock4NotMysql(windowId);

        }

        //************************************************************************
        /// <summary>MysqlDBのIDデータロック取得</summary>
        /// <param name="windowId">画面ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock4Mysql(object windowId)
        {
            int count = 0;
            string _getLock = "SELECT GET_LOCK('ja_parameter_table." + windowId + "', 0) as count";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@windowId", windowId));
            DataTable dt = _db.ExecuteQuery(_getLock, sqlParams, TableName);

            count = int.Parse(Convert.ToString(dt.Rows[0]["count"]));
            if (count < 1)
            {
                RealseLock(windowId);
                _db.CloseSqlConnect();
                throw new DBException();
            }
            return count;

        }

        //************************************************************************
        /// <summary>NotMysqlDBのIDデータロック取得</summary>
        /// <param name="windowId">画面ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock4NotMysql(object windowId)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@windowId", windowId));
            DataTable dt = _db.ExecuteQuery(_selectWithLock, sqlParams, TableName);

            return dt.Rows.Count;

        }

        //************************************************************************
        /// <summary>MysqlDBのIDデータロック開放</summary>
        /// <param name="windowId">画面ID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public String RealseLock(object windowId)
        {
            string count = "";

            string _releaseLock = "SELECT RELEASE_LOCK('ja_parameter_table." + windowId + "') as count";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@windowId", windowId));
            DataTable dt = _db.ExecuteQuery(_releaseLock, sqlParams, TableName);

            if (dt != null)
            {
                count = Convert.ToString(dt.Rows[0]["count"]);
            }

            return count;
        }


        //added by YAMA 2014/08/18
        public  void SetJaParameterTable(String parameter_name, String value)
        {
            String updateSql = "update ja_parameter_table set value = ? where parameter_name = ? ";

            //String updateSql = "update ja_parameter_table set value = " + "'" + value + "'" + " where parameter_name = " + "'" + parameter_name + "'" ;


            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.String, "@value", value));
            sqlParams.Add(new ComSqlParam(DbType.String, "@parameter_name", parameter_name));
            //DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            _db.ExecuteNonQuery(updateSql, sqlParams);

        }

        #endregion
    }
}
