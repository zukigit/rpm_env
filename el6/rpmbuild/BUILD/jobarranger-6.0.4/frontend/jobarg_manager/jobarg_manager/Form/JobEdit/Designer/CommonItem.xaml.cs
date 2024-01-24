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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/15 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
/// <summary>マウス移動委託</summary>
public delegate void MoveItemDelegate(CommonItem a, MouseEventArgs e);

/// <summary>
/// CommonItem.xaml の相互作用ロジッククラス
/// </summary>
public partial class CommonItem : UserControl, IRoom
{

    #region コンストラクタ
    public CommonItem(IContainer container, JobData data, Consts.EditType editType, RunJobMethodType methodType)
    {

        InitializeComponent();

        // コンテナのセット
        _container = container;

        // 内容アイコンの初期化
        InitContentItem(data, editType, methodType);

        // 内容のコンテナのセット
        _contentItem.Container = container;

        // 各アイコン設定テーブルを登録
        if (Consts.EditType.Add == editType)
            InsertIconData(data);

        this.Height = this.PicHeight;
        this.Width = this.PicWidth;
        this.ItemEditType = editType;
        ResetInitColor();

    }

    public CommonItem(IContainer container, JobData data, Consts.EditType editType, SolidColorBrush color)
    {

        InitializeComponent();

        // コンテナのセット
        _container = container;

        // 内容アイコンの初期化
        InitContentItem(data, editType, color);

        // 内容のコンテナのセット
        _contentItem.Container = container;

        // 各アイコン設定テーブルを登録
        if (Consts.EditType.Add == editType)
            InsertIconData(data);
        this.ItemEditType = editType;
        this.Height = this.PicHeight;
        this.Width = this.PicWidth;

    }

    //added by YAMA 2014/07/01
    public CommonItem(IContainer container, JobData data, Consts.EditType editType, SolidColorBrush color, SolidColorBrush characterColor)
    {

        InitializeComponent();

        // コンテナのセット
        _container = container;

        // 内容アイコンの初期化
        InitContentItem(data, editType, color, characterColor);

        // 内容のコンテナのセット
        _contentItem.Container = container;

        // 各アイコン設定テーブルを登録
        if (Consts.EditType.Add == editType)
            InsertIconData(data);
        this.ItemEditType = editType;
        this.Height = this.PicHeight;
        this.Width = this.PicWidth;

    }

    #endregion

    #region フィールド

    DBConnect dbAccess;

    /// <summary>マウス追従フラグ</summary>
    bool trackingMouseMove = false;

    /// <summary>マウスの位置</summary>
    Point mousePosition;

    /// <summary>マウス移動フラグ</summary>
    bool hadActualMove = false;

    /// <summary>アイコン移動の委任</summary>
    public event MoveItemDelegate ItemMove;


    #endregion

    #region プロパティ

    /// <summary>タイプ</summary>
    public ElementType ElementType{ get;set;}

    /// <summary>内容アイコン</summary>
    private IElement _contentItem;
    public IElement ContentItem
    {
        get
        {
            return _contentItem;
        }
        set
        {
            _contentItem = value;
        }
    }

    /// <summary>アイコンの幅</summary>
    double _picWidth;
    public double PicWidth
    {
        get
        {
            _picWidth = this.ContentItem.PicWidth;
            return _picWidth;
        }
    }

    /// <summary>アイコンの高さ</summary>
    double _picHeight;
    public double PicHeight
    {
        get
        {
            _picHeight = this.ContentItem.PicHeight;
            return _picHeight;
        }
    }

    /// <summary>コンテナ</summary>
    IContainer _container;
    public IContainer Container
    {
        get
        {
            return _container;
        }
        set
        {
            _container = value;
        }
    }

    /// <summary>当該コンポーネントの位置</summary>
    public Point Position
    {
        get
        {
            Point position;

            position = new Point();
            position.Y = (double)this.GetValue(Canvas.TopProperty);
            position.X = (double)this.GetValue(Canvas.LeftProperty);

            return position;
        }
        set
        {
            this.SetValue(Canvas.TopProperty, value.Y);
            this.SetValue(Canvas.LeftProperty, value.X);
            Move(this, null);
        }
    }

    /// <summary>中心点</summary>
    public Point CenterPoint
    {
        get
        {
            return new Point((double)this.GetValue(Canvas.LeftProperty) + PicWidth / 2, (double)this.GetValue(Canvas.TopProperty) + PicHeight / 2);
        }
        set
        {
            this.SetValue(Canvas.LeftProperty, value.X - _picWidth / 2);
            this.SetValue(Canvas.TopProperty, value.Y - _picHeight / 2);
            Move(this, null);
        }
    }

    /// <summary>選択フラグ</summary>
    bool isSelectd = false;
    public bool IsSelectd
    {
        get
        {
            return isSelectd;
        }
        set
        {
            isSelectd = value;

            if (isSelectd)
            {
                SetSelectedColor();

                SetFocus();

                if (!_container.CurrentSelectedControlCollection.Contains(this))
                    _container.AddSelectedControl(this);
            }
            else
            {
                ResetInitColor();
                SetUnFocus();
            }
        }

    }

    /// <summary>編集タイプ</summary>
    public Consts.EditType ItemEditType { get; set; }

    /// <summary>連接点：上</summary>
    public Point TopConnectPosition
    {
        get
        {
            Point p = ContentItem.TopConnectPosition;

            p.X += (double)this.GetValue(Canvas.LeftProperty);
            p.Y += (double)this.GetValue(Canvas.TopProperty);
            return p;
        }
    }

    /// <summary>連接点：下</summary>
    public Point BottomConnectPosition
    {
        get
        {
            Point p = ContentItem.BottomConnectPosition;

            p.X += (double)this.GetValue(Canvas.LeftProperty);
            p.Y += (double)this.GetValue(Canvas.TopProperty);

            return p;
        }
    }

    /// <summary>連接点：左</summary>
    public Point LeftConnectPosition
    {
        get
        {
            Point p = ContentItem.LeftConnectPosition;

            p.X += (double)this.GetValue(Canvas.LeftProperty);
            p.Y += (double)this.GetValue(Canvas.TopProperty);

            return p;
        }
    }

    /// <summary>連接点：右</summary>
    public Point RightConnectPosition
    {
        get
        {
            Point p = ContentItem.RightConnectPosition;

            p.X += (double)this.GetValue(Canvas.LeftProperty);
            p.Y += (double)this.GetValue(Canvas.TopProperty);

            return p;
        }
    }

    public void RemoveAllEvent()
    {
        this.MouseMove -= new MouseEventHandler(UserControl_MouseMove);
        this.MouseLeftButtonUp -= new MouseButtonEventHandler(UserControl_MouseLeftButtonUp);
        this.MouseLeftButtonDown -= new MouseButtonEventHandler(UserControl_MouseLeftButtonDown);
        this.MouseDoubleClick -= new MouseButtonEventHandler(UserControl_MouseDoubleClick);
        this.MouseRightButtonDown -= new MouseButtonEventHandler(UserControl_MouseRightButtonDown);

    }

    public void RemoveEvent4Read()
    {
        this.MouseMove -= new MouseEventHandler(UserControl_MouseMove);
        this.MouseLeftButtonUp -= new MouseButtonEventHandler(UserControl_MouseLeftButtonUp);
        this.MouseLeftButtonDown -= new MouseButtonEventHandler(UserControl_MouseLeftButtonDown);
        this.MouseRightButtonDown -= new MouseButtonEventHandler(UserControl_MouseRightButtonDown);
    }

    /// <summary>ジョブID</summary>
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    public string JobId
    {
        get
        {
            if (_contentItem != null)
                return _contentItem.JobId;
            else
                return "";
        }
        set
        {
            if (_contentItem != null)
                _contentItem.JobId = value;
            // ジョブIDを変更した場合、アイコンの表示内容も変更される
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("JobId"));
            }
        }
    }

    public string InnerJobId
    {
        get
        {
            if (_contentItem != null)
                return _contentItem.InnerJobId;
            else
                return "";
        }
        set
        {
            if (_contentItem != null)
                _contentItem.InnerJobId = value;
        }
    }

    /// <summary>ジョブ名</summary>
    public string JobName
    {
        get
        {
            if (_contentItem != null)
                return _contentItem.JobName;
            else
                return "";
        }
        set
        {
            if (_contentItem != null)
                _contentItem.JobName = value;
            // ジョブ名を変更した場合、アイコンの表示内容も変更される
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("JobName"));
            }
        }
    }

    public List<IFlow> BeginFlowList = new List<IFlow>();
    public List<IFlow> EndFlowList = new List<IFlow>();

    #endregion

    #region イベント

    /// <summary>マウスを左クリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // 左クリック
        if (e.ClickCount == 1)
        {
            e.Handled = true;
            hadActualMove = false;
            /*

            if (isSelectd == false && _container.ShiftKeyIsPress)
            {
                _container.IsSelectedByShiftKey = true;
            }
            if (!_container.ShiftKeyIsPress)
                IsSelectd = true;
            if(_container.ShiftKeyIsPress)
                IsSelectd = !IsSelectd;
                _container.SetWorkFlowElementSelected(this, IsSelectd);
            */
            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(Window.GetWindow(this));

            if (null != element)
            {
                element.CaptureMouse();
                element.Cursor = Cursors.Hand;
                if (isSelectd == true)
                    trackingMouseMove = true;
            }
            _container.CanvasClickFlg = false;

        }
        ((UserControl)_container).Focusable = true;
        Keyboard.Focus((UserControl)_container);

    }

    /// <summary>マウスを左釈放</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (hadActualMove)
            {
                //処理前現在データで履歴を作成
                ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Container)_container).CreateHistData();

                Point p = e.GetPosition(_container.ContainerCanvas);

                double x = (double)this.GetValue(Canvas.LeftProperty);
                double y = (double)this.GetValue(Canvas.TopProperty);

                _container.SetItemPosition(this.JobId, x, y);

                // フローのTrue、Falseの位置をセット
                foreach (IFlow flow in BeginFlowList)
                    flow.setRuleNameControlPosition();
                foreach (IFlow flow in EndFlowList)
                    flow.setRuleNameControlPosition();

                // 別の選択したアイコンのフローのTrue、Falseの位置をセット
                _container.SetControlCollectionItemAndRuleNameControlPosition(this);
            }
            else
            {

                if (isSelectd == false && _container.ShiftKeyIsPress)
                {
                    _container.IsSelectedByShiftKey = true;
                }
                if (!_container.ShiftKeyIsPress)
                    IsSelectd = true;
                if (_container.ShiftKeyIsPress)
                    IsSelectd = !IsSelectd;
                _container.SetWorkFlowElementSelected(this, IsSelectd);

            }
            //FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();

            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;
            _container.CanvasClickFlg = true;
        }
        ((UserControl)_container).Focusable = true;
        Keyboard.Focus((UserControl)_container);

    }

    /// <summary>マウスを左クリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        //e.Handled = true;
        hadActualMove = false;

        if (isSelectd == false)
        {
            IsSelectd = !IsSelectd;
            _container.SetWorkFlowElementSelected(this, IsSelectd);
        }
        ((UserControl)_container).Focusable = true;
        Keyboard.Focus((UserControl)_container);
    }

    /// <summary>部品をダブルクリック</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // 設定画面を表示する
        ShowIconSetting(false);
    }

    /// <summary>部品の移動処理</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (trackingMouseMove)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Cursor = Cursors.Hand;

            if (e.GetPosition(null) == mousePosition)
                return;
            hadActualMove = true;

            double deltaV = (e.GetPosition(null).Y  - mousePosition.Y) /_container.ZoomValue;
            double deltaH = (e.GetPosition(null).X - mousePosition.X) /_container.ZoomValue;

            deltaH = this.ResizeMoveDalta(deltaH, deltaV)[0];
            deltaV = this.ResizeMoveDalta(deltaH, deltaV)[1];

            double newTop = deltaV + Position.Y;
            double newLeft = deltaH + Position.X;

            if(deltaH>0 || deltaH<0 || deltaV>0 || deltaV<0)
            {
                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);

                Move(this, e);
                mousePosition = e.GetPosition(null);

                // 別の選択したアイコンを移動
                _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
            }
        }
    }

    #endregion

    #region publicメッソド

    /// <summary>移動処理</summary>
    /// <param name="sender">源</param>
    /// <param name="e">マウスイベント</param>
    public void Move(CommonItem a, MouseEventArgs e)
    {
        if (ItemMove != null)
            ItemMove(a, e);
    }

    /// <summary>選択色をセット</summary>
    public void SetSelectedColor()
    {
        this.ContentItem.SetSelectedColor();
    }

    /// <summary>選択色をリセット</summary>
    public void ResetInitColor()
    {
            this.ContentItem.ResetInitColor();
    }

    /// <summary>部品欄の色をセット</summary>
    public void InitSampleColor()
    {
        this.ContentItem.InitSampleColor();
    }

    /// <summary>部品欄の色をセット</summary>
    public void SetStatusColor(SolidColorBrush color)
    {
        this.ContentItem.SetStatusColor(color);
    }

    //added by YAMA 2014/07/01
    /// <summary>部品欄の文字色をセット</summary>
    public void SetStatusCharacterColor(SolidColorBrush color)
    {
        this.ContentItem.SetStatusCharacterColor(color);
    }

    /// <summary>部品の位置をセット</summary>
    /// <param name="x">x座標</param>
    /// <param name="y">y座標</param>
    public void SetPositionByDisplacement(double x, double y)
    {
        Point p = new Point();
        p.X = (double)this.GetValue(Canvas.LeftProperty);
        p.Y = (double)this.GetValue(Canvas.TopProperty);

        this.SetValue(Canvas.TopProperty, p.Y + y);
        this.SetValue(Canvas.LeftProperty, p.X + x);
        Move(this, null);
    }

    /// <summary>アイコンを削除</summary>
    public void Delete()
    {
        // 線からこのコンポーネントを削除(線を削除しない)
        if (this.BeginFlowList != null)
        {
            foreach (IFlow a in this.BeginFlowList)
            {
                _container.RemoveFlow(a);
            }
        }
        if (this.EndFlowList != null)
        {
            foreach (IFlow a in this.EndFlowList)
            {
                _container.RemoveFlow(a);
            }
        }

        // コンテナから当該部品を削除
        _container.RemoveItem(this);
    }

    /// <summary>最近いタイプを取得（上、下、左、右）</summary>
    /// <param name="point">位置</param>
    public ConnectType GetNearHotspot(Point point)
    {
        PointCollection points = new PointCollection();
        points.Add(this.GetHotspot(ConnectType.LEFT));
        points.Add(this.GetHotspot(ConnectType.TOP));
        points.Add(this.GetHotspot(ConnectType.RIGHT));
        points.Add(this.GetHotspot(ConnectType.BOTTOM));

        ConnectType hotspotType = ConnectType.LEFT;
        int idx = 0;
        double minValue = Math.Abs(point.X - points[0].X) + Math.Abs(point.Y - points[0].Y);
        for (int i = 1; i < points.Count; i++)
        {
            if (minValue > (Math.Abs(point.X - points[i].X) + Math.Abs(point.Y - points[i].Y)))
            {
                idx = i;
                minValue = Math.Abs(point.X - points[i].X) + Math.Abs(point.Y - points[i].Y);
            }
        }
        switch (idx)
        {
            case 0:
                hotspotType = ConnectType.LEFT;
                break;
            case 1:
                hotspotType = ConnectType.TOP;
                break;
            case 2:
                hotspotType = ConnectType.RIGHT;
                break;
            case 3:
                hotspotType = ConnectType.BOTTOM;
                break;
        }
        return hotspotType;
    }

    /// <summary>連接点を取得</summary>
    /// <param name="hotspotType">連接点のタイプ</param>
    public Point GetHotspot(ConnectType hotspotType)
    {
        Point point = new Point(0, 0);
        switch (hotspotType)
        {
            case ConnectType.LEFT:
                point = LeftConnectPosition;
                break;
            case ConnectType.TOP:
                point = TopConnectPosition;
                break;
            case ConnectType.RIGHT:
                point = RightConnectPosition;
                break;
            case ConnectType.BOTTOM:
                point = BottomConnectPosition;
                break;
        }
        return point;
    }

    /// <summary>部品をフォーカス</summary>
    public void SetFocus()
    {
        ContentItem.SetFocus();
    }

    /// <summary>部品のフォーカスをはずす</summary>
    public void SetUnFocus()
    {
        ContentItem.SetUnFocus();
    }

    /// <summary>部品を選択</summary>
    public void SetSelected()
    {
        ContentItem.SetSelected();
    }

    /// <summary>アイコン設定画面を表示</summary>
    public void ShowIconSetting(bool isSetting)
    {
        bool viewer = false;
        #if VIEWER
            viewer = true;
        #endif
        string jobId = this.JobId;
        switch (ElementType)
        {
            // 0:開始、5:ジョブネット、6:並行処理開始、7：並行処理終了、8：ループの場合
            case ElementType.START:
            //case ElementType.JOBNET:
            case ElementType.LOOP:
            case ElementType.MTS:
            case ElementType.MTE:
            //added by kim 2012/11/14
            case ElementType.IFE:
                OtherSetting otherSetting = new OtherSetting(this, jobId, ElementType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    otherSetting.SetDisable();
                otherSetting.ShowDialog();
                break;
            // 1:終了の場合
            case ElementType.END:
                EndSetting endSetting = new EndSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    endSetting.SetDisable();
                endSetting.ShowDialog();
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                IfSetting ifSetting = new IfSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    ifSetting.SetDisable();
                ifSetting.ShowDialog();
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                EnvSetting envSetting = new EnvSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    envSetting.SetDisable();
                envSetting.ShowDialog();
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                JobSetting jobSetting = new JobSetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    jobSetting.SetDisable();
                jobSetting.ShowDialog();
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                ExtJobSetting extJobSetting = new ExtJobSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    extJobSetting.SetDisable();
                extJobSetting.ShowDialog();
                break;
            //  10：計算の場合
            case ElementType.CAL:
                CalSetting calSetting = new CalSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    calSetting.SetDisable();
                calSetting.ShowDialog();
                break;
            // 11：タスク場合
            case ElementType.TASK:
                TaskSetting taskSetting = new TaskSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    taskSetting.SetDisable();
                taskSetting.ShowDialog();
                break;
            // 12：情報取得場合
            case ElementType.INF:
                InfSetting infSetting = new InfSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    infSetting.SetDisable();
                infSetting.ShowDialog();
                break;
            // 13：ジョブネットの場合
            case ElementType.JOBNET:
                if (isSetting)
                {
                    JobnetSetting jobnetSetting = new JobnetSetting(this, jobId);
                    if (ItemEditType == Consts.EditType.READ || viewer)
                        jobnetSetting.SetDisable();
                    jobnetSetting.ShowDialog();
                }
                else
                {
                    SubJobEdit subJobEdit;
                    DataRow[] rows = _container.IconJobnetTable.Select("job_id='" + jobId + "'");
                    String subJobnetId = rows[0]["link_jobnet_id"].ToString();

                    GetDBConnect();
                    dbAccess.CreateSqlConnect();
                    JobnetControlDAO jobnetDao = new JobnetControlDAO(dbAccess);
                    DataTable dt = jobnetDao.GetValidORMaxUpdateDateEntityById(subJobnetId);
                    dbAccess.CloseSqlConnect();
                    String updDate = dt.Rows[0]["update_date"].ToString();
                    String userName = (String)dt.Rows[0]["user_name"];
                    List<Decimal> objectUserGroupList = DBUtil.GetGroupIDListByAlias(userName);
                    Consts.ObjectOwnType objectOwnType = Consts.ObjectOwnType.Other;
                    if (CheckUtil.isExistGroupId(LoginSetting.GroupList, objectUserGroupList))
                    {
                        objectOwnType = Consts.ObjectOwnType.Owner;
                    }
                    int validFlag = (Int32)dt.Rows[0]["valid_flag"];

                    if (ItemEditType == Consts.EditType.READ || viewer || validFlag == 1 || (objectOwnType == Consts.ObjectOwnType.Other && !(LoginSetting.Authority == Consts.AuthorityEnum.SUPER)))
                    {
                        subJobEdit = new SubJobEdit(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)(_container.ParantWindow)).ParantWindow, subJobnetId, updDate, Consts.EditType.READ, true);
                    }
                    else
                    {
                        subJobEdit = new SubJobEdit(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)(_container.ParantWindow)).ParantWindow, subJobnetId, updDate, Consts.EditType.Modify, true);
                    }

                    if (subJobEdit.SuccessFlg)
                    {
                        //subJobEdit.Focusable = true;
                        //Keyboard.Focus(subJobEdit);
                        //subJobEdit.Topmost = true;
                        //subJobEdit.Owner = ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)(_container.ParantWindow)).ParantWindow;
                        subJobEdit.Show();
                        return;
                    }
                }
                break;

            // 14：ファイル転送場合
            case ElementType.FCOPY:
                FCopySetting fcopy = new FCopySetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    fcopy.SetDisable();
                fcopy.ShowDialog();
                break;

            // 15：ファイル待ち合わせ場合
            case ElementType.FWAIT:
                FWaitSetting fwait = new FWaitSetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    fwait.SetDisable();
                fwait.ShowDialog();
                break;
            // 16：リブートの場合
            case ElementType.REBOOT:
                RebootSetting reboot = new RebootSetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    reboot.SetDisable();
                reboot.ShowDialog();
                break;
            // 17：保留解除の場合
            case ElementType.RELEASE:
                ReleaseSetting release = new ReleaseSetting(this, jobId);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    release.SetDisable();
                release.ShowDialog();
                break;
            //added by YAMA 2014/02/04
            // 18：Zabbix連携の場合
            case ElementType.COOPERATION:
                CooperationSetting cooperation = new CooperationSetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    cooperation.SetDisable();
                cooperation.ShowDialog();
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレスの場合
            case ElementType.AGENTLESS:
                AgentlessSetting agentless = new AgentlessSetting(this, jobId, ItemEditType);
                if (ItemEditType == Consts.EditType.READ || viewer)
                    agentless.SetDisable();
                agentless.ShowDialog();
                break;
            default:
                break;
        }
        // ToolTip更新
        _contentItem.SetToolTip();
    }


    #region 関連の線の処理

    /// <summary>開始の線を追加</summary>
    /// <param name="flow">連接線</param>
    public void AddBeginFlow(IFlow flow)
    {
        if (!BeginFlowList.Contains(flow))
        {
            BeginFlowList.Add(flow);
            //flow.BeginActivity = this;
            Move(this, null);
        }

    }

    /// <summary>開始の線を削除</summary>
    /// <param name="flow">連接線</param>
    public void RemoveBeginFlow(IFlow flow)
    {
        if (BeginFlowList.Contains(flow))
        {
            BeginFlowList.Remove(flow);
            //flow.RemoveBeginActivity(this);

        }
    }

    /// <summary>結束の線を追加</summary>
    /// <param name="flow">連接線</param>
    public void AddEndFlow(IFlow flow)
    {
        if (!EndFlowList.Contains(flow))
        {
            EndFlowList.Add(flow);
            //r.EndActivity = this;
            Move(this, null);

        }

    }

    /// <summary>結束の線を削除</summary>
    /// <param name="flow">連接線</param>
    public void RemoveEndFlow(IFlow flow)
    {
        if (EndFlowList.Contains(flow))
        {
            EndFlowList.Remove(flow);
        }
    }

    /// <summary>点のアイテムの中の判定</summary>
    /// <param name="point">点</param>
    public bool PointIsInside(Point point)
    {
        bool isInside = false;

        if (Position.X < point.X && point.X < Position.X + PicWidth
            && Position.Y < point.Y && point.Y < Position.Y + PicHeight)
        {
            isInside = true;
        }
        return isInside;
    }

    #endregion

    #endregion

    #region private メッソド

    //************************************************************************
    /// <summary>アイコンの初期化処理</summary>
    /// <param name="data">データ</param>
    /// <param name="editType">編集タイプ</param>
    //************************************************************************
    private void GetDBConnect()
    {
        if (dbAccess == null)
        {
            dbAccess = new DBConnect(LoginSetting.ConnectStr);
        }
    }

    //************************************************************************
    /// <summary>アイコンの初期化処理</summary>
    /// <param name="data">データ</param>
    /// <param name="editType">編集タイプ</param>
    //************************************************************************
    private bool InitContentItem(JobData data, Consts.EditType editType)
    {
        if (data == null)
            return false;

        ElementType type = data.JobType;
        this.ElementType = type;

        // アイテムのインスタンス
        IElement item = null;

        switch (type)
        {
            // 0:開始の場合
            case ElementType.START:
                item = new Start();
                break;
            // 1:終了の場合
            case ElementType.END:
                item = new End();
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                item = new If();
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                item = new Env();
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                item = new Job();
                break;
            // 5:ジョブネットの場合
            case ElementType.JOBNET:
                item = new JobNet();
                break;
            // 6:並行処理開始の場合
            case ElementType.MTS:
                item = new Mts();
                break;
            // 7：並行処理終了の場合
            case ElementType.MTE:
                item = new Mte();
                break;
            // 8：ループの場合
            case ElementType.LOOP:
                item = new Loop();
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                item = new ExtJob();
                break;
            //  10：計算の場合
            case ElementType.CAL:
                item = new Cal();
                break;
            // 11：タスク場合
            case ElementType.TASK:
                item = new Task();
                break;
            // 12：情報取得場合
            case ElementType.INF:
                item = new Inf();
                break;
            //added by kim 2012/11/14
            // 13：条件分岐場合
            case ElementType.IFE:
                item = new Ife();
                break;
            // 14：ファイル転送場合
            case ElementType.FCOPY:
                item = new FCopy();
                break;
            // 15：ファイル待ち合わせ場合
            case ElementType.FWAIT:
                item = new FWait();
                break;
            // 16：リブートの場合
            case ElementType.REBOOT:
                item = new Reboot();
                break;
            // 17：保留解除の場合
            case ElementType.RELEASE:
                item = new Release();
                break;
            //added by YAMA 2014/02/04
            // 18：Zabbix連携
            case ElementType.COOPERATION:
                item = new Cooperation();
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレス
            case ElementType.AGENTLESS:
                item = new Agentless();
                break;
        }

        string jobId = "";
        if (Consts.EditType.Add == editType)
        {
            // ジョブデータをセット
            DataRow row = _container.JobControlTable.NewRow();
            _container.JobControlTable.Rows.Add(row);

            jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
            if (ElementType.JOBNET == type)
                jobId = data.Data.ToString();

            // 既存の場合、繰り返して取得
            int count = 0;
            while(ElementType.START != type
                && _container.JobItems.ContainsKey(jobId))
            {
                count++;
                jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
                if (ElementType.JOBNET == type)
                    jobId = data.Data.ToString() + "-" + count;
            }
            // ジョブネットID
            row["jobnet_id"] = _container.JobnetId;
            // ジョブID
            row["job_id"] = jobId;
            // 更新日
            row["update_date"] = _container.TmpUpdDate;
            // ジョブタイプ
            row["job_type"] = type;
            // ジョブネットの場合
            if (ElementType.JOBNET == type)
                row["job_name"] = data.Data.ToString();
        }

        // コンクリート
        commonRoom.Children.Add((UIElement)item);

        // 変数の初期化
        this.ContentItem = (IElement)item;
        this.JobId = jobId;
        // ジョブネットの場合、ジョブ名にリンク先ジョブネットIDをセット
        if (ElementType.JOBNET == type && Consts.EditType.Add == editType)
            this.JobName = data.Data.ToString();
        else
            this.JobName = "";

        return true;
    }

    //************************************************************************
    /// <summary>アイコンの初期化処理</summary>
    /// <param name="data">データ</param>
    /// <param name="editType">編集タイプ</param>
    /// <param name="methodType">処理フラグ</param>
    //************************************************************************
    private bool InitContentItem(JobData data, Consts.EditType editType, RunJobMethodType methodType)
    {
        if (data == null)
            return false;

        ElementType type = data.JobType;
        this.ElementType = type;

        // アイテムのインスタンス
        IElement item = null;

        switch (type)
        {
            // 0:開始の場合
            case ElementType.START:
                item = new Start(methodType);
                break;
            // 1:終了の場合
            case ElementType.END:
                item = new End(methodType);
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                item = new If(methodType);
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                item = new Env(methodType);
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                item = new Job(methodType);
                break;
            // 5:ジョブネットの場合
            case ElementType.JOBNET:
                item = new JobNet(methodType);
                break;
            // 6:並行処理開始の場合
            case ElementType.MTS:
                item = new Mts(methodType);
                break;
            // 7：並行処理終了の場合
            case ElementType.MTE:
                item = new Mte(methodType);
                break;
            // 8：ループの場合
            case ElementType.LOOP:
                item = new Loop(methodType);
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                item = new ExtJob(methodType);
                break;
            //  10：計算の場合
            case ElementType.CAL:
                item = new Cal(methodType);
                break;
            // 11：タスク場合
            case ElementType.TASK:
                item = new Task(methodType);
                break;
            // 12：情報取得場合
            case ElementType.INF:
                item = new Inf(methodType);
                break;
            // 13：分岐終了
            case ElementType.IFE:
                item = new Ife(methodType);
                break;
            // 14：ファイル転送
            case ElementType.FCOPY:
                item = new FCopy(methodType);
                break;
            // 15：ファイル待ち合わせ
            case ElementType.FWAIT:
                item = new FWait(methodType);
                break;
            // 16：リブート
            case ElementType.REBOOT:
                item = new Reboot(methodType);
                break;
            // 17：保留解除
            case ElementType.RELEASE:
                item = new Release(methodType);
                break;
            //added by YAMA 2014/02/04
            // 18：Zabbix連携
            case ElementType.COOPERATION:
                item = new Cooperation(methodType);
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレス
            case ElementType.AGENTLESS:
                item = new Agentless(methodType);
                break;
        }

        string jobId = "";
        if (Consts.EditType.Add == editType)
        {
            // ジョブデータをセット
            DataRow row = _container.JobControlTable.NewRow();
            _container.JobControlTable.Rows.Add(row);

            jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
            if (ElementType.JOBNET == type)
                jobId = data.Data.ToString();

            // 既存の場合、繰り返して取得
            int count = 0;
            while (ElementType.START != type
                && _container.JobItems.ContainsKey(jobId))
            {
                count++;
                jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
                if (ElementType.JOBNET == type)
                    jobId = data.Data.ToString() + "-" + count;
            }
            // ジョブネットID
            row["jobnet_id"] = _container.JobnetId;
            // ジョブID
            row["job_id"] = jobId;
            // 更新日
            row["update_date"] = _container.TmpUpdDate;
            // ジョブタイプ
            row["job_type"] = type;
            // 処理タイプ
            row["method_flag"] = (int)RunJobMethodType.NORMAL;
            // ジョブネットの場合
            if (ElementType.JOBNET == type)
                row["job_name"] = data.Data.ToString();
        }

        // コンクリート
        commonRoom.Children.Add((UIElement)item);

        // 変数の初期化
        this.ContentItem = (IElement)item;
        this.JobId = jobId;
        // ジョブネットの場合、ジョブ名にリンク先ジョブネットIDをセット
        if (ElementType.JOBNET == type && Consts.EditType.Add == editType)
        {
            this.JobName = data.Data.ToString();
            jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobNet jobnet =
                (jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobNet)item;
            jobnet.SetToolTip(jobId, data.Data.ToString());
        }
        else
            this.JobName = "";

        return true;
    }

    //************************************************************************
    /// <summary>アイコンの初期化処理</summary>
    /// <param name="data">データ</param>
    /// <param name="editType">編集タイプ</param>
    /// <param name="editType">アイコンカラー</param>
    //************************************************************************
    private bool InitContentItem(JobData data, Consts.EditType editType, SolidColorBrush color)
    {
        if (data == null)
            return false;

        ElementType type = data.JobType;
        this.ElementType = type;

        // アイテムのインスタンス
        IElement item = null;

        switch (type)
        {
            // 0:開始の場合
            case ElementType.START:
                item = new Start(color);
                break;
            // 1:終了の場合
            case ElementType.END:
                item = new End(color);
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                item = new If(color);
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                item = new Env(color);
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                item = new Job(color);
                break;
            // 5:ジョブネットの場合
            case ElementType.JOBNET:
                item = new JobNet(color);
                break;
            // 6:並行処理開始の場合
            case ElementType.MTS:
                item = new Mts(color);
                break;
            // 7：並行処理終了の場合
            case ElementType.MTE:
                item = new Mte(color);
                break;
            // 8：ループの場合
            case ElementType.LOOP:
                item = new Loop(color);
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                item = new ExtJob(color);
                break;
            //  10：計算の場合
            case ElementType.CAL:
                item = new Cal(color);
                break;
            // 11：タスク場合
            case ElementType.TASK:
                item = new Task(color);
                break;
            // 12：情報取得場合
            case ElementType.INF:
                item = new Inf(color);
                break;
            // 13：分岐終了
            case ElementType.IFE:
                item = new Ife(color);
                break;
            // 14：ファイル転送
            case ElementType.FCOPY:
                item = new FCopy(color);
                break;
            // 15：ファイル待ち合わせ
            case ElementType.FWAIT:
                item = new FWait(color);
                break;
            // 16：リブート
            case ElementType.REBOOT:
                item = new Reboot(color);
                break;
            // 17：保留解除
            case ElementType.RELEASE:
                item = new Release(color);
                break;
            //added by YAMA 2014/02/04
            // 18：Zabbix連携
            case ElementType.COOPERATION:
                item = new Cooperation(color);
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレス
            case ElementType.AGENTLESS:
                item = new Agentless(color);
                break;
        }

        string jobId = "";
        if (Consts.EditType.Add == editType)
        {
            // ジョブデータをセット
            DataRow row = _container.JobControlTable.NewRow();
            _container.JobControlTable.Rows.Add(row);

            jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
            if (ElementType.JOBNET == type)
                jobId = data.Data.ToString();

            // 既存の場合、繰り返して取得
            int count = 0;
            while (ElementType.START != type
                && _container.JobItems.ContainsKey(jobId))
            {
                count++;
                jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
                if (ElementType.JOBNET == type)
                    jobId = data.Data.ToString() + "-" + count;
            }
            // ジョブネットID
            row["jobnet_id"] = _container.JobnetId;
            // ジョブID
            row["job_id"] = jobId;
            // 更新日
            row["update_date"] = _container.TmpUpdDate;
            // ジョブタイプ
            row["job_type"] = type;
            // ジョブネットの場合
            if (ElementType.JOBNET == type)
                row["job_name"] = data.Data.ToString();
        }

        // コンクリート
        commonRoom.Children.Add((UIElement)item);

        // 変数の初期化
        this.ContentItem = (IElement)item;
        this.JobId = jobId;
        // ジョブネットの場合、ジョブ名にリンク先ジョブネットIDをセット
        if (ElementType.JOBNET == type && Consts.EditType.Add == editType)
            this.JobName = data.Data.ToString();
        else
            this.JobName = "";

        return true;
    }

    //added by YAMA 2014/07/01
    //************************************************************************
    /// <summary>アイコンの初期化処理</summary>
    /// <param name="data">データ</param>
    /// <param name="editType">編集タイプ</param>
    /// <param name="color">アイコンカラー</param>
    /// <param name="characterColor">文字色</param>
    //************************************************************************
    private bool InitContentItem(JobData data, Consts.EditType editType, SolidColorBrush color, SolidColorBrush characterColor)
    {
        if (data == null)
            return false;

        ElementType type = data.JobType;
        this.ElementType = type;

        // アイテムのインスタンス
        IElement item = null;

        switch (type)
        {
            // 0:開始の場合
            case ElementType.START:
                item = new Start(color);
                break;
            // 1:終了の場合
            case ElementType.END:
                item = new End(color);
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                item = new If(color);
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                item = new Env(color, characterColor);
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                item = new Job(color, characterColor);
                break;
            // 5:ジョブネットの場合
            case ElementType.JOBNET:
                item = new JobNet(color, characterColor);
                break;
            // 6:並行処理開始の場合
            case ElementType.MTS:
                item = new Mts(color);
                break;
            // 7：並行処理終了の場合
            case ElementType.MTE:
                item = new Mte(color);
                break;
            // 8：ループの場合
            case ElementType.LOOP:
                item = new Loop(color);
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                item = new ExtJob(color, characterColor);
                break;
            //  10：計算の場合
            case ElementType.CAL:
                item = new Cal(color, characterColor);
                break;
            // 11：タスク場合
            case ElementType.TASK:
                item = new Task(color, characterColor);
                break;
            // 12：情報取得場合
            case ElementType.INF:
                item = new Inf(color, characterColor);
                break;
            // 13：分岐終了
            case ElementType.IFE:
                item = new Ife(color);
                break;
            // 14：ファイル転送
            case ElementType.FCOPY:
                item = new FCopy(color, characterColor);
                break;
            // 15：ファイル待ち合わせ
            case ElementType.FWAIT:
                item = new FWait(color, characterColor);
                break;
            // 16：リブート
            case ElementType.REBOOT:
                item = new Reboot(color, characterColor);
                break;
            // 17：保留解除
            case ElementType.RELEASE:
                item = new Release(color, characterColor);
                break;
            // 18：Zabbix連携
            case ElementType.COOPERATION:
                item = new Cooperation(color, characterColor);
                break;
            // 19：エージェントレス
            case ElementType.AGENTLESS:
                item = new Agentless(color, characterColor);
                break;
        }

        string jobId = "";
        if (Consts.EditType.Add == editType)
        {
            // ジョブデータをセット
            DataRow row = _container.JobControlTable.NewRow();
            _container.JobControlTable.Rows.Add(row);

            jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
            if (ElementType.JOBNET == type)
                jobId = data.Data.ToString();

            // 既存の場合、繰り返して取得
            int count = 0;
            while (ElementType.START != type
                && _container.JobItems.ContainsKey(jobId))
            {
                count++;
                jobId = CommonUtil.GetJobId(((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.JobEdit)_container.ParantWindow).JobNoHash, type);
                if (ElementType.JOBNET == type)
                    jobId = data.Data.ToString() + "-" + count;
            }
            // ジョブネットID
            row["jobnet_id"] = _container.JobnetId;
            // ジョブID
            row["job_id"] = jobId;
            // 更新日
            row["update_date"] = _container.TmpUpdDate;
            // ジョブタイプ
            row["job_type"] = type;
            // ジョブネットの場合
            if (ElementType.JOBNET == type)
                row["job_name"] = data.Data.ToString();
        }

        // コンクリート
        commonRoom.Children.Add((UIElement)item);

        // 変数の初期化
        this.ContentItem = (IElement)item;
        this.JobId = jobId;
        // ジョブネットの場合、ジョブ名にリンク先ジョブネットIDをセット
        if (ElementType.JOBNET == type && Consts.EditType.Add == editType)
            this.JobName = data.Data.ToString();
        else
            this.JobName = "";

        return true;
    }

    //************************************************************************
    /// <summary>各アイコン設定テーブルの登録処理</summary>
    /// <param name="data">データ</param>
    //************************************************************************
    private bool InsertIconData(JobData data)
    {
        if (data == null)
            return false;

        ElementType type = data.JobType;
        this.ElementType = type;

        // 登録
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
                InsertIconEndTbl();
                break;
            // 2:条件分岐の場合
            case ElementType.IF:
                InsertIconIfTbl();
                break;
            // 3:ジョブコントローラ変数の場合
            case ElementType.ENV:
                //InsertIconValueTbl();
                break;
            // 4:ジョブの場合
            case ElementType.JOB:
                InsertIconJobTbl();
                break;
            // 5:ジョブネット
            case ElementType.JOBNET:
                InsertIconJobnetTbl(data.Data.ToString());
                break;
            // 9：拡張ジョブの場合
            case ElementType.EXTJOB:
                InsertIconExtjobTbl();
                break;
            //  10：計算の場合
            case ElementType.CAL:
                InsertIconCalcTbl();
                break;
            // 11：タスク場合
            case ElementType.TASK:
                InsertIconTaskTbl();
                break;
            // 12：情報取得場合
            case ElementType.INF:
                InsertIconInfoTbl();
                break;
            // 14：ファイル転送場合
            case ElementType.FCOPY:
                InsertIconFcopyTbl();
                break;
            // 15：ファイル待ち合わせ場合
            case ElementType.FWAIT:
                InsertIconFwaitTbl();
                break;
            // 16：リブートの場合
            case ElementType.REBOOT:
                InsertIconRebootTbl();
                break;
            // 17：保留解除の場合
            case ElementType.RELEASE:
                InsertIconReleaseTbl();
                break;

            //added by YAMA 2014/02/06
            // 18：Zabbix連携の場合
            case ElementType.COOPERATION:
                InsertIconCooperationTbl();
                break;
            //added by YAMA 2014/05/19
            // 19：エージェントレスの場合
            case ElementType.AGENTLESS:
                InsertIconAgentlessTbl();
                break;
        }

        return true;
    }

    //************************************************************************
    /// <summary>計算アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconCalcTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconCalcTable.NewRow();
        _container.IconCalcTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // 計算方法(0：整数計算)
        row["hand_flag"] = 0;
        // 計算式
        row["formula"] = "";
        // ジョブコントローラ変数名
        row["value_name"] = "";
    }

    //************************************************************************
    /// <summary>終了アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconEndTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconEndTable.NewRow();
        _container.IconEndTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // ジョブネット停止フラグ(0：処理継続)
        row["jobnet_stop_flag"] = 0;
        // 終了コード
        row["jobnet_stop_code"] = 0;
    }

    //************************************************************************
    /// <summary>拡張ジョブアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconExtjobTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconExtjobTable.NewRow();
        _container.IconExtjobTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // コマンドID
        row["command_id"] = "";
        // パラメータ値
        row["value"] = DBNull.Value;
    }

    //************************************************************************
    /// <summary>条件分岐アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconIfTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconIfTable.NewRow();
        _container.IconIfTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // 処理方式(0：数値)
        row["hand_flag"] = 0;
        row["value_name"] = "";
        row["comparison_value"] = "";
    }

    //************************************************************************
    /// <summary>情報取得アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconInfoTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconInfoTable.NewRow();
        _container.IconInfoTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;

        // 情報種別(0：ジョブ状態)
        row["info_flag"] = 0;
        // アイテムID
        row["item_id"] = DBNull.Value;
        // トリガーID
        row["trigger_id"] = DBNull.Value;
        // ホストグループ名
        row["host_group"] = DBNull.Value;
        // ホスト名
        row["host_name"] = DBNull.Value;
        // 取得先ジョブID
        row["get_job_id"] = DBNull.Value;
    }

    //************************************************************************
    /// <summary>ジョブネットアイコン設定テーブルの登録処理</summary>
    /// <param name="linkJobnetId">リンク先ジョブネットID</param>
    //************************************************************************
    private void InsertIconJobnetTbl(string linkJobnetId)
    {
        string jobId = JobId;
        DataRow row = _container.IconJobnetTable.NewRow();
        _container.IconJobnetTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // リンク先ジョブネットID
        row["link_jobnet_id"] = linkJobnetId;
    }

    //************************************************************************
    /// <summary>ジョブアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconJobTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconJobTable.NewRow();
        _container.IconJobTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // ホストフラグ(0：ホスト名)
        row["host_flag"] = 0;
        // 停止コマンドフラグ
        row["stop_flag"] = 0;
        // コマンドタイプ
        row["command_type"] = 0;
        // タイムアウト警告
        row["timeout"] = 0;
        // ホスト名
        row["host_name"] = DBNull.Value;
        // ジョブ停止コード
        row["stop_code"] = DBNull.Value;
    }

    //************************************************************************
    /// <summary>タスクアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconTaskTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconTaskTable.NewRow();
        _container.IconTaskTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // 起動ジョブネットID
        row["submit_jobnet_id"] = "";
    }

    //************************************************************************
    /// <summary>ファイル転送アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconFcopyTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconFcopyTable.NewRow();
        _container.IconFcopyTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // 転送元ホストフラグ(0：ホスト名)
        row["from_host_flag"] = 0;
        // 転送先ホストフラグ(0：ホスト名)
        row["to_host_flag"] = 0;
        // 転送元ホスト名
        row["from_host_name"] = DBNull.Value;
        // 転送先ホスト名
        row["to_host_name"] = DBNull.Value;
        // 転送元ディレクトリ
        row["from_directory"] = DBNull.Value;
        // 転送元ファイル名
        row["from_file_name"] = DBNull.Value;
        // 転送先ディレクトリ
        row["to_directory"] = DBNull.Value;
    }

    //************************************************************************
    /// <summary>ファイル待ち合わせアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconFwaitTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconFwaitTable.NewRow();
        _container.IconFwaitTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // ホストフラグ(0：ホスト名)
        row["host_flag"] = 0;
        // ホスト名
        row["host_name"] = DBNull.Value;
        // ファイル名
        row["file_name"] = DBNull.Value;
        // 処理モード
        row["fwait_mode_flag"] = 0;
        // 削除モード
        row["file_delete_flag"] = 0;
        // 待合せ時間(初期値:0)
        row["file_wait_time"] = 0;

    }

    //************************************************************************
    /// <summary>リブートアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconRebootTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconRebootTable.NewRow();
        _container.IconRebootTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        // ホストフラグ(0：ホスト名)
        row["host_flag"] = 0;
        // ホスト名
        row["host_name"] = DBNull.Value;
        // リブートモードフラグ(0：強制)
        row["reboot_mode_flag"] = 0;
        // 待合せ時間(初期値:0)
        row["reboot_wait_time"] = 0;

        //added by YAMA 2014/09/22
        // タイムアウト警告(初期値:0)
        row["timeout"] = 0;
    }

    //************************************************************************
    /// <summary>保留解除アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconReleaseTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconReleaseTable.NewRow();
        _container.IconReleaseTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        row["release_job_id"] = DBNull.Value;


    }

    //added by YAMA 2014/02/06
    //************************************************************************
    /// <summary>Zabbix連携アイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconCooperationTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconCooperationTable.NewRow();
        _container.IconCooperationTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        row["link_target"] = 0;
        row["link_operation"] = 0;
        row["groupid"] = DBNull.Value;
        row["hostid"] = DBNull.Value;
        row["itemid"] = DBNull.Value;
        row["triggerid"] = DBNull.Value;

    }

    //added by YAMA 2014/05/19
    //************************************************************************
    /// <summary>エージェントレスアイコン設定テーブルの登録処理</summary>
    //************************************************************************
    private void InsertIconAgentlessTbl()
    {
        string jobId = JobId;
        DataRow row = _container.IconAgentlessTable.NewRow();
        _container.IconAgentlessTable.Rows.Add(row);

        row["jobnet_id"] = _container.JobnetId;
        row["job_id"] = JobId;
        row["update_date"] = _container.TmpUpdDate;
        row["host_flag"] = 0;
        row["connection_method"] = 0;
        row["session_flag"] = 0;
        row["auth_method"] = 0;
        row["run_mode"] = 0;
        row["line_feed_code"] = 0;
        row["timeout"] = 0;
        row["session_id"] = DBNull.Value;
        row["login_user"] = DBNull.Value;
        row["login_password"] = DBNull.Value;
        row["public_key"] = DBNull.Value;
        row["private_key"] = DBNull.Value;
        row["passphrase"] = DBNull.Value;
        row["host_name"] = DBNull.Value;
        row["stop_code"] = DBNull.Value;
        row["terminal_type"] = DBNull.Value;
        row["character_code"] = DBNull.Value;
        row["prompt_string"] = DBNull.Value;
        row["command"] = DBNull.Value;
    }


    private double[] ResizeMoveDalta(double deltaH, double deltaV)
    {
        CommonItem tmpItem = null;

        double[] ResizeDelta = {deltaH,deltaV};

        for (int i = 0; i < _container.CurrentSelectedControlCollection.Count; i++)
        {
            if (_container.CurrentSelectedControlCollection[i] is CommonItem)
            {
                tmpItem = _container.CurrentSelectedControlCollection[i] as CommonItem;
                double newTop = ResizeDelta[1] + tmpItem.Position.Y;
                double newLeft = ResizeDelta[0] + tmpItem.Position.X;

                // コンテナの範囲を超える
                if (!_container.MouseIsInContainer
                    //|| (_container.LeftXOfSelection - this.PicWidth / 2　< 1 && deltaH < 0)
                    //|| (_container.TopYOfSelection - this.PicHeight / 2  < 1 && deltaV < 0)
                    || (tmpItem.Position.X <= 0 && ResizeDelta[0] < 0)
                    || (tmpItem.Position.Y <= 0 && ResizeDelta[1] < 0)
                    || (tmpItem.Position.X + this.PicWidth >= _container.ContainerCanvas.Width && ResizeDelta[0] > 0)
                    || (tmpItem.Position.Y + this.PicHeight >= _container.ContainerCanvas.Height && ResizeDelta[1] > 0)
                    )
                {
                    return new double[]{0,0};
                }
                else
                {
                    // コンテナの範囲を越える
                    if (newTop < 0) ResizeDelta[1] = 0-tmpItem.Position.Y;
                    if (newLeft < 0) ResizeDelta[0] = 0-tmpItem.Position.X;
                }
            }
        }
        return ResizeDelta;
    }

    #endregion

}
}
