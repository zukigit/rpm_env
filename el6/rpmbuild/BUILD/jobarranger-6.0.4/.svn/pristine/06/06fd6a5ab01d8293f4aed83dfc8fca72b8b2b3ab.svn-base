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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using jp.co.ftf.jobcontroller.Common;
// added by YAMA 2014/10/20    マネージャ内部時刻同期
using jp.co.ftf.jobcontroller.DAO;
using System.Text;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/14 新規作成<BR>                            *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
/// <summary>
/// Container.xaml の相互作用ロジック
/// </summary>
    public partial class CalendarContainer : System.Windows.Controls.UserControl
    {
        #region フィールド

        private CustomControls.MonthCalendar monthCalendar;

        #endregion

        #region コンストラクタ
        public CalendarContainer()
        {
            // 初期化
            InitializeComponent();
            monthCalendar = new CustomControls.MonthCalendar();
            monthCalendar.CalendarDimensions = new System.Drawing.Size(4, 3);
            monthCalendar.Location = new System.Drawing.Point(0, 25);
            monthCalendar.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            // added by YAMA 2014/10/20    マネージャ内部時刻同期
            //monthCalendar.ViewStart = new DateTime(DateTime.Now.Year, 1, 1);
            monthCalendar.ViewStart = new DateTime((DBUtil.GetSysTime()).Year, 1, 1);
            this.winForm.Child = monthCalendar;
        }
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

        /// <summary>カレンダーID</summary>
        private string _calendarId;
        public string CalendarId
        {
            get
            {
                return _calendarId;
            }
            set
            {
                _calendarId = value;
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
        public DataTable CalendarControlTable { get; set; }

        /// <summary>フィルター稼働日テーブル</summary>
        public DataTable FilterControlTable { get; set; }

        /// <summary>カレンダー稼働日テーブル</summary>
        public DataTable CalendarDetailTable { get; set; }

        /// <summary>年毎カレンダー稼働日テーブル</summary>
        public DataTable YearCalendarDetailTable { get; set; }


        #endregion

        #region イベント
        private void left_arrow_click(object sender, EventArgs e)
        {
            //YearCalendarDetailTable.Clear();
            String year = (Convert.ToInt16(textBox_year.Text) - 1).ToString();
            textBox_year.Text = year;
            monthCalendar.Enabled = true;
            monthCalendar.ViewStart = new DateTime(GetCurrentYear(), 1, 1);
            monthCalendar.Enabled = false;

        }

        private void right_arrow_click(object sender, EventArgs e)
        {
            //YearCalendarDetailTable.Clear();
            String year = (Convert.ToInt16(textBox_year.Text) + 1).ToString();
            textBox_year.Text = year;
            monthCalendar.Enabled = true;
            monthCalendar.ViewStart = new DateTime(GetCurrentYear(), 1, 1);
            monthCalendar.Enabled = false;

        }


        #endregion

        #region publicメッソド
        //*******************************************************************
        /// <summary>初回、矢印遷移の場合稼働日セット</summary>
        /// <param name="year">年</param>
        //*******************************************************************
        public void SetYearCalendarDetail(String year)
        {
            if (year != null)
            {
                textBox_year.Text = year;
            }
            else
            {
                year = textBox_year.Text;
            }
            monthCalendar.Enabled = true;
            monthCalendar.SelectionRanges.Clear();
            monthCalendar.ViewStart = new DateTime(GetCurrentYear(), 1, 1);
            DataRow[] rows = CalendarDetailTable.Select();
            SetSelectedDates(rows);
            DataRow[] maxRows = CalendarDetailTable.Select("operating_date = MAX(operating_date)");
            last_operation_day_value.Text = "";
            if (maxRows.Length > 0)
            {
                last_operation_day_value.Text = ConvertUtil.ConverIntYYYYMMDD2Date((Int32)maxRows[0]["operating_date"]).ToShortDateString();
            }
            monthCalendar.Enabled = false;

        }

        //*******************************************************************
        /// <summary>フィルター選択時のカレンダー表示</summary>
        //*******************************************************************
        public void SetFilterCalendarDetail()
        {
            int baseYear = GetCurrentYear();
            monthCalendar.Enabled = true;
            monthCalendar.SelectionRanges.Clear();
            monthCalendar.ViewStart = new DateTime(baseYear, 1, 1);
            DataRow row = FilterControlTable.Rows[0];
            if (row == null){
                monthCalendar.Enabled = false;
                return;
            }
            int baseDate = Convert.ToInt16(row["base_date_flag"]);
            int shiftDay = Convert.ToInt16(row["shift_day"]);
            decimal [] targetDate = new decimal[14];
            try {
                int month = -1;
                int year  = -1;
                for (int i = 0; i < targetDate.Length; i++) {
                    if (i == 0){
                        year = baseYear - 1;
                        month = 12;
                    }else if (i == (targetDate.Length -1)){
                        year = baseYear + 1;
                        month = 1;
                    }else {
                        year = baseYear;
                        month = i;
                    }
                    if (baseDate == 0) {
                        targetDate[i] = ConvertUtil.ConverDate2IntYYYYMMDD(new DateTime (year, month, 1));
                    } else if (baseDate == 1){
                        targetDate[i] = ConvertUtil.ConverDate2IntYYYYMMDD(new DateTime (year, month, DateTime.DaysInMonth(year, month)));
                    } else if (baseDate == 2){
                        int designatedDay = Convert.ToInt16(row["designated_day"]);
                        StringBuilder yyyymmdd = new StringBuilder();
                        yyyymmdd.Append(year.ToString("D4"));
                        yyyymmdd.Append(month.ToString("D2"));
                        yyyymmdd.Append(designatedDay.ToString("D2"));
                        targetDate[i] = Convert.ToDecimal(yyyymmdd.ToString());
                    } else {
                        monthCalendar.Enabled = false;
                        return;
                    }
                    if (isOperatingDay(targetDate[i])){
                        // 稼働日移動しない
                        SetSelectDate(ConvertUtil.ConverIntYYYYMMDD2Date(targetDate[i]));
                    }else{
                        // 非稼働日
                        if (shiftDay > 0){
                            string condition = "operating_date > " + targetDate[i];
                            string sortExpression = "operating_date asc";
                            DataRow[] rows = CalendarDetailTable.Select(condition, sortExpression);
                            if (rows.Length > 0){
                                DateTime shifted = ConvertUtil.ConverIntYYYYMMDD2Date(Convert.ToDecimal(rows[shiftDay - 1]["operating_date"]));
                                SetSelectDate(shifted);
                            }
                        } else if (shiftDay < 0){
                            string condition = "operating_date < " + targetDate[i];
                            string sortExpression = "operating_date desc";
                            DataRow[] rows = CalendarDetailTable.Select(condition, sortExpression);
                            if (rows.Length > 0){
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
            catch (Exception) {
                //log error msg
                monthCalendar.Enabled = false;
                return;
            }
            monthCalendar.Enabled = false;
        }
        #endregion

        #region privateメッソド

        //*******************************************************************
        /// <summary>稼働日かどうかチェック</summary>
        //*******************************************************************
        private bool isOperatingDay(decimal theDay){
            string condition = "operating_date = " +  theDay;
            DataRow[] rows = CalendarDetailTable.Select(condition);
            if (rows.Length > 0){
                return true;
            }
            return false;

        }

        //*******************************************************************
        /// <summary>カレンダー選択日付をセット</summary>
        /// <param name="rows">カレンダー詳細データ</param>
        //*******************************************************************
        private void SetSelectedDates(DataRow[] rows)
        {
            foreach (DataRow row in rows)
            {
                DateTime date = ConvertUtil.ConverIntYYYYMMDD2Date(Convert.ToDecimal(row["operating_date"]));
                SetSelectDate(date);
            }
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
        /// <summary>カレンダー選択日付をセット</summary>
        /// <return>現在表示年</return>
        //*******************************************************************
        private int GetCurrentYear()
        {
            return Convert.ToInt32(textBox_year.Text);
        }

        #endregion

        //added by YAMA 2014/04/10
        private void rbStartTime_Checked(object sender, RoutedEventArgs e)
        {
            //rbStartTime.IsEnabled = true;
            textBox_StartTime.IsEnabled = true;

            textBox_CyclePeriodFrom.IsEnabled = false;
            textBox_CyclePeriodTo.IsEnabled = false;
            textBox_CycleInterval.IsEnabled = false;
        }

        //added by YAMA 2014/04/10
        private void rbCycleStart_Checked(object sender, RoutedEventArgs e)
        {
            //rbCycleStart.IsEnabled = true;
            textBox_StartTime.IsEnabled = false;

            textBox_CyclePeriodFrom.IsEnabled = true;
            textBox_CyclePeriodTo.IsEnabled = true;
            textBox_CycleInterval.IsEnabled = true;

        }

    }

}
