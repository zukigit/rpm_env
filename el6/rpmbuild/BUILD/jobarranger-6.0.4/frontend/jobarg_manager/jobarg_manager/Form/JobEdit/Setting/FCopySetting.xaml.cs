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
using System.Windows;
using System.Data;
using System;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Controls;
using System.Collections.Generic;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/05 新規作成<BR>                           *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// JobSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class FCopySetting : Window
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

        #region コンストラクタ

        public FCopySetting(IRoom room, string jobId)
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

        public FCopySetting(IRoom room, string jobId, Consts.EditType editType)
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
        #endregion

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

        #region イベント

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

            // ファイル転送アイコン設定テーブルの更新
            DataRow[] rowIconFcopy = _myJob.Container.IconFcopyTable.Select("job_id='" + _oldJobId + "'");
            if (rowIconFcopy != null && rowIconFcopy.Length > 0)
            {
                // ジョブID
                rowIconFcopy[0]["job_id"] = newJobId;
                // 転送元ホストフラグ
                if (rbHostName.IsChecked == true)
                {
                    rowIconFcopy[0]["from_host_flag"] = "0";
                    // ホスト名
                    rowIconFcopy[0]["from_host_name"] = Convert.ToString(combHostName.SelectedValue);
                }
                else
                {
                    rowIconFcopy[0]["from_host_flag"] = "1";
                    // ホスト名
                    rowIconFcopy[0]["from_host_name"] = textVariableName.Text;
                }
                // 転送先ホストフラグ
                if (rbDestinationHostName.IsChecked == true)
                {
                    rowIconFcopy[0]["to_host_flag"] = "0";
                    // ホスト名
                    rowIconFcopy[0]["to_host_name"] = Convert.ToString(combDestinationHostName.SelectedValue);
                }
                else
                {
                    rowIconFcopy[0]["to_host_flag"] = "1";
                    // ホスト名
                    rowIconFcopy[0]["to_host_name"] = textDestinationVariableName.Text;
                }
                //上書き許可フラグ
                if (CheckBoxOverride.IsChecked == true)
                {
                    rowIconFcopy[0]["overwrite_flag"] = "1";
                }
                else
                {
                    rowIconFcopy[0]["overwrite_flag"] = "0";
                }
                //転送元ディレクトリー
                rowIconFcopy[0]["from_directory"] = textDir.Text;
                //転送元ファイル名
                rowIconFcopy[0]["from_file_name"] = textFileName.Text;
                //転送先ディレクトリー
                rowIconFcopy[0]["to_directory"] = textDestinationDir.Text;

                //added by YAMA 2014/02/19
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

        /// <summary>転送元ホスト名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbHostName_Checked(object sender, RoutedEventArgs e)
        {
            combHostName.IsEnabled = true;
            textVariableName.IsEnabled = false;
        }

        /// <summary>転送元変数名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbVariableName_Checked(object sender, RoutedEventArgs e)
        {
            combHostName.IsEnabled = false;
            textVariableName.IsEnabled = true;
        }

        /// <summary>転送先ホスト名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbToHostName_Checked(object sender, RoutedEventArgs e)
        {
            combDestinationHostName.IsEnabled = true;
            textDestinationVariableName.IsEnabled = false;
        }

        /// <summary>転送先変数名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbToVariableName_Checked(object sender, RoutedEventArgs e)
        {
            combDestinationHostName.IsEnabled = false;
            textDestinationVariableName.IsEnabled = true;
        }


        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_myJob.ItemEditType == Consts.EditType.READ && _myJob.Container.ParantWindow is JobEdit)
                _myJob.ResetInitColor();
        }

        #endregion

        #region publicメッソド
        public void SetDisable()
        {
            txtJobId.IsEnabled = false;
            txtJobName.IsEnabled = false;
            btnToroku.IsEnabled = false;
            combHostName.IsEnabled = false;
            textVariableName.IsEnabled = false;
            rbHostName.IsEnabled = false;
            rbVariableName.IsEnabled = false;
            combDestinationHostName.IsEnabled = false;
            textDestinationVariableName.IsEnabled = false;
            rbDestinationHostName.IsEnabled = false;
            rbDestinationVariableName.IsEnabled = false;
            textDir.IsEnabled = false;
            textFileName.IsEnabled = false;
            textDestinationDir.IsEnabled = false;
            CheckBoxOverride.IsEnabled = false;

            //added by YAMA 2014/02/19
            cbForce.IsEnabled = false;
        }
        #endregion

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

            //転送元ホスト情報
            combHostName.Items.Clear();
            combHostName.ItemsSource = dtHost.DefaultView;
            combHostName.DisplayMemberPath = Convert.ToString(dtHost.Columns["host"]);
            combHostName.SelectedValuePath = Convert.ToString(dtHost.Columns["host"]);

            //転送先ホスト情報
            combDestinationHostName.Items.Clear();
            combDestinationHostName.ItemsSource = dtHost.DefaultView;
            combDestinationHostName.DisplayMemberPath = Convert.ToString(dtHost.Columns["host"]);
            combDestinationHostName.SelectedValuePath = Convert.ToString(dtHost.Columns["host"]);

            // ファイル転送アイコン設定テーブルのデータを取得
            DataRow[] rowIconFcopy;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconFcopy = _myJob.Container.IconFcopyTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconFcopy = _myJob.Container.IconFcopyTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowIconFcopy != null && rowIconFcopy.Length > 0)
            {
                // 転送元ホスト
                string hostFlag = Convert.ToString(rowIconFcopy[0]["from_host_flag"]);
                string hostName = Convert.ToString(rowIconFcopy[0]["from_host_name"]);
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

                // 転送先ホスト
                string toHostFlag = Convert.ToString(rowIconFcopy[0]["to_host_flag"]);
                string toHostName = Convert.ToString(rowIconFcopy[0]["to_host_name"]);
                if ("1".Equals(toHostFlag))
                {
                    rbDestinationVariableName.IsChecked = true;
                    textDestinationVariableName.Text = toHostName;
                }
                else
                {
                    if (editType == Consts.EditType.READ)
                    {
                        DataRow[] rows = dtHost.Select("host='" + toHostName + "'");
                        if (rows.Length < 1)
                        {
                            DataRow row = dtHost.NewRow();
                            row["host"] = toHostName;
                            dtHost.Rows.Add(row);
                        }
                    }
                    rbDestinationHostName.IsChecked = true;
                    combDestinationHostName.SelectedValue = toHostName;
                }
                // 上書きフラグ
                string overwriteFlag = Convert.ToString(rowIconFcopy[0]["overwrite_flag"]);
                if ("1".Equals(overwriteFlag))
                {
                    CheckBoxOverride.IsChecked = true;
                }
                else
                {
                    CheckBoxOverride.IsChecked = false;
                }
                //転送元ディレクトリー
                textDir.Text = Convert.ToString(rowIconFcopy[0]["from_directory"]);
                //転送元ファイル名
                textFileName.Text = Convert.ToString(rowIconFcopy[0]["from_file_name"]);
                //転送先ディレクトリー
                textDestinationDir.Text = Convert.ToString(rowIconFcopy[0]["to_directory"]);

		    	//added by YAMA 2014/02/19
	            // 強制実行
                string forceFlag = Convert.ToString(rowJob[0]["force_flag"]);
	            if ("1".Equals(forceFlag))
	            {
	                cbForce.IsChecked = true;
	            }


            }
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

            // 転送元ホスト（変数名）
            if (rbVariableName.IsChecked == true)
            {
                string fromHostValueNameForChange = Properties.Resources.err_message_from_host_value_name;
                string fromHostValueName = Convert.ToString(textVariableName.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(fromHostValueName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { fromHostValueNameForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(fromHostValueName, 128))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { fromHostValueNameForChange, "128" });
                    return false;
                }
                // 半角英数値、アンダーバー、最初文字数値以外チェック
                if (!CheckUtil.IsHankakuStrAndUnderbarAndFirstNotNum(fromHostValueName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_015,
                        new string[] { fromHostValueNameForChange });
                    return false;
                }
            }
            // 転送元ホスト名
            if (rbHostName.IsChecked == true)
            {
                string fromHostNameForChange = Properties.Resources.err_message_from_host_name;
                string fromHostName = Convert.ToString(combHostName.SelectedValue);
                if (CheckUtil.IsNullOrEmpty(fromHostName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { fromHostNameForChange });
                    return false;
                }
            }

            // 転送先ホスト（変数名）
            if (rbDestinationVariableName.IsChecked == true)
            {
                string toHostValueNameForChange = Properties.Resources.err_message_to_host_value_name;
                string toHostValueName = Convert.ToString(textDestinationVariableName.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(toHostValueName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { toHostValueNameForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(toHostValueName, 128))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { toHostValueNameForChange, "128" });
                    return false;
                }
                // 半角英数値、アンダーバー、最初文字数値以外チェック
                if (!CheckUtil.IsHankakuStrAndUnderbarAndFirstNotNum(toHostValueName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_015,
                        new string[] { toHostValueNameForChange });
                    return false;
                }
            }
            // 転送先ホスト名
            if (rbDestinationHostName.IsChecked == true)
            {
                string toHostNameForChange = Properties.Resources.err_message_to_host_name;
                string toHostName = Convert.ToString(combDestinationHostName.SelectedValue);
                if (CheckUtil.IsNullOrEmpty(toHostName))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { toHostNameForChange });
                    return false;
                }
            }
            //転送元ディレクトリー
            string fromDirForChange = Properties.Resources.err_message_from_dir;
            String fromDir = textDir.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(fromDir))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { fromDirForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(fromDir, 1024))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { fromDirForChange, "1024" });
                return false;
            }

            //転送元ファイル名
            string fromFileNameForChange = Properties.Resources.err_message_from_file_name;
            String fromFileName = textFileName.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(fromFileName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { fromFileNameForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(fromFileName, 1024))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { fromFileNameForChange, "1024" });
                return false;
            }


            //転送先ディレクトリー
            string toDirForChange = Properties.Resources.err_message_to_dir;
            String toDir = textDestinationDir.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(toDir))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { toDirForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(toDir, 1024))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { toDirForChange, "1024" });
                return false;
            }

            return true;
        }

        /// <summary> 詳細画面からの参照時のボタンの切り替え</summary>
        private void ChangeButton4DetailRef()
        {
            GridSetting.Children.Remove(btnToroku);
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            btnCancel.Content = Properties.Resources.close_button_text;
            btnCancel.IsDefault = true;
        }
        #endregion
    }
}
