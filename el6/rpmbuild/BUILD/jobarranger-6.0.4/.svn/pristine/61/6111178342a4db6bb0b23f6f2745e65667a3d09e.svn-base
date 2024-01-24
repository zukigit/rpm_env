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
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;
using System.Data;
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
    /// コンテナインターフェイス
    /// </summary>
    public interface IContainer
    {

        #region プロパティ

        /// <summary>ジョブネットID</summary>
        string JobnetId { get; set; }

        /// <summary>更新日</summary>
        string TmpUpdDate { get; set; }

        /// <summary>コンテナ（ジョブフロー作成区域）</summary>
        Canvas ContainerCanvas { get; }

        /// <summary>父ウィンドウ</summary>
        ContentControl ParantWindow { get; set; }

        /// <summary>ジョブアイコンリスト</summary>
        Hashtable JobItems { get; }

        /// <summary>コンテナの空白区域にクリック判定フラグ</summary>
        bool CanvasClickFlg { get; set; }

        /// <summary>シフトキー押下判定フラグ</summary>
        bool ShiftKeyIsPress { get; }

        /// <summary>シフトキーで複数選択されたか判定フラグ</summary>
        bool IsSelectedByShiftKey { get; set; }

        /// <summary>選択するアイコンリスト</summary>
        List<Control> CurrentSelectedControlCollection { get; }

        /// <summary>設定済ジョブＩＤ</summary>
        Hashtable SetedJobIds { get; set; }

        /// <summary>Zoom Value</summary>
        double ZoomValue { get; }

        ///// <summary>選択した部品リストの最左位置のX座標</summary>
        //double TopYOfSelection { get; set; }

        #region データ格納場所
        /// <summary>ジョブネットアイコン設定テーブル</summary>
        DataTable JobnetControlTable { get; set; }

        /// <summary>ジョブ管理テーブル</summary>
        DataTable JobControlTable { get; set; }

        /// <summary>フロー管理テーブル</summary>
        DataTable FlowControlTable { get; set; }

        /// <summary>計算アイコン設定テーブル</summary>
        DataTable IconCalcTable { get; set; }

        /// <summary>終了アイコン設定テーブル</summary>
        DataTable IconEndTable { get; set; }

        /// <summary>拡張ジョブアイコン設定テーブル</summary>
        DataTable IconExtjobTable { get; set; }

        // <summary>条件分岐アイコン設定テーブル</summary>
        DataTable IconIfTable { get; set; }

        // <summary>情報取得アイコン設定テーブル</summary>
        DataTable IconInfoTable { get; set; }

        // <summary>ジョブネットアイコン設定テーブル</summary>
        DataTable IconJobnetTable { get; set; }

        // <summary>ジョブアイコン設定テーブル</summary>
        DataTable IconJobTable { get; set; }

        // <summary>ジョブコマンド設定テーブル</summary>
        DataTable JobCommandTable { get; set; }

        // <summary>ジョブ変数設定テーブル</summary>
        DataTable ValueJobTable { get; set; }

        // <summary>ジョブコントローラ変数設定テーブル</summary>
        DataTable ValueJobConTable { get; set; }

        // <summary>タスクアイコン設定テーブル</summary>
        DataTable IconTaskTable { get; set; }

        // <summary>ジョブコントローラ変数アイコン設定テーブル</summary>
        DataTable IconValueTable { get; set; }

        // <summary>ジョブコントローラ変数定義テーブル</summary>
        DataTable DefineValueJobconTable { get; set; }

        // <summary>拡張ジョブ定義テーブル</summary>
        DataTable DefineExtJobTable { get; set; }

        // <summary>ファイル転送設定テーブル</summary>
        DataTable IconFcopyTable { get; set; }

        // <summary>ファイル待ち合わせ設定テーブル</summary>
        DataTable IconFwaitTable { get; set; }

        // <summary>リブート設定テーブル</summary>
        DataTable IconRebootTable { get; set; }

        // <summary>保留解除アイコン設定テーブル</summary>
        DataTable IconReleaseTable { get; set; }

        //added by YAMA 2014/02/06
        // <summary>Zabbix連携アイコン設定テーブル</summary>
        DataTable IconCooperationTable { get; set; }

        //added by YAMA 2014/05/19
        // <summary>エージェントレスアイコン設定テーブル</summary>
        DataTable IconAgentlessTable { get; set; }

        #endregion

        #endregion

        #region メッソド


        /// <summary>部品を削除</summary>
        /// <param name="a">部品</param>
        void RemoveItem(Control a);

        /// <summary>フローを削除</summary>
        /// <param name="a">フロー</param>
        void RemoveFlow(IFlow a);

        /// <summary>選択アイコンを選択リストに追加</summary>
        /// <param name="uc">アイコン/param>
        void AddSelectedControl(Control uc);

        /// <summary>選択アイコンを選択リストから削除</summary>
        /// <param name="uc">アイコン/param>
        void RemoveSelectedControl(Control uc);

        /// <summary>アイコンを選択</summary>
        /// <param name="uc">アイコン/param>
        void SetWorkFlowElementSelected(Control uc, bool isSelect);

        /// <summary>アイコンを移動</summary>
        /// <param name="x">x座標/param>
        /// <param name="y">y座標/param>
        /// <param name="uc">例外のアイコン/param>
        void MoveControlCollectionByDisplacement(double x, double y, UserControl uc);

        /// <summary>アイコンのTRUE、FALSE位置設定</summary>
        /// <param name="uc">例外のアイコン/param>
        void SetControlCollectionItemAndRuleNameControlPosition(UserControl uc);

        /// <summary>選択状態を外す</summary>
        /// <param name="x">アイコン/param>
        void ClearSelectFlowElement(Control uc);

        /// <summary>選択アイコンを削除</summary>
        void DeleteSelectedControl();

        /// <summary>アイコンの含む判定</summary>
        bool Contains(UIElement uiel);

        /// <summary>マウスがコンテナ内かどうか判定</summary>
        bool MouseIsInContainer { get; set; }

        // 2012.11.1一旦削除（線の連接点を別のアイコンに変更用）

        //CommonItem GetItemByPoint(Point point);

        /// <summary>フローを作成</summary>
        /// <param name="lineType">線のタイプ</param>
        /// <param name="item1">開始ジョブ</param>
        /// <param name="item2">終了ジョブ</param>
        /// <param name="flowType">フロータイプ</param>
        /// <param name="editType">編集タイプ</param>
        void MakeFlow(FlowLineType lineType, IRoom item1, IRoom item2,
            int flowType, Consts.EditType editType);

        /// <summary>ジョブ位置設定</summary>
        /// <param name="jobId">ジョブＩＤ</param>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        void SetItemPosition(string jobId, double x, double y);

        #endregion

    }
}
