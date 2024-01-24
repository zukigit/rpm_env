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
using System.Collections.Generic;
using jp.co.ftf.jobcontroller.Common;
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
    /// IfSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class IfSetting : Window
    {
        #region コンストラクタ
        public IfSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

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

        /// <summary> 登録処理</summary>
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
            DataRow[] drJobCon = _myJob.Container.JobControlTable.Select("job_id='" + OldJobId + "'");
            if (drJobCon != null && drJobCon.Length > 0)
            {
                drJobCon[0]["job_id"] = newJobId;
                drJobCon[0]["job_name"] = newJobNm;
            }

            // 条件分岐アイコン設定テーブルの更新
            DataRow[] drIf = _myJob.Container.IconIfTable.Select("job_id='" + OldJobId + "'");
            if (drIf != null && drIf.Length > 0)
            {
                drIf[0]["job_id"] = newJobId;
                drIf[0]["hand_flag"] = Convert.ToInt16(combMethod.SelectedValue);
                drIf[0]["value_name"] = txtVariableName.Text;
                drIf[0]["comparison_value"] = txtCompareValue.Text;
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
            txtCompareValue.IsEnabled = false;
            txtVariableName.IsEnabled = false;
            combMethod.IsEnabled = false;

        }
        #endregion

        #region privateメッソド

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues(string jobId)
        {
            // 当該ジョブのデータを取得
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);
            }

            if (ElementType.START == _myJob.ElementType)
            {
                txtJobId.IsEnabled = false;
            }

            // 処理方法
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, Properties.Resources.numerical_value_text);
            dic.Add(1, Properties.Resources.character_string_text);
            combMethod.Items.Clear();
            combMethod.ItemsSource = dic;
            combMethod.DisplayMemberPath = "Value";
            combMethod.SelectedValuePath = "Key";

            // 条件分岐アイコン設定テーブルのデータを取得
            DataRow[] rowIf;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIf = _myJob.Container.IconIfTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIf = _myJob.Container.IconIfTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowIf != null && rowIf.Length > 0)
            {
                txtVariableName.Text = Convert.ToString(rowIf[0]["value_name"]);
                if (!Convert.IsDBNull(rowIf[0]["hand_flag"]))
                {
                    combMethod.SelectedValue = Convert.ToInt16(rowIf[0]["hand_flag"]);
                }
                txtCompareValue.Text = Convert.ToString(rowIf[0]["comparison_value"]);
            }
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

            // 変数名
            string variableForChange = Properties.Resources.err_message_value_name;
            string variable = Convert.ToString(txtVariableName.Text);
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(variable))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { variableForChange });
                return false;
            }
            // 半角英数値、アンダーバー、最初文字数値以外チェック
            if (!CheckUtil.IsHankakuStrAndUnderbarAndFirstNotNum(variable))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_015,
                    new string[] { variableForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(variable, 128))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { variableForChange, "128" });
                return false;
            }

            // 比較値
            string compareForChange = Properties.Resources.err_message_compare_value;
            string compare = Convert.ToString(txtCompareValue.Text);
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(compare))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { compareForChange });
                return false;
            }
            // 処理方法が数値
            if ("0".Equals(Convert.ToString(combMethod.SelectedValue)))
            {
                // 半角数字、およびカンマ（,）とハイフン（-）チェック
                if (!CheckUtil.IsHankakuNumAndCommaAndHyphen(compare))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_008,
                        new string[] { compareForChange });
                    return false;
                }
                // 比較値のカンマ（,）、およびハイフン（-）の前後が数字チェック
                if (!CheckUtil.IsHankakuNumBeforeOrAfterCommaAndHyphen(compare))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_009,
                        new string[] { compareForChange });
                    return false;
                }
            }
            else
            {
                //// ASCII文字チェック
                //if (!CheckUtil.IsASCIIStr(compare))
                //{
                //    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
                //        new string[] { compareForChange });
                //    return false;
                //}
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(compare, 4000))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { compareForChange, "4000" });
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
