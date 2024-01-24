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

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// ImportWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ImportWindow : BaseWindow
    {
        #region フィールド
        private String TABLE_NAME_USER_INFO = "UserInfo";
        public JobArrangerWindow _jobArrangerWindow;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImportWindow(JobArrangerWindow jobArrangerWindow)
        {
            InitializeComponent();
            _jobArrangerWindow = jobArrangerWindow;
            DataContext = this;
        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "ImportWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_250;
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
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            // 初期表示するディレクトリを設定する
            if (Consts.IMPORT_PATH == null || Consts.IMPORT_PATH.Equals(""))
            {
                openFileDialog1.InitialDirectory = @"C:\";
            }
            else
            {
                openFileDialog1.InitialDirectory = Consts.IMPORT_PATH;
            }

            // ファイルのフィルタを設定する
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

            // ファイルの種類 の初期設定を 2 番目に設定する (初期値 1)
            openFileDialog1.FilterIndex = 2;

            // ダイアログボックスを閉じる前に現在のディレクトリを復元する (初期値 false)
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
        /// <summary>ＯＫボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void ok_Click(object sender, EventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("ok_Click", Consts.PROCESS_013);

            // 入力チェック
            if (!InputCheck())
                return;

            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(textBox_fileName.Text, System.IO.FileMode.Open);
                System.IO.DirectoryInfo dirInfoBar = new System.IO.DirectoryInfo(textBox_fileName.Text);
                System.IO.DirectoryInfo dirInfo = dirInfoBar.Parent;

                Consts.IMPORT_PATH = dirInfo.FullName;

                DataSet ds = new DataSet();
                try
                {
                    ds.ReadXml(fs, XmlReadMode.InferSchema);
                }
                catch (Exception)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_IMPORT_001);
                    return;
                }
                try
                {
                    DataTable userInfo = checkUserInfo(ds);
                    if (userInfo == null)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_IMPORT_002);
                        return;
                    }
                    if (!checkImportAuth(userInfo))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_IMPORT_003);
                        return;
                    }

                    Consts.ImportResultType result = DBUtil.ImportForm(ds, (bool)CheckBoxOverride.IsChecked);
                    if(result == Consts.ImportResultType.DubleKeyErr)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_IMPORT_004);
                        return;
                    }
                    if (result == Consts.ImportResultType.RelationErr)
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_IMPORT_005);
                        return;
                    }
                    fs.Close();
                    Close();
                    _jobArrangerWindow.RefreshObjectList();
                }
                catch (Exception ex1)
                {
                    throw ex1;
                }
                finally
                {
                    fs.Close();
                }
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
            catch (FileNotFoundException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_011);
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
            base.WriteEndLog("ok_Click", Consts.PROCESS_013);

        }
        #endregion

        #region privateメソッド

        /// <summary>ユーザー情報有無チェック</summary>
        /// <param name="ds">XMLファイルから読み込んだDataSet</param>
        private DataTable checkUserInfo(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName.Equals(TABLE_NAME_USER_INFO))
                {
                    if (dt.Rows.Count == 1 && (dt.Rows[0]["user_name"] != null) && (dt.Rows[0]["type"] != null)) return dt;
                    return dt;
                }
            }
            return null;
        }

        /// <summary>権限チェック</summary>
        /// <param name="userInfo">ユーザー情報テーブル</param>
        private bool checkImportAuth(DataTable userInfo)
        {
            DataRow dr = userInfo.Rows[0];
            if (LoginSetting.Authority.Equals(Consts.AuthorityEnum.SUPER)) return true;
            if ((((String)dr["type"]).Equals(Consts.AuthorityEnum.SUPER.ToString())) && (LoginSetting.Authority < Consts.AuthorityEnum.SUPER)) return false;
            if (DBUtil.checkImportAuth(dr)) return true;
            return false;
        }

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
