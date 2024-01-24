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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.Collections;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/04 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
/// <summary>
/// Container.xaml の相互作用ロジック
/// </summary>
    public partial class Container : UserControl
    {
        #region フィールド
        /// <summary>削除起動時刻</summary>
        private DataRow[] deleteCalendarArray;

        /// <summary>削除ジョブネット</summary>
        private DataRow[] deleteJobnetArray;

        /// <summary>Tabキーかのフラグ</summary>
        private bool _isTabKey;

        #endregion

        #region コンストラクタ
        public Container()
        {
            // 初期化
            InitializeComponent();
        }
        #endregion

        #region プロパティ

        /// <summary>編集起動時刻行データ</summary>
        DataRowView _dataRow;
        public DataRowView DataRow
        {
            get
            {
                return _dataRow;
            }
            set
            {
                _dataRow = value;
            }
        }

        /// <summary>ウィンドウ</summary>
        ContentControl _parantWindow;
        public ContentControl ParantWindow
        {
            get
            {
                return _parantWindow;
            }
            set
            {
                _parantWindow = value;
            }
        }


        /// <summary> 選択コントローラリスト</summary>
        List<System.Windows.Controls.Control> _currentSelectedControlCollection;
        public List<Control> CurrentSelectedControlCollection
        {
            get
            {
                if (_currentSelectedControlCollection == null)
                    _currentSelectedControlCollection = new List<Control>();
                return _currentSelectedControlCollection;
            }
        }

        /// <summary>ジョブネットID</summary>
        private string _calendarId;
        public string ScheduleId
        {
            get
            {
                return _calendarId;
            }
            set
            {
                _calendarId = value;
            }
        }

        /// <summary>更新日</summary>
        private string _updDate;
        public string UpdDate
        {
            get
            {
                return _updDate;
            }
            set
            {
                _updDate = value;
            }
        }

        /// <summary>仮更新日</summary>
        private string _tmpUpdDate;
        public string TmpUpdDate
        {
            get
            {
                return _tmpUpdDate;
            }
            set
            {
                _tmpUpdDate = value;
            }
        }
        /// <summary>ドラッグのジョブネットID</summary>
        private string _dragScheduleId;
        public string DragScheduleId
        {
            get
            {
                return _dragScheduleId;
            }
            set
            {
                _dragScheduleId = value;
            }
        }

        /// <summary>シフトキー押下状態フラグ</summary>
        public bool ShiftKeyIsPress
        {
            get
            {
                return (Keyboard.Modifiers == ModifierKeys.Shift);
            }
        }

        #endregion

        #region データ格納場所

        /// <summary>スケジュール管理テーブル</summary>
        public DataTable ScheduleControlTable { get; set; }

        /// <summary>スケジュール起動時刻テーブル</summary>
        public DataTable ScheduleDetailTable { get; set; }

        /// <summary>スケジュールジョブネットテーブル</summary>
        public DataTable ScheduleJobnetTable { get; set; }

        #endregion

        #region イベント

        //*******************************************************************
        /// <summary>コンテナを右クリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ContextMenu.Visibility = Visibility.Visible;
        }

        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCalendar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dgCalendarBootTime.Items.Count <= 0) return;
            if (dgCalendarBootTime.SelectedItems.Count < 1)
            {
                dgCalendarBootTime.SelectedItem = dgCalendarBootTime.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    System.Windows.Controls.DataGridRow dgrow = (System.Windows.Controls.DataGridRow)dgCalendarBootTime.ItemContainerGenerator.ContainerFromItem(dgCalendarBootTime.Items[dgCalendarBootTime.SelectedIndex]);
                    System.Windows.Controls.DataGridCell dgc = dgCalendarBootTime.Columns[0].GetCellContent(dgrow).Parent as System.Windows.Controls.DataGridCell;
                    FocusManager.SetFocusedElement(dgCalendarBootTime, dgc as IInputElement);
                    _isTabKey = false;
                }
            }
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendar"]).Color = SystemColors.HighlightColor;
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendarText"]).Color = SystemColors.HighlightTextColor;
        }

        private void dgCalendar_LostFocus(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendar"]).Color = Colors.LightGray;
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendarText"]).Color = Colors.Black;
        }

        private void dgCalendar_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendar"]).Color = Colors.LightGray;
            ((SolidColorBrush)dgCalendarBootTime.Resources["SelectionColorKeyCalendarText"]).Color = Colors.Black;
        }

        private void dgCalendar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isTabKey = false;
        }

        /// <summary>フォーカス取得時処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgJobnet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (dgJobnet.Items.Count <= 0) return;
            if (dgJobnet.SelectedItems.Count < 1)
            {
                dgJobnet.SelectedItem = dgJobnet.Items[0];
            }
            else
            {
                if (_isTabKey)
                {
                    System.Windows.Controls.DataGridRow dgrow = (System.Windows.Controls.DataGridRow)dgJobnet.ItemContainerGenerator.ContainerFromItem(dgJobnet.Items[dgJobnet.SelectedIndex]);
                    System.Windows.Controls.DataGridCell dgc = dgJobnet.Columns[0].GetCellContent(dgrow).Parent as System.Windows.Controls.DataGridCell;
                    FocusManager.SetFocusedElement(dgJobnet, dgc as IInputElement);
                    _isTabKey = false;
                }
            }
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnet"]).Color = SystemColors.HighlightColor;
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnetText"]).Color = SystemColors.HighlightTextColor;
        }

        private void dgJobnet_LostFocus(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnet"]).Color = Colors.LightGray;
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnetText"]).Color = Colors.Black;
        }

        private void dgJobnet_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnet"]).Color = Colors.LightGray;
            ((SolidColorBrush)dgJobnet.Resources["SelectionColorKeyJobnetText"]).Color = Colors.Black;
        }

        private void dgJobnet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isTabKey = false;
        }

        //*******************************************************************
        /// <summary>カレンダーが選択された場合</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void select_deleteCalendar(object sender, EventArgs e)
        {
            if (dgCalendarBootTime.SelectedItems.Count > 0)
            {
                deleteCalendarArray = new DataRow[dgCalendarBootTime.SelectedItems.Count];
                for (int i = 0; i < dgCalendarBootTime.SelectedItems.Count; i++)
                {
                    System.Data.DataRowView selected = (System.Data.DataRowView)dgCalendarBootTime.SelectedItems[i];
                    deleteCalendarArray[i] = selected.Row;
                }
            }

        }

        //*******************************************************************
        /// <summary>ジョブネットが選択された場合</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void select_deleteJobnet(object sender, EventArgs e)
        {
            if (dgJobnet.SelectedItems.Count > 0)
            {
                deleteJobnetArray = new DataRow[dgJobnet.SelectedItems.Count];
                for (int i = 0; i < dgJobnet.SelectedItems.Count; i++)
                {
                    System.Data.DataRowView selected = (System.Data.DataRowView)dgJobnet.SelectedItems[i];
                    deleteJobnetArray[i] = selected.Row;
                }
            }
        }

        //*******************************************************************
        /// <summary>カレンダー追加ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void addCalendar_Click(object sender, EventArgs e)
        {
            ScheduleCalendarRegistWindow scheduleCalendarRegistWindow = new ScheduleCalendarRegistWindow(this);
            scheduleCalendarRegistWindow.Owner = ((ScheduleEdit)ParantWindow).ParantWindow;
            scheduleCalendarRegistWindow.ShowDialog();
        }

        //*******************************************************************
        /// <summary>カレンダー削除ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void delCalendar_Click(object sender, EventArgs e)
        {
            if (deleteCalendarArray != null)
            {
                foreach (DataRow row in deleteCalendarArray)
                {
                    row.Delete();
                }
            }
        }

        //*******************************************************************
        /// <summary>ジョブネット追加ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void addJobnet_Click(object sender, EventArgs e)
        {
            ScheduleJobnetRegistWindow scheduleJobnetRegistWindow = new ScheduleJobnetRegistWindow(this);
            scheduleJobnetRegistWindow.Owner = ((ScheduleEdit)ParantWindow).ParantWindow;
            scheduleJobnetRegistWindow.ShowDialog();

        }

        //*******************************************************************
        /// <summary>ジョブネット削除ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void delJobnet_Click(object sender, EventArgs e)
        {
            if (deleteJobnetArray != null)
            {
                foreach (DataRow row in deleteJobnetArray)
                {
                    row.Delete();
                }
            }
        }

        //*******************************************************************
        /// <summary>編集が完了</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void DataGrid_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        {
            _dataRow = e.Row.Item as DataRowView;
            TextBox t = e.EditingElement as TextBox;
            DataRow row = _dataRow.Row;
            DataRow[] exitRows = ScheduleDetailTable.Select("calendar_id='" + row["calendar_id"] + "' and boot_time='" + t.Text + "'");

            if (exitRows.Length > 0 && !t.Text.Equals(_dataRow["boot_time"].ToString()))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_BOOT_TIME_003);
                t.Text = _dataRow["boot_time"].ToString();
                return;
            }
            if (!InputCheck(t.Text))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_BOOT_TIME_004);
                t.Text = _dataRow["boot_time"].ToString();
                return;
            }
            if (t.Text.Length == 3)
                t.Text = "0" + t.Text;

            _dataRow.EndEdit();
            _dataRow = null;

        }

        #endregion

        #region protected　override メッソド
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            _isTabKey = false;
            bool isEdit = false;
            System.Data.DataRowView selectedRow = null;
            if (dgCalendarBootTime.SelectedItems.Count == 1)
            {
                selectedRow = (System.Data.DataRowView)dgCalendarBootTime.SelectedItem;
                isEdit = selectedRow.IsEdit;
            }
            if (((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return))) && !isEdit)
            {
                ((ScheduleEdit)ParantWindow).regist();
                e.Handled = true;
                return;
            }
            if (e.Key.Equals(Key.Escape) && !isEdit)
            {
                ((ScheduleEdit)ParantWindow).cancel();
                e.Handled = true;
                return;
            }

            if (e.Key.Equals(Key.Delete) && dgCalendarBootTime.IsKeyboardFocusWithin && !isEdit)
            {
                if (deleteCalendarArray != null && deleteCalendarArray.Length > 0)
                {
                    int nextSelectRow = GetNextSelectRow(dgCalendarBootTime);
                    object item=null;
                    if(nextSelectRow>=0)
                        item = dgCalendarBootTime.Items[nextSelectRow];

                    foreach (DataRow row in deleteCalendarArray)
                    {
                        row.Delete();
                    }
                    dgCalendarBootTime.SelectedItem = item;
                    Keyboard.Focus(dgCalendarBootTime);
                    dgCalendarBootTime.CurrentCell = new DataGridCellInfo(item, dgCalendarBootTime.Columns[0]);
                }
                e.Handled = true;
                return;
            }
            if (e.Key.Equals(Key.Delete) && dgJobnet.IsKeyboardFocusWithin)
            {
                if (deleteJobnetArray != null && deleteJobnetArray.Length > 0)
                {
                    int nextSelectRow = GetNextSelectRow(dgJobnet);
                    object item=null;
                    if(nextSelectRow>=0)
                        item=dgJobnet.Items[nextSelectRow];

                    foreach (DataRow row in deleteJobnetArray)
                    {
                        row.Delete();
                    }
                    dgJobnet.SelectedItem = item;
                    Keyboard.Focus(dgJobnet);
                    dgJobnet.CurrentCell = new DataGridCellInfo(item, dgJobnet.Columns[0]);
                }
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Tab)
                _isTabKey = true;
            base.OnPreviewKeyDown(e);
        }
        #endregion

        #region publicメッソド
        //*******************************************************************
        /// <summary>選択モジュールを追加</summary>
        /// <param name="uc">部品</param>
        //*******************************************************************
        public void AddSelectedControl(System.Windows.Controls.Control uc)
        {
            if (!CurrentSelectedControlCollection.Contains(uc))
            {
                _currentSelectedControlCollection.Add(uc);
            }

        }

        /// <summary>Disable処理</summary>
        public void SetDisable()
        {
            calendarAdd.IsEnabled = false;
            calendarDel.IsEnabled = false;
            jobnetAdd.IsEnabled = false;
            jobnetDel.IsEnabled = false;
            dgCalendarBootTime.IsReadOnly = true;
            KeyboardNavigation.SetTabNavigation(dgCalendarBootTime, KeyboardNavigationMode.None);
            dgJobnet.IsReadOnly = true;
            KeyboardNavigation.SetTabNavigation(dgJobnet, KeyboardNavigationMode.None);
            dgCalendarBootTime.Columns[0].CellStyle = Application.Current.Resources["DisableVCenterDataListCellStyle"] as Style;
            dgCalendarBootTime.Columns[1].CellStyle = Application.Current.Resources["DisableVCenterDataListCellStyle"] as Style;
            dgJobnet.Columns[0].CellStyle = Application.Current.Resources["DisableIDDataListCellStyle"] as Style;
            dgJobnet.Columns[1].CellStyle = Application.Current.Resources["DisableVCenterDataListCellStyle"] as Style;
            dgCalendarBootTime.SelectionChanged -= select_deleteCalendar;
            dgJobnet.SelectionChanged -= select_deleteJobnet;

        }

        #endregion

        #region privateメッソド

        //*******************************************************************
        /// <summary>起動時刻編集入力チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheck(String bootTime)
        {

            if (CheckUtil.IsNullOrEmpty(bootTime))
            {
                return false;
            }
            if (CheckUtil.IsLenOver(bootTime, 4))
            {
                return false;
            }
            if (CheckUtil.IsLenUnder(bootTime, 3))
            {
                return false;
            }
            if (!CheckUtil.IsHankakuNum(bootTime))
            {
                return false;
            }

            // 起動時刻分を取得
            string bootTimeMI;
            if (bootTime.Length > 3)
            {
                bootTimeMI = bootTime.Substring(2, 2);
            }
            else
            {
                bootTimeMI = bootTime.Substring(1, 2);
            }

            int iMI = int.Parse(bootTimeMI);

            if (iMI < 0 || iMI > 59)
            {
                return false;
            }

            return true;
        }
        private int GetNextSelectRow(DataGrid Target)
        {
            int[] selectedIndex = new int[Target.SelectedItems.Count];
            for(int i=0;i<Target.SelectedItems.Count;i++)
            {
                selectedIndex[i] = (Target.ItemContainerGenerator.ContainerFromItem(Target.SelectedItems[i]) as DataGridRow).GetIndex();
            }
            for (int i = 0; i < Target.Items.Count; i++)
            {
                if (selectedIndex.Contains(i))
                    continue;
                for (int j = 0; j < selectedIndex.Length; j++)
                {
                    if (i > selectedIndex[j])
                    {
                        return i;
                    }
                }
            }
            if (Target.SelectedItems.Count > 0)
            {
                return selectedIndex[0] - 1;
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }

}
