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
using System.Windows.Media;
using System.Collections.Generic;

//added by YAMA 2014/08/15
using System.Security.Cryptography;
using System.Text;

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
    /// JobSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class JobSetting : Window
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
        // private string _selectForHostSqlSuper = "select hostid, host from hosts where status=0 or status=1 order by hostid ASC";
        private string _selectForHostSqlSuper = "select hostid, host from hosts where status in (0,1) and flags = 0 order by host ASC";

        /// <summary>Tabキーかのフラグ</summary>
        private bool _isTabKey;
        #endregion

        #region コンストラクタ

        public JobSetting(IRoom room, string jobId)
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

        public JobSetting(IRoom room, string jobId, Consts.EditType editType)
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

        /// <summary>ジョブコントローラ変数設定テーブル</summary>
        private DataTable _valueJobConTable;
        public DataTable ValueJobConTable
        {
            get
            {
                return _valueJobConTable;
            }
            set
            {
                _valueJobConTable = value;
            }
        }

        /// <summary>ジョブ変数設定テーブル</summary>
        private DataTable _valueJobTable;
        public DataTable ValueJobTable
        {
            get
            {
                return _valueJobTable;
            }
            set
            {
                _valueJobTable = value;
            }
        }

        /// <summary>ジョブ変数表示用テーブル</summary>
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

        /// <summary>登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            Toroku();
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

        /// <summary>停止コマンドをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void cbStop_Click(object sender, RoutedEventArgs e)
        {
            if (cbStop.IsChecked == true)
            {
                txtStopCmd.IsEnabled = true;
            }
            else
            {
                txtStopCmd.IsEnabled = false;
            }
        }

        /// <summary>削除をクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            string selectValue = Convert.ToString(dgJobValue.SelectedValue);
            // 表示用
            DataRow[] rows = _gridViewTable.Select("value_name='" + selectValue + "'");
            if (rows.Length > 0)
            {
                rows[0].Delete();
                dgJobValue.ItemsSource = _gridViewTable.DefaultView;
            }
        }

        /// <summary>追加ボタン押下</summary>
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
            DataRow[] rowValueJob = _gridViewTable.Select("value_name='" + txtJobValueName.Text + "'");
            if (rowValueJob != null && rowValueJob.Length > 0)
            {
                // 表示用
                _gridViewTable.Select("value_name='" + txtJobValueName.Text + "'")
                    [0]["value"] = txtJobValue.Text;
            }
            else
            {
                // 表示用
                DataRow rowIndict = _gridViewTable.NewRow();
                rowIndict["jobnet_id"] = _myJob.Container.JobnetId;
                rowIndict["job_id"] = _oldJobId;
                rowIndict["update_date"] = _myJob.Container.TmpUpdDate;
                rowIndict["value_name"] = txtJobValueName.Text;
                rowIndict["value"] = txtJobValue.Text;
                _gridViewTable.Rows.Add(rowIndict);
            }
            dgJobValue.ItemsSource = _gridViewTable.DefaultView;
            dgJobValue.SelectedItem = null;

            // クリア
            txtJobValueName.Text = "";
            txtJobValue.Text = "";
        }

        /// <summary>Deleteキー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void dgJobValue_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //選択されている場合
            if (dgJobValue.SelectedItems.Count > 0)
            {
                string selectValueName = Convert.ToString(dgJobValue.SelectedValue);
                string selectValue = (String)_gridViewTable.Select("value_name='" + selectValueName + "'")[0]["value"];
                txtJobValueName.Text = selectValueName;
                txtJobValue.Text = selectValue;
            }
            //選択がない場合
            else
            {
                txtJobValueName.Text = "";
                txtJobValue.Text = "";
            }
        }

        /// <summary>Deleteキー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void dgJobValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                string selectValue = Convert.ToString(dgJobValue.SelectedValue);
                // 表示用
                _gridViewTable.Select("value_name='" + selectValue + "'")[0].Delete();
                dgJobValue.ItemsSource = _gridViewTable.DefaultView;
                e.Handled = true;
            }
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

        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)dgJobValue.Resources["SelectionColorKey"]).Color = SystemColors.HighlightColor;
            if (dgJobValue.SelectedItems.Count < 1)
            {
                dgJobValue.SelectedItem = dgJobValue.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    System.Windows.Controls.DataGridRow dgrow = (System.Windows.Controls.DataGridRow)dgJobValue.ItemContainerGenerator.ContainerFromItem(dgJobValue.Items[dgJobValue.SelectedIndex]);
                    System.Windows.Controls.DataGridCell dgc = dgJobValue.Columns[0].GetCellContent(dgrow).Parent as System.Windows.Controls.DataGridCell;
                    FocusManager.SetFocusedElement(dgJobValue, dgc as IInputElement);
                }
            }
            _isTabKey = false;

        }

        /// <summary>フォーカスロスと時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            //((SolidColorBrush)dgJobValue.Resources["SelectionColorKey"]).Color = Colors.Gray;
        }


        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lstbJobCon.SelectedItems.Count < 1)
            {
                lstbJobCon.SelectedItem = lstbJobCon.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    FocusManager.SetFocusedElement(lstbJobCon, lstbJobCon.SelectedItem as IInputElement);
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

        #endregion

        #region publicメッソド
        public void SetDisable()
        {
            txtJobId.IsEnabled = false;
            txtJobName.IsEnabled = false;
            btnToroku.IsEnabled = false;
            combHostName.IsEnabled = false;
            textVariableName.IsEnabled = false;
            cbStop.IsEnabled = false;
            txtStopCmd.IsEnabled = false;
            txtCmd.IsReadOnly = true;
            txtJobValueName.IsEnabled = false;
            txtJobValue.IsEnabled = false;
            btnAdd.IsEnabled = false;
            btnDel.IsEnabled = false;
            txtTimeOut.IsEnabled = false;
            combRunType.IsEnabled = false;

            //added by YAMA 2014/02/19
            cbForce.IsEnabled = false;

            txtStopCode.IsEnabled = false;
            rbHostName.IsEnabled = false;
            rbVariableName.IsEnabled = false;
            dgJobValue.Columns[0].CellStyle = Application.Current.Resources["DisableDataGridCell"] as Style;
            dgJobValue.Columns[1].CellStyle = Application.Current.Resources["DisableDataGridCell"] as Style;
            KeyboardNavigation.SetTabNavigation(lstbJobCon, KeyboardNavigationMode.None);
            KeyboardNavigation.SetTabNavigation(dgJobValue, KeyboardNavigationMode.None);
            lstbJobCon.ItemContainerStyle = Application.Current.Resources["UnFocusableControlStyle"] as Style;

            //added by YAMA 2014/08/14
            cbContinue.IsEnabled = false;
            txtRunUser.IsEnabled = false;
            txtRunUserPW.IsEnabled = false;

            //added by YAMA 2014/09/24  (ジョブエラー継続)
            cbContinue.IsEnabled = false;
        }
        #endregion

        #region protected　override メッソド
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            _isTabKey = false;
            if (((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return))) && dgJobValue.IsKeyboardFocusWithin)
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
        private void SetValues(string jobId, Consts.EditType editType)
        {
            // ジョブ管理テーブルのデータを取得
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);

                //added by YAMA 2014/08/15
                // 実行ユーザー
                txtRunUser.Text = Convert.ToString(rowJob[0]["run_user"]);
                // 実行ユーザーのパスワード
                //                txtRunUserPW.Text = Convert.ToString(rowJob[0]["run_user_password"]);
                if ((Convert.ToString(rowJob[0]["run_user_password"]).Equals("")))
                {
                    txtRunUserPW.Text = Convert.ToString(rowJob[0]["run_user_password"]);
                }
                else
                {
                    //Park.iggy 修正 START
                    //txtRunUserPW.Text = Decryption(Convert.ToString(rowJob[0]["run_user_password"]));
                    string passwd = Convert.ToString(rowJob[0]["run_user_password"]).Substring(0, 1);
                    if (passwd.CompareTo("1") == 0)
                    {
                        txtRunUserPW.Text = ConvertUtil.getStringFromX16Password(Convert.ToString(rowJob[0]["run_user_password"]));
                    }
                    else
                    {
                        txtRunUserPW.Text = ConvertUtil.getStringFromPassword(Convert.ToString(rowJob[0]["run_user_password"]));
                    }

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

                                txtRunUserPW.Text = "******";
                            }
                        }


                    }
                    else
                    {
                        txtRunUserPW.Text = "******";
                    }


                    //Park.iggy 修正 END


                }
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

            combHostName.Items.Clear();
            combHostName.ItemsSource = dtHost.DefaultView;
            combHostName.DisplayMemberPath = Convert.ToString(dtHost.Columns["host"]);
            combHostName.SelectedValuePath = Convert.ToString(dtHost.Columns["host"]);

            // ジョブアイコン設定テーブルのデータを取得
            DataRow[] rowIconJob;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconJob = _myJob.Container.IconJobTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconJob = _myJob.Container.IconJobTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowIconJob != null && rowIconJob.Length > 0)
            {
                // ホスト
                string hostFlag = Convert.ToString(rowIconJob[0]["host_flag"]);
                string hostName = Convert.ToString(rowIconJob[0]["host_name"]);
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

                //added by YAMA 2014/11/12
                // 停止コマンドの取得
                DataRow[] rowCmd;
                if (_myJob.ContentItem.InnerJobId == null)
                {
                    rowCmd = _myJob.Container.JobCommandTable.Select("job_id='" + jobId + "'"
                            + " and command_cls='2'");
                }
                else
                {
                    rowCmd = _myJob.Container.JobCommandTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId + " and command_cls='2'");
                }

                if (rowCmd != null && rowCmd.Length > 0)
                {
                    txtStopCmd.Text = Convert.ToString(rowCmd[0]["command"]);
                }
                // 停止コマンド
                string stopFlag = Convert.ToString(rowIconJob[0]["stop_flag"]);
                if ("1".Equals(stopFlag))
                {
                    cbStop.IsChecked = true;
                    txtStopCmd.IsEnabled = true;
                    //added by YAMA 2014/11/12
                    /*
                    // 停止コマンドの取得
                    DataRow[] rowCmd;
                    if (_myJob.ContentItem.InnerJobId == null)
                    {
                        rowCmd = _myJob.Container.JobCommandTable.Select("job_id='" + jobId + "'"
                                + " and command_cls='2'");
                    }
                    else
                    {
                        rowCmd = _myJob.Container.JobCommandTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId + " and command_cls='2'");
                    }

                    if (rowCmd != null && rowCmd.Length > 0)
                    {
                        txtStopCmd.Text = Convert.ToString(rowCmd[0]["command"]);
                    }
                    */
                }
                else
                {
                    cbStop.IsChecked = false;
                    txtStopCmd.IsEnabled = false;
                }

                // 実行
                // コマンドの取得
                DataRow[] rowCommand;
                if (_myJob.ContentItem.InnerJobId == null)
                {
                    rowCommand = _myJob.Container.JobCommandTable.Select("job_id='" + jobId + "'"
                    + " and command_cls='0'");
                }
                else
                {
                    rowCommand = _myJob.Container.JobCommandTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId
                    + " and command_cls='0'");
                }
                if (rowCommand != null && rowCommand.Length > 0)
                {
                    txtCmd.Text = Convert.ToString(rowCommand[0]["command"]);
                }

                // タイムアウト警告
                txtTimeOut.Text = Convert.ToString(rowIconJob[0]["timeout"]);

                // タイムアウト実行タイプ
                String runType = Convert.ToString(rowIconJob[0]["timeout_run_type"]);
                if ("".Equals(runType))
                {
                    combRunType.SelectedIndex = 0;
                }else{
                    combRunType.SelectedIndex = Convert.ToInt32(runType);
                }


                //added by YAMA 2014/02/19
                // 強制実行
                string forceFlag = Convert.ToString(rowJob[0]["force_flag"]);
                if ("1".Equals(forceFlag))
                {
                    cbForce.IsChecked = true;
                }

                // ジョブ停止コード
                txtStopCode.Text = Convert.ToString(rowIconJob[0]["stop_code"]);

                //added by YAMA 2014/09/24  (ジョブエラー継続)
                // 処理継続
                string ContinueFlag = Convert.ToString(rowJob[0]["continue_flag"]);
                if ("1".Equals(ContinueFlag))
                {
                    cbContinue.IsChecked = true;
                }
            }

            // ジョブコントローラ変数定義テーブルのデータを取得
            DefineValueDAO _defineValueDAO = new DefineValueDAO(dbAccess);
            DataTable dtDefineValue = _defineValueDAO.GetEntityByNone();
            dbAccess.CloseSqlConnect();
            // ジョブコントローラ変数名
            string valueNameForIndict = "";
            if (dtDefineValue != null & dtDefineValue.Rows.Count > 0)
            {
                for (int i = 0; i < dtDefineValue.Rows.Count; i++)
                {
                    valueNameForIndict = Convert.ToString(dtDefineValue.Rows[i]["value_name"]);
                    valueNameForIndict = valueNameForIndict.Replace("_", "__");

                    CheckBox cbDefineValue = new CheckBox();
                    cbDefineValue.Content = valueNameForIndict;
                    cbDefineValue.IsChecked = false;
                    cbDefineValue.Margin = new Thickness(0, 3, 0, 3);
                    cbDefineValue.IsTabStop = false;

                    if (_myJob.ItemEditType == Consts.EditType.READ)
                    {
                        cbDefineValue.IsHitTestVisible = false;
                    }
                    lstbJobCon.Items.Add(cbDefineValue);
                }

            }

            // ジョブコントローラ変数設定テーブルのデータを取得
            _valueJobConTable = _myJob.Container.ValueJobConTable;
            DataRow[] rowValueJobCon;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowValueJobCon = _valueJobConTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowValueJobCon = _valueJobConTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            // ジョブコントローラ変数名
            string valueName = "";
            if (rowValueJobCon != null && rowValueJobCon.Length > 0)
            {
                DataView dvSort = rowValueJobCon.CopyToDataTable().DefaultView;
                dvSort.Sort = "value_name ASC";
                DataTable dtSort = dvSort.ToTable();

                for (int j = 0; j < dtSort.Rows.Count; j++)
                {
                    foreach (CheckBox item in lstbJobCon.Items)
                    {
                        valueName = Convert.ToString(dtSort.Rows[j]["value_name"]);
                        valueName = valueName.Replace("_", "__");

                        if (item.Content.Equals(valueName))
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            }

            // ジョブ変数設定テーブルのデータを取得
            _valueJobTable = _myJob.Container.ValueJobTable;
            DataRow[] rowValue;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowValue = _valueJobTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowValue = _valueJobTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }
            if (rowValue != null && rowValue.Length > 0)
            {
                DataView dv = rowValue.CopyToDataTable().DefaultView;
                dv.Sort = "value_name ASC";
                _gridViewTable = dv.ToTable();
                dgJobValue.ItemsSource = dv;
                dgJobValue.SelectedValuePath = "value_name";
            }
            else
            {
                _gridViewTable = _valueJobTable.Clone();
                _gridViewTable.Clear();
                dgJobValue.ItemsSource = _gridViewTable.DefaultView;
                dgJobValue.SelectedValuePath = "value_name";
            }
        }

        /// <summary> 各項目のチェック処理(追加)</summary>
        private bool InputCheckAdd()
        {
            // 変数名
            string valueNameForChange = Properties.Resources.err_message_job_value_name;
            String valueName = txtJobValueName.Text;
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
            string valueForChange = Properties.Resources.err_message_job_value;
            String value = txtJobValue.Text;
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

            // 停止コマンド
            if (cbStop.IsChecked == true)
            {
                string stopCmdForChange = Properties.Resources.err_message_stop_command;
                string stopCmd = Convert.ToString(txtStopCmd.Text);
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(stopCmd))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { stopCmdForChange });
                    return false;
                }
                // ASCII文字チェック
                if (!CheckUtil.IsASCIIStr(stopCmd))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_002,
                        new string[] { stopCmdForChange });
                    return false;
                }
                // バイト数チェック
                if (CheckUtil.IsLenOver(stopCmd, 4000))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { stopCmdForChange, "4000" });
                    return false;
                }
            }

            // 実行欄
            string scriptCmdForChange = Properties.Resources.err_message_exec;
            string scriptCmd = "";
            scriptCmd = txtCmd.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(scriptCmd))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { scriptCmdForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(scriptCmd, 4000))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { scriptCmdForChange, "4000" });
                return false;
            }

            // タイムアウト警告
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

            // ジョブ停止コード
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

            //added by YAMA 2014/08/15
            // 実行ユーザー（日本語入力可、省略可）
            string runUserForChange = Properties.Resources.err_message_run_user;
            String runUser = txtRunUser.Text;
            bool runUserFlg = false;

            // 入力時は以下をチェック
            if (CheckUtil.IsNullOrEmpty(runUser) == false)
            {
                // バイト数チェック
                if (CheckUtil.IsLenOver(runUser, 256))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { runUserForChange, "256" });
                    return false;
                }

                // 禁則文字チェック
                if (!CheckUtil.IsProhibitedCharacterUserName(runUser))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_014,
                        new string[] { runUserForChange });
                    return false;
                }

                runUserFlg = true;

            }

            // パスワード（省略可、パスワードのみは不可）
            string runUserPWForChange = Properties.Resources.err_message_run_user_pw;
            String runUserPW = txtRunUserPW.Text;

            // 入力時は以下をチェック
            if (CheckUtil.IsNullOrEmpty(runUserPW) == false)
            {
                // 実行ユーザーが未設定
                if (runUserFlg == false)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { runUserForChange });
                    return false;
                }

                // 全角文字チェック
                if (CheckUtil.isHankaku(runUserPW) == false)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_015,
                        new string[] { runUserForChange });
                    return false;
                }

                // バイト数チェック
                if (CheckUtil.IsLenOver(runUserPW, 256))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                        new string[] { runUserPWForChange, "256" });
                    return false;
                }

                // 禁則文字チェック
                if (!CheckUtil.IsProhibitedCharacterUserPW(runUserPW))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_015,
                        new string[] { runUserForChange });
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

        /// <summary>登録処理</summary>
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

            //added by YAMA 2014/08/15
            // 実行ユーザー
            string newRunUser = txtRunUser.Text;
            // 実行ユーザーのパスワード
            string newRunUserPW = txtRunUserPW.Text;

            // ジョブ管理テーブルの更新
            DataRow[] rowJobCon = _myJob.Container.JobControlTable.Select("job_id='" + _oldJobId + "'");
            if (rowJobCon != null && rowJobCon.Length > 0)
            {
                rowJobCon[0]["job_id"] = newJobId;
                rowJobCon[0]["job_name"] = newJobNm;

                if (newRunUser.Equals(""))
                {
                    rowJobCon[0]["run_user"] = null;
                    rowJobCon[0]["run_user_password"] = null;
                }
                else if (newRunUserPW.Equals(""))
                {
                    //added by YAMA 2014/09/26
                    rowJobCon[0]["run_user"] = newRunUser;

                    rowJobCon[0]["run_user_password"] = null;
                }
                else
                {
                    rowJobCon[0]["run_user"] = newRunUser;
                    rowJobCon[0]["run_user_password"] = ConvertUtil.getPasswordFromString(newRunUserPW);
                }

            }

            // ジョブアイコン設定テーブルの更新
            DataRow[] rowIconJob = _myJob.Container.IconJobTable.Select("job_id='" + _oldJobId + "'");
            if (rowIconJob != null && rowIconJob.Length > 0)
            {
                // ジョブID
                rowIconJob[0]["job_id"] = newJobId;
                // ホストフラグ
                if (rbHostName.IsChecked == true)
                {
                    rowIconJob[0]["host_flag"] = "0";
                    // ホスト名
                    rowIconJob[0]["host_name"] = Convert.ToString(combHostName.SelectedValue);
                }
                else
                {
                    rowIconJob[0]["host_flag"] = "1";
                    // ホスト名
                    rowIconJob[0]["host_name"] = textVariableName.Text;
                }
                // 停止コマンドフラグ
                if (cbStop.IsChecked == false)
                {
                    rowIconJob[0]["stop_flag"] = "0";
                }
                else
                {
                    rowIconJob[0]["stop_flag"] = "1";
                }
                // コマンドタイプ
                rowIconJob[0]["command_type"] = "0";

                // タイムアウト警告
                if (!CheckUtil.IsNullOrEmpty(txtTimeOut.Text))
                {
                    rowIconJob[0]["timeout"] = txtTimeOut.Text;
                }
                else
                {
                    rowIconJob[0]["timeout"] = Convert.DBNull;
                }
                // タイムアウトの場合実行内容
                // 0 警告のみ
                // 1 job stop
                // 2 job skip
                rowIconJob[0]["timeout_run_type"] = combRunType.SelectedIndex;

                // ジョブ停止コード
                rowIconJob[0]["stop_code"] = txtStopCode.Text;

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

                //added by YAMA 2014/09/24  (ジョブエラー継続)
                // 処理継続
                if (cbContinue.IsChecked == false)
                {
                    rowJobCon[0]["continue_flag"] = "0";    // アイコン停止（初期値）
                }
                else
                {
                    rowJobCon[0]["continue_flag"] = "1";    // 処理継続
                }
            }

            // ジョブコマンド設定テーブルの削除
            DataRow[] rowScriptCmd = _myJob.Container.JobCommandTable.Select("job_id='" + _oldJobId + "'");
            if (rowScriptCmd != null && rowScriptCmd.Length > 0)
            {
                for (int i = 0; i < rowScriptCmd.Length; i++)
                {
                    rowScriptCmd[i].Delete();
                }
            }

            // ジョブコマンド設定テーブルの登録
            // 停止コマンド
            //added by YAMA 2014/11/12
            //if (cbStop.IsChecked == true)
            string stopCmd = Convert.ToString(txtStopCmd.Text);
            if (!CheckUtil.IsNullOrEmpty(stopCmd))
            {
                // 停止コマンドの登録
                DataRow rowAdd = _myJob.Container.JobCommandTable.NewRow();
                rowAdd["jobnet_id"] = _myJob.Container.JobnetId;
                rowAdd["job_id"] = newJobId;
                rowAdd["update_date"] = _myJob.Container.TmpUpdDate;
                rowAdd["command_cls"] = 2;
                rowAdd["command"] = txtStopCmd.Text;
                _myJob.Container.JobCommandTable.Rows.Add(rowAdd);
            }

            // スクリプト、コマンドの登録
            DataRow rowScriptCmdAdd = _myJob.Container.JobCommandTable.NewRow();
            rowScriptCmdAdd["jobnet_id"] = _myJob.Container.JobnetId;
            rowScriptCmdAdd["job_id"] = newJobId;
            rowScriptCmdAdd["update_date"] = _myJob.Container.TmpUpdDate;

            rowScriptCmdAdd["command_cls"] = 0;
            rowScriptCmdAdd["command"] = txtCmd.Text;

            _myJob.Container.JobCommandTable.Rows.Add(rowScriptCmdAdd);


            // ジョブ変数設定テーブルの更新
            DataRow[] rows = _valueJobTable.Select("job_id='" + OldJobId + "'");
            Array.ForEach<DataRow>(rows, row => _valueJobTable.Rows.Remove(row));
            DataRow[] rowViews = _gridViewTable.Select("job_id='" + _oldJobId + "'");

            if (rowViews != null && rowViews.Length > 0)
            {
                foreach (DataRow rowView in rowViews)
                {
                    DataRow rowAdd = _valueJobTable.NewRow();
                    rowAdd["jobnet_id"] = rowView["jobnet_id"];
                    rowAdd["job_id"] = newJobId;
                    rowAdd["update_date"] = rowView["update_date"];
                    rowAdd["value_name"] = rowView["value_name"];
                    rowAdd["value"] = rowView["value"];
                    _valueJobTable.Rows.Add(rowAdd);
                }
            }

            // ジョブコントローラ変数設定テーブルの更新
            DataRow[] rowValueJobCon = _valueJobConTable.Select("job_id='" + _oldJobId + "'");
            // 既存データの削除
            Array.ForEach<DataRow>(rowValueJobCon, row => _valueJobConTable.Rows.Remove(row));

            // 入力データの登録
            foreach (CheckBox item in lstbJobCon.Items)
            {
                if (item.IsChecked == true)
                {
                    // ジョブコントローラ変数名
                    string valueName = Convert.ToString(item.Content);
                    valueName = valueName.Replace("__", "_");

                    DataRow rowAdd = _valueJobConTable.NewRow();
                    rowAdd["jobnet_id"] = _myJob.Container.JobnetId;
                    rowAdd["job_id"] = newJobId;
                    rowAdd["value_name"] = valueName;
                    rowAdd["update_date"] = _myJob.Container.TmpUpdDate;
                    _valueJobConTable.Rows.Add(rowAdd);
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
