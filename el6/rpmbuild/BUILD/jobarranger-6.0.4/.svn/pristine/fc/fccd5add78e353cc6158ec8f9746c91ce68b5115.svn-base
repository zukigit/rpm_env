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
using System.Text;
using log4net;
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

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// ログ出力の処理.
    /// </summary>
    public class LogInfo
    {

        #region フィールド


        /// <summary> 全角スペース </summary>
        public static string strSpace = "　";

        #endregion

        #region publicメソッド

        /// <summary>DEBUG用の開始ログを出力</summary>
        /// <param name="strClassName">クラス名</param>
        /// <param name="strMethodName">メソッド名</param>
        public static void WriteStartDebugLog(string strClassName, string strMethodName)
        {
            // DEBUG用のログ
            ILog _debugLog = LogManager.GetLogger("debugLog");

            if (!_debugLog.IsDebugEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(strClassName);
            msg.Append(strSpace);
            msg.Append(strMethodName);
            msg.Append(strSpace);
            msg.Append("start");

            _debugLog.Debug(msg.ToString());

        }

        /// <summary>DEBUG用の終了ログを出力</summary>
        /// <param name="strClassName">クラス名</param>
        /// <param name="strMethodName">メソッド名</param>
        public static void WriteEndDebugLog(string strClassName, string strMethodName)
        {
            // DEBUG用のログ
            ILog _debugLog = LogManager.GetLogger("debugLog");

            if (!_debugLog.IsDebugEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(strClassName);
            msg.Append(strSpace);
            msg.Append(strMethodName);
            msg.Append(strSpace);
            msg.Append("end");

            _debugLog.Debug(msg.ToString());

        }

        /// <summary>INFO用の開始ログを出力</summary>
        /// <param name="strGamenId">画面ＩＤ</param>
        /// <param name="strSyoriId">処理ＩＤ</param>
        public static void WriteStartInfoLog(string strGamenId, string strSyoriId)
        {
            // INFO用のログ
            ILog _infoLog = LogManager.GetLogger("infoLog");

            if (!_infoLog.IsInfoEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strGamenId));
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strSyoriId));
            msg.Append(strSpace);
            msg.Append("start");

            _infoLog.Info(msg.ToString());
        }

        /// <summary>INFO用の終了ログを出力</summary>
        /// <param name="strGamenId">画面ＩＤ</param>
        /// <param name="strSyoriId">処理ＩＤ</param>
        public static void WriteEndInfoLog(string strGamenId, string strSyoriId)
        {
            // INFO用のログ
            ILog _infoLog = LogManager.GetLogger("infoLog");

            if (!_infoLog.IsInfoEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strGamenId));
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strSyoriId));
            msg.Append(strSpace);
            msg.Append("end");

            _infoLog.Info(msg.ToString());
        }

        /// <summary>WARN用のログを出力</summary>
        /// <param name="strGamenId">画面ＩＤ</param>
        /// <param name="strMsgId">WarnメッセージＩＤ</param>
        public static void WriteWarnLog(string strMsgId)
        {
            // WARN用のログ
            ILog _warnLog = LogManager.GetLogger("warnLog");

            if (!_warnLog.IsWarnEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strMsgId));

            _warnLog.Warn(msg.ToString());
        }

        /// <summary>ERROR用のログを出力</summary>
        /// <param name="strMsgId">エラーメッセージＩＤ</param>
        /// <param name="ex">例外情報</param>
        public static void WriteErrorLog(string strMsgId, Exception ex)
        {
            // ERROR用のログ
            ILog _errorLog = LogManager.GetLogger("errorLog");

            if (!_errorLog.IsErrorEnabled)
            {
                return;
            }

            StringBuilder msg = new StringBuilder();

            msg.Append(GetUserName());
            msg.Append(strSpace);
            msg.Append(MessageUtil.GetMsgById(strMsgId));

            _errorLog.Error(msg.ToString());

            if (ex == null)
            {
                return;
            }

            _errorLog.Error(ex.GetType().FullName + ":" + ex.Message);

            _errorLog.Error(ex.StackTrace);

            if (ex.InnerException != null)
            {
                _errorLog.Error(ex.InnerException.GetType().FullName + ":"
                                + ex.InnerException.Message);
                _errorLog.Error(ex.InnerException.StackTrace);
            }
        }

        /// <summary>ERROR用のログを出力</summary>
        /// <param name="ex">例外情報</param>
        public static void WriteErrorLog(Exception ex)
        {
            // ERROR用のログ
            ILog _errorLog = LogManager.GetLogger("errorLog");

            if (!_errorLog.IsErrorEnabled)
            {
                return;
            }

            _errorLog.Error(GetUserName());

            if (ex == null)
            {
                return;
            }

            _errorLog.Error(ex.GetType().FullName + ":" + ex.Message);

            _errorLog.Error(ex.StackTrace);

            if (ex.InnerException != null)
            {
                _errorLog.Error(ex.InnerException.GetType().FullName + ":"
                                + ex.InnerException.Message);
                _errorLog.Error(ex.InnerException.StackTrace);
            }
        }

        /// <summary>FATAL用のログを出力</summary>
        /// <param name="ex">例外情報</param>
        public static void WriteFatalLog(Exception ex)
        {
            // FATAL用のログ
            ILog _fatalLog = LogManager.GetLogger("fatalLog");

            if (!_fatalLog.IsFatalEnabled)
            {
                return;
            }

            _fatalLog.Fatal(GetUserName());

            if (ex == null)
            {
                return;
            }

            _fatalLog.Fatal(ex.GetType().FullName + ":" + ex.Message);

            _fatalLog.Fatal(ex.StackTrace);

            if (ex.InnerException != null)
            {
                _fatalLog.Fatal(ex.InnerException.GetType().FullName + ":"
                                + ex.InnerException.Message);
                _fatalLog.Fatal(ex.InnerException.StackTrace);
            }
        }

        /// <summary>DEBUGログを出力</summary>
        /// <param name="message">メッセージ</param>
        public static void WriteDebugLog(string message)
        {
            // INFO用のログ
            ILog _debugLog = LogManager.GetLogger("debugLog");

            if (!_debugLog.IsDebugEnabled)
            {
                return;
            }

            _debugLog.Info(message);
        }

        #endregion

        #region privateメソッド

        /// <summary>ユーザー名の取得</summary>
        /// <return>ユーザー名</return>
        private static string GetUserName()
        {
            string strUserNmae = LoginSetting.UserName;

            if (strUserNmae == null || strUserNmae.Length == 0)
            {
                strUserNmae = "-";
            }

            return strUserNmae;
        }

        #endregion
    }
}
