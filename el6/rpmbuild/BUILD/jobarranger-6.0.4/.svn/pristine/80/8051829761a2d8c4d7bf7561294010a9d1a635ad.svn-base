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
using System.Windows;
using System.Data;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Setting
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// AgentlessSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class AgentlessSetting : Window
    {
        #region フィールド

        private string _selectForHostSql = "select distinct(hosts.hostid), hosts.host from users " +
            "inner join users_groups on users.userid = users_groups.userid inner join usrgrp " +
            "on users_groups.usrgrpid = usrgrp.usrgrpid inner join rights on usrgrp.usrgrpid = rights.groupid " +
            "inner join hosts_groups on rights.id = hosts_groups.groupid inner join hosts on " +
            "hosts_groups.hostid = hosts.hostid " +
            "where users.username = ? and rights.permission <> '0' and hosts.status in (0,1) and hosts.flags = 0 " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"order by hosts.hostid ASC";
            "order by hosts.host ASC";

        //added by YAMA 2014/08/08    （ホスト名でソート）
        //private string _selectForHostSqlSuper = "select hostid, host from hosts where status=0 or status=1 order by hostid ASC";
        private string _selectForHostSqlSuper = "select hostid, host from hosts where status in (0,1) and flags = 0 order by host ASC";

        #endregion

        // 各ComboBoxのITEMの格納場所
        DataTable CombDataAuthentic = new DataTable();          // 認証方式(combAuthentic)
        DataTable CombDataExecutive = new DataTable();          // 実行モード(combExecutiveMode)
        DataTable CombDatacombCharcode = new DataTable();       // 文字コード(combCharcode)
        DataTable CombDataLinefeedcode = new DataTable();       // 改行コード(combLinefeedcode)


        public AgentlessSetting()
        {
            InitializeComponent();
        }

        public AgentlessSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            SetValues(jobId, Consts.EditType.Modify);

            if (_myJob.ContentItem.InnerJobId != null)
            {
                ChangeButton4DetailRef();
            }
        }
        public AgentlessSetting(IRoom room, string jobId, Consts.EditType editType)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            SetValues(jobId, editType);

            if (_myJob.ContentItem.InnerJobId != null)
            {
                ChangeButton4DetailRef();
            }
        }

        #region プロパティ
        /// <summary>ジョブ</summary>
        private IRoom _myJob;
        public IRoom MyJob
        {
            get
            {
                return _myJob;
            }
            set
            {
                _myJob = value;
            }
        }

        /// <summary>ジョブID</summary>
        private string _oldJobId;
        public string OldJobId
        {
            get
            {
                return _oldJobId;
            }
            set
            {
                _oldJobId = value;
            }
        }
        #endregion


        /// <summary>登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            // 入力チェック
            if (!InputCheck())
            {
                return;
            }

            //処理前現在データで履歴を作成
            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Container)_myJob.Container).CreateHistData();

            // 入力されたジョブID
            string newJobId = txtJobId.Text;

            // 入力されたジョブ名
            string newJobNm = txtJobName.Text;

            // ジョブ管理テーブルの更新
            DataRow[] rowJobCon = _myJob.Container.JobControlTable.Select("job_id='" + _oldJobId + "'");
            if (rowJobCon != null && rowJobCon.Length > 0)
            {
                rowJobCon[0]["job_id"] = newJobId;
                rowJobCon[0]["job_name"] = newJobNm;
            }

            // エージェントレスアイコン設定テーブルの更新
            DataRow[] rowIconAgentlessTbl = _myJob.Container.IconAgentlessTable.Select("job_id='" + _oldJobId + "'");
            if (rowIconAgentlessTbl != null && rowIconAgentlessTbl.Length > 0)
            {
                // ジョブID
                rowIconAgentlessTbl[0]["job_id"] = newJobId;

                // セッションフラグ
                RadioButton[] rbMode = new RadioButton[4];
                rbMode[0] = rbOnetime;
                rbMode[1] = rbConnect;
                rbMode[2] = rbContinue;
                rbMode[3] = rbDisconnect;
                int idx = 0;
                for (idx = 0; idx < 4; idx++)
                {
                    if (rbMode[idx].IsChecked == true)
                    {
                        break;
                    }
                }
                rowIconAgentlessTbl[0]["session_flag"] = idx;

                // セッションID
                if (txtSessionID.IsEnabled == false)
                {
                    // セッションID非活性時
                    rowIconAgentlessTbl[0]["session_id"] = null;
                }
                else
                {
                    // セッションID活性時
                    if (String.IsNullOrEmpty(txtSessionID.Text) == false)
                    {
                        // nullではなく、かつ空文字列でもない
                        rowIconAgentlessTbl[0]["session_id"] = txtSessionID.Text;

                    }
                    else
                    {
                        // null、もしくは空文字列である
                        rowIconAgentlessTbl[0]["session_id"] = null;
                    }
                }

                // ホストフラグ
                if (rbHostName.IsChecked == true)
                {
                    rowIconAgentlessTbl[0]["host_flag"] = 0;
                    // ホスト名
                    rowIconAgentlessTbl[0]["host_name"] = Convert.ToString(combHostName.SelectedValue);
                }
                else
                {
                    rowIconAgentlessTbl[0]["host_flag"] = 1;
                    // ホスト名
                    rowIconAgentlessTbl[0]["host_name"] = textVariableName.Text;
                }

                // ホストがグレーアウトされていた場合
                if (gbHost.IsEnabled == false)
                {
                    // ホストフラグ(host_flag)を『0：ホスト名（初期値）』に設定
                    rowIconAgentlessTbl[0]["host_flag"] = 0;
                    // ホスト名 を初期化
                    rowIconAgentlessTbl[0]["host_name"] = "";
                }

                // 認証方式
                if (combAuthentic.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["auth_method"] = combAuthentic.SelectedIndex;
                }
                else
                {
                    rowIconAgentlessTbl[0]["auth_method"] = 0;
                }

                // 実行モード
                if (combExecutiveMode.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["run_mode"] = combExecutiveMode.SelectedIndex;
                }
                else
                {
                    rowIconAgentlessTbl[0]["run_mode"] = 0;
                }

                // ユーザー名
                if (txtUserName.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["login_user"] = txtUserName.Text;
                }
                else
                {
                    rowIconAgentlessTbl[0]["login_user"] = "";
                }
                // パスワード
                if (txtPassword.IsEnabled == true)
                {
                    //Park.iggy Add
                    String strLength = txtPassword.Text.Length+"|";

                    rowIconAgentlessTbl[0]["login_password"] = ConvertUtil.getPasswordFromString((strLength + txtPassword.Text));
                    // rowIconAgentlessTbl[0]["login_password"] = txtPassword.Password;
                }
                else
                {
                    rowIconAgentlessTbl[0]["login_password"] = "";
                }
                // 公開鍵
                if (txtPublicKey.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["public_key"] = txtPublicKey.Text;
                }
                else
                {
                    rowIconAgentlessTbl[0]["public_key"] = "";
                }
                // 秘密鍵
                if (txtPrivateKey.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["private_key"] = txtPrivateKey.Text;
                }
                else
                {
                    rowIconAgentlessTbl[0]["private_key"] = "";
                }
                // パスフレーズ
                if (txtPassPhrase.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["passphrase"] = txtPassPhrase.Text;
                    // rowIconAgentlessTbl[0]["passphrase"] = txtPassPhrase.Password;
                }
                else
                {
                    rowIconAgentlessTbl[0]["passphrase"] = "";
                }
                // 実行コマンド
                if (txtCmd.IsEnabled == true)
                {
                    if (String.IsNullOrEmpty(txtCmd.Text) == false)
                    {
                        // nullではなく、かつ空文字列でもない
                        // 最後に改行コードを設定
                        txtCmd.Text = txtCmd.Text.TrimEnd('\r', '\n');
                        txtCmd.Text = txtCmd.Text + Environment.NewLine;
                        rowIconAgentlessTbl[0]["command"] = txtCmd.Text;
                    }
                    else
                    {
                        // null、もしくは空文字列である
                        rowIconAgentlessTbl[0]["command"] = null;
                    }
                }
                else
                {
                    rowIconAgentlessTbl[0]["command"] = null;
                }


                // プロンプト文字列
                if (txtPromptString.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["prompt_string"] = txtPromptString.Text;
                }
                else
                {
                    rowIconAgentlessTbl[0]["prompt_string"] = "";
                }

                // 文字コード
                if (combCharcode.IsEnabled == true)
                {
                    // 新規入力時
                    if (combCharcode.SelectedValue == null)
                    {
                        rowIconAgentlessTbl[0]["character_code"] = combCharcode.Text.Trim().ToUpper();
                    }
                    // 指定なし選択時
                    else if (combCharcode.SelectedValue.ToString() == "0")
                    {
                        rowIconAgentlessTbl[0]["character_code"] = null;
                    }
                    // コンボボックスに登録されている文字コード
                    else
                    {
                        rowIconAgentlessTbl[0]["character_code"] = combCharcode.Text;
                        //                    rowIconAgentlessTbl[0]["character_code"] = combCharcode.SelectedValue;
                    }
                }
                else
                {
                    rowIconAgentlessTbl[0]["character_code"] = null;
                }

                // 改行コード
                if (combLinefeedcode.IsEnabled == true)
                // if (txtTimeOut.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["line_feed_code"] = combLinefeedcode.SelectedValue;
                }
                else
                {
                    rowIconAgentlessTbl[0]["line_feed_code"] = 0;
                }
                // タイムアウト警告
                if (txtTimeOut.IsEnabled == true)
                {
                    rowIconAgentlessTbl[0]["timeout"] = txtTimeOut.Text;
                }
                else
                {
                    rowIconAgentlessTbl[0]["timeout"] = 0;
                }
                // ジョブ停止コード
                if (txtStopCode.IsEnabled == true)
                {
                    if (String.IsNullOrEmpty(txtStopCode.Text) == false)
                    {
                        // nullではなく、かつ空文字列でもない
                        rowIconAgentlessTbl[0]["stop_code"] = txtStopCode.Text;
                    }
                    else
                    {
                        // null、もしくは空文字列である
                        rowIconAgentlessTbl[0]["stop_code"] = null;
                    }
                }
                else
                {
                    rowIconAgentlessTbl[0]["stop_code"] = null;
                }

                // 端末タイプ
                rowIconAgentlessTbl[0]["terminal_type"] = "vanilla";

                // 接続方式
                rowIconAgentlessTbl[0]["connection_method"] = 0;

                // 強制実行フラグ
                if (cbForce.IsChecked == false)
                {
                    rowJobCon[0]["force_flag"] = "0";
                }
                else
                {
                    rowJobCon[0]["force_flag"] = "1";
                }
            }


            // ジョブIDが変更された場合、フロー管理テーブルを更新
            if (!_oldJobId.Equals(newJobId))
                CommonUtil.UpdateFlowForJobId(_myJob.Container.FlowControlTable, _oldJobId, newJobId);

            // 画面再表示
            _myJob.Container.JobItems.Remove(_oldJobId);
            _myJob.Container.JobItems.Add(newJobId, _myJob);
            _myJob.JobId = newJobId;
            _myJob.JobName = newJobNm;

            _myJob.Container.SetedJobIds[_myJob.JobId] = "1";
            this.Close();
        }


        /// <summary>ワンタイム を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbOnetime_Checked(object sender, RoutedEventArgs e)
        {
            // ホスト全項目  ： 入力可
            // SSH全項目     ： 入力可
            // セッションID  ： グレーアウト
            // ｼﾞｮﾌﾞ停止ｺｰﾄﾞ ： 入力可
            // ﾌﾟﾛﾝﾌﾟﾄ文字列 ： 入力可
            // 実行          ： 入力可
            // 文字コード    ： 入力可
            // 改行コード    ： 入力可
            // タイムアウト  ： 入力可

            gbHost.IsEnabled = true;			// ホストの全項目
            gbSSH.IsEnabled = true;				// SSHの全項目
            txtSessionID.IsEnabled = false;		// セッションID

            txtStopCode.IsEnabled = true;		// ジョブ停止コード
            txtPromptString.IsEnabled = true;	// ﾌﾟﾛﾝﾌﾟﾄ文字列
            txtCmd.IsEnabled = true; 			// 実行
            combCharcode.IsEnabled = true; 		// 文字コード
            combLinefeedcode.IsEnabled = true; 	// 改行コード
            txtTimeOut.IsEnabled = true; 		// タイムアウト
        }


        /// <summary>接続 を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbConnect_Checked(object sender, RoutedEventArgs e)
        {
            // ホスト全項目  ： 入力可
            // SSH全項目     ： 入力可
            // セッションID  ： 入力可
            // ｼﾞｮﾌﾞ停止ｺｰﾄﾞ ： 入力可
            // ﾌﾟﾛﾝﾌﾟﾄ文字列 ： 入力可
            // 実行          ： 入力可
            // 文字コード    ： 入力可
            // 改行コード    ： 入力可
            // タイムアウト  ： 入力可

            gbHost.IsEnabled = true;			// ホストの全項目
            gbSSH.IsEnabled = true;				// SSHの全項目
            txtSessionID.IsEnabled = true;		// セッションID

            txtStopCode.IsEnabled = true;		// ジョブ停止コード
            txtPromptString.IsEnabled = true;	// ﾌﾟﾛﾝﾌﾟﾄ文字列
            txtCmd.IsEnabled = true; 			// 実行
            combCharcode.IsEnabled = true; 		// 文字コード
            combLinefeedcode.IsEnabled = true; 	// 改行コード
            txtTimeOut.IsEnabled = true; 		// タイムアウト
        }


        /// <summary>継続 を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbContinue_Checked(object sender, RoutedEventArgs e)
        {
            // ホスト全項目  ： グレーアウト
            // SSH全項目     ： グレーアウト
            // セッションID  ： 入力可
            // ｼﾞｮﾌﾞ停止ｺｰﾄﾞ ： 入力可
            // ﾌﾟﾛﾝﾌﾟﾄ文字列 ： 入力可
            // 実行          ： 入力可
            // 文字コード    ： 入力可
            // 改行コード    ： 入力可
            // タイムアウト  ： 入力可

            gbHost.IsEnabled = false;			// ホストの全項目
            gbSSH.IsEnabled = false;			// SSHの全項目
            txtSessionID.IsEnabled = true;		// セッションID

            txtStopCode.IsEnabled = true;		// ジョブ停止コード
            txtPromptString.IsEnabled = true;	// ﾌﾟﾛﾝﾌﾟﾄ文字列
            txtCmd.IsEnabled = true; 			// 実行
            combCharcode.IsEnabled = false;		// 文字コード (6/2 true→false)
            combLinefeedcode.IsEnabled = false; // 改行コード (6/2 true→false)
            txtTimeOut.IsEnabled = true; 		// タイムアウト
        }


        /// <summary>切断 を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbDisconnect_Checked(object sender, RoutedEventArgs e)
        {
            // ホスト全項目  ： グレーアウト
            // SSH全項目     ： グレーアウト
            // セッションID  ： 入力可
            // ｼﾞｮﾌﾞ停止ｺｰﾄﾞ ： グレーアウト
            // ﾌﾟﾛﾝﾌﾟﾄ文字列 ： グレーアウト
            // 実行          ： グレーアウト
            // 文字コード    ： グレーアウト
            // 改行コード    ： グレーアウト
            // タイムアウト  ： グレーアウト

            gbHost.IsEnabled = false;			// ホストの全項目
            gbSSH.IsEnabled = false;			// SSHの全項目
            txtSessionID.IsEnabled = true;		// セッションID
            txtStopCode.IsEnabled = false;		// ジョブ停止コード

            txtPromptString.IsEnabled = false;	// ﾌﾟﾛﾝﾌﾟﾄ文字列
            txtCmd.IsEnabled = false; 			// 実行
            combCharcode.IsEnabled = false; 	// 文字コード
            combLinefeedcode.IsEnabled = false; // 改行コード
            txtTimeOut.IsEnabled = false; 		// タイムアウト
        }


        /// <summary>ホスト名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbHostName_Checked(object sender, RoutedEventArgs e)
        {
            combHostName.IsEnabled = true;
            textVariableName.IsEnabled = false;
        }


        /// <summary>変数名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbVariableName_Checked(object sender, RoutedEventArgs e)
        {
            combHostName.IsEnabled = false;
            textVariableName.IsEnabled = true;
        }


        /// <summary>	認証方式を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void combAuthentic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 認証方法が "パスワード"の場合
            //		ﾊﾟｽﾜｰﾄﾞ  ： 入力可
            //		公開鍵   ： グレーアウト
            //		秘密鍵   ： グレーアウト
            //		ﾊﾟｽﾌﾚｰｽﾞ ： グレーアウト
            // 認証方法が "公開鍵"の場合
            //		ﾊﾟｽﾜｰﾄﾞ  ： グレーアウト
            //		公開鍵   ： 入力可
            //		秘密鍵   ： 入力可
            //		ﾊﾟｽﾌﾚｰｽﾞ ： 入力可
            if (Convert.ToInt16(combAuthentic.SelectedValue) == 0)
            {
                // 認証方法が "パスワード"
                txtPassword.IsEnabled = true;		// パスワード
                txtPublicKey.IsEnabled = false;		// 公開鍵
                txtPrivateKey.IsEnabled = false;	// 秘密鍵
                txtPassPhrase.IsEnabled = false;	// パスフレーズ
            }
            else
            {
                // 認証方法が "公開鍵"
                txtPassword.IsEnabled = false;		// パスワード
                txtPublicKey.IsEnabled = true;		// 公開鍵
                txtPrivateKey.IsEnabled = true;	    // 秘密鍵
                txtPassPhrase.IsEnabled = true;	    // パスフレーズ
            }
        }


        //        /// <summary>	実行モードを選択</summary>
        //        /// <param name="sender">源</param>
        //        /// <param name="e">イベント</param>
        //        private void combExecutiveMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //        {
        //            // 実行モード		ﾌﾟﾛﾝﾌﾟﾄ文字列
        //            //--------------	-------------
        //            //	非対話モード	グレーアウト
        //            //	対話モード		入力可
        //            if (Convert.ToInt16(combExecutiveMode.SelectedValue) == 0)
        //            {
        //                // 実行モードが "非対話モード"
        //                txtPromptString.IsEnabled = false;		// 『プロンプト文字列』をグレーアウト
        //            }
        //            else
        //            {
        //                // 実行モードが "対話モード"
        //                txtPromptString.IsEnabled = true;		// 『プロンプト文字列』を入力可能
        //            }
        //        }


        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public void SetDisable()
        {
            // ジョブID,ジョブ名
            txtJobId.IsEnabled = false;
            txtJobName.IsEnabled = false;

            // セッション
            rbOnetime.IsEnabled = false;			// rbﾜﾝﾀｲﾑ
            rbConnect.IsEnabled = false;			// rb接続
            rbContinue.IsEnabled = false;			// rb継続
            rbDisconnect.IsEnabled = false;			// rb切断
            txtSessionID.IsEnabled = false;			// txｾｯｼｮﾝID

            // ホスト
            rbHostName.IsEnabled = false;
            combHostName.IsEnabled = false;
            rbVariableName.IsEnabled = false;
            textVariableName.IsEnabled = false;

            // SSH
            combAuthentic.IsEnabled = false;
            combExecutiveMode.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtPublicKey.IsEnabled = false;
            txtPrivateKey.IsEnabled = false;
            txtPassPhrase.IsEnabled = false;

            // コマンド
            txtCmd.IsEnabled = false;
            txtPromptString.IsEnabled = false;
            combCharcode.IsEnabled = false;
            combLinefeedcode.IsEnabled = false;
            txtTimeOut.IsEnabled = false;
            txtStopCode.IsEnabled = false;
            cbForce.IsEnabled = false;

            // 登録ﾎﾞﾀﾝ
            btnToroku.IsEnabled = false;
        }



        #region privateメッソド

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues(string jobId, Consts.EditType editType)
        {
            // ジョブ管理テーブルのデータを取得
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);
            }

            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();

            // ホスト情報の取得
            DataTable dtHost;
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                dtHost = dbAccess.ExecuteQuery(_selectForHostSqlSuper);
            }
            else
            {
                List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                sqlParams.Add(new ComSqlParam(DbType.String, "@username", LoginSetting.UserName));
                dtHost = dbAccess.ExecuteQuery(_selectForHostSql, sqlParams);
            }

            //ホスト情報
            combHostName.Items.Clear();
            combHostName.ItemsSource = dtHost.DefaultView;
            combHostName.DisplayMemberPath = Convert.ToString(dtHost.Columns["host"]);
            combHostName.SelectedValuePath = Convert.ToString(dtHost.Columns["host"]);

            // '認証方式' のComboBox(combAuthentic)の設定
            DataRow CombDataRow;
            //  CombDataAuthenticに列を追加
            CombDataAuthentic.Columns.Add("ID", typeof(string));     // 選択時のID用
            CombDataAuthentic.Columns.Add("ITEM", typeof(string));   // 表示用

            //  各列に値をセット
            CombDataRow = CombDataAuthentic.NewRow();
            CombDataRow["ID"] = "0";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_passwd_text;
            CombDataAuthentic.Rows.Add(CombDataRow);

            CombDataRow = CombDataAuthentic.NewRow();
            CombDataRow["ID"] = "1";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_publickey_text;
            CombDataAuthentic.Rows.Add(CombDataRow);

            combAuthentic.Items.Clear();
            combAuthentic.ItemsSource = CombDataAuthentic.DefaultView;
            combAuthentic.DisplayMemberPath = "ITEM";
            combAuthentic.SelectedValuePath = "ID";

            // 実行モード
            //  DataTableの列に型を定義
            CombDataExecutive.Columns.Add("ID", typeof(string));     // 選択時のID用
            CombDataExecutive.Columns.Add("ITEM", typeof(string));   // 表示用
            //  DataTableの行に項目を追加
            //   『対話モード』 を追加
            CombDataRow = CombDataExecutive.NewRow();
            CombDataRow["ID"] = "0";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_interactive_text;
            CombDataExecutive.Rows.Add(CombDataRow);
            //   『非対話モード』 を追加
            CombDataRow = CombDataExecutive.NewRow();
            CombDataRow["ID"] = "1";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_noninteractive_text;
            CombDataExecutive.Rows.Add(CombDataRow);
            //  DataTableから値を取得してコンボボックスへ項目追加
            combExecutiveMode.Items.Clear();
            combExecutiveMode.ItemsSource = CombDataExecutive.DefaultView;
            combExecutiveMode.DisplayMemberPath = "ITEM";
            combExecutiveMode.SelectedValuePath = "ID";

            // 文字コード　(コンボボックス )
            //  CombDatacombCharcodeに列を追加
            CombDatacombCharcode.Columns.Add("ID", typeof(string));     // 選択時のID用
            CombDatacombCharcode.Columns.Add("ITEM", typeof(string));   // 表示用
            //  指定なし を列にセット
            CombDataRow = CombDatacombCharcode.NewRow();
            CombDataRow["ID"] = "0";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_unspecified_text;
            CombDatacombCharcode.Rows.Add(CombDataRow);
            //  UTF-8 を列にセット
            CombDataRow = CombDatacombCharcode.NewRow();
            CombDataRow["ID"] = "1";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_UTF8_text;
            CombDatacombCharcode.Rows.Add(CombDataRow);
            //  SHIFT_JIS を列にセット
            CombDataRow = CombDatacombCharcode.NewRow();
            CombDataRow["ID"] = "2";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_SHIFT_JIS_text;
            CombDatacombCharcode.Rows.Add(CombDataRow);
            //  EUC-JP を列にセット
            CombDataRow = CombDatacombCharcode.NewRow();
            CombDataRow["ID"] = "3";
            CombDataRow["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_EUC_JP_text;
            CombDatacombCharcode.Rows.Add(CombDataRow);
            //  項目追加
            combCharcode.Items.Clear();
            combCharcode.ItemsSource = CombDatacombCharcode.DefaultView;
            combCharcode.DisplayMemberPath = "ITEM";
            combCharcode.SelectedValuePath = "ID";

            // 改行コード  (コンボボックス)
            //  DataTableの列に型を定義
            CombDataLinefeedcode.Columns.Add("ID", typeof(string));     // 選択時のID用
            CombDataLinefeedcode.Columns.Add("ITEM", typeof(string));   // 表示用
            //  行を追加
            DataRow rowLF;
            rowLF = CombDataLinefeedcode.NewRow();				        // LF を追加
            rowLF["ID"] = "0";
            rowLF["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_LF_text;
            CombDataLinefeedcode.Rows.Add(rowLF);
            rowLF = CombDataLinefeedcode.NewRow();				        // CR を追加
            rowLF["ID"] = "1";
            rowLF["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_CR_text;
            CombDataLinefeedcode.Rows.Add(rowLF);
            rowLF = CombDataLinefeedcode.NewRow();				        // CRLF を追加
            rowLF["ID"] = "2";
            rowLF["ITEM"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_CRLF_text;
            CombDataLinefeedcode.Rows.Add(rowLF);
            //  DataTableから値を取得してコンボボックスへ項目追加
            combLinefeedcode.Items.Clear();
            combLinefeedcode.ItemsSource = CombDataLinefeedcode.DefaultView;
            combLinefeedcode.DisplayMemberPath = "ITEM";
            combLinefeedcode.SelectedValuePath = "ID";

            // エージェントレスアイコン設定テーブルのデータを取得 前回設定状態を復元する
            DataRow[] rowIconAgentless;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconAgentless = _myJob.Container.IconAgentlessTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconAgentless = _myJob.Container.IconAgentlessTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }

            if (rowIconAgentless != null && rowIconAgentless.Length > 0)
            {

                // セッション情報
                int sessionFlg = Convert.ToInt16(rowIconAgentless[0]["session_flag"]);

                RadioButton[] rbMode = new RadioButton[4];
                rbMode[0] = rbOnetime;
                rbMode[1] = rbConnect;
                rbMode[2] = rbContinue;
                rbMode[3] = rbDisconnect;

                rbMode[sessionFlg].IsChecked = true;

                switch (sessionFlg)
                {
                    // ワンタイム
                    case 0:
                        gbHost.IsEnabled = true;            // ホストの全項目
                        gbSSH.IsEnabled = true;             // SSHの全項目
                        txtSessionID.IsEnabled = false;     // セッションID

                        txtStopCode.IsEnabled = true;       // ジョブ停止コード
                        txtPromptString.IsEnabled = true;   // ﾌﾟﾛﾝﾌﾟﾄ文字列
                        txtCmd.IsEnabled = true;            // 実行
                        combCharcode.IsEnabled = true;      // 文字コード
                        combLinefeedcode.IsEnabled = true;  // 改行コード
                        txtTimeOut.IsEnabled = true;        // タイムアウト
                        break;

                    // 接続
                    case 1:
                        gbHost.IsEnabled = true;            // ホストの全項目
                        gbSSH.IsEnabled = true;             // SSHの全項目
                        txtSessionID.IsEnabled = true;      // セッションID

                        txtStopCode.IsEnabled = true;       // ジョブ停止コード
                        txtPromptString.IsEnabled = true;   // ﾌﾟﾛﾝﾌﾟﾄ文字列
                        txtCmd.IsEnabled = true;            // 実行
                        combCharcode.IsEnabled = true;      // 文字コード
                        combLinefeedcode.IsEnabled = true;  // 改行コード
                        txtTimeOut.IsEnabled = true;        // タイムアウト
                        break;

                    // 継続
                    case 2:
                        gbHost.IsEnabled = false;           // ホストの全項目
                        gbSSH.IsEnabled = false;            // SSHの全項目
                        txtSessionID.IsEnabled = true;      // セッションID

                        txtStopCode.IsEnabled = true;       // ジョブ停止コード
                        txtPromptString.IsEnabled = true;   // ﾌﾟﾛﾝﾌﾟﾄ文字列
                        txtCmd.IsEnabled = true;            // 実行
                        combCharcode.IsEnabled = false;     // 文字コード (6/2 true→false)
                        combLinefeedcode.IsEnabled = false; // 改行コード (6/2 true→false)
                        txtTimeOut.IsEnabled = true;        // タイムアウト
                        break;

                    // 切断
                    case 3:
                        gbHost.IsEnabled = false;           // ホストの全項目
                        gbSSH.IsEnabled = false;            // SSHの全項目
                        txtSessionID.IsEnabled = true;      // セッションID
                        txtStopCode.IsEnabled = false;      // ジョブ停止コード

                        txtPromptString.IsEnabled = false;  // ﾌﾟﾛﾝﾌﾟﾄ文字列
                        txtCmd.IsEnabled = false;           // 実行
                        combCharcode.IsEnabled = false;     // 文字コード
                        combLinefeedcode.IsEnabled = false; // 改行コード
                        txtTimeOut.IsEnabled = false;       // タイムアウト
                        break;
                }


                // セッションID
                txtSessionID.Text = Convert.ToString(rowIconAgentless[0]["session_id"]);

                // ホスト
                string hostFlag = Convert.ToString(rowIconAgentless[0]["host_flag"]);
                string hostName = Convert.ToString(rowIconAgentless[0]["host_name"]);
                if ("1".Equals(hostFlag))
                {
                    rbVariableName.IsChecked = true;
                    textVariableName.Text = hostName;
                }
                else
                {
                    if (editType == Consts.EditType.READ)
                    {
                        DataRow[] rows = dtHost.Select("host='" + hostName + "'");
                        if (rows.Length < 1)
                        {
                            DataRow row = dtHost.NewRow();
                            row["host"] = hostName;
                            dtHost.Rows.Add(row);
                        }
                    }

                    rbHostName.IsChecked = true;
                    combHostName.SelectedValue = hostName;
                }

                // 認証方式
                string authMethod = Convert.ToString(rowIconAgentless[0]["auth_method"]);
                combAuthentic.SelectedValue = authMethod;

                if (Convert.ToInt16(authMethod) == 0)
                {
                    // 認証方法が "パスワード"
                    txtPassword.IsEnabled = true;		// パスワード
                    txtPublicKey.IsEnabled = false;		// 公開鍵
                    txtPrivateKey.IsEnabled = false;	// 秘密鍵
                    txtPassPhrase.IsEnabled = false;	// パスフレーズ
                }
                else
                {
                    // 認証方法が "公開鍵"
                    txtPassword.IsEnabled = false;		// パスワード
                    txtPublicKey.IsEnabled = true;		// 公開鍵
                    txtPrivateKey.IsEnabled = true;	    // 秘密鍵
                    txtPassPhrase.IsEnabled = true;	    // パスフレーズ
                }

                // 実行モード
                string run_mode = Convert.ToString(rowIconAgentless[0]["run_mode"]);
                combExecutiveMode.SelectedValue = run_mode;

                // ユーザー名
                txtUserName.Text = Convert.ToString(rowIconAgentless[0]["login_user"]);

                // パスワード
                //Park.iggy Add
                String strPassOrg = Convert.ToString(rowIconAgentless[0]["login_password"]);
                int len = 0;
                int strIndex = 0;
                String passWd = null;

                if (strPassOrg != null && strPassOrg.Length > 0 && strPassOrg.Substring(0, 1).Equals("1"))
                {
                    try
                    {
                        strPassOrg = ConvertUtil.getStringFromX16Password(strPassOrg);
                        strIndex = strPassOrg.IndexOf("|");
                        if (strIndex == 0) throw new FormatException();
                        len = int.Parse(strPassOrg.Substring(0, strIndex));
                        passWd = strPassOrg.Substring(strIndex+1);
                        if (len != passWd.Length) throw new FormatException();
                    }
                    catch (FormatException)
                    {
                        passWd = Convert.ToString(rowIconAgentless[0]["login_password"]);
                    }
                }
                else passWd = strPassOrg;


                if (_myJob.ContentItem.InnerJobId == null)
                {
                    DataRow[] rowJobNet = _myJob.Container.JobnetControlTable.Select("jobnet_id='" + Convert.ToString(rowJob[0]["jobnet_id"]) + "'" +
                       " AND update_date='" + Convert.ToString(rowJob[0]["update_date"]) + "'");
                    if (rowJobNet.Length > 0)
                    {
                        String objectUserName = rowJobNet[0]["user_name"].ToString();
                        List<Decimal> objectUserGroupList = DBUtil.GetGroupIDListByAlias(objectUserName);

                        if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER) &&
                            (Consts.ActionMode.USE == LoginSetting.Mode ||
                            !CheckUtil.isExistGroupId(LoginSetting.GroupList, objectUserGroupList)) &&
                            objectUserName.Length > 0)
                        {
                            if (passWd != null && !passWd.Equals(""))
                            {
                                passWd = "******";
                            }
                        }
                    }
                }
                else
                {
                    if (passWd != null && !passWd.Equals(""))
                    {
                        passWd = "******";
                    }
                }


                txtPassword.Text = passWd;

                //txtPassword.Text = Convert.ToString(rowIconAgentless[0]["login_password"]);
                // txtPassword.Password = Convert.ToString(rowIconAgentless[0]["login_password"]);

                // 公開鍵
                txtPublicKey.Text = Convert.ToString(rowIconAgentless[0]["public_key"]);

                // 秘密鍵
                txtPrivateKey.Text = Convert.ToString(rowIconAgentless[0]["private_key"]);

                // パスフレーズ
                txtPassPhrase.Text = Convert.ToString(rowIconAgentless[0]["passphrase"]);
                // txtPassPhrase.Password = Convert.ToString(rowIconAgentless[0]["passphrase"]);

                // 実行コマンド
                txtCmd.Text = Convert.ToString(rowIconAgentless[0]["command"]);

                // プロンプト文字列
                txtPromptString.Text = Convert.ToString(rowIconAgentless[0]["prompt_string"]);

                // 文字コード
                string chrCode = Convert.ToString(rowIconAgentless[0]["character_code"]);
                int cnt = CombDatacombCharcode.Rows.Count;
                bool existFlg = false;
                DataRow CombDataRowChrcd;
                int idx = 0;
                // コンボボックス行データ用
                for (idx = 0; idx < cnt; idx++)         // 入力項目が既登録かチェック
                {
                    if (chrCode == CombDatacombCharcode.Rows[idx]["ITEM"].ToString())
                    {
                        existFlg = true;
                        break;
                    }
                }
                // 指定なしを設定
                if (String.IsNullOrEmpty(chrCode) == true)
                {
                    chrCode = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_unspecified_text;
                    //                    chrCode = "0";
                }
                else
                {
                    // 追加登録時
                    if (existFlg == false)
                    {
                        CombDataRowChrcd = CombDatacombCharcode.NewRow();
                        CombDataRowChrcd["ID"] = cnt.ToString();
                        CombDataRowChrcd["ITEM"] = chrCode;
                        CombDatacombCharcode.Rows.Add(CombDataRowChrcd);
                    }
                }
                if (existFlg == true)
                {
                    combCharcode.SelectedValue = idx.ToString();
                }
                else
                {
                    if (chrCode == Properties.Resources.agentless_comb_unspecified_text)
                    {
                        combCharcode.SelectedValue = "0";
                    }
                    else
                    {
                        combCharcode.SelectedValue = cnt.ToString();
                    }

                }


                // 改行コード
                string newLine = Convert.ToString(rowIconAgentless[0]["line_feed_code"]);
                combLinefeedcode.SelectedValue = newLine;

                // タイムアウト警告
                txtTimeOut.Text = Convert.ToString(rowIconAgentless[0]["timeout"]);

                // ジョブ停止コード
                txtStopCode.Text = Convert.ToString(rowIconAgentless[0]["stop_code"]);

                // 強制実行
                string forceFlag = Convert.ToString(rowJob[0]["force_flag"]);
                if ("1".Equals(forceFlag))
                {
                    cbForce.IsChecked = true;
                }
            }
        }


        /// <summary> 詳細画面からの参照時のボタンの切り替え</summary>
        private void ChangeButton4DetailRef()
        {
            GridSetting.Children.Remove(btnToroku);
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            btnCancel.Content = Properties.Resources.close_button_text;
            btnCancel.IsDefault = true;
        }


        /// <summary> 各項目のチェック処理(登録)</summary>
        private bool InputCheck()
        {
            // ジョブID
            string jobIdForChange = Properties.Resources.err_message_job_id;
            String jobId = txtJobId.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { jobIdForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(jobId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobIdForChange, "32" });
                return false;
            }
            // 半角英数値、「-」、「_」チェック
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { jobIdForChange });
                return false;
            }
            // 予約語（START）チェック
            if (CheckUtil.IsHoldStrSTART(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_001);
                return false;
            }
            // すでに登録済みの場合
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                foreach (DataRow row in rowJob)
                {
                    if (!jobId.Equals(_oldJobId) && jobId.Equals(row["job_id"]))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                            new string[] { jobIdForChange });
                        return false;
                    }
                }
            }

            // ジョブ名
            string jobNameForChange = Properties.Resources.err_message_job_name;
            String jobName = txtJobName.Text;
            // バイト数チェック
            if (CheckUtil.IsLenOver(jobName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobNameForChange, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(jobName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { jobNameForChange });
                return false;
            }


            // セッションID
            if (rbOnetime.IsChecked == false)
            {
                string sessionIDForChange = Properties.Resources.err_message_session_id;
                string sessionID = Convert.ToString(txtSessionID.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(sessionID))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { sessionIDForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(sessionID, 64))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { sessionIDForChange, "64" });
                    return false;
                }
                // 半角英数値、「-」、「_」チェック
                if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(sessionID))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                        new string[] { sessionIDForChange });
                    return false;
                }
            }


            // ホスト（変数名）
            if (rbHostName.IsEnabled == true)
            {
                if (rbVariableName.IsChecked == true)
                {
                    string hostValueNameForChange = Properties.Resources.err_message_host_value_name;
                    string hostValueName = Convert.ToString(textVariableName.Text);
                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(hostValueName))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { hostValueNameForChange });
                        return false;
                    }
                    // バイト数チェック
                    if (CheckUtil.IsLenOver(hostValueName, 128))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { hostValueNameForChange, "128" });
                        return false;
                    }
                    // 半角英数値、アンダーバー、最初文字数値以外チェック
                    if (!CheckUtil.IsHankakuStrAndUnderbarAndFirstNotNum(hostValueName))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_015,
                            new string[] { hostValueNameForChange });
                        return false;
                    }
                }
            }
            // ホスト名
            if (rbHostName.IsEnabled == true)
            {
                if (rbHostName.IsChecked == true)
                {
                    string hostNameForChange = Properties.Resources.err_message_host_name;
                    string hostName = Convert.ToString(combHostName.SelectedValue);
                    if (CheckUtil.IsNullOrEmpty(hostName))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { hostNameForChange });
                        return false;
                    }
                }
            }

            // ユーザー名
            if (txtUserName.IsEnabled == true)
            {
                string userNameForChange = Properties.Resources.err_message_user_name;
                String userName = txtUserName.Text;
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(userName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { userNameForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(userName, 256))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { userNameForChange, "256" });
                    return false;
                }

            }
            // パスワード
            if (txtPassword.IsEnabled == true)
            {
                if (combAuthentic.SelectedIndex == 0)
                {
                    string passWordForChange = Properties.Resources.err_message_password;
                    String passWord = txtPassword.Text;
                    // String passWord = txtPassword.Password;
                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(passWord))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { passWordForChange });
                        return false;
                    }
                    // バイト数チェック
                    if (CheckUtil.IsLenOver(passWord, 256))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { passWordForChange, "256" });
                        return false;
                    }
                }
            }

            // 認証方法が公開鍵の場合
            if (combAuthentic.SelectedIndex == 1)
            {
                // 公開鍵
                if (txtPublicKey.IsEnabled == true)
                {
                    string publicKeyForChange = Properties.Resources.err_message_publickey;
                    String publicKey = txtPublicKey.Text;
                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(publicKey))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { publicKeyForChange });
                        return false;
                    }
                    // バイト数チェック
                    if (CheckUtil.IsLenOver(publicKey, 2048))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { publicKeyForChange, "2048" });
                        return false;
                    }
                    // ASCII文字チェック
                    if (!CheckUtil.IsASCIIStr(publicKey))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
                            new string[] { publicKeyForChange });
                        return false;
                    }
                }

                // 秘密鍵
                if (txtPrivateKey.IsEnabled == true)
                {
                    string privateKeyForChange = Properties.Resources.err_message_privatekey;
                    String privateKey = txtPrivateKey.Text;
                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(privateKey))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { privateKeyForChange });
                        return false;
                    }
                    // バイト数チェック
                    if (CheckUtil.IsLenOver(privateKey, 2048))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { privateKeyForChange, "2048" });
                        return false;
                    }
                    // ASCII文字チェック
                    if (!CheckUtil.IsASCIIStr(privateKey))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
                            new string[] { privateKeyForChange });
                        return false;
                    }
                }
                // パスフレーズ
                if (txtPassPhrase.IsEnabled == true)
                {
                    string passPhraseForChange = Properties.Resources.err_message_passphrase;
                    string passPhrase = txtPassPhrase.Text;
                    // string passPhrase = txtPassPhrase.Password;
                    // 未入力の場合
                    //                if (CheckUtil.IsNullOrEmpty(passPhrase))
                    //                {
                    //                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    //                        new string[] { passPhraseForChange });
                    //                    return false;
                    //                }

                    // バイト数チェック
                    if (CheckUtil.IsLenOver(passPhrase, 256))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { passPhraseForChange, "256" });
                        return false;
                    }
                }
            }


            // 実行欄
            if (txtCmd.IsEnabled == true)
            {
                string scriptCmdForChange = Properties.Resources.err_message_exec;
                string scriptCmd = txtCmd.Text;
                // バイト数チェック
                if (CheckUtil.IsLenOver(scriptCmd, 4000))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { scriptCmdForChange, "4000" });
                    return false;
                }
            }
            // プロンプト文字列
            if (txtPromptString.IsEnabled == true)
            {
                string promptForChange = Properties.Resources.err_message_prompt;
                string prompt = txtPromptString.Text;
                // 未入力の場合
                //            if (CheckUtil.IsNullOrEmpty(prompt))
                //            {
                //                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                //                    new string[] { promptForChange });
                //                return false;
                //            }
                // 桁数チェック
                if (CheckUtil.IsLenOver(prompt, 256))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { promptForChange, "256" });
                    return false;
                }
            }

            // 文字コード
            if (combCharcode.IsEnabled == true)
            {
                // 入力項目が既登録かチェック
                int cnt = CombDatacombCharcode.Rows.Count;
                //int existFlg = -1;
                //existFlg = combCharcode.SelectedIndex;

                // 大文字に変換
                combCharcode.Text = combCharcode.Text.Trim().ToUpper();
                string inChaCode = combCharcode.Text;
                bool extFlg = false;

                for (int idx = 0; idx < cnt; idx++)
                {
                    if (inChaCode == CombDatacombCharcode.Rows[idx]["ITEM"].ToString())
                    {
                        extFlg = true;
                        break;
                    }
                }
                // 新規であれば入力チェック
                if (extFlg == false)
                {
                    string charcodeForChange = Properties.Resources.err_message_charcode;

                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(inChaCode))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { charcodeForChange });
                        return false;
                    }
                    // バイト数チェック
                    if (CheckUtil.IsLenOver(inChaCode, 80))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                            new string[] { charcodeForChange, "80" });
                        return false;
                    }
                    // ASCII文字チェック
                    if (!CheckUtil.IsASCIIStr(inChaCode))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
                            new string[] { charcodeForChange });
                        return false;
                    }
                }
            }


            // タイムアウト警告
            if (txtTimeOut.IsEnabled == true)
            {
                string timeOutForChange = Properties.Resources.err_message_timeout;
                string timeOut = Convert.ToString(txtTimeOut.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(timeOut))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { timeOutForChange });
                    return false;
                }
                // 半角数字チェック
                if (!CheckUtil.IsHankakuNum(timeOut))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                        new string[] { timeOutForChange });
                    return false;
                }
                // 桁数チェック
                if (CheckUtil.IsLenOver(timeOut, 5))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { timeOutForChange, "5" });
                    return false;
                }
            }
            // ジョブ停止コード
            if (txtStopCode.IsEnabled == true)
            {
                string stopCodeForChange = Properties.Resources.err_message_stop_code;
                string stopCode = Convert.ToString(txtStopCode.Text);
                // 半角数字、カンマ、ハイフンチェック
                if (!CheckUtil.IsHankakuNumAndCommaAndHyphen(stopCode))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_008,
                        new string[] { stopCodeForChange });
                    return false;
                }
                // カンマ、およびハイフンの前後が数字チェック
                if (!CheckUtil.IsHankakuNumBeforeOrAfterCommaAndHyphen(stopCode))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_009,
                        new string[] { stopCodeForChange });
                    return false;
                }
                // 桁数チェック
                if (CheckUtil.IsLenOver(stopCode, 32))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { stopCodeForChange, "32" });
                    return false;
                }
            }

            return true;
        }


        #endregion

    }
}
