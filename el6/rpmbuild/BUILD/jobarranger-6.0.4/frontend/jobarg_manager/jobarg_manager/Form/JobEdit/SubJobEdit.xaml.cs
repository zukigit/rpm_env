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
using jp.co.ftf.jobcontroller.Common;
using System.Data;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM  2012/11/13 新規作成<BR>                    　　   *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// SubJobEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class SubJobEdit : BaseWindow
    {
        #region フィールド

        /// <summary> ジョブ編集画面 </summary>
        JobEdit jobEdit;

        /// <summary> フォーカス強制処理 </summary>
        private bool _needFocus;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="innerJobnetId">実行ジョブネットID</param>
        public SubJobEdit(JobArrangerWindow parentWindow, string jobnetId, string updDate, Consts.EditType editType, bool needFocus)
        {
            InitializeComponent();
            jobEdit = new JobEdit(parentWindow, jobnetId, updDate, editType);
            _successFlg = false;
            if (jobEdit.SuccessFlg)
                _successFlg = true;
            jobEdit.IsSubJobnet = true;
            SubJobnetGrid.Children.Add(jobEdit);
            _needFocus = needFocus;

        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "SubJobEdit";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_230;
            }
        }
        /// <summary>成功フラグ</summary>
        private bool _successFlg = false;
        public bool SuccessFlg
        {
            get
            {
                return _successFlg;
            }
            set
            {
                _successFlg = true;
            }
        }
        #endregion

        #region イベント
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            jobEdit.Rollback();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Topmost = false;
            this.Focusable = true;
            Keyboard.Focus(this);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
        #endregion
    }
}
