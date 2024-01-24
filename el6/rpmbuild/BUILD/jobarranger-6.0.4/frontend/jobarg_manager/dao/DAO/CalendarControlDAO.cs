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
//  * @author KIM 2012/11/095 新規作成<BR>                    *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.DAO
{
    /// <summary>
    /// カレンダー管理テーブルのDAO
    /// </summary>
    public class CalendarControlDAO : ObjectBaseDAO
    {
        #region フィールド

        private string _tableName = "ja_calendar_control_table";

        private string[] _primaryKey = { "calendar_id", "update_date" };

        private string _selectSql = "select * from ja_calendar_control_table where 0!=0";

        /// <summary>ロック </summary>
        private string _selectWithLock = "select * from ja_calendar_control_table " +
            "where calendar_id = ? for update nowait ";

        /// <summary>ロックしない </summary>
        private string _selectCountByPk = "select count(1) as count from ja_calendar_control_table " +
            "where calendar_id = ? and update_date = ?";

        private string _selectCountByCalendarIdSql = "select count(1) as count from ja_calendar_control_table " +
            "where calendar_id = ?";

        private string _selectValidByCalendarIdSql = "select * from ja_calendar_control_table " +
            "where calendar_id = ? and valid_flag = '1'";

        private string _selectSqlByUserNm = "select calendar_id, calendar_name, MAX(update_date) from " +
            "ja_calendar_control_table where public_flag = '1' and user_name in (select username from users " +
            "where userid in (select distinct userid from users_groups where usrgrpid in (select b.usrgrpid " +
            "from users a inner join users_groups b on a.userid = b.userid where a.username = ?))) " +
            "order by calendar_id ASC";

        private string _selectSqlByPk = "select * from ja_calendar_control_table " +
            "where calendar_id = ? and update_date = ? ";

        private string _selectSqlByCalendarId = "select * from ja_calendar_control_table " +
            "where calendar_id = ? order by update_date desc";

        /// <summary>カレンダーIDの重複登録チェック用のSQL </summary>
        private string _selectSqlForCheck = "select count(1) as count from ja_calendar_control_table " +
             "where calendar_id = ?";

        /// <summary>カレンダーIDの最新データ取得用のSQL </summary>
        private string _selectMaxUpdateDateByCalendarIdSql = "select * from ja_calendar_control_table " +
             "where calendar_id = ? and update_date = (select max(update_date) from ja_calendar_control_table where calendar_id=?)";

        private string _deleteSqlByPk = "delete from ja_calendar_control_table " +
              "where calendar_id = ? and update_date = ? ";

        private string _selectSqlByUserId = "SELECT calendar.calendar_id, calendar.calendar_name, calendar.update_date " +
                                        "FROM ( " +
                                        "(" +
                                        "SELECT C.calendar_id, C.calendar_name, C.update_date " +
                                        "FROM ja_calendar_control_table as C " +
                                        "WHERE C.public_flag =1 " +
                                        "and C.update_date=" +
                                        "( SELECT MAX(D.update_date) " +
                                        "FROM ja_calendar_control_table AS D " +
                                        "WHERE D.calendar_id = C.calendar_id) " +
                                        ") " +
                                        "UNION (" +
                                        "SELECT A.calendar_id, A.calendar_name, A.update_date " +
                                        "FROM ja_calendar_control_table AS A, users AS U, users_groups AS UG1, users_groups AS UG2 " +
                                        "WHERE A.user_name = U.username " +
                                        "AND U.userid = UG1.userid " +
                                        "AND UG2.userid=? " +
                                        "AND UG1.usrgrpid = UG2.usrgrpid " +
                                        "AND A.update_date = ( " +
                                        "SELECT MAX( B.update_date ) " +
                                        "FROM ja_calendar_control_table AS B " +
                                        "WHERE B.calendar_id = A.calendar_id " +
                                        "GROUP BY B.calendar_id ) " +
                                        "AND A.public_flag =0" +
                                        ")" +
                                        ") AS calendar " +
                                        "ORDER BY calendar.calendar_id ";
        private string _selectSqlByUserIdSuper = "SELECT A.calendar_id,A.calendar_name,A.update_date " +
                                                "FROM ja_calendar_control_table AS A " +
                                                "WHERE A.update_date=" +
                                                "( SELECT MAX(B.update_date) " +
                                                "FROM ja_calendar_control_table AS B " +
                                                "WHERE A.calendar_id = B.calendar_id) " +
                                                "order by A.calendar_id";

        private DBConnect _db = null;

        #endregion

        #region コンストラクタ

        public CalendarControlDAO(DBConnect db)
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
        /// <summary>
        /// テーブルの構築.
        /// </summary>
        /// <return>テーブルの結構</return>
        //************************************************************************
        public DataTable GetEmptyTable()
        {

            DataTable dt = _db.ExecuteQuery(SelectSql);

            return dt;
        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetCountByPk(object calendar_id, object update_date)
        {
            int count = 0;

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "@update_date", update_date));
            DataTable dt = _db.ExecuteQuery(_selectCountByPk, sqlParams, TableName);

            if (dt != null)
                count = int.Parse(Convert.ToString(dt.Rows[0]["count"]));

            return count;
        }

        //************************************************************************
        /// <summary>MysqlDBのIDデータロック取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock4Mysql(object calendar_id)
        {
            int count = 0;
            string _getLock = "SELECT GET_LOCK('ja_calendar_control_table." + calendar_id + "', 0) as count";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_getLock, sqlParams, TableName);

            count = int.Parse(Convert.ToString(dt.Rows[0]["count"]));
            if (count < 1)
            {
                RealseLock(calendar_id);
                _db.CloseSqlConnect();
                throw new DBException();
            }
            return count;

        }

        //************************************************************************
        /// <summary>NotMysqlDBのIDデータロック取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock4NotMysql(object calendar_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectWithLock, sqlParams, TableName);

            return dt.Rows.Count;

        }

        //************************************************************************
        /// <summary>IDデータロック取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="dbType">DB種別</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int GetLock(object calendar_id, Consts.DBTYPE dbType)
        {
            if (dbType == Consts.DBTYPE.MYSQL)
            {
                return GetLock4Mysql(calendar_id);
            }
            return GetLock4NotMysql(calendar_id);

        }


        //************************************************************************
        /// <summary>MysqlDBのIDデータロック開放</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public String RealseLock(object calendar_id)
        {
            string count = "";

            string _releaseLock = "SELECT RELEASE_LOCK('ja_calendar_control_table." + calendar_id + "') as count";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_releaseLock, sqlParams, TableName);

            if (dt != null)
            {
                count = Convert.ToString(dt.Rows[0]["count"]);
            }

            return count;
        }

        //************************************************************************
        /// <summary>IDデータ件数の取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public string GetCountByCalendarId(object calendar_id)
        {
            string count = "";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectCountByCalendarIdSql, sqlParams, TableName);

            if (dt != null)
                count = Convert.ToString(dt.Rows[0]["count"]);

            return count;
        }

        //************************************************************************
        /// <summary> データの取得(カレンダーIDの重複チェック用)</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public string GetCountForCheck(object calendar_id)
        {
            string count = "";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectSqlForCheck, sqlParams, TableName);

            if (dt != null)
                count = Convert.ToString(dt.Rows[0]["count"]);

            return count;
        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetInfoByUserNm(object user_name)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "username", user_name));
            DataTable dt = _db.ExecuteQuery(_selectSqlByUserNm, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> 有効データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetValidEntityById(object calendar_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectValidByCalendarIdSql, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByPk(object calendar_id, object update_date)
        {

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "@update_date", update_date));
            DataTable dt = _db.ExecuteQuery(_selectSqlByPk, sqlParams, TableName);

            return dt;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetEntityByCalendarId(object calendar_id)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectSqlByCalendarId, sqlParams, TableName);

            return dt;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="userid">ユーザーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetInfoByUserId(object userid)
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@userid", userid));
            DataTable dt = _db.ExecuteQuery(_selectSqlByUserId, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="userid">ユーザーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetInfoByUserIdSuper()
        {
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            DataTable dt = _db.ExecuteQuery(_selectSqlByUserIdSuper, sqlParams, TableName);

            return dt;
        }

        //************************************************************************
        /// <summary>
        /// データを削除.
        /// </summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="update_date">更新日</param>
        /// <return>検索結果</return>
        //************************************************************************
        public int DeleteByPk(object calendar_id, object update_date)
        {

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            sqlParams.Add(new ComSqlParam(DbType.Int64, "@update_date", update_date));
            int count = _db.ExecuteNonQuery(_deleteSqlByPk, sqlParams);

            return count;

        }

        //************************************************************************
        /// <summary> データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public override DataTable GetEntityByObjectId(object calendar_id)
        {
            DataTable dt = GetEntityByCalendarId(calendar_id);

            return dt;
        }

        //************************************************************************
        /// <summary> 最新データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetMaxUpdateDateEntityById(object calendar_id)
        {

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            sqlParams.Add(new ComSqlParam(DbType.String, "@calendar_id", calendar_id));
            DataTable dt = _db.ExecuteQuery(_selectMaxUpdateDateByCalendarIdSql, sqlParams, TableName);

            return dt;

        }

        //************************************************************************
        /// <summary> 有効データが存在する場合有効データ、存在しない場合最新データの取得</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <return>検索結果</return>
        //************************************************************************
        public DataTable GetValidORMaxUpdateDateEntityById(object calendar_id)
        {
            DataTable dt = GetValidEntityById(calendar_id);
            if (dt.Rows.Count == 1)
            {
                return dt;
            }

            return GetMaxUpdateDateEntityById(calendar_id);

        }

        //Park.iggy Add
        public override DataTable GetEntityByObjectALL(Boolean public_flg)
        {
            int public_flag = Convert.ToInt32(public_flg);
            bool isBelongToUSRGRP = LoginSetting.BelongToUsrgrpFlag;
            String selectObjectAll = "SELECT * FROM ja_calendar_control_table WHERE valid_flag = 1 AND public_flag = ? " +
                                   " UNION ALL  " +
                                   "SELECT * FROM ja_calendar_control_table A " +
                                   " WHERE A.update_date= ( SELECT MAX(update_date) FROM ja_calendar_control_table B " +
                                   "                         WHERE B.calendar_id NOT IN (SELECT calendar_id FROM ja_calendar_control_table  " +
                                   "                                                      WHERE valid_flag = 1 )  " +
                                   "                           AND B.public_flag = ? AND A.calendar_id = B.calendar_id " +
                                   "                           GROUP BY calendar_id  " +
                                   "                       )  ";
            String selectObjectAllStr = "";
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.String, "@public_flag", public_flag));
            sqlParams.Add(new ComSqlParam(DbType.String, "@public_flag", public_flag));

            if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER) && !public_flg)
            {
                selectObjectAllStr = "SELECT CALENDAR.* FROM ( ";
                selectObjectAllStr = selectObjectAllStr + selectObjectAll;
                selectObjectAllStr = selectObjectAllStr + " ) AS CALENDAR, users AS U, users_groups AS UG1, users_groups AS UG2  " +
                                         "WHERE CALENDAR.user_name = U.username  " +
                                         "AND U.userid = UG1.userid  " +
                                         "AND UG2.userid=? " +
                                         "AND UG1.usrgrpid = UG2.usrgrpid  " +
                                         "ORDER BY CALENDAR.calendar_id ";

                sqlParams.Add(new ComSqlParam(DbType.String, "@userid", LoginSetting.UserID));
            }
            else
            {
                selectObjectAllStr = "SELECT CALENDAR.* FROM ( ";
                selectObjectAllStr = selectObjectAllStr + selectObjectAll;
                selectObjectAllStr = selectObjectAllStr + " ) AS CALENDAR ORDER BY CALENDAR.calendar_id ";
            }

            DataTable dt = _db.ExecuteQuery(selectObjectAllStr, sqlParams, TableName);

            return dt;
        }

        #endregion
    }
}
