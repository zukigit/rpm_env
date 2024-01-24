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
using System.Collections.Generic;
using System.Windows;
using jp.co.ftf.jobcontroller.Common;
using System.Data;
using System.Configuration;
using System.Collections;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/04 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// ユーティリティークラス
    /// </summary>
    public class CommonUtil
    {

        #region 定数フィールド
        /// <summary> 開始アイコンの識別子</summary>
        private const string START_SIGN = "START";

        /// <summary> 終了アイコンの識別子</summary>
        private const string END_SIGN = "END-";

        /// <summary> 条件分岐アイコンの識別子</summary>
        private const string IF_SIGN = "IF-";

        /// <summary> ジョブコントローラ変数アイコンの識別子</summary>
        private const string ENV_SIGN = "ENV-";

        /// <summary> ジョブアイコンの識別子</summary>
        private const string JOB_SIGN = "JOB-";

        /// <summary> ジョブネットアイコンの識別子</summary>
        private const string JOBNET_SIGN = "JOBNET-";

        /// <summary> 並行処理開始アイコンの識別子</summary>
        private const string MTS_SIGN = "MTS-";

        /// <summary> 並行処理終了アイコンの識別子</summary>
        private const string MTE_SIGN = "MTE-";

        /// <summary> ループアイコンの識別子</summary>
        private const string LOOP_SIGN = "LOOP-";

        /// <summary> 拡張ジョブアイコンの識別子</summary>
        private const string EXTJOB_SIGN = "EXTJOB-";

        /// <summary> 計算アイコンの識別子</summary>
        private const string CAL_SIGN = "CAL-";

        /// <summary> タスクアイコンの識別子</summary>
        private const string TASK_SIGN = "TASK-";

        /// <summary> 情報取得アイコンの識別子</summary>
        private const string INF_SIGN = "INFO-";

        //added by kim 2011/11/14
        /// <summary> 分岐終了アイコンの識別子</summary>
        private const string IFE_SIGN = "IFE-";

        /// <summary> ファイル転送の識別子</summary>
        private const string FCOPY_SIGN = "FCOPY-";

        /// <summary> ファイル待ち合わせの識別子</summary>
        private const string FWAIT_SIGN = "FWAIT-";

        /// <summary> リブートの識別子</summary>
        private const string REBOOT_SIGN = "REBOOT-";

        /// <summary> 保留解除の識別子</summary>
        private const string RELEASE_SIGN = "REL-";

        //added by YAMA 2014/02/04
        /// <summary> Zabbix連携の識別子</summary>
        private const string COOPERATION_SIGN = "ZBX-";

        //added by YAMA 2014/05/19
        /// <summary> エージェントレスの識別子</summary>
        private const string AGENTLESS_SIGN = "LESS-";

        #endregion


        #region public メッソド
        /// <summary>ジョブＩＤを取得</summary>
        public static string GetJobId(Hashtable JobNoHash, ElementType type)
        {
            string jobId = "";
            switch (type)
            {
                // 開始の場合
                case ElementType.START:
                    jobId = START_SIGN;
                    break;
                // 終了の場合
                case ElementType.END:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = END_SIGN + (int)JobNoHash[type];
                    break;
                // 条件分岐の場合
                case ElementType.IF:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = IF_SIGN + (int)JobNoHash[type];
                    break;
                // ジョブコントローラ変数の場合
                case ElementType.ENV:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = ENV_SIGN + (int)JobNoHash[type];
                    break;
                // ジョブの場合
                case ElementType.JOB:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = JOB_SIGN + (int)JobNoHash[type];
                    break;
                // ジョブネットの場合
                case ElementType.JOBNET:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = JOBNET_SIGN + (int)JobNoHash[type];
                    break;
                // 並行処理開始の場合
                case ElementType.MTS:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = MTS_SIGN + (int)JobNoHash[type];
                    break;
                // 並行処理終了の場合
                case ElementType.MTE:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = MTE_SIGN + (int)JobNoHash[type];
                    break;
                // ループの場合
                case ElementType.LOOP:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = LOOP_SIGN + (int)JobNoHash[type];
                    break;
                // 拡張ジョブの場合
                case ElementType.EXTJOB:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = EXTJOB_SIGN + (int)JobNoHash[type];
                    break;
                // 計算の場合
                case ElementType.CAL:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = CAL_SIGN + (int)JobNoHash[type];
                    break;
                // タスクの場合
                case ElementType.TASK:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = TASK_SIGN + (int)JobNoHash[type];
                    break;
                // 情報の場合
                case ElementType.INF:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = INF_SIGN + (int)JobNoHash[type];
                    break;
                case ElementType.IFE:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = IFE_SIGN + (int)JobNoHash[type];
                    break;
                // ファイル転送の場合
                case ElementType.FCOPY:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = FCOPY_SIGN + (int)JobNoHash[type];
                    break;
                // ファイル待合せの場合
                case ElementType.FWAIT:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = FWAIT_SIGN + (int)JobNoHash[type];
                    break;
                // リブートの場合
                case ElementType.REBOOT:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = REBOOT_SIGN + (int)JobNoHash[type];
                    break;
                // 保留解除の場合
                case ElementType.RELEASE:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = RELEASE_SIGN + (int)JobNoHash[type];
                    break;
                //added by YAMA 2014/02/04
                // Zabbix連携の場合
                case ElementType.COOPERATION:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = COOPERATION_SIGN + (int)JobNoHash[type];
                    break;
                //added by YAMA 2014/05/19
                // エージェントレスの場合
                case ElementType.AGENTLESS:
                    JobNoHash[type] = (int)JobNoHash[type] + 1;
                    jobId = AGENTLESS_SIGN + (int)JobNoHash[type];
                    break;
            }


            return jobId;
        }

        /// <summary>ジョブID用の番号を初期化</summary>
        public static void InitJobNo(Hashtable JobNoHash)
        {

            //終了アイコンの連番
            JobNoHash[ElementType.END] = 0;

            //条件分岐アイコンの連番
            JobNoHash[ElementType.IF] = 0;

            //ジョブコントローラ変数アイコンの識別子
            JobNoHash[ElementType.ENV] = 0;

            //ジョブアイコンの識別子
            JobNoHash[ElementType.JOB] = 0;

            //ジョブネットアイコンの識別子
            JobNoHash[ElementType.JOBNET] = 0;

            //並行処理開始アイコンの連番
            JobNoHash[ElementType.MTS] = 0;

            //並行処理終了アイコンの連番
            JobNoHash[ElementType.MTE] = 0;

            //ループアイコンの連番
            JobNoHash[ElementType.LOOP] = 0;

            //拡張ジョブアイコンの連番
            JobNoHash[ElementType.EXTJOB] = 0;

            //計算アイコンの連番
            JobNoHash[ElementType.CAL] = 0;

            // タスクアイコンの連番
            JobNoHash[ElementType.TASK] = 0;

            //情報取得アイコンの連番
            JobNoHash[ElementType.INF] = 0;

            //分岐終了アイコンの連番
            JobNoHash[ElementType.IFE] = 0;

            //ファイル転送アイコンの連番
            JobNoHash[ElementType.FCOPY] = 0;

            //ファイル待ち合わせアイコンの連番
            JobNoHash[ElementType.FWAIT] = 0;

            //リブートアイコンの連番
            JobNoHash[ElementType.REBOOT] = 0;

            //保留解除アイコンの連番
            JobNoHash[ElementType.RELEASE] = 0;

            //added by YAMA 2014/02/04
            //Zabbix連携アイコンの連番
            JobNoHash[ElementType.COOPERATION] = 0;

            //added by YAMA 2014/05/19
            //エージェントレスアイコンの連番
            JobNoHash[ElementType.AGENTLESS] = 0;
        }

        /// <summary>線の連接点を取得</summary>
        /// <param name="beginItem">開始ジョブ</param>
        /// <param name="endItem">終了ジョブ</param>
        /// <param name="arrow">フロー</param>
        public static List<Point> GetConnectPoints(IRoom beginItem, IRoom endItem)
        {
            List<Point> list = new List<Point>();
            Point startP = beginItem.LeftConnectPosition;
            Point endP = endItem.LeftConnectPosition;
            double tmpLen = 0;

            // 開始ジョブの左点-----------
            // 左⇒左
            double minLen = GetLength(startP, endP);

            //flow.BeginConType = ConnectType.LEFT;
            //flow.EndConType = ConnectType.LEFT;

            // 左⇒右
            tmpLen = GetLength(beginItem.LeftConnectPosition, endItem.RightConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                endP = endItem.RightConnectPosition;
                //flow.EndConType = ConnectType.RIGHT;
            }
            // 左⇒上
            tmpLen = GetLength(beginItem.LeftConnectPosition, endItem.TopConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                endP = endItem.TopConnectPosition;
                //flow.EndConType = ConnectType.TOP;
            }
            // 左⇒下
            tmpLen = GetLength(beginItem.LeftConnectPosition, endItem.BottomConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                endP = endItem.BottomConnectPosition;
                //flow.EndConType = ConnectType.BOTTOM;
            }

            // 開始ジョブの右点-----------
            // 右⇒左
            tmpLen = GetLength(beginItem.RightConnectPosition, endItem.LeftConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.RightConnectPosition;
                endP = endItem.LeftConnectPosition;
                //flow.BeginConType = ConnectType.RIGHT;
                //flow.EndConType = ConnectType.LEFT;
            }
            // 右⇒右
            tmpLen = GetLength(beginItem.RightConnectPosition, endItem.RightConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.RightConnectPosition;
                endP = endItem.RightConnectPosition;
                //flow.BeginConType = ConnectType.RIGHT;
                //flow.EndConType = ConnectType.RIGHT;
            }
            // 右⇒上
            tmpLen = GetLength(beginItem.RightConnectPosition, endItem.TopConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.RightConnectPosition;
                endP = endItem.TopConnectPosition;
                //flow.BeginConType = ConnectType.RIGHT;
                //flow.EndConType = ConnectType.TOP;
            }
            // 右⇒下
            tmpLen = GetLength(beginItem.RightConnectPosition, endItem.BottomConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.RightConnectPosition;
                endP = endItem.BottomConnectPosition;
                //flow.BeginConType = ConnectType.RIGHT;
                //flow.EndConType = ConnectType.BOTTOM;
            }

            // 開始ジョブの上点-----------
            // 上⇒左
            tmpLen = GetLength(beginItem.TopConnectPosition, endItem.LeftConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.TopConnectPosition;
                endP = endItem.LeftConnectPosition;
                //flow.BeginConType = ConnectType.TOP;
                //flow.EndConType = ConnectType.LEFT;
            }
            // 上⇒右
            tmpLen = GetLength(beginItem.TopConnectPosition, endItem.RightConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.TopConnectPosition;
                endP = endItem.RightConnectPosition;
                //flow.BeginConType = ConnectType.TOP;
                //flow.EndConType = ConnectType.RIGHT;
            }
            // 上⇒上
            tmpLen = GetLength(beginItem.TopConnectPosition, endItem.TopConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.TopConnectPosition;
                endP = endItem.TopConnectPosition;
                //flow.BeginConType = ConnectType.TOP;
                //flow.EndConType = ConnectType.TOP;
            }
            // 上⇒下
            tmpLen = GetLength(beginItem.TopConnectPosition, endItem.BottomConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.TopConnectPosition;
                endP = endItem.BottomConnectPosition;
                //flow.BeginConType = ConnectType.TOP;
                //flow.EndConType = ConnectType.BOTTOM;
            }

            // 開始ジョブの下点-----------
            // 下⇒左
            tmpLen = GetLength(beginItem.BottomConnectPosition, endItem.LeftConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.BottomConnectPosition;
                endP = endItem.LeftConnectPosition;
                //flow.BeginConType = ConnectType.BOTTOM;
                //flow.EndConType = ConnectType.LEFT;
            }
            // 下⇒右
            tmpLen = GetLength(beginItem.BottomConnectPosition, endItem.RightConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.BottomConnectPosition;
                endP = endItem.RightConnectPosition;
                //flow.BeginConType = ConnectType.BOTTOM;
                //flow.EndConType = ConnectType.RIGHT;
            }
            // 下⇒上
            tmpLen = GetLength(beginItem.BottomConnectPosition, endItem.TopConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.BottomConnectPosition;
                endP = endItem.TopConnectPosition;
                //flow.BeginConType = ConnectType.BOTTOM;
                //flow.EndConType = ConnectType.TOP;
            }
            // 下⇒下
            tmpLen = GetLength(beginItem.BottomConnectPosition, endItem.BottomConnectPosition);
            if (minLen > tmpLen)
            {
                minLen = tmpLen;
                startP = beginItem.BottomConnectPosition;
                endP = endItem.BottomConnectPosition;
                //flow.BeginConType = ConnectType.BOTTOM;
                //flow.EndConType = ConnectType.BOTTOM;
            }
            list.Add(startP);
            list.Add(endP);

            return list;

            // 角度の方法（2012.11.1削除）

            //Point centerP1 = beginItem.CenterPoint;

            //Point centerP2 = endItem.CenterPoint;

            //double gapX = centerP1.X - centerP2.X;
            //double gapY = centerP1.Y - centerP2.Y;

            //double angle = gapY / gapX;

            //// 左
            //if (gapX > 0 && angle > -1 && angle < 1)
            //{
            //    list.Add(beginItem.LeftConnectPosition);
            //    list.Add(endItem.RightConnectPosition);

            //    flow.BeginConType = ConnectType.LEFT;
            //    flow.EndConType = ConnectType.RIGHT;
            //}
            //// 右
            //else if (gapX < 0 && angle > -1 && angle < 1)
            //{
            //    list.Add(beginItem.RightConnectPosition);
            //    list.Add(endItem.LeftConnectPosition);

            //    flow.BeginConType = ConnectType.RIGHT;
            //    flow.EndConType = ConnectType.LEFT;
            //}
            //// 上

            //else if (gapY > 0 && (angle > 1 || angle < -1))
            //{
            //    list.Add(beginItem.TopConnectPosition);
            //    list.Add(endItem.BottomConnectPosition);

            //    flow.BeginConType = ConnectType.TOP;
            //    flow.EndConType = ConnectType.BOTTOM;
            //}
            //// 下

            //else if (gapY < 0 && (angle > 1 || angle < -1))
            //{
            //    list.Add(beginItem.BottomConnectPosition);
            //    list.Add(endItem.TopConnectPosition);

            //    flow.BeginConType = ConnectType.BOTTOM;
            //    flow.EndConType = ConnectType.TOP;
            //}

        }

        /// <summary>線の連接点を取得</summary>
        /// <param name="beginItem">開始ジョブ</param>
        /// <param name="endItem">終了ジョブ</param>
        /// <param name="arrow">フロー</param>
        public static List<Point> GetConnectPointsForCurve(IRoom beginItem, IRoom endItem)
        {
            List<Point> list = new List<Point>();

            Point centerP1 = beginItem.CenterPoint;
            Point centerP2 = endItem.CenterPoint;

            double gapX = centerP1.X - centerP2.X;
            double gapY = centerP1.Y - centerP2.Y;

            double angle = gapY / gapX;

            // 右⇒左
            if (gapX > 0 && angle > -1 && angle < 1)
            {
                list.Add(beginItem.BottomConnectPosition);
                list.Add(endItem.BottomConnectPosition);

            }
            // 左⇒右
            else if (gapX < 0 && angle > -1 && angle < 1)
            {
                list.Add(beginItem.TopConnectPosition);
                list.Add(endItem.TopConnectPosition);
            }
            // 下⇒上
            else if (gapY > 0 && (angle > 1 || angle < -1))
            {
                list.Add(beginItem.LeftConnectPosition);
                list.Add(endItem.LeftConnectPosition);
            }
            // 上⇒下
            else if (gapY < 0 && (angle > 1 || angle < -1))
            {
                list.Add(beginItem.RightConnectPosition);
                list.Add(endItem.RightConnectPosition);
            }

            return list;
        }

        /// <summary>二つ点の間隔を取得</summary>
        /// <param name="p1">開始点</param>
        /// <param name="p2">終了点</param>
        /// <returns></returns>
        public static double GetLength(Point p1, Point p2)
        {
            if (p1 == null || p2 == null)
                return 0;
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        /// <summary>省略の文字列を取得</summary>
        /// <param name="input">入力文字列</param>
        /// <param name="maxByteLen">最大のバイト数</param>
        /// <returns></returns>
        public static String GetOmitString(String input, int maxByteLen)
        {
            // 変換対象文字列のバイト数を求める

            int inputByteLength = CheckUtil.Get_ByteLength(input);

            // 変換対象のバイト数が、保持するバイト数以内に
            // 収まっている場合は、そのままの文字列を返す
            if (inputByteLength <= maxByteLen)
            {
                return input;
            }
            // 保持バイト長
            int keepByteLength = maxByteLen - 3;

            // バイト変換した文字列の桁数が、指定バイト数を上回る箇所を判定

            for (int i = 0; i <= input.Length; i++)
            {
                int checkStrByteLength = CheckUtil.Get_ByteLength(input.Substring(0, i));

                if (keepByteLength < checkStrByteLength)
                {
                    // 変換文字のバイト数が指定バイト数を上回る位置に来たら、
                    // その手前までの文字列(+略号)を返す
                    return input.Substring(0, i - 1) + SystemConst.SUFFIX_JOB;
                }
            }
            return input;
        }

        /// <summary>フロー管理のデータを更新(ジョブＩＤ変更有の場合)</summary>
        /// <param name="flowTbl">フロー管理テーブル</param>
        /// <param name="oldJobId">古いジョブID</param>
        /// <param name="jobId">新しいジョブID</param>
        public static void UpdateFlowForJobId(DataTable flowTbl, string oldJobId, string newJobId)
        {
            // INフローを更新
            DataRow[] rows = flowTbl.Select("end_job_id='" + oldJobId + "'");

            foreach (DataRow row in rows)
                row["end_job_id"] = newJobId;

            // OUTフローを更新
            rows = flowTbl.Select("start_job_id='" + oldJobId + "'");

            foreach (DataRow row in rows)
                row["start_job_id"] = newJobId;
        }

        #endregion
    }
}
