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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Timers;
using System.Collections.ObjectModel;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;
using System.Windows.Threading;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// JobnetExecControlPage.xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecControlPage : BaseUserControl
    {
        #region フィールド

        private List<decimal> hideJobnetInnerIdList;
        private RunJobnetSummaryDAO runJobnetSummaryDAO;
        public int viewCount = 3;
        public int oldviewCount = -1;
        public bool viewJobnet = true;
        public bool viewErrJobnet = true;
        public bool viewRunningJobnet = true;
        private JobnetExecControlAllPage allPage;
        private JobnetExecControlErrPage errPage;
        private JobnetExecControlRunningPage runningPage;

        //added by YAMA 2014/07/07
        //private int jobnetLoadSpan = 0;
        private int jobnetViewSpan = 0;

        private DispatcherTimer dispatcherTimer;

        /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        /// <summary> エラーダイアログを表示するか </summary>
        private bool _isDb;    // true：表示する

        #endregion

        #region コンストラクタ
        public JobnetExecControlPage(JobArrangerWindow parent)
        {
            InitializeComponent();
            allPage = new JobnetExecControlAllPage(this);
            allPage.JobnetExecList = new ObservableCollection<JobnetExecInfo>();
            errPage = new JobnetExecControlErrPage(this);
            errPage.JobnetExecList = new ObservableCollection<JobnetExecInfo>();
            runningPage = new JobnetExecControlRunningPage(this);
            runningPage.JobnetExecList = new ObservableCollection<JobnetExecInfo>();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(refresh);
            dispatcherTimer.Start();
            _db.CreateSqlConnect();
            runJobnetSummaryDAO = new RunJobnetSummaryDAO(_db);
            hideJobnetInnerIdList = new List<decimal>();

            //added by YAMA 2014/07/07
            //jobnetLoadSpan = Convert.ToInt32(DBUtil.GetParameterVelue("JOBNET_LOAD_SPAN"));
            jobnetViewSpan = Convert.ToInt32(DBUtil.GetParameterVelue("JOBNET_VIEW_SPAN"));
            _parent = parent;

            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            _isDb = true;

            SetInit();

        }
        #endregion

        #region プロパティ
        private JobArrangerWindow _parent;
        public JobArrangerWindow Parent
        {
            get
            {
                return _parent;
            }
        }

        public override string ClassName
        {
            get
            {
                return "JobnetExecControlPage";
            }
        }

        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_300;
            }
        }

        private DBConnect _db = new DBConnect(LoginSetting.ConnectStr);
        public DBConnect DB
        {
            get
            {
                return _db;
            }
        }

        #endregion

        #region イベント
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    if (keyboardFocus.Equals(allPage.listView1))
                    {
                        if (allPage.listView1.Items.Count > 0)
                        {
                            allPage.listView1.SelectedIndex = 0;
                        }
                        if (viewErrJobnet)
                        {
                            Keyboard.Focus(errPage.listView1);
                            if (errPage.listView1.Items.Count > 0)
                            {
                                errPage.listView1.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (viewRunningJobnet)
                            {
                                Keyboard.Focus(runningPage.listView1);
                                if (runningPage.listView1.Items.Count > 0)
                                {
                                    runningPage.listView1.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                Keyboard.Focus(Parent.MenuItemFile);
                            }
                        }
                    }
                    else if (keyboardFocus.Equals(errPage.listView1))
                    {
                        if (viewRunningJobnet)
                        {
                            Keyboard.Focus(runningPage.listView1);
                            if (runningPage.listView1.Items.Count > 0)
                            {
                                runningPage.listView1.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            Keyboard.Focus(Parent.MenuItemFile);
                        }
                    }
                    else if (keyboardFocus.Equals(runningPage.listView1))
                    {
                        Keyboard.Focus(Parent.MenuItemFile);
                    }
                    else
                    {
                        keyboardFocus.MoveFocus(tRequest);
                    }
                }

                e.Handled = true;
                return;
            }
            if (e.Key == Key.Enter)
            {
                JobnetExecInfo jobnetExecInfo;
                if (allPage.listView1.IsKeyboardFocusWithin && allPage.listView1.SelectedItems.Count > 0)
                {
                    jobnetExecInfo = (JobnetExecInfo)allPage.listView1.SelectedItem;
                    if (jobnetExecInfo != null) ViewDetail(jobnetExecInfo.inner_jobnet_id);
                    return;
                }
                if (errPage.listView1.IsKeyboardFocusWithin && errPage.listView1.SelectedItems.Count > 0)
                {
                    jobnetExecInfo = (JobnetExecInfo)errPage.listView1.SelectedItem;
                    if (jobnetExecInfo != null) ViewDetail(jobnetExecInfo.inner_jobnet_id);
                    return;
                }
                if (runningPage.listView1.IsKeyboardFocusWithin && runningPage.listView1.SelectedItems.Count > 0)
                {
                    jobnetExecInfo = (JobnetExecInfo)runningPage.listView1.SelectedItem;
                    if (jobnetExecInfo != null) ViewDetail(jobnetExecInfo.inner_jobnet_id);
                    return;
                }

            }

        }
        private void refresh(object sender, EventArgs e)
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            DBException dbEx;

            dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                ((DispatcherTimer)sender).Interval = new TimeSpan(0, 0, 1);
                SetInit();
                resetDefinitions();
                resetData();
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }
            //((DispatcherTimer)sender).Interval = new TimeSpan(0, 0, 1);
            //SetInit();
            //resetDefinitions();
            //resetData();
        }

        /// <summary>コマンド実行可否</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>予定削除(未実行)</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        /// 削除は未実行で実行時刻が５分前のもの、または起動保留になったデータを削除
        private void ScheduleDeleteBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.deleteContextMenu.Tag;
            //Console.WriteLine("MessageBoxResul" + jobnetExecInfo.schedule_id);
            if (DBUtil.GetScheduleValidFlag(jobnetExecInfo.schedule_id))
            {
                if (MessageBoxResult.Yes == CommonDialog.ShowScheduleDelete())
                {
                    if (DBUtil.SetScheduleJobnetDelete(jobnetExecInfo.inner_jobnet_id))
                    {
                        Console.WriteLine("delete jobnet id " + jobnetExecInfo.inner_jobnet_id);
                    }
                    else
                    {
                        CommonDialog.ShowDeleteDialog(Consts.ERROR_COMMON_030);
                    }
                }
            }
            else
            {
                CommonDialog.ShowNotScheduleDelete(jobnetExecInfo.schedule_id);
            }
        }

        /// <summary>強制停止</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        private void AllStopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.stopContextMenu.Tag;
            if ((RunJobStatusType)jobnetExecInfo.status == RunJobStatusType.During)
            {
                DBUtil.StopRunningJobnet(jobnetExecInfo.inner_jobnet_id);
            }
            else
            {
                DBUtil.StopUnexecutedJobnet(jobnetExecInfo.inner_jobnet_id);
            }

        }

        /// <summary>強制停止</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        private void ErrStopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)errPage.stopContextMenu.Tag;
            //DBUtil.StopRunningJobnet(jobnetExecInfo.inner_jobnet_id);

            //Console.WriteLine("Stop listView1.SelectedItems.Count=[" + errPage.listView1.SelectedItems.Count + "] ");
            for (int i = 0; i < errPage.listView1.SelectedItems.Count; i++)
            {
                JobnetExecInfo stopJonnet = (JobnetExecInfo)errPage.listView1.SelectedItems[i];
                DBUtil.StopRunningJobnet(stopJonnet.inner_jobnet_id);
            }
            //JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)errPage.hideContextMenu.Tag;
            //hideJobnetInnerIdList.Add(jobnetExecInfo.inner_jobnet_id);
            errPage.ErrSelectedInnerJobnetIds.Clear();
        }

        /// <summary>強制停止</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        private void RunningStopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)runningPage.stopContextMenu.Tag;
            DBUtil.StopRunningJobnet(jobnetExecInfo.inner_jobnet_id);

        }

        /// <summary>非表示</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        private void HideCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //Console.WriteLine("listView1.SelectedItems.Count=[" + errPage.listView1.SelectedItems.Count + "] ");
            for (int i = 0; i < errPage.listView1.SelectedItems.Count; i++)
            {
                JobnetExecInfo hiddenJonnet = (JobnetExecInfo)errPage.listView1.SelectedItems[i];
                hideJobnetInnerIdList.Add(hiddenJonnet.inner_jobnet_id);
            }
            //JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)errPage.hideContextMenu.Tag;
            //hideJobnetInnerIdList.Add(jobnetExecInfo.inner_jobnet_id);
        }

        //added by YAMA 2014/04/25
        private void AllDelayedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.stopContextMenu.Tag;

            // 展開状況が「遅延起動」
            if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay)
            {
                DBUtil.StartDelayScheduleJobnet(jobnetExecInfo.inner_jobnet_id);
            }
        }

        //added by YAMA 2014/04/25
        private void ErrDelayedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)errPage.DelayedContextMenu.Tag;

            // 展開状況が「遅延起動」
            if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay)
            {
                DBUtil.StartDelayScheduleJobnet(jobnetExecInfo.inner_jobnet_id);
            }
        }

        //added by YAMA 2014/04/25
        private void RunningDelayedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)runningPage.stopContextMenu.Tag;

            // 展開状況が「遅延起動」
            if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay)
            {
                DBUtil.StartDelayScheduleJobnet(jobnetExecInfo.inner_jobnet_id);
            }
        }

        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary>開始予定時刻変更</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdtCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.updtContextMenu.Tag;
            DBUtil.SetReserveJobnet(jobnetExecInfo.inner_jobnet_id);
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary> 起動保留 </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReserveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // 実行用ジョブネット内部管理IDをキーに実行ジョブネットサマリ管理テーブルの起動保留フラグを「起動保留」に更新する
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.reserveContextMenu.Tag;
            DBUtil.SetReserveJobnet(jobnetExecInfo.inner_jobnet_id);
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary>起動保留解除</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // 実行用ジョブネット内部管理IDをキーに、当該内部管理IDが起動保留の場合、
            // 実行ジョブネットサマリ管理テーブルの起動保留フラグを「保留解除」に更新する
            JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)allPage.releaseContextMenu.Tag;
            DBUtil.SetReleaseJobnet(jobnetExecInfo.inner_jobnet_id);
        }


        #endregion

        #region publicメソッド

        /// <summary>実行ジョブネット詳細画面表示</summary>
        /// <param name="inner_jobnet_id">実行ジョブネット内部管理ID</param>
        public void ViewDetail(decimal inner_jobnet_id)
        {
            JobnetExecDetail detail = new JobnetExecDetail(inner_jobnet_id.ToString(), true);
            detail.Topmost = true;
            detail.Focusable = true;
            Keyboard.Focus(detail);
            detail.Show();
            detail.Focusable = true;
            Keyboard.Focus(detail);
        }

        /// <summary>refreshをストップ</summary>
        public void Stop()
        {
            dispatcherTimer.Stop();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0);
        }

        /// <summary>refreshをスタート</summary>
        public void Start()
        {
            /* added by YAMA 2014/12/11    V2.1.0 No34 対応 */
            //dispatcherTimer.Start();
            DBException dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                dispatcherTimer.Start();

                /* added by YAMA 2014/12/18 （ DBアクセスエラー時画面クリア対応）*/
                _isDb = true;
            }
        }
        #endregion

        #region privateメソッド

        /// <summary>表示設定を初期化</summary>
        private void SetInit()
        {
            viewCount = 0;
            viewJobnet = false;
            viewErrJobnet = false;
            viewRunningJobnet = false;
            if (_parent.menuitemViewAll.IsChecked)
            {
                viewCount++;
                viewJobnet = true;
            }
            if (_parent.menuitemViewErr.IsChecked)
            {
                viewCount++;
                viewErrJobnet = true;
            }
            if (_parent.menuitemViewRunning.IsChecked)
            {
                viewCount++;
                viewRunningJobnet = true;
            }

        }

        /// <summary>画面配置を再設定する</summary>
        private void resetDefinitions()
        {
            if (viewCount != oldviewCount)
            {
                jobnetExecControlGrid.Children.Clear();
                jobnetExecControlGrid.RowDefinitions.Clear();
                for (int i = 0; i < viewCount; i++)
                {
                    jobnetExecControlGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                if (viewJobnet)
                {
                    jobnetExecControlGrid.Children.Add(allPage);
                    Grid.SetColumn(allPage, 0);
                    Grid.SetRow(allPage, getRowNums()[0]);

                }
                if (viewErrJobnet)
                {
                    jobnetExecControlGrid.Children.Add(errPage);
                    Grid.SetColumn(errPage, 0);
                    Grid.SetRow(errPage, getRowNums()[1]);

                }
                if (viewRunningJobnet)
                {
                    jobnetExecControlGrid.Children.Add(runningPage);
                    Grid.SetColumn(runningPage, 0);
                    Grid.SetRow(runningPage, getRowNums()[2]);

                }
                oldviewCount = viewCount;
            }
        }

        /// <summary>画面データを再設定する</summary>
        private void resetData()
        {
            if (viewJobnet) viewJobnetList();
            if (viewErrJobnet) viewErrJobnetList();
            if (viewRunningJobnet) viewRunningJobnetList();

        }

        /// <summary>ジョブネットリスト表示する</summary>
        private void viewJobnetList()
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            bool isDb = true;    // false：DBダウン
            DBException dbEx;

            /* added by YAMA 2014/12/18 （ DBアクセスエラー時画面クリア対応）*/
            //allPage.JobnetExecList.Clear();
            dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
                allPage.JobnetExecList.Clear();

            DataTable dt;

            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            // added by YAMA 2014/10/20    マネージャ内部時刻同期
            //DateTime now = DateTime.Now;
            //DateTime now = DBUtil.GetSysTime();
            DateTime now;
            dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                now = DBUtil.GetSysTime();
            }
            else
            {
                isDb = false;
                now = DateTime.Now;
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            //added by YAMA 2014/07/07
            //DateTime before = now.AddMinutes(-1 * jobnetLoadSpan);
            //DateTime after = now.AddMinutes(jobnetLoadSpan);
            DateTime before = now.AddMinutes(-1 * jobnetViewSpan);
            DateTime after = now.AddMinutes(jobnetViewSpan);

            decimal fromTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMI(before);
            decimal toTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMI(after);

            decimal startFromTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMISS(before);
            decimal startToTime = ConvertUtil.ConverDate2IntYYYYMMDDHHMISS(after);

            if (LoginSetting.Authority.Equals(Consts.AuthorityEnum.SUPER))
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntitySuperAll(fromTime, toTime, startFromTime, startToTime);
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntitySuperAll(fromTime, toTime, startFromTime, startToTime);
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }
            }
            else
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntityAll(fromTime, toTime, startFromTime, startToTime, LoginSetting.UserID);
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntityAll(fromTime, toTime, startFromTime, startToTime, LoginSetting.UserID);
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    isDb = false;
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }
            }
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            if (isDb)
            {

                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    JobnetExecInfo jobnetExecInfo = createJobnetExecInfo(row);
                    allPage.JobnetExecList.Add(jobnetExecInfo);
                    if (allPage.AllSelectedInnerJobnetId.Equals(Convert.ToString(jobnetExecInfo.inner_jobnet_id)))
                    {
                        allPage.listView1.SelectedIndex = i;
                    }
                    i++;
                }
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //allPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    allPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                }
                else
                {
                    isDb = false;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }

            }    /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        }

        /// <summary>エラージョブネットリスト表示する</summary>
        private void viewErrJobnetList()
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            bool isDb = true;    // false：DBダウン
            DBException dbEx;

            /* added by YAMA 2014/12/18 （ DBアクセスエラー時画面クリア対応）*/
            //errPage.JobnetExecList.Clear();
            dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
                errPage.JobnetExecList.Clear();

            DataTable dt;
            if (LoginSetting.Authority.Equals(Consts.AuthorityEnum.SUPER))
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntitySuperErr();
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntitySuperErr();
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    isDb = false;
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }
            }
            else
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntityErr(LoginSetting.UserID);
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntityErr(LoginSetting.UserID);
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    isDb = false;
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }
            }
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            if (isDb)
            {

                int i = 0;

                foreach (DataRow row in dt.Rows)
                {
                    if (!hideJobnetInnerIdList.Contains(Convert.ToDecimal(row["inner_jobnet_id"])))
                    {
                        JobnetExecInfo jobnetExecInfo = createJobnetExecInfo(row);
                        errPage.JobnetExecList.Add(jobnetExecInfo);
                        /*
                        if (errPage.ErrSelectedInnerJobnetId.Equals(Convert.ToString(jobnetExecInfo.inner_jobnet_id)))
                        {
                            errPage.listView1.SelectedIndex = i;

                        }
                        i++;
                         * */
                        if (errPage.ErrSelectedInnerJobnetIds.Count > 0)
                        {
                            //Console.WriteLine("ErrSelectedInnerJobnetIds.Count=[" + errPage.ErrSelectedInnerJobnetIds.Count + "]");
                            foreach (String jobnetid in errPage.ErrSelectedInnerJobnetIds)
                            {
                                if (jobnetid.Equals(Convert.ToString(jobnetExecInfo.inner_jobnet_id)))
                                {
                                    //Console.WriteLine("IN If jobnetid=[" + jobnetid + "]");
                                    errPage.listView1.SelectedItems.Add(jobnetExecInfo);
                                    break;
                                }
                            }
                        }
                    }
                }

                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //errPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    errPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                }
                else
                {
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }

            }    /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        }

        /// <summary>実行中ジョブネットリスト表示する</summary>
        private void viewRunningJobnetList()
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            bool isDb = true;    // false：DBダウン
            DBException dbEx;

            /* added by YAMA 2014/12/18 （ DBアクセスエラー時画面クリア対応）*/
            //runningPage.JobnetExecList.Clear();
            dbEx = _db.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
                runningPage.JobnetExecList.Clear();

            DataTable dt;
            if (LoginSetting.Authority.Equals(Consts.AuthorityEnum.SUPER))
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntitySuperRunning();
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntitySuperRunning();
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    isDb = false;
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }

            }
            else
            {
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //dt = runJobnetSummaryDAO.GetEntityRunning(LoginSetting.UserID);
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    dt = runJobnetSummaryDAO.GetEntityRunning(LoginSetting.UserID);
                    if (dt == null)
                    {
                        isDb = false;
                    }
                }
                else
                {
                    isDb = false;
                    dt = null;
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }

            }
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            if (isDb)
            {

                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    JobnetExecInfo jobnetExecInfo = createJobnetExecInfo(row);
                    runningPage.JobnetExecList.Add(jobnetExecInfo);
                    if (runningPage.RunningSelectedInnerJobnetId.Equals(Convert.ToString(jobnetExecInfo.inner_jobnet_id)))
                    {
                        runningPage.listView1.SelectedItem = jobnetExecInfo;
                    }
                    i++;
                }
                /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
                //runningPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                dbEx = _db.exExecuteHealthCheck();
                if (dbEx.MessageID.Equals(""))
                {
                    runningPage.listView1.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                }
                else
                {
                    if (_isDb)
                    {
                        _isDb = false;
                        LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                        CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                    }
                }

            }    /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        }

        /// <summary>実行ジョブネットデータを作成する</summary>
        /// <param name="row">実行ジョブネットデータ</param>
        private JobnetExecInfo createJobnetExecInfo(DataRow row)
        {
            JobnetExecInfo jobnetExecInfo = new JobnetExecInfo();
            jobnetExecInfo.jobnet_id = row["jobnet_id"].ToString();
            jobnetExecInfo.status = (int)row["status"];

            //added by YAMA 2014/04/25
            jobnetExecInfo.load_status = (int)row["load_status"];

            // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
            // jobnetExecInfo.display_status = getRunJobStatusStr((int)row["status"], (int)row["load_status"]);
            jobnetExecInfo.start_pending_flag = (int)row["start_pending_flag"];
            jobnetExecInfo.display_status = getRunJobDisplayStatus((int)row["status"], (int)row["load_status"], (int)row["start_pending_flag"]);

            //added by YAMA 2014/04/25
            //jobnetExecInfo.status_color = getRunJobStatusColor((int)row["status"], (int)row["job_status"]);
            // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
            // jobnetExecInfo.status_color = getRunJobStatusOfColor((int)row["status"], (int)row["job_status"], (int)row["load_status"]);
            jobnetExecInfo.status_color = getRunJobStatusColor((int)row["status"], (int)row["job_status"], (int)row["load_status"], (int)row["start_pending_flag"]);

            // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
            //added by YAMA 2014/07/01
            // jobnetExecInfo.Foreground_color = getRunJobStatusOfChrColor((int)row["status"], (int)row["job_status"], (int)row["load_status"]);
            jobnetExecInfo.Foreground_color = getRunJobStatusOfChrColor((int)row["status"], (int)row["job_status"], (int)row["load_status"], (int)row["start_pending_flag"]);

            jobnetExecInfo.jobnet_name = row["jobnet_name"].ToString();

            //added by YAMA 2014/09/22 実行中ジョブID表示
            jobnetExecInfo.running_job_id = row["running_job_id"].ToString();
            jobnetExecInfo.running_job_name = row["running_job_name"].ToString();

            if (Convert.ToDecimal(row["scheduled_time"]) > 0)
            {
                jobnetExecInfo.scheduled_time = ConvertUtil.ConverIntYYYYMMDDHHMI2Date(Convert.ToDecimal(row["scheduled_time"])).ToString("yyyy/MM/dd HH:mm:ss");
                jobnetExecInfo.schedule_id = row["schedule_id"].ToString();

            }
            else
            {
                jobnetExecInfo.scheduled_time = "";
                jobnetExecInfo.schedule_id = "";
            }
            if (Convert.ToDecimal(row["start_time"]) > 0)
            {
                //added by YAMA 2014/04/25
                if ((LoadStausType)jobnetExecInfo.load_status != LoadStausType.Skip)
                {
                    jobnetExecInfo.start_time = ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(row["start_time"])).ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                }
            }
            else
            {
                jobnetExecInfo.start_time = "";
            }
            if (Convert.ToDecimal(row["end_time"]) > 0)
            {
                //added by YAMA 2014/04/25
                if ((LoadStausType)jobnetExecInfo.load_status != LoadStausType.Skip)
                {
                    jobnetExecInfo.end_time = ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(row["end_time"])).ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                }
            }
            else
            {
                jobnetExecInfo.end_time = "";
            }
            jobnetExecInfo.inner_jobnet_id = Convert.ToDecimal(row["inner_jobnet_id"]);

            jobnetExecInfo.jobnet_timeout = Convert.ToInt32(row["jobnet_timeout"]);
            if(row["jobnet_timeout"].Equals("1")){
                jobnetExecInfo.jobnet_timeout_run_type = "jobnet stop";
            }
            return jobnetExecInfo;

        }
        /// <summary>各リストの画面Gridの行番号を取得</summary>
        private int[] getRowNums()
        {
            int[] rowNumbers = new int[] { 0, 1, 2 };
            if (viewCount == 3)
            {
                rowNumbers[0] = 0;
                rowNumbers[1] = 1;
                rowNumbers[2] = 2;
            }
            if (viewCount == 2)
            {
                if (!viewJobnet)
                {
                    rowNumbers[0] = -1;
                    rowNumbers[1] = 0;
                    rowNumbers[2] = 1;
                }
                if (!viewErrJobnet)
                {
                    rowNumbers[0] = 0;
                    rowNumbers[1] = -1;
                    rowNumbers[2] = 1;
                }
                if (!viewRunningJobnet)
                {
                    rowNumbers[0] = 0;
                    rowNumbers[1] = 1;
                    rowNumbers[2] = -1;
                }
            }
            if (viewCount == 1)
            {
                if (viewJobnet)
                {
                    rowNumbers[0] = 0;
                    rowNumbers[1] = -1;
                    rowNumbers[2] = -1;
                }
                if (viewErrJobnet)
                {
                    rowNumbers[0] = -1;
                    rowNumbers[1] = 0;
                    rowNumbers[2] = -1;
                }
                if (viewRunningJobnet)
                {
                    rowNumbers[0] = -1;
                    rowNumbers[1] = -1;
                    rowNumbers[2] = 0;
                }
            }
            if (viewCount == 0)
            {
                rowNumbers[0] = -1;
                rowNumbers[1] = -1;
                rowNumbers[2] = -1;
            }
            return rowNumbers;
        }

        /// <summary>各リストの画面Gridの行番号を取得</summary>
        private String getRunJobStatusStr_(int status, int load_status)
        {
            String str;
            switch (status)
            {
                case 2:
                case 4:
                case 6:
                    str = Properties.Resources.job_run_status_running;
                    break;
                case 3:
                    str = Properties.Resources.job_run_status_done;
                    break;
                case 5:
                    str = Properties.Resources.job_run_status_done;
                    if (load_status==1)
                        str = Properties.Resources.load_err;
                    break;
                default:
                    str = Properties.Resources.job_run_status_schedule;
                    break;
            }
            return str;
        }

        //added by YAMA 2014/04/25
        private String getRunJobStatusStr(int status, int load_status)
        {
            String str;

            switch (status)
            {
                case 0:
                    str = Properties.Resources.job_run_status_schedule;
                    if (load_status == 2)
                        str = Properties.Resources.job_run_status_scheduled_wait;
                    break;
                case 1:
                    str = Properties.Resources.job_run_status_schedule;
                    break;
                case 2:
                case 4:
                    //str = Properties.Resources.job_run_status_running;
                    if (load_status == 2)
                    {
                        // 「『ｽﾃｰﾀｽ(status) = 実行中(2)』and『展開状況(load_status) = 遅延起動(2)』」の場合、『遅延起動エラー』を設定
                        str = Properties.Resources.job_run_status_delay_err;
                    }
                    else
                    {
                        str = Properties.Resources.job_run_status_running;
                    }

                    break;
                case 3:
                    str = Properties.Resources.job_run_status_done;
                    if (load_status == 3)
                        str = Properties.Resources.job_run_status_skip;
                    break;
                case 5:
                    str = Properties.Resources.job_run_status_done;
                    if (load_status == 1)
                    {
                        str = Properties.Resources.load_err;
                    }
                    //added by YAMA 2014/04/25
                    //else if (load_status == 2)
                    //{
                    //    str = Properties.Resources.job_run_status_delay_err;
                    //}
                    break;
                default:
                    str = Properties.Resources.job_run_status_schedule;
                    break;
            }
            return str;
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary>ステータスを設定</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="load_status">実行ジョブネット展開状況</param>
        /// <param name="start_pending_flag">実行ジョブネット起動保留フラグ</param>
        private String getRunJobDisplayStatus(int status, int load_status, int start_pending_flag)
        {
            String str;

            switch (status)
            {
                case 0:
                    // 「ステータス」が『未実行』の場合、表示優先順位は、「展開状況(load_status)」 ＜ 「起動保留フラグ(start_pending_flag)」
                    str = Properties.Resources.job_run_status_schedule;
                    if (load_status == 2)
                        str = Properties.Resources.job_run_status_scheduled_wait;
                    if (start_pending_flag == 1)
                        str = Properties.Resources.job_run_status_reserve;
                    break;
                case 1:
                    str = Properties.Resources.job_run_status_schedule;
                    break;
                case 2:
                case 4:
                    if (load_status == 2)
                    {
                        // 「『ｽﾃｰﾀｽ(status) = 実行中(2)』and『展開状況(load_status) = 遅延起動(2)』」の場合、『遅延起動エラー』を設定
                        str = Properties.Resources.job_run_status_delay_err;
                    }
                    else
                    {
                        str = Properties.Resources.job_run_status_running;
                    }
                    break;
                case 3:
                    str = Properties.Resources.job_run_status_done;
                    if (load_status == 3)
                        str = Properties.Resources.job_run_status_skip;
                    break;
                case 5:
                    str = Properties.Resources.job_run_status_done;
                    if (load_status == 1)
                    {
                        str = Properties.Resources.load_err;
                    }
                    break;
                default:
                    str = Properties.Resources.job_run_status_schedule;
                    break;
            }
            return str;
        }


        /// <summary>各リストの画面Gridの行番号を取得</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="run_type">実行ジョブネット実行種別</param>
        private SolidColorBrush getRunJobStatusColor(int status, int run_type)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.Aquamarine);
            switch (status)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                case 6:
                    switch (run_type)
                    {
                        case 0:
                            color = new SolidColorBrush(Colors.Yellow);
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 3:
                    switch (run_type)
                    {
                        case 0:
                            color = new SolidColorBrush(Colors.Lime);
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 4:
                case 5:
                    color = new SolidColorBrush(Colors.Red);
                    break;
            }
            return color;
        }



        //added by YAMA 2014/04/25  (実行スキップのグレイ色対応)
        /// <summary>ステータスに対応する色を設定</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="job_status">実行ジョブネットジョブ状況</param>
        /// <param name="load_status">実行ジョブネット展開状況</param>
        private SolidColorBrush getRunJobStatusOfColor(int status, int job_status, int load_status)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.Aquamarine);
            switch (status)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                case 6:
                    switch (job_status)
                    {
                        case 0:
                            color = new SolidColorBrush(Colors.Yellow);
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 3:
                    switch (job_status)
                    {
                        case 0:
                            if ((LoadStausType)load_status != LoadStausType.Skip)
                            {
                                color = new SolidColorBrush(Colors.Lime);
                            }
                            else
                            {
                                color = new SolidColorBrush(Colors.Gray);
                            }
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 4:
                case 5:
                    color = new SolidColorBrush(Colors.Red);
                    break;
            }
            return color;
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary>ステータスに対応する色を設定</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="job_status">実行ジョブネットジョブ状況</param>
        /// <param name="load_status">実行ジョブネット展開状況</param>
        /// <param name="start_pending_flag">実行ジョブネット起動保留フラグ</param>
        private SolidColorBrush getRunJobStatusColor(int status, int job_status, int load_status, int start_pending_flag)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.Aquamarine);
            switch (status)
            {
                case 0:
                    switch (start_pending_flag)
                    {
                        case 1:
                            color = new SolidColorBrush(Colors.Blue);
                            break;
                    }
                    break;
                case 1:
                    break;
                case 2:
                case 6:
                    switch (job_status)
                    {
                        case 0:
                            color = new SolidColorBrush(Colors.Yellow);
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 3:
                    switch (job_status)
                    {
                        case 0:
                            if ((LoadStausType)load_status != LoadStausType.Skip)
                            {
                                color = new SolidColorBrush(Colors.Lime);
                            }
                            else
                            {
                                color = new SolidColorBrush(Colors.Gray);
                            }
                            break;
                        case 1:
                            color = new SolidColorBrush(Colors.Orange);
                            break;
                        case 2:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    break;
                case 4:
                case 5:
                    color = new SolidColorBrush(Colors.Red);
                    break;
            }
            return color;
        }


        //added by YAMA 2014/07/01  (実行スキップ時の文字色対応)
        /// <summary>ステータスに対応する文字色を設定</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="job_status">実行ジョブネットジョブ状況</param>
        /// <param name="load_status">実行ジョブネット展開状況</param>
        private String getRunJobStatusOfChrColor(int status, int job_status, int load_status)
        {
            String color = "Black";
            switch (status)
            {
                case 0:                 // 0：未実行（初期値）
                    break;
                case 1:                 // 1：実行準備
                    break;
                case 2:                 // 2：実行中
                case 6:
                    switch (job_status)
                    {
                        case 0:             // 0：通常（初期値）
                            break;
                        case 1:             // 1：タイムアウト
                            break;
                        case 2:             // 2：エラー
                            break;
                    }
                    break;
                case 3:                 // 3：正常終了
                    switch (job_status)
                    {
                        case 0:             // 0：通常（初期値）
                            if ((LoadStausType)load_status != LoadStausType.Skip)
                            {
                                ;
                            }
                            else
                            {
                                color = "White";
                            }
                            break;
                        case 1:             // 1：タイムアウト
                            break;
                        case 2:             // 2：エラー
                            break;
                    }
                    break;
                case 4:
                case 5:                 // 5：異常終了
                    break;
            }
            return color;
        }


        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        /// <summary>ステータスに対応する文字色を設定</summary>
        /// <param name="status">実行ジョブネットステータス</param>
        /// <param name="job_status">実行ジョブネットジョブ状況</param>
        /// <param name="load_status">実行ジョブネット展開状況</param>
        /// <param name="start_pending_flag">実行ジョブネット起動保留フラグ</param>
        private String getRunJobStatusOfChrColor(int status, int job_status, int load_status, int start_pending_flag)
        {
            String color = "Black";
            switch (status)
            {
                case 0:     // 0：未実行（初期値）
                    switch (start_pending_flag)
                    {
                        case 1:
                            color = "White";
                            break;
                    }
                    break;
                case 1:     // 1：実行準備
                    break;
                case 2:     // 2：実行中
                case 6:
                    switch (job_status)
                    {
                        case 0:             // 0：通常（初期値）
                            break;
                        case 1:             // 1：タイムアウト
                            break;
                        case 2:             // 2：エラー
                            break;
                    }
                    break;
                case 3:     // 3：正常終了
                    switch (job_status)
                    {
                        case 0:             // 0：通常（初期値）
                            if ((LoadStausType)load_status != LoadStausType.Skip)
                            {
                                ;
                            }
                            else
                            {
                                color = "White";
                            }
                            break;
                        case 1:             // 1：タイムアウト
                            break;
                        case 2:             // 2：エラー
                            break;
                    }
                    break;
                case 4:
                case 5:     // 5：異常終了
                    break;
            }
            return color;
        }


        #endregion


    }
}
