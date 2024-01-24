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
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/15 新規作成<BR>                            *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// コントロールの共通クラス.
    /// </summary>
    public abstract class EditBaseUserControl : BaseUserControl
    {

        #region フィールド

        /// <summary>親</summary>
        public abstract JobArrangerWindow ParantWindow { get;  set;}

        /// <summary>成功フラグ</summary>
        public abstract bool SuccessFlg { get; set; }

        /// <summary>ヘルスチェックフラグ</summary>
        public abstract bool HealthCheckFlag { get; set; }

        /// <summary>テキストボックス半角制御</summary>
        public HankakuTextChangeEvent HankakuTextChangeEvent = new HankakuTextChangeEvent();

        #endregion

        #region コンストラクタ

        public EditBaseUserControl()
        {
        }

        #endregion

        #region publicメソッド

        public virtual bool HasEditedCheck()
        {
            return true;
        }

        public virtual bool RegistObject()
        {
            return true;
        }

        public virtual void Rollback()
        {
        }

        public virtual void Commit()
        {
        }

        public virtual void ResetTree(String objectId)
        {
        }

        #endregion

    }
}
