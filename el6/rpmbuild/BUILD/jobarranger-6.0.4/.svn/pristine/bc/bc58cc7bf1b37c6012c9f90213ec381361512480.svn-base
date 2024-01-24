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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Security.Cryptography;
using System.Threading;
using System.Globalization;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.IO;
using Microsoft.Win32;

using Ysq.Zabbix;
using System.Dynamic;


namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : BaseWindow
    {

        #region フィールド
        private static String DB_CONFIG_FILE = ".." +
                            System.IO.Path.DirectorySeparatorChar +
                            "conf" +
                            System.IO.Path.DirectorySeparatorChar +
                            "jobarg_manager.conf";

        private DataSet ds;
        private List<String> dsnList;

        #endregion

        #region コンストラクター
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            setContents();

        }
        #endregion

        #region プロパティ
        public ObservableCollection<JobconDBSource> DBInfos { get; set; }
        public String DBConnectStr { get; set; }

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "LoginWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_100;
            }
        }

        #endregion

        #region イベント

        //*******************************************************************
        /// <summary>ログインボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void login_buttonClick(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("login_buttonClick", Consts.PROCESS_010);

            login();

            // 終了ログ
            base.WriteEndLog("login_buttonClick", Consts.PROCESS_010);
        }

        //*******************************************************************
        /// <summary>キャンセルボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void cancel_buttonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        /// <summary>キー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // 開始ログ
                base.WriteStartLog("OnKeyDownHandler", Consts.PROCESS_010);

                login();

                // 終了ログ
                base.WriteEndLog("OnKeyDownHandler", Consts.PROCESS_010);
            }
        }


        #endregion

        #region private メソッド

        /// <summary>ＤＢ設定情報を読み込み画面にセットする</summary>
        private void setContents()
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(@DB_CONFIG_FILE, System.IO.FileMode.Open);
                ds = new DataSet("JobconDBInfo");
                try
                {
                    ds.ReadXml(fs, XmlReadMode.InferSchema);
                }
                catch (Exception ex)
                {
                    fs.Close();
                    throw new FileException(Consts.ERROR_LOGIN_004, ex);
                }
                fs.Close();
                DBInfos = new ObservableCollection<JobconDBSource>();
                DataTable dt = new DataTable();
                if (ds.Tables.Count < 1)
                    throw new FileException(Consts.ERROR_LOGIN_004, null);
                dt = ds.Tables[0];
                if (dt.Rows.Count < 1)
                    throw new FileException(Consts.ERROR_LOGIN_004, null);
                int i = 0;
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        JobconDBSource item = JobconDBSource.Create(dr);
                        if (i == 0)
                        {
                            DBConnectStr = item.ConnnectStr;
                        }
                        DBInfos.Add(item);
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    throw new FileException(Consts.ERROR_LOGIN_004, ex);
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileException(Consts.ERROR_LOGIN_003, ex);
            }

            EnumDsn();

        }

        /// <summary>ログイン処理</summary>
        private void login()
        {
            if (!dsnList.Contains<String>(((JobconDBSource)this.comboBox_jobarg.SelectedItem).DBS))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_LOGIN_005);
                return;
            }
            String connectStr = ((JobconDBSource)this.comboBox_jobarg.SelectedItem).ConnnectStr;
            DBConnect dbConnect;
            bool authSuccess = false;

            dbConnect = new DBConnect(connectStr);
            try
            {
                dbConnect.CreateSqlConnect();
            }
            catch (Exception)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_LOGIN_006);
                return;
            }
            authSuccess = auth(dbConnect);
            if (authSuccess)
            {
                if (!CheckUserStatus(dbConnect))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_LOGIN_002);
                    dbConnect.CloseSqlConnect();
                    return;
                }
                if (LoginSetting.Lang.Equals("ja_jp"))
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP", false);
                }
                else if (LoginSetting.Lang.Equals("ko_kr"))
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ko-KR", false);
                }
                else
                {
                    LoginSetting.Lang = "en_us";
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
                }
                JobArrangerWindow jobArrangerWindow = new JobArrangerWindow();
                jobArrangerWindow.Show();
                this.Close();
            }
            else
            {
                dbConnect.CloseSqlConnect();
                CommonDialog.ShowErrorDialog(Consts.ERROR_LOGIN_001);
            }
        }

        /// <summary>認証処理</summary>
        private bool auth(DBConnect dbConnect)
        {
            UsersDAO dao = new UsersDAO(dbConnect);
            ApiClient api;
            string sessionid="";
            string zabbixURL = ((JobconDBSource)comboBox_jobarg.SelectedItem).ZabbixUrl + "/api_jsonrpc.php";
            try
            {
                zabbixURL = zabbixURL.Replace("//api_jsonrpc.php", "/api_jsonrpc.php");
                
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
                        | (System.Net.SecurityProtocolType)0x00000300  // Tls11 
                        | (System.Net.SecurityProtocolType)0x00000C00  // Tls12 
                        | (System.Net.SecurityProtocolType)0x00003000  // Tls13
                    ;

                }
                catch
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3
                        | System.Net.SecurityProtocolType.Tls
                        ;
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                  delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                  { return true; };
                  
                api = new ApiClient(zabbixURL, textBox_user.Text, passBox_pass.Password);
                api.Login();
                System.Reflection.FieldInfo[] filedList = api.GetType().GetFields(System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                foreach (var filed in filedList)
                {
                    if ("System.String _auth".Equals(filed.ToString()))
                    {
                        //Console.WriteLine(filed + "===" + filed.GetValue(api));
                        sessionid = filed.GetValue(api).ToString();
                        break;
                    }
                }

            }
            catch(Exception ex)
            {
                base.WriteFatalLog(ex);
                return false;
            }
            base.WriteEndLog("zabbix api success sessionid="+ sessionid, "");
            DataTable dt = dao.GetEntityByNameAndPass(textBox_user.Text, sessionid);

            if (dt.Rows.Count == 1)
            {
                DataRow row0 = dt.Rows[0];
                LoginSetting.UserID = Convert.ToDecimal(row0["userid"]);
                LoginSetting.UserName = (String)row0["username"];
                LoginSetting.Authority = (Consts.AuthorityEnum)row0["type"];
                LoginSetting.Mode = Consts.ActionMode.DEVELOP;
                if (LoginSetting.Authority == Consts.AuthorityEnum.GENERAL)
                {
                    LoginSetting.Mode = Consts.ActionMode.USE;
                }
                LoginSetting.ConnectStr = ((JobconDBSource)comboBox_jobarg.SelectedItem).ConnnectStr;
                LoginSetting.GroupList = DBUtil.GetGroupIDListByAlias(LoginSetting.UserName);
                LoginSetting.JobconName = ((JobconDBSource)comboBox_jobarg.SelectedItem).JobconName;

                LoginSetting.DBType = ((JobconDBSource)comboBox_jobarg.SelectedItem).DBType;

                //added by YAMA 2014/02/26
                string wkLang = (String)row0["lang"];
                LoginSetting.Lang = wkLang.ToLower();

                LoginSetting.HealthCheckFlag = ((JobconDBSource)comboBox_jobarg.SelectedItem).HealthCheckFlag;
                LoginSetting.HealthCheckInterval = ((JobconDBSource)comboBox_jobarg.SelectedItem).HealthCheckInterval;

                //added by YAMA 2014/03/03
                LoginSetting.JaZabbixVersion = ((JobconDBSource)comboBox_jobarg.SelectedItem).JaZabbixVersion;

                // added by YAMA 2014/10/20    マネージャ内部時刻同期
                LoginSetting.ManagerTimeSync = DBUtil.GetManagerTimeSync();

                // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
                if (LoginSetting.GroupList.Count == 0)
                {
                    LoginSetting.BelongToUsrgrpFlag = false;
                }
                else
                {
                    LoginSetting.BelongToUsrgrpFlag = true;
                }

                return true;
            }
            return false;
        }

        private bool CheckUserStatus(DBConnect dbConnect)
        {
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                return true;
            }
            String checkSql = "select users.userid,usrgrp.users_status from users,users_groups,usrgrp where users.username='"
                                    + LoginSetting.UserName + "' and users.userid=users_groups.userid and users_groups.usrgrpid=usrgrp.usrgrpid and usrgrp.users_status=1";
            DataTable dt = dbConnect.ExecuteQuery(checkSql);
            if (dt.Rows.Count > 0)
                return false;
            return true;
        }

        /// <summary>パスワードの暗号化</summary>       BCrypt
        private string GetMD5String(string str)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            Byte[] bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>ODBCのDSN一覧を取得</summary>
        private void EnumDsn()
        {
            dsnList = new List<string>();
            dsnList.AddRange(EnumDsn(Registry.CurrentUser));
            dsnList.AddRange(EnumDsn(Registry.LocalMachine));

        }
        /// <summary>特定レジストリーキーのODBCのDSN一覧を取得</summary>
        private IEnumerable<string> EnumDsn(RegistryKey rootKey)
        {
            RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    string value = regKey.GetValue(name, "").ToString();
                    yield return name;
                }
            }
        }

        #endregion
    }
}
