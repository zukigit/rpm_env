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
using jp.co.ftf.jobcontroller.Common;
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
    /// アイコンの格納場所インターフェイス
    /// </summary>
    public interface IRoom
    {
        #region プロパティ

        /// <summary>アイコン移動の委任</summary>
        event MoveItemDelegate ItemMove;

        /// <summary>アイコンのタイプ</summary>
        ElementType ElementType { get; }

        /// <summary>内容アイコン</summary>
        IElement ContentItem { get; set; }

        /// <summary>選択フラグ</summary>
        bool IsSelectd { get; set; }

        /// <summary>コンテナ</summary>
        IContainer Container { get; set; }

        /// <summary>ジョブID</summary>
        string JobId { get; set; }

        /// <summary>ジョブ名</summary>
        string JobName { get; set; }

        /// <summary>幅</summary>
        double PicWidth { get; }

        /// <summary>高さ</summary>
        double PicHeight { get; }

        /// <summary>ジョブの中心点</summary>
        Point CenterPoint { get; set; }

        /// <summary>ジョブの上点</summary>
        Point TopConnectPosition { get; }

        /// <summary>ジョブの下点</summary>
        Point BottomConnectPosition { get; }

        /// <summary>ジョブの左点</summary>
        Point LeftConnectPosition { get; }

        /// <summary>ジョブの右点</summary>
        Point RightConnectPosition { get; }

        /// <summary>ジョブの右点</summary>
        Consts.EditType ItemEditType { get; set; }

        #endregion

        #region メッソド

        /// <summary>アイコンを削除</summary>
        void Delete();

        /// <summary>初期色をリセット</summary>
        void ResetInitColor();

        /// <summary>開始フロー（OUTフロー）を追加</summary>
        void AddBeginFlow(IFlow flow);

        /// <summary>終了フロー（INフロー）を追加</summary>
        void AddEndFlow(IFlow flow);

        /// <summary>開始フロー（OUTフロー）を削除</summary>
        void RemoveBeginFlow(IFlow flow);

        /// <summary>終了フロー（INフロー）を削除</summary>
        void RemoveEndFlow(IFlow flow);

        /// <summary>選択色をセット</summary>
        void SetSelectedColor();

        /// <summary>フォーカスをリリース</summary>
        void SetUnFocus();

        /// <summary>設定画面を表示</summary>
        void ShowIconSetting(bool isSetting);

        /// <summary>タイプよって連接点を取得</summary>
        Point GetHotspot(ConnectType hotspotType);

        /// <summary>最近の連接点タイプを取得</summary>
        ConnectType GetNearHotspot(Point point);

        #endregion

    }
}
