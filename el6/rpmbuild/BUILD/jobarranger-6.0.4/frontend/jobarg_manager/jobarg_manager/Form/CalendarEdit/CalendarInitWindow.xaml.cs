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
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.JobController.Properties;

namespace jp.co.ftf.jobcontroller.JobController.Form.CalendarEdit
{
    /// <summary>
    /// CalendarInitWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CalendarInitWindow : BaseWindow
    {
        #region フィールド
        private CalendarEdit _calendarEdit;
        private String _targetYear;
        #endregion

        #region コンストラクタ
        public CalendarInitWindow(CalendarEdit calendarEdit, String targetYear)
        {
            InitializeComponent();
            _calendarEdit = calendarEdit;
            _targetYear = targetYear;

            DataContext = this;
        }
        #endregion

        #region プロパティ

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "CalendarInitWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_212;
            }
        }
        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>キャンセルボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //*******************************************************************
        /// <summary>登録ボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void register_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("register_Click", Consts.PROCESS_001);

            List<DateTime> selectedDates = new List<DateTime>();
            DateTime firstDate = new DateTime(Convert.ToInt32(_targetYear), 1, 1);
            DateTime lastDate = new DateTime(Convert.ToInt32(_targetYear), 12, 31);
            for (DateTime d = firstDate; d.CompareTo(lastDate) <= 0; d = d.AddDays(1))
            {
                if ((bool)checkBox_sun.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_mon.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_tue.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_wed.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_thu.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Thursday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_fri.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Friday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_sat.IsChecked)
                {
                    if (d.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_first.IsChecked)
                {
                    if (d.Day == 1)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }

                if ((bool)checkBox_last.IsChecked)
                {
                    if (d.Day == DateTime.DaysInMonth(d.Year, d.Month))
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }
                foreach (int selectedDay in listBox1.SelectedItems)
                {
                    if (d.Day == selectedDay)
                    {
                        if (!selectedDates.Contains(d)) selectedDates.Add(d);
                    }
                }
            }
            //ControlObjectInfo objectInfo = jobArrangerWindow.PART_Transition.currentPage.pageInfo.ObjectInfo;
            if ((bool)checkBox_init.IsChecked)
            {

                foreach (DateTime operationDate in selectedDates)
                {
                    DataRow[] rows = _calendarEdit.container.CalendarDetailTable.Select("operating_date=" + ConvertUtil.ConverDate2IntYYYYMMDD(operationDate));
                    if(rows.Count() < 1)
                    {
                        DataRow row = _calendarEdit.container.CalendarDetailTable.NewRow();
                        _calendarEdit.container.CalendarDetailTable.Rows.Add(row);

                        row["calendar_id"] = _calendarEdit.CalendarId;
                        row["update_date"] = _calendarEdit.UpdateDate;
                        row["operating_date"] = ConvertUtil.ConverDate2IntYYYYMMDD(operationDate);
                    }
                }
                _calendarEdit.container.SetYearCalendarDetail(null);
                Hide();
            }
            else
            {
                String from = _targetYear + "0101";
                String to = _targetYear + "1231";
                DataRow[] rows = _calendarEdit.container.CalendarDetailTable.Select("operating_date>='" + from + "' and operating_date<='" + to + "'");
                foreach (DataRow row in rows)
                    row.Delete();


                foreach (DateTime operationDate in selectedDates)
                {
                    DataRow row = _calendarEdit.container.CalendarDetailTable.NewRow();
                    _calendarEdit.container.CalendarDetailTable.Rows.Add(row);

                    row["calendar_id"] = _calendarEdit.CalendarId;
                    row["update_date"] = _calendarEdit.UpdateDate;
                    row["operating_date"] = ConvertUtil.ConverDate2IntYYYYMMDD(operationDate);
                }
                _calendarEdit.container.SetYearCalendarDetail(null);
                Close();

                // 終了ログ
                base.WriteEndLog("register_Click", Consts.PROCESS_001);
            }
        }
        #endregion
    }
}
