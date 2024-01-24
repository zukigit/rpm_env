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
    /// <>
    /// ユーザーテーブルのDAOクラス
    /// </>
    public class RunJobnetDAO : BaseDAO
    {
        #region フィールド


        private string _tableName = "ja_run_jobnet_table";

        private string[] _primaryKey = { "inner_jobnet_id" };

        private string _selectSql = "select * from ja_run_jobnet_table where 0!=0";

        private string _selectSqlByPk = "select * from ja_run_jobnet_table " +
                                        "where inner_jobnet_id = ? ";


        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public RunJobnetDAO(DBConnect db)
        {
            _db = db;
        }
        #endregion

        #region プロパティ

        /// <>　テーブル名前 </>
        public override string TableName
        {
            get
            {
                return _tableName;
            }
        }

        /// <>　キー </>
        public override string[] PrimaryKey
        {
            get
            {
                return _primaryKey;
            }
        }

        /// <> 検索用のSQL </>
        public override string SelectSql
        {
            get
            {
                return _selectSql;
            }
        }

        /// <> 検索条件を指定したSQL文 </>
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
        /// <> テーブルの構築</>
        /// <return>テーブルの結構</return>
        //************************************************************************
        public DataTable GetEmptyTable()
        {
            _db.CreateSqlConnect();

            DataTable dt = _db.ExecuteQuery(SelectSql);

            return dt;

        }

        //************************************************************************
        /// <> データの取得</>
        /// <param name="user_id">ユーザーＩＤ</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object user_id)
        {
            _db.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.UInt64, "inner_jobnet_id", user_id));

            DataTable dt = _db.ExecuteQuery(this._selectSqlByPk, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
