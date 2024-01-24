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
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/09 新規作成<BR>                            *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <>
    /// Container.xaml の相互作用ロジック
    /// </>
    public partial class Container : UserControl, IContainer
    {
        #region フィールド
        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect _dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary> 実行ジョブ管理テーブル </summary>
        private RunJobDAO _runJobControlDAO;

        // added by YAMA 2014/11/05    拡張ジョブアイコンの再実行
        /// <summary> 実行拡張ジョブアイコン設定テーブル </summary>
        private RunIconExtJobDAO _runIconExtJobDAO;

        // added by Park.iggy 2014/11/05    ジョブネット停止のため
        /// <summary> 実行ジョブネットサマリ管理テーブル </summary>
        private RunJobnetSummaryDAO _runJobnetSummaryDAO;

        /// <>マウスの位置</>
        private Point mousePosition;

        private bool _viewContextMenu = true;
        #endregion

        #region コンストラクタ
        public Container()
        {
            // 初期化
            InitializeComponent();
            _runJobControlDAO = new RunJobDAO(_dbAccess);
        }
        #endregion

        #region プロパティ

        /// <>画布</>
        public Canvas ContainerCanvas
        {
            get
            {
                return cnsDesignerContainer;
            }
        }

        /// <>ウィンドウ</>
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

        /// <>画布内のアイテム</>
        private Hashtable _jobItems;
        public Hashtable JobItems
        {
            get
            {
                if (this._jobItems == null)
                {
                    this._jobItems = new Hashtable();
                }
                return this._jobItems;
            }
        }

        /// <summary>設定済ジョブＩＤ</summary>
        private Hashtable _setedjobIds;
        public Hashtable SetedJobIds
        {
            get
            {
                if (this._setedjobIds == null)
                {
                    this._setedjobIds = new Hashtable();
                }
                return this._setedjobIds;
            }
            set
            {
                _setedjobIds = value;
            }
        }

        /// <>画布の空白の場所にクリックフラグ</>
        bool _canvasClickFlg = true;
        public bool CanvasClickFlg
        {
            get
            {
                return _canvasClickFlg;
            }
            set
            {
                _canvasClickFlg = value;
            }
        }

        /// <>マウスがコンテナ内かどうかのフラグ</>
        bool mouseIsInContainer = false;
        public bool MouseIsInContainer
        {
            get
            {
                return mouseIsInContainer;
            }
            set
            {
                mouseIsInContainer = value;
            }
        }

        /// <> 選択コントローラリスト</>
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

        /// <>ジョブネットID</>
        private string _jobnetId;
        public string JobnetId
        {
            get
            {
                return _jobnetId;
            }
            set
            {
                _jobnetId = value;
            }
        }

        /// <>仮更新日</>
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
        /// <>ドラッグのジョブネットID</>
        private string _dragJobnetId;
        public string DragJobnetId
        {
            get
            {
                return _dragJobnetId;
            }
            set
            {
                _dragJobnetId = value;
            }
        }

        /// <>シフトキー押下状態フラグ</>
        public bool ShiftKeyIsPress
        {
            get
            {
                return (Keyboard.Modifiers == ModifierKeys.Shift);
            }
        }

        /// <summary>シフトキーで複数選択されたか判定フラグ</summary>
        private bool _isSelectedByShiftKey;
        public bool IsSelectedByShiftKey
        {
            get
            {
                return _isSelectedByShiftKey;
            }
            set
            {
                _isSelectedByShiftKey = value;
            }
        }

        /// <summary>zoom value</summary>
        public double ZoomValue
        {
            get
            {
                return (double)zoomSlider.Value;
            }
        }

        #region データ格納場所

        /// <summary>ジョブネットアイコン設定テーブル</summary>
        public DataTable JobnetControlTable { get; set; }

        /// <summary>ジョブ管理テーブル</summary>
        public DataTable JobControlTable { get; set; }

        /// <summary>フロー管理テーブル</summary>
        public DataTable FlowControlTable { get; set; }

        /// <summary>計算アイコン設定テーブル</summary>
        public DataTable IconCalcTable { get; set; }

        /// <summary>終了アイコン設定テーブル</summary>
        public DataTable IconEndTable { get; set; }

        /// <summary>拡張ジョブアイコン設定テーブル</summary>
        public DataTable IconExtjobTable { get; set; }

        // <summary>条件分岐アイコン設定テーブル</summary>
        public DataTable IconIfTable { get; set; }

        // <summary>情報取得アイコン設定テーブル</summary>
        public DataTable IconInfoTable { get; set; }

        // <summary>ジョブネットアイコン設定テーブル</summary>
        public DataTable IconJobnetTable { get; set; }

        // <summary>ジョブアイコン設定テーブル</summary>
        public DataTable IconJobTable { get; set; }

        // <summary>ジョブコマンド設定テーブル</summary>
        public DataTable JobCommandTable { get; set; }

        // <summary>ジョブ変数設定テーブル</summary>
        public DataTable ValueJobTable { get; set; }

        // <summary>ジョブコントローラ変数設定テーブル</summary>
        public DataTable ValueJobConTable { get; set; }

        // <summary>タスクアイコン設定テーブル</summary>
        public DataTable IconTaskTable { get; set; }

        // <summary>ジョブコントローラ変数アイコン設定テーブル</summary>
        public DataTable IconValueTable { get; set; }

        // <summary>ジョブコントローラ変数定義テーブル</summary>
        public DataTable DefineValueJobconTable { get; set; }

        // <summary>拡張ジョブ定義テーブル</summary>
        public DataTable DefineExtJobTable { get; set; }

        // <summary>ファイル転送アイコン設定テーブル</summary>
        public DataTable IconFcopyTable { get; set; }

        // <summary>ファイル待ち合わせアイコン設定テーブル</summary>
        public DataTable IconFwaitTable { get; set; }

        // <summary>リブートアイコン設定テーブル</summary>
        public DataTable IconRebootTable { get; set; }

        // <summary>保留解除アイコン設定テーブル</summary>
        public DataTable IconReleaseTable { get; set; }

        //added by YAMA 2014/02/06
        // <summary>Zabbix連携アイコン設定テーブル</summary>
        public DataTable IconCooperationTable { get; set; }

        //added by YAMA 2014/05/19
        // <summary>エージェントレスアイコン設定テーブル</summary>
        public DataTable IconAgentlessTable { get; set; }


        #endregion

        public double getLeftX()
        {
            if (CurrentSelectedControlCollection.Count < 1)
                return -1;

            double leftX = -1;

            // 始め
            //_leftXOfSelection = ((CommonItem)CurrentSelectedControlCollection[0]).CenterPoint.X;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is CommonItem)
                {
                    if (leftX == -1
                        || leftX > ((CommonItem)CurrentSelectedControlCollection[i]).CenterPoint.X)
                        leftX = ((CommonItem)CurrentSelectedControlCollection[i]).CenterPoint.X;
                }
            }

            return leftX;
        }

        public double getTopY()
        {
            if (CurrentSelectedControlCollection.Count < 1)
                return -1;

            double topY = -1;

            // 始め
            //topY = ((CommonItem)CurrentSelectedControlCollection[0]).CenterPoint.Y;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is CommonItem)
                {
                    if (topY == -1
                        || topY > ((CommonItem)CurrentSelectedControlCollection[i]).CenterPoint.Y)
                        topY = ((CommonItem)CurrentSelectedControlCollection[i]).CenterPoint.Y;
                }
            }

            return topY;
        }

        #endregion

        #region イベント


        //*******************************************************************
        /// <>コンテナをを左クリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void Container_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                // 選択をクリア
                if (_canvasClickFlg)
                    ClearSelectFlowElement(null);

                FrameworkElement element = sender as FrameworkElement;
                mousePosition = e.GetPosition(element);

                CurrentSelectedControlCollection.Clear();
                object item = element.InputHitTest(mousePosition);
                DependencyObject child = item as DependencyObject;
                while ((child != null) && !(child is IRoom))
                {
                    child = VisualTreeHelper.GetParent(child);
                }

                if (child != null)
                {
                    AddSelectedControl(child as Control);
                    contextMenu.IsOpen = false;
                    contextMenu.Visibility = Visibility.Visible;
                    e.Handled = true;
                    _viewContextMenu = true;
                }
                else
                {
                    CurrentSelectedControlCollection.Clear();
                    contextMenu.Visibility = Visibility.Hidden;
                    _viewContextMenu = false;
                }

            }
        }


        //*******************************************************************
        /// <>コンテナを右クリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ContextMenu.Visibility = Visibility.Visible;
        }


        //*******************************************************************
        /// <>左マウスの解放処理</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(element);

        }

        //*******************************************************************
        /// <>コンテナに入れる</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseIsInContainer = true;
        }

        //*******************************************************************
        /// <>コンテナを離れる</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseIsInContainer = false;
        }

        #region 部品のドラッグ処理


        #endregion

        #region メニューコンテクストのイベント


        //*******************************************************************
        /// <>コンテクストメニューを表示</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void ContextMenu_Open(object sender, RoutedEventArgs e)
        {
            if (!_viewContextMenu)
            {
                e.Handled = true;
                _viewContextMenu = true;
            }
            else
            {
                SetContextStatus();
            }
        }

        //*******************************************************************
        /// <>保留をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemHold_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemHold_Click", Consts.PROCESS_018);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetHold((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemHold_Click", Consts.PROCESS_018);
        }

        //*******************************************************************
        /// <>保留解除をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemUnHold_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemUnHold_Click", Consts.PROCESS_019);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetUnHold((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemUnHold_Click", Consts.PROCESS_019);
        }

        //*******************************************************************
        /// <>スキップをクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemSkip_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemSkip_Click", Consts.PROCESS_020);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetSkip((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemSkip_Click", Consts.PROCESS_020);

        }

        //*******************************************************************
        /// <>スキップ解除をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemUnSkip_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemUnSkip_Click", Consts.PROCESS_021);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetUnSkip((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemUnSkip_Click", Consts.PROCESS_021);
        }

        //*******************************************************************
        /// <>強制停止をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemStop_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemStop_Click", Consts.PROCESS_022);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetStop((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemStop_Click", Consts.PROCESS_022);

        }

        //*******************************************************************
        /// <>再実行をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemReStart_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteStartLog("MenuitemReStart_Click", Consts.PROCESS_023);

            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            SetReStart((IRoom)selectItems[0]);

            // 終了ログ
            ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)ParantWindow).WriteEndLog("MenuitemReStart_Click", Consts.PROCESS_023);
        }

        //*******************************************************************
        /// <>変数値変更をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemParameter_Click(object sender, RoutedEventArgs e)
        {
            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            ShowParameterSetting((IRoom)selectItems[0]);
        }

        //*******************************************************************
        /// <>変数値表示をクリック</>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void MenuitemView_Click(object sender, RoutedEventArgs e)
        {
            List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
            ShowParameterView((IRoom)selectItems[0]);
        }


        //*******************************************************************
        /// <>設定をクリック</>
        /// <param name="lineType">線のタイプ</param>
        /// <param name="item1">開始ジョブ</param>
        /// <param name="item2">終了ジョブ</param>
        /// <param name="flowType">フローのタイプ(True、False)</param>
        /// <param name="editType">編集タイプ</param>
        //*******************************************************************
        public void MakeFlow(FlowLineType lineType, IRoom item1, IRoom item2, int flowType, Consts.EditType editType)
        {
            Point startPos;
            Point endPos;
            IFlow flow = null;
            double flowWith = 0;

            // 直線の場合

            if (FlowLineType.Line.Equals(lineType))
            {
                flow = new Flow();

                List<Point> connectPoints = CommonUtil.GetConnectPoints(item1, item2);

                if (connectPoints == null || connectPoints.Count != 2)
                {
                    return;
                }

                startPos = connectPoints[0];
                endPos = connectPoints[1];

                flow.BeginPosition = connectPoints[0];
                flow.EndPosition = connectPoints[1];
                if (editType == Consts.EditType.READ)
                {
                    ((Flow)flow).RemoveAllEvent();
                }
            }
            // 曲線の場合

            else if (FlowLineType.Curve.Equals(lineType))
            {
                flow = new FlowArc();

                List<Point> connectPoints = CommonUtil.GetConnectPointsForCurve(item1, item2);

                if (connectPoints == null || connectPoints.Count != 2)
                {
                    return;
                }

                flow.BeginPosition = connectPoints[0];
                flow.EndPosition = connectPoints[1];

                // フロー幅 = 半径
                flowWith = Point.Subtract(connectPoints[0], connectPoints[1]).Length;
                ((FlowArc)flow).arrow.Radius = flowWith * 0.65;
                ((FlowArc)flow).arrow.LineSweepDirection = SweepDirection.Clockwise;
                ((FlowArc)flow).arrow.InitCenterPosition();
                if (editType == Consts.EditType.READ)
                {
                    ((FlowArc)flow).RemoveAllEvent();
                }
            }
            else
                return;

            AddFlow(flow, item1, item2, flowWith, editType);

            item1.AddBeginFlow(flow);
            item2.AddEndFlow(flow);

            flow.BeginItem = item1;
            flow.EndItem = item2;

            // フロータイプをセット
            if (FlowType.TRUE == (FlowType)flowType)
                flow.SetTrue(editType);
            if (FlowType.FALSE == (FlowType)flowType)
                flow.SetFalse(editType);

        }
        #endregion

        #endregion

        #region publicメッソド

        //*******************************************************************
        /// <>選択モジュールを追加</>
        /// <param name="uc">部品</param>
        //*******************************************************************
        public void AddSelectedControl(System.Windows.Controls.Control uc)
        {
            if (!CurrentSelectedControlCollection.Contains(uc))
            {
                _currentSelectedControlCollection.Add(uc);
            }

        }

        //*******************************************************************
        /// <>フローを追加</>
        /// <param name="flow">フロー</param>
        /// <param name="item1">開始ジョブ/param>
        /// <param name="item2">終了ジョブ/param>
        /// <param name="flowWith">曲線幅(半径)/param>
        //*******************************************************************
        public void AddFlow(IFlow flow, IRoom item1, IRoom item2, double flowWith, Consts.EditType editType)
        {
            cnsDesignerContainer.Children.Add((UIElement)flow);
            flow.Container = this;

            // データを追加
            if (Consts.EditType.Add == editType)
            {
                DataRow row = FlowControlTable.NewRow();
                //ジョブネットID
                row["jobnet_id"] = _jobnetId;
                // 開始ジョブID
                row["start_job_id"] = item1.JobId;
                // 終了ジョブID
                row["end_job_id"] = item2.JobId;
                // 更新日
                row["update_date"] = _tmpUpdDate;
                // フロータイプ(0：通常（初期値）)
                row["flow_type"] = 0;
                // フロー幅

                // 左曲線の場合
                if (flow.BeginPosition.Y > flow.EndPosition.Y)
                    flowWith = 0 - flowWith;

                row["flow_width"] = Convert.ToInt16(flowWith);

                FlowControlTable.Rows.Add(row);
            }
        }

        //*******************************************************************
        /// <>特定の部品を選択</>
        /// <param name="uc">部品</param>
        /// <param name="isSelected">選択フラグ</param>
        //*******************************************************************
        public void SetWorkFlowElementSelected(System.Windows.Controls.Control uc, bool isSelected)
        {
            CurrentSelectedControlCollection.Clear();
            AddSelectedControl(uc);
        }

        //*******************************************************************
        /// <>特定の部品選択状態をはずす</>
        /// <param name="uc">部品</param>
        //*******************************************************************
        public void RemoveSelectedControl(System.Windows.Controls.Control uc)
        {
            if (CurrentSelectedControlCollection.Contains(uc))
            {
                CurrentSelectedControlCollection.Remove(uc);
            }
        }

        //*******************************************************************
        /// <>部品選択状態をはずす</>
        /// <param name="uc">部品（nullの場合、全部をはずす）</param>
        //*******************************************************************
        public void ClearSelectFlowElement(System.Windows.Controls.Control uc)
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;

            CurrentSelectedControlCollection.Clear();

            if (uc != null)
            {
                if (!(uc is IFlow))
                    ((IRoom)uc).IsSelectd = true;
                else
                    ((IFlow)uc).IsSelectd = true;
                AddSelectedControl(uc);
            }
            mouseIsInContainer = true;
        }

        //*******************************************************************
        /// <>選択した部品の移動処理（uc以外）</>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="uc">部品</param>
        //*******************************************************************
        public void MoveControlCollectionByDisplacement(double x, double y, UserControl uc)
        {

        }
        /// <summary>アイコンのTRUE、FALSE位置設定</summary>
        /// <param name="uc">例外のアイコン/param>
        public void SetControlCollectionItemAndRuleNameControlPosition(UserControl uc)
        {
        }

        //*******************************************************************
        /// <>コンテナに部品を含むかどうかの判定</>
        /// <param name="uie">部品</param>
        //*******************************************************************
        public bool Contains(UIElement uie)
        {
            return cnsDesignerContainer.Children.Contains(uie);
        }

        //*******************************************************************
        /// <>選択部品を削除</>
        //*******************************************************************
        public void DeleteSelectedControl()
        {
            if (_currentSelectedControlCollection == null
                || _currentSelectedControlCollection.Count != 1
                || !(_currentSelectedControlCollection[0] is IRoom))
                return;

            // ジョブを削除
            CommonItem item = (CommonItem)_currentSelectedControlCollection[0];

            string jobid = item.JobId;

            DataRow[] rows = JobControlTable.Select("job_id='" + jobid + "'");

            // アイコン設定テーブルにデータを削除
            ElementType type = (ElementType)(Convert.ToInt16(rows[0]["job_type"]));
            DeleteIconSetting(jobid, type);

            // ジョブ管理テーブルから削除
            if (rows != null && rows.Count() > 0)
                rows[0].Delete();

            //JobControlTable.AcceptChanges();

            // フローを削除
            DataRow[] rowsFlow = FlowControlTable.Select("start_job_id='" + jobid + "' or end_job_id='" + jobid + "'");

            foreach (DataRow row in rowsFlow)
                row.Delete();

            //FlowControlTable.AcceptChanges();

            // アイコンを削除
            ((IRoom)_currentSelectedControlCollection[0]).Delete();
            _currentSelectedControlCollection.Remove(item);

            _jobItems.Remove(jobid);

        }

        //*******************************************************************
        /// <>選択フローを削除</>
        //*******************************************************************
        public void DeleteSeletedFlow()
        {
            IFlow flow = (IFlow)_currentSelectedControlCollection[0];

            string beginJobId = flow.BeginItem.JobId;
            string endJobId = flow.EndItem.JobId;

            DataRow[] rows = FlowControlTable.Select("start_job_id='" + beginJobId + "' and end_job_id='" + endJobId + "'");

            if (rows != null && rows.Count() > 0)
                rows[0].Delete();

            RemoveFlow(flow);
        }

        //*******************************************************************
        /// <>特定の部品を削除</>
        /// <param name="uie">部品</param>
        //*******************************************************************
        public void RemoveItem(Control a)
        {
            if (cnsDesignerContainer.Children.Contains(a))
                cnsDesignerContainer.Children.Remove(a);
        }

        //*******************************************************************
        /// <>特定のフローを削除</>
        /// <param name="a">アイコン</param>
        //*******************************************************************
        public void RemoveFlow(IFlow a)
        {
            if (cnsDesignerContainer.Children.Contains((UIElement)a))
                cnsDesignerContainer.Children.Remove((UIElement)a);
        }

        #endregion

        #region privateメッソド


        #region コンテクストの制限処理


        //*******************************************************************
        /// <>コンテクスの利用可能をセット</>
        //*******************************************************************
        private void SetContextStatus()
        {
            menuitemHold.IsEnabled = true;
            menuitemUnHold.IsEnabled = true;
            menuitemSkip.IsEnabled = true;
            menuitemUnSkip.IsEnabled = true;
            menuitemStop.IsEnabled = true;
            menuitemReStart.IsEnabled = true;
            menuitemParameter.IsEnabled = true;

            //Viewerの場合
            #if VIEWER
                menuitemHold.IsEnabled = false;
                menuitemUnHold.IsEnabled = false;
                menuitemSkip.IsEnabled = false;
                menuitemUnSkip.IsEnabled = false;
                menuitemStop.IsEnabled = false;
                menuitemReStart.IsEnabled = false;
                menuitemParameter.IsEnabled = false;
                return;
            #endif

            // アイコン未選択、または一つ以外のアイコンを選択
            if (_currentSelectedControlCollection == null
                || _currentSelectedControlCollection.Count != 1
                || ((JobnetExecDetail)ParantWindow).JobnetRunStatus == RunJobStatusType.Normal
                || ((JobnetExecDetail)ParantWindow).JobnetRunStatus == RunJobStatusType.Abnormal)
            {
                menuitemHold.IsEnabled = false;
                menuitemUnHold.IsEnabled = false;
                menuitemSkip.IsEnabled = false;
                menuitemUnSkip.IsEnabled = false;
                menuitemStop.IsEnabled = false;
                menuitemReStart.IsEnabled = false;
                menuitemParameter.IsEnabled = false;
                return;

            }

            IElement item = ((IRoom)_currentSelectedControlCollection[0]).ContentItem;
            string job = item.InnerJobId;
            DataRow[] existJob = JobControlTable.Select("inner_job_id=" + job);
            // 保留可能判定
            if (!IsHoldEnable(item, existJob[0]))
            {
                menuitemHold.IsEnabled = false;
            }

            // 保留解除可能判定
            if (!IsUnHoldEnable(item, existJob[0]))
            {
                menuitemUnHold.IsEnabled = false;
            }

            // スキップ可能判定
            if (!IsSkipEnable(item, existJob[0]))
            {
                menuitemSkip.IsEnabled = false;
            }

            // スキップ解除可能判定
            if (!IsUnSkipEnable(item, existJob[0]))
            {
                menuitemUnSkip.IsEnabled = false;
            }

            // 強制停止可能判定
            if (!IsStopEnable(item, existJob[0]))
            {
                menuitemStop.IsEnabled = false;
            }

            // 再実行可能判定
            if (!IsReStartEnable(item, existJob[0]))
            {
                menuitemReStart.IsEnabled = false;
            }

            // 変数値変更可能判定
            if (!IsParameterEnable(item, existJob[0]))
            {
                menuitemParameter.IsEnabled = false;
            }



        }

        //*******************************************************************
        /// <>保留可能判定</>
        //*******************************************************************
        private bool IsHoldEnable(IElement item, DataRow existJob)
        {
            // 開始アイコンの場合利用不可
            if (item is Start)
                return false;

            //ステータスが未実行以外の場合利用不可
            if (!((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.None))
                return false;
            //処理フラグが保留の場合利用不可
            if (((RunJobMethodType)existJob["method_flag"]).Equals(RunJobMethodType.HOLD))
                return false;

            //Park.iggy 追加 START
            //親ジョブネットがジョブネット停止が行った場合利用不可
            if (IsJobnetAbortFlag(existJob))
                return false;

            //Park.iggy 追加 END

            return true;
        }

        //*******************************************************************
        /// <>保留解除可能判定</>
        //*******************************************************************
        private bool IsUnHoldEnable(IElement item, DataRow existJob)
        {

            //処理フラグが保留以外の場合利用不可
            if (!((RunJobMethodType)existJob["method_flag"]).Equals(RunJobMethodType.HOLD))
                return false;
            //ステータスが未実行、実行準備以外の場合利用不可
            if (!(((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.None) || ((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.Prepare)))
                return false;
            return true;
        }

        //*******************************************************************
        /// <>スキップ可能判定</>
        //*******************************************************************
        private bool IsSkipEnable(IElement item, DataRow existJob)
        {
            // 開始、終了、条件分岐、並行処理、ループアイコンの場合利用不可
            if (item is Start || item is End || item is If || item is Ife || item is Mts || item is Mte || item is Loop)
                return false;

            //ステータスが未実行、実行準備、実行エラー以外の場合利用不可
            if (!(((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.None) || ((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.Prepare) || ((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.RunErr)))
                return false;

            //処理フラグがスキップの場合利用不可
            if (((RunJobMethodType)existJob["method_flag"]).Equals(RunJobMethodType.SKIP))
                return false;

            // added by YAMA 2014/11/05    処理継続時の実行ジョブ詳細画面でのコンテキストメニュー
            // 処理継続のジョブの場合、利用不可
            if (isContinuousJOB(item, existJob))
                return false;

            //Park.iggy 追加 START
            //親ジョブネットがジョブネット停止が行った場合利用不可
            if (IsJobnetAbortFlag(existJob))
                return false;

            //Park.iggy 追加 END

            return true;
        }

        //*******************************************************************
        /// <>スキップ解除可能判定</>
        //*******************************************************************
        private bool IsUnSkipEnable(IElement item, DataRow existJob)
        {
            //処理フラグがスキップ＆ステータスが未実行or実行準備以外利用不可
            if (!(((RunJobMethodType)existJob["method_flag"]).Equals(RunJobMethodType.SKIP) && (((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.None) || ((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.Prepare))))
                return false;

            //Park.iggy 追加 START
            //親ジョブネットがジョブネット停止が行った場合利用不可
            if (IsJobnetAbortFlag(existJob))
                return false;

            //Park.iggy 追加 END

            return true;
        }

        //*******************************************************************
        /// <>強制停止可能判定</>
        //*******************************************************************
        private bool IsStopEnable(IElement item, DataRow existJob)
        {
            // ジョブアイコン以外の場合利用不可
            //    if (!(item is Job || item is ExtJob || item is Reboot))
            //added by YAMA 2014/05/30 エージェントレスアイコンも追加
            //    if (!(item is Job || item is ExtJob || item is Reboot || item is Agentless))
            //added by YAMA 2014/06/26 ファイル待合わせアイコンも追加
            if (!(item is Job || item is ExtJob || item is Reboot || item is Agentless || item is FWait))
                return false;

            //ステータスが実行中以外の場合利用不可
            if (!((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.During))
                return false;
            return true;
        }


        //*******************************************************************
        /// <>再実行可能判定</>
        //*******************************************************************
        private bool IsReStartEnable(IElement item, DataRow existJob)
        {
            // ジョブ、拡張ジョブ、ファイル伝送アイコン以外の場合利用不可
            //    if (!(item is Job || item is ExtJob || item is FCopy || item is Reboot || item is Release))
            //added by YAMA 2014/05/30 エージェントレスアイコンも追加
            //    if (!(item is Job || item is ExtJob || item is FCopy || item is Reboot || item is Release || item is Agentless))
            //added by YAMA 2014/06/26 ファイル待合わせアイコンも追加
            if (!(item is Job || item is ExtJob || item is FCopy || item is Reboot || item is Release || item is Agentless || item is FWait || item is If))

                return false;

            //ステータスが実行エラー以外の場合利用不可
            if (!((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.RunErr))
                return false;

            // added by YAMA 2014/11/05    処理継続時の実行ジョブ詳細画面でのコンテキストメニュー
            // 処理継続のジョブの場合、利用不可
            if (isContinuousJOB(item, existJob))
                return false;

            //Park.iggy 追加 START
            //親ジョブネットがジョブネット停止が行った場合利用不可
            if (IsJobnetAbortFlag(existJob))
                return false;

            //Park.iggy 追加 END

            return true;
        }

        //*******************************************************************
        /// <>変数値変更可能判定</>
        //*******************************************************************
        private bool IsParameterEnable(IElement item, DataRow existJob)
        {
            // ジョブ、条件、終了、ジョブコントローラ変数アイコン以外の場合利用不可
            //    if (!(item is End || item is Job || item is If || item is Env))
            //added by YAMA 2014/05/30 エージェントレスアイコンも追加
            if (!(item is End || item is Job || item is If || item is Env || item is Agentless))
                return false;

            //ステータスが未実行＋処理フラグが保留かステータスが実行エラー以外の場合利用不可
            if (!(((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.None) ||
                    (((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.Prepare) && ((RunJobMethodType)existJob["method_flag"]).Equals(RunJobMethodType.HOLD)) ||
                    ((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.RunErr)))
                return false;

            // added by YAMA 2014/11/05    処理継続時の実行ジョブ詳細画面でのコンテキストメニュー
            // 処理継続のジョブの場合、利用不可
            if (isContinuousJOB(item, existJob))
                return false;

            return true;
        }
        //*******************************************************************
        /// <>保留処理</>
        //*******************************************************************
        private void SetHold(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.None))
            {
                row["method_flag"] = (int)RunJobMethodType.HOLD;
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }

        //*******************************************************************
        /// <>保留解除処理</>
        //*******************************************************************
        private void SetUnHold(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if ((((RunJobStatusType)row["status"]).Equals(RunJobStatusType.None) || ((RunJobStatusType)row["status"]).Equals(RunJobStatusType.Prepare)) && ((RunJobMethodType)row["method_flag"]).Equals(RunJobMethodType.HOLD))
            {
                row["method_flag"] = (int)RunJobMethodType.NORMAL;
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }

        //*******************************************************************
        /// <>スキップ処理</>
        //*******************************************************************
        private void SetSkip(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.None) || ((RunJobStatusType)row["status"]).Equals(RunJobStatusType.Prepare) || ((RunJobStatusType)row["status"]).Equals(RunJobStatusType.RunErr))
            {
                row["method_flag"] = (int)RunJobMethodType.SKIP;
                if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.RunErr))
                {
                    row["status"] = (int)RunJobStatusType.Prepare;
                }
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }

        //*******************************************************************
        /// <>スキップ解除解除処理</>
        //*******************************************************************
        private void SetUnSkip(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.None) && ((RunJobMethodType)row["method_flag"]).Equals(RunJobMethodType.SKIP))
            {
                row["method_flag"] = (int)RunJobMethodType.NORMAL;
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }

        //*******************************************************************
        /// <>強制停止処理</>
        //*******************************************************************
        private void SetStop(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.During))
            {
                row["method_flag"] = (int)RunJobMethodType.STOP;
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }

        //*******************************************************************
        /// <>再実行処理</>
        //*******************************************************************
        private void SetReStart(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            //トランザクション開始
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            DataTable dt = _runJobControlDAO.GetEntityByPkForUpdate(jobId);
            DataRow row = dt.Rows[0];
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.RunErr))
            {
                row["status"] = (int)RunJobStatusType.Prepare;
                row["method_flag"] = (int)RunJobMethodType.RERUN;
                row["timeout_flag"] = (int)RunJobTimeoutType.NORMAL;
                _dbAccess.ExecuteNonQuery(dt, _runJobControlDAO);

                // added by YAMA 2014/11/05    拡張ジョブアイコンの再実行
                // 実行ジョブ管理テーブルのジョブタイプが「拡張ジョブ」の場合、
                // 実行用ジョブネット内部管理IDをキーに実行拡張ジョブアイコン設定テーブルの待合せ回数を「0」に更新する
                if (((ElementType)row["job_type"]).Equals(ElementType.EXTJOB))
                {
                    _runIconExtJobDAO = new RunIconExtJobDAO(_dbAccess);
                    int cnt = _runIconExtJobDAO.SetWaitCountByJobnet(0, jobId);
                }

            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

        }
        //*******************************************************************
        /// <>変数値変更処理</>
        //*******************************************************************
        private void ShowParameterSetting(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            _dbAccess.CreateSqlConnect();
            DataTable dt = _runJobControlDAO.GetEntityByPk(jobId);
            DataRow row = dt.Rows[0];
            _dbAccess.CloseSqlConnect();
            if (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.None) ||
                (((RunJobStatusType)row["status"]).Equals(RunJobStatusType.Prepare) && ((RunJobMethodType)row["method_flag"]).Equals(RunJobMethodType.HOLD)) ||
                ((RunJobStatusType)row["status"]).Equals(RunJobStatusType.RunErr))
            {
                ParameterSetting paremeterSetting = new ParameterSetting(room);
                paremeterSetting.Owner = (Window)ParantWindow;
                var parent = paremeterSetting.Owner;

                EventHandler parentDeactivate = (_, __) => { paremeterSetting.Activate(); };
                parent.Activated += parentDeactivate;

                RoutedEventHandler window_Loaded = (_, __) => {
                    ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)parent).IsHitTestVisible = false;
                };
                EventHandler window_Closed = (_, __) => {
                    ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)parent).IsHitTestVisible = true;
                    parent.Activated -= parentDeactivate;
                };
                paremeterSetting.Loaded += window_Loaded;
                paremeterSetting.Closed += window_Closed;
                paremeterSetting.Show();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        //*******************************************************************
        /// <>変数値表示処理</>
        //*******************************************************************
        private void ShowParameterView(IRoom room)
        {
            String jobId = room.ContentItem.InnerJobId;
            _dbAccess.CreateSqlConnect();
            DataTable dt = _runJobControlDAO.GetEntityByPk(jobId);
            DataRow row = dt.Rows[0];
            _dbAccess.CloseSqlConnect();
            ParameterView paremeterView = new ParameterView(room);
            paremeterView.Owner = (Window)ParantWindow;
            var parent = paremeterView.Owner;

            EventHandler parentDeactivate = (_, __) => { paremeterView.Activate(); };
            parent.Activated += parentDeactivate;

            RoutedEventHandler window_Loaded = (_, __) => {
                paremeterView.Activate();
                ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)parent).IsHitTestVisible = false; };
            EventHandler window_Closed = (_, __) => {
                ((jp.co.ftf.jobcontroller.JobController.Form.JobManager.JobnetExecDetail)parent).IsHitTestVisible = true;
                parent.Activated -= parentDeactivate;
            };
            paremeterView.Loaded += window_Loaded;
            paremeterView.Closed += window_Closed;
            paremeterView.Show();

        }

        // added by YAMA 2014/11/05    処理継続時の実行ジョブ詳細画面でのコンテキストメニュー
        //*******************************************************************
        /// <>処理継続のジョブか否かの判定</>
        //    true：「ステータスが実行エラー」かつ「処理継続」の場合
        //*******************************************************************
        private bool isContinuousJOB(IElement item, DataRow existJob)
        {

            // 「ステータスが実行エラー」かつ「処理継続」の場合、"true" を返す
            if ((((RunJobStatusType)existJob["status"]).Equals(RunJobStatusType.RunErr)) && (existJob["continue_flag"].ToString().Equals("1")))
                return true;

            // 上記以外の場合、"false"  を返す
            return false;
        }

        #endregion

        #region DB処理


        //*******************************************************************
        /// <>ジョブ位置設定</>
        /// <param name="jobId">ジョブID</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        //*******************************************************************
        public void SetItemPosition(string jobId, double x, double y)
        {
            DataRow row = JobControlTable.Select("job_id = '" + jobId + "'")[0];
            row["point_x"] = Convert.ToInt32(x);
            row["point_y"] = Convert.ToInt32(y);

        }

        //*******************************************************************
        /// <>アイコンの存在チェック</>
        /// <param name="type">ジョブタイプ</param>
        /// <returns>true:既存 False:存在しない</returns>
        //*******************************************************************
        private bool CheckJobExist(ElementType type)
        {
            // 開始アイコンの場合
            if (ElementType.START == type)
            {
                DataRow[] rows = JobControlTable.Select("job_type = 0");
                if (rows != null && rows.Count() > 0)
                    return true;
            }

            return false;
        }

        //*******************************************************************
        /// <>アイコン設定テーブルのデータを削除</>
        /// <param name="jobid"></param>
        /// <param name="type"></param>
        //*******************************************************************
        private void DeleteIconSetting(string jobid, ElementType type)
        {
            string where = "job_id='" + jobid + "'";
            DataRow[] rows = null;
            switch (type)
            {
                // 0:開始、6:並行処理開始、7：並行処理終了、8：ループの場合
                case ElementType.START:
                case ElementType.LOOP:
                case ElementType.MTS:
                case ElementType.MTE:
                //added by kim 2012/11/14
                case ElementType.IFE:
                    break;
                // 1:終了の場合
                case ElementType.END:
                    rows = IconEndTable.Select(where);
                    break;
                // 2:条件分岐の場合
                case ElementType.IF:
                    rows = IconIfTable.Select(where);
                    break;
                // 3:ジョブコントローラ変数の場合
                case ElementType.ENV:
                    rows = IconValueTable.Select(where);
                    break;
                // 4:ジョブの場合
                case ElementType.JOB:
                    rows = IconJobTable.Select(where);
                    // ジョブコマンド設定テーブル
                    DataRow[] rowsTmp = JobCommandTable.Select(where);
                    if (rowsTmp != null)
                    {
                        foreach (DataRow row in rowsTmp)
                            row.Delete();
                    }

                    // ジョブ変数設定テーブル
                    rowsTmp = JobCommandTable.Select(where);
                    if (rowsTmp != null)
                    {
                        foreach (DataRow row in rowsTmp)
                            row.Delete();
                    }

                    // ジョブコントローラ変数設定テーブル
                    rowsTmp = ValueJobConTable.Select(where);
                    if (rowsTmp != null)
                    {
                        foreach (DataRow row in rowsTmp)
                            row.Delete();
                    }

                    break;
                // 5:ジョブネット
                case ElementType.JOBNET:
                    rows = IconJobnetTable.Select(where);
                    break;
                // 9：拡張ジョブの場合
                case ElementType.EXTJOB:
                    rows = IconExtjobTable.Select(where);
                    break;
                //  10：計算の場合
                case ElementType.CAL:
                    rows = IconCalcTable.Select(where);
                    break;
                // 11：タスク場合
                case ElementType.TASK:
                    rows = IconTaskTable.Select(where);
                    break;
                // 12：情報取得場合
                case ElementType.INF:
                    rows = IconInfoTable.Select(where);
                    break;
                // 14:ファイル転送の場合
                case ElementType.FCOPY:
                    rows = IconFcopyTable.Select(where);
                    break;
                // 15:ファイル待ち合わせの場合
                case ElementType.FWAIT:
                    rows = IconFwaitTable.Select(where);
                    break;
                // 16:リブートの場合
                case ElementType.REBOOT:
                    rows = IconRebootTable.Select(where);
                    break;
                // 17:保留解除の場合
                case ElementType.RELEASE:
                    rows = IconReleaseTable.Select(where);
                    break;

                //added by YAMA 2014/02/06
                // 18:Zabbix連携の場合
                case ElementType.COOPERATION:
                    rows = IconCooperationTable.Select(where);
                    break;

                //added by YAMA 2014/05/19
                // 19:エージェントレスの場合
                case ElementType.AGENTLESS:
                    rows = IconAgentlessTable.Select(where);
                    break;
            }
            // 削除
            if (rows != null)
            {
                foreach (DataRow row in rows)
                    row.Delete();
            }
        }

        //added by Park.iggy 2015/04/30
        //*******************************************************************
        /// <>ジョブネット停止フラグ</>
        /// <param name="type">ジョブタイプ</param>
        /// <returns>true:ジョブネット停止 False:ジョブネット停止なし</returns>
        //*******************************************************************
        private bool IsJobnetAbortFlag(DataRow existJob)
        {

            Object innerJobnetId = (Object)existJob["inner_jobnet_main_id"];
            Boolean reFlag = false;
            _dbAccess.CreateSqlConnect();
            _dbAccess.BeginTransaction();
            _runJobnetSummaryDAO = new RunJobnetSummaryDAO(_dbAccess);
            DataTable dt = _runJobnetSummaryDAO.GetEntityByPk(innerJobnetId);
            DataRow row = dt.Rows[0];
            if (((JobnetAbortFlag)row["jobnet_abort_flag"]).Equals(JobnetAbortFlag.TRUE))
            {
                reFlag = true;
            }
            _dbAccess.TransactionCommit();
            _dbAccess.CloseSqlConnect();

            return reFlag;
        }

        #endregion

        #endregion

    }
}
