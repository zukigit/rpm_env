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
using System.Windows.Shapes;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.JobController.Form.JobManager;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/04 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
/// <summary>
/// Container.xaml の相互作用ロジック
/// </summary>
public partial class Container : UserControl,IContainer
{
    #region コンストラクタ
    public Container()
    {
        // 初期化
        InitializeComponent();
    }
    #endregion

    #region フィールド

    /// <summary>最大UNDO値</summary> stat
    private static int MAX_UNDO = 50;

    /// <summary>JOBデータ区別詞</summary>
    private static string JOB_DIST="JOB";

    /// <summary>COMMANDデータ区別詞</summary>
    private static string COMMAND_DIST = "COMMAND";

    /// <summary>job Valueデータ区別詞</summary>
    private static string JOB_VALUE_DIST = "JOB_VALUE";

    /// <summary>Jobcontrol Valueデータ区別詞</summary>
    private static string JOBCON_VALUE_DIST = "JOBCON_VALUE";

    /// <summary>マウスの位置</summary>
    private Point mousePosition;

    /// <summary>マウス追従フラグ</summary>
    private bool trackingMouseMove = false;

    /// <summary>選択範囲</summary>
    Rectangle temproaryEllipse;

    /// <summary>DBアクセスインスタンス</summary>
    private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

    /// <summary>inner_jobnet_id index取得SQL</summary>
    private String GET_INNER_JOBNET_ID_SQL = "select nextid from ja_index_table where count_id=2 for update";

    /// <summary>inner_job_id index取得SQL</summary>
    private String GET_INNER_JOB_ID_SQL_START = "select nextid from ja_index_table where count_id=20 for update";

    /// <summary>inner_flow_id index取得SQL</summary>
    private String GET_INNER_FLOW_ID_SQL1 = "select nextid from ja_index_table where count_id=30 for update";

    private List<HistoryData> HistoryDataList = new List<HistoryData>();

    private Hashtable TempHash = new Hashtable();
    private int TempJobNo = 0;


    /// <summary>画布</summary>
    public Canvas ContainerCanvas
    {
        get
        {
            return cnsDesignerContainer;
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

    /// <summary>画布内のアイテム</summary>
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

    /// <summary>画布の空白の場所にクリックフラグ</summary>
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

    /// <summary>マウスがコンテナ内かどうかのフラグ</summary>
    bool mouseIsInContainer = false;
    public bool MouseIsInContainer {
        get
        {
            return mouseIsInContainer;
        }
        set
        {
            mouseIsInContainer = value;
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

    /// <summary>選択した部品リストの最左位置のX座標</summary>
    private double _leftXOfSelection = -1;
    public double LeftXOfSelection
    {
        get
        {
            return _leftXOfSelection;
        }
        set
        {
            _leftXOfSelection = value;
        }
    }

    /// <summary>選択した部品リストの最上位置のY座標</summary>
    private double _topYOfSelection = -1;
    public double TopYOfSelection
    {
        get
        {
            return _topYOfSelection;
        }
        set
        {
            _topYOfSelection = value;
        }
    }

    /// <summary>マウス選択かどうか。</summary>
    public bool IsMouseSelecting
    {
        get
        {
            return (temproaryEllipse != null);
        }
    }

    /// <summary>ジョブネットID</summary>
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

    /// <summary>シフトキー押下状態フラグ</summary>
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
    /// <summary>コンテナをを左クリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            // 選択をクリア
            if (_canvasClickFlg)
                ClearSelectFlowElement(null);

            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(element);
            // 一旦削除（区域選択用）

            trackingMouseMove = true;

        }
    }

    //*******************************************************************
    /// <summary>コンテナを右クリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.ContextMenu.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// コンテナ内のマウス移動処理
    /// </summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    public void Container_MouseMove(object sender, MouseEventArgs e)
    {
        // 追従の場合

        if (trackingMouseMove)
        {
            FrameworkElement element = sender as FrameworkElement;
            Point beginPoint = mousePosition;
            Point endPoint = e.GetPosition(element);

            if (temproaryEllipse == null)
            {
                temproaryEllipse = new Rectangle();

                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 234, 213, 2);
                temproaryEllipse.Fill = brush;
                temproaryEllipse.Opacity = 0.2;

                brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 0, 0, 0);
                temproaryEllipse.Stroke = brush;
                temproaryEllipse.StrokeMiterLimit = 2.0;

                cnsDesignerContainer.Children.Add(temproaryEllipse);

            }

            if (endPoint.X >= beginPoint.X)
            {
                if (endPoint.Y >= beginPoint.Y)
                {
                    temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                    temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                }
                else
                {
                    temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                    temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                }

            }
            else
            {
                if (endPoint.Y >= beginPoint.Y)
                {
                    temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                    temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                }
                else
                {
                    temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                    temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                }

            }

            temproaryEllipse.Width = Math.Abs(endPoint.X - beginPoint.X);
            temproaryEllipse.Height = Math.Abs(endPoint.Y - beginPoint.Y);
            //スクロール際の場合、スクロールする
            ScrollIfNeeded(e.GetPosition(svContainer));

        }
    }
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        double deltaX = 0;
        double deltaY = 0;
        if (e.Key == Key.Left)
        {
            deltaX = -1;
        }
        if (e.Key == Key.Right)
        {
            deltaX = 1;
        }
        if (e.Key == Key.Up)
        {
            deltaY = -1;
        }
        if (e.Key == Key.Down)
        {
            deltaY = 1;
        }

        if ((deltaX > 0 || deltaX < 0 || deltaY > 0 || deltaY < 0) && IsCanArrowMove(deltaX, deltaY))
        {
            MoveControlCollectionByDisplacement(deltaX, deltaY, null);
            SetControlCollectionItemAndRuleNameControlPosition(null);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.Delete)
        {
            DeleteSelectedControl();
            DeleteSelectedFlow();
            e.Handled = true;
            return;
        }
        if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.C)
        {
            SaveSelectedControlCollection();
            e.Handled = true;
            return;
        }
        if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.V)
        {
            PasteSelectedControlCollection();
            e.Handled = true;
            return;
        }
        if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.Z)
        {
            Undo();
            e.Handled = true;
            return;
        }
    }

    //*******************************************************************
    /// <summary>左マウスの解放処理</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        trackingMouseMove = false;

        FrameworkElement element = sender as FrameworkElement;
        mousePosition = e.GetPosition(element);

        if (temproaryEllipse != null)
        {
            double width = temproaryEllipse.Width;
            double height = temproaryEllipse.Height;

            if (width > 10 && height > 10)
            {
                Point p = new Point();
                p.X = (double)temproaryEllipse.GetValue(Canvas.LeftProperty);
                p.Y = (double)temproaryEllipse.GetValue(Canvas.TopProperty);

                //Activity a = null;
                //Rule r = null;
                Label l = null;
                foreach (UIElement uie in cnsDesignerContainer.Children)
                {
                    if (uie is Flow || uie is FlowArc || uie is Rectangle) continue;
                    CommonItem item = (CommonItem)uie;

                    if (p.X < item.CenterPoint.X && item.CenterPoint.X < p.X + width
             && p.Y < item.CenterPoint.Y && item.CenterPoint.Y < p.Y + height)
                    {
                        AddSelectedControl(item);
                        item.IsSelectd = true;
                    }
                }
            }
            cnsDesignerContainer.Children.Remove(temproaryEllipse);

            _leftXOfSelection = getLeftX();
            _topYOfSelection = getTopY();

            temproaryEllipse = null;
        }

    }

    //*******************************************************************
    /// <summary>コンテナに入れる</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void Container_MouseEnter(object sender, MouseEventArgs e)
    {
        mouseIsInContainer = true;
    }

    //*******************************************************************
    /// <summary>コンテナを離れる</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void Container_MouseLeave(object sender, MouseEventArgs e)
    {
        mouseIsInContainer = false;
    }

    #region 部品のドラッグ処理

    //*******************************************************************
    /// <summary>ジョブをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void JobSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.JOB;
        DragDrop.DoDragDrop((JobSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>条件をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void IfSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.IF;
        DragDrop.DoDragDrop((IfSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>並行の開始をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void ParStartSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.MTS;
        DragDrop.DoDragDrop((MtsSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>並行の終了をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void ParEndSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.MTE;
        DragDrop.DoDragDrop((MteSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>変数をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void VarSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.ENV;
        DragDrop.DoDragDrop((EnvSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>拡張ジョブをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void JobExSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.EXTJOB;
        DragDrop.DoDragDrop((ExtJobSample)sender, data, DragDropEffects.Copy);
    }


    //*******************************************************************
    /// <summary>Startドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void StartSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.START;
        DragDrop.DoDragDrop((StartSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>ENDドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void EndSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.END;
        DragDrop.DoDragDrop((EndSample)sender, data, DragDropEffects.Copy);

    }

    //*******************************************************************
    /// <summary>計算をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void CalcSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.CAL;
        DragDrop.DoDragDrop((CalSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>Loopをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void LoopSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.LOOP;
        DragDrop.DoDragDrop((LoopSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>タスクをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void TaskSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.TASK;
        DragDrop.DoDragDrop((TaskSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>情報をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void InfoSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.INF;
        DragDrop.DoDragDrop((InfSample)sender, data, DragDropEffects.Copy);
    }

    //added by kim 2012/11/14
    //*******************************************************************
    /// <summary>分岐の終了をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void IfeSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.IFE;
        DragDrop.DoDragDrop((IfeSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>ファイル転送をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void FCopySample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.FCOPY;
        DragDrop.DoDragDrop((FCopySample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>ファイル待ち合わせをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void FWaitSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.FWAIT;
        DragDrop.DoDragDrop((FWaitSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>リブートをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void RebootSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.REBOOT;
        DragDrop.DoDragDrop((RebootSample)sender, data, DragDropEffects.Copy);
    }

    //*******************************************************************
    /// <summary>保留解除をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void ReleaseSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.RELEASE;
        DragDrop.DoDragDrop((ReleaseSample)sender, data, DragDropEffects.Copy);
    }

    //added by YAMA 2014/02/04
    //*******************************************************************
    /// <summary>Zabbix連携をドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void CooperationSample_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.COOPERATION;
        DragDrop.DoDragDrop((CooperationSample)sender, data, DragDropEffects.Copy);
    }

    //added by YAMA 2014/05/19
    //*******************************************************************
    /// <summary>エージェントレスをドラッグ</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    public void AgentlessSample_MouseLeftButtonDow(object sender, MouseButtonEventArgs e)
    {
        JobData data = new JobData();
        data.JobType = ElementType.AGENTLESS;
        DragDrop.DoDragDrop((AgentlessSample)sender, data, DragDropEffects.Copy);
    }

    //************************************************************************
    /// <summary>ドラッグ部品を受け入れる</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //************************************************************************
    public void Target_Drop(object sender, DragEventArgs e)
    {
        //処理前現在データで履歴を作成
        CreateHistData();

        JobData data = (JobData)e.Data.GetData
            ("jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobData");
        //アイコンの存在チェック
        if (CheckJobExist(data.JobType))
        {
            CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_004);
            return;
        }

        CommonItem item = new CommonItem((IContainer)this, data, Consts.EditType.Add, RunJobMethodType.NORMAL);

        ((Canvas)sender).Children.Add(item);
        this.JobItems.Add(item.JobId,item);

        Point p = e.GetPosition(item);

        double x = p.X - item.PicWidth / 2;
        double y = p.Y - item.PicHeight / 2;

        double maxWidth = ContainerCanvas.Width - item.PicWidth;
        double maxHeight = ContainerCanvas.Height - item.PicHeight;

        // コンテナの範囲を越える
        if (x < 0)
            x = 0;
        else if (x > maxWidth)
            x = maxWidth;

        if (y < 0)
            y = 0;
        else if (y > maxHeight)
            y = maxHeight;

        item.SetValue(Canvas.TopProperty, y);
        item.SetValue(Canvas.LeftProperty, x);

        SetItemPosition(item.JobId, x, y);
    }

    #endregion

    #region メニューコンテクストのイベント


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
    /// <summary>直線フローをクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemLine_Click(object sender, RoutedEventArgs e)
    {
        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        MakeFlow(FlowLineType.Line, (IRoom)selectItems[0], (IRoom)selectItems[1], 0, Consts.EditType.Add);
    }

    //*******************************************************************
    /// <summary>曲線フローをクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemCurve_Click(object sender, RoutedEventArgs e)
    {
        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        MakeFlow(FlowLineType.Curve, (IRoom)selectItems[0], (IRoom)selectItems[1],0, Consts.EditType.Add);

    }

    //*******************************************************************
    /// <summary>TRUE設定をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemSetTrue_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentSelectedControlCollection == null
            || CurrentSelectedControlCollection.Count != 1
            || !(CurrentSelectedControlCollection[0] is IFlow))
            return;

        //処理前現在データで履歴を作成
        CreateHistData();

        IFlow flow = (IFlow)CurrentSelectedControlCollection[0];
        flow.SetTrue(Consts.EditType.Add);
    }

    //*******************************************************************
    /// <summary>FALSE設定をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemSetFalse_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentSelectedControlCollection == null
            || CurrentSelectedControlCollection.Count != 1
            || !(CurrentSelectedControlCollection[0] is IFlow))
            return;

        //処理前現在データで履歴を作成
        CreateHistData();

        IFlow flow = (IFlow)CurrentSelectedControlCollection[0];
        flow.SetFalse(Consts.EditType.Add);
    }

    //*******************************************************************
    /// <summary>フロー削除をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemDelFlow_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectedFlow();
    }

    //*******************************************************************
    /// <summary>削除をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemlineDel_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectedControl();
    }

    //*******************************************************************
    /// <summary>設定をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemlineSet_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSelectedControlCollection.Count == 1)
        {
            ((IRoom)_currentSelectedControlCollection[0]).ShowIconSetting(true);
        }
    }

    //*******************************************************************
    /// <summary>ジョブ起動をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemJobRun_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSelectedControlCollection.Count == 1 &&
            ((CommonItem)_currentSelectedControlCollection[0]).ElementType == ElementType.JOB &&
            (SetedJobIds.Contains(((CommonItem)_currentSelectedControlCollection[0]).JobId) ||
            ((CommonItem)_currentSelectedControlCollection[0]).ItemEditType != Consts.EditType.Add)
            )
        {
            JobRun(((CommonItem)_currentSelectedControlCollection[0]).JobId);
        }
    }

    //*******************************************************************
    /// <summary>保留をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemHold_Click(object sender, RoutedEventArgs e)
    {
        // 開始ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteStartLog("MenuitemHold_Click", Consts.PROCESS_018);

        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        //added by YAMA 2014/10/24
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            SetHold(((IRoom)selectItems[i]).ContentItem);
        }
        //SetHold(((IRoom)selectItems[0]).ContentItem);

        // 終了ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteEndLog("MenuitemHold_Click", Consts.PROCESS_018);
    }

    //*******************************************************************
    /// <summary>保留解除をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemUnHold_Click(object sender, RoutedEventArgs e)
    {
        // 開始ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteStartLog("MenuitemUnHold_Click", Consts.PROCESS_018);

        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        //added by YAMA 2014/10/24
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (((IRoom)selectItems[i]).ContentItem.MethodType == RunJobMethodType.HOLD)
            {
                SetUnHoldORUnSkip(((IRoom)selectItems[i]).ContentItem);
            }
        }
        //SetUnHoldORUnSkip(((IRoom)selectItems[0]).ContentItem);


        // 終了ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteEndLog("MenuitemUnHold_Click", Consts.PROCESS_018);
    }
    //*******************************************************************
    /// <summary>スキップをクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemSkip_Click(object sender, RoutedEventArgs e)
    {
        // 開始ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteStartLog("MenuitemSkip_Click", Consts.PROCESS_018);

        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        //added by YAMA 2014/10/24
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            IElement item = ((IRoom)selectItems[i]).ContentItem;

            // 開始、終了、条件分岐、並行処理、ループアイコンの場合利用不可
            if (item is Start || item is End || item is If || item is Ife || item is Mts || item is Mte || item is Loop)
            {
                continue;
            }
            SetSkip(((IRoom)selectItems[i]).ContentItem);
        }
        //SetSkip(((IRoom)selectItems[0]).ContentItem);

        // 終了ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteEndLog("MenuitemSkip_Click", Consts.PROCESS_018);
    }

    //*******************************************************************
    /// <summary>スキップ解除をクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    //*******************************************************************
    private void MenuitemUnSkip_Click(object sender, RoutedEventArgs e)
    {
        // 開始ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteStartLog("MenuitemUnSkip_Click", Consts.PROCESS_018);

        //処理前現在データで履歴を作成
        CreateHistData();

        List<System.Windows.Controls.Control> selectItems = CurrentSelectedControlCollection;
        //added by YAMA 2014/10/24
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (((IRoom)selectItems[i]).ContentItem.MethodType == RunJobMethodType.SKIP)
            {
                SetUnHoldORUnSkip(((IRoom)selectItems[i]).ContentItem);
            }
        }
        //SetUnHoldORUnSkip(((IRoom)selectItems[0]).ContentItem);

        // 終了ログ
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).WriteEndLog("MenuitemUnSkip_Click", Consts.PROCESS_018);
    }

    #endregion

    #endregion

    #region publicメッソド

    //*******************************************************************
    /// <summary>設定をクリック</summary>
    /// <param name="lineType">線のタイプ</param>
    /// <param name="item1">開始ジョブ</param>
    /// <param name="item2">終了ジョブ</param>
    /// <param name="flowType">フローのタイプ(True、False)</param>
    /// <param name="editType">編集タイプ</param>
    //*******************************************************************
    public void MakeFlow(FlowLineType lineType, IRoom item1, IRoom item2,　int flowType, Consts.EditType editType)
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

    //*******************************************************************
    /// <summary>UNDO処理用履歴データ作成</summary>
    //*******************************************************************
    public void CreateHistData()
    {
        HistoryData hist = new HistoryData();
        hist.JobControlTable = JobControlTable.Copy();
        hist.IconCalcTable = IconCalcTable.Copy();
        hist.IconEndTable = IconEndTable.Copy();
        hist.IconExtjobTable = IconExtjobTable.Copy();
        hist.IconFcopyTable = IconFcopyTable.Copy();
        hist.IconFwaitTable = IconFwaitTable.Copy();
        hist.IconIfTable = IconIfTable.Copy();
        hist.IconInfoTable = IconInfoTable.Copy();
        hist.IconJobnetTable = IconJobnetTable.Copy();
        hist.IconJobTable = IconJobTable.Copy();
        hist.IconRebootTable = IconRebootTable.Copy();
        hist.IconReleaseTable = IconReleaseTable.Copy();
        hist.IconTaskTable = IconTaskTable.Copy();
        hist.IconValueTable = IconValueTable.Copy();
        hist.JobCommandTable = JobCommandTable.Copy();
        hist.ValueJobConTable = ValueJobConTable.Copy();
        hist.ValueJobTable = ValueJobTable.Copy();
        hist.FlowControlTable = FlowControlTable.Copy();
        hist.SetedJobIds = (Hashtable)SetedJobIds.Clone();
        hist.JobIdNos = (Hashtable)((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).JobNoHash.Clone();

        //added by YAMA 2014/02/06
        hist.IconCooperationTable = IconCooperationTable.Copy();

        //added by YAMA 2014/05/19
        hist.IconAgentlessTable = IconAgentlessTable.Copy();

        HistoryDataList.Add(hist);

        if (HistoryDataList.Count > MAX_UNDO)
        {
            HistoryDataList.RemoveAt(0);
        }
    }



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

    //*******************************************************************
    /// <summary>フローを追加</summary>
    /// <param name="flow">フロー</param>
    /// <param name="item1">開始ジョブ/param>
    /// <param name="item2">終了ジョブ/param>
    /// <param name="flowWith">曲線幅(半径)/param>
    //*******************************************************************
    public void AddFlow(IFlow flow, IRoom item1, IRoom item2, double flowWith, Consts.EditType editType)
    {
        if (editType == Consts.EditType.READ || Consts.ActionMode.USE == LoginSetting.Mode)
        {
            ((UserControl)flow).IsEnabled = false;
        }
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
    /// <summary>特定の部品を選択</summary>
    /// <param name="uc">部品</param>
    /// <param name="isSelected">選択フラグ</param>
    //*******************************************************************
    public void SetWorkFlowElementSelected(System.Windows.Controls.Control uc, bool isSelected)
    {
        if (!ShiftKeyIsPress)
        {
            ClearSelectFlowElement(uc);
            AddSelectedControl(uc);
        }
        else
        {
            if (isSelected)
            {
                AddSelectedControl(uc);
                //_leftXOfSelection = getLeftX();
                //_topYOfSelection = getTopY();
            }
            else
                RemoveSelectedControl(uc);
        }

    }

    //*******************************************************************
    /// <summary>特定の部品選択状態をはずす</summary>
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
    /// <summary>部品選択状態をはずす</summary>
    /// <param name="uc">部品（nullの場合、全部をはずす）</param>
    //*******************************************************************
    public void ClearSelectFlowElement(System.Windows.Controls.Control uc)
    {
        if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
            return;

        int count = CurrentSelectedControlCollection.Count;
        for (int i = 0; i < count; i++)
        {
            if (!(CurrentSelectedControlCollection[i] is IFlow))
                ((IRoom)CurrentSelectedControlCollection[i]).IsSelectd = false;
            else if (CurrentSelectedControlCollection[i] is IFlow)
                ((IFlow)CurrentSelectedControlCollection[i]).IsSelectd = false;
        }

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
        IsSelectedByShiftKey = false;
    }

    //*******************************************************************
    /// <summary>選択した部品の移動処理（uc以外）</summary>
    /// <param name="x">x座標</param>
    /// <param name="y">y座標</param>
    /// <param name="uc">部品</param>
    //*******************************************************************
    public void MoveControlCollectionByDisplacement(double x, double y, UserControl uc)
    {
        if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
            return;

        CommonItem selectItem = uc as CommonItem;

        CommonItem tmpItem = null;
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                tmpItem = CurrentSelectedControlCollection[i] as CommonItem;
                if (tmpItem == selectItem)
                    continue;
                tmpItem.SetPositionByDisplacement(x, y);
            }
        }
    }

    //*******************************************************************
    /// <summary>選択した部品のフロー名の位置セット（uc以外）</summary>
    /// <param name="uc">部品</param>
    //*******************************************************************
    public void SetControlCollectionItemAndRuleNameControlPosition(UserControl uc)
    {
        if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
            return;

        CommonItem selectItem = uc as CommonItem;

        CommonItem tmpItem = null;
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                tmpItem = CurrentSelectedControlCollection[i] as CommonItem;
                if (tmpItem == selectItem)
                    continue;

                double x = (double)tmpItem.GetValue(Canvas.LeftProperty);
                double y = (double)tmpItem.GetValue(Canvas.TopProperty);

                SetItemPosition(tmpItem.JobId, x, y);
                // フローのTrue、Falseの位置をセット
                foreach (IFlow flow in tmpItem.BeginFlowList)
                    flow.setRuleNameControlPosition();
                foreach (IFlow flow in tmpItem.EndFlowList)
                    flow.setRuleNameControlPosition();
            }
        }
    }

    //*******************************************************************
    /// <summary>コンテナに部品を含むかどうかの判定</summary>
    /// <param name="uie">部品</param>
    //*******************************************************************
    public bool Contains(UIElement uie)
    {
        return cnsDesignerContainer.Children.Contains(uie);
    }

    //*******************************************************************
    /// <summary>特定の部品を削除</summary>
    /// <param name="uie">部品</param>
    //*******************************************************************
    public void RemoveItem(Control a)
    {
        if (cnsDesignerContainer.Children.Contains(a))
            cnsDesignerContainer.Children.Remove(a);
    }

    //*******************************************************************
    /// <summary>特定のフローを削除</summary>
    /// <param name="a">アイコン</param>
    //*******************************************************************
    public void RemoveFlow(IFlow a)
    {
        if (cnsDesignerContainer.Children.Contains((UIElement)a))
            cnsDesignerContainer.Children.Remove((UIElement)a);
    }

    /// <summary>MouseMoveイベントを削除</summary>
    public void RemoveContainerMoveEvent()
    {
        cnsDesignerContainer.MouseMove -= Container_MouseMove;
    }
    //*******************************************************************
    /// <summary>選択部品を削除</summary>
    //*******************************************************************
    public void DeleteSelectedControl()
    {
        //if (_currentSelectedControlCollection == null
        //    || _currentSelectedControlCollection.Count < 1
        //    || !(_currentSelectedControlCollection[0] is IRoom))
        //    return;

        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1)
            return;

        //処理前現在データで履歴を作成
        CreateHistData();

        for (int i = CurrentSelectedControlCollection.Count - 1; i >= 0; i--)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                // ジョブを削除
                CommonItem item = (CommonItem)_currentSelectedControlCollection[i];

                string jobid = item.JobId;

                DataRow[] rows = JobControlTable.Select("job_id='" + jobid + "'");

                // アイコン設定テーブルにデータを削除
                ElementType type = (ElementType)(Convert.ToInt16(rows[0]["job_type"]));

                DeleteIconSetting(jobid, type);

                // ジョブ管理テーブルから削除
                if (rows != null && rows.Count() > 0)
                {
                    rows[0].Delete();
                }

                //JobControlTable.AcceptChanges();

                // フローを削除
                DataRow[] rowsFlow = FlowControlTable.Select("start_job_id='" + jobid + "' or end_job_id='" + jobid + "'");

                foreach (DataRow row in rowsFlow)
                {
                    row.Delete();
                }

                // アイコンを削除
                ((IRoom)_currentSelectedControlCollection[i]).Delete();
                _currentSelectedControlCollection.Remove(item);

                _jobItems.Remove(jobid);
                SetedJobIds.Remove(jobid);
            }
        }

    }

    //*******************************************************************
    /// <summary>選択フローを削除</summary>
    //*******************************************************************
    public void DeleteSelectedFlow_org()    //added by YAMA 2014/10/24
    {
        //if (CurrentSelectedControlCollection == null
        //    || CurrentSelectedControlCollection.Count != 1
        //    || !(CurrentSelectedControlCollection[0] is IFlow))
        //    return;

        if (CurrentSelectedControlCollection == null
            || CurrentSelectedControlCollection.Count != 1)
            return;

        //処理前現在データで履歴を作成
        CreateHistData();

        IFlow flow = (IFlow)_currentSelectedControlCollection[0];

        string beginJobId = flow.BeginItem.JobId;
        string endJobId = flow.EndItem.JobId;

        DataRow[] rows = FlowControlTable.Select("start_job_id='" + beginJobId + "' and end_job_id='" + endJobId + "'");

        if (rows != null && rows.Count() > 0)
            rows[0].Delete();

        RemoveFlow(flow);
    }

    //*******************************************************************
    /// <summary>選択フローを削除</summary>
    //*******************************************************************
    //added by YAMA 2014/10/24
    public void DeleteSelectedFlow()
    {
        //if (CurrentSelectedControlCollection == null
        //    || CurrentSelectedControlCollection.Count != 1
        //    || !(CurrentSelectedControlCollection[0] is IFlow))
        //    return;

        if (CurrentSelectedControlCollection == null
            ///|| CurrentSelectedControlCollection.Count != 1)
            || CurrentSelectedControlCollection.Count < 1)
            return;

        //処理前現在データで履歴を作成
        CreateHistData();

        for (int i = CurrentSelectedControlCollection.Count - 1; i >= 0; i--)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                // ジョブを取得
                CommonItem item = (CommonItem)_currentSelectedControlCollection[i];
                string jobid = item.JobId;

                // フローを削除
                DataRow[] rowsFlow = FlowControlTable.Select("start_job_id='" + jobid + "' or end_job_id='" + jobid + "'");

                foreach (DataRow row in rowsFlow)
                {
                    row.Delete();
                }
                if (item.BeginFlowList != null)
                {
                    foreach (IFlow a in item.BeginFlowList)
                    {
                        RemoveFlow(a);
                    }
                }
                if (item.EndFlowList != null)
                {
                    foreach (IFlow a in item.EndFlowList)
                    {
                        RemoveFlow(a);
                    }
                }
                item.ResetInitColor();
            }
            if (CurrentSelectedControlCollection[i] is IFlow)
            {
                IFlow flow = (IFlow)_currentSelectedControlCollection[0];

                string beginJobId = flow.BeginItem.JobId;
                string endJobId = flow.EndItem.JobId;

                DataRow[] rows = FlowControlTable.Select("start_job_id='" + beginJobId + "' and end_job_id='" + endJobId + "'");

                if (rows != null && rows.Count() > 0)
                    rows[0].Delete();

                RemoveFlow(flow);
            }
        }
    }

    #endregion

    #region privateメッソド

    //*******************************************************************
    /// <summary>ジョブ起動</summary>
    //*******************************************************************
    private void JobRun(Object JobId)
    {
        DateTime now = DBUtil.GetSysTime();
        String startXPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_START_X");
        String startYPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_START_Y");
        String jobXPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_JOB_X");
        String jobYPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_JOB_Y");
        String endXPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_END_X");
        String endYPoint = DBUtil.GetParameterVelue("JOBNET_DUMMY_END_Y");
        dbAccess.CreateSqlConnect();
        dbAccess.BeginTransaction();
        DataTable dt_inner_jobnet = dbAccess.ExecuteQuery(GET_INNER_JOBNET_ID_SQL);
        DataTable dt_inner_job_start = dbAccess.ExecuteQuery(GET_INNER_JOB_ID_SQL_START);
        DataTable dt_inner_flow1 = dbAccess.ExecuteQuery(GET_INNER_FLOW_ID_SQL1);

        String strInnerJobnetNextId = "";
        String strInnerJobNextIdStart = "";
        String strInnerJobNextIdJob = "";
        String strInnerJobNextIdEnd = "";
        String strInnerFlowNextId1 = "";
        String strInnerFlowNextId2 = "";
        String strJobnetNextId = "";

        string strNow = now.ToString("yyyyMMddHHmmss");
        int runType = (int)Consts.RunTypeEnum.Job;
        String jobnetName = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).tbxJobNetId.Text;

        //added by YAMA 2014/04/22
        //String multiple_starts = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).combMultipleStart.SelectedValue.ToString();

        String runJobnetName = jobnetName + "/" + JobId;
        int nameLen = runJobnetName.Length;
        if (nameLen > 64)
        {
            jobnetName = jobnetName.Substring(0,jobnetName.Length+64-nameLen);
        }
        runJobnetName = jobnetName + "/" + JobId;

        DataRow[] rowJob = JobControlTable.Select("job_id='" + JobId + "'");
        DataRow[] rowIconJob = IconJobTable.Select("job_id='" + JobId + "'");
        DataRow[] rowJobCommand = JobCommandTable.Select("job_id='" + JobId + "'");
        DataRow[] rowJobValue = ValueJobTable.Select("job_id='" + JobId + "'");
        DataRow[] rowJobconValue = ValueJobConTable.Select("job_id='" + JobId + "'");

        if (dt_inner_jobnet.Rows.Count == 1 && dt_inner_job_start.Rows.Count == 1 && dt_inner_flow1.Rows.Count == 1)
        {
            strInnerJobnetNextId = dt_inner_jobnet.Rows[0]["NEXTID"].ToString();
            strInnerJobNextIdStart = dt_inner_job_start.Rows[0]["NEXTID"].ToString();
            strInnerJobNextIdJob = (Convert.ToDecimal(dt_inner_job_start.Rows[0]["NEXTID"]) + 1).ToString();
            strInnerJobNextIdEnd = (Convert.ToDecimal(dt_inner_job_start.Rows[0]["NEXTID"]) + 2).ToString();
            strInnerFlowNextId1 = dt_inner_flow1.Rows[0]["NEXTID"].ToString();
            strInnerFlowNextId2 = (Convert.ToDecimal(dt_inner_flow1.Rows[0]["NEXTID"]) + 1).ToString();
            strJobnetNextId = "RUN_JOB_" + dt_inner_jobnet.Rows[0]["NEXTID"].ToString();

        }
        else
        {
            dbAccess.TransactionRollback();
            dbAccess.CloseSqlConnect();
            throw new DBException(Consts.SYSERR_004, null);
        }
        //added by YAMA 2014/04/22  add -> multiple_start_up
        String insertRunJobnet = "insert into ja_run_jobnet_table "
                + "(inner_jobnet_id, inner_jobnet_main_id, inner_job_id, update_date, run_type, "
                + "main_flag, status, start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo, execution_user_name) "
                + "VALUES (?,?,0,?,?,0,0,0,0,0,?,?,?,?,null,?)";
        List<ComSqlParam> insertRunJobnetSqlParams = new List<ComSqlParam>();
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_main_id", strInnerJobnetNextId));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@update_date", strNow));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@run_type", (int)runType));

        //added by YAMA 2014/04/22  ジョブ起動の場合は、多重起動ありで実行する
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@multiple_start_up", "0"));

        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_id", strJobnetNextId));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@user_name", LoginSetting.UserName));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_name", runJobnetName));
        insertRunJobnetSqlParams.Add(new ComSqlParam(DbType.String, "@execution_user_name", LoginSetting.UserName));
        dbAccess.ExecuteNonQuery(insertRunJobnet, insertRunJobnetSqlParams);

        //added by YAMA 2014/04/22  add -> multiple_start_up
        String insertRunJobnetSummary = "insert into ja_run_jobnet_summary_table "
                + "(inner_jobnet_id, update_date, run_type, invo_flag,"
                + "start_time, end_time, public_flag, multiple_start_up, jobnet_id, user_name, jobnet_name, memo,jobnet_timeout) "
                + "VALUES (?,?,?,1,0,0,0,?,?,?,?,null,?)";
        List<ComSqlParam> insertRunJobnetSummarySqlParams = new List<ComSqlParam>();
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@update_date", strNow));
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@run_type", (int)runType));

        //added by YAMA 2014/04/22 ジョブ起動の場合は、多重起動ありで実行する
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@multiple_start_up", "0"));

        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_id", strJobnetNextId));
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@user_name", LoginSetting.UserName));
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_name", runJobnetName));
        insertRunJobnetSummarySqlParams.Add(new ComSqlParam(DbType.String, "@jobnet_timeout", "0"));


        dbAccess.ExecuteNonQuery(insertRunJobnetSummary, insertRunJobnetSummarySqlParams);

        String insertRunJobStart = "insert into ja_run_job_table "
                + "(inner_job_id, inner_jobnet_id, inner_jobnet_main_id, job_type, invo_flag,"
                + "boot_count, start_time, end_time, point_x, point_y, job_id) "
                + "VALUES (?,?,?,?,1,0,0,0,?,?,?)";
        List<ComSqlParam> insertRunJobStartSqlParams = new List<ComSqlParam>();
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdStart));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_main_id", strInnerJobnetNextId));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@job_type", (int)ElementType.START));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@point_x", startXPoint));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@point_y", startYPoint));
        insertRunJobStartSqlParams.Add(new ComSqlParam(DbType.String, "@job_id", "START"));
        dbAccess.ExecuteNonQuery(insertRunJobStart, insertRunJobStartSqlParams);


        //
        String insertRunJobJob = "insert into ja_run_job_table "
                + "(inner_job_id, inner_jobnet_id, inner_jobnet_main_id, job_type, method_flag, invo_flag,"
            //added by Park.iggy 2015/05/01
                + " force_flag, "
            //added by YAMA 2014/09/26
                + "boot_count, start_time, end_time, point_x, point_y, job_id, job_name, run_user, run_user_password) "
            //  + "VALUES (?,?,?,?,?,1,1,0,0,?,?,?,?,?,?)";
            //added by Park.iggy 2015/05/01 add -> force_flag
                + "VALUES (?,?,?,?,?,1,?,1,0,0,?,?,?,?,?,?)";
            // + "boot_count, start_time, end_time, point_x, point_y, job_id, job_name) "
            // + "VALUES (?,?,?,?,?,1,1,0,0,?,?,?,?)";

        List<ComSqlParam> insertRunJobJobSqlParams = new List<ComSqlParam>();
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_main_id", strInnerJobnetNextId));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@job_type", (int)ElementType.JOB));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@method_flag", rowJob[0]["method_flag"]));
        //added by Park.iggy 2015/05/01 START
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@force_flag", rowJob[0]["force_flag"]));
        // END
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@point_x", jobXPoint));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@point_y", jobYPoint));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@job_id", rowIconJob[0]["job_id"]));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@job_name", rowJob[0]["job_name"]));

        //added by YAMA 2014/09/26
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@job_name", rowJob[0]["run_user"]));
        insertRunJobJobSqlParams.Add(new ComSqlParam(DbType.String, "@job_name", rowJob[0]["run_user_password"]));

        dbAccess.ExecuteNonQuery(insertRunJobJob, insertRunJobJobSqlParams);


        String insertRunJobEnd = "insert into ja_run_job_table "
                + "(inner_job_id, inner_jobnet_id, inner_jobnet_main_id, job_type, invo_flag,"
                + "boot_count, start_time, end_time, point_x, point_y, job_id) "
                + "VALUES (?,?,?,?,1,1,0,0,?,?,?)";
        List<ComSqlParam> insertRunJobEndSqlParams = new List<ComSqlParam>();
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdEnd));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_main_id", strInnerJobnetNextId));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@job_type", (int)ElementType.END));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@point_x", endXPoint));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@point_y", endYPoint));
        insertRunJobEndSqlParams.Add(new ComSqlParam(DbType.String, "@job_id", "END-1"));
        dbAccess.ExecuteNonQuery(insertRunJobEnd, insertRunJobEndSqlParams);

        String insertRunJobIcon = "insert into ja_run_icon_job_table "
                + "(inner_job_id, inner_jobnet_id, host_flag, stop_flag, command_type, timeout,host_name, stop_code) "
                + "VALUES (?,?,?,?,?,?,?,?)";
        List<ComSqlParam> insertRunJobIconSqlParams = new List<ComSqlParam>();
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@host_flag", rowIconJob[0]["host_flag"]));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@stop_flag", rowIconJob[0]["stop_flag"]));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@command_type", rowIconJob[0]["command_type"]));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@timeout", rowIconJob[0]["timeout"]));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@host_name", rowIconJob[0]["host_name"]));
        insertRunJobIconSqlParams.Add(new ComSqlParam(DbType.String, "@stop_code", rowIconJob[0]["stop_code"]));
        dbAccess.ExecuteNonQuery(insertRunJobIcon, insertRunJobIconSqlParams);

        String insertRunEndIcon = "insert into ja_run_icon_end_table "
                + "(inner_job_id, inner_jobnet_id) "
                + "VALUES (?,?)";
        List<ComSqlParam> insertRunEndIconSqlParams = new List<ComSqlParam>();

//      insertRunEndIconSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
        insertRunEndIconSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdEnd));

        insertRunEndIconSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        dbAccess.ExecuteNonQuery(insertRunEndIcon, insertRunEndIconSqlParams);

        String insertRunFlow1 = "insert into ja_run_flow_table "
                + "(inner_flow_id, inner_jobnet_id, start_inner_job_id, end_inner_job_id) "
                + "VALUES (?,?,?,?)";
        List<ComSqlParam> insertRunFlow1SqlParams = new List<ComSqlParam>();
        insertRunFlow1SqlParams.Add(new ComSqlParam(DbType.String, "@inner_flow_id", strInnerFlowNextId1));
        insertRunFlow1SqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunFlow1SqlParams.Add(new ComSqlParam(DbType.String, "@start_inner_job_id", strInnerJobNextIdStart));
        insertRunFlow1SqlParams.Add(new ComSqlParam(DbType.String, "@end_inner_job_id", strInnerJobNextIdJob));
        dbAccess.ExecuteNonQuery(insertRunFlow1, insertRunFlow1SqlParams);

        String insertRunFlow2 = "insert into ja_run_flow_table "
                + "(inner_flow_id, inner_jobnet_id, start_inner_job_id, end_inner_job_id) "
                + "VALUES (?,?,?,?)";
        List<ComSqlParam> insertRunFlow2SqlParams = new List<ComSqlParam>();
        insertRunFlow2SqlParams.Add(new ComSqlParam(DbType.String, "@inner_flow_id", strInnerFlowNextId2));
        insertRunFlow2SqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
        insertRunFlow2SqlParams.Add(new ComSqlParam(DbType.String, "@start_inner_job_id", strInnerJobNextIdJob));
        insertRunFlow2SqlParams.Add(new ComSqlParam(DbType.String, "@end_inner_job_id", strInnerJobNextIdEnd));
        dbAccess.ExecuteNonQuery(insertRunFlow2, insertRunFlow2SqlParams);


        String insertRunJobCommand = "";
        foreach (DataRow row in rowJobCommand)
        {
            insertRunJobCommand = "insert into ja_run_job_command_table "
                + "(inner_job_id, inner_jobnet_id, command_cls, command) "
                + "VALUES (?,?,?,?)";
            List<ComSqlParam> insertRunJobCommandSqlParams = new List<ComSqlParam>();
            insertRunJobCommandSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
            insertRunJobCommandSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
            insertRunJobCommandSqlParams.Add(new ComSqlParam(DbType.String, "@command_cls", row["command_cls"]));
            insertRunJobCommandSqlParams.Add(new ComSqlParam(DbType.String, "@command", row["command"]));
            dbAccess.ExecuteNonQuery(insertRunJobCommand, insertRunJobCommandSqlParams);

        }
        String insertRunJobValue = "";
        foreach (DataRow row in rowJobValue)
        {
            insertRunJobValue = "insert into ja_run_value_job_table "
                            + "(inner_job_id, inner_jobnet_id, value_name, value) "
                            + "VALUES (?,?,?,?)";
            List<ComSqlParam> insertRunJobValueSqlParams = new List<ComSqlParam>();
            insertRunJobValueSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
            insertRunJobValueSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
            insertRunJobValueSqlParams.Add(new ComSqlParam(DbType.String, "@value_name", row["value_name"]));
            insertRunJobValueSqlParams.Add(new ComSqlParam(DbType.String, "@value", row["value"]));
            dbAccess.ExecuteNonQuery(insertRunJobValue, insertRunJobValueSqlParams);
        }
        String insertRunJobconValue = "";
        foreach (DataRow row in rowJobconValue)
        {
            insertRunJobconValue = "insert into ja_run_value_jobcon_table"
                            + " (inner_job_id, inner_jobnet_id, value_name) "
                            + "VALUES (?,?,?)";
            List<ComSqlParam> insertRunJobconValueSqlParams = new List<ComSqlParam>();
            insertRunJobconValueSqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", strInnerJobNextIdJob));
            insertRunJobconValueSqlParams.Add(new ComSqlParam(DbType.String, "@inner_jobnet_id", strInnerJobnetNextId));
            insertRunJobconValueSqlParams.Add(new ComSqlParam(DbType.String, "@value_name", row["value_name"]));
            dbAccess.ExecuteNonQuery(insertRunJobconValue, insertRunJobconValueSqlParams);
        }
        String updateInnerJobnetIndex = "UPDATE ja_index_table SET nextid = nextid + 1 WHERE count_id = 2";
        dbAccess.ExecuteNonQuery(updateInnerJobnetIndex);
        String updateInnerJobIndex = "UPDATE ja_index_table SET nextid = nextid + 3 WHERE count_id = 20";
        dbAccess.ExecuteNonQuery(updateInnerJobIndex);
        String updateInnerFlowIndex = "UPDATE ja_index_table SET nextid = nextid + 2 WHERE count_id = 30";
        dbAccess.ExecuteNonQuery(updateInnerFlowIndex);

        dbAccess.TransactionCommit();
        dbAccess.CloseSqlConnect();

        JobnetExecDetail detail = new JobnetExecDetail(strInnerJobnetNextId, false);
        detail.Topmost = true;
        detail.Show();
    }

    //*******************************************************************
    /// <summary>選択部品をコピー</summary>
    //*******************************************************************
    private void SaveSelectedControlCollection()
    {
        //前のコピーをクリアする
        ClearSaveCollection();
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1)
            return;
        String where;
        String whereFlowStart="";
        String whereFlowEnd="";
        DataRow endRow;
        DataRow ifRow;
        DataRow envRow;
        DataRow jobRow;
        DataRow commandRow;
        DataRow valueRow;
        DataRow jobconValueRow;
        DataRow jobnetRow;
        DataRow extJobRow;
        DataRow calcRow;
        DataRow taskRow;
        DataRow infoRow;
        DataRow fcopyRow;
        DataRow fwaitRow;
        DataRow rebootRow;
        DataRow releaseRow;

        //added by YAMA 2014/02/06
        DataRow cooperationRow;
        //added by YAMA 2014/05/19
        DataRow agentlessRow;

        Hashtable jobAllData;

        int iconCount = 0;
        for (int i = CurrentSelectedControlCollection.Count - 1; i >= 0; i--)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                iconCount++;
                // ジョブを削除
                CommonItem item = (CommonItem)_currentSelectedControlCollection[i];

                string jobid = item.JobId;
                where = "job_id='" + jobid + "'";
                if (iconCount == 1)
                {
                    whereFlowStart = "start_job_id='" + jobid + "'";
                    whereFlowEnd = "end_job_id='" + jobid + "'";
                }
                else
                {
                    whereFlowStart = whereFlowStart+" or start_job_id='" + jobid + "'";
                    whereFlowEnd = whereFlowEnd + " or end_job_id='" + jobid + "'";
                }
                DataRow[] rows = JobControlTable.Select(where);
                DataRow jobControlRow = JobControlTable.NewRow();
                jobControlRow.ItemArray = rows[0].ItemArray;

                ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveJobControlRows.Add(rows[0]);

                // アイコン設定テーブルにデータを削除
                ElementType type = (ElementType)(Convert.ToInt16(rows[0]["job_type"]));
                switch (type)
                {
                    // 0:開始、6:並行処理開始、7：並行処理終了、8：ループの場合
                    case ElementType.START:
                    case ElementType.LOOP:
                    case ElementType.MTS:
                    case ElementType.MTE:
                    case ElementType.IFE:
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, null);
                        break;
                    // 1:終了の場合
                    case ElementType.END:
                        endRow = IconEndTable.NewRow();
                        endRow.ItemArray = IconEndTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, endRow);
                        break;
                    // 2:条件分岐の場合
                    case ElementType.IF:
                        ifRow = IconIfTable.NewRow();
                        ifRow.ItemArray = IconIfTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, ifRow);
                        break;
                    // 3:ジョブコントローラ変数の場合
                    case ElementType.ENV:
                        if (IconValueTable.Select(where).Length > 0)
                        {
                            List<DataRow> valuedDataList = new List<DataRow>();
                            foreach (DataRow row in IconValueTable.Select(where))
                            {
                                envRow = IconValueTable.NewRow();
                                envRow.ItemArray = row.ItemArray;
                                valuedDataList.Add(envRow);
                            }
                            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, valuedDataList);
                        }
                        else
                        {
                            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, null);
                        }
                        break;
                    // 4:ジョブの場合
                    case ElementType.JOB:
                        jobRow = IconJobTable.NewRow();
                        jobRow.ItemArray = IconJobTable.Select(where)[0].ItemArray;
                        jobAllData = new Hashtable();
                        jobAllData.Add(JOB_DIST, jobRow);
                        if (JobCommandTable.Select(where).Length > 0)
                        {
                            List<DataRow> commandDataList = new List<DataRow>();
                            foreach (DataRow row in JobCommandTable.Select(where))
                            {
                                commandRow = JobCommandTable.NewRow();
                                commandRow.ItemArray = row.ItemArray;
                                commandDataList.Add(commandRow);
                            }
                            jobAllData.Add(COMMAND_DIST, commandDataList);
                        }
                        if (ValueJobTable.Select(where).Length > 0)
                        {
                            List<DataRow> valueDataList = new List<DataRow>();
                            foreach (DataRow row in ValueJobTable.Select(where))
                            {
                                valueRow = ValueJobTable.NewRow();
                                valueRow.ItemArray = row.ItemArray;
                                valueDataList.Add(valueRow);
                            }
                            jobAllData.Add(JOB_VALUE_DIST, valueDataList);
                        }

                        if (ValueJobConTable.Select(where).Length > 0)
                        {
                            List<DataRow> jobconValueDataList = new List<DataRow>();
                            foreach (DataRow row in ValueJobConTable.Select(where))
                            {
                                jobconValueRow = ValueJobConTable.NewRow();
                                jobconValueRow.ItemArray = row.ItemArray;
                                jobconValueDataList.Add(jobconValueRow);
                            }
                            jobAllData.Add(JOBCON_VALUE_DIST, jobconValueDataList);
                        }

                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, jobAllData);
                        break;
                    // 5:ジョブネットの場合
                    case ElementType.JOBNET:
                        jobnetRow = IconJobnetTable.NewRow();
                        jobnetRow.ItemArray = IconJobnetTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, jobnetRow);
                        break;
                    // 9：拡張ジョブの場合
                    case ElementType.EXTJOB:
                        extJobRow = IconExtjobTable.NewRow();
                        extJobRow.ItemArray = IconExtjobTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, extJobRow);
                        break;
                    //  10：計算の場合
                    case ElementType.CAL:
                        calcRow = IconCalcTable.NewRow();
                        calcRow.ItemArray = IconCalcTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, calcRow);
                        break;
                    // 11：タスク場合
                    case ElementType.TASK:
                        taskRow = IconTaskTable.NewRow();
                        taskRow.ItemArray = IconTaskTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, taskRow);
                        break;
                    // 12：情報取得場合
                    case ElementType.INF:
                        infoRow = IconInfoTable.NewRow();
                        infoRow.ItemArray = IconInfoTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, infoRow);
                        break;
                    // 14：ファイル転送場合
                    case ElementType.FCOPY:
                        fcopyRow = IconFcopyTable.NewRow();
                        fcopyRow.ItemArray = IconFcopyTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, fcopyRow);
                        break;
                    // 15：ファイル待ち合わせ場合
                    case ElementType.FWAIT:
                        fwaitRow = IconFwaitTable.NewRow();
                        fwaitRow.ItemArray = IconFwaitTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, fwaitRow);
                        break;
                    // 16：リブートの場合
                    case ElementType.REBOOT:
                        rebootRow = IconRebootTable.NewRow();
                        rebootRow.ItemArray = IconRebootTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, rebootRow);
                        break;
                    // 17：保留解除の場合
                    case ElementType.RELEASE:
                        releaseRow = IconReleaseTable.NewRow();
                        releaseRow.ItemArray = IconReleaseTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, releaseRow);
                        break;
                    //added by YAMA 2014/02/06
                    // 18：Zabbix連携の場合
                    case ElementType.COOPERATION:
                        cooperationRow = IconCooperationTable.NewRow();
                        cooperationRow.ItemArray = IconCooperationTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, cooperationRow);
                        break;
                    //added by YAMA 2014/05/19
                    // 19：エージェントレスの場合
                    case ElementType.AGENTLESS:
                        agentlessRow = IconAgentlessTable.NewRow();
                        agentlessRow.ItemArray = IconAgentlessTable.Select(where)[0].ItemArray;
                        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Add(jobControlRow, agentlessRow);
                        break;
                }
                if (SetedJobIds.Contains(jobid))
                {
                    ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveSetedJobIds[jobid] = "1";
                }
            }
        }

        IEnumerable<DataRow> query = FlowControlTable.Select("(" + whereFlowStart + ") and (" + whereFlowEnd + ")");
        if (query.Count()>0)
            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveFlows = query.CopyToDataTable();
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveMinX = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveJobControlRows.Min(r => Convert.ToDouble(r["point_x"]));
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveMinY = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveJobControlRows.Min(r => Convert.ToDouble(r["point_y"]));

    }

    //*******************************************************************
    /// <summary>選択部品をコピー</summary>
    //*******************************************************************
    private void PasteSelectedControlCollection()
    {
        TempHash.Clear();
        TempJobNo = 0;
        if (((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Keys.Count > 0)
        {
            Point p = Mouse.GetPosition(cnsDesignerContainer);

            //キャンバス内ではない場合、コピーしない
            if (p.X*zoomSlider.Value - svContainer.HorizontalOffset <= 0 || p.Y*zoomSlider.Value - svContainer.VerticalOffset <= 0)
            {
                return;
            }
            if (p.X*zoomSlider.Value - svContainer.HorizontalOffset >= svContainer.ViewportWidth || p.Y*zoomSlider.Value - svContainer.VerticalOffset >= svContainer.ViewportHeight)
            {
                return;
            }

            //処理前現在データで履歴を作成
            CreateHistData();

            double deltaX = p.X - ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveMinX;
            double deltaY = p.Y - ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveMinY;
            DataRow jobControlRow;
            DataTable workFlowTable = new DataTable();
            DataRow flowControlRow;
            DataRow iconRow;
            DataRow commandRow;
            DataRow jobValueRow;
            DataRow jobconValueRow;
            String oldJobId;
            String tmpJobId="";
            String jobId;
            ElementType type;
            CommonItem room;
            Hashtable jobHash;
            DataRow jobnetIconRow = null;
            // ジョブデータ（ジョブアイコンの生成用）
            JobData jobData = null;
            bool needFlow = false;
            needFlow = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveFlows.Rows.Count > 0;
            if (needFlow)
                workFlowTable = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveFlows.Copy();
            foreach (DictionaryEntry item in ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems)
            {
                jobControlRow = JobControlTable.NewRow();
                oldJobId = (String)((DataRow)item.Key)["job_id"];
                jobControlRow.ItemArray = ((DataRow)item.Key).ItemArray;
                type = (ElementType)jobControlRow["job_type"];
                jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).JobNoHash, type);
                if (type == ElementType.JOBNET)
                {
                    jobnetIconRow = (DataRow)item.Value;
                    jobId = oldJobId;
                }

                // 既存の場合、繰り返して取得
                int count = 0;
                while (ElementType.START != type
                    && JobItems.ContainsKey(jobId))
                {
                    count++;
                    jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).JobNoHash, type);
                    if (type == ElementType.JOBNET)
                    {
                        jobId = jobnetIconRow["link_jobnet_id"].ToString() + "-" + count;
                    }
                }
                jobControlRow["job_id"] = jobId;
                jobControlRow["jobnet_id"] = JobnetId;
                jobData = new JobData();
                // ジョブタイプ
                jobData.JobType = type;
                room = new CommonItem(this, jobData, Consts.EditType.Modify, (RunJobMethodType)jobControlRow["method_flag"]);
                // ジョブID
                room.JobId = jobId;
                //ジョブ名
                room.JobName = Convert.ToString(jobControlRow["job_name"]);
                // X位置
                room.SetValue(Canvas.LeftProperty, Convert.ToDouble(jobControlRow["point_x"]) + deltaX);
                // Y位置
                room.SetValue(Canvas.TopProperty, Convert.ToDouble(jobControlRow["point_y"]) + deltaY);
                jobControlRow["point_x"] = Convert.ToDouble(jobControlRow["point_x"]) + deltaX;
                jobControlRow["point_y"] = Convert.ToDouble(jobControlRow["point_y"]) + deltaY;

                if (JobControlTable.Select("job_type=0").Count() == 0 || ElementType.START != type)
                {
                    JobControlTable.Rows.Add(jobControlRow);
                    // ジョブフロー領域に追加
                    ContainerCanvas.Children.Add(room);
                    JobItems.Add(room.JobId, room);
                    if (needFlow)
                    {
                        if (workFlowTable.Select("start_job_id='" + jobId + "' or end_job_id='" + jobId + "'").Count() > 0)
                        {
                            tmpJobId = GetNextTempJobId();
                            workFlowTable.Select("start_job_id='" + jobId + "'").ToList<DataRow>().ForEach(r => r["start_job_id"] = tmpJobId);
                            workFlowTable.Select("end_job_id='" + jobId + "'").ToList<DataRow>().ForEach(r => r["end_job_id"] = tmpJobId);
                            TempHash[jobId] = tmpJobId;
                        }
                        if (TempHash.Contains(oldJobId))
                        {
                            workFlowTable.Select("start_job_id='" + TempHash[oldJobId] + "'").ToList<DataRow>().ForEach(r => r["start_job_id"] = jobId);
                            workFlowTable.Select("end_job_id='" + TempHash[oldJobId] + "'").ToList<DataRow>().ForEach(r => r["end_job_id"] = jobId);
                        }
                        else
                        {
                            workFlowTable.Select("start_job_id='" + oldJobId + "'").ToList<DataRow>().ForEach(r => r["start_job_id"] = jobId);
                            workFlowTable.Select("end_job_id='" + oldJobId + "'").ToList<DataRow>().ForEach(r => r["end_job_id"] = jobId);
                        }
                    }
                }
                else
                {
                    if (needFlow)
                    {
                        DataRow[] deleteRows = workFlowTable.Select("start_job_id='" + oldJobId + "'");
                        foreach (DataRow row in deleteRows)
                            workFlowTable.Rows.Remove(row);
                    }

                }

                if (((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveSetedJobIds.Contains(((DataRow)item.Key)["job_id"].ToString()))
                {
                    SetedJobIds[jobId] = "1";
                }
                switch (type)
                {
                    // 0:開始、6:並行処理開始、7：並行処理終了、8：ループの場合
                    case ElementType.START:
                    case ElementType.LOOP:
                    case ElementType.MTS:
                    case ElementType.MTE:
                    case ElementType.IFE:
                        break;
                    // 1:終了の場合
                    case ElementType.END:
                        iconRow = IconEndTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconEndTable.Rows.Add(iconRow);
                        break;
                    // 2:条件分岐の場合
                    case ElementType.IF:
                        iconRow = IconIfTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconIfTable.Rows.Add(iconRow);
                        break;
                    // 3:ジョブコントローラ変数の場合
                    case ElementType.ENV:
                        if (item.Value != null)
                        {
                            foreach (DataRow row in (List<DataRow>)item.Value)
                            {
                                iconRow = IconValueTable.NewRow();
                                iconRow.ItemArray = row.ItemArray;
                                iconRow["job_id"] = jobId;
                                iconRow["jobnet_id"] = JobnetId;
                                IconValueTable.Rows.Add(iconRow);
                            }
                        }
                        break;
                    // 4:ジョブの場合
                    case ElementType.JOB:
                        iconRow = IconJobTable.NewRow();
                        jobHash = (Hashtable)item.Value;
                        iconRow.ItemArray = ((DataRow)jobHash[JOB_DIST]).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconJobTable.Rows.Add(iconRow);
                        if (jobHash.Contains(COMMAND_DIST))
                        {
                            foreach (DataRow row in (List<DataRow>)jobHash[COMMAND_DIST])
                            {
                                commandRow = JobCommandTable.NewRow();
                                commandRow.ItemArray = row.ItemArray;
                                commandRow["job_id"] = jobId;
                                commandRow["jobnet_id"] = JobnetId;
                                JobCommandTable.Rows.Add(commandRow);
                            }
                        }
                        if (jobHash.Contains(JOB_VALUE_DIST))
                        {
                            foreach (DataRow row in (List<DataRow>)jobHash[JOB_VALUE_DIST])
                            {
                                jobValueRow = ValueJobTable.NewRow();
                                jobValueRow.ItemArray = row.ItemArray;
                                jobValueRow["job_id"] = jobId;
                                jobValueRow["jobnet_id"] = JobnetId;
                                ValueJobTable.Rows.Add(jobValueRow);
                            }
                        }

                        if (jobHash.Contains(JOBCON_VALUE_DIST))
                        {
                            foreach (DataRow row in (List<DataRow>)jobHash[JOBCON_VALUE_DIST])
                            {
                                jobconValueRow = ValueJobConTable.NewRow();
                                jobconValueRow.ItemArray = row.ItemArray;
                                jobconValueRow["job_id"] = jobId;
                                jobconValueRow["jobnet_id"] = JobnetId;
                                ValueJobConTable.Rows.Add(jobconValueRow);
                            }
                        }

                        break;
                    // 5:ジョブネットの場合
                    case ElementType.JOBNET:
                        iconRow = IconJobnetTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconJobnetTable.Rows.Add(iconRow);
                        break;
                    // 9：拡張ジョブの場合
                    case ElementType.EXTJOB:
                        iconRow = IconExtjobTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconExtjobTable.Rows.Add(iconRow);
                        break;
                    //  10：計算の場合
                    case ElementType.CAL:
                        iconRow = IconCalcTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconCalcTable.Rows.Add(iconRow);
                        break;
                    // 11：タスク場合
                    case ElementType.TASK:
                        iconRow = IconTaskTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconTaskTable.Rows.Add(iconRow);
                        break;
                    // 12：情報取得場合
                    case ElementType.INF:
                        iconRow = IconInfoTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconInfoTable.Rows.Add(iconRow);
                        break;
                    // 14：ファイル転送場合
                    case ElementType.FCOPY:
                        iconRow = IconFcopyTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconFcopyTable.Rows.Add(iconRow);
                        break;
                    // 15：ファイル待ち合わせ場合
                    case ElementType.FWAIT:
                        iconRow = IconFwaitTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconFwaitTable.Rows.Add(iconRow);
                        break;
                    // 16：リブートの場合
                    case ElementType.REBOOT:
                        iconRow = IconRebootTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconRebootTable.Rows.Add(iconRow);
                        break;
                    // 17：保留解除の場合
                    case ElementType.RELEASE:
                        iconRow = IconReleaseTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconReleaseTable.Rows.Add(iconRow);
                        break;
                    //added by YAMA 2014/02/06
                    // 18：Zabbix連携の場合
                    case ElementType.COOPERATION:
                        iconRow = IconCooperationTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconCooperationTable.Rows.Add(iconRow);
                        break;
                    //added by YAMA 2014/05/19
                    // 19：エージェントレスの場合
                    case ElementType.AGENTLESS:
                        iconRow = IconAgentlessTable.NewRow();
                        iconRow.ItemArray = ((DataRow)item.Value).ItemArray;
                        iconRow["job_id"] = jobId;
                        iconRow["jobnet_id"] = JobnetId;
                        IconAgentlessTable.Rows.Add(iconRow);
                        break;
                }
            }
            // フローを表示------------------
            // 開始ジョブID、終了ジョブId
            string startJobId, endJobId;
            // 開始ジョブ、終了ジョブ
            IRoom startJob, endJob;
            // フロー幅
            int flowWidth;
            // フロータイプ(直線、曲線)
            FlowLineType lineType;
            // フロータイプ（　0：通常、　1：TURE、　2：FALSE）
            int flowType = 0;
            foreach (DataRow row in workFlowTable.Rows)
            {
                flowControlRow = FlowControlTable.NewRow();
                flowControlRow.ItemArray = row.ItemArray;
                flowControlRow["jobnet_id"] = JobnetId;
                FlowControlTable.Rows.Add(flowControlRow);
                startJobId = Convert.ToString(flowControlRow["start_job_id"]);
                endJobId = Convert.ToString(flowControlRow["end_job_id"]);
                flowWidth = Convert.ToInt16(flowControlRow["flow_width"]);
                flowType = Convert.ToInt16(flowControlRow["flow_type"]);

                // フロータイプの判定
                if (flowWidth == 0)
                {
                    lineType = FlowLineType.Line;
                }
                else
                {
                    lineType = FlowLineType.Curve;
                }

                startJob = (IRoom)JobItems[startJobId];
                endJob = (IRoom)JobItems[endJobId];

                MakeFlow(lineType, startJob, endJob, flowType, Consts.EditType.Modify);
            }

        }

    }

    private String GetNextTempJobId()
    {
        TempJobNo = TempJobNo + 1;
        return "temp_" + TempJobNo;
    }

    //*******************************************************************
    /// <summary>コピーをクリア</summary>
    //*******************************************************************
    private void ClearSaveCollection()
    {
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveJobControlRows.Clear();
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveItems.Clear();
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ParantWindow.SaveFlows.Clear();
    }

    //*******************************************************************
    /// <summary>UNDO処理</summary>
    //********a***********************************************************
    private void Undo()
    {
        if (HistoryDataList.Count > 0)
        {
            HistoryData lastHist = HistoryDataList.Last<HistoryData>();
            UndoDatas(lastHist);
            UndoViews();
            HistoryDataList[HistoryDataList.Count - 1] = null;
            HistoryDataList.RemoveAt(HistoryDataList.Count - 1);
        }
    }

    //*******************************************************************
    /// <summary>保留処理</summary>
    //*******************************************************************
    private void SetHold(IElement item)
    {
        String jobId = item.JobId;
        DataRow[] rows = JobControlTable.Select("job_id='" + jobId + "'");

        rows[0]["method_flag"] = (int)RunJobMethodType.HOLD;
        item.MethodType = RunJobMethodType.HOLD;
    }
    //*******************************************************************
    /// <summary>スキップ処理</summary>
    //*******************************************************************
    private void SetSkip(IElement item)
    {
        String jobId = item.JobId;
        DataRow[] rows = JobControlTable.Select("job_id='" + jobId + "'");

        rows[0]["method_flag"] = (int)RunJobMethodType.SKIP;
        item.MethodType = RunJobMethodType.SKIP;
    }

    //*******************************************************************
    /// <summary>保留、スキップ解除処理</summary>
    //*******************************************************************
    private void SetUnHoldORUnSkip(IElement item)
    {
        String jobId = item.JobId;
        DataRow[] rows = JobControlTable.Select("job_id='" + jobId + "'");

        rows[0]["method_flag"] = (int)RunJobMethodType.NORMAL;
        item.MethodType = RunJobMethodType.NORMAL;
    }

    //*******************************************************************
    /// <summary>データのUNDO</summary>
    //*******************************************************************
    private void UndoDatas(HistoryData hist)
    {
        JobControlTable = hist.JobControlTable.Copy();
        JobControlTable.AcceptChanges();
        IconCalcTable = hist.IconCalcTable.Copy();
        IconEndTable = hist.IconEndTable.Copy();
        IconExtjobTable = hist.IconExtjobTable.Copy();
        IconFcopyTable = hist.IconFcopyTable.Copy();
        IconFwaitTable = hist.IconFwaitTable.Copy();
        IconIfTable = hist.IconIfTable.Copy();
        IconInfoTable = hist.IconInfoTable.Copy();
        IconJobnetTable = hist.IconJobnetTable.Copy();
        IconJobTable = hist.IconJobTable.Copy();
        IconRebootTable = hist.IconRebootTable.Copy();
        IconReleaseTable = hist.IconReleaseTable.Copy();

        //added by YAMA 2014/02/06
        IconCooperationTable = hist.IconCooperationTable.Copy();
        //added by YAMA 2014/05/19
        IconAgentlessTable = hist.IconAgentlessTable.Copy();

        IconTaskTable = hist.IconTaskTable.Copy();
        IconValueTable = hist.IconValueTable.Copy();
        JobCommandTable = hist.JobCommandTable.Copy();
        ValueJobConTable = hist.ValueJobConTable.Copy();
        ValueJobTable = hist.ValueJobTable.Copy();
        FlowControlTable = hist.FlowControlTable.Copy();
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).ResetDataSet();
        SetedJobIds = (Hashtable)hist.SetedJobIds.Clone();
        ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)ParantWindow).JobNoHash =  (Hashtable)hist.JobIdNos.Clone();
    }

    //*******************************************************************
    /// <summary>表示のUNDO</summary>
    //*******************************************************************
    private void UndoViews()
    {
        ContainerCanvas.Children.Clear();
        JobItems.Clear();
        // ジョブデータ（ジョブアイコンの生成用）
        JobData jobData = null;

        // ジョブを表示------------------
        foreach (DataRow row in JobControlTable.Select())
        {
            jobData = new JobData();
            // ジョブタイプ
            jobData.JobType = (ElementType)row["job_type"];

            CommonItem room = new CommonItem(this, jobData, Consts.EditType.Modify, (RunJobMethodType)row["method_flag"]);
            // ジョブID
            room.JobId = Convert.ToString(row["job_id"]);
            //ジョブ名
            room.JobName = Convert.ToString(row["job_name"]);
            // X位置
            room.SetValue(Canvas.LeftProperty, Convert.ToDouble(row["point_x"]));
            // Y位置
            room.SetValue(Canvas.TopProperty, Convert.ToDouble(row["point_y"]));


            // ジョブフロー領域に追加
            ContainerCanvas.Children.Add(room);
            JobItems.Add(room.JobId, room);
        }

        // フローを表示------------------
        // 開始ジョブID、終了ジョブId
        string startJobId, endJobId;
        // 開始ジョブ、終了ジョブ
        IRoom startJob, endJob;
        // フロー幅
        int flowWidth;
        // フロータイプ(直線、曲線)
        FlowLineType lineType;
        // フロータイプ（　0：通常、　1：TURE、　2：FALSE）
        int flowType = 0;
        foreach (DataRow row in FlowControlTable.Select())
        {
            startJobId = Convert.ToString(row["start_job_id"]);
            endJobId = Convert.ToString(row["end_job_id"]);
            flowWidth = Convert.ToInt16(row["flow_width"]);
            flowType = Convert.ToInt16(row["flow_type"]);

            // フロータイプの判定
            if (flowWidth == 0)
            {
                lineType = FlowLineType.Line;
            }
            else
            {
                lineType = FlowLineType.Curve;
            }

            startJob = (IRoom)JobItems[startJobId];
            endJob = (IRoom)JobItems[endJobId];

            MakeFlow(lineType, startJob, endJob, flowType, Consts.EditType.Modify);
        }
    }

    #region コンテクストの制限処理

    //*******************************************************************
    /// <summary>コンテクスの利用可能をセット</summary>
    //*******************************************************************
    private void SetContextStatus()
    {
        // フローを利用可能判定
        if (!IsFlowEnable())
        {
            menuitemLine.IsEnabled = false;
            menuitemCurve.IsEnabled = false;
        }
        else
        {
            menuitemLine.IsEnabled = true;
            menuitemCurve.IsEnabled = true;
        }

        // TRUE設定を利用可能判定
        if (!IsSetTrueEnable())
            menuitemSetTrue.IsEnabled = false;
        else
            menuitemSetTrue.IsEnabled = true;

        // TRUE設定を利用可能判定
        if (!IsSetFalseEnable())
            menuitemSetFalse.IsEnabled = false;
        else
            menuitemSetFalse.IsEnabled = true;

        // フロー削除を利用可能判定
        if (!IsDelFlowEnable())
            menuitemDelFlow.IsEnabled = false;
        else
            menuitemDelFlow.IsEnabled = true;

        // アイコン削除を利用可能判定
        if (!IsDelIconEnable())
            menuitemDel.IsEnabled = false;
        else
            menuitemDel.IsEnabled = true;

        // アイコン削除を利用可能判定
        if (!IsSettingEnable())
            menuitemSet.IsEnabled = false;
        else
            menuitemSet.IsEnabled = true;

        // ジョブ起動を利用可能判定
        if (!IsJobRunEnable())
            menuitemJobRun.IsEnabled = false;
        else
            menuitemJobRun.IsEnabled = true;

        // 保留可能判定
        if (!IsHoldEnable())
            menuitemHold.IsEnabled = false;
        else
            menuitemHold.IsEnabled = true;

        // 保留解除可能判定
        if (!IsUnHoldEnable())
            menuitemUnHold.IsEnabled = false;
        else
            menuitemUnHold.IsEnabled = true;

        // スキップ可能判定
        if (!IsSkipEnable())
            menuitemSkip.IsEnabled = false;
        else
            menuitemSkip.IsEnabled = true;

        // スキップ解除可能判定
        if (!IsUnSkipEnable())
            menuitemUnSkip.IsEnabled = false;
        else
            menuitemUnSkip.IsEnabled = true;
    }

    //*******************************************************************
    /// <summary>フローを利用可能判定</summary>
    //*******************************************************************
    private bool IsFlowEnable()
    {
        bool result = true;

        result = IsFlowEnablePositiveDirection();
        if (!result && !IsSelectedByShiftKey)
        {
            result = IsFlowEnableReverseDirection();
        }
        return result;
    }

    //*******************************************************************
    /// <summary>フローを利用可能判定</summary>
    //*******************************************************************
    private bool IsFlowEnablePositiveDirection()
    {
        // アイコン未選択、または２つ以外のアイコンを選択
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 2
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(_currentSelectedControlCollection[1] is IRoom))
            return false;

        IElement item1 = ((IRoom)_currentSelectedControlCollection[0]).ContentItem;
        IElement item2 = ((IRoom)_currentSelectedControlCollection[1]).ContentItem;

        // 最初に選択されたアイコンが終了アイコン
        if (item1 is End)
            return false;

        // 最後に選択されたアイコンが開始アイコン
        if (item2 is Start)
            return false;

        // すでにフローが設定済みのアイコンを選択。（フロー管理テーブルに同一データが存在する）
        string job1 = item1.JobId;
        string job2 = item2.JobId;

        DataRow[] existFlows = FlowControlTable.Select
            ("start_job_id ='" + job1 + "' and end_job_id='" + job2 + "'");
        if (existFlows != null && existFlows.Count() > 0)
            return false;

        // 最初に選択されたアイコンが開始アイコンで、


        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Start)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが終了アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is End)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが条件分岐アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is If)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが条件分岐アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に２件以上のデータが存在する
        if (item1 is If)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 2)
                return false;
        }

        // 最後に選択されたアイコンが並行処理開始アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is Mts)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが並行処理終了アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Mte)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        //added by kim 2012/11/15
        // 最初に選択されたアイコンが分岐処理終了アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Ife)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンがループアイコンで、
        // フロー管理テーブル（ＩＮフロージョブIDをキー）に２件以上のデータが存在する
        if (item2 is Loop)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 2)
                return false;
        }

        // 最初に選択されたアイコンがループアイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Loop)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが上記以外のアイコンで、
        // (開始、終了、条件分岐、分岐終了、並行処理開始、並行処理終了、ループ以外)
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (!(item2 is Start) && !(item2 is End) && !(item2 is If)
            && !(item2 is Mts) && !(item2 is Mte) && !(item2 is Loop) && !(item2 is Ife))
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが上記以外のアイコンで、
        // (開始、終了、条件分岐、分岐終了、並行処理開始、並行処理終了、ループ以外)
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (!(item1 is Start) && !(item1 is End) && !(item1 is If)
            && !(item1 is Mts) && !(item1 is Mte) && !(item1 is Loop) && !(item1 is Ife))
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        return true;
    }

    //*******************************************************************
    /// <summary>フローを利用可能判定</summary>
    //*******************************************************************
    private bool IsFlowEnableReverseDirection()
    {
        // アイコン未選択、または２つ以外のアイコンを選択
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 2
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(_currentSelectedControlCollection[1] is IRoom))
            return false;

        IElement item1 = ((IRoom)_currentSelectedControlCollection[1]).ContentItem;
        IElement item2 = ((IRoom)_currentSelectedControlCollection[0]).ContentItem;

        // 最初に選択されたアイコンが終了アイコン
        if (item1 is End)
            return false;

        // 最後に選択されたアイコンが開始アイコン
        if (item2 is Start)
            return false;

        // すでにフローが設定済みのアイコンを選択。（フロー管理テーブルに同一データが存在する）
        string job1 = item1.JobId;
        string job2 = item2.JobId;

        DataRow[] existFlows = FlowControlTable.Select
            ("start_job_id ='" + job1 + "' and end_job_id='" + job2 + "'");
        if (existFlows != null && existFlows.Count() > 0)
            return false;

        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Start)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが終了アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is End)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが条件分岐アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is If)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが条件分岐アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に２件以上のデータが存在する
        if (item1 is If)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 2)
                return false;
        }

        // 最後に選択されたアイコンが並行処理開始アイコンで、
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (item2 is Mts)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが並行処理終了アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Mte)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        //added by kim 2012/11/15
        // 最初に選択されたアイコンが分岐処理終了アイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Ife)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンがループアイコンで、
        // フロー管理テーブル（ＩＮフロージョブIDをキー）に２件以上のデータが存在する
        if (item2 is Loop)
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 2)
                return false;
        }

        // 最初に選択されたアイコンがループアイコンで、
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (item1 is Loop)
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最後に選択されたアイコンが上記以外のアイコンで、
        // (開始、終了、条件分岐、分岐終了、並行処理開始、並行処理終了、ループ以外)
        // フロー管理テーブル（INフロージョブIDをキー）に１件以上のデータが存在する
        if (!(item2 is Start) && !(item2 is End) && !(item2 is If)
            && !(item2 is Mts) && !(item2 is Mte) && !(item2 is Loop) && !(item2 is Ife))
        {
            existFlows = FlowControlTable.Select
                ("end_job_id='" + job2 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }

        // 最初に選択されたアイコンが上記以外のアイコンで、
        // (開始、終了、条件分岐、分岐終了、並行処理開始、並行処理終了、ループ以外)
        // フロー管理テーブル（OUTフロージョブIDをキー）に１件以上のデータが存在する
        if (!(item1 is Start) && !(item1 is End) && !(item1 is If)
            && !(item1 is Mts) && !(item1 is Mte) && !(item1 is Loop) && !(item1 is Ife))
        {
            existFlows = FlowControlTable.Select
                ("start_job_id='" + job1 + "'");
            if (existFlows != null && existFlows.Count() >= 1)
                return false;
        }
        _currentSelectedControlCollection.Reverse();
        return true;
    }

    //*******************************************************************
    /// <summary>TRUE設定を利用可能判定</summary>
    //*******************************************************************
    private bool IsSetTrueEnable()
    {
        // 対象：条件分岐アイコンに設定されたOUTフロー
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IFlow))
            return false;

        IFlow flow = (IFlow)_currentSelectedControlCollection[0];
        IRoom item1 = flow.BeginItem;
        //IRoom item2 = flow.EndItem;
        string beginJob = item1.JobId;
        //string endJob = item2.JobId;

        // フロー管理テーブルのOUTフロージョブIDをキーにジョブ管理テーブルを検索し、
        // ジョブタイプが「条件分岐」以外
        DataRow[] existFlows = JobControlTable.Select
            ("job_id ='" + beginJob + "'");

        if (existFlows != null && existFlows.Count() > 0)
        {
            ElementType type = (ElementType)(existFlows[0]["job_type"]);
            if (ElementType.IF != type)
                return false;
        }

        // フロー管理テーブルのOUTフロージョブIDとフロータイプ「TRUE」をキーに、
        // フロー管理テーブルのOUTフロージョブIDで検索し、
        // １件以上のデータが存在する
        existFlows = FlowControlTable.Select
            ("start_job_id ='" + beginJob + "' and flow_type=1");

        if (existFlows != null && existFlows.Count() >= 1)
        {
            return false;
        }

        return true;
    }

    //*******************************************************************
    /// <summary>FALSE設定を利用可能判定</summary>
    //*******************************************************************
    private bool IsSetFalseEnable()
    {
        // 対象：条件分岐アイコンに設定されたOUTフロー
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IFlow))
            return false;

        IFlow flow = (IFlow)_currentSelectedControlCollection[0];
        IRoom item1 = flow.BeginItem;
        //IRoom item2 = flow.EndItem;
        string beginJob = item1.JobId;
        //string endJob = item2.JobId;

        // フロー管理テーブルのOUTフロージョブIDをキーにジョブ管理テーブルを検索し、
        // ジョブタイプが「条件分岐」以外
        DataRow[] existFlows = JobControlTable.Select
            ("job_id ='" + beginJob + "'");

        if (existFlows != null && existFlows.Count() > 0)
        {
            ElementType type = (ElementType)(existFlows[0]["job_type"]);
            if (ElementType.IF != type)
                return false;
        }

        // フロー管理テーブルのOUTフロージョブIDとフロータイプ「FALSE」をキーに、
        // フロー管理テーブルのOUTフロージョブIDで検索し、
        // １件以上のデータが存在する
        existFlows = FlowControlTable.Select
            ("start_job_id ='" + beginJob + "' and flow_type=2");

        if (existFlows != null && existFlows.Count() >= 1)
        {
            return false;
        }

        return true;
    }

    //*******************************************************************
    /// <summary>フロー削除を利用可能判定</summary>
    //*******************************************************************
    private bool IsDelFlowEnable()
    {
        /*
        // フロー以外が選択された場合
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IFlow))
            return false;
        */
        //added by YAMA 2014/10/24
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1)
            //  || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }

    //*******************************************************************
    /// <summary>削除を利用可能判定</summary>
    //*******************************************************************
    private bool IsDelIconEnable()
    {
        // アイコン以外が選択された場合
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }

    //*******************************************************************
    /// <summary>設定を利用可能判定</summary>
    //*******************************************************************
    private bool IsSettingEnable()
    {
        // アイコン以外が選択された場合
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }

    //*******************************************************************
    /// <summary>ジョブ起動を利用可能判定</summary>
    //*******************************************************************
    private bool IsJobRunEnable()
    {
        // アイコン以外が選択された場合
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(((CommonItem)_currentSelectedControlCollection[0]).ElementType == ElementType.JOB)
            || (!SetedJobIds.Contains(((CommonItem)_currentSelectedControlCollection[0]).JobId) && ((CommonItem)_currentSelectedControlCollection[0]).ItemEditType == Consts.EditType.Add))
            return false;

        return true;
    }

    //*******************************************************************
    /// <summary>保留を利用可能判定</summary>
    //*******************************************************************
    private bool IsHoldEnable()
    {
        /*
        // 全ての保留、スキップ設定されてないアイコンで可能
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(((CommonItem)_currentSelectedControlCollection[0]).ContentItem.MethodType == RunJobMethodType.NORMAL))
            return false;
        */
        //added by YAMA 2014/10/24
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }
    //*******************************************************************
    /// <summary>保留解除を利用可能判定</summary>
    //*******************************************************************
    private bool IsUnHoldEnable()
    {
        /*
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(((CommonItem)_currentSelectedControlCollection[0]).ContentItem.MethodType == RunJobMethodType.HOLD))
            return false;
        */
        //added by YAMA 2014/10/24
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }
    //*******************************************************************
    /// <summary>スキップ可能判定</summary>
    //*******************************************************************
    private bool IsSkipEnable()
    {
        /*
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(((CommonItem)_currentSelectedControlCollection[0]).ContentItem.MethodType == RunJobMethodType.NORMAL))
            return false;

        IElement item = ((CommonItem)_currentSelectedControlCollection[0]).ContentItem;

        // 開始、終了、条件分岐、並行処理、ループアイコンの場合利用不可
        if (item is Start || item is End || item is If || item is Ife || item is Mts || item is Mte || item is Loop)
            return false;
        */
        //added by YAMA 2014/10/24
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }
    //*******************************************************************
    /// <summary>スキップ解除を利用可能判定</summary>
    //*******************************************************************
    private bool IsUnSkipEnable()
    {
        /*
        // スキップされたアイコンで可能
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count != 1
            || !(_currentSelectedControlCollection[0] is IRoom)
            || !(((CommonItem)_currentSelectedControlCollection[0]).ContentItem.MethodType == RunJobMethodType.SKIP))
            return false;
        */
        //added by YAMA 2014/10/24
        if (_currentSelectedControlCollection == null
            || _currentSelectedControlCollection.Count < 1
            || !(_currentSelectedControlCollection[0] is IRoom))
            return false;

        return true;
    }
    #endregion
    #endregion

    #region DB処理
    //*******************************************************************
    /// <summary>ジョブ位置設定</summary>
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
    /// <summary>アイコンの存在チェック</summary>
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
    /// <summary>アイコン設定テーブルのデータを削除</summary>
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
                if(rowsTmp != null)
                {
                    foreach (DataRow row in rowsTmp)
                    {
                        row.Delete();
                    }
                }

                // ジョブ変数設定テーブル
                rowsTmp = ValueJobTable.Select(where);
                if(rowsTmp != null)
                {
                    foreach (DataRow row in rowsTmp)
                    {
                        row.Delete();
                    }
                }

                // ジョブコントローラ変数設定テーブル
                rowsTmp = ValueJobConTable.Select(where);
                if (rowsTmp != null)
                {
                    foreach (DataRow row in rowsTmp)
                    {
                        row.Delete();
                    }
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
            // 14：ファイル転送場合
            case ElementType.FCOPY:
                rows = IconFcopyTable.Select(where);
                break;
            // 15：ファイル待ち合わせ場合
            case ElementType.FWAIT:
                rows = IconFwaitTable.Select(where);
                break;
            // 16：リブート場合
            case ElementType.REBOOT:
                rows = IconRebootTable.Select(where);
                break;
            // 17：保留解除場合
            case ElementType.RELEASE:
                rows = IconReleaseTable.Select(where);
                break;
            //added by YAMA 2014/02/06
            // 18：Zabbix連携の場合
            case ElementType.COOPERATION:
                rows = IconCooperationTable.Select(where);
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレスの場合
            case ElementType.AGENTLESS:
                rows = IconAgentlessTable.Select(where);
                break;
        }
        // 削除
        if (rows != null)
        {
            foreach (DataRow row in rows)
            {
                row.Delete();
            }
        }
    }
    #endregion

    #region internalメソッド

    internal bool IsCanArrowMove(double deltaX, double deltaY)
    {
        CommonItem tmpItem = null;
        bool canMove = false;
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                tmpItem = CurrentSelectedControlCollection[i] as CommonItem;
                double X = (double)tmpItem.GetValue(Canvas.LeftProperty);
                double Y = (double)tmpItem.GetValue(Canvas.TopProperty);
                if ((X + deltaX <= 0) || (X + deltaX >= cnsDesignerContainer.Width)) return false;
                if ((Y + deltaY <= 0) || (Y + deltaY >= cnsDesignerContainer.Height)) return false;
            }
            canMove = true;
        }
        return canMove;
    }

    internal bool[] IsNeedScrollMove(double deltaX, double deltaY)
    {
        CommonItem tmpItem = null;
        bool[] need = { false, false };
        for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
        {
            if (CurrentSelectedControlCollection[i] is CommonItem)
            {
                tmpItem = CurrentSelectedControlCollection[i] as CommonItem;
                double X = (double)tmpItem.GetValue(Canvas.LeftProperty);
                double Y = (double)tmpItem.GetValue(Canvas.TopProperty);
                if (X + deltaX > svContainer.ViewportWidth) need[0] = true;
                if (Y + deltaY > svContainer.ViewportHeight) need[1] = true;
            }
        }
        return need;
    }

    internal void ScrollIfNeeded(Point mouseLocation)
    {
        if (svContainer != null)
        {
            double scrollVerticalOffset = 0.0;
            double scrollHorizontalOffset = 0.0;

            // See if we need to scroll down
            if (svContainer.ViewportHeight - mouseLocation.Y < 15.0)
            {
                scrollVerticalOffset = 1.0;
            }
            else if (mouseLocation.Y < 15.0)
            {
                scrollVerticalOffset = -1.0;
            }
            if (svContainer.ViewportWidth - mouseLocation.X < 15.0)
            {
                scrollHorizontalOffset = 1.0;
            }
            else if (mouseLocation.X < 15.0)
            {
                scrollHorizontalOffset = -1.0;
            }

            // Scroll the tree down or up
            if (scrollVerticalOffset != 0.0)
            {
                scrollVerticalOffset += svContainer.VerticalOffset;

                if (scrollVerticalOffset < 0.0)
                {
                    scrollVerticalOffset = 0.0;
                }
                else if (scrollVerticalOffset > svContainer.ScrollableHeight)
                {
                    scrollVerticalOffset = svContainer.ScrollableHeight;
                }

                svContainer.ScrollToVerticalOffset(scrollVerticalOffset);
            }

            if (scrollHorizontalOffset != 0.0)
            {
                scrollHorizontalOffset += svContainer.HorizontalOffset;

                if (scrollHorizontalOffset < 0.0)
                {
                    scrollHorizontalOffset = 0.0;
                }
                else if (scrollHorizontalOffset > svContainer.ScrollableHeight)
                {
                    scrollHorizontalOffset = svContainer.ScrollableHeight;
                }

                svContainer.ScrollToHorizontalOffset(scrollHorizontalOffset);
            }
        }
    }

    #endregion

    }
}
