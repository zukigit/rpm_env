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
using System.Windows;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.JobController;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.JobController.Form.CalendarEdit;
using jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
using System.Configuration;
using jp.co.ftf.jobcontroller.DAO;
using System.Data;
using System;
using System.Windows.Media;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/15 新規作成<BR>                           *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
    /// <summary>
    /// JobArrangerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScheduleCalendarRegistWindow : BaseWindow
    {
        #region フィールド

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        /// <summary> カレンダーテーブル（公開フラグ更新用） </summary>
        private DataTable _calendarTbl;

        /// <summary> フィルターテーブル（公開フラグ更新用） </summary>
        private DataTable _filterTbl;

        /// <summary> カレンダー管理テーブル </summary>
        private CalendarControlDAO _calendarControlDAO;

        /// <summary> フィルター管理テーブル </summary>
        private FilterControlDAO _filterControlDAO;

        /// <summary> カレンダー稼働日テーブル </summary>
        private CalendarDetailDAO _calendarDetailDAO;

        /// <summary> オブジェクト種別 </summary>
        private Consts.ObjectEnum _objectType;

        #endregion

        #region コンストラクタ
        public ScheduleCalendarRegistWindow(Container parantContainer)
        {
            InitializeComponent();
            LoadForInit(parantContainer);
            treeViewCalendar.IsSelected = true;

        }
        #endregion

        #region プロパティ
        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "ScheduleCalendarRegistWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_221;
            }
        }
        /// <summary>カレンダーＩＤ</summary>
        private string _objectId;
        public string ObjectId
        {
            get
            {
                return _objectId;
            }
            set
            {
                _objectId = value;
            }
        }
        /// <summary>親Container</summary>
        private Container _parantContainer;
        public Container ParentContainer
        {
            get
            {
                return _parantContainer;
            }
            set
            {
                _parantContainer = value;
            }
        }
        #endregion

        #region イベント
        /// <summary>オブジェクトを選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            _objectId = ((TreeViewItem)sender).Header.ToString();

            _objectType = Consts.ObjectEnum.CALENDAR;
            if(item.Tag != null){
                _objectType =(Consts.ObjectEnum)item.Tag;
            }else{
                return;
            }

            // 各テーブルのデータをコピー追加
            FillTables(_objectType, _objectId);
            // 情報エリアの表示
            SetInfoArea(_objectType);
            // カレンダー稼働日の表示
            ShowCalendarDetail(_objectType);
            btnRegist.IsEnabled = true;

            container.rbStartTime.IsEnabled = true;
            container.rbCycleStart.IsEnabled = true;
            if (container.rbStartTime.IsChecked == true){
                container.textBox_StartTime.IsEnabled = true;
                container.textBox_CyclePeriodFrom.IsEnabled = false;
                container.textBox_CyclePeriodTo.IsEnabled = false;
                container.textBox_CycleInterval.IsEnabled = false;
            }else{
                container.textBox_StartTime.IsEnabled = false;
                container.textBox_CyclePeriodFrom.IsEnabled = true;
                container.textBox_CyclePeriodTo.IsEnabled = true;
                container.textBox_CycleInterval.IsEnabled = true;
            }

            container.btnLeft.IsEnabled = true;
            container.btnRight.IsEnabled = true;
            e.Handled = true;

        }

        //*******************************************************************
        /// <summary>登録ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void regist_Click(object sender, RoutedEventArgs e)
        {
            int interval = 0;
            int[] stHM = new int[2];
            int[] edHM = new int[2];

            string bootTime = "";
            string startTime = "";

            string CycleInterval = "";
            string[] TmData = new string[4];


            // 開始ログ
            base.WriteStartLog("regist_Click", Consts.PROCESS_001);

            // 起動方法の判別
            if (container.rbStartTime.IsChecked == true)
            {
                // 起動時刻ラジオボタンを選択

                // 入力チェック
                if (!InputCheck(0, ref TmData))
                    return;
                // TmDataには時分に分解された起動時刻が格納されている

                // 起動時刻を取得
                bootTime = TmData[0] + TmData[1];

                if (bootTime.Length == 3) bootTime = "0" + bootTime;
                DataRow[] rows = ParentContainer.ScheduleDetailTable.Select("calendar_id='" + _objectId + "' and boot_time='" + bootTime + "'");

                if (rows.Length < 1)
                {
                    DataRow row = ParentContainer.ScheduleDetailTable.NewRow();
                    row["calendar_id"] = _objectId;
                    row["boot_time"] = bootTime;
                    row["schedule_id"] = ParentContainer.ScheduleId;
                    if(Consts.ObjectEnum.FILTER == _objectType){
                        row["object_flag"] = "1";
                        FilterControlDAO filterControlDAO = new FilterControlDAO(dbAccess);
                        DataTable dt = filterControlDAO.GetValidORMaxUpdateDateEntityById(_objectId);
                        row["object_name"] = dt.Rows[0]["filter_name"];
                    } else if(Consts.ObjectEnum.CALENDAR == _objectType) {
                        row["object_flag"] = "0";
                        CalendarControlDAO calendarControlDAO = new CalendarControlDAO(dbAccess);
                        DataTable dt = calendarControlDAO.GetValidORMaxUpdateDateEntityById(_objectId);
                        row["object_name"] = dt.Rows[0]["calendar_name"];
                    }
                    ParentContainer.ScheduleDetailTable.Rows.Add(row);

                }
            }
            else
            {
                // サイクル起動ラジオボタンを選択

                // 入力チェック
                if (!InputCheck(1, ref TmData))
                    return;
                // TmDataには時分に分解された開始時刻と終了時刻が格納されている

                // 開始時刻を取得
                stHM[0] = int.Parse(TmData[0]);
                stHM[1] = int.Parse(TmData[1]);

                // 終了時刻を取得
                edHM[0] = int.Parse(TmData[2]);
                edHM[1] = int.Parse(TmData[3]);

                // 間隔時間（分）を取得
                CycleInterval = container.textBox_CycleInterval.Text.Trim();

                interval = int.Parse(CycleInterval);

                TimeSpan t1 = new TimeSpan(stHM[0], stHM[1], 0);
                TimeSpan t2 = new TimeSpan(0, 0, 0);
                TimeSpan t3 = new TimeSpan(edHM[0], edHM[1], 0);

                // 開始時刻を取得
                startTime = TmData[0] + TmData[1];

                DataRow[] rows = ParentContainer.ScheduleDetailTable.Select("calendar_id='" + _objectId + "' and boot_time='" + startTime + "'");

                if (rows.Length < 1)
                {
                    for (; t1 <= t3; t1 = t1 + t2)
                    {
                        t2 = new TimeSpan(0, interval, 0);
                        DataRow row = ParentContainer.ScheduleDetailTable.NewRow();
                        row["calendar_id"] = _objectId;
                        row["boot_time"] = t1.ToString("hhmm");
                        row["schedule_id"] = ParentContainer.ScheduleId;
                        if(Consts.ObjectEnum.FILTER == _objectType){
                            row["object_flag"] = "1";
                            FilterControlDAO filterControlDAO = new FilterControlDAO(dbAccess);
                            DataTable dt = filterControlDAO.GetValidORMaxUpdateDateEntityById(_objectId);
                            row["object_name"] = dt.Rows[0]["filter_name"];
                        } else if(Consts.ObjectEnum.CALENDAR == _objectType) {
                            row["object_flag"] = "0";
                            CalendarControlDAO calendarControlDAO = new CalendarControlDAO(dbAccess);
                            DataTable dt = calendarControlDAO.GetValidORMaxUpdateDateEntityById(_objectId);
                            row["object_name"] = dt.Rows[0]["calendar_name"];
                        }
                        ParentContainer.ScheduleDetailTable.Rows.Add(row);
                    }
                }
            }

            this.Close();
            // 終了ログ
            base.WriteEndLog("regist_Click", Consts.PROCESS_001);
        }

        //*******************************************************************
        /// <summary> キャンセルボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("regist_Click", Consts.PROCESS_002);

            this.Close();

            // 終了ログ
            base.WriteEndLog("regist_Click", Consts.PROCESS_002);
        }

        /// <summary>公開カレンダーを選択</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Public_Calendar_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeCalendar(true);
        }


        /// <summary>プライベートカレンダーを選択</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Private_Calendar_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeCalendar(false);
        }

        /// <summary>公開フィルターを選択</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Public_Filter_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeFilter(true);
        }

        /// <summary>プライベートフィルターを選択</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Private_Filter_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeFilter(false);
        }

        #endregion

        #region  private メッソド
        //*******************************************************************
        /// <summary>初期化</summary>
        //*******************************************************************
        private void LoadForInit(Container parantContainer)
        {
            // DAOの初期化
            InitialDAO();

            // 空のテーブルを取得
            SetTables();

            //　親Containerをセット
            _parantContainer = parantContainer;

            // プロパティをセット
            container.ParantWindow = this;

            btnRegist.IsEnabled = false;
            // container.textBox_bootTimeHH.IsEnabled = false;
            // container.textBox_bootTimeMI.IsEnabled = false;

            //added by YAMA 2014/04/10
            container.rbStartTime.IsEnabled = false;
            container.rbCycleStart.IsEnabled = false;
            container.textBox_StartTime.IsEnabled = false;
            container.textBox_CyclePeriodFrom.IsEnabled = false;
            container.textBox_CyclePeriodTo.IsEnabled = false;
            container.textBox_CycleInterval.IsEnabled = false;

            container.rbStartTime.IsChecked = true;

            ShowCalendarDetail(Consts.ObjectEnum.CALENDAR);
            container.btnLeft.IsEnabled = false;
            container.btnRight.IsEnabled = false;
        }

        //*******************************************************************
        /// <summary> DAOの初期化処理</summary>
        //*******************************************************************
        private void InitialDAO()
        {
            // カレンダー管理テーブル
            _calendarControlDAO = new CalendarControlDAO(dbAccess);

            // フィルター管理テーブル
            _filterControlDAO = new FilterControlDAO(dbAccess);

            // カレンダー稼働日テーブル
            _calendarDetailDAO = new CalendarDetailDAO(dbAccess);

        }

        //*******************************************************************
        /// <summary> 空テーブルをDataTableにセット(新規追加用)</summary>
        //*******************************************************************
        private void SetTables()
        {
            // カレンダー管理テーブル
            dbAccess.CreateSqlConnect();

            // カレンダーテーブル
            container.CalendarControlTable = _calendarControlDAO.GetEmptyTable();
            // フィルターテーブル
            container.FilterControlTable = _filterControlDAO.GetEmptyTable();
            // カレンダー稼働日テーブル
            container.CalendarDetailTable = _calendarDetailDAO.GetEmptyTable();

            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> カレンダーデータの検索（編集、コピー新規用）</summary>
        /// <param name="objectId">`カレンダーID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(Consts.ObjectEnum objectType, string objectId)
        {
            switch (objectType) {
                case Consts.ObjectEnum.CALENDAR:
                    // カレンダー管理テーブル
                    container.CalendarControlTable = _calendarControlDAO.GetValidORMaxUpdateDateEntityById(objectId);

                    if (container.CalendarControlTable.Rows.Count > 0){
                        // カレンダー稼働日テーブル
                        container.CalendarDetailTable = _calendarDetailDAO.GetEntityByCalendar(
                            container.CalendarControlTable.Rows[0]["calendar_id"],
                            container.CalendarControlTable.Rows[0]["update_date"]);
                    }

                    break;
                case Consts.ObjectEnum.FILTER:
                    // フィルター管理テーブル
                    container.FilterControlTable = _filterControlDAO.GetValidORMaxUpdateDateEntityById(objectId);

                    // カレンダー管理テーブル
                    container.CalendarControlTable = _calendarControlDAO.GetValidORMaxUpdateDateEntityById(
                        container.FilterControlTable.Rows[0]["base_calendar_id"]);

                    if (container.CalendarControlTable.Rows.Count > 0){
                        // カレンダー稼働日テーブル
                        container.CalendarDetailTable = _calendarDetailDAO.GetEntityByCalendar(
                            container.CalendarControlTable.Rows[0]["calendar_id"],
                            container.CalendarControlTable.Rows[0]["update_date"]);
                    }

                    break;
            }

        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea(Consts.ObjectEnum objectType)
        {
            DataRow row;
            if (Consts.ObjectEnum.CALENDAR == objectType){
                row = container.CalendarControlTable.Select()[0];
                lblObjectId.Text = Convert.ToString(row["calendar_id"]);
                lblObjectName.Text = Convert.ToString(row["calendar_name"]);
            }else if (Consts.ObjectEnum.FILTER == objectType){
                row = container.FilterControlTable.Select()[0];
                lblObjectId.Text = Convert.ToString(row["filter_id"]);
                lblObjectName.Text = Convert.ToString(row["filter_name"]);
            }else{
                return ;
            }

            // 公開チェックボックス
            int openFlg = Convert.ToInt16(row["public_flag"]);
            if (openFlg == 0)
                lblOpen.Text = "";
            else if (openFlg == 1)
                lblOpen.Text = "○";

            // 説明
            lblComment.Text = Convert.ToString(row["memo"]);

            //更新日
            lblUpdDate.Text = (ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToInt64(row["update_date"]))).ToString("yyyy/MM/dd HH:mm:ss");
            //ユーザー名
            lblUserName.Text = Convert.ToString(row["user_name"]);
        }

        //*******************************************************************
        /// <summary>カレンダー稼働日の表示</summary>
        //*******************************************************************

        private void ShowCalendarDetail(Consts.ObjectEnum objectType)
        {
            if (Consts.ObjectEnum.CALENDAR == objectType){
                // added by YAMA 2014/10/20    マネージャ内部時刻同期
                //int year = DateTime.Now.Year;
                int year = (DBUtil.GetSysTime()).Year;
                container.SetYearCalendarDetail(year.ToString());
            }else if (Consts.ObjectEnum.FILTER == objectType){
                container.SetFilterCalendarDetail();
            }
        }

        /// <summary>カレンダーを展開</summary>
        /// <param name="public_flg">公開フラグ</param>
        public void SetTreeCalendar(bool public_flg)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicCalendar;
            }
            else
            {
                flg = 0;
                treeViewItem = privateCalendar;
            }


            string selectSql;
            if (public_flg)
            {
                selectSql = "select calendar_id, max(update_date) from ja_calendar_control_table where public_flag= " +
                                flg + " group by calendar_id order by calendar_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    selectSql = "SELECT distinct A.calendar_id,A.update_date "
                                                    + "FROM ja_calendar_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                    + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                    + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                    + "A.update_date= "
                                                    + "( SELECT MAX(B.update_date) FROM ja_calendar_control_table AS B "
                                                    + "WHERE B.calendar_id = A.calendar_id group by B.calendar_id) and A.public_flag=0 order by A.calendar_id";
                }
                else
                {
                    selectSql = "select calendar_id, max(update_date) from ja_calendar_control_table where public_flag= " +
                                    flg + " group by calendar_id order by calendar_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {
                treeViewItem.Items.Clear();
                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["calendar_id"].ToString();
                    item.Tag = Consts.ObjectEnum.CALENDAR;
                    treeViewItem.Items.Add(item);
                }
            }

            TreeViewItem itemCalendar;
            foreach (object item in treeViewItem.Items)
            {
                itemCalendar = (TreeViewItem)item;
                itemCalendar.Selected += Item_Selected;
            }

            dbaccess.CloseSqlConnect();
        }

        /// <summary>フィルターを展開</summary>
        /// <param name="public_flg">公開フラグ</param>
        public void SetTreeFilter(bool public_flg)
        {
            DataTable dataTbl;
            DBConnect dbaccess = new DBConnect(LoginSetting.ConnectStr);
            dbaccess.CreateSqlConnect();

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicFilter;
            }
            else
            {
                flg = 0;
                treeViewItem = privateFilter;
            }


            string selectSql;
            if (public_flg)
            {
                selectSql = "select filter_id, max(update_date) from ja_filter_control_table where public_flag= " +
                                flg + " group by filter_id order by filter_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    selectSql = "SELECT distinct A.filter_id,A.update_date "
                                                    + "FROM ja_filter_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                    + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                    + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                    + "A.update_date= "
                                                    + "( SELECT MAX(B.update_date) FROM ja_filter_control_table AS B "
                                                    + "WHERE B.filter_id = A.filter_id group by B.filter_id) and A.public_flag=0 order by A.filter_id";
                }
                else
                {
                    selectSql = "select filter_id, max(update_date) from ja_filter_control_table where public_flag= " +
                                    flg + " group by filter_id order by filter_id";

                }
            }

            dataTbl = dbaccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {
                treeViewItem.Items.Clear();
                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["filter_id"].ToString();
                    item.Tag = Consts.ObjectEnum.FILTER;
                    treeViewItem.Items.Add(item);
                }
            }

            TreeViewItem itemFilter;
            foreach (object item in treeViewItem.Items)
            {
                itemFilter = (TreeViewItem)item;
                itemFilter.Selected += Item_Selected;
            }

            dbaccess.CloseSqlConnect();
        }


        //added by YAMA 2014/04/10
        private int ChkTimeData(string inData, ref string[] outTimeData)
        {
            int idx = 0;
            int ret = 0;

            // 未入力チェック
            if (CheckUtil.IsNullOrEmpty(inData))
            {
                return 1;
            }

            // 半角数字・コロンチェック
            if (CheckUtil.IsHankakuStrAndColon(inData))
            {
            }
            else
            {
                return 2;
            }

            // 時分に分解
            idx = inData.IndexOf(":");
            if (idx == -1)
            {
                return 3;
            }
            else
            {
                outTimeData[0] = inData.Substring(0, idx);
                outTimeData[1] = inData.Substring(idx + 1);

                // コロンのみ入力された場合
                if (outTimeData[0].Length == 0 || outTimeData[1].Length == 0)
                {
                    return 4;
                }
            }
            return ret;

        }

        //*******************************************************************
        /// <summary>入力チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheck(int selectRbType, ref string[] outTmData)
        {
            bool retCode = true;
            int ret = 0;
            int wktime = 0;

            string bootTime = "";
            string CycleInterval = "";

            String[] errItem = new String[3];

            string[] CycleTimeDataData = new string[2];

            int[] stHM = new int[2];
            int[] edHM = new int[2];


            // 起動時刻を選択時
            if (selectRbType == 0)
            {
                // 起動時刻を取得
                bootTime = container.textBox_StartTime.Text.Trim();

                // 起動時刻のデータチェック
                ret = ChkTimeData(bootTime, ref outTmData);
                // outTmDataには時分に分解された値が格納されている

                errItem[0] = Properties.Resources.err_message_boot_time; // 起動時刻
                errItem[1] = "2"; // 2バイト、2桁の２

                switch (ret)
                {
                    // 未入力(1)
                    case 1:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, errItem);
                        retCode = false;
                        break;

                    // 半角数字・コロン以外を入力(2)
                    case 2:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_027, errItem);
                        retCode = false;
                        break;

                    // 時間の入力形式[hh:mm]誤り(3)
                    case 3:
                    case 4:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_026, errItem);
                        retCode = false;
                        break;

                }

                if (retCode == false)
                {
                    return retCode;
                }


                // 起動時刻（時）の値が「0」～「99」以外の場合、エラー
                if (CheckUtil.IsLenOver(outTmData[0], 2))
                {
                    errItem[0] = Properties.Resources.err_message_boot_time_hh; // 起動時刻の時間
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003, errItem);
                    return false;
                }

                // 起動時刻（分）の値が「00」～「59」以外の場合、エラー
                wktime = int.Parse(outTmData[1]);

                if (wktime < 0 || wktime > 59)
                {
                    errItem[0] = Properties.Resources.err_message_boot_time_mi; // 起動時刻の分
                    CommonDialog.ShowErrorDialog(Consts.ERROR_BOOT_TIME_001, errItem);
                    return false;
                }
            }
            else
            {
                // サイクル起動を選択時
                // 開始時刻を取得
                bootTime = container.textBox_CyclePeriodFrom.Text.Trim();

                // 開始時刻のデータチェック
                ret = ChkTimeData(bootTime, ref CycleTimeDataData);

                errItem[0] = Properties.Resources.err_message_boot_start_time; // 開始時刻
                errItem[1] = "2"; // 2バイト、2桁の２
                switch (ret)
                {
                    // 未入力(1)
                    case 1:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, errItem);
                        retCode = false;
                        break;

                    // 半角数字・コロン以外を入力(2)
                    case 2:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_027, errItem);
                        retCode = false;
                        break;

                    // 時間の入力形式[hh:mm]誤り(3)
                    case 3:
                    case 4:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_026, errItem);
                        retCode = false;
                        break;
                }

                if (retCode == false)
                {
                    return retCode;
                }

                // outTmDataには時分に分解された値が格納されている
                outTmData[0] = CycleTimeDataData[0];
                outTmData[1] = CycleTimeDataData[1];

                // 開始時刻（時）の値が「0」～「23」以外の場合、エラーダイアログを表示する。
                wktime = int.Parse(outTmData[0]);

                if (wktime < 0 || wktime > 23)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                                    new string[] { Properties.Resources.err_message_boot_start_time_hh, "0", "23" });
                    return false;
                }

                // 開始時刻（分）の値が「00」～「59」以外の場合、エラーダイアログを表示する。
                wktime = int.Parse(outTmData[1]);

                if (wktime < 0 || wktime > 59)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                                    new string[] { Properties.Resources.err_message_boot_start_time_mi, "0", "59" });
                    return false;
                }


                // 終了時刻を取得
                bootTime = container.textBox_CyclePeriodTo.Text.Trim();

                // 終了時刻のデータチェック
                ret = ChkTimeData(bootTime, ref CycleTimeDataData);

                errItem[0] = Properties.Resources.err_message_boot_end_time; // 終了時刻
                errItem[1] = "2"; // 2バイト、2桁の２
                switch (ret)
                {
                    // 未入力(1)
                    case 1:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, errItem);
                        retCode = false;
                        break;

                    // 半角数字・コロン以外を入力(2)
                    case 2:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_027, errItem);
                        retCode = false;
                        break;

                    // 時間の入力形式[hh:mm]誤り(3)
                    case 3:
                    case 4:
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_026, errItem);
                        retCode = false;
                        break;
                }

                if (retCode == false)
                {
                    return retCode;
                }


                // outTmDataには時分に分解された値が格納されている
                outTmData[2] = CycleTimeDataData[0];
                outTmData[3] = CycleTimeDataData[1];

                //added by YAMA 2014/06/23
                // 終了時刻（時）の値が「0」～「23」以外の場合、エラーダイアログを表示する。 <-- 廃止
                // 終了時刻（時）の値が「0」～「47」以外の場合、エラーダイアログを表示する。
                wktime = int.Parse(outTmData[2]);

                if (wktime < 0 || wktime > 47)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                                    new string[] { Properties.Resources.err_message_boot_end_time_hh, "0", "47" });
                    return false;
                }

                // 終了時刻（分）の値が「00」～「59」以外の場合、エラーダイアログを表示する。
                wktime = int.Parse(outTmData[3]);

                if (wktime < 0 || wktime > 59)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                                                    new string[] { Properties.Resources.err_message_boot_end_time_mi, "0", "59" });
                    return false;
                }

                // 開始時刻＜終了時刻以外の場合、エラーダイアログを表示する。

                // 開始時刻を取得
                stHM[0] = int.Parse(outTmData[0]);
                stHM[1] = int.Parse(outTmData[1]);

                // 終了時刻を取得
                edHM[0] = int.Parse(outTmData[2]);
                edHM[1] = int.Parse(outTmData[3]);

                TimeSpan t1 = new TimeSpan(stHM[0], stHM[1], 0);
                TimeSpan t2 = new TimeSpan(edHM[0], edHM[1], 0);

                if (t1 >= t2)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_BOOT_TIME_005);
                    return false;
                }

                //added by YAMA 2014/06/23
                // 開始時刻と終了時刻の時間幅が23時59分を超えてる場合、エラーダイアログを表示する
                //23時間59分（23:59:00）を表すTimeSpanオブジェクトを作成する
                TimeSpan ts3 = TimeSpan.Parse("23:59");
                TimeSpan ts4 = t2 - t1;

                if (ts3 < ts4)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_BOOT_TIME_006);
                    return false;
                }

                // 間隔時間（分）の半角数字チェック
                CycleInterval = container.textBox_CycleInterval.Text.Trim();
                errItem[0] = Properties.Resources.err_message_boot_time_CycleInterval; // 間隔時間（分）

                if (CycleInterval.Length == 0)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, errItem);
                    return false;
                }

                if (CheckUtil.IsHankakuNum(CycleInterval))
                {
                }
                else
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, errItem);
                    return false;
                }

                // 間隔時間（分）の値が「1」～「720（12時間）」以外の場合、エラーダイアログを表示する。
                wktime = int.Parse(CycleInterval);

                if (wktime < 1 || wktime > 720)
                {
                    errItem[1] = "1";
                    errItem[2] = "720";

                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017, errItem);
                    return false;
                }

            }

            return retCode;
        }


        #endregion

        private void container_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
