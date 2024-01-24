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
using jp.co.ftf.jobcontroller.Common;
using System.Collections.Generic;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.DAO;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 旭 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// InfSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class InfSetting : Window
    {
        #region フィールド

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>カレンダー管理のDAO</summary>
        private CalendarControlDAO _calendarControlDAO;

        /// <summary>カレンダー管理テーブル</summary>
        private DataTable _calendarTable;

        #endregion

        #region コンストラクタ
        public InfSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            _calendarControlDAO = new CalendarControlDAO(dbAccess);

            SetValues(jobId);

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

            // 情報取得アイコン設定テーブルの更新
            DataRow[] rowInfo = _myJob.Container.IconInfoTable.Select("job_id='" + _oldJobId + "'");
            if (rowInfo != null && rowInfo.Length > 0)
            {
                rowInfo[0]["job_id"] = newJobId;
                Int16 infoFlag = Convert.ToInt16(combInfo.SelectedValue);
                rowInfo[0]["info_flag"] = infoFlag;
                if (infoFlag == 0)
                {
                    rowInfo[0]["get_job_id"] = txtJobIdInfo.Text;
                    rowInfo[0]["get_calendar_id"] = Convert.DBNull;
                }
                else if (infoFlag == 3)
                {
                    rowInfo[0]["get_calendar_id"] = combCalendarId.SelectedValue;
                    rowInfo[0]["get_job_id"] = Convert.DBNull;
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

        /// <summary>情報種別を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void combMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Convert.ToInt16(combInfo.SelectedValue) == 0)
            {
                txtJobIdInfo.IsEnabled = true;
                combCalendarId.IsEnabled = false;
            }
            else if (Convert.ToInt16(combInfo.SelectedValue) == 3)
            {
                txtJobIdInfo.IsEnabled = false;
                combCalendarId.IsEnabled = true;
            }
        }

        /// <summary>カレンダーIDを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void combCalendarId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string calendarId = Convert.ToString(combCalendarId.SelectedValue);
            dbAccess.CreateSqlConnect();
            DataTable dtCalendar = _calendarControlDAO.GetValidORMaxUpdateDateEntityById(calendarId);
            if (dtCalendar != null && dtCalendar.Rows.Count > 0)
            {
                tbCalendarName.Text = Convert.ToString(dtCalendar.Rows[0]["calendar_name"]);
            }
            dbAccess.CloseSqlConnect();
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
            txtJobIdInfo.IsEnabled = false;
            combInfo.IsEnabled = false;
            combCalendarId.IsEnabled = false;

        }
        #endregion

        #region privateメッソド

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues(string jobId)
        {
            // ジョブ管理テーブルのデータを取得
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);
            }

            // 情報種別
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, Properties.Resources.info_type_job_status_text);
            dic.Add(3, Properties.Resources.info_type_running_day_text);
            combInfo.Items.Clear();
            combInfo.ItemsSource = dic;
            combInfo.DisplayMemberPath = "Value";
            combInfo.SelectedValuePath = "Key";

            dbAccess.CreateSqlConnect();

            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                _calendarTable = _calendarControlDAO.GetInfoByUserIdSuper();
            }
            else
            {
                _calendarTable = _calendarControlDAO.GetInfoByUserId(LoginSetting.UserID);
            }

            combCalendarId.ItemsSource = _calendarTable.DefaultView;
            combCalendarId.DisplayMemberPath = Convert.ToString(_calendarTable.Columns["calendar_id"]);
            combCalendarId.SelectedValuePath = Convert.ToString(_calendarTable.Columns["calendar_id"]);

            // 情報取得アイコン設定テーブルのデータを取得
            DataRow[] rowInfo;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowInfo = _myJob.Container.IconInfoTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowInfo = _myJob.Container.IconInfoTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowInfo != null && rowInfo.Length > 0)
            {
                if (!Convert.IsDBNull(rowInfo[0]["info_flag"]))
                {
                    Int16 infoFlag = Convert.ToInt16(rowInfo[0]["info_flag"]);
                    combInfo.SelectedValue = infoFlag;
                    if (infoFlag == 0)
                    {
                        txtJobIdInfo.Text = Convert.ToString(rowInfo[0]["get_job_id"]);
                        txtJobIdInfo.IsEnabled = true;
                        combCalendarId.IsEnabled = false;
                    }
                    else if (infoFlag == 3)
                    {
                        string calendarId = Convert.ToString(rowInfo[0]["get_calendar_id"]);
                        combCalendarId.SelectedValue = calendarId;

                        DataTable dtCalendar = _calendarControlDAO.GetValidORMaxUpdateDateEntityById(calendarId);
                        if (dtCalendar != null && dtCalendar.Rows.Count > 0)
                        {
                            tbCalendarName.Text = Convert.ToString(dtCalendar.Rows[0]["calendar_name"]);
                        }
                        txtJobIdInfo.IsEnabled = false;
                        combCalendarId.IsEnabled = true;
                    }
                }
                else
                {
                    txtJobIdInfo.IsEnabled = false;
                    combCalendarId.IsEnabled = false;
                }
            }
            dbAccess.CloseSqlConnect();
        }

        /// <summary> 各項目のチェック処理</summary>
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
            // 桁数チェック
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

            // ジョブ情報
            if (Convert.ToInt16(combInfo.SelectedValue) == 0)
            {
                string jobInfoForChange = Properties.Resources.err_message_job_info;
                string jobInfo = txtJobIdInfo.Text;
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(jobInfo))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { jobInfoForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(jobInfo, 1024))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { jobInfoForChange, "1024" });
                    return false;
                }
                // 半角英数字とハイフン、アンダーバー、スラッシュチェック
                if (!CheckUtil.IsHankakuStrAndHyphenUnderbarAndSlash(jobInfo))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_016,
                        new string[] { jobInfoForChange });
                    return false;
                }
                // ジョブネット階層がない場合
                if (!jobInfo.Contains("/"))
                {
                    DataRow[] rowJobInfo = _myJob.Container.JobControlTable.Select("job_id='" + jobInfo + "'");
                    if (rowJobInfo == null || rowJobInfo.Length == 0)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_003);
                        return false;
                    }
                }
                else
                {
                    // ジョブネット階層がある場合
                    string[] jobInfojobId = jobInfo.Split(new char[] { '/' });
                    // 呼出元ジョブネットと呼出先ジョブネットの関係を確認
                    DataRow[] rowIconJobnet = _myJob.Container.IconJobnetTable.Select("job_id='" +
                        jobInfojobId[0] + "'");
                    if (rowIconJobnet == null || rowIconJobnet.Length == 0)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_003);
                        return false;
                    }
                    // リンク先ジョブネットID
                    string linkJobnetId = Convert.ToString(rowIconJobnet[0]["link_jobnet_id"]);

                    JobnetControlDAO _jobnetControlDAO = new JobnetControlDAO(dbAccess);
                    IconJobnetDAO _iconJobnetDAO = new IconJobnetDAO(dbAccess);
                    JobControlDAO _jobControlDAO = new JobControlDAO(dbAccess);
                    // ジョブネットID
                    string jobnetId = "";
                    // 更新日
                    string updateDate = "";
                    // 呼出元ジョブネットと呼出先ジョブネットの関係を確認
                    for (int k = 1; k < jobInfojobId.Length ; k++)
                    {
                        DataTable tbJobnetCon = _jobnetControlDAO.GetInfoForJobInfo(linkJobnetId);

                        if (tbJobnetCon == null || tbJobnetCon.Rows.Count == 0)
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_003);
                            return false;
                        }
                        else
                        {
                            jobnetId = Convert.ToString(tbJobnetCon.Rows[0]["jobnet_id"]);
                            updateDate = Convert.ToString(tbJobnetCon.Rows[0]["update_date"]);
                            DataTable tbJobControl = _jobControlDAO.GetEntityByPk(jobnetId, jobInfojobId[k], updateDate);
                            if (tbJobControl == null || tbJobControl.Rows.Count == 0)
                            {
                                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_003);
                                return false;
                            }
                            else
                            {
                                DataTable tbIconJobnet = _iconJobnetDAO.GetEntityByPk(jobnetId, jobInfojobId[k], updateDate);
                                if (tbIconJobnet != null && tbIconJobnet.Rows.Count > 0)
                                {
                                    linkJobnetId = Convert.ToString(tbIconJobnet.Rows[0]["link_jobnet_id"]);
                                }
                            }
                        }
                    }
                }
            }
            else if (Convert.ToInt16(combInfo.SelectedValue) == 3)
            {
                // カレンダーID
                string calendarIdForChange = Properties.Resources.err_message_calendar_id;
                String calendarId = Convert.ToString(combCalendarId.SelectedValue);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(calendarId))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { calendarIdForChange });
                    return false;
                }

                // カレンダーIDの存在チェック
                dbAccess.CreateSqlConnect();
                string count = _calendarControlDAO.GetCountByCalendarId(calendarId);
                if ("0".Equals(count))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_CALENDAR_003);
                    return false;
                }
                dbAccess.CloseSqlConnect();
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
