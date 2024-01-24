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
//  * @author DHC 劉 偉 2012/11/05 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.ScheduleEdit
{
    /// <summary>
    /// JobArrangerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScheduleJobnetRegistWindow : BaseWindow
    {
        #region フィールド

        /// <summary>DBアクセスインスタンス</summary>
        private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);

        /// <summary>全部テーブル格納場所（編集状態判定用） </summary>
        private DataSet dataSet = new DataSet();

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
        /// <summary> 実行エージェントレスアイコン設定テーブル </summary>
        private IconAgentlessDAO _iconAgentlessDAO;

        #endregion

        #region コンストラクタ
        public ScheduleJobnetRegistWindow(Container parantContainer)
        {
            InitializeComponent();
            LoadForInit(parantContainer);
            treeViewJobNet.IsSelected = true;

        }
        #endregion

        #region プロパティ
        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "ScheduleJobnetRegistWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_222;
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
        /// <summary>ジョブネットＩＤ</summary>
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

        #endregion

        #region イベント



        /// <summary>オブジェクトを編集</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            _jobnetId = ((TreeViewItem)sender).Header.ToString();

            // 各テーブルのデータをコピー追加
            FillTables(_jobnetId);
            // 情報エリアの表示
            SetInfoArea();
            // ジョブフロー領域の表示
            ShowJobNet();
            btnRegist.IsEnabled = true;
            e.Handled = true;

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

            DataRow[] rows = ParentContainer.ScheduleJobnetTable.Select("jobnet_id='" + JobnetId + "'");

            if (rows.Length < 1)
            {
                DataRow row = ParentContainer.ScheduleJobnetTable.NewRow();
                row["jobnet_id"] = JobnetId;
                row["schedule_id"] = ParentContainer.ScheduleId;
                ParentContainer.ScheduleJobnetTable.Rows.Add(row);
                JobnetControlDAO jobnetControlDAO = new JobnetControlDAO(dbAccess);
                DataTable dt = jobnetControlDAO.GetValidORMaxUpdateDateEntityById(_jobnetId);
                row["jobnet_name"] = dt.Rows[0]["jobnet_name"];

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
            base.WriteStartLog("cancel_Click", Consts.PROCESS_002);

            this.Close();

            // 終了ログ
            base.WriteEndLog("cancel_Click", Consts.PROCESS_002);
        }

        /// <summary>公開ジョブネットを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Public_Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeJobnet(true);
        }


        /// <summary>プライベートジョブネットを展開</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Private_Jobnet_Selected(object sender, RoutedEventArgs e)
        {
            SetTreeJobnet(false);
        }

        #endregion

        #region private メッソド
        //*******************************************************************
        /// <summary>ジョブフロー領域の表示</summary>
        //*******************************************************************
        private void ShowJobNet()
        {
            container.ContainerCanvas.Children.Clear();
            container.JobItems.Clear();

            // ジョブデータ（ジョブアイコンの生成用）
            JobData jobData = null;
            // ジョブを表示------------------
            foreach (DataRow row in container.JobControlTable.Select())
            {
                jobData = new JobData();
                // ジョブタイプ
                jobData.JobType = (ElementType)row["job_type"];

                CommonItem room = new CommonItem(container, jobData, Consts.EditType.Modify, (RunJobMethodType)row["method_flag"]);
                // ジョブID
                room.JobId = Convert.ToString(row["job_id"]);
                //ジョブ名
                room.JobName = Convert.ToString(row["job_name"]);
                // X位置
                room.SetValue(Canvas.LeftProperty, Convert.ToDouble(row["point_x"]));
                // Y位置
                room.SetValue(Canvas.TopProperty, Convert.ToDouble(row["point_y"]));

                room.IsEnabled = false;

                // ジョブフロー領域に追加
                container.ContainerCanvas.Children.Add(room);
                container.JobItems.Add(room.JobId, room);
            }

            // フローを表示------------------
            // 開始ジョブID、終了ジョブId
            string startJobId, endJobId;
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

                container.MakeFlow(lineType, startJob, endJob, flowType, Consts.EditType.READ);
            }
        }

        //*******************************************************************
        /// <summary>初期化</summary>
        //*******************************************************************
        private void LoadForInit(Container parantContainer)
        {
            // DAOの初期化
            InitialDAO();

            // 空のテーブルを取得
            SetTables();

            _parantContainer = parantContainer;

            // プロパティをセット
            container.ParantWindow = this;
            btnRegist.IsEnabled = false;
            container.sampleContainer.IsEnabled = false;
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

            /// リブート保留解除アイコン設定テーブル
            _iconReleaseDAO = new IconReleaseDAO(dbAccess);

            //added by YAMA 2014/02/06
            /// Zabbix連携アイコン設定テーブル
            _iconCooperationDAO = new IconCooperationDAO(dbAccess);

            //added by YAMA 2014/2014/05/19
            /// 実行エージェントレスアイコン設定テーブル
            _iconAgentlessDAO = new IconAgentlessDAO(dbAccess);
        }

        //*******************************************************************
        /// <summary> 空テーブルをDataTableにセット(新規追加用)</summary>
        //*******************************************************************
        private void SetTables()
        {
            // ＤＢ Connect
            dbAccess.CreateSqlConnect();

            // ジョブネット管理テーブル
            container.JobnetControlTable = _jobnetControlDAO.GetEmptyTable();
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

            //added by YAMA 2014/2014/05/19
            /// 実行エージェントレスアイコン設定テーブル
            container.IconAgentlessTable = _iconAgentlessDAO.GetEmptyTable();

            dbAccess.CloseSqlConnect();
        }

        //*******************************************************************
        /// <summary> ジョブネットデータの検索（編集、コピー新規用）</summary>
        /// <param name="jobnetId">`ジョブネットID</param>
        //*******************************************************************
        private void FillTables(string jobnetId)
        {

            // ジョブネット管理テーブル
            container.JobnetControlTable = _jobnetControlDAO.GetValidORMaxUpdateDateEntityById(jobnetId);

            Object updDate = container.JobnetControlTable.Rows[0]["update_date"];

            // ジョブ管理テーブル
            container.JobControlTable = _jobControlDAO.GetEntityByJobIdForUpdate(jobnetId, updDate);

            // フロー管理テーブル
            container.FlowControlTable = _flowControlDAO.GetEntityByJobnet(jobnetId, updDate);

            // 計算アイコン設定テーブル
            container.IconCalcTable = _iconCalcDAO.GetEntityByJobnet(jobnetId, updDate);

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

            //added by YAMA 2014/2014/05/19
            /// 実行エージェントレスアイコン設定テーブル
            container.IconAgentlessTable = _iconAgentlessDAO.GetEntityByJobnet(jobnetId, updDate);
        }

        //*******************************************************************
        /// <summary>情報エリアをセット（編集、コピー新規用）</summary>
        //*******************************************************************
        private void SetInfoArea()
        {
            DataRow row = container.JobnetControlTable.Select()[0];
            // ジョブネットIDをセット
            this.lblJobNetId.Text = Convert.ToString(row["jobnet_id"]);

            // ジョブネット名をセット
            lblJobnetName.Text = Convert.ToString(row["jobnet_name"]);

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

        }

        /// <summary>ジョブネットを展開</summary>
        /// <param name="public_flg">公開フラグ</param>
        private void SetTreeJobnet(bool public_flg)
        {
            DataTable dataTbl;

            dbAccess.CreateSqlConnect();

            int flg;
            TreeViewItem treeViewItem;
            if (public_flg)
            {
                flg = 1;
                treeViewItem = publicJobnet;
            }
            else
            {
                flg = 0;
                treeViewItem = privateJobnet;
            }

            string selectSql;
            if (public_flg)
            {
                selectSql = "select jobnet_id, max(update_date) from ja_jobnet_control_table where public_flag= " +
                                flg + " group by jobnet_id order by jobnet_id";
            }
            else
            {
                if (!(LoginSetting.Authority == Consts.AuthorityEnum.SUPER))
                {
                    selectSql = "SELECT distinct A.jobnet_id,A.update_date "
                                                    + "FROM ja_jobnet_control_table AS A,users AS U,users_groups AS UG1,users_groups AS UG2 "
                                                    + "WHERE A.user_name = U.username and U.userid=UG1.userid and UG2.userid=" + LoginSetting.UserID
                                                    + " and UG1.usrgrpid = UG2.usrgrpid and "
                                                    + "A.update_date= "
                                                    + "( SELECT MAX(B.update_date) FROM ja_jobnet_control_table AS B "
                                                    + "WHERE B.jobnet_id = A.jobnet_id group by B.jobnet_id) and A.public_flag=0 order by A.jobnet_id";
                }
                else
                {
                    selectSql = "select jobnet_id, max(update_date) from ja_jobnet_control_table where public_flag= " +
                                    flg + " group by jobnet_id order by jobnet_id";

                }
            }

            dataTbl = dbAccess.ExecuteQuery(selectSql);

            if (dataTbl != null)
            {
                treeViewItem.Items.Clear();
                foreach (DataRow row in dataTbl.Rows)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = row["jobnet_id"].ToString();
                    item.Tag = Consts.ObjectEnum.JOBNET;
                    //item.FontFamily = new FontFamily("MS Gothic");
                    treeViewItem.Items.Add(item);
                }
            }

            TreeViewItem itemJobnet;
            foreach (object item in treeViewItem.Items)
            {
                itemJobnet = (TreeViewItem)item;
                itemJobnet.Selected += Jobnet_Selected;
            }

            dbAccess.CloseSqlConnect();
        }

        #endregion
    }
}
