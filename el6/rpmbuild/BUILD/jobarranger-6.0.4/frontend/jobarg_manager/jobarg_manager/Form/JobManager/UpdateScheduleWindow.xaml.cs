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
using System.Text.RegularExpressions;
using System.IO;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;

namespace jp.co.ftf.jobcontroller.JobController.Form.JobManager
{
    // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
    /// <summary>
    /// UpdateScheduleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateScheduleWindow : BaseWindow
    {
        #region フィールド

        /// <summary>実行用ジョブネット内部管理ID</summary>
        private decimal _innerJobnetId;

        /// <summary>半角数字と半角空白、スラッシュ、コロン</summary>
        private const string MATCH_HANKAKU_SPACE_SLASH_COLON = "^[0-9 /\\:]*$";

        #endregion

        #region コンストラクタ
        //*******************************************************************
        /// <summary>コンストラクタ</summary>
        /// <param name="innerJobnetId">実行用ジョブネット内部管理ID</param>
        /// <param name="jobnet_Id">ジョブネットID</param>
        /// <param name="scheduledTime">実行予定時刻D</param>
        //*******************************************************************
        public UpdateScheduleWindow(decimal innerJobnetId, String jobnet_Id, String scheduledTime)
        {
            char[] delimiterChars = { ' ', '/', ':' };

            InitializeComponent();

            _innerJobnetId = innerJobnetId;
            manageId.Text = innerJobnetId.ToString();
            jobnetId.Text = jobnet_Id;

            if (LoginSetting.Lang.StartsWith("ja_"))
            {
                schedule.Text = scheduledTime;
                textBox_time.Text = scheduledTime;
            }
            else
            {
                string stTime = getUStime(scheduledTime, "g");
                schedule.Text = stTime;
                textBox_time.Text = stTime;
                textBox_time.MaxLength = 19;
            }

            DataContext = this;
        }
        #endregion


        #region プロパティ
        /// <summary>クラス名</summary>
        public override string ClassName
        {
            get
            {
                return "UpdateScheduleWindow";
            }
        }

        /// <summary>画面ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_320;
            }
        }
        #endregion


        #region イベント
        //*******************************************************************
        /// <summary>キャンセルボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        //*******************************************************************
        /// <summary>ＯＫボタンクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">マウスイベント</param>
        //*******************************************************************
        private void ok_Click(object sender, EventArgs e)
        {
            char[] removeChars = new char[] { '/', ' ', ':' };      // 削除する文字の配列
            String time = textBox_time.Text;                        // 画面からの入力値
            int intNum = -1;

            // 開始ログ
            base.WriteStartLog("ok_Click", Consts.PROCESS_012);

            try
            {
                // 入力チェック
                if (!InputCheck(time))
                    return;

                // 英語圏の場合、西暦を和暦に変換
                if (!(LoginSetting.Lang.StartsWith("ja_")))
                {
                    time = getJPtime(time);
                }

                // 月/日/時/分の先頭を"0"で埋める
                time = ZeroPadding(time);

                // DBへの書き込み前のフォーマットのチェック
                if (!(IsFormat(time)))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_006);
                    return;
                }

                //削除する文字（"/" ":"）を1文字ずつ削除する
                foreach (char c in removeChars)
                {
                    time = time.Replace(c.ToString(), "");
                }

                intNum = DBUtil.SetScheduledTime(_innerJobnetId.ToString(), time);

                if (intNum != 2)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_005);
                }

                this.Close();

            }
            catch (ArgumentException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (NotSupportedException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_019);
            }
            catch (DirectoryNotFoundException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_022);
            }
            catch (UnauthorizedAccessException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_023);
            }
            catch (System.IO.IOException ex)
            {
                CommonDialog.ShowErrorDialogFromMessage(ex.Message);
            }
            catch (Exception ex)
            {
                CommonDialog.ShowErrorDialogFromMessage(ex.Message);
            }
            // 終了ログ
            base.WriteEndLog("ok_Click", Consts.PROCESS_012);
        }
        #endregion


        #region privateメソッド
        //*******************************************************************
        /// <summary>入力チェック </summary>
        /// <param name="time">開始時刻</param>
        /// <returns>チェック結果</returns>
        //*******************************************************************
        private bool InputCheck(String time)
        {
            System.Globalization.CultureInfo ci;    // CultureInfo クラス
            DateTime dt1;                           // DateTimeオブジェクト

            String[] errItem = new String[3];

            // 開始時刻が未入力の場合、エラーダイアログを表示
            if (CheckUtil.IsNullOrEmpty(time))
            {
                errItem[0] = Properties.Resources.err_message_boot_start_time;      // 開始時刻
                errItem[1] = "2";                                                   // 2バイト、2桁の２
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, errItem);
                return false;
            }

            // 日本の場合  ：半角数字、半角空白、スラッシュ、コロン以外が入力されている場合、エラーダイアログを表示
            // 英語圏の場合：半角数字、半角空白、スラッシュ、コロン、AM、PM 以外が入力されている場合、エラーダイアログを表示
            if (!IsHankakuStrAndColon(time))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_006);
                return false;
            }

            // 日本の場合  ：「"/" が2個 、" " が1個、":" が1個」以外、入力されている場合、エラーダイアログを表示
            // 英語圏の場合：「"/" が2個 、" " が2個、":" が1個」以外、入力されている場合、エラーダイアログを表示
            if (LoginSetting.Lang.StartsWith("ja_"))
            {
                if ((CountChar(time, '/') != 2) || (CountChar(time, ' ') != 1) || (CountChar(time, ':') != 1))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_006);
                    return false;
                }
            }else{
                if ((CountChar(time, '/') != 2) || (CountChar(time, ' ') != 2) || (CountChar(time, ':') != 1))
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_006);
                    return false;
                }
            }

            // 時刻チェック（月、日(閏年/大の月/小の月 含む)、時、分 の確認）
            /// 日時を表す文字列をDateTimeオブジェクトに変換
            try
            {
                ci = new System.Globalization.CultureInfo("ja-JP");
                dt1 = DateTime.Parse(time, ci, System.Globalization.DateTimeStyles.AssumeLocal);
            }
            catch (Exception)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_006);
                return false;
            }

            // 過去日が入力されたの場合、エラーダイアログを表示
            DateTime dNow = DBUtil.GetSysTime();                // 現在の日時を取得
            DateTime dDate = dNow.Date;                         // 現在の日付を取得
            // added by YAMA 2014/12/03    (V2.1.0 No23)
            //if (!(dDate <= dt1))
            if (!(dNow < dt1))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SCHEDULE_007);
                return false;
            }

            return true;
        }


        //*******************************************************************
        /// <summary>存在チェック </summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角数字と半角空白、スラッシュ、コロン」「False：それ以外」</return>
        //*******************************************************************
        private static bool IsHankakuStrAndColon(string str)
        {
            string output;

            if (LoginSetting.Lang.StartsWith("ja_"))
            {
                output = str;
            }
            else
            {
                Regex pm = new Regex("PM", RegexOptions.Singleline);
                Regex am = new Regex("AM", RegexOptions.Singleline);
                output = pm.Replace(str, "");
                output = am.Replace(output, "");
            }

            if (output == null || output.Length == 0
                || Regex.IsMatch(output, MATCH_HANKAKU_SPACE_SLASH_COLON))
            {
                return true;
            }
            return false;
        }


        //*******************************************************************
        /// <summary>文字の出現回数をカウント</summary>
        /// <param name="s">検索文字列</param>
        /// <param name="c">検索文字</param>
        /// <return>文字の出現回数</return>
        //*******************************************************************
        private static int CountChar(string s, char c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }


        //*******************************************************************
        /// <summary>DBへの書き込み前のフォーマットのチェック</summary>
        /// <param name="time">チェックする文字列</param>
        /// <return>「True：OK」「False：NG」</return>
        //*******************************************************************
        private static bool IsFormat(string time)
        {
            // 5文字目と8文字目 が "/" であること
            if ((time.Substring(4, 1).Equals("/")) && (time.Substring(7, 1).Equals("/")))
            {
                // 9字目 が " " であること
                if (time.Substring(10, 1).Equals(" "))
                {
                    // 12字目 が ":" であること
                    if (!(time.Substring(13, 1).Equals(":"))) return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        //*******************************************************************
        /// <summary>位置の確認</summary>
        /// <param name="time">チェックする文字列</param>
        /// <return>「True：OK」「False：NG」</return>
        //*******************************************************************
        private static bool IsPlace(string time)
        {
            if ((time.Substring(4, 1).Equals("/")) && (time.Substring(6, 1).Equals("/") || time.Substring(7, 1).Equals("/")))
            {
                if ((time.Substring(8, 1).Equals(" ") || time.Substring(9, 1).Equals(" ") || time.Substring(10, 1).Equals(" ")))
                {
                    int idx = time.IndexOf(":");
                    if (!((9 < idx) && (idx < 14)))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        //*******************************************************************
        /// <summary>時刻チェック </summary>
        /// <param name="time">時刻</param>
        /// <return></return>
        //*******************************************************************
        private static int CheckTime(string time)
        {
            char[] delimiterChars = { ' ', '/', ':' };
            int itY;
            int itM;
            int itD;
            int itH;
            int itMm;
            int maxDay;

            string[] inTime = time.Split(delimiterChars);

            itY =  Convert.ToInt32(inTime[0]);  // 年
            itM =  Convert.ToInt32(inTime[1]);  // 月
            itD =  Convert.ToInt32(inTime[2]);  // 日
            itH =  Convert.ToInt32(inTime[3]);  // 時
            itMm = Convert.ToInt32(inTime[4]);  // 分


            // 月 の確認
            if (!(CheckUtil.IsMonth(itM)))   return -1;

            // 日 の確認
            switch (itM)
            {
                case 2:
                    // 2月
                    if (DateTime.IsLeapYear(itY))	            // 入力された年が閏年か?
                    {
                        maxDay = 29;
                    }
                    else
                    {
                        maxDay = 28;
                    }
                    break;

                case 4:
                case 6:
                case 9:
                case 11:
                    // 小の月(4,6,9,11月)
                    maxDay = 30;
                    break;

                default:
                    // 大の月
                    maxDay = 31;
                    break;
            }
            if ((itD < 1) || (itD > maxDay)) return -2;	        // 範囲外

            // 時 の確認
            if (!(CheckUtil.IsHour(itH)))    return -3;

            // 分 の確認
            if (!(CheckUtil.IsMin(itMm)))    return -4;

            // 過去の日付でないか の確認
            DateTime dNow = System.DateTime.Now;                // 現在の日時を取得
            DateTime dDate = dNow.Date;                         // 現在の日付を取得
            DateTime dIntime = new DateTime(itY, itM, itD);     // 入力された日付
            if (!(dDate <= dIntime)) return -5;

            return 0;
        }


        //*******************************************************************
        /// <summary>月/日/時/分の先頭を"0"で埋める</summary>
        /// <param name="time">時刻</param>
        /// <return>string</return>
        //*******************************************************************
        private static string ZeroPadding(string time)
        {
            char[] delimiterChars = { ' ', '/', ':' };
            string stOut;

            string[] inTime = time.Split(delimiterChars);

            stOut = inTime[0] + "/" + inTime[1].PadLeft(2, '0') + "/" + inTime[2].PadLeft(2, '0') + " " + inTime[3].PadLeft(2, '0') + ":" + inTime[4].PadLeft(2, '0');
            return stOut;
        }


        //*******************************************************************
        /// <summary>日時書式指定文字列を返す</summary>
        /// <param name="time">時刻</param>
        /// <param name="form">書式</param>
        /// <return>string</return>
        //*******************************************************************
        private static string getUStime(string time, string form)
        {
            char[] delimiterChars = { ' ', '/', ':' };
            string stOut;

            string[] inTime = time.Split(delimiterChars);

            int itY = Convert.ToInt32(inTime[0]);	// 年
            int itM = Convert.ToInt32(inTime[1]);	// 月
            int itD = Convert.ToInt32(inTime[2]);	// 日
            int itH = Convert.ToInt32(inTime[3]);	// 時
            int itMm = Convert.ToInt32(inTime[4]);	// 分

            DateTime dt = new DateTime(itY, itM, itD, itH, itMm, 00, 00);

            stOut = dt.ToString(form, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

            return stOut;
        }


        //*******************************************************************
        /// <summary>西暦を和暦に変換した文字列を返す</summary>
        /// <param name="time">西暦時刻</param>
        /// <return>string</return>
        //*******************************************************************
        private static string getJPtime(string time)
        {
            System.Globalization.CultureInfo ci;    // CultureInfo クラス
            DateTime dt1;                           // DateTimeオブジェクト
            string stOut;

            try
            {
                ci = new System.Globalization.CultureInfo("ja-JP");
                dt1 = DateTime.Parse(time, ci, System.Globalization.DateTimeStyles.AssumeLocal);
            }
            catch (Exception ex)
            {
                throw new DBException(Consts.SYSERR_001, ex);
            }

            stOut = dt1.ToString("g", System.Globalization.CultureInfo.CreateSpecificCulture("ja-JP"));

            return stOut;
        }

        #endregion
    }
}
