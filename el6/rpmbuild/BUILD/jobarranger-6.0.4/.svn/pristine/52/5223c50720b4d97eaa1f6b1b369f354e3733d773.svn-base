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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Controls.Primitives;
using jp.co.ftf.jobcontroller;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;
using System.Windows.Threading;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobResult
{
    /// <summary>
    /// JobnetExecResultPage..xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecResultPage : BaseUserControl
    {
        #region フィールド
        private String from;
        private String to;
        private String manageId;
        private Regex regexJobnetId;
        private Regex regexJobId;
        private Regex regexUserName;
        private DBConnect _db = new DBConnect(LoginSetting.ConnectStr);
        private RunLogDAO runLogDAO;
        private UsersDAO userDAO;
        private DataTable resultDt;
        private DataTable viewDt;
        /// <summary>テキストボックス半角制御</summary>
        private HankakuTextChangeEvent HankakuTextChangeEvent = new HankakuTextChangeEvent();

        #endregion

        #region コンストラクタ
        public JobnetExecResultPage(JobArrangerWindow parent)
        {
            InitializeComponent();
            tbxJobnetId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            tbxJobId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            tbxManageId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);

            runLogDAO = new RunLogDAO(_db);
            userDAO = new UsersDAO(_db);
            InitSearchData();
            HankakuTextChangeEvent.AddTextChangedEventHander(tbxJobnetId);
            HankakuTextChangeEvent.AddTextChangedEventHander(tbxJobId);
        }
        #endregion

        #region プロパティ
        private JobArrangerWindow _parent;
        public JobArrangerWindow Parent
        {
            get
            {
                return _parent;
            }
        }

        public override string ClassName
        {
            get
            {
                return "JobnetExecResultPage";
            }
        }

        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_500;
            }
        }

        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>検索ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void search_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("search_Click", Consts.PROCESS_024);

            // 入力チェック
            if (!SearchItemCheck())
            {
                return;
            }
            viewDt.Clear();
            //検索データ作成
            CreateSearchItem();
            //検索
            _db.CreateSqlConnect();

            if (LoginSetting.Authority.Equals(Consts.AuthorityEnum.SUPER))
            {
                resultDt = runLogDAO.GetEntitySuper(manageId, from, to, LoginSetting.Lang);
            }
            else
            {
                resultDt = runLogDAO.GetEntity(manageId, from, to, LoginSetting.UserID, LoginSetting.Lang);
            }
            _db.CloseSqlConnect();

            Filter(resultDt);

            if (resultDt.Rows.Count > 0)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadNumberDelegate(LoadNumber), 0);

            }
            else
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_024);
                return;
            }
            dgResult.ItemsSource = viewDt.DefaultView;

            // 終了ログ
            base.WriteEndLog("search_Click", Consts.PROCESS_024);
        }

        // Declare a delegate to wrap the LoadNumber method
        private delegate void LoadNumberDelegate(int number);
        private void LoadNumber(int number)
        {
            // Add the number to the observable collection
            // bound to the ListBox
            if (number < resultDt.Rows.Count)
            {
                DataRow row = viewDt.NewRow();
                row.ItemArray = resultDt.Rows[number].ItemArray;
                viewDt.Rows.Add(row);

                // Load the next number, by executing this method
                // recursively on the dispatcher queue, with
                // a priority of Background.
                //
                this.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new LoadNumberDelegate(LoadNumber), ++number);
            }
        }


        //*******************************************************************
        /// <summary>CSVタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void csv_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("csv_Click", Consts.PROCESS_025);
            if (resultDt == null || resultDt.Rows.Count < 1)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_021);
                return;
            }
            CsvWindow CsvWindow = new CsvWindow(resultDt);
            CsvWindow.Owner = this.Parent;
            CsvWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            CsvWindow.ShowDialog();
            // 終了ログ
            base.WriteEndLog("csv_Click", Consts.PROCESS_025);
        }


        //*******************************************************************
        /// <summary>カレンダーIDを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ComboBox_Loaded(object sender, EventArgs e)
        {
            var obj = (ComboBox)sender;
            if (obj != null)
            {
                var myTextBox = (TextBox)obj.Template.FindName("PART_EditableTextBox",obj);
                if (myTextBox != null)
                {
                    myTextBox.MaxLength = 2;
                }
            }
        }
        //*******************************************************************
        /// <summary>カレンダーIDを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            TextBox textBox = (TextBox)combo.Template.FindName("PART_EditableTextBox", combo);

            Int32 selectionStart = textBox.SelectionStart;
            Int32 selectionLength = textBox.SelectionLength;
            String newText = String.Empty;
            int count = 0;
            foreach (Char c in textBox.Text.ToCharArray())
            {
                if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && count == 0))
                {
                    newText += c;
                    if (c == '.')
                        count += 1;
                }
            }
            textBox.Text = newText;
            newText = String.Empty;
            for (int i = 0; i < textBox.Text.Length; i++)
            {
                String str = textBox.Text.Substring(i, 1);
                if (CheckUtil.EncSJis.GetByteCount(str) == 1)
                {
                    newText += str;
                }
            }


            textBox.Text = newText;
            textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
        }

        //*******************************************************************
        /// <summary>カレンダーIDを変える</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void dgResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (DataGridCellInfo cellInfo in dgResult.SelectedCells)
            {
                // this changes the cell's content not the data item behind it
                DataGridCell gridCell = TryToFindGridCell(dgResult, cellInfo);


                if (gridCell !=null && gridCell.Column.DisplayIndex == 0)
                {
                    gridCell.IsSelected = false;
                }
            }
        }

        //*******************************************************************
        /// <summary>詳細ボタンをクリック</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //*******************************************************************
        private void Detail_Click(object sender, RoutedEventArgs e)
        {
            DataRowView rowView = (DataRowView)((Button)e.Source).DataContext;
            DataRow row = rowView.Row;
            JobResultDetail detail = new JobResultDetail(row);
            detail.ShowDialog();
        }
        #endregion

        #region publicメソッド

        #endregion

        #region privateメソッド
        //*******************************************************************
        /// <summary>検索項目の初期セット</summary>
        //*******************************************************************
        private void InitSearchData()
        {
            // added by YAMA 2014/10/20    マネージャ内部時刻同期
            //DateTime now = System.DateTime.Now;
            DateTime now = DBUtil.GetSysTime();

            tbxFromYear.Text = now.Year.ToString();
            tbxToYear.Text = now.Year.ToString();

            // 情報種別
            Dictionary<string, string> dicMonth = new Dictionary<string, string>();
            dicMonth.Add("", "");
            for (int i = 1; i <= 12; i++)
            {
                dicMonth.Add(i.ToString(), i.ToString());
            }
            combFromMonth.Items.Clear();
            combFromMonth.ItemsSource = dicMonth;
            combFromMonth.DisplayMemberPath = "Value";
            combFromMonth.SelectedValuePath = "Key";
            combFromMonth.SelectedValue = now.Month.ToString();

            combToMonth.Items.Clear();
            combToMonth.ItemsSource = dicMonth;
            combToMonth.DisplayMemberPath = "Value";
            combToMonth.SelectedValuePath = "Key";
            combToMonth.SelectedValue = now.Month.ToString();

            Dictionary<string, string> dicDay = new Dictionary<string, string>();
            dicDay.Add("", "");
            for (int i = 1; i <= 31; i++)
            {
                dicDay.Add(i.ToString(), i.ToString());
            }

            combFromDay.Items.Clear();
            combFromDay.ItemsSource = dicDay;
            combFromDay.DisplayMemberPath = "Value";
            combFromDay.SelectedValuePath = "Key";
            combFromDay.SelectedValue = now.Day.ToString();

            combToDay.Items.Clear();
            combToDay.ItemsSource = dicDay;
            combToDay.DisplayMemberPath = "Value";
            combToDay.SelectedValuePath = "Key";
            combToDay.SelectedValue = now.Day.ToString();

            Dictionary<string, string> dicHour = new Dictionary<string, string>();
            dicHour.Add("", "");
            for (int i = 0; i <= 23; i++)
            {
                dicHour.Add(GetStringFromInt(i), GetStringFromInt(i));
            }
            combFromHour.Items.Clear();
            combFromHour.ItemsSource = dicHour;
            combFromHour.DisplayMemberPath = "Value";
            combFromHour.SelectedValuePath = "Key";
            combFromHour.SelectedValue = "00";

            combToHour.Items.Clear();
            combToHour.ItemsSource = dicHour;
            combToHour.DisplayMemberPath = "Value";
            combToHour.SelectedValuePath = "Key";
            combToHour.SelectedValue = "23";

            Dictionary<string, string> dicMin = new Dictionary<string, string>();
            dicMin.Add("", "");
            for (int i = 0; i <= 59; i++)
            {
                dicMin.Add(GetStringFromInt(i), GetStringFromInt(i));
            }
            combFromMin.Items.Clear();
            combFromMin.ItemsSource = dicMin;
            combFromMin.DisplayMemberPath = "Value";
            combFromMin.SelectedValuePath = "Key";
            combFromMin.SelectedValue = "00";

            combToMin.Items.Clear();
            combToMin.ItemsSource = dicMin;
            combToMin.DisplayMemberPath = "Value";
            combToMin.SelectedValuePath = "Key";
            combToMin.SelectedValue = "59";

            Dictionary<string, string> dicUser = new Dictionary<string, string>();
            //ユーザー検索
            _db.CreateSqlConnect();
            DataTable dt = userDAO.GetAllEntity();

            dicUser.Add("", "");
            foreach (DataRow row in dt.Rows)
            {
                dicUser.Add(row["username"].ToString(), row["username"].ToString());
            }
            combUserName.Items.Clear();
            combUserName.ItemsSource = dicUser;
            combUserName.DisplayMemberPath = "Value";
            combUserName.SelectedValuePath = "Key";
            combUserName.SelectedIndex = 0;
            viewDt = runLogDAO.GetEmptyTable();
            _db.CloseSqlConnect();


        }

        //*******************************************************************
        /// <summary>数値に前ゼロセット</summary>
        //*******************************************************************
        private String GetStringFromInt(int i)
        {
            String str = i.ToString();
            if (i < 10)
                str = "0" + str;
            return str;

        }

        //*******************************************************************
        /// <summary>検索項目チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool SearchItemCheck()
        {
            // 検索管理IDを取得
            string manageID = tbxManageId.Text;
            if (CheckUtil.IsNullOrEmpty(manageID))
            {
                // 検索From年を取得
                string fromYear = tbxFromYear.Text;
                // 入力の場合
                if (CheckUtil.IsNullOrEmpty(fromYear))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { Properties.Resources.err_message_search_from_year });
                    return false;
                }

                // 半角数値かチェック
                if (!CheckUtil.IsHankakuNum(fromYear))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                        new string[] { Properties.Resources.err_message_search_from_year });
                    return false;
                }

                // 4桁かチェック
                if (!CheckUtil.IsLen(fromYear,4))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_012,
                        new string[] { Properties.Resources.err_message_search_from_year, "4" });
                    return false;
                }

                // 検索From月を取得
                string fromMonth = combFromMonth.Text;
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(fromMonth))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { Properties.Resources.err_message_search_from_month });
                    return false;
                }

                // 半角数値かチェック
                if (!CheckUtil.IsHankakuNum(fromMonth))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                        new string[] { Properties.Resources.err_message_search_from_month, "1", "12" });
                    return false;
                }

                // 月入力が正しいかチェック
                if (!CheckUtil.IsMonth(Convert.ToInt16(fromMonth)))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                        new string[] { Properties.Resources.err_message_search_from_month,"1","12"});
                    return false;
                }

                // 検索From日を取得
                string fromDay = combFromDay.Text;
                // 未入力の場合
                if (CheckUtil.IsNullOrEmpty(fromDay))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { Properties.Resources.err_message_search_from_day });
                    return false;
                }

                // 半角数値かチェック
                if (!CheckUtil.IsHankakuNum(fromDay))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                        new string[] { Properties.Resources.err_message_search_from_day, "1", "31" });
                    return false;
                }

                // 日入力が正しいかチェック
                if (!CheckUtil.IsDay(Convert.ToInt16(fromDay)))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                        new string[] { Properties.Resources.err_message_search_from_day, "1", "31" });
                    return false;
                }

                // 検索From時を取得
                string fromHour = combFromHour.Text;
                // 検索From分を取得
                string fromMin = combFromMin.Text;
                if (CheckUtil.IsNullOrEmpty(fromHour) != CheckUtil.IsNullOrEmpty(fromMin))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_026,
                        new string[] { Properties.Resources.err_message_search_from_time });
                    return false;
                }
                // 入力の場合
                if (!CheckUtil.IsNullOrEmpty(fromHour))
                {
                    // 半角数値かチェック
                    if (!CheckUtil.IsHankakuNum(fromHour))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_from_hour, "0", "23" });
                        return false;
                    }

                    // 時入力が正しいかチェック
                    if (!CheckUtil.IsHour(Convert.ToInt16(fromHour)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_from_hour, "0", "23" });
                        return false;
                    }
                }


                // 入力の場合
                if (!CheckUtil.IsNullOrEmpty(fromMin))
                {
                    // 半角数値かチェック
                    if (!CheckUtil.IsHankakuNum(fromMin))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_from_min, "0", "59" });
                        return false;
                    }
                    // 分入力が正しいかチェック
                    if (!CheckUtil.IsMin(Convert.ToInt16(fromMin)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_from_min, "0", "59" });
                        return false;
                    }
                }
                //日付チェック
                if(Convert.ToInt16(fromDay)>DateTime.DaysInMonth(Convert.ToInt16(fromYear), Convert.ToInt16(fromMonth)))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_018,
                        new string[] { Properties.Resources.err_message_search_from_date });
                    return false;
                }
                // 検索To年、月、日、時、分を取得
                string toYear = tbxToYear.Text;
                string toMonth = combToMonth.Text;
                string toDay = combToDay.Text;
                string toHour = combToHour.Text;
                string toMin = combToMin.Text;
                if (!(CheckUtil.IsNullOrEmpty(toYear) &&
                    CheckUtil.IsNullOrEmpty(toMonth) &&
                    CheckUtil.IsNullOrEmpty(toDay) &&
                    CheckUtil.IsNullOrEmpty(toHour) &&
                    CheckUtil.IsNullOrEmpty(toMin)))
                {
                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(toYear))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { Properties.Resources.err_message_search_to_year });
                        return false;
                    }

                    // 半角数値かチェック
                    if (!CheckUtil.IsHankakuNum(toYear))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                            new string[] { Properties.Resources.err_message_search_to_year });
                        return false;
                    }

                    // 4桁かチェック
                    if (!CheckUtil.IsLen(toYear, 4))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_012,
                            new string[] { Properties.Resources.err_message_search_to_year, "4"});
                        return false;
                    }

                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(toMonth))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { Properties.Resources.err_message_search_to_month });
                        return false;
                    }

                    // 半角数値かチェック
                    if (!CheckUtil.IsHankakuNum(toMonth))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_to_month, "1", "12" });
                        return false;
                    }
                    // 月入力が正しいかチェック
                    if (!CheckUtil.IsMonth(Convert.ToInt16(toMonth)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_to_month, "1", "12" });
                        return false;
                    }

                    // 未入力の場合
                    if (CheckUtil.IsNullOrEmpty(toDay))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                            new string[] { Properties.Resources.err_message_search_to_day });
                        return false;
                    }

                    // 半角数値かチェック
                    if (!CheckUtil.IsHankakuNum(toDay))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_to_day, "1", "31" });
                        return false;
                    }
                    // 日入力が正しいかチェック
                    if (!CheckUtil.IsDay(Convert.ToInt16(toDay)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                            new string[] { Properties.Resources.err_message_search_to_day, "1", "31" });
                        return false;
                    }

                    if (CheckUtil.IsNullOrEmpty(toHour) != CheckUtil.IsNullOrEmpty(toMin))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_026,
                            new string[] { Properties.Resources.err_message_search_to_time});
                        return false;
                    }

                    // 入力の場合
                    if (!CheckUtil.IsNullOrEmpty(toHour))
                    {
                        // 半角数値かチェック
                        if (!CheckUtil.IsHankakuNum(toHour))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                new string[] { Properties.Resources.err_message_search_to_hour, "0", "23" });
                            return false;
                        }

                        // 時入力が正しいかチェック
                        if (!CheckUtil.IsHour(Convert.ToInt16(toHour)))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                new string[] { Properties.Resources.err_message_search_to_hour, "0", "23" });
                            return false;
                        }
                    }

                    // 入力の場合
                    if (!CheckUtil.IsNullOrEmpty(toMin))
                    {
                        // 半角数値かチェック
                        if (!CheckUtil.IsHankakuNum(toMin))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                new string[] { Properties.Resources.err_message_search_to_min, "0", "59" });
                            return false;
                        }
                        // 分入力が正しいかチェック
                        if (!CheckUtil.IsMin(Convert.ToInt16(toMin)))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                new string[] { Properties.Resources.err_message_search_to_min, "0", "59" });
                            return false;
                        }
                    }
                    //日付チェック
                    if (Convert.ToInt16(toDay) > DateTime.DaysInMonth(Convert.ToInt16(toYear), Convert.ToInt16(toMonth)))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_018,
                            new string[] { Properties.Resources.err_message_search_to_date });
                        return false;
                    }
                }
            }

            try
            {
                if (CheckUtil.IsNullOrEmpty(tbxJobnetId.Text))
                {
                    regexJobnetId = null;
                }
                else
                {
                    regexJobnetId = new Regex("^" + tbxJobnetId.Text + "$");
                }

                if (CheckUtil.IsNullOrEmpty(tbxJobId.Text))
                {
                    regexJobId = null;
                }
                else
                {
                    regexJobId = new Regex("^" + tbxJobId.Text + "$");
                }

                if (CheckUtil.IsNullOrEmpty(combUserName.Text))
                {
                    regexUserName = null;
                }
                else
                {
                    regexUserName = new Regex("^" + combUserName.Text + "$");
                }
            }
            catch (Exception)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_020);
                return false;
            }
            return true;
        }

        //*******************************************************************
        /// <summary>検索項目作成 </summary>
        //*******************************************************************
        private void CreateSearchItem()
        {
            if (CheckUtil.IsNullOrEmpty(tbxManageId.Text))
            {
                from = tbxFromYear.Text +
                    GetStringFromInt(Convert.ToInt16(combFromMonth.Text)) +
                    GetStringFromInt(Convert.ToInt16(combFromDay.Text));
                if (!CheckUtil.IsNullOrEmpty(combFromHour.Text))
                {
                    from = from + GetStringFromInt(Convert.ToInt16(combFromHour.Text));
                }
                else
                {
                    from = from + "00";
                }
                if (!CheckUtil.IsNullOrEmpty(combFromMin.Text))
                {
                    from = from + GetStringFromInt(Convert.ToInt16(combFromMin.Text));
                }
                else
                {
                    from = from + "00";
                }
                from = from + "00"+"000";
                if (!CheckUtil.IsNullOrEmpty(tbxToYear.Text))
                {
                    to = tbxToYear.Text +
                        GetStringFromInt(Convert.ToInt16(combToMonth.Text)) +
                        GetStringFromInt(Convert.ToInt16(combToDay.Text));
                    if (!CheckUtil.IsNullOrEmpty(combToHour.Text))
                    {
                        to = to + GetStringFromInt(Convert.ToInt16(combToHour.Text));
                    }
                    else
                    {
                        to = to + "23";
                    }
                    if (!CheckUtil.IsNullOrEmpty(combToMin.Text))
                    {
                        to = to + GetStringFromInt(Convert.ToInt16(combToMin.Text));
                    }
                    else
                    {
                        to = to + "59";
                    }
                    to = to + "59"+"999";
                }
                else
                {
                    to = null;
                }
                manageId = null;
            }
            else
            {
                manageId = tbxManageId.Text;
                from = null;
                to = null;
            }

        }

        //*******************************************************************
        /// <summary>正規表現検索項目でフィルター </summary>
        //*******************************************************************
        private void Filter(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (regexJobnetId != null)
                {
                    if (!regexJobnetId.IsMatch(row["jobnet_id"].ToString()))
                    {
                        row.Delete();
                        continue;
                    }
                }
                if (regexJobId != null)
                {
                    if (!regexJobId.IsMatch(row["job_id"].ToString()))
                    {
                        row.Delete();
                        continue;
                    }
                }
                if (regexUserName != null)
                {
                    if (!regexUserName.IsMatch(row["user_name"].ToString()))
                    {
                        row.Delete();
                        continue;
                    }
                }
            }
            dt.AcceptChanges();

        }

        static DataGridCell TryToFindGridCell(DataGrid grid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return result;
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        #endregion


    }

}
