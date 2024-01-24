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
    /// FWaitSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class FWaitSetting : Window
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

        public FWaitSetting(IRoom room, string jobId)
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
        public FWaitSetting(IRoom room, string jobId, Consts.EditType editType)
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

        /// <summary>ファイル存在チェックを選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void existCheck_Checked(object sender, RoutedEventArgs e)
        {
            txtWaitTime.IsEnabled = false;
        }

        /// <summary>待ち合わせリブートを選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void wait_Checked(object sender, RoutedEventArgs e)
        {
            txtWaitTime.IsEnabled = true;
        }

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

            // ファイル待ち合わせアイコン設定テーブルの更新
            DataRow[] rowIconFwait = _myJob.Container.IconFwaitTable.Select("job_id='" + _oldJobId + "'");
            if (rowIconFwait != null && rowIconFwait.Length > 0)
            {
                // ジョブID
                rowIconFwait[0]["job_id"] = newJobId;
                // ホストフラグ
                if (rbHostName.IsChecked == true)
                {
                    rowIconFwait[0]["host_flag"] = "0";
                    // ホスト名
                    rowIconFwait[0]["host_name"] = Convert.ToString(combHostName.SelectedValue);
                }
                else
                {
                    rowIconFwait[0]["host_flag"] = "1";
                    // ホスト名
                    rowIconFwait[0]["host_name"] = textVariableName.Text;
                }
                //ファイル名
                rowIconFwait[0]["file_name"] = textFileName.Text;

                //ファイル削除フラグ
                if (CheckBoxDelete.IsChecked == true)
                {
                    rowIconFwait[0]["file_delete_flag"] = "1";
                }
                else
                {
                    rowIconFwait[0]["file_delete_flag"] = "0";
                }

                // モード
                if (rbFileWait.IsChecked == true)
                {
                    rowIconFwait[0]["fwait_mode_flag"] = "0";
                    // 待ち合わせ時間
                    rowIconFwait[0]["file_wait_time"] = txtWaitTime.Text;
                }
                else
                {
                    rowIconFwait[0]["fwait_mode_flag"] = "1";
                    // 待ち合わせ時間
                    rowIconFwait[0]["file_wait_time"] = "0";
                }

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

                if (cbContinue.IsChecked == false)
                {
                    rowJobCon[0]["continue_flag"] = "0";
                }
                else
                {
                    rowJobCon[0]["continue_flag"] = "1";    // 処理継続
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
            textFileName.IsEnabled = false;
            rbFileWait.IsEnabled = false;
            rbFileCheck.IsEnabled = false;
            CheckBoxDelete.IsEnabled = false;
            txtWaitTime.IsEnabled = false;

            //added by YAMA 2014/02/19
            cbForce.IsEnabled = false;
            cbContinue.IsEnabled = false;
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

            //ホスト情報
            combHostName.Items.Clear();
            combHostName.ItemsSource = dtHost.DefaultView;
            combHostName.DisplayMemberPath = Convert.ToString(dtHost.Columns["host"]);
            combHostName.SelectedValuePath = Convert.ToString(dtHost.Columns["host"]);

            // ファイル待ち合わせアイコン設定テーブルのデータを取得
            DataRow[] rowIconFwait;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconFwait = _myJob.Container.IconFwaitTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconFwait = _myJob.Container.IconFwaitTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowIconFwait != null && rowIconFwait.Length > 0)
            {
                // ホスト
                string hostFlag = Convert.ToString(rowIconFwait[0]["host_flag"]);
                string hostName = Convert.ToString(rowIconFwait[0]["host_name"]);
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

                //ファイル名
                textFileName.Text = Convert.ToString(rowIconFwait[0]["file_name"]);

                // モード
                string waitMode = Convert.ToString(rowIconFwait[0]["fwait_mode_flag"]);
                string waitTime = Convert.ToString(rowIconFwait[0]["file_wait_time"]);
                txtWaitTime.Text = waitTime;
                if ("0".Equals(waitMode))
                {
                    rbFileWait.IsChecked = true;
                    txtWaitTime.IsEnabled = true;
                }
                else
                {
                    rbFileCheck.IsChecked = true;
                    txtWaitTime.IsEnabled = false;
                }

                // ファイル削除
                string deleteFile = Convert.ToString(rowIconFwait[0]["file_delete_flag"]);
                if ("0".Equals(deleteFile))
                {
                    CheckBoxDelete.IsChecked = false;
                }
                else
                {
                    CheckBoxDelete.IsChecked = true;
                }

                //added by YAMA 2014/02/19
                // 強制実行
                string forceFlag = Convert.ToString(rowJob[0]["force_flag"]);
                if ("1".Equals(forceFlag))
                {
                    cbForce.IsChecked = true;
                }

                string ContinueFlag = Convert.ToString(rowJob[0]["continue_flag"]);
                if ("1".Equals(ContinueFlag))
                {
                    cbContinue.IsChecked = true;
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

            // ホスト（変数名）
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
            // ホスト名
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


            //ファイル名
            string fileNameForChange = Properties.Resources.err_message_file_name;
            String fileName = textFileName.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(fileName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { fileNameForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(fileName, 1024))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { fileNameForChange, "1024" });
                return false;
            }

            // リブートモード
            if (rbFileWait.IsChecked == true)
            {
                string waitTimeForChange = Properties.Resources.err_message_wait_time;
                string waitTime = Convert.ToString(txtWaitTime.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(waitTime))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { waitTimeForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(waitTime, 4))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { waitTimeForChange, "4" });
                    return false;
                }
                // 半角数値以外チェック
                if (!CheckUtil.IsHankakuNum(waitTime))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                        new string[] { waitTimeForChange });
                    return false;
                }
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
