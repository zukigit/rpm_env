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
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.JobController;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.JobController.Form.CalendarEdit;
using jp.co.ftf.jobcontroller.JobController.Form.FilterEdit;
using jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
using jp.co.ftf.jobcontroller.JobController.Form.JobManager;
using jp.co.ftf.jobcontroller.JobController.Form.JobResult;
using System.Configuration;
using jp.co.ftf.jobcontroller.DAO;
using System.Data;
using System;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections;
using System.Collections.Generic;
//added by YAMA 2014/08/18
using jp.co.ftf.jobcontroller.JobController.Form.SetParameter;
//Park.iggy
using System.Diagnostics;
using System.Windows.Navigation;




//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// JobArrangerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class JobArrangerWindow : BaseWindow
    {
        #region フィールド

        /// <summary>編集画面</summary>
        private EditBaseUserControl _objectEdit = null;

        /// <summary>オブジェクト一覧画面</summary>
        private ObjectList _objList = null;

        /// <summary>実行管理画面</summary>
        private JobnetExecControlPage _execControl = null;

        /// <summary>実行結果画面</summary>
        private JobnetExecResultPage _execResult = null;

        //added by YAMA 2014/08/18
        /// <summary>一般設定画面</summary>
        private SetParameterPage _setParameter = null;

        /// <summary>Tab1のタイトル</summary>
        private String tab1Title = null;

        /// <summary>編集画面表示中のJobnet</summary>
        public Hashtable viewJobEdit = new Hashtable();

        /// <summary>ジョブネット編集画面コピー用SaveJobControlRows</summary>
        public List<DataRow> SaveJobControlRows = new List<DataRow>();

        /// <summary>ジョブネット編集画面コピー用SaveSetedJobIds</summary>
        public Hashtable SaveSetedJobIds = new Hashtable();

        /// <summary>ジョブネット編集画面コピー用SaveItems</summary>
        public Hashtable SaveItems = new Hashtable();

        /// <summary>ジョブネット編集画面コピー用SaveFlows</summary>
        public DataTable SaveFlows = new DataTable();

        /// <summary>ジョブネット編集画面コピー用SaveMinX</summary>
        public double SaveMinX = 0;

        /// <summary>ジョブネット編集画面コピー用SaveMinY</summary>
        public double SaveMinY = 0;

        /// <summary>オブジェクトALL種別</summary>
        /// Park.iggy Add
        private Boolean _objectAllFlag = false;
        public Boolean ObjectAllFlag
        {
            get
            {
                return _objectAllFlag;
            }
            set
            {
                _objectAllFlag = value;
            }
        }
        /// <summary>オブジェクトList フラグ</summary>
        /// Park.iggy Add
        private Boolean _objectListFlag;
        public Boolean ObjectListFlag
        {
            get
            {
                return _objectListFlag;
            }
            set
            {
                _objectListFlag = value;
            }
        }



        #endregion

        #region コンストラクタ
        public JobArrangerWindow()
        {
            InitializeComponent();
            this.tabControl.SelectedIndex = 0;
            this.Title = this.Title + " - " + LoginSetting.JobconName;
            #if VIEWER
                MenuItemEdit.IsEnabled = false;
                MenuItemRun.IsEnabled = false;
            #endif
        }
        #endregion

        #region プロパティ
        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "JobArrangerWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_200;
            }
        }

        /// <summary>編集画面オブジェクト</summary>
        public EditBaseUserControl ObjectEdit
        {
            get
            {
                return _objectEdit;
            }
            set
            {
                _objectEdit = value;
            }
        }
        #endregion

        #region イベント

        //*******************************************************************
        /// <summary>画面ロード</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void mainWindow_Load(object sender, RoutedEventArgs e)
        {
            userName.Text = LoginSetting.UserName;
            _objList = new ObjectList();
            _objList.DadWindow = this;

            this.Title = MessageUtil.GetMsgById(_objList.GamenId) + " - " + LoginSetting.JobconName;
            treeViewCalendar.IsSelected = true;

            JobNetGrid.Children.Add(_objList);
        }

        //*******************************************************************
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("Window_Closed", Consts.PROCESS_011);

            MessageBoxResult result = CommonDialog.ShowEndDialog();

            if (result.Equals(MessageBoxResult.No))
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
            // 終了ログ
            base.WriteEndLog("Window_Closed", Consts.PROCESS_011);
        }


        //Park.iggy
        private void Hyperlink_SendClick(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        //END



        //*******************************************************************
        /// <summary>タブ切り替え</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void tabControl_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {

                switch (tabControl.SelectedIndex) /* ここでは選択されたインデックスが返される */
                {
                    case 0: /* tabPage1 */
                        if (_execControl != null)
                        {
                            _execControl.Stop();
                            _execControl.DB.CloseSqlConnect();
                        }

                        //added by YAMA 2014/08/06    （ｼﾞｮﾌﾞﾏﾈｰｼﾞｬのﾒｲﾝ画面に表示する画面名不備の対応）
                        // 表示中の画面が「オブジェクト一覧画面 or ジョブ編集画面」を判断して、this.Titleを更新する
                        //if (tab1Title != null) this.Title = tab1Title;
                        if (_objectEdit == null || !JobNetGrid.Children.Contains(_objectEdit))
                        {   // オブジェクト一覧画面の場合
                            if (_objList != null)
                            {   // _objListがnewされている場合は_objListからタイトルを取得、newされていない場合は画面ロードでタイトルを設定
                                this.Title = MessageUtil.GetMsgById(_objList.GamenId) + " - " + LoginSetting.JobconName;
                            }
                        }
                        else
                        {   // ジョブ編集画面の場合
                            this.Title = MessageUtil.GetMsgById(_objectEdit.GamenId) + " - " + LoginSetting.JobconName;
                        }

                        tabItemObjectList.IsTabStop = true;
                        tabItemJobManager.IsTabStop = false;
                        tabItemJobResult.IsTabStop = false;
                        break;
                    case 1: /* tabPage2 */
                        if (_execControl == null)
                        {
                            _execControl = new JobnetExecControlPage(this);
                            tabItemJobManager.Content = _execControl;
                        }
                        tab1Title = this.Title;
                        this.Title = MessageUtil.GetMsgById(_execControl.GamenId) + " - " + LoginSetting.JobconName;
                        _execControl.Start();
                        _execControl.DB.CreateSqlConnect();
                        tabItemJobManager.IsTabStop = true;
                        tabItemObjectList.IsTabStop = false;
                        tabItemJobResult.IsTabStop = false;
                        break;
                    case 2: /* tabPage3 */
                        if (_execControl != null)
                        {
                            _execControl.Stop();
                            _execControl.DB.CloseSqlConnect();
                        }
                        if (_execResult == null)
                        {
                            _execResult = new JobnetExecResultPage(this);
                            tabItemJobResult.Content = _execResult;
                        }
                        tab1Title = this.Title;
                        this.Title = MessageUtil.GetMsgById(_execResult.GamenId) + " - " + LoginSetting.JobconName;
                        tabItemJobResult.IsTabStop = true;
                        tabItemJobManager.IsTabStop = false;
                        tabItemObjectList.IsTabStop = false;
                        break;
                    //added by YAMA 2014/08/18
                    case 3: /* tabPage4 */
                        if (_execControl != null)
                        {
                            _execControl.Stop();
                            _execControl.DB.CloseSqlConnect();
                        }
                        if (_setParameter == null)
                        {
                            _setParameter = new SetParameterPage(this);
                            tabItemSetParameter.Content = _setParameter;
                        }
                        tab1Title = this.Title;
                        this.Title = MessageUtil.GetMsgById(_setParameter.GamenId) + " - " + LoginSetting.JobconName;
                        tabItemSetParameter.IsTabStop = true;
                        tabItemJobManager.IsTabStop = false;
                        tabItemObjectList.IsTabStop = false;
                        tabItemJobResult.IsTabStop = false;
                        break;

                    default:
                        break;
                }
            }
       }

        //*******************************************************************
        /// <summary>Delete、F5キー押下</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void JobArrangerWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                #if VIEWER
                #else
                if (tabControl.SelectedIndex == 0 && _objList != null && JobNetGrid.Children.Contains(_objList))
                {
                    DelFromMenuitemOrKey();
                }
                e.Handled = true;
                #endif
            }
            if (e.Key == Key.F5)
            {
                if (tabControl.SelectedIndex == 0 && _objList != null && JobNetGrid.Children.Contains(_objList))
                {
                    RefreshObjectList();
                }
                e.Handled = true;
            }
        }


        //*******************************************************************
        /// <summary>コンテクストメニューを表示</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void ContextMenu_Open(object sender, RoutedEventArgs e)
        {
            SetContextStatus();
        }

        //*******************************************************************
        /// <summary>TreeViewのPreview右マウスクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void TreetView_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            contextMenu.PlacementTarget = sender as UIElement;
            contextMenu.IsOpen = false;

            #if VIEWER
                this.contextMenu.Visibility = System.Windows.Visibility.Hidden;

            #else
                if (_objList != null && JobNetGrid.Children.Contains(_objList))
                {
                    this.contextMenu.IsOpen = true;
                    this.contextMenu.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.contextMenu.Visibility = System.Windows.Visibility.Hidden;
                }
            #endif

        }

        //*******************************************************************
        /// <summary>TreeViewItemのPreview右マウスクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (_objList != null && JobNetGrid.Children.Contains(_objList))
            {
                TreeViewItem ClickedTreeViewItem = new TreeViewItem();

                //find the original object that raised the event
                UIElement ClickedItem = VisualTreeHelper.GetParent(e.OriginalSource as UIElement) as UIElement;

                //find the clicked TreeViewItem
                while ((ClickedItem != null) && !(ClickedItem is TreeViewItem))
                {
                    ClickedItem = VisualTreeHelper.GetParent(ClickedItem) as UIElement;
                }

                ClickedTreeViewItem = ClickedItem as TreeViewItem;
                if (ClickedTreeViewItem != null)
                {
                    ClickedTreeViewItem.IsSelected = true;
                    ClickedTreeViewItem.Focus();
                }
                e.Handled = true;
            }

        }

        //*******************************************************************
        /// <summary>親ノードの右クリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void TreeViewParentItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            TreeViewItem ClickedTreeViewItem = new TreeViewItem();

            //find the original object that raised the event
            UIElement ClickedItem = VisualTreeHelper.GetParent(e.OriginalSource as UIElement) as UIElement;

            //find the clicked TreeViewItem
            while ((ClickedItem != null) && !(ClickedItem is TreeViewItem))
            {
                ClickedItem = VisualTreeHelper.GetParent(ClickedItem) as UIElement;
            }

            ClickedTreeViewItem = ClickedItem as TreeViewItem;
            if (ClickedTreeViewItem != null)
            {
                ClickedTreeViewItem.IsSelected = true;
                ClickedTreeViewItem.Focus();
            }
            e.Handled = true;
        }

        //*******************************************************************
        /// <summary>ファイルメニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemFile_Click(object sender, RoutedEventArgs e)
        {
            #if VIEWER
                menuitemAdd.IsEnabled = false;
                menuitemExport.IsEnabled = false;
                menuitemImport.IsEnabled = false;
                return;
            #endif
            menuitemAdd.IsEnabled = false;

            if (tabControl.SelectedIndex == 0 && (_objectEdit == null || !JobNetGrid.Children.Contains(_objectEdit)))
            {
                if (LoginSetting.Mode == Consts.ActionMode.DEVELOP)
                {
                    menuitemAdd.IsEnabled = true;
                }
            }
        }

        //*******************************************************************
        /// <summary>新規追加メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemAdd_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemAdd_Click", Consts.PROCESS_003);

            if (_objList != null && _objList.ObjectId != null)
            {
                NewObjectEditFromObjectList();
            }
            else
            {
                NewObjectEditFromTreeView();
            }
            if (_objectEdit.SuccessFlg)
            {
                ClearGridContent();

                JobNetGrid.Children.Add(_objectEdit);

                this.Title = MessageUtil.GetMsgById(_objectEdit.GamenId) + " - " + LoginSetting.JobconName;
            }
            // 終了ログ
            base.WriteEndLog("MenuitemAdd_Click", Consts.PROCESS_003);
        }

        //*******************************************************************
        /// <summary>インポートメニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemImport_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemImport_Click", Consts.PROCESS_013);

            ImportWindow importWindow = new ImportWindow(this);
            importWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            importWindow.Owner = this;
            importWindow.ShowDialog();

            // 終了ログ
            base.WriteEndLog("MenuitemImport_Click", Consts.PROCESS_013);

        }

        //*******************************************************************
        /// <summary>エクスポートメニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemExport_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemExport_Click", Consts.PROCESS_012);

            ExportWindow exportWindow = new ExportWindow(null, Consts.ObjectEnum.CALENDAR, null);
            exportWindow.Owner = this;
            exportWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            exportWindow.ShowDialog();

            // 終了ログ
            base.WriteEndLog("MenuitemExport_Click", Consts.PROCESS_012);
        }

        //*******************************************************************
        /// <summary>編集メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemEdit_Click(object sender, RoutedEventArgs e)
        {
            menuitemDel.IsEnabled = false;

            if (tabControl.SelectedIndex == 0 && (_objectEdit == null || !JobNetGrid.Children.Contains(_objectEdit)))
            {
                if (LoginSetting.Mode == Consts.ActionMode.DEVELOP && (IsSelectedListItem() || IsSelectedTreeItem()))
                {
                    menuitemDel.IsEnabled = true;
                }
            }
        }

        //*******************************************************************
        /// <summary>表示メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemView_Click(object sender, RoutedEventArgs e)
        {
            menuitemViewAll.IsEnabled = false;
            menuitemViewErr.IsEnabled = false;
            menuitemViewRunning.IsEnabled = false;

            if (tabControl.SelectedIndex == 1)
            {
                menuitemViewAll.IsEnabled = true;
                menuitemViewErr.IsEnabled = true;
                menuitemViewRunning.IsEnabled = true;
            }
        }

        //*******************************************************************
        /// <summary>削除メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemDel_Click(object sender, RoutedEventArgs e)
        {
            DelFromMenuitemOrKey();
        }

        //*******************************************************************
        /// <summary>カレンダーを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Calendar_Selected(object sender, RoutedEventArgs e)
        {
            if (_objList.ObjectId == null && _objList.ObjectType != Consts.ObjectEnum.FILTER)
                _objList.ObjectType = Consts.ObjectEnum.CALENDAR;
        }

        //*******************************************************************
        /// <summary>公開カレンダーを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Public_Calendar_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeCalendar(true, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.CALENDAR;
                _objectAllFlag = true;
            }
        }


        //*******************************************************************
        /// <summary>プライベートカレンダーを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Private_Calendar_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeCalendar(false, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.CALENDAR;
                _objectAllFlag = true;
            }
        }

        //*******************************************************************
        /// <summary>公開フィルターを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Public_Filter_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeFilter(true, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.FILTER;
                _objectAllFlag = true;
            }
        }


        //*******************************************************************
        /// <summary>プライベートフィルターを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Private_Filter_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeFilter(false, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.FILTER;
                _objectAllFlag = true;
            }
        }

        //*******************************************************************
        /// <summary>スケジュールを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Schedule_Selected(object sender, RoutedEventArgs e)
        {
            if (_objList.ObjectId == null)
                _objList.ObjectType = Consts.ObjectEnum.SCHEDULE;
        }

        //*******************************************************************
        /// <summary>公開スケジュールを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Public_Schedule_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeSchedule(true, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.SCHEDULE;
                _objectAllFlag = true;
            }
        }


        //*******************************************************************
        /// <summary>プライベートスケジュールを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Private_Schedule_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeSchedule(false, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.SCHEDULE;
                _objectAllFlag = true;
            }
        }

        //*******************************************************************
        /// <summary>ジョブネットを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            if (_objList.ObjectId == null)
                _objList.ObjectType = Consts.ObjectEnum.JOBNET;
        }

        //*******************************************************************
        /// <summary>公開ジョブネットを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Public_Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeJobnet(true, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.JOBNET;
                _objectAllFlag = true;
            }
        }

        //*******************************************************************
        /// <summary>プライベートジョブネットを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Private_Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            //Park.iggy Add
            if (!_objectListFlag)
            {
                _objectAllFlag = true;
            }
            SetTreeJobnet(false, null);
            if (_objList.ObjectId == null)
            {
                _objList.ObjectType = Consts.ObjectEnum.JOBNET;
                _objectAllFlag = true;
            }
        }

        //*******************************************************************
        /// <summary>実行メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemRun_Click(object sender, RoutedEventArgs e)
        {
            menuItemImmediateRun.IsEnabled = false;
            menuItemReserveRun.IsEnabled = false;
            menuItemTestRun.IsEnabled = false;
            if (tabControl.SelectedIndex == 0 && (_objectEdit == null || !JobNetGrid.Children.Contains(_objectEdit)))
            {
                if (IsSelectedTreeItem())
                {
                    if ((Consts.ObjectEnum)((TreeViewItem)treeView1.SelectedItem).Tag == Consts.ObjectEnum.JOBNET)
                    {
                        menuItemImmediateRun.IsEnabled = true;
                        menuItemReserveRun.IsEnabled = true;
                        menuItemTestRun.IsEnabled = true;
                    }
                }
            }
        }

        //*******************************************************************
        /// <summary>即時実行メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemImmediateRun_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemImmediateRun_Click", Consts.PROCESS_009);

            RunJobnet(Consts.RunTypeEnum.Immediate);

            // 終了ログ
            base.WriteEndLog("MenuitemImmediateRun_Click", Consts.PROCESS_009);
        }

        //*******************************************************************
        /// <summary>保留実行メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemReserveRun_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemReserveRun_Click", Consts.PROCESS_009);

            RunJobnet(Consts.RunTypeEnum.Reservation);

            // 終了ログ
            base.WriteEndLog("MenuitemReserveRun_Click", Consts.PROCESS_009);
        }

        //*******************************************************************
        /// <summary>テスト実行メニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemTestRun_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("MenuitemTestRun_Click", Consts.PROCESS_009);

            RunJobnet(Consts.RunTypeEnum.Test);

            // 終了ログ
            base.WriteEndLog("MenuitemTestRun_Click", Consts.PROCESS_009);
        }

        //*******************************************************************
        /// <summary>バージョンメニュークリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemVer_Click(object sender, RoutedEventArgs e)
        {
            CommonDialog.ShowVersionDialog();
        }

        //*******************************************************************
        /// <summary>オブジェクトを新規追加</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ContextMenuitemAdd_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("ContextMenuitemAdd_Click", Consts.PROCESS_003);

            // 登録を行う
            if (_objectEdit != null && JobNetGrid.Children.Contains(_objectEdit))
            {
                if (!HandleEdit(_objectEdit))
                    return;
            }

            //Treeの選択状況からオブジェクト編集画面を作成
            NewObjectEditFromTreeView();

            if (_objectEdit.SuccessFlg)
            {
                ClearGridContent();

                _objectEdit.ParantWindow = this;

                JobNetGrid.Children.Add(_objectEdit);

                this.Title = MessageUtil.GetMsgById(_objectEdit.GamenId) + " - " + LoginSetting.JobconName;
            }

            // 終了ログ
            base.WriteEndLog("ContextMenuitemAdd_Click", Consts.PROCESS_003);
        }

        //*******************************************************************
        /// <summary>削除</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ContextMenuitemDel_Click(object sender, RoutedEventArgs e)
        {
            string objectId = ((TreeViewItem)treeView1.SelectedItem).Header.ToString();
            Consts.ObjectEnum objectType = (Consts.ObjectEnum)((TreeViewItem)treeView1.SelectedItem).Tag;
            if (CheckUtil.isExistGroupId(LoginSetting.GroupList, DBUtil.GetGroupIDListByID(objectId, objectType)) || LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                _objList.DelObject(objectId, objectType, null);
                return;
            }
            CommonDialog.ShowErrorDialog(Consts.MSG_COMMON_006);
            e.Handled = true;
        }

        //*******************************************************************
        /// <summary>エクスポート</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ContextMenuitemExport_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("ContextMenuitemExport_Click", Consts.PROCESS_012);

            string objectId = ((TreeViewItem)treeView1.SelectedItem).Header.ToString();
            Consts.ObjectEnum objectType = (Consts.ObjectEnum)((TreeViewItem)treeView1.SelectedItem).Tag;
            ExportWindow exportWindow = new ExportWindow(objectId, objectType, null);
            exportWindow.Owner = this;
            exportWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            exportWindow.ShowDialog();

            // 終了ログ
            base.WriteEndLog("ContextMenuitemExport_Click", Consts.PROCESS_012);
        }

        //*******************************************************************
        /// <summary>ウインドウをクローズ</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void MenuitemEnd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        /// <summary>オブジェクトノードを選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Object_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("Object_Click", Consts.PROCESS_015);

            //Park.iggy Add 朴朴
            _objectAllFlag = false;

            // 登録を行う
            if (_objectEdit != null && JobNetGrid.Children.Contains(_objectEdit))
            {
                if (!HandleEdit(_objectEdit))
                    return;
            }
            TreeViewItem item = (TreeViewItem)sender;

            string objectId = item.Header.ToString();
            Consts.ObjectEnum objectType = (Consts.ObjectEnum)item.Tag;
            item.Focus();
            ShowObjectList(objectId, objectType);
            e.Handled = true;

            // 終了ログ
            base.WriteEndLog("Object_Click", Consts.PROCESS_015);

        }

        //*******************************************************************
        /// <summary>オブジェクトノードを選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        //Park.iggy Add
        private void Object_All_List(Boolean public_flag, Consts.ObjectEnum objectType)
        {
            // 開始ログ
            base.WriteStartLog("Object_All_List", Consts.PROCESS_015);

            // 登録を行う
            if (_objectEdit != null && JobNetGrid.Children.Contains(_objectEdit))
            {
                if (!HandleEdit(_objectEdit))
                    return;
            }
            //Consts.ObjectEnum objectType = (Consts.ObjectEnum)"JOBNET";
            ShowObjectListALL(public_flag,objectType);

            // 終了ログ
            base.WriteEndLog("Object_All_List", Consts.PROCESS_015);

        }

        //*******************************************************************
        /// <summary>ジョブネットをドラッグ</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void Jobnet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_objectEdit != null && JobNetGrid.Children.Contains(_objectEdit) && _objectEdit is JobEdit
                    && !(((JobEdit)_objectEdit).FlowEditType==Consts.EditType.READ))

            {
                // single click
                if (e.ClickCount == 1)
                {
                    String jobnetId = ((TreeViewItem)sender).Header.ToString();
                    JobEdit jobEdit = ((JobEdit)_objectEdit);
                    if (!jobEdit.JobnetId.Equals(jobnetId)
                        || jobEdit.FlowEditType == Consts.EditType.CopyNew)
					{
                        JobData jobData = new JobData();
                        jobData.JobType = ElementType.JOBNET;
                        jobData.Data = jobnetId;
                        DragDrop.DoDragDrop((TreeViewItem)sender, jobData, DragDropEffects.Copy);
                    }
                    e.Handled = false;

                }
            }
        }

        #endregion

        #region protected override メソッド
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Prior)
            {
                if (treeView1.IsKeyboardFocusWithin)
                {
                    ScrollViewer treeViewScroll = FindVisualChild<ScrollViewer>(treeView1);
                    treeViewScroll.ScrollToVerticalOffset(treeViewScroll.VerticalOffset - treeViewScroll.ViewportHeight);
                }
                if (_objList.IsKeyboardFocusWithin)
                {
                    if (JobNetGrid.Children.Contains(_objList))
                    {
                        ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(_objList.dgObject);
                        if (scrollViewer != null && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollViewer.ViewportHeight);
                        }

                    }
                }
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Next)
            {
                if (treeView1.IsKeyboardFocusWithin)
                {
                    ScrollViewer treeViewScroll = FindVisualChild<ScrollViewer>(treeView1);
                    treeViewScroll.ScrollToVerticalOffset(treeViewScroll.VerticalOffset + treeViewScroll.ViewportHeight);
                }
                if (_objList.IsKeyboardFocusWithin)
                {
                    if (JobNetGrid.Children.Contains(_objList))
                    {
                        ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(_objList.dgObject);
                        if (scrollViewer != null && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollViewer.ViewportHeight);
                        }

                    }
                }

                e.Handled = true;
                return;
            }
            base.OnPreviewKeyDown(e);
        }
        #endregion

        #region public メソッド
        //*******************************************************************
        /// <summary>オブジェクト一覧を表示</summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objecType">オブジェクトタイプ</param>
        //*******************************************************************
        public void ShowObjectList(string objectId, Consts.ObjectEnum objectType)
        {
            // 開始ログ
            base.WriteStartLog("ShowObjectList", Consts.PROCESS_015);
            //Park.iggy Add
            String lObjectId = "";

            ClearGridContent();

            if (objectId != null)
            {
                _objList = new ObjectList(this, objectId, objectType);
                //リストにデータがない場合、選択された親TreeViewからオブジェクト種別を取得してセット
                if (_objList.dgObject.Items.Count < 1)
                {
                    _objList.ObjectType = GetObjectEnum4NotData();
                }

            }

            JobNetGrid.Children.Add(_objList);

            //Park.iggy Add
            if (objectId == null || objectId.Length == 0)
            {
                lObjectId = _objList.ObjectId;
            }
            else
            {
                lObjectId = objectId;
            }

            Boolean publicFlag = false;
            DataTable dt = DBUtil.GetObjectsById(lObjectId, objectType);
            if (dt.Rows.Count > 0 && (Int32)(dt.Rows[0]["public_flag"]) == 1)
                publicFlag = true;

            TreeViewItem item = null;
            if (publicFlag)
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    item = publicCalendar;
                }
                else if (objectType == Consts.ObjectEnum.FILTER)
                {
                    item = publicFilter;
                }
                else if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    item = publicSchedule;
                }
                else if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    item = publicJobnet;
                }
            }
            else
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    item = privateCalendar;
                }
                else if (objectType == Consts.ObjectEnum.FILTER)
                {
                    item = privateFilter;
                }
                else if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    item = privateSchedule;
                }
                else if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    item = privateJobnet;
                }
            }

            for (int i = 0; i < item.Items.Count;i++ )
            {
                TreeViewItem chaView = (TreeViewItem)item.Items[i];
                if (lObjectId != null && chaView.Header.Equals(lObjectId))
                {
                    chaView.IsSelected = true;
                    break;
                }
            }

            //Park.iggy END

            this.Title = MessageUtil.GetMsgById(_objList.GamenId) + " - " + LoginSetting.JobconName;


            // 終了ログ
            base.WriteEndLog("ShowObjectList", Consts.PROCESS_015);
        }

        //*******************************************************************
        /// <summary>オブジェクトALL一覧を表示</summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objecType">オブジェクトタイプ</param>
        //*******************************************************************
        //Park.iggy
        public void ShowObjectListALL(Boolean public_flag,Consts.ObjectEnum objectType)
        {
            // 開始ログ
            base.WriteStartLog("ShowObjectListALL", Consts.PROCESS_015);
            ClearGridContent();

            if (objectType != null)
            {
                _objList = new ObjectList(this, public_flag, objectType);
                //リストにデータがない場合、選択された親TreeViewからオブジェクト種別を取得してセット
                if (_objList.dgObject.Items.Count < 1)
                {
                    _objList.ObjectType = GetObjectEnum4NotData();
                }

            }

            JobNetGrid.Children.Add(_objList);

            this.Title = MessageUtil.GetMsgById(_objList.GamenId) + " - " + LoginSetting.JobconName;


            // 終了ログ
            base.WriteEndLog("ShowObjectListALL", Consts.PROCESS_015);
        }

        //*******************************************************************
        /// <summary>オブジェクト一覧を表示</summary>
        /// <param name="objectId">オブジェクトＩＤ</param>
        /// <param name="objecType">オブジェクトタイプ</param>
        //*******************************************************************
        public void RefreshObjectList()
        {
            String objectId = null;
            Consts.ObjectEnum objectType = Consts.ObjectEnum.CALENDAR;
            if (_objList != null)
            {
                objectId = _objList.ObjectId;
                objectType = _objList.ObjectType;
            }

            if (publicCalendar.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    SetTreeObject(true, Consts.ObjectEnum.CALENDAR, objectId);
                }
                else
                {
                    SetTreeObject(true, Consts.ObjectEnum.CALENDAR, null);
                }
            }
            if (privateCalendar.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.CALENDAR)
                {
                    SetTreeObject(false, Consts.ObjectEnum.CALENDAR, objectId);
                }
                else
                {
                    SetTreeObject(false, Consts.ObjectEnum.CALENDAR, null);
                }
            }
            if (publicFilter.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    SetTreeObject(true, Consts.ObjectEnum.FILTER, objectId);
                }
                else
                {
                    SetTreeObject(true, Consts.ObjectEnum.FILTER, null);
                }
            }
            if (privateFilter.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.FILTER)
                {
                    SetTreeObject(false, Consts.ObjectEnum.FILTER, objectId);
                }
                else
                {
                    SetTreeObject(false, Consts.ObjectEnum.FILTER, null);
                }
            }

            if (publicSchedule.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    SetTreeObject(true, Consts.ObjectEnum.SCHEDULE, objectId);
                }
                else
                {
                    SetTreeObject(true, Consts.ObjectEnum.SCHEDULE, null);
                }
            }
            if (privateSchedule.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.SCHEDULE)
                {
                    SetTreeObject(false, Consts.ObjectEnum.SCHEDULE, objectId);
                }
                else
                {
                    SetTreeObject(false, Consts.ObjectEnum.CALENDAR, null);
                }
            }

            if (publicJobnet.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    SetTreeObject(true, Consts.ObjectEnum.JOBNET, objectId);
                }
                else
                {
                    SetTreeObject(true, Consts.ObjectEnum.JOBNET, null);
                }
            }
            if (privateJobnet.IsExpanded)
            {
                if (objectType == Consts.ObjectEnum.JOBNET)
                {
                    SetTreeObject(false, Consts.ObjectEnum.JOBNET, objectId);
                }
                else
                {
                    SetTreeObject(false, Consts.ObjectEnum.JOBNET, null);
                }
            }
            if (_objList != null )
                ShowObjectList(objectId, objectType);

        }

        //*******************************************************************
        /// <summary>オブジェクトを編集画面を表示</summary>
        /// <param name="jobnetId">オブジェクトＩＤ</param>
        /// <param name="updDate">更新日</param>
        /// <param name="editType">編集タイプ</param>
        /// <param name="objecType">オブジェクトタイプ</param>
        //*******************************************************************
        public void EditObject(string objectId, string updDate, Consts.EditType editType, Consts.ObjectEnum objecType)
        {
            // 開始ログ
            base.WriteStartLog("EditObject", Consts.PROCESS_016);

            // 登録を行う
            if (_objectEdit != null && JobNetGrid.Children.Contains(_objectEdit))
            {
                if (!HandleEdit(_objectEdit))
                    return;

                JobNetGrid.Children.Remove(_objectEdit);
            }
            _objectEdit = getEditObject(objectId, updDate, editType, objecType);

            if (_objectEdit.SuccessFlg)
            {
                ClearGridContent();

                this.Title = MessageUtil.GetMsgById(_objectEdit.GamenId) + " - " + LoginSetting.JobconName;

                JobNetGrid.Children.Add(_objectEdit);
            }
            // 終了ログ
            base.WriteEndLog("EditObject", Consts.PROCESS_016);
        }

        //*******************************************************************
        /// <summary>Treeコンテクストメニュー、メニューバー、Delete Keyから削除</summary>
        //*******************************************************************
        public void DelFromMenuitemOrKey()
        {
            // 開始ログ
            base.WriteStartLog("DelFromMenuitemOrKey", Consts.PROCESS_006);

            if (IsSelectedListItem())
            {
                #if VIEWER
                #else
                if (_objList.ListObjectOwnType == Consts.ObjectOwnType.Owner || LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
                {
                    _objList.DelObject(_objList.ObjectId, _objList.ObjectType, _objList.GetSelectedRows());
                    return;
                }
                else
                {
                    CommonDialog.ShowErrorDialog(Consts.MSG_COMMON_006);
                }
                #endif
            }
            else
            {
                if (IsSelectedTreeItem())
                {
                    string objectId = ((TreeViewItem)treeView1.SelectedItem).Header.ToString();
                    Consts.ObjectEnum objectType = (Consts.ObjectEnum)((TreeViewItem)treeView1.SelectedItem).Tag;
                    if (CheckUtil.isExistGroupId(LoginSetting.GroupList, DBUtil.GetGroupIDListByID(objectId, objectType)) || LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
                    {
                        _objList.DelObject(objectId, objectType, null);
                        return;
                    }
                    else
                    {
                        CommonDialog.ShowErrorDialog(Consts.MSG_COMMON_006);
                    }
                }
            }

            // 終了ログ
            base.WriteEndLog("DelFromMenuitemOrKey", Consts.PROCESS_006);
        }
        //*******************************************************************
        /// <summary>オブジェクトTreeを展開</summary>
        /// <param name="publicFlag">公開フラグ</param>
        /// <param name="objectType">オブジェクト種別</param>
        //*******************************************************************
        public void SetTreeObject(bool publicFlag, Consts.ObjectEnum objectType, String objectID)
        {
            //Park.iggy Add
            _objectListFlag = true;

            switch (objectType)
            {
                case Consts.ObjectEnum.CALENDAR:
                    if (publicFlag)
                    {
                        publicCalendar.IsExpanded = false;
                        SetTreeCalendar(publicFlag, objectID);
                        publicCalendar.IsExpanded = true;

                    }
                    if (!publicFlag)
                    {
                        privateCalendar.IsExpanded = false;
                        SetTreeCalendar(publicFlag, objectID);
                        privateCalendar.IsExpanded = true;
                    }
                    break;
                case Consts.ObjectEnum.FILTER:
                    if (publicFlag)
                    {
                        publicFilter.IsExpanded = false;
                        SetTreeFilter(publicFlag, objectID);
                        publicFilter.IsExpanded = true;

                    }
                    if (!publicFlag)
                    {
                        privateFilter.IsExpanded = false;
                        SetTreeFilter(publicFlag, objectID);
                        privateFilter.IsExpanded = true;
                    }
                    break;
                case Consts.ObjectEnum.SCHEDULE:
                    if (publicFlag)
                    {
                        publicSchedule.IsExpanded = false;
                        SetTreeSchedule(publicFlag, objectID);
                        publicSchedule.IsExpanded = true;

                    }
                    if (!publicFlag)
                    {
                        privateSchedule.IsExpanded = false;
                        SetTreeSchedule(publicFlag, objectID);
                        privateSchedule.IsExpanded = true;
                    }
                    break;
                case Consts.ObjectEnum.JOBNET:
                    if (publicFlag)
                    {
                        publicJobnet.IsExpanded = false;
                        SetTreeJobnet(publicFlag, objectID);
                        publicJobnet.IsExpanded = true;

                    }
                    if (!publicFlag)
                    {

                        privateJobnet.IsExpanded = false;
                        SetTreeJobnet(publicFlag, objectID);
                        privateJobnet.IsExpanded = true;
                    }
                    break;
            }
            //Park.iggy Add
            _objectListFlag = false;
        }
        #endregion

        #region  private メソッド
        //*******************************************************************
        /// <summary>Treeコンテクストメニューの使用可能をセットする</summary>
        //*******************************************************************
        private void SetContextStatus()
        {
            contextMenuitemAdd.IsEnabled = true;
            contextMenuitemDel.IsEnabled = false;
            contextMenuitemExport.IsEnabled = true;
            contextMenuItemImmediateRun.IsEnabled = true;
            contextMenuItemReserveRun.IsEnabled = true;
            contextMenuItemTestRun.IsEnabled = true;

            if (!IsSelectedTreeItem())
            {
                contextMenuitemDel.IsEnabled = false;
                contextMenuitemExport.IsEnabled = false;
            }
            if (LoginSetting.Mode == Consts.ActionMode.USE)
            {
                contextMenuitemAdd.IsEnabled = false;
                contextMenuitemDel.IsEnabled = false;
            }
            if (!IsSelectedJobnetTreeItem())
            {
                contextMenuItemImmediateRun.IsEnabled = false;
                contextMenuItemReserveRun.IsEnabled = false;
                contextMenuItemTestRun.IsEnabled = false;
            }
        }
        //*******************************************************************
        /// <summary>選択されたTreeViewから編集画面を取得</summary>
        //*******************************************************************
        private void NewObjectEditFromTreeView()
        {
            _objectEdit = null;
            //Park.iggy Add
            _objectAllFlag = false;

            if (treeView1.SelectedItem == null)
            {
                _objectEdit = new CalendarEdit(this);
                return;
            }
            if (treeView1.SelectedItem.Equals(treeViewCalendar)
                || treeView1.SelectedItem.Equals(publicCalendar)
                || treeView1.SelectedItem.Equals(privateCalendar))
            {
                _objectEdit = new CalendarEdit(this);
                return;
            }
            if (treeView1.SelectedItem.Equals(publicFilter)
                || treeView1.SelectedItem.Equals(privateFilter))
            {
                _objectEdit = new FilterEdit(this);
                return;
            }

            if (treeView1.SelectedItem.Equals(treeViewSchedule)
                || treeView1.SelectedItem.Equals(publicSchedule)
                || treeView1.SelectedItem.Equals(privateSchedule))
            {
                _objectEdit = new ScheduleEdit(this);
                return;
            }

            if (treeView1.SelectedItem.Equals(treeViewJobNet)
                || treeView1.SelectedItem.Equals(publicJobnet)
                || treeView1.SelectedItem.Equals(privateJobnet))
            {
                _objectEdit = new JobEdit(this);
                return;
            }

            if ((Consts.ObjectEnum)(((TreeViewItem)(treeView1.SelectedItem)).Tag) == Consts.ObjectEnum.CALENDAR)
            {
                _objectEdit = new CalendarEdit(this);
                return;
            }
            if ((Consts.ObjectEnum)(((TreeViewItem)(treeView1.SelectedItem)).Tag) == Consts.ObjectEnum.FILTER)
            {
                _objectEdit = new FilterEdit(this);
                return;
            }
            if ((Consts.ObjectEnum)(((TreeViewItem)(treeView1.SelectedItem)).Tag) == Consts.ObjectEnum.SCHEDULE)
            {
                _objectEdit = new ScheduleEdit(this);
                return;
            }
            if ((Consts.ObjectEnum)(((TreeViewItem)(treeView1.SelectedItem)).Tag) == Consts.ObjectEnum.JOBNET)
            {
                _objectEdit = new JobEdit(this);
                return;
            }
        }
        //*******************************************************************
        /// <summary>選択されたデータ以外のTreeViewからオブジェクト種別取得</summary>
        //*******************************************************************
        private Consts.ObjectEnum GetObjectEnum4NotData()
        {
            if (treeView1.SelectedItem == null)
            {
                return Consts.ObjectEnum.CALENDAR;
            }
            if (treeView1.SelectedItem.Equals(treeViewCalendar)
                || treeView1.SelectedItem.Equals(publicCalendar)
                || treeView1.SelectedItem.Equals(privateCalendar))
            {
                return Consts.ObjectEnum.CALENDAR;
            }
            if (treeView1.SelectedItem.Equals(publicFilter)
                || treeView1.SelectedItem.Equals(privateFilter))
            {
                return Consts.ObjectEnum.FILTER;
            }

            if (treeView1.SelectedItem.Equals(treeViewSchedule)
                || treeView1.SelectedItem.Equals(publicSchedule)
                || treeView1.SelectedItem.Equals(privateSchedule))
            {
                return Consts.ObjectEnum.SCHEDULE;
            }

            if (treeView1.SelectedItem.Equals(treeViewJobNet)
                || treeView1.SelectedItem.Equals(publicJobnet)
                || treeView1.SelectedItem.Equals(privateJobnet))
            {
                return Consts.ObjectEnum.JOBNET;
            }
            return Consts.ObjectEnum.CALENDAR;
        }
        //*******************************************************************
        /// <summary>オブジェクト一覧画面から編集画面を取得</summary>
        //*******************************************************************
        private void NewObjectEditFromObjectList()
        {
            _objectEdit = null;
            //Pakr.iggy Add
            _objectAllFlag = false;

            switch (_objList.ObjectType)
            {
                case Consts.ObjectEnum.CALENDAR:
                    _objectEdit = new CalendarEdit(this);
                    break;
                case Consts.ObjectEnum.FILTER:
                    _objectEdit = new FilterEdit(this);
                    break;
                case Consts.ObjectEnum.SCHEDULE:
                    _objectEdit = new ScheduleEdit(this);
                    break;
                case Consts.ObjectEnum.JOBNET:
                    _objectEdit = new JobEdit(this);
                    break;
            }
        }

        /// <summary>編集画面のオブジェクトを取得</summary>
        private EditBaseUserControl getEditObject(string objectId, string updDate, Consts.EditType editType, Consts.ObjectEnum objecType)
        {
            _objectEdit = null;
            //Park.iggy Add 朴
            //_objectAllFlag = false;

            switch (objecType)
            {
                case Consts.ObjectEnum.CALENDAR:
                    _objectEdit = new CalendarEdit(this, objectId, updDate, editType);
                    break;
                case Consts.ObjectEnum.FILTER:
                    _objectEdit = new FilterEdit(this, objectId, updDate, editType);
                    break;
                case Consts.ObjectEnum.SCHEDULE:
                    _objectEdit = new ScheduleEdit(this, objectId, updDate, editType);
                    break;
                case Consts.ObjectEnum.JOBNET:
                    _objectEdit = new JobEdit(this, objectId, updDate, editType);
                    break;
            }
            return _objectEdit;
        }

        /// <summary>オブジェクト編集画面の処理</summary>
        private bool HandleEdit(EditBaseUserControl editObject)
        {
            // 未編集の場合
            if (!editObject.HasEditedCheck())
            {
                editObject.Rollback();
                return true;
            }

            // 編集登録確認ダイアログの表示
            if (MessageBoxResult.Yes == CommonDialog.ShowEditRegistDialog())
            {
                // 登録が失敗の場合
                if (!editObject.RegistObject())
                    return false;
                editObject.Commit();
                editObject.ResetTree(null);
            }
            else
            {
                editObject.Rollback();
            }
            return true;
        }

        /// <summary>Gridの内容をクリア</summary>
        public void ClearGridContent()
        {
            UIElementCollection items = JobNetGrid.Children;
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                    if (!(items[i] is Label))
                        JobNetGrid.Children.Remove(items[i]);
            }

        }

        /// <summary>オブジェクト一覧でデータが選択されたか判断</summary>
        /// <return>選択データ存在する場合True、存在しない場合False</return>
        private bool IsSelectedListItem()
        {
            if (_objList != null && _objList.dgObject.SelectedItems.Count > 0)
                return true;
            return false;
        }

        /// <summary>オブジェクトTreeでオブジェクトノードが選択されたか判断</summary>
        /// <return>選択データ存在する場合True、存在しない場合False</return>
        private bool IsSelectedTreeItem()
        {
            if (treeView1.SelectedItem != null)
            {
                if (!(treeView1.SelectedItem.Equals(treeViewCalendar) ||
                    treeView1.SelectedItem.Equals(publicCalendar) ||
                    treeView1.SelectedItem.Equals(privateCalendar) ||
                    treeView1.SelectedItem.Equals(publicFilter) ||
                    treeView1.SelectedItem.Equals(privateFilter) ||
                    treeView1.SelectedItem.Equals(treeViewSchedule) ||
                    treeView1.SelectedItem.Equals(publicSchedule) ||
                    treeView1.SelectedItem.Equals(privateSchedule) ||
                    treeView1.SelectedItem.Equals(treeViewJobNet) ||
                    treeView1.SelectedItem.Equals(publicJobnet) ||
                    treeView1.SelectedItem.Equals(privateJobnet)))
                {

                    return true;
                }
            }
            return false;
        }

        /// <summary>オブジェクトTreeでジョブネットオブジェクトノードが選択されたか判断</summary>
        /// <return>選択データ存在する場合True、存在しない場合False</return>
        private bool IsSelectedJobnetTreeItem()
        {
            if(IsSelectedTreeItem())
            {
                if ((Consts.ObjectEnum)((TreeViewItem)treeView1.SelectedItem).Tag == Consts.ObjectEnum.JOBNET)
                {

                    return true;
                }
            }

            return false;
        }

        /// <summary>ジョブネット実行</summary>
        /// <param name="runType">実行種別 1:即時実行、2:保留実行、3:テスト実行</param>
        private void RunJobnet(Consts.RunTypeEnum runType)
        {
            String innerJobnetId = null;

            String jobnetId = ((TreeViewItem)treeView1.SelectedItem).Header.ToString();
            DataTable dtJobnet = DBUtil.GetValidJobnetVer(jobnetId);
            if (dtJobnet.Rows.Count > 0)
            {
                MessageBoxResult result = CommonDialog.ShowJobnetStartDialog();

                if (result == MessageBoxResult.Yes)
                {
                    innerJobnetId = DBUtil.InsertRunJobnet(dtJobnet.Rows[0], runType);
                    if (innerJobnetId != null)
                    {
                        DuringStartupJobnetWindow duringStartupJobnetWindow = new DuringStartupJobnetWindow(innerJobnetId, this);
                        duringStartupJobnetWindow.startCheck();
                        duringStartupJobnetWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        duringStartupJobnetWindow.Show();
                    }
                    return;
                }
            }
            else
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_RUN_JOBNET_001);
                return;
            }

        }

        /// <summary>カレンダーTree展開</summary>
        /// <param name="publicFlag">公開フラグ</param>
        /// <param name="calendarID">カレンダーＩＤ</param>
        private void SetTreeCalendar(bool public_flg, String calendarID)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
            bool isBelongToUSRGRP = LoginSetting.BelongToUsrgrpFlag;

            int flg;
            TreeViewItem treeViewItem;

            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicCalendar;
                publicCalendar.Selected -= Public_Calendar_Selected;
                //treeViewItem.Items.Clear();
            }
            else
            {
                flg = 0;
                treeViewItem = privateCalendar;
                privateCalendar.Selected -= Private_Calendar_Selected;
            }

            treeViewItem.Items.Clear();

            string selectSql;
            if (public_flg)
            {
                selectSql = "select calendar_id, max(update_date) from ja_calendar_control_table where public_flag= " +
                                flg + " group by calendar_id order by calendar_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
                    //selectSql = "SELECT distinct A.calendar_id,A.update_date "
                    //                                + "FROM ja_calendar_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                    //                                + "WHERE A.user_name = U.alias and U.userid=UG1.userid and UG2.userid="+LoginSetting.UserID
                    //                                +" and UG1.usrgrpid = UG2.usrgrpid and "
                    //                                + "A.update_date= "
                    //                                + "( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B "
                    //                                + "WHERE B.calendar_id = A.calendar_id group by B.calendar_id) and A.public_flag=0 order by A.calendar_id";
                    if (isBelongToUSRGRP)
                    {
                        selectSql = "SELECT distinct A.calendar_id,A.update_date "
                                                        + "FROM ja_calendar_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                        + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                        + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                        + "A.update_date= "
                                                        + "( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B "
                                                        + "WHERE B.calendar_id = A.calendar_id group by B.calendar_id) and A.public_flag=0 order by A.calendar_id";
                    }
                    else
                    {
                        selectSql = "SELECT distinct A.calendar_id,A.update_date "
                                                        + "FROM ja_calendar_control_table AS A,users AS U "
                                                        + "WHERE A.user_name = U.username "
                                                        + " and "
                                                        + "A.update_date= "
                                                        + "( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B "
                                                        + "WHERE B.calendar_id = A.calendar_id group by B.calendar_id) and A.public_flag=0 order by A.calendar_id";
                    }

                }
                else
                {
                    selectSql = "select calendar_id, max(update_date) from ja_calendar_control_table where public_flag= " +
                                    flg + " group by calendar_id order by calendar_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {

                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["calendar_id"].ToString();
                    item.Tag = Consts.ObjectEnum.CALENDAR;
                    treeViewItem.Items.Add(item);
                    if (calendarID != null && item.Header.Equals(calendarID))
                    {
                        item.IsSelected = true;
                    }

                }
            }

            TreeViewItem itemCalendar;
            foreach (object item in treeViewItem.Items)
            {
                itemCalendar = (TreeViewItem)item;
                itemCalendar.Selected += Object_Click;
            }
            if (public_flg)
            {
                treeViewItem = publicCalendar;
                publicCalendar.Selected += Public_Calendar_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(true, Consts.ObjectEnum.CALENDAR);
                }
            }
            else
            {
                treeViewItem = privateCalendar;
                privateCalendar.Selected += Private_Calendar_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(false, Consts.ObjectEnum.CALENDAR);
                }
            }
            dbaccess.CloseSqlConnect();
        }
        /// <summary>フィルターTree展開</summary>
        /// <param name="publicFlag">公開フラグ</param>
        /// <param name="filterID">フィルターＩＤ</param>
        private void SetTreeFilter(bool public_flg, String filterID)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
            bool isBelongToUSRGRP = LoginSetting.BelongToUsrgrpFlag;

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicFilter;
                publicFilter.Selected -= Public_Filter_Selected;
            }
            else
            {
                flg = 0;
                treeViewItem = privateFilter;
                privateFilter.Selected -= Private_Filter_Selected;
            }
            treeViewItem.Items.Clear();

            string selectSql;
            if (public_flg)
           {
                selectSql = "select filter_id, max(update_date) from ja_filter_control_table where public_flag= " +
                                flg + " group by filter_id order by filter_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
                    //selectSql = "SELECT distinct A.filter_id,A.update_date "
                    //                                + "FROM ja_filter_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                    //                                + "WHERE A.user_name = U.alias and U.userid=UG1.userid and UG2.userid="+LoginSetting.UserID
                    //                                +" and UG1.usrgrpid = UG2.usrgrpid and "
                    //                                + "A.update_date= "
                    //                                + "( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B "
                    //                                + "WHERE B.filter_id = A.filter_id group by B.filter_id) and A.public_flag=0 order by A.filter_id";
                    if (isBelongToUSRGRP)
                    {
                        selectSql = "SELECT distinct A.filter_id,A.update_date "
                                                        + "FROM ja_filter_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                        + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                        + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                        + "A.update_date= "
                                                        + "( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B "
                                                        + "WHERE B.filter_id = A.filter_id group by B.filter_id) and A.public_flag=0 order by A.filter_id";
                    }
                    else
                    {
                        selectSql = "SELECT distinct A.filter_id,A.update_date "
                                                        + "FROM ja_filter_control_table AS A,users AS U "
                                                        + "WHERE A.user_name = U.username"
                                                        + " and "
                                                        + "A.update_date= "
                                                        + "( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B "
                                                        + "WHERE B.filter_id = A.filter_id group by B.filter_id) and A.public_flag=0 order by A.filter_id";
                    }

                }
                else
                {
                    selectSql = "select filter_id, max(update_date) from ja_filter_control_table where public_flag= " +
                                    flg + " group by filter_id order by filter_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {

                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["filter_id"].ToString();
                    item.Tag = Consts.ObjectEnum.FILTER;
                    treeViewItem.Items.Add(item);
                    if (filterID != null && item.Header.Equals(filterID))
                    {
                        item.IsSelected = true;
                    }

                }
            }

            TreeViewItem itemFilter;
            foreach (object item in treeViewItem.Items)
            {
                itemFilter = (TreeViewItem)item;
                itemFilter.Selected += Object_Click;
            }
            if (public_flg)
            {
                treeViewItem = publicFilter;
                publicFilter.Selected += Public_Filter_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(true, Consts.ObjectEnum.FILTER);
                }
            }
            else
            {
                treeViewItem = privateFilter;
                privateFilter.Selected += Private_Filter_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(false, Consts.ObjectEnum.FILTER);
                }
            }
            dbaccess.CloseSqlConnect();
        }

        /// <summary>スケジュールTree展開</summary>
        /// <param name="publicFlag">公開フラグ</param>
        /// <param name="scheduleID">スケジュールＩＤ</param>
        private void SetTreeSchedule(bool public_flg, String scheduleID)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
            bool isBelongToUSRGRP = LoginSetting.BelongToUsrgrpFlag;

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicSchedule;
                publicSchedule.Selected -= Public_Schedule_Selected;
            }
            else
            {
                flg = 0;
                treeViewItem = privateSchedule;
                privateSchedule.Selected -= Private_Schedule_Selected;
            }

            treeViewItem.Items.Clear();

            string selectSql;
            if (public_flg)
            {
                selectSql = "select schedule_id, max(update_date) from ja_schedule_control_table where public_flag= " +
                                flg + " group by schedule_id order by schedule_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
                    //selectSql = "SELECT distinct A.schedule_id,A.update_date "
                    //                            + "FROM ja_schedule_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                    //                            + "WHERE A.user_name = U.alias and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                    //                            + " and UG1.usrgrpid = UG2.usrgrpid and "
                    //                            + "A.update_date= "
                    //                            + "( SELECT MAX(B.update_date) FROM ja_schedule_control_table AS B "
                    //                            + "WHERE B.schedule_id = A.schedule_id group by B.schedule_id) and A.public_flag=0 order by A.schedule_id";
                    if (isBelongToUSRGRP)
                    {
                        selectSql = "SELECT distinct A.schedule_id,A.update_date "
                                                    + "FROM ja_schedule_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                    + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                    + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                    + "A.update_date= "
                                                    + "( SELECT MAX(B.update_date) FROM ja_schedule_control_table AS B "
                                                    + "WHERE B.schedule_id = A.schedule_id group by B.schedule_id) and A.public_flag=0 order by A.schedule_id";
                    }
                    else
                    {
                        selectSql = "SELECT distinct A.schedule_id,A.update_date "
                                                    + "FROM ja_schedule_control_table AS A,users AS U "
                                                    + "WHERE A.user_name = U.username "
                                                    + " and "
                                                    + "A.update_date= "
                                                    + "( SELECT MAX(B.update_date) FROM ja_schedule_control_table AS B "
                                                    + "WHERE B.schedule_id = A.schedule_id group by B.schedule_id) and A.public_flag=0 order by A.schedule_id";
                    }

                }
                else
                {
                    selectSql = "select schedule_id, max(update_date) from ja_schedule_control_table where public_flag= " +
                                    flg + " group by schedule_id order by schedule_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {

                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["schedule_id"].ToString();
                    item.Tag = Consts.ObjectEnum.SCHEDULE;
                    treeViewItem.Items.Add(item);
                    if (scheduleID != null && item.Header.Equals(scheduleID))
                    {
                        item.IsSelected = true;
                    }
                }
            }

            TreeViewItem itemSchedule;
            foreach (object item in treeViewItem.Items)
            {
                itemSchedule = (TreeViewItem)item;
                itemSchedule.Selected += Object_Click;
            }

            if (public_flg)
            {
                treeViewItem = publicSchedule;
                publicSchedule.Selected += Public_Schedule_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(true, Consts.ObjectEnum.SCHEDULE);
                }
            }
            else
            {
                treeViewItem = privateSchedule;
                privateSchedule.Selected += Private_Schedule_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(false, Consts.ObjectEnum.SCHEDULE);
                }
            }

            dbaccess.CloseSqlConnect();
        }


        /// <summary>ジョブネットTree展開</summary>
        /// <param name="publicFlag">公開フラグ</param>
        /// <param name="jobnetID">ジョブネットＩＤ</param>
        private void SetTreeJobnet(bool public_flg, String jobnetID)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
            bool isBelongToUSRGRP = LoginSetting.BelongToUsrgrpFlag;

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicJobnet;
                publicJobnet.Selected -= Public_Jobnet_Selected;
            }
            else
            {
                flg = 0;
                treeViewItem = privateJobnet;
                privateJobnet.Selected -= Private_Jobnet_Selected;
            }

            treeViewItem.Items.Clear();
            string selectSql;
            if (public_flg)
            {
                selectSql = "select jobnet_id, max(update_date) from ja_jobnet_control_table where public_flag= " +
                                flg + " group by jobnet_id order by jobnet_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    // added by YAMA 2014/10/30    グループ所属無しユーザーでのマネージャ動作
                    //selectSql = "SELECT distinct A.jobnet_id,A.update_date "
                    //                                + "FROM ja_jobnet_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                    //                                + "WHERE A.user_name = U.alias and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                    //                                + " and UG1.usrgrpid = UG2.usrgrpid and "
                    //                                + "A.update_date= "
                    //                                + "( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B "
                    //                                + "WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) and A.public_flag=0 order by A.jobnet_id";
                    if (isBelongToUSRGRP)
                    {
                        selectSql = "SELECT distinct A.jobnet_id,A.update_date "
                                                        + "FROM ja_jobnet_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                        + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                        + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                        + "A.update_date= "
                                                        + "( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B "
                                                        + "WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) and A.public_flag=0 order by A.jobnet_id";
                    }
                    else
                    {
                        selectSql = "SELECT distinct A.jobnet_id,A.update_date "
                                  + "FROM ja_jobnet_control_table AS A,users AS U "
                                  + "WHERE A.user_name = U.username and "
                                  + "A.update_date= "
                                  + "( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B "
                                  + "WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) and A.public_flag=0 order by A.jobnet_id";
                    }

                }
                else
                {
                    selectSql = "select jobnet_id, max(update_date) from ja_jobnet_control_table where public_flag= " +
                                    flg + " group by jobnet_id order by jobnet_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {
                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["jobnet_id"].ToString();
                    item.Tag = Consts.ObjectEnum.JOBNET;
                    treeViewItem.Items.Add(item);
                    if (jobnetID != null && item.Header.Equals(jobnetID))
                    {
                        item.IsSelected = true;
                    }
                }
            }

            TreeViewItem itemJobnet;
            foreach (object item in treeViewItem.Items)
            {
                itemJobnet = (TreeViewItem)item;
                itemJobnet.PreviewMouseLeftButtonDown += Jobnet_MouseLeftButtonDown;
                itemJobnet.Selected += Object_Click;
            }
            if (public_flg)
            {
                treeViewItem = publicJobnet;
                publicJobnet.Selected += Public_Jobnet_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(true, Consts.ObjectEnum.JOBNET);
                }

            }
            else
            {
                treeViewItem = privateJobnet;
                privateJobnet.Selected += Private_Jobnet_Selected;
                //Park.iggy Add
                if (_objectAllFlag)
                {
                    Object_All_List(false, Consts.ObjectEnum.JOBNET);
                }
            }

            dbaccess.CloseSqlConnect();
        }

        /// <summary>子要素を検索</summary>
        private childItem FindVisualChild<childItem>(DependencyObject obj)
where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        #endregion
    }
public class ContentToPathConverter: IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var ps = new PathSegmentCollection(4);
        ContentPresenter cp = (ContentPresenter)value;
        double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
        double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
        ps.Add(new LineSegment(new Point(1, 0.7 * h), true));
        ps.Add(new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true));
        ps.Add(new LineSegment(new Point(w, h), true));
        ps.Add(new BezierSegment(new Point(w + 0.6 * h, h), new Point(w + h, 0), new Point(w + h * 1.3, 0), true));
        return ps;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion

}
public class ContentToMarginConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return new Thickness(0, 0, -((ContentPresenter)value).ActualHeight, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}

}
