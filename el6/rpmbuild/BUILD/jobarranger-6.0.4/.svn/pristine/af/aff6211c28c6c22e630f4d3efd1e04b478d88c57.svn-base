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
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/09 新規作成<BR>                           *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
/// <summary>
/// Container.xaml の相互作用ロジック
/// </summary>
    public partial class JobnetContainer : UserControl, IContainer
    {
        #region コンストラクタ
        public JobnetContainer()
        {
            // 初期化
            InitializeComponent();
        }
        #endregion

        #region プロパティ

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
        /// <summary>コンテナの空白区域にクリック判定フラグ</summary>
        public bool CanvasClickFlg { get; set; }

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

        /// <summary>zoom value</summary>
        public double ZoomValue
        {
            get
            {
                return (double)zoomSlider.Value;
            }
        }

        #endregion

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


        #region public メソッド

        /// <summary>部品を削除</summary>
        /// <param name="a">部品</param>
        public void RemoveItem(Control a)
        {
        }

        /// <summary>フローを削除</summary>
        /// <param name="a">フロー</param>
        public void RemoveFlow(IFlow a)
        {
        }

        /// <summary>選択アイコンを選択リストに追加</summary>
        /// <param name="uc">アイコン/param>
        public void AddSelectedControl(Control uc)
        {
        }

        /// <summary>選択アイコンを選択リストから削除</summary>
        /// <param name="uc">アイコン/param>
        public void RemoveSelectedControl(Control uc)
        {
        }

        /// <summary>アイコンを選択</summary>
        /// <param name="uc">アイコン/param>
        public void SetWorkFlowElementSelected(Control uc, bool isSelect)
        {
        }

        /// <summary>アイコンを移動</summary>
        /// <param name="x">x座標/param>
        /// <param name="y">y座標/param>
        /// <param name="uc">例外のアイコン/param>
        public void MoveControlCollectionByDisplacement(double x, double y, UserControl uc)
        {
        }

        /// <summary>アイコンのTRUE、FALSE位置設定</summary>
        /// <param name="uc">例外のアイコン/param>
        public void SetControlCollectionItemAndRuleNameControlPosition(UserControl uc)
        {
        }

        /// <summary>選択状態を外す</summary>
        /// <param name="x">アイコン/param>
        public void ClearSelectFlowElement(Control uc)
        {
        }

        /// <summary>選択アイコンを削除</summary>
        public void DeleteSelectedControl()
        {
        }

        /// <summary>アイコンの含む判定</summary>
        public bool Contains(UIElement uiel)
        {
            return true;
        }

        /// <summary>マウスがコンテナ内かどうか判定</summary>
        public bool MouseIsInContainer { get; set; }

        //*******************************************************************
        /// <summary>設定をクリック</summary>
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
        /// <summary>ジョブ位置設定</summary>
        /// <param name="jobId">ジョブＩＤ</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public void SetItemPosition(string jobId, double x, double y)
        {
        }
        #endregion

    }



}
