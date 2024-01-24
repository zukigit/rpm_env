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
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 偉 2012/10/27 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>曲線計算用の列挙型</summary>
    public enum ExtentDirectionEnum
    {
        /// <summary>第一象限</summary>
        Quadrant1 = 0,

        /// <summary>第二象限</summary>
        Quadrant2 = 1,

        /// <summary>第三象限</summary>
        Quadrant3 = 2,

        /// <summary>第四象限</summary>
        Quadrant4 = 3,

        /// <summary>X軸の正の部分</summary>
        XB = 4,

        /// <summary>X軸の負の部分</summary>
        XL = 5,

        /// <summary>Y軸の正の部分</summary>
        YB = 6,

        /// <summary>Y軸の負の部分</summary>
        YL = 7,

        /// <summary>原点</summary>
        Origin = 9
    }
}
