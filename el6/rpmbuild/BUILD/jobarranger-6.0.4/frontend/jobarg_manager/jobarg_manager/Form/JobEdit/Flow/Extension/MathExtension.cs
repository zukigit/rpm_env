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
using System.Windows.Media;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/20 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>曲線計算用の数学クラス</summary>
    public class MathExtension
    {
        /// <summary>直線の方向を取得</summary>
        /// <param name="startPoint">開始点</param>
        /// <param name="endPoint">終了点</param>
        /// <returns>方向の区域</returns>
        public static ExtentDirectionEnum GetExtentDirection(Point startPoint, Point endPoint)
        {
            double xOffset = startPoint.X - endPoint.X;
            double yOffset = startPoint.Y - endPoint.Y;

            if (xOffset != 0 && yOffset != 0)
                if (xOffset > 0)
                    if (yOffset > 0)
                        return ExtentDirectionEnum.Quadrant1;
                    else
                        return ExtentDirectionEnum.Quadrant4;
                else
                    if (yOffset > 0)
                        return ExtentDirectionEnum.Quadrant2;
                    else
                        return ExtentDirectionEnum.Quadrant3;
            else if (xOffset == 0 && yOffset != 0)
                if (yOffset > 0)
                    return ExtentDirectionEnum.YB;
                else
                    return ExtentDirectionEnum.YL;
            else if (xOffset != 0 && yOffset == 0)
                if (xOffset > 0)
                    return ExtentDirectionEnum.XB;
                else
                    return ExtentDirectionEnum.XL;
            else
                return ExtentDirectionEnum.Origin;
        }

        /// <summary>>開始点、終了点、弧の半径、描画方向,大小弧によって返回円の中心点を取得</summary>
        /// <param name="startPoint">開始点</param>
        /// <param name="endPoint">終了点</param>
        /// <param name="radius">半径</param>
        /// <param name="sweepDirection">弧が描画される方向</param>
        /// <param name="isLargeArc">True:大弧,False:小弧 </param>
        /// <returns>円の中心点</returns>
        public static Point GetRoundCenter(Point startPoint, Point endPoint, double radius,
            SweepDirection sweepDirection, bool isLargeArc)
        {
            // 開始と終了点が同じの場合
            if (startPoint == endPoint)
                throw new BaseException(Consts.ERROR_JOBEDIT_007);

            //開始点と終了点の長さ(※現在の実装方法が不可能)
            double width = Math.Sqrt((startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y) +
                (startPoint.X - endPoint.X) * (startPoint.X - endPoint.X));

            if (width > radius*2)
                throw new RadiusException("");

            //交差点
            Point crossingPoint = new Point((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);

            //開始点と終了点の傾度
            double k1 = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);

            //交差点と円の中心の傾度
            double k2;

            //交差点と円の中心の長さ
            double distance;
            distance = Math.Sqrt(radius * radius - (width * width) / 4);

            //円の中心
            Point roundCenterLeft = new Point();
            Point roundCenterRight = new Point();

            // 傾度が無限大の場合(開始点と終了点のＹ座標が同じ)
            if (double.IsInfinity(k1))
            {
                roundCenterLeft = new Point(crossingPoint.X + distance, crossingPoint.Y);
                roundCenterRight = new Point(crossingPoint.X - distance, crossingPoint.Y);
            }
            // 傾度が0の場合(開始点と終了点のＸ座標が同じ)
            else if (k1 == 0)
            {
                roundCenterLeft = new Point(crossingPoint.X, crossingPoint.Y + distance);
                roundCenterRight = new Point(crossingPoint.X, crossingPoint.Y - distance);
            }
            // その他の場合
            else
            {
                k2 = (-1) / k1;
                double xOffset = distance / (Math.Sqrt(1 + k2 * k2));

                double xTemp, yTemp;
                #region 一番目の円の中心点
                xTemp = crossingPoint.X + xOffset;
                yTemp = k2 * (xTemp - crossingPoint.X) + crossingPoint.Y;
                roundCenterLeft = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));
                #endregion

                #region ニ番目の円の中心点
                xTemp = crossingPoint.X - xOffset;
                yTemp = k2 * (xTemp - crossingPoint.X) + crossingPoint.Y;
                roundCenterRight = new Point(Math.Round(xTemp, 4), Math.Round(yTemp, 4));
                #endregion
            }

            if (-1 == MathExtension.GetPointLeftOrRight(startPoint, endPoint, roundCenterLeft))
            {
                Point pointTemp = roundCenterLeft;
                roundCenterLeft = roundCenterRight;
                roundCenterRight = pointTemp;
            }

            //大弧
            if (isLargeArc)
            {
                //時計回り (正の角) 、 右の円の中心を取得
                if (sweepDirection == SweepDirection.Clockwise)
                {
                    return roundCenterRight;
                }
                // 反時計回り (負の角) 、 左の円の中心を取得
                else
                {
                    return roundCenterLeft;
                }
            }
            //小弧
            else
            {
                //時計回り (正の角) 、 左の円の中心を取得
                if (sweepDirection == SweepDirection.Clockwise)
                {
                    return roundCenterLeft;
                }
                // 反時計回り (負の角) 、 右の円の中心を取得
                else
                {
                    return roundCenterRight;
                }
            }
        }

        /// <summary>点が直线の左または右を判定</summary>
        /// <param name="startPoint">開始点</param>
        /// <param name="endPoint">終了点</param>
        /// <param name="point">判定点</param>
        /// <returns>0:直線内、1：左、-1右 </returns>
        public static int GetPointLeftOrRight(Point startPoint, Point endPoint, Point point)
        {
            double t;
            startPoint.X -= point.X;
            startPoint.Y -= point.Y;

            endPoint.X -= point.X;
            endPoint.Y -= point.Y;

            t = startPoint.X * endPoint.Y - startPoint.Y * endPoint.X;
            return t == 0 ? 0 : t < 0 ? -1 : 1;
        }
    }
}
