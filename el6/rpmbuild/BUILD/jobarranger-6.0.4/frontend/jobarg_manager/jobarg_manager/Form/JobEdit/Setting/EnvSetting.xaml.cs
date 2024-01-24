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
using System.Windows.Input;
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
    /// EnvSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class EnvSetting : Window
    {
        #region フィールド
        /// <summary>Tabキーかのフラグ</summary>
        private bool _isTabKey;

        #endregion

        #region コンストラクタ
        public EnvSetting(IRoom room, string jobId)
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

        /// <summary>ジョブコントローラ変数アイコン設定テーブル</summary>
        private DataTable _iconvalueTable;
        public DataTable IconvalueTable
        {
            get
            {
                return _iconvalueTable;
            }
            set
            {
                _iconvalueTable = value;
            }
        }

        /// <summary>表示用テーブル</summary>
        private DataTable _gridViewTable;
        public DataTable GridViewTable
        {
            get
            {
                return _gridViewTable;
            }
            set
            {
                _gridViewTable = value;
            }
        }
        #endregion

        #region イベント
        /// <summary> 登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            Toroku();
        }

        /// <summary>削除をクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            string selectValue = Convert.ToString(dgJobConVariable.SelectedValue);
            // 表示用
            DataRow[] rows = _gridViewTable.Select("value_name='" + selectValue + "'");
            if (rows.Length > 0)
            {
                rows[0].Delete();
                dgJobConVariable.ItemsSource = _gridViewTable.DefaultView;
            }
        }

        /// <summary>追加をクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // 入力チェック
            if (!InputCheckAdd())
            {
                return;
            }

            // 更新
            DataRow[] rowIconValue = _gridViewTable.Select("value_name='" + txtValueName.Text + "'");
            if (rowIconValue != null && rowIconValue.Length > 0)
            {
                // 登録用
                //rowIconValue[0]["value"] = txtValue.Text;
                // 表示用
                _gridViewTable.Select("value_name='" + txtValueName.Text + "'")
                    [0]["value"] = txtValue.Text;
            }
            else
            {
                // 表示用
                DataRow rowIndict = _gridViewTable.NewRow();
                rowIndict["jobnet_id"] = _myJob.Container.JobnetId;
                rowIndict["job_id"] = _oldJobId;
                rowIndict["update_date"] = _myJob.Container.TmpUpdDate;
                rowIndict["value_name"] = txtValueName.Text;
                rowIndict["value"] = txtValue.Text;
                _gridViewTable.Rows.Add(rowIndict);
            }
            dgJobConVariable.ItemsSource = _gridViewTable.DefaultView;
            dgJobConVariable.SelectedItem = null;

            // クリア
            txtValueName.Text = "";
            txtValue.Text = "";
        }

        /// <summary>Deleteキー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void dgJobConVariable_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //選択されている場合
            if (dgJobConVariable.SelectedItems.Count > 0)
            {
                string selectValueName = Convert.ToString(dgJobConVariable.SelectedValue);
                string selectValue = (String)_gridViewTable.Select("value_name='" + selectValueName + "'")[0]["value"];
                txtValueName.Text = selectValueName;
                txtValue.Text = selectValue;
            }
            //選択がない場合
            else
            {
                txtValueName.Text = "";
                txtValue.Text = "";
            }
        }

        /// <summary>Deleteキー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void dgJobConVariable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                string selectValue = Convert.ToString(dgJobConVariable.SelectedValue);
                // 表示用
                _gridViewTable.Select("value_name='" + selectValue + "'")[0].Delete();
                dgJobConVariable.ItemsSource = _gridViewTable.DefaultView;
            }
            e.Handled = true;
        }

        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dgJobConVariable.SelectedItems.Count < 1)
            {
                dgJobConVariable.SelectedItem = dgJobConVariable.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    System.Windows.Controls.DataGridRow dgrow = (System.Windows.Controls.DataGridRow)dgJobConVariable.ItemContainerGenerator.ContainerFromItem(dgJobConVariable.Items[dgJobConVariable.SelectedIndex]);
                    System.Windows.Controls.DataGridCell dgc = dgJobConVariable.Columns[0].GetCellContent(dgrow).Parent as System.Windows.Controls.DataGridCell;
                    FocusManager.SetFocusedElement(dgJobConVariable, dgc as IInputElement);
                }
            }
            _isTabKey = false;
        }

        /// <summary>Windowマウスアップ処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isTabKey = false;
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
            txtValue.IsEnabled = false;
            txtValueName.IsEnabled = false;
            btnAdd.IsEnabled = false;
            btnDel.IsEnabled = false;
            dgJobConVariable.Columns[0].CellStyle = Application.Current.Resources["DisableDataGridCell"] as Style;
            dgJobConVariable.Columns[1].CellStyle = Application.Current.Resources["DisableDataGridCell"] as Style;
            KeyboardNavigation.SetTabNavigation(dgJobConVariable, KeyboardNavigationMode.None);

        }
        #endregion

        #region protected　override メッソド
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            _isTabKey = false;

            if (((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return))) && dgJobConVariable.IsKeyboardFocusWithin)
            {
                Toroku();
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Tab)
                _isTabKey = true;

            base.OnPreviewKeyDown(e);
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

            // ジョブコントローラ変数アイコン設定テーブルのデータを取得
            _iconvalueTable = _myJob.Container.IconValueTable;
            DataRow[] rowValue;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowValue = _iconvalueTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowValue = _iconvalueTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowValue != null && rowValue.Length > 0)
            {
                DataView dv = rowValue.CopyToDataTable().DefaultView;
                dv.Sort = "value_name ASC";
                _gridViewTable = dv.ToTable();
                dgJobConVariable.ItemsSource = dv;
                dgJobConVariable.SelectedValuePath = "value_name";
            }
            else
            {
                _gridViewTable = _iconvalueTable.Clone();
                _gridViewTable.Clear();
                dgJobConVariable.ItemsSource = _gridViewTable.DefaultView;
                dgJobConVariable.SelectedValuePath = "value_name";
            }
        }

        /// <summary> 各項目のチェック処理(追加)</summary>
        private bool InputCheckAdd()
        {
            // 変数名
            string valueNameForChange = Properties.Resources.err_message_jobcontrol_value_name;
            String valueName = txtValueName.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(valueName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { valueNameForChange });
                return false;
            }
            // 半角英数値、アンダーバー、最初文字数値以外チェック
            if (!CheckUtil.IsHankakuStrAndUnderbarAndFirstNotNum(valueName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_015,
                    new string[] { valueNameForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(valueName, 128))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { valueNameForChange, "128" });
                return false;
            }

            // 値
            string valueForChange = Properties.Resources.err_message_jobcontrol_value;
            String value = txtValue.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(value))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { valueForChange });
                return false;
            }
            //// ASCII文字チェック
            //if (!CheckUtil.IsASCIIStr(value))
            //{
            //    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
            //        new string[] { valueForChange });
            //    return false;
            //}
            // バイト数チェック
            if (CheckUtil.IsLenOver(value, 4000))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { valueForChange, "4000" });
                return false;
            }

            return true;
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

            // ジョブコントローラ変数
            string jonConValueForChange = Properties.Resources.err_message_jobcontrol_variable;
            if (_gridViewTable == null || _gridViewTable.Rows.Count == 0)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                   new string[] { jonConValueForChange });
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
        // <summary>登録処理</summary>
        private void Toroku()
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
            DataRow[] drJobCon = _myJob.Container.JobControlTable.Select("job_id='" + _oldJobId + "'");
            if (drJobCon != null && drJobCon.Length > 0)
            {
                drJobCon[0]["job_id"] = newJobId;
                drJobCon[0]["job_name"] = newJobNm;
            }

            // ジョブコントローラ変数アイコン設定テーブルの更新
            DataRow[] rows = _iconvalueTable.Select("job_id='" + OldJobId + "'");
            Array.ForEach<DataRow>(rows, row => _iconvalueTable.Rows.Remove(row));
            DataRow[] rowViews = _gridViewTable.Select("job_id='" + OldJobId + "'");
            if (rowViews != null && rowViews.Length > 0)
            {
                foreach (DataRow rowView in rowViews)
                {
                    DataRow row = _iconvalueTable.NewRow();
                    row["jobnet_id"] = rowView["jobnet_id"];
                    row["job_id"] = newJobId;
                    row["update_date"] = rowView["update_date"];
                    row["value_name"] = rowView["value_name"];
                    row["value"] = rowView["value"];
                    _iconvalueTable.Rows.Add(row);
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
        #endregion
    }
}
