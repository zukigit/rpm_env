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
using System.Collections;
using System.Collections.Generic;
using jp.co.ftf.jobcontroller.Common;
using System.Windows.Input;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/14 新規作成<BR>                            *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// EnvSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class ParameterSetting : Window
    {
        #region フィールド
        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect _dbAccess = new DBConnect(LoginSetting.ConnectStr);

        //変数変更情報
        private List<String[]> updateInfos = new List<String[]>();

        private DataRowView _dataRow;

        /// <summary>Tabキーかのフラグ</summary>
        private bool _isTabKey;
        #endregion

        #region コンストラクタ
        public ParameterSetting(IRoom room)
        {
            InitializeComponent();
            dgVariable.RowHeight = double.NaN;
            _dbAccess.CreateSqlConnect();

            _myJob = room;
            _jobId = room.ContentItem.InnerJobId;
            FillTables();

            SetValues();

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
        private string _jobId;
        public string JobId
        {
            get
            {
                return _jobId;
            }
            set
            {
                _jobId = value;
            }
        }


        /// <summary>変数テーブル</summary>
        private DataTable _valueTable;
        public DataTable ValueTable
        {
            get
            {
                return _valueTable;
            }
            set
            {
                _valueTable = value;
            }
        }
        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>編集が完了</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void DataGrid_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        {
            _dataRow = e.Row.Item as DataRowView;
            TextBox t = e.EditingElement as TextBox;

            // 値
            string valueForChange = Properties.Resources.err_message_job_value;
            String value = t.Text;
            // バイト数チェック
            if (CheckUtil.IsLenOver(value, 4000))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { valueForChange, "4000" });
                t.Text = _dataRow["value"].ToString();
                return;
            }
            _dataRow.EndEdit();
            DataRow row = _dataRow.Row;
            String tableName = (String)row["table_name"];
            String columnName = "value";
            if (tableName.Equals("ja_run_value_before_table")) columnName = "before_value";

            string[] updateData = new string[5];

            updateData[0] = tableName;
            updateData[1] = columnName;
            updateData[2] = value;
            updateData[3] = _jobId;
            updateData[4] = row["value_name"].ToString();


            updateInfos.Add(updateData);
        }

        //*******************************************************************
        /// <summary>編集時、Shift+Enterキーが押された場合改行追加</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Return == e.Key &&
                0 < (ModifierKeys.Shift & e.KeyboardDevice.Modifiers))
            {
                var tb = (TextBox)sender;
                var caret = tb.CaretIndex;
                tb.Text = tb.Text.Insert(caret, Environment.NewLine);
                tb.CaretIndex = caret + 1;
                e.Handled = true;
            }
        }


        /// <summary> 登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            Touroku();
        }

        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dgVariable.SelectedItems.Count < 1)
            {
                dgVariable.SelectedItem = dgVariable.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    System.Windows.Controls.DataGridRow dgrow = (System.Windows.Controls.DataGridRow)dgVariable.ItemContainerGenerator.ContainerFromItem(dgVariable.Items[dgVariable.SelectedIndex]);
                    System.Windows.Controls.DataGridCell dgc = dgVariable.Columns[0].GetCellContent(dgrow).Parent as System.Windows.Controls.DataGridCell;
                    FocusManager.SetFocusedElement(dgVariable, dgc as IInputElement);
                }
            }
            _isTabKey = false;
        }

        private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isTabKey = false;
        }
        #endregion

        #region protected　override メッソド
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            bool isEdit = false;
            _isTabKey = false;
            System.Data.DataRowView selectedRow = null;
            if (dgVariable.SelectedItems.Count == 1)
            {
                selectedRow = (System.Data.DataRowView)dgVariable.SelectedItem;
                isEdit = selectedRow.IsEdit;
            }
            if (((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return))) && !isEdit)
            {
                Touroku();
                e.Handled = true;
                return;
            }
            if (e.Key.Equals(Key.Escape) && !isEdit)
            {
                Cancel();
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Tab)
                _isTabKey = true;

            base.OnPreviewKeyDown(e);
        }
        #endregion

        #region privateメッソド
        //*******************************************************************
        /// <summary> ジョブネットデータの検索（編集、コピー新規用）</summary>
        //*******************************************************************
        private void FillTables()
        {
            //added by YAMA 2014/10/08 add [order by value_name]

            string strSql = "(select 'ja_run_value_before_table' as table_name, BV.inner_jobnet_id, BV.value_name, BV.before_value as value from ja_run_value_before_table BV " +
                                "where BV.inner_job_id = " + _jobId + " " +
                                "and BV.value_name not in " +
                                "(select JV1.value_name from ja_run_value_job_table JV1 " +
                                "where JV1.inner_job_id = " + _jobId + ")) " +
                                "union " +
                                "(select 'ja_run_value_job_table' as table_name, JV2.inner_jobnet_id, JV2.value_name, JV2.value from ja_run_value_job_table JV2 " +
                                "where JV2.inner_job_id = " + _jobId + ") order by value_name";

            _valueTable = _dbAccess.ExecuteQuery(strSql);
        }

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues()
        {
            dgVariable.ItemsSource = _valueTable.DefaultView;
            lblJobId.Content = _myJob.ContentItem.JobId;
            lblJobName.Content = _myJob.ContentItem.JobName;
        }

        //*******************************************************************
        /// <summary> 登録処理</summary>
        //*******************************************************************
        private void Touroku()
        {
            _dbAccess.BeginTransaction();
            String sql;
            foreach(string[] updateDate in updateInfos)
            {
                sql = "update " + updateDate[0] + " set " + updateDate[1] + "=? where inner_job_id=? and value_name=?";
                List<ComSqlParam> updateSqlParams = new List<ComSqlParam>();
                updateSqlParams.Add(new ComSqlParam(DbType.String, "@" + updateDate[1], updateDate[2]));
                updateSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", updateDate[3]));
                updateSqlParams.Add(new ComSqlParam(DbType.String, "@value_name", updateDate[4]));
                _dbAccess.ExecuteNonQuery(sql, updateSqlParams);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();
            this.Close();
        }

        //*******************************************************************
        /// <summary> キャンセル処理</summary>
        //*******************************************************************
        private void Cancel()
        {
            _dbAccess.CloseSqlConnect();
            this.Close();
        }
        #endregion
    }
}
