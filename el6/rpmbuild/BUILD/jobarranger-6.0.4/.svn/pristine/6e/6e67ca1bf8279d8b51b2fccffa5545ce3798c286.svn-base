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
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Text.RegularExpressions;
using System;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author 孟凡軍 2014/10/1 新規作成<BR>                         *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.FilterEdit
{
    /// <summary>
    /// FilterEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class FilterEdit : EditBaseUserControl
    {
        #region フィールド

        /// <summary>公開フラグ</summary>
        private bool oldPublicFlg;

        /// <summary>フィルター名</summary>
        private string oldFilterName;

        /// <summary>説明</summary>
        private string oldComment;

        /// <summary>カレンダー</summary>
        private object oldCalendar;

        /// <summary>基準日</summary>
        private int oldBaseDate;

        /// <summary>指定日</summary>
        private string oldDesignatedDay;

        /// <summary>移動日数</summary>
        private string oldShiftDay;

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary> カレンダーテーブル（公開フラグ更新用） </summary>
        private DataTable _filterTbl;

        /// <summary> フィルター管理テーブル </summary>
        private FilterControlDAO _filterControlDAO;

        /// <summary> カレンダー管理テーブル </summary>
        private CalendarControlDAO _calendarControlDAO;

        /// <summary> ヘルスチェック用タイマー </summary>
        private DispatcherTimer _dispatcherTimer;

        /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ(新規追加用)</summary>
        public FilterEdit(JobArrangerWindow parentWindow)
        {
            ParantWindow = parentWindow;

            InitializeComponent();
            tbxFilterId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            // 初期化
            LoadForAdd();

            HankakuTextChangeEvent.AddTextChangedEventHander(tbxFilterId);

            _successFlg = true;
        }

        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="filterId">フィルターID</param>
        /// <param name="updDate">更新日</param>
        public FilterEdit(JobArrangerWindow parentWindow, string filterId, string updDate, Consts.EditType editType)
        {
            ParantWindow = parentWindow;
            if (LoadForUpd(filterId, updDate, editType))
            {
                InitializeComponent();
                tbxFilterId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                HankakuTextChangeEvent.AddTextChangedEventHander(tbxFilterId);
                _successFlg = true;
            }
            else
                _successFlg = false;
        }

        #endregion

        #region プロパティ
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

        /// <summary>元データの更新日</summary>
        private string _oldUpdateDate;
        public string OldUpdateDate
        {
            get
            {
                return _oldUpdateDate;
            }
            set
            {
                _oldUpdateDate = value;
            }
        }

        /// <summary>更新日</summary>
        private string _updateDate;
        public string UpdateDate
        {
            get
            {
                return _updateDate;
            }
            set
            {
                _updateDate = value;
            }
        }

        /// <summary>フィルターの編集タイプ</summary>
        private Consts.EditType _editType;

        /// <summary>ウィンドウ</summary>
        JobArrangerWindow _parantWindow;
        public override JobArrangerWindow ParantWindow
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

        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "FilterEdit";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_213;
            }
        }

        /// <summary>成功フラグ</summary>
        private bool _successFlg = false;
        public override bool SuccessFlg
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

        /// <summary>ヘルスチェックフラグ</summary>
        private bool _healthCheckFlag = false;
        public override bool HealthCheckFlag
        {
            get
            {
                return _healthCheckFlag;
            }
            set
            {
                _healthCheckFlag = true;
            }
        }

        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>登録ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void HealthCheck(object sender, EventArgs e)
        {
            dbAccess.ExecuteHealthCheck();
        }

        //*******************************************************************
        /// <summary>登録ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void regist_Click(object sender, RoutedEventArgs e)
        {
            // 開始ログ
            base.WriteStartLog("regist_Click", Consts.PROCESS_001);

            //Park.iggy Add
            ParantWindow.ObjectAllFlag = false;

            // 入力チェック
            if (!InputCheck())
                return;

            // 新規追加の場合のチェック
            if ((_editType == Consts.EditType.Add || _editType == Consts.EditType.CopyNew) && !InputCheckForAdd())
                return;

            // 編集登録確認ダイアログの表示
            if (MessageBoxResult.Yes == CommonDialog.ShowEditRegistDialog())
            {
                // 登録が失敗の場合、終了
                RegistProcess();

                this.Commit();

                ResetTree(tbxFilterId.Text);

                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(tbxFilterId.Text, Consts.ObjectEnum.FILTER);
            }

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
            base.WriteStartLog("cancel_Click", Consts.PROCESS_002);

            if (HasEditedCheck())
            {
                // 編集登録確認ダイアログの表示
                if (MessageBoxResult.Yes == CommonDialog.ShowCancelDialog())
                {
                    // ロールバック
                    this.Rollback();

                    // オブジェクト一覧画面を表示する
                    ParantWindow.ShowObjectList(null, Consts.ObjectEnum.FILTER);

                }
            }
            else
            {
                // ロールバック
                this.Rollback();

                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(null, Consts.ObjectEnum.FILTER);
            }

            // 終了ログ
            base.WriteEndLog("cancel_Click", Consts.PROCESS_002);
        }

        #endregion

        #region publicメソッド

        //*******************************************************************
        /// <summary>
        /// フィルターを登録（Treeのオブジェクトをクリック用）
        /// </summary>
        //*******************************************************************
        public override bool RegistObject()
        {
            // 入力チェック
            if (!InputCheck())
                return false;


            // 新規追加の場合のチェック
            if ((_editType == Consts.EditType.Add || _editType == Consts.EditType.CopyNew) && !InputCheckForAdd())
                return false;

            // DB登録処理
            RegistProcess();

            return true;
        }

        //*******************************************************************
        /// <summary>
        /// Treeを再セット
        /// </summary>
        //*******************************************************************
        public override void ResetTree(String objectId)
        {
            //Park.iggy Add
            ParantWindow.ObjectAllFlag = false;

            if (objectId == null && (_editType == Consts.EditType.Modify || _editType == Consts.EditType.READ))
                objectId = ((TreeViewItem)ParantWindow.treeView1.SelectedItem).Header.ToString();

            if (oldPublicFlg != cbOpen.IsChecked &&
                _editType != Consts.EditType.Add &&
                _editType != Consts.EditType.CopyNew)
            {
                ParantWindow.SetTreeObject(!cbOpen.IsChecked.Value, Consts.ObjectEnum.FILTER, objectId);
            }
            if (oldPublicFlg != cbOpen.IsChecked ||
                _editType == Consts.EditType.Add ||
                _editType == Consts.EditType.CopyNew)
            {
                ParantWindow.SetTreeObject(cbOpen.IsChecked.Value, Consts.ObjectEnum.FILTER, objectId);
            }

        }

        //*******************************************************************
        /// <summary>コミット</summary>
        //*******************************************************************
        public override void Commit()
        {
            // ロックをリリース
            if ((_editType == Consts.EditType.Modify || _editType == Consts.EditType.CopyVer) && Consts.DBTYPE.MYSQL == LoginSetting.DBType)
                this.RealseLock(_filterId);
            if (_healthCheckFlag)
                _dispatcherTimer.Stop();
            dbAccess.TransactionCommit();
            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary>ロールバック</summary>
        //*******************************************************************
        public override void Rollback()
        {
            // ロックをリリース
            if ((_editType == Consts.EditType.Modify || _editType == Consts.EditType.CopyVer) && Consts.DBTYPE.MYSQL == LoginSetting.DBType)
                this.RealseLock(_filterId);
            if (_healthCheckFlag)
                _dispatcherTimer.Stop();
            dbAccess.TransactionRollback();
            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary>終了の場合（メニューから呼び出す用）</summary>
        //*******************************************************************
        public void ShowEndDialog()
        {
            if (CommonDialog.ShowEndDialog() == MessageBoxResult.Yes)
            {
                dbAccess.TransactionRollback();
                dbAccess.CloseSqlConnect();
                Application.Current.Shutdown();
            }
        }

        //*******************************************************************
        /// <summary>編集かどうかの判定</summary>
        //*******************************************************************
        public override bool HasEditedCheck()
        {
            // 情報エリアの情報が変更された場合
            if (cbOpen.IsChecked != oldPublicFlg
                || !(tbFilterName.Text.Equals(oldFilterName))
                || !(tbComment.Text.Equals(oldComment)))
            {
                return true;
            }

            if ((container.cmbCalendar.SelectedItem != null && !container.cmbCalendar.SelectedItem.Equals(oldCalendar))
                    || GetBaseDate(container.rbFirstDay,container.rbLastDay,container.rbDesignatedDay) != oldBaseDate
                    || (container.rbDesignatedDay.IsChecked.Value && !oldDesignatedDay.Equals(container.tbDesignatedDay.Text))
                    || (container.cmbShiftDay.SelectedItem != null && !container.cmbShiftDay.SelectedItem.Equals(oldShiftDay))
                    )
            {
                return true;
            }

            return false;
        }

        #endregion

        #region privateメソッド

        //*******************************************************************
        /// <summary>初期化（新規追加用）</summary>
        //*******************************************************************
        private void LoadForAdd()
        {
            // DAOの初期化
            InitialDAO();

            // 仮更新日をセット
            SetTmpUpdDate();

            // フィルターIDをセット
            _filterId = "FILTER_" + DBUtil.GetNextId("103");
            tbxFilterId.Text = _filterId;
            //ユーザー名
            lblUserName.Content = LoginSetting.UserName;

            // 空のテーブルを取得
            SetTables();

            // 編集タイプをセット
            _editType = Consts.EditType.Add;

            // プロパティをセット
            container.ParantWindow = this;

            DataTable _calendarTable;
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER) {
                _calendarTable = _calendarControlDAO.GetInfoByUserIdSuper();
            } else {
                _calendarTable = _calendarControlDAO.GetInfoByUserId(LoginSetting.UserID);
            }

            List<string> calendarIds = new List<string>();
            foreach (DataRow calendarRow in _calendarTable.Select()) {
                calendarIds.Add(Convert.ToString(calendarRow["calendar_id"]));
            }
            container.cmbCalendar.Items.Clear();
            container.cmbCalendar.ItemsSource = calendarIds;

            container.rbFirstDay.IsChecked = true;

            List<string> shiftDays = new List<string>();
            for (int i = -7; i <= 7; i++) {
                shiftDays.Add(Convert.ToString(i));
            }
            container.cmbShiftDay.Items.Clear();
            container.cmbShiftDay.ItemsSource = shiftDays;
            container.cmbShiftDay.SelectedItem = "1";
        }

        //*******************************************************************
        /// <summary>初期化（編集、コピー新規用）</summary>
        /// <param name="filterId">フィルターID</param>
        /// <param name="updDate">更新日</param>
        /// <param name="editType">編集タイプ</param>
        //*******************************************************************
        private bool LoadForUpd(string filterId, string updDate, Consts.EditType editType)
        {
            // DAOの初期化
            InitialDAO();

            // DB接続
            dbAccess.CreateSqlConnect();

            // 編集タイプをセット
            _editType = editType;

            // 更新日を保存
            _oldUpdateDate = updDate;

            // フィルターが存在、編集モード時ロック取得
            bool exitLockFlag = ExistCheckAndGetLockForUpd(filterId, updDate, editType);
            if (!exitLockFlag)
                return false;

            // 画面の初期化
            InitializeComponent();
            // 仮更新日を取得
            SetTmpUpdDate();

            // 各テーブルのデータをコピー追加
            FillTables(filterId, updDate);

            /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
            SetDataSet();

            //　フィルターIDをセット
            _filterId = filterId;
            container.UpdDate = updDate;

            container.ParantWindow = this;

            // 対象フィルター
            DataRow row = container.FilterControlTable.Rows[0];

            if(Convert.ToString(row["base_date_flag"]) == "0")
            {
                container.rbFirstDay.IsChecked=true;
                container.tbDesignatedDay.Text = "";
            }
            else if(Convert.ToString(row["base_date_flag"]) == "1")
            {
                container.rbLastDay.IsChecked=true;
                container.tbDesignatedDay.Text = "";
            }
            else
            {
                container.rbDesignatedDay.IsChecked=true;
                container.tbDesignatedDay.Text = Convert.ToString(row["designated_day"]);
                oldDesignatedDay = Convert.ToString(row["designated_day"]);
            }

            oldBaseDate = GetBaseDate(container.rbFirstDay, container.rbLastDay, container.rbDesignatedDay);

            List<string> shiftDays = new List<string>();
            for (int i = -7; i <= 7; i++){
                shiftDays.Add(Convert.ToString(i));
            }
            container.cmbShiftDay.Items.Clear();
            container.cmbShiftDay.ItemsSource = shiftDays;
            container.cmbShiftDay.SelectedItem = Convert.ToString(row["shift_day"]);
            oldShiftDay = Convert.ToString(row["shift_day"]);

            // カレンダー
            DataTable _calendarTable;
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                _calendarTable = _calendarControlDAO.GetInfoByUserIdSuper();
            }
            else
            {
                _calendarTable = _calendarControlDAO.GetInfoByUserId(LoginSetting.UserID);
            }

            List<string> calendarIds = new List<string>();
            foreach (DataRow calendarRow in _calendarTable.Select())
            {
                calendarIds.Add(Convert.ToString(calendarRow["calendar_id"]));
            }
            container.cmbCalendar.Items.Clear();
            container.cmbCalendar.ItemsSource = calendarIds;
            container.cmbCalendar.SelectedItem = Convert.ToString(row["base_calendar_id"]);

            oldCalendar = row["base_calendar_id"];

            // カレンダーの表示
            ShowCalendarDetail();

            // 情報エリアの表示
            SetInfoArea();

            //ＤＢヘルスチェックをセット
            SetHealthCheck();

            return true;
        }

        //*******************************************************************
        /// <summary> DBのロック取得、存在チェック</summary>
        //*******************************************************************
        private bool ExistCheckAndGetLockForUpd(string filterId, string updDate, Consts.EditType editType)
        {
            //編集モード時、Filter_idベースでロックする。
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.CopyVer)
            {
                dbAccess.BeginTransaction();
                try
                {
                    GetLock(filterId);
                }
                catch (DBException ex)
                {
                    return false;
                }
            }

            //存在チェック
            bool exitFlg = ExistCheck(filterId, updDate);

            // 存在しない場合
            if (exitFlg == false)
            {
                Rollback();
                return false;
            }

            return true;

        }

        //*******************************************************************
        /// <summary> DAOの初期化処理</summary>
        //*******************************************************************
        private void InitialDAO()
        {
            // フィルター管理テーブル
            _filterControlDAO = new FilterControlDAO(dbAccess);

            // カレンダー管理テーブル
            _calendarControlDAO = new CalendarControlDAO(dbAccess);

        }

        //*******************************************************************
        /// <summary> 空テーブルをDataTableにセット(新規追加用)</summary>
        //*******************************************************************
        private void SetTables()
        {
            dbAccess.CreateSqlConnect();

            // フィルター管理テーブル
            container.FilterControlTable = _filterControlDAO.GetEmptyTable();

            DataRow row = container.FilterControlTable.NewRow();
            container.FilterControlTable.Rows.Add(row);

            row["filter_id"] = this.FilterId;
            row["update_date"] = this.UpdateDate;

            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> カレンダーデータの検索（編集、コピー新規用）</summary>
        /// <param name="filterId">`フィルターID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(string filterId, string updDate)
        {
            // フィルター管理テーブル（公開フラグ更新用）
            _filterTbl = _filterControlDAO.GetEntityByFilterId(filterId);

            // フィルター管理テーブル
            container.FilterControlTable = _filterControlDAO.GetEntityByPk(filterId, updDate);

            // 元データの更新日を取得
            _oldUpdateDate = Convert.ToString(container.FilterControlTable.Rows[0]["update_date"]);

        }


        /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
        //*******************************************************************
        /// <summary>行状態をAddに変更</summary>
        //*******************************************************************
        private void SetAllRowsState()
        {
            // フィルター管理テーブル
            if (container.FilterControlTable != null)
                foreach (DataRow row in container.FilterControlTable.Select())
                    row.SetAdded();

            // カレンダー稼働日テーブル
            if (container.CalendarDetailTable != null)
                foreach (DataRow row in container.CalendarDetailTable.Select())
                    row.SetAdded();

        }


        //*******************************************************************
        /// <summary> DBがMysql時、ロックをリリース</summary>
        //*******************************************************************
        private void RealseLock(string filterId)
        {
            _filterControlDAO.RealseLock(filterId);
        }

        //*******************************************************************
        /// <summary> カレンダーロック</summary>
        //*******************************************************************
        private void GetLock(string filterId)
        {
            _filterControlDAO.GetLock(filterId, LoginSetting.DBType);

        }

        //*******************************************************************
        /// <summary> フィルター存在チェック</summary>
        //*******************************************************************
        private bool ExistCheck(string filterId, string updDate)
        {
            int count = 0; ;

            count = _filterControlDAO.GetCountByPk(filterId, updDate);

            if (count != 1)
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        /// <summary>カレンダー稼働日の表示</summary>
        //*******************************************************************

        private void ShowCalendarDetail()
        {
            container.SetCalendarDetail();
        }

        //*******************************************************************
        /// <summary>登録前にデータの更新（新規追加用）</summary>
        //*******************************************************************
        private void UpdateForAdd()
        {
            // 現在日付の取得
            DateTime now = DBUtil.GetSysTime();
            string updateDate = now.ToString("yyyyMMddHHmmss");
            string filterId = tbxFilterId.Text.Trim();

            // フィルター管理テーブル
            DataRow[] rowsFilter = container.FilterControlTable.Select();

            // 更新日
            rowsFilter[0]["update_date"] = updateDate;
            // フィルターID
            rowsFilter[0]["filter_id"] = filterId;
            // フィルター名
            rowsFilter[0]["filter_name"] = tbFilterName.Text.Trim();
            // 有効フラグ（0：無効）
            rowsFilter[0]["valid_flag"] = "0";
            // 公開フラグ(チェック有：1 チェック無：0)
            if (cbOpen.IsChecked == true)
                rowsFilter[0]["public_flag"] = "1";
            else
                rowsFilter[0]["public_flag"] = "0";

            if (container.rbFirstDay.IsChecked == true)
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "0";
                // 指定日
                rowsFilter[0]["designated_day"] = "0";
            }
            else if (container.rbLastDay.IsChecked == true)
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "1";
                // 指定日
                rowsFilter[0]["designated_day"] = "0";
            }
            else
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "2";
                // 指定日
                rowsFilter[0]["designated_day"] = container.tbDesignatedDay.Text.Trim();
            }
            // シフト日数
            rowsFilter[0]["shift_day"] = container.cmbShiftDay.SelectedItem;
            // ベースカレンダーID
            rowsFilter[0]["base_calendar_id"] = container.cmbCalendar.SelectedItem;
            // ユーザー名
            rowsFilter[0]["user_name"] = LoginSetting.UserName;
            // 説明
            rowsFilter[0]["memo"] = tbComment.Text;

        }

        //*******************************************************************
        /// <summary>登録前にデータの更新（編集、コピー追加用）</summary>
        /// <param name="updateDate">更新日</param>
        //*******************************************************************
        private void UpdateForUpd()
        {
            // DBサーバの時間を取得

            DateTime now = DBUtil.GetSysTime();
            string updateDate = now.ToString("yyyyMMddHHmmss");

            // フィルター管理テーブル
            DataRow[] rowsFilter = container.FilterControlTable.Select();

            // 更新日
            rowsFilter[0]["update_date"] = updateDate;
            // フィルター名
            rowsFilter[0]["filter_name"] = tbFilterName.Text;
            // 説明
            rowsFilter[0]["memo"] = tbComment.Text;
            // 全てのバージョンの公開フラグの更新
            int publicFlg = 0;
            if (cbOpen.IsChecked == true)
                publicFlg = 1;
            rowsFilter[0]["public_flag"] = publicFlg;
            // 有効フラグ(　0：無効（初期値）)
            rowsFilter[0]["valid_flag"] = 0;

            // 既存カレンダーバージョンの公開フラグを更新
            UpdatePublicFlg(publicFlg);

            if (container.rbFirstDay.IsChecked == true)
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "0";
                // 指定日
                rowsFilter[0]["designated_day"] = "0";
            }
            else if (container.rbLastDay.IsChecked == true)
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "1";
                // 指定日
                rowsFilter[0]["designated_day"] = "0";
            }
            else
            {
                // 基準日フラグ
                rowsFilter[0]["base_date_flag"] = "2";
                // 指定日
                rowsFilter[0]["designated_day"] = container.tbDesignatedDay.Text;
            }
            // シフト日数
            rowsFilter[0]["shift_day"] = container.cmbShiftDay.SelectedItem;
            // ベースカレンダーID
            rowsFilter[0]["base_calendar_id"] = container.cmbCalendar.SelectedItem;

        }

        //*******************************************************************
        /// <summary>仮更新日をセット</summary>
        //*******************************************************************
        private void SetTmpUpdDate()
        {
            // DBサーバの時間を取得

            DateTime now = DBUtil.GetSysTime();
            _updateDate = now.ToString("yyyyMMddHHmmss");
            container.TmpUpdDate = _updateDate;
        }

        //*******************************************************************
        /// <summary>運用モードをセット（編集不可）</summary>
        //*******************************************************************
        private void SetUseMode()
        {
            tbxFilterId.IsEnabled = false;
            cbOpen.IsEnabled = false;
            tbFilterName.IsEnabled = false;
            tbComment.IsEnabled = false;
            btnRegist.IsEnabled = false;
            container.cmbCalendar.IsEnabled =false;
            container.rbFirstDay.IsEnabled =false;
            container.rbLastDay.IsEnabled =false;
            container.rbDesignatedDay.IsEnabled =false;
            container.tbDesignatedDay.IsEnabled =false;
            container.cmbShiftDay.IsEnabled =false;
        }

        //*******************************************************************
        /// <summary>ラジオグループの値取得</summary>
        //*******************************************************************
        private int GetBaseDate(RadioButton rb1, RadioButton rb2, RadioButton rb3){
            int val  = 0;
            if (rb1.IsChecked.Value){
                val = 1;
            }else if(rb2.IsChecked.Value){
                val = 2;
            }else if (rb3.IsChecked.Value){
                val = 3;
            }
            return val;
        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            DataRow row = container.FilterControlTable.Select()[0];
            //コピー新規の場合、採番したデフォルトＩＤをセット
            if (_editType == Consts.EditType.CopyNew)
            {
                tbxFilterId.Text = "FILTER_" + DBUtil.GetNextId("103");
                // フィルター名をセット
                tbFilterName.Text = Convert.ToString(row["filter_name"]);
            }
            else
            {
                // フィルターIDをセット
                tbxFilterId.Text = Convert.ToString(row["filter_id"]);
                // フィルターIDのテキストボックスをグレーアウトし、編集不可の状態とする
                tbxFilterId.IsEnabled = false;
                // フィルター名をセット
                tbFilterName.Text = oldFilterName = Convert.ToString(row["filter_name"]);
            }

            // 公開チェックボックス
            int openFlg = Convert.ToInt16(row["public_flag"]);
            if (openFlg == 0)
                cbOpen.IsChecked = oldPublicFlg = false;
            else if (openFlg == 1)
                cbOpen.IsChecked = oldPublicFlg = true;

            // 権限が運用の場合
            if (Consts.ActionMode.USE == LoginSetting.Mode || _editType == Consts.EditType.READ)
            {
                SetUseMode();
                powerLabel.Content = Properties.Resources.can_not_update_auth;
            }

            // 説明
            tbComment.Text = oldComment = Convert.ToString(row["memo"]);
            //ユーザー名
            if (_editType == Consts.EditType.CopyNew)
            {
                lblUserName.Content = LoginSetting.UserName;
            }
            else
            {
                lblUserName.Content = Convert.ToString(row["user_name"]);
            }

            //更新日
            if (_editType == Consts.EditType.READ || _editType == Consts.EditType.Modify)
            {
                lblUpdDate.Content = ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(row["update_date"])).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                lblUpdDate.Content = "";
            }

        }


        //*******************************************************************
        /// <summary>入力チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheck()
        {
            // フィルター名を取得
            string filterName = tbFilterName.Text.Trim();

            // フィルター名が未入力の場合
            if (CheckUtil.IsNullOrEmpty(filterName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_filter_name });
                return false;
            }

            // バイト数＞64の場合
            if (CheckUtil.IsLenOver(filterName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_filter_name, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(filterName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { Properties.Resources.err_message_filter_name });
                return false;
            }

            // 説明のチェック
            string comment = tbComment.Text.Trim();
            if (CheckUtil.IsLenOver(comment, 100))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_memo, "100" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(comment))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { Properties.Resources.err_message_memo });
                return false;
            }

            // カレンダーチェック
            object calendar = container.cmbCalendar.SelectedItem;
            if (calendar == null)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_calendar });
                return false;
            }

            if (container.rbDesignatedDay.IsChecked == true) {
                // 指定日
                string designatedDay = container.tbDesignatedDay.Text.Trim();
                if (CheckUtil.IsNullOrEmpty(designatedDay)) {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                        new string[] { Properties.Resources.err_message_designated_day });
                    return false;
                }

                if (!CheckUtil.IsHankakuNum(designatedDay)) {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                        new string[] { Properties.Resources.err_message_designated_day });
                    return false;
                }

                int designatedDayNumber = Convert.ToInt16(designatedDay);
                if (designatedDayNumber < 1 || designatedDayNumber > 31 ) {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017,
                        new string[] { Properties.Resources.err_message_designated_day, "1", "31" });
                    return false;
                }
            }

            // 移動日数
            object shiftDay = container.cmbShiftDay.SelectedItem;
            if (shiftDay == null)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_shift_day });
                return false;
            }

            return true;
        }

        //*******************************************************************
        /// <summary>入力チェック（新規追加用） </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheckForAdd()
        {
            string filterId = tbxFilterId.Text;
            // フィルターIDが未入力の場合
            if (CheckUtil.IsNullOrEmpty(filterId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_filter_id });
                return false;
            }

            // 半角英数値、「-」、「_」以外文字以外の場合
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(filterId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { Properties.Resources.err_message_filter_id });
                return false;
            }

            // フィルターIDの桁数＞32の場合
            if (CheckUtil.IsLenOver(filterId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_filter_id, "32" });
                return false;
            }

            // すでに登録済みのフィルターIDが指定された場合
            dbAccess.CreateSqlConnect();
            int count = Convert.ToInt16(_filterControlDAO.GetCountForCheck(filterId));
            dbAccess.CloseSqlConnect();

            if (count >= 1)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                    new string[] { Properties.Resources.err_message_filter_id });
                return false;
            }

            return true;
        }

        //*******************************************************************
        /// <summary>ＤＢデータ登録</summary>
        //*******************************************************************
        private void RegistProcess()
        {
            dbAccess.CreateSqlConnect();
            dbAccess.BeginTransaction();

            // 新規追加の場合
            if (Consts.EditType.Add == _editType)
            {
                UpdateForAdd();
            }
            // コピー新規の場合
            else if (Consts.EditType.CopyNew == _editType)
            {
                UpdateForAdd();

                /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
                // 目的：Rowstateをセット
                dataSet.AcceptChanges();
                // RowstateをAddにセット
                SetAllRowsState();

            }
            // 編集またはコピーバージョンの場合
            else
            {
                UpdateForUpd();

                /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
                // 目的：Rowstateをセット
                dataSet.AcceptChanges();
                // RowstateをAddにセット
                SetAllRowsState();

            }

            // 登録
            RegistDataTable();

            // 古いバージョンを削除
            if (Consts.EditType.Modify == _editType)
            {
                _filterControlDAO.DeleteByPk(_filterId, _oldUpdateDate);
            }
        }

        //*******************************************************************
        /// <summary>全てのバージョンの公開フラグを更新</summary>
        /// <param name="publicFlg">公開フラグ</param>
        //*******************************************************************
        private void UpdatePublicFlg(int publicFlg)
        {
            if (_filterTbl != null)
                foreach (DataRow row in _filterTbl.Select())
                    row["public_flag"] = publicFlg;
        }


        /* added by YAMA 2014/12/05    V2.1.0 No29対応 */
        //*******************************************************************
        /// <summary>DataSetをセット（編集状態判定用）</summary>
        //*******************************************************************
        private void SetDataSet()
        {
            // dataSetにセット
            dataSet.Tables.Add(container.FilterControlTable);
//            dataSet.Tables.Add(container.CalendarDetailTable);
        }


        //*******************************************************************
        /// <summary>データ登録</summary>
        //*******************************************************************
        private void RegistDataTable()
        {
            // 同一のフィルターの公開フラグを更新
            if (_filterTbl != null)
                dbAccess.ExecuteNonQuery(_filterTbl, _filterControlDAO);
            // フィルター管理テーブル
            dbAccess.ExecuteNonQuery(container.FilterControlTable, _filterControlDAO);
        }

        //*******************************************************************
        /// <summary>ヘルスチェックセット</summary>
        //*******************************************************************
        private void SetHealthCheck()
        {
            if (LoginSetting.DBType == Consts.DBTYPE.MYSQL && LoginSetting.HealthCheckFlag)
            {
                _healthCheckFlag = true;
                _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
                _dispatcherTimer.Interval = new TimeSpan(0, LoginSetting.HealthCheckInterval, 0);
                _dispatcherTimer.Tick += new EventHandler(HealthCheck);
                _dispatcherTimer.Start();
            }
        }

        #endregion
    }
}
