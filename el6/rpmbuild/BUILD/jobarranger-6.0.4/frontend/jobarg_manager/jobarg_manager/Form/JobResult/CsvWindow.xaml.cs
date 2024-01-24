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
using System.Collections;
using System.IO;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobResult
{
    /// <summary>
    /// CsvWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CsvWindow : BaseWindow
    {
        #region フィールド

        /// <summary>実行結果データテーブル</summary>
        private DataTable _resultDt;

        //CSV出力カラム名
        private static String[] CSV_COLUMN_NAMES = { "log_date",
                                                  "inner_jobnet_main_id",
                                                  "inner_jobnet_id",
                                                  "run_type",
                                                  "public_flag",
                                                  "jobnet_id",
                                                  "job_id",
                                                  "message_id",
                                                  "message",
                                                  "jobnet_name",
                                                  "job_name",
                                                  "user_name",
                                                  "update_date",
                                                  "return_code"
                                                };
        private static Hashtable CSV_DATE_CONVERT = new Hashtable();
        private static Hashtable CSV_MSEC_DATE_CONVERT = new Hashtable();
        #endregion

        #region コンストラクタ
        public CsvWindow(DataTable resultDt)
        {
            InitializeComponent();
            _resultDt = resultDt;
            DataContext = this;
            CSV_MSEC_DATE_CONVERT["log_date"] = "1";
            CSV_DATE_CONVERT["update_date"] = "1";
        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "CsvWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_510;
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
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

            // 初期表示するディレクトリを設定する
            if (Consts.CSV_PATH == null || Consts.CSV_PATH.Equals(""))
            {
                saveFileDialog1.InitialDirectory = @"C:\";
            }
            else
            {
                saveFileDialog1.InitialDirectory = Consts.CSV_PATH;
            }

            // ファイルのフィルタを設定する
            saveFileDialog1.Filter = "csv File(*.csv)|*.csv";

            // ファイルの種類 の初期設定を 2 番目に設定する (初期値 1)
            saveFileDialog1.FilterIndex = 2;

            // ダイアログボックスを閉じる前に現在のディレクトリを復元する (初期値 false)
            saveFileDialog1.RestoreDirectory = true;

            // 存在しないファイルを指定した場合は、
            // 新しく作成するかどうかの問い合わせを表示する (初期値 false)
            /* added by YAMA 2014/09/22 フォルダ選択ダイアログの操作性向上 */
            // saveFileDialog1.CreatePrompt = true;
            saveFileDialog1.CreatePrompt = false;
            saveFileDialog1.OverwritePrompt = true;

            //ダイアログを表示する
            if (saveFileDialog1.ShowDialog() == true )
            {
                textBox_fileName.Text = saveFileDialog1.FileName;
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
        /// <summary>ＯＫボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void ok_Click(object sender, EventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("ok_Click", Consts.PROCESS_012);

            // 入力チェック
            if (!InputCheck())
                return;

            try
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("Shift_JIS");

                //開く
                System.IO.StreamWriter sr =
                    new System.IO.StreamWriter(textBox_fileName.Text, false, enc);


                int colCount = CSV_COLUMN_NAMES.Length;
                int lastColIndex = CSV_COLUMN_NAMES.Length - 1;

                //ヘッダを書き込む
                for (int i = 0; i < CSV_COLUMN_NAMES.Length; i++)
                {
                    //ヘッダの取得
                    string field = CSV_COLUMN_NAMES[i];

                    field = "\"" + field.Replace("_", " ") + "\"";
                    //フィールドを書き込む
                    sr.Write(field);
                    //カンマを書き込む
                    if (lastColIndex > i)
                    {
                        sr.Write(',');
                    }
                }
                //改行する
                sr.Write("\r\n");

                //レコードを書き込む
                foreach (DataRow row in _resultDt.Rows)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        //フィールドの取得
                        string field = row[CSV_COLUMN_NAMES[i]].ToString();
                        if (CSV_MSEC_DATE_CONVERT.ContainsKey(CSV_COLUMN_NAMES[i]))
                        {
                            field = (ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(field))).ToString("yyyy/MM/dd HH:mm:ss.fff");
                        }
                        if (CSV_DATE_CONVERT.ContainsKey(CSV_COLUMN_NAMES[i]))
                        {
                            field = (ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(field))).ToString("yyyy/MM/dd HH:mm:ss");
                        }

                        if (field.IndexOf('"') > -1)
                        {
                            //"を""とする
                            field = field.Replace("\"", "\"\"");
                        }
                        field = "\"" + field + "\"";
                        //フィールドを書き込む
                        sr.Write(field);
                        //カンマを書き込む
                        if (lastColIndex > i)
                        {
                            sr.Write(',');
                        }
                    }
                    //改行する
                    sr.Write("\r\n");
                }

                //閉じる
                sr.Close();
                this.Close();

                System.IO.DirectoryInfo dirInfoBar = new System.IO.DirectoryInfo(textBox_fileName.Text);
                System.IO.DirectoryInfo dirInfo = dirInfoBar.Parent;

                Consts.CSV_PATH = dirInfo.FullName;
            }
            catch (ArgumentException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (NotSupportedException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (DirectoryNotFoundException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_022);
            }
            catch (UnauthorizedAccessException)
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
            base.WriteEndLog("ok_Click", Consts.PROCESS_012);
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
