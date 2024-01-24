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
using System.Collections;
using jp.co.ftf.jobcontroller;
using jp.co.ftf.jobcontroller.Common;

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
    public class DBUtil
    {
        //エクスポートカレンダーオブジェクト関連テーブル一覧最初が管理テーブル
        private static String[] EXPORT_CALENDAR_TABLES = { "ja_calendar_control_table",
                                                    "ja_calendar_detail_table"
                                                  };
        //エクスポートフィルターオブジェクト関連テーブル一覧最初が管理テーブル
        private static String[] EXPORT_FILTER_TABLES = { "ja_filter_control_table" };
        //エクスポートスケジュールオブジェクト関連テーブル一覧最初が管理テーブル
        private static String[] EXPORT_SCHEDULE_TABLES = { "ja_schedule_control_table",
                                                    "ja_schedule_detail_table",
                                                    "ja_schedule_jobnet_table"
                                                  };
        //エクスポートジョブネットオブジェクト関連テーブル一覧最初が管理テーブル
        //private static String[] EXPORT_JOBNET_TABLES = { "ja_jobnet_control_table",
        //                                          "ja_job_control_table",
        //                                          "ja_flow_control_table",
        //                                          "ja_icon_calc_table",
        //                                          "ja_icon_end_table",
        //                                          "ja_icon_extjob_table",
        //                                          "ja_icon_if_table",
        //                                          "ja_icon_info_table",
        //                                          "ja_icon_jobnet_table",
        //                                          "ja_icon_job_table",
        //                                          "ja_job_command_table",
        //                                          "ja_value_job_table",
        //                                          "ja_value_jobcon_table",
        //                                          "ja_icon_task_table",
        //                                          "ja_icon_value_table",
        //                                          "ja_icon_fcopy_table",
        //                                          "ja_icon_fwait_table",
        //                                          "ja_icon_reboot_table",
        //                                          "ja_icon_release_table"
        //                                        };

        //added by YAMA 2014/05/19 [ja_icon_agentless_table]
        //added by YAMA 2014/02/05 [ja_icon_zabbix_link_table]
        //エクスポートジョブネットオブジェクト関連テーブル一覧最初が管理テーブル
        private static String[] EXPORT_JOBNET_TABLES = { "ja_jobnet_control_table",
                                                  "ja_job_control_table",
                                                  "ja_flow_control_table",
                                                  "ja_icon_calc_table",
                                                  "ja_icon_end_table",
                                                  "ja_icon_extjob_table",
                                                  "ja_icon_if_table",
                                                  "ja_icon_info_table",
                                                  "ja_icon_jobnet_table",
                                                  "ja_icon_job_table",
                                                  "ja_job_command_table",
                                                  "ja_value_job_table",
                                                  "ja_value_jobcon_table",
                                                  "ja_icon_task_table",
                                                  "ja_icon_value_table",
                                                  "ja_icon_fcopy_table",
                                                  "ja_icon_fwait_table",
                                                  "ja_icon_reboot_table",
                                                  "ja_icon_release_table",
                                                  "ja_icon_zabbix_link_table",
                                                  "ja_icon_agentless_table"
                                                };
        //インポート時、重複チェック用情報
        private static Hashtable KEY_FOR_DOUBLE_CHECK = new Hashtable();
        //インポート時、オブジェクト間関連チェック用情報
        private static Hashtable KEY_FOR_RELATION_CHECK = new Hashtable();

        private static bool SET_DOUBLE_KEY = false;
        //整合性チェック情報セットフラグ
        private static bool SET_RELATE_KEY = true;

        //オブジェクト関連チェック added by YAMA 2014/10/17
        private const string RELATEDOBJECT_CALENDAR = "CALENDAR";
        private const string RELATEDOBJECT_FILTER = "FILTER";
        private const string RELATEDOBJECT_SCHEDULE = "SCHEDULE";
        private const string RELATEDOBJECT_JOBNET = "JOBNET";


        #region 採番処理


        /// <summary>通番の取得</summary>
        /// <param name="strIndexType">通番のタイプ</param>
        /// <return>通番ID</return>
        public static string GetNextId(string strIndexType)
        {
            string strNextId = "";

            string strSql = "SELECT nextid FROM ja_index_table WHERE count_id = " + strIndexType+" for update";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();

            DataTable dt = db.ExecuteQuery(strSql);

            if (dt.Rows.Count == 1)
            {
                strNextId = dt.Rows[0]["NEXTID"].ToString();

                strSql = "UPDATE ja_index_table SET nextid = nextid + 1 WHERE count_id = " + strIndexType;

                db.ExecuteNonQuery(strSql);

                db.TransactionCommit();

                db.CloseSqlConnect();
            }
            else
            {
                db.TransactionRollback();
                db.CloseSqlConnect();
                throw new DBException(Consts.SYSERR_004, null);
            }

            return strNextId;
        }

        /// <summary>有効なジョブネットを取得</summary>
        /// <param name="jobnetId">ジョブネットＩＤ</param>
        /// <return>データ</return>
        public static DataTable GetValidJobnetVer(string jobnetId)
        {
            String sql = "select * from ja_jobnet_control_table where jobnet_id='" + jobnetId + "' and valid_flag=1";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable dt = db.ExecuteQuery(sql);
            db.CloseSqlConnect();
            return dt;
        }

        /// <summary>ジョブネット画面から実行時、実行テーブルに追加する</summary>
        /// <param name="jobnetRow">ジョブネットデータ</param>
        /// <param name="runType">実行種別</param>
        /// <return>実行ジョブネット内部管理ＩＤ</return>
        public static String InsertRunJobnet(DataRow jobnetRow, Consts.RunTypeEnum runType)
        {
            String innerJobnetId = DBUtil.GetNextId("2");

            //added by YAMA 2014/04/22 add-> multiple_start_up
            String sql = "insert into ja_run_jobnet_table "
                            + "(inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date, run_type, "
                            + "main_flag, status, start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo, execution_user_name) "
                            + "VALUES (?,?,0,?,?,0,0,0,0,?,?,?,?,?,?,?)";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);

            List<ComSqlParam> insertRunJobnetSqlParams = new List<ComSqlParam>();
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", innerJobnetId));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_main_id", innerJobnetId));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@update_date", jobnetRow["update_date"]));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@run_type", (int)runType));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@public_flag", jobnetRow["public_flag"]));

            //added by YAMA 2014/04/22
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@multiple_start_up", jobnetRow["multiple_start_up"]));

            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_id", jobnetRow["jobnet_id"]));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@user_name", jobnetRow["user_name"]));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_name", jobnetRow["jobnet_name"]));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@memo", jobnetRow["memo"]));
            insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@execution_user_name", LoginSetting.UserName));
            db.CreateSqlConnect();
            db.BeginTransaction();
            int count = db.ExecuteNonQuery(sql, insertRunJobnetSqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
            if (count == 1) return innerJobnetId;
            return null;
        }

        /// <summary>実行ジョブネットが展開されたか判断</summary>
        /// <param name="innerJobnetId">実行ジョブネット内部管理ＩＤ</param>
        /// <return>展開された場合True、されてない場合False</return>
        public static DataTable GetRunJobnetSummary(String innerJobnetId)
        {
            String sql = "select * from ja_run_jobnet_summary_table where inner_jobnet_id=" + innerJobnetId + " and invo_flag=1";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable dt = db.ExecuteQuery(sql);
            db.CloseSqlConnect();
            return dt;
        }


        /// <summary>パラメータ取得ary>
        /// <param name="parameterName">パラメータ名</param>
        /// <return>パラメータ値</return>
        public static string GetParameterVelue(string parameterName)
        {
            string strParameterVelue = "";

            string strSql = "SELECT value FROM ja_parameter_table WHERE parameter_name = '" + parameterName + "'";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();

            DataTable dt = db.ExecuteQuery(strSql);

            if (dt.Rows.Count == 1)
            {
                strParameterVelue = dt.Rows[0]["value"].ToString();
                //added by YAMA 2014/04/25 マイナス値の場合、初期値を設定する
                int retVal = Convert.ToInt32(strParameterVelue);
                if (retVal < 0)
                {
                    strParameterVelue = GetParamDefaultData(parameterName);
                }

                db.CloseSqlConnect();
            }
            else
            {
                //added by YAMA 2014/04/25 該当データなしの場合、初期値を設定する
                //strParameterVelue = "60";
                strParameterVelue = GetParamDefaultData(parameterName);
                db.CloseSqlConnect();
            }

            return strParameterVelue;
        }

        //added by YAMA 2014/08/18
        public static string GetParameterVelueForStrData(string parameterName)
        {
            string strParameterVelue = "";

            string strSql = "SELECT value FROM ja_parameter_table WHERE parameter_name = '" + parameterName + "'";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();

            DataTable dt = db.ExecuteQuery(strSql);

            if (dt.Rows.Count == 1)
            {
                strParameterVelue = dt.Rows[0]["value"].ToString();

                db.CloseSqlConnect();
            }
            else
            {
                //added by YAMA 2014/04/25 該当データなしの場合、初期値を設定する
                //strParameterVelue = "60";
                strParameterVelue = GetParamDefaultData(parameterName);
                db.CloseSqlConnect();
            }

            return strParameterVelue;
        }


        //added by YAMA 2014/04/25
        private static string GetParamDefaultData(string parameterName)
        {
            string defaultValue = "";

            switch (parameterName)
            {
                case "JOBNET_VIEW_SPAN":
                    defaultValue = "60";
                    break;
                case "JOBNET_LOAD_SPAN":
                    defaultValue = "60";
                    break;
                case "JOBNET_KEEP_SPAN":
                    defaultValue = "60";
                    break;
                case "JOBLOG_KEEP_SPAN":
                    defaultValue = "129600";
                    break;
                case "JOBNET_DUMMY_START_X":
                    defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_START_Y":
                    defaultValue = "39";
                    break;
                case "JOBNET_DUMMY_JOB_X":
                    defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_JOB_Y":
                    defaultValue = "93";
                    break;
                case "JOBNET_DUMMY_END_X":
                    defaultValue = "117";
                    break;
                case "JOBNET_DUMMY_END_Y":
                    defaultValue = "146";
                    break;
                //added by YAMA 2014/08/18
                case "MANAGER_TIME_SYNC":
                    defaultValue = "0";
                    break;
                case "ZBXSND_ZABBIX_IP":
                    defaultValue = "127.0.0.1";
                    break;
                case "ZBXSND_ZABBIX_PORT":
                    defaultValue = "10051";
                    break;
                case "ZBXSND_ZABBIX_HOST":
                    defaultValue = "Zabbix server";
                    break;
                case "ZBXSND_ITEM_KEY":
                    defaultValue = "jasender";
                    break;
                case "ZBXSND_SENDER":
                    defaultValue = "zabbix_sender";
                    break;
                case "ZBXSND_RETRY":
                    defaultValue = "0";
                    break;
                case "ZBXSND_RETRY_COUNT":
                    defaultValue = "0";
                    break;
                case "ZBXSND_RETRY_INTERVAL":
                    defaultValue = "5";
                    break;
            }
            return defaultValue;
        }



        /// <summary>ユーザーが属するグループ取得ary>
        /// <param name="alias">別名</param>
        /// <return>グループＩＤリスト</return>
        public static List<Decimal> GetGroupIDListByAlias(string alias)
        {
            List<Decimal> groupList = new List<Decimal>();

            string strSql = "select UG.usrgrpid from users_groups AS UG,users AS U where U.username='"+alias+"' and U.userid=UG.userid";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();

            DataTable dt = db.ExecuteQuery(strSql);

            foreach(DataRow row in dt.Rows)
            {
                groupList.Add(Convert.ToDecimal(row["usrgrpid"]));

            }
            db.CloseSqlConnect();

            return groupList;
        }

        /// <summary>ユーザーが属するグループ取得ary>
        /// <param name="alias">別名</param>
        /// <return>グループＩＤリスト</return>
        public static List<Decimal> GetGroupIDListByID(string objectId, Consts.ObjectEnum objectType)
        {
            List<Decimal> groupList = new List<Decimal>();

            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                tableName = "ja_filter_control_table";
                idColumnName = "filter_id";
            }
            if (objectType == Consts.ObjectEnum.SCHEDULE)
            {
                tableName = "ja_schedule_control_table";
                idColumnName = "schedule_id";
            }
            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                tableName = "ja_jobnet_control_table";
                idColumnName = "jobnet_id";
            }
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String objectSql = "select * from " + tableName + " where " + idColumnName + "='" + objectId + "'";
            DataTable objectDt = db.ExecuteQuery(objectSql);
            if (objectDt.Rows.Count > 0)
            {

                string strSql = "select UG.usrgrpid from users_groups AS UG,users AS U where U.username='" + (String)objectDt.Rows[0]["user_name"] + "' and U.userid=UG.userid";

                DataTable dt = db.ExecuteQuery(strSql);

                foreach (DataRow row in dt.Rows)
                {
                    groupList.Add(Convert.ToDecimal(row["usrgrpid"]));

                }
            }
            db.CloseSqlConnect();

            return groupList;
        }

        /// <summary>ユーザーが属するグループ取得ary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <return>オブジェクトDataTable</return>
        public static DataTable GetObjectsById(string objectId, Consts.ObjectEnum objectType)
        {

            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                tableName = "ja_filter_control_table";
                idColumnName = "filter_id";
            }
            if (objectType == Consts.ObjectEnum.SCHEDULE)
            {
                tableName = "ja_schedule_control_table";
                idColumnName = "schedule_id";
            }
            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                tableName = "ja_jobnet_control_table";
                idColumnName = "jobnet_id";
            }
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String objectSql = "select * from " + tableName + " where " + idColumnName + "='" + objectId + "'";
            DataTable objectDt = db.ExecuteQuery(objectSql);

            db.CloseSqlConnect();

            return objectDt;
        }

        /// <summary>オブジェクトを有効にする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="updDate">更新日</param>
        /// <param name="objectType">オブジェクト種別</param>
        public static void SetObjectValid_org(String objectId, String updDate, Consts.ObjectEnum objectType)
        {
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            string strSql1 = "update "+ tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and valid_flag=1";
            string strSql2 = "update " + tableName + " set valid_flag=1 where " + idColumnName + "='" + objectId + "' and update_date=" + updDate;
            db.AddBatch(strSql1);
            db.AddBatch(strSql2);
            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        /// <summary>実行中ジョブネットを強制停止する。<summary>
        /// <param name="innerJobnetId">実行ジョブネット内部管理ＩＤ</param>
        public static void StopRunningJobnet(object innerJobnetId)
        {
            //エラー実行中ジョブネットを停止SQL文
            String _stop_err_jobnet = "update ja_run_jobnet_summary_table set jobnet_abort_flag=1 where inner_jobnet_id=? and status=2";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        /// <summary>未実行ジョブネットを停止する。<summary>
        /// <param name="innerJobnetId">実行ジョブネット内部管理ＩＤ</param>
        public static void StopUnexecutedJobnet(object innerJobnetId)
        {
            //エラー実行中ジョブネットを停止SQL文
            int jobnetLoadSpan = Convert.ToInt32(DBUtil.GetParameterVelue("JOBNET_LOAD_SPAN"));
            DateTime now = DateTime.Now;
            decimal startTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMISS(now);
            //added by YAMA 2014/07/01
            //decimal endTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMISS(now.AddMinutes(2 * jobnetLoadSpan));
            decimal endTime = startTime;
            String _stop_enexecuted_jobnet = "update ja_run_jobnet_summary_table set status=5, start_time=" + startTime + " ,end_time=" + endTime + " where inner_jobnet_id=? and status=0";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_enexecuted_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }


        //added by THHZ 2022/06/02
        public static void StartDelayScheduleJobnet(object innerJobnetId)
        {
            //set run jobnet status to begin
            String _stop_err_jobnet = "update ja_run_jobnet_table set status = 0 where inner_jobnet_id = ? ";

            List<ComSqlParam> sqlParamsJobnet = new List<ComSqlParam>();

            sqlParamsJobnet.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));

            //set run jobnet summary status to ready
            String _stop_err_jobnet_summary = "update ja_run_jobnet_summary_table set status = 1, job_status = 0, load_status = 0 where inner_jobnet_id = ? ";

            List<ComSqlParam> sqlParamsJobnetSummary = new List<ComSqlParam>();

            sqlParamsJobnetSummary.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParamsJobnet);
            db.ExecuteNonQuery(_stop_err_jobnet_summary, sqlParamsJobnetSummary);
            //db.ExecuteNonQuery(_stop_err_job, sqlParamsJob);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        //added by YAMA 2014/04/25
        public static void SetJaRunJobnetTableStatus(object innerJobnetId)
        {
            
            DateTime dt = DateTime.Now;
            int millisec = dt.Millisecond;
            String datetime = dt.ToString() + ":" + millisec.ToString();
            
            String _stop_err_jobnet = "update ja_run_jobnet_table set status = 0 where inner_jobnet_id = ? ";
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        //added by YAMA 2014/04/25
        public static void SetJaRunJobnetSummaryTableStatus(object innerJobnetId)
        {
            DateTime dt = DateTime.Now;
            int millisec = dt.Millisecond;
            String datetime = dt.ToString() + ":" + millisec.ToString();
            String _stop_err_jobnet = "update ja_run_jobnet_summary_table set status = 1, job_status = 0, load_status = 0 where inner_jobnet_id = ? ";
            // String _stop_err_jobnet = "update ja_run_jobnet_summary_table set status = 0, job_status = 0, load_status = 0 where inner_jobnet_id = ? ";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        //added by YAMA 2014/04/25
        public static void SetJaRunJobTableStatus(object innerJobnetId)
        {
            DateTime dt = DateTime.Now;
            int millisec = dt.Millisecond;
            String datetime = dt.ToString() + ":" + millisec.ToString();
            String _stop_err_jobnet = "update ja_run_job_table set status = 0 where inner_jobnet_id = ? and job_type = 0 ";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary> 未実行 ジョブネットを起動保留にする <summary>
        /// <param name="innerJobnetId"> 実行ジョブネット内部管理ＩＤ </param>
        public static void SetReserveJobnet(object innerJobnetId)
        {
            // 稼働状態が未実行のジョブネットを起動保留にする
            String _stop_err_jobnet = "update ja_run_jobnet_summary_table set start_pending_flag=1 where inner_jobnet_id=? and status=0";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary> 未実行 ジョブネットを起動保留にする <summary>
        /// <param name="innerJobnetId"> 実行ジョブネット内部管理ＩＤ </param>
        public static int Set_Reserve_Jobnet(object innerJobnetId)
        {
            // 稼働状態が未実行のジョブネットを起動保留にする
            int intNum = 0;
            String _stop_err_jobnet = "update ja_run_jobnet_summary_table set start_pending_flag=1 where inner_jobnet_id=? and status=0";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            intNum = db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();

            return intNum;
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary> 未実行 ジョブネットを保留解除にする <summary>
        /// <param name="innerJobnetId"> 実行ジョブネット内部管理ＩＤ </param>
        public static void SetReleaseJobnet(object innerJobnetId)
        {
            // 稼働状態が未実行のジョブネットを起動保留にする
            String _stop_err_jobnet = "update ja_run_jobnet_summary_table set start_pending_flag=2 where inner_jobnet_id=? and start_pending_flag=1";

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();

            sqlParams.Add(new ComSqlParam(DbType.UInt64, "@inner_jobnet_id", innerJobnetId));
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            db.ExecuteNonQuery(_stop_err_jobnet, sqlParams);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary> 未実行ジョブネットの実行予定時刻を更新する <summary>
        /// <param name="innerJobnetId"> 実行ジョブネット内部管理ＩＤ </param>
        /// <param name="scheduledtime"> 実行ジョブネット実行予定時刻 </param>
        public static int SetScheduledTime(String innerJobnetId, String scheduledTime)
        {
            int intNum = 0;

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();

            string strSql1 = "update ja_run_jobnet_summary_table set scheduled_time = '" + scheduledTime + "' where inner_jobnet_id = '" + innerJobnetId + "' and status = 0";
            string strSql2 = "update ja_run_jobnet_table set scheduled_time = '" + scheduledTime + "' where inner_jobnet_id = '" + innerJobnetId + "' and status = 0";

            db.AddBatch(strSql1);
            db.AddBatch(strSql2);
            intNum = db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();

            return intNum;
        }



        /// <summary>オブジェクトを無効にする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        public static void SetObjectsInValid_org(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            foreach (DataRow row in rows)
            {
                if ((Int32)row["valid_flag"] == 1)
                {
                    string strSql = "update " + tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and update_date=" + Convert.ToString(row["update_date"]);
                    db.AddBatch(strSql);
                }
            }

            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();

        }

        /// <summary>オブジェクトを削除するため、関連データ有無チェック<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        public static bool CheckForRelation4Del(String objectId, Consts.ObjectEnum objectType, DataRow[] rows, ref DataTable chkResult)
        {
            if (objectType == Consts.ObjectEnum.CALENDAR)
            {
                //added by YAMA 2014/10/17
                //if (IsExistRelate4Calendar(objectId, rows)) return false;
                if (CntObjectData(objectId, objectType, rows))
                {
                    if (!CheckRelatedObjectForCalendar(objectId, ref chkResult, "Delete")) return false;
                }
            }
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                //added by YAMA 2014/10/17
                if (CntObjectData(objectId, objectType, rows))
                {
                    if (!CheckRelatedObjectForFilter(objectId, ref chkResult, "Delete")) return false;
                }
            }

            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                //added by YAMA 2014/10/17
                // if (IsExistRelate4Jobnet(objectId, rows)) return false;
                if (CntObjectData(objectId, objectType, rows))
                {
                    if (!CheckRelatedObjectForJobNet(objectId, ref chkResult, "Delete")) return false;
                }
            }
            return true;
        }

        private static bool CntObjectData(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            String checkSql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();

            if (objectType == Consts.ObjectEnum.CALENDAR)
            {
                checkSql = "select calendar_id from ja_calendar_control_table where calendar_id = '" + objectId + "'";
            }
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                checkSql = "select filter_id from ja_filter_control_table where filter_id = '" + objectId + "'";
            }
            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                checkSql = "select jobnet_id from ja_jobnet_control_table where jobnet_id = '" + objectId + "'";
            }
            DataTable dtDB = db.ExecuteQuery(checkSql);
            db.CloseSqlConnect();
            if (dtDB.Rows.Count == 1 || rows == null)
            {
                return true;
            }
            return false;
        }


        /// <summary>オブジェクトを削除する<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        public static void DelObject(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            if (rows == null)
            {
                DelAllVer(objectId, objectType);
            }
            else
            {
                DelSpecialVer(objectId, objectType, rows);
            }
        }

        /// <summary>オブジェクトを削除する<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        private static void DelAllVer(String objectId, Consts.ObjectEnum objectType)
        {
            /*
            if (objectType == Consts.ObjectEnum.CALENDAR)
            {
                if (IsExistRelate4Calendar(objectId, null)) return false;
            }

            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                if (IsExistRelate4Jobnet(objectId, null)) return false;
            }
            */
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }

            string strSql = "delete from " + tableName + " where " + idColumnName + "='" + objectId + "'";
            db.ExecuteNonQuery(strSql);
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        /// <summary>オブジェクトを削除する<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        private static void DelSpecialVer(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            /*
            if (objectType == Consts.ObjectEnum.CALENDAR)
            {
                if (IsExistRelate4Calendar(objectId, rows)) return false;
            }

            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                if (IsExistRelate4Jobnet(objectId, rows)) return false;
            }
            */

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            foreach (DataRow row in rows)
            {
                string strSql = "delete from " + tableName + " where " + idColumnName + "='" + objectId + "' and update_date=" + Convert.ToString(row["update_date"]);
                db.AddBatch(strSql);
            }

            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();
        }

        private static bool IsExistRelate4Calendar(String objectId, DataRow[] rows)
        {
            String sql = "select * from ja_schedule_detail_table where calendar_id='" + objectId + "'";
            DataTable dt;
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            if (rows == null)
            {
                dt = db.ExecuteQuery(sql);
                if (dt.Rows.Count > 0) return true;
            }
            else
            {
                DataTable objectDT = GetObjectsById(objectId, Consts.ObjectEnum.CALENDAR);
                if (objectDT.Rows.Count == rows.Length)
                {
                    dt = db.ExecuteQuery(sql);
                    if (dt.Rows.Count > 0) return true;
                }
            }
            return false;

        }

        private static bool IsExistRelate4Jobnet(String objectId, DataRow[] rows)
        {
            String sqlSchedule = "select * from ja_schedule_jobnet_table where jobnet_id='" + objectId + "'";
            String sqlLinkedJobnet = "select * from ja_icon_jobnet_table where link_jobnet_id='" + objectId + "'";
            String sqlSubmitJobnet = "select * from ja_icon_task_table where submit_jobnet_id='" + objectId + "'";
            DataTable dt;
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            if (rows == null)
            {
                dt = db.ExecuteQuery(sqlSchedule);
                if (dt.Rows.Count > 0) return true;
                dt = db.ExecuteQuery(sqlLinkedJobnet);
                if (dt.Rows.Count > 0) return true;
                dt = db.ExecuteQuery(sqlSubmitJobnet);
                if (dt.Rows.Count > 0) return true;
            }
            else
            {
                DataTable objectDT = GetObjectsById(objectId, Consts.ObjectEnum.JOBNET);
                if (objectDT.Rows.Count == rows.Length)
                {
                    dt = db.ExecuteQuery(sqlSchedule);
                    if (dt.Rows.Count > 0) return true;
                    dt = db.ExecuteQuery(sqlLinkedJobnet);
                    if (dt.Rows.Count > 0) return true;
                    dt = db.ExecuteQuery(sqlSubmitJobnet);
                    if (dt.Rows.Count > 0) return true;
                }
            }
            return false;

        }


        /// <summary>オブジェクトをエクスポーする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        public static DataSet Export(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {

            if (objectId == null)
            {
                return ExportAll();

            }
            if (rows == null)
            {
                return ExportAllVer(objectId, objectType);
            }
            return ExportSpecialVer(objectId, objectType, rows);
        }
        /// <summary>全バージョンオブジェクトをエクスポーする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        public static DataSet ExportAll()
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String idColumnName = "calendar_id";
            String dsName = "All";
            String[] sqlCalendar = new String[EXPORT_CALENDAR_TABLES.Length];
            int i = 0;
            foreach (String calendarTableName in EXPORT_CALENDAR_TABLES)
            {
                idColumnName = "calendar_id";
                sqlCalendar[i] = "select * from " + calendarTableName;
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    if (i == 0)
                    {
                        sqlCalendar[i] = "select distinct A.* from " + calendarTableName + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 where A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                    else
                    {
                        sqlCalendar[i] = "select distinct B.* from " + calendarTableName + " AS B," + EXPORT_CALENDAR_TABLES[0] + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 "
                                                + "where A." + idColumnName + "=B." + idColumnName + " and A.update_date=B.update_date and "
                                                + "A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                }
                db.AddSelectBatch(sqlCalendar[i], calendarTableName);
                i++;
            }

            // added by YAMA 2014/11/07    インポート・エクスポート
            String[] sqlFilter = new String[EXPORT_FILTER_TABLES.Length];
            i = 0;
            foreach (String filterTableName in EXPORT_FILTER_TABLES)
            {
                idColumnName = "filter_id";
                sqlFilter[i] = "select * from " + filterTableName;
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    if (i == 0)
                    {
                        sqlFilter[i] = "select distinct A.* from " + filterTableName + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 where A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                    else
                    {
                        sqlFilter[i] = "select distinct B.* from " + filterTableName + " AS B," + EXPORT_FILTER_TABLES[0] + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 "
                                                + "where A." + idColumnName + "=B." + idColumnName + " and A.update_date=B.update_date and "
                                                + "A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                }
                db.AddSelectBatch(sqlFilter[i], filterTableName);
                i++;
            }

            String[] sqlSchedule = new String[EXPORT_SCHEDULE_TABLES.Length];
            i = 0;
            foreach (String scheduleTableName in EXPORT_SCHEDULE_TABLES)
            {
                idColumnName = "schedule_id";
                sqlSchedule[i] = "select * from " + scheduleTableName;
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    if (i == 0)
                    {
                        sqlSchedule[i] = "select distinct A.* from " + scheduleTableName + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 where A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                    else
                    {
                        sqlSchedule[i] = "select distinct B.* from " + scheduleTableName + " AS B," + EXPORT_SCHEDULE_TABLES[0] + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 "
                                                + "where A." + idColumnName + "=B." + idColumnName + " and A.update_date=B.update_date and "
                                                + "A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                }
                db.AddSelectBatch(sqlSchedule[i], scheduleTableName);
                i++;
            }

            String[] sqlJobnet = new String[EXPORT_JOBNET_TABLES.Length];
            i = 0;
            foreach (String jobnetTableName in EXPORT_JOBNET_TABLES)
            {
                idColumnName = "jobnet_id";
                sqlJobnet[i] = "select * from " + jobnetTableName;
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    if (i == 0)
                    {
                        sqlJobnet[i] = "select distinct A.* from " + jobnetTableName + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 where A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                    else
                    {
                        sqlJobnet[i] = "select distinct B.* from " + jobnetTableName + " AS B," + EXPORT_JOBNET_TABLES[0] + " AS A,users AS U,users_groups AS UG1, users_groups AS UG2 "
                                                + "where A." + idColumnName + "=B." + idColumnName + " and A.update_date=B.update_date and "
                                                + "A.user_name=U.username and UG1.userid=U.userid and UG2.userid=" + LoginSetting.UserID + " and UG1.usrgrpid=UG2.usrgrpid ORDER BY 1,2";
                    }
                }
                db.AddSelectBatch(sqlJobnet[i], jobnetTableName);
                i++;
            }
            DataSet ds = db.ExecuteBatchQuery();
            ds.DataSetName = dsName;
            db.CloseSqlConnect();
            setExportUserInfo(ds);
            return ds;

        }
        /// <summary>全バージョンオブジェクトをエクスポーする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        public static DataSet ExportAllVer(String objectId, Consts.ObjectEnum objectType)
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String idColumnName = "calendar_id";
            String dsName = "Calendar";
            String[] tables = EXPORT_CALENDAR_TABLES;
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                idColumnName = "filter_id";
                dsName = "Filter";
                tables = EXPORT_FILTER_TABLES;
            }
            if (objectType == Consts.ObjectEnum.SCHEDULE)
            {
                idColumnName = "schedule_id";
                dsName = "Schedule";
                tables = EXPORT_SCHEDULE_TABLES;
            }
            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                idColumnName = "jobnet_id";
                dsName = "Jobnet";
                tables = EXPORT_JOBNET_TABLES;
            }
            //setExportUserInfo(ds);
            String[] sql = new String[tables.Length];
            int i = 0;
            foreach (String tableName in tables)
            {
                sql[i] = "select * from " + tables[i] + " where " + idColumnName + "='" + objectId + "' ORDER BY 1,2";
                db.AddSelectBatch(sql[i], tableName);
                i++;
            }
            DataSet ds = db.ExecuteBatchQuery();
            ds.DataSetName = dsName;
            db.CloseSqlConnect();
            setExportUserInfo(ds);
            return ds;
        }
        /// <summary>個別バージョンオブジェクトをエクスポーする<summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objectType">オブジェクト種別</param>
        /// <param name="rows">オブジェクトRows</param>
        public static DataSet ExportSpecialVer(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String idColumnName = "calendar_id";
            String dsName = "Calendar";
            String[] tables = EXPORT_CALENDAR_TABLES;
            if (objectType == Consts.ObjectEnum.FILTER)
            {
                idColumnName = "filter_id";
                dsName = "Filter";
                tables = EXPORT_FILTER_TABLES;
            }
            if (objectType == Consts.ObjectEnum.SCHEDULE)
            {
                idColumnName = "schedule_id";
                dsName = "Schedule";
                tables = EXPORT_SCHEDULE_TABLES;
            }
            if (objectType == Consts.ObjectEnum.JOBNET)
            {
                idColumnName = "jobnet_id";
                dsName = "Jobnet";
                tables = EXPORT_JOBNET_TABLES;
            }

            String[] sql = new String[tables.Length];
            //Park.iggy ADD
            int i = 0;
            int j = 0;
            foreach (String tableName in tables)
            {
                j = 0;
                foreach (DataRow row in rows)
                {
                    String patchSql = "select * from " + tables[i] + " where " + idColumnName + "='" + Convert.ToString(row["object_id"]) + "' and "+
                                      "update_date=" + Convert.ToString(row["update_date"]);
                    if (j == 0)
                    {
                        sql[i] = patchSql;
                    }
                    else
                    {
                        sql[i] = sql[i] + " \n UNION ALL \n ";
                        sql[i] = sql[i] + patchSql;
                    }
                    j++;
                }
                db.AddSelectBatch(sql[i], tableName);
                i++;
            }
            //Park.iggy END
            DataSet ds = db.ExecuteBatchQuery();
            ds.DataSetName = dsName;
            db.CloseSqlConnect();
            setExportUserInfo(ds);
            return ds;

        }

        /// <summary>システム時間を取得</summary>
        /// <returns>パラメータ設定テーブルのマネージャ内部時刻同期（MANAGER_TIME_SYNC）が「1：同期あり」の場合、ジョブサーバのシステム時間、「0：同期なし」の場合、ローカルタイム</returns>
        public static DateTime GetSysTime()
        {
            // added by YAMA 2014/10/20    マネージャ内部時刻同期
            DateTime sysTime;

            if (LoginSetting.ManagerTimeSync == 1)
            {
                // ジョブサーバー時刻を使用
                DBConnect db = new DBConnect(LoginSetting.ConnectStr);
                db.CreateSqlConnect();

                string strSql = "SELECT CURRENT_TIMESTAMP AS systemtime";

                DataTable resultTbl = db.ExecuteQuery(strSql);

                sysTime = Convert.ToDateTime(resultTbl.Rows[0]["systemtime"]);

                db.CloseSqlConnect();
            }
            else
            {
                // ローカルタイムを使用
                sysTime = System.DateTime.Now;
            }
            //            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            //            db.CreateSqlConnect();

            //            string strSql = "SELECT CURRENT_TIMESTAMP AS systemtime";

            //            DataTable resultTbl = db.ExecuteQuery(strSql);

            //            DateTime sysTime = Convert.ToDateTime(resultTbl.Rows[0]["systemtime"]);

            //            db.CloseSqlConnect();

            return sysTime;

        }

        /// <summary>DataSetにエクスポートユーザー情報をセット</summary>
        /// <param name="ds">DataSet</param>
        private static void setExportUserInfo(DataSet ds)
        {
            DataTable dt = new DataTable("UserInfo");
            dt.Columns.Add("user_name");
            dt.Columns.Add("type");
            DataRow dr = dt.NewRow();
            dr["user_name"] = LoginSetting.UserName;
            dr["type"] = LoginSetting.Authority;
            dt.Rows.Add(dr);
            DataTable[] tables = new DataTable[ds.Tables.Count];
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                tables[i] = ds.Tables[i];
            }
            ds.Tables.Clear();
            ds.Tables.Add(dt);
            foreach (DataTable table in tables)
            {
                ds.Tables.Add(table);
            }

        }

        /// <summary>ＤＢにインポート</summary>
        /// <param name="ds">DataSet</param>
        /// <param name="overrideFlag">上書きフラグ</param>
        public static Consts.ImportResultType ImportForm(DataSet ds, bool overrideFlag)
        {
            setKeyForDoubleCheck();
            setKeyForRelationCheck();

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();

            //3.2 release START
            if (LoginSetting.DBType == 0)
            {
                db.ExecuteQuery("SET sql_mode=''");
            }
            //END

            String insertSql = "";
            foreach (DataTable dt in ds.Tables)
            {
                if (!dt.TableName.Equals("UserInfo"))
                {
                    insertSql = createInserSql(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!checkDouble(dr, db, overrideFlag))
                        {
                            db.TransactionRollback();
                            db.CloseSqlConnect();
                            return Consts.ImportResultType.DubleKeyErr;
                        }

                        if (!checkRelation(ds, dr, db))
                        {
                            db.TransactionRollback();
                            db.CloseSqlConnect();
                            return Consts.ImportResultType.RelationErr;
                        }

                        List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (!dc.ColumnName.Equals("valid_flag"))
                            {
                                sqlParams.Add(new ComSqlParam(DbType.String, "@" + dc.ColumnName, dr[dc.ColumnName]));
                            }
                            else
                            {
                                sqlParams.Add(new ComSqlParam(DbType.String, "@" + dc.ColumnName, "0"));
                            }

                        }
                        db.ExecuteNonQuery(insertSql, sqlParams);
                    }
                }
            }
            db.TransactionCommit();
            db.CloseSqlConnect();
            return Consts.ImportResultType.Success;
        }

        /// <summary>インポート重複テーブル情報セット</summary>
        private static void setKeyForDoubleCheck()
        {
            if (!SET_DOUBLE_KEY)
            {
                KEY_FOR_DOUBLE_CHECK.Add("ja_calendar_control_table",
                    new ImportDoubleRelationCheck("ja_calendar_control_table", new String[] { "calendar_id", "update_date" }, null, null));
                KEY_FOR_DOUBLE_CHECK.Add("ja_filter_control_table",
                    new ImportDoubleRelationCheck("ja_filter_control_table", new String[] { "filter_id", "update_date" }, null, null));
                KEY_FOR_DOUBLE_CHECK.Add("ja_schedule_control_table",
                    new ImportDoubleRelationCheck("ja_schedule_control_table", new String[] { "schedule_id", "update_date" }, null, null));
                KEY_FOR_DOUBLE_CHECK.Add("ja_jobnet_control_table",
                    new ImportDoubleRelationCheck("ja_jobnet_control_table", new String[] { "jobnet_id", "update_date" }, null, null));
                SET_DOUBLE_KEY = true;
            }
        }

        /// <summary>インポート関連テーブル情報セット</summary>
        private static void setKeyForRelationCheck()
        {
            if (!SET_RELATE_KEY)
            {
                KEY_FOR_RELATION_CHECK.Add("ja_schedule_detail_table",
                    new ImportDoubleRelationCheck("ja_schedule_detail_table", new String[] { "calendar_id" }, "ja_calendar_control_table", new String[] { "calendar_id" }));
                KEY_FOR_RELATION_CHECK.Add("ja_schedule_jobnet_table",
                    new ImportDoubleRelationCheck("ja_schedule_jobnet_table", new String[] { "jobnet_id" }, "ja_jobnet_control_table", new String[] { "jobnet_id" }));
                KEY_FOR_RELATION_CHECK.Add("ja_icon_jobnet_table",
                    new ImportDoubleRelationCheck("ja_icon_jobnet_table", new String[] { "link_jobnet_id" }, "ja_jobnet_control_table", new String[] { "jobnet_id" }));
                KEY_FOR_RELATION_CHECK.Add("ja_icon_task_table",
                    new ImportDoubleRelationCheck("ja_icon_task_table", new String[] { "submit_jobnet_id" }, "ja_jobnet_control_table", new String[] { "jobnet_id" }));
                KEY_FOR_RELATION_CHECK.Add("ja_icon_extjob_table",
                    new ImportDoubleRelationCheck("ja_icon_extjob_table", new String[] { "command_id" }, "ja_define_extjob_table", new String[] { "command_id" }));
                KEY_FOR_RELATION_CHECK.Add("ja_icon_value_table",
                    new ImportDoubleRelationCheck("ja_icon_value_table", new String[] { "value_name" }, "ja_define_value_jobcon_table", new String[] { "value_name" }));
                SET_RELATE_KEY = true;
            }

        }

        /// <summary>インポート追加ＳＱＬ作成</summary>
        /// <param name="dt">DataTable</param>
        /// <return>InsertSQL文</return>
        private static String createInserSql(DataTable dt)
        {
            String insertSql;
            int collumnCount = 0;

            insertSql = "insert into " + dt.TableName + " (";
            foreach (DataColumn dc in dt.Columns)
            {
                if (collumnCount > 0) insertSql = insertSql + ",";
                insertSql = insertSql + dc.ColumnName;
                collumnCount++;
            }

            insertSql = insertSql + ") values (";
            collumnCount = 0;
            foreach (DataColumn dc in dt.Columns)
            {
                if (collumnCount > 0) insertSql = insertSql + ",";
                insertSql = insertSql + "?";
                collumnCount++;
            }
            insertSql = insertSql + ")";
            return insertSql;
        }


        /// <summary>上書き許可の場合、重複チェックして、存在する場合、削除する</summary>
        /// <param name="dr">データ</param>
        /// <param name="db">ＤＢ</param>
        private static bool checkDouble(DataRow dr, DBConnect db, bool overrideFlag)
        {
            String checkSql = "";
            String deleteSql = "";
            if (KEY_FOR_DOUBLE_CHECK.ContainsKey(dr.Table.TableName))
            {
                ImportDoubleRelationCheck checkInfo = (ImportDoubleRelationCheck)KEY_FOR_DOUBLE_CHECK[dr.Table.TableName];
                checkSql = "select * from " + dr.Table.TableName + " where ";
                deleteSql = "delete from " + dr.Table.TableName + " where ";
                for (int i = 0; i < checkInfo.Keys.Length; i++)
                {
                    if (i > 0)
                    {
                        checkSql = checkSql + " and ";
                        deleteSql = deleteSql + " and ";
                    }
                    checkSql = checkSql + checkInfo.Keys[i] + "=?";
                    deleteSql = deleteSql + checkInfo.Keys[i] + "=?";
                }
                //OdbcCommand command = new OdbcCommand(checkSql, connection, tran);
                List<ComSqlParam> sqlParams = new List<ComSqlParam>();

                for (int i = 0; i < checkInfo.Keys.Length; i++)
                {
                    sqlParams.Add(new ComSqlParam(DbType.String, "@" + checkInfo.Keys[i], dr[checkInfo.Keys[i]]));
                }
                DataTable dt = db.ExecuteQuery(checkSql, sqlParams);
                if ((int)dt.Rows.Count > 0)
                {
                    if (!overrideFlag) return false;
                    db.ExecuteNonQuery(deleteSql, sqlParams);
                }
            }
            return true;
        }

        /// <summary>関連データが存在するか確認</summary>
        /// <param name="ds">DataSet</param>
        /// <param name="dr">データ</param>
        /// <param name="db">ＤＢ</param>
        /// <return>関連チェック結果</return>
        private static bool checkRelation(DataSet ds, DataRow dr, DBConnect db)
        {
            if (KEY_FOR_RELATION_CHECK.ContainsKey(dr.Table.TableName))
            {
                ImportDoubleRelationCheck checkInfo = (ImportDoubleRelationCheck)KEY_FOR_RELATION_CHECK[dr.Table.TableName];
                if (!checkRelationForDB(checkInfo, ds, dr, db))
                {
                    return checkRelationForDataSet(checkInfo, ds, dr);
                }
            }
            return true;
        }

        /// <summary>ＤＢ上関連データが存在するか確認</summary>
        /// <param name="checkInfo">関連情報</param>
        /// <param name="ds">DataSet</param>
        /// <param name="dr">データ</param>
        /// <param name="db">ＤＢ</param>
        /// <return>ＤＢ関連チェック結果</return>
        private static bool checkRelationForDB(ImportDoubleRelationCheck checkInfo, DataSet ds, DataRow dr, DBConnect db)
        {
            String checkSql = "select * from " + checkInfo.RefTableName + " where ";
            for (int i = 0; i < checkInfo.RefKeys.Length; i++)
            {
                if (i > 0)
                {
                    checkSql = checkSql + " and ";
                }
                checkSql = checkSql + checkInfo.RefKeys[i] + "=?";
            }
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            for (int i = 0; i < checkInfo.RefKeys.Length; i++)
            {
                sqlParams.Add(new ComSqlParam(DbType.String, "@" + checkInfo.RefKeys[i], dr[checkInfo.Keys[i]]));
            }

            DataTable dtDB = db.ExecuteQuery(checkSql, sqlParams);

            if (dtDB.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>インポートデータ上関連データが存在するか確認</summary>
        /// <param name="checkInfo">関連情報</param>
        /// <param name="ds">DataSet</param>
        /// <param name="dr">データ</param>
        /// <return>インポートデータ関連チェック結果</return>
        private static bool checkRelationForDataSet(ImportDoubleRelationCheck checkInfo, DataSet ds, DataRow dr)
        {
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName.Equals(checkInfo.RefTableName))
                {
                    String checkSql = "";
                    for (int i = 0; i < checkInfo.RefKeys.Length; i++)
                    {
                        if (i > 0)
                        {
                            checkSql = checkSql + ",";
                        }
                        checkSql = checkSql + checkInfo.RefKeys[i] + "='" + dr[checkInfo.Keys[i]] + "'";
                    }
                    DataRow[] selectedRows = dt.Select(checkSql);
                    if (selectedRows.Length == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>インポート権限チェック</summary>
        /// <param name="dr">データ</param>
        /// <return>権限が有りの場合True、権限がない場合False</return>
        public static bool checkImportAuth(DataRow dr)
        {
            if(Convert.ToString(dr["user_name"]).Equals(LoginSetting.UserName))
                return true;
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            String sql = "select UG1.usrgrpid from users U1,users U2,users_groups AS UG1,users_groups AS UG2 where U1.username=? and U2.username=? and UG1.userid=U1.userid and UG2.userid=U2.userid and UG1.usrgrpid = UG2.usrgrpid";
            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@username", dr["user_name"]));
            sqlParams.Add(new ComSqlParam(DbType.String, "@username", LoginSetting.UserName));
            DataTable dt = db.ExecuteQuery(sql, sqlParams);
            if (dt.Rows.Count > 0) return true;
            return false;
        }


        // added by YAMA 2014/10/20    マネージャ内部時刻同期
        /// <summary>マネージャ内部時刻同期を取得</summary>
        /// <returns>0：同期なし(パラメータ未登録含む)，1：同期あり</returns>
        public static int GetManagerTimeSync()
        {
            int rtn = 0;

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();

            string strSql = "select value from ja_parameter_table where parameter_name='MANAGER_TIME_SYNC'";

            DataTable resultTbl = db.ExecuteQuery(strSql);

            if (!(resultTbl.Rows.Count == 0))
            {
                rtn = Convert.ToInt32(resultTbl.Rows[0]["value"]);
            }
            return rtn;
        }

        //added by YAMA 2014/10/17
        public static bool CheckRelatedInValidCalendar(String calendar_id, String updDate, ref DataTable dt)
        {
            bool fstFilterflg = false;
            bool fstScheduleflg = false;
            bool chkflg = false;
            String sql = "";
            String chkDatesql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;

            // カレンダーに紐付くフィルターをチェック
            if (string.IsNullOrEmpty(updDate))
            {
                chkDatesql = "' ";
            }
            else
            {
                chkDatesql = "' and cal.update_date = " + updDate + " ";
            }

            sql = "select fil.filter_id from ja_calendar_control_table as cal, ja_filter_control_table as fil " +
                  "where cal.calendar_id = '" + calendar_id + chkDatesql +
                  "and cal.calendar_id = fil.base_calendar_id and fil.valid_flag = 1";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstFilterflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_FILTER;
                        validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                        dt.Rows.Add(validDataRow);
                        fstFilterflg = true;
                        chkflg = true;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["filter_id"];
                    validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                    dt.Rows.Add(validDataRow);
                }
            }

            // カレンダーに紐付くスケジュールをチェック
            sql = "select distinct scDtl.schedule_id " +
                  "from ja_calendar_control_table as cal, ja_schedule_control_table as scCtl, ja_schedule_detail_table as scDtl " +
                  "where cal.calendar_id = '" + calendar_id + chkDatesql + " and cal.calendar_id = scDtl.calendar_id " +
                  "and scDtl.schedule_id = scCtl.schedule_id and scCtl.valid_flag = 1";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstScheduleflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_SCHEDULE;
                        validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                        dt.Rows.Add(validDataRow);
                        fstScheduleflg = true;
                        chkflg = true;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["schedule_id"];
                    validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                    dt.Rows.Add(validDataRow);
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }


        //added by YAMA 2014/10/17
        public static bool CheckRelatedInValidJobNet(String jobnetId, ref DataTable dt)
        {
            bool fstScheduleflg = false;
            bool chkflg = false;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;

            // スケジュールをチェック
            sql = "select distinct ctl.schedule_id " +
                  "from ja_schedule_jobnet_table as sch, ja_schedule_control_table as ctl " +
                  "where sch.jobnet_id = '" + jobnetId + "' and sch.schedule_id = ctl.schedule_id and ctl.valid_flag = 1";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstScheduleflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_SCHEDULE;
                        validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                        dt.Rows.Add(validDataRow);
                        fstScheduleflg = true;
                        chkflg = true;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["schedule_id"];
                    validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                    dt.Rows.Add(validDataRow);
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }


        //added by YAMA 2014/10/17
        public static void SetObjectValid(String objectId, String updDate, Consts.ObjectEnum objectType, ref DataTable chkResult)
        {
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";

            bool ret = true;

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    //added by YAMA 2014/10/17
                    ret = CheckRelatedValidFilter(objectId, updDate, ref chkResult);
                    if (ret == false)
                    {
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    //added by YAMA 2014/10/17
                    ret = CheckRelatedValidSchedule(objectId, updDate, ref chkResult);
                    if (ret == false)
                    {
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    //added by YAMA 2014/10/17
                    ret = CheckRelatedValidJobNet(objectId, updDate, ref chkResult);
                    if (ret == true)
                    {
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            string strSql1 = "update " + tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and valid_flag=1";
            string strSql2 = "update " + tableName + " set valid_flag=1 where " + idColumnName + "='" + objectId + "' and update_date=" + updDate;
            db.AddBatch(strSql1);
            db.AddBatch(strSql2);
            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();
        }




        //added by YAMA 2014/10/17
        public static void SetObjectsInValid(String objectId, Consts.ObjectEnum objectType, DataRow[] rows, ref DataTable chkResult)
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";

            String updDate = Convert.ToString(rows[0]["update_date"]);

            bool ret = true;

            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    //added by YAMA 2014/10/17
                    ret = CheckRelatedObjectForCalendar(objectId, ref chkResult, "Invalid");
                    if (ret == false)
                    {
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {

                    //added by YAMA 2014/10/17
                    ret = CheckRelatedObjectForFilter(objectId, ref chkResult, "Invalid");
                    if (ret == false)
                    {
                        // db.TransactionCommit();
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    //added by YAMA 2014/10/17
                    ret = CheckRelatedObjectForJobNet(objectId, ref chkResult, "Invalid");
                    if (ret == false)
                    {
                        db.CloseSqlConnect();
                        return;
                    }
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            foreach (DataRow row in rows)
            {
                if ((Int32)row["valid_flag"] == 1)
                {
                    string strSql = "update " + tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and update_date=" + Convert.ToString(row["update_date"]);
                    db.AddBatch(strSql);
                }
            }
            chkResult.Clear();
            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();

        }


        //added by YAMA 2014/10/17
        /// <summary>選択したカレンダーの関連オブジェクトの有効チェック</summary>
        /// <param name="filter_id">選択したカレンダーID</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <param name="Operation">操作種別「Invalid」、「Delete」</param>
        /// <return>「True：関連する有効オブジェクトがない」「False：それ以外」</return>
        public static bool CheckRelatedObjectForCalendar(String calendar_id, ref DataTable dt, String Operation)
        {
            bool fstFilterflg = false;
            bool fstScheduleflg = false;
            bool chkflg = true;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;
            DateTime chgDate;

            // カレンダーに紐付くフィルターをチェック
            sql = "select distinct fil.filter_id" + (Operation == "Delete" ? " , fil.update_date " : " ") +
                  "from ja_calendar_control_table as cal, ja_filter_control_table as fil " +
                  "where cal.calendar_id = '" + calendar_id + "' " +
                  "and cal.calendar_id = fil.base_calendar_id" + (Operation == "Delete" ? "" : " and fil.valid_flag = 1");

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;
                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstFilterflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_FILTER;
                        validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                        dt.Rows.Add(validDataRow);
                        fstFilterflg = true;
                        chkflg = false;
                    }
                    if (Operation == "Delete")
                    {
                        chgDate = DateTime.ParseExact(chkDt.Rows[i]["update_date"].ToString(), "yyyyMMddHHmmss", null);
                    }
                    else
                    {
                        chgDate = DateTime.MinValue;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["filter_id"] + (Operation == "Delete" ? " (" + chgDate.ToString() + ")" : "");
                    validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                    dt.Rows.Add(validDataRow);
                }
            }
            // カレンダーに紐付くスケジュールをチェック
            sql = "select distinct ctl.schedule_id" + (Operation == "Delete" ? ", ctl.update_date " : " ") +
                  "from ja_calendar_control_table as cal, ja_schedule_detail_table as dtl, ja_schedule_control_table as ctl " +
                  "where cal.calendar_id = '" + calendar_id + "' and cal.calendar_id = dtl.calendar_id " +
                  "and dtl.schedule_id = ctl.schedule_id" + (Operation == "Delete" ? "" : " and ctl.valid_flag = 1");
            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstScheduleflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_SCHEDULE;
                        validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                        dt.Rows.Add(validDataRow);
                        fstScheduleflg = true;
                        chkflg = false;
                    }
                    if (Operation == "Delete")
                    {
                        chgDate = DateTime.ParseExact(chkDt.Rows[i]["update_date"].ToString(), "yyyyMMddHHmmss", null);
                    }
                    else
                    {
                        chgDate = DateTime.MinValue;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["schedule_id"] + (Operation == "Delete" ? " (" + chgDate.ToString() + ")" : "");
                    validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                    dt.Rows.Add(validDataRow);
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }

        //added by YAMA 2014/10/17
        /// <summary>選択したフィルターの関連オブジェクトの有効チェック</summary>
        /// <param name="filter_id">選択したフィルターID</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <param name="Operation">操作種別「Invalid」、「Delete」</param>
        /// <return>「True：関連する有効オブジェクトがない」「False：それ以外」</return>
        public static bool CheckRelatedObjectForFilter(String filter_id, ref DataTable dt, String Operation)
        {
            bool fstScheduleflg = false;
            bool chkflg = true;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;
            DateTime chgDate;

            // フィルターに紐付くスケジュールをチェック
            sql = "select distinct ctl.schedule_id, ctl.update_date " +
                  "from ja_filter_control_table as fil, ja_schedule_detail_table as dtl, ja_schedule_control_table as ctl " +
                  "where fil.filter_id = '" + filter_id + "' and fil.filter_id = dtl.calendar_id " +
                  "and dtl.schedule_id = ctl.schedule_id" + (Operation == "Delete" ? "" : " and ctl.valid_flag = 1");

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstScheduleflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_SCHEDULE;
                        validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                        dt.Rows.Add(validDataRow);
                        fstScheduleflg = true;
                        chkflg = false;
                    }
                    if (Operation == "Delete")
                    {
                        chgDate = DateTime.ParseExact(chkDt.Rows[i]["update_date"].ToString(), "yyyyMMddHHmmss", null);
                    }
                    else
                    {
                        chgDate = DateTime.MinValue;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["schedule_id"] + (Operation == "Delete" ? " (" + chgDate.ToString() + ")" : "");
                    validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                    dt.Rows.Add(validDataRow);
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }



        //added by YAMA 2014/10/17
        /// <summary>選択したジョブネットの関連オブジェクトの有効チェック</summary>
        /// <param name="filter_id">選択したジョブネットID</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <param name="Operation">操作種別「Invalid」、「Delete」</param>
        /// <return>「True：関連する有効オブジェクトがない」「False：それ以外」</return>
        public static bool CheckRelatedObjectForJobNet(String jobnetId, ref DataTable dt, String Operation)
        {
            bool fstScheduleflg = false;
            bool fstJobNetflg = false;
            bool chkflg = true;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;
            DataRow validDataRow;
            DateTime chgDate;

            // 選択したジョブネットがスケジュールで使用されているかをチェック
            sql = "select distinct sch.schedule_id" + (Operation == "Delete" ? ", sch.update_date " : " ") +
                  "from ja_schedule_jobnet_table as sch, ja_schedule_control_table as ctl " +
                   "where sch.jobnet_id = '" + jobnetId + "' " +
                   " and sch.schedule_id = ctl.schedule_id" + (Operation == "Delete" ? "" : " and ctl.valid_flag = 1");

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    if (fstScheduleflg == false)
                    {
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = RELATEDOBJECT_SCHEDULE;
                        validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                        dt.Rows.Add(validDataRow);
                        fstScheduleflg = true;
                        chkflg = false;
                    }
                    if (Operation == "Delete")
                    {
                        chgDate = DateTime.ParseExact(chkDt.Rows[i]["update_date"].ToString(), "yyyyMMddHHmmss", null);
                    }
                    else
                    {
                        chgDate = DateTime.MinValue;
                    }
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = chkDt.Rows[i]["schedule_id"] + (Operation == "Delete" ? " (" + chgDate.ToString() + ")" : "");
                    validDataRow["ObjectType"] = RELATEDOBJECT_SCHEDULE;
                    dt.Rows.Add(validDataRow);
                }
            }

            // 選択したジョブネットがジョブネットアイコン、タスクアイコンとして他ジョブネットで使用されているかをチェック
            sql = "select icon.jobnet_id ,icon.update_date from ja_icon_jobnet_table as icon where icon.link_jobnet_id = '" + jobnetId + "' " +
                  "union " +
                  "select tsk.jobnet_id , tsk.update_date from ja_icon_task_table as tsk where tsk.submit_jobnet_id = '" + jobnetId + "'";
            chkDt = db.ExecuteQuery(sql);
            if (chkDt.Rows.Count != 0)
            {

                // 該当するジョブネットをチェック
                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    sql = "select jobnet_id, update_date, valid_flag, user_name, jobnet_name " +
                          "from ja_jobnet_control_table " +
                          "where jobnet_id = '" + chkDt.Rows[i]["jobnet_id"] + "' and update_date = " + chkDt.Rows[i]["update_date"] + (Operation == "Delete" ? "" : " and valid_flag = 1");
                    DataTable objDt;
                    objDt = db.ExecuteQuery(sql);
                    if (objDt.Rows.Count != 0)
                    {
                        if (fstJobNetflg == false)
                        {
                            validDataRow = dt.NewRow();
                            validDataRow["RelatedObject"] = RELATEDOBJECT_JOBNET;
                            validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                            dt.Rows.Add(validDataRow);
                            fstJobNetflg = true;
                            chkflg = false;
                        }
                        if (Operation == "Delete")
                        {
                            chgDate = DateTime.ParseExact(objDt.Rows[0]["update_date"].ToString(), "yyyyMMddHHmmss", null);
                        }
                        else
                        {
                            chgDate = DateTime.MinValue;
                        }
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = objDt.Rows[0]["jobnet_id"] + (Operation == "Delete" ? " (" + chgDate.ToString() + ")" : "");
                        validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                        dt.Rows.Add(validDataRow);
                    }
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }

        //added by YAMA 2014/10/17
        /// <summary>選択したフィルターの有効化チェック</summary>
        /// <param name="filter_id">選択したフィルターID</param>
        /// <param name="updDate">カレンダーレコードの更新日</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <return>「True：フィルターが有効化可能」「False：それ以外」</return>
        public static bool CheckRelatedValidFilter(String filter_id, String updDate, ref DataTable dt)
        {
            int errCnt = 0;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;

            // フィルターに紐付くカレンダーのチェック
            sql = "select distinct cal.calendar_id from ja_filter_control_table as fil, ja_calendar_control_table as cal " +
                  "where fil.filter_id = '" + filter_id + "' and fil.update_date = " + updDate + " " +
                  "and fil.base_calendar_id = cal.calendar_id ";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    // 無効カレンダーがあればerrCntを加算
                    if (!CheckValidCalendar(chkDt.Rows[i]["calendar_id"].ToString(), db, ref dt, errCnt)) errCnt++;
                }

            }
            db.CloseSqlConnect();
            return (errCnt == 0 ? true : false);
        }



        //added by YAMA 2014/10/17
        /// <summary>選択したスケジュールの有効化チェック</summary>
        /// <summary>関連する[カレンダー、フィルター、ジョブネット]の有効チェック</summary>
        /// <param name="schedule_id">選択したスケジュールID</param>
        /// <param name="updDate">カレンダー、フィルター」レコードの更新日</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <return>「True：スケジュールが有効化可能」「False：それ以外」</return>
        public static bool CheckRelatedValidSchedule(String schedule_id, String updDate, ref DataTable dt)
        {
            int errCnt = 0;
            bool fstJobNetflg = false;
            String sql = "";

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;

            // スケジュールに紐付くカレンダー・フィルターのチェック
            sql = "select scDtl.calendar_id, scDtl.object_flag from ja_schedule_control_table as scCtl, ja_schedule_detail_table as scDtl " +
                  "where scCtl.schedule_id = '" + schedule_id + "' and scCtl.update_date = " + updDate + " " +
                  "and scCtl.schedule_id = scDtl.schedule_id and scCtl.update_date = scDtl.update_date " +
                  "order by object_flag, scDtl.calendar_id ";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    switch ((Int32)chkDt.Rows[i]["object_flag"])
                    {
                        // カレンダー
                        case 0:
                            if (!CheckValidCalendar(chkDt.Rows[i]["calendar_id"].ToString(), db, ref dt, errCnt)) errCnt++;

                            break;
                        // フィルター
                        case 1:
                            if (!CheckValidFilter(chkDt.Rows[i]["calendar_id"].ToString(), db, ref dt)) errCnt++;

                            break;
                    }
                }
            }

            // スケジュールに紐付くジョブネットのチェック
            sql = "select scNet.jobnet_id from ja_schedule_control_table as scCtl, ja_schedule_jobnet_table as scNet " +
                  "where scCtl.schedule_id = '" + schedule_id + "' and scCtl.update_date = " + updDate + " " +
                  "and scCtl.schedule_id = scNet.schedule_id and scCtl.update_date = scNet.update_date ";

            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataTable InvalidDt;
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    sql = "select jobnet_id from ja_jobnet_control_table " +
                          "where jobnet_id = '" + chkDt.Rows[i]["jobnet_id"] + "' and valid_flag = 1 ";

                    InvalidDt = db.ExecuteQuery(sql);

                    if (InvalidDt.Rows.Count == 0)
                    {
                        if (fstJobNetflg == false)
                        {
                            validDataRow = dt.NewRow();
                            validDataRow["RelatedObject"] = RELATEDOBJECT_JOBNET;
                            validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                            dt.Rows.Add(validDataRow);
                            fstJobNetflg = true;
                            errCnt++;
                        }
                        validDataRow = dt.NewRow();
                        validDataRow["RelatedObject"] = chkDt.Rows[i]["jobnet_id"];
                        validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                        dt.Rows.Add(validDataRow);
                    }
                }
            }
            db.CloseSqlConnect();

            return (errCnt == 0 ? true : false);
        }


        //added by YAMA 2014/10/17
        /// <summary>選択したジョブネットの有効化チェック</summary>
        /// <summary>関連する[ジョブネット]の有効チェック</summary>
        /// <param name="schedule_id">選択したスケジュールID</param>
        /// <param name="updDate">ジョブネットレコードの更新日</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <return>「True：ジョブネットが有効化可能」「False：それ以外」</return>
        public static bool CheckRelatedValidJobNet(String jobnetId, String updDate, ref DataTable dt)
        {
            bool fstJobNetflg = false;
            bool chkflg = false;
            String sql = "select netCtl.jobnet_id, jobCtl.job_id, jobCtl.job_name, jobCtl.job_type " +
                         "from ja_jobnet_control_table as netCtl, ja_job_control_table as jobCtl " +
                         "where netCtl.jobnet_id = '" + jobnetId + "' and netCtl.update_date = " + updDate + " " +
                         "and netCtl.jobnet_id = jobCtl.jobnet_id and netCtl.update_date = jobCtl.update_date " +
                         "and jobCtl.job_type in (5, 11) ";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;
            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count != 0)
            {
                DataTable InvalidDt;
                DataRow validDataRow;

                for (int i = 0; i < chkDt.Rows.Count; i++)
                {
                    switch ((Int32)chkDt.Rows[i]["job_type"])
                    {
                        // chkJobnetIconData
                        case 5:
                            // ジョブネットアイコン設定テーブルのリンク先ジョブネットIDの有効をチェック
                            sql = "select icon.link_jobnet_id from ja_icon_jobnet_table as icon, ja_jobnet_control_table as netCtl " +
                                  "where icon.jobnet_id = '" + chkDt.Rows[i]["jobnet_id"] + "' and icon.job_id = '" + chkDt.Rows[i]["job_id"] + "' " +
                                  "and icon.link_jobnet_id = netCtl.jobnet_id and netCtl.valid_flag = 1 and icon.update_date = " + updDate;

                            InvalidDt = db.ExecuteQuery(sql);

                            if (InvalidDt.Rows.Count == 0)
                            {   // 無効となっているリンク先ジョブネットIDを抽出
                                sql = "select distinct icon.link_jobnet_id from ja_icon_jobnet_table as icon, ja_jobnet_control_table as netCtl " +
                                      "where icon.jobnet_id = '" + chkDt.Rows[i]["jobnet_id"] + "' and icon.job_id = '" + chkDt.Rows[i]["job_id"] + "' " +
                                      "and icon.link_jobnet_id = netCtl.jobnet_id and netCtl.valid_flag = 0 and icon.update_date = " + updDate;

                                InvalidDt = db.ExecuteQuery(sql);
                                if (InvalidDt.Rows.Count != 0)
                                {
                                    if (fstJobNetflg == false)
                                    {
                                        validDataRow = dt.NewRow();
                                        validDataRow["RelatedObject"] = RELATEDOBJECT_JOBNET;
                                        validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                                        dt.Rows.Add(validDataRow);
                                        fstJobNetflg = true;
                                        chkflg = true;
                                    }
                                    validDataRow = dt.NewRow();
                                    validDataRow["RelatedObject"] = InvalidDt.Rows[0]["link_jobnet_id"];
                                    validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                                    dt.Rows.Add(validDataRow);
                                }
                            }

                            break;
                        // chkTaskIconData
                        case 11:
                            sql = "select  jobnet_id from ja_jobnet_control_table where jobnet_id = " +
                                  "(select submit_jobnet_id from ja_icon_task_table " +
                                  "where jobnet_id = '" + jobnetId + "' and job_id = '" + chkDt.Rows[i]["job_id"] + "' and update_date = " + updDate + ") " +
                                  "and valid_flag = 1 ";


                            InvalidDt = db.ExecuteQuery(sql);
                            if (InvalidDt.Rows.Count == 0)
                            {
                                if (fstJobNetflg == false)
                                {
                                    validDataRow = dt.NewRow();
                                    validDataRow["RelatedObject"] = RELATEDOBJECT_JOBNET;
                                    validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                                    dt.Rows.Add(validDataRow);
                                    fstJobNetflg = true;
                                    chkflg = true;
                                }
                                sql = "select distinct submit_jobnet_id from ja_icon_task_table " +
                                      "where jobnet_id = '" + jobnetId + "' and job_id = '" + chkDt.Rows[i]["job_id"] + "' and update_date = " + updDate;
                                InvalidDt = db.ExecuteQuery(sql);

                                validDataRow = dt.NewRow();
                                validDataRow["RelatedObject"] = InvalidDt.Rows[0]["submit_jobnet_id"];
                                validDataRow["ObjectType"] = RELATEDOBJECT_JOBNET;
                                dt.Rows.Add(validDataRow);
                            }
                            break;
                    }
                }
            }
            db.CloseSqlConnect();
            return chkflg;
        }

        //added by YAMA 2014/10/17
        public static void SetObjectValidForceRun(String objectId, String updDate, Consts.ObjectEnum objectType)
        {
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";

            bool ret = true;

            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            string strSql1 = "update " + tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and valid_flag=1";
            string strSql2 = "update " + tableName + " set valid_flag=1 where " + idColumnName + "='" + objectId + "' and update_date=" + updDate;
            db.AddBatch(strSql1);
            db.AddBatch(strSql2);
            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();
        }


        //added by YAMA 2014/10/17
        public static void SetObjectsInValidForceRun(String objectId, Consts.ObjectEnum objectType, DataRow[] rows)
        {
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            String tableName = "ja_calendar_control_table";
            String idColumnName = "calendar_id";
            try
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    tableName = "ja_calendar_control_table";
                    idColumnName = "calendar_id";
                    CalendarControlDAO calendarControlDAO = new CalendarControlDAO(db);
                    calendarControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    tableName = "ja_filter_control_table";
                    idColumnName = "filter_id";
                    FilterControlDAO filterControlDAO = new FilterControlDAO(db);
                    filterControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    tableName = "ja_schedule_control_table";
                    idColumnName = "schedule_id";
                    ScheduleControlDAO scheduleControlDAO = new ScheduleControlDAO(db);
                    scheduleControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    tableName = "ja_jobnet_control_table";
                    idColumnName = "jobnet_id";
                    JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(db);
                    jobnetControlDAO.GetLock(objectId, LoginSetting.DBType);
                }
            }
            catch (DBException e)
            {
                e.MessageID = Consts.ERROR_DB_LOCK;
                throw e;
            }
            foreach (DataRow row in rows)
            {
                if ((Int32)row["valid_flag"] == 1)
                {
                    string strSql = "update " + tableName + " set valid_flag=0 where " + idColumnName + "='" + objectId + "' and update_date=" + Convert.ToString(row["update_date"]);
                    db.AddBatch(strSql);
                }
            }

            db.ExecuteBatchUpdate();
            db.TransactionCommit();
            db.CloseSqlConnect();

        }
        //スケジュールが有効かを確認
        public static bool GetScheduleValidFlag(String schedule_id)
        {
            string sql = "";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            DataTable chkDt;
            sql = "select * from ja_schedule_control_table where schedule_id='" + schedule_id + "' and valid_flag=1";
            chkDt = db.ExecuteQuery(sql);
            if (chkDt.Rows.Count == 0)
            {
                db.CloseSqlConnect();
                return true;
            }
            db.CloseSqlConnect();
            return false;

        }

        //スケジュールが有効かを確認
        public static bool SetScheduleJobnetDelete(Object innerJobnetId)
        {
            int cnt = 0;
            Boolean _return = false;
            string jobnet_main_id = Convert.ToString(innerJobnetId);
            string sql = "select count(*) from ja_run_jobnet_table where inner_jobnet_main_id=" + jobnet_main_id + " and main_flag=0 and status=0 ";
            DBConnect db = new DBConnect(LoginSetting.ConnectStr);
            db.CreateSqlConnect();
            db.BeginTransaction();
            DataTable chkDt;
            chkDt = db.ExecuteQuery(sql);

            if (chkDt.Rows.Count == 0)
            {
                db.TransactionCommit();
                return _return;
            }
            string _delete_jobnet = "";
            if (LoginSetting.DBType == Consts.DBTYPE.MYSQL){
                _delete_jobnet = "delete from ja_run_jobnet_table where inner_jobnet_main_id=" + jobnet_main_id + " and scheduled_time > cast(date_format((current_timestamp + interval + 5 minute),'%Y%m%d%H%i') as decimal(12))";
            }else{
                _delete_jobnet = "delete from ja_run_jobnet_table where inner_jobnet_main_id=" + jobnet_main_id + " and scheduled_time > to_number(to_char((current_timestamp+ '5 minutes'),'YYYYMMDDHH24MI'),'999999999999')";
            }

            db.AddBatch(_delete_jobnet);
            cnt = db.ExecuteBatchUpdate();
            if (cnt > 1)
            {
                _return = true;
            }
            db.TransactionCommit();
            db.CloseSqlConnect();
            return _return;
        }

        //added by YAMA 2014/10/17
        /// <summary>カレンダーの有効チェック</summary>
        /// <param name="calendar_id">カレンダーID</param>
        /// <param name="db">dBコネクション情報</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <return>「True：カレンダーが有効」「False：それ以外」</return>
        private static bool CheckValidCalendar(String calendar_id, DBConnect db, ref DataTable dt, int errCnt)
        {
            DataTable validDt;
            DataRow validDataRow;
            String sql = "";
            bool chkflg = true;

            // 該当するカレンダーが有効化されているかチェック
            sql = "select calendar_id from ja_calendar_control_table " +
                  "where calendar_id = '" + calendar_id + "' and valid_flag = 1";

            validDt = db.ExecuteQuery(sql);

            // 有効化レコードが無い場合、そのカレンダーIDをエラー表示用に格納
            if (validDt.Rows.Count == 0)
            {
                if (errCnt == 0)
                {
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = RELATEDOBJECT_CALENDAR;
                    validDataRow["ObjectType"] = RELATEDOBJECT_CALENDAR;
                    dt.Rows.Add(validDataRow);
                    chkflg = false;
                }
                validDataRow = dt.NewRow();
                validDataRow["RelatedObject"] = calendar_id;
                validDataRow["ObjectType"] = RELATEDOBJECT_CALENDAR;
                dt.Rows.Add(validDataRow);
            }

            return chkflg;
        }

        //added by YAMA 2014/10/17
        /// <summary>フィルターの有効チェック</summary>
        /// <param name="filter_id">カレンダーID</param>
        /// <param name="db">dBコネクション情報</param>
        /// <param name="dt">関連データ格納後返却</param>
        /// <return>「True：フィルターが有効」「False：それ以外」</return>
        private static bool CheckValidFilter(String filter_id, DBConnect db, ref DataTable dt)
        {
            bool fstfilterflg = false;
            DataTable validDt;
            DataRow validDataRow;
            String sql = "";
            bool chkflg = true;

            // 該当するフィルターが有効化されているかチェック
            sql = "select filter_id from ja_filter_control_table " +
                  "where filter_id = '" + filter_id + "' and valid_flag = 1";

            validDt = db.ExecuteQuery(sql);

            // 有効化レコードが無い場合、そのフィルターIDをエラー表示用に格納
            if (validDt.Rows.Count == 0)
            {
                if (fstfilterflg == false)
                {
                    validDataRow = dt.NewRow();
                    validDataRow["RelatedObject"] = RELATEDOBJECT_FILTER;
                    validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                    dt.Rows.Add(validDataRow);
                    fstfilterflg = true;
                    chkflg = false;
                }
                validDataRow = dt.NewRow();
                validDataRow["RelatedObject"] = filter_id;
                validDataRow["ObjectType"] = RELATEDOBJECT_FILTER;
                dt.Rows.Add(validDataRow);
            }

            return chkflg;
        }



        #endregion

    }
}
