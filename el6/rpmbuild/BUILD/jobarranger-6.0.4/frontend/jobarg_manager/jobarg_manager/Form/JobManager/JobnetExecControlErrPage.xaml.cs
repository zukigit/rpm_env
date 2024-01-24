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
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// JobnetExecControlErrPage.xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecControlErrPage : BaseUserControl
    {
        #region コンストラクタ
        public JobnetExecControlErrPage(JobnetExecControlPage parent)
        {
            InitializeComponent();
            DataContext = this;
            _parent = parent;
            _errSelectedInnerJobnetId = "";
            _errSelectedInnerJobnetIds = new List<string>();
            error_label.Text = "[" + Convert.ToString(listView1.SelectedItems.Count) + "]";
        }
        #endregion

        #region プロパティ
        /// <summary>エラージョブネット一覧</summary>
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
                return "JobnetExecControlErrPage";
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
        private String _errSelectedInnerJobnetId;
        public String ErrSelectedInnerJobnetId
        {
            get
            {
                return _errSelectedInnerJobnetId;
            }
        }
        private List<string> _errSelectedInnerJobnetIds;
        public List<string> ErrSelectedInnerJobnetIds
        {
            get
            {
                return _errSelectedInnerJobnetIds;
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
            //SelectItemOnRightClick(e);
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
            //ErrorListAddAndRemove(2);

            hideContextMenu.IsEnabled = false;
            stopContextMenu.IsEnabled = false;

            //added by YAMA 2014/04/25
            DelayedContextMenu.IsEnabled = false;
            int listCont = ErrSelectedInnerJobnetIds.Count;
            int listNum = 0;
            if (listCont > listView1.SelectedItems.Count)
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)listView1.Items[i];
                    for (int j = 0; j < _errSelectedInnerJobnetIds.Count; j++)
                    {
                        if(_errSelectedInnerJobnetIds[j].Equals(Convert.ToString(jobnetExecInfo.inner_jobnet_id)))
                        {
                            listView1.SelectedItems.Add(jobnetExecInfo);
                            listNum++;
                            break;
                        }
                    }
                    if(listCont == listNum)
                    {
                        break;
                    }
                }
            }

            // 何らかのアイテムを選択した状態のとき
            if (listView1.SelectedItems.Count > 0)
            {
                hideContextMenu.IsEnabled = true;
                int status_flag = 0;
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    JobnetExecInfo jobnetExecInfo = (JobnetExecInfo)listView1.SelectedItems[i];
                    hideContextMenu.Tag = jobnetExecInfo;
                    if ((RunJobStatusType)jobnetExecInfo.status == RunJobStatusType.During
                         && ((status_flag == 0) ||  (status_flag == 1))
                        )
                    {
                        stopContextMenu.IsEnabled = true;
                        //stopContextMenu.Tag = jobnetExecInfo;
                        status_flag = 1;
                    }

                    //added by YAMA 2014/04/25
                    // 展開状況が「遅延起動」
                    if ((LoadStausType)jobnetExecInfo.load_status == LoadStausType.Delay
                        && listView1.SelectedItems.Count == 1
                        )
                    {
                        DelayedContextMenu.IsEnabled = true;
                        DelayedContextMenu.Tag = jobnetExecInfo;
                        status_flag = 1;
                    }

                }
            }
            #if VIEWER
                this.stopContextMenu.Visibility = Visibility.Collapsed;
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
                JobnetExecInfo selected = (JobnetExecInfo)listView1.SelectedItems[listView1.SelectedItems.Count-1];
                _errSelectedInnerJobnetId = Convert.ToString(selected.inner_jobnet_id);
                //Console.WriteLine("listView1.SelectedItems.Count=[" + listView1.SelectedItems.Count + "] ");
                //Console.WriteLine("_errSelectedInnerJobnetIds.Count=[" + _errSelectedInnerJobnetIds.Count + "] ");
                //Console.WriteLine("_errSelectedInnerJobnetId=[" + _errSelectedInnerJobnetId + "] ");
                if (_errSelectedInnerJobnetIds.Contains(_errSelectedInnerJobnetId) == false)
                {
                    _errSelectedInnerJobnetIds.Add(Convert.ToString(selected.inner_jobnet_id));
                    //_errSelectedInnerJobnetIds.Remove(Convert.ToString(selected.inner_jobnet_id));
                }
            }

        }
        //*******************************************************************
        /// <summary>リスト行選択・外した場合</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ErrorListAddAndRemove(2);
            ErrorListAddAndRemove(1);
        }

        //*******************************************************************
        /// <summary>リスト行選択・外した場合の処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e"></param>
        //*******************************************************************
        private void ErrorListAddAndRemove(int run_flag)
        {
            //_errSelectedInnerJobnetIds remove
            if (run_flag == 1)
            {
                for (int i = 0; i < _errSelectedInnerJobnetIds.Count; i++)
                {
                    bool delete_flag = true;
                    for (int j = 0; j < listView1.SelectedItems.Count; j++)
                    {
                        JobnetExecInfo deleteJonnet = (JobnetExecInfo)listView1.SelectedItems[j];
                        if (Convert.ToString(deleteJonnet.inner_jobnet_id).Equals(_errSelectedInnerJobnetIds[i]))
                        {
                            //Console.WriteLine("ListView_MouseLeftButtonUp=[" + track.inner_jobnet_id + "] ");
                            delete_flag = false;
                            break;
                        }

                    }
                    if (delete_flag)
                    {
                        //Console.WriteLine(" Remove =[" + _errSelectedInnerJobnetIds[i] + "] ");
                        _errSelectedInnerJobnetIds.Remove(_errSelectedInnerJobnetIds[i]);
                    }
                }
            }
            //_errSelectedInnerJobnetIds ADD
            if (run_flag == 2)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    bool delete_flag = true;
                    String jobnetid = "";
                    JobnetExecInfo insertJonnet = (JobnetExecInfo)listView1.SelectedItems[i];
                    jobnetid = Convert.ToString(insertJonnet.inner_jobnet_id);
                    for (int j = 0; j < _errSelectedInnerJobnetIds.Count; j++)
                    {
                        if (jobnetid.Equals(_errSelectedInnerJobnetIds[j]))
                        {
                            delete_flag = false;
                            break;
                        }

                    }
                    if (delete_flag && jobnetid.Length > 0)
                    {
                        //Console.WriteLine(" ADD =[" + jobnetid + "] ");
                        _errSelectedInnerJobnetIds.Add(jobnetid);
                    }
                }
            }
            error_label.Text = "[" + Convert.ToString(listView1.SelectedItems.Count) + "]";
        }

        private void AllSelect(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(" AllSelect ");
            _errSelectedInnerJobnetIds.Clear();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                JobnetExecInfo insertJonnet = (JobnetExecInfo)listView1.Items[i];
                listView1.SelectedItems.Add(insertJonnet);
                _errSelectedInnerJobnetIds.Add(Convert.ToString(insertJonnet.inner_jobnet_id));
            }
            error_label.Text = "[" + Convert.ToString(listView1.SelectedItems.Count) + "]";
        }

        private void AllClear(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(" AllClear ");
            listView1.SelectedItems.Clear();
            _errSelectedInnerJobnetIds.Clear();
            error_label.Text = "[" + Convert.ToString(listView1.SelectedItems.Count) + "]";
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {/*
            if (e.Key == Key.Up)
            {
                listView1.SelectedIndex = listView1.SelectedIndex - 1;
                if (listView1.SelectedIndex < 0)
                    listView1.SelectedIndex = 0;
            }

            if (e.Key == Key.Down)
            {
                listView1.SelectedIndex = listView1.SelectedIndex + 1;
            }
            e.Handled = true;
          */
        }
        #endregion

        #region privateメソッド

        /// <summary>リスト右マウスクリック</summary>
        /// <param name="e">イベント</param>
        private void SelectItemOnRightClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            /*
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
             * */
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
