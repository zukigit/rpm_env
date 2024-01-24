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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;

namespace jp.co.ftf.jobcontroller.JobController.Form.CalendarEdit
{
    /// <summary>
    /// FileReadWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FileReadWindow : BaseWindow
    {
        #region フィールド
        private CalendarEdit _calendarEdit;
        private Microsoft.Win32.OpenFileDialog openFileDialog1;
        #endregion

        #region コンストラクタ
        public FileReadWindow(CalendarEdit calendarEdit)
        {
            InitializeComponent();
            _calendarEdit = calendarEdit;
            formatComboBox.SelectedIndex = 0;
        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "FileReadWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_211;
            }
        }
        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>参照ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void refFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            // 初期表示するディレクトリを設定する
            if (Consts.FILEREAD_PATH == null || Consts.FILEREAD_PATH.Equals(""))
            {
                openFileDialog1.InitialDirectory = @"C:\";
            }
            else
            {
                openFileDialog1.InitialDirectory = Consts.FILEREAD_PATH;
            }

            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            openFileDialog1.Filter =
                "All File(*.*)|*.*";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            openFileDialog1.FilterIndex = 2;
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            openFileDialog1.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            openFileDialog1.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            openFileDialog1.CheckPathExists = true;

            //ダイアログを表示する
            if (openFileDialog1.ShowDialog() == true)
            {
                textBox_fileName.Text = openFileDialog1.FileName;
            }
        }

        //*******************************************************************
        /// <summary>キャンセルボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        //*******************************************************************
        /// <summary>ファイル読込クリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void fileRead_Click(object sender, EventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("fileRead_Click", Consts.PROCESS_001);

            // 入力チェック
            if (!InputCheck())
                return;

            string line;
            List<DateTime> selectedDates = new List<DateTime>();
            String format_str = ((FormatComboBoxItem)formatComboBox.SelectedItem).Format;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(textBox_fileName.Text);

                if (!System.IO.File.Exists(textBox_fileName.Text))
                {
                    throw new FileException(Consts.SYSERR_002, null);
                }

                System.IO.DirectoryInfo dirInfoBar = new System.IO.DirectoryInfo(textBox_fileName.Text);
                System.IO.DirectoryInfo dirInfo = dirInfoBar.Parent;

                Consts.FILEREAD_PATH = dirInfo.FullName;

                while ((line = file.ReadLine()) != null)
                {
                    try
                    {
                        DateTime operationDate = DateTime.ParseExact(line, format_str,
                            System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                        selectedDates.Add(operationDate);
                    }
                    catch (Exception ex)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_CALENDAR_FILE_READ_001);
                        return;
                    }

                }
                _calendarEdit.container.CalendarDetailTable.Rows.Clear();

                foreach (DateTime operationDate in selectedDates)
                {
                    DataRow row = _calendarEdit.container.CalendarDetailTable.NewRow();
                    _calendarEdit.container.CalendarDetailTable.Rows.Add(row);

                    row["calendar_id"] = _calendarEdit.CalendarId;
                    row["update_date"] = _calendarEdit.UpdateDate;
                    row["operating_date"] = ConvertUtil.ConverDate2IntYYYYMMDD(operationDate);
                }
                _calendarEdit.container.SetYearCalendarDetail(null);
                file.Close();
                Close();
            }
            catch (ArgumentException ex)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (NotSupportedException ex)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (DirectoryNotFoundException ex)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_022);
            }
            catch (FileNotFoundException ex)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_011);
            }
            catch (UnauthorizedAccessException ex)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_023);
            }
            catch (System.IO.IOException ex)
            {
                CommonDialog.ShowErrorDialogFromMessage(ex.Message);
            }
            catch (Exception ex)
            {
                CommonDialog.ShowErrorDialogFromMessage(ex.Message);
            }
            // 終了ログ
            base.WriteEndLog("fileRead_Click", Consts.PROCESS_001);

        }
        #endregion

        #region privateメソッド
        //*******************************************************************
        /// <summary>入力チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheck()
        {
            // ジョブネット名が未入力の場合
            if (CheckUtil.IsNullOrEmpty(textBox_fileName.Text))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_014);
                return false;
            }
            return true;
        }
        #endregion
    }
}
