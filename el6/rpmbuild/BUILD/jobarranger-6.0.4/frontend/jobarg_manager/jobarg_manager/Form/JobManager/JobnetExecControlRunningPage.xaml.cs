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
using System.Collections.ObjectModel;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;

//added by YAMA 2014/04/25
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// JobnetExecControlRunningPage.xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecControlRunningPage : BaseUserControl
    {
        #region コンストラクタ
        public JobnetExecControlRunningPage(JobnetExecControlPage parent)
        {
            InitializeComponent();
            DataContext = this;
            _parent = parent;
            _runningSelectedInnerJobnetId = "";
        }
        #endregion

        #region プロパティ
        /// <summary>実行中ジョブネット一覧</summary>
        public ObservableCollection<JobnetExecInfo> JobnetExecList { get; set; }

        /// <summary>実行管理画面</summary>
        private JobnetExecControlPage _parent;
        public JobnetExecControlPage Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "JobnetExecControlRunningPage";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_300;
            }
        }

        /// <summary>選択された内部実行管理ＩＤ</summary>
        private String _runningSelectedInnerJobnetId;
        public String RunningSelectedInnerJobnetId
        {
            get
            {
                return _runningSelectedInnerJobnetId;
            }
        }
        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>リストPreview右マウスダウン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
            e.Handled = true;
        }

        //*******************************************************************
        /// <summary>リストPreview右マウスUP</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
            this.contextMenu.PlacementTarget = sender as UIElement;
            this.contextMenu.IsOpen = true;
        }
        //*******************************************************************
        /// <summary>リスト右マウスダウン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void FriendsListViewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectItemOnRightClick(e);
        }

        //*******************************************************************
        /// <summary>コンテキストメニューオープン</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void listView_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            hideContextMenu.IsEnabled = false;

            //added by YAMA 2014/04/25
            DelayedContextMenu.IsEnabled = false;

            // 何らかのアイテムを選択した状態のとき
            if (listView1.SelectedItems.Count > 0)
            {
                stopContextMenu.IsEnabled = true;
                JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)listView1.SelectedItems[0];
                stopContextMenu.Tag = jobnetExecInfo;

                //added by YAMA 2014/04/25
                // 展開状況が「遅延起動」
                if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay)
                {
                    DelayedContextMenu.IsEnabled = true;
                    DelayedContextMenu.Tag = jobnetExecInfo;
                }

            }
            else
            {
                stopContextMenu.IsEnabled = false;
                //added by YAMA 2014/04/25
                DelayedContextMenu.IsEnabled = false;

            }
            #if VIEWER
                this.contextMenu.Visibility = System.Windows.Visibility.Hidden;
            #endif
        }

        //*******************************************************************
        /// <summary>リスト行データをダブルクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JobnetExecInfo jobnetExecInfo = ((ListViewItem)sender).Content as JobnetExecInfo;
            if (jobnetExecInfo != null) Parent.ViewDetail(jobnetExecInfo.inner_jobnet_id);

        }

        //*******************************************************************
        /// <summary>リスト行選択が変更された場合</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                JobnetExecInfo selected = (JobnetExecInfo)listView1.SelectedItems[0];
                _runningSelectedInnerJobnetId = Convert.ToString(selected.inner_jobnet_id);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    listView1.SelectedIndex = listView1.SelectedIndex - 1;
                    if (listView1.SelectedIndex < 0)
                        listView1.SelectedIndex = 0;
                }
            }

            if (e.Key == Key.Down)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    listView1.SelectedIndex = listView1.SelectedIndex + 1;
                }
            }
            e.Handled = true;
        }

        #endregion

        #region privateメソッド

        /// <summary>リスト右マウスクリック</summary>
        /// <param name="e">イベント</param>
        private void SelectItemOnRightClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
            {
                Point clickPoint = e.GetPosition(listView1);
                object element = listView1.InputHitTest(clickPoint);
                if (element != null)
                {
                    ListViewItem clickedListViewItem = GetVisualParent<ListViewItem>(element);
                    if (clickedListViewItem != null)
                    {
                        listView1.SelectedItem = clickedListViewItem.Content;
                    }
                    else
                    {
                        listView1.SelectedItem = null;
                    }

                }
            }
        }

        /// <summary>親オブジェクト取得</summary>
        /// <param name="childObject">子オブジェクト</param>
        private T GetVisualParent<T>(object childObject) where T : Visual
        {
            DependencyObject child = childObject as DependencyObject;
            while ((child != null) && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }
        #endregion
    }
}
