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


//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 郭 暁宇 2012/10/15 新規作成<BR>                    *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// 共通ダイアログ
    /// </summary>
    public class CommonDialog
    {
        #region フィールド

        /// <summary>エラーダイアログのタイトル</summary>
        private static String errDialogTitle = Properties.Resources.Title_Msgbox_Error;

        /// <summary>終了確認ダイアログのタイトル</summary>
        private static String endDialogTitle = Properties.Resources.Title_Msgbox_End;

        /// <summary>終了確認ダイアログのメッセージ</summary>
        private static String endDialogMessage = Properties.Resources.MSG_COMMON_001;

        /// <summary>編集登録確認ダイアログのタイトル</summary>
        private static String editRegistDialogTitle = Properties.Resources.Title_Msgbox_Edit;

        /// <summary>編集登録確認ダイアログのメッセージ</summary>
        private static String editRegistMessage = Properties.Resources.MSG_COMMON_002;

        /// <summary>バージョンダイアログのタイトル</summary>
        private static String versionDialogTitle = Properties.Resources.Title_Msgbox_Head;

        /// <summary>バージョンダイアログのメッセージ</summary>
        private static String versionDialogMessage = Properties.Resources.MSG_COMMON_003;

        /// <summary>削除確認ダイアログのタイトル</summary>
        private static String deleteDialogTitle = Properties.Resources.Title_Msgbox_Delete;

        /// <summary>削除確認ダイアログのメッセージ</summary>
        private static String deleteMessage = Properties.Resources.MSG_COMMON_004;

        /// <summary>ジョブネット起動ダイアログのメッセージ</summary>
        private static String jobnetStartMessage = Properties.Resources.MSG_COMMON_005;

        /// <summary>削除確認ダイアログのタイトル</summary>
        private static String jobnetStartDialogTitle = Properties.Resources.Title_Msgbox_Jobnet_Start;

        /// <summary>キャンセル確認ダイアログのタイトル</summary>
        private static String cancelDialogTitle = Properties.Resources.Title_Msgbox_Cancel;

        /// <summary>キャンセル確認ダイアログのメッセージ</summary>
        private static String cancelMessage = Properties.Resources.MSG_COMMON_008;

        //added by YAMA 2014/10/17
        /// <summary>強制実行確認ダイアログのタイトル</summary>
        private static String ForceRunDialogTitle = Properties.Resources.Title_Msgbox_ForceRun;

        //added by YAMA 2014/10/17
        /// <summary>強制実行確認ダイアログのメッセージ</summary>
        private static String ForceRunMessage = Properties.Resources.ForceRun_dialog_text;

        //予定されたジョブネットを削除しますか
        private static String jobnetDeleteMessage = Properties.Resources.MSG_COMMON_009;
        //スケジュールは有効です。無効にしてから削除お願いします。
        private static String enableMessage = Properties.Resources.MSG_COMMON_010;

        #endregion

        #region publicメソッド
        /// <summary>
        /// エラーダイアログ
        /// </summary>
        /// <param name="msgId">メッセージＩＤ</param>
        public static MessageBoxResult ShowErrorDialogFromMessage(String msg)
        {
            return MessageBox.Show(msg, errDialogTitle, MessageBoxButton.OK);
        }

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        /// <param name="msgId">メッセージＩＤ</param>
        public static MessageBoxResult ShowErrorDialog(String msgId)
        {
            string msg = MessageUtil.GetMsgById(msgId);
            return MessageBox.Show(msg, errDialogTitle, MessageBoxButton.OK);
        }

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        /// <param name="msgId">メッセージＩＤ</param>
        public static MessageBoxResult ShowDeleteDialog(String msgId)
        {
            string msg = MessageUtil.GetMsgById(msgId);
            return MessageBox.Show(msg, deleteDialogTitle, MessageBoxButton.OK);
        }

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        /// <param name="msgId">メッセージＩＤ</param>
        /// <param name="ChangeMsg">置換文字列</param>
        public static void ShowErrorDialog(String msgId, string[] ChangeMsg)
        {
            string msg = MessageUtil.GetMsgById(msgId);
            msg = MessageUtil.GetReplaceMessage(msg, ChangeMsg);
            MessageBox.Show(msg, errDialogTitle, MessageBoxButton.OK);
        }

        /// <summary>
        /// 終了確認ダイアログ
        /// </summary>
        public static MessageBoxResult ShowEndDialog()
        {
            return System.Windows.MessageBox.Show(endDialogMessage, endDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo,
                                   System.Windows.MessageBoxImage.None, MessageBoxResult.No);
        }

        /// <summary>
        /// 編集登録確認ダイアログ
        /// </summary>
        public static MessageBoxResult ShowEditRegistDialog()
        {
            return System.Windows.MessageBox.Show(editRegistMessage, editRegistDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo);
        }

        /// <summary>
        /// キャンセル確認ダイアログ
        /// </summary>
        public static MessageBoxResult ShowCancelDialog()
        {
            return System.Windows.MessageBox.Show(cancelMessage, cancelDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo);
        }

        /// <summary>
        /// バージョンダイアログ
        /// </summary>
        public static void ShowVersionDialog()
        {
            jp.co.ftf.jobcontroller.JobController.JobArrangerMessageBox.Show(versionDialogTitle, versionDialogMessage,
                            System.Windows.MessageBoxButton.OK);
        }

        /// <summary>
        /// 削除確認ダイアログ
        /// </summary>
        public static MessageBoxResult ShowDeleteDialog()
        {
            return System.Windows.MessageBox.Show(deleteMessage, deleteDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo,
                                  System.Windows.MessageBoxImage.None, MessageBoxResult.No);
        }

        /// <summary>
        /// ジョブネット起動ダイアログ
        /// </summary>
        public static MessageBoxResult ShowJobnetStartDialog()
        {
            return System.Windows.MessageBox.Show(jobnetStartMessage, jobnetStartDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo,
                                   System.Windows.MessageBoxImage.None, MessageBoxResult.No);
        }

        //added by YAMA 2014/10/17
        /// <summary>
        /// 強制実行確認ダイアログ
        /// </summary>
        public static MessageBoxResult ForceRunDialog()
        {
            return System.Windows.MessageBox.Show(ForceRunMessage, ForceRunDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo,
                                  System.Windows.MessageBoxImage.None, MessageBoxResult.No);
        }

        /// <summary>
        /// 予定ジョブネットを削除でスケジュールが有効の場合
        /// </summary>
        public static void ShowNotScheduleDelete(String schedule_id)
        {
            string msg = MessageUtil.GetReplaceMessage(enableMessage, new string[] { schedule_id });
            MessageBox.Show(msg, errDialogTitle, MessageBoxButton.OK);

        }
        /// <summary>
        /// 予定ジョブネットを削除確認
        /// </summary>
        public static MessageBoxResult ShowScheduleDelete()
        {
            return System.Windows.MessageBox.Show(jobnetDeleteMessage, deleteDialogTitle,
                                   System.Windows.MessageBoxButton.YesNo,
                                   System.Windows.MessageBoxImage.None, MessageBoxResult.No);
        }


        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommonDialog()
        {
            //
            // TODO: コンストラクタ ロジックをここに追加してください。

            //
        }
        #endregion
    }
}
