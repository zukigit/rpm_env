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
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/20 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// FlowArc.xaml の相互作用ロジック
    /// </summary>
    public partial class FlowArc : UserControl, IFlow
    {
        #region コンストラクタ
        public FlowArc()
        {
            InitializeComponent();
        }
        #endregion

        #region プロパティ
        /// <summary>開始ジョブ</summary>
        IRoom _beginItem;
        public IRoom BeginItem
        {
            get
            {
                return _beginItem;
            }
            set
            {
                _beginItem = value;
                if (_beginItem != null)
                {
                    _beginItem.AddBeginFlow(this);
                    _beginItem.ItemMove += new MoveItemDelegate(OnItemMove);
                }
            }
        }

        /// <summary>終了ジョブ</summary>
        IRoom _endItem;
        public IRoom EndItem
        {
            get { return _endItem; }
            set
            {
                _endItem = value;

                if (_endItem != null)
                {
                    _endItem.AddEndFlow(this);
                    _endItem.ItemMove += new MoveItemDelegate(OnItemMove);
                }

            }
        }

        // 起点
        public Point BeginPosition
        {
            get
            {
                return this.arrow.StartPoint;
            }
            set
            {
                this.arrow.StartPoint = value;
                // 一旦削除(連接点の変更用)
                //ellipseBegin.SetValue(Canvas.TopProperty, value.Y);
                //ellipseBegin.SetValue(Canvas.LeftProperty, value.X - ellipseBegin.Width / 2);
            }
        }

        // 終点
        public Point EndPosition
        {
            get
            {
                return this.arrow.EndPoint;
            }
            set
            {
                this.arrow.EndPoint = value;
                // 一旦削除(連接点の変更用)
                //ellipseEnd.SetValue(Canvas.TopProperty, value.Y - ellipseEnd.Height / 2);
                //ellipseEnd.SetValue(Canvas.LeftProperty, value.X - ellipseEnd.Width / 2);

            }
        }

        // コンテナ
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
        // フィールド：選択フラグ
        bool _isSelectd = false;
        public bool IsSelectd
        {
            get
            {
                return _isSelectd;
            }
            set
            {
                _isSelectd = value;

                if (_isSelectd)
                {
                    // 一旦削除(連接点の変更用)
                    //ellipseBegin.Visibility = Visibility.Visible;
                    //ellipseEnd.Visibility = Visibility.Visible;
                    arrow.Background = SystemConst.ColorConst.SelectedColor;
                }
                else
                {

                    //ellipseBegin.Visibility = Visibility.Collapsed;
                    //ellipseEnd.Visibility = Visibility.Collapsed;
                    arrow.Background = SystemConst.ColorConst.BlackColor;
                }
            }

        }

        /// <summary>開始アイコンの連接位置タイプ</summary>
        private ConnectType beginConType;
        public ConnectType BeginConType
        {
            get { return beginConType; }
            set
            {
                beginConType = value;
            }
        }

        /// <summary>終了アイコンの連接位置タイプ</summary>
        private ConnectType endConType;
        public ConnectType EndConType
        {
            get { return endConType; }
            set
            {
                endConType = value;
            }
        }

        #endregion

        #region public メッソド


        /// <summary>True設定 </summary>
        public void SetTrue(Consts.EditType editType)
        {
            tbFlowName.Text = "TRUE";
            tbFlowName.Visibility = Visibility.Visible;
            setRuleNameControlPosition();
            if (editType == Consts.EditType.Add)
                SetDbData(FlowType.TRUE);
        }

        /// <summary>False設定 </summary>
        public void SetFalse(Consts.EditType editType)
        {
            tbFlowName.Text = "FALSE";
            tbFlowName.Visibility = Visibility.Visible;
            setRuleNameControlPosition();
            if (editType == Consts.EditType.Add)
                SetDbData(FlowType.FALSE);
        }

        #endregion

        #region イベント


        /// <summary>ジョブを移動する時、フローの処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        void OnItemMove(CommonItem a, MouseEventArgs e)
        {
            if (a != EndItem && a != BeginItem )
                return;

            // 一旦削除(連接点の変更用)
            //if (this._MoveType == LineMoveType.Begin || this._MoveType == LineMoveType.End)
            //    return;

            List<Point> connPoints = CommonUtil.GetConnectPointsForCurve(_beginItem, _endItem);

            if (connPoints != null && connPoints.Count == 2)
            {
                this.BeginPosition = connPoints[0];
                this.EndPosition = connPoints[1];
            }

            // 一旦削除（連接点が固定）

            //if (BeginItem == a)
            //{
            //    if (beginConType == ConnectType.LEFT)
            //    {
            //        this.BeginPosition = a.LeftConnectPosition;
            //    }
            //    else if (beginConType == ConnectType.RIGHT)
            //    {
            //        this.BeginPosition = a.RightConnectPosition;
            //    }
            //    else if (beginConType == ConnectType.TOP)
            //    {
            //        this.BeginPosition = a.TopConnectPosition;
            //    }
            //    else if (beginConType == ConnectType.BOTTOM)
            //    {
            //        this.BeginPosition = a.BottomConnectPosition;

            //    }

            //}
            //else if (EndItem == a)
            //{
            //    if (endConType == ConnectType.LEFT)
            //    {
            //        this.EndPosition = a.LeftConnectPosition;

            //    }
            //    else if (endConType == ConnectType.RIGHT)
            //    {
            //        this.EndPosition = a.RightConnectPosition;
            //    }
            //    else if (endConType == ConnectType.TOP)
            //    {
            //        this.EndPosition = a.TopConnectPosition;
            //    }
            //    else if (endConType == ConnectType.BOTTOM)
            //    {
            //        this.EndPosition = a.BottomConnectPosition;
            //    }
            //}

            // 半径を調整
            arrow.Radius = Point.Subtract(BeginPosition, EndPosition).Length * 0.65;

        }

        // 一旦削除(連接点の変更用)
        ///// <summary>マウスが入る時処理</summary>
        ///// <param name="sender">源</param>
        ///// <param name="e">マウスイベント</param>
        //private void Hotspot_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    FrameworkElement element = e.OriginalSource as FrameworkElement;
        //    if (element != null)
        //    {
        //        if (LineMoveType.Begin == _MoveType)
        //            _beginItem.IsSelectd = true;
        //        else if (LineMoveType.End == _MoveType)
        //            _endItem.IsSelectd = true;
        //        element.Cursor = Cursors.Hand;
        //    }
        //}

        // 一旦削除(連接点の変更用)
        ///// <summary>マウスが外す時処理</summary>
        ///// <param name="sender">源</param>
        ///// <param name="e">マウスイベント</param>
        //private void Hotspot_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (trackingPointMouseMove != true)
        //    {
        //        FrameworkElement element = e.OriginalSource as FrameworkElement;
        //        if (element != null)
        //        {
        //            if (LineMoveType.Begin == _MoveType)
        //                _beginItem.IsSelectd = false;
        //            else if (LineMoveType.End == _MoveType)
        //                _endItem.IsSelectd = false;
        //            element.Cursor = Cursors.Arrow;
        //        }
        //    }
        //}

        //bool trackingPointMouseMove = false;
        //bool linkElement = false;
        //bool pointHadActualMove = false;

        // 一旦削除(連接点の変更用)
        //private LineMoveType _MoveType = LineMoveType.None;

        /// <summary>連接点をクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        private void Hotspot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 一旦削除(連接点の変更用)
            //pointHadActualMove = false;
            //trackingPointMouseMove = false;
            //IsSelectd = true;

            FrameworkElement element = e.OriginalSource as FrameworkElement;
            if (element != null)
            {
                if (!_container.ShiftKeyIsPress // シフトキーが押下しない場合
                    || _container.CurrentSelectedControlCollection.Count < 1) // 選択アイコン数が０場合
                {
                    IsSelectd = true;
                    _container.SetWorkFlowElementSelected(this, IsSelectd);
                }
                // 一旦削除(連接点の変更用)
                //_MoveType = LineMoveType.None;

                //if (element.Name == ellipseBegin.Name)
                //{
                //    _MoveType = LineMoveType.Begin;
                //    element.Cursor = Cursors.Hand;
                //}
                //else if (element.Name == ellipseEnd.Name)
                //{
                //    _MoveType = LineMoveType.End;
                //    element.Cursor = Cursors.Hand;
                //}

                //if (_MoveType != LineMoveType.None)
                //{
                //    trackingPointMouseMove = true;
                //    element.CaptureMouse();

                //    e.Handled = true;
                //}
                //else
                //{
                    //_container.SetWorkFlowElementSelected(this, true);
                //}

                _container.CanvasClickFlg = false;

            }
        }

        //// 一旦削除(連接点の変更用)
        ///// <summary>連接点を移動</summary>
        ///// <param name="sender">源</param>
        ///// <param name="e">マウスイベント</param>
        //private void Hotspot_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (trackingPointMouseMove)
        //    {
        //        switch (_MoveType)
        //        {
        //            case LineMoveType.Begin:
        //                this.BeginPosition = e.GetPosition(_container.ContainerCanvas);
        //                linkElement = true;

        //                break;
        //            case LineMoveType.End:
        //                this.EndPosition = e.GetPosition(_container.ContainerCanvas);
        //                linkElement = true;
        //                break;
        //        }

        //        // 半径を調整
        //        arrow.Radius = Point.Subtract(BeginPosition, EndPosition).Length * 0.65;
        //    }
        //}

        /// <summary>連接点の押下を釈放</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        private void Hotspot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 一旦削除(連接点の変更用)
            //if (trackingPointMouseMove)
            //{
            //    FrameworkElement element = e.OriginalSource as FrameworkElement;
            //    if (element != null)
            //    {
            //        element.Cursor = Cursors.Arrow;
            //        element.ReleaseMouseCapture();
            //    }
            //    if (pointHadActualMove == true)
            //    {
            //         一旦削除(連接点の変更用)　
            //        if (linkElement == true && (this.BeginItem != null || this.EndItem != null))
            //        {
            //            //Point pBegin, pEnd;
            //            if (this._MoveType == LineMoveType.Begin && this.BeginItem != null)
            //            {
            //                ConnectType hotspotType = this.BeginItem.GetNearHotspot(e.GetPosition(_container.ContainerCanvas));
            //                this.BeginPosition = this.BeginItem.GetHotspot(hotspotType);
            //                this.BeginConType = hotspotType;
            //                this.BeginItem.SetUnFocus();
            //            }
            //            else if (this._MoveType == LineMoveType.End && this.BeginItem != null)
            //            {
            //                ConnectType hotspotType = this.EndItem.GetNearHotspot(e.GetPosition(_container.ContainerCanvas));
            //                this.EndPosition = this.EndItem.GetHotspot(hotspotType);
            //                this.EndConType = hotspotType;
            //                this.EndItem.SetUnFocus();
            //            }

            //        }

            //        e.Handled = true;

            //        //pointHadActualMove = false;
            //    }

            //this._MoveType = LineMoveType.None;
            //trackingPointMouseMove = false;
            //element.ReleaseMouseCapture();
            //}
            _container.CanvasClickFlg = true;
        }

　　　　#endregion

        #region public メッソド

        /// <summary>イベント解除</summary>
        public void RemoveAllEvent()
        {
            arrow.MouseLeftButtonDown -= Hotspot_MouseLeftButtonDown;
            arrow.MouseLeftButtonUp -= Hotspot_MouseLeftButtonUp;
        }

        /// <summary>True設定 </summary>
        public void RemoveBeginItem(CommonItem a)
        {
            if (BeginItem == a)
                BeginItem = null;
        }

        /// <summary>False設定 </summary>
        public void RemoveEndItem(CommonItem a)
        {
            if (EndItem == a)
                EndItem = null;
        }

        /// <summary>True、またはFalseの位置を設定</summary>
        public void setRuleNameControlPosition()
        {
            double dtop = 0;
            double dleft = 0;

            if (arrow.CenterPoint.Y <= arrow.RoundCenter.Y && arrow.CenterPoint.X <= arrow.RoundCenter.X)
            {
                dtop = 5 * Math.Abs(arrow.CenterPoint.Y - arrow.RoundCenter.Y) / arrow.Radius;
                dleft = 5 * Math.Abs(arrow.CenterPoint.X - arrow.RoundCenter.X) / arrow.Radius;
            }
            if (arrow.CenterPoint.Y <= arrow.RoundCenter.Y && arrow.CenterPoint.X > arrow.RoundCenter.X)
            {
                dtop = -15 * Math.Abs(arrow.CenterPoint.Y - arrow.RoundCenter.Y) / arrow.Radius;
                dleft = 3 * Math.Abs(arrow.CenterPoint.X - arrow.RoundCenter.X) / arrow.Radius;
            }
            if (arrow.CenterPoint.Y > arrow.RoundCenter.Y && arrow.CenterPoint.X <= arrow.RoundCenter.X)
            {
                dtop = -15 * Math.Abs(arrow.CenterPoint.Y - arrow.RoundCenter.Y) / arrow.Radius;
                dleft = 3 * Math.Abs(arrow.CenterPoint.X - arrow.RoundCenter.X) / arrow.Radius;
            }
            if (arrow.CenterPoint.Y > arrow.RoundCenter.Y && arrow.CenterPoint.X > arrow.RoundCenter.X)
            {
                dtop = 5 * Math.Abs(arrow.CenterPoint.Y - arrow.RoundCenter.Y) / arrow.Radius;
                dleft = 5 * Math.Abs(arrow.CenterPoint.X - arrow.RoundCenter.X) / arrow.Radius;
            }

            tbFlowName.SetValue(Canvas.TopProperty, arrow.CenterPoint.Y + dtop);
            tbFlowName.SetValue(Canvas.LeftProperty, arrow.CenterPoint.X + dleft);

        }

        #endregion

        #region private メッソド


        /// <summary>DBデータをセット</summary>
        /// <param name="flowType">フロータイプ</param>
        private void SetDbData(FlowType flowType)
        {
            DataRow row = _container.FlowControlTable.Select("start_job_id='" +
                this.BeginItem.JobId + "' and end_job_id='" + this.EndItem.JobId + "'")[0];

            row["flow_type"] = flowType;
        }
        #endregion


    }

}
