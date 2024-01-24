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
using System.Windows.Input;
using jp.co.ftf.jobcontroller.Common;

namespace jp.co.ftf.jobcontroller.JobController
{
	/// <summary>
    /// TextBoxに、全角入力制限機能を提供するためのクラスです。
    /// </summary>
    public class HankakuTextChangeEvent
    {

        #region イベント追加処理

        /// <summary>
        /// 指定されたTextBoxに、IME状態をチェックしつつ入力バイト数制限を行うTextChangedイベントを追加します。
        /// 入力バイト数制限に使用する値は、TextBoxのMaxLengthの値です。
        /// </summary>
        /// <param name="t">イベントを追加するTextBox</param>
        public void AddTextChangedEventHander(TextBox t)
        {
            if (t == null) return;

            t.TextChanged += new TextChangedEventHandler(OnTextChanged);
        }

        #endregion

        #region 入力制限処理

        /// <summary>
        /// TextChangedイベントの処理を行います。
        /// </summary>
        /// <param name="sender">処理対象のTextBox</param>
        /// <param name="e">イベント引数</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = sender as TextBox;

            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;

            String newText = String.Empty;

            for (int i = 0; i < textBox.Text.Length;i++ )
            {
                String str = textBox.Text.Substring(i, 1);
                if (CheckUtil.EncSJis.GetByteCount(str) == 1)
                {
                    newText += str;
                }
                else
                {
                    selectionStart--;
                }

                if (textBox.MaxLength >0 && CheckUtil.EncSJis.GetByteCount(newText) >= textBox.MaxLength)
                    break;
            }

            textBox.Text = newText;
            textBox.Select((selectionStart + selectionLength), 0);
        }


        #endregion

    }
}
