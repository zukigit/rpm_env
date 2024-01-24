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

using CustomControls;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;    /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author 孟　凡軍 2014/10/04 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.FilterEdit
{
    /// <summary>
    /// Container.xaml の相互作用ロジック
    /// </summary>
    public partial class Container : System.Windows.Controls.UserControl
    {
        #region コンストラクタ
        public Container()
        {
            // 初期化
            InitializeComponent();
            _calendarControlDAO = new CalendarControlDAO(_dbAccess);
            _calendarDetailDAO = new CalendarDetailDAO(_dbAccess);

            monthCalendar = new CustomControls.MonthCalendar();
            monthCalendar.CalendarDimensions = new System.Drawing.Size(4, 3);
            monthCalendar.Location = new System.Drawing.Point(0, 25);
            monthCalendar.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            monthCalendar.ViewStart = new DateTime(DateTime.Now.Year, 1, 1);
            monthCalendar.SelectionRanges.Clear();
            this.winForm.Child = monthCalendar;

            /* added by YAMA 2014/11/28    V2.1.0 No5対応 */
            int year = (DBUtil.GetSysTime()).Year;
            textBox_year.Text = year.ToString();
        }
        #endregion

        #region フィールド

        private CustomControls.MonthCalendar monthCalendar;

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect _dbAccess = new DBConnect(LoginSetting.ConnectStr);
        /// <summary> カレンダー管理テーブル </summary>
        private CalendarControlDAO _calendarControlDAO;
        /// <summary> カレンダー詳細テーブル </summary>
        private CalendarDetailDAO _calendarDetailDAO;

        #endregion

        #region プロパティ

        /// <summary>ウィンドウ</summary>
        ContentControl _parantWindow;
        public ContentControl ParantWindow
        {
            get
            {
                return _parantWindow;
            }
            set
            {
                _parantWindow = value;
            }
        }


        /// <summary> 選択コントローラリスト</summary>
        List<System.Windows.Controls.Control> _currentSelectedControlCollection;
        public List<System.Windows.Controls.Control> CurrentSelectedControlCollection
        {
            get
            {
                if (_currentSelectedControlCollection == null)
                    _currentSelectedControlCollection = new List<System.Windows.Controls.Control>();
                return _currentSelectedControlCollection;
            }
        }

        /// <summary>フィルターID</summary>
        private string _filterId;
        public string FilterId
        {
            get
            {
                return _filterId;
            }
            set
            {
                _filterId = value;
            }
        }

        /// <summary>更新日</summary>
        private string _updDate;
        public string UpdDate
        {
            get
            {
                return _updDate;
            }
            set
            {
                _updDate = value;
            }
        }

        /// <summary>仮更新日</summary>
        private string _tmpUpdDate;
        public string TmpUpdDate
        {
            get
            {
                return _tmpUpdDate;
            }
            set
            {
                _tmpUpdDate = value;
            }
        }

        #endregion

        #region データ格納場所

        /// <summary>カレンダー管理テーブル</summary>
        public DataTable FilterControlTable { get; set; }

        /// <summary>カレンダー稼働日テーブル</summary>
        public DataTable CalendarDetailTable { get; set; }

        /// <summary>年毎カレンダー稼働日テーブル</summary>
        public DataTable YearCalendarDetailTable { get; set; }


        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>カレンダー年右矢印ボタンクリック時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void left_arrow_click(object sender, EventArgs e)
        {
            String year = (Convert.ToInt16(textBox_year.Text) - 1).ToString();
            ViewYearCalendarDetail(year);

        }

        //*******************************************************************
        /// <summary>カレンダー年左矢印ボタンクリック時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void right_arrow_click(object sender, EventArgs e)
        {
            String year = (Convert.ToInt16(textBox_year.Text) + 1).ToString();
            ViewYearCalendarDetail(year);
        }

        //*******************************************************************
        /// <summary>基準日クリック時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.RadioButton;
            if (button == rbDesignatedDay){
                tbDesignatedDay.IsEnabled = true;
            } else {
                tbDesignatedDay.Text = "";
                tbDesignatedDay.IsEnabled = false;
            }
            if (!refreshMonthCalendar())
                e.Handled = true;
        }
        //*******************************************************************
        /// <summary>カレンダー選択時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Calendar_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SetCalendarDetail();
            if(!refreshMonthCalendar())
                e.Handled = true;
        }
        //*******************************************************************
        /// <summary>指定日入力時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void DesignatedDay_TextChanged(object sender, TextChangedEventArgs e) {
            if(!refreshMonthCalendar())
                e.Handled = true;
        }
        //*******************************************************************
        /// <summary>移動日数変更時</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void ShiftDay_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(!refreshMonthCalendar())
                e.Handled = true;
        }
        #endregion

        #region publicメソッド
        //*******************************************************************
        /// <summary>初回、矢印遷移の場合稼働日セット</summary>
        /// <param name="year">年</param>
        //*******************************************************************
        public void SetCalendarDetail()
        {
            // ベースカレンダーIDを取得
            string baseCalendarId = Convert.ToString(cmbCalendar.SelectedItem);

            _dbAccess.CreateSqlConnect();
            DataRow[] baseCalendar = _calendarControlDAO.GetValidORMaxUpdateDateEntityById(baseCalendarId).Select();
            if (baseCalendar.Length == 1){
                DataRow targetRow = baseCalendar[0];
                CalendarDetailTable = _calendarDetailDAO.GetEntityByCalendar(baseCalendarId, Convert.ToString(targetRow["update_date"]));
            }else{
                CalendarDetailTable = _calendarDetailDAO.GetEmptyTable();
            }
            _dbAccess.CloseSqlConnect();
        }

        #endregion

        #region privateメソッド
        //*******************************************************************
        /// <summary>初回、矢印遷移の場合稼働日セット</summary>
        /// <param name="year">年</param>
        //*******************************************************************
        private void ViewYearCalendarDetail(String year)
        {
            textBox_year.Text = year;
            monthCalendar.ViewStart = new DateTime(GetCurrentYear(), 1, 1);
            refreshMonthCalendar();
        }

        //*******************************************************************
        /// <summary>カレンダー選択日付をセット</summary>
        /// <param name="date">カレンダー稼動日</param>
        //*******************************************************************
        private void SetSelectDate(DateTime date)
        {
            SelectionRange selectionRange = new SelectionRange(date, date);
            monthCalendar.SelectionRanges.Add(selectionRange);
        }

        //*******************************************************************
        /// <summary>稼働日かどうかチェック</summary>
        //*******************************************************************
        private bool isOperatingDay(decimal theDay){
            string condition = "operating_date = " + theDay;
            DataRow[] rows = CalendarDetailTable.Select(condition);
            if (rows.Length > 0){
                return true;
            }
            return false;

        }

        //*******************************************************************
        /// <summary>カレンダー内容変更</summary>
        //*******************************************************************
        private bool refreshMonthCalendar()
        {
            int baseYear = GetCurrentYear();
            monthCalendar.Enabled = true;
            // 初期表示時
            if (CalendarDetailTable == null){
                monthCalendar.Enabled = false;
                return false;
            }
            monthCalendar.SelectionRanges.Clear();
            if (tbDesignatedDay.IsEnabled == true && tbDesignatedDay.Text == ""){
                monthCalendar.Enabled = false;
                return false;
            }

            decimal[] targetDate = new decimal[14];
            try {
                int shiftDay = Convert.ToInt16(cmbShiftDay.SelectedItem);
                int month = -1;
                int year  = -1;
                for (int i = 0; i < targetDate.Length; i++) {
                    if (i == 0){
                        year = baseYear - 1;
                        month = 12;
                    } else if (i == (targetDate.Length -1)){
                        year = baseYear + 1;
                        month = 1;
                    } else {
                        year = baseYear;
                        month = i;
                    }
                    if (rbFirstDay.IsChecked == true) {
                        targetDate[i] = ConvertUtil.ConverDate2IntYYYYMMDD(new DateTime (year, month, 1));
                    } else if (rbLastDay.IsChecked == true){
                        targetDate[i] = ConvertUtil.ConverDate2IntYYYYMMDD(new DateTime (year, month, DateTime.DaysInMonth(year, month)));
                    } else {
                        int designatedDay = Convert.ToInt16(tbDesignatedDay.Text);
                        if (tbDesignatedDay.Text != null) {
                            StringBuilder yyyymmdd = new StringBuilder();
                            yyyymmdd.Append(year.ToString("D4"));
                            yyyymmdd.Append(month.ToString("D2"));
                            yyyymmdd.Append(designatedDay.ToString("D2"));
                            targetDate[i] = Convert.ToDecimal(yyyymmdd.ToString());
                        }
                    }
                    if (isOperatingDay(targetDate[i])){
                        // 稼働日移動しない
                        SetSelectDate(ConvertUtil.ConverIntYYYYMMDD2Date(targetDate[i]));
                    }else{
                        // 非稼働日

                        /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
                        string stDate = Regex.Replace(targetDate[i].ToString(), @"(\d{4})(\d{2})(\d{2})", @"$1/$2/$3");
                        DateTime dtDate = DateTime.Parse(stDate);

                        if (shiftDay > 0){

                            /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
                            // string condition = "operating_date > " + targetDate[i];
                            dtDate = dtDate.AddDays(29);    // 29日加算する
                            decimal afterDate = Convert.ToDecimal(dtDate.ToString("yyyyMMdd"));
                            string condition = "operating_date >= " + targetDate[i] + " and operating_date <= " + afterDate;

                            string sortExpression = "operating_date asc";
                            DataRow[] rows = CalendarDetailTable.Select(condition, sortExpression);

                            /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
                            // if (rows.Length > 0){
                            if ((0 < rows.Length) && (Math.Abs(shiftDay) - 1 < rows.Length))
                            {

                                DateTime shifted = ConvertUtil.ConverIntYYYYMMDD2Date(Convert.ToDecimal(rows[shiftDay - 1]["operating_date"]));
                                SetSelectDate(shifted);
                            }
                        } else if (shiftDay < 0){

                            /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
                            //string condition = "operating_date < " + targetDate[i];
                            dtDate = dtDate.AddDays(-29);    // 29日減算する
                            decimal beforeDate = Convert.ToDecimal(dtDate.ToString("yyyyMMdd"));
                            string condition = "operating_date <= " + targetDate[i] + " and operating_date >= " + beforeDate;

                            string sortExpression = "operating_date desc";
                            DataRow[] rows = CalendarDetailTable.Select(condition, sortExpression);

                            /* added by YAMA 2014/12/05    V2.1.0 No28 対応 */
                            //if (rows.Length > 0){
                            if ((0 < rows.Length) && (Math.Abs(shiftDay) - 1 < rows.Length))
                            {

                                DateTime shifted = ConvertUtil.ConverIntYYYYMMDD2Date(Convert.ToDecimal(rows[-1 - shiftDay]["operating_date"]));
                                SetSelectDate(shifted);
                            }
                        } else {
                            string condition = "operating_date = " + targetDate[i];
                            DataRow[] rows = CalendarDetailTable.Select(condition);
                            if (rows.Length > 0){
                                DateTime shifted = ConvertUtil.ConverIntYYYYMMDD2Date(Convert.ToDecimal(rows[0]["operating_date"]));
                                SetSelectDate(shifted);
                            }
                        }
                    }
                }
            }
            catch (FormatException e) {
                monthCalendar.Enabled = false;
                return false;
            }
            catch (Exception e) {
                //System.Windows.MessageBox.Show(e.ToString());
                monthCalendar.Enabled = false;
                return false;
            }
            monthCalendar.Enabled = false;
            return true;
        }
        //*******************************************************************
        /// <summary>カレンダー選択日付をセット</summary>
        /// <return>現在表示年</return>
        //*******************************************************************
        private int GetCurrentYear()
        {
            return Convert.ToInt16(textBox_year.Text);
        }
        #endregion
    }
}
