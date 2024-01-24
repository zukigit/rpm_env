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
//  * @author DHC 劉 偉 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
    /// <summary>
    /// ScheduleEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class ScheduleEdit : EditBaseUserControl
    {
        #region フィールド

        /// <summary>公開フラグ</summary>
        private bool oldPublicFlg;

        /// <summary>スケジュール名</summary>
        private string oldScheduleName;

        /// <summary>説明</summary>
        private string oldComment;

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        /// <summary> スケジュールテーブル（公開フラグ更新用） </summary>
        private DataTable _scheduleTbl;

        /// <summary> スケジュール管理テーブル </summary>
        private ScheduleControlDAO _scheduleControlDAO;

        /// <summary> スケジュール起動時刻テーブル </summary>
        private ScheduleDetailDAO _scheduleDetailDAO;

        /// <summary> スケジュールジョブネットテーブル </summary>
        private ScheduleJobnetDAO _scheduleJobnetDAO;

        /// <summary> ヘルスチェック用タイマー </summary>
        private DispatcherTimer _dispatcherTimer;


        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ(新規追加用)</summary>
        public ScheduleEdit(JobArrangerWindow parentWindow)
        {
            ParantWindow = parentWindow;
            InitializeComponent();
            tbxScheduleId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            // 初期化
            LoadForAdd();

            HankakuTextChangeEvent.AddTextChangedEventHander(tbxScheduleId);

            _successFlg = true;
        }

        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="scheduleId">スケジュールID</param>
        /// <param name="updDate">更新日</param>
        public ScheduleEdit(JobArrangerWindow parentWindow, string scheduleId, string updDate, Consts.EditType editType)
        {
            ParantWindow = parentWindow;
            if (LoadForUpd(scheduleId, updDate, editType))
            {
                InitializeComponent();
                tbxScheduleId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                HankakuTextChangeEvent.AddTextChangedEventHander(tbxScheduleId);
                _successFlg = true;
            }
            else
                _successFlg = false;
        }

        #endregion

        #region プロパティ
        /// <summary>スケジュールID</summary>
        private string _scheduleId;
        public string ScheduleId
        {
            get
            {
                return _scheduleId;
            }
            set
            {
                _scheduleId = value;
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

        /// <summary>スケジュールの編集タイプ</summary>
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
                return "ScheduleEdit";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_220;
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
            regist();
        }


        //*******************************************************************
        /// <summary> キャンセルボタンをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cancel();
        }

        #endregion

        #region publicメソッド

        /// <summary>登録処理</summary>
        public void regist()
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


            //added by YAMA 2014/06/30 参照モードでは登録不可とする
            if (_editType == Consts.EditType.READ)
                return;


            // 編集登録確認ダイアログの表示
            if (MessageBoxResult.Yes == CommonDialog.ShowEditRegistDialog())
            {
                // 登録が失敗の場合、終了
                RegistProcess();

                this.Commit();

                ResetTree(tbxScheduleId.Text);
                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(tbxScheduleId.Text, Consts.ObjectEnum.SCHEDULE);
            }
            // 終了ログ
            base.WriteEndLog("regist_Click", Consts.PROCESS_001);
        }

        /// <summary>キャンセル処理</summary>
        public void cancel()
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
                    ParantWindow.ShowObjectList(null, Consts.ObjectEnum.SCHEDULE);

                }
            }
            else
            {
                // ロールバック
                this.Rollback();

                // オブジェクト一覧画面を表示する
                ParantWindow.ShowObjectList(null, Consts.ObjectEnum.SCHEDULE);
            }

            // 終了ログ
            base.WriteEndLog("cancel_Click", Consts.PROCESS_002);
        }

        //*******************************************************************
        /// <summary>
        /// スケジュールを登録（スケジュール、スケジュールオブジェクトをクリック用）
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
                ParantWindow.SetTreeObject(!cbOpen.IsChecked.Value, Consts.ObjectEnum.SCHEDULE, objectId);
            }
            if (oldPublicFlg != cbOpen.IsChecked ||
                _editType == Consts.EditType.Add ||
                _editType == Consts.EditType.CopyNew)
            {
                ParantWindow.SetTreeObject(cbOpen.IsChecked.Value, Consts.ObjectEnum.SCHEDULE, objectId);
            }

        }

        //*******************************************************************
        /// <summary>コミット</summary>
        //*******************************************************************
        public override void Commit()
        {
            // ロックをリリース
            if ((_editType == Consts.EditType.Modify || _editType == Consts.EditType.CopyVer) && Consts.DBTYPE.MYSQL == LoginSetting.DBType)
                this.RealseLock(_scheduleId);

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
                this.RealseLock(_scheduleId);

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
                || !(tbScheduleName.Text.Equals(oldScheduleName))
                || !(tbComment.Text.Equals(oldComment)))
                return true;

            // 起動時刻、ジョブネットが変更された場合
            if (!dataSet.HasChanges())
                return false;

            return true;
        }

        #endregion

        #region privateメッソド

        //*******************************************************************
        /// <summary>初期化（新規追加用）</summary>
        //*******************************************************************
        private void LoadForAdd()
        {
            // DAOの初期化
            InitialDAO();

            // 仮更新日をセット
            SetTmpUpdDate();

            // スケジュールIDをセット
            _scheduleId = "SCHEDULE_" + DBUtil.GetNextId("102");
            container.ScheduleId = _scheduleId;
            tbxScheduleId.Text = _scheduleId;
            //ユーザー名
            lblUserName.Content = LoginSetting.UserName;

            // 空のテーブルを取得
            SetTables();

            // 編集タイプをセット
            _editType = Consts.EditType.Add;

            // プロパティをセット
            container.ParantWindow = this;

            // スケジュール詳細、ジョブネットの表示
            ShowSchedule();
        }

        //*******************************************************************
        /// <summary>初期化（編集、コピー新規用）</summary>
        /// <param name="scheduleId">スケジュールID</param>
        /// <param name="updDate">更新日</param>
        /// <param name="editType">編集タイプ</param>
        //*******************************************************************
        private bool LoadForUpd(string scheduleId, string updDate, Consts.EditType editType)
        {
            // DAOの初期化
            InitialDAO();

            // DB接続
            dbAccess.CreateSqlConnect();

            // 編集タイプをセット
            _editType = editType;

            // 更新日を保存
            _oldUpdateDate = updDate;

            // スケジュールが存在、編集モード時ロック取得
            bool exitLockFlag = ExistCheckAndGetLockForUpd(scheduleId, updDate, editType);
            if (!exitLockFlag)
                return false;

            // 画面の初期化
            InitializeComponent();
            // 仮更新日を取得
            SetTmpUpdDate();
            // 各テーブルのデータをコピー追加
            FillTables(scheduleId, updDate);

            SetDataSet();
            //　スケジュールIDをセット
            _scheduleId = scheduleId;
            container.ScheduleId = scheduleId;
            container.UpdDate = updDate;

            // 仮更新日をセット
            //UpdateUpdDate(container.TmpUpdDate);

            container.ParantWindow = this;

            // 情報エリアの表示
            SetInfoArea();

            // スケジュール詳細、ジョブネットの表示
            ShowSchedule();

            //ＤＢヘルスチェックをセット
            SetHealthCheck();

            return true;
        }

        //*******************************************************************
        /// <summary> DBのロック取得、存在チェック</summary>
        //*******************************************************************
        private bool ExistCheckAndGetLockForUpd(string scheduleId, string updDate, Consts.EditType editType)
        {
            //編集モード時、calendar_idベースでロックする。
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.CopyVer)
            {
                dbAccess.BeginTransaction();
                try
                {
                    GetLock(scheduleId);
                }
                catch (DBException)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_002);
                    return false;
                }
            }

            //存在チェック
            bool exitFlg = ExistCheck(scheduleId, updDate);

            // 存在しない場合
            if (exitFlg == false)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_001);
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
            // スケジュール管理テーブル
            _scheduleControlDAO = new ScheduleControlDAO(dbAccess);

            // スケジュール起動時刻テーブル
            _scheduleDetailDAO = new ScheduleDetailDAO(dbAccess);

            // スケジュールジョブネットテーブル
            _scheduleJobnetDAO = new ScheduleJobnetDAO(dbAccess);

        }

        //*******************************************************************
        /// <summary> 空テーブルをDataTableにセット(新規追加用)</summary>
        //*******************************************************************
        private void SetTables()
        {
            // スケジュール管理テーブル
            dbAccess.CreateSqlConnect();

            // スケジュール管理テーブル
            container.ScheduleControlTable = _scheduleControlDAO.GetEmptyTable();

            DataRow row = container.ScheduleControlTable.NewRow();
            container.ScheduleControlTable.Rows.Add(row);

            row["schedule_id"] = this.ScheduleId;
            row["update_date"] = this.UpdateDate;

            // スケジュール起動時刻テーブル
            container.ScheduleDetailTable = _scheduleDetailDAO.GetEntityByScheduleEmpty();
            // スケジュールジョブネットテーブル
            container.ScheduleJobnetTable = _scheduleJobnetDAO.GetEntityByScheduleEmpty();


            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> スケジュールデータの検索（編集、コピー新規用）</summary>
        /// <param name="scheduleId">`スケジュールID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(string scheduleId, string updDate)
        {
            // スケジュール管理テーブル（公開フラグ更新用）
            _scheduleTbl = _scheduleControlDAO.GetEntityByScheduleId(scheduleId);

            // スケジュール管理テーブル
            container.ScheduleControlTable = _scheduleControlDAO.GetEntityByPk(scheduleId, updDate);

            // 元データの更新日を取得
            _oldUpdateDate = Convert.ToString(container.ScheduleControlTable.Rows[0]["update_date"]);

            // スケジュール起動時刻テーブル
            container.ScheduleDetailTable = _scheduleDetailDAO.GetEntityBySchedule(scheduleId, updDate);

            // スケジュールジョブネットテーブル
            container.ScheduleJobnetTable = _scheduleJobnetDAO.GetEntityBySchedule(scheduleId, updDate);
        }

        //*******************************************************************
        /// <summary>行状態をAddに変更</summary>
        //*******************************************************************
        private void SetAllRowsState()
        {
            // スケジュール管理テーブル
            if (container.ScheduleControlTable != null)
                foreach (DataRow row in container.ScheduleControlTable.Select())
                {
                    row.SetAdded();
                }

            // スケジュール起動時刻テーブル
            if (container.ScheduleDetailTable != null)
                foreach (DataRow row in container.ScheduleDetailTable.Select())
                {
                    row.SetAdded();
                }

            // スケジュールジョブネットテーブル
            if (container.ScheduleJobnetTable != null)
                foreach (DataRow row in container.ScheduleJobnetTable.Select())
                    row.SetAdded();

        }

        //*******************************************************************
        /// <summary> DBがMysql時、ロックをリリース</summary>
        //*******************************************************************
        private void RealseLock(string scheduleId)
        {
            _scheduleControlDAO.RealseLock(scheduleId);

        }


        //*******************************************************************
        /// <summary> スケジュールロック</summary>
        //*******************************************************************
        private void GetLock(string scheduleId)
        {
            _scheduleControlDAO.GetLock(scheduleId, LoginSetting.DBType);

        }

        //*******************************************************************
        /// <summary> スケジュール存在チェック</summary>
        //*******************************************************************
        private bool ExistCheck(string scheduleId, string updDate)
        {
            int count = 0; ;

            count = _scheduleControlDAO.GetCountByPk(scheduleId, updDate);

            if (count != 1)
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        /// <summary>起動時刻、登録ジョブネットの表示</summary>
        //*******************************************************************
        private void ShowSchedule()
        {
            container.dgCalendarBootTime.ItemsSource = container.ScheduleDetailTable.DefaultView;
            container.dgJobnet.ItemsSource = container.ScheduleJobnetTable.DefaultView;
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
            string scheduleId = tbxScheduleId.Text;

            // スケジュール管理テーブル
            DataRow[] rowsSchedule = container.ScheduleControlTable.Select();

            rowsSchedule[0]["update_date"] = updateDate;
            rowsSchedule[0]["schedule_id"] = scheduleId;

            // 有効フラグ（0：無効）
            rowsSchedule[0]["valid_flag"] = "0";
            // 公開フラグ(チェック有：1 チェック無：0)
            if (cbOpen.IsChecked == true)
                rowsSchedule[0]["public_flag"] = "1";
            else
                rowsSchedule[0]["public_flag"] = "0";
            // ユーザー名
            rowsSchedule[0]["user_name"] = LoginSetting.UserName;
            // スケジュール名
            rowsSchedule[0]["schedule_name"] = tbScheduleName.Text;
            // 説明
            rowsSchedule[0]["memo"] = tbComment.Text;

            // スケジュール起動時刻テーブル
            DataRow[] rowsScheduleDetail = container.ScheduleDetailTable.Select();
            foreach (DataRow row in rowsScheduleDetail)
            {
                row["update_date"] = updateDate;
                row["schedule_id"] = scheduleId;
            }

            // スケジュールジョブネットテーブル
            DataRow[] rowsScheduleJobnet = container.ScheduleJobnetTable.Select();
            foreach (DataRow row in rowsScheduleJobnet)
            {
                row["update_date"] = updateDate;
                row["schedule_id"] = scheduleId;
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

            // スケジュール管理テーブル
            DataRow[] rowsSchedule = container.ScheduleControlTable.Select();

            // 更新日
            rowsSchedule[0]["update_date"] = updateDate;
            // スケジュール名
            rowsSchedule[0]["schedule_name"] = tbScheduleName.Text;
            // 説明
            rowsSchedule[0]["memo"] = tbComment.Text;
            // 全てのバージョンの公開フラグの更新
            int publicFlg = 0;
            if (cbOpen.IsChecked == true)
                publicFlg = 1;
            rowsSchedule[0]["public_flag"] = publicFlg;
            // 有効フラグ(　0：無効（初期値）)
            rowsSchedule[0]["valid_flag"] = 0;

            // 既存スケジュールバージョンの公開フラグを更新
            UpdatePublicFlg(publicFlg);


            // スケジュール起動時刻テーブル
            DataRow[] rowsScheduleDetail = container.ScheduleDetailTable.Select();

            foreach (DataRow row in rowsScheduleDetail)
            {
                row["update_date"] = updateDate;
            }

            // スケジュールジョブネットテーブル
            DataRow[] rowsScheduleJobnet = container.ScheduleJobnetTable.Select();

            foreach (DataRow row in rowsScheduleJobnet)
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
            tbxScheduleId.IsEnabled = false;
            cbOpen.IsEnabled = false;

            tbScheduleName.IsEnabled = false;
            tbComment.IsEnabled = false;
            btnRegist.IsEnabled = false;
            container.SetDisable();
        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            DataRow row = container.ScheduleControlTable.Select()[0];
            //コピー新規の場合、採番したデフォルトＩＤをセット
            if (_editType == Consts.EditType.CopyNew)
            {
                tbxScheduleId.Text = "SCHEDULE_" + DBUtil.GetNextId("102");
                // スケジュール名をセット
                tbScheduleName.Text = Convert.ToString(row["schedule_name"]);
            }
            else
            {
                // スケジュールIDをセット
                tbxScheduleId.Text = Convert.ToString(row["schedule_id"]);
                // スケジュールIDのテキストボックスをグレーアウトし、編集不可の状態とする
                tbxScheduleId.IsEnabled = false;
                // スケジュール名をセット
                tbScheduleName.Text = oldScheduleName = Convert.ToString(row["schedule_name"]);
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
            tbComment.Text = oldComment =Convert.ToString(row["memo"]);
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
            // スケジュール名を取得
            string scheduleName = tbScheduleName.Text.Trim();

            // スケジュール名が未入力の場合
            if (CheckUtil.IsNullOrEmpty(scheduleName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_schedule_name });
                return false;
            }

            // バイト数＞64の場合
            if (CheckUtil.IsLenOver(scheduleName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_schedule_name, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(scheduleName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { Properties.Resources.err_message_schedule_name });
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
            string scheduleId = tbxScheduleId.Text;
            // スケジュールIDが未入力の場合
            if (CheckUtil.IsNullOrEmpty(scheduleId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_schedule_id });
                return false;
            }

            // 半角英数値、「-」、「_」以外文字以外の場合
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(scheduleId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { Properties.Resources.err_message_schedule_id });
                return false;
            }

            // スケジュールIDの桁数＞16の場合
            if (CheckUtil.IsLenOver(scheduleId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_schedule_id, "32" });
                return false;
            }

            // すでに登録済みのスケジュールIDが指定された場合
            dbAccess.CreateSqlConnect();
            int count = Convert.ToInt16(_scheduleControlDAO.GetCountForCheck(scheduleId));
            dbAccess.CloseSqlConnect();

            if (count >= 1)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                    new string[] { Properties.Resources.err_message_schedule_id });
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
                _scheduleControlDAO.DeleteByPk(_scheduleId, _oldUpdateDate);
            }
        }

        //*******************************************************************
        /// <summary>全てのバージョンの公開フラグを更新</summary>
        /// <param name="publicFlg">公開フラグ</param>
        //*******************************************************************
        private void UpdatePublicFlg(int publicFlg)
        {
            if (_scheduleTbl != null)
                foreach (DataRow row in _scheduleTbl.Select())
                    row["public_flag"] = publicFlg;
        }

        //*******************************************************************
        /// <summary>データ登録</summary>
        //*******************************************************************
        private void RegistDataTable()
        {
            // 同一のスケジュールの公開フラグを更新
            if (_scheduleTbl != null)
                dbAccess.ExecuteNonQuery(_scheduleTbl, _scheduleControlDAO);
            // スケジュール管理テーブル
            dbAccess.ExecuteNonQuery(container.ScheduleControlTable, _scheduleControlDAO);
            // スケジュール起動時刻テーブル
            dbAccess.ExecuteNonQuery(container.ScheduleDetailTable, _scheduleDetailDAO);
            // スケジュールジョブネットテーブル
            dbAccess.ExecuteNonQuery(container.ScheduleJobnetTable, _scheduleJobnetDAO);
        }

        //*******************************************************************
        /// <summary>DataSetをセット（編集状態判定用）</summary>
        //*******************************************************************
        private void SetDataSet()
        {
            // dataSetにセット
            dataSet.Tables.Add(container.ScheduleControlTable);
            dataSet.Tables.Add(container.ScheduleDetailTable);
            dataSet.Tables.Add(container.ScheduleJobnetTable);
        }

        //*******************************************************************
        /// <summary>ヘルスチェックセット</summary>
        //*******************************************************************
        private void SetHealthCheck()
        {
            if (LoginSetting.DBType == Consts.DBTYPE.MYSQL && _editType == Consts.EditType.Modify && LoginSetting.HealthCheckFlag)
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
