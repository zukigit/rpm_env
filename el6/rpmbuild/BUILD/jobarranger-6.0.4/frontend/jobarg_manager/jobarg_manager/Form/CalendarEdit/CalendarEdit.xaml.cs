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
using System.Windows.Input;
using System.Windows.Controls;
using jp.co.ftf.jobcontroller.Common;
using System.Data;
using System.Windows.Threading;
using jp.co.ftf.jobcontroller.DAO;

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM 2012/11/15 新規作成<BR>                            *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.CalendarEdit
{
    /// <summary>
    /// CalendarEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class CalendarEdit : EditBaseUserControl
    {
        #region フィールド

        /// <summary>公開フラグ</summary>
        private bool oldPublicFlg;

        /// <summary>カレンダー名</summary>
        private string oldCalendarName;

        /// <summary>説明</summary>
        private string oldComment;

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        /// <summary> カレンダーテーブル（公開フラグ更新用） </summary>
        private DataTable _calendarTbl;

        /// <summary> カレンダー管理テーブル </summary>
        private CalendarControlDAO _calendarControlDAO;

        /// <summary> カレンダー稼働日テーブル </summary>
        private CalendarDetailDAO _calendarDetailDAO;

        /// <summary> ファイル読込WINDOW </summary>
        private FileReadWindow _fileReadWindow;

        /// <summary> 初期登録WINDOW </summary>
        private CalendarInitWindow _calendarInitWindow;

        /// <summary> ヘルスチェック用タイマー </summary>
        private DispatcherTimer _dispatcherTimer;


        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ(新規追加用)</summary>
        public CalendarEdit(JobArrangerWindow parentWindow)
        {
            ParantWindow = parentWindow;

            InitializeComponent();
            tbxCalendarId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            // 初期化
            LoadForAdd();

            HankakuTextChangeEvent.AddTextChangedEventHander(tbxCalendarId);

            _successFlg = true;
        }

        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="calendarId">カレンダーID</param>
        /// <param name="updDate">更新日</param>
        public CalendarEdit(JobArrangerWindow parentWindow, string calendarId, string updDate, Consts.EditType editType)
        {
            ParantWindow = parentWindow;
            if (LoadForUpd(calendarId, updDate, editType))
            {
                InitializeComponent();
                tbxCalendarId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                HankakuTextChangeEvent.AddTextChangedEventHander(tbxCalendarId);
                _successFlg = true;
            }
            else
                _successFlg = false;
        }

        #endregion

        #region プロパティ
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

        /// <summary>カレンダーの編集タイプ</summary>
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
                return "CalendarEdit";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_210;
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

        /// <summary>ＤＢフラグ</summary>
        //private DBTYPE _dbType;

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

                ResetTree(tbxCalendarId.Text);

                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(tbxCalendarId.Text, Consts.ObjectEnum.CALENDAR);
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
                    ParantWindow.ShowObjectList(null, Consts.ObjectEnum.CALENDAR);

                }
            }
            else
            {
                // ロールバック
                this.Rollback();

                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(null, Consts.ObjectEnum.CALENDAR);
            }

            // 終了ログ
            base.WriteEndLog("cancel_Click", Consts.PROCESS_002);
        }

        //*******************************************************************
        /// <summary> ファイルから読込ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void fileRead_Click(object sender, EventArgs e)
        {
            _fileReadWindow = new FileReadWindow(this);
            _fileReadWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _fileReadWindow.Owner = ParantWindow;
            _fileReadWindow.ShowDialog();
        }

        //*******************************************************************
        /// <summary> 初期登録ボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void initRegist_Click(object sender, EventArgs e)
        {
            _calendarInitWindow = new CalendarInitWindow(this, container.textBox_year.Text);
            _calendarInitWindow.Owner = ParantWindow;
            _calendarInitWindow.ShowDialog();
        }

        #endregion

        #region publicメソッド

        //*******************************************************************
        /// <summary>
        /// カレンダーを登録（Treeのオブジェクトをクリック用）
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
                ParantWindow.SetTreeObject(!cbOpen.IsChecked.Value, Consts.ObjectEnum.CALENDAR, objectId);
            }
            if (oldPublicFlg != cbOpen.IsChecked ||
                _editType == Consts.EditType.Add ||
                _editType == Consts.EditType.CopyNew)
            {
                ParantWindow.SetTreeObject(cbOpen.IsChecked.Value, Consts.ObjectEnum.CALENDAR, objectId);
            }

        }

        //*******************************************************************
        /// <summary>コミット</summary>
        //*******************************************************************
        public override void Commit()
        {
            // ロックをリリース
            if ((_editType == Consts.EditType.Modify || _editType == Consts.EditType.CopyVer) && Consts.DBTYPE.MYSQL == LoginSetting.DBType)
                this.RealseLock(_calendarId);
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
                this.RealseLock(_calendarId);
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
                || !(tbCalendarName.Text.Equals(oldCalendarName))
                || !(tbComment.Text.Equals(oldComment)))
            {
                return true;
            }

            // カレンダー詳細が変更された場合
            if (!dataSet.HasChanges())
                return false;

            return true;
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

            // カレンダーIDをセット
            _calendarId = "CALENDAR_" + DBUtil.GetNextId("101");
            container.CalendarId = _calendarId;
            tbxCalendarId.Text = _calendarId;
            //ユーザー名
            lblUserName.Content = LoginSetting.UserName;

            // 空のテーブルを取得
            SetTables();

            // 編集タイプをセット
            _editType = Consts.EditType.Add;

            // プロパティをセット
            container.ParantWindow = this;

            // カレンダーの表示
            ShowCalendarDetail();

        }

        //*******************************************************************
        /// <summary>初期化（編集、コピー新規用）</summary>
        /// <param name="calendarId">カレンダーID</param>
        /// <param name="updDate">更新日</param>
        /// <param name="editType">編集タイプ</param>
        //*******************************************************************
        private bool LoadForUpd(string calendarId, string updDate, Consts.EditType editType)
        {
            // DAOの初期化
            InitialDAO();

            // DB接続
            dbAccess.CreateSqlConnect();

            // 編集タイプをセット
            _editType = editType;

            // 更新日を保存
            _oldUpdateDate = updDate;

            // カレンダーが存在、編集モード時ロック取得
            bool exitLockFlag = ExistCheckAndGetLockForUpd(calendarId, updDate, editType);
            if (!exitLockFlag)
                return false;

            // 画面の初期化
            InitializeComponent();
            // 仮更新日を取得
            SetTmpUpdDate();
            // 各テーブルのデータをコピー追加
            FillTables(calendarId, updDate);

            SetDataSet();
            //　カレンダーIDをセット
            _calendarId = calendarId;
            container.CalendarId = calendarId;
            container.UpdDate = updDate;

            // 仮更新日をセット
            //UpdateUpdDate(container.TmpUpdDate);

            container.ParantWindow = this;

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
        private bool ExistCheckAndGetLockForUpd(string calendarId, string updDate, Consts.EditType editType)
        {
            //編集モード時、calendar_idベースでロックする。
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.CopyVer)
            {
                dbAccess.BeginTransaction();
                try
                {
                    GetLock(calendarId);
                }
                catch (DBException)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_CALENDAR_002);
                    return false;
                }
            }

            //存在チェック
            bool exitFlg = ExistCheck(calendarId, updDate);

            // 存在しない場合
            if (exitFlg == false)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_CALENDAR_001);
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
            // カレンダー管理テーブル
            _calendarControlDAO = new CalendarControlDAO(dbAccess);

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

            // カレンダー管理テーブル
            container.CalendarControlTable = _calendarControlDAO.GetEmptyTable();

            DataRow row = container.CalendarControlTable.NewRow();
            container.CalendarControlTable.Rows.Add(row);

            row["calendar_id"] = this.CalendarId;
            row["update_date"] = this.UpdateDate;

            // カレンダー稼働日テーブル
            container.CalendarDetailTable = _calendarDetailDAO.GetEmptyTable();

            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> カレンダーデータの検索（編集、コピー新規用）</summary>
        /// <param name="calendarId">`カレンダーID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(string calendarId, string updDate)
        {
            // カレンダー管理テーブル（公開フラグ更新用）
            _calendarTbl = _calendarControlDAO.GetEntityByCalendarId(calendarId);

            // カレンダー管理テーブル
            container.CalendarControlTable = _calendarControlDAO.GetEntityByPk(calendarId, updDate);

            // 元データの更新日を取得
            _oldUpdateDate = Convert.ToString(container.CalendarControlTable.Rows[0]["update_date"]);

            // カレンダー稼働日テーブル
            container.CalendarDetailTable = _calendarDetailDAO.GetEntityByCalendar(calendarId, updDate);

        }

        //*******************************************************************
        /// <summary>行状態をAddに変更</summary>
        //*******************************************************************
        private void SetAllRowsState()
        {
            // カレンダー管理テーブル
            if (container.CalendarControlTable != null)
                foreach (DataRow row in container.CalendarControlTable.Select())
                    row.SetAdded();

            // カレンダー稼働日テーブル
            if (container.CalendarDetailTable != null)
                foreach (DataRow row in container.CalendarDetailTable.Select())
                    row.SetAdded();

        }


        //*******************************************************************
        /// <summary> DBがMysql時、ロックをリリース</summary>
        //*******************************************************************
        private void RealseLock(string calendarId)
        {
            _calendarControlDAO.RealseLock(calendarId);
        }

        //*******************************************************************
        /// <summary> カレンダーロック</summary>
        //*******************************************************************
        private void GetLock(string calendarId)
        {
            _calendarControlDAO.GetLock(calendarId, LoginSetting.DBType);

        }

        //*******************************************************************
        /// <summary> カレンダー存在チェック</summary>
        //*******************************************************************
        private bool ExistCheck(string calendarId, string updDate)
        {
            int count = 0; ;

            count = _calendarControlDAO.GetCountByPk(calendarId, updDate);

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
            // added by YAMA 2014/10/20    マネージャ内部時刻同期
            //int year = DateTime.Now.Year;
            int year = (DBUtil.GetSysTime()).Year;
            container.SetYearCalendarDetail(year.ToString());
        }

        //*******************************************************************
        /// <summary>登録前にデータの更新（新規追加用）</summary>
        //*******************************************************************
        private void UpdateForAdd()
        {
            // 現在日付の取得
            //DateTime now = System.DateTime.Now;
            DateTime now = DBUtil.GetSysTime();
            string updateDate = now.ToString("yyyyMMddHHmmss");

            string calendarId = tbxCalendarId.Text;

            // カレンダー管理テーブル
            DataRow[] rowsCalendar = container.CalendarControlTable.Select();

            rowsCalendar[0]["update_date"] = updateDate;
            rowsCalendar[0]["calendar_id"] = calendarId;
            // 有効フラグ（0：無効）
            rowsCalendar[0]["valid_flag"] = "0";
            // 公開フラグ(チェック有：1 チェック無：0)
            if (cbOpen.IsChecked == true)
                rowsCalendar[0]["public_flag"] = "1";
            else
                rowsCalendar[0]["public_flag"] = "0";
            // ユーザー名
            rowsCalendar[0]["user_name"] = LoginSetting.UserName;
            // カレンダー名
            rowsCalendar[0]["calendar_name"] = tbCalendarName.Text;
            // 説明
            rowsCalendar[0]["memo"] = tbComment.Text;

            // カレンダー稼働日テーブル
            DataRow[] rowsCalendarDetail = container.CalendarDetailTable.Select();
            foreach (DataRow row in rowsCalendarDetail)
            {
                row["update_date"] = updateDate;
                row["calendar_id"] = calendarId;
            }

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

            // カレンダー管理テーブル
            DataRow[] rowsCalendar = container.CalendarControlTable.Select();

            // 更新日
            rowsCalendar[0]["update_date"] = updateDate;
            // カレンダー名
            rowsCalendar[0]["calendar_name"] = tbCalendarName.Text;
            // 説明
            rowsCalendar[0]["memo"] = tbComment.Text;
            // 全てのバージョンの公開フラグの更新
            int publicFlg = 0;
            if (cbOpen.IsChecked == true)
                publicFlg = 1;
            rowsCalendar[0]["public_flag"] = publicFlg;
            // 有効フラグ(　0：無効（初期値）)
            rowsCalendar[0]["valid_flag"] = 0;

            // 既存カレンダーバージョンの公開フラグを更新
            UpdatePublicFlg(publicFlg);

            // カレンダー稼働日テーブル
            DataRow[] rowsCalendarDetail = container.CalendarDetailTable.Select();

            foreach (DataRow row in rowsCalendarDetail)
            {
                row["update_date"] = updateDate;
            }

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
            //lblUpdDate.Content = now.ToString("yyyy/MM/dd HH:mm");
        }

        //*******************************************************************
        /// <summary>運用モードをセット（編集不可）</summary>
        //*******************************************************************
        private void SetUseMode()
        {
            tbxCalendarId.IsEnabled = false;
            cbOpen.IsEnabled = false;
            tbCalendarName.IsEnabled = false;
            tbComment.IsEnabled = false;
            btnInit.IsEnabled = false;
            btnFileRead.IsEnabled = false;
            btnRegist.IsEnabled = false;
            ((CustomControls.MonthCalendar)container.winForm.Child).Enabled = false;

        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            DataRow row = container.CalendarControlTable.Select()[0];
            //コピー新規の場合、採番したデフォルトＩＤをセット
            if (_editType == Consts.EditType.CopyNew)
            {
                tbxCalendarId.Text = "CALENDAR_" + DBUtil.GetNextId("101");
                // カレンダー名をセット
                tbCalendarName.Text = Convert.ToString(row["calendar_name"]);
            }
            else
            {
                // カレンダーIDをセット
                tbxCalendarId.Text = Convert.ToString(row["calendar_id"]);
                // カレンダーIDのテキストボックスをグレーアウトし、編集不可の状態とする
                tbxCalendarId.IsEnabled = false;
                // カレンダー名をセット
                tbCalendarName.Text = oldCalendarName = Convert.ToString(row["calendar_name"]);
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
            // カレンダー名を取得
            string calendarName = tbCalendarName.Text.Trim();

            // カレンダー名が未入力の場合
            if (CheckUtil.IsNullOrEmpty(calendarName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_calendar_name });
                return false;
            }

            // バイト数＞64の場合
            if (CheckUtil.IsLenOver(calendarName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_calendar_name, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(calendarName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { Properties.Resources.err_message_calendar_name });
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

            return true;
        }


        //*******************************************************************
        /// <summary>入力チェック（新規追加用） </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheckForAdd()
        {
            string calendarId = tbxCalendarId.Text;
            // カレンダーIDが未入力の場合
            if (CheckUtil.IsNullOrEmpty(calendarId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_calendar_id });
                return false;
            }

            // 半角英数値、「-」、「_」以外文字以外の場合
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(calendarId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { Properties.Resources.err_message_calendar_id });
                return false;
            }

            // カレンダーIDの桁数＞32の場合
            if (CheckUtil.IsLenOver(calendarId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_calendar_id, "32" });
                return false;
            }

            // すでに登録済みのカレンダーIDが指定された場合
            dbAccess.CreateSqlConnect();
            int count = Convert.ToInt16(_calendarControlDAO.GetCountForCheck(calendarId));
            dbAccess.CloseSqlConnect();

            if (count >= 1)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                    new string[] { Properties.Resources.err_message_calendar_id });
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
                // 目的：Rowstateをセット
                dataSet.AcceptChanges();

                // RowstateをAddにセット
                SetAllRowsState();
            }
            // 編集またはコピーバージョンの場合
            else
            {
                UpdateForUpd();
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
                _calendarControlDAO.DeleteByPk(_calendarId, _oldUpdateDate);
            }
        }

        //*******************************************************************
        /// <summary>全てのバージョンの公開フラグを更新</summary>
        /// <param name="publicFlg">公開フラグ</param>
        //*******************************************************************
        private void UpdatePublicFlg(int publicFlg)
        {
            if (_calendarTbl != null)
                foreach (DataRow row in _calendarTbl.Select())
                    row["public_flag"] = publicFlg;
        }

        //*******************************************************************
        /// <summary>データ登録</summary>
        //*******************************************************************
        private void RegistDataTable()
        {
            // 同一のカレンダーの公開フラグを更新
            if (_calendarTbl != null)
                dbAccess.ExecuteNonQuery(_calendarTbl, _calendarControlDAO);
            // カレンダー管理テーブル
            dbAccess.ExecuteNonQuery(container.CalendarControlTable, _calendarControlDAO);
            // カレンダー稼働日テーブル
            dbAccess.ExecuteNonQuery(container.CalendarDetailTable, _calendarDetailDAO);
        }

        //*******************************************************************
        /// <summary>DataSetをセット（編集状態判定用）</summary>
        //*******************************************************************
        private void SetDataSet()
        {
            // dataSetにセット
            dataSet.Tables.Add(container.CalendarControlTable);
            dataSet.Tables.Add(container.CalendarDetailTable);
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
