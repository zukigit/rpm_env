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
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author YAMA 2014/05/19 新規作成<BR>                          *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// Agentless.xaml の相互作用ロジック
    /// </summary>
    public partial class Agentless : UserControl, IElement
    {
        #region コンストラクタ
        public Agentless()
        {
            InitializeComponent();
            this.DataContext = new IconViewData();
        }
        public Agentless(RunJobMethodType methodType)
        {
            InitializeComponent();
            _methodType = methodType;
            this.DataContext = new IconViewData();
        }
        public Agentless(SolidColorBrush color)
        {
            InitializeComponent();
            picAgentless1.Fill = color;
            picAgentless2.Fill = color;
            this.DataContext = new IconViewData();
        }
        //added by YAMA 2014/07/01
        public Agentless(SolidColorBrush color, SolidColorBrush characterColor)
        {
            InitializeComponent();
            picAgentless1.Fill = color;					// アイコンカラー 設定
            picAgentless2.Fill = color;					// アイコンカラー 設定
            tbJobId.Foreground = characterColor;		// 文字の色 設定
            tbJobName.Foreground = characterColor;		// 文字の色 設定
            this.DataContext = new IconViewData();
        }

        #endregion

        #region プロパティ
        // コンテナ
        IContainer _container;
        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        /// <summary>ジョブId</summary>
        private string _jobId;
        public string JobId
        {
            get
            {
                return _jobId;
            }
            set
            {
                _jobId = value;

                // 表示文字をセット
                tbJobId.Text = CommonUtil.GetOmitString(value, SystemConst.LEN_JOBID_AGENTLESS);
                IconViewData data = (IconViewData)this.DataContext;
                data.JobId = value;
            }
        }

        /// <summary>処理フラグ</summary>
        private RunJobMethodType _methodType;
        public RunJobMethodType MethodType
        {
            get
            {
                return _methodType;
            }
            set
            {
                _methodType = value;

            }
        }

        /// <summary>内部ジョブId</summary>
        public string InnerJobId { get; set; }

        /// <summary>ジョブ名</summary>
        private string _jobName;
        public string JobName
        {
            get
            {
                return _jobName;
            }
            set
            {
                _jobName = value;

                // 表示文字をセット
                tbJobName.Text = CommonUtil.GetOmitString(value, SystemConst.LEN_JOBNAME_AGENTLESS);
                IconViewData data = (IconViewData)this.DataContext;
                data.JobName = value;

            }
        }

        /// <summary>ZIndex</summary>
        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(Canvas.ZIndexProperty);

            }
            set
            {
                this.SetValue(Canvas.ZIndexProperty, value);
            }

        }

        /// <summary>幅</summary>
        public double PicWidth
        {
            get
            {
                return tbJobId.Width;
            }
        }

        /// <summary>高さ</summary>
        public double PicHeight
        {
            get
            {
                return tbJobId.Height + tbJobName.Height + 4;
            }
        }

        /// <summary>アイコンの状態</summary>
        private IElementState _state = IElementState.Focus;
        public IElementState State
        {
            get { return _state; }
        }

        /// <summary>連接点：Top</summary>
        public Point TopConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotTop.GetValue(Canvas.LeftProperty) + HotspotTop.Width / 2;
                p.Y = (double)HotspotTop.GetValue(Canvas.TopProperty) + HotspotTop.Height / 2;

                return p;
            }
        }

        /// <summary>連接点：Bottom</summary>
        public Point BottomConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotBottom.GetValue(Canvas.LeftProperty) + HotspotBottom.Width / 2;
                p.Y = (double)HotspotBottom.GetValue(Canvas.TopProperty) + HotspotBottom.Height / 2;
                return p;
            }
        }

        /// <summary>連接点：Left</summary>
        public Point LeftConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotLeft.GetValue(Canvas.LeftProperty) + HotspotLeft.Width / 2;
                p.Y = (double)HotspotLeft.GetValue(Canvas.TopProperty) + HotspotLeft.Height / 2;
                return p;
            }
        }

        /// <summary>連接点：Right</summary>
        public Point RightConnectPosition
        {
            get
            {
                Point p = new Point();
                p.X = (double)HotspotRight.GetValue(Canvas.LeftProperty) + HotspotRight.Width / 2;
                p.Y = (double)HotspotRight.GetValue(Canvas.TopProperty) + HotspotRight.Height / 2;
                return p;
            }
        }

        /// <summary>画面項目：Top点</summary>
        public Rectangle TopSpot
        {
            get
            {
                return HotspotTop;
            }
        }

        /// <summary>画面項目：Bottom点</summary>
        public Rectangle BottomSpot
        {
            get
            {
                return BottomSpot;
            }
        }

        /// <summary>画面項目：Left点</summary>
        public Rectangle LeftSpot
        {
            get
            {
                return HotspotLeft;
            }
        }

        /// <summary>画面項目：Right点</summary>
        public Rectangle RightSpot
        {
            get
            {
                return HotspotRight;
            }
        }

        #endregion

        #region イベント
        //*******************************************************************
        /// <summary>JobId変更時イベント</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void textBlockJobId_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            bool IsTextTrimmed = false;
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null && CheckUtil.IsLenOver(textBlock.Tag.ToString(), GetJobIdTrimLimitLength()))
            {
                IsTextTrimmed = true;
            }
            TextBlockService.SetIsTextTrimmed(textBlock, IsTextTrimmed);
        }

        //*******************************************************************
        /// <summary>JobName変更時イベント</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void textBlockJobName_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            bool IsTextTrimmed = false;
            var textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null
                    && CheckUtil.IsLenOver(textBlock.Tag.ToString(), GetJobNameTrimLimitLength()))
            {
                IsTextTrimmed = true;
            }
            TextBlockService.SetIsTextTrimmed(textBlock, IsTextTrimmed);
        }

        #endregion

        #region privateメッソド

        /// <summary>連接点の色をセット</summary>
        private void SetHotspotStyle(Color color, double opacity)
        {
            HotspotLeft.Fill = new SolidColorBrush(color);
            HotspotLeft.Opacity = opacity;
            HotspotTop.Fill = new SolidColorBrush(color);
            HotspotTop.Opacity = opacity;
            HotspotRight.Fill = new SolidColorBrush(color);
            HotspotRight.Opacity = opacity;
            HotspotBottom.Fill = new SolidColorBrush(color);
            HotspotBottom.Opacity = opacity;
        }

        /// <summary>JobID表示文字数</summary>//
        private int GetJobIdTrimLimitLength()
        {
            return SystemConst.LEN_JOBID_AGENTLESS - 3;
        }

        /// <summary>Job名表示文字数</summary>///
        private int GetJobNameTrimLimitLength()
        {
            return SystemConst.LEN_JOBNAME_AGENTLESS - 3;
        }

        #endregion

        #region public メッソド

        /// <summary>自分を削除</summary>
        public void Delete()
        {
            _container.RemoveItem(this);
            this.Delete();
        }

        /// <summary>フォーカスする</summary>
        public void SetFocus()
        {
            if (this._state != IElementState.Focus)
            {
                SetHotspotStyle(Colors.Blue, 1.0);
                //HotspotLeft.Visibility = Visibility.Visible;
                //HotspotTop.Visibility = Visibility.Visible;
                //HotspotRight.Visibility = Visibility.Visible;
                //HotspotBottom.Visibility = Visibility.Visible;

                this._state = IElementState.Focus;
            }
        }

        /// <summary>フォーカスを釈放</summary>
        public void SetUnFocus()
        {
            if (this._state != IElementState.UnFocus)
            {
                //HotspotLeft.Visibility = Visibility.Collapsed;
                //HotspotTop.Visibility = Visibility.Collapsed;
                //HotspotRight.Visibility = Visibility.Collapsed;
                //HotspotBottom.Visibility = Visibility.Collapsed;

                this._state = IElementState.UnFocus;
            }
        }

        /// <summary>選択の色をセット</summary>
        public void SetSelectedColor()
        {
            picAgentless1.Fill = SystemConst.ColorConst.SelectedColor;
            picAgentless2.Fill = SystemConst.ColorConst.SelectedColor;
        }

        /// <summary>色のリセット</summary>
        public void ResetInitColor()
        {
            Brush color;
            switch (MethodType)
            {
                case RunJobMethodType.HOLD:
                    color = SystemConst.ColorConst.HoldColor;
                    break;
                case RunJobMethodType.SKIP:
                    color = SystemConst.ColorConst.SkipColor;

                    //added by YAMA 2014/07/01
                    // スキップ設定時は白文字を設定
                    tbJobId.Foreground = SystemConst.ColorConst.WhiteColor;
                    tbJobName.Foreground = SystemConst.ColorConst.WhiteColor;

                    break;
                default:
                    color = SystemConst.ColorConst.JobColor;

                    //added by YAMA 2014/07/01
                    // スキップ設定以外は黒文字を設定
                    tbJobId.Foreground = SystemConst.ColorConst.BlackColor;
                    tbJobName.Foreground = SystemConst.ColorConst.BlackColor;

                    break;
            }
            picAgentless1.Fill = color;
            picAgentless2.Fill = color;
        }

        /// <summary>部品欄のアイコン色をセット</summary>
        public void InitSampleColor()
        {
            picAgentless1.Fill = SystemConst.ColorConst.SampleColor;
            picAgentless2.Fill = SystemConst.ColorConst.SampleColor;
        }

        /// <summary>実行状態によりアイコン色をセット</summary>
        public void SetStatusColor(SolidColorBrush color)
        {
            picAgentless1.Fill = color;
            picAgentless2.Fill = color;
        }

        //added by YAMA 2014/07/01
        /// <summary>部品欄の文字色をセット</summary>
        public void SetStatusCharacterColor(SolidColorBrush color)
        {
            tbJobId.Foreground = color;
            tbJobName.Foreground = color;
        }

        /// <summary>部品欄のアイコン選択状態をセット</summary>
        public void SetSelected()
        {
            if (this._state != IElementState.Selected)
            {
                SetHotspotStyle(Colors.Red, 0.5);
                //HotspotLeft.Visibility = Visibility.Visible;
                //HotspotTop.Visibility = Visibility.Visible;
                //HotspotRight.Visibility = Visibility.Visible;
                //HotspotBottom.Visibility = Visibility.Visible;

                this._state = IElementState.Selected;
            }
        }

        /// <summary>ToolTip表示内容設定</summary>///
        public void SetToolTip()
        {
            StringBuilder sbSession = new StringBuilder();
            StringBuilder sbHost = new StringBuilder();
            StringBuilder sbSSH = new StringBuilder();
            StringBuilder sbExec = new StringBuilder();
            StringBuilder sbPrompt = new StringBuilder();
            bool showHostAndSSH = false;
            string characterCode = "";
            string linefeed = "";
            string timeout = "";
            string stopCode = "";
            string forceStr = Properties.Resources.tooltip_flag_off;
            StringBuilder sb = new StringBuilder();

            sb.Append(Properties.Resources.job_id_label_text);
            /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            if (!LoginSetting.Lang.StartsWith("ja_"))  sb.Append(" ");
            sb.Append(_jobId);
            sb.Append("\n");
            sb.Append(Properties.Resources.job_name_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(_jobName);

            DataRow[] rowIconAgentless;
            if (InnerJobId == null) {
                rowIconAgentless = _container.IconAgentlessTable.Select("job_id='" + _jobId + "'");
            } else {
                rowIconAgentless = _container.IconAgentlessTable.Select("inner_job_id=" + InnerJobId);
            }

            if (rowIconAgentless != null && rowIconAgentless.Length > 0) {
                int sessionFlg = Convert.ToInt16(rowIconAgentless[0]["session_flag"]);
                switch (sessionFlg)
                {
                    // ワンタイム
                    case 0:
                        showHostAndSSH = true;
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_onetime_label_text);
                        break;
                    // 接続
                    case 1:
                        showHostAndSSH = true;
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_connection_label_text);
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_sessionid_label_text);
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSession.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSession.Append(Convert.ToString(rowIconAgentless[0]["session_id"]));
                        break;
                    // 継続
                    case 2:
                        showHostAndSSH = false;
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_continue_label_text);
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_sessionid_label_text);
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSession.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSession.Append(Convert.ToString(rowIconAgentless[0]["session_id"]));
                        break;
                    // 切断
                    case 3:
                        showHostAndSSH = false;
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_disconnect_label_text);
                        sbSession.Append("\n");
                        sbSession.Append("  ");
                        sbSession.Append(Properties.Resources.agentless_sessionid_label_text);
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSession.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSession.Append(Convert.ToString(rowIconAgentless[0]["session_id"]));
                        break;
                }
                string hostFlag = Convert.ToString(rowIconAgentless[0]["host_flag"]);
                string hostName = Convert.ToString(rowIconAgentless[0]["host_name"]);
                if ("1".Equals(hostFlag)) {
                    sbHost.Append("\n");
                    sbHost.Append("  ");
                    sbHost.Append(Properties.Resources.value_name_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbHost.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbHost.Append(hostName);
                } else {
                    sbHost.Append("\n");
                    sbHost.Append("  ");
                    sbHost.Append(Properties.Resources.host_name_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbHost.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbHost.Append(hostName);
                }

                // 認証方式
                string authMethod = Convert.ToString(rowIconAgentless[0]["auth_method"]);
                if (Convert.ToInt16(authMethod) == 0)
                {
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_authentic_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Properties.Resources.agentless_comb_passwd_text);
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_executemode_label_text);

                    string run_mode = Convert.ToString(rowIconAgentless[0]["run_mode"]);
                    if ("0".Equals(run_mode))
                    {
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSSH.Append(Properties.Resources.agentless_comb_interactive_text);
                    }
                    else
                    {
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSSH.Append(Properties.Resources.agentless_comb_noninteractive_text);
                    }
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.user_name_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Convert.ToString(rowIconAgentless[0]["login_user"]));
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_passwd_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    //Park.iggy Add
                    //sbSSH.Append(Convert.ToString(rowIconAgentless[0]["login_password"]));
                    if (Convert.ToString(rowIconAgentless[0]["login_password"]) != null)
                    {
                        sbSSH.Append("******");
                    }
                    else
                    {
                        sbSSH.Append(Convert.ToString(rowIconAgentless[0]["login_password"]));
                    }
                }
                else
                {
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_authentic_label_text);
                    sbSSH.Append(Properties.Resources.agentless_comb_publickey_text);

                    //added by YAMA 2014/12/04
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_executemode_label_text);

                    string run_mode = Convert.ToString(rowIconAgentless[0]["run_mode"]);
                    if ("0".Equals(run_mode))
                    {
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSSH.Append(Properties.Resources.agentless_comb_interactive_text);
                    }
                    else
                    {
                        if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                        sbSSH.Append(Properties.Resources.agentless_comb_noninteractive_text);
                    }

                    //added by YAMA 2014/12/04
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.user_name_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Convert.ToString(rowIconAgentless[0]["login_user"]));

                    // 公開鍵
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_publickey_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Convert.ToString(rowIconAgentless[0]["public_key"]));
                    // 秘密鍵
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_privatekey_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Convert.ToString(rowIconAgentless[0]["private_key"]));
                    // パスフレーズ
                    sbSSH.Append("\n");
                    sbSSH.Append("  ");
                    sbSSH.Append(Properties.Resources.agentless_passphrase_label_text);
                    if (!LoginSetting.Lang.StartsWith("ja_")) sbSSH.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
                    sbSSH.Append(Convert.ToString(rowIconAgentless[0]["passphrase"]));
                }
                sbPrompt.Append("\n");
                sbPrompt.Append("  ");
                sbPrompt.Append(Convert.ToString(rowIconAgentless[0]["prompt_string"]));
                characterCode = Convert.ToString(rowIconAgentless[0]["character_code"]);
                string linefeedCode = Convert.ToString(rowIconAgentless[0]["line_feed_code"]);
                if("0".Equals(linefeedCode)){
                    linefeed = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_LF_text;
                }else if("1".Equals(linefeedCode)){
                    linefeed = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_CR_text;
                }else if("2".Equals(linefeedCode)){
                    linefeed = jp.co.ftf.jobcontroller.JobController.Properties.Resources.agentless_comb_CRLF_text;
                }
                timeout = Convert.ToString(rowIconAgentless[0]["timeout"]);
                stopCode = Convert.ToString(rowIconAgentless[0]["stop_code"]);

                DataRow[] rowJob = _container.JobControlTable.Select("job_id='" + _jobId + "'");
                string forceFlag = Convert.ToString(rowJob[0]["force_flag"]);
                if ("1".Equals(forceFlag)) {
                    forceStr = Properties.Resources.tooltip_flag_on;
                }
            }

            // 実行
            string command = Convert.ToString(rowIconAgentless[0]["command"]);
            foreach (string line in command.Trim().Split(new Char[] {'\n'})){
                sbExec.Append("\n");
                sbExec.Append("  ");
                sbExec.Append(line);
            }

            sb.Append("\n");
            sb.Append(Properties.Resources.agentless_session_label_text);
            sb.Append(sbSession.ToString());
            if(showHostAndSSH){
                sb.Append("\n");
                sb.Append(Properties.Resources.host_label_text);
                sb.Append(sbHost.ToString());
                sb.Append("\n");
                sb.Append(Properties.Resources.agentless_ssh_label_text);
                sb.Append(sbSSH.ToString());
            }

            sb.Append("\n");
            sb.Append(Properties.Resources.exec_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(sbExec.ToString());
            sb.Append("\n");
            sb.Append(Properties.Resources.agentless_prompt_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(sbPrompt.ToString());
            sb.Append("\n");
            sb.Append(Properties.Resources.agentless_charcode_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(characterCode);
            sb.Append("\n");
            sb.Append(Properties.Resources.agentless_linefeedcode_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(linefeed);
            sb.Append("\n");
            sb.Append(Properties.Resources.agentless_timeout_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(timeout);
            sb.Append("\n");
            sb.Append(Properties.Resources.stop_code_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(stopCode);
            sb.Append("\n");
            sb.Append(Properties.Resources.tooltip_force_label_text);
            if (!LoginSetting.Lang.StartsWith("ja_")) sb.Append(" ");    /* added by YAMA 2014/12/15    V2.1.0 No32 対応 */
            sb.Append(forceStr);

            picToolTip.ToolTip = sb.ToString();
        }

        /// <summary>ToolTip表示内容リセット</summary>///
        public void ResetToolTip(string toolTip)
        {
            picToolTip.ToolTip = toolTip;
        }

        #endregion

    }
}
