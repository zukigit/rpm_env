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
using System.Diagnostics;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Threading;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.Threading;
using System.Globalization;

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
    /// プロジェクトの入り口.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 例外発生時のエラーコード
        /// </summary>
        private const int ERROR_EXIT_CODE = 1;

        public App()
        {

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            log4net.GlobalContext.Properties["pid"] = System.Diagnostics.Process.GetCurrentProcess().Id;

            InitializeComponent();
            LoginWindow window = new LoginWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
        }


        /// <summary>プロジェクトの異常処理</summary>
        void App_DispatcherUnhandledException(object sender,
                                DispatcherUnhandledExceptionEventArgs e)
        {
                if (e.Exception is DBException)
                {
                    DBException ex = (DBException)e.Exception;
                    LogInfo.WriteErrorLog(ex.MessageID, ex);
                    LogInfo.WriteFatalLog(ex.InnerException);
                    CommonDialog.ShowErrorDialog(ex.MessageID);
                }
                else if (e.Exception is FileException)
                {
                    FileException ex = (FileException)e.Exception;
                    LogInfo.WriteErrorLog(ex.MessageID, ex.InnerException);
                    CommonDialog.ShowErrorDialog(ex.MessageID);
                }
                else if (e.Exception is BaseException)
                {
                    BaseException ex = (BaseException)e.Exception;
                    LogInfo.WriteErrorLog(ex.MessageID, ex.InnerException);
                    CommonDialog.ShowErrorDialog(ex.MessageID);
                }
                else
                {
                    LogInfo.WriteErrorLog(Consts.SYSERR_003,e.Exception);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_003);
                }


                e.Handled = true;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBoxResult result;

            Exception exception = (Exception)e.ExceptionObject;
            if (exception is DBException)
            {
                DBException ex = (DBException)exception;
                LogInfo.WriteErrorLog(ex.MessageID, ex);
                LogInfo.WriteFatalLog(ex.InnerException);
                result = CommonDialog.ShowErrorDialog(ex.MessageID);
            }
            else if (exception is FileException)
            {
                FileException ex = (FileException)exception;
                LogInfo.WriteErrorLog(ex.MessageID, ex.InnerException);
                result = CommonDialog.ShowErrorDialog(ex.MessageID);
            }
            else if (exception is BaseException)
            {
                BaseException ex = (BaseException)exception;
                LogInfo.WriteErrorLog(ex.MessageID, ex.InnerException);
                result = CommonDialog.ShowErrorDialog(ex.MessageID);
            }
            else
            {
                LogInfo.WriteErrorLog(Consts.SYSERR_003, exception);
                result = CommonDialog.ShowErrorDialog(Consts.SYSERR_003);
            }
            if (result == MessageBoxResult.OK) { Environment.Exit(ERROR_EXIT_CODE); }
        }

    }
}
