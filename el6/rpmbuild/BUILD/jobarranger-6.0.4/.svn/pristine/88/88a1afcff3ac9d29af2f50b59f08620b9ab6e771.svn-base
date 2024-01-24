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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
    /// <summary>曲線クラス（矢印あり）</summary>
    public class ArrowArc : System.Windows.Controls.ContentControl
    {
        #region フィールド

        internal const string LineElementName = "LineElement";
        internal const string RootElementName = "RootElement";
        internal const string ArrowElementName = "ArrowElement";
        internal const string CenterGridElementName = "CenterGridElement";
        internal const string ArrowStartElementName = "ArrowStartElement";
        internal const string LinePathFigureElementName = "LinePathFigureElement";
        internal const string ContentPresenterElementName = "ContentPresenterElement";
        internal const string ContentPresenterCanvasElementName = "ContentPresenterCanvasElement";
        #endregion

        #region フィールド　

        /// <summary>開始点</summary>
        public PathFigure LinePathFigureElement;

        /// <summary>線</summary>
        public ArcSegment lineElement;

        /// <summary>矢印</summary>
        public PolyLineSegment arrowElement;

        /// <summary>開始矢印</summary>
        public PathFigure arrowStartElement;

        /// <summary>表す矢印</summary>
        private ContentPresenter contentPresenterElement;
        #endregion

        #region プロパティ

        /// <summary>直線フラグ</summary>
        public bool IsBeeline
        {
            get
            {
                if (null == this.lineElement)
                {
                    return false;
                }
                else
                {
                    if (this.lineElement.Size == new Size(0, 1))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>ルート</summary>
        public Canvas RootElement { get; private set; }

        /// <summary>中心点</summary>
        public Point CenterPoint
        {
            get
            {
                return this._CenterPoint;
            }
            set
            {
                this._CenterPoint = value;
            }
        }
        Point _CenterPoint = new Point();
        /// <summary>円中心点</summary>
        public Point RoundCenter
        {
            get
            {
                return this._roundCenter;
            }
            set
            {
                this._roundCenter = value;
            }
        }
        Point _roundCenter = new Point();

        private Grid CenterGridElement { get; set; }
        private Canvas ContentPresenterCanvasElement { get; set; }
        #endregion

        #region 依赖プロパティ
        /// <summary>开始点</summary>
        public Point StartPoint
        {
            get
            {
                return (Point)this.GetValue(StartPointProperty);
            }
            set
            {
                this.SetValue(StartPointProperty, value);
            }
        }
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
           "StartPoint",
           typeof(Point),
           typeof(ArrowArc),
           //new FrameworkPropertyMetadata(new Point(100, 10), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
           new PropertyMetadata(new Point(10, 10), new PropertyChangedCallback(ReSize)));

        /// <summary>终点</summary>
        public Point EndPoint
        {
            get
            {
                return (Point)this.GetValue(EndPointProperty);
            }
            set
            {
                this.SetValue(EndPointProperty, value);
            }
        }
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
           "EndPoint",
           typeof(Point),
           typeof(ArrowArc),
           //new FrameworkPropertyMetadata(new Point(100, 10), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
           new PropertyMetadata(new Point(100, 10), new PropertyChangedCallback(ReSize)));

        /// <summary>半径</summary>
        public double Radius
        {
            get
            {
                return (double)this.GetValue(RadiusProperty);
            }
            set
            {
                this.SetValue(RadiusProperty, value);
            }

        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
           "Radius",
           typeof(double),
           typeof(ArrowArc),
           new PropertyMetadata(120.0, new PropertyChangedCallback(ReSize)));

        /// <summary>アウトラインの幅</summary>
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }

        }
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
           "StrokeThickness",
           typeof(double),
           typeof(ArrowArc),
           new PropertyMetadata(2.0, null));

        /// <summary>線の幅</summary>
        public double LineThickness
        {
            get
            {
                return (double)this.GetValue(LineThicknessProperty);
            }
            set
            {
                this.SetValue(LineThicknessProperty, value);
            }

        }
        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
           "LineThickness",
           typeof(double),
           typeof(ArrowArc),
           new PropertyMetadata(2.0, null));

        /// <summary>矢印のサイズ</summary>
        public Size ArrowSize
        {
            get
            {
                return (Size)this.GetValue(ArrowSizeProperty);
            }
            set
            {
                this.SetValue(ArrowSizeProperty, value);
            }

        }
        public static readonly DependencyProperty ArrowSizeProperty = DependencyProperty.Register(
           "ArrowSize",
           typeof(Size),
           typeof(ArrowArc),
           new PropertyMetadata(new Size(4, 8), new PropertyChangedCallback(ReSize)));

        /// <summary>描画方向（時計回り、または反時計回り）</summary>
        public SweepDirection LineSweepDirection
        {
            get
            {
                return (SweepDirection)this.GetValue(LineSweepDirectionProperty);
            }
            set
            {
                this.SetValue(LineSweepDirectionProperty, value);
            }
        }
        public static readonly DependencyProperty LineSweepDirectionProperty = DependencyProperty.Register(
           "SweepDirection",
           typeof(SweepDirection),
           typeof(ArrowArc),
           new PropertyMetadata(SweepDirection.Clockwise, new PropertyChangedCallback(ReSize)));

        /// <summary>大弧フラグ</summary>
        public bool IsLargeArc
        {
            get
            {
                return (bool)this.GetValue(IsLargeArcProperty);
            }
            set
            {
                this.SetValue(IsLargeArcProperty, value);
            }

        }
        public static readonly DependencyProperty IsLargeArcProperty = DependencyProperty.Register(
           "IsLargeArc",
           typeof(bool),
           typeof(ArrowArc),
           new PropertyMetadata(false, new PropertyChangedCallback(ReSize)));

        /// <summary>点線</summary>
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }

        }
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(
           "StrokeDashArray",
           typeof(DoubleCollection),
           typeof(ArrowArc),
           new PropertyMetadata(null));


        /// <summary>矢印の表示フラグ</summary>
        public Visibility ArrowVisibility
        {
            get
            {
                return (Visibility)this.GetValue(ArrowVisibilityProperty);
            }
            set
            {
                this.SetValue(ArrowVisibilityProperty, value);
            }
        }
        public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.Register(
           "ArrowVisibility",
           typeof(Visibility),
           typeof(ArrowArc),
           new PropertyMetadata(Visibility.Visible, null));
        #endregion

        #region コンストラクタ
        /// <summary>曲線</summary>
        public ArrowArc()
        {
            this.DefaultStyleKey = typeof(ArrowArc);

        }
        #endregion

        #region public メッソド


        /// <summary>曲線の更新</summary>
        public void ArrowArcSegment_LayoutUpdated(object sender, EventArgs e)
        {
            OnApplyTemplate();
            this.SetContentPosition(this.CenterPoint);
        }

        /// <summary>線の表す</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.arrowElement = (PolyLineSegment)this.GetTemplateChild(ArrowArc.ArrowElementName);
            this.lineElement = (ArcSegment)this.GetTemplateChild(ArrowArc.LineElementName);
            //this.
            this.arrowStartElement = (PathFigure)this.GetTemplateChild(ArrowArc.ArrowStartElementName);
            this.LinePathFigureElement = (PathFigure)this.GetTemplateChild(ArrowArc.LinePathFigureElementName);
            this.contentPresenterElement = (ContentPresenter)this.GetTemplateChild(ArrowArc.ContentPresenterElementName);
            //FrameworkTemplate.FindName
            this.CenterGridElement = (Grid)this.GetTemplateChild(ArrowArc.CenterGridElementName);
            this.ContentPresenterCanvasElement = (Canvas)this.GetTemplateChild(ArrowArc.ContentPresenterCanvasElementName);
            this.RootElement = (Canvas)this.GetTemplateChild(ArrowArc.RootElementName);

            this.SetPosition();
        }
        #endregion

        /// <summary>再描画</summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ReSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ArrowArc arrowArcSegment = (ArrowArc)d;
            if (arrowArcSegment.lineElement != null)
            {
                arrowArcSegment.SetPosition();
            }
        }

        /// <summary>位置をセット</summary>
        public void SetPosition()
        {
            // 弧
            this.lineElement.Point = this.EndPoint;
            this.LinePathFigureElement.StartPoint = this.StartPoint;
            this.lineElement.IsLargeArc = this.IsLargeArc;
            this.lineElement.SweepDirection = this.LineSweepDirection;
            this.lineElement.Size = new Size(this.Radius, this.Radius);

            // 矢印
            _roundCenter = new Point();
            double k1;

            //円の中心点を取得
            try
            {
                _roundCenter = MathExtension.GetRoundCenter(this.StartPoint, this.EndPoint, this.Radius,
                    this.LineSweepDirection, this.IsLargeArc);

                //円の中心点と矢印点の傾斜度
                k1 = (_roundCenter.Y - this.EndPoint.Y) / (_roundCenter.X - this.EndPoint.X);
            }
            catch (Exception ex)
            {
                if (ex is RadiusException)
                {
                    k1 = (-1) * (this.StartPoint.X - this.EndPoint.X) / (this.StartPoint.Y - this.EndPoint.Y);
                    this.lineElement.Size = new Size(0, 1);
                }
                else
                    return;
            }

            Point centerPoint1, centerPoint2, endPoint11, endPoint12, endPoint21, endPoint22;

            //接線の傾斜度
            double k2 = 0;

            // Y軸方向
            if (double.IsInfinity(k1))
            {
                //開始点と終了点の中点
                this._CenterPoint.X = (this.StartPoint.X + this.EndPoint.X) / 2;
                this._CenterPoint.Y = (this.StartPoint.Y + this.EndPoint.Y) / 2;

                //Y軸方向の直線
                if (this.IsBeeline)
                {
                    if (this.StartPoint.X < this.EndPoint.X)
                    {
                        this.arrowStartElement.StartPoint = new Point(this.EndPoint.X - this.ArrowSize.Height,
                            this.EndPoint.Y - this.ArrowSize.Width);
                        this.arrowElement.Points.Clear();
                        this.arrowElement.Points.Add(this.EndPoint);
                        this.arrowElement.Points.Add(new Point(this.EndPoint.X - this.ArrowSize.Height,
                            this.EndPoint.Y + this.ArrowSize.Width));
                    }
                    else
                    {
                        this.arrowStartElement.StartPoint = new Point(this.EndPoint.X + this.ArrowSize.Height,
                            this.EndPoint.Y - this.ArrowSize.Width);
                        this.arrowElement.Points.Clear();
                        this.arrowElement.Points.Add(this.EndPoint);
                        this.arrowElement.Points.Add(new Point(this.EndPoint.X + this.ArrowSize.Height,
                            this.EndPoint.Y + this.ArrowSize.Width));
                    }
                }
            }
            // X軸方向
            else if (Math.Round(k1, 4) == 0)
            {
                //開始点と終了点の中点
                this._CenterPoint.X = (this.StartPoint.X + this.EndPoint.X) / 2;
                this._CenterPoint.Y = (this.StartPoint.Y + this.EndPoint.Y) / 2;

                //X軸方向の直线
                if (this.IsBeeline)
                {
                    if (this.StartPoint.Y < this.EndPoint.Y)
                    {
                        this.arrowStartElement.StartPoint = new Point(this.EndPoint.X - this.ArrowSize.Width,
                            this.EndPoint.Y - this.ArrowSize.Height);
                        this.arrowElement.Points.Clear();
                        this.arrowElement.Points.Add(this.EndPoint);
                        this.arrowElement.Points.Add(new Point(this.EndPoint.X + this.ArrowSize.Width,
                            this.EndPoint.Y - this.ArrowSize.Height));
                    }
                    else
                    {
                        this.arrowStartElement.StartPoint = new Point(this.EndPoint.X - this.ArrowSize.Width,
                            this.EndPoint.Y + this.ArrowSize.Height);
                        this.arrowElement.Points.Clear();
                        this.arrowElement.Points.Add(this.EndPoint);
                        this.arrowElement.Points.Add(new Point(this.EndPoint.X + this.ArrowSize.Width,
                            this.EndPoint.Y + this.ArrowSize.Height));
                    }
                }
            }
            else
            {
                k2 = (-1) / k1;
                double xOffset = this.ArrowSize.Height / (Math.Sqrt(1 + k2 * k2));

                double xTemp, yTemp;

                // 一番目の点
                xTemp = this.EndPoint.X + xOffset;
                yTemp = k2 * (xTemp - this.EndPoint.X) + this.EndPoint.Y;
                centerPoint1 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                // ニ番目の点
                xTemp = this.EndPoint.X - xOffset;
                yTemp = k2 * (xTemp - this.EndPoint.X) + this.EndPoint.Y;
                centerPoint2 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                k2 = k1;
                xOffset = this.ArrowSize.Width / (Math.Sqrt(1 + k2 * k2));

                // 矢印の点
                xTemp = centerPoint1.X + xOffset;
                yTemp = k2 * (xTemp - centerPoint1.X) + centerPoint1.Y;
                endPoint11 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                xTemp = centerPoint1.X - xOffset;
                yTemp = k2 * (xTemp - centerPoint1.X) + centerPoint1.Y;
                endPoint12 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                // 矢印の点
                xTemp = centerPoint2.X + xOffset;
                yTemp = k2 * (xTemp - centerPoint2.X) + centerPoint2.Y;
                endPoint21 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                xTemp = centerPoint2.X - xOffset;
                yTemp = k2 * (xTemp - centerPoint2.X) + centerPoint2.Y;
                endPoint22 = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));

                //小弧
                if (!this.IsLargeArc)
                {
                    #region 小弧
                    //直線の場合
                    if (this.IsBeeline)
                    {
                        double d1 = (endPoint11.X - this.StartPoint.X) * (endPoint11.X - this.StartPoint.X) + (endPoint11.Y - this.StartPoint.Y) * (endPoint11.Y - this.StartPoint.Y);
                        double d2 = (endPoint21.X - this.StartPoint.X) * (endPoint21.X - this.StartPoint.X) + (endPoint21.Y - this.StartPoint.Y) * (endPoint21.Y - this.StartPoint.Y);

                        if (d1 < d2)
                        {
                            this.arrowStartElement.StartPoint = endPoint11;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint12);
                        }
                        else
                        {
                            this.arrowStartElement.StartPoint = endPoint21;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint22);
                        }
                    }
                    else
                    {
                        //点の位置を判定
                        if (MathExtension.GetPointLeftOrRight(_roundCenter, this.EndPoint, this.StartPoint) ==
                            MathExtension.GetPointLeftOrRight(_roundCenter, this.EndPoint, centerPoint1))
                        {
                            this.arrowStartElement.StartPoint = endPoint11;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint12);
                        }
                        else
                        {
                            this.arrowStartElement.StartPoint = endPoint21;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint22);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 大弧
                    //直線の場合
                    if (this.IsBeeline)
                    {
                        double d1 = (endPoint11.X - this.StartPoint.X) * (endPoint11.X - this.StartPoint.X) + (endPoint11.Y - this.StartPoint.Y) * (endPoint11.Y - this.StartPoint.Y);
                        double d2 = (endPoint21.X - this.StartPoint.X) * (endPoint21.X - this.StartPoint.X) + (endPoint21.Y - this.StartPoint.Y) * (endPoint21.Y - this.StartPoint.Y);

                        if (d1 < d2)
                        {
                            this.arrowStartElement.StartPoint = endPoint11;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint12);
                        }
                        else
                        {
                            this.arrowStartElement.StartPoint = endPoint21;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint22);
                        }
                    }
                    else
                    {
                        //点の位置を判定
                        if (MathExtension.GetPointLeftOrRight(_roundCenter, this.EndPoint, this.StartPoint) == (-1) * MathExtension.GetPointLeftOrRight(_roundCenter, this.EndPoint, centerPoint1))
                        {
                            this.arrowStartElement.StartPoint = endPoint11;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint12);
                        }
                        else
                        {
                            this.arrowStartElement.StartPoint = endPoint21;
                            this.arrowElement.Points.Clear();
                            this.arrowElement.Points.Add(this.EndPoint);
                            this.arrowElement.Points.Add(endPoint22);
                        }
                    }
                    #endregion
                }

                this.GetCenterPositen(_roundCenter);
            }
        }

        /// <summary>中心点を初期化する</summary>
        public void InitCenterPosition()
        {
            //円の中心点を取得
            _roundCenter = MathExtension.GetRoundCenter(this.StartPoint, this.EndPoint, this.Radius,
                this.LineSweepDirection, this.IsLargeArc);

            GetCenterPositen(_roundCenter);

        }

        /// <summary>中心点を取得</summary>
        /// <param name="roundCenter"></param>
        public void GetCenterPositen(Point roundCenter)
        {
            //線の中点
            Point centerPointTemp = new Point();
            this.CenterPoint = new Point();
            centerPointTemp.X = (this.EndPoint.X + this.StartPoint.X) / 2;
            centerPointTemp.Y = (this.EndPoint.Y + this.StartPoint.Y) / 2;

            if (this.IsBeeline)
            {
                this._CenterPoint = centerPointTemp;
            }
            else
            {
                double d = Math.Sqrt((centerPointTemp.X - roundCenter.X) * (centerPointTemp.X - roundCenter.X) + (centerPointTemp.Y - roundCenter.Y) * (centerPointTemp.Y - roundCenter.Y));

                if (this.IsLargeArc)
                {
                    this._CenterPoint.X = ((this.Radius + d) * (roundCenter.X - centerPointTemp.X) + d * centerPointTemp.X) / d;
                    this._CenterPoint.Y = ((this.Radius + d) * (roundCenter.Y - centerPointTemp.Y) + d * centerPointTemp.Y) / d;
                }
                else
                {
                    this._CenterPoint.X = (this.Radius * (centerPointTemp.X - roundCenter.X) + d * roundCenter.X) / d;
                    this._CenterPoint.Y = (this.Radius * (centerPointTemp.Y - roundCenter.Y) + d * roundCenter.Y) / d;
                }
            }
        }

        /// <summary>内容の位置をセット</summary>
        /// <param name="centerPoint">円の中心点</param>
        public void SetContentPosition(Point centerPoint)
        {
            if (this.contentPresenterElement != null)
            {
                this.ContentPresenterCanvasElement.Height = this.contentPresenterElement.ActualHeight;
                this.ContentPresenterCanvasElement.Width = this.contentPresenterElement.ActualWidth;
            }
            Canvas.SetLeft(this.CenterGridElement, centerPoint.X);
            Canvas.SetTop(this.CenterGridElement, centerPoint.Y);
        }
    }
}
