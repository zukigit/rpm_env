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
    /// フローインターフェイス
    /// </summary>
    public interface IFlow
    {
        #region プロパティ

        /// <summary>選択判定</summary>
        bool IsSelectd { get; set; }

        /// <summary>開始点</summary>
        Point BeginPosition { get; set; }

        /// <summary>終了点</summary>
        Point EndPosition { get; set; }

        /// <summary>開始点の連接タイプ（上、下、左、右）</summary>
        ConnectType BeginConType { get; set; }

        /// <summary>終了点の連接タイプ（上、下、左、右）</summary>
        ConnectType EndConType { get; set; }

        /// <summary>コンテナ</summary>
        IContainer Container { get; set; }

        /// <summary>開始アイコン</summary>
        IRoom BeginItem { get; set; }

        /// <summary>終了アイコン</summary>
        IRoom EndItem { get; set; }

        #endregion

        #region メッソド

        /// <summary>Trueをセット</summary>
        void SetTrue(Consts.EditType editType);

        /// <summary>Falseアイコン</summary>
        void SetFalse(Consts.EditType editType);

        /// <summary>条件の位置をセット</summary>
        void setRuleNameControlPosition();

        #endregion
    }
}
