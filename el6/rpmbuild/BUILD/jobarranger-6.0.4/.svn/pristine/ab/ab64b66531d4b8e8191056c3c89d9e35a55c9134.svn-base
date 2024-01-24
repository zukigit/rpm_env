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
using System.Windows.Media;
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
    //added by YAMA 2014/02/04
    /// <summary>各ジョブタイプ</summary>
    //public enum ElementType { START = 0, END, IF, ENV, JOB, JOBNET, MTS, MTE, LOOP, EXTJOB, CAL, TASK, INF, IFE, FCOPY, FWAIT, REBOOT, RELEASE, NONE  }
    public enum ElementType { START = 0, END, IF, ENV, JOB, JOBNET, MTS, MTE, LOOP, EXTJOB, CAL, TASK, INF, IFE, FCOPY, FWAIT, REBOOT, RELEASE, AGENTLESS, COOPERATION, NONE }

    /// <summary>線のタイプ：直線、曲線</summary>
    public enum FlowLineType { Line = 0, Curve }

    /// <summary>線の条件タイプ</summary>
    public enum FlowType { DEFAULT = 0, TRUE, FALSE }

    /// <summary>アイコンのフォーカス状態</summary>
    public enum IElementState { Focus = 0, UnFocus, Selected }

    /// <summary>線の移動タイプ</summary>
    public enum LineMoveType { None = 0, Begin, Center, End, Line };

    /// <summary>連接点のタイプ</summary>
    public enum ConnectType { LEFT = 0, RIGHT, TOP, BOTTOM };

    /// <summary>実行ジョブ状態</summary>
    public enum RunJobStatusType { None = 0, Prepare, During, Normal, RunErr, Abnormal,ForceStop };

    /// <summary>ジョブ実行処理フラグ</summary>
    public enum RunJobMethodType { NORMAL = 0, HOLD, SKIP, STOP, RERUN };

    /// <summary>ジョブ実行タイムアウト</summary>
    public enum RunJobTimeoutType { NORMAL = 0, TIMEOUT };

    /// <summary>画面操作種別</summary>
    public enum OperationType { NONE, ADD_ITEMS, DEL_ITEMS, ADD_FLOW, DEL_FLOW, SET_TRUE_FLOW,
                                SET_FALSE_FLOW, UPDATE_ICON, MOVE_ITEM,
                                SET_HOLD, SET_UNHOLD, SET_SKIP, SET_UNSKIP};

    //added by YAMA 2014/04/25
    /// <summary>展開状況</summary>
    public enum LoadStausType { None = 0, LoadErr, Delay, Skip };


    /// <summary>データ種別</summary>
    //added by YAMA 2014/02/04  add-> COOPERATION
    public enum DataType
    {
        JOB_CONTROL, FLOW, JOB, COMMAND, VALUE_JOB, VALUE_JOBCON, JOBNET,
        CAL, END, IF, ENV, EXTJOB, INF, TASK, FCOPY, FWAIT, REBOOT, RELEASE, COOPERATION, AGENTLESS
    };

    //added by Park.iggy 2015/04/30
    /// <summary>ジョブネット停止フラグ</summary>
    public enum JobnetAbortFlag { FALASE = 0, TRUE};


    /// <summary>
    /// ジョブインターフェイス
    /// </summary>
    public interface IElement
    {
        #region プロパティ
        /// <summary>コンテナ</summary>
        IContainer Container { get; set; }

        /// <summary>幅 </summary>
        double PicWidth { get; }

        /// <summary>高さ</summary>
        double PicHeight { get; }

        /// <summary>ジョブID</summary>
        string JobId { get; set; }

        /// <summary>ジョブ名</summary>
        string JobName { get; set; }

        /// <summary>上連接点</summary>
        Point TopConnectPosition { get; }

        /// <summary>下連接点</summary>
        Point BottomConnectPosition { get; }

        /// <summary>左連接点</summary>
        Point LeftConnectPosition { get; }

        /// <summary>右連接点</summary>
        Point RightConnectPosition { get; }

        /// <summary>内部実行処理用ジョブID</summary>
        string InnerJobId { get; set; }

        /// <summary>処理フラグ</summary>
        RunJobMethodType MethodType { get; set; }

        //Point HotspotLeft { get; set; }
        //Point HotspotTop { get; set; }
        //Point HotspotRight { get; set; }
        //Point HotspotBottom { get; set; }

        //AbstractEntity Entity { get; set; }

        #endregion

        #region メッソド

        /// <summary>選択色をセット</summary>
        void SetSelectedColor();

        /// <summary>アイコンの色をリセット</summary>
        void ResetInitColor();

        /// <summary>フォーカス</summary>
        void SetFocus();

        /// <summary>フォーカスをはずす</summary>
        void SetUnFocus();

        /// <summary>選択</summary>
        void SetSelected();

        /// <summary>部品区の色に初期化</summary>
        void InitSampleColor();

        /// <summary>状態色をセット</summary>
        void SetStatusColor(SolidColorBrush color);

        //added by YAMA 2014/07/01
        /// <summary>文字色をセット</summary>
        void SetStatusCharacterColor(SolidColorBrush color);

        /// <summary>ToolTipをセット</summary>
        void SetToolTip();

        /// <summary>ToolTipをリセット</summary>
        void ResetToolTip(string toolTip);

        //Point GetHotspot(ConnectType hotspotType);

        //ConnectType GetNearHotspot(Point point);
        #endregion

    }
}
