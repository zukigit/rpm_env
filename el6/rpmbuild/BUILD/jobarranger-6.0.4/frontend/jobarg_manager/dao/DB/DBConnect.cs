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
using System.Data.Odbc;
using System.Collections;
using jp.co.ftf.jobcontroller.Common;

namespace jp.co.ftf.jobcontroller.DAO
{
    public class DBConnect
    {
        private OdbcConnection connection;
        private OdbcTransaction tran;
        private String dsn;
        private String userID;
        private String userPass;
        private String connectStr;
        private Boolean opened;
        private Boolean tranBeginned;

        /// <summary>DBコマンド</summary>
        private OdbcCommand _sqlCommand = null;

        private Hashtable _mapSelectSql = new Hashtable();

        private ArrayList _arrSelectSql = new ArrayList();

        private ArrayList _arrExecuteSql = new ArrayList();

        private String ConnectConfirmSql = "select userid from users where 1<>1";

        public DBConnect(String dsn, String userID, String userPass)
        {
            this.dsn = dsn;
            this.userID = userID;
            this.userPass = userPass;
            this.connectStr = "DSN=" + dsn + ";UID=" + userID + ";PWD=" + userPass + "; Connect Timeout=300";
            this.opened = false;
        }

        public DBConnect(String connectStr)
        {
            this.connectStr = connectStr;
            this.opened = false;
        }

        public void CreateSqlConnect()
        {
            if (connection == null)
            {
                connection = new OdbcConnection();
                connection.ConnectionString = connectStr;
                _sqlCommand = connection.CreateCommand();
            }

            if (!opened)
            {
                connection.Open();
                opened = true;
            }

        }
        public void BeginTransaction()
        {

            if (tranBeginned) TransactionRollback();
            tran = connection.BeginTransaction();
            tranBeginned = true;
        }

        public void TransactionCommit()
        {
            tran.Commit();
            tranBeginned = false;
        }

        public void TransactionRollback()
        {
            if (tranBeginned)
            {
                try
                {
                    tran.Rollback();

                }
                catch (InvalidOperationException ex)
                {
                    // DO nothing here
                }
                catch (Exception ex)
                {
                    this.CloseSqlConnect();
                    throw new DBException(Consts.SYSERR_001, ex);
                }
            }
            tranBeginned = false;
        }

        public void CloseSqlConnect()
        {
            if (connection != null)
            {
                connection.Close();
                opened = false;
                //connection = null;
            }
        }

        private void Select(OdbcCommand command, DataTable dt)
        {
            OdbcDataAdapter adapter = new OdbcDataAdapter(command);
            adapter.Fill(dt);
            adapter.Dispose();
            command.Dispose();
        }


        /// <summary>主キーの取得</summary>
        /// <param name="dt">実体クラスに応じてテーブル</param>
        /// <param name="arrPk"> 主キー</param>
        /// <return>更新件数</return>
        private DataColumn[] GetPrimaryKey(ref DataTable dt, string[] arrPk)
        {
            int arrIndex = 0;
            int arrLength = arrPk.Length;

            DataColumn[] colPk = new DataColumn[arrLength];

            foreach (string pkValue in arrPk)
            {
                colPk.SetValue(dt.Columns[pkValue], arrIndex);
                arrIndex++;
            }

            return colPk;
        }

        /// <summary>データの更新件数を取得</summary>
        /// <param name="strSql">SQL文</param>
        /// <return>更新件数</return>
        public int ExecuteNonQuery(string strSql)
        {
            int intNum = -1;

            try
            {
            _sqlCommand.CommandText = strSql;
            _sqlCommand.Transaction = tran;
            intNum = _sqlCommand.ExecuteNonQuery();

            return intNum;
            }
            catch (Exception ex)
            {
                this.TransactionRollback();
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }
        /// <summary>データの更新件数を取得</summary>
        /// <param name="dt">データレコード</param>
        /// <param name="baseEntity">DB操作用の実体クラス</param>
        /// <return>更新件数</return>
        public int ExecuteNonQuery(DataTable dt, BaseDAO baseEntity)
        {
            int intNum = -1;

            try
            {
                // 対象データはありません。

                if (dt == null || baseEntity == null)
                {
                    throw new DBException(Consts.SYSERR_001, null);
                }

                // テーブルの名前を再設定
                dt.TableName = baseEntity.TableName;

                // テーブルのキーを設定
                dt.PrimaryKey = this.GetPrimaryKey(ref dt, baseEntity.PrimaryKey);

                using (OdbcDataAdapter sqlAdapter = new OdbcDataAdapter())
                {
                    _sqlCommand.CommandText = baseEntity.SelectSql;
                    _sqlCommand.Transaction = tran;
                    sqlAdapter.SelectCommand = _sqlCommand;

                    OdbcCommandBuilder commandBuilder = new OdbcCommandBuilder(sqlAdapter);
                    commandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    commandBuilder.SetAllValues = false;
                    sqlAdapter.MissingSchemaAction = MissingSchemaAction.Ignore;
                    intNum = sqlAdapter.Update(dt);
                }

                return intNum;
            }
            catch (Exception ex)
            {

                this.TransactionRollback();
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの更新件数を取得</summary>
        /// <param name="strSql">SQL文</param>
        /// <param name="sqlParams">SQL文内のパラメータ</param>
        /// <return>更新件数</return>
        public int ExecuteNonQuery(string strSql, List<ComSqlParam> sqlParams)
        {
            int intNum = -1;

            try
            {
            _sqlCommand.CommandText = strSql;
            _sqlCommand.Transaction = tran;

            if (sqlParams != null)
            {
                _sqlCommand.Parameters.Clear();
                foreach (ComSqlParam param in sqlParams)
                {
                    OdbcParameter dbParam = new OdbcParameter();
                    //dbParam.DbType = param.ParamType;
                    dbParam.ParameterName = param.ParamName;
                    dbParam.Value = param.ParamValue;
                    _sqlCommand.Parameters.Add(dbParam);
                }
            }
            intNum = _sqlCommand.ExecuteNonQuery();

            return intNum;
            }
            catch (Exception ex)
            {
                this.TransactionRollback();
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの更新件数を取得</summary>
        /// <return>更新件数</return>
        public int ExecuteBatchUpdate()
        {
            int intNum = -1;

            try
            {
                // 実行しておくＳＱＬ文がない場合

                if (_arrExecuteSql.Count <= 0)
                {
                    return intNum;
                }

                for (int i = 0; i < _arrExecuteSql.Count ; i++)
                {
                    string strSql = Convert.ToString(_arrExecuteSql[i]);
                    _sqlCommand.CommandText = strSql;
                    _sqlCommand.Transaction = tran;
                    intNum = _sqlCommand.ExecuteNonQuery();
                    intNum = intNum + 1;
                }

                // 実行しておくＳＱＬ文を削除
                this.ClearBatch();

                return intNum;

            }
            catch (Exception ex)
            {
                this.TransactionRollback();
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの検索処理</summary>
        /// <return>検索結果</return>
        public DataSet ExecuteBatchQuery()
        {

            DataSet ds = new DataSet();

            try
            {
                // 実行しておくSQLがない場合

                if (_arrSelectSql.Count <= 0)
                {
                    return ds;
                }

                using (OdbcDataAdapter sqlAdapter = new OdbcDataAdapter())
                {
                    for (int i = 0; i < _mapSelectSql.Count; i++)
                    {
                        string tName = Convert.ToString(_arrSelectSql[i]);

                        _sqlCommand.CommandText = Convert.ToString(_mapSelectSql[tName]);
                        sqlAdapter.SelectCommand = _sqlCommand;
                        sqlAdapter.Fill(ds, tName);
                    }

                    // 実行しておくSQLを削除
                    this.ClearSelectBatch();
                }

                return ds;

            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの検索処理</summary>
        /// <param name="strSql">SQL文</param>
        /// <return>検索結果</return>
        public DataTable ExecuteQuery(string strSql)
        {
            return ExecuteQuery(strSql, "");
        }

        /// <summary>ヘルスチェック処理</summary>
        public void ExecuteHealthCheck()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OdbcDataAdapter sqlDataAdapter = new OdbcDataAdapter())
                {
                    _sqlCommand.CommandText = ConnectConfirmSql;
                    _sqlCommand.Transaction = tran;
                    sqlDataAdapter.SelectCommand = _sqlCommand;
                    sqlDataAdapter.Fill(dt);
                }

            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        /// <summary>ヘルスチェック処理</summary>
        public DBException exExecuteHealthCheck()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OdbcDataAdapter sqlDataAdapter = new OdbcDataAdapter())
                {
                    _sqlCommand.CommandText = ConnectConfirmSql;
                    _sqlCommand.Transaction = tran;
                    sqlDataAdapter.SelectCommand = _sqlCommand;
                    sqlDataAdapter.Fill(dt);
                }
                return new DBException("", null);
                //return null;
            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                return new DBException(Consts.SYSERR_001, ex);
            }
        }


        /// <summary>データの検索処理</summary>
        /// <param name="strSql">SQL文</param>
        /// <param name="tableName">テーブル名前</param>
        /// <return>検索結果</return>
        public DataTable ExecuteQuery(string strSql, string tableName)
        {
            DataTable dt = new DataTable();

            try
            {
            using (OdbcDataAdapter sqlDataAdapter = new OdbcDataAdapter())
            {
                _sqlCommand.CommandText = strSql;
                _sqlCommand.Transaction = tran;
                sqlDataAdapter.SelectCommand = _sqlCommand;
                sqlDataAdapter.Fill(dt);
                if (!String.IsNullOrEmpty(tableName))
                {
                    dt.TableName = tableName;
                }
            }

            return dt;
            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの検索処理</summary>
        /// <param name="strSql">SQL文</param>
        /// <param name="sqlParams">SQL文内のパラメータ</param>
        /// <return>検索結果</return>
        public DataTable ExecuteQuery(string strSql, List<ComSqlParam> sqlParams)
        {
            return ExecuteQuery(strSql, sqlParams, "");
        }

        /// <summary>データの検索処理</summary>
        /// <param name="strSql">SQL文</param>
        /// <param name="sqlParams">SQL文内のパラメータ</param>
        /// <param name="tableName">テーブル名前</param>
        /// <return>検索結果</return>
        public DataTable ExecuteQuery(string strSql, List<ComSqlParam> sqlParams,
                                    string tableName)
        {
            DataTable dt = new DataTable();

            try
            {
            using (OdbcDataAdapter sqlAdapter = new OdbcDataAdapter())
            {
                _sqlCommand.CommandText = strSql;
                _sqlCommand.Transaction = tran;
                sqlAdapter.SelectCommand = _sqlCommand;
                if (sqlParams != null)
                {
                    sqlAdapter.SelectCommand.Parameters.Clear();
                    foreach (ComSqlParam param in sqlParams)
                    {
                        OdbcParameter dbParam = new OdbcParameter();
                        //dbParam.DbType = param.ParamType;
                        dbParam.ParameterName = param.ParamName;
                        dbParam.Value = param.ParamValue;
                        sqlAdapter.SelectCommand.Parameters.Add(dbParam);
                    }
                }
                sqlAdapter.Fill(dt);
                if (!String.IsNullOrEmpty(tableName))
                {
                    dt.TableName = tableName;
                }
            }

            return dt;
            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        /// <summary>データの検索処理</summary>
        /// <param name="strSql">SQL文</param>
        /// <param name="intStart">データの開始行</param>
        /// <param name="intCount">データの取得件数</param>
        /// <param name="tableName">テーブルの名前</param>
        /// <param name="sqlParams">SQL文内のパラメータ</param>
        /// <return>検索結果</return>
        public DataSet ExecuteQuery(string strSql, int intStart, int intCount,
                         string tableName, List<ComSqlParam> sqlParams)
        {
            DataSet ds = new DataSet();

            try
            {
                using (OdbcDataAdapter sqlAdapter = new OdbcDataAdapter())
                {
                    _sqlCommand.CommandText = strSql;
                    sqlAdapter.SelectCommand = _sqlCommand;
                    if (sqlParams != null)
                    {
                        sqlAdapter.SelectCommand.Parameters.Clear();
                        foreach (ComSqlParam param in sqlParams)
                        {
                            OdbcParameter dbParam = new OdbcParameter();
                            //dbParam.DbType = param.ParamType;
                            dbParam.ParameterName = param.ParamName;
                            dbParam.Value = param.ParamValue;
                            sqlAdapter.SelectCommand.Parameters.Add(dbParam);
                        }
                    }
                    sqlAdapter.Fill(ds, intStart, intCount, tableName);
                }

                return ds;

            }
            catch (Exception ex)
            {
                this.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_001, ex);
            }
        }

        #region ＳＱＬ文の一時保存


        /// <summary>データの操作（登録、更新、削除）時、実行しておくＳＱＬ文を保存</summary>
        /// <param name="strSql">実行しておくSQL文</param>
        public void AddBatch(string strSql)
        {
            _arrExecuteSql.Add(strSql);
        }

        /// <summary>データの検索時、実行しておくＳＱＬ文を保存</summary>
        /// <param name="strSql">実行しておくSQL文</param>
        /// <param name="tName">テーブルの名前</param>
        public bool AddSelectBatch(string strSql, string tName)
        {
            if (_mapSelectSql.ContainsKey(tName))
            {
                return false;
            }

            _mapSelectSql.Add(tName, strSql);
            _arrSelectSql.Add(tName);
            return true;
        }

        /// <summary>データの操作完了の時、実行しておくＳＱＬを削除</summary>
        public void ClearBatch()
        {
            _arrExecuteSql.Clear();
            _arrExecuteSql.TrimToSize();
        }

        /// <summary>データの検索完了の時、実行しておくＳＱＬを削除</summary>
        public void ClearSelectBatch()
        {
            _arrSelectSql.Clear();
            _arrSelectSql.TrimToSize();
            _mapSelectSql.Clear();
        }

        #endregion

        #region トランザクションの判定


        /// <summary>トランザクションの判定</summary>
        /// <return>「True:トランザクションON」「False:トランザクションOFF」</return>
        public bool isBeginTransaction()
        {
            return tranBeginned;
        }
        #endregion
    }
}
