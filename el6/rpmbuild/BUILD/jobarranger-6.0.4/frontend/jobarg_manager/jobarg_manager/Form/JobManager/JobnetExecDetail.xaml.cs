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
using jp.co.ftf.jobcontroller.Common;
using System.Data;
using System.Text;
using System.Collections.Generic;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM  2012/11/13 新規作成<BR>                    　　   *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    /// <summary>
    /// JobEdit.xaml の相互作用ロジック
    /// </summary>
    public partial class JobnetExecDetail : BaseWindow
    {
        #region フィールド


        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect _dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

        /// <summary> 実行ジョブネットサマリーテーブル </summary>
        private RunJobnetSummaryDAO _runJobnetSummaryDAO;

        /// <summary> 実行ジョブネット管理テーブル </summary>
        private RunJobnetDAO _runJobnetDAO;

        /// <summary> 実行ジョブ管理テーブル </summary>
        private RunJobDAO _runJobControlDAO;

        /// <summary> 実行フロー管理テーブル </summary>
        private RunFlowDAO _runFlowControlDAO;

        /// <summary> 実行計算アイコン設定テーブル </summary>
        private RunIconCalcDAO _runIconCalcDAO;

        /// <summary> 実行終了アイコン設定テーブル </summary>
        private RunIconEndDAO _runIconEndDAO;

        /// <summary> 実行拡張実行ジョブアイコン設定テーブル </summary>
        private RunIconExtJobDAO _runIconExtJobDAO;

        /// <summary> 実行条件分岐アイコン設定テーブル </summary>
        private RunIconIfDAO _runIconIfDAO;

        /// <summary> 情報取得アイコン設定テーブル </summary>
        private RunIconInfoDAO _runIconInfoDAO;

        /// <summary> 実行ジョブネットアイコン設定テーブル </summary>
        private RunIconJobnetDAO _runIconJobnetDAO;

        /// <summary> 実行ジョブアイコン設定テーブル </summary>
        private RunIconJobDAO _runIconJobDAO;

        /// <summary> 実行ジョブコマンド設定テーブル </summary>
        private RunJobCommandDAO _jobCommandDAO;

        /// <summary> 実行ジョブ変数設定テーブル </summary>
        private RunValueJobDAO _valueJobDAO;

        /// <summary> 実行ジョブコントローラ変数設定テーブル </summary>
        private RunValueJobConDAO _valueJobConDAO;

        /// <summary> 実行タスクアイコン設定テーブル </summary>
        private RunIconTaskDAO _runIconTaskDAO;

        /// <summary> 実行ジョブコントローラ変数アイコン設定テーブル </summary>
        private RunIconValueDAO _runIconValueDAO;

        /// <summary> ジョブコントローラ変数定義テーブル </summary>
        private DefineValueDAO _defineValueDAO;

        /// <summary> 拡張ジョブ定義テーブル </summary>
        private DefineExtJobDAO _defineExtJobDAO;

        /// <summary> 実行ファイル転送アイコン設定テーブル </summary>
        private RunIconFcopyDAO _runIconFcopyDAO;

        /// <summary> 実行ファイル待ち合わせアイコン設定テーブル </summary>
        private RunIconFwaitDAO _runIconFwaitDAO;

        /// <summary> 実行リブートアイコン設定テーブル </summary>
        private RunIconRebootDAO _runIconRebootDAO;

        /// <summary> 実行保留解除アイコン設定テーブル </summary>
        private RunIconReleaseDAO _runIconReleaseDAO;

        //added by YAMA 2014/02/06
        /// <summary> 実行Zabbix連携アイコン設定テーブル </summary>
        private RunIconCooperationDAO _runIconCooperationDAO;

        //added by YAMA 2014/05/19
        /// <summary> 実行エージェントレスアイコン設定テーブル </summary>
        private RunIconAgentlessDAO _runIconAgentlessDAO;

        /// <summary>実行ジョブネットサマリ管理テーブル</summary>
        public DataTable RunJobnetSummaryTable { get; set; }

        /// <summary> DispatcherTimer </summary>
        public DispatcherTimer dispatcherTimer;

        /// <summary> フォーカス強制処理 </summary>
        private bool _needFocus;

        /// <summary>エラーが発生したアイコン</summary>
        private HashSet<CommonItem> errorItems = new HashSet<CommonItem>();

        /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
        /// <summary> エラーダイアログを表示するか </summary>
        private bool _isDb;    // true：表示する

        #endregion

        #region コンストラクタ


        /// <summary>コンストラクタ(編集、コピー新規用)</summary>
        /// <param name="innerJobnetId">実行ジョブネットID</param>
        public JobnetExecDetail(string innerJobnetId, bool needFocus)
        {
            if (LoadForUpd(innerJobnetId))
            {
                InitializeComponent();
                _successFlg = true;
            }
            else
                _successFlg = false;

            _needFocus = needFocus;

            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            _isDb = true;

        }

        #endregion

        #region プロパティ
        /// <summary>実行ジョブネットID</summary>
        private string _innerJobnetId;
        public string InnerJobnetId
        {
            get
            {
                return _innerJobnetId;
            }
            set
            {
                _innerJobnetId = value;
            }
        }

        /// <summary>ウィンドウ</summary>
        JobArrangerWindow _parantWindow;
        public JobArrangerWindow ParantWindow
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
                return "JobnetExecDetail";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_310;
            }
        }

        /// <summary>成功フラグ</summary>
        private bool _successFlg = false;
        public bool SuccessFlg
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

        /// <summary>ジョブネット実行ステータス</summary>
        RunJobStatusType _jobnetRunStatus;
        public RunJobStatusType JobnetRunStatus
        {
            get
            {
                return _jobnetRunStatus;
            }
            set
            {
                _jobnetRunStatus = value;
            }
        }


        #endregion

        #region イベント
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer.Stop();
            _dbAccess.CloseSqlConnect();
            _dbAccess = null;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void refresh(object sender, EventArgs e)
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            DBException dbEx = _dbAccess.exExecuteHealthCheck();

            if (dbEx.MessageID.Equals(""))
            {
                FillTables(_innerJobnetId);
                ResetColor();
                ResetToolTip();
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }
            //FillTables(_innerJobnetId);
            //ResetColor();
            //ResetToolTip();
        }

        private void jobnetItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IRoom room = sender as IRoom;
            if (room != null)
            {
                DataRow[] rows = container.IconJobnetTable.Select("inner_job_id=" + room.ContentItem.InnerJobId);
                JobnetExecDetail jobnetDetail = new JobnetExecDetail(Convert.ToString(rows[0]["link_inner_jobnet_id"]),true);

                jobnetDetail.Topmost = true;
                jobnetDetail.Show();
            }
        }

        /// <summary>部品をダブルクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        private void UserControl_MouseDoubleClick4Read(object sender, MouseButtonEventArgs e)
        {
            CommonItem room = (CommonItem)sender;
            room.ShowIconSetting(false);
        }

        /// <summary>フォーカスロス</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Window_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_needFocus)
            {
                this.Focusable = true;
                Keyboard.Focus(this);
                _needFocus=false;
            }
            else
            {
                this.Topmost = false;
            }
        }

        /// <summary>フォーカス取得</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Window_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            this.Topmost = false;
        }
        #endregion

        #region privateメソッド

        //*******************************************************************
        /// <summary>初期化（編集、コピー新規用）</summary>
        /// <param name="innerJobnetId">実行ジョブネットID</param>
        /// <param name="updDate">更新日</param>
        //*******************************************************************
        private bool LoadForUpd(string innerJobnetId)
        {
            // DAOの初期化
            InitialDAO();

            // トランザクションを開始
            _dbAccess.CreateSqlConnect();

            // 画面の初期化
            InitializeComponent();

            // 各テーブルのデータをコピー追加
            FillTables(innerJobnetId);

            //　実行ジョブIDをセット
            _innerJobnetId = innerJobnetId;
            container.JobnetId = innerJobnetId;

            container.ParantWindow = this;

            // 情報エリアの表示
            SetInfoArea();

            // 実行ジョブフロー領域の表示
            ShowJobNet();

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(refresh);
            dispatcherTimer.Start();


            return true;
        }

        //*******************************************************************
        /// <summary> DAOの初期化処理</summary>
        //*******************************************************************
        private void InitialDAO()
        {
            // 実行ジョブネット管理テーブル
            _runJobnetSummaryDAO = new RunJobnetSummaryDAO(_dbAccess);

            // 実行ジョブネット管理テーブル
            _runJobnetDAO = new RunJobnetDAO(_dbAccess);

            // 実行ジョブ管理テーブル
            _runJobControlDAO = new RunJobDAO(_dbAccess);

            // フロー管理テーブル
            _runFlowControlDAO = new RunFlowDAO(_dbAccess);

            // 計算アイコン設定テーブル
            _runIconCalcDAO = new RunIconCalcDAO(_dbAccess);

            // 終了アイコン設定テーブル
            _runIconEndDAO = new RunIconEndDAO(_dbAccess);

            /// 拡張実行ジョブアイコン設定テーブル
            _runIconExtJobDAO = new RunIconExtJobDAO(_dbAccess);

            /// 条件分岐アイコン設定テーブル
            _runIconIfDAO = new RunIconIfDAO(_dbAccess);

            /// 情報取得アイコン設定テーブル
            _runIconInfoDAO = new RunIconInfoDAO(_dbAccess);

            /// 実行ジョブネットアイコン設定テーブル
            _runIconJobnetDAO = new RunIconJobnetDAO(_dbAccess);

            /// 実行ジョブアイコン設定テーブル
            _runIconJobDAO = new RunIconJobDAO(_dbAccess);

            /// 実行ジョブコマンド設定テーブル
            _jobCommandDAO = new RunJobCommandDAO(_dbAccess);

            /// 実行ジョブ変数設定テーブル
            _valueJobDAO = new RunValueJobDAO(_dbAccess);

            /// 実行ジョブコントローラ変数設定テーブル
            _valueJobConDAO = new RunValueJobConDAO(_dbAccess);

            /// タスクアイコン設定テーブル
            _runIconTaskDAO = new RunIconTaskDAO(_dbAccess);

            /// 実行ジョブコントローラ変数アイコン設定テーブル
            _runIconValueDAO = new RunIconValueDAO(_dbAccess);

            /// 実行ジョブコントローラ変数定義テーブル
            _defineValueDAO = new DefineValueDAO(_dbAccess);

            /// 拡張実行ジョブ定義テーブル
            _defineExtJobDAO = new DefineExtJobDAO(_dbAccess);

            /// 実行ファイル転送アイコン設定テーブル
            _runIconFcopyDAO = new RunIconFcopyDAO(_dbAccess);

            /// 実行ファイル待ち合わせアイコン設定テーブル
            _runIconFwaitDAO = new RunIconFwaitDAO(_dbAccess);

            /// 実行リブートアイコン設定テーブル
            _runIconRebootDAO = new RunIconRebootDAO(_dbAccess);

            /// 実行リブートアイコン設定テーブル
            _runIconReleaseDAO = new RunIconReleaseDAO(_dbAccess);

            //added by YAMA 2014/02/06
            /// 実行Zabbix連携アイコン設定テーブル
            _runIconCooperationDAO = new RunIconCooperationDAO(_dbAccess);

            //added by YAMA 2014/05/19
            /// 実行エージェントレスアイコン設定テーブル
            _runIconAgentlessDAO = new RunIconAgentlessDAO(_dbAccess);
        }



        //*******************************************************************
        /// <summary> 実行ジョブネットデータの検索（編集、コピー新規用）</summary>
        /// <param name="innerJobnetId">`実行ジョブネットID</param>
        /// <param name="updDate">`更新日</param>
        //*******************************************************************
        private void FillTables(string innerJobnetId)
        {
            /* added by YAMA 2014/12/09    V2.1.0 No23 対応 */
            DBException dbEx = _dbAccess.exExecuteHealthCheck();

            if (dbEx.MessageID.Equals(""))
            {
                RunJobnetSummaryTable = _runJobnetSummaryDAO.GetEntityByPk(innerJobnetId);
                if (RunJobnetSummaryTable.Rows.Count > 0) JobnetRunStatus = (RunJobStatusType)RunJobnetSummaryTable.Rows[0]["status"];
                // 実行ジョブネット管理テーブル
                container.JobnetControlTable = _runJobnetDAO.GetEntityByPk(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブ管理テーブル
                container.JobControlTable = null;
                container.JobControlTable = _runJobControlDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // フロー管理テーブル
                container.FlowControlTable = _runFlowControlDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 計算アイコン設定テーブル
                container.IconCalcTable = _runIconCalcDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 終了アイコン設定テーブル
                container.IconEndTable = _runIconEndDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 拡張実行ジョブアイコン設定テーブル
                container.IconExtjobTable = _runIconExtJobDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 条件分岐アイコン設定テーブル
                container.IconIfTable = _runIconIfDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 情報取得アイコン設定テーブル
                container.IconInfoTable = _runIconInfoDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブネットアイコン設定テーブル
                container.IconJobnetTable = _runIconJobnetDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブアイコン設定テーブル
                container.IconJobTable = _runIconJobDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブコマンド設定テーブル
                container.JobCommandTable = _jobCommandDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブ変数設定テーブル
                container.ValueJobTable = _valueJobDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブコントローラ変数設定テーブル
                container.ValueJobConTable = _valueJobConDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // タスクアイコン設定テーブル
                container.IconTaskTable = _runIconTaskDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ジョブコントローラ変数アイコン設定テーブル
                container.IconValueTable = _runIconValueDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ファイル転送アイコン設定テーブル
                container.IconFcopyTable = _runIconFcopyDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行ファイル待ち合わせアイコン設定テーブル
                container.IconFwaitTable = _runIconFwaitDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行リブートアイコン設定テーブル
                container.IconRebootTable = _runIconRebootDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                // 実行保留解除アイコン設定テーブル
                container.IconReleaseTable = _runIconReleaseDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                //added by YAMA 2014/02/06
                // 実行Zabbix連携アイコン設定テーブル
                container.IconCooperationTable = _runIconCooperationDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }

            dbEx = _dbAccess.exExecuteHealthCheck();
            if (dbEx.MessageID.Equals(""))
            {
                //added by YAMA 2014/05/19
                /// 実行エージェントレスアイコン設定テーブル
                container.IconAgentlessTable = _runIconAgentlessDAO.GetEntityByJobnet(innerJobnetId);
            }
            else
            {
                if (_isDb)
                {
                    _isDb = false;
                    LogInfo.WriteErrorLog(Consts.SYSERR_001, dbEx.InnerException);
                    CommonDialog.ShowErrorDialog(Consts.SYSERR_001);
                }
            }
            /*
            RunJobnetSummaryTable = _runJobnetSummaryDAO.GetEntityByPk(innerJobnetId);
            if (RunJobnetSummaryTable.Rows.Count > 0) JobnetRunStatus = (RunJobStatusType)RunJobnetSummaryTable.Rows[0]["status"];
            // 実行ジョブネット管理テーブル
            container.JobnetControlTable = _runJobnetDAO.GetEntityByPk(innerJobnetId);

            // 実行ジョブ管理テーブル
            container.JobControlTable = null;
            container.JobControlTable = _runJobControlDAO.GetEntityByJobnet(innerJobnetId);
            // フロー管理テーブル
            container.FlowControlTable = _runFlowControlDAO.GetEntityByJobnet(innerJobnetId);

            // 計算アイコン設定テーブル
            container.IconCalcTable = _runIconCalcDAO.GetEntityByJobnet(innerJobnetId);

            //container.IconCalcTable.Rows
            // 終了アイコン設定テーブル
            container.IconEndTable = _runIconEndDAO.GetEntityByJobnet(innerJobnetId);
            // 拡張実行ジョブアイコン設定テーブル
            container.IconExtjobTable = _runIconExtJobDAO.GetEntityByJobnet(innerJobnetId);

            // 条件分岐アイコン設定テーブル
            container.IconIfTable = _runIconIfDAO.GetEntityByJobnet(innerJobnetId);

            // 情報取得アイコン設定テーブル
            container.IconInfoTable = _runIconInfoDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブネットアイコン設定テーブル
            container.IconJobnetTable = _runIconJobnetDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブアイコン設定テーブル
            container.IconJobTable = _runIconJobDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブコマンド設定テーブル
            container.JobCommandTable = _jobCommandDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブ変数設定テーブル
            container.ValueJobTable = _valueJobDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブコントローラ変数設定テーブル
            container.ValueJobConTable = _valueJobConDAO.GetEntityByJobnet(innerJobnetId);

            // タスクアイコン設定テーブル
            container.IconTaskTable = _runIconTaskDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ジョブコントローラ変数アイコン設定テーブル
            container.IconValueTable = _runIconValueDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ファイル転送アイコン設定テーブル
            container.IconFcopyTable = _runIconFcopyDAO.GetEntityByJobnet(innerJobnetId);

            // 実行ファイル待ち合わせアイコン設定テーブル
            container.IconFwaitTable = _runIconFwaitDAO.GetEntityByJobnet(innerJobnetId);

            // 実行リブートアイコン設定テーブル
            container.IconRebootTable = _runIconRebootDAO.GetEntityByJobnet(innerJobnetId);

            // 実行保留解除アイコン設定テーブル
            container.IconReleaseTable = _runIconReleaseDAO.GetEntityByJobnet(innerJobnetId);

            //added by YAMA 2014/02/06
            // 実行Zabbix連携アイコン設定テーブル
            container.IconCooperationTable = _runIconCooperationDAO.GetEntityByJobnet(innerJobnetId);

            //added by YAMA 2014/05/19
            /// 実行エージェントレスアイコン設定テーブル
            container.IconAgentlessTable = _runIconAgentlessDAO.GetEntityByJobnet(innerJobnetId);
            */
        }

        //*******************************************************************
        /// <summary>実行ジョブフロー領域の表示</summary>
        //*******************************************************************
        private void ShowJobNet()
        {
            container.ContainerCanvas.Children.Clear();
            container.JobItems.Clear();
            // 実行ジョブデータ（実行ジョブアイコンの生成用）
            JobData jobData = null;
            // 実行ジョブを表示------------------
            foreach (DataRow row in container.JobControlTable.Select())
            {
                jobData = new JobData();
                // 実行ジョブタイプ
                jobData.JobType = (ElementType)row["job_type"];
                SolidColorBrush iconColor = getIconColor(row);

                //added by YAMA 2014/07/01
                //CommonItem room = new CommonItem(container, jobData, Consts.EditType.READ, iconColor);
                SolidColorBrush characterColor = getCharacterColor(row);
                CommonItem room = new CommonItem(container, jobData, Consts.EditType.READ, iconColor, characterColor);

                // 実行ジョブID
                room.JobId = Convert.ToString(row["job_id"]);
                // 実行内部ジョブID
                room.InnerJobId = Convert.ToString(row["inner_job_id"]);
                //実行ジョブ名
                room.JobName = Convert.ToString(row["job_name"]);
                // X位置
                room.SetValue(Canvas.LeftProperty, Convert.ToDouble(row["point_x"]));
                // Y位置
                room.SetValue(Canvas.TopProperty, Convert.ToDouble(row["point_y"]));

                // ToolTip設定
                room.ContentItem.SetToolTip();

                room.RemoveAllEvent();
                //room.ContextMenu = contextMenu;


                if (jobData.JobType.Equals(ElementType.JOBNET))
                {
                    room.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(jobnetItem_MouseDoubleClick);
                }
                else
                {
                    room.MouseDoubleClick += UserControl_MouseDoubleClick4Read;
                }

                // 実行ジョブフロー領域に追加
                container.ContainerCanvas.Children.Add(room);
                container.JobItems.Add(room.InnerJobId, room);
            }

            // フローを表示------------------
            // 開始実行ジョブID、終了実行ジョブId
            string startJobId, endJobId;
            // 開始実行ジョブ、終了実行ジョブ
            IRoom startJob, endJob;
            // フロー幅
            int flowWidth;
            // フロータイプ(直線、曲線)
            FlowLineType lineType;
            // フロータイプ（　0：通常、　1：TURE、　2：FALSE）
            int flowType = 0;
            foreach (DataRow row in container.FlowControlTable.Select())
            {
                startJobId = Convert.ToString(row["start_inner_job_id"]);
                endJobId = Convert.ToString(row["end_inner_job_id"]);
                flowWidth = Convert.ToInt32(row["flow_width"]);
                flowType = Convert.ToInt32(row["flow_type"]);

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

                container.MakeFlow(lineType, startJob, endJob, flowType, Consts.EditType.READ);
            }

        }

        //*******************************************************************
        /// <summary>実行ジョブフロー領域の表示</summary>
        //*******************************************************************
        private void ResetColor()
        {
            // 実行ジョブを表示------------------
            foreach (DataRow row in container.JobControlTable.Select())
            {
                SolidColorBrush iconColor = getIconColor(row);
                CommonItem room = (CommonItem)container.JobItems[Convert.ToString(row["inner_job_id"])];
                room.SetStatusColor(iconColor);

                //added by YAMA 2014/07/01
                SolidColorBrush characterColor = getCharacterColor(row);
                room.SetStatusCharacterColor(characterColor);

            }

        }

        //*******************************************************************
        /// <summary>実行ジョブフロー領域の表示</summary>
        //*******************************************************************
        private void ResetToolTip()
        {
            string strSqlAfter = "select inner_jobnet_id,value_name,after_value from ja_run_value_after_table where inner_job_id = ?";
            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();
            foreach (DataRow row in container.JobControlTable.Select())
            {
                //added by YAMA 2014/11/12
                //StringBuilder sb = new StringBuilder("Text");
                StringBuilder sb = new StringBuilder();
                RunJobStatusType statusType = (RunJobStatusType)row["status"];
                string innerJobId = Convert.ToString(row["inner_job_id"]);
                CommonItem room = (CommonItem)container.JobItems[innerJobId];

                if (statusType == RunJobStatusType.RunErr || statusType == RunJobStatusType.Abnormal)
                {
                    //added by YAMA 2014/11/12
                    //string stdOut = "";
                    //string stdErr = "";
                    StringBuilder stdOut = new StringBuilder();
                    StringBuilder stdErr = new StringBuilder();

                    string jobExitCd = "";
                    string jobargMessage = "";

                    List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                    sqlParams.Add(new ComSqlParam(DbType.String, "@inner_job_id", innerJobId));

                    DataTable runValueAfterTable = dbAccess.ExecuteQuery(strSqlAfter, sqlParams);
                    DataRow[] rows = runValueAfterTable.Select("value_name='STD_OUT'");
                    if (rows != null && rows.Length > 0)
                    {
                        //added by YAMA 2014/11/12
                        //stdOut = Convert.ToString(rows[0]["after_value"]);
                        string command = Convert.ToString(rows[0]["after_value"]);
                        foreach (string line in command.Split(new Char[] { '\n' }))
                        {
                            stdOut.Append("\n");
                            stdOut.Append("  ");
                            stdOut.Append(line);
                        }
                    }
                    rows = runValueAfterTable.Select("value_name='STD_ERR'");
                    if (rows != null && rows.Length > 0)
                    {
                        //added by YAMA 2014/11/12
                        //stdErr = Convert.ToString(rows[0]["after_value"]);
                        string command = Convert.ToString(rows[0]["after_value"]);
                        foreach (string line in command.Split(new Char[] { '\n' }))
                        {
                            stdErr.Append("\n");
                            stdErr.Append("  ");
                            stdErr.Append(line);
                        }
                    }
                    rows = runValueAfterTable.Select("value_name='JOB_EXIT_CD'");
                    if (rows != null && rows.Length > 0)
                    {
                        jobExitCd = Convert.ToString(rows[0]["after_value"]);
                    }
                    rows = runValueAfterTable.Select("value_name='JOBARG_MESSAGE'");
                    if (rows != null && rows.Length > 0)
                    {
                        jobargMessage = Convert.ToString(rows[0]["after_value"]);
                    }
                    sb.Append(Properties.Resources.tooltip_err_stdout);
                    //added by YAMA 2014/11/12
                    //sb.Append(stdOut);
                    sb.Append(stdOut.ToString());
                    sb.Append("\n");
                    sb.Append(Properties.Resources.tooltip_err_stderr);
                    //added by YAMA 2014/11/12
                    //sb.Append(stdErr);
                    sb.Append(stdErr.ToString());
                    sb.Append("\n");
                    sb.Append(Properties.Resources.tooltip_err_job_exit_cd);
                    sb.Append(jobExitCd);
                    sb.Append("\n");
                    sb.Append(Properties.Resources.tooltip_err_jobarg_message);
                    sb.Append(jobargMessage);
                    room.ContentItem.ResetToolTip(sb.ToString().Trim());
                    errorItems.Add(room);
                }
                else
                {
                    if (errorItems.Contains(room))
                    {
                        errorItems.Remove(room);
                        room.ContentItem.SetToolTip();
                    }
                }
            }
            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            lblManageId.Text = Convert.ToString(container.JobnetControlTable.Rows[0]["inner_jobnet_id"]);
            if (Convert.ToDecimal(container.JobnetControlTable.Rows[0]["scheduled_time"]) > 0)
            {
                lblScheduledTime.Text = ConvertUtil.ConverIntYYYYMMDDHHMI2Date(Convert.ToDecimal(container.JobnetControlTable.Rows[0]["scheduled_time"])).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                lblScheduledTime.Text = "";
            }
            if (Convert.ToDecimal(container.JobnetControlTable.Rows[0]["start_time"]) > 0)
            {
                lblStartTime.Text = ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(container.JobnetControlTable.Rows[0]["start_time"])).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                lblStartTime.Text = "";
            }
            if (Convert.ToDecimal(container.JobnetControlTable.Rows[0]["end_time"]) > 0)
            {
                lblEndTime.Text = ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToDecimal(container.JobnetControlTable.Rows[0]["end_time"])).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                lblEndTime.Text = "";
            }
            DataRow row = container.JobnetControlTable.Select()[0];
            // 実行ジョブネットIDをセット
            lblJobNetId.Text = Convert.ToString(row["jobnet_id"]);

            // 実行ジョブネット名をセット
            lblJobnetName.Text = Convert.ToString(row["jobnet_name"]);

            // 公開チェックボックス
            int openFlg = Convert.ToInt32(row["public_flag"]);
            if (openFlg == 1)
                lblOpen.Text = "○";
            else if (openFlg == 0)
                lblOpen.Text = "";

            // 説明
            lblComment.Text = Convert.ToString(row["memo"]);
            //更新日
            lblUpdDate.Text = (ConvertUtil.ConverIntYYYYMMDDHHMISS2Date(Convert.ToInt64(row["update_date"]))).ToString("yyyy/MM/dd HH:mm:ss");
            //ユーザー名
            lblUserName.Text = Convert.ToString(row["user_name"]);

            //added by YAMA 2014/04/22
            // ジョブネットの多重起動の有無
            switch (Convert.ToInt32(row["multiple_start_up"]))
            {
                case 0:
                    lblmultiple_start.Text = Properties.Resources.multiple_start_type1;
                    break;
                case 1:
                    lblmultiple_start.Text = Properties.Resources.multiple_start_type2;
                    break;
                case 2:
                    lblmultiple_start.Text = Properties.Resources.multiple_start_type3;
                    break;
            }

            //ジョブネットタイムアウト追加
            if (RunJobnetSummaryTable.Rows.Count > 0)
            {
                tbJobnetTimeout.Text = Convert.ToString(RunJobnetSummaryTable.Rows[0]["jobnet_timeout"]);
                if (Convert.ToString(RunJobnetSummaryTable.Rows[0]["timeout_run_type"]).Equals("1"))
                {
                    jobNetRunType.Text = "jobnet stop";
                }
            }
            else
            {
                jobNetRunType.Text = "";
            }


        }

        //*******************************************************************
        /// <summary>アイコン色取得</summary>
        //*******************************************************************
        private SolidColorBrush getIconColor(DataRow row)
        {
            int status = (Int32)row["status"];
            RunJobMethodType method_flag = (RunJobMethodType)row["method_flag"];
            RunJobTimeoutType timeout_flag = (RunJobTimeoutType)row["timeout_flag"];
            SolidColorBrush color = new SolidColorBrush(Colors.Aquamarine);

            switch ((RunJobStatusType)row["status"])
            {
                case RunJobStatusType.None:
                    if (method_flag.Equals(RunJobMethodType.HOLD)) color = new SolidColorBrush(Colors.MediumOrchid);
                    if (method_flag.Equals(RunJobMethodType.SKIP)) color = new SolidColorBrush(Colors.Gray);
                    break;
                case RunJobStatusType.Prepare:
                    if (method_flag.Equals(RunJobMethodType.HOLD)) color = new SolidColorBrush(Colors.MediumOrchid);
                    if (method_flag.Equals(RunJobMethodType.SKIP)) color = new SolidColorBrush(Colors.Gray);
                    break;
                case RunJobStatusType.During:
                    color = new SolidColorBrush(Colors.Yellow);
                    if (timeout_flag.Equals(RunJobTimeoutType.TIMEOUT)) color = new SolidColorBrush(Colors.Orange);
                    break;
                case RunJobStatusType.Normal:
                    color = new SolidColorBrush(Colors.Lime);
                    if (method_flag.Equals(RunJobMethodType.SKIP))
                    {
                        color = new SolidColorBrush(Colors.Gray);
                    }
                    else if (timeout_flag.Equals(RunJobTimeoutType.TIMEOUT))
                    {
                        color = new SolidColorBrush(Colors.Orange);
                    }
                    break;
                case RunJobStatusType.RunErr:
                    color = new SolidColorBrush(Colors.Red);
                    break;
                case RunJobStatusType.Abnormal:
                    color = new SolidColorBrush(Colors.Red);
                    break;
                case RunJobStatusType.ForceStop:
                    color = new SolidColorBrush(Colors.Yellow);
                    if (timeout_flag.Equals(RunJobTimeoutType.TIMEOUT)) color = new SolidColorBrush(Colors.Orange);
                    break;

            }
            return color;
        }


        //added by YAMA 2014/07/01
        //*******************************************************************
        /// <summary>文字色取得</summary>
        //*******************************************************************
        private SolidColorBrush getCharacterColor(DataRow row)
        {
            int status = (Int32)row["status"];
            RunJobMethodType method_flag = (RunJobMethodType)row["method_flag"];
            RunJobTimeoutType timeout_flag = (RunJobTimeoutType)row["timeout_flag"];
            SolidColorBrush color = new SolidColorBrush(Colors.Aquamarine);

            color = new SolidColorBrush(Colors.Black);

            switch ((RunJobStatusType)row["status"])
            {
                case RunJobStatusType.None:
                    if (method_flag.Equals(RunJobMethodType.SKIP))
                    {
                        color = new SolidColorBrush(Colors.White);
                    }
                    break;
                case RunJobStatusType.Prepare:
                    if (method_flag.Equals(RunJobMethodType.SKIP))
                    {
                        color = new SolidColorBrush(Colors.White);
                    }
                    break;
                case RunJobStatusType.During:
                    break;
                case RunJobStatusType.Normal:
                    if (method_flag.Equals(RunJobMethodType.SKIP))
                    {
                        color = new SolidColorBrush(Colors.White);
                    }
                    break;
                case RunJobStatusType.RunErr:
                    break;
                case RunJobStatusType.Abnormal:
                    break;
                case RunJobStatusType.ForceStop:
                    break;
            }
            return color;
        }


        #endregion
    }
}
