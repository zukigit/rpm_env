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
using System.Windows.Controls;
using jp.co.ftf.jobcontroller;
using jp.co.ftf.jobcontroller.Common;
using System.Data;
using System.Collections;
using System.Windows.Input;
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
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// JobEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class JobEdit : EditBaseUserControl
    {
        #region フィールド

        /// <summary>公開フラグ</summary>
        private bool oldPublicFlg;

        /// <summary>ジョブネット名</summary>
        private string oldJobnetName;

        /// <summary>説明</summary>
        private string oldComment;

        /// <summary>ジョブネットタイムアウト</summary>
        private string oldJobnetTimeout;

        /// <summary>ジョブネットタイムアウト実行タイプ</summary>
        private int oldJobnetTimeoutRunType;

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        /// <summary> ジョブネットテーブル（公開フラグ更新用） </summary>
        private DataTable _jobnetTbl;

        /// <summary> ジョブネット管理テーブル </summary>
        private JobnetControlDAO _jobnetControlDAO;

        /// <summary> ジョブ管理テーブル </summary>
        private JobControlDAO _jobControlDAO;

        /// <summary> フロー管理テーブル </summary>
        private FlowControlDAO _flowControlDAO;

        /// <summary> 計算アイコン設定テーブル </summary>
        private IconCalcDAO _iconCalcDAO;

        /// <summary> 終了アイコン設定テーブル </summary>
        private IconEndDAO _iconEndDAO;

        /// <summary> 拡張ジョブアイコン設定テーブル </summary>
        private IconExtJobDAO _iconExtJobDAO;

        /// <summary> 条件分岐アイコン設定テーブル </summary>
        private IconIfDAO _iconIfDAO;

        /// <summary> 情報取得アイコン設定テーブル </summary>
        private IconInfoDAO _iconInfoDAO;

        /// <summary> ジョブネットアイコン設定テーブル </summary>
        private IconJobnetDAO _iconJobnetDAO;

        /// <summary> ジョブアイコン設定テーブル </summary>
        private IconJobDAO _iconJobDAO;

        /// <summary> ジョブコマンド設定テーブル </summary>
        private JobCommandDAO _jobCommandDAO;

        /// <summary> ジョブ変数設定テーブル </summary>
        private ValueJobDAO _valueJobDAO;

        /// <summary> ジョブコントローラ変数設定テーブル </summary>
        private ValueJobConDAO _valueJobConDAO;

        /// <summary> タスクアイコン設定テーブル </summary>
        private IconTaskDAO _iconTaskDAO;

        /// <summary> ジョブコントローラ変数アイコン設定テーブル </summary>
        private IconValueDAO _iconValueDAO;

        /// <summary> ジョブコントローラ変数定義テーブル </summary>
        private DefineValueDAO _defineValueDAO;

        /// <summary> 拡張ジョブ定義テーブル </summary>
        private DefineExtJobDAO _defineExtJobDAO;

        /// <summary> ヘルスチェック用タイマー </summary>
        private DispatcherTimer _dispatcherTimer;

        /// <summary> ファイル転送アイコン設定テーブル </summary>
        private IconFcopyDAO _iconFcopyDAO;

        /// <summary> ファイル待ち合わせアイコン設定テーブル </summary>
        private IconFwaitDAO _iconFwaitDAO;

        /// <summary> リブートアイコン設定テーブル </summary>
        private IconRebootDAO _iconRebootDAO;

        /// <summary> 保留解除アイコン設定テーブル </summary>
        private IconReleaseDAO _iconReleaseDAO;

        //added by YAMA 2014/02/06
        /// <summary> Zabbix連携アイコン設定テーブル </summary>
        private IconCooperationDAO _iconCooperationDAO;

        //added by YAMA 2014/05/19
        /// <summary> エージェントレスアイコン設定テーブル </summary>
        private IconAgentlessDAO _iconAgentlessDAO;

        public Hashtable JobNoHash = new Hashtable();

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ(新規追加用)</summary>
        public JobEdit(JobArrangerWindow parentWindow)
        {
            ParantWindow = parentWindow;

            InitializeComponent();
            tbxJobNetId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            // 初期化
            LoadForAdd();

            HankakuTextChangeEvent.AddTextChangedEventHander(tbxJobNetId);
            CommonUtil.InitJobNo(JobNoHash);

            _successFlg = true;
        }

        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="jobnetId">ジョブネットID</param>
        /// <param name="updDate">更新日</param>
        public JobEdit(JobArrangerWindow parentWindow, string jobnetId, string updDate, Consts.EditType editType)
        {
            ParantWindow = parentWindow;
            if (LoadForUpd(jobnetId, updDate, editType))
            {
                InitializeComponent();
                tbxJobNetId.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                HankakuTextChangeEvent.AddTextChangedEventHander(tbxJobNetId);
                CommonUtil.InitJobNo(JobNoHash);
                _successFlg = true;
            }
            else
                _successFlg = false;
        }

        #endregion

        #region プロパティ
        /// <summary>ジョブネットID</summary>
        private string _jobnetId;
        public string JobnetId
        {
            get
            {
                return _jobnetId;
            }
            set
            {
                _jobnetId = value;
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

        /// <summary>ジョブフローの編集タイプ</summary>
        private Consts.EditType _editType;

        public Consts.EditType FlowEditType
        {
            get
            {
                return _editType;
            }
            set
            {
                _editType = value;
            }
        }

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
                return "JobEdit";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_230;
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
        /// <summaryサブジョブネットフラグ</summary>
        private bool _isSubJobnet = false;
        public bool IsSubJobnet
        {
            get
            {
                return _isSubJobnet;
            }
            set
            {
                _isSubJobnet = value;
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
            if ((_editType == Consts.EditType.Add  || _editType == Consts.EditType.CopyNew) && !InputCheckForAdd())
                return;

            // CheckParallelEnd
            if (!CheckParallelEnd())
                return;

            //設定済みチェック
            if (!SettingCheck())
                return;

            // 編集登録確認ダイアログの表示
            if (MessageBoxResult.Yes == CommonDialog.ShowEditRegistDialog())
            {
                // 登録が失敗の場合、終了

                RegistProcess();

                this.Commit();

                //画面表示
                AfterRegistView();

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

                    if (!_isSubJobnet)
                    {
                        // オブジェクト一覧画面を表示する
                        ParantWindow.ShowObjectList(null, Consts.ObjectEnum.JOBNET);
                    }
                    else
                    {
                        Window.GetWindow(this).Close();
                    }

                }
            }
            else
            {
                // ロールバック
                this.Rollback();

                if (!_isSubJobnet)
                {
                    // オブジェクト一覧画面を表示する
                    ParantWindow.ShowObjectList(null, Consts.ObjectEnum.JOBNET);
                }
                else
                {
                    Window.GetWindow(this).Close();
                }
            }

            // 終了ログ
            base.WriteEndLog("cancel_Click", Consts.PROCESS_002);
        }

        /// <summary>部品をダブルクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        private void UserControl_MouseDoubleClick4Read(object sender, MouseButtonEventArgs e)
        {
            CommonItem room = (CommonItem)sender;
            room.SetSelectedColor();
            room.ShowIconSetting(false);
        }

        #endregion

        #region publicメソッド

        //*******************************************************************
        /// <summary>
        /// ジョブネットを登録（カレンダー、スケジュールオブジェクトをクリック用）
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

            //設定済みチェック
            if (!SettingCheck())
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
                ParantWindow.SetTreeObject(!cbOpen.IsChecked.Value, Consts.ObjectEnum.JOBNET, objectId);
            }
            if (oldPublicFlg != cbOpen.IsChecked ||
                _editType == Consts.EditType.Add ||
                _editType == Consts.EditType.CopyNew)
            {
                ParantWindow.SetTreeObject(cbOpen.IsChecked.Value, Consts.ObjectEnum.JOBNET, objectId);
            }

        }

        //*******************************************************************
        /// <summary>コミット</summary>
        //*******************************************************************
        public override void Commit()
        {
            // ロックをリリース
            if ((_editType == Consts.EditType.Modify || _editType == Consts.EditType.CopyVer) && Consts.DBTYPE.MYSQL == LoginSetting.DBType)
                this.RealseLock(_jobnetId);

            //表示中から削除
            ParantWindow.viewJobEdit.Remove(_jobnetId + _oldUpdateDate);

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
                this.RealseLock(_jobnetId);

            //表示中から削除
            ParantWindow.viewJobEdit.Remove(_jobnetId + _oldUpdateDate);
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
                || !(tbJobnetName.Text.Equals(oldJobnetName))
                || !(tbComment.Text.Equals(oldComment))
                || !(tbJobnetTimeout.Text.Equals(oldJobnetTimeout))
                || !(combJobNetRunType.SelectedIndex == oldJobnetTimeoutRunType))
                return true;

            // ジョブフローが変更された場合
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

            // ジョブネットIDをセット
            _jobnetId = "JOBNET_" + DBUtil.GetNextId("100");
            container.JobnetId = _jobnetId;
            tbxJobNetId.Text = _jobnetId;
            //ユーザー名
            lblUserName.Content = LoginSetting.UserName;

            // 空のテーブルを取得
            SetTables();

            // 編集タイプをセット
            _editType = Consts.EditType.Add;

            // ジョブIDを初期化
            //CommonUtil.InitJobNo();
            // プロパティをセット
            container.ParantWindow = this;


            //added by YAMA 2014/04/22
            // 多重起動コンボボックスの設定

            //DataTableオブジェクトを用意
            DataTable CombData = new DataTable();

            //DataTableに列を追加
            CombData.Columns.Add("ID", typeof(string));
            CombData.Columns.Add("ITEM", typeof(string));

            DataRow CombDataRow = CombData.NewRow();

            //各列に値をセット
            CombDataRow["ID"] = "0";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type1;
            CombData.Rows.Add(CombDataRow);

            CombDataRow = CombData.NewRow();
            CombDataRow["ID"] = "1";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type2;
            CombData.Rows.Add(CombDataRow);

            CombDataRow = CombData.NewRow();
            CombDataRow["ID"] = "2";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type3;
            CombData.Rows.Add(CombDataRow);

            combMultipleStart.Items.Clear();
            combMultipleStart.ItemsSource = CombData.DefaultView;
            combMultipleStart.DisplayMemberPath = "ITEM";
            combMultipleStart.SelectedValuePath = "ID";

            combMultipleStart.SelectedValue = "0";
        }

        //*******************************************************************
        /// <summary>初期化（編集、コピー新規用）</summary>
        /// <param name="jobnetId">ジョブネットID</param>
        /// <param name="updDate">更新日</param>
        /// <param name="editType">編集タイプ</param>
        //*******************************************************************
        private bool LoadForUpd(string jobnetId, string updDate, Consts.EditType editType)
        {
            //既に表示中か判断して、表示中の場合はエラーメッセージを表示
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.READ)
            {
                if (ParantWindow.viewJobEdit.Contains(jobnetId + updDate))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_013);
                    return false;
                }
            }

            // DAOの初期化
            InitialDAO();

            // DB接続
            dbAccess.CreateSqlConnect();

            // 編集タイプをセット
            _editType = editType;

            // 更新日を保存
            _oldUpdateDate = updDate;

            // ジョブネットが存在、編集モード時ロック取得
            bool exitLockFlag = ExistCheckAndGetLockForUpd(jobnetId, updDate, editType);
            if (!exitLockFlag)
                return false;

            // 画面の初期化
            InitializeComponent();
            // 仮更新日を取得
            SetTmpUpdDate();
            // 各テーブルのデータをコピー追加
            FillTables(jobnetId, updDate);

            SetDataSet();
            //　ジョブIDをセット
            _jobnetId = jobnetId;
            container.JobnetId = jobnetId;

            container.ParantWindow = this;

            // 情報エリアの表示
            SetInfoArea();

            // ジョブフロー領域の表示
            ShowJobNet();

            // ジョブIDを初期化
            //CommonUtil.InitJobNo();

            //ＤＢヘルスチェックをセット
            SetHealthCheck();

            //表示中に追加
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.READ)
            {
                ParantWindow.viewJobEdit[jobnetId + updDate] = "1";
            }

            return true;
        }

        //*******************************************************************
        /// <summary> DBのロック取得、存在チェック</summary>
        //*******************************************************************
        private bool ExistCheckAndGetLockForUpd(string jobnetId, string updDate, Consts.EditType editType)
        {
            //編集モード時、jobnet_idベースでロックする。
            if (editType == Consts.EditType.Modify || editType == Consts.EditType.CopyVer)
            {
                dbAccess.BeginTransaction();
                try
                {
                    GetLock(jobnetId);
                }
                catch (DBException)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_006);
                    return false;
                }
            }

            //存在チェック
            bool exitFlg = ExistCheck(jobnetId, updDate);

            // 存在しない場合
            if (exitFlg == false)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_002);
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
            // ジョブネット管理テーブル
            _jobnetControlDAO = new JobnetControlDAO(dbAccess);

            // ジョブ管理テーブル
            _jobControlDAO = new JobControlDAO(dbAccess);

            // フロー管理テーブル
            _flowControlDAO = new FlowControlDAO(dbAccess);

            // 計算アイコン設定テーブル
            _iconCalcDAO = new IconCalcDAO(dbAccess);

            // 終了アイコン設定テーブル
            _iconEndDAO = new IconEndDAO(dbAccess);

            /// 拡張ジョブアイコン設定テーブル
            _iconExtJobDAO = new IconExtJobDAO(dbAccess);

            /// 条件分岐アイコン設定テーブル
            _iconIfDAO = new IconIfDAO(dbAccess);

            /// 情報取得アイコン設定テーブル
            _iconInfoDAO = new IconInfoDAO(dbAccess);

            /// ジョブネットアイコン設定テーブル
            _iconJobnetDAO = new IconJobnetDAO(dbAccess);

            /// ジョブアイコン設定テーブル
            _iconJobDAO = new IconJobDAO(dbAccess);

            /// ジョブコマンド設定テーブル
            _jobCommandDAO = new JobCommandDAO(dbAccess);

            /// ジョブ変数設定テーブル
            _valueJobDAO = new ValueJobDAO(dbAccess);

            /// ジョブコントローラ変数設定テーブル
            _valueJobConDAO = new ValueJobConDAO(dbAccess);

            /// タスクアイコン設定テーブル
            _iconTaskDAO = new IconTaskDAO(dbAccess);

            /// ジョブコントローラ変数アイコン設定テーブル
            _iconValueDAO = new IconValueDAO(dbAccess);

            /// ジョブコントローラ変数定義テーブル
            _defineValueDAO = new DefineValueDAO(dbAccess);

            /// 拡張ジョブ定義テーブル
            _defineExtJobDAO = new DefineExtJobDAO(dbAccess);

            /// ファイル転送アイコン設定テーブル
            _iconFcopyDAO = new IconFcopyDAO(dbAccess);

            /// ファイル待ち合わせアイコン設定テーブル
            _iconFwaitDAO = new IconFwaitDAO(dbAccess);

            /// リブートアイコン設定テーブル
            _iconRebootDAO = new IconRebootDAO(dbAccess);

            /// 保留解除アイコン設定テーブル
            _iconReleaseDAO = new IconReleaseDAO(dbAccess);

            //added by YAMA 2014/02/06
            /// Zabbix連携アイコン設定テーブル
            _iconCooperationDAO = new IconCooperationDAO(dbAccess);

            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            _iconAgentlessDAO = new IconAgentlessDAO(dbAccess);
        }

        //*******************************************************************
        /// <summary> 空テーブルをDataTableにセット(新規追加用)</summary>
        //*******************************************************************
        private void SetTables()
        {
            // ジョブネット管理テーブル
            dbAccess.CreateSqlConnect();

            // ジョブネットテーブル
            container.JobnetControlTable = _jobnetControlDAO.GetEmptyTable();

            DataRow row = container.JobnetControlTable.NewRow();
            container.JobnetControlTable.Rows.Add(row);

            row["jobnet_id"] = this.JobnetId;
            row["update_date"] = this.UpdateDate;

            // ジョブ管理テーブル
            container.JobControlTable = _jobControlDAO.GetEmptyTable();
            // フロー管理テーブル
            container.FlowControlTable = _flowControlDAO.GetEmptyTable();
            // 計算アイコン設定テーブル
            container.IconCalcTable = _iconCalcDAO.GetEmptyTable();
            // 終了アイコン設定テーブル
            container.IconEndTable = _iconEndDAO.GetEmptyTable();
            // 拡張ジョブアイコン設定テーブル
            container.IconExtjobTable = _iconExtJobDAO.GetEmptyTable();
            // 条件分岐アイコン設定テーブル
            container.IconIfTable = _iconIfDAO.GetEmptyTable();
            // 情報取得アイコン設定テーブル
            container.IconInfoTable = _iconInfoDAO.GetEmptyTable();
            // ジョブネットアイコン設定テーブル
            container.IconJobnetTable = _iconJobnetDAO.GetEmptyTable();
            // ジョブアイコン設定テーブル
            container.IconJobTable = _iconJobDAO.GetEmptyTable();
            // ジョブコマンド設定テーブル
            container.JobCommandTable = _jobCommandDAO.GetEmptyTable();
            // ジョブ変数設定テーブル
            container.ValueJobTable = _valueJobDAO.GetEmptyTable();
            // ジョブコントローラ変数設定テーブル
            container.ValueJobConTable = _valueJobConDAO.GetEmptyTable();
            // タスクアイコン設定テーブル
            container.IconTaskTable = _iconTaskDAO.GetEmptyTable();
            // ジョブコントローラ変数アイコン設定テーブル
            container.IconValueTable = _iconValueDAO.GetEmptyTable();
            // ジョブコントローラ変数定義テーブル
            container.DefineValueJobconTable = _defineValueDAO.GetEmptyTable();
            // 拡張ジョブ定義テーブル (一旦不要)
            //container.DefineExtJobTable = _defineExtJobDAO.GetEmptyTable();
            // ファイル転送アイコン設定テーブル
            container.IconFcopyTable = _iconFcopyDAO.GetEmptyTable();
            // ファイル待ち合わせアイコン設定テーブル
            container.IconFwaitTable = _iconFwaitDAO.GetEmptyTable();
            // リブートアイコン設定テーブル
            container.IconRebootTable = _iconRebootDAO.GetEmptyTable();
            // 保留解除アイコン設定テーブル
            container.IconReleaseTable = _iconReleaseDAO.GetEmptyTable();

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            container.IconCooperationTable = _iconCooperationDAO.GetEmptyTable();
            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            container.IconAgentlessTable = _iconAgentlessDAO.GetEmptyTable();

            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> ジョブネットデータの検索（編集、コピー新規用）</summary>
        /// <param name="jobnetId">`ジョブネットID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(string jobnetId, string updDate)
        {
            // ジョブネット管理テーブル（公開フラグ更新用）
            _jobnetTbl = _jobnetControlDAO.GetEntityByJobnetId(jobnetId);

            // ジョブネット管理テーブル
            container.JobnetControlTable = _jobnetControlDAO.GetEntityByPk(jobnetId, updDate);

            // 元データの更新日を取得
            _oldUpdateDate = Convert.ToString(container.JobnetControlTable.Rows[0]["update_date"]);
            // ジョブ管理テーブル
            container.JobControlTable = _jobControlDAO.GetEntityByJobIdForUpdate(jobnetId, updDate);
            // フロー管理テーブル
            container.FlowControlTable = _flowControlDAO.GetEntityByJobnet(jobnetId, updDate);

            // 計算アイコン設定テーブル
            container.IconCalcTable = _iconCalcDAO.GetEntityByJobnet(jobnetId, updDate);

            //container.IconCalcTable.Rows
            // 終了アイコン設定テーブル
            container.IconEndTable = _iconEndDAO.GetEntityByJobnet(jobnetId, updDate);
            // 拡張ジョブアイコン設定テーブル
            container.IconExtjobTable = _iconExtJobDAO.GetEntityByJobnet(jobnetId, updDate);

            // 条件分岐アイコン設定テーブル
            container.IconIfTable = _iconIfDAO.GetEntityByJobnet(jobnetId, updDate);

            // 情報取得アイコン設定テーブル
            container.IconInfoTable = _iconInfoDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブネットアイコン設定テーブル
            container.IconJobnetTable = _iconJobnetDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブアイコン設定テーブル
            container.IconJobTable = _iconJobDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブコマンド設定テーブル
            container.JobCommandTable = _jobCommandDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブ変数設定テーブル
            container.ValueJobTable = _valueJobDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブコントローラ変数設定テーブル
            container.ValueJobConTable = _valueJobConDAO.GetEntityByJobnet(jobnetId, updDate);

            // タスクアイコン設定テーブル
            container.IconTaskTable = _iconTaskDAO.GetEntityByJobnet(jobnetId, updDate);

            // ジョブコントローラ変数アイコン設定テーブル
            container.IconValueTable = _iconValueDAO.GetEntityByJobnet(jobnetId, updDate);

            // ファイル転送アイコン設定テーブル
            container.IconFcopyTable = _iconFcopyDAO.GetEntityByJobnet(jobnetId, updDate);

            // ファイル待ち合わせアイコン設定テーブル
            container.IconFwaitTable = _iconFwaitDAO.GetEntityByJobnet(jobnetId, updDate);

            // リブートアイコン設定テーブル
            container.IconRebootTable = _iconRebootDAO.GetEntityByJobnet(jobnetId, updDate);

            // 保留解除アイコン設定テーブル
            container.IconReleaseTable = _iconReleaseDAO.GetEntityByJobnet(jobnetId, updDate);

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            container.IconCooperationTable = _iconCooperationDAO.GetEntityByJobnet(jobnetId, updDate);

            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            container.IconAgentlessTable = _iconAgentlessDAO.GetEntityByJobnet(jobnetId, updDate);
        }

        //*******************************************************************
        /// <summary>行状態をAddに変更</summary>
        //*******************************************************************
        private void SetAllRowsState()
        {
            // ジョブネット管理テーブル
            if (container.JobnetControlTable != null)
                foreach (DataRow row in container.JobnetControlTable.Select())
                    row.SetAdded();

            // ジョブ管理テーブル
            if (container.JobControlTable != null)
                foreach (DataRow row in container.JobControlTable.Select())
                    row.SetAdded();

            // フロー管理テーブル
            if (container.FlowControlTable != null)
                foreach (DataRow row in container.FlowControlTable.Select())
                    row.SetAdded();

            // 計算アイコン設定テーブル
            if (container.IconCalcTable != null)
                foreach (DataRow row in container.IconCalcTable.Select())
                    row.SetAdded();

            // 終了アイコン設定テーブル
            if (container.IconEndTable != null)
                foreach (DataRow row in container.IconEndTable.Select())
                    row.SetAdded();

            // 拡張ジョブアイコン設定テーブル
            if (container.IconExtjobTable != null)
                foreach (DataRow row in container.IconExtjobTable.Select())
                    row.SetAdded();

            // 条件分岐アイコン設定テーブル
            if (container.IconIfTable != null)
                foreach (DataRow row in container.IconIfTable.Select())
                    row.SetAdded();

            // 情報取得アイコン設定テーブル
            if (container.IconInfoTable != null)
                foreach (DataRow row in container.IconInfoTable.Select())
                    row.SetAdded();

            // ジョブネットアイコン設定テーブル
            if (container.IconJobnetTable != null)
                foreach (DataRow row in container.IconJobnetTable.Select())
                    row.SetAdded();

            // ジョブアイコン設定テーブル
            if (container.IconJobTable != null)
                foreach (DataRow row in container.IconJobTable.Select())
                    row.SetAdded();

            // ジョブコマンド設定テーブル
            if (container.JobCommandTable != null)
                foreach (DataRow row in container.JobCommandTable.Select())
                    row.SetAdded();

            // ジョブ変数設定テーブル
            if (container.ValueJobTable != null)
                foreach (DataRow row in container.ValueJobTable.Select())
                    row.SetAdded();

            // ジョブコントローラ変数設定テーブル
            if (container.ValueJobConTable != null)
                foreach (DataRow row in container.ValueJobConTable.Select())
                    row.SetAdded();

            // タスクアイコン設定テーブル
            if (container.IconTaskTable != null)
                foreach (DataRow row in container.IconTaskTable.Select())
                    row.SetAdded();

            // ジョブコントローラ変数アイコン設定テーブル
            if (container.IconValueTable != null)
                foreach (DataRow row in container.IconValueTable.Select())
                    row.SetAdded();

            // ファイル転送アイコン設定テーブル
            if (container.IconFcopyTable != null)
                foreach (DataRow row in container.IconFcopyTable.Select())
                    row.SetAdded();

            // ファイル待ち合わせアイコン設定テーブル
            if (container.IconFwaitTable != null)
                foreach (DataRow row in container.IconFwaitTable.Select())
                    row.SetAdded();

            // リブートアイコン設定テーブル
            if (container.IconRebootTable != null)
                foreach (DataRow row in container.IconRebootTable.Select())
                    row.SetAdded();

            // 保留解除アイコン設定テーブル
            if (container.IconReleaseTable != null)
                foreach (DataRow row in container.IconReleaseTable.Select())
                    row.SetAdded();

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            if (container.IconCooperationTable != null)
                foreach (DataRow row in container.IconCooperationTable.Select())
                    row.SetAdded();

            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            if (container.IconAgentlessTable != null)
                foreach (DataRow row in container.IconAgentlessTable.Select())
                    row.SetAdded();
        }

        //*******************************************************************
        /// <summary> DBがMysql時、ロックをリリース</summary>
        //*******************************************************************
        private void RealseLock(string jobnetId)
        {
            _jobnetControlDAO.RealseLock(jobnetId);

        }

        //*******************************************************************
        /// <summary> ジョブネットロック</summary>
        //*******************************************************************
        private void GetLock(string jobnetId)
        {
            _jobnetControlDAO.GetLock(jobnetId, LoginSetting.DBType);

        }

        //*******************************************************************
        /// <summary> ジョブネット存在チェック</summary>
        //*******************************************************************
        private bool ExistCheck(string jobnetId, string updDate)
        {
            int count = 0;;

            count = _jobnetControlDAO.GetCountByPk(jobnetId, updDate);

            if (count!=1)
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        /// <summary>ジョブフロー領域の表示</summary>
        //*******************************************************************
        private void ShowJobNet()
        {
            // ジョブデータ（ジョブアイコンの生成用）
            JobData jobData = null;

            // ジョブを表示------------------
            foreach (DataRow row in container.JobControlTable.Select())
            {
                jobData = new JobData();
                // ジョブタイプ
                jobData.JobType = (ElementType)row["job_type"];

                CommonItem room = new CommonItem(container, jobData, _editType, (RunJobMethodType)row["method_flag"]);
                // ジョブID
                room.JobId = Convert.ToString(row["job_id"]);
                //ジョブ名
                room.JobName = Convert.ToString(row["job_name"]);
                // X位置
                room.SetValue(Canvas.LeftProperty, Convert.ToDouble(row["point_x"]));
                // Y位置
                room.SetValue(Canvas.TopProperty, Convert.ToDouble(row["point_y"]));

                // ToolTip設定
                room.ContentItem.SetToolTip();

                // 権限が運用の場合
                if (Consts.ActionMode.USE == LoginSetting.Mode || _editType == Consts.EditType.READ)
                {
                    room.RemoveAllEvent();
                    room.MouseDoubleClick += UserControl_MouseDoubleClick4Read;
                    container.RemoveContainerMoveEvent();
                }

                // ジョブフロー領域に追加
                container.ContainerCanvas.Children.Add(room);
                container.JobItems.Add(room.JobId, room);
                container.SetedJobIds[room.JobId] = "1";
            }

            // フローを表示------------------
            // 開始ジョブID、終了ジョブId
            string startJobId,endJobId;
            // 開始ジョブ、終了ジョブ
            IRoom startJob, endJob;
            // フロー幅
            int flowWidth;
            // フロータイプ(直線、曲線)
            FlowLineType lineType;
            // フロータイプ（　0：通常、　1：TURE、　2：FALSE）
            int flowType = 0;
            foreach (DataRow row in container.FlowControlTable.Select())
            {
                startJobId = Convert.ToString(row["start_job_id"]);
                endJobId = Convert.ToString(row["end_job_id"]);
                flowWidth = Convert.ToInt16(row["flow_width"]);
                flowType = Convert.ToInt16(row["flow_type"]);

                // フロータイプの判定
                if (flowWidth == 0)
                {
                    lineType = FlowLineType.Line;
                }
                else
                {
                    lineType = FlowLineType.Curve;
                }

                startJob = (IRoom)container.JobItems[startJobId];
                endJob = (IRoom)container.JobItems[endJobId];

                container.MakeFlow(lineType, startJob, endJob, flowType, _editType);
            }
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
            string createDate = now.ToString("yyyy-MM-dd HH:mm:ss.sss");

            string jobnetId = tbxJobNetId.Text;

            // ジョブネット管理テーブル
            DataRow[] rowsJobnet = container.JobnetControlTable.Select();

            rowsJobnet[0]["update_date"] = updateDate;
            rowsJobnet[0]["jobnet_id"] = jobnetId;
            // 有効フラグ（0：無効）
            rowsJobnet[0]["valid_flag"] = "0";
            // 公開フラグ(チェック有：1 チェック無：0)
            if (cbOpen.IsChecked == true)
                rowsJobnet[0]["public_flag"] = "1";
            else
                rowsJobnet[0]["public_flag"] = "0";
            // ユーザー名
            rowsJobnet[0]["user_name"] = LoginSetting.UserName;
            // ジョブネット名
            rowsJobnet[0]["jobnet_name"] = tbJobnetName.Text;
            // 説明
            rowsJobnet[0]["memo"] = tbComment.Text;

            // ジョブネットタイムアウト
            rowsJobnet[0]["jobnet_timeout"] = tbJobnetTimeout.Text;

            //ジョブネットタイムアウト実行タイプ
            rowsJobnet[0]["timeout_run_type"] = combJobNetRunType.SelectedIndex;

            //added by YAMA 2014/04/22
            // ジョブネットの多重起動の有無
            rowsJobnet[0]["multiple_start_up"] = combMultipleStart.SelectedValue;


            // ジョブ管理テーブル
            DataRow[] rowsJob = container.JobControlTable.Select();
            foreach (DataRow row in rowsJob)
            {
                row["update_date"] = updateDate;
                row["jobnet_id"] = jobnetId;
            }

            // フロー管理テーブル
            DataRow[] rowsFlow = container.FlowControlTable.Select();
            foreach (DataRow row in rowsFlow)
            {
                row["update_date"] = updateDate;
                row["jobnet_id"] = jobnetId;
            }

            // 計算アイコン設定テーブル
            DataRow[] rowsIconCalc = container.IconCalcTable.Select();
            foreach (DataRow row in rowsIconCalc)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // 終了アイコン設定テーブル
            DataRow[] rowsIconEnd = container.IconEndTable.Select();
            foreach (DataRow row in rowsIconEnd)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // 拡張ジョブアイコン設定テーブル
            DataRow[] rowsIconExtjob = container.IconExtjobTable.Select();
            foreach (DataRow row in rowsIconExtjob)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // 条件分岐アイコン設定テーブル
            DataRow[] rowsIconIf = container.IconIfTable.Select();
            foreach (DataRow row in rowsIconIf)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // 情報取得アイコン設定テーブル
            DataRow[] rowsIconInfo = container.IconInfoTable.Select();
            foreach (DataRow row in rowsIconInfo)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブネットアイコン設定テーブル
            DataRow[] rowsIconJobnet = container.IconJobnetTable.Select();
            foreach (DataRow row in rowsIconJobnet)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブアイコン設定テーブル
            DataRow[] rowsIconJob = container.IconJobTable.Select();
            foreach (DataRow row in rowsIconJob)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブコマンド設定テーブル
            DataRow[] rowsJobCommand = container.JobCommandTable.Select();
            foreach (DataRow row in rowsJobCommand)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブ変数設定テーブル
            DataRow[] rowsValueJob = container.ValueJobTable.Select();
            foreach (DataRow row in rowsValueJob)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブコントローラ変数設定テーブル
            DataRow[] rowsValueJobcon = container.ValueJobConTable.Select();
            foreach (DataRow row in rowsValueJobcon)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // タスクアイコン設定テーブル
            DataRow[] rowsIconTask = container.IconTaskTable.Select();
            foreach (DataRow row in rowsIconTask)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // ジョブコントローラ変数アイコン設定テーブル
            DataRow[] rowsIconValue = container.IconValueTable.Select();
            foreach (DataRow row in rowsIconValue)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }
            // ファイル転送アイコン設定テーブル
            DataRow[] rowsIconFcopy = container.IconFcopyTable.Select();
            foreach (DataRow row in rowsIconFcopy)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }
            // ファイル待ち合わせアイコン設定テーブル
            DataRow[] rowsIconFwait = container.IconFwaitTable.Select();
            foreach (DataRow row in rowsIconFwait)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }
            // リブートアイコン設定テーブル
            DataRow[] rowsIconReboot = container.IconRebootTable.Select();
            foreach (DataRow row in rowsIconReboot)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            // 保留解除アイコン設定テーブル
            DataRow[] rowsIconRelease = container.IconReleaseTable.Select();
            foreach (DataRow row in rowsIconRelease)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            DataRow[] rowsIconCooperation = container.IconCooperationTable.Select();
            foreach (DataRow row in rowsIconCooperation)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
            }

            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            DataRow[] rowsIconAgentless = container.IconAgentlessTable.Select();
            foreach (DataRow row in rowsIconAgentless)
            {
                row["jobnet_id"] = jobnetId;
                row["update_date"] = updateDate;
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

            // ジョブネット管理テーブル
            DataRow[] rowsJobnet = container.JobnetControlTable.Select();

            // 更新日
            rowsJobnet[0]["update_date"] = updateDate;
            // ジョブネット名
            rowsJobnet[0]["jobnet_name"] = tbJobnetName.Text;
            // 説明
            rowsJobnet[0]["memo"] = tbComment.Text;
            // ジョブネットタイムアウト
            // タイムアウト警告
            if (!CheckUtil.IsNullOrEmpty(tbJobnetTimeout.Text))
            {
                rowsJobnet[0]["jobnet_timeout"] = tbJobnetTimeout.Text;
            }
            else
            {
                rowsJobnet[0]["jobnet_timeout"] = 0;
            }

            //ジョブネットタイムアウト実行タイプ
            rowsJobnet[0]["timeout_run_type"] = combJobNetRunType.SelectedIndex;

            // 全てのバージョンの公開フラグの更新
            int publicFlg = 0;
            if (cbOpen.IsChecked == true)
                publicFlg = 1;
            rowsJobnet[0]["public_flag"] = publicFlg;
            // 有効フラグ(　0：無効（初期値）)
            rowsJobnet[0]["valid_flag"] = 0;

            //added by YAMA 2014/04/22
            // ジョブネットの多重起動の有無
            rowsJobnet[0]["multiple_start_up"] = combMultipleStart.SelectedValue;


            // 既存ジョブネットバージョンの公開フラグを更新
            UpdatePublicFlg(publicFlg);

            // ジョブ管理テーブル
            DataRow[] rowsJob = container.JobControlTable.Select();

            foreach (DataRow row in rowsJob)
            {
                row["update_date"] = updateDate;
            }

            // フロー管理テーブル
            DataRow[] rowsFlow = container.FlowControlTable.Select();
            foreach (DataRow row in rowsFlow)
            {
                row["update_date"] = updateDate;
            }

            // 計算アイコン設定テーブル
            DataRow[] rowsIconCalc = container.IconCalcTable.Select();
            foreach (DataRow row in rowsIconCalc)
            {
                row["update_date"] = updateDate;
            }

            // 終了アイコン設定テーブル
            DataRow[] rowsIconEnd = container.IconEndTable.Select();
            foreach (DataRow row in rowsIconEnd)
            {
                row["update_date"] = updateDate;
            }

            // 拡張ジョブアイコン設定テーブル
            DataRow[] rowsIconExtjob = container.IconExtjobTable.Select();
            foreach (DataRow row in rowsIconExtjob)
            {
                row["update_date"] = updateDate;
            }

            // 条件分岐アイコン設定テーブル
            DataRow[] rowsIconIf = container.IconIfTable.Select();
            foreach (DataRow row in rowsIconIf)
            {
                row["update_date"] = updateDate;
            }

            // 情報取得アイコン設定テーブル
            DataRow[] rowsIconInfo = container.IconInfoTable.Select();
            foreach (DataRow row in rowsIconInfo)
            {
                row["update_date"] = updateDate;
            }

            // ジョブネットアイコン設定テーブル
            DataRow[] rowsIconJobnet = container.IconJobnetTable.Select();
            foreach (DataRow row in rowsIconJobnet)
            {
                row["update_date"] = updateDate;
            }

            // ジョブアイコン設定テーブル
            DataRow[] rowsIconJob = container.IconJobTable.Select();
            foreach (DataRow row in rowsIconJob)
            {
                row["update_date"] = updateDate;
            }

            // ジョブコマンド設定テーブル
            DataRow[] rowsJobCommand = container.JobCommandTable.Select();
            foreach (DataRow row in rowsJobCommand)
            {
                row["update_date"] = updateDate;
            }

            // ジョブ変数設定テーブル
            DataRow[] rowsValueJob = container.ValueJobTable.Select();
            foreach (DataRow row in rowsValueJob)
            {
                row["update_date"] = updateDate;
            }

            // ジョブコントローラ変数設定テーブル
            DataRow[] rowsValueJobcon = container.ValueJobConTable.Select();
            foreach (DataRow row in rowsValueJobcon)
            {
                row["update_date"] = updateDate;
            }

            // タスクアイコン設定テーブル
            DataRow[] rowsIconTask = container.IconTaskTable.Select();
            foreach (DataRow row in rowsIconTask)
            {
                row["update_date"] = updateDate;
            }

            // ジョブコントローラ変数アイコン設定テーブル
            DataRow[] rowsIconValue = container.IconValueTable.Select();
            foreach (DataRow row in rowsIconValue)
            {
                row["update_date"] = updateDate;
            }
            // ファイル転送アイコン設定テーブル
            DataRow[] rowsIconFcopy = container.IconFcopyTable.Select();
            foreach (DataRow row in rowsIconFcopy)
            {
                row["update_date"] = updateDate;
            }
            // ファイル待ち合わせアイコン設定テーブル
            DataRow[] rowsIconFwait = container.IconFwaitTable.Select();
            foreach (DataRow row in rowsIconFwait)
            {
                row["update_date"] = updateDate;
            }
            // リブートアイコン設定テーブル
            DataRow[] rowsIconReboot = container.IconRebootTable.Select();
            foreach (DataRow row in rowsIconReboot)
            {
                row["update_date"] = updateDate;
            }
            // 保留解除アイコン設定テーブル
            DataRow[] rowsIconRelease = container.IconReleaseTable.Select();
            foreach (DataRow row in rowsIconRelease)
            {
                row["update_date"] = updateDate;
            }

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            DataRow[] rowsIconCooperation = container.IconCooperationTable.Select();
            foreach (DataRow row in rowsIconCooperation)
            {
                row["update_date"] = updateDate;
            }

            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            DataRow[] rowsIconAgentless = container.IconAgentlessTable.Select();
            foreach (DataRow row in rowsIconAgentless)
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

            tbxJobNetId.IsEnabled = false;
            cbOpen.IsEnabled = false;

            tbJobnetName.IsEnabled = false;
            tbComment.IsEnabled = false;
            tbJobnetTimeout.IsEnabled = false;
            combJobNetRunType.IsEnabled = false;
            btnRegist.IsEnabled = false;
            container.sampleContainer.IsEnabled = false;
            container.cnsDesignerContainer.ContextMenu.Visibility = System.Windows.Visibility.Hidden;
            //added by YAMA 2014/04/22
            combMultipleStart.IsEnabled = false;

        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            DataRow row = container.JobnetControlTable.Select()[0];


            //added by YAMA 2014/04/22
            // 多重起動コンボボックスの設定

            //DataTableオブジェクトを用意
            DataTable CombData = new DataTable();

            //DataTableに列を追加
            CombData.Columns.Add("ID", typeof(string));
            CombData.Columns.Add("ITEM", typeof(string));

            DataRow CombDataRow = CombData.NewRow();

            //各列に値をセット
            CombDataRow["ID"] = "0";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type1;
            CombData.Rows.Add(CombDataRow);

            CombDataRow = CombData.NewRow();
            CombDataRow["ID"] = "1";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type2;
            CombData.Rows.Add(CombDataRow);

            CombDataRow = CombData.NewRow();
            CombDataRow["ID"] = "2";
            CombDataRow["ITEM"] = Properties.Resources.multiple_start_type3;
            CombData.Rows.Add(CombDataRow);

            combMultipleStart.Items.Clear();
            combMultipleStart.ItemsSource = CombData.DefaultView;
            combMultipleStart.DisplayMemberPath = "ITEM";
            combMultipleStart.SelectedValuePath = "ID";

            //コピー新規の場合、採番したデフォルトＩＤをセット
            if (_editType == Consts.EditType.CopyNew)
            {
                tbxJobNetId.Text = "JOBNET_" + DBUtil.GetNextId("100");
                // ジョブネット名をセット
                tbJobnetName.Text = Convert.ToString(row["jobnet_name"]);

                //added by YAMA 2014/04/22
                // ジョブネットの多重起動の有無
                combMultipleStart.SelectedValue = "0";

            }
            else
            {
                // ジョブネットIDをセット
                tbxJobNetId.Text = Convert.ToString(row["jobnet_id"]);
                // ジョブネットIDのテキストボックスをグレーアウトし、編集不可の状態とする
                tbxJobNetId.IsEnabled = false;
                // ジョブネット名をセット
                tbJobnetName.Text = oldJobnetName = Convert.ToString(row["jobnet_name"]);

                //added by YAMA 2014/04/22
                // ジョブネットの多重起動の有無
                combMultipleStart.SelectedValue = Convert.ToString(row["multiple_start_up"]);

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

            // ジョブネットタイムアウト
            tbJobnetTimeout.Text = oldJobnetTimeout = Convert.ToString(row["jobnet_timeout"]);

            // ジョブネットタイムアウト実行タイプ
            String runType = Convert.ToString(row["timeout_run_type"]);
            if ("".Equals(runType))
            {
                combJobNetRunType.SelectedIndex = oldJobnetTimeoutRunType = 0;
            }
            else
            {
                combJobNetRunType.SelectedIndex = oldJobnetTimeoutRunType = Convert.ToInt32(runType);

            }

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
            // ジョブネット名を取得
            string jobnetName = tbJobnetName.Text.Trim();

            // ジョブネット名が未入力の場合
            if (CheckUtil.IsNullOrEmpty(jobnetName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_jobnet_name });
                return false;
            }

            // バイト数＞64の場合
            if (CheckUtil.IsLenOver(jobnetName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_jobnet_name, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(jobnetName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { Properties.Resources.err_message_jobnet_name});
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

            //TimeoutCheck(1-9999)

            string timeOut = tbJobnetTimeout.Text.Trim();
            if (!CheckUtil.IsHankakuNum(timeOut))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007,
                    new string[] { Properties.Resources.memo_label_jobnet_timeout });
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
            string jobnetId = tbxJobNetId.Text;
            // ジョブネットIDが未入力の場合
            if (CheckUtil.IsNullOrEmpty(jobnetId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { Properties.Resources.err_message_jobnet_id });
                return false;
            }

            // 半角英数値、「-」、「_」以外文字以外の場合
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(jobnetId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { Properties.Resources.err_message_jobnet_id });
                return false;
            }

            // ジョブネットIDの桁数＞32の場合
            if (CheckUtil.IsLenOver(jobnetId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { Properties.Resources.err_message_jobnet_id, "32" });
                return false;
            }

            // すでに登録済みのジョブネットIDが指定された場合
            dbAccess.CreateSqlConnect();
            int count = Convert.ToInt16(_jobnetControlDAO.GetCountForCheck(jobnetId));
            dbAccess.CloseSqlConnect();

            if (count >= 1)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                    new string[] { Properties.Resources.err_message_jobnet_id });
                return false;
            }

            return true;
        }

        //*******************************************************************
        //CheckParallelEnd
        //*******************************************************************
        private bool CheckParallelEnd()
        {
            bool result = true;
            int parallelStartCount=0;
            int parallelEndCount=0;

            foreach(string key in container.JobItems.Keys)
            {
                String jobId = key;
                CommonItem room = (CommonItem)container.JobItems[jobId];
                ElementType JobType = room.ElementType;
                if(JobType == ElementType.MTS)
                {
                    parallelStartCount++;
                }
                if(JobType == ElementType.MTE)
                {
                    parallelEndCount++;
                }
            }
            if(parallelStartCount != parallelEndCount)
            {
                if (parallelStartCount > parallelEndCount)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_016);
                }
                else
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_017);
                }
            }
            return result;
        }

        //*******************************************************************
        /// <summary>設定チェック </summary>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool SettingCheck()
        {
            bool result = true;

            foreach (string key in container.JobItems.Keys)
            {
                String jobId = key;
                CommonItem room = (CommonItem)container.JobItems[jobId];
                ElementType JobType = room.ElementType;

                Consts.EditType jobEditType = room.ItemEditType;
                switch (JobType)
                {
                    // 0:開始、1:終了、5:ジョブネット、6:並行処理開始、7：並行処理終了、8：ループ、13:分岐終了の場合
                    case ElementType.START:
                    case ElementType.END:
                    case ElementType.JOBNET:
                    case ElementType.LOOP:
                    case ElementType.MTS:
                    case ElementType.MTE:
                    case ElementType.IFE:
                        break;
                    // その以外の場合
                    default:
                       if (!container.SetedJobIds.Contains(jobId))
                        {
                            CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_010,
                                new string[] { jobId });
                            return false;
                        }
                        break;
                }
            }
            return result;
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
                _jobnetControlDAO.DeleteByPk(_jobnetId, _oldUpdateDate);
            }
        }

        //*******************************************************************
        /// <summary>全てのバージョンの公開フラグを更新</summary>
        /// <param name="publicFlg">公開フラグ</param>
        //*******************************************************************
        private void UpdatePublicFlg(int publicFlg)
        {
            if (_jobnetTbl != null)
                foreach (DataRow row in _jobnetTbl.Select())
                    row["public_flag"] = publicFlg;
        }

        //*******************************************************************
        /// <summary>データ登録</summary>
        //*******************************************************************
        private void RegistDataTable()
        {
            // 同一のジョブネットの公開フラグを更新
            if (_jobnetTbl != null)
                dbAccess.ExecuteNonQuery(_jobnetTbl, _jobnetControlDAO);
            // ジョブネット管理テーブル
            dbAccess.ExecuteNonQuery(container.JobnetControlTable, _jobnetControlDAO);
            // ジョブ管理テーブル
            dbAccess.ExecuteNonQuery(container.JobControlTable, _jobControlDAO);
            // フロー管理テーブル
            dbAccess.ExecuteNonQuery(container.FlowControlTable, _flowControlDAO);
            // 計算アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconCalcTable, _iconCalcDAO);
            // 終了アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconEndTable, _iconEndDAO);
            // 拡張ジョブアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconExtjobTable, _iconExtJobDAO);
            // 条件分岐アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconIfTable, _iconIfDAO);
            // 情報取得アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconInfoTable, _iconInfoDAO);
            // ジョブネットアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconJobnetTable, _iconJobnetDAO);
            // ジョブアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconJobTable, _iconJobDAO);
            // ジョブコマンド設定テーブル
            dbAccess.ExecuteNonQuery(container.JobCommandTable, _jobCommandDAO);
            // ジョブ変数設定テーブル
            dbAccess.ExecuteNonQuery(container.ValueJobTable, _valueJobDAO);
            // ジョブコントローラ変数設定テーブル
            dbAccess.ExecuteNonQuery(container.ValueJobConTable, _valueJobConDAO);
            // タスクアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconTaskTable, _iconTaskDAO);
            // ジョブコントローラ変数アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconValueTable, _iconValueDAO);
            // ジョブコントローラ変数定義テーブル
            //dbAccess.ExecuteNonQuery(container.DefineValueJobconTable, _defineValueDAO);
            // 拡張ジョブ定義テーブル
            //dbAccess.ExecuteNonQuery(container.DefineExtJobTable, _defineExtJobDAO);
            // ファイル転送アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconFcopyTable, _iconFcopyDAO);
            // ファイル待ち合わせアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconFwaitTable, _iconFwaitDAO);
            // リブートアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconRebootTable, _iconRebootDAO);
            // 保留解除アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconReleaseTable, _iconReleaseDAO);

            //added by YAMA 2014/02/06
            // Zabbix連携アイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconCooperationTable, _iconCooperationDAO);
            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            dbAccess.ExecuteNonQuery(container.IconAgentlessTable, _iconAgentlessDAO);
        }

        //*******************************************************************
        /// <summary>DataSetをセット（編集状態判定用）</summary>
        //*******************************************************************
        private void SetDataSet()
        {
            // dataSetにセット
            dataSet.Tables.Add(container.JobnetControlTable);
            dataSet.Tables.Add(container.JobControlTable);
            dataSet.Tables.Add(container.FlowControlTable);
            dataSet.Tables.Add(container.IconCalcTable);
            dataSet.Tables.Add(container.IconEndTable);
            dataSet.Tables.Add(container.IconExtjobTable);
            dataSet.Tables.Add(container.IconIfTable);
            dataSet.Tables.Add(container.IconInfoTable);
            dataSet.Tables.Add(container.IconJobnetTable);
            dataSet.Tables.Add(container.IconJobTable);
            dataSet.Tables.Add(container.JobCommandTable);
            dataSet.Tables.Add(container.ValueJobTable);
            dataSet.Tables.Add(container.ValueJobConTable);
            dataSet.Tables.Add(container.IconTaskTable);
            dataSet.Tables.Add(container.IconValueTable);
            dataSet.Tables.Add(container.IconFcopyTable);
            dataSet.Tables.Add(container.IconFwaitTable);
            dataSet.Tables.Add(container.IconRebootTable);
            dataSet.Tables.Add(container.IconReleaseTable);

            //added by YAMA 2014/02/06
            dataSet.Tables.Add(container.IconCooperationTable);
            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            dataSet.Tables.Add(container.IconAgentlessTable);
        }

        //*******************************************************************
        /// <summary>DataSetをリセット（UNDO用）</summary>
        //*******************************************************************
        public void ResetDataSet()
        {
            dataSet.Tables.Clear();
            // dataSetにセット
            dataSet.Tables.Add(container.JobnetControlTable);
            dataSet.Tables.Add(container.JobControlTable);
            dataSet.Tables.Add(container.FlowControlTable);
            dataSet.Tables.Add(container.IconCalcTable);
            dataSet.Tables.Add(container.IconEndTable);
            dataSet.Tables.Add(container.IconExtjobTable);
            dataSet.Tables.Add(container.IconIfTable);
            dataSet.Tables.Add(container.IconInfoTable);
            dataSet.Tables.Add(container.IconJobnetTable);
            dataSet.Tables.Add(container.IconJobTable);
            dataSet.Tables.Add(container.JobCommandTable);
            dataSet.Tables.Add(container.ValueJobTable);
            dataSet.Tables.Add(container.ValueJobConTable);
            dataSet.Tables.Add(container.IconTaskTable);
            dataSet.Tables.Add(container.IconValueTable);
            dataSet.Tables.Add(container.IconFcopyTable);
            dataSet.Tables.Add(container.IconFwaitTable);
            dataSet.Tables.Add(container.IconRebootTable);
            dataSet.Tables.Add(container.IconReleaseTable);


            //added by YAMA 2014/02/06
            dataSet.Tables.Add(container.IconCooperationTable);
            //added by YAMA 2014/05/19
            /// エージェントレスアイコン設定テーブル
            dataSet.Tables.Add(container.IconAgentlessTable);

            // 目的：Rowstateをセット
            dataSet.AcceptChanges();

            // RowstateをAddにセット
            SetAllRowsState();
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

        //*******************************************************************
        /// <summary>登録後表示</summary>
        //*******************************************************************
        private void AfterRegistView()
        {
            if (_isSubJobnet)
            {
                ResetTree(null);
                Window.GetWindow(this).Close();
            }
            else
            {
                ResetTree(tbxJobNetId.Text);
                ParantWindow.ShowObjectList(tbxJobNetId.Text, Consts.ObjectEnum.JOBNET);
            }
        }

        #endregion
    }
}
